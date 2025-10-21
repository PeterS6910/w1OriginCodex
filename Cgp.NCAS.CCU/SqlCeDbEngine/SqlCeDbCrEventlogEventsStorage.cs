using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbCrEventlogEventsStorage :
        ASqlCeDbDelayedAccessor<EventParameters.EventParameters, Int64>,
        ICrEventlogEventsStorage
    {
        private class CrEventlgoEventsPrimaryKeyDbColumn : IPrimaryKeyDbColumn<EventParameters.EventParameters, Int64>
        {
            public object GetValueFromPrimaryKey(Int64 primaryKey)
            {
                return primaryKey;
            }

            public object GetValue(EventParameters.EventParameters obj)
            {
                return obj.EventId;
            }

            public string Name { get { return PRIMARY_KEY_COLUMN_NAME; } }
            public string DbTypeString { get { return "bigint"; } }
            public SqlDbType DbType { get { return SqlDbType.BigInt; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private const string PRIMARY_KEY_COLUMN_NAME = "EventId";

        public SqlCeDbCrEventlogEventsStorage(ISqlCeDbCommandFactory sqlCeDbCommandFactory) :
            base(
            sqlCeDbCommandFactory,
            "CrEventlogEvent",
            Enumerable.Repeat<IPrimaryKeyDbColumn<EventParameters.EventParameters, Int64>>(
                new CrEventlgoEventsPrimaryKeyDbColumn(),
                1),
            new ObjectCreatorFromSerializedData<EventParameters.EventParameters>())
        {

        }

        protected override Int64 CreatePrimaryKey(Dictionary<string, object> primaryKeyDbColumnsValues)
        {
            return (Int64) primaryKeyDbColumnsValues[PRIMARY_KEY_COLUMN_NAME];
        }

        protected override Int64 GetPrimaryKey(EventParameters.EventParameters obj)
        {
            return (Int64) obj.EventId;
        }

        public void SaveToDatabase(EventParameters.EventParameters eventParameters)
        {
            SaveToDatabase(
                (Int64) eventParameters.EventId,
                eventParameters);
        }

        public void DeleteFromDatabase(UInt64 eventId)
        {
            DeleteFromDatabase((Int64) eventId);
        }

        public IEnumerable<EventParameters.EventParameters> GetAllEventParameters()
        {
            return GetAllFromDatabase();
        }
    }
}
