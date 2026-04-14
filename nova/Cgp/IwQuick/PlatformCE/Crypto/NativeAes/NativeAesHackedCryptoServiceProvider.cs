using System;
using System.Security.Cryptography;

namespace Contal.IwQuick.Crypto.NativeAes
{
    public class NativeAesHackedCryptoServiceProvider : NativeRijndael
    {
        internal abstract class CryptoTransform : AesCryptoTransform
        {
            protected readonly byte[] Key;
            protected readonly byte[] Iv;

            protected CryptoTransform(
                byte[] key,
                byte[] iv)
            {
                Key = key;
                Iv = iv;
            }

            public override void Dispose()
            {
            }

            public override int TransformBlock(
                byte[] inputBuffer,
                int inputOffset,
                int inputCount,
                byte[] outputBuffer,
                int outputOffset)
            {
                throw new NotImplementedException();
            }
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new NativeAesHackedEncryptor(rgbKey, rgbIV);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new NativeAesHackedDecryptor(rgbKey, rgbIV);
        }
    }
}
