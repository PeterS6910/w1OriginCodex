using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using System.IO;

using Contal.IwQuick.UI;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using System.Xml;
using Contal.IwQuick.Sys;
using Cgp.Components;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Parsing;

namespace Contal.Cgp.Client
{
    public partial class PersonEditForm :
#if DESIGNER
        Form
#else
 ACgpEditForm<Person>
#endif
    {
        #region Constants for Print and Capture feature
        /// <summary>
        /// The maximum width of the compressed picture in pixels
        /// </summary>
        private const int MAX_PICTURE_WIDTH = 320;
        /// <summary>
        /// The maximum hight of the compressed picture in pixels
        /// </summary>
        private const int MAX_PICTURE_HEIGHT = 480;
        #endregion

        private const string DEFAULT_PASSWORD = "********";

        private Login _actLogin;
        private Person _clonedPerson;
        private UserFoldersStructure _actDepartment;
        private BinaryPhoto _binaryPhoto;
        private bool _isPhotoModified;
        private UserFoldersStructure _oldDepartment;
        private bool _settingDefaultPassword;

        public PersonEditForm(Person person, ShowOptionsEditForm showOption)
            : base(person, showOption)
        {
            InitializeComponent();
            _tbdpDate.LocalizationHelper = LocalizationHelper;

            if (showOption != ShowOptionsEditForm.InsertWithData)
            {
                if (Insert)
                {
                    _tcPerson.TabPages.Remove(_tpAddAccessControlList);
                    _tcPerson.TabPages.Remove(_tpAccessZone);
                    _tcPerson.TabPages.Remove(_tpActualAccess);
                }
                else
                {
                    LoadNCASPluginTabPages(_editingObject, true);
                }
            }

            SetReferenceEditColors();
            WheelTabContorol = _tcPerson;
            EnableDirectCrButton();
            InitCardStateImageList();
            _eDescription.MouseWheel += ControlMouseWheel;
            CgpClient.Singleton.LoginCrOnlineStateChanged += Singleton_LoginCrOnlineStateChanged;
            CardIdManagementManager.Singleton.ImageCaptureFinished += ImageCaptureFinished;
            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void LoadNCASPluginTabPages(Person person, bool allowEdit)
        {
            if (
                !(CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesView)) ||
                  CgpClient.Singleton.MainServerProvider.HasAccess(
                      BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin))) ||
                !CgpClient.Singleton.LoadPluginControlToForm("NCAS plugin",
                    person,
                    _tpAddAccessControlList,
                    this,
                    allowEdit &&
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin))))
            {
                _tcPerson.TabPages.Remove(_tpAddAccessControlList);
            }

            if (!(CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesView)) ||
                  CgpClient.Singleton.MainServerProvider.HasAccess(
                      BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin))) ||
                !CgpClient.Singleton.LoadPluginControlToForm("NCAS plugin",
                    person,
                    _tpAccessZone,
                    this,
                    allowEdit &&
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin))))
            {
                _tcPerson.TabPages.Remove(_tpAccessZone);
            }
            
            if (!CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.PersonsActualAccessListView)) ||
                !CgpClient.Singleton.LoadPluginControlToForm("NCAS plugin",
                    person,
                    _tpActualAccess,
                    this,
                    allowEdit))
            {
                _tcPerson.TabPages.Remove(_tpActualAccess);
            }
        }

        Action<ObjectType, object, bool> _cudObjectEvent;

        protected override void RegisterEvents()
        {
            _cudObjectEvent = CudObjectEvent;
            CUDObjectHandler.Singleton.Register(_cudObjectEvent, ObjectType.Card);
        }

        protected override void UnregisterEvents()
        {
            if (_cudObjectEvent != null)
            {
                CUDObjectHandler.Singleton.Unregister(_cudObjectEvent, ObjectType.Card);
            }
        }

        private void CudObjectEvent(ObjectType objectType, object id, bool isInsert)
        {
            if ((id == null) || !(id is Guid)) return;

            Card card = CgpClient.Singleton.MainServerProvider.Cards.GetObjectById(id);

            if (card != null)
            {
                UpdateEditingObjectCard(card);
            }
        }


        ImageList _imagelist = null;

        private void InitCardStateImageList()
        {
            _ilbCards.ImageList = ObjectImageList.Singleton.ClientObjectImages;
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageInformation(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsInformationView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsInformationAdmin)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsPersonalCodeView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsPersonalCodeAdmin)));

                HideDisableTabPageLogins(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsLoginsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsLoginsAdmin)),
                    LoginsForm.Singleton.HasAccessInsert());

                HideDisableTabPageCards(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsCardsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsCardsAdmin)),
                    CardsForm.Singleton.HasAccessInsert());

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        BaseAccess.GetAccess(LoginAccess.PersonsDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageInformation(
            bool view,
            bool admin,
            bool personalCodeView,
            bool personalCodeAdmin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool, bool, bool>(HideDisableTabPageInformation),
                    view,
                    admin,
                    personalCodeView,
                    personalCodeAdmin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcPerson.TabPages.Remove(_tpInformation);
                    return;
                }

                if (!admin)
                {
                    _eName.ReadOnly = true;
                    _eMiddleName.ReadOnly = true;
                    _eSurname.ReadOnly = true;
                    _eTitle.ReadOnly = true;
                }

                if (!personalCodeView && !personalCodeAdmin)
                {
                    _lPersonalCode.Visible = false;
                    _ePersonalCode.Visible = false;
                    _bGeneratePersonalCode.Visible = false;
                }
                else if (!personalCodeAdmin)
                {
                    _lPersonalCode.Visible = true;
                    _ePersonalCode.Visible = true;
                    _ePersonalCode.ReadOnly = true;
                    _bGeneratePersonalCode.Visible = false;
                }
                else
                {
                    _lPersonalCode.Visible = true;
                    _ePersonalCode.Visible = true;
                    _ePersonalCode.ReadOnly = false;
                    _bGeneratePersonalCode.Visible = true;
                }

                _tpInformation.Enabled = admin;
            }
        }

        private void HideDisableTabPageLogins(bool view, bool admin, bool insertLogin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool, bool>(HideDisableTabPageLogins),
                    view,
                    admin,
                    insertLogin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcPerson.TabPages.Remove(_tbLogins);
                    return;
                }

                _tbLogins.Enabled = admin;

                if (!insertLogin)
                    _tbmLogin.ButtonPopupMenu.Items.Remove(_tsiRemove);
            }
        }

        private void HideDisableTabPageCards(bool view, bool admin, bool insertCard)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool, bool>(HideDisableTabPageCards),
                    view,
                    admin,
                    insertCard);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcPerson.TabPages.Remove(_tbCards);
                    return;
                }

                _tbCards.Enabled = admin;

                if (!insertCard)
                    _bCreate1.Enabled = false;
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
                    _tcPerson.TabPages.Remove(_tpUserFolders);
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
                    _tcPerson.TabPages.Remove(_tpReferencedBy);
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
                    _tcPerson.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        public override void SetValuesInsertFromObj(object obj)
        {
            if (obj != null && obj is Person)
            {
                _clonedPerson = obj as Person;

                LoadNCASPluginTabPages(_clonedPerson, false);
            }
        }

        public override void SetValuesInsertFromObjWithFilter(object obj, Dictionary<string, bool> filter)
        {
            if (filter == null || filter.Count == 0)
                SetValuesInsertFromObj(obj);
            else
            {
                _clonedPerson = obj as Person;

                foreach (KeyValuePair<string, bool> filterItem in filter)
                {
                    if (filterItem.Value)
                    {
                        // SB
                        switch (filterItem.Key)
                        {
                            case "Company":
                                _editingObject.Company = _clonedPerson.Company;
                                break;
                            case "Role":
                                _editingObject.Role = _clonedPerson.Role;
                                break;
                            case "Address":
                                _editingObject.Address = _clonedPerson.Address;
                                break;
                            case "Email":
                                _editingObject.Email = _clonedPerson.Email;
                                break;
                            case "Phone":
                                _editingObject.PhoneNumber = _clonedPerson.PhoneNumber;
                                break;
                            case "Department":
                                _editingObject.Department = _clonedPerson.Department;
                                break;
                            case "Relative_superior":
                                _editingObject.RelativeSuperior = _clonedPerson.RelativeSuperior;
                                break;
                            case "Relative_superiorsPhoneNumber":
                                _editingObject.RelativeSuperiorsPhoneNumber = _clonedPerson.RelativeSuperiorsPhoneNumber;
                                break;
                            case "CostCenter":
                                _editingObject.CostCenter = _clonedPerson.CostCenter;
                                break;
                            case "EmploymentBeginningDate":
                                _editingObject.EmploymentBeginningDate = _clonedPerson.EmploymentBeginningDate;
                                break;
                            case "EmpoymentEndDate":
                                _editingObject.EmploymentEndDate = _clonedPerson.EmploymentEndDate;
                                break;
                            default:
                                break;
                        }
                    }
                }

                SetValuesEdit();

                LoadNCASPluginTabPages(_clonedPerson, false);
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = LocalizationHelper.GetString("PersonEditFormInsertText");
            }

            if (_editingObject.SynchronizedWithTimetec)
            {
                DisabledControls(this);

                EnabledControls(_tbmDepartment);
                EnabledControls(_tpAccessZone);
                EnabledControls(_tpActualAccess);
                EnabledControls(_tpAddAccessControlList);
                EnabledControls(_tbLogins);
                EnabledControls(_ilbCards);
                EnabledControls(_tpDescription);
                EnabledControls(_bBrowsePhoto);
                EnabledControls(_bCapture);

                Text = string.Format("{0} [Timetec]", Text);
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            PersonsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            PersonsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        private void ShowCards(string filter)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowCards), filter);
            }
            else
            {
                ReloadCardsForPerson();
                _ilbCards.Items.Clear();

                if (_editingObject.Cards != null)
                {
                    foreach (Card card in _editingObject.Cards)
                    {
                        if (filter == "" || card.ToString().IndexOf(filter) == 0)
                        {
                            _ilbCards.Items.Add(new ImageListBoxItem(card, card.GetSubTypeImageString("State")));//
                        }
                    }
                }
            }
        }

        private void UpdateEditingObjectCard(Card card)
        {
            try
            {
                foreach (ImageListBoxItem item in _ilbCards.Items)
                {
                    if ((item.MyObject as Card).IdCard == card.IdCard)
                    {
                        item.MyObject = card;
                        item.ImageIndex = card.State;
                        RemoveCardFromEditingObject(item.MyObject as Card);
                        _editingObject.Cards.Add(card);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
            RefreshCardsImageListBox();
        }

        private void RemoveCardFromEditingObject(Card card)
        {
            Card cardToRemove = null;

            foreach (Card item in _editingObject.Cards)
            {
                if (item.IdCard == card.IdCard)
                {
                    cardToRemove = item;
                    break;
                }
            }

            _editingObject.Cards.Remove(cardToRemove);
        }

        private void AddCardsImageListBoxItem(Card card)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Card>(AddCardsImageListBoxItem), card);
            }
            else
            {
                _ilbCards.Items.Add(new ImageListBoxItem(card, card.GetSubTypeImageString("State")));
            }
        }


        private void RemoveDeletedCard(ImageListBoxItem itemToRemove)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ImageListBoxItem>(RemoveDeletedCard), itemToRemove);
            }
            else
            {
                if (itemToRemove != null)
                {
                    _ilbCards.Items.Remove(itemToRemove);
                }
            }
        }

        private void RefreshCardsImageListBox()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(RefreshCardsImageListBox));
            }
            else
            {
                _ilbCards.Refresh();
            }
        }

        protected override void InternalReloadEditingObject(out bool allowEdit)
        {
            Exception error = null;
            Person obj = CgpClient.Singleton.MainServerProvider.Persons.GetObjectForEdit(_editingObject.IdPerson, out error);
            if (error != null)
            {
                allowEdit = false;
                if (error is Contal.IwQuick.AccessDeniedException)
                {
                    obj = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(_editingObject.IdPerson);
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
            Exception error = null;
            CgpClient.Singleton.MainServerProvider.Persons.RenewObjectForEdit(_editingObject.IdPerson, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            _bClone.Enabled = false;
            _bApply.Enabled = false;
        }

        protected override void SetValuesEdit()
        {
            _eName.Text = _editingObject.FirstName;
            _eMiddleName.Text = _editingObject.MiddleName;
            _eSurname.Text = _editingObject.Surname;
            _eTitle.Text = _editingObject.Tiltle;
            _eAddress.Text = _editingObject.Address;
            _eCompany.Text = _editingObject.Company;
            _eRole.Text = _editingObject.Role;
            _eEmail.Text = _editingObject.Email;
            _ePhoneNumber.Text = _editingObject.PhoneNumber;
            _eNumber.Text = _editingObject.Identification;
            _eDescription.Text = _editingObject.Description;
            _eEmployeeNumber.Text = _editingObject.EmployeeNumber;
            _eRelativeSuperior.Text = _editingObject.RelativeSuperior;
            _eRelativeSuperiorsPhoneNumber.Text = _editingObject.RelativeSuperiorsPhoneNumber;
            _eCostCenter.Text = _editingObject.CostCenter;

            SetDepartmentFullName(_editingObject.Department);
            SetActDepartment(_editingObject.Department);
            _oldDepartment = _actDepartment;

            if (_editingObject.Birthday != null)
            {
                _tbdpDate.Value = _editingObject.Birthday.Value;
            }
            else
            {
                _tbdpDate.Value = null;
            }

            if (_editingObject.EmploymentBeginningDate != null)
            {
                _tbdpEmploymentBeginningDate.Value = _editingObject.EmploymentBeginningDate.Value;
            }
            else
            {
                _tbdpEmploymentBeginningDate.Value = null;
            }

            if (_editingObject.EmploymentEndDate != null)
            {
                _tbdpEmploymentEndDate.Value = _editingObject.EmploymentEndDate.Value;
            }
            else
            {
                _tbdpEmploymentEndDate.Value = null;
            }

            if (_editingObject.Logins != null && _editingObject.Logins.Count > 0)
            {
                _actLogin = _editingObject.Logins.ElementAt(0);
                _tbmLogin.Text = _actLogin.ToString();
                _tbmLogin.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actLogin);
            }
            SetReferencedBy();

            if (!string.IsNullOrEmpty(_editingObject.PersonalCodeHash))
            {
                _settingDefaultPassword = true;
                _ePersonalCode.Text = DEFAULT_PASSWORD;
                _settingDefaultPassword = false;
            }

            _bApply.Enabled = false;

            SafeThread.StartThread(ShowPhoto);
            SafeThread.StartThread(SetCardPrintAndCaptureFeature);
        }

        private void SetDepartmentFullName(UserFoldersStructure department)
        {
            if (department == null)
                return;

            department.SetFullFolderName(
                CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetFullDepartmentName(
                    department.GetIdString(),
                    department.FolderName,
                    @"\",
                    CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode")));
        }

        private void SetCardPrintAndCaptureFeature()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            bool hasAccessCardPrintPerform =
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.IdManagementCardPrintPerform));

            bool enabledByLicense = false;

#if !DEBUG
            string localisedName = string.Empty;
            object value = null;
            if (CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(Cgp.Globals.RequiredLicenceProperties.IdManagement.ToString(), out localisedName, out value))
                enabledByLicense = (value is bool && (bool)value == true);
#else
            enabledByLicense = true;
#endif

            Invoke(new MethodInvoker(delegate
                {
                    _bPrintCards.Enabled = enabledByLicense && hasAccessCardPrintPerform;
                }));
        }

        protected override void DisabledForm()
        {
            base.DisabledForm();

            _bCancel.Enabled = true;
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eName,
                        GetString("ErrorInsertName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(_eSurname.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eSurname,
                        GetString("ErrorInsertSurname"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eSurname.Focus();
                return false;
            }


            if (_tbdpEmploymentBeginningDate.Value != null && _tbdpEmploymentEndDate.Value != null && _tbdpEmploymentEndDate.Value < _tbdpEmploymentBeginningDate.Value)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbdpEmploymentEndDate.TextBox,
                GetString("ErrorEndDateBeginningDate"), CgpClient.Singleton.ClientControlNotificationSettings);
                _tbdpEmploymentEndDate.TextBox.Focus();
                _tcPerson.SelectedTab = _tpInformation;
                return false;
            }

            try
            {
                if (GeneralOptionsForm.Singleton.GetUniqueNotNullPersonalKey)
                {
                    if (string.IsNullOrEmpty(_eNumber.Text))
                    {
                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eNumber,
                            GetString("ErrorEntryPersonalKey"), CgpClient.Singleton.ClientControlNotificationSettings);
                        _tcPerson.SelectedTab = _tpInformation;
                        _eNumber.Focus();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }

            if (_eEmail.Text != string.Empty)
            {
                if (!Contal.IwQuick.Net.EmailAddress.IsValid(_eEmail.Text))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eEmail,
                            GetString("ErrorWrongEmailAddress"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _tcPerson.SelectedTab = _tpInformation;
                    _eEmail.Focus();
                    return false;
                }
            }

            if (_ePhoneNumber.Text != string.Empty)
            {
                if (!PresentationGroupEditForm.IsValidPhoneNumber(_ePhoneNumber.Text))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _ePhoneNumber,
                                GetString("ErrorInvalidPhoneNumber"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _tcPerson.SelectedTab = _tpInformation;
                    _ePhoneNumber.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(_ePersonalCode.Text))
            {
                if (_ePersonalCode.Text == DEFAULT_PASSWORD)
                {
                    if (CodeHasWrongLength(_editingObject.PersonalCodeLength))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _ePersonalCode,
                            string.Format(
                                GetString("ErrorWrongPersonalCodeLength"),
                                GeneralOptionsForm.Singleton.MinimalCodeLength,
                                GeneralOptionsForm.Singleton.MaximalCodeLength),
                            CgpClient.Singleton.ClientControlNotificationSettings);

                        _tcPerson.SelectedTab = _tpInformation;

                        _ePersonalCode.Focus();

                        return false;
                    }
                }
                else
                {
                    if (CodeHasWrongLength(_ePersonalCode.Text.Length))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _ePersonalCode,
                            string.Format(
                                GetString("ErrorWrongPersonalCodeLength"),
                                GeneralOptionsForm.Singleton.MinimalCodeLength,
                                GeneralOptionsForm.Singleton.MaximalCodeLength),
                            CgpClient.Singleton.ClientControlNotificationSettings);

                        _tcPerson.SelectedTab = _tpInformation;

                        _ePersonalCode.Focus();

                        return false;
                    }

#if !DEBUG
                    if (CodeHasLowSecurityStrength(_ePersonalCode.Text))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _ePersonalCode,
                            GetString("ErrorPersonslaCodeLowSecurityStrength"),
                            CgpClient.Singleton.ClientControlNotificationSettings);

                        _tcPerson.SelectedTab = _tpInformation;

                        _ePersonalCode.Focus();

                        return false;
                    }
