using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using System.Data;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbAclSettingsAccessor :
        SqlCeDbConfigObjectsAccessor<ACLSetting>,
        IAclSettingsStorage
    {
        private const string TABLE_NAME = "AclSetting";
        private const string PRIMARY_KEY_COLUMN_NAME = "IdAclSetting";
        private const string GUID_ACCESS_CONTROL_LIST_COLUMN_NAME = "GuidAccessControlList";
        private const string GUID_CARD_READER_OBJECT_COLUMN_NAME = "GuidCardReaderObject";
        private const string CARD_READER_OBJECT_TYPE_COLUMN_NAME = "CardReaderObjectType";
        private const string GUID_TIME_ZONE_COLUMN_NAME = "GuidTimeZone";
        private const string DISABLED_COLUMN_NAME = "Disabled";

        private class AclSettingPrimaryKeyDbColumn : IPrimaryKeyDbColumn<ACLSetting, Guid>
        {
            public object GetValueFromPrimaryKey(Guid primaryKey)
            {
                return primaryKey;
            }

            public object GetValue(ACLSetting obj)
            {
                return obj.IdACLSetting;
            }

            public string Name { get { return PRIMARY_KEY_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidAccessControlListDbColumn : IDbColumn<ACLSetting>
        {
            public object GetValue(ACLSetting aclSetting)
            {
                return aclSetting.GuidAccessControlList;
            }

            public string Name { get { return GUID_ACCESS_CONTROL_LIST_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidCardReaderObjectDbColumn : IDbColumn<ACLSetting>
        {
            public object GetValue(ACLSetting aclPerson)
            {
                return aclPerson.GuidCardReaderObject;
            }

            public string Name { get { return GUID_CARD_READER_OBJECT_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class CardReaderObjectTypeDbColumn : IDbColumn<ACLSetting>
        {
            public object GetValue(ACLSetting aclPerson)
            {
                return aclPerson.CardReaderObjectType;
            }

            public string Name { get { return CARD_READER_OBJECT_TYPE_COLUMN_NAME; } }
            public string DbTypeString { get { return "tinyint"; } }
            public SqlDbType DbType { get { return SqlDbType.TinyInt; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class ObjectCreator : AObjectCreator<ACLSetting>
        {
            private class IdAclSettingObjectCreatorDbColumn :
                AclSettingPrimaryKeyDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLSetting aclSetting, object dbData)
                {
                    aclSetting.IdACLSetting = (Guid)dbData;
                }
            }

            private class GuidCardReaderObjectObjectCreatorDbColumn :
                GuidCardReaderObjectDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLSetting aclSetting, object dbData)
                {
                    aclSetting.GuidCardReaderObject = (Guid)dbData;
                }
            }

            private class CardReaderObjectTypeObjectCreatorDbColumn :
                CardReaderObjectTypeDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLSetting aclSetting, object dbData)
                {
                    aclSetting.CardReaderObjectType = (byte)dbData;
                }
            }

            private class GuidAccessControlListObjectCreatorDbColumn :
                GuidAccessControlListDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLSetting aclSetting, object dbData)
                {
                    aclSetting.GuidAccessControlList = (Guid)dbData;
                }
            }

            private class GuidTimeZoneObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSetting aclSetting)
                {
                    return aclSetting.GuidTimeZone;
                }

                public void SetFieldValue(ACLSetting aclSetting, object dbData)
                {
                    aclSetting.GuidTimeZone = (Guid)dbData;
                }

                public string Name { get { return GUID_TIME_ZONE_COLUMN_NAME; } }
                public string DbTypeString { get { return "uniqueidentifier"; } }
                public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            private class DisabledObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(ACLSetting aclSetting)
                {
                    return aclSetting.Disabled ?? false;
                }

                public void SetFieldValue(ACLSetting aclSetting, object dbData)
                {
                    aclSetting.Disabled = (bool)dbData;
                }

                public string Name { get { return DISABLED_COLUMN_NAME; } }
                public string DbTypeString { get { return "bit"; } }
                public SqlDbType DbType { get { return SqlDbType.Bit; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            public ObjectCreator()
                : base(
                    new IObjectCreatorDbColumn[]
                    {
                        new IdAclSettingObjectCreatorDbColumn(),
                        new GuidAccessControlListObjectCreatorDbColumn(),
                        new GuidCardReaderObjectObjectCreatorDbColumn(), 
                        new CardReaderObjectTypeObjectCreatorDbColumn(), 
                        new GuidTimeZoneObjectCreatorDbColumn(),
                        new DisabledObjectCreatorDbColumn()
                    })
            {
            }

            protected override ACLSetting CreateInstance()
            {
                return new ACLSetting();
            }
        }

        private readonly SqlCeDbCommand _getTimeZoneIdsForCrObjectCommand;
        private readonly SqlCeDbCommand _getTimeZoneIdsForAlarmAreasCommand;
        private readonly SqlCeDbCommand _getTimeZoneIdsForMultiDoorElementCommand;
        private readonly SqlCeDbCommand _getTimeZoneIdsForFloorCommand;

        public SqlCeDbAclSettingsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener) :
                this(
                sqlCeDbCommandFactory,
                dbObjectRemovalListener,
                new GuidAccessControlListDbColumn(),
                new GuidCardReaderObjectDbColumn(),
                new CardReaderObjectTypeDbColumn())
        {
        }

        private SqlCeDbAclSettingsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener,
            GuidAccessControlListDbColumn guidAccessControlListDbColumn,
            GuidCardReaderObjectDbColumn guidCardReaderObjectDbColumn,
            CardReaderObjectTypeDbColumn cardReaderObjectTypeDbColumn) :
            base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.ACLSetting,
                PRIMARY_KEY_COLUMN_NAME,
                Enumerable.Repeat<IPrimaryKeyDbColumn<ACLSetting, Guid>>(
                    new AclSettingPrimaryKeyDbColumn(),
                    1),
                new []
                {
                    new DbIndex<ACLSetting>(
                        guidAccessControlListDbColumn,
                        guidCardReaderObjectDbColumn),
                    new DbIndex<ACLSetting>(guidCardReaderObjectDbColumn),
                    new DbIndex<ACLSetting>(cardReaderObjectTypeDbColumn)
                },
                new ObjectCreator(),
                null,
                dbObjectRemovalListener)
        {
            _getTimeZoneIdsForCrObjectCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2} AND {3} = @{3} AND {4} = @{4} AND {5} = 0",
                    GUID_TIME_ZONE_COLUMN_NAME,
                    TABLE_NAME,
                    GUID_ACCESS_CONTROL_LIST_COLUMN_NAME,
                    CARD_READER_OBJECT_TYPE_COLUMN_NAME,
                    GUID_CARD_READER_OBJECT_COLUMN_NAME,
                    DISABLED_COLUMN_NAME));

            _getTimeZoneIdsForCrObjectCommand.Prepare(new IDbColumnDefinition[]
            {
                guidAccessControlListDbColumn,
                cardReaderObjectTypeDbColumn,
                guidCardReaderObjectDbColumn
            });

            _getTimeZoneIdsForAlarmAreasCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} JOIN {2} ON ({3} = {4}) WHERE {5} = @{5} AND {6} = @{6} AND {7} = {8} AND {9} = 0",
                    GUID_TIME_ZONE_COLUMN_NAME,
                    TABLE_NAME,
                    SqlCeDbAaCardReadersAccessor.TABLE_NAME,
                    GUID_CARD_READER_OBJECT_COLUMN_NAME,
                    SqlCeDbAaCardReadersAccessor.GUID_ALARM_AREA_COLUMN_NAME,
                    GUID_ACCESS_CONTROL_LIST_COLUMN_NAME,
                    SqlCeDbAaCardReadersAccessor.GUID_CARD_READER_COLUMN_NAME,
                    CARD_READER_OBJECT_TYPE_COLUMN_NAME,
                    (byte) ObjectType.AlarmArea,
                    DISABLED_COLUMN_NAME));

            _getTimeZoneIdsForAlarmAreasCommand.Prepare(new IDbColumnDefinition[]
            {
                guidAccessControlListDbColumn,
                guidCardReaderObjectDbColumn,
                new SqlCeDbAaCardReadersAccessor.GuidCardReaderDbColumnDefinition()
            });

            _getTimeZoneIdsForMultiDoorElementCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2} AND {3} = @{3} AND {4} = {5} AND {6} = 0",
                    GUID_TIME_ZONE_COLUMN_NAME,
                    TABLE_NAME,
                    GUID_ACCESS_CONTROL_LIST_COLUMN_NAME,
                    GUID_CARD_READER_OBJECT_COLUMN_NAME,
                    CARD_READER_OBJECT_TYPE_COLUMN_NAME,
                    (byte) ObjectType.MultiDoorElement,
                    DISABLED_COLUMN_NAME));

            _getTimeZoneIdsForMultiDoorElementCommand.Prepare(
                new IDbColumnDefinition[]
                {
                    guidAccessControlListDbColumn,
                    guidCardReaderObjectDbColumn
                });

            _getTimeZoneIdsForFloorCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} JOIN {2} ON ({3} = {4}) WHERE {5} = @{5} AND {6} = @{6} AND {7} = {8} AND {9} = 0",
                    GUID_TIME_ZONE_COLUMN_NAME,
                    SqlCeDbMultiDoorElementsAccessor.TABLE_NAME,
                    TABLE_NAME,
                    SqlCeDbMultiDoorElementsAccessor.FLOOR_ID_COLUMN_NAME,
                    GUID_CARD_READER_OBJECT_COLUMN_NAME,
                    SqlCeDbMultiDoorElementsAccessor.ID_MULTI_DOOR_ELEMENT_COLUMN_NAME,
                    GUID_ACCESS_CONTROL_LIST_COLUMN_NAME,
                    CARD_READER_OBJECT_TYPE_COLUMN_NAME,
                    (byte) ObjectType.Floor,
                    DISABLED_COLUMN_NAME));

            _getTimeZoneIdsForFloorCommand.Prepare(new IDbColumnDefinition[]
            {
                guidAccessControlListDbColumn,
                new SqlCeDbMultiDoorElementsAccessor.IdMultiDoorElementDbColumnDefinition()
            });
        }

        public IEnumerable<Guid> GetTimeZoneIdsForCardReaderObject(
            ObjectType objectType,
            Guid idCardReaderObject,
            Guid idAccessControlList)
        {
            lock (OperationLock)
                return _getTimeZoneIdsForCrObjectCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(GUID_ACCESS_CONTROL_LIST_COLUMN_NAME, idAccessControlList),
                        new KeyValuePair<string, object>(CARD_READER_OBJECT_TYPE_COLUMN_NAME, (byte) objectType),
                        new KeyValuePair<string, object>(GUID_CARD_READER_OBJECT_COLUMN_NAME, idCardReaderObject)
                    })
                    .Select(row => (Guid) row[0]);
        }

        public IEnumerable<Guid> GetTimeZoneIdsForAlarmAreas(
            Guid idCardReader,
            Guid idAccessControlList)
        {
            lock (OperationLock)
                return _getTimeZoneIdsForAlarmAreasCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(
                            GUID_ACCESS_CONTROL_LIST_COLUMN_NAME,
                            idAccessControlList),
                        new KeyValuePair<string, object>(SqlCeDbAaCardReadersAccessor.GUID_CARD_READER_COLUMN_NAME,
                            idCardReader)
                    })
                    .Select(row => (Guid) row[0]);
        }

        public IEnumerable<Guid> GetTimeZoneIdsForMultiDoorElement(
            Guid idMultiDooElement,
            Guid idAccessControlList)
        {
            lock (OperationLock)
                return _getTimeZoneIdsForMultiDoorElementCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(
                            GUID_ACCESS_CONTROL_LIST_COLUMN_NAME,
                            idAccessControlList),
                        new KeyValuePair<string, object>(
                            GUID_CARD_READER_OBJECT_COLUMN_NAME,
                            idMultiDooElement)
                    })
                    .Select(row => (Guid) row[0]);
        }

        public IEnumerable<Guid> GetTimeZoneIdsForFloor(
            Guid idMultiDooElement,
            Guid idAccessControlList)
        {
            lock (OperationLock)
                return _getTimeZoneIdsForFloorCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(
                            GUID_ACCESS_CONTROL_LIST_COLUMN_NAME,
                            idAccessControlList),
                        new KeyValuePair<string, object>(
                            SqlCeDbMultiDoorElementsAccessor.ID_MULTI_DOOR_ELEMENT_COLUMN_NAME,
                            idMultiDooElement)
                    })
                    .Select(row => (Guid) row[0]);
        }
    }
}
