using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client.Properties;
using Contal.Cgp.Client.StructuredSubSites;
using Contal.Cgp.Globals.PlatformPC;
using Contal.IwQuick.Localization;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Data;
using System.Collections;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using JetBrains.Annotations;
using Timer = System.Threading.Timer;

namespace Contal.Cgp.Client
{
    public partial class AlarmsDialog :
#if DESIGNER
        Form
#else
        MdiChildForm
#endif
    {
        private class DataGridAlarm
        {
            public const string COLUMN_GENERAL_BLOCKING_STATE = "GeneralBlockingState";

            public DataGridAlarm(ServerAlarmCore serverAlarm)
            {
                if (serverAlarm == null)
                    throw new NullReferenceException();

                ServerAlarm = serverAlarm;
            }

            [Browsable(false)]
            public ServerAlarmCore ServerAlarm { get; [UsedImplicitly] set; }

            [Browsable(false)]
            public IdServerAlarm IdServerAlarm
            {
                get
                {
                    return ServerAlarm.IdServerAlarm;
                }
            }

            [UsedImplicitly]
            public Icon OwnerState
            {
                get
                {
                    if (ServerAlarm.IdServerAlarm.IdOwner == Guid.Empty)
                        return ResourceGlobal.IconServerAlarm16;

                    if (ServerAlarm.OwnerIsOffline)
                        return ResourceGlobal.IconDisconnected16;

                    return ResourceGlobal.IconConnected16;
                }
            }

            [UsedImplicitly]
            public string CreatedDateTime
            {
                get
                {
                    string strDateTime = ServerAlarm.Alarm.CreatedDateTime.ToString(CultureInfo.InvariantCulture);
                    for (int i = 0; i < strDateTime.Length; i++)
                    {
                        if (Char.IsDigit(strDateTime, i))
                        {
                            if (i > 0 && Char.IsDigit(strDateTime, i - 1))
                            {
                                continue;
                            }
                            if (i < strDateTime.Length - 1 && Char.IsDigit(strDateTime, i + 1))
                            {
                                continue;
                            }

                            strDateTime = strDateTime.Insert(i, "0");
                            i++;
                        }
                    }

                    return strDateTime;
                }
            }

            [UsedImplicitly]
            public string AlarmState
            {
                get
                {
                    return CgpClient.Singleton.LocalizationHelper.GetString("AlarmStates_" + ServerAlarm.Alarm.AlarmState);
                }
            }

            [UsedImplicitly]
            public Icon Acknowledged
            {
                get
                {
                    if (ServerAlarm.AcknowledgeInPending)
                        return ResourceGlobal.IconAcknowledgeInPending20;

                    if (ServerAlarm.Alarm.IsAcknowledged)
                        return ResourceGlobal.IconNewAcknowledged20;

                    return ResourceGlobal.IconWarning20;
                }
            }

            [UsedImplicitly]
            public Icon IndividualBlockingState
            {
                get
                {
                    if (ServerAlarm.IndividualBlockinInPending != null)
                        return ResourceGlobal.IconBlockingInPending20;

                    if (ServerAlarm.IndividualUnblockinInPending != null)
                        return ResourceGlobal.IconUnblockingInPending20;

                    if (ServerAlarm.Alarm.IsBlockedIndividual)
                        return ResourceGlobal.IconNewBlocked20;

                    return null;
                }
            }

            [UsedImplicitly]
            public Icon GeneralBlockingState
            {
                get
                {
                    if (ServerAlarm.Alarm.IsBlockedGeneral)
                        return ResourceGlobal.IconNewBlocked20;

                    return null;
                }
            }

            [UsedImplicitly]
            public string Priority
            {
                get
                {
                    return CgpClient.Singleton.LocalizationHelper.GetString("AlarmPriority_" + ServerAlarm.AlarmPriority);
                }
            }

            [UsedImplicitly]
            public string Name
            {
                get
                {
                    return ServerAlarm.Name;
                }
            }

            [UsedImplicitly]
            public string Sources
            {
                get
                {
                    if (GeneralOptionsForm.Singleton.EventSourcesReverseOrder)
                        return ServerAlarm.ParentsStringReversed;
                    return ServerAlarm.ParentsString;
                }
            }
        }

        private readonly SortableBindingList<DataGridAlarm> _listServerAlarms;
        private ICollection<ServerAlarmCore> _listServerAlarmsOrigin;
        private List<ServerAlarmCore> _selectedServerAlarms;
        private BindingSource _bindingSource;

        private const int SLEEP_BEFORE_FILTER_KEY_PRESSED = 1000;
        private bool _wasChecked;
        private bool _nestedInAnotherWindow = true;
        
        private readonly object _syncTimerForSelectingAlarms = new object();
        private volatile Timer _timerForSelectingAlarms;
        private readonly SelectingOfSubSitesSupport _selectingOfSubSitesSupport;
        private readonly TimerManager _showAlarmsTimerManager = new TimerManager();
        private ITimer _showAlarmsTimer;

        public bool NestedInAnotherWindow
        {
            get { return _nestedInAnotherWindow; }
            set
            {
                _nestedInAnotherWindow = value;
                _bDockUndock.Visible = value;
                _bCancel.Visible = value;
            }
        }

        private const string SymbolObjectsSearchForm_bUndock = "ObjectsSearchForm_bUndock";

        public AlarmsDialog(ICollection<ServerAlarmCore> listServerAlarms)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            NestedInAnotherWindow = true;
            
            _cbAllSubSites.CheckState = CheckState.Checked;
            _selectingOfSubSitesSupport = new SelectingOfSubSitesSupport(
                _wpfCheckBoxTreeView,
                _cbAllSubSites);
            
            _tbdpDateFrom.LocalizationHelper = LocalizationHelper;
            _tbdpDateTo.LocalizationHelper = LocalizationHelper;
            MdiParent = CgpClientMainForm.Singleton;
            FormOnEnter += Form_Enter;
            StartPosition = FormStartPosition.CenterScreen;
            _cdgvData.Dock = DockStyle.Fill;

            FormBorderStyle = FormBorderStyle.None;
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Dock = DockStyle.Fill;
            _bDockUndock.Text = LocalizationHelper.GetString(SymbolObjectsSearchForm_bUndock);
            _listServerAlarmsOrigin = listServerAlarms;

            _listServerAlarms = new SortableBindingList<DataGridAlarm>();

            if (listServerAlarms != null)
                foreach (var alarm in listServerAlarms)
                {
                    _listServerAlarms.Add(new DataGridAlarm(alarm));
                }

            _cdgvData.VisibleChanged += _dgValues_VisibleChanged;
            _cdgvData.DataGrid.CellMouseClick += DataGrid_CellMouseClick;

            _bDockUndock.Text = LocalizationHelper.GetString(SymbolObjectsSearchForm_bUndock);
            InitAlarmPriority();
            _eventColorChanged = ColorSettingsChanged;
            ColorSettingsChangedHandler.Singleton.RegisterColorChanged(_eventColorChanged);
            _wasChecked = _cbShowOnlyBlockedAlarm.Checked;
            InitCGPDataGridView();

            _cdgvData.DataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _cdgvData.DataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            SetReferenceEditColors();

            // SB
            var dataGrid = _cdgvData.DataGrid;

            dataGrid.GotFocus += OnDataGridFocusChanged;
            dataGrid.LostFocus += OnDataGridFocusChanged;
        }

        // SB
        /// <summary>
        /// OnDataGridFocusChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataGridFocusChanged(object sender, EventArgs e)
        {
            var selectedRows = _cdgvData.SelectedRows;

            if (selectedRows.Count > 0 && this.LocalizationHelper.ActualLanguage != "Swedish")
            {                
                var defaultCellStyle = _cdgvData.DataGrid.DefaultCellStyle;

                var selectionBackColor = defaultCellStyle.SelectionBackColor;
                var selectionForeColor = defaultCellStyle.SelectionForeColor;

                var isFocused = _cdgvData.DataGrid.Focused;

                for (int i = 0; i < selectedRows.Count; i++)
                {
                    var cellStyle = selectedRows[i].DefaultCellStyle;

                    if (isFocused)
                    {
                        cellStyle.SelectionBackColor = selectionBackColor;
                        cellStyle.SelectionForeColor = selectionForeColor;
                    }
                    else
                    {
                        cellStyle.SelectionBackColor = cellStyle.BackColor;
                        cellStyle.SelectionForeColor = cellStyle.ForeColor;
                    }
                }
            }
        }

