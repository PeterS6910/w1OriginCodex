using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections.Generic;
using System.Text;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public interface ISqlCeConnectionProvider
    {
        SqlCeConnection Connection { get; }

        void StartDatabaseOperaion();
        void FinishDatabaseOperation();
    }

    public interface ISqlCeDbCommandFactory
    {
        SqlCeDbCommand CreateSqlCeDbCommand(string command);
        SqlCeDbResultSet CreateSqlCeDbResultSet(string tableName);
    }

    public class SqlCeDbCommand : IDisposable
    {
        private readonly ISqlCeConnectionProvider _sqlCeConnectionProvider;
        private readonly SqlCeCommand _sqlCommand;

        public SqlCeDbCommand(
            ISqlCeConnectionProvider sqlCeConnectionProvider,
            string command)
        {
            _sqlCeConnectionProvider = sqlCeConnectionProvider;

            _sqlCommand = new SqlCeCommand(
                command,
                _sqlCeConnectionProvider.Connection);
        }

        public int ExecuteNonQuery()
        {
            _sqlCeConnectionProvider.StartDatabaseOperaion();

            try
            {
                return _sqlCommand.ExecuteNonQuery();
            }
            finally
            {
                _sqlCeConnectionProvider.FinishDatabaseOperation();
            }
        }

        public int ExecuteNonQuery(IEnumerable<KeyValuePair<string, object>> parameterNamesAndValues)
        {
            _sqlCeConnectionProvider.StartDatabaseOperaion();

            try
            {
                SetParameters(parameterNamesAndValues);
                return _sqlCommand.ExecuteNonQuery();
            }
            finally
            {
                ClearParametersValue();
                _sqlCeConnectionProvider.FinishDatabaseOperation();
            }
        }

        private void ClearParametersValue()
        {
            if (_sqlCommand.Parameters == null)
                return;

            foreach (SqlCeParameter parameterCreator in _sqlCommand.Parameters)
            {
                parameterCreator.Value = null;
            }
        }

        public object ExecuteScalar()
        {
            _sqlCeConnectionProvider.StartDatabaseOperaion();

            try
            {
                return _sqlCommand.ExecuteScalar();
            }
            finally
            {
                _sqlCeConnectionProvider.FinishDatabaseOperation();
            }
        }

        public object ExecuteScalar(IEnumerable<KeyValuePair<string, object>> parameterNamesAndValues)
        {
            _sqlCeConnectionProvider.StartDatabaseOperaion();

            try
            {
                SetParameters(parameterNamesAndValues);
                return _sqlCommand.ExecuteScalar();
            }
            finally
            {
                ClearParametersValue();
                _sqlCeConnectionProvider.FinishDatabaseOperation();
            }
        }

        public IEnumerable<object[]> ExecuteReader()
        {
            _sqlCeConnectionProvider.StartDatabaseOperaion();

            try
            {
                var values = ExecuteReaderCore();

                foreach (var value in values)
                {
                    yield return value;
                }
            }
            finally
            {
                _sqlCeConnectionProvider.FinishDatabaseOperation();
            }
        }

        private IEnumerable<object[]> ExecuteReaderCore()
        {
            SqlCeDataReader reader = null;

            try
            {
                reader = _sqlCommand.ExecuteReader();

                var values = new object[reader.FieldCount];

                while (reader.Read())
                {
                    reader.GetValues(values);

                    yield return values;
                }
            }
            finally
            {
                if (reader != null)
                    try
                    {
                        reader.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
            }
        }

        public IEnumerable<object[]> ExecuteReader(IEnumerable<KeyValuePair<string, object>> parameterNamesAndValues)
        {
            _sqlCeConnectionProvider.StartDatabaseOperaion();

            try
            {
                SetParameters(parameterNamesAndValues);
                var values =  ExecuteReaderCore();

                foreach (var value in values)
                {
                    yield return value;
                }
            }
            finally
            {
                ClearParametersValue();
                _sqlCeConnectionProvider.FinishDatabaseOperation();
            }
        }

        public void Prepare(IEnumerable<IDbParameterDefinition> parameterColumns)
        {
            _sqlCeConnectionProvider.StartDatabaseOperaion();

            try
            {
                foreach (var parameterColumn in parameterColumns)
                {
                    var size = parameterColumn.DbSize;

                    var parameter =
                        size.HasValue
                            ? new SqlCeParameter(
                                parameterColumn.Name,
                                parameterColumn.DbType,
                                size.Value)
                            : new SqlCeParameter(
                                parameterColumn.Name,
                                parameterColumn.DbType);

                    _sqlCommand.Parameters.Add(parameter);
                }

                _sqlCommand.Prepare();
            }
            finally
            {
                _sqlCeConnectionProvider.FinishDatabaseOperation();
            }
        }

        private void SetParameters(IEnumerable<KeyValuePair<string, object>> parameterNamesAndValues)
        {
            if (parameterNamesAndValues == null)
                return;

            foreach (var parameterNameAndValue in parameterNamesAndValues)
            {
                _sqlCommand.Parameters[parameterNameAndValue.Key].Value = parameterNameAndValue.Value;
            }
        }

        public void Dispose()
        {
            _sqlCommand.Dispose();
        }
    }
}
