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
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class CalendarEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<Contal.Cgp.Server.Beans.Calendar>
#endif
    {
        private BindingSource _bindingSource = null;
        private BindingSource _bindingSourceDayTypeView = null;
        private DayType _actDayType = null;
        private ICollection<TimeZoneYears> _timeZoneYearsList = null;
        private ICollection<TimeZoneMonths> _timeZoneMonthsList = null;
        private ICollection<TimeZoneWeeks> _timeZoneWeeksList = null;
        private ICollection<TimeZoneDays> _timeZoneDaysList = null;
        private ICollection<DayType> _dayTypeList = null;
        private CalendarDateSetting _editCalendarDateSetting = null;
        bool _firstTimeDgValues = true;

        public CalendarEditForm(Contal.Cgp.Server.Beans.Calendar calendar, ShowOptionsEditForm showOption)
            : base(calendar, showOption)
        {
            InitializeComponent();
            SetReferenceEditColors();
            WheelTabContorol = _tcDateSettingsStatus;
            _cdgvData.DataGrid.VisibleChanged += new EventHandler(DgValuesVisibleChanged);

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

        private void InitCGPDataGridView()
        {
            _cdgvData.DataGrid.MouseDoubleClick += new MouseEventHandler(_dgValues_MouseDoubleClick);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageCalendarSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesCalendarSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesCalendarSettingsAdmin)));

                HideDisableTabPageCalendarStatus(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesCalendarStatusView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesCalendarStatusAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.CalendarsDayTypesDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageCalendarSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageCalendarSettings),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eName.ReadOnly = true;
                }

                if (!view && !admin)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tpCalendarSettings);
                    return;
                }

                _tpCalendarSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageCalendarStatus(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageCalendarStatus),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tpDayTypeStatus);
                    return;
                }

                _tpDayTypeStatus.Enabled = admin;
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

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        void DgValuesVisibleChanged(object sender, EventArgs e)
        {
            if (_firstTimeDgValues)
            {
                if (_editingObject == null) return;
                ShowDateSettingsGrid();
                _firstTimeDgValues = false;
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = LocalizationHelper.GetString("CalendarEditFormInsertText");
            }
            if (_editingObject.ToString() == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                CgpClientMainForm.Singleton.SetTextOpenWindow(this, true);
            else
                CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            CalendarsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            CalendarsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            CalendarsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            CalendarsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            Calendar obj = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEdit(_editingObject.IdCalendar, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectById(_editingObject.IdCalendar);
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
            CgpClient.Singleton.MainServerProvider.Calendars.RenewObjectForEdit(_editingObject.IdCalendar, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            LoadTimeZoneDateSettings();
            LoadDateSettingTypes();
            FillComboBoxDayType();
        }

        protected override void SetValuesEdit()
        {
            if (_editingObject.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT)
            {
                _eName.Text = GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                _eName.ReadOnly = true;
            }
            else
            {
                _eName.Text = _editingObject.CalendarName;
            }
            _eDescription.Text = _editingObject.Description;

            LoadTimeZoneDateSettings();
            LoadDateSettingTypes();
            FillComboBoxDayType();
            SetReferencedBy();
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = true;
            _bCalendar.Enabled = true;
            _eDate.Enabled = true;
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

            int lastPosition = 0;
            if (_bindingSource != null && _bindingSource.Count != 0)
                lastPosition = _bindingSource.Position;

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _editingObject.DateSettings;
            if (lastPosition != 0) _bindingSource.Position = lastPosition;
            _bindingSource.AllowNew = false;
            _cdgvData.DataGrid.DataSource = _bindingSource;

            if (!_cdgvData.DataGrid.Columns.Contains("str" + CalendarDateSetting.COLUMNYEAR))
            {
                _cdgvData.DataGrid.Columns.Add("str" + CalendarDateSetting.COLUMNYEAR, "str" + CalendarDateSetting.COLUMNYEAR);
                _cdgvData.DataGrid.Columns["str" + CalendarDateSetting.COLUMNYEAR].DisplayIndex = _cdgvData.DataGrid.Columns[CalendarDateSetting.COLUMNYEAR].DisplayIndex;
            }

            if (!_cdgvData.DataGrid.Columns.Contains("str" + CalendarDateSetting.COLUMNMONTH))
            {
                _cdgvData.DataGrid.Columns.Add("str" + CalendarDateSetting.COLUMNMONTH, "str" + CalendarDateSetting.COLUMNMONTH);
                _cdgvData.DataGrid.Columns["str" + CalendarDateSetting.COLUMNMONTH].DisplayIndex = _cdgvData.DataGrid.Columns[CalendarDateSetting.COLUMNMONTH].DisplayIndex;
            }

            if (!_cdgvData.DataGrid.Columns.Contains("str" + CalendarDateSetting.COLUMNWEEK))
            {
                _cdgvData.DataGrid.Columns.Add("str" + CalendarDateSetting.COLUMNWEEK, "str" + CalendarDateSetting.COLUMNWEEK);
                _cdgvData.DataGrid.Columns["str" + CalendarDateSetting.COLUMNWEEK].DisplayIndex = _cdgvData.DataGrid.Columns[CalendarDateSetting.COLUMNWEEK].DisplayIndex;
            }

            if (!_cdgvData.DataGrid.Columns.Contains("str" + CalendarDateSetting.COLUMNDAY))
            {
                _cdgvData.DataGrid.Columns.Add("str" + CalendarDateSetting.COLUMNDAY, "str" + CalendarDateSetting.COLUMNDAY);
                _cdgvData.DataGrid.Columns["str" + CalendarDateSetting.COLUMNDAY].DisplayIndex = _cdgvData.DataGrid.Columns[CalendarDateSetting.COLUMNDAY].DisplayIndex;
            }

            if (!_cdgvData.DataGrid.Columns.Contains("str" + CalendarDateSetting.COLUMNDAYTYPE))
            {
                _cdgvData.DataGrid.Columns.Add("str" + CalendarDateSetting.COLUMNDAYTYPE, "str" + CalendarDateSetting.COLUMNDAYTYPE);
                _cdgvData.DataGrid.Columns["str" + CalendarDateSetting.COLUMNDAYTYPE].DisplayIndex = _cdgvData.DataGrid.Columns[CalendarDateSetting.COLUMNDAYTYPE].DisplayIndex;
            }



            if (_cdgvData.DataGrid.Columns.Contains(CalendarDateSetting.COLUMNDESCRIPTION))
                _cdgvData.DataGrid.Columns[CalendarDateSetting.COLUMNDESCRIPTION].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNIDCALENDARDATESETTING);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNYEAR);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNYEARFROM);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNWEEK);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNDAY);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNMONTH);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNCALENDAR);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNDAYTYPE);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.COLUMNGUIDDAYTYPE);
            HideColumnDgw(_cdgvData.DataGrid, CalendarDateSetting.ColumnVersion);

            _cdgvData.DataGrid.AutoGenerateColumns = false;
            _cdgvData.DataGrid.AllowUserToAddRows = false;

            CgpClient.Singleton.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvData.DataGrid);

            if (_editCalendarDateSetting == null)
                ClearDateSettingsValues();

            ShowDateSettingsGrid();
        }

        private void ShowDateSettingsGrid()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(ShowDateSettingsGrid));
            }
            else
            {
                if (_cdgvData.DataGrid.DataSource == null) return;
                foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                {
                    CalendarDateSetting dateSetting = (CalendarDateSetting)_bindingSource.List[row.Index];
                    TimeZoneYears year = TimeZoneYears.GetTimeZoneYear(LocalizationHelper, dateSetting.Year);
                    if (year != null)
                    {
                        if (year.Value == YearSelection.AllYears)
                            row.Cells["str" + CalendarDateSetting.COLUMNYEAR].Value = year.Name;
                        else if (year.Value == YearSelection.Year && dateSetting.SetYear != null)
                            row.Cells["str" + CalendarDateSetting.COLUMNYEAR].Value = dateSetting.SetYear.Value.ToString();
                        else
                        {
                            row.Cells["str" + CalendarDateSetting.COLUMNYEAR].Value = string.Empty;
                        }
                    }
                    else
                    {
                        row.Cells["str" + CalendarDateSetting.COLUMNYEAR].Value = string.Empty;
                    }

                    TimeZoneMonths month = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, dateSetting.Month);
                    if (month != null)
                    {
                        row.Cells["str" + CalendarDateSetting.COLUMNMONTH].Value = month.Name;
                    }
                    else
                    {
                        row.Cells["str" + CalendarDateSetting.COLUMNMONTH].Value = string.Empty;
                    }

                    TimeZoneWeeks week = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, dateSetting.Week);
                    if (week != null)
                    {
                        row.Cells["str" + CalendarDateSetting.COLUMNWEEK].Value = week.Name;
                    }
                    else
                    {
                        row.Cells["str" + CalendarDateSetting.COLUMNWEEK].Value = string.Empty;
                    }

                    TimeZoneDays day = TimeZoneDays.GetTimeZoneDay(LocalizationHelper, dateSetting.Day);
                    if (day != null)
                    {
                        row.Cells["str" + CalendarDateSetting.COLUMNDAY].Value = day.Name;
                    }
                    else
                    {
                        row.Cells["str" + CalendarDateSetting.COLUMNDAY].Value = string.Empty;
                    }

                    DayType actDayType = (DayType)row.Cells[CalendarDateSetting.COLUMNDAYTYPE].Value;
                    row.Cells["str" + CalendarDateSetting.COLUMNDAYTYPE].Value = GetNameFromDayType(actDayType);
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
            _cbDay.BeginUpdate();
            _cbDay.Items.Clear();
            foreach (TimeZoneDays timeZoneDay in _timeZoneDaysList)
            {
                _cbDay.Items.Add(timeZoneDay);
            }
            _cbDay.SelectedItem = TimeZoneDays.GetTimeZoneDay(LocalizationHelper, _timeZoneDaysList, (byte)DaySelection.allDays);
            _cbDay.EndUpdate();
        }

        protected override bool GetValues()
        {
            try
            {
                if (_editingObject.CalendarName != Calendar.IMPLICIT_CALENDAR_DEFAULT)
                {
                    _editingObject.CalendarName = _eName.Text;
                }
                _editingObject.Description = _eDescription.Text;

                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool CheckValues()
        {
            if (_eName.Text == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                GetString("ErrorEntryCalendarName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.Calendars.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message.Contains("Name"))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedCalendarName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else if (error is Contal.IwQuick.SqlUniqueException && error.Message == "SetYear,Year,Month,Week,Day,Calendar,DayType")
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedCalendarSettingPair"));
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
                retValue = CgpClient.Singleton.MainServerProvider.Calendars.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = CgpClient.Singleton.MainServerProvider.Calendars.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message.Contains("Name"))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedCalendarName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else if (error is Contal.IwQuick.SqlUniqueException && error.Message == "SetYear,Year,Month,Week,Day,Calendar,DayType")
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedCalendarSettingPair"));
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

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            DayType dayType = new DayType();
            if (DayTypesForm.Singleton.OpenInsertDialg(ref dayType))
            {
                _actDayType = dayType;
                _cbDayType.Text = GetNameFromDayType(_actDayType);
                FillComboBoxDayType();
                _cbDayType.SelectedItem = GetNameFromDayType(_actDayType);
                //_cbDayType.Text = _actDayType.DayTypeName;
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

        private void Delete()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            CalendarDateSetting dateSetting = (CalendarDateSetting)_bindingSource.List[_bindingSource.Position];

            if (dateSetting == _editCalendarDateSetting)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _bCancelEdit,
                GetString("ErrorDeleteEditing"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _bCancelEdit.Focus();
                return;
            }

            _editingObject.DateSettings.Remove(dateSetting);

            if (!Insert)
            {
                Exception error;
                if (!CgpClient.Singleton.MainServerProvider.Calendars.Update(_editingObject, out error))
                {
                    if (error is Contal.IwQuick.IncoherentDataException)
                    {
                        if (Contal.IwQuick.UI.Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                        {
                            SetValues();
                            SetDateSettingValues(dateSetting);
                        }
                        else
                        {
                            _editingObject = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEdit(_editingObject.IdCalendar, out error);
                            LoadTimeZoneDateSettings();
                            SetDateSettingValues(dateSetting);
                        }
                    }
                    else
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteFailed"));

                    return;
                }

                _editingObject = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEdit(_editingObject.IdCalendar, out error);
            }

            EditTextChanger(null, null);
            LoadTimeZoneDateSettings();
        }

        private bool ControlDateSettingsValues()
        {
            if (_cbYear.SelectedItem == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbYear,
                GetString("ErrorEntryCalendarYear"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _cbYear.Focus();
                return false;
            }

            if (_cbMonth.SelectedItem == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbMonth,
                GetString("ErrorEntryCalendarMonth"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _cbMonth.Focus();
                return false;
            }

            if (_cbWeek.SelectedItem == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbWeek,
                GetString("ErrorEntryCalendarWeek"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _cbWeek.Focus();
                return false;
            }

            if (_cbDay.SelectedItem == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbDay,
                GetString("ErrorEntryCalendarDay"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _cbDay.Focus();
                return false;
            }

            if (_actDayType == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbDayType,
                GetString("ErrorEntryCalendarDayType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _cbDayType.Focus();
                return false;
            }

            return true;
        }

        private void GetDateSettingValues(CalendarDateSetting dateSetting)
        {
            dateSetting.DayType = _actDayType;
            dateSetting.Calendar = _editingObject;

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
            dateSetting.Description = _eDescriptionDateSettings.Text;
        }

        private void _bCreateDateSetting_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (!ControlDateSettingsValues())
                return;

            if (IsDuplicitPair())
                return;

            CalendarDateSetting dateSetting = new CalendarDateSetting();
            GetDateSettingValues(dateSetting);

            if (_editingObject.DateSettings == null)
                _editingObject.DateSettings = new List<CalendarDateSetting>();

            _editingObject.DateSettings.Add(dateSetting);

            if (!Insert)
            {
                Exception error;
                bool retValue = CgpClient.Singleton.MainServerProvider.Calendars.Update(_editingObject, out error);
                if (!retValue)
                {
                    _editingObject.DateSettings.Remove(dateSetting);
                    if (error is Contal.IwQuick.SqlUniqueException)
                    {
                        if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,Calendar,DayType")
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedTimeZoneSettingPair"));
                            return;
                        }
                    }

                    if (error is Contal.IwQuick.IncoherentDataException)
                    {
                        if (Contal.IwQuick.UI.Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                        {
                            bool allowEdit;
                            ReloadEditingObject(out allowEdit);
                            SetValues();
                            SetDateSettingValues(dateSetting);
                        }
                        else
                        {
                            _editingObject = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEdit(_editingObject.IdCalendar, out error);
                            LoadTimeZoneDateSettings();
                            SetDateSettingValues(dateSetting);
                        }

                        return;
                    }

                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertFailed"));
                    return;
                }

                _editingObject = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEdit(_editingObject.IdCalendar, out error);
            }

            EditTextChanger(null, null);
            LoadTimeZoneDateSettings();
        }

        private void SetDateSettingValues(CalendarDateSetting dateSetting)
        {
            TimeZoneYears year = TimeZoneYears.GetTimeZoneYear(LocalizationHelper, _timeZoneYearsList, dateSetting.Year);
            _cbYear.SelectedItem = year;

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

            TimeZoneMonths month = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, _timeZoneMonthsList, dateSetting.Month);
            _cbMonth.SelectedItem = month;

            TimeZoneWeeks week = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, _timeZoneWeeksList, dateSetting.Week);
            _cbWeek.SelectedItem = week;

            TimeZoneDays day = TimeZoneDays.GetTimeZoneDay(LocalizationHelper, _timeZoneDaysList, dateSetting.Day);
            _cbDay.SelectedItem = day;

            _eDescriptionDateSettings.Text = dateSetting.Description;

            _actDayType = dateSetting.DayType;
            _cbDayType.SelectedItem = GetNameFromDayType(_actDayType);
            _cbDayType.Text = GetNameFromDayType(_actDayType);
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;

            CalendarDateSetting dateSetting = (CalendarDateSetting)_bindingSource.List[_bindingSource.Position];
            SetDateSettingValues(dateSetting);

            _editCalendarDateSetting = dateSetting;
            _bCreateCalendarSetting.Visible = false;
            _bUpdateDateSetting.Visible = true;
            _bCancelEdit.Visible = true;
        }

        private void _bCancelEdit_Click(object sender, EventArgs e)
        {
            _editCalendarDateSetting = null;
            _bCreateCalendarSetting.Visible = true;
            _bUpdateDateSetting.Visible = false;
            _bCancelEdit.Visible = false;

            ClearDateSettingsValues();
        }

        private void _bUpdateDateSettings_Click(object sender, EventArgs e)
        {
            _editCalendarDateSetting = null;

            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (!ControlDateSettingsValues())
                return;

            CalendarDateSetting dateSetting = (CalendarDateSetting)_bindingSource.List[_bindingSource.Position];
            GetDateSettingValues(dateSetting);

            _bCreateCalendarSetting.Visible = true;
            _bUpdateDateSetting.Visible = false;
            _bCancelEdit.Visible = false;

            EditTextChanger(null, null);
            LoadTimeZoneDateSettings();
        }

        private void ClearDateSettingsValues()
        {
            _cbYear.SelectedItem = TimeZoneYears.GetTimeZoneYear(LocalizationHelper, _timeZoneYearsList, (byte)YearSelection.AllYears);
            _cbMonth.SelectedItem = TimeZoneMonths.GetTimeZoneMonth(LocalizationHelper, _timeZoneMonthsList, (byte)MonthType.AllMonths);
            _cbWeek.SelectedItem = TimeZoneWeeks.GetTimeZoneWeek(LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks);
            _cbDay.SelectedItem = TimeZoneDays.GetTimeZoneDay(LocalizationHelper, _timeZoneDaysList, (byte)DaySelection.allDays);
            _actDayType = null;
            _cbDayType.SelectedItem = null;
            _cbDayType.Text = GetNameFromDayType(_actDayType);
            _eDescriptionDateSettings.Text = string.Empty;
        }

        private void _eDailyPlan_DoubleClick(object sender, EventArgs e)
        {
            if (_actDayType != null)
                DayTypesForm.Singleton.OpenEditForm(_actDayType);
        }

        private void _eDailyPlan_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eDailyPlan_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddDailyPlan((object)e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddDailyPlan(object newDayType)
        {
            try
            {
                if (newDayType.GetType() == typeof(DayType))
                {
                    DayType dayType = newDayType as DayType;
                    _actDayType = dayType;
                    _cbDayType.Text = GetNameFromDayType(_actDayType);
                    CgpClientMainForm.Singleton.AddToRecentList(newDayType);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbDayType,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionDeleteConfirm")))
            {
                Delete();
            }
        }

        private void _bUpdateDateSetting_Click(object sender, EventArgs e)
        {
            _editCalendarDateSetting = null;

            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (!ControlDateSettingsValues())
                return;

            if (IsDuplicitPair())
                return;

            CalendarDateSetting dateSetting = (CalendarDateSetting)_bindingSource.List[_bindingSource.Position];
            GetDateSettingValues(dateSetting);

            if (!Insert)
            {
                Exception error;
                bool retValue = CgpClient.Singleton.MainServerProvider.Calendars.Update(_editingObject, out error);
                if (!retValue)
                {
                    _editingObject.DateSettings.Remove(dateSetting);
                    if (error is Contal.IwQuick.SqlUniqueException)
                    {
                        if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,Calendar,DayType")
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
                            SetDateSettingValues(dateSetting);
                        }
                        else
                        {
                            _editingObject = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEdit(_editingObject.IdCalendar, out error);
                            LoadTimeZoneDateSettings();
                            SetDateSettingValues(dateSetting);
                        }

                        return;
                    }

                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertFailed"));
                    return;
                }

                _editingObject = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEdit(_editingObject.IdCalendar, out error);
            }

            _bCreateCalendarSetting.Visible = true;
            _bUpdateDateSetting.Visible = false;
            _bCancelEdit.Visible = false;

            LoadTimeZoneDateSettings();
        }

        private void _dgValues_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_cdgvData.DataGrid.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            if (_bindingSource == null || _bindingSource.Count == 0) return;

            CalendarDateSetting dateSetting = (CalendarDateSetting)_bindingSource.List[_bindingSource.Position];
            SetDateSettingValues(dateSetting);

            _editCalendarDateSetting = dateSetting;
            _bCreateCalendarSetting.Visible = false;
            _bUpdateDateSetting.Visible = true;
            _bCancelEdit.Visible = true;
        }

        private void comboBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_cbDayType.Text != string.Empty)
            {
                DayTypesForm.Singleton.OpenEditForm(_actDayType);
            }
        }

        private void FillComboBoxDayType()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            Exception error;
            _dayTypeList = CgpClient.Singleton.MainServerProvider.DayTypes.List(out error);
            _cbDayType.Items.Clear();
            foreach (DayType dt in _dayTypeList)
            {
                _cbDayType.Items.Add(GetNameFromDayType(dt));
            }
        }

        private void _cbDayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cbDayType.SelectedItem == null) return;
            _actDayType = _dayTypeList.ElementAt(_cbDayType.SelectedIndex);
            EditTextChanger(sender, e);
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
                _eDate.Text = setDate.Date.ToShortDateString();
                LoadDayTypeStatus(setDate.Date);
            }
        }

        private void LoadDayTypeStatus(DateTime dateTime)
        {
            IList<String> listDayTypes = new List<String>();

            BindingList<MyList> Categories = new BindingList<MyList>();


            if (_editingObject.DateSettings != null)
            {
                foreach (CalendarDateSetting dateSetting in _editingObject.DateSettings)
                {
                    if (dateSetting.DayType != null && dateSetting.IsActual(dateTime))
                    {
                        Categories.Add(new MyList(GetNameFromDayType(dateSetting.DayType)));
                        listDayTypes.Add(GetNameFromDayType(dateSetting.DayType));
                    }
                }
            }

            _bindingSourceDayTypeView = new BindingSource();
            _bindingSourceDayTypeView.DataSource = listDayTypes;
            _cdgvDayTypeView.DataGrid.DataSource = Categories;

            if (_cdgvDayTypeView.DataGrid.Columns.Contains("DayTypeName"))
                _cdgvDayTypeView.DataGrid.Columns["DayTypeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            CgpClient.Singleton.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvDayTypeView.DataGrid);
        }

        public class MyList
        {
            private string _dayTypeName;

            public MyList(string _ListItem)
            {
                _dayTypeName = _ListItem;
            }
            public string DayTypeName
            {
                get { return _dayTypeName; }
            }
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.Calendars != null)
                CgpClient.Singleton.MainServerProvider.Calendars.EditEnd(_editingObject);
        }

        private bool IsDuplicitPair()
        {
            try
            {
                if (_editingObject.DateSettings == null) return false;

                foreach (CalendarDateSetting cds in _editingObject.DateSettings)
                {
                    short? newSetYear = null;
                    byte newYear = 0;

                    if (_cbYear.SelectedItem is TimeZoneYears)
                    {
                        YearSelection yearType = (_cbYear.SelectedItem as TimeZoneYears).Value;
                        newYear = (byte)yearType;

                        if (yearType == YearSelection.AllYears)
                        {
                            newSetYear = null;
                            newYear = (byte)yearType;
                        }
                    }
                    else if (_cbYear.SelectedItem is short)
                    {
                        newSetYear = (short)_cbYear.SelectedItem;
                        newYear = (byte)YearSelection.Year;
                    }

                    if (newSetYear == cds.SetYear &&
                        newYear == cds.Year &&
                        (byte)(_cbMonth.SelectedItem as TimeZoneMonths).Value == cds.Month &&
                        (byte)(_cbWeek.SelectedItem as TimeZoneWeeks).Value == cds.Week &&
                        (byte)(_cbDay.SelectedItem as TimeZoneDays).Value == cds.Day &&
                        _actDayType.Compare(cds.DayType))
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDuplicatedCalendarSettingPair"));
                        return true;
                    }
                }
                return false;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorControlDuplicitPairFailed"));
                return true;
            }
        }

        private string GetNameFromDayType(DayType dayType)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return null;

            if (dayType == null) return string.Empty;

            Exception error;
            dayType = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(dayType.IdDayType);

            if (dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
            {
                return GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
            }
            else if (dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
            {
                return GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
            }
            else
            {
                return dayType.ToString();
            }
        }

        private void _bCalendar1_Click(object sender, EventArgs e)
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

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.Calendars.
                GetReferencedObjects(_editingObject.IdCalendar as object, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            UserFolders_MouseDoubleClick(_lbUserFolders);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        protected override void AfterDockUndock()
        {
            _firstTimeDgValues = true;
            LoadTimeZoneDateSettings();
            if (this.MdiParent != null)
            {
                if (_editingObject.ToString() == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                    CgpClientMainForm.Singleton.SetTextOpenWindow(this, true);
            }
        }
    }
}
