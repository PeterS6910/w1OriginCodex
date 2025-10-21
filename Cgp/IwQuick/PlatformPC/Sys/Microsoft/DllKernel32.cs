using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class DllKernel32
    {
        public const string DLL_PATH = "Kernel32.dll";

        public enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified,
            ComputerNameMax
        }

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        public delegate bool DCtrlEventHandler(CtrlType sig);

        [DllImport(DLL_PATH)]
        public static extern bool SetConsoleCtrlHandler(DCtrlEventHandler handler, bool add);

        [DllImport(DLL_PATH, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetComputerNameEx(
            COMPUTER_NAME_FORMAT nameType,
            [MarshalAs(UnmanagedType.LPTStr)] string lpBuffer);


        public struct MemoryStatus
        {
            public uint Length;
            public uint MemoryLoad;
            public uint TotalPhysical;
            public uint AvailablePhysical;
            public uint TotalPageFile;
            public uint AvailablePageFile;
            public uint TotalVirtual;
            public uint AvailableVirtual;
        }

        [DllImport(DLL_PATH)]
        public static extern void GlobalMemoryStatus(out MemoryStatus stat);

        [DllImport(DLL_PATH)]
        public static extern uint GetCurrentThreadId();

        [Flags]
        public enum ThreadSecurityRights : uint
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            SYNCHRONIZE = 0x00100000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,

            THREAD_DIRECT_IMPERSONATION = 0x0200,
            THREAD_GET_CONTEXT = 0x0008,
            THREAD_IMPERSONATE = 0x0100,

            THREAD_QUERY_INFORMATION = 0x0040,
            THREAD_QUERY_LIMITED_INFORMATION =0x0800,

            THREAD_SET_CONTEXT  = 0x0010,
            THREAD_SET_INFORMATION = 0x0020,
            THREAD_SET_LIMITED_INFORMATION = 0x0400,
            THREAD_SET_THREAD_TOKEN = 0x0080,
            THREAD_SUSPEND_RESUME = 0x0002,
            THREAD_TERMINATE = 0x0001,

            THREAD_ALL_ACCESS = 0xFFFF

        }

        [DllImport(DLL_PATH)]
        public static extern IntPtr OpenThread(
            [In] ThreadSecurityRights dwDesiredAccess,
            [In,MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            [In] UInt32 dwThreadId
        );

        [DllImport(DLL_PATH)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateThread(
            [In,Out] IntPtr hThread,
            [In]    UInt32 dwExitCode
        );


        /// <summary>
        /// see details in http://msdn.microsoft.com/en-us/library/windows/desktop/ms724936%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [DllImport(DLL_PATH, SetLastError = true)]
        public static extern bool SetLocalTime(ref SYSTEMTIME time);

        // SYSTEMTIME structure used by SetSystemTime
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }


        [DllImport(DLL_PATH, SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME systemTime);

        [DllImport(DLL_PATH, SetLastError = false)]
        public static extern UInt32 GetLastError();
    }
}
