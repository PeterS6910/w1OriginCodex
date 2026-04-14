using System.Globalization;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemInfo
    {
        private static readonly SystemInfo _singleton = new SystemInfo();

        private SystemInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static SystemInfo Singleton
        {
            get
            {
                return _singleton;
            }
        }

        public const float F1M = 1024*1024;

        /*
        private struct SYSTEMINFO
        {
            public UInt16 wProcessorArchitecture;
            public UInt16 wReserved;
            public UInt32 dwPageSize;
            public UInt32 lpMinimumApplicationAddress;
            public UInt32 lpMaximumApplicationAddress;
            public UInt32 dwActiveProcessorMark;
            public UInt32 dwNumberOfProcessors;
            public UInt32 dwProcessorType;
            public UInt32 dwAllocationGranularity;
            public UInt16 wProcessorLevel;
            public UInt16 wProcessorRevision;
        };*/

        /*[DllImport("CoreDll.dll")]
        private static extern void GetSystemInfo(ref SYSTEMINFO lpmst);*/

        private static DllCoredll.MEMORYSTATUS _memoryStatus = new DllCoredll.MEMORYSTATUS();
        /*private static SYSTEMINFO _systemInfo = new SYSTEMINFO();*/



        private float _totalPhysicalMem;
        /// <summary>
        /// 
        /// </summary>
        public float TotalPhysicalMemory
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.TotalPhysical);
                return _totalPhysicalMem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TotalPhysicalMemoryReport
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.TotalPhysical);
                return _totalPhysicalMem.ToString("0.000");
            }
        }

        private float _availablePhysicalMem;
        /// <summary>
        /// 
        /// </summary>
        public float AvailablePhysicalMemory
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.AvailablePhysical);
                return _availablePhysicalMem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AvailablePhysicalMemoryReport
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.AvailablePhysical);
                return _availablePhysicalMem.ToString("0.000");
            }
        }

        private float _usedPhysicalMem;
        /// <summary>
        /// 
        /// </summary>
        public float UsedPhysicalMemory
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.UsedPhysical);
                return _usedPhysicalMem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UsedPhysicalMemoryReport
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.UsedPhysical);
                return _usedPhysicalMem.ToString("0.000");
            }
        }

        private float _totalVirtualMem;
        /// <summary>
        /// 
        /// </summary>
// ReSharper disable once UnusedMember.Local
        private float TotalVirtualMemory
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.TotalVirtual);
                return _totalVirtualMem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TotalVirtualMemoryReport
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.TotalVirtual);
                return _totalVirtualMem.ToString("0.000");
            }
        }

        private float _availableVirtualMem;
        /// <summary>
        /// 
        /// </summary>
        public float AvailableVirtualMemory
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.AvailableVirtual);
                return _availableVirtualMem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AvailableVirtualMemoryReport
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.AvailableVirtual);
                return _availableVirtualMem.ToString("0.000");
            }
        }

        private float _usedVirtualMem;
        /// <summary>
        /// 
        /// </summary>
        public float UsedVirtualMemory
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.UsedVirtual);
                return _usedVirtualMem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UsedVirtualMemoryReport
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.UsedVirtual);
                return _usedVirtualMem.ToString("0.000");
            }
        }

        private uint _memoryLoad;
        /// <summary>
        /// 
        /// </summary>
        public uint MemoryLoad
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.Load);
                return _memoryLoad;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MemoryLoadReport
        {
            get
            {
                EnsureLatestMemoryIndicators(MemoryIndicatorType.Load);
                return _memoryLoad.ToString(CultureInfo.InvariantCulture);
            }
        }

        private enum MemoryIndicatorType
        {
            None,
            TotalPhysical,
            AvailablePhysical,
            UsedPhysical,
            TotalVirtual,
            AvailableVirtual,
            UsedVirtual,
            Load
        }

        private MemoryIndicatorType _lastMemoryIndicator = MemoryIndicatorType.None;

        private void EnsureLatestMemoryIndicators(MemoryIndicatorType indicatorType)
        {
            if (indicatorType == _lastMemoryIndicator ||
                _lastMemoryIndicator == MemoryIndicatorType.None)
            {
                RefreshMemoryIndicators();
                _lastMemoryIndicator = indicatorType;
            }

        }

        private void RefreshMemoryIndicators()
        {
            _memoryStatus.dwLength = (uint)Marshal.SizeOf(_memoryStatus);

            DllCoredll.GlobalMemoryStatus(ref _memoryStatus);

            _totalPhysicalMem = (_memoryStatus.dwTotalPhys / F1M);
            _availablePhysicalMem = (_memoryStatus.dwAvailPhys / F1M);
            _usedPhysicalMem = (_memoryStatus.dwTotalPhys / F1M) -
                (_memoryStatus.dwAvailPhys / F1M);

            _totalVirtualMem = (_memoryStatus.dwTotalVirtual / F1M);
            _availableVirtualMem = (_memoryStatus.dwAvailVirtual / F1M);
            _usedVirtualMem = (_memoryStatus.dwTotalVirtual / F1M) -
                (_memoryStatus.dwAvailVirtual / F1M);

            _memoryLoad = _memoryStatus.dwMemoryLoad;
        }


        /* original Status
        public static void Status()
        {

            try
            {
                _memoryStatus.dwLength = (uint)Marshal.SizeOf(_memoryStatus);

                GlobalMemoryStatus(ref _memoryStatus);

                Console.WriteLine("Total physical memory (MB) : " + ((float)_memoryStatus.dwTotalPhys / I1M).ToString("0.00"));
                Console.WriteLine("Available physical memory (MB) : " + ((float)_memoryStatus.dwAvailPhys / I1M).ToString("0.00"));
                Console.WriteLine("Total virtual memory (MB) : " + ((float)_memoryStatus.dwTotalVirtual / I1M).ToString("0.00"));
                Console.WriteLine("Available virtual memory (MB) : " + ((float)_memoryStatus.dwAvailVirtual / I1M).ToString("0.00"));
                Console.WriteLine("Memory load (%) : " + _memoryStatus.dwMemoryLoad);

                GetSystemInfo(ref _systemInfo);

                Console.WriteLine(
                    "Allocation granularity : " + _systemInfo.dwAllocationGranularity +
                    "\nActive CPU mark : " + _systemInfo.dwActiveProcessorMark +
                    "\nCPU type " + _systemInfo.dwProcessorType +
                    "\nCPU arch " + _systemInfo.wProcessorArchitecture +
                    "\nCPU level " + _systemInfo.wProcessorLevel +
                    "\nCPU revision " + _systemInfo.wProcessorRevision +
                    "\nMinAppAddr " + _systemInfo.lpMinimumApplicationAddress +
                    "\nMaxAppAddr " + _systemInfo.lpMaximumApplicationAddress
                    );
                Console.WriteLine();

            }
            catch (Exception error)
            {
                Console.WriteLine("error : " + error.Message);
            }

        }
        */
    }
}