        private bool UnlockShowAlarmsTimeout(TimerCarrier timerCarrier)
        {
            _showAlarmsManualResetEvent.Set();
            return true;
        }

        private void DataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_cdgvData.DataGrid.DataSource == null
                || e.RowIndex != -1
                || e.ColumnIndex != 0)
            {
                return;
            }

            if (_showAlarmsTimer != null)
                _showAlarmsTimer.StopTimer();

            _showAlarmsManualResetEvent.Set();

            var odlOwnerStateSortOrder = _cdgvData.DataGrid.Columns[SymbolOwnerState].HeaderCell.SortGlyphDirection;

            _cdgvData.DataGrid.Columns[SymbolOwnerState].HeaderCell.SortGlyphDirection =
                odlOwnerStateSortOrder == SortOrder.None || odlOwnerStateSortOrder == SortOrder.Descending
                    ? SortOrder.Ascending
                    : SortOrder.Descending;

            ShowAlarms(_listServerAlarmsOrigin, false);
        }

        private const int DelayForAlarmSelection = 500;

        private void RestartTimerForSelectingAlarms()
        {
            lock (_syncTimerForSelectingAlarms)
            {
                if (null == _timerForSelectingAlarms)
                    _timerForSelectingAlarms = new Timer(
                        OnTimerForSelectingAlarms_Tick,
                        null,
                        Timeout.Infinite,
                        Timeout.Infinite
                        );

                _timerForSelectingAlarms.Change(DelayForAlarmSelection, Timeout.Infinite);
            }
        }

        private void DisposeTimerForSelectingAlarms()
        {
            lock (_syncTimerForSelectingAlarms)
            {
                try
                {
                    _timerForSelectingAlarms.Change(Timeout.Infinite, Timeout.Infinite);
                }
                catch
                {
                }

                try
                {
                    _timerForSelectingAlarms.Dispose();
                }
                catch
                {
                }
            }
        }

        void OnTimerForSelectingAlarms_Tick(object sender)
        {
            if (!Monitor.TryEnter(_syncTimerForSelectingAlarms))
                return;

            try
            {
                Thread2UI.Invoke(this, RefreshAlarmStateButtons,true);
            }
            catch
            {

            }
            finally
            {
                Monitor.Exit(_syncTimerForSelectingAlarms);
            }


        }

        private void InitCGPDataGridView()
        {
            //splitContainer.Panel2Collapsed = true;
            HideInstructionsPanel(true);
            _cdgvData.DataGrid.Click += _cdgvData_Click;
            _cdgvData.DataGrid.CellMouseUp += _dgValues_CellMouseUp;
            _cdgvData.DataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
            _cdgvData.DataGrid.SelectionChanged += _dgValues_SelectionChanged;
            _cdgvData.AfterSort += _cdgvData_AfterSort;
            _cdgvData.DataGrid.CellMouseDown += DataGrid_CellMouseDown;
        }

        void DataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_cdgvData.DataGrid.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            _bViewDetails_Click(null, null);
        }

        void DataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_showAlarmsTimer != null)
                _showAlarmsTimer.StopTimer();

            _showAlarmsManualResetEvent.Reset();

            if (e.RowIndex == -1)
            {
                SaveSelectedAlarms();
            }
        }

        private void SaveSelectedAlarms()
        {
            if (_selectedServerAlarms == null)
                _selectedServerAlarms = new List<ServerAlarmCore>();
            _selectedServerAlarms.Clear();

            if (_cdgvData.DataGrid.SelectedRows != null)
                foreach (DataGridViewRow row in _cdgvData.DataGrid.SelectedRows)
                {
                    DataGridAlarm dga;
                    if (_bindingSource.Count > row.Index &&
                        (dga = (_bindingSource[row.Index] as DataGridAlarm))!=null) 
                        _selectedServerAlarms.Add(dga.ServerAlarm);
                }
        }

        void _cdgvData_AfterSort(IList dataSourceList)
        {
            UpdateDGVRows();
            DoSelectRows();
        }

        private void DoSelectRows()
        {
            if (_bindingSource == null)
                return;

            var positionSelectRows = new List<int>();

            if (_selectedServerAlarms != null && _selectedServerAlarms.Count > 0 && _cdgvData.DataGrid.Rows.Count > 0)
            {
                // SB
                //_cdgvData.DataGrid.Rows[0].Selected = false;

                for (int i = 0; i < _bindingSource.Count; i++)
                {
                    var idServerAlarm = ((DataGridAlarm) _bindingSource[i]).IdServerAlarm;

                    if (_selectedServerAlarms.Any(alarm => alarm.IdServerAlarm.Equals(idServerAlarm)))
                    {
                        _cdgvData.DataGrid.Rows[i].Selected = true;
                        positionSelectRows.Add(i);
                    }
                }
                
                int visibleRows = _cdgvData.DataGrid.DisplayedRowCount(true);
                int firstDisplayedScrollingRowIndex = 0;
                int maxSelectedRows = 0;

                for (int i = 0; i < positionSelectRows.Count; i++)
                {
                    int selectedRowsCount = 0;
                   
                    for (int j = i; j < positionSelectRows.Count; j++)
                    {
                        if (positionSelectRows[j] < positionSelectRows[i] + visibleRows)
                            selectedRowsCount++;
                        else
                            break;
                    }

                    if (selectedRowsCount > maxSelectedRows)
                    {
                        maxSelectedRows = selectedRowsCount;
                        firstDisplayedScrollingRowIndex = positionSelectRows[i];
                    }
                }

                _cdgvData.DataGrid.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex;
                positionSelectRows.Clear();
            }
        }

        public override void CallEscape()
        {
            Close();
        }

        private readonly DVoid2Void _eventColorChanged = null;

        private void ColorSettingsChanged()
        {
            ShowAlarmsScSw(_listServerAlarms);
        }

        protected override void AfterTranslateForm()
        {
            foreach (ToolStripItem item in _alarmMenu.Items)
            {
                item.Text = GetString(Name + item.Name);
            }
        }

        void _dgValues_VisibleChanged(object sender, EventArgs e)
        {
            ShowAlarms(_listServerAlarmsOrigin, true);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bViewDetails_Click(object sender, EventArgs e)
        {
            if (_bindingSource != null && _bindingSource.Count > 0 && _bindingSource.Current != null)
            {
                var serverAlarm = ((DataGridAlarm) _bindingSource.Current).ServerAlarm;
                if (serverAlarm != null)
                {
                    if (CgpClientMainForm.Singleton._alarmDetails == null || CgpClientMainForm.Singleton._alarmDetails.IsDisposed)
                    {
                        CgpClientMainForm.Singleton._alarmDetails = new AlarmDetailsForm(serverAlarm);
                    }
                    else
                    {
                        CgpClientMainForm.Singleton._alarmDetails.SetAnotherAlarm(serverAlarm);
                    }
                    CgpClientMainForm.Singleton._alarmDetails.Show();
                }
            }
        }

        private ServerAlarmCore _selectServerAlarmForInstruction;
        private readonly ManualResetEvent _showAlarmsManualResetEvent = new ManualResetEvent(true);

        public void ShowAlarms(ICollection<ServerAlarmCore> listServerAlarms, bool forcedImplicitSort)
        {
            _showAlarmsManualResetEvent.WaitOne();

            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            var listAlarmsToShow = new List<ServerAlarmCore>();
            ServerAlarmCore alarmForShowInstructions = null;

            if (listServerAlarms != null)
            {
                foreach (var serverAlarm in listServerAlarms)
                {
                    var alarm = serverAlarm.Alarm;

                    serverAlarm.Name = LocalizeAlarmType(
                        serverAlarm.Alarm.AlarmKey.AlarmType,
                        serverAlarm.Name);


                    if (serverAlarm.ReferencedServerAlarm != null)
                    {
                        serverAlarm.ReferencedServerAlarm.Name = LocalizeAlarmType(
                            serverAlarm.ReferencedServerAlarm.Alarm.AlarmKey.AlarmType,
                            serverAlarm.ReferencedServerAlarm.Name);

                        serverAlarm.ParentsStringReversed = LocalizeAlarmType(
                            serverAlarm.ReferencedServerAlarm.Alarm.AlarmKey.AlarmType,
                            serverAlarm.ParentsStringReversed);

                        serverAlarm.ParentsString = LocalizeAlarmType(
                            serverAlarm.ReferencedServerAlarm.Alarm.AlarmKey.AlarmType,
                            serverAlarm.ParentsString);
                    }

                    listAlarmsToShow.Add(serverAlarm);

                    if (!serverAlarm.IsAcknowledged && alarm.AlarmState == AlarmState.Alarm)
                    {
                        if ((alarmForShowInstructions == null
                             || alarmForShowInstructions.Alarm.CreatedDateTime < alarm.CreatedDateTime)
                            && serverAlarm.HasAlarmInstructions)
                        {
                            alarmForShowInstructions = serverAlarm;
                        }
                    }
                }
            }

            var ownerStateSortOrder = SortOrder.None;

            Invoke(new MethodInvoker(delegate
            {
                var alarmsBindingSource = _cdgvData.DataGrid.DataSource as BindingSource;

                if (alarmsBindingSource != null)
                {
                    if (forcedImplicitSort)
                    {
                        alarmsBindingSource.Sort = string.Empty;
                        return;
                    }

                    ownerStateSortOrder =
                        _cdgvData.DataGrid.Columns[SymbolOwnerState].HeaderCell.SortGlyphDirection;

                    if (ownerStateSortOrder != SortOrder.None)
                        alarmsBindingSource.Sort = string.Empty;
                }
            }));

            if (_cdgvData.DataGrid.SortOrder == SortOrder.None)
            {
                listAlarmsToShow =
                    listAlarmsToShow.OrderBy(
                        serverAlarm =>
                        {
                            if (ownerStateSortOrder == SortOrder.None)
                                return string.Concat(
                                    (byte) serverAlarm.AlarmPriority,
                                    (byte) serverAlarm.Alarm.AlarmState,
                                    serverAlarm.Name,
                                    serverAlarm.OwnerIsOffline);

                            return string.Concat(
                                serverAlarm.OwnerIsOffline
                                    ? ownerStateSortOrder == SortOrder.Ascending
                                    : ownerStateSortOrder == SortOrder.Descending,
                                (byte) serverAlarm.AlarmPriority,
                                (byte) serverAlarm.Alarm.AlarmState,
                                serverAlarm.Name);
                        }).ToList();
            }

            _listServerAlarmsOrigin = listAlarmsToShow;
            DoFilterAlarm();

            if (ownerStateSortOrder != SortOrder.None)
                Invoke(new MethodInvoker(delegate
                {
                    _cdgvData.DataGrid.Columns[SymbolOwnerState].HeaderCell.SortGlyphDirection = ownerStateSortOrder;
                }));

            if (alarmForShowInstructions != null && ExistsAlarm(alarmForShowInstructions))
            {
                _selectServerAlarmForInstruction = alarmForShowInstructions;
                List<AlarmInstruction> alarmInstrunctions = GetAlarmInstrunctions(alarmForShowInstructions);
                Invoke(new DVoid2Void(
                    delegate()
                    {
                        _llAlarmName.Visible = true;
                        ShowAlarmInstructions(alarmInstrunctions, alarmForShowInstructions.Name);
                    }));
            }
        }

        private void ShowAlarmsScSw(IEnumerable<DataGridAlarm> listAlarms)
        {
            Thread2UI.Invoke(this, () =>
            {
                SuspendLayout();
                SaveSelectedAlarms();
                //if _listAlarms is listAlarms, than clearing one collection will clear the other collection too
                if (_listServerAlarms != listAlarms)
                {
                    _listServerAlarms.Clear();
                    foreach (DataGridAlarm alarm in listAlarms)
                    {
                        _listServerAlarms.Add(alarm);
                    }
                }

                if (_bindingSource == null)
                {
                    _bindingSource = new BindingSource {DataSource = _listServerAlarms};
                }

                if (_cdgvData.DataGrid.DataSource == null)
                {
                    _cdgvData.DataGrid.DataSource = _bindingSource;
                    SetDGVColumns();
                }

                //call DoSelectRows only if no sort will be applied to prevent selecting two times (after sort, DoSelectRows will be called automatically)
                if (_cdgvData.DataGrid.SortedColumn != null && _cdgvData.DataGrid.SortOrder != SortOrder.None)
                    _cdgvData.DataGrid.Sort(_cdgvData.DataGrid.SortedColumn,
                        _cdgvData.DataGrid.SortOrder == SortOrder.Ascending
                            ? ListSortDirection.Ascending
                            : ListSortDirection.Descending);
                else
                    DoSelectRows();

                UpdateDGVRows();
                CgpClient.Singleton.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvData.DataGrid);
                ResumeLayout();
            });
        }

        private void UpdateDGVRows()
        {
            Invoke(new MethodInvoker(delegate
            {
                int AlarmNotAcknowledgedCount = 0;
                int AlarmCount = 0;
                int NormalNotAcknowledgedCount = 0;

                for (int i = 0; i < _listServerAlarms.Count; i++)
                {
                    var serverAlarm = _listServerAlarms[i].ServerAlarm;
                    var alarm = serverAlarm.Alarm;

                    if (serverAlarm.IsBlocked)
                    {
                        if (!serverAlarm.IsAcknowledged)
                            NormalNotAcknowledgedCount++;
                    }
                    else
                    {
                        if (alarm.AlarmState == AlarmState.Alarm && !serverAlarm.IsAcknowledged)
                        {
                            AlarmNotAcknowledgedCount++;
                        }
                        else if (alarm.AlarmState == AlarmState.Alarm && serverAlarm.IsAcknowledged)
                        {
                            AlarmCount++;
                        }
                        else if (alarm.AlarmState == AlarmState.Normal && !serverAlarm.IsAcknowledged)
                        {
                            NormalNotAcknowledgedCount++;
                        }
                    }

                    if (serverAlarm.IsBlocked &&
                        !(CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.AlarmsBlockedAlarmsView)) ||
                          CgpClient.Singleton.MainServerProvider.HasAccess(
                              BaseAccess.GetAccess(LoginAccess.AlarmsBlockedAlarmsAdmin))))
                    {
                        _cdgvData.DataGrid.Rows[i].Visible = false;
                    }

                    _cdgvData.DataGrid.Rows[i].DefaultCellStyle.BackColor =
                        GeneralOptionsForm.Singleton.GetAlarmStateColorBackground(
                            serverAlarm.IsBlocked,
                            alarm.AlarmState,
                            serverAlarm.IsAcknowledged);

                    _cdgvData.DataGrid.Rows[i].DefaultCellStyle.ForeColor =
                        GeneralOptionsForm.Singleton.GetAlarmStateColorText(
                            serverAlarm.OwnerIsOffline,
                            serverAlarm.IsBlocked,
                            alarm.AlarmState,
                            serverAlarm.IsAcknowledged);

                    ChangeSelectedRowColor(_cdgvData.DataGrid.Rows[i].DefaultCellStyle);
                }

                _lAlarmNotAcknowladgedCount.Text = AlarmNotAcknowledgedCount.ToString(CultureInfo.InvariantCulture);
                _lAlarmCount.Text = AlarmCount.ToString(CultureInfo.InvariantCulture);
                _lNormalNotAcknowledgedCount.Text = NormalNotAcknowledgedCount.ToString(CultureInfo.InvariantCulture);
            }));
        }

        private const string SymbolOwnerState = "OwnerState";

        private void SetDGVColumns()
        {
            var individualBlockingStateColumn = _cdgvData.DataGrid.Columns["IndividualBlockingState"] as DataGridViewImageColumn;
            if (individualBlockingStateColumn != null)
                individualBlockingStateColumn.DefaultCellStyle.NullValue = null;

            var generalBlockingStateColumn = _cdgvData.DataGrid.Columns["GeneralBlockingState"] as DataGridViewImageColumn;
            if (generalBlockingStateColumn != null)
                generalBlockingStateColumn.DefaultCellStyle.NullValue = null;

            _cdgvData.DataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _cdgvData.DataGrid.Columns["CreatedDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _cdgvData.DataGrid.Columns[_cdgvData.DataGrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _cdgvData.DataGrid.Columns[_cdgvData.DataGrid.Columns.Count - 1].MinimumWidth = 100;

            _cdgvData.DataGrid.Columns[SymbolOwnerState].HeaderText = string.Empty;
            _cdgvData.DataGrid.Columns[SymbolOwnerState].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _cdgvData.DataGrid.Columns[SymbolOwnerState].SortMode = DataGridViewColumnSortMode.Programmatic;
        }

        private void _bConfirmState_Click(object sender, EventArgs e)
        {
            if (_showAlarmsTimer != null)
                _showAlarmsTimer.StopTimer();

            _showAlarmsManualResetEvent.Set();

            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            var idsAlarm = GetSelectedAlarmsFromBindingSource();
            if (!idsAlarm.Any())
                return;

            CgpClient.Singleton.MainServerProvider.ConfirmAlarmsState(idsAlarm);
        }

        private ICollection<IdServerAlarm> GetSelectedAlarmsFromBindingSource()
        {
            var idsAlarm = new LinkedList<IdServerAlarm>();
            DataGridViewSelectedRowCollection selected = _cdgvData.SelectedRows;

            for (int i = 0; i < selected.Count; i++)
            {
                idsAlarm.AddLast(((DataGridAlarm) selected[i].DataBoundItem).IdServerAlarm);
            }

            return idsAlarm;
        }

        private void _dgValues_SelectionChanged(object sender, EventArgs e)
        {
            _bAcknowledgeState.Enabled = false;
            _bViewDetails.Enabled = false;
            _bBlock.Enabled = false;
            _bAcknowledgeAndBlock.Enabled = false;
            _bUnblock.Enabled = false;

            _tsAcknowledge.Visible = false;
            _tsAcknowledgeAndBlock.Visible = false;
            _tsBlock.Visible = false;
            _tsUnblock.Visible = false;

            if (_bindingSource != null && _bindingSource.Count > 0)
            {
                //RefreshAlarmStateButtons();

                _tsLoading.Visible = true;

                RestartTimerForSelectingAlarms();
                
                _bViewDetails.Enabled = true;
            }
        }

        /// <summary>
        ///  Change backroud/foreground color of the selected row. 
        /// </summary>
        /// <param name="cellStyle">Row cell style.</param>
        private void ChangeSelectedRowColor(DataGridViewCellStyle cellStyle)
        {            
            if (isValidVersion())
            {
                // Calculate alpha blending 
                Color blandOverColor = Color.White;
                if (cellStyle.BackColor.Equals(blandOverColor))
                    blandOverColor = Color.LightBlue;
                Color blendColor = CalculateAlphaBlending(127, cellStyle.BackColor, blandOverColor);
                cellStyle.SelectionBackColor = Color.FromArgb(255, blendColor.R, blendColor.G, blendColor.B);
                cellStyle.SelectionForeColor = CalculateAlphaBlending(255, cellStyle.ForeColor, blendColor);
            }
            // SB
            /*
            else
            {
                cellStyle.SelectionBackColor = cellStyle.BackColor;
                cellStyle.SelectionForeColor = cellStyle.ForeColor;
            }
            */
        }

        /// <summary>
        /// Checks operating system version to display blanded colors.
        /// </summary>
        /// <returns>True if version is 6.1 and upper. </returns>
        private bool isValidVersion()
        {
            // SB
            bool isValid = false;
            OperatingSystem os = Environment.OSVersion;

            if (os.Platform == PlatformID.Win32NT)
            {                
                isValid = os.Version.Major >= 6 && this.LocalizationHelper.ActualLanguage == "Swedish";
            }

            return isValid;
        }

        /// <summary>
        ///     Calculates alpha blending color. 
        /// </summary>
        /// <param name="alpha">Transparency, 255 means fully transparent and 0 means fully opaque.</param>
        /// <param name="srcColor">Source Color.</param>
        /// <param name="backColor">Background Color.</param>
        /// <example> This sample shows how to calculate the 50% transparent red pixel over the white.
        /// <code>
        ///  CalculateAlphaBlending(127, Color.Red, Color.White);
        /// </code>
        /// </example>
        private Color CalculateAlphaBlending(int alpha, Color srcColor, Color backColor)
        {
            // The alpha blend algorithm:
            // displayColor = sourceColor×alpha / 255 + backgroundColor×(255 – alpha) / 255
            var x = (255 - alpha);
            var final_R = (srcColor.R * alpha / 255) + (backColor.R * x / 255);
            var final_G = (srcColor.G * alpha / 255) + (backColor.G * x / 255);
            var final_B = (srcColor.B * alpha / 255) + (backColor.B * x / 255);
            return Color.FromArgb(alpha, final_R, final_G, final_B);
        }

        private void RefreshAlarmStateButtons()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            try
            {
                var selected = _cdgvData.SelectedRows;

                _bAcknowledgeState.Enabled = false;
                _bAcknowledgeAndBlock.Enabled = false;
                _bBlock.Enabled = false;
                _bUnblock.Enabled = false;

                _tsAcknowledge.Visible = false;
                _tsBlock.Visible = false;
                _tsUnblock.Visible = false;
                _tsAcknowledgeAndBlock.Visible = false;
                _tsLoading.Visible = false;

                for (int i = 0; i < selected.Count; i++)
                {
                    int i2 = selected[i].Index;

                    ChangeSelectedRowColor(selected[i].DefaultCellStyle);

                    var serverAlarm = (_bindingSource.List[i2] as DataGridAlarm) == null
                        ? null
                        : (_bindingSource.List[i2] as DataGridAlarm).ServerAlarm;


                    if (serverAlarm != null)
                    {
                        bool hasAlarmAdmin =
                            CgpClient.Singleton.MainServerProvider.HasAccess(
                                BaseAccess.GetAccess(LoginAccess.AlarmsAlarmsAdmin));

                        if (!serverAlarm.IsAcknowledged && !serverAlarm.IsBlockedIndividual)
                        {
                            _bAcknowledgeAndBlock.Enabled = hasAlarmAdmin;
                            _tsAcknowledgeAndBlock.Visible = hasAlarmAdmin;
                        }

                        //Collection contains alarmNotAckowledged - enable button _bAcknowledgeState and menu item _tsAcknowledge
                        if (!serverAlarm.IsAcknowledged)
                        {
                            _bAcknowledgeState.Enabled = hasAlarmAdmin;
                            _tsAcknowledge.Visible = hasAlarmAdmin;
                        }

                        bool hasBlockedAlarmAdmin =
                            CgpClient.Singleton.MainServerProvider.HasAccess(
                                BaseAccess.GetAccess(LoginAccess.AlarmsBlockedAlarmsAdmin));

                        //Collection contains blocked alarm enable - button _bUnblock and menu item _tsUnblock
                        if (serverAlarm.IsBlockedIndividual)
                        {
                            _bUnblock.Enabled = hasBlockedAlarmAdmin;
                            _tsUnblock.Visible = hasBlockedAlarmAdmin;
                        }
                            //Collection contains unblocked alarm - enable button _bUnblock and menu item _tsBlock
                        else
                        {
                            _bBlock.Enabled = hasBlockedAlarmAdmin;
                            _tsBlock.Visible = hasBlockedAlarmAdmin;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private bool _isRunedForm_Enter;

        private void Form_Enter(Form form)
        {
            if (!_isRunedForm_Enter)
            {
                if (_nestedInAnotherWindow)
                    CgpClientMainForm.Singleton.AddToOpenWindows(this);

                _isRunedForm_Enter = true;
                this.Dock = DockStyle.Fill;
            }

            if (CgpClient.Singleton.MainServerProvider == null || !CgpClient.Singleton.MainServerProvider.HasAccess(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS)))
            {
                Close();
                return;
            }

            if (CgpClient.Singleton.MainServerProvider.HasAccess(
                BaseAccess.GetAccess(LoginAccess.AlarmsBlockedAlarmsAdmin)))
            {
                _cbShowOnlyBlockedAlarm.Enabled = true;
            }
            else
            {
                if (CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.AlarmsBlockedAlarmsView)))
                {
                    if (CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.AlarmsAlarmsView)) ||
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.AlarmsAlarmsAdmin)))
                    {
                        _cbShowOnlyBlockedAlarm.Enabled = true;
                    }
                    else
                    {
                        _cbShowOnlyBlockedAlarm.Checked = true;
                        _wasChecked = true;
                        _cbShowOnlyBlockedAlarm.Enabled = false;
                    }
                }
                else
                {
                    _cbShowOnlyBlockedAlarm.Checked = false;
                    _wasChecked = false;
                    _cbShowOnlyBlockedAlarm.Enabled = false;
                }
            }

            RefreshAlarmStateButtons();

            _selectingOfSubSitesSupport.LoadSubSites();

            CgpClientMainForm.Singleton.SetActOpenWindow(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
            CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
            CgpClientMainForm.Singleton.RemoveFormUndockedForms(this);
            CgpClientMainForm.Singleton.AlarmsDialogClose();
            ColorSettingsChangedHandler.Singleton.UnregisterColorChanged(_eventColorChanged);

            DisposeTimerForSelectingAlarms();

            if (_selectingOfSubSitesSupport != null)
                _selectingOfSubSitesSupport.ClearSubSites();
        }

        

        private void _bDockUndock_Click(object sender, EventArgs e)
        {
            DoDockUndock();
        }

        public void DoDockUndock()
        {
            this.BeginInvokeInUI(() =>
            {
                if (CgpClient.Singleton.IsConnectionLost(false))
                {
                    Close();
                }
                else
                {
                    if (MdiParent == null)
                    {
                        TopMost = false;
                        WindowState = FormWindowState.Normal;
                        MdiParent = CgpClientMainForm.Singleton;
                        FormBorderStyle = FormBorderStyle.None;
                        Dock = DockStyle.Fill;
                        CgpClientMainForm.Singleton.AddToOpenWindows(this);
                        CgpClientMainForm.Singleton.SetActOpenWindow(this);
                        CgpClientMainForm.Singleton.RemoveFormUndockedForms(this);
                        _bDockUndock.Text = LocalizationHelper.GetString(SymbolObjectsSearchForm_bUndock);
                    }
                    else
                    {
                        MdiParent = null;
                        FormBorderStyle = FormBorderStyle.Sizable;
                        //this.TopMost = true;
                        CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);
                        CgpClientMainForm.Singleton.AddToUndockedForms(this);
                        _bDockUndock.Text = LocalizationHelper.GetString("ObjectsSearchForm_bDockUndock");
                    }
                    ShowAlarmsScSw(_listServerAlarms);
                }
            });
        }

        /// <summary>
        /// used also by other forms/dialog
        /// </summary>
        /// <param name="alarmType"></param>
        /// <param name="localizeString"></param>
        /// <returns></returns>
        internal static string LocalizeAlarmType(
            AlarmType alarmType,
            string localizeString)
        {
            var universalRui = alarmType.ToString();
            var localizedRui = CgpClient.Singleton.GetLocalizedString(CgpClient.ALARM_TYPE_LOCALIZATION_PREFIX + universalRui);

            if (Validator.IsNullString(universalRui) ||
                Validator.IsNullString(localizedRui))
            {
                return localizeString;
            }

            return localizeString.Replace(universalRui, localizedRui);
        }

        public static bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                {
                    return
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS));
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private List<ServerAlarmCore> _filteredListServerAlarms;
        private void DoFilterAlarm()
        {
            _filteredListServerAlarms = new List<ServerAlarmCore>();
            if (_listServerAlarmsOrigin == null)
            {
                _listServerAlarmsOrigin = 
                    _listServerAlarms.Select(
                        dataGridAlarm => dataGridAlarm.ServerAlarm).ToList();
            }
            if (_listServerAlarmsOrigin == null) return;

            PrepareDataForFilter();
            var selectedSites = _selectingOfSubSitesSupport.GetSelectedSiteIds();

            foreach (var serverAlarm in _listServerAlarmsOrigin)
            {
                var alarm = serverAlarm.Alarm;

                if (!_isFilterSet
                    || (FilterAlarmDate(alarm)
                    && FilterAlarmPriority(serverAlarm)
                    && FilterAlarmText(serverAlarm)
                    && FilterBySites(serverAlarm, selectedSites)
                    && (FilterByEventSources(serverAlarm))))
                {
                    if (((_cbShowOnlyBlockedAlarm.Checked && serverAlarm.IsBlocked) || (!_cbShowOnlyBlockedAlarm.Checked && (!serverAlarm.IsBlocked)))
                        && !_filteredListServerAlarms.Contains(serverAlarm))
                    {
                        _filteredListServerAlarms.Add(serverAlarm);
                    }
                }
            }

            ShowFilteredCount(_filteredListServerAlarms.Count);

            ShowAlarmsScSw(_filteredListServerAlarms.Select(serverAlarm => new DataGridAlarm(serverAlarm)));

            if (_cdgvData.DataGrid.Columns.Contains(DataGridAlarm.COLUMN_GENERAL_BLOCKING_STATE))
                _cdgvData.DataGrid.Columns[DataGridAlarm.COLUMN_GENERAL_BLOCKING_STATE].Visible = _cbShowOnlyBlockedAlarm.Checked;

            if (_wasChecked &&
                _filteredListServerAlarms.Count == 0 &&
                (CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.AlarmsAlarmsView)) ||
                 CgpClient.Singleton.MainServerProvider.HasAccess(
                     BaseAccess.GetAccess(LoginAccess.AlarmsAlarmsAdmin))))
            {
                SetCheckedCbShowOnlyBlockedAlarm(false);
                _wasChecked = false;
            }
        }

        private void SetCheckedCbShowOnlyBlockedAlarm(bool isChecked)
        {
            this.BeginInvokeInUI(() =>
            {
                _cbShowOnlyBlockedAlarm.Checked = isChecked;
            });
        }

        private void ForcedFilterAlarm()
        {
            _isFilterSet = true;
            DoFilterAlarm();
        }

        private void ShowFilteredCount(int countFO)
        {
            Thread2UI.Invoke(this,
                () =>
                {
                    _lFilteredCount.Visible = true;
                    _lFilteredCount.Text = string.Format("{0}: {1}", GetString("TextAlarmCount"), countFO);
                });
            
        }

        private bool _isDateApplied;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private String _filterTextString = string.Empty;
        private ItemAlarmPriority _filterAlarmPriority;

        private void PrepareDataForFilter()
        {
            this.InvokeInUI(() =>
            {
                if (_cbFilterAlarmPriority.SelectedItem is ItemAlarmPriority)
                {
                    _filterAlarmPriority = (_cbFilterAlarmPriority.SelectedItem as ItemAlarmPriority);
                }
                else
                {
                    _filterAlarmPriority = null;
                }
                _filterTextString = _eFilterText.Text.ToLower();

                if (_tbdpDateTo.Text == string.Empty &&
                    _tbdpDateFrom.Text == string.Empty)
                {
                    _isDateApplied = false;
                    return;
                }

                if (_tbdpDateFrom.Text == string.Empty)
                {
                    _dateFrom = DateTime.MinValue;
                }
                else
                {
                    DateTime.TryParse(_tbdpDateFrom.Text, out _dateFrom);
                }

                if (_tbdpDateTo.Text == string.Empty)
                {
                    _dateTo = DateTime.MaxValue;
                }
                else
                {
                    DateTime.TryParse(_tbdpDateTo.Text, out _dateTo);
                }
                _isDateApplied = true;
            });
        }

        private bool FilterAlarmText(ServerAlarmCore serverAlarm)
        {
            //parent object
            if (String.IsNullOrEmpty(_filterTextString))
                return true;

            if (serverAlarm.ParentObject != null && serverAlarm.ParentObject.ToLower().Contains(_filterTextString))
            {
                return true;
            }

            if (serverAlarm.Name != null && serverAlarm.Name.ToLower().Contains(_filterTextString))
            {
                return true;
            }
            if (serverAlarm.Description != null && serverAlarm.Description.ToLower().Contains(_filterTextString))
            {
                return true;
            }
            if (serverAlarm.Alarm.AlarmKey.Parameters != null)
            {
                foreach (AlarmParameter alarmParameter in serverAlarm.Alarm.AlarmKey.Parameters)
                {
                    if (alarmParameter.TypeParameter.ToString().ToLower().Contains(_filterTextString))
                        return true;

                    if (alarmParameter.Value == null)
                        continue;

                    if (alarmParameter.Value.ToLower().Contains(_filterTextString))
                        return true;
                }
            }
            return false;
        }

        private bool FilterAlarmDate(Alarm alarm)
        {
            if (!_isDateApplied)
                return true;

            if (alarm.CreatedDateTime.Date >= _dateFrom.Date &&
                 alarm.CreatedDateTime.Date <= _dateTo.Date)
            {
                return true;
            }
            return false;
        }

        private bool FilterAlarmPriority(ServerAlarmCore serverAlarm)
        {
            if (_filterAlarmPriority == null)
            {
                return true;
            }

            if (_chbAndAbove.Checked)
            {
                if ((int) _filterAlarmPriority.AlarmPriority >= (int) serverAlarm.AlarmPriority)
                    return true;
            }
            else if (_filterAlarmPriority.AlarmPriority == serverAlarm.AlarmPriority)
            {
                return true;
            }

            return false;
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            _eFilterText.Text = string.Empty;
            _tbdpDateFrom.Value = null;
            _tbdpDateTo.Value = null;
            _cbFilterAlarmPriority.SelectedItem = string.Empty;
            _isFilterSet = false;
            _lFilteredCount.Visible = false;
            _cbShowOnlyBlockedAlarm.Checked = false;
            _cbAllSubSites.CheckState = CheckState.Checked;
            DoFilterAlarm();
        }

        private bool _isFiltering;
        private bool _isFilterSet;

        private void _bFilter_Click(object sender, EventArgs e)
        {
            if (_eFilterText.Text == string.Empty
                && _tbdpDateFrom.Text == string.Empty
                && _tbdpDateTo.Text == string.Empty
                && !(_cbFilterAlarmPriority.SelectedItem is ItemAlarmPriority)
                && _cbShowOnlyBlockedAlarm.Checked == false
                && _ilbEventSourceObject.Items.Count == 0
                && _cbAllSubSites.CheckState == CheckState.Checked)
            {
                _isFilterSet = false;
                _lFilteredCount.Visible = false;
                DoFilterAlarm();
                return;
            }

            try
            {
                if (!_isFiltering)
                {
                    _isFiltering = true;
                    _isFilterSet = true;
                    DoFilterAlarm();
                    _isFiltering = false;
                    _lFilteredCount.Visible = true;
                }
            }
            catch (Exception error)
            {
                _isFiltering = false;
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void InitAlarmPriority()
        {
            _cbFilterAlarmPriority.Items.Clear();
            _cbFilterAlarmPriority.Items.Add(string.Empty);
            IList<ItemAlarmPriority> list = ItemAlarmPriority.GetList(LocalizationHelper);

            if (list != null && list.Count > 0)
            {
                foreach (ItemAlarmPriority itemAlarmPriority in list)
                {
                    _cbFilterAlarmPriority.Items.Add(itemAlarmPriority);
                }
            }
        }

        private void _eFilterText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ForcedFilterAlarm();
            }
            else
            {
                _timeKeyPressed = DateTime.Now;
                SafeThread.StartThread(PrepareToFiltering);
            }
        }

        private DateTime _timeKeyPressed = DateTime.MinValue;
        private void PrepareToFiltering()
        {
            Thread.Sleep(SLEEP_BEFORE_FILTER_KEY_PRESSED);
            TimeSpan ts = (DateTime.Now - _timeKeyPressed);
            if (ts.TotalMilliseconds > SLEEP_BEFORE_FILTER_KEY_PRESSED)
            {
                ForcedFilterAlarm();
            }
        }

        private void _cbFilterAlarmPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            ForcedFilterAlarm();
        }

        private void _cbShowOnlyBlockedAlarm_CheckedChanged(object sender, EventArgs e)
        {
            ForcedFilterAlarm();
            _wasChecked = _cbShowOnlyBlockedAlarm.Checked;
        }

        private void _bBlock_Click(object sender, EventArgs e)
        {
            if (_showAlarmsTimer != null)
                _showAlarmsTimer.StopTimer();

            _showAlarmsManualResetEvent.Set();

            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            var idsAlarm = GetSelectedAlarmsFromBindingSource();

            if (!idsAlarm.Any())
                return;

            if (idsAlarm.Count == 1)
            {
                if (Dialog.Question(GetString("QuestionBlockAlarm")))
                {
                    CgpClient.Singleton.MainServerProvider.BlockUnblockAlarms(idsAlarm, true);
                }
            }
            else
            {
                if (Dialog.Question(GetString("QuestionBlockAlarms")))
                {
                    CgpClient.Singleton.MainServerProvider.BlockUnblockAlarms(idsAlarm, true);
                }
            }
        }

        private void _bUnblock_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            var idsAlarm = GetSelectedAlarmsFromBindingSource();

            if (!idsAlarm.Any())
                return;

            if (idsAlarm.Count == 1)
            {
                if (Dialog.Question(GetString("QuestionUnblockAlarm")))
                {
                    CgpClient.Singleton.MainServerProvider.BlockUnblockAlarms(idsAlarm, false);
                }
            }
            else
            {
                if (Dialog.Question(GetString("QuestionUnblockAlarms")))
                {
                    CgpClient.Singleton.MainServerProvider.BlockUnblockAlarms(idsAlarm, false);
                }
            }
        }

        private void _tsAcknowledge_Click(object sender, EventArgs e)
        {
            _bConfirmState_Click(sender, e);
        }

        private void _tsBlock_Click(object sender, EventArgs e)
        {
            _bBlock_Click(sender, e);
        }

        private void _tsUnblock_Click(object sender, EventArgs e)
        {
            _bUnblock_Click(sender, e);
        }

        private void _dgValues_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_showAlarmsTimer != null)
                _showAlarmsTimer.StopTimer();

            _showAlarmsTimer = _showAlarmsTimerManager.StartTimeout(
                GeneralOptionsForm.Singleton.GetAlarmListSuspendedRefreshTimeout * 1000,
                null,
                UnlockShowAlarmsTimeout);

            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                if (!_cdgvData.DataGrid.Rows[e.RowIndex].Cells[1].Selected)
                {
                    _cdgvData.DataGrid.CurrentCell = _cdgvData.DataGrid.Rows[e.RowIndex].Cells[1];
                }
                _alarmMenu.Show(MousePosition);
            }
        }

        private void _bDefaultSort_Click(object sender, EventArgs e)
        {
            ShowAlarms(_listServerAlarmsOrigin, true);
        }

        private BindingSource _bindingSourceAlarmInstructions;

        private List<AlarmInstruction> GetAlarmInstrunctions(ServerAlarmCore sererAlarm)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return null;

            try
            {
                var relatedAlarmObjects = sererAlarm.RelatedObjects;

                if (relatedAlarmObjects == null)
                    return null;

                var closestAlarmInstrunctions = new List<AlarmInstruction>();
                var secondClosestAlarmInstrunctions = new List<AlarmInstruction>();

                List<Guid> addedGlobalAlarmInstructionsGuid = new List<Guid>();

                var alarmType = sererAlarm.Alarm.AlarmKey.AlarmType;

                ObjectType? closestParentObject =
                    CgpClient.Singleton.MainServerProvider.AlarmPrioritiesDbs.GetClosestParentObject(
                        alarmType);

                ObjectType? secondClosestParentObject =
                    CgpClient.Singleton.MainServerProvider.AlarmPrioritiesDbs.GetSecondClosestParentObject(
                        alarmType);

                foreach (var relatedAlarmObject in relatedAlarmObjects)
                {
                    if (relatedAlarmObject != null && closestParentObject != ObjectType.NotSupport &&
                        (closestParentObject == ObjectType.AllObjectTypes ||
                         AlarmDetailsForm.CompareObjectTypes(relatedAlarmObject.ObjectType, closestParentObject) ||
                         AlarmDetailsForm.CompareObjectTypes(relatedAlarmObject.ObjectType, secondClosestParentObject)))
                    {
                        AOrmObject aOrmObject = DbsSupport.GetTableObject(
                            relatedAlarmObject.ObjectType,
                            (Guid) relatedAlarmObject.Id);

                        var iOrmObjectWithAlarmInstructions =
                            aOrmObject as IOrmObjectWithAlarmInstructions;
                        if (iOrmObjectWithAlarmInstructions != null)
                        {
                            string localAlarmInstruciton =
                                iOrmObjectWithAlarmInstructions.GetLocalAlarmInstruction();

                            if (!string.IsNullOrEmpty(localAlarmInstruciton))
                            {
                                if (secondClosestParentObject == relatedAlarmObject.ObjectType)
                                    secondClosestAlarmInstrunctions.Add(
                                        new AlarmInstruction(localAlarmInstruciton,
                                            aOrmObject.GetObjectType(),
                                            null));
                                else
                                    closestAlarmInstrunctions.Add(new AlarmInstruction(
                                        localAlarmInstruciton,
                                        aOrmObject.GetObjectType(),
                                        null));
                            }

                            List<GlobalAlarmInstruction> globalAlarmInstructions =
                                CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions
                                    .GetGlobalAlarmInstructionsForObject(aOrmObject.GetObjectType(),
                                        aOrmObject.GetIdString());
                            if (globalAlarmInstructions != null && globalAlarmInstructions.Count > 0)
                            {
                                foreach (GlobalAlarmInstruction globalAlarmInstruction in globalAlarmInstructions)
                                {
                                    if (globalAlarmInstruction != null &&
                                        !addedGlobalAlarmInstructionsGuid.Contains(
                                            globalAlarmInstruction.IdGlobalAlarmInstruction))
                                    {
                                        if (secondClosestParentObject == relatedAlarmObject.ObjectType)
                                            secondClosestAlarmInstrunctions.Add(
                                                new AlarmInstruction(
                                                    globalAlarmInstruction.Instructions,
                                                    aOrmObject.GetObjectType(),
                                                    globalAlarmInstruction));
                                        else
                                            closestAlarmInstrunctions.Add(
                                                new AlarmInstruction(
                                                    globalAlarmInstruction.Instructions,
                                                    aOrmObject.GetObjectType(),
                                                    globalAlarmInstruction));

                                        addedGlobalAlarmInstructionsGuid.Add(
                                            globalAlarmInstruction.IdGlobalAlarmInstruction);
                                    }
                                }
                            }
                        }
                    }
                }

                return new List<AlarmInstruction>(
                    closestAlarmInstrunctions.Concat(
                        secondClosestAlarmInstrunctions));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return null;
            }
        }

        private void ShowAlarmInstructions(List<AlarmInstruction> alarmInstrunctions, string alarmName)
        {
            if (_firstAlarmDialogOpen)
            {
                _firstAlarmDialogOpen = false;
                return;
            }

            if (alarmInstrunctions != null && alarmInstrunctions.Count > 0)
            {
                //Invoke(new Contal.IwQuick.DVoid2Void(
                //delegate()
                //{
                    ShowInstructionsPanel();
                    //splitContainer.Panel2Collapsed = false;
                    _lAlarmInstruction.Text = alarmName;
                    _llAlarmName.Text = alarmName;
                    _bindingSourceAlarmInstructions = new BindingSource {DataSource = alarmInstrunctions};
                    _dgAlarmInstructions.DataSource = _bindingSourceAlarmInstructions;

                    if (_dgAlarmInstructions.Columns.Contains("ObjectIcon"))
                    {
                        _dgAlarmInstructions.Columns["ObjectIcon"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    }

                    if (_dgAlarmInstructions.Columns.Contains("AlarmInstructions"))
                        _dgAlarmInstructions.Columns["AlarmInstructions"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


                    if (_dgAlarmInstructions.Columns.Contains("GlobalAlarmInstructionGuid"))
                        _dgAlarmInstructions.Columns["GlobalAlarmInstructionGuid"].Visible = false;
                //}));
            }
            else
            {
                try
                {
                    Invoke(new DVoid2Void(
                        delegate
                        {
                            HideInstructionsPanel(true);
                            //splitContainer.Panel2Collapsed = true;
                            _lAlarmInstruction.Text = string.Empty;
                            _llAlarmName.Text = string.Empty;
                            _bindingSourceAlarmInstructions = null;
                            _dgAlarmInstructions.DataSource = null;
                        }));
                }
                catch (Exception)
                {
                }
            }
        }

        private void _cdgvData_Click(object sender, EventArgs e)
        {
            _selectServerAlarmForInstruction = null;
            _llAlarmName.Visible = false;

            if (_bindingSource != null && _bindingSource.Count > 0 && _bindingSource.Current != null)
            {
                var serverAlarm = ((DataGridAlarm) _bindingSource.Current).ServerAlarm;

                if (serverAlarm != null)
                {
                    ShowAlarmInstructions(GetAlarmInstrunctions(serverAlarm), serverAlarm.Name);
                }
            }
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            if (_bindingSourceAlarmInstructions != null && _bindingSourceAlarmInstructions.Count > 0)
            {
                var alarmInstruction = _bindingSourceAlarmInstructions[_bindingSourceAlarmInstructions.Position] as AlarmInstruction;
                if (alarmInstruction != null && alarmInstruction.GlobalAlarmInstructionGuid != Guid.Empty)
                {
                    var globalAlarmInstruction = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectById(alarmInstruction.GlobalAlarmInstructionGuid);

                    if (globalAlarmInstruction != null)
                        GlobalAlarmInstructionsForm.Singleton.OpenEditForm(globalAlarmInstruction);
                }
            }
        }

        private bool _isVisibleInstructionsPanel;
        private bool _firstAlarmDialogOpen = true;

        private void _bHideOrShow_Click(object sender, EventArgs e)
        {
            if (_isVisibleInstructionsPanel)
                HideInstructionsPanel(false);
            else
                ShowInstructionsPanel();
        }

        private void ShowInstructionsPanel()
        {
            //if (_selectAlarmForInstruction == null || !ExistsAlarm(_selectAlarmForInstruction))
            //    return;

            splitContainer.SplitterDistance = splitContainer.Width - 250;
            _isVisibleInstructionsPanel = true;
            _bHideOrShow.Image = Resources.hide;
            _bEdit.Visible = true;
            _lAlarmInstruction.Visible = true;
            _dgAlarmInstructions.Visible = true;
            _bHideOrShow.Visible = true;
        }

        private void HideInstructionsPanel(bool hideControlButton)
        {
            splitContainer.Panel2MinSize = _bHideOrShow.Width;
            splitContainer.SplitterDistance = splitContainer.Width - _bHideOrShow.Width;
            _isVisibleInstructionsPanel = false;
            _bHideOrShow.Image = Resources.show;
            _bEdit.Visible = false;
            _lAlarmInstruction.Visible = false;
            _llAlarmName.Visible = false;
            _dgAlarmInstructions.Visible = false;

            if (hideControlButton)
                _bHideOrShow.Visible = false;
        }

        private bool ExistsAlarm(ServerAlarmCore serverAlarm)
        {
            if (_listServerAlarmsOrigin == null || serverAlarm == null)
                return false;

            var idServerAlarm = serverAlarm.IdServerAlarm;

            return _listServerAlarmsOrigin.Any(
                originServerAlarm =>
                    !originServerAlarm.IsBlocked
                    && originServerAlarm.IdServerAlarm.Equals(idServerAlarm));
        }

        private void AlarmsDialog_Load(object sender, EventArgs e)
        {
            HideInstructionsPanel(true);
            ObjectImageList.Singleton.FillWithClientAndPluginsObjectImages(_objectsImageList);
            _ilbEventSourceObject.ImageList = _objectsImageList;
        }

        private void AlarmsDialog_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_isVisibleInstructionsPanel)
                    HideInstructionsPanel(true);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }            
        }

        private void _llAlarmName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_bindingSource == null || _selectServerAlarmForInstruction == null)
                return;

            for (int i = 0; i < _cdgvData.DataGrid.RowCount; i++)
                _cdgvData.DataGrid.Rows[i].Selected = false;

            int index = 0;

            var idSelectServerAlarmForInstruction = _selectServerAlarmForInstruction.IdServerAlarm;

            foreach (DataGridAlarm alarm in _bindingSource)
            {
                if (alarm.ServerAlarm.IdServerAlarm.Equals(idSelectServerAlarmForInstruction))
                {
                    _cdgvData.DataGrid.Rows[index].Selected = true;
                    break;
                }

                index++;
            }
        }

        private bool FilterBySites(ServerAlarmCore serverAlarm, ICollection<int> selectedSites)
        {
            if (selectedSites == null)
                return true;

            if (CgpClient.Singleton.IsConnectionLost(true))
                return false;

            return
                CgpClient.Singleton.MainServerProvider.StructuredSubSites.IsAlarmVisibleForSites(
                    serverAlarm.RelatedObjects,
                    selectedSites);
        }

        private bool FilterByEventSources(ServerAlarmCore serverAlarm)
        {
            if (serverAlarm == null
                || serverAlarm.RelatedObjects == null
                || _selectedRelatedObjects.Count == 0)
                return true;

            if (_rbParametricSearchOr.Checked)
            {
                return serverAlarm.RelatedObjects.Any(obj => _selectedRelatedObjects.Contains(obj));
            }

            return _selectedRelatedObjects.All(obj => serverAlarm.RelatedObjects.Contains(obj));
        }

        private void _bAcknowledgeAndBlock_Click(object sender, EventArgs e)
        {
            _bConfirmState_Click(sender, e);
            _bBlock_Click(sender, e);
        }

        private void _tsAcknowledgeAndBlock_Click(object sender, EventArgs e)
        {
            _bConfirmState_Click(sender, e);
            _bBlock_Click(sender, e);
        }

        private readonly ImageList _objectsImageList = new ImageList();
        private readonly HashSet<IdAndObjectType> _selectedRelatedObjects = new HashSet<IdAndObjectType>();

        private void _bAdd_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            try
            {
                if (_listServerAlarmsOrigin == null)
                    return;

                var relatedObjects = new HashSet<IdAndObjectType>(
                    _listServerAlarmsOrigin.SelectMany(
                        alarm =>
                            alarm.RelatedObjects.Where(
                                idAndObjectType =>
                                    !_selectedRelatedObjects.Contains(idAndObjectType))
                        ));

                if (relatedObjects.Count == 0)
                    return;

                var listModObj = CgpClient.Singleton.MainServerProvider.GetModifyObjects(relatedObjects);
                var formAdd = new ListboxFormAdd(listModObj,
                    CgpClient.Singleton.LocalizationHelper.GetString("EventlogsForm_AddEventSourceObjectText"));
                ListOfObjects outModObjects;
                formAdd.ShowDialogMultiSelect(out outModObjects);
                if (outModObjects != null && outModObjects.Count > 0)
                {
                    foreach (IModifyObject obj in outModObjects)
                    {
                        if (obj != null)
                        {
                            _selectedRelatedObjects.Add(new IdAndObjectType(obj.GetId, obj.GetOrmObjectType));
                            var aOrmObject = DbsSupport.GetTableObject(obj.GetOrmObjectType, obj.GetId.ToString());
                            _ilbEventSourceObject.Items.Add(new ImageListBoxItem(aOrmObject,
                                aOrmObject.GetObjectType().ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void _bRemove_Click(object sender, EventArgs e)
        {
            if (_ilbEventSourceObject.SelectedItem != null)
            {
                string question = String.Format("{0} {1}?", GetString("QuestionDeleteParametricSearchItem"),
                    _ilbEventSourceObject.SelectedItem.ToString());

                bool dialogResult = Dialog.WarningQuestion(question);
                if (dialogResult)
                {
                    var aormObj = (AOrmObject)(((ImageListBoxItem)_ilbEventSourceObject.SelectedItem).MyObject);
                    _selectedRelatedObjects.Remove(new IdAndObjectType(aormObj.GetId(), aormObj.GetObjectType()));
                    _ilbEventSourceObject.Items.Remove(_ilbEventSourceObject.SelectedItem);
                }     
            }
        }

        private void _bClearList_Click(object sender, EventArgs e)
        {
            _ilbEventSourceObject.Items.Clear();
            _selectedRelatedObjects.Clear();
        }

        private void _ilbEventSourceObject_DoubleClick(object sender, EventArgs e)
        {
            _bAdd_Click(this, e);
        }

        private void _ilbEventSourceObject_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _ilbEventSourceObject.SelectedIndex = _ilbEventSourceObject.IndexFromPoint(e.X, e.Y);
                _bRemove_Click(sender, e);
            }
        }

        private void _ilbEventSourceObject_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _ilbEventSourceObject_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null)
                    return;
                var ormObject = e.Data.GetData(output[0]) as AOrmObject;

                bool added = false;

                if (ormObject != null)
                {
                    IdAndObjectType idObjectType = new IdAndObjectType(ormObject.GetId(), ormObject.GetObjectType());

                    var relatedObjects = new HashSet<IdAndObjectType>(
                        _listServerAlarmsOrigin.SelectMany(
                            alarm =>
                                alarm.RelatedObjects.Where(
                                    idAndObjectType =>
                                        !_selectedRelatedObjects.Contains(idAndObjectType))
                            ));

                    if (relatedObjects.Count == 0)
                        return;

                    if (relatedObjects.Contains(idObjectType))
                    {
                        if (!_selectedRelatedObjects.Contains(idObjectType))
                        {
                            _selectedRelatedObjects.Add(idObjectType);
                            _ilbEventSourceObject.Items.Add(new ImageListBoxItem(ormObject, ormObject.GetObjectType().ToString()));
                            CgpClientMainForm.Singleton.AddToRecentList(ormObject);
                        }
                        else
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ilbEventSourceObject,
                                GetString("ErrorObjectAlreadyAdded"), ControlNotificationSettings.Default);
                        }

                        added = true;
                    }
                }

                if (!added)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _ilbEventSourceObject,
                       GetString("ErrorWrongObjectType"),
                       ControlNotificationSettings.Default);
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void _chbAndAbove_CheckedChanged(object sender, EventArgs e)
        {
            ForcedFilterAlarm();
        }
    }

    public class ItemAlarmPriority
    {
        private readonly AlarmPriority _alarmPriority;
        private readonly LocalizationHelper _localizationHelper;

        public AlarmPriority AlarmPriority { get { return _alarmPriority; } }

        public ItemAlarmPriority(AlarmPriority alarmPriority, LocalizationHelper localizationHelper)
        {
            _alarmPriority = alarmPriority;
            _localizationHelper = localizationHelper;
        }

        public override string ToString()
        {
            return _localizationHelper.GetString(
                string.Format(
                    "AlarmPriority_{0}",
                    _alarmPriority));
        }

        public static IList<ItemAlarmPriority> GetList(LocalizationHelper localizationHelper)
        {
            return EnumHelper.ListAllValuesWithProcessing<AlarmPriority, ItemAlarmPriority>(
                value => new ItemAlarmPriority(value, localizationHelper) );
        }
    }
}
