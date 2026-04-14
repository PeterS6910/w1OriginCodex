using System;

using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using Contal.IwQuick;
using Contal.IwQuick.Sys.Microsoft;

namespace Contal.IwQuick.Net.Microsoft
{
    public class NetworkInterface
    {
        // listing functions
        private static uint ERROR_INSUFFICIENT_BUFFER = (uint)122;
        private static Dictionary<string,NetworkInterface> _interfaces = null;

        public static NetworkInterface GetInterface(string name)
        {
            if (Validator.IsNullString(name))
                return null;

             LoadInterfaceInfo();
            
            NetworkInterface ret = null;
            _interfaces.TryGetValue(name,out ret);
            return ret;
        }


        private uint _index;
        private string _name;

        private DllIpHlpApi.MIB_IFROW _ifMibRow = new DllIpHlpApi.MIB_IFROW();

        /// <summary>
        ///   Count of interfaces
        /// </summary>
        public static int InterfaceCount
        {
            get
            {
                int count = 0;
                uint result = DllIpHlpApi.GetNumberOfInterfaces(ref count);

                if (result == 0)
                    return count;
                else
                    return -1;
            }
        }

        private static NetworkInterface[] _ifArray = null;
        /// <summary>
        ///   Collection of Interfaces
        /// </summary>
        public static NetworkInterface[] Interfaces
        {
            get
            {
                if (_interfaces == null)
                    LoadInterfaceInfo();

                if (null == _ifArray)
                {
                    _ifArray = new NetworkInterface[_interfaces.Count];
                    _interfaces.Values.CopyTo(_ifArray, 0);
                }

                return _ifArray;
            }
        }

        /*/// <summary>
        ///   Retrieves an interface by index number
        /// </summary>
        public static NetworkInterface Interface(uint index)
        {
            if ( interfaces == null )
                LoadInterfaceInfo();

            return (NetworkInterface)interfaces[index];
        }*/

        /// <summary>
        ///   Loads the Interface data
        /// </summary>
        private static void LoadInterfaceInfo()
        {
            if (_interfaces == null)
            {
                _interfaces = new Dictionary<string, NetworkInterface>(InterfaceCount);

                if (InterfaceCount != 0)
                {
                    //
                    // Try baseline buffer size for a machine with a single
                    // network interface and allocate the buffer from native
                    // heap.
                    //
                    int size = Marshal.SizeOf(typeof(int)) +
                                        (Marshal.SizeOf(typeof(DllIpHlpApi.IP_INTERFACE_INFO)) * InterfaceCount);
                    IntPtr buffer = Marshal.AllocHGlobal(size);
                    //
                    // Attempt to get the IP_ADAPTER_INFO for the machine
                    //
                    uint result = DllIpHlpApi.GetInterfaceInfo(buffer, ref size);

                    if (result == ERROR_INSUFFICIENT_BUFFER)
                    {
                        //
                        // Buffer overflow means that we need more space for
                        // the IP_ADAPTER_INFO data. GetAdaptersInfo should have
                        // returned the actual size needed for the buffer. So
                        // free the current buffer and allocate a new one then
                        // try the call again.
                        //
                        Marshal.FreeHGlobal(buffer);

                        buffer = Marshal.AllocHGlobal(size);

                        result = DllIpHlpApi.GetInterfaceInfo(buffer, ref size);
                    }

                    if (result == 0)
                    {
                        //
                        // OK, we're good to go. We now have data in the buffer
                        // indicating the number of interfaces and an array of
                        // IP_ADAPTER_INDEX_MAP structures. We need to peel
                        // each of the IP_ADAPTER_INDEX_MAP structures out of the
                        // buffer and create NetworkInterface objects to represent
                        // them.
                        //
                        int count = Marshal.ReadInt32(buffer);
                        int pos = buffer.ToInt32() + 4;

                        for (int i = 0; i < count; ++i, pos += Marshal.SizeOf(typeof(DllIpHlpApi.IP_ADAPTER_INDEX_MAP)))
                        {
                            DllIpHlpApi.IP_ADAPTER_INDEX_MAP info = (DllIpHlpApi.IP_ADAPTER_INDEX_MAP)Marshal.PtrToStructure((IntPtr)pos, typeof(DllIpHlpApi.IP_ADAPTER_INDEX_MAP));

                            _interfaces.Add(info.Name, new NetworkInterface(info));
                        }
                        //
                        // Release the native heap buffer
                        //
                        Marshal.FreeHGlobal(buffer);
                    }
                    else
                    {
                        //
                        // Release the native heap buffer
                        //
                        Marshal.FreeHGlobal(buffer);
                        //
                        // Throw an exception since we failed
                        //
                        throw new InvalidOperationException("Call to GetInterfaceInfo failed. Error was " + result);
                    }
                }
            }
        }

