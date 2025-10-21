using System;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(336)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class StoredCardInAntiPassBackZone
    {
        public Guid GuidCard { get; set; }
        public Guid GuidAntiPassBackZone { get; set; }
        public Guid GuidEntryCardReader { get; set; }
        public DateTime EntryDateTime { get; set; }
        public ApbzCardReaderEntryExitBy EntryBy { get; set; }
    }
}