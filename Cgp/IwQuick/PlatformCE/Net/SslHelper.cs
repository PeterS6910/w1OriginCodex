using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Globalization;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// Class for create ssl socket.
    /// </summary>
    public static class SslHelper
    {
        private const ushort SO_SECURE = 0x2001;
        private const ushort SO_SEC_SSL = 0x2004;
        private const int _SO_SSL_FLAGS = 0x02;
        private const int _SO_SSL_VALIDATE_CERT_HOOK = 0x08;
        private const int SO_SSL_FAMILY = 0x00730000;
        private const long _SO_SSL = ((2L << 27) | SO_SSL_FAMILY);
        private const uint IOC_IN = 0x80000000;
        private const long SO_SSL_SET_VALIDATE_CERT_HOOK = (IOC_IN | _SO_SSL | _SO_SSL_VALIDATE_CERT_HOOK);
        private const long SO_SSL_SET_FLAGS = (IOC_IN | _SO_SSL | _SO_SSL_FLAGS);
        private const int SSL_FLAG_DEFER_HANDSHAKE = 0x0008;
        private const int _SO_SSL_PERFORM_HANDSHAKE = 0x0d;
        private const long SO_SSL_PERFORM_HANDSHAKE = _SO_SSL | _SO_SSL_PERFORM_HANDSHAKE;

        public const int SSL_CERT_X59 = 1;
        public const int SSL_ERR_OKAY = 0;
        public const int SSL_ERR_FAILED = 2;
        public const int SSL_ERR_BAD_LEN = 3;
        public const int SSL_ERR_BAD_TYPE = 4;
        public const int SSL_ERR_BAD_DATA = 5;
        public const int SSL_ERR_NO_CERT = 6;
        public const int SSL_ERR_BAD_SIG = 7;
        public const int SSL_ERR_CERT_EXPIRED = 8;
        public const int SSL_ERR_CERT_REVOKED = 9;
        public const int SSL_ERR_CERT_UNKNOWN = 10;
        public const int SSL_ERR_SIGNATURE = 11;
        public const int SSL_CERT_FLAG_ISSUER_UNKNOWN = 0x0001;

        private static void WriteASCIIString(IntPtr basePtr, string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            for (int i = 0; i < bytes.Length; i++)
                Marshal.WriteByte(basePtr, i, bytes[i]);

            //null terminate the string
            Marshal.WriteByte(basePtr, bytes.Length, 0);
        }


        /// <summary>
        /// Function for create ssl socket.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="certificateValidationHook"></param>
        /// <param name="deferHandshake"></param>
        public static void SetSslSocket(
            [NotNull] Socket socket,
            [NotNull] DRemoteCertificateValidation certificateValidationHook, 
            bool deferHandshake)
        {
            Validator.CheckForNull(socket,"socket");
            Validator.CheckForNull(certificateValidationHook,"certificateValidationHook");

            IntPtr hookFunc = Marshal.GetFunctionPointerForDelegate(new DRemoteCertificateValidation(certificateValidationHook));

            //The managed SocketOptionName enum doesn't have SO_SECURE so here we cast the integer value
            socket.SetSocketOption(SocketOptionLevel.Socket, (SocketOptionName)SO_SECURE, SO_SEC_SSL);


            //Allocate the buffer for the string
            IntPtr ptrHost = (IntPtr)0;

            //Now put both pointers into a byte[]
            var inBuffer = new byte[8];
            var hookFuncBytes = BitConverter.GetBytes(hookFunc.ToInt32());
            var hostPtrBytes = BitConverter.GetBytes(ptrHost.ToInt32());
            Array.Copy(hookFuncBytes, inBuffer, hookFuncBytes.Length);
            Array.Copy(hostPtrBytes, 0, inBuffer, hookFuncBytes.Length, hostPtrBytes.Length);

            unchecked
            {
                socket.IOControl((int)SO_SSL_SET_VALIDATE_CERT_HOOK, inBuffer, null);
                if (deferHandshake)
                    socket.IOControl((int)SO_SSL_SET_FLAGS, BitConverter.GetBytes(SSL_FLAG_DEFER_HANDSHAKE), null);
            }
        }

        /// <summary>
        /// Function for validate certificate.
        /// </summary>
        /// <param name="dwType"></param>
        /// <param name="host"></param>
        /// <param name="dwChainLen"></param>
        /// <param name="pCertChain"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        public static int ValidateCert(uint dwType, string host, uint dwChainLen, IntPtr pCertChain, uint dwFlags)
        {
            //According to http://msdn.microsoft.com/en-us/library/ms940451.aspx:
            //
            //- dwChainLen is always 1
            //- Windows CE performs the cert chain validation
            //- pvArg is the context data we passed into the SO_SSL_SET_VALIDATE_CERT_HOOK call so in our
            //- case is the host name
            //
            //So here we are responsible for validating the dates on the certificate and the CN

            if (dwType != SSL_CERT_X59)
                return SSL_ERR_BAD_TYPE;

            //When in debug mode let self-signed certificates through ...
            //if ((dwFlags & SSL_CERT_FLAG_ISSUER_UNKNOWN) != 0)
            //return SSL_ERR_CERT_UNKNOWN;

            //Note about the note: an unmanaged long is 32 bits, unlike a managed long which is 64. I was missing
            //this fact when I wrote the comment. So the docs are accurate.
            //NOTE: The documentation says pCertChain is a pointer to a LPBLOB struct:
            //
            // {ulong size, byte* data}
            //
            //in reality the size is a 32 bit integer (not 64).
            int certSize = Marshal.ReadInt32(pCertChain);
            IntPtr pData = Marshal.ReadIntPtr(new IntPtr(pCertChain.ToInt32() + sizeof(int)));

            byte[] certData = new byte[certSize];

            for (int i = 0; i < certSize; i++)
                certData[i] = Marshal.ReadByte(pData, (int)i);

            X509Certificate cert;
            try
            {
                cert = new X509Certificate(certData);
            }
            catch (ArgumentException) { return SSL_ERR_BAD_DATA; }
            catch (CryptographicException) { return SSL_ERR_BAD_DATA; }

            //Validate the expiration date
            if (DateTime.Now > DateTime.Parse(cert.GetExpirationDateString(), CultureInfo.CurrentCulture))
                return SSL_ERR_CERT_EXPIRED;

            //Validate the effective date
            //if (DateTime.Now < DateTime.Parse(cert.GetEffectiveDateString(), CultureInfo.CurrentCulture))
            //    return SSL_ERR_FAILED;

            string certName = cert.GetName();
            certName = certName.Substring(certName.IndexOf("CN=") + 3);

            //Validate the name
            if (host != null && host.Length != 0 && host != certName)
                return SSL_ERR_FAILED;

            return SSL_ERR_OKAY;
        }

        public static void DoHandshake(Socket socket)
        {
            //Allocate the buffer for the string
            IntPtr ptrHost;// = (IntPtr)0;
            ptrHost = Marshal.AllocHGlobal(10 + 1);
            //WriteASCIIString(ptrHost, host);

            socket.IOControl((int)SO_SSL_PERFORM_HANDSHAKE, null, null);
        }

        private static string ReadAnsiString(IntPtr pvArg)
        {
            byte[] buffer = new byte[1024];
            int j = 0;
            do
            {
                buffer[j] = Marshal.ReadByte(pvArg, j);
                j++;
            } while (buffer[j - 1] != 0);
            string host = Encoding.ASCII.GetString(buffer, 0, j - 1);
            return host;
        }
    }
}
