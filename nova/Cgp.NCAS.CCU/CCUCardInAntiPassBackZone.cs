using System;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(311)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class CCUCardInAntiPassBackZone
    {
        [LwSerialize]
        public Guid GuidCard { get; private set; }

        [LwSerialize]
        public Guid GuidEntryCardReader { get; private set; }

        [LwSerialize]
        public DateTime EntryDateTime { get; private set; }

        [LwSerialize]
        public ApbzCardReaderEntryExitBy EntryBy { get; private set; }

        public int EndTickCount { get; set; }

        public CCUCardInAntiPassBackZone(
            Guid guidCard,
            Guid guidEntryCardReader,
            int endTickCount,
            DateTime entryDateTime,
            ApbzCardReaderEntryExitBy entryBy)
        {
            GuidCard = guidCard;
            GuidEntryCardReader = guidEntryCardReader;
            EndTickCount = endTickCount;
            EntryDateTime = entryDateTime;
            EntryBy = entryBy;
        }
    }
}