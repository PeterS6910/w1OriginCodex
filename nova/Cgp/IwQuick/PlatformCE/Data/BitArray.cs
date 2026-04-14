using System;
using System.Text;

namespace Contal.IwQuickCF.Data
{
    /// <summary>
    /// Allows conversions from byte arrays to bool arrays and vice-versa 
    /// </summary>
    public class BitArray
    {
        protected bool[] _binaryArray = null;
        private byte _bitLength = 0;

        public int BitLength
        {
            get
            {
                if (null == _binaryArray)
                    return 0;
                else
                    return _binaryArray.Length;
            }
        }

        protected BitArray()
        {
        }

        protected BitArray(byte bitLength, bool createNew)
        {
            Validator.CheckZero(bitLength);
            _bitLength = bitLength;
            if (createNew)
            {
                _binaryArray = new bool[_bitLength];
            }
        }

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

        protected bool[] GetBinaryArrayFromByteArray(byte[] byteArray)
        {
            Validator.CheckNull(byteArray);
            return GetBinaryArrayFromByteArray(byteArray, _bitLength);
        }

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

        protected void FillBinaryArrayFromBiteArray(byte[] byteArray)
        {
            Validator.CheckNull(byteArray);
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
            if (binaryArray[8 - 1] == true)
            {
                for (int i = 0; i < 8; i++)
                {
                    SBinaryArray[i] = !binaryArray[i];
                }
                int intValue = (BitConverter.ToInt16(GetByteArrayFromBinaryArray(SBinaryArray, 16), 0) + 1) * (-1);
                return Convert.ToSByte(intValue);
            }
            else
            {
                return Convert.ToSByte(GetByteArrayFromBinaryArray(binaryArray, 8)[0]);
            }
        }

        /// <summary>
        ///  Convert from sbyte value to byte array
        /// </summary>
        /// <param name="binaryArray">Input binary array</param>
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
        public static bool GetBit(bool[] binaryArray, int index)
        {
            Validator.CheckNull(binaryArray);
            Validator.CheckIntegerRange(index, 0, binaryArray.Length - 1);

            return binaryArray[index];
        }

        public static void SetBit(bool[] binaryArray, int index, bool value)
        {
            Validator.CheckNull(binaryArray);
            Validator.CheckIntegerRange(index, 0, binaryArray.Length - 1);

            binaryArray[index] = value;
        }

        public static bool[] GetBits(bool[] binaryArray, int startIndex, int length)
        {
            Validator.CheckZero(length);
            bool[] extractedArray = new bool[length];
            if ((startIndex < 0) || (startIndex > (binaryArray.Length - 1)))
            {
                throw new IndexOutOfRangeException("Start index was out of range");
            }
            else if ((startIndex + length) > binaryArray.Length)
            {
                throw new IndexOutOfRangeException("Length from startIndex is out of range");
            }
            else
            {
                int j = 0;
                for (int i = startIndex; i < (length + startIndex); i++)
                {
                    extractedArray[j] = binaryArray[i];
                    j++;
                }
            }
            return extractedArray;
        }

        public static void SetBits(bool[] binaryArray, int index, int length, bool[] value)
        {

            Validator.CheckNull(binaryArray);
            Validator.CheckNull(value);
            Validator.CheckIntegerRange(index + length, index, binaryArray.Length);           
            Validator.CheckIntegerRange(value.Length, length, long.MaxValue);
           
            for (int i = 0; i < length; i++)
            {
                binaryArray[i + index] = value[i];
            }
        }

        #region Conversions
        public char ToChar()
        {
            return BitConverter.ToChar(BitArray.GetByteArrayFromBinaryArray(_binaryArray, 16), 0);
        }
        public Int16 ToInt16()
        {
            return BitConverter.ToInt16(BitArray.GetByteArrayFromBinaryArray(_binaryArray, 16), 0);
        }
        public int ToInt32()
        {
            return BitConverter.ToInt32(BitArray.GetByteArrayFromBinaryArray(_binaryArray, 32), 0);
        }
        public long ToInt64()
        {
            return BitConverter.ToInt64(BitArray.GetByteArrayFromBinaryArray(_binaryArray, 64), 0);
        }
        public UInt16 ToUInt16()
        {
            return BitConverter.ToUInt16(BitArray.GetByteArrayFromBinaryArray(_binaryArray, 16), 0);
        }
        public uint ToUInt32()
        {
            return BitConverter.ToUInt32(BitArray.GetByteArrayFromBinaryArray(_binaryArray, 32), 0);
        }
        public ulong ToUInt64()
        {
            return BitConverter.ToUInt64(BitArray.GetByteArrayFromBinaryArray(_binaryArray, 64), 0);
        }
        public byte ToByte()
        {
            return BitArray.GetByteArrayFromBinaryArray(_binaryArray, 8)[0];
        }
        public sbyte ToSByte()
        {
            return BitArray.ToSbyte(_binaryArray);
        }            

