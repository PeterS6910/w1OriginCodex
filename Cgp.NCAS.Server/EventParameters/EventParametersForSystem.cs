using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(520)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventRunMethodFailed : EventParameters
    {
        public string MethodName { get; private set; }
        public string Exception { get; private set; }

        public EventRunMethodFailed(
            UInt64 eventId,
            string methodName,
            string exception)
            : base(
                eventId,
                EventType.RunMethodFailed)
        {
            MethodName = methodName;
            Exception = exception;
        }

        protected EventRunMethodFailed()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Method name: {0}, Exception: {1}",
                    MethodName,
                    Exception));
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPERUNMETHODFAILED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format(
                    "Run method {0} on the ccu failed with exception: {1}",
                    MethodName,
                    Exception),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPECCU,
                ccuSettings.IPAddressString,
                EventlogParameter.TYPE_CCU_VERSION,
                VersionsInfo.CcuVersion,
                EventlogParameter.TYPE_CE_VERSION,
                VersionsInfo.CeVersion,
                EventlogParameter.TYPE_SERVER_VERSION,
                VersionsInfo.ServerVersion);
        }
    }

    [LwSerialize(521)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuMemoryLoadStateChanged : EventParametersWithState
    {
        public EventCcuMemoryLoadStateChanged(
            UInt64 eventId,
            bool isHighCcuMemoryLoad)
            : base(
                eventId,
                EventType.CCUMemoryLoadStateChanged,
                isHighCcuMemoryLoad
                    ? State.Alarm
                    : State.Normal)
        {

        }

        protected EventCcuMemoryLoadStateChanged()
        {
            
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description = string.Empty;
            var memoryLoad = string.Empty;
            switch (State)
            {
                case State.Alarm:
                    description = string.Format("Memory load the CCU is high - above {0}%", NCASConstants.CCU_MEMORY_LOAD_TRESHOLD);
                    memoryLoad = "High";
                    break;
                case State.Normal:
                    description = string.Format("Memory load the CCU is low - below {0}%", NCASConstants.CCU_MEMORY_LOAD_TRESHOLD);
                    memoryLoad = "Low";
                    break;
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_MEMORY_LOAD,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPE_CCU_MEMORY_LOAD,
                memoryLoad);
        }
    }

    [LwSerialize(522)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventExceptionOccured : EventParameters
    {
        public string ExceptionMessage { get; private set; }
        public string StackTrace { get; private set; }
        public int ThreadId { get; private set; }

        public EventExceptionOccured(
            UInt64 eventId,
            DateTime dateTime,
            Exception exception,
            int threadId)
            : base(
                eventId,
                EventType.ExceptionOccurred,
                dateTime)
        {
            ExceptionMessage = exception.ToString();
            StackTrace = exception.StackTrace;
            ThreadId = threadId;
        }

        protected EventExceptionOccured()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Exception: {0}, StackTrace: {1}, ThreadId: {2}",
                    ExceptionMessage,
                    StackTrace,
                    ThreadId));
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_EXCEPTION_OCCURRED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format(
                    "CCU exception occurred : {0}",
                    ExceptionMessage),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPE_STACKTRACE,
                StackTrace ?? "",
                EventlogParameter.TYPE_THREAD_ID,
                ThreadId.ToString("x8"),
                EventlogParameter.TYPE_CCU_VERSION,
                VersionsInfo.CcuVersion,
                EventlogParameter.TYPE_CE_VERSION,
                VersionsInfo.CeVersion,
                EventlogParameter.TYPE_SERVER_VERSION,
                VersionsInfo.ServerVersion);
        }
    }

    [LwSerialize(523)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuFilesystemProblem : EventParametersWithState
    {
        public string FileName { get; private set; }
        public string FileOperation { get; private set; }

        public EventCcuFilesystemProblem(
            UInt64 eventId,
            Exception exception,
            string fileName,
            string fileOperation)
            : base(
                eventId,
                EventType.CCUFilesystemProblem,
                exception != null
                    ? State.Alarm
                    : State.Normal)
        {
            FileName = fileName;
            FileOperation = fileOperation;
        }

        protected EventCcuFilesystemProblem()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", File name: {0}, File operation: {1}",
                    FileName,
                    FileOperation));
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description = string.Empty;
            switch (State)
            {
                case State.Alarm:
                    description = "CCU filesystem problem occured, ";
                    break;
                case State.Normal:
                    description = "CCU filesystem problem expired, ";
                    break;
            }

            description += string.Format(
                "file name: {0}, file operation: {1}",
                FileName,
                FileOperation);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_CCU_FILESYSTEM_PROBLEM,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    description,
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPE_FILE_NAME,
                    FileName,
                    EventlogParameter.TYPE_FILE_OPERATION,
                    FileOperation,
                    EventlogParameter.TYPE_CCU_VERSION,
                    VersionsInfo.CcuVersion,
                    EventlogParameter.TYPE_CE_VERSION,
                    VersionsInfo.CeVersion,
                    EventlogParameter.TYPE_SERVER_VERSION,
                    VersionsInfo.ServerVersion);
        }
    }

    [LwSerialize(524)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuSdCardNotFound : EventParametersWithState
    {
        public EventCcuSdCardNotFound(
            UInt64 eventId,
            bool sdCardPresent)
            : base(
                eventId,
                EventType.CCUSdCardNotFound,
                sdCardPresent
                    ? State.Normal
                    : State.Alarm)
        {

        }

        protected EventCcuSdCardNotFound()
        {
            
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
            if (State != State.Alarm)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_CCU_SD_CARD_NOT_FOUND,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    "CCU SD card not found",
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPE_CCU_VERSION,
                    VersionsInfo.CcuVersion,
                    EventlogParameter.TYPE_CE_VERSION,
                    VersionsInfo.CeVersion,
                    EventlogParameter.TYPE_SERVER_VERSION,
                    VersionsInfo.ServerVersion);
        }
    }

    [LwSerialize(525)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventObjectDeserializeFailed : EventParametersWithObjectId
    {
        public ObjectType ObjectType { get; private set; }
        public string Message { get; private set; }
        public string ExceptionMessage { get; private set; }

        public EventObjectDeserializeFailed(
            UInt64 eventId,
            Guid idObject,
            ObjectType objectType,
            string message,
            string exceptionMessage)
            : base(
                eventId,
                EventType.ObjectDeserializeFailed,
                idObject)
        {
            ObjectType = objectType;
            Message = message;
            ExceptionMessage = exceptionMessage;
        }

        protected EventObjectDeserializeFailed()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);


            parameters.Append(
                string.Format(
                    ", Object type: {0}, Message: {1}, Exception message: {2}",
                    ObjectType,
                    Message,
                    ExceptionMessage));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            if (GeneralOptions.Singleton.CorrectDeserializationFailures)
            {
                var objectDeserializeFailedCcu =
                    CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

                if (objectDeserializeFailedCcu != null
                    && IdObject != Guid.Empty)
                {
                    if (ObjectType != ObjectType.NotSupport)
                        CCUConfigurationHandler.Singleton.SendModifyObjectsToCCUsAsync(
                            IdObject,
                            ObjectType);
                }
            }
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds =
                new LinkedList<Guid>(
                    Enumerable.Repeat(ccu.IdCCU, 1));

            var description = new StringBuilder("Failed to deserialize ");

            if (IdObject != Guid.Empty)
            {
                if (ObjectType != ObjectType.CCU
                    || IdObject != ccu.IdCCU)
                {
                    eventSourcesIds.AddLast(IdObject);
                }

                description.AppendFormat(
                    "the object of type {0} with id {1} ",
                    ObjectType,
                    IdObject);
            }
            else
            {
                description.Append("an object ");

                if (ObjectType != ObjectType.NotSupport)
                    description.AppendFormat(
                        "of type {0} ",
                        ObjectType);
            }

            description.AppendFormat(
                "in method: {0}, with error: {1}",
                Message,
                ExceptionMessage);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_OBJECT_DESERIALIZE_FAILED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description.ToString(),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPE_CCU_VERSION,
                VersionsInfo.CcuVersion,
                EventlogParameter.TYPE_CE_VERSION,
                VersionsInfo.CeVersion,
                EventlogParameter.TYPE_SERVER_VERSION,
                VersionsInfo.ServerVersion);
        }
    }

    [LwSerialize(526)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSendTamper : EventParametersWithState
    {
        public Guid IdObject { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public EventSendTamper(
            UInt64 eventId,
            Guid idObject,
            ObjectType objectType,
            bool isTamper)
            : base(
                eventId,
                EventType.SendTamper,
                isTamper
                    ? State.On
                    : State.Off)
        {
            IdObject = idObject;
            ObjectType = objectType;
        }

        protected EventSendTamper()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Object id: {0}, Object type: {1}",
                    IdObject,
                    ObjectType));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            if (ObjectType == ObjectType.CardReader)
            {
                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

                if (ccu != null)
                {
                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        CCUCallbackRunner.RunCardReaderTamperStateChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[]
                        {
                            IdObject,
                            State == State.On,
                            ccu.IdCCU
                        });
                }
            }
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            IEnumerable<Guid> eventSourcesIds;
            string eventlogType;
            string description;

            switch (ObjectType)
            {
                case ObjectType.CCU:

                    var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);
                    if (ccu == null)
                    {
                        eventlog = null;
                        eventSources = null;
                        eventlogParameters = null;

                        return false;
                    }

                    eventSourcesIds = new[] {ccu.IdCCU};

                    eventlogType = Eventlog.TYPE_CCU_TAMPER_STATE_CHANGED;

                    description =
                        string.Format(
                            "Tamper on the CCU is in state {0}",
                            State == State.On
                                ? "alarm"
                                : "normal");
                    break;

                case ObjectType.DCU:

                    var dcu = DCUs.Singleton.GetById(IdObject);
                    if (dcu == null)
                    {
                        eventlog = null;
                        eventSources = null;
                        eventlogParameters = null;

                        return false;
                    }

                    eventSourcesIds = CCUEventsManager.GetEventSourcesFromDCU(dcu);

                    eventlogType = Eventlog.TYPE_DCU_TAMPER_STATE_CHANGED;

                    description =
                        string.Format(
                            "Tamper on the DCU is in state {0}",
                            State == State.On
                                ? "alarm"
                                : "normal");

                    break;

                case ObjectType.CardReader:

                    var cardReader = CardReaders.Singleton.GetById(IdObject);
                    if (cardReader == null)
                    {
                        eventlog = null;
                        eventSources = null;
                        eventlogParameters = null;

                        return false;
                    }

                    eventSourcesIds = CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        false);

                    eventlogType = Eventlog.TYPE_CARD_READER_TAMPER_STATE_CHANGED;

                    description =
                        string.Format(
                            "Tamper on the card reader is in state {0}",
                            State == State.On
                                ? "alarm"
                                : "normal");

                    break;

                default:
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
            }

            return Eventlogs.Singleton.CreateEvent(
                eventlogType,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(527)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventTamperInfo : EventParametersWithState
    {
        public Guid IdObject { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public EventTamperInfo(
            UInt64 eventId,
            Guid idObject,
            ObjectType objectType,
            bool isTamper)
            : base(
                eventId,
                EventType.TamperInfo,
                isTamper
                    ? State.On
                    : State.Off)
        {
            IdObject = idObject;
            ObjectType = objectType;
        }

        protected EventTamperInfo()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Object id: {0}, Object type: {1}",
                    IdObject,
                    ObjectType));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            if (ObjectType == ObjectType.CardReader)
            {
                var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

                if (ccu != null)
                {
                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        CCUCallbackRunner.RunCardReaderTamperStateChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[]
                        {
                            IdObject,
                            State == State.On,
                            ccu.IdCCU
                        });
                }
            }
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }

    [LwSerialize(528)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuActualTimeSent : EventParameters
    {
        public DateTime CcuTime { get; private set; }
        public DateTime ServerTime { get; set; }

        public EventCcuActualTimeSent(
            UInt64 eventId,
            DateTime ccuTime)
            : base(
                eventId,
                EventType.CcuActualTimeSent)
        {
            CcuTime = ccuTime;
        }

        protected EventCcuActualTimeSent()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", CCU time: {0}",
                    CcuTime));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            ccuSettings.CcuActualTimeReceived(
                CcuTime,
                ServerTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }

        public override void Initialize()
        {
            base.Initialize();

            ServerTime = DateTime.UtcNow;
        }
    }

    [LwSerialize(529)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuTimeAdjusted : EventParameters
    {
        public DateTime OldDateTime { get; private set; }
        public DateTime NewDateTime { get; private set; }

        public EventCcuTimeAdjusted(
            UInt64 eventId,
            DateTime oldDateTime,
            DateTime newDateTime)
            : base(
                eventId,
                EventType.CCUTimeAdjusted)
        {
            OldDateTime = oldDateTime;
            NewDateTime = newDateTime;
        }

        protected EventCcuTimeAdjusted()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Old time: {0}, New time: {1}",
                    OldDateTime,
                    NewDateTime));
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var previousTime = OldDateTime.ToString("dd.MM.yyyy HH:mm:ss");
            var newTime = NewDateTime.ToString("dd.MM.yyyy HH:mm:ss");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_TIME_ADJUSTED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format(
                    "CCU adjusted its date/time from {0} to {1} ",
                    previousTime,
                    newTime),
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(530)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuIncomingTransferInfo : EventParameters
    {
        public string DestinationName { get; private set; }
        public TimeSpan TransferDurationPure { get; private set; }

        public EventCcuIncomingTransferInfo(
            UInt64 eventId,
            string destinationName,
            TimeSpan transferDurationPure)
            : base(
                eventId,
                EventType.CCUIncomingTransferInfo)
        {
            DestinationName = destinationName;
            TransferDurationPure = transferDurationPure;
        }

        protected EventCcuIncomingTransferInfo()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Destination name: {0}, Transfer duration pure: {1}",
                    DestinationName,
                    TransferDurationPure));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            if (ccu == null)
                return;

            CCUConfigurationHandler.Singleton.TCPTransferSucceeded(ccu.IdCCU, DestinationName);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_INCOMING_TRANSFER_INFO,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                DestinationName,
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPE_STREAM_NAME,
                DestinationName,
                EventlogParameter.TYPE_DURATION,
                TransferDurationPure.ToString());
        }
    }

    [LwSerialize(531)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventForSpecialInputs : EventParametersWithState
    {
        protected EventForSpecialInputs(
            UInt64 eventId,
            EventType eventType,
            bool isOn)
            : base(
                eventId,
                eventType,
                isOn
                    ? State.On
                    : State.Off)
        {

        }

        protected EventForSpecialInputs()
        {
            
        }
    }
    [LwSerialize(532)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuPrimaryPowerMissing : EventForSpecialInputs
    {
        public EventAlarmCcuPrimaryPowerMissing(
            UInt64 eventId,
            bool isOn)
            : base(
                eventId,
                EventType.AlarmCCUPrimaryPowerMissing,
                isOn)
        {

        }

        protected EventAlarmCcuPrimaryPowerMissing()
        {

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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description =
                string.Format(
                    "Alarm primary power missing on the CCU is in state {0}",
                    State == State.On
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_PRIMARY_POWER_MISSING,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(533)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuBatteryIsLow : EventForSpecialInputs
    {
        public EventAlarmCcuBatteryIsLow(
            UInt64 eventId,
            bool isOn)
            : base(
                eventId,
                EventType.AlarmCCUBatteryIsLow,
                isOn)
        {

        }

        protected EventAlarmCcuBatteryIsLow()
        {

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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description =
                string.Format(
                    "Alarm battery is low on the CCU is in state {0}",
                    State == State.On
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_BATTERY_IS_LOW,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(534)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuExtFuse : EventForSpecialInputs
    {
        public EventAlarmCcuExtFuse(
            UInt64 eventId,
            bool isOn)
            : base(
                eventId,
                EventType.AlarmCCUExtFuse,
                isOn)
        {

        }

        protected EventAlarmCcuExtFuse()
        {

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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description =
                string.Format(
                    "Alarm fuse on extension board on the CCU is in state {0}",
                    State == State.On
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_FUSE_ON_EXTENSION_BOARD,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(535)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuTimingProblem : EventParametersWithObjectIdAndState
    {
        public EventCcuTimingProblem(
            UInt64 eventId,
            State actualState,
            Guid idObject)
            : base(
                eventId,
                EventType.CCUTimingProblem,
                idObject,
                actualState)
        {

        }

        protected EventCcuTimingProblem()
        {
            
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            if (ccu == null
                || IdObject == Guid.Empty)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_TIMING_PROBLEM,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                new[] {ccu.IdCCU, IdObject},
                "CCU timing problem",
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPE_CCU_VERSION,
                VersionsInfo.CcuVersion,
                EventlogParameter.TYPE_CE_VERSION,
                VersionsInfo.CeVersion,
                EventlogParameter.TYPE_SERVER_VERSION,
                VersionsInfo.ServerVersion);
        }
    }

    [LwSerialize(536)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCoprocessorFailureChanged : EventParametersWithState
    {
        public EventCoprocessorFailureChanged(
            UInt64 eventId,
            bool coprocessorFailure)
            : base(
                eventId,
                EventType.CoprocessorFailureChanged,
                coprocessorFailure
                    ? State.Alarm
                    : State.Normal)
        {

        }

        protected EventCoprocessorFailureChanged()
        {
            
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_COPROCESSOR_FAILURE_CHANGED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                new[] {ccu.IdCCU},
                string.Format(
                    "CLSP coprocessor is {0}working properly: {1}",
                    State == State.Alarm
                        ? "not "
                        : "",
                    ccu.Name),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPE_CCU_VERSION,
                VersionsInfo.CcuVersion,
                EventlogParameter.TYPE_CE_VERSION,
                VersionsInfo.CeVersion,
                EventlogParameter.TYPE_SERVER_VERSION,
                VersionsInfo.ServerVersion);
        }
    }

    [LwSerialize(537)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventICcuPortAlreadyUsed : EventParametersWithState
    {
        public EventICcuPortAlreadyUsed(
            UInt64 eventId,
            State state)
            : base(
                eventId,
                EventType.ICCUPortAlreadyUsed,
                state)
        {

        }

        protected EventICcuPortAlreadyUsed()
        {
            
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
            if (State != State.Alarm)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ICCU_PORT_ALREADY_USED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                new[] {ccu.IdCCU},
                "ICCU: Port for ICCU communication is already used",
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(538)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventICcuSendingOfObjectStateFailed : EventParameters
    {
        public Guid IdObject { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public EventICcuSendingOfObjectStateFailed(
            UInt64 eventId,
            Guid idObject,
            ObjectType objectType)
            : base(
                eventId,
                EventType.ICCUSendingOfObjectStateFailed)
        {
            IdObject = idObject;
            ObjectType = objectType;
        }

        protected EventICcuSendingOfObjectStateFailed()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Object id: {0}, Object type: {1}",
                    IdObject,
                    ObjectType));
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = new LinkedList<Guid>(
                Enumerable.Repeat(
                    ccu.IdCCU,
                    1));

            switch (ObjectType)
            {
                case ObjectType.Input:

                    var input = Inputs.Singleton.GetById(IdObject);

                    if (input != null &&
                        input.DCU != null)
                        eventSourcesIds.AddLast(input.DCU.IdDCU);
                    break;

                case ObjectType.Output:

                    var output = Outputs.Singleton.GetById(IdObject);

                    if (output != null &&
                        output.DCU != null)
                        eventSourcesIds.AddLast(output.DCU.IdDCU);
                    break;
            }

            eventSourcesIds.AddLast(IdObject);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ICCU_SENDING_OF_OBJECT_STATE_FAILED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                "ICCU: Sending of object state failed",
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(539)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSecurityTimeChannelChanged : EventParametersWithObjectIdAndState
    {
        public ObjectType ObjectType { get; private set; }
        public DateTime? NextTickDateTime { get; private set; }
        public int? NextTickSeconds { get; private set; }

        protected EventSecurityTimeChannelChanged(
            UInt64 eventId,
            State state,
            Guid idObject,
            ObjectType? objectType,
            DateTime? nextTickDateTime,
            int? nextTickSeconds)
            : base(
                eventId,
                EventType.SecurityTimeChannelChanged,
                idObject,
                state)
        {
            ObjectType = objectType != null
                ? objectType.Value
                : ObjectType.NotSupport;

            NextTickDateTime = nextTickDateTime;
            NextTickSeconds = nextTickSeconds;
        }

        public EventSecurityTimeChannelChanged(
            UInt64 eventId,
            State state,
            Guid idObject,
            ObjectType objectType)
            : this(
                eventId,
                state,
                idObject,
                objectType,
                null,
                null)
        {

        }

        public EventSecurityTimeChannelChanged(
            UInt64 eventId,
            DateTime nextTickDateTime,
            int nextTickSeconds)
            : this(
                eventId,
                State.Unknown,
                Guid.Empty,
                null,
                nextTickDateTime,
                nextTickSeconds)
        {

        }

        protected EventSecurityTimeChannelChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Object type: {0}",
                    ObjectType));

            if (NextTickDateTime != null)
                parameters.Append(
                    string.Format(
                        ", Next tick date time: {0}",
                        NextTickDateTime));

            if (NextTickSeconds != null)
                parameters.Append(
                    string.Format(
                        ", Next tick seconds: {0}",
                        NextTickSeconds));
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            if (IdObject == Guid.Empty)
            {
                if (NextTickDateTime == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_TIME_FOR_NEXT_EVALUATING_STATES_OF_SDP,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    string.Format(
                        "Time for next evaluating states of security daily plans is {0}",
                        NextTickDateTime.Value),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
            }

            switch (ObjectType)
            {
                case ObjectType.SecurityDailyPlan:
                    var securityDailyPlan =
                        SecurityDailyPlans.Singleton.GetById(IdObject);

                    if (securityDailyPlan == null)
                    {
                        eventlog = null;
                        eventSources = null;
                        eventlogParameters = null;

                        return false;
                    }

                    eventSourcesIds.AddLast(securityDailyPlan.IdSecurityDailyPlan);

                    return Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPE_ACTUAL_STATE_OF_SECURITY_DAILY_PLAN,
                        DateTime,
                        ccuSettings.CCUEvents.ThisAssemblyName,
                        eventSourcesIds,
                        string.Format(
                            "Actual state of securtiy daily plan {0} is {1}",
                            securityDailyPlan,
                            State),
                        out eventlog,
                        out eventSources,
                        out eventlogParameters);

                case ObjectType.SecurityTimeZone:

                    var securityTimeZone =
                        SecurityTimeZones.Singleton.GetById(IdObject);

                    if (securityTimeZone == null)
                    {
                        eventlog = null;
                        eventSources = null;
                        eventlogParameters = null;

                        return false;
                    }

                    eventSourcesIds.AddLast(securityTimeZone.IdSecurityTimeZone);

                    return
                        Eventlogs.Singleton.CreateEvent(
                            Eventlog.TYPE_ACTUAL_STATE_OF_SECURITY_TIME_ZONE,
                            DateTime,
                            ccuSettings.CCUEvents.ThisAssemblyName,
                            eventSourcesIds,
                            string.Format(
                                "Actual state of securtiy time zone {0} is {1}",
                                securityTimeZone,
                                State),
                            out eventlog,
                            out eventSources,
                            out eventlogParameters);

                default:
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
            }
        }
    }

    [LwSerialize(540)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventGetFromDatabaseReturnNull : EventParameters
    {
        public Guid IdObject { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public EventGetFromDatabaseReturnNull(
            UInt64 eventId,
            Guid idObject,
            ObjectType objectType)
            : base(
                eventId,
                EventType.GetFromDatabaseReturnNull)
        {
            IdObject = idObject;
            ObjectType = objectType;
        }

        protected EventGetFromDatabaseReturnNull()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Object id: {0}, Object type: {1}",
                    IdObject,
                    ObjectType));
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
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);
            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = new LinkedList<Guid>(
                Enumerable.Repeat(
                    ccu.IdCCU,
                    1));

            if (ObjectType != ObjectType.CCU || IdObject != ccu.IdCCU)
                eventSourcesIds.AddLast(IdObject);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_GETFROMDATABASE_RETURN_NULL,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format(
                    "The CCU returned null in loading the object of type {0} with id {1}",
                    ObjectType,
                    IdObject),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPE_CCU_VERSION,
                VersionsInfo.CcuVersion,
                EventlogParameter.TYPE_CE_VERSION,
                VersionsInfo.CeVersion,
                EventlogParameter.TYPE_SERVER_VERSION,
                VersionsInfo.ServerVersion);
        }
    }

    [LwSerialize(543)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventInvalidPinRetriesLimitReached : EventParametersWithObjectId
    {
        public DateTime UtcTime
        {
            get;
            private set;
        }

        protected EventInvalidPinRetriesLimitReached()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.AppendFormat(
                ", UtcTime: {0}",
                UtcTime);
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            Cards.Singleton.OnInvalidPinRetriesLimitReached(
                IdObject,
                UtcTime);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }
}
