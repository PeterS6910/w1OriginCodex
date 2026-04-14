using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

using Contal.Cgp.ORM;
using System.Data.SqlClient;
using Contal.IwQuick;
using System.Reflection;

namespace Contal.Cgp.DBSCreator
{
    public class CreatorProperties
    {
        public class DatabaseFileParams
        {
            private readonly int _initialSize;
            private readonly int _maxSize;
            private readonly int _fileGrowth;

            public DatabaseFileParams(
                int initialSize,
                int maxSize,
                int fileGrowth)
            {
                _initialSize = initialSize;
                _maxSize = maxSize;
                _fileGrowth = fileGrowth;
            }

            private static void AppendName(
                StringBuilder commandText,
                string fileName)
            {
                commandText.AppendFormat(
                    "(name = '{0}'",
                    fileName);
            }

            private void AppendSize(StringBuilder commandText)
            {
                commandText.AppendFormat(
                    ", size = {0}MB",
                    _initialSize);
            }

            private void AppendMaxSize(StringBuilder commandText)
            {
                if (_maxSize > 0)
                    commandText.AppendFormat(
                        ", maxsize = {0}MB",
                        _maxSize);
                else
                    if (_maxSize == -1)
                        commandText.Append(", maxsize = UNLIMITED");
            }

            private void AppendFileGrowth(StringBuilder commandText)
            {
                commandText.AppendFormat(
                    ", filegrowth = {0}%",
                    _fileGrowth);
            }

            public void AppendToCreationCommandText(
                StringBuilder creationCommandText,
                string fileName,
                string directoryName)
            {
                AppendName(creationCommandText, fileName);

                creationCommandText.AppendFormat(
                    ", filename = '{0}{1}.mdf'",
                    directoryName,
                    fileName);

                AppendSize(creationCommandText);
                AppendMaxSize(creationCommandText);
                AppendFileGrowth(creationCommandText);

                creationCommandText.Append(')');
            }

            public bool AppendToUpdateCommandText(
                StringBuilder updateCommandText,
                string fileName,
                int currentSize,
                int currentMaxSize,
                int currentFileGrowth,
                bool currentPercentGrowth)
            {
                bool fileParametersChanged = false;

                AppendName(updateCommandText, fileName);

                if (currentSize < _initialSize)
                {
                    AppendSize(updateCommandText);
                    fileParametersChanged = true;
                }

                if (currentMaxSize != -1)
                    if (currentMaxSize < _maxSize || _maxSize <= 0)
                    {
                        AppendMaxSize(updateCommandText);
                        fileParametersChanged = true;
                    }

                if (!currentPercentGrowth || currentFileGrowth < _fileGrowth)
                {
                    AppendFileGrowth(updateCommandText);
                    fileParametersChanged = true;
                }

                updateCommandText.Append(')');

                return fileParametersChanged;
            }
        }

        private string _databaseServer;
        private string _databaseName;
        private string _databasePrincipalUser;
        private string _databasePrincipalUserPassword;
        private string _databaseServerForExternDatabase;
        private string _externDatabaseName;
        private string _databasePrincipalUserExternDatabase;
        private string _databasePrincipalUserPasswordExternDatabase;
        private string _databaseLogin;
        private string _databaseLoginPassword;

        public string DatabaseServer
        {
            get { return _databaseServer; }
            set
            {
                Validator.CheckNullString(value);
                _databaseServer = value;
            }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                Validator.CheckNullString(value);
                _databaseName = value;
            }
        }

        public string DatabaseServerForExternDatabase
        {
            get { return _databaseServerForExternDatabase; }
            set
            {
                Validator.CheckNullString(value);
                _databaseServerForExternDatabase = value;
            }
        }

        public string ExternDatabaseName
        {
            get { return _externDatabaseName; }
            set
            {
                Validator.CheckNullString(value);
                _externDatabaseName = value;
            }
        }

        public string DatabasePrincipalUser
        {
            get { return _databasePrincipalUser; }
            set
            {
                Validator.CheckNullString(value);
                _databasePrincipalUser = value;
            }
        }

        public string DatabasePrincipalUserExternDatabase
        {
            get { return _databasePrincipalUserExternDatabase; }
            set
            {
                Validator.CheckNullString(value);
                _databasePrincipalUserExternDatabase = value;
            }
        }

        public string DatabasePrincipalUserPassword
        {
            get { return _databasePrincipalUserPassword; }
            set
            {
                Validator.CheckNullString(value);
                _databasePrincipalUserPassword = value;
            }
        }

        public string DatabasePrincipalUserPasswordExternDatabase
        {
            get { return _databasePrincipalUserPasswordExternDatabase; }
            set
            {
                Validator.CheckNullString(value);
                _databasePrincipalUserPasswordExternDatabase = value;
            }
        }

        public string DatabaseLogin
        {
            get { return _databaseLogin; }
            set
            {
                Validator.CheckNullString(value);
                _databaseLogin = value;
            }
        }

        public string DatabaseLogPassword
        {
            get { return _databaseLoginPassword; }
            set
            {
                Validator.CheckNullString(value);
                _databaseLoginPassword = value;
            }
        }

        public string DatabasePath { get; set; }

        public string ExternDatabasePath { get; set; }

        public LoginType LoginType { get; set; }

