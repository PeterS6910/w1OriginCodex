using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Contal.Cgp.DBSCreator
{
    public partial class fAlterTables : Form
    {
        bool _succes = false;
        List<Support.tableErrors> _error;
        System.Exception _lastException;
        string _sqlServer;
        string _databaseName;
        string _userName;
        string _userPassword;

        #region Properties
        public string SqlServer
        {
            set { _sqlServer = value; }
        }
        public string DatabaseName
        {
            set { _databaseName = value; }
        }
        public string UserName
        {
            set { _userName = value; }
        }
        public string UserPassword
        {
            set { _userPassword = value; }
        }

        public bool Succes
        {
            get { return _succes; }
        }

        public List<Support.tableErrors> Error
        {
            get { return _error; }
            set { _error = value; }
        }
        public System.Exception LastException
        {
            get { return _lastException; }
        }
        #endregion

        public fAlterTables()
        {
            InitializeComponent();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShowErrors()
        {
            if (_error == null)
            {
                _rbInfo.Text += "Databáza je zhodná.\n";
                return;
            }
            if (_error.Count == 0)
            {
                _rbInfo.Text += "Databáza je zhodná.\n";
                return;
            }

            _rbInfo.Clear();
            _rbInfo.Text += "Databáza obsahuje nasledujúce chyby.\n";

            for (int i = 0; i < _error.Count; i++)
            {
                _rbInfo.Text += "Tabuľka: "+_error[i]._table+ " stĺpec: "+_error[i]._column +'\n';
            }
            _rbInfo.Text += '\n';
        }

        private bool AlterTable()
        {
            if (_error == null)
            {
                _rbInfo.Text += "Databáza je zhodná.\n";
                return true;
            }
            if (_error.Count == 0)
            {
                _rbInfo.Text += "Databáza je zhodná.\n";
                return true;
            }

            try
            {
                CreateDBS killer;
                killer = new CreateDBS();
                killer.ServerName = _sqlServer;
                killer.DatabaseName = _databaseName;
                killer.User = _userName;
                killer.Password = _userPassword;

                killer.DropColumns(_error);        
                return true;
            }
            catch (Exception ex)
            {
                _lastException = ex;
                return false;
            }            
        }
        
        private void _bListErrors_Click(object sender, EventArgs e)
        {
            ShowErrors();
        }

        private void _bRunAlter_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Upraviť databázu", "Alter DBS", MessageBoxButtons.OKCancel);
            if (DialogResult.Cancel == dr) return;
            
            _bListErrors.Enabled = false;
            if (AlterTable())
            {
                _rbInfo.Text += "Úprava tabuliek ukončená.\n";
                _succes = true;
            }
            else
            {
                _rbInfo.Text += "Úprava tabuliek zlyhala.\n";
                _succes = false;
            }
        }
    }
}
