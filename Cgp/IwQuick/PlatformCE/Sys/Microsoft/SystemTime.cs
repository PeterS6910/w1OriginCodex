using System;
using System.Threading;
using System.Diagnostics;
using Contal.IwQuick;
using Contal.IwQuick.Threads;

namespace Contal.IwQuick.Sys.Microsoft
{
    /// <summary>
    /// 
    /// </summary>
    public class NativeDateTime
    {
        /// <summary>
        /// 
        /// </summary>
        public ushort Year { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort Month { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort DayOfWeek { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort Day { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort Hour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort Minute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort Second { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort Millisecond { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return SystemTime.GetLocalTime();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0:2}-{1:2}-{2:2} {3:2}:{4:2}:{5:2}",
                Year, Month, Day, Hour, Minute, Second);
        }

    }

    /// <summary>
    /// class for wrapping API date-time routines
    /// </summary>
    public static class SystemTime
    {
        /// <summary>
        /// starts the initial synchronisation, as neither DateTime.Now neither other
        /// calls provide milisecond fraction of current time
        /// </summary>
        /// <returns>false, if the thread had already started or the synchronization had already been done, 
        /// or there has been an error preventing the synchronization</returns>
        [Obsolete("Use WindowsCE image 3056 and newer, which provides miliseconds directly")]
        public static bool SynchronizeMiliseconds()
        {
            if (_msSyncValid)
                return false;

            if (_msSyncThread == null)
                lock (_msSyncThreadLock)
                {
                    if (_msSyncThread == null)
                        _msSyncThread = new SafeThread(InitialMilisecondSyncThread, ThreadPriority.Highest, false);
                    else
                        return false;
                }
            else
                return false;

            var threadHandle = IntPtr.Zero;

            try
            {
                threadHandle = DllCoredll.OpenThread(DllCoredll.THREAD_ALL_ACCESS, 0, (uint)_msSyncThread.Thread.ManagedThreadId);

                var result = DllCoredll.SetThreadPriority(threadHandle, 180); // make the priority very high to ensure precision, even higher that CLR
                DebugHelper.Keep(result);
                // but higher than 160
            }
            catch (Exception)
            {
            }
            finally
            {
                if (threadHandle != IntPtr.Zero)
                    try { DllCoredll.CloseHandle(threadHandle); }
                    catch { }
            }//*/

            _msSyncThread.Start();

            return true;
        }

        private static volatile SafeThread _msSyncThread = null;
        private static readonly object _msSyncThreadLock = new object();

        private static volatile uint _examinedMsSync = 0;

        // used for sync instead of explicit locking 
        // SynchronizeMiliseconds should be called only once per program anyway
        private static volatile bool _msSyncValid = false;

        /// <summary>
        /// returns whether initial milisecond synchronisation already passed
        /// </summary>
        public static bool MilisecondsSynchronized
        {
            get
            {
                return _msSyncValid;
            }
        }

        /// <summary>
        /// actual syncing according the ticks when they pass from 999 to 0 fraction of 1000 ; 
        /// should be executed at the begining of application to avoid distruption by other processes ; 
        /// the mechanism has only limited preciseness
        /// </summary>
        private static void InitialMilisecondSyncThread()
        {
            if (_msSyncValid)
                // double check to not enter the same river twice
                return;

            int raceCondition = 0;

            Stopwatch sw = Stopwatch.StartNew();

            int referenceSecond;

            DllCoredll.SYSTEMTIME systime = new DllCoredll.SYSTEMTIME();
            DllCoredll.GetLocalTime(ref systime);

            referenceSecond = systime.wSecond;

            while (raceCondition < 0xFFFF)
            {
                DllCoredll.GetLocalTime(ref systime);

                if (systime.wSecond > referenceSecond ||
                    (systime.wSecond == 0 && referenceSecond==59))
                {
                    _examinedMsSync = (uint)Environment.TickCount;
                    _msSyncValid = true;
                    break;
                }

                uint k = 0;
                for (uint i = 0; i < 3072; i++)
                    // as there is no usleep, try to slow-down the rate of the GetLocalTime call 
                {
                    k += i;
                    DebugHelper.Keep(k);
                }


                //Thread.Sleep(1);
                raceCondition++;
            }

            long took = sw.ElapsedMilliseconds;
            try
            {
                string tmp;
                if (_msSyncValid)
                    tmp =
                        string.Format(
                            "\r\n\tSystemTime: Milisecond datetime synchronized after {0}ms and {1} iterations\r\n",
                            took, raceCondition);
                else
                    tmp = String.Format("\r\n\tSystemTime: Milisecond datetime DID NOT synchronized after {0}ms\r\n",
                        took);

                Console.WriteLine(tmp);
                Debug.WriteLine(tmp);
            }
            catch
            {
                
            }

            lock(_msSyncThreadLock)
                _msSyncThread = null;
        }

        /*
        private static void GetTime()
        {
            // Call the native GetSystemTime method
            // with the defined structure.
            SYSTEMTIME stime = new SYSTEMTIME();
            GetSystemTime(ref stime);
        }
        */

        private static readonly object _lastLocalTimeLock = new object();
        private static DateTime _lastLocalTime = new DateTime(0);
        private static int _lastTickCountForLocal = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="futureTickCount"></param>
        /// <param name="pastTickCount"></param>
        /// <returns></returns>
        public static int TickCountDifference(int futureTickCount, int pastTickCount)
        {
            int diff;
            if (futureTickCount > pastTickCount)
                diff = futureTickCount - pastTickCount;
            else
                diff = int.MaxValue - pastTickCount + (futureTickCount- int.MinValue) + 1;

            return diff;
        }

        /// <summary>
        /// retrieves the current DateTime structure by API call GetLocalTime from coredll.dll ; 
        /// it does count with time swift and timezones ; 
        /// 
        /// in case of a problem it returns a counted time from last GetLocalTime call plus tick count difference
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLocalTime()
        {
            if (_useDateTimeNow)
                return DateTime.Now;

            DateTime dt;

            for (int i = 0; i < GET_TIME_RETRY_COUNT; i++)
                try
                {
                    var nativeLocalTime = new DllCoredll.SYSTEMTIME();
                    DllCoredll.GetLocalTime(ref nativeLocalTime);

                    if (nativeLocalTime.wDay == 0 ||
                        nativeLocalTime.wMonth == 0 ||
                        nativeLocalTime.wYear == 0
                        )
                    {
                        // means invalid native reading
                        if (i < GET_TIME_RETRY_DELAY - 1)
                            Thread.Sleep(GET_TIME_RETRY_DELAY);
                        //DebugHelper.Keep(nativeLocalTime);
                    }
                    else
                    {
                        dt = ParseDateTime(nativeLocalTime);

                        lock (_lastLocalTimeLock)
                        {
                            _lastLocalTime = dt;
                            _lastTickCountForLocal = Environment.TickCount;
                        }

                        return dt;
                    }

                }
                catch
                {
                }


            int now = Environment.TickCount;

            int diff = TickCountDifference(now, _lastTickCountForLocal);

            lock (_lastLocalTimeLock)
            {
                dt = _lastLocalTime = _lastLocalTime.AddMilliseconds(diff);
                _lastTickCountForLocal = Environment.TickCount;
            }


            return dt;
        }

        private static readonly object _lastSystemTimeLock = new object();
        private static DateTime _lastSystemTime = new DateTime(0);
        private static int _lastTickCountForSystem = 0;


        private const int GET_TIME_RETRY_COUNT = 3;
        private const int GET_TIME_RETRY_DELAY = 20;

        /// <summary>
        /// retrieves the current DateTime structure by API call GetSystemTime from coredll.dll ; 
        /// it's avoiding recounting for time-shift and time-zones, thus providing the RTC time ; 
        /// 
        /// in case of a problem it returns a counted time from last GetSystemTime call plus tick count difference
        /// </summary>
        /// <returns></returns>
        public static DateTime GetSystemTime()
        {
            DateTime dt;

            for (int i=0;i<GET_TIME_RETRY_COUNT;i++)
                try
                {
                    var nativeSystemTime = new DllCoredll.SYSTEMTIME();
                    DllCoredll.GetSystemTime(ref nativeSystemTime);

                    if (nativeSystemTime.wDay == 0 ||
                        nativeSystemTime.wMonth == 0 ||
                        nativeSystemTime.wYear == 0
                        )
                    {
                        // means invalid native reading
                        if (i < GET_TIME_RETRY_DELAY - 1)
                            Thread.Sleep(GET_TIME_RETRY_DELAY);
                        //DebugHelper.Keep(nativeSystemTime);
                    }
                    else
                    {
                        dt = ParseDateTime(nativeSystemTime);

                        lock (_lastSystemTimeLock)
                        {
                            _lastSystemTime = dt;
                            _lastTickCountForSystem = Environment.TickCount;
                        }

                        return dt;
                    }

                }
                catch
                {
                }


            int now = Environment.TickCount;

            int diff = TickCountDifference(now, _lastTickCountForSystem);

            lock (_lastSystemTimeLock)
            {
                dt = _lastSystemTime = _lastSystemTime.AddMilliseconds(diff);
                _lastTickCountForSystem = Environment.TickCount;
            }


            return dt;
        }

        private static DateTime ParseDateTime(DllCoredll.SYSTEMTIME systime)
        {
            uint miliseconds;
            uint secondsPlus = 0;

            if (_msSyncValid)
            {
                uint diff = (uint) Environment.TickCount - _examinedMsSync;
                miliseconds = diff%1000;
                secondsPlus = (diff/1000);
            }
            else
            {
                miliseconds = systime.wMilliseconds;
            }

            var dateTime = new DateTime(
                systime.wYear, systime.wMonth, systime.wDay,
                systime.wHour, 
                systime.wMinute,  
                systime.wSecond, 
                (int) miliseconds);

            if (secondsPlus > 0)
                dateTime=dateTime.AddSeconds(secondsPlus);

            return dateTime;
        }

        /// <summary>
        /// sets time according to actual time-zone into date-time controller
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliseconds"></param>
        public static void SetLocalTime(int year, int month, int day, int hour, int minute, int second, int milliseconds)
        {
            DllCoredll.SYSTEMTIME systime = new DllCoredll.SYSTEMTIME
            {
                wYear = (ushort) year,
                wMonth = (ushort) month,
                wDay = (ushort) day,
                wHour = (ushort) hour,
                wMinute = (ushort) minute,
                wSecond = (ushort) second,
                wMilliseconds = (ushort) milliseconds
            };

            DllCoredll.SetLocalTime(ref systime);
        }

        /// <summary>
        /// sets the time directly into RTC/main system time controller
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <param name="milliseconds"></param>
        public static void SetSystemTime(int year, int month, int day, int hour, int minute, int second, int milliseconds)
        {
            DllCoredll.SYSTEMTIME systime = new DllCoredll.SYSTEMTIME
            {
                wYear = (ushort) year,
                wMonth = (ushort) month,
                wDay = (ushort) day,
                wHour = (ushort) hour,
                wMinute = (ushort) minute,
                wSecond = (ushort) second,
                wMilliseconds = (ushort) milliseconds
            };

            DllCoredll.SetSystemTime(ref systime);
        }

        /// <summary>
        /// 
        /// </summary>
        public static event Action<DateTime> _timeChanged;
        /// <summary>
        /// 
        /// </summary>
        public static event Action<TimeZone, DateTime> _timeZoneChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeChangeEvent"></param>
        /// <returns></returns>
        public static bool RegisterTimeChangedEvent(Action<DateTime> timeChangeEvent)
        {
            if (StartTimeChangedEvent())
            {
                _timeChanged += timeChangeEvent;
                return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeChangeEvent"></param>
        public static void UnRegisterTimeChangedEvent(Action<DateTime> timeChangeEvent)
        {
// ReSharper disable once DelegateSubtraction
            _timeChanged -= timeChangeEvent;
            StopTimeChangedEvent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeZoneChangeEvent"></param>
        /// <returns></returns>
        public static bool RegisterTimeZoneChangedEvent(Action<TimeZone, DateTime> timeZoneChangeEvent)
        {
            if (StartTimeZoneChangedEvent())
            {
                _timeZoneChanged += timeZoneChangeEvent;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeZoneChangeEvent"></param>
        public static void UnRegisterTimeZoneChangedEvent(Action<TimeZone, DateTime> timeZoneChangeEvent)
        {
// ReSharper disable once DelegateSubtraction
            _timeZoneChanged -= timeZoneChangeEvent;
            StopTimeZoneChangedEvent();
        }

        private static SafeThread _safeThreadTimeChange = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool StartTimeChangedEvent()
        {
            try
            {
                StartSafeThreadTimeChange();

                //Remove existing calls CeRunAppAtEvent to TimeChangeEvent (DllCoredll.NOTIFICATION_EVENT_NONE)
                DllCoredll.CeRunAppAtEvent(URI_TIME_CHANGED_EVENT, DllCoredll.NOTIFICATION_EVENT_NONE);
                //Add TimeChange event
                int result = DllCoredll.CeRunAppAtEvent(URI_TIME_CHANGED_EVENT, DllCoredll.NOTIFICATION_EVENT_TIME_CHANGE);

                return ResolveTimeChangedRegistrationResult(result);
            }
            catch { }

            return false;
        }

        private static void StartSafeThreadTimeChange()
        {
            if (_safeThreadTimeChange == null)
            {
                _safeThreadTimeChange = new SafeThread(TimeChangedReportingThread);
                _safeThreadTimeChange.Start();
            }
        }

        private static void TimeChangedReportingThread()
        {
            IntPtr hEvent = DllCoredll.CreateEvent(IntPtr.Zero, false, false, "TimeChangedEvent");

            try
            {
                //infinite loop with waiting for the signal "time changed"
                while (DllCoredll.WaitForSingleObject(hEvent, DllCoredll.INFINITE) == 0)
                {
                    if (_timeChanged != null)
                    {
                        _timeChanged(GetLocalTime());
                    }
                }
            }
            catch
            {

            }
            finally
            {
                try
                {
                    if (hEvent != IntPtr.Zero)
                        DllCoredll.CloseHandle(hEvent);
                }
                catch
                {
                    
                }
            }
        }

        private const string URI_TIME_CHANGED_EVENT = @"\\.\Notifications\NamedEvents\TimeChangedEvent";
        private const string URI_TIME_ZONE_CHANGED_EVENT = @"\\.\Notifications\NamedEvents\TimeZoneChangedEvent";

        private static void StopTimeChangedEvent()
        {
            try
            {
                DllCoredll.CeRunAppAtEvent(URI_TIME_CHANGED_EVENT, DllCoredll.NOTIFICATION_EVENT_NONE);
                if (_safeThreadTimeChange != null)
                {
                    try
                    {
                        _safeThreadTimeChange.Stop(500);
                    }
                    catch(Exception e)
                    {
                        HandledExceptionAdapter.Examine(e);
                    }
                    finally
                    {
                        _safeThreadTimeChange = null;    
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Log.Singleton.Error("Error stopping TimeChange event: " + ex.Message);
            }
        }

        private static bool ResolveTimeChangedRegistrationResult(int result)
        {
            if (result == 1)
            {
                return true;
            }

            try
            {
                _safeThreadTimeChange.Stop(500);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
            finally
            {
                _safeThreadTimeChange = null;
            }

            return false;
        }

        private static SafeThread _timeZoneChangedReportingThread = null;

        private static bool StartTimeZoneChangedEvent()
        {
            try
            {
                StartSafeThreadTimeZoneChange();

                //Remove existing calls CeRunAppAtEvent to TimeZoneChangeEvent (DllCoredll.NOTIFICATION_EVENT_NONE)
                DllCoredll.CeRunAppAtEvent(URI_TIME_ZONE_CHANGED_EVENT, DllCoredll.NOTIFICATION_EVENT_NONE);
                //Add TimeZoneChangeEvent event
                int result = DllCoredll.CeRunAppAtEvent(URI_TIME_ZONE_CHANGED_EVENT, DllCoredll.NOTIFICATION_EVENT_TZ_CHANGE);

                return ResolveTimeZoneChangedRegistrationResult(result);
            }
            catch { }

            return false;
        }

        private static void StartSafeThreadTimeZoneChange()
        {
            if (_timeZoneChangedReportingThread == null)
            {
                _timeZoneChangedReportingThread = new SafeThread(TimeZoneChangedReportingThread);
                _timeZoneChangedReportingThread.Start();
            }
        }

        private static void TimeZoneChangedReportingThread()
        {
            IntPtr hEvent = DllCoredll.CreateEvent(IntPtr.Zero, false, false, "TimeZoneChangedEvent");

            try
            {
                //infinite loop with waiting for the signal "time zone changed"
                while (DllCoredll.WaitForSingleObject(hEvent, DllCoredll.INFINITE) == 0)
                {
                    if (_timeZoneChanged != null)
                    {
                        _timeZoneChanged(TimeZone.CurrentTimeZone, GetLocalTime());
                    }
                }
            }
            catch
            {

            }
            finally
            {
                try
                {
                    if (hEvent != IntPtr.Zero)
                        DllCoredll.CloseHandle(hEvent);
                }
                catch
                {
                    
                }
            }
                
        }

        private static void StopTimeZoneChangedEvent()
        {
            try
            {
                DllCoredll.CeRunAppAtEvent(URI_TIME_ZONE_CHANGED_EVENT, DllCoredll.NOTIFICATION_EVENT_NONE);
                if (_timeZoneChangedReportingThread != null)
                {
                    try
                    {
                        _timeZoneChangedReportingThread.Stop(THREAD_WAIT_FOR_STOP);
                    }
                    catch (Exception e)
                    {
                        HandledExceptionAdapter.Examine(e);
                    }
                    finally
                    {
                        _timeZoneChangedReportingThread = null;
                    }
                }
            }
            catch { }
        }

        private const int THREAD_WAIT_FOR_STOP = 500;

        private static bool ResolveTimeZoneChangedRegistrationResult(int result)
        {
            if (result == 1)
            {
                return true;
            }

            try
            {
                _timeZoneChangedReportingThread.Stop(THREAD_WAIT_FOR_STOP);
                
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
            finally
            {
                _timeZoneChangedReportingThread = null;
            }
            return false;
        }

        private static uint _lastUptimeReadTimestamp = 0;
        private static uint _tickCountOverflowCount = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GetUpTime()
        {
            UptimeMonitoring(false);

            uint tickCount = (uint)Environment.TickCount;

            return ((long)_tickCountOverflowCount * uint.MaxValue) + tickCount;
        }

        private static Timer _timerUptimeMonitoring = null;
        private static Action<uint, uint> _uptimeSaveValues = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastUptimeReadTimestamp"></param>
        /// <param name="tickCountOverflowCount"></param>
        /// <param name="saveValues"></param>
        public static void InitUptimeMonitoring(uint lastUptimeReadTimestamp, uint tickCountOverflowCount, Action<uint, uint> saveValues)
        {
            _lastUptimeReadTimestamp = lastUptimeReadTimestamp;
            _tickCountOverflowCount = tickCountOverflowCount;

            if (_timerUptimeMonitoring == null)
            {
                _uptimeSaveValues += saveValues;
                _timerUptimeMonitoring = new Timer(OnTimerUptimeMonitoring, null, 0, 24 * 60 * 60 * 1000);
            }
        }

        private static void OnTimerUptimeMonitoring(object obj)
        {
            UptimeMonitoring(true);
        }

        private static void UptimeMonitoring(bool saveValues)
        {
            uint tickCount = (uint)Environment.TickCount;

            if (tickCount < _lastUptimeReadTimestamp)
            {
                _tickCountOverflowCount++;
            }

            _lastUptimeReadTimestamp = tickCount;

            if (saveValues && _uptimeSaveValues != null)
            {
                try
                {
                    _uptimeSaveValues(_lastUptimeReadTimestamp, _tickCountOverflowCount);
                }
                catch { }
            }
        }

        private static bool _useDateTimeNow = false;

        public static bool UseDateTimeNow
        {
            get
            {
                return _useDateTimeNow;
            }

            set
            {
                _useDateTimeNow = value;
            }
        }
    }
}
