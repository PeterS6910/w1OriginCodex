using System;
using System.Globalization;
using System.Linq;

namespace Contal.IwQuick.CrossPlatform
{
    public static class IntegerExtensions
    {
        public static bool IsInt(string s)
        {
            return s.All(char.IsNumber);
        }

        public static bool TryParseInt(string s, NumberStyles numberStyles, out int value)
        {
            value = 0;

            try
            {
                value = int.Parse(s, numberStyles);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
