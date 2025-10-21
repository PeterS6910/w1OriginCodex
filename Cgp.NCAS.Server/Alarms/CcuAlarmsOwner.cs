using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(710)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmEvent
    {
        protected AlarmEvent()
        {

        }

        public virtual void ProcessEvent(Guid idCcu)
        {

        }
    }

    [LwSerialize(711)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmEventWithId : AlarmEvent
    {
        public Guid IdAlarm { get; private set; }

        protected AlarmEventWithId()
        {

        }

        public AlarmEventWithId(
            Guid idAlarm)
        {
            IdAlarm = idAlarm;
        }
    }

    [LwSerialize(712)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAdded : AlarmEvent
    {
        public Alarm Alarm { get; private set; }

        protected AlarmAdded()
        {

        }

        public AlarmAdded(Alarm alarm)
        {
            Alarm = alarm;
        }

        public override void ProcessEvent(Guid idCcu)
        {
            var iCreateServerAlarm = Alarm as ICreateServerAlarm;

            if (iCreateServerAlarm == null)
                return;

            var serverAlarm = iCreateServerAlarm.CreateServerAlarm(idCcu);

            if (serverAlarm == null)
                return;

            NCASServer.Singleton.GetAlarmsQueue().AddAlarm(
                serverAlarm);
        }
    }

    [LwSerialize(713)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmStopped : AlarmEventWithId
    {
        public DateTime DateTime { get; private set; }

        protected AlarmStopped()
        {

        }

        public AlarmStopped(Guid idAlarm)
            : base(idAlarm)
        {
        }

        public override void ProcessEvent(Guid idCcu)
        {
            NCASServer.Singleton.GetAlarmsQueue().TryRunOnAlarmsOwner(
                idCcu,
                externalAlarmsOwner =>
                    externalAlarmsOwner.StopAlarmFromOwner(
                        IdAlarm));
        }
    }

    [LwSerialize(714)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAcknowledged : AlarmEventWithId
    {
        protected AlarmAcknowledged()
        {

        }

        public AlarmAcknowledged(
            Guid idAlarm)
            : base(idAlarm)
        {

        }

        public override void ProcessEvent(Guid idCcu)
        {
            NCASServer.Singleton.GetAlarmsQueue().TryRunOnAlarmsOwner(
                idCcu,
                externalAlarmsOwner =>
                    externalAlarmsOwner.AcknowledgeAlarmFromOwner(
                        IdAlarm));
        }
    }

    [LwSerialize(715)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmRemoved : AlarmEventWithId
    {
        protected AlarmRemoved()
        {

        }

        public AlarmRemoved(
            Guid idAlarm)
            : base(idAlarm)
        {

        }

        public override void ProcessEvent(Guid idCcu)
        {
            NCASServer.Singleton.GetAlarmsQueue().TryRunOnAlarmsOwner(
                idCcu,
                externalAlarmsOwner =>
                    externalAlarmsOwner.RemoveAlarmFromOwner(
                        IdAlarm));
        }
    }

    [LwSerialize(716)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmGeneralBlockingChanged : AlarmEventWithId
    {
        public bool AlarmBlocked { get; private set; }

        protected AlarmGeneralBlockingChanged()
        {

        }

        public AlarmGeneralBlockingChanged(
            Guid idAlarm,
            bool alarmBlocked)
            : base(idAlarm)
        {
            AlarmBlocked = alarmBlocked;
        }

        public override void ProcessEvent(Guid idCcu)
        {
            if (AlarmBlocked)
            {
                NCASServer.Singleton.GetAlarmsQueue()
                    .TryRunOnAlarmsOwner(
                        idCcu,
                        externalAlarmsOwner =>
                            externalAlarmsOwner.BlockAlarmGeneralFromOwner(
                                IdAlarm));

                return;
            }

            NCASServer.Singleton.GetAlarmsQueue()
                .TryRunOnAlarmsOwner(
                    idCcu,
                    externalAlarmsOwner =>
                        externalAlarmsOwner.UnblockAlarmGeneralFromOwner(
                            IdAlarm));
        }
    }

    [LwSerialize(717)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmIndividualBlockingChanged : AlarmEventWithId
    {
        public bool AlarmBlocked { get; private set; }
        public DateTime UtcDateTime { get; private set; }

        protected AlarmIndividualBlockingChanged()
        {

        }

        public AlarmIndividualBlockingChanged(
            Guid idAlarm,
            bool alarmBlocked,
            DateTime utcDateTime)
            : base(idAlarm)
        {
            AlarmBlocked = alarmBlocked;
            UtcDateTime = utcDateTime;
        }

        public override void ProcessEvent(Guid idCcu)
        {
            if (AlarmBlocked)
            {
                NCASServer.Singleton.GetAlarmsQueue()
                    .TryRunOnAlarmsOwner(
                        idCcu,
                        externalAlarmsOwner =>
                            externalAlarmsOwner.BlockAlarmIndividualFromOwner(
                                IdAlarm,
                                UtcDateTime));

                return;
            }

            NCASServer.Singleton.GetAlarmsQueue()
                .TryRunOnAlarmsOwner(
                    idCcu,
                    externalAlarmsOwner =>
                        externalAlarmsOwner.UnblockAlarmIndividualFromOwner(
                            IdAlarm,
                            UtcDateTime));
        }
    }

    [LwSerialize(718)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAcknowledgeInPending
    {
        public Guid IdAlarm { get; private set; }
        public DateTime CreatedDateTime { get; private set; }

        protected AlarmAcknowledgeInPending()
        {

        }

        public AlarmAcknowledgeInPending(
            Guid idAlarm,
            DateTime createdDateTime)
        {
            IdAlarm = idAlarm;
            CreatedDateTime = createdDateTime;
        }
    }

    [LwSerialize(719)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmIndividualBlockingChangeInPending
    {
        public Guid IdAlarm { get; private set; }
        public bool AlarmBlocked { get; private set; }
        public DateTime UtcDateTime { get; private set; }

        protected AlarmIndividualBlockingChangeInPending()
        {

        }

        public AlarmIndividualBlockingChangeInPending(
            Guid idAlarm,
            bool alarmBlocked,
            DateTime utcDateTime)
        {
            IdAlarm = idAlarm;
            AlarmBlocked = alarmBlocked;
            UtcDateTime = utcDateTime;
        }
    }

    public sealed class CcuAlarmsOwner : IExternalAlarmsOwner
    {
        private readonly Guid _idOwner;

        private readonly object _lockOperation = new object();

        private readonly SyncDictionary<Guid, ServerAlarm> _serverAlarmsByIdAlarm
            = new SyncDictionary<Guid, ServerAlarm>();

        private readonly SyncDictionary<AlarmKey, ServerAlarm> _serverAlarmsByAlarmKey
            = new SyncDictionary<AlarmKey, ServerAlarm>();

        private readonly SyncDictionary<AlarmType, ICollection<ServerAlarm>> _serverAlarmsByAlarmType
            = new SyncDictionary<AlarmType, ICollection<ServerAlarm>>();

        private readonly SyncDictionary<IdAndObjectType, ICollection<ServerAlarm>> _serverAlarmsByAlarmObject
            = new SyncDictionary<IdAndObjectType, ICollection<ServerAlarm>>();

        private bool _ownerIsOffline = true;

        public CcuAlarmsOwner(Guid idOwner)
        {
            _idOwner = idOwner;
        }

        public void LoadServerAlarmsFromDatabase()
        {
            lock (_lockOperation)
            {
                var serverAlarms = DatabaseServerAlarms.Singleton.GetServerAlarmsForOwner(_idOwner);

                foreach (var serverAlarm in serverAlarms)
                {
                    serverAlarm.ServerAlarmCore.OwnerIsOffline = _ownerIsOffline;

                    AddAlarmCore(
                        serverAlarm,
                        true);
                }
            }

            AlarmsManager.Singleton.RunDelegateChangeAlarm();
        }

        private void AddAlarmToMemoryStructures(ServerAlarm serverAlarm)
        {
            _serverAlarmsByAlarmKey.Add(
                serverAlarm.ServerAlarmCore.Alarm.AlarmKey,
                serverAlarm);

            _serverAlarmsByIdAlarm.Add(
                serverAlarm.ServerAlarmCore.Alarm.Id,
                serverAlarm);

            _serverAlarmsByAlarmType.GetOrAddValue(
                serverAlarm.ServerAlarmCore.Alarm.AlarmKey.AlarmType,
                key =>
                    new HashSet<ServerAlarm>(),
                (key, value, newlyAdded) =>
                    value.Add(serverAlarm));

            var alarmObject = serverAlarm.ServerAlarmCore.Alarm.AlarmKey.AlarmObject;

            if (alarmObject != null)
            {
                _serverAlarmsByAlarmObject.GetOrAddValue(
                    alarmObject,
                    key =>
                        new HashSet<ServerAlarm>(),
                    (key, value, newlyAdded) =>
                        value.Add(serverAlarm));
            }
        }

        private void RemoveAlarmFromMemoryStructures(ServerAlarm serverAlarm)
        {
            _serverAlarmsByAlarmKey.Remove(
                serverAlarm.ServerAlarmCore.Alarm.AlarmKey);

            _serverAlarmsByIdAlarm.Remove(
                serverAlarm.ServerAlarmCore.Alarm.Id);

            _serverAlarmsByAlarmType.Remove(
                serverAlarm.ServerAlarmCore.Alarm.AlarmKey.AlarmType,
                (AlarmType key, ICollection<ServerAlarm> possibleValueForRemove, out bool continueInRemove) =>
                {
                    possibleValueForRemove.Remove(serverAlarm);
                    continueInRemove = possibleValueForRemove.Count == 0;
                },
                null);

            var alarmObject = serverAlarm.ServerAlarmCore.Alarm.AlarmKey.AlarmObject;

            if (alarmObject != null)
            {
                _serverAlarmsByAlarmObject.Remove(
                    alarmObject,
                    (IdAndObjectType key, ICollection<ServerAlarm> possibleValueForRemove, out bool continueInRemove) =>
                    {
                        possibleValueForRemove.Remove(serverAlarm);
                        continueInRemove = possibleValueForRemove.Count == 0;
                    },
                    null);
            }
        }

        /// <summary>
        /// Adds alarm
        /// </summary>
        /// <param name="serverAlarm"></param>

        public void AddAlarm(ServerAlarm serverAlarm)
        {
            lock (_lockOperation)
            {
                AddAlarmCore(
                    serverAlarm,
                    false);

                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        private void AddAlarmCore(
            ServerAlarm serverAlarm,
            bool loadingAlarms)
        {
            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            var oldServerAlarm = FindAlarmByAlarmKey(alarm.AlarmKey);

            if (oldServerAlarm == null)
            {
                serverAlarm.ServerAlarmCore.AlarmPriority =
                    AlarmsManager.Singleton.GetAlarmPriority(
                        serverAlarm.ServerAlarmCore.Alarm.AlarmKey.AlarmType);

                AddAlarmToMemoryStructures(serverAlarm);

                if (!loadingAlarms)
                {
                    DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                    AlarmsManager.Singleton.AfterAlarmAdded(
                        serverAlarm,
                        true,
                        null);
                }

                return;
            }

            if (alarm.AlarmState == oldServerAlarm.ServerAlarmCore.Alarm.AlarmState
                && alarm.AlarmKey.SameParameters(oldServerAlarm.ServerAlarmCore.Alarm.AlarmKey)
                && alarm.CreatedDateTime == oldServerAlarm.ServerAlarmCore.Alarm.CreatedDateTime)
            {
                return;
            }

            UpdateAlarm(
                oldServerAlarm,
                alarm.AlarmState,
                alarm.CreatedDateTime,
                alarm.IsAcknowledged,
                alarm.AlarmKey.ExtendedObjects,
                alarm.AlarmKey.Parameters);

            if (!loadingAlarms)
            {
                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                AlarmsManager.Singleton.AfterAlarmAdded(
                    oldServerAlarm,
                    false,
                    null);
            }
        }

        /// <summary>
        /// Finds alarm withc specific alarm key
        /// </summary>
        /// <param name="alarmKey"></param>
        /// <returns></returns>
        private ServerAlarm FindAlarmByAlarmKey(AlarmKey alarmKey)
        {
            ServerAlarm serverAlarm;
            if (_serverAlarmsByAlarmKey.TryGetValue(alarmKey, out serverAlarm))
            {
                return serverAlarm;
            }

            return null;
        }

        private void UpdateAlarm(
            ServerAlarm serverAlarm,
            AlarmState alarmState,
            DateTime createdDateTime,
            bool isAcknowledged,
            IEnumerable<IdAndObjectType> extendedObjects,
            IEnumerable<AlarmParameter> parameters)
        {
            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            alarm.AlarmState = alarmState;

            if (alarm.CreatedDateTime != createdDateTime)
            {
                serverAlarm.ServerAlarmCore.AcknowledgeInPending = false;
            }

            alarm.CreatedDateTime = createdDateTime;
            alarm.IsAcknowledged = isAcknowledged;
            alarm.AlarmKey.SetExtendedObjects(extendedObjects);
            alarm.AlarmKey.SetParameters(parameters);
        }

        /// <summary>
        /// Finds alarm withc specific Guid
        /// </summary>
        /// <param name="idAlarm"></param>
        /// <returns></returns>
        public ServerAlarm FindAlarmByIdAlarm(Guid idAlarm)
        {
            lock (_lockOperation)
            {
                if (_serverAlarmsByIdAlarm.Count > 0)
                {
                    ServerAlarm severAlarm;
                    if (_serverAlarmsByIdAlarm.TryGetValue(idAlarm, out severAlarm))
                    {
                        return severAlarm;
                    }
                }

                return null;
            }
        }

        public void StopAlarmFromOwner(
            Guid idAlarm)
        {
            lock (_lockOperation)
            {
                StopAlarm(FindAlarmByIdAlarm(idAlarm));
            }
        }

        private void StopAlarm(
            ServerAlarm serverAlarm)
        {
            if (serverAlarm == null)
                return;

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm.AlarmState == AlarmState.Normal)
                return;

            alarm.AlarmState = AlarmState.Normal;

            DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

            AlarmsManager.Singleton.AfterChangeAlarm(
                serverAlarm,
                null);

            AlarmsManager.Singleton.RunDelegateChangeAlarm();
        }

        public ServerAlarm AcknowledgeAlarm(Guid idAlarm)
        {
            return AcknowledgeAlarm(
                idAlarm,
                true);
        }

        public ServerAlarm AcknowledgeAlarm(
            Guid idAlarm,
            bool notifyClient)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                    return null;

                if (serverAlarm.ServerAlarmCore.Alarm.IsAcknowledged)
                    return serverAlarm;

                serverAlarm.ServerAlarmCore.AcknowledgeInPending = true;

                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                SafeThread<ServerAlarm>.StartThread(
                    AcknoledgeAlarmOnCcu,
                    serverAlarm);

                if (notifyClient)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();

                return serverAlarm;
            }
        }

        private void AcknoledgeAlarmOnCcu(ServerAlarm serverAlarm)
        {
            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            var wasAcknowledged = CCUConfigurationHandler.Singleton.ProcessAlarmEvent(
                _idOwner,
                new AlarmAcknowledged(alarm.Id));

            if (wasAcknowledged == null)
                return;

            lock (_lockOperation)
            {
                serverAlarm.ServerAlarmCore.AcknowledgeInPending = false;

                if (wasAcknowledged.Value)
                {
                    alarm.AcknowledgeState();

                    AlarmsManager.Singleton.AfterChangeAlarm(
                        serverAlarm,
                        null);
                }

                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        public void AcknowledgeAlarmFromOwner(
            Guid idAlarm)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                {
                    return;
                }

                if (serverAlarm.ServerAlarmCore.Alarm.AcknowledgeState())
                {
                    DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                    AlarmsManager.Singleton.AfterChangeAlarm(
                        serverAlarm,
                        null);

                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
                }
            }
        }

        public ServerAlarm BlockAlarmIndividual(Guid idAlarm)
        {
            return BlockAlarmIndividual(
                idAlarm,
                true);
        }

        public ServerAlarm BlockAlarmIndividual(
            Guid idAlarm,
            bool notifyClient)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                    return null;

                if (serverAlarm.ServerAlarmCore.Alarm.IsBlockedIndividual
                    && serverAlarm.ServerAlarmCore.IndividualUnblockinInPending == null)
                {
                    return serverAlarm;
                }

                serverAlarm.ServerAlarmCore.IndividualBlockinInPending = DateTime.UtcNow;
                serverAlarm.ServerAlarmCore.IndividualUnblockinInPending = null;

                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                SafeThread<ServerAlarm>.StartThread(
                    BlockAlarmIndividualOnCcu,
                    serverAlarm);

                if (notifyClient)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();

                return serverAlarm;
            }
        }

        private void BlockAlarmIndividualOnCcu(ServerAlarm serverAlarm)
        {
            if (serverAlarm.ServerAlarmCore.IndividualBlockinInPending == null)
                return;

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            var wasBlockedIndividual = CCUConfigurationHandler.Singleton.ProcessAlarmEvent(
                _idOwner,
                new AlarmIndividualBlockingChanged(
                    alarm.Id,
                    true,
                    serverAlarm.ServerAlarmCore.IndividualBlockinInPending.Value));

            if (wasBlockedIndividual == null)
                return;

            lock (_lockOperation)
            {
                if (wasBlockedIndividual.Value)
                    alarm.BlockAlarmIndividual(serverAlarm.ServerAlarmCore.IndividualBlockinInPending.Value);

                serverAlarm.ServerAlarmCore.IndividualBlockinInPending = null;

                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                AlarmsManager.Singleton.AfterChangeAlarm(
                    serverAlarm,
                    null);

                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        public void BlockAlarmIndividualFromOwner(
            Guid idAlarm,
            DateTime utcDateTime)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                    return;

                if (serverAlarm.ServerAlarmCore.Alarm.BlockAlarmIndividual(utcDateTime))
                {
                    DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                    AlarmsManager.Singleton.AfterChangeAlarm(
                        serverAlarm,
                        null);

                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
                }
            }
        }

        public void BlockAlarmGeneralFromOwner(Guid idAlarm)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                    return;

                if (serverAlarm.ServerAlarmCore.Alarm.BlockAlarmGeneral())
                {
                    DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                    AlarmsManager.Singleton.AfterChangeAlarm(
                        serverAlarm,
                        null);

                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
                }
            }
        }

        public ServerAlarm UnblockAlarmIndividual(Guid idAlarm)
        {
            return UnblockAlarmIndividual(
                idAlarm,
                true);
        }

        public ServerAlarm UnblockAlarmIndividual(
            Guid idAlarm,
            bool notifyClient)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                    return null;

                if (!serverAlarm.ServerAlarmCore.Alarm.IsBlockedIndividual
                    && serverAlarm.ServerAlarmCore.IndividualBlockinInPending == null)
                {
                    return serverAlarm;
                }

                serverAlarm.ServerAlarmCore.IndividualUnblockinInPending = DateTime.UtcNow;
                serverAlarm.ServerAlarmCore.IndividualBlockinInPending = null;

                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                SafeThread<ServerAlarm>.StartThread(
                    UnblockAlarmIndividualOnCcu,
                    serverAlarm);

                if (notifyClient)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();

                return serverAlarm;
            }
        }

        private void UnblockAlarmIndividualOnCcu(ServerAlarm serverAlarm)
        {
            if (serverAlarm.ServerAlarmCore.IndividualUnblockinInPending == null)
                return;

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            var wasUnblockedIndividual = CCUConfigurationHandler.Singleton.ProcessAlarmEvent(
                _idOwner,
                new AlarmIndividualBlockingChanged(
                    alarm.Id,
                    false,
                    serverAlarm.ServerAlarmCore.IndividualUnblockinInPending.Value));

            if (wasUnblockedIndividual == null)
                return;

            lock (_lockOperation)
            {
                if (wasUnblockedIndividual.Value)
                    alarm.UnblockAlarmIndividual(serverAlarm.ServerAlarmCore.IndividualUnblockinInPending.Value);

                serverAlarm.ServerAlarmCore.IndividualUnblockinInPending = null;

                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);

                AlarmsManager.Singleton.AfterChangeAlarm(
                    serverAlarm,
                    null);

                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        public void UnblockAlarmIndividualFromOwner(
            Guid idAlarm,
            DateTime utcDateTime)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                    return;

                if (serverAlarm.ServerAlarmCore.Alarm.UnblockAlarmIndividual(utcDateTime))
                {
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();

                    AlarmsManager.Singleton.AfterChangeAlarm(
                        serverAlarm,
                        null);

                    DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);
                }
            }
        }

        public void UnblockAlarmGeneralFromOwner(Guid idAlarm)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                    return;

                if (serverAlarm.ServerAlarmCore.Alarm.UnblockAlarmGeneral())
                {
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();

                    AlarmsManager.Singleton.AfterChangeAlarm(
                        serverAlarm,
                        null);

                    DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);
                }
            }
        }

        public bool IsAlarmBlocked(Guid idAlarm)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                {
                    return false;
                }

                return serverAlarm.ServerAlarmCore.Alarm.IsBlocked;
            }
        }

        /// <summary>
        /// Removes alarm
        /// </summary>
        /// <param name="idAlarm"></param>
        public void RemoveAlarmFromOwner(Guid idAlarm)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                if (serverAlarm == null)
                    return;

                RemoveAlarmCore(
                    serverAlarm,
                    false);

                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        private void RemoveAlarmCore(
            ServerAlarm serverAlarm,
            bool removeBlockedAlarm)
        {
            if (!removeBlockedAlarm
                && serverAlarm.ServerAlarmCore.Alarm.IsBlocked)
            {
                serverAlarm.ServerAlarmCore.Alarm.AlarmState = AlarmState.Normal;

                AlarmsManager.Singleton.AfterChangeAlarm(
                    serverAlarm,
                    null);

                return;
            }

            RemoveAlarmFromMemoryStructures(serverAlarm);

            DatabaseServerAlarms.Singleton.DeleteServerAlarm(
                serverAlarm.ServerAlarmCore.IdServerAlarm);

            AlarmsManager.Singleton.AfterAlarmRemoved(serverAlarm);
        }

        public void RemoveAlarmsForAlarmObjects(
            IEnumerable<IdAndObjectType> alarmObjects)
        {
            lock (_lockOperation)
            {
                bool alarmRemoved = false;

                foreach (var alarmObject in alarmObjects)
                {
                    var serverAlarms = FindAlarmsByAlarmObject(alarmObject);

                    if (serverAlarms == null)
                        continue;

                    alarmRemoved = true;

                    foreach (var serverAlarm in serverAlarms)
                        RemoveAlarmCore(
                            serverAlarm,
                            true);
                }

                if (alarmRemoved)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        private IEnumerable<ServerAlarm> FindAlarmsByAlarmObject(IdAndObjectType alarmObject)
        {
            ICollection<ServerAlarm> serverAlarms;

            if (!_serverAlarmsByAlarmObject.TryGetValue(
                alarmObject,
                out serverAlarms))
            {
                return null;
            }

            return new LinkedList<ServerAlarm>(serverAlarms);
        }

        public void RemoveAllAlarms()
        {
            lock (_lockOperation)
            {
                RemoveAllAlarmsCore();
                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        private void RemoveAllAlarmsCore()
        {
            var serverAlarms = _serverAlarmsByAlarmKey.GetValuesSnapshot(true);

            _serverAlarmsByAlarmKey.Clear();
            _serverAlarmsByIdAlarm.Clear();
            _serverAlarmsByAlarmType.Clear();
            _serverAlarmsByAlarmObject.Clear();

            DatabaseServerAlarms.Singleton.DeleteServerAlarmsForOwner(_idOwner);

            if (serverAlarms == null)
                return;

            foreach (var serverAlarm in serverAlarms)
            {
                AlarmsManager.Singleton.AfterAlarmRemoved(serverAlarm);
            }
        }

        /// <summary>
        /// Finds alarms with specific alarm type
        /// </summary>
        /// <param name="alarmType"></param>
        /// <returns></returns>
        public ICollection<ServerAlarm> FindAlarmsByAlarmType(AlarmType alarmType)
        {
            lock (_lockOperation)
            {
                ICollection<ServerAlarm> serverAlarms;

                if (!_serverAlarmsByAlarmType.TryGetValue(
                    alarmType,
                    out serverAlarms))
                {
                    return null;
                }

                return new LinkedList<ServerAlarm>(serverAlarms);
            }
        }

        /// <summary>
        /// Returns all alarms
        /// </summary>
        /// <returns></returns>
        public ICollection<ServerAlarm> GetAlarms()
        {
            lock (_lockOperation)
                return _serverAlarmsByIdAlarm.GetValuesSnapshot(true);
        }

        public IEnumerable<ServerAlarm> GetAlarms(
            AlarmType alarmType,
            IdAndObjectType alarmObject)
        {
            lock (_lockOperation)
                return _serverAlarmsByIdAlarm.Values.Where(
                    serverAlarm =>
                    {
                        var alarmKey = serverAlarm.ServerAlarmCore.Alarm.AlarmKey;

                        if (alarmKey.AlarmType != alarmType)
                            return false;

                        if (alarmKey.AlarmObject == null)
                            return alarmObject == null;

                        return alarmKey.AlarmObject.Equals(alarmObject);
                    });
        }

        public void AlarmOwnerConnected()
        {
            lock (_lockOperation)
            {
                var serverAlarms = _serverAlarmsByIdAlarm.GetValuesSnapshot(false);

                var individualBlockingChangesInPending = new LinkedList<AlarmIndividualBlockingChangeInPending>();
                var alarmsAcknowledgeInPending = new LinkedList<AlarmAcknowledgeInPending>();

                foreach (var serverAlarm in serverAlarms)
                {
                    var alarm = serverAlarm.ServerAlarmCore.Alarm;

                    if (serverAlarm.ServerAlarmCore.IndividualBlockinInPending.HasValue)
                        individualBlockingChangesInPending.AddLast(
                            new AlarmIndividualBlockingChangeInPending(
                                alarm.Id,
                                true,
                                serverAlarm.ServerAlarmCore.IndividualBlockinInPending.Value));
                    else if (serverAlarm.ServerAlarmCore.IndividualUnblockinInPending.HasValue)
                        individualBlockingChangesInPending.AddLast(
                            new AlarmIndividualBlockingChangeInPending(
                                alarm.Id,
                                false,
                                serverAlarm.ServerAlarmCore.IndividualUnblockinInPending.Value));

                    if (serverAlarm.ServerAlarmCore.AcknowledgeInPending)
                        alarmsAcknowledgeInPending.AddLast(
                            new AlarmAcknowledgeInPending(
                                alarm.Id,
                                alarm.CreatedDateTime));
                }

                var ownerAlarms = CCUConfigurationHandler.Singleton.GetAlarms(
                    _idOwner,
                    individualBlockingChangesInPending.Count > 0
                        ? individualBlockingChangesInPending
                        : null,
                    alarmsAcknowledgeInPending.Count > 0
                        ? alarmsAcknowledgeInPending
                        : null);

                if (ownerAlarms == null)
                    return;

                _ownerIsOffline = false;

                var ownerAlarmsIds = new HashSet<Guid>(
                    ownerAlarms.Select(
                        alarm =>
                            alarm.Id));

                foreach (var serverAlarm in serverAlarms)
                {
                    if (!ownerAlarmsIds.Contains(serverAlarm.ServerAlarmCore.Alarm.Id))
                        RemoveAlarmCore(
                            serverAlarm,
                            true);
                    else
                    {
                        serverAlarm.ServerAlarmCore.OwnerIsOffline = false;
                        DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(serverAlarm);
                    }

                }

                foreach (var ownerAlarm in ownerAlarms)
                {
                    var iCreateServerAlarm = ownerAlarm as ICreateServerAlarm;

                    if (iCreateServerAlarm == null)
                        continue;

                    var serverAlarmToAdd = iCreateServerAlarm.CreateServerAlarm(_idOwner);

                    if (serverAlarmToAdd == null)
                        continue;

                    AddAlarmCore(
                        serverAlarmToAdd,
                        false);
                }

                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        public void AlarmOwnerDisconnected()
        {
            lock (_lockOperation)
            {
                _ownerIsOffline = true;

                _serverAlarmsByIdAlarm.ForEach(
                    (key, value) =>
                    {
                        value.ServerAlarmCore.OwnerIsOffline = true;
                        DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(value);
                    });

                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }
    }
}
