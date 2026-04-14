using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.SLA.Client;

namespace AnalyticTool.Advitech
{
    public partial class DatabaseServerSettingsForm : Form, IDialog
    {
        private Form _resultDialog;
        private const string DBS_LOGIN_NAME = "Advitech";

        public DatabaseServerSettingsForm()
        {
            InitializeComponent();

            LoadSettings();
        }

        private void LoadSettings()
        {
            _cbSqlServers.Text = ApplicationProperties.Singleton.SqlServerName;
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            _resultDialog = new ServerLoginSettingsForm();
            DialogResult = DialogResult.Cancel;
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            SetValues();

            if (!DatabaseAccessor.Singleton.IsValidMasterConnectionString())
            {
                Dialog.Error("Login or password is wrong");
                return;
            }

            if (_rbUseCGPLogin.Checked)
            {
                ApplicationProperties.Singleton.DatabaseLogin = DBS_LOGIN_NAME + "_" + GetHashedExternalCIID();
                DatabaseAccessor.Singleton.PrepareCustomLogin(DatabaseAccessor.Singleton.UserExists(), true);
            }
            else if (_rbUseCustomLogin.Checked)
            {
                var insertLoginAndPasswordForm = new InsertLoginAndPasswordForm();

                if (insertLoginAndPasswordForm.ShowDialog() == DialogResult.OK)
                {
                    ApplicationProperties.Singleton.DatabaseLogin = insertLoginAndPasswordForm.CustomUserName;
                    ApplicationProperties.Singleton.DatabasePassword = insertLoginAndPasswordForm.CustomUserPassword;

                    if (!DatabaseAccessor.Singleton.IsValidConnectionString())
                    {
                        Dialog.Error("Login or password is wrong");
                        return;
                    }
                }
            }

            _resultDialog = new SelectDatabaseAndTableForm();
            DialogResult = DialogResult.OK;
        }

        private void SetValues()
        {
            ApplicationProperties.Singleton.SqlServerName = _cbSqlServers.Text;
            ApplicationProperties.Singleton.SuperUserName = _tbLoginName.Text;
            ApplicationProperties.Singleton.SuperUserPassword = _tbDbPassword.Text;
        }

        private bool _threadRunning;

        private void StartObtainingSqlServers(bool dropDown)
        {
            if (!_threadRunning)
            {
                foreach (Control control in Controls)
                    control.Enabled = false;

                Cursor = Cursors.WaitCursor;
                _threadRunning = true;
                SafeThread.StartThread(ObtainSqlServers);

                if (dropDown)
                    _cbSqlServers.DroppedDown = true;
            }
        }

        private void ObtainSqlServers()
        {
            System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
            var dataTable = instance.GetDataSources();

            var sqlServers = new string[dataTable.Rows.Count];
            int position = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (col.ColumnName == "ServerName")
                    {
                        sqlServers[position] = (string)row[col];

                    }

                    if (col.ColumnName == "InstanceName")
                    {
                        if (!(row[col] is DBNull))
                            sqlServers[position] += "\\" + (string)row[col];
                    }
                }
                position++;
            }

            _cbSqlServers.BeginInvoke(
                new Action(() =>
                {
                    _cbSqlServers.DataSource = sqlServers;

                    foreach (Control control in Controls)
                        control.Enabled = true;

                    Cursor = Cursors.Default;
                }));

            _threadRunning = false;
        }

        public string GetHashedExternalCIID()
        {
            string ciid = SLAClientModule.Singleton.GetExternalCIID();

            SHA256 sha = new SHA256Managed();
            byte[] bytes = sha.ComputeHash(Encoding.ASCII.GetBytes(ciid));

            var builder = new StringBuilder("", 0x20);

            foreach (byte t in bytes)
                builder.Append(t.ToString("x2").ToLower());

            return builder.ToString().Substring(0, 15);
        }

        private void _bObtainDatabases_Click(object sender, EventArgs e)
        {
            StartObtainingSqlServers(true);
        }

        #region IDialog Members

        public Form ResultDialog
        {
            get { return _resultDialog; }
        }

        #endregion
    }
}
