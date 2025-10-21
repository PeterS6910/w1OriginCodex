using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASLprCameraEditForm :
#if DESIGNER
        Form
#else
        ACgpPluginEditFormWithAlarmInstructions<LprCamera>
#endif
    {
        private sealed class CcuComboItem
        {
            public CcuComboItem(Guid id, string display)
            {
                Id = id;
                Display = display;
            }

            public Guid Id { get; }

            public string Display { get; }

            public override string ToString()
            {
                return Display;
            }
        }

#if !DESIGNER
        private bool _isLoadingValues;
        private IList<CcuComboItem> _ccuItems;
#endif
#if DESIGNER
        public NCASLprCameraEditForm()
        {
            InitializeComponent();
        }
#else
        public NCASLprCameraEditForm(
            LprCamera camera,
            ShowOptionsEditForm showOption,
            PluginMainForm<NCASClient> myTableForm)
            : base(
                camera ?? new LprCamera(),
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            _bApply.Enabled = false;
        }
#endif

#if !DESIGNER
        protected override void RegisterEvents()
        {
            _eName.TextChanged += EditTextChanger;
            _eId.TextChanged += EditTextChanger;
            _eIpAddress.TextChanged += EditTextChanger;
            _ePort.TextChanged += EditTextChanger;
            _ePortSsl.TextChanged += EditTextChanger;
            _eMacAddress.TextChanged += EditTextChanger;
            _eDescription.TextChanged += EditTextChanger;
            _eLocalAlarmInstruction.TextChanged += EditTextChanger;
            _cbCommunicationScope.SelectedIndexChanged += ControlChanged;
            _cbCcu.SelectedIndexChanged += ControlChanged;
            _chkEnableParentInFullName.CheckedChanged += ControlChanged;
        }

        protected override void BeforeInsert()
        {
        }

        protected override void BeforeEdit()
        {
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            allowEdit = true;

            var table = GetLprCameraTable();
            if (table == null)
                return;

            var result = GetObjectForEditById(table, _editingObject.IdLprCamera, out allowEdit);

            if (!allowEdit)
            {
                var readOnly = table.GetObjectById(_editingObject.IdLprCamera);
                if (readOnly != null)
                    _editingObject = readOnly;
                DisableForm();
            }
            else if (result != null)
            {
                _editingObject = result;
            }
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            var table = GetLprCameraTable();
            if (table == null)
                return;

            try
            {
                Exception error;
                table.RenewObjectForEdit(_editingObject.IdLprCamera, out error);

                if (error != null)
                    throw error;
            }
            catch (MissingMethodException)
            {

            }
        }

        protected override void SetValuesInsert()
        {
            SetFormValues();
        }

        protected override void SetValuesEdit()
        {
            SetFormValues();
        }

        private void SetFormValues()
        {
            LockChanges();
            _isLoadingValues = true;
            try
            {
                EnsureCommunicationScopeItems();
                EnsureHealthStateItems();
                EnsureCcuItems();
                _eName.Text = _editingObject.Name ?? string.Empty;
                _eId.Text = _editingObject.IdLprCamera != Guid.Empty
                    ? _editingObject.IdLprCamera.ToString()
                    : string.Empty;
                _eIpAddress.Text = _editingObject.IpAddress ?? string.Empty;
                _ePort.Text = _editingObject.Port ?? string.Empty;
                _ePortSsl.Text = _editingObject.PortSsl ?? string.Empty;
                _eMacAddress.Text = _editingObject.MacAddress ?? string.Empty;
                _eDescription.Text = _editingObject.Description ?? string.Empty;
                _eLocalAlarmInstruction.Text = _editingObject.LocalAlarmInstruction ?? string.Empty;
                _cbCommunicationScope.SelectedItem = _editingObject.CommunicationScope;
                SelectCcu(_editingObject.GuidCCU);
                _chkLocked.Checked = _editingObject.Locked;
                _eLockingClientIp.Text = _editingObject.LockingClientIp ?? string.Empty;
                _chkIsOnline.Checked = _editingObject.IsOnline;
                _eLastHeartbeatAt.Text = _editingObject.LastHeartbeatAt?.ToString("g", CultureInfo.CurrentCulture) ?? string.Empty;
                _eLastLicensePlate.Text = _editingObject.LastLicensePlate ?? string.Empty;
                _cbHealthState.SelectedItem = _editingObject.HealthState;
                _chkEnableParentInFullName.Checked = _editingObject.EnableParentInFullName;
                _eCkUnique.Text = _editingObject.CkUnique != Guid.Empty
                    ? _editingObject.CkUnique.ToString()
                    : string.Empty;
                _eObjectType.Text = _editingObject.ObjectType.ToString(CultureInfo.InvariantCulture);
                _eVersion.Text = _editingObject.Version.ToString(CultureInfo.InvariantCulture);
            }
            finally
            {
                _isLoadingValues = false;
                UnlockChanges();
            }
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrWhiteSpace(_eName.Text))
            {
                Dialog.Error("Name can not be empty.");
                _eName.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(_eIpAddress.Text)
                && !IPAddress.TryParse(_eIpAddress.Text.Trim(), out _))
            {
                Dialog.Error(GetString("ErrorNotValidIpAddress"));
                _eIpAddress.Focus();
                return false;
            }

            return true;
        }

        protected override bool GetValues()
        {
            _editingObject.Name = _eName.Text.Trim();
            _editingObject.IpAddress = _eIpAddress.Text.Trim();
            _editingObject.Port = _ePort.Text.Trim();
            _editingObject.PortSsl = _ePortSsl.Text.Trim();
            _editingObject.MacAddress = _eMacAddress.Text.Trim();
            _editingObject.Description = _eDescription.Text.Trim();
            _editingObject.LocalAlarmInstruction = _eLocalAlarmInstruction.Text;
            _editingObject.CommunicationScope = _cbCommunicationScope.SelectedItem is CommunicationScope scope
    ? scope
    : CommunicationScope.CcuOnly;
            _editingObject.EnableParentInFullName = _chkEnableParentInFullName.Checked;

            var selectedCcuId = GetSelectedCcuId();
            _editingObject.GuidCCU = selectedCcuId;
            if (selectedCcuId == Guid.Empty)
            {
                _editingObject.CCU = null;
            }
            else if (_editingObject.CCU == null || _editingObject.CCU.IdCCU != selectedCcuId)
            {
                var provider = Plugin?.MainServerProvider?.CCUs;
                if (provider != null)
                {
                    try
                    {
                        _editingObject.CCU = provider.GetObjectById(selectedCcuId);
                    }
                    catch
                    {
                        _editingObject.CCU = new CCU { IdCCU = selectedCcuId };
                    }
                }
                else
                {
                    _editingObject.CCU = new CCU { IdCCU = selectedCcuId };
                }
            }

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            var table = GetLprCameraTableOrThrow();
            var result = table.Insert(ref _editingObject, out var error);

            if (error != null)
                throw error;

            return result;
        }

        protected override bool SaveToDatabaseEdit()
        {
            var table = GetLprCameraTableOrThrow();

            var result = table.Update(_editingObject, out var error);


            if (error != null)
                throw error;

            return result;
        }

        protected override void UnregisterEvents()
        {
            _eName.TextChanged -= EditTextChanger;
            _eId.TextChanged -= EditTextChanger;
            _eIpAddress.TextChanged -= EditTextChanger;
            _ePort.TextChanged -= EditTextChanger;
            _ePortSsl.TextChanged -= EditTextChanger;
            _eMacAddress.TextChanged -= EditTextChanger;
            _eDescription.TextChanged -= EditTextChanger;
            _eLocalAlarmInstruction.TextChanged -= EditTextChanger;
            _cbCommunicationScope.SelectedIndexChanged -= ControlChanged;
            _cbCcu.SelectedIndexChanged -= ControlChanged;
            _chkEnableParentInFullName.CheckedChanged -= ControlChanged;
        }

        protected override void EditEnd()
        {
            var table = GetLprCameraTable();
            if (table == null)
                return;

            table.EditEnd(_editingObject);
        }

        protected override void AfterInsert()
        {
        }

        protected override void AfterEdit()
        {
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            if (_isLoadingValues)
                return;

            base.EditTextChanger(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        private void ControlChanged(object sender, EventArgs e)
        {
            if (_isLoadingValues)
                return;

            EditTextChanger(sender, e);
        }

        private void EnsureCommunicationScopeItems()
        {
            if (_cbCommunicationScope.Items.Count > 0)
                return;

            _cbCommunicationScope.Items.AddRange(Enum.GetValues(typeof(CommunicationScope))
                .Cast<object>()
                .ToArray());
        }

        private void EnsureHealthStateItems()
        {
            if (_cbHealthState.Items.Count > 0)
                return;

            _cbHealthState.Items.AddRange(Enum.GetValues(typeof(HealthState))
                .Cast<object>()
                .ToArray());
        }

        private string GetStringOrDefault(string key, string fallback)
        {
            var value = GetString(key);
            return string.IsNullOrEmpty(value) || string.Equals(value, key, StringComparison.Ordinal)
                ? fallback
                : value;
        }

        private void EnsureCcuItems()
        {
            if (_ccuItems != null)
                return;

            var provider = Plugin?.MainServerProvider?.CCUs;
            var noneText = GetStringOrDefault("None", "<None>");
            if (provider == null)
            {
                _ccuItems = new List<CcuComboItem> { new CcuComboItem(Guid.Empty, noneText) };
                _cbCcu.DataSource = _ccuItems;
                _cbCcu.Enabled = false;
                return;
            }

            try
            {
                Exception error;
                var ccus = provider.ShortSelectByCriteria(null, out error) ?? new List<CCUShort>();

                if (error != null)
                    throw error;

                _ccuItems = new List<CcuComboItem>
                {
                    new CcuComboItem(Guid.Empty, noneText)
                };

                _ccuItems = _ccuItems
                    .Concat(ccus
                        .OrderBy(ccu => ccu.IndexCCU)
                        .ThenBy(ccu => ccu.Name)
                        .Select(ccu => new CcuComboItem(ccu.IdCCU, string.Format(CultureInfo.CurrentCulture, "{0} - {1}", ccu.IndexCCU, ccu.Name ?? string.Empty))))
                    .ToList();

                _cbCcu.DataSource = _ccuItems;
                _cbCcu.Enabled = true;
            }
            catch
            {
                _ccuItems = new List<CcuComboItem> { new CcuComboItem(Guid.Empty, noneText) };
                _cbCcu.DataSource = _ccuItems;
                _cbCcu.Enabled = false;
            }
        }

        private void SelectCcu(Guid ccuId)
        {
            if (_ccuItems == null)
            {
                _cbCcu.SelectedIndex = -1;
                return;
            }

            if (ccuId != Guid.Empty && _ccuItems.All(item => item.Id != ccuId))
            {
                var wasEnabled = _cbCcu.Enabled;
                var missingText = GetStringOrDefault("NotAvailable", "Not available");
                _ccuItems.Add(new CcuComboItem(ccuId, string.Format(CultureInfo.CurrentCulture, "{0} ({1})", ccuId, missingText)));
                _cbCcu.DataSource = null;
                _cbCcu.DataSource = _ccuItems;
                _cbCcu.Enabled = wasEnabled;
            }

            var index = _ccuItems
                .Select((item, position) => new { item, position })
                .FirstOrDefault(pair => pair.item.Id == ccuId)?.position ?? 0;

            if (index >= 0 && index < _cbCcu.Items.Count)
                _cbCcu.SelectedIndex = index;
            else
                _cbCcu.SelectedIndex = 0;
        }

        private Guid GetSelectedCcuId()
        {
            return _cbCcu.SelectedItem is CcuComboItem item
                ? item.Id
                : Guid.Empty;
        }

        private static LprCamera GetObjectForEditById(ILprCameras table, Guid id, out bool allowEdit)
        {
            try
            {
                return table.GetObjectForEditById(id, out allowEdit);
            }
            catch (Exception ex) when (ex is MissingMethodException
                                       || ex is NotImplementedException
                                       || ex.InnerException is MissingMethodException)
            {
                var result = table.GetObjectForEdit(id, out var error);

                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    return null;
                }

                if (error != null)
                    throw error;

                allowEdit = true;
                return result;
            }
        }

        private ILprCameras GetLprCameraTable()
        {
            var provider = Plugin?.MainServerProvider;
            if (provider == null)
                return null;

            return provider.LprCameras;
        }

        private ILprCameras GetLprCameraTableOrThrow()
        {
            var table = GetLprCameraTable();
            if (table == null)
                throw new InvalidOperationException("Lpr cameras provider is not available.");

            return table;
        }
#endif

        private void _bApply_Click(object sender, EventArgs e)
        {
#if !DESIGNER
            if (Apply_Click())
                _bApply.Enabled = false;
#endif
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
#if !DESIGNER
            Ok_Click();
#endif
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
#if !DESIGNER
            Cancel_Click();
#else
            Close();
#endif
        }
    }
}