        public byte ToByte(int indexFrom, int length)
        {
            return ToByte(GetBits(_binaryArray, indexFrom, length));
        }

        public sbyte ToSByte(int indexFrom, int length)
        {
            return ToSByte(GetBits(_binaryArray, indexFrom, length));
        }

        public char ToChar(int indexFrom, int length)
        {
            return ToChar(GetBits(_binaryArray, indexFrom, length));
        }

        public short ToInt16(int indexFrom, int length)
        {
            return ToInt16(GetBits(_binaryArray, indexFrom, length));
        }

        public int ToInt32(int indexFrom, int length)
        {
            return ToInt32(GetBits(_binaryArray, indexFrom, length));
        }

        public long ToInt64(int indexFrom, int length)
        {
            return ToInt64(GetBits(_binaryArray, indexFrom, length));
        }

        public ushort ToUInt16(int indexFrom, int length)
        {
            return ToUInt16(GetBits(_binaryArray, indexFrom, length));
        }

        public uint ToUInt32(int indexFrom, int length)
        {
            return ToUInt32(GetBits(_binaryArray, indexFrom, length));
        }

        public ulong ToUInt64(int indexFrom, int length)
        {
            return ToUInt64(GetBits(_binaryArray, indexFrom, length));
        }
        #endregion

        #region StaticConversions
        
        public static char ToChar(bool[] binaryArray)
        {
            return BitConverter.ToChar(BitArray.GetByteArrayFromBinaryArray(binaryArray, 16), 0);
        }
        public static Int16 ToInt16(bool[] binaryArray)
        {
            return BitConverter.ToInt16(BitArray.GetByteArrayFromBinaryArray(binaryArray, 16), 0);
        }
        public static int ToInt32(bool[] binaryArray)
        {
            return BitConverter.ToInt32(BitArray.GetByteArrayFromBinaryArray(binaryArray, 32), 0);
        }
        public static long ToInt64(bool[] binaryArray)
        {
            return BitConverter.ToInt64(BitArray.GetByteArrayFromBinaryArray(binaryArray, 64), 0);
        }
        public static UInt16 ToUInt16(bool[] binaryArray)
        {
            return BitConverter.ToUInt16(BitArray.GetByteArrayFromBinaryArray(binaryArray, 16), 0);
        }
        public static uint ToUInt32(bool[] binaryArray)
        {
            return BitConverter.ToUInt32(BitArray.GetByteArrayFromBinaryArray(binaryArray, 32), 0);
        }
        public static ulong ToUInt64(bool[] binaryArray)
        {
            return BitConverter.ToUInt64(BitArray.GetByteArrayFromBinaryArray(binaryArray, 64), 0);
        }
        public static byte ToByte(bool[] binaryArray)
        {
            return BitArray.GetByteArrayFromBinaryArray(binaryArray, 8)[0];
        }

        public static sbyte ToSByte(bool[] binaryArray)
        {
            return BitArray.ToSbyte(binaryArray);
        }

        #endregion               
      
        #region Import
      
        public void Import(byte byteValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(byteValue));
         
        }        
     
        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(byte byteValue,int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(byteValue)));           
        }

        public void Import(sbyte sbyteValue)
        {
            FillBinaryArrayFromBiteArray(FromSbyte(sbyteValue)); 
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(sbyte sbyteValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(FromSbyte(sbyteValue)));
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(char charValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(charValue));
        }

        public void Import(char charValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(charValue)));
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(short shortValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(shortValue));
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(short shortValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(shortValue)));
        }

        public void Import(int intValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(intValue));
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(int intValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(intValue)));
        }

        public void Import(long longValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(longValue));
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(long longValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(longValue)));
        }

        public void Import(ushort ushortValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(ushortValue));
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(ushort ushortValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(ushortValue)));
        }

        public void Import(uint uintValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(uintValue));
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(uint uintValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(uintValue)));
        }

        public void Import(ulong ulongValue)
        {
            FillBinaryArrayFromBiteArray(BitConverter.GetBytes(ulongValue));
        }

        /// <param name="startIndex">The zero-based starting bit index at witch adding begins</param>
        /// <param name="length">Number of bits to add</param>     
        public void Import(ulong ulongValue, int startIndex, int length)
        {
            SetBits(_binaryArray, startIndex, length, GetBinaryArrayFromByteArray(BitConverter.GetBytes(ulongValue)));
        }

        #endregion

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

        public override string ToString()
        {
            StringBuilder bits = new StringBuilder();
            for (int i = 0; i < _binaryArray.Length; i++)
            {
                bits.Append(_binaryArray[i] ? "1" : "0");
                
            }           
            return bits.ToString();
        }

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
