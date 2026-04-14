using System;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// simple single connection TCP client
    /// with SSL support
    /// </summary>
    public class SimpleTcpClient : ASimpleTcpPeer
    {
        private volatile ITimer _connectionTimer;
        private readonly object _ctSync = new object();

        private const int DEFAULT_CONNECT_TIMEOUT = 3000;
        private const int MINIMAL_CONNECT_TIMEOUT = 50;

        private int _connectTimeout = DEFAULT_CONNECT_TIMEOUT;

        /// <summary>
        /// timeout for connect operation in miliseconds
        /// </summary>
        public int ConnectTimeout
        {
            get
            {
                return _connectTimeout;
            }
            set
            {
                if (value > 0)
                {
                    _connectTimeout = value < MINIMAL_CONNECT_TIMEOUT ? MINIMAL_CONNECT_TIMEOUT : value;
                }
                else
                    _connectTimeout = DEFAULT_CONNECT_TIMEOUT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="transportSettings"></param>
        public void Connect(
            [NotNull] IPAddress ip, 
            int port, 
            ITransportSettings transportSettings)
        {
            Validator.CheckForNull(ip,"ip");
            TcpUdpPort.CheckValidity(port);

            IPEndPoint ipE = new IPEndPoint(ip, port);

            Connect(ipE, transportSettings);

        }

        private bool StopPreviousConnectTimeout()
        {
            if (_connectionTimer != null)
                lock(_ctSync)
                    if (_connectionTimer != null)
                    {
                        _connectionTimer.StopTimer();
                        _connectionTimer = null;
                        return true;
                    }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="transportSettings"></param>
        public void Connect([NotNull] IPEndPoint ipEndpoint, ITransportSettings transportSettings)
        {
            Validator.CheckForNull(ipEndpoint,"ipEndpoint");

            bool alreadyConnected = true;

            if (null == _mainSocket)
            {
                lock (_msSync)
                    if (null == _mainSocket)
                    {
                        _mainSocket = new SuperSocket();

                        if (null != _transportSettings && null != _transportSettings.GetLayer())
                            try
                            {
                                _transportSettings.GetLayer().PreConnect(_mainSocket);
                            }
                            catch
                            {
                            }

                        alreadyConnected = false;
                    }
            }

            if (alreadyConnected)
                throw new InvalidOperationException("Client is either in connecting process or already connected");

            _transportSettings = transportSettings;

            try
            {
                _waitForConnectLock.Reset();

                lock (_ctSync)
                {
                    StopPreviousConnectTimeout();
                    _connectionTimer = TimerManager.Static.StartTimeout(_connectTimeout, ipEndpoint, OnConnectTimeout);
                }

                SafeThread<IPEndPoint>.StartThread(ConnectThread, ipEndpoint );

            }
            catch (Exception aError)
            {
                ConnectFailed(ipEndpoint, aError,true);
            }

        }

        private void ConnectFailed(IPEndPoint ipEndpoint, Exception error,bool verifyIfConnectTimeoutStopped)
        {
            if (!StopPreviousConnectTimeout() && verifyIfConnectTimeoutStopped)
                return;

            DisposeConnection(false);

            InvokeClientConnectionFailed(ipEndpoint, error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndPoint"></param>
        private void ConnectThread(IPEndPoint ipEndPoint)
        {

            try
            {
                _mainSocket.Connect(ipEndPoint);

                // do this ASAP
                if (!StopPreviousConnectTimeout())
                    // this should mean that OnConnectTimeout took place
                    return;
            }
            catch (Exception e)
            {
                ConnectFailed(ipEndPoint, e,true);
                return;
            }

            try
            {
                // this should not happen as by previous call [if (!StopPreviousConnectTimeout())]
                // the OnConnectTimeout should be blocked
                /*if (null == _mainSocket)
                    // this means, that onconnect was handled by the onconnecttimeout delegate
                    return;*/

                try
                {
                    lock (_msSync)
                    {
                        _connection = new SimpleGeneralTcpConnection(this, _mainSocket);

                        // indicate connection connected by lock
                        _waitForConnectLock.Set();
                    }

                    InvokeClientConnected();

                    _connection.StartReceive();
                }
                catch (Exception error)
                {
                    DisposeConnection(false);
                    InvokeClientConnectionFailed(ipEndPoint, error);
                }
            }
            catch (Exception error)
            {
                ConnectFailed(ipEndPoint, error,false);
            }
        }

        // the first instance of the lock is for cases, that Connect has not been invoked yet

        private readonly ManualResetEvent _waitForConnectLock = new ManualResetEvent(false);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForConnect(int timeout)
        {
            int t = timeout <= 0 ? _connectTimeout : timeout;

            bool retVal = _waitForConnectLock.WaitOne(t, false);

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool WaitForConnect()
        {
            return WaitForConnect(0);
        }

        /// <summary>
        /// connects to the specified remote side
        /// </summary>
        /// <param name="ip">string representation of the IP</param>
        /// <param name="port"></param>
        /// <param name="transportSettings"></param>
        public void Connect(string ip, int port, ITransportSettings transportSettings)
        {
            Validator.CheckNullString(ip);

            IPAddress aIP = IPHelper.Parse(ip);

            Connect(aIP, port, transportSettings);
        }

        private bool OnConnectTimeout(TimerCarrier timer)
        {
            lock (_ctSync)
            {
                if (_connectionTimer == null)
                    // timeout was stopped in last-moment
                    return true;

                // marks the fact the connecting process is handled
                _connectionTimer = null;
            }

            IPEndPoint ipEndpoint = null;
            if (timer != null && timer.Data is IPEndPoint)
                ipEndpoint = timer.Data as IPEndPoint;

            ConnectTimedOut(ipEndpoint);
            return true;

            /*lock (_msSync)
            {
                if (_waitForConnectLock.WaitOne(0, false))
                    return true;

                ConnectTimedOut(ipEndpoint);

                return true;
            }*/
        }

        private void ConnectTimedOut(IPEndPoint ipEndpoint)
        {
            DisposeConnection(false);
            InvokeClientConnectionFailed(ipEndpoint, new SocketException((int)SocketError.TimedOut));
        }

        private volatile SimpleGeneralTcpConnection _connection = null;

        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (_connection == null)
                    return null;

                lock (_msSync)
                {
                    if (_connection == null)
                        return null;
                    
                    return _connection.RemoteEndPoint;
                }
            }
        }

        
        /*
        private void OnConnected(IPEndPoint ipEndpoint)
        {
            // wait some time... it is supposed that the timer will start in specified time period, if not, ignore it and try to continue
            //_connectEventsSync.WaitOne(100, false);

            if (null == _mainSocket)
                // this means, that onconnect was handled by the onconnecttimeout delegate
                return;

            
        }*/

        /// <summary>
        /// disconnects the TCP connection
        /// </summary>
        public void Disconnect()
        {
            // not necessary, checked inside DisposeConnection
            /*if (_mainSocket == null)
                // client is not connected
                return;*/

            DisposeConnection(true);
        }

        


        /// <summary>
        /// event raised, when the TCP client become connected
        /// </summary>
        public event DSimpleTcpConnectionEvent Connected = null;

        private void InvokeClientConnected()
        {
            if (null != Connected)
                try
                {
                    //Log.Singleton.Error("TcpClient OnCOnnect call");
                    Connected(_connection);
                }
                catch
                {
                }
        }

        /// <summary>
        /// event raised, when the connecting will timeout or will fail other way
        /// </summary>
        public event DSimpleTcpConnectionFailed ConnectionFailed = null;

        private void InvokeClientConnectionFailed(IPEndPoint ipEndpoint, Exception exception)
        {
            if (null != ConnectionFailed)
                try
                {
                    ConnectionFailed(ipEndpoint, exception);
                }
                catch
                {
                }
        }

        protected internal override void InvokeSocketException(SimpleGeneralTcpConnection connection, SocketException socketException)
        {
            ExamineSocketError(socketException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketError"></param>
        private void ExamineSocketError(SocketException socketError)
        {
            DebugHelper.NOP(socketError.ErrorCode);

            DisposeConnection(true);
        }

        private void DisposeConnection(bool raiseDisconnectEvent)
        {
            SimpleGeneralTcpConnection connectionSnapshot = null;

            if (null != _mainSocket)
            {
                lock (_msSync)
                    if (null != _mainSocket)
                    {
                        try
                        {
                            _mainSocket.Close();
                            _mainSocket = null;

                            connectionSnapshot = _connection;
                            _connection = null;
                        }
                        catch
                        {

                        }

                        if (raiseDisconnectEvent)
                            InvokeClientDisconnected(connectionSnapshot);
                    }
            }
            
            

        }

        /// <summary>
        /// event raised, if the TCP connection has been closed
        /// </summary>
        public event DSimpleTcpConnectionEvent Disconnected = null;

        private void InvokeClientDisconnected(SimpleGeneralTcpConnection connection)
        {
            if (null == connection)
                return;

            if (null != Disconnected)
                try
                {
                    Disconnected(connection);
                }
                catch
                {
                }
        }

        protected internal override void InvokeDisconnected(SimpleGeneralTcpConnection connection)
        {
            DisposeConnection(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public delegate void DSimpleTcpConnectionEvent(ISimpleTcpConnection connection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="error"></param>
        public delegate void DSimpleTcpConnectionFailed(IPEndPoint ipEndpoint, Exception error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        public delegate void DSimpleTcpDataEvent(ISimpleTcpConnection connection, ByteDataCarrier data);

        /// <summary>
        /// event raised, when remote peer data has been received
        /// </summary>
        public event DSimpleTcpDataEvent DataReceived = null;

        protected internal override void InvokeDataReceived(SimpleGeneralTcpConnection connection, ByteDataCarrier data)
        {
            if (null == connection ||
                null == data)
                return;

            if (null != DataReceived)
                try
                {
                    DataReceived(connection, data);
                }
                catch
                {
                }
        }

        private bool _useSendingProcessingQueue = false;
        /// <summary>
        /// 
        /// </summary>
// ReSharper disable once ConvertToAutoProperty
        public override bool UseSendingProcessingQueue { get { return _useSendingProcessingQueue; } set { _useSendingProcessingQueue = value; } }

        /// <summary>
        /// sends data asynchronously
        /// </summary>
        /// <param name="data">encapsulated data to be sent</param>
        /// <exception cref="SocketException">if the client isn't connected</exception>
        /// <exception cref="InvalidOperationException">if specified data are empty</exception>
        public void Send(ByteDataCarrier data)
        {
            Send(_connection, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <exception cref="SocketException">if the client isn't connected</exception>
        /// <param name="data"></param>
        protected internal override void Send(SimpleGeneralTcpConnection connection, ByteDataCarrier data)
        {
            if (_connection == null)
                throw new SocketException((int)SocketError.NotConnected);

            if (null == data ||
                data.ActualSize == 0)
                throw new InvalidOperationException("Data must not be empty");

            try
            {
                connection.Send(data);
            }
            catch { }
        }

        /// <summary>
        /// event raised, when the data populated to the system's socket buffers
        /// </summary>
        public event DSimpleTcpDataEvent DataSent = null;

        protected internal override void InvokeDataSent(SimpleGeneralTcpConnection connection, ByteDataCarrier data)
        {
            if (null == connection ||
                null == data)
                return;

            if (null != DataSent)
                try
                {
                    DataSent(connection, data);
                }
                catch
                {
                }
        }

        /// <summary>
        /// returns true, if the TCP connection is established
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return null != _connection;
            }
        }

        internal ISimpleTcpConnection SimpleTcpConnection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="keepaliveInterval"></param>
        public void EnableKeepalive(bool enable, int keepaliveInterval)
        {
        }
    }
}
