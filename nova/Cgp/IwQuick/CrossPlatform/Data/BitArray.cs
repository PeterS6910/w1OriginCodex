using System;
using System.Text;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// Allows conversions from byte arrays to bool arrays and vice-versa 
    /// </summary>
    public class BitArray
    {
        protected bool[] _binaryArray = null;
        private readonly byte _bitLength = 0;

        /// <summary>
        /// 
        /// </summary>
        public int BitLength
        {
            get
            {
                if (null == _binaryArray)
                    return 0;
                
                return _binaryArray.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected BitArray()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitLength"></param>
        /// <param name="createNew"></param>
        protected BitArray(byte bitLength, bool createNew)
        {
            Validator.CheckZero(bitLength);
            _bitLength = bitLength;
            if (createNew)
            {
                _binaryArray = new bool[_bitLength];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (_binaryArray != null)
                for (int i = 0; i < _binaryArray.Length; i++)
                    _binaryArray[i] = false;
        }

        /// <summary>
        /// Gets boolean array representing bits from byte array
        /// </summary>
        /// <param name="byteArray">Input byte array</param> 
        /// <param name="binaryArrayLength">Length of the output boolean array</param> 
        public static bool[] GetBinaryArrayFromByteArray(byte[] byteArray, byte binaryArrayLength)
        {
            bool[] binaryArray = new bool[binaryArrayLength];
            int bitPosition = binaryArrayLength - 1;
            int i = 0;
            int j = 0;

            while ((j < (binaryArrayLength / 8)) && (j < byteArray.Length))
            {
                while (i < 8)
                {
                    if ((byteArray[j] & (1 << i)) != 0)
                    {
                        binaryArray[(binaryArrayLength - 1) - bitPosition] = true;
                    }
                    else
                    {
                        binaryArray[(binaryArrayLength - 1) - bitPosition] = false;
                    }
                    bitPosition--;
                    i++;
                }
                j++;
                i = 0;
            }
            return binaryArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static bool CompareByteArray(byte[] array1, byte[] array2)
        {            
            if (array1 == array2)
                return true;
            if (array1 == null || array2 == null)
                return false;
            if (array1.Length != array2.Length)
                return false;
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        protected bool[] GetBinaryArrayFromByteArray([NotNull] byte[] byteArray)
        {
            Validator.CheckForNull(byteArray,"byteArray");
            return GetBinaryArrayFromByteArray(byteArray, _bitLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="binaryArray"></param>
        public static void FillBinaryArrayFromByteArray(byte[] byteArray, bool[] binaryArray)
        {
            int bitPosition = binaryArray.Length - 1;
            int i = 0;
            int j = 0;

            while ((j < (binaryArray.Length / 8)) && (j < byteArray.Length))
            {
                while (i < 8)
                {
                    if ((byteArray[j] & (1 << i)) != 0)
                    {
                        binaryArray[(binaryArray.Length - 1) - bitPosition] = true;
                    }
                    else
                    {
                        binaryArray[(binaryArray.Length - 1) - bitPosition] = false;
                    }
                    bitPosition--;
                    i++;
                }
                j++;
                i = 0;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        protected void FillBinaryArrayFromBiteArray([NotNull] byte[] byteArray)
        {
            Validator.CheckForNull(byteArray,"byteArray");
            FillBinaryArrayFromByteArray(byteArray, _binaryArray);
        }      

        /// <summary>
        /// Gets byte array from boolean array representing bits
        /// </summary>
        /// <param name="binaryArray">Input binary array</param>     
        /// <param name="size">Byte array size in bites</param>
        /// <returns></returns>
        public static byte[] GetByteArrayFromBinaryArray(bool[] binaryArray, byte size)
        {
            byte[] byteArray = new byte[size / 8];
            int j = 0;
            int m = 1;
            int i = 0;
            int byteValue = 0;

            while ((j < (binaryArray.Length / 8)) && (j < (size / 8)) || (j == 0))
            {
                while ((i < binaryArray.Length) && (i < size))
                {
                    byteValue = byteValue + (Convert.ToInt32(binaryArray[i]) * m);
                    m = m * 2;
                    if (i % 8 == 7 || (i == (binaryArray.Length - 1)))
                    {
                        byteArray[j] = Convert.ToByte(byteValue);
                        byteValue = 0;
                        j++;
                        m = 1;
                    }
                    i++;
                }
                i = 0;
            }
            return byteArray;
        }

        /// <summary>
        ///  Convert binary array to SByte value
        /// </summary>
        /// <param name="binaryArray">Input binary array</param>
        /// <returns></returns>
        public static SByte ToSbyte(bool[] binaryArray)
        {
            bool[] SBinaryArray = new bool[16];
            if (binaryArray.Length < 8)
            {
                return Convert.ToSByte(GetByteArrayFromBinaryArray(binaryArray, 8)[0]);
            }
            if (binaryArray[8 - 1])
            {
                for (int i = 0; i < 8; i++)
                {
                    SBinaryArray[i] = !binaryArray[i];
                }
                int intValue = (BitConverter.ToInt16(GetByteArrayFromBinaryArray(SBinaryArray, 16), 0) + 1) * (-1);
                return Convert.ToSByte(intValue);
            }
            
            return Convert.ToSByte(GetByteArrayFromBinaryArray(binaryArray, 8)[0]);
        }

        /// <summary>
        ///  Convert from sbyte value to byte array
        /// </summary>
        /// <returns></returns>
        public static byte[] FromSbyte(sbyte sbyteValue)
        {
            byte[] bytes = BitConverter.GetBytes(sbyteValue);
            for (int i = 1; i < bytes.Length; i++)
            {
                bytes[i] = 0;
            }
            return bytes;
        }

        /// <summary>
        /// Gets bool value at specific index from binary array 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetBit(
            [NotNull] bool[] binaryArray, 
            int index)
        {
            Validator.CheckForNull(binaryArray,"binaryArray");
            Validator.CheckIntegerRange(index, 0, binaryArray.Length - 1);

            return binaryArray[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void SetBit(
            [NotNull] bool[] binaryArray, 
            int index, 
            bool value)
        {
            Validator.CheckForNull(binaryArray,"binaryArray");
            Validator.CheckIntegerRange(index, 0, binaryArray.Length - 1);

            binaryArray[index] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool[] GetBits(bool[] binaryArray, int startIndex, int length)
        {
            Validator.CheckZero(length);
            bool[] extractedArray = new bool[length];
            if ((startIndex < 0) || (startIndex > (binaryArray.Length - 1)))
            {
                throw new IndexOutOfRangeException("Start index was out of range");
            }
            
            if ((startIndex + length) > binaryArray.Length)
            {
                throw new IndexOutOfRangeException("Length from startIndex is out of range");
            }
            
            int j = 0;
            for (int i = startIndex; i < (length + startIndex); i++)
            {
                extractedArray[j] = binaryArray[i];
                j++;
            }
            return extractedArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        public static void SetBits(
            [NotNull] bool[] binaryArray,
            int index, int length, 
            [NotNull] bool[] value)
        {

            Validator.CheckForNull(binaryArray,"binaryArray");
            Validator.CheckForNull(value,"value");
            Validator.CheckIntegerRange(index + length, index, binaryArray.Length);           
            Validator.CheckIntegerRange(value.Length, length, long.MaxValue);
           
            for (int i = 0; i < length; i++)
            {
                binaryArray[i + index] = value[i];
            }
        }

        #region Conversions
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public char ToChar()
        {
            return BitConverter.ToChar(GetByteArrayFromBinaryArray(_binaryArray, 16), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Int16 ToInt16()
        {
            return BitConverter.ToInt16(GetByteArrayFromBinaryArray(_binaryArray, 16), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ToInt32()
        {
            return BitConverter.ToInt32(GetByteArrayFromBinaryArray(_binaryArray, 32), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long ToInt64()
        {
            return BitConverter.ToInt64(GetByteArrayFromBinaryArray(_binaryArray, 64), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UInt16 ToUInt16()
        {
            return BitConverter.ToUInt16(GetByteArrayFromBinaryArray(_binaryArray, 16), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uint ToUInt32()
        {
            return BitConverter.ToUInt32(GetByteArrayFromBinaryArray(_binaryArray, 32), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ulong ToUInt64()
        {
            return BitConverter.ToUInt64(GetByteArrayFromBinaryArray(_binaryArray, 64), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte ToByte()
        {
            return GetByteArrayFromBinaryArray(_binaryArray, 8)[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sbyte ToSByte()
        {
            return ToSbyte(_binaryArray);
        }            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte ToByte(int indexFrom, int length)
        {
            return ToByte(GetBits(_binaryArray, indexFrom, length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public sbyte ToSByte(int indexFrom, int length)
        {
            return ToSByte(GetBits(_binaryArray, indexFrom, length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public char ToChar(int indexFrom, int length)
        {
            return ToChar(GetBits(_binaryArray, indexFrom, length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public short ToInt16(int indexFrom, int length)
        {
            return ToInt16(GetBits(_binaryArray, indexFrom, length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int ToInt32(int indexFrom, int length)
        {
            return ToInt32(GetBits(_binaryArray, indexFrom, length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public long ToInt64(int indexFrom, int length)
        {
            return ToInt64(GetBits(_binaryArray, indexFrom, length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public ushort ToUInt16(int indexFrom, int length)
        {
            return ToUInt16(GetBits(_binaryArray, indexFrom, length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public uint ToUInt32(int indexFrom, int length)
        {
            return ToUInt32(GetBits(_binaryArray, indexFrom, length));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public ulong ToUInt64(int indexFrom, int length)
        {
            return ToUInt64(GetBits(_binaryArray, indexFrom, length));
        }
        #endregion

        #region StaticConversions
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static char ToChar(bool[] binaryArray)
        {
            return BitConverter.ToChar(GetByteArrayFromBinaryArray(binaryArray, 16), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static Int16 ToInt16(bool[] binaryArray)
        {
            return BitConverter.ToInt16(GetByteArrayFromBinaryArray(binaryArray, 16), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static int ToInt32(bool[] binaryArray)
        {
            return BitConverter.ToInt32(GetByteArrayFromBinaryArray(binaryArray, 32), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static long ToInt64(bool[] binaryArray)
        {
            return BitConverter.ToInt64(GetByteArrayFromBinaryArray(binaryArray, 64), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static UInt16 ToUInt16(bool[] binaryArray)
        {
            return BitConverter.ToUInt16(GetByteArrayFromBinaryArray(binaryArray, 16), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static uint ToUInt32(bool[] binaryArray)
        {
            return BitConverter.ToUInt32(GetByteArrayFromBinaryArray(binaryArray, 32), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static ulong ToUInt64(bool[] binaryArray)
        {
            return BitConverter.ToUInt64(GetByteArrayFromBinaryArray(binaryArray, 64), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static byte ToByte(bool[] binaryArray)
        {
            return GetByteArrayFromBinaryArray(binaryArray, 8)[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binaryArray"></param>
        /// <returns></returns>
        public static sbyte ToSByte(bool[] binaryArray)
        {
            return ToSbyte(binaryArray);
        }

        #endregion               
      
        #region Import
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteValue"></param>
        public void Import(byte byteValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(byteValue));
         
        }

        ///  <param name="byteValue"></param>
        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        ///  <param name="length">Number of bits to add</param>     
        public void Import(byte byteValue,int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(byteValue)));           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sbyteValue"></param>
        public void Import(sbyte sbyteValue)
        {
            FillBinaryArrayFromBiteArray(FromSbyte(sbyteValue)); 
        }

        /// <param name="sbyteValue"></param>
        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(sbyte sbyteValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(FromSbyte(sbyteValue)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charValue"></param>
        public void Import(char charValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(charValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Import(char charValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(charValue)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortValue"></param>
        public void Import(short shortValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(shortValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Import(short shortValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(shortValue)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intValue"></param>
        public void Import(int intValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(intValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Import(int intValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(intValue)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="longValue"></param>
        public void Import(long longValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(longValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="longValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Import(long longValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(longValue)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ushortValue"></param>
        public void Import(ushort ushortValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(ushortValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ushortValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Import(ushort ushortValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(ushortValue)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uintValue"></param>
        public void Import(uint uintValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(uintValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uintValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Import(uint uintValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(uintValue)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ulongValue"></param>
        public void Import(ulong ulongValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(ulongValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ulongValue"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public void Import(ulong ulongValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(ulongValue)));
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool this[int index]
        {
            get
            {
                return GetBit(_binaryArray, index);
            }
            set
            {
                SetBit(_binaryArray, index, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool[] this[int index, int length]
        {
            get
            {
                return GetBits(_binaryArray, index, length);
            }
            set
            {
                SetBits(_binaryArray, index, length, value);
            }
        }     

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder bits = new StringBuilder();
            for (int i = 0; i < _binaryArray.Length; i++)
            {
                bits.Append(_binaryArray[i] ? "1" : "0");
                
            }           
            return bits.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToReverseString()
        {
            StringBuilder bits = new StringBuilder();
            for (int i = 0; i < _binaryArray.Length; i++)
            {
                bits.Append(_binaryArray[_binaryArray.Length - i - 1] ? "1" : "0");
            }
            return bits.ToString();
        }
    }
}
