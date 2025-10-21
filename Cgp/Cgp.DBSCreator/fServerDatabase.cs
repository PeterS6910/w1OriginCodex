using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace Contal.Cgp.DBSCreator
{
    public partial class fServerDatabase : Form
    {
        bool _canceled = false;
        bool _goBack = false;
        string _sqlServer;
        List<string> _databasesList;
        string _databaseSel;

        string _userName;
        string _password;
        bool _newDbs;

        Thread obtainServers;
        bool threadRunning;

        #region Properties
        public bool NewDbs
        {
            get { return _newDbs; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string SqlServer
        {
            get { return _sqlServer; }
            set 
            { 
                _sqlServer = value;
                _eSqlServer.Text = _sqlServer;
            }               
        }

        public string DatabaseSel
        {
            get { return _databaseSel; }
            set
            {
                _databaseSel = value;
                _cbDatabaseList.Text = _databaseSel;
            }
        }

        public bool WasCanceled
        {
            get { return _canceled; }
        }

        public bool GoBack
        {
            get { return _goBack; }
        }
        #endregion

        public fServerDatabase()
        {
            InitializeComponent();
        }

        public void SetDatabases()
        {
            //ObtainDatabases();
            //NaplnChebox();
            ExecThread();
        }

        private void ObtainDatabases()
        {
            //sp_databases
            try
            {
                SqlConnection sqlCon = new SqlConnection();
                sqlCon.ConnectionString = "SERVER = " + _sqlServer + "; DATABASE = master; User ID = " + _userName + "; Pwd = " + _password;
                sqlCon.Open();
                string sqlQuery;
                sqlQuery = "sp_databases";
                SqlCommand myCommand = new SqlCommand(sqlQuery, sqlCon);
                SqlDataReader myReader = myCommand.ExecuteReader();
                _databasesList = new List<string>();
                while (myReader.Read())
                {
                    _databasesList.Add(myReader.GetString(0));
                }
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private void NaplnChebox()
        {
            if (_databasesList == null) return;
            foreach (string str in _databasesList)
            {
                _cbDatabaseList.Items.Add(str);
            }
        }

        private bool DatabaseExist()
        {
            if (_databasesList == null || _databasesList.Count == 0) return false;
            foreach (string dbs in _databasesList)
            {
                if (dbs == _databaseSel) return true;
            }
            return false;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _canceled = true;
            _goBack = false;
            Close();
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            _databaseSel = _cbDatabaseList.Text;
            _newDbs = !DatabaseExist();
            if (_databaseSel == string.Empty)
            {
                MessageBox.Show("Zadajte databazu!");
                _cbDatabaseList.Focus();
                return;
            }
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

        #region ThreadObtainDatabase

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
            obtainServers = new Thread(ObtainDatabasesThread);
            obtainServers.IsBackground = true;
            obtainServers.Start();
        }

        private void ObtainDatabasesThread()
        {
            try
            {
                SqlConnection sqlCon = new SqlConnection();
                sqlCon.ConnectionString = "SERVER = " + _sqlServer + "; DATABASE = master; User ID = " + _userName + "; Pwd = " + _password;
                sqlCon.Open();
                string sqlQuery;
                sqlQuery = "sp_databases";
                SqlCommand myCommand = new SqlCommand(sqlQuery, sqlCon);
                SqlDataReader myReader = myCommand.ExecuteReader();
                _databasesList = new List<string>();
                while (myReader.Read())
                {
                    _databasesList.Add(myReader.GetString(0));
                }
                sqlCon.Close();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            threadRunning = false;
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
                if (_databasesList == null) return;
                foreach (string str in _databasesList)
                {
                    _cbDatabaseList.Items.Add(str);
                }
            }
        }
        #endregion
    }
}
