using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Contal.Cgp.DBSCreator
{
    public partial class fServerSelect : Form
    {
        bool _canceled = true;
        bool _newObt = false;
        private string[] _sqlServers;
        string _selectedServer;
        string _databaseInfo;
        Thread obtainServers;
        bool threadRunning;

        #region Properties
        public string SqlServer
        {
            get { return _selectedServer; }
            set 
            { 
                _selectedServer = value;
                _cbSqlServers.Text = _selectedServer;
            }
        }

        public bool WasCanceled
        {
            get { return _canceled; }
        }

        public string DatabaseInfo
        {
            get { return _databaseInfo; }
            set
            {
                _databaseInfo = value;
                _lServerInfo.Text += " " + _databaseInfo;
                _lMainInfo.Text += " " + _databaseInfo;
            }
        }
        #endregion

        public fServerSelect()
        {
            InitializeComponent();
            _lProgress.Visible = false;
            ExecThread();
        }

        private void Old_ObtainSqlServers()
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
                        _sqlServers[position] = (string) row[col];
                        position++;
                    }
                }
            }

            if (_sqlServers.Length == 0) return;
            foreach (string server in _sqlServers)
            {
                _cbSqlServers.Items.Add(server);
            }
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _selectedServer = string.Empty;
            _canceled = true;
            Close();
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            _selectedServer = _cbSqlServers.Text;
            if (_selectedServer == string.Empty)
            {
                MessageBox.Show("Zvoľte prosím SQL Server!");
                _cbSqlServers.Focus();
                return;
            }
            _canceled = false;            
            Close();
        }

        private void _bObtainDatabases_Click(object sender, EventArgs e)
        {
            RunObtain();
            //Thread obtainServers = new Thread(ObtainSqlServers);
            //obtainServers.IsBackground = true;
            //obtainServers.Start();
        }
        
        private void RunObtain()
        {
            if (threadRunning)
            {
                WhileRunning();
            }
            else
            {
                WhileRunning();
                ExecThread();
            }
        }

        private void ExecThread()
        {
            threadRunning = true;
            obtainServers = new Thread(ObtainSqlServers);
            obtainServers.IsBackground = true;
            obtainServers.Start();
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
                        position++;
                    }
                }
            }
            threadRunning = false;
            _newObt = true;
            AfterThreadDone();
        }
        
        public delegate void DShowRunning();
        private void WhileRunning()
        {
            if (InvokeRequired)
                BeginInvoke(new DShowRunning(WhileRunning));
            else
            {
                this._lProgress.Visible = true;
            }
        }

        private void AfterThreadDone()
        {
            if (InvokeRequired)
                BeginInvoke(new DShowRunning(AfterThreadDone));
            else
            {
                this._lProgress.Visible = false;
                if (_sqlServers.Length == 0) return;
            }
        }

        private void _cbSqlServers_DropDown(object sender, EventArgs e)
        {
            if (threadRunning)
            {
                _lProgress.Visible = true;
                return;
            }
            if (_newObt)
            {               
                string selText = _cbSqlServers.Text;
                _cbSqlServers.Items.Clear();
                foreach (string server in _sqlServers)
                {
                    _cbSqlServers.Items.Add(server);
                }
                _cbSqlServers.Text = selText;
                _newObt = false;                
            }
            
        }


    }
}