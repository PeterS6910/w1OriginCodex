using System;
using System.Windows.Forms;

using Contal.IwQuick.Localization;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace Contal.Cgp.DBSCreator
{
    public partial class FormServerSelect : TranslateForm
    {
        bool _canceled = true;
        bool _newSqlServersFound;
        private string[] _sqlServers;
        bool _threadRunning;

        private readonly CreatorProperties _creatorProperties;
        
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            TopMost = true;
        }

        #region Properties
        public bool WasCanceled
        {
            get { return _canceled; }
        }
        #endregion

        public FormServerSelect(LocalizationHelper localizationHelper, CreatorProperties mDbsCS, IntPtr hwnd)
            : base(localizationHelper)
        {
            InitializeComponent();

            try
            {
                    Screen screen = Screen.FromHandle(hwnd);
                    StartPosition = FormStartPosition.Manual;
                    Left = (screen.WorkingArea.Width - Width) / 2 + screen.WorkingArea.Left;
                    Top = (screen.WorkingArea.Height - Height) / 2 + screen.WorkingArea.Top;
            }
            catch { }

            ShowLanguages();
            _creatorProperties = mDbsCS;
            _cbSqlServers.Text = _creatorProperties.DatabaseServer;
            //_chbEnableExternalDatabase.Checked = _creatorProperties.EnableExternDatabase;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _canceled = true;
            Close();
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            if (_cbSqlServers.Text == string.Empty)
            {
                Dialog.Error(GetString("FormServerSelectSelectSQLServer"));
                _cbSqlServers.Focus();
                return;
            }

            //if (_chbEnableExternalDatabase.Checked)
            //{
            //    _creatorProperties.DatabaseServerForExternDatabase = _cbSqlServers.Text;
            //}

            _creatorProperties.DatabaseServerForExternDatabase = _cbSqlServers.Text;
            _creatorProperties.DatabaseServer = _cbSqlServers.Text;
            //_creatorProperties.EnableExternDatabase = _chbEnableExternalDatabase.Checked;
            _canceled = false;            
            Close();
        }

        private void _bObtainDatabases_Click(object sender, EventArgs e)
        {
            StartObtainingSqlServers(true);
        }
        
        private void StartObtainingSqlServers(bool dropDown)
        {
            if (!_threadRunning)
            {
                _threadRunning = true;
                SafeThread.StartThread(ObtainSqlServers);

                if (dropDown)
                    _cbSqlServers.DroppedDown = true;
            }
        }

        private void ObtainSqlServers()
        {
            System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
            System.Data.DataTable dataTable = instance.GetDataSources();

            _sqlServers = new string[dataTable.Rows.Count];
            int position = 0;

            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                foreach (System.Data.DataColumn col in dataTable.Columns)
                {
                    if (col.ColumnName == "ServerName")
                    {
                        _sqlServers[position] = (string)row[col];
                        
                    }

                    if (col.ColumnName == "InstanceName")
                    {
                        if (!(row[col] is DBNull))
                            _sqlServers[position] += "\\" + (string)row[col];
                    }
                }
                position++;
            }
            _threadRunning = false;
            _newSqlServersFound = true;
            AfterThreadDone();
        }
       

        private void AfterThreadDone()
        {
            if (InvokeRequired)
                Invoke(new DVoid2Void(AfterThreadDone));
            else
            {
                _cbSqlServers_DropDown(this, null);
            }
        }

        private void _cbSqlServers_DropDown(object sender, EventArgs e)
        {
            if (_threadRunning)
            {
                _cbSqlServers.Items.Clear();
                _cbSqlServers.Items.Add(LocalizationHelper.GetString("searchingForSqlServers"));
            }
            else
            {
                if (_newSqlServersFound)
                {
                    string selText = _cbSqlServers.Text;
                    _cbSqlServers.Items.Clear();

                    foreach (string server in _sqlServers)
                    {
                        _cbSqlServers.Items.Add(server);
                    }

                    _cbSqlServers.Text = selText;
                    _newSqlServersFound = false;

                    if (_cbSqlServers.DroppedDown)
                    {
                        _cbSqlServers.DroppedDown = false;
                        _cbSqlServers.DroppedDown = true;
                    }
                }
            }
        }

        private void FormServerSelect_Shown(object sender, EventArgs e)
        {
            if (Validator.IsNullString(_cbSqlServers.Text))
                StartObtainingSqlServers(true);
        }

        private void _cbSqlServers_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_threadRunning)
            {
                _cbSqlServers.SelectedIndex = -1;
            }
        }
    }
}