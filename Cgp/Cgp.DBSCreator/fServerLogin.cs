using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace Contal.Cgp.DBSCreator
{
    public partial class fServerLogin : Form
    {
        bool _canceled = false;
        bool _goBack = false;
        string _sqlServer;
        string _userName;
        string _password;

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

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                _eName.Text = _userName;
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                _ePassword.Text = _password;
            }
        }
        
        public fServerLogin()
        {
            InitializeComponent();
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

        private bool IsValidUser()
        {
            SqlConnection sqlCon = new SqlConnection();
            sqlCon.ConnectionString = "SERVER = " + _sqlServer + "; DATABASE = master; User ID = "+_userName+"; Pwd = "+_password;
            try
            {
                sqlCon.Open();
                sqlCon.Close();
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return false;
            }
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            _userName = _eName.Text;
            _password = _ePassword.Text;
            if (!IsValidUser())
            {
                MessageBox.Show("User not valid");
                return;
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
    }
}
