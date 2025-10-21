using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// email address helper
    /// </summary>
    public class EmailAddress
    {
        public static bool IsValid(string emailAddress)
        {
            if (Validator.IsNullString(emailAddress))
                return false;

            try
            {
                Regex aRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,4}$");
                return (aRegex.IsMatch(emailAddress));
                    
            }
            catch
            {
                return false;
            }
        }

        public static void CheckValid(string emailAddress)
        {
            if (!IsValid(emailAddress))
                throw new ArgumentException("Invalid email address \""+emailAddress+"\"");
        }
    }
}