        public IList<FileSystemAccessRule> RulesList { get; set; }

        public IList<FileSystemAccessRule> RulesListExternDatabase { get; set; }

        public IEnumerable<Assembly> Assemblies { get; private set; }

        public IEnumerable<Assembly> AssembliesEventlogDatabase { get; private set; }

        public DatabaseFileParams MainDatabaseDataFileParams
        {
            get
            {
                return
                    new DatabaseFileParams(
                        100,
                        -1,
                        20);
            }
        }

        public DatabaseFileParams MainDatabaseLogFileParams
        {
            get
            {
                return
                    new DatabaseFileParams(
                        100,
                        -1,
                        50);
            }
        }

        public DatabaseFileParams ExternDatabaseDataFileParams
        {
            get
            {
                return
                    new DatabaseFileParams(
                        512,
                        -1,
                        20);
            }
        }

        public DatabaseFileParams ExternDatabaseLogFileParams
        {
            get
            {
                return
                    new DatabaseFileParams(
                        512,
                        -1,
                        50);
            }
        }

        public CreatorProperties(
            ConnectionString connectionString, 
            IEnumerable<Assembly> assemblies, 
            ConnectionString connectionStringExternDatabase, 
            IEnumerable<Assembly> assembliesEventlogDatabase)
        {
            _databaseServer = connectionString.DatabaseServer;
            _databaseName = connectionString.DatabaseName;
            _databaseLogin = connectionString.DatabaseLogin;
            _databaseLoginPassword = connectionString.DatabaseLogPassword;
            Assemblies = assemblies;
            _databaseServerForExternDatabase = connectionStringExternDatabase.DatabaseServer;
            _externDatabaseName = connectionStringExternDatabase.DatabaseName;
            AssembliesEventlogDatabase = assembliesEventlogDatabase;
        }

        public string GetConnectionStringLoginToMaster()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                DataSource = _databaseServer,
                InitialCatalog = "master",
                UserID = _databaseLogin,
                Password = _databaseLoginPassword,
                IntegratedSecurity = false,
                ConnectTimeout = ConnectionString.DEFAULT_CONNECTION_TIMEOUT
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public string GetConnectionStringLoginToMasterExternDatabase()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                DataSource = _databaseServerForExternDatabase,
                InitialCatalog = "master",
                UserID = _databaseLogin,
                Password = _databaseLoginPassword,
                IntegratedSecurity = false,
                ConnectTimeout = ConnectionString.DEFAULT_CONNECTION_TIMEOUT
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public string GetConnectionStringPrincipalToMaster()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                DataSource = _databaseServer,
                InitialCatalog = "master",
                UserID = _databasePrincipalUser,
                Password = _databasePrincipalUserPassword,
                IntegratedSecurity = false,
                ConnectTimeout = ConnectionString.DEFAULT_CONNECTION_TIMEOUT
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public string GetConnectionStringPrincipalToMasterExternDatabase()
        {
            var sqlConnectionStringBuilder = 
                new SqlConnectionStringBuilder
                {
                    PersistSecurityInfo = true,
                    DataSource = _databaseServerForExternDatabase,
                    InitialCatalog = "master",
                    UserID = _databasePrincipalUser,
                    Password = _databasePrincipalUserPassword,
                    IntegratedSecurity = false,
                    ConnectTimeout = ConnectionString.DEFAULT_CONNECTION_TIMEOUT
                };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public string GetConnectionStringLoginToDatabase()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                InitialCatalog = _databaseName,
                DataSource = _databaseServer,
                UserID = _databaseLogin,
                Password = _databaseLoginPassword,
                IntegratedSecurity = false,
                ConnectTimeout = ConnectionString.DEFAULT_CONNECTION_TIMEOUT
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public string GetConnectionStringLoginToExternDatabase()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                PersistSecurityInfo = true,
                InitialCatalog = _externDatabaseName,
                DataSource = _databaseServerForExternDatabase,
                UserID = _databaseLogin,
                Password = _databaseLoginPassword,
                IntegratedSecurity = false,
                ConnectTimeout = ConnectionString.DEFAULT_CONNECTION_TIMEOUT
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public ConnectionString GetConnectionString()
        {
            return 
                new ConnectionString(
                    _databaseServer, 
                    _databaseName, 
                    _databaseLogin, 
                    _databaseLoginPassword, 
                    false);
        }

        public ConnectionString GetConnectionStringExternDatabase()
        {
            return
                new ConnectionString(
                    _databaseServerForExternDatabase,
                    _externDatabaseName,
                    _databaseLogin,
                    _databaseLoginPassword,
                    false);
        }

        public bool IsDbsVariableOk()
        {
            return
                !string.IsNullOrEmpty(_databaseName) &&
                !string.IsNullOrEmpty(_databaseServer) &&
                !string.IsNullOrEmpty(_databaseLogin) &&
                !string.IsNullOrEmpty(_databaseLoginPassword) &&
                (!string.IsNullOrEmpty(_externDatabaseName) &&
                 !string.IsNullOrEmpty(_databaseServerForExternDatabase));
        }
    }

    /// <summary>
    /// Enum for database login type
    /// </summary>
    public enum LoginType : byte
    {
        UseCgpLogin = 0x01,
        UseCustomLogin = 0x02,
        UseSaLogin = 0x03
    }
}
