using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Client
{
    public partial class DayTypesForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<DayType, DayTypeShort>
#endif
    {
        public DayTypesForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private static volatile DayTypesForm _singleton = null;
        private static object _syncRoot = new object();

        public static DayTypesForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new DayTypesForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvData.ImageList = ObjectImageList.Singleton.ClientObjectImages;
            _cdgvData.CgpDataGridEvents = this;
        }

        protected override DayType GetObjectForEdit(DayTypeShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectForEditById(listObj.IdDayType, out editAllowed);
        }

        protected override DayType GetFromShort(DayTypeShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(listObj.IdDayType);
        }

        protected override DayType GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(idObj);
        }

        #region CRUD DayType
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
                DayType onDeleteObj = new DayType();
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

        protected override bool DeleteObjectNotAllowed(DayType obj, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (obj == null) return false;
            Exception error;
            DayType dayType = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(obj.IdDayType);
            if (dayType == null) return false;

            if (dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY
                    || dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
            {
                errorMsg = GetString("ErrorDeleteImplicitDayType");
                return true;
            }
            return false;
        }

        protected override void DeleteObj(DayType obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.DayTypes.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.DayTypes.DeleteById(idObj, out error))
                throw error;
        }

        protected override ACgpEditForm<DayType> CreateEditForm(DayType obj, ShowOptionsEditForm showOption)
        {
            return new DayTypeEditForm(obj, showOption);
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
                FilterSettings filterSettingName = new FilterSettings(DayType.COLUMNDAYTYPENAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingName);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
        }

        private void FilterClearClick(object sender, EventArgs e)
        {
            FilterClear_Click();
        }
        #endregion

        #region RefreshDataGrid
        protected override ICollection<DayTypeShort> GetData()
        {
            Exception error;
            ICollection<DayTypeShort> list = CgpClient.Singleton.MainServerProvider.DayTypes.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(DayType obj1, DayType obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, DayTypeShort.COLUMNDAYTYPENAME, DayTypeShort.COLUMNDESCRIPTION);

            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                string actValue = (string)row.Cells[DayType.COLUMNDAYTYPENAME].Value;
                if (actValue == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
                {
                    row.Cells[DayType.COLUMNDAYTYPENAME].Value = GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
                }
                else if (actValue == DayType.IMPLICIT_DAY_TYPE_VACATION)
                {
                    row.Cells[DayType.COLUMNDAYTYPENAME].Value = GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
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

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.DayTypes.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(DayType dayType)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.DayTypes.HasAccessViewForObject(dayType);

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
                    return CgpClient.Singleton.MainServerProvider.DayTypes.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.DayTypes.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
