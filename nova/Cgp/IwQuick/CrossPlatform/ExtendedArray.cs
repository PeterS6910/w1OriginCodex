using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtendedArray
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static bool VerifyIndexOffset(this Array array, ref int offset, ref int length,bool strict)
        {
            if (array == null ||
                array.Length == 0)
                return false;

            if (offset < 0 ||
                offset >= array.Length)
            {
                if (strict)
                    return false;
                
                offset = 0;
            }

            if (strict && length < 0)
                return false;
               
            if (length < 0 || offset + length > array.Length)
                length = array.Length - offset;

            return true;
        }

        private const char PrefixDash = '-';
        private const char PrefixSlash = '/';

        public static bool FindArgument(this IEnumerable<string> stringArray,string argumentName,out string extractedValue)
        {
            extractedValue = string.Empty;

            if (string.IsNullOrEmpty(argumentName))
                return false;

            if (stringArray == null)
                return false;

            var al = argumentName.Length;

            foreach (var s in stringArray)
            {
                var pos = s.IndexOf(argumentName, StringComparison.Ordinal);
                if (pos == 0
                    || (pos == 1 && (s[0] == PrefixSlash || s[0] == PrefixDash))
                    || (pos == 2 && s[0] == PrefixDash && s[1] == PrefixDash))
                {
                    if (s.Length == pos + al)
                        return true;

                    var valuePos = pos + al;
                    if (s[valuePos] == ':' || s[valuePos] == '=')
                    {
                        extractedValue = s.Substring(valuePos + 1);
                    }

                    return true;
                }

            }

            return false;
        }

        private static bool FindArgument<T>(
            [NotNull] IEnumerable<string> stringArray,
            [NotNull] string argumentName,
            out T extractedValue,
            [NotNull] Func<string, T> conversionLambda)
            where T : struct
        {
            string extractedString;
            extractedValue = default(T);

            if (stringArray.FindArgument(argumentName, out extractedString) 
                && !string.IsNullOrEmpty(extractedString))
            {
                // intentionally .Parse instead of .TryParse due crossplatforming
                try
                {
                    extractedValue = conversionLambda(extractedString);

                    return true;
                }
                catch
                {

                }

            }

            return false;
        }

        public static bool FindArgument(this string[] stringArray, string argumentName, out byte extractedValue)
        {
            return FindArgument(stringArray, argumentName, out extractedValue, byte.Parse);
        }

        public static bool FindArgument(this string[] stringArray, string argumentName, out int extractedValue)
        {
            return FindArgument(stringArray, argumentName, out extractedValue, int.Parse);
        }

        public static bool FindArgument(this string[] stringArray, string argumentName, out uint extractedValue)
        {
            return FindArgument(stringArray, argumentName, out extractedValue, uint.Parse);
        }

        public static bool FindArgument(this string[] stringArray, string argumentName, out ushort extractedValue)
        {
            return FindArgument(stringArray, argumentName, out extractedValue, ushort.Parse);
        }

        public static bool FindArgument(this string[] stringArray, string argumentName, out short extractedValue)
        {
            return FindArgument(stringArray, argumentName, out extractedValue, short.Parse);
        }

        public static bool FindArgument<T>(
            [NotNull] this string[] stringArray,
            [NotNull] string argumentName,
            [NotNull] Func<string, T> conversionLambda,
            out T extractedValue
            )
            where T : struct
        {
            Validator.CheckForNull(argumentName,"argumentName");
            Validator.CheckForNull(conversionLambda, "conversionLambda");

            return FindArgument(stringArray, argumentName, out extractedValue, conversionLambda);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }
    }
}
