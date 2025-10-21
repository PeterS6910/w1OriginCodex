using System;

using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    [LwSerialize(209)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class MifareSectorSectorInfo
    {
        public const string COLUMN_ID = "Id";
        public const string COLUMN_SECTOR_NUMBER = "SectorNumber";
        //public const string COLUMN_RESERVED = "Reserved";
        public const string COLUMN_INHERIT_A_KEY = "InheritAKey";
        public const string COLUMN_INHERIT_B_KEY = "InheritBKey";
        public const string COLUMN_A_KEY = "AKey";
        public const string COLUMN_B_KEY = "BKey";
        public const string COLUMN_BANK = "Bank";
        public const string COLUMN_OFFSET = "Offset";
        public const string COLUMN_LENGTH = "Length";
        public const string COLUMN_MIFATE_SECTOR_DATA = "MifareSectorData";

        public virtual int Id { get; set; }
        public virtual int SectorNumber { get; set; }
        //public virtual bool Reserved { get; set; }
        public virtual bool InheritAKey { get; set; }
        public virtual bool InheritBKey { get; set; }
        public virtual byte[] AKey { get; set; }
        public virtual byte[] BKey { get; set; }
        public virtual byte? Bank { get; set; }
        public virtual byte? Offset { get; set; }
        public virtual byte? Length { get; set; }
        public virtual MifareSectorData MifareSectorData { get; set; }
    }
}
