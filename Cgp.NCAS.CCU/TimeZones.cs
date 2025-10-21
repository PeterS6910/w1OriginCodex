using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;
using Contal.Cgp.Globals;
using Contal.Drivers.LPC3250;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Data;
using TimeZone = Contal.Cgp.NCAS.CCU.DB.TimeZone;

namespace Contal.Cgp.NCAS.CCU
{
    public class TimeZones : IDbObjectChangeListener<DB.TimeZone>
    {
        public const string PROCESSING_QUEUE_NAME_STATE_CHANGED_EVENTS = "TimeZones: state changed events";

        private static volatile TimeZones _singleton = null;
        private static object _syncRoot = new object();

        private readonly SyncDictionary<Guid, StateChangedDelegatesD2Type2Void> _eventsStateChangedTimeZone =
            new SyncDictionary<Guid, StateChangedDelegatesD2Type2Void>();

        private  readonly Dictionary<Guid, State> _actualStates = new Dictionary<Guid, State>();
        private readonly SyncDictionary<Guid, RemovedTimeZone> _removedTimeZones = new SyncDictionary<Guid, RemovedTimeZone>();

        private readonly ThreadPoolQueue<TZDPEvent> _queueForEvents =
            new ThreadPoolQueue<TZDPEvent>(ThreadPoolGetter.Get()); 

        public delegate void TimeZonesStateChangedDelegate(Guid TimeZoneId, State state);
        public event TimeZonesStateChangedDelegate TimeZonesStateChanged;

        public static TimeZones Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new TimeZones();
                    }

