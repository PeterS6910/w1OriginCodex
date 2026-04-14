using System;
using System.Data.SqlClient;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.DBSCreator
{
    public partial class FormServerLogin : TranslateForm
    {
        private const string DBS_LOGIN_NAME = "cgpLogin";

        bool _canceled;
        bool _goBack;
        private readonly CreatorProperties _creatorProperties;

        #region Properties
        public bool WasCanceled
        {
            get { return _canceled; }
        }
        public bool GoBack
        {
            get { return _goBack; }
        }
        #endregion


        public FormServerLogin(LocalizationHelper localizationHelper, CreatorProperties creatorProperties)
            : base(localizationHelper)
        {
            InitializeComponent();
            _creatorProperties = creatorProperties;
            _eSqlServer.Text = _creatorProperties.DatabaseServer;
            _eName.Text = _creatorProperties.DatabasePrincipalUser;
            _ePassword.Text = _creatorProperties.DatabasePrincipalUserPassword;
        }


        private bool IsValidUser()
        {
            try
            {
                return 
                    DatabaseCreator.IsValidConnectionString(
                        _creatorProperties.GetConnectionStringPrincipalToMaster());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Control sql server version. Sql server version must be Sql Server 2008 or later.
        /// </summary>
        /// <returns></returns>
        private bool IsValidSQLServerVersion()
        {
            try
            {
                string connectionString = _creatorProperties.GetConnectionStringPrincipalToMaster();

                SqlConnection sqlConnection = new SqlConnection();
                try
                {
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    string sqlQuery = "select serverproperty('ProductVersion')";
                    SqlCommand myCommand = new SqlCommand(sqlQuery, sqlConnection);
                    string strSQLServerProductVersion = myCommand.ExecuteScalar() as string;

                    if (strSQLServerProductVersion != null && strSQLServerProductVersion != string.Empty)
                    {
                        string[] arraySQLServerProductVersions = strSQLServerProductVersion.Split('.');

                        
                        if (arraySQLServerProductVersions != null && arraySQLServerProductVersions.Length > 0)
                        {
                            int sqlServerProductVersion;
                            if (int.TryParse(arraySQLServerProductVersions[0], out sqlServerProductVersion))
                            {
                                return sqlServerProductVersion >= 10;
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    sqlConnection.Close();
                }
            }
            catch { }

            return false;
        }

        private bool IsValidUserExternDatabase()
        {
            try
            {
                return 
                    DatabaseCreator.IsValidConnectionString(
                        _creatorProperties
                            .GetConnectionStringPrincipalToMasterExternDatabase());
            }
            catch
            {
                return false;
            }
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            if (_eName.Text == string.Empty)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorEntryLogin"), ControlNotificationSettings.Default);
                _eName.Focus();
                return;
            }

            if (_ePassword.Text == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _ePassword,
                    GetString("ErrorEntryPassword"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _ePassword.Focus();
                return;
            }

            _creatorProperties.DatabasePrincipalUser = _eName.Text;
            _creatorProperties.DatabasePrincipalUserPassword = _ePassword.Text;

            //if (_creatorProperties.EnableExternDatabase)
            //{
                _creatorProperties.DatabasePrincipalUserExternDatabase = _eName.Text;
                _creatorProperties.DatabasePrincipalUserPasswordExternDatabase = _ePassword.Text;
            //}

            if (_rbUseCGPLogin.Checked)
            {
                _creatorProperties.LoginType = LoginType.UseCgpLogin;
                _creatorProperties.DatabaseLogin = DBS_LOGIN_NAME + "_" + Manager.GetHashedExternalCIID();
            }
            else if (_rbUseCustomLogin.Checked)
            {
                _creatorProperties.LoginType = LoginType.UseCustomLogin;
                _creatorProperties.DatabaseLogin = GetString("TextNotSetLoginName");
            }
            else if (_rbUseSaLogin.Checked)
            {
                _creatorProperties.LoginType = LoginType.UseSaLogin;
                _creatorProperties.DatabaseLogin = _eName.Text;
                _creatorProperties.DatabaseLogPassword = _ePassword.Text;
            }

            if (!IsValidUser())
            {
                Dialog.Error(GetString("FormServerLoginUserNotValid"));
                return;
            }

            if (!IsValidSQLServerVersion())
            {
                Dialog.Error(GetString("FormServerLoginSQLServerVersionNotValid"));
                return;
            }

            if (_creatorProperties.DatabaseServer != _creatorProperties.DatabaseServerForExternDatabase)
            {
                if (!IsValidUserExternDatabase())
                {
                    Dialog.Error(GetString("FormServerExternalDatabaseLoginUserNotValid"));
                    return;
                }
            }

            _canceled = false;
            _goBack = false;
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _canceled = true;
            _goBack = false;
            Close();
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            _canceled = false;
            _goBack = true;
            Close();
        }

        private void FormServerLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
