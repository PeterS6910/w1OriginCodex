using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.Client
{
    public partial class LookupedLprCamerasForm : CgpTranslateForm
    {
        private readonly NCASClient _plugin;
        private BindingSource _bindingSource;
        private IList<LookupedLprCamera> _selectedCameras;
        private int? _idSelectedSubSite;
        private bool _camerasAddedToDatabase;

        public IList<LookupedLprCamera> SelectedCameras => _selectedCameras ?? new List<LookupedLprCamera>();

        public int? IdSelectedSubSite => _idSelectedSubSite;
        public bool CamerasAddedToDatabase => _camerasAddedToDatabase;

        public LookupedLprCamerasForm(NCASClient plugin)
            : base(NCASClient.LocalizationHelper)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            _plugin = plugin;
            InitializeComponent();
        }

        public DialogResult ShowDialog(ICollection<LookupedLprCamera> lookupedCameras)
        {
            BindLookupedCameras(lookupedCameras);
            return ShowDialog();
        }

        private void BindLookupedCameras(ICollection<LookupedLprCamera> lookupedCameras)
        {
            _bindingSource = new BindingSource
            {
                DataSource = new BindingList<LookupedLprCamera>(
                    lookupedCameras != null
                        ? lookupedCameras.Select(CloneLookupedCamera).ToList()
                        : new List<LookupedLprCamera>())
            };

            _dgLookupedCameras.AutoGenerateColumns = false;
            _dgLookupedCameras.DataSource = _bindingSource;

            EnsureColumns();
            NCASClient.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgLookupedCameras);
        }

        private static LookupedLprCamera CloneLookupedCamera(LookupedLprCamera camera)
        {
            if (camera == null)
                return null;

            return new LookupedLprCamera
            {
                IsChecked = camera.IsChecked,
                IpAddress = camera.IpAddress,
                Name = camera.Name,
                Port = camera.Port,
                PortSsl = camera.PortSsl,
                Equipment = camera.Equipment,
                Version = camera.Version,
                Locked = camera.Locked,
                LockingClientIp = camera.LockingClientIp,
                MacAddress = camera.MacAddress,
                Serial = camera.Serial,
                Model = camera.Model,
                Type = camera.Type,
                Build = camera.Build,
                InterfaceSource = camera.InterfaceSource,
                UniqueKey = camera.UniqueKey
            };
        }

        private void EnsureColumns()
        {
            if (!_dgLookupedCameras.Columns.Contains(LookupedLprCamera.COLUMN_CHECKED))
            {
                var checkColumn = new DataGridViewCheckBoxColumn
                {
                    Name = LookupedLprCamera.COLUMN_CHECKED,
                    DataPropertyName = LookupedLprCamera.COLUMN_CHECKED,
                    Width = 50,
                    ReadOnly = false
                };

                _dgLookupedCameras.Columns.Add(checkColumn);
            }

            AddTextColumn(LookupedLprCamera.COLUMN_IP_ADDRESS, true, 140);
            AddTextColumn(LookupedLprCamera.COLUMN_NAME, true, 160);
            AddTextColumn(LookupedLprCamera.COLUMN_PORT, true, 60);
            AddTextColumn(LookupedLprCamera.COLUMN_PORT_SSL, true, 80);
            AddTextColumn(LookupedLprCamera.COLUMN_MODEL, true, 140);
            AddTextColumn(LookupedLprCamera.COLUMN_TYPE, true, 120);
            AddTextColumn(LookupedLprCamera.COLUMN_EQUIPMENT, true, 140);
            AddTextColumn(LookupedLprCamera.COLUMN_VERSION, true, 120);
            AddTextColumn(LookupedLprCamera.COLUMN_MAC_ADDRESS, true, 140);
            AddTextColumn(LookupedLprCamera.COLUMN_SERIAL, true, 140);
            AddTextColumn(LookupedLprCamera.COLUMN_INTERFACE_SOURCE, true, 140);
            AddTextColumn(LookupedLprCamera.COLUMN_BUILD, true, 80);
            AddTextColumn(LookupedLprCamera.COLUMN_LOCKED, true, 70);
            AddTextColumn(LookupedLprCamera.COLUMN_LOCKING_CLIENT_IP, true, 140);
            AddTextColumn(LookupedLprCamera.COLUMN_UNIQUE_KEY, true, 160);
        }

        private void AddTextColumn(string columnName, bool readOnly, int width)
        {
            if (_dgLookupedCameras.Columns.Contains(columnName))
                return;

            var column = new DataGridViewTextBoxColumn
            {
                Name = columnName,
                DataPropertyName = columnName,
                ReadOnly = readOnly,
                Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            };

            _dgLookupedCameras.Columns.Add(column);
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            _camerasAddedToDatabase = false;

            _selectedCameras = _bindingSource.Cast<LookupedLprCamera>()
                .Where(camera => camera.IsChecked)
                .Select(CloneLookupedCamera)
                .ToList();

            if (_selectedCameras.Count == 0)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }

            ICollection<int> selectedSubSites;
            var selectForm = new SelectStructuredSubSiteForm();

            if (!selectForm.SelectStructuredSubSites(false, out selectedSubSites))
                return;

            _idSelectedSubSite = selectedSubSites != null
                ? (int?)selectedSubSites.FirstOrDefault()
                : null;
            AddSelectedCamerasToDatabase();
            DialogResult = DialogResult.OK;
        }

        private void AddSelectedCamerasToDatabase()
        {
            if (_selectedCameras == null || _selectedCameras.Count == 0)
                return;

            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            var lprCameras = _plugin?.MainServerProvider?.LprCameras;

            if (lprCameras == null)
            {
                MessageBox.Show("Main Server Provider not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                lprCameras.CreateLookupedLprCameras(
                    _selectedCameras,
                    _idSelectedSubSite);

                _camerasAddedToDatabase = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void _cbSelectUnselectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (_bindingSource == null)
                return;

            foreach (LookupedLprCamera camera in _bindingSource)
            {
                camera.IsChecked = _cbSelectUnselectAll.Checked;
            }

            _bindingSource.ResetBindings(false);
        }

        private void LookupedLprCamerasForm_Shown(object sender, EventArgs e)
        {
            BringToFront();
        }
    }
}
