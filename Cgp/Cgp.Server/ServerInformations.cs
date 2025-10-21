using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

using Contal.Cgp.ORM;

namespace Contal.Cgp.Server
{
    /// <summary>
    /// Enum for server informationTypes
    /// </summary>
    public enum ServerInformationType : int
    {
        VersionOfOperatingSystem,
        DotNetVersion,
        VersionOfDatabaseSever,
        CPUCoreCount,
        TotalMemory,
        FreeMemory,
        AcutalServerDataTime
    }

    public static class ServerInformations
    {
        /// <summary>
        /// Function return all server informations in the dictionary
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> GetServerInformations()
        {
            Dictionary<string, object> serverInformations = new Dictionary<string, object>();

            // Version of operating system
            serverInformations.Add(ServerInformationType.VersionOfOperatingSystem.ToString(), GetOSName());

            // .Net version
            serverInformations.Add(ServerInformationType.DotNetVersion.ToString(), Environment.Version.ToString());

            //Version of database server
            serverInformations.Add(ServerInformationType.VersionOfDatabaseSever.ToString(), NhHelper.Singleton.GetVersionOfDatabaseServer());

            // CPU core count
            serverInformations.Add(ServerInformationType.CPUCoreCount.ToString(), Environment.ProcessorCount.ToString());

            // Total memory
            serverInformations.Add(ServerInformationType.TotalMemory.ToString(), PerformanceInfo.GetTotalMemory().ToString() + " MB");

            // Free memory
            serverInformations.Add(ServerInformationType.FreeMemory.ToString(), PerformanceInfo.GetAvailableMemory().ToString() + " MB");

            // Actual server's date and time
            serverInformations.Add(ServerInformationType.AcutalServerDataTime.ToString(), DateTime.Now);

            return serverInformations;
        }

        /// <summary>
        /// Return oparating system friendly name
        /// </summary>
        /// <returns></returns>
        private static string GetOSName()
        {
            System.OperatingSystem os = System.Environment.OSVersion;
            string osName = string.Empty;
            // In order to target Windows 8.1 or Windows 10, you need to include the app.manifest: https://msdn.microsoft.com/en-us/library/windows/desktop/dn481241(v=vs.85).aspx 

            // https://stackoverflow.com/questions/2819934/detect-windows-version-in-net
            switch (os.Platform)
            {
                case System.PlatformID.Win32Windows:
                    switch (os.Version.Minor)
                    {
                        case 0:
                            osName = "Windows 95";
                            break;
                        case 10:
                            osName = "Windows 98";
                            break;
                        case 90:
                            osName = "Windows ME";
                            break;
                    }
                    break;

                case System.PlatformID.Win32NT:
                    switch (os.Version.Major)
                    {
                        case 3:
                            osName = "Windws NT 3.51";
                            break;
                        case 4:
                            osName = "Windows NT 4";
                            break;
                        case 5:
                            if (os.Version.Minor == 0)
                                osName = "Windows 2000";
                            else if (os.Version.Minor == 1)
                                osName = "Windows XP";
                            else if (os.Version.Minor == 2)
                                osName = "Windows Server 2003";
                            break;
                        case 6:
                            if (os.Version.Minor == 0)
                                osName = "Windows Vista / Windows Server 2008";
                            else if (os.Version.Minor == 1)
                                osName = "Windows 7 / Windows 2008 R2";
                            else if (os.Version.Minor == 2)
                                osName = "Windows 8";
                            else if (os.Version.Minor == 3)
                                osName = "Windows 8.1";
                            break;
                        case 10:
                            if (os.Version.Minor == 0)
                                osName = "Windows 10";
                            else
                                osName = "Windows 10 (Unrecognized version)";
                            break;
                    }
                    break;
            }

            if (osName != string.Empty)
                return osName + ", " + os.ServicePack;
            else
                return os.ToString();
        }
    }

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        /// <summary>
        /// Return avialble memory (MB)
        /// </summary>
        /// <returns></returns>
        public static Int64 GetAvailableMemory()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }

        /// <summary>
        /// Return total memory (MB)
        /// </summary>
        /// <returns></returns>
        public static Int64 GetTotalMemory()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }
    }
}
