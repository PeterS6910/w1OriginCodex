using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Contal.Cgp.DBSCreator
{
    public partial class fBeforeExecution : Form
    {
        string _sqlServer = string.Empty;
        string _userName = string.Empty;
        string _password = string.Empty;
        string _login = string.Empty;
        string _database = string.Empty;
        string _databasePath = string.Empty;
        string _xmlNameSpace = string.Empty;
        bool _canceled = false;
        bool _goBack = false;
        bool _doneOk = false;
        IEnumerable<Assembly> _asmHbm;

        #region Properties
        public IEnumerable<Assembly> AsmHbm
        {
            get { return _asmHbm; }
            set { _asmHbm = value; }
        }
        public bool WasCanceled
        {
            get { return _canceled; }
        }
        public bool GoBack
        {
            get { return _goBack; }
        }
        public string SqlServer
        {
            set 
            { 
                _sqlServer = value;
                _eSqlServer.Text = _sqlServer;
            }
        }
        public string UserName
        {
            set { _userName = value; }
        }
        public string Password
        {
            set { _password = value; }
        }
        public string Login
        {
            set 
            { 
                _login = value;
                _eUser.Text = _login;
            }
        }
        public string DatabasePath
        {
            set { _databasePath = value; }
        }
        public string Database
        {
            set
            {
                _database = value;
                _eDatabase.Text = _database;
            }
        }
        public string XmlNameSpace
        {
            set { _xmlNameSpace = value; }
        }     
        #endregion

        public void ShowUndo()
        {
            _bUndoDatabase.Visible = true;
            _bUndoUser.Visible = true;
        }

        public fBeforeExecution()
        {
            InitializeComponent();
            _bUndoDatabase.Visible = false;
            _bUndoUser.Visible = false;
            _cbAlterLogin.Visible = false;
        }

        private void _bPerform_Click(object sender, EventArgs e)
        {
            _canceled = false;
            _goBack = false;

            if (_doneOk)
            {
                Close();
                return;
            }

            
            if (CreateDatabase())
            {
                _doneOk = true;
                _bBack.Enabled = false;
                _bCancel.Enabled = false;
                _bPerform.Text = "Koniec";
            }
            else
            {
                MessageBox.Show("Vytvorenie dbs skončilo neúspešne.");
            }
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            _canceled = false;
            _goBack = true;
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _canceled = true;
            _goBack = false;
            Close();
        }

        //private void 

        public bool CreateDatabase()
        {
            if (!IsAllOkToCreateDBS())
            {
                _rbProgress.Text += "Nie je možné vytvoriť databázu.\n";
                return false;
            }

            CreateDBS cDbs = new CreateDBS();
            cDbs.ServerName = _sqlServer;
            cDbs.Login = _login;
            cDbs.User = _userName;
            cDbs.Password = _password;
            cDbs.DatabaseName = _database;
            cDbs.DatabasePath = _databasePath;

            if (!cDbs.CreateLoginDbs())
            {
                if (cDbs.DbsCreated)
                {
                    _bUndoDatabase.Visible = true;
                }
                else
                {
                    _rbProgress.Text += "Chyba pri vytvarani databázy.\n";
                }

                if (cDbs.UserCreated)
                {
                    _bUndoUser.Visible = true;
                }
                else
                {
                    _rbProgress.Text += "Chyba pri vytvarani užívateľa.\n";
                }
                _rbProgress.Text += cDbs.Error + '\n';
                return false;
            }
            else
            {
                _rbProgress.Text += "Databáza vytvorená.\n";
                _rbProgress.Text += "Užívateľ vytvorený.\n";
            }
            
            CreateSchema cSchema = new CreateSchema();
            cSchema.CatalogName = _database;
            cSchema.ServerName = _sqlServer;
            cSchema.AsmHbm = _asmHbm;
            //cSchema.TablesNameSpace = _xmlNameSpace;
            if (cSchema.CreateTables())
            {
                _rbProgress.Text += "Tabuľky vytvorené.\n";
            }
            else
            {
                _rbProgress.Text += "Chyba pri vytváraní tabuliek.\n";
                _bUndoDatabase.Visible = true;
                _bUndoUser.Visible = true;
                return false;
            }
            
            return true;
        }

        private void _bUndoDatabase_Click(object sender, EventArgs e)
        {
            CreateDBS dropDbs = new CreateDBS();
            dropDbs.ServerName = _sqlServer;
            dropDbs.Login = _login;
            dropDbs.User = _userName;
            dropDbs.Password = _password;
            dropDbs.DatabaseName = _database;
            if (dropDbs.DropDatabase())
            {
                MessageBox.Show("Databáza bola zrušená");
            }
            else
            {
                MessageBox.Show("Zrušenie databázy zlyhalo.");
            }
        }

        private void _bUndoUser_Click(object sender, EventArgs e)
        {
            CreateDBS dropDbs = new CreateDBS();
            dropDbs.ServerName = _sqlServer;
            dropDbs.Login = _login;
            dropDbs.User = _userName;
            dropDbs.Password = _password;
            dropDbs.DatabaseName = _database;
            if (dropDbs.DropLogin())
            {
                MessageBox.Show("Užívateľ bol zrušený.");
            }
            else
            {
                MessageBox.Show("Zrušenie užívateľa zlyhalo.");
            }
        }

        private bool IsAllOkToCreateDBS()
        {
            CreateDBS conDBS = new CreateDBS();
            conDBS.ServerName = _sqlServer;
            conDBS.Login = _login;
            conDBS.User = _userName;
            conDBS.Password = _password;
            conDBS.DatabaseName = _database;
            conDBS.DatabasePath = _databasePath;
            
            if (conDBS.DatabaseExist())
            {
                _rbProgress.Text += "Databáza už existuje.\n";
                return false;
            }
            if (conDBS.UserExist())
            {
                _rbProgress.Text += "Užívateľ už existuje.\n";
                _cbAlterLogin.Visible = true;
                if (_cbAlterLogin.Checked)
                    return true;
                else
                    return false;
            }
            return true;
        }
    }
}