using System.Security.Cryptography;

namespace Contal.IwQuick.Crypto
{
    internal class EncryptorPool : CryptoTranformPool
    {
        public EncryptorPool(SymmetricAlgorithm algorithm)
            : base(algorithm)
        {

        }

        protected override ICryptoTransform CreateObject()
        {
            return Algorithm.CreateEncryptor();
        }
    }
}