using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Contal.IwQuick;

namespace Contal.IwQuick.Parsing
{
    public class QuickParser
    {
        public static string TrimUnquot(string input)
        {
            if (null == input)
                return null;

            input = input.Trim();

            if ("" == input)
                return null;

            if ('\"' == input[0])
                input = input.Substring(1);

            if ('\"' == input[input.Length - 1])
                input = input.Substring(0, input.Length - 1);

            return input;
            
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
        /// provides spliting of a string qutoed parts, 
        /// quoted parts are transcripted as one item of array, even if they contain whitespace
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
                        if (i==0 || ' '!=input[i-1])
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

            string[] arRet = new string[aArray.Count+1];
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

        public static int CountIndexOf(string input, string i_strPatternToSearch)
        {
            if (Validator.IsNullString(input))
                return 0;

            if (Validator.IsNullString(i_strPatternToSearch))
                return 0;

            int iStartIndex = 0;
            int iCount = 0;
            while (true)
            {
                iStartIndex = input.IndexOf(i_strPatternToSearch, iStartIndex);
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

        public static int ArgumentPresent(string[] i_arArguments,bool i_bCaseSensitive,params string[] i_arParameterEquivalents)
        {
            if (null == i_arArguments ||
                i_arArguments.Length == 0)
                return -1;

            if (null == i_arParameterEquivalents ||
                i_arParameterEquivalents.Length == 0)
                return -1;

            int i = 0;
            foreach (string strArgument in i_arArguments)
            {
                foreach (string strParam in i_arParameterEquivalents)
                {
                    if (i_bCaseSensitive)
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

        public static bool IsArgumentPresent(string[] i_arArguments, bool i_bCaseSensitive, params string[] i_arParameterEquivalents)
        {
            return (-1 != ArgumentPresent(i_arArguments, i_bCaseSensitive, i_arParameterEquivalents));
        }

        public static string Combine(String[] array,string separator)
        {
            if (null == array || array.Length == 0)
                return String.Empty;

            if (null == separator)
                separator = String.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (sb.Length > 0)
                    sb.Append(separator);

                sb.Append(array[i]);
            }

            return sb.ToString();
        }
    }
}
