using System;
using System.Security.Cryptography;

using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto.NativeAes
{
    internal class NativeAesDecryptor : NativeAesCryptoServiceProvider.CryptoTransform
    {
        private byte[] _depadBuffer;
        private byte[] _finalBlock;
        private byte[] _outputBuffer;

        public NativeAesDecryptor(byte[] rgbKey, byte[] rgbIv) 
            : base(rgbKey, rgbIv)
        {
        }

        private int Decrypt(
            byte[] inputBuffer, int inputOffset, int inputCount, 
            byte[] outputBuffer, int outputOffset)
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

            int numDecryptedBytes =
                NativeAES.NativeSuperAes_DecryptBlock(
                    ContextId,
                    inputBuffer, inputBuffer.Length, inputOffset, inputCount,
                    outputBuffer2, outputBuffer2.Length, out resultCode);

            if ((AESResult)resultCode != AESResult.Ok)
                throw new CryptographicException(resultCode);

            if (outputOffset > 0)
                Buffer.BlockCopy(
                    _outputBuffer, 0,
                    outputBuffer, outputOffset,
                    numDecryptedBytes);

            return numDecryptedBytes;
        }

        public override int TransformBlock(
            byte[] inputBuffer, 
            int inputOffset, 
            int inputCount, 
            byte[] outputBuffer, 
            int outputOffset)
        {
            int numDecryptedBytes = 0;

            if (inputCount < InputBlockSize)
            {
                if (inputCount > 0)
                    throw new ArgumentException();

                return 0;
            }

            if (_depadBuffer != null)
            {
                numDecryptedBytes =
                    Decrypt(
                        _depadBuffer, 0, _depadBuffer.Length,
                        outputBuffer, outputOffset);

                outputOffset += numDecryptedBytes;
            }
            else
                _depadBuffer = new byte[InputBlockSize];

            Buffer.BlockCopy(
                inputBuffer, inputOffset + inputCount - _depadBuffer.Length,
                _depadBuffer, 0,
                _depadBuffer.Length);

            inputCount -= _depadBuffer.Length;

            if (inputCount > 0)
                numDecryptedBytes +=
                    Decrypt(
                        inputBuffer, inputOffset, inputCount,
                        outputBuffer, outputOffset);

            return numDecryptedBytes;
        }

        public override byte[] TransformFinalBlock(
            byte[] inputBuffer, 
            int inputOffset, 
            int inputCount)
        {
            int requestedOutputBufferLength =
                inputCount +
                (_depadBuffer != null
                    ? InputBlockSize
                    : 0);

            if (_outputBuffer == null || _outputBuffer.Length < requestedOutputBufferLength)
                _outputBuffer = new byte[requestedOutputBufferLength];

            int resultCode;

            if (_depadBuffer != null)
            {
                if (_finalBlock == null || _finalBlock.Length < requestedOutputBufferLength)
                    _finalBlock = new byte[requestedOutputBufferLength];

                Buffer.BlockCopy(
                    _depadBuffer,
                    0,
                    _finalBlock,
                    0,
                    InputBlockSize);

                Buffer.BlockCopy(
                    inputBuffer,
                    inputOffset,
                    _finalBlock,
                    InputBlockSize,
                    inputCount);

                inputBuffer = _finalBlock;
                inputOffset = 0;
                inputCount += InputBlockSize;

                _depadBuffer = null;
            }

            int outputLength = 
                NativeAES.NativeSuperAes_DecryptLastBlock(
                    ContextId,
                    inputBuffer, inputBuffer.Length, inputOffset, inputCount,
                    _outputBuffer, _outputBuffer.Length, out resultCode);

            if ((AESResult)resultCode != AESResult.Ok)
                throw new CryptographicException(resultCode);

            NativeAES.NativeSuperAes_ResetKey(ContextId, out resultCode);

            if ((AESResult)resultCode != AESResult.Ok)
                throw new CryptographicException(resultCode);

            byte[] result;

            if (outputLength == _outputBuffer.Length)
            {
                result = _outputBuffer;
                _outputBuffer = null;

                return result;
            }

            result = new byte[outputLength];

            Buffer.BlockCopy(
                _outputBuffer, 0,
                result, 0,
                outputLength);

            return result;
        }
    }
}