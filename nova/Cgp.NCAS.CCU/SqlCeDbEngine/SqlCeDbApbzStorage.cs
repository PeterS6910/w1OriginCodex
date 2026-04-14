using System;
using System.Collections.Generic;
using System.Data;
using Contal.Cgp.NCAS.CCU.DB;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class ApbzPrimaryKey : IEquatable<ApbzPrimaryKey>
    {
        public Guid IdCard { get; private set; }
        public Guid IdAntiPassBackZone { get; private set; }

        public ApbzPrimaryKey(
            Guid idCard,
            Guid idAntiPassBackZone)
        {
            IdCard = idCard;
            IdAntiPassBackZone = idAntiPassBackZone;
        }

        public bool Equals(ApbzPrimaryKey other)
        {
            return other != null
                   && other.IdCard.Equals(IdCard)
                   && other.IdAntiPassBackZone.Equals(IdAntiPassBackZone);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ApbzPrimaryKey);
        }

        public override int GetHashCode()
        {
            return IdCard.GetHashCode() ^ IdAntiPassBackZone.GetHashCode();
        }
    }

    public class SqlCeDbApbzStorage :
        ASqlCeDbDelayedAccessor<StoredCardInAntiPassBackZone, ApbzPrimaryKey>,
        IApbzStorage
    {
        private class IdCardDbColumn : IPrimaryKeyDbColumn<StoredCardInAntiPassBackZone, ApbzPrimaryKey>
        {
            public object GetValueFromPrimaryKey(ApbzPrimaryKey primaryKey)
            {
                return primaryKey.IdCard;
            }

            public object GetValue(StoredCardInAntiPassBackZone obj)
            {
                return obj.GuidCard;
            }

            public string Name { get { return ID_CARD_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class IdAntiPassBackZoneDbColumn : IPrimaryKeyDbColumn<StoredCardInAntiPassBackZone, ApbzPrimaryKey>
        {
            public object GetValueFromPrimaryKey(ApbzPrimaryKey primaryKey)
            {
                return primaryKey.IdAntiPassBackZone;
            }

            public object GetValue(StoredCardInAntiPassBackZone obj)
            {
                return obj.GuidAntiPassBackZone;
            }

            public string Name { get { return ID_ANTI_PASS_BACK_ZONE_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private const string ID_CARD_COLUMN_NAME = "GuidCard";
        private const string ID_ANTI_PASS_BACK_ZONE_COLUMN_NAME = "GuidAntiPassBackZone";

        public SqlCeDbApbzStorage(ISqlCeDbCommandFactory sqlCeDbCommandFactory) :
            base(
            sqlCeDbCommandFactory,
            "StoredCardInAntiPassBackZone",
            new[]
            {
                (IPrimaryKeyDbColumn<StoredCardInAntiPassBackZone, ApbzPrimaryKey>) new IdCardDbColumn(),
                new IdAntiPassBackZoneDbColumn()
            },
            new ObjectCreatorFromSerializedData<StoredCardInAntiPassBackZone>())
        {

        }

        protected override ApbzPrimaryKey CreatePrimaryKey(Dictionary<string, object> primaryKeyDbColumnsValues)
        {
            return new ApbzPrimaryKey(
                (Guid) primaryKeyDbColumnsValues[ID_CARD_COLUMN_NAME],
                (Guid) primaryKeyDbColumnsValues[ID_ANTI_PASS_BACK_ZONE_COLUMN_NAME]);
        }

        protected override ApbzPrimaryKey GetPrimaryKey(StoredCardInAntiPassBackZone obj)
        {
            return new ApbzPrimaryKey(
                obj.GuidCard,
                obj.GuidAntiPassBackZone);
        }

        public void AddCard(
            CCUCardInAntiPassBackZone ccuCardInAntiPassBackZone,
            Guid guidAntiPassBackZone)
        {
            SaveToDatabase(
                new ApbzPrimaryKey(
                    ccuCardInAntiPassBackZone.GuidCard,
                    guidAntiPassBackZone),
                new StoredCardInAntiPassBackZone
                {
                    GuidCard = ccuCardInAntiPassBackZone.GuidCard,
                    GuidAntiPassBackZone = guidAntiPassBackZone,
                    GuidEntryCardReader = ccuCardInAntiPassBackZone.GuidEntryCardReader,
                    EntryDateTime = ccuCardInAntiPassBackZone.EntryDateTime,
                    EntryBy = ccuCardInAntiPassBackZone.EntryBy
                });
        }

        public void RemoveCard(
            Guid idCard,
            Guid idAntiPassBackZone)
        {
            DeleteFromDatabase(
                new ApbzPrimaryKey(
                    idCard,
                    idAntiPassBackZone));
        }

        public ICollection<StoredCardInAntiPassBackZone> LoadCards()
        {
            return GetAllFromDatabase();
        }
    }
}
