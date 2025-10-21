using System;

namespace Contal.Cgp.Globals
{
    public class PasswordHelper
    {
        public static string GetPasswordhash(string username, string password)
        {
            return IwQuick.Crypto.QuickHashes.GetSHA256String((username ?? String.Empty) + ":" + password);
        }
    }
}
