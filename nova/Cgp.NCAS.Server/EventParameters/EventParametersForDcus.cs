using System;
using System.Collections.Generic;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(470)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuCommandNotAck : EventParametersWithObjectId
    {
        public string ErrorCode { get; private set; }
        public string Command { get; private set; }

        public EventDcuCommandNotAck(
            UInt64 eventId,
            Guid idDcu,
            string errorCode,
            string command)
            : base(
                eventId,
                EventType.DcuCommandNotACK,
                idDcu)
        {
            ErrorCode = errorCode;
            Command = command;
        }

        protected EventDcuCommandNotAck()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Error code: {0}, Command: {1}",
                    ErrorCode,
                    Command));
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
            var dcu = DCUs.Singleton.GetById(IdObject);

            if (dcu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var parameters = string.IsNullOrEmpty(Command)
                ? new[]
                {
                    EventlogParameter.TYPE_REASON,
                    ErrorCode
                }
                : new[]
                {
                    EventlogParameter.TYPE_REASON,
                    ErrorCode,
                    EventlogParameter.TYPE_COMMAND,
                    Command
                };

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUCOMMANDNACK,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                "DCU command not ack ",
                out eventlog,
                out eventSources,
                out eventlogParameters,
                parameters);
        }
    }

    [LwSerialize(471)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuCommandTimeOut : EventParametersWithObjectId
    {
        public string Command { get; private set; }

        public EventDcuCommandTimeOut(
            UInt64 eventId,
            Guid idDcu,
            string command)
            : base(
                eventId,
                EventType.DcuCommandTimeOut,
                idDcu)
        {
            Command = command;
        }

        protected EventDcuCommandTimeOut()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Command: {0}",
                    Command));
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
            var dcu = DCUs.Singleton.GetById(IdObject);
            
            if (dcu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var description = new StringBuilder("DCU command timeout: ");

            description.Append(
                string.IsNullOrEmpty(Command)
                    ? "unknown"
                    : Command);

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUCOMMANDTIMEOUT,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                description.ToString(),
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(472)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuNodeAssigned : EventParametersWithObjectId
    {
        public EventDcuNodeAssigned(
            UInt64 eventId,
            Guid idDcu)
            : base(
                eventId,
                EventType.DcuNodeAssigned,
                idDcu)
        {

        }

        protected EventDcuNodeAssigned()
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
            var dcu = DCUs.Singleton.GetById(IdObject);

            if (dcu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUNODASSIGNED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                "DCU assigned ",
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(473)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuNodeReleased : EventParametersWithObjectId
    {
        public EventDcuNodeReleased(
            UInt64 eventId,
            Guid idDcu)
            : base(
                eventId,
                EventType.DcuNodeReleased,
                idDcu)
        {

        }

        protected EventDcuNodeReleased()
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
            var dcu = DCUs.Singleton.GetById(IdObject);

            if (dcu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUNODRELEASED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                "DCU released ",
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(474)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuNodeRenewed : EventParametersWithObjectId
    {
        public EventDcuNodeRenewed(
            UInt64 eventId,
            Guid idDcu)
            : base(
                eventId,
                EventType.DcuNodeRenewed,
                idDcu)
        {

        }

        protected EventDcuNodeRenewed()
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
            var dcu = DCUs.Singleton.GetById(IdObject);

            if (dcu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEDCUNODRENEWED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CCUEventsManager.GetEventSourcesFromDCU(dcu),
                "DCU renewed ",
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(475)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuInputsSabotage : EventParametersWithObjectIdAndState
    {
        public EventDcuInputsSabotage(
            UInt64 eventId,
            State sabotageState,
            Guid idDcu)
            : base(
                eventId,
                EventType.DcuInputsSabotage,
                idDcu,
                sabotageState)
        {

        }

        protected EventDcuInputsSabotage()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.DcuInputsSabotageStateChanged(
                DateTime,
                IdObject,
                State);
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

    [LwSerialize(476)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuFirmwareVersion : EventParametersWithObjectId
    {
        public string FirmwareVersion { get; private set; }

        public EventDcuFirmwareVersion(
            UInt64 eventId,
            Guid idDcu,
            string firmwareVersion)
            : base(
                eventId,
                EventType.DcuFirmwareVersion,
                idDcu)
        {
            FirmwareVersion = firmwareVersion;
        }

        protected EventDcuFirmwareVersion()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Firmware version: {0}",
                    FirmwareVersion));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.DCUFirmwareVersion(
                IdObject,
                FirmwareVersion);
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

    [LwSerialize(477)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuMemoryWarning : EventParametersWithObjectId
    {
        public byte Memory { get; private set; }

        public EventDcuMemoryWarning(
            UInt64 eventId,
            Guid idDcu,
            byte memory)
            : base(
                eventId,
                EventType.DcuMemoryWarning,
                idDcu)
        {
            Memory = memory;
        }

        protected EventDcuMemoryWarning()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Memory: {0}",
                    Memory));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.DcuMemoryWarning(
                IdObject,
                Memory);
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

    [LwSerialize(478)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuInputCount : EventParametersWithObjectId
    {
        public byte InputCount { get; private set; }

        public EventDcuInputCount(
            UInt64 eventId,
            Guid idDcu,
            byte inputCount)
            : base(
                eventId,
                EventType.DcuInputCount,
                idDcu)
        {
            InputCount = inputCount;
        }

        protected EventDcuInputCount()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Input count: {0}",
                    InputCount));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.DCUInputCount(
                IdObject,
                InputCount);
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

    [LwSerialize(479)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuOutputCount : EventParametersWithObjectId
    {
        public byte OutputCount { get; private set; }

        public EventDcuOutputCount(
            UInt64 eventId,
            Guid idDcu,
            byte outputCount)
            : base(
                eventId,
                EventType.DcuOutputCount,
                idDcu)
        {
            OutputCount = outputCount;
        }

        protected EventDcuOutputCount()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Output count: {0}",
                    OutputCount));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.DCUOutputCount(
                IdObject,
                OutputCount);
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

    [LwSerialize(480)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuOnlineStateChanged : EventParametersWithState
    {
        public byte LogicalAddress { get; private set; }
        public byte ProtocolId { get; private set; }

        public EventDcuOnlineStateChanged(
            UInt64 eventId,
            State onlineState,
            byte logicalAddress,
            byte protocolId)
            : base(
                eventId,
                EventType.DcuOnlineStateChanged,
                onlineState)
        {
            LogicalAddress = logicalAddress;
            ProtocolId = protocolId;
        }

        protected EventDcuOnlineStateChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Logical address: {0}, Protocol id: {1}",
                    LogicalAddress,
                    ProtocolId));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.DCUOnlineStateChanged(
                DateTime,
                LogicalAddress,
                CCUConfigurationHandler.ConvertToOnlineStateFromState(State),
                ccuSettings.IPAddressString,
                ProtocolId);
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

    [LwSerialize(481)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuPhysicalAddressChanged : EventParameters
    {
        public byte LogicalAddress { get; private set; }
        public string PhysicalAddress { get; private set; }

        public EventDcuPhysicalAddressChanged(
            UInt64 eventId,
            byte logicalAddress,
            string physicalAddress)
            : base(
                eventId,
                EventType.DcuPhysicalAddressChanged)
        {
            LogicalAddress = logicalAddress;
            PhysicalAddress = physicalAddress;
        }

        protected EventDcuPhysicalAddressChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Logical address: {0}, Physical address: {1}",
                    LogicalAddress,
                    PhysicalAddress));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.DCUPhysicalAddressChanged(
                DateTime,
                LogicalAddress,
                PhysicalAddress,
                ccuSettings.IPAddressString);
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
