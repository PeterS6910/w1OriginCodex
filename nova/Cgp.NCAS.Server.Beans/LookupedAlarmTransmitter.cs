using System;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class LookupedAlarmTransmitter
    {
        public const string COLUMN_IP_ADDRESS = "IPAddress";
        public const string COLUMN_MAINBOARD_TYPE = "MainboardType";
        public const string COLUMN_CCU_VERSION = "Cat12ceVersion";
        public const string COLUMN_CE_VERSION = "CeVersion";

        public string IpAddress { get; private set; }
        public string MainboardType { get; private set; }
        public string Cat12ceVersion { get; private set; }
        public string CeVersion { get; private set; }

        public LookupedAlarmTransmitter(string ipAddress)
            : this(
                ipAddress,
                string.Empty,
                string.Empty,
                string.Empty)
        {

        }

        public LookupedAlarmTransmitter(
            string ipAddress,
            string mainboardType,
            string cat12CeVersion,
            string ceVersion)
        {
            IpAddress = ipAddress;
            MainboardType = mainboardType;
            Cat12ceVersion = cat12CeVersion;
            CeVersion = ceVersion;
        }

        public override string ToString()
        {
            return IpAddress;
        }
    }
}
