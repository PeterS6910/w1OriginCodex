using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using Contal.Cgp.ORM;
using Contal.IwQuick;
using System.IO;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;
using Contal.SLA.Client;

namespace Contal.Cgp.DBSCreator
{
    public class Manager
    {
        private const int CANCELED = 0;
        private const int RUN_SERVERCONFIGURE = 1;
        private const int RUN_USERCONFIGURE = 2;
        private const int RUN_DATABASECONFIGURE = 3;
        private const int RUN_DATABASESETTINGS = 4;
        private const int CREATE_DATABASE = 7;
        private const int UPDATE_DATABASE = 8;
        private const int CONFIGURE_DONE = 9;

        private const string DBS_LOGIN_NAME = "cgpLogin";
        private string _oldLogin = String.Empty;

        private readonly Assembly _serverAssembly;
        private readonly ConnectionString _connectionString;
        private readonly ConnectionString _connectionStringExternDatabase;

        private readonly CreatorProperties _creatorProperties;

        private bool _runAsService;
        private readonly LocalizationHelper _localizationHelper;

        public event DException2Void OnError;

        //public void SetAssemblies(IEnumerable<Assembly> assemblies) 
        //{
        //    if (null != assemblies)
        //        _assemblies = assemblies;
        //}

        #region Properties
        public bool RunAsService
        {
            get { return _runAsService; }
            set { _runAsService = value; }
        }
        #endregion

        /// <summary>
        /// Create manager
        /// </summary>
        /// <param name="connectionString">string to connect to database</param>
        /// <param name="assemblies">assemblies for resources</param>
        /// <param name="assembliesExternDatabase"></param>
        /// <param name="isService">if is run as service</param>
        /// <param name="selectLanguage">String with selected language</param>
        /// <param name="connectionStringExternDatabase"></param>
        public Manager(
            Assembly serverAssembly,
            ConnectionString connectionString, 
            IEnumerable<Assembly> assemblies, 
            ConnectionString connectionStringExternDatabase, 
            IEnumerable<Assembly> assembliesExternDatabase, 
            bool isService, 
            string selectLanguage)
        {
            _serverAssembly = serverAssembly;
            _runAsService = isService;

            if (connectionString == null)
                connectionString = new ConnectionString();

            _connectionString = connectionString;

            if (connectionStringExternDatabase == null)
                connectionStringExternDatabase = new ConnectionString();

            _connectionStringExternDatabase = connectionStringExternDatabase;

            _localizationHelper = new LocalizationHelper(GetType().Assembly);

            if (selectLanguage != "")
                _localizationHelper.SetLanguage(selectLanguage);

            _creatorProperties = 
                new CreatorProperties(
                    _connectionString, 
                    assemblies, 
                    _connectionStringExternDatabase, 
                    assembliesExternDatabase);

            if (_creatorProperties.DatabaseLogin == String.Empty)
                _creatorProperties.DatabaseLogin = 
                    DBS_LOGIN_NAME + "_" +
                    GetHashedExternalCIID();
        }

        /// <summary>
        /// Compare database with the definition, and if is inconsistent run dialogs to fix it.;
        /// returns true, if database is corrct or database was updated succeeded
        /// </summary>
        /// <param name="connectionStringWasEdited"></param>
        /// <returns></returns>
        public bool Process(out bool connectionStringWasEdited)
        {
            connectionStringWasEdited = false;

            if (_creatorProperties.IsDbsVariableOk())
            {
                try
                {

                    var sqlConnection = new SqlConnection
                    {
                        ConnectionString = _creatorProperties.GetConnectionStringLoginToMaster()
                    };

                    sqlConnection.Open();
                    sqlConnection.Close();

                    if (CompareDatabases())
                        return true;
                }
                catch (Exception)
                {
                    if (_runAsService)
                        return false;

                    if (!Dialog.ErrorQuestion(
                        _localizationHelper.GetString(
                            "ErrorQuestionSqlServerNotFoundRunDatabaseConfiguration")))
                    {
                        return false;
                    }
                }
            }

            if (_runAsService || !RunSettings())
                return false;

            connectionStringWasEdited = true;
            return true;
        }

