using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Contal.IwQuick.Sys.Microsoft
{
    /// <summary>
    /// class for often operations with the registry
    /// </summary>
    public partial class RegistryHelper
    {
        private static void InitHiveTranslation()
        {
            _hiveTranslation = new Dictionary<string, RegistryKey>()
            {
                {"HKLM", Registry.LocalMachine},
                {"HKEY_LOCAL_MACHINE", Registry.LocalMachine},
                {"HKEYLOCALMACHINE", Registry.LocalMachine},
                {"LOCALMACHINE", Registry.LocalMachine},
                {"LOCAL_MACHINE", Registry.LocalMachine},

                {"HKCU", Registry.CurrentUser},
                {"HKEY_CURRENT_USER", Registry.CurrentUser},
                {"HKEYCURRENTUSER", Registry.CurrentUser},
                {"CURRENTUSER", Registry.CurrentUser},
                {"CURRENT_USER", Registry.CurrentUser},


                {"HKCR", Registry.ClassesRoot},
                {"HKEY_CLASSES_ROOT", Registry.ClassesRoot},
                {"HKEYCLASSESROOT", Registry.ClassesRoot},
                {"CLASSESROOT", Registry.ClassesRoot},
                {"CLASSES_ROOT", Registry.ClassesRoot},

                {"HKU", Registry.Users},
                {"HKEY_USERS", Registry.Users},
                {"HKEYUSERS", Registry.Users},
                {"USERS", Registry.Users},


                {"HKCC", Registry.CurrentConfig},
                {"HKEY_CURRENT_CONFIG", Registry.CurrentConfig},
                {"HKEYCURRENTCONFIG", Registry.CurrentConfig},
                {"CURRENTCONFIG", Registry.CurrentConfig},
                {"CURRENT_CONFIG", Registry.CurrentConfig},


                {"HKDD", Registry.CurrentConfig},
                {"HKEY_DYN_DATA", Registry.CurrentConfig},
                {"HKEYDYNDATA", Registry.CurrentConfig},
                {"DYNDATA", Registry.CurrentConfig},
                {"DYN_DATA", Registry.CurrentConfig},


                {"HKPD", Registry.PerformanceData},
                {"HKEY_PERFORMANCE_DATA", Registry.PerformanceData},
                {"HKEYPERFORMANCEDATA", Registry.PerformanceData},
                {"PERFORMANCEDATA", Registry.PerformanceData},
                {"PERFORMANCE_DATA", Registry.PerformanceData}

            };
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

            if (Validator.IsNullString(key))
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
                    
                    return false;
                }
                catch
                {
                    return false;
                }
            
            registryKey = rootKey;
            return true;
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
        public static RegistryKey GetOrAddKey(string key,bool writable)
        {
            Validator.CheckNullString(key);

            string subKey;

            RegistryKey rootKey = ParseHive(key, out subKey);
            if (null == rootKey)
                return null;

            RegistryKey rk;

            if (subKey != null)
                try
                {
                    rk = rootKey.OpenSubKey(subKey,writable);

                    if (null != rk)
                    {
                        return rk;
                    }
                    
                    if (writable)
                        try
                        {
                            return rootKey.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        }
                        catch
                        {
                            return null;
                        }
                    
                    return null;
                }
                catch (System.Security.SecurityException)
                {
                    // the key most probably does exist, but the requested mode does not allow access to it
                    try
                    {
                        rk = rootKey.OpenSubKey(subKey, false);
                        return rk;
                    }
                    catch
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            
            return null;
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
            Validator.CheckNullString(key);
            Validator.CheckNullString(valueName);

            RegistryKey rk = GetOrAddKey(key,parentWritable);
            if (null == rk)
                throw new DoesNotExistException(key);

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
            Validator.CheckNullString(key);
            Validator.CheckNullString(valueName);

            RegistryKey rk = GetOrAddKey(key);
            if (null == rk)
                return null;

            return rk.GetValue(valueName);
        }
    }
}
