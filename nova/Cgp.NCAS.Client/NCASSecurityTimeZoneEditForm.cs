using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

using DateForm = Contal.Cgp.Client.DateForm;
using SetDate = Contal.Cgp.Client.SetDate;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASSecurityTimeZoneEditForm :
#if DESIGNER
        Form
#else
        ACgpPluginEditForm<NCASClient, SecurityTimeZone>
#endif
    {
        private BindingSource _bindingSource;
        private SecurityDailyPlan _actSecurityDailyPlan;
        private Calendar _actCalendar;
        private DayType _actDayType;
        private IList<TimeZoneYears> _timeZoneYearsList;
        private IList<TimeZoneMonths> _timeZoneMonthsList;
        private IList<TimeZoneWeeks> _timeZoneWeeksList;
        private IList<TimeZoneDays> _timeZoneDaysList;
        private Action<Guid, byte> _eventStatusChanged;
        private SecurityTimeZoneDateSetting _editTimeZonesDateSetting;
        bool _firstTimeDgValues = true;

        private readonly SecurityLevelColor _secLevelColor = new SecurityLevelColor();

        public NCASSecurityTimeZoneEditForm(
                SecurityTimeZone securityTimeZone,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                securityTimeZone,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            WheelTabContorol = _tcDateSettingsStatus;
            _cdgvData.DataGrid.VisibleChanged += DgValuesVisibleChanged;
            MinimumSize = new Size(Width, Height);

            _eDescription.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            _cbYear.MouseWheel += ControlMouseWheel;
            _cbMonth.MouseWheel += ControlMouseWheel;
            _cbWeek.MouseWheel += ControlMouseWheel;
            _cbDay.MouseWheel += ControlMouseWheel;
            _eDescriptionDateSettings.MouseWheel += ControlMouseWheel;
            SetReferenceEditColors();
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
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsDateSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsDateSettingsAdmin)));

                HideDisableTabPageDailyStatus(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsDailyStatusView)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.SdpsStzsDescriptionAdmin)));
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

        private void HideDisableTabPageDailyStatus(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageDailyStatus),
                    view);
            }
            else
            {
                if (!view)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tbDailyStatus);
                }
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
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.DataGrid.CellContentClick += _dgValues_CellContentClick;
            _cdgvData.DataGrid.CellDoubleClick += _dgValues_CellDoubleClick;
            _cdgvData.DataGrid.CellFormatting += _dgValues_CellFormatting;
            _cdgvData.DataGrid.CellMouseClick += _dgValues_CellMouseClick;
            _cdgvData.DataGrid.CellValueChanged += _dgValues_CellValueChanged;
            _cdgvData.DataGrid.DataError += _dgValues_DataError;
            _cdgvData.DataGrid.DragDrop += _dgValues_DragDrop;
            _cdgvData.DataGrid.DragOver += _dgValues_DragOver;
            _cdgvData.DataGrid.EditingControlShowing += _dgValues_EditingControlShowing;
        }

        protected override void RegisterEvents()
        {
            _eventStatusChanged = ChangeStatus;
            StatusChangedSecurityTimeZoneHandler.Singleton.RegisterStatusChanged(_eventStatusChanged);
        }

        protected override void UnregisterEvents()
        {
            StatusChangedSecurityTimeZoneHandler.Singleton.UnregisterStatusChanged(_eventStatusChanged);
        }

        void DgValuesVisibleChanged(object sender, EventArgs e)
        {
            if (_firstTimeDgValues)
            {
                if (_editingObject == null) return;
                ShowGridTimeZoneDateSettings();
                _firstTimeDgValues = false;
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString("SecurityTimeZonesEditFormInsertText");
            }
            foreach (ToolStripItem item in _cmsSecDailyPlan.Items)
            {
                item.Text = GetString(Name + item.Name);
            }
            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            NCASSecurityTimeZonesForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASSecurityTimeZonesForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASSecurityTimeZonesForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASSecurityTimeZonesForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.SecurityTimeZones.GetObjectById(_editingObject.IdSecurityTimeZone);
                }
                else
                {
                    throw error;
                }
                DisableForm();
            }
            else
            {
                allowEdit = true;
            }

            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.SecurityTimeZones.RenewObjectForEdit(_editingObject.IdSecurityTimeZone, out error);

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

            _eName.Text = _editingObject.Name;
            _eDescription.Text = _editingObject.Description;
            _actCalendar = _editingObject.Calendar;

            if (_actCalendar.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT)
            {
                _tbmCalendar.Text = CgpClient.Singleton.LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                _tbmCalendar.TextImage = Plugin.GetImageForObjectType(ObjectType.Calendar);
            }
            else
            {
                _tbmCalendar.Text = _actCalendar.CalendarName;
                _tbmCalendar.TextImage = Plugin.GetImageForAOrmObject(_actCalendar);
            }

            ChangeStatus(_editingObject.IdSecurityTimeZone, Plugin.MainServerProvider.GetSecurityTimeZoneActualStatus(_editingObject.IdSecurityTimeZone));

            LoadDateSettingTypes();
            LoadTimeZoneDateSettings();
            SetReferencedBy();
        }

        protected override void DisableForm()
        {
            base.DisableForm();
            _bCancel.Enabled = true;
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
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eName,
                    GetString("ErrorInsertSecurityTimeZoneName"),
                    CgpClient.Singleton.ClientControlNotificationSettings);
                
                _eName.Focus();
                return false;
            }
            if (_actCalendar == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmCalendar.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorEntryCalendar"),
                    ControlNotificationSettings.Default);

                _tbmCalendar.ImageTextBox.Focus();
                return false;
            }

            return true;
        }

        private void CancelClick(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void OkClick(object sender, EventArgs e)
        {
            Ok_Click();
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            var retValue = Plugin.MainServerProvider.SecurityTimeZones.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message.Contains("Name"))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedSecurityTimeZoneName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
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
                retValue = Plugin.MainServerProvider.SecurityTimeZones.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = Plugin.MainServerProvider.SecurityTimeZones.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message.Contains("Name"))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedSecurityTimeZoneName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.SecurityTimeZones != null)
                Plugin.MainServerProvider.SecurityTimeZones.EditEnd(_editingObject);
        }

        private void LoadDayStatus()
        {
            var dateTime = DateTime.Now;

            if (_eDate.Text != string.Empty)
            {
                DateTime outDate;
                DateTime.TryParse(_eDate.Text, out outDate);
                dateTime = outDate;
            }
            LoadDayStatus(dateTime);
        }

        private void LoadDayStatus(DateTime dateTime)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            _eDate.Text = dateTime.ToShortDateString();

            IList<SecurityDayInterval> intervals = new List<SecurityDayInterval>();

            if (_editingObject.DateSettings != null)
            {
                intervals = Plugin.MainServerProvider.SecurityTimeZones.GetActualSecurityDayInterval(_editingObject.IdSecurityTimeZone, dateTime);
            }

            _dmSecDailyStatus.SetIntervalsWithPriority(GetIntervals(intervals), GetPriorites());
        }

        private List<Interval> GetIntervals(ICollection<SecurityDayInterval> secDayIntervals)
        {
            var slp = new SecurityLevelPriority();
            var intervals = new List<Interval>();

            if (secDayIntervals != null)
            {
                foreach (var interval in secDayIntervals)
                {
                    intervals.Add(new Interval(interval.MinutesFrom, interval.MinutesTo, interval.IntervalType, slp.GetPriorityForSecurityLevel(interval.IntervalType)));
                }
            }

            return intervals;
        }

        private byte[] GetPriorites()
        {
            var slp = new SecurityLevelPriority();
            var priorites = new byte[9];
            for (var i = 0; i < 9; i++)
            {
                priorites[i] = slp.GetPriorityForSecurityLevel((byte)i);
            }
            return priorites;
        }

        private void ChangeStatus(Guid idTimeZone, byte status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(ChangeStatus), idTimeZone, status);
            }
            else
            {
                if (_editingObject.IdSecurityTimeZone == idTimeZone)
                {
                    _eStatus.Text = GetString(((SecurityLevel4SLDP)status).ToString());
                    _eStatus.BackColor = _secLevelColor.GetCollorForSecurityLevel(status);
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

            foreach (var tzDateSetting in _editingObject.DateSettings)
            {
                if (tzDateSetting.SecurityDailyPlan != null)
                {
                    tzDateSetting.SecurityDailyPlan =
                        Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(tzDateSetting.SecurityDailyPlan.IdSecurityDailyPlan);
                }
            }

            var lastPosition = 0;
            if (_bindingSource != null && _bindingSource.Count != 0)
                lastPosition = _bindingSource.Position;

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _editingObject.DateSettings;

            if (lastPosition != 0) _bindingSource.Position = lastPosition;
            _bindingSource.AllowNew = false;
            _cdgvData.DataGrid.DataSource = _bindingSource;

            if (!_cdgvData.DataGrid.Columns.Contains("str" + TimeZoneDateSetting.COLUMNYEAR))
            {
                var yearColumn = new DataGridViewComboBoxColumn();
                yearColumn.Name = "str" + TimeZoneDateSetting.COLUMNYEAR;
                yearColumn.HeaderText = "str" + TimeZoneDateSetting.COLUMNYEAR;

                IList<ComboBoxItem> items = new List<ComboBoxItem>();
                foreach (var timeZoneYear in _timeZoneYearsList)
                {
                    if (timeZoneYear.Value == YearSelection.Year)
                    {
                        var actYear = (short)DateTime.Now.Year;

                        for (var year = actYear; year <= actYear + 30; year++)
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
                var monthColumn = new DataGridViewComboBoxColumn();
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
                var weekColumn = new DataGridViewComboBoxColumn();
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
                var dayColumn = new DataGridViewComboBoxColumn();
                dayColumn.Name = "str" + TimeZoneDateSetting.COLUMNDAY;
                dayColumn.HeaderText = "str" + TimeZoneDateSetting.COLUMNDAY;
                dayColumn.DataSource = _timeZoneDaysList;
                dayColumn.ValueMember = "Value";
                dayColumn.DisplayMember = "Name";
                dayColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                _cdgvData.DataGrid.Columns.Add(dayColumn);
                _cdgvData.DataGrid.Columns["str" + TimeZoneDateSetting.COLUMNDAY].DisplayIndex = _cdgvData.DataGrid.Columns[TimeZoneDateSetting.COLUMNDAY].DisplayIndex;
            }

            if (_cdgvData.DataGrid.Columns.Contains(SecurityTimeZoneDateSetting.COLUMNSECURITYDAILYPLAN))
            {
                _cdgvData.DataGrid.Columns[SecurityTimeZoneDateSetting.COLUMNSECURITYDAILYPLAN].ReadOnly = true;
            }

            if (_cdgvData.DataGrid.Columns.Contains(SecurityTimeZoneDateSetting.COLUMNDESCRIPTION))
                _cdgvData.DataGrid.Columns[SecurityTimeZoneDateSetting.COLUMNDESCRIPTION].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNIDSECURITYTIMEZONEDATESETTING);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNYEAR);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNSETYEAR);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNMONTH);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNWEEK);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNDAY);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNSECURITYTIMEZONE);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNGUIDSECURITYTIMEZONE);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNGUIDSECURITYDAILYPLAN);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNDAYTYPE);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.COLUMNGUIDDAYTYPE);
            HideColumnDgw(_cdgvData.DataGrid, SecurityTimeZoneDateSetting.ColumnVersion);
            _cdgvData.DataGrid.AutoGenerateColumns = false;
            _cdgvData.DataGrid.AllowUserToAddRows = false;

            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_cdgvData.DataGrid);

            if (_editTimeZonesDateSetting == null)
                ClearDateSettingsValues();

            ShowGridTimeZoneDateSettings();
            LoadDayTypes();
        }

        private void ShowGridTimeZoneDateSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowGridTimeZoneDateSettings));
            }
            else
            {
                if (_cdgvData.DataGrid.DataSource == null) return;
                foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                {
                    var dateSetting = (SecurityTimeZoneDateSetting)_bindingSource.List[row.Index];
                    var year = TimeZoneYears.GetTimeZoneYear(CgpClient.Singleton.LocalizationHelper, dateSetting.Year);
                    if (year != null)
                    {
                        if (year.Value == YearSelection.AllYears)
                        {
                            row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNYEAR].Value = year.Value;
                        }
                        else if (year.Value == YearSelection.Year && dateSetting.SetYear != null)
                        {
                            row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNYEAR].Value = dateSetting.SetYear.Value;
                        }
                        else
                        {
                            row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNYEAR].Value = null;
                        }
                    }
                    else
                    {
                        row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNYEAR].Value = null;
                    }

                    var month = TimeZoneMonths.GetTimeZoneMonth(CgpClient.Singleton.LocalizationHelper, dateSetting.Month);
                    if (month != null)
                    {
                        row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNMONTH].Value = month.Value;
                    }
                    else
                    {
                        row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNMONTH].Value = null;
                    }

                    var week = TimeZoneWeeks.GetTimeZoneWeek(CgpClient.Singleton.LocalizationHelper, dateSetting.Week);
                    if (week != null)
                    {
                        row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNWEEK].Value = week.Value;
                    }
                    else
                    {
                        row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNWEEK].Value = null;
                    }

                    var day = TimeZoneDays.GetTimeZoneDay(CgpClient.Singleton.LocalizationHelper, dateSetting.Day);
                    if (day != null)
                    {
                        row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNDAY].Value = day.Value;
                    }
                    else
                    {
                        row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNDAY].Value = null;
                    }
                    if (dateSetting.DayType != null)
                    {
                        row.Cells["str" + SecurityTimeZoneDateSetting.COLUMNDAY].Value = dateSetting.DayType.IdDayType;
                    }
                }
            }
        }

        private void LoadDateSettingTypes()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            _cbYear.Enabled = true;
            _cbMonth.Enabled = true;
            _cbWeek.Enabled = true;

            _timeZoneYearsList = TimeZoneYears.GetTimeZoneYearsList(CgpClient.Singleton.LocalizationHelper);
            _cbYear.BeginUpdate();
            _cbYear.Items.Clear();
            foreach (var timeZoneYear in _timeZoneYearsList)
            {
                if (timeZoneYear.Value == YearSelection.Year)
                {
                    var actYear = (short)DateTime.Now.Year;

                    for (var year = actYear; year <= actYear + 30; year++)
                    {
                        _cbYear.Items.Add(year);
                    }
                }
                else
                {
                    _cbYear.Items.Add(timeZoneYear);
                }
            }
            _cbYear.SelectedItem = TimeZoneYears.GetTimeZoneYear(CgpClient.Singleton.LocalizationHelper, _timeZoneYearsList, (byte)YearSelection.AllYears);
            _cbYear.EndUpdate();

            _timeZoneMonthsList = TimeZoneMonths.GetTimeZoneMonthsList(CgpClient.Singleton.LocalizationHelper);
            _cbMonth.BeginUpdate();
            _cbMonth.Items.Clear();
            foreach (var timeZoneMonth in _timeZoneMonthsList)
            {
                _cbMonth.Items.Add(timeZoneMonth);
            }
            _cbMonth.SelectedItem = TimeZoneMonths.GetTimeZoneMonth(CgpClient.Singleton.LocalizationHelper, _timeZoneMonthsList, (byte)MonthType.AllMonths);
            _cbMonth.EndUpdate();

            _timeZoneWeeksList = TimeZoneWeeks.GetTimeZoneWeeksList(CgpClient.Singleton.LocalizationHelper);
            _cbWeek.BeginUpdate();
            _cbWeek.Items.Clear();
            foreach (var timeZoneWeek in _timeZoneWeeksList)
            {
                _cbWeek.Items.Add(timeZoneWeek);
            }
            _cbWeek.SelectedItem = TimeZoneWeeks.GetTimeZoneWeek(CgpClient.Singleton.LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks);
            _cbWeek.EndUpdate();

            _timeZoneDaysList = TimeZoneDays.GetTimeZoneDaysList(CgpClient.Singleton.LocalizationHelper);
            LoadDayTypes();
        }

        private void LoadDayTypes()
        {
            _cbDay.BeginUpdate();
            _cbDay.Items.Clear();
            foreach (var timeZoneDay in _timeZoneDaysList)
            {
                _cbDay.Items.Add(timeZoneDay);
            }
            var calendarDayTypes = CgpClient.Singleton.MainServerProvider.CalendarDateSettings.GetDayTypeUniqueByCalendar(_actCalendar);
            foreach (var dayType in calendarDayTypes)
            {
                var dtcb = new DayTypeComboBox(dayType);
                _cbDay.Items.Add(dtcb);
            }
            _cbDay.SelectedItem = TimeZoneDays.GetTimeZoneDay(CgpClient.Singleton.LocalizationHelper, _timeZoneDaysList, (byte)DaySelection.allDays);
            _cbDay.EndUpdate();

            if (_cdgvData.DataGrid.Columns.Contains("str" + SecurityTimeZoneDateSetting.COLUMNDAY))
            {
                var columnItems = new List<ComboBoxItem>();
                foreach (var timeZoneDay in _timeZoneDaysList)
                {
                    columnItems.Add(new ComboBoxItem(timeZoneDay.Value, timeZoneDay.Name));
                }
                calendarDayTypes = CgpClient.Singleton.MainServerProvider.CalendarDateSettings.GetDayTypeUniqueByCalendar(_actCalendar);
                foreach (var dayType in calendarDayTypes)
                {
                    DayType dt = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(dayType.IdDayType);
                    columnItems.Add(new ComboBoxItem(dt.IdDayType, DayTypeName(dt)));
                }
                (_cdgvData.DataGrid.Columns["str" + SecurityTimeZoneDateSetting.COLUMNDAY] as DataGridViewComboBoxColumn).DataSource = columnItems;
            }
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

        private void RefreshDailyPlan()
        {
            if (_actSecurityDailyPlan == null)
            {
                _tbmDailyPlan.Text = string.Empty;
            }
            else
            {
                _tbmDailyPlan.Text = _actSecurityDailyPlan.ToString();
                _tbmDailyPlan.TextImage = Plugin.GetImageForAOrmObject(_actSecurityDailyPlan);
            }
        }

        private void ModifyDaily()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                var listModObj = new List<IModifyObject>();
                Exception error;
                var listDailyPlansFromDatabase = Plugin.MainServerProvider.SecurityDailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listDailyPlansFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, GetString("NCASSecurityDailyPlansFormNCASSecurityDailyPlansForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    _actSecurityDailyPlan = Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(outModObj.GetId);
                    RefreshDailyPlan();
                    Plugin.AddToRecentList(_actSecurityDailyPlan);
                }
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private void _bRemove_Click(object sender, EventArgs e)
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            if (Dialog.Question(GetString("QuestionDeleteConfirm")))
            {
                Delete();
            }
        }

        private void SetDateSettingValues(bool setDataFromActualObject)
        {
            if (_editTimeZonesDateSetting == null)
                return;

            SecurityTimeZoneDateSetting dateSetting = null;

            if (_editingObject.DateSettings != null)
            {
                foreach (var actDateSetting in _editingObject.DateSettings)
                {
                    if (_editTimeZonesDateSetting.Compare(actDateSetting))
                        dateSetting = actDateSetting;
                }
            }

            _editTimeZonesDateSetting = dateSetting;

            if (_editTimeZonesDateSetting != null)
            {
                if (setDataFromActualObject)
                    SetDateSettingValues(_editTimeZonesDateSetting);
            }
        }

        private void Delete()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            var dateSetting = (SecurityTimeZoneDateSetting)_bindingSource.List[_bindingSource.Position];

            foreach (var dateSet in _editingObject.DateSettings)
            {
                if (dateSetting.IdSecurityTimeZoneDateSetting == dateSet.IdSecurityTimeZoneDateSetting)
                {
                    _editingObject.DateSettings.Remove(dateSet);
                    break;
                }
            }

            if (!Insert)
            {
                Exception error;
                if (!Plugin.MainServerProvider.SecurityTimeZones.Update(_editingObject, out error))
                {
                    if (error is IncoherentDataException)
                    {
                        if (!Dialog.WarningQuestion(GetString("QuestionLoadActualData")))
                        {
                            return;
                        }
                    }
                    else
                        Dialog.Error(GetString("ErrorDeleteFailed"));

                    _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                    LoadTimeZoneDateSettings();
                    return;
                }

                _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
            }

            LoadTimeZoneDateSettings();
        }

        private bool ControlDateSettingsValues()
        {
            if (_actCalendar == null)
            {
                if (_cbYear.SelectedItem == null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbYear,
                    GetString("ErrorEntryTimeZoneYear"), ControlNotificationSettings.Default);
                    _cbYear.Focus();
                    return false;
                }

                if (_cbMonth.SelectedItem == null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbMonth,
                    GetString("ErrorEntryTimeZoneMonth"), ControlNotificationSettings.Default);
                    _cbMonth.Focus();
                    return false;
                }

                if (_cbWeek.SelectedItem == null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbWeek,
                    GetString("ErrorEntryTimeZoneWeek"), ControlNotificationSettings.Default);
                    _cbWeek.Focus();
                    return false;
                }
            }

            if (_cbDay.SelectedItem == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbDay,
                GetString("ErrorEntryTimeZoneDay"), ControlNotificationSettings.Default);
                _cbDay.Focus();
                return false;
            }

            if (_actSecurityDailyPlan == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDailyPlan.ImageTextBox,
                GetString("ErrorEntryTimeZoneDailyPlan"), ControlNotificationSettings.Default);
                _tbmDailyPlan.ImageTextBox.Focus();
                return false;
            }
            if (ContainsKeyboardNeededIntervals())
            {
                if (ContainsCardReadersWithoutKeyboard())
                {
                    Dialog.Info(GetString("ErrorDayIntervalsUnsupportedByCardReader"));
                    return false;
                }
            }

            return true;
        }

        private bool ContainsCardReadersWithoutKeyboard()
        {
            var objects = GetListReferencedObjects();

            if (objects != null)
            {
                foreach (var item in objects)
                {
                    if (item is CardReader)
                    {
                        var hasKeyboard = Plugin.MainServerProvider.CardReaders.GetHasKeyboard((item as CardReader).IdCardReader);
                        if (((item as CardReader).SecurityLevel == (byte)SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
                            && (!hasKeyboard.HasValue || !hasKeyboard.Value))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool ContainsKeyboardNeededIntervals()
        {
            if (_actSecurityDailyPlan != null)
            {
                foreach (var di in _actSecurityDailyPlan.SecurityDayIntervals)
                {
                    if (di.IntervalType == (byte)SecurityLevel4SLDP.cardpin
                        || di.IntervalType == (byte)SecurityLevel4SLDP.code
                        || di.IntervalType == (byte)SecurityLevel4SLDP.codeorcard
                        || di.IntervalType == (byte)SecurityLevel4SLDP.codeorcardpin
                        || di.IntervalType == (byte)SecurityLevel4SLDP.togglecardpin)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void GetDateSettingValues(SecurityTimeZoneDateSetting dateSetting)
        {
            dateSetting.SecurityDailyPlan = _actSecurityDailyPlan;
            dateSetting.SecurityTimeZone = _editingObject;
            dateSetting.Description = _eDescriptionDateSettings.Text;
            dateSetting.ExplicitSecurityDailyPlan = _cbExplicitDailyPlan.Checked;

            if (_actDayType == null)
            {
                dateSetting.DayType = null;
                if (_cbYear.SelectedItem is TimeZoneYears)
                {
                    var yearType = (_cbYear.SelectedItem as TimeZoneYears).Value;
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

            var dateSetting = new SecurityTimeZoneDateSetting();
            GetDateSettingValues(dateSetting);

            if (_editingObject.DateSettings == null)
                _editingObject.DateSettings = new List<SecurityTimeZoneDateSetting>();

            _editingObject.DateSettings.Add(dateSetting);

            if (!Insert)
            {
                Exception error;
                var retValue = Plugin.MainServerProvider.SecurityTimeZones.Update(_editingObject, out error);
                if (!retValue)
                {
                    _editingObject.DateSettings.Remove(dateSetting);
                    if (error is SqlUniqueException)
                    {
                        if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,SecurityTimeZone,SecurityDailyPlan,DayType")
                        {
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDuplicatedTimeZoneSettingPair"));
                        }
                    }
                    else if (error is IncoherentDataException)
                    {
                        if (!Dialog.WarningQuestion(CgpClient.Singleton.LocalizationHelper.GetString("QuestionLoadActualData")))
                        {
                            return;
                        }
                    }
                    else
                        Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertFailed"));

                    _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                    LoadTimeZoneDateSettings();
                    return;
                }

                _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
            }
            LoadTimeZoneDateSettings();
        }

        private void SetDateSettingValues(SecurityTimeZoneDateSetting dateSetting)
        {
            var year = TimeZoneYears.GetTimeZoneYear(CgpClient.Singleton.LocalizationHelper, _timeZoneYearsList, dateSetting.Year);
            _cbYear.SelectedItem = year;

            if (year != null)
            {
                if (year.Value == YearSelection.Year)
                {
                    object setItem = null;
                    foreach (var item in _cbYear.Items)
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

            var month = TimeZoneMonths.GetTimeZoneMonth(CgpClient.Singleton.LocalizationHelper, _timeZoneMonthsList, dateSetting.Month);
            _cbMonth.SelectedItem = month;

            var week = TimeZoneWeeks.GetTimeZoneWeek(CgpClient.Singleton.LocalizationHelper, _timeZoneWeeksList, dateSetting.Week);
            _cbWeek.SelectedItem = week;

            var day = TimeZoneDays.GetTimeZoneDay(CgpClient.Singleton.LocalizationHelper, _timeZoneDaysList, dateSetting.Day);
            _cbDay.SelectedItem = day;

            if (dateSetting.DayType != null)
            {
                _actDayType = dateSetting.DayType;
                //_cbDay.SelectedItem = new DayTypeComboBox(_actDayType);
                var name = DayTypeName(_actDayType);
                foreach (var obj in _cbDay.Items)
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
            _cbExplicitDailyPlan.Checked = dateSetting.ExplicitSecurityDailyPlan;

            _actSecurityDailyPlan = dateSetting.SecurityDailyPlan;
            RefreshDailyPlan();
        }

        private void UpdateEditingObject()
        {
            if (!Insert)
            {
                Exception error;
                var retValue = Plugin.MainServerProvider.SecurityTimeZones.Update(_editingObject, out error);
                if (!retValue)
                {
                    if (error is SqlUniqueException)
                    {
                        if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,SecurityTimeZone,SecurityDailyPlan,DayType")
                        {
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDuplicatedTimeZoneSettingPair"));
                        }
                    }

                    else if (error is IncoherentDataException)
                    {
                        if (!Dialog.WarningQuestion(CgpClient.Singleton.LocalizationHelper.GetString("QuestionLoadActualData")))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (error != null)
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditFailed") + ": " + error.Message);
                        else
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditFailed") + ": unknown error");
                    }

                    _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                    LoadTimeZoneDateSettings();
                    return;
                }

                _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
            }
        }

        private void ClearDateSettingsValues()
        {
            _cbYear.SelectedItem = TimeZoneYears.GetTimeZoneYear(CgpClient.Singleton.LocalizationHelper, _timeZoneYearsList, (byte)YearSelection.AllYears);
            _cbMonth.SelectedItem = TimeZoneMonths.GetTimeZoneMonth(CgpClient.Singleton.LocalizationHelper, _timeZoneMonthsList, (byte)MonthType.AllMonths);
            _cbWeek.SelectedItem = TimeZoneWeeks.GetTimeZoneWeek(CgpClient.Singleton.LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks);
            _cbDay.SelectedItem = TimeZoneDays.GetTimeZoneDay(CgpClient.Singleton.LocalizationHelper, _timeZoneDaysList, (byte)DaySelection.allDays);
            _actSecurityDailyPlan = null;
            RefreshDailyPlan();
            _eDescriptionDateSettings.Text = string.Empty;
            _cbExplicitDailyPlan.Checked = false;
        }

        private void _tbmDailyPlan_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actSecurityDailyPlan != null)
                NCASSecurityDailyPlansForm.Singleton.OpenEditForm(_actSecurityDailyPlan);
        }

        private void _tbmDailyPlan_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmDailyPlan_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddDailyPlan(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddDailyPlan(object newSecurityDailyPlan)
        {
            try
            {
                if (newSecurityDailyPlan.GetType() == typeof(SecurityDailyPlan))
                {
                    var securityDailyPlan = newSecurityDailyPlan as SecurityDailyPlan;
                    _actSecurityDailyPlan = securityDailyPlan;
                    RefreshDailyPlan();
                    Plugin.AddToRecentList(newSecurityDailyPlan);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDailyPlan.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
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

            var setDate = new SetDate(dateTime);
            var calendarForm = new DateForm(CgpClient.Singleton.LocalizationHelper, setDate);
            calendarForm.ShowDialog();

            if (setDate.IsSetDate)
            {
                LoadDayStatus(setDate.Date);
            }
        }

        private void ModifyCalendar()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                var listModObj = new List<IModifyObject>();

                Exception error;
                var listCalendarsFromDatabase = CgpClient.Singleton.MainServerProvider.Calendars.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateCalendars(listCalendarsFromDatabase);
                listModObj.AddRange(listCalendarsFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, CgpClient.Singleton.LocalizationHelper.GetString("CalendarsFormCalendarsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    var newCalendar = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectById(outModObj.GetId);
                    SetCalendar(newCalendar);
                    Plugin.AddToRecentList(_actCalendar);
                }
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
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
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddCalendar(e.Data.GetData(output[0]));
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
                    Plugin.AddToRecentList(newCalendar);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCalendar.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private bool SetCalendatToTimeZone()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return false;
            try
            {
                _editingObject.Calendar = _actCalendar;
                Exception error;
                var retValue = Plugin.MainServerProvider.SecurityTimeZones.Update(_editingObject, out error);
                if (!retValue)
                {
                    if (error is SqlUniqueException)
                    {
                        if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,TimeZone,DailyPlan,DayType")
                        {
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDuplicatedTimeZoneSettingPair"));
                        }
                        else
                        {
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorTimeZoneNameExists"));
                        }
                    }
                    else if (error is IncoherentDataException)
                    {
                        if (Dialog.WarningQuestion(CgpClient.Singleton.LocalizationHelper.GetString("QuestionLoadActualData")))
                            SetValues();
                        else
                            _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                    }
                    else
                    {
                        if (error != null)
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditFailed") + ": " + error.Message);
                        else
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditFailed") + ": unknown error");
                    }
                    return false;
                }
                _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
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
                if (CgpClient.Singleton.IsConnectionLost(false)) return;

                var oldCalendar = _actCalendar;
                _actCalendar = objCalendar;
                Exception error;

                var notDefined = CalendarNotHave(_actCalendar);
                if (notDefined != null && notDefined.Count > 0)
                {
                    var form = new NCASSecurityTimeZoneCalendarChangeForm(oldCalendar, _actCalendar, _editingObject, Insert, LocalizationHelper, this);
                    var dr = form.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                        LoadTimeZoneDateSettings();
                        if (error != null)
                        {
                            Dialog.Error(error);
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
                        if (!SetCalendatToTimeZone())
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
                        _tbmCalendar.Text = CgpClient.Singleton.LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                        _tbmCalendar.TextImage = Plugin.GetImageForObjectType(ObjectType.Calendar);

                    }
                    else
                    {
                        _tbmCalendar.Text = _actCalendar.CalendarName;
                        _tbmCalendar.TextImage = Plugin.GetImageForAOrmObject(_actCalendar);
                    }
                }
                LoadDateSettingTypes();
                UpdateDateSettings();
            }
            catch
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorSetCalendarFailed"));
            }
        }

        private class DayTypeComboBox
        {
            readonly DayType _dayType;
            public DayType DayType
            {
                get { return _dayType; }
            }

            public DayTypeComboBox(DayType dayType)
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                _dayType = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(dayType.IdDayType);
                //_dayType = dayType;
            }

            public override string ToString()
            {
                if (_dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
                {
                    return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
                }
                if (_dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
                {
                    return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
                }
                return _dayType.ToString();
            }
        }

        private class CalendarList : IListObjects
        {
            private readonly Calendar _calendar;

            public bool Contains(string expression)
            {
                if (_calendar.Contains(expression))
                    return true;
                if (ToString().ToLower().Contains(expression.ToLower()))
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
                return _calendar.CalendarName;
            }
        }

        private void _cbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            object obj;
            obj = _cbDay.SelectedItem;
            if (obj == null) return;
            if (obj.GetType() == typeof(DayTypeComboBox))
            {
                var dtCB = obj as DayTypeComboBox;
                _actDayType = dtCB.DayType;

                _cbYear.SelectedItem = null;
                _cbYear.Enabled = false;
                _cbMonth.SelectedItem = null;
                _cbMonth.Enabled = false;
                _cbWeek.SelectedItem = null;
                _cbWeek.Enabled = false;

                _cbExplicitDailyPlan.Checked = true;
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

            if (CgpClient.Singleton.IsConnectionLost(false)) return string.Empty;
            var dayType = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(dt.IdDayType);

            if (dayType == null) return string.Empty;
            if (dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
            {
                return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
            }
            if (dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
            {
                return CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
            }
            return dayType.ToString();
        }

        private IList<string> CalendarOwnedDayTypes(Calendar calendar)
        {
            try
            {
                if (_actCalendar == null) return null;
                if (_actCalendar.DateSettings == null || _actCalendar.DateSettings.Count == 0) return null;

                IList<string> allDayTypes = new List<string>();
                foreach (var cads in _actCalendar.DateSettings)
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
                foreach (var tzds in _editingObject.DateSettings)
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

        private IList<string> CalendarNotHave(Calendar calendar)
        {
            try
            {
                var calendarHave = CalendarOwnedDayTypes(calendar);
                var timeZonaHave = TimeZoneOwnedDayTypes();
                IList<string> missed = new List<string>();

                if (timeZonaHave == null) return null;
                if (calendarHave == null || calendarHave.Count == 0)
                {
                    missed = timeZonaHave;
                }
                else
                {
                    foreach (var str in timeZonaHave)
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
            var setDate = new SetDate(DateTime.Now);
            var calendarForm = new DateForm(CgpClient.Singleton.LocalizationHelper, setDate);
            calendarForm.ShowDialog();

            if (setDate.IsSetDate)
            {
                _cbYear.SelectedItem = (short)setDate.Date.Year;
                _cbWeek.SelectedItem = null;
                var month = TimeZoneMonths.GetTimeZoneMonth(CgpClient.Singleton.LocalizationHelper, _timeZoneMonthsList, (byte)(setDate.Date.Month + 9));
                _cbMonth.SelectedItem = month;
                _cbWeek.SelectedItem = TimeZoneWeeks.GetTimeZoneWeek(CgpClient.Singleton.LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks);
                var day = TimeZoneDays.GetTimeZoneDay(CgpClient.Singleton.LocalizationHelper, _timeZoneDaysList, (byte)(setDate.Date.Day + 9));
                _cbDay.SelectedItem = day;
            }
        }

        private void _tbDailyStatus_Enter(object sender, EventArgs e)
        {
            LoadDayStatus();
        }

        private void _dgValues_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _dgValues_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                var testInfo = _cdgvData.DataGrid.HitTest(_cdgvData.DataGrid.PointToClient(new Point(e.X, e.Y)).X, _cdgvData.DataGrid.PointToClient(new Point(e.X, e.Y)).Y);
                if (testInfo.ColumnIndex == _cdgvData.DataGrid.Columns[SecurityTimeZoneDateSetting.COLUMNSECURITYDAILYPLAN].Index &&
                    testInfo.RowIndex >= 0)
                    UpdateSecurityTimeZoneDateSetting(e.Data.GetData(output[0]), testInfo.RowIndex);
                else
                    AddSecurityTimeZoneDateSetting(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void UpdateSecurityTimeZoneDateSetting(object newSecurityDailyPlan, int rowIndex)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            try
            {
                if (newSecurityDailyPlan.GetType() == typeof(SecurityDailyPlan))
                {
                    var securityDailyPlan = newSecurityDailyPlan as SecurityDailyPlan;
                    var dateSetting = _bindingSource.List[rowIndex] as SecurityTimeZoneDateSetting;
                    dateSetting.SecurityDailyPlan = securityDailyPlan;
                    if (!Insert)
                    {
                        Exception error;
                        var retValue = Plugin.MainServerProvider.SecurityTimeZones.Update(_editingObject, out error);
                        if (!retValue)
                        {
                            if (error is SqlUniqueException)
                            {
                                if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,SecurityTimeZone,SecurityDailyPlan,DayType")
                                {
                                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDuplicatedTimeZoneSettingPair"));
                                }
                            }
                            else if (error is IncoherentDataException)
                            {
                                if (!Dialog.WarningQuestion(CgpClient.Singleton.LocalizationHelper.GetString("QuestionLoadActualData")))
                                {
                                    return;
                                }
                            }
                            else
                                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertFailed"));

                            _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                            LoadTimeZoneDateSettings();
                            return;
                        }

                        _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                    }

                    LoadTimeZoneDateSettings();
                    Plugin.AddToRecentList(newSecurityDailyPlan);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvData.DataGrid,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void AddSecurityTimeZoneDateSetting(object newSecurityDailyPlan)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            try
            {
                if (newSecurityDailyPlan.GetType() == typeof(SecurityDailyPlan))
                {
                    var securityDailyPlan = newSecurityDailyPlan as SecurityDailyPlan;
                    var dateSetting = new SecurityTimeZoneDateSetting();
                    dateSetting.SecurityTimeZone = _editingObject;
                    dateSetting.SecurityDailyPlan = securityDailyPlan;
                    dateSetting.DayType = null;
                    dateSetting.Day = (byte)DaySelection.allDays;
                    dateSetting.Week = (byte)WeekSelection.allWeeks;
                    dateSetting.Month = (byte)MonthType.AllMonths;
                    dateSetting.Year = (byte)YearSelection.AllYears;

                    if (_editingObject.DateSettings == null)
                        _editingObject.DateSettings = new List<SecurityTimeZoneDateSetting>();

                    _editingObject.DateSettings.Add(dateSetting);

                    if (!Insert)
                    {
                        Exception error;
                        var retValue = Plugin.MainServerProvider.SecurityTimeZones.Update(_editingObject, out error);
                        if (!retValue)
                        {
                            _editingObject.DateSettings.Remove(dateSetting);
                            if (error is SqlUniqueException)
                            {
                                if (error.Message != null && error.Message == "SetYear,Year,Month,Week,Day,SecurityTimeZone,SecurityDailyPlan,DayType")
                                {
                                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDuplicatedTimeZoneSettingPair"));
                                }
                            }
                            else if (error is IncoherentDataException)
                            {
                                if (!Dialog.WarningQuestion(CgpClient.Singleton.LocalizationHelper.GetString("QuestionLoadActualData")))
                                {
                                    return;
                                }
                            }
                            else
                                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertFailed"));

                            _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                            LoadTimeZoneDateSettings();
                            return;
                        }

                        _editingObject = Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEdit(_editingObject.IdSecurityTimeZone, out error);
                    }

                    LoadTimeZoneDateSettings();
                    Plugin.AddToRecentList(newSecurityDailyPlan);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvData.DataGrid,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, NCASClient.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return Plugin.MainServerProvider.SecurityTimeZones.
                GetReferencedObjects(_editingObject.IdSecurityTimeZone, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _tbmDailyPlan_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify1")
            {
                ModifyDaily();
            }
            else if (item.Name == "_tsiCreate1")
            {
                var securityDailyPlan = new SecurityDailyPlan();
                if (NCASSecurityDailyPlansForm.Singleton.OpenInsertDialg(ref securityDailyPlan))
                {
                    _actSecurityDailyPlan = securityDailyPlan;
                    RefreshDailyPlan();
                }
            }
        }

        private void _tbmCalendar_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyCalendar();
            }
            else if (item.Name == "_tsiCreate")
            {
                var calendar = new Calendar();
                if (CalendarsForm.Singleton.OpenInsertDialg(ref calendar))
                {
                    SetCalendar(calendar);
                }
            }
        }

        protected override void AfterDockUndock()
        {
            _firstTimeDgValues = true;
            LoadTimeZoneDateSettings();
        }

        private void _dgValues_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {


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

            private object _value;

            public object Value
            {
                get { return _value; }
                set { _value = value; }
            }

            private readonly string _name = string.Empty;

            public string Name
            {
                get
                {
                    return _name;
                }
            }
        }

        private bool _changedByUser;
        private void _dgValues_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            if (!_changedByUser)
                return;
            _changedByUser = false;

            var dateSetting = _bindingSource.List[e.RowIndex] as SecurityTimeZoneDateSetting;

            if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "str" + SecurityTimeZoneDateSetting.COLUMNYEAR)
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
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "str" + SecurityTimeZoneDateSetting.COLUMNMONTH)
            {
                if (dateSetting.Month == (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value)
                    return;
                dateSetting.Month = (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
            }
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "str" + SecurityTimeZoneDateSetting.COLUMNWEEK)
            {
                if (dateSetting.Week == (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value)
                    return;
                dateSetting.Week = (byte)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
            }
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == "str" + SecurityTimeZoneDateSetting.COLUMNDAY)
            {
                if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value is Guid)
                {
                    var dt = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById((Guid)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value);
                    if (dt != null)
                    {
                        dateSetting.Day = null;

                        if (dateSetting.DayType == null)
                        {
                            foreach (DataGridViewColumn column in _cdgvData.DataGrid.Columns)
                            {
                                if (column.Name == SecurityTimeZoneDateSetting.COLUMNEXPLICITSECURITYDAILYPLAN)
                                {
                                    _cdgvData.DataGrid[column.Index, e.RowIndex].Value = true;
                                }
                            }

                            dateSetting.ExplicitSecurityDailyPlan = true;
                        }

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
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == SecurityTimeZoneDateSetting.COLUMNDESCRIPTION)
            {
                var value = string.Empty;
                if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value != null)
                    value = _cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value.ToString();

                dateSetting.Description = value;
            }
            else if (_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == SecurityTimeZoneDateSetting.COLUMNEXPLICITSECURITYDAILYPLAN)
            {
                dateSetting.ExplicitSecurityDailyPlan = (bool)_cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].Value;
            }
            else
                return;

            UpdateDateSettings();
        }

        private void _dgValues_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox)
            {
                (e.Control as ComboBox).SelectionChangeCommitted -= NCASSecurityTimeZoneEditForm_TextChanged;
                (e.Control as ComboBox).SelectionChangeCommitted += NCASSecurityTimeZoneEditForm_TextChanged;
            }
            else if (e.Control is TextBox)
            {
                (e.Control as TextBox).TextChanged -= NCASSecurityTimeZoneEditForm_TextChanged;
                (e.Control as TextBox).TextChanged += NCASSecurityTimeZoneEditForm_TextChanged;
            }
        }

        private void NCASSecurityTimeZoneEditForm_TextChanged(object sender, EventArgs e)
        {
            _changedByUser = true;
        }

        private void _dgValues_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == -1)
            {
                var setDate = new SetDate(DateTime.Now);
                var calendarForm = new DateForm(CgpClient.Singleton.LocalizationHelper, setDate);
                calendarForm.ShowDialog();

                if (setDate.IsSetDate)
                {
                    var dateSetting = _bindingSource.List[e.RowIndex] as SecurityTimeZoneDateSetting;
                    if (dateSetting == null)
                        return;

                    dateSetting.SetYear = (short)setDate.Date.Year;
                    dateSetting.Year = (byte)YearSelection.Year;
                    var month = TimeZoneMonths.GetTimeZoneMonth(CgpClient.Singleton.LocalizationHelper, _timeZoneMonthsList, (byte)(setDate.Date.Month + 9));
                    dateSetting.Month = (byte)month.Value;
                    dateSetting.Week = (byte)TimeZoneWeeks.GetTimeZoneWeek(CgpClient.Singleton.LocalizationHelper, _timeZoneWeeksList, (byte)WeekSelection.allWeeks).Value;
                    var day = TimeZoneDays.GetTimeZoneDay(CgpClient.Singleton.LocalizationHelper, _timeZoneDaysList, (byte)(setDate.Date.Day + 9));
                    dateSetting.Day = (byte)day.Value;
                    UpdateDateSettings();
                }
            }
            else if (e.ColumnIndex == _cdgvData.DataGrid.Columns[SecurityTimeZoneDateSetting.COLUMNSECURITYDAILYPLAN].Index)
            {
                var setting = _bindingSource.Current as SecurityTimeZoneDateSetting;
                if (setting == null)
                    return;

                NCASSecurityDailyPlansForm.Singleton.OpenEditForm(setting.SecurityDailyPlan);
            }
        }

        private void UpdateDateSettings()
        {
            UpdateEditingObject();
            LoadTimeZoneDateSettings();
            _cdgvData.DataGrid.CancelEdit();
            if (_bindingSource != null)
                _bindingSource.CancelEdit();
        }

        private void _dgValues_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex == _cdgvData.DataGrid.Columns[SecurityTimeZoneDateSetting.COLUMNSECURITYDAILYPLAN].Index)
            {
                _cdgvData.DataGrid.CurrentCell = _cdgvData.DataGrid[e.ColumnIndex, e.RowIndex];
                _cmsSecDailyPlan.Show(Cursor.Position);
            }
        }

        private void _cmsSecDailyPlan_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (_cdgvData.DataGrid.CurrentCell == null)
                return;

            var dateSetting = _bindingSource.List[_cdgvData.DataGrid.CurrentCell.RowIndex] as SecurityTimeZoneDateSetting;
            if (dateSetting == null)
                return;

            if (e.ClickedItem.Name == "_tsiCreate2")
            {
                var dailyPlan = new SecurityDailyPlan();
                if (NCASSecurityDailyPlansForm.Singleton.OpenInsertDialg(ref dailyPlan))
                {
                    dateSetting.SecurityDailyPlan = dailyPlan;
                }
            }
            else if (e.ClickedItem.Name == "_tsiModify2")
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;

                Exception error = null;
                try
                {
                    var listModObj = new List<IModifyObject>();
                    var listDailyPlansFromDatabase = Plugin.MainServerProvider.SecurityDailyPlans.ListModifyObjects(out error);
                    if (error != null) throw error;
                    listModObj.AddRange(listDailyPlansFromDatabase);

                    var formAdd = new ListboxFormAdd(listModObj, GetString("NCASSecurityDailyPlansFormNCASSecurityDailyPlansForm"));
                    IModifyObject outModObj;
                    formAdd.ShowDialog(out outModObj);
                    if (outModObj != null)
                    {
                        var sdp = Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(outModObj.GetId);
                        dateSetting.SecurityDailyPlan = sdp;
                        Plugin.AddToRecentList(sdp);
                    }
                }
                catch (Exception ex)
                {
                    Dialog.Error(ex);
                }
            }

            UpdateDateSettings();
        }

        private void _dgValues_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == _cdgvData.DataGrid.Columns[SecurityTimeZoneDateSetting.COLUMNSECURITYDAILYPLAN].Index)
            {
                e.CellStyle.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                e.CellStyle.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
            }
        }

        private void _dgValues_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && _cdgvData.DataGrid[e.ColumnIndex, e.RowIndex].OwningColumn.Name == SecurityTimeZoneDateSetting.COLUMNEXPLICITSECURITYDAILYPLAN)
                NCASSecurityTimeZoneEditForm_TextChanged(sender, e);
        }
    }
}
