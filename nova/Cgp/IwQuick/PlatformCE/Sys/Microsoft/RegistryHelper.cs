using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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

                // These are not supported on CF

                //{"HKCC", Registry.CurrentConfig},
                //{"HKEY_CURRENT_CONFIG", Registry.CurrentConfig},
                //{"HKEYCURRENTCONFIG", Registry.CurrentConfig},
                //{"CURRENTCONFIG", Registry.CurrentConfig},
                //{"CURRENT_CONFIG", Registry.CurrentConfig},


                //{"HKDD", Registry.CurrentConfig},
                //{"HKEY_DYN_DATA", Registry.CurrentConfig},
                //{"HKEYDYNDATA", Registry.CurrentConfig},
                //{"DYNDATA", Registry.CurrentConfig},
                //{"DYN_DATA", Registry.CurrentConfig},


                //{"HKPD", Registry.PerformanceData},
                //{"HKEY_PERFORMANCE_DATA", Registry.PerformanceData},
                //{"HKEYPERFORMANCEDATA", Registry.PerformanceData},
                //{"PERFORMANCEDATA", Registry.PerformanceData},
                //{"PERFORMANCE_DATA", Registry.PerformanceData}

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
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static RegistryKey GetKey(string key,bool readOnly)
        {
            if (Validator.IsNullString(key))
                return null;

            RegistryKey rk;
            if (!TryParseKey(key, readOnly, out rk))
                return null;
            return rk;
        }


        /// <summary>
        /// tries to retrieve the registry key or add it, when it does not exists
        /// </summary>
        /// <param name="key">string addressation of the key</param>
        /// <returns></returns>
        public static RegistryKey GetOrAddKey([NotNull] string key)
        {
            Validator.CheckNullString(key);

            string subKey;

            RegistryKey rootKey = ParseHive(key, out subKey);
            if (null == rootKey)
                return null;

            if (subKey != null)
                try
                {
                    RegistryKey rk = rootKey.OpenSubKey(subKey,true);

                    if (null != rk)
                    {
                        return rk;
                    }
                    try
                    {
                        return rootKey.CreateSubKey(subKey);
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

        private static bool _autoFlush = true;
        /// <summary>
        /// 
        /// </summary>
        public static bool AutoFlush
        {
            get { return _autoFlush; }
            set { _autoFlush = value; }
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
            Validator.CheckNullString(key);
            Validator.CheckNullString(valueName);

            RegistryKey rk = GetOrAddKey(key);
            if (null == rk)
                throw new DoesNotExistException(key);

            object value = rk.GetValue(valueName);
            if (value == null && null != defaultValue)
            {
                rk.SetValue(valueName, defaultValue);

                if (_autoFlush)
                    Close(rk);
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetOrAddValue<T>(
            [NotNull] RegistryKey registryKey, 
            [NotNull] string valueName, 
            T defaultValue)
        {
            Validator.CheckForNull(registryKey,"registryKey");
            Validator.CheckNullString(valueName);

            object value = registryKey.GetValue(valueName);
            if (value == null)
            {
                if (!ReferenceEquals(defaultValue,null))
                {
                    if (defaultValue is bool) {
                        object o = defaultValue;

                        registryKey.SetValue(valueName, ((bool)o ? 1 : 0), RegistryValueKind.DWord);
                    }
                    else
                        registryKey.SetValue(valueName, defaultValue);

                    value = defaultValue;
                }
            }
            else
            {
                if (!ReferenceEquals(defaultValue, null))
                {
                    if ((defaultValue is bool) && (value is int))
                        value = ((int)value != 0);                        
                }
            }


            return (T) value;
        }

        /// <summary>
        /// retrieves value of the specified key
        /// </summary>
        /// <param name="key">string adresation of the registry key</param>
        /// <param name="valueName">name of the value</param>
        /// <returns></returns>
        public static object GetValue(string key, string valueName)
        {
            if (Validator.IsNullString(key))
                return null;

            RegistryKey rk = GetOrAddKey(key);
            if (null == rk)
                return null;

            object o;
            try
            {
                o = rk.GetValue(valueName);
            }
            catch
            {
                o = null;
            }
            finally
            {
                try { rk.Close(); }
                catch { }
            }

            return o;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static object GetValue(RegistryKey key, string valueName)
        {
            if (null == key)
                return null;


            object o;
            try
            {
                o = key.GetValue(valueName);
            }
            catch
            {
                o = null;
            }

            return o;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryGetValue<T>(RegistryKey key, string valueName, ref T value)
        {
            try
            {
                object o = GetValue(key, valueName);
                if (o != null)
                    try
                    {
                        if (o is T)
                        {

                            value = (T)o;
                            return true;
                        }
                        return false;
                    }
                    catch
                    {
                        return false;
                    }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool TryGetValue<T>(string key, string valueName, ref T value)
        {
            try
            {
                object o = GetValue(key, valueName);
                if (o != null)
                    try
                    {
                        if (o is T)
                        {

                            value = (T)o;
                            return true;
                        }
                        return false;
                    }
                    catch
                    {
                        return false;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public static bool TryGetValue(RegistryKey key, string valueName, ref UInt32 value)
        {
            return TryGetValue<UInt32>(key, valueName, ref value);
        }

        public static bool TryGetValue(RegistryKey key, string valueName, ref UInt64 value)
        {
            return TryGetValue<UInt64>(key, valueName, ref value);
        }

        public static bool TryGetValue(RegistryKey key, string valueName, ref string value)
        {
            return TryGetValue<string>(key, valueName, ref value);
        }

        public static bool TryGetValue(RegistryKey key, string valueName, ref string[] value)
        {
            return TryGetValue<string[]>(key, valueName, ref value);
        }

        public static bool TryGetValue(string key, string valueName, ref UInt32 value)
        {
            return TryGetValue<UInt32>(key, valueName, ref value);
        }

        public static bool TryGetValue(string key, string valueName, ref UInt64 value)
        {
            return TryGetValue<UInt64>(key, valueName, ref value);
        }

        public static bool TryGetValue(string key, string valueName, ref string value)
        {
            return TryGetValue<string>(key, valueName, ref value);
        }

        public static bool TryGetValue(string key, string valueName, ref string[] value)
        {
            return TryGetValue<string[]>(key, valueName, ref value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool SetValue(string key, string valueName, object value, RegistryValueKind valueType)
        {
            Validator.CheckNullString(key);

            RegistryKey rk = GetOrAddKey(key);
            if (null == rk)
                return false;

            bool rVal = false;

            try
            {
                rk.SetValue(valueName, value, valueType);
                rVal = true;

                if (_autoFlush)
                    try { rk.Flush(); }
                    catch { }
            }
            catch
            {
            }
            finally
            {
                if (_autoFlush)
                {
                    try { rk.Close(); }
                    catch { }
                }
            }

            return rVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool TrySetValue(string key, string valueName, object value, RegistryValueKind valueType)
        {
            try
            {
                return SetValue(key, valueName, value, valueType);
            }
            catch
            {
                return false;
            }
        }

        public static bool SetValue(string key, string valueName, string value)
        {
            return SetValue(key, valueName, value, RegistryValueKind.String);
        }

        public static bool SetValue(string key, string valueName, string[] value)
        {
            return SetValue(key, valueName, value, RegistryValueKind.MultiString);
        }

        public static bool SetValue(string key, string valueName, UInt32 value)
        {
            return SetValue(key, valueName, value, RegistryValueKind.DWord);
        }

        public static bool SetValue(string key, string valueName, Int32 value)
        {
            return SetValue(key, valueName, (UInt32)value, RegistryValueKind.DWord);
        }

        public static bool SetValue(string key, string valueName, UInt64 value)
        {
            return SetValue(key, valueName, value, RegistryValueKind.QWord);
        }

        public static bool SetValue(string key, string valueName, Int64 value)
        {
            return SetValue(key, valueName, (UInt64)value, RegistryValueKind.QWord);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string[] GetSubkeys(string key)
        {
            if (Validator.IsNullString(key))
                return null;

            RegistryKey rk;
            if (!TryParseKey(key, true, out rk))
                return null;
            try
            {
                return rk.GetSubKeyNames();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ensures closing and flushing of the registry key
        /// </summary>
        /// <param name="registryKeys"></param>
        public static void Close(params RegistryKey[] registryKeys)
        {
            if (null == registryKeys || registryKeys.Length <=0)
                return;

            foreach (RegistryKey rk in registryKeys)
            {
                if (rk != null)
                    try
                    {
                        rk.Close();
                    }
                    catch
                    {
                    }
            }
        }


        
    }

    /// <summary>
    /// 
    /// </summary>
    public static class RegistryKeyExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(this RegistryKey registryKey,string valueName,T defaultValue)
        {
            Validator.CheckForNull(registryKey,"registryKey");

            var result = registryKey.GetValue(valueName, defaultValue);

            return (T)result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetBoolValue(
            [NotNull] this RegistryKey registryKey, 
            [NotNull] string valueName, 
            bool defaultValue)
        {
            Validator.CheckForNull(registryKey, "registryKey");

            var result = registryKey.GetValue(valueName, defaultValue);
            if (!(result is int))
                return false;

            return (int)result > 0;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registryKey"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        public static void SetValue<T>(this RegistryKey registryKey, string valueName, T value)
        {
            Validator.CheckForNull(registryKey, "registryKey");

            registryKey.SetValue(valueName,value);

        }
    }


}
