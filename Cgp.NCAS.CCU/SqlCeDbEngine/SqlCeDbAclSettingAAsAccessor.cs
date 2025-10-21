using System;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using System.Data;
using Contal.IwQuick;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbAclSettingAAsAccessor :
        SqlCeDbConfigObjectsAccessor<ACLSettingAA>,
        IAclSettingAAsStorage
    {
        private const string TABLE_NAME = "AclSettingAA";
        private const string PRIMARY_KEY_COLUMN_NAME = "IdACLSettingAA";
        private const string GUID_ACCESS_CONTROL_LIST_COLUMN_NAME = "GuidAccessControlList";
        private const string GUID_ALARM_AREA_COLUMN_NAME = "GuidAlarmArea";
        private const string ALARM_AREA_SET_COLUMN_NAME = "AlarmAreaSet";
        private const string ALARM_AREA_UNSET_COLUMN_NAME = "AlarmAreaUnset";
        private const string ALARM_AREA_UNCONDITIONAL_SET_COLUMN_NAME = "AlarmAreaUnconditionalSet";
        private const string ALARM_AREA_ALARM_ACKNOWLEDGE_COLUMN_NAME = "AlarmAreaAlarmAcknowledge";
        private const string SENSOR_HANDLING_COLUMN_NAME = "SensorHandling";
        private const string CR_EVENTLOG_HANDLING_COLUMN_NAME = "CREventLogHandling";
        private const string ALARM_AREA_TIME_BUYING_COLUMN_NAME = "AlarmAreaTimeBuying";

        private class AclSettingAAPrimaryKeyDbColumn : IPrimaryKeyDbColumn<ACLSettingAA, Guid>
        {
            public object GetValueFromPrimaryKey(Guid primaryKey)
            {
                return primaryKey;
            }

            public object GetValue(ACLSettingAA obj)
            {
                return obj.IdACLSettingAA;
            }

            public string Name { get { return PRIMARY_KEY_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidAlarmAreaDbColumn : IDbColumn<ACLSettingAA>
        {
            public object GetValue(ACLSettingAA aclSettingAa)
            {
                return aclSettingAa.GuidAlarmArea;
            }

            public string Name { get { return GUID_ALARM_AREA_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class ObjectCreator : AObjectCreator<ACLSettingAA>
        {
            private class IdAclSettingAAObjectCreatorDbColumn :
                AclSettingAAPrimaryKeyDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLSettingAA aclSetting, object dbData)
                {
                    aclSetting.IdACLSettingAA = (Guid)dbData;
                }
            }

            private class GuidAccessControlListObjectCreatorDbColumn :
                IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.GuidAccessControlList;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.GuidAccessControlList = (Guid)dbData;
                }

                public string Name { get { return GUID_ACCESS_CONTROL_LIST_COLUMN_NAME; } }
                public string DbTypeString { get { return "uniqueidentifier"; } }
                public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            private class GuidAlarmAreaObjectCreatorDbColumn :
                GuidAlarmAreaDbColumn,
                IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.GuidAlarmArea;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.GuidAlarmArea = (Guid)dbData;
                }

                public string Name { get { return GUID_ALARM_AREA_COLUMN_NAME; } }
                public string DbTypeString { get { return "uniqueidentifier"; } }
                public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
                public int? DbSize { get { return null; } }
            }

            private class AlarmAreaSetObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.AlarmAreaSet;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.AlarmAreaSet = (bool)dbData;
                }

                public string Name { get { return ALARM_AREA_SET_COLUMN_NAME; } }
                public string DbTypeString { get { return "bit"; } }
                public SqlDbType DbType { get { return SqlDbType.Bit; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            private class AlarmAreaUnsetObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.AlarmAreaUnset;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.AlarmAreaUnset = (bool)dbData;
                }

                public string Name { get { return ALARM_AREA_UNSET_COLUMN_NAME; } }
                public string DbTypeString { get { return "bit"; } }
                public SqlDbType DbType { get { return SqlDbType.Bit; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            private class AlarmAreaUnconditionalSetObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.AlarmAreaUnconditionalSet;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.AlarmAreaUnconditionalSet = (bool)dbData;
                }

                public string Name { get { return ALARM_AREA_UNCONDITIONAL_SET_COLUMN_NAME; } }
                public string DbTypeString { get { return "bit"; } }
                public SqlDbType DbType { get { return SqlDbType.Bit; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            private class AlarmAreaAlarmAcknowledgeObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.AlarmAreaAlarmAcknowledge;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.AlarmAreaAlarmAcknowledge = (bool)dbData;
                }

                public string Name { get { return ALARM_AREA_ALARM_ACKNOWLEDGE_COLUMN_NAME; } }
                public string DbTypeString { get { return "bit"; } }
                public SqlDbType DbType { get { return SqlDbType.Bit; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            private class SensorHandlingObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.SensorHandling;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.SensorHandling = (bool)dbData;
                }

                public string Name { get { return SENSOR_HANDLING_COLUMN_NAME; } }
                public string DbTypeString { get { return "bit"; } }
                public SqlDbType DbType { get { return SqlDbType.Bit; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            private class CREvnetlogHandlingObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.CREventLogHandling;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.CREventLogHandling = (bool)dbData;
                }

                public string Name { get { return CR_EVENTLOG_HANDLING_COLUMN_NAME; } }
                public string DbTypeString { get { return "bit"; } }
                public SqlDbType DbType { get { return SqlDbType.Bit; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            private class AlarmAreaTimeBuyingObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSettingAA aclSettingAa)
                {
                    return aclSettingAa.AlarmAreaTimeBuying;
                }

                public void SetFieldValue(ACLSettingAA aclSettingAa, object dbData)
                {
                    aclSettingAa.AlarmAreaTimeBuying = (bool)dbData;
                }

                public string Name { get { return ALARM_AREA_TIME_BUYING_COLUMN_NAME; } }
                public string DbTypeString { get { return "bit"; } }
                public SqlDbType DbType { get { return SqlDbType.Bit; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            public ObjectCreator()
                : base(
                    new IObjectCreatorDbColumn[]
                    {
                        new IdAclSettingAAObjectCreatorDbColumn(),
                        new GuidAccessControlListObjectCreatorDbColumn(),
                        new GuidAlarmAreaObjectCreatorDbColumn(), 
                        new AlarmAreaSetObjectCreatorDbColumn(), 
                        new AlarmAreaUnsetObjectCreatorDbColumn(), 
                        new AlarmAreaUnconditionalSetObjectCreatorDbColumn(),
                        new AlarmAreaAlarmAcknowledgeObjectCreatorDbColumn(), 
                        new SensorHandlingObjectCreatorDbColumn(), 
                        new CREvnetlogHandlingObjectCreatorDbColumn(),
                        new AlarmAreaTimeBuyingObjectCreatorDbColumn()
                    })
            {
            }

            protected override ACLSettingAA CreateInstance()
            {
                return new ACLSettingAA();
            }
        }

        private readonly SqlCeDbCommand _getAclSettingAAsForAlarmAreaCommand;
        private readonly SqlCeDbCommand _getAclSettingAAsForAllAlarmAreasCommand;
        private readonly ObjectCreator _objectCreator;

        public SqlCeDbAclSettingAAsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener) :
                this(
                sqlCeDbCommandFactory,
                dbObjectRemovalListener,
                new GuidAlarmAreaDbColumn(),
                new ObjectCreator())
        {
        }

        private SqlCeDbAclSettingAAsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener,
            GuidAlarmAreaDbColumn guidAlarmAreaDbColumn,
            ObjectCreator objectCreator) :
                base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.ACLSettingAA,
                PRIMARY_KEY_COLUMN_NAME,
                Enumerable.Repeat<IPrimaryKeyDbColumn<ACLSettingAA, Guid>>(
                    new AclSettingAAPrimaryKeyDbColumn(),
                    1),
                new []
                {
                    new DbIndex<ACLSettingAA>(guidAlarmAreaDbColumn)
                },
                objectCreator,
                null,
                dbObjectRemovalListener)
        {
            _objectCreator = objectCreator;

            var columnNamesForObjectCreator = new StringBuilder();

            foreach (var column in _objectCreator.RequiredDbColumns)
            {
                if (columnNamesForObjectCreator.Length > 0)
                    columnNamesForObjectCreator.Append(",");

                columnNamesForObjectCreator.AppendFormat(
                    "{0}.{1}",
                    TABLE_NAME,
                    column.Name);
            }

            _getAclSettingAAsForAlarmAreaCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} JOIN {2} ON ({3} = {4}) WHERE {5} = @{5} AND {6} <= @{6} AND {7} >= @{7} AND {8} = @{8}",
                    columnNamesForObjectCreator,
                    TABLE_NAME,
                    SqlCeDbAclPersonsAccessor.TABLE_NAME,
                    string.Format(
                        "{0}.{1}",
                        TABLE_NAME,
                        GUID_ACCESS_CONTROL_LIST_COLUMN_NAME),
                    string.Format(
                        "{0}.{1}",
                        SqlCeDbAclPersonsAccessor.TABLE_NAME,
                        SqlCeDbAclPersonsAccessor.GUID_ACCESS_CONTROL_LIST_COLUMN_NAME),
                    SqlCeDbAclPersonsAccessor.GUID_PERSON_COLUMN_NAME,
                    SqlCeDbAclPersonsAccessor.DATE_FROM_COLUMN_NAME,
                    SqlCeDbAclPersonsAccessor.DATE_TO_COLUMN_NAME,
                    GUID_ALARM_AREA_COLUMN_NAME));

            _getAclSettingAAsForAlarmAreaCommand.Prepare(new IDbColumnDefinition[]
            {
                new SqlCeDbAclPersonsAccessor.GuidPersonDbColumnDefintion(),
                new SqlCeDbAclPersonsAccessor.DateFromDbColumnDefintion(),
                new SqlCeDbAclPersonsAccessor.DateToDbColumnDefinition(),
                guidAlarmAreaDbColumn
            });

            _getAclSettingAAsForAllAlarmAreasCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} JOIN {2} ON ({3} = {4}) WHERE {5} = @{5} AND {6} <= @{6} AND {7} >= @{7}",
                    columnNamesForObjectCreator,
                    TABLE_NAME,
                    SqlCeDbAclPersonsAccessor.TABLE_NAME,
                    string.Format(
                        "{0}.{1}",
                        TABLE_NAME,
                        GUID_ACCESS_CONTROL_LIST_COLUMN_NAME),
                    string.Format(
                        "{0}.{1}",
                        SqlCeDbAclPersonsAccessor.TABLE_NAME,
                        SqlCeDbAclPersonsAccessor.GUID_ACCESS_CONTROL_LIST_COLUMN_NAME),
                    SqlCeDbAclPersonsAccessor.GUID_PERSON_COLUMN_NAME,
                    SqlCeDbAclPersonsAccessor.DATE_FROM_COLUMN_NAME,
                    SqlCeDbAclPersonsAccessor.DATE_TO_COLUMN_NAME));

            _getAclSettingAAsForAllAlarmAreasCommand.Prepare(new IDbColumnDefinition[]
            {
                new SqlCeDbAclPersonsAccessor.GuidPersonDbColumnDefintion(),
                new SqlCeDbAclPersonsAccessor.DateFromDbColumnDefintion(),
                new SqlCeDbAclPersonsAccessor.DateToDbColumnDefinition()
            });
        }

        public IEnumerable<ACLSettingAA> GetAclSettingAAs(
            Guid idPerson,
            Guid idAlamrArea)
        {
            lock (OperationLock)
            {
                var dateTime = DateTime.Now;

                return _getAclSettingAAsForAlarmAreaCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(
                            SqlCeDbAclPersonsAccessor.GUID_PERSON_COLUMN_NAME,
                            idPerson),
                        new KeyValuePair<string, object>(
                            SqlCeDbAclPersonsAccessor.DATE_FROM_COLUMN_NAME,
                            dateTime),
                        new KeyValuePair<string, object>(
                            SqlCeDbAclPersonsAccessor.DATE_TO_COLUMN_NAME,
                            dateTime),
                        new KeyValuePair<string, object>(GUID_ALARM_AREA_COLUMN_NAME, idAlamrArea)
                    })
                    .Select(row => _objectCreator.CreateObject(row));
            }
        }

        public IEnumerable<ACLSettingAA> GetAclSettingAAs(
            Guid idPerson)
        {
            lock (OperationLock)
            {
                var dateTime = DateTime.Now;

                return _getAclSettingAAsForAllAlarmAreasCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(
                            SqlCeDbAclPersonsAccessor.GUID_PERSON_COLUMN_NAME,
                            idPerson),
                        new KeyValuePair<string, object>(
                            SqlCeDbAclPersonsAccessor.DATE_FROM_COLUMN_NAME,
                            dateTime),
                        new KeyValuePair<string, object>(
                            SqlCeDbAclPersonsAccessor.DATE_TO_COLUMN_NAME,
                            dateTime)
                    })
                    .Select(row => _objectCreator.CreateObject(row));
            }
        }
    }
}
