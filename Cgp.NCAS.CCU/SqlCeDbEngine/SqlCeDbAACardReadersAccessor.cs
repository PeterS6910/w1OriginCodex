using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbAaCardReadersAccessor
        : SqlCeDbConfigObjectsAccessor<DB.AACardReader>,
        IAaCardReadersStorage
    {
        public const string TABLE_NAME = "AACardReader";
        public const string GUID_ALARM_AREA_COLUMN_NAME = "GuidAlarmArea";
        public const string GUID_CARD_READER_COLUMN_NAME = "GuidCardReader";

        private class GuidAlarmAreaDbColumn : IDbColumn<DB.AACardReader>
        {
            public object GetValue(DB.AACardReader aaCardReader)
            {
                return aaCardReader.GuidAlarmArea;
            }

            public string Name { get { return GUID_ALARM_AREA_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidCardReaderDbColumn :
            GuidCardReaderDbColumnDefinition,
            IDbColumn<DB.AACardReader>
        {
            public object GetValue(DB.AACardReader aaCardReader)
            {
                return aaCardReader.GuidCardReader;
            }
        }

        public class GuidCardReaderDbColumnDefinition : IDbColumnDefinition
        {
            public string Name { get { return GUID_CARD_READER_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private readonly ISelectByIndexedColumnsCommand<DB.AACardReader> _getAaCardReadersByIdCardReader;

        public SqlCeDbAaCardReadersAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<DB.IDbObjectRemovalListener> dbObjectRemovalListener)
            : this(
                sqlCeDbCommandFactory,
                new GuidCardReaderDbColumn(),
                dbObjectRemovalListener)
        {
        }

        private SqlCeDbAaCardReadersAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            GuidCardReaderDbColumn guidCardReaderDbColumn,
            [NotNull] EventHandlerGroup<DB.IDbObjectRemovalListener> dbObjectRemovalListener)
            : base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.AACardReader,
                "IdAACardReader",
                new[]
                {
                    new DbIndex<DB.AACardReader>(new GuidAlarmAreaDbColumn()),
                    new DbIndex<DB.AACardReader>(guidCardReaderDbColumn)
                },
                new ObjectCreatorFromSerializedData<DB.AACardReader>(),
                CardReaders.Singleton,
                dbObjectRemovalListener)
        {
            _getAaCardReadersByIdCardReader = CreateSelectByIndexedColumnsCommand(
                new IDbParameterDefinition[] {guidCardReaderDbColumn});
        }

        public ICollection<DB.AACardReader> GetAaCardReadersByIdCardReader(Guid idCardReader)
        {
            lock (OperationLock)
            {
                return _getAaCardReadersByIdCardReader.ExecuteReader(
                    Enumerable.Repeat(
                        new KeyValuePair<string, object>(
                            GUID_CARD_READER_COLUMN_NAME,
                            idCardReader),
                        1));
            }
        }
    }
}
