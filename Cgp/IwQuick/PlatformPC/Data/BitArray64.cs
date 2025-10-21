using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// Represents binary array stored as boolean array with length of 64
    /// </summary>
    public class BitArray64:BitArray
    {
        const byte BITARRAY_LENGTH_64 = 64;

        public BitArray64()
            : base(BITARRAY_LENGTH_64, true)
        {
        }

        public BitArray64(byte byteValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(byteValue));
        }

        public BitArray64(sbyte sbyteValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(FromSbyte(sbyteValue));
        }

        public BitArray64(char charValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(charValue));
        }

        public BitArray64(short shortValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(shortValue));
        }

        public BitArray64(ushort ushortValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ushortValue));
        }

        public BitArray64(int intValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(intValue));
        }

        public BitArray64(uint uintValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(uintValue));
        }

        public BitArray64(long longValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(longValue));
        }

        public BitArray64(ulong ulongValue)
            : base(BITARRAY_LENGTH_64, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ulongValue));
        }
    }
}
