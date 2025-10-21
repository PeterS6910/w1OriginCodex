using System;
using System.Security.Cryptography;

using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto.NativeAes
{
    internal class NativeAesHackedEncryptor : NativeAesHackedCryptoServiceProvider.CryptoTransform
    {
        private byte[] _outputBuffer;

        public NativeAesHackedEncryptor(byte[] key, byte[] iv) : 
            base(key, iv)
        {
        }

        public override byte[] TransformFinalBlock(
            byte[] inputBuffer, 
            int inputOffset, 
            int inputCount)
        {
            int resultCode;

            int requestedOutputBufferLength =
                ((inputCount + 1) / InputBlockSize + 1) * OutputBlockSize;

            if (_outputBuffer == null || _outputBuffer.Length < requestedOutputBufferLength)
                _outputBuffer = new byte[requestedOutputBufferLength];

            int numEncryptedBytes =
                NativeAES.NativeSuperAes_Encrypt(
                    Key, Key.Length, Iv, Iv.Length,
                    inputBuffer, inputBuffer.Length, inputOffset, inputCount,
                    _outputBuffer, _outputBuffer.Length, out resultCode);

            if ((AESResult)resultCode != AESResult.Ok)
                throw new CryptographicException(resultCode);

            byte[] result;

            if (numEncryptedBytes == _outputBuffer.Length)
            {
                result = _outputBuffer;
                _outputBuffer = null;
            }
            else
            {
                result = new byte[numEncryptedBytes];
                Buffer.BlockCopy(_outputBuffer, 0, result, 0, numEncryptedBytes);
            }

            return result;
        }
    }
}