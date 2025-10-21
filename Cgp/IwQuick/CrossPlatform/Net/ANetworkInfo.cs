using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public abstract class ANetworkInfo : INetworkInfo
    {
        public abstract bool IsLocalIp(IPAddress ip);

        
        public abstract IEnumerable<IPAddress> LocalIpAddresses { get; }

        public abstract IEnumerable<string> GetIpAddressReport();

        public abstract bool IsLocalNetwork4(IPAddress ip);
        public abstract string MachineName { get; }

        protected class Ipv4WithMask : APoolable<Ipv4WithMask>
        {
            public IPAddress IP;
            public IPAddress Mask;

            public Ipv4WithMask(AObjectPool<Ipv4WithMask> objectPool)
                : base(objectPool)
            {
            }

            protected override bool FinalizeBeforeReturn()
            {
                IP = null;
                Mask = null;
                return true;
            }

            public static implicit operator IPAddress(Ipv4WithMask ipv4WithMask)
            {
                if (ReferenceEquals(ipv4WithMask, null))
                    return null;

                return ipv4WithMask.IP;
            }

            public override string ToString()
            {
                return IP + StringConstants.SLASH + Mask;
            }
        }


    }
}