using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

using SqlUniqueException = Contal.IwQuick.SqlUniqueException;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAccessControlListEditForm :
#if DESIGNER
        Form
#else
 ACgpPluginEditForm<NCASClient, AccessControlList>
#endif
    {
        private const int ALARMAREASCOMBOBOXWIDTH = 110;
        private const int ALARMAREASHEIGHT = 17;
        private const int ALARMAREASLEFTMARGIN = 5;
        private const int ALARMAREASRIGHTMARGIN = 15;
        private const int ALARMAREASSTARTY = 5;
        private const int ALARMAREASLINESPACE = 0;

        TimeZone _actTimeZone;
        ICollection<ACLSetting> _actAclSettings;
        BindingSource _bindingSourceAclSettings;
        ACLSetting _editAclSetting;
        ListOfObjects _actCardReaderObjects;
        readonly IList<ACLSetting> _deletedAclSettings = new List<ACLSetting>();
        private bool _allowEdit = true;
        private AccessControlList _clonedAccessControlList;

        public NCASAccessControlListEditForm(
                AccessControlList cardReader,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                cardReader,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
#if DEBUG
            {
                _tbmCardReader.ButtonPopupMenu.Items.Add("_tsiCreateDebug");
                _tbmCardReader.ButtonPopupMenu.Items[1].Name = "_tsiCreateDebug";
            }
#endif
            InitCGPDataGridView();
            WheelTabContorol = _tcDateSettingsStatus;
            MinimumSize = new Size(Width, Height);

            _eDescription.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            _eDescriptionACLSettings.MouseWheel += ControlMouseWheel;
            SetReferenceEditColors();

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclsSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclsSettingsAdmin)));

                HideDisableTabPageAlarmAreas(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclsAlarmAreasView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclsAlarmAreasAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AclsDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageSettings),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eName.ReadOnly = true;
                }

                if (!view && !admin)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tpSettings);
                    return;
                }

                _tpSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageAlarmAreas(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAlarmAreas),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDateSettingsStatus.TabPages.Remove(_tpAlarmAreas);
                    return;
                }

                _tpAlarmAreas.Enabled = admin;
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
                    _tcDateSettingsStatus.TabPages.Remove(_tpReferencedBy);
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
                    _tcDateSettingsStatus.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = Plugin.ObjectImages;
            _cdgvData.AllwaysRefreshOrder = true;
            _cdgvData.EnabledDeleteButton = false;
            _cdgvData.EnabledInsertButton = false;
            _cdgvData.DataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
            _cdgvData.BeforeGridModified += DataGrid_BeforeGridModified;
            _cdgvData.DataGrid.DragDrop += DataGrid_DragDrop;
            _cdgvData.DataGrid.DragOver += DataGrid_DragOver;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_cdgvData.DataGrid.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            _bEdit_Click(null, null);
        }

        void DataGrid_BeforeGridModified(BindingSource bindingSource)
        {
            var i = 0;
            foreach (ACLSetting aclSetting in bindingSource.List)
            {
                aclSetting.Symbol = _cdgvData.GetDefaultImage(aclSetting.CardReaderObject.GetObjectType().ToString(), i);
                if (aclSetting.Disabled != null)
                {
                    aclSetting.StringDisabled =
                        (aclSetting.Disabled.Value ? LocalizationHelper.GetString("General_True") : LocalizationHelper.GetString("General_False"));
                }
                i++;
            }
        }

        void DataGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        void DataGrid_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddACLSetting(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString("AccessControlListEditFormInsertText");
            }

            AlarmAreasTextsSetPosition();

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        public override void SetValuesInsertFromObj(object obj)
        {
            _clonedAccessControlList = obj as AccessControlList;
            if (_clonedAccessControlList != null)
            {
                if (_clonedAccessControlList.ACLSettings != null && _clonedAccessControlList.ACLSettings.Count > 0)
                {
                    if (_actAclSettings == null)
                        _actAclSettings = new List<ACLSetting>();

                    foreach (var aclSetting in _clonedAccessControlList.ACLSettings)
                    {
                        var actACLSetting = Plugin.MainServerProvider.ACLSettings.GetObjectById(aclSetting.IdACLSetting);
                        if (actACLSetting != null)
                        {
                            var newACLSetting = new ACLSetting();
                            newACLSetting.GuidCardReaderObject = actACLSetting.GuidCardReaderObject;
                            newACLSetting.CardReaderObjectType = actACLSetting.CardReaderObjectType;
                            newACLSetting.CardReaderObject = actACLSetting.CardReaderObject;
                            newACLSetting.Disabled = actACLSetting.Disabled;
                            newACLSetting.TimeZone = actACLSetting.TimeZone;
                            newACLSetting.Description = actACLSetting.Description;

                            _actAclSettings.Add(newACLSetting);
                        }
                    }
                }

                if (_clonedAccessControlList.ACLSettingAAs != null && _clonedAccessControlList.ACLSettingAAs.Count > 0)
                {
                    if (_editingObject.ACLSettingAAs == null)
                        _editingObject.ACLSettingAAs = new HashSet<ACLSettingAA>();

                    foreach (var aclSettingAA in _clonedAccessControlList.ACLSettingAAs)
                    {
                        var actACLSettingAA = Plugin.MainServerProvider.ACLSettingAAs.GetObjectById(aclSettingAA.IdACLSettingAA);
                        if (actACLSettingAA != null)
                        {
                            var newACLSettingAA = new ACLSettingAA();
                            newACLSettingAA.AlarmArea = actACLSettingAA.AlarmArea;
                            newACLSettingAA.AlarmAreaAlarmAcknowledge = actACLSettingAA.AlarmAreaAlarmAcknowledge;
                            newACLSettingAA.AlarmAreaSet = actACLSettingAA.AlarmAreaSet;
                            newACLSettingAA.AlarmAreaUnconditionalSet = actACLSettingAA.AlarmAreaUnconditionalSet;
                            newACLSettingAA.AlarmAreaUnset = actACLSettingAA.AlarmAreaUnset;
                            newACLSettingAA.SensorHandling = actACLSettingAA.SensorHandling;
                            newACLSettingAA.CREventLogHandling = actACLSettingAA.CREventLogHandling;
                            _editingObject.ACLSettingAAs.Add(newACLSettingAA);
                        }
                    }
                }
            }
        }

        protected override void BeforeInsert()
        {
            NCASAccessControlListsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASAccessControlListsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASAccessControlListsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASAccessControlListsForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj = Plugin.MainServerProvider.AccessControlLists.GetObjectForEdit(_editingObject.IdAccessControlList, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.AccessControlLists.GetObjectById(_editingObject.IdAccessControlList);
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
            _actAclSettings = null;
            LoadACLSettings();
            LoadAlarmAreas();
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.AccessControlLists.RenewObjectForEdit(_editingObject.IdAccessControlList, out error);

            if (error != null) 
                throw error;
        }

        protected override void SetValuesInsert()
        {
            LoadACLSettings();
            LoadAlarmAreas();
            _bClone.Enabled = false;
            _bApply.Enabled = false;
        }

        protected override void SetValuesEdit()
        {
            _eName.Text = _editingObject.Name;
            _eDescription.Text = _editingObject.Description;

            LoadACLSettings();
            LoadAlarmAreas();
            SetReferencedBy();
            _bApply.Enabled = false;
        }

        protected override void DisableForm()
        {
            base.DisableForm();

            _allowEdit = false;
            _bCancel.Enabled = true;
            _eFilterNameAA.Enabled = true;
            _cbFilterSetAA.Enabled = true;
            _cbFilterUnsetAA.Enabled = true;
            _cbFilterUnconditionalSetAA.Enabled = true;
            _gbFilterSettings.Enabled = true;
            _bFilterClear.Enabled = true;
            _cbFilterShowOnlySetAA.Enabled = true;
        }

        private void GetActualACLSettings()
        {
            ConnectionLost();

            _actAclSettings = new List<ACLSetting>();

            if (_editingObject.ACLSettings != null)
            {
                foreach (var aclSetting in _editingObject.ACLSettings)
                {
                    var actAclSetting = Plugin.MainServerProvider.ACLSettings.GetObjectById(aclSetting.IdACLSetting);
                    if (actAclSetting != null)
                        _actAclSettings.Add(actAclSetting);
                }
            }
        }

        private void InitBindingSource()
        {
            var lastPosition = 0;
            if (_bindingSourceAclSettings != null && _bindingSourceAclSettings.Count != 0)
                lastPosition = _bindingSourceAclSettings.Position;

            _bindingSourceAclSettings = new BindingSource();
            _bindingSourceAclSettings.DataSource = (_actAclSettings = _actAclSettings.OrderBy(a => a.CardReaderObject.ToString()).ToList());
            if (lastPosition != 0) _bindingSourceAclSettings.Position = lastPosition;
            _bindingSourceAclSettings.AllowNew = false;
        }

        private void LoadACLSettings()
        {
            if (_actAclSettings == null)
            {
                GetActualACLSettings();
            }

            InitBindingSource();
            _cdgvData.ModifyGridView(_bindingSourceAclSettings, ACLSetting.COLUMN_SYMBOL, ACLSetting.COLUMN_CARD_READER_OBJECT,
                ACLSetting.COLUMN_TIMEZONE, ACLSetting.COLUMN_STRING_DISABLED, ACLSetting.COLUMN_DESCRIPTION);

            var cardReaderColumn = _cdgvData.DataGrid.Columns[ACLSetting.COLUMN_CARD_READER_OBJECT];
            if (cardReaderColumn != null)
            {
                cardReaderColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cardReaderColumn.FillWeight = 60;
                cardReaderColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cardReaderColumn.MinimumWidth = 300;
                cardReaderColumn.Width = 300;
            }

            var descriptionColumn = _cdgvData.DataGrid.Columns[ACLSetting.COLUMN_DESCRIPTION];
            if (descriptionColumn != null)
            {
                descriptionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                descriptionColumn.FillWeight = 40;
            }

            if (_editAclSetting == null)
                ClearAclSettingsValues();
        }

        private void ClearAclSettingsValues()
        {
            _actCardReaderObjects = null;
            RefreshCardReaderObject();

            _actTimeZone = null;
            RefreshTimeZone();

            _cbDisabled.Checked = false;
            _eDescriptionACLSettings.Text = string.Empty;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;
                _editingObject.Description = _eDescription.Text;

                if (_actAclSettings != null)
                {
                    if (_editingObject.ACLSettings == null)
                        _editingObject.ACLSettings = new HashSet<ACLSetting>();

                    _editingObject.ACLSettings.Clear();

                    foreach (var aclSetting in _actAclSettings)
                        _editingObject.ACLSettings.Add(aclSetting);
                }

                SaveAlarmAreas();

                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                GetString("ErrorEntryACLName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            var retValue = Plugin.MainServerProvider.AccessControlLists.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedACLName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
            }

            if (_clonedAccessControlList != null)
            {
                var userFolders = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetUserFoldersForObject(_clonedAccessControlList.GetIdString(), _clonedAccessControlList.GetObjectType());
                if (userFolders != null && userFolders.Count > 0)
                {
                    foreach (var userFolderStructure in userFolders)
                    {
                        if (userFolderStructure != null)
                        {
                            var actUserFolderStructure = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.GetObjectForEdit(userFolderStructure.IdUserFoldersStructure, out error);
                            if (userFolderStructure != null)
                            {
                                var userFoldersStructureObject = new UserFoldersStructureObject();
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
                retValue = Plugin.MainServerProvider.AccessControlLists.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = Plugin.MainServerProvider.AccessControlLists.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains("Name"))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                        GetString("ErrorUsedACLName"), CgpClient.Singleton.ClientControlNotificationSettings);
                        _eName.Focus();
                    }
                    else if (error.Message.Contains("GuidCardReaderObject"))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvData.DataGrid,
                        GetString("ErrorUsedACLSettings"), ControlNotificationSettings.Default);
                        _cdgvData.DataGrid.Focus();
                    }
                    else
                    {
                        throw error;
                    }
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.AccessControlLists != null)
                Plugin.MainServerProvider.AccessControlLists.EditEnd(_editingObject);
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            foreach (var aclSetting in _deletedAclSettings)
            {
                Exception error;
                var actAclSetting = Plugin.MainServerProvider.ACLSettings.GetObjectForEdit(aclSetting.IdACLSetting, out error);
                if (actAclSetting != null)
                {
                    actAclSetting.AccessControlList = _editingObject;
                    Plugin.MainServerProvider.ACLSettings.Update(actAclSetting, out error);
                    Plugin.MainServerProvider.ACLSettings.EditEnd(actAclSetting);
                }
            }

            Cancel_Click();
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        private void Modify2()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                var listModObj = new List<IModifyObject>();

                Exception error;
                var listCardReadersFromDatabase =
                    Plugin.MainServerProvider.CardReaders.ListModifyObjects(false, out error);

                if (error != null) throw error;
                listModObj.AddRange(listCardReadersFromDatabase);

                var listAlarmAreasFromDatabase = Plugin.MainServerProvider.AlarmAreas.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listAlarmAreasFromDatabase);

                var listDoorEnvironmentsFromDatabase = Plugin.MainServerProvider.DoorEnvironments.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listDoorEnvironmentsFromDatabase);

                var listDCUsFromDatabase = Plugin.MainServerProvider.DCUs.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listDCUsFromDatabase);

                var listMultiDoorsFromDatabase = Plugin.MainServerProvider.MultiDoors.ListModifyObjects(null, out error);
                if (error != null) throw error;
                
                if (listMultiDoorsFromDatabase != null)
                    listModObj.AddRange(listMultiDoorsFromDatabase);

                var listMultiDoorElementsFromDatabase =
                    Plugin.MainServerProvider.MultiDoorElements.ListModifyObjects(out error);

                if (error != null) throw error;

                if (listMultiDoorElementsFromDatabase != null)
                    listModObj.AddRange(listMultiDoorElementsFromDatabase);

                var listFloorsFromDatabase = Plugin.MainServerProvider.Floors.ListModifyObjects(out error);
                if (error != null) throw error;

                if (listFloorsFromDatabase != null)
                    listModObj.AddRange(listFloorsFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, GetString("NCASAccessControlListEditForm_CardReaderObjects"));

                ListOfObjects outCardReaderObjects = null;
                //enables to create more object at once by enabling multiselection (only in create mode!!!)
                if (_bCreate3.Visible)
                {
                    formAdd.ShowDialogMultiSelect(out outCardReaderObjects);
                    if (outCardReaderObjects != null)
                    {
                        _actCardReaderObjects = outCardReaderObjects;

                    }
                }
                //editing the object
                else
                {
                    object outCardReaderObject;
                    formAdd.ShowDialog(out outCardReaderObject);
                    if (outCardReaderObject != null)
                    {
                        outCardReaderObjects = new ListOfObjects();
                        outCardReaderObjects.Objects.Add(outCardReaderObject);

                    }
                }

                if (outCardReaderObjects != null)
                {
                    _actCardReaderObjects = new ListOfObjects();
                    foreach (var selectedObject in outCardReaderObjects)
                    {
                        var modObj = selectedObject as IModifyObject;
                        if (modObj != null)
                        {
                            AOrmObject newOrmObj = null;
                            switch (modObj.GetOrmObjectType)
                            {
                                case ObjectType.CardReader:
                                    newOrmObj = Plugin.MainServerProvider.CardReaders.GetObjectById(modObj.GetId);
                                    break;
                                case ObjectType.AlarmArea:
                                    newOrmObj = Plugin.MainServerProvider.AlarmAreas.GetObjectById(modObj.GetId);
                                    break;
                                case ObjectType.DoorEnvironment:
                                    newOrmObj = Plugin.MainServerProvider.DoorEnvironments.GetObjectById(modObj.GetId);
                                    break;
                                case ObjectType.DCU:
                                    newOrmObj = Plugin.MainServerProvider.DCUs.GetObjectById(modObj.GetId);
                                    break;
                                case ObjectType.MultiDoor:
                                    newOrmObj = Plugin.MainServerProvider.MultiDoors.GetObjectById(modObj.GetId);
                                    break;
                                case ObjectType.MultiDoorElement:
                                    newOrmObj = Plugin.MainServerProvider.MultiDoorElements.GetObjectById(modObj.GetId);
                                    break;
                                case ObjectType.Floor:
                                    newOrmObj = Plugin.MainServerProvider.Floors.GetObjectById(modObj.GetId);
                                    break;
                            }
                            if (newOrmObj != null)
                            {
                                _actCardReaderObjects.Objects.Add(newOrmObj);
                                //(this.Plugin as NCASClient).AddToRecentList(newOrmObj);
                            }
                        }
                    }
                    RefreshCardReaderObject();
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(ex);
            }
        }

        private void RefreshCardReaderObject()
        {
            if (_actCardReaderObjects != null)
            {
                _tbmCardReader.Text = _actCardReaderObjects.ToString();
                _tbmCardReader.TextImage = Plugin.GetImageForListOfObject(_actCardReaderObjects);
            }
            else
            {
                _tbmCardReader.Text = string.Empty;
            }
        }

        private void DoAfterCreatedCR(object newCR)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCreatedCR), newCR);
            }
            else
            {
                if (newCR is CardReader)
                {
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(newCR as CardReader);
                    RefreshCardReaderObject();
                }
            }
        }

        private void Modify()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                var listModObj = new List<IModifyObject>();

                var listTimeZonesFromDatabase = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listTimeZonesFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, CgpClient.Singleton.LocalizationHelper.GetString("TimeZonesFormTimeZonesForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    _actTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outModObj.GetId);
                    RefreshTimeZone();
                    Plugin.AddToRecentList(_actTimeZone);
                }
            }
            catch
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(error);
            }
        }

        private void RefreshTimeZone()
        {
            if (_actTimeZone != null)
            {
                _tbmTimeZone.Text = _actTimeZone.ToString();
                _tbmTimeZone.TextImage = Plugin.GetImageForAOrmObject(_actTimeZone);
            }
            else
                _tbmTimeZone.Text = string.Empty;
        }

        private void DoAfterCreatedTZ(object newTZ)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCreatedTZ), newTZ);
            }
            else
            {
                if (newTZ is TimeZone)
                {
                    _actTimeZone = newTZ as TimeZone;
                    RefreshTimeZone();
                }
            }
        }

        private bool ControlValuesAclSetting()
        {
            if (_actCardReaderObjects == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader.ImageTextBox,
                GetString("ErrorEntryCardReader"), ControlNotificationSettings.Default);
                _tbmCardReader.Focus();
                return false;
            }

            foreach (var obj in _actCardReaderObjects.Objects)
            {
                if (!ACLSettingControlAddedCardReaderTimeZone(obj as AOrmObject, _actTimeZone))
                    return false;
            }

            return true;
        }

        private bool ACLSettingControlAddedCardReaderTimeZone(AOrmObject cardReaderObject, TimeZone timeZone)
        {
            if (_actAclSettings != null)
            {
                foreach (var aclSetting in _actAclSettings)
                {
                    var objectType = ObjectType.NotSupport;
                    var guid = Guid.Empty;

                    switch (cardReaderObject.GetObjectType())
                    {
                        case ObjectType.CardReader:
                            objectType = ObjectType.CardReader;
                            guid = (cardReaderObject as CardReader).IdCardReader;
                            break;
                        case ObjectType.AlarmArea:
                            objectType = ObjectType.AlarmArea;
                            guid = (cardReaderObject as AlarmArea).IdAlarmArea;
                            break;
                        case ObjectType.DoorEnvironment:
                            objectType = ObjectType.DoorEnvironment;
                            guid = (cardReaderObject as DoorEnvironment).IdDoorEnvironment;
                            break;
                        case ObjectType.DCU:
                            objectType = ObjectType.DCU;
                            guid = (cardReaderObject as DCU).IdDCU;
                            break;
                        case ObjectType.MultiDoor:
                            objectType = ObjectType.MultiDoor;
                            guid = (cardReaderObject as MultiDoor).IdMultiDoor;
                            break;
                        case ObjectType.MultiDoorElement:
                            objectType = ObjectType.MultiDoorElement;
                            guid = (cardReaderObject as MultiDoorElement).IdMultiDoorElement;
                            break;
                        case ObjectType.Floor:
                            objectType = ObjectType.Floor;
                            guid = (cardReaderObject as Floor).IdFloor;
                            break;
                    }

                    if (_editAclSetting != aclSetting
                        && aclSetting.CardReaderObjectType == (byte)objectType
                        && aclSetting.GuidCardReaderObject == guid
                        && ((timeZone != null && timeZone.Compare(aclSetting.TimeZone) || timeZone == aclSetting.TimeZone)))
                    {
                        Dialog.Error(GetString("ErrorUsedCardReaderAndTimeZone"));
                        return false;
                    }
                }
            }

            if (cardReaderObject is DoorEnvironment)
            {
                var doorEnvironment = Plugin.MainServerProvider.DoorEnvironments.GetObjectById((cardReaderObject as DoorEnvironment).IdDoorEnvironment);

                if (doorEnvironment != null)
                {
                    if (doorEnvironment.CardReaderInternal == null && doorEnvironment.CardReaderExternal == null)
                    {
                        Dialog.Error(GetString("ErrorDoorEnvironmentHasNotCardReaders"));
                        return false;
                    }
                }
            }
            else if (cardReaderObject is AlarmArea)
            {
                var alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById((cardReaderObject as AlarmArea).IdAlarmArea);

                if (alarmArea != null)
                {
                    if (alarmArea.AACardReaders == null || alarmArea.AACardReaders.Count == 0)
                    {
                        Dialog.Error(GetString("ErrorAlarmAreaHasNotCardReaders"));
                        return false;
                    }
                }
            }
            else if (cardReaderObject is DCU)
            {
                var dcu = Plugin.MainServerProvider.DCUs.GetObjectById((cardReaderObject as DCU).IdDCU);

                if (dcu != null)
                {
                    if (dcu.CardReaders == null || dcu.CardReaders.Count == 0)
                    {
                        Dialog.Error(GetString("ErrorDCUHasNotCardReaders"));
                        return false;
                    }
                }
            }

            return true;
        }

        private void GetValuesAclSetting(ACLSetting aclSetting, AOrmObject cardReaderObject)
        {
            aclSetting.CardReaderObject = DbsSupport.GetTableObject(cardReaderObject.GetObjectType(), (Guid)cardReaderObject.GetId());

            switch (cardReaderObject.GetObjectType())
            {
                case ObjectType.CardReader:
                    aclSetting.CardReaderObjectType = (byte)ObjectType.CardReader;
                    aclSetting.GuidCardReaderObject = (cardReaderObject as CardReader).IdCardReader;
                    break;
                case ObjectType.DoorEnvironment:
                    aclSetting.CardReaderObjectType = (byte)ObjectType.DoorEnvironment;
                    aclSetting.GuidCardReaderObject = (cardReaderObject as DoorEnvironment).IdDoorEnvironment;
                    break;
                case ObjectType.AlarmArea:
                    aclSetting.CardReaderObjectType = (byte)ObjectType.AlarmArea;
                    aclSetting.GuidCardReaderObject = (cardReaderObject as AlarmArea).IdAlarmArea;
                    break;
                case ObjectType.DCU:
                    aclSetting.CardReaderObjectType = (byte)ObjectType.DCU;
                    aclSetting.GuidCardReaderObject = (cardReaderObject as DCU).IdDCU;
                    break;
                case ObjectType.MultiDoor:
                    aclSetting.CardReaderObjectType = (byte)ObjectType.MultiDoor;
                    aclSetting.GuidCardReaderObject = (cardReaderObject as MultiDoor).IdMultiDoor;
                    break;
                case ObjectType.MultiDoorElement:
                    aclSetting.CardReaderObjectType = (byte)ObjectType.MultiDoorElement;
                    aclSetting.GuidCardReaderObject = (cardReaderObject as MultiDoorElement).IdMultiDoorElement;
                    break;
                case ObjectType.Floor:
                    aclSetting.CardReaderObjectType = (byte)ObjectType.Floor;
                    aclSetting.GuidCardReaderObject = (cardReaderObject as Floor).IdFloor;
                    break;
            }

            aclSetting.TimeZone = _actTimeZone;
            aclSetting.Disabled = _cbDisabled.Checked;
            aclSetting.Description = _eDescriptionACLSettings.Text;
            aclSetting.AccessControlList = _editingObject;
        }

        private bool GetValuesAclSetting(out IList<ACLSetting> aclSettings)
        {
            aclSettings = null;

            try
            {
                if (!ControlValuesAclSetting())
                    return false;

                aclSettings = new List<ACLSetting>();

                foreach (var obj in _actCardReaderObjects.Objects)
                {
                    var aclSetting = new ACLSetting();
                    GetValuesAclSetting(aclSetting, obj as AOrmObject);

                    aclSettings.Add(aclSetting);
                }

                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        private bool GetValuesAclSetting(ACLSetting aclSetting)
        {
            try
            {
                if (!ControlValuesAclSetting())
                    return false;

                if (_actCardReaderObjects.Objects.Count == 0)
                    return false;

                GetValuesAclSetting(aclSetting, _actCardReaderObjects.Objects[0] as AOrmObject);
                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        private void SetValuesAclSetting(ACLSetting aclSetting)
        {
            _actCardReaderObjects = new ListOfObjects();
            if (aclSetting.CardReaderObjectType == (byte)ObjectType.CardReader)
            {
                _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.CardReaders.GetObjectById(aclSetting.GuidCardReaderObject));
            }
            else if (aclSetting.CardReaderObjectType == (byte)ObjectType.AlarmArea)
            {
                _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.AlarmAreas.GetObjectById(aclSetting.GuidCardReaderObject));
            }
            else if (aclSetting.CardReaderObjectType == (byte)ObjectType.DoorEnvironment)
            {
                _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.DoorEnvironments.GetObjectById(aclSetting.GuidCardReaderObject));
            }
            else if (aclSetting.CardReaderObjectType == (byte)ObjectType.DCU)
            {
                _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.DCUs.GetObjectById(aclSetting.GuidCardReaderObject));
            }
            else if (aclSetting.CardReaderObjectType == (byte)ObjectType.MultiDoor)
            {
                _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.MultiDoors.GetObjectById(aclSetting.GuidCardReaderObject));
            }
            else if (aclSetting.CardReaderObjectType == (byte)ObjectType.MultiDoorElement)
            {
                _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.MultiDoorElements.GetObjectById(aclSetting.GuidCardReaderObject));
            }
            else if (aclSetting.CardReaderObjectType == (byte)ObjectType.Floor)
            {
                _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.Floors.GetObjectById(aclSetting.GuidCardReaderObject));
            }

            RefreshCardReaderObject();

            _actTimeZone = aclSetting.TimeZone;
            RefreshTimeZone();

            if (aclSetting.Disabled != null && aclSetting.Disabled.Value)
                _cbDisabled.Checked = true;
            else
                _cbDisabled.Checked = false;

            _eDescriptionACLSettings.Text = aclSetting.Description;
        }

        private void _bCreate3_Click(object sender, EventArgs e)
        {
            IList<ACLSetting> aclSettings;

            if (!GetValuesAclSetting(out aclSettings))
                return;

            if (aclSettings != null)
            {
                foreach (var aclSetting in aclSettings)
                    _actAclSettings.Add(aclSetting);

                EditTextChanger(null, null);
                LoadACLSettings();
            }
        }

        private void _bUpdate_Click(object sender, EventArgs e)
        {
            if (!GetValuesAclSetting(_editAclSetting))
                return;

            _editAclSetting = null;
            _bCreate3.Visible = true;
            _bUpdate.Visible = false;
            _bCancelEdit.Visible = false;

            EditTextChanger(null, null);
            LoadACLSettings();
        }

        private void _bCancelEdit_Click(object sender, EventArgs e)
        {
            _editAclSetting = null;
            _bCreate3.Visible = true;
            _bUpdate.Visible = false;
            _bCancelEdit.Visible = false;

            ClearAclSettingsValues();
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (!_allowEdit || _bindingSourceAclSettings == null || _bindingSourceAclSettings.Count == 0) return;

            _editAclSetting = (ACLSetting)_bindingSourceAclSettings.List[_bindingSourceAclSettings.Position];
            SetValuesAclSetting(_editAclSetting);

            _bCreate3.Visible = false;
            _bUpdate.Visible = true;
            _bCancelEdit.Visible = true;
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_bindingSourceAclSettings == null || _bindingSourceAclSettings.Count == 0) return;

            if (Dialog.Question(GetString("QuestionDeleteConfirm")))
            {
                var aclSettings =
                    new LinkedList<ACLSetting>(
                        _cdgvData.DataGrid.SelectedRows.Cast<DataGridViewRow>()
                            .Select(selectedItem => (ACLSetting) _bindingSourceAclSettings.List[selectedItem.Index]));

                foreach (ACLSetting aclSetting in aclSettings)
                {
                    _actAclSettings.Remove(aclSetting);
                    try
                    {
                        if (aclSetting.IdACLSetting != Guid.Empty)
                        {
                            Exception error;
                            ACLSetting aclSettingForDelete = Plugin.MainServerProvider.ACLSettings.GetObjectForEdit(
                                aclSetting.IdACLSetting, out error);
                            if (aclSettingForDelete != null)
                            {
                                aclSetting.AccessControlList = null;
                                Plugin.MainServerProvider.ACLSettings.Update(aclSettingForDelete, out error);
                                Plugin.MainServerProvider.ACLSettings.EditEnd(aclSettingForDelete);
                                _deletedAclSettings.Add(aclSettingForDelete);
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }
            }

            EditTextChanger(null, null);
            LoadACLSettings();
        }

        private void _dgValues_DoubleClick(object sender, EventArgs e)
        {
            _bEdit_Click(null, null);
        }

        private void _tbmCardReader_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmCardReader_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddCardReader(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddCardReader(object newCardReaderObject)
        {
            try
            {
                if (newCardReaderObject is CardReader ||
                    newCardReaderObject is AlarmArea ||
                    newCardReaderObject is DoorEnvironment ||
                    newCardReaderObject is DCU ||
                    newCardReaderObject is MultiDoor ||
                    newCardReaderObject is MultiDoorElement ||
                    newCardReaderObject is Floor)
                {
                    var cardReader = newCardReaderObject as CardReader;

                    if (cardReader != null &&
                        Plugin.MainServerProvider.MultiDoors.IsCardReaderUsedInMultiDoor(
                            cardReader.IdCardReader))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _tbmCardReader.ImageTextBox,
                            GetString("ErrorCanNotBeUsedCardReaderAssignedToMultiDoor"),
                            ControlNotificationSettings.Default);

                        return;
                    }

                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(newCardReaderObject as AOrmObject);
                    RefreshCardReaderObject();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _tbmCardReader_DoubleClick(object sender, EventArgs e)
        {
            if (_actCardReaderObjects != null && _actCardReaderObjects.Count == 1)
            {
                if ((_actCardReaderObjects[0] as AOrmObject) is CardReader)
                    NCASCardReadersForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as CardReader);
                else if ((_actCardReaderObjects[0] as AOrmObject) is AlarmArea)
                    NCASAlarmAreasForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as AlarmArea);
                else if ((_actCardReaderObjects[0] as AOrmObject) is DoorEnvironment)
                    NCASDoorEnvironmentsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as DoorEnvironment);
                else if ((_actCardReaderObjects[0] as AOrmObject) is DCU)
                    NCASDCUsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as DCU);
                else if ((_actCardReaderObjects[0] as AOrmObject) is MultiDoor)
                    NCASMultiDoorsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as MultiDoor);
                else if ((_actCardReaderObjects[0] as AOrmObject) is MultiDoorElement)
                    NCASMultiDoorElementsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as MultiDoorElement);
                else if ((_actCardReaderObjects[0] as AOrmObject) is Floor)
                    NCASFloorsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as Floor);
            }
        }

        private void _tbmTimeZone_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmTimeZone_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddTimeZone(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddTimeZone(object newTimeZone)
        {
            try
            {
                if (newTimeZone.GetType() == typeof(TimeZone))
                {
                    var timeZone = newTimeZone as TimeZone;
                    _actTimeZone = timeZone;
                    RefreshTimeZone();
                    Plugin.AddToRecentList(newTimeZone);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmTimeZone.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _tbmTimeZone_DoubleClick(object sender, EventArgs e)
        {
            if (_actTimeZone != null)
                TimeZonesForm.Singleton.OpenEditForm(_actTimeZone);
        }

        private void AlarmAreasTextsSetPosition()
        {
            _lName2.Left = ALARMAREASLEFTMARGIN;
            _lAlarmAreasSet1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (6 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasSet1.Width) / 2;
            _lAlarmAreasSet.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (6 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasSet.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterSetAA.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (6 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterSetAA.Width) / 2 - _gbFilterSettings.Left;

            _lAlarmAreasUnset1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (5 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasUnset1.Width) / 2;
            _lAlarmAreasUnset.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (5 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasUnset.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterUnsetAA.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (5 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterUnsetAA.Width) / 2 - _gbFilterSettings.Left;

            _lAlarmAreasUnconditionalSet1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (4 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasUnconditionalSet1.Width) / 2;
            _lAlarmAreasUnconditionalSet.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (4 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasUnconditionalSet.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterUnconditionalSetAA.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (4 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterUnconditionalSetAA.Width) / 2 - _gbFilterSettings.Left;

            _lSensorHandling.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (3 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lSensorHandling.Width) / 2;
            _lSensorHandling1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (3 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lSensorHandling1.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterSensorHandling.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (3 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterSensorHandling.Width) / 2 - _gbFilterSettings.Left;

            _lAlarmAreasAlarmAcknowledge1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasAlarmAcknowledge1.Width) / 2;
            _lAlarmAreasAlarmAcknowledge.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasAlarmAcknowledge.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterAlarmAcknowledgeAA.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterAlarmAcknowledgeAA.Width) / 2 - _gbFilterSettings.Left;

            _lCREventlogHandling.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lCREventlogHandling.Width) / 2;
            _lCREventlogHandling1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lCREventlogHandling1.Width) / 2 - _gbFilterSettings.Left;
            _cbCREventLogHandling.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbCREventLogHandling.Width) / 2 - _gbFilterSettings.Left;

            _lTimeBuying.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (1 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lTimeBuying.Width) / 2;
            _lTimeBuying1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (1 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lTimeBuying1.Width) / 2 - _gbFilterSettings.Left;
            _cbTimeBuying.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (1 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbTimeBuying.Width) / 2 - _gbFilterSettings.Left;
        }

        private void LoadAlarmAreas()
        {
            _panelLinesAlarmAreas.AutoScrollPosition = new Point(0, 0);

            var top = ALARMAREASSTARTY;
            if (_alarmAreasComboBox.Count > 0)
            {
                foreach (var kvp in _alarmAreasComboBox)
                {
                    SetAlarmAreaACLSettings(kvp.Value, ref top);
                }
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;

                Exception error;
                var alarmAreas = Plugin.MainServerProvider.AlarmAreas.SelectByCriteria(null, out error);

                if (error != null || alarmAreas == null)
                    return;

                foreach (var alarmArea in alarmAreas)
                {
                    var alarmAreaACLSetting = CreateAlarmAreaLine(alarmArea);
                    SetAlarmAreaACLSettings(alarmAreaACLSetting, ref top);
                }

                if (_editingObject.ACLSettingAAs != null)
                {
                    foreach (var aclSettingAA in _editingObject.ACLSettingAAs)
                    {
                        if (aclSettingAA != null)
                        {
                            if (_alarmAreasComboBox.ContainsKey(aclSettingAA.AlarmArea.IdAlarmArea))
                            {
                                _alarmAreasComboBox[aclSettingAA.AlarmArea.IdAlarmArea].Set(
                                    aclSettingAA.AlarmAreaSet, 
                                    aclSettingAA.AlarmAreaUnset,
                                    aclSettingAA.AlarmAreaUnconditionalSet, 
                                    aclSettingAA.AlarmAreaAlarmAcknowledge, 
                                    aclSettingAA.SensorHandling, 
                                    aclSettingAA.CREventLogHandling,
                                    aclSettingAA.AlarmAreaTimeBuying);
                            }
                        }
                    }
                }
            }
        }

        private void SetAlarmAreaACLSettings(AlaramAreaACLSettings alarmAreaACLSetting, ref int top)
        {
            bool bShow = 
                _eFilterNameAA.Text == string.Empty || 
                alarmAreaACLSetting.GetText().ToUpper()
                    .Contains(_eFilterNameAA.Text.ToUpper());

            if (bShow && _cbFilterSetAA.Checked && !alarmAreaACLSetting.GetAlarmAreaSet())
                bShow = false;

            if (bShow && _cbFilterUnsetAA.Checked && !alarmAreaACLSetting.GetAlarmAreaUnset())
                bShow = false;

            if (bShow && _cbFilterUnconditionalSetAA.Checked && !alarmAreaACLSetting.GetAlarmAreaUnconditionalSet())
                bShow = false;

            //if (bShow && _cbFilterAlarmAcknowledgeAA.Checked && !alarmAreaACLSetting.GetAlarmAreaAlaramAcknowledge())
            //    bShow = false;

            if (bShow && _cbFilterShowOnlySetAA.Checked && !alarmAreaACLSetting.IsSet())
                bShow = false;

            if (bShow && _cbFilterSensorHandling.Checked && !alarmAreaACLSetting.GetSensorHandling())
                bShow = false;

            if (bShow && _cbCREventLogHandling.Checked && !alarmAreaACLSetting.GetCREventLogHandling())
                bShow = false;

            if (bShow && _cbTimeBuying.Checked && !alarmAreaACLSetting.GetAlarmAreaTimeBuying())
                bShow = false;

            if (bShow)
            {
                alarmAreaACLSetting.SetTop(top);
                alarmAreaACLSetting.SetVisible(true);
                top += ALARMAREASHEIGHT + ALARMAREASLINESPACE;
            }
            else
            {
                alarmAreaACLSetting.SetVisible(false);
            }
        }

        private readonly Dictionary<Guid, AlaramAreaACLSettings> _alarmAreasComboBox = new Dictionary<Guid, AlaramAreaACLSettings>();
        private AlaramAreaACLSettings CreateAlarmAreaLine(AlarmArea alarmArea)
        {
            if (alarmArea != null)
            {
                var panel = new Panel();
                panel.Parent = _panelLinesAlarmAreas;
                panel.Left = ALARMAREASLEFTMARGIN;
                panel.Top = ALARMAREASSTARTY;
                panel.Height = ALARMAREASHEIGHT;
                panel.Width = _panelLinesAlarmAreas.Width - ALARMAREASLEFTMARGIN - ALARMAREASRIGHTMARGIN;
                panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                panel.MouseEnter += AlarmAreasPanelMouseEnter;
                panel.MouseLeave += AlarmAreasPanelMouseLeave;

                var name = 
                    new Label
                    {
                        Parent = panel,
                        Text = alarmArea.ToString(),
                        Top = 0,
                        Left = ALARMAREASLEFTMARGIN,
                        Height = ALARMAREASHEIGHT,
                        Width = panel.Width - (7*ALARMAREASCOMBOBOXWIDTH),
                        TextAlign = ContentAlignment.MiddleLeft,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Tag = alarmArea.IdAlarmArea
                    };

                name.MouseLeave += AlarmAreasMouseLeave;
                name.MouseEnter += AlarmAreasMouseEnter;
                name.DoubleClick += AlarmAreasNameDoubleClick;

                var cbSet = 
                    new CheckBox
                    {
                        Parent = panel,
                        Top = 0,
                        Left = panel.Width - (6*ALARMAREASCOMBOBOXWIDTH),
                        Height = ALARMAREASHEIGHT,
                        Width = ALARMAREASCOMBOBOXWIDTH,
                        CheckAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };

                cbSet.MouseLeave += AlarmAreasMouseLeave;
                cbSet.MouseEnter += AlarmAreasMouseEnter;
                cbSet.CheckedChanged += EditTextChanger;

                var cbUnset = 
                    new CheckBox
                    {
                        Parent = panel,
                        Top = 0,
                        Left = panel.Width - (5*ALARMAREASCOMBOBOXWIDTH),
                        Height = ALARMAREASHEIGHT,
                        Width = ALARMAREASCOMBOBOXWIDTH,
                        CheckAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };

                cbUnset.MouseLeave += AlarmAreasMouseLeave;
                cbUnset.MouseEnter += AlarmAreasMouseEnter;
                cbUnset.CheckedChanged += EditTextChanger;

                var cbUnconditionalSet = 
                    new CheckBox
                    {
                        Parent = panel,
                        Top = 0,
                        Left = panel.Width - (4*ALARMAREASCOMBOBOXWIDTH),
                        Height = ALARMAREASHEIGHT,
                        Width = ALARMAREASCOMBOBOXWIDTH,
                        CheckAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };

                cbUnconditionalSet.MouseLeave += AlarmAreasMouseLeave;
                cbUnconditionalSet.MouseEnter += AlarmAreasMouseEnter;
                cbUnconditionalSet.CheckedChanged += EditTextChanger;

                var cbSensorHandling = 
                    new CheckBox
                    {
                        Parent = panel,
                        Top = 0,
                        Left = panel.Width - (3*ALARMAREASCOMBOBOXWIDTH),
                        Height = ALARMAREASHEIGHT,
                        Width = ALARMAREASCOMBOBOXWIDTH,
                        CheckAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };

                cbSensorHandling.MouseLeave += AlarmAreasMouseLeave;
                cbSensorHandling.MouseEnter += AlarmAreasMouseEnter;
                cbSensorHandling.CheckedChanged += EditTextChanger;

                var cbCEEventLogHandling = 
                    new CheckBox
                    {
                        Parent = panel,
                        Top = 0,
                        Left = panel.Width - (2*ALARMAREASCOMBOBOXWIDTH),
                        Height = ALARMAREASHEIGHT,
                        Width = ALARMAREASCOMBOBOXWIDTH,
                        CheckAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };

                cbCEEventLogHandling.MouseLeave += AlarmAreasMouseLeave;
                cbCEEventLogHandling.MouseEnter += AlarmAreasMouseEnter;
                cbCEEventLogHandling.CheckedChanged += EditTextChanger;

                var cbTimeBuying = 
                    new CheckBox
                    {
                        Parent = panel,
                        Top = 0,
                        Left = panel.Width - (1*ALARMAREASCOMBOBOXWIDTH),
                        Height = ALARMAREASHEIGHT,
                        Width = ALARMAREASCOMBOBOXWIDTH,
                        CheckAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };

                cbTimeBuying.MouseLeave += AlarmAreasMouseLeave;
                cbTimeBuying.MouseEnter += AlarmAreasMouseEnter;
                cbTimeBuying.CheckedChanged += EditTextChanger;
                
                var alarmAreaACLSetting = 
                    new AlaramAreaACLSettings(
                        panel, 
                        name, 
                        cbSet, 
                        cbUnset, 
                        cbUnconditionalSet, 
                        cbSensorHandling, 
                        cbCEEventLogHandling, 
                        cbTimeBuying);//, cbAlarmAcknowledge);

                if (!_alarmAreasComboBox.ContainsKey(alarmArea.IdAlarmArea))
                    _alarmAreasComboBox.Add(
                        alarmArea.IdAlarmArea, 
                        alarmAreaACLSetting);
                else
                    _alarmAreasComboBox[alarmArea.IdAlarmArea] = alarmAreaACLSetting;

                return alarmAreaACLSetting;
            }

            return null;
        }

        void AlarmAreasMouseEnter(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                AlarmAreasPanelMouseEnter(control.Parent, e);
            }
        }

        void AlarmAreasMouseLeave(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                AlarmAreasPanelMouseLeave(control.Parent, e);
            }
        }

        void AlarmAreasPanelMouseLeave(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                if (ActiveForm != this || MousePosition.X <= panel.PointToScreen(new Point(0, 0)).X || MousePosition.Y <= panel.PointToScreen(new Point(0, 0)).Y ||
                    MousePosition.X >= panel.PointToScreen(new Point(panel.Width, panel.Height)).X || MousePosition.Y >= panel.PointToScreen(new Point(panel.Width, panel.Height)).Y)
                {
                    foreach (Control control in panel.Controls)
                        control.BackColor = Color.White;
                }
            }
        }

        void AlarmAreasPanelMouseEnter(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                foreach (Control control in panel.Controls)
                    control.BackColor = Color.LightBlue;
            }
        }

        void AlarmAreasNameDoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            var label = sender as Label;

            if (label != null)
            {
                if (label.Tag is Guid)
                {
                    var guidAlarmArea = (Guid)label.Tag;

                    var alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById(guidAlarmArea);

                    if (alarmArea != null)
                    {
                        NCASAlarmAreasForm.Singleton.OpenEditForm(alarmArea);
                    }
                }
            }
        }

        public void SaveAlarmAreas()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_editingObject.ACLSettingAAs != null)
                _editingObject.ACLSettingAAs.Clear();
            else
                _editingObject.ACLSettingAAs = new HashSet<ACLSettingAA>();

            if (_alarmAreasComboBox != null)
            {
                foreach (var kvp in _alarmAreasComboBox)
                {
                    var alarmAreaACLSetting = kvp.Value;

                    if (alarmAreaACLSetting != null && alarmAreaACLSetting.IsSet())
                    {
                        var aclSettingAA = new ACLSettingAA
                        {
                            AccessControlList = _editingObject,
                            AlarmAreaSet = alarmAreaACLSetting.GetAlarmAreaSet(),
                            AlarmAreaUnset = alarmAreaACLSetting.GetAlarmAreaUnset(),
                            AlarmAreaUnconditionalSet =
                                alarmAreaACLSetting.GetAlarmAreaUnconditionalSet(),
                            SensorHandling = alarmAreaACLSetting.GetSensorHandling(),
                            CREventLogHandling = alarmAreaACLSetting.GetCREventLogHandling(),
                            AlarmAreaTimeBuying = alarmAreaACLSetting.GetAlarmAreaTimeBuying(),
                            AlarmAreaAlarmAcknowledge = false
                        };

                        var alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById(kvp.Key);
                        if (alarmArea != null)
                        {
                            aclSettingAA.AlarmArea = alarmArea;
                            _editingObject.ACLSettingAAs.Add(aclSettingAA);
                        }
                    }
                }
            }
        }

        private void AlarmAreaFilterChanged(object sender, EventArgs e)
        {
            LoadAlarmAreas();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _eFilterNameAA.Text = string.Empty;
            _cbFilterSetAA.Checked = false;
            _cbFilterUnsetAA.Checked = false;
            _cbFilterUnconditionalSetAA.Checked = false;
            _cbFilterAlarmAcknowledgeAA.Checked = false;
            _cbFilterShowOnlySetAA.Checked = false;
            _cbFilterSensorHandling.Checked = false;
            LoadAlarmAreas();
        }

        private void _panelLinesAlarmAreas_Resize(object sender, EventArgs e)
        {
            AlarmAreasTextsSetPosition();
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
                AddACLSetting(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddACLSetting(object newCardReaderObject)
        {
            try
            {
                if (newCardReaderObject is CardReader ||
                    newCardReaderObject is AlarmArea ||
                    newCardReaderObject is DoorEnvironment ||
                    newCardReaderObject is DCU ||
                    newCardReaderObject is MultiDoor ||
                    newCardReaderObject is MultiDoorElement ||
                    newCardReaderObject is Floor)
                {
                    var cardReaderObject = newCardReaderObject as AOrmObject;
                    if (ACLSettingControlAddedCardReaderTimeZone(cardReaderObject, null))
                    {
                        var aclSetting = new ACLSetting();
                        aclSetting.AccessControlList = _editingObject;
                        aclSetting.CardReaderObject = cardReaderObject;

                        if (cardReaderObject is CardReader)
                        {
                            if (Plugin.MainServerProvider.MultiDoors.IsCardReaderUsedInMultiDoor(
                                (Guid) cardReaderObject.GetId()))
                            {
                                ControlNotification.Singleton.Error(
                                    NotificationPriority.JustOne,
                                    _cdgvData.DataGrid,
                                    GetString("ErrorCanNotBeUsedCardReaderAssignedToMultiDoor"),
                                    ControlNotificationSettings.Default);

                                return;
                            }

                            aclSetting.CardReaderObjectType = (byte)ObjectType.CardReader;
                            aclSetting.GuidCardReaderObject = (cardReaderObject as CardReader).IdCardReader;
                        }
                        else if (cardReaderObject is AlarmArea)
                        {
                            aclSetting.CardReaderObjectType = (byte)ObjectType.AlarmArea;
                            aclSetting.GuidCardReaderObject = (cardReaderObject as AlarmArea).IdAlarmArea;
                        }
                        else if (cardReaderObject is DoorEnvironment)
                        {
                            aclSetting.CardReaderObjectType = (byte)ObjectType.DoorEnvironment;
                            aclSetting.GuidCardReaderObject = (cardReaderObject as DoorEnvironment).IdDoorEnvironment;
                        }
                        else if (cardReaderObject is DCU)
                        {
                            aclSetting.CardReaderObjectType = (byte)ObjectType.DCU;
                            aclSetting.GuidCardReaderObject = (cardReaderObject as DCU).IdDCU;
                        }
                        else if (cardReaderObject is MultiDoor)
                        {
                            aclSetting.CardReaderObjectType = (byte)ObjectType.MultiDoor;
                            aclSetting.GuidCardReaderObject = (cardReaderObject as MultiDoor).IdMultiDoor;
                        }
                        else if (cardReaderObject is MultiDoorElement)
                        {
                            aclSetting.CardReaderObjectType = (byte)ObjectType.MultiDoorElement;
                            aclSetting.GuidCardReaderObject = (cardReaderObject as MultiDoorElement).IdMultiDoorElement;
                        }
                        else if (cardReaderObject is Floor)
                        {
                            aclSetting.CardReaderObjectType = (byte)ObjectType.Floor;
                            aclSetting.GuidCardReaderObject = (cardReaderObject as Floor).IdFloor;
                        }

                        aclSetting.Disabled = false;
                        aclSetting.TimeZone = null;
                        _actAclSettings.Add(aclSetting);
                        EditTextChanger(null, null);
                        LoadACLSettings();
                        Plugin.AddToRecentList(newCardReaderObject);
                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvData.DataGrid,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, NCASClient.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return Plugin.MainServerProvider.AccessControlLists.
                GetReferencedObjects(_editingObject.IdAccessControlList, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_clonedAccessControlList == null)
                ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            if (_clonedAccessControlList != null)
                ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _clonedAccessControlList);
            else
                ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _tbmTimeZone_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                Modify();
            }
            else if (item.Name == "_tsiRemove")
            {
                if (Dialog.Question(GetString("QuestionRemoveTimeZone")))
                {
                    _actTimeZone = null;
                    RefreshTimeZone();
                }
            }
            else if (item.Name == "_tsiCreate")
            {
                var timeZone = new TimeZone();
                TimeZonesForm.Singleton.OpenInsertFromEdit(ref timeZone, DoAfterCreatedTZ);
            }
        }

        private void _tbmCardReader_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                Modify2();
            }
            else if (item.Name == "_tsiCreateDebug")
            {
                var cardReader = new CardReader();
                cardReader.EnableParentInFullName = Plugin.MainServerProvider.GetEnableParentInFullName();
                NCASCardReadersForm.Singleton.OpenInsertFromEdit(ref cardReader, DoAfterCreatedCR);
            }
        }

        private void _bClone_Click(object sender, EventArgs e)
        {
            NCASAccessControlListsForm.Singleton.InsertWithData(_editingObject);
        }

        protected override void AfterDockUndock()
        {

        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                _bApply.Enabled = false;
            }
        }
    }

    public class AlaramAreaACLSettings
    {
        private readonly Panel _panel;
        private readonly Label _lName;
        private readonly CheckBox _cbSet;
        private readonly CheckBox _cbUnset;
        private readonly CheckBox _cbUnconditionalSet;
        private readonly CheckBox _cbSensorHandling;
        private readonly CheckBox _cbCREventLogHandling;
        private readonly CheckBox _cbTimeBuying;
        //private CheckBox _cbAlarmAcknowledge;

        public AlaramAreaACLSettings(
            Panel panel, 
            Label lName, 
            CheckBox cbSet, 
            CheckBox cbUnset, 
            CheckBox cbUnconditionalSet, 
            CheckBox cbSensorHandling, 
            CheckBox cbCREventLogHandling, 
            CheckBox cbTimeBuying)//, CheckBox cbAlaramAcknowledge)
        {
            _panel = panel;
            _lName = lName;
            _cbSet = cbSet;
            _cbUnset = cbUnset;
            _cbUnconditionalSet = cbUnconditionalSet;
            _cbSensorHandling = cbSensorHandling;
            _cbCREventLogHandling = cbCREventLogHandling;
            _cbTimeBuying = cbTimeBuying;
            //_cbAlarmAcknowledge = cbAlaramAcknowledge;

            _cbSet.CheckedChanged += CheckedChanged;
            _cbUnconditionalSet.CheckedChanged += CheckedChanged;
            _cbTimeBuying.CheckedChanged += CheckedChanged;
            _cbUnset.CheckedChanged += CheckedChanged;
        }

        void CheckedChanged(object sender, EventArgs e)
        {
            if (sender == _cbSet && !_cbSet.Checked)
                _cbUnconditionalSet.Checked = false;

            if (sender == _cbUnconditionalSet && _cbUnconditionalSet.Checked)
                _cbSet.Checked = true;

            if (sender == _cbUnset && _cbUnset.Checked)
                _cbTimeBuying.Checked = true;

            if (sender == _cbTimeBuying && !_cbTimeBuying.Checked)
                _cbUnset.Checked = false;
        }

        public void Set(
            bool alarmAreaSet, 
            bool alarmAreaUnset, 
            bool alarmAreaUnconditionalSet, 
            bool alarmAreaAlarmAcknowledge, 
            bool sensorHandling, 
            bool crEventLogHandling,
            bool alarmAreaTimeBuying)
        {
            _cbTimeBuying.Checked = alarmAreaTimeBuying;
            _cbSet.Checked = alarmAreaSet;
            _cbUnset.Checked = alarmAreaUnset;
            _cbUnconditionalSet.Checked = alarmAreaUnconditionalSet;
            _cbSensorHandling.Checked = sensorHandling;
            _cbCREventLogHandling.Checked = crEventLogHandling;
            //_cbAlarmAcknowledge.Checked = alarmAreaAlarmAcknowledge;
        }

        public string GetText()
        {
            if (_lName != null)
                return _lName.Text;
            return string.Empty;
        }

        public void SetTop(int top)
        {
            if (_panel != null)
                _panel.Top = top;
        }

        public void SetVisible(bool visible)
        {
            if (_panel != null)
                _panel.Visible = visible;
        }

        public bool GetAlarmAreaSet()
        {
            if (_cbSet != null)
                return _cbSet.Checked;

            return false;
        }

        public bool GetAlarmAreaUnset()
        {
            if (_cbUnset != null)
                return _cbUnset.Checked;

            return false;
        }

        public bool GetAlarmAreaUnconditionalSet()
        {
            if (_cbUnconditionalSet != null)
                return _cbUnconditionalSet.Checked;

            return false;
        }

        public bool GetSensorHandling()
        {
            if (_cbSensorHandling != null)
                return _cbSensorHandling.Checked;

            return false;
        }

        public bool GetCREventLogHandling()
        {
            if (_cbCREventLogHandling != null)
                return _cbCREventLogHandling.Checked;

            return false;
        }

        public bool GetAlarmAreaTimeBuying()
        {
            if (_cbTimeBuying != null)
                return _cbTimeBuying.Checked;

            return false;
        }

        //public bool GetAlarmAreaAlaramAcknowledge()
        //{
        //    if (_cbAlarmAcknowledge != null)
        //        return _cbAlarmAcknowledge.Checked;

        //    return false;
        //}

        public bool IsSet()
        {
            return (_cbSet != null && _cbSet.Checked) ||
                   (_cbUnset != null && _cbUnset.Checked) ||
                   (_cbUnconditionalSet != null && _cbUnconditionalSet.Checked) ||
                   (_cbSensorHandling != null && _cbSensorHandling.Checked) ||
                   (_cbCREventLogHandling != null && _cbCREventLogHandling.Checked) ||
                   (_cbTimeBuying != null && _cbTimeBuying.Checked);
            //(_cbAlarmAcknowledge != null && _cbAlarmAcknowledge.Checked);
        }
    }
}
