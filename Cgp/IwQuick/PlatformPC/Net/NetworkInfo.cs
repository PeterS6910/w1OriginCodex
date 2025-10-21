using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace Contal.IwQuick.Net
{
    public class NetworkInfo : ANetworkInfo
    {
        private Dictionary<NetworkInterface, IPInterfaceProperties> _ipList;

        private LinkedList<Ipv4WithMask> _ipv4Cache;

        private readonly object _syncRefreshLocalIpInfo = new object();

        /// <summary>
        /// 
        /// </summary>
        public static readonly NetworkInfo Singleton = new NetworkInfo();

        private readonly object _syncWithReads = new object();

        private NetworkInfo()
        {
            RefreshLocalIpInfo();

            NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
        }

        private void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            RefreshLocalIpInfo();
        }

        private void OnNetworkAddressChanged(object sender, System.EventArgs e)
        {
            RefreshLocalIpInfo();
        }

        private void RefreshLocalIpInfo()
        {
            // will try to exclude parallel/semiparallel calls of this refresh
            if (!Monitor.TryEnter(_syncRefreshLocalIpInfo))
                return;

            try
            {
                lock (_syncWithReads)
                {
                    if (_ipList == null)
                        _ipList = new Dictionary<NetworkInterface, IPInterfaceProperties>();
                    else
                        _ipList.Clear();

                    if (_ipv4Cache == null)
                        _ipv4Cache = new LinkedList<Ipv4WithMask>();
                    else
                    {
                        foreach (var ipv4WithMask in _ipv4Cache)
                        {
                            ipv4WithMask.Return();
                        }
                        _ipv4Cache.Clear();
                    }



                    var ifaces = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (var iface in ifaces)
                    {
                        var ipp = iface.GetIPProperties();
                        ipp.GetIPv4Properties();

                        _ipList[iface] = ipp;

                        foreach (var unicastIp in ipp.UnicastAddresses)
                        {
                            if (unicastIp.Address.AddressFamily != AddressFamily.InterNetwork)
                                continue;

                            if (unicastIp.IPv4Mask == null)
                                continue;

                            var ipv4WithMask = Ipv4WithMask.Get();
                            ipv4WithMask.IP = unicastIp.Address;
                            ipv4WithMask.Mask = unicastIp.IPv4Mask;

                            _ipv4Cache.AddLast(ipv4WithMask);
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(_syncRefreshLocalIpInfo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public override bool IsLocalIp(IPAddress ip)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ip == null)
// ReSharper disable HeuristicUnreachableCode
                return false;
// ReSharper restore HeuristicUnreachableCode

            lock (_syncWithReads)
            {
                foreach (var ifaceInfo in _ipList.Values)
                {
                    var unicastIps = ifaceInfo.UnicastAddresses;

                    foreach (var unicastIp in unicastIps)
                    {
                        if (unicastIp.Address.Equals(ip))
                            return true;
                    }


                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<IPAddress> LocalIpAddresses
        {
            get
            {
                lock (_syncWithReads)
                {
                    LinkedList<IPAddress> result = new LinkedList<IPAddress>();

                    foreach (var ifaceInfo in _ipList.Values)
                    {
                        var unicastIps = ifaceInfo.UnicastAddresses;

                        foreach (var unicastIp in unicastIps)
                        {
                            result.AddLast(unicastIp.Address);
                        }


                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<string> GetIpAddressReport()
        {


            lock (_syncWithReads)
            {
                LinkedList<string> result = new LinkedList<string>();

                foreach (var ifaceInfo in _ipList.Values)
                {
                    var unicastIps = ifaceInfo.UnicastAddresses;

                    foreach (var unicastIp in unicastIps)
                    {
                        if (unicastIp.Address.AddressFamily == AddressFamily.InterNetwork)
                            result.AddLast(unicastIp.Address + StringConstants.SLASH + unicastIp.IPv4Mask);
                        else
                        {
                            result.AddLast(unicastIp.Address.ToString());
                        }

                    }


                }

                var arrayResult = result.ToArray();
                result.Clear();

                return arrayResult;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public override bool IsLocalNetwork4(IPAddress ip)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(ip, null))
// ReSharper disable HeuristicUnreachableCode
                return false;
// ReSharper restore HeuristicUnreachableCode

            if (ip.Equals(IPAddress.Loopback))
                return true;

            lock (_syncWithReads)
            {
                foreach (var ifaceIpv4s in _ipv4Cache)
                {
                    if (IPHelper.IsSameNetwork4(ifaceIpv4s.IP, ifaceIpv4s.Mask, ip))
                        return true;
                }
            }

            return false;
        }

        public override string MachineName
        {
            get { return Environment.MachineName; }
        }

        /*public bool IsLocalNetwork([NotNull] IPAddress ip)
        {
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(ip, null))
// ReSharper disable HeuristicUnreachableCode
                return false;
// ReSharper restore HeuristicUnreachableCode

            lock (_syncWithReads)
            {
                foreach (var ifaceInfo in _ipList.Values)
                {
                    var unicastIps = ifaceInfo.UnicastAddresses;

                    foreach (var unicastIp in unicastIps)
                    {
                        var localIp = unicastIp.Address;

                        if (localIp.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (IPHelper.IsSameNetwork4(unicastIp.Address, unicastIp.IPv4Mask, ip))
                                return true;
                        }
                        else
                        {
                            //TODO : no direct access to IPv6 prefix   
                        }
                    }
                }
            }

            return false;
        }*/
    }

    public static class NetworkInfoFactoryExtension
    {
        public static INetworkInfo Obtain(this NetworkInfoFactory factory)
        {
            return NetworkInfo.Singleton;
        }
    }
}
