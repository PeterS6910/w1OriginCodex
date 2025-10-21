using System;
using System.Collections.Generic;
using System.Linq;

namespace Contal.IwQuick
{
    public class Bits
    {
        private Bits()
        {
            
        }

        public static readonly Bits Singleton = new Bits();

        public const int ByteBitLength = 8;
        public const int ShortBitLength = 16;
        public const int IntBitLength = 32;


        /// <summary>
        /// first bool of array is LSB, last bool is MSB
        /// </summary>
        /// <param name="boolLikeValues"></param>
        /// <param name="convertValueToBoolLambda"></param>
        /// <param name="bitLimit"></param>
        /// <param name="littleEndian"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if convertValueToBoolLambda is null while T is not bool</exception>
        /// <exception cref="ArgumentOutOfRangeException">if bitLimit is negative or zero</exception>
        public void Convert<T>(
            IList<T> boolLikeValues, 
            Func<T,bool> convertValueToBoolLambda, 
            int bitLimit,
            bool littleEndian,
            out long result)
        {
            var tIsBool = false;
            if (typeof (T) != typeof (bool))
                Validator.CheckForNull(convertValueToBoolLambda, "convertValueToBoolLambda");
            else
                tIsBool = true;

            Validator.CheckNegativeOrZeroInt(bitLimit,"bitLimit");

            result = 0;

            if (boolLikeValues == null ||
                boolLikeValues.Count == 0)
                return;

            var arrLen = boolLikeValues.Count;

            if (arrLen > bitLimit)
                arrLen = bitLimit;

            for (int i = 0; i < arrLen; i++)
            {
                int shiftFactor =
                    littleEndian
                        ? i
                        : arrLen - i - 1;

                bool boolValue;

                if (tIsBool)
                    boolValue = (bool)(object)boolLikeValues[i];
                else
                {
                    boolValue = convertValueToBoolLambda(boolLikeValues[i]);
                }

                result |= ((boolValue ? 1 : 0) << shiftFactor);

            }
        }

        /// <summary>
        /// first bool of array is LSB, last bool is MSB
        /// </summary>
        /// <param name="boolLikeValues"></param>
        /// <param name="convertValueToBoolLambda"></param>
        /// <param name="bitLimit"></param>
        /// <param name="littleEndian"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if convertValueToBoolLambda is null while T is not bool</exception>
        /// <exception cref="ArgumentOutOfRangeException">if bitLimit is negative or zero</exception>
        public void Convert<T>(
            IEnumerable<T> boolLikeValues, 
            Func<T, bool> convertValueToBoolLambda,
            int bitLimit,
            bool littleEndian,
            out long result)
        {
            Convert(
                boolLikeValues.ToArray(),
                convertValueToBoolLambda,
                bitLimit,
                littleEndian,
                out result);
        }
    }
}
