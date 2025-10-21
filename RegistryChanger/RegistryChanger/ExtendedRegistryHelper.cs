using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace RegistryChanger
{
    /// <summary>
    /// class for often operations with the registry
    /// </summary>
    public class ExtendedRegistryHelper
    {
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

            if (key == null || key == string.Empty)
                return null;

            int posRoot = key.IndexOf('\\');

            string rootKey = null;
            if (posRoot >= 0)
            {
                rootKey = key.Substring(0, posRoot);
                key = key.Substring(posRoot + 1);
                registrySubkey = key;
            }
            else
                rootKey = key;

            switch (rootKey.ToUpper())
            {
                case "HKLM":
                case "HKEY_LOCAL_MACHINE":
                case "HKEYLOCALMACHINE":
                case "LOCALMACHINE":
                case "LOCAL_MACHINE":
                    return Registry.LocalMachine;

                case "HKCU":
                case "HKEY_CURRENT_USER":
                case "HKEYCURRENTUSER":
                case "CURRENTUSER":
                case "CURRENT_USER":
                    return Registry.CurrentUser;

                case "HKCR":
                case "HKEY_CLASSES_ROOT":
                case "HKEYCLASSESROOT":
                case "CLASSESROOT":
                case "CLASSES_ROOT":
                    return Registry.ClassesRoot;

                case "HKU":
                case "HKEY_USERS":
                case "HKEYUSERS":
                case "USERS":
                    return Registry.Users;

                case "HKCC":
                case "HKEY_CURRENT_CONFIG":
                case "HKEYCURRENTCONFIG":
                case "CURRENTCONFIG":
                case "CURRENT_CONFIG":
                    return Registry.CurrentConfig;

                case "HKDD":
                case "HKEY_DYN_DATA":
                case "HKEYDYNDATA":
                case "DYNDATA":
                case "DYN_DATA":
                    return Registry.CurrentConfig;

                case "HKPD":
                case "HKEY_PERFORMANCE_DATA":
                case "HKEYPERFORMANCEDATA":
                case "PERFORMANCEDATA":
                case "PERFORMANCE_DATA":
                    return Registry.PerformanceData;

                default:
                    return null;
            }
        }

        /// <summary>
        /// tries to parse the registry key from it's string representation; 
        /// returns true, if parsing succeeded
        /// </summary>
        /// <param name="key">string interpretation of the registry key's path</param>
        /// <param name="readOnly">if the key should be opened as readonly</param>
        /// <param name="registryKey">registry key instance after parsing, or null if unsuccessful</param>
        /// <returns></returns>
        public static bool TryParseKey(string key, bool readOnly, out RegistryKey registryKey)
        {
            registryKey = null;

            if (key == null || key == string.Empty)
                return false;

            string subKey;

            RegistryKey rootKey = ParseHive(key, out subKey);
            if (null == rootKey)
                return false;

            if (subKey != null)
                try
                {
                    RegistryKey rk = rootKey.OpenSubKey(subKey, !readOnly);

                    if (null != rk)
                    {
                        registryKey = rk;
                        return true;
                    }
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            else
            {
                registryKey = rootKey;
                return true;
            }
        }

        /// <summary>
        /// tries to retrieve the registry key or add it, when it does not exists
        /// </summary>
        /// <param name="key">string addressation of the key</param>
        /// <exception cref="ArgumentNullException">if the key is null or empty</exception>
        /// <returns>registry key wrapper instance or null, if the key cannot be found</returns>
        public static RegistryKey GetOrAddKey(string key)
        {
            return GetOrAddKey(key, true);
        }

        /// <summary>
        /// tries to retrieve the registry key or add it, when it does not exists (writable=true)
        /// </summary>
        /// <param name="key">string addressation of the key</param>
        /// <param name="writable">if false, and the key is not to be found, it's NOT aimed to be created</param>
        /// <exception cref="ArgumentNullException">if the key is null or empty</exception>
        /// <returns>registry key wrapper instance or null, if the key cannot be found</returns>
        public static RegistryKey GetOrAddKey(string key, bool writable)
        {
            string subKey = null;
            try
            {
                RegistryKey rootKey = ParseHive(key, out subKey);
                if (null == rootKey)
                    return null;

                RegistryKey rk = null;

                if (subKey != null)
                {
                    try
                    {
                        rk = rootKey.OpenSubKey(subKey, writable);

                        if (null != rk)
                        {
                            return rk;
                        }
                        else
                        {
                            if (writable)
                            {
                                try
                                {
                                    return rootKey.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                                }
                                catch { }
                            }
                        }

                    }
                    catch (System.Security.SecurityException)
                    {
                        // the key most probably does exist, but the requested mode does not allow access to it
                        try
                        {
                            rk = rootKey.OpenSubKey(subKey, false);
                            return rk;
                        }
                        catch { }
                    }
                    catch { }
                }
            }
            catch { }

            return null;
        }

        public static bool ExistsKey(string key)
        {
            string subKey = null;
            try
            {
                RegistryKey rootKey = ParseHive(key, out subKey);

                if (null == rootKey)
                    return false;

                RegistryKey rk = null;

                if (subKey != null)
                {
                    rk = rootKey.OpenSubKey(subKey);

                    if (rk != null)
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// returns the value at a specific key, and tries to add it when possible;
        /// if anything went wrong, or if the value does not exist and the value was not specified, null is returned
        /// </summary>
        /// <param name="key">string addresation of the registru key</param>
        /// <param name="valueName">name of the value</param>
        /// <param name="defaultValue">implicit value to store into value, if not present</param>
        /// <exception cref="DoesNotExistException">if the registry key was not able to be created</exception>
        /// <exception cref="ArgumentNullException">if the key or the valueName is null or empty</exception>
        /// <returns></returns>
        public static object GetOrAddValue(string key, string valueName, object defaultValue)
        {
            return GetOrAddValue(key, valueName, true, defaultValue);
        }

        /// <summary>
        /// returns the value at a specific key, and tries to add it when possible;
        /// if anything went wrong, or if the value does not exist and the value was not specified, null is returned
        /// </summary>
        /// <param name="key">string addresation of the registru key</param>
        /// <param name="valueName">name of the value</param>
        /// <param name="parentWritable">if false, and the parent key nor valueName does not exist, it won't try to create them</param>
        /// <param name="defaultValue">implicit value to store into value, if not present</param>
        /// <exception cref="DoesNotExistException">if the registry key was not able to be created</exception>
        /// <exception cref="ArgumentNullException">if the key or the valueName is null or empty</exception>
        /// <returns></returns>
        public static object GetOrAddValue(string key, string valueName, bool parentWritable, object defaultValue)
        {
            RegistryKey rk = GetOrAddKey(key, parentWritable);
            if (null == rk)
                return null;

            object value = rk.GetValue(valueName);
            if (value == null &&
                parentWritable &&
                null != defaultValue)
            {
                rk.SetValue(valueName, defaultValue);
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// retrieves value of the specified key
        /// </summary>
        /// <param name="key">string adresation of the registry key</param>
        /// <param name="valueName">name of the value</param>
        /// <exception cref="ArgumentNullException">if the key or valueName is null or empty</exception>
        /// <returns></returns>
        public static object GetValue(string key, string valueName)
        {
            RegistryKey rk = GetOrAddKey(key);
            if (null == rk)
                return null;

            return rk.GetValue(valueName);
        }

        /// <summary>
        /// Renames a subkey of the passed in registry key since 
        /// the Framework totally forgot to include such a handy feature.
        /// </summary>
        /// <param name="regKey">The RegistryKey that contains the subkey 
        /// you want to rename (must be writeable)</param>
        /// <param name="subKeyName">The name of the subkey that you want to rename
        /// </param>
        /// <param name="newSubKeyName">The new name of the RegistryKey</param>
        /// <returns>True if succeeds</returns>
        public static bool RenameSubKey(RegistryKey parentKey,
            string subKeyName, string newSubKeyName)
        {
            CopyKey(parentKey, subKeyName, newSubKeyName);
            parentKey.DeleteSubKeyTree(subKeyName);
            return true;
        }

        /// <summary>
        /// Copy a registry key.  The parentKey must be writeable.
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="keyNameToCopy"></param>
        /// <param name="newKeyName"></param>
        /// <returns></returns>
        public static bool CopyKey(RegistryKey parentKey,
            string keyNameToCopy, string newKeyName)
        {
            //Create new key
            RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName);

            //Open the sourceKey we are copying from
            RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy);

            RecurseCopyKey(sourceKey, destinationKey);

            return true;
        }

        public static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            //copy all the values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object objValue = sourceKey.GetValue(valueName);
                RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, objValue, valKind);
            }

            //For Each subKey 
            //Create a new subKey in destinationKey 
            //Call myself 
            foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName);
                RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
                RecurseCopyKey(sourceSubKey, destSubKey);
            }
        }
    }
}
