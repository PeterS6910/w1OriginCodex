using System.Collections.Generic;
using Microsoft.Win32;

namespace Contal.IwQuick.Sys.Microsoft
{
    public partial class RegistryHelper
    {

        private static Dictionary<string, RegistryKey> _hiveTranslation;

        

        static RegistryHelper()
        {
            InitHiveTranslation();
        }

        /// <summary>
        /// parses the RegistryKey instance of the hive and the rest,
        /// from the string presentation of the registry;
        /// returns null, if the parsing is not successful;
        /// hive names are allowed in usual forms as HKLM, HKEY_LOCAL_MACHINE, etc.
        /// </summary>
        /// <param name="key">string intepretation of the registry key</param>
        /// <param name="registrySubkey">subkey, if parsing successful</param>
        /// <returns></returns>
        public static RegistryKey ParseHive(string key, out string registrySubkey)
        {
            registrySubkey = null;

            if (Validator.IsNullString(key))
                return null;

            int posRoot = key.IndexOf('\\');

            string rootKey;
            if (posRoot >= 0)
            {
                rootKey = key.Substring(0, posRoot);
                key = key.Substring(posRoot + 1);
                registrySubkey = key;
            }
            else
                rootKey = key;

            RegistryKey resultRk;
            _hiveTranslation.TryGetValue(rootKey.ToUpper(), out resultRk);

            return resultRk;
        }

    }
}
