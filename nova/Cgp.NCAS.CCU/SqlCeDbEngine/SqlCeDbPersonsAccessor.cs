using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    class SqlCeDbPersonsAccessor : SqlCeDbConfigObjectsAccessor<DB.Person>, IPersonsStorage
    {
        private class ObjectCreator : AObjectCreator<DB.Person>
        {
            private class ObjectCreatorPersonIdColumn :
                GuidPrimaryKeyDbColumn,
                IObjectCreatorDbColumn
            {
                public ObjectCreatorPersonIdColumn()
                    : base(ID_PERSON_COLUMN_NAME)
                {
                }

                public void SetFieldValue(DB.Person newPerson, object dbData)
                {
                    newPerson.IdPerson = (Guid) dbData;
                }
            }

            private class ObjectCreatorPersonCodeHashColumn :
                PersonalCodeHashDbColumn,
                IObjectCreatorDbColumn
            {
                public void SetFieldValue(DB.Person newPerson, object dbData)
                {
                    newPerson.PersonalCodeHash = (string) dbData;
                }
            }

            public ObjectCreator()
                : base(
                    new IObjectCreatorDbColumn[]
                    {
                        new ObjectCreatorPersonIdColumn(),
                        new ObjectCreatorPersonCodeHashColumn()
                    })
            {
            }

            protected override DB.Person CreateInstance()
            {
                return new DB.Person();
            }
        }

        private class PersonalCodeHashDbColumn : IDbColumn<DB.Person>
        {
            public object GetValue(DB.Person person)
            {
                return person.PersonalCodeHash;
            }

            public string Name { get { return PERSON_CODE_HASH_COLUMN_NAME; } }
            public string DbTypeString { get { return "nVarChar(8)"; } }
            public SqlDbType DbType { get { return SqlDbType.NVarChar; } }
            public int? DbSize { get { return 8; } }
            public bool AllowNull { get { return false; } }
        }

        private const string TABLE_NAME = "Person";
        private const string ID_PERSON_COLUMN_NAME = "IdPerson";
        private const string PERSON_CODE_HASH_COLUMN_NAME = "PersonalCodeHash";

        private readonly SqlCeDbCommand _getPersonIdFromPersonalCode;

        public SqlCeDbPersonsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            EventHandlerGroup<DB.IDbObjectRemovalListener> dbObjectRemovalHandlerGroup)
            : this(
                sqlCeDbCommandFactory,
                new PersonalCodeHashDbColumn(),
                dbObjectRemovalHandlerGroup)
        {
        }

        private SqlCeDbPersonsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            PersonalCodeHashDbColumn personalCodeHashDbColumn,
            EventHandlerGroup<DB.IDbObjectRemovalListener> dbObjectRemovalHandlerGroup)
            : base(
                sqlCeDbCommandFactory,
                TABLE_NAME,
                ObjectType.Person,
                ID_PERSON_COLUMN_NAME,
                new []
                {
                    new DbIndex<DB.Person>(personalCodeHashDbColumn)
                },
                new ObjectCreator(),
                null,
                dbObjectRemovalHandlerGroup)
        {
            _getPersonIdFromPersonalCode = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @{2}",
                    ID_PERSON_COLUMN_NAME,
                    TABLE_NAME,
                    PERSON_CODE_HASH_COLUMN_NAME));

            _getPersonIdFromPersonalCode.Prepare(
                Enumerable.Repeat(
                    (IDbParameterDefinition) personalCodeHashDbColumn,
                    1));
        }

        public Guid GetPersonId(string personCodeHash)
        {
            lock (OperationLock)
            {
                var result = _getPersonIdFromPersonalCode.ExecuteScalar(
                    Enumerable.Repeat(
                        new KeyValuePair<string, object>(
                            PERSON_CODE_HASH_COLUMN_NAME,
                            personCodeHash),
                        1));

                return result != null
                    ? (Guid) result
                    : Guid.Empty;
            }
        }
    }
}
