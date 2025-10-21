using System;
using System.Data;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbMultiDoorElementsAccessor :
        SqlCeDbConfigObjectsAccessor<MultiDoorElement>
    {
        public const string TABLE_NAME = "MultiDoorElement";
        public const string ID_MULTI_DOOR_ELEMENT_COLUMN_NAME = "IdMultiDoorElement";
        public const string MULTI_DOOR_ID_COLUMN_NAME = "MultiDoorId";
        public const string FLOOR_ID_COLUMN_NAME = "FloorId";

        private class MultiDoorElementPrimaryKeyDbColumn :
            IdMultiDoorElementDbColumnDefinition,
            IPrimaryKeyDbColumn<MultiDoorElement, Guid>
        {
            public object GetValueFromPrimaryKey(Guid primaryKey)
            {
                return primaryKey;
            }

            public object GetValue(MultiDoorElement obj)
            {
                return obj.IdMultiDoorElement;
            }
        }

        public class IdMultiDoorElementDbColumnDefinition
            : IDbColumnDefinition
        {
            public string Name { get { return ID_MULTI_DOOR_ELEMENT_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class MultiDoorIdDbColumn :
            MultiDoorIdDbColumnDefinition,
            IDbColumn<MultiDoorElement>
        {
            public object GetValue(MultiDoorElement multiDoorElement)
            {
                return multiDoorElement.MultiDoorId;
            }
        }

        public class MultiDoorIdDbColumnDefinition : IDbColumnDefinition
        {
            public string Name { get { return MULTI_DOOR_ID_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class FloorIdDbColumn :
            IDbColumn<MultiDoorElement>
        {
            public object GetValue(MultiDoorElement multiDoorElement)
            {
                return multiDoorElement.FloorId;
            }

            public string Name { get { return FLOOR_ID_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }


        public SqlCeDbMultiDoorElementsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener)
            : base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.MultiDoorElement,
                ID_MULTI_DOOR_ELEMENT_COLUMN_NAME,
                Enumerable.Repeat<IPrimaryKeyDbColumn<MultiDoorElement, Guid>>(
                    new MultiDoorElementPrimaryKeyDbColumn(),
                    1),
                new []
                {
                    new DbIndex<MultiDoorElement>(new MultiDoorIdDbColumn()),
                    new DbIndex<MultiDoorElement>(new FloorIdDbColumn())
                },
                new ObjectCreatorFromSerializedData<MultiDoorElement>(),
                MultiDoorElements.Singleton,
                dbObjectRemovalListener)
        {
        }
    }
}
