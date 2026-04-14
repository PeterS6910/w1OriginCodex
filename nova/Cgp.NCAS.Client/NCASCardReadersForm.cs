using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.Cgp.Globals;

using System.Collections;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASCardReadersForm :
#if DESIGNER
        Form
#else
 ACgpPluginTableForm<NCASClient, CardReader, CardReaderShort>
#endif
    {
        private Action<Guid, byte, Guid> _eventInOutStateChanged;
        private Action<Guid, byte> _eventTzDpStateChanged;
        private Action<Guid, byte> _eventSecurityTimeZoneStateChanged;

        private Action<Guid, byte, Guid> _eventCardReaderOnlineStateChanged;
        private Action<Guid, byte> _eventCardReaderCommandChanged;

        private Action<Guid, bool> _eventBlockedStateChanged;

        private readonly ICollection<FilterSettings>[] _filterSettingsWithJoin;
        private LogicalOperators _filterSettingsJoinOperator = LogicalOperators.AND;

        private readonly SyncDictionary<Guid, State> _onOffObjectsToState =
            new SyncDictionary<Guid, State>();

        private readonly SyncDictionary<Guid, ShortObjectExtension<CardReaderShort, Guid?, SecurityLevel?, SecurityLevel?>> _secTzDpToExtCardReaderShort =
            new SyncDictionary<Guid, ShortObjectExtension<CardReaderShort, Guid?, SecurityLevel?, SecurityLevel?>>();

        private readonly SyncDictionary<Guid, ShortObjectExtension<CardReaderShort, Guid?, SecurityLevel?, SecurityLevel?>> _extCardReaderShort =
            new SyncDictionary<Guid, ShortObjectExtension<CardReaderShort, Guid?, SecurityLevel?, SecurityLevel?>>();

        private OnlineState? _currentOnlineStateFilter;

        private readonly ICollection<SecurityLevel?> _currentSecuritlyLevelFilter =
            new List<SecurityLevel?>();

        private bool? _currentForcedSecurityLevel;

        public NCASCardReadersForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.Cardreader48;
            InitializeComponent();
            _filterSettingsWithJoin = new[] { FilterSettings, new List<FilterSettings>(), new List<FilterSettings>(), new List<FilterSettings>() };
            InitCGPDataGridView();
            SetReferenceEditColors();
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
            _cdgvData.DataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private void DataGrid_SelectionChanged(object sender, EventArgs e)
        {
            _bUnblock.Enabled = false;

            var selectedRows = _cdgvData.SelectedRows;

            if (selectedRows == null)
                return;

            foreach (DataGridViewRow selectedRow in selectedRows)
            {
                var crShort = selectedRow.DataBoundItem as CardReaderShort;

                if (crShort == null
                    || !crShort.IsBlocked)
                {
                    continue;
                }

                _bUnblock.Enabled = true;
                break;
            }
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (CardReaderShort crShort in bindingSource.List)
            {
                crShort.Symbol = _cdgvData.GetSubTypeImage(crShort, crShort.OnlineState);
                crShort.StringOnlineState = GetTranslatedOnlineState(crShort);
                crShort.SecurityLevelDSM = GetTranslatedActualDSMState(crShort);
                crShort.ActualState = GetTranslatedActualCRState(crShort);
                crShort.BlockedState = crShort.IsBlocked
                    ? Cgp.Client.ResourceGlobal.IconNewBlocked20.ToBitmap()
                    : null;
            }
        }

        void _cdgvData_AfterGridModified(BindingSource bindingSource)
        {
            AfterDataChanged(bindingSource.List);
        }

        private void AfterDataChanged(IList dataSourceList)
        {
            RuntimeFilterValueChanged(dataSourceList, null);
            RunFilter();
        }

        protected override CardReader GetObjectForEdit(CardReaderShort listObj, out bool editAllowed)
        {
            return Plugin.MainServerProvider.CardReaders.GetObjectForEditById(listObj.IdCardReader, out editAllowed);
        }

        protected override CardReader GetFromShort(CardReaderShort listObj)
        {
            return Plugin.MainServerProvider.CardReaders.GetObjectById(listObj.IdCardReader);
        }

        private static volatile NCASCardReadersForm _singleton;
        private static readonly object _syncRoot = new object();

        public static NCASCardReadersForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASCardReadersForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        protected override ICollection<CardReaderShort> GetData()
        {
            Exception error;
            ICollection<CardReaderShort> resultList;

            var mainServerProvider = Plugin.MainServerProvider;

            if (_currentSecuritlyLevelFilter.Count > 0 || _currentForcedSecurityLevel != null)
            {
                resultList = new List<CardReaderShort>();

                var cardReaders =
                    mainServerProvider.CardReaders.SelectByCriteria(
                        out error,
                        _filterSettingsJoinOperator,
                        _filterSettingsWithJoin);

                _onOffObjectsToState.Clear(
                    () => GetDataBySecurityLevel(
                        cardReaders,
                        mainServerProvider,
                        resultList));
            }
            else
            {
                _onOffObjectsToState.Clear();
                _extCardReaderShort.Clear();

                resultList =
                    mainServerProvider.CardReaders.ShortSelectByCriteria(
                        out error,
                        _filterSettingsJoinOperator,
                        _filterSettingsWithJoin);
            }

            if (error != null)
                throw error;

            return resultList;
        }

        private void GetDataBySecurityLevel(
            IEnumerable<CardReader> cardReaders,
            ICgpNCASRemotingProvider mainServerProvider,
            ICollection<CardReaderShort> resultList)
        {
            foreach (var cardReader in cardReaders)
            {
                var crShort = new CardReaderShort(cardReader);

                SecurityLevel currentSecLevel =
                    GetCurrentSecLevel(
                        mainServerProvider,
                        cardReader);

                var onOffObjectId = cardReader.OnOffObjectId;

                var filterValues =
                    new ShortObjectExtension<CardReaderShort, Guid?, SecurityLevel?, SecurityLevel?>(
                        crShort,
                        cardReader.IsForcedSecurityLevel
                            ? onOffObjectId
                            : null,
                        currentSecLevel,
                        (SecurityLevel)cardReader.ForcedSecurityLevel);

                if (cardReader.SecurityDailyPlan != null)
                    _secTzDpToExtCardReaderShort[cardReader.SecurityDailyPlan.IdSecurityDailyPlan] =
                        filterValues;
                else
                    if (cardReader.SecurityTimeZone != null)
                        _secTzDpToExtCardReaderShort[cardReader.SecurityTimeZone.IdSecurityTimeZone] =
                            filterValues;

                if (cardReader.IsForcedSecurityLevel &&
                    onOffObjectId != null)
                {
                    var state =
                        mainServerProvider.GetRealOnOffObjectState(
                            cardReader.OnOffObject.GetObjectType(),
                            onOffObjectId.Value); //input.OnOffObject.State ? State.On : State.Off;

                    _onOffObjectsToState[onOffObjectId.Value] = state;
                }

                crShort.OnlineState =
                    mainServerProvider.CardReaders
                        .GetOnlineStates(cardReader.IdCardReader);

                crShort.UsedInDoorEnvironment =
                    mainServerProvider.CardReaders
                        .IsUsedInDoorEnvironment(cardReader);

                // If this reader is not used in door environment its current security level will be "Not used"
                if (!crShort.UsedInDoorEnvironment)
                {
                    filterValues.Value2 = null;
                    filterValues.Value3 = null;
                }

                crShort.CardReaderSceneType =
                    mainServerProvider.CardReaders
                        .GetCardReaderCommand(cardReader.IdCardReader);

                _extCardReaderShort[crShort.IdCardReader] = filterValues;

                resultList.Add(crShort);
            }
        }

        private static SecurityLevel GetCurrentSecLevel(
            ICgpNCASRemotingProvider mainServerProvider,
            CardReader cardReader)
        {
            var currentSecLevel = (SecurityLevel)cardReader.SecurityLevel;

            if (currentSecLevel != SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
                return currentSecLevel;

            SecurityLevel? secLevel;
            SecurityLevel4SLDP? secLevel4SlDP;

            mainServerProvider.CardReaders.GetActualSecurityLevel(
                cardReader.IdCardReader,
                out secLevel,
                out secLevel4SlDP);

            if (secLevel4SlDP == null)
                return currentSecLevel;

            secLevel = ConvertSecLevele4SLDP(secLevel4SlDP.Value);

            return
                secLevel != null
                    ? secLevel.Value
                    : currentSecLevel;
        }

        protected override bool Compare(CardReader obj1, CardReader obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(CardReader obj, Guid idObj)
        {
            return obj.IdCardReader == idObj;
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            var dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            _cdgvData.ModifyGridView(
                bindingSource,
                CardReaderShort.COLUMN_SYMBOL,
                CardReaderShort.COLUMN_FULL_NAME,
                CardReaderShort.COLUMN_STRING_ONLINE_STATE,
                CardReaderShort.COLUMN_SECURITY_LEVEL_DSM,
                CardReaderShort.COLUMN_ACTUAL_STATE,
                CardReaderShort.COLUMN_BLOCKED_STATE,
                CardReaderShort.COLUMN_DESCRIPTION);

            _cdgvData.DataGrid.ColumnHeadersHeight = 34;
            _cdgvData.DataGrid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            _cdgvData.DataGrid.Columns[CardReaderShort.COLUMN_BLOCKED_STATE]
                .DefaultCellStyle.NullValue = null;

            try
            {
                if (dgcell != null && dgcell.Visible)
                    _cdgvData.DataGrid.CurrentCell = dgcell;
            }
            catch
            {
            }

            SafeThread.StartThread(FillComboboxCcu);
            SafeThread.StartThread(FillComboboxDcu);
        }

        private string GetTranslatedOnlineState(CardReaderShort crShort)
        {
            return GetString(crShort.OnlineState.ToString());
        }

        private string GetTranslatedActualDSMState(CardReaderShort crShort)
        {
            return crShort.UsedInDoorEnvironment ? GetString("SecurityLevelStates_" + ((SecurityLevel)crShort.SecurityLevel))
                : new ItemSlForEnterToMenu((SecurityLevel)crShort.SLForEnterToMenu, LocalizationHelper).ToString();
        }

        private string GetTranslatedActualCRState(CardReaderShort crShort)
        {
            var crCommand = crShort.CardReaderSceneType;

            if (crShort.UsedInDoorEnvironment)
            {
                return GetString("CardReaderCommands_" + crCommand);
            }
            else
            {
                switch (crCommand)
                {
                    case CardReaderSceneType.OutOfOrder:
                    case CardReaderSceneType.Unknown:
                        return GetString("CardReaderCommands_" + crCommand);
                    default:
                        return GetString("CardReaderCommands_AlarmPanelState");
                }
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        protected override void DeleteObj(CardReader obj)
        {
            throw new NotImplementedException();
        }

        protected override ACgpPluginEditForm<NCASClient, CardReader> CreateEditForm(CardReader obj, ShowOptionsEditForm showOption)
        {
            return new NCASCardReaderEditForm(obj, showOption, this);
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_cdgvData.DataGrid.SelectedRows.Count == 1)
            {
                Edit_Click();
            }
            else
            {
                var selected = _cdgvData.DataGrid.SelectedRows;
                for (var i = 0; i < selected.Count; i++)
                {
                    EditFromPosition(selected[i].Index);
                }
            }
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            _filterSettingsJoinOperator = 
                _rbFilterAnd.Checked 
                    ? LogicalOperators.AND 
                    : LogicalOperators.OR;

            foreach (var list in _filterSettingsWithJoin)
                list.Clear();

            if (_eNameFilter.Text != "")
            {
                var filterSetting = new FilterSettings(CardReader.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }

            if (_cbForcedSecurityLevel.SelectedIndex > 0)
            {
                var value = _cbForcedSecurityLevel.SelectedIndex == 1;
                var filterSetting = new FilterSettings(CardReader.COLUMNISFORCEDSECURITYLEVEL, value, ComparerModes.EQUALL, _filterSettingsJoinOperator);
                FilterSettings.Add(filterSetting);
            }

            if (_cbEmergencyCode.SelectedIndex > 0)
            {
                var value = _cbEmergencyCode.SelectedIndex == 1;
                var filterSetting = new FilterSettings(CardReader.COLUMNISEMERGENCYCODE, value, ComparerModes.EQUALL, _filterSettingsJoinOperator);
                FilterSettings.Add(filterSetting);
            }

            var obj = _cbCcu.SelectedItem as CcuListObj;
            if (obj != null)
            {
                var ccuList = obj;
                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(ccuList.Id);
                if (ccu != null)
                {
                    var filterSetting = new FilterSettings(CardReader.COLUMNCCU, ccu, ComparerModes.EQUALL, LogicalOperators.OR);
                    _filterSettingsWithJoin[2].Add(filterSetting);

                    if (ccu.DCUs != null)
                    {
                        foreach (var dcu in ccu.DCUs)
                        {
                            var filterSettingDcu = new FilterSettings(CardReader.COLUMNDCU, dcu, ComparerModes.EQUALL, LogicalOperators.OR);
                            _filterSettingsWithJoin[2].Add(filterSettingDcu);
                        }
                    }
                }
            }

            var listObj = _cbDcu.SelectedItem as DcuListObj;
            if (listObj != null)
            {
                var dcuList = listObj;
                var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(dcuList.Id);
                var filterSetting = new FilterSettings(CardReader.COLUMNDCU, dcu, ComparerModes.EQUALL);
                FilterSettings.Add(filterSetting);
            }

            if (_tbmMemberOfAclFilter.Tag != null)
            {
                var list = _tbmMemberOfAclFilter.Tag as ListOfObjects;
                if (list != null)
                {
                    LinkedList<Guid> crGuids = new LinkedList<Guid>();

                    foreach (AOrmObject item in list)
                    {
                        if (item != null)
                        {
                            var acl = Plugin.MainServerProvider.AccessControlLists.GetObjectById(item.GetId());

                            foreach (var setting in acl.ACLSettings)
                            {
                                foreach (var guid in GetCardReaderGuids(setting))
                                {
                                    crGuids.AddLast(guid);
                                }
                            }
                        }
                    }

                    var filterSetting = new FilterSettings(CardReader.COLUMNIDCARDREADER, crGuids,
                                ComparerModes.IN);
                    _filterSettingsWithJoin[1].Add(filterSetting);
                }
            }

            foreach (var item in _clbSecurityLevelFilter.Items)
            {
                if (item != null)
                {
                    var value = (byte)_clbSecurityLevelFilter.Items.IndexOf(item);
                    if (_clbSecurityLevelFilter.GetItemCheckState(value) == CheckState.Checked)
                    {
                        if (value > 6)
                            value = 9;
                        var filterSetting = new FilterSettings(CardReader.COLUMNSECURITYLEVEL, value, ComparerModes.EQUALL, LogicalOperators.OR);
                        _filterSettingsWithJoin[3].Add(filterSetting);
                    }
                }
            }

            if (_cbOnlineStateFilter.SelectedIndex > 0)
            {
                _currentOnlineStateFilter = (OnlineState)(_cbOnlineStateFilter.SelectedIndex - 1);
            }
            else
                _currentOnlineStateFilter = null;

            if (_cbCurrentForcedSecurityLevel.SelectedIndex > 0)
            {
                _currentForcedSecurityLevel = _cbCurrentForcedSecurityLevel.SelectedIndex == 1;
            }
            else
                _currentForcedSecurityLevel = null;

            _currentSecuritlyLevelFilter.Clear();
            foreach (var item in _clbCurrentSecurityLevel.Items)
            {
                if (item != null)
                {
                    var value = (byte)_clbCurrentSecurityLevel.Items.IndexOf(item);
                    if (_clbCurrentSecurityLevel.GetItemCheckState(value) == CheckState.Checked)
                    {
                        if (value <= 6)
                            _currentSecuritlyLevelFilter.Add((SecurityLevel)value);
                        else
                            _currentSecuritlyLevelFilter.Add(null);
                    }
                }
            }
        }

        private IEnumerable<Guid> GetCardReaderGuids(ACLSetting setting)
        {
            var list = new List<Guid>();

            if (setting == null)
                return list.ToArray();

            switch ((ObjectType)setting.CardReaderObjectType)
            {
                case ObjectType.CardReader:
                    list.Add(setting.GuidCardReaderObject);
                    break;

                case ObjectType.AlarmArea:
                    var aa = Plugin.MainServerProvider.AlarmAreas.GetObjectById(setting.GuidCardReaderObject);
                    if (aa.AACardReaders != null)
                    {
                        foreach (var cr in aa.AACardReaders)
                            list.Add(cr.CardReader.IdCardReader);
                    }
                    break;

                case ObjectType.DoorEnvironment:
                    var de = Plugin.MainServerProvider.DoorEnvironments.GetObjectById(setting.GuidCardReaderObject);
                    if (de == null)
                        break;

                    if (de.CardReaderExternal != null)
                        list.Add(de.CardReaderExternal.IdCardReader);

                    if (de.CardReaderInternal != null)
                        list.Add(de.CardReaderInternal.IdCardReader);
                    break;

                case ObjectType.DCU:
                    var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(setting.GuidCardReaderObject);
                    if (dcu == null)
                        break;

                    if (dcu.GuidCardReaders != null)
                    {
                        foreach (var cr in dcu.CardReaders)
                            list.Add(cr.IdCardReader);
                    }
                    break;
            }

            return list.ToArray();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _filterSettingsJoinOperator = LogicalOperators.AND;
            _rbFilterAnd.Checked = true;

            _currentOnlineStateFilter = null;
            _currentSecuritlyLevelFilter.Clear();
            _currentForcedSecurityLevel = null;

            foreach (var filter in _filterSettingsWithJoin)
                filter.Clear();

            _eNameFilter.Text = "";
            _cbCcu.SelectedItem = string.Empty;
            _cbDcu.SelectedItem = string.Empty;

            _cbForcedSecurityLevel.SelectedIndex = 0;
            _cbEmergencyCode.SelectedIndex = 0;
            for (var i = 0; i < _clbSecurityLevelFilter.Items.Count; i++)
            {
                _clbSecurityLevelFilter.SetItemChecked(i, false);
            }

            _cbCurrentForcedSecurityLevel.SelectedIndex = 0;
            _cbOnlineStateFilter.SelectedIndex = 0;
            for (var i = 0; i < _clbCurrentSecurityLevel.Items.Count; i++)
            {
                _clbCurrentSecurityLevel.SetItemChecked(i, false);
            }

            SetSelectedObjects(_tbmMemberOfAclFilter, null);

            _chbBlocked.Checked = false;
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn
                        && Plugin != null
                        && Plugin.MainServerProvider != null
                        && Plugin.MainServerProvider.CardReaders != null)
                    return Plugin.MainServerProvider.CardReaders.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(CardReader cardReader)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn
                        && Plugin != null
                        && Plugin.MainServerProvider != null
                        && Plugin.MainServerProvider.CardReaders != null)
                    return
                        Plugin.MainServerProvider.CardReaders
                            .HasAccessViewForObject(cardReader);
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
                    return
                        Plugin.MainServerProvider.CardReaders
                            .HasAccessInsert();
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
                    return
                        Plugin.MainServerProvider.CardReaders
                            .HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        #region Events from server

        private void ChangeCardReaderOnlineState(Guid crGuid, byte state, Guid parentGuid)
        {
            SafeThread<Guid, byte>.StartThread(DoChangeCardReaderOnlineState, crGuid, state);
        }

        private void DoChangeCardReaderOnlineState(Guid crGuid, byte state)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoChangeCardReaderOnlineState), crGuid, state);
            }
            else
            {
                try
                {
                    if (!IsRunnedFormEnter) return;

                    CardReaderOnlineStateChanged(crGuid, (OnlineState)state);
                }
                catch { }
            }
        }

        private void CardReaderOnlineStateChanged(Guid crGuid, OnlineState onlineState)
        {
            var i = 0;

            foreach (CardReaderShort crShort in BindingSource.List)
            {
                if (crShort.IdCardReader == crGuid)
                {
                    if (crShort.OnlineState != onlineState)
                    {
                        RuntimeFilterValueChanged(crGuid, null);
                        crShort.OnlineState = onlineState;
                        _cdgvData.UpdateValue(i, CardReaderShort.COLUMN_STRING_ONLINE_STATE, (GetString(onlineState.ToString())));
                        _cdgvData.UpdateImage(crShort, i, crShort.OnlineState);
                    }
                    break;
                }
                i++;
            }

            RunFilter();
        }

        protected override void RegisterEvents()
        {
            if (_eventCardReaderOnlineStateChanged == null)
            {
                _eventCardReaderOnlineStateChanged = ChangeCardReaderOnlineState;
                StateChangedCardReaderHandler.Singleton.RegisterStateChanged(_eventCardReaderOnlineStateChanged);
            }

            if (_eventCardReaderCommandChanged == null)
            {
                _eventCardReaderCommandChanged = ChangeCardReaderCommand;
                CommandChangedCardReaderHandler.Singleton.RegisterCommandChanged(_eventCardReaderCommandChanged);
            }

            if (_eventInOutStateChanged == null)
            {
                _eventInOutStateChanged = ChangeInOutState;
                StateChangedInputHandler.Singleton.RegisterStateChanged(_eventInOutStateChanged);
                StateChangedOutputHandler.Singleton.RegisterStateChanged(_eventInOutStateChanged);
            }

            if (_eventTzDpStateChanged == null)
            {
                _eventTzDpStateChanged = ChangeTzDpState;
                StatusChangedTimeZoneHandler.Singleton.RegisterStatusChanged(_eventTzDpStateChanged);
                StatusChangedDailyPlainHandler.Singleton.RegisterStatusChanged(_eventTzDpStateChanged);
            }

            if (_eventSecurityTimeZoneStateChanged == null)
            {
                _eventSecurityTimeZoneStateChanged = ChangeSecurityTimeZoneState;
                StatusChangedSecurityTimeZoneHandler.Singleton.RegisterStatusChanged(_eventSecurityTimeZoneStateChanged);
                StatusChangedSecurityDailyPlanHandler.Singleton.RegisterStatusChanged(_eventSecurityTimeZoneStateChanged);
            }

            if (_eventBlockedStateChanged == null)
            {
                _eventBlockedStateChanged = ChangeBlockedState;
                BlockedStateChangedCardReaderHandler.Singleton.RegisterBlockedStateChanged(_eventBlockedStateChanged);
            }
        }

        protected override void UnregisterEvents()
        {
            if (_eventCardReaderOnlineStateChanged != null)
            {
                StateChangedCardReaderHandler.Singleton.UnregisterStateChanged(_eventCardReaderOnlineStateChanged);
                _eventCardReaderOnlineStateChanged = null;
            }

            if (_eventCardReaderCommandChanged != null)
            {
                CommandChangedCardReaderHandler.Singleton.UnregisterCommandChanged(_eventCardReaderCommandChanged);
                _eventCardReaderCommandChanged = null;
            }

            if (_eventInOutStateChanged != null)
            {
                StateChangedInputHandler.Singleton.UnregisterStateChanged(_eventInOutStateChanged);
                StateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventInOutStateChanged);
                _eventInOutStateChanged = null;
            }

            if (_eventTzDpStateChanged != null)
            {
                StatusChangedTimeZoneHandler.Singleton.UnregisterStatusChanged(_eventTzDpStateChanged);
                StatusChangedDailyPlainHandler.Singleton.UnregisterStatusChanged(_eventTzDpStateChanged);
                _eventTzDpStateChanged = null;
            }

            if (_eventSecurityTimeZoneStateChanged != null)
            {
                StatusChangedSecurityTimeZoneHandler.Singleton.UnregisterStatusChanged(_eventSecurityTimeZoneStateChanged);
                StatusChangedSecurityDailyPlanHandler.Singleton.UnregisterStatusChanged(_eventSecurityTimeZoneStateChanged);
                _eventSecurityTimeZoneStateChanged = null;
            }

            if (_eventBlockedStateChanged != null)
            {
                BlockedStateChangedCardReaderHandler.Singleton.UnregisterBlockedStateChanged(_eventBlockedStateChanged);
                _eventBlockedStateChanged = null;
            }
        }

        private void ChangeCardReaderCommand(Guid crGuid, byte command)
        {
            SafeThread<Guid, byte>.StartThread(DoChangeCardReaderCommand, crGuid, command);
        }

        private void DoChangeCardReaderCommand(Guid crGuid, byte command)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoChangeCardReaderCommand), crGuid, command);
            }
            else
            {
                if (!IsRunnedFormEnter) return;

                try
                {
                    if (CgpClient.Singleton.IsConnectionLost(false)) return;
                    CardReaderCommandChanged(crGuid, (CardReaderSceneType)command);
                }
                catch { }
            }
        }

        private void CardReaderCommandChanged(Guid crGuid, CardReaderSceneType command)
        {
            var i = 0;
            foreach (CardReaderShort crShort in BindingSource.List)
            {
                if (crShort.IdCardReader == crGuid)
                {
                    if (crShort.CardReaderSceneType != command)
                    {
                        crShort.CardReaderSceneType = command;
                        _cdgvData.UpdateValue(i, CardReaderShort.COLUMN_ACTUAL_STATE, GetString("CardReaderCommands_" + command.ToString()));
                        RuntimeFilterValueChanged(crShort, null);
                    }
                    break;
                }
                i++;
            }
            RunFilter();
        }

        private void ChangeBlockedState(Guid crGuid, bool isBlocked)
        {
            SafeThread<Guid, bool>.StartThread(DoChangeBlockedState, crGuid, isBlocked);
        }

        private void DoChangeBlockedState(Guid crGuid, bool isBlocked)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, bool>(DoChangeBlockedState), crGuid, isBlocked);
            }
            else
            {
                if (!IsRunnedFormEnter) return;

                try
                {
                    if (CgpClient.Singleton.IsConnectionLost(false)) return;
                    BlockedStateChanged(crGuid, isBlocked);
                }
                catch { }
            }
        }

        private void BlockedStateChanged(Guid crGuid, bool isBlocked)
        {
            var i = 0;
            foreach (CardReaderShort crShort in BindingSource.List)
            {
                if (crShort.IdCardReader == crGuid)
                {
                    if (crShort.IsBlocked != isBlocked)
                    {
                        crShort.IsBlocked = isBlocked;

                        _cdgvData.UpdateValue(
                            i,
                            CardReaderShort.COLUMN_BLOCKED_STATE,
                            isBlocked
                                ? Cgp.Client.ResourceGlobal.IconNewBlocked20.ToBitmap()
                                : null);

                        RuntimeFilterValueChanged(crShort, null);
                    }
                    break;
                }
                i++;
            }

            RunFilter();
        }

        public bool DeleteAllowedNotEdited(CardReader cardReader)
        {
            if (!IsEditedObject(cardReader))
                return true;

            if (!Dialog.Question(GetString("QuestionCardReaderOpenedCloseBeforeDelete")))
                return false;

            CloseEditedForm(cardReader);
            return true;
        }

        private void ChangeInOutState(Guid inputGuid, byte state, Guid parent)
        {
            UpdateRuntimeFilter(ObjectType.Input, inputGuid, state);
        }

        private void ChangeTzDpState(Guid inputGuid, byte state)
        {
            UpdateRuntimeFilter(ObjectType.TimeZone, inputGuid, state);
        }

        private void ChangeSecurityTimeZoneState(Guid tzGuid, byte state)
        {
            var level = ConvertSecLevele4SLDP((SecurityLevel4SLDP)state);

            if (level == null)
                return;

            ShortObjectExtension<CardReaderShort, Guid?, SecurityLevel?, SecurityLevel?> crShortExt;

            if (!_secTzDpToExtCardReaderShort.TryGetValue(tzGuid, out crShortExt))
                return;

            if (crShortExt.Value2 != null
                && crShortExt.Value2 != level.Value)
            {
                crShortExt.Value2 = level.Value;
                RuntimeFilterValueChanged(null, null);

                RunFilter();
            }
        }

        #endregion

        private void UpdateRuntimeFilter(ObjectType objectType, Guid objectId, byte state)
        {
            SafeThread<ObjectType, Guid, byte>.StartThread(
                DoUpdateRuntimeFilter,
                objectType,
                objectId,
                state);
        }

        private void DoUpdateRuntimeFilter(ObjectType objectType, Guid objectId, byte state)
        {
            State objectState;
            switch (state)
            {
                case 0:
                    objectState = State.Off;
                    break;

                case 1:
                    objectState = State.On;
                    break;

                default:
                    objectState = State.Unknown;
                    break;
            }

            var updated = false;

            _onOffObjectsToState.TryGetValue(objectId,
                (key, found, value) =>
                {
                    if ((found && value != objectState)
                        || !found)
                    {
                        updated = true;
                        RuntimeFilterValueChanged(key, null);
                    }

                    _onOffObjectsToState[key] = objectState;
                });

            if (updated)
                RunFilter();
        }

        private static SecurityLevel? ConvertSecLevele4SLDP(SecurityLevel4SLDP level)
        {
            switch (level)
            {
                case SecurityLevel4SLDP.unlocked:
                    return SecurityLevel.Unlocked;

                case SecurityLevel4SLDP.locked:
                    return SecurityLevel.Locked;

                case SecurityLevel4SLDP.card:
                    return SecurityLevel.CARD;

                case SecurityLevel4SLDP.cardpin:
                    return SecurityLevel.CARDPIN;

                case SecurityLevel4SLDP.code:
                    return SecurityLevel.CODE;

                case SecurityLevel4SLDP.codeorcard:
                    return SecurityLevel.CODEORCARD;

                case SecurityLevel4SLDP.codeorcardpin:
                    return SecurityLevel.CODEORCARDPIN;

                default:
                    return null;
            }
        }

        private IList<CcuListObj> _listCcu;

        private void FillComboboxCcu()
        {
            Exception error;
            var listCcu = Plugin.MainServerProvider.CCUs.GetListObj(out error);

            if (_listCcu == null)
            {
                ShowComboboxCcu(listCcu);
            }
            else if (IsNewCcus(listCcu))
            {
                ShowComboboxCcu(listCcu);
            }
        }

        private bool IsNewCcus(IEnumerable<CcuListObj> listCcus)
        {
            return 
                _listCcu == null || 
                listCcus.Any(
                    obj => _listCcu.All(
                        oldCcuObj => oldCcuObj.Id != obj.Id));
        }

        private void ShowComboboxCcu(IList<CcuListObj> listCcu)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<IList<CcuListObj>>(ShowComboboxCcu), 
                    listCcu);

                return;
            }

            try
            {
                _listCcu = listCcu;
                _cbCcu.BeginUpdate();

                var selected = _cbCcu.SelectedItem as CcuListObj;

                _cbCcu.Items.Clear();
                _cbCcu.Items.Add(string.Empty);

                if (listCcu != null)
                    foreach (var ccuListObj in listCcu)
                    {
                        _cbCcu.Items.Add(ccuListObj);

                        if (selected != null &&
                            selected.Id == ccuListObj.Id)
                        {
                            selected = ccuListObj;
                        }
                    }

                if (selected != null)
                    _cbCcu.SelectedItem = selected;
            }
            catch
            { }
            finally
            {
                _cbCcu.EndUpdate();
            }
        }

        private void FillComboboxDcu()
        {
            Exception error;
            var listDcu = Plugin.MainServerProvider.DCUs.GetListObj(out error);
            ShowComboboxDcu(listDcu);
        }

        private void ShowComboboxDcu(IList<DcuListObj> listDcu)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<IList<DcuListObj>>(ShowComboboxDcu), 
                    listDcu);

                return;
            }

            try
            {
                _cbDcu.BeginUpdate();

                var selected = _cbDcu.SelectedItem as DcuListObj;

                _cbDcu.Items.Clear();
                _cbDcu.Items.Add(string.Empty);

                if (listDcu != null)
                    foreach (var dcuListObj in listDcu)
                    {
                        _cbDcu.Items.Add(dcuListObj);

                        if (selected != null &&
                            selected.Id == dcuListObj.Id)
                        {
                            selected = dcuListObj;
                        }
                    }

                if (selected != null)
                    _cbDcu.SelectedItem = selected;
            }
            catch
            {
            }
            finally
            {
                _cbDcu.EndUpdate();
            }
        }

        private void _cbDcu_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterValueChanged(sender, e);

            if (_cbDcu.SelectedItem is DcuListObj)
                _cbCcu.SelectedItem = string.Empty;
        }

        private void _cbCcu_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterValueChanged(sender, e);

            if (_cbCcu.SelectedItem is CcuListObj)
                _cbDcu.SelectedItem = string.Empty;
        }

        private void _rbFilterOr_CheckedChanged(object sender, EventArgs e)
        {
            _filterSettingsJoinOperator = 
                _rbFilterAnd.Checked
                    ? LogicalOperators.AND
                    : LogicalOperators.OR;

            FilterValueChanged(sender, e);
        }

        private void DisableKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBoxListItemChecked_ItemCheck(
            object sender, 
            ItemCheckEventArgs e)
        {
            FilterValueChanged(sender, e);
        }

        private void _tbmMemberOfAclFilter_DragOver(object sender, DragEventArgs e)
        {
            var pg = GetObjectFromDragDrop<AccessControlList>(e);

            if (pg != null)
                e.Effect = DragDropEffects.All;
        }

        private void _tbmMemberOfAclFilter_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var pg = GetObjectFromDragDrop<AccessControlList>(e);

                if (pg == null)
                    return;

                SetSelectedObjects(
                    _tbmMemberOfAclFilter,
                    new ListOfObjects(
                        new List<object>
                        {
                            pg
                        }));
            }
            catch
            {
            }
        }

        private void _tbmMemberOfAclFilter_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify1)
            {
                ModifyAccessControlListObjects();
            }
            else if (item == _tsiRemove1)
            {
                SetSelectedObjects(_tbmMemberOfAclFilter, null);
            }
        }

        private void ModifyAccessControlListObjects()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                ConnectionLost();

                Exception error;
                IList<IModifyObject> listAclFromDatabase =
                    Plugin.MainServerProvider.AccessControlLists
                        .ListModifyObjects(out error);

                if (error != null) throw error;

                if (listAclFromDatabase == null)
                    listAclFromDatabase = new List<IModifyObject>();

                var formAdd = new ListboxFormAdd(listAclFromDatabase, GetString("NCASAccessControlListsFormNCASAccessControlListsForm"));

                ListOfObjects outObjects;
                formAdd.ShowDialogMultiSelect(out outObjects);

                if (outObjects != null)
                {
                    for (var i = 0; i < outObjects.Count; i++)
                    {
                        var modifyObject = outObjects[i] as IModifyObject;
                        if (modifyObject != null
                            && modifyObject.GetOrmObjectType == ObjectType.AccessControlList)
                        {
                            outObjects[i] = Plugin.MainServerProvider.AccessControlLists.GetObjectById(modifyObject.GetId);
                        }
                    }

                    SetSelectedObjects(_tbmMemberOfAclFilter, outObjects);
                }
            }
            catch
            {
            }
        }

        private void SetSelectedObjects(TextBoxMenu control, ListOfObjects objects)
        {
            control.Tag = objects;
            RefreshSelectedObject(control);
        }

        private void RefreshSelectedObject(TextBoxMenu control)
        {
            var actOnOffObjects = control.Tag as ListOfObjects;

            if (actOnOffObjects == null)
            {
                control.Text = string.Empty;
                control.TextImage = null;
            }
            else
            {
                if (actOnOffObjects.Count > 1)
                {
                    control.Text = actOnOffObjects.ToString();
                    try
                    {
                        control.TextImage =
                            Plugin.GetImageForListOfObject(actOnOffObjects);
                    }
                    catch
                    {
                        control.TextImage = null;
                    }
                }
                else
                {
                    var tempAOrm = actOnOffObjects[0] as AOrmObject;
                    if (tempAOrm != null)
                    {
                        var listFS = new ListObjFS(tempAOrm);
                        control.Text = listFS.ToString();
                        control.TextImage = Plugin.GetImageForAOrmObject(tempAOrm);
                    }
                    else
                    {
                        control.Text = actOnOffObjects.ToString();
                        control.TextImage = Plugin.GetImageForListOfObject(actOnOffObjects);
                    }
                }
            }

            base.FilterValueChanged(control, new EventArgs());
        }

        private bool IsRowVisible(CardReaderShort crShort)
        {
            if (crShort == null)
            {
                // Wrong data in DataGridView
                return false;
            }

            if (_currentOnlineStateFilter != null && 
                _currentOnlineStateFilter != crShort.OnlineState)
            {
                // Filtered by online state
                return false;
            }

            ShortObjectExtension<CardReaderShort, Guid?, SecurityLevel?, SecurityLevel?> crExt = null;

            if (_currentForcedSecurityLevel != null)
            {
                if (!_extCardReaderShort.TryGetValue(crShort.IdCardReader, out crExt))
                {
                    // Filtered because this card reader does not have extended informations
                    return false;
                }
                
                if (crExt.Value1 == null)
                {
                    // Filtered because this card reader does not have forced security level
                    return false;
                }

                State crOnOffObjectState;

                if (!_onOffObjectsToState.TryGetValue(
                    crExt.Value1.Value,
                    out crOnOffObjectState))
                {
                    // Filtered by currently forced security level
                    return false;
                }

                if (crOnOffObjectState !=
                    (_currentForcedSecurityLevel.Value ? State.On : State.Off))
                {
                    return false;
                }
            }

            if (_currentSecuritlyLevelFilter.Count > 0)
            {
                if (crExt == null &&!_extCardReaderShort.TryGetValue(crShort.IdCardReader, out crExt))
                {
                    // Filtered because this card reader does not have extended informations
                    return false;
                }

                State crOnOffObjectState;

                if (crExt.Value1 == null 
                    || !_onOffObjectsToState.TryGetValue(
                        crExt.Value1.Value,
                        out crOnOffObjectState))
                {
                    // This cr does not have forced security level
                    if (!_currentSecuritlyLevelFilter.Contains(crExt.Value2))
                    {
                        // Filtered by current security level
                        return false;
                    }    
                }
                else
                {
                    // This cr have forced security level
                    if (crOnOffObjectState == State.On)
                    {
                        // Check if selected filter match FORCED security level
                        if (!_currentSecuritlyLevelFilter.Contains(crExt.Value3))
                        {
                            // Filtered by current security level
                            return false;
                        }
                    }
                    else
                    {
                        // Check if selected filter match security level
                        if (!_currentSecuritlyLevelFilter.Contains(crExt.Value2))
                        {
                            // Filtered by current security level
                            return false;
                        }
                    }
                }
            }

            if (_chbBlocked.Checked && !crShort.IsBlocked)
                return false;

            return true;
        }

        protected void ApplyRuntimeFilterInternal()
        {
            if (_currentForcedSecurityLevel == null
                && _currentOnlineStateFilter == null
                && _currentSecuritlyLevelFilter.Count == 0
                && !_chbBlocked.Checked)
            {
                foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                {
                    _cdgvData.UpdateVisibility(row.Index, true);
                }

                _lRecordCount.Text = GetString("NCASCardReadersForm_lRecordCount") + ": " + _cdgvData.DataGrid.Rows.Count;
                return;
            }

            var dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            var bindingSourceList = BindingSource.List;

            var visibleCount = 0;

            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                if (IsRowVisible((CardReaderShort)bindingSourceList[row.Index]))
                {
                    visibleCount++;

                    _cdgvData.UpdateVisibility(row.Index, true);
                }
                else
                    _cdgvData.UpdateVisibility(row.Index, false);

            _lRecordCount.Text = GetString(Name + "_lRecordCount") + ": " + visibleCount + "/" + _cdgvData.DataGrid.Rows.Count;

            try
            {
                if (dgcell != null && dgcell.Visible)
                    _cdgvData.DataGrid.CurrentCell = dgcell;
            }
            catch
            {
            }
        }

        protected override void ApplyRuntimeFilter()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(ApplyRuntimeFilterInternal));
            else
                ApplyRuntimeFilterInternal();
        }

        private void _eNameFilter_TextChanged(object sender, EventArgs e)
        {
            FilterValueChanged(sender, e);
        }

        void _chbBlocked_CheckedChanged(object sender, System.EventArgs e)
        {
            FilterValueChanged(sender, e);
        }

        private void _bUnblock_Click(object sender, EventArgs e)
        {
            var selectedRows = _cdgvData.SelectedRows;

            if (selectedRows == null)
                return;

            foreach (DataGridViewRow selectedRow in selectedRows)
            {
                var crShort = selectedRow.DataBoundItem as CardReaderShort;

                if (crShort == null
                    || !crShort.IsBlocked)
                {
                    continue;
                }

                SafeThread<Guid>.StartThread(
                    UnblockCardReader,
                    crShort.IdCardReader);
            }

            _bUnblock.Enabled = false;
        }

        private void UnblockCardReader(Guid idCardReader)
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;

                Plugin.MainServerProvider.CardReaders.Unblock(idCardReader);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void bExportToExcel_Click(object sender, EventArgs e)
        {
            var exportForm = new ExcelExportForm(true, FilterSettings);
            exportForm.Show();
        }
    }
}
