using System;
using System.Data.SqlClient;
using System.Threading;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Threads;
using Contal.Cgp.DBSCreator;
using Contal.Cgp.ORM;

namespace Contal.Cgp.Server
{
    public class DatabaseBackup
    {
        private string _path;
        private Beans.TimeZone _timeZone;
        private string _errorStr;

        private static volatile DatabaseBackup _singleton = null;
        private static object _syncRoot = new object();

        public static DatabaseBackup Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new DatabaseBackup();
                    }

                return _singleton;
            }
        }

        public DatabaseBackup()
        {
            SafeThread.StartThread(DatabaseBackupThread);
        }

        public void Start()
        {
            try
            {
                TimeAxis.Singleton.TimeZoneStateChanged += OnTimeZoneStateChanged;
                ServerGeneralOptionsProvider.Singleton.DatabaseBackupChanged += OnDatabaseBackupChanged;
                _singleton.ReadFromGeneralSettings();
            }
            catch
            { }
        }

        static void OnDatabaseBackupChanged()
        {
            _singleton.ReadFromGeneralSettings();
        }

        static void OnTimeZoneStateChanged(Guid idTimeZone, byte status)
        {
            if (DatabaseBackup.Singleton._timeZone != null)
            {
                if (DatabaseBackup.Singleton._timeZone.IdTimeZone == idTimeZone)
                {
                    if (status == 1)
                    {
                        DatabaseBackup.Singleton.StartDatabaseBackup();
                    }
                }
            }
        }

        public string BackupFileName
        {
            get
            {
                if (_connectString == null)
                {
                    _connectString = Contal.Cgp.ORM.ConnectionString.LoadFromRegistry(CgpServerGlobals.REGISTRY_CONNECTION_STRING);
                }
                return _connectString.DatabaseName + ".Bak";
            }
        }

        private void ReadFromGeneralSettings()
        {
            string timeZoneStringGuid = GeneralOptions.Singleton.TimeZoneGuidString;
            if (timeZoneStringGuid != null && timeZoneStringGuid != string.Empty)
            {
                Guid tzGuid = new Guid(timeZoneStringGuid);
                _timeZone = Contal.Cgp.Server.DB.TimeZones.Singleton.GetById(tzGuid);
            }
            else
            {
                _timeZone = null;
            }

            _path = GeneralOptions.Singleton.DatabaseBackupPath;
        }

        public void StartDatabaseBackup()
        {
            _waitForBackup.Set();
        }

        AutoResetEvent _waitForBackup = new AutoResetEvent(false);
        private void DatabaseBackupThread()
        {
            while (true)
            {
                _waitForBackup.WaitOne();
                if (_backupInProgress)
                    continue;
                DoDatabaseBackup();
            }
        }

        private bool _backupInProgress = false;
        private void DoDatabaseBackup()
        {
            _backupInProgress = true;
            try
            {
                ConnectionString connectString = GetConnectionStringString();
                if (connectString == null || !connectString.IsValid())
                {
                    _errorStr = CgpServer.Singleton.LocalizationHelper.GetString("ErrorNotValidConnectionString");
                    SendErrorBackupDatabase();
                    return;
                }

                if (!DatabaseBackupAndRestore.DatabaseBackup(_path, connectString.DatabaseName, connectString.ToString(), out _errorStr) ||
                    !DatabaseBackupAndRestore.CheckSuccessDatabaseBackup(_path, connectString.DatabaseName, connectString.ToString(), out _errorStr))
                {
                    SendErrorBackupDatabase();
                    return;
                }

                ConnectionString connectStringExternDatabase = GetConnectionStringStringExternDatabase();
                if (connectStringExternDatabase != null && connectStringExternDatabase.IsValid())
                {
                    if (!DatabaseBackupAndRestore.DatabaseBackup(_path, connectStringExternDatabase.DatabaseName, connectStringExternDatabase.ToString(), out _errorStr) ||
                        !DatabaseBackupAndRestore.CheckSuccessDatabaseBackup(_path, connectStringExternDatabase.DatabaseName, connectStringExternDatabase.ToString(), out _errorStr))
                    {
                        SendErrorBackupDatabase();
                        return;
                    }
                }

                SendEventLogBackupDatabase();
            }
            catch { }
            finally
            {
                _backupInProgress = false;
            }
        }

        SqlConnection _sqlConnection;
        ConnectionString _connectString;

        private ConnectionString GetConnectionStringString()
        {
            if (_connectString == null)
            {
                _connectString = Contal.Cgp.ORM.ConnectionString.LoadFromRegistry(CgpServerGlobals.REGISTRY_CONNECTION_STRING);
            }

            return _connectString;
        }

        ConnectionString _connectStringExternDatabase;

        private ConnectionString GetConnectionStringStringExternDatabase()
        {
            if (_connectStringExternDatabase == null)
            {
                _connectStringExternDatabase = Contal.Cgp.ORM.ConnectionString.LoadFromRegistry(CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE);
            }

            return _connectStringExternDatabase;
        }

        private void SendErrorBackupDatabase()
        {
            AlarmsManager.Singleton.AddAlarm(
                new ServerAlarm(
                    new ServerAlarmCore(
                        new Alarm(
                            new AlarmKey(
                                AlarmType.GeneralAlarm,
                                null),
                            AlarmState.Normal),
                        "Database backup",
                        "Database backup",
                        _errorStr)));
        }

        private void SendEventLogBackupDatabase()
        {
            Guid datebaseNameGuid = CentralNameRegisters.Singleton.GetGuidFromName(CentralNameRegister.IMPLICIT_CN_DATABASE_NAME);
            DB.Eventlogs.Singleton.InsertEvent(Beans.Extern.Eventlog.TYPEDATABASEBACKUP, this.GetType().Assembly.GetName().Name,
                new Guid[] { datebaseNameGuid }, "Database backup succeed");
        }

        public bool ForceDatabaseBackup(string path)
        {
            ConnectionString connectString = GetConnectionStringString();
            if (connectString == null || !connectString.IsValid())
            {
                _errorStr = CgpServer.Singleton.LocalizationHelper.GetString("ErrorNotValidConnectionString");
                SendErrorBackupDatabase();
                return false;
            }

            if (!DatabaseBackupAndRestore.DatabaseBackup(path, connectString.DatabaseName, connectString.ToString(), out _errorStr) ||
                !DatabaseBackupAndRestore.CheckSuccessDatabaseBackup(path, connectString.DatabaseName, connectString.ToString(), out _errorStr))
            {
                SendErrorBackupDatabase();
                return false;
            }

            ConnectionString connectStringExternDatabase = GetConnectionStringStringExternDatabase();
            if (connectStringExternDatabase != null && connectStringExternDatabase.IsValid())
            {
                if (!DatabaseBackupAndRestore.DatabaseBackup(path, connectStringExternDatabase.DatabaseName, connectStringExternDatabase.ToString(), out _errorStr) ||
                    !DatabaseBackupAndRestore.CheckSuccessDatabaseBackup(path, connectStringExternDatabase.DatabaseName, connectStringExternDatabase.ToString(), out _errorStr))
                {
                    SendErrorBackupDatabase();
                    return false;
                }
            }

            SendEventLogBackupDatabase();
            return true;
        }
    }
}
