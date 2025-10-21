using System;
using System.Text;

using System.Security.Cryptography;
using JetBrains.Annotations;

namespace Contal.IwQuick.Crypto
{
    /// <summary>
    /// re-implementation of the PasswordDerivedBytes from big .NET because of lacking implementation in .NET Compact
    /// </summary>
    public class PasswordDeriveBytesSHA1
    {
        private byte[] _pseudoPwdBytes = null;

        private int _iterationCount = 100;
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

        public PasswordDeriveBytesSHA1(
            [NotNull] byte[] plainPasswordBytes, 
            [NotNull] byte[] rgbSalt)
        {
            Validator.CheckNullAndEmpty(plainPasswordBytes, "plainPasswordBytes");
            Validator.CheckNullAndEmpty(rgbSalt,"rgbSalt");

            _pseudoPwdBytes = new byte[plainPasswordBytes.Length + rgbSalt.Length];

            plainPasswordBytes.CopyTo(_pseudoPwdBytes, 0);
            rgbSalt.CopyTo(_pseudoPwdBytes, plainPasswordBytes.Length);
        }

        public PasswordDeriveBytesSHA1([NotNull] string plainPassword, [NotNull] byte[] rgbSalt)
        {
            Validator.CheckForNull(plainPassword, "plainPassword");
            Validator.CheckNullAndEmpty(rgbSalt, "rgbSalt");

            byte[] plainBytes = Encoding.ASCII.GetBytes(plainPassword);

            _pseudoPwdBytes = new byte[plainBytes.Length + rgbSalt.Length];

            plainBytes.CopyTo(_pseudoPwdBytes, 0);
            rgbSalt.CopyTo(_pseudoPwdBytes, plainBytes.Length);
        }

        public byte[] GetBytes(int cb)
        {
            Validator.CheckZero(cb);

            SHA1Managed sha1 = new SHA1Managed();

            byte[] destinationArray = new byte[cb];
            int destinationIndex = 0;
            while (destinationIndex < cb)
            {
                for (int i = 0; i < _iterationCount; i++)
                {
                    _pseudoPwdBytes = sha1.ComputeHash(_pseudoPwdBytes);
                }
                int length = cb - destinationIndex;
                length = (length < _pseudoPwdBytes.Length) ? length : _pseudoPwdBytes.Length;
                Array.Copy(_pseudoPwdBytes, 0, destinationArray, destinationIndex, length);
                destinationIndex += length;
            }
            return destinationArray;
        }
        
    }
}