        /// <summary>
        ///   Constructor
        /// </summary>
        internal NetworkInterface(DllIpHlpApi.IP_ADAPTER_INDEX_MAP info)
        {
            

            _index = info.Index;
            _name = info.Name;

            RefreshMIB();
        }

        private void RefreshMIB()
        {
            uint result = 0;

            //_ifMibRow = new DllIpHlpApi.MIB_IFROW();
            _ifMibRow.dwIndex = _index;

            if ((result = DllIpHlpApi.GetIfEntry(ref _ifMibRow)) != 0)
            {
                throw new InvalidOperationException("Call to GetIfEntry failed: " + result);
            }
        }


        /// <summary>
        ///   Admin statis
        /// </summary>
        public uint AdminStatus
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwAdminStatus;
            }
        }

        /// <summary>
        ///   Description
        /// </summary>
        public string Description
        {
            get
            {
                return Encoding.ASCII.GetString(_ifMibRow.bDescr, 0, (int)_ifMibRow.dwDescrLen);
            }
        }

        /// <summary>
        ///   Interface index
        /// </summary>
        public uint Index
        {
            get
            {
                return _index;
            }
        }

        /// <summary>
        ///   MTU size
        /// </summary>
        public uint Mtu
        {
            get
            {
                return _ifMibRow.dwMtu;
            }
        }

        /// <summary>
        ///   Name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        ///   Number of non-unicast packets received
        /// </summary>
        public uint NonUnicastReceived
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwInNUcastPkts;
            }
        }

        /// <summary>
        ///   Number of non-unicast packets sent
        /// </summary>
        public uint NonUnicastSent
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwOutNUcastPkts;
            }
        }

        /// <summary>
        ///   Physical address
        /// </summary>
        public string PhysicalAddress
        {
            get
            {
                return Encoding.ASCII.GetString(_ifMibRow.bPhysAddr, 0, (int)_ifMibRow.dwPhysAddrLen);
            }
        }

        /// <summary>
        ///   Number of packets received
        /// </summary>
        public uint Received
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwInOctets;
            }
        }

        /// <summary>
        ///   Number of received packets that have been discarded
        /// </summary>
        public uint ReceivedDiscarded
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwInDiscards;
            }
        }

        /// <summary>
        ///   Number of received packets that were in error
        /// </summary>
        public uint ReceivedError
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwInErrors;
            }
        }

        /// <summary>
        ///   Number of packets sent
        /// </summary>
        public uint Sent
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwOutOctets;
            }
        }

        /// <summary>
        ///   Number of sent packets that were discarded
        /// </summary>
        public uint SentDiscarded
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwOutDiscards;
            }
        }

        /// <summary>
        ///   Number of sent packets that were in error
        /// </summary>
        public uint SentError
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwOutErrors;
            }
        }

        /// <summary>
        ///   Interface speed
        /// </summary>
        public uint Speed
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwSpeed;
            }
        }

        public uint Type
        {
            get
            {
                // no need to refresh here
                return _ifMibRow.dwType;
            }
        }

        /// <summary>
        ///   Number of unicast packets received
        /// </summary>
        public uint UnicastReceived
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwInUcastPkts;
            }
        }

        /// <summary>
        ///   Number of unicast packets sent
        /// </summary>
        public uint UnicastSent
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwOutUcastPkts;
            }
        }

        public uint OperationStatus
        {
            get
            {
                RefreshMIB();

                return _ifMibRow.dwOperStatus;
            }
        }

        public bool IsOperational
        {
            get
            {
                RefreshMIB();

                return //_ifMibRow.dwOperStatus == DllIpHlpApi.IF_OPER_STATUS_CONNECTED ||
                    _ifMibRow.dwOperStatus == DllIpHlpApi.IF_OPER_STATUS_OPERATIONAL;
            }
        }

    }
}
