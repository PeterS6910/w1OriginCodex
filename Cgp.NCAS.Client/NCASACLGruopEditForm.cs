using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.Cgp.Globals;

using SqlUniqueException = Contal.IwQuick.SqlUniqueException;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASACLGruopEditForm :
#if DESIGNER
        Form
#else
        ACgpPluginEditForm<NCASClient, ACLGroup>
#endif
    {
        private bool _allowEdit = true;

        public NCASACLGruopEditForm(
                ACLGroup aclGroup,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                aclGroup,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            WheelTabContorol = _tcSettings;

            MinimumSize = new Size(Width, Height);

            _eDescription.MouseWheel += ControlMouseWheel; 
            
            SetReferenceEditColors();

            _lbUserFolders.MouseWheel += ControlMouseWheel;

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            _ilbAccessControlLists.ImageList = ObjectImageList.Singleton.ClientObjectImages;
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageImplicitAssigningConfiguration(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsImplicitAssigningConfigurationView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsImplicitAssigningConfigurationAdmin)));

                HideDisableTabPageAccessControlLists(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsAclsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsAclsAdmin)));

                HideDisableTabPageDepartments(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsDepartmentsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsDepartmentsAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclGroupsDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageImplicitAssigningConfiguration(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageImplicitAssigningConfiguration),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcSettings.TabPages.Remove(_tpImplicitAssigningConfiguration);
                    return;
                }

                _tpImplicitAssigningConfiguration.Enabled = admin;
            }
        }

        private void HideDisableTabPageAccessControlLists(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAccessControlLists),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcSettings.TabPages.Remove(_tpAccessControlLists);
                    return;
                }

                _tpAccessControlLists.Enabled = admin;
            }
        }

        private void HideDisableTabPageDepartments(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDepartments),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcSettings.TabPages.Remove(_tpDepartments);
                    return;
                }

                _tpDepartments.Enabled = admin;
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
                    _tcSettings.TabPages.Remove(_tpUserFolders);
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
                    _tcSettings.TabPages.Remove(_tpReferencedBy);
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
                    _tcSettings.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        #region BeforeInsertEdit

        protected override void BeforeInsert()
        {
            NCASACLGroupsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASACLGroupsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void SetValuesInsert()
        {
            _bApply.Enabled = false;
        }

        protected override void SetValuesEdit()
        {
            _eName.Text = _editingObject.Name;
            _eDescription.Text = _editingObject.Description;
            LoadAccessControlLists();

            _chbIsImplicit.Checked = _editingObject.IsImplicit;
            _cbUseForAllPersons.Checked = _editingObject.UseForAllPerson;
            _cbRemoveAllAcls.Checked = _editingObject.RemoveAllAcls;

            _cbUseDepartmentAclGroupRelation.Checked = _editingObject.UseDepartmentAclGroupRelation;
            _cbRemoveAclsDepartmentChange.Checked = _editingObject.RemoveWhenDepartmentChange;
            _cbRemoveAclsDepartmentRemove.Checked = _editingObject.RemoveWhenDepartmentRemove;

            _chbApplyEndValidity.Checked = !_editingObject.ApplyEndlessValidity;
            _nudYears.Value = _editingObject.YearsOfValidity;
            _nudMonths.Value = _editingObject.MonthsOfValidity;
            _nudDays.Value = _editingObject.DaysOfValidity;

            LoadDepartments();

            _bApply.Enabled = false;
            SetReferencedBy();
        }

        private void LoadAccessControlLists()
        {
            if (_editingObject.AccessControlLists != null && _editingObject.AccessControlLists.Count > 0)
            {
                _ilbAccessControlLists.Items.AddRange(_editingObject.AccessControlLists.ToArray());
            }
        }

        private void LoadDepartments()
        {
            if (_editingObject.Departments != null && _editingObject.Departments.Count > 0)
            {
                foreach (var department in _editingObject.Departments)
                {
                    department.SetFullFolderName(
                        CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetFullDepartmentName(
                            department.GetIdString(),
                            department.FolderName,
                            @"\",
                            CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode")));
                }

                _ilbDepartments.Items.AddRange(_editingObject.Departments.ToArray());
            }
        }

        protected override void RegisterEvents()
        {
        }

        #endregion

        #region AfterInsertEdit

        protected override void AfterInsert()
        {
            NCASACLGroupsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASACLGroupsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void UnregisterEvents()
        {
        }

        #endregion

        #region RaedFromDatabase

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            ACLGroup obj = Plugin.MainServerProvider.AclGroups.GetObjectForEdit(_editingObject.IdACLGroup, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.AclGroups.GetObjectById(_editingObject.IdACLGroup);
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

        protected override void DisableForm()
        {
            base.DisableForm();

            _allowEdit = false;
            _bCancel.Enabled = true;
            _ilbAccessControlLists.Enabled = true;
            _ilbDepartments.Enabled = true;
            _bEdit.Enabled = true;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.AclGroups.RenewObjectForEdit(_editingObject.IdACLGroup, out error);

            if (error != null) 
                throw error;
        }

        #endregion

        #region SaveToDatabase

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne, 
                    _eName,
                    GetString("ErrorEntryACLGroupName"), 
                    CgpClient.Singleton.ClientControlNotificationSettings);

                _eName.Focus();
                return false;
            }

            return true;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;
                _editingObject.Description = _eDescription.Text;

                _editingObject.IsImplicit = _chbIsImplicit.Checked;
                _editingObject.UseForAllPerson = _cbUseForAllPersons.Checked;
                _editingObject.RemoveAllAcls = _cbRemoveAllAcls.Checked;

                _editingObject.UseDepartmentAclGroupRelation = _cbUseDepartmentAclGroupRelation.Checked;
                _editingObject.RemoveWhenDepartmentChange = _cbRemoveAclsDepartmentChange.Checked;
                _editingObject.RemoveWhenDepartmentRemove = _cbRemoveAclsDepartmentRemove.Checked;

                _editingObject.ApplyEndlessValidity = !_chbApplyEndValidity.Checked;
                _editingObject.YearsOfValidity = (byte) _nudYears.Value;
                _editingObject.MonthsOfValidity = (byte) _nudMonths.Value;
                _editingObject.DaysOfValidity = (byte) _nudDays.Value;

                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = Plugin.MainServerProvider.AclGroups.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, 
                        _eName,
                        GetString("ErrorUsedACLGroupName"), 
                        CgpClient.Singleton.ClientControlNotificationSettings);

                    _eName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = Plugin.MainServerProvider.AclGroups.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, _eName,
                        GetString("ErrorUsedACLGroupName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.AccessControlLists != null)
                Plugin.MainServerProvider.AclGroups.EditEnd(_editingObject);
        }

        #endregion

        #region FormActions

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                _bApply.Enabled = false;
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        #endregion

        #region AddDeleteEditAccessControlLists

        private void _bAdd_Click(object sender, EventArgs e)
        {
            ConnectionLost();
            IList<IModifyObject> accessControlLists = new List<IModifyObject>();

            List<FilterSettings> listFilterSettings = new List<FilterSettings>();
            if (_editingObject.AccessControlLists != null && _editingObject.AccessControlLists.Count > 0)
            {
                foreach (AccessControlList accessControlList in _editingObject.AccessControlLists)
                {
                    listFilterSettings.Add(new FilterSettings(AccessControlList.COLUMNIDACCESSCONTROLLIST,
                        accessControlList.IdAccessControlList, ComparerModes.NOTEQUALL));
                }
            }

            Exception error = null;
            accessControlLists =
                Plugin.MainServerProvider.AccessControlLists.ListModifyObjectsSelectByCriteria(
                    listFilterSettings, out error);

            if (error != null)
                return;

            ListboxFormAdd formAdd = new ListboxFormAdd(accessControlLists,
                GetString("NCASAccessControlListsFormNCASAccessControlListsForm"), false);

            ListOfObjects outAccessControlLists;
            formAdd.ShowDialogMultiSelect(out outAccessControlLists);

            if (outAccessControlLists != null)
            {
                foreach (object obj in outAccessControlLists)
                {
                    IModifyObject modifyObject = obj as IModifyObject;

                    AccessControlList accessControlList = modifyObject == null
                        ? null
                        : Plugin.MainServerProvider.AccessControlLists.GetObjectById(modifyObject.GetId);
                    
                    if (accessControlList != null)
                    {
                        AddAccessControlList(accessControlList);
                    }
                }
            }
        }

        private void AddAccessControlList(AccessControlList newAccessControlList)
        {
            if (_editingObject.AccessControlLists == null)
            {
                _editingObject.AccessControlLists = new List<AccessControlList>();
            }
            else
            {
                foreach (AccessControlList accessControlList in _editingObject.AccessControlLists)
                {
                    if (accessControlList.Compare(newAccessControlList))
                    {
                        Dialog.Error(GetString("ACLGroupEditFormAccessControlListAlreadyInList"));
                        return;
                    }
                }
            }

            _editingObject.AccessControlLists.Add(newAccessControlList);
            _ilbAccessControlLists.Items.Add(newAccessControlList);
            NCASClient.Singleton.AddToRecentList(newAccessControlList);
            EditTextChanger(null, null);
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_editingObject.AccessControlLists == null ||
                _editingObject.AccessControlLists.Count == 0)
                return;

            ListBox.SelectedObjectCollection seletedObjects = _ilbAccessControlLists.SelectedItems;
            if (seletedObjects == null || seletedObjects.Count == 0)
                return;

            object[] selectedObjectsArray = new object[seletedObjects.Count];
            seletedObjects.CopyTo(selectedObjectsArray, 0);

            foreach (object seletedObject in selectedObjectsArray)
            {
                _editingObject.AccessControlLists.Remove(seletedObject as AccessControlList);
                _ilbAccessControlLists.Items.Remove(seletedObject);
            }

            EditTextChanger(null, null);
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_editingObject.AccessControlLists == null ||
                _editingObject.AccessControlLists.Count == 0)
                return;

            ListBox.SelectedObjectCollection seletedObjects = _ilbAccessControlLists.SelectedItems;
            if (seletedObjects == null || seletedObjects.Count == 0)
                return;

            foreach (object seletedObject in seletedObjects)
            {
                AccessControlList accessControlList = seletedObject as AccessControlList;
                if (accessControlList != null)
                {
                    NCASAccessControlListsForm.Singleton.OpenEditForm(accessControlList);
                }
            }
        }

        private void _ilbAccessControlLists_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _bEdit_Click(null, null);
        }

        private void _ilbAccessControlLists_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _ilbAccessControlLists_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (!_allowEdit)
                    return;

                string[] output = e.Data.GetFormats();
                if (output == null) return;
                
                AccessControlList accessControlList = e.Data.GetData(output[0]) as AccessControlList;
                if (accessControlList == null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ilbAccessControlLists,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                    return;
                }

                AddAccessControlList(accessControlList);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        #endregion

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString("ACLGroupEditFormInsertText");
            }
            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        private void _bAdd1_Click(object sender, EventArgs e)
        {
            ConnectionLost();

            try
            {
                HashSet<object> addedDepartments = new HashSet<object>();
                if (_editingObject.Departments != null)
                {
                    foreach (UserFoldersStructure userFolderStructure in _editingObject.Departments)
                    {
                        addedDepartments.Add(userFolderStructure.IdUserFoldersStructure);
                    }
                }

                Exception error;
                var departmentsListFromDatabase = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.ListDepartments(
                    CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode"),
                    @"\",
                    null,
                    out error);

                if (error != null)
                    throw error;

                if (departmentsListFromDatabase == null)
                {
                    return;
                }

                var departmentsList = new LinkedList<AOrmObject>();
                foreach (var department in departmentsListFromDatabase)
                {
                    if (!addedDepartments.Contains(department.GetId()))
                        departmentsList.AddLast(department);
                }

                ListboxFormAdd formAdd = new ListboxFormAdd(
                    departmentsList,
                    CgpClient.Singleton.LocalizationHelper.GetString(
                        "UserFoldersStructuresFormUserFoldersStructuresForm"));

                ListOfObjects outDepartments;
                formAdd.ShowDialogMultiSelect(out outDepartments);

                if (outDepartments != null)
                {
                    foreach (object obj in outDepartments)
                    {
                        AddDepartment(obj as UserFoldersStructure);
                    }
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void AddDepartment(UserFoldersStructure department)
        {
            if (department == null)
                return;

            if (_editingObject.Departments == null)
            {
                _editingObject.Departments = new List<UserFoldersStructure>();
            }
            else
            {
                foreach (UserFoldersStructure userFolderStructure in _editingObject.Departments)
                {
                    if (userFolderStructure.Compare(department))
                    {
                        Dialog.Error(GetString("ErrorACLGroupEditFormDepartmentAlreadyInList"));
                        return;
                    }
                }
            }

            _editingObject.Departments.Add(department);
            _ilbDepartments.Items.Add(department);
            CgpClientMainForm.Singleton.AddToRecentList(department);
            EditTextChanger(null, null);
        }

        private void _bDelete1_Click(object sender, EventArgs e)
        {
            if (_editingObject.Departments == null ||
                _editingObject.Departments.Count == 0)
                return;

            var seletedObjects = _ilbDepartments.SelectedItems;
            if (seletedObjects == null || seletedObjects.Count == 0)
                return;

            object[] selectedObjectsArray = new object[seletedObjects.Count];
            seletedObjects.CopyTo(selectedObjectsArray, 0);

            foreach (object seletedObject in selectedObjectsArray)
            {
                _editingObject.Departments.Remove(seletedObject as UserFoldersStructure);
                _ilbDepartments.Items.Remove(seletedObject);
            }

            EditTextChanger(null, null);
        }

        private void _bEdit1_Click(object sender, EventArgs e)
        {
            if (_editingObject.Departments == null ||
                _editingObject.Departments.Count == 0)
                return;

            ListBox.SelectedObjectCollection seletedObjects = _ilbDepartments.SelectedItems;
            if (seletedObjects == null || seletedObjects.Count == 0)
                return;

            foreach (object seletedObject in seletedObjects)
            {
                UserFoldersStructure department = seletedObject as UserFoldersStructure;
                if (department != null)
                {
                    DbsSupport.OpenEditForm(department);
                }
            }
        }

        private void _ilbDepartments_DoubleClick(object sender, EventArgs e)
        {
            _bEdit1_Click(null, null);
        }

        private void _ilbDepartments_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _ilbDepartments_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (!_allowEdit)
                    return;

                string[] output = e.Data.GetFormats();
                if (output == null) return;

                UserFoldersStructure department = e.Data.GetData(output[0]) as UserFoldersStructure;
                if (department == null)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _ilbDepartments,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                AddDepartment(department);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            if (!_allowEdit)
                return;

            if (sender == _cbUseDepartmentAclGroupRelation)
            {
                _cbRemoveAclsDepartmentChange.Enabled = _cbUseDepartmentAclGroupRelation.Checked;
                _cbRemoveAclsDepartmentRemove.Enabled = _cbUseDepartmentAclGroupRelation.Checked;
            }

            var enabledValidityTimeOfAssigning = _chbIsImplicit.Checked
                                                 || _cbUseForAllPersons.Checked
                                                 || _cbUseDepartmentAclGroupRelation.Checked;

            _chbApplyEndValidity.Enabled = enabledValidityTimeOfAssigning;

            var enabledYearsMonthsDays = enabledValidityTimeOfAssigning && _chbApplyEndValidity.Checked;

            _nudYears.Enabled = enabledYearsMonthsDays;
            _nudMonths.Enabled = enabledYearsMonthsDays;
            _nudDays.Enabled = enabledYearsMonthsDays;

            EditTextChanger(null, null);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, NCASClient.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return Plugin.MainServerProvider.AclGroups.
                GetReferencedObjects(_editingObject.IdACLGroup, CgpClient.Singleton.GetListLoadedPlugins());

        }
    }
}
