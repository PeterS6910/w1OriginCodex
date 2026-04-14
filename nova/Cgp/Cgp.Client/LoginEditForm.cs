using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.IwQuick.UI;


namespace Contal.Cgp.Client
{
    public partial class LoginEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<Login>
#endif
    {
        private const string EDITPASSWORD = "******";

        private Person _actPerson;
        private LoginGroup _actLoginGroup;
        private readonly AccessPanelControl _accessPanelControl;
        private Login _clonedLogin = null;

        public LoginEditForm(Login login, ShowOptionsEditForm showOption)
            : base(login, showOption)
        {
            InitializeComponent();
            _tbdpExpirationDate.LocalizationHelper = LocalizationHelper;
            _editingObject = login;
            SetReferenceEditColors();
            WheelTabContorol = tabControl1;
            CheckAccessControl();

            _eDescription.MouseWheel += ControlMouseWheel;
            _panelClbAccess.MouseWheel += ControlMouseWheel;
            _accessPanelControl = new AccessPanelControl(CheckLoginAccess, EditTextChanger, _panelClbAccess);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private bool CheckLoginAccess(Access access)
        {
            if (_actLoginGroup != null)
                return Login.HasAccess(_actLoginGroup.AccessControls, access);
                    

            if (!Insert)
            {
                return Login.HasAccess(_editingObject.AccessControls, access);
            }

            if (_clonedLogin != null)
            {
                return Login.HasAccess(_clonedLogin.AccessControls, access);
            }

            return false;
        }

        protected override void RegisterEvents()
        {
            CUDObjectHandler.Singleton.Register(CudObjectEvent, ObjectType.LoginGroup);
        }

        private void CudObjectEvent(ObjectType objectType, object id, bool isInsert)
        {
            switch (objectType)
            {
                case ObjectType.LoginGroup:
                    AfterEditLoginGroup((Guid) id);
                    break;
            }
        }

        private void AfterEditLoginGroup(Guid idLoginGroup)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid>(AfterEditLoginGroup), idLoginGroup);
                return;
            }

            if (_actLoginGroup == null)
                return;

            if (_actLoginGroup.IdLoginGroup != idLoginGroup)
                return;

            _actLoginGroup =
                CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(
                    idLoginGroup);

            if (_actLoginGroup != null)
            {
                _tbmLoginGroup.Text = _actLoginGroup.ToString();
                _lLoginGroupIsDisabled.Visible = _actLoginGroup.IsDisabled;

                return;
            }

