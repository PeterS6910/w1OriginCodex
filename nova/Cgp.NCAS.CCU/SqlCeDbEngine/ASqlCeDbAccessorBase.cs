using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public interface IDbParameterDefinition
    {
        string Name { get; }
        SqlDbType DbType { get; }
        int? DbSize { get; }
    }

    public interface IDbColumnDefinition : IDbParameterDefinition
    {
        string DbTypeString { get; }
        bool AllowNull { get; }
    }

    public interface IDbColumn<TObject> : IDbColumnDefinition
    {
        object GetValue(TObject obj);
    }

    public interface IPrimaryKeyDbColumn<TObject, TPrimaryKey> : IDbColumn<TObject>
    {
        object GetValueFromPrimaryKey(TPrimaryKey primaryKey);
    }

    public interface ISelectByIndexedColumnsCommand<TObject>
    {
        ICollection<TObject> ExecuteReader(IEnumerable<KeyValuePair<string, object>> parameterNamesAndValues);
    }

    public interface IObjectCreator<TObject>
    {
        IEnumerable<IDbColumn<TObject>> RequiredDbColumns { get; }
        TObject CreateObject(object[] row);
    }

    public class DbIndex<TObject>
    {
        public IEnumerable<IDbColumn<TObject>> DbColumns { get; private set; }

        public DbIndex(params IDbColumn<TObject>[] dbColumns)
        {
            DbColumns = dbColumns;
        }
    }

    public class ObjectCreatorFromSerializedData<TObject> : IObjectCreator<TObject>
    {
        private class DataDbColumn : IDbColumn<TObject>
        {
            public object GetValue(TObject obj)
            {
                return Serialization.Serialize(obj);
            }

            public string Name { get { return "data"; } }
            public string DbTypeString { get { return "image"; } }
            public SqlDbType DbType { get { return SqlDbType.Image; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        public IEnumerable<IDbColumn<TObject>> RequiredDbColumns { get; private set; }

        public ObjectCreatorFromSerializedData()
        {
            RequiredDbColumns = new[] { new DataDbColumn() };
        }

        public TObject CreateObject(object[] row)
        {
            return Serialization.Deserialize<TObject>((byte[])row[0]);
        }
    }

    public abstract class AObjectCreator<TObject> : IObjectCreator<TObject>
    {
        protected interface IObjectCreatorDbColumn : IDbColumn<TObject>
        {
            void SetFieldValue(
                TObject newInstance,
                object dbData);
        }

        private readonly IObjectCreatorDbColumn[] _requiredDbColumns;

        public IEnumerable<IDbColumn<TObject>> RequiredDbColumns
        {
            get { return _requiredDbColumns; }
        }

        protected AObjectCreator([NotNull] IObjectCreatorDbColumn[] requiredDbColumns)
        {
            _requiredDbColumns = requiredDbColumns;
        }

        protected abstract TObject CreateInstance();

        public TObject CreateObject(object[] row)
        {
            var newInstance = CreateInstance();

            for (int index = 0; index < _requiredDbColumns.Length; index++)
            {
                _requiredDbColumns[index].SetFieldValue(
                    newInstance,
                    row[index]);
            }

            return newInstance;
        }
    }

    public abstract class ASqlCeDbAccessorBase<TObject, TPrimaryKey>
        where TObject : class
    {
        private class SelectByIndexedColumnsCommand : ISelectByIndexedColumnsCommand<TObject>
        {
            private readonly SqlCeDbCommand _sqlCeDbCommand;
            private readonly IObjectCreator<TObject> _objectCreator;

            public SelectByIndexedColumnsCommand(
                ASqlCeDbAccessorBase<TObject, TPrimaryKey> sqlCeDbAccessorBase,
                [NotNull]
                ICollection<IDbParameterDefinition> paramterColumns)
            {
                var condition = new StringBuilder();

                foreach (var dbColumnDefinition in paramterColumns)
                {
                    condition.AppendFormat(
                        condition.Length > 0
                            ? ", {0} = @{0}"
                            : "{0} = @{0}",
                        dbColumnDefinition.Name);
                }

                _sqlCeDbCommand = sqlCeDbAccessorBase._sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "SELECT {0} FROM {1} WHERE {2}",
                        sqlCeDbAccessorBase._objectCreatorRequiredColumnsNames,
                        sqlCeDbAccessorBase.TableName,
                        condition));

                _sqlCeDbCommand.Prepare(paramterColumns);

                _objectCreator = sqlCeDbAccessorBase._objectCreator;
            }

            public ICollection<TObject> ExecuteReader(IEnumerable<KeyValuePair<string, object>> parameterNamesAndValues)
            {
                try
                {
                    var result = new LinkedList<TObject>();

                    var rows = _sqlCeDbCommand.ExecuteReader(parameterNamesAndValues);

                    foreach (var row in rows)
                        result.AddLast(_objectCreator.CreateObject(row));

                    return result.Count > 0
                        ? result
                        : null;
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                return null;
            }
        }

        protected string TableName { get; private set; }

        protected readonly IPrimaryKeyDbColumn<TObject, TPrimaryKey>[] PrimaryKeyDbColumns;

        private readonly IDictionary<string, IDbColumn<TObject>> _allDbColumns =
            new Dictionary<string, IDbColumn<TObject>>();

        private readonly ISqlCeDbCommandFactory _sqlCeDbCommandFactory;

        protected ISqlCeDbCommandFactory SqlCeDbCommandFactory
        {
            get { return _sqlCeDbCommandFactory; }
        }

        private readonly IObjectCreator<TObject> _objectCreator;

        private readonly string _objectCreatorRequiredColumnsNames;

        private readonly SqlCeDbCommand _updateCommand;
        private readonly SqlCeDbCommand _selectByPrimaryKeyCommand;
        private readonly SqlCeDbCommand _deleteByPrimaryKeyCommand;

        protected ASqlCeDbAccessorBase(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            string tableName,
            [NotNull]
            IEnumerable<IPrimaryKeyDbColumn<TObject, TPrimaryKey>> primaryKeyDbColumns,
            ICollection<DbIndex<TObject>> dbIndexes,
            [NotNull]
            IObjectCreator<TObject> objectCreator)
        {
            try
            {
                _sqlCeDbCommandFactory = sqlCeDbCommandFactory;

                TableName = tableName;

                PrimaryKeyDbColumns = primaryKeyDbColumns.ToArray();

                _objectCreator = objectCreator;

                var allDbColumnsNames = new StringBuilder();
                var allDbColumnsParameterNames = new StringBuilder();
                var nonPrimaryKeyDbColumnsSetStatement = new StringBuilder();
                var primaryKeyDbColumnsCondition = new StringBuilder();

                foreach (var primaryKeyDbColumn in PrimaryKeyDbColumns)
                {
                    string name = primaryKeyDbColumn.Name;

                    _allDbColumns.Add(
                        name,
                        primaryKeyDbColumn);

                    allDbColumnsNames.AppendFormat(
                        allDbColumnsNames.Length > 0
                            ? ", {0}"
                            : "{0}",
                        name);

                    allDbColumnsParameterNames.AppendFormat(
                        allDbColumnsParameterNames.Length > 0
                            ? ", @{0}"
                            : "@{0}",
                        name);

                    primaryKeyDbColumnsCondition.AppendFormat(
                        primaryKeyDbColumnsCondition.Length > 0
                            ? " AND {0} = @{0}"
                            : "{0} = @{0}",
                        name);
                }

                if (dbIndexes != null)
                    foreach (var indexedDbColumn in dbIndexes.SelectMany(dbIndex => dbIndex.DbColumns))
                    {
                        var name = indexedDbColumn.Name;

                        if (_allDbColumns.ContainsKey(name))
                            continue;

                        _allDbColumns.Add(
                            name,
                            indexedDbColumn);

                        allDbColumnsNames.AppendFormat(
                            ", {0}",
                            name);

                        allDbColumnsParameterNames.AppendFormat(
                            ", @{0}",
                            name);

                        nonPrimaryKeyDbColumnsSetStatement.AppendFormat(
                            nonPrimaryKeyDbColumnsSetStatement.Length > 0
                                ? ", {0} = @{0}"
                                : "{0} = @{0}",
                            name);
                    }

                var objectCreatorRequiredColumnsNames = new StringBuilder();

                foreach (var objectCreatorRequiredColumn in _objectCreator.RequiredDbColumns)
                {
                    var name = objectCreatorRequiredColumn.Name;

                    objectCreatorRequiredColumnsNames.AppendFormat(
                        objectCreatorRequiredColumnsNames.Length > 0
                            ? ", {0}"
                            : "{0}",
                        name);

                    if (_allDbColumns.ContainsKey(name))
                        continue;

                    _allDbColumns.Add(
                        name,
                        objectCreatorRequiredColumn);

                    allDbColumnsNames.AppendFormat(
                        ", {0}",
                        name);

                    allDbColumnsParameterNames.AppendFormat(
                        ", @{0}",
                        name);

                    nonPrimaryKeyDbColumnsSetStatement.AppendFormat(
                        nonPrimaryKeyDbColumnsSetStatement.Length > 0
                            ? ", {0} = @{0}"
                            : "{0} = @{0}",
                        name);
                }

                _objectCreatorRequiredColumnsNames = objectCreatorRequiredColumnsNames.ToString();

                CreateTable(dbIndexes);

                _updateCommand = _sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "UPDATE {0} SET {1} WHERE {2}",
                        TableName,
                        nonPrimaryKeyDbColumnsSetStatement,
                        primaryKeyDbColumnsCondition));

                _updateCommand.Prepare(
                    _allDbColumns.Values.Cast<IDbParameterDefinition>());

                _selectByPrimaryKeyCommand = _sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "SELECT {0} FROM {1} WHERE {2}",
                        _objectCreatorRequiredColumnsNames,
                        TableName,
                        primaryKeyDbColumnsCondition));

                _selectByPrimaryKeyCommand.Prepare(
                    PrimaryKeyDbColumns);

                _deleteByPrimaryKeyCommand = _sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "DELETE FROM {0} WHERE {1}",
                        TableName,
                        primaryKeyDbColumnsCondition));

                _deleteByPrimaryKeyCommand.Prepare(
                    PrimaryKeyDbColumns);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void CreateTable(IEnumerable<DbIndex<TObject>> dbIndexes)
        {
            SqlCeDbCommand sqlCeDbCommandTableExist = null;

            try
            {
                sqlCeDbCommandTableExist = _sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{0}'",
                        TableName));

                if ((int)sqlCeDbCommandTableExist.ExecuteScalar() > 0)
                    return;
            }
            finally
            {
                if (sqlCeDbCommandTableExist != null)
                    sqlCeDbCommandTableExist.Dispose();
            }

            var allColumnsDeclaration = new StringBuilder();

            var first = true;

            foreach (var dbColumn in _allDbColumns.Values)
            {
                allColumnsDeclaration.AppendFormat(
                    first
                        ? "{0} {1} {2}"
                        : ", {0} {1} {2}",
                    dbColumn.Name,
                    dbColumn.DbTypeString,
                    dbColumn.AllowNull ? "NULL" : "NOT NULL");

                first = false;
            }

            SqlCeDbCommand sqlCeDbCommandCreateTable = null;

            try
            {
                sqlCeDbCommandCreateTable = _sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "CREATE TABLE {0} ({1}, PRIMARY KEY ({2}))",
                        TableName,
                        allColumnsDeclaration,
                        GetPrimaryKeyDbColumnNames()));

                sqlCeDbCommandCreateTable.ExecuteNonQuery();
            }
            finally
            {
                if (sqlCeDbCommandCreateTable != null)
                    sqlCeDbCommandCreateTable.Dispose();
            }

            if (dbIndexes == null)
                return;

            foreach (var dbIndex in dbIndexes)
            {
                SqlCeDbCommand sqlCeDbCommandCreateIndex = null;

                try
                {
                    var indexName = new StringBuilder(TableName);
                    var indexColumns = new StringBuilder();

                    foreach (var dbColumn in dbIndex.DbColumns)
                    {
                        indexName.AppendFormat("_{0}", dbColumn.Name);

                        if (indexColumns.Length > 0)
                            indexColumns.Append(",");

                        indexColumns.Append(dbColumn.Name);
                    }

                    sqlCeDbCommandCreateIndex = _sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                        string.Format(
                            "CREATE INDEX {0} ON {1} ({2})",
                            indexName,
                            TableName,
                            indexColumns));

                    sqlCeDbCommandCreateIndex.ExecuteNonQuery();
                }
                finally
                {
                    if (sqlCeDbCommandCreateIndex != null)
                        sqlCeDbCommandCreateIndex.Dispose();
                }
            }
        }

        protected string GetPrimaryKeyDbColumnNames()
        {
            var result = new StringBuilder();

            foreach (var primaryKeyDbColumn in PrimaryKeyDbColumns)
            {
                result.AppendFormat(
                    result.Length > 0
                        ? ", {0}"
                        : "{0}",
                    primaryKeyDbColumn.Name);
            }

            return result.ToString();
        }

        protected void InsertIntoDatabaseInternal(
            TObject objectToInsert,
            SqlCeDbResultSet sqlCeDbResultSet)
        {
            sqlCeDbResultSet.InsertData(
                _allDbColumns.Values.Select(
                    dbColumn =>
                        dbColumn.GetValue(
                            objectToInsert)));
        }

        protected void UpdateDatabaseInternal(TObject updatedObject)
        {
            _updateCommand.ExecuteNonQuery(
                _allDbColumns.Values.Select(
                    dbColumn =>
                        new KeyValuePair<string, object>(
                            dbColumn.Name,
                            dbColumn.GetValue(updatedObject))));
        }

        protected virtual TObject GetFromDatabaseInternal(TPrimaryKey primaryKey)
        {
            try
            {
                return _objectCreator.CreateObject(
                    _selectByPrimaryKeyCommand.ExecuteReader(
                        PrimaryKeyDbColumns.Select(
                            primaryKeyDbColumn =>
                                new KeyValuePair<string, object>(
                                    primaryKeyDbColumn.Name,
                                    primaryKeyDbColumn.GetValueFromPrimaryKey(primaryKey))))
                        .First());
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return null;
        }

        protected ICollection<TObject> GetAllFromDatabaseInternal()
        {
            SqlCeDbCommand sqlCeDbCommand = null;
            var result = new LinkedList<TObject>();

            try
            {
                sqlCeDbCommand = _sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "SELECT {0} FROM {1}",
                        _objectCreatorRequiredColumnsNames,
                        TableName));

                var rows = sqlCeDbCommand.ExecuteReader();

                foreach (var row in rows)
                {
                    result.AddLast(_objectCreator.CreateObject(row));
                }

                return result.Count > 0
                    ? result
                    : null;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (sqlCeDbCommand != null)
                    sqlCeDbCommand.Dispose();
            }

            return null;
        }

        protected ISelectByIndexedColumnsCommand<TObject> CreateSelectByIndexedColumnsCommand(ICollection<IDbParameterDefinition> parameterColumns)
        {
            return new SelectByIndexedColumnsCommand(
                this,
                parameterColumns);
        }

        protected virtual void DeleteFromDatabaseInternal(TPrimaryKey primaryKey)
        {
            try
            {
                _deleteByPrimaryKeyCommand.ExecuteNonQuery(
                    PrimaryKeyDbColumns.Select(
                        primaryKeyDbColumn =>
                            new KeyValuePair<string, object>(
                                primaryKeyDbColumn.Name,
                                primaryKeyDbColumn.GetValueFromPrimaryKey(primaryKey))));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        protected virtual void DeleteAllFromDatabaseInternal()
        {
            SqlCeDbCommand sqlCeDbCommand = null;

            try
            {
                sqlCeDbCommand = _sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "DELETE FROM {0}",
                        TableName));


                sqlCeDbCommand.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (sqlCeDbCommand != null)
                    sqlCeDbCommand.Dispose();
            }
        }
    }
}
