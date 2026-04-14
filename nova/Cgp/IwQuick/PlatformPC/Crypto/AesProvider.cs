using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using Contal.IwQuick.Crypto.Microsoft;
using JetBrains.Annotations;

namespace Contal.IwQuick.Crypto
{
    public class AesProvider : Aes
    {
        private readonly SafeCspHandle _capiProvider;

        private SafeCapiKeyHandle _capiKey;

        internal abstract class AesTransform : ICryptoTransform
        {
            protected const int BlockSize = 16;
            protected readonly SafeCapiKeyHandle CapiKey;

            protected AesTransform(AesProvider aesProvider) 
                : this(
                    aesProvider, 
                    aesProvider._capiKey.Duplicate(), 
                    aesProvider.IV)
            {
                
            }

            protected AesTransform(
                AesProvider aesProvider, 
                SafeCapiKeyHandle safeCapiKeyHandle,
                byte[] iv)
            {
                CapiKey = safeCapiKeyHandle;

                CapiNative.SetKeyParameter(
                    CapiKey,
                    CapiNative.KeyParameter.Mode,
                    (int)aesProvider.Mode);

                if (aesProvider.Mode == CipherMode.ECB) 
                    return;

                CapiNative.SetKeyParameter(
                    CapiKey,
                    CapiNative.KeyParameter.IV,
                    iv);

                if (
                        aesProvider.Mode == CipherMode.CFB ||
                        aesProvider.Mode == CipherMode.OFB)
                    CapiNative.SetKeyParameter(
                        CapiKey,
                        CapiNative.KeyParameter.ModeBits,
                        aesProvider.FeedbackSize);
            }

            public void Dispose()
            {
                CapiKey.Dispose();
            }

            public abstract int TransformBlock(
                byte[] inputBuffer,
                int inputOffset,
                int inputCount,
                byte[] outputBuffer,
                int outputOffset);

            public abstract byte[] TransformFinalBlock(
                byte[] inputBuffer,
                int inputOffset,
                int inputCount);

            public int InputBlockSize
            {
                get { return 16; }
            }

            public int OutputBlockSize
            {
                get { return 16; }
            }

            public bool CanTransformMultipleBlocks
            {
                get { return true; }
            }

            public bool CanReuseTransform
            {
                get { return true; }
            }
        }

        public AesProvider()
        {
            _capiProvider = 
                CapiNative.AcquireCsp(
                    null, 
                    Environment.OSVersion.Version.Major != 5 || Environment.OSVersion.Version.Minor != 1
                        ? "Microsoft Enhanced RSA and AES Cryptographic Provider"
                        : "Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)", 
                    CapiNative.ProviderType.RsaAes, 
                    CapiNative.CryptAcquireContextFlags.VerifyContext, 
                    true);
        }

        private SafeCapiKeyHandle CreateKey([NotNull] byte[] key)
        {
            var keyLength = key.Length;

            CapiNative.AlgorithmId algorithmId =
                GetAlgorithmId(keyLength * 8);

            if (algorithmId == CapiNative.AlgorithmId.None)
                throw new CryptographicException(
                    keyLength + "is invalid key length for this AES algorithm support. Valid lengths are 16,24 or 32 bytes.");

            return
                CapiNative.ImportSymmetricKey(
                    _capiProvider,
                    algorithmId,
                    key);
        }

        /// <summary>
        /// 
        /// </summary>
        public override byte[] Key
        {
            [SecurityCritical]
            get { return base.Key; }

            [SecurityCritical]
            set
            {
                SafeCapiKeyHandle newCapiKey = CreateKey(value);

                if (_capiKey != null)
                    _capiKey.Dispose();

                base.Key = value;
                _capiKey = newCapiKey;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ICryptoTransform CreateEncryptor()
        {
            return new AesEncryptor(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIv"></param>
        /// <returns></returns>
        public override ICryptoTransform CreateEncryptor(
            byte[] rgbKey, 
            byte[] rgbIv)
        {
            return 
                new AesEncryptor(
                    this, 
                    CreateKey(rgbKey), 
                    rgbIv);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        private static CapiNative.AlgorithmId GetAlgorithmId(int blockSize)
        {
            switch (blockSize)
            {
                case 128:
                    return CapiNative.AlgorithmId.Aes128;

                case 192:
                    return CapiNative.AlgorithmId.Aes192;

                case 256:
                    return CapiNative.AlgorithmId.Aes256;

                default:
                    return CapiNative.AlgorithmId.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIv"></param>
        /// <returns></returns>
        public override ICryptoTransform CreateDecryptor(
            byte[] rgbKey, 
            byte[] rgbIv)
        {
            return 
                new AesDecryptor(
                    this,
                    CreateKey(rgbKey),
                    rgbIv);
        }

        [SecurityCritical]
        public override void GenerateKey()
        {
            SafeCapiKeyHandle safeCapiKeyHandle = null;
            CapiNative.AlgorithmId algorithmId = 
                GetAlgorithmId(KeySize);

            if (algorithmId == CapiNative.AlgorithmId.None)
                throw new CryptographicException("Invalid block size");

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                if (
                    !CapiNative.UnsafeNativeMethods.CryptGenKey(
                        _capiProvider,
                        algorithmId,
                        CapiNative.KeyFlags.Exportable,
                        out safeCapiKeyHandle))
                    throw new CryptographicException(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (
                    safeCapiKeyHandle != null && 
                    !safeCapiKeyHandle.IsInvalid)
                    safeCapiKeyHandle.SetParentCsp(_capiProvider);
            }

            if (_capiKey != null)
                _capiKey.Dispose();

            _capiKey = safeCapiKeyHandle;
            KeyValue = CapiNative.ExportSymmetricKey(_capiKey);
        }

        [SecurityCritical]
        public override void GenerateIV()
        {
            var array = new byte[BlockSizeValue / 8];

            if (
                    !CapiNative.UnsafeNativeMethods.CryptGenRandom(
                        _capiProvider,
                        array.Length,
                        array))
                throw new CryptographicException(Marshal.GetLastWin32Error());

            IV = array;
        }

        public override ICryptoTransform CreateDecryptor()
        {
            return new AesDecryptor(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing) 
                return;

            if (_capiKey != null)
                _capiKey.Dispose();

            if (_capiProvider != null)
                _capiProvider.Dispose();
        }
    }
}
