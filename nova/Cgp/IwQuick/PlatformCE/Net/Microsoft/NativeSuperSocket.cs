using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using JetBrains.Annotations;


namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public enum SSLVersion
    {
        /// <summary>
        /// 
        /// </summary>
        TLS = 1,
        /// <summary>
        /// 
        /// </summary>
        SSL2 = 2,
        /// <summary>
        /// 
        /// </summary>
        SSL3 = 3,
        /// <summary>
        /// 
        /// </summary>
        Auto = 0
    }

    /// <summary>
    /// wrapping class for NativeSuperSocket.dll
    /// </summary>
    public static class NativeSuperSocket
    {
        #region NativeSuperSocket interop bindings

        private const string DLL_PATH = "NativeSuperSocket.dll";

        //[DllImport(DLL_PATH)]
        //private static extern void NativeSuperSocket_Open();

        //[DllImport(DLL_PATH)]
        //private static extern void NativeSuperSocket_Close();

        [DllImport(DLL_PATH)]
        private static extern IntPtr NativeSuperSocket_CreateSocket4(int isTcp, int useSSL);

        [DllImport(DLL_PATH)]
        private static extern IntPtr NativeSuperSocket_CreateSocket4Specific(int isTcp, int useSSL, int sslVersion);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSocket_Bind4(IntPtr socket, uint ipBytes, int port, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSocket_Listen(IntPtr socket, int backLog, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern IntPtr NativeSuperSocket_Accept(IntPtr socket, ref uint ipBytes, ref int port, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSocket_Connect4(IntPtr socket, uint remoteIpBytes, int remotePort, int timeout, ref uint localIpBytes, ref int localPort,  ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSocket_Send(IntPtr socket, byte[] buffer, int length, int timeout, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSocket_Send(IntPtr socket, IntPtr buffer, int length, int timeout, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSocket_Receive(IntPtr socket, byte[] buffer, int maxLength, int timeout, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSocket_Receive(IntPtr socket, IntPtr buffer, int maxLength, int timeout, ref int resultCode);

        [DllImport(DLL_PATH)]
        private static extern int NativeSuperSocket_CloseSocket(IntPtr socket);

        #endregion

        internal static IntPtr CreateSocket(bool isTcp, bool useSSL)
        {
            try
            {
                //EnsureOpen();

                int useSSLint = (useSSL) ? 1 : 0;

                IntPtr socketHandle = NativeSuperSocket_CreateSocket4(isTcp ? 1 : 0, useSSLint);

                return socketHandle;
            }
            catch(Exception e)
            {
                throw new IOException("Unable to allocate socket",e);
            }
        }

        internal static IntPtr CreateSocket(bool isTcp, bool useSSL, SSLVersion sslVersion)
        {
            try
            {
                IntPtr socketHandle = NativeSuperSocket_CreateSocket4Specific(isTcp ? 1 : 0, useSSL ? 1 : 0, (int)sslVersion);
                return socketHandle;
            }
            catch (Exception e)
            {
                throw new IOException("Unable to allocate socket", e);
            }
        }

        internal static void Connect(IntPtr socketHandle, IPEndPoint remoteIpEndpoint, int timeout, ref IPEndPoint localIpEndpoint)
        {
            int resultCode = -1;

            try
            {
                uint remoteIpBytes = BitConverter.ToUInt32(remoteIpEndpoint.Address.GetAddressBytes(),0);

                uint localIpBytes = 0;
                int localPort = 0;


                // TODO error checking
                NativeSuperSocket_Connect4(socketHandle, remoteIpBytes, remoteIpEndpoint.Port, timeout, ref localIpBytes, ref localPort,  ref resultCode);

                if (null == localIpEndpoint)
                    localIpEndpoint = new IPEndPoint(localIpBytes, localPort);
                else
                {
                    localIpEndpoint.Address = new IPAddress(localIpBytes);
                    localIpEndpoint.Port = localPort;
                }

            }
            catch
            {
                throw new IOException("Unable to connect socket");
            }

            if (resultCode != 0)
            {
                throw new SocketException(resultCode);
            }
        }

        internal static void Bind(IntPtr socketHandle, IPEndPoint localIpEndpoint)
        {
            int resultCode = -1;

            try
            {
                uint localIpBytes = BitConverter.ToUInt32(localIpEndpoint.Address.GetAddressBytes(), 0);

                // TODO error checking
                NativeSuperSocket_Bind4(socketHandle, localIpBytes, localIpEndpoint.Port, ref resultCode);
            }
            catch(Exception e)
            {
                throw new IOException("Unable to connect socket",e);
            }

            if (resultCode != 0)
            {
                throw new SocketException(resultCode);
            }
        }

        internal static void Listen(IntPtr socketHandle, int backLog)
        {
            int resultCode = -1;

            try
            {
                NativeSuperSocket_Listen(socketHandle, backLog, ref resultCode);
            }
            catch(Exception e)
            {
                throw new IOException("Unknown error calling listen",e);
            }

            if (resultCode != 0)
                throw new SocketException(resultCode);
        }

        internal static IntPtr Accept(IntPtr socketHandle, out IPEndPoint ipEndpoint)
        {
            try
            {
                int resultCode = -1;
                uint ipAB = 0;
                int port = 0;
                IntPtr clientSocket = NativeSuperSocket_Accept(socketHandle, ref ipAB, ref port, ref resultCode);

                if (resultCode != 0)
                    throw new SocketException(resultCode);

                IPAddress ip = new IPAddress(ipAB);
                ipEndpoint = new IPEndPoint(ip, port);

                return clientSocket;
            }
            catch (Exception e)
            {                
                throw new IOException("Unknown error calling accept", e);
            }

            
        }

        internal static int Send(IntPtr socketHandle, ByteDataCarrier data, int timeout)
        {
            var resultCode = -1;
            int bytesSend;

            try
            {
                bytesSend = NativeSuperSocket_Send(socketHandle, data.Buffer, data.ActualSize, timeout, ref resultCode);
            }
            catch (Exception e)
            {
                throw new IOException("Unknown error while sending data", e);
            }

            if (resultCode != 0)
                throw new SocketException(resultCode);

            return bytesSend;
        }

        internal static int Send(IntPtr socketHandle, byte[] buffer, int offset, int count, int timeout)
        {
            int resultCode = -1;
            int bytesSend;

            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr bufferAddress = gcHandle.AddrOfPinnedObject();

            try
            {
                bytesSend =
                    NativeSuperSocket_Send(
                        socketHandle,
                        IntPtr.Size == 4
                            ? new IntPtr(bufferAddress.ToInt32() + offset)
                            : new IntPtr(bufferAddress.ToInt64() + offset),
                        count,
                        timeout,
                        ref resultCode);
            }
            catch (Exception e)
            {
                throw new IOException("Unknown error while sending data", e);
            }
            finally
            {
                gcHandle.Free();
            }

            if (resultCode != 0)
                throw new SocketException(resultCode);

            return bytesSend;
        }

        internal static int Receive(
            IntPtr socketHandle, 
            [NotNull] ByteDataCarrier data, 
            int timeout)
        {
            Validator.CheckForNull(data,"data");
            Validator.CheckZero(data.Size);

            int resultCode = -1;
            int bytesRead;
            try
            {
                bytesRead = NativeSuperSocket_Receive(socketHandle, data.Buffer, data.Buffer.Length, timeout, ref resultCode);
                
            }
            catch (Exception error)
            {
                throw new IOException("Error while reading data from socket", error);
            }

            if (resultCode != 0)
                throw new SocketException(resultCode);

            data.ActualSize = bytesRead;
            return bytesRead;
        }

        internal static int Receive(
            IntPtr socketHandle, 
            [NotNull] byte[] buffer,
            int offset, 
            int count, 
            int timeout)
        {
            Validator.CheckForNull(buffer,"buffer");
            Validator.CheckZero(count);

            int resultCode = -1;
            int bytesRead;
            GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr bufferAddress = gcHandle.AddrOfPinnedObject();

            try
            {
                bytesRead =
                    NativeSuperSocket_Receive(
                        socketHandle,
                        IntPtr.Size == 4 
                            ? new IntPtr(bufferAddress.ToInt32() + offset)
                            : new IntPtr(bufferAddress.ToInt64() + offset),
                        count,
                        timeout,
                        ref resultCode);

            }
            catch (Exception error)
            {
                throw new IOException("Error while reading data from socket", error);
            }
            finally
            {
                gcHandle.Free();
            }

            if (resultCode != 0)
                throw new SocketException(resultCode);

            return bytesRead;
        }

        internal static void Close(IntPtr socketHandle)
        {
            int resultCode = NativeSuperSocket_CloseSocket(socketHandle);
            if (resultCode != 0)
            {
                throw new SocketException(resultCode);
            }
        }
    }
}
