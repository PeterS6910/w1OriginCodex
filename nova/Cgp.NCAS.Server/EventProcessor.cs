using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Sys;
using Contal.IwQuick;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server
{
    public class EventProcessor
    {
        private static readonly Dictionary<EventType, DType2Void<EventProcessor>> _processors 
            = new Dictionary<EventType, DType2Void<EventProcessor>>();
        private readonly CCUEvents _ccuEvents;
        private readonly string _ccuIpAddress;

        private readonly EventOptions _eventOptions;
        private readonly int _eventId;
        private readonly DateTime _dateTime;
        private readonly Guid _objectGuid;
        private readonly EventType _eventType;
        private readonly State _state;
        private readonly object[] _parameters;

        private bool _eventlogEnqueued;

        static EventProcessor()
        {
            _processors.Add(EventType.InputStateChanged, HandleInputStateChanged);
            _processors.Add(EventType.InputStateInfo, HandleInputStateChanged);
            _processors.Add(EventType.OutputStateChanged, HandleOutputStateChanged);
            _processors.Add(EventType.CardReaderOnlineStateChanged, HandleCardReaderOnlineStateChanged);
            _processors.Add(EventType.DcuOnlineStateChanged, HandleDcuOnlineStateChanged);
            _processors.Add(EventType.DSMStateChanged, HandleDsmStateChanged);
            _processors.Add(EventType.OutputRealStateChanged, HandleOutputRealStateChanged);
            _processors.Add(EventType.TemporarilyBlockedInputChanged, HandleTemporarilyBlockedInputChanged);
            _processors.Add(EventType.TemporarilyBlockedInputInfo, HandleTemporarilyBlockedInputChanged);
            _processors.Add(EventType.SensorInAlarmCount, HandleSensorInAlarmCount);
            _processors.Add(EventType.AcknowledgeInputAlarm, HandleAcknowledgeInputAlarm);
            _processors.Add(EventType.AcknowledgeInputAlarmInfo, HandleAcknowledgeInputAlarm);
            _processors.Add(EventType.BlockedInputChanged, HandleBlockedInputChanged);
            _processors.Add(EventType.CardReaderCommandChanged, HandleCardReaderCommandChanged);
            _processors.Add(EventType.CardReaderLastCardChanged, HandleCardReaderLastCardChanged);
            _processors.Add(EventType.SendTamper, HandleSendTamper);
            _processors.Add(EventType.CcuActualTimeSent, HandleCcuActualTimeSent);
            _processors.Add(EventType.CCUTFTPFileReceived, HandleCCUTFTPFileReceived);
            _processors.Add(EventType.CcuUpgradeFileUnpackProgress, HandleCcuUpgradeFileUnpackProgress);
            _processors.Add(EventType.CCUUpgraderStartFailed, HandleCCUUpgraderStartFailed);
            _processors.Add(EventType.CEUpgradeFinished, HandleCEUpgradeFinished);
            _processors.Add(EventType.DcuOutputCount, HandleDcuOutputCount);
            _processors.Add(EventType.DcuFirmwareVersion, HandleDcuFirmwareVersion);
            _processors.Add(EventType.DcuMemoryWarning, HandleDcuMemoryWarning);
            _processors.Add(EventType.CRUpgradePercentageSet, HandleCRUpgradePercentageSet);
            _processors.Add(EventType.CRUpgradeResultSet, HandleCRUpgradeResultSet);
            _processors.Add(EventType.DcuInputCount, HandleDcuInputCount);
            _processors.Add(EventType.DcuPhysicalAddressChanged, HandleDcuPhysicalAddressChanged);
            _processors.Add(EventType.DcuUpgradePercentageSet, HandleDcuUpgradePercentageSet);
            _processors.Add(EventType.ProcessDCUUpgradePackageFailed, HandleProcessDCUUpgradePackageFailed);
            _processors.Add(EventType.ProcessCRUpgradePackageFailed, HandleProcessCRUpgradePackageFailed);
            _processors.Add(EventType.DoSendChecksumEvent, HandleDoSendChecksumEvent);
            _processors.Add(EventType.AlarmCCUPrimaryPowerMissing, HandleAlarmCCUPrimaryPowerMissing);
            _processors.Add(EventType.AlarmCCUBatteryIsLow, HandleAlarmCCUBatteryIsLow);
            _processors.Add(EventType.AlarmCCUExtFuse, HandleAlarmCCUExtFuse);
            _processors.Add(EventType.DcuCommandTimeOut, EnqueueEventLog);
            _processors.Add(EventType.DSMAccessPermitted, EnqueueEventLog);
            _processors.Add(EventType.DSMAccessInterupted, EnqueueEventLog);
            _processors.Add(EventType.DSMAccessViolated, EnqueueEventLog);
            _processors.Add(EventType.DSMApasRestored, EnqueueEventLog);
            _processors.Add(EventType.DSMNormalAccess, EnqueueEventLog);
            _processors.Add(EventType.DcuCommandNotACK, EnqueueEventLog);
            _processors.Add(EventType.DcuNodeReleased, EnqueueEventLog);
            _processors.Add(EventType.DcuNodeRenewed, EnqueueEventLog);
            _processors.Add(EventType.DSMAccessRestricted, EnqueueEventLog);
            _processors.Add(EventType.DcuNodeAssigned, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedCardBlockedOrInactive, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedInvalidGin, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedInvalidEmergencyCode, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedInvalidPin, EnqueueEventLog);
            _processors.Add(EventType.AccessDenied, EnqueueEventLog);
            _processors.Add(EventType.UnknownCard, EnqueueEventLog);
            _processors.Add(EventType.RunMethodFailed, EnqueueEventLog);
            _processors.Add(EventType.CCUTimeAdjusted, EnqueueEventLog);
            _processors.Add(EventType.CCUTimingProblem, EnqueueEventLog);
            _processors.Add(EventType.GetFromDatabaseReturnNull, EnqueueEventLog);
            _processors.Add(EventType.CCUIncomingTransferInfo, EnqueueEventLog);
            _processors.Add(EventType.SectorCardSystemAdded, EnqueueEventLog);
            _processors.Add(EventType.SectorCardSystemRemoved, EnqueueEventLog);
            _processors.Add(EventType.ExceptionOccurred, EnqueueEventLog);
            _processors.Add(EventType.SecurityTimeChannelChanged, HandleSecurityTimeChannelChanged);
            _processors.Add(EventType.UpsOnlineStateChanged, HandleUpsOnlineStateChanged);
            _processors.Add(EventType.UpsAlarmStateChanged, HandleUpsAlarmStateChanged);
            _processors.Add(EventType.UpsValuesChanged, HandleUpsValuesChanged);
            _processors.Add(EventType.CoprocessorFailureChanged, HandleCoprocessorFailureChanged);
            _processors.Add(EventType.ObjectDeserializeFailed, HandleObjectDeserializeFailed);
            _processors.Add(EventType.NotConfirmSetUnsetAAFromEIS, HandleNotConfirmSetUnsetAAFromEIS);
            _processors.Add(EventType.ICCUSendingOfObjectStateFailed, HandleICCUSendingOfObjectStateFailed);
            _processors.Add(EventType.ICCUPortAlreadyUsed, HandleICCUPortAlreadyUsed);
            _processors.Add(EventType.DcuInputsSabotage, HandleDcuInputsSabotage);
            _processors.Add(EventType.CCUMemoryLoadStateChanged, HandleCCUMemoryLoadStateChanged);
            _processors.Add(EventType.CCUFilesystemProblem, HandleCCUFilesystemProblem);
            _processors.Add(EventType.CCUSdCardNotFound, HandeCCUSdCardNotFound);
            _processors.Add(EventType.AntiPassBackZoneCardEntered, HandleAntiPassBackZoneCardEntered);
            _processors.Add(EventType.AntiPassBackZoneCardExited, HandleAntiPassBackZoneCardExited);
            _processors.Add(EventType.AntiPassBackZoneCardTimedOut, HandleAntiPassBackZoneCardTimedOut);
            _processors.Add(EventType.FunctionKeyPressed, EnqueueEventLog);
            _processors.Add(EventType.SetAlarmAreaFromClient, EnqueueEventLog);
            _processors.Add(EventType.UnsetAlarmAreaFromClient, EnqueueEventLog);

            #region AlarmArea

            _processors.Add(EventType.AccessDeniedSetAlarmAreaInvalidGin, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedUnsetAlarmAreaInvalidGin, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedSetAlarmAreaInvalidPin, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedUnsetAlarmAreaInvalidPin, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedSetAlarmAreaNoRights, EnqueueEventLog);
            _processors.Add(EventType.AccessDeniedUnsetAlarmAreaNoRights, EnqueueEventLog);
            _processors.Add(EventType.SetAlarmAreaFromCardReader, EnqueueEventLog);
            _processors.Add(EventType.UnsetAlarmAreaFromCardReader, EnqueueEventLog);
            _processors.Add(EventType.SetAlarmAreaByObjectForAutomaticActivation, EnqueueEventLog);
            _processors.Add(EventType.UnsetAlarmAreaByObjectForAutomaticActivation, EnqueueEventLog);
            _processors.Add(EventType.AlarmAreaAlarmStateChanged, HandleAlarmAreaAlarmStateChanged);
            _processors.Add(EventType.AlarmAreaAlarmStateInfo, HandleAlarmAreaAlarmStateChanged);
            _processors.Add(EventType.AlarmAreaActivationStateChanged, HandleAlarmAreaActivationStateChanged);
            _processors.Add(EventType.AlarmAreaSabotageStateChanged, HandleAlarmAreaSabotageStateChanged);
            _processors.Add(EventType.AlarmAreaSabotageStateInfo, HandleAlarmAreaSabotageStateChanged);
            _processors.Add(EventType.AlarmAreaRequestActivationState, HandleAlarmAreaRequestActivationState);
            _processors.Add(EventType.AlarmAreaAlarmStateChangeAlarm, HandleAlarmAreaAlarmStateChangeAlarm);
            _processors.Add(EventType.AlarmAreaSetFromCRFailed, EnqueueEventLog);
            _processors.Add(EventType.AlarmAreaBoughtTimeChanged, HandleAlarmAreaBoughtTimeChanged);
            _processors.Add(EventType.AlarmAreaTimeBuyingFailed, HandleAlarmAreaTimeBuyingFailed);
            _processors.Add(EventType.AlarmAreaBoughtTimeExpired, HandleAlarmAreaBoughtTimeExpired);

            #endregion
        }

        public EventProcessor(
            CCUEvents ccuEvents,
            EventOptions eventOptions)
        {
            _ccuEvents = ccuEvents;
            _ccuIpAddress = _ccuEvents.CCUSettings.IPAddressString;

            _eventOptions = eventOptions;

            _eventlogEnqueued = false;

            _eventId = eventOptions.EventId;
            _dateTime = eventOptions.DateTime;
            _objectGuid = eventOptions.ObjectGuid;
            _eventType = eventOptions.EventType;
            _state = eventOptions.State;
            _parameters = eventOptions.Parameters;
        }

        public void Process()
        {
            DType2Void<EventProcessor> processor;

            if(_processors.TryGetValue(_eventType, out processor))
                processor(this);

            if (_eventlogEnqueued)
                return;

            if (_eventOptions.EventFromAutonomousRun)
                _ccuEvents.DeleteEventFromAutonomousRun(_eventId);
            else
                _ccuEvents.EndEventProcessing(_eventId);
        }

        private static void HandleUpsValuesChanged(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is CUps2750Values)
                CCUConfigurationHandler.Singleton.CcuUpsValuesChanged(
                    processor._ccuIpAddress,
                    (CUps2750Values)processor._parameters[0]);
        }

        private static void HandleAlarmAreaAlarmStateChangeAlarm(EventProcessor processor)
        {
            CreateAlarmsForAlarmAreas.ChangeAlarmForAlarmArea(
                processor._objectGuid, 
                processor._state, 
                processor._dateTime, 
                processor._ccuIpAddress);
        }

        private static void HandleDoSendChecksumEvent(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 2 ||
                    !(processor._parameters[0] is uint) ||
                    !(processor._parameters[1] is string))
                return;

            if (String.Equals(
                    (string)processor._parameters[1],
                    Cards.CARDS_STREAM_NAME,
                    StringComparison.CurrentCultureIgnoreCase))
                Cards.Singleton.ProcessReceivedChecksum((uint)processor._parameters[0]);
        }

        private static void HandleProcessCRUpgradePackageFailed(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 2 &&
                processor._parameters[0] is byte &&
                processor._parameters[1] is byte[])
                CCUConfigurationHandler.Singleton.ProcessCRUpgradePackageFailed(
                    (byte)processor._parameters[0],
                    (byte[])processor._parameters[1],
                    processor._ccuIpAddress);
        }

        private static void HandleProcessDCUUpgradePackageFailed(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 2 &&
                processor._parameters[0] is byte[] &&
                processor._parameters[1] is byte)
                CCUConfigurationHandler.Singleton.ProcessDCUUpgradePackageFailed(
                    (byte[])processor._parameters[0],
                    (byte)processor._parameters[1],
                    processor._ccuIpAddress);
        }

        private static void HandleDcuUpgradePercentageSet(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 2 &&
                processor._parameters[0] is byte &&
                processor._parameters[1] is int)
                CCUConfigurationHandler.Singleton.DcuUpgradePercentageSet(
                    (byte)processor._parameters[0],
                    (int)processor._parameters[1],
                    processor._ccuIpAddress);
        }

        private static void HandleDcuPhysicalAddressChanged(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 2 &&
                processor._parameters[0] is byte &&
                processor._parameters[1] is string)
                CCUConfigurationHandler.Singleton.DCUPhysicalAddressChanged(
                    processor._dateTime,
                    (byte)processor._parameters[0],
                    (string)processor._parameters[1],
                    processor._ccuIpAddress);
        }

        private static void HandleDcuInputCount(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is byte)
                CCUConfigurationHandler.Singleton.DCUInputCount(
                    processor._objectGuid,
                    (byte)processor._parameters[0]);
        }

        private static void HandleCRUpgradeResultSet(EventProcessor processor)
        {
            try
            {
                if (processor._parameters != null &&
                    processor._parameters.Length == 3 &&
                    (processor._parameters[0] == null || processor._parameters[0] is byte?) &&
                    processor._parameters[1] is byte &&
                    processor._parameters[2] is byte)
                    CCUConfigurationHandler.Singleton.CRUpgradeResultSet(
                        (byte?) processor._parameters[0],
                        (byte) processor._parameters[1],
                        (byte) processor._parameters[2],
                        processor._ccuIpAddress);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private static void HandleCRUpgradePercentageSet(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 3 &&
                (processor._parameters[0] == null || processor._parameters[0] is byte?) &&
                processor._parameters[1] is byte &&
                processor._parameters[2] is int)
                CCUConfigurationHandler.Singleton.CRUpgradePercentageSet(
                    (byte?)processor._parameters[0],
                    (byte)processor._parameters[1],
                    (int)processor._parameters[2],
                    processor._ccuIpAddress);
        }

        private static void HandleDcuMemoryWarning(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is byte)
                CCUConfigurationHandler.Singleton.DcuMemoryWarning(
                    processor._objectGuid,
                    (byte)processor._parameters[0]);
        }

        private static void HandleDcuFirmwareVersion(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is string)
                try
                {
                    CCUConfigurationHandler.Singleton.DCUFirmwareVersion(
                        processor._objectGuid,
                        (string)processor._parameters[0]);
                }
                catch
                {
                }
        }

        private static void HandleCCUUpgraderStartFailed(EventProcessor processor)
        {
            CCUConfigurationHandler.Singleton.CCUUpgraderStartFailed(processor._ccuIpAddress);
        }

        private static void HandleCcuUpgradeFileUnpackProgress(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is int)
                CCUConfigurationHandler.Singleton.CcuUpgradeFileUnpackProgress(
                    (int)processor._parameters[0],
                    processor._ccuIpAddress);
        }

        private static void HandleCCUTFTPFileReceived(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is string)
                CCUConfigurationHandler.Singleton.CCUTFTPFileReceived(
                    (string)processor._parameters[0],
                    processor._ccuIpAddress);
        }

        private static void HandleCcuActualTimeSent(EventProcessor processor)
        {
            if (processor._parameters != null)
            {
                if (processor._parameters.Length == 2 
                    && processor._parameters[0] is DateTime
                    && processor._parameters[1] is DateTime)
                {
                    // Old implementation use LOCAL time so convert server time to local
                    processor._ccuEvents.CCUSettings.CcuActualTimeReceived(
                        (DateTime) processor._parameters[0],
                        ((DateTime) processor._parameters[1]).ToLocalTime(),
                        processor._ccuIpAddress);
                }
                else if (processor._parameters.Length == 3
                         && processor._parameters[0] is bool
                         && processor._parameters[1] is DateTime
                         && processor._parameters[2] is DateTime)
                {
                    // new implementation of time use UTC time
                    if ((bool) processor._parameters[0])
                    {
                        processor._ccuEvents.CCUSettings.CcuActualTimeReceived(
                        (DateTime)processor._parameters[1],
                        ((DateTime)processor._parameters[2]),
                        processor._ccuIpAddress);
                    }
                }
            }
        }

        private static void HandleCardReaderLastCardChanged(EventProcessor processor)
        {
            if (processor._parameters != null &&
                    processor._parameters.Length == 1 &&
                    processor._parameters[0] is string)
                CCUConfigurationHandler.Singleton.CardReaderLastCardChanged(
                    processor._objectGuid,
                    processor._dateTime,
                    (string)processor._parameters[0],
                    processor._ccuIpAddress);
        }

        private static void HandleCardReaderCommandChanged(EventProcessor processor)
        {
            if (processor._parameters != null &&
                    processor._parameters.Length == 1 &&
                    processor._parameters[0] is byte)
                CCUConfigurationHandler.Singleton.CardReaderCommandChanged(
                    processor._objectGuid,
                    processor._dateTime,
                    (byte)processor._parameters[0]);
        }

        private static void HandleBlockedInputChanged(EventProcessor processor)
        {
            CCUConfigurationHandler.Singleton.BlockedInputChanged(
                processor._objectGuid,
                processor._state == State.On);
        }

        private static void HandleAcknowledgeInputAlarm(EventProcessor processor)
        {
            CCUConfigurationHandler.Singleton.AcknowledgeInputAlarm(processor._objectGuid);
        }

        private static void HandleDcuOutputCount(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is byte)
                CCUConfigurationHandler.Singleton.DCUOutputCount(
                    processor._objectGuid,
                    (byte)processor._parameters[0]);
        }

        private static void HandleCEUpgradeFinished(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 3 &&
                processor._parameters[0] is int &&
                processor._parameters[1] is int &&
                processor._parameters[2] is string)
                CCUConfigurationHandler.Singleton.CEUpgradeFinished(
                    (int)processor._parameters[0],
                    (int)processor._parameters[1],
                    (string)processor._parameters[2],
                    processor._ccuIpAddress);
        }

        private static void HandleSensorInAlarmCount(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is int)
                CCUConfigurationHandler.Singleton.SensorsInAlarmCount(
                    processor._objectGuid,
                    processor._dateTime,
                    (int)processor._parameters[0],
                    processor._ccuIpAddress);
        }

        private static void HandleTemporarilyBlockedInputChanged(EventProcessor processor)
        {
            CCUConfigurationHandler.Singleton.TemporarilyBlockedInputChanged(
                processor._objectGuid,
                processor._dateTime,
                processor._state == State.On);
        }

        private static void HandleUpsAlarmStateChanged(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is bool)
                CCUConfigurationHandler.Singleton.CcuUpsAlarmStateChanged(
                    processor._ccuIpAddress,
                    (bool)processor._parameters[0]);
        }

        private static void HandleUpsOnlineStateChanged(EventProcessor processor)
        {
            if (processor._parameters != null &&
                processor._parameters.Length == 1 &&
                processor._parameters[0] is byte)
                CCUConfigurationHandler.Singleton.CcuUpsOnlineStateChanged(
                    processor._ccuIpAddress,
                    (byte)processor._parameters[0]);
        }

        private static void HandleSecurityTimeChannelChanged(EventProcessor processor)
        {
            processor._eventOptions.Parameters =
                Enumerable
                    .Repeat((object)processor._state, 1)
                    .Concat(processor._parameters ?? Enumerable.Empty<object>())
                    .ToArray();

            EnqueueEventLog(processor);
        }

        private static void HandleDcuInputsSabotage(EventProcessor processor)
        {
            CCUConfigurationHandler.Singleton.DcuInputsSabotageStateChanged(
                processor._dateTime,
                processor._objectGuid,
                processor._state);
        }

        private static void HandleAlarmAreaSabotageStateChanged(EventProcessor processor)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSabotageStateChanged(
                processor._objectGuid,
                processor._dateTime,
                processor._state);
        }

        private static void HandleNotConfirmSetUnsetAAFromEIS(EventProcessor processor)
        {
            CCUConfigurationHandler.Singleton.AlarmAreaSetUnsetNotConfirm(
                processor._objectGuid,
                processor._dateTime);

            EnqueueEventLog(processor);
        }

        private static void HandleICCUPortAlreadyUsed(EventProcessor processor)
        {
            var iccuPortAlredyUsedCCU =
                CCUs.Singleton.GetCCUFormIpAddress(processor._ccuIpAddress);

            if (iccuPortAlredyUsedCCU == null)
                return;

            switch (processor._state)
            {
                case State.Alarm:

                    CreateAlarmsGlobals.CreateAlarm(
                        processor._dateTime,
                        processor._ccuIpAddress,
                        iccuPortAlredyUsedCCU.IdCCU,
                        ObjectType.CCU,
                        AlarmType.ICCU_PortAlreadyUsed,
                        null);

                    EnqueueEventLog(processor);
                    break;

                case State.Normal:

                    CreateAlarmsGlobals.StopAlarm(
                        processor._dateTime,
                        processor._ccuIpAddress,
                        iccuPortAlredyUsedCCU.IdCCU,
                        ObjectType.CCU,
                        AlarmType.ICCU_PortAlreadyUsed,
                        null);

                    break;
            }
        }

        private static void HandleICCUSendingOfObjectStateFailed(EventProcessor processor)
        {
            if (processor._parameters == null ||
                processor._parameters.Length <= 0 ||
                !(processor._parameters[0] is int))
                return;

            var objectTypeICCUSendingOfObjectStateFailed =
                (ObjectType)processor._parameters[0];

            CreateAlarmsGlobals.ForceCreateAlarmInNormal(
                processor._dateTime,
                processor._ccuIpAddress,
                processor._objectGuid,
                objectTypeICCUSendingOfObjectStateFailed,
                AlarmType.ICCU_SendingOfObjectStateFailed,
                null);

            EnqueueEventLog(processor);
        }

        private static void HandleAlarmAreaRequestActivationState(EventProcessor processor)
        {
            RequestActivationState requestActivationState;

            switch (processor._state)
            {
                case State.Set:
                    requestActivationState = RequestActivationState.Set;
                    break;

                case State.Unset:
                    requestActivationState = RequestActivationState.Unset;
                    break;

                case State.UnconditionaSet:
                    requestActivationState = RequestActivationState.UnconditionalSet;
                    break;

                default:
                    requestActivationState = RequestActivationState.Unknown;
                    break;
            }

            CCUConfigurationHandler.Singleton.AlarmAreaRequestActivationStateChanged(
                processor._objectGuid,
                processor._dateTime,
                requestActivationState);
        }

        private static void HandleObjectDeserializeFailed(EventProcessor processor)
        {
            if (GeneralOptions.Singleton.CorrectDeserializationFailures)
            {
                var objectDeserializeFailedCCU =
                    CCUs.Singleton.GetCCUFormIpAddress(processor._ccuIpAddress);

                if (objectDeserializeFailedCCU != null &&
                    processor._objectGuid != Guid.Empty &&
                    processor._parameters.Length > 0 &&
                    processor._parameters[0] is int)
                {
                    var objectType = (ObjectType)processor._parameters[0];

                    if (objectType != ObjectType.NotSupport)
                        CCUConfigurationHandler.Singleton.SendModifyObjectsToCCUsAsync(
                            processor._objectGuid,
                            objectType);
                }
            }

            EnqueueEventLog(processor);
        }

        private static void HandleCoprocessorFailureChanged(EventProcessor processor)
        {
            var coprocessorFailureChangedCCU =
                CCUs.Singleton.GetCCUFormIpAddress(processor._ccuIpAddress);

            if (coprocessorFailureChangedCCU != null)
            {
                var relativeUniqueIdentifikate =
                    AlarmsManager.GetAlarmRelativeUniqueIdentifier(
                        coprocessorFailureChangedCCU.IdCCU,
                        ObjectType.CCU,
                        AlarmType.CCU_CoprocessorFailure,
                        null);

                var description =
                    AlarmsManager.GetAlarmDescription(
                        coprocessorFailureChangedCCU.IdCCU,
                        ObjectType.CCU,
                        AlarmType.CCU_CoprocessorFailure,
                        null);

                var alarmParentObjectList =
                    AlarmsManager.GetAlarmParentObjectList(
                        coprocessorFailureChangedCCU.IdCCU,
                        ObjectType.CCU,
                        processor._ccuIpAddress);

                // ReSharper disable once ObjectCreationAsStatement
                new Alarm(
                    CCUConfigurationHandler.Singleton.NcasServer.GetAlarmsQueue(),
                    processor._dateTime,
                    AlarmsManager.GetParentObject(processor._ccuIpAddress),
                    alarmParentObjectList,
                    relativeUniqueIdentifikate,
                    processor._state == State.Alarm
                        ? AlarmState.Alarm
                        : AlarmState.Normal,
                    false,
                    description,
                    coprocessorFailureChangedCCU.IdCCU,
                    ObjectType.CCU,
                    AlarmType.CCU_CoprocessorFailure,
                    null);
            }

            processor._eventOptions.Parameters = new object[] { processor._state };

            EnqueueEventLog(processor);
        }

        private static void HandleAlarmCCUExtFuse(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 2 ||
                    !(processor._parameters[0] is bool) ||
                    !(processor._parameters[1] is bool))
                return;

            switch (processor._state)
            {
                case State.On:

                    if ((bool)processor._parameters[0])
                    {
                        processor._eventOptions.Parameters = new object[] { true };

                        EnqueueEventLog(processor);
                    }

                    if ((bool)processor._parameters[1])
                        CreateAlarmsGlobals.CreateAlarm(
                            processor._dateTime,
                            processor._ccuIpAddress,
                            processor._objectGuid,
                            ObjectType.CCU,
                            AlarmType.CCU_ExtFuse);
                    break;

                case State.Off:

                    if ((bool)processor._parameters[0])
                    {
                        processor._eventOptions.Parameters = new object[] { false };

                        EnqueueEventLog(processor);
                    }

                    if ((bool)processor._parameters[1])
                        CreateAlarmsGlobals.StopAlarm(
                            processor._dateTime,
                            processor._ccuIpAddress,
                            processor._objectGuid,
                            ObjectType.CCU,
                            AlarmType.CCU_ExtFuse);

                    break;
            }
        }

        private static void HandleSendTamper(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 3 ||
                    !(processor._parameters[0] is byte) ||
                    !(processor._parameters[1] is bool) ||
                    !(processor._parameters[2] is bool))
                return;

            if ((byte) processor._parameters[0] == (byte) ObjectType.CardReader)
            {
                var ccu = CCUs.Singleton.GetCCUFormIpAddress(processor._ccuIpAddress);

                if (ccu != null)
                {
                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        CCUCallbackRunner.RunCardReaderTamperStateChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[]
                        {
                            processor._objectGuid,
                            processor._state == State.On,
                            ccu.IdCCU
                        });
                }
            }

            if ((bool)processor._parameters[1])
            {
                processor._eventOptions.Parameters =
                    new[]
                    {
                        processor._parameters[0],
                        processor._state == State.On
                    };

                EnqueueEventLog(processor);
            }

            if ((bool)processor._parameters[2])
                CCUAlarms.CreateAlarmTamper(
                    processor._objectGuid,
                    processor._dateTime,
                    processor._state == State.On,
                    (byte)processor._parameters[0],
                    processor._ccuIpAddress);
        }

        private static void HandleAlarmCCUBatteryIsLow(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 2 ||
                    !(processor._parameters[0] is bool) ||
                    !(processor._parameters[1] is bool))
                return;

            switch (processor._state)
            {
                case State.On:

                    if ((bool)processor._parameters[0])
                    {
                        processor._eventOptions.Parameters = new object[] { true };

                        EnqueueEventLog(processor);
                    }

                    if ((bool)processor._parameters[1])
                        CreateAlarmsGlobals.CreateAlarm(
                            processor._dateTime,
                            processor._ccuIpAddress,
                            processor._objectGuid,
                            ObjectType.CCU,
                            AlarmType.CCU_BatteryLow);

                    break;

                case State.Off:

                    if ((bool)processor._parameters[0])
                    {
                        processor._eventOptions.Parameters = new object[] { false };

                        EnqueueEventLog(processor);
                    }

                    if ((bool)processor._parameters[1])
                        CreateAlarmsGlobals.StopAlarm(
                            processor._dateTime,
                            processor._ccuIpAddress,
                            processor._objectGuid,
                            ObjectType.CCU,
                            AlarmType.CCU_BatteryLow);

                    break;
            }
        }

        private static void HandleAlarmCCUPrimaryPowerMissing(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 2 ||
                    !(processor._parameters[0] is bool) ||
                    !(processor._parameters[1] is bool))
                return;

            switch (processor._state)
            {
                case State.On:

                    if ((bool)processor._parameters[0])
                    {
                        processor._eventOptions.Parameters = new object[] { true };
                        EnqueueEventLog(processor);
                    }

                    if ((bool)processor._parameters[1])
                        CreateAlarmsGlobals.CreateAlarm(
                            processor._dateTime,
                            processor._ccuIpAddress,
                            processor._objectGuid,
                            ObjectType.CCU,
                            AlarmType.CCU_PrimaryPowerMissing);

                    break;

                case State.Off:

                    if ((bool)processor._parameters[0])
                    {
                        processor._eventOptions.Parameters = new object[] { false };
                        EnqueueEventLog(processor);
                    }

                    if ((bool)processor._parameters[1])
                        CreateAlarmsGlobals.StopAlarm(
                            processor._dateTime,
                            processor._ccuIpAddress,
                            processor._objectGuid,
                            ObjectType.CCU,
                            AlarmType.CCU_PrimaryPowerMissing);

                    break;
            }
        }

        private static void HandleOutputRealStateChanged(EventProcessor processor)
        {
            var realOutputState = OutputState.Unknown;

            switch (processor._state)
            {
                case State.On:
                    realOutputState = OutputState.On;
                    break;

                case State.Off:
                    realOutputState = OutputState.Off;
                    break;
            }

            CCUConfigurationHandler.Singleton.OutputRealStateChanged(
                processor._objectGuid,
                processor._dateTime,
                realOutputState);
        }

        private static void HandleDsmStateChanged(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 2 ||
                    !(processor._parameters[0] is bool) ||
                    !(processor._parameters[1] is bool))
                return;

            var doorEnvironmentState = DoorEnvironmentState.Unknown;

            switch (processor._state)
            {
                case State.locked:
                    doorEnvironmentState = DoorEnvironmentState.Locked;
                    break;

                case State.unlocking:
                    doorEnvironmentState = DoorEnvironmentState.Unlocking;
                    break;

                case State.unlocked:
                    doorEnvironmentState = DoorEnvironmentState.Unlocked;
                    break;

                case State.opened:
                    doorEnvironmentState = DoorEnvironmentState.Opened;
                    break;

                case State.locking:
                    doorEnvironmentState = DoorEnvironmentState.Locking;
                    break;

                case State.intrusion:
                    doorEnvironmentState = DoorEnvironmentState.Intrusion;
                    break;

                case State.sabotage:
                    doorEnvironmentState = DoorEnvironmentState.Sabotage;
                    break;

                case State.ajarPrewarning:
                    doorEnvironmentState = DoorEnvironmentState.AjarPrewarning;
                    break;

                case State.ajar:
                    doorEnvironmentState = DoorEnvironmentState.Ajar;
                    break;
            }

            CCUConfigurationHandler.Singleton.DoorEnvironmentStateChanged(
                processor._objectGuid,
                processor._dateTime,
                doorEnvironmentState,
                (bool)processor._parameters[1]);

            if (!(bool)processor._parameters[0])
                return;

            processor._eventOptions.Parameters = new object[] { doorEnvironmentState };
            EnqueueEventLog(processor);
        }

        private static void HandleDcuOnlineStateChanged(EventProcessor processor)
        {
            if (processor._parameters == null ||
                processor._parameters.Length != 2 ||
                !(processor._parameters[0] is byte))
                return;

            var onlineState = OnlineState.Unknown;

            switch (processor._state)
            {
                case State.Online:
                    onlineState = OnlineState.Online;
                    break;

                case State.Offline:
                    onlineState = OnlineState.Offline;
                    break;

                case State.AutoUpgrading:
                    onlineState = OnlineState.AutoUpgrading;
                    break;

                case State.Reseting:
                    onlineState = OnlineState.Reseting;
                    break;

                case State.Upgrading:
                    onlineState = OnlineState.Upgrading;
                    break;

                case State.WaitingForUpgrade:
                    onlineState = OnlineState.WaitingForUpgrade;
                    break;
            }

            CCUConfigurationHandler.Singleton.DCUOnlineStateChanged(
                processor._dateTime,
                (byte)processor._parameters[0],
                onlineState,
                processor._ccuIpAddress,
                processor._parameters[1] is byte
                    ? (byte)processor._parameters[1]
                    : (byte)0xFF);
        }

        private static void HandleAlarmAreaActivationStateChanged(EventProcessor processor)
        {
            var activationState = ActivationState.Unknown;

            switch (processor._state)
            {
                case State.Set:
                    activationState = ActivationState.Set;
                    break;

                case State.Unset:
                    activationState = ActivationState.Unset;
                    break;

                case State.Prewarning:
                    activationState = ActivationState.Prewarning;
                    break;

                case State.TemporaryUnsetExit:
                    activationState = ActivationState.TemporaryUnsetExit;
                    break;

                case State.TemporaryUnsetEntry:
                    activationState = ActivationState.TemporaryUnsetEntry;
                    break;

                case State.UnsetBoughtTime:
                    activationState = ActivationState.UnsetBoughtTime;
                    break;
            }

            CCUConfigurationHandler.Singleton.AlarmAreaActivationStateChanged(
                processor._objectGuid,
                processor._dateTime,
                activationState);

            if (!GeneralOptions.Singleton.EventlogAlarmAreaActivationStateChanged)
                return;

            processor._eventOptions.Parameters =
                Enumerable
                    .Repeat((object)activationState, 1)
                    .Concat(processor._parameters ?? Enumerable.Empty<object>())
                    .ToArray();

            EnqueueEventLog(processor);
        }

        private static void EnqueueEventLog(EventProcessor processor)
        {
            if (processor._ccuEvents.EnqueueEventLog(
                processor._ccuIpAddress,
                processor._eventOptions))
            {
                processor._eventlogEnqueued = true;
            }
        }

        private static void HandleAlarmAreaAlarmStateChanged(EventProcessor processor)
        {
            var alarmAreaAlarmState = AlarmAreaAlarmState.Unknown;

            switch (processor._state)
            {
                case State.Alarm:
                    alarmAreaAlarmState = AlarmAreaAlarmState.Alarm;
                    break;

                case State.Normal:
                    alarmAreaAlarmState = AlarmAreaAlarmState.Normal;
                    break;
            }

            CCUConfigurationHandler.Singleton.AlarmAreaAlarmStateChanged(
                processor._objectGuid,
                processor._dateTime,
                alarmAreaAlarmState);

            if (processor._eventType != EventType.AlarmAreaAlarmStateChanged
                || !GeneralOptions.Singleton.EventlogAlarmAreaAlarmStateChanged)
            {
                return;
            }

            processor._eventOptions.Parameters = new object[] { alarmAreaAlarmState };

            EnqueueEventLog(processor);
        }

        private static void HandleOutputStateChanged(EventProcessor processor)
        {
            var outputState = OutputState.Unknown;

            switch (processor._state)
            {
                case State.On:
                    outputState = OutputState.On;
                    break;

                case State.Off:
                    outputState = OutputState.Off;
                    break;

                case State.UsedByAnotherAplication:
                    outputState = OutputState.UsedByAnotherAplication;
                    break;

                case State.OutOfRange:
                    outputState = OutputState.OutOfRange;
                    break;
            }

            bool saveAlarmOutput;

            if (processor._parameters != null &&
                processor._parameters.Length > 0 &&
                processor._parameters[0] is bool)
                saveAlarmOutput = (bool)processor._parameters[0];
            else
            {
                var output = Outputs.Singleton.GetById(processor._objectGuid);

                saveAlarmOutput =
                    output != null &&
                    output.AlarmControlByObjOn;
            }

            CCUConfigurationHandler.Singleton.OutputStateChanged(
                processor._objectGuid,
                processor._dateTime,
                outputState,
                saveAlarmOutput);

            if (!GeneralOptions.Singleton.EventlogOutputStateChanged)
                return;

            processor._eventOptions.Parameters = new object[] { outputState };
            EnqueueEventLog(processor);
        }

        private static void HandleInputStateChanged(EventProcessor processor)
        {
            var inputState = InputState.Unknown;

            switch (processor._state)
            {
                case State.Alarm:
                    inputState = InputState.Alarm;
                    break;

                case State.Normal:
                    inputState = InputState.Normal;
                    break;

                case State.Break:
                    inputState = InputState.Break;
                    break;

                case State.Short:
                    inputState = InputState.Short;
                    break;

                case State.UsedByAnotherAplication:
                    inputState = InputState.UsedByAnotherAplication;
                    break;

                case State.OutOfRange:
                    inputState = InputState.OutOfRange;
                    break;
            }

            var saveAlarm =
                ShouldBeAlarmSaved(
                    processor._parameters,
                    processor._objectGuid,
                    inputState);

            var alarmAreasGuids =
                processor._parameters != null && processor._parameters.Length > 3
                    ? processor._parameters
                        .Skip(3)
                        .Where(parameter => parameter != null)
                        .Cast<Guid>()
                        .ToList()
                    : new List<Guid>();

            var saveNormalAlarm = false;

            if (processor._parameters != null &&
                    processor._parameters.Length > 2 &&
                    processor._parameters[2] is bool)
                saveNormalAlarm = (bool)processor._parameters[2];

            CCUConfigurationHandler.Singleton.InputChanged(
                processor._objectGuid,
                processor._dateTime,
                inputState,
                saveAlarm,
                saveNormalAlarm,
                alarmAreasGuids);

            if (processor._eventType != EventType.InputStateChanged
                || !GeneralOptions.Singleton.EventlogInputStateChanged)
            {
                return;
            }

            if (processor._eventOptions.Parameters != null && processor._eventOptions.Parameters.Length > 0 &&
                    processor._eventOptions.Parameters[0] is bool && !(bool)processor._eventOptions.Parameters[0])
                return;

            var intputParameters =
                new List<object>
                    {
                        inputState
                    };
            intputParameters.AddRange(alarmAreasGuids.Cast<object>());

            processor._eventOptions.Parameters = intputParameters.ToArray();

            EnqueueEventLog(processor);
        }

        private static bool ShouldBeAlarmSaved(
            object[] parameters,
            Guid objectGuid,
            InputState inputState)
        {
            if (parameters != null && parameters.Length > 1 &&
                    parameters[1] is bool)
                return (bool)parameters[1];

            var input = Inputs.Singleton.GetById(objectGuid);

            if (input == null)
                return false;

            if (inputState == InputState.Short || inputState == InputState.Break)
                return
                    input.AlarmTamper ||
                    input.AAInputs != null && input.AAInputs.Count > 0;

            if (input.AlarmOn)
                return true;

            if (input.AAInputs == null)
                return false;

            foreach (var aaInput in input.AAInputs)
            {
                if (aaInput == null || aaInput.AlarmArea == null)
                    continue;

                var alstmAreaActivationState =
                    CCUConfigurationHandler.Singleton
                        .GetAlarmAreaActivationState(
                            aaInput.AlarmArea.IdAlarmArea);

                if (alstmAreaActivationState != ActivationState.Unknown &&
                        alstmAreaActivationState != ActivationState.Unset &&
                        alstmAreaActivationState != ActivationState.UnsetBoughtTime)
                    return true;
            }

            return false;
        }

        private static void HandleCardReaderOnlineStateChanged(EventProcessor processor)
        {
            if (processor._state == State.Upgrading)
            {
                CCUConfigurationHandler.Singleton.SetCrStateUpgrading(
                    CCUConfigurationHandler.Singleton.GetIdCCUFromIpAddress(processor._ccuIpAddress), 
                    processor._objectGuid, 
                    true);

                return;
            }

            if (processor._parameters == null ||
                processor._parameters.Length != 7 ||
                !(processor._parameters[0] is string) ||
                !(processor._parameters[1] is byte) ||
                !(processor._parameters[2] is bool) ||
                !(processor._parameters[3] is string) ||
                !(processor._parameters[4] is string) ||
                !(processor._parameters[5] is string) ||
                !(processor._parameters[6] is byte))
                return;

            CCUConfigurationHandler.Singleton.CardReaderOnlineStateChanged(
                processor._dateTime,
                processor._objectGuid,
                new CCUConfigurationHandler.CardReaderCreationParams
                {
                    port = (string)processor._parameters[0],
                    address = (byte)processor._parameters[1],
                    onlineState = (bool)processor._parameters[2],
                    protocolVersion = (string)processor._parameters[3],
                    firmwareVersion = (string)processor._parameters[4],
                    hardwareVersion = (string)processor._parameters[5],
                    protocolMajor = (byte)processor._parameters[6],
                },
                processor._ccuIpAddress);
        }

        private static void HandleCCUMemoryLoadStateChanged(EventProcessor processor)
        {
            if (processor._parameters == null ||
                processor._parameters.Length != 2 ||
                !(processor._parameters[0] is bool) ||
                !(processor._parameters[1] is bool))
            {
                return;
            }

            var saveEventlog = (bool)processor._parameters[0];
            var saveAlarm = (bool)processor._parameters[1];

            if (saveEventlog)
            {
                processor._eventOptions.Parameters = new object[] { processor._state };
                EnqueueEventLog(processor);
            }

            if (saveAlarm)
            {
                var ccu = CCUs.Singleton.GetCCUFormIpAddress(processor._ccuIpAddress);
                if (ccu == null)
                    return;

                switch (processor._state)
                {
                    case State.Alarm:
                        CreateAlarmsGlobals.CreateAlarm(
                            processor._dateTime,
                            processor._ccuIpAddress,
                            ccu.IdCCU,
                            ObjectType.CCU,
                            AlarmType.CCU_HighMemoryLoad,
                            null);

                        break;
                    case State.Normal:
                        CreateAlarmsGlobals.StopAlarm(
                            processor._dateTime,
                            processor._ccuIpAddress,
                            ccu.IdCCU,
                            ObjectType.CCU,
                            AlarmType.CCU_HighMemoryLoad,
                            null);

                        break;
                }
            }
        }

        private static void HandleCCUFilesystemProblem(EventProcessor processor)
        {
            if (processor._parameters == null ||
                processor._parameters.Length != 2 ||
                !(processor._parameters[0] is string) ||
                !(processor._parameters[1] is string))
            {
                return;
            }

            processor._eventOptions.Parameters = 
                new[]
                {
                    processor._state, 
                    processor._parameters[0], 
                    processor._parameters[1]
                };

            EnqueueEventLog(processor);

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(processor._ccuIpAddress);
            if (ccu == null)
                return;

            switch (processor._state)
            {
                case State.Alarm:
                    CreateAlarmsGlobals.CreateAlarm(
                        processor._dateTime,
                        processor._ccuIpAddress,
                        ccu.IdCCU,
                        ObjectType.CCU,
                        AlarmType.CCU_FilesystemProblem,
                        new[]
                        {
                            (string)processor._parameters[0], 
                            (string)processor._parameters[1]
                        });

                    break;
                case State.Normal:
                    CreateAlarmsGlobals.StopAlarm(
                        processor._dateTime,
                        processor._ccuIpAddress,
                        ccu.IdCCU,
                        ObjectType.CCU,
                        AlarmType.CCU_FilesystemProblem,
                        new[]
                        {
                            (string)processor._parameters[0], 
                            (string)processor._parameters[1]
                        });

                    break;
            }
        }

        private static void HandeCCUSdCardNotFound(EventProcessor processor)
        {
            processor._eventOptions.Parameters = new object[] { processor._state };
            EnqueueEventLog(processor);

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(processor._ccuIpAddress);
            if (ccu == null)
                return;

            switch (processor._state)
            {
                case State.Alarm:
                    CreateAlarmsGlobals.CreateAlarm(
                        processor._dateTime,
                        processor._ccuIpAddress,
                        ccu.IdCCU,
                        ObjectType.CCU,
                        AlarmType.CCU_SdCardNotFound);

                    break;

                case State.Normal:
                    CreateAlarmsGlobals.StopAlarm(
                        processor._dateTime,
                        processor._ccuIpAddress,
                        ccu.IdCCU,
                        ObjectType.CCU,
                        AlarmType.CCU_SdCardNotFound);

                    break;
            }
        }

        private static void HandleAlarmAreaBoughtTimeChanged(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 5
                    || !(processor._parameters[0] is Guid)  //Guid card reader
                    || !(processor._parameters[1] is Guid)  //Guid card or person
                    || !(processor._parameters[2] is Guid)  //Guid login
                    || !(processor._parameters[3] is int)   //int used time
                    || !(processor._parameters[4] is int))  //int remaining time
                return;

            var guidLogin = (Guid)processor._parameters[2];

            var stringLogin = string.Empty;

            if (guidLogin != Guid.Empty)
            {
                var temp = Logins.Singleton.GetById(guidLogin);
                if (temp != null)
                    stringLogin = temp.Username;
            }

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaBoughtTimeChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                typeof(AlarmAreaTimeBuyingHandler),
                processor._objectGuid,          //Guid alarm area
                stringLogin,                    //string idLogin
                processor._parameters[3],       //int used   
                processor._parameters[4]);      //int remaining
     
            EnqueueEventLog(processor);
        }

        private static void HandleAlarmAreaBoughtTimeExpired(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 2
                    || !(processor._parameters[0] is int)
                    || !(processor._parameters[1] is int))
                return;

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                CCUCallbackRunner.RunAlarmAreaBoughtTimeExpired,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                typeof(AlarmAreaTimeBuyingHandler),
                processor._objectGuid,          //Guid alarm area
                processor._parameters[0],       //int used time
                processor._parameters[1]);      //int total bought time

            EnqueueEventLog(processor);
        }

        private static void HandleAlarmAreaTimeBuyingFailed(EventProcessor processor)
        {
            if (processor._parameters == null ||
                    processor._parameters.Length != 6
                    || !(processor._parameters[0] is byte)      //byte reason
                    || !(processor._parameters[1] is Guid)      //Guid guidLogin
                    || !(processor._parameters[2] is Guid)      //Guid guidPerson
                    || !(processor._parameters[3] is int)       //int timeToBuy
                    || !(processor._parameters[4] is int)       //int remainingTime
                    || !(processor._parameters[5] is Guid[]))   //Guid[] sources
                return;

            CCUConfigurationHandler.Singleton.AlarmAreaTimeBuyingFailed(
                processor._objectGuid, 
                (Guid)processor._parameters[1],
                (byte)processor._parameters[0],
                (int)processor._parameters[3],
                (int)processor._parameters[4]);

            EnqueueEventLog(processor);
        }

        private static void HandleAntiPassBackZoneCardEntered(EventProcessor processor)
        {
            if (processor._parameters == null ||
                processor._parameters.Length != 3 ||
                !(processor._parameters[0] is Guid) ||  // guidCard
                !(processor._parameters[1] is Guid) ||  // guidEntryCardReader
                !(processor._parameters[2] is bool))    // accessInterrupted
            {
                return;
            }

            AntiPassBackZones.Singleton.OnCardEntered(
                processor._objectGuid,
                processor._dateTime,
                (Guid)processor._parameters[0],
                (Guid)processor._parameters[1],
                (bool)processor._parameters[2]);

            EnqueueEventLog(processor);
        }

        private static void HandleAntiPassBackZoneCardExited(EventProcessor processor)
        {
            if (processor._parameters == null ||
                processor._parameters.Length != 5 ||
                !(processor._parameters[0] is Guid) ||      // guidCard
                !(processor._parameters[1] is Guid) ||      // guidEntryCardReader
                !(processor._parameters[2] is bool) ||      // accessInterrupted
                !(processor._parameters[3] is DateTime) ||  // entryDateTime
                !(processor._parameters[4] is Guid))        // guidExitCardReader
            {
                return;
            }

            AntiPassBackZones.Singleton.OnCardExitedOrTimedOut(
                processor._objectGuid,
                (Guid)processor._parameters[0]);

            EnqueueEventLog(processor);
        }

        private static void HandleAntiPassBackZoneCardTimedOut(EventProcessor processor)
        {
            if (processor._parameters == null ||
                processor._parameters.Length != 4 ||
                !(processor._parameters[0] is Guid) ||      // guidCard
                !(processor._parameters[1] is Guid) ||      // guidEntryCardReader
                !(processor._parameters[2] is bool) ||      // accessInterrupted
                !(processor._parameters[3] is DateTime))    // entryDateTime
            {
                return;
            }

            AntiPassBackZones.Singleton.OnCardExitedOrTimedOut(
                processor._objectGuid,
                (Guid)processor._parameters[0]);

            EnqueueEventLog(processor);
        }
    }
}
