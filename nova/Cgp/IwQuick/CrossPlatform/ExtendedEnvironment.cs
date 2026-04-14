#if COMPACT_FRAMEWORK
using Microsoft.Win32;
#endif
using System;

namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtendedEnvironment
    {
#if COMPACT_FRAMEWORK
        private const string _implicitComputerName = "WINCE_Machine";
#endif
        /// <summary>
        /// 
        /// </summary>
        public static string ComputerName
        {
            get
            {
#if COMPACT_FRAMEWORK
                RegistryKey reg = null;

                try
                {
                    reg = Registry.LocalMachine.OpenSubKey("Ident");
                    object tmp = reg.GetValue("Name", "");
                    if (tmp is string)
                    {
                        if (tmp.ToString().Length == 0)
                            return _implicitComputerName;
                        else
                            return (string)tmp;
                    }
                    else
                        return _implicitComputerName;

                }
                catch
                {
                    return _implicitComputerName;
                }
                finally
                {
                    if (null != reg)
                        reg.Close();
                }
#else
                return Environment.MachineName;
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string MachineName
        {
            get
            {
#if COMPACT_FRAMEWORK
                return ComputerName;
#else
                return Environment.MachineName;
#endif
            }
        }
    }
}
