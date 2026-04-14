using System;
using System.Text;

using System.Security.Cryptography;

namespace Contal.IwQuick.Crypto
{
    public class QuickHashes
    {
        private static SHA1 _sha1 = new SHA1CryptoServiceProvider();

        /// <summary>
        /// computes SHA1 hash and returns in form of hexadecimal formatted string
        /// </summary>
        /// <param name="i_strValue"></param>
        /// <returns></returns>
        public static string GetSHA1String(string input, Encoding encoding)
        {
            if (null == encoding)
                return String.Empty;

            if (null == input)
                return String.Empty;

            byte[] data = encoding.GetBytes(input);

            return GetSHA1String(data, 0, data.Length);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSHA1String(string input)
        {
            return GetSHA1String(input, Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetSHA1String(byte[] input, int offset, int length)
        {
            if (null == input || offset < 0 || length < 0)
                return string.Empty;

            if (offset >= input.Length)
                offset = 0;

            if (offset + length > input.Length)
            {
                length = input.Length - offset;
            }

            try
            {

                byte[] output = null;
                lock (_sha1)
                {
                    // access to native SHA1 provider must be exclusive at the time
                    output = _sha1.ComputeHash(input, offset, length);
                }

                StringBuilder ret = new StringBuilder(String.Empty, 32);
                for (int i = 0; i < output.Length; i++)
                    ret.Append(output[i].ToString(StringConstants.HEX2_FORMAT).ToLower());

                return ret.ToString();
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSHA1String(byte[] input)
        {
            return GetSHA1String(input, 0, input.Length);
        }

        /// <summary>
        /// computes SHA512 hash and returns in form of byte array;
        /// returns empty byte array if anything goes wrong
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static byte[] GetSHA512(String inputValue)
        {
            if (null == inputValue)
                return new byte[0];

            try
            {
                SHA512 sha = new SHA512Managed();

                byte[] data = Encoding.UTF8.GetBytes(inputValue);
                return sha.ComputeHash(data);
            }
            catch
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// computes SHA512 hash and returns in form of hexadecimal formatted string;
        /// returns empty string if anything goes wrong
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string GetSHA512String(string inputValue)
        {
            try
            {
                if (null == inputValue)
                    inputValue = String.Empty;

                byte[] data = GetSHA512(inputValue);

                StringBuilder ret = new StringBuilder(String.Empty, 32);
                for (int i = 0; i < data.Length; i++)
                    ret.Append(data[i].ToString(StringConstants.HEX2_FORMAT).ToLower());

                return ret.ToString();
            }
            catch
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// computes SHA256 hash and returns in form of byte array;
        /// returns empty byte array if anything goes wrong
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetSHA256(String inputValue)
        {
            if (null == inputValue)
                return new byte[0];
            try
            {
                SHA256 sha = new SHA256Managed();

                byte[] data = Encoding.UTF8.GetBytes(inputValue);
                return sha.ComputeHash(data);
            }
            catch
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// computes SHA256 hash and returns in form of hexadecimal formatted string;
        /// returns empty string if anything goes wrong
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSHA256String(string inputValue)
        {
            try
            {
                if (null == inputValue)
                    inputValue = String.Empty;

                byte[] data = GetSHA256(inputValue);

                StringBuilder ret = new StringBuilder(String.Empty, 32);
                for (int i = 0; i < data.Length; i++)
                    ret.Append(data[i].ToString(StringConstants.HEX2_FORMAT).ToLower());

                return ret.ToString();
            }
            catch
            {
                return String.Empty;
            }

        }

        private static MD5 _md5 = new MD5CryptoServiceProvider();

        /// <summary>
        /// quick method for 32-character MD5 checksum
        /// </summary>
        /// <param name="i_strValue">string input</param>
        /// <returns>32-character hex-presented MD5 checksum</returns>
        public static string GetMD5String(string input)
        {
            if (null == input)
                input = String.Empty;

            byte[] rawData = Encoding.ASCII.GetBytes(input);

            lock (_md5)
            {
                // access to native SHA1 provider must be exclusive at the time
                rawData = _md5.ComputeHash(rawData);
            }

            StringBuilder output = new StringBuilder(String.Empty, 32);
            for (int i = 0; i < rawData.Length; i++)
                output.Append(rawData[i].ToString(StringConstants.HEX2_FORMAT).ToLower());

            return output.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_strValue"></param>
        /// <returns></returns>
        public static string GetCRC32String(string input)
        {
            if (null == input)
                input = String.Empty;

            byte[] rawData = Crc32.ComputeChecksumBytes(input);

            StringBuilder output = new StringBuilder(String.Empty, 32);
            for (int i = 0; i < rawData.Length; i++)
                output.Append(rawData[i].ToString(StringConstants.HEX2_FORMAT).ToLower());

            return output.ToString();
        }
    }
}
