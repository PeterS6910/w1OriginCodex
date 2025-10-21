using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Contal.IwQuick.Threads;
using System.Threading;
using Contal.IwQuick.Data;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// simple single connection TCP client
    /// with SSL support
    /// </summary>
    public class SimpleTcpClient : ASimpleTcpPeer
    {
        private volatile Timer _connectionTimer = null;
        private readonly object _ctSync = new object();

        // covers system's usual retry at 0s, 3s, 9s
        private const int DEFAULT_CONNECT_TIMEOUT = 15000; 
		// using minimal timeouts is mostly for testing purpose
		// in reality it's advised to be used
        private const int MINIMAL_CONNECT_TIMEOUT = 
#if DEBUG
			1;
#else
			500;
#endif

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
                    if (value < MINIMAL_CONNECT_TIMEOUT)
                        _connectTimeout = MINIMAL_CONNECT_TIMEOUT;
                    else
                        _connectTimeout = value;
                }
                else
                    _connectTimeout = DEFAULT_CONNECT_TIMEOUT;
            }
        }

        /// <summary>
        /// for information purpose only ... passed to ConnectFailed event
        /// </summary>
        private volatile IPEndPoint _lastEndpointBeingConnected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="transportSettings"></param>
        public void Connect(IPAddress ip, int port, ITransportSettings transportSettings)
        {
            Connect(new IPEndPoint(ip, port), transportSettings);
        }


        /// <summary>
        /// connects to the specified remote side
        /// </summary>
        /// <param name="ipEndpoint">ip of the destination</param>
        /// <param name="transportSettings">instannce for the SSL settings or null, if SSL is not to be applied</param>
        /// <exception cref="ArgumentNullException">if ipEndpoint is null</exception>
        /// <exception cref="InvalidOperationException">if client is already connected</exception>
        /// <exception cref="ArgumentOutOfRangeException">if the port is out of range</exception>
        public void Connect(IPEndPoint ipEndpoint, ITransportSettings transportSettings)
        {
            if (null == ipEndpoint)
                throw new ArgumentNullException("ipEndpoint");

            lock (_syncRoot)
            {
                if (_connectState == ConnectState.Connected || _connectState == ConnectState.Connecting ||
                    _mainSocket != null)
                {
                    throw new InvalidOperationException(
                           "Tcp client is already in connecting process or is connected");
                }

                try
                {
                    _transportSettings = transportSettings;

                    if (null == _mainSocket)
                    {
                        _mainSocket = new Socket(ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        if (_enableKeepalive)
                            SetKeepaliveValues(_mainSocket);

                        if (null != _transportSettings && null != _transportSettings.GetLayer())
                            try
                            {
                                _transportSettings.GetLayer().PreConnect(_mainSocket);
                            }
                            catch
                            {
                            }
                    }
                }
                catch(Exception preConnectError)
                {
                    // also reverts the state to Disconnected
                    if (!ConnectFailed(ipEndpoint, preConnectError,false))
                        throw;// preConnectError;
                    
                    return;
                }
            }

            try
            {
                StartStopConnectTimeout(ipEndpoint,false); // in case there is any timeout running

                _waitForConnectLock.Reset();
                _lastEndpointBeingConnected = ipEndpoint;

                lock (_syncRoot)
                {
                    _connectState = ConnectState.Connecting;
                }

                // make the priority of the connecting thread highest, as the connect blocking call would NOT overhaul the CPU
                // TODO : priority inversion checking
                SafeThread<IPEndPoint>.StartThread(ConnectAsyncThread, ipEndpoint, ThreadPriority.Highest, false);

                // planning the timeout
                StartStopConnectTimeout(ipEndpoint, true);
            }
            catch (Exception aError)
            {
                bool acquiredFailedBeforeConnectCall = false;

                if (_connectState == ConnectState.Connecting)
                    lock (_syncRoot)
                    {
                        if (_connectState == ConnectState.Connecting)
                        {
                            _connectState = ConnectState.FailedBeforeConnectCall;
                            acquiredFailedBeforeConnectCall = true;
                        }
                    }

                if (acquiredFailedBeforeConnectCall)
                {
                    StartStopConnectTimeout(ipEndpoint,false);
                    ConnectFailed(ipEndpoint, aError,true);
                }
            }

        }

        private bool ConnectFailed(IPEndPoint ipEndpoint, Exception aError, bool signalWaitForConnectLock)
        {
            DisposeConnection(false,false);

            bool invoked = InvokeClientConnectionFailed(ipEndpoint, aError);

            if (signalWaitForConnectLock)
                // release the wait after the ConnectionFailed events had been raised
                _waitForConnectLock.Set();

            return invoked;
        }

        private volatile object _syncRoot = new object();

        private enum ConnectState
        {
            Disconnected = -1,
            Connecting = 0,
            Connected = 1,
            ConnectionTimedOut = 2,
            FailedWhileConnectCall = 3,
            FailedBeforeConnectCall = 4
        }

        private volatile ConnectState _connectState = ConnectState.Disconnected; // volatile ensures atomicity during simple instructions in paralel contexts

        /// <summary>
        /// separation of the connect process into the thread
        /// </summary>
        /// <param name="ipEndPoint"></param>
        private void ConnectAsyncThread(IPEndPoint ipEndPoint)
        {
            try
            {
                if (_connectState != ConnectState.Connecting) 
                    lock(_syncRoot)
                        if (_connectState != ConnectState.Connecting)
                        {
                            return;
                                // for some reason OnConnectTimeout or Disconnect has been called first or the result is already known
                            // therefore proceeding with this thread has no meaning
                        }


                //Thread.CurrentThread.Priority = ThreadPriority.Highest; // set during thread start
                _mainSocket.Connect(ipEndPoint); // this could be a long-lasting operation

                #region Only for synchronisation with connection timeout
                bool acquiredConnected = false;

                if (_connectState == ConnectState.Connecting)
                    lock (_syncRoot)
                    {
                        if (_connectState == ConnectState.Connecting)
                        {
                            _connectState = ConnectState.Connected;
                            acquiredConnected = true;
                        }
                        //else
                        // other things will be handled by the OnConnectTimeout mechanism
                    }

                if (!acquiredConnected)
                    // most possibly due running OnConnectTimeout or Disconnect
                    return;

                StartStopConnectTimeout(ipEndPoint,false);

                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                #endregion

                try
                {
                    _connection = SimpleGeneralTcpConnection.Create(this, _mainSocket);

                    // signal the connect after the connection has been instantiated
                    // indicate connection connected by lock
                    _waitForConnectLock.Set();

                    InvokeClientConnected();

                    PrepareReceive(_connection);

                }
                catch (Exception aError)
                {
                    ConnectFailed(ipEndPoint, aError, false);
                }
                
            }
            catch(Exception error)
            {
                bool acquiredFailedWhileConnectCall = false;

                if (_connectState == ConnectState.Connecting)
                    lock (_syncRoot)
                    {
                        if (_connectState == ConnectState.Connecting)
                        {
                            _connectState = ConnectState.FailedWhileConnectCall;
                            acquiredFailedWhileConnectCall = true;
                        }
                        //else
                        // other things will be handled by the OnConnectTimeout or Disconnect mechanism
                    }

                if (acquiredFailedWhileConnectCall)
                {
                    StartStopConnectTimeout(ipEndPoint,false);
                    ConnectFailed(ipEndPoint, error,true);
                }
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
            if (!retVal)
            {
                // this has to be done for situations where custom/application timeout finished with false result
                // but the internal connect timeout did not 
                //
                // thus if next Connect would follow this situation , the _mainSocket would certainly NOT be nulled yet by DisposeConnection 
                // by internal OnConnectTimeout
                OnConnectTimeout(_lastEndpointBeingConnected);

                // during the process of forcing the timeout, the 
                // Connect could have ended successfuly 
                // 
                // in such case OnConnectTimeout wouldn't do anything, 
                // thus the "false" result wouldn't be relevant
                return IsConnected;
            }
            
            return true;
        }

        /// <summary>
        /// BE AWARE OF FACT THAT EVEN IF THE THE WAITFORCONNECT ENDS WITH TRUE RESULT, IT CAN BE DUE CONNECT HAD FAILED
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
        /// <param name="transportSettings">encryption settings instance or null, if socket would be plain</param>
        /// <exception cref="ArgumentNullException">if ip is null or empty</exception>
        /// <exception cref="InvalidIPException">if ip is not valid IP</exception>
        /// <exception cref="ArgumentOutOfRangeException">if the port is out of range</exception>
        public void Connect(string ip, int port, ITransportSettings transportSettings)
        {
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentNullException("ip");

            IPAddress aIP = IPHelper.Parse(ip);

            IPEndPoint ipE = new IPEndPoint(aIP, port);

            Connect(ipE, transportSettings);
        }

        private void OnConnectTimeout(object state)
        {
            bool acquiredConnectionTimedOut = false;

            if (_connectState == ConnectState.Connecting)
                lock (_syncRoot)
                {
                    if (_connectState == ConnectState.Connecting)
                    {
                        _connectState = ConnectState.ConnectionTimedOut;
                        acquiredConnectionTimedOut = true;
                    }
                }

            if (acquiredConnectionTimedOut)
            {
                IPEndPoint ipEndpoint = null;
                if (state is IPEndPoint)
                    ipEndpoint = state as IPEndPoint;

                DisposeConnection(false,false);
                InvokeClientConnectionFailed(ipEndpoint, new SocketException((int)SocketError.TimedOut));
            }
            
            //}
        }

        private volatile ISimpleTcpConnection _connection = null;

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (null == _connection)
                    return null;
                
                return _connection.RemoteEndPoint;
            }
        }

        private bool _enableKeepalive = false;
        private uint _keepaliveInterval = 0;

        private void SetKeepaliveValues(Socket s)
        {
            if (s == null)
                return;

            uint onoff = 0;
           
            if (_enableKeepalive)
                onoff = 1;

            byte[] result = new byte[sizeof(int)];

            byte[] keepAliveBytes = new byte[12];

            ByteConverter.ToBytes(keepAliveBytes, 0, onoff);
            ByteConverter.ToBytes(keepAliveBytes, 4, _keepaliveInterval);
            ByteConverter.ToBytes(keepAliveBytes, 8, _keepaliveInterval / 2);

            int resultSize = s.IOControl(IOControlCode.KeepAliveValues, keepAliveBytes, result);
            DebugHelper.Keep(resultSize);

            uint resultInt = BitConverter.ToUInt32(result,0);
            if (resultInt != 0)
                throw new SystemException("Unable to setup keepalive with system error " + resultInt);
        }

        private const uint LOWEST_KEEPALIVE_INTERVAL = 200;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="keepaliveInterval">interval between keepalive TCP packets in miliseconds</param>
        public void EnableKeepalive(bool enable, uint keepaliveInterval)
        {
            if (!enable)
            {
                _keepaliveInterval = 0;
            }
            else
            {
                if (keepaliveInterval < LOWEST_KEEPALIVE_INTERVAL)
                    throw new ArgumentException("Having keepalive interval less than " + LOWEST_KEEPALIVE_INTERVAL + "ms is inadvisable");
                
                _keepaliveInterval = keepaliveInterval;
                
            }

            _enableKeepalive = enable;
        }

        private void StartStopConnectTimeout(IPEndPoint ipEndpoint, bool toStart)
        {
            lock (_ctSync)
            {
                if (toStart)
                {
                    if (_connectionTimer == null)
                        _connectionTimer = new Timer(OnConnectTimeout, ipEndpoint, _connectTimeout, Timeout.Infinite);
                    else
                        _connectionTimer.Change(_connectTimeout, Timeout.Infinite);
                }
                else
                {
                    if (_connectionTimer != null)
                    {
                        try
                        {
                            _connectionTimer.Change(Timeout.Infinite, Timeout.Infinite);

                        }
                        catch
                        {

                        }

                    }
                }
            }
        }

        /// <summary>
        /// disconnects the TCP connection
        /// </summary>
        public void Disconnect()
        {
            if (_connectState != ConnectState.Connected)
                lock (_syncRoot)
                {
                    if (_connectState != ConnectState.Connected)
                        throw new InvalidOperationException("Connection in state " + _connectState +
                                                            " cannot be disconnected");
                }

            DisposeConnection(true,true);
        }

        /// <summary>
        /// event raised, when the TCP client become connected
        /// </summary>
        public event DTcpConnectEvent Connected = null;

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
        public event DTcpConnectFailureEvent ConnectionFailed = null;

        private bool InvokeClientConnectionFailed(IPEndPoint ipEndpoint, Exception exception)
        {
            if (null != ConnectionFailed)
                try
                {
                    ConnectionFailed(ipEndpoint, exception);
                    return true;
                }
                catch
                {
                    return false;
                }
            
            return false;
        }

        protected internal override void FireSocketException(ISimpleTcpConnection connection, SocketException socketException)
        {
            ExamineSocketError(socketException);
        }

        private void ExamineSocketError(SocketException socketError)
        {
            switch (socketError.SocketErrorCode)
            {
                case SocketError.ConnectionReset:
                case SocketError.ConnectionAborted:
                case SocketError.ConnectionRefused:
                case SocketError.HostDown:
                case SocketError.HostUnreachable:
                case SocketError.NetworkDown:
                case SocketError.NetworkReset:
                case SocketError.NetworkUnreachable:
                case SocketError.NotConnected:
                case SocketError.TimedOut:
                case SocketError.Interrupted:
                case SocketError.Fault:
                case SocketError.OperationAborted:
                    DisposeConnection(false,true);
                    break;
                default:
                    DisposeConnection(false, true);
#if DEBUG && !COMPACT_FRAMEWORK
                    Debug.Assert(false);
#endif
                    break;
            }
        }

        private void PrepareReceive(ISimpleTcpConnection connection)
        {
            if (null == connection)
                return;

            try
            {
                //ByteDataCarrier aReceiveData = new ByteDataCarrier(_bufferSize);
                connection.StartReceive();
                //SimpleTcpDataCarrier aAsyncDataCarrier =
                //    new SimpleTcpDataCarrier(connection, aReceiveData);

                //connection.ChannelStream.BeginRead(aReceiveData.Buffer, 0, aReceiveData.Size,
                //    new AsyncCallback(OnReceive), aAsyncDataCarrier);
                /*
                connection.ClientSocket.BeginReceive(aReceiveData.Buffer, 0, aReceiveData.Size,
                    SocketFlags.None, new AsyncCallback(OnReceive), aAsyncDataCarrier);*/
            }
            //catch (SocketException aSocketError)
            //{
            //    ExamineSocketError(connection, aSocketError);
            //}
            //catch (Exception aError)
            //{
            //    if (null != aError.InnerException &&
            //        aError.InnerException is SocketException)
            //        ExamineSocketError(connection, (SocketException)aError.InnerException);
            //}
            catch { }
        }

        //private void OnReceive(IAsyncResult result)
        //{
        //    if (null == result)
        //        return;

        //    SimpleTcpDataCarrier aAsyncDataCarrier = (SimpleTcpDataCarrier)result.AsyncState;

        //    try
        //    {
        //        int iActualSize = aAsyncDataCarrier._connection.ChannelStream.EndRead(result);
        //        //int iActualSize = aAsyncDataCarrier._connection.ClientSocket.EndReceive(result);

        //        if (iActualSize > 0)
        //        {
        //            aAsyncDataCarrier._data.ActualSize = iActualSize;
        //            // to avoid receive buffer overwriting

        //            PrepareReceive(aAsyncDataCarrier._connection);

        //            // process the data after planning
        //            FireDataReceived(aAsyncDataCarrier._connection, aAsyncDataCarrier._data);

        //            // test for optimization
        //            // test for optimization
        //            //aAsyncDataCarrier._connection = null;
        //            //aAsyncDataCarrier._data = null;
        //        }
        //        else
        //        {
        //            DisposeConnection(true);
        //        }
        //    }
        //    catch (SocketException aSocketError)
        //    {
        //        ExamineSocketError(aAsyncDataCarrier._connection, aSocketError);
        //    }
        //    catch (Exception aError)
        //    {
        //        if (null != aError.InnerException &&
        //            aError.InnerException is SocketException)
        //            ExamineSocketError(aAsyncDataCarrier._connection, (SocketException)aError.InnerException);

        //        // if ObjectDisposed over socket occured, it means, it was closed directly by disconnect or similar
        //        /*if (null != aError.InnerException &&
        //            aError.InnerException is ObjectDisposedException)
        //            if (UnregisterConnection(_connection))
        //                DisposeConnection();*/
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketShutdown"></param>
        /// <param name="raiseDisconnectEvent"></param>
        private void DisposeConnection(bool socketShutdown,bool raiseDisconnectEvent)
        {
#pragma warning disable 420
            Socket socketRemnant = Interlocked.Exchange(ref _mainSocket, null);
            ISimpleTcpConnection connectionRemnant = Interlocked.Exchange(ref _connection, null);
#pragma warning restore 420


            Debug.WriteLine("Nulling _mainSocket");

            bool alreadyDisconnected = false;
            lock (_syncRoot) // this lock is not just for assigning the state value, but also for locking concurrent DisposeConnection calls
            {
                if (_connectState != ConnectState.Disconnected)
                    _connectState = ConnectState.Disconnected;
                else
                    alreadyDisconnected = true;
            }

            if (null != socketRemnant)
            {
                try
                {
                    if (socketShutdown)
                        socketRemnant.Shutdown(SocketShutdown.Both);
                    socketRemnant.Close();
                }
                catch
                {

                }
                finally
                {
                    if (raiseDisconnectEvent && !alreadyDisconnected)
                        InvokeClientDisconnected(connectionRemnant);
                }
            }
        }

        /// <summary>
        /// event raised, if the TCP connection has been closed
        /// </summary>
        public event DTcpConnectEvent Disconnected = null;

        private void InvokeClientDisconnected(ISimpleTcpConnection connection)
        {
            if (ReferenceEquals(connection,null))
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

        protected internal override void FireDisconnected(ISimpleTcpConnection connection)
        {
            DisposeConnection(false,true);
        }

        /// <summary>
        /// event raised, when remote peer data has been received
        /// </summary>
        public event DTcpDataEvent DataReceived = null;

        protected internal override void FireDataReceived(ISimpleTcpConnection connection, ByteDataCarrier data)
        {
            if (ReferenceEquals(connection,null) ||
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
// ReSharper disable once ConvertToAutoProperty
        public override bool UseSendingProcessingQueue { get { return _useSendingProcessingQueue; } set { _useSendingProcessingQueue = value; } }

        /// <summary>
        /// sends data asynchronously
        /// </summary>
        /// <param name="data">encapsulated data to be sent</param>
        /// <exception cref="ArgumentNullException">if the connection is invalid</exception>
        /// <exception cref="InvalidOperationException">if specified data are empty</exception>
        public void Send(ByteDataCarrier data)
        {
            Send(_connection, data);
        }

        protected internal override void Send(ISimpleTcpConnection connection, ByteDataCarrier data)
        {
            if (ReferenceEquals(connection, null))
                throw new ArgumentNullException("connection");

            if (null == data ||
                data.ActualSize == 0)
                throw new InvalidOperationException("Data must not be empty");

            try
            {
                connection.Send(data);
                //SimpleTcpDataCarrier aAsyncDataCarrier = new SimpleTcpDataCarrier(
                //    connection,
                //    data);

                //connection.ChannelStream.BeginWrite(aAsyncDataCarrier._data.Buffer, 0, aAsyncDataCarrier._data.ActualSize,
                //    new AsyncCallback(OnSend), aAsyncDataCarrier);
                /*
                connection.ClientSocket.BeginSend(aAsyncDataCarrier._data.Buffer, 0, aAsyncDataCarrier._data.ActualSize,
                    SocketFlags.None, new AsyncCallback(OnSend), aAsyncDataCarrier);*/
            }
            //catch (SocketException aSocketError)
            //{
            //    ExamineSocketError(connection, aSocketError);
            //}
            //catch (Exception aError)
            //{
            //    if (null != aError.InnerException &&
            //        aError.InnerException is SocketException)
            //        ExamineSocketError(connection, (SocketException)aError.InnerException);
            //}
            catch { }
        }

        //private void OnSend(IAsyncResult result)
        //{
        //    if (null == result)
        //        return;

        //    SimpleTcpDataCarrier aAsyncDataCarrier = (SimpleTcpDataCarrier)result.AsyncState;


        //    try
        //    {
        //        aAsyncDataCarrier._connection.ChannelStream.EndWrite(result);
        //        /*#if DEBUG
        //                        if (iSend != aAsyncDataCarrier._data.ActualSize)
        //                            Contal.IwQuick.UI.CConsole.Warning("Different sizes " + iSend + " " + aAsyncDataCarrier._data.ActualSize);
        //        #endif*/

        //        //aAsyncDataCarrier._data.ActualSize = iSend;

        //        FireDataSent(aAsyncDataCarrier._connection, aAsyncDataCarrier._data);

        //        // test for optimization
        //        aAsyncDataCarrier._connection = null;
        //        aAsyncDataCarrier._data = null;

        //    }
        //    catch
        //    {
        //        return;
        //    }

        //    // variant without stream operations
        //    /*int iSend =  aAsyncDataCarrier._connection.ClientSocket.EndSend(result);
        //    if (iSend > 0)
        //    {

        //        aAsyncDataCarrier._data.ActualSize = iSend;

        //        FireDataSent(aAsyncDataCarrier._connection, aAsyncDataCarrier._data);

        //        // test for optimization
        //        aAsyncDataCarrier._connection = null;
        //        aAsyncDataCarrier._data = null;
        //    }
        //    */

        //}

        /// <summary>
        /// event raised, when the data populated to the system's socket buffers
        /// </summary>
        public event DTcpDataEvent DataSent = null;

        protected internal override void FireDataSent(ISimpleTcpConnection connection, ByteDataCarrier data)
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

        /*
        public event DTcpDataEvent DataSendTimedOut = null;

        private void FireDataSentTimedOut(ISimpleTcpConnection connection, ByteDataCarrier data)
        {
            if (null == connection ||
                null == data)
                return;

            if (null != DataSendTimedOut)
                try
                {
                    DataSendTimedOut(connection, data);
                }
                catch
                {
                }
        }*/

        /// <summary>
        /// returns true, if the TCP connection is established
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _connectState == ConnectState.Connected;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal ISimpleTcpConnection SimpleTcpConnection
        {
            get
            {
                return _connection;
            }
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            base.InternalDispose(isExplicitDispose);

            if (null != _connectionTimer)
            {
                // defensive approach, every substep can fail separately
                // but not preventing the rest from execution
                try
                {
                    if (isExplicitDispose) Monitor.Enter(_ctSync);
                }
                catch
                {
                }

                try { _connectionTimer.Change(Timeout.Infinite, Timeout.Infinite); }catch{}

                try
                {
                    _connectionTimer.Dispose();
                    _connectionTimer = null;
                } catch { }

                try
                {
                    if (isExplicitDispose) try { Monitor.Exit(_ctSync); } catch {  }
                } 
                catch
                {

                }
            }

            DisposeConnection(false, false);
        }


    }
}
