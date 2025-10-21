using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="separator"></param>
        /// <param name="partsToCombine"></param>
        public static void Combine(
            this StringBuilder stringBuilder, 
            string separator,
            IEnumerable<object> partsToCombine)
        {
            if (stringBuilder == null ||
                ReferenceEquals(partsToCombine,null))
                return;

            if (separator == null)
                separator = string.Empty;


            bool first = true;
            foreach (var part in partsToCombine)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    stringBuilder.Append(separator);
                }

                stringBuilder.Append(part ?? String.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="separator"></param>
        /// <param name="partsToCombine"></param>
        public static void Combine(
            this StringBuilder stringBuilder,
            string separator,
            params object[] partsToCombine)
        {
            Combine(stringBuilder,separator,(IEnumerable<object>)partsToCombine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringBuilder"></param>
        public static void Clear(this StringBuilder stringBuilder)
        {
            if (stringBuilder != null)
                stringBuilder.Remove(0, stringBuilder.Length);
        }
    }
}
