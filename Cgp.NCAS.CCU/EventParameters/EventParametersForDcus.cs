using System;
using System.Text;
using Contal.Cgp.Globals;
using Contal.Drivers.ClspDrivers;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(470)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuCommandNotAck : EventParametersWithObjectId
    {
        public string ErrorCode { get; private set; }
        public string Command { get; private set; }

        public EventDcuCommandNotAck(
            Guid idDcu,
            string errorCode,
            string command)
            : base(
                EventType.DcuCommandNotACK,
                idDcu)
        {
            ErrorCode = errorCode;
            Command = command;
        }

        public EventDcuCommandNotAck()
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
    }

    [LwSerialize(471)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuCommandTimeOut : EventParametersWithObjectId
    {
        public string Command { get; private set; }

        public EventDcuCommandTimeOut(
            Guid idDcu,
            string command)
            : base(
                EventType.DcuCommandTimeOut,
                idDcu)
        {
            Command = command;
        }

        public EventDcuCommandTimeOut()
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
    }

    [LwSerialize(472)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuNodeAssigned : EventParametersWithObjectId
    {
        public EventDcuNodeAssigned(
            Guid idDcu)
            : base(
                EventType.DcuNodeAssigned,
                idDcu)
        {

        }

        public EventDcuNodeAssigned()
        {
            
        }
    }

    [LwSerialize(473)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuNodeReleased : EventParametersWithObjectId
    {
        public EventDcuNodeReleased(
            Guid idDcu)
            : base(
                EventType.DcuNodeReleased,
                idDcu)
        {

        }

        public EventDcuNodeReleased()
        {
            
        }
    }

    [LwSerialize(474)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuNodeRenewed : EventParametersWithObjectId
    {
        public EventDcuNodeRenewed(
            Guid idDcu)
            : base(
                EventType.DcuNodeRenewed,
                idDcu)
        {

        }

        public EventDcuNodeRenewed()
        {
            
        }
    }

    [LwSerialize(475)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuInputsSabotage : EventParametersWithObjectIdAndState
    {
        public EventDcuInputsSabotage(
            State sabotageState,
            Guid idDcu)
            : base(
                EventType.DcuInputsSabotage,
                idDcu,
                sabotageState)
        {

        }

        public EventDcuInputsSabotage()
        {
            
        }
    }

    [LwSerialize(476)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuFirmwareVersion : EventParametersWithObjectId
    {
        public string FirmwareVersion { get; private set; }

        public EventDcuFirmwareVersion(
            Guid idDcu,
            string firmwareVersion)
            : base(
                EventType.DcuFirmwareVersion,
                idDcu)
        {
            FirmwareVersion = firmwareVersion;
        }

        public EventDcuFirmwareVersion()
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
    }

    [LwSerialize(477)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuMemoryWarning : EventParametersWithObjectId
    {
        public byte Memory { get; private set; }

        public EventDcuMemoryWarning(
            Guid idDcu,
            byte memory)
            : base(
                EventType.DcuMemoryWarning,
                idDcu)
        {
            Memory = memory;
        }

        public EventDcuMemoryWarning()
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
    }

    [LwSerialize(478)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuInputCount : EventParametersWithObjectId
    {
        public byte InputCount { get; private set; }

        public EventDcuInputCount(
            Guid idDcu,
            byte inputCount)
            : base(
                EventType.DcuInputCount,
                idDcu)
        {
            InputCount = inputCount;
        }

        public EventDcuInputCount()
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
    }

    [LwSerialize(479)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuOutputCount : EventParametersWithObjectId
    {
        public byte OutputCount { get; private set; }

        public EventDcuOutputCount(
            Guid idDcu,
            byte outputCount)
            : base(
                EventType.DcuOutputCount,
                idDcu)
        {
            OutputCount = outputCount;
        }

        public EventDcuOutputCount()
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
    }

    [LwSerialize(480)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuOnlineStateChanged : EventParametersWithState
    {
        public byte LogicalAddress { get; private set; }
        public byte ProtocolId { get; private set; }

        public EventDcuOnlineStateChanged(
            State onlineState,
            byte logicalAddress,
            byte protocolId)
            : base(
                EventType.DcuOnlineStateChanged,
                onlineState)
        {
            LogicalAddress = logicalAddress;
            ProtocolId = protocolId;
        }

        public EventDcuOnlineStateChanged()
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
    }

    [LwSerialize(481)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventDcuPhysicalAddressChanged : EventParameters
    {
        public byte LogicalAddress { get; private set; }
        public string PhysicalAddress { get; private set; }

        public EventDcuPhysicalAddressChanged(
            byte logicalAddress,
            string physicalAddress)
            : base(
                EventType.DcuPhysicalAddressChanged)
        {
            LogicalAddress = logicalAddress;
            PhysicalAddress = physicalAddress;
        }

        public EventDcuPhysicalAddressChanged()
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
    }

    public static class TestDcuEvents
    {
        public static void EnqueueTestEvents(
            Guid idDcu)
        {
            Events.ProcessEvent(
                new EventDcuCommandNotAck(
                    idDcu,
                    "Test",
                    "Test command"));

            Events.ProcessEvent(
                new EventDcuCommandTimeOut(
                    idDcu,
                    "Test command"));

            Events.ProcessEvent(
                new EventDcuNodeAssigned(
                    idDcu));

            Events.ProcessEvent(
                new EventDcuNodeReleased(
                    idDcu));

            Events.ProcessEvent(
                new EventDcuNodeRenewed(
                    idDcu));

            Events.ProcessEvent(
                new EventDcuInputsSabotage(
                    State.Alarm,
                    idDcu));

            Events.ProcessEvent(
                new EventDcuFirmwareVersion(
                    idDcu,
                    "Test version"));

            Events.ProcessEvent(
                new EventDcuMemoryWarning(
                    idDcu,
                    80));

            Events.ProcessEvent(
                new EventDcuInputCount(
                    idDcu,
                    4));

            Events.ProcessEvent(
                new EventDcuOutputCount(
                    idDcu,
                    4));

            Events.ProcessEvent(
                new EventDcuOnlineStateChanged(
                    State.Offline,
                    1,
                    (byte)ProtocolId.InvalidProtocol));

            Events.ProcessEvent(
                new EventDcuPhysicalAddressChanged(
                    1,
                    "Test address"));
        }
    }
}
