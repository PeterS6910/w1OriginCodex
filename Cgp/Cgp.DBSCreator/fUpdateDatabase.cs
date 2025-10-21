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
    public partial class fUpdateDatabase : Form
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
        bool _doneExit = false;
        List<Support.tableErrors> _error;
        List<Support.tableErrors> _alterDbs;
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
            set
            {
                _userName = value;
                _eUser.Text = _userName;
            }
        }
        public string Password
        {
            set { _password = value; }
        }
        public string Login
        {
            set { _login = value; }
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

        public fUpdateDatabase()
        {
            InitializeComponent();
            _bFixDbs.Visible = false;
        }

        private bool UpdateDatabase()
        {
            CreateSchema cSchema = new CreateSchema();
            cSchema.CatalogName = _database;
            cSchema.ServerName = _sqlServer;
            cSchema.AsmHbm = _asmHbm;
            //cSchema.TablesNameSpace = _xmlNameSpace;
            if (!cSchema.UpdateTables()) return false;
            return true;
        }

        private bool CompareOrmSql()
        {
            bool ok = true;
            CompareOrmModel comp = new CompareOrmModel();
            comp.DatabaseName = _database;
            comp.Password = _password;
            comp.UserName = _userName;
            comp.SqlServer = _sqlServer;
            comp.AsmAll = this._asmHbm;
            comp.RunCompareAlls();
            
            _error = comp.WrongColumnsSet;
            if (_error.Count != 0)
            {
                _rbProgress.Text += "Wrong columns:\n";
                ok = false;
                for (int i = 0; i < _error.Count; i++)
                {
                    _rbProgress.Text += _error[i]._table +" " +  _error[i]._column +'\n';
                }
                _rbProgress.Text += "\n";
            }
            _error = comp.ColumnsNotInOrm;
            _alterDbs = comp.ColumnsNotInOrm;
            if (_error.Count != 0)
            {
                _rbProgress.Text += "Orm miss columns:\n";
                ok = false;
                for (int i = 0; i < _error.Count; i++)
                {
                    _rbProgress.Text += _error[i]._table + " " + _error[i]._column + '\n';
                }
                _rbProgress.Text += "\n";
                _bFixDbs.Visible = true;
            }
            _error = comp.ColumnsNotInSql;
            
            if (_error.Count != 0)
            {
                _rbProgress.Text += "SQL miss columns:\n";
                ok = false;
                for (int i = 0; i < _error.Count; i++)
                {
                    _rbProgress.Text += _error[i]._table + " " + _error[i]._column + '\n';
                }
                _rbProgress.Text += "\n";
            }


            return ok;
        }

        private void FinishOk()
        {
        }

        private void Fail()
        {
        }

        private void _bPerform_Click(object sender, EventArgs e)
        {
            _canceled = false;
            _goBack = false;

            if (_doneExit == true)
            {
                Close();
                return;
            }

            UpdateDatabase();
            if (CompareOrmSql())
            {
                _rbProgress.Text += "Databáza bola úspešne aktualizovaná.\n";
                _doneExit = true;
                _bPerform.Text = "Zavri";
            }
            else
            {
                _rbProgress.Text += "Pri aktualizácií sa vyskytli chyby:\n";
                ListErrors();
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

        private void ListErrors()
        {
            //for (int i = 0; i < orm.Count; i++)
            //{
            //    _rbProgress.Text += orm[i] + " ORM modul neobsahuje not null stĺpec z tabluľky.\n";
            //}
        }

        private void _bFixDbs_Click(object sender, EventArgs e)
        {
            fAlterTables alter = new fAlterTables();
            alter.DatabaseName = _database;
            alter.SqlServer = _sqlServer;
            alter.UserName = _userName;
            alter.UserPassword = _password;
            alter.Error = _alterDbs;
            alter.ShowDialog();
        }
    }
}
