using System;
using System.Net;
using System.Net.Sockets;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// data carrier descriptor
    /// </summary>
    class SimpleUdpDataCarrier : IDisposable
    {
        public EndPoint _iPEndpoint;

        public ByteDataCarrier _data;

        public SimpleUdpDataCarrier(
            [NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(data,"data");

            _iPEndpoint = new IPEndPoint(IPAddress.Any, 0);
            _data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="data"></param>
        public SimpleUdpDataCarrier(
            [NotNull] IPEndPoint ipEndpoint,
            [NotNull] ByteDataCarrier data
            )
        {
            Validator.CheckForNull(ipEndpoint,"ipEndpoint");
            Validator.CheckForNull(data,"data");

            _iPEndpoint = ipEndpoint;
            _data = data;
        }

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }



    /// <summary>
    /// UDP peer class, equivalent to TcpClient and/or TcpListener
    /// </summary>
    public class SimpleUdpPeer : ADisposable, ISimpleUdpPeer
    {
        private readonly Socket _udpSocket = 
            new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        private readonly IPEndPoint _bindingEndpoint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindToIP"></param>
        /// <param name="bindToPort"></param>
        public SimpleUdpPeer(
            [NotNull] IPAddress bindToIP,
            int bindToPort)
        {
            Validator.CheckForNull(bindToIP,"bindToIP");

            if (bindToPort < 0)
                bindToPort = TcpUdpPort.GetUdpFreeDescending(Equals(bindToIP, IPAddress.Loopback));
            else
                // this allows 0 , so the system will decide the port
                TcpUdpPort.CheckValidity(bindToPort, true);


            _bindingEndpoint = new IPEndPoint(bindToIP, bindToPort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLocalOnly"></param>
        /// <param name="port"></param>
        public SimpleUdpPeer(bool isLocalOnly, int port)
        {
            if (port == 0)
                port = 0;// let it on the system assignment
            else
            {
                if (port < 0)
                    port = TcpUdpPort.GetUdpFreeDescending(isLocalOnly);
                else
                    TcpUdpPort.CheckValidity(port);
            }

            _bindingEndpoint = isLocalOnly ? new IPEndPoint(IPAddress.Loopback, port) : new IPEndPoint(IPAddress.Any, port);
        }

        /// <summary>
        /// client UDP peer, the binding port is assigned automaticaly
        /// </summary>
        /// <param name="isLocalOnly"></param>
        public SimpleUdpPeer(bool isLocalOnly)
            : this(isLocalOnly, 0)
        {
        }

        public IPAddress SourceAddress
        {
            get
            {
                return _bindingEndpoint.Address;
            }
            set
            {
                if (_listening)
                    throw new InvalidOperationException("Source address cannot be changed after the socket has been started");

                
// ReSharper disable once NotResolvedInText
                Validator.CheckForNull(value,"SourceAddress");

                _bindingEndpoint.Address = value;
            }
        }

        private int _bufferSize = 8192;
        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
            set
            {
                if (value >= 1)
                    _bufferSize = value;
            }
        }

        private bool _listening;
        /// <summary>
        /// 
        /// </summary>
        public bool IsListening
        {
            get
            {
                return _listening;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Start()
        {
            if (_listening)
                return;

// ReSharper disable once NotResolvedInText
            Validator.CheckForNull(_bindingEndpoint,"_bindingEndpoint");

            _udpSocket.Bind(_bindingEndpoint);

            if (_bindingEndpoint.Port == 0)
                _bindingEndpoint.Port = ((IPEndPoint)_udpSocket.LocalEndPoint).Port;

            if (null == _sendingQueue)
            {
                _sendingQueue = new ProcessingQueue<DataToIpe>();
                _sendingQueue.ItemProcessing += _sendingQueue_ItemProcessing;
            }

            PrepareReceive();

            _listening = true;
        }

        void _sendingQueue_ItemProcessing(DataToIpe parameter)
        {
            // wait until write available
            //while (!_udpSocket.Poll(100, SelectMode.SelectWrite)) ;

            try
            {
                _udpSocket.SendTo(parameter._data.Buffer, parameter._data.ActualSize, SocketFlags.None, parameter._ipEndpoint);
            }
// ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        public virtual void Stop()
        {
            if (!_listening)
                return;

            _sendingQueue.Clear();

            try { _udpSocket.Close(); }
            catch { }

            try
            {
                if (!_readingThread.Join(200))
                    _readingThread.Abort();
            }
            catch
            {
            }

            _listening = false;
        }

        private SafeThread _readingThread;

        private void PrepareReceive()
        {
            try
            {

                _receiveBuffer = new ByteDataCarrier(_bufferSize);

                _readingThread = new SafeThread(ReadingThread);
                _readingThread.Start();

            }
            catch//(Exception aError)
            {
            }
        }

        private ByteDataCarrier _receiveBuffer;
        private void ReadingThread()
        {
            try
            {
                EndPoint ipe = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {

                    //while (!_udpSocket.Poll(1000, SelectMode.SelectRead)) ;

                    int size = _udpSocket.ReceiveFrom(_receiveBuffer.Buffer, _receiveBuffer.Size, SocketFlags.None, ref ipe);
                    _receiveBuffer.ActualSize = size;

                    if (size > 0)
                        FireDataReceived((IPEndPoint)ipe, _receiveBuffer);
                }
            }
            catch (SocketException)// se)
            {

            }
            catch (Exception)// ge)
            {
            }
        }

        public event DUdpDataEvent DataReceived = null;
        protected virtual void OnDataReceived(IPEndPoint iPEndpoint, ByteDataCarrier data)
        {
        }

        public void ReleaseDataReceived()
        {
            DataReceived = null;
        }

        private void FireDataReceived(IPEndPoint ipEndpoint, ByteDataCarrier data)
        {
            try
            {
                OnDataReceived(ipEndpoint, data);
            }
            catch
            {
            }

            if (null != DataReceived)
            {
                try
                {
                    DataReceived(this, ipEndpoint, data);
                }
                catch
                {
                }
            }
        }


        public void SetSocketOption(SocketOptionLevel optionLevel,
            SocketOptionName optionName,
            bool enable)
        {
            try
            {
                _udpSocket.SetSocketOption(optionLevel, optionName, enable);
            }
            catch//(Exception error)
            {
            }
        }

        public void SetSocketOption(SocketOptionLevel optionLevel,
            SocketOptionName optionName,
            int value)
        {
            try
            {
                _udpSocket.SetSocketOption(optionLevel, optionName, value);
            }
            catch// (Exception error)
            {
            }
        }


        public bool SetMulticastInterface(IPAddress mcastSource)
        {
            try
            {
                if (null == mcastSource)
                    _udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, 0);
                else
                {
                    int ipBinary = BitConverter.ToInt32(mcastSource.GetAddressBytes(), 0);

                    _udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, ipBinary);
                }

                return true;

            }
            catch
            {
                return false;
            }
        }

        public bool AddMulticastMembership(IPAddress mcastGroup)
        {
            Validator.CheckInvalidOperation(!_listening);

            try
            {
                _udpSocket.SetSocketOption(
                    SocketOptionLevel.IP,
                    SocketOptionName.AddMembership,
                    new MulticastOption(mcastGroup, _bindingEndpoint.Address));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void RemoveMulticastMembership(IPAddress mcastGroup)
        {
            Validator.CheckInvalidOperation(!_listening);
            _udpSocket.SetSocketOption(
                SocketOptionLevel.IP,
                SocketOptionName.DropMembership,
                new MulticastOption(mcastGroup, IPAddress.Any));
        }

        public bool AllowBroadcast
        {
            get
            {
                return ((int)_udpSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast)) > 0;
            }
            set
            {
                _udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, value ? 1 : 0);
            }
        }

        public bool AllowReuseAddress
        {
            get
            {
                return (bool)_udpSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress);
            }
            set
            {
                _udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, value);
            }
        }

        private class DataToIpe
        {
            internal readonly IPEndPoint _ipEndpoint;
            internal readonly ByteDataCarrier _data;

            public DataToIpe(
                [NotNull] IPEndPoint ipEndpoint, 
                [NotNull] ByteDataCarrier data)
            {
                _ipEndpoint = ipEndpoint;
                _data = data;
            }
        }

        private ProcessingQueue<DataToIpe> _sendingQueue;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="data"></param>
        public void Send(
            [NotNull] IPEndPoint ipEndpoint, 
            [NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(ipEndpoint, "ipEndpoint");
            Validator.CheckForNull(data,"data");

            if (null == data ||
                data.ActualSize == 0)
                throw new InvalidOperationException("Data must not be empty");

            try
            {

                _sendingQueue.Enqueue(new DataToIpe(ipEndpoint, data));

            }
            catch (Exception)
            {
            }
        }


        //private void OnSend(IAsyncResult result)
        //{
        //    if (null == result)
        //        return;

        //    SimpleUdpDataCarrier aAsyncDataCarrier = (SimpleUdpDataCarrier)result.AsyncState;


        //    try
        //    {
        //        _udpSocket.EndSendTo(result);
        //        /*#if DEBUG
        //                        if (iSend != aAsyncDataCarrier._data.ActualSize)
        //                            Contal.IwQuickCF.UI.CConsole.Warning("Different sizes " + iSend + " " + aAsyncDataCarrier._data.ActualSize);
        //        #endif*/

        //        //aAsyncDataCarrier._data.ActualSize = iSend;

        //        FireDataSent(aAsyncDataCarrier);

        //    }
        //    catch
        //    {
        //        return;
        //    }

        //}

        public event DUdpDataEvent DataSent = null;
        protected virtual void OnDataSent(IPEndPoint iPEndpoint, ByteDataCarrier data)
        {
        }


        protected override void InternalDispose(bool isExplicitDispose)
        {
            Stop();

            try
            {
                if (_sendingQueue != null)
                    _sendingQueue.Dispose();
            }
            catch { }
        }
    }
}
