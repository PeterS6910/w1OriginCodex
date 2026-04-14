using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto.NativeAes
{
    internal class NativeAesV2Decryptor : NativeAesV2CryptoServiceProvider.CryptoTransform
    {
        private byte[] _depadBuffer;
        private byte[] _outputBuffer;

        private bool _depadBufferPopulated;

        public NativeAesV2Decryptor(byte[] rgbKey, byte[] rgbIv)
            : base(rgbKey, rgbIv)
        {
            _depadBuffer = new byte[InputBlockSize];
        }

        public override int TransformBlock(
            byte[] inputBuffer, 
            int inputOffset, 
            int inputCount, 
            byte[] outputBuffer, 
            int outputOffset)
        {
            int result;

            if (inputCount < InputBlockSize)
            {
                if (inputCount > 0)
                    throw new ArgumentException();

                return 0;
            }

            int outputLength;

            if (_depadBufferPopulated)
            {
                outputLength = InputBlockSize;

                if (!NativeSuperAesV2.Decrypt(
                        Key,
                        false,
                        _depadBuffer, 0,
                        outputBuffer, outputOffset,
                        ref outputLength))
                    throw new CryptographicException();

                result = outputLength;
                outputOffset += result;
            }
            else
            {
                _depadBufferPopulated = true;
                result = 0;
            }

            Buffer.BlockCopy(
                inputBuffer, inputOffset + inputCount - InputBlockSize,
                _depadBuffer, 0,
                InputBlockSize);

            inputCount -= InputBlockSize;

            if (inputCount <= 0)
                return result;

            outputLength = inputCount;

            if (!NativeSuperAesV2.Decrypt(
                Key,
                false,
                inputBuffer, inputOffset,
                outputBuffer, outputOffset, 
                ref outputLength))
                throw new CryptographicException();

            return result + outputLength;
        }

        public override byte[] TransformFinalBlock(
            byte[] inputBuffer, 
            int inputOffset, 
            int inputCount)
        {
            int requestedOutputBufferLength = 
                inputCount + 
                (_depadBufferPopulated 
                    ? InputBlockSize
                    : 0);

            if (_outputBuffer == null || _outputBuffer.Length < requestedOutputBufferLength)
                _outputBuffer = new byte[requestedOutputBufferLength];

            if (_depadBufferPopulated)
            {
                if (_depadBuffer.Length < requestedOutputBufferLength)
                {
                    var newDepadBuffer = new byte[requestedOutputBufferLength];

                    Buffer.BlockCopy(
                        _depadBuffer,
                        0,
                        newDepadBuffer,
                        0,
                        InputBlockSize);

                    _depadBuffer = newDepadBuffer;
                }

                Buffer.BlockCopy(
                    inputBuffer,
                    inputOffset,
                    _depadBuffer,
                    InputBlockSize,
                    inputCount);

                inputBuffer = _depadBuffer;
                inputOffset = 0;

                inputCount += InputBlockSize;

                _depadBufferPopulated = false;
            }

            int outputLength = inputCount;

            if (!NativeSuperAesV2.Decrypt(
                    Key,
                    true,
                    inputBuffer, inputOffset, 
                    _outputBuffer, 0, 
                    ref outputLength))
                throw new CryptographicException(Marshal.GetLastWin32Error());

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