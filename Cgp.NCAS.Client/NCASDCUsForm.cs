using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.Cgp.Server.Beans;

using System.Collections;
using Contal.Cgp.NCAS.Definitions;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASDCUsForm :
#if DESIGNER
        Form
#else
 ACgpPluginTableForm<NCASClient, DCU, DCUShort>
#endif
    {
        private DVoid2Void _eventCreatedDCU;
        private Action<Guid, byte> _eventDoorEnvironmentStateChanged;
        private Action<Guid, OnlineState, Guid> _eventDCUOnlineStateChanged;

        public NCASDCUsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.dcu_48;
            InitializeComponent();
            FormOnEnter += FormEnterHF;
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.AfterGridModified += _cdgvData_AfterGridModified;
            _cdgvData.AfterSort += AfterDataChanged;
            _cdgvData.CgpDataGridEvents = this;
            _cdgvData.EnabledInsertButton = false;
            _cdgvData.EnabledDeleteButton = false;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (DCUShort dcuShort in bindingSource.List)
            {
                dcuShort.Symbol = _cdgvData.GetSubTypeImage(dcuShort, dcuShort.OnlineState);
                dcuShort.StringOnlineState = GetTranslatedDCUState(dcuShort.OnlineState);
                dcuShort.StringDoorEnvironmentState = GetTranslatedDoorEnvironmentState(dcuShort.StateDoorEnvironment, LocalizationHelper);
            }
        }

        void _cdgvData_AfterGridModified(BindingSource bindingSource)
        {
            AfterDataChanged(bindingSource.List);
        }

        private void AfterDataChanged(IList dataSourceList)
        {
            OnlineState cbSelectedOnlineState;
            DataGridViewCell dgcell;

            if (_cbDcuOnlineState.SelectedItem is CbOnlineState)
            {
                cbSelectedOnlineState = ((CbOnlineState)_cbDcuOnlineState.SelectedItem).OnlineState;
                dgcell = _cdgvData.DataGrid.CurrentCell;
                _cdgvData.DataGrid.CurrentCell = null;
            }
            else return;

            int i = 0;
            if (dataSourceList != null)
                foreach (DCUShort dcuShort in dataSourceList)
                {
                    FastShowHideRowByDCUOnlieState(i, dcuShort.OnlineState, cbSelectedOnlineState);
                    i++;
                }

            if (dgcell != null && dgcell.Visible) _cdgvData.DataGrid.CurrentCell = dgcell;
        }

        private void FormEnterHF(Form form)
        {
            FillCbLocalizeName();
        }

        private static volatile NCASDCUsForm _singleton;
        private static object _syncRoot = new object();

        public static NCASDCUsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASDCUsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        #region Override ACgpPluginTableForm
        protected override DCU GetFromShort(DCUShort listObj)
        {
            return Plugin.MainServerProvider.DCUs.GetObjectById(listObj.IdDCU);
        }

        protected override ICollection<DCUShort> GetData()
        {
            Exception error = null;
            ICollection<DCUShort> list = Plugin.MainServerProvider.DCUs.SvDcuSelectByCriteria(FilterSettings, out error);

            if (error != null)
                throw (error);
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

        protected override bool Compare(DCU obj1, DCU obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(DCU obj, Guid idObj)
        {
            return obj.IdDCU == idObj;
        }

        protected override void DeleteObj(DCU obj)
        {
        }

        protected override ACgpPluginEditForm<NCASClient, DCU> CreateEditForm(DCU obj, ShowOptionsEditForm showOption)
        {
            return new NCASDCUEditForm(obj, showOption, this);
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != "")
            {
                FilterSettings filterSetting = new FilterSettings(DCU.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }
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
                    return Plugin.MainServerProvider.DCUs.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(DCU dcu)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.DCUs.HasAccessViewForObject(dcu);
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
                    return Plugin.MainServerProvider.DCUs.HasAccessInsert();
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
                    return Plugin.MainServerProvider.DCUs.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        protected override DCU GetObjectForEdit(DCUShort listObj, out bool editAllowed)
        {
            return Plugin.MainServerProvider.DCUs.GetObjectForEditById(listObj.IdDCU, out editAllowed);
        }

        #endregion

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, CardShort.COLUMN_SYMBOL, DCUShort.COLUMN_FULL_NAME, DCUShort.COLUMN_STRING_STATE,
                   DCUShort.COLUMN_STRING_DOOR_ENVIRONMENT_STATE, DCUShort.COLUMN_DESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        #region EventsFromServer
        protected override void RegisterEvents()
        {
            if (_eventCreatedDCU == null)
            {
                _eventCreatedDCU = CreatedDCU;
                CreatedDCUHandler.Singleton.RegisterCreatedDCUEvent(_eventCreatedDCU);
            }

            if (_eventDoorEnvironmentStateChanged == null)
            {
                _eventDoorEnvironmentStateChanged = DoorEnvironmentStateChanged;
                DoorEnvironmentStateChangedHandler.Singleton.RegisterStateChanged(_eventDoorEnvironmentStateChanged);
            }

            if (_eventDCUOnlineStateChanged == null)
            {
                _eventDCUOnlineStateChanged = DCUOnlineStateChanged;
                DCUOnlineStateChangedHandler.Singleton.RegisterStateChanged(_eventDCUOnlineStateChanged);
            }
        }

        private void DCUOnlineStateChanged(Guid dcuGuid, OnlineState state, Guid parentGuid)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, OnlineState, Guid>(DCUOnlineStateChanged), dcuGuid, state, parentGuid);
            }
            else
            {
                if (_cdgvData.DataGrid.DataSource == null) return;

                int i = 0;
                foreach (DCUShort dcuShort in (_cdgvData.DataGrid.DataSource as BindingSource).List)
                {
                    if (dcuShort != null && dcuShort.IdDCU == dcuGuid)
                    {
                        dcuShort.OnlineState = state;
                        _cdgvData.UpdateValue(i, DCUShort.COLUMN_STRING_STATE, GetTranslatedDCUState(state));
                        _cdgvData.UpdateImage(dcuShort, i, dcuShort.OnlineState);
                        ShowHideRowByDCUOnlieState(i, state);
                        return;
                    }
                    i++;
                }
            }
        }

        private string GetTranslatedDCUState(OnlineState state)
        {
            switch (state)
            {
                case OnlineState.Online: return GetString("Online");
                case OnlineState.Offline: return GetString("Offline");
                case OnlineState.Upgrading: return GetString("Upgrading");
                case OnlineState.WaitingForUpgrade: return GetString("WaitingForUpgrade");
                case OnlineState.AutoUpgrading: return GetString("AutoUpgrading");
                default: return GetString("Unknown");
            }
        }

        private void ShowHideRowByDCUOnlieState(int rowIndex, OnlineState onlineState)
        {
            var item = _cbDcuOnlineState.SelectedItem as CbOnlineState;

            if (item == null)
                return;

            OnlineState cbSelectedOnlineState = item.OnlineState;

            DataGridViewCell dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            FastShowHideRowByDCUOnlieState(rowIndex, onlineState, cbSelectedOnlineState);

            if (dgcell != null && dgcell.Visible)
            {
                _cdgvData.DataGrid.CurrentCell = dgcell;
            }
        }

        private void FastShowHideRowByDCUOnlieState(int rowIndex, OnlineState onlineState, OnlineState cbSelectedOnlineState)
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

        private void DoorEnvironmentStateChanged(Guid doorEnvironmentGuid, byte state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoorEnvironmentStateChanged), doorEnvironmentGuid, state);
            }
            else
            {
                int i = 0;
                foreach (DCUShort dcuShort in (_cdgvData.DataGrid.DataSource as BindingSource).List)
                {
                    if (dcuShort.IdDoorEnvironment == doorEnvironmentGuid)
                    {
                        dcuShort.StateDoorEnvironment = (DoorEnvironmentState)state;
                        _cdgvData.UpdateValue(i, DCUShort.COLUMN_STRING_DOOR_ENVIRONMENT_STATE, GetTranslatedDoorEnvironmentState((DoorEnvironmentState)state, LocalizationHelper));
                        return;
                    }
                    i++;
                }
            }
        }
        #endregion

        public static string GetTranslatedDoorEnvironmentState(DoorEnvironmentState doorEnvironmentState, LocalizationHelper localizationHelper)
        {
            switch (doorEnvironmentState)
            {
                case DoorEnvironmentState.Locked: return localizationHelper.GetString("DoorEnvironmentStateLocked");
                case DoorEnvironmentState.Locking: return localizationHelper.GetString("DoorEnvironmentStateLocking");
                case DoorEnvironmentState.Opened: return localizationHelper.GetString("DoorEnvironmentStateOpened");
                case DoorEnvironmentState.Unlocked: return localizationHelper.GetString("DoorEnvironmentStateUnlocked");
                case DoorEnvironmentState.Unlocking: return localizationHelper.GetString("DoorEnvironmentStateUnlocking");
                case DoorEnvironmentState.AjarPrewarning: return localizationHelper.GetString("DoorEnvironmentStateDoorAjarPrewarning");
                case DoorEnvironmentState.Ajar: return localizationHelper.GetString("DoorEnvironmentStateDoorAjar");
                case DoorEnvironmentState.Intrusion: return localizationHelper.GetString("DoorEnvironmentStateIntrusion");
                case DoorEnvironmentState.Sabotage: return localizationHelper.GetString("DoorEnvironmentStateSatotage");
                default: return localizationHelper.GetString("Unknown");
            }
        }

        private void CreatedDCU()
        {
            ShowData();
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_cdgvData.DataGrid.SelectedRows.Count == 1)
            {
                Edit_Click();
            }
            else
            {
                DataGridViewSelectedRowCollection selected = _cdgvData.DataGrid.SelectedRows;
                for (int i = 0; i < selected.Count; i++)
                {
                    EditFromPosition(selected[i].Index);
                }
            }
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
            if (_cbDcuOnlineState.Items != null && _cbDcuOnlineState.Items.Count > 1)
            {
                _cbDcuOnlineState.SelectedIndex = 0;
            }
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
        }

        protected override void FilterKeyDown(object sender, KeyEventArgs e)
        {
            base.FilterKeyDown(sender, e);
        }

        #region DeleteDCU
        public bool DeleteAllowedNotEdited(DCU dcu)
        {
            if (IsEditedObject(dcu))
            {
                if (Dialog.Question(GetString("QuestionDCUOpenedCloseBeforeDelete")))
                {
                    CloseDCUandReletedObjects(dcu);
                    return true;
                }
                return false;
            }
            bool isEditedRelatedObject = false;
            DCU dcuReObj = Plugin.MainServerProvider.DCUs.GetObjectById(dcu.IdDCU);
            if (dcuReObj == null) return true;

            foreach (Input input in dcuReObj.Inputs)
            {
                if (NCASInputsForm.Singleton.IsEditedObject(input))
                {
                    isEditedRelatedObject = true;
                    break;
                }
            }

            if (!isEditedRelatedObject)
            {
                foreach (Output output in dcuReObj.Outputs)
                {
                    if (NCASOutputsForm.Singleton.IsEditedObject(output))
                    {
                        isEditedRelatedObject = true;
                        break;
                    }
                }
            }

            if (!isEditedRelatedObject)
            {
                foreach (CardReader cardReader in dcuReObj.CardReaders)
                {
                    if (NCASCardReadersForm.Singleton.IsEditedObject(cardReader))
                    {
                        isEditedRelatedObject = true;
                        break;
                    }
                }
            }

            if (!isEditedRelatedObject)
                return true;

            if (Dialog.Question(GetString("QuestionDCUObjectsOpenedCloseBeforeDelete")))
            {
                CloseDCUandReletedObjects(dcu);
                return true;
            }
            return false;
        }

        public void CloseDCUandReletedObjects(DCU dcu)
        {
            DCU dcuReObj = Plugin.MainServerProvider.DCUs.GetObjectById(dcu.IdDCU);

            if (IsEditedObject(dcuReObj))
            {
                CloseEditedForm(dcuReObj);
            }
            if (dcuReObj == null) return;

            foreach (Input input in dcuReObj.Inputs)
            {
                NCASInputsForm.Singleton.CloseEditedForm(input);
            }

            foreach (Output output in dcuReObj.Outputs)
            {
                NCASOutputsForm.Singleton.CloseEditedForm(output);
            }

            foreach (CardReader cardReader in dcuReObj.CardReaders)
            {
                NCASCardReadersForm.Singleton.CloseEditedForm(cardReader);
            }
        }
        #endregion


        private void FillCbLocalizeName()
        {
            try
            {
                CbOnlineState selectedCbDcuOnlineState = null;
                CbOnlineState newSelectedCbDcuOnlineState = null;
                var item = _cbDcuOnlineState.SelectedItem as CbOnlineState;
                if (item != null)
                {
                    selectedCbDcuOnlineState = item;
                }

                _cbDcuOnlineState.Items.Clear();
                _cbDcuOnlineState.Items.Add(string.Empty);
                foreach (OnlineState dcuOnlineState in Enum.GetValues(typeof(OnlineState)))
                {
                    CbOnlineState cbOnlineState = new CbOnlineState(dcuOnlineState, GetString(dcuOnlineState.ToString()));
                    _cbDcuOnlineState.Items.Add(cbOnlineState);
                    if (selectedCbDcuOnlineState != null && selectedCbDcuOnlineState.OnlineState == cbOnlineState.OnlineState)
                    {
                        newSelectedCbDcuOnlineState = cbOnlineState;
                    }
                }

                if (newSelectedCbDcuOnlineState != null)
                {
                    _cbDcuOnlineState.SelectedItem = newSelectedCbDcuOnlineState;
                }
            }
            catch { }
        }

        private void _cbDcuOnlineState_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeVisibleByDCUOnlineState();
        }

        private void ChangeVisibleByDCUOnlineState()
        {
            if (_cbDcuOnlineState.SelectedItem is CbOnlineState)
            {
                OnlineState onlineState = ((CbOnlineState)_cbDcuOnlineState.SelectedItem).OnlineState;
                DataGridViewCell dgcell = _cdgvData.DataGrid.CurrentCell;
                _cdgvData.DataGrid.CurrentCell = null;
                foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                {
                    DCUShort dcu = (DCUShort)((BindingSource)_cdgvData.DataGrid.DataSource).List[row.Index];
                    if (dcu != null && dcu.OnlineState == onlineState)
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

        protected override void UnregisterEvents()
        {
            if (_eventCreatedDCU != null)
            {
                CreatedDCUHandler.Singleton.UnregisterCreatedDCUEvent(_eventCreatedDCU);
                _eventCreatedDCU = null;
            }
            if (_eventDoorEnvironmentStateChanged != null)
            {
                DoorEnvironmentStateChangedHandler.Singleton.UnregisterStateChanged(_eventDoorEnvironmentStateChanged);
                _eventDoorEnvironmentStateChanged = null;
            }
            if (_eventDCUOnlineStateChanged != null)
            {
                DCUOnlineStateChangedHandler.Singleton.UnregisterStateChanged(_eventDCUOnlineStateChanged);
                _eventDCUOnlineStateChanged = null;
            }
        }
    }

    public class CbOnlineState
    {
        private OnlineState _onlineState;
        private string _localizeString;

        public OnlineState OnlineState { get { return _onlineState; } }

        public override string ToString()
        {
            return _localizeString;
        }

        public CbOnlineState(OnlineState onlineState, string localizeName)
        {
            _onlineState = onlineState;
            _localizeString = localizeName;
        }
    }
}
