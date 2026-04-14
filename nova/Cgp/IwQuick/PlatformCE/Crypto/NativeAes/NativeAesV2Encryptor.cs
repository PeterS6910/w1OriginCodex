using System;
using System.Security.Cryptography;

using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto.NativeAes
{
    internal class NativeAesV2Encryptor : NativeAesV2CryptoServiceProvider.CryptoTransform
    {
        private byte[] _outputBuffer;

        public NativeAesV2Encryptor(byte[] rgbKey, byte[] rgbIv)
            : base(rgbKey, rgbIv)
        {
        }

        public override int TransformBlock(
            byte[] inputBuffer, 
            int inputOffset, 
            int inputCount, 
            byte[] outputBuffer, 
            int outputOffset)
        {
            Buffer.BlockCopy(
                inputBuffer, inputOffset, 
                outputBuffer, outputOffset, 
                inputCount);

            int outputLength = inputCount;

            if (!NativeSuperAesV2.Encrypt(
                    Key,
                    false,
                    outputBuffer, outputOffset, ref outputLength, 
                    outputBuffer.Length))
                throw new CryptographicException();

            return outputLength;
        }

        public override byte[] TransformFinalBlock(
            byte[] inputBuffer, 
            int inputOffset, 
            int inputCount)
        {
            int requestedOutputBufferLength = 
                ((inputCount + 1) / InputBlockSize + 1) * OutputBlockSize;

            if (_outputBuffer == null || _outputBuffer.Length < requestedOutputBufferLength)
                _outputBuffer = new byte[requestedOutputBufferLength];

            Buffer.BlockCopy(
                inputBuffer, inputOffset,
                _outputBuffer, 0,
                inputCount);

            int outputLength = inputCount;

            if (!NativeSuperAesV2.Encrypt(
                    Key,
                    true,
                    _outputBuffer, 0, ref outputLength, 
                    _outputBuffer.Length))
                throw new CryptographicException();

            byte[] result;

            if (outputLength == _outputBuffer.Length)
            {
                result = _outputBuffer;
                _outputBuffer = null;
            }
            else
            {
                result = new byte[outputLength];
                Buffer.BlockCopy(_outputBuffer, 0, result, 0, outputLength);
            }

            return result;
        }
    }
}