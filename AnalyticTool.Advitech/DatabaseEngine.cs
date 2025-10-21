using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Contal.IwQuick.Sys;

namespace AnalyticTool.Advitech
{
    public class DatabaseEngine
    {
        private readonly SqlConnection _sqlConnection;
        private readonly string _tableName;

        public DatabaseEngine(SqlConnectionStringBuilder connectionStringBuilder, string tableName)
        {
            _tableName = tableName;

            try
            {
                _sqlConnection = new SqlConnection(connectionStringBuilder.ConnectionString);
                _sqlConnection.Open();
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Console.WriteLine(ex.ToString());
            }
        }

        public long GetLastId()
        {
            if (_sqlConnection.State != ConnectionState.Open)
                _sqlConnection.Open();

            var sqlCommand = new SqlCommand("SELECT TOP 1 LastId FROM SystemData", _sqlConnection);
            return (long)sqlCommand.ExecuteScalar();
        }

        public bool InsertRecords(ICollection<DatabaseRecord> databaseRecords, long lastId)
        {
            if (databaseRecords == null
                || databaseRecords.Count == 0)
            {
                return false;
            }

            try
            {
                if (_sqlConnection.State != ConnectionState.Open)
                    _sqlConnection.Open();

                var transaction = _sqlConnection.BeginTransaction();

                var sqlCommand = new SqlCommand(
                            "DELETE FROM SystemData",
                            _sqlConnection,
                            transaction);

                try
                {
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.CommandText = string.Format(
                        "INSERT INTO SystemData (LastId) VALUES ('{0}')",
                        lastId);
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }

                foreach (var databaseRecord in databaseRecords)
                {
                    try
                    {
                        sqlCommand.CommandText = GetSqlCommandString(databaseRecord);
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetSqlCommandString(DatabaseRecord databaseRecords)
        {
            string employmentBeginningDate = databaseRecords.EmploymentBeginningDate == null
                ? string.Empty
                : databaseRecords.EmploymentBeginningDate.ToString();

            string employmentEndDate = databaseRecords.EmploymentEndDate == null
                ? string.Empty
                : databaseRecords.EmploymentEndDate.ToString();

            return string.Format(
                "INSERT INTO {0} (FirstName, Surname, Company, Address, Email, PhoneNumber, Identification, CostCenter, Department, EmploymentBeginningDate, " +
                "EmploymentEndDate, CardNumber, Pump1, Pump2, Pump3, Pump4, Start, Stop, Invoiced) VALUES('{1}','{2}','{3}','{4}','{5}','{6}','{7}'" +
                ",'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}')"
                , _tableName
                , databaseRecords.FirstName
                , databaseRecords.Surname
                , databaseRecords.Company
                , databaseRecords.Address
                , databaseRecords.Email
                , databaseRecords.PhoneNumber
                , databaseRecords.Identification
                , databaseRecords.CostCenter
                , databaseRecords.Department
                , employmentBeginningDate
                , employmentEndDate
                , databaseRecords.CardNumber
                , databaseRecords.Pump1
                , databaseRecords.Pump2
                , databaseRecords.Pump3
                , databaseRecords.Pump4
                , databaseRecords.Start
                , databaseRecords.Stop
                , databaseRecords.Invoiced);
        }
    }
}
