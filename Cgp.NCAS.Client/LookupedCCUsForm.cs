using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Contal.IwQuick.Localization;
using Contal.Cgp.NCAS.Client;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.Client
{
    public partial class LookupedCCUsForm : CgpTranslateForm
    {
        List<string> _selectedIpAddresses = new List<string>();

        public List<string> SelectedIpAddresses
        {
            get { return _selectedIpAddresses; }
            set { _selectedIpAddresses = value; }
        }

        private int? _idSelectedSubSite;

        public int? IdSelectedSubSite
        {
            get
            {
                return _idSelectedSubSite != -1
                    ? _idSelectedSubSite
                    : null;
            }
        }

        public LookupedCCUsForm(ICollection<LookupedCcu> ipAddresses)
            : base(NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            AddCheckBoxes(ipAddresses);
        }

        private BindingSource _bsCcus;
        private void AddCheckBoxes(ICollection<LookupedCcu> ipAddresses)
        {
            _dgLookupedCcus.AutoGenerateColumns = false;
            _bsCcus = new BindingSource();
            _bsCcus.DataSource = ipAddresses;
            _dgLookupedCcus.DataSource = _bsCcus;

            if (!_dgLookupedCcus.Columns.Contains(LookupedCcu.COLUMN_CHECKED))
            {
                DataGridViewCheckBoxColumn columnTypeIsChecked = new DataGridViewCheckBoxColumn();
                columnTypeIsChecked.Name = LookupedCcu.COLUMN_CHECKED;
                columnTypeIsChecked.DataPropertyName = LookupedCcu.COLUMN_CHECKED;
                columnTypeIsChecked.ReadOnly = false;
                columnTypeIsChecked.Width = 60;
                //columnTypeIsChecked.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;                
                _dgLookupedCcus.Columns.Add(columnTypeIsChecked);
            }

            if (!_dgLookupedCcus.Columns.Contains(LookupedCcu.COLUMN_IP_ADDRESS))
            {
                DataGridViewTextBoxColumn columnTypeIpAddress = new DataGridViewTextBoxColumn();
                columnTypeIpAddress.Name = LookupedCcu.COLUMN_IP_ADDRESS;
                columnTypeIpAddress.DataPropertyName = LookupedCcu.COLUMN_IP_ADDRESS;
                columnTypeIpAddress.ReadOnly = true;
                columnTypeIpAddress.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                _dgLookupedCcus.Columns.Add(columnTypeIpAddress);
            }

            if (!_dgLookupedCcus.Columns.Contains(LookupedCcu.COLUMN_MAINBOARD_TYPE))
            {
                DataGridViewTextBoxColumn columnTypeMainBoadType = new DataGridViewTextBoxColumn();
                columnTypeMainBoadType.Name = LookupedCcu.COLUMN_MAINBOARD_TYPE;
                columnTypeMainBoadType.DataPropertyName = LookupedCcu.COLUMN_MAINBOARD_TYPE;
                columnTypeMainBoadType.ReadOnly = true;
                columnTypeMainBoadType.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                _dgLookupedCcus.Columns.Add(columnTypeMainBoadType);
            }

            if (!_dgLookupedCcus.Columns.Contains(LookupedCcu.COLUMN_CCU_VERSION))
            {
                DataGridViewTextBoxColumn columnTypeCcuVersion = new DataGridViewTextBoxColumn();
                columnTypeCcuVersion.Name = LookupedCcu.COLUMN_CCU_VERSION;
                columnTypeCcuVersion.DataPropertyName = LookupedCcu.COLUMN_CCU_VERSION;
                columnTypeCcuVersion.ReadOnly = true;
                columnTypeCcuVersion.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                _dgLookupedCcus.Columns.Add(columnTypeCcuVersion);
            }

            if (!_dgLookupedCcus.Columns.Contains(LookupedCcu.COLUMN_CE_VERSION))
            {
                DataGridViewTextBoxColumn columnTypeCeVersion = new DataGridViewTextBoxColumn();
                columnTypeCeVersion.Name = LookupedCcu.COLUMN_CE_VERSION;
                columnTypeCeVersion.DataPropertyName = LookupedCcu.COLUMN_CE_VERSION;
                columnTypeCeVersion.ReadOnly = true;
                columnTypeCeVersion.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                _dgLookupedCcus.Columns.Add(columnTypeCeVersion);
            }
            NCASClient.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgLookupedCcus);
        }

        private void AddSelectedCCUs()
        {
            _selectedIpAddresses.Clear();

            foreach (DataGridViewRow row in _dgLookupedCcus.Rows)
            {
                LookupedCcu lCcu = (LookupedCcu)((BindingSource)_dgLookupedCcus.DataSource).List[row.Index];
                if (lCcu != null && lCcu.IsChecked)
                {
                    _selectedIpAddresses.Add(lCcu.IPAddress);
                }
            }
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            AddSelectedCCUs();

            var selectStructuredSubSiteForm = new SelectStructuredSubSiteForm();

            ICollection<int> idSelectedSubSites;

            if (!selectStructuredSubSiteForm.SelectStructuredSubSites(
                    false,
                    out idSelectedSubSites))
            {
                return;
            }

            _idSelectedSubSite = idSelectedSubSites != null
                ? (int?)idSelectedSubSites.First()
                : null;

            DialogResult = DialogResult.OK;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void LookupedCCUsForm_Shown(object sender, EventArgs e)
        {
            BringToFront();
        }

        private void _cbSelectUnselectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in _dgLookupedCcus.Rows)
            {
                LookupedCcu lCcu = (LookupedCcu)((BindingSource)_dgLookupedCcus.DataSource).List[row.Index];
                if (lCcu != null)
                {
                    lCcu.IsChecked = _cbSelectUnselectAll.Checked;
                }
            }
            _dgLookupedCcus.DataSource = _bsCcus;
            _dgLookupedCcus.Refresh();
        }
    }
}
