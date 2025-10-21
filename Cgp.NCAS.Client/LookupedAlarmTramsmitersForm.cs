using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Contal.Cgp.Client;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Client
{
    public partial class LookupedAlarmTramsmitersForm : CgpTranslateForm
    {
        private const string COLUMN_CHECKED = "IsChecked";
        private readonly NCASClient _plugin;

        public LookupedAlarmTramsmitersForm(NCASClient plugin)
            : base (NCASClient.LocalizationHelper)
        {
            _plugin = plugin;
            InitializeComponent();
        }

        private void ShowLookupedTransmitters(IEnumerable<LookupedAlarmTransmitter> lookupedTransmitters)
        {

            if (!_dgLookupedTransmitters.Columns.Contains(COLUMN_CHECKED))
            {
                var columnTypeIsChecked = new DataGridViewCheckBoxColumn
                {
                    Name = COLUMN_CHECKED,
                    ReadOnly = false,
                    Width = 60
                };

                _dgLookupedTransmitters.Columns.Add(columnTypeIsChecked);
            }

            if (!_dgLookupedTransmitters.Columns.Contains(LookupedAlarmTransmitter.COLUMN_IP_ADDRESS))
            {
                var columnTypeIpAddress = new DataGridViewTextBoxColumn
                {
                    Name = LookupedAlarmTransmitter.COLUMN_IP_ADDRESS,
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };

                _dgLookupedTransmitters.Columns.Add(columnTypeIpAddress);
            }

            if (!_dgLookupedTransmitters.Columns.Contains(LookupedAlarmTransmitter.COLUMN_MAINBOARD_TYPE))
            {
                var columnTypeMainBoadType = new DataGridViewTextBoxColumn
                {
                    Name = LookupedAlarmTransmitter.COLUMN_MAINBOARD_TYPE,
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };

                _dgLookupedTransmitters.Columns.Add(columnTypeMainBoadType);
            }

            if (!_dgLookupedTransmitters.Columns.Contains(LookupedAlarmTransmitter.COLUMN_CCU_VERSION))
            {
                var columnTypeCcuVersion = new DataGridViewTextBoxColumn
                {
                    Name = LookupedAlarmTransmitter.COLUMN_CCU_VERSION,
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };

                _dgLookupedTransmitters.Columns.Add(columnTypeCcuVersion);
            }

            if (!_dgLookupedTransmitters.Columns.Contains(LookupedAlarmTransmitter.COLUMN_CE_VERSION))
            {
                var columnTypeCeVersion = new DataGridViewTextBoxColumn
                {
                    Name = LookupedAlarmTransmitter.COLUMN_CE_VERSION,
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };

                _dgLookupedTransmitters.Columns.Add(columnTypeCeVersion);
            }

            NCASClient.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgLookupedTransmitters);

            if (lookupedTransmitters == null)
                return;

            foreach (var lookupedTransmitter in lookupedTransmitters)
            {
                _dgLookupedTransmitters.Rows.Add(
                    true,
                    lookupedTransmitter.IpAddress,
                    lookupedTransmitter.MainboardType,
                    lookupedTransmitter.Cat12ceVersion,
                    lookupedTransmitter.CeVersion);
            }
        }

        private void _cbSelectUnselectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in _dgLookupedTransmitters.Rows)
            {
                row.Cells[COLUMN_CHECKED].Value = _cbSelectUnselectAll.Checked;
            }
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            var selectStructuredSubSiteForm = new SelectStructuredSubSiteForm();

            ICollection<int> idSelectedSubSites;

            if (!selectStructuredSubSiteForm.SelectStructuredSubSites(
                    false,
                    out idSelectedSubSites))
            {
                return;
            }

            var idSelectedSubSite = idSelectedSubSites != null
                ? (int?) idSelectedSubSites.First()
                : null;

            var selectedIpAddresses = new LinkedList<string>();

            foreach (DataGridViewRow row in _dgLookupedTransmitters.Rows)
            {
                if (!(bool)row.Cells[COLUMN_CHECKED].Value)
                {
                    continue;
                }

                selectedIpAddresses.AddLast(
                    row.Cells[LookupedAlarmTransmitter.COLUMN_IP_ADDRESS].Value.ToString());
            }

            try
            {
                _plugin.MainServerProvider.AlarmTransmitters.CreateLookupedAlarmTransmitters(
                    selectedIpAddresses,
                    idSelectedSubSite != -1
                        ? idSelectedSubSite
                        : null
                    );
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LookupedTramsmitersForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _bCancel_Click(null, null);
        }

        public void ShowDialog(IEnumerable<LookupedAlarmTransmitter> lookupedTransmitters)
        {
            ShowLookupedTransmitters(lookupedTransmitters);
            ShowDialog();
        }
    }
}
