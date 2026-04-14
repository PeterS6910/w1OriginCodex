using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Drivers.CardReader;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(468)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedForSetUnset : EventForAccessDenied
    {
        public Guid IdAlarmArea { get; private set; }

        protected EventCrAccessDeniedForSetUnset(
            EventType eventType,
            Guid guidCardReader,
            [NotNull] 
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                eventType,
                guidCardReader,
                accessData)
        {
            IdAlarmArea = idAlarmArea;
        }

        protected EventCrAccessDeniedForSetUnset()
        {

        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", AlarmArea: {0}",
                    IdAlarmArea));
        }
    }

    [LwSerialize(441)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedUnsetAlarmAreaNoRights : EventCrAccessDeniedForSetUnset
    {
        public EventCrAccessDeniedUnsetAlarmAreaNoRights(
            Guid guidCardReader,
            [NotNull] 
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                EventType.AccessDeniedUnsetAlarmAreaNoRights,
                guidCardReader,
                accessData,
                idAlarmArea)
        {
        }

        public EventCrAccessDeniedUnsetAlarmAreaNoRights()
        { 
        }
    }

    [LwSerialize(442)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedSetAlarmAreaNoRights : EventCrAccessDeniedForSetUnset
    {
        public EventCrAccessDeniedSetAlarmAreaNoRights(
            Guid guidCardReader,
            [NotNull] 
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                EventType.AccessDeniedSetAlarmAreaNoRights,
                guidCardReader,
                accessData,
                idAlarmArea)
        {
        }

        public EventCrAccessDeniedSetAlarmAreaNoRights()
        {
        }
    }

    [LwSerialize(443)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedSetAlarmAreaInvalidPin : EventCrAccessDeniedForSetUnset
    {
        public EventCrAccessDeniedSetAlarmAreaInvalidPin(
            Guid guidCardReader,
            [NotNull] 
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                EventType.AccessDeniedSetAlarmAreaInvalidPin,
                guidCardReader,
                accessData,
                idAlarmArea)
        {
        }

        public EventCrAccessDeniedSetAlarmAreaInvalidPin()
        {
        }
    }

    [LwSerialize(444)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedUnsetAlarmAreaInvalidPin : EventCrAccessDeniedForSetUnset
    {
        public EventCrAccessDeniedUnsetAlarmAreaInvalidPin(
            Guid guidCardReader,
            [NotNull] 
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                EventType.AccessDeniedUnsetAlarmAreaInvalidPin,
                guidCardReader,
                accessData,
                idAlarmArea)
        {
        }

        public EventCrAccessDeniedUnsetAlarmAreaInvalidPin()
        {
        }
    }

    [LwSerialize(445)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventsForCardReaderOnlineState : EventParametersWithState
    {
        public int DcuLogicalAddress { get; private set; }
        public string SerialPortName { get; private set; }
        public byte Address { get; private set; }
        public string ProtocolVersion { get; private set; }
        public string FirmwareVersion { get; private set; }
        public string HardwareVersion { get; private set; }
        public byte ProtocolVersionHigh { get; private set; }

        protected EventsForCardReaderOnlineState(
            EventType eventType,
            bool isOnline,
            int dcuLogicalAddress,
            string serialPortName,
            byte address,
            string protocolVersion,
            string firmwareVersion,
            string hardwareVersion,
            byte protocolVersionHigh)
            : base(
                eventType,
                isOnline
                    ? State.Online
                    : State.Offline)
        {
            DcuLogicalAddress = dcuLogicalAddress;
            SerialPortName = serialPortName;
            Address = address;
            ProtocolVersion = protocolVersion;
            FirmwareVersion = firmwareVersion;
            HardwareVersion = hardwareVersion;
            ProtocolVersionHigh = protocolVersionHigh;
        }

        protected EventsForCardReaderOnlineState()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", DCU: {0}, Serial port name: {1}, Address: {2}, Protocol version: {3}, Firmware version: {4}, Hardware version: {5}, Protocol version high: {6}",
                    DcuLogicalAddress,
                    SerialPortName,
                    Address,
                    ProtocolVersion,
                    FirmwareVersion,
                    HardwareVersion,
                    ProtocolVersionHigh));
        }
    }

    [LwSerialize(446)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderOnlineStateChanged : EventsForCardReaderOnlineState
    {
        public EventCardReaderOnlineStateChanged(
            bool isOnline,
            int dcuLogicalAddress,
            string serialPortName,
            byte address,
            string protocolVersion,
            string firmwareVersion,
            string hardwareVersion,
            byte protocolVersionHigh)
            : base(
                EventType.CardReaderOnlineStateChanged,
                isOnline,
                dcuLogicalAddress,
                serialPortName,
                address,
                protocolVersion,
                firmwareVersion,
                hardwareVersion,
                protocolVersionHigh)
        {

        }

        public EventCardReaderOnlineStateChanged()
        {
            
        }
    }

    [LwSerialize(447)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderOnlineStateInfo : EventsForCardReaderOnlineState
    {
        public EventCardReaderOnlineStateInfo(
            bool isOnline,
            int dcuLogicalAddress,
            string serialPortName,
            byte address,
            string protocolVersion,
            string firmwareVersion,
            string hardwareVersion,
            byte protocolVersionHigh)
            : base(
                EventType.CardReaderOnlineStateInfo,
                isOnline,
                dcuLogicalAddress,
                serialPortName,
                address,
                protocolVersion,
                firmwareVersion,
                hardwareVersion,
                protocolVersionHigh)
        {

        }

        public EventCardReaderOnlineStateInfo()
        {
            
        }
    }

    [LwSerialize(448)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderUpgradingState : EventParametersWithObjectId
    {
        public EventCardReaderUpgradingState(
            Guid idCardReader)
            : base(
                EventType.CardReaderLastCardChanged,
                idCardReader)
        {

        }

        public EventCardReaderUpgradingState()
        {
            
        }
    }

    [LwSerialize(449)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSectorCardSystemRemoved : EventParametersWithObjectId
    {
        public EventSectorCardSystemRemoved(Guid idCardSystem)
            : base(
                EventType.SectorCardSystemRemoved,
                idCardSystem)
        {

        }

        public EventSectorCardSystemRemoved()
        {
            
        }
    }

    [LwSerialize(450)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSectorCardSystemAdded : EventParametersWithObjectId
    {
        public EventSectorCardSystemAdded(Guid idCardSystem)
            : base(
                EventType.SectorCardSystemAdded,
                idCardSystem)
        {

        }

        public EventSectorCardSystemAdded()
        {
            
        }
    }

    [LwSerialize(451)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventForAccessDenied : EventParametersWithObjectId
    {
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }

        protected EventForAccessDenied(
            EventType eventType,
            Guid idCardReader,
            AccessDataBase accessData)
            : base(
                eventType,
                idCardReader)
        {
            IdCard = accessData.IdCard;
            IdPerson = accessData.IdPerson;
        }

        protected EventForAccessDenied(
            EventType eventType,
            Guid idCardReader)
            : base(
                eventType,
                idCardReader)
        {
        }

        protected EventForAccessDenied()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}, Person: {1}",
                    IdCard,
                    IdPerson));
        }
    }

    [LwSerialize(452)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDenied : EventForAccessDenied
    {
        public EventAccessDenied(
            Guid idCardReader,
            [NotNull]
            AccessDataBase accessData)
            : base(
                EventType.AccessDenied,
                idCardReader,
                accessData)
        {

        }

        public EventAccessDenied()
        {

        }
    }

    [LwSerialize(453)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedInvalidPin : EventForAccessDenied
    {
        public EventAccessDeniedInvalidPin(
            Guid idCardReader,
            [NotNull]
            AccessDataBase accessData)
            : base(
                EventType.AccessDeniedInvalidPin,
                idCardReader,
                accessData)
        {

        }

        public EventAccessDeniedInvalidPin()
        {
            
        }
    }

    [LwSerialize(454)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedInvalidEmergencyCode : EventForAccessDenied
    {
        public EventAccessDeniedInvalidEmergencyCode(Guid idCardReader)
            : base(
                EventType.AccessDeniedInvalidEmergencyCode,
                idCardReader,
                null)
        {

        }

        public EventAccessDeniedInvalidEmergencyCode()
        {
            
        }
    }

    [LwSerialize(455)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedInvalidCode : EventForAccessDenied
    {
        public EventAccessDeniedInvalidCode(Guid idCardReader)
            : base(
                EventType.AccessDeniedInvalidCode,
                idCardReader)
        {

        }

        public EventAccessDeniedInvalidCode()
        {
            
        }
    }

    [LwSerialize(456)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedCardBlockedOrInactive : EventForAccessDenied
    {
        public EventAccessDeniedCardBlockedOrInactive(
            Guid idCardReader,
            AccessDataBase accessData)
            : base(
                EventType.AccessDeniedCardBlockedOrInactive,
                idCardReader,
                accessData)
        {

        }

        public EventAccessDeniedCardBlockedOrInactive()
        {
            
        }
    }

    [LwSerialize(457)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventForAccessDeniedSetUnsetAlarmAreaInvalidCode : EventParametersWithObjectId
    {
        public Guid IdAlarmArea { get; private set; }

        protected EventForAccessDeniedSetUnsetAlarmAreaInvalidCode(
            EventType eventType,
            Guid idCardReader,
            Guid idAlarmArea)
            : base(
                eventType,
                idCardReader)
        {
            IdAlarmArea = idAlarmArea;
        }

        protected EventForAccessDeniedSetUnsetAlarmAreaInvalidCode()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Alarm area: {0}",
                    IdAlarmArea));
        }
    }

    [LwSerialize(458)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedUnsetAlarmAreaInvalidCode : EventForAccessDeniedSetUnsetAlarmAreaInvalidCode
    {
        public EventAccessDeniedUnsetAlarmAreaInvalidCode(
            Guid idCardReader,
            Guid idAlarmArea)
            : base(
                EventType.AccessDeniedUnsetAlarmAreaInvalidCode,
                idCardReader,
                idAlarmArea)
        {

        }

        public EventAccessDeniedUnsetAlarmAreaInvalidCode()
        {
            
        }
    }

    [LwSerialize(459)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedSetAlarmAreaInvalidCode : EventForAccessDeniedSetUnsetAlarmAreaInvalidCode
    {
        public EventAccessDeniedSetAlarmAreaInvalidCode(
            Guid idCardReader,
            Guid idAlarmArea)
            : base(
                EventType.AccessDeniedSetAlarmAreaInvalidCode,
                idCardReader,
                idAlarmArea)
        {

        }

        public EventAccessDeniedSetAlarmAreaInvalidCode()
        {
            
        }
    }

    [LwSerialize(460)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventUnknownCard : EventParametersWithObjectId
    {
        public string CardNumber { get; private set; }

        public EventUnknownCard(
            Guid idCardReader,
            string cardNumber)
            : base(
                EventType.UnknownCard,
                idCardReader)
        {
            CardNumber = cardNumber;
        }

        public EventUnknownCard()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card number: {0}",
                    CardNumber));
        }
    }

    [LwSerialize(461)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderCommandChanged : EventParametersWithObjectId
    {
        public byte CardReaderCommand { get; private set; }

        public EventCardReaderCommandChanged(
            Guid idCardReader,
            byte cardReaderCommand)
            : base(
                EventType.CardReaderCommandChanged,
                idCardReader)
        {
            CardReaderCommand = cardReaderCommand;
        }

        public EventCardReaderCommandChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card reader command: {0}",
                    CardReaderCommand));
        }
    }

    [LwSerialize(462)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderLastCardChanged : EventParametersWithObjectId
    {
        public string CardNumber { get; private set; }

        public EventCardReaderLastCardChanged(
            Guid idCardReader,
            string cardNumber)
            : base(
                EventType.CardReaderLastCardChanged,
                idCardReader)
        {
            CardNumber = cardNumber;
        }

        public EventCardReaderLastCardChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card number: {0}",
                    CardNumber));
        }
    }

    [LwSerialize(463)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventForSetUnsetAlarmAreaFromCardReader : EventParametersWithObjectId
    {
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }
        public Guid IdAlarmArea { get; private set; }

        protected EventForSetUnsetAlarmAreaFromCardReader(
            EventType eventType,
            Guid idCardReader,
            [NotNull]
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                eventType,
                idCardReader)
        {
            IdCard = accessData.IdCard;
            IdPerson = accessData.IdPerson;
            IdAlarmArea = idAlarmArea;
        }

        protected EventForSetUnsetAlarmAreaFromCardReader()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}, Person: {1}, Alarm area: {2}",
                    IdCard,
                    IdPerson,
                    IdAlarmArea));
        }
    }

    [LwSerialize(464)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSetAlarmAreaFromCardReader : EventForSetUnsetAlarmAreaFromCardReader
    {
        private class EventSetAlarmAreaFromCardReaderForCardReader :
            AEventForCardReader<EventSetAlarmAreaFromCardReader>
        {
            public EventSetAlarmAreaFromCardReaderForCardReader(
                EventSetAlarmAreaFromCardReader eventSetAlarmAreaFromCardReader)
                : base(eventSetAlarmAreaFromCardReader)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdAlarmArea);
            }

            public override State? EventState
            {
                get { return State.Set; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return new[]
                {
                    crEventlogProcessor.GetCardInformation(
                        EventParameters.IdCard)
                };
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get { yield return CrIconSymbol.SetAlarmArea; }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdAlarmArea)
                    || (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaSet(EventParameters.IdAlarmArea)
                        && !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnconditionalSet(EventParameters.IdAlarmArea)))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdAlarmArea,
                    1);
            }
        }

        public EventSetAlarmAreaFromCardReader(
            Guid idCardReader,
            [NotNull]
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                EventType.SetAlarmAreaFromCardReader,
                idCardReader,
                accessData,
                idAlarmArea)
        {

        }

        public EventSetAlarmAreaFromCardReader()
        {
            
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventSetAlarmAreaFromCardReaderForCardReader(this);
        }
    }

    [LwSerialize(465)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaSetFromCrFailed : EventForSetUnsetAlarmAreaFromCardReader
    {
        private class EventAlarmAreaSetFromCrFailedForCardReader :
            AEventForCardReader<EventAlarmAreaSetFromCrFailed>
        {
            public EventAlarmAreaSetFromCrFailedForCardReader(
                EventAlarmAreaSetFromCrFailed eventAlarmAreaSetFromCrFailed)
                : base(eventAlarmAreaSetFromCrFailed)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdAlarmArea);
            }

            public override State? EventState
            {
                get { return null; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return new[]
                {
                    crEventlogProcessor.GetCardInformation(
                        EventParameters.IdCard)
                };
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get
                {
                    yield return CrIconSymbol.SetAlarmArea;
                    yield return CrIconSymbol.ActionFailed;
                }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdAlarmArea)
                    || (!AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaSet(EventParameters.IdAlarmArea)
                        && !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnconditionalSet(EventParameters.IdAlarmArea)))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdAlarmArea,
                    1);
            }
        }

        public EventAlarmAreaSetFromCrFailed(
            Guid idCardReader,
            [NotNull]
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                EventType.AlarmAreaSetFromCRFailed,
                idCardReader,
                accessData,
                idAlarmArea)
        {

        }

        public EventAlarmAreaSetFromCrFailed()
        {
            
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventAlarmAreaSetFromCrFailedForCardReader(this);
        }
    }

    [LwSerialize(466)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventUnsetAlarmAreaFromCardReader : EventForSetUnsetAlarmAreaFromCardReader
    {
        private class EventUnsetAlarmAreaFromCardReaderForCardReader :
            AEventForCardReader<EventUnsetAlarmAreaFromCardReader>
        {
            public EventUnsetAlarmAreaFromCardReaderForCardReader(
                EventUnsetAlarmAreaFromCardReader eventUnsetAlarmAreaFromCardReader)
                : base(eventUnsetAlarmAreaFromCardReader)
            {
                
            }

            public override string GetEventObjectName(ICrEventlogDisplayContext eventlogDisplayContext)
            {
                return CrEventlogProcessor.GetNickNameForAlarmArea(
                    EventParameters.IdAlarmArea);
            }

            public override State? EventState
            {
                get { return State.Unset; }
            }

            public override string[] GetExtraParameters(CrEventlogProcessor crEventlogProcessor)
            {
                return new[]
                {
                    crEventlogProcessor.GetCardInformation(
                        EventParameters.IdCard)
                };
            }

            public override IEnumerable<CrIconSymbol> InlinedIcons
            {
                get { yield return CrIconSymbol.UnsetAlarmArea; }
            }

            protected override IEnumerable<Guid> InternalGetAlarmAreasForSavingEventToCrEventlog()
            {
                if (!AlarmAreas.Singleton.EnabledCrEventlogs(EventParameters.IdAlarmArea)
                    || !AlarmAreas.Singleton.EnabledCrEventlogsAlarmAreaUnset(EventParameters.IdAlarmArea))
                {
                    return null;
                }

                return Enumerable.Repeat(
                    EventParameters.IdAlarmArea,
                    1);
            }
        }

        public EventUnsetAlarmAreaFromCardReader(
            Guid idCardReader,
            [NotNull]
            AccessDataBase accessData,
            Guid idAlarmArea)
            : base(
                EventType.UnsetAlarmAreaFromCardReader,
                idCardReader,
                accessData,
                idAlarmArea)
        {

        }

        public EventUnsetAlarmAreaFromCardReader()
        {
            
        }

        public override IEventForCardReader CreateEventForCardReader()
        {
            return new EventUnsetAlarmAreaFromCardReaderForCardReader(this);
        }
    }

    [LwSerialize(467)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventFunctionKeyPressed : EventParametersWithObjectIdAndState
    {
        public Guid IdOutput { get; private set; }
        public Guid IdCard { get; private set; }

        public EventFunctionKeyPressed(
            Guid idCardReader,
            State state,
            Guid idOutput,
            Guid idCard)
            : base(
                EventType.FunctionKeyPressed,
                idCardReader,
                state)
        {
            IdOutput = idOutput;
            IdCard = idCard;
        }

        public EventFunctionKeyPressed()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Output: {0}, Card: {1}",
                    IdOutput,
                    IdCard));
        }
    }

    [LwSerialize(650)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToAaMenuInvalidPin : EventForAccessDenied
    {
        public EventCrAccessDeniedEnterToAaMenuInvalidPin(
            Guid idCardReader,
            [NotNull] AccessDataBase accessData)
            : base(
                EventType.AccessDeniedEnterToAaMenuInvalidPin,
                idCardReader,
                accessData)
        {
        }

        public EventCrAccessDeniedEnterToAaMenuInvalidPin()
        {
        }
    }

    [LwSerialize(651)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToAaMenuInvalidCode : EventParametersWithObjectId
    {
        public EventCrAccessDeniedEnterToAaMenuInvalidCode(
            Guid idCardReader)
            : base(
                EventType.AccessDeniedEnterToAaMenuInvalidCode,
                idCardReader)
        {

        }

        public EventCrAccessDeniedEnterToAaMenuInvalidCode()
        {

        }
    }

    [LwSerialize(652)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToSensorsMenuInvalidPin : EventForAccessDenied
    {
        public EventCrAccessDeniedEnterToSensorsMenuInvalidPin(
            Guid idCardReader,
            [NotNull]
            AccessDataBase accessData)
            : base(
                EventType.AccessDeniedEnterToSensorsMenuInvalidPin,
                idCardReader,
                accessData)
        {
        }

        public EventCrAccessDeniedEnterToSensorsMenuInvalidPin()
        {
        }
    }

    [LwSerialize(653)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToSensorsMenuInvalidCode : EventParametersWithObjectId
    {
        public EventCrAccessDeniedEnterToSensorsMenuInvalidCode(
            Guid idCardReader)
            : base(
                EventType.AccessDeniedEnterToSensorsMenuInvalidCode,
                idCardReader)
        {

        }

        public EventCrAccessDeniedEnterToSensorsMenuInvalidCode()
        {

        }
    }

    [LwSerialize(654)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToEventlogsMenuInvalidPin : EventForAccessDenied
    {
        public EventCrAccessDeniedEnterToEventlogsMenuInvalidPin(
            Guid idCardReader,
            [NotNull]
            AccessDataBase accessData)
            : base(
                EventType.AccessDeniedEnterToSensorsMenuInvalidPin,
                idCardReader,
                accessData)
        {

        }

        public EventCrAccessDeniedEnterToEventlogsMenuInvalidPin()
        {

        }
    }

    [LwSerialize(655)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToEventlogsMenuInvalidCode : EventParametersWithObjectId
    {
        public EventCrAccessDeniedEnterToEventlogsMenuInvalidCode(Guid idCardReader)
            : base(
                EventType.AccessDeniedEnterToSensorsMenuInvalidCode,
                idCardReader)
        {

        }

        public EventCrAccessDeniedEnterToEventlogsMenuInvalidCode()
        {

        }
    }

    [LwSerialize(656)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderBlockedStateChanged : EventParametersWithObjectId
    {
        public bool IsBlocked
        {
            get;
            private set;
        }

        public EventCardReaderBlockedStateChanged()
        {
        }

        public EventCardReaderBlockedStateChanged(
            Guid id,
            bool isBlocked) : base(EventType.CardReaderBlockedStateChanged, id)
        {
            IsBlocked = isBlocked;
        }
    }

    public static class TestCardReaderEvents
    {
        public static void EnqueueTestEvents(
            Guid idCardReader,
            Guid idCardSystem,
            ICard card,
            Guid idAlarmArea,
            Guid idOutput)
        {
            Events.ProcessEvent(new EventCrAccessDeniedUnsetAlarmAreaNoRights(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card), 
                idAlarmArea));

            Events.ProcessEvent(new EventCrAccessDeniedSetAlarmAreaNoRights(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card),
                idAlarmArea));

            Events.ProcessEvent(new EventCrAccessDeniedUnsetAlarmAreaInvalidPin(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card),
                idAlarmArea));

            Events.ProcessEvent(new EventCrAccessDeniedSetAlarmAreaInvalidPin(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card),
                idAlarmArea));

            Events.ProcessEvent(new EventCardReaderOnlineStateChanged(
                false,
                1,
                string.Empty,
                1,
                "Test",
                "Test",
                CRHWVersion.SmartPremiumCCR.ToString(),
                0));

            Events.ProcessEvent(new EventCardReaderOnlineStateInfo(
                false,
                1,
                string.Empty,
                1,
                "Test",
                "Test",
                CRHWVersion.SmartPremiumCCR.ToString(),
                0));

            Events.ProcessEvent(new EventCardReaderUpgradingState(
                idCardReader));

            Events.ProcessEvent(new EventSectorCardSystemRemoved(
                idCardSystem));

            Events.ProcessEvent(new EventSectorCardSystemAdded(
                idCardSystem));

            Events.ProcessEvent(new EventSectorCardSystemAdded(
                idCardSystem));

            Events.ProcessEvent(new EventAccessDenied(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card)));

            Events.ProcessEvent(new EventAccessDeniedInvalidPin(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card)));

            Events.ProcessEvent(new EventAccessDeniedInvalidEmergencyCode(
                idCardReader));

            Events.ProcessEvent(new EventAccessDeniedInvalidCode(idCardReader));

            Events.ProcessEvent(new EventAccessDeniedCardBlockedOrInactive(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card)));

            Events.ProcessEvent(new EventAccessDeniedUnsetAlarmAreaInvalidCode(
                idCardReader,
                idAlarmArea));

            Events.ProcessEvent(new EventAccessDeniedSetAlarmAreaInvalidCode(
                idCardReader,
                idAlarmArea));

            Events.ProcessEvent(new EventUnknownCard(
                idCardReader,
                "123456789012"));

            Events.ProcessEvent(new EventCardReaderCommandChanged(
                idCardReader,
                (byte) CardReaderSceneType.AlarmAreaSet));

            Events.ProcessEvent(new EventCardReaderLastCardChanged(
                idCardReader,
                "123456789012"));

            Events.ProcessEvent(new EventCardReaderLastCardChanged(
                idCardReader,
                "123456789012"));

            Events.ProcessEvent(new EventSetAlarmAreaFromCardReader(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card),
                idAlarmArea));

            Events.ProcessEvent(new EventAlarmAreaSetFromCrFailed(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card),
                idAlarmArea));

            Events.ProcessEvent(new EventUnsetAlarmAreaFromCardReader(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card),
                idAlarmArea));

            Events.ProcessEvent(new EventFunctionKeyPressed(
                idCardReader,
                State.On,
                idOutput,
                card.IdCard));

            Events.ProcessEvent(new EventCrAccessDeniedEnterToAaMenuInvalidPin(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card)));

            Events.ProcessEvent(new EventCrAccessDeniedEnterToAaMenuInvalidCode(
                idCardReader));

            Events.ProcessEvent(new EventCrAccessDeniedEnterToSensorsMenuInvalidPin(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card)));

            Events.ProcessEvent(new EventCrAccessDeniedEnterToSensorsMenuInvalidCode(
                idCardReader));

            Events.ProcessEvent(new EventCrAccessDeniedEnterToEventlogsMenuInvalidPin(
                idCardReader,
                new TestDoorEnvironmentEvents.CardAccessData(card)));

            Events.ProcessEvent(new EventCrAccessDeniedEnterToEventlogsMenuInvalidCode(
                idCardReader));

            Events.ProcessEvent(new EventCardReaderBlockedStateChanged(
                idCardReader,
                true));
        }
    }
}