#endif
                }
            }

            return true;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.FirstName = _eName.Text;
                _editingObject.MiddleName = _eMiddleName.Text;
                _editingObject.Surname = _eSurname.Text;
                if (_eTitle.Text != string.Empty && _eTitle.Text.Length > 30)
                    _eTitle.Text = _eTitle.Text.Substring(0, 30);
                _editingObject.Tiltle = _eTitle.Text;
                _editingObject.Address = _eAddress.Text;
                _editingObject.Company = _eCompany.Text;
                _editingObject.Role = _eRole.Text;
                _editingObject.Email = _eEmail.Text;
                _editingObject.PhoneNumber = _ePhoneNumber.Text;
                _editingObject.Identification = _eNumber.Text;
                _editingObject.Description = _eDescription.Text;
                _editingObject.Department = _actDepartment;
                _editingObject.EmployeeNumber = _eEmployeeNumber.Text;
                _editingObject.RelativeSuperior = _eRelativeSuperior.Text;
                _editingObject.RelativeSuperiorsPhoneNumber = _eRelativeSuperiorsPhoneNumber.Text;
                _editingObject.CostCenter = _eCostCenter.Text;

                if (_tbdpDate.Text == string.Empty)
                {
                    _editingObject.Birthday = null;
                }
                else
                {
                    DateTime dt;
                    if (DateTime.TryParse(_tbdpDate.Text, out dt))
                    {
                        if (dt > DateTime.Now)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbdpDate.TextBox,
                                GetString("ErrorBirthDateLaterThanNow"), ControlNotificationSettings.Default);
                            return false;
                        }
                        else
                        {
                            _editingObject.Birthday = dt;
                        }
                    }
                    else
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInvalidData"));
                        return false;
                    }
                }

                if (_tbdpEmploymentBeginningDate.Text == string.Empty)
                {
                    _editingObject.EmploymentBeginningDate = null;
                }
                else
                {
                    DateTime dt;
                    if (DateTime.TryParse(_tbdpEmploymentBeginningDate.Text, out dt))
                    {
                        _editingObject.EmploymentBeginningDate = dt;
                    }
                    else
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInvalidData"));
                        return false;
                    }
                }

                if (_tbdpEmploymentEndDate.Text == string.Empty)
                {
                    _editingObject.EmploymentEndDate = null;
                }
                else
                {
                    DateTime dt;
                    if (DateTime.TryParse(_tbdpEmploymentEndDate.Text, out dt))
                    {
                        _editingObject.EmploymentEndDate = dt;
                    }
                    else
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInvalidData"));
                        return false;
                    }
                }

                ICollection<Login> logins = new List<Login>();
                if (_actLogin != null)
                {
                    logins.Add(_actLogin);
                }
                _editingObject.Logins = logins;

                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

                if (!string.IsNullOrEmpty(_ePersonalCode.Text))
                {
                    if (_ePersonalCode.Text != DEFAULT_PASSWORD)
                    {
                        _editingObject.PersonalCodeHash = QuickHashes.GetCRC32String(_ePersonalCode.Text);
                        _editingObject.PersonalCodeLength = (byte)_ePersonalCode.Text.Length;
                    }
                }
                else
                {
                    _editingObject.PersonalCodeHash = null;
                    _editingObject.PersonalCodeLength = 0;
                }

                return true;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorGetValuesFailed"));

                return false;
            }
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error = null;

            bool retValue = CgpClient.Singleton.MainServerProvider.Persons.InsertPerson(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message == Person.COLUMNIDENTIFICATION)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eNumber,
                        GetString("ErrorUsedPersonalKey"),
                        CgpClient.Singleton.ClientControlNotificationSettings);

                    _eNumber.Focus();

                    return false;
                }
                else if (error is Contal.IwQuick.SqlUniqueException && error.Message == Person.COLUMN_PERSONAL_CODE_HASH)
                {
                    ErrorNotificationOverControl(_ePersonalCode,"ErrorCodeAlreadyUsed");

                    _tcPerson.SelectedTab = _tpInformation;
                    _ePersonalCode.Focus();

                    return false;
                }
                else
                    throw error;
            }

            // SB
            if (_clonedPerson != null && _editingObject.Department != null)
            {
                CgpClient.Singleton.SaveAfterInsertWithData("NCAS plugin", _editingObject, _clonedPerson);

                IList<UserFoldersStructure> userFolders = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetUserFoldersForObject(_clonedPerson.GetIdString(), _clonedPerson.GetObjectType());

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
            
            CgpClient.Singleton.MainServerProvider.Persons.SetPersonDepartment(_actDepartment, _editingObject.IdPerson);

            if (retValue)
            {
                SavePhoto();
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
            Exception error = null;
            bool retValue;

            if (onlyInDatabase)
                retValue = CgpClient.Singleton.MainServerProvider.Persons.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = CgpClient.Singleton.MainServerProvider.Persons.UpdatePerson(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is Contal.IwQuick.SqlUniqueException && error.Message == Person.COLUMNIDENTIFICATION)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eNumber,
                        GetString("ErrorUsedPersonalKey"),
                        CgpClient.Singleton.ClientControlNotificationSettings);

                    _eNumber.Focus();

                    return false;
                }
                else if (error is Contal.IwQuick.SqlUniqueException && error.Message == Person.COLUMN_PERSONAL_CODE_HASH)
                {
                    ErrorNotificationOverControl(_ePersonalCode,"ErrorCodeAlreadyUsed");

                    _tcPerson.SelectedTab = _tpInformation;
                    _ePersonalCode.Focus();

                    return false;
                }
                else
                    throw error;
            }

            CgpClient.Singleton.MainServerProvider.Persons.SetPersonDepartment(_actDepartment, _oldDepartment, _editingObject.IdPerson);
            
            if (retValue)
            {
                SavePhoto();
            }

            return retValue;
        }

        private void SavePhoto()
        {
            if (_isPhotoModified)
            {
                _isPhotoModified = false;
                SafeThread<Guid, BinaryPhoto>.StartThread(DoSavePhoto, _editingObject.IdPerson, _binaryPhoto);
            }
        }

        private void DoSavePhoto(Guid idPerson, BinaryPhoto binaryPhoto)
        {
            try
            {
                CgpClient.Singleton.MainServerProvider.Persons.SavePhoto(idPerson, binaryPhoto);

                Exception error;
                _editingObject = CgpClient.Singleton.MainServerProvider.Persons.GetObjectForEdit(_editingObject.IdPerson, out error);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        protected override bool SelectSubSiteImplicit(Person person, out ICollection<int> selectedSubSites)
        {
            if (person != null && person.Department != null)
            {
                var objectPlacement = CgpClient.Singleton.MainServerProvider.StructuredSubSites.GetObjectPlacements(
                    person.Department.GetObjectType(),
                    person.Department.GetIdString(),
                    @"\",
                    CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode"),
                    true).FirstOrDefault();

                if (objectPlacement != null)
                {
                    selectedSubSites = new LinkedList<int>(
                        Enumerable.Repeat(objectPlacement.SiteId, 1));

                    return true;
                }
            }

            return base.SelectSubSiteImplicit(person, out selectedSubSites);
        }

        protected override void AfterInsert()
        {
            PersonsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            PersonsForm.Singleton.AfterEdit(_editingObject);
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

            if (!_settingDefaultPassword
                && sender == _ePersonalCode)
            {
                var textBox = (TextBox)sender;

                if (textBox.Text == DEFAULT_PASSWORD)
                    textBox.Text = string.Empty;

                var validText = QuickParser.GetValidDigitString(textBox.Text);

                if (textBox.Text != validText)
                {
                    textBox.Text = validText;
                    textBox.SelectionStart = textBox.TextLength;
                    textBox.SelectionLength = 0;
                }

                var maximalCodeLength = GeneralOptionsForm.Singleton.MaximalCodeLength;

                if (textBox.Text.Length > maximalCodeLength)
                {
                    textBox.Text = textBox.Text.Substring(0, maximalCodeLength);
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                }
            }

            if (!Insert)
                _bApply.Enabled = true;
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (sender == _eTitle)
            {
                if (_eTitle.Text.Length > 30)
                {
                    _eTitle.Text = _eTitle.Text.Substring(0, 30);
                    _eTitle.SelectionStart = _eTitle.Text.Length;
                    _eTitle.SelectionLength = 0;
                }
            }

            if (!Insert)
                _bApply.Enabled = true;
        }

        private void _bAddCard_Click(object sender, EventArgs e)
        {
            if (AddCards())
            {
                ShowCards(_eFilterCards.Text);
            }
        }

        private bool AddCards()
        {
            if (_editingObject.IdPerson == Guid.Empty)
            {
                Ok_Click(false);
                return false;
            }
            ConnectionLost();
            try
            {
                IList<IModifyObject> listCards = new List<IModifyObject>();

                Exception error = null;
                listCards = CgpClient.Singleton.MainServerProvider.Cards.ModifyObjectsFormPersonAddCard(_editingObject.IdPerson, out error);

                if (error != null)
                    throw error;

                ListboxFormAdd formAdd = new ListboxFormAdd(listCards, GetString("CardsFormCardsForm"), true);

                ListOfObjects outCards;
                formAdd.ShowDialogMultiSelect(out outCards);

                if (outCards != null)
                {
                    IList<Card> listcard = CgpClient.Singleton.MainServerProvider.Cards.GetCardsByListGuids(CalGuidFromListObj(outCards));

                    if (Contal.IwQuick.UI.Dialog.Question(GetString("PersonEditFormAddCardsConfirm")))
                    {
                        IList<Guid> addCardsList = new List<Guid>();
                        foreach (Card card in listcard)
                        {
                            bool bAdd = true;
                            if (card.Person != null && !_editingObject.Compare(card.Person))
                            {
                                if (!Contal.IwQuick.UI.Dialog.Question(GetString("PersonEditFormAddAddedCardConfirm") + card.Person.ToString()))
                                    bAdd = false;
                            }

                            if (bAdd)
                            {
                                addCardsList.Add(card.IdCard);
                            }
                        }

                        _questionForPinByFirstCard(addCardsList);
                        CgpClient.Singleton.MainServerProvider.Cards.SetCardsToPerson(addCardsList, _editingObject.IdPerson);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAddCard"));
                return false;
            }
        }

        private void ReloadCardsForPerson()
        {
            Person tmpPerson = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(_editingObject.IdPerson);
            if (tmpPerson != null)
                _editingObject.Cards = tmpPerson.Cards;
        }

        private IList<object> CalGuidFromListObj(ListOfObjects cards)
        {
            IList<object> listGuids = new List<object>();
            foreach (object cardLo in cards)
            {
                listGuids.Add((cardLo as IModifyObject).GetId);
            }
            return listGuids;
        }

        private void _bCreate1_Click(object sender, EventArgs e)
        {
            if (_editingObject.IdPerson == Guid.Empty)
            {
                Ok_Click(false);
            }
            if (_editingObject.IdPerson != Guid.Empty)
            {
                Card card = new Card();
                card.Person = _editingObject;
                CardsForm.Singleton.OpenInsertFromEdit(ref card, DoAfterCreateCard);
            }
        }

        private void DoAfterCreateCard(object card)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCreateCard), card);
            }
            else
            {
                ShowCards(_eFilterCards.Text);
            }
        }

        private void _bDeleteCard_Click(object sender, EventArgs e)
        {
            ConnectionLost();
            try
            {
                ListBox.SelectedObjectCollection sic = _ilbCards.SelectedItems;
                if (sic == null) return;
                string questionString = string.Empty;
                if (sic.Count == 1)
                    questionString = GetString("PersonEditFormDeleteCardConfirm");
                else
                    questionString = GetString("PersonEditFormDeleteCardsConfirm");


                if (Contal.IwQuick.UI.Dialog.Question(questionString))
                {
                    IList<Guid> deleteCardsList = new List<Guid>();
                    Card card;
                    foreach (object obj in sic)
                    {
                        card = (obj as ImageListBoxItem).MyObject as Card;
                        if (card != null)
                        {
                            deleteCardsList.Add(card.IdCard);
                        }
                    }

                    CgpClient.Singleton.MainServerProvider.Cards.RemoveCardsFromPerson(deleteCardsList, _editingObject.IdPerson);
                    ShowCards(_eFilterCards.Text);
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteCard"));
                ShowCards(_eFilterCards.Text);
            }
        }

        private void _eFilterCards_KeyUp(object sender, KeyEventArgs e)
        {
            ShowCards(_eFilterCards.Text);
        }

        private void _ilbCards_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _ilbCards_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddCard((object)e.Data.GetData(output[0]));
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void AddCard(object newCard)
        {
            if (_editingObject.IdPerson == Guid.Empty)
            {
                Ok_Click(false);
                return;
            }
            ConnectionLost();
            try
            {
                if (newCard.GetType() == typeof(Card))
                {
                    bool bAdd = false;
                    Card card = newCard as Card;

                    foreach (ImageListBoxItem item in _ilbCards.Items)
                    {
                        if (card.Compare(item.MyObject))
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("PersonEditFormAddCardAlreadyInList"));
                            return;
                        }
                    }

                    if (card.Person != null && !_editingObject.Compare(card.Person))
                    {
                        if (Contal.IwQuick.UI.Dialog.Question(GetString("PersonEditFormAddAddedCardConfirm") + card.Person.ToString()))
                            bAdd = true;
                    }
                    else
                        bAdd = true;

                    if (bAdd)
                    {
                        IList<Guid> addCardList = new List<Guid>();
                        addCardList.Add(card.IdCard);
                        _questionForPinByFirstCard(addCardList);
                        CgpClient.Singleton.MainServerProvider.Cards.SetCardsToPerson(addCardList, _editingObject.IdPerson);
                        ShowCards(_eFilterCards.Text);
                    }
                    CgpClientMainForm.Singleton.AddToRecentList(newCard);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _ilbCards,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void _questionForPinByFirstCard(IList<Guid> cards)
        {
            Exception error;
            IList<Card> personCards = _editingObject.Cards as IList<Card>;

            if (personCards.Count > 0 && IwQuick.UI.Dialog.Question(String.Format(GetString("QuestionSamePIN"),
                _editingObject.FirstName, _editingObject.Surname)))
            {
                foreach (Guid newCardGuid in cards)
                {
                    Card personNewCard = CgpClient.Singleton.MainServerProvider.Cards.GetObjectForEdit(newCardGuid, out error);

                    if (error != null)
                        throw error;

                    personNewCard.Pin = personCards[0].Pin;
                    personNewCard.PinLength = personCards[0].PinLength;
                    if (CgpClient.Singleton.MainServerProvider.Cards.Update(personNewCard, out error) == false)
                    {
                        IwQuick.UI.Dialog.Error(GetString("ErrorInsertPIN"));
                    }
                    CgpClient.Singleton.MainServerProvider.Cards.EditEnd(personNewCard);
                }
            }
        }

        private void _ilbCards_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_ilbCards.SelectedItem != null)
            {
              CardsForm.Singleton.OpenEditForm(_ilbCards.SelectedItemObject as Card, DoAfterCardEdited);
            }
        }

        private void DoAfterCardEdited(object card)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCardEdited), card);
            }
            else
            {
                ShowCards(_eFilterCards.Text);
            }
        }

        protected override void EditEnd()
        {
            if (CgpClient.Singleton.MainServerProvider != null && CgpClient.Singleton.MainServerProvider.Persons != null)
                CgpClient.Singleton.MainServerProvider.Persons.EditEnd(_editingObject);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, CgpClient.Singleton.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return CgpClient.Singleton.MainServerProvider.Persons.
                GetReferencedObjects(_editingObject.IdPerson as object, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            if (_clonedPerson != null)
                UserFolders_Enter(_clonedPerson, _lbUserFolders);
            else
                UserFolders_Enter(_lbUserFolders);
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_clonedPerson == null)
                UserFolders_MouseDoubleClick(_lbUserFolders);
        }

        private void _bRefresh1_Click(object sender, EventArgs e)
        {
            UserFolders_Enter(_lbUserFolders);
        }

        #region Login TextBox
        //_actLogin
        private void SetActLogin(Login login)
        {
            EditTextChanger(null, null);
            _actLogin = login;
            if (_actLogin == null)
            {
                _tbmLogin.Text = string.Empty;
            }
            else
            {
                _tbmLogin.Text = _actLogin.ToString();
                _tbmLogin.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actLogin);
            }
        }

        private void AddActLogin(object newLogin)
        {
            try
            {
                if (newLogin.GetType() == typeof(Login))
                {
                    SetActLogin((Login)newLogin);
                    CgpClientMainForm.Singleton.AddToRecentList(newLogin);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmLogin.ImageTextBox,
                       GetString("ErrorWrongObjectType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void ModifyActLogin()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                List<IModifyObject> listModObj = new List<IModifyObject>();

                IList<IModifyObject> listLogins = CgpClient.Singleton.MainServerProvider.Logins.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listLogins);

                ListboxFormAdd formAdd = new ListboxFormAdd(listModObj, CgpClient.Singleton.LocalizationHelper.GetString("LoginsFormLoginsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    Login login = CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(outModObj.GetId);
                    if (error != null) throw error;
                    SetActLogin(login);
                    CgpClientMainForm.Singleton.AddToRecentList(_actLogin);
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Contal.IwQuick.UI.Dialog.Error(error);
            }
        }

        private void DoAfterCreatedActLogin(object newLogin)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCreatedActLogin), newLogin);
            }
            else
            {
                if (newLogin is Login)
                {
                    SetActLogin(newLogin as Login);
                }
            }
        }

        private void _tbmLogin_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyActLogin();
            }
            else if (item.Name == "_tsiRemove")
            {
                SetActLogin(null);
            }
            else if (item.Name == "_tsiCreate")
            {
                Login login = new Login();
                LoginsForm.Singleton.OpenInsertFromEdit(ref login, DoAfterCreatedActLogin);
            }
        }

        private void _tbmLogin_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddActLogin((object)e.Data.GetData(output[0]));
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void _tbmLogin_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmLogin_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actLogin != null)
            {
                LoginsForm.Singleton.OpenEditForm(_actLogin);
            }
        }
        #endregion

        private void _bClone_Click(object sender, EventArgs e)
        {
            ApplyClickWithDialog();
            CloneFilterForm cloneForm = new CloneFilterForm(CloneFilterValues.Singleton.GetCloneFilterValues(ObjectType.Person));
            if (cloneForm.ShowDialog() == DialogResult.OK)
            {
                CloneFilterValues.Singleton.SetDefaultCloneFilterForObjectType(ObjectType.Person, cloneForm.FilterValues);
                PersonsForm.Singleton.InsertWithDataAndFilter(_editingObject, cloneForm.FilterValues);
            }
        }

        protected override void AfterDockUndock()
        {

        }

        private bool _tpCardFirstEnter = true;
        private void _tbCards_Enter(object sender, EventArgs e)
        {
            if (_tpCardFirstEnter)
            {
                _tpCardFirstEnter = false;
                Contal.IwQuick.Threads.SafeThread<string>.StartThread(ShowCards, _eFilterCards.Text);
            }
        }

        #region Department TextBox
        //_actLogin
        private void SetActDepartment(UserFoldersStructure department)
        {
            EditTextChangerOnlyInDatabase(null, null);
            _actDepartment = department;
            if (_actDepartment == null)
            {
                _tbmDepartment.Text = string.Empty;
            }
            else
            {
                _tbmDepartment.Text = _actDepartment.ToString();
                _tbmDepartment.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(_actDepartment);
            }
        }

        private void AddActDepartment(object newDepartment)
        {
            try
            {
                var department = newDepartment as UserFoldersStructure;
                if (department != null)
                {
                    SetDepartmentFullName(department);
                    SetActDepartment(department);
                    CgpClientMainForm.Singleton.AddToRecentList(department);
                }
                else
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmDepartment.ImageTextBox,
                       GetString("ErrorWrongObjectType"), CgpClient.Singleton.ClientControlNotificationSettings);
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void ModifyActDepartment()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                var personId = Insert ? null : (object) _editingObject.IdPerson;

                Exception error;
                var listDepartments = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.ListDepartments(
                    CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode"),
                    @"\",
                    personId,
                    out error);

                if (error != null) throw error;

                ListboxFormAdd formAdd = new ListboxFormAdd(
                    listDepartments,
                    CgpClient.Singleton.LocalizationHelper.GetString(
                        "UserFoldersStructuresFormUserFoldersStructuresForm"));

                object outUserFolder;
                formAdd.ShowDialog(out outUserFolder);
                if (outUserFolder != null)
                {
                    UserFoldersStructure UserFolder = outUserFolder as UserFoldersStructure;
                    SetActDepartment(UserFolder);
                    CgpClientMainForm.Singleton.AddToRecentList(_actDepartment);
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void DepartmentButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify1")
            {
                ModifyActDepartment();
            }
            else if (item.Name == "_tsiRemove1")
            {
                SetActDepartment(null);
            }
        }

        private void DepartmentDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddActDepartment((object)e.Data.GetData(output[0]));
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void DepartmentDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void DepartmentTextBoxDoubleClick(object sender, EventArgs e)
        {
            if (_actDepartment != null)
            {
                try
                {
                    DbsSupport.OpenEditForm(_actDepartment);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }
        #endregion

        private void ReadCardFromDirectCRClick(object sender, EventArgs e)
        {
            if (_editingObject.IdPerson == Guid.Empty)
            {
                Ok_Click(false);
            }

            if (_editingObject.IdPerson != Guid.Empty)
            {
                DirectCrSwipCard directCrSwipCard = new DirectCrSwipCard(CgpClient.Singleton.LocalizationHelper);
                string cardNubmer;
                directCrSwipCard.ShowDialog(out cardNubmer);
                if (!String.IsNullOrEmpty(cardNubmer))
                {
                    Card card = new Card();
                    card.Person = _editingObject;
                    card.FullCardNumber = cardNubmer;
                    _bReadCardFromDirectCR.Enabled = false;
                    SafeThread<Card>.StartThread(DoCreateCard, card);
                }
            }
        }

        private void DoCreateCard(Card card)
        {
            try
            {
                CardsForm.Singleton.InsertWithData(card, DoAfterCreateCard);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
            DoAfterShowCardForm();
        }

        private void DoAfterShowCardForm()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(DoAfterShowCardForm));
            }
            else
            {
                _bReadCardFromDirectCR.Enabled = true;
            }
        }

        private void EnableDirectCrButton()
        {
            if (CgpClient.Singleton.LoginCrState != null && (bool)CgpClient.Singleton.LoginCrState)
            {
                SetButtonDirectCrEnabledState(true);
            }
            else
            {
                SetButtonDirectCrEnabledState(false);
            }
        }

        private void SetButtonDirectCrEnabledState(bool enabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DBool2Void(SetButtonDirectCrEnabledState), enabled);
            }
            else
            {
                _bReadCardFromDirectCR.Enabled = enabled;
            }
        }

        void Singleton_LoginCrOnlineStateChanged(bool inputBoolean)
        {
            SetButtonDirectCrEnabledState(inputBoolean);
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                SetValuesEdit();
                ResetWasChangedValues();
                _oldDepartment = _editingObject.Department;
                _bApply.Enabled = false;
            }
        }

        private void _bBrowsePhoto_Click(object sender, EventArgs e)
        {
            if (_ofdBrowseImage.ShowDialog() == DialogResult.OK)
            {
                string photoPath = _ofdBrowseImage.FileName;

                try
                {
                    Image imgPhoto = Image.FromFile(photoPath);
                    ProcessPhoto((Bitmap)imgPhoto);
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                }
            }
        }

        private bool ProcessPhotoStream(Stream photoStream)
        {
            if (photoStream == null || photoStream.Length == 0)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _pbPhoto,
                    GetString("ErrorOpenPhotoFileFailed"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                return false;
            }
            using (Bitmap photo = new Bitmap(photoStream))
            {
              return ProcessPhoto(photo);
            }
         }

        private bool ProcessPhoto(Bitmap photo)
        {
            Image newPhoto = null;
            try
            {  
                if (photo.Width > MAX_PICTURE_WIDTH || photo.Height > MAX_PICTURE_HEIGHT)
                {
                    // Resize original Image.
                    newPhoto = ImageResizeUtility.GetResizedImage(photo, MAX_PICTURE_WIDTH, MAX_PICTURE_HEIGHT, ImageResizeUtility.ResizeMode.Normal, true);
                }
                else
                {
                    newPhoto = photo;
                }

                if (newPhoto == null)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _pbPhoto,
                        GetString("ErrorOpenPhotoFileFailed"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    return false;
                }
           
                ShowPhotoBitmap((Bitmap)newPhoto);
                _isPhotoModified = true;
                EditTextChangerOnlyInDatabase(null, null);
                // Convert an image to a byte array. 
                Byte[] binaryPhotoData = (Byte[])new ImageConverter().ConvertTo(newPhoto, typeof(Byte[]));
                _binaryPhoto = new BinaryPhoto(binaryPhotoData, ".jpg");
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
            return true;
        }

        private void ShowPhotoBitmap(Bitmap photo)
        {
            try
            {
                if (photo != null)
                {
                    _pbPhoto.Image = ImageResizeUtility.GetResizedImage(photo, _pbPhoto.Width, _pbPhoto.Height, ImageResizeUtility.ResizeMode.Crop, true);
                    _pbPhoto.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                else
                {
                    _pbPhoto.Image = ResourceGlobal.PersonsNew48;
                    _pbPhoto.SizeMode = PictureBoxSizeMode.CenterImage;
                }
             }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void ShowPhoto()
        {
            try
            {
                ShowPhoto(CgpClient.Singleton.MainServerProvider.Persons.GetPhoto(_editingObject.IdPerson));
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void ShowPhoto(BinaryPhoto binaryPhoto)
        {
            _binaryPhoto = binaryPhoto;
           
           if (_binaryPhoto != null)
           {
                using (MemoryStream photoStream = new MemoryStream())
                {
                    photoStream.Write(_binaryPhoto.BinaryData, 0, _binaryPhoto.BinaryData.Length);
                    using (Bitmap photo = new Bitmap(photoStream))
                    {
                        ShowPhotoBitmap(photo);
                    }
                }
           }
           else
           {
               ShowPhotoBitmap(null);
           }
        }

        private void _eName_Leave(object sender, EventArgs e)
        {
            if (_eName.Text.Length > 0)
            {
                if (_eSurname.Text.Length == 0 && _eMiddleName.Text.Length == 0)
                {
                    string[] parts = _eName.Text.Trim().Split(StringConstants.SPACE[0]);

                    if (parts.Length > 1)
                    {
                        int j = 0;
                        string[] clearPart = new string[3];

                        for (int i = 0; i < parts.Length; i++)
                        {
                            string tmp = parts[i].Trim();

                            if (tmp.Length > 0)
                            {
                                clearPart[j++] = tmp;
                                if (j >= 3)
                                    break;
                            }
                        }

                        if (j == 2)
                        {
                            _eName.Text = clearPart[0];
                            _eSurname.Text = clearPart[1];
                        }
                        else
                            if (j == 3)
                            {
                                _eName.Text = clearPart[0];
                                _eMiddleName.Text = clearPart[1];
                                _eSurname.Text = clearPart[2];
                            }
                    }
                }
            }
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.PersonsLocalAlarmInstructionView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.PersonsLocalAlarmInstructionAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

        private void _bPrintCards_Click(object sender, EventArgs e)
        {
            if (_ilbCards.SelectedItemsObjects == null || _ilbCards.SelectedItemsObjects.Count == 0)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _bPrintCards, GetString("NoCardsForPrintSelected"), ControlNotificationSettings.Default);
                _ilbCards.Focus();
                return;
            }

            List<object> selectedObjects = _ilbCards.SelectedItemsObjects;
            List<Card> selectedCards = new List<Card>();
            foreach (object item in selectedObjects)
            {
                selectedCards.Add(item as Card);
            }

            try
            {
                CardPrintForm printForm = new CardPrintForm(selectedCards.ToArray(), _editingObject);
                printForm.Show();
            }
            catch (Exception ex)
            {
                Dialog.Error(GetString("ExceptionOccured") + ": " + ex.Message);
            }
        }

        private void _bCapture_Click(object sender, EventArgs e)
        {
            Exception ex = null;
            if (!CardIdManagementManager.Singleton.RunCaptureDialog(out ex))
            {
                if (ex == null)
                    Dialog.Error(GetString("FailedToOpenIdManagementDialog"));
                else
                    Dialog.Error(GetString("ExceptionOccured") + ": " + ex.Message);
            }
        }

        void ImageCaptureFinished(string result, string resultXML)
        {
            if (result.ToLower() == "success")
            {
                SavePersonsPhoto(resultXML);
            }
            else if (result.ToLower() != "user canceled")
                Dialog.Error(GetString("ExceptionOccured") + ": " + result);
        }

        private void SavePersonsPhoto(string xmlString)
        {
            if (xmlString == null || xmlString == string.Empty)
                return;

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);

            XmlNodeList resultNodes = xml.GetElementsByTagName("result");
            if (resultNodes == null || resultNodes.Count == 0)
                return;

            XmlNode resultNode = resultNodes[0];

            XmlAttribute xmlAttribute = resultNode.Attributes["data"];
            if (xmlAttribute == null)
                return;

            string base64string = xmlAttribute.Value;
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64string)))
            {
                ProcessPhotoStream(stream);
            }
        }

        private void PersonEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CardIdManagementManager.Singleton.ImageCaptureFinished -= new CardIdManagementManager.ImageCaptureFinishedHandler(ImageCaptureFinished);
        }

        private void _bGeneratePersonalCode_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;

                var randomPersonalCode = CgpClient.Singleton.MainServerProvider.Persons.GetRandomPersonalCode(_editingObject.IdPerson);

                if (Dialog.Question(
                    string.Format(
                        GetString("UsedGeneratedPersonalCode"),
                        randomPersonalCode)))
                {
                    _ePersonalCode.Text = randomPersonalCode;
                }
            }
            catch
            {
            }
        }

        public static bool CodeHasWrongLength(int codeLength)
        {
            return codeLength < GeneralOptionsForm.Singleton.MinimalCodeLength
                   || codeLength > GeneralOptionsForm.Singleton.MaximalCodeLength;
        }

        public static bool CodeHasLowSecurityStrength(string code)
        {
            if (code == null
                || code.Length < 2)
            {
                return true;
            }

            if (code[0] == code[1])
            {
                for (var i = 2; i < code.Length; i++)
                {
                    if (code[i - 1] != code[i])
                        return false;
                }
            }
            else if (code[0] == code[1] + 1)
            {
                for (var i = 2; i < code.Length; i++)
                {
                    if (code[i - 1] != code[i] + 1)
                        return false;
                }
            }
            else if (code[0] == code[1] - 1)
            {
                for (var i = 2; i < code.Length; i++)
                {
                    if (code[i - 1] != code[i] - 1)
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}