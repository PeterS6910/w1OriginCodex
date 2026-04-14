using System.Text;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    public static class StringExtensions
    {
        public static byte[] ToBytes(this string inputString,[NotNull] Encoding encoding)
        {
            Validator.CheckForNull(encoding,"encoding");

            if (ReferenceEquals(inputString, null))
                return null;

            return encoding.GetBytes(inputString);

        }

        public static byte[] ToBytes(this string inputString)
        {
            return ToBytes(inputString, Encoding.UTF8);
        }
    }
}
