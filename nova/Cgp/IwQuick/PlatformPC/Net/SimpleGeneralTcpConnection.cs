using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public class SimpleGeneralTcpConnection : ADisposable, ISimpleTcpConnection
    {
        private readonly ASimpleTcpPeer _handler;
        private readonly Socket _clientSocket;

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            try
            {
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

        private SimpleGeneralTcpConnection(
            [NotNull] ASimpleTcpPeer handler, 
            [NotNull] Socket clientSocket)
        {
            _handler = handler;
            _clientSocket = clientSocket;
            _remoteEndPoint = (IPEndPoint) clientSocket.RemoteEndPoint;
            _localEndPoint = (IPEndPoint) clientSocket.LocalEndPoint;

            if (handler is SimpleTcpServer)
                PrepareServerStream();
            else
            {
                if (handler is SimpleTcpClient)
                    PrepareClientStream();
                else
                    throw new InvalidOperationException("Invalid handler type");
            }
        }

        protected internal static ISimpleTcpConnection Create(
            [NotNull] ASimpleTcpPeer handler, 
            [NotNull] Socket clientSocket)
        {
            return new SimpleGeneralTcpConnection(handler, clientSocket);
        }

        private void PrepareServerStream()
        {
            ITransportLayer transportLayer = 
                _handler.TransportSettings != null 
                    ? _handler.TransportSettings.GetLayer() 
                    : new PlainLayer();

            _channelStream = transportLayer.PrepareServerStream(_clientSocket);
        }

        private void PrepareClientStream()
        {
            ITransportLayer transportLayer = 
                _handler.TransportSettings != null 
                    ? _handler.TransportSettings.GetLayer() 
                    : new PlainLayer();

            _channelStream = transportLayer.PrepareClientStream(_clientSocket);
        }

        //public void Send(Contal.IwQuick.Data.ByteDataCarrier data)
        //{
        //    _handler.Send(this, data);
        //}

        private Stream _channelStream;

        private volatile SafeThread _readingThread;
        private readonly object _readingThreadSync = new object();

        public void StartReceive()
        {
            if (null == _readingThread)
                lock (_readingThreadSync)
                {
                    if (null == _readingThread)
                    {
                        SafeThread st = new SafeThread(ReceiveThread);
                        st.Start();

                        _readingThread = st;
                    }
                }
        }

        private byte[] _singleReceiveBuffer;

        private void FireSocketException(Exception exception)
        {
            if (exception != null)
            {
                if (exception is SocketException)
                {
                    _handler.FireSocketException(this, exception as SocketException);
                }
                else
                {
                    if (exception.InnerException is SocketException)
                        _handler.FireSocketException(this, exception.InnerException as SocketException);
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                if (null == _clientSocket)
                    return false;
                
                return _clientSocket.Connected;
            }
        }

        private volatile ProcessingQueue<ByteDataCarrier> _dataToSend = null;
        private readonly object _dataToSendLock = new object();

        public void Send([NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(data,"data");

            if (_handler.UseSendingProcessingQueue)
            {
                if (null == _dataToSend)
                {
                    lock (_dataToSendLock)
                    {
                        if (null == _dataToSend)
                        {
                            _dataToSend = new ProcessingQueue<ByteDataCarrier>();
                            _dataToSend.ItemProcessing += SendData;
                        }
                    }
                }

                _dataToSend.Enqueue(data);
            }
            else
                SendData(data);
        }

        private readonly object _sendDataLock = new object();

        private void SendData(ByteDataCarrier data)
        {
            try
            {
                // lock this section to avoid inatomicity on underlying _channelStream instances
                lock (_sendDataLock)
                {
                    if (_channelStream != null)
                    {
                        _channelStream.Write(data.Buffer, 0, data.ActualSize);
                    }
                    else
                    {
                        _clientSocket.Send(data.Buffer,data.ActualSize,SocketFlags.None);
                    }
                }

                _handler.FireDataSent(this, data);
            }
            catch (Exception e)
            {
                FireSocketException(e);
            }
        }

        private void ReceiveThread()
        {
            if (null == _singleReceiveBuffer)
                _singleReceiveBuffer = new byte[_handler.BufferSize];

            try
            {
                while (true)
                {
                    int numBytesRead;

// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (_channelStream != null)
                    {
                        numBytesRead = _channelStream.Read(_singleReceiveBuffer, 0, _singleReceiveBuffer.Length);
                    }
                    else
                    {
                        numBytesRead = _clientSocket.Receive(_singleReceiveBuffer);
                    }

                    if (numBytesRead > 0)
                    {
                        ByteDataCarrier clonedBuffer = new ByteDataCarrier(_singleReceiveBuffer, 0, numBytesRead);

                        _handler.FireDataReceived(this, clonedBuffer);
                    }
                    else
                    {
                        //_clientSocket.Close();
                        _handler.FireDisconnected(this);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                FireSocketException(e);
            }
            finally
            {
                _readingThread = null;
            }
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            if (_clientSocket != null)
            {
                try
                {
                    _clientSocket.Close();
                }
                catch { }
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

            if (null != _dataToSend)
            {
                try { _dataToSend.Dispose(); }
                catch
                {
                }
                _dataToSend = null;
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
    }
}
