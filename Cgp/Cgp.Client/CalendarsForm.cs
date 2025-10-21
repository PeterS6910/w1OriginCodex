using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Contal.Cgp.BaseLib;

using Contal.IwQuick;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Client
{
    public partial class CalendarsForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<Calendar, CalendarShort>
#endif
    {
        public CalendarsForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvData.ImageList = ObjectImageList.Singleton.ClientObjectImages;
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (CalendarShort calendar in bindingSource)
            {
                calendar.Symbol = _cdgvData.GetDefaultImage(calendar);
            }
        }

        private static volatile CalendarsForm _singleton = null;
        private static object _syncRoot = new object();

        public static CalendarsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new CalendarsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }
                return _singleton;
            }
        }

        protected override Calendar GetObjectForEdit(CalendarShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.Calendars.GetObjectForEditById(listObj.IdCalendar, out editAllowed);
        }

        protected override Calendar GetFromShort(CalendarShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.Calendars.GetObjectById(listObj.IdCalendar);
        }

        protected override Calendar GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.Calendars.GetObjectById(idObj);
        }

        #region CRUD Calendar
        private void InsertClick(object sender, EventArgs e)
        {
            Insert();
        }

        private void EditClick(object sender, EventArgs e)
        {
            if (_cdgvData.SelectedRows.Count == 1)
            {
                Edit_Click();
            }
            else
            {
                DataGridViewSelectedRowCollection selected = _cdgvData.SelectedRows;
                for (int i = 0; i < selected.Count; i++)
                {
                    EditFromPosition(selected[i].Index);
                }
            }
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            if (_cdgvData.SelectedRows.Count == 1)
            {
                Calendar onDeleteObj = new Calendar();
                if (base.GetObjectFromActualRow(ref onDeleteObj))
                {
                    string msg;
                    if (DeleteObjectNotAllowed(onDeleteObj, out msg))
                    {
                        Contal.IwQuick.UI.Dialog.Error(msg);
                        return;
                    }
                    Delete_Click();
                }
            }
            else
            {
                MultiselectDeleteClic(_cdgvData.SelectedRows);
            }
        }

        protected override bool DeleteObjectNotAllowed(Calendar obj, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (obj == null) return false;

            Exception error;
            Calendar deleteObj = CgpClient.Singleton.MainServerProvider.Calendars.GetObjectById(obj.IdCalendar);

            if (deleteObj == null) return false;
            if (deleteObj.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT)
            {
                errorMsg = GetString("ErrorDeleteImplicitCalendar");
                return true;
            }
            return false;
        }

        protected override void DeleteObj(Calendar obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.Calendars.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.Calendars.DeleteById(idObj, out error))
                throw error;
        }

        protected override ACgpEditForm<Calendar> CreateEditForm(Calendar obj, ShowOptionsEditForm showOption)
        {
            return new CalendarEditForm(obj, showOption);
        }

        #endregion

        #region Filters
        private void RunFilterClick(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();
            if (!string.IsNullOrEmpty(_eNameFilter.Text))
            {
                FilterSettings filterSettingName = new FilterSettings(Calendar.COLUMNCALENDARNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingName);
            }
        }

        private void FilterClearClick(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
        }

        #endregion

        #region RefreshDataGrid
        protected override ICollection<CalendarShort> GetData()
        {
            Exception error;
            ICollection<CalendarShort> list = CgpClient.Singleton.MainServerProvider.Calendars.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(Calendar obj1, Calendar obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, CalendarShort.COLUMN_SYMBOL, CalendarShort.COLUMNCALENDARNAME, CalendarShort.COLUMNDESCRIPTION);

            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                string actValue = (string)row.Cells[CalendarShort.COLUMNCALENDARNAME].Value;
                if (actValue == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                {
                    row.Cells[CalendarShort.COLUMNCALENDARNAME].Value = GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                }
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }
        #endregion

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
        }

        protected override void FilterKeyDown(object sender, KeyEventArgs e)
        {
            base.FilterKeyDown(sender, e);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Calendars.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(Calendar calendar)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Calendars.HasAccessViewForObject(calendar);

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Calendars.HasAccessInsert();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessDelete()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Calendars.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
