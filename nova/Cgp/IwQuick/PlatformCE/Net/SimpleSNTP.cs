using System;
using System.Net;
using Contal.IwQuick;
using Contal.IwQuick.Net.Microsoft;
using Contal.IwQuick.Threads;

namespace Contal.IwQuick.Net
{
    enum ServerTypeAddressing
    {
        HostNames = 0,
        IPAddresses = 1,
        Unset = 2
    }

    /// <summary>
    /// Wrapper class for automatic time synchronization. Either host names or IP addresses
    /// can be used (in case both have been set, the latest configuration will be used)
    /// </summary>
    public class SimpleSNTP
    {
        private static volatile SimpleSNTP _singleton = null;
        private static readonly object _syncRoot = new object();

        private SimpleSNTP()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public static SimpleSNTP Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new SimpleSNTP();
                    }


                return _singleton;
            }
        }

        private readonly SNTP _sntp = new SNTP();

        /// <summary>
        /// Set value in miliseconds. Time won't be set if absolute difference between current time and time recieved from server is lower than this value.
        /// </summary>
        public uint AllowedTimeTolerance
        {
            get { return _sntp.AllowedTimeTolerance; }
            set { _sntp.AllowedTimeTolerance = value; }
        }

        private TimeSpan _pollingInterval = new TimeSpan(0, 1, 0);
        /// <summary>
        /// Interval in which the device will try to synchronize its time (TimeSpan)
        /// </summary>
        public TimeSpan PollingInterval
        {
            get { return _pollingInterval; }
            set
            {
                if (value.TotalMilliseconds > 0)
                    _pollingInterval = value;
            }
        }

        private volatile string[] _hostNames;
        /// <summary>
        /// 
        /// </summary>
        public string[] HostNames
        {
            get { return _hostNames; }
            set
            {
                lock (_updateSettingsLock)
                {
                    _hostNames = value;
                    _serverTypeAddressing = ServerTypeAddressing.HostNames;
                }
            }
        }

        private volatile IPAddress[] _ipAddresses;
        /// <summary>
        /// 
        /// </summary>
        public IPAddress[] IPAddresses
        {
            get { return _ipAddresses; }
            set
            {
                lock (_updateSettingsLock)
                {
                    _ipAddresses = value;
                    _serverTypeAddressing = ServerTypeAddressing.IPAddresses;
                }
            }
        }

        private volatile bool _isRunning = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        private ServerTypeAddressing _serverTypeAddressing;
        private SafeThread _pollingThread;
        private readonly object _updateSettingsLock = new object();

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            lock (_startStopLock)
            {
                if (!_isRunning)
                {
                    Validator.CheckInvalidArgument(_serverTypeAddressing, ServerTypeAddressing.Unset,
                        "No synchronization sources has been set");

                    _pollingThread = new SafeThread(PollNTPServers);
                    _pollingThread.Start();
                    _isRunning = true;
                }
            }
        }

        private readonly object _startStopLock = new object();
        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            lock (_startStopLock)
            {
                if (_isRunning)
                {
                    _isRunning = false;

                    if (null != _pollingThread)
                        _pollingThread.Stop(1000);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostNames"></param>
        public bool SynchronizeNow(string[] hostNames)
        {
            if (hostNames != null)
            {
                lock (_startStopLock)
                {
                    bool timeChanged;
                    return _sntp.Synchronize(out timeChanged, hostNames);
                }
            }

            return false;
        }
    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddresses"></param>
        public bool SynchronizeNow(params IPAddress[] ipAddresses)
        {
            if (ipAddresses != null &&
                ipAddresses.Length > 0)
            {
                lock (_startStopLock)
                {
                    bool timeChanged;
                    return _sntp.Synchronize(out timeChanged, ipAddresses);
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSynchronizedEvent"></param>
        public void BindTimeSynchronizedEvent(Action<DateTime> timeSynchronizedEvent)
        {
            try
            {
                _sntp.TimeSynchronized += timeSynchronizedEvent;
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSynchronizedEvent"></param>
        public void RemoveTimeSynchronizedEvent(Action<DateTime> timeSynchronizedEvent)
        {
            try
            {
                _sntp.TimeSynchronized -= timeSynchronizedEvent;
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSynchronizedEventWithPreviousTime"></param>
        public void BindTimeSynchronizedEventWithPreviousTime(Action<DateTime, DateTime> timeSynchronizedEventWithPreviousTime)
        {
            try
            {
                _sntp.TimeSynchronizedWithPreviousTime += timeSynchronizedEventWithPreviousTime;
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSynchronizedEventWithPreviousTime"></param>
        public void RemoveTimeSynchronizedEventWithPreviousTime(Action<DateTime, DateTime> timeSynchronizedEventWithPreviousTime)
        {
            try
            {
                _sntp.TimeSynchronizedWithPreviousTime -= timeSynchronizedEventWithPreviousTime;
            }
            catch { }
        }

        private volatile bool _timeWasSynchronized;

        /// <summary>
        /// 
        /// </summary>
        public bool TimeWasSynchronized
        {
            get { return _timeWasSynchronized; }
            set { _timeWasSynchronized = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public event DException2Void SynchronizationErrorOccured;
        private void InvokeSynchronizationErrorOccured(Exception e)
        {
            if (null == e)
                return;

            if (null != SynchronizationErrorOccured)
                try { SynchronizationErrorOccured(e); }
                catch { }
        }

        private void PollNTPServers()
        {
            while (_isRunning)
            {
                ServerTypeAddressing addressing;
                string[] hostNames;
                IPAddress[] ipAddresses;

                lock (_updateSettingsLock)
                {
                    addressing = _serverTypeAddressing;
                    hostNames = _hostNames;
                    ipAddresses = _ipAddresses;
                }

                switch(addressing)
                {
                    case ServerTypeAddressing.HostNames:
                        if (hostNames != null)
                            try
                            {
                                lock (_startStopLock)
                                {
                                    if (_isRunning)
#pragma warning disable 420
                                        _sntp.Synchronize(out _timeWasSynchronized, hostNames);
#pragma warning restore 420
                                }
                            }
                            catch (Exception e)
                            {
                                _timeWasSynchronized = false;
                                Sys.HandledExceptionAdapter.Examine(e);
                                InvokeSynchronizationErrorOccured(e);
                            }
                        break;
                    case ServerTypeAddressing.IPAddresses:
                        if (ipAddresses != null)
                        {
                            try
                            {
                                lock (_startStopLock)
                                {
                                    if (_isRunning)
#pragma warning disable 420
                                        _sntp.Synchronize(out _timeWasSynchronized, ipAddresses);
#pragma warning restore 420
                                }
                            }
                            catch (Exception e)
                            {
                                _timeWasSynchronized = false;
                                Sys.HandledExceptionAdapter.Examine(e);
                                InvokeSynchronizationErrorOccured(e);
                            }
                        }
                        break;
                    
                }

#pragma warning disable 420
                ASafeThreadBase.Sleep((int) _pollingInterval.TotalMilliseconds, ref _isRunning);
#pragma warning restore 420
                //Thread.Sleep((int)_pollingInterval.TotalMilliseconds);
            }
        }

        ~SimpleSNTP()
        {
            Stop();
        }
    }
}
