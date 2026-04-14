using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
   
    /// <summary>
    /// basic implementation of the TCP server
    /// with SSL support
    /// </summary>
    public class SimpleTcpServer : ASimpleTcpPeer, IEnumerable<ISimpleTcpConnection>
    {
        private Dictionary<IPEndPoint, ISimpleTcpConnection> _connections = null;

        public ISimpleTcpConnection GetConnection([NotNull] IPEndPoint ipEndpoint)
        {
            Validator.CheckForNull(ipEndpoint,"ipEndpoint");

            if (_connections == null)
                return null;
            
            lock (_connections)
            {
                ISimpleTcpConnection connection;
                if (_connections.TryGetValue(ipEndpoint, out connection))
                    return connection;
                
                return null;
            }
        }

        public ISimpleTcpConnection GetFirstConnection()
        {
            if (_connections == null)
                return null;
            
            lock (_connections)
            {
                if (_connections.Count <= 0)
                    return null;
                
                foreach (ISimpleTcpConnection connection in _connections.Values)
                {
                    if (connection != null)
                        return connection;
                }
                return null;
            }
        }

        private bool _listening = false;

        /// <summary>
        /// starts listening of the server
        /// </summary>
        /// <param name="ip">ip for bounding the server, can be IPAddress.Any</param>
        /// <param name="port">TCP port to listen on</param>
        /// <param name="transportSettings">SSL settings instance or null, if SSL not applied</param>
        /// <param name="backLog">number of pending connections</param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start(
            [NotNull] IPAddress ip, 
            int port, 
            ITransportSettings transportSettings, 
            int backLog)
        {
            if (_listening)
                throw new InvalidOperationException("Tcp server already started");

            Validator.CheckForNull(ip,"ip");
            TcpUdpPort.CheckValidity(port);

            _transportSettings = transportSettings;
// ReSharper disable once RedundantComparisonWithNull
            if (_transportSettings != null && _transportSettings is SslSettings)
            {
                SslSettings sslSettings = (SslSettings)_transportSettings;
                if (string.IsNullOrEmpty(sslSettings.CertificatePath))
                    throw new ArgumentNullException("Tcp server started with SSL enabled and no certificate configured.");
            }

            if (null == _mainSocket)
            {
                _mainSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                if (_useKeepalives)
                    _mainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                //_mainSocket.SendTimeout = _sendTimeout;
            }

            if (null == _connections)
                _connections = new Dictionary<IPEndPoint, ISimpleTcpConnection>();

            _mainSocket.Bind(new IPEndPoint(ip, port));
            _mainSocket.Listen(backLog <= 0 ? 32 : backLog);

            PrepareAccept();

            _listening = true;
        }

        /// <summary>
        /// starts listening of the server
        /// </summary>
        /// <param name="useIPv6">if true, IPv6/TCP listening socket will be used</param>
        /// <param name="isLocal">if true, the socket will be bound only to loopback interface</param>
        /// <param name="port">TCP port to listen on</param>
        /// <param name="transportSettings"></param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start(bool useIPv6, bool isLocal, int port, ITransportSettings transportSettings)
        {
            IPAddress ip;
            if (useIPv6)
            {
                ip = isLocal ? IPAddress.IPv6Loopback : IPAddress.IPv6Any;
            }
            else
            {
                ip = isLocal ? IPAddress.Loopback : IPAddress.Any;
            }

            Start(ip, port, transportSettings, 0);
        }

        /// <summary>
        /// starts listening of the server (IPv4 only)
        /// </summary>
        /// <param name="isLocal">if true, the socket will be bound only to loopback interface</param>
        /// <param name="port">TCP port to listen on</param>
        /// <param name="transportSettings"></param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start(bool isLocal, int port, ITransportSettings transportSettings)
        {
            Start(false, isLocal, port, transportSettings);
        }

        /// <summary>
        /// starts listening of the server (IPv4 only)
        /// </summary>
        /// <param name="port">TCP port to listen on</param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start(int port)
        {
            Start(false, false, port,null);
        }

        /// <summary>
        /// starts listening of the server (IPv6 only)
        /// </summary>
        /// <param name="port">TCP port to listen on</param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start6(int port)
        {
            Start(true, false, port,null);
        }

        /// <summary>
        /// starts listening of the server (IPv4 only)
        /// </summary>
        /// <param name="port">TCP port to listen on</param>
        /// <param name="transportSettings">SSL settings instance or null, if SSL not applied</param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start(int port, ITransportSettings transportSettings)
        {
            Start(IPAddress.Any, port, transportSettings, 0);
        }

        /// <summary>
        /// starts listening of the server (IPv6 only)
        /// </summary>
        /// <param name="port">TCP port to listen on</param>
        /// <param name="transportSettings">SSL settings instance or null, if SSL not applied</param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start6(int port, ITransportSettings transportSettings)
        {
            Start(IPAddress.IPv6Any, port, transportSettings, 0);
        }

        private bool _useKeepalives = false;
        public bool UseKeepalives
        {
            get { return _useKeepalives; }
            set
            {
                _useKeepalives = value;
            }
        }

        /// <summary>
        /// stops listening on the server
        /// </summary>
        /// <exception cref="InvalidOperationException">if the server has been already stopped</exception>
        public void Stop()
        {
            if (!_listening)
                throw new InvalidOperationException("Tcp server is not listening");

            try
            {
                List<ISimpleTcpConnection> aConnectionsToClose = new List<ISimpleTcpConnection>(_connections.Values);

                // to avoid back closing from receive/send/accept methods
                _connections.Clear();

                foreach (ISimpleTcpConnection aConnection in aConnectionsToClose)
                    aConnection.Disconnect();

                _mainSocket.Close();

                _mainSocket = null;

            }
            catch
            {
            }
            finally
            {
                _listening = false;
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

            s.IOControl(IOControlCode.KeepAliveValues, keepAliveBytes, result);

            uint resultInt = BitConverter.ToUInt32(result, 0);
            if (resultInt != 0)
                throw new SystemException("Unable to setup keepalive with system error " + resultInt);
        }

        private const uint LOWEST_KEEPALIVE_INTERVAL = 200;

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

        private SafeThread _acceptThread = null;
        private void PrepareAccept()
        {
            if (null == _mainSocket)
                return;

            if (null == _acceptThread ||
                !_acceptThread.IsStarted || _acceptThread.IsAborted)
            {
                _acceptThread = new SafeThread(AcceptThread);

                _acceptThread.Start();
            }

            //_mainSocket.BeginAccept(new AsyncCallback(OnAccept), null);
        }

        private void AcceptThread()
        {
            try
            {
                while (true)
                {
                    Socket s = _mainSocket.Accept();

                    OnAccept(s);
                }
            }
            catch
            {
            }
        }

        private void OnAccept(Socket clientSocket)
        {
            /*if (null == result)
                return;

            Socket aClientSocket = null;

            try
            {
                if (null == _mainSocket)
                    // this means, that server socket was forcibly stopped
                    // don't call PrepareAccept any more
                    return;

                aClientSocket = _mainSocket.EndAccept(result);
            }
            catch (ObjectDisposedException)
            {
                // this means, that server socket was forcibly stopped
                // don't call PrepareAccept any more
                return;
            }
            catch //(Exception aError)
            {
                aClientSocket = null;
                // at the end, the PrepareAccept should be called 
            }*/

            try
            {
                if (null != clientSocket)
                {
                    if (_enableKeepalive)
                        SetKeepaliveValues(clientSocket);

                    //aClientSocket.SendTimeout = _sendTimeout;

                    // if certificate is specified, the connection is created as SSL
                    ISimpleTcpConnection aConnection = SimpleGeneralTcpConnection.Create(this, clientSocket);

                    FireClientConnected(aConnection);

                    PrepareReceive(aConnection);
                }
            }
            catch
            {
                try
                {
                    if (null != clientSocket)
                        clientSocket.Close();
                }
                catch { }
            }
            //finally
            //{
            //    // prepare accept after to ensure this thread won't be terminated by next accept
            //    PrepareAccept();
            //}
        }

        /// <summary>
        /// event raised, when the client is connected to this listening server
        /// </summary>
        public event DTcpConnectEvent Connected = null;

        private void FireClientConnected(ISimpleTcpConnection connection)
        {
            if (null == connection)
                return;

            // register connection after the connected event was raised
            // especially necessary for delayed SSL connection event hadnling
            IPEndPoint aIPe = connection.RemoteEndPoint;

            if (!_connections.ContainsKey(aIPe))
            {

                _connections[aIPe] = connection;

                if (null != Connected)
                    try
                    {
                        Connected(connection);
                    }
                    catch
                    {
                    }
            }
        }

        private void ExamineSocketError(ISimpleTcpConnection connection,SocketException socketError)
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
                        if (UnregisterConnection(connection))
                            DisposeConnection(connection);
                        break;
                    default:
                        Debug.Assert(false);
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
            //catch(Exception aError)
            //{
            //    if (null != aError.InnerException &&
            //        aError.InnerException is SocketException)
            //        ExamineSocketError(connection, (SocketException)aError.InnerException);
            //}
            catch { }
        }

        protected internal override void FireSocketException(ISimpleTcpConnection connection, SocketException socketException)
        {
            ExamineSocketError(connection, socketException);
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
        //            FireClientDataReceived(aAsyncDataCarrier._connection, aAsyncDataCarrier._data);

        //            // test for optimization
        //            aAsyncDataCarrier._connection = null;
        //            aAsyncDataCarrier._data = null;
        //        }
        //        else
        //        {
        //            if (UnregisterConnection(aAsyncDataCarrier._connection))
        //                DisposeConnection(aAsyncDataCarrier._connection);
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
        //    }
        //}

        /// <summary>
        /// event raised, when TCP client disconnects from this listening server
        /// </summary>
        public event DTcpConnectEvent Disconnected = null;

        private void FireClientDisconnected(ISimpleTcpConnection connection)
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

        protected internal override void FireDisconnected(ISimpleTcpConnection connection)
        {
            if (UnregisterConnection(connection))
                DisposeConnection(connection);
        }

        /// <summary>
        /// MUST BE CALLED BEFORE THE SOCKET IS DISPOSED BY CLOSE METHOD !!!
        /// </summary>
        /// <param name="connection"></param>
        private bool UnregisterConnection(ISimpleTcpConnection connection)
        {
            if (null == connection)
                return false;

            if (_connections.ContainsKey(connection.RemoteEndPoint))
            {
                _connections.Remove(connection.RemoteEndPoint);

                FireClientDisconnected(connection);

                return true;
            }
            return false;
        }

        private void DisposeConnection(ISimpleTcpConnection connection)
        {
            if (connection == null)
                return;

            connection.Disconnect();
        }

        /// <summary>
        /// event raised, when data has been received from any connected client
        /// </summary>
        public event DTcpDataEvent DataReceived = null;

        protected internal override void FireDataReceived(ISimpleTcpConnection connection, ByteDataCarrier data)
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
// ReSharper disable once ConvertToAutoProperty
        public override bool UseSendingProcessingQueue { get { return _useSendingProcessingQueue; } set { _useSendingProcessingQueue = value; } }

        /// <summary>
        /// sends data to a specified connection descriptor
        /// </summary>
        /// <param name="connection">connection descriptor</param>
        /// <param name="data">data to be sent</param>
        /// <exception cref="ArgumentNullException">if the connection or the data are null referrence</exception>
        /// <exception cref="ByteDataCarrier">if the connection or the data are null referrence</exception>
        protected internal override void Send(
            [NotNull] ISimpleTcpConnection connection, 
            [NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(connection, "connection");
            Validator.CheckForNull(data, "data");

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

//        private void OnSend(IAsyncResult result)
//        {
//            if (null == result)
//                return;

//            SimpleTcpDataCarrier aAsyncDataCarrier = (SimpleTcpDataCarrier)result.AsyncState;

            
//            try
//            {
//                aAsyncDataCarrier._connection.ChannelStream.EndWrite(result);
///*#if DEBUG
//                if (iSend != aAsyncDataCarrier._data.ActualSize)
//                    Contal.IwQuick.UI.CConsole.Warning("Different sizes " + iSend + " " + aAsyncDataCarrier._data.ActualSize);
//#endif*/

//                //aAsyncDataCarrier._data.ActualSize = iSend;

//                FireDataSent(aAsyncDataCarrier._connection, aAsyncDataCarrier._data);

//                // test for optimization
//                aAsyncDataCarrier._connection = null;
//                aAsyncDataCarrier._data = null;

//            }
//            catch
//            {
//                return;
//            }
//            /*int iSend =  aAsyncDataCarrier._connection.ClientSocket.EndSend(result);
//            if (iSend > 0)
//            {
//#if DEBUG
//                if (iSend != aAsyncDataCarrier._data.ActualSize)
//                    Contal.IwQuick.UI.CConsole.Warning("Different sizes " + iSend + " " + aAsyncDataCarrier._data.ActualSize);
//#endif

//                aAsyncDataCarrier._data.ActualSize = iSend;

//                FireDataSent(aAsyncDataCarrier._connection, aAsyncDataCarrier._data);

//                // test for optimization
//                aAsyncDataCarrier._connection = null;
//                aAsyncDataCarrier._data = null;
//            }
//            else
//            {
//#if DEBUG
//                Contal.IwQuick.UI.CConsole.Error("No data sent " + iSend + " " + aAsyncDataCarrier._data.ActualSize);
//#endif
//            }*/

//        }

        /// <summary>
        /// raised, when data populated to the client
        /// </summary>
        public event DTcpDataEvent DataSent;

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


        #region IEnumerable<CSimpleTcpConnection> Members

        public IEnumerator<ISimpleTcpConnection> GetEnumerator()
        {
            return _connections.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _connections.Values.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// count of the clients connected
        /// </summary>
        public int Count
        {
            get
            {
                if (null == _connections)
                    return 0;
                return _connections.Count;
            }
        }

        /// <summary>
        /// returns true, if the server is listening for the TCP connection
        /// </summary>
        public bool IsListening
        {
            get
            {
                return _listening;
            }
        }

        /*
        private int _sendTimeout = 1000;
        public int SendTimeout
        {
            get
            {
                return _sendTimeout;
            }
            set
            {
                if (value > 0)
                    _sendTimeout = value;
            }
        }*/
    }
}
