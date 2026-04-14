using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Contal.IwQuick.Sys.Microsoft;


namespace Contal.IwQuick.Sys
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessInfo
    {
        private string _processName = null;

        private string _processPath = null;

        private Process _processInstance = null;

        /// <summary>
        /// 
        /// </summary>
        private class ThreadTimeSnapshot
        {
            internal UInt64 _lastKernel;
            internal UInt64 _lastUser;
            internal UInt64 _kernel;
            internal UInt64 _user;

#if DEBUG
            internal readonly LinkedList<UInt64> _userTrace = new LinkedList<ulong>(); 
#endif
        }

        public class ProcMemoryInfo
        {
            public UInt32 CommitImage;
            public UInt32 CommitMapped;
            public UInt32 CommitPrivate;

            public UInt32 ReserveImage;
            public UInt32 ReserveMapped;
            public UInt32 ReservePrivate;

            public UInt32 Free;
        }

        private readonly DllToolHelp.PROCESSENTRY32 _processEntry;

        private static readonly Data.SyncDictionary<uint, ThreadTimeSnapshot> _lastProcessTimes =
            new Data.SyncDictionary<uint, ThreadTimeSnapshot>(8);

        // TODO : allow access to thread-level information of the process
        /*private readonly Data.SyncDictionary<uint, DllToolHelp.THREADENTRY32> _threadEntries = 
            new Data.SyncDictionary<uint, DllToolHelp.THREADENTRY32>(8);*/

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                if (null == _processName)
                {
                    if (_processEntry.szExeFile != null)
                    {
                        {
                            var pos = _processEntry.szExeFile.IndexOf('\0');

                            _processName = pos < 0 
                                ? _processEntry.szExeFile 
                                : _processEntry.szExeFile.Substring(0, pos);
                        }
                    }
                }

                return _processName;
            }
        }

        private const int ModuleFileNameBufferSize = 256;

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            get {
                if (_processPath != null) 
                    return _processPath;

                var buffer = Marshal.AllocHGlobal(ModuleFileNameBufferSize);
                try
                {
                    var result = DllCoredll.GetModuleFileName((IntPtr)_processEntry.th32ProcessID, buffer, ModuleFileNameBufferSize);

                    _processPath = result > 0 ? Marshal.PtrToStringUni(buffer, result) : string.Empty;
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }

                return _processPath;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Process ProcessInstance
        {
            get
            {
                if (_processInstance != null)
                    return _processInstance;

                _processInstance = Process.GetProcessById((int)_processEntry.th32ProcessID);
                return _processInstance;
            }
        }

        private Version _executableVersion = null;
        /// <summary>
        /// 
        /// </summary>
        public Version ExecutableVersion
        {
            get
            {
                if (null != _executableVersion)
                    return _executableVersion;

                _executableVersion = FileVersionInfo.GetVersionInfo(Path);
                return _executableVersion;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExecutableVersionString
        {
            get
            {
                try
                {
                    Version v = ExecutableVersion;
                    return v.ToString();
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public uint PID
        {
            get
            {
                return _processEntry.th32ProcessID;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public uint ThreadCount
        {
            get
            {
                return _processEntry.cntThreads;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public uint ReferenceCount
        {
            get
            {
                return _processEntry.cntUsage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BasePriority
        {
            get
            {
                return _processEntry.pcPriClassBase;
            }
        }

        private int _prioSum;
        private double _averagePriority = -1;

        /// <summary>
        /// 
        /// </summary>
        public double AveragePriority
        {
            get
            {
                if (_averagePriority < 0)
                    _averagePriority = _prioSum/(double)_processEntry.cntThreads;

                return _averagePriority;
            }
        }

        // in 100-nanosecond units
        private UInt64 _kernelTime = 0;
        /// <summary>
        /// in miliseconds
        /// </summary>
        public UInt64 KernelTime
        {
            get { return _kernelTime; }
        }

        // in 100-nanosecond units
        private UInt64 _userTime = 0;

        /// <summary>
        /// in miliseconds
        /// </summary>
        public UInt64 UserTime
        {
            get { return _userTime; }
        }

        //private UInt64 _lastKernelTime = 0;
        //private UInt64 _lastUserTime = 0;

        /// <summary>
        /// in miliseconds
        /// </summary>
        public static UInt64 TotalProcessorTime
        {
            get
            {
                lock (_syncRoot)
                {
                    return _currentTotalTime;

                    // do not use this variant, cause this equation
                    // does not count with process times of processes that 
                    // were running and stopped before calling GetProcessInfos
                    //return (_allKernelTime + _allUserTime)/THREAD_TIMES_DIVIDER_L + _allIdleTime;
                }
            }
        }

        /// <summary>
        /// in miliseconds
        /// </summary>
        public static UInt64 RecentProcessorTime
        {
            get
            {
                lock (_syncRoot)
                {
                    return _currentTotalTime - _lastTotalTime;

                    /*
                    var kernelDiff = _allKernelTime - _lastAllKernelTime;
                    var userDiff = _allUserTime - _lastAllUserTime;
                    var idleDiff = _allIdleTime - _lastAllIdleTime;

                    var ret = (kernelDiff + userDiff)/THREAD_TIMES_DIVIDER_L + idleDiff;
                    return ret;*/
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public double AverageKernelLoad
        {
            get
            {
                lock (_syncRoot)
                    return ((double)_kernelTime / _totalTimeSnapshot) * 100;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public double AverageUserLoad
        {
            get
            {
                lock (_syncRoot)
                    return (double)_userTime / _totalTimeSnapshot * 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double AverageTotalLoad
        {
            get
            {
                lock (_syncRoot)
                    return ((double)(_kernelTime + _userTime)) / _totalTimeSnapshot * 100;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double RecentTotalLoad
        {
            get
            {
                lock (_syncRoot)
                {
                    ThreadTimeSnapshot ts = GetThreadTimeSnapshot(PID);
                    if (ts == null)
                        return 0;

                    

                    var recentKernelTime = ts._lastKernel;
                    var recentUserTime = ts._lastUser;

                    var kernelDiff = _kernelTime - recentKernelTime;
                    var userDiff = _userTime - recentUserTime;

                    var rpt = RecentProcessorTime;

                    if (rpt == 0)
                        return 0;

                    var ret = ((double)(kernelDiff + userDiff) ) / rpt * 100;

                    ValidatePercentRelevancy(ref ret);
                        

                    return ret;

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double RecentKernelLoad
        {
            get
            {
                lock (_syncRoot)
                {
                    var ts = GetThreadTimeSnapshot(PID);
                    if (ts == null)
                        return 0;

                    var recentKernelTime = ts._lastKernel;

                    var ret = ((double)(_kernelTime - recentKernelTime)) / RecentProcessorTime * 100;
                    ValidatePercentRelevancy(ref ret);
                    return ret;
                }
            }
        }

        /// <summary>
        /// after retrieving the ThreadTimeSnapshot characteristic, revalidates the process existence
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private ThreadTimeSnapshot GetThreadTimeSnapshot(uint pid)
        {
            ThreadTimeSnapshot ts;
            if (!_lastProcessTimes.TryGetValue(pid, out ts))
                return null;

            try
            {
                var p = Process.GetProcessById((int) pid);

                DebugHelper.Keep(p);

                
            }
            catch(Exception)
            {
                _lastProcessTimes.Remove(pid);
                return null;
            }

            return ts;
        }

        /// <summary>
        /// 
        /// </summary>
        public double RecentUserLoad
        {
            get
            {
                lock (_syncRoot)
                {
                    ThreadTimeSnapshot ts = GetThreadTimeSnapshot(PID);
                    if (ts == null)
                        return 0;

                    var recentUserTime = ts._lastUser;

                    var rpt = RecentProcessorTime;

                    if (rpt == 0)
                        return 0;

                    var ret= ((double)(_userTime - recentUserTime)) / rpt * 100;

                    ValidatePercentRelevancy(ref ret);

                    return ret;
                }
            }
        }

        private void ValidatePercentRelevancy(ref double ret)
        {
            if (ret > IrrelevantPercentCoefficent)
            {
                // revalidate relevancy of the record
                GetThreadTimeSnapshot(PID);
                ret = 0;
            }
        }

        public float PhysicalMemoryConsumption
        {
            get
            {
                return MemoryInfo.CommitPrivate / SystemInfo.F1M;
            }
        }

        public ProcMemoryInfo MemoryInfo
        {
            get
            {
                lock (_syncRoot)
                {
                    var hProcess =
                        DllCoredll.OpenProcess(
                            0,
                            false,
                            _processEntry.th32ProcessID);

                    if (hProcess == IntPtr.Zero)
                        throw new Win32Exception();

                    var result = new ProcMemoryInfo();

                    try
                    {
                        var mbi = new DllCoredll.MEMORY_BASIC_INFORMATION();

                        for (UInt32 offset = 0; offset < 0x40000000; offset += mbi.RegionSize)
                        {
                            var bytesRead =
                                DllCoredll.VirtualQueryEx(
                                    hProcess,
                                    new UIntPtr(_processEntry.th32MemoryBase + offset),
                                    ref mbi,
                                    (uint)Marshal.SizeOf(typeof(DllCoredll.MEMORY_BASIC_INFORMATION)));

                            if (bytesRead == 0)
                                break;

                            switch ((DllCoredll.MemStates)mbi.State)
                            {
                                case DllCoredll.MemStates.MemCommit:

                                    switch ((DllCoredll.MemType)mbi.Type)
                                    {
                                        case DllCoredll.MemType.MemImage:
                                            result.CommitImage += mbi.RegionSize;
                                            break;

                                        case DllCoredll.MemType.MemMapped:
                                            result.CommitMapped += mbi.RegionSize;
                                            break;

                                        case DllCoredll.MemType.MemPrivate:
                                            result.CommitPrivate += mbi.RegionSize;
                                            break;
                                    }

                                    break;

                                case DllCoredll.MemStates.MemReserve:

                                    switch ((DllCoredll.MemType)mbi.Type)
                                    {
                                        case DllCoredll.MemType.MemImage:
                                            result.ReserveImage += mbi.RegionSize;
                                            break;

                                        case DllCoredll.MemType.MemMapped:
                                            result.ReserveMapped += mbi.RegionSize;
                                            break;

                                        case DllCoredll.MemType.MemPrivate:
                                            result.ReservePrivate += mbi.RegionSize;
                                            break;
                                    }

                                    break;

                                case DllCoredll.MemStates.MemFree:

                                    result.Free += mbi.RegionSize;
                                    break;
                            }
                        }
                    }
                    finally
                    {
                        DllCoredll.CloseHandle(hProcess);
                    }

                    return result;
                }
            }
        }
        //default constructor
        /*public ProcessInfo()
        {

        }*/

        //private helper constructor
        private ProcessInfo(DllToolHelp.PROCESSENTRY32 processEntry)
        {
            _processEntry = processEntry;

        }

        #region Overral static characteristics
        private static UInt64 _currentSumIdleTime = 0;
        private static UInt64 _currentTotalTime = 0;


        private static uint _lastEnvironmentTickCount = 0;
        private static uint _overFlowCount = 0;

        /// <summary>
        /// 
        /// </summary>
        public static UInt64 AllIdleTime
        {
            get
            {
                lock (_syncRoot)
                    return _currentSumIdleTime;
            }
        }

        // in 100-nanosecond units
        //private static UInt64 _lastSumKernelTime = 0;
        // in 100-nanosecond units
        //private static UInt64 _lastSumUserTime = 0;

        // in miliseconds
        private static UInt64 _lastSumIdleTime = 0;


        private static UInt64 _lastTotalTime = 0;
        #endregion

        /// <summary>
        /// used for converting 100-nanosecond unit into miliseconds
        /// </summary>
        private const UInt64 ThreadTimesDividerLong = 10000;

        private const int PercentCoefficent = 100;
        private const int IrrelevantPercentCoefficent = 110;

        /// <summary>
        /// 
        /// </summary>
        public static double AverageIdleTime
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_currentTotalTime == 0)
                        return 0;

                    return (_currentSumIdleTime / (double)_currentTotalTime * PercentCoefficent);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static double RecentIdleTime
        {
            get
            {
                lock (_syncRoot)
                {
                    var idleDiff = _currentSumIdleTime - _lastSumIdleTime;
                    //var kernelDiff = _currentSumKernelTime - _lastSumKernelTime;
                    //var userDiff = _curentSumUserTime - _lastSumUserTime;

                    var rpt = RecentProcessorTime;
                    if (rpt == 0)
                        return 0;

                    return (idleDiff * 100.0) / rpt;
                }
            }
        }

        private static readonly object _syncRoot = new object();
        
        // value _currentTotalTime copied to every ProcessInfo
        private ulong _totalTimeSnapshot;
        

        /// <summary>
        /// main examining method using CreateToolhelp32Snapshot from ToolHelp.dll
        /// </summary>
        /// <returns></returns>
        public static ProcessInfo[] GetAllProcessInfos()
        {
            lock (_syncRoot)
            {

                //uint oldPermissions = 0;

                ProcessInfo[] returnList = null;
                var snapshotHandle = IntPtr.Zero;

                try
                {
                    // NOT SURE IF THIS HAS MEANING ELSEWHERE THAN IN DRIVER
                    //oldPermissions = DllCoredll.SetProcPermissions(0xFFFFFFFF);

                    //temp ArrayList
                    var processList = new Dictionary<uint, ProcessInfo>();

                    

                    _lastProcessTimes.ForEach(
                        (key, value) =>
                        {
                            value._lastKernel = value._kernel;
                            value._lastUser = value._user;
                            value._user = 0;
                            value._kernel = 0;
                        }
                        );

                    
                    var sw = StopwatchPool.Singleton.MeasureStart();

                    
                    var thisThreadId = (IntPtr)Thread.CurrentThread.ManagedThreadId;
                    var formerPrio = DllCoredll.GetThreadPriority(thisThreadId);
                    DllCoredll.SetThreadPriority(thisThreadId, 100);

                    try
                    {
                        // call this closest before CreateToolhelp32Snapshot call 
                        UpdateGlobalTimes();
                        snapshotHandle =
                            DllToolHelp.CreateToolhelp32Snapshot(
                                DllToolHelp.TH32CS_SNAPPROCESS | DllToolHelp.TH32CS_SNAPTHREAD, 0);
                    }
                    finally
                    {
                        DllCoredll.SetThreadPriority(thisThreadId, formerPrio);
                    }

                    if (snapshotHandle == IntPtr.Zero) 
                        throw new Exception("Unable to create snapshot");

                    
                    

                    //_currentSumKernelTime = 0;
                    //_curentSumUserTime = 0;


                    try
                    {
                        var pe32 = DllToolHelp.PROCESSENTRY32.Create();

                        //Get the first process
                        var retVal = DllToolHelp.Process32First(snapshotHandle, ref pe32);
                        while (retVal)
                        {
                            //New instance of the Process class
                            var proc = new ProcessInfo(pe32);


                            //DllToolHelp.THREADENTRY32 te32 = DllToolHelp


                            processList[proc.PID] = proc;

                            pe32 = DllToolHelp.PROCESSENTRY32.Create();

                            retVal = DllToolHelp.Process32Next(snapshotHandle, ref pe32);
                        }
                    }
                    catch (Exception ex)
                    {
                        HandledExceptionAdapter.Examine(ex);
                    }

                    // to avoid stacking of unqueryable snapshots
                    _lastProcessTimes.RemoveWhere(
                        (key, value) => !processList.ContainsKey(key));

                    ExamineThreadsFromSnapshot(snapshotHandle, processList);

                    foreach (var processInfo in processList.Values)
                    {
                        processInfo._totalTimeSnapshot = _currentTotalTime;
                    }

                    try
                    {
                        var took = StopwatchPool.Singleton.MeasureStop(sw);

                        var tmp = "\t\tProcessInfo. examination took " + took + " ms";
                        Console.WriteLine(tmp);
                        Debug.WriteLine(tmp);
                    }
                    catch
                    {
                            
                    }


                    returnList = new ProcessInfo[processList.Values.Count];
                    processList.Values.CopyTo(returnList, 0);

                    

                    return returnList;
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
                finally
                {
                    try
                    {
                        if (snapshotHandle != IntPtr.Zero)
                            //Close handle
                            DllToolHelp.CloseToolhelp32Snapshot(snapshotHandle);
                    }
                    catch (Exception ex)
                    {
                        HandledExceptionAdapter.Examine(ex);
                    }

                    //try{
                    //    if (oldPermissions > 0)
                    //        DllCoredll.SetProcPermissions(oldPermissions);
                    //}
                    //catch
                    //{
                    //}
                }

                return returnList;
            }
        }

        /// <summary>
        /// call this closest before CreateToolhelp32Snapshot call 
        /// </summary>
        private static void UpdateGlobalTimes()
        {
            // making snapshot of old values
            _lastSumIdleTime = _currentSumIdleTime;
            _lastTotalTime = _currentTotalTime;

            // do not increment this one, it's already an aggregate of idle ms from boot
            _currentSumIdleTime = DllCoredll.GetIdleTime();
            
            var currentEtc = (uint) Environment.TickCount;
            // simple overflowing handling
            if (currentEtc < _lastEnvironmentTickCount)
                _overFlowCount++;
            _currentTotalTime = currentEtc + _overFlowCount*UInt32.MaxValue;

            _lastEnvironmentTickCount = currentEtc;
        }

        private static void ExamineThreadsFromSnapshot(IntPtr snapshotHandle, Dictionary<uint, ProcessInfo> processList)
        {
            try
            {
                var te32 = DllToolHelp.THREADENTRY32.Create();

                //Get the first process
                var retval = DllToolHelp.Thread32First(snapshotHandle, ref te32);

                //UInt64 sumKernelTime = 0;
                //UInt64 sumUserTime = 0;

                while (retval)
                {
                    ProcessInfo pi;
                    if (processList.TryGetValue(te32.th32OwnerProcessID, out pi))
                    {
                        UInt64 ftCreate = 0, ftExit = 0, ftKernel = 0, ftUser = 0;

                        var threadHandle = IntPtr.Zero;
                        try
                        {
                            threadHandle = DllCoredll.OpenThread(
                                DllCoredll.THREAD_QUERY_INFORMATION,
                                0, // stands for false
                                te32.th32ThreadID);


                            //DebugHelper.NOP(te32.th32ThreadID, threadHandle,th2);

                            //bool threadTimes = (DllCoredll.GetThreadTimes((IntPtr)te32.th32ThreadID, ref ftCreate, ref ftExit, ref ftKernel, ref ftUser));

                            var threadTimesSucceeded = false;

                            if (threadHandle != IntPtr.Zero)
                                threadTimesSucceeded = (DllCoredll.GetThreadTimes(threadHandle,
                                    ref ftCreate,
                                    ref ftExit,
                                    ref ftKernel,
                                    ref ftUser));

                            if (threadTimesSucceeded && ftExit == 0)
                            {
                                ftCreate = ftCreate/ThreadTimesDividerLong;
                                ftExit = ftExit/ThreadTimesDividerLong;

                                if (ftExit > 0)
                                    DebugHelper.Keep(ftCreate, ftExit);

                                ftUser = ftUser/ThreadTimesDividerLong;
                                ftKernel = ftKernel/ThreadTimesDividerLong;

                                pi._kernelTime += ftKernel;
                                pi._userTime += ftUser;

                                pi._prioSum += te32.tpBasePri;

                                _lastProcessTimes.SetValue(
                                    te32.th32OwnerProcessID,
                                    pid =>
                                    {
                                        var ts = new ThreadTimeSnapshot
                                        {
                                            _kernel = ftKernel,
                                            _user = ftUser
                                        };
                                        return ts;
                                    },
                                    (key, timeSnapshot) =>
                                    {
                                        timeSnapshot._kernel += ftKernel;

#if DEBUG
                                        if (timeSnapshot._userTrace.Count >= 20)
                                            timeSnapshot._userTrace.RemoveFirst();

                                        timeSnapshot._userTrace.AddLast(timeSnapshot._user);
#endif


                                        timeSnapshot._user += ftUser;

                                        return timeSnapshot;
                                    }
                                    , null
                                    );

                                //sumKernelTime += ftKernel;
                                //sumUserTime += ftUser;

                                //pi._threadEntries[te32.th32ThreadID] = te32;
                            }
                        }
                        catch (Exception e)
                        {
                            HandledExceptionAdapter.Examine(e);
                        }
                        finally
                        {
                            if (threadHandle != IntPtr.Zero)
                                try
                                {
                                    DllCoredll.CloseHandle(threadHandle);
                                }
                                catch
                                {
                                }
                        }
                    }
                    else
                        DebugHelper.NOP(te32);

                    // need to create a new instance as that is stored into threadEntries
                    te32 = DllToolHelp.THREADENTRY32.Create();

                    retval = DllToolHelp.Thread32Next(snapshotHandle, ref te32);
                }       
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        /// <summary>
        /// Gets thread entries for specific process
        /// </summary>
        /// <param name="processId">process ID</param>
        /// <returns>array of thread entries</returns>
        public static DllToolHelp.THREADENTRY32[] GetThreadEntries(uint processId)
        {
            var result = new List<DllToolHelp.THREADENTRY32>();

            var handle = DllToolHelp.CreateToolhelp32Snapshot(DllToolHelp.TH32CS_SNAPPROCESS | DllToolHelp.TH32CS_SNAPTHREAD, 0);
            try
            {
                var te32 = DllToolHelp.THREADENTRY32.Create();

                //Get the first process
                var retval = DllToolHelp.Thread32First(handle, ref te32);
                while (retval)
                {
                    if (te32.th32OwnerProcessID == processId)
                        result.Add(te32);

                    te32 = DllToolHelp.THREADENTRY32.Create();
                    retval = DllToolHelp.Thread32Next(handle, ref te32);
                }
            }
            catch// (Exception ex)
            {

            }
            DllToolHelp.CloseToolhelp32Snapshot(handle);

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Process[] GetAllProcesses()
        {
            var pis = GetAllProcessInfos();
            if (null == pis)
                return null;

            var ret = new Process[pis.Length];

            for (var i = 0; i < pis.Length; i++)
            {
                try
                {
                    var p = Process.GetProcessById((int)pis[i].PID);
                    if (null != p)
                        ret[i] = p;
                }
                catch
                {

                }
            }

            return ret;

        }

    }
}
