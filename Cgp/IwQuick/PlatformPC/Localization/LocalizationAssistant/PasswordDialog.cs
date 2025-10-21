using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Contal.IwQuick.Crypto;

namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    public partial class PasswordDialog : Form
    {
        public const string _accessCode = "e1ed22f04281e0991e92839c08be640767c816e633629a8dd20f695bdbcca1cd2d5671130c8f57a780a4b0868889154a89fef3a4ad3a1574f474bfdef5903c89";
        private string password;
        public string Password { get { return password; } }

        public PasswordDialog()
        {
            InitializeComponent();
            password = string.Empty;
        }

        private void _btOk_Click(object sender, EventArgs e)
        {
            if (QuickHashes.GetSHA512String(_edPasswd.Text.ToString()).Equals(_accessCode))
            {
                password = _edPasswd.Text.ToString();
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Abort;
            }
        }

        private void _btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void PasswordDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (QuickHashes.GetSHA512String(_edPasswd.Text.ToString()).Equals(_accessCode))
                {
                    password = _edPasswd.Text.ToString();
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    DialogResult = DialogResult.Abort;
                }
            }
        }
   }
}
