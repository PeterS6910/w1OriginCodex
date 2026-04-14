using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(570)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventForAlarm : EventParameters
    {
        public bool AlarmOccured { get; private set; }
        public Guid IdAlarm { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public AlarmState AlarmState { get; private set; }
        public bool IsAcknowledged { get; private set; }
        public bool IsBlocked { get; private set; }
        public IdAndObjectType AlarmObject { get; private set; }

        public EventForAlarm(
            bool alarmOccured,
            Guid idAlarm,
            DateTime createdDateTime,
            AlarmState alarmState,
            bool isAknowledged,
            bool isBlocked,
            IdAndObjectType alarmObject)
            : base(EventType.AlarmOccuredOrChanged)
        {
            AlarmOccured = alarmOccured;
            IdAlarm = idAlarm;
            CreatedDateTime = createdDateTime;
            AlarmState = alarmState;
            IsAcknowledged = isAknowledged;
            IsBlocked = isBlocked;
            AlarmObject = alarmObject;
        }

        public EventForAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Alarm occured: {0}, Alarm id: {1}, Created date time: {2}, Alarm state: {3}, Is acknowleged {4}, Is blocked {5}",
                    AlarmOccured,
                    IdAlarm,
                    CreatedDateTime,
                    AlarmState,
                    IsAcknowledged,
                    IsBlocked));

            if (AlarmObject != null)
                parameters.Append(
                    string.Format(
                        ", Alarm object type: {0}, Alarm object id: {1}",
                        AlarmObject.ObjectType,
                        AlarmObject.Id));
        }
    }

    [LwSerialize(571)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedInputAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedInputAlarm(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedInputAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.Input_Alarm));
        }
    }

    [LwSerialize(572)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedInputTamper : EventForAlarm
    {
        public EventAlarmOccuredOrChangedInputTamper(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedInputTamper()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.Input_Tamper));
        }
    }

    [LwSerialize(573)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedOutputAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedOutputAlarm(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedOutputAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.Output_Alarm));
        }
    }

    [LwSerialize(574)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedAlarmAreaAAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedAlarmAreaAAlarm(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedAlarmAreaAAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.AlarmArea_AAlarm));
        }
    }

    [LwSerialize(575)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedAlarmAreaAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedAlarmAreaAlarm(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedAlarmAreaAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.AlarmArea_Alarm));
        }
    }

    [LwSerialize(576)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDcuOfflineAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDcuOfflineAlarm(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedDcuOfflineAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.DCU_Offline));
        }
    }

    [LwSerialize(577)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrOfflineAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrOfflineAlarm(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCrOfflineAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_Offline));
        }
    }

    [LwSerialize(578)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed : EventForAlarm
    {
        public EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.ICCU_SendingOfObjectStateFailed));
        }
    }

    [LwSerialize(579)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedIccuPortAlreadyUsed : EventForAlarm
    {
        public EventAlarmOccuredOrChangedIccuPortAlreadyUsed(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedIccuPortAlreadyUsed()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.ICCU_PortAlreadyUsed));
        }
    }

    [LwSerialize(580)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuSdCardNotFound : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuSdCardNotFound(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuSdCardNotFound()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CCU_SdCardNotFound));
        }
    }

    [LwSerialize(581)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuFileSystemProblem : EventForAlarm
    {
        public string FileName { get; private set; }

        public EventAlarmOccuredOrChangedCcuFileSystemProblem(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {
            var parameters = alarm.AlarmKey.Parameters;

            var fileNameAlarmParameter = parameters != null
                ? parameters.FirstOrDefault(
                    alarmParameter =>
                        alarmParameter.TypeParameter == ParameterType.FileName)
                : null;

            FileName = fileNameAlarmParameter != null
                ? fileNameAlarmParameter.Value
                : string.Empty;
        }

        public EventAlarmOccuredOrChangedCcuFileSystemProblem()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", File name: {0}, Alarm type: {1}",
                    FileName,
                    AlarmType.CCU_FilesystemProblem));
        }
    }

    [LwSerialize(582)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuHighMemoryLoad : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuHighMemoryLoad(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuHighMemoryLoad()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CCU_HighMemoryLoad));
        }
    }

    [LwSerialize(583)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuCoprocessorFailure : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuCoprocessorFailure(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuCoprocessorFailure()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CCU_CoprocessorFailure));
        }
    }

    [LwSerialize(584)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuTamper : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuTamper(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuTamper()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CCU_TamperSabotage));
        }
    }

    [LwSerialize(585)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDcuTamper : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDcuTamper(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedDcuTamper()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.DCU_TamperSabotage));
        }
    }

    [LwSerialize(586)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrTamper : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrTamper(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCrTamper()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_TamperSabotage));
        }
    }

    [LwSerialize(587)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuPrimaryPowerMissing : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuPrimaryPowerMissing(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuPrimaryPowerMissing()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CCU_PrimaryPowerMissing));
        }
    }

    [LwSerialize(588)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuBatteryLow : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuBatteryLow(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuBatteryLow()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CCU_BatteryLow));
        }
    }

    [LwSerialize(589)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuExtFuse : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuExtFuse(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuExtFuse()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CCU_ExtFuse));
        }
    }

    [LwSerialize(590)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDeIntrusion : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDeIntrusion(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedDeIntrusion()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.DoorEnvironment_Intrusion));
        }
    }

    [LwSerialize(591)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDeDoorAjar : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDeDoorAjar(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedDeDoorAjar()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.DoorEnvironment_DoorAjar));
        }
    }

    [LwSerialize(592)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDeSabotage : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDeSabotage(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedDeSabotage()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.DoorEnvironment_Sabotage));
        }
    }

    [LwSerialize(593)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventForAlarmsWithCard : EventForAlarm
    {
        public Guid IdCard { get; private set; }

        public EventForAlarmsWithCard(
            bool alarmOccured,
            Guid idAlarm,
            DateTime createdDateTime,
            AlarmState alarmState,
            bool isAknowledged,
            bool isBlocked,
            IdAndObjectType alarmObject,
            IEnumerable<IdAndObjectType> extendedObjects)
            : base(
                alarmOccured,
                idAlarm,
                createdDateTime,
                alarmState,
                isAknowledged,
                isBlocked,
                alarmObject)
        {
            var cardIdAndObjectType = extendedObjects != null
                ? extendedObjects.FirstOrDefault(
                    idAndObjectType =>
                        idAndObjectType.ObjectType == ObjectType.Card)
                : null;

            IdCard = cardIdAndObjectType != null
                ? (Guid) cardIdAndObjectType.Id
                : Guid.Empty;
        }

        public EventForAlarmsWithCard()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}",
                    IdCard));
        }
    }

    [LwSerialize(594)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrAccessDeniedWithCard : EventForAlarmsWithCard
    {
        public EventAlarmOccuredOrChangedCrAccessDeniedWithCard(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject,
                alarm.AlarmKey.ExtendedObjects)
        {

        }

        public EventAlarmOccuredOrChangedCrAccessDeniedWithCard()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_AccessDenied));
        }
    }

    [LwSerialize(595)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrUnknownCard : EventForAlarm
    {
        public string FullCardNumber { get; private set; }

        public EventAlarmOccuredOrChangedCrUnknownCard(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {
            var parameters = alarm.AlarmKey.Parameters;

            var cardNumberAlarmParameter = parameters != null
                ? parameters.FirstOrDefault(
                    alarmParameter =>
                        alarmParameter.TypeParameter == ParameterType.CardNumber)
                : null;

            FullCardNumber = cardNumberAlarmParameter != null
                ? cardNumberAlarmParameter.Value
                : string.Empty;
        }

        public EventAlarmOccuredOrChangedCrUnknownCard()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Full card Number: {0}, Alarm type: {1}",
                    FullCardNumber,
                    AlarmType.CardReader_UnknownCard));
        }
    }

    [LwSerialize(596)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrCardBlockedOrInactive : EventForAlarmsWithCard
    {
        public EventAlarmOccuredOrChangedCrCardBlockedOrInactive(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject,
                alarm.AlarmKey.ExtendedObjects)
        {

        }

        public EventAlarmOccuredOrChangedCrCardBlockedOrInactive()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_CardBlockedOrInactive));
        }
    }

    [LwSerialize(597)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidPin : EventForAlarmsWithCard
    {
        public EventAlarmOccuredOrChangedCrInvalidPin(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject,
                alarm.AlarmKey.ExtendedObjects)
        {

        }

        public EventAlarmOccuredOrChangedCrInvalidPin()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_InvalidPIN));
        }
    }

    [LwSerialize(598)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidCode : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrInvalidCode(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCrInvalidCode()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_InvalidCode));
        }
    }

    [LwSerialize(599)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrAccessPermittedWithCard : EventForAlarmsWithCard
    {
        public EventAlarmOccuredOrChangedCrAccessPermittedWithCard(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject,
                alarm.AlarmKey.ExtendedObjects)
        {

        }

        public EventAlarmOccuredOrChangedCrAccessPermittedWithCard()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_AccessPermitted));
        }
    }

    [LwSerialize(600)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidEmergency : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrInvalidEmergency(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCrInvalidEmergency()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_InvalidEmergencyCode));
        }
    }

    [LwSerialize(601)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsOutputFuse : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsOutputFuse(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuUpsOutputFuse()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.Ccu_Ups_OutputFuse));
        }
    }

    [LwSerialize(602)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsBatteryFault : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsBatteryFault(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuUpsBatteryFault()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.Ccu_Ups_BatteryFault));
        }
    }

    [LwSerialize(603)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsBatteryFuse : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsBatteryFuse(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuUpsBatteryFuse()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.Ccu_Ups_BatteryFuse));
        }
    }

    [LwSerialize(604)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsOvertemperature : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsOvertemperature(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuUpsOvertemperature()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.Ccu_Ups_Overtemperature));
        }
    }

    [LwSerialize(605)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsTamperSabotage : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsTamperSabotage(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCcuUpsTamperSabotage()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.Ccu_Ups_TamperSabotage));
        }
    }

    [LwSerialize(607)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidGinRetriesLimitReached : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrInvalidGinRetriesLimitReached(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        public EventAlarmOccuredOrChangedCrInvalidGinRetriesLimitReached()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached));
        }
    }

    [LwSerialize(608)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuCatUnreachable : EventForAlarm
    {
        public string AlarmTransmitterIpAddress { get; private set; }

        public EventAlarmOccuredOrChangedCcuCatUnreachable(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {
            var parameters = alarm.AlarmKey.Parameters;

            var alarmTransmitterIpAddressParameter = parameters != null
                ? parameters.FirstOrDefault(
                    alarmParameter =>
                        alarmParameter.TypeParameter == ParameterType.CatIpAddress)
                : null;

            AlarmTransmitterIpAddress = alarmTransmitterIpAddressParameter != null
                ? alarmTransmitterIpAddressParameter.Value
                : string.Empty;
        }

        public EventAlarmOccuredOrChangedCcuCatUnreachable()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm transmitter IP address: {0}, Alarm type: {1}",
                    AlarmTransmitterIpAddress,
                    AlarmType.Ccu_CatUnreachable));
        }
    }

    [LwSerialize(609)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuTransferToArcTimedOut : EventForAlarm
    {
        public string AlarmTransmitterIpAddress { get; private set; }
        public string ArcName { get; private set; }

        public EventAlarmOccuredOrChangedCcuTransferToArcTimedOut(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {
            var parameters = alarm.AlarmKey.Parameters;

            var alarmTransmitterIpAddressParameter = parameters != null
                ? parameters.FirstOrDefault(
                    alarmParameter =>
                        alarmParameter.TypeParameter == ParameterType.CatIpAddress)
                : null;

            AlarmTransmitterIpAddress = alarmTransmitterIpAddressParameter != null
                ? alarmTransmitterIpAddressParameter.Value
                : string.Empty;

            var arcNameParameter = parameters != null
                ? parameters.FirstOrDefault(
                    alarmParameter =>
                        alarmParameter.TypeParameter == ParameterType.ArcName)
                : null;

            ArcName = arcNameParameter != null
                ? arcNameParameter.Value
                : string.Empty;
        }

        public EventAlarmOccuredOrChangedCcuTransferToArcTimedOut()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm transmitter IP address: {0}, Alarm type: {1}, ARC name {2}",
                    AlarmTransmitterIpAddress,
                    AlarmType.Ccu_TransferToArcTimedOut,
                    ArcName));
        }
    }

    [LwSerialize(610)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed : EventForAlarm
    {
        public IdAndObjectType OnOffObject { get; private set; }

        public EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {
            var extendedObjects = alarm.AlarmKey.ExtendedObjects;

            if (extendedObjects != null)
                OnOffObject = extendedObjects.First();
        }

        public EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            if (OnOffObject == null)
                return;

            parameters.Append(
                string.Format(
                    "Alarm type: {0}, On/Off object type: {1}, On/Off object ID: {2}",
                    AlarmType.AlarmArea_SetByOnOffObjectFailed,
                    OnOffObject.ObjectType,
                    OnOffObject.Id));
        }
    }

    [LwSerialize(611)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedSensorAlarm : EventForAlarm
    {
        private Guid IdAlarmArea { get; set; }

        public EventAlarmOccuredOrChangedSensorAlarm(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {
            var extendedObjects = alarm.AlarmKey.ExtendedObjects;

            var alarmAreaIdAndObjectType = extendedObjects.FirstOrDefault();

            IdAlarmArea = alarmAreaIdAndObjectType != null
                ? (Guid) alarmAreaIdAndObjectType.Id
                : Guid.Empty;
        }

        public EventAlarmOccuredOrChangedSensorAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}, Alarm area : {1}",
                    AlarmType.Sensor_Alarm,
                    IdAlarmArea));
        }
    }

    [LwSerialize(612)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedSensorTamperAlarm : EventForAlarm
    {
        private Guid IdAlarmArea { get; set; }

        public EventAlarmOccuredOrChangedSensorTamperAlarm(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {
            var extendedObjects = alarm.AlarmKey.ExtendedObjects;

            var alarmAreaIdAndObjectType = extendedObjects.FirstOrDefault();

            IdAlarmArea = alarmAreaIdAndObjectType != null
                ? (Guid)alarmAreaIdAndObjectType.Id
                : Guid.Empty;
        }

        public EventAlarmOccuredOrChangedSensorTamperAlarm()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}, Alarm area : {1}",
                    AlarmType.Sensor_Tamper_Alarm,
                    IdAlarmArea));
        }
    }

    [LwSerialize(613)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached : EventForAlarm
    {
        public Guid CardReaderId { get; private set; }

        public EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {
            var extendedObjects = alarm.AlarmKey.ExtendedObjects;

            var cardReaderIdAndObjectType = extendedObjects != null
                ? extendedObjects.FirstOrDefault(idAndObjectType => idAndObjectType.ObjectType == ObjectType.CardReader)
                : null;

            CardReaderId = cardReaderIdAndObjectType != null
                ? (Guid) cardReaderIdAndObjectType.Id
                : Guid.Empty;
        }

        public EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}, Card reader: {1}",
                    AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached,
                    CardReaderId));
        }
    }

    [LwSerialize(614)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventForAlarmsWithPerson : EventForAlarm
    {
        public Guid IdPerson { get; private set; }

        public EventForAlarmsWithPerson(
            bool alarmOccured,
            Guid idAlarm,
            DateTime createdDateTime,
            AlarmState alarmState,
            bool isAknowledged,
            bool isBlocked,
            IdAndObjectType alarmObject,
            IEnumerable<IdAndObjectType> extendedObjects)
            : base(
                alarmOccured,
                idAlarm,
                createdDateTime,
                alarmState,
                isAknowledged,
                isBlocked,
                alarmObject)
        {
            var personIdAndObjectType = extendedObjects != null
                ? extendedObjects.FirstOrDefault(
                    idAndObjectType =>
                        idAndObjectType.ObjectType == ObjectType.Person)
                : null;

            IdPerson = personIdAndObjectType != null
                ? (Guid)personIdAndObjectType.Id
                : Guid.Empty;
        }

        public EventForAlarmsWithPerson()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Person: {0}",
                    IdPerson));
        }
    }

    [LwSerialize(615)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrAccessDeniedWithPerson : EventForAlarmsWithPerson
    {
        public EventAlarmOccuredOrChangedCrAccessDeniedWithPerson(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject,
                alarm.AlarmKey.ExtendedObjects)
        {

        }

        public EventAlarmOccuredOrChangedCrAccessDeniedWithPerson()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_AccessDenied));
        }
    }

    [LwSerialize(616)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrAccessPermittedWithPerson : EventForAlarmsWithPerson
    {
        public EventAlarmOccuredOrChangedCrAccessPermittedWithPerson(
            bool alarmOccured,
            Alarm alarm)
            : base(
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject,
                alarm.AlarmKey.ExtendedObjects)
        {

        }

        public EventAlarmOccuredOrChangedCrAccessPermittedWithPerson()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm type: {0}",
                    AlarmType.CardReader_AccessPermitted));
        }
    }

    public static class TestAlarmEvents
    {
        public static void EnqueueTestEvents(
            Guid idCcu,
            Guid idInput,
            Guid idAlarmArea,
            Guid idDcu,
            Guid idDoorEnvironment,
            Guid idCardReader,
            ICard card)
        {
            Events.ProcessEvent(new EventAlarmOccuredOrChangedInputAlarm(
                true,
                new InputAlarm(
                    idInput)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedInputTamper(
                true,
                new InputTamperAlarm(
                    idInput)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedOutputAlarm(
                true,
                new OutputAlarm(
                    idInput)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedAlarmAreaAAlarm(
                true,
                new AlarmAreaAAlarm(
                    idAlarmArea,
                    5)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedAlarmAreaAlarm(
                true,
                new AlarmAreaAlarm(
                    idAlarmArea)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedDcuOfflineAlarm(
                true,
                new DcuOfflineAlarm(
                    idDcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrOfflineAlarm(
                true,
                new CrOfflineAlarm(
                    idCardReader)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed(
                true,
                new IccuSendingOfObjectStateFailedAlarm(
                    idInput,
                    ObjectType.Input)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedIccuPortAlreadyUsed(
                true,
                new IccuPortAlreadyUsedAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuSdCardNotFound(
                true,
                new CcuSdCardNotFoundAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuFileSystemProblem(
                true,
                new CcuFileSystemProblemAlarm(
                    idCcu,
                    "Test",
                    "Write")));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuHighMemoryLoad(
                true,
                new CcuHighMemoryLoadAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuCoprocessorFailure(
                true,
                new CcuCoprocessorFailureAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuTamper(
                true,
                new CcuTamperAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedDcuTamper(
                true,
                new DcuTamperAlarm(
                    idDcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrTamper(
                true,
                new CrTamperAlarm(
                    idCardReader)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuPrimaryPowerMissing(
                true,
                new CcuPrimaryPowerMissingAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuBatteryLow(
                true,
                new CcuBatteryLowAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuExtFuse(
                true,
                new CcuExtFuseAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedDeIntrusion(
                true,
                new DeIntrusionAlarm(
                    idDoorEnvironment)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedDeDoorAjar(
                true,
                new DeDoorAjarAlarm(
                    idDoorEnvironment)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedDeSabotage(
                true,
                new DeSabotageAlarm(
                    idDoorEnvironment)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrAccessDeniedWithCard(
                true,
                new CrAccessDeniedAlarm(
                    idCardReader,
                    new TestDoorEnvironmentEvents.CardAccessData(card))));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrUnknownCard(
                true,
                new CrUnknownCardAlarm(
                    idCardReader,
                    "0123456789012")));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrCardBlockedOrInactive(
                true,
                new CrCardBlockedOrInactiveAlarm(
                    idCardReader,
                    card.IdCard)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrInvalidPin(
                true,
                new CrInvalidPinAlarm(
                    idCardReader,
                    card.IdCard)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrInvalidCode(
                true,
                new CrInvalidCodeAlarm(
                    idCardReader)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrAccessPermittedWithCard(
                true,
                new CrAccessPermittedAlarm(
                    idCardReader,
                    new TestDoorEnvironmentEvents.CardAccessData(card))));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrInvalidEmergency(
                true,
                new CrInvalidEmergencyCodeAlarm(
                    idCardReader)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuUpsOutputFuse(
                true,
                new CcuUpsOutputFuseAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuUpsBatteryFault(
                true,
                new CcuUpsBatteryFaultAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuUpsBatteryFuse(
                true,
                new CcuUpsBatteryFuseAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuUpsOvertemperature(
                true,
                new CcuUpsOvertemperatureAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuUpsTamperSabotage(
                true,
                new CcuUpsTamperSabotageAlarm(
                    idCcu)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrInvalidGinRetriesLimitReached(
                true,
                new CrInvalidCodeRetriesLimitReached(
                    idCardReader)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuCatUnreachable(
                true,
                new CcuCatUnreachableAlarm(
                    idCcu,
                    "10.0.0.1",
                    AlarmState.Alarm)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCcuTransferToArcTimedOut(
                true,
                new CcuTransferToArcTimedOutAlarm(
                    idCcu,
                    "10.0.0.1",
                    "Pokus")));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed(
                true,
                new AlarmAreaSetByOnOffObjectFailedAlarm(
                    idAlarmArea,
                    new IdAndObjectType(
                        idInput,
                        ObjectType.Input))));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedSensorAlarm(
                true,
                new SensorAlarm(
                    idInput,
                    idAlarmArea)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedSensorTamperAlarm(
                true,
                new SensorTamperAlarm(
                    idInput,
                    idAlarmArea)));

            Events.ProcessEvent(new EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached(
                true,
                new CrInvalidPinRetriesLimitReached(
                    card.IdCard,
                    idCardReader)));
        }
    }
}