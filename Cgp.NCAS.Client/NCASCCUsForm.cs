using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;

using System.Collections;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASCCUsForm :
#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, CCU, CCUShort>
#endif
    {
        private Action<Guid, byte> _eventCCUOnlineStateChanged;
        private Action<Guid, byte, bool, bool> _eventCCUConfigurationStateChanged;
        private DVoid2Void _eventCreatedCCU;
        private Action<ICollection<LookupedCcu>, ICollection<Guid>> _eventCCULookupFinished;

        public NCASCCUsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.ccu_48;
            InitializeComponent();
            InitCGPDataGridView();
            FormOnEnter += FormEnterHF;
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.AfterGridModified += _cdgvData_AfterGridModified;
            _cdgvData.AfterSort += AfterDataChanged;
            _cdgvData.CgpDataGridEvents = this;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (CCUShort ccuShort in bindingSource.List)
            {
                ccuShort.Symbol = _cdgvData.GetSubTypeImage(ccuShort, ccuShort.OnlineState);
                ccuShort.StringOnlineState = GetTranslatedOnlineState(ccuShort.OnlineState);
            }
        }

        void _cdgvData_AfterGridModified(BindingSource bindingSource)
        {
            AfterDataChanged(bindingSource.List);
        }

        private void AfterDataChanged(IList dataSourceList)
        {
            int i = 0;

            if (dataSourceList != null)
                foreach (CCUShort ccuShort in dataSourceList)
                {
                    bool blockedByLicence = false;

                    if (Plugin.MainServerProvider.CCUs.IsCat12Combo(ccuShort.IdCCU))
                    {
                        if (Plugin.MainServerProvider.CCUs.GetFreeCat12ComboLicenceCount() == 0)
                            blockedByLicence = true;
                    }

                    Color color;
                    _cdgvData.UpdateValueTextColor(i, CCUShort.COLUMN_STRING_CONFIGURATION_STATE,
                        GetTranslatedConfigurationState(ccuShort.ConfigurationState, blockedByLicence, out color), color);
                    ShowHideRowByCCUOnlieState(i, ccuShort.OnlineState);
                    i++;
                }
        }

        private void FormEnterHF(Form form)
        {
            CbOnlineStateLocalizeName();
        }

        protected override CCU GetObjectForEdit(CCUShort listObj, out bool editAllowed)
        {
            return Plugin.MainServerProvider.CCUs.GetObjectForEditById(listObj.IdCCU, out editAllowed);
        }

        protected override CCU GetFromShort(CCUShort listObj)
        {
            if (listObj == null) return null;

            return Plugin.MainServerProvider.CCUs.GetObjectById(listObj.IdCCU);
        }

        private static volatile NCASCCUsForm _singleton;
        private static readonly object _syncRoot = new object();

        public static NCASCCUsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton =
                                new NCASCCUsForm
                                {
                                    MdiParent = CgpClientMainForm.Singleton
                                };
                    }
                return _singleton;
            }
        }

        protected override ICollection<CCUShort> GetData()
        {
            Exception error;
            ICollection<CCUShort> list = Plugin.MainServerProvider.CCUs.ShortSelectByCriteria(FilterSettings, out error);

            if (error != null)
                throw (error);

            CheckAccess();
            _lRecordCount.BeginInvoke(new Action(
            () =>
            {
                _lRecordCount.Text = string.Format("{0} : {1}",
                                                        GetString("TextRecordCount"),
                                                        list == null
                                                            ? 0
                                                            : list.Count);
            }));
            return list;
        }

        private void CCUOnlineStateChanged(Guid ccuGuid, byte onlineState)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(CCUOnlineStateChanged), ccuGuid, onlineState);
            }
            else
            {
                int i = 0;
                var ccuShorts = ((BindingSource)_cdgvData.DataGrid.DataSource).List;

                foreach (CCUShort ccuShort in ccuShorts)
                {
                    if (ccuShort != null && ccuShort.IdCCU == ccuGuid)
                    {
                        ccuShort.OnlineState = (CCUOnlineState)onlineState;
                        _cdgvData.UpdateValue(i, CCUShort.COLUMN_STRING_ONLINE_STATE, GetTranslatedOnlineState((CCUOnlineState)onlineState));
                        _cdgvData.UpdateImage(ccuShort, i, ccuShort.OnlineState);
                        ShowHideRowByCCUOnlieState(i, ccuShort.OnlineState);
                    }
                    i++;
                }
            }
        }

        private void CCUConfigurationStateChanged(Guid ccuGuid, byte configurationState, bool isCCU0, bool blockedByLicence)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte, bool, bool>(CCUConfigurationStateChanged), ccuGuid, configurationState, isCCU0, blockedByLicence);
            }
            else
            {
                int i = 0;

                var ccuShorts = ((BindingSource)_cdgvData.DataGrid.DataSource).List;

                foreach (CCUShort ccuShort in ccuShorts)
                {
                    if (ccuShort != null && ccuShort.IdCCU == ccuGuid)
                    {
                        ccuShort.ConfigurationState = (CCUConfigurationState)configurationState;

                        Color color;

                        String configurationText = GetTranslatedConfigurationState(
                            (CCUConfigurationState) configurationState,
                            blockedByLicence,
                            out color);

                        _cdgvData.UpdateValueTextColor(
                            i, 
                            CCUShort.COLUMN_STRING_CONFIGURATION_STATE,
                            configurationText, 
                            color);

                        break;
                    }
                    i++;
                }
            }
        }

        private string GetTranslatedOnlineState(CCUOnlineState onlineState)
        {
            switch (onlineState)
            {
                case CCUOnlineState.Unknown: return GetString("Unknown");
                case CCUOnlineState.Offline: return GetString("Offline");
                case CCUOnlineState.Online: return GetString("Online");
                default: return GetString("Unknown");
            }
        }

        private string GetTranslatedConfigurationState(CCUConfigurationState configurationState, bool blockedByLicence, out Color color)
        {
            color = Color.Black;
            switch (configurationState)
            {
                case CCUConfigurationState.Unconfigured: 
                    color = Color.Black;
                    String configurationText = GetString("CCUConfigured_Unconfigured");

                    if (blockedByLicence)
                        configurationText += @" - " + GetString("BlockedByLicence");

                    return configurationText;

                case CCUConfigurationState.ConfiguredForThisServer: 
                    color = Color.Green;
                    return GetString("CCUConfigured_ConfiguredForThisServer");

                case CCUConfigurationState.ConfiguredForAnotherServer: 
                    color = Color.Red;
                    return GetString("CCUConfigured_ConfiguredForAnotherServer");

                case CCUConfigurationState.Upgrading: 
                    color = Color.Orange;
                    return GetString("CCUConfigured_Upgrading");

                case CCUConfigurationState.Unknown: 
                    color = Color.Black;
                    return GetString("Unknown");

                case CCUConfigurationState.ForceReconfiguration: 
                    color = Color.Blue;
                    return GetString("CCUConfigured_ForceReconfiguration");

                case CCUConfigurationState.ConfiguredForThisServerUpgradeOnly:
                case CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe: 
                    color = Color.Gray;
                    return GetString("CCUConfigured_ConfiguredForThisServerUpgradeOnly");

                default: 
                    return GetString("Unknown");
            }
        }

        private void CreatedCCU()
        {
            ShowData();
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

        protected override bool Compare(CCU obj1, CCU obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(CCU obj, Guid idObj)
        {
            return obj.IdCCU == idObj;
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, CCUShort.COLUMN_SYMBOL, CCUShort.COLUMN_INDEX_CCU, CCUShort.COLUMN_NAME, CCUShort.COLUMN_IP_ADDRESS, 
                CCUShort.COLUMN_MAC_ADDRESS, CCUShort.COLUMN_MAINBOARD_TYPE, CCUShort.COLUMN_STRING_ONLINE_STATE, CCUShort.COLUMN_STRING_CONFIGURATION_STATE,
                CCUShort.COLUMN_DESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        protected override ACgpPluginEditForm<NCASClient, CCU> CreateEditForm(CCU obj, ShowOptionsEditForm showOption)
        {
            return new NCASCCUEditForm(obj, showOption, this);
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            Edit_Click();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            Delete_Click();
        }

        protected override void DeleteObj(CCU obj)
        {
            if (!AddToDeletedCcus(obj.IdCCU))
            {
                //Dialog.Info(GetString("InfoCcuIsDeleting"));
                return;
            }

            StatusReport.Info($"Starting to delete CCU: {obj}");

            SafeThread<CCU>.StartThread(ThreadDeleteCcu, obj);
        }
        
        private readonly List<Guid> _actualDeletingCcus = new List<Guid>();

        private void ThreadDeleteCcu(CCU obj)
        {
            Guid idCcu = obj.IdCCU;
            string ccuName = obj.ToString();

            try
            {
                Exception error;

                if (Plugin.MainServerProvider.CCUs.Delete(obj, out error))
                {
                    ShowData();

                    StatusReport.Info("CCU: " + ccuName + " " + GetString("CcuWasDeleted"));
                }
                else
                {
                    string errorMessage = error is SqlDeleteReferenceConstraintException ?
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteRowInRelationship") :
                        "CCU: " + ccuName + " " + GetString("CcuDeleteFailed");

                    StatusReport.Error(errorMessage);
                    Dialog.Error(errorMessage);
                }
            }
            finally
            {
                RemoveFromDeletingCcus(idCcu);
            }
        }

        private bool AddToDeletedCcus(Guid idCcu)
        {
            try
            {
                if (_actualDeletingCcus.Contains(idCcu))
                    return false;
                _actualDeletingCcus.Add(idCcu);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void RemoveFromDeletingCcus(Guid idCcu)
        {
            try
            {
                _actualDeletingCcus.Remove(idCcu);
            }
            catch { }
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != "")
                FilterSettings.Add(
                    new FilterSettings(
                        CCU.COLUMNNAME,
                        _eNameFilter.Text,
                        ComparerModes.LIKEBOTH));
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _cbOnlineStateFilter.SelectedItem = string.Empty;
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = "";
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.CCUs.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(CCU ccu)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.CCUs.HasAccessViewForObject(ccu);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.CCUs.HasAccessInsert();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public bool HasAccessDelete()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.CCUs.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private void _bCCUsLookUp_Click(object sender, EventArgs e)
        {
            SafeThread.StartThread(DoCCUsLookUp);
        }

        private void DoCCUsLookUp()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            Plugin.MainServerProvider.CCUs.DoCCUsLookUp(CgpClient.Singleton.ClientID);
        }

        private void CCULookupFinished(ICollection<LookupedCcu> ipAddresses, ICollection<Guid> lookupingClients)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ICollection<LookupedCcu>, ICollection<Guid>>(CCULookupFinished), ipAddresses, lookupingClients);
            }
            else
            {
                if (ipAddresses == null || ipAddresses.Count < 1)
                    return;

                if (lookupingClients == null || !lookupingClients.Contains(CgpClient.Singleton.ClientID))
                    return;

                var lookupedCCUsform = new LookupedCCUsForm(ipAddresses);

                if (lookupedCCUsform.ShowDialog(ParentForm) == DialogResult.OK)
                    AddLookupedCCUs(
                        lookupedCCUsform.SelectedIpAddresses,
                        lookupedCCUsform.IdSelectedSubSite);
            }
        }

        private void AddLookupedCCUs(
            List<string> ipAddresses,
            int? idSelectedSubSite)
        {
            Plugin.MainServerProvider.InsertLookupedCCUs(
                ipAddresses,
                idSelectedSubSite);
        }

        protected override void RegisterEvents()
        {
            if (_eventCCUOnlineStateChanged == null)
            {
                _eventCCUOnlineStateChanged = CCUOnlineStateChanged;
                StateChangedCCUHandler.Singleton.RegisterStateChanged(_eventCCUOnlineStateChanged);
            }

            if (_eventCCUConfigurationStateChanged == null)
            {
                _eventCCUConfigurationStateChanged = CCUConfigurationStateChanged;
                ConfiguredChangedCCUHandler.Singleton.RegisterConfiguredChanged(_eventCCUConfigurationStateChanged);
            }

            if (_eventCreatedCCU == null)
            {
                _eventCreatedCCU = CreatedCCU;
                CreatedCCUHandler.Singleton.RegisterCreatedCCUEvent(_eventCreatedCCU);
            }

            if (_eventCCULookupFinished == null)
            {
                _eventCCULookupFinished = CCULookupFinished;
                CCULookupFinishedHandler.Singleton.RegisterLookupFinished(_eventCCULookupFinished);
            }
        }

        protected override void UnregisterEvents()
        {
            if (_eventCCUOnlineStateChanged != null)
            {
                StateChangedCCUHandler.Singleton.UnregisterStateChanged(_eventCCUOnlineStateChanged);
                _eventCCUOnlineStateChanged = null;
            }
            if (_eventCCUConfigurationStateChanged != null)
            {
                ConfiguredChangedCCUHandler.Singleton.UnregisterConfiguredChanged(_eventCCUConfigurationStateChanged);
                _eventCCUConfigurationStateChanged = null;
            }
            if (_eventCreatedCCU != null)
            {
                CreatedCCUHandler.Singleton.UnregisterCreatedCCUEvent(_eventCreatedCCU);
                _eventCreatedCCU = null;
            }

            if (_eventCCULookupFinished != null)
            {
                CCULookupFinishedHandler.Singleton.UnregisterLookupFinished(_eventCCULookupFinished);
                _eventCCULookupFinished = null;
            }
        }

        private void CbOnlineStateLocalizeName()
        {
            try
            {
                CbClassCcuOnlineState selectedCbOnlineState = null;
                CbClassCcuOnlineState newSelectedCbDcuOnlineState = null;
                if (_cbOnlineStateFilter.SelectedItem is CbClassCcuOnlineState)
                {
                    selectedCbOnlineState = (CbClassCcuOnlineState)_cbOnlineStateFilter.SelectedItem;
                }

                _cbOnlineStateFilter.Items.Clear();
                _cbOnlineStateFilter.Items.Add(string.Empty);
                foreach (CCUOnlineState ccuOnlineState in Enum.GetValues(typeof(CCUOnlineState)))
                {
                    if (ccuOnlineState == CCUOnlineState.Online || ccuOnlineState == CCUOnlineState.Offline)
                    {
                        var cbOnlineState =
                            new CbClassCcuOnlineState(
                                GetString(ccuOnlineState.ToString()), 
                                ccuOnlineState);

                        _cbOnlineStateFilter.Items.Add(cbOnlineState);
                        if (selectedCbOnlineState != null && selectedCbOnlineState.CcuOnlineState == cbOnlineState.CcuOnlineState)
                        {
                            newSelectedCbDcuOnlineState = cbOnlineState;
                        }
                    }
                }

                if (newSelectedCbDcuOnlineState != null)
                {
                    _cbOnlineStateFilter.SelectedItem = newSelectedCbDcuOnlineState;
                }
            }
            catch { }
        }

        private void _cbOnlineStateFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeVisibleByOnlineState();
        }


        private void ShowHideRowByCCUOnlieState(int rowIndex, CCUOnlineState onlineState)
        {
            var item = _cbOnlineStateFilter.SelectedItem as CbClassCcuOnlineState;

            if (item == null)
                return;

            CCUOnlineState cbSelectedOnlineState = item.CcuOnlineState;

            DataGridViewCell dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            FastShowHideRowByCCUOnlineState(rowIndex, onlineState, cbSelectedOnlineState);

            if (dgcell != null && dgcell.Visible)
            {
                _cdgvData.DataGrid.CurrentCell = dgcell;
            }
        }

        private void FastShowHideRowByCCUOnlineState(int rowIndex, CCUOnlineState onlineState, CCUOnlineState cbSelectedOnlineState)
        {
            if (onlineState == cbSelectedOnlineState)
            {
                if (!_cdgvData.DataGrid.Rows[rowIndex].Visible) _cdgvData.DataGrid.Rows[rowIndex].Visible = true;
            }
            else
            {
                if (_cdgvData.DataGrid.Rows[rowIndex].Visible) _cdgvData.DataGrid.Rows[rowIndex].Visible = false;
            }
        }

        private void ChangeVisibleByOnlineState()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ChangeVisibleByOnlineState));
            }
            else
            {
                if (_cbOnlineStateFilter.SelectedItem is CbClassCcuOnlineState)
                {
                    CCUOnlineState actOnlineState = (_cbOnlineStateFilter.SelectedItem as CbClassCcuOnlineState).CcuOnlineState;
                    DataGridViewCell dgcell = _cdgvData.DataGrid.CurrentCell;
                    _cdgvData.DataGrid.CurrentCell = null;
                    foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                    {
                        CCUShort ccu = (CCUShort)((BindingSource)_cdgvData.DataGrid.DataSource).List[row.Index];
                        if (ccu != null && ccu.OnlineState == actOnlineState)
                        {
                            if (!_cdgvData.DataGrid.Rows[row.Index].Visible)
                                _cdgvData.DataGrid.Rows[row.Index].Visible = true;
                        }
                        else
                        {
                            if (_cdgvData.DataGrid.Rows[row.Index].Visible)
                                _cdgvData.DataGrid.Rows[row.Index].Visible = false;
                        }
                    }
                    if (dgcell != null && dgcell.Visible)
                    {
                        _cdgvData.DataGrid.CurrentCell = dgcell;
                    }
                }
                else
                {
                    foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                    {
                        if (!_cdgvData.DataGrid.Rows[row.Index].Visible)
                            _cdgvData.DataGrid.Rows[row.Index].Visible = true;
                    }
                }
            }
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
        }
    }

    public class CbClassCcuOnlineState
    {
        string _name;
        CCUOnlineState _ccuOnlineState;
        public CCUOnlineState CcuOnlineState { get { return _ccuOnlineState; } }

        public CbClassCcuOnlineState(string name, CCUOnlineState onlineState)
        {
            _name = name;
            _ccuOnlineState = onlineState;
            
        }

        public override string ToString()
        {
            return _name;
        }
    }
}

