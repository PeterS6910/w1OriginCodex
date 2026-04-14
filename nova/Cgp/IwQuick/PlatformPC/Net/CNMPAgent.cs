//#define DEBUG_OUTPUTS

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using System.Net;
using System.Xml;
using System.Threading;
using Contal.IwQuick.Crypto;

using System.Net.NetworkInformation;
using Contal.IwQuick.Data;
using System.Diagnostics;
using System.Net.Sockets;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;


namespace Contal.IwQuick.Net
{
    /// <summary>
    /// class for secure lookup of entities over network via UDP protocol
    /// </summary>
    /// PC
    public partial class CNMPAgent : SimpleUdpPeer
    {
        //private TimerCallback _tcb;
        private volatile Timer _timerNew;
        private readonly object _timerNewChangeLock = new object();

        public class InterfacePeer : ADisposable
        {
            public IPAddress ipAddress = null;
            public IPAddress subnetMask = null;
            public NetworkInterface networkInterface = null;

            public SimpleUdpPeer udpPeer = null;
            //public 

            protected override void InternalDispose(bool isExplicitDispose)
            {
                if (udpPeer != null)
                    try
                    {
                        udpPeer.Stop();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        udpPeer = null;
                    }

                networkInterface = null;
            }


        }

        //Defines if operationl status of the interfaces are check each time BeginLookup is called
        //true: interfaces are check on each BeginLookup 
        //false: interfaces are check only on NetworkAddressChanged event
        private bool _checkInterfacesOnLookup = false;
        public bool CheckInterfacesOnLookup
        {
            get { return _checkInterfacesOnLookup; }
            set { _checkInterfacesOnLookup = value; }
        }

        //Specify the name of one interface that InterfacePeer can be bound to
        private string _monitorInterfaceName = string.Empty;
        public string MonitorInterfaceName
        {
            get { return _monitorInterfaceName; }
            set { _monitorInterfaceName = value; }
        }

        private volatile object _peersSync = new object();
        private readonly Dictionary<IPAddress, InterfacePeer> _peers = new Dictionary<IPAddress, InterfacePeer>(8);

        

        




