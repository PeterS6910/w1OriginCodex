using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using System.Net;

using System.Diagnostics;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// helper for IP to string conversions
    /// </summary>
    public class IPHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddressString"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool TryParse(string ipAddressString, out IPAddress ip)
        {
            if (string.IsNullOrEmpty(ipAddressString))
            {
                ip = null;
                return false;
            }

            try
            {
                IPAddress tmpIP = IPAddress.Parse(ipAddressString);
                ip = tmpIP;
                return true;
            }
            catch
            {
                ip = null;
                return false;
            }
        }

        /// <summary>
        /// returns true, if the specified IPv4 is valid
        /// </summary>
        /// <param name="ipAddress">string interpretation of the IPv4</param>
        public static bool IsValid4(string ipAddress)
        {
            return IsValid4(ipAddress, false);
        }

        /// <summary>
        /// returns true, if the specified IPv4 is valid
        /// </summary>
        /// <param name="ipAddress">string interpretation of the IPv4</param>
        /// <param name="isHost">if true, network and broadcast addresses are not allowed</param>
        /// <returns></returns>
        public static bool IsValid4(string ipAddress, bool isHost)
        {
            Regex aRegex = null;
            try
            {
                if (!isHost)
                    aRegex = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$");
                else
                    aRegex = new Regex(@"^(25[0-5]|2[0-4][0-9]|[1][0-9][0-9]|[1-9][0-9]|[1-9])\."+
                        @"((25[0-5]|2[0-4][0-9]|[1][0-9][0-9]|[1-9][0-9]|[0-9])\.){2}"+
                        @"(25[0-4]|2[0-4][0-9]|[1][0-9][0-9]|[1-9][0-9]|[1-9])$");
            }
            catch (Exception err)
            {
                DebugHelper.TryBreak("IPHelper.IsValid4 possibly Regex problem", err, aRegex);
                return false;
            }

            return (aRegex.Match(ipAddress).Success);
        }

        /// <summary>
        /// returns true, if the specified IP is valid IPv4 mask
        /// </summary>
        /// <param name="subnetMask">string interpretation of the IPv4 mask</param>
        public static bool IsValidMask4(string subnetMask)
        {
            if (Validator.IsNullString(subnetMask))
                return false;

            IPAddress ip;
            try
            {
                ip = IPAddress.Parse(subnetMask);
            }
            catch
            {
                return false;
            }

            byte[] arIPBytes = ip.GetAddressBytes();

            // IPv4 check
            if (4 != arIPBytes.Length)
                return false;


            for (int i = 0; i < arIPBytes.Length - 1; i++)
            {
                if (arIPBytes[i] < arIPBytes[i + 1])
                    return false;
            }

            byte[] allowedScopes = { 0, 128, 192, 224, 240, 248, 252, 254, 255 };

            foreach (byte byScope in arIPBytes)
            {
                bool bFound = false;
                foreach (byte byAllowedScope in allowedScopes)
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
        /// parses IPAddress object from string, if possible; supports both IPv4/IPv6
        /// </summary>
        /// <param name="ipAddress">string IP presentation</param>
        /// <exception cref="InvalidIPException">if the IP string format is invalid</exception>
        public static IPAddress Parse(string ipAddress)
        {
            if (!IsValid4(ipAddress))
                throw new InvalidIPException(ipAddress);

            try
            {
                return IPAddress.Parse(ipAddress);
            }
            catch (FormatException)
            {
                throw new InvalidIPException(ipAddress);
            }
            catch (Exception err)
            {
                DebugHelper.TryBreak("IPHelper.Parse Unexpected exception", err, ipAddress);
                return null; 
            }

        }


        /// <summary>
        /// throws InvalidIPException if the format is invalid
        /// </summary>
        /// <param name="ipString">string presentation of the IP</param>
        /// <exception cref="InvalidIPException">if the IP format is invalid</exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static IPAddress CheckValidity(string ipString)
        {
            //if (!IsValid4(ipString))
            //   throw new InvalidIPException(ipString);

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
        /// 
        /// </summary>
        /// <param name="ipString"></param>
        /// <returns></returns>
        /// <exception cref="InvalidIPException"></exception>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static IPAddress CheckValidity4(string ipString)
        {
            if (!IsValid4(ipString))
               throw new InvalidIPException(ipString);

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
        /// <param name="maskIP">string presentation of the IPv4 mask</param>
        [DebuggerHidden]
        [DebuggerStepThrough]
        public static void CheckMaskValidity4(string maskIP)
        {
            if (!IsValidMask4(maskIP))
                throw new InvalidSubnetMaskException(maskIP);
        }

        private const int I256 = 256;

        /// <summary>
        /// converts IPv4 into MSB-first integer;
        /// returns -1 if the address family is not IPv4
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static int ToInt32(IPAddress ipAddress)
        {
            if (null == ipAddress)
                throw new ArgumentNullException("ipAddress");

            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {

                byte[] arIP = ipAddress.GetAddressBytes();
                return arIP[0] + arIP[1] * I256 + arIP[2] * I256 * I256 + arIP[3] * I256 * I256 * I256;
            }
            
            return -1;
        }

        /// <summary>
        /// converts IPv4 into LSB-first integer;
        /// returns -1 if the address family is not IPv4
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static int ToReverseInt(IPAddress ipAddress)
        {
            if (null == ipAddress)
                throw new ArgumentNullException("ipAddress");

            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] arIP = ipAddress.GetAddressBytes();
                return arIP[0] * I256 * I256 * I256 + arIP[1] * I256 * I256 + arIP[2] * I256 + arIP[3];
            }
            
            return -1;
        }

        /// <summary>
        /// converse IPv4 LSB-first binary representation into IPAddress instance if possible
        /// </summary>       
        public static IPAddress FromReverseInt4(int ipBinary)
        {
            byte[] arIP = new byte[4];

            arIP[0] = (byte)(ipBinary / (I256 * I256 * I256));
            ipBinary = ipBinary % (I256 * I256 * I256);

            arIP[1] = (byte)(ipBinary / (I256 * I256));
            ipBinary = ipBinary % (I256 * I256);

            arIP[2] = (byte)(ipBinary / (I256));
            ipBinary = ipBinary % (I256);

            arIP[3] = (byte)(ipBinary);

            return new IPAddress(arIP);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static IPAddress SubnetFromIP4(IPAddress ip, IPAddress mask)
        {

            if (null == ip ||
                null == mask)
                return null;

            byte[] arrayIP = ip.GetAddressBytes();
            byte[] arrayMask = mask.GetAddressBytes();

            if (arrayIP == null || arrayMask == null ||
                arrayIP.Length != arrayMask.Length)
                return null;

            byte[] arraySubnet = new byte[arrayIP.Length];

            for(int i = 0;i<arrayIP.Length;i++)
                arraySubnet[i] = (byte)(arrayIP[i] & arrayMask[i]);

            return new IPAddress(arraySubnet);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string SubnetFromIP4(string ip, string mask)
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

            IPAddress retIP = SubnetFromIP4(ipB, maskB);
            if (null == retIP)
                return null;
            
            return retIP.ToString();
        }

        private const int IPV4_LENGTH = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static IPAddress BroadcastFromIP4(IPAddress ip, IPAddress mask)
        {

            if (null == ip ||
                null == mask ||
                ip.AddressFamily != AddressFamily.InterNetwork ||
                mask.AddressFamily != AddressFamily.InterNetwork)
                return null;          

            byte[] arrayIP = ip.GetAddressBytes();
            byte[] arrayMask = mask.GetAddressBytes();

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
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
