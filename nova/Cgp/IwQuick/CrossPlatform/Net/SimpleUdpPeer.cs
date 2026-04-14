using System;
using System.Net;
using System.Net.Sockets;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

using Contal.IwQuick.Threads;
using Contal.IwQuick.Data;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleUdpPeer : ADisposable, ISimpleUdpPeer
    {
        private readonly Socket _udpSocket =
            new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        private readonly IPEndPoint _bindingEndpoint = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindToIP"></param>
        /// <param name="bindToPort"></param>
        public SimpleUdpPeer(
            [NotNull] IPAddress bindToIP,
            int bindToPort)
        {
            Validator.CheckForNull(bindToIP, "bindToIP");

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

            _bindingEndpoint = new IPEndPoint(
                isLocalOnly
                    ? IPAddress.Loopback
                    : IPAddress.Any, port);

        }

        /// <summary>
        /// client UDP peer, the binding port is assigned automaticaly
        /// </summary>
        /// <param name="isLocalOnly"></param>
        public SimpleUdpPeer(bool isLocalOnly)
            : this(isLocalOnly, 0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
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

                Validator.CheckForNull(value, "value");

                _bindingEndpoint.Address = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("No need to use anymore, as reading buffer fixed to 64K")]
        public int BufferSize
        {
            get { return PreallocBdcPool.Implicit64kPreallocSize; }

            set
            {
                // INTENTIONALLY DONE NOTHING
                // BEST RESULTS WITH UDP READING ARE WITH FULL DGRAM SIZE
                DebugHelper.Keep(value);
            }
        }

        private volatile bool _listening = false;
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

        private readonly object _startStopLock = new object();

        /// <summary>
        /// starts listening on the UDP ip:port
        /// </summary>
        public virtual void Start()
        {
            // ReSharper disable once NotResolvedInText
            Validator.CheckForNull(_bindingEndpoint, "Binding endpoint is not set");

            if (!_listening)
                lock (_startStopLock)
                {
                    if (!_listening)
                    {
                        _udpSocket.Bind(_bindingEndpoint);

                        if (_bindingEndpoint.Port == 0)
                            _bindingEndpoint.Port = ((IPEndPoint)_udpSocket.LocalEndPoint).Port;

                        if (null == _sendingQueue)
                            _sendingQueue =
                                new ThreadPoolQueue<DataToIpe, Socket>(
                                    ThreadPoolGetter.Get(),
                                    _udpSocket);
                        else
                            _sendingQueue.Unblock();

                        PrepareReceive();

                        _listening = true;
                    }
                }
        }

        /// stops the UDP listening, if no other Stop is already in progress
        /// </summary>
        public virtual void Stop()
        {
            if (_listening)
                lock (_startStopLock)
                {
                    if (_listening)
                    {
                        _readingThreadRunning = false;

                        _sendingQueue.ClearAndBlock();
                        _sendingQueue.WaitUntilIdle();

                        try { _udpSocket.Close(); }
                        catch { }

                        try
                        {
                            // expected that the thread stops on SocketException when 
                            // _udpSocket is closed above this call
                            _readingThread.Join(500);

                            // if it's hanging due FireDataReceived call
                            // leave it 
                        }
                        catch
                        {
                        }
                        finally
                        {
                            _readingThread = null;
                        }

                        _listening = false;
                    }
                }
        }

        private volatile bool _readingThreadRunning;
        private volatile SafeThread _readingThread;

        /// <summary>
        /// 
        /// </summary>
        private void PrepareReceive()
        {
            try
            {

                _readingThreadRunning = true;
                _readingThread = new SafeThread(ReadingThread);
                _readingThread.Start();

            }
            catch//(Exception aError)
            {
            }
        }


        private void ReadingThread()
        {
            try
            {
                EndPoint ipe = new IPEndPoint(IPAddress.Any, 0);
                while (_readingThreadRunning)
                {

                    _udpSocket.Poll(-1, SelectMode.SelectRead);

                    var buffer = PreallocBdcPool.Implicit64k.Get();

                    try
                    {

                        int size = _udpSocket.ReceiveFrom(buffer.Buffer, PreallocBdcPool.Implicit64kPreallocSize, SocketFlags.None,
                            ref ipe);
                        buffer.ActualSize = size;

                        if (size > 0)
                            FireDataReceived((IPEndPoint)ipe, buffer);
                    }
                    finally
                    {
                        PreallocBdcPool.Implicit64k.Return(buffer);
                    }
                }
            }
            catch (SocketException)// se)
            {

            }
            catch (Exception)// ge)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event DUdpDataEvent DataReceived = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iPEndpoint"></param>
        /// <param name="data"></param>
        protected virtual void OnDataReceived(IPEndPoint iPEndpoint, ByteDataCarrier data)
        {
        }

        /// <summary>
        /// 
        /// </summary>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionLevel"></param>
        /// <param name="optionName"></param>
        /// <param name="enable"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionLevel"></param>
        /// <param name="optionName"></param>
        /// <param name="value"></param>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mcastSource"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mcastGroup"></param>
        /// <exception cref="ArgumentNullException">if mcastGroup is null</exception>
        /// <exception cref="InvalidOperationException">if listening on the UDP port has not yet been started</exception>
        /// <returns></returns>
        public bool AddMulticastMembership([NotNull] IPAddress mcastGroup)
        {
            Validator.CheckForNull(mcastGroup, "mcastGroup");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mcastGroup"></param>
        /// <exception cref="ArgumentNullException">if mcastGroup is null</exception>
        /// <exception cref="InvalidOperationException">if listening on the UDP port has been already stopped</exception>
        public void RemoveMulticastMembership([NotNull] IPAddress mcastGroup)
        {
            Validator.CheckForNull(mcastGroup, "mcastGroup");
            Validator.CheckInvalidOperation(!_listening);

            _udpSocket.SetSocketOption(
                SocketOptionLevel.IP,
                SocketOptionName.DropMembership,
                new MulticastOption(mcastGroup, IPAddress.Any));
        }

        /// <summary>
        /// if true, sending broadcasts from this socket would be allowed
        /// </summary>
        public bool AllowBroadcast
        {
            get
            {
                return ((int)_udpSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast)) > 0;
            }
            set
            {
                SetBroadcastAllowed(value);
            }
        }

        private void SetBroadcastAllowed(bool value)
        {
            _udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, value ? 1 : 0);
        }

        /// <summary>
        /// if true, this UDP socket and other subsequent with this flag can share the same UDP port
        /// </summary>
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
        private ThreadPoolQueue<DataToIpe, Socket> _sendingQueue;

        /// <summary>
        /// 
        /// </summary>
        private class DataToIpe : IProcessingQueueRequest<Socket>
        {
            private readonly IPEndPoint _ipEndpoint;
            private readonly ByteDataCarrier _data;

            internal DataToIpe(
                [NotNull] IPEndPoint ipEndpoint,
                [NotNull] ByteDataCarrier data)
            {
                _ipEndpoint = ipEndpoint;
                _data = data;
            }

            public void Execute(Socket socket)
            {
                socket.SendTo(
                    _data.Buffer,
                    _data.ActualSize,
                    SocketFlags.None,
                    _ipEndpoint);
            }

            public void OnError(Socket param, Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException">if ipEndpoint or data are null</exception>
        /// <exception cref="InvalidOperationException">if the data are empty</exception>
        public void Send(
            [NotNull] IPEndPoint ipEndpoint,
            [NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(ipEndpoint, "ipEndpoint");
            Validator.CheckForNull(data, "data");

            if (data.ActualSize == 0)
                throw new InvalidOperationException("Data must not be empty");

            try
            {
                _sendingQueue.Enqueue(
                    new DataToIpe(
                        ipEndpoint,
                        data));

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
        //                            Contal.IwQuick.UI.CConsole.Warning("Different sizes " + iSend + " " + aAsyncDataCarrier._data.ActualSize);
        //        #endif*/

        //        //aAsyncDataCarrier._data.ActualSize = iSend;

        //        FireDataSent(aAsyncDataCarrier);

        //    }
        //    catch
        //    {
        //        return;
        //    }

        //}

        /// <summary>
        /// raised when the packet is sent
        /// </summary>
        public event DUdpDataEvent DataSent = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iPEndpoint"></param>
        /// <param name="data"></param>
        protected virtual void OnDataSent(IPEndPoint iPEndpoint, ByteDataCarrier data)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="data"></param>
        // ReSharper disable once UnusedMember.Local
        private void FireDataSent(IPEndPoint ipEndpoint, ByteDataCarrier data)
        {
            try
            {
                OnDataSent(ipEndpoint, data);
            }
            catch
            {
            }

            if (null != DataSent)
            {
                try
                {
                    DataSent(
                        this,
                        ipEndpoint,
                        data);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            Stop();

            if (_sendingQueue != null) try
                {
                    _sendingQueue.Dispose();
                }
                catch { }
        }
    }
}
