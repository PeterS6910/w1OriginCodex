using System.Text;

using System.Net;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// Hardware address helper
    /// </summary>
    public class HwAddress
    {
        [DllImport("iphlpapi.dll")]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetMAC(IPAddress ip)
        {
            if (null == ip)
                return null;

            int iIP = IPHelper.ToInt32(ip);

            return GetMAC(iIP);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipInInteger"></param>
        /// <returns></returns>
        public static string GetMAC(int ipInInteger)
        {
            byte[] arDstMac = new byte[6];
            uint iMacLength = (uint)arDstMac.Length;
            if (SendARP(ipInInteger, 0, arDstMac, ref iMacLength) != 0)
            {
                return null;
            }

            StringBuilder strRet = new StringBuilder();
            for (int i = 0; i < iMacLength; i++)
            {
                if (i > 0)
                    strRet.Append(':');

                strRet.Append(arDstMac[i].ToString("x2").ToUpper());
            }

            return strRet.ToString();
        }
    }
}
