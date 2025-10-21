using System;
using System.Security.Cryptography;

using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto.NativeAes
{
    public class NativeAesV2CryptoServiceProvider : NativeRijndael
    {
        internal abstract class CryptoTransform : AesCryptoTransform
        {
            protected IntPtr Key;

            protected CryptoTransform(byte[] rgbKey, byte[] rgbIv)
            {
                Key = NativeSuperAesV2.CreateKey(rgbKey, rgbKey.Length, rgbIv);
            }

            public override void Dispose()
            {
                NativeSuperAesV2.ReleaseKey(Key);
            }
        }

        public override ICryptoTransform CreateEncryptor(
            byte[] rgbKey, byte[] rgbIV)
        {
            return new NativeAesV2Encryptor(rgbKey, rgbIV);
        }

        public override ICryptoTransform CreateDecryptor(
            byte[] rgbKey, byte[] rgbIV)
        {
            return new NativeAesV2Decryptor(rgbKey, rgbIV);
        }
    }
}