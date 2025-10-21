using System.Security.Cryptography;

using Contal.IwQuick.Crypto.NativeAes;

namespace Contal.IwQuick.Net
{
    public class AESSettings : AESSettingsBase
    {
        private readonly bool _newImage;

        public AESSettings(bool newImage, byte[] key, byte[] iv, AESKeySize aesKeySize) 
            : base(key, iv, aesKeySize)
        {
            _newImage = newImage;
        }

        protected override SymmetricAlgorithm CreateAlgorithm()
        {
            return
                _newImage
                    ? (SymmetricAlgorithm)new NativeAesV2CryptoServiceProvider()
                    : new NativeAesHackedCryptoServiceProvider();
        }

        protected override bool UseAesStream2
        {
            get
            {
                return _newImage;
            }
        }
    }
}
