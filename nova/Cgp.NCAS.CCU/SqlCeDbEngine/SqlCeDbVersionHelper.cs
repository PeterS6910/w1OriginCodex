using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public sealed class SqlCeDbVersionHelper : ASingleton<SqlCeDbVersionHelper>
    {
        private sealed class ObjectTypeDbColumnDefinition :
            ASingleton<ObjectTypeDbColumnDefinition>,
            IDbColumnDefinition
        {
            private ObjectTypeDbColumnDefinition()
                : base(null)
            {
            }

            public string Name{ get { return "objectType"; } }
            public string DbTypeString{ get { return "tinyint"; } }
            public SqlDbType DbType{ get { return SqlDbType.TinyInt; } }
            public int? DbSize{ get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private sealed class VersionDbColumnDefinition :
            ASingleton<VersionDbColumnDefinition>,
            IDbColumnDefinition
        {
            private VersionDbColumnDefinition()
                : base(null)
            {
            }

            public string Name{ get { return "version"; } }
            public string DbTypeString{ get { return "integer"; } }
            public SqlDbType DbType{ get { return SqlDbType.Int; } }
            public int? DbSize{ get { return null; } }
            public bool AllowNull { get { return false; } }
        }

        private const string TABLE_NAME = "TableVersions";

        private volatile object _operationLock = new object();

        private readonly Dictionary<byte, int> _versions = new Dictionary<byte, int>();

        private SqlCeDbCommand _insertVersionCommand;
        private SqlCeDbCommand _updateVersionCommand;


        private SqlCeDbVersionHelper()
            : base(null)
        {
        }

        public void Init(ISqlCeDbCommandFactory sqlCeDbCommandFactory)
        {
            lock (_operationLock)
                try
                {
                    InitTable(sqlCeDbCommandFactory);

                    _insertVersionCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                        string.Format(
                            "INSERT INTO {0} ({1}, {2}) VALUES (@{1}, @{2})",
                            TABLE_NAME,
                            ObjectTypeDbColumnDefinition.Singleton.Name,
                            VersionDbColumnDefinition.Singleton.Name));

                    _insertVersionCommand.Prepare(
                        new[]
                        {
                            (IDbParameterDefinition) ObjectTypeDbColumnDefinition.Singleton,
                            VersionDbColumnDefinition.Singleton
                        });

                    _updateVersionCommand = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                        string.Format(
                            "UPDATE {0} SET {1} = @{1} WHERE {2} = @{2}",
                            TABLE_NAME,
                            VersionDbColumnDefinition.Singleton.Name,
                            ObjectTypeDbColumnDefinition.Singleton.Name));

                    _updateVersionCommand.Prepare(
                        new[]
                        {
                            (IDbParameterDefinition) VersionDbColumnDefinition.Singleton,
                            ObjectTypeDbColumnDefinition.Singleton
                        });
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
        }

        private void InitTable(ISqlCeDbCommandFactory sqlCeDbCommandFactory)
        {
            SqlCeDbCommand sqlCeDbCommandTableExist = null;

            try
            {
                sqlCeDbCommandTableExist = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{0}'",
                        TABLE_NAME));

                if ((int)sqlCeDbCommandTableExist.ExecuteScalar() > 0)
                {
                    SqlCeDbCommand sqlCeDbCommandSelectVersions = null;

                    try
                    {
                        sqlCeDbCommandSelectVersions = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                            string.Format(
                                "SELECT {0}, {1} FROM {2}",
                                ObjectTypeDbColumnDefinition.Singleton.Name,
                                VersionDbColumnDefinition.Singleton.Name,
                                TABLE_NAME));

                        var rows = sqlCeDbCommandSelectVersions.ExecuteReader();

                        foreach (var row in rows)
                        {
                            _versions.Add(
                                (byte)row[0],
                                (int)row[1]);
                        }
                    }
                    finally
                    {
                        if (sqlCeDbCommandSelectVersions != null)
                            sqlCeDbCommandSelectVersions.Dispose();
                    }

                    return;
                }
            }
            finally
            {
                if (sqlCeDbCommandTableExist != null)
                    sqlCeDbCommandTableExist.Dispose();
            }

            SqlCeDbCommand sqlCeDbCommandCreateTable = null;

            try
            {
                sqlCeDbCommandCreateTable = sqlCeDbCommandFactory.CreateSqlCeDbCommand(
                    string.Format(
                        "CREATE TABLE {0} ({1} {2} PRIMARY KEY, {3} {4})",
                        TABLE_NAME,
                        ObjectTypeDbColumnDefinition.Singleton.Name,
                        ObjectTypeDbColumnDefinition.Singleton.DbTypeString,
                        VersionDbColumnDefinition.Singleton.Name,
                        VersionDbColumnDefinition.Singleton.DbTypeString));

                sqlCeDbCommandCreateTable.ExecuteNonQuery();
            }
            finally
            {
                if (sqlCeDbCommandCreateTable != null)
                    sqlCeDbCommandCreateTable.Dispose();
            }
        }

        public void SaveVersion(ObjectType objectType, int version)
        {
            lock (_operationLock)
            {
                try
                {
                    var parameters = Enumerable.Repeat(
                        new KeyValuePair<string, object>(
                            ObjectTypeDbColumnDefinition.Singleton.Name,
                            (byte)objectType),
                        1)
                        .Concat(Enumerable.Repeat(
                            new KeyValuePair<string, object>(
                                VersionDbColumnDefinition.Singleton.Name,
                                version),
                            1));

                    if (_versions.ContainsKey((byte)objectType))
                        _updateVersionCommand.ExecuteNonQuery(parameters);
                    else
                        _insertVersionCommand.ExecuteNonQuery(parameters);

                    _versions[(byte)objectType] = version;
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        public int ReadVersion(ObjectType objectType)
        {
            lock (_operationLock)
            {
                int version;

                if (_versions.TryGetValue(
                    (byte)objectType,
                    out version))
                {
                    return version;
                }

                return -1;
            }
        }
    }
}
