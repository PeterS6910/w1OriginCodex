using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace AnalyticTool.Advitech
{
    public class DatabaseAccessor
    {
        private static DatabaseAccessor _singleton;

        public static DatabaseAccessor Singleton
        {
            get { return _singleton ?? (_singleton = new DatabaseAccessor()); }
        }

        public bool UserCreated { get; private set; }
        public bool DatabasesExist
        {
            get
            {
                return MainDatabaseExists;
            }
        }

        /// <summary>
        /// Check if the database exists
        /// </summary>
        /// <returns>true if exists</returns>
        public bool MainDatabaseExists
        {
            get
            {
                var sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString =
                            ApplicationProperties.Singleton.GetConnectionStringForMaster()
                    };

                var myCommand =
                    new SqlCommand(
                        @"select db_id(@databaseName)",
                        sqlConnection);

                myCommand.Parameters.Add(
                    new SqlParameter(
                        "@databaseName",
                        ApplicationProperties.Singleton.DatabaseName));

                sqlConnection.Open();

                object result = myCommand.ExecuteScalar();

                sqlConnection.Close();

                return result.ToString() != string.Empty;
            }
        }

        public bool IsValidMasterConnectionString()
        {
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString = ApplicationProperties.Singleton.GetConnectionStringForMaster()
                    };

                sqlConnection.Open();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();
            }
        }

        public bool IsValidConnectionString()
        {
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString = ApplicationProperties.Singleton.GetConnectionString()
                    };

                sqlConnection.Open();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();
            }
        }

        private bool UserExistsInDatabase(string connectionString)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString = connectionString
                    };

                var myCommand =
                    new SqlCommand(
                        @"select loginname from master.dbo.syslogins where name = @databaseLogin",
                        sqlConnection);

                myCommand.Parameters.Add(
                    new SqlParameter(
                        "@databaseLogin",
                        ApplicationProperties.Singleton.DatabaseLogin));

                sqlConnection.Open();

                object obj = myCommand.ExecuteScalar();

                return obj != null &&
                       obj.ToString() == ApplicationProperties.Singleton.DatabaseLogin;
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();
            }
        }

        /// <summary>
        /// Check if the login exists
        /// </summary>
        /// <returns>true if exists</returns>
        private bool UserExistsInMainDatabase()
        {
            return
                UserExistsInDatabase(
                    ApplicationProperties.Singleton.GetConnectionStringForMaster());
        }

        public bool UserExists()
        {
            return UserExistsInMainDatabase();
        }

        public event DException2Void OnError;

        /// <summary>
        /// Create the login
        /// </summary>
        /// <returns>true on success</returns>
        protected bool CreateUser(bool generatePassword)
        {
            SqlConnection sqlConnection = null;

            try
            {
                if (generatePassword)
                    GeneratePassword();

                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString = ApplicationProperties.Singleton.GetConnectionStringForMaster()
                    };

                sqlConnection.Open();

                var parameters = new List<SqlParameter>();

                var myCommand =
                    new SqlCommand(
                        "CREATE LOGIN [" + ApplicationProperties.Singleton.DatabaseLogin + "]" + @" WITH PASSWORD = '" + ApplicationProperties.Singleton.DatabasePassword + "'",
                        sqlConnection);

                myCommand.Parameters.AddRange(parameters.ToArray());
                myCommand.ExecuteNonQuery();

                //update server roles
                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + ApplicationProperties.Singleton.DatabaseLogin + "], 'dbcreator'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + ApplicationProperties.Singleton.DatabaseLogin + "], 'securityadmin'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + ApplicationProperties.Singleton.DatabaseLogin + "], 'sysadmin'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                UserCreated = true;
                return true;
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);

                InvokeOnError(ex);

                return false;
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();
            }
        }

        /// <summary>
        /// try drop login
        /// </summary>
        /// <returns>true on success</returns>
        protected bool DropLogin()
        {
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString = ApplicationProperties.Singleton.GetConnectionStringForMaster()
                    };

                var myCommand =
                    new SqlCommand(
                        @"drop login [" + ApplicationProperties.Singleton.DatabaseLogin + "]",
                        sqlConnection);

                sqlConnection.Open();

                myCommand.ExecuteNonQuery();

                UserCreated = false;
                return true;
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);

                InvokeOnError(ex);

                return false;
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();
            }
        }

        public bool AlterUser(bool generatePassword)
        {
            SqlConnection sqlConnection = null;

            try
            {
                if (generatePassword)
                    GeneratePassword();

                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString =
                            ApplicationProperties
                                .Singleton
                                .GetConnectionStringForMaster()
                    };

                sqlConnection.Open();

                var parameters = new List<SqlParameter>();

                var myCommand =
                    new SqlCommand(
                        "ALTER LOGIN [" + ApplicationProperties.Singleton.DatabaseLogin + "] WITH PASSWORD = '" 
                        + ApplicationProperties.Singleton.DatabasePassword + "'",
                        sqlConnection);

                myCommand.Parameters.AddRange(parameters.ToArray());
                myCommand.ExecuteNonQuery();

                //update server roles
                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + ApplicationProperties.Singleton.DatabaseLogin + "], 'dbcreator'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + ApplicationProperties.Singleton.DatabaseLogin + "], 'securityadmin'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + ApplicationProperties.Singleton.DatabaseLogin + "], 'sysadmin'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);

                InvokeOnError(ex);

                return false;
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();
            }
        }

        /// <summary>
        /// to the _logPassword save generated password
        /// </summary>
        private void GeneratePassword()
        {
            string logPassword = DateTime.Now.Ticks.ToString();
            SHA256 sha = new SHA256Managed();
            byte[] bytes = sha.ComputeHash(Encoding.ASCII.GetBytes(logPassword));

            var builder = new StringBuilder("", 0x20);

            foreach (byte t in bytes)
                builder.Append(t.ToString("x2").ToLower());

            ApplicationProperties.Singleton.DatabasePassword = "@H+#A" + builder;
        }

        protected void InvokeOnError(Exception ex)
        {
            if (OnError != null)
                OnError(ex);
        }

        public bool PrepareCustomLogin(bool existingLogin, bool generatePassword)
        {
            try
            {
                if (existingLogin)
                {
                    if (!IsValidMasterConnectionString())
                    {
                        Dialog.Error("ErrorWrongLoginNameOrPassword");
                        return false;
                    }

                    if (AlterUser(generatePassword))
                        return true;

                    Dialog.Error("ErrorAlterUserFailed");
                    return false;
                }

                if (UserExists())
                {
                    Dialog.Error("ErrorLoginAlreadyExist");
                    return false;
                }

                if (CreateUser(generatePassword))
                    return true;

                Dialog.Error("ErrorCreateUserFailed");
                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetDatabases()
        {
            var databases = new List<string>();

            using (var sqlConnection =
                    new SqlConnection(ApplicationProperties.Singleton.GetConnectionStringForMaster()))
                try
                {
                    sqlConnection.Open();

                    var myCommand = new SqlCommand("sp_databases", sqlConnection);

                    var myReader = myCommand.ExecuteReader();

                    while (myReader.Read())
                        databases.Add(myReader.GetString(0));

                    return databases;
                }
                catch (Exception)
                {
                    return null;
                }
        }

        public List<string> GetTables(string databaseName)
        {
            var tables = new List<string>();

            using (var sqlConnection =
                    new SqlConnection(ApplicationProperties.Singleton.GetConnectionStringToDatabase(databaseName)))
                try
                {
                    sqlConnection.Open();

                    var myCommand = new SqlCommand("SELECT TABLE_NAME FROM information_schema.tables", sqlConnection);

                    var myReader = myCommand.ExecuteReader();

                    while (myReader.Read())
                        tables.Add(myReader.GetString(0));

                    return tables;
                }
                catch (Exception)
                {
                    return null;
                }
        }

        private bool TableExists(string tableName)
        {
            var tables = GetTables(ApplicationProperties.Singleton.DatabaseName);

            return tables.Any(table => table == tableName);
        }

        public bool CheckDatabaseAndTable()
        {
            try
            {
                if (!DatabasesExist)
                {
                    var sqlConnection =
                        new SqlConnection
                        {
                            ConnectionString =
                                ApplicationProperties.Singleton.GetConnectionStringForMaster()
                        };

                    var myCommand =
                        new SqlCommand(
                            string.Format("CREATE DATABASE {0};", ApplicationProperties.Singleton.DatabaseName),
                            sqlConnection);

                    sqlConnection.Open();
                    myCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }

                if (!TableExists(ApplicationProperties.Singleton.TableName))
                {
                    var sqlConnection =
                        new SqlConnection
                        {
                            ConnectionString =
                                ApplicationProperties.Singleton.GetConnectionStringToDatabase(ApplicationProperties.Singleton.DatabaseName)
                        };

                    var myCommand =
                        new SqlCommand(
                            string.Format(
                                "CREATE TABLE {0} (EventId int IDENTITY(1,1) PRIMARY KEY, FirstName varchar(100), Surname varchar(100), "
                                +
                                "Company varchar(100), Address varchar(100), Email varchar(100), PhoneNumber varchar(100), Identification varchar(100), "
                                +
                                "CostCenter varchar(100), Department varchar(100), EmploymentBeginningDate datetime, EmploymentEndDate datetime, CardNumber varchar(100), "
                                +
                                "Pump1 int, Pump2 int, Pump3 int, Pump4 int, Start datetime, Stop datetime, Invoiced int)",
                                ApplicationProperties.Singleton.TableName),
                            sqlConnection);

                    sqlConnection.Open();
                    myCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }

                if (!TableExists("SystemData"))
                {
                    var sqlConnection =
                        new SqlConnection
                        {
                            ConnectionString =
                                ApplicationProperties.Singleton.GetConnectionStringToDatabase(ApplicationProperties.Singleton.DatabaseName)
                        };

                    var myCommand = new SqlCommand("CREATE TABLE SystemData (LastId bigint)", sqlConnection);

                    sqlConnection.Open();
                    myCommand.ExecuteNonQuery();
                    myCommand.CommandText = "INSERT INTO SystemData (LastId) VALUES('-1')";
                    myCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}