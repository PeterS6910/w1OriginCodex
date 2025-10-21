using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    public partial class LocalizationDialog : Form
    {
        private string _language;
        public string Language { get { return _language; } }

        private bool _isMasterResource;
        public bool IsMasterResource { get { return _isMasterResource; } }

        public LocalizationDialog()
        {
            InitializeComponent();
        }

        private void _btOk_Click(object sender, EventArgs e)
        {
            if (_cbLanguage.Text.ToString().Equals(string.Empty))
            {
                _cbLanguage.BackColor = Color.Red;
            }
            else
            {
                _language = _cbLanguage.Text.ToString();
                _isMasterResource = _chbMasterResource.Checked;
                DialogResult = DialogResult.OK;

            }
        }

        private void _cbLanguage_TextChanged(object sender, EventArgs e)
        {
            _cbLanguage.BackColor = Color.White;
        }

        private void _btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
