using System;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// Class implement ssl stream
    /// </summary>
    public class SslStream : Stream
    {
        

        private const int BUFFERSIZE = 8192;
        
        private Socket _client = null;
        private Socket _server = null;
        private String _targetHost = String.Empty;
        private Stream _externalRealStream = null;
        private Stream _internalServerStream = null;
        private Stream _internalClientStream = null;
        private bool _connected = false;
        private DRemoteCertificateValidation _hookFunc = null;
        private int _bufferSize = BUFFERSIZE;
        private Exception _readException = null;
        private Exception _writeException = null;
        private Exception _acceptException = null;

        /// <summary>
        /// Create ssl stream
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <param name="bufferSize">Byffer size</param>
        public SslStream(Stream stream, int bufferSize)
        {
            CreateSslStream(stream, bufferSize);
        }

        /// <summary>
        /// Create ssl stream with default buffer size
        /// </summary>
        /// <param name="stream">Input stream</param>
        public SslStream(Stream stream)
        {
            CreateSslStream(stream, BUFFERSIZE);
        }

        /// <summary>
        /// Function for incialize
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <param name="bufferSize">Buffer size</param>
        private void CreateSslStream(Stream stream, int bufferSize)
        {
            _externalRealStream = stream;
            _bufferSize = bufferSize;
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Function for autentificate as client
        /// </summary>
        /// <param name="targetHost">Target host</param>
        /// <param name="func">Function for validate server certificate</param>
        public void AutentificateAsClient(string targetHost, DRemoteCertificateValidation func)
        {
            try
            {
                SslHelper.SetSslSocket(_client, new DRemoteCertificateValidation(ValidateCertHook), false);
                _targetHost = targetHost;
                _hookFunc = func;

                int port = TcpUdpPort.GetTcpFreeDescending(true);
                _server.Bind(new IPEndPoint(IPAddress.Loopback, port));
                _server.Listen(32);
                _server.BeginAccept(OnAccept, null);

                _client.Connect(new IPEndPoint(IPAddress.Loopback, port));
                _internalClientStream = new NetworkStream(_client);
                _connected = true;
            }
            catch (Exception exception)
            {
                _connected = false;
                Close();
                throw exception;
            }
        }

        /// <summary>
        /// Function for autentificate as client
        /// </summary>
        /// <param name="targetHost">Target host</param>
        public void AutentificateAsClient(string targetHost)
        {
            AutentificateAsClient(targetHost, null);
        }

        /// <summary>
        /// Function for validate server certificate
        /// </summary>
        /// <param name="dwType">Type of certificate</param>
        /// <param name="host">Target host</param>
        /// <param name="dwChainLen">Length of server certificate</param>
        /// <param name="pCertChain">IntPtr to server certificate</param>
        /// <param name="dwFlags">Input flags from server</param>
        /// <returns>If validate is ok then return SSL_ERR_OKAY</returns>
        private int ValidateCertHook(uint dwType, string host, uint dwChainLen, IntPtr pCertChain, uint dwFlags)
        {
            try
            {
                int retValue = 0;
                if (_hookFunc != null)
                {
                    retValue = _hookFunc(dwType, _targetHost, dwChainLen, pCertChain, dwFlags);
                }
                else
                {
                    retValue = SslHelper.ValidateCert(dwType, _targetHost, dwChainLen, pCertChain, dwFlags);
                }

                return retValue;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Accept internal client stream connection. Run begin read on internal sever stream and external real stream.
        /// </summary>
        /// <param name="result">Asynchronous result</param>
        private void OnAccept(IAsyncResult result)
        {
            try
            {
                _server = _server.EndAccept(result);
                _internalServerStream = new NetworkStream(_server);
                byte[] inputServer = new byte[_bufferSize];
                _internalServerStream.BeginRead(inputServer, 0, inputServer.Length, OnReadInternalServerStream, inputServer);
                byte[] inputSocket = new byte[_bufferSize];
                _externalRealStream.BeginRead(inputSocket, 0, inputSocket.Length, OnReadExternalRealStream, inputSocket);
            }
            catch (Exception exception)
            {
                _connected = false;
                Close();
                _acceptException = exception;
            }
        }

        /// <summary>
        /// Read data from internal sever stream and write them to external real stream
        /// </summary>
        /// <param name="result">Asynchronous result</param>
        private void OnReadInternalServerStream(IAsyncResult result)
        {
            try
            {
                byte[] output = result.AsyncState as byte[];
                int length = _internalServerStream.EndRead(result);
                
                if (length == 0)
                {
                    throw new ArgumentException("Internal client stream failed.");
                }

                _externalRealStream.Write(output, 0, length);
                byte[] input = new byte[_bufferSize];
                _internalServerStream.BeginRead(input, 0, input.Length, OnReadInternalServerStream, input);
            }
            catch (Exception exception)
            {
                _connected = false;
                Close();
                _writeException = exception;
            }
        }

        /// <summary>
        /// Read data from external real stream and write them to internal server stream
        /// </summary>
        /// <param name="result">Asynchronous result</param>
        private void OnReadExternalRealStream(IAsyncResult result)
        {
            try
            {
                byte[] output = result.AsyncState as byte[];
                int length = _externalRealStream.EndRead(result);

                if (length == 0)
                {
                    _connected = false;
                    Close();
                }
                else
                {
                    _internalServerStream.Write(output, 0, length);
                    byte[] input = new byte[_bufferSize];
                    if (!_client.Connected)
                    {
                        _externalRealStream.BeginRead(input, 0, input.Length, OnReadExternalRealStream, input);
                    }
                }
            }
            catch (Exception exception)
            {
                _connected = false;
                Close();
                _readException = exception;
            }
        }

        /// <summary>
        /// If is connected return CanRead from internal client stream
        /// </summary>
        public override bool CanRead
        {
            get
            {
                if (_internalClientStream == null)
                {
                    throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
                }
                else
                {
                    return _internalClientStream.CanRead && _connected;
                }
            }
        }

        /// <summary>
        /// Seek is not implemented
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                if (_internalClientStream == null)
                {
                    throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
                }

                return false;
            }
        }

        /// <summary>
        /// If is connected return CanWrite from internal client stream
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                if (_internalClientStream == null)
                {
                    throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
                }

                return _internalClientStream.CanWrite && _connected;
            }
        }

        /// <summary>
        /// Run SetLength on the internal client stream
        /// </summary>
        /// <param name="value">Length in bytes</param>
        public override void SetLength(long value)
        {
            if (_internalClientStream == null)
            {
                throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
            }

            if (!_connected)
            {
                throw new ArgumentException("SSL stream is not conected");
            }

            _internalClientStream.SetLength(value);
        }

        /// <summary>
        /// If is connected return Position from internal real stream
        /// </summary>
        public override long Position
        {
            get
            {
                if (_internalClientStream == null)
                {
                    throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
                }

                if (!_connected)
                {
                    throw new ArgumentException("SSL stream is not conected");
                }

                return _internalClientStream.Position;
            }
            set
            {
                throw new NotSupportedException("net_noseek");
            }
        }

        /// <summary>
        /// If is connected return Length from internal client stream
        /// </summary>
        public override long Length
        {
            get
            {
                if (_internalClientStream == null)
                {
                    throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
                }

                if (!_connected)
                {
                    throw new ArgumentException("SSL stream is not conected");
                }

                return _internalClientStream.Length;
            }
        }

        /// <summary>
        /// Seek is not implemented
        /// </summary>
        /// <param name="offset">Count of bytes to offset</param>
        /// <param name="origin">From where</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("net_noseek");
        }

        /// <summary>
        /// If is connected run Flush on the internal client stream
        /// </summary>
        public override void Flush()
        {
            if (_internalClientStream == null)
            {
                throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
            }

            if (!_connected)
            {
                throw new ArgumentException("SSL stream is not conected");
            }

            _internalClientStream.Flush();
        }

        /// <summary>
        /// Run BeginRead on the external real stream and read bytes from internal client stream
        /// </summary>
        /// <param name="buffer">Buffer for reading bytes</param>
        /// <param name="offset">Count of bytes to offset from begin of buffer</param>
        /// <param name="count">Count of bytes to read</param>
        /// <returns>Count of reading bytes</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_internalClientStream == null)
            {
                throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
            }

            if (!_connected)
            {
                throw new ArgumentException("SSL stream is not conected");
            }

            byte[] input = new byte[_bufferSize];
            _externalRealStream.BeginRead(input, 0, input.Length, OnReadExternalRealStream, input);
            return _internalClientStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Write bytes to internal client stream
        /// </summary>
        /// <param name="buffer">Buffer with bytes to write</param>
        /// <param name="offset">Count of bytes to offset from begin of buffer</param>
        /// <param name="count">Count of bytes to write</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_internalClientStream == null)
            {
                throw new ArgumentException("SslStream is not connected. Run method AutentifikateAsClient first");
            }

            if (!_connected)
            {
                throw new ArgumentException("SSL stream is not conected");
            }

            _internalClientStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Run BeginRead on the external real stream and BeginRead on the internal client stream
        /// </summary>
        /// <param name="buffer">Buffer for reading bytes</param>
        /// <param name="offset">Count of bytes to offset from begin of buffer</param>
        /// <param name="count">Count of bytes to read</</param>
        /// <param name="asyncCallback">This function is called when bytes are reading from stream</param>
        /// <param name="asyncState">Object for asyncState</param>
        /// <returns>Asynchronous result</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            if (_internalClientStream == null)
            {
                throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
            }

            if (!_connected)
            {
                throw new ArgumentException("SSL stream is not conected");
            }

            byte[] input = new byte[_bufferSize];
            _externalRealStream.BeginRead(input, 0, input.Length, OnReadExternalRealStream, input);
            return _internalClientStream.BeginRead(buffer, offset, count, asyncCallback, asyncState);
        }

        /// <summary>
        /// Run BeginWrite on the internal client stream
        /// </summary>
        /// <param name="buffer">Buffer with bytes to write</param>
        /// <param name="offset">Count of bytes to offset from begin of buffer</param>
        /// <param name="count">Count of bytes to write</param>
        /// <param name="asyncCallback">This function is called when bytes are writing to stream</param>
        /// <param name="asyncState">Object for asyncState</param>
        /// <returns>Asynchronous result</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            if (_internalClientStream == null)
            {
                throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
            }

            if (!_connected)
            {
                throw new ArgumentException("SSL stream is not conected");
            }

            return _internalClientStream.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
        }

        /// <summary>
        /// Run EndRead on the internal client stream
        /// </summary>
        /// <param name="asyncResult">Asynchronous result</param>
        /// <returns>Count of readed bytes</returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            if (_readException != null)
            {
                Exception readException = _readException;
                _readException = null;
                throw readException;
            }

            if (!_connected)
            {
                return 0;
            }

            if (_internalClientStream == null)
            {
                throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
            }

            return _internalClientStream.EndRead(asyncResult);
        }

        /// <summary>
        /// Run EndWrite on the internal client stream
        /// </summary>
        /// <param name="asyncResult">Asynchronous result</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (_writeException != null)
            {
                Exception writeException = _writeException;
                _writeException = null;
                throw writeException;
            }

            if (_internalClientStream == null)
            {
                throw new ArgumentException("SslStream is not connected. Run methode AutentifikateAsClient first");
            }

            if (!_connected)
            {
                throw new ArgumentException("SSL stream is not conected");
            }

            _internalClientStream.EndWrite(asyncResult);
        }

        /// <summary>
        /// Close the internal server stream and internal client stream
        /// </summary>
        public override void Close()
        {
            if (_internalServerStream != null)
            {
                _internalServerStream.Close();
            }

            if (_internalClientStream != null)
            {
                _internalClientStream.Close();
            }
        }
    }
}
