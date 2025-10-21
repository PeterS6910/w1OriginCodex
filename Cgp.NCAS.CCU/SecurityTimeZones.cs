using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.Globals;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public class SecurityTimeZones : IDbObjectChangeListener<DB.SecurityTimeZone>
    {
        public const string PROCESSING_QUEUE_NAME_STATE_CHANGED_EVENTS = "SecurityTimeZones: state changed events";

        private static volatile SecurityTimeZones _singleton;
        private static object _syncRoot = new object();

        private SyncDictionary<Guid, StateChangedDelegatesDType2Void> _eventsStateChangedSecurityTimeZone = new SyncDictionary<Guid, StateChangedDelegatesDType2Void>();
        private Dictionary<Guid, State> _actualStates = new Dictionary<Guid, State>();
        private readonly SyncDictionary<Guid, RemovedSecurityTimeZone> _removedSecurityTimeZones = new SyncDictionary<Guid, RemovedSecurityTimeZone>();
        private readonly ThreadPoolQueue<STZSDPEvent> _queueForEvents;

        public static SecurityTimeZones Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new SecurityTimeZones();
                    }

                return _singleton;
            }
        }

        public SecurityTimeZones()
        {
            _queueForEvents = new ThreadPoolQueue<STZSDPEvent>(ThreadPoolGetter.Get());
        }



        public void AddEvent(Guid guidSecurityTimeZone, Action<State> eventStateChanged)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void SecurityTimeZones.AddEvent(Guid guidSecurityTimeZone, Action<State> eventStateChanged): [{0}]",
                        Log.GetStringFromParameters(guidSecurityTimeZone, eventStateChanged)));
            
            if (eventStateChanged == null)
                return;

            StateChangedDelegatesDType2Void stateChangedDelegates;
            _eventsStateChangedSecurityTimeZone.GetOrAddValue(
                guidSecurityTimeZone, 
                out stateChangedDelegates,
                (key) => new StateChangedDelegatesDType2Void(),
                null
                );

            stateChangedDelegates.AddStateChangedDelegates(eventStateChanged);

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    "STZ[" + guidSecurityTimeZone.ToString() + "]: add event - total count [" +
                    stateChangedDelegates.GetInvocationList().Length + "]");

            lock (_actualStates)
            {
                State state;
                if (_actualStates.TryGetValue(guidSecurityTimeZone, out state) && state != State.Unknown)
                {
                    eventStateChanged(state);
                }
            }
        }

        public void RemoveEvent(Guid guidSecurityTimeZone, Action<State> eventStateChanged)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void SecurityTimeZones.RemoveEvent(Guid guidSecurityTimeZone, Action<State> eventStateChanged): [{0}]",
                Log.GetStringFromParameters(guidSecurityTimeZone, eventStateChanged)));

            StateChangedDelegatesDType2Void stateChangedDelegates;
            if (_eventsStateChangedSecurityTimeZone.TryGetValue(guidSecurityTimeZone, out stateChangedDelegates))
                {
                    stateChangedDelegates.RemoveStateChangedDelegates(eventStateChanged);

                    if (stateChangedDelegates.IsEmpty)
                    {
                        _eventsStateChangedSecurityTimeZone.Remove(guidSecurityTimeZone);
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "STZ[" + guidSecurityTimeZone.ToString() + "]: remove event - no events left");
                    }
                    else
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "STZ[" + guidSecurityTimeZone.ToString() + "]: remove event - events left [" + stateChangedDelegates.GetInvocationList().Length + "]");
                    }
                }
        }

        private void UnconfigureSecurityTimeZone(Guid guidSecurityTimeZone)
        {
            RemovedSecurityTimeZone removedSecurityTimeZone;

            _removedSecurityTimeZones.GetOrAddValue(
                guidSecurityTimeZone,
                out removedSecurityTimeZone,
                key => new RemovedSecurityTimeZone(key),
                null
                );
        }

        public void AddSecurityTimeZones(ICollection<Guid> listSecurityTimeZones)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void SecurityTimeZones.AddSecurityTimeZones(IList<Guid> listSecurityTimeZones): [{0}]",
                        Log.GetStringFromParameters(listSecurityTimeZones)));

            ICollection<Guid> colGuidsToRemove = _removedSecurityTimeZones.KeysSnapshot;
            List<Guid> guidsToRemove = new List<Guid>();
            if (colGuidsToRemove != null && colGuidsToRemove.Count > 0)
            {
                guidsToRemove.AddRange(colGuidsToRemove);
            }

            if (guidsToRemove != null && guidsToRemove.Count > 0)
            {
                if (listSecurityTimeZones != null)
                {
                    foreach (Guid guidTimeZone in listSecurityTimeZones)
                    {
                        if (guidsToRemove.Contains(guidTimeZone))
                            guidsToRemove.Remove(guidTimeZone);
                    }
                }

                foreach (Guid guidToRemove in guidsToRemove)
                {
                    _eventsStateChangedSecurityTimeZone.Remove(guidToRemove);

                    lock (_actualStates)
                    {
                        _actualStates.Remove(guidToRemove);
                    }

                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "STZ[" + guidToRemove.ToString() + "]: remove from states");
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () => "STZ[" + guidToRemove.ToString() + "]: remove from events");
                }
            }

            _removedSecurityTimeZones.Clear();

            if ((listSecurityTimeZones != null
                 && listSecurityTimeZones.Count > 0)
                || guidsToRemove.Count > 0)
            {
                OnTimeout(CcuCore.LocalTime);
            }
        }

        public void OnTimeout(DateTime dateTime)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void SecurityTimeZones.OnTimeout(DateTime dateTime): [{0}]", Log.GetStringFromParameters(dateTime)));

            ICollection<KeyValuePair<Guid, RemovedSecurityTimeZone>> actualRemovedSecurityTimeZones =
                _removedSecurityTimeZones.PairsSnapshot;

            if (actualRemovedSecurityTimeZones != null && actualRemovedSecurityTimeZones.Count > 0)
            {
                foreach (KeyValuePair<Guid, RemovedSecurityTimeZone> kvp in actualRemovedSecurityTimeZones)
                {
                    RemovedSecurityTimeZone removedSecurityTimeZone = kvp.Value;
                    if (removedSecurityTimeZone != null)
                    {
                        UpdateActualStateAndRunEvent(kvp.Key, removedSecurityTimeZone.GetCurrentState());
                    }
                }
            }

            var listGuidSecurityTimeZones = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.SecurityTimeZone);
            if (listGuidSecurityTimeZones != null)
            {
                foreach (Guid guidSecurityTimeZone in listGuidSecurityTimeZones)
                {
                    if (!_removedSecurityTimeZones.ContainsKey(guidSecurityTimeZone))
                    {
                        UpdateActualStateAndRunEvent(guidSecurityTimeZone, SecurityTimeZone.GetState(dateTime, guidSecurityTimeZone));
                    }
                }
            }

            try
            {
                if (SecurityDailyPlans.Singleton.IsEnableLoggingSDPSTZChanges())
                {
                    lock (_actualStates)
                    {
                        foreach (KeyValuePair<Guid, State> kvp in _actualStates)
                        {
                            Events.ProcessEvent(
                                new EventSecurityTimeChannelChanged(
                                    kvp.Value,
                                    kvp.Key,
                                    ObjectType.SecurityTimeZone));
                        }
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void UpdateActualStateAndRunEvent(Guid guidSecurityTimeZone, State actualState)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void SecurityTimeZones.UpdateActualStateAndRunEvent(Guid guidSecurityTimeZone, State actualState): [" +
                Log.GetStringFromParameters(guidSecurityTimeZone, actualState) + "]");

            bool runEvent = false;

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "STZ[" + guidSecurityTimeZone.ToString() + "]: state to [" + actualState.ToString() + "]");

            lock (_actualStates)
            {
                State state;
                if (_actualStates.TryGetValue(guidSecurityTimeZone, out state))
                {
                    if (state != actualState)
                    {
                        _actualStates[guidSecurityTimeZone] = actualState;
                        runEvent = true;
                    }
                }
                else
                {
                    _actualStates.Add(guidSecurityTimeZone, actualState);
                    runEvent = true;
                }
            }

            if (runEvent)
            {
                try
                {
                    StateChangedDelegatesDType2Void stateChangedDelegates;
                    if (_eventsStateChangedSecurityTimeZone.TryGetValue(guidSecurityTimeZone, out stateChangedDelegates))
                    {
                        Delegate[] eventDelegates = stateChangedDelegates.GetInvocationList();
                        if (eventDelegates != null)
                        {
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                                () =>
                                    "STZ[" + guidSecurityTimeZone.ToString() + "]: run events [" + eventDelegates.Length +
                                    "] - state [" + actualState + "]");

                            foreach (Delegate eventDelegate in eventDelegates)
                            {
                                _queueForEvents.Enqueue(new STZSDPEvent((Action<State>)eventDelegate, actualState));
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    CcuCore.DebugLog.Error("STZ: " + error.ToString());
                }
            }
        }

        public State GetActualState(Guid guidSecurityTimeZone)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State SecurityTimeZones.GetActualState(Guid guidSecurityTimeZone): [{0}]", Log.GetStringFromParameters(guidSecurityTimeZone)));
            if (guidSecurityTimeZone != Guid.Empty)
            {
                lock (_actualStates)
                {
                    State state;
                    if (_actualStates.TryGetValue(guidSecurityTimeZone, out state))
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                            () =>
                                string.Format("State SecurityTimeZones.GetActualState return {0}",
                                    Log.GetStringFromParameters(state)));
                        return state;
                    }
                }
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL,()=>string.Format("State SecurityTimeZones.GetActualState return {0}", Log.GetStringFromParameters(State.Unknown)));
            return State.Unknown;
        }

        public void PrepareObjectUpdate(
            Guid idObject,
            SecurityTimeZone newObject)
        {
            UnconfigureSecurityTimeZone(idObject);
        }

        public void OnObjectSaved(
            Guid idObject,
            SecurityTimeZone newObject)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            UnconfigureSecurityTimeZone(idObject);
        }
    }

    public class RemovedSecurityTimeZone
    {
        readonly List<Guid> _actualSecurityDailyPlans;

        public RemovedSecurityTimeZone(Guid guidSecurityTimeZone)
        {
            var securityTimeZone = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.SecurityTimeZone, guidSecurityTimeZone) as SecurityTimeZone;
            if (securityTimeZone != null && securityTimeZone.GuidDateSettings != null)
            {
                _actualSecurityDailyPlans = securityTimeZone.GetGuidsActualSecurityDailyPlans(CcuCore.LocalTime);
            }
        }

        public State GetCurrentState()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "State RemovedSecurityTimeZone.GetCurrentState()");
            if (_actualSecurityDailyPlans != null && _actualSecurityDailyPlans.Count > 0)
            {
                State state = State.Unknown;
                foreach (Guid guidSecurityDailyPlan in _actualSecurityDailyPlans)
                {
                    State actualState = SecurityDailyPlans.Singleton.GetActualState(guidSecurityDailyPlan);

                    if (SecurityTimeZone.GetPriorityFromState(state) < SecurityTimeZone.GetPriorityFromState(actualState))
                        state = actualState;
                }

                if (state != State.Unknown)
                {
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("State RemovedSecurityTimeZone.GetCurrentState return {0}", Log.GetStringFromParameters(state)));
                    return state;
                }
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL,()=>string.Format("State RemovedSecurityTimeZone.GetCurrentState return {0}", Log.GetStringFromParameters(State.locked)));
            return State.locked;
        }
    }

    public class STZSDPEvent : IProcessingQueueRequest
    {
        private readonly Action<State> _eventToRun;
        private readonly State _actualState;

        public STZSDPEvent(Action<State> eventToRun, State actualState)
        {
            _eventToRun = eventToRun;
            _actualState = actualState;
        }

        public void Execute()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void STZSDPEvent.RunEvent()");

            if (_eventToRun != null)
                _eventToRun(_actualState);
        }

        public void OnError(Exception error)
        {
            HandledExceptionAdapter.Examine(error);
        }
    }
}
