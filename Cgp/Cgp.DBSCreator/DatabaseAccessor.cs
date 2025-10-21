using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.DBSCreator
{
    internal class DatabaseAccessor
    {
        public readonly CreatorProperties CreatorProperties;
        public bool UserCreated { get; private set; }

        public DatabaseAccessor(
            CreatorProperties creatorProperties,
            LocalizationHelper localizationHelper)
        {
            CreatorProperties = creatorProperties;
            LocalizationHelper = localizationHelper;
        }

        public LocalizationHelper LocalizationHelper { get; private set; }

        public bool DatabasesExist
        {
            get
            {
                bool mainDatabaseExists = MainDatabaseExists;

                //if (!CreatorProperties.EnableExternDatabase)
                //    return mainDatabaseExists;

                return mainDatabaseExists && ExternDatabaseExists;
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
                            CreatorProperties.GetConnectionStringPrincipalToMaster()
                    };

                var myCommand =
                    new SqlCommand(
                        @"select db_id(@databaseName)",
                        sqlConnection);

                myCommand.Parameters.Add(
                    new SqlParameter(
                        "@databaseName",
                        CreatorProperties.DatabaseName));

                sqlConnection.Open();

                object result = myCommand.ExecuteScalar();

                sqlConnection.Close();

                return result.ToString() != string.Empty;

            }
        }

        public bool ExternDatabaseExists
        {
            get
            {
                //if (!CreatorProperties.EnableExternDatabase)
                //    return false;

                var sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString =
                            CreatorProperties
                                .GetConnectionStringPrincipalToMasterExternDatabase()
                    };

                var myCommand =
                    new SqlCommand(
                        @"select db_id(@databaseName)",
                        sqlConnection);

                myCommand.Parameters.Add(
                    new SqlParameter(
                        "@databaseName",
                        CreatorProperties.ExternDatabaseName));

                sqlConnection.Open();

                object result = myCommand.ExecuteScalar();

                sqlConnection.Close();

                return result.ToString() != string.Empty;
            }
        }

        public static bool IsValidConnectionString(string connectionString)
        {
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString = connectionString
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
                        CreatorProperties.DatabaseLogin));

                sqlConnection.Open();

                object obj = myCommand.ExecuteScalar();

                return obj != null &&
                       obj.ToString() == CreatorProperties.DatabaseLogin;
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
                    CreatorProperties.GetConnectionStringPrincipalToMaster());
        }

        //private bool UserExistsInExternDatabase()
        //{
        //    //if (!CreatorProperties.EnableExternDatabase)
        //    //    return false;

        //    return
        //        UserExistsInDatabase(
        //            CreatorProperties.GetConnectionStringPrincipalToMasterExternDatabase());
        //}

        public bool UserExists()
        {
            bool retValue = UserExistsInMainDatabase();
            //if (retValue)
            //    retValue = UserExistsInExternDatabase();

            return retValue;
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
                        ConnectionString = CreatorProperties.GetConnectionStringPrincipalToMaster()
                    };

                sqlConnection.Open();

                var parameters = new List<SqlParameter>();

                var myCommand =
                    new SqlCommand(
                        "CREATE LOGIN [" + CreatorProperties.DatabaseLogin + "]" + @" WITH PASSWORD = '" + CreatorProperties.DatabaseLogPassword + "'",
                        sqlConnection);

                myCommand.Parameters.AddRange(parameters.ToArray());
                myCommand.ExecuteNonQuery();

                //update server roles
                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + CreatorProperties.DatabaseLogin + "], 'dbcreator'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + CreatorProperties.DatabaseLogin + "], 'securityadmin'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + CreatorProperties.DatabaseLogin + "], 'sysadmin'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                //only one SQL server is supported therefore single login will be used 

                //if (_creatorProperties.EnableExternDatabase && _creatorProperties.DatabaseName != _creatorProperties.ExternDatabaseName)
                //{
                //    sqlQuery = "CREATE LOGIN [" + _creatorProperties.DatabaseLogin + "]";
                //    sqlQuery += @" WITH PASSWORD = '" + _creatorProperties.DatabaseLogPassword + "'";

                //    sqlConnectionExternDatabase = new SqlConnection();
                //    sqlConnectionExternDatabase.ConnectionString = _creatorProperties.GetConnectionStringPrincipalToMasterExternDatabase();

                //    sqlConnectionExternDatabase.Open();
                //    myCommand = new SqlCommand(sqlQuery, sqlConnectionExternDatabase);
                //    myCommand.Parameters.AddRange(parameters.ToArray());
                //    myCommand.ExecuteNonQuery();
                //    //update server roles
                //    sqlQuery = @"sp_addsrvrolemember [" + _creatorProperties.DatabaseLogin + "], 'dbcreator'";
                //    myCommand = new SqlCommand(sqlQuery, sqlConnectionExternDatabase);
                //    myCommand.ExecuteNonQuery();
                //    sqlQuery = @"sp_addsrvrolemember [" + _creatorProperties.DatabaseLogin + "], 'securityadmin'";
                //    myCommand = new SqlCommand(sqlQuery, sqlConnectionExternDatabase);
                //    myCommand.ExecuteNonQuery();
                //    sqlQuery = @"sp_addsrvrolemember [" + _creatorProperties.DatabaseLogin + "], 'sysadmin'";
                //    myCommand = new SqlCommand(sqlQuery, sqlConnectionExternDatabase);
                //    myCommand.ExecuteNonQuery();
                //}

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
                        ConnectionString = CreatorProperties.GetConnectionStringPrincipalToMaster()
                    };

                var myCommand =
                    new SqlCommand(
                        @"drop login [" + CreatorProperties.DatabaseLogin + "]",
                        sqlConnection);

                sqlConnection.Open();

                myCommand.ExecuteNonQuery();

                //only one SQL server is supported therefore single login will be used 

                //if (!CreatorProperties.EnableExternDatabase ||
                //    CreatorProperties.DatabaseServer == CreatorProperties.DatabaseServerForExternDatabase)
                //{
                //    return true;
                //}

                //sqlConnection = 
                //    new SqlConnection
                //    {
                //        ConnectionString =
                //            CreatorProperties.GetConnectionStringPrincipalToMasterExternDatabase()
                //    };

                //myCommand = 
                //    new SqlCommand(
                //        @"drop login [" + CreatorProperties.DatabaseLogin + "]", 
                //        sqlConnection);

                //sqlConnection.Open();

                //myCommand.ExecuteNonQuery();

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
                            CreatorProperties
                                .GetConnectionStringPrincipalToMaster()
                    };

                sqlConnection.Open();

                var parameters = new List<SqlParameter>();

                var myCommand =
                    new SqlCommand(
                        "ALTER LOGIN [" + CreatorProperties.DatabaseLogin + "] WITH PASSWORD = '" + CreatorProperties.DatabaseLogPassword + "'",
                        sqlConnection);

                myCommand.Parameters.AddRange(parameters.ToArray());
                myCommand.ExecuteNonQuery();

                //update server roles
                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + CreatorProperties.DatabaseLogin + "], 'dbcreator'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + CreatorProperties.DatabaseLogin + "], 'securityadmin'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                myCommand =
                    new SqlCommand(
                        @"sp_addsrvrolemember [" + CreatorProperties.DatabaseLogin + "], 'sysadmin'",
                        sqlConnection);

                myCommand.ExecuteNonQuery();

                //only one SQL server is supported therefore will be uses single login

                //if (_creatorProperties.EnableExternDatabase && _creatorProperties.DatabaseName != _creatorProperties.ExternDatabaseName)
                //{
                //    sqlQuery = "ALTER LOGIN [" + _creatorProperties.DatabaseLogin + "] WITH PASSWORD = '" + _creatorProperties.DatabaseLogPassword + "'";
                //    sqlConnectionExternDatabase = new SqlConnection();
                //    sqlConnectionExternDatabase.ConnectionString = _creatorProperties.GetConnectionStringPrincipalToMasterExternDatabase();

                //    sqlConnectionExternDatabase.Open();
                //    myCommand = new SqlCommand(sqlQuery, sqlConnectionExternDatabase);
                //    myCommand.Parameters.AddRange(parameters.ToArray());
                //    myCommand.ExecuteNonQuery();
                //    //update server roles
                //    sqlQuery = @"sp_addsrvrolemember [" + _creatorProperties.DatabaseLogin + "], 'dbcreator'";
                //    myCommand = new SqlCommand(sqlQuery, sqlConnectionExternDatabase);
                //    myCommand.ExecuteNonQuery();
                //    sqlQuery = @"sp_addsrvrolemember [" + _creatorProperties.DatabaseLogin + "], 'securityadmin'";
                //    myCommand = new SqlCommand(sqlQuery, sqlConnectionExternDatabase);
                //    myCommand.ExecuteNonQuery();
                //    sqlQuery = @"sp_addsrvrolemember [" + _creatorProperties.DatabaseLogin + "], 'sysadmin'";
                //    myCommand = new SqlCommand(sqlQuery, sqlConnectionExternDatabase);
                //    myCommand.ExecuteNonQuery();
                //}
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

            CreatorProperties.DatabaseLogPassword = "@H+#A" + builder;
        }

        protected void InvokeOnError(Exception ex)
        {
            if (OnError != null)
                OnError(ex);
        }

        public bool PrepareCustomLogin(bool existingLogin)
        {
            try
            {
                if (existingLogin)
                {
                    if (!IsValidConnectionString(
                        CreatorProperties.GetConnectionStringLoginToMaster()))
                    {
                        Dialog.Error(LocalizationHelper.GetString("ErrorWrongLoginNameOrPassword"));
                        return false;
                    }

                    if (AlterUser(false))
                        return true;

                    Dialog.Error(LocalizationHelper.GetString("ErrorAlterUserFailed"));
                    return false;
                }

                if (UserExists())
                {
                    Dialog.Error(LocalizationHelper.GetString("ErrorLoginAlreadyExist"));
                    return false;
                }

                if (CreateUser(false))
                    return true;

                Dialog.Error(LocalizationHelper.GetString("ErrorCreateUserFailed"));
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool PrepareLogin(
            ICreateOrUpdateForm createOrUpdateForm)
        {
            switch (CreatorProperties.LoginType)
            {
                case LoginType.UseCgpLogin:

                    if (!UserExists())
                        return CreateUser(true);

                    if (IsValidConnectionString(
                        CreatorProperties.GetConnectionStringLoginToMaster()))
                        return true;

                    if (!Dialog.WarningQuestion(
                        LocalizationHelper.GetString("QuestionAlterDbsLogin") + " " +
                        CreatorProperties.DatabaseLogin + " ?"))
                        return false;

                    return AlterUser(true);

                case LoginType.UseCustomLogin:

                    if (!createOrUpdateForm.PrepareCustomLogin())
                        return false;

                    CreatorProperties
                        .GetConnectionString()
                        .SaveToRegistryLoginPassword();

                    break;

                case LoginType.UseSaLogin:

                    CreatorProperties
                        .GetConnectionString()
                        .SaveToRegistryLoginPassword();

                    break;
            }

            return true;
        }

        protected void CreateEventlogDatabaseTables()
        {
            var sqlConnection = new SqlConnection();

            try
            {
                sqlConnection.ConnectionString =
                    /*CreatorProperties.EnableExternDatabase
                        ?*/ CreatorProperties.GetConnectionStringLoginToExternDatabase();
                        //: CreatorProperties.GetConnectionStringLoginToDatabase();

                sqlConnection.Open();

                var commandTextStreams =
                    CreatorProperties.AssembliesEventlogDatabase
                        .Select(
                            assembly =>
                                assembly.GetManifestResourceStream(
                                    assembly.GetName().Name + ".CreateSchema.sql"))
                        .Where(stream => stream != null);

                foreach (Stream commandTextStream in commandTextStreams)
                {
                    string commandText;

                    using (TextReader textReader = new StreamReader(commandTextStream))
                        commandText = textReader.ReadToEnd();

                    var createSchemaCommand =
                        new SqlCommand(
                            commandText,
                            sqlConnection);

                    createSchemaCommand.ExecuteNonQuery();
                }
            }
            finally
            {
                sqlConnection.Close();
                SqlConnection.ClearPool(sqlConnection);
            }
        }
    }
}