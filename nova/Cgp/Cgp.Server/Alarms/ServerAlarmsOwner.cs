using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Server.Alarms
{
    public class ServerAlarmsOwner : IServerAlarmsOwner
    {
        private readonly object _lockOperation = new object();

        private readonly SyncDictionary<Guid, ServerAlarm> _serverAlarmsByIdAlarm
            = new SyncDictionary<Guid, ServerAlarm>();

        private readonly SyncDictionary<AlarmKey, ServerAlarm> _serverAlarmsByAlarmKey
            = new SyncDictionary<AlarmKey, ServerAlarm>();

        private readonly SyncDictionary<AlarmType, ICollection<ServerAlarm>> _serverAlarmsByAlarmType
            = new SyncDictionary<AlarmType, ICollection<ServerAlarm>>();

        private readonly SyncDictionary<IdAndObjectType, ICollection<ServerAlarm>> _serverAlarmsByAlarmObject
            = new SyncDictionary<IdAndObjectType, ICollection<ServerAlarm>>();

        public event Action<ServerAlarm> AlarmAdded;

        public event Action<ServerAlarm> AlarmStopped;

        private readonly SyncDictionary<Guid, ICollection<Guid>> _backAlarmReferencesById =
            new SyncDictionary<Guid, ICollection<Guid>>();

        public void LoadServerAlarmsFromDatabase()
        {
            lock (_lockOperation)
            {
                var serverAlarms = DatabaseServerAlarms.Singleton.GetServerAlarmsForOwner(Guid.Empty);

                foreach (var serverAlarm in serverAlarms)
                {
                    AddAlarmCore(
                        serverAlarm,
                        null,
                        true);
                }
            }

            AlarmsManager.Singleton.RunDelegateChangeAlarm();
        }

        private void AddAlarmToMemoryStructures(ServerAlarm serverAlarm)
        {
            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm.AlarmKey.ReferencedAlarmId != Guid.Empty)
            {
                #if DEBUG
                var referencedServerAlarm = FindAlarmByIdAlarm(
                   alarm.AlarmKey.ReferencedAlarmId);

                if (referencedServerAlarm != null
                    && referencedServerAlarm.ServerAlarmCore.Alarm.AlarmKey.ReferencedAlarmId != Guid.Empty)
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

            _serverAlarmsByAlarmKey.Add(
                alarm.AlarmKey,
                serverAlarm);

            _serverAlarmsByIdAlarm.Add(
                alarm.Id,
                serverAlarm);

            _serverAlarmsByAlarmType.GetOrAddValue(
                alarm.AlarmKey.AlarmType,
                key =>
                    new HashSet<ServerAlarm>(),
                (key, value, newlyAdded) =>
                    value.Add(serverAlarm));

            var alarmObject = alarm.AlarmKey.AlarmObject;

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
            var alarm = serverAlarm.ServerAlarmCore.Alarm;

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

                var referencedAlarm = FindAlarmByIdAlarm(alarm.AlarmKey.ReferencedAlarmId);

                if (referencedAlarm != null)
                    RemoveAlarm(referencedAlarm);
            }

            _serverAlarmsByAlarmKey.Remove(
                alarm.AlarmKey);

            _serverAlarmsByIdAlarm.Remove(
                alarm.Id);

            _serverAlarmsByAlarmType.Remove(
                alarm.AlarmKey.AlarmType,
                (AlarmType key, ICollection<ServerAlarm> possibleValueForRemove, out bool continueInRemove) =>
                {
                    possibleValueForRemove.Remove(serverAlarm);
                    continueInRemove = possibleValueForRemove.Count == 0;
                },
                null);

            var alarmObject = alarm.AlarmKey.AlarmObject;

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
            AddAlarm(
                serverAlarm,
                null);
        }

        public void AddAlarm(
            ServerAlarm serverAlarm,
            ServerAlarm referencedAlarm)
        {
            lock (_lockOperation)
            {
                AddAlarmCore(
                    serverAlarm,
                    referencedAlarm,
                    false);
            }

            AlarmsManager.Singleton.RunDelegateChangeAlarm();
        }

        private void AddAlarmCore(
            ServerAlarm serverAlarm,
            ServerAlarm referencedAlarm,
            bool loadingAlarms)
        {
            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (referencedAlarm != null)
            {
                alarm.AlarmKey.ReferencedAlarmId = referencedAlarm.ServerAlarmCore.Alarm.Id;

                if (FindAlarmByAlarmKey(referencedAlarm.ServerAlarmCore.Alarm.AlarmKey) == null)
                    AddAlarmToMemoryStructures(referencedAlarm);
            }

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
                        this);
                }

                RunAlarmAdded(serverAlarm);

                return;
            }

            if (alarm.AlarmState == AlarmState.Alarm
                && oldServerAlarm.ServerAlarmCore.Alarm.AlarmState == AlarmState.Alarm
                && alarm.AlarmKey.SameParameters(oldServerAlarm.ServerAlarmCore.Alarm.AlarmKey))
            {
                return;
            }

            UpdateAlarm(
                oldServerAlarm.ServerAlarmCore.Alarm,
                alarm.AlarmState,
                alarm.CreatedDateTime,
                alarm.IsAcknowledged,
                alarm.AlarmKey.Parameters);

            if (!loadingAlarms)
            {
                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(oldServerAlarm);

                AlarmsManager.Singleton.AfterAlarmAdded(
                    oldServerAlarm,
                    false,
                    this);
            }

            if (!oldServerAlarm.ServerAlarmCore.Alarm.IsBlocked)
                RunAlarmAdded(oldServerAlarm);
        }

        private void RunAlarmAdded(
            ServerAlarm serverAlarm)
        {
            if (AlarmAdded == null)
                return;

            try
            {
                AlarmAdded(serverAlarm);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
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
            Alarm alarm,
            AlarmState alarmState,
            DateTime createdDateTime,
            bool isAcknowledged,
            IEnumerable<AlarmParameter> parameters)
        {
            alarm.AlarmState = alarmState;
            alarm.CreatedDateTime = createdDateTime;
            alarm.IsAcknowledged = isAcknowledged;
            alarm.AlarmKey.SetParameters(parameters);
        }

        /// <summary>
        /// Finds alarm withc specific Guid
        /// </summary>
        /// <param name="idAlarm"></param>
        /// <returns></returns>
        public ServerAlarm FindAlarmByIdAlarm(Guid idAlarm)
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

        public void StopAlarm(
            AlarmKey alarmKey)
        {
            lock (_lockOperation)
            {
                if (StopAlarm(FindAlarmByAlarmKey(alarmKey)))
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        private bool StopAlarm(
            ServerAlarm serverAlarm)
        {
            if (serverAlarm == null)
                return false;

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm.AlarmState == AlarmState.Normal)
                return false;

            alarm.AlarmState = AlarmState.Normal;

            if (!alarm.IsBlocked)
                RunAlarmStopped(serverAlarm);

            if (!RemoveAlarm(serverAlarm))
            {
                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(
                    serverAlarm);

                AlarmsManager.Singleton.AfterChangeAlarm(
                    serverAlarm,
                    this);
            }

            return true;
        }

        private void RunAlarmStopped(
            ServerAlarm serverAlarm)
        {
            if (AlarmStopped == null)
                return;

            try
            {
                AlarmStopped(serverAlarm);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void StopAlarmsForAlarmType(AlarmType alarmType)
        {
            lock (_lockOperation)
            {
                var serverAlarms = FindAlarmsByAlarmType(alarmType);

                if (serverAlarms == null)
                    return;

                bool runDelegateChangeAlarm = false;

                foreach (var serverAlarm in serverAlarms)
                {
                    if (StopAlarm(serverAlarm))
                    {
                        runDelegateChangeAlarm = true;
                    }
                }

                if (runDelegateChangeAlarm)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        public void StopAlarmsForAlarmType(
            AlarmType alarmType,
            IEnumerable<IdAndObjectType> alarmObjects)
        {
            lock (_lockOperation)
            {
                var alarmObjectsHashSet = new HashSet<IdAndObjectType>(
                    alarmObjects);

                var serverAlarms = FindAlarmsByAlarmType(alarmType);

                if (serverAlarms == null)
                    return;

                bool runDelegateChangeAlarm = false;

                foreach (var serverAlarm in serverAlarms)
                {
                    var alarmObject = serverAlarm.ServerAlarmCore.Alarm.AlarmKey.AlarmObject;

                    if (alarmObject == null)
                        continue;

                    if (!alarmObjectsHashSet.Contains(alarmObject))
                        continue;

                    if (StopAlarm(serverAlarm))
                    {
                        runDelegateChangeAlarm = true;
                    }
                }

                if (runDelegateChangeAlarm)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
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
                {
                    return null;
                }

                if (AcknowledgeAlarm(
                        serverAlarm)
                    && notifyClient)
                {
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
                }

                return serverAlarm;
            }
        }

        public void AcknowledgeAlarm(IEnumerable<Guid> idAlarms)
        {
            lock (_lockOperation)
            {
                bool runDelegateChangeAlarm = false;

                foreach (var idAlarm in idAlarms)
                {
                    var serverAlarm = FindAlarmByIdAlarm(idAlarm);

                    if (serverAlarm == null)
                        continue;

                    if (AcknowledgeAlarm(
                            serverAlarm))
                    {
                        runDelegateChangeAlarm = true;
                    }
                }

                if (runDelegateChangeAlarm)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        private bool AcknowledgeAlarm(
            ServerAlarm serverAlarm)
        {
            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (!alarm.AcknowledgeState())
            {
                return false;
            }

            if (!RemoveAlarm(serverAlarm))
            {
                DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(
                    serverAlarm);

                AlarmsManager.Singleton.AfterChangeAlarm(
                    serverAlarm,
                    this);
            }

            return true;
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
                {
                    return null;
                }

                if (serverAlarm.ServerAlarmCore.Alarm.BlockAlarmIndividual(DateTime.UtcNow))
                {
                    DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(
                        serverAlarm);

                    RunAlarmStopped(serverAlarm);

                    AlarmsManager.Singleton.AfterChangeAlarm(
                        serverAlarm,
                        this);

                    if (notifyClient)
                        AlarmsManager.Singleton.RunDelegateChangeAlarm();
                }

                return serverAlarm;
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

                var alarm = serverAlarm.ServerAlarmCore.Alarm;

                var wasUnblocked = alarm.UnblockAlarmIndividual(DateTime.UtcNow);

                if (!wasUnblocked)
                    return serverAlarm;

                if (!RemoveAlarm(serverAlarm))
                {
                    DatabaseServerAlarms.Singleton.InsertOrUpdateServerAlarm(
                        serverAlarm);

                    AlarmsManager.Singleton.AfterChangeAlarm(
                        serverAlarm,
                        this);

                    if (alarm.AlarmState == AlarmState.Alarm)
                        RunAlarmAdded(serverAlarm);
                }

                if (notifyClient)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();

                return serverAlarm;
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
        /// <param name="alarmKey"></param>
        public void RemoveAlarm(AlarmKey alarmKey)
        {
            lock (_lockOperation)
            {
                var serverAlarm = FindAlarmByAlarmKey(alarmKey);

                if (serverAlarm == null)
                    return;

                RemoveAlarmCore(serverAlarm);

                AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        private bool RemoveAlarm(ServerAlarm serverAlarm)
        {
            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm.AlarmState != AlarmState.Normal
                || !alarm.IsAcknowledged
                || alarm.IsBlocked
                || _backAlarmReferencesById.ContainsKey(alarm.Id))
            {
                return false;
            }

            RemoveAlarmCore(serverAlarm);
            return true;
        }

        private void RemoveAlarmCore(ServerAlarm serverAlarm)
        {
            RemoveAlarmFromMemoryStructures(serverAlarm);

            DatabaseServerAlarms.Singleton.DeleteServerAlarm(
                serverAlarm.ServerAlarmCore.IdServerAlarm);

            AlarmsManager.Singleton.AfterAlarmRemoved(serverAlarm);

            if (serverAlarm.ServerAlarmCore.Alarm.AlarmState == AlarmState.Alarm)
                RunAlarmStopped(serverAlarm);
        }

        public void RemoveAlarmsForAlarmObjects(
            IEnumerable<IdAndObjectType> alarmObjects)
        {
            lock (_lockOperation)
            {
                bool runDelegateChangeAlarm = false;

                foreach (var alarmObject in alarmObjects)
                {
                    var serverAlarms = FindAlarmsByAlarmObject(alarmObject);

                    if (serverAlarms == null)
                        continue;

                    runDelegateChangeAlarm = true;

                    foreach (var serverAlarm in serverAlarms)
                        RemoveAlarmCore(serverAlarm);
                }

                if (runDelegateChangeAlarm)
                    AlarmsManager.Singleton.RunDelegateChangeAlarm();
            }
        }

        private IEnumerable<ServerAlarm> FindAlarmsByAlarmObject(IdAndObjectType alarmObject)
        {
            lock (_lockOperation)
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
            return _serverAlarmsByIdAlarm.GetValuesSnapshot(true);
        }

        public IEnumerable<ServerAlarm> GetAlarms(AlarmType alarmType, IdAndObjectType alarmObject)
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

        public void WriteEventlogAlarmOccured(ServerAlarm serverAlarm)
        {
            CgpServer.Singleton.WriteToEventlogAlarmOccured(serverAlarm);
        }

        public void WriteEventlogAlarmChanged(ServerAlarm serverAlarm)
        {
            CgpServer.Singleton.WriteToEventlogAlarmChanged(serverAlarm);
        }
    }
}