        private bool CompareDatabases()
        {
            var databaseCommandExecutor = 
                new DatabaseCommandExecutor(
                    _creatorProperties,
                    _localizationHelper);

            if (!databaseCommandExecutor.CompareVersion())
                return false;

            try
            {
                var compareOrmModel = new CompareOrmModel(databaseCommandExecutor);

                compareOrmModel.OnError += InvokeOnError;
                compareOrmModel.RunCompareAll();

                compareOrmModel.OnError -= InvokeOnError;

                return compareOrmModel.ModelOk;
            }
            catch
            {
                return false;
            }
        }

        void InvokeOnError(Exception inputError)
        {
            if (OnError != null) 
                OnError(inputError);
        }

        /// <summary>
        /// Wizard to configure SQL Server settings, and perform it
        /// </summary>
        /// <returns></returns>
        public bool RunSettings()
        {
            int actionDet = RUN_SERVERCONFIGURE;
            _oldLogin = _connectionString.DatabaseLogin;

            while (actionDet != CONFIGURE_DONE) // all done
            {
                if (actionDet == CANCELED)
                    return false;

                switch (actionDet)
                {
                    case RUN_SERVERCONFIGURE: 
                        actionDet = ServerConfigure(); 
                        break;

                    case RUN_USERCONFIGURE: 
                        actionDet = UserConfigure(); 
                        break;

                    case RUN_DATABASECONFIGURE: 
                        actionDet = DatabaseConfigure(); 
                        break;

                    case RUN_DATABASESETTINGS: 
                        actionDet = DatabaseSettings(); 
                        break;

                    case CREATE_DATABASE: 
                        actionDet = CreateDatabase(); 
                        break;

                    case UPDATE_DATABASE:
                        actionDet = UpdateDatabase(); 
                        break;
                }
            }

            _connectionString.DatabaseLogin = 
                _creatorProperties.DatabaseLogin;

            _connectionString.DatabaseLogPassword = 
                _creatorProperties.DatabaseLogPassword;

            _connectionString.DatabaseName = 
                _creatorProperties.DatabaseName;

            _connectionString.DatabaseServer = 
                _creatorProperties.DatabaseServer;

            //if (_creatorProperties.EnableExternDatabase)
            //{
                _connectionStringExternDatabase.DatabaseLogin = 
                    _creatorProperties.DatabaseLogin;

                _connectionStringExternDatabase.DatabaseLogPassword =
                    _creatorProperties.DatabaseLogPassword;

                _connectionStringExternDatabase.DatabaseName = 
                    _creatorProperties.ExternDatabaseName;

                _connectionStringExternDatabase.DatabaseServer =
                    _creatorProperties.DatabaseServerForExternDatabase;
            //}
            //else
            //    _connectionStringExternDatabase.Clear();

            var databaseCommandExecutor = new DatabaseCommandExecutor(_creatorProperties, _localizationHelper);
            databaseCommandExecutor.SaveServerVersionToDatabase(_serverAssembly);
            return true;
        }

        //1. SqlServer
        /// <summary>
        /// run Form to select SQL server
        /// </summary>
        /// <returns></returns>
        private int ServerConfigure()
        {
            var formServerSelect = 
                new FormServerSelect(
                    _localizationHelper, 
                    _creatorProperties, 
                    System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle)
                {
                    TopMost = true
                };

            formServerSelect.ShowDialog();

            return 
                formServerSelect.WasCanceled 
                    ? CANCELED 
                    : RUN_USERCONFIGURE;
        }

        //2. Server user
        /// <summary>
        /// run Form to select Login user, to connnect to SQL server
        /// </summary>
        /// <returns></returns>
        private int UserConfigure()
        {
            var formServerLogin = 
                new FormServerLogin(
                    _localizationHelper, 
                    _creatorProperties);

            formServerLogin.ShowDialog();

            return 
                formServerLogin.WasCanceled
                    ? CANCELED
                    : (formServerLogin.GoBack 
                        ? RUN_SERVERCONFIGURE 
                        : RUN_DATABASECONFIGURE);
        }

        private bool _mainDatabaseExisted;
        private bool _externDatabaseExisted;

