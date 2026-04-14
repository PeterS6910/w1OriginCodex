using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;


namespace Contal.IwQuick.Sys.Microsoft
{
    /// <summary>
    /// 
    /// </summary>
    public class DllIpHlpApi
    {




        //
        // Definitions and structures used by getnetworkparams and getadaptersinfo apis
        //
        internal const int MAX_ADAPTER_DESCRIPTION_LENGTH = 128;
        internal const int MAX_ADAPTER_NAME_LENGTH = 256;
        internal const int MAX_ADAPTER_ADDRESS_LENGTH = 8;
        internal const int DEFAULT_MINIMUM_ENTITIES = 32;
        internal const int MAX_HOSTNAME_LEN = 128;
        internal const int MAX_DOMAIN_NAME_LEN = 128;
        internal const int MAX_SCOPE_ID_LEN = 256;
        //
        // Node Type
        //
        internal const int BROADCAST_NODETYPE = 1;
        internal const int PEER_TO_PEER_NODETYPE = 2;
        internal const int MIXED_NODETYPE = 4;
        internal const int HYBRID_NODETYPE = 8;
        //
        // Adapter Type
        //
        internal const int IF_OTHER_ADAPTERTYPE = 0;
        internal const int IF_ETHERNET_ADAPTERTYPE = 1;
        internal const int IF_TOKEN_RING_ADAPTERTYPE = 2;
        internal const int IF_FDDI_ADAPTERTYPE = 3;
        internal const int IF_PPP_ADAPTERTYPE = 4;
        internal const int IF_LOOPBACK_ADAPTERTYPE = 5;
        internal const int IF_SLIP_ADAPTERTYPE = 6;

        internal const int MAX_INTERFACE_NAME_LEN = 256;
        internal const int MAXLEN_IFDESCR = 256;
        internal const int MAXLEN_PHYSADDR = 8;

        /// <summary>
        /// 
        /// </summary>
        public const int IF_OPER_STATUS_NON_OPERATIONAL = 0;

        /// <summary>
        /// 
        /// </summary>
        public const int IF_OPER_STATUS_UNREACHABLE = 1;
        /// <summary>
        /// 
        /// </summary>
        public const int IF_OPER_STATUS_DISCONNECTED = 2;
        /// <summary>
        /// 
        /// </summary>
        public const int IF_OPER_STATUS_CONNECTING = 3;
        /// <summary>
        /// 
        /// </summary>
        public const int IF_OPER_STATUS_CONNECTED = 4;
        /// <summary>
        /// 
        /// </summary>
        public const int IF_OPER_STATUS_OPERATIONAL = 5;

        /// <summary>
        ///   IP_ADDRESS_STRING - store an IP address as a dotted decimal string
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct IP_ADDRESS_STRING
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            internal string address;
        };

