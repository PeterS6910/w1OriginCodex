using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAccessControlListsForm :
#if DESIGNER
        Form
#else
 ACgpPluginTableForm<NCASClient, AccessControlList, AccessControlListShort>
#endif
    {
        public NCASAccessControlListsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.ACL48;
            InitializeComponent();
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.AllwaysRefreshOrder = true;
            _cdgvData.BeforeGridModified += DataGrid_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        void DataGrid_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (AccessControlListShort aclShort in bindingSource.List)
            {
                aclShort.Symbol = _cdgvData.GetDefaultImage(aclShort);
            }
        }

        protected override AccessControlList GetObjectForEdit(AccessControlListShort listObj, out bool editAllowed)
        {
            return Plugin.MainServerProvider.AccessControlLists.GetObjectForEditById(listObj.IdAccessControlList, out editAllowed);
        }

        protected override AccessControlList GetFromShort(AccessControlListShort listObj)
        {
            return Plugin.MainServerProvider.AccessControlLists.GetObjectById(listObj.IdAccessControlList);
        }

        private static volatile NCASAccessControlListsForm _singleton;
        private static object _syncRoot = new object();

        public static NCASAccessControlListsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASAccessControlListsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        protected override ICollection<AccessControlListShort> GetData()
        {
            Exception error;
            ICollection<AccessControlListShort> list = Plugin.MainServerProvider.AccessControlLists.ShortSelectByCriteria(FilterSettings, out error);
            if (error != null)
                throw (error);
            CheckAccess();
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

        protected override bool Compare(AccessControlList obj1, AccessControlList obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(AccessControlList obj, Guid idObj)
        {
            return obj.IdAccessControlList == idObj;
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, AccessControlListShort.COLUMN_SYBMOL,
                AccessControlListShort.COLUMN_NAME, AccessControlListShort.COLUMN_DESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        protected override ACgpPluginEditForm<NCASClient, AccessControlList> CreateEditForm(AccessControlList obj, ShowOptionsEditForm showOption)
        {
            return new NCASAccessControlListEditForm(obj, showOption, this);
        }

        private void _bEdit_Click(object sender, EventArgs e)
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

        private void _bDelete_Click(object sender, EventArgs e)
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

        protected override void DeleteObj(AccessControlList obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.AccessControlLists.Delete(obj, out error))
                throw error;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != "")
            {
                FilterSettings filterSetting = new FilterSettings(AccessControlListShort.COLUMN_NAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
        }

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
                    return Plugin.MainServerProvider.AccessControlLists.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(AccessControlList accessControlList)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return
                        Plugin.MainServerProvider.AccessControlLists.HasAccessViewForObject(
                            accessControlList);
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
                    return Plugin.MainServerProvider.AccessControlLists.HasAccessInsert();
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
                    return Plugin.MainServerProvider.AccessControlLists.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        private void bExportExcel_Click(object sender, EventArgs e)
        {
            var exportForm = new ExcelExportForm(false, FilterSettings);
            exportForm.Show();

        }
    }
}
