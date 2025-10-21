using System.Text.RegularExpressions;
using Contal.IwQuick;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// email address helper class
    /// </summary>
    public class EmailAddress
    {
        /// <summary>
        /// returns true, if the email address is according specification
        /// </summary>
        /// <param name="emailAddress"></param>
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
    }
}
