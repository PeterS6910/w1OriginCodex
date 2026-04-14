using System;
using System.Text;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(430)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventApbzCardEntered : EventParametersWithObjectId
    {
        public Guid IdCard { get; private set; }
        public Guid IdEntryCardReader { get; private set; }
        public ApbzCardReaderEntryExitBy EntryBy { get; private set; }

        public EventApbzCardEntered(
            Guid guidAntiPassBackZone,
            Guid guidCard,
            Guid guidEntryCardReader,
            ApbzCardReaderEntryExitBy entryBy)
            : base(
                EventType.AntiPassBackZoneCardEntered,
                guidAntiPassBackZone)
        {
            IdCard = guidCard;
            IdEntryCardReader = guidEntryCardReader;
            EntryBy = entryBy;
        }

        public EventApbzCardEntered()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}, EntryCardReader: {1}, EntryBy: {2}",
                    IdCard,
                    IdEntryCardReader,
                    EntryBy));
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
            Guid guidAntiPassBackZone,
            Guid guidCard,
            Guid guidEntryCardReader,
            ApbzCardReaderEntryExitBy entryBy,
            DateTime entryDateTime,
            Guid guidExitCardReader,
            ApbzCardReaderEntryExitBy exitBy)
            : base(
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

        public EventApbzCardExited()
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
            Guid guidAntiPassBackZone,
            Guid guidCard,
            Guid guidEntryCardReader,
            ApbzCardReaderEntryExitBy entryBy,
            DateTime entryDateTime)
            :
                base(
                EventType.AntiPassBackZoneCardTimedOut,
                guidAntiPassBackZone)
        {
            IdCard = guidCard;
            IdEntryCardReader = guidEntryCardReader;
            EntryBy = entryBy;
            EntryDateTime = entryDateTime;
        }

        public EventApbzCardTimedOut()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Card: {0}, EntryCardReader: {1}, EntryBy: {2}, EntryDateTime: {3}",
                    IdCard,
                    IdEntryCardReader,
                    EntryBy,
                    EntryDateTime));
        }
    }

    public static class TestApbzEvents
    {
        public static void EnqueueTestEvents(
            Guid idApbz,
            Guid idCardReader,
            Guid idCard)
        {
            Events.ProcessEvent(new EventApbzCardEntered(
                idApbz,
                idCard,
                idCardReader,
                ApbzCardReaderEntryExitBy.AccessInterupted));

            Events.ProcessEvent(new EventApbzCardExited(
                idApbz,
                idCard,
                idCardReader,
                ApbzCardReaderEntryExitBy.AccessPermitted,
                DateTime.Now,
                idCardReader,
                ApbzCardReaderEntryExitBy.AccessInterupted));

            Events.ProcessEvent(new EventApbzCardTimedOut(
                idApbz,
                idCard,
                idCardReader,
                ApbzCardReaderEntryExitBy.NormalAccess,
                DateTime.Now));
        }
    }
}