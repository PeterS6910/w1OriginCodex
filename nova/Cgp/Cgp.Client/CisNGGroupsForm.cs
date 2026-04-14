using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class CisNGGroupsForm : 
#if DESIGNER
        Form
#else
        ACgpTableForm<CisNGGroup, CisNGGroupShort>
#endif
    {
        public CisNGGroupsForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private static volatile CisNGGroupsForm _singleton = null;
        private static object _syncRoot = new object();

        public static CisNGGroupsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new CisNGGroupsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvData.ImageList = ObjectImageList.Singleton.GetClientObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (CisNGGroupShort cis in bindingSource)
            {
                cis.Symbol = _cdgvData.GetDefaultImage(cis);
            }
        }

        protected override CisNGGroup GetObjectForEdit(CisNGGroupShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.CisNGGroups.GetObjectForEditById(listObj.IdCisNGGroup, out editAllowed);
        }

        protected override CisNGGroup GetFromShort(CisNGGroupShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.CisNGGroups.GetObjectById(listObj.IdCisNGGroup);
        }

        protected override CisNGGroup GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.CisNGGroups.GetObjectById(idObj);
        }

        #region CRUD Cis.NG Groups
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
                Delete_Click();
            }
            else
            {
                MultiselectDeleteClic(_cdgvData.SelectedRows);
            }
        }
        
        protected override void DeleteObj(CisNGGroup obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.CisNGGroups.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.CisNGGroups.DeleteById(idObj, out error))
                throw error;
        }

        protected override ACgpEditForm<CisNGGroup> CreateEditForm(CisNGGroup obj, ShowOptionsEditForm showOption)
        {
            return new CisNGGroupEditForm(obj, showOption);
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
            if (!string.IsNullOrEmpty(_eGroupNameFilter.Text))
            {
                FilterSettings filterSettingName = new FilterSettings(CisNGGroup.COLUMNGROUPNAME, _eGroupNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingName);
            }
            if (!string.IsNullOrEmpty(_eDescriptionFilter.Text))
            {
                FilterSettings filterSettingIpAddress = new FilterSettings(CisNGGroup.COLUMNDESCRIPTION, _eDescriptionFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingIpAddress);
            }
        }
        
        protected override void ClearFilterEdits()
        {
            _eGroupNameFilter.Text = string.Empty;
            _eDescriptionFilter.Text = string.Empty;
        }

        private void FilterClearClick(object sender, EventArgs e)
        {
            FilterClear_Click();
        }
        #endregion

        #region RefreshDataGrid
        protected override ICollection<CisNGGroupShort> GetData()
        {
            Exception error;
            ICollection<CisNGGroupShort> list = CgpClient.Singleton.MainServerProvider.CisNGGroups.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(CisNGGroup obj1, CisNGGroup obj2)
        {
            return obj1.Compare(obj2);
        }
        
        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, CisNGGroupShort.COLUMN_SYMBOL, CisNGGroupShort.COLUMNGROUPNAME, CisNGGroupShort.COLUMNDESCRIPTION);
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
                    return CgpClient.Singleton.MainServerProvider.CisNGGroups.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(CisNGGroup cisNgGroup)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CisNGGroups.HasAccessViewForObject(cisNgGroup);

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
                    return CgpClient.Singleton.MainServerProvider.CisNGGroups.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.CisNGGroups.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