        //3. Database
        /// <summary>
        /// run Form to select database
        /// </summary>
        /// <returns></returns>
        private int DatabaseConfigure()
        {
            var fsd = 
                new FormServerDatabase(
                    _localizationHelper, 
                    _creatorProperties);

            fsd.ShowDialog();

            if (fsd.WasCanceled) 
                return CANCELED;

            if (fsd.GoBack) 
                return RUN_USERCONFIGURE;

            var databaseAccessor = 
                new DatabaseAccessor(
                    _creatorProperties, 
                    _localizationHelper);

            _mainDatabaseExisted = databaseAccessor.MainDatabaseExists;
            _externDatabaseExisted = databaseAccessor.ExternDatabaseExists;

            if (_mainDatabaseExisted && (/*!_creatorProperties.EnableExternDatabase ||*/ _externDatabaseExisted))
            {
                _connectionString.DatabaseLogin = 
                    String.IsNullOrEmpty(_oldLogin) 
                        ? DBS_LOGIN_NAME 
                        : _oldLogin;

                return UPDATE_DATABASE;
            }

            _connectionString.DatabaseLogin = DBS_LOGIN_NAME;

            return
                    !_mainDatabaseExisted && !string.IsNullOrEmpty(_creatorProperties.DatabasePath) ||
                    !_externDatabaseExisted && !string.IsNullOrEmpty(_creatorProperties.ExternDatabasePath)
                ? RUN_DATABASESETTINGS
                : CREATE_DATABASE;
        }

        private int DatabaseSettings()
        {
            var formDatabaseSettings = 
                new FormDatabaseSettings(
                    _localizationHelper, 
                    _creatorProperties, 
                    _mainDatabaseExisted, 
                    _externDatabaseExisted);

            formDatabaseSettings.ShowDialog();

            if (formDatabaseSettings.WasCanceled) 
                return CANCELED;

            return 
                formDatabaseSettings.GoBack
                    ? RUN_DATABASECONFIGURE
                    : CREATE_DATABASE;
        }

        private int CreateDatabase()
        {
            var databaseCreator = 
                new DatabaseCreator(
                    _creatorProperties,
                    _localizationHelper);

            var formBeforeExecution = 
                new FormBeforeExecution(databaseCreator);

            formBeforeExecution.OnError += InvokeOnError;
            //fbefexe.XmlNameSpace = _xmlNameSpace;
            //fbefexe.AsmHbm = _assemblies;

            formBeforeExecution.ShowDialog();

            if (formBeforeExecution.WasCanceled) 
                return CANCELED;

            if (formBeforeExecution.GoBack)
                return !_mainDatabaseExisted && !string.IsNullOrEmpty(_creatorProperties.DatabasePath) ||
                       !_externDatabaseExisted && !string.IsNullOrEmpty(_creatorProperties.ExternDatabasePath)
                    ? RUN_DATABASESETTINGS
                    : RUN_SERVERCONFIGURE;

            return
                !CompareDatabases()
                    ? UPDATE_DATABASE
                    : CONFIGURE_DONE;
        }

        private int UpdateDatabase()
        {
            var databaseUpdater = 
                new DatabaseUpdater(
                    _creatorProperties, 
                    _localizationHelper);

            var formUpdateDatabase = 
                new FormUpdateDatabase(databaseUpdater);

            formUpdateDatabase.OnError += InvokeOnError;

            formUpdateDatabase.ShowDialog();

            switch (databaseUpdater.Result)
            {
                case DatabaseUpdater.ResultEnum.Cancel:
                    return CANCELED;

                case DatabaseUpdater.ResultEnum.GoBack:
                    return RUN_DATABASECONFIGURE;

                default:
                    return CONFIGURE_DONE;
            }
        }

        public static string GetHashedExternalCIID()
        {
            string ciid = SLAClientModule.Singleton.GetExternalCIID();

            SHA256 sha = new SHA256Managed();
            byte[] bytes = sha.ComputeHash(Encoding.ASCII.GetBytes(ciid));

            var builder = new StringBuilder("", 0x20);

            foreach (byte t in bytes)
                builder.Append(t.ToString("x2").ToLower());

            return builder.ToString().Substring(0, 15);
        }
    }
}