using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace Contal.IwQuick.Net.Microsoft
{
    public class NetworkManagement
    {
        private static ManagementObjectCollection GetInterface(string interfaceId,bool isCheckIPEnabled)
        {
            bool bWMIFailure = false;
            ManagementObjectCollection aManClasses = null;
            try
            {
                //ManagementClass aManClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
                string strQuery = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE SettingID=\"" + interfaceId + "\"";
                if (isCheckIPEnabled)
                    strQuery+=" AND IPEnabled = 'TRUE'";
                aManClasses = (new ManagementObjectSearcher(new SelectQuery(strQuery))).Get();
            }
            catch
            {
                bWMIFailure = true;
            }

            if (null == aManClasses)
                bWMIFailure = true;

            if (bWMIFailure)
                throw new InvalidProgramException("WMI initialization failed");

            return aManClasses;
        }

        /// <summary>
        /// Set's a new IP Address and it's Submask of the local machine
        /// </summary>
        /// <param name="ip_address">The IP Address</param>
        /// <param name="subnet_mask">The Submask IP Address</param>
        /// <exception cref="InvalidProgramException">if unable to initialize WMI module</exception>
        /// 
        public static void SetStaticIP(string interfaceId, string iPAddress, string i_strSubnetMask)
        {
            ManagementObjectCollection aManClasses = GetInterface(interfaceId,true);            

            foreach (ManagementObject aManObject in aManClasses)
            {
                
                    try
                    {
                        ManagementBaseObject setIP;
                        ManagementBaseObject newIP =
                            aManObject.GetMethodParameters("EnableStatic");

                        newIP["IPAddress"] = new string[] { iPAddress };
                        newIP["SubnetMask"] = new string[] { i_strSubnetMask };

                        setIP = aManObject.InvokeMethod("EnableStatic", newIP, null);
                        UInt32 iResult = (UInt32) setIP["returnValue"];

                        if (iResult > 1)
                            throw new System.Management.ManagementException("Unable to set static IP");
                        
                    }
                    catch (Exception)
                    {
                        throw;
                    }
            }
        }

        public static void SetDHCP(string interfaceId)
        {
            ManagementObjectCollection aManClasses = GetInterface(interfaceId,true);

            foreach (ManagementObject aManObject in aManClasses)
            {

                try
                {

                    ManagementBaseObject newDNS = aManObject.GetMethodParameters("SetDNSServerSearchOrder");
                    newDNS["DNSServerSearchOrder"] = null;
                    ManagementBaseObject enableDHCP = aManObject.InvokeMethod("EnableDHCP", null, null);
                    UInt32 iResult = (UInt32)enableDHCP["returnValue"];
                    if (iResult > 1)
                        throw new System.Management.ManagementException("Unable to IP by DHCP");

                    ManagementBaseObject setDNS = aManObject.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                }
                catch(Exception)
                {
                    throw;
                }
            }

        }


        public static void SetGateway(string interfaceId,string gateway)
        {

            ManagementObjectCollection aManClassess = GetInterface(interfaceId, true);

            foreach (ManagementObject objMO in aManClassess)
            {
                try
                {
                    ManagementBaseObject setGateway;
                    ManagementBaseObject newGateway =
                        objMO.GetMethodParameters("SetGateways");

                    if (Validator.IsNullString(gateway))
                    {
                        newGateway["DefaultIPGateway"] = new string[] { };
                        newGateway["GatewayCostMetric"] = new int[] { };
                    }
                    else
                    {
                        newGateway["DefaultIPGateway"] = new string[] { gateway };
                        newGateway["GatewayCostMetric"] = new int[] { 1 };
                    }

                    setGateway = objMO.InvokeMethod("SetGateways", newGateway, null);
                    UInt32 iResult = (UInt32)setGateway["returnValue"];
                    if (iResult > 1)
                        throw new System.Management.ManagementException("Unable to set gateway");

                }
                catch (Exception)
                {
                    throw;
                }
                
            }
        }

        public static void SetDNS(string interfaceId, string[] i_arDnsAddresses)
        {
            if (null == i_arDnsAddresses)
                i_arDnsAddresses = new string[0];

            ManagementObjectCollection aManClassess = GetInterface(interfaceId, true);

            foreach (ManagementObject objMO in aManClassess)
            {
                try
                {
                    ManagementBaseObject newDNS =
                        objMO.GetMethodParameters("SetDNSServerSearchOrder");
                    newDNS["DNSServerSearchOrder"] = i_arDnsAddresses;
                    ManagementBaseObject setDNS =
                        objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);

                    UInt32 iResult = (UInt32)setDNS["returnValue"];
                    if (iResult > 1)
                        throw new System.Management.ManagementException("Unable to set DNS");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        
        public static void SetWINS(string interfaceId, string[] i_arWinsAddresses)
        {
            if (null == i_arWinsAddresses)
                i_arWinsAddresses = new string[0];

            ManagementObjectCollection aManClassess = GetInterface(interfaceId, true);

            foreach (ManagementObject objMO in aManClassess)
            {
                try
                {
                    ManagementBaseObject setWINS;
                    ManagementBaseObject wins =
                    objMO.GetMethodParameters("SetWINSServer");

                    
                    wins.SetPropertyValue("WINSPrimaryServer", i_arWinsAddresses[0]);
                    wins.SetPropertyValue("WINSSecondaryServer",
                        i_arWinsAddresses.Length >= 2 ? i_arWinsAddresses[1] : "");

                    setWINS = objMO.InvokeMethod("SetWINSServer", wins, null);

                    UInt32 iResult = (UInt32)setWINS["returnValue"];
                    if (iResult > 1)
                        throw new System.Management.ManagementException("Unable to set WINS");
                }
                catch (Exception)
                {
                    throw;
                }
                  
            }
        }
    }

}
