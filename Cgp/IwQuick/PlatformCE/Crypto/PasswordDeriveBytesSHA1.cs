using System;
using System.Diagnostics;
using System.Text;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.Crypto
{
    /// <summary>
    /// 
    /// </summary>
    public class PasswordDeriveBytesSHA1
    {
        private byte[] _pseudoPwdBytes = null;

        private int _iterationCount = 100; // do not change this default value, can influence encrypt/decrypt processes
        /// <summary>
        /// count of iteration to repeat the hash
        /// </summary>
        public int IterationCount
        {
            get
            {
                return _iterationCount;
            }
            set
            {
                if (value > 0)
                    _iterationCount = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainPasswordBytes"></param>
        /// <param name="rgbSalt"></param>
        public PasswordDeriveBytesSHA1(
            [NotNull] byte[] plainPasswordBytes, 
            [NotNull] byte[] rgbSalt)
        {
            Validator.CheckForNull(plainPasswordBytes, "plainPasswordBytes");
            Validator.CheckForNull(rgbSalt, "rgbSalt");

            GeneratePseudoPasswordBytes(plainPasswordBytes, rgbSalt);
        }

        private void GeneratePseudoPasswordBytes(byte[] plainPasswordBytes, byte[] rgbSalt)
        {
            _pseudoPwdBytes = new byte[plainPasswordBytes.Length + rgbSalt.Length];

            plainPasswordBytes.CopyTo(_pseudoPwdBytes, 0);
            rgbSalt.CopyTo(_pseudoPwdBytes, plainPasswordBytes.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainPassword"></param>
        /// <param name="rgbSalt"></param>
        public PasswordDeriveBytesSHA1(
            [NotNull] string plainPassword, 
            [NotNull] byte[] rgbSalt)
        {
            Validator.CheckForNull(plainPassword,"plainPassword");
            Validator.CheckForNull(rgbSalt,"rgbSalt");

            ByteDataCarrier.StringToBytes(
                plainPassword,
                Encoding.ASCII, 
                (stringBytes, size) => GeneratePseudoPasswordBytes(stringBytes, rgbSalt));
    
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnedBufferSize"></param>
        /// <returns></returns>
        public byte[] GetBytes(int returnedBufferSize)
        {
            Validator.CheckZero(returnedBufferSize);

            var sha1 = Sha1ManagedPool.Singleton.Get();

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var destinationArray = new byte[returnedBufferSize];
                int destinationIndex = 0;
                while (destinationIndex < returnedBufferSize)
                {
                    for (int i = 0; i < _iterationCount; i++)
                    {
                        _pseudoPwdBytes = sha1.ComputeHash(_pseudoPwdBytes);
                    }
                    int length = returnedBufferSize - destinationIndex;
                    length = (length < _pseudoPwdBytes.Length) ? length : _pseudoPwdBytes.Length;
                    Array.Copy(_pseudoPwdBytes, 0, destinationArray, destinationIndex, length);
                    destinationIndex += length;
                }

                sw.Stop();

                DebugHelper.NOP(sw.ElapsedMilliseconds);
                return destinationArray;
            }
            finally
            {
                Sha1ManagedPool.Singleton.Return(sha1);
            }
        }
        
    }
}
