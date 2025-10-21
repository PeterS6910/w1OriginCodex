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
        private readonly ICollection<FilterSettings>[] _filterSettingsWithJoin;
        private LogicalOperators _filterSettingsJoinOperator = LogicalOperators.AND;

        private OnlineState? _currentOnlineStateFilter;
        private Guid? _currentLocationFilter;
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
                new KeyValuePair<string, OnlineState?>(allLabel, null),
                new KeyValuePair<string, OnlineState?>(GetString(OnlineState.Online.ToString()), OnlineState.Online),
                new KeyValuePair<string, OnlineState?>(GetString(OnlineState.Offline.ToString()), OnlineState.Offline)
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
            SafeThread<IList>.StartThread(UpdateLocationFilter, dataSourceList);
        }

        private void UpdateLocationFilter(IList dataSourceList)
        {
            var selectedValue = _cbLocationFilter.InvokeRequired
                ? (Guid?)_cbLocationFilter.Invoke(new Func<Guid?>(() =>
                    _cbLocationFilter.SelectedValue is Guid guid ? guid : (Guid?)null))
                : _cbLocationFilter.SelectedValue as Guid?;

            var items = new List<KeyValuePair<string, Guid?>>
            {
                new KeyValuePair<string, Guid?>(GetAllFilterLabel(), null)
            };

            if (dataSourceList != null)
            {
                var locationPairs = new Dictionary<Guid, string>();

                foreach (LprCameraShort camera in dataSourceList)
                {
                    if (!camera.GuidCCU.HasValue)
                        continue;

                    if (!locationPairs.ContainsKey(camera.GuidCCU.Value))
                    {
                        locationPairs.Add(
                            camera.GuidCCU.Value,
                            string.IsNullOrEmpty(camera.Location)
                                ? camera.GuidCCU.Value.ToString()
                                : camera.Location);
                    }
                }

                items.AddRange(locationPairs.Select(pair =>
                    new KeyValuePair<string, Guid?>(pair.Value, pair.Key)));
            }

            if (_cbLocationFilter.InvokeRequired)
            {
                _cbLocationFilter.BeginInvoke(new Action(() => BindLocationFilter(items, selectedValue)));
            }
            else
            {
                BindLocationFilter(items, selectedValue);
            }
        }

        private void BindLocationFilter(IEnumerable<KeyValuePair<string, Guid?>> items, Guid? selectedValue)
        {
            var itemList = items.ToList();

            _cbLocationFilter.DisplayMember = "Key";
            _cbLocationFilter.ValueMember = "Value";
            _cbLocationFilter.DataSource = itemList;

            if (selectedValue.HasValue)
            {
                var match = itemList.FirstOrDefault(pair => pair.Value == selectedValue);
                if (!EqualityComparer<KeyValuePair<string, Guid?>>.Default.Equals(match, default(KeyValuePair<string, Guid?>))
                    && !string.IsNullOrEmpty(match.Key))
                {
                    var index = _cbLocationFilter.FindStringExact(match.Key);
                    if (index >= 0)
                        _cbLocationFilter.SelectedIndex = index;
                }
            }
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

            if (_currentLocationFilter.HasValue)
                filtered = filtered.Where(camera => camera.GuidCCU == _currentLocationFilter);

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

        private string GetAllFilterLabel()
        {
            var keys = new[] { "ComboBoxFilterAll", "FilterAll", "ComboBoxAll", "TextAll", "All" };

            foreach (var key in keys)
            {
                var localized = GetString(key);
                if (!string.Equals(localized, key, StringComparison.OrdinalIgnoreCase))
                    return localized;
            }

            return GetString("All");
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
                LprCameraShort.COLUMN_LAST_LICENSE_PLATE,
                LprCameraShort.COLUMN_LOCATION,
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
            throw new NotSupportedException();
        }

        protected override ACgpPluginEditForm<NCASClient, LprCamera> CreateEditForm(LprCamera obj, ShowOptionsEditForm showOption)
        {
            return new NCASLprCameraEditForm(obj, showOption, this);
        }

        private string GetTranslatedOnlineState(OnlineState onlineState)
        {
            return GetString(onlineState.ToString());
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

                var selectedCameras = lookupedForm.SelectedCameras;

                if (selectedCameras == null || selectedCameras.Count == 0)
                    return;

                Plugin.MainServerProvider.LprCameras.CreateLookupedLprCameras(
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
            _currentLocationFilter = null;

            if (!string.IsNullOrWhiteSpace(_eNameFilter.Text))
            {
                FilterSettings.Add(
                    new FilterSettings(LprCamera.COLUMNNAME, _eNameFilter.Text.Trim(), ComparerModes.LIKEBOTH));
            }

            if (_cbOnlineStateFilter.SelectedValue is OnlineState onlineState)
            {
                _currentOnlineStateFilter = onlineState;
                FilterSettings.Add(
                    new FilterSettings(LprCamera.COLUMNISONLINE, onlineState == OnlineState.Online, ComparerModes.EQUALL));
            }

            if (_cbLocationFilter.SelectedValue is Guid guid && guid != Guid.Empty)
            {
                _currentLocationFilter = guid;
                FilterSettings.Add(new FilterSettings(LprCamera.COLUMNGUIDCCU, guid, ComparerModes.EQUALL));
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
            _cbOnlineStateFilter.SelectedIndex = 0;
            _cbLocationFilter.SelectedIndex = 0;
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn
                    && CgpClient.Singleton.MainServerProvider != null)
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

        private void DataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_cdgvData.DataGrid.HitTest(e.X, e.Y).RowIndex < 0)
                return;

            EditSelectedRows();
        }

        private void EditSelectedRows()
        {
            var selectedRows = _cdgvData.DataGrid.SelectedRows;

            if (selectedRows == null || selectedRows.Count == 0)
            {
                EditClick();
                return;
            }

            var rowIndexes = selectedRows
                .OfType<DataGridViewRow>()
                .Select(row => row.Index)
                .Where(index => index >= 0)
                .Distinct()
                .OrderBy(index => index)
                .ToList();

            if (rowIndexes.Count <= 1)
            {
                EditClick();
                return;
            }

            EditClick(rowIndexes);
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
