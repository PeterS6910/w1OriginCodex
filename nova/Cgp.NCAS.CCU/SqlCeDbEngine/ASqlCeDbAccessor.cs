using System;
using System.Collections.Generic;
using JetBrains.Annotations;

using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public abstract class ASqlCeDbAccessor<TObject, TPrimaryKey>
        : ASqlCeDbAccessorBase<TObject, TPrimaryKey>
        where TObject : class
    {
        protected readonly HashSet<TPrimaryKey> PrimaryKeyValues = new HashSet<TPrimaryKey>();

        protected ASqlCeDbAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            string tableName,
            [NotNull] IEnumerable<IPrimaryKeyDbColumn<TObject, TPrimaryKey>> primaryKeyDbColumns,
            ICollection<DbIndex<TObject>> dbIndexes,
            [NotNull] IObjectCreator<TObject> objectCreator)
            : base(
                sqlCeDbCommandFactory,
                tableName,
                primaryKeyDbColumns,
                dbIndexes,
                objectCreator)
        {
        }

        protected abstract TPrimaryKey CreatePrimaryKey(Dictionary<string, object> primaryKeyDbColumnsValues);

        protected void LoadPrimaryKeysInternal()
        {
            SqlCeDbCommand sqlCeDbCommand = null;

            try
            {
                sqlCeDbCommand = SqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "SELECT {0} FROM {1}",
                        GetPrimaryKeyDbColumnNames(),
                        TableName));

                var rows = sqlCeDbCommand.ExecuteReader();

                var primaryKeyDbColumnsValues = new Dictionary<string, object>();

                int numColumns = PrimaryKeyDbColumns.Length;

                foreach (var row in rows)
                {
                    primaryKeyDbColumnsValues.Clear();

                    for (int columnIndex = 0; columnIndex < numColumns; columnIndex++)
                        primaryKeyDbColumnsValues.Add(
                            PrimaryKeyDbColumns[columnIndex].Name,
                            row[columnIndex]);

                    PrimaryKeyValues.Add(
                        CreatePrimaryKey(primaryKeyDbColumnsValues));
                }
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

        protected void SaveToDatabaseInternal(
            IEnumerable<KeyValuePair<TPrimaryKey, TObject>> primaryKeyAndObjects,
            Action<TObject> beforeSaveToDatabase,
            Action<TObject> afterSaveToDatabase)
        {
            SqlCeDbResultSet sqlCeDbResultSet = null;
            try
            {
                sqlCeDbResultSet = SqlCeDbCommandFactory.CreateSqlCeDbResultSet(TableName);

                foreach (var primaryKeyAndObject in primaryKeyAndObjects)
                {
                    var primaryKey = primaryKeyAndObject.Key;
                    var obj = primaryKeyAndObject.Value;

                    if (beforeSaveToDatabase != null)
                        beforeSaveToDatabase(obj);

                    if (PrimaryKeyValues.Contains(primaryKey))
                    {
                        UpdateDatabaseInternal(obj);
                    }
                    else
                    {
                        InsertIntoDatabaseInternal(
                            obj,
                            sqlCeDbResultSet);

                        PrimaryKeyValues.Add(primaryKey);
                    }

                    if (afterSaveToDatabase != null)
                        afterSaveToDatabase(obj);
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

        protected sealed override TObject GetFromDatabaseInternal(TPrimaryKey primaryKey)
        {
            return PrimaryKeyValues.Contains(primaryKey)
                ? base.GetFromDatabaseInternal(primaryKey) 
                : null;
        }

        protected sealed override void DeleteFromDatabaseInternal(TPrimaryKey primaryKey)
        {
            if (PrimaryKeyValues.Remove(primaryKey))
                base.DeleteFromDatabaseInternal(primaryKey);
        }

        protected sealed override void DeleteAllFromDatabaseInternal()
        {
            base.DeleteAllFromDatabaseInternal();
            PrimaryKeyValues.Clear();
        }
    }
}
