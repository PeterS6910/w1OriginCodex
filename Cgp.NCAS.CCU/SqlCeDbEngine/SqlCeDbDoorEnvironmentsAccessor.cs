using System;
using System.Data;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbDoorEnvironmentsAccessor :
        SqlCeDbConfigObjectsAccessor<DoorEnvironment>,
        IDoorEnvironmentsStorage
    {
        private const string TABLE_NAME = "DoorEnvironemnt";
        private const string ID_DOOR_ENVIRONMENT_COLUMN_NAME = "IdDoorEnvironment";
        private const string GUID_CARD_READER_INTERNAL_COLUMN_NAME = "GuidCardReaderInternal";
        private const string GUID_CARD_READER_EXTERNAL_COLUMN_NAME = "GuidCardReaderExternal";

        private class GuidCardReaderInternalDbColumn :
            IDbColumn<DoorEnvironment>
        {
            public object GetValue(DoorEnvironment doorEnvironment)
            {
                return doorEnvironment.GuidCardReaderInternal;
            }

            public string Name { get { return GUID_CARD_READER_INTERNAL_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidCardReaderExternalDbColumn :
            IDbColumn<DoorEnvironment>
        {
            public object GetValue(DoorEnvironment doorEnvironment)
            {
                return doorEnvironment.GuidCardReaderExternal;
            }

            public string Name { get { return GUID_CARD_READER_EXTERNAL_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private readonly SqlCeDbCommand _getDoorEnvironemntIdForCardReaderCommand;

        public SqlCeDbDoorEnvironmentsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener)
            : this(
                sqlCeDbCommandFactory,
                dbObjectRemovalListener,
                new GuidCardReaderInternalDbColumn(),
                new GuidCardReaderExternalDbColumn())
        {
        }

        private SqlCeDbDoorEnvironmentsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener,
            GuidCardReaderInternalDbColumn guidCardReaderInternalDbColumn,
            GuidCardReaderExternalDbColumn guidCardReaderExternalDbColumn)
            : base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.DoorEnvironment,
                ID_DOOR_ENVIRONMENT_COLUMN_NAME,
                new []
                {
                    new DbIndex<DoorEnvironment>(guidCardReaderInternalDbColumn),
                    new DbIndex<DoorEnvironment>(guidCardReaderExternalDbColumn)
                },
                new ObjectCreatorFromSerializedData<DoorEnvironment>(),
                DoorEnvironments.Singleton,
                dbObjectRemovalListener)
        {
            _getDoorEnvironemntIdForCardReaderCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2} OR {3} = @{3}",
                    ID_DOOR_ENVIRONMENT_COLUMN_NAME,
                    TABLE_NAME,
                    GUID_CARD_READER_INTERNAL_COLUMN_NAME,
                    GUID_CARD_READER_EXTERNAL_COLUMN_NAME));

            _getDoorEnvironemntIdForCardReaderCommand.Prepare(
                new IDbParameterDefinition[]
                {
                    guidCardReaderInternalDbColumn,
                    guidCardReaderExternalDbColumn
                });
        }

        public Guid GetDoorEnvironemntIdForCardReader(Guid idCardReader)
        {
            lock (OperationLock)
                return _getDoorEnvironemntIdForCardReaderCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(GUID_CARD_READER_INTERNAL_COLUMN_NAME, idCardReader),
                        new KeyValuePair<string, object>(GUID_CARD_READER_EXTERNAL_COLUMN_NAME, idCardReader)
                    })
                    .Select(row => (Guid) row[0]).FirstOrDefault();
        }
    }
}
