using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Contal.IwQuick.Localization;
using System.Collections;
using Contal.IwQuick.UI;

namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    public partial class ChooseLanguage : Form
    {
        private string _language;
        public string Language { get { return _language; } }

        private List<string> _languages;
     
        public ChooseLanguage(List<string> languages)
        {
            InitializeComponent();
            _languages = languages;
            _language = string.Empty;
        }

        private void frm_ChooseCustomer_Load(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < _languages.Count; i++)
                {
                    int newRow = _dgwLanguage.Rows.Add();
                    _dgwLanguage.Rows[newRow].Cells[0].Value = _languages[i];
                }
            }
            catch(Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private void _dgwLanguage_SelectionChanged(object sender, EventArgs e)
        {
            if (_dgwLanguage.SelectedCells.Count > 0)
            {
                foreach (DataGridViewCell cell in _dgwLanguage.SelectedCells)
                {
                    cell.Selected = false;
                }
            }
        }

        private void _dgwLanguage_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (((DataGridViewCheckBoxCell)_dgwLanguage.Rows[e.RowIndex].Cells[e.ColumnIndex]).EditedFormattedValue.ToString().Equals(Boolean.TrueString))
                {
                    _dgwLanguage.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                }
                else
                {
                    foreach (DataGridViewRow row in _dgwLanguage.Rows)
                    {
                        row.Cells[1].Value = false;
                    }
                    _dgwLanguage.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
                }
            }
        }

        private void _btSave_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in _dgwLanguage.Rows)
            {
                if (((DataGridViewCheckBoxCell)row.Cells[1]).EditedFormattedValue.ToString().Equals(Boolean.TrueString))
                {
                    _language = row.Cells[0].Value.ToString();
                    DialogResult = DialogResult.OK;
                    return;
                }
            }
            DialogResult = DialogResult.Abort;
        }

        private void _btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
 
    }
}
