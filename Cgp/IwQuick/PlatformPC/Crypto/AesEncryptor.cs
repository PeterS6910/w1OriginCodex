using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Contal.IwQuick.Crypto.Microsoft;

namespace Contal.IwQuick.Crypto
{
    internal sealed class AesEncryptor : AesProvider.AesTransform
    {
        private byte[] _outputBuffer;

        public AesEncryptor(
            AesProvider aesProvider,
            SafeCapiKeyHandle safeCapiKeyHandle,
            byte[] iv)
            : base(aesProvider, safeCapiKeyHandle, iv)
        {
        }

        public AesEncryptor(
            AesProvider aesProvider)
            : base(aesProvider)
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
                inputBuffer,
                inputOffset,
                outputBuffer,
                outputOffset,
                inputCount);

            int resultLength = inputCount;

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

                if (
                    !CapiNative.UnsafeNativeMethods.CryptEncrypt(
                        CapiKey,
                        SafeCapiHashHandle.InvalidHandle,
                        false,
                        0,
                        outputBufferPtr,
                        ref resultLength,
                        outputBuffer.Length - outputOffset))
                    throw new CryptographicException(Marshal.GetLastWin32Error());
            }
            finally
            {
                outputBufferHandle.Free();
            }

            return resultLength;
        }

        public override byte[] TransformFinalBlock(
            byte[] inputBuffer,
            int inputOffset,
            int inputCount)
        {
            int requestedOutputBufferLength = inputBuffer.Length + BlockSize;

            if (
                    _outputBuffer == null || 
                    _outputBuffer.Length < requestedOutputBufferLength)
                _outputBuffer = new byte[requestedOutputBufferLength];

            Buffer.BlockCopy(
                inputBuffer,
                inputOffset,
                _outputBuffer,
                0,
                inputCount);

            int outputLength = inputCount;

            GCHandle outputBufferHandle = 
                GCHandle.Alloc(
                    _outputBuffer, 
                    GCHandleType.Pinned);

            try
            {
                IntPtr outputBufferPtr = outputBufferHandle.AddrOfPinnedObject();

                if (!
                    CapiNative.UnsafeNativeMethods.CryptEncrypt(
                        CapiKey,
                        SafeCapiHashHandle.InvalidHandle,
                        true,
                        0,
                        outputBufferPtr,
                        ref outputLength,
                        _outputBuffer.Length))
                    throw new CryptographicException(Marshal.GetLastWin32Error());
            }
            finally
            {
                outputBufferHandle.Free();
            }

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
                Buffer.BlockCopy(_outputBuffer, 0, result, 0, outputLength);
            }

            return result;
        }
    }
}