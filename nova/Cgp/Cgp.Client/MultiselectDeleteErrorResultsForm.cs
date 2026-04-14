using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick.Localization;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Client
{
    public partial class MultiselectDeleteErrorResultsForm :
#if DESIGNER
    Form
#else
    CgpTranslateForm
#endif
    {
        public MultiselectDeleteErrorResultsForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgErrorResults);
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void ShowDialog(Dictionary<AOrmObject, string> multiSelectDeleteResults)
        {
            if (multiSelectDeleteResults != null && multiSelectDeleteResults.Count > 0)
            {
                List<MultiselectDeleteErrorResults> errorResults = new List<MultiselectDeleteErrorResults>();

                foreach (KeyValuePair<AOrmObject, string> kvp in multiSelectDeleteResults)
                {
                    MultiselectDeleteErrorResults multiselectDeleteErrorResults = new MultiselectDeleteErrorResults(kvp.Key, kvp.Value);
                    errorResults.Add(multiselectDeleteErrorResults);
                }

                _dgErrorResults.DataSource = errorResults;

                ShowDialog();
            }
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            int rowIndex = -1;
            DataGridViewSelectedRowCollection selectedRows = _dgErrorResults.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
                rowIndex = selectedRows[0].Index;

            List<MultiselectDeleteErrorResults> rows = _dgErrorResults.DataSource as List<MultiselectDeleteErrorResults>;
            if (rows != null && rowIndex >= 0 && rows.Count > rowIndex)
            {
                DbsSupport.OpenEditDialog(rows[rowIndex].AOrmObject);
            }
        }

        private void _dgErrorResults_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _bEdit_Click(sender, null);
        }
    }

    public class MultiselectDeleteErrorResults
    {
        private AOrmObject _aOrmObject;
        private string _fullName;
        private string _error;

        public AOrmObject AOrmObject { get { return _aOrmObject; } }
        public string FullName { get { return _fullName; } }
        public string Error { get { return _error; } }

        public MultiselectDeleteErrorResults(AOrmObject ormObject, string error)
        {
            if (ormObject != null)
            {
                _aOrmObject = ormObject;
                _fullName = ormObject.ToString();
                _error = error;
            }
            else
            {
                _aOrmObject = null;
                _fullName = string.Empty;
                _error = string.Empty;
            }
        }
    }
}
