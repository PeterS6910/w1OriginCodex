using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(490)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmStateChanged : EventParametersWithObjectIdAndState
    {
        public EventDsmStateChanged(
            UInt64 eventId,
            State doorEnvironmentState,
            Guid idDoorEnvironment)
            : base(
                eventId,
                EventType.DSMStateChanged,
                idDoorEnvironment,
                doorEnvironmentState)
        {

        }

        protected EventDsmStateChanged()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var doorEnvironmentState = DoorEnvironmentState.Unknown;

            switch (State)
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
                IdObject,
                DateTime,
                doorEnvironmentState);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            IEnumerable<Guid> eventSourcesIds;
            var doorEnvironment = DoorEnvironments.Singleton.GetById(IdObject);
            if (doorEnvironment != null)
            {
                eventSourcesIds =
                    CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment);
            }
            else
            {
                var multiDoorElement = MultiDoorElements.Singleton.GetById(IdObject);
                if (multiDoorElement == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                eventSourcesIds =
                    CCUEventsManager.GetEventSourcesFromMultiDoorElement(multiDoorElement);
            }

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_DSM_STATE_CHANGED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format(
                    "DSM state was changed. Actual DSM state is: {0}",
                    State),
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(491)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventForDsmAccess : EventParametersWithObjectId
    {
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }
        public Guid IdPushButton { get; private set; }

        protected EventForDsmAccess(
            UInt64 eventId,
            EventType eventType,
            Guid idDoorEnvironment,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idPushButton)
            : base(
                eventId,
                eventType,
                idDoorEnvironment)
        {
            IdCardReader = idCardReader;
            IdCard = idCard;
            IdPerson = idPerson;
            IdPushButton = idPushButton;
        }

        protected EventForDsmAccess()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card reader: {0}, Card: {1}, Person: {2}, Pushbutton: {3}",
                    IdCardReader,
                    IdCard,
                    IdPerson,
                    IdPushButton));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            
        }
    }

    [LwSerialize(492)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmAccessPermitted : EventForDsmAccess
    {
        public EventDsmAccessPermitted(
            UInt64 eventId,
            Guid idDoorEnvironment,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idPushButton)
            : base(
                eventId,
                EventType.DSMAccessPermitted,
                idDoorEnvironment,
                idCardReader,
                idCard,
                idPerson,
                idPushButton)
        {

        }

        protected EventDsmAccessPermitted()
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
            ICollection<Guid> eventSourcesIds;
            var doorEnvironment = DoorEnvironments.Singleton.GetById(IdObject);
            if (doorEnvironment != null)
            {
                eventSourcesIds = new LinkedList<Guid>(
                    CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment));
            }
            else
            {
                var multiDoorElement = MultiDoorElements.Singleton.GetById(IdObject);
                if (multiDoorElement == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                eventSourcesIds = new LinkedList<Guid>(
                    CCUEventsManager.GetEventSourcesFromMultiDoorElement(multiDoorElement));
            }

            var parameters =
                CcuEvents.CreateDsmEventsParametersAndFillEventSources(
                    IdCardReader,
                    IdCard,
                    null,
                    IdPerson,
                    IdPushButton,
                    null,
                    eventSourcesIds);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMACCESSPERMITTED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                "Access permitted",
                out eventlog,
                out eventSources,
                out eventlogParameters,
                parameters != null && parameters.Count > 0
                    ? parameters.ToArray()
                    : null);
        }
    }

    [LwSerialize(493)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmNormalAccess : EventForDsmAccess
    {
        public EventDsmNormalAccess(
            UInt64 eventId,
            Guid idDoorEnvironment,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idPushButton)
            : base(
                eventId,
                EventType.DSMNormalAccess,
                idDoorEnvironment,
                idCardReader,
                idCard,
                idPerson,
                idPushButton)
        {

        }

        protected EventDsmNormalAccess()
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(IdObject);
            
            if (doorEnvironment == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = new LinkedList<Guid>(
                CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment));

            var parameters =
                CcuEvents.CreateDsmEventsParametersAndFillEventSources(
                    IdCardReader,
                    IdCard,
                    null,
                    IdPerson,
                    IdPushButton,
                    null,
                    eventSourcesIds);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEDSMNORMALACCESS,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    "Normal access",
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    parameters != null && parameters.Count > 0
                        ? parameters.ToArray()
                        : null);
        }
    }

    [LwSerialize(494)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmApasRestored : EventForDsmAccess
    {
        public EventDsmApasRestored(
            UInt64 eventId,
            Guid idDoorEnvironment,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idPushButton)
            : base(
                eventId,
                EventType.DSMApasRestored,
                idDoorEnvironment,
                idCardReader,
                idCard,
                idPerson,
                idPushButton)
        {

        }

        protected EventDsmApasRestored()
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(IdObject);

            if (doorEnvironment == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = new LinkedList<Guid>(
                CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment));

            var parameters =
                CcuEvents.CreateDsmEventsParametersAndFillEventSources(
                    IdCardReader,
                    IdCard,
                    null,
                    IdPerson,
                    IdPushButton,
                    null,
                    eventSourcesIds);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMAPASRESTORED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                "APAS restored ",
                out eventlog,
                out eventSources,
                out eventlogParameters,
                parameters != null && parameters.Count > 0
                    ? parameters.ToArray()
                    : null);
        }
    }

    [LwSerialize(495)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmAccessInterupted : EventForDsmAccess
    {
        public EventDsmAccessInterupted(
            UInt64 eventId,
            Guid idDoorEnvironment,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idPushButton)
            : base(
                eventId,
                EventType.DSMAccessInterupted,
                idDoorEnvironment,
                idCardReader,
                idCard,
                idPerson,
                idPushButton)
        {

        }

        protected EventDsmAccessInterupted()
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var doorEnvironment = DoorEnvironments.Singleton.GetById(IdObject);

            if (doorEnvironment == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = new LinkedList<Guid>(
                CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment));

            var parameters =
                CcuEvents.CreateDsmEventsParametersAndFillEventSources(
                    IdCardReader,
                    IdCard,
                    null,
                    IdPerson,
                    IdPushButton,
                    null,
                    eventSourcesIds);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMACCESSINTERRUPTED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                "Access interrupted",
                out eventlog,
                out eventSources,
                out eventlogParameters,
                parameters != null && parameters.Count > 0
                    ? parameters.ToArray()
                    : null);
        }
    }

    [LwSerialize(496)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmAccessViolated : EventParametersWithObjectId
    {
        public string Reason { get; private set; }

        public EventDsmAccessViolated(
            UInt64 eventId,
            Guid idDoorEnvironment,
            string reason)
            : base(
                eventId,
                EventType.DSMAccessViolated,
                idDoorEnvironment)
        {
            Reason = reason;
        }

        protected EventDsmAccessViolated()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Reason: {0}",
                    Reason));
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
            var doorEnvironment = DoorEnvironments.Singleton.GetById(IdObject);

            if (doorEnvironment == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = new LinkedList<Guid>(
                CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment));

            var parameters =
                CcuEvents.CreateDsmEventsParametersAndFillEventSources(
                    Guid.Empty,
                    Guid.Empty,
                    null,
                    Guid.Empty,
                    Guid.Empty,
                    Reason,
                    eventSourcesIds);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMACCESSVIOLATED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                "Access violated ",
                out eventlog,
                out eventSources,
                out eventlogParameters,
                parameters != null && parameters.Count > 0
                    ? parameters.ToArray()
                    : null);
        }
    }

    [LwSerialize(497)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDsmAccessRestricted : EventParametersWithObjectId
    {
        public Guid IdCardReader { get; private set; }
        public Guid IdCard { get; private set; }
        public string CardNumber { get; private set; }
        public string Message { get; private set; }

        public EventDsmAccessRestricted(
            UInt64 eventId,
            Guid idDoorEnvironment,
            Guid idCardReader,
            Guid idCard,
            string cardNumber,
            string message)
            : base(
                eventId,
                EventType.DSMAccessRestricted,
                idDoorEnvironment)
        {
            IdCardReader = idCardReader;
            IdCard = idCard;
            CardNumber = cardNumber;
            Message = message;
        }

        protected EventDsmAccessRestricted()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card reader: {0}, Card: {1}, Card number: {2}, Message: {3}",
                    IdCardReader,
                    IdCard,
                    CardNumber,
                    Message));
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
            var doorEnvironment = DoorEnvironments.Singleton.GetById(IdObject);

            if (doorEnvironment == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = new LinkedList<Guid>(
                CCUEventsManager.GetEventSourcesFromDoorEnvironment(doorEnvironment));

            var parameters =
                CcuEvents.CreateDsmEventsParametersAndFillEventSources(
                    IdCardReader,
                    IdCard,
                    CardNumber,
                    Guid.Empty,
                    Guid.Empty,
                    Message,
                    eventSourcesIds);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDSMACCESSRESTRICTED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                "Access restricted",
                out eventlog,
                out eventSources,
                out eventlogParameters,
                parameters != null && parameters.Count > 0
                    ? parameters.ToArray()
                    : null);
        }
    }
}
