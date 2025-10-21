using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    public sealed class AlarmsManager :
        ASingleton<AlarmsManager>,
        IDbObjectRemovalListener
    {
        public interface IActionSources
        {
            bool ActionFromServer
            {
                get;
            }

            Guid IdCardReader
            {
                get;
            }

            Guid IdCard
            {
                get;
            }

            Guid IdPerson
            {
                get;
            }
        }

        private sealed class ActionFromServerSources :
            ASingleton<ActionFromServerSources>,
            IActionSources
        {
            private ActionFromServerSources()
                : base(null)
            {
            }

            public bool ActionFromServer
            {
                get { return true; }
            }

            public Guid IdCardReader
            {
                get { return Guid.Empty; }
            }

            public Guid IdCard
            {
                get { return Guid.Empty; }
            }

            public Guid IdPerson
            {
                get { return Guid.Empty; }
            }
        }

        public class ActionFromCcuSources : IActionSources
        {
            public ActionFromCcuSources(
                Guid idCardReader,
                AccessDataBase accessData)
            {
                IdCard = accessData.IdCard;
                IdCardReader = idCardReader;
                IdPerson = accessData.IdPerson;
            }

            public bool ActionFromServer
            {
                get { return false; }
            }

            public Guid IdCardReader
            {
                get;
                private set;
            }

            public Guid IdCard
            {
                get;
                private set;
            }

            public Guid IdPerson
            {
                get;
                private set;
            }
        }

        private class ProcessedResult<T>
        {
            private T _result;
            private readonly ManualResetEvent _eventProcessed = new ManualResetEvent(false);

            public T Result
            {
                get
                {
                    _eventProcessed.WaitOne();
                    return _result;
                }
            }

            public void SetResult(T result)
            {
                _result = result;
                _eventProcessed.Set();
            }
        }

        private interface IRemoveAlarm
        {
            void RemoveAlarm(Alarm alarm);
        }

        private sealed class SendEventAlarmRemovedToServer :
            ASingleton<SendEventAlarmRemovedToServer>,
            IRemoveAlarm
        {
            private SendEventAlarmRemovedToServer()
                : base(null)
            {
            }

            public void RemoveAlarm(Alarm alarm)
            {
                AlarmsManager.Singleton.SendAlarmEventToServer(
                    () =>
                        new AlarmRemoved(alarm.Id));
            }
        }

        private sealed class DirectRemoveAlarm :
            ASingleton<DirectRemoveAlarm>,
            IRemoveAlarm
        {
            private DirectRemoveAlarm()
                : base(null)
            {
            }

            public void RemoveAlarm(Alarm alarm)
            {
                AlarmsManager.Singleton.RemoveAlarmCore(
                    alarm,
                    this);
            }
        }

        private interface IAlarmAcknowledged : IRemoveAlarm
        {
            void SendEventAlarmAcknowledged(Guid idAlarm);
        }

        private sealed class AlarmAcknowledgedOnCcu :
            ASingleton<AlarmAcknowledgedOnCcu>,
            IAlarmAcknowledged
        {
            private AlarmAcknowledgedOnCcu()
                : base(null)
            {
            }

            public void SendEventAlarmAcknowledged(Guid idAlarm)
            {
                AlarmsManager.Singleton.SendAlarmEventToServer(
                    () =>
                        new AlarmAcknowledged(idAlarm));
            }

            public void RemoveAlarm(Alarm alarm)
            {
                AlarmsManager.Singleton.SendAlarmEventToServer(
                    () =>
                        new AlarmRemoved(alarm.Id));
            }
        }

        private sealed class AlarmAcknowledgedFromServer :
            ASingleton<AlarmAcknowledgedFromServer>,
            IAlarmAcknowledged
        {
            private AlarmAcknowledgedFromServer()
                : base(null)
            {
            }

            public void SendEventAlarmAcknowledged(Guid idAlarm)
            {

            }

            public void RemoveAlarm(Alarm alarm)
            {
                AlarmsManager.Singleton.SendAlarmEventToServer(
                    () =>
                        new AlarmRemoved(alarm.Id));
            }
        }

        private sealed class AlarmAcknowledgedFromServerAfterConnected :
            ASingleton<AlarmAcknowledgedFromServerAfterConnected>,
            IAlarmAcknowledged
        {
            private AlarmAcknowledgedFromServerAfterConnected()
                : base(null)
            {
            }

            public void SendEventAlarmAcknowledged(Guid idAlarm)
            {

            }

            public void RemoveAlarm(Alarm alarm)
            {
                AlarmsManager.Singleton.RemoveAlarmCore(
                    alarm,
                    this);
            }
        }

        private interface IAlarmBlockedIndividual
        {
            void SendEventAlarmIndividualBlockingChanged(
                Guid idAlarm,
                DateTime utcDateTime);
        }

        private sealed class AlarmBlockedIndividualOnCcu :
            ASingleton<AlarmBlockedIndividualOnCcu>,
            IAlarmBlockedIndividual
        {
            private AlarmBlockedIndividualOnCcu()
                : base(null)
            {
            }

            public void SendEventAlarmIndividualBlockingChanged(
                Guid idAlarm,
                DateTime utcDateTime)
            {
                AlarmsManager.Singleton.SendAlarmEventToServer(
                    () =>
                        new AlarmIndividualBlockingChanged(
                            idAlarm,
                            true,
                            utcDateTime));
            }
        }

        private sealed class AlarmBlockedIndividualFromServer :
            ASingleton<AlarmBlockedIndividualFromServer>,
            IAlarmBlockedIndividual
        {
            private AlarmBlockedIndividualFromServer()
                : base(null)
            {
            }

            public void SendEventAlarmIndividualBlockingChanged(
                Guid idAlarm,
                DateTime utcDateTime)
            {

            }
        }

        private interface IAlarmUnblockedIndividual : IRemoveAlarm
        {
            void SendEventAlarmIndividualBlockingChanged(
                Guid idAlarm,
                DateTime utcDateTime);
        }

        private sealed class AlarmUnblockedIndividualOnCcu :
            ASingleton<AlarmUnblockedIndividualOnCcu>,
            IAlarmUnblockedIndividual
        {
            private AlarmUnblockedIndividualOnCcu()
                : base(null)
            {
            }

            public void SendEventAlarmIndividualBlockingChanged(
                Guid idAlarm,
                DateTime utcDateTime)
            {
                AlarmsManager.Singleton.SendAlarmEventToServer(
                    () =>
                        new AlarmIndividualBlockingChanged(
                            idAlarm,
                            false,
                            utcDateTime));
            }

            public void RemoveAlarm(Alarm alarm)
            {
                AlarmsManager.Singleton.SendAlarmEventToServer(
                    () =>
                        new AlarmRemoved(alarm.Id));
            }
        }

        private sealed class AlarmUnblockedIndividualFromServer :
            ASingleton<AlarmUnblockedIndividualFromServer>,
            IAlarmUnblockedIndividual
        {
            private AlarmUnblockedIndividualFromServer()
                : base(null)
            {
            }

            public void SendEventAlarmIndividualBlockingChanged(
                Guid idAlarm,
                DateTime utcDateTime)
            {

            }

            public void RemoveAlarm(Alarm alarm)
            {
                AlarmsManager.Singleton.SendAlarmEventToServer(
                    () =>
                        new AlarmRemoved(alarm.Id));
            }
        }

        private sealed class AlarmUnblockedIndividualFromServerAfterConnected :
            ASingleton<AlarmUnblockedIndividualFromServerAfterConnected>,
            IAlarmUnblockedIndividual
        {
            private AlarmUnblockedIndividualFromServerAfterConnected()
                : base(null)
            {
            }

            public void SendEventAlarmIndividualBlockingChanged(
                Guid idAlarm,
                DateTime utcDateTime)
            {

            }

            public void RemoveAlarm(Alarm alarm)
            {
                AlarmsManager.Singleton.RemoveAlarmCore(
                    alarm,
                    this);
            }
        }

        private readonly SyncDictionary<AlarmKey, Alarm> _alarmsByAlarmKey =
        new SyncDictionary<AlarmKey, Alarm>();

        private readonly SyncDictionary<Guid, Alarm> _alarmsById =
            new SyncDictionary<Guid, Alarm>();

        private readonly SyncDictionary<IdAndObjectType, ICollection<Alarm>> _alarmsByAlarmObject =
            new SyncDictionary<IdAndObjectType, ICollection<Alarm>>();

        private readonly SyncDictionary<AlarmType, ICollection<Alarm>> _alarmsByAlarmType =
            new SyncDictionary<AlarmType, ICollection<Alarm>>();

        private readonly object _lockQueueAlarmActions = new object();

        private bool _serverConnected;

        private readonly ICollection<AlarmAction> _alarmActions = new LinkedList<AlarmAction>();

        private ThreadPoolQueue<AlarmAction> _queueAlarmActions;

        public event Action<Alarm> AlarmAdded;

        public event Action<Alarm, IActionSources> AlarmAcknowledged;

        public event Action<Alarm> AlarmStopped;

        public event Action<Alarm, IActionSources> AlarmBlockedIndividual;

        public event Action<Alarm, IActionSources> AlarmUnblockedIndividual;

        private readonly SyncDictionary<Guid, ICollection<Guid>> _backAlarmReferencesById =
            new SyncDictionary<Guid, ICollection<Guid>>();

        private AlarmsManager()
            : base(null)
        {

        }

        public void Init()
        {
            var loadFromDtabasePostProcessingActions = new LinkedList<Action>();

            var alarms = Database.ConfigObjectsEngine.AlarmsStorage.GetAllAlarms();

            if (alarms != null)
                foreach (var alarm in alarms)
                    try
                    {
                        if (alarm == null)
                            continue;

                        AddAlarmToMemoryStructures(alarm);

                        var alarmLoadFromDatabasePostProcessing = alarm as IAlarmLoadFromDatabasePostProcessing;

                        if (alarmLoadFromDatabasePostProcessing == null)
                            continue;

                        Action loadFromDatabasePostProcessingAction =
                            alarmLoadFromDatabasePostProcessing.LoadFromDatabasePostProcessing;

                        loadFromDtabasePostProcessingActions.AddLast(loadFromDatabasePostProcessingAction);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                        Database.ConfigObjectsEngine.AlarmsStorage.DeleteAlarm(alarm);
                    }

            lock (_lockQueueAlarmActions)
            {
                _queueAlarmActions = new ThreadPoolQueue<AlarmAction>(ThreadPoolGetter.Get());

                foreach (var loadFromDtabasePostProcessingAction in loadFromDtabasePostProcessingActions)
                {
                    try
                    {
                        loadFromDtabasePostProcessingAction();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }

                foreach (var alarmAction in _alarmActions)
                {
                    _queueAlarmActions.Enqueue(alarmAction);
                }

                _alarmActions.Clear();
            }
        }

        private class AlarmAction : IProcessingQueueRequest
        {
            private readonly Action _action;

            public AlarmAction([NotNull] Action action)
            {
                _action = action;
            }

            public void Execute()
            {
                _action();
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void AddAlarm(
            Alarm newAlarm)
        {
            AddAlarm(
                newAlarm,
                null);
        }

        public void AddAlarm(
            Alarm newAlarm,
            Alarm referencedAlarm)
        {
            Enqueue(
                () =>
                    InternalAddAlarm(
                        newAlarm,
                        referencedAlarm));
        }

        private void InternalAddAlarm(
            Alarm newAlarm,
            Alarm referencedAlarm)
        {
            if (newAlarm == null)
                return;

            if (BlockedAlarmsManager.Singleton.AddNotEnabledAlarm(newAlarm))
                return;

            InternalAddEnabledAlarm(
                newAlarm,
                referencedAlarm);
        }

        private void InternalAddEnabledAlarm(
            Alarm newAlarm,
            Alarm referencedAlarm)
        {
            if (referencedAlarm != null)
            {
                newAlarm.AlarmKey.ReferencedAlarmId = referencedAlarm.Id;

                if (FindAlarm(referencedAlarm.AlarmKey) == null)
                    CreateAlarm(referencedAlarm);
            }

            var oldAlarm = FindAlarm(newAlarm.AlarmKey);

            if (oldAlarm == null)
            {
                if (BlockedAlarmsManager.Singleton.IsAlarmBlocked(newAlarm.AlarmKey))
                    newAlarm.BlockAlarmGeneral();

                CreateAlarm(newAlarm);

                RunAlarmAdded(newAlarm);

                if (!newAlarm.IsBlocked)
                    CreateEventAlarmOccured(newAlarm as ICreateEventAlarmOccured);

                return;
            }

            if (newAlarm.AlarmState == AlarmState.Alarm
                && oldAlarm.AlarmState == AlarmState.Alarm
                && newAlarm.AlarmKey.SameExtendedObjects(oldAlarm.AlarmKey)
                && newAlarm.AlarmKey.SameParameters(oldAlarm.AlarmKey))
            {
                return;
            }

            UpdateAlarm(
                oldAlarm,
                newAlarm.AlarmState,
                newAlarm.CreatedDateTime,
                newAlarm.AlarmKey.ExtendedObjects,
                newAlarm.AlarmKey.Parameters);

            RunAlarmAdded(oldAlarm);

            if (!newAlarm.IsBlocked)
                CreateEventAlarmOccured(oldAlarm as ICreateEventAlarmOccured);
        }

        public void AddEnabledAlarms(IEnumerable<Alarm> enabledAlarms)
        {
            Enqueue(
                () =>
                    InternalAddEnabledAlarms(
                        enabledAlarms));
        }

        private void InternalAddEnabledAlarms(IEnumerable<Alarm> enabledAlarms)
        {
            if (enabledAlarms == null)
                return;

            foreach (var enabledAlarm in enabledAlarms)
            {
                InternalAddEnabledAlarm(
                    enabledAlarm,
                    null);
            }
        }

        private void RunAlarmAdded(Alarm alarm)
        {
            if (AlarmAdded == null)
                return;

            try
            {
                AlarmAdded(alarm);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private Alarm FindAlarm(AlarmKey alarmKey)
        {
            Alarm alarm;

            return _alarmsByAlarmKey.TryGetValue(
                alarmKey,
                out alarm)
                ? alarm
                : null;
        }

        private Alarm FindAlarm(Guid idAlarm)
        {
            Alarm alarm;

            return _alarmsById.TryGetValue(
                idAlarm,
                out alarm)
                ? alarm
                : null;
        }

        private IEnumerable<Alarm> FindAlarms(IdAndObjectType alarmObject)
        {
            ICollection<Alarm> alarms;

            return _alarmsByAlarmObject.TryGetValue(
                alarmObject,
                out alarms)
                ? new LinkedList<Alarm>(alarms)
                : null;
        }

        private void CreateAlarm(
            Alarm newAlarm)
        {
            AddAlarmToMemoryStructures(newAlarm);

            Database.ConfigObjectsEngine.AlarmsStorage.SaveAlarm(newAlarm);

            SendAlarmEventToServer(
                () =>
                    new AlarmAdded(newAlarm));
        }

        private static void CreateEventAlarmOccured(ICreateEventAlarmOccured iCreateEventAlarmOccured)
        {
            if (iCreateEventAlarmOccured == null)
                return;

            var eventParameters = iCreateEventAlarmOccured.CreateEventAlarmOccured();

            Events.ProcessEvent(eventParameters);
        }

        private void SendAlarmEventToServer(Func<AlarmEvent> lambdaCreateAlarmEvent)
        {
            if (!_serverConnected)
                return;

            SendAlarmsToServerDispatcher.Singleton.SendAlarmEvent(
                lambdaCreateAlarmEvent());
        }

        private void AddAlarmToMemoryStructures(Alarm alarm)
        {
            if (alarm.AlarmKey.ReferencedAlarmId != Guid.Empty)
            {
#if DEBUG
                var referencedAlarm = FindAlarm(
                    alarm.AlarmKey.ReferencedAlarmId);

                if (referencedAlarm != null
                    && referencedAlarm.AlarmKey.ReferencedAlarmId != Guid.Empty)
                {
                    DebugHelper.TryBreak(
                        "The referenced alarm has reference to another alarm");
                }
#endif

                _backAlarmReferencesById.GetOrAddValue(
                    alarm.AlarmKey.ReferencedAlarmId,
                    key =>
                        new HashSet<Guid>(),
                    (key, value, newlyAdded) =>
                        value.Add(alarm.Id));
            }

            _alarmsByAlarmKey.Add(
                alarm.AlarmKey,
                alarm);

            _alarmsById.Add(
                alarm.Id,
                alarm);

            _alarmsByAlarmType.GetOrAddValue(
                alarm.AlarmKey.AlarmType,
                key =>
                    new HashSet<Alarm>(),
                (key, value, newlyAdded) =>
                    value.Add(alarm));

            if (alarm.AlarmKey.AlarmObject == null)
                return;

            _alarmsByAlarmObject.GetOrAddValue(
                alarm.AlarmKey.AlarmObject,
                key =>
                    new HashSet<Alarm>(),
                (key, value, newlyAdded) =>
                    value.Add(alarm));
        }

        private void UpdateAlarm(
            Alarm oldAlarm,
            AlarmState alarmState,
            DateTime dateTime,
            IEnumerable<IdAndObjectType> extendedObjects,
            IEnumerable<AlarmParameter> parameters)
        {
            oldAlarm.AlarmState = alarmState;
            oldAlarm.CreatedDateTime = dateTime;
            oldAlarm.IsAcknowledged = false;
            oldAlarm.AlarmKey.SetExtendedObjects(extendedObjects);
            oldAlarm.AlarmKey.SetParameters(parameters);

            Database.ConfigObjectsEngine.AlarmsStorage.UpdateAlarm(oldAlarm);

            SendAlarmEventToServer(
                () =>
                    new AlarmAdded(oldAlarm));
        }

        public void StopAlarm(
            AlarmKey alarmKey)
        {
            Enqueue(
                () =>
                    InternalStopAlarm(alarmKey));
        }

        private void InternalStopAlarm(
            AlarmKey alarmKey)
        {
            BlockedAlarmsManager.Singleton.RemoveNotEnabledAlarm(alarmKey);

            var oldAlarm = FindAlarm(alarmKey);

            if (oldAlarm == null)
                return;

            InternalStopAlarm(oldAlarm);
        }

        private void InternalStopAlarm(
            Alarm oldAlarm)
        {
            if (oldAlarm.AlarmState == AlarmState.Normal)
                return;

            oldAlarm.AlarmState = AlarmState.Normal;

            RunAlarmStopped(oldAlarm);

            if (RemoveAlarm(
                oldAlarm,
                SendEventAlarmRemovedToServer.Singleton))
            {
                if (!oldAlarm.IsBlocked)
                    CreateEventAlarmChanged(oldAlarm as ICreateEventAlarmChanged);

                return;
            }

            Database.ConfigObjectsEngine.AlarmsStorage.UpdateAlarm(oldAlarm);

            SendAlarmEventToServer(
                () =>
                    new AlarmStopped(oldAlarm.Id));

            if (!oldAlarm.IsBlocked)
                CreateEventAlarmChanged(oldAlarm as ICreateEventAlarmChanged);
        }

        private void RunAlarmStopped(
            Alarm alarm)
        {
            if (AlarmStopped == null)
                return;

            try
            {
                AlarmStopped(alarm);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void StopAlarms(
            AlarmType alarmType,
            Func<Alarm, bool> filterForAlarms)
        {
            Enqueue(
                () =>
                    InternalStopAlarms(
                        alarmType,
                        filterForAlarms));
        }

        private void InternalStopAlarms(
            AlarmType alarmType,
            Func<Alarm, bool> filterForAlarms)
        {
            var alarms = FindAlarms(alarmType);

            if (alarms == null)
                return;

            foreach (var alarm in alarms)
            {
                if (alarm.AlarmState != AlarmState.Alarm)
                    continue;

                if (filterForAlarms != null
                    && !filterForAlarms(alarm))
                {
                    continue;
                }

                InternalStopAlarm(alarm);
            }
        }

        private static void CreateEventAlarmChanged(ICreateEventAlarmChanged iCreateEventAlarmChanged)
        {
            if (iCreateEventAlarmChanged == null)
                return;

            var eventParameters = iCreateEventAlarmChanged.CreateEventAlarmChanged();

            Events.ProcessEvent(eventParameters);
        }

        private void Enqueue(Action item)
        {
            var alarmAction = new AlarmAction(item);

            if (_queueAlarmActions == null)
                lock (_lockQueueAlarmActions)
                {
                    if (_queueAlarmActions == null)
                    {
                        _alarmActions.Add(alarmAction);
                        return;
                    }
                }

            _queueAlarmActions.Enqueue(alarmAction);
        }

        public void AcknowledgeAlarm(
            AlarmKey alarmKey,
            [NotNull]
            IActionSources acknowledgeSources)
        {
            Enqueue(
                () =>
                    InternalAcknowledgeAlarm(
                        alarmKey,
                        acknowledgeSources));
        }

        private void InternalAcknowledgeAlarm(
            AlarmKey alarmKey,
            [NotNull]
            IActionSources acknowledgeSources)
        {
            var alarm = FindAlarm(alarmKey);

            if (alarm == null)
                return;

            InternalAcknowledgeAlarm(
                alarm,
                acknowledgeSources,
                AlarmAcknowledgedOnCcu.Singleton);
        }

        public bool AcknowledgeAlarmFromServer(Guid idAlarm)
        {
            var processedResult = new ProcessedResult<bool>();

            Enqueue(
                () =>
                    InternalAcknowledgeAlarmFromServer(
                        idAlarm,
                        processedResult));

            return processedResult.Result;
        }

        private void InternalAcknowledgeAlarmFromServer(
            Guid idAlarm,
            ProcessedResult<bool> processedResult)
        {
            var alarm = FindAlarm(idAlarm);

            if (alarm == null)
            {
                processedResult.SetResult(false);
                return;
            }

            processedResult.SetResult(
                InternalAcknowledgeAlarm(
                    alarm,
                    ActionFromServerSources.Singleton,
                    AlarmAcknowledgedFromServer.Singleton));
        }

        private bool InternalAcknowledgeAlarm(
            Alarm alarm,
            [NotNull]
            IActionSources acknowledgeSources,
            [NotNull]
            IAlarmAcknowledged alarmAcknowledged)
        {
            if (!alarm.AcknowledgeState())
                return false;

            RunAlarmAcknowledged(
                alarm,
                acknowledgeSources);

            if (!RemoveAlarm(
                alarm,
                alarmAcknowledged))
            {
                Database.ConfigObjectsEngine.AlarmsStorage.UpdateAlarm(alarm);

                alarmAcknowledged.SendEventAlarmAcknowledged(alarm.Id);
            }

            if (!alarm.IsBlocked)
                CreateEventAlarmChanged(alarm as ICreateEventAlarmChanged);

            return true;
        }

        private void RunAlarmAcknowledged(
            Alarm alarm,
            IActionSources acknowledgeSources)
        {
            if (AlarmAcknowledged == null)
                return;
            try
            {
                AlarmAcknowledged(
                    alarm,
                    acknowledgeSources);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private bool RemoveAlarm(
            Alarm alarm,
            [NotNull]
            IRemoveAlarm removeAlarm)
        {
            if (!AlarmCanBeRemoved(alarm))
                return false;

            removeAlarm.RemoveAlarm(alarm);

            return true;
        }

        private bool AlarmCanBeRemoved(Alarm alarm)
        {
            if (alarm.AlarmState != AlarmState.Normal)
                return false;

            if (!alarm.IsAcknowledged)
                return false;

            if (alarm.IsBlocked)
                return false;

            if (_backAlarmReferencesById.ContainsKey(alarm.Id))
                return false;

            return true;
        }

        public void SendEventAlarmRemovedToServerSucceded(Guid idAlarm)
        {
            Enqueue(
                () =>
                    InternalSendEventAlarmRemovedToServerSucceded(idAlarm));
        }

        private void InternalSendEventAlarmRemovedToServerSucceded(Guid idAlarm)
        {
            var alarm = FindAlarm(idAlarm);

            if (alarm == null)
                return;

            if (!AlarmCanBeRemoved(alarm))
            {
                SendAlarmEventToServer(
                    () =>
                        new AlarmAdded(alarm));

                return;
            }

            RemoveAlarmCore(
                alarm,
                SendEventAlarmRemovedToServer.Singleton);
        }

        private void RemoveAlarmCore(
            Alarm alarm,
            [NotNull]
            IRemoveAlarm removeAlarm)
        {
            RemoveAlarmFromMemoryStructures(
                alarm,
                removeAlarm);

            Database.ConfigObjectsEngine.AlarmsStorage.DeleteAlarm(alarm);
        }

        private void RemoveAlarmFromMemoryStructures(
            Alarm alarm,
            [NotNull]
            IRemoveAlarm removeAlarm)
        {
            if (alarm.AlarmKey.ReferencedAlarmId != Guid.Empty)
            {
                _backAlarmReferencesById.Remove(
                    alarm.AlarmKey.ReferencedAlarmId,
                    (Guid key, ICollection<Guid> possibleValueForRemove, out bool continueInRemove) =>
                    {
                        possibleValueForRemove.Remove(alarm.Id);
                        continueInRemove = possibleValueForRemove.Count == 0;
                    }
                    ,
                    null);

                var referencedAlarm = FindAlarm(alarm.AlarmKey.ReferencedAlarmId);

                if (referencedAlarm != null)
                    RemoveAlarm(
                        referencedAlarm,
                        removeAlarm);
            }

            _alarmsByAlarmKey.Remove(alarm.AlarmKey);

            _alarmsById.Remove(alarm.Id);

            _alarmsByAlarmType.Remove(
                alarm.AlarmKey.AlarmType,
                (AlarmType key, ICollection<Alarm> possibleValueForRemove, out bool continueInRemove) =>
                {
                    possibleValueForRemove.Remove(alarm);
                    continueInRemove = possibleValueForRemove.Count == 0;
                },
                null);

            if (alarm.AlarmKey.AlarmObject == null)
                return;

            _alarmsByAlarmObject.Remove(
                alarm.AlarmKey.AlarmObject,
                (IdAndObjectType key, ICollection<Alarm> possibleValueForRemove, out bool continueInRemove) =>
                {
                    possibleValueForRemove.Remove(alarm);
                    continueInRemove = possibleValueForRemove.Count == 0;
                },
                null);
        }

        public void StopAlarmsForAlarmObjects(IEnumerable<IdAndObjectType> alarmObjects)
        {
            Enqueue(
                () =>
                    InternalStopAlarmsForAlarmObjects(alarmObjects));
        }

        private void InternalStopAlarmsForAlarmObjects(IEnumerable<IdAndObjectType> alarmObjects)
        {
            if (alarmObjects == null)
                return;

            foreach (var alarmObject in alarmObjects)
            {
                var alarms = FindAlarms(alarmObject);

                if (alarms != null)
                    foreach (var alarm in alarms)
                    {
                        if (IsAlarmTypeOffline(alarm.AlarmKey.AlarmType))
                            continue;

                        InternalStopAlarm(alarm);
                    }

                BlockedAlarmsManager.Singleton.RemoveNotEnabledAlarms(alarmObject);
            }
        }

        private static bool IsAlarmTypeOffline(AlarmType alarmType)
        {
            if (alarmType == AlarmType.DCU_Offline
                || alarmType == AlarmType.CardReader_Offline)
            {
                return true;
            }

            return false;
        }

        public void RemoveAlarmsForAlarmObjects(IdAndObjectType alarmObject)
        {
            Enqueue(
                () =>
                    InternalRemoveAlarmsForAlarmObjects(alarmObject));
        }

        private void InternalRemoveAlarmsForAlarmObjects(IdAndObjectType alarmObject)
        {
            var alarms = FindAlarms(alarmObject);

            if (alarms != null)
                foreach (var alarm in alarms)
                {
                    var idAlarm = alarm.Id;

                    SendAlarmEventToServer(
                        () =>
                            new AlarmRemoved(idAlarm));
                }

            BlockedAlarmsManager.Singleton.RemoveNotEnabledAlarms(alarmObject);
        }

        public void RemoveAlarm(AlarmKey alarmKey)
        {
            Enqueue(
                () =>
                    InternalRemoveAlarm(alarmKey));
        }

        private void InternalRemoveAlarm(AlarmKey alarmKey)
        {
            var alarm = FindAlarm(alarmKey);

            if (alarm != null)
            {
                alarm.AlarmState = AlarmState.Normal;
                alarm.IsAcknowledged = true;

                var idAlarm = alarm.Id;

                SendAlarmEventToServer(
                    () =>
                        new AlarmRemoved(idAlarm));
            }

            BlockedAlarmsManager.Singleton.RemoveNotEnabledAlarm(alarmKey);
        }

        public ICollection<Alarm> GetAlarms(
            ICollection<AlarmIndividualBlockingChangeInPending> individualBlockingChangesInPending,
            ICollection<AlarmAcknowledgeInPending> alarmsAcknowledgeInPending)
        {
            var processedResult = new ProcessedResult<ICollection<Alarm>>();

            Enqueue(
                () =>
                    InternalGetAlarms(
                        individualBlockingChangesInPending,
                        alarmsAcknowledgeInPending,
                        processedResult));

            return processedResult.Result;
        }

        private void InternalGetAlarms(
            IEnumerable<AlarmIndividualBlockingChangeInPending> individualBlockingChangesInPending,
            IEnumerable<AlarmAcknowledgeInPending> alarmsAcknowledgeInPending,
            ProcessedResult<ICollection<Alarm>> processedResult)
        {
            if (individualBlockingChangesInPending != null)
            {
                foreach (var alarmIndividualBlockingChangeInPending in individualBlockingChangesInPending)
                {
                    var alarm = FindAlarm(alarmIndividualBlockingChangeInPending.IdAlarm);

                    if (alarm == null)
                        continue;

                    if (alarmIndividualBlockingChangeInPending.AlarmBlocked)
                        InternalBlockAlarmIndividual(
                            alarm,
                            alarmIndividualBlockingChangeInPending.UtcDateTime,
                            ActionFromServerSources.Singleton,
                            AlarmBlockedIndividualFromServer.Singleton);
                    else
                        InternalUnblockAlarmIndividual(
                            alarm,
                            alarmIndividualBlockingChangeInPending.UtcDateTime,
                            ActionFromServerSources.Singleton,
                            AlarmUnblockedIndividualFromServerAfterConnected.Singleton);
                }
            }

            if (alarmsAcknowledgeInPending != null)
            {
                foreach (var alarmAcknowledgeInPending in alarmsAcknowledgeInPending)
                {
                    var alarm = FindAlarm(alarmAcknowledgeInPending.IdAlarm);

                    if (alarm == null
                        || alarm.CreatedDateTime != alarmAcknowledgeInPending.CreatedDateTime)
                    {
                        continue;
                    }

                    InternalAcknowledgeAlarm(
                        alarm,
                        ActionFromServerSources.Singleton,
                        AlarmAcknowledgedFromServerAfterConnected.Singleton);
                }
            }

            var result = new LinkedList<Alarm>(
                _alarmsById.GetValuesSnapshot(false)
                    .Where(
                        alarm =>
                            !RemoveAlarm(
                                alarm,
                                DirectRemoveAlarm.Singleton)));

            processedResult.SetResult(result);
        }

        public ICollection<Alarm> GetAlarms(
            AlarmType alarmType,
            IdAndObjectType alarmObject)
        {
            ICollection<Alarm> alarmsForAlarmType;

            if (!_alarmsByAlarmType.TryGetValue(
                alarmType,
                out alarmsForAlarmType))
            {
                return null;
            }

            var result = new LinkedList<Alarm>(
                alarmsForAlarmType.Where(
                    alarm =>
                        alarm.AlarmKey != null
                        && (alarmObject != null
                            ? alarmObject.Equals(alarm.AlarmKey.AlarmObject)
                            : alarm.AlarmKey.AlarmObject == null)));

            return
                result.Count > 0
                    ? result
                    : null;
        }

        public bool ProcessAlarmEventsFromServer(AlarmEvent alarmEvent)
        {
            if (alarmEvent == null)
                return false;

            return alarmEvent.ProcessEvent();
        }

        public void ServerConnected()
        {
            Enqueue(InternalServerConnected);
        }

        private void InternalServerConnected()
        {
            _serverConnected = true;
        }

        public void ServerDisconnected()
        {
            Enqueue(InternalServerDisconnected);
        }

        private void InternalServerDisconnected()
        {
            SendAlarmsToServerDispatcher.Singleton.Clear();

            _serverConnected = false;
        }

        public void ClearAlarms()
        {
            Enqueue(InternalClearAlarms);
        }

        private void InternalClearAlarms()
        {
            SendAlarmsToServerDispatcher.Singleton.Clear();

            _alarmsByAlarmKey.Clear();
            _alarmsById.Clear();
            _alarmsByAlarmObject.Clear();
            _alarmsByAlarmType.Clear();
            _backAlarmReferencesById.Clear();
        }

        public bool ExistsNotAcknowledgedAlarm(AlarmKey alarmKey)
        {
            var processedResult = new ProcessedResult<bool>();

            Enqueue(
                () =>
                    InternalExistsNotAcknowledgedAlarm(
                        alarmKey,
                        processedResult));

            return processedResult.Result;
        }

        private void InternalExistsNotAcknowledgedAlarm(
            AlarmKey alarmKey,
            ProcessedResult<bool> processedResult)
        {
            var alarm = FindAlarm(alarmKey);

            processedResult.SetResult(
                alarm != null
                && !alarm.IsAcknowledged);
        }

        public void BlockAlarmIndividual(
            Guid idAlarm,
            DateTime utcDateTime,
            [NotNull] IActionSources blockAlarmIndividualSources)
        {
            Enqueue(
                () =>
                    InternalBlockAlarmIndividual(
                        idAlarm,
                        utcDateTime,
                        blockAlarmIndividualSources));
        }

        private void InternalBlockAlarmIndividual(
            Guid idAlarm,
            DateTime utcDateTime,
            [NotNull]
            IActionSources blockAlarmIndividualSources)
        {
            var alarm = FindAlarm(idAlarm);

            if (alarm == null)
            {
                return;
            }

            InternalBlockAlarmIndividual(
                alarm,
                utcDateTime,
                blockAlarmIndividualSources,
                AlarmBlockedIndividualOnCcu.Singleton);
        }

        public bool BlockAlarmIndividualFromServer(
            Guid idAlarm,
            DateTime utcDateTime)
        {
            var processedResult = new ProcessedResult<bool>();

            Enqueue(
                () =>
                    InternalBlockAlarmIndividualFromServer(
                        idAlarm,
                        utcDateTime,
                        processedResult));

            return processedResult.Result;
        }

        private void InternalBlockAlarmIndividualFromServer(
            Guid idAlarm,
            DateTime utcDateTime,
            ProcessedResult<bool> processedResult)
        {
            var alarm = FindAlarm(idAlarm);

            if (alarm == null)
            {
                processedResult.SetResult(false);
                return;
            }

            processedResult.SetResult(InternalBlockAlarmIndividual(
                alarm,
                utcDateTime,
                ActionFromServerSources.Singleton,
                AlarmBlockedIndividualFromServer.Singleton));
        }

        private bool InternalBlockAlarmIndividual(
            Alarm alarm,
            DateTime utcDateTime,
            [NotNull]
            IActionSources blockAlarmIndividualSources,
            [NotNull]
            IAlarmBlockedIndividual alarmBlockedIndividual)
        {
            if (!alarm.BlockAlarmIndividual(utcDateTime))
            {
                return false;
            }

            RunAlarmBlockedIndividual(
                alarm,
                blockAlarmIndividualSources);

            Database.ConfigObjectsEngine.AlarmsStorage.UpdateAlarm(alarm);

            alarmBlockedIndividual.SendEventAlarmIndividualBlockingChanged(
                alarm.Id,
                utcDateTime);

            CreateEventAlarmChanged(alarm as ICreateEventAlarmChanged);

            return true;
        }

        private void RunAlarmBlockedIndividual(
            Alarm alarm,
            IActionSources sources)
        {
            if (AlarmBlockedIndividual == null)
                return;
            try
            {
                AlarmBlockedIndividual(
                    alarm,
                    sources);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        internal void BlockAlarmsGeneral(
            AlarmType alarmType,
            Func<IdAndObjectType, bool> filterForAlarmParentObject)
        {
            Enqueue(
                () =>
                    InternalBlockAlarmsGeneral(
                        alarmType,
                        filterForAlarmParentObject));
        }

        private void InternalBlockAlarmsGeneral(
            AlarmType alarmType,
            Func<IdAndObjectType, bool> filterForAlarmParentObject)
        {
            var alarms = FindAlarms(alarmType);

            if (alarms == null)
                return;

            foreach (var alarm in alarms)
            {
                var alarmObject = alarm.AlarmKey.AlarmObject;

                if (filterForAlarmParentObject != null
                    && !filterForAlarmParentObject(alarmObject))
                {
                    continue;
                }

                if (!alarm.BlockAlarmGeneral())
                    continue;

                Database.ConfigObjectsEngine.AlarmsStorage.UpdateAlarm(alarm);

                var alarmId = alarm.Id;

                SendAlarmEventToServer(
                    () =>
                        new AlarmGeneralBlockingChanged(
                            alarmId,
                            true));

                CreateEventAlarmChanged(alarm as ICreateEventAlarmChanged);
            }
        }

        private IEnumerable<Alarm> FindAlarms(AlarmType alarmType)
        {
            ICollection<Alarm> alarms;

            return _alarmsByAlarmType.TryGetValue(
                alarmType,
                out alarms)
                ? new LinkedList<Alarm>(alarms)
                : null;
        }

        public void UnblockAlarmIndividual(
            Guid idAlarm,
            DateTime utcDateTime,
            [NotNull]
            IActionSources blockAlarmIndividualSources)
        {
            Enqueue(
                () =>
                    InternalUnblockAlarmIndividual(
                        idAlarm,
                        utcDateTime,
                        blockAlarmIndividualSources));
        }

        private void InternalUnblockAlarmIndividual(
            Guid idAlarm,
            DateTime utcDateTime,
            [NotNull]
            IActionSources blockAlarmIndividualSources)
        {
            var alarm = FindAlarm(idAlarm);

            if (alarm == null)
            {
                return;
            }

            InternalUnblockAlarmIndividual(
                alarm,
                utcDateTime,
                blockAlarmIndividualSources,
                AlarmUnblockedIndividualOnCcu.Singleton);
        }

        public bool UnblockAlarmIndividualFromServer(
            Guid idAlarm,
            DateTime utcDateTime)
        {
            var processedResult = new ProcessedResult<bool>();

            Enqueue(
                () =>
                    InternalUnblockAlarmIndividualFromServer(
                        idAlarm,
                        utcDateTime,
                        processedResult));

            return processedResult.Result;
        }

        private void InternalUnblockAlarmIndividualFromServer(
            Guid idAlarm,
            DateTime utcDateTime,
            ProcessedResult<bool> processedResult)
        {
            var alarm = FindAlarm(idAlarm);

            if (alarm == null)
            {
                processedResult.SetResult(false);
                return;
            }

            processedResult.SetResult(InternalUnblockAlarmIndividual(
                alarm,
                utcDateTime,
                ActionFromServerSources.Singleton,
                AlarmUnblockedIndividualFromServer.Singleton));
        }

        private bool InternalUnblockAlarmIndividual(
            Alarm alarm,
            DateTime utcDateTime,
            [NotNull]
            IActionSources blockAlarmIndividualSources,
            [NotNull]
            IAlarmUnblockedIndividual alarmUnblockedIndividual)
        {
            if (!alarm.UnblockAlarmIndividual(utcDateTime))
            {
                return false;
            }

            RunAlarmUnblockedIndividual(
                alarm,
                blockAlarmIndividualSources);

            if (!RemoveAlarm(
                alarm,
                alarmUnblockedIndividual))
            {
                Database.ConfigObjectsEngine.AlarmsStorage.UpdateAlarm(alarm);

                alarmUnblockedIndividual.SendEventAlarmIndividualBlockingChanged(
                    alarm.Id,
                    utcDateTime);
            }

            CreateEventAlarmChanged(alarm as ICreateEventAlarmChanged);

            return true;
        }

        private void RunAlarmUnblockedIndividual(
            Alarm alarm,
            IActionSources sources)
        {
            if (AlarmUnblockedIndividual == null)
                return;

            try
            {
                AlarmUnblockedIndividual(
                    alarm,
                    sources);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        internal void UnblockAlarmsGeneral(
            AlarmType alarmType,
            Func<IdAndObjectType, bool> filterForAlarmParentObject,
            Action<Alarm> postPrecessingLambda)
        {
            Enqueue(
                () =>
                    InternalUnblockAlarmsGeneral(
                        alarmType,
                        filterForAlarmParentObject,
                        postPrecessingLambda));
        }

        private void InternalUnblockAlarmsGeneral(
            AlarmType alarmType,
            Func<IdAndObjectType, bool> filterForAlarmParentObject,
            Action<Alarm> postPrecessingLambda)
        {
            var alarms = FindAlarms(alarmType);

            if (alarms == null)
                return;

            foreach (var alarm in alarms)
            {
                var alarmObject = alarm.AlarmKey.AlarmObject;

                if (filterForAlarmParentObject != null
                    && !filterForAlarmParentObject(alarmObject))
                {
                    continue;
                }


                if (!alarm.UnblockAlarmGeneral())
                    continue;

                if (!RemoveAlarm(
                    alarm,
                    SendEventAlarmRemovedToServer.Singleton))
                {
                    Database.ConfigObjectsEngine.AlarmsStorage.UpdateAlarm(alarm);

                    var alarmId = alarm.Id;

                    SendAlarmEventToServer(
                        () =>
                            new AlarmGeneralBlockingChanged(
                                alarmId,
                                false));
                }

                if (postPrecessingLambda != null)
                    postPrecessingLambda(alarm);

                CreateEventAlarmChanged(alarm as ICreateEventAlarmChanged);
            }
        }

        public void PrepareObjectDelete(
            Guid idObject,
            ObjectType objectType)
        {
            RemoveAlarmsForAlarmObjects(
                new IdAndObjectType(
                    idObject,
                    objectType));
        }
    }
}
