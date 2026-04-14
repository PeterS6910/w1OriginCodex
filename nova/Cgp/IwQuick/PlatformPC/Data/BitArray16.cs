using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// Represents binary array stored as boolean array with length of 16
    /// </summary>
    public class BitArray16 : BitArray
    {
        const byte BITARRAY_LENGTH_16 = 16;

        #region Constructors

        public BitArray16()
            : base(BITARRAY_LENGTH_16, true)
        {
        }

        public BitArray16(byte byteValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(byteValue));
        }

        public BitArray16(sbyte sbyteValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(FromSbyte(sbyteValue));
        }

        public BitArray16(char charValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(charValue));
        }

        public BitArray16(short shortValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(shortValue));
        }

        public BitArray16(ushort ushortValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ushortValue));
        }

        public BitArray16(int intValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(intValue));
        }

        public BitArray16(uint uintValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(uintValue));
        }

        public BitArray16(long longValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(longValue));
        }

        public BitArray16(ulong ulongValue)
            : base(BITARRAY_LENGTH_16, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ulongValue));
        }
        
        #endregion       
    }
}
