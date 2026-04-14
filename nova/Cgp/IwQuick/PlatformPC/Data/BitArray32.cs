using System;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// Represents binary array stored as boolean array with length of 32
    /// </summary>
    public class BitArray32: BitArray
    {
        const byte BITARRAY_LENGTH_32 = 32;

        public BitArray32()
            : base(BITARRAY_LENGTH_32, true)
        {
        }

        public BitArray32(byte byteValue)
            : base(BITARRAY_LENGTH_32,false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(byteValue));
        }
       
        public BitArray32(sbyte sbyteValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(FromSbyte(sbyteValue));
        }

        public BitArray32(char charValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(charValue));
        }

        public BitArray32(short shortValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(shortValue));
        }

        public BitArray32(ushort ushortValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ushortValue));
        }

        public BitArray32(int intValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(intValue));
        }

        public BitArray32(uint uintValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(uintValue));
        }

        public BitArray32(long longValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(longValue));
        }

        public BitArray32(ulong ulongValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ulongValue));
        }        
    }
}
