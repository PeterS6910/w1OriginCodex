using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleGeneralTcpConnection : ADisposable, ISimpleTcpConnection
    {
        /// <summary>
        /// 
        /// </summary>
        private class ByteDataCarrierForTcpConnection : IProcessingQueueRequest
        {
            private readonly SimpleGeneralTcpConnection _tcpConnection;
            private readonly ByteDataCarrier _data;

            public ByteDataCarrierForTcpConnection(SimpleGeneralTcpConnection tcpConnection,ByteDataCarrier data)
            {
                _tcpConnection = tcpConnection;
                _data = data;
            }

            public void Execute()
            {
                _tcpConnection.SendData(_data);
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(new ArgumentNullException("ByteDataCarries is not defined"));
            }
        }

        private readonly ASimpleTcpPeer _handler;
        private volatile ISuperSocket _clientSocket;
        
        /*private volatile Socket _clientNetSocket;

        private readonly UnderlyingMode _underlyingMode;

        private enum UnderlyingMode
        {
            DirectSuperSocket,
            DirectNetSocket,

        }*/

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_clientSocket != null)
                    _clientSocket.Close();
            }
            catch
            {
            }
        }

        private readonly IPEndPoint _remoteEndPoint;

        /// <summary>
        /// remote IP endpoint of the connected client
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return _remoteEndPoint;
            }
        }

        private readonly IPEndPoint _localEndPoint;

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

        private volatile Stream _channelStream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="clientSocket"></param>
        protected internal SimpleGeneralTcpConnection(
            [NotNull] ASimpleTcpPeer handler, 
            [NotNull] ISuperSocket clientSocket)
        {
            Validator.CheckForNull(clientSocket,"clientSocket");
            Validator.CheckForNull(handler,"handler");

            _handler = handler;
            _clientSocket = clientSocket;
            _remoteEndPoint = clientSocket.RemoteEndPoint;
            _localEndPoint = clientSocket.LocalEndPoint;
           
            //_underlyingMode = UnderlyingMode.DirectSuperSocket;

            if (handler is SimpleTcpServer)
                PrepareServerStream();
            else
            {
                if (handler is SimpleTcpClient)
                    PrepareClientStream();
                else
                    throw new InvalidOperationException("Invalid handler type");
            }

            // DO NOT START IT HERE,
            // CAUSE DATA RECEPTION CAN PRECEDE CONNECT EVENT
            //StartReceive();
        }

        /*
        protected internal SimpleGeneralTcpConnection(ASimpleTcpPeer handler, Socket clientSocket)
        {
            Validator.CheckNull(clientSocket);
            Validator.CheckNull(handler);

            _handler = handler;
            _clientNetSocket = clientSocket;
            _underlyingMode = UnderlyingMode.DirectNetSocket;

            if (handler is SimpleTcpServer)
                PrepareServerStream();
            else
            {
                if (handler is SimpleTcpClient)
                    PrepareClientStream();
            }

            StartReceive();
        }*/

        private void PrepareServerStream()
        {
            try
            {
                var transportLayer = 
                    _handler.TransportSettings == null ? new PlainLayer() : _handler.TransportSettings.GetLayer();

                _channelStream = transportLayer.PrepareServerStream(_clientSocket);
            }
            catch
            {
                _channelStream = null;
            }
        }

        private void PrepareClientStream()
        {
            
            try
            {
                var transportLayer =
                    _handler.TransportSettings == null ? new PlainLayer() : _handler.TransportSettings.GetLayer();

                _channelStream = transportLayer.PrepareClientStream(_clientSocket);
            }
            catch
            {
                _channelStream = null;
            }
        }


        private void FireSocketException(Exception exception)
        {
            if (exception != null)
            {
                if (exception is SocketException)
                {
                    _handler.InvokeSocketException(this, exception as SocketException);
                }
                else
                {
                    if (exception.InnerException is SocketException)
                        _handler.InvokeSocketException(this, exception.InnerException as SocketException);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (null == _clientSocket)
                    return false;
                
                return _clientSocket.Connected;
            }
        }

        private ThreadPoolQueue<ByteDataCarrierForTcpConnection> _dataToSend2 = null; 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Send([NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(data,"data");

            if (_handler.UseSendingProcessingQueue)
            {
                if (null == _dataToSend2)
                {
                    _dataToSend2 = new ThreadPoolQueue<ByteDataCarrierForTcpConnection>(ThreadPoolGetter.Get());
                }

                _dataToSend2.Enqueue(new ByteDataCarrierForTcpConnection(this,data));
            }
            else
                SendData(data);
        }

        private readonly object _sendingDataLock = new object();

        /// <summary>
        /// 
        /// </summary>
        public event Action<ByteDataCarrier> DataSent;
        private void SendData(ByteDataCarrier data)
        {
            try
            {
                lock (_sendingDataLock)
                {
                    if (_channelStream != null)
                    {
                        _channelStream.Write(data.Buffer, 0, data.ActualSize);
                    }
                    else
                    {
                        _clientSocket.Send(data);
                    }
                }

                _handler.InvokeDataSent(this, data);

                if (null != DataSent)
                    try { DataSent(data); }
                    catch
                    {
                    }
            }
            catch (Exception error)
            {
                FireSocketException(error);
            }
        }

        private volatile SafeThread _readingThread = null;
        private readonly object _readingThreadSync = new object();

        /// <summary>
        /// 
        /// </summary>
        public void StartReceive()
        {
            if (null == _readingThread)
            {
                lock (_readingThreadSync)
                {
                    if (null == _readingThread)
                    {
                        var st = new SafeThread(ReceiveThread);
                        st.Start();

                        _readingThread = st;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<ByteDataCarrier> DataReceived;

        private void ReceiveThread()
        {
            var _singleReceiveBuffer = new ByteDataCarrier(_handler.BufferSize); // size to max, actualsize to 0

            try
            {
                while (true)
                {
                    int readBytes;

                    if (_channelStream != null)
                    {
                        readBytes = _channelStream.Read(_singleReceiveBuffer.Buffer, 0, _singleReceiveBuffer.Size); // here the max size of buffer must be used
                        
                    }
                    else
                    {
                        readBytes = _clientSocket.Receive(_singleReceiveBuffer);
                        
                    }

                    _singleReceiveBuffer.ActualSize = readBytes > 0 ? readBytes : 0;

                    if (readBytes > 0)
                    {
                        var clonedBuffer =
                            new ByteDataCarrier(_singleReceiveBuffer.Buffer, 0, readBytes);

                            // don't use this variant, cause it's replicating even the preallocated size
                            //new ByteDataCarrier(_singleReceiveBuffer,true);

                        _handler.InvokeDataReceived(this, clonedBuffer);

                        if (null != DataReceived)
                            try { DataReceived(clonedBuffer); }
                            catch { }
                    }
                    else
                    {
                        //_clientSocket.Close();
                        _handler.InvokeDisconnected(this);
                        break;
                    }
                }
            }
            //catch (SocketException)// e)
            //{
            //    _clientSocket.Close();
            //    _handler.FireClientDisconnected(this);
            //}
            catch (Exception error)
            {
                FireSocketException(error);
            }
            finally
            {
                _readingThread = null;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            if (_clientSocket != null)
            {
                try
                {
                    _clientSocket.Close();
                    _clientSocket.Dispose();
                }
                catch { }
                finally
                {
                    _clientSocket = null;
                }
            }

            if (null != _readingThread)
            {
                if (_readingThread.Thread.ManagedThreadId != System.Threading.Thread.CurrentThread.ManagedThreadId)
                {
                    try { _readingThread.Stop(10); }
                    catch { }
                }
                //else
                // otherwise most possibly the readingThread itself is calling the disposal and therefore it's on it's way to end

                _readingThread = null;
            }

            if (null != _dataToSend2)
            {
                try { _dataToSend2.Dispose(); }
                catch
                {
                }
                _dataToSend2 = null;
            }

            if (_channelStream != null)
            {
                try
                {
                    _channelStream.Close();
                }
                catch
                {
                }

                _channelStream = null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                if (_clientSocket != null)
                {
                    return string.Format("TcpConnection[{0}]<->{1}", _clientSocket.LocalEndPoint, _clientSocket.RemoteEndPoint);
                }
                
                return GetType().ToString();

            }
            catch
            {
                return GetType().ToString();
            }
        }

        
    }
}
