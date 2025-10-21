using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbAutonomousEventsAcessor : ASqlCeDbAccessorBase<EventParameters.EventParameters, UInt64>
    {
        private class IdEventPrimaryKeyDbColumn : IPrimaryKeyDbColumn<EventParameters.EventParameters, UInt64>
        {
            public string Name { get { return IdEventColumnName; } }
            public string DbTypeString { get { return "bigint"; } }
            public SqlDbType DbType { get { return SqlDbType.BigInt; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }

            public object GetValue(EventParameters.EventParameters obj)
            {
                return obj.EventId;
            }

            public object GetValueFromPrimaryKey(UInt64 primaryKey)
            {
                return primaryKey;
            }
        }

        private class ObjectCreator : IObjectCreator<EventParameters.EventParameters>
        {
            private class DataDbColumn : IDbColumn<EventParameters.EventParameters>
            {
                public object GetValue(EventParameters.EventParameters obj)
                {
                    return Serialization.Serialize(obj);
                }

                // https://stackoverflow.com/questions/6736881/sql-ce-max-length
                public string Name { get { return DataColumnName; } }
                // Extended to 4000 to match the SqlDbType.NVarChar length restrictions
                public string DbTypeString { get { return "varbinary(4000)"; } }
                public SqlDbType DbType { get { return SqlDbType.VarBinary; } }
                public int? DbSize { get { return 4000; } }
                public bool AllowNull { get { return false; } }
            }

            public IEnumerable<IDbColumn<EventParameters.EventParameters>> RequiredDbColumns { get; private set; }

            public ObjectCreator()
            {
                RequiredDbColumns = new[] { new DataDbColumn() };
            }

            public EventParameters.EventParameters CreateObject(object[] row)
            {
                return Serialization.Deserialize<EventParameters.EventParameters>((byte[])row[0]);
            }
        }

        public class TopCountColumnDefinition : IDbParameterDefinition
        {
            public string Name { get { return TopConditionColumnName; } }
            public SqlDbType DbType { get { return SqlDbType.SmallInt; } }
            public int? DbSize { get { return null; } }
        }

        public const string DataColumnName = "Data";
        public const string IdEventColumnName = "IdEvent";
        public const string TopConditionColumnName = "Count";

        private volatile object _operationLock = new object();
        private readonly ObjectCreator _objectCreator;

        private readonly SqlCeDbCommand _selectTopCommand;
        private readonly SqlCeDbCommand _selectMaxEventIdToDelete;
        private readonly SqlCeDbCommand _deleteOldestEventsCommand;
        private readonly SqlCeDbCommand _selectCountCommand;

        public SqlCeDbAutonomousEventsAcessor(ISqlCeDbCommandFactory sqlCeDbCommandFactory)
            : this(
                sqlCeDbCommandFactory,
                new IdEventPrimaryKeyDbColumn(),
                new ObjectCreator())
        {
        }

        private SqlCeDbAutonomousEventsAcessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            IdEventPrimaryKeyDbColumn idEventPrimaryKeyDbColumn,
            ObjectCreator objectCreator)
            : base(
                sqlCeDbCommandFactory,
                "AutonomousEvents",
                new[] {idEventPrimaryKeyDbColumn},
                null,
                objectCreator)
        {
            _objectCreator = objectCreator;

            var topCountColumnDefinition = new TopCountColumnDefinition();

            _selectTopCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT TOP (@{0}) {1} FROM {2} ORDER BY {3}",
                    TopConditionColumnName,
                    DataColumnName,
                    TableName,
                    IdEventColumnName));

            _selectTopCommand.Prepare(new IDbParameterDefinition[] { topCountColumnDefinition });

            _selectMaxEventIdToDelete = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT MAX ({0}) AS {0} FROM (SELECT TOP (@{1}) {0} FROM {2} ORDER BY {0}) AS tempTable",
                    IdEventColumnName,
                    TopConditionColumnName,
                    TableName));

            _selectMaxEventIdToDelete.Prepare(new IDbParameterDefinition[] { topCountColumnDefinition });

            _deleteOldestEventsCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "DELETE FROM {0} WHERE {1} <= @{1}",
                    TableName,
                    IdEventColumnName));

            _deleteOldestEventsCommand.Prepare(new IDbColumnDefinition[] { idEventPrimaryKeyDbColumn });

            _selectCountCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                string.Format(
                    "SELECT COUNT({0}) FROM {1}",
                    IdEventColumnName,
                    TableName));
        }

        public int InsertIntoDatabase(IEnumerable<EventParameters.EventParameters> eventParametersCollection)
        {
            int rowsInserted = 0;

            lock (_operationLock)
            {
                SqlCeDbResultSet sqlCeDbResultSet = null;

                try
                {
                    sqlCeDbResultSet = SqlCeDbCommandFactory.CreateSqlCeDbResultSet(TableName);

                    foreach (var eventParameters in eventParametersCollection)
                    {
                        InsertIntoDatabaseInternal(
                            eventParameters,
                            sqlCeDbResultSet);

                        ++rowsInserted;
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
                finally
                {
                    if (sqlCeDbResultSet != null)
                        sqlCeDbResultSet.Dispose();
                }
            }

            return rowsInserted;
        }

        public void DeleteFromDataBase(UInt64 eventId)
        {
            lock (_operationLock)
                try
                {
                    DeleteFromDatabaseInternal(eventId);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
        }

        [NotNull]
        public IEnumerable<EventParameters.EventParameters> SelectTop(int count)
        {
            lock (_operationLock)
                try
                {
                    var rows = _selectTopCommand.ExecuteReader(
                        new[]
                        {
                            new KeyValuePair<string, object>(
                                "Count",
                                count)
                        });

                    return rows.Select(
                        row =>
                            _objectCreator.CreateObject(row));
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

            return Enumerable.Empty<EventParameters.EventParameters>();
        }

        public void DeleteOldestFromDatabase(int count)
        {
            lock (_operationLock)
                try
                {
                    _deleteOldestEventsCommand.ExecuteNonQuery(
                        new[]
                        {
                            new KeyValuePair<string, object>(
                                IdEventColumnName,
                                _selectMaxEventIdToDelete.ExecuteScalar())
                        });
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
        }

        public int SelectCount()
        {
            lock (_operationLock)
                try
                {
                    return (int) _selectCountCommand.ExecuteScalar();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

            return 0;
        }

        public void DeleteAllFromDatabase()
        {
            lock (_operationLock)
                try
                {
                    DeleteAllFromDatabaseInternal();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

        }
    }
}
