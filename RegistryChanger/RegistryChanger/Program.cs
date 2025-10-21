using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace RegistryChanger
{
    class Program
    {
        //private const string DEFAULT_REGISTRY_GENERAL_SETTINGS_PATH = @"HKLM\Software\Contal\Cgp\Server\General";
        //private const string WOW6432NODE_REGISTRY_GENERAL_SETTINGS_PATH = @"HKLM\Software\Wow6432Node\Contal\Cgp\Server\General";
        //private const string TIMESTAMP_VALUE_NAME = "TimeStamp";

        static void Main(string[] args)
        {
            string defaultPath = args[0];
            string wow6432NodePath = args[1];
            string timeStampName = args[2];

            if (DuplicatedRegistryValues(defaultPath, wow6432NodePath))
            {
                CompareTimeStamps(defaultPath, wow6432NodePath, timeStampName);
            }
        }

        private static void SaveRegistryTimeStamp(string keyPath, string timeStampName)
        {
            try
            {
                RegistryKey key;
                key = ExtendedRegistryHelper.GetOrAddKey(keyPath);

                key.SetValue(timeStampName, DateTime.Now.ToUniversalTime().ToString());
                key.Close();
            }
            catch { }
        }

        private static bool DuplicatedRegistryValues(string defaultPath, string wow6432NodePath)
        {
            if (ExtendedRegistryHelper.ExistsKey(defaultPath) && ExtendedRegistryHelper.ExistsKey(wow6432NodePath))
            {
                return true;
            }

            return false;
        }

        private static void GetRegistryTimeStamps(string defaultPath, string wow6432NodePath, string timeStampName,
            ref  DateTime? timeStampDefaultDateTime, ref DateTime? timeStampWow6432NodeDateTime)
        {
            try
            {
                timeStampDefaultDateTime = Convert.ToDateTime(ExtendedRegistryHelper.GetValue(defaultPath, timeStampName));
                if (timeStampDefaultDateTime != null) Console.WriteLine(timeStampDefaultDateTime.ToString());

            }
            catch { }

            try
            {
                timeStampWow6432NodeDateTime = Convert.ToDateTime(ExtendedRegistryHelper.GetValue(wow6432NodePath, timeStampName));
                if (timeStampWow6432NodeDateTime != null) Console.WriteLine(timeStampWow6432NodeDateTime.ToString());
            }
            catch { }
        }

        private static void CompareTimeStamps(string defaultPath, string wow6432NodePath, string timeStampName)
        {
            DateTime? timeStampDefaultDateTime = null;
            DateTime? timeStampWow6432NodeDateTime = null;

            int result = int.MinValue;
            try
            {
                GetRegistryTimeStamps(defaultPath, wow6432NodePath, timeStampName, ref timeStampDefaultDateTime, ref timeStampWow6432NodeDateTime);
                result = DateTime.Compare(timeStampDefaultDateTime.Value, timeStampWow6432NodeDateTime.Value);
            }
            catch { }

            if (result != int.MinValue)//succeeded to read or compare timeStamps
            {
                if (result < 0)
                {
                    CopyRegistryValues(defaultPath, wow6432NodePath);
                    DeleteGeneralSettingsKey(defaultPath);
                }
                else if (result > 0)
                {
                    CopyRegistryValues(wow6432NodePath, defaultPath);
                    DeleteGeneralSettingsKey(wow6432NodePath);
                }
            }
        }

        private static string GetParentPath(string keyPath, out string subKeyName)
        {
            int index = keyPath.LastIndexOf(@"\");
            subKeyName = keyPath.Substring(index + 1, (keyPath.Length - (index + 1)));
            return keyPath.Remove(index, (keyPath.Length - index));
        }

        private static void DeleteGeneralSettingsKey(string keyPath)
        {
            try
            {
                string subKeyName = string.Empty;
                string keyParentPath = GetParentPath(keyPath, out subKeyName);

                RegistryKey key = ExtendedRegistryHelper.GetOrAddKey(keyParentPath);

                key.DeleteSubKeyTree(subKeyName);
                key.Close();
            }
            catch { }
        }

        private static void CopyRegistryValues(string fromKeyPath, string toKeyPath)
        {
            RegistryKey keyFrom = ExtendedRegistryHelper.GetOrAddKey(fromKeyPath, true);
            RegistryKey keyTo = ExtendedRegistryHelper.GetOrAddKey(toKeyPath, true);

            ExtendedRegistryHelper.RecurseCopyKey(keyFrom, keyTo);

            try
            {
                keyFrom.Close();
                keyTo.Close();
            }
            catch { }
        }
    }
}
