using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Net.Microsoft
{
    public class EthernetLayer
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public static string GetMAC(IPAddress iP)
        {
            if (null == iP)
                return null;

            int iIP = Contal.IwQuick.Net.IPHelper.ToInt4(iP);

            return GetMAC(iIP);

        }

        public static string GetMAC(int i_iIP)
        {
            byte[] arDstMac = new byte[6];
            uint iMacLength = (uint)arDstMac.Length;
            if (SendARP(i_iIP, 0, arDstMac, ref iMacLength) != 0)
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
