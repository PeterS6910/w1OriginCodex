using System;

using System.Collections.Generic;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Threads;
using Contal.Drivers.LPC3250;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public class StateChangedDelegatesD2Type2Void
    {
        private readonly object _lockStateChangedDelegates = new object();
        Action<Guid, State> _stateChangedDelegates;

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

        public void AddStateChangedDelegates(Action<Guid, State> stateChangedDelegate)
        {
            lock (_lockStateChangedDelegates)
            {
                _stateChangedDelegates += stateChangedDelegate;
            }
        }

        public void RemoveStateChangedDelegates(Action<Guid, State> stateChangedDelegate)
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

    public class DailyPlans : DB.IDbObjectChangeListener<DB.DailyPlan>
    {
        private static volatile DailyPlans _singleton;
        
        private static object _syncRoot = new object();

        public const int SLEEP_AUTOCORRECTION_THREAD = 120000;
        public const int TOLERANCE_FOR_AUTOCORRECTION_THREAD = 60000;
        public const string THREAD_NAME_AUTO_CORRECTION = "DailyPlans: auto correction";
        public const string PROCESSING_QUEUE_NAME_STATE_CHANGED_EVENTS = "DailyPlans: state changed events";

        private readonly SyncDictionary<Guid, StateChangedDelegatesD2Type2Void> _evetnsStateChangedDailyPlan = new SyncDictionary<Guid, StateChangedDelegatesD2Type2Void>();
        private readonly Dictionary<Guid, DPSDPActualState> _actualStates = new Dictionary<Guid, DPSDPActualState>();
        private NativeTimer _timer;
        private readonly SyncDictionary<Guid, RemovedDailyPlan> _removedDailyPlans = new SyncDictionary<Guid, RemovedDailyPlan>();
        private readonly ThreadPoolQueue<TZDPEvent> _queueForEvents =
            new ThreadPoolQueue<TZDPEvent>(ThreadPoolGetter.Get());
      
        private readonly SafeThread _autocorrectionThread;
        private bool _isStartedAutocorrectionThread;
        private bool _safeThreadSleepCondition = true;

        public delegate void DailyPlansStateChangedDelegate(Guid DailyPlanId, State state);
        public event DailyPlansStateChangedDelegate DailyPlansStateChanged;


        public static DailyPlans Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new DailyPlans();
                    }

                return _singleton;
            }
        }

        public DailyPlans()
        {
            CcuCore.Singleton.BeforeExit += BeforeExit;
            Contal.IwQuick.Sys.Microsoft.SystemTime.RegisterTimeChangedEvent(OnTimeout);
            //_queueForEvents.ItemProcessing += RunEvent;
            _autocorrectionThread = new SafeThread(AutocorrectionThread, THREAD_NAME_AUTO_CORRECTION);
        }

 
        public void AddEvent(Guid guidDailyPlan, Action<Guid, State> eventStateChanged)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void DailyPlans.AddEvent(Guid guidDailyPlan, Action<Guid, State> eventStateChanged): [{0}]",
                        Log.GetStringFromParameters(guidDailyPlan, eventStateChanged)));

            if (eventStateChanged == null)
                return;

            StateChangedDelegatesD2Type2Void stateChangedDelegates;
            _evetnsStateChangedDailyPlan.GetOrAddValue(
                guidDailyPlan, 
                out stateChangedDelegates,
                key => (new StateChangedDelegatesD2Type2Void()),
                null);

            stateChangedDelegates.AddStateChangedDelegates(eventStateChanged);

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    "DP[" + guidDailyPlan + "]: add event - total count [" +
                    stateChangedDelegates.GetInvocationList().Length + "]");

            lock (_actualStates)
            {
                DPSDPActualState dpsdpActualState;
                if (_actualStates.TryGetValue(guidDailyPlan, out dpsdpActualState) && dpsdpActualState != null &&
                    dpsdpActualState.ActualState != State.Unknown)
                {
                    eventStateChanged(guidDailyPlan, dpsdpActualState.ActualState);
                }
            }
        }

        public void RemoveEvent(Guid guidDailyPlan, Action<Guid, State> eventStateChanged)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void DailyPlans.RemoveEvent(Guid guidDailyPlan, Action<Guid, State> eventStateChanged): [{0}]",
                        Log.GetStringFromParameters(guidDailyPlan, eventStateChanged)));

            StateChangedDelegatesD2Type2Void stateChangedDelegates;
            if (_evetnsStateChangedDailyPlan.TryGetValue(guidDailyPlan, out stateChangedDelegates) &&
                stateChangedDelegates != null)
            {
                stateChangedDelegates.RemoveStateChangedDelegates(eventStateChanged);
                if (stateChangedDelegates.IsEmpty)
                {
                    _evetnsStateChangedDailyPlan.Remove(guidDailyPlan);
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "DP[" + guidDailyPlan + "]: remove event - no events left");
                }
                else
                {
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () =>
                            "DP[" + guidDailyPlan + "]: remove event - events left [" +
                            stateChangedDelegates.GetInvocationList().Length + "]");
                }
            }
        }

        private void UnconfigureDailyPlan(Guid guidDailyPlan)
        {
            RemovedDailyPlan removedDailyPlan;

            _removedDailyPlans.GetOrAddValue(
                guidDailyPlan,
                out removedDailyPlan,
                key => (new RemovedDailyPlan(key)),
                null
                );
        }

        public void AddDailyPlans(ICollection<Guid> listDailyPlans)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () => string.Format("void DailyPlans.AddDailyPlans(IList<Guid> listDailyPlans): [{0}]",
                    Log.GetStringFromParameters(listDailyPlans)));

            ICollection<Guid> colGuidsToRemove = _removedDailyPlans.KeysSnapshot;
            var guidsToRemove = new List<Guid>();
            if (colGuidsToRemove != null && colGuidsToRemove.Count > 0)
            {
                guidsToRemove.AddRange(colGuidsToRemove);
            }


            if (guidsToRemove != null && guidsToRemove.Count > 0)
            {
                if (listDailyPlans != null && listDailyPlans.Count > 0)
                {
                    foreach (Guid guidDailyPlan in listDailyPlans)
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "DP[" + guidDailyPlan + "]: add");

                        if (guidsToRemove.Contains(guidDailyPlan))
                            guidsToRemove.Remove(guidDailyPlan);
                    }
                }

                foreach (Guid guidToRemove in guidsToRemove)
                {
                    _evetnsStateChangedDailyPlan.Remove(guidToRemove);

                    lock (_actualStates)
                    {
                        _actualStates.Remove(guidToRemove);
                    }

                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "DP[" + guidToRemove + "]: remove from states");
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "DP[" + guidToRemove + "]: remove from events");
                }
            }


            _removedDailyPlans.Clear();

            if ((listDailyPlans != null && listDailyPlans.Count > 0) ||
                (guidsToRemove != null && guidsToRemove.Count > 0))
            {
                OnTimeout();
            }
        }

        private void OnTimeout(DateTime dateTime)
        {
            OnTimeout();
        }

        private object _lockOnTimeout = new object();
        private void OnTimeout()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void DailyPlans.OnTimeout()");

            lock (_lockOnTimeout)
            {
                StopTimer();

                DateTime dateTime = CcuCore.LocalTime;
                DateTime actualDate = dateTime.Date;
                int minNextChangeTime = -1;
                bool runNextTimeout = false;

                var actualRemovedDailyPlanGuids = new Dictionary<Guid, byte>();
                ICollection<KeyValuePair<Guid, RemovedDailyPlan>> actualRemovedDailyPlan =
                    _removedDailyPlans.PairsSnapshot;

                if (actualRemovedDailyPlan != null && actualRemovedDailyPlan.Count > 0)
                {
                    runNextTimeout = true;

                    foreach (KeyValuePair<Guid, RemovedDailyPlan> kvp in actualRemovedDailyPlan)
                    {
                        RemovedDailyPlan removedDailyPlan = kvp.Value;
                        if (removedDailyPlan != null)
                        {
                            UpdateActualStateAndRunEvent(kvp.Key, removedDailyPlan.GetCurrentState(dateTime), dateTime,
                                removedDailyPlan.GetNextChangeTime(dateTime), ref minNextChangeTime);
                        }

                        actualRemovedDailyPlanGuids.Add(kvp.Key, 0);
                    }
                }

                var listGuidDailyPlans = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.DailyPlan);

                if (listGuidDailyPlans != null)
                    foreach (Guid guidDailyPlan in listGuidDailyPlans)
                    {
                        if (!actualRemovedDailyPlanGuids.ContainsKey(guidDailyPlan))
                        {
                            DB.DailyPlan dailyPlan =
                                Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.DailyPlan, guidDailyPlan) as DB.DailyPlan;
                            if (dailyPlan != null)
                            {
                                runNextTimeout = true;

                                int nextChangeTime = -1;
                                State actualState = dailyPlan.GetState(ref nextChangeTime);
                                UpdateActualStateAndRunEvent(guidDailyPlan, actualState, dateTime, nextChangeTime,
                                    ref minNextChangeTime);
                            }
                        }
                    }

                TimeZones.Singleton.OnTimeout();

                if (runNextTimeout)
                {
                    long timeoutLenght = 60000;

                    if (minNextChangeTime >= 0)
                    {
                        dateTime = CcuCore.LocalTime;
                        if (actualDate == dateTime.Date)
                        {
                            dateTime = CcuCore.LocalTime;
                            int actualMinute = dateTime.Hour * 60 + dateTime.Minute;

                            if (actualMinute < minNextChangeTime)
                                timeoutLenght = ((minNextChangeTime - actualMinute) * 60000) - dateTime.Second * 1000;
                            else
                                timeoutLenght = 0;
                        }
                        else
                        {
                            timeoutLenght = 0;
                        }
                    }

                    if (timeoutLenght > 0)
                    {
                        StartTimer(timeoutLenght);
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "DP: timeout scheduled [" + timeoutLenght + "] ms");
                    }
                    else
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "DP: timeout not scheduled (direct call)");
                        OnTimeout();
                    }
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

            _timer = NativeTimerManager.StartTimeout(timeoutLenght, OnTimeout, (byte)PrirotyForOnTimerEvent.DailyPlanSDP);
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

            if (_timer != null)
            {
                _timer.StopTimer();
                _timer = null;
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
                ASafeThreadBase.Sleep(SLEEP_AUTOCORRECTION_THREAD, ref _safeThreadSleepCondition);

                if (!CcuCore.Singleton.WasExited)
                {
                    bool runTimeout = false;
                    lock (_lockOnTimeout)
                    {
                        ulong correctedTickCount = CcuCore.Singleton.GetCEUpTime() - TOLERANCE_FOR_AUTOCORRECTION_THREAD;

                        lock (_actualStates)
                        {
                            foreach (KeyValuePair<Guid, DPSDPActualState> kvp in _actualStates)
                            {
                                DPSDPActualState dpsdpActualState = kvp.Value;
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

        private void UpdateActualStateAndRunEvent(Guid guidDialyPlan, State actualState, DateTime actualDateTime, int nextChangeTime, ref int minNextChangeTime)
        {
            int mnct = minNextChangeTime;
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => ("void DailyPlans.UpdateActualStateAndRunEvent(Guid guidDialyPlan, State actualState, int nextChangeTime, ref int minNextChangeTime): [" +
                Log.GetStringFromParameters(guidDialyPlan, actualState, nextChangeTime, mnct) + "]"));

            bool runEvent = false;

            if (nextChangeTime > 0 && (minNextChangeTime == -1 || nextChangeTime < minNextChangeTime))
                minNextChangeTime = nextChangeTime;

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "DP[" + guidDialyPlan + "]: state to [" + actualState + "]");

            ulong milisecondsToNextChanged = 0;
            int actualMinute = actualDateTime.Hour * 60 + actualDateTime.Minute;
            if (nextChangeTime > actualMinute)
                milisecondsToNextChanged = (ulong)((nextChangeTime - actualMinute) * 60 - actualDateTime.Second) * 1000;

            lock (_actualStates)
            {
                DPSDPActualState dpsdpActualState;
                if (_actualStates.TryGetValue(guidDialyPlan, out dpsdpActualState))
                {
                    if (dpsdpActualState != null)
                    {
                        runEvent = actualState != dpsdpActualState.ActualState;
                        dpsdpActualState.SetActualState(actualState, milisecondsToNextChanged);
                    }
                    else
                    {
                        _actualStates.Remove(guidDialyPlan);
                        _actualStates.Add(guidDialyPlan, new DPSDPActualState(actualState, milisecondsToNextChanged));
                        runEvent = true;
                    }
                }
                else
                {
                    _actualStates.Add(guidDialyPlan, new DPSDPActualState(actualState, milisecondsToNextChanged));
                    runEvent = true;
                }
            }

            if (runEvent)
            {
                try
                {
                    if (DailyPlansStateChanged != null)
                        DailyPlansStateChanged(guidDialyPlan, actualState);

                    StateChangedDelegatesD2Type2Void stateChangedDelegates;
                    if (_evetnsStateChangedDailyPlan.TryGetValue(guidDialyPlan, out stateChangedDelegates) && stateChangedDelegates != null)
                    {
                        Delegate[] eventDelegates = stateChangedDelegates.GetInvocationList();
                        if (eventDelegates != null)
                        {
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                                () =>
                                    "DP[" + guidDialyPlan + "]: run events [" + eventDelegates.Length +
                                    "] - state [" + actualState + "]");

                            foreach (Delegate eventDelegate in eventDelegates)
                            {
                                _queueForEvents.Enqueue(
                                    new TZDPEvent(
                                        (Action<Guid, State>)eventDelegate,
                                        guidDialyPlan, 
                                        actualState));
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    CcuCore.DebugLog.Error("DP: " + error);
                }
            }

            mnct = minNextChangeTime;
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, 
                () => "DP[" + guidDialyPlan + "]: nextChangeTime [" + nextChangeTime + "] - minNextChangeTime [" + mnct + "]");
        }

        private bool OnTimeout(NativeTimer timer)
        {
            OnTimeout();
            return true;
        }

        public State GetActualState(Guid guidDailyPlan)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State DailyPlans.GetActualState(Guid guidDailyPlan): [{0}]",
                Log.GetStringFromParameters(guidDailyPlan)));
            if (guidDailyPlan != Guid.Empty)
            {
                lock (_actualStates)
                {
                    DPSDPActualState dpsdpActualState;
                    if (_actualStates.TryGetValue(guidDailyPlan, out dpsdpActualState) && dpsdpActualState != null)
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                            () =>
                                string.Format("State DailyPlans.GetActualState return {0}[1]",
                                    Log.GetStringFromParameters(dpsdpActualState)));
                        return dpsdpActualState.ActualState;
                    }
                }
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL,()=>string.Format("State DailyPlans.GetActualState return {0}[2]", Log.GetStringFromParameters(State.Unknown)));
            return State.Unknown;
        }

        public void PrepareObjectUpdate(
            Guid idObject,
            DB.DailyPlan newObject)
        {
            UnconfigureDailyPlan(idObject);
        }

        public void OnObjectSaved(
            Guid idObject,
            DB.DailyPlan newObject)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            UnconfigureDailyPlan(idObject);
        }
    }

    public class RemovedDailyPlan
    {
        byte[] _arrayDayIntevals;

        public RemovedDailyPlan(Guid guidDailyPlan)
        {
            DB.DailyPlan dailyPlan = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.DailyPlan, guidDailyPlan) as DB.DailyPlan;
            if (dailyPlan != null)
            {
                _arrayDayIntevals = dailyPlan.ArrayDayIntervals;
            }
        }

        public State GetCurrentState(DateTime dateTime)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State DailyPlans.GetCurrentState(DateTime dateTime): [{0}]", Log.GetStringFromParameters(dateTime)));
            int actMinute = dateTime.Hour * 60 + dateTime.Minute;

            if (_arrayDayIntevals != null && actMinute < _arrayDayIntevals.Length)
            {
                State result = (State)_arrayDayIntevals[actMinute];
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State DailyPlans.GetCurrentState return {0}[1]", Log.GetStringFromParameters(result)));
                return result;
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL,()=>string.Format("State DailyPlans.GetCurrentState return {0}[2]", Log.GetStringFromParameters(State.Unknown)));
            return State.Unknown;
        }

        public int GetNextChangeTime(DateTime dateTime)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("int DailyPlans.GetNextChangeTime(DateTime dateTime): [{0}]", Log.GetStringFromParameters(dateTime)));
            try
            {
                int actMinute = dateTime.Hour * 60 + dateTime.Minute;

                if (_arrayDayIntevals != null && actMinute < _arrayDayIntevals.Length)
                {
                    byte actualState = _arrayDayIntevals[actMinute];

                    int minute = actMinute + 1;
                    while (minute < _arrayDayIntevals.Length && _arrayDayIntevals[minute] == actualState)
                        minute++;

                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("int DailyPlans.GetNextChangeTime return {0}", minute));
                    return minute;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL,()=>string.Format("int DailyPlans.GetNextChangeTime return {0}", -1));
            return -1;
        }
    }

    public class DPSDPActualState
    {
        private State _actualState;
        private ulong _lastTick;
        private ulong _nextTick;

        public State ActualState { get { return _actualState; } }
        public ulong LastTick { get { return _lastTick; } }
        public ulong NextTick { get { return _nextTick; } }

        public DPSDPActualState(State actualState, ulong milisecondsToNextChange)
        {
            _actualState = actualState;
            _lastTick = CcuCore.Singleton.GetCEUpTime();
            _nextTick = _lastTick + milisecondsToNextChange;
        }

        public void SetActualState(State actualState, ulong milisecondsToNextChange)
        {
            _actualState = actualState;
            _lastTick = CcuCore.Singleton.GetCEUpTime();
            _nextTick = _lastTick + milisecondsToNextChange;
        }
    }
}
