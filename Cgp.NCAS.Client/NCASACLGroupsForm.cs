using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASACLGroupsForm :
#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, ACLGroup, ACLGroupShort>
#endif
    {
        private static volatile NCASACLGroupsForm _singleton;
        private readonly static object _syncRoot = new object();
        private UserFoldersStructure _actFilterDepartment;

        public static NCASACLGroupsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASACLGroupsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        public NCASACLGroupsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.ACLGroup48;
            InitializeComponent();
            InitDataGridView();

            FormOnEnter += SetFilterDepartmentTextBoxColors;
        }

        private void SetFilterDepartmentTextBoxColors(Form form)
        {
            _tbmDepartment.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
            _tbmDepartment.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
        }

        #region DataGridView

        private void InitDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.AllwaysRefreshOrder = true;
            _cdgvData.BeforeGridModified += DataGrid_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        void DataGrid_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (ACLGroupShort aclGroupShort in bindingSource.List)
            {
                aclGroupShort.Symbol = _cdgvData.GetDefaultImage(aclGroupShort);
            }
        }

        protected override ICollection<ACLGroupShort> GetData()
        {
            Exception error;
            ICollection<ACLGroupShort> list = Plugin.MainServerProvider.AclGroups.ShortSelectByCriteria(FilterSettings, out error);
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

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.AclGroups.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(ACLGroup aclGroup)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.AclGroups.HasAccessViewForObject(aclGroup);
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
                    return Plugin.MainServerProvider.AclGroups.HasAccessInsert();
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
                    return Plugin.MainServerProvider.AclGroups.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, ACLGroupShort.COLUMN_SYBMOL,
                ACLGroupShort.COLUMN_NAME, ACLGroupShort.COLUMN_DESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        #endregion

        #region FilterSettings

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != "")
            {
                FilterSettings.Add(
                    new FilterSettings(
                        ACLGroupShort.COLUMN_NAME,
                        _eNameFilter.Text,
                        ComparerModes.LIKEBOTH));
            }

            if (_actFilterDepartment != null)
            {
                FilterSettings.Add(
                    new FilterSettings(
                        ACLGroup.COLUMN_DEPARTMENTS,
                        _actFilterDepartment,
                        ComparerModes.EQUALL));
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
            SetFilterDepartment(null);
        }

        private void _eNameFilter_TextChanged(object sender, EventArgs e)
        {
            FilterValueChanged(sender, e);
        }

        private void _eNameFilter_KeyDown(object sender, KeyEventArgs e)
        {
            FilterKeyDown(sender, e);
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        #endregion

        #region InsertUpdateDelete

        protected override ACgpPluginEditForm<NCASClient, ACLGroup>  CreateEditForm(ACLGroup obj, ShowOptionsEditForm showOption)
        {
            return new NCASACLGruopEditForm(obj, showOption, this);
        }

        protected override ACLGroup GetObjectForEdit(ACLGroupShort listObj, out bool editEnabled)
        {
            return Plugin.MainServerProvider.AclGroups.GetObjectForEditById(listObj.IdACLGroup, out editEnabled);
        }

        protected override ACLGroup GetFromShort(ACLGroupShort listObj)
        {
            return Plugin.MainServerProvider.AclGroups.GetObjectById(listObj.IdACLGroup);
        }

        protected override bool Compare(ACLGroup obj1, ACLGroup obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(ACLGroup obj, Guid idObj)
        {
            return obj.IdACLGroup == idObj;
        }

        protected override void DeleteObj(ACLGroup obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.AclGroups.Delete(obj, out error))
                throw error;
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
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

        #endregion

        private void _tbmDepartment_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiRemove)
            {
                SetFilterDepartment(null);
            }

            if (item == _tsiModify)
            {
                try
                {
                    Exception error = null;
                    var departmentsList = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.ListDepartments(
                        CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode"),
                        @"\",
                        null,
                        out error);

                    if (error != null)
                        throw error;

                    ListboxFormAdd formAdd = new ListboxFormAdd(
                        departmentsList,
                        CgpClient.Singleton.LocalizationHelper.GetString(
                            "UserFoldersStructuresFormUserFoldersStructuresForm"));

                    object outDepartment;
                    formAdd.ShowDialog(out outDepartment);
                    SetFilterDepartment(outDepartment as UserFoldersStructure);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        private void SetFilterDepartment(UserFoldersStructure department)
        {
            _actFilterDepartment = department;

            if (_actFilterDepartment != null)
            {
                _tbmDepartment.Text = _actFilterDepartment.ToString();
                _tbmDepartment.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actFilterDepartment);
            }
            else
            {
                _tbmDepartment.Text = string.Empty;
            }

            FilterValueChanged(null, null);
        }

        private void _tbmDepartment_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmDepartment_DragDrop(object sender, DragEventArgs e)
        {
            string[] output = e.Data.GetFormats();
            if (output == null) return;
            var department = e.Data.GetData(output[0]) as UserFoldersStructure;

            if (department == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                    _tbmDepartment.ImageTextBox,
                    GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);

                return;
            }

            SetFilterDepartment(department);
        }
    }
}
