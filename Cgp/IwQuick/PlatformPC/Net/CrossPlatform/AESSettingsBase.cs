using System;
using System.Security.Cryptography;
using System.Text;

#if !COMPACT_FRAMEWORK
using Contal.IwQuick.Data;
using Contal.IwQuick.Crypto;
#else
using Contal.IwQuickCF.Data;
using Contal.IwQuickCF.Crypto;
#endif

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Net
#else
namespace Contal.IwQuickCF.Net
#endif
{
    public enum AESKeySize
    {
        Size128 = 128,
        Size192 = 192,
        Size256 = 256
    }

    /// <summary>
    /// Class for sttings and prepare stream with symetric encryption
    /// </summary>
    public abstract class AESSettingsBase : IAESSettings
    {
        public const int BLOCK_SIZE = 128;

        /// security precaution, do not store security entity here
        /*private byte[] _password;
        private byte[] _salt;*/

        private AESKeySize _aesKeySize;
        public AESKeySize AesKeySize
        {
            get { return _aesKeySize; }
        }

        private PasswordDeriveBytesSHA1 _derivedBytes = null;

        public AESSettingsBase(byte[] key, byte[] iv, AESKeySize keySize)
        {
            _aesKeySize = keySize;

            if ((iv.Length * 8) != BLOCK_SIZE)
                throw new ArgumentException();

            _aesIV = new byte[BLOCK_SIZE / 8];
            Array.Copy(iv, _aesIV, BLOCK_SIZE / 8);

            switch (keySize)
            {
                case AESKeySize.Size128:
                    if (key.Length != 16) 
                        throw new ArgumentException();

                    _aes128key = new byte[16];
                    Array.Copy(key, _aes128key, 16);

                    break;

                case AESKeySize.Size192:
                    if (key.Length != 24)
                        throw new ArgumentException();

                    _aes192key = new byte[24];
                    Array.Copy(key, _aes192key, 24);

                    break;

                case AESKeySize.Size256:
                    if (key.Length != 32)
                        throw new ArgumentException();

                    _aes256key = new byte[32];
                    Array.Copy(key, _aes256key, 32);
                    
                    break;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Key in array of bytes</param>
        /// <param name="salt">IV in array of bytes</param>
        public AESSettingsBase(byte[] password, byte[] salt)
        {
            Validator.CheckNull(password, salt);

            PrepareDerivedBytes(password,salt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Key in string</param>
        /// <param name="salt">IV in string</param>
        public AESSettingsBase(string password, string salt)
        {
            Validator.CheckNull(password, salt);

            PrepareDerivedBytes(Encoding.UTF8.GetBytes(password),Encoding.UTF8.GetBytes(salt));
        }

        public AESSettingsBase(string password, byte[] salt)
        {
            Validator.CheckNull(password, salt);

            PrepareDerivedBytes(Encoding.UTF8.GetBytes(password), salt);
        }

        public byte[] GetBytes(int cb)
        {
            if (cb <=0)
                return new byte[0];

            return _derivedBytes.GetBytes(cb);
        }

        private byte[] _aes128key;
        public byte[] Aes128Key
        {
            get
            {
                return _aes128key;
            }
        }

        private byte[] _aes192key;
        public byte[] Aes192Key
        {
            get
            {
                return _aes192key;
            }
        }

        private byte[] _aesIV;
        public byte[] AesIV
        {
            get
            {
                return _aesIV;
            }
        }

        private byte[] _aes256key;
        /// <summary>
        /// derived byte key for AES 256bit
        /// </summary>
        public byte[] Aes256Key
        {
            get
            {
                if (null == _aes256key)
                    _aes256key = _derivedBytes.GetBytes(32);

                return _aes256key;
            }
        }

        private byte[] _aes256IV;
        /// <summary>
        /// derived byte initialization vector for AES 256bit
        /// </summary>
        public byte[] Aes256IV
        {
            get
            {
                if (null == _aes256IV)
                    _aes256IV = _derivedBytes.GetBytes(16);

                return _aes256IV;
            }
        }


        /// <summary>
        /// Get AESLayer
        /// </summary>
        /// <returns>Return new AESLayer</returns>
        public ITransportLayer GetLayer()
        {
            SymmetricAlgorithm algorithm = CreateAlgorithm();

            switch (AesKeySize)
            {
                case AESKeySize.Size128:
                    algorithm.Key = Aes128Key;
                    algorithm.IV = AesIV;
                    break;

                case AESKeySize.Size192:
                    algorithm.Key = Aes192Key;
                    algorithm.IV = Aes256IV;
                    break;

                case AESKeySize.Size256:
                    algorithm.Key = Aes256Key;
                    algorithm.IV = Aes256IV;
                    break;
            }

            return new AESLayer(algorithm, UseAesStream2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract SymmetricAlgorithm CreateAlgorithm();
        /// <summary>
        /// 
        /// </summary>
        protected abstract bool UseAesStream2 { get; }

        private void PrepareDerivedBytes(byte[] password,byte[] salt)
        {
            Validator.CheckNull(password, salt);

            if (null == _derivedBytes)
            {
                _derivedBytes = new PasswordDeriveBytesSHA1(password, salt);
                _derivedBytes.IterationCount = 1000;
            }
        }

        public override string ToString()
        {
            return ByteDataCarrier.HexDump(Aes256Key) + " " + ByteDataCarrier.HexDump(Aes256IV);
        }
    }
}
