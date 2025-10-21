using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Text.RegularExpressions;

namespace Contal.IwQuick.Parsing
{
    /// <summary>
    /// class for usual text parsing routines aggregation
    /// </summary>
    public class QuickParser
    {
        /// <summary>
        /// trims the strings including trimming the quotations by ' or " at the beginning and the end
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TrimUnquot(string input)
        {
            if (null == input)
                return null;

            input = input.Trim();

            if (input.Length == 0)
                return null;

            if ('\"' == input[0])
                input = input.Substring(1);

            if ('\"' == input[input.Length - 1])
                input = input.Substring(0, input.Length - 1);

            return input;

        }

        /// <summary>
        /// modify string to contain only digits 
        /// </summary>
        /// <param name="input"></param>
        /// <returns>substring of input starting at position 0 and ending on first no digit character</returns>
        public static string GetValidDigitString(string input)
        {
            if (input == null)
                return string.Empty;
            StringBuilder text = new StringBuilder(input);
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsDigit(input[i]))
                {
                    text.Length = i;
                    break;
                }
            }
            return text.ToString();
        }

        /// <summary>
        /// modify string to contain only chars from 0x20 to 0x7E
        /// </summary>
        /// <param name="input"></param>
        /// <returns>substring of input starting at position 0 and ending on first no digit character</returns>
        public static string GetValidPrintableString(string input)
        {
            if (input == null)
                return string.Empty;
            StringBuilder text = new StringBuilder(input);
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] < 0x20 || input[i] > 0x7E)
                {
                    text.Length = i;
                    break;
                }
            }
            return text.ToString();
        }

        /// <summary>
        /// modify string to contain only hexadecimal characters
        /// </summary>
        /// <param name="input"></param>
        /// <returns>substring of input starting at position 0 and ending on first no hexadecimal character</returns>
        public static string GetValidHexaString(string input)
        {
            if (input == null)
                return string.Empty;
            Regex hexaRegex = new Regex("([A-Fa-f0-9])");
            StringBuilder validText = new StringBuilder(input);
            for (int i = 0; i < input.Length; i++)
            {
                if (!hexaRegex.IsMatch(input[i].ToString()))
                {
                    validText.Length = i;
                    break;
                }
            }
            return validText.ToString();
        }

        /// <summary>
        /// provides spliting of a string qutoed parts, 
        /// quoted parts are transcripted as one item of array, even if they contain whitespace
        /// </summary>
        /// <param name="input">string input</param>
        public static string[] QuotedSplit(string input)
        {
            if (Validator.IsNullString(input))
                return null;

            input = input.Trim();
            LinkedList<int> aArray = new LinkedList<int>();

            bool bQuoteBlock = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (' ' == input[i])
                {
                    if (!bQuoteBlock)
                    {
                        if (i == 0 || ' ' != input[i - 1])
                            aArray.AddLast(i);
                    }
                }
                else
                {
                    if ('\"' == input[i])
                    {
                        if (bQuoteBlock &&
                            i > 0 && '\\' == input[i - 1])
                        {
                            //continue;
                        }
                        else
                            bQuoteBlock = !bQuoteBlock;
                    }
                }
            }

            string[] arRet = new string[aArray.Count + 1];
            LinkedListNode<int> aNode = aArray.First;
            int iLastPos = 0;
            int k = 0;
            int iPos = 0;
            string strVal = null;
            while (null != aNode)
            {
                iPos = aNode.Value;
                strVal = input.Substring(iLastPos, iPos - iLastPos);
                strVal = TrimUnquot(strVal);

                arRet[k] = strVal;

                aNode = aNode.Next;
                iLastPos = iPos;
                k++;
            }

            strVal = input.Substring(iLastPos, input.Length - iLastPos);
            strVal = TrimUnquot(strVal);
            arRet[k] = strVal;

            return arRet;

        }

        /// <summary>
        /// counts number of occurences of pattern in the input string;
        /// returns 0 if nothing found, or invalid arguments specified
        /// </summary>
        /// <param name="input"></param>
        /// <param name="patternToSearch"></param>
        /// <returns></returns>
        public static int CountIndexOf(string input, string patternToSearch)
        {
            if (Validator.IsNullString(input))
                return 0;

            if (Validator.IsNullString(patternToSearch))
                return 0;

            int iStartIndex = 0;
            int iCount = 0;
            while (true)
            {
                iStartIndex = input.IndexOf(patternToSearch, iStartIndex);
                if (iStartIndex >= 0)
                    iCount++;
                else
                    break;

                iStartIndex++;
                if (iStartIndex >= input.Length)
                    break;
            }

            return iCount;
        }

        /// <summary>
        /// returns position of the argument/parameter equivalent in the array of arguments
        /// or -1 if none of the equivalents was found
        /// </summary>
        /// <param name="arguments">array of the arguments</param>
        /// <param name="isCaseSensitive">if true, the equivalents are compared case sensitively</param>
        /// <param name="parameterEquivalents">list of parameter/argument equivalent to look for</param>
        /// <returns></returns>
        public static int ArgumentPresent(string[] arguments, bool isCaseSensitive, params string[] parameterEquivalents)
        {
            if (null == arguments ||
                arguments.Length == 0)
                return -1;

            if (null == parameterEquivalents ||
                parameterEquivalents.Length == 0)
                return -1;

            int i = 0;
            foreach (string strArgument in arguments)
            {
                foreach (string strParam in parameterEquivalents)
                {
                    if (isCaseSensitive)
                    {
                        if (strParam == strArgument)
                            return i;
                    }
                    else
                    {
                        if (strParam.ToLower() == strArgument.ToLower())
                            return i;
                    }
                }

                i++;
            }

            return -1;
        }


        /// <summary>
        /// returns true, if any of the paramater equivalents is found in the array of arguments;
        /// returns false, if not found or if the input arguments are incorrect
        /// </summary>
        /// <param name="arguments">array of the arguments</param>
        /// <param name="isCaseSensitive">if true, the equivalents are compared case sensitively</param>
        /// <param name="parameterEquivalents">list of parameter/argument equivalent to look for</param>
        public static bool IsArgumentPresent(string[] arguments, bool isCaseSensitive, params string[] parameterEquivalents)
        {
            return (-1 != ArgumentPresent(arguments, isCaseSensitive, parameterEquivalents));
        }

    }
}
