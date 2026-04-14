using System;

namespace Contal.IwQuick
{
    public static class BoolExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="boolValue"></param>
        /// <returns></returns>
        public static int ToInt32(this bool boolValue)
        {
            return boolValue ? 1 : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boolValue"></param>
        /// <returns></returns>
        public static UInt32 ToUInt32(this bool boolValue)
        {
            return (UInt32)ToInt32(boolValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boolValue"></param>
        /// <returns></returns>
        public static UInt16 ToUInt16(this bool boolValue)
        {
            return (UInt16)ToInt32(boolValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boolValue"></param>
        /// <returns></returns>
        public static Int16 ToInt16(this bool boolValue)
        {
            return (Int16)ToInt32(boolValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boolValue"></param>
        /// <returns></returns>
        public static byte ToByte(this bool boolValue)
        {
            return (byte) ToInt32(boolValue);
        }

        /// <summary>
        /// first bool of array is LSB, last bool is MSB
        /// </summary>
        /// <param name="boolArray"></param>
        /// <param name="littleEndian"></param>
        /// <param name="result"></param>
        public static void ToByte(this bool[] boolArray, bool littleEndian, out byte result)
        {
            long lResult;
            Bits.Singleton.Convert(boolArray,null,  Bits.ByteBitLength, littleEndian, out lResult);

            result = (byte)lResult;
        }

        /// <summary>
        /// first bool of array is LSB, last bool is MSB
        /// </summary>
        /// <param name="boolArray"></param>
        /// <param name="littleEndian"></param>
        /// <returns></returns>
        public static byte ToByte(this bool[] boolArray, bool littleEndian)
        {
            byte result;
            ToByte(boolArray, littleEndian, out result);
            return result;
        }

        /// <summary>
        /// first bool of array is LSB, last bool is MSB
        /// </summary>
        /// <param name="boolArray"></param>
        /// <param name="littleEndian"></param>
        /// <param name="result"></param>
        public static void ToInt32(this bool[] boolArray, bool littleEndian, out Int32 result)
        {
            long lResult;
            Bits.Singleton.Convert(boolArray, null, Bits.IntBitLength, littleEndian, out lResult);
            result = (int)lResult;
        }

        /// <summary>
        /// first bool of array is LSB, last bool is MSB
        /// </summary>
        /// <param name="boolArray"></param>
        /// <param name="littleEndian"></param>
        /// <returns></returns>
        public static Int32 ToInt32(this bool[] boolArray, bool littleEndian)
        {
            Int32 result;
            ToInt32(boolArray, littleEndian, out result);
            return result;
        }

        /// <summary>
        /// first bool of array is LSB, last bool is MSB
        /// </summary>
        /// <param name="boolArray"></param>
        /// <param name="littleEndian"></param>
        /// <param name="result"></param>
        public static void ToUInt32(this bool[] boolArray, bool littleEndian, out UInt32 result)
        {
            long lResult;
            Bits.Singleton.Convert(boolArray, null, Bits.IntBitLength, littleEndian, out lResult);
            result = (UInt32)lResult;
        }

        /// <summary>
        /// first bool of array is LSB, last bool is MSB
        /// </summary>
        /// <param name="boolArray"></param>
        /// <param name="littleEndian"></param>
        /// <returns></returns>
        public static UInt32 ToUInt32(this bool[] boolArray, bool littleEndian)
        {
            UInt32 result;
            ToUInt32(boolArray, littleEndian, out result);
            return result;
        }
    }
}
