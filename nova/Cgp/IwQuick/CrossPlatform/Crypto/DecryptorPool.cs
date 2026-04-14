using System.Security.Cryptography;

namespace Contal.IwQuick.Crypto
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