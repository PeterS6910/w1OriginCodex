using System;
using System.IO;
using System.Net.Sockets;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// Class for create ssl stream
    /// </summary>
    public class SslLayer : ITransportLayer
    {
        private SslSettings _sslSettings = null;
        private SimpleSslStream _sslStream = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sslSettings">Input settings</param>
        public SslLayer(SslSettings sslSettings)
        {
            _sslSettings = sslSettings;
        }

        /// <summary>
        /// Create ssl stream and authenticate as client
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return ssl stream</returns>
        public Stream PrepareClientStream(Socket socket)
        {

            return _sslStream;
            /*
            X509Certificate aPKI = null;
            if (null != _sslSettings.CertificatePath)
            {
                FileStream reader = new FileStream(_sslSettings.CertificatePath, FileMode.Open, FileAccess.Read);
                byte[] input = new byte[reader.Length];
                reader.Read(input, 0, input.Length);
                reader.Close();
                aPKI = new X509Certificate(input);
            }

            NetworkStream aNetworkStream = new NetworkStream(socket, false);

            return aNetworkStream;

            /*SslStream aSslStream = new SslStream(aNetworkStream);

            if (aPKI == null)
            {
                aSslStream.AutentificateAsClient(_sslSettings.TargetHost, ValidateCertificate);
            }
            else
            {
                string targetHost = aPKI.GetName();
                targetHost = targetHost.Substring(targetHost.IndexOf("CN=") + 3);
                aSslStream.AutentificateAsClient(targetHost, ValidateCertificate);
            }

            return aSslStream;*/
        }

        public Stream PrepareClientStream(ISuperSocket socket)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If ReverseAutentification is true than authenticate as client, else throw exception
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return ssl stream</returns>
        public Stream PrepareServerStream(Socket socket)
        {
            if (_sslSettings.ReverseAuthentication)
            {
                return PrepareClientStream(socket);
            }
            else
            {
                throw new ArgumentException("Authentification as server is not implemented");
            }
        }

        public Stream PrepareServerStream(ISuperSocket socket)
        {
            throw new NotImplementedException();
        }
        
        public void PreConnect(Socket socket)
        {
            if (null != _sslStream)
                _sslStream.Dispose();

            _sslStream = new SimpleSslStream(socket, _sslSettings);
        }

        public void PreConnect(ISuperSocket socket)
        {
            throw new NotImplementedException();
        }
    }
}
