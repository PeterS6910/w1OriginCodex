using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace Contal.Cgp.DBSCreator
{
    public class DatabaseCommandExecutor
    {
        public CreatorProperties CreatorProperties { get; private set; }
        public LocalizationHelper LocalizationHelper { get; private set; }

        public DatabaseCommandExecutor(
            CreatorProperties creatorProperties, 
            LocalizationHelper localizationHelper)
        {
            CreatorProperties = creatorProperties;
            LocalizationHelper = localizationHelper;
        }

        public int RunSqlNonQueryWithParameters(
            string command,
            bool isExtern,
            params SqlParameterTypeAndValue[] parameters)
        {
            string connectionString =
                isExtern
                    ? CreatorProperties.GetConnectionStringLoginToExternDatabase()
                    : CreatorProperties.GetConnectionStringLoginToDatabase();

            using (var sqlConnection = new SqlConnection())
                try
                {
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();

                    string sqlQuery = command;

                    var myCommand =
                        new SqlCommand(
                            sqlQuery,
                            sqlConnection)
                        {
                            CommandTimeout = 7200
                        };

                    if (parameters == null ||
                        parameters.Length <= 0)
                        return myCommand.ExecuteNonQuery();

                    string[] commandParameters =
                        GetParametersNamesFromSqlCommand(command);

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (i >= commandParameters.Length)
                            continue;

                        if (parameters[i].Value == null)
                            parameters[i].Value = DBNull.Value;

                        var parameter = new SqlParameter
                        {
                            ParameterName = commandParameters[i]
                        };

                        if (parameters[i].Type != null)
                            parameter.SqlDbType = parameters[i].Type.Value;

                        parameter.Value = parameters[i].Value;
                        myCommand.Parameters.Add(parameter);
                    }

                    return myCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                    return 0;
                }
        }

        private bool RunSqlNonQuery(string command, int commandTimeout, bool isExtern, out Exception error)
        {
            string connectionString =
                isExtern
                    ? CreatorProperties.GetConnectionStringLoginToExternDatabase()
                    : CreatorProperties.GetConnectionStringLoginToDatabase();

            error = null;

            var sqlConnection = new SqlConnection();

            try
            {
                sqlConnection.ConnectionString = connectionString;
                sqlConnection.Open();

                string sqlQuery = command;

                var myCommand = new SqlCommand(sqlQuery, sqlConnection)
                {
                    CommandTimeout = commandTimeout
                };

                myCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception exception)
            {
                error = exception;
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public bool RunSqlNonQuery(string command, bool isExtern, out Exception error)
        {
            return RunSqlNonQuery(command, 7200, isExtern, out error);
        }

        private static string[] GetParametersNamesFromSqlCommand(string sqlCommand)
        {
            if (String.IsNullOrEmpty(sqlCommand))
                return null;

            var result = new List<string>();
            var parameter = new StringBuilder();

            bool startFound = false;

            foreach (char t in sqlCommand)
            {
                if (t == '@')
                {
                    startFound = true;
                    parameter.Append(t);

                    continue;
                }

                if (!startFound)
                    continue;

                if (t == ' ' ||
                    t == ',' ||
                    t == ')')
                {
                    result.Add(parameter.ToString());
                    parameter = new StringBuilder();
                    startFound = false;

                    continue;
                }

                parameter.Append(t);
            }

            return result.ToArray();
        }

        public List<object[]> RunSqlQuery(string command, bool isExtern, out Exception error)
        {
            string connectionString =
                isExtern
                    ? CreatorProperties.GetConnectionStringLoginToExternDatabase()
                    : CreatorProperties.GetConnectionStringLoginToDatabase();

            using (var sqlConnection = new SqlConnection())
            {
                var result = new List<object[]>();

                try
                {
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();

                    var myCommand = new SqlCommand(command, sqlConnection)
                    {
                        CommandTimeout = 7200
                    };

                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rowData = new List<object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                                rowData.Add(
                                    reader[i] is DBNull
                                        ? null
                                        : reader[i]);

                            result.Add(rowData.ToArray());
                        }

                        error = null;
                        return result;
                    }
                }
                catch (Exception exception)
                {
                    error = exception;
                    return null;
                }
            }
        }

        private bool RunSqlScalarCommandWithResponse(string command, bool isExtern)
        {
            var sqlConnection = new SqlConnection();

            try
            {
                sqlConnection.ConnectionString =
                    isExtern
                        ? CreatorProperties.GetConnectionStringLoginToExternDatabase()
                        : CreatorProperties.GetConnectionStringLoginToDatabase();

                sqlConnection.Open();

                var myCommand = new SqlCommand(command, sqlConnection);

                return myCommand.ExecuteScalar() != null;
            }
            catch
            {
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public bool SqlDropColumn(
            string tableName,
            string columnName,
            bool isExtern,
            out Exception error)
        {
            var sqlCommand = new StringBuilder();

            sqlCommand.Append("if exists ");
            sqlCommand.Append("( select * from INFORMATION_SCHEMA.COLUMNS ");

            sqlCommand.AppendFormat(
                "where TABLE_NAME='{0}' and COLUMN_NAME='{1}') alter table {0} drop COLUMN {1}",
                tableName,
                columnName);

            return RunSqlNonQuery(sqlCommand.ToString(), isExtern, out error);
        }

        public bool ColumnExists(
            string tableName,
            string columnName,
            bool isExtern)
        {
            var sqlCommand = new StringBuilder();

            sqlCommand.AppendFormat(
                "select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='{0}' and COLUMN_NAME='{1}'",
                tableName,
                columnName);

            return
                RunSqlScalarCommandWithResponse(
                sqlCommand.ToString(),
                isExtern);
        }

        public bool SqlDropTable(
            string tableName,
            bool isExtern,
            out Exception error)
        {
            return
                RunSqlNonQuery(
                    string.Format(
                        "IF EXISTS (SELECT 1 FROM sysobjects WHERE xtype='u' AND name='{0}') Drop table [{0}]",
                        tableName),
                    isExtern,
                    out error);
        }

        public bool TableExists(string tableName, bool isExtern)
        {
            return
                RunSqlScalarCommandWithResponse(
                    string.Format(
                        "(SELECT 1 FROM sysobjects WHERE xtype='u' AND name='{0}')", 
                        tableName),
                    isExtern);
        }

        public bool DatabaseIsEmpty(bool isExtern)
        {
            return
                !RunSqlScalarCommandWithResponse(
                    string.Format(
                        "(SELECT 1 FROM sysobjects WHERE xtype='u')"),
                    isExtern);
        }

        public bool RowExists(string command, bool isExtern)
        {
            var sqlConnection = new SqlConnection
            {
                ConnectionString =
                    isExtern
                        ? CreatorProperties.GetConnectionStringLoginToExternDatabase()
                        : CreatorProperties.GetConnectionStringLoginToDatabase()
            };

            try
            {
                sqlConnection.Open();

                var myCommand = new SqlCommand(command, sqlConnection);
                var reader = myCommand.ExecuteReader();

                return reader.HasRows;
            }
            catch
            {
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public void SaveVersionToDatabase(Assembly assembly, bool externDatabase)
        {
            SaveVersionToDatabase(assembly.ManifestModule.Name, GetAssemblyVersion(assembly), externDatabase);
        }

        public void SaveServerVersionToDatabase(Assembly serverAssembly)
        {
            SaveVersionToDatabase(serverAssembly.ManifestModule.Name, GetAssemblyVersion(serverAssembly), false);
        }

        private string GetAssemblyVersion(Assembly assembly)
        {
            AssemblyName name = assembly.GetName();

            Version version = name.Version;
            return version.Build == 0
                ? string.Format("{0}.{1}", version.Major, version.Minor)
                : string.Format("{0}.{1}{2}", version.Major, version.Minor, version.Build);
        }

        public void SaveVersionToDatabase(string assembly, string version, bool externDatabase)
        {
            var sqlConnection = new SqlConnection();

            try
            {
                if (externDatabase)
                {
                    sqlConnection.ConnectionString =
                        CreatorProperties.GetConnectionStringLoginToExternDatabase();
                }
                else
                {
                    sqlConnection.ConnectionString =
                        CreatorProperties.GetConnectionStringLoginToDatabase();
                }

                sqlConnection.Open();

                string sqlQuery = @"select Version from SystemVersion where DbsName = @assemblyName";

                var myCommand = new SqlCommand(sqlQuery, sqlConnection);

                myCommand.Parameters.Add(new SqlParameter("@assemblyName", assembly));

                var vesrionDbs = (string)myCommand.ExecuteScalar();

                sqlQuery =
                    vesrionDbs == null
                        ? @"insert into SystemVersion Values(newid(), @assemblyName, @version)"
                        : @"update SystemVersion set Version = @version where DbsName = @assemblyName";

                myCommand = new SqlCommand(sqlQuery, sqlConnection);

                myCommand.Parameters.Add(new SqlParameter("@assemblyName", assembly));
                myCommand.Parameters.Add(new SqlParameter("@version", version));

                myCommand.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public string GetVersionFromDatabase(string assemblyName, bool externDatabase)
        {
            var sqlConnection = new SqlConnection();

            try
            {
                if (externDatabase && TableExists("SystemVersion", true))
                {
                    sqlConnection.ConnectionString =
                        CreatorProperties.GetConnectionStringLoginToExternDatabase();
                }
                else
                {
                    sqlConnection.ConnectionString =
                        CreatorProperties.GetConnectionStringLoginToDatabase();
                }

                sqlConnection.Open();

                var myCommand =
                    new SqlCommand(
                        @"select Version from SystemVersion where DbsName = @assemblyName",
                        sqlConnection);

                myCommand.Parameters.Add(new SqlParameter("@assemblyName", assemblyName));

                return (string)myCommand.ExecuteScalar();
            }
            catch
            {
                return null;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public bool DeleteVersionFromDatabase(string assemblyName, bool externDatabase, out Exception error)
        {
            var sqlConnection = new SqlConnection();

            try
            {
                if (externDatabase && TableExists("SystemVersion", true))
                {
                    sqlConnection.ConnectionString =
                        CreatorProperties.GetConnectionStringLoginToExternDatabase();
                }
                else
                {
                    sqlConnection.ConnectionString =
                        CreatorProperties.GetConnectionStringLoginToDatabase();
                }

                sqlConnection.Open();

                var myCommand =
                    new SqlCommand(
                        @"delete from SystemVersion where DbsName = @assemblyName",
                        sqlConnection);

                myCommand.Parameters.Add(new SqlParameter("@assemblyName", assemblyName));

                myCommand.ExecuteNonQuery();

                error = null;
                return true;
            }
            catch(Exception ex)
            {
                error = ex;
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public void DoEnsureFixSlipDbs()
        {
            try
            {
                if (!TableExists("EventlogParameter", false) &&
                        !TableExists("EventSource", false) &&
                        !TableExists("Eventlog", false))
                    return;

                if (!Dialog.WarningQuestion(
                        LocalizationHelper.GetString("QuestionDropExternDbs")))
                    return;

                Exception error;

                SqlDropTable("EventlogParameter", false, out error);
                SqlDropTable("EventSource", false, out error);
                SqlDropTable("Eventlog", false, out error);
            }
            catch
            {
            }
        }

        public bool CompareVersion()
        {
            return
                CompareVersion(CreatorProperties.Assemblies, false) &&
                CompareVersion(CreatorProperties.AssembliesEventlogDatabase, true);
        }

        private bool CompareVersion(IEnumerable<Assembly> assemblies, bool externDatabase)
        {
            foreach (Assembly assem in assemblies)
            {
                if (GetAssemblyVersion(assem) != GetVersionFromDatabase(assem.ManifestModule.Name, externDatabase))
                    return false;
            }

            return true;
        }

        public bool RunSqlNonQueryFromStream(
            Stream stream,
            int commandTimeout,
            bool isExtern,
            out Exception error)
        {
            string commandText;

            StreamReader streamReader = null;

            try
            {
                streamReader = new StreamReader(stream);

                commandText = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();
            }

            return
                RunSqlNonQuery(
                    commandText,
                    commandTimeout,
                    isExtern,
                    out error);
        }

        public bool DropProcedure(
            string procedureName, 
            bool isExtern, 
            out Exception error)
        {
            return
                RunSqlNonQuery(
                    string.Format(
                        "if object_id('{0}', 'P') is not null drop procedure {0}",
                        procedureName),
                    isExtern,
                    out error);
        }

        public bool CreateProcedure(
            string procedureName,
            Stream createProcedureStream,
            bool isExtern, 
            out Exception error)
        {
            if (!DropProcedure(procedureName, isExtern, out error))
                return false;

            return
                RunSqlNonQueryFromStream(
                    createProcedureStream,
                    14400,
                    isExtern,
                    out error);
        }
    }
}
