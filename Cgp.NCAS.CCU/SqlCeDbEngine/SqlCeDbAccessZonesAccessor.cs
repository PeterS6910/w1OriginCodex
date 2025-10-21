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
    public class SqlCeDbAccessZonesAccessor :
        SqlCeDbConfigObjectsAccessor<AccessZone>,
        IAccessZonesStorage
    {
        private const string TABLE_NAME = "AccessZone";
        private const string PRIMARY_KEY_COLUMN_NAME = "IdAccessZone";
        private const string GUID_PERSON_COLUMN_NAME = "GuidPerson";
        private const string GUID_CARD_READER_OBJECT_COLUMN_NAME = "GuidCardReaderObject";
        private const string CARD_READER_OBJECT_TYPE_COLUMN_NAME = "CardReaderObjectType";
        private const string GUID_TIME_ZONE_COLUMN_NAME = "GuidTimeZone";

        private class AccessZonePrimaryKeyDbColumn : IPrimaryKeyDbColumn<AccessZone, Guid>
        {
            public object GetValueFromPrimaryKey(Guid primaryKey)
            {
                return primaryKey;
            }

            public object GetValue(AccessZone obj)
            {
                return obj.IdAccessZone;
            }

            public string Name { get { return PRIMARY_KEY_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidPersonDbColumn : IDbColumn<AccessZone>
        {
            public object GetValue(AccessZone accessZone)
            {
                return accessZone.GuidPerson;
            }

            public string Name { get { return GUID_PERSON_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidCardReaderObjectDbColumn : IDbColumn<AccessZone>
        {
            public object GetValue(AccessZone accessZone)
            {
                return accessZone.GuidCardReaderObject;
            }

            public string Name { get { return GUID_CARD_READER_OBJECT_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class CardReaderObjectTypeDbColumn : IDbColumn<AccessZone>
        {
            public object GetValue(AccessZone accessZone)
            {
                return accessZone.CardReaderObjectType;
            }

            public string Name { get { return CARD_READER_OBJECT_TYPE_COLUMN_NAME; } }
            public string DbTypeString { get { return "tinyint"; } }
            public SqlDbType DbType { get { return SqlDbType.TinyInt; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class ObjectCreator : AObjectCreator<AccessZone>
        {
            private class IdAccessZoneObjectCreatorDbColumn :
                AccessZonePrimaryKeyDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(AccessZone accessZone, object dbData)
                {
                    accessZone.IdAccessZone = (Guid) dbData;
                }
            }

            private class GuidPersonObjectCreatorDbColumn :
                GuidPersonDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(AccessZone accessZone, object dbData)
                {
                    accessZone.GuidPerson = (Guid) dbData;
                }
            }

            private class GuidCardReaderObjectObjectCreatorDbColumn :
                GuidCardReaderObjectDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(AccessZone accessZone, object dbData)
                {
                    accessZone.GuidCardReaderObject = (Guid)dbData;
                }
            }

            private class CardReaderObjectTypeObjectCreatorDbColumn :
                CardReaderObjectTypeDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(AccessZone accessZone, object dbData)
                {
                    accessZone.CardReaderObjectType = (byte)dbData;
                }
            }

            private class GuidTimeZoneObjectCreatorDbColumn : IObjectCreatorDbColumn
            {
                public object GetValue(AccessZone accessZone)
                {
                    return accessZone.GuidTimeZone;
                }

                public void SetFieldValue(AccessZone accessZone, object dbData)
                {
                    accessZone.GuidTimeZone = (Guid)dbData;
                }

                public string Name { get { return GUID_TIME_ZONE_COLUMN_NAME; } }
                public string DbTypeString { get { return "uniqueidentifier"; } }
                public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
                public int? DbSize { get { return null; } }
                public bool AllowNull { get { return false; } }
            }

            public ObjectCreator()
                : base(
                    new IObjectCreatorDbColumn[]
                    {
                        new IdAccessZoneObjectCreatorDbColumn(),
                        new GuidPersonObjectCreatorDbColumn(),
                        new GuidCardReaderObjectObjectCreatorDbColumn(), 
                        new CardReaderObjectTypeObjectCreatorDbColumn(), 
                        new GuidTimeZoneObjectCreatorDbColumn()
                    })
            {
            }

            protected override AccessZone CreateInstance()
            {
                return new AccessZone();
            }
        }

        private readonly SqlCeDbCommand _getTimeZoneIdsForCrObjectCommand;
        private readonly SqlCeDbCommand _getTimeZoneIdsForAlarmAreasCommand;
        private readonly SqlCeDbCommand _getTimeZoneIdsForMultiDoorElementCommand;
        private readonly SqlCeDbCommand _getTimeZoneIdsForFloorCommand;
        private readonly SqlCeDbCommand _getAccessZonesForPersonCountCommand;

        public SqlCeDbAccessZonesAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener) :
            this(
                sqlCeDbCommandFactory,
                dbObjectRemovalListener,
                new GuidPersonDbColumn(), 
                new GuidCardReaderObjectDbColumn(),
                new CardReaderObjectTypeDbColumn())
        {
        }

        private SqlCeDbAccessZonesAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener,
            GuidPersonDbColumn guidPersonDbColumn,
            GuidCardReaderObjectDbColumn guidCardReaderObjectDbColumn,
            CardReaderObjectTypeDbColumn cardReaderObjectTypeDbColumn) :
                base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.AccessZone,
                PRIMARY_KEY_COLUMN_NAME,
                Enumerable.Repeat<IPrimaryKeyDbColumn<AccessZone, Guid>>(
                    new AccessZonePrimaryKeyDbColumn(),
                    1),
                new []
                {
                    new DbIndex<AccessZone>(guidPersonDbColumn),
                    new DbIndex<AccessZone>(
                        guidPersonDbColumn,
                        guidCardReaderObjectDbColumn)
                },
                new ObjectCreator(),
                null,
                dbObjectRemovalListener)
        {
            _getTimeZoneIdsForCrObjectCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2} AND {3} = @{3} AND {4} = @{4}",
                    GUID_TIME_ZONE_COLUMN_NAME,
                    TABLE_NAME,
                    GUID_PERSON_COLUMN_NAME,
                    CARD_READER_OBJECT_TYPE_COLUMN_NAME,
                    GUID_CARD_READER_OBJECT_COLUMN_NAME));

            _getTimeZoneIdsForCrObjectCommand.Prepare(new IDbColumnDefinition[]
            {
                guidPersonDbColumn,
                cardReaderObjectTypeDbColumn,
                guidCardReaderObjectDbColumn
            });

            _getTimeZoneIdsForAlarmAreasCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} JOIN {2} ON ({3} = {4}) WHERE {5} = @{5} AND {6} = @{6} AND {7} = {8}",
                    GUID_TIME_ZONE_COLUMN_NAME,
                    TABLE_NAME,
                    SqlCeDbAaCardReadersAccessor.TABLE_NAME,
                    GUID_CARD_READER_OBJECT_COLUMN_NAME,
                    SqlCeDbAaCardReadersAccessor.GUID_ALARM_AREA_COLUMN_NAME,
                    SqlCeDbAaCardReadersAccessor.GUID_CARD_READER_COLUMN_NAME,
                    GUID_PERSON_COLUMN_NAME,
                    CARD_READER_OBJECT_TYPE_COLUMN_NAME,
                    (byte) ObjectType.AlarmArea));

            _getTimeZoneIdsForAlarmAreasCommand.Prepare(
                new IDbColumnDefinition[]
                {
                    guidPersonDbColumn,
                    new SqlCeDbAaCardReadersAccessor.GuidCardReaderDbColumnDefinition()
                });

            _getTimeZoneIdsForMultiDoorElementCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2} AND {3} = @{3} AND {4} = {5}",
                    GUID_TIME_ZONE_COLUMN_NAME,
                    TABLE_NAME,
                    GUID_PERSON_COLUMN_NAME,
                    GUID_CARD_READER_OBJECT_COLUMN_NAME,
                    CARD_READER_OBJECT_TYPE_COLUMN_NAME,
                    (byte) ObjectType.MultiDoorElement));

            _getTimeZoneIdsForMultiDoorElementCommand.Prepare(new IDbColumnDefinition[]
            {
                guidPersonDbColumn,
                guidCardReaderObjectDbColumn
            });

            _getTimeZoneIdsForFloorCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} JOIN {2} ON ({3} = {4}) WHERE {5} = @{5} AND {6} = @{6} AND {7} = {8}",
                    GUID_TIME_ZONE_COLUMN_NAME,
                    TABLE_NAME,
                    SqlCeDbMultiDoorElementsAccessor.TABLE_NAME,
                    GUID_CARD_READER_OBJECT_COLUMN_NAME,
                    SqlCeDbMultiDoorElementsAccessor.FLOOR_ID_COLUMN_NAME,
                    SqlCeDbMultiDoorElementsAccessor.ID_MULTI_DOOR_ELEMENT_COLUMN_NAME,
                    GUID_PERSON_COLUMN_NAME,
                    CARD_READER_OBJECT_TYPE_COLUMN_NAME,
                    (byte) ObjectType.Floor));

            _getTimeZoneIdsForFloorCommand.Prepare(
                new IDbColumnDefinition[]
                {
                    guidPersonDbColumn,
                    new SqlCeDbMultiDoorElementsAccessor.IdMultiDoorElementDbColumnDefinition()
                });

            _getAccessZonesForPersonCountCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT COUNT({0}) FROM {1} WHERE {2} = @{2}",
                    PRIMARY_KEY_COLUMN_NAME,
                    TABLE_NAME,
                    GUID_PERSON_COLUMN_NAME));

            _getAccessZonesForPersonCountCommand.Prepare(
                new IDbColumnDefinition[]
                {
                    guidPersonDbColumn
                });
        }

        public IEnumerable<Guid> GetTimeZoneIdsForCrObject(
            Guid idPerson,
            ObjectType objectType,
            Guid idCardReaderObject)
        {
            lock (OperationLock)
                return _getTimeZoneIdsForCrObjectCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(GUID_PERSON_COLUMN_NAME, idPerson),
                        new KeyValuePair<string, object>(CARD_READER_OBJECT_TYPE_COLUMN_NAME, (byte) objectType),
                        new KeyValuePair<string, object>(GUID_CARD_READER_OBJECT_COLUMN_NAME, idCardReaderObject)
                    })
                    .Select(row => (Guid) row[0]);
        }

        public IEnumerable<Guid> GetTimeZoneIdsForAlarmAreas(
            Guid idPerson,
            Guid idCardReader)
        {
            lock (OperationLock)
                return _getTimeZoneIdsForAlarmAreasCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(GUID_PERSON_COLUMN_NAME, idPerson),
                        new KeyValuePair<string, object>(SqlCeDbAaCardReadersAccessor.GUID_CARD_READER_COLUMN_NAME,
                            idCardReader)
                    })
                    .Select(row => (Guid) row[0]);
        }

        public IEnumerable<Guid> GetTimeZoneIdsForMultiDoorElement(
            Guid idPerson,
            Guid idMultiDoorElement)
        {
            lock (OperationLock)
                return _getTimeZoneIdsForMultiDoorElementCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(GUID_PERSON_COLUMN_NAME, idPerson),
                        new KeyValuePair<string, object>(GUID_CARD_READER_OBJECT_COLUMN_NAME, idMultiDoorElement)
                    })
                    .Select(row => (Guid) row[0]);
        }

        public IEnumerable<Guid> GetTimeZoneIdsForFloor(
            Guid idPerson,
            Guid idMultiDoorElement)
        {
            lock (OperationLock)
                return _getTimeZoneIdsForFloorCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(GUID_PERSON_COLUMN_NAME, idPerson),
                        new KeyValuePair<string, object>(
                            SqlCeDbMultiDoorElementsAccessor.ID_MULTI_DOOR_ELEMENT_COLUMN_NAME,
                            idMultiDoorElement)
                    })
                    .Select(row => (Guid) row[0]);
        }

        public bool ExistsAccessZonesForPerson(Guid idPerson)
        {
            lock (OperationLock)
                return (int) _getAccessZonesForPersonCountCommand.ExecuteScalar(
                    new[]
                    {
                        new KeyValuePair<string, object>(GUID_PERSON_COLUMN_NAME, idPerson)
                    }) > 0;
        }
    }
}
