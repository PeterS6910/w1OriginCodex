using System.Security.Cryptography;

#if COMPACT_FRAMEWORK
namespace Contal.IwQuickCF.Crypto
#else
namespace Contal.IwQuick.Crypto
#endif
{
    internal class DecryptorPool : CryptoTranformPool
    {
        public DecryptorPool(SymmetricAlgorithm algorithm)
            : base(algorithm)
        {

        }

        protected override ICryptoTransform CreateObject()
        {
            return Algorithm.CreateDecryptor();
        }
    }
}