        /// <summary>
        /// creates instance of the UDP lookup entity allowing lookups and also identifying itself
        /// </summary>
        /// <param name="udpPort">UDP port to run on</param>
        /// <param name="instanceName">string specificator of this entity instance</param>
        /// <param name="typeName">string specificator of this entity type</param>
        /// <param name="keys"></param>
        /// <exception cref="ArgumentNullException">if instance name or type name is null or empty string</exception>
        public CNMPAgent(
            int udpPort, 
            string instanceName, 
            [NotNull] string typeName, 
            params string[] keys)
            : base(false, (udpPort > 0 ? udpPort : DefaultCnmpPort))
        {
            ConstructorCore(udpPort,instanceName,typeName,keys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="typeName"></param>
        public CNMPAgent(string instanceName, string typeName)
            : this(0, instanceName, typeName, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        public CNMPAgent(string typeName)
            : this(0, null, typeName, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="typeName"></param>
        /// <param name="keys"></param>
        public CNMPAgent(string instanceName, string typeName, params string[] keys)
            : this(0, instanceName, typeName, keys)
        {
        }

        private void CloseBoundPeers()
        {
            lock (_peers)
            {
                if (_peers.Count > 0)
                {
                    
                    foreach (var ipeer in _peers.Values)
                    {
                        try
                        {
                            ipeer.Dispose();
                        }
                        catch
                        {
                            
                        }
                    }

                    _peers.Clear();
                }
            }
        }

        /// <summary>
        /// starts CNMP responding on default or explicit port
        /// </summary>
        public override void Start()
        {
            base.Start();

            CheckInterfacesStatus();

            //if (!_checkInterfacesOnLookup)
            {
                NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            }           

            //_tcb = OnRequestTimeout;
            _multicastResolved = AddMulticastMembership(MulticastIp);
        }

        public override void Stop()
        {
            if (_multicastResolved)
            {
                try { RemoveMulticastMembership(MulticastIp); }
                catch { }
                _multicastResolved = false;
            }

            base.Stop();
            try
            {
                CloseBoundPeers();
            }
            catch
            {
            }
            //if (! _checkInterfacesOnLookup)
            {
                NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
            }
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            if (IsListening)
            {
                CheckInterfacesStatus();

                if (!_multicastResolved)
                    _multicastResolved = AddMulticastMembership(MulticastIp);
            }
           

        }

        

       

        


        
       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal InterfacePeer[] GetInterfacePeers()
        {
            InterfacePeer[] list;

            lock(_peersSync) {
                list = new InterfacePeer[_peers.Count];
                _peers.Values.CopyTo(list, 0);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="lookupInfo"></param>
        private void Identify(IPEndPoint ipEndpoint, LookupInfo lookupInfo)
        {
            var xmlResponse = ConceptResponse(lookupInfo, ipEndpoint);

            // FIND, IF THE REQUESTEE IS FROM OUR NETWORK

            var replyAsBroadcast = false;

            try
            {
                var found = false;
                foreach (var ifacePeer in _peers.Values)
                {
                    if (IPHelper.IsSameNetwork4(ifacePeer.ipAddress, ifacePeer.subnetMask, ipEndpoint.Address))
                    {
                        found = true;
                        break;
                    }

                }

                if (!found)
                    replyAsBroadcast = true;

                //IDictionary<UnicastIPAddressInformation, NetworkInterface> ipsAndIfaces = GetInterfaceAddresses(false, false);

                //if (ipsAndIfaces != null &&
                //    ipsAndIfaces.Count > 0)
                //{
                //    bool found = false;
                //    foreach (KeyValuePair<UnicastIPAddressInformation, NetworkInterface> pair in ipsAndIfaces)
                //    {

                //        if (IPHelper.IsSameNetwork4(pair.Key.Address, pair.Key.IPv4Mask, ipEndpoint.Address))
                //        {
                //            found = true;
                //            break;
                //        }
                //    }

                //    if (!found)
                //        replyAsBroadcast = true;
                //}
            }
            catch
            {
            }

            // IF THE REQUESTEE IS FROM NONE OF OUR NETWORKS , RESPOND VIA BROADCAST
            var ipe = replyAsBroadcast 
                ? new IPEndPoint(IPAddress.Broadcast, ipEndpoint.Port) 
                : new IPEndPoint(ipEndpoint.Address, _port);

            Send(ipe, new ByteDataCarrier(xmlResponse));
        }

        

        private static int _requestCounter = 0;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookupType"></param>
        /// <param name="value"></param>
        /// <param name="isBlocking"></param>
        /// <param name="getExtras"></param>
        /// <param name="lookupMode"></param>
        /// <param name="ignoreLocalResponse"></param>
        /// <param name="ipDown"></param>
        /// <param name="ipUp"></param>
        /// <param name="explicitDelegate"></param>
        /// <returns></returns>
        private LookupInfo BeginLookup(
            CNMPLookupType lookupType,
            [NotNull] string value, 
            bool isBlocking, 
            [CanBeNull] string[] getExtras, 
            CNMPLookupMode lookupMode, 
            bool ignoreLocalResponse,
            IPAddress ipDown, 
            IPAddress ipUp, 
            DCNMPLookupFinished explicitDelegate)
        {
            if (!IsListening)
                throw new InvalidOperationException(
                    "Lookup cannot be started, if the UDP socket was not bound.\n" +
                    "Call the \"Start\" method first");

            Validator.CheckNullString(value,"value");
            if (lookupType == CNMPLookupType.Unknown)
                throw new InvalidOperationException("Lookup type cannnot be unknown");

            var lookupInfo = new LookupInfo
            {
                LookupType = lookupType,
                Value = value,
                _lookupResult = new CNMPLookupResultList(this),
                _lookupMode = lookupMode,
                _unicastIPDown = ipDown,
                _unicastIPUp = ipUp,
                _explicitDelegate = explicitDelegate,
                _ignoreLocalResponse = ignoreLocalResponse
            };

            if (null != getExtras &&
                0 < getExtras.Length)
            {
                lookupInfo._requestExtras = new Dictionary<string, string>();
                foreach (var strKey in getExtras)
                {
                    if (string.IsNullOrEmpty(strKey))
                        continue;

                    lookupInfo._requestExtras[strKey] = String.Empty;
                }
            }

            if (isBlocking)
            {
                lookupInfo._waitMutex = new EventWaitHandle(false, EventResetMode.ManualReset);
            }

            //If CheckInterfacesOnLookup property is true check interfaces and their operational status before every lookup 
            if (_checkInterfacesOnLookup)
            {
                CheckInterfacesStatus();
            }            

            SendRequest(lookupInfo);

            return lookupInfo;
        }

        /// <summary>
        /// asynchronously begins lookup, 
        /// the end of asynchronous operation is notified by calling the LookupSucceeded or LookupTimeout event
        /// </summary>
        /// <param name="unicastIPAddress"></param>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <param name="getExtras">variable list of extra parameters to request in response</param>
        /// <exception cref="ArgumentNullException">if the unicastIPAddress is null</exception>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="ArgumentException">if the unicastIPAddress is not unicast</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        public void BeginLookup(
            [NotNull]
            IPAddress unicastIPAddress, 
            CNMPLookupType lookupType, 
            [NotNull]
            string value, 
            params string[] getExtras)
        {
            Validator.CheckForNull(unicastIPAddress, "unicastIPAddress");
            if (unicastIPAddress.Equals(IPAddress.Broadcast) ||
                unicastIPAddress.Equals(IPAddress.Any) ||
                unicastIPAddress.Equals(IPAddress.None) ||
                unicastIPAddress.Equals(IPAddress.IPv6Any) ||
                unicastIPAddress.Equals(IPAddress.IPv6None))
                throw new ArgumentException("Only unicast IP address is allowed", "unicastIPAddress");

            BeginLookup(lookupType, value, false, getExtras, CNMPLookupMode.Unicast, true, unicastIPAddress, null, null);
        }



       
        

        private void CheckInterfacesStatus()
        {
            var ifaces = NetworkInterface.GetAllNetworkInterfaces();

			// NECESSARY TO UPDATE networkInterface INSTANCES, AS THEY DON'T UPDATE AUTOMATICALLY
            var ifaceSet = new Dictionary<string, NetworkInterface>();
            
            foreach (var networkInterface in ifaces)
            {
                ifaceSet[networkInterface.Id] = networkInterface;

                if ((networkInterface.OperationalStatus == OperationalStatus.Up ||
                    networkInterface.OperationalStatus == OperationalStatus.Dormant) &&
                    
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Tunnel ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx)
                    )
                {
                    var uips = networkInterface.GetIPProperties().UnicastAddresses;
                    if (uips != null)
                    {
                        foreach (var uip in uips)
                        {
                            if (uip.Address.AddressFamily != AddressFamily.InterNetwork ||
                                uip.SuffixOrigin == SuffixOrigin.LinkLayerAddress ||
                                uip.SuffixOrigin == SuffixOrigin.Other)
                                continue;

                            InterfacePeer ifacePeer = null;
                            lock (_peersSync)
                            {
                                if (!_peers.ContainsKey(uip.Address))
                                {
                                    ifacePeer = new InterfacePeer();
                                    _peers.Add(uip.Address, ifacePeer);
                                }
                            }

                            if (ifacePeer != null)
                            {
                                try
                                {
                                    ifacePeer.udpPeer = new SimpleUdpPeer(uip.Address, 0);

                                    ifacePeer.udpPeer.DataReceived += OnDataReceivedPartial;
                                    ifacePeer.udpPeer.AllowBroadcast = true;

                                    ifacePeer.udpPeer.Start();

                                    ifacePeer.ipAddress = uip.Address;
                                    ifacePeer.subnetMask = uip.IPv4Mask;
                                    ifacePeer.networkInterface = networkInterface;

                                }
                                catch
                                {
                                    lock (_peers)
                                    {
                                        try
                                        {
                                            _peers.Remove(uip.Address);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }


                        }
                    }
                }
            }

            var disposedPeers = new LinkedList<InterfacePeer>();

            lock(_peersSync) 
            {
                foreach(var ifacePeer in _peers.Values) {
                    try
                    {
                        if (ifacePeer.networkInterface == null)
                        {
                            // THIS SHOULD NOT HAPPEN
                            disposedPeers.AddLast(ifacePeer);
                            continue;
                        }

                        NetworkInterface updatedNetworkInterface;
                        if (!ifaceSet.TryGetValue(ifacePeer.networkInterface.Id, out updatedNetworkInterface))
                        {
                            ifacePeer.networkInterface = null; // means the iface has been removed, e.g. unplugged USB ethernet
                            disposedPeers.AddLast(ifacePeer);
                            continue;
                        }
                        ifacePeer.networkInterface = updatedNetworkInterface;

                        if (ifacePeer.networkInterface.OperationalStatus != OperationalStatus.Up &&
                            ifacePeer.networkInterface.OperationalStatus != OperationalStatus.Dormant &&
                            ifacePeer.networkInterface.OperationalStatus != OperationalStatus.LowerLayerDown)
                        {
                            disposedPeers.AddLast(ifacePeer);
                        }
                    }
                    catch
                    {
                        disposedPeers.AddLast(ifacePeer);
                    }
                }   
            }

            // DO THIS ASAP TO REMOVE THE REGISTRATIONS FROM THE DICTIONARY _peers
            lock (_peersSync)
            {
                foreach (var ip in disposedPeers)
                {
                    
                        try
                        {
                            _peers.Remove(ip.ipAddress);
                        }
                        catch
                        {
                        }
                        
                }
            }

            foreach (var ip in disposedPeers)
            {
                try
                {
                    ip.Dispose();
                }
                catch
                {
                }
            }
            /*if (networkInterface.OperationalStatus != OperationalStatus.Up &&
                            networkInterface.OperationalStatus != OperationalStatus.Dormant &&
                            networkInterface.OperationalStatus != OperationalStatus.LowerLayerDown)
                        {
                            foreach (UnicastIPAddressInformation uip in uips)
                            {
                                if (uip.Address.AddressFamily != AddressFamily.InterNetwork)
                                    continue;

                                InterfacePeer ifacePeer = null;
                                lock (_peersSync)
                                {
                                    if (_peers.TryGetValue(uip.Address, out ifacePeer))
                                        _peers.Remove(uip.Address);
                                }

                                if (null != ifacePeer)
                                    try
                                    {
                                        ifacePeer.Dispose();
                                    }
                                    catch
                                    {
                                    }
                            }

                        }
                        else*/
        }

        void OnDataReceivedPartial(ISimpleUdpPeer udpPeer, IPEndPoint ipEndpoint, ByteDataCarrier data)
        {
            OnDataReceived(ipEndpoint, data);
        }

        private void RemoveBoundPeers(IEnumerable<IPAddress> peersToRemove)
        {
            if (null == peersToRemove)
                return;

            
            foreach (var ip in peersToRemove)
            {
                InterfacePeer ifacePeer;
                lock (_peers)
                {
                    if (_peers.TryGetValue(ip, out ifacePeer))
                    {
                        _peers.Remove(ip);
                    }
                }

                if (ifacePeer != null)
                    try
                    {
                        ifacePeer.Dispose();
                    }
                    catch
                    {
                    }
            }
        }

        private void SendRequest(
            [NotNull]
            LookupInfo lookupInfo)
        {
            if (!IsListening)
                throw new InvalidOperationException("Lookup cannot be started, if the UDP socket was not bound");

            lookupInfo._id = _requestCounter++;
            lookupInfo._timeStamp = DateTime.Now;

            ++lookupInfo._retryCount;


            var xmlRequestData = ConceptRequest(lookupInfo);

            //LookupInfo lookupInf = CopyLookupInfo(lookupInfo);          

            _requests[lookupInfo._id] = lookupInfo;

            IPAddress destination;

            switch (lookupInfo._lookupMode)
            {
                case CNMPLookupMode.Multicast:
                    destination = MulticastIp;
                    break;
                case CNMPLookupMode.Unicast:
                    destination = lookupInfo._unicastIPDown;
                    break;
                //case CNMPLookupMode.Broadcast:
                default:
                    destination = IPAddress.Broadcast;
                    break;
            }


            try
            {
                if (Equals(destination, IPAddress.Broadcast))
                {
                    var bcastIPe = new IPEndPoint(IPAddress.Broadcast, _port);

                    var peersToRemove = new LinkedList<IPAddress>();
                    foreach (var ifacePeer in _peers.Values)
                    {
                        try
                        {
                            ifacePeer.udpPeer.Send(bcastIPe, new ByteDataCarrier(xmlRequestData));
#if DEBUG_OUTPUTS
                            Console.WriteLine("CNMP: Sending to " + IPAddress.Broadcast + ":" + _port);
#endif

                            // ANTI-BROADCAST STORM CONDITION
                            //Thread.Sleep(r.Next(51)+50);
                        }
                        catch
                        {
                            try
                            {
                                peersToRemove.AddLast(ifacePeer.ipAddress);
#if DEBUG_OUTPUTS
                                Console.WriteLine("Removed socket bounded to interface: " + item.Key);
#endif
                                // ANTI-BROADCAST STORM CONDITION
                                //Thread.Sleep(r.Next(51) + 50);
                            }
                            catch //(Exception ex)
                            {
#if DEBUG_OUTPUTS
                                Console.WriteLine("Exception occured on interface: " + item.Key);
#endif
                            }
                        }
                    }
                    if (peersToRemove.Count > 0)
                    {
                        RemoveBoundPeers(peersToRemove);
                    }
                    //peersToRemove = null;
                    //                    IDictionary<UnicastIPAddressInformation, NetworkInterface> ipsAndIfaces = GetInterfaceAddresses(true, false);
                    //                    foreach (KeyValuePair<UnicastIPAddressInformation, NetworkInterface> pair in ipsAndIfaces)
                    //                    {
                    //                        if (pair.Value.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    //                        {

                    //                            IPAddress bcast = IPHelper.BroadcastFromIP4(pair.Key.Address, pair.Key.IPv4Mask);
                    //#if DEBUG_OUTPUTS
                    //                            Console.WriteLine("CNMP: Sending to " + bcast + ":" + _port);
                    //#endif
                    //                            Send(new IPEndPoint(bcast, _port), new ByteDataCarrier(xmlRequestData));
                    //                        }
                    //                        else
                    //                        {
                    //                            if (!lookupInfo._ignoreLocalResponse)
                    //                            {
                    //#if DEBUG_OUTPUTS
                    //                                Console.WriteLine("CNMP: Sending to " + IPAddress.Loopback + ":" + _port);
                    //#endif
                    //                                Send(new IPEndPoint(IPAddress.Loopback, _port), new ByteDataCarrier(xmlRequestData));
                    //                            }
                    //                        }
                    //                    }

                    //                    //SEND BROADCAST FOR LOGICALY SEPARATED AGENTS THAT ARE ON THE SAME PHYSICAL NETWORK AS REQUESTEE
                    //#if DEBUG_OUTPUTS
                    //                    Console.WriteLine("CNMP: Sending to " + IPAddress.Broadcast + ":" + _port);
                    //#endif
                    //                    Send(new IPEndPoint(IPAddress.Broadcast, _port), new ByteDataCarrier(xmlRequestData));
                }
                else
                {
#if DEBUG_OUTPUTS
                    Console.WriteLine("CNMP: Sending to " + destination + ":" + _port);
#endif
                    Send(new IPEndPoint(destination, _port), new ByteDataCarrier(xmlRequestData));
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                throw; // error;
            }

            //if _retryCount eqals 0, only one send at 0s is made, else one send at 0s an other retries at _timeout / (_retryCount + 1))s
            //example _timeout = 1000 _retryCount = 2  0s, 0.333s, 0.666s time intervas sends are made            
            //_timers.StartTimeout(((_retryCount < 1) ? _timeout : _timeout / (_retryCount + 1)), lookupInfo, new DOnTimerEvent(OnRequestTimeout));
            lock (_timerNewChangeLock)
            {
                _timerNewStateBridgeInstance._lookupInfo = lookupInfo;

                if (_timerNew != null)
                {
                    _timerNew.Change(
                        ((_retryCount < 1) ? _timeout : _timeout / (_retryCount + 1)),
                        System.Threading.Timeout.Infinite);


                        //_timerNew.Dispose();
                }
                else
                {
                    _timerNew = new Timer(
                        OnRequestTimeout,
                        _timerNewStateBridgeInstance,
                        ((_retryCount < 1) ? _timeout : _timeout/(_retryCount + 1)),
                        System.Threading.Timeout.Infinite);
                }
            }
        }

        private class TimerNewStateBridge
        {
            internal LookupInfo _lookupInfo;
        }

        /// <summary>
        /// used as bridge to changing reference of the LookupInfo
        /// </summary>
        private volatile TimerNewStateBridge _timerNewStateBridgeInstance = new TimerNewStateBridge();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        private void OnRequestTimeout(object info)
        {
            try
            {
                if (!Monitor.TryEnter(_timerNewChangeLock))
                    // something is just changing the timer
                    return;

                LookupInfo lookupInfo;
                try
                {
                    var stateBridge = info as TimerNewStateBridge;
                    if (null == stateBridge)
                    {
                        DebugHelper.TryBreak("CNMPAgent.OnRequestTimeout wrong state bridge instance", info);
                        return;
                    }


                    lookupInfo = stateBridge._lookupInfo;
                    if (null == lookupInfo)
                    {
                        DebugHelper.TryBreak("CNMPAgent.OnRequestTimeout lookupInfo is null", info);
                        return;
                    }
                }
                finally
                {
                    Monitor.Exit(_timerNewChangeLock);
                }

                // do not stop searching after first successfull find
                //if (_requests.ContainsKey(aInfo._id))
                //{
                // do not condition this ... actual timer can vary
                // if the timer has reached this state, it means it's timeout
                //IsTimedOut(aInfo);

                //if (_requests.ContainsKey(lookupInfo._id))//*
                //    _requests.Remove(lookupInfo._id);//*

                if (lookupInfo._retryCount < _retryCount)
                {
#if DEBUG_OUTPUTS
                    Console.WriteLine("CNMP: retrying ... " + (lookupInfo._retryCount + 1));
#endif
                    SendRequest(lookupInfo);
                }
                else
                {
                    //if (_requests.ContainsKey(lookupInfo._id))//*
                    //    _requests.Remove(lookupInfo._id);//*

                    if (lookupInfo._lookupResult.Count == 0)
                        FireTimedOut(lookupInfo);
                    else
                        FireIdentified(lookupInfo);

                    if (null != lookupInfo._waitMutex)
                        lookupInfo._waitMutex.Set();
                    _requests.Clear();
                }
                //}

            }
            catch(Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
        }

   

        

        protected override void InternalDispose(bool isExplicitDispose)
        {
            base.InternalDispose(isExplicitDispose);
            try
            {
                CloseBoundPeers();
            }
            catch { }
            if (IsListening && _multicastResolved)
                try { RemoveMulticastMembership(MulticastIp); }
                catch { }
        }

        public string MachineName { get { return Environment.MachineName; } }

    }
}

