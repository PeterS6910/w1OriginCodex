using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using Contal.IwQuick.Net;
using System.Net.NetworkInformation;
using Contal.IwQuick.Threads;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// Remoting peer based on TcpChannel and binary serialization/formatting
    /// </summary>
    public class TcpRemotingPeer : ARemotingPeer
    {
        private static int _channelCounter; // for dynamic allocation of channel names

        private readonly string _bindToIp = string.Empty;
        private string _connectTo;
        private int _connectTcpPort = -1;

        private int _localPort = -1;
        private bool _isLocalOnly;

        public bool Multihomed
        {
            get
            {
                return 
					(string.IsNullOrEmpty(_bindToIp) ||
                    _bindToIp.Equals(StringConstants.ANY_IPV4)) 
                    && !_isLocalOnly;
            }
        }

        private AESSettings _aesSettings;

        private int _dynamicPortsLowBound = 1024;
        /// <summary>
        /// 
        /// </summary>
        public int DynamicPortsLowBound
        {
            get
            {
                return _dynamicPortsLowBound;
            }
            set
            {
                TcpUdpPort.CheckValidity(value, false);

                if (value >= _dynamicPortsUpperBound)
                    throw new OutOfRangeException(value, 1, _dynamicPortsUpperBound - 1);

                _dynamicPortsLowBound = value;
            }
        }

        private int _dynamicPortsUpperBound = ushort.MaxValue;
        /// <summary>
        /// 
        /// </summary>
        public int DynamicPortsUpperBound
        {
            get
            {
                return _dynamicPortsUpperBound;
            }
            set
            {
                TcpUdpPort.CheckValidity(value, false);

                if (value <= _dynamicPortsLowBound)
                    throw new OutOfRangeException(value, _dynamicPortsLowBound +1, ushort.MaxValue);

                _dynamicPortsUpperBound = value;
            }
        }

        /// <summary>
        /// general URL identification for TCP-based channels
        /// </summary>
        /// <param name="requiredType"></param>
        /// <returns></returns>
        protected override string GetRemotingUrl(Type requiredType)
        {
            if (null == requiredType)
                return null;

            return string.Format("tcp://{0}:{1}/{2}", _connectTo, _connectTcpPort, requiredType.Name);
        }

        /// <summary>
        /// IChannel instance preparation according to current TCP implementation
        /// </summary>
        /// <returns></returns>
        /// 

        protected override IChannelSender CreateClientChannel()
        {
            IDictionary channelProperties = new Hashtable(8);

            channelProperties["name"] = 
                string.Concat(
                    LOCAL_CHANNEL_PREFIX,
                    IsServer ? "Server" : "Client",
                    Process.GetCurrentProcess().Id,
                    StringConstants.SLASH,
                    _channelCounter++);

            // prevent strict versioning
            channelProperties["includeVersions"] = false;

            IClientChannelSinkProvider clientChannelSinkProvider =
                null;

            if (_aesSettings != null)
                clientChannelSinkProvider =
                    new AesCryptoClientSinkProvider(_aesSettings);

            clientChannelSinkProvider =
                new ClientIdentificationClientSinkProvider
                    {
                        Next = clientChannelSinkProvider
                    };

            clientChannelSinkProvider =
                new MultihomingConfigurationClientSinkProvider(Multihomed)
                {
                    Next = clientChannelSinkProvider
                };

            return 
                new TcpClientChannel(
                    channelProperties, 
                    new BinaryClientFormatterSinkProvider
                        {
                            Next = clientChannelSinkProvider
                        });
        }

        private IChannelReceiver CreateServerChannel(
            string ipBindTo)
        {
#if TRACE_REMOTING
            Trace.WriteLine(
                string.Concat(
                    "TcpRemotingPeer: Creating server channel for IP=", 
                    ipBindTo));
#endif
            // IChannel configuration
            IDictionary channelProperties = new Hashtable(8);
            channelProperties["useIpAddress"] = true;
            channelProperties["rejectRemoteRequests"] = _isLocalOnly;

            channelProperties["name"] = 
                string.Concat(
                    LOCAL_CHANNEL_PREFIX,
                    IsServer ? "Server" : "Client",
                    Process.GetCurrentProcess().Id,
                    StringConstants.SLASH,
                    _channelCounter++);

            // prevent strict versioning
            channelProperties["includeVersions"] = false;

            //specify exact IP address for the remoting to be bound (avoid connections from different ifaces in multihomed environment)
            if (IPHelper.IsValid(ipBindTo))
                channelProperties["bindTo"] = ipBindTo;

            int localPort = _localPort;
            bool dynamicPort = _localPort <= 0;

            int lowBound = _dynamicPortsLowBound;

            IChannelReceiver channel = null;

            while (channel == null)
            {
                if (dynamicPort)
                    localPort = 
                        TcpUdpPort.GetTcpFree(
                            _isLocalOnly, 
                            lowBound, 
                            _dynamicPortsUpperBound);

                // _localPort is 0, it's choosen by remoting framework
                channelProperties["port"] = localPort;

                try
                {
                    channel = CreateServerChannel(channelProperties);
                }
                catch (SocketException socketError)
                {
                    if (!dynamicPort)
                        throw;

                    if (socketError.ErrorCode != 10048)
                        throw;

                    if (
                            localPort == _dynamicPortsUpperBound ||
                            lowBound == _dynamicPortsUpperBound)
                        throw;

                    lowBound++;
                }
            }

            return channel;
        }

        private IChannelReceiver CreateServerChannel(IDictionary channelProperties)
        {
            IServerChannelSinkProvider serverSinkProvider =
                new BinaryServerFormatterSinkProvider
                    {
                        TypeFilterLevel = TypeFilterLevel.Full
                    };

            serverSinkProvider =
                new MultihomingConfigurationServerSinkProvider(Multihomed)
                    {
                        Next = serverSinkProvider
                    };

            serverSinkProvider =
                new ClientIdentificationServerSinkProvider
                    {
                        Next = serverSinkProvider
                    };

            if (_aesSettings != null)
                serverSinkProvider =
                    new AesCryptoServerSinkProvider(_aesSettings)
                        {
                            Next = serverSinkProvider
                        };

            return 
                new TcpServerChannel(
                    channelProperties, 
                    serverSinkProvider);
        }

        private const string LOCAL_CHANNEL_PREFIX = "TCP/";

        protected override void PrepareServerChannels(
            IDictionary<string, ChannelInfo<IChannelReceiver>> channelSet)
        {
            if (Multihomed)
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    PrepareServerChannelsForInterface(
                        ni,
                        channelSet);
            else
                PrepareServerChannel(
                    _bindToIp ?? string.Empty,
                    channelSet);

        }

        private void PrepareServerChannel(
            string address, 
            IDictionary<string, ChannelInfo<IChannelReceiver>> channelSet)
        {
            ChannelInfo<IChannelReceiver> channelInfo = GetServerChannelInfo(address);

            if (channelInfo == null)
            {
                IChannelReceiver channel =
                    CreateServerChannel(address);

                if (channel != null)
                    channelInfo = new ChannelInfo<IChannelReceiver>(channel);
            }

            if (channelInfo != null)
                channelSet.Add(address, channelInfo);
        }

        private void PrepareServerChannelsForInterface(
            NetworkInterface ni, 
            IDictionary<string, ChannelInfo<IChannelReceiver>> channelSet)
        {
            if (ni.OperationalStatus != OperationalStatus.Up)
                return;

            IPInterfaceProperties ipProps = ni.GetIPProperties();

            if (ipProps == null) 
                return;

            UnicastIPAddressInformationCollection uipList = 
                ipProps.UnicastAddresses;

            if (uipList == null) 
                return;
#if TRACE_REMOTING
            Trace.WriteLine(
                string.Concat(
                    "Preparing server channel for interface ",
                    ni));
#endif
            foreach (UnicastIPAddressInformation uip in uipList)
            {
                if (uip.Address.AddressFamily != AddressFamily.InterNetwork) 
                    continue;

                if (uip.SuffixOrigin != SuffixOrigin.Manual && 
                    uip.SuffixOrigin != SuffixOrigin.WellKnown &&
                    uip.SuffixOrigin != SuffixOrigin.OriginDhcp)
                {
                    continue;
                }

                string address = uip.Address.ToString();

                PrepareServerChannel(
                    address,
                    channelSet);
            }
        }

        private void BindNetworkInterfaceEvents()
        {
            NetworkChange.NetworkAddressChanged += 
                NetworkChange_NetworkAddressChanged;

            NetworkChange.NetworkAvailabilityChanged += 
                NetworkChange_NetworkAvailabilityChanged;
        }

        private readonly object _networkChangedTimeoutSync = new object();
        private ITimer _networkChangedTimeout;

        private bool OnNetworkChangedTimeout(TimerCarrier timercarrier)
        {
            lock (_networkChangedTimeoutSync)
            {
                EnsureChannelRegistration();

                _networkChangedTimeout = null;
            }

            return true;
        }

        private void OnNetworkChanged()
        {
            if (!Multihomed)
                return;

            lock (_networkChangedTimeoutSync)
                if (_networkChangedTimeout == null)
                    _networkChangedTimeout =
                        TimerManager.Static.StartTimeout(
                            500, 
                            OnNetworkChangedTimeout);
        }

        private void NetworkChange_NetworkAvailabilityChanged(
            object sender, 
            NetworkAvailabilityEventArgs e)
        {
            OnNetworkChanged();
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            OnNetworkChanged();
        }

        /// <summary>
        /// Server-side Tcp-based remoting channel
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port">TCP port</param>
        /// <param name="isLocal">if true, the Remoting channel is bound only to 127.0.0.1</param>
        /// <param name="aesSettings"></param>
        private TcpRemotingPeer(string ipAddress, int port, bool isLocal, AESSettings aesSettings)
            : base(true, false)
        {
            if (port > 0)
                TcpUdpPort.CheckValidity(port, false);

            _localPort = port;
            _isLocalOnly = isLocal;
            _aesSettings = aesSettings;
            _bindToIp = ipAddress;

            BindNetworkInterfaceEvents();
        }


        /// <summary>
        /// Server-side Tcp-based remoting channel
        /// </summary>
        /// <param name="port">TCP port</param>
        /// <param name="isLocal">if true, the Remoting channel is bound only to 127.0.0.1</param>
        private TcpRemotingPeer(int port, bool isLocal)
            : base(true, false)
        {
            if (port > 0)
                TcpUdpPort.CheckValidity(port, false);

            _localPort = port;
            _isLocalOnly = isLocal;
            _aesSettings = null;

            BindNetworkInterfaceEvents();
        }

        /// <summary>
        /// returns server side remoting peer
        /// </summary>
        /// <param name="bindToIp"></param>
        /// <param name="port"></param>
        /// <param name="isLocal"></param>
        /// <param name="aesSettings">can be null</param>
        /// <returns></returns>
        public static TcpRemotingPeer GetServerSide(
            string bindToIp, 
            int port, 
            bool isLocal, 
            AESSettings aesSettings)
        {
            return 
                new TcpRemotingPeer(
                    bindToIp, 
                    port, 
                    isLocal, 
                    aesSettings);
        }

        /// <summary>
        /// returns server side remoting peer
        /// </summary>
        /// <param name="port"></param>
        /// <param name="isLocal"></param>
        /// <returns></returns>
        public static TcpRemotingPeer GetServerSide(
            int port, 
            bool isLocal)
        {
            return 
                new TcpRemotingPeer(
                    port, 
                    isLocal);
        }

        /// <summary>
        /// Client-side Tcp-based remoting channel
        /// </summary>
        /// <param name="destinationHost"></param>
        /// <param name="destinationPort"></param>
        /// <param name="localPort"></param>
        /// <param name="aesSettings"></param>
        /// <exception cref="ArgumentNullException">if destination host is null or empty</exception>
        /// <exception cref="InvalidTransportPortException">if the specified destination or </exception>
        private TcpRemotingPeer(string destinationHost, int destinationPort, int localPort, AESSettings aesSettings)
            : base(false, false)
        {
            ConfigureClient(destinationHost, destinationPort, localPort, aesSettings);
        }

        public static TcpRemotingPeer GetClientSide(string destinationHost, int destinationPort)
        {
            return new TcpRemotingPeer(destinationHost, destinationPort, 0, null);
        }

        /// <summary>
        /// returns client side remoting peer instance
        /// </summary>
        /// <param name="destinationHost"></param>
        /// <param name="destinationPort"></param>
        /// <param name="localPort"></param>
        /// <param name="aesSettings"></param>
        /// <returns></returns>
        public static TcpRemotingPeer GetClientSide(string destinationHost, int destinationPort, int localPort, AESSettings aesSettings)
        {
            return new TcpRemotingPeer(destinationHost, destinationPort, localPort, aesSettings);
        }

        private void ConfigureClient(string destinationHost, int destinationPort, int localPort, AESSettings aesSettings)
        {
            Validator.CheckNullString(destinationHost);
            TcpUdpPort.CheckValidity(destinationPort);

            if (localPort > 0)
                TcpUdpPort.CheckValidity(localPort, false);

            _connectTo = destinationHost;
            _connectTcpPort = destinationPort;
            _aesSettings = aesSettings;

            _isLocalOnly = IPHelper.IsLoopBack(destinationHost);
            _localPort = localPort > 0 ? localPort : -1;
        }

        public void Reconfigure(bool multihomed, string destinationHost, int destinationPort, int localPort, AESSettings aesSettings)
        {
            //if (IsServer)
            //    throw new InvalidOperationException();

            UnregisterChannels();

            ConfigureClient(destinationHost, destinationPort, localPort, aesSettings);

            RestartProxyKeepers();
        }

    }
}
