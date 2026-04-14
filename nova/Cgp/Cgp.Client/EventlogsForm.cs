using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.StructuredSubSites;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Threads;
using Contal.Cgp.Globals;
using Contal.IwQuick.UI;
using Contal.IwQuick.Sys;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.Cgp.Client
{
    public enum SelectedTimeOfDay : byte
    {
        Unknown, StartOfDay, Delta, EndOfDay
    }

    public partial class EventlogsForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<Eventlog, Eventlog>
#endif
    {
        int _firstLoadedIndex = 0;
        int _lastLoadedIndex = 0;
        int _firstVisibleRow = 0;

        int _pagesPreloaded = 10;

        private bool _columnsSet = false;
        private bool _wasAborted = false;
        private double _lastQueryDelay;
        private bool _isEnableExportButton = false;
        private bool _isOpenEventlogsExportForm = false;
        private int _insertCounter = 0;
        private readonly ImageList _objectsImageList = new ImageList();
        private readonly SelectingOfSubSitesSupport _selectingOfSubSitesSupport;

        public EventlogsForm()
        {
            InitializeComponent();
            _tbdpDateToFilter.LocalizationHelper = LocalizationHelper;
            _tbdpDateFromFilter.LocalizationHelper = LocalizationHelper;
            ConsiderPagesPreloaded();
            _tbdpDateFromFilter.Value = DateTime.Now.Date;
            _rbParametricSearchOr.Checked = true;
            _rbFullTextSearchAnd.Checked = true;
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            SetFilterSettings();
            LoadDataOnFormEnter = false;
            _bStop.Enabled = false;
            DisableExportButton();
            InitCGPDataGridView();
            _cbAllSubSites.CheckState = CheckState.Checked;
            _selectingOfSubSitesSupport = new SelectingOfSubSitesSupport(_wpfCheckBoxTreeView, _cbAllSubSites);
            _selectingOfSubSitesSupport.SelectedSitesChanged += () => FilterValueChanged(null, null);

            if (HasAccessExport() && !_isOpenEventlogsExportForm)
                _bExportExcel.Enabled = true;

            FormOnEnter += Form_Enter;
            SetReferenceEditColors();
        }

        private void Form_Enter(Form form)
        {
            RefreshReporterFilterSettings();
            _selectingOfSubSitesSupport.LoadSubSites();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.DataGrid.DataError += _dgValues_DataError;
            _cdgvData.DataGrid.Scroll += _dgValues_Scroll;
            _cdgvData.DataGrid.MouseDoubleClick += _dgValues_CellMouseDoubleClick;
        }

        protected override void AfterTranslateForm()
        {
            FillEventLogTypes(true);
        }

        protected override void WarningNotActualData()
        {
            if (_insertCounter != CgpClient.Singleton.MainServerProvider.Eventlogs.GetInsertCounter())
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _bRefresh,
                    GetString("WarningNotActualEventlogsData"), new ControlNotificationSettings(NotificationParts.All, HintPosition.RightBottom));
            }
        }

        private void EventlogsForm_Load(object sender, EventArgs e)
        {
            FillEventLogTypes(true);
            //load icons for _ilbEventSourceObject
            ObjectImageList.Singleton.FillWithClientAndPluginsObjectImages(_objectsImageList);
            _objectsImageList.ColorDepth = ColorDepth.Depth32Bit;
            _objectsImageList.ImageSize = new Size(16, 16);
            _ilbEventSourceObject.ImageList = _objectsImageList;

            _ilbEventSourceObject.Cursor = Cursors.Hand;
        }

        private class EventTypeItem
        {
            public bool IsChecked { get; set; }
            public string Value { get; set; }
        }

        private readonly Dictionary<string, EventTypeItem> _eventTypeValues = new Dictionary<string, EventTypeItem>();

        private void FillEventLogTypes(bool initValue)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            _eventTypeValues.Clear();
            _chblType.Items.Clear();
            var eventTypes = CgpClient.Singleton.MainServerProvider.Eventlogs.GetEventLogTypes();
            var eventTypeValues = new Dictionary<string, string>();

            if (eventTypes != null && eventTypes.Count > 0)
            {
                foreach (string eventType in eventTypes)
                {
                    string localisedType = GetString(GetEventTypeFormatForLocalisation(eventType));

                    if (!eventTypeValues.ContainsKey(localisedType))
                        eventTypeValues.Add(localisedType, eventType);
                }
                //... Sorting by localisedType key.
                var items = from pair in eventTypeValues
                            orderby pair.Key ascending
                            select pair;

                _eventTypeValues.Clear();

                foreach (KeyValuePair<string, string> pair in items)
                {
                    _chblType.Items.Add(pair.Key, initValue);
                    _eventTypeValues.Add(pair.Key, new EventTypeItem() { IsChecked = initValue, Value = pair.Value });
                }
            }
        }

        private string GetEventTypeFormatForLocalisation(string input)
        {
            var result = new StringBuilder();
            string[] words = input.Split(' ', '-');
            foreach (string word in words)
            {
                result.Append(char.ToUpper(word[0]) + word.Substring(1));
            }
            return result.ToString();
        }

        private static volatile EventlogsForm _singleton = null;
        private static readonly object _syncRoot = new object();

        public static EventlogsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new EventlogsForm { MdiParent = CgpClientMainForm.Singleton };
                        }
                    }

                return _singleton;
            }
        }

        protected override Eventlog GetObjectForEdit(Eventlog listObj, out bool editAllowed)
        {
            editAllowed = false;
            return listObj;
        }

        protected override Eventlog GetFromShort(Eventlog listObj)
        {
            return listObj;
        }

        protected override Eventlog GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.Eventlogs.GetObjectById((long)idObj);
        }

        public void BlockShowData()
        {
            SuspendShowData();
        }

        public void UnblockShowData()
        {
            ResumeShowData();
        }

        #region Filters

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            if (IsChangedFilerValues && (_filterSettings == null || _filterSettings.Count == 0))
                if (!Dialog.WarningQuestion(GetString("EventLogOperationWarning")))
                    return;

            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                if (IsChangedFilerValues)
                {
                    _lRecordCount.Visible = false;
                    _cdgvData.DataGrid.DataSource = null;
                    _bRunFilter.Enabled = false;
                    _bFilterClear.Enabled = false;
                    _bRefresh.Enabled = false;
                }

                RunFilter();
            }
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                _filterSettings.Clear();
                ClearFilterEdits();
            }
        }

        protected override void ClearFilterEdits()
        {
            //_cbType.SelectedItem = string.Empty;
            _tbdpDateFromFilter.Value = null;
            _tbdpDateToFilter.Value = null;
            _eCGPSourceFilter.Text = "";
            _eEventSourceFilter.Text = "";
            _eDescriptionFilter.Text = "";
            _ilbEventSourceObject.Items.Clear();
            _cbAllSubSites.CheckState = CheckState.Checked;
        }

        private void ConsiderPagesPreloaded()
        {
            if (_pagesPreloaded < 0)
                _pagesPreloaded = -_pagesPreloaded;
            if (_pagesPreloaded == 0)
                _pagesPreloaded = 2;
            if (_pagesPreloaded % 2 != 0)
                _pagesPreloaded += 1;
        }

        private bool FilterCheckDates(bool checkBiggerDateTo)
        {
            if (_tbdpDateFromFilter.Value == null || _tbdpDateToFilter.Value == null)
                return true;

            if (_tbdpDateFromFilter.Value > _tbdpDateToFilter.Value && checkBiggerDateTo)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbdpDateToFilter.TextBox,
                    GetString("ErrorDateTo"), ControlNotificationSettings.Default);
                _tbdpDateToFilter.TextBox.Focus();
                return false;
            }

            return true;
        }

        protected override bool CheckFilterValues()
        {
            return FilterCheckDates(true);
        }

        protected override void SetFilterSettings()
        {
            if (_tbdpDateFromFilter.Text != "")
            {
                DateTime dateFrom = DateTime.Parse(_tbdpDateFromFilter.Text);

                FilterSettings filterSetting = new FilterSettings(Eventlog.COLUMN_EVENTLOG_DATE_TIME, dateFrom, ComparerModes.EQUALLMORE);
                _filterSettings.Add(filterSetting);
            }

            if (_tbdpDateToFilter.Text != "")
            {
                DateTime dateTo = DateTime.Parse(_tbdpDateToFilter.Text);
                dateTo = dateTo.AddMilliseconds(999);

                FilterSettings filterSetting = new FilterSettings(Eventlog.COLUMN_EVENTLOG_DATE_TIME, dateTo, ComparerModes.EQUALLLESS);
                _filterSettings.Add(filterSetting);
            }

            if (_tcSearch.SelectedTab == _tpFulltextSearch)
            {
                LogicalOperators logicalOperator = LogicalOperators.AND;

                if (_rbFullTextSearchOr.Checked)
                    logicalOperator = LogicalOperators.OR;

                if (_eCGPSourceFilter.Text != "")
                {
                    FilterSettings filterSetting = new FilterSettings(Eventlog.COLUMN_CGPSOURCE, _eCGPSourceFilter.Text, ComparerModes.LIKEBOTH);
                    _filterSettings.Add(filterSetting);
                }

                if (_eEventSourceFilter.Text != "")
                {
                    FilterSettings filterSetting = new FilterSettings(Eventlog.COLUMN_EVENTSOURCES, _eEventSourceFilter.Text, ComparerModes.LIKEBOTH, logicalOperator);
                    _filterSettings.Add(filterSetting);
                }

                if (_eDescriptionFilter.Text != "")
                {
                    FilterSettings filterSetting = new FilterSettings(Eventlog.COLUMN_DESCRIPTION, _eDescriptionFilter.Text, ComparerModes.LIKEBOTH, logicalOperator);
                    _filterSettings.Add(filterSetting);
                }
            }
            else if (_tcSearch.SelectedTab == _tpParametricSearch)
            {
                if (_ilbEventSourceObject.Items != null && _ilbEventSourceObject.Items.Count > 0)
                {
                    var guids = new List<Guid>();

                    foreach (var obj in _ilbEventSourceObject.Items)
                    {
                        var ilbItem = obj as ImageListBoxItem;

                        if (ilbItem != null)
                        {
                            var aOrmObject = ilbItem.MyObject as AOrmObject;
                            if (aOrmObject != null)
                            {
                                guids.Add((Guid)aOrmObject.GetId());
                            }
                        }
                    }

                    if (guids.Count > 0)
                    {
                        if (_rbParametricSearchOr.Checked)
                        {
                            var filterSetting = new FilterSettings(Eventlog.COLUMN_EVENTSOURCES, guids, ComparerModes.IN);
                            _filterSettings.Add(filterSetting);
                        }
                        else
                        {
                            var filterSetting = new FilterSettings(Eventlog.COLUMN_EVENTSOURCES, guids, ComparerModes.EQUALL);
                            _filterSettings.Add(filterSetting);
                        }
                    }
                }
            }
            else if (_tcSearch.SelectedTab == _tpSubSitesFilter)
            {

            }

            if (_chblType.CheckedItems.Count < _eventTypeValues.Count)
            {
                var eventlogTypes = new LinkedList<string>();

                foreach (var item in _eventTypeValues.Where((item) => item.Value.IsChecked))
                {
                    eventlogTypes.AddLast(item.Value.Value);
                }

                if (eventlogTypes.Count == 0)
                    eventlogTypes.AddLast(string.Empty);

                var filterSetting = new FilterSettings(Eventlog.COLUMN_TYPE, eventlogTypes, ComparerModes.IN);
                _filterSettings.Add(filterSetting);
            }
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            if (sender is DateTimePicker)
            {
                if (_tbdpDateToFilter.Value != null && _tbdpDateToFilter.Value.Value == _tbdpDateToFilter.Value.Value.Date)
                {
                    DateTime dateTo = _tbdpDateToFilter.Value.Value.Date;
                    dateTo = dateTo.AddHours(24);
                    dateTo = dateTo.AddMilliseconds(-1);

                    _tbdpDateToFilter.Value = dateTo;
                }
            }
            base.FilterValueChanged(sender, e);
        }

        #endregion

        #region ShowData

        protected override ICollection<Eventlog> GetData()
        {
            StartProgress();
            StartCountTime();
            _ilbEventSourceObject.ImageList = ObjectImageList.Singleton.GetAllObjectImages();

            if (_cdgvData.DataGrid.DataSource == null)
                CreateDGVBindingSource();
            else
                UpdateDGVBindingSource();

            _firstVisibleRow = 0;
            _firstLoadedIndex = 0;
            _lastLoadedIndex = 0;

            var bindingSource = _cdgvData.DataGrid.DataSource as BindingSource;

            return
                bindingSource != null
                    ? bindingSource.DataSource as List<Eventlog>
                    : null;
        }

        protected override bool SortEnabled()
        {
            return false;
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            this.InvokeInUI(() =>
            {
                _bRunFilter.Enabled = false;
                _bFilterClear.Enabled = false;
                _bRefresh.Enabled = false;
                if (!_columnsSet)
                {
                    SetColumnsVisualParameters();
                    _columnsSet = true;
                }
                SetReporterVisible();
            });

            LoadData();
            SetEventSources();

            this.InvokeInUI(() =>
            {
                CgpClient.Singleton.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvData.DataGrid);

                _bRunFilter.Enabled = true;
                _bFilterClear.Enabled = true;
                _bRefresh.Enabled = true;
            });

            StopCountTimeAndShowInfo();
            StopProgress();
        }

        private void SetColumnsVisualParameters()
        {
            this.InvokeInUI(() =>
            {
                if (!_cdgvData.DataGrid.Columns.Contains("strEventSources"))
                {
                    _cdgvData.DataGrid.Columns.Add("strEventSources", "EventSources");
                    _cdgvData.DataGrid.Columns["strEventSources"].DisplayIndex = _cdgvData.DataGrid.Columns["EventSources"].DisplayIndex;
                }

                SetWidthColumn(_cdgvData.DataGrid, Eventlog.COLUMN_TYPE, 150);

                _cdgvData.DataGrid.Columns[Eventlog.COLUMN_EVENTLOG_DATE_TIME].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss"; //added seconds                
                SetWidthColumn(_cdgvData.DataGrid, Eventlog.COLUMN_EVENTLOG_DATE_TIME, 150);

                SetWidthColumn(_cdgvData.DataGrid, Eventlog.COLUMN_CGPSOURCE, 150);
                SetWidthColumn(_cdgvData.DataGrid, "strEventSources", 400);

                HideColumnDgw(_cdgvData.DataGrid, Eventlog.COLUMN_EVENTSOURCES);
                HideColumnDgw(_cdgvData.DataGrid, Eventlog.COLUMN_ID_EVENTLOG);
                HideColumnDgw(_cdgvData.DataGrid, Eventlog.COLUMN_EVENTLOG_PARAMETERS);

                _cdgvData.DataGrid.AutoGenerateColumns = false;
                _cdgvData.DataGrid.AllowUserToAddRows = false;

                if (_cdgvData.DataGrid.Columns.Contains(Eventlog.COLUMN_DESCRIPTION))
                {
                    _cdgvData.DataGrid.Columns[Eventlog.COLUMN_DESCRIPTION].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    _cdgvData.DataGrid.Columns[Eventlog.COLUMN_DESCRIPTION].MinimumWidth = 100;
                }
            });
        }

        private void SetReporterVisible()
        {
            if (GeneralOptions.Singleton.EventlogListReporter)
            {
                ShowColumnDgw(_cdgvData.DataGrid, Eventlog.COLUMN_CGPSOURCE);
            }
            else
            {
                HideColumnDgw(_cdgvData.DataGrid, Eventlog.COLUMN_CGPSOURCE);
            }
        }

        private void RefreshReporterFilterSettings()
        {
            if (!GeneralOptions.Singleton.EventlogListReporter)
            {
                _lCGPSourceFilter.Visible = false;
                _eCGPSourceFilter.Visible = false;
                _eCGPSourceFilter.Text = string.Empty;
            }
            else
            {
                _lCGPSourceFilter.Visible = true;
                _eCGPSourceFilter.Visible = true;
            }
        }

        private void UpdateDGVBindingSource()
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (_cdgvData.DataGrid.DataSource is BindingSource && ((_cdgvData.DataGrid.DataSource as BindingSource).DataSource is List<Eventlog>))
            {
                Exception ex;
                int logsCount = CgpClient.Singleton.MainServerProvider.Eventlogs.SelectCount(
                    _filterSettings,
                    _selectingOfSubSitesSupport.GetSelectedSiteIds(),
                    out ex);

                if (_eventLogs.Count > logsCount)
                {
                    _eventLogs.RemoveRange(0, _eventLogs.Count - logsCount);
                }
                else if (_eventLogs.Count < logsCount)
                {
                    Eventlog emptyRow = new Eventlog("__", DateTime.Now, "__", "__");

                    for (int i = logsCount - _eventLogs.Count; i > 0; i--)
                    {
                        _eventLogs.Add(emptyRow);
                    }
                }

                this.InvokeInUI(() =>
                {
                    ShowRowCount(logsCount);
                    var bindingSource = (_cdgvData.DataGrid.DataSource as BindingSource);
                    if (bindingSource != null)
                        bindingSource.ResetBindings(true);
                });
            }
            else
            {
                CreateDGVBindingSource();
            }
        }

        private int _rowCount = 0;

        private void ShowRowCount(int rowCount)
        {
            Thread2UI.Invoke(this, () =>
            {
                try
                {
                    _rowCount = rowCount;
                    string strRowCount = rowCount.ToString(CultureInfo.InvariantCulture);
                    int strRowCountLength = strRowCount.Length;
                    string strRowCountWithSpace;
                    if (strRowCountLength > 3)
                    {
                        strRowCountWithSpace = strRowCount.Substring(strRowCountLength - 3);
                        int pos = strRowCountLength - 3;
                        do
                        {
                            pos -= 3;
                            if (pos > 0)
                            {
                                strRowCountWithSpace = strRowCount.Substring(pos, 3) + " " + strRowCountWithSpace;
                            }
                            else
                            {
                                strRowCountWithSpace = strRowCount.Substring(0, pos + 3) + " " + strRowCountWithSpace;
                            }
                        } while (pos > 0);
                    }
                    else
                    {
                        strRowCountWithSpace = strRowCount;
                    }

                    _lRecordCount.Text = GetString("TextRecordCount") + " : " + strRowCountWithSpace;
                    _lRecordCount.Left = _tcSearch.Right - _lRecordCount.Width;
                    _lRecordCount.Visible = true;
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                }
            }, true);
        }

        private readonly List<Eventlog> _eventLogs = new List<Eventlog>();

        private void CreateDGVBindingSource()
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            Exception ex;
            BindingSource bsValues = new BindingSource();
            _eventLogs.Clear();
            int logsCount = CgpClient.Singleton.MainServerProvider.Eventlogs.SelectCount(
                _filterSettings,
                _selectingOfSubSitesSupport.GetSelectedSiteIds(),
                out ex);

            Eventlog emptyRow = new Eventlog("__", DateTime.Now, "__", "__");

            for (int i = 0; i < logsCount; i++)
            {
                _eventLogs.Add(emptyRow);
            }

            this.InvokeInUI(() =>
            {
                ShowRowCount(logsCount);
                bsValues.DataSource = _eventLogs;
                _cdgvData.DataGrid.DataSource = bsValues;
            });
        }

        private void LoadData()
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(true))
                return;
            try
            {
                this.InvokeInUI(() =>
                {
                    int displayedRows = _cdgvData.DataGrid.Rows.GetRowCount(DataGridViewElementStates.Displayed);

                    _firstLoadedIndex = _firstVisibleRow - ((_pagesPreloaded / 2) * displayedRows) >= 0 ?
                        _firstVisibleRow - ((_pagesPreloaded / 2) * displayedRows) : 0;
                    _lastLoadedIndex = _firstVisibleRow + (((_pagesPreloaded / 2) + 1) * displayedRows) <= _cdgvData.DataGrid.RowCount ?
                        _firstVisibleRow + (((_pagesPreloaded / 2) + 1) * displayedRows) : _cdgvData.DataGrid.RowCount;
                });

                ICollection<Eventlog> eventlogs = new List<Eventlog>();
                if (_lastLoadedIndex > 0)
                {
                    _insertCounter = CgpClient.Singleton.MainServerProvider.Eventlogs.GetInsertCounter();
                    eventlogs = CgpClient.Singleton.MainServerProvider.Eventlogs.SelectRangeByCriteria(
                        _filterSettings,
                        _selectingOfSubSitesSupport.GetSelectedSiteIds(),
                        _firstLoadedIndex,
                        _lastLoadedIndex - _firstLoadedIndex);
                }

                this.InvokeInUI(() =>
                {
                    if (eventlogs != null && eventlogs.Count > 0)
                    {
                        var bindingSource = _cdgvData.DataGrid.DataSource as BindingSource;

                        if (bindingSource != null)
                        {
                            var dataSource = bindingSource.DataSource as List<Eventlog>;
                            if (dataSource != null)
                            {
                                dataSource.RemoveRange(_firstLoadedIndex, _lastLoadedIndex - _firstLoadedIndex);
                                dataSource.InsertRange(_firstLoadedIndex, eventlogs);
                            }
                        }
                    }
                    else
                    {
                        RemoveGridView();
                    }
                });
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
            PrePaintDGV();
        }

        private void SetEventSources()
        {
            try
            {
                Eventlog log = null;
                List<Eventlog> eventLogs = new List<Eventlog>();

                this.InvokeInUI(() =>
                {
                    for (int i = _firstLoadedIndex; i < _lastLoadedIndex; i++)
                    {
                        log = _cdgvData.DataGrid.Rows[i].DataBoundItem as Eventlog;
                        if (log == null)
                            continue;
                        eventLogs.Add(log);
                    }
                });

                var names =
                    new Dictionary<long, ICollection<string>>();

                if (eventLogs.Count > 0)
                    names =
                        CgpClient.Singleton.MainServerProvider.Eventlogs
                            .GetEventSourceNames(
                                eventLogs
                                    .Where(eventlog => eventlog != null)
                                    .Select(eventlog => eventlog.IdEventlog)
                                    .ToList());

                this.InvokeInUI(() =>
                {
                    for (int i = _firstLoadedIndex; i < _lastLoadedIndex; i++)
                    {
                        log = _cdgvData.DataGrid.Rows[i].DataBoundItem as Eventlog;

                        if (log == null)
                            continue;

                        var cellValue = new StringBuilder();

                        ICollection<string> eventLogNames;

                        if (!names.TryGetValue(log.IdEventlog, out eventLogNames))
                            continue;

                        if (eventLogNames == null)
                            continue;

                        foreach (string name in eventLogNames)
                            cellValue.Append(name + ",");

                        if (cellValue[cellValue.Length - 1] == ',')
                            cellValue.Length = cellValue.Length - 1;

                        SetCellValue(
                            _cdgvData.DataGrid.Rows[i].Cells[
                                "strEventSources"],
                            cellValue.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void SetCellValue(DataGridViewCell cell, string value)
        {
            this.InvokeInUI(() =>
            {
                cell.Value = value;
            });
        }

        private void PrePaintDGV()
        {
            this.InvokeInUI(() => _cdgvData.DataGrid.Invalidate(true));
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        private void _bParameters_Click(object sender, EventArgs e)
        {
            if (_cdgvData.DataGrid.DataSource is BindingSource && ((_cdgvData.DataGrid.DataSource as BindingSource).Current is Eventlog))
                OpenEditForm((_cdgvData.DataGrid.DataSource as BindingSource).Current as Eventlog);
        }

        protected override ACgpEditForm<Eventlog> CreateEditForm(Eventlog obj, ShowOptionsEditForm showOption)
        {
            return new EventlogParametersForm(obj);
        }

        protected override bool Compare(Eventlog obj1, Eventlog obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void DeleteObj(Eventlog obj)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteObjById(object objId)
        {
            throw new NotImplementedException();
        }

        private void _dgValues_CellMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_cdgvData.DataGrid.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            if (_cdgvData.DataGrid.DataSource is BindingSource && ((_cdgvData.DataGrid.DataSource as BindingSource).Current is Eventlog))
            {
                //Eventlog eventLog = CgpClient.Singleton.MainServerProvider.Eventlogs.GetObjectById(((_dgValues.DataSource as BindingSource).Current as Eventlog).IdEventlog, out ex);
                Eventlog eventLog = (_cdgvData.DataGrid.DataSource as BindingSource).Current as Eventlog;
                OpenEditForm(eventLog);
            }
        }

        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        private void _bClearDateFrom_Click(object sender, EventArgs e)

        {
            if (_tbdpDateFromFilter.Text != string.Empty)
                _tbdpDateFromFilter.Value = null;
        }


        private void _bClearDateTo_Click(object sender, EventArgs e)

        {
            if (_tbdpDateToFilter.Text != string.Empty)
                _tbdpDateToFilter.Value = null;
        }
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local

        private const int DAY_WITHOUT_MILISECOND = 86399999;
        private void _bFilterDateOneDay_Click(object sender, EventArgs e)
        {
            if (!FilterCheckDates(false))
                return;

            if (_tbdpDateFromFilter.Value == null && _tbdpDateToFilter.Value == null)
            {
                _bToDay_Click(null, null);
            }
            else if (_tbdpDateFromFilter.Value != null)
            {
                DateTime dtFrom = new DateTime(_tbdpDateFromFilter.Value.Value.Year,
                    _tbdpDateFromFilter.Value.Value.Month,
                    _tbdpDateFromFilter.Value.Value.Day);

                _tbdpDateFromFilter.Value = dtFrom;
                _tbdpDateToFilter.Value = dtFrom.AddMilliseconds(DAY_WITHOUT_MILISECOND);
            }
            else if (_tbdpDateFromFilter.Value == null && _tbdpDateToFilter.Value != null)
            {
                DateTime dtFrom = new DateTime(_tbdpDateToFilter.Value.Value.Year,
                    _tbdpDateToFilter.Value.Value.Month,
                    _tbdpDateToFilter.Value.Value.Day);

                _tbdpDateFromFilter.Value = dtFrom;
                _tbdpDateToFilter.Value = dtFrom.AddMilliseconds(DAY_WITHOUT_MILISECOND);
            }
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Eventlogs.HasAccessView();

                return false;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        public override bool HasAccessView(Eventlog eventlog)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Eventlogs.HasAccessViewForObject(eventlog);

                return false;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        public static bool HasAccessExport()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Eventlogs.HasAccessExport();

                return false;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        private new void RefreshData()
        {
            StartProgress();
            StartCountTime();
            UpdateDGVBindingSource();
            LoadData();
            SetEventSources();
            if (!_columnsSet)
            {
                SetColumnsVisualParameters();
                _columnsSet = true;
            }
            SetReporterVisible();
            StopCountTimeAndShowInfo();
            StopProgress();
        }

        private readonly Stopwatch _stopWatch = new Stopwatch();

        private void StartCountTime()
        {
            _stopWatch.Reset();
            _stopWatch.Start();

            DisableExportButton();
        }

        private void StopCountTimeAndShowInfo()
        {
            if (_stopWatch == null)
                return;

            long miliseconds = _stopWatch.ElapsedMilliseconds;
            _lastQueryDelay = (double)miliseconds / 1000;

            ShowRowCount(_rowCount);
            this.InvokeInUI(() =>
            {
                if (_wasAborted && _cdgvData.DataGrid.RowCount > 0)
                    _wasAborted = false;

                if (_wasAborted)
                {
                    _wasAborted = false;
                    _lRecordCount.Text = string.Empty;
                }
                else
                {
                    _lRecordCount.Text = _lRecordCount.Text + " (" + Math.Round(_lastQueryDelay, 3) + " " + GetString("Seconds") + ")";
                    _lRecordCount.Left = _tcSearch.Right - _lRecordCount.Width;

                    if (_rowCount > 0)
                    {
                        EnableExportButton();
                    }
                }
            });
        }

        private bool LoadAndUpdateData(TimerCarrier timerCarrier)
        {
            int displayedRows = _cdgvData.DataGrid.Rows.GetRowCount(DataGridViewElementStates.Displayed);
            if (_firstVisibleRow < _firstLoadedIndex || (_firstVisibleRow + displayedRows) > _lastLoadedIndex)
            {
                StartProgress();
                StartCountTime();
                LoadData();
                SetEventSources();
                StopCountTimeAndShowInfo();
                StopProgress();
            }
            return true;
        }

        private void StartProgress()
        {
            this.InvokeInUI(() =>
            {
                _pDGVBack.Visible = true;
                _pbLoading.Visible = true;
                _bStop.Enabled = true;
            });
        }

        private void StopProgress()
        {
            this.InvokeInUI(() =>
            {
                _pDGVBack.Visible = false;
                _pbLoading.Visible = false;
                _bStop.Enabled = false;
            });
        }

        ITimer _scrollTimerCarrier;
        private void _dgValues_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                return;

            if (_scrollTimerCarrier != null)
                _scrollTimerCarrier.StopTimer();
            _firstVisibleRow = e.NewValue;
            _scrollTimerCarrier = TimerManager.Static.StartTimeout(500, LoadAndUpdateData);
        }

        private void _dgValues_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        // ReSharper disable once UnusedMember.Local
        private BindingSource PrepareBindingSource(int startIndex, ICollection<Eventlog> data, int eventLogsCount)
        {
            List<Eventlog> eventLogs = new List<Eventlog>();
            for (int i = 0; i < startIndex; i++)
            {
                eventLogs.Add(new Eventlog());
            }
            for (int i = 0; i < data.Count; i++)
            {
                eventLogs.Add(data.ElementAt(i));
            }
            for (int i = 0; i < eventLogsCount; i++)
            {
                eventLogs.Add(new Eventlog());
            }
            var returnValue = new BindingSource { DataSource = eventLogs };
            return returnValue;
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            if (IsChangedFilerValues || _filterSettings == null || _filterSettings.Count == 0)
                if (!Dialog.WarningQuestion(GetString("EventLogOperationWarning")))
                    return;
            //run filter if was changed , otherwise refresh 
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                if (IsChangedFilerValues)
                {
                    _lRecordCount.Visible = false;
                    _cdgvData.DataGrid.DataSource = null;
                    _bRunFilter.Enabled = false;
                    _bFilterClear.Enabled = false;
                    _bRefresh.Enabled = false;
                    RunFilter();
                }
                else
                {
                    _lRecordCount.Visible = false;
                    SafeThread.StartThread(RefreshData);
                }
            }
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            var listModObj = new List<IModifyObject>();
            var addedEventSoruceObjects = GetEventSourceObjects();

            var listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.Person);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.Card);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.CardSystem);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.CardReader);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.CCU);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.DCU);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.Input);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.Output);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.DoorEnvironment);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            listFromDatabase = DbsSupport.GetIModifyObjects(ObjectType.AlarmArea);
            AddToModifyObjects(addedEventSoruceObjects, listFromDatabase, listModObj);

            var formAdd = new ListboxFormAdd(listModObj, CgpClient.Singleton.LocalizationHelper.GetString("EventlogsForm_AddEventSourceObjectText"));
            ListOfObjects outModObjects;
            formAdd.ShowDialogMultiSelect(out outModObjects);

            if (outModObjects != null && outModObjects.Count > 0)
            {
                foreach (IModifyObject obj in outModObjects)
                {
                    if (obj != null)
                    {
                        AOrmObject aOrmObject = DbsSupport.GetTableObject(obj.GetOrmObjectType, obj.GetId.ToString());
                        AddToEventSourceObject(aOrmObject);
                    }
                }
            }
        }

        private void AddToModifyObjects(
            List<string> addedEventSourceObjects,
            [NotNull] ICollection<IModifyObject> listFromDatabase,
            List<IModifyObject> listModObj)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (listFromDatabase == null || listFromDatabase.Count == 0)
                return;

            foreach (IModifyObject iModifyObject in listFromDatabase)
            {
                if (listModObj != null && iModifyObject != null)
                {
                    if (addedEventSourceObjects == null || !addedEventSourceObjects.Contains(iModifyObject.GetId.ToString()))
                        listModObj.Add(iModifyObject);
                }
            }
        }

        private List<string> GetEventSourceObjects()
        {
            List<string> eventSourceObjects = new List<string>();

            if (_ilbEventSourceObject.Items != null && _ilbEventSourceObject.Items.Count > 0)
            {
                foreach (object obj in _ilbEventSourceObject.Items)
                {
                    ImageListBoxItem imageListBoxItem = obj as ImageListBoxItem;
                    if (imageListBoxItem != null)
                    {
                        AOrmObject aOrmObject = imageListBoxItem.MyObject as AOrmObject;
                        if (aOrmObject != null)
                            eventSourceObjects.Add(aOrmObject.GetIdString());
                    }
                }
            }

            return eventSourceObjects;
        }

        private void AddToEventSourceObject(AOrmObject aOrmObject)
        {
            if (aOrmObject == null)
                return;

            List<string> addedEventSoruceObjects = GetEventSourceObjects();

            if (!addedEventSoruceObjects.Contains(aOrmObject.GetIdString()))
            {
                _ilbEventSourceObject.Items.Add(new ImageListBoxItem(aOrmObject, aOrmObject.GetObjectType().ToString()));
                CgpClientMainForm.Singleton.AddToRecentList(aOrmObject);
                FilterValueChanged(null, null);
            }
            else
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ilbEventSourceObject,
                    GetString("ErrorObjectAlreadyAdded"), ControlNotificationSettings.Default);
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
                    _ilbEventSourceObject.Items.Remove(_ilbEventSourceObject.SelectedItem);
                    FilterValueChanged(null, null);
                }
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
                if (output == null) return;
                var ormObject = e.Data.GetData(output[0]) as AOrmObject;

                bool added = false;

                if (ormObject != null)
                {
                    var objectType = ormObject.GetObjectType();

                    switch (objectType)
                    {
                        case ObjectType.Person:
                        case ObjectType.Card:
                        case ObjectType.CardSystem:
                        case ObjectType.CardReader:
                        case ObjectType.CCU:
                        case ObjectType.DCU:
                        case ObjectType.Input:
                        case ObjectType.Output:
                        case ObjectType.DoorEnvironment:
                        case ObjectType.AlarmArea:
                            AddToEventSourceObject(ormObject);
                            added = true;
                            break;
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

        private void _tcSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterValueChanged(null, null);
        }

        private void _pDGVBack_Resize(object sender, EventArgs e)
        {
            _pbLoading.Location = new Point(_cdgvData.DataGrid.Location.X + _cdgvData.DataGrid.Width / 2 - _pbLoading.Width / 2,
                    _cdgvData.DataGrid.Location.Y + _cdgvData.DataGrid.Height / 2);
        }

        private void _bClearList_Click(object sender, EventArgs e)
        {
            _ilbEventSourceObject.Items.Clear();
            FilterValueChanged(null, null);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _eventLogs.Clear();
            _tcSearch.SelectedTab = _tpParametricSearch;
            _filterSettings.Clear();
            ClearFilterEdits();
            _tbdpDateFromFilter.Value = DateTime.Now.Date;
            _rbParametricSearchOr.Checked = true;
            _rbFullTextSearchAnd.Checked = true;
            _lRecordCount.Text = string.Empty;
            SetFilterSettings();
            DisableExportButton();
            _selectingOfSubSitesSupport.ClearSubSites();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Abort actual query for read eventlogs on server
        /// </summary>
        private void AbortAcutalQuery()
        {
            CgpClient.Singleton.MainServerProvider.Eventlogs.AbortCurrentQuery();
            _wasAborted = true;
            StopProgress();
        }

        private void _bStop_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            if (Dialog.Question(GetString("QuestionStopQuery")))
                SafeThread.StartThread(AbortAcutalQuery);
        }

        private void _bExportCsv_Click(object sender, EventArgs e)
        {
            int loadedRowCount = _lastLoadedIndex - _firstLoadedIndex;

            if (loadedRowCount > 0)
            {
                if (_cdgvData != null)
                {
                    var exportForm = new EventlogsExportForm(
                        _filterSettings,
                        (int)(_cdgvData.DataGrid.RowCount / loadedRowCount * _lastQueryDelay));
                    exportForm.Show();
                }
            }
        }

        private void _bExportExcel_Click(object sender, EventArgs e)
        {
            var exportForm = new EventlogsExportForm(_filterSettings, _selectingOfSubSitesSupport);
            exportForm.Show();
        }

        /// <summary>
        /// Enable or disable export buttons
        /// </summary>
        private void EnableDisableExportButtons()
        {
            Thread2UI.Invoke(this, () =>
            {
                if (HasAccessExport() && _isEnableExportButton && !_isOpenEventlogsExportForm)
                    _bExportCsv.Enabled = true;
                else
                    _bExportCsv.Enabled = false;

                if (HasAccessExport() && !_isOpenEventlogsExportForm)
                    _bExportExcel.Enabled = true;
                else
                    _bExportExcel.Enabled = false;
            }, true);

        }

        /// <summary>
        /// Set eventlogs export form is opened
        /// </summary>
        public void OpeningEventlogsExportForm()
        {
            _isOpenEventlogsExportForm = true;
            EnableDisableExportButtons();
        }

        /// <summary>
        /// Unset eventlogs export form is opened
        /// </summary>
        public void ClosingEventlogsExportForm()
        {
            _isOpenEventlogsExportForm = false;
            EnableDisableExportButtons();
        }

        /// <summary>
        /// Set export button is enable
        /// </summary>
        private void EnableExportButton()
        {
            _isEnableExportButton = true;
            EnableDisableExportButtons();
        }

        /// <summary>
        /// Set export button is disable
        /// </summary>
        private void DisableExportButton()
        {
            _isEnableExportButton = false;
            EnableDisableExportButtons();
        }

        private void _ilbEventSourceObject_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _ilbEventSourceObject.SelectedIndex = _ilbEventSourceObject.IndexFromPoint(e.X, e.Y);
                _bRemove_Click(sender, e);
            }
        }

        private void _ilbEventSourceObject_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _bAdd_Click(sender, e);
        }

        private void SelectItems(string[] itemsName)
        {
            if (itemsName == null
                || itemsName.Length == 0)
                return;

            _lockItemCheckEvent = true;

            for (int i = 0; i < _chblType.Items.Count; i++)
            {
                if (itemsName.Contains(_chblType.GetItemText(_chblType.Items[i])))
                {
                    _chblType.SetItemChecked(i, true);
                    _eventTypeValues[_chblType.Items[i].ToString()].IsChecked = true;
                }
            }

            _lockItemCheckEvent = false;
        }

        private bool _lockItemCheckEvent;

        private void _chblType_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_chblType.SelectedItem == null
                || _lockItemCheckEvent)
                return;

            base.FilterValueChanged(sender, null);

            _eventTypeValues[_chblType.SelectedItem.ToString()].IsChecked = e.NewValue == CheckState.Checked;

            if (e.NewValue != CheckState.Checked)
                return;

            if (_chblType.SelectedItem.Equals("CCU online")
                || _chblType.SelectedItem.Equals("CCU offline"))
            {
                SelectItems(new[] { "CCU online", "CCU offline" });
            }
            else if (_chblType.SelectedItem.Equals("DCU online")
                || _chblType.SelectedItem.Equals("DCU offline"))
            {
                SelectItems(new[] { "DCU online", "DCU offline" });
            }
            else if (_chblType.SelectedItem.Equals("Client login")
                || _chblType.SelectedItem.Equals("Client logout"))
            {
                SelectItems(new[] { "Client login", "Client logout" });
            }
            else if (_chblType.SelectedItem.Equals("Client connected")
                || _chblType.SelectedItem.Equals("Client disconnected"))
            {
                SelectItems(new[] { "Client connected", "Client disconnected" });
            }
            else if (_chblType.SelectedItem.Equals("DCU node assigned")
                || _chblType.SelectedItem.Equals("DCU node released")
                || _chblType.SelectedItem.Equals("DCU node renewed"))
            {
                SelectItems(new[] { "DCU node assigned", "DCU node released", "DCU node renewed" });
            }
            else if (_chblType.SelectedItem.Equals("Database connected")
                || _chblType.SelectedItem.Equals("Database disconnected"))
            {
                SelectItems(new[] { "Database connected", "Database disconnected" });
            }
        }

        private int _lastChblTypeIndex = -1;
        private void _chblType_MouseClick(object sender, MouseEventArgs e)
        {
            var currentChblTypeIndex = _chblType.SelectedIndex;

            if (_lastChblTypeIndex == currentChblTypeIndex)
                return;

            if (e.Button == MouseButtons.Left && e.X <= _chbCheckUncheckOnlyVisibleItems.Height)
            {
                _chblType.SetItemChecked(currentChblTypeIndex, !_chblType.GetItemChecked(_chblType.SelectedIndex));
            }

            _lastChblTypeIndex = currentChblTypeIndex;
        }

        private void _bToDay_Click(object sender, EventArgs e)
        {
            if (!FilterCheckDates(false))
                return;

            var now = DateTime.Now;
            _tbdpDateFromFilter.Value = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            _tbdpDateToFilter.Value = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999);
        }

        private void _tbSearchEventType_TextChanged(object sender, EventArgs e)
        {
            _chblType.Items.Clear();

            if (string.IsNullOrEmpty(_tbSearchEventType.Text))
            {
                ResetCheckedListboxEventTypes();
                return;
            }

            var filterItems = _eventTypeValues.Where(
                (dicItem) => dicItem.Key.ToLower().Contains(_tbSearchEventType.Text.ToLower()));

            foreach (var item in filterItems)
            {
                _chblType.Items.Add(item.Key, item.Value.IsChecked);
            }
        }

        private void ResetCheckedListboxEventTypes()
        {
            foreach (var item in _eventTypeValues)
            {
                _chblType.Items.Add(item.Key, item.Value.IsChecked);
            }
        }

        private void _bSelectAll_Click(object sender, EventArgs e)
        {
            SetCheckedState(true);
        }

        private void _bUnselectAll_Click(object sender, EventArgs e)
        {
            SetCheckedState(false);
        }

        private void SetCheckedState(bool isChecked)
        {
            //int startPos = _chbCheckUncheckOnlyVisibleItems.Checked 
            //    ? _chblType.TopIndex 
            //    : 0;

            //int maxDisplayedItems = _chbCheckUncheckOnlyVisibleItems.Checked
            //    ? _chblType.ClientSize.Height/_chblType.ItemHeight
            //    : _chblType.Items.Count;

            //int count = startPos + maxDisplayedItems;

            //if (count > _chblType.Items.Count)
            //    count = _chblType.Items.Count;

            //for (int i = startPos; i < count; i++)
            //{
            //    _chblType.SetItemChecked(i, isChecked);
            //    _eventTypeValues[_chblType.Items[i].ToString()].IsChecked = isChecked;
            //}

            for (int i = 0; i < _chblType.Items.Count; i++)
            {
                _chblType.SetItemChecked(i, isChecked);
                _eventTypeValues[_chblType.Items[i].ToString()].IsChecked = isChecked;
            }

            if (_chblType.Items.Count < _eventTypeValues.Count
                && !_chbCheckUncheckOnlyVisibleItems.Checked)
            {
                foreach (var value in _eventTypeValues)
                    value.Value.IsChecked = isChecked;
            }

            FilterValueChanged(null, null);
        }
    }
}
