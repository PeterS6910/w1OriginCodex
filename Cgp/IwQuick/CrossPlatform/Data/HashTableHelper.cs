using System.Linq;
using System.Collections.Generic;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public static class HashTableHelper 
    {
        public class ByteArrayComparer : IEqualityComparer<IEnumerable<byte>>
        {
            bool IEqualityComparer<IEnumerable<byte>>.Equals(IEnumerable<byte> x, IEnumerable<byte> y)
            {
                return x.SequenceEqual(y);
            }

            int IEqualityComparer<IEnumerable<byte>>.GetHashCode(IEnumerable<byte> obj)
            {
                return HashTableHelper.GetHashCode(obj);
            }
        }

        public static int GetHashCode(IEnumerable<byte> bytes)
        {
            unchecked
            {
                int num1 = 352654597;
                int num2 = num1;

                var byteEnum = bytes.GetEnumerator();

                while (byteEnum.MoveNext())
                {
                    num1 += (num1 << 5) + (num1 >> 27);
                    num1 ^= byteEnum.Current;

                    if (!byteEnum.MoveNext())
                        break;

                    num2 += (num2 << 5) + (num2 >> 27);
                    num2 ^= byteEnum.Current;
                }

                return num1 + num2 * 1566083941;
            }
        }

        public static int CombineHashCodes(
            IEnumerable<int> hashCodes)
        {
            unchecked
            {
                int result = 5381;

                foreach (var hashCode in hashCodes)
                {
                    result += (result << 5);
                    result ^= hashCode;
                }

                return result;
            }
        }

        public static int CombineHashCodes(
            params int[] hashCodes)
        {
            unchecked
            {
                int result = 5381;

                foreach (var hashCode in hashCodes)
                {
                    result += (result << 5);
                    result ^= hashCode;
                }

                return result;
            }
        }
    }
}
