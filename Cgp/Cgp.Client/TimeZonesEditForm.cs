using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using System.Threading;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class TimeZonesEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<Contal.Cgp.Server.Beans.TimeZone>
#endif
    {
        private BindingSource _bindingSource = null;
        private DailyPlan _actDailyPlan = null;
        private Calendar _actCalendar = null;
        private DayType _actDayType = null;
        private IList<TimeZoneYears> _timeZoneYearsList = null;
        private IList<TimeZoneMonths> _timeZoneMonthsList = null;
        private IList<TimeZoneWeeks> _timeZoneWeeksList = null;
        private IList<TimeZoneDays> _timeZoneDaysList = null;
        private Action<Guid, byte> _eventStatusChanged = null;

        private TimeZoneDateSetting _editTimeZonesDateSetting = null;
        bool _firstTimeDgValues = true;

        public TimeZonesEditForm(Contal.Cgp.Server.Beans.TimeZone timeZone, ShowOptionsEditForm showOption)
            : base(timeZone, showOption)
        {
            InitializeComponent();
            SetReferenceEditColors();
            WheelTabContorol = _tcDateSettingsStatus;
            _cdgvData.DataGrid.VisibleChanged += new EventHandler(DgValuesVisibleChanged);
            //LoadDayStatus();

            _eDescription.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbYear.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbMonth.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbWeek.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _cbDay.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            _eDescriptionDateSettings.MouseWheel += new MouseEventHandler(ControlMouseWheel);
            InitCGPDataGridView();
            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageDateSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesDateSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesDateSettingsAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageDateSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDateSettings),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eName.ReadOnly = true;
                    _tbmCalendar.Enabled = false;
                }

                if (!view && !admin)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tbDateSettings);
                    return;
                }

                _tbDateSettings.Enabled = admin;
            }
        }

        private void HideDisableTabFoldersSructure(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabFoldersSructure), view);
            }
            else
            {
                if (!view)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tpUserFolders);
                    return;
                }
            }
        }

        private void HideDisableTabReferencedBy(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabReferencedBy), view);
            }
            else
            {
                if (!view)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tpReferencedBy);
                    return;
                }
            }
        }

        private void HideDisableTabPageDescription(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDescription),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.DataGrid.CellContentClick += new DataGridViewCellEventHandler(_dgValues_CellContentClick);
            _cdgvData.DataGrid.CellDoubleClick += new DataGridViewCellEventHandler(_dgValues_CellDoubleClick);
            _cdgvData.DataGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(_dgValues_CellFormatting);
            _cdgvData.DataGrid.CellMouseClick += new DataGridViewCellMouseEventHandler(_dgValues_CellMouseClick);
            _cdgvData.DataGrid.CellValueChanged += new DataGridViewCellEventHandler(_dgValues_CellValueChanged);
            _cdgvData.DataGrid.DragDrop += new DragEventHandler(_dgValues_DragDrop);
            _cdgvData.DataGrid.DragOver += new DragEventHandler(_dgValues_DragOver);
            _cdgvData.DataGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(_dgValues_EditingControlShowing);
        }

        protected override void RegisterEvents()
        {
            _eventStatusChanged = new Action<Guid, byte>(ChangeStatus);
            StatusChangedTimeZoneHandler.Singleton.RegisterStatusChanged(_eventStatusChanged);
        }

        protected override void UnregisterEvents()
        {
            StatusChangedTimeZoneHandler.Singleton.UnregisterStatusChanged(_eventStatusChanged);
        }

        void DgValuesVisibleChanged(object sender, EventArgs e)
        {
            if (_firstTimeDgValues)
            {
                if (_editingObject == null) return;
                ShowTimeZoneDateSettings();
                _firstTimeDgValues = false;
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = LocalizationHelper.GetString("TimeZonesEditFormInsertText");
            }

            foreach (ToolStripItem item in _cmsDailyPlan.Items)
            {
                item.Text = GetString(Name + item.Name);
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            TimeZonesForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            TimeZonesForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            Contal.Cgp.Server.Beans.TimeZone obj = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(_editingObject.IdTimeZone);
                }
                else
                {
                    throw error;
                }
                DisabledForm();
            }
            else
            {
                allowEdit = true;
            }

            _editingObject = obj;
        }

        public override void InternalReloadEditingObjectWithEditedData()
        {
            Exception error;
            CgpClient.Singleton.MainServerProvider.TimeZones.RenewObjectForEdit(_editingObject.IdTimeZone, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            SetCalendar(CgpClient.Singleton.MainServerProvider.Calendars.GetDefaultCalendar());
            LoadDateSettingTypes();
            LoadTimeZoneDateSettings();
        }

        protected override void SetValuesEdit()
        {
            _eName.Text = _editingObject.Name;
            _eDescription.Text = _editingObject.Description;
            _actCalendar = _editingObject.Calendar;

            if (_actCalendar.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT)
            {
                _tbmCalendar.Text = GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                _tbmCalendar.TextImage = ObjectImageList.Singleton.GetImageForObjectType(ObjectType.Calendar);
            }
            else
            {
                _tbmCalendar.Text = _actCalendar.CalendarName;
                _tbmCalendar.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actCalendar);
            }

            ChangeStatus(_editingObject.IdTimeZone, CgpClient.Singleton.MainServerProvider.GetTimeZoneActualStatus(_editingObject.IdTimeZone));

            LoadDateSettingTypes();
            LoadTimeZoneDateSettings();
            SetReferencedBy();
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = true;
            _bCalendar.Enabled = true;
            _eDate.Enabled = true;
        }

        private void ChangeStatus(Guid idTimeZone, byte status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(ChangeStatus), idTimeZone, status);
            }
            else
            {
                if (_editingObject.IdTimeZone == idTimeZone)
                {
                    if (status == 0)
                    {
                        _eStatus.Text = LocalizationHelper.GetString("TimeZonesEditForm_StatusOff");
                        _eStatus.BackColor = Color.Red;
                    }
                    else
                    {
                        _eStatus.Text = LocalizationHelper.GetString("TimeZonesEditForm_StatusOn");
                        _eStatus.BackColor = Color.LightGreen;
                    }
                }
            }
        }

        private void LoadTimeZoneDateSettings()
        {
            ConnectionLost();

            if (_editingObject.DateSettings == null || _editingObject.DateSettings.Count == 0)
            {
                _bindingSource = null;
                _cdgvData.DataGrid.DataSource = null;
                return;
            }

            foreach (TimeZoneDateSetting tzDateSetting in _editingObject.DateSettings)
            {
                if (tzDateSetting.DailyPlan != null)
                    tzDateSetting.DailyPlan = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(tzDateSetting.DailyPlan.IdDailyPlan);
            }

            int lastPosition = 0;
            if (_bindingSource != null && _bindingSource.Count != 0)
                lastPosition = _bindingSource.Position;

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _editingObject.DateSettings;
            if (lastPosition != 0) _bindingSource.Position = lastPosition;
            _bindingSource.AllowNew = false;
            _cdgvData.DataGrid.SuspendLayout();
            _cdgvData.DataGrid.DataSource = _bindingSource;

            if (!_cdgvData.DataGrid.Columns.Contains("str" + TimeZoneDateSetting.COLUMNYEAR))
            {
                DataGridViewComboBoxColumn yearColumn = new DataGridViewComboBoxColumn();
                yearColumn.Name = "str" + TimeZoneDateSetting.COLUMNYEAR;
                yearColumn.HeaderText = "str" + TimeZoneDateSetting.COLUMNYEAR;

                IList<ComboBoxItem> items = new List<ComboBoxItem>();
                foreach (TimeZoneYears timeZoneYear in _timeZoneYearsList)
                {
                    if (timeZoneYear.Value == YearSelection.Year)
                    {
                        short actYear = (short)DateTime.Now.Year;

                        for (short year = actYear; year <= actYear + 30; year++)
                        {
                            items.Add(new ComboBoxItem(year));
                        }
                    }
                    else
                    {
                        items.Add(new ComboBoxItem(timeZoneYear.Value, timeZoneYear.Name));
                    }
                }
                yearColumn.DataSource = items;
                yearColumn.ValueMember = "Value";
                yearColumn.DisplayMember = "Name";
                yearColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                _cdgvData.DataGrid.Columns.Add(yearColumn);
                _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNYEAR].DisplayIndex = _cdgvData.DataGrid.Columns[TimeZoneDateSetting.COLUMNYEAR].DisplayIndex;
            }

            if (!_cdgvData.DataGrid.Columns.Contains("str" + TimeZoneDateSetting.COLUMNMONTH))
            {
                DataGridViewComboBoxColumn monthColumn = new DataGridViewComboBoxColumn();
                monthColumn.Name = "str" + TimeZoneDateSetting.COLUMNMONTH;
                monthColumn.HeaderText = "str" + TimeZoneDateSetting.COLUMNMONTH;
                monthColumn.DataSource = _timeZoneMonthsList;
                monthColumn.ValueMember = "Value";
                monthColumn.DisplayMember = "Name";
                monthColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                _cdgvData.DataGrid.Columns.Add(monthColumn);
                _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNMONTH].DisplayIndex = _cdgvData.DataGrid.Columns[TimeZoneDateSetting.COLUMNMONTH].DisplayIndex;
            }

            if (!_cdgvData.DataGrid.Columns.Contains("str" + TimeZoneDateSetting.COLUMNWEEK))
            {
                DataGridViewComboBoxColumn weekColumn = new DataGridViewComboBoxColumn();
                weekColumn.Name = "str" + TimeZoneDateSetting.COLUMNWEEK;
                weekColumn.HeaderText = "str" + TimeZoneDateSetting.COLUMNWEEK;
                weekColumn.DataSource = _timeZoneWeeksList;
                weekColumn.ValueMember = "Value";
                weekColumn.DisplayMember = "Name";
                weekColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                _cdgvData.DataGrid.Columns.Add(weekColumn);
                _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNWEEK].DisplayIndex = _cdgvData.DataGrid.Columns[TimeZoneDateSetting.COLUMNWEEK].DisplayIndex;
            }

            if (!_cdgvData.DataGrid.Columns.Contains("str" + TimeZoneDateSetting.COLUMNDAY))
            {
                DataGridViewComboBoxColumn dayColumn = new DataGridViewComboBoxColumn();
                dayColumn.Name = "str" + TimeZoneDateSetting.COLUMNDAY;
                dayColumn.HeaderText = "str" + TimeZoneDateSetting.COLUMNDAY;
                dayColumn.DataSource = _timeZoneDaysList;
                dayColumn.ValueMember = "Value";
                dayColumn.DisplayMember = "Name";
                dayColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                _cdgvData.DataGrid.Columns.Add(dayColumn);
                _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAY].DisplayIndex = _cdgvData.DataGrid.Columns[TimeZoneDateSetting.COLUMNDAY].DisplayIndex;
            }

            if (!_cdgvData.DataGrid.Columns.Contains("str" + TimeZoneDateSetting.COLUMNDAILYPLAN))
            {
                _cdgvData.DataGrid.Columns.Add("str" + TimeZoneDateSetting.COLUMNDAILYPLAN, "str" + TimeZoneDateSetting.COLUMNDAILYPLAN);
                _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].ReadOnly = true;
                _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].DisplayIndex = _cdgvData.DataGrid.Columns[TimeZoneDateSetting.COLUMNDAILYPLAN].DisplayIndex;
            }

            if (_cdgvData.DataGrid.Columns.Contains(TimeZoneDateSetting.COLUMNDESCRIPTION))
                _cdgvData.DataGrid.Columns[TimeZoneDateSetting.COLUMNDESCRIPTION].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNIDTIMEZONEDATESETTING);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNYEAR);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNSETYEAR);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNMONTH);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNWEEK);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNDAY);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNTIMEZONE);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNDAYTYPE);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNGUIDTIMEZONE);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNGUIDDAYTYPE);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNGUIDDAILYPLAN);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.COLUMNDAILYPLAN);
            HideColumnDgw(_cdgvData.DataGrid, TimeZoneDateSetting.ColumnVersion);

            _cdgvData.DataGrid.AutoGenerateColumns = false;
            _cdgvData.DataGrid.AllowUserToAddRows = false;

            CgpClient.Singleton.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvData.DataGrid);

            _cdgvData.DataGrid.ResumeLayout();
            if (_editTimeZonesDateSetting == null)
                ClearDateSettingsValues();
            ShowTimeZoneDateSettings();
            LoadDayTypes();
        }

        private void ShowTimeZoneDateSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(ShowTimeZoneDateSettings));
            }
            else
            {
                if (_cdgvData.DataGrid.DataSource == null) return;
                _cdgvData.DataGrid.SuspendLayout();
                foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                {
                    TimeZoneDateSetting dateSetting = (TimeZoneDateSetting)_bindingSource.List[row.Index];
                    TimeZoneYears year = TimeZoneYears.GetTimeZoneYear(LocalizationHelper, dateSetting.Year);
                    if (year != null)
                    {
                        if (year.Value == YearSelection.AllYears)
                            row.Cells["str" + TimeZoneDateSetting.COLUMNYEAR].Value = year.Value;
                        else if (year.Value == YearSelection.Year && dateSetting.SetYear != null)
                            row.Cells["str" + TimeZoneDateSetting.COLUMNYEAR].Value = dateSetting.SetYear.Value;
                        else
                        {
                            row.Cells["str" + TimeZoneDateSetting.COLUMNYEAR].Value = null;
                        }
                    }
                    else
                    {
                        row.Cells["str" + TimeZoneDateSetting.COLUMNYEAR].Value = null;
                    }

                    TimeZoneMonths month = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, dateSetting.Month);
                    if (month != null)
                    {
                        row.Cells["str" + TimeZoneDateSetting.COLUMNMONTH].Value = month.Value;
                    }
                    else
                    {
                        row.Cells["str" + TimeZoneDateSetting.COLUMNMONTH].Value = null;
                    }

                    TimeZoneWeeks week = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, dateSetting.Week);
                    if (week != null)
                    {
                        row.Cells["str" + TimeZoneDateSetting.COLUMNWEEK].Value = week.Value;
                    }
                    else
                    {
                        row.Cells["str" + TimeZoneDateSetting.COLUMNWEEK].Value = null;
                    }

                    TimeZoneDays day = TimeZoneDays.GetTimeZoneDay(LocalizationHelper, dateSetting.Day);
                    if (day != null)
                    {
                        row.Cells["str" + TimeZoneDateSetting.COLUMNDAY].Value = day.Value;
                    }
                    else
                    {
                        row.Cells["str" + TimeZoneDateSetting.COLUMNDAY].Value = null;
                    }
                    if (dateSetting.DayType != null)
                    {
                        row.Cells["str" + TimeZoneDateSetting.COLUMNDAY].Value = dateSetting.DayType.IdDayType;
                    }

                    string actValue = row.Cells[TimeZoneDateSetting.COLUMNDAILYPLAN].Value.ToString();
                    switch (actValue)
                    {
                        case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                            row.Cells["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Value = GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                            break;
                        case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                            row.Cells["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Value = GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                            break;
                        case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                            row.Cells["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Value = GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                            break;
                        case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                            row.Cells["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Value = GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                            break;
                        default:
                            row.Cells["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Value = actValue;
                            break;
                    }
                    _cdgvData.DataGrid.ResumeLayout();
                }
            }
        }

        protected void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView == null) return;

            if (gridView.Columns.Contains(columnName))
                gridView.Columns[columnName].Visible = false;
        }

        private void LoadDateSettingTypes()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            _cbYear.Enabled = true;
            _cbMonth.Enabled = true;
            _cbWeek.Enabled = true;

            _timeZoneYearsList = TimeZoneYears.GetTimeZoneYearsList(LocalizationHelper);
            _cbYear.BeginUpdate();
            _cbYear.Items.Clear();
            foreach (TimeZoneYears timeZoneYear in _timeZoneYearsList)
            {
                if (timeZoneYear.Value == YearSelection.Year)
                {
                    short actYear = (short)DateTime.Now.Year;

                    for (short year = actYear; year <= actYear + 30; year++)
                    {
                        _cbYear.Items.Add(year);
                    }
                }
                else
                {
                    _cbYear.Items.Add(timeZoneYear);
                }
            }
            _cbYear.SelectedItem = TimeZoneYears.GetTimeZoneYear(LocalizationHelper, _timeZoneYearsList, (byte)YearSelection.AllYears);
            _cbYear.EndUpdate();

            _timeZoneMonthsList = TimeZoneMonths.GetTimeZoneMonthsList(LocalizationHelper);
            _cbMonth.BeginUpdate();
            _cbMonth.Items.Clear();
            foreach (TimeZoneMonths timeZoneMonth in _timeZoneMonthsList)
            {
                _cbMonth.Items.Add(timeZoneMonth);
            }
            _cbMonth.SelectedItem = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, _timeZoneMonthsList, (byte)MonthType.AllMonths);
            _cbMonth.EndUpdate();

            _timeZoneWeeksList = TimeZoneWeeks.GetTimeZoneWeeksList(LocalizationHelper);
            _cbWeek.BeginUpdate();
            _cbWeek.Items.Clear();
            foreach (TimeZoneWeeks timeZoneWeek in _timeZoneWeeksList)
            {
                _cbWeek.Items.Add(timeZoneWeek);
            }
            _cbWeek.SelectedItem = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks);
            _cbWeek.EndUpdate();
            _timeZoneDaysList = TimeZoneDays.GetTimeZoneDaysList(LocalizationHelper);
            LoadDayTypes();
        }

        private void LoadDayTypes()
        {
            _cbDay.BeginUpdate();
            _cbDay.Items.Clear();
            foreach (TimeZoneDays timeZoneDay in _timeZoneDaysList)
            {
                _cbDay.Items.Add(timeZoneDay);
            }
            ICollection<DayType> calendarDayTypes = CgpClient.Singleton.MainServerProvider.CalendarDateSettings.GetDayTypeUniqueByCalendar(_actCalendar);
            foreach (DayType dayType in calendarDayTypes)
            {
                DayTypeComboBox dtcb = new DayTypeComboBox(dayType);
                _cbDay.Items.Add(dtcb);
            }
            _cbDay.SelectedItem = TimeZoneDays.GetTimeZoneDay(CgpClient.Singleton.LocalizationHelper, _timeZoneDaysList, (byte)DaySelection.allDays);
            _cbDay.EndUpdate();

            if (_cdgvData.DataGrid.Columns.Contains("str" + TimeZoneDateSetting.COLUMNDAY))
            {
                List<ComboBoxItem> columnItems = new List<ComboBoxItem>();
                foreach (TimeZoneDays timeZoneDay in _timeZoneDaysList)
                {
                    columnItems.Add(new ComboBoxItem(timeZoneDay.Value, timeZoneDay.Name));
                }
                calendarDayTypes = CgpClient.Singleton.MainServerProvider.CalendarDateSettings.GetDayTypeUniqueByCalendar(_actCalendar);
                foreach (DayType dayType in calendarDayTypes)
                {
                    DayType dt = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(dayType.IdDayType);
                    columnItems.Add(new ComboBoxItem(dt.IdDayType, DayTypeName(dt)));
                }
                (_cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAY] as DataGridViewComboBoxColumn).DataSource = columnItems;
            }
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;
                _editingObject.Description = _eDescription.Text;
                if (_actCalendar == null)
                {
                    _editingObject.Calendar = null;
                }
                else
                {
                    _editingObject.Calendar = _actCalendar;
                }
                return true;
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool CheckValues()
        {
            if (_eName.Text == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                GetString("ErrorEntryTimeZoneName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }

            if (_actCalendar == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmCalendar.ImageTextBox,
                GetString("ErrorEntryCalendar"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tbmCalendar.ImageTextBox.Focus();
                return false;
            }

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.TimeZones.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedTimeZoneSettingPair"));
                }
                else if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorTimeZoneNameExists"));
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            return SaveToDatabaseEditCore(false);
        }

        protected override bool SaveToDatabaseEditOnlyInDatabase()
        {
            return SaveToDatabaseEditCore(true);
        }

        private bool SaveToDatabaseEditCore(bool onlyInDatabase)
        {
            Exception error;
            bool retValue;

            if (onlyInDatabase)
                retValue = CgpClient.Singleton.MainServerProvider.TimeZones.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = CgpClient.Singleton.MainServerProvider.TimeZones.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedTimeZoneSettingPair"));
                }
                else if (error is Contal.IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorTimeZoneNameExists"));
                }
                else
                    throw error;
            }

            return retValue;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        protected override void AfterInsert()
        {
            TimeZonesForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            TimeZonesForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);
        }

        private void RefreshDailyPlan()
        {
            if (_actDailyPlan == null)
            {
                _tbmDailyPlan.Text = string.Empty;
            }
            else
            {
                DailyPlanList dpl = new DailyPlanList(_actDailyPlan);
                _tbmDailyPlan.Text = dpl.ToString();
                _tbmDailyPlan.TextImage = ObjectImageList.Singleton.GetImageForObjectType(ObjectType.DailyPlan);
            }
        }

        private void ModifyDailyPlan()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            Exception error = null;
            try
            {
                List<IModifyObject> listDailyPlans = new List<IModifyObject>();

                IList<IModifyObject> listDailyPlansFromDatabase = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateDailyPlans(listDailyPlansFromDatabase);
                listDailyPlans.AddRange(listDailyPlansFromDatabase);

                ListboxFormAdd formAdd = new ListboxFormAdd(listDailyPlans, GetString("DailyPlansFormDailyPlansForm"));
                object outDailyPlan;
                formAdd.ShowDialog(out outDailyPlan);
                if (outDailyPlan != null)
                {
                    var dpModifyObject = (IModifyObject)outDailyPlan;
                    _actDailyPlan = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(dpModifyObject.GetId);
                    RefreshDailyPlan();
                    CgpClientMainForm.Singleton.AddToRecentList(_actDailyPlan);
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(error);
            }
        }

        private void _bModify_Click(object sender, EventArgs e)
        {
            ModifyDailyPlan();
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            DailyPlan dailyPlan = new DailyPlan();
            if (DailyPlansForm.Singleton.OpenInsertDialg(ref dailyPlan))
            {
                _actDailyPlan = dailyPlan;
                RefreshDailyPlan();
            }
        }

        private void _bRemove_Click(object sender, EventArgs e)
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionDeleteConfirm")))
            {
                Delete();
            }
        }

        private void SetDateSettingValues(bool setDataFromActualObject)
        {
            if (_editTimeZonesDateSetting == null)
                return;

            TimeZoneDateSetting dateSetting = null;

            if (_editingObject.DateSettings != null)
            {
                foreach (TimeZoneDateSetting actDateSetting in _editingObject.DateSettings)
                {
                    if (_editTimeZonesDateSetting.Compare(actDateSetting))
                        dateSetting = actDateSetting;
                }
            }

            _editTimeZonesDateSetting = dateSetting;

            if (_editTimeZonesDateSetting == null)
            {
                _bCancelEdit_Click(null, null);
            }
            else
            {
                if (setDataFromActualObject)
                    SetDateSettingValues(_editTimeZonesDateSetting);
            }
        }

        private void Delete()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            TimeZoneDateSetting dateSetting = (TimeZoneDateSetting)_bindingSource.List[_bindingSource.Position];

            _editingObject.DateSettings.Remove(dateSetting);

            if (!Insert)
            {
                Exception error;
                if (!CgpClient.Singleton.MainServerProvider.TimeZones.Update(_editingObject, out error))
                {
                    if (error is Contal.IwQuick.IncoherentDataException)
                    {
                        if (Contal.IwQuick.UI.Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                        {
                            SetValues();
                            SetDateSettingValues(true);
                        }
                        else
                        {
                            _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                            LoadTimeZoneDateSettings();
                            SetDateSettingValues(false);
                        }
                    }
                    else
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteFailed"));

                    return;
                }

                _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
            }

            LoadTimeZoneDateSettings();
            LoadDayStatus();
        }

        private bool ControlDateSettingsValues()
        {
            if (_actCalendar == null)
            {
                if (_cbYear.SelectedItem == null)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbYear,
                    GetString("ErrorEntryTimeZoneYear"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    _cbYear.Focus();
                    return false;
                }

                if (_cbMonth.SelectedItem == null)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbMonth,
                    GetString("ErrorEntryTimeZoneMonth"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    _cbMonth.Focus();
                    return false;
                }

                if (_cbWeek.SelectedItem == null)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbWeek,
                    GetString("ErrorEntryTimeZoneWeek"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    _cbWeek.Focus();
                    return false;
                }
            }

            if (_cbDay.SelectedItem == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbDay,
                GetString("ErrorEntryTimeZoneDay"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _cbDay.Focus();
                return false;
            }

            if (_actDailyPlan == null)
            {
                _actDailyPlan = CgpClient.Singleton.MainServerProvider.DailyPlans.GetDailyPlanAlwaysON();
                if (_actDailyPlan == null)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmDailyPlan,
                    GetString("ErrorEntryTimeZoneDailyPlan"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    _tbmDailyPlan.Focus();
                    return false;
                }
            }

            return true;
        }

        private void GetDateSettingValues(TimeZoneDateSetting dateSetting)
        {
            dateSetting.DailyPlan = _actDailyPlan;
            dateSetting.TimeZone = _editingObject;
            dateSetting.Description = _eDescriptionDateSettings.Text;
            dateSetting.ExplicitDailyPlan = _cbExplicitDailyPlan.Checked;

            if (_actDayType == null)
            {
                dateSetting.DayType = null;
                if (_cbYear.SelectedItem is TimeZoneYears)
                {
                    YearSelection yearType = (_cbYear.SelectedItem as TimeZoneYears).Value;
                    dateSetting.Year = (byte)yearType;

                    if (yearType == YearSelection.AllYears)
                    {
                        dateSetting.SetYear = null;
                        dateSetting.Year = (byte)yearType;
                    }
                }
                else if (_cbYear.SelectedItem is short)
                {
                    dateSetting.SetYear = (short)_cbYear.SelectedItem;
                    dateSetting.Year = (byte)YearSelection.Year;
                }

                dateSetting.Month = (byte)(_cbMonth.SelectedItem as TimeZoneMonths).Value;
                dateSetting.Week = (byte)(_cbWeek.SelectedItem as TimeZoneWeeks).Value;
                dateSetting.Day = (byte)(_cbDay.SelectedItem as TimeZoneDays).Value;
            }
            else
            {
                dateSetting.Year = null;
                dateSetting.SetYear = null;
                dateSetting.Month = null;
                dateSetting.Week = null;
                dateSetting.Day = null;
                dateSetting.DayType = _actDayType;
            }
        }

        private void _bCreateDateSettings_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (!ControlDateSettingsValues())
                return;

            TimeZoneDateSetting dateSetting = new TimeZoneDateSetting();
            GetDateSettingValues(dateSetting);

            if (_editingObject.DateSettings == null)
                _editingObject.DateSettings = new List<TimeZoneDateSetting>();

            _editingObject.DateSettings.Add(dateSetting);

            if (!Insert)
            {
                Exception error;
                bool retValue = CgpClient.Singleton.MainServerProvider.TimeZones.Update(_editingObject, out error);
                if (!retValue)
                {
                    _editingObject.DateSettings.Remove(dateSetting);
                    if (error is Contal.IwQuick.SqlUniqueException)
                    {
                        if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedTimeZoneSettingPair"));
                        }
                    }
                    else if (error is Contal.IwQuick.IncoherentDataException)
                    {
                        if (!Contal.IwQuick.UI.Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                        {
                            return;
                        }
                    }
                    else
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertFailed"));

                    _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                    LoadTimeZoneDateSettings();
                    return;
                }

                _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
            }
            LoadTimeZoneDateSettings();
            LoadDayStatus();
        }

        private void SetDateSettingValues(TimeZoneDateSetting dateSetting)
        {
            TimeZoneYears year = TimeZoneYears.GetTimeZoneYear(LocalizationHelper, _timeZoneYearsList, dateSetting.Year);
            _cbYear.SelectedItem = year;

            if (year != null)
            {
                if (year.Value == YearSelection.Year)
                {
                    object setItem = null;
                    foreach (object item in _cbYear.Items)
                    {
                        if (item is short)
                        {
                            if ((short)item == dateSetting.SetYear)
                                setItem = item;
                        }
                    }

                    if (setItem == null)
                    {
                        _cbYear.Items.Add(dateSetting.SetYear);
                        setItem = dateSetting.SetYear;
                    }

                    _cbYear.SelectedItem = setItem;
                }
            }

            TimeZoneMonths month = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, _timeZoneMonthsList, dateSetting.Month);
            _cbMonth.SelectedItem = month;

            TimeZoneWeeks week = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, _timeZoneWeeksList, dateSetting.Week);
            _cbWeek.SelectedItem = week;

            TimeZoneDays day = TimeZoneDays.GetTimeZoneDay(LocalizationHelper, _timeZoneDaysList, dateSetting.Day);
            _cbDay.SelectedItem = day;

            if (dateSetting.DayType != null)
            {
                _actDayType = dateSetting.DayType;
                //_cbDay.SelectedItem = new DayTypeComboBox(_actDayType);
                string name = DayTypeName(_actDayType);
                foreach (object obj in _cbDay.Items)
                {
                    if (obj is DayTypeComboBox)
                    {
                        if ((obj as DayTypeComboBox).ToString() == name)
                        {
                            _cbDay.SelectedItem = obj;
                            break;
                        }
                    }
                }
            }

            _eDescriptionDateSettings.Text = dateSetting.Description;
            _cbExplicitDailyPlan.Checked = dateSetting.ExplicitDailyPlan;

            _actDailyPlan = dateSetting.DailyPlan;
            RefreshDailyPlan();
        }

        private void _bCancelEdit_Click(object sender, EventArgs e)
        {
            _editTimeZonesDateSetting = null;
            _bCreateDateSetting.Visible = true;

            ClearDateSettingsValues();
        }

        private void _bUpdateDateSettings_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (!ControlDateSettingsValues())
                return;

            GetDateSettingValues(_editTimeZonesDateSetting);

            if (!Insert)
            {
                Exception error;
                bool retValue = CgpClient.Singleton.MainServerProvider.TimeZones.Update(_editingObject, out error);
                if (!retValue)
                {
                    if (error is Contal.IwQuick.IncoherentDataException)
                    {
                        if (error is Contal.IwQuick.SqlUniqueException)
                        {
                            if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                            {
                                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedTimeZoneSettingPair"));
                            }
                        }
                        else if (error is Contal.IwQuick.IncoherentDataException)
                        {
                            if (!Contal.IwQuick.UI.Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (error != null)
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorEditFailed") + ": " + error.Message);
                        else
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorEditFailed") + ": unknown error");
                    }

                    _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                    LoadTimeZoneDateSettings();
                    return;
                }

                _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                foreach (Cgp.Server.Beans.TimeZoneDateSetting item in _editingObject.DateSettings)
                {
                    item.DailyPlan = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(item.DailyPlan.IdDailyPlan);
                }

            }

            //EditTextChanger(null, null);
            _editTimeZonesDateSetting = null;
            _bCreateDateSetting.Visible = true;

            LoadTimeZoneDateSettings();
            LoadDayStatus();
            _cdgvData.DataGrid.Refresh();
        }

        private void ClearDateSettingsValues()
        {
            _cbYear.SelectedItem = TimeZoneYears.GetTimeZoneYear(LocalizationHelper, _timeZoneYearsList, (byte)YearSelection.AllYears);
            _cbMonth.SelectedItem = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, _timeZoneMonthsList, (byte)MonthType.AllMonths);
            _cbWeek.SelectedItem = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks);
            _cbDay.SelectedItem = TimeZoneDays.GetTimeZoneDay(LocalizationHelper, _timeZoneDaysList, (byte)DaySelection.allDays);
            _actDailyPlan = null;
            RefreshDailyPlan();
            _eDescriptionDateSettings.Text = string.Empty;
            _cbExplicitDailyPlan.Checked = false;
        }

        private void _tbmDailyPlan_DoubleClick(object sender, EventArgs e)
        {
            if (_actDailyPlan != null)
                DailyPlansForm.Singleton.OpenEditForm(_actDailyPlan);
        }

        private void _tbmDailyPlan_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmDailyPlan_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddDailyPlan((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddDailyPlan(object newDailyPlan)
        {
            try
            {
                if (newDailyPlan.GetType() == typeof(DailyPlan))
                {
                    DailyPlan dailyPlan = newDailyPlan as DailyPlan;
                    _actDailyPlan = dailyPlan;
                    RefreshDailyPlan();
                    CgpClientMainForm.Singleton.AddToRecentList(newDailyPlan);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmDailyPlan.ImageTextBox,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _bCalendar_Click(object sender, EventArgs e)
        {
            DateTime? dateTime = null;

            if (_eDate.Text != string.Empty)
            {
                DateTime outDate;
                DateTime.TryParse(_eDate.Text, out outDate);

                dateTime = outDate;
            }

            SetDate setDate = new SetDate(dateTime);
            DateForm calendarForm = new DateForm(LocalizationHelper, setDate);
            calendarForm.ShowDialog();

            if (setDate.IsSetDate)
            {
                LoadDayStatus(setDate.Date);
            }
        }

        private void LoadDayStatus(DateTime dateTime)
        {
            _eDate.Text = dateTime.ToShortDateString();
            IList<DayInterval> intervals = new List<DayInterval>();

            if (_editingObject.DateSettings != null)
            {
                intervals = CgpClient.Singleton.MainServerProvider.TimeZones.GetActualDayInterval(_editingObject.IdTimeZone, dateTime);
            }
            _dmDayStatus.SetIntervals(GetIntervals(intervals));

            //_dsDayStatus.SetIntervals(intervals);
            //_dmDailyStatus.Refresh();
        }

        private List<Interval> GetIntervals(ICollection<DayInterval> dayIntervals)
        {
            List<Interval> intervals = new List<Interval>();

            if (dayIntervals != null)
            {
                foreach (DayInterval interval in dayIntervals)
                {
                    intervals.Add(new Interval(interval.MinutesFrom, interval.MinutesTo, 1));
                }
            }
            return intervals;
        }

        private void LoadDayStatus()
        {
            DateTime dateTime = DateTime.Now;

            if (_eDate.Text != string.Empty)
            {
                DateTime outDate;
                DateTime.TryParse(_eDate.Text, out outDate);

                dateTime = outDate;
            }

            LoadDayStatus(dateTime);
        }
        private void Modify()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            Exception error = null;
            try
            {
                List<IModifyObject> listModObj = new List<IModifyObject>();
                IList<IModifyObject> listCalendarsFromDatabase = CgpClient.Singleton.MainServerProvider.Calendars.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateCalendars(listCalendarsFromDatabase);
                listModObj.AddRange(listCalendarsFromDatabase);

                ListboxFormAdd formAdd = new ListboxFormAdd(listModObj, GetString("CalendarsFormCalendarsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    Calendar newCalendar = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectById(outModObj.GetId);
                    SetCalendar(newCalendar);
                    CgpClientMainForm.Singleton.AddToRecentList(_actCalendar);
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(error);
            }
        }

        private void _bModify1_Click(object sender, EventArgs e)
        {
            Modify();
        }

        private void _bCreate1_Click(object sender, EventArgs e)
        {
            Calendar calendar = new Calendar();
            if (CalendarsForm.Singleton.OpenInsertDialg(ref calendar))
            {
                SetCalendar(calendar);
            }
        }

        private void _tbmCalendar_DoubleClick(object sender, EventArgs e)
        {
            if (_actCalendar != null)
                CalendarsForm.Singleton.OpenEditForm(_actCalendar);
        }

        private void _tbmCalendar_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddCalendar((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmCalendar_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void AddCalendar(object newCalendar)
        {
            try
            {
                if (newCalendar.GetType() == typeof(Calendar))
                {
                    SetCalendar((Calendar)newCalendar);
                    CgpClientMainForm.Singleton.AddToRecentList(newCalendar);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmCalendar.ImageTextBox,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private bool SetCalendarToTimeZone()
        {
            try
            {
                _editingObject.Calendar = _actCalendar;
                Exception error;
                bool retValue = CgpClient.Singleton.MainServerProvider.TimeZones.Update(_editingObject, out error);
                if (!retValue)
                {
                    if (error is Contal.IwQuick.SqlUniqueException)
                    {
                        if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedTimeZoneSettingPair"));
                        }
                        else
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorTimeZoneNameExists"));
                        }
                    }
                    else if (error is Contal.IwQuick.IncoherentDataException)
                    {
                        if (Contal.IwQuick.UI.Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                            SetValues();
                        else
                            _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                    }
                    else
                    {
                        if (error != null)
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorEditFailed") + ": " + error.Message);
                        else
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorEditFailed") + ": unknown error");
                    }
                    return false;
                }
                _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void SetCalendar(Calendar objCalendar)
        {
            try
            {
                Calendar oldCalendar = _actCalendar;
                _actCalendar = objCalendar;
                Exception error;

                IList<string> notDefined = CalendarNotHave(_actCalendar);
                if (notDefined != null && notDefined.Count > 0)
                {
                    TimeZoneCalendarChangeForm form = new TimeZoneCalendarChangeForm(oldCalendar, _actCalendar, _editingObject, Insert);
                    DialogResult dr = form.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                        LoadTimeZoneDateSettings();
                        if (error != null)
                        {
                            Contal.IwQuick.UI.Dialog.Error(error);
                            return;
                        }
                    }
                    else
                    {
                        _actCalendar = oldCalendar;
                    }
                }
                else
                {
                    if (!Insert)
                    {
                        if (!SetCalendarToTimeZone())
                            return;
                    }
                }

                if (_actCalendar == null)
                {
                    _tbmCalendar.Text = string.Empty;
                }
                else
                {
                    if (_actCalendar.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                    {
                        _tbmCalendar.Text = GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                        _tbmCalendar.TextImage = ObjectImageList.Singleton.GetImageForObjectType(ObjectType.Calendar);
                    }
                    else
                    {
                        _tbmCalendar.Text = _actCalendar.CalendarName;
                        _tbmCalendar.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actCalendar);
                    }
                }
                LoadDateSettingTypes();
                UpdateDateSettings();
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorSetCalendarFailed"));
            }
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.TimeZones != null)
                CgpClient.Singleton.MainServerProvider.TimeZones.EditEnd(_editingObject);
        }


        private class DayTypeComboBox
        {
            DayType _dayType;
            public DayType DayType
            {
                get { return _dayType; }
            }

            public DayTypeComboBox(DayType dayType)
            {
                _dayType = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(dayType.IdDayType);
            }

            public override string ToString()
            {
                if (_dayType == null) return string.Empty;

                if (_dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
                {
                    return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
                }
                else if (_dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
                {
                    return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
                }
                else
                {
                    return _dayType.ToString();
                }
            }
        }

        private class CalendarList : IListObjects
        {
            private Calendar _calendar = null;

            public bool Contains(string expression)
            {
                if (_calendar.Contains(expression))
                    return true;
                if (this.ToString().ToLower().Contains(expression.ToLower()))
                    return true;
                return false;
            }

            public AOrmObject GetOrmObj()
            {
                return _calendar;
            }

            public CalendarList(Calendar calendar)
            {
                _calendar = calendar;
            }

            public override string ToString()
            {
                if (_calendar.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                {
                    return CgpClient.Singleton.LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                }
                else
                {
                    return _calendar.CalendarName;
                }
            }
        }

        private void _cbDay_SelectedIndexChanged(object sender, EventArgs e)
        {

            object obj;
            obj = _cbDay.SelectedItem;
            if (obj == null) return;
            if (obj.GetType() == typeof(DayTypeComboBox))
            {
                DayTypeComboBox dtCB = obj as DayTypeComboBox;
                _actDayType = dtCB.DayType;

                _cbYear.SelectedItem = null;
                _cbYear.Enabled = false;
                _cbMonth.SelectedItem = null;
                _cbMonth.Enabled = false;
                _cbWeek.SelectedItem = null;
                _cbWeek.Enabled = false;
            }
            else
            {
                _actDayType = null;
                _cbYear.Enabled = true;
                _cbMonth.Enabled = true;
                _cbWeek.Enabled = true;
                if (_cbYear.SelectedItem == null && _cbMonth.SelectedItem == null && _cbWeek.SelectedItem == null)
                {
                    _cbYear.SelectedItem = TimeZoneYears.GetTimeZoneYear(LocalizationHelper, _timeZoneYearsList, (byte)YearSelection.AllYears);
                    _cbMonth.SelectedItem = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, _timeZoneMonthsList, (byte)MonthType.AllMonths);
                    _cbWeek.SelectedItem = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks);
                }
            }
        }

        private string DayTypeName(DayType dt)
        {
            if (dt == null) return string.Empty;

            DayType dayType = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(dt.IdDayType);

            if (dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
            {
                return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
            }
            else if (dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
            {
                return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
            }
            else
            {
                return dayType.ToString();
            }
        }

        private IList<string> CalendarOwnedDayTypes(Calendar calendar)
        {
            try
            {
                if (_actCalendar == null) return null;
                if (_actCalendar.DateSettings == null || _actCalendar.DateSettings.Count == 0) return null;

                IList<string> allDayTypes = new List<string>();
                foreach (CalendarDateSetting cads in _actCalendar.DateSettings)
                {
                    allDayTypes.Add(DayTypeName(cads.DayType));
                }
                return allDayTypes;
            }
            catch
            {
                return null;
            }
        }

        private IList<string> TimeZoneOwnedDayTypes()
        {
            try
            {
                if (_editingObject == null) return null;
                if (_editingObject.DateSettings == null || _editingObject.DateSettings.Count == 0) return null;

                IList<string> allDayTypes = new List<string>();
                foreach (TimeZoneDateSetting tzds in _editingObject.DateSettings)
                {
                    if (tzds.DayType != null)
                        allDayTypes.Add(DayTypeName(tzds.DayType));
                }
                return allDayTypes;
            }
            catch
            {
                return null;
            }
        }

        private void NotHave(Calendar calendar)
        {
            try
            {
                IList<string> calendarHave = CalendarOwnedDayTypes(calendar);
                IList<string> timeZonaHave = TimeZoneOwnedDayTypes();
                IList<string> missed = new List<string>();

                if (timeZonaHave == null) return;
                if (calendarHave == null || calendarHave.Count == 0)
                {
                    missed = timeZonaHave;
                }
                else
                {
                    foreach (string str in timeZonaHave)
                    {
                        if (!calendarHave.Contains<string>(str))
                        {
                            missed.Add(str);
                        }
                    }
                }

                if (missed.Count > 0)
                {
                    string notification = string.Empty;
                    int newlineCount = 1;

                    foreach (string s in missed)
                    {
                        if (notification != string.Empty)
                            notification += ", ";
                        if ((notification.Length / (100 * newlineCount)) > 0)
                        {
                            notification += Environment.NewLine;
                            newlineCount++;
                        }
                        notification += s;
                    }

                    notification = GetString("ErrorCalendarMissDayTypes") + Environment.NewLine + notification;

                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmCalendar.ImageTextBox,
                    notification, Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private IList<string> CalendarNotHave(Calendar calendar)
        {
            try
            {
                IList<string> calendarHave = CalendarOwnedDayTypes(calendar);
                IList<string> timeZonaHave = TimeZoneOwnedDayTypes();
                IList<string> missed = new List<string>();

                if (timeZonaHave == null) return null;
                if (calendarHave == null || calendarHave.Count == 0)
                {
                    missed = timeZonaHave;
                }
                else
                {
                    foreach (string str in timeZonaHave)
                    {
                        if (!calendarHave.Contains<string>(str))
                        {
                            missed.Add(str);
                        }
                    }
                }
                return missed;
            }
            catch
            { }
            return null;
        }

        private void _bCalendarDate_Click(object sender, EventArgs e)
        {
            SetDate setDate = new SetDate(DateTime.Now);
            DateForm calendarForm = new DateForm(LocalizationHelper, setDate);
            calendarForm.ShowDialog();

            if (setDate.IsSetDate)
            {
                _cbYear.SelectedItem = (short)setDate.Date.Year;
                _cbWeek.SelectedItem = null;
                TimeZoneMonths month = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, _timeZoneMonthsList, (byte)(setDate.Date.Month + 9));
                _cbMonth.SelectedItem = month;
                _cbWeek.SelectedItem = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks);
                TimeZoneDays day = TimeZoneDays.GetTimeZoneDay(LocalizationHelper, _timeZoneDaysList, (byte)(setDate.Date.Day + 9));
                _cbDay.SelectedItem = day;
            }
        }

        private void _dgValues_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _dgValues_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                DataGridView.HitTestInfo testInfo = _cdgvData.DataGrid.HitTest(_cdgvData.DataGrid.PointToClient(new Point(e.X, e.Y)).X, _cdgvData.DataGrid.PointToClient(new Point(e.X, e.Y)).Y);
                if (testInfo.ColumnIndex == _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Index &&
                    testInfo.RowIndex >= 0)
                    UpdateSecurityTimeZoneDateSetting((object)e.Data.GetData(output[0]), testInfo.RowIndex);
                else
                    AddTimeZoneDateSetting((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void UpdateSecurityTimeZoneDateSetting(object newDailyPlan, int rowIndex)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            try
            {
                if (newDailyPlan.GetType() == typeof(DailyPlan))
                {
                    DailyPlan dailyPlan = newDailyPlan as DailyPlan;
                    TimeZoneDateSetting dateSetting = _bindingSource.List[rowIndex] as TimeZoneDateSetting;
                    dateSetting.DailyPlan = dailyPlan;
                    if (!Insert)
                    {
                        Exception error;
                        bool retValue = CgpClient.Singleton.MainServerProvider.TimeZones.Update(_editingObject, out error);
                        if (!retValue)
                        {
                            if (error is Contal.IwQuick.SqlUniqueException)
                            {
                                if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                                {
                                    Contal.IwQuick.UI.Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDuplicatedTimeZoneSettingPair"));
                                }
                            }
                            else if (error is Contal.IwQuick.IncoherentDataException)
                            {
                                if (!Contal.IwQuick.UI.Dialog.WarningQuestion(CgpClient.Singleton.LocalizationHelper.GetString("QuestionLoadActualData")))
                                {
                                    return;
                                }
                            }
                            else
                                Contal.IwQuick.UI.Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertFailed"));

                            _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                            LoadTimeZoneDateSettings();
                            return;
                        }

                        _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                    }

                    LoadTimeZoneDateSettings();
                    CgpClientMainForm.Singleton.AddToRecentList(newDailyPlan);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cdgvData.DataGrid,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void AddTimeZoneDateSetting(object newDailyPlan)
        {
            try
            {
                if (newDailyPlan.GetType() == typeof(DailyPlan))
                {
                    DailyPlan dailyPlan = newDailyPlan as DailyPlan;
                    TimeZoneDateSetting dateSetting = new TimeZoneDateSetting();
                    dateSetting.TimeZone = _editingObject;
                    dateSetting.DailyPlan = dailyPlan;
                    dateSetting.DayType = null;
                    dateSetting.Day = (byte)DaySelection.allDays;
                    dateSetting.Week = (byte)WeekSelection.allWeeks;
                    dateSetting.Month = (byte)MonthType.AllMonths;
                    dateSetting.Year = (byte)YearSelection.AllYears;

                    if (_editingObject.DateSettings == null)
                        _editingObject.DateSettings = new List<TimeZoneDateSetting>();

                    _editingObject.DateSettings.Add(dateSetting);

                    if (!Insert)
                    {
                        Exception error;
                        bool retValue = CgpClient.Singleton.MainServerProvider.TimeZones.Update(_editingObject, out error);
                        if (!retValue)
                        {
                            _editingObject.DateSettings.Remove(dateSetting);
                            if (error is Contal.IwQuick.SqlUniqueException)
                            {
                                if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                                {
                                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedTimeZoneSettingPair"));
                                    return;
                                }
                            }

                            if (error is Contal.IwQuick.IncoherentDataException)
                            {
                                if (Contal.IwQuick.UI.Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                                {
                                    SetValues();
                                    SetDateSettingValues(true);
                                }
                                else
                                {
                                    _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                                    LoadTimeZoneDateSettings();
                                    SetDateSettingValues(false);
                                }

                                return;
                            }

                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertFailed"));
                            return;
                        }

                        _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                    }

                    LoadTimeZoneDateSettings();
                    LoadDayStatus();
                    CgpClientMainForm.Singleton.AddToRecentList(newDailyPlan);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cdgvData.DataGrid,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.TimeZones.
                GetReferencedObjects(_editingObject.IdTimeZone as object, CgpClient.Singleton.GetListLoadedPlugins());
        }


        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UserFolders_MouseDoubleClick(_lbUserFolders);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void _tbmCalendar_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                Modify();
            }
            else if (item.Name == "_tsiCreate")
            {
                Calendar calendar = new Calendar();
                if (CalendarsForm.Singleton.OpenInsertDialg(ref calendar))
                {
                    SetCalendar(calendar);
                }
            }
        }

        private void _tbmDailyPlan_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify1")
            {
                ModifyDailyPlan();
            }
            else if (item.Name == "_tsiCreate1")
            {
                DailyPlan dailyPlan = new DailyPlan();
                if (DailyPlansForm.Singleton.OpenInsertDialg(ref dailyPlan))
                {
                    _actDailyPlan = dailyPlan;
                    RefreshDailyPlan();
                }
            }
        }

        protected override void AfterDockUndock()
        {
            _firstTimeDgValues = true;
            LoadTimeZoneDateSettings();
        }

        private void _bRefreshDays_Click(object sender, EventArgs e)
        {
            LoadDayTypes();
        }

        private void _tcDateSettingsStatus_Enter(object sender, EventArgs e)
        {
            LoadDayStatus();
        }

        private void TimeZoneEditForm_TextChanged(object sender, EventArgs e)
        {
            _changedByUser = true;
        }

        private void UpdateDateSettings()
        {
            UpdateEditingObject();
            LoadTimeZoneDateSettings();
            if (_cdgvData.DataGrid != null)
                _cdgvData.DataGrid.CancelEdit();
            if (_bindingSource != null)
                _bindingSource.CancelEdit();
        }

        private void UpdateEditingObject()
        {
            if (!Insert)
            {
                Exception error;
                bool retValue = CgpClient.Singleton.MainServerProvider.TimeZones.Update(_editingObject, out error);
                if (!retValue)
                {
                    if (error is Contal.IwQuick.SqlUniqueException)
                    {
                        if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                        {
                            Contal.IwQuick.UI.Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDuplicatedTimeZoneSettingPair"));
                        }
                    }

                    else if (error is Contal.IwQuick.IncoherentDataException)
                    {
                        if (!Contal.IwQuick.UI.Dialog.WarningQuestion(CgpClient.Singleton.LocalizationHelper.GetString("QuestionLoadActualData")))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (error != null)
                            Contal.IwQuick.UI.Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditFailed") + ": " + error.Message);
                        else
                            Contal.IwQuick.UI.Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditFailed") + ": unknown error");
                    }

                    _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
                    LoadTimeZoneDateSettings();
                    return;
                }

                _editingObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEdit(_editingObject.IdTimeZone, out error);
            }
        }

        private void _dgValues_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == -1)
            {
                SetDate setDate = new SetDate(DateTime.Now);
                DateForm calendarForm = new DateForm(CgpClient.Singleton.LocalizationHelper, setDate);
                calendarForm.ShowDialog();

                if (setDate.IsSetDate)
                {
                    TimeZoneDateSetting dateSetting = _bindingSource.List[e.RowIndex] as TimeZoneDateSetting;
                    if (dateSetting == null)
                        return;

                    dateSetting.SetYear = (short)setDate.Date.Year;
                    dateSetting.Year = (byte)YearSelection.Year;
                    TimeZoneMonths month = TimeZoneMonths.GetTimeZoneMonth(CgpClient.Singleton.LocalizationHelper, _timeZoneMonthsList, (byte)(setDate.Date.Month + 9));
                    dateSetting.Month = (byte)month.Value;
                    dateSetting.Week = (byte)TimeZoneWeeks.GetTimeZoneWeek(CgpClient.Singleton.LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks).Value;
                    TimeZoneDays day = TimeZoneDays.GetTimeZoneDay(CgpClient.Singleton.LocalizationHelper, _timeZoneDaysList, (byte)(setDate.Date.Day + 9));
                    dateSetting.Day = (byte)day.Value;
                    UpdateDateSettings();
                }
            }
            else if (e.ColumnIndex == _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Index)
            {
                TimeZoneDateSetting setting = _bindingSource.Current as TimeZoneDateSetting;
                if (setting == null)
                    return;

                DailyPlansForm.Singleton.OpenEditForm(setting.DailyPlan);
            }
        }

        private bool _changedByUser = false;
        private void _dgValues_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_changedByUser)
                return;
            else
                _changedByUser = false;

            TimeZoneDateSetting dateSetting = _bindingSource.List[e.RowIndex] as TimeZoneDateSetting;

            if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "str" + TimeZoneDateSetting.COLUMNYEAR)
            {
                if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value is short)
                {
                    dateSetting.SetYear = (short)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
                    dateSetting.Year = (byte)YearSelection.Year;
                }
                else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value is YearSelection)
                {
                    dateSetting.SetYear = null;
                    dateSetting.Year =
                        (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
                }
            }
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "str" + TimeZoneDateSetting.COLUMNMONTH)
            {
                if (dateSetting.Month == (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value)
                    return;
                dateSetting.Month = (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
            }
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "str" + TimeZoneDateSetting.COLUMNWEEK)
            {
                if (dateSetting.Week == (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value)
                    return;
                dateSetting.Week = (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
            }
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "str" + TimeZoneDateSetting.COLUMNDAY)
            {
                if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value is Guid)
                {
                    DayType dt = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById((Guid)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value);
                    if (dt != null)
                    {
                        dateSetting.Day = null;
                        dateSetting.DayType = dt;
                    }
                }
                else
                {
                    if (dateSetting.Day == (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value)
                        return;
                    dateSetting.DayType = null;
                    dateSetting.Day = (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
                }
            }
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == TimeZoneDateSetting.COLUMNDESCRIPTION)
            {
                string value = string.Empty;
                if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value != null)
                    value = _cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value.ToString();

                dateSetting.Description = value;
            }
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == TimeZoneDateSetting.COLUMNEXPLICITDAILYPLAN)
            {
                dateSetting.ExplicitDailyPlan = (bool)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
            }
            else
                return;

            UpdateDateSettings();
        }

        private void _dgValues_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox)
            {
                (e.Control as ComboBox).SelectionChangeCommitted -= new EventHandler(TimeZoneEditForm_TextChanged);
                (e.Control as ComboBox).SelectionChangeCommitted += new EventHandler(TimeZoneEditForm_TextChanged);
            }
            else if (e.Control is TextBox)
            {
                (e.Control as TextBox).TextChanged -= new EventHandler(TimeZoneEditForm_TextChanged);
                (e.Control as TextBox).TextChanged += new EventHandler(TimeZoneEditForm_TextChanged);
            }
        }

        private void _dgValues_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_cdgvData.DataGrid.CurrentCell == null)
                return;

            if (_cdgvData.DataGrid.CurrentCell is DataGridViewCheckBoxCell)
                _changedByUser = true;
        }

        private void _cmsDailyPlan_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (_cdgvData.DataGrid.CurrentCell == null)
                return;

            TimeZoneDateSetting dateSetting = _bindingSource.List[_cdgvData.DataGrid.CurrentCell.RowIndex] as TimeZoneDateSetting;
            if (dateSetting == null)
                return;

            if (e.ClickedItem.Name == "_tsiCreate2")
            {
                DailyPlan dailyPlan = new DailyPlan();
                if (DailyPlansForm.Singleton.OpenInsertDialg(ref dailyPlan))
                {
                    dateSetting.DailyPlan = dailyPlan;
                }
            }
            else if (e.ClickedItem.Name == "_tsiModify2")
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;
                Exception error = null;
                try
                {
                    List<IModifyObject> listDailyPlans = new List<IModifyObject>();

                    IList<IModifyObject> listDailyPlansFromDatabase = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                    if (error != null) throw error;
                    DbsSupport.TranslateDailyPlans(listDailyPlansFromDatabase);
                    listDailyPlans.AddRange(listDailyPlansFromDatabase);

                    ListboxFormAdd formAdd = new ListboxFormAdd(listDailyPlans, GetString("DailyPlansFormDailyPlansForm"));
                    object outDailyPlan;
                    formAdd.ShowDialog(out outDailyPlan);
                    if (outDailyPlan != null)
                    {
                        var dpModifyObject = (IModifyObject)outDailyPlan;
                        DailyPlan dp = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(dpModifyObject.GetId);
                        dateSetting.DailyPlan = dp;
                        CgpClientMainForm.Singleton.AddToRecentList(dp);
                    }
                }
                catch
                {
                    Contal.IwQuick.UI.Dialog.Error(error);
                }
            }

            UpdateDateSettings();
        }

        private void _dgValues_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex == _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Index)
            {
                _cdgvData.DataGrid.CurrentCell = _cdgvData.DataGrid[e.ColumnIndex, e.RowIndex];
                _cmsDailyPlan.Show(Cursor.Position);
            }
        }

        private void _dgValues_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!_cdgvData.DataGrid.Columns.Contains("str" + TimeZoneDateSetting.COLUMNDAILYPLAN))
                return;

            if (e.ColumnIndex == _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAILYPLAN].Index)
            {
                e.CellStyle.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                e.CellStyle.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
            }
        }
    }

    public class ComboBoxItem
    {
        public ComboBoxItem(object value, string name)
        {
            _value = value;
            _name = name;
        }

        public ComboBoxItem(object value)
        {
            _value = value;
            _name = _value.ToString();
        }

        private object _value = null;

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _name = string.Empty;

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
