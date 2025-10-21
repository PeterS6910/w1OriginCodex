using System;
using System.Text.RegularExpressions;

using System.Net;

using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// helper for IP to string conversions
    /// </summary>
    public class IPHelper
    {
        private static readonly object _syncRoot = new object();

        private static Regex _ipv4Regex;

        /// <summary>
        /// returns true, if the specified IPv4/IPv6 address is valid
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsValid(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            IPAddress tmpIP;
            return IPAddress.TryParse(ipAddress,out tmpIP);
        }

        /// <summary>
        /// returns true, if the specified IPv4 is valid. network and broadcast addresses are not allowed
        /// </summary>
        /// <param name="ipAddress">string interpretation of the IPv4</param>
        public static bool IsValid4(string ipAddress)
        {
            if (ipAddress == null)
                return false;

            try
            {
                if (_ipv4Regex == null)
                {
                    lock (_syncRoot)
                    {
                        if (_ipv4Regex == null)
                        {
                            _ipv4Regex = new Regex(@"^(25[0-5]|2[0-4][0-9]|[1][0-9][0-9]|[1-9][0-9]|[1-9])\." +
                                  @"((25[0-5]|2[0-4][0-9]|[1][0-9][0-9]|[1-9][0-9]|[0-9])\.){2}" +
                                  @"(25[0-4]|2[0-4][0-9]|[1][0-9][0-9]|[1-9][0-9]|[1-9])$");
                        }
                    }
                }

                return _ipv4Regex.IsMatch(ipAddress);    
            }
            catch (Exception aError)
            {
                Debug.Assert(false, aError.Message);
                return false;
            }
        }

        public static bool IsValidMask4(string subnetMask)
        {
            IPAddress aIP;
            if (!IPAddress.TryParse(subnetMask, out aIP))
                return false;

            byte[] arIPBytes = aIP.GetAddressBytes();

            // IPv4 check
            if (4 != arIPBytes.Length)
                return false;


            for (int i = 0; i < arIPBytes.Length - 1; i++)
            {
                if (arIPBytes[i] < arIPBytes[i + 1])
                    return false;
            }

            byte[] arAllowedScopes = { 0, 128, 192, 224, 240, 248, 252, 254, 255 };

            foreach (byte byScope in arIPBytes)
            {
                bool bFound = false;
                foreach (byte byAllowedScope in arAllowedScopes)
                {
                    if (byScope == byAllowedScope)
                    {
                        bFound = true;
                        break;
                    }      
                }

                if (!bFound)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localIP"></param>
        /// <param name="subnetMask"></param>
        /// <param name="ipToCompare"></param>
        /// <returns></returns>
        public static bool IsSameNetwork4(
            [NotNull] IPAddress localIP, 
            [NotNull] IPAddress subnetMask, 
            [NotNull] IPAddress ipToCompare)
        {
            Validator.CheckForNull(localIP,"localIP");
            Validator.CheckForNull(subnetMask,"subnetMask");
            Validator.CheckForNull(ipToCompare,"ipToCompare");

            byte[] arNetworkIP = localIP.GetAddressBytes();
            byte[] arMask = subnetMask.GetAddressBytes();

            for (int i = 0; i < 4; i++)
            {
                arNetworkIP[i] = (byte)(arNetworkIP[i] & arMask[i]);
            }

            byte[] arTestedNetworkIP = ipToCompare.GetAddressBytes();
            for (int i = 0; i < 4; i++)
            {
                arTestedNetworkIP[i] = (byte)(arTestedNetworkIP[i] & arMask[i]);
            }

            bool bSame = true;
            for (int i = 0; i < 4; i++)
            {
                if (arNetworkIP[i] != arTestedNetworkIP[i])
                {
                    bSame = false;
                    break;
                }
            }

            return bSame;
        }

        /// <summary>
        /// returns true, if the local and other IP are from the same network
        /// </summary>
        /// <param name="localIP">IP from the network</param>
        /// <param name="subnetMask">IPv4 mask</param>
        /// <param name="ipToCompare">another IP to be evaluated</param>
        public static bool IsSameNetwork4(string localIP, string subnetMask, string ipToCompare)
        {
            IPAddress aIPLocal;
            if (!TryParse(localIP, out aIPLocal))
                throw new InvalidIPException(localIP);

            CheckMaskValidity4(subnetMask);
            IPAddress aSubnetMask = IPAddress.Parse(subnetMask);

            IPAddress aIP2Compare;
            if (!TryParse(ipToCompare, out aIP2Compare))
                throw new InvalidIPException(ipToCompare);

            return IsSameNetwork4(aIPLocal, aSubnetMask, aIP2Compare);
        }

        /// <summary>
        /// converts the string into the IPAddress or throws exception if invalid format;
        ///  supports both IPv4/IPv6
        /// </summary>
        /// <param name="ipAddress">string IP presentation</param>
        /// <exception cref="InvalidIPException">if the IP format specified is invalid</exception>
        public static IPAddress Parse(string ipAddress)
        {
            try
            {
                return IPAddress.Parse(ipAddress);
            }
            catch (FormatException)
            {
                throw new InvalidIPException(ipAddress);
            }
            catch (Exception aError)
            { 
#if DEBUG
                Debug.Assert(false,aError.Message);
#endif
// ReSharper disable once HeuristicUnreachableCode
                return null; 
            }
        }

        /// <summary>
        /// converts the string into the IPAddress if possible; 
        /// returns true if conversion successful, false otherwise
        /// </summary>
        /// <param name="ipAddress">string IP representation</param>
        /// <param name="ip">output of the IPAddress class instance</param>
        public static bool TryParse(string ipAddress, out IPAddress ip)
        {
            return IPAddress.TryParse(ipAddress, out ip);
        }

        /// <summary>
        /// throws InvalidIPException if the format is invalid
        /// </summary>
        /// <param name="ipString">string presentation of the IP</param>
        /// <exception cref="InvalidIPException">if the IP format is invalid</exception>
        public static IPAddress CheckValidity(string ipString)
        {
            //if (!IsValid4(ipString))
            //    throw new InvalidIPException(ipString);

            try
            {
                IPAddress ip = IPAddress.Parse(ipString);
                return ip;
            }
            catch//(Exception err)
            {
                throw new InvalidIPException(ipString);
            }


        }

        /// <summary>
        /// checks whether the IPv4 mask is valid, if not, the InvalidSubnetMaskException is raised
        /// </summary>
        /// <param name="ip">string presentation of the IPv4 mask</param>
        public static void CheckMaskValidity4(string ip)
        {
            if (!IsValidMask4(ip))
                throw new InvalidSubnetMaskException(ip);
        }

        private const int BYTE_SPACE = 256;

        /// <summary>
        /// converts the IPv4 into MSB-first integer; 
        /// for other address families 0 is always returned
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static int ToInt4([NotNull] IPAddress ipAddress)
        {
            Validator.CheckForNull(ipAddress,"ipAddress");

            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] arIP = ipAddress.GetAddressBytes();
                return arIP[0] + arIP[1] * BYTE_SPACE + arIP[2] * BYTE_SPACE * BYTE_SPACE + arIP[3] * BYTE_SPACE * BYTE_SPACE * BYTE_SPACE;
            }
            
            return 0;
        }

        /// <summary>
        /// converts the IPv4 into MSB-last integer;
        /// for other address families 0 is always returned
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static int ToReverseInt4([NotNull] IPAddress ipAddress)
        {
            Validator.CheckForNull(ipAddress,"ipAddress");
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] ipBytes = ipAddress.GetAddressBytes();
                return ipBytes[0] * BYTE_SPACE * BYTE_SPACE * BYTE_SPACE + ipBytes[1] * BYTE_SPACE * BYTE_SPACE + ipBytes[2] * BYTE_SPACE + ipBytes[3];
            }
            
            return 0;
        }

        /// <summary>
        /// converse IPv4 LSB-first binary representation into IPAddress instance if possible
        /// </summary>
        public static IPAddress FromReverseInt4(int ipBinary)
        {
            byte[] arIP = new byte[4];

            arIP[0] = (byte)(ipBinary / (BYTE_SPACE * BYTE_SPACE * BYTE_SPACE));
            ipBinary = ipBinary % (BYTE_SPACE * BYTE_SPACE * BYTE_SPACE);

            arIP[1] = (byte)(ipBinary / (BYTE_SPACE * BYTE_SPACE));
            ipBinary = ipBinary % (BYTE_SPACE * BYTE_SPACE);

            arIP[2] = (byte)(ipBinary / (BYTE_SPACE));
            ipBinary = ipBinary % (BYTE_SPACE);

            arIP[3] = (byte)(ipBinary);

            return new IPAddress(arIP);
        }

        public static bool IsLoopBack(string ipString) {
            if (Validator.IsNullString(ipString))
                return false;

            ipString = ipString.Trim().ToLower();

            return 

                ipString == "127.0.0.1" ||
                ipString == "localhost" ||
                ipString == "loopback" ||
                ipString == "::1" ||
                ipString == "0:0:0:0:0:0:0:1" ||
                ipString == "0000:0000:0000:0000:0000:0000:0000:0001";
        
        }

        private const int IPV4_LENGTH = 4;

        public static IPAddress BroadcastFromIP4(IPAddress ip, IPAddress mask)
        {

            if (null == ip ||
                null == mask ||
                ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork ||
                mask.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                return null;



            byte[] arrayIP = ip.GetAddressBytes();
            byte[] arrayMask = mask.GetAddressBytes();

// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (arrayIP == null || arrayMask == null ||
                arrayIP.Length != arrayMask.Length)
                return null;

            byte[] arraySubnet = new byte[arrayIP.Length];

            for (int i = 0; i < IPV4_LENGTH; i++)
                arraySubnet[i] = (byte)(arrayIP[i] & arrayMask[i]);

            // invert the mask
            for (int i = 0; i < IPV4_LENGTH; i++)
            {
                arrayMask[i] = (byte)~arrayMask[i];
            }

            // apply inverted mask to subnet to get the broadcast
            for (int i = 0; i < IPV4_LENGTH; i++)
                arraySubnet[i] = (byte)(arraySubnet[i] ^ arrayMask[i]);

            return new IPAddress(arraySubnet);

        }

        public static string BroadcastFromIP4(string ip, string mask)
        {
            IPAddress ipB, maskB;

            try
            {
                ipB = IPAddress.Parse(ip);
                maskB = IPAddress.Parse(mask);
            }
            catch
            {
                return null;
            }

            IPAddress retIP = BroadcastFromIP4(ipB, maskB);
            if (null == retIP)
                return null;
            
            return retIP.ToString();
        }
    }
}
