using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Contal.Cgp.DBSCreator
{
    public partial class fDatabaseSettings : Form
    {
        bool _canceled = false;
        bool _goBack = false;

        string _sqlServer;
        string _databaseName;
        string _databasePath;

        #region Properties
        public string SqlServer
        {
            get { return _sqlServer; }
            set
            {
                _sqlServer = value;
                _eSqlServer.Text = _sqlServer;
            }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                _databaseName = value;
                _eDatabase.Text = _databaseName;
            }
        }

        public string Path
        {
            get { return _databasePath; }
            set
            {
                _databasePath = value;
                if (_databasePath != string.Empty)
                    _eDatabasePath.Text = _databasePath;
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

        public fDatabaseSettings()
        {
            InitializeComponent();
            SetDatabasePath();
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            _canceled = false;
            _goBack = true;
            Close();
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            _databasePath = _eDatabasePath.Text;
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

        private void SetDatabasePath()
        {
            string strPath;
            strPath = @"SOFTWARE\Microsoft\Microsoft SQL Server\MSSQL.1\Setup";
            RegistryKey MyReg = Registry.LocalMachine.OpenSubKey(strPath);
            _databasePath = (string)(MyReg.GetValue("SQLPath"));   
            _databasePath += @"\Data";
            _eDatabasePath.Text = _databasePath;
        }
    }
}
