using System;

namespace Contal.IwQuickCF.Data
{
    /// <summary>
    /// Represents binary array stored as boolean array with length of 32
    /// </summary>
    public class BitArray32: BitArray
    {
        const byte BITARRAY_LENGTH_32 = 32;

        /// <summary>
        /// 
        /// </summary>
        public BitArray32()
            : base(BITARRAY_LENGTH_32, true)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteValue"></param>
        public BitArray32(byte byteValue)
            : base(BITARRAY_LENGTH_32,false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(byteValue));
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sbyteValue"></param>
        public BitArray32(sbyte sbyteValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(FromSbyte(sbyteValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charValue"></param>
        public BitArray32(char charValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(charValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortValue"></param>
        public BitArray32(short shortValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(shortValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ushortValue"></param>
        public BitArray32(ushort ushortValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ushortValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intValue"></param>
        public BitArray32(int intValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(intValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uintValue"></param>
        public BitArray32(uint uintValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(uintValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="longValue"></param>
        public BitArray32(long longValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(longValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ulongValue"></param>
        public BitArray32(ulong ulongValue)
            : base(BITARRAY_LENGTH_32, false)
        {
            _binaryArray = GetBinaryArrayFromByteArray(BitConverter.GetBytes(ulongValue));
        }        
    }
}