                return _singleton;
            }
        }

        public void AddEvent(Guid guidTimeZone, Action<Guid, State> eventStateChanged)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void TimeZones.AddEvent(Guid guidTimeZone, Action<Guid, State> eventStateChanged): [{0}]",
                        Log.GetStringFromParameters(guidTimeZone, eventStateChanged)));

            if (eventStateChanged == null)
                return;

            StateChangedDelegatesD2Type2Void stateChangedDelegates;
            _eventsStateChangedTimeZone.GetOrAddValue(
                guidTimeZone, 
                out stateChangedDelegates,
                (key) => new StateChangedDelegatesD2Type2Void(),
                null
                );

            stateChangedDelegates.AddStateChangedDelegates(eventStateChanged);

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    "TZ[" + guidTimeZone + "]: add event - total count [" +
                    stateChangedDelegates.GetInvocationList().Length + "]");

            lock (_actualStates)
            {
                State state;
                if (_actualStates.TryGetValue(guidTimeZone, out state) && state != State.Unknown)
                {
                    eventStateChanged(guidTimeZone, state);
                }
            }
        }

        public void RemoveEvent(Guid guidTimeZone, Action<Guid, State> eventStateChanged)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void TimeZones.RemoveEvent(Guid guidTimeZone, Action<Guid, State> eventStateChanged): [{0}]",
                        Log.GetStringFromParameters(guidTimeZone, eventStateChanged)));

            StateChangedDelegatesD2Type2Void stateChangedDelegates;
            if (_eventsStateChangedTimeZone.TryGetValue(guidTimeZone, out stateChangedDelegates))
            {
                stateChangedDelegates.RemoveStateChangedDelegates(eventStateChanged);

                if (stateChangedDelegates.IsEmpty)
                {
                    _eventsStateChangedTimeZone.Remove(guidTimeZone);
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "TZ[" + guidTimeZone.ToString() + "]: remove event - no events left");
                }
                else
                {
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () =>
                            "TZ[" + guidTimeZone.ToString() + "]: remove event - events left [" +
                            stateChangedDelegates.GetInvocationList().Length + "]");
                }
            }
        }

        private void UnconfigureTimeZone(Guid guidTimeZone)
        {
            RemovedTimeZone removedTimeZone;

            _removedTimeZones.GetOrAddValue(
                guidTimeZone, 
                out removedTimeZone,
                key => new RemovedTimeZone(key),
                null);
        }

        public void AddTimeZones(ICollection<Guid> listTimeZones)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format("void TimeZones.AddTimeZones(IList<Guid> listTimeZones): [{0}]",
                        Log.GetStringFromParameters(listTimeZones)));

            ICollection<Guid> colGuidsToRemove = _removedTimeZones.KeysSnapshot;
            List<Guid> guidsToRemove = new List<Guid>();
            if (colGuidsToRemove != null && colGuidsToRemove.Count > 0)
            {
                guidsToRemove.AddRange(colGuidsToRemove);
            }

            if (guidsToRemove != null && guidsToRemove.Count > 0)
            {
                if (listTimeZones != null)
                {
                    foreach (Guid guidTimeZone in listTimeZones)
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "TZ[" + guidTimeZone.ToString() + "]: add");

                        if (guidsToRemove.Contains(guidTimeZone))
                            guidsToRemove.Remove(guidTimeZone);
                    }
                }

                foreach (Guid guidToRemove in guidsToRemove)
                {
                    _eventsStateChangedTimeZone.Remove(guidToRemove);

                    lock (_actualStates)
                    {
                        _actualStates.Remove(guidToRemove);
                    }

                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "TZ[" + guidToRemove.ToString() + "]: remove from states");
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "TZ[" + guidToRemove.ToString() + "]: remove from events");
                }
            }

            _removedTimeZones.Clear();

            if ((listTimeZones != null
                 && listTimeZones.Count > 0)
                || guidsToRemove.Count > 0)
            {
                OnTimeout();
            }
        }

        public void OnTimeout()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void TimeZones.OnTimeout()");

            DateTime dateTime = CcuCore.LocalTime;

            ICollection<KeyValuePair<Guid, RemovedTimeZone>> actualRemovedTimeZones = _removedTimeZones.PairsSnapshot;
            if (actualRemovedTimeZones != null && actualRemovedTimeZones.Count > 0)
            {
                foreach (KeyValuePair<Guid, RemovedTimeZone> kvp in actualRemovedTimeZones)
                {
                    RemovedTimeZone removedTimeZone = kvp.Value;
                    if (removedTimeZone != null)
                    {
                        UpdateActualStateAndRunEvent(kvp.Key, removedTimeZone.GetCurrentState());
                    }
                }
            }

            var listGuidTimeZones = 
                Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.TimeZone);

            if (listGuidTimeZones != null)
                foreach (Guid guidTimeZone in listGuidTimeZones)
                    if (!_removedTimeZones.ContainsKey(guidTimeZone))
                        UpdateActualStateAndRunEvent(
                            guidTimeZone,
                            DB.TimeZone.CurrentState(guidTimeZone));
        }

        private void UpdateActualStateAndRunEvent(Guid guidTimeZone, State actualState)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void TimeZones.UpdateActualStateAndRunEvent(Guid guidTimeZone, State actualState): [" +
                Log.GetStringFromParameters(guidTimeZone, actualState) + "]");

            bool runEvent = false;

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "TZ[" + guidTimeZone.ToString() + "]: state to [" + actualState.ToString() + "]");

            lock (_actualStates)
            {
                State state;
                if (_actualStates.TryGetValue(guidTimeZone, out state))
                {
                    if (state != actualState)
                    {
                        _actualStates[guidTimeZone] = actualState;
                        runEvent = true;
                    }
                }
                else
                {
                    _actualStates.Add(guidTimeZone, actualState);
                    runEvent = true;
                }
            }

            if (runEvent)
            {
                try
                {
                    if (TimeZonesStateChanged != null)
                        TimeZonesStateChanged(guidTimeZone, actualState);

                    StateChangedDelegatesD2Type2Void stateChangedDelegates;
                    if (_eventsStateChangedTimeZone.TryGetValue(guidTimeZone, out stateChangedDelegates))
                    {
                        Delegate[] eventDelegates = stateChangedDelegates.GetInvocationList();
                        if (eventDelegates != null)
                        {
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                                () =>
                                    "TZ[" + guidTimeZone.ToString() + "]: run events [" + eventDelegates.Length +
                                    "] - state [" + actualState + "]");                            

                            foreach (Delegate eventDelegate in eventDelegates)
                            {
                                _queueForEvents.Enqueue(new TZDPEvent((Action<Guid, State>) eventDelegate,
                                    guidTimeZone, actualState));
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    CcuCore.DebugLog.Error("TZ: " + error.ToString());
                }
            }
        }

        private bool OnTimeout(NativeTimer timer)
        {
            OnTimeout();
            return true;
        }

        public State GetActualState(Guid guidTimeZone)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State TimeZones.GetActualState(Guid guidTimeZone): [{0}]", Log.GetStringFromParameters(guidTimeZone)));
            if (guidTimeZone != Guid.Empty)
            {
                lock (_actualStates)
                {
                    State state;
                    if (_actualStates.TryGetValue(guidTimeZone, out state))
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                            () =>
                                string.Format("State TimeZones.GetActualState return {0}",
                                    Log.GetStringFromParameters(state)));
                        return state;
                    }
                }
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL,()=>string.Format("State TimeZones.GetActualState return {0}", Log.GetStringFromParameters(State.Unknown)));
            return State.Unknown;
        }

        public void PrepareObjectUpdate(
            Guid idObject,
            TimeZone newObject)
        {
            UnconfigureTimeZone(idObject);
        }

        public void OnObjectSaved(
            Guid idObject,
            TimeZone newObject)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            UnconfigureTimeZone(idObject);
        }
    }

    public class RemovedTimeZone
    {
        List<Guid> _actualDailyPlans = null;

        public RemovedTimeZone(Guid guidTimeZone)
        {
            DB.TimeZone timeZone = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.TimeZone, guidTimeZone) as DB.TimeZone;
            if (timeZone != null && timeZone.GuidDateSettings != null)
            {
                _actualDailyPlans = timeZone.GetGuidsActualDailyPlans(CcuCore.LocalTime);
            }
        }

        public State GetCurrentState()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "State TimeZones.GetCurrentState()");
            if (_actualDailyPlans != null && _actualDailyPlans.Count > 0)
            {
                foreach (Guid guidDailyPlan in _actualDailyPlans)
                {
                    if (DailyPlans.Singleton.GetActualState(guidDailyPlan) == State.On)
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State TimeZones.GetCurrentState return {0}", Log.GetStringFromParameters(State.On)));
                        return State.On;
                    }
                }
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State TimeZones.GetCurrentState return {0}", Log.GetStringFromParameters(State.Off)));
            return State.Off;
        }
    }

    public class TZDPEvent : IProcessingQueueRequest
    {
        private readonly Action<Guid, State> _eventToRun;
        private readonly Guid _timeZoneGuid;
        private readonly State _actualState;

        public TZDPEvent(Action<Guid, State> eventToRun, Guid timeZoneGuid, State actualState)
        {
            _eventToRun = eventToRun;
            _timeZoneGuid = timeZoneGuid;
            _actualState = actualState;
        }


        public void Execute()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "void TimeZones.RunEvent()");

            if (_eventToRun != null)
                _eventToRun(
                    _timeZoneGuid, 
                    _actualState);
        }

        public void OnError(Exception error)
        {
            HandledExceptionAdapter.Examine(error);
        }
    }
}
