using System;
using System.Net;
using System.Net.Sockets;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using JetBrains.Annotations;


namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class SuperSocket:ADisposable, ISuperSocket
    {
        /// pointer to ESOCKET not SOCKET!!
        private volatile IntPtr _socketHandle = IntPtr.Zero;                   
        private readonly object _socketHandleSync = new object();
        private int _receiveTimeout = 0; // makes receive call infinite
        private int _sendTimeout = 10000;
        private int _connectTimeout = 20000;
        private bool _useSSL = false;
        private SSLVersion _sslVersion = SSLVersion.Auto;
        private bool _socketConnected = false;

        /// <summary>
        /// 
        /// </summary>
        public SuperSocket()
        {
        }

        /// <summary>
        /// this is used usually via socket.accept routine
        /// </summary>
        /// <param name="newSocket"></param>
        /// <param name="remoteIpEndpoint"></param>
        private SuperSocket(IntPtr newSocket,IPEndPoint remoteIpEndpoint)
        {
            _socketHandle = newSocket;
            _remoteEndPoint = remoteIpEndpoint;

            _socketConnected = true;
        }

        #region Socket methods

        /// <summary>
        /// Connect to specified endpoint
        /// </summary>
        /// <param name="endPoint">IPEndPoint specifing remote host</param>
        public void Connect([NotNull] IPEndPoint endPoint)
        {
            Validator.CheckForNull(endPoint,"endPoint");

            if (_socketHandle == IntPtr.Zero)
                lock (_socketHandleSync)
                {
                    if (_socketHandle == IntPtr.Zero)
                        _socketHandle = NativeSuperSocket.CreateSocket(true, _useSSL, _sslVersion);
                    //_socketHandle = NativeSuperSocket.CreateSocket(true, _useSSL);
                }

            NativeSuperSocket.Connect(_socketHandle, endPoint, _connectTimeout, ref _localEndPoint);

            _socketConnected = true;

            _remoteEndPoint = endPoint;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndPoint"></param>
        public void Bind([NotNull] IPEndPoint ipEndPoint)
        {
            Validator.CheckForNull(ipEndPoint,"ipEndPoint");

            lock (_socketHandleSync)
            {
                if (_socketHandle == IntPtr.Zero)
                    _socketHandle = NativeSuperSocket.CreateSocket(true, _useSSL, _sslVersion);
                    //_socketHandle = NativeSuperSocket.CreateSocket(true, _useSSL);

            }
            NativeSuperSocket.Bind(_socketHandle, ipEndPoint);

            _localEndPoint = ipEndPoint;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Bind(IPAddress ip, int port)
        {
            Bind(new IPEndPoint(ip, port));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public void Bind(string ipAddress, int port)
        {
            Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        }

        private void ValidateSocket()
        {
            if (_socketHandle == IntPtr.Zero)
                throw new SocketException((int)SocketError.NotInitialized);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backLog"></param>
        public void Listen(int backLog)
        {
            ValidateSocket();

            NativeSuperSocket.Listen(_socketHandle, backLog);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Listen()
        {
            ValidateSocket();

            NativeSuperSocket.Listen(_socketHandle, 32);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SuperSocket Accept()
        {
            ValidateSocket();

            IPEndPoint ipE;
            IntPtr newSocket = NativeSuperSocket.Accept(_socketHandle, out ipE);

            return new SuperSocket(newSocket,ipE);
        }

        private IPEndPoint _remoteEndPoint = null;
        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return _remoteEndPoint;
            }
        }

        private IPEndPoint _localEndPoint = null;
        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return _localEndPoint;
            }
        }

        /// <summary>
        /// Connect to specified address and port
        /// </summary>
        /// <param name="ipAddress">IP address of the remote host</param>
        /// <param name="port">port of the remote host</param>
        public void Connect(IPAddress ipAddress, int port)
        {
            Connect(new IPEndPoint(ipAddress, port));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public void Connect(string ipAddress, int port)
        {
            Connect(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Send([NotNull] byte[] buffer, int offset, int count)
        {
            ValidateSocket();

            Validator.CheckNullAndEmpty(buffer,"buffer");

            if (count > buffer.Length)
                throw new ArgumentOutOfRangeException("length is greater than data.length");

            try
            {
                NativeSuperSocket.Send(_socketHandle, buffer, offset, count, _sendTimeout);
            }
            catch (SocketException)
            {
                if (_socketConnected)
                    _socketConnected = false;

                throw;
            }
        }

        /// <summary>
        /// Send data viac the socket
        /// </summary>
        /// <param name="data">byte array containing data</param>
        /// <param name="length">number of bytes to send</param>
        public void Send([NotNull] byte[] data, int length)
        {
            ValidateSocket();

            Validator.CheckNullAndEmpty(data,"data");
            if (length > data.Length)
                throw new ArgumentOutOfRangeException("length is greater than data.length");

            Send(new ByteDataCarrier(data, length));
        }

        /// <summary>
        /// Send data via the socket
        /// </summary>
        /// <param name="data">data object that contains the data to be sent</param>
        public void Send([NotNull] ByteDataCarrier data)
        {
            ValidateSocket();

            Validator.CheckForNull(data,"data");

            try
            {
                NativeSuperSocket.Send(_socketHandle, data, _sendTimeout);
            }
            catch(SocketException)
            {
                if (_socketConnected)
                    _socketConnected = false;

                throw;
            }
        }

        /// <summary>
        /// Receive data from the socket
        /// </summary>
        /// <param name="data">reference to object that will contain the data</param>
        /// <param name="receiveTimeout"></param>
        /// <returns>number of bytes read</returns>
        public int Receive([NotNull] ByteDataCarrier data, int receiveTimeout)
        {
            ValidateSocket();
            Validator.CheckForNull(data,"data");

            try
            {
                return NativeSuperSocket.Receive(_socketHandle, data, receiveTimeout != -1 ? receiveTimeout : _receiveTimeout);
            }
            catch (SocketException)
            {
                if (_socketConnected)
                    _socketConnected = false;

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Receive(ByteDataCarrier data)
        {
            return Receive(data, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Receive([NotNull] byte[] buffer, int offset, int count)
        {
            ValidateSocket();

            Validator.CheckNullAndEmpty(buffer,"buffer");
            Validator.CheckNegativeOrZeroInt(count);

            if (count > buffer.Length)
                count = buffer.Length;

            try
            {
                return NativeSuperSocket.Receive(_socketHandle, buffer, offset, count, _receiveTimeout);
            }
            catch (SocketException)
            {
                if (_socketConnected)
                    _socketConnected = false;

                throw;
            }
        }

        /// <summary>
        /// Receive data from the socket
        /// </summary>
        /// <param name="data">reference to byte array</param>
        /// <param name="length">maxlength of the data</param>
        /// <returns>number of bytes read</returns>
        public int Receive([NotNull] ref byte[] data, int length)
        {
            ValidateSocket();

            Validator.CheckNullAndEmpty(data,"data");
            Validator.CheckNegativeOrZeroInt(length);

            if (length > data.Length)
                length = data.Length;

            ByteDataCarrier byteDataCarrier = new ByteDataCarrier(data, length);

            int bytesRead = Receive(byteDataCarrier);

            byteDataCarrier.Buffer.CopyTo(data, 0);

            return bytesRead;
        }

        #endregion

        /// <summary>
        /// Disconnect socket
        /// </summary>
        public void Disconnect()
        {
            Close();
        }

        
        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            if (_socketHandle != IntPtr.Zero)
                lock (_socketHandleSync)
                {
                    if (_socketHandle != IntPtr.Zero)
                        try
                        {
                            NativeSuperSocket.Close(_socketHandle);
                            _socketHandle = IntPtr.Zero;
                        }
                        catch
                        {
                            _socketHandle = IntPtr.Zero;
                            throw;
                        }
                }
        }

        #region Socket Properties

        /// <summary>
        /// Indicates whether socket is already connected
        /// </summary>
        public bool Connected
        {
            get { return _socketConnected; }
        }

        /// <summary>
        /// SOCKET handle
        /// </summary>
        public IntPtr Handle
        {
            get { return _socketHandle; }
        }

        /// <summary>
        /// Timeout for receive operation in ms
        /// </summary>
        public int ReceiveTimeout
        {
            get { return _receiveTimeout; }
            set { _receiveTimeout = value; }
        }

        /// <summary>
        /// Timeout for send operation in ms
        /// </summary>
        public int SendTimeout
        {
            get { return _sendTimeout; }
            set { _sendTimeout = value; }
        }

        /// <summary>
        /// Timeout for connect operation in ms
        /// </summary>
        public int ConnectTimeout
        {
            get { return _connectTimeout; }
            set { _connectTimeout = value; }
        }

        /// <summary>
        /// Set whether SSL should be used
        /// </summary>
        public bool UseSSL
        {
            get { return _useSSL; }
            set { _useSSL = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public SSLVersion SSLVersion
        {
            get { return _sslVersion; }
            set { _sslVersion = value; }
        }

        #endregion



        

        /// <summary>
        /// 
        /// </summary>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            Close();
        }

    }
}
