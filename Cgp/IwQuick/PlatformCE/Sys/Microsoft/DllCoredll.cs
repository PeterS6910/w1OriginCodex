using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    /// <summary>
    /// 
    /// </summary>
    public static class DllCoredll
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DllName = "coredll.dll";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="Function"></param>
        /// <param name="Method"></param>
        /// <param name="Access"></param>
        /// <returns></returns>
        public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }

        /// <summary>
        /// 
        /// </summary>
        public const UInt32 FILE_DEVICE_SERVICE = 0x00000104;

        /// <summary>
        /// 
        /// </summary>
        public const UInt32 FILE_DEVICE_NETWORK = 0x00000012;

        /// <summary>
        /// 
        /// </summary>
        public const UInt32 THREAD_ALL_ACCESS = 0x001F03FF;

        /// <summary>
        /// 
        /// </summary>
        public const UInt32 THREAD_QUERY_INFORMATION = 0x0040;

        /// <summary>
        /// 
        /// </summary>
        public const UInt32 METHOD_BUFFERED = 0;

        /// <summary>
        /// 
        /// </summary>
        public const UInt32 FILE_ANY_ACCESS = 0;

        /// <summary>
        /// 
        /// </summary>
        public const uint IOCTL_NDIS_REBIND_ADAPTER = 0x0017002e;

        /// <summary>
        /// 
        /// </summary>
        public const int MEDIA_STATE_CONNECTED = 0;
        /// <summary>
        /// 
        /// </summary>
        public const int MEDIA_STATE_DISCONNECTED = 1;
        /// <summary>
        /// 
        /// </summary>
        public const int MEDIA_STATE_UNKNOWN = -1;

        /// <summary>
        /// 
        /// </summary>
        public const int PROCESS_TERMINATE = 1;
        /// <summary>
        /// 
        /// </summary>
        public const int INVALID_HANDLE_VALUE = -1;

        /// <summary>
        /// 
        /// </summary>
        public static UInt32 IOCTL_SERVICE_CONTROL =
            CTL_CODE(FILE_DEVICE_SERVICE, 7, METHOD_BUFFERED, FILE_ANY_ACCESS);

        /// <summary>
        /// 
        /// </summary>
        public static UInt32 IOCTL_NDISUIO_NIC_STATISTICS =
            CTL_CODE(FILE_DEVICE_NETWORK, 0x209, METHOD_BUFFERED, FILE_ANY_ACCESS);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern uint GetTickCount();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern int DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            byte[] lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuffer"></param>
        /// <param name="nInBufferSize"></param>
        /// <param name="lpOutBuffer"></param>
        /// <param name="nOutBufferSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern int DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            byte[] lpInBuffer,
            int nInBufferSize,
            ref NIC_STATISTICS lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            int lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            int hTemplateFile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            [MarshalAs(UnmanagedType.U4)]
            System.IO.FileAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)]
            System.IO.FileShare dwShareMode,
            int lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)]
            System.IO.FileMode dwCreationDisposition,
            uint dwFlagsAndAttributes,
            int hTemplateFile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern int CloseHandle(IntPtr hDevice);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="dwProcessId"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, bool bInheritHandle, UInt32 dwProcessId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="bInheritHandle"></param>
        /// <param name="dwThreadId"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern IntPtr OpenThread(UInt32 dwDesiredAccess, int bInheritHandle, UInt32 dwThreadId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hThread"></param>
        /// <param name="quantum"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "CeSetThreadQuantum", SetLastError = true)]
        public static extern int SetThreadQuantum(IntPtr hThread, UInt32 quantum);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hThread"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "CeGetThreadQuantum", SetLastError = true)]
        public static extern UInt32 GetThreadQuantum(IntPtr hThread);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hThread"></param>
        /// <param name="nPriority"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "CeSetThreadPriority", SetLastError = true)]
        public static extern int SetThreadPriority(IntPtr hThread, int nPriority);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hThread"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "CeGetThreadPriority", SetLastError = true)]
        public static extern int GetThreadPriority(IntPtr hThread);

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION 
        {
            /// <summary>
            /// 
            /// </summary>
            public UIntPtr BaseAddress;
            /// <summary>
            /// 
            /// </summary>
            public UIntPtr AllocationBase;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 AllocationProtect;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 RegionSize;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 State;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 Protect;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 Type;
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lpAddress"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="dwLength"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern UInt32 VirtualQueryEx(
            IntPtr hProcess,
            UIntPtr lpAddress,
            ref MEMORY_BASIC_INFORMATION lpBuffer,
            UInt32 dwLength);

        /// <summary>
        /// 
        /// </summary>
        public enum MemStates
        {
            /// <summary>
            /// 
            /// </summary>
            MemCommit = 0x1000,
            /// <summary>
            /// 
            /// </summary>
            MemReserve = 0x2000,
            /// <summary>
            /// 
            /// </summary>
            MemFree = 0x10000
        }

        /// <summary>
        /// 
        /// </summary>
        public enum MemType
        {
            /// <summary>
            /// 
            /// </summary>
            MemPrivate = 0x20000,
            /// <summary>
            /// 
            /// </summary>
            MemMapped = 0x40000,
            /// <summary>
            /// 
            /// </summary>
            MemImage =0x1000000
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpSystemTime"></param>
        [DllImport(DllName)]
        public extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpSystemTime"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpSystemTime"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public extern static uint SetLocalTime(ref SYSTEMTIME lpSystemTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpSystemTime"></param>
        [DllImport(DllName)]
        public extern static void GetLocalTime(ref SYSTEMTIME lpSystemTime);

        [DllImport(DllName)]
        public static extern int GetModuleFileName(IntPtr hModule, IntPtr lpFileName, uint nSize);
  

        /// <summary>
        /// 
        /// </summary>
        public struct SYSTEMTIME
        {
            /// <summary>
            /// 
            /// </summary>
            public ushort wYear;
            /// <summary>
            /// 
            /// </summary>
            public ushort wMonth;
            /// <summary>
            /// 
            /// </summary>
            public ushort wDayOfWeek;
            /// <summary>
            /// 
            /// </summary>
            public ushort wDay;
            /// <summary>
            /// 
            /// </summary>
            public ushort wHour;
            /// <summary>
            /// 
            /// </summary>
            public ushort wMinute;
            /// <summary>
            /// 
            /// </summary>
            public ushort wSecond;
            /// <summary>
            /// 
            /// </summary>
            public ushort wMilliseconds;
        }

        /// <summary>
        /// 
        /// </summary>
        public struct MEMORYSTATUS
        {
            /// <summary>
            /// 
            /// </summary>
            public UInt32 dwLength;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 dwMemoryLoad;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 dwTotalPhys;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 dwAvailPhys;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 dwTotalPageFile;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 dwAvailPageFile;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 dwTotalVirtual;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 dwAvailVirtual;
        };

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NIC_STATISTICS
        {
            /// <summary>
            /// 
            /// </summary>
            public uint Size;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr ptcDeviceName;
            /// <summary>
            /// 
            /// </summary>
            public uint DeviceState;
            /// <summary>
            /// 
            /// </summary>
            public uint MediaType;
            /// <summary>
            /// 
            /// </summary>
            public uint MediaState;
            /// <summary>
            /// 
            /// </summary>
            public uint PhysicalMediaType;
            /// <summary>
            /// 
            /// </summary>
            public uint LinkSpeed;
            /// <summary>
            /// 
            /// </summary>
            public ulong PacketsSent;
            /// <summary>
            /// 
            /// </summary>
            public ulong PacktsReceived;
            /// <summary>
            /// 
            /// </summary>
            public uint InitTime;
            /// <summary>
            /// 
            /// </summary>
            public uint ConnectTime;
            /// <summary>
            /// 
            /// </summary>
            public ulong BytesSent;
            /// <summary>
            /// 
            /// </summary>
            public ulong BytesReceived;
            /// <summary>
            /// 
            /// </summary>
            public ulong DirectedBytesReceived;
            /// <summary>
            /// 
            /// </summary>
            public ulong DirectedPacketsReceived;
            /// <summary>
            /// 
            /// </summary>
            public uint PacketsReceiveErrors;
            /// <summary>
            /// 
            /// </summary>
            public uint PacketsSendErrors;
            /// <summary>
            /// 
            /// </summary>
            public uint ResetCount;
            /// <summary>
            /// 
            /// </summary>
            public uint MediaSenseConnectCount;
            /// <summary>
            /// 
            /// </summary>
            public uint MediaSenseDisconnectCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpInBuf"></param>
        /// <param name="nInBufSize"></param>
        /// <param name="lpOutBuf"></param>
        /// <param name="nOutBufSize"></param>
        /// <param name="lpBytesReturned"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public extern static uint KernelIoControl
        (
            uint dwIoControlCode,
            IntPtr lpInBuf,
            uint nInBufSize,
            IntPtr lpOutBuf,
            uint nOutBufSize,
            ref uint lpBytesReturned
        );

        /*public struct IP_ADAPTER_INDEX_MAP 

        [DllImport("Coredll.dll")]
        public extern static GetInterfaceInfo(
            IN PIP_INTERFACE_INFO pIfTable,
            OUT PULONG            dwOutBufLen
        );*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="ExitCode"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint ExitCode);



        /// <summary>
        /// 
        /// </summary>
        public struct FILETIME
        {
            /// <summary>
            /// 
            /// </summary>
            public uint dwLowDateTime;
            /// <summary>
            /// 
            /// </summary>
            public uint dwHighDateTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadHandle"></param>
        /// <param name="creationTime"></param>
        /// <param name="exitTime"></param>
        /// <param name="kernelTime"></param>
        /// <param name="userTime"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern bool GetThreadTimes(IntPtr threadHandle, ref UInt64 creationTime, ref UInt64 exitTime, ref UInt64 kernelTime, ref UInt64 userTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPermissions"></param>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern UInt32 SetProcPermissions(UInt32 newPermissions);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern UInt32 GetCurrentPermissions();

        /// <summary>
        /// No events — remove all event registrations for this application.
        /// </summary>
        public const int NOTIFICATION_EVENT_NONE = 0;               

        /// <summary>
        /// When the system time is changed.
        /// </summary>
        public const int NOTIFICATION_EVENT_TIME_CHANGE = 1;        

        /// <summary>
        /// When data synchronization finishes.
        /// </summary>
        public const int NOTIFICATION_EVENT_SYNC_END = 2;           

        /// <summary>
        /// When a PC Card device is changed.
        /// </summary>
        public const int NOTIFICATION_EVENT_DEVICE_CHANGE = 7;      

        /// <summary>
        /// When an RS232 connection is made.
        /// </summary>
        public const int NOTIFICATION_EVENT_RS232_DETECTED = 9;     

        /// <summary>
        /// When a full device data restore completes.
        /// </summary>
        public const int NOTIFICATION_EVENT_RESTORE_END = 10;	    

        /// <summary>
        /// When the device wakes up.
        /// </summary>
        public const int NOTIFICATION_EVENT_WAKEUP = 11;            

        /// <summary>
        /// When the time zone is changed.
        /// </summary>
        public const int NOTIFICATION_EVENT_TZ_CHANGE = 12;         

        /// <summary>
        /// 
        /// </summary>
        public const UInt32 INFINITE = 0xFFFFFFFF;

        /// <summary>
        /// 
        /// </summary>
        public const int EVENT_ALL_ACCESS = 0x001F0003;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pwszAppName"></param>
        /// <param name="lWhichEvent"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern int CeRunAppAtEvent(string pwszAppName, int lWhichEvent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="security"></param>
        /// <param name="manualreset"></param>
        /// <param name="initialstate"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern IntPtr CreateEvent(IntPtr security, bool manualreset, bool initialstate, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="millisec"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern int WaitForSingleObject(IntPtr handle, uint millisec);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desiredAccess"></param>
        /// <param name="inheritHandle"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport(DllName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr OpenEvent(int desiredAccess, bool inheritHandle, string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpmst"></param>
        [DllImport(DllName)]
        public static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpmst);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpDirectoryName"></param>
        /// <param name="lpFreeBytesAvailable"></param>
        /// <param name="lpTotalNumberOfBytes"></param>
        /// <param name="lpTotalNumberOfFreeBytes"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(
            string lpDirectoryName,
            out ulong lpFreeBytesAvailable,
            out ulong lpTotalNumberOfBytes,
            out ulong lpTotalNumberOfFreeBytes);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(DllName)]
        public static extern UInt32 GetIdleTime();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lptstrFilename"></param>
        /// <param name="lpdwHandle"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern int GetFileVersionInfoSize(string lptstrFilename, out IntPtr lpdwHandle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lptstrFilename"></param>
        /// <param name="dwHandle"></param>
        /// <param name="dwLen"></param>
        /// <param name="lpData"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern bool GetFileVersionInfo(string lptstrFilename, IntPtr dwHandle, int dwLen, IntPtr lpData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBlock"></param>
        /// <param name="lpSubBlock"></param>
        /// <param name="lplpBuffer"></param>
        /// <param name="puLen"></param>
        /// <returns></returns>
        [DllImport(DllName, SetLastError = true)]
        public static extern bool VerQueryValue(IntPtr pBlock, string lpSubBlock, out IntPtr lplpBuffer, out int puLen);


        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct VS_FIXEDFILEINFO
        {

            // ReSharper disable MemberCanBePrivate.Local
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            public UInt32 dwSignature;


            public UInt32 dwStrucVersion;


            public UInt32 dwFileVersionMS;

            public UInt32 dwFileVersionLS;

            public UInt32 dwProductVersionMS;

            public UInt32 dwProductVersionLS;

            public UInt32 dwFileFlagsMask;

            public UInt32 dwFileFlags;

            public FileOS dwFileOS;

            public FileType dwFileType;

            public UInt32 dwFileSubtype;

            public UInt32 dwFileDateMS;

            public UInt32 dwFileDateLS;
            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore FieldCanBeMadeReadOnly.Local

        };

        /// <summary>
        /// 
        /// </summary>
        public enum FileOS : uint
        {

            Unknown = 0x00000000,

            DOS = 0x00010000,

            OS2_16 = 0x00020000,

            OS2_32 = 0x00030000,

            NT = 0x00040000,

            WindowsCE = 0x00050000,

        }

        /// <summary>
        /// 
        /// </summary>
        public enum FileType : uint
        {

            Unknown = 0x00,

            Application = 0x01,

            DLL = 0x02,

            Driver = 0x03,

            Font = 0x04,

            VXD = 0x05,

            StaticLib = 0x07

        }
    }
}
