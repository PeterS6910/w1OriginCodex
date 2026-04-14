using System;
using System.Text;

namespace Contal.IwQuick.Net.Microsoft
{
    public class EthernetLayer
    {

        /*
        [DllImport("iphlpapi.dll")]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public static string GetMAC(IPAddress iP)
        {
            if (null == iP)
                return null;

            int iIP = Contal.IwQuick.Net.IPHelper.ToInt32(iP);

            return GetMAC(iIP);

        }*/

        public static string MAC2String(byte[] mac)
        {
            if (null == mac || mac.Length == 0)
                return String.Empty;

            try
            {
                StringBuilder strRet = new StringBuilder();
                for (int i = 0; i < mac.Length; i++)
                {
                    if (i > 0)
                        strRet.Append(':');

                    strRet.Append(mac[i].ToString("x2").ToUpper());
                }

                return strRet.ToString();
            }
            catch
            {
                return String.Empty;
            }
        }

        /*
        public static string GetMAC(int binaryIP)
        {
            byte[] arDstMac = new byte[6];
            uint iMacLength = (uint)arDstMac.Length;
            if (SendARP(binaryIP, 0, arDstMac, ref iMacLength) != 0)
            {
                return null;
            }
            return MAC2String(arDstMac);
        }*/
    }
}
