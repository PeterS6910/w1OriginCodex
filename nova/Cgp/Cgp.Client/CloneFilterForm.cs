using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.Globals;
using Contal.IwQuick.UI;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public partial class CloneFilterForm :
#if DESIGNER
    Form
#else
 CgpTranslateForm
#endif
    {
        private Dictionary<string, bool> _filterValues = null;

        public Dictionary<string, bool> FilterValues
        {
            get { return _filterValues; }
            set { _filterValues = value; }
        }

        public CloneFilterForm(Dictionary<string, bool> filterValues)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            _filterValues = filterValues;
            InitializeComponent();
        }       

        private void CloneFilterForm_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, bool> item in _filterValues)
            {
                string localisedString = CgpClient.Singleton.GetLocalizedString(item.Key);
                if (localisedString == null || localisedString == string.Empty)
                    localisedString = item.Key;

                ListViewItem newItem = new ListViewItem(localisedString);
                newItem.Checked = item.Value;
                newItem.Tag = item.Key;
                _lvFilterValues.Items.Add(newItem);                
            }
            _lvFilterValues.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);            
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in _lvFilterValues.Items)
            {
                _filterValues[item.Tag as string] = item.Checked;
            }            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _lvFilterValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lvFilterValues.SelectedItems != null && _lvFilterValues.SelectedItems.Count > 0)
            {
                _lvFilterValues.SelectedItems[0].Checked = !_lvFilterValues.SelectedItems[0].Checked;
                _lvFilterValues.SelectedItems.Clear();
            }
        }
    }
}
