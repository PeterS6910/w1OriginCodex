using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto
{
    internal sealed class AesDecryptor : AesProvider.AesTransform
    {
        private byte[] _depadBuffer;
        private byte[] _outputBuffer;

        public AesDecryptor(
            AesProvider aesProvider,
            SafeCapiKeyHandle safeCapiKeyHandle,
            byte[] iv)
            : base(aesProvider, safeCapiKeyHandle, iv)
        {
        }

        public AesDecryptor(
            AesProvider aesProvider)
            : base(aesProvider)
        {
        }

        private int PrepareDecrypt(
            byte[] inputBuffer,
            int inputOffset,
            int inputCount,
            byte[] outputBuffer,
            int outputOffset,
            bool final)
        {
            int outputLength = 0;

            if (_depadBuffer != null)
            {
                Buffer.BlockCopy(
                    _depadBuffer,
                    0,
                    outputBuffer,
                    outputOffset,
                    _depadBuffer.Length);

                outputLength += _depadBuffer.Length;
                outputOffset += _depadBuffer.Length;
            }

            if (!final)
            {
                if (_depadBuffer == null)
                    _depadBuffer = new byte[BlockSize];

                inputCount -= _depadBuffer.Length;

                Buffer.BlockCopy(
                    inputBuffer,
                    inputOffset + inputCount,
                    _depadBuffer,
                    0,
                    _depadBuffer.Length);
            }

            Buffer.BlockCopy(
                inputBuffer,
                inputOffset,
                outputBuffer,
                outputOffset,
                inputCount);

            outputLength += inputCount;
            return outputLength;
        }

        public override int TransformBlock(
            byte[] inputBuffer,
            int inputOffset,
            int inputCount,
            byte[] outputBuffer,
            int outputOffset)
        {
            int outputLength =
                PrepareDecrypt(
                    inputBuffer,
                    inputOffset,
                    inputCount,
                    outputBuffer,
                    outputOffset,
                    false);

            GCHandle outputBufferHandle =
                GCHandle.Alloc(
                    outputBuffer,
                    GCHandleType.Pinned);

            try
            {
                IntPtr outputBufferPtr =
                    IntPtr.Size == 4
                        ? new IntPtr(outputBufferHandle.AddrOfPinnedObject().ToInt32() + outputOffset)
                        : new IntPtr(outputBufferHandle.AddrOfPinnedObject().ToInt64() + outputOffset);

                CapiNative.UnsafeNativeMethods.CryptDecrypt(
                    CapiKey,
                    SafeCapiHashHandle.InvalidHandle,
                    false,
                    0,
                    outputBufferPtr,
                    ref outputLength);
            }
            finally
            {
                outputBufferHandle.Free();
            }

            return outputLength;
        }

        public override byte[] TransformFinalBlock(
            byte[] inputBuffer,
            int inputOffset,
            int inputCount)
        {
            int requestedOutputBufferLength =
                inputBuffer.Length +
                (_depadBuffer != null
                    ? _depadBuffer.Length
                    : 0);

            if (
                    _outputBuffer == null || 
                    _outputBuffer.Length < requestedOutputBufferLength)
                _outputBuffer =
                    new byte[requestedOutputBufferLength];

            int outputLength =
                PrepareDecrypt(
                    inputBuffer,
                    inputOffset,
                    inputCount,
                    _outputBuffer,
                    0,
                    true);

            GCHandle outputBufferHandle =
                GCHandle.Alloc(
                    _outputBuffer,
                    GCHandleType.Pinned);

            try
            {
                IntPtr outputBufferPtr = outputBufferHandle.AddrOfPinnedObject();

                if (
                        !CapiNative.UnsafeNativeMethods.CryptDecrypt(
                            CapiKey,
                            SafeCapiHashHandle.InvalidHandle,
                            true,
                            0,
                            outputBufferPtr,
                            ref outputLength))
                    throw new CryptographicException(Marshal.GetLastWin32Error());
            }
            finally
            {
                outputBufferHandle.Free();
            }

            _depadBuffer = null;

            if (outputLength > _outputBuffer.Length)
                throw new CryptographicException("Wrong size of final output buffer");

            byte[] result;

            if (outputLength == _outputBuffer.Length)
            {
                result = _outputBuffer;
                _outputBuffer = null;
            }
            else
            {
                result = new byte[outputLength];

                Buffer.BlockCopy(
                    _outputBuffer, 0, 
                    result, 0, 
                    outputLength);
            }

            return result;
        }
    }
}