using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Server.Alarms
{
    public class AlarmsManager : MarshalByRefObject
    {
        private static volatile AlarmsManager _singleton;
        private static readonly object _syncRoot = new object();

        public IServerAlarmsOwner ServerAlarmsOwner { get; private set; }

        private readonly SyncDictionary<Guid, IExternalAlarmsOwner> _externalAlarmsOwners
            = new SyncDictionary<Guid, IExternalAlarmsOwner>();

        public static AlarmsManager Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new AlarmsManager();
                    }

                return _singleton;
            }
        }

        public AlarmsManager GetSingleton()
        {
            return
                _singleton ?? (_singleton = new AlarmsManager());
        }

        public void RegisterAlarmsOwner(
            IServerAlarmsOwner serverAlarmsOwner)
        {
            if (serverAlarmsOwner == null)
                return;

            ServerAlarmsOwner = serverAlarmsOwner;
        }

        public void RegisterAlarmsOwner(
            Guid idAlarmOwner,
            IExternalAlarmsOwner externalAlarmsOwner)
        {
            if (externalAlarmsOwner == null)
                return;

            _externalAlarmsOwners.Add(
                idAlarmOwner,
                externalAlarmsOwner);
        }

        public void UnregisterExternalAlarmsOwner(
            Guid idExternalAlarmsOwner)
        {
            _externalAlarmsOwners.Remove(
                idExternalAlarmsOwner,
                (key, remved, value) =>
                {
                    if (!remved)
                        return;

                    value.RemoveAllAlarms();
                });
        }

        public void TryRunOnAlarmsOwner(Action<IServerAlarmsOwner> processingLambda)
        {
            if (processingLambda == null
                || ServerAlarmsOwner == null)
            {
                return;
            }

            processingLambda(ServerAlarmsOwner);
        }

        public void TryRunOnAlarmsOwner(
            Guid idExternalOwner,
            Action<IExternalAlarmsOwner> processingLambda)
        {
            if (processingLambda == null)
                return;

            IExternalAlarmsOwner externalAlarmsOwner;

            if (!_externalAlarmsOwners.TryGetValue(
                    idExternalOwner,
                    out externalAlarmsOwner))
            {
                return;
            }

            processingLambda(externalAlarmsOwner);
        }

        public event Action<ServerAlarm, bool> ChangedAlarm;

        private DVoid2Void _dChangeAlarm;
        public void SetDelegateChangeAlarm(DVoid2Void dChangeAlarm)
        {
            _dChangeAlarm += dChangeAlarm;
        }

        public void RunDelegateChangeAlarm()
        {
            SafeThread.StartThread(DoRunDelegateChangeAlarm);
        }

        private void DoRunDelegateChangeAlarm()
        {
            if (_dChangeAlarm != null)
            {
                _dChangeAlarm();
            }
        }

        public Action<IdServerAlarm> RemovedAlarm;

        private void DoRunRemovedAlarm(IdServerAlarm deleted)
        {
            if (RemovedAlarm != null)
            {
                try
                {
                    RemovedAlarm(deleted);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        /// <summary>
        /// Adds alarm
        /// </summary>
        /// <param name="serverAlarm"></param>
        public void AddAlarm(
            ServerAlarm serverAlarm)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null
                || serverAlarm.ServerAlarmCore.Alarm == null
                || serverAlarm.ServerAlarmCore.Alarm.AlarmKey == null)
            {
                return;
            }

            var alarmsOwner = GetAlarmsOwner(serverAlarm.ServerAlarmCore.IdServerAlarm.IdOwner);

            if (alarmsOwner == null)
                return;

            AddAlarm(
                serverAlarm,
                alarmsOwner);
        }

        private IAlarmsOwner GetAlarmsOwner(Guid idAlarmsOwner)
        {
            if (idAlarmsOwner == Guid.Empty)
                return ServerAlarmsOwner;

            IExternalAlarmsOwner externalAlarmsOwner;

            if (!_externalAlarmsOwners.TryGetValue(
                idAlarmsOwner,
                out externalAlarmsOwner))
            {
                return null;
            }

            return externalAlarmsOwner;
        }

        private void AddAlarm(
            ServerAlarm serverAlarm,
            IAlarmsOwner alarmsOwner)
        {
            alarmsOwner.AddAlarm(serverAlarm);
        }

        public void AfterAlarmAdded(
            ServerAlarm addedOrUpdatedAlarm,
            bool alarmAdded,
            IServerAlarmsOwner serverAlarmsOwner)
        {
            var serverAlarmCore = addedOrUpdatedAlarm.ServerAlarmCore;

            if (serverAlarmsOwner != null)
                serverAlarmsOwner.WriteEventlogAlarmOccured(addedOrUpdatedAlarm);

            try
            {
                if (ChangedAlarm != null)
                    ChangedAlarm(
                        addedOrUpdatedAlarm,
                        alarmAdded
                        || serverAlarmCore.Alarm.AlarmState == AlarmState.Alarm);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// Finds alarms with specific alarm type
        /// </summary>
        /// <param name="alarmType"></param>
        /// <returns></returns>
        public ICollection<ServerAlarm> FindAlarmsByAlarmType(AlarmType alarmType)
        {
            return new LinkedList<ServerAlarm>(
                GetAllAlarmsOwners()
                    .Select(
                        alarmsOwner =>
                            alarmsOwner.FindAlarmsByAlarmType(alarmType))
                    .Where(
                        ownerServerAlarms =>
                            ownerServerAlarms != null)
                    .SelectMany(
                        ownerServerAlarms =>
                            ownerServerAlarms));
        }

        private IEnumerable<IAlarmsOwner> GetAllAlarmsOwners()
        {
            IEnumerable<IAlarmsOwner> alarmsOwners = Enumerable.Empty<IAlarmsOwner>();

            if (ServerAlarmsOwner != null)
                alarmsOwners = alarmsOwners.Concat(
                    Enumerable.Repeat(
                        ServerAlarmsOwner,
                        1).Cast<IAlarmsOwner>());

            alarmsOwners = alarmsOwners.Concat(
                _externalAlarmsOwners.ValuesSnapshot
                    .Cast<IAlarmsOwner>());

            return alarmsOwners;
        }

        public ICollection<ServerAlarm> FindAlarmsByAlarmType(
            AlarmType alarmType,
            ObjectType alarmObjectType,
            Guid alarmObjectId)
        {
            var idAndObjectType = new IdAndObjectType(
                alarmObjectId,
                alarmObjectType);

            return new LinkedList<ServerAlarm>(
                GetAllAlarmsOwners()
                    .Select(
                        alarmsOwner =>
                            alarmsOwner.FindAlarmsByAlarmType(alarmType))
                    .Where(
                        ownerServerAlarms =>
                            ownerServerAlarms != null)
                    .SelectMany(
                        ownerServerAlarms =>
                            ownerServerAlarms)
                    .Where(
                        serverAlarm =>
                            idAndObjectType.Equals(serverAlarm.ServerAlarmCore.Alarm.AlarmKey.AlarmObject)));
        }

        public void AfterChangeAlarm(
            ServerAlarm serverAlarm,
            IServerAlarmsOwner serverAlarmsOwner)
        {
            var serverAlarmProxy = serverAlarm.ServerAlarmCore;

            if (serverAlarmsOwner != null)
                serverAlarmsOwner.WriteEventlogAlarmChanged(serverAlarm);

            try
            {
                if (ChangedAlarm != null)
                {
                    ChangedAlarm(
                        serverAlarm,
                        true);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void AfterAlarmRemoved(
            ServerAlarm serverAlarm)
        {
            SafeThread<Alarm>.StartThread(
                DoRunRemovedAlarm,
                serverAlarm.ServerAlarmCore.IdServerAlarm);
        }

        public void RemoveAlarmsForAlarmObjects(
            IEnumerable<IdAndObjectType> alarmObjects)
        {
            if (alarmObjects == null)
                return;

            var collectionAlarmObjects = new LinkedList<IdAndObjectType>(
                alarmObjects);

            var allAlarmsOwners = GetAllAlarmsOwners();

            foreach (var alarmsOwner in allAlarmsOwners)
            {
                alarmsOwner.RemoveAlarmsForAlarmObjects(
                    collectionAlarmObjects);
            }
        }

        /// <summary>
        /// Returns all alarms
        /// </summary>
        /// <returns></returns>
        public ICollection<ServerAlarmCore> GetAlarms()
        {
            return new LinkedList<ServerAlarmCore>(
                GetAllAlarmsOwners()
                    .Select(
                        alarmsOwner =>
                            alarmsOwner.GetAlarms())
                    .Where(
                        ownerServerAlarms =>
                            ownerServerAlarms != null)
                    .SelectMany(
                        ownerServerAlarms =>
                            ownerServerAlarms)
                    .Select(serverAlarm =>
                        serverAlarm.ServerAlarmCore)
                    .Where(
                        serverAlarmProxy =>
                            serverAlarmProxy.Alarm.IsBlocked
                            || serverAlarmProxy.Alarm.AlarmState == AlarmState.Alarm
                            || !serverAlarmProxy.Alarm.IsAcknowledged));
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ServerAlarm FindServerAlarmByIdServerAlarm(IdServerAlarm idServerAlarm)
        {
            if (idServerAlarm == null)
                return null;

            var alarmsOwner = GetAlarmsOwner(idServerAlarm.IdOwner);

            if (alarmsOwner == null)
            {
                return null;
            }

            return alarmsOwner.FindAlarmByIdAlarm(idServerAlarm.Id);
        }

        /// <summary>
        /// Acknowledges state of alarms
        /// </summary>
        /// <param name="idsServerAlarm"></param>
        public void AcknowledgeAlarm(
            IEnumerable<IdServerAlarm> idsServerAlarm)
        {
            if (idsServerAlarm == null)
                return;

            foreach (var idServerAlarm in idsServerAlarm)
            {
                var alarmsOwner = GetAlarmsOwner(
                    idServerAlarm.IdOwner);

                if (alarmsOwner == null)
                    continue;

                alarmsOwner.AcknowledgeAlarm(
                    idServerAlarm.Id,
                    false);
            }

            RunDelegateChangeAlarm();
        }

        /// <summary>
        /// Acknowledges state of alarm
        /// </summary>
        /// <param name="idServerAlarm"></param>
        /// <returns></returns>
        public ServerAlarm AcknowledgeAlarm(
            IdServerAlarm idServerAlarm)
        {
            if (idServerAlarm == null)
                return null;

            var alarmsOwner = GetAlarmsOwner(
                idServerAlarm.IdOwner);

            if (alarmsOwner == null)
                return null;

            return alarmsOwner.AcknowledgeAlarm(
                idServerAlarm.Id);
        }

        /// <summary>
        /// Block or unblocks alarms
        /// </summary>
        /// <param name="idsServerAlarm"></param>
        /// <param name="block"></param>
        public void BlockUnblockAlarms(
            IEnumerable<IdServerAlarm> idsServerAlarm,
            bool block)
        {
            if (idsServerAlarm == null)
                return;

            foreach (var idServerAlarm in idsServerAlarm)
            {
                if (block)
                {
                    var alarmsOwner = GetAlarmsOwner(
                        idServerAlarm.IdOwner);

                    if (alarmsOwner == null)
                        continue;

                    alarmsOwner.BlockAlarmIndividual(
                        idServerAlarm.Id,
                        false);
                }
                else
                {
                    var alarmsOwner = GetAlarmsOwner(
                        idServerAlarm.IdOwner);

                    if (alarmsOwner == null)
                        continue;

                    alarmsOwner.UnblockAlarmIndividual(
                        idServerAlarm.Id,
                        false);
                }
            }
            
            RunDelegateChangeAlarm();
        }

        /// <summary>
        /// Block alarm
        /// </summary>
        /// <param name="idServerAlarm"></param>
        /// <returns></returns>
        public ServerAlarm BlockAlarm(IdServerAlarm idServerAlarm)
        {
            if (idServerAlarm == null)
                return null;

            var alarmsOwner = GetAlarmsOwner(
                idServerAlarm.IdOwner);

            if (alarmsOwner == null)
                return null;

            return alarmsOwner.BlockAlarmIndividual(
                idServerAlarm.Id);
        }

        /// <summary>
        /// Unblocks alarm
        /// </summary>
        /// <param name="idServerAlarm"></param>
        /// <returns></returns>
        public ServerAlarm UnblockAlarm(IdServerAlarm idServerAlarm)
        {
            if (idServerAlarm == null)
                return null;

            var alarmsOwner = GetAlarmsOwner(
                idServerAlarm.IdOwner);

            if (alarmsOwner == null)
                return null;

            return alarmsOwner.UnblockAlarmIndividual(
                idServerAlarm.Id);
        }

        public bool IsAlarmBlocked(
            Guid idOwner,
            Guid idAlarm)
        {
            var alarmsOwner = GetAlarmsOwner(idOwner);

            return alarmsOwner != null
                   && alarmsOwner.IsAlarmBlocked(idAlarm);
        }

        public ICollection<ServerAlarmCore> GetAlarms(
            AlarmType alarmType,
            IdAndObjectType alarmObject)
        {
            var serverAlarmsCore =
                new LinkedList<ServerAlarmCore>(
                    GetAllAlarmsOwners().SelectMany(
                        alarmsOwner =>
                        {
                            var ownerAlarms = alarmsOwner.GetAlarms(
                                alarmType,
                                alarmObject);

                            if (ownerAlarms == null)
                                return Enumerable.Empty<ServerAlarmCore>();

                            return ownerAlarms.Select(
                                ownerAlarm =>
                                    ownerAlarm.ServerAlarmCore);
                        }));

            return serverAlarmsCore.Count > 0
                ? serverAlarmsCore
                : null;
        }

        private IDictionary<AlarmType, AlarmPriority> _dicAlarmPriority;

        public AlarmPriority GetAlarmPriority(AlarmType alarmType)
        {
            AlarmPriority resultAlarmPriority;
            if (_dicAlarmPriority != null && _dicAlarmPriority.TryGetValue(alarmType, out resultAlarmPriority))
            {
                return resultAlarmPriority;
            }

            return GetDefaultAlarmPriority(alarmType);
        }

        private AlarmPriority GetDefaultAlarmPriority(AlarmType alarmType)
        {
            switch (alarmType)
            {
                case AlarmType.CCU_Offline:
                case AlarmType.DCU_Offline:
                case AlarmType.DCU_Offline_Due_CCU_Offline:
                case AlarmType.CardReader_Offline:
                case AlarmType.CardReader_Offline_Due_CCU_Offline:
                case AlarmType.CCU_TamperSabotage:
                case AlarmType.DCU_TamperSabotage:
                case AlarmType.CardReader_TamperSabotage:
                case AlarmType.DoorEnvironment_Sabotage:
                case AlarmType.Input_Tamper:
                case AlarmType.CCU_OutdatedFirmware:
                case AlarmType.DCU_OutdatedFirmware:
                case AlarmType.CCU_PrimaryPowerMissing:
                case AlarmType.CCU_BatteryLow:
                case AlarmType.Ccu_Ups_OutputFuse:
                case AlarmType.Ccu_Ups_BatteryFault:
                case AlarmType.Ccu_Ups_BatteryFuse:
                case AlarmType.Ccu_Ups_Overtemperature:
                case AlarmType.Ccu_Ups_TamperSabotage:
                case AlarmType.CCU_ExtFuse:
                case AlarmType.CCU_CoprocessorFailure:
                case AlarmType.CCU_DataChannelDistrupted:
                case AlarmType.ICCU_PortAlreadyUsed:
                case AlarmType.CCU_HighMemoryLoad:
                case AlarmType.CCU_FilesystemProblem:
                case AlarmType.CCU_SdCardNotFound:
                case AlarmType.Sensor_Alarm:
                case AlarmType.Sensor_Tamper_Alarm:
                    return AlarmPriority.Critical;
                case AlarmType.CCU_Unconfigured:
                case AlarmType.DoorEnvironment_Intrusion:
                case AlarmType.ICCU_SendingOfObjectStateFailed:
                case AlarmType.Ccu_CatUnreachable:
                case AlarmType.Ccu_TransferToArcTimedOut:
                    return AlarmPriority.High;
                case AlarmType.CardReader_AccessPermitted:
                    return AlarmPriority.Low;
                default:
                    return AlarmPriority.Medium;
            }
        }

        private IDictionary<AlarmType, ObjectType?> _dicClosestParentObject;

        public ObjectType? GetClosestParentObject(AlarmType alarmType)
        {
            ObjectType? closestParentObject;
            if (_dicClosestParentObject != null && _dicClosestParentObject.TryGetValue(alarmType, out closestParentObject))
            {
                return closestParentObject;
            }

            return GetDefaultClosetParentObject(alarmType);
        }

        public ObjectType? GetDefaultClosetParentObject(AlarmType alarmType)
        {
            switch (alarmType)
            {
                case AlarmType.CCU_Offline:
                case AlarmType.CCU_TamperSabotage:
                case AlarmType.CCU_ClockUnsynchronized:
                case AlarmType.CCU_OutdatedFirmware:
                case AlarmType.CCU_Unconfigured:
                case AlarmType.CCU_CoprocessorFailure:
                    return ObjectType.CCU;
                case AlarmType.DCU_Offline:
                case AlarmType.DCU_Offline_Due_CCU_Offline:
                case AlarmType.DCU_TamperSabotage:
                case AlarmType.DCU_WaitingForUpgrade:
                case AlarmType.DCU_OutdatedFirmware:
                    return ObjectType.DCU;
                case AlarmType.Input_Alarm:
                case AlarmType.Input_Tamper:
                case AlarmType.Sensor_Alarm:
                case AlarmType.Sensor_Tamper_Alarm:
                    return ObjectType.Input;
                case AlarmType.Output_Alarm:
                    return ObjectType.Output;
                case AlarmType.DoorEnvironment_Intrusion:
                case AlarmType.DoorEnvironment_DoorAjar:
                case AlarmType.DoorEnvironment_Sabotage:
                    return ObjectType.DoorEnvironment;
                case AlarmType.AlarmArea_Alarm:
                case AlarmType.AlarmArea_AAlarm:
                    return ObjectType.AlarmArea;
                case AlarmType.CardReader_Offline:
                case AlarmType.CardReader_Offline_Due_CCU_Offline:
                case AlarmType.CardReader_TamperSabotage:
                case AlarmType.CardReader_InvalidCode:
                case AlarmType.CardReader_InvalidEmergencyCode:
                    return ObjectType.CardReader;
                case AlarmType.CardReader_AccessDenied:
                case AlarmType.CardReader_UnknownCard:
                case AlarmType.CardReader_CardBlockedOrInactive:
                case AlarmType.CardReader_InvalidPIN:

                    return ObjectType.Card;
                default:
                    return ObjectType.AllObjectTypes;
            }
        }

        private IDictionary<AlarmType, ObjectType?> _dicSecondClosestParentObject;

        public ObjectType? GetSecondClosestParentObject(AlarmType alarmType)
        {
            ObjectType? secondClosestParentObject;
            if (_dicSecondClosestParentObject != null && _dicSecondClosestParentObject.TryGetValue(alarmType, out secondClosestParentObject))
            {
                return secondClosestParentObject;
            }

            return GetDefaultSecondClosetParentObject(alarmType);
        }

        public ObjectType? GetDefaultSecondClosetParentObject(AlarmType alarmType)
        {
            switch (alarmType)
            {
                case AlarmType.CardReader_AccessDenied:
                case AlarmType.CardReader_CardBlockedOrInactive:
                case AlarmType.CardReader_InvalidPIN:
                    return ObjectType.Person;
                default:
                    return ObjectType.NotSupport;
            }
        }

        public void ReloadAlarmPriorityFromDatabase(IDictionary<AlarmType, AlarmPriority> dicAlarmPriority, IDictionary<AlarmType, ObjectType?> dicClosestParentObject, IDictionary<AlarmType, ObjectType?> dicSecondClosestParentObject)
        {
            _dicAlarmPriority = dicAlarmPriority;
            _dicClosestParentObject = dicClosestParentObject;
            _dicSecondClosestParentObject = dicSecondClosestParentObject;
            ReloadAlarmPriority();
        }

        private void ReloadAlarmPriority()
        {
            var serverAlarmsCore = GetAllAlarmsOwners().SelectMany(
                alarmsOwner =>
                {
                    var ownerServerAlarms = alarmsOwner.GetAlarms();

                    if (ownerServerAlarms == null)
                        return Enumerable.Empty<ServerAlarmCore>();

                    return ownerServerAlarms.Select(
                        serverAlarm =>
                            serverAlarm.ServerAlarmCore);
                });

            foreach (var serverAlarmCore in serverAlarmsCore)
            {
                serverAlarmCore.AlarmPriority = GetAlarmPriority(serverAlarmCore.Alarm.AlarmKey.AlarmType);
            }

            RunDelegateChangeAlarm();
        }
    }
}
