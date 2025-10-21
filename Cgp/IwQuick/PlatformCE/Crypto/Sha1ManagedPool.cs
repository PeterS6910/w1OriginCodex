using System.Security.Cryptography;

namespace Contal.IwQuick.Crypto
{
    internal class Sha1ManagedPool : AObjectPool<SHA1Managed>
    {
        protected override SHA1Managed CreateObject()
        {
            return new SHA1Managed();
        }

        private Sha1ManagedPool()
        {
            
        }

        public static readonly Sha1ManagedPool Singleton = new Sha1ManagedPool();
    }
}