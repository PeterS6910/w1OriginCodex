using System;
using System.Text;

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Data
#else
namespace Contal.IwQuickCF.Data
#endif
{
    public class ByteConverter
    {
        public static void ToBytes(byte[] destinationArray, int destinationIndex, params Int32[] values)
        {
            Validator.CheckNull(destinationArray);
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckNull(values);

            if (sizeof(Int32) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            byte[] byteArray;

            foreach (Int32 value in values)
            {
                byteArray = System.BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        public static void ToBytes(byte[] destinationArray, int destinationIndex, params UInt32[] values)
        {
            Validator.CheckNull(destinationArray);
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckNull(values);

            if (sizeof(UInt32) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            byte[] byteArray;

            foreach (UInt32 value in values)
            {
                byteArray = System.BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        public static void ToBytes(byte[] destinationArray, int destinationIndex, params Int64[] values)
        {
            Validator.CheckNull(destinationArray);
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckNull(values);

            if (sizeof(Int64) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            byte[] byteArray;

            foreach (Int64 value in values)
            {
                byteArray = System.BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        public static void ToBytes(byte[] destinationArray, int destinationIndex, params UInt64[] values)
        {
            Validator.CheckNull(destinationArray);
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckNull(values);

            if (sizeof(UInt64) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            byte[] byteArray;

            foreach (UInt64 value in values)
            {
                byteArray = System.BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        public static void ToBytes(byte[] destinationArray, int destinationIndex, params Int16[] values)
        {
            Validator.CheckNull(destinationArray);
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckNull(values);

            if (sizeof(Int16) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            byte[] byteArray;

            foreach (Int16 value in values)
            {
                byteArray = System.BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        public static void ToBytes(byte[] destinationArray, int destinationIndex, params UInt16[] values)
        {
            Validator.CheckNull(destinationArray);
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckNull(values);

            if (sizeof(UInt16) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            byte[] byteArray;

            foreach (UInt16 value in values)
            {
                byteArray = System.BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        public static void ToBytes(byte[] destinationArray, int destinationIndex, string value, Encoding encoding)
        {
            Validator.CheckNull(destinationArray);
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckNullString(value);

            Byte[] encodedBytes;

            if (encoding == null)
            {
                encodedBytes = Encoding.UTF8.GetBytes(value);
            }
            else
            {
                encodedBytes = encoding.GetBytes(value);
            }

            if (encodedBytes.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Value would not fit into the array");


            Array.Copy(encodedBytes, 0, destinationArray, destinationIndex, encodedBytes.Length);
        }

        public static void ToBytes(byte[] destinationArray, int destinationIndex, string value)
        {
            ToBytes(destinationArray, destinationIndex, value, null);
        }

        public static void ToBytes(byte[] destinationArray, int destinationIndex, params bool[] values)
        {
            Validator.CheckNull(destinationArray);
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckNull(values);

            if (values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");


            foreach (bool value in values)
                destinationArray[destinationIndex++] = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static uint ReverseUint(uint source)
        {
            byte[] bytes = BitConverter.GetBytes(source);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
