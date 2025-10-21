using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Alarms;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(570)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventForAlarm : EventParameters
    {
        private bool AlarmOccured { get; set; }
        private Guid IdAlarm { get; set; }
        private DateTime CreatedDateTime { get; set; }
        private AlarmState AlarmState { get; set; }
        private bool IsAcknowledged { get; set; }
        private bool IsBlocked { get; set; }
        private IdAndObjectType AlarmObject { get; set; }

        protected EventForAlarm(
            UInt64 eventId,
            bool alarmOccured,
            Guid idAlarm,
            DateTime createdDateTime,
            AlarmState alarmState,
            bool isAknowledged,
            bool isBlocked,
            IdAndObjectType alarmObject)
            : base(
                eventId,
                EventType.AlarmOccuredOrChanged)
        {
            AlarmOccured = alarmOccured;
            IdAlarm = idAlarm;
            CreatedDateTime = createdDateTime;
            AlarmState = alarmState;
            IsAcknowledged = isAknowledged;
            IsBlocked = isBlocked;
            AlarmObject = alarmObject;
        }

        protected EventForAlarm()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {

        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            if (NCASServer.Singleton.GetAlarmsQueue().IsAlarmBlocked(
                ccuSettings.GuidCCU,
                IdAlarm))
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string name;
            IEnumerable<IdAndObjectType> relatedObjects;

            GetAlarmParameters(
                ccuSettings.GuidCCU,
                AlarmObject,
                out name,
                out relatedObjects);

            CgpServer.Singleton.CreateAlarmEventlog(
                AlarmOccured,
                DateTime,
                AlarmState,
                IsAcknowledged,
                IsBlocked,
                false,
                name,
                ccuSettings.CcuName,
                ccuSettings.CCUEvents.ThisAssemblyName,
                relatedObjects,
                out eventlog,
                out eventSources,
                out eventlogParameters);

            return true;
        }

        protected abstract void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects);
    }

    [LwSerialize(571)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedInputAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedInputAlarm(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedInputAlarm()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var input = Inputs.Singleton.GetById(alarmObject.Id);

            if (input == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = InputServerAlarm.GetName(input);

            relatedObjects = InputServerAlarm.GetRelatedObjects(
                idCcu,
                input);
        }
    }

    [LwSerialize(572)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedInputTamper : EventForAlarm
    {
        public EventAlarmOccuredOrChangedInputTamper(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedInputTamper()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var input = Inputs.Singleton.GetById(alarmObject.Id);

            if (input == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = InputTamperServerAlarm.GetName(input);

            relatedObjects = InputTamperServerAlarm.GetRelatedObjects(
                idCcu,
                input);
        }
    }

    [LwSerialize(573)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedOutputAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedOutputAlarm(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedOutputAlarm()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var output = Outputs.Singleton.GetById(alarmObject.Id);

            if (output == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = OutputServerAlarm.GetName(output);

            relatedObjects = OutputServerAlarm.GetRelatedObjects(
                idCcu,
                output);
        }
    }

    [LwSerialize(574)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedAlarmAreaAAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedAlarmAreaAAlarm(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedAlarmAreaAAlarm()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var alarmArea = AlarmAreas.Singleton.GetById(alarmObject.Id);

            if (alarmArea == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = AlarmAreaAServerAlarm.GetName(alarmArea);

            relatedObjects = AlarmAreaAServerAlarm.GetRelatedObjects(
                idCcu,
                alarmArea);
        }
    }

    [LwSerialize(575)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedAlarmAreaAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedAlarmAreaAlarm(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedAlarmAreaAlarm()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var alarmArea = AlarmAreas.Singleton.GetById(alarmObject.Id);

            if (alarmArea == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = AlarmAreaServerAlarm.GetName(alarmArea);

            relatedObjects = AlarmAreaServerAlarm.GetRelatedObjects(
                idCcu,
                alarmArea);
        }
    }

    [LwSerialize(576)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDcuOfflineAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDcuOfflineAlarm(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedDcuOfflineAlarm()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var dcu = DCUs.Singleton.GetById(alarmObject.Id);

            if (dcu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = DcuOfflineServerAlarm.GetName(dcu);

            relatedObjects = DcuOfflineServerAlarm.GetRelatedObjects(
                idCcu,
                dcu);
        }
    }

    [LwSerialize(577)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrOfflineAlarm : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrOfflineAlarm(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCrOfflineAlarm()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrOfflineServerAlarm.GetName(cardReader);

            relatedObjects = CrOfflineServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader);
        }
    }

    [LwSerialize(578)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed : EventForAlarm
    {
        public EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var iTableOrm = CgpServerRemotingProvider.Singleton.GetTableOrmForObjectType(alarmObject.ObjectType);

            if (iTableOrm == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            var ormObject = iTableOrm.GetObjectById(alarmObject.Id);

            if (ormObject == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = IccuSendingOfObjectStateFailedAlarm.GetName(ormObject);

            relatedObjects = IccuSendingOfObjectStateFailedAlarm.GetRelatedObjects(
                idCcu,
                ormObject);
        }
    }

    [LwSerialize(579)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedIccuPortAlreadyUsed : EventForAlarm
    {
        public EventAlarmOccuredOrChangedIccuPortAlreadyUsed(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedIccuPortAlreadyUsed()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = IccuPortAlreadyUsedAlarm.GetName(ccu);

            relatedObjects = IccuPortAlreadyUsedAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(580)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuSdCardNotFound : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuSdCardNotFound(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuSdCardNotFound()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuSdCardNotFoundAlarm.GetName(ccu);

            relatedObjects = CcuSdCardNotFoundAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(581)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuFileSystemProblem : EventForAlarm
    {
        public string FileName { get; private set; }

        protected EventAlarmOccuredOrChangedCcuFileSystemProblem(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCcuFileSystemProblem()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuFileSystemProblemAlarm.GetName(
                ccu,
                FileName);

            relatedObjects = CcuFileSystemProblemAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(582)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuHighMemoryLoad : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuHighMemoryLoad(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuHighMemoryLoad()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuHighMemoryLoadAlarm.GetName(ccu);

            relatedObjects = CcuHighMemoryLoadAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(583)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuCoprocessorFailure : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuCoprocessorFailure(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuCoprocessorFailure()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuCoprocessorFailureAlarm.GetName(ccu);

            relatedObjects = CcuCoprocessorFailureAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(584)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuTamper : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuTamper(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuTamper()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuTamperServerAlarm.GetName(ccu);

            relatedObjects = CcuTamperServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(585)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDcuTamper : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDcuTamper(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedDcuTamper()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var dcu = DCUs.Singleton.GetById(alarmObject.Id);

            if (dcu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = DcuTamperServerAlarm.GetName(dcu);

            relatedObjects = DcuTamperServerAlarm.GetRelatedObjects(
                idCcu,
                dcu);
        }
    }

    [LwSerialize(586)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrTamper : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrTamper(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCrTamper()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrTamperServerAlarm.GetName(cardReader);

            relatedObjects = CrTamperServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader);
        }
    }

    [LwSerialize(587)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuPrimaryPowerMissing : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuPrimaryPowerMissing(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuPrimaryPowerMissing()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuPrimaryPowerMissingServerAlarm.GetName(ccu);

            relatedObjects = CcuPrimaryPowerMissingServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(588)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuBatteryLow : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuBatteryLow(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuBatteryLow()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuBatteryLowServerAlarm.GetName(ccu);

            relatedObjects = CcuBatteryLowServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(589)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuExtFuse : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuExtFuse(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuExtFuse()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuExtFuseServerAlarm.GetName(ccu);

            relatedObjects = CcuExtFuseServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(590)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDeIntrusion : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDeIntrusion(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedDeIntrusion()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(alarmObject.Id);

            if (doorEnvironment == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = DeIntrusionServerAlarm.GetName(doorEnvironment);

            relatedObjects = DeIntrusionServerAlarm.GetRelatedObjects(
                idCcu,
                doorEnvironment);
        }
    }

    [LwSerialize(591)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDeDoorAjar : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDeDoorAjar(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedDeDoorAjar()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(alarmObject.Id);

            if (doorEnvironment == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = DeDoorAjarServerAlarm.GetName(doorEnvironment);

            relatedObjects = DeDoorAjarServerAlarm.GetRelatedObjects(
                idCcu,
                doorEnvironment);
        }
    }

    [LwSerialize(592)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedDeSabotage : EventForAlarm
    {
        public EventAlarmOccuredOrChangedDeSabotage(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedDeSabotage()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(alarmObject.Id);

            if (doorEnvironment == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = DeSabotageServerAlarm.GetName(doorEnvironment);

            relatedObjects = DeSabotageServerAlarm.GetRelatedObjects(
                idCcu,
                doorEnvironment);
        }
    }

    [LwSerialize(593)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventForAlarmsWithCard : EventForAlarm
    {
        public Guid IdCard { get; private set; }

        protected EventForAlarmsWithCard(
            UInt64 eventId,
            bool alarmOccured,
            Guid idAlarm,
            DateTime createdDateTime,
            AlarmState alarmState,
            bool isAknowledged,
            bool isBlocked,
            IdAndObjectType alarmObject,
            IEnumerable<IdAndObjectType> extendedObjects)
            : base(
                eventId,
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

        protected EventForAlarmsWithCard()
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
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCrAccessDeniedWithCard()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            var card = Cards.Singleton.GetById(IdCard);

            if (cardReader == null
                || card == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrAccessDeniedServerAlarm.GetName(cardReader);

            relatedObjects = CrAccessDeniedServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader,
                card);
        }
    }

    [LwSerialize(595)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrUnknownCard : EventForAlarm
    {
        public string FullCardNumber { get; private set; }

        public EventAlarmOccuredOrChangedCrUnknownCard(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCrUnknownCard()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrUnknownCardServerAlarm.GetName(FullCardNumber);

            relatedObjects = CrUnknownCardServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader,
                Cards.Singleton.GetCardByFullNumber(
                    FullCardNumber));
        }
    }

    [LwSerialize(596)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrCardBlockedOrInactive : EventForAlarmsWithCard
    {
        public EventAlarmOccuredOrChangedCrCardBlockedOrInactive(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCrCardBlockedOrInactive()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            var card = Cards.Singleton.GetById(IdCard);

            if (cardReader == null
                || card == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrCardBlockedOrInactiveServerAlarm.GetName(cardReader);

            relatedObjects = CrCardBlockedOrInactiveServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader,
                card);
        }
    }

    [LwSerialize(597)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidPin : EventForAlarmsWithCard
    {
        public EventAlarmOccuredOrChangedCrInvalidPin(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCrInvalidPin()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            var card = Cards.Singleton.GetById(IdCard);

            if (cardReader == null
                || card == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrInvalidPinServerAlarm.GetName(cardReader);

            relatedObjects = CrInvalidPinServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader,
                card);
        }
    }

    [LwSerialize(598)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidCode : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrInvalidCode(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCrInvalidCode()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrInvalidCodeServerAlarm.GetName(cardReader);

            relatedObjects = CrInvalidCodeServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader);
        }
    }

    [LwSerialize(599)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrAccessPermittedWithCard : EventForAlarmsWithCard
    {
        public EventAlarmOccuredOrChangedCrAccessPermittedWithCard(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCrAccessPermittedWithCard()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrAccessPermittedServerAlarm.GetName(cardReader);

            relatedObjects = CrAccessPermittedServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader,
                Cards.Singleton.GetById(
                    IdCard));
        }
    }

    [LwSerialize(600)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidEmergency : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrInvalidEmergency(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCrInvalidEmergency()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrInvalidEmergencyCodeServerAlarm.GetName(cardReader);

            relatedObjects = CrInvalidEmergencyCodeServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader);
        }
    }

    [LwSerialize(601)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsOutputFuse : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsOutputFuse(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuUpsOutputFuse()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuUpsOutputFuseServerAlarm.GetName(ccu);

            relatedObjects = CcuUpsOutputFuseServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(602)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsBatteryFault : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsBatteryFault(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuUpsBatteryFault()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuUpsBatteryFaultServerAlarm.GetName(ccu);

            relatedObjects = CcuUpsBatteryFaultServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(603)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsBatteryFuse : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsBatteryFuse(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuUpsBatteryFuse()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuUpsBatteryFuseServerAlarm.GetName(ccu);

            relatedObjects = CcuUpsBatteryFuseServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(604)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsOvertemperature : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsOvertemperature(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuUpsOvertemperature()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuUpsOvertemperatureServerAlarm.GetName(ccu);

            relatedObjects = CcuUpsOvertemperatureServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(605)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuUpsTamperSabotage : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCcuUpsTamperSabotage(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCcuUpsTamperSabotage()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CcuUpsTamperSabotageServerAlarm.GetName(ccu);

            relatedObjects = CcuUpsTamperSabotageServerAlarm.GetRelatedObjects(idCcu);
        }
    }

    [LwSerialize(607)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidGinRetriesLimitReached : EventForAlarm
    {
        public EventAlarmOccuredOrChangedCrInvalidGinRetriesLimitReached(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
                alarmOccured,
                alarm.Id,
                alarm.CreatedDateTime,
                alarm.AlarmState,
                alarm.IsAcknowledged,
                alarm.IsBlocked,
                alarm.AlarmKey.AlarmObject)
        {

        }

        protected EventAlarmOccuredOrChangedCrInvalidGinRetriesLimitReached()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrInvalidCodeRetriesLimitReachedServerAlarm.GetName(cardReader);

            relatedObjects = CrInvalidCodeRetriesLimitReachedServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader);
        }
    }

    [LwSerialize(608)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuCatUnreachable : EventForAlarm
    {
        public string AlarmTransmitterIpAddress { get; private set; }

        public EventAlarmOccuredOrChangedCcuCatUnreachable(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCcuCatUnreachable()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var alarmTransmitter = AlarmTransmitters.Singleton.GetAlarmTransmitterByIpAddress(AlarmTransmitterIpAddress);

            name = CcuCatUnreachableServerAlarm.GetName(
                alarmTransmitter != null
                    ? alarmTransmitter.ToString()
                    : AlarmTransmitterIpAddress);

            relatedObjects = CcuCatUnreachableServerAlarm.GetRelatedObjects(
                idCcu,
                alarmTransmitter);
        }
    }

    [LwSerialize(609)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCcuTransferToArcTimedOut : EventForAlarm
    {
        public string AlarmTransmitterIpAddress { get; private set; }
        public string ArcName { get; private set; }

        public EventAlarmOccuredOrChangedCcuTransferToArcTimedOut(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCcuTransferToArcTimedOut()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            name = CcuTransferToArcTimedOutServerAlarm.GetName(ArcName);

            var alarmTransmitter = AlarmTransmitters.Singleton.GetAlarmTransmitterByIpAddress(AlarmTransmitterIpAddress);

            var alarmArc = AlarmArcs.Singleton.GetAlarmArcByName(ArcName);

            relatedObjects = CcuTransferToArcTimedOutServerAlarm.GetRelatedObjects(
                idCcu,
                alarmTransmitter,
                alarmArc);
        }
    }

    [LwSerialize(610)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed : EventForAlarm
    {
        public IdAndObjectType OnOffObject { get; private set; }

        public EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var alarmArea = AlarmAreas.Singleton.GetById(alarmObject.Id);

            if (alarmArea == null
                || OnOffObject == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = AlarmAreaSetByOnOffObjectFailedServerAlarm.GetName(alarmArea);

            relatedObjects = AlarmAreaSetByOnOffObjectFailedServerAlarm.GetRelatedObjects(
                idCcu,
                alarmArea,
                OnOffObject);
        }
    }

    [LwSerialize(611)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedSensorAlarm : EventForAlarm
    {
        private Guid IdAlarmArea { get; set; }

        private EventAlarmOccuredOrChangedSensorAlarm()
        {
            IdAlarmArea = Guid.Empty;
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var input = Inputs.Singleton.GetById(alarmObject.Id);

            if (input == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = SensorServerAlarm.GetName(
                input,
                IdAlarmArea);

            relatedObjects = SensorServerAlarm.GetRelatedObjects(
                idCcu,
                input,
                IdAlarmArea);
        }
    }

    [LwSerialize(612)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedSensorTamperAlarm : EventForAlarm
    {
        private Guid IdAlarmArea { get; set; }

        private EventAlarmOccuredOrChangedSensorTamperAlarm()
        {
            IdAlarmArea = Guid.Empty;
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var input = Inputs.Singleton.GetById(alarmObject.Id);

            if (input == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = SensorTamperServerAlarm.GetName(
                input,
                IdAlarmArea);

            relatedObjects = SensorTamperServerAlarm.GetRelatedObjects(
                idCcu,
                input,
                IdAlarmArea);
        }
    }

    [LwSerialize(613)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached : EventForAlarm
    {
        public Guid CardReaderId { get; private set; }

        public EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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
                ? (Guid)cardReaderIdAndObjectType.Id
                : Guid.Empty;
        }

        protected EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var card = Cards.Singleton.GetById(alarmObject.Id);

            if (card == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            var cardReader = CardReaders.Singleton.GetById(CardReaderId);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrInvalidPinRetriesLimitReachedServerAlarm.GetName(
                card);

            relatedObjects = CrInvalidPinRetriesLimitReachedServerAlarm.GetRelatedObjects(
                idCcu,
                card,
                cardReader);
        }
    }

    [LwSerialize(614)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventForAlarmsWithPerson : EventForAlarm
    {
        public Guid IdPerson { get; private set; }

        protected EventForAlarmsWithPerson(
            UInt64 eventId,
            bool alarmOccured,
            Guid idAlarm,
            DateTime createdDateTime,
            AlarmState alarmState,
            bool isAknowledged,
            bool isBlocked,
            IdAndObjectType alarmObject,
            IEnumerable<IdAndObjectType> extendedObjects)
            : base(
                eventId,
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

        protected EventForAlarmsWithPerson()
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
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCrAccessDeniedWithPerson()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            var person = Persons.Singleton.GetById(IdPerson);

            if (cardReader == null
                || person == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrAccessDeniedServerAlarm.GetName(cardReader);

            relatedObjects = CrAccessDeniedServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader,
                person);
        }
    }

    [LwSerialize(616)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmOccuredOrChangedCrAccessPermittedWithPerson : EventForAlarmsWithPerson
    {
        public EventAlarmOccuredOrChangedCrAccessPermittedWithPerson(
            UInt64 eventId,
            bool alarmOccured,
            Alarm alarm)
            : base(
                eventId,
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

        protected EventAlarmOccuredOrChangedCrAccessPermittedWithPerson()
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

        protected override void GetAlarmParameters(
            Guid idCcu,
            IdAndObjectType alarmObject,
            out string name,
            out IEnumerable<IdAndObjectType> relatedObjects)
        {
            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                name = string.Empty;
                relatedObjects = null;

                return;
            }

            name = CrAccessPermittedServerAlarm.GetName(cardReader);

            relatedObjects = CrAccessPermittedServerAlarm.GetRelatedObjects(
                idCcu,
                cardReader,
                Persons.Singleton.GetById(
                    IdPerson));
        }
    }
}