        /// <summary>
        ///   IP_MASK_STRING - store an IP address as a dotted decimal string
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct IP_MASK_STRING
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            internal string address;
        };

        /// <summary>
        ///   IP_ADDR_STRING - store an IP address with its corresponding subnet mask,
        ///   both as dotted decimal strings
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct IP_ADDR_STRING
        {
            internal int Next;      /* struct _IP_ADDR_STRING* */
            internal IP_ADDRESS_STRING IpAddress;
            internal IP_MASK_STRING IpMask;
            internal uint Context;

            internal IPAddress[] ToIPAddressArray()
            {
                //
                // Deal with the DNS server list.
                //
                IP_ADDR_STRING addr = this;
                ArrayList addresslist = new ArrayList();
                IPAddress[] addresses;

                addresslist.Add(IPAddress.Parse(addr.IpAddress.address));

// ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                while (addr.Next != 0)
                {
                    addr = (IP_ADDR_STRING)Marshal.PtrToStructure((IntPtr)addr.Next, typeof(IP_ADDR_STRING));

                    addresslist.Add(IPAddress.Parse(addr.IpAddress.address));
                }

                addresses = new IPAddress[addresslist.Count];

                for (int i = 0; i < addresslist.Count; ++i)
                {
                    addresses[i] = (IPAddress)addresslist[i];
                }

                return addresses;
            }
        };

        /// <summary>
        ///   ADAPTER_INFO - per-adapter information. All IP addresses are stored as
        ///   strings
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct IP_ADAPTER_INFO
        {
            internal int /* struct _IP_ADAPTER_INFO* */ Next;
            internal uint ComboIndex;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_NAME_LENGTH + 4)]
            internal String AdapterName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_DESCRIPTION_LENGTH + 4)]
            internal String Description;
            internal uint AddressLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ADAPTER_ADDRESS_LENGTH)]
            internal byte[] Address;
            internal uint Index;
            internal uint Type;
            internal uint DhcpEnabled;
            internal uint CurrentIpAddress; /* IP_ADDR_STRING* */
            internal IP_ADDR_STRING IpAddressList;
            internal IP_ADDR_STRING GatewayList;
            internal IP_ADDR_STRING DhcpServer;
            [MarshalAs(UnmanagedType.Bool)]
            internal bool HaveWins;
            internal IP_ADDR_STRING PrimaryWinsServer;
            internal IP_ADDR_STRING SecondaryWinsServer;
            internal uint/*time_t*/ LeaseObtained;
            internal uint/*time_t*/ LeaseExpires;
        };

        /// <summary>
        ///   IP_PER_ADAPTER_INFO - per-adapter IP information such as DNS server list.
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct IP_PER_ADAPTER_INFO
        {
            internal uint AutoconfigEnabled;
            internal uint AutoconfigActive;
            internal uint CurrentDnsServer; /* IP_ADDR_STRING* */
            internal IP_ADDR_STRING DnsServerList;
        };

        /// <summary>
        ///   Core network information.
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct FIXED_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_HOSTNAME_LEN + 4)]
            internal String HostName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_DOMAIN_NAME_LEN + 4)]
            internal String DomainName;
            internal uint CurrentDnsServer; /* IP_ADDR_STRING* */
            internal IP_ADDR_STRING DnsServerList;
            internal uint NodeType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SCOPE_ID_LEN + 4)]
            internal String ScopeId;
            internal uint EnableRouting;
            internal uint EnableProxy;
            internal uint EnableDns;
        };

        /// <summary>
        ///   Adapter map.
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct IP_ADAPTER_INDEX_MAP
        {
            internal uint Index;                                       // adapter index 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ADAPTER_NAME_LENGTH)]
            internal string Name;                                        // name of the adapter 
        }

        /// <summary>
        ///   Basic Interface information.
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct IP_INTERFACE_INFO
        {
            internal int NumAdapters;                   // number of adapters in array 
            internal IP_ADAPTER_INDEX_MAP Adapter;                       // adapter indices and names 
        }

        /// <summary>
        ///   Network Interface information.
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MIB_IFROW
        {
            /*
            [MarshalAs(UnmanagedType.ByValTStr,SizeConst=iphlpapi.MAX_INTERFACE_NAME_LEN)]
            internal string wszName           = null;
            internal uint   dwIndex           = 0;                                 // index of the interface
            internal uint   dwType            = 0;                                 // type of interface
            internal uint   dwMtu             = 0;                                 // max transmission unit 
            internal uint   dwSpeed           = 0;                                 // speed of the interface 
            internal uint   dwPhysAddrLen     = iphlpapi.MAXLEN_PHYSADDR;          // length of physical address
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=iphlpapi.MAXLEN_PHYSADDR)]
            internal byte[] bPhysAddr         = new byte[iphlpapi.MAXLEN_PHYSADDR];// physical address of adapter
            internal uint   dwAdminStatus     = 0;                                 // administrative status
            internal uint   dwOperStatus      = 0;                                 // operational status
            internal uint   dwLastChange      = 0;                                 // last time operational status changed 
            internal uint   dwInOctets        = 0;                                 // octets received
            internal uint   dwInUcastPkts     = 0;                                 // unicast packets received 
            internal uint   dwInNUcastPkts    = 0;                                 // non-unicast packets received 
            internal uint   dwInDiscards      = 0;                                 // received packets discarded 
            internal uint   dwInErrors        = 0;                                 // erroneous packets received 
            internal uint   dwInUnknownProtos = 0;                                 // unknown protocol packets received 
            internal uint   dwOutOctets       = 0;                                 // octets sent 
            internal uint   dwOutUcastPkts    = 0;                                 // unicast packets sent 
            internal uint   dwOutNUcastPkts   = 0;                                 // non-unicast packets sent 
            internal uint   dwOutDiscards     = 0;                                 // outgoing packets discarded 
            internal uint   dwOutErrors       = 0;                                 // erroneous packets sent 
            internal uint   dwOutQLen         = 0;                                 // output queue length 
            internal uint   dwDescrLen        = iphlpapi.MAXLEN_IFDESCR;           // length of bDescr member 
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=iphlpapi.MAXLEN_IFDESCR)]
            internal byte[] bDescr            = new byte[iphlpapi.MAXLEN_IFDESCR]; // interface description 
            */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_INTERFACE_NAME_LEN)]
            internal string wszName;
            internal uint dwIndex;
            internal uint dwType;
            internal uint dwMtu;
            internal uint dwSpeed;
            internal uint dwPhysAddrLen;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXLEN_PHYSADDR)]
            internal byte[] bPhysAddr;
            internal uint dwAdminStatus;
            internal uint dwOperStatus;
            internal uint dwLastChange;
            internal uint dwInOctets;
            internal uint dwInUcastPkts;
            internal uint dwInNUcastPkts;
            internal uint dwInDiscards;
            internal uint dwInErrors;
            internal uint dwInUnknownProtos;
            internal uint dwOutOctets;
            internal uint dwOutUcastPkts;
            internal uint dwOutNUcastPkts;
            internal uint dwOutDiscards;
            internal uint dwOutErrors;
            internal uint dwOutQLen;
            internal uint dwDescrLen;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXLEN_IFDESCR)]
            internal byte[] bDescr;
        }

        /// <summary>
        ///   IP Address row.
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MIB_IPADDRROW
        {
            internal uint dwAddr;                                      // IP address
            internal uint dwIndex;                                     // interface index
            internal uint dwMask;                                      // subnet mask
            internal uint dwBCastAddr;                                 // broadcast address 
            internal uint dwReasmSize;                                 // reassembly size 
            internal ushort unused1;                                     // not currently used 
            internal ushort unused2;                                     // not currently used 
        }

        /// <summary>
        ///   IP Address table.
        /// </summary>
        [ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MIB_IPADDRTABLE
        {
            internal uint dwNumEntries;                         // number of entries in the table
            internal MIB_IPADDRROW table;                                // array of IP address entries
        }

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static uint GetAdaptersInfo(IntPtr pAdapterInfo, ref int pOutBufLen);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static uint GetBestInterface(uint dwDestAddr, ref uint pdwBestIfIndex);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static uint GetIfEntry(ref MIB_IFROW pIfRow);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static uint GetInterfaceInfo(IntPtr pIfTable, ref int dwOutBufLen);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static uint GetIpAddrTable(IntPtr pIpAddrTable, ref int pdwSize, [MarshalAs(UnmanagedType.Bool)]bool bOrder);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static uint GetNetworkParams(IntPtr pFixedInfo, ref int pOutBufLen);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static uint GetNumberOfInterfaces(ref int pdwNumIf);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static uint GetPerAdapterInfo(uint IfIndex, IntPtr pPerAdapterInfo, ref int pOutBufLen);

        [DllImport("iphlpapi", CharSet = CharSet.Auto)]
        internal extern static bool GetRTTAndHopCount(uint DestIpAddress,  // destination IP address
                                                      ref uint HopCount,       // returned hop count
                                                      uint MaxHops,        // limit on number of hops to search
                                                      ref uint RTT);           // round-trip time

        /*[DllImport("iphlpapi", CharSet=CharSet.Auto)]
        internal extern static uint NotifyAddrChange(out uint waithandle,ref NativeOverlapped overlapped);
        [DllImport("iphlpapi", CharSet=CharSet.Auto)]
        internal extern static uint NotifyAddrChange(uint nullhandle,uint nulloverlapped);*/



    }
}
