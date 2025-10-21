using System;
using System.Collections.Generic;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(468)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventCrAccessDeniedForSetUnset : EventForAccessDenied
    {
        public Guid IdAlarmArea { get; private set; }

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
        protected EventCrAccessDeniedUnsetAlarmAreaNoRights()
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
            var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);

            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            if (IdCard != Guid.Empty)
            {
                var card = Cards.Singleton.GetById(IdCard);

                if (card == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                string personName;

                var eventSourcesIdsWithCard =
                    CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        alarmArea,
                        out personName);

                return
                    Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPEACCESSDENIEDUNSETALARMAREANORIGHTS,
                        DateTime,
                        ccuSettings.CCUEvents.ThisAssemblyName,
                        eventSourcesIdsWithCard,
                        string.Format(
                            "Access dennied unset alarm area no rights, card: {0}, person: {1}, alarm area: {2}",
                            card.GetFullCardNumber(),
                            personName,
                            alarmArea),
                        out eventlog,
                        out eventSources,
                        out eventlogParameters,
                        EventlogParameter.TYPECARDNUMBER,
                        card.GetFullCardNumber(),
                        EventlogParameter.TYPEPERSONNAME,
                        personName,
                        EventlogParameter.TYPEPEALARMAREANAME,
                        alarmArea.ToString());
            }

            var person = Persons.Singleton.GetById(IdPerson);

            if (person == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIdsWithPerson =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    person,
                    alarmArea);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDUNSETALARMAREANORIGHTS,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIdsWithPerson,
                    string.Format(
                        "Access dennied unset alarm area no rights, person: {0}, alarm area: {1}",
                        person,
                        alarmArea),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPEPERSONNAME,
                    person.ToString(),
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
        }
    }

    [LwSerialize(442)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedSetAlarmAreaNoRights : EventCrAccessDeniedForSetUnset
    {
        protected EventCrAccessDeniedSetAlarmAreaNoRights()
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
            var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);

            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var cardReader = CardReaders.Singleton.GetById(IdObject);
            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            if (IdCard != Guid.Empty)
            {
                var card = Cards.Singleton.GetById(IdCard);

                if (card == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                string personName;

                var eventSourcesIdsWithCard =
                    CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        alarmArea,
                        out personName);

                return
                    Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPEACCESSDENIEDSETALARMAREANORIGHTS,
                        DateTime,
                        ccuSettings.CCUEvents.ThisAssemblyName,
                        eventSourcesIdsWithCard,
                        string.Format(
                            "Access dennied set alarm area no rights, card: {0}, person: {1}, alarm area: {2}",
                            card.GetFullCardNumber(),
                            personName,
                            alarmArea),
                        out eventlog,
                        out eventSources,
                        out eventlogParameters,
                        EventlogParameter.TYPECARDNUMBER,
                        card.GetFullCardNumber(),
                        EventlogParameter.TYPEPERSONNAME,
                        personName,
                        EventlogParameter.TYPEPEALARMAREANAME,
                        alarmArea.ToString());
            }

            var person = Persons.Singleton.GetById(IdPerson);

            if (person == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIdsWithPerson =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    person,
                    alarmArea);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDSETALARMAREANORIGHTS,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIdsWithPerson,
                    string.Format(
                        "Access dennied set alarm area no rights, person: {0}, alarm area: {1}",
                        person,
                        alarmArea),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPEPERSONNAME,
                    person.ToString(),
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
        }
    }

    [LwSerialize(443)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedSetAlarmAreaInvalidPin : EventCrAccessDeniedForSetUnset
    {
        protected EventCrAccessDeniedSetAlarmAreaInvalidPin()
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
            var card = Cards.Singleton.GetById(IdCard);

            if (card == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;
            IEnumerable<Guid> eventSourcesIds;

            if (IdAlarmArea != Guid.Empty)
            {
                var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);

                if (alarmArea == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                eventSourcesIds =
                    CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        alarmArea,
                        out personName);

                return
                    Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDPIN,
                        DateTime,
                        ccuSettings.CCUEvents.ThisAssemblyName,
                        eventSourcesIds,
                        string.Format(
                            "Access dennied set alarm area invalid pin, card: {0}, person: {1}, alarm area: {2}",
                            card.GetFullCardNumber(),
                            personName,
                            alarmArea),
                        out eventlog,
                        out eventSources,
                        out eventlogParameters,
                        EventlogParameter.TYPECARDNUMBER,
                        card.GetFullCardNumber(),
                        EventlogParameter.TYPEPERSONNAME,
                        personName,
                        EventlogParameter.TYPEPEALARMAREANAME,
                        alarmArea.ToString());
            }

            eventSourcesIds =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    false,
                    card,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDPIN,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    string.Format(
                        "Access dennied set all alarm areas invalid pin, card: {0}, person: {1}",
                        card.GetFullCardNumber(),
                        personName),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName);
        }
    }

    [LwSerialize(444)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedUnsetAlarmAreaInvalidPin : EventCrAccessDeniedForSetUnset
    {
        protected EventCrAccessDeniedUnsetAlarmAreaInvalidPin()
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
            var card = Cards.Singleton.GetById(IdCard);

            if (card == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;
            IEnumerable<Guid> eventSourcesIds;

            if (IdAlarmArea != Guid.Empty)
            {
                var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);

                if (alarmArea == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                eventSourcesIds =
                    CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        alarmArea,
                        out personName);

                return
                    Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN,
                        DateTime,
                        ccuSettings.CCUEvents.ThisAssemblyName,
                        eventSourcesIds,
                        string.Format(
                            "Access dennied unset alarm area invalid pin, card: {0}, person: {1}, alarm area: {2}",
                            card.GetFullCardNumber(),
                            personName,
                            alarmArea),
                        out eventlog,
                        out eventSources,
                        out eventlogParameters,
                        EventlogParameter.TYPECARDNUMBER,
                        card.GetFullCardNumber(),
                        EventlogParameter.TYPEPERSONNAME,
                        personName,
                        EventlogParameter.TYPEPEALARMAREANAME,
                        alarmArea.ToString());
            }

            eventSourcesIds =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    false,
                    card,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    string.Format(
                        "Access dennied unset all alarm areas invalid pin, card: {0}, person: {1}",
                        card.GetFullCardNumber(),
                        personName),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName);
        }
    }

    [LwSerialize(445)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventsForCardReaderOnlineState : EventParametersWithState
    {
        public int DcuLogicalAddress { get; private set; }
        public string SerialPortName { get; private set; }
        public byte Address { get; private set; }
        public string ProtocolVersion { get; private set; }
        public string FirmwareVersion { get; private set; }
        public string HardwareVersion { get; private set; }
        public byte ProtocolVersionHigh { get; private set; }

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
        protected EventCardReaderOnlineStateChanged()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var cardReader = CCUConfigurationHandler.Singleton.CardReaderOnlineStateChanged(
                DateTime,
                DcuLogicalAddress,
                new CCUConfigurationHandler.CardReaderCreationParams
                {
                    port = SerialPortName,
                    address = Address,
                    onlineState = State == State.Online,
                    protocolVersion = ProtocolVersion,
                    firmwareVersion = FirmwareVersion,
                    hardwareVersion = HardwareVersion,
                    protocolMajor = ProtocolVersionHigh,
                },
                ccuSettings.IPAddressString);

            if (cardReader == null)
                return;

            CCUConfigurationHandler.CreateCardReaderOnlineStateChangedEventlog(
                cardReader,
                DateTime,
                CCUConfigurationHandler.ConvertToOnlineStateFromState(State));
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

    [LwSerialize(447)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderOnlineStateInfo : EventsForCardReaderOnlineState
    {
        protected EventCardReaderOnlineStateInfo()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CardReaderOnlineStateChanged(
                DateTime,
                DcuLogicalAddress,
                new CCUConfigurationHandler.CardReaderCreationParams
                {
                    port = SerialPortName,
                    address = Address,
                    onlineState = State == State.Online,
                    protocolVersion = ProtocolVersion,
                    firmwareVersion = FirmwareVersion,
                    hardwareVersion = HardwareVersion,
                    protocolMajor = ProtocolVersionHigh,
                },
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

    [LwSerialize(448)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderUpgradingState : EventParametersWithObjectId
    {
        public EventCardReaderUpgradingState(
            UInt64 eventId,
            Guid idCardReader)
            : base(
                eventId,
                EventType.CardReaderOnlineStateChanged,
                idCardReader)
        {

        }

        protected EventCardReaderUpgradingState()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.SetCrStateUpgrading(
                ccuSettings.GuidCCU,
                IdObject,
                true);
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

    [LwSerialize(449)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSectorCardSystemRemoved : EventParametersWithObjectId
    {
        public EventSectorCardSystemRemoved(
            UInt64 eventId,
            Guid idCardSystem)
            : base(
                eventId,
                EventType.SectorCardSystemRemoved,
                idCardSystem)
        {

        }

        protected EventSectorCardSystemRemoved()
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
            var cardSystem = CardSystems.Singleton.GetById(IdObject);
            if (cardSystem == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds =
                new List<Guid>
                {
                    cardSystem.IdCardSystem
                };

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);
            if (ccu != null)
                eventSourcesIds.Add(ccu.IdCCU);

            var description = new StringBuilder();

            description.AppendFormat(
                "Card system  {0} was removed",
                cardSystem);

            if (ccu != null)
                description.AppendFormat(" from CCU: {0}", ccu);

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CARDSYSTEM_REMOVED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description.ToString(),
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(450)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSectorCardSystemAdded : EventParametersWithObjectId
    {
        public EventSectorCardSystemAdded(
            UInt64 eventId,
            Guid idCardSystem)
            : base(
                eventId,
                EventType.SectorCardSystemAdded,
                idCardSystem)
        {

        }

        protected EventSectorCardSystemAdded()
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
            var cardSystem = CardSystems.Singleton.GetById(IdObject);
            if (cardSystem == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds =
                new List<Guid>
                {
                    cardSystem.IdCardSystem
                };

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);
            if (ccu != null)
                eventSourcesIds.Add(ccu.IdCCU);

            var description = new StringBuilder();

            description.AppendFormat(
                "Card system  {0} was added",
                cardSystem);

            if (ccu != null)
                description.AppendFormat(
                    " to CCU: {0}",
                    ccu);

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CARDSYSTEM_ADDED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description.ToString(),
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(451)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventForAccessDenied : EventParametersWithObjectId
    {
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }

        protected EventForAccessDenied(
            UInt64 eventId,
            EventType eventType,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson)
            : base(
                eventId,
                eventType,
                idCardReader)
        {
            IdCard = idCard;
            IdPerson = idPerson;
        }

        protected EventForAccessDenied()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}, Person: {0}",
                    IdCard,
                    IdPerson));
        }
    }

    [LwSerialize(452)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDenied : EventForAccessDenied
    {
        public EventAccessDenied(
            UInt64 eventId,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson)
            : base(
                eventId,
                EventType.AccessDenied,
                idCardReader,
                idCard,
                idPerson)
        {

        }

        protected EventAccessDenied()
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
            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            if (IdCard == Guid.Empty)
            {
                var person = Persons.Singleton.GetById(IdPerson);

                if (person == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                var eventSourcesIdsWithPerson =
                    CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        true,
                        person);

                return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEACCESSDENIED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIdsWithPerson,
                    string.Format(
                        "Access dennied person: {0}",
                        person),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPEPERSONNAME,
                    person.ToString());
            }

            var card = Cards.Singleton.GetById(IdCard);

            if (card == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;

            var eventSourcesIdsWithCard =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    true,
                    card,
                    out personName);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEACCESSDENIED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIdsWithCard,
                string.Format(
                    "Access dennied card: {0}, person: {1}",
                    card.GetFullCardNumber(),
                    personName),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPECARDNUMBER,
                card.GetFullCardNumber(),
                EventlogParameter.TYPEPERSONNAME,
                personName);
        }
    }

    [LwSerialize(453)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedInvalidPin : EventForAccessDenied
    {
        public EventAccessDeniedInvalidPin(
            UInt64 eventId,
            Guid idCardReader,
            Guid idCard)
            : base(
                eventId,
                EventType.AccessDeniedInvalidPin,
                idCardReader,
                idCard,
                Guid.Empty)
        {

        }

        protected EventAccessDeniedInvalidPin()
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
            var cardReader = CardReaders.Singleton.GetById(IdObject);
            var card = Cards.Singleton.GetById(IdCard);

            if (cardReader == null
                || card == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;

            var eventSourcesIds =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    true,
                    card,
                    out personName);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEACCESSDENIEDINVALIDPIN,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format(
                    "Access dennied invalid PIN, card: {0}, person: {1}",
                    card.GetFullCardNumber(),
                    personName),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPECARDNUMBER,
                card.GetFullCardNumber(),
                EventlogParameter.TYPEPERSONNAME,
                personName);
        }
    }

    [LwSerialize(454)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedInvalidEmergencyCode : EventForAccessDenied
    {
        public EventAccessDeniedInvalidEmergencyCode(
            UInt64 eventId,
            Guid idCardReader)
            : base(
                eventId,
                EventType.AccessDeniedInvalidEmergencyCode,
                idCardReader,
                Guid.Empty,
                Guid.Empty)
        {

        }

        protected EventAccessDeniedInvalidEmergencyCode()
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
            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEACCESSDENIEDINVALIDEMERGENCYCODE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    true),
                "Access dennied invalid emergency code",
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(455)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedInvalidCode : EventForAccessDenied
    {
        public EventAccessDeniedInvalidCode(
            UInt64 eventId,
            Guid idCardReader)
            : base(
                eventId,
                EventType.AccessDeniedInvalidCode,
                idCardReader,
                Guid.Empty,
                Guid.Empty)
        {

        }

        protected EventAccessDeniedInvalidCode()
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
            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEACCESSDENIEDINVALIDCODE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    true),
                "Access dennied invalid CODE",
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(456)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedCardBlockedOrInactive : EventForAccessDenied
    {
        public EventAccessDeniedCardBlockedOrInactive(
            UInt64 eventId,
            Guid idCardReader,
            Guid idCard)
            : base(
                eventId,
                EventType.AccessDeniedCardBlockedOrInactive,
                idCardReader,
                idCard,
                Guid.Empty)
        {

        }

        protected EventAccessDeniedCardBlockedOrInactive()
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
            var cardReader = CardReaders.Singleton.GetById(IdObject);
            var card = Cards.Singleton.GetById(IdCard);

            if (cardReader == null
                || card == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;

            var eventSourcesIds =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    true,
                    card,
                    out personName);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format(
                    "Access dennied card blocked or inactive, card: {0}, person: {1}",
                    card.GetFullCardNumber(),
                    personName),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPECARDNUMBER,
                card.GetFullCardNumber(),
                EventlogParameter.TYPEPERSONNAME,
                personName);
        }
    }

    [LwSerialize(457)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventForAccessDeniedSetUnsetAlarmAreaInvalidCode : EventParametersWithObjectId
    {
        public Guid IdAlarmArea { get; private set; }

        protected EventForAccessDeniedSetUnsetAlarmAreaInvalidCode(
            UInt64 eventId,
            EventType eventType,
            Guid idCardReader,
            Guid idAlarmArea)
            : base(
                eventId,
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            
        }
    }

    [LwSerialize(458)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedUnsetAlarmAreaInvalidCode : EventForAccessDeniedSetUnsetAlarmAreaInvalidCode
    {
        public EventAccessDeniedUnsetAlarmAreaInvalidCode(
            UInt64 eventId,
            Guid idCardReader,
            Guid idAlarmArea)
            : base(
                eventId,
                EventType.AccessDeniedUnsetAlarmAreaInvalidCode,
                idCardReader,
                idAlarmArea)
        {

        }

        protected EventAccessDeniedUnsetAlarmAreaInvalidCode()
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var cardReader = CardReaders.Singleton.GetById(IdObject);
            var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);
            if (cardReader == null
                || alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDCODE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(cardReader, alarmArea),
                string.Format(
                    "Access dennied unset alarm area invalid code, alarm area: {0}",
                    alarmArea),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPEPEALARMAREANAME,
                alarmArea.ToString());
        }
    }
    
    [LwSerialize(459)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAccessDeniedSetAlarmAreaInvalidCode : EventForAccessDeniedSetUnsetAlarmAreaInvalidCode
    {
        public EventAccessDeniedSetAlarmAreaInvalidCode(
            UInt64 eventId,
            Guid idCardReader,
            Guid idAlarmArea)
            : base(
                eventId,
                EventType.AccessDeniedSetAlarmAreaInvalidCode,
                idCardReader,
                idAlarmArea)
        {

        }

        protected EventAccessDeniedSetAlarmAreaInvalidCode()
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var cardReader = CardReaders.Singleton.GetById(IdObject);
            var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);

            if (cardReader == null
                || alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDCODE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    alarmArea),
                string.Format(
                    "Access dennied set alarm area invalid code, alarm area: {0}",
                    alarmArea),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPEPEALARMAREANAME,
                alarmArea.ToString());
        }
    }

    [LwSerialize(460)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventUnknownCard : EventParametersWithObjectId
    {
        public string CardNumber { get; private set; }

        public EventUnknownCard(
            UInt64 eventId,
            Guid idCardReader,
            string cardNumber)
            : base(
                eventId,
                EventType.UnknownCard,
                idCardReader)
        {
            CardNumber = cardNumber;
        }

        protected EventUnknownCard()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {

        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var cardReader = CardReaders.Singleton.GetById(IdObject);
            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;

            ICollection<Guid> eventSourcesIds = CcuEvents.GetEventSourcesFromCardReader(
                cardReader,
                true,
                Cards.Singleton.GetCardFromFullCardNumber(CardNumber),
                out personName);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEUNKNOWNCARD,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                string.Format("Unknown card: {0}", CardNumber),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPECARDNUMBER,
                CardNumber);
        }
    }

    [LwSerialize(461)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderCommandChanged : EventParametersWithObjectId
    {
        public byte CardReaderCommand { get; private set; }

        public EventCardReaderCommandChanged(
            UInt64 eventId,
            Guid idCardReader,
            byte cardReaderCommand)
            : base(
                eventId,
                EventType.CardReaderCommandChanged,
                idCardReader)
        {
            CardReaderCommand = cardReaderCommand;
        }

        protected EventCardReaderCommandChanged()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CardReaderCommandChanged(
                IdObject,
                DateTime,
                CardReaderCommand);
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

    [LwSerialize(462)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCardReaderLastCardChanged : EventParametersWithObjectId
    {
        public string CardNumber { get; private set; }

        public EventCardReaderLastCardChanged(
            UInt64 eventId,
            Guid idCardReader,
            string cardNumber)
            : base(
                eventId,
                EventType.CardReaderLastCardChanged,
                idCardReader)
        {
            CardNumber = cardNumber;
        }

        protected EventCardReaderLastCardChanged()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CardReaderLastCardChanged(
                IdObject,
                DateTime,
                CardNumber,
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

    [LwSerialize(463)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventForSetUnsetAlarmAreaFromCardReader : EventParametersWithObjectId
    {
        public Guid IdCard { get; private set; }
        public Guid IdPerson { get; private set; }
        public Guid IdAlarmArea { get; private set; }

        protected EventForSetUnsetAlarmAreaFromCardReader(
            UInt64 eventId,
            EventType eventType,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idAlarmArea)
            : base(
                eventId,
                eventType,
                idCardReader)
        {
            IdCard = idCard;
            IdPerson = IdPerson;
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            
        }
    }

    [LwSerialize(464)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventSetAlarmAreaFromCardReader : EventForSetUnsetAlarmAreaFromCardReader
    {
        public EventSetAlarmAreaFromCardReader(
            UInt64 eventId,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idAlarmArea)
            : base(
                eventId,
                EventType.SetAlarmAreaFromCardReader,
                idCardReader,
                idCard,
                idPerson,
                idAlarmArea)
        {

        }

        protected EventSetAlarmAreaFromCardReader()
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var cardReader = CardReaders.Singleton.GetById(IdObject);
            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            CCU ccu = null;
            if (cardReader.CCU != null)
                ccu = cardReader.CCU;
            else if (cardReader.DCU != null)
                ccu = cardReader.DCU.CCU;

            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);
            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            if (IdCard != Guid.Empty
                || IdPerson != Guid.Empty)
            {
                if (IdCard != Guid.Empty)
                {
                    var card = Cards.Singleton.GetById(IdCard);
                    if (card == null)
                    {
                        eventlog = null;
                        eventSources = null;
                        eventlogParameters = null;

                        return false;
                    }

                    string personName;

                    var eventSourcesIdsWithCard =
                        CcuEvents.GetEventSourcesFromCardReader(
                            cardReader,
                            card,
                            alarmArea,
                            out personName);

                    return Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPESETALARMAREAFROMCARDREADER,
                        DateTime,
                        ccuSettings.CCUEvents.ThisAssemblyName,
                        eventSourcesIdsWithCard,
                        string.Format(
                            "Set alarm area from card reader, card reader: {0}, card: {1}, person: {2}, alarm area: {3}",
                            cardReader.Name,
                            card.GetFullCardNumber(),
                            personName,
                            alarmArea),
                        out eventlog,
                        out eventSources,
                        out eventlogParameters,
                        EventlogParameter.TYPECARDNUMBER,
                        card.GetFullCardNumber(),
                        EventlogParameter.TYPEPERSONNAME,
                        personName,
                        EventlogParameter.TYPEPEALARMAREANAME,
                        alarmArea.ToString());
                }

                var person = Persons.Singleton.GetById(IdPerson);
                if (person == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                var eventSourcesIdsWithPerson =
                    CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        person,
                        alarmArea);

                return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPESETALARMAREAFROMCARDREADER,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIdsWithPerson,
                    string.Format(
                        "Set alarm area from card reader, card reader: {0}, person: {1}, alarm area: {2}",
                        cardReader.Name,
                        person,
                        alarmArea),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPEPERSONNAME,
                    person.ToString(),
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
            }


            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPESETALARMAREAFROMCARDREADER,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    alarmArea),
                string.Format(
                    "Set alarm area from card reader with code, card reader: {0}, alarm area: {1}",
                    cardReader.Name,
                    alarmArea),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPEPEALARMAREANAME,
                alarmArea.ToString());
        }
    }

    [LwSerialize(465)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmAreaSetFromCrFailed : EventForSetUnsetAlarmAreaFromCardReader
    {
        public EventAlarmAreaSetFromCrFailed(
            UInt64 eventId,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idAlarmArea)
            : base(
                eventId,
                EventType.AlarmAreaSetFromCRFailed,
                idCardReader,
                idCard,
                idPerson,
                idAlarmArea)
        {

        }

        protected EventAlarmAreaSetFromCrFailed()
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var cardReader = CardReaders.Singleton.GetById(IdObject);
            var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);

            if (cardReader == null
                || alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            if (IdCard != Guid.Empty)
            {
                var card = Cards.Singleton.GetById(IdCard);

                if (card == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                string personName;

                var eventSourcesIdsWithCard =
                    CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        card,
                        alarmArea,
                        out personName);

                return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ALARM_AREA_SET_FROM_CR_FAILED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIdsWithCard,
                    string.Format(
                        "Set alarm area from card reader failed, card reader: {0}, card: {1}, person: {2}, alarm area: {3}",
                        cardReader.Name,
                        card.GetFullCardNumber(),
                        personName,
                        alarmArea),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName,
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
            }

            var person = Persons.Singleton.GetById(IdPerson);

            if (person == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIdsWithPerson =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    person,
                    alarmArea);

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ALARM_AREA_SET_FROM_CR_FAILED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIdsWithPerson,
                string.Format(
                    "Set alarm area from card reader failed, card reader: {0}, person: {1}, alarm area: {2}",
                    cardReader.Name,
                    person,
                    alarmArea),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPEPERSONNAME,
                person.ToString(),
                EventlogParameter.TYPEPEALARMAREANAME,
                alarmArea.ToString());
        }
    }

    [LwSerialize(466)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventUnsetAlarmAreaFromCardReader : EventForSetUnsetAlarmAreaFromCardReader
    {
        public EventUnsetAlarmAreaFromCardReader(
            UInt64 eventId,
            Guid idCardReader,
            Guid idCard,
            Guid idPerson,
            Guid idAlarmArea)
            : base(
                eventId,
                EventType.UnsetAlarmAreaFromCardReader,
                idCardReader,
                idCard,
                idPerson,
                idAlarmArea)
        {

        }

        protected EventUnsetAlarmAreaFromCardReader()
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            CCU ccu = null;
            if (cardReader.CCU != null)
                ccu = cardReader.CCU;
            else if (cardReader.DCU != null)
                ccu = cardReader.DCU.CCU;

            if (ccu == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var alarmArea = AlarmAreas.Singleton.GetById(IdAlarmArea);

            if (alarmArea == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            if (IdCard != Guid.Empty
                || IdPerson != Guid.Empty)
            {
                if (IdCard != Guid.Empty)
                {
                    var card = Cards.Singleton.GetById(IdCard);

                    if (card == null)
                    {
                        eventlog = null;
                        eventSources = null;
                        eventlogParameters = null;

                        return false;
                    }

                    string personName;

                    var eventSourcesIdsWithCard =
                        CcuEvents.GetEventSourcesFromCardReader(
                            cardReader,
                            card,
                            alarmArea,
                            out personName);

                    return Eventlogs.Singleton.CreateEvent(
                        Eventlog.TYPEUNSETALARMAREAFROMCARDREADER,
                        DateTime,
                        ccuSettings.CCUEvents.ThisAssemblyName,
                        eventSourcesIdsWithCard,
                        string.Format(
                            "Unset alarm area from card reader, card reader: {0}, card: {1}, person: {2}, alarm area: {3}",
                            cardReader.Name,
                            card.GetFullCardNumber(),
                            personName,
                            alarmArea),
                        out eventlog,
                        out eventSources,
                        out eventlogParameters,
                        EventlogParameter.TYPECARDNUMBER,
                        card.GetFullCardNumber(),
                        EventlogParameter.TYPEPERSONNAME,
                        personName,
                        EventlogParameter.TYPEPEALARMAREANAME,
                        alarmArea.ToString());
                }

                var person = Persons.Singleton.GetById(IdCard);

                if (person == null)
                {
                    eventlog = null;
                    eventSources = null;
                    eventlogParameters = null;

                    return false;
                }

                var eventSourcesIdsWithPerson =
                    CcuEvents.GetEventSourcesFromCardReader(
                        cardReader,
                        person,
                        alarmArea);

                return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPEUNSETALARMAREAFROMCARDREADER,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIdsWithPerson,
                    string.Format(
                        "Unset alarm area from card reader, card reader: {0}, person: {1}, alarm area: {2}",
                        cardReader.Name,
                        person,
                        alarmArea),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPEPERSONNAME,
                    person.ToString(),
                    EventlogParameter.TYPEPEALARMAREANAME,
                    alarmArea.ToString());
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPEUNSETALARMAREAFROMCARDREADER,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    alarmArea),
                string.Format(
                    "Unset alarm area from card reader with code, card reader: {0}, alarm area: {1}",
                    cardReader.Name,
                    alarmArea),
                out eventlog,
                out eventSources,
                out eventlogParameters,
                EventlogParameter.TYPEPEALARMAREANAME,
                alarmArea.ToString());
        }
    }

    [LwSerialize(467)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventFunctionKeyPressed : EventParametersWithObjectIdAndState
    {
        public Guid IdOutput { get; private set; }
        public Guid IdCard { get; private set; }

        public EventFunctionKeyPressed(
            UInt64 eventId,
            Guid idCardReader,
            State state,
            Guid idOutput,
            Guid idCard)
            : base(
                eventId,
                EventType.FunctionKeyPressed,
                idCardReader,
                state)
        {
            IdOutput = idOutput;
            IdCard = idCard;
        }

        protected EventFunctionKeyPressed()
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

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;

            var eventSourcesIds = CcuEvents.GetEventSourcesFromCardReader(
                cardReader,
                false,
                Cards.Singleton.GetById(IdCard),
                out personName);

            var output = Outputs.Singleton.GetById(IdOutput);

            if (output == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            eventSourcesIds.Add(output.IdOutput);

            var description = string.Format(
                "The Function key changed the output {0} state to {1}",
                output,
                State);

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_FUNCTIONKEY_PRESSED,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(650)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToAaMenuInvalidPin : EventForAccessDenied
    {
        protected EventCrAccessDeniedEnterToAaMenuInvalidPin()
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
            var card = Cards.Singleton.GetById(IdCard);

            if (card == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var cardReader = CardReaders.Singleton.GetById(IdObject);
            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;

            var eventSourcesIds =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    false,
                    card,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_PIN,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    string.Format(
                        "Access dennied enter to alarm area menu invalid pin, card: {0}, person: {1}",
                        card.GetFullCardNumber(),
                        personName),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName);
        }
    }

    [LwSerialize(651)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToAaMenuInvalidCode : EventParametersWithObjectId
    {
        protected EventCrAccessDeniedEnterToAaMenuInvalidCode()
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
            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_CODE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    false),
                "Access dennied enter to alarm area menu invalid code",
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(652)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToSensorsMenuInvalidPin : EventForAccessDenied
    {
        protected EventCrAccessDeniedEnterToSensorsMenuInvalidPin()
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
            var card = Cards.Singleton.GetById(IdCard);

            if (card == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var cardReader = CardReaders.Singleton.GetById(IdObject);
            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;

            var eventSourcesIds =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    false,
                    card,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_PIN,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    string.Format(
                        "Access dennied enter to sensors menu invalid pin, card: {0}, person: {1}",
                        card.GetFullCardNumber(),
                        personName),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName);
        }
    }

    [LwSerialize(653)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToSensorsMenuInvalidCode : EventParametersWithObjectId
    {
        protected EventCrAccessDeniedEnterToSensorsMenuInvalidCode()
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
            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_CODE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    false),
                "Access dennied enter to sensors menu invalid code",
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(654)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToEventlogsMenuInvalidPin : EventForAccessDenied
    {
        public EventCrAccessDeniedEnterToEventlogsMenuInvalidPin(
            UInt64 eventId,
            Guid idCardReader,
            Guid idCard)
            : base(
                eventId,
                EventType.AccessDeniedEnterToSensorsMenuInvalidPin,
                idCardReader,
                idCard,
                Guid.Empty)
        {

        }

        protected EventCrAccessDeniedEnterToEventlogsMenuInvalidPin()
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
            var card = Cards.Singleton.GetById(IdCard);

            if (card == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var cardReader = CardReaders.Singleton.GetById(IdObject);
            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            string personName;

            var eventSourcesIds =
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    false,
                    card,
                    out personName);

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_PIN,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    string.Format(
                        "Access dennied enter to eventlogs menu invalid pin, card: {0}, person: {1}",
                        card.GetFullCardNumber(),
                        personName),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters,
                    EventlogParameter.TYPECARDNUMBER,
                    card.GetFullCardNumber(),
                    EventlogParameter.TYPEPERSONNAME,
                    personName);
        }
    }

    [LwSerialize(655)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventCrAccessDeniedEnterToEventlogsMenuInvalidCode : EventParametersWithObjectId
    {
        public EventCrAccessDeniedEnterToEventlogsMenuInvalidCode(
            UInt64 eventId,
            Guid idCardReader)
            : base(
                eventId,
                EventType.AccessDeniedEnterToSensorsMenuInvalidCode,
                idCardReader)
        {

        }

        protected EventCrAccessDeniedEnterToEventlogsMenuInvalidCode()
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
            var cardReader = CardReaders.Singleton.GetById(IdObject);

            if (cardReader == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_CODE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                CcuEvents.GetEventSourcesFromCardReader(
                    cardReader,
                    false),
                "Access dennied enter to eventlogs menu invalid code",
                out eventlog,
                out eventSources);
        }
    }

    [LwSerialize(656)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    internal class EventCardReaderBlockedStateChanged : EventParametersWithObjectId
    {
        public bool IsBlocked
        {
            get;
            private set;
        }

        protected EventCardReaderBlockedStateChanged()
        {
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CardReaderBlockedStateChanged(
                IdObject,
                DateTime,
                IsBlocked);
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
