using System;
using System.Security.Cryptography;

using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto.NativeAes
{
    internal class NativeAesHackedDecryptor : NativeAesHackedCryptoServiceProvider.CryptoTransform
    {
        private byte[] _outputBuffer;

        public NativeAesHackedDecryptor(
            byte[] key, 
            byte[] iv) 
            : base(key, iv)
        {
        }

        public override byte[] TransformFinalBlock(
            byte[] inputBuffer, 
            int inputOffset, 
            int inputCount)
        {
            int resultCode;

            if (_outputBuffer == null || _outputBuffer.Length < inputCount)
                _outputBuffer = new byte[inputCount];

            int numDecryptedBytesFromInput =
                NativeAES.NativeSuperAes_Decrypt(
                    Key, Key.Length, Iv, Iv.Length,
                    inputBuffer, inputBuffer.Length, inputOffset, inputCount,
                    _outputBuffer, _outputBuffer.Length, out resultCode);

            if ((AESResult)resultCode != AESResult.Ok)
                throw new CryptographicException(resultCode);

            byte[] result;

            if (numDecryptedBytesFromInput == _outputBuffer.Length)
            {
                result = _outputBuffer;
                _outputBuffer = null;

                return result;
            }

            result = new byte[numDecryptedBytesFromInput];

            Buffer.BlockCopy(
                _outputBuffer, 0,
                result, 0,
                numDecryptedBytesFromInput);

            return result;
        }
    }
}