using System;
using System.Collections.Generic;
using System.Text;
using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(520)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventRunMethodFailed : EventParameters
    {
        public string MethodName { get; private set; }
        public string Exception { get; private set; }

        public EventRunMethodFailed(
            string methodName,
            string exception)
            : base(
                EventType.RunMethodFailed)
        {
            MethodName = methodName;
            Exception = exception;
            VersionsInfo = CcuCore.Singleton.VersionsInfo;
        }

        public EventRunMethodFailed()
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
    }

    [LwSerialize(521)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuMemoryLoadStateChanged : EventParametersWithState
    {
        public EventCcuMemoryLoadStateChanged(bool isHighCcuMemoryLoad)
            : base(
                EventType.CCUMemoryLoadStateChanged,
                isHighCcuMemoryLoad
                    ? State.Alarm
                    : State.Normal)
        {
            
        }

        public EventCcuMemoryLoadStateChanged()
        {
            
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
            DateTime dateTime,
            Exception exception,
            int threadId)
            : base(
                EventType.ExceptionOccurred,
                dateTime)
        {
            ExceptionMessage = exception.ToString();
            
            if (ExceptionMessage.Length > 1000)
                ExceptionMessage = ExceptionMessage.Substring(0, 1000);

            StackTrace = exception.StackTrace;

            var maxStackTraceLength = 1500 - ExceptionMessage.Length;
            if (StackTrace.Length > maxStackTraceLength)
                StackTrace = StackTrace.Substring(0, maxStackTraceLength);

            ThreadId = threadId;
            VersionsInfo = CcuCore.Singleton.VersionsInfo;
        }

        public EventExceptionOccured()
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
    }

    [LwSerialize(523)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuFilesystemProblem : EventParametersWithState
    {
        public string FileName { get; private set; }
        public string FileOperation { get; private set; }

        public EventCcuFilesystemProblem(
            Exception exception,
            string fileName,
            string fileOperation)
            : base(
                EventType.CCUFilesystemProblem,
                exception != null
                    ? State.Alarm
                    : State.Normal)
        {
            FileName = fileName;
            FileOperation = fileOperation;
            VersionsInfo = CcuCore.Singleton.VersionsInfo;
        }

        public EventCcuFilesystemProblem()
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
    }

    [LwSerialize(524)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuSdCardNotFound : EventParametersWithState
    {
        public EventCcuSdCardNotFound(
            bool sdCardPresent)
            : base(
                EventType.CCUSdCardNotFound,
                sdCardPresent
                    ? State.Normal
                    : State.Alarm)
        {
            VersionsInfo = CcuCore.Singleton.VersionsInfo;
        }

        public EventCcuSdCardNotFound()
        {
            
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
            Guid idObject,
            ObjectType objectType,
            string message,
            string exceptionMessage)
            : base(
                EventType.ObjectDeserializeFailed,
                idObject)
        {
            ObjectType = objectType;
            Message = message;
            ExceptionMessage = exceptionMessage;
            VersionsInfo = CcuCore.Singleton.VersionsInfo;
        }

        public EventObjectDeserializeFailed()
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
    }

    [LwSerialize(526)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSendTamper : EventParametersWithState
    {
        public Guid IdObject { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public EventSendTamper(
            Guid idObject,
            ObjectType objectType,
            bool isTamper)
            : base(
                EventType.SendTamper,
                isTamper
                    ? State.On
                    : State.Off)
        {
            IdObject = idObject;
            ObjectType = objectType;
        }

        public EventSendTamper()
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
    }

    [LwSerialize(527)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventTamperInfo : EventParametersWithState
    {
        public Guid IdObject { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public EventTamperInfo(
            Guid idObject,
            ObjectType objectType,
            bool isTamper)
            : base(
                EventType.TamperInfo,
                isTamper
                    ? State.On
                    : State.Off)
        {
            IdObject = idObject;
            ObjectType = objectType;
        }

        public EventTamperInfo()
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
    }

    [LwSerialize(528)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuActualTimeSent : EventParameters
    {
        public DateTime CcuTime { get; private set; }

        public EventCcuActualTimeSent(
            DateTime ccuTime)
            : base(
                EventType.CcuActualTimeSent)
        {
            CcuTime = ccuTime;
        }

        public EventCcuActualTimeSent()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", CCU time: {0}",
                    CcuTime));
        }
    }

    [LwSerialize(529)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuTimeAdjusted : EventParameters
    {
        public DateTime OldDateTime { get; private set; }
        public DateTime NewDateTime { get; private set; }

        public EventCcuTimeAdjusted(
            DateTime oldDateTime,
            DateTime newDateTime)
            : base(
                EventType.CCUTimeAdjusted)
        {
            OldDateTime = oldDateTime;
            NewDateTime = newDateTime;
        }

        public EventCcuTimeAdjusted()
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
    }

    [LwSerialize(530)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuIncomingTransferInfo : EventParameters
    {
        public string DestinationName { get; private set; }
        public TimeSpan TransferDurationPure { get; private set; }

        public EventCcuIncomingTransferInfo(
            string destinationName,
            TimeSpan transferDurationPure)
            : base(
                EventType.CCUIncomingTransferInfo)
        {
            DestinationName = destinationName;
            TransferDurationPure = transferDurationPure;
        }

        public EventCcuIncomingTransferInfo()
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
    }

    [LwSerialize(531)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventForSpecialInputs : EventParametersWithState
    {
        protected EventForSpecialInputs(
            EventType eventType,
            bool isOn)
            : base(
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
            bool isOn)
            : base(
                EventType.AlarmCCUPrimaryPowerMissing,
                isOn)
        {

        }

        public EventAlarmCcuPrimaryPowerMissing()
        {
            
        }
    }

    [LwSerialize(533)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuBatteryIsLow : EventForSpecialInputs
    {
        public EventAlarmCcuBatteryIsLow(
            bool isOn)
            : base(
                EventType.AlarmCCUBatteryIsLow,
                isOn)
        {

        }

        public EventAlarmCcuBatteryIsLow()
        {
            
        }
    }

    [LwSerialize(534)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuExtFuse : EventForSpecialInputs
    {
        public EventAlarmCcuExtFuse(
            bool isOn)
            : base(
                EventType.AlarmCCUExtFuse,
                isOn)
        {

        }

        public EventAlarmCcuExtFuse()
        {
            
        }
    }

    [LwSerialize(535)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCcuTimingProblem : EventParametersWithObjectIdAndState
    {
        public EventCcuTimingProblem(
            State actualState,
            Guid idObject)
            : base(
                EventType.CCUTimingProblem,
                idObject,
                actualState)
        {
            VersionsInfo = CcuCore.Singleton.VersionsInfo;
        }

        public EventCcuTimingProblem()
        {
            
        }
    }

    [LwSerialize(536)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCoprocessorFailureChanged : EventParametersWithState
    {
        public EventCoprocessorFailureChanged(
            bool coprocessorFailure)
            : base(
                EventType.CoprocessorFailureChanged,
                coprocessorFailure
                    ? State.Alarm
                    : State.Normal)
        {
            VersionsInfo = CcuCore.Singleton.VersionsInfo;
        }

        public EventCoprocessorFailureChanged()
        {
            
        }
    }

    [LwSerialize(537)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventICcuPortAlreadyUsed : EventParametersWithState
    {
        public EventICcuPortAlreadyUsed(State state)
            : base(
                EventType.ICCUPortAlreadyUsed,
                state)
        {

        }

        public EventICcuPortAlreadyUsed()
        {
            
        }
    }

    [LwSerialize(538)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventICcuSendingOfObjectStateFailed : EventParameters
    {
        public Guid IdObject { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public EventICcuSendingOfObjectStateFailed(
            Guid idObject,
            ObjectType objectType)
            : base(
                EventType.ICCUSendingOfObjectStateFailed)
        {
            IdObject = idObject;
            ObjectType = objectType;
        }

        public EventICcuSendingOfObjectStateFailed()
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
    }

    [LwSerialize(539)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSecurityTimeChannelChanged : EventParametersWithObjectIdAndState
    {
        public ObjectType ObjectType { get; private set; }
        public DateTime? NextTickDateTime { get; private set; }
        public int? NextTickSeconds { get; private set; }

        protected EventSecurityTimeChannelChanged(
            State state,
            Guid idObject,
            ObjectType? objectType,
            DateTime? nextTickDateTime,
            int? nextTickSeconds)
            : base(
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
            State state,
            Guid idObject,
            ObjectType objectType)
            : this(
                state,
                idObject,
                objectType,
                null,
                null)
        {

        }

        public EventSecurityTimeChannelChanged(
            DateTime nextTickDateTime,
            int nextTickSeconds)
            : this(
                State.Unknown,
                Guid.Empty,
                null,
                nextTickDateTime,
                nextTickSeconds)
        {

        }

        public EventSecurityTimeChannelChanged()
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
    }

    [LwSerialize(540)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventGetFromDatabaseReturnNull : EventParameters
    {
        public Guid IdObject { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public EventGetFromDatabaseReturnNull(
            Guid idObject,
            ObjectType objectType)
            : base(
                EventType.GetFromDatabaseReturnNull)
        {
            IdObject = idObject;
            ObjectType = objectType;
            VersionsInfo = CcuCore.Singleton.VersionsInfo;
        }

        public EventGetFromDatabaseReturnNull()
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

        public EventInvalidPinRetriesLimitReached()
        {
            
        }

        public EventInvalidPinRetriesLimitReached(
            Guid idCard)
            : base(EventType.InvalidPinRetriesLimitReached, idCard)
        {
            UtcTime = DateTime.UtcNow;
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.AppendFormat(
                ", UtcTime: {0}",
                UtcTime);
        }
    }

    public static class TestSystemEvents
    {
        public static void EnqueueTestEvents(
            Guid idInput,
            Guid idCard,
            Guid idCardReader,
            Guid idDailyPlan,
            Guid idSecurityTimeZone)
        {
            Events.ProcessEvent(new EventRunMethodFailed(
                "TestMethod",
                "error"));

            //Events.ProcessEvent(new EventCcuMemoryLoadStateChanged(
            //    false));

            Events.ProcessEvent(new EventExceptionOccured(
                DateTime.Now,
                new Exception("TestException"),
                System.Threading.Thread.CurrentThread.ManagedThreadId));

            Events.ProcessEvent(new EventCcuFilesystemProblem(
                new Exception("TestException"),
                "TestFile",          
                "Write"));

            Events.ProcessEvent(new EventCcuSdCardNotFound(
                false));

            Events.ProcessEvent(new EventObjectDeserializeFailed(
                idInput,
                ObjectType.Input,
                "message",
                "exception message"));

            //Events.ProcessEvent(new EventSendTamper(
            //    idCardReader,
            //    ObjectType.CardReader,
            //    true));

            //Events.ProcessEvent(new EventTamperInfo(
            //    idCardReader,
            //    ObjectType.CardReader,
            //    false));

            //Events.ProcessEvent(new EventCcuActualTimeSent(DateTime.Now));

            //Events.ProcessEvent(new EventCcuTimeAdjusted(
            //    DateTime.Now.AddMinutes(-10),
            //    DateTime.Now));

            //Events.ProcessEvent(new EventCcuIncomingTransferInfo(
            //    "Test",
            //    new TimeSpan(0, 0, 1, 0)));

            //Events.ProcessEvent(new EventAlarmCcuPrimaryPowerMissing(true));

            //Events.ProcessEvent(new EventAlarmCcuBatteryIsLow(true));
            
            //Events.ProcessEvent(new EventAlarmCcuExtFuse(true));

            Events.ProcessEvent(new EventCcuTimingProblem(
                State.On,
                idDailyPlan));

            Events.ProcessEvent(new EventCoprocessorFailureChanged(true));
            
            //Events.ProcessEvent(new EventICcuPortAlreadyUsed(State.Alarm));

            //Events.ProcessEvent(new EventSecurityTimeChannelChanged(
            //    DateTime.Now,
            //    10));

            //Events.ProcessEvent(new EventSecurityTimeChannelChanged(
            //    State.card,
            //    idSecurityTimeZone,
            //    ObjectType.SecurityTimeZone));

            Events.ProcessEvent(new EventGetFromDatabaseReturnNull(
                idCardReader,
                ObjectType.CardReader));

            //Events.ProcessEvent(new EventAutonomousRunEventsFilesPrepared(
            //    new List<string>
            //    {
            //        "Event0.dat"
            //    }));

            //Events.ProcessEvent(new EventAutonomousRunEventFileProcessed("Event0.dat"));

            //Events.ProcessEvent(new EventInvalidPinRetriesLimitReached(idCard));
        }
    }
}
