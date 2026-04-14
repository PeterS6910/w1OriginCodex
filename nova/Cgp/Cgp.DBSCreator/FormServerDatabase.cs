using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.DBSCreator
{
    public partial class FormServerDatabase : TranslateForm
    {
        private bool _canceled;
        private bool _goBack;

        private List<string> _databasesList;
        private List<string> _externDatabasesList;
        private readonly CreatorProperties _creatorProperties;

        private Thread _obtainServersThread;
        private Thread _obtainServersExternDatabaseThread;

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

        public FormServerDatabase(LocalizationHelper localizationHelper, CreatorProperties creatorProperties)
            : base(localizationHelper)
        {
            InitializeComponent();
            _creatorProperties = creatorProperties;
            _eSqlServer.Text = _creatorProperties.DatabaseServer;
            _cbDatabaseList.Text = _creatorProperties.DatabaseName;
            _cbExternDatabaseName.Text = _creatorProperties.ExternDatabaseName;
            SetDatabasesExternDatabase();
            SetDatabasePath();
            SetDatabases();
        }

        public void SetDatabases()
        {
            ExecThread();
        }

        public void SetDatabasesExternDatabase()
        {
            ExecThreadExternDatabase();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _canceled = true;
            _goBack = false;
            Close();
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            if (_cbDatabaseList.Text == string.Empty)
            {
                Dialog.Error(GetString("FormServerDatabaseEnterDatabaseName"));
                _cbDatabaseList.Focus();
                return;
            }

            if (_cbExternDatabaseName.Text == string.Empty)
            {
                Dialog.Error(GetString("FormServerDatabaseEnterExternalDatabaseName"));
                _cbExternDatabaseName.Focus();
                return;
            }

            if (_cbExternDatabaseName.Text == _cbDatabaseList.Text)
            {
                Dialog.Error(GetString("ErrorExternDbsNameEqualToMainDbs"));
                _cbExternDatabaseName.Focus();
                return;
            }

            _creatorProperties.DatabaseName = _cbDatabaseList.Text;
            _creatorProperties.ExternDatabaseName = _cbExternDatabaseName.Text;
            _canceled = false;
            _goBack = false;
            Close();
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            _canceled = false;
            _goBack = true;

            Close();
        }

        private static string GetDatabasePath(
            string databaseServer, 
            string connectionString, 
            string thisHostName)
        {
            string hostName = databaseServer;

            int pos = hostName.IndexOf('\\');

            if (pos > 0)
                hostName = hostName.Substring(0, pos);

            if (hostName.ToUpper() != thisHostName)
                return string.Empty;

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                var myCommand =
                    new SqlCommand(
                        "select physical_name from sys.database_files",
                        sqlConnection);

                SqlDataReader myReader = myCommand.ExecuteReader();

                if (!myReader.Read())
                    return string.Empty;

                string result = myReader.GetString(0);

                return
                    result.Substring(
                        0,
                        result.LastIndexOf('\\') + 1);
            }
        }

        private void SetDatabasePath()
        {
            _creatorProperties.DatabasePath = string.Empty;
            _creatorProperties.ExternDatabasePath = string.Empty;

            try
            {
                string thisHostName = 
                    System.Net.Dns.GetHostName().ToUpper();

                string databasePath = 
                    GetDatabasePath(
                        _creatorProperties.DatabaseServer,
                        _creatorProperties.GetConnectionStringPrincipalToMaster(),
                        thisHostName);

                _creatorProperties.DatabasePath = databasePath;

                databasePath = 
                    GetDatabasePath(
                        _creatorProperties.DatabaseServerForExternDatabase,
                        _creatorProperties.GetConnectionStringPrincipalToMasterExternDatabase(),
                        thisHostName);

                _creatorProperties.ExternDatabasePath = databasePath;
            }
            catch
            {
                _creatorProperties.DatabasePath = Application.StartupPath + @"\Data";
                _creatorProperties.ExternDatabasePath = Application.StartupPath + @"\Data";
            }
        }

        #region ThreadObtainDatabase

        private void ExecThread()
        {
            _obtainServersThread = 
                new Thread(ObtainDatabasesThread)
                {
                    IsBackground = true
                };

            _obtainServersThread.Start();
        }

        private void ExecThreadExternDatabase()
        {
            _obtainServersExternDatabaseThread =
                new Thread(ObtainDatabasesThreadExternDatabase)
                {
                    IsBackground = true
                };

            _obtainServersExternDatabaseThread.Start();
        }

        private void ObtainDatabasesThread()
        {
            using (var sqlConnection =
                    new SqlConnection(_creatorProperties.GetConnectionStringPrincipalToMaster()))
                try
                {
                    sqlConnection.Open();

                    var myCommand = new SqlCommand("sp_databases", sqlConnection);

                    SqlDataReader myReader = myCommand.ExecuteReader();

                    _databasesList = new List<string>();

                    while (myReader.Read())
                        _databasesList.Add(myReader.GetString(0));

                }
                catch (Exception)
                {
                }

            AfterThreadDone();
        }

        private void ObtainDatabasesThreadExternDatabase()
        {
            using (var sqlConnection = 
                    new SqlConnection(_creatorProperties.GetConnectionStringPrincipalToMasterExternDatabase()))
                try
                {
                    sqlConnection.Open();

                    var myCommand = new SqlCommand("sp_databases", sqlConnection);

                    SqlDataReader myReader = myCommand.ExecuteReader();

                    _externDatabasesList = new List<string>();

                    while (myReader.Read())
                        _externDatabasesList.Add(myReader.GetString(0));
                }
                catch (Exception)
                {
                }

            AfterThreadDoneExternDatabase();
        }

        public delegate void DShowRunning();

        private void AfterThreadDone()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DShowRunning(AfterThreadDone));
                return;
            }

            _lProgress.Visible = false;

            if (_databasesList == null)
                return;

            foreach (string str in _databasesList)
                _cbDatabaseList.Items.Add(str);
        }

        private void AfterThreadDoneExternDatabase()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DShowRunning(AfterThreadDoneExternDatabase));
                return;
            }

            _lProgress1.Visible = false;

            if (_externDatabasesList == null) 
                return;

            foreach (string str in _externDatabasesList)
                _cbExternDatabaseName.Items.Add(str);
        }

        #endregion
    }
}
