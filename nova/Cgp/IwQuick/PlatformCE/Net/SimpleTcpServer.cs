using System;
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
    public class SimpleTcpServer : ASimpleTcpPeer, IEnumerable<SimpleGeneralTcpConnection>
    {
        private Dictionary<IPEndPoint, SimpleGeneralTcpConnection> _connections = null;

        private bool _listening = false;

        private const int DefaultListenBacklog = 16;
        
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

            if (null == _mainSocket)
                _mainSocket = new SuperSocket();

            if (null == _connections)
                _connections = new Dictionary<IPEndPoint, SimpleGeneralTcpConnection>();

            _mainSocket.Bind(new IPEndPoint(ip, port));
            _mainSocket.Listen(backLog <= 0 ? DefaultListenBacklog : backLog);

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
        /// 
        /// </summary>
        /// <param name="local"></param>
        /// <param name="port"></param>
        public void Start(bool local, int port)
        {
            Start(false, local, port, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="local"></param>
        /// <param name="port"></param>
        public void Start6(bool local, int port)
        {
            Start(true, true, port, null);
        }

        /// <summary>
        /// starts listening of the server (IPv4 only)
        /// </summary>
        /// <param name="port">TCP port to listen on</param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start(int port)
        {
            Start(false, false,port, null);
        }

        /// <summary>
        /// starts listening of the server (IPv6 only)
        /// </summary>
        /// <param name="port">TCP port to listen on</param>
        /// <exception cref="InvalidOperationException">if the server has been already started</exception>
        public void Start6(int port)
        {
            Start(true, false, port, null);
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

        private const int AcceptThreadStopTimeout = 3000;

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
                var aConnectionsToClose = new List<SimpleGeneralTcpConnection>(_connections.Values);

                // to avoid back closing from receive/send/accept methods
                _connections.Clear();

                foreach (ISimpleTcpConnection aConnection in aConnectionsToClose)
                    aConnection.Disconnect();

                _mainSocket.Close();

                _mainSocket = null;

                // stopping of the thread is counting on the fact,
                // that closing the _mainSocket will produce exception in the thread
                // thus causing its graceful/controlled stopping
                var stsr = _acceptThread.Stop(AcceptThreadStopTimeout);
                if (stsr != SafeThreadStopResolution.StoppedGracefuly)
                    DebugHelper.Keep(stsr);

            }
            catch
            {
            }
            finally
            {
                _listening = false;
            }
        }

        private SafeThread _acceptThread = null;
        private void PrepareAccept()
        {
            if (null == _mainSocket)
                return;

            if (null == _acceptThread ||
                !_acceptThread.IsStarted || 
                _acceptThread.IsAborted)
            {
                _acceptThread = new SafeThread(AcceptThread);

                _acceptThread.Start();
            }
        }

        private void AcceptThread()
        {
            try
            {
                while (true)
                {
                    var s = _mainSocket.Accept();                    

                    OnAccept(s);
                }
            }
            catch
            {
            }
        }

        private void OnAccept(ISuperSocket s)
        {
            if (null == _mainSocket)
                // this means, that server socket was forcibly stopped
                // don't call PrepareAccept any more
                return;

            try
            {
                if (null != s)
                {
                    //aClientSocket.SendTimeout = _sendTimeout;
                    
                    // if certificate is specified, the connection is created as SSL
                    var connection = new SimpleGeneralTcpConnection(this, s);

                    FireClientConnected(connection);

                    connection.StartReceive();
                }
            }
            catch
            {
                try
                {
                    if (null != s)
                        s.Close();
                }
                catch { }
            }
            /*finally
            {
            }*/
        }

        /// <summary>
        /// event raised, when the client is connected to this listening server
        /// </summary>
        public event Action<SimpleGeneralTcpConnection> Connected = null;

        
        private void FireClientConnected(SimpleGeneralTcpConnection connection)
        {
            if (null == connection)
                return;

            // register connection after the connected event was raised
            // especially necessary for delayed SSL connection event hadnling
            var aIPe = connection.RemoteEndPoint;

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
        
        protected internal override void InvokeDisconnected(SimpleGeneralTcpConnection connection)
        {
            FireClientDisconnected(connection);
        }

        /// <summary>
        /// event raised, when TCP client disconnects from this listening server
        /// </summary>
        public event Action<SimpleGeneralTcpConnection> Disconnected = null;

        private void FireClientDisconnected(SimpleGeneralTcpConnection connection)
        {
            if (null == connection)
                return;

            if (null != Disconnected)
                try
                {
                    if (UnregisterConnection(connection))
                    {
                        Disconnected(connection);

                        connection.Dispose();
                    }
                }
                catch//(Exception err)
                {
                    //string tmp = err.Message;
                }
        }

        /// <summary>
        /// MUST BE CALLED BEFORE THE SOCKET IS DISPOSED BY CLOSE METHOD !!!
        /// </summary>
        /// <param name="connection"></param>
        private bool UnregisterConnection(SimpleGeneralTcpConnection connection)
        {
            if (null == connection)
                return false;

            if (_connections.ContainsKey(connection.RemoteEndPoint))
            {
                _connections.Remove(connection.RemoteEndPoint);

                // srack overflow prevention
                //FireClientDisconnected(connection);

                return true;
            }
            
            return false;
        }


        /// <summary>
        /// event raised, when data has been received from any connected client
        /// </summary>
        public event Action<SimpleGeneralTcpConnection,ByteDataCarrier> DataReceived = null;

        protected override internal void InvokeDataReceived(SimpleGeneralTcpConnection connection,ByteDataCarrier data)
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
        /// sends data to a specified connection descriptor
        /// </summary>
        /// <param name="connection">connection descriptor</param>
        /// <param name="data">data to be sent</param>
        /// <exception cref="ArgumentNullException">if the connection or the data are null referrence</exception>
        /// <exception cref="ByteDataCarrier">if the connection or the data are null referrence</exception>
        protected internal override void Send(
            [NotNull] SimpleGeneralTcpConnection connection, 
            [NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(connection,"connection");

            connection.Send(data);
        }

        /// <summary>
        /// raised, when data populated to the client
        /// </summary>
        public event Action<SimpleGeneralTcpConnection,ByteDataCarrier> DataSent = null;

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

        protected internal override void InvokeSocketException(SimpleGeneralTcpConnection connection, SocketException socketException)
        {
            ExamineSocketError(connection, socketException);
        }

        private void ExamineSocketError(SimpleGeneralTcpConnection connection, SocketException socketError)
        {
            switch (socketError.ErrorCode)
            {
                case (int)SocketError.ConnectionReset:
                case (int)SocketError.ConnectionAborted:
                case (int)SocketError.ConnectionRefused:
                case (int)SocketError.HostDown:
                case (int)SocketError.HostUnreachable:
                case (int)SocketError.NetworkDown:
                case (int)SocketError.NetworkReset:
                case (int)SocketError.NetworkUnreachable:
                case (int)SocketError.NotConnected:
                case (int)SocketError.TimedOut:
                case (int)SocketError.Interrupted:
                    connection.Disconnect();
                    FireClientDisconnected(connection);
                    break;
                default:
#if DEBUG
                    Debug.WriteLine("Unexpected error: " + socketError.ErrorCode + 
                        "(" + socketError.ErrorCode + ")");
                    //Debug.Assert(false);
#endif
                    break;
            }
        }

        #region IEnumerable<SimpleTcpConnection> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SimpleGeneralTcpConnection> GetEnumerator()
        {
            return _connections.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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

        
        private int _sendTimeout = 1000;
        /// <summary>
        /// 
        /// </summary>
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
        }
    }
}
