using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class LoginGroupEditForm :
#if DESIGNER
        Form
#else
  ACgpEditForm<LoginGroup>
#endif
    {
        private readonly AccessPanelControl _accessPanelControl;
        private readonly object _lockAfterEditLogin = new object();

        public LoginGroupEditForm(LoginGroup loginGroup, ShowOptionsEditForm showOption)
            : base(loginGroup, showOption)
        {
            InitializeComponent();

            _tbdpExpirationDate.LocalizationHelper = LocalizationHelper;
            SetReferenceEditColors();
            WheelTabContorol = tabControl1;

            _eDescription.MouseWheel += ControlMouseWheel;
            _panelClbAccess.MouseWheel += ControlMouseWheel;

            _accessPanelControl = new AccessPanelControl(CheckLoginAccess, EditTextChanger, _panelClbAccess);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private bool CheckLoginAccess(Access access)
        {
            if (!Insert)
            {
                return Login.HasAccess(_editingObject.AccessControls, access);
            }

            return false;
        }

        protected override void RegisterEvents()
        {
            CUDObjectHandler.Singleton.Register(CudObjectEvent, ObjectType.Login);
        }

        private void CudObjectEvent(ObjectType objectType, object id, bool isInsert)
        {
            switch (objectType)
            {
                case ObjectType.Login:
                    AfterEditLogin(CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(id));
                    break;
            }
        }

        protected override void UnregisterEvents()
        {
            CUDObjectHandler.Singleton.Unregister(CudObjectEvent, ObjectType.Login);
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = LocalizationHelper.GetString("LoginGroupEditFormInsertText");
            }

            _accessPanelControl.Translate();

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            LoginGroupsForm.Singleton.BeforeInsert(this);
        }

        protected override void AfterInsert()
        {
            LoginGroupsForm.Singleton.AfterInsert();
        }

        protected override void BeforeEdit()
        {
            LoginGroupsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterEdit()
        {
            LoginGroupsForm.Singleton.AfterEdit(_editingObject);
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            LoginGroup obj = CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectForEdit(_editingObject.IdLoginGroup, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(_editingObject.IdLoginGroup);
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
            CgpClient.Singleton.MainServerProvider.LoginGroups.RenewObjectForEdit(_editingObject.IdLoginGroup, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            _bApply2.Enabled = false;
            _accessPanelControl.LoadAccessRights(false);
            LoadMembers();
            _bApply.Enabled = false;
        }

        protected override void SetValuesEdit()
        {
            if (_editingObject.LoginGroupName == CgpServerGlobals.DEFAULT_ADMIN_LOGIN_GROUP)
            {
                _cbDisabled.Enabled = false;
                _tbdpExpirationDate.Enabled = false;
            }

            _accessPanelControl.LoadAccessRights(false);

            _eLoginGroupName.Text = _editingObject.LoginGroupName;
            _eLoginGroupName.Enabled = false;

            _cbDisabled.Checked = _editingObject.IsDisabled;
            _eDescription.Text = _editingObject.Description;
            EnableDisableMustChangePassword();

            if (_editingObject.ExpirationDate != null)
            {
                _tbdpExpirationDate.Value = _editingObject.ExpirationDate.Value;
            }
            else
            {
                _tbdpExpirationDate.Value = null;
            }

            LoadMembers();
            SetReferencedBy();

            _bApply.Enabled = false;

            if (CgpClient.Singleton.MainServerProvider.LoginGroups.IsLoginGroupSuperAdmin(_editingObject.LoginGroupName) &&
                !CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs))
            {
                DisabledForm();
            }
        }

        private void EnableDisableMustChangePassword()
        {
            if (_editingObject.Logins != null && _editingObject.Logins.Count > 0)
            {
                _bApply2.Enabled = true;
            }
            else
            {
                _bApply2.Enabled = false;
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.LoginGroups.
                GetReferencedObjects(_editingObject.IdLoginGroup, CgpClient.Singleton.GetListLoadedPlugins());
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = true;
        }

        private void LoadMembers()
        {
            _cdgvMembers.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvMembers.ImageList = ObjectImageList.Singleton.ClientObjectImages;
            _cdgvMembers.DataGrid.MouseDoubleClick += _cdgvMembers_MouseDoubleClick;
            _cdgvMembers.DataGrid.DataError += DataGrid_DataError;

            RefreshMembers(null);
        }

        void DataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void _cdgvMembers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_cdgvMembers.DataGrid.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            _bEdit_Click(sender, e);
        }

        protected override bool CheckValues()
        {
            if (_eLoginGroupName.Text == "")
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eLoginGroupName,
                    GetString("ErrorEntryLoginGroupName"), CgpClient.Singleton.ClientControlNotificationSettings);

                return false;
            }

            return true;
        }

        protected override bool GetValues()
        {
            try
            {
                if (_eLoginGroupName.Text != "")
                    _editingObject.LoginGroupName = _eLoginGroupName.Text;

                _editingObject.Description = _eDescription.Text;
                _editingObject.IsDisabled = _cbDisabled.Checked;

                if (_tbdpExpirationDate.Text == "")
                {
                    _editingObject.ExpirationDate = null;
                }
                else
                {
                    _editingObject.ExpirationDate = DateTime.Parse(_tbdpExpirationDate.Text);
                }

                var listAccess = CgpClient.Singleton.MainServerProvider.LoginGroups.IsLoginGroupSuperAdmin(_editingObject.LoginGroupName)
                    ? new List<Access> {BaseAccess.GetAccess(LoginAccess.SuperAdmin)}
                    : _accessPanelControl.GetListOfAccesses();

                _editingObject.SetAccessControls(listAccess);

                return true;
            }
            catch
            {
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        private void SetMembersExpirationDate()
        {
            if (_editingObject.ExpirationDate == null)
                return;

            if (_editingObject.Logins != null && _editingObject.Logins.Count > 0)
            {
                var loginsForEdit = new List<Login>();
                loginsForEdit.AddRange(_editingObject.Logins);

                foreach (var login in loginsForEdit)
                {
                    if (login.ExpirationDate == null || login.ExpirationDate > _editingObject.ExpirationDate)
                    {
                        Exception error;
                        Login loginForEdit =
                            CgpClient.Singleton.MainServerProvider.Logins.GetObjectForEdit(login.IdLogin,
                                out error);

                        if (loginForEdit != null)
                        {
                            loginForEdit.ExpirationDate = _editingObject.ExpirationDate;
                            CgpClient.Singleton.MainServerProvider.Logins.Update(loginForEdit, out error);
                            CgpClient.Singleton.MainServerProvider.Logins.EditEnd(loginForEdit);
                        }
                    }
                }
            }
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.LoginGroups.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is BaseLib.SqlUniqueException)
                {
                    Dialog.Error(GetString("LoginGroupEditFormErrorUniqueColumn"));
                }
                else
                    throw error;
            }

            if (retValue)
            {
                SetMembersExpirationDate();
            }

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.LoginGroups.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                throw error;
            }

            if (retValue)
            {
                SetMembersExpirationDate();

                if (_editingObject.Logins.Any(login => login.Username == CgpClient.Singleton.LoggedAs))
                {
                    CgpClientMainForm.Singleton.RefreshLeftMenuItems();
                }
            }

            return retValue;
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false) && CgpClient.Singleton.MainServerProvider.LoginGroups != null)
                CgpClient.Singleton.MainServerProvider.LoginGroups.EditEnd(_editingObject);
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                _bApply.Enabled = false;
                RefreshMembers(null);
            }
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void _tbdpExpirationDate_ButtonClearDateClick()
        {
            if (Dialog.Question(GetString("LoginGroupEditFormDeleteExpirationDateConfirm")))
            {
                _tbdpExpirationDate.Value = null;
            }
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                Exception error;
                IList<IModifyObject> listModObjFromDatabase =
                    CgpClient.Singleton.MainServerProvider.Logins.ListModifyObjectsFromLoginGroupReferencesSites(
                        _editingObject.IdLoginGroup,
                        _editingObject.Logins,
                        out error);

                if (error != null) throw error;

                var listModObj = new List<IModifyObject>();
                foreach (var modifyObject in listModObjFromDatabase)
                {
                    if (modifyObject.ToString() != CgpServerGlobals.DEFAULT_ADMIN_LOGIN &&
                        (CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs) ||
                         !CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(modifyObject.ToString())))
                    {
                        listModObj.Add(modifyObject);
                    }
                }

                var formAdd = new ListboxFormAdd(listModObj, GetString("LoginsFormLoginsForm"));

                ListOfObjects outObjects;
                formAdd.ShowDialogMultiSelect(out outObjects);

                if (outObjects != null)
                {
                    if (_editingObject.Logins == null)
                    {
                        _editingObject.Logins = new List<Login>();
                    }

                    Login lastSelectedLogin = null;

                    foreach (object selectedObject in outObjects)
                    {
                        var modObj = selectedObject as IModifyObject;
                        if (modObj != null)
                        {
                            Login login = CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(modObj.GetId);

                            if (login == null)
                                continue;

                            if (login.LoginGroup != null && !_editingObject.Compare(login.LoginGroup))
                            {
                                if (!Dialog.Question(string.Format(GetString("QuestionAddLoginConfirm"), login, login.LoginGroup)))
                                    continue;
                            }

                            lastSelectedLogin = login;
                            _editingObject.Logins.Add(login);
                        }
                    }


                    RefreshMembers(lastSelectedLogin);
                    EditTextChanger(null, null);
                    EnableDisableMustChangePassword();
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(ex);
            }
        }

        private void RefreshMembers(Login login)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Login>(RefreshMembers), login);
            }
            else
            {
                if (_editingObject.Logins == null)
                {
                    _editingObject.Logins = new List<Login>();
                }

                var logins = string.IsNullOrEmpty(_eUsername.Text)
                    ? _editingObject.Logins.OrderBy(actLogin => actLogin.Username).ToList()
                    : _editingObject.Logins.Where(actLogin => actLogin.Username.ToLower().IndexOf(_eUsername.Text.ToLower()) == 0)
                        .OrderBy(actLogin => actLogin.Username).ToList();

                if (logins.Count == 0)
                {
                    _cdgvMembers.RemoveDataSource();
                }
                else
                {
                    _cdgvMembers.ModifyGridView(
                        new BindingSource {DataSource = logins},
                        Login.COLUMN_USERNAME,
                        Login.COLUMN_IS_DISABLED,
                        Login.COLUMN_EXPIRATION_DATE,
                        Login.COLUMN_MUST_CHANGE_PASSWORD,
                        Login.COLUMN_CLIENT_LANGUAGE,
                        Login.COLUMN_DESCRIPTION);

                    if (login != null && _cdgvMembers.DataGrid != null)
                    {
                        foreach (DataGridViewRow row in _cdgvMembers.DataGrid.Rows)
                        {
                            if (login.Compare(row.DataBoundItem))
                            {
                                _cdgvMembers.DataGrid.CurrentCell = row.Cells[1];
                                row.Selected = true;
                            }
                        }
                    }

                    FillColumnLoginIsDisabled();
                }
            }
        }

        private void FillColumnLoginIsDisabled()
        {
            if (_cdgvMembers.DataGrid != null)
            {
                foreach (DataGridViewRow row in _cdgvMembers.DataGrid.Rows)
                {
                    row.Cells[Login.COLUMN_IS_DISABLED].Value =
                        _editingObject.IsDisabled
                        || (bool) row.Cells[Login.COLUMN_IS_DISABLED].Value;
                }
            }
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            var login = new Login();
            login.ExpirationDate = _editingObject.ExpirationDate;
            login.LoginGroup = _editingObject;

            if (!LoginsForm.Singleton.OpenInsertDialg(ref login))
                return;

            if (_editingObject.Logins == null)
            {
                _editingObject.Logins = new List<Login>();
            }

            _editingObject.Logins.Add(login);
            RefreshMembers(login);
            EditTextChanger(null, null);
            EnableDisableMustChangePassword();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_cdgvMembers.DataGrid != null && _cdgvMembers.DataGrid.CurrentRow != null)
            {
                Login login = _cdgvMembers.DataGrid.CurrentRow.DataBoundItem as Login;
                if (login == null)
                    return;

                if (CgpClient.Singleton.IsConnectionLost(true))
                {
                    return;
                }

                if (login.Username == CgpServerGlobals.DEFAULT_ADMIN_LOGIN)
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteSuperAdmin"));
                    return;
                }

                _editingObject.Logins.Remove(login);
                RefreshMembers(null);
                EditTextChanger(null, null);
                EnableDisableMustChangePassword();
            }
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_cdgvMembers.DataGrid != null && _cdgvMembers.DataGrid.CurrentRow != null)
            {
                LoginsForm.Singleton.OpenEditForm(_cdgvMembers.DataGrid.CurrentRow.DataBoundItem as Login);
            }
        }

        private void AfterEditLogin(object obj)
        {
            lock (_lockAfterEditLogin)
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;

                try
                {
                    var login = obj as Login;
                    if (login == null || _editingObject.Logins == null)
                        return;

                    Login selectedLogin = null;
                    if (_cdgvMembers.DataGrid != null && _cdgvMembers.DataGrid.CurrentRow != null)
                    {
                        selectedLogin = _cdgvMembers.DataGrid.CurrentRow.DataBoundItem as Login;
                    }

                    Login changedLogin = _editingObject.Logins.FirstOrDefault(login.Compare);

                    if (changedLogin != null)
                    {
                        _editingObject.Logins.Remove(changedLogin);
                        _editingObject.Logins.Add(login);
                    }

                    RefreshMembers(selectedLogin);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        private void _cdgvMembers_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _cdgvMembers_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddLogin(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddLogin(object newLogin)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                if (newLogin is Login)
                {
                    var login = newLogin as Login;

                    if (login.Username == CgpServerGlobals.DEFAULT_ADMIN_LOGIN ||
                        (CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(login.Username) &&
                        !CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs)))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvMembers,
                            GetString("ErrorAddLoginAccessDenied"),
                            ControlNotificationSettings.Default);

                        return;
                    }

                    if (!CgpClient.Singleton.MainServerProvider.Logins.IsLoginInLoginGroupReferencesSites(
                        _editingObject.IdLoginGroup,
                        login.IdLogin))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvMembers,
                            GetString("LoginMustBePlacedInSameSiteAsLoginGroup"),
                            ControlNotificationSettings.Default);

                        return;
                    }

                    if (_editingObject.Logins == null)
                    {
                        _editingObject.Logins = new List<Login>();
                    }
                    else
                    {
                        foreach (var actLogin in _editingObject.Logins)
                        {
                            if (actLogin.Compare(login))
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvMembers,
                                    GetString("ErrorLoginAlreadyAdded"),
                                    ControlNotificationSettings.Default);
                                return;
                            }
                        }
                    }

                    if (login.LoginGroup != null && !_editingObject.Compare(login.LoginGroup))
                    {
                        if (!Dialog.Question(string.Format(GetString("QuestionAddLoginConfirm"), login, login.LoginGroup)))
                            return;
                    }

                    _editingObject.Logins.Add(login);
                    RefreshMembers(login);

                    if (_cdgvMembers.DataGrid != null)
                    {
                        foreach (DataGridViewRow row in _cdgvMembers.DataGrid.Rows)
                        {
                            if (login.Compare(row.DataBoundItem))
                            {
                                _cdgvMembers.DataGrid.FirstDisplayedScrollingRowIndex = row.Index;
                                _cdgvMembers.DataGrid.CurrentCell = row.Cells[0];
                                row.Selected = true;
                            }
                        }
                    }

                    EditTextChanger(null, null);
                    EnableDisableMustChangePassword();
                    CgpClientMainForm.Singleton.AddToRecentList(login);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvMembers,
                        GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RefreshMembers(null);
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _eUsername.Text = string.Empty;
            RefreshMembers(null);
        }

        private void _bChangedPasswordAtNextLogin_Click(object sender, EventArgs e)
        {
            SafeThread.StartThread(ApplyMustChangePassword);
        }

        private void ApplyMustChangePassword()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (_editingObject.Logins != null && _editingObject.Logins.Count > 0)
            {
                var loginsForEdit = new List<Login>();
                loginsForEdit.AddRange(_editingObject.Logins);

                foreach (var login in loginsForEdit)
                {
                    if (!login.MustChangePassword)
                    {
                        Exception error;
                        Login loginForEdit =
                            CgpClient.Singleton.MainServerProvider.Logins.GetObjectForEdit(
                                login.IdLogin,
                                out error);

                        if (loginForEdit != null)
                        {
                            loginForEdit.MustChangePassword = true;
                            CgpClient.Singleton.MainServerProvider.Logins.Update(loginForEdit, out error);
                            CgpClient.Singleton.MainServerProvider.Logins.EditEnd(loginForEdit);
                        }
                    }
                }
            }
        }
    }
}
