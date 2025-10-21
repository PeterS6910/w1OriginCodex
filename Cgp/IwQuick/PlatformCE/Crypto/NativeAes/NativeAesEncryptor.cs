using System;
using System.Security.Cryptography;

using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto.NativeAes
{
    internal class NativeAesEncryptor : NativeAesCryptoServiceProvider.CryptoTransform
    {
        private byte[] _outputBuffer;

        public NativeAesEncryptor(byte[] rgbKey, byte[] rgbIv) 
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
            byte[] outputBuffer2;

            if (outputOffset > 0)
            {
                int requestedOutputBufferLength = outputBuffer.Length - outputOffset;

                if (_outputBuffer == null || _outputBuffer.Length < requestedOutputBufferLength)
                    _outputBuffer = new byte[requestedOutputBufferLength];

                outputBuffer2 = _outputBuffer;
            }
            else
                outputBuffer2 = outputBuffer;

            int resultCode;

            int numEncryptedBytes =
                NativeAES.NativeSuperAes_EncryptBlock(
                    ContextId,
                    inputBuffer, inputBuffer.Length, inputOffset, inputCount,
                    outputBuffer2, outputBuffer2.Length, out resultCode);

            if ((AESResult)resultCode != AESResult.Ok)
                throw new CryptographicException(resultCode);

            if (outputOffset > 0)
                Buffer.BlockCopy(
                    _outputBuffer, 0,
                    outputBuffer, outputOffset,
                    numEncryptedBytes);

            return numEncryptedBytes;
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
                NativeAES.NativeSuperAes_EncryptLastBlock(
                    ContextId,
                    inputBuffer, inputBuffer.Length, inputOffset, inputCount,
                    _outputBuffer, _outputBuffer.Length, out resultCode);

            if ((AESResult)resultCode != AESResult.Ok)
                throw new CryptographicException(resultCode);

            NativeAES.NativeSuperAes_ResetKey(ContextId, out resultCode);

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