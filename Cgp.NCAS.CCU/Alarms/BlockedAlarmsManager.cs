using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    public sealed class BlockedAlarmsManager :
        ASingleton<BlockedAlarmsManager>,
        IDbObjectRemovalListener
    {
        internal class BlockedAlarmsManagerForAlarmType
        {
            private readonly AlarmType _alarmType;
            private bool _generalProcessEventIfBlocked;
            private bool _generalAlarmEnabled;
            private BlockingAlarmListener _generalBlockingAlarmListener;

            private readonly SyncDictionary<IdAndObjectType, bool> _specificAlarmEnabledByAlarmParentObject =
                new SyncDictionary<IdAndObjectType, bool>();

            private readonly SyncDictionary<IdAndObjectType, bool> _specificProcessEventIfBlockedByAlarmParentObject =
                new SyncDictionary<IdAndObjectType, bool>();

            private readonly SyncDictionary<IdAndObjectType, BlockingAlarmListener> _specificBlockingAlarmListenersByAlarmParentObject =
                new SyncDictionary<IdAndObjectType, BlockingAlarmListener>();

            private readonly object _lockConfigureUnconfigure = new object();

            private readonly SyncDictionary<AlarmKey, ICreateAlarmFactoryWhenAlarmWasEnabled> _notEnabledAlarmsByAlarmKey =
                new SyncDictionary<AlarmKey, ICreateAlarmFactoryWhenAlarmWasEnabled>();

            private readonly SyncDictionary<IdAndObjectType, ICollection<AlarmKey>> _notEnabledAlarmKeysByAlarmObject =
                new SyncDictionary<IdAndObjectType, ICollection<AlarmKey>>();

            public BlockedAlarmsManagerForAlarmType(AlarmType alarmType)
            {
                _alarmType = alarmType;

                _generalAlarmEnabled = true;
                _generalProcessEventIfBlocked = false;

                _generalBlockingAlarmListener = BlockingAlarmListener.CreateBlockedAlarmListener(
                    _alarmType,
                    false,
                    null,
                    BlockUnblockAlarms);
            }

            public void ConfigureGeneralBlockingAlarmListener(
                bool alarmEnabled,
                bool blockedAlarm,
                IdAndObjectType blockingOnOffObject,
                bool processEventIfBlocked)
            {
                lock (_lockConfigureUnconfigure)
                {
                    _generalBlockingAlarmListener.Uncofigure();

                    _generalAlarmEnabled = alarmEnabled;
                    _generalProcessEventIfBlocked = processEventIfBlocked;

                    _generalBlockingAlarmListener = BlockingAlarmListener.CreateBlockedAlarmListener(
                        _alarmType,
                        blockedAlarm,
                        blockingOnOffObject,
                        BlockUnblockAlarms);

                    if (alarmEnabled)
                    {
                        AddEnabledAlarms(_notEnabledAlarmsByAlarmKey.PairsSnapshot
                            .Where(
                                createAlarmFactoryByAlarmKey =>
                                    createAlarmFactoryByAlarmKey.Key.AlarmObject == null
                                    || !_specificAlarmEnabledByAlarmParentObject.ContainsKey(
                                        createAlarmFactoryByAlarmKey.Key.AlarmObject)));
                    }
                    else
                    {
                        AlarmsManager.Singleton.StopAlarms(
                            _alarmType,
                            alarm =>
                            {
                                var alarmObject = alarm.AlarmKey.AlarmObject;

                                if (alarmObject != null
                                    && _specificAlarmEnabledByAlarmParentObject.ContainsKey(alarmObject))
                                {
                                    return false;
                                }

                                AddNotEnabledAlarmToMemoryStructures(alarm);
                                return true;
                            });
                    }
                }
            }

            private void BlockUnblockAlarms(
                bool isBlocked,
                IdAndObjectType alarmParentObject)
            {
                if (alarmParentObject != null)
                {
                    BlockUnblockAlarmsWithSpecificBlockingAlarmListener(
                        isBlocked,
                        alarmParentObject);

                    return;
                }

                BlockUnblockAlarmsWithGeneralBlockingAlarmListener(isBlocked);
            }

            private void BlockUnblockAlarmsWithGeneralBlockingAlarmListener(bool isBlocked)
            {
                var objectsWhithSpecificBlockingAlarmListener = new HashSet<IdAndObjectType>(
                    _specificBlockingAlarmListenersByAlarmParentObject.KeysSnapshot);

                if (isBlocked)
                {
                    AlarmsManager.Singleton.BlockAlarmsGeneral(
                        _alarmType,
                        idAndObjectType =>
                            IsAlarmEnabled(idAndObjectType)
                            && (idAndObjectType == null
                                || !objectsWhithSpecificBlockingAlarmListener.Contains(idAndObjectType)));

                    return;
                }

                AlarmsManager.Singleton.UnblockAlarmsGeneral(
                    _alarmType,
                    idAndObjectType =>
                        IsAlarmEnabled(idAndObjectType)
                        && (idAndObjectType == null
                            || !objectsWhithSpecificBlockingAlarmListener.Contains(idAndObjectType)),
                    CreateEventAfterUnblocked);
            }

            private void CreateEventAfterUnblocked(Alarm alarm)
            {
                if (alarm.AlarmState != AlarmState.Alarm
                    || ProcessEventIfBlocked(alarm.AlarmKey.AlarmObject))
                {
                    return;
                }

                var iGetCreateAlarmFactory = alarm as IGetCreateAlarmFactoryWhenAlarmWasEnabled;

                if (iGetCreateAlarmFactory == null)
                    return;

                var createAlarmFactory = iGetCreateAlarmFactory.GetCreateAlarmFactory();

                if (createAlarmFactory == null)
                    return;

                createAlarmFactory.CreateEvent();
            }

            private void BlockUnblockAlarmsWithSpecificBlockingAlarmListener(
                bool isBlocked,
                IdAndObjectType alarmParentObject)
            {
                if (!IsAlarmEnabled(alarmParentObject))
                    return;

                if (isBlocked)
                {
                    AlarmsManager.Singleton.BlockAlarmsGeneral(
                        _alarmType,
                        alarmParentObject.Equals);

                    return;
                }

                AlarmsManager.Singleton.UnblockAlarmsGeneral(
                    _alarmType,
                    alarmParentObject.Equals,
                    CreateEventAfterUnblocked);
            }

            private void AddEnabledAlarms(IEnumerable<KeyValuePair<AlarmKey, ICreateAlarmFactoryWhenAlarmWasEnabled>> createAlarmFactoriesByAlarmKey)
            {
                if (createAlarmFactoriesByAlarmKey == null)
                    return;

                var enabledAlarms = new LinkedList<Alarm>();

                foreach (var createAlarmFactoryByAlarmKey in createAlarmFactoriesByAlarmKey)
                {
                    RemoveNotEnabledAlarmFromMemoryStructures(createAlarmFactoryByAlarmKey.Key);

                    var newAlarm = createAlarmFactoryByAlarmKey.Value.CreateAlarm(
                        !ProcessEventIfBlocked(createAlarmFactoryByAlarmKey.Key.AlarmObject));

                    if (newAlarm != null)
                        enabledAlarms.AddLast(newAlarm);
                }

                AlarmsManager.Singleton.AddEnabledAlarms(enabledAlarms);
            }

            private bool IsAlarmEnabled(IdAndObjectType alarmParentObject)
            {
                if (alarmParentObject == null)
                    return _generalAlarmEnabled;

                bool specificAlarmEnabled;

                return _specificAlarmEnabledByAlarmParentObject.TryGetValue(
                    alarmParentObject,
                    out specificAlarmEnabled)
                    ? specificAlarmEnabled
                    : _generalAlarmEnabled;
            }

            public void ConfigureSpecificBlockingAlarmListener(
                IdAndObjectType alarmParentObject,
                bool? alarmEnabled,
                bool? blockedAlarm,
                IdAndObjectType blockingOnOffObject,
                bool? processEventIfBlocked)
            {
                lock (_lockConfigureUnconfigure)
                {
                    UnconfigureSpecificBlockingAlarmListener(alarmParentObject);

                    if (alarmEnabled != null)
                    {
                        _specificAlarmEnabledByAlarmParentObject.Add(
                            alarmParentObject,
                            alarmEnabled.Value);
                    }

                    if (processEventIfBlocked != null)
                    {
                        _specificProcessEventIfBlockedByAlarmParentObject.Add(
                            alarmParentObject,
                            processEventIfBlocked.Value);
                    }

                    if (blockedAlarm != null)
                    {
                        var blockedAlarmListener = BlockingAlarmListener.CreateBlockedAlarmListener(
                            _alarmType,
                            alarmParentObject,
                            blockedAlarm.Value,
                            blockingOnOffObject,
                            BlockUnblockAlarms);

                        _specificBlockingAlarmListenersByAlarmParentObject.Add(
                            alarmParentObject,
                            blockedAlarmListener);
                    }
                    else
                    {
                        BlockUnblockAlarms(
                            _generalBlockingAlarmListener.IsBlocked,
                            alarmParentObject);
                    }

                    var isAlarmEnabled = alarmEnabled != null
                        ? alarmEnabled.Value
                        : _generalAlarmEnabled;

                    if (isAlarmEnabled)
                    {
                        AddEnabledAlarms(new LinkedList
                            <KeyValuePair<AlarmKey, ICreateAlarmFactoryWhenAlarmWasEnabled>>(
                            FindCreateAlarmFactoriesByAlarmKey(alarmParentObject)));
                    }
                    else
                    {
                        AlarmsManager.Singleton.StopAlarms(
                            _alarmType,
                            alarm =>
                            {
                                if (!alarmParentObject.Equals(alarm.AlarmKey.AlarmObject))
                                    return false;

                                AddNotEnabledAlarmToMemoryStructures(alarm);
                                return true;
                            });
                    }
                }
            }

            private IEnumerable<KeyValuePair<AlarmKey, ICreateAlarmFactoryWhenAlarmWasEnabled>>
                FindCreateAlarmFactoriesByAlarmKey(IdAndObjectType alarmParentObject)
            {
                var alarmKeys = FindAlarmKeys(alarmParentObject);

                if (alarmKeys == null)
                    yield break;

                foreach (var alarmKey in alarmKeys)
                {
                    ICreateAlarmFactoryWhenAlarmWasEnabled createAlarmFactory;
                    if (!_notEnabledAlarmsByAlarmKey.TryGetValue(
                        alarmKey,
                        out createAlarmFactory))
                    {
                        continue;
                    }

                    yield return new KeyValuePair<AlarmKey, ICreateAlarmFactoryWhenAlarmWasEnabled>(
                        alarmKey,
                        createAlarmFactory);
                }
            }

            public void ObjectRemovedFromDatabase(IdAndObjectType alarmParentObject)
            {
                lock (_lockConfigureUnconfigure)
                    UnconfigureSpecificBlockingAlarmListener(alarmParentObject);
            }

            private void UnconfigureSpecificBlockingAlarmListener(IdAndObjectType alarmParentObject)
            {
                _specificBlockingAlarmListenersByAlarmParentObject.Remove(
                    alarmParentObject,
                    (key, removed, remevedValue) =>
                    {
                        if (!removed)
                            return;

                        remevedValue.Uncofigure();
                    });

                _specificAlarmEnabledByAlarmParentObject.Remove(alarmParentObject);
                _specificProcessEventIfBlockedByAlarmParentObject.Remove(alarmParentObject);
            }

            public bool ProcessEvent(IdAndObjectType alarmParentObject)
            {
                if (!IsAlarmEnabled(alarmParentObject))
                    return false;

                return !IsAlarmBlocked(alarmParentObject)
                       || ProcessEventIfBlocked(alarmParentObject);
            }

            private bool ProcessEventIfBlocked(IdAndObjectType alarmParentObject)
            {
                if (alarmParentObject == null)
                    return _generalProcessEventIfBlocked;

                bool specificProcessEventIfBlocked;

                return _specificProcessEventIfBlockedByAlarmParentObject.TryGetValue(
                    alarmParentObject,
                    out specificProcessEventIfBlocked)
                    ? specificProcessEventIfBlocked
                    : _generalProcessEventIfBlocked;
            }

            public void Unconfigure()
            {
                lock (_lockConfigureUnconfigure)
                {
                    _specificBlockingAlarmListenersByAlarmParentObject.Clear(
                        (key, value) =>
                            value.Uncofigure(),
                        null);

                    _generalBlockingAlarmListener.Uncofigure();

                    _specificAlarmEnabledByAlarmParentObject.Clear();
                    _specificProcessEventIfBlockedByAlarmParentObject.Clear();
                }
            }

            public bool IsAlarmBlocked(IdAndObjectType alarmParentObject)
            {
                if (alarmParentObject == null)
                    return _generalBlockingAlarmListener.IsBlocked;

                BlockingAlarmListener specificBlockingAlarmListener;

                return _specificBlockingAlarmListenersByAlarmParentObject.TryGetValue(
                    alarmParentObject,
                    out specificBlockingAlarmListener)
                    ? specificBlockingAlarmListener.IsBlocked
                    : _generalBlockingAlarmListener.IsBlocked;
            }

            public bool AddNotEnabledAlarm(Alarm alarm)
            {
                if (IsAlarmEnabled(alarm.AlarmKey.AlarmObject))
                {
                    return false;
                }

                if (alarm.AlarmState == AlarmState.Alarm)
                {
                    AddNotEnabledAlarmToMemoryStructures(alarm);
                }

                return true;
            }

            private void AddNotEnabledAlarmToMemoryStructures(
                Alarm alarm)
            {
                var iGetCreateAlarmFactory = alarm as IGetCreateAlarmFactoryWhenAlarmWasEnabled;

                if (iGetCreateAlarmFactory == null)
                    return;

                var iCreateAlarmFactory = iGetCreateAlarmFactory.GetCreateAlarmFactory();

                if (iCreateAlarmFactory == null)
                    return;

                _notEnabledAlarmsByAlarmKey[alarm.AlarmKey] = iCreateAlarmFactory;

                if (alarm.AlarmKey.AlarmObject == null)
                    return;

                _notEnabledAlarmKeysByAlarmObject.GetOrAddValue(
                    alarm.AlarmKey.AlarmObject,
                    key =>
                        new HashSet<AlarmKey>(),
                    (key, value, newlyAdded) =>
                        value.Add(alarm.AlarmKey));
            }

            public void RemoveNotEnabledAlarm(AlarmKey alarmKey)
            {
                if (IsAlarmEnabled(alarmKey.AlarmObject))
                    return;

                RemoveNotEnabledAlarmFromMemoryStructures(alarmKey);
            }

            private void RemoveNotEnabledAlarmFromMemoryStructures(AlarmKey alarmKey)
            {
                _notEnabledAlarmsByAlarmKey.Remove(alarmKey);

                if (alarmKey.AlarmObject == null)
                    return;

                _notEnabledAlarmKeysByAlarmObject.Remove(
                    alarmKey.AlarmObject,
                    (IdAndObjectType key, ICollection<AlarmKey> possibleValueForRemove, out bool continueInRemove) =>
                    {
                        possibleValueForRemove.Remove(alarmKey);
                        continueInRemove = possibleValueForRemove.Count == 0;
                    },
                    null);
            }

            public void RemoveNotEnabledAlarms(IdAndObjectType alarmObject)
            {
                if (IsAlarmEnabled(alarmObject))
                    return;

                var alarmKeys = FindAlarmKeys(alarmObject);

                if (alarmKeys == null)
                    return;

                foreach (var alarmKey in alarmKeys)
                {
                    RemoveNotEnabledAlarmFromMemoryStructures(alarmKey);
                }
            }

            private IEnumerable<AlarmKey> FindAlarmKeys(IdAndObjectType alarmObject)
            {
                ICollection<AlarmKey> alarmKeys;

                return _notEnabledAlarmKeysByAlarmObject.TryGetValue(
                    alarmObject,
                    out alarmKeys)
                    ? new HashSet<AlarmKey>(alarmKeys)
                    : null;
            }
        }

        private readonly SyncDictionary<AlarmType, BlockedAlarmsManagerForAlarmType> _blockeAlarmManagersByAlarmType =
            new SyncDictionary<AlarmType, BlockedAlarmsManagerForAlarmType>();

        private BlockedAlarmsManager()
            : base(null)
        {

        }

        public void ConfigureGeneralBlocking(
            AlarmType alarmType,
            bool alarmEnabled,
            bool blockedAlarm,
            Guid? blockingOnOffObjectId,
            byte? blockingOnOffObjectType,
            bool processEventIfBlocked)
        {
            _blockeAlarmManagersByAlarmType.GetOrAddValue(
                alarmType,
                key =>
                    new BlockedAlarmsManagerForAlarmType(alarmType),
                (key, value, newlyAdded) =>
                    value.ConfigureGeneralBlockingAlarmListener(
                        alarmEnabled,
                        blockedAlarm,
                        GetBlockingOnOffObjectIdAndObjectType(
                            blockingOnOffObjectId,
                            blockingOnOffObjectType),
                        processEventIfBlocked));
        }

        private IdAndObjectType GetBlockingOnOffObjectIdAndObjectType(
            Guid? blockingOnOffObjectId,
            byte? blockingOnOffObjectType)
        {
            if (blockingOnOffObjectId == null
                || blockingOnOffObjectType == null)
            {
                return null;
            }

            return new IdAndObjectType(
                blockingOnOffObjectId.Value,
                (ObjectType) blockingOnOffObjectType.Value);
        }

        public void ConfigureSpecificBlockingForObject(
            AlarmType alarmType,
            IdAndObjectType alarmParentObject,
            bool? alarmEnabled,
            bool? blockedAlarm,
            Guid? blockingOnOffObjectId,
            byte? blockingOnOffObjectType,
            bool? processEventIfBlocked)
        {
            _blockeAlarmManagersByAlarmType.GetOrAddValue(
                alarmType,
                key =>
                    new BlockedAlarmsManagerForAlarmType(alarmType),
                (key, value, newlyAdded) =>
                    value.ConfigureSpecificBlockingAlarmListener(
                        alarmParentObject,
                        alarmEnabled,
                        blockedAlarm,
                        GetBlockingOnOffObjectIdAndObjectType(
                            blockingOnOffObjectId,
                            blockingOnOffObjectType),
                        processEventIfBlocked));
        }

        public bool ProcessEvent(
            AlarmType alarmType,
            IdAndObjectType alarmParentObject)
        {
            BlockedAlarmsManagerForAlarmType blockedAlarmsManagerForAlarmType;

            if (!_blockeAlarmManagersByAlarmType.TryGetValue(
                alarmType,
                out blockedAlarmsManagerForAlarmType))
            {
                return true;
            }

            return blockedAlarmsManagerForAlarmType.ProcessEvent(alarmParentObject);
        }

        public void Unconfigure()
        {
            _blockeAlarmManagersByAlarmType.Clear(
                (key, value) =>
                    value.Unconfigure(),
                null);
        }

        internal bool IsAlarmBlocked(AlarmKey alarmKey)
        {
            if (alarmKey == null)
                return false;

            BlockedAlarmsManagerForAlarmType blockedAlarmsManagerForAlarmType;

            if (!_blockeAlarmManagersByAlarmType.TryGetValue(
                alarmKey.AlarmType,
                out blockedAlarmsManagerForAlarmType))
            {
                return false;
            }

            return blockedAlarmsManagerForAlarmType.IsAlarmBlocked(alarmKey.AlarmObject);
        }

        internal bool AddNotEnabledAlarm(Alarm alarm)
        {
            if (alarm == null)
                return false;

            BlockedAlarmsManagerForAlarmType blockedAlarmsManagerForAlarmType;

            if (!_blockeAlarmManagersByAlarmType.TryGetValue(
                alarm.AlarmKey.AlarmType,
                out blockedAlarmsManagerForAlarmType))
            {
                return false;
            }

            return blockedAlarmsManagerForAlarmType.AddNotEnabledAlarm(alarm);
        }

        internal void RemoveNotEnabledAlarm(AlarmKey alarmKey)
        {
            if (alarmKey == null)
                return;

            BlockedAlarmsManagerForAlarmType blockedAlarmsManagerForAlarmType;

            if (!_blockeAlarmManagersByAlarmType.TryGetValue(
                alarmKey.AlarmType,
                out blockedAlarmsManagerForAlarmType))
            {
                return;
            }

            blockedAlarmsManagerForAlarmType.RemoveNotEnabledAlarm(alarmKey);
        }

        internal void RemoveNotEnabledAlarms(IdAndObjectType alarmObject)
        {
            _blockeAlarmManagersByAlarmType.ForEach(
                (key, value) =>
                    value.RemoveNotEnabledAlarms(alarmObject));
        }

        public void PrepareObjectDelete(
            Guid idObject,
            ObjectType objectType)
        {
            var idAndObjectType = new IdAndObjectType(
                idObject,
                objectType);

            _blockeAlarmManagersByAlarmType.ForEach(
                (key, value) =>
                    value.ObjectRemovedFromDatabase(idAndObjectType));
        }
    }
}
