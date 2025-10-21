using System;

using System.Collections.Generic;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Sys.Microsoft;
using Microsoft.Win32;

using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Threads;
using Contal.Drivers.LPC3250;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public class SecurityDailyPlans :
        DB.IDbObjectChangeListener<DB.SecurityDailyPlan>
    {
        private static volatile SecurityDailyPlans _singleton;
        private static readonly object _syncRoot = new object();

        private const string CCU_REG_ENABLE_LOGGING_SDPSTZ_CHANGES = "EnableLoggingSDPSTZChanges";
        public const string THREAD_NAME_AUTO_CORRECTION = "SecurityDailyPlans: auto correction";
        public const string PROCESSING_QUEUE_NAME_STATE_CHANGED_EVENTS = "SecurityDailyPlans: state changed events";


        private readonly SyncDictionary<Guid, StateChangedDelegatesDType2Void> _eventsStateChangedSecurityDailyPlan = new SyncDictionary<Guid, StateChangedDelegatesDType2Void>();
        private readonly Dictionary<Guid, DPSDPActualState> _actualStates = new Dictionary<Guid, DPSDPActualState>();
        private NativeTimer _timeout;
        private readonly SyncDictionary<Guid, RemovedSecurityDailyPlan> _removedSecurityDailyPlans = new SyncDictionary<Guid, RemovedSecurityDailyPlan>();
        private bool _enableLoggingSDPSTZChanges;

        private readonly ThreadPoolQueue<STZSDPEvent> _queueForEvents =
            new ThreadPoolQueue<STZSDPEvent>(ThreadPoolGetter.Get());

        private readonly SafeThread _autocorrectionThread;
        private bool _isStartedAutocorrectionThread;
        private bool _safeThreadSleepCondition = true;

        public static SecurityDailyPlans Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new SecurityDailyPlans();
                    }

                return _singleton;
            }
        }

        public SecurityDailyPlans()
        {
            CcuCore.Singleton.BeforeExit += BeforeExit;
            SystemTime.RegisterTimeChangedEvent(OnTimeout);
            GetEnableLoggingSDPSTZChanges();
            _autocorrectionThread = new SafeThread(AutocorrectionThread, THREAD_NAME_AUTO_CORRECTION);
        }


        public void AddEvent(Guid guidSecurityDailyPlan, Action<State> eventStateChanged)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void SecurityDailyPlans.AddEvent(Guid guidSecurityDailyPlan, Action<State> eventStateChanged): [{0}]",
                        Log.GetStringFromParameters(guidSecurityDailyPlan, eventStateChanged)));

            if (eventStateChanged == null)
                return;

            StateChangedDelegatesDType2Void stateChangedDelegates;
            _eventsStateChangedSecurityDailyPlan.GetOrAddValue(
                guidSecurityDailyPlan, 
                out stateChangedDelegates,
                key => new StateChangedDelegatesDType2Void(),
                null
                );

            stateChangedDelegates.AddStateChangedDelegates(eventStateChanged);

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    "SDP[" + guidSecurityDailyPlan + "]: add event - total count [" +
                    stateChangedDelegates.GetInvocationList().Length + "]");

            lock (_actualStates)
            {
                DPSDPActualState dpsdpActualState;
                if (_actualStates.TryGetValue(guidSecurityDailyPlan, out dpsdpActualState) && dpsdpActualState != null &&
                    dpsdpActualState.ActualState != State.Unknown)
                {
                    eventStateChanged(dpsdpActualState.ActualState);
                }
            }
        }

        public void RemoveEvent(Guid guidSecurityDailyPlan, Action<State> eventStateChanged)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void SecurityDailyPlans.RemoveEvent(Guid guidSecurityDailyPlan, Action<State> eventStateChanged): [{0}]",
                        Log.GetStringFromParameters(guidSecurityDailyPlan, eventStateChanged)));

            StateChangedDelegatesDType2Void stateChangedDelegates;
            if (_eventsStateChangedSecurityDailyPlan.TryGetValue(guidSecurityDailyPlan, out stateChangedDelegates))
            {
                stateChangedDelegates.RemoveStateChangedDelegates(eventStateChanged);

                if (stateChangedDelegates.IsEmpty)
                {
                    _eventsStateChangedSecurityDailyPlan.Remove(guidSecurityDailyPlan);
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "SDP[" + guidSecurityDailyPlan + "]: remove event - no events left");
                }
                else
                {
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () =>
                            "SDP[" + guidSecurityDailyPlan + "]: remove event - events left [" +
                            stateChangedDelegates.GetInvocationList().Length + "]");
                }
            }
        }

        private void UnconfigureSecurityDailyPlan(Guid guidSecurityDailyPlan)
        {
            RemovedSecurityDailyPlan removedSecurityDailyPlan;

            _removedSecurityDailyPlans.GetOrAddValue(
                guidSecurityDailyPlan,
                out removedSecurityDailyPlan,
                key => new RemovedSecurityDailyPlan(key),
                null
                );
        }

        public void AddSecurityDailyPlans(ICollection<Guid> listSecurityDailyPlans)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    "void SecurityDailyPlans.AddSecurityDailyPlans(IList<Guid> listSecurityDailyPlans): [" +
                    Log.GetStringFromParameters(listSecurityDailyPlans) + "]");

            ICollection<Guid> colGuidsToRemove = _removedSecurityDailyPlans.KeysSnapshot;
            var guidsToRemove = new List<Guid>();
            if (colGuidsToRemove.Count > 0)
            {
                guidsToRemove.AddRange(colGuidsToRemove);
            }

            if (guidsToRemove.Count > 0)
            {
                if (listSecurityDailyPlans != null)
                {
                    foreach (var guidSecurityDailyPlan in listSecurityDailyPlans)
                    {
                        Guid plan = guidSecurityDailyPlan;
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                            () => "SDP[" + plan + "]: add");
                        if (guidsToRemove.Contains(guidSecurityDailyPlan))
                            guidsToRemove.Remove(guidSecurityDailyPlan);
                    }
                }

                foreach (var guidToRemove in guidsToRemove)
                {
                    _eventsStateChangedSecurityDailyPlan.Remove(guidToRemove);

                    lock (_actualStates)
                    {
                        _actualStates.Remove(guidToRemove);
                    }

                    Guid guidToRemove_ = guidToRemove;
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "SDP[" + guidToRemove_ + "]: remove from states");
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "SDP[" + guidToRemove_ + "]: remove from events");
                }
            }

            _removedSecurityDailyPlans.Clear();

            if ((listSecurityDailyPlans != null
                 && listSecurityDailyPlans.Count > 0)
                || guidsToRemove.Count > 0)
            {
                OnTimeout();
            }
        }

        private void OnTimeout(DateTime dateTime)
        {
            OnTimeout();
        }

        private readonly object _lockOnTimeout = new object();
        private void OnTimeout()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void SecurityDailyPlans.OnTimeout()");

            lock (_lockOnTimeout)
            {
                StopTimer();

                try
                {
                    var dateTime = CcuCore.LocalTime;
                    var actualDate = dateTime.Date;
                    var minNextChangeTime = -1;
                    var runNextTimeout = false;

                    var actualRemovedSecurityDailyPlanGuids = new Dictionary<Guid, byte>();
                    var actualRemovedSecurityDailyPlans =
                        _removedSecurityDailyPlans.PairsSnapshot;
                    
                    if (actualRemovedSecurityDailyPlans.Count > 0)
                    {
                        runNextTimeout = true;

                        foreach (var kvp in actualRemovedSecurityDailyPlans)
                        {
                            var removedSecurityDailyPlan = kvp.Value;
                            if (removedSecurityDailyPlan != null)
                            {
                                UpdateActualStateAndRunEvent(kvp.Key, removedSecurityDailyPlan.GetCurrentState(dateTime), dateTime,
                                    removedSecurityDailyPlan.GetNextChangeTime(dateTime), ref minNextChangeTime);
                            }

                            actualRemovedSecurityDailyPlanGuids.Add(kvp.Key, 0);
                        }
                    }

                    var listGuidSecurityDailyPlans = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.SecurityDailyPlan);
                    if (listGuidSecurityDailyPlans != null)
                        foreach (var guidSecurityDailyPlan in listGuidSecurityDailyPlans)
                            if (!actualRemovedSecurityDailyPlanGuids.ContainsKey(guidSecurityDailyPlan))
                            {
                                var securityDailyPlan =
                                    Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.SecurityDailyPlan, guidSecurityDailyPlan)
                                        as DB.SecurityDailyPlan;
                                if (securityDailyPlan != null)
                                {
                                    runNextTimeout = true;

                                    var nextChangeTime = -1;
                                    var actualState = securityDailyPlan.GetState(dateTime, ref nextChangeTime);
                                    UpdateActualStateAndRunEvent(guidSecurityDailyPlan, actualState, dateTime,
                                        nextChangeTime, ref minNextChangeTime);
                                }
                            }

                    SecurityTimeZones.Singleton.OnTimeout(dateTime);

                    if (runNextTimeout)
                    {
                        long timeoutLenght = 60000;

                        if (minNextChangeTime >= 0)
                        {
                            dateTime = CcuCore.LocalTime;
                            if (actualDate == dateTime.Date)
                            {
                                var actualMinute = dateTime.Hour * 60 + dateTime.Minute;

                                if (actualMinute < minNextChangeTime)
                                {
                                    timeoutLenght = ((minNextChangeTime - actualMinute) * 60000) - dateTime.Second * 1000;
                                }
                                else
                                    timeoutLenght = 0;
                            }
                            else
                            {
                                timeoutLenght = 0;
                            }
                        }

                        try
                        {
                            if (_enableLoggingSDPSTZChanges)
                            {
                                lock (_actualStates)
                                {
                                    foreach (var kvp in _actualStates)
                                    {
                                        if (kvp.Value != null)
                                        {
                                            Events.ProcessEvent(
                                                new EventSecurityTimeChannelChanged(
                                                    kvp.Value.ActualState,
                                                    kvp.Key,
                                                    ObjectType.SecurityDailyPlan));
                                        }
                                    }

                                    var seconds = (int)(timeoutLenght / 1000);
                                    dateTime = dateTime.AddSeconds(seconds);

                                    Events.ProcessEvent(
                                        new EventSecurityTimeChannelChanged(
                                            dateTime,
                                            seconds));
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }

                        if (timeoutLenght > 0)
                        {
                            StartTimer(timeoutLenght);
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "SDP: timeout scheduled [" + timeoutLenght + "] ms");
                        }
                        else
                        {
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "SDP: timeout not scheduled (direct call)");
                            OnTimeout();
                        }
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    StartTimer(60000);
                }
            }
        }

        /// <summary>
        /// Start timer for OnTimeout and resume _autocorrectionThread
        /// </summary>
        /// <param name="timeoutLenght"></param>
        private void StartTimer(long timeoutLenght)
        {
            if (_autocorrectionThread != null)
            {
                if (!_isStartedAutocorrectionThread)
                {
                    _autocorrectionThread.Start();
                    _isStartedAutocorrectionThread = true;
                }

                _autocorrectionThread.Resume();
            }

            _timeout = NativeTimerManager.StartTimeout(timeoutLenght, OnTimeout, (byte)PrirotyForOnTimerEvent.DailyPlanSDP);
        }

        /// <summary>
        /// Stop timer for OnTimeout and suspend _autocorrectionThread
        /// </summary>
        private void StopTimer()
        {
            if (_autocorrectionThread != null)
            {
                _autocorrectionThread.Suspend();
            }

            if (_timeout != null)
            {
                _timeout.StopTimer();
                _timeout = null;
            }
        }

        /// <summary>
        /// Check OnTimeout tick
        /// </summary>
        private void AutocorrectionThread()
        {
            while (true)
            {
                _autocorrectionThread.WaitForResume();
                ASafeThreadBase.Sleep(DailyPlans.SLEEP_AUTOCORRECTION_THREAD, ref _safeThreadSleepCondition);

                if (!CcuCore.Singleton.WasExited)
                {
                    var runTimeout = false;
                    lock (_lockOnTimeout)
                    {
                        var correctedTickCount = 
                            CcuCore.Singleton.GetCEUpTime() -
                                DailyPlans.TOLERANCE_FOR_AUTOCORRECTION_THREAD;

                        lock (_actualStates)
                        {
                            foreach (var kvp in _actualStates)
                            {
                                var dpsdpActualState = kvp.Value;
                                if (dpsdpActualState != null)
                                {
                                    if (dpsdpActualState.NextTick < correctedTickCount)
                                    {
                                        Events.ProcessEvent(
                                            new EventCcuTimingProblem(
                                                dpsdpActualState.ActualState,
                                                kvp.Key));

                                        runTimeout = true;
                                    }
                                }
                            }
                        }
                    }

                    if (runTimeout)
                    {
                        try
                        {
                            OnTimeout();
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void BeforeExit()
        {
            _safeThreadSleepCondition = false;
            _autocorrectionThread.Resume();
        }

        private void UpdateActualStateAndRunEvent(Guid guidSecurityDialyPlan, State actualState, DateTime actualDateTime, int nextChangeTime, ref int minNextChangeTime)
        {
            var mnct = minNextChangeTime;
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void SecurityDailyPlans.UpdateActualStateAndRunEvent(Guid guidSecurityDialyPlan, State actualState, int nextChangeTime, ref int minNextChangeTime): [" +
                Log.GetStringFromParameters(guidSecurityDialyPlan, actualState, nextChangeTime, mnct) + "]");

            bool runEvent;

            if (nextChangeTime > 0 && (minNextChangeTime == -1 || nextChangeTime < minNextChangeTime))
                minNextChangeTime = nextChangeTime;

            ulong milisecondsToNextChanged = 0;
            var actualMinute = actualDateTime.Hour * 60 + actualDateTime.Minute;
            if (nextChangeTime > actualMinute)
                milisecondsToNextChanged = (ulong)((nextChangeTime - actualMinute) * 60 - actualDateTime.Second) * 1000;

            lock (_actualStates)
            {
                DPSDPActualState dpsdpActualState;
                if (_actualStates.TryGetValue(guidSecurityDialyPlan, out dpsdpActualState))
                {
                    if (dpsdpActualState != null)
                    {
                        runEvent = actualState != dpsdpActualState.ActualState;
                        dpsdpActualState.SetActualState(actualState, milisecondsToNextChanged);
                    }
                    else
                    {
                        _actualStates.Remove(guidSecurityDialyPlan);
                        _actualStates.Add(guidSecurityDialyPlan, new DPSDPActualState(actualState, milisecondsToNextChanged));
                        runEvent = true;
                    }
                }
                else
                {
                    _actualStates.Add(guidSecurityDialyPlan, new DPSDPActualState(actualState, milisecondsToNextChanged));
                    runEvent = true;
                }
            }

            if (runEvent)
            {
                try
                {
                    StateChangedDelegatesDType2Void stateChangedDelegates;
                    if (_eventsStateChangedSecurityDailyPlan.TryGetValue(guidSecurityDialyPlan, out stateChangedDelegates))
                    {
                        var eventDelegates = stateChangedDelegates.GetInvocationList();
                        if (eventDelegates != null)
                        {
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                                () =>
                                    "SDP[" + guidSecurityDialyPlan + "]: run events [" +
                                    eventDelegates.Length + "] - state [" + actualState + "]");
                            foreach (var eventDelegate in eventDelegates)
                            {
                                _queueForEvents.Enqueue(new STZSDPEvent((Action<State>) eventDelegate, actualState));
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }

            mnct = minNextChangeTime;
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "SDP[" + guidSecurityDialyPlan + "]: nextChangeTime [" + nextChangeTime + "] - minNextChangeTime [" + mnct + "]");
        }

        private bool OnTimeout(NativeTimer timer)
        {
            _timeout = null;
            OnTimeout();

            return true;
        }

        public State GetActualState(Guid guidSecuritDailyPlan)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State SecurityDailyPlans.GetActualState(Guid guidSecuritDailyPlan): [{0}]", Log.GetStringFromParameters(guidSecuritDailyPlan)));
            if (guidSecuritDailyPlan != Guid.Empty)
            {
                lock (_actualStates)
                {
                    DPSDPActualState dpsdpActualState;
                    if (_actualStates.TryGetValue(guidSecuritDailyPlan, out dpsdpActualState) &&
                        dpsdpActualState != null)
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                            () =>
                                string.Format("State SecurityDailyPlans.GetActualState return {0}",
                                    Log.GetStringFromParameters(dpsdpActualState.ActualState)));
                        return dpsdpActualState.ActualState;
                    }
                }
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => string.Format("State SecurityDailyPlans.GetActualState return {0}", Log.GetStringFromParameters(State.Unknown)));
            return State.Unknown;
        }

        public void SetEnableLoggingSDPSTZChanges(bool enableLoggingSDPSTZChanges)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void SecurityDailyPlans.SetEnableLoggingSDPSTZChanges(bool enableLoggingSDPSTZChanges): [{0}]", Log.GetStringFromParameters(enableLoggingSDPSTZChanges)));
            _enableLoggingSDPSTZChanges = enableLoggingSDPSTZChanges;

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "SDP: SDP/STZ change logging set to [" + enableLoggingSDPSTZChanges + "]");

            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(CCU_REG_ENABLE_LOGGING_SDPSTZ_CHANGES, _enableLoggingSDPSTZChanges, RegistryValueKind.DWord);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void GetEnableLoggingSDPSTZChanges()
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    var enableLoggingSDPSTZChanges = Convert.ToInt32(registryKey.GetValue(CCU_REG_ENABLE_LOGGING_SDPSTZ_CHANGES));
                    _enableLoggingSDPSTZChanges = enableLoggingSDPSTZChanges != 0;
                    return;
                }
            }
            catch { }

            _enableLoggingSDPSTZChanges = false;
        }

        public bool IsEnableLoggingSDPSTZChanges()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool SecurityDailyPlans.IsEnableLoggingSDPSTZChanges()");
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool SecurityDailyPlans.IsEnableLoggingSDPSTZChanges return {0}", Log.GetStringFromParameters(_enableLoggingSDPSTZChanges)));
            return _enableLoggingSDPSTZChanges;
        }

        public void PrepareObjectUpdate(
            Guid idObject,
            DB.SecurityDailyPlan newObject)
        {
            UnconfigureSecurityDailyPlan(idObject);
        }

        public void OnObjectSaved(
            Guid idObject,
            DB.SecurityDailyPlan newObject)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            UnconfigureSecurityDailyPlan(idObject);
        }
    }

    public class RemovedSecurityDailyPlan
    {
        private readonly byte[] _arraySecurityDayIntevals;

        public RemovedSecurityDailyPlan(Guid guidSecurityDailyPlan)
        {
            var securityDailyPlan = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.SecurityDailyPlan, guidSecurityDailyPlan) as DB.SecurityDailyPlan;
            if (securityDailyPlan != null)
            {
                _arraySecurityDayIntevals = securityDailyPlan.ArraySecurityDayIntervals;
            }
        }

        public State GetCurrentState(DateTime dateTime)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State SecurityDailyPlans.GetCurrentState(DateTime dateTime): [{0}]", Log.GetStringFromParameters(dateTime)));
            var actMinute = dateTime.Hour * 60 + dateTime.Minute;

            if (_arraySecurityDayIntevals != null && actMinute < _arraySecurityDayIntevals.Length)
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State SecurityDailyPlans.GetCurrentState return {0}", Log.GetStringFromParameters(_arraySecurityDayIntevals[actMinute])));
                return (State)_arraySecurityDayIntevals[actMinute];
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => string.Format("State SecurityDailyPlans.GetCurrentState return {0}", Log.GetStringFromParameters(State.Unknown)));
            return State.Unknown;
        }

        public int GetNextChangeTime(DateTime dateTime)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("int SecurityDailyPlans.GetNextChangeTime(DateTime dateTime): [{0}]", Log.GetStringFromParameters(dateTime)));
            try
            {
                var actMinute = dateTime.Hour * 60 + dateTime.Minute;

                if (_arraySecurityDayIntevals != null && actMinute < _arraySecurityDayIntevals.Length)
                {
                    var actualState = _arraySecurityDayIntevals[actMinute];

                    var minute = actMinute + 1;
                    while (minute < _arraySecurityDayIntevals.Length && _arraySecurityDayIntevals[minute] == actualState)
                        minute++;

                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("int SecurityDailyPlans.GetNextChangeTime return {0}", Log.GetStringFromParameters(minute)));
                    return minute;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "int SecurityDailyPlans.GetNextChangeTime return -1");
            return -1;
        }
    }

    public class StateChangedDelegatesDType2Void
    {
        private readonly object _lockStateChangedDelegates = new object();
        Action<State> _stateChangedDelegates;

        public bool IsEmpty
        {
            get
            {
                lock (_lockStateChangedDelegates)
                {
                    return _stateChangedDelegates == null;
                }
            }
        }

        public void AddStateChangedDelegates(Action<State> stateChangedDelegate)
        {
            lock (_lockStateChangedDelegates)
            {
                _stateChangedDelegates += stateChangedDelegate;
            }
        }

        public void RemoveStateChangedDelegates(Action<State> stateChangedDelegate)
        {
            lock (_lockStateChangedDelegates)
            {
                _stateChangedDelegates -= stateChangedDelegate;
            }
        }

        public Delegate[] GetInvocationList()
        {
            lock (_lockStateChangedDelegates)
            {
                return _stateChangedDelegates != null ? _stateChangedDelegates.GetInvocationList() : null;
            }
        }
    }
}
