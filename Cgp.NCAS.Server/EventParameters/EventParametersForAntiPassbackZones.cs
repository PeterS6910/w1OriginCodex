using System;
using System.Collections.Generic;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(430)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventApbzCardEntered : EventParametersWithObjectId
    {
        public Guid IdCard { get; private set; }
        public Guid IdEntryCardReader { get; private set; }
        public ApbzCardReaderEntryExitBy EntryBy { get; private set; }

        public EventApbzCardEntered(
            UInt64 eventId,
            Guid guidAntiPassBackZone,
            Guid guidCard,
            Guid guidEntryCardReader,
            ApbzCardReaderEntryExitBy entryBy)
            : base(
                eventId,
                EventType.AntiPassBackZoneCardEntered,
                guidAntiPassBackZone)
        {
            IdCard = guidCard;
            IdEntryCardReader = guidEntryCardReader;
            EntryBy = entryBy;
        }

        protected EventApbzCardEntered()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}, EntryCardReader: {1}, EntryBy {2}",
                    IdCard,
                    IdEntryCardReader,
                    EntryBy));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            AntiPassBackZones.Singleton.OnCardEntered(
                IdObject,
                DateTime,
                IdCard,
                IdEntryCardReader,
                EntryBy);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            ICollection<Guid> eventSourcesIds =
                new LinkedList<Guid>();

            var card =
                Cards.Singleton.GetById(IdCard);

            Person person = null;

            if (card != null)
            {
                eventSourcesIds.Add(card.IdCard);

                person = card.Person;

                if (person != null)
                    eventSourcesIds.Add(person.IdPerson);
            }

            var antiPassBackZone =
                AntiPassBackZones.Singleton.GetById(IdObject);

            if (antiPassBackZone != null)
                eventSourcesIds.Add(antiPassBackZone.IdAntiPassBackZone);

            var entryCardReader =
                CardReaders.Singleton.GetById(IdEntryCardReader);

            if (entryCardReader != null)
            {
                eventSourcesIds.Add(entryCardReader.IdCardReader);

                var dcu = entryCardReader.DCU;

                CCU ccu;

                if (dcu != null)
                {
                    eventSourcesIds.Add(dcu.IdDCU);

                    ccu = dcu.CCU;
                }
                else
                    ccu = entryCardReader.CCU;

                if (ccu != null)
                    eventSourcesIds.Add(ccu.IdCCU);
            }

            var stringBuilder =
                new StringBuilder("Card ");

            if (card != null)
            {
                stringBuilder.Append(card.FullCardNumber);
                stringBuilder.Append(' ');

                if (person != null)
                    stringBuilder.AppendFormat(
                        "(person name \"{0}\") ",
                        person.WholeName);
            }

            stringBuilder.Append("entered anti-passback zone ");

            if (antiPassBackZone != null)
                stringBuilder.AppendFormat(
                    "\"{0}\" ",
                    antiPassBackZone.Name);

            if (entryCardReader != null)
                stringBuilder.AppendFormat(
                    "via {0} on card reader {1}",
                    GetTextForEntryExitBy(EntryBy),
                    entryCardReader.Name);
            else
                stringBuilder.Append("without card reader access");

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_ENTERED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    stringBuilder.ToString(),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
        }

        public static string GetTextForEntryExitBy(ApbzCardReaderEntryExitBy entryBy)
        {
            switch (entryBy)
            {
                case ApbzCardReaderEntryExitBy.NormalAccess:
                    return "normal access";
                case ApbzCardReaderEntryExitBy.AccessPermitted:
                    return "access permitted";
                case ApbzCardReaderEntryExitBy.AccessInterupted:
                    return "access interupted";
            }

            return string.Empty;
        }
    }

    [LwSerialize(431)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventApbzCardExited : EventParametersWithObjectId
    {
        public Guid IdCard { get; private set; }
        public Guid IdEntryCardReader { get; private set; }
        public ApbzCardReaderEntryExitBy EntryBy { get; private set; }
        public DateTime EntryDateTime { get; private set; }
        public Guid IdExitCardReader { get; private set; }
        public ApbzCardReaderEntryExitBy ExitBy { get; private set; }

        public EventApbzCardExited(
            UInt64 eventId,
            Guid guidAntiPassBackZone,
            Guid guidCard,
            Guid guidEntryCardReader,
            ApbzCardReaderEntryExitBy entryBy,
            DateTime entryDateTime,
            Guid guidExitCardReader,
            ApbzCardReaderEntryExitBy exitBy)
            : base(
                eventId,
                EventType.AntiPassBackZoneCardExited,
                guidAntiPassBackZone)
        {
            IdCard = guidCard;
            IdEntryCardReader = guidEntryCardReader;
            EntryBy = entryBy;
            EntryDateTime = entryDateTime;
            IdExitCardReader = guidExitCardReader;
            ExitBy = exitBy;
        }

        protected EventApbzCardExited()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}, EntryCardReader: {1}, EntryBy: {2}, EntryDateTime: {3}, ExitCardReader: {4}, ExitBy: {5}",
                    IdCard,
                    IdEntryCardReader,
                    EntryBy,
                    EntryDateTime,
                    IdExitCardReader,
                    ExitBy));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            AntiPassBackZones.Singleton.OnCardExitedOrTimedOut(
                IdObject,
                IdCard);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            ICollection<Guid> eventSourcesIds =
                new LinkedList<Guid>();

            var card =
                Cards.Singleton.GetById(IdCard);

            Person person = null;

            if (card != null)
            {
                eventSourcesIds.Add(card.IdCard);

                person = card.Person;

                if (person != null)
                    eventSourcesIds.Add(person.IdPerson);
            }

            var antiPassBackZone =
                AntiPassBackZones.Singleton.GetById(IdObject);

            if (antiPassBackZone != null)
                eventSourcesIds.Add(antiPassBackZone.IdAntiPassBackZone);

            var exitCardReader =
                CardReaders.Singleton.GetById(IdExitCardReader);

            if (exitCardReader != null)
            {
                eventSourcesIds.Add(exitCardReader.IdCardReader);

                var dcu = exitCardReader.DCU;

                CCU ccu;

                if (dcu != null)
                {
                    eventSourcesIds.Add(dcu.IdDCU);

                    ccu = dcu.CCU;
                }
                else
                    ccu = exitCardReader.CCU;

                if (ccu != null)
                    eventSourcesIds.Add(ccu.IdCCU);
            }

            var stringBuilder =
                new StringBuilder("Card ");

            if (card != null)
            {
                stringBuilder.Append(card.FullCardNumber);
                stringBuilder.Append(' ');

                if (person != null)
                    stringBuilder.AppendFormat(
                        "(person name \"{0}\") ",
                        person.WholeName);
            }

            stringBuilder.Append("exited from anti-passback zone ");

            if (antiPassBackZone != null)
                stringBuilder.AppendFormat(
                    "\"{0}\" ",
                    antiPassBackZone.Name);

            if (exitCardReader != null)
                stringBuilder.AppendFormat(
                    "via {0} on card reader {1} ",
                    EventApbzCardEntered.GetTextForEntryExitBy(ExitBy),
                    exitCardReader.Name);

            stringBuilder.AppendFormat(
                "[entered on {0} ",
                EntryDateTime);

            var entryCardReader =
                CardReaders.Singleton.GetById(IdEntryCardReader);

            if (entryCardReader != null)
                stringBuilder.AppendFormat(
                    "via {0} on card reader {1}",
                    EventApbzCardEntered.GetTextForEntryExitBy(EntryBy),
                    entryCardReader.Name);
            else
                stringBuilder.Append("without card reader access");

            stringBuilder.Append(']');

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_EXITED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    stringBuilder.ToString(),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
        }
    }

    [LwSerialize(432)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventApbzCardTimedOut : EventParametersWithObjectId
    {
        public Guid IdCard { get; private set; }
        public Guid IdEntryCardReader { get; private set; }
        public ApbzCardReaderEntryExitBy EntryBy { get; private set; }
        public DateTime EntryDateTime { get; private set; }

        public EventApbzCardTimedOut(
            UInt64 eventId,
            Guid guidAntiPassBackZone,
            Guid guidCard,
            Guid guidEntryCardReader,
            ApbzCardReaderEntryExitBy entryBy,
            DateTime entryDateTime)
            : base(
                eventId,
                EventType.AntiPassBackZoneCardTimedOut,
                guidAntiPassBackZone)
        {
            IdCard = guidCard;
            IdEntryCardReader = guidEntryCardReader;
            EntryBy = entryBy;
            EntryDateTime = entryDateTime;
        }

        protected EventApbzCardTimedOut()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}, EntryCardReader: {1}, EntryBy {2}, EntryDateTime: {3}",
                    IdCard,
                    IdEntryCardReader,
                    EntryBy,
                    EntryDateTime));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            AntiPassBackZones.Singleton.OnCardExitedOrTimedOut(
                IdObject,
                IdCard);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            ICollection<Guid> eventSourcesIds =
                new LinkedList<Guid>();

            var card =
                Cards.Singleton.GetById(IdCard);

            Person person = null;

            if (card != null)
            {
                eventSourcesIds.Add(card.IdCard);

                person = card.Person;

                if (person != null)
                    eventSourcesIds.Add(person.IdPerson);
            }

            var antiPassBackZone =
                AntiPassBackZones.Singleton.GetById(IdObject);

            if (antiPassBackZone != null)
            {
                eventSourcesIds.Add(antiPassBackZone.IdAntiPassBackZone);
                var ccu = antiPassBackZone.GetParentCCU();

                if (ccu != null)
                    eventSourcesIds.Add(ccu.IdCCU);
            }

            var stringBuilder =
                new StringBuilder("Card ");

            if (card != null)
            {
                stringBuilder.Append(card.Number);
                stringBuilder.Append(' ');

                if (person != null)
                    stringBuilder.AppendFormat(
                        "(person name \"{0}\") ",
                        person.WholeName);
            }

            stringBuilder.Append("exited from anti-passback zone ");

            if (antiPassBackZone != null)
                stringBuilder.AppendFormat(
                    "\"{0}\" ",
                    antiPassBackZone.Name);

            stringBuilder.AppendFormat(
                "after timeout [entered on {0} ",
                EntryDateTime);

            var entryCardReader =
                CardReaders.Singleton.GetById(IdEntryCardReader);

            if (entryCardReader != null)
                stringBuilder.AppendFormat(
                    "via {0} on card reader {1}",
                    EventApbzCardEntered.GetTextForEntryExitBy(EntryBy),
                    entryCardReader.Name);
            else
                stringBuilder.Append("without card reader access");

            stringBuilder.Append(']');

            return
                Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_TIMED_OUT,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    stringBuilder.ToString(),
                    out eventlog,
                    out eventSources,
                    out eventlogParameters);
        }
    }
}