            _tbmLoginGroup.Text = string.Empty;
            _lLoginGroupIsDisabled.Visible = false;
        }

        protected override void UnregisterEvents()
        {
            CUDObjectHandler.Singleton.Unregister(CudObjectEvent, ObjectType.LoginGroup);
        }

        private void CheckAccessControl()
        {
            if (!PersonsForm.Singleton.HasAccessView())
            {
                _tbmPerson.Enabled = false;
            }
        }

        public override void SetValuesInsertFromObj(object obj)
        {
            _clonedLogin = obj as Login;
            if (_clonedLogin != null)
            {
                _cbDisabled.Checked = _clonedLogin.IsDisabled;
                _cbMustChangePassword.Checked = _clonedLogin.MustChangePassword;

                if (_clonedLogin.ExpirationDate != null)
                    _tbdpExpirationDate.Value = _clonedLogin.ExpirationDate.Value;

                _actLoginGroup = _clonedLogin.LoginGroup;
                if (_actLoginGroup == null)
                {
                    _tbmLoginGroup.Text = string.Empty;
                    _lLoginGroupIsDisabled.Visible = false;
                }
                else
                {
                    _tbmLoginGroup.Text = _actLoginGroup.ToString();
                    _tbmLoginGroup.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actLoginGroup);

                    _lLoginGroupIsDisabled.Visible = _actLoginGroup.IsDisabled;
                }
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = LocalizationHelper.GetString("LoginEditFormInsertText");
            }

            _accessPanelControl.Translate();

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            LoginsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            LoginsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            Login obj = CgpClient.Singleton.MainServerProvider.Logins.GetObjectForEdit(_editingObject.IdLogin, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(_editingObject.IdLogin);
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
            CgpClient.Singleton.MainServerProvider.Logins.RenewObjectForEdit(_editingObject.Username, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesEdit()
        {
            if (CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(_editingObject.Username) &&
                !CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs))
            {
                DisabledForm();
            }

            if (_editingObject.Username == CgpServerGlobals.DEFAULT_ADMIN_LOGIN)
            {
                _cbDisabled.Enabled = false;
                _tbdpExpirationDate.Enabled = false;
                _tbmLoginGroup.Enabled = false;
            }

            _eUserName.Text = _editingObject.Username;
            _eUserName.Enabled = false;
            _ePassword.Text = EDITPASSWORD;
            //_ePublicKey.Text = _editingObject.PublicKey; //single sign-in function will be used later

            _actLoginGroup = _editingObject.LoginGroup;
            if (_actLoginGroup == null)
            {
                _tbmLoginGroup.Text = string.Empty;
                _lLoginGroupIsDisabled.Visible = false;
            }
            else
            {
                _actLoginGroup =
                    CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(_actLoginGroup.IdLoginGroup);

                _tbmLoginGroup.Text = _actLoginGroup.ToString();
                _tbmLoginGroup.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actLoginGroup);

                if (
                    CgpClient.Singleton.MainServerProvider.LoginGroups.IsLoginGroupSuperAdmin(
                        _actLoginGroup.LoginGroupName) &&
                    !CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs))
                {
                    _tbmLoginGroup.Enabled = false;
                }

                _lLoginGroupIsDisabled.Visible = _actLoginGroup.IsDisabled;
            }

            _accessPanelControl.LoadAccessRights(_actLoginGroup != null);

            _cbDisabled.Checked = _editingObject.IsDisabled;
            _cbMustChangePassword.Checked = _editingObject.MustChangePassword;
            _eDescription.Text = _editingObject.Description;

            if (_editingObject.ExpirationDate != null)
            {
                _tbdpExpirationDate.Value = _editingObject.ExpirationDate.Value;
            }
            else
            {
                _tbdpExpirationDate.Value = null;
            }

            _actPerson = _editingObject.Person;
            if (_actPerson == null)
                _tbmPerson.Text = string.Empty;
            else
            {
                _tbmPerson.Text = _actPerson.ToString();
                _tbmPerson.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actPerson);
            }

            SetReferencedBy();
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = true;
        }

        protected override void SetValuesInsert()
        {
            _bClone.Enabled = false;
            
            if (_editingObject.LoginGroup != null)
            {
                _actLoginGroup =
                    CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(
                        _editingObject.LoginGroup.IdLoginGroup);

                _tbmLoginGroup.Text = _actLoginGroup.ToString();
                _tbmLoginGroup.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actLoginGroup);

                _tbmLoginGroup.Enabled = false;

                _lLoginGroupIsDisabled.Visible = _actLoginGroup.IsDisabled;
            }

            if (_editingObject.ExpirationDate != null)
            {
                _tbdpExpirationDate.Value = _editingObject.ExpirationDate;
            }

            _accessPanelControl.LoadAccessRights(_actLoginGroup != null);

            if (_editingObject.Person != null)
            {
                _actPerson = _editingObject.Person;
                _tbmPerson.Text = _actPerson.ToString();
                _tbmPerson.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actPerson);

                _tbmPerson.Enabled = false;
            }
        }

        protected override bool GetValues()
        {
            try
            {
                if (_eUserName.Text != "")
                    _editingObject.Username = _eUserName.Text;

                //if (_ePublicKey.Text != "")
                //    _editingObject.PublicKey = _ePublicKey.Text;   //single sign-in function will be used later single sign-in function

                if (_ePassword.Text != EDITPASSWORD)
                {
                    _editingObject.PasswordHash = PasswordHelper.GetPasswordhash(_eUserName.Text, _ePassword.Text);
                    _editingObject.LastPasswordChangeDate = DateTime.Now; //
                }

                _editingObject.LoginGroup = _actLoginGroup;
                if (_actLoginGroup == null)
                {
                    IList<Access> listAccess = _accessPanelControl.GetListOfAccesses();

                    Access superAdmin = BaseAccess.GetAccess(LoginAccess.SuperAdmin);
                    if (_editingObject.Username == CgpServerGlobals.DEFAULT_ADMIN_LOGIN)
                    {
                        listAccess.Add(superAdmin);
                    }

                    _editingObject.SetAccessControls(listAccess);
                }

                _editingObject.Description = _eDescription.Text;
                _editingObject.IsDisabled = _cbDisabled.Checked;
                _editingObject.MustChangePassword = _cbMustChangePassword.Checked;

                if (_tbdpExpirationDate.Text == "")
                {
                    _editingObject.ExpirationDate = null;
                }
                else
                {
                    _editingObject.ExpirationDate = DateTime.Parse(_tbdpExpirationDate.Text);
                }

                if (_actPerson != null && !_actPerson.Compare(_editingObject.Person) && _actPerson.Logins != null && _actPerson.Logins.Count > 0)
                {
                    Login login = _actPerson.Logins.ElementAt(0);

                    string strLogin = string.Empty;
                    if (login != null)
                        strLogin = login.Username;

                    if (Dialog.Question(LocalizationHelper.GetString("QuestionReplaceLoginForPerson") + " " + strLogin))
                    {
                        CgpClient.Singleton.MainServerProvider.Persons.RemoveLogins(_actPerson.IdPerson);
                    }
                    else
                    {
                        return false;
                    }
                }

                _editingObject.Person = _actPerson;
                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

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
            if (_eUserName.Text == "")
            {
                ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eUserName,
                    GetString("ErrorEntryUserName"), CgpClient.Singleton.ClientControlNotificationSettings);

                return false;
            }

            if (_actLoginGroup != null && _actLoginGroup.ExpirationDate != null)
            {
                DateTime? expirationDate = null;
                if (_tbdpExpirationDate.Text != "")
                {
                    expirationDate = DateTime.Parse(_tbdpExpirationDate.Text);
                }

                if (expirationDate == null || expirationDate > _actLoginGroup.ExpirationDate)
                {
                    if (!Dialog.Question(GetString("QuestionApplyExpirationDateFromLoginGroup")))
                    {
                        return false;
                    }

                    _tbdpExpirationDate.Value = _actLoginGroup.ExpirationDate;
                }
            }

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.Logins.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is IwQuick.SqlUniqueException)
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("LoginEditFormErrorUniqueColumn"));
                }
                else
                    throw error;
            }

            if (_clonedLogin != null)
            {
                IList<UserFoldersStructure> userFolders = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetUserFoldersForObject(_clonedLogin.GetIdString(), _clonedLogin.GetObjectType());
                if (userFolders != null && userFolders.Count > 0)
                {
                    foreach (UserFoldersStructure userFolderStructure in userFolders)
                    {
                        if (userFolderStructure != null)
                        {
                            UserFoldersStructure actUserFolderStructure = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetObjectForEdit(userFolderStructure.IdUserFoldersStructure, out error);
                            if (userFolderStructure != null)
                            {
                                UserFoldersStructureObject userFoldersStructureObject = new UserFoldersStructureObject();
                                userFoldersStructureObject.Folder = userFolderStructure;
                                userFoldersStructureObject.ObjectId = _editingObject.GetIdString();
                                userFoldersStructureObject.ObjectType = _editingObject.GetObjectType();
                                actUserFolderStructure.UserFoldersStructureObjects.Add(userFoldersStructureObject);
                                CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.Update(actUserFolderStructure, out error);
                                CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.EditEnd(actUserFolderStructure);
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue = CgpClient.Singleton.MainServerProvider.Logins.Update(_editingObject, out error);
            if (!retValue && error != null)
            {
                throw error;
            }

            if (retValue)
            {
                if (_editingObject.Username == CgpClient.Singleton.LoggedAs)
                {
                    CgpClientMainForm.Singleton.RefreshLeftMenuItems();
                }
            }

            return retValue;
        }

        private void _bCalendar_Click(object sender, EventArgs e)
        {
            DateTime? expirationDate = null;
            if (_tbdpExpirationDate.Text != "")
            {
                DateTime dateTime;
                if (DateTime.TryParse(_tbdpExpirationDate.Text, out dateTime))
                    expirationDate = dateTime;
            }

            SetDate setDate = new SetDate(expirationDate);
            DateForm calendarForm = new DateForm(LocalizationHelper, setDate);
            calendarForm.ShowDialog();

            if (setDate.IsSetDate)
            {
                _tbdpExpirationDate.Value = setDate.Date;
            }
        }

        private void Modify()
        {
            ConnectionLost();
            Exception error = null;
            try
            {
                List<IModifyObject> listPersons = new List<IModifyObject>();

                IList<IModifyObject> listPersonsFromDatabase = CgpClient.Singleton.MainServerProvider.Persons.ListModifyObjects(out error);
                if (error != null) throw error;
                listPersons.AddRange(listPersonsFromDatabase);


                ListboxFormAdd formAdd = new ListboxFormAdd(listPersons, GetString("LoginEditFormAddPersonFormText"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    _actPerson = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(outModObj.GetId); ;
                    _tbmPerson.Text = _actPerson.ToString();
                    _tbmPerson.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actPerson);

                    CgpClientMainForm.Singleton.AddToRecentList(_actPerson);
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(error);
            }
        }

        protected override void AfterInsert()
        {
            LoginsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            LoginsForm.Singleton.AfterEdit(_editingObject);
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
                //Add((object)e.Data.GetData(typeof(System.MarshalByRefObject)));
                AddLogin((object)e.Data.GetData(output[0]));
                //_listView.AllowDrop = true;
            }
            catch
            {
            }
        }

        private void AddLogin(object newPerson)
        {
            try
            {
                if (newPerson.GetType() == typeof(Contal.Cgp.Server.Beans.Person))
                {
                    Person person = newPerson as Person;
                    _actPerson = person;
                    _tbmPerson.Text = person.ToString();
                    _tbmPerson.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actPerson);

                    CgpClientMainForm.Singleton.AddToRecentList(newPerson);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmPerson.ImageTextBox,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        private void _tbmDailyPlan_DoubleClick(object sender, EventArgs e)
        {
            if (_actPerson != null)
            {
                PersonsForm.Singleton.OpenEditForm(_actPerson);
            }
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && !CgpClient.Singleton.IsConnectionLost(false) && CgpClient.Singleton.MainServerProvider.Logins != null)
                CgpClient.Singleton.MainServerProvider.Logins.EditEnd(_editingObject);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.Logins.
                GetReferencedObjects(_editingObject.IdLogin, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_clonedLogin == null)
                UserFolders_MouseDoubleClick(_lbUserFolders);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            if (_clonedLogin != null)
                UserFolders_Enter(_clonedLogin, _lbUserFolders);
            else
                UserFolders_Enter(_lbUserFolders);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        private void _tbdpExpirationDate_ButtonClearDateClick()
        {
            if (Contal.IwQuick.UI.Dialog.Question(GetString("LoginEditFormDeleteExpirationDateConfirm")))
            {
                _tbdpExpirationDate.Value = null;
            }
        }

        private void _tbmDailyPlan_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                Modify();
            }
            else if (item.Name == "_tsiRemove")
            {
                if (Contal.IwQuick.UI.Dialog.Question(GetString("LoginEditFormDeletePersonConfirm")))
                {
                    _actPerson = null;
                    _tbmPerson.Text = "";
                }
            }
        }

        private void _bClone_Click(object sender, EventArgs e)
        {
            LoginsForm.Singleton.InsertWithData(_editingObject);
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return true;
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return true;
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

        private void _tbmLoginGroup_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                ModifyLoginGroup();
            }
            else if (item.Name == "_tsiRemove2")
            {
                if (Dialog.Question(GetString("LoginEditFormRemoveLoginGroupConfirm")))
                {
                    _actLoginGroup = null;
                    _tbmLoginGroup.Text = "";

                    _accessPanelControl.LoadAccessRights(false);

                    _lLoginGroupIsDisabled.Visible = false;
                }
            }
        }

        private void ModifyLoginGroup()
        {
            ConnectionLost();
            Exception error = null;
            try
            {
                var listLoginGroups = new List<IModifyObject>();

                IList<IModifyObject> listLoginGroupsFromDatabase =
                    CgpClient.Singleton.MainServerProvider.LoginGroups.ListModifyObjectsFromLoginReferencesSites(
                        Insert
                            ? null
                            : (Guid?) _editingObject.IdLogin,
                        out error);

                if (error != null) throw error;

                foreach (var modifyObject in listLoginGroupsFromDatabase)
                {
                    if (!CgpClient.Singleton.MainServerProvider.LoginGroups.IsLoginGroupSuperAdmin(modifyObject.ToString()) ||
                        CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs))
                    {
                        listLoginGroups.Add(modifyObject);
                    }
                }

                var formAdd = new ListboxFormAdd(listLoginGroups, GetString("LoginGroupsFormLoginGroupsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    SetLoginGroup(outModObj.GetId);
                }
            }
            catch
            {
                Dialog.Error(error);
            }
        }

        private void SetLoginGroup(Guid logrinGroupId)
        {
            _actLoginGroup = CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(logrinGroupId);
            _tbmLoginGroup.Text = _actLoginGroup.ToString();
            _tbmLoginGroup.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actLoginGroup);
            _accessPanelControl.LoadAccessRights(true);
            
            _lLoginGroupIsDisabled.Visible = _actLoginGroup.IsDisabled;

            CgpClientMainForm.Singleton.AddToRecentList(_actLoginGroup);
        }

        private void _tbmLoginGroup_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmLoginGroup_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddLoginGroup(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddLoginGroup(object newLoginGroup)
        {
            var loginGroup = newLoginGroup as LoginGroup;

            if (loginGroup != null)
            {
                if (
                    (CgpClient.Singleton.MainServerProvider.LoginGroups.IsLoginGroupSuperAdmin(loginGroup.LoginGroupName) &&
                     !CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs)))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmLoginGroup.ImageTextBox,
                        GetString("ErrorAddLoginGroupAccessDenied"),
                        ControlNotificationSettings.Default);

                    return;
                }

                if (!Insert
                    && !CgpClient.Singleton.MainServerProvider.Logins.IsLoginInLoginGroupReferencesSites(
                        loginGroup.IdLoginGroup,
                        _editingObject.IdLogin))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmLoginGroup.ImageTextBox,
                        GetString("LoginGroupMustBePlacedInSameSiteAsLogin"),
                        ControlNotificationSettings.Default);

                    return;
                }

                SetLoginGroup(loginGroup.IdLoginGroup);
            }
            else
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                    _tbmLoginGroup.ImageTextBox,
                    GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);
            }
        }

        private void _tbmLoginGroup_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actLoginGroup != null)
            {
                LoginGroupsForm.Singleton.OpenEditForm(_actLoginGroup);
            }
        }
    }
}
