using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;

namespace Contal.IwQuick.Parsing
{
    public class EscapeString
    {

        private static readonly IDictionary<string, string> _forwardReplaceDictionary
            = new Dictionary<string, string>();


        private const string _regexEscapes = @"[\a\b\f\n\r\t\v\\""]";

        public static string Do(string input)
        {
            return Regex.Replace(input, _regexEscapes, RegexMatch);
        }

        public static string Undo(string input)
        {
            if (null == input)
                return String.Empty;

            foreach (KeyValuePair<string, string> pair in _forwardReplaceDictionary)
            {
                input = input.Replace(pair.Value, pair.Key);
            }

            return input;
        }

        public static string StringLiteral(string input)
        {
            return Do(input);
        }

        public static string CharLiteral(char c)
        {
            return c == '\'' ? @"'\''" : string.Format("'{0}'", c);
        }

        private static string RegexMatch(Match m)
        {
            string match = m.ToString();
            if (_forwardReplaceDictionary.ContainsKey(match))
            {
                return _forwardReplaceDictionary[match];
            }

            throw new NotSupportedException();
        }

        static EscapeString()
        {
            _forwardReplaceDictionary.Add("\a", @"\a");
            _forwardReplaceDictionary.Add("\b", @"\b");
            _forwardReplaceDictionary.Add("\f", @"\f");
            _forwardReplaceDictionary.Add("\n", @"\n");
            _forwardReplaceDictionary.Add("\r", @"\r");
            _forwardReplaceDictionary.Add("\t", @"\t");
            _forwardReplaceDictionary.Add("\v", @"\v");

            _forwardReplaceDictionary.Add("\\", @"\\");
            _forwardReplaceDictionary.Add("\0", @"\0");

            //The SO parser gets fooled by the verbatim version 
            //of the string to replace - @"\"""
            //so use the 'regular' version
            _forwardReplaceDictionary.Add("\"", "\\\"");
        }
    }

}
