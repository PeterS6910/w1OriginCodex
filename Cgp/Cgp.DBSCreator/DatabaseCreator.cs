using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;

using Contal.Cgp.ORM;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;
using JetBrains.Annotations;

namespace Contal.Cgp.DBSCreator
{
    internal class DatabaseCreator : DatabaseAccessor
    {
        private bool _mainDatabaseExists;
        private bool _externDatabaseExists;

        public string ErrorMessage { get; private set; }

        public bool DatabaseCreated { get; private set; }

        public DatabaseCreator(
            [NotNull] CreatorProperties creatorProperties,
            LocalizationHelper localizationHelper)
            : base(creatorProperties, localizationHelper)
        {
            Validator.CheckForNull(creatorProperties,"creatorProperties");
        }

        /// <summary>
        /// Create the database
        /// </summary>
        /// <returns>true on success</returns>
        private bool CreateDatabases()
        {
            _mainDatabaseExists = MainDatabaseExists;
            _externDatabaseExists = ExternDatabaseExists;

            bool externDatabaseWillBeCreated = !_externDatabaseExists;

            if (_mainDatabaseExists && !externDatabaseWillBeCreated)
            {
                ErrorMessage = LocalizationHelper.GetString("ErrorDatabaseAlreadyExists");
                return false;
            }

            if (!_mainDatabaseExists)
            {
                string databasePath = CreatorProperties.DatabasePath;

                if (!CreateDatabase(
                        CreatorProperties.GetConnectionStringLoginToMaster(),
                        CreatorProperties.MainDatabaseDataFileParams,
                        CreatorProperties.MainDatabaseLogFileParams, 
                        false,
                        ref databasePath,
                        CreatorProperties.DatabaseName))
                    return false;

                CreatorProperties.DatabasePath = databasePath;
            }

            if (externDatabaseWillBeCreated)
            {
                string externDatabasePath = CreatorProperties.ExternDatabasePath;

                if (!CreateDatabase(
                        CreatorProperties.GetConnectionStringLoginToMasterExternDatabase(),
                        CreatorProperties.ExternDatabaseDataFileParams,
                        CreatorProperties.ExternDatabaseLogFileParams,
                        true,
                        ref externDatabasePath,
                        CreatorProperties.ExternDatabaseName))
                    return false;

                CreatorProperties.ExternDatabasePath = externDatabasePath;
            }

            return true;
        }

        public void Create(IFormBeforeExecution formBeforeExecution)
        {
            if (!CreateLoginAndDatabases())
            {
                UndoCreate();
                formBeforeExecution.ShowCreateDatabaseFailure();

                return;
            }

            formBeforeExecution.ShowCreateDatabaseSuccess();

            if (CreateTables())
            {
                formBeforeExecution.ShowBeforeExecutionSuccess();
                return;
            }

            UndoCreate();
            formBeforeExecution.ShowCreateTablesFailure();
        }

        private bool CreateTables()
        {
            var databaseCommandExecutor =
                new DatabaseCommandExecutor(
                    CreatorProperties,
                    LocalizationHelper);

            try
            {
                NhHelper.Singleton.ConnectionString =
                    CreatorProperties.GetConnectionString();

                if (!_mainDatabaseExists)
                {
                    NhHelper.Singleton.CreateSchema(CreatorProperties.Assemblies);

                    Exception error;

                    if (!ImportData.ImportGraphicsSymbols(databaseCommandExecutor, out error))
                        throw error;

                    if (!ImportData.InsertTimetecSetting(databaseCommandExecutor, out error))
                        throw error;

                    foreach (var assembly in CreatorProperties.Assemblies)
                        databaseCommandExecutor.SaveVersionToDatabase(assembly, false);
                }

                bool eventlogDatabaseExists =_externDatabaseExists;

                if (!eventlogDatabaseExists)
                {
                    CreateEventlogDatabaseTables();

                    foreach (var assembly in CreatorProperties.AssembliesEventlogDatabase)
                        databaseCommandExecutor.SaveVersionToDatabase(assembly, true);
                }
            }
            catch (Exception ex)
            {
                Dialog.Error(ex.ToString());

                InvokeOnError(ex);

                NhHelper.Singleton.Disconnect();

                return false;
            }

            try
            {
                databaseCommandExecutor.DoEnsureFixSlipDbs();
            }
            catch (Exception ex)
            {
                Dialog.Warning(ex.ToString());
            }

            return true;
        }

        private void UndoCreate()
        {
            if (DatabaseCreated)
                if (Dialog.WarningQuestion(
                        LocalizationHelper.GetString("FormBeforeExecutionQuestionDropDatabase")))
                {
                    if (DropDatabase())
                        Dialog.Info(LocalizationHelper.GetString("FormBeforeExecutionDatabaseCancelled"));
                    else
                        Dialog.Error(LocalizationHelper.GetString("FormBeforeExecutionDropDatabaseFail"));
                }

            if (!UserCreated)
                return;

            if (!Dialog.Question(
                    LocalizationHelper.GetString("FormBeforeExecutionQuestionDeleteUser")))
                return;

            if (DropLogin())
                Dialog.Info(LocalizationHelper.GetString("FormBeforeExecutionUserCancelled"));
            else
                Dialog.Error(LocalizationHelper.GetString("FormBeforeExecutionDropUserFail"));
        }

