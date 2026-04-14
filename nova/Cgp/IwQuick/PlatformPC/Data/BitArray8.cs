using System;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// Represents binary array stored as boolean array with length of 8
    /// </summary>
    public class BitArray8 : BitArray
    {
        const byte BITARRAY_LENGTH_8 = 8;

        public BitArray8()
            : base(BITARRAY_LENGTH_8, true)
        {
        }

        public BitArray8(byte byteValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(byteValue));
        }

        public BitArray8(sbyte sbyteValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(FromSbyte(sbyteValue));
        }

        public BitArray8(char charValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(charValue));
        }

        public BitArray8(short shortValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(shortValue));
        }

        public BitArray8(ushort ushortValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ushortValue));
        }

        public BitArray8(int intValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(intValue));
        }

        public BitArray8(uint uintValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(uintValue));
        }

        public BitArray8(long longValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(longValue));
        }

        public BitArray8(ulong ulongValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ulongValue));
        }
    }
}
