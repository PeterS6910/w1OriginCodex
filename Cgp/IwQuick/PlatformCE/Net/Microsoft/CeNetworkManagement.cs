using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys.Microsoft;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace Contal.IwQuick.Net.Microsoft
{
    /// <summary>
    /// 
    /// </summary>
    public class CeNetworkManagement
    {
        private static readonly SyncDictionary<string,CeNetworkManagement> _ifaces = new SyncDictionary<string, CeNetworkManagement>(2);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="ndisPort"></param>
        /// <returns></returns>
        public static CeNetworkManagement Obtain(
            [NotNull] string deviceName,
            [NotNull] string ndisPort)
        {
            CeNetworkManagement ceNetworkManagement;

            _ifaces.GetOrAddValue(
                deviceName,
                out ceNetworkManagement,
                key => new CeNetworkManagement(deviceName,ndisPort),
                null);

            return ceNetworkManagement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="ndisPort"></param>
        private CeNetworkManagement (
            [NotNull] string deviceName,
            [NotNull] string ndisPort)
        {

            Validator.CheckNullString(deviceName);
            Validator.CheckNullString(ndisPort);
            

            _deviceName = deviceName;

            if (ndisPort.Length == 1 &&
                ndisPort[0] <= '9' && ndisPort[0] >= '0')
                _ndisPort = "NDS" + ndisPort + ":";
            else
                _ndisPort = ndisPort;

            // to validate deviceName/ndisPort combination by opening the registry
            var rk = OpenRegistryKey(false);
            try {rk.Close();}catch{}
        }

        private RegistryKey OpenRegistryKey(bool forWritting)
        {
            RegistryKey _rootRegistry;

            // validates if the registry is able on defined branch
            try
            {
                _rootRegistry =
                    Registry.LocalMachine.OpenSubKey(@"Comm\" + _deviceName + ParametersSubPath, forWritting);
            }
            catch (Exception e)
            {
                Sys.HandledExceptionAdapter.Examine(e);

                _rootRegistry = null;
                e.TryBreakAndThrow();
            }
            return _rootRegistry;
        }

        /// <summary>
        /// 
        /// </summary>
        public const string ZeroMac = "00:00:00:00:00:00";
        /// <summary>
        /// 
        /// </summary>
        public const string ZeroIp = "0.0.0.0";

        /// <summary>
        /// 
        /// </summary>
        public const string ZeroIp6 = "::";

        private string _fallbackIp = "192.168.1.254";
        /// <summary>
        /// 
        /// </summary>
        public string FallbackIp
        {
            get { return _fallbackIp; }
            set
            {
                IPHelper.CheckValidity4(value);

                _fallbackIp = value;

                RefreshFallbackSubnet();
            }
        }

        private string _fallbackMask = "255.255.255.0";
        /// <summary>
        /// 
        /// </summary>
        public string FallbackMask
        {
            get { return _fallbackMask; }
            set
            {
                IPHelper.CheckMaskValidity4(value);

                _fallbackMask = value;

                RefreshFallbackSubnet();
            }
        }

        private string _fallbackGateway = "192.168.1.1";
        /// <summary>
        /// 
        /// </summary>
        public string FallbackGateway
        {
            get { return _fallbackGateway; }
            set
            {
                IPHelper.CheckValidity4(value);

                _fallbackGateway = value;
            }
        }

        private void RefreshFallbackSubnet()
        {
            string ret = IPHelper.SubnetFromIP4(_fallbackIp, _fallbackMask);

            if (ret != null)
                _fallbackSubnet = ret;
        }

        private string _fallbackSubnet = "192.168.1.0";
        /// <summary>
        /// 
        /// </summary>
        public string FallbackSubnet
        {
            // TODO : not in sync with Fallback IP
            get { return _fallbackSubnet; }
        }

        private int _fallbackInterval = 90;
        /// <summary>
        /// 
        /// </summary>
        public int FallbackInterval
        {
            get { return _fallbackInterval; }
            set
            {
                Validator.CheckNegativeOrZeroInt(value);

                _fallbackInterval = value;
            }
        }

        

        private readonly string _ndisPort;
        /// <summary>
        /// 
        /// </summary>
        public string NdisPort
        {
            get { return _ndisPort; }
        }

        private static bool _isNetworkCableConnected = true;
        /// <summary>
        /// 
        /// </summary>
        public bool IsNetworkCableConnected
        {
            get { return _isNetworkCableConnected; }
            set
            {
                _isNetworkCableConnected = value;
                if(_isNetworkCableConnected)
                {
                    if (_wasCallSetFallback)
                    {
                        SetFallback(false, true);
                        _wasCallSetFallback = false;
                    }
                }
            }
        }

        private readonly string _deviceName;
        /// <summary>
        /// 
        /// </summary>
        public string DeviceName
        {
            get { return _deviceName; }
        }

        private const string ParametersSubPath = @"\Parms\TCPIP";

        private static volatile string[] _deviceNames;
        private static readonly object _deviceNamesSync = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// TODO : not plug and play aware
        public static string[] GetDeviceNames()
        {
            if (null == _deviceNames)
                lock (_deviceNamesSync)
                {
                    if (null == _deviceNames)
                    {

                        LinkedList<string> names = new LinkedList<string>();

                        RegistryKey rk = null;
                        try
                        {
                            rk = Registry.LocalMachine.OpenSubKey("Comm");
                            string[] subkeys = rk.GetSubKeyNames();

                            foreach (string subkey in subkeys)
                            {
                                try
                                {
                                    RegistryKey srk = rk.OpenSubKey(subkey + ParametersSubPath);
                                    if (srk != null)
                                        names.AddLast(subkey);
                                }
                                catch
                                {
                                }
                            }

                        }
                        catch
                        {
                            return null;
                        }
                        finally
                        {
                            rk.Close();
                        }

                        var deviceNames = new string[names.Count];
                        names.CopyTo(deviceNames, 0);

                        _deviceNames = deviceNames;
                    }
                }

            return _deviceNames;
        }


        /// <summary>
        /// does not flush the registry
        /// </summary>
        /// <param name="ifaceRegistry"></param>
        /// <param name="forceStatic"></param>
        /// <param name="rebindInterface"></param>
        private void ApplyFallbackSettings(
            [NotNull] RegistryKey ifaceRegistry,
            bool forceStatic,
            bool rebindInterface)
        {
            
            ifaceRegistry.SetValue("AutoIP", _fallbackIp, RegistryValueKind.String);
            ifaceRegistry.SetValue("AutoMask", _fallbackMask, RegistryValueKind.String);
            ifaceRegistry.SetValue("AutoSubnet", _fallbackSubnet, RegistryValueKind.String);
            ifaceRegistry.SetValue("AutoCfg",1,RegistryValueKind.DWord);
            ifaceRegistry.SetValue("AutoInterval",_fallbackInterval,RegistryValueKind.DWord);


            if (forceStatic)
            {
                try
                {
                    ifaceRegistry.SetValue(ValueEnableDhcp, 0, RegistryValueKind.DWord);
                    ifaceRegistry.SetValue(ValueIpAddress, _fallbackIp, RegistryValueKind.String);
                    ifaceRegistry.SetValue(ValueSubnetMask, _fallbackMask, RegistryValueKind.String);
                    ifaceRegistry.SetValue(ValueDefaultGateway, _fallbackGateway, RegistryValueKind.String);
                    Console.WriteLine("CeNetworkManagement: Reverting IP settings to fallback " + _fallbackIp + "/" +
                                      _fallbackMask);

                }
                catch
                {
                }
            }

            if (rebindInterface)
            {
                ApplySystemNetworkSettings();
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadNICStatistics()
        {
            int rVal;

            try
            {
                IntPtr hFile = DllCoredll.CreateFile(_ndisPort, (uint)FileAccess.ReadWrite, (uint)FileShare.None, 0, (uint)FileMode.Open, 0, 0);

                byte[] AdapterName = Encoding.Unicode.GetBytes(_deviceName);
                int numBytesReturned;

                DllCoredll.NIC_STATISTICS nicStatistics = new DllCoredll.NIC_STATISTICS();

                rVal = DllCoredll.DeviceIoControl(
                    hFile,
                    DllCoredll.IOCTL_NDISUIO_NIC_STATISTICS,
                    AdapterName, AdapterName.Length,
                    ref nicStatistics, Marshal.SizeOf(nicStatistics),
                    out numBytesReturned,
                    IntPtr.Zero);
                    
            }
            catch
            {
                return Marshal.GetLastWin32Error();
            }

            if (rVal == 1)
                return 0;
            
            return Marshal.GetLastWin32Error();
        }

        /// <summary>
        /// applies the settings to the CE below
        /// </summary>
        /// <returns></returns>
        private int ApplySystemNetworkSettings()
        {
            IntPtr hFile;

            try
            {

                hFile = DllCoredll.CreateFile(_ndisPort, (uint)FileAccess.ReadWrite, (uint)FileShare.None, 0, (uint)FileMode.Open, 0, 0);
            }
            catch (Exception e)
            {
                Sys.HandledExceptionAdapter.Examine(e);
                return Marshal.GetLastWin32Error();
            }



            byte[] AdapterName = Encoding.Unicode.GetBytes(_deviceName);
            int resultCode;

            try
            {
                int numBytesReturned;
                resultCode = DllCoredll.DeviceIoControl(
                    hFile,
                    DllCoredll.IOCTL_NDIS_REBIND_ADAPTER, 
                    AdapterName, AdapterName.Length, 
                    IntPtr.Zero, 
                    0, 
                    out numBytesReturned,
                    IntPtr.Zero);
            }
            catch(Exception e)
            {
                resultCode = Marshal.GetLastWin32Error(); 
                Sys.HandledExceptionAdapter.Examine(e);
                
            }
            finally
            {
                if (hFile != IntPtr.Zero)
                {
                    try { DllCoredll.CloseHandle(hFile); }
                    catch
                    {
                    }
                }
            }

            if (resultCode == 1)
                return 0;

            return Marshal.GetLastWin32Error();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForceRebindInterface()
        {
            ApplySystemNetworkSettings();
        }

        private static bool _wasCallSetFallback = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applyAsStaticSettings"></param>
        /// <param name="rebindInterface"></param>
        public void SetFallback(bool applyAsStaticSettings, bool rebindInterface)
        {
            if (applyAsStaticSettings == false && rebindInterface)
                _wasCallSetFallback = true;

            var ifaceRegistry = OpenRegistryKey(true);

            try
            {
                ApplyFallbackSettings(ifaceRegistry,applyAsStaticSettings, rebindInterface);
            }
            finally
            {
                ForceRegistryFlush(ifaceRegistry);
            }
            
        }

        private const string ValueDns = "DNS";
        private const string ValueEnableDhcp = "EnableDHCP";
        private const string ValueIpAddress = "IpAddress";
        private const string ValueSubnetMask = "SubnetMask";
        private const string ValueDefaultGateway = "DefaultGateway";

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="SystemException"></exception>
        public void SetDynamicDns()
        {

            var ifaceRegistry = OpenRegistryKey(true);

            try
            {
                ifaceRegistry.DeleteValue(ValueDns, false);
            }
            finally
            {
                ForceRegistryFlush(ifaceRegistry);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dns"></param>
        /// <exception cref="SystemException"></exception>
        public void SetStaticDns(string[] dns)
        {
            if (dns != null &&
                dns.Length > 0)
            {
                foreach (string ipAddress in dns)
                    IPHelper.CheckValidity4(ipAddress);
            }
            else
            {
                dns = new string[1];
                dns[0] = ZeroIp;
            }

            var ifaceRegistry = OpenRegistryKey(true);

             ifaceRegistry.SetValue(ValueDns, dns, RegistryValueKind.MultiString);

            ForceRegistryFlush(ifaceRegistry);
        }

        /// <summary>
        /// sets the IP to static settings
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="subnetMask"></param>
        /// <param name="gateway"></param>
        /// <exception cref="InvalidIPException">if the IP or the gateway IP is invalid</exception>
        /// <exception cref="InvalidSubnetMaskException">if the mask is invalid</exception>
        /// <exception cref="SystemException">if the network configuration was unable to be applied</exception>
        public void SetStatic(string ipAddress, string subnetMask, string gateway)
        {
            IPHelper.CheckValidity4(ipAddress);
            IPHelper.CheckMaskValidity4(subnetMask);

            if (!string.IsNullOrEmpty(gateway))
                IPHelper.CheckValidity4(gateway);
            else
                gateway = ZeroIp;

            var ifaceRegistry = OpenRegistryKey(true);

            try
            {

                try
                {
                    ifaceRegistry.SetValue(ValueEnableDhcp, 0, RegistryValueKind.DWord);
                    ifaceRegistry.SetValue(ValueIpAddress, ipAddress, RegistryValueKind.String);
                    ifaceRegistry.SetValue(ValueSubnetMask, subnetMask, RegistryValueKind.String);
                    ifaceRegistry.SetValue(ValueDefaultGateway, gateway, RegistryValueKind.String);

                    ApplyFallbackSettings(ifaceRegistry, false, false);
                }
                catch
                {
                    ApplyFallbackSettings(ifaceRegistry, true, true);
                    throw;
                }

                int applyRet = ApplySystemNetworkSettings();
                if (applyRet != 0)
                {
                    Console.WriteLine("CeNetworkManagement: Static IP settings for " + DeviceName + " failed with code " +
                                      applyRet);
                    throw new SystemException("Error during network settings apply\nError " + applyRet);
                }

                Console.WriteLine("CeNetworkManagement: Static IP settings for " + DeviceName + " successfuly applied to " +
                                  ipAddress + "/" + subnetMask);

            }
            finally
            {
                ForceRegistryFlush(ifaceRegistry);
            }

            
        }

        private static int _dhcpMaxRetry = 2;//-1 == 0xffffffff
        /// <summary>
        /// 
        /// </summary>
        public static int DhcpMaxRetry
        {
            get
            {
                return _dhcpMaxRetry;
            }
            set
            {
                Validator.CheckZero(value);

                if (value > 0)
                    _dhcpMaxRetry = value;
                else
                    _dhcpMaxRetry = -1;
            }
        }

        /// <summary>
        /// sets the IP to dynamic
        /// </summary>
        /// <exception cref="SystemException">if setting the network configuration failed</exception>
        public void SetDynamic()
        {
            var ifaceRegistry = OpenRegistryKey(true);

            try
            {
                try
                {
                    ifaceRegistry.SetValue(ValueEnableDhcp, 1, RegistryValueKind.DWord);
                    ifaceRegistry.SetValue("DhcpMaxRetry", _dhcpMaxRetry, RegistryValueKind.DWord);
                    ifaceRegistry.SetValue("DhcpConstantRate", 1, RegistryValueKind.DWord);

                    try
                    {
                        ifaceRegistry.DeleteValue("DhcpIPAddress");
                    }
                    catch
                    {
                    }

                    try
                    {
                        ifaceRegistry.DeleteValue("DhcpSubnetMask");
                    }
                    catch
                    {
                    }

                    try
                    {
                        ifaceRegistry.DeleteValue("DhcpDefaultGateway");
                    }
                    catch
                    {
                    }

                    try
                    {
                        ifaceRegistry.DeleteValue("DhcpDNS");
                    }
                    catch
                    {
                    }

                    try
                    {
                        ifaceRegistry.DeleteValue("DhcpWINS");
                    }
                    catch
                    {
                    }

                    ApplyFallbackSettings(ifaceRegistry,false, false);
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception error)
                {
                    Console.WriteLine("CeNetworkManagement: Failed to set dynamic IP settings with error\r\n" +
                                      error.Message);

                    ApplyFallbackSettings(ifaceRegistry,true, true);
                    
                    throw;
                }

                int applyRet = ApplySystemNetworkSettings();
                if (applyRet != 0)
                {
                    Console.WriteLine("CeNetworkManagement: Dynamic IP settings for " + DeviceName + " failed with code " +
                                      applyRet);
                    throw new SystemException("Error during network settings apply\nError " + applyRet);
                }

                Console.WriteLine("CeNetworkManagement: Dynamic IP settings for " + DeviceName + " successfuly applied");
            }
            finally
            {
                ForceRegistryFlush(ifaceRegistry);    
            }

            
        }

        private void ForceRegistryFlush(RegistryKey registryKey)
        {
            if (registryKey == null)
                return;

            try
            {
                registryKey.Flush();
            }
            catch
            {
            }
            
            try
            {
                registryKey.Close();
            }
            catch
            {
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDhcp"></param>
        /// <param name="ipAddress"></param>
        /// <param name="subnetMask"></param>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public bool GetIPSettings(out bool isDhcp, out string ipAddress, out string subnetMask, out string gateway)
        {
            var ifaceRegistry = OpenRegistryKey(false);

            try
            {
                return GetIPSettings(ifaceRegistry, out isDhcp, out ipAddress, out subnetMask, out gateway);
            }
            finally
            {
                ifaceRegistry.Close();
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="isDhcp"></param>
        /// <param name="ipAddress"></param>
        /// <param name="subnetMask"></param>
        /// <param name="gateway"></param>
        /// <returns></returns>
        public bool GetIPSettings(
            string deviceName,
            out bool isDhcp, 
            out string ipAddress, 
            out string subnetMask, 
            out string gateway)
        {
            isDhcp = false;
            ipAddress = ZeroIp;
            subnetMask = ZeroIp;
            gateway = ZeroIp;

            if (string.IsNullOrEmpty(deviceName))
                return false;

            RegistryKey rk;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey(@"Comm\" + deviceName + ParametersSubPath, false);
                if (null == rk)
                    return false;
            }
            catch
            {
                return false;
            }
            

            return GetIPSettings(rk, out isDhcp, out ipAddress, out subnetMask, out gateway);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDynamic"></param>
        /// <param name="dns"></param>
        /// <returns></returns>
        public bool GetDnsSettings(ref bool isDynamic, out string[] dns)
        {
            var ifaceRegistry = OpenRegistryKey(false);
            try
            {
                return GetDnsSettings(ifaceRegistry, ref isDynamic, out dns);
            }
            finally
            {
                ifaceRegistry.Close();
            }
        }

        private bool GetDnsSettings(
            [NotNull] RegistryKey rootRegistry, 
            ref bool isDynamic, 
            out string[] dns)
        {
            bool result;

            try
            {
                dns = (string[])rootRegistry.GetValue(ValueDns);
                if (dns == null || dns.Length == 0)
                {
                    isDynamic = true;
                    dns = (string[])rootRegistry.GetValue("DhcpDNS");

                    result = (dns != null && dns.Length != 0);
                }
                else
                {
                    isDynamic = false;
                    result = true;
                }
            }
            catch
            {
                dns = null;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// returns true, if loading settings succeeded
        /// </summary>
        /// <param name="rootRegistry"></param>
        /// <param name="isDhcp"></param>
        /// <param name="ipAddress"></param>
        /// <param name="subnetMask"></param>
        /// <param name="gateway"></param>
        /// <returns></returns>
        private bool GetIPSettings(
            RegistryKey rootRegistry, 
            out bool isDhcp, 
            out string ipAddress, 
            out string subnetMask, 
            out string gateway)
        {
            //[HKEY_LOCAL_MACHINE\Comm\<DEVICENAME>\Parms\TCPIP]

            isDhcp = false;
            ipAddress = ZeroIp;
            subnetMask = ZeroIp;
            gateway = ZeroIp;


                if (rootRegistry == null)
                    return false;

                object objectIp = null;
                object objectSubnetMask = null;
                object objectGateway = null;

                try
                {
                    isDhcp = ((int) rootRegistry.GetValue(ValueEnableDhcp)) > 0;
                }
                catch
                {
                    return false;
                }


            if (isDhcp)
            {
                // dynamic
                try
                {
                    objectIp = rootRegistry.GetValue("DhcpIpAddress");
                }
                catch
                {
                }

                try
                {
                    objectSubnetMask = rootRegistry.GetValue("DhcpSubnetMask");
                }
                catch
                {
                }

                try
                {
                    objectGateway = rootRegistry.GetValue("DhcpDefaultGateway");
                }
                catch
                {
                }
            }
            else
            {
                // static config
                try
                {
                    objectIp = rootRegistry.GetValue(ValueIpAddress);
                }
                catch
                {
                }


                try
                {
                    objectSubnetMask = rootRegistry.GetValue(ValueSubnetMask);
                }
                catch
                {
                }

                try
                {
                    objectGateway = rootRegistry.GetValue(ValueDefaultGateway);
                }
                catch
                {
                }
            }

            var ipAddresses = objectIp as string[];
            if (ipAddresses != null && ipAddresses.Length > 0)
                ipAddress = ipAddresses[0];
            else
                ipAddress = (objectIp as string) ?? ZeroIp;

            var subnetMasks = objectSubnetMask as string[];
            if (subnetMasks != null && subnetMasks.Length > 0)
                subnetMask = subnetMasks[0];
            else
                subnetMask = (objectSubnetMask as string) ?? ZeroIp;

            var gateways = objectGateway as string[];
            if (gateways != null && gateways.Length > 0)
                gateway = gateways[0];
            else
                gateway = (objectGateway as string) ?? ZeroIp;

            return true;

        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        static char NibbleAToHex(char c)
        {
            char ucResult = (char)0;

            if (!((c < '0') || (c > '9')))
            {
                ucResult = (char)(c - '0');
            }
            else if (!((c < 'A') || (c > 'F')))
            {
                ucResult = (char)(c - 'A' + 10);
            }
            else if (!((c < 'a') || (c > 'f')))
            {
                ucResult = (char)(c - 'a' + 10);
            }

            return ucResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registryString"></param>
        /// <returns></returns>
// ReSharper disable once UnusedMember.Local
        static string RegistryString2MACString(string registryString )
        {
            int i,j = 0;

            byte[] regBytes = Encoding.Unicode.GetBytes(registryString);

            byte[] macAddressBytes = new byte[6];

            for (i = 0; i < macAddressBytes.Length; i++)
            {
                macAddressBytes[i] = (byte)(NibbleAToHex((char)regBytes[j++]) << 4);
                macAddressBytes[i] |= (byte)(0xF & NibbleAToHex((char)regBytes[j++]));
                j++;
            }



            return EthernetLayer.MAC2String(macAddressBytes);
        }

        /// <summary>
        /// returns MAC address of the pr
        /// </summary>
        /// <returns></returns>
        public string GetMAC()
        {
            //HKEY_LOCAL_MACHINE\Comm\<device_name>\ - MacAddress

            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"Comm\"+_deviceName);

                if (null != rk)
                {
                    object mac = rk.GetValue("MacAddress");

                    if (null == mac)
                        return ZeroMac;

                    return (string)mac; 
                        //RegistryString2MACString((string)mac);
                }
                
                return ZeroMac;
            }
            catch
            {
                return ZeroMac;
            }
        }

        
        /*
        public static bool IsNetworkCableConnected()
        {
             bool rVal = false;
           
            IntPtr hFile = DllCoredll.CreateFile(_ndisPort, FileAccess.ReadWrite, FileShare.None, 0, FileMode.Open, 0, 0);
            byte[] adapterName = System.Text.Encoding.Unicode.GetBytes(_deviceName);
            byte[] adapterNameNull =  new byte[adapterName.Length + 2];//System.Text.Encoding.Unicode.GetBytes(_deviceName+"\0");
            int numBytesReturned;
            DllCoredll.NIC_STATISTICS nicStats = new DllCoredll.NIC_STATISTICS();

            Array.Copy(adapterName, adapterNameNull, adapterName.Length);
            adapterNameNull[adapterNameNull.Length - 2] = (byte)'\0';
            adapterNameNull[adapterNameNull.Length - 1] = 0;

            nicStats.ptcDeviceName = Marshal.AllocHGlobal(adapterNameNull.Length);
            nicStats.Size = (uint)Marshal.SizeOf(nicStats);
            Marshal.Copy(adapterNameNull, 0, nicStats.ptcDeviceName, adapterNameNull.Length);

            try
            {
                int diRet = DllCoredll.DeviceIoControl(hFile, DllCoredll.IOCTL_NDISUIO_NIC_STATISTICS, adapterName, adapterName.Length,
                    ref nicStats, (int)nicStats.Size, out numBytesReturned, IntPtr.Zero);
                if (diRet != 0)
                {
                    if (nicStats.MediaState == DllCoredll.MEDIA_STATE_CONNECTED)
                    {
                        rVal = true;
                    }
                }
                else
                {
                    int err = Marshal.GetLastWin32Error();
                }
            }

            catch (Exception ex)
            {
                throw (ex);
            }

            finally
            {
                if (hFile != IntPtr.Zero)
                {
                    DllCoredll.CloseHandle(hFile);
                }

                if (nicStats.ptcDeviceName != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(nicStats.ptcDeviceName);
                }
            }

            return (rVal);
        }
        */
    }
}