        private bool CreateDatabase(
            string connectionString,
            CreatorProperties.DatabaseFileParams dataFileParams,
            CreatorProperties.DatabaseFileParams logFileParams,
            bool forceSimpleRecoveryModel,
            ref string databasePath, 
            string databaseName)
        {
            var sqlConnection = new SqlConnection();

            try
            {
                sqlConnection.ConnectionString = connectionString;

                sqlConnection.Open();

                if (string.IsNullOrEmpty(databasePath))
                    databasePath = GetDatabasePath(sqlConnection);
                else
                    if (!PrepareDatabasePath(databasePath))
                        return false;

                if (databasePath[databasePath.Length - 1] != '\\')
                    databasePath += '\\';

                var creationCommandText = new StringBuilder();

                creationCommandText.AppendFormat(
                    "create database [{0}] on primary ",
                    databaseName);

                dataFileParams.AppendToCreationCommandText(
                    creationCommandText,
                    databaseName + "dat",
                    databasePath);

                creationCommandText.Append(" log on ");

                logFileParams.AppendToCreationCommandText(
                    creationCommandText,
                    databaseName + "log",
                    databasePath);

                if (forceSimpleRecoveryModel)
                {
                    creationCommandText.Append(';');
                    creationCommandText.AppendLine();

                    creationCommandText.AppendFormat(
                        "alter database [{0}] set recovery simple",
                        databaseName);
                }

                var creationCommand =
                    new SqlCommand(
                        creationCommandText.ToString(),
                        sqlConnection);

                creationCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);

                InvokeOnError(ex);

                return false;
            }
            finally
            {
                sqlConnection.Close();
                SqlConnection.ClearPool(sqlConnection);
            }

            return true;
        }

        private bool PrepareDatabasePath(string databasePath)
        {
            if (!Directory.Exists(databasePath))
                try
                {
                    Directory.CreateDirectory(databasePath);
                }
                catch
                {
                    ErrorMessage = LocalizationHelper.GetString("ErrorDirectoryCreationFailed");

                    return false;
                }

            if (CreatorProperties.RulesList == null)
                return true;

            if (!Dialog.Question(
                    LocalizationHelper.GetString("QuestionSetRightsForSQL")))
                return true;

            var dirInfo = new DirectoryInfo(databasePath);

            System.Security.AccessControl.DirectorySecurity dirSec = 
                dirInfo.GetAccessControl();

            foreach (System.Security.AccessControl.FileSystemAccessRule rule in 
                    CreatorProperties.RulesList)
                dirSec.AddAccessRule(rule);

            dirInfo.SetAccessControl(dirSec);

            return true;
        }

        private static string GetDatabasePath(SqlConnection sqlConnection)
        {
            var myCommandGetPath =
                new SqlCommand(
                    "select physical_name from sys.database_files",
                    sqlConnection);

            using (SqlDataReader sqlReader = myCommandGetPath.ExecuteReader())
            {

                if (!sqlReader.Read())
                    return null;

                string result = sqlReader.GetString(0);

                result =
                    result.Substring(
                        0,
                        result.LastIndexOf('\\') + 1);

                return result;
            }
        }

        /// <summary>
        /// try drop the database
        /// </summary>
        /// <returns>true on success</returns>
        private bool DropDatabase()
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
                        @"drop database [" + CreatorProperties.DatabaseName + "]",
                        sqlConnection);

                sqlConnection.Open();
                myCommand.ExecuteNonQuery();

                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString = CreatorProperties.GetConnectionStringPrincipalToMasterExternDatabase()
                    };

                myCommand =
                    new SqlCommand(
                        @"drop database [" + CreatorProperties.ExternDatabaseName + "]",
                        sqlConnection);

                sqlConnection.Open();

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
        /// Runnig the whole creation of Login + DBS
        /// </summary>
        /// <returns></returns>
        private bool CreateLoginAndDatabases()
        {
            try
            {
                //Check if connection is available
                if (!IsConnectionValid())
                {
                    ErrorMessage = "The connection is invalid.";
                    return false;
                }

                //create login
                if (!UserExists())
                    if (!CreateUser(true))
                    {
                        ErrorMessage = "Create login failed.";
                        return false;
                    }

                //create database via login
                //login will be db_owner for database
                if (!CreateDatabases())
                    return false;

                DatabaseCreated = true;
                return true;
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);

                InvokeOnError(ex);

                return false;
            }
        }

        /// <summary>
        /// Check connection if is valid
        /// </summary>
        /// <returns>true on success</returns>
        private bool IsConnectionValid()
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection =
                    new SqlConnection
                    {
                        ConnectionString = CreatorProperties.GetConnectionStringPrincipalToMasterExternDatabase()
                    };

                sqlConnection.Open();

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

        public string ObtainLoginForDbs()
        {
            //select suser_sname(sid) from master.dbo.sysdatabases where name = 'CgpServer'
            string loginDbs = string.Empty;

            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = 
                    new SqlConnection
                    {
                        ConnectionString = CreatorProperties.GetConnectionStringPrincipalToMaster()
                    };

                sqlConnection.Open();

                var myCommand = 
                    new SqlCommand(
                        @"select suser_sname(sid) from master.dbo.sysdatabases where name = @databaseName", 
                        sqlConnection);

                myCommand.Parameters.Add(
                    new SqlParameter(
                        "@databaseName", 
                        CreatorProperties.DatabaseName));

                loginDbs = (string)myCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
            finally
            {
                if (sqlConnection != null)
                    sqlConnection.Close();
            }

            return loginDbs;
        }
    }
}

/*
use testHB
go
CREATE USER _userForDbs FOR LOGIN _login;
GO
EXEC sp_addrolemember 'db_owner', '_userForDbs'
*/