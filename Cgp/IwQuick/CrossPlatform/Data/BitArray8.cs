using System;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// Represents binary array stored as boolean array with length of 8
    /// </summary>
    [Obsolete("Use extension method ToByte on bool array instead")]
    public class BitArray8 : BitArray
    {
        const byte BITARRAY_LENGTH_8 = 8;

        /// <summary>
        /// 
        /// </summary>
        public BitArray8()
            : base(BITARRAY_LENGTH_8, true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteValue"></param>
        public BitArray8(byte byteValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(byteValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sbyteValue"></param>
        public BitArray8(sbyte sbyteValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(FromSbyte(sbyteValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charValue"></param>
        public BitArray8(char charValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(charValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortValue"></param>
        public BitArray8(short shortValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(shortValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ushortValue"></param>
        public BitArray8(ushort ushortValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ushortValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intValue"></param>
        public BitArray8(int intValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(intValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uintValue"></param>
        public BitArray8(uint uintValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(uintValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="longValue"></param>
        public BitArray8(long longValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(longValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ulongValue"></param>
        public BitArray8(ulong ulongValue)
            : base(BITARRAY_LENGTH_8, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ulongValue));
        }
    }
}
