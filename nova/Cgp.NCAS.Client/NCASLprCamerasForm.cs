using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASLprCamerasForm :
#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, LprCamera, LprCameraShort>
#endif
    {
        private const string TranslationError = "TRANSLATION_ERROR";
        private const string NoTranslationPrefix = "(!T)";
        private readonly ICollection<FilterSettings>[] _filterSettingsWithJoin;
        private LogicalOperators _filterSettingsJoinOperator = LogicalOperators.AND;

        private OnlineState? _currentOnlineStateFilter;
        private string _currentIpAddressFilter;
        private string _currentMacAddressFilter;
        private Action<ICollection<LookupedLprCamera>, ICollection<Guid>> _eventLprCameraLookupFinished;

        public NCASLprCamerasForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.LprCamera48;
            Icon = ResourceGlobal.IconLprCamera16;
            InitializeComponent();
            _filterSettingsWithJoin = new[]
            {
                FilterSettings,
                new List<FilterSettings>(),
                new List<FilterSettings>(),
                new List<FilterSettings>()
            };

            InitCGPDataGridView();
            PopulateOnlineStateFilter();
            SetOnlineStateFilterToDefault();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.AfterGridModified += _cdgvData_AfterGridModified;
            _cdgvData.AfterSort += AfterDataChanged;
            _cdgvData.CgpDataGridEvents = this;
            _cdgvData.EnabledInsertButton = true;
            _cdgvData.EnabledDeleteButton = false;
            _cdgvData.AutoOpenEditFormByDoubleClick = true;
        }

        private void PopulateOnlineStateFilter()
        {
            var allLabel = GetAllFilterLabel();
            var items = new List<KeyValuePair<string, OnlineState?>>
            {
                new KeyValuePair<string, OnlineState?>(GetAllFilterLabel(), null),
                new KeyValuePair<string, OnlineState?>(GetTranslatedOnlineState(OnlineState.Online), OnlineState.Online),
                new KeyValuePair<string, OnlineState?>(GetTranslatedOnlineState(OnlineState.Offline), OnlineState.Offline)
            };

            _cbOnlineStateFilter.DisplayMember = "Key";
            _cbOnlineStateFilter.ValueMember = "Value";
            _cbOnlineStateFilter.DataSource = items;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (LprCameraShort cameraShort in bindingSource.List)
            {
                cameraShort.Symbol = _cdgvData.GetDefaultImage(cameraShort);
                cameraShort.StringOnlineState = GetTranslatedOnlineState(cameraShort.OnlineState);
            }
        }

        void _cdgvData_AfterGridModified(BindingSource bindingSource)
        {
            AfterDataChanged(bindingSource?.List as IList);
        }

        private void AfterDataChanged(IList dataSourceList)
        {
            var count = dataSourceList?.Count ?? 0;

            if (_lRecordCount.InvokeRequired)
                _lRecordCount.BeginInvoke(new Action(() => UpdateRecordCount(count)));
            else
                UpdateRecordCount(count);
        }

        protected override LprCamera GetObjectForEdit(LprCameraShort listObj, out bool editAllowed)
        {
            if (listObj == null)
            {
                editAllowed = false;
                return null;
            }

            var provider = Plugin?.MainServerProvider;
            if (provider?.LprCameras == null)
            {
                editAllowed = false;
                return null;
            }
            return provider.LprCameras.GetObjectForEditById(listObj.IdLprCamera, out editAllowed);
        }

        protected override LprCamera GetFromShort(LprCameraShort listObj)
        {
            if (listObj == null)
                return null;

            var table = GetLprCameraTable();
            if (table == null)
                return null;

            return table.GetObjectById(listObj.IdLprCamera);
        }

        private static volatile NCASLprCamerasForm _singleton;
        private static readonly object _syncRoot = new object();

        public static NCASLprCamerasForm Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASLprCamerasForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        protected override ICollection<LprCameraShort> GetData()
        {
            var resultList = new List<LprCameraShort>();
            CheckAccess();

            var table = GetLprCameraTable();
            if (table == null)
                return ApplyRuntimeFiltering(resultList);

            try
            {
                var result = table.ShortSelectByCriteria(out var error, _filterSettingsJoinOperator, _filterSettingsWithJoin);

                if (error != null)
                    throw error;
                if (result == null)
                    return ApplyRuntimeFiltering(resultList);

                resultList.AddRange(result);
            }
            catch (Exception ex) when (ex is MissingMethodException
                           || ex is NotImplementedException
                           || ex.InnerException is MissingMethodException)
            {
                var legacyResult = table.ShortSelectByCriteria(FilterSettings, out var error);

                if (error != null)
                    throw error;

                if (legacyResult != null)
                    resultList.AddRange(legacyResult);
            }

            return ApplyRuntimeFiltering(resultList);
        }

        private void CheckAccess()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(CheckAccess));
            else
            {
                _cdgvData.EnabledInsertButton = HasAccessInsert();
                _cdgvData.EnabledDeleteButton = HasAccessDelete();
            }
        }

        private bool HasAccessDelete()
        {
            try
            {
                if (!CgpClient.Singleton.IsLoggedIn)
                    return false;

                var table = GetLprCameraTable();
                if (table == null)
                    return false;

                return table.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private ICollection<LprCameraShort> ApplyRuntimeFiltering(IEnumerable<LprCameraShort> cameras)
        {
            var filtered = cameras;

            if (_currentOnlineStateFilter.HasValue)
                filtered = filtered.Where(camera => camera.OnlineState == _currentOnlineStateFilter.Value);

            if (!string.IsNullOrWhiteSpace(_currentIpAddressFilter))
            {
                var ipFilter = _currentIpAddressFilter;
                filtered = filtered.Where(camera =>
                    !string.IsNullOrEmpty(camera.IpAddress)
                    && camera.IpAddress.IndexOf(ipFilter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(_currentMacAddressFilter))
            {
                var macFilter = _currentMacAddressFilter;
                filtered = filtered.Where(camera =>
                    !string.IsNullOrEmpty(camera.MacAddress)
                    && camera.MacAddress.IndexOf(macFilter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(_eNameFilter.Text))
            {
                var nameFilter = _eNameFilter.Text.Trim();
                filtered = filtered.Where(camera =>
                    (!string.IsNullOrEmpty(camera.Name) && camera.Name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                    || (!string.IsNullOrEmpty(camera.FullName) && camera.FullName.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            var list = filtered.ToList();

            if (_lRecordCount.InvokeRequired)
                _lRecordCount.BeginInvoke(new Action(() => UpdateRecordCount(list.Count)));
            else
                UpdateRecordCount(list.Count);

            return list;
        }

        private void UpdateRecordCount(int count)
        {
            _lRecordCount.Text = string.Format("{0} : {1}", GetString("TextRecordCount"), count);
        }

        private void SetOnlineStateFilterToDefault()
        {
            if (_cbOnlineStateFilter.DataSource == null)
                return;

            _cbOnlineStateFilter.SelectedIndex = 0;
        }

        private string GetAllFilterLabel()
        {
            var keys = new[] { "ComboBoxFilterAll", "FilterAll", "ComboBoxAll", "TextAll", "All" };

            foreach (var key in keys)
            {
                var localized = TryGetTranslation(key);

                if (string.IsNullOrWhiteSpace(localized))
                    continue;

                if (string.Equals(localized, key, StringComparison.OrdinalIgnoreCase))
                    continue;

                return localized;
            }

            return keys.Last();
        }

        protected override bool Compare(LprCamera obj1, LprCamera obj2)
        {
            return Equals(obj1, obj2);
        }

        protected override bool CombareByGuid(LprCamera obj, Guid idObj)
        {
            return obj != null && obj.IdLprCamera == idObj;
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            var dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            _cdgvData.ModifyGridView(
                bindingSource,
                LprCameraShort.COLUMN_SYMBOL,
                LprCameraShort.COLUMN_FULL_NAME,
                LprCameraShort.COLUMN_STRING_ONLINE_STATE,
                LprCameraShort.COLUMN_IP_ADDRESS,
                LprCameraShort.COLUMN_MAC_ADDRESS,
                LprCameraShort.COLUMN_PORT,
                LprCameraShort.COLUMN_PORT_SSL,
                LprCameraShort.COLUMN_LAST_LICENSE_PLATE,
                LprCameraShort.COLUMN_DESCRIPTION);

            _cdgvData.DataGrid.ColumnHeadersHeight = 34;
            _cdgvData.DataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            try
            {
                if (dgcell != null && dgcell.Visible)
                    _cdgvData.DataGrid.CurrentCell = dgcell;
            }
            catch
            {
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override void DeleteObj(LprCamera obj)
        {
            var table = GetLprCameraTable();
            if (table == null)
            {
                throw new InvalidOperationException("Lpr cameras provider is not available.");
            }

            Exception error;
            if (!table.Delete(obj, out error))
            {
                if (error != null)
                    throw error;

                throw new InvalidOperationException("Deleting LPR camera failed.");
            }
        }

        protected override ACgpPluginEditForm<NCASClient, LprCamera> CreateEditForm(LprCamera obj, ShowOptionsEditForm showOption)
        {
            return new NCASLprCameraEditForm(obj, showOption, this);
        }

        private string GetTranslatedOnlineState(OnlineState onlineState)
        {
            return TryGetTranslation(onlineState.ToString()) ?? onlineState.ToString();
        }

        private string TryGetTranslation(string key)
        {
            var localized = GetString(key);

            if (string.IsNullOrWhiteSpace(localized))
                return null;

            if (localized.StartsWith(NoTranslationPrefix, StringComparison.Ordinal))
                return null;

            return string.Equals(localized, TranslationError, StringComparison.OrdinalIgnoreCase)
                ? null
                : localized;
        }

        protected override void AfterTranslateForm()
        {
            base.AfterTranslateForm();

            if (_cbOnlineStateFilter == null)
                return;

            var selectAll = _cbOnlineStateFilter.DataSource != null
                             && _cbOnlineStateFilter.SelectedIndex == 0
                             && _cbOnlineStateFilter.SelectedValue == null;

            var selectedState = _cbOnlineStateFilter.SelectedValue is OnlineState state
                ? (OnlineState?)state
                : (OnlineState?)null;

            PopulateOnlineStateFilter();

            if (selectAll)
            {
                if (_cbOnlineStateFilter.Items.Count > 0)
                    _cbOnlineStateFilter.SelectedIndex = 0;
            }
            else if (selectedState.HasValue)
            {
                _cbOnlineStateFilter.SelectedValue = selectedState.Value;
            }
            else
            {
                SetOnlineStateFilterToDefault();
            }
        }

        private ILprCameras GetLprCameraTable()
        {
            var provider = Plugin?.MainServerProvider;
            if (provider?.LprCameras != null)
                return provider.LprCameras;

            var mainProvider = CgpClient.Singleton?.MainServerProvider as ICgpNCASRemotingProvider;
            return mainProvider?.LprCameras;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        private void _bFindAll_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        private void _bLookupLprCameras_Click(object sender, EventArgs e)
        {
            SafeThread.StartThread(DoLprCamerasLookup);
        }

        private void DoLprCamerasLookup()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            Plugin.MainServerProvider.LprCameras.LprCamerasLookup(CgpClient.Singleton.ClientID);
        }

        private void LprCameraLookupFinished(
            ICollection<LookupedLprCamera> lookupedCameras,
            ICollection<Guid> lookupingClients)
        {
            if (lookupedCameras == null
                || lookupedCameras.Count == 0
                || lookupingClients == null
                || !lookupingClients.Contains(CgpClient.Singleton.ClientID))
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<ICollection<LookupedLprCamera>, ICollection<Guid>>(LprCameraLookupFinished),
                    lookupedCameras,
                    lookupingClients);

                return;
            }

            using (var lookupedForm = new LookupedLprCamerasForm(Plugin))
            {
                if (lookupedForm.ShowDialog(lookupedCameras) != DialogResult.OK)
                    return;

                if (lookupedForm.CamerasAddedToDatabase)
                {
                    SafeThread.StartThread(ShowData);
                    return;
                }

                var selectedCameras = lookupedForm.SelectedCameras;

                if (selectedCameras == null || selectedCameras.Count == 0)
                    return;

                Plugin.MainServerProvider.CreateLookupedLprCameras(
                    selectedCameras,
                    lookupedForm.IdSelectedSubSite);

                SafeThread.StartThread(ShowData);
            }
        }

        protected override void SetFilterSettings()
        {
            _filterSettingsJoinOperator = LogicalOperators.AND;

            foreach (var list in _filterSettingsWithJoin)
                list.Clear();

            _currentOnlineStateFilter = null;
            _currentIpAddressFilter = null;
            _currentMacAddressFilter = null;

            if (!string.IsNullOrWhiteSpace(_eNameFilter.Text))
            {
                FilterSettings.Add(
                    new FilterSettings(LprCamera.COLUMNNAME, _eNameFilter.Text.Trim(), ComparerModes.LIKEBOTH));
            }

            if (!string.IsNullOrWhiteSpace(_eIpAddressFilter.Text))
            {
                _currentIpAddressFilter = _eIpAddressFilter.Text.Trim();
                FilterSettings.Add(
                    new FilterSettings(LprCamera.COLUMNIPADDRESS, _currentIpAddressFilter, ComparerModes.LIKEBOTH));
            }

            if (!string.IsNullOrWhiteSpace(_eMacAddressFilter.Text))
            {
                _currentMacAddressFilter = _eMacAddressFilter.Text.Trim();
                FilterSettings.Add(
                    new FilterSettings(LprCamera.COLUMNMACADDRESS, _currentMacAddressFilter, ComparerModes.LIKEBOTH));
            }

            if (_cbOnlineStateFilter.SelectedValue is OnlineState onlineState)
            {
                _currentOnlineStateFilter = onlineState;
                FilterSettings.Add(
                    new FilterSettings(LprCamera.COLUMNISONLINE, onlineState == OnlineState.Online, ComparerModes.EQUALL));
            }
        }

        protected override void ClearFilterEdits()
        {
            _currentOnlineStateFilter = null;
            _currentIpAddressFilter = null;
            _currentMacAddressFilter = null;
            _eNameFilter.Text = string.Empty;
            _eIpAddressFilter.Text = string.Empty;
            _eMacAddressFilter.Text = string.Empty;
            if (_cbOnlineStateFilter.DataSource != null)
                _cbOnlineStateFilter.SelectedIndex = 0;
        }

        public override bool HasAccessView()
        {
            try
            {
                if (!CgpClient.Singleton.IsLoggedIn)
                    return false;

                var table = GetLprCameraTable();

                if (table != null)
                    return table.HasAccessView();

                if (CgpClient.Singleton.MainServerProvider != null)
                    return CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.LprCamerasView));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(LprCamera lprCamera)
        {
            try
            {
                if (!CgpClient.Singleton.IsLoggedIn)
                    return false;

                var table = GetLprCameraTable();

                if (table != null)
                    return table.HasAccessViewForObject(lprCamera);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return HasAccessView();
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (!CgpClient.Singleton.IsLoggedIn)
                    return false;

                var table = GetLprCameraTable();
                if (table != null)
                    return table.HasAccessInsert();

                if (CgpClient.Singleton.MainServerProvider != null)
                    return CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.LprCameras));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        protected override void ApplyRuntimeFilter()
        {
            if (BindingSource == null)
                return;

            var data = BindingSource.List.Cast<LprCameraShort>();
            var filtered = ApplyRuntimeFiltering(data);
            BindingSource.DataSource = new BindingList<LprCameraShort>(filtered.ToList());
        }

        protected override void RegisterEvents()
        {
            if (_eventLprCameraLookupFinished == null)
            {
                _eventLprCameraLookupFinished = LprCameraLookupFinished;
                LprCameraLookupFinishedHandler.Singleton.RegisterLookupFinished(_eventLprCameraLookupFinished);
            }
        }

        protected override void UnregisterEvents()
        {
            if (_eventLprCameraLookupFinished != null)
            {
                LprCameraLookupFinishedHandler.Singleton.UnregisterLookupFinished(_eventLprCameraLookupFinished);
                _eventLprCameraLookupFinished = null;
            }
        }
    }
}
