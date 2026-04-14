using System;
using System.Text;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ByteConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ToBytes(
            [NotNull] byte[] destinationArray, 
            int destinationIndex, 
            [NotNull] params Int32[] values)
        {
            Validator.CheckForNull(destinationArray,"destinationArray");
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckForNull(values,"values");

            if (sizeof(Int32) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            foreach (Int32 value in values)
            {
                byte[] byteArray = BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ToBytes(
            [NotNull] byte[] destinationArray,
            int destinationIndex, 
            [NotNull] params UInt32[] values)
        {
            Validator.CheckForNull(destinationArray,"destinationArray");
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckForNull(values,"values");

            if (sizeof(UInt32) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            foreach (UInt32 value in values)
            {
                byte[] byteArray = BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ToBytes(
            [NotNull] byte[] destinationArray,
            int destinationIndex, 
            [NotNull] params Int64[] values)
        {
            Validator.CheckForNull(destinationArray,"destinationArray");
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckForNull(values,"values");

            if (sizeof(Int64) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            foreach (Int64 value in values)
            {
                byte[] byteArray = BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ToBytes(
            [NotNull] byte[] destinationArray, 
            int destinationIndex, 
            [NotNull] params UInt64[] values)
        {
            Validator.CheckForNull(destinationArray,"destinationArray");
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckForNull(values,"values");

            if (sizeof(UInt64) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            foreach (UInt64 value in values)
            {
                byte[] byteArray = BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ToBytes(
            [NotNull] byte[] destinationArray,
            int destinationIndex, 
            [NotNull] params Int16[] values)
        {
            Validator.CheckForNull(destinationArray, "destinationArray");
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckForNull(values,"values");

            if (sizeof(Int16) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            foreach (Int16 value in values)
            {
                byte[] byteArray = BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ToBytes(
            [NotNull] byte[] destinationArray, 
            int destinationIndex, 
            [NotNull] params UInt16[] values)
        {
            Validator.CheckForNull(destinationArray,"destinationArray");
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckForNull(values,"values");

            if (sizeof(UInt16) * values.Length + destinationIndex > destinationArray.Length)
                throw new ArgumentException("Values would not fit into the array");

            foreach (UInt16 value in values)
            {
                byte[] byteArray = BitConverter.GetBytes(value);

                Array.Copy(byteArray, 0, destinationArray, destinationIndex, byteArray.Length);

                destinationIndex += byteArray.Length;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ToBytes(
            [NotNull] byte[] destinationArray, 
            int destinationIndex, 
            [NotNull] string value, 
            Encoding encoding)
        {
            Validator.CheckForNull(destinationArray,"destinationArray");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="value"></param>
        public static void ToBytes(byte[] destinationArray, int destinationIndex, string value)
        {
            ToBytes(destinationArray, destinationIndex, value, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationArray"></param>
        /// <param name="destinationIndex"></param>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ToBytes(
            [NotNull] byte[] destinationArray, 
            int destinationIndex, 
            params bool[] values)
        {
            Validator.CheckForNull(destinationArray,"destinationArray");
            Validator.CheckIntegerRange(destinationIndex, 0, destinationArray.Length - 1);
            Validator.CheckForNull(values,"values");

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static byte[] GetBytesFromStruct<T>(T str) where T : struct
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetStructFromBytes<T>(byte[] arr) where T : struct
        {
            T str = new T();
            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (T)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }
    }
}
