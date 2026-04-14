using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbResultSet : IDisposable
    {
        private readonly SqlCeResultSet _sqlCeResultSet;

        public SqlCeDbResultSet(
            ISqlCeConnectionProvider sqlCeConnectionProvider,
            string tableName)
        {
            var sqlCommand = new SqlCeCommand(
                tableName,
                sqlCeConnectionProvider.Connection)
            {
                CommandType = CommandType.TableDirect
            };

            _sqlCeResultSet = sqlCommand.ExecuteResultSet(ResultSetOptions.Updatable);
        }

        public void InsertData(IEnumerable<object> data)
        {
            var index = 0;

            var sqlCeUpdateRecord = _sqlCeResultSet.CreateRecord();

            foreach (var columnData in data)
                sqlCeUpdateRecord.SetValue(
                    index++,
                    columnData);

            _sqlCeResultSet.Insert(sqlCeUpdateRecord);
        }

        public void Dispose()
        {
            _sqlCeResultSet.Dispose();
        }
    }
}
