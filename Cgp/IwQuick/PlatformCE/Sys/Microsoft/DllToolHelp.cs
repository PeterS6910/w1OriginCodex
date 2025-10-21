using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class DllToolHelp
    {
        public const string DLL_NAME = "toolhelp.dll";

        public const int TH32CS_SNAPHEAPLIST = 0x01;
        public const int TH32CS_SNAPPROCESS = 0x02;
        public const int TH32CS_SNAPTHREAD = 0x04;
        public const int  TH32CS_SNAPMODULE	= 0x08;

        [DllImport(DLL_NAME)]
        public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);
        
        [DllImport(DLL_NAME)]
        public static extern int CloseToolhelp32Snapshot(IntPtr snapshotHandle);

        //[DllImport(DLL_NAME)]
        //public static extern int Process32First(IntPtr snapshotHandle, byte[] pe);

        [DllImport(DLL_NAME)]
        public static extern bool Process32First(IntPtr snapshotHandle, ref PROCESSENTRY32 processEntry);

        [DllImport(DLL_NAME)]
        public static extern bool Process32Next(IntPtr snapshotHandle, ref PROCESSENTRY32 processEntry);

        [DllImport(DLL_NAME)]
        public static extern bool Thread32First(IntPtr snapshotHandle,ref THREADENTRY32 threadEntry);

        [DllImport(DLL_NAME)]
        public static extern bool Thread32Next(IntPtr snapshotHandle, ref THREADENTRY32 threadEntry);

        public const int MAX_PATH = 260;

         public const int PROCESSENTRY32_SIZE = 564;

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        public struct PROCESSENTRY32 {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public uint th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr,SizeConst=MAX_PATH)]
            public string szExeFile;
            public uint th32MemoryBase;
            public uint th32AccessKey;

            public static PROCESSENTRY32 Create()
            {
                var pe = new PROCESSENTRY32 {dwSize = PROCESSENTRY32_SIZE};

                return pe;
            }
        }

        public const int THREADENTRY32_SIZE  = 36;
        public struct THREADENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ThreadID;
            public uint th32OwnerProcessID;
            public int tpBasePri;
            public int tpDeltaPri;
            public uint dwFlags;
            public uint th32AccessKey;
            public uint th32CurrentProcessID;

            public static THREADENTRY32 Create()
            {
                THREADENTRY32 te = new THREADENTRY32();
                te.dwSize = THREADENTRY32_SIZE;
                return te;
            }

        }

       
    }
}
