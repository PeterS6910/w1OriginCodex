using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbAlarmsStorage :
        ASqlCeDbDelayedAccessor<Alarm, Guid>,
        IAlarmsStorage
    {
        private class AlarmPrimaryKeyDbColumn : IPrimaryKeyDbColumn<Alarm, Guid>
        {
            public object GetValueFromPrimaryKey(Guid primaryKey)
            {
                return primaryKey;
            }

            public object GetValue(Alarm obj)
            {
                return obj.Id;
            }

            public string Name { get { return PRIMARY_KEY_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private const string PRIMARY_KEY_COLUMN_NAME = "Id";

        public SqlCeDbAlarmsStorage(ISqlCeDbCommandFactory sqlCeDbCommandFactory) :
            base(
            sqlCeDbCommandFactory,
            "Alarm",
            Enumerable.Repeat<IPrimaryKeyDbColumn<Alarm, Guid>>(
                new AlarmPrimaryKeyDbColumn(),
                1),
            new ObjectCreatorFromSerializedData<Alarm>())
        {

        }

        protected override Guid CreatePrimaryKey(Dictionary<string, object> primaryKeyDbColumnsValues)
        {
            return (Guid)primaryKeyDbColumnsValues[PRIMARY_KEY_COLUMN_NAME];
        }

        protected override Guid GetPrimaryKey(Alarm obj)
        {
            return obj.Id;
        }

        public IEnumerable<Alarm> GetAllAlarms()
        {
            return GetAllFromDatabase();
        }

        public void SaveAlarm(Alarm alarm)
        {
            SaveToDatabase(
                alarm.Id,
                alarm);
        }

        public void UpdateAlarm(Alarm alarm)
        {
            SaveToDatabase(
                alarm.Id,
                alarm);
        }

        public void DeleteAlarm(Alarm alarm)
        {
            DeleteFromDatabase(alarm.Id);
        }
    }
}
