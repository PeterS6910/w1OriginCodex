using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Contal.Cgp.DBSCreator
{
    public partial class fUserSettings : Form
    {
        string _login;
        bool _canceled;
        bool _goBack;

        #region Properties
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                _eLogin.Text = _login;
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

        public fUserSettings()
        {
            InitializeComponent();
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

        private void _bNext_Click(object sender, EventArgs e)
        {
            _login = _eLogin.Text;
            if (_login == string.Empty)
            {
                MessageBox.Show("Prosím zadajte prihlasovacie meno.");
                return;
            }
            _canceled = false;
            _goBack = false;
            Close();
        }
    }
}