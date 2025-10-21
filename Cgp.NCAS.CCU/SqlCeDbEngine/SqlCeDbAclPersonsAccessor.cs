using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using System.Data;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbAclPersonsAccessor :
        SqlCeDbConfigObjectsAccessor<ACLPerson>,
        IAclPersonsStorage
    {
        public const string TABLE_NAME = "AclPerson";
        private const string PRIMARY_KEY_COLUMN_NAME = "IdAclPerson";
        public const string GUID_PERSON_COLUMN_NAME = "GuidPerson";
        public const string GUID_ACCESS_CONTROL_LIST_COLUMN_NAME = "GuidAccessControlList";
        public const string DATE_FROM_COLUMN_NAME = "DateFrom";
        public const string DATE_TO_COLUMN_NAME = "DateTo";

        private class AclPersonPrimaryKeyDbColumn : IPrimaryKeyDbColumn<ACLPerson, Guid>
        {
            public object GetValueFromPrimaryKey(Guid primaryKey)
            {
                return primaryKey;
            }

            public object GetValue(ACLPerson obj)
            {
                return obj.IdACLPerson;
            }

            public string Name { get { return PRIMARY_KEY_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidAccessControlListDbColumn : IDbColumn<ACLPerson>
        {
            public object GetValue(ACLPerson aclPerson)
            {
                return aclPerson.GuidAccessControlList;
            }

            public string Name { get { return GUID_ACCESS_CONTROL_LIST_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class GuidPersonDbColumn :
            GuidPersonDbColumnDefintion,
            IDbColumn<ACLPerson>
        {
            public object GetValue(ACLPerson aclPerson)
            {
                return aclPerson.GuidPerson;
            }
        }

        public class GuidPersonDbColumnDefintion : IDbColumnDefinition
        {
            public string Name { get { return GUID_PERSON_COLUMN_NAME; } }
            public string DbTypeString { get { return "uniqueidentifier"; } }
            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private class DateFromDbColumn :
            DateFromDbColumnDefintion,
            IDbColumn<ACLPerson>
        {
            public object GetValue(ACLPerson aclPerson)
            {
                return aclPerson.DateFrom ?? SqlDateTime.MinValue;
            }
        }

        public class DateFromDbColumnDefintion : IDbColumnDefinition
        {
            public string Name { get { return DATE_FROM_COLUMN_NAME; } }
            public string DbTypeString { get { return "datetime"; } }
            public SqlDbType DbType { get { return SqlDbType.DateTime; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return true; } }
        }

        private class DateToDbColumn :
            DateToDbColumnDefinition,
            IDbColumn<ACLPerson>
        {
            public object GetValue(ACLPerson aclPerson)
            {
                return aclPerson.DateTo ?? SqlDateTime.MaxValue;
            }
        }

        public class DateToDbColumnDefinition : IDbColumnDefinition
        {
            public string Name { get { return DATE_TO_COLUMN_NAME; } }
            public string DbTypeString { get { return "datetime"; } }
            public SqlDbType DbType { get { return SqlDbType.DateTime; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return true; } }
        }

        private class ObjectCreator : AObjectCreator<ACLPerson>
        {
            private class IdAclPersonObjectCreatorDbColumn :
                AclPersonPrimaryKeyDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLPerson aclPerson, object dbData)
                {
                    aclPerson.IdACLPerson = (Guid) dbData;
                }
            }

            private class GuidPersonObjectCreatorDbColumn :
                GuidPersonDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLPerson aclPerson, object dbData)
                {
                    aclPerson.GuidPerson = (Guid) dbData;
                }
            }

            private class GuidAccessControlListObjectCreatorDbColumn :
                GuidAccessControlListDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLPerson aclPerson, object dbData)
                {
                    aclPerson.GuidAccessControlList = (Guid)dbData;
                }
            }

            private class DateFromObjectCreatorDbColumn :
                DateFromDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLPerson aclPerson, object dbData)
                {
                    aclPerson.DateFrom = (DateTime?) dbData;
                }
            }

            private class DateToObjectCreatorDbColumn :
                DateToDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(ACLPerson aclPerson, object dbData)
                {
                    aclPerson.DateTo = (DateTime?) dbData;
                }
            }

            public ObjectCreator()
                : base(
                    new IObjectCreatorDbColumn[]
                    {
                        new IdAclPersonObjectCreatorDbColumn(),
                        new GuidAccessControlListObjectCreatorDbColumn(),
                        new GuidPersonObjectCreatorDbColumn(),
                        new DateFromObjectCreatorDbColumn(),
                        new DateToObjectCreatorDbColumn()
                    })
            {
            }

            protected override ACLPerson CreateInstance()
            {
                return new ACLPerson();
            }
        }

        private readonly SqlCeDbCommand _getAclIdsForPersonCommand;

        public SqlCeDbAclPersonsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener) :
            this(
                sqlCeDbCommandFactory,
                dbObjectRemovalListener,
                new GuidPersonDbColumn(),
                new DateFromDbColumn(),
                new DateToDbColumn())
        {
        }

        private SqlCeDbAclPersonsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener,
            GuidPersonDbColumn guidPersonDbColumn,
            DateFromDbColumn dateFromDbColumn,
            DateToDbColumn dateToDbColumn) :
            base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.ACLPerson,
                PRIMARY_KEY_COLUMN_NAME,
                Enumerable.Repeat<IPrimaryKeyDbColumn<ACLPerson, Guid>>(
                    new AclPersonPrimaryKeyDbColumn(),
                    1),
                new []
                {
                    new DbIndex<ACLPerson>(guidPersonDbColumn), 
                    new DbIndex<ACLPerson>(new GuidAccessControlListDbColumn())
                },
                new ObjectCreator(),
                null,
                dbObjectRemovalListener)
        {
            _getAclIdsForPersonCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2} AND {3} <= @{3} AND {4} >= @{4}",
                    GUID_ACCESS_CONTROL_LIST_COLUMN_NAME,
                    TABLE_NAME,
                    GUID_PERSON_COLUMN_NAME,
                    DATE_FROM_COLUMN_NAME,
                    DATE_TO_COLUMN_NAME));

            _getAclIdsForPersonCommand.Prepare(new IDbColumnDefinition[]
            {
                guidPersonDbColumn,
                dateFromDbColumn,
                dateToDbColumn
            });
        }

        public IEnumerable<Guid> GetAclIdsForPerson(Guid idPerson)
        {
            lock (OperationLock)
            {
                var dateTime = DateTime.Now;

                return _getAclIdsForPersonCommand.ExecuteReader(
                    new[]
                    {
                        new KeyValuePair<string, object>(GUID_PERSON_COLUMN_NAME, idPerson),
                        new KeyValuePair<string, object>(DATE_FROM_COLUMN_NAME, dateTime),
                        new KeyValuePair<string, object>(DATE_TO_COLUMN_NAME, dateTime)
                    })
                    .Select(row => (Guid) row[0]);
            }
        }
    }
}
