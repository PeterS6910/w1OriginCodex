using System.Security.Cryptography;

using Contal.IwQuick.Crypto;

namespace Contal.IwQuick.Net
{
    public class AESSettings : AESSettingsBase
    {
        public AESSettings(byte[] password, byte[] salt)
            : base(password, salt)
        {}

        public AESSettings(byte[] key, byte[] iv, AESKeySize aesKeySize)
            : base(key, iv, aesKeySize)
        {}

        protected override SymmetricAlgorithm CreateAlgorithm()
        {
            return new AesProvider();
        }

        protected override bool UseAesStream2
        {
            get { return true; }
        }
    }
}
