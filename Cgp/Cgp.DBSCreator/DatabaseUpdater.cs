using System;

using Contal.Cgp.ORM;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.DBSCreator
{
    internal class DatabaseUpdater : DatabaseAccessor
    {
        public enum ResultEnum
        {
            Init,
            GoBack,
            Cancel,
            Done
        };

        private readonly DatabaseCommandExecutor _databaseCommandExecutor;

        private bool? _createdDatabaseBackup;
        private SafeThread<IFormUpdateDatabase> _threadUpdate;

        public ResultEnum Result { get; private set; }
        public CompareOrmModel CompareOrmModel { get; private set; }

        public DatabaseUpdater(
            CreatorProperties creatorProperties,
            LocalizationHelper localizationHelper)
            : base(creatorProperties, localizationHelper)
        {
            Result = ResultEnum.Init;

            _databaseCommandExecutor =
                new DatabaseCommandExecutor(
                    CreatorProperties,
                    LocalizationHelper);
        }

        private void UpdateTables()
        {
            try
            {
                bool writeVersionMainAfterUpdateSchema =
                    _databaseCommandExecutor.DatabaseIsEmpty(false);

                bool writeVersionExternAfterUpdateSchema =
                    _databaseCommandExecutor.DatabaseIsEmpty(true);

                NhHelper.Singleton.ConnectionString =
                    CreatorProperties.GetConnectionString();

                NhHelper.Singleton.UpdateSchema(CreatorProperties.Assemblies);

                UpdateEventlogTables();

                if (writeVersionMainAfterUpdateSchema)
                {
                    Exception error;

                    ImportData.ImportGraphicsSymbols(_databaseCommandExecutor, out error);

                    ImportData.InsertTimetecSetting(_databaseCommandExecutor, out error);

                    foreach (var assembly in CreatorProperties.Assemblies)
                        _databaseCommandExecutor.SaveVersionToDatabase(
                            assembly,
                            false);
                }

                if (writeVersionExternAfterUpdateSchema)
                    foreach (var assembly in CreatorProperties.AssembliesEventlogDatabase)
                        _databaseCommandExecutor.SaveVersionToDatabase(
                            assembly,
                            true);
            }
            catch (Exception ex)
            {
                Dialog.Error(ex.ToString());

                Result = ResultEnum.Cancel;

                InvokeOnError(ex);
            }
        }

        private void UpdateEventlogTables()
        {
            bool eventLogTableExist =
                _databaseCommandExecutor.TableExists(
                    "EventLog", true);

            bool eventSourceTableExist =
                _databaseCommandExecutor.TableExists(
                    "EventSource", true);

            bool eventlogParameterTableExist =
                _databaseCommandExecutor.TableExists(
                    "EventlogParameter", true);

            if (eventLogTableExist &&
                eventSourceTableExist &&
                eventlogParameterTableExist)
            {
                return;
            }

            if (eventLogTableExist ||
                eventSourceTableExist ||
                eventlogParameterTableExist)
            {
                throw new ArgumentException("Some eventlog tables exist, but not all of them");
            }

            CreateEventlogDatabaseTables();
        }

        public bool CreatedDatabaseBackup
        {
            get
            {
                return _createdDatabaseBackup ?? false;
            }
        }

        public void Update(IFormUpdateDatabase formUpdateDatabase)
        {
            _threadUpdate =
                SafeThread<IFormUpdateDatabase>.StartThread(
                    UpdateInternal,
                    formUpdateDatabase);
        }

        public void UpdateInternal(IFormUpdateDatabase formUpdateDatabase)
        {
            CompareOrmModel = null;

            Result = ResultEnum.Init;

            if (_createdDatabaseBackup == null)
            {
                // Show question for create database backup, if it is first running this metod
                _createdDatabaseBackup = false;

                if (Dialog.Question(LocalizationHelper.GetString("QuestionBackupDatabase")))
                {
                    // Create database backup
                    formUpdateDatabase.ShowStartDatabaseBackup();

                    if (!CreateDatabaseBackup())
                    {
                        Result = ResultEnum.Cancel;

                        formUpdateDatabase.CloseSync();
                        return;
                    }

                    formUpdateDatabase.ShowStopDatabaseBackup(_createdDatabaseBackup.Value);
                }
            }

            UpdateTables();

            formUpdateDatabase.StopProgress();

            //if (CreatorProperties.EnableExternDatabase)
            _databaseCommandExecutor.DoEnsureFixSlipDbs();

            if (!_databaseCommandExecutor.CompareVersion())
                if (!formUpdateDatabase.PerformVersionBasedDatabaseConversion(_databaseCommandExecutor))
                {
                    Result = ResultEnum.Cancel;
                    formUpdateDatabase.OnUpdateFinished();

                    return;
                }

            CompareOrmModel = new CompareOrmModel(_databaseCommandExecutor);
            CompareOrmModel.RunCompareAll();

            Result =
                CompareOrmModel.ModelOk
                    ? ResultEnum.Done
                    : ResultEnum.Cancel;

            formUpdateDatabase.OnUpdateFinished();
        }

        private static bool CreateDatabaseBackup(ConnectionString connectionString)
        {
            if (connectionString == null || !connectionString.IsValid())
                // Return false, if connection string to master database is not valid.
                return false;

            string error;

            return
                DatabaseBackupAndRestore.DatabaseBackup(
                    String.Empty,
                    connectionString.DatabaseName,
                    connectionString.ToString(),
                    out error) &&
                DatabaseBackupAndRestore.CheckSuccessDatabaseBackup(
                    String.Empty,
                    connectionString.DatabaseName,
                    connectionString.ToString(),
                    out error);
        }

        /// <summary>
        /// Backup the database. Return true if backup the database succeeded otherwise false.
        /// </summary>
        /// <returns></returns>
        private bool CreateDatabaseBackup()
        {
            bool result = CreateDatabaseBackup(CreatorProperties.GetConnectionString());

            if (result)
                result = CreateDatabaseBackup(CreatorProperties.GetConnectionStringExternDatabase());

            if (!result)
                return Dialog.ErrorQuestion(
                    LocalizationHelper
                        .GetString("QuestionBackupDatabaseFailedContinueUpdate"));

            _createdDatabaseBackup = true;
            Dialog.Info(LocalizationHelper.GetString("TextBackupDatabaseSucceeded"));

            return true;
        }

        private static bool RestoreDatabaseBackup(string databaseName, string connectionString)
        {
            string error;

            if (string.IsNullOrEmpty(connectionString))
                return false;

            return
                DatabaseBackupAndRestore.DatabaseRestore(
                    String.Empty,
                    databaseName,
                    connectionString,
                    out error);
        }

        public bool RestoreDatabaseBackup()
        {
            NhHelper.Singleton.Disconnect();

            bool result =
                RestoreDatabaseBackup(
                    CreatorProperties.DatabaseName,
                    CreatorProperties.GetConnectionStringLoginToMaster());

            if (result)
                result =
                    RestoreDatabaseBackup(
                        CreatorProperties.ExternDatabaseName,
                        CreatorProperties.GetConnectionStringLoginToMasterExternDatabase());

            // Restore the database
            if (result)
                Dialog.Info(LocalizationHelper.GetString("TextRestoreDatabaseSucceeded"));
            else
                Dialog.Error(LocalizationHelper.GetString("TextRestoreDatabaseFailed"));

            return result;
        }

        public void OnCancel(IFormUpdateDatabase formUpdateDatabase)
        {
            if (_threadUpdate != null &&
                _threadUpdate.Thread != null &&
                _threadUpdate.Thread.IsAlive)
            {
                if (!Dialog.Question("Stop progress"))
                    return;

                _threadUpdate.Abort();
                _threadUpdate = null;
            }

            Result = ResultEnum.Cancel;

            formUpdateDatabase.CloseSync();
        }

        public void OnGoBack(IFormUpdateDatabase formUpdateDatabase)
        {
            Result = ResultEnum.GoBack;

            formUpdateDatabase.CloseSync();
        }
    }
}