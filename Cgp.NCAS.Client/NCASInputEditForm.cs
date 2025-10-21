using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Definitions;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

using SqlUniqueException = Contal.IwQuick.SqlUniqueException;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASInputEditForm :
#if DESIGNER
    Form
#else
 ACgpPluginEditFormWithAlarmInstructions<Input>
#endif
    {
        PresentationGroup _actAlarmPG;
        PresentationGroup _actTamperAlarmPG;
        DCU _actDcu;
        CCU _actCcu;
        readonly DelayTextEdit _editDelayToOn = new DelayTextEdit();
        readonly DelayTextEdit _editDelayToOff = new DelayTextEdit();
        readonly DelayTextEdit _editTamperDelayToOn = new DelayTextEdit();
        private AOrmObject _doorEnvironment;
        private Action<Guid, byte, Guid> _eventStateChanged;
        private AOnOffObject _actOnOffObject;
        private BlockingTypeItem _baseBlockingTypeItem;
        private AlarmArea _alarmArea;

        private bool _ccuInput;
        public bool CcuInput
        {
            get { return _ccuInput; }
            set
            {
                _ccuInput = value;
                SetVisualComponents(_ccuInput);
            }
        }

        public NCASInputEditForm(
                Input input,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                input,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            CcuInput = input.CCU != null;
            _editDelayToOn.AssimilateTextBox(_eDelayToOn);
            _editDelayToOff.AssimilateTextBox(_eDelayToOff);
            _editTamperDelayToOn.AssimilateTextBox(_eTamperDelayToOn);
            WheelTabContorol = _tcInput;
            ControlAccessRights();
            SetReferenceEditColors();
            if (showOption == ShowOptionsEditForm.Insert)
            {
                _nudInputIndex.Width = 264;
                _tbmDCU.Width = 264;
            }

            _cbBlockingType.MouseWheel += ControlMouseWheel;
            _eDescription.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            ShowTamperTab(false);

            _dgvAaInputs.CgpDataGridEvents = new CgpDataGridViewActions(
                OpenEditFormForAlarmArea,
                null,
                null);

            _dgvAaInputs.EnabledInsertButton = false;
            _dgvAaInputs.EnabledDeleteButton = false;

            _dgvAaInputs.LocalizationHelper = LocalizationHelper;

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsSettingsAdmin)));

                HideDisableTabPageAlarmAreas(Plugin.MainServerProvider.AlarmAreas.HasAccessView());

                HideDisableTabPageAlarmSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsAlarmSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsAlarmSettingsAdmin)));

                HideDisableTabPageInputControl(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsInputControlView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsInputControlAdmin)));

                HideDisableTabFoldersSructure(
                    CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessView());
                    // avoid the variant below to avoid instantiation of the form
                    //UserFoldersStructuresForm.Singleton.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.InputsDescriptionAdmin)));
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
                    _eNickName.ReadOnly = true;
                }

                if (!view && !admin)
                {
                    _tcInput.TabPages.Remove(_tpSettings);
                    return;
                }

                _tpSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageAlarmAreas(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageAlarmAreas),
                    view);
            }
            else
            {
                if (!view)
                    _tcInput.TabPages.Remove(_tpAlarmAreas);
            }
        }

        private void HideDisableTabPageAlarmSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAlarmSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcInput.TabPages.Remove(_tpAlarmSettings);
                    return;
                }

                _tpAlarmSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageInputControl(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageInputControl),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcInput.TabPages.Remove(_tpInputControl);
                    return;
                }

                _tpInputControl.Enabled = admin;
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
                    _tcInput.TabPages.Remove(_tpUserFolders);
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
                    _tcInput.TabPages.Remove(_tpReferencedBy);
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
                    _tcInput.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        private void SetVisualComponents(bool ccuInput)
        {
            if (ccuInput)
            {
                _lParent.Text = GetString("NCASInputEditForm_lCCUName");
                _itbDoorEnvironment.Enabled = false;
                _lDoorEnvironment.Enabled = false;

                if (Insert)
                    Text = GetString("CreateCCUInput");
            }
            else
            {
                _lParent.Text = GetString("NCASInputEditForm_lDCUName");
                _itbDoorEnvironment.Enabled = true;
                _lDoorEnvironment.Enabled = true;

                if (Insert)
                    Text = GetString("CreateDCUInput");
            }
        }

        public void SetFixedCCUParent(CCU ccu)
        {
            if (ccu == null)
                return;

            _actCcu = ccu;
            SetInputNumberInterval();
            _editingObject.CCU = ccu;
            _itbDCUName.Text = ccu.Name;
            _itbDCUName.Image = Plugin.GetImageForAOrmObject(_editingObject.CCU);
            _tbmDCU.Visible = false;
            _itbDCUName.Visible = true;
        }

        protected override void RegisterEvents()
        {
            _eventStateChanged = ChangeState;
            StateChangedInputHandler.Singleton.RegisterStateChanged(_eventStateChanged);

            CUDObjectHandler.Singleton.Register(CudObjectEvent, ObjectType.AlarmArea);
        }

        private void CudObjectEvent(
            ObjectType objectType,
            object id,
            bool inserted)
        {
            if (objectType != ObjectType.AlarmArea)
                return;

            var alarmArea = CgpClient.Singleton.MainServerProvider.GetTableObject(
                objectType,
                id.ToString()) as AlarmArea;

            var refreshAaInputs = false;

            if (_editingObject.AAInputs != null)
            {
                var aaInputsToRemove = new LinkedList<AAInput>();

                foreach (var aaInput in _editingObject.AAInputs)
                {
                    if (aaInput.AlarmArea.IdAlarmArea.Equals(id))
                        aaInputsToRemove.AddLast(aaInput);
                }

                foreach (var aaInput in aaInputsToRemove)
                {
                    _editingObject.AAInputs.Remove(aaInput);
                    refreshAaInputs = true;
                }
            }

            if (alarmArea != null)
            {
                var aaInputsToAdd = new LinkedList<AAInput>();

                foreach (var aaInput in alarmArea.AAInputs)
                {
                    if (aaInput.Input.IdInput.Equals(_editingObject.IdInput))
                        aaInputsToAdd.AddLast(aaInput);
                }

                if (_editingObject.AAInputs == null)
                    _editingObject.AAInputs = new List<AAInput>();

                foreach (var aaInput in aaInputsToAdd)
                {
                    _editingObject.AAInputs.Add(aaInput);
                    refreshAaInputs = true;
                }
            }

            if (refreshAaInputs)
                ShowAaInputs();
        }

        protected override void UnregisterEvents()
        {
            StateChangedInputHandler.Singleton.UnregisterStateChanged(_eventStateChanged);

            CUDObjectHandler.Singleton.Unregister(CudObjectEvent, ObjectType.AlarmArea);
        }

        private bool _isPGEnabled;
        private void ControlAccessRights()
        {
            try
            {
                if (!PresentationGroupsForm.Singleton.HasAccessView())
                {
                    _gbAlarmPresentationGroup.Enabled = false;
                    _gbTamperPresentationGroup.Enabled = false;
                }
                else
                {
                    _isPGEnabled = true;
                }
            }
            catch
            { }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString(!_ccuInput ? "CreateDCUInput" : "CreateCCUInput");
            }
            else
            {
                if (_ccuInput)
                    Text = GetString("InputCCUEditFormCCUText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            NCASInputsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASInputsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASInputsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASInputsForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;

            Input obj = Plugin.MainServerProvider.Inputs.GetObjectForEdit(_editingObject.IdInput, out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.Inputs.GetObjectById(_editingObject.IdInput);
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
                CheckInputFoorDoorEnvironment();
            }

            _firstTimeTpAlarmSettings = true;
            _firstTimeTpTamper = true;
            _firstTimeTpDescription = true;
            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.Inputs.RenewObjectForEdit(_editingObject.IdInput, out error);
            if (error != null)
            {
                throw error;
            }
        }


        protected override void SetValuesInsert()
        {
            _rbDI.Checked = true;
            _chbDIAlarmOn.Checked = true;
            _chbAlarmTamper.Checked = false;
            _nudInputIndex.Visible = true;
            _eInputIndex.Visible = false;
            _tbmDCU.Visible = true;
            _itbDCUName.Visible = false;
            AddInputControl();
            _bApply.Enabled = false;
        }

        protected override void SetValuesEdit()
        {
            _eName.Text = _editingObject.Name;
            _eNickName.Text = _editingObject.NickName;
            _chbHighPriority.Checked = _editingObject.HighPriority;
            _chbInverted.Checked = _editingObject.Inverted;
            _chbOffACK.Checked = _editingObject.OffACK;
            _eInputIndex.Text = (_editingObject.InputNumber + 1).ToString();
            _nudInputIndex.Visible = false;
            _eInputIndex.Visible = true;
            _itbDCUName.Visible = true;
            _tbmDCU.Visible = false;

            SetVisualComponents(_ccuInput);
            if (_editingObject.DCU != null)
            {
                _itbDCUName.Text = _editingObject.DCU.Name;
                _itbDCUName.Image = Plugin.GetImageForAOrmObject(_editingObject.DCU);
                _actCcu = _editingObject.DCU.CCU;
            }
            else if (_editingObject.CCU != null)
            {
                _itbDCUName.Text = _editingObject.CCU.Name;
                _itbDCUName.Image = Plugin.GetImageForAOrmObject(_editingObject.CCU);
                _actCcu = _editingObject.CCU;
            }

            LoadTpAlarmSettings();

            if (_editingObject.InputType == (byte)InputType.DI)
                _rbDI.Checked = true;
            if (_editingObject.InputType == (byte)InputType.BSI)
                _rbBSI.Checked = true;

            SafeThread.StartThread(ShowActualState, ThreadPriority.Lowest);
            SetTimeFromEditObject();
            CheckInputFoorDoorEnvironment();
            ShowAaInputs();
            SetReferencedBy();
            AddInputControl();
            SetOnOffObject(_editingObject.OnOffObject);
            
            _bApply.Enabled = false;
        }

        private void AddInputControl()
        {
            _cbBlockingType.Items.Clear();
            var blockingTypeItems = BlockingTypeItem.GetList(LocalizationHelper);

            if (blockingTypeItems != null && blockingTypeItems.Count > 0)
            {
                foreach (var blockingTypeItem in blockingTypeItems)
                {
                    _cbBlockingType.Items.Add(blockingTypeItem);
                    if (_editingObject.BlockingType == (byte)blockingTypeItem.BlockingType)
                    {
                        _baseBlockingTypeItem = blockingTypeItem;
                        _cbBlockingType.SelectedItem = blockingTypeItem;
                    }
                }
            }
        }

        private void SetOnOffObject(AOnOffObject onOffObject)
        {
            _actOnOffObject = onOffObject;
            RefreshOnOffObject();
        }

        private void RefreshOnOffObject()
        {
            if (_actOnOffObject == null)
            {
                _tbmControlObject.Text = string.Empty;
                _lICCU.Visible = false;
            }
            else
            {
                var listFS = new ListObjFS(_actOnOffObject);
                _tbmControlObject.Text = listFS.ToString();
                _tbmControlObject.TextImage = Plugin.GetImageForAOrmObject(_actOnOffObject);

                var idCCUObjAutomaticAct = Guid.Empty;
                var input = _actOnOffObject as Input;

                if (input != null)
                {
                    if (input.CCU != null)
                    {
                        idCCUObjAutomaticAct = input.CCU.IdCCU;
                    }
                    else
                        if (input.DCU != null &&
                            input.DCU.CCU != null)
                        {
                            idCCUObjAutomaticAct = input.DCU.CCU.IdCCU;
                        }
                }
                else
                {
                    var output = _actOnOffObject as Output;
                    if (output != null)
                    {
                        if (output.CCU != null)
                        {
                            idCCUObjAutomaticAct = output.CCU.IdCCU;
                        }
                        else if (output.DCU != null && output.DCU.CCU != null)
                        {
                            idCCUObjAutomaticAct = output.DCU.CCU.IdCCU;
                        }
                    }
                }

                var idInputCCU = Guid.Empty;
                if (_editingObject != null)
                {
                    if (_editingObject.CCU != null)
                    {
                        idInputCCU = _editingObject.CCU.IdCCU;
                    }
                    else if (_editingObject.DCU != null && _editingObject.DCU.CCU != null)
                    {
                        idInputCCU = _editingObject.DCU.CCU.IdCCU;
                    }
                }

                if (idCCUObjAutomaticAct != Guid.Empty && idInputCCU != Guid.Empty && idCCUObjAutomaticAct != idInputCCU)
                {
                    _lICCU.Visible = true;
                }
                else
                {
                    _lICCU.Visible = false;
                }
            }
        }

        private void CheckInputFoorDoorEnvironment()
        {
            try
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsInputInDoorEnvironments(_editingObject.IdInput))
                {
                    DisabledFormInDoorEnvironment();
                    ObtainDoorEnvironmentForInput();
                }
            }
            catch { }
        }

        private void ShowActualState()
        {
            var state = (byte)Plugin.MainServerProvider.Inputs.GetActualStates(_editingObject);
            ChangeState(_editingObject.IdInput, state, Guid.Empty);
        }

        private void ChangeState(Guid inputGuid, byte state, Guid parent)
        {
            if (inputGuid != _editingObject.IdInput)
                return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte, Guid>(ChangeState), inputGuid, state, parent);
            }
            else
            {
                switch (state)
                {
                    case (byte)InputState.Alarm:
                        _eStatusOnOff.Text = GetString("InputAlarm");
                        break;
                    case (byte)InputState.Short:
                        _eStatusOnOff.Text = GetString("InputShort");
                        break;
                    case (byte)InputState.Break:
                        _eStatusOnOff.Text = GetString("InputBreak");
                        break;
                    case (byte)InputState.Normal:
                        _eStatusOnOff.Text = GetString("InputNormal");
                        break;
                    case (byte)InputState.UsedByAnotherAplication:
                        _eStatusOnOff.Text = GetString("UsedByAnotherAplication");
                        break;
                    case (byte)InputState.OutOfRange:
                        _eStatusOnOff.Text = GetString("OutOfRange");
                        break;
                    default:
                        _eStatusOnOff.Text = GetString("Unknown");
                        break;
                }
            }
        }

        protected override void DisableForm()
        {
            base.DisableForm();
            _bCancel.Enabled = true;
        }

        private void DisabledFormInDoorEnvironment()
        {
            base.DisableForm();
            _bOk.Enabled = true;
            _bCancel.Enabled = true;
            _eName.Enabled = true;
            _eNickName.Enabled = true;
            _eDescription.Enabled = true;
            _itbDoorEnvironment.Enabled = true;
            _itbDoorEnvironment.TextBox.Enabled = true;
            _itbDCUName.Enabled = true;
            _itbDCUName.TextBox.Enabled = true;
            //_pRbSearch.Enabled = true;
            //_pRbTable.Enabled = true;
            //_pRbTop.Enabled = true;
            //_bRoRefresh.Enabled = true;
            //_dgReferencedBy.Enabled = true;
            //_bRoSearch.Enabled = true;
            //_bRoClear.Enabled = true;
            //_eRoFilterText.Enabled = true;
            _eInputIndex.Enabled = true;
            _itbDCUName.Enabled = true;
            EnabledControls(_tpAlarmSettings);
            EnabledControls(_tpTamper);
            EnabledControls(_tpReferencedBy);
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;
                _editingObject.NickName = _eNickName.Text;

                if (_rbDI.Checked)
                    _editingObject.InputType = (byte)InputType.DI;
                else if (_rbBSI.Checked)
                    _editingObject.InputType = (byte)InputType.BSI;

                _editingObject.HighPriority = _chbHighPriority.Checked;
                _editingObject.Inverted = _chbInverted.Checked;
                _editingObject.OffACK = _chbOffACK.Checked;
                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

                GetValuesTpAlarmSettings();
                GetValuesTpDescription();
                GetValuesTpTamper();
                TimeToEditObject();

                var blockingTypeItem = _baseBlockingTypeItem;
                if (blockingTypeItem != null)
                    _editingObject.BlockingType = (byte)blockingTypeItem.BlockingType;

                _editingObject.OnOffObject = _actOnOffObject;
                RefreshOnOffObject();

                if (Insert)
                {
                    _editingObject.DCU = _actDcu;
                    _editingObject.CCU = _actCcu;
                    _editingObject.InputNumber = (byte)(_nudInputIndex.Value - 1);

                    //changing name to full form if required
                    if (_editingObject.DCU != null)
                    {
                        var newName = string.Empty;
                        if (!Plugin.MainServerProvider.GetEnableParentInFullName())
                        {
                            if (_editingObject.DCU.CCU != null)
                                newName = _editingObject.DCU.CCU.Name + StringConstants.SLASHWITHSPACES;
                            if (_editingObject.DCU.Name.Contains(StringConstants.SLASH[0]))
                                newName += _editingObject.DCU.Name.Split(StringConstants.SLASH[0])[1] + StringConstants.SLASHWITHSPACES + _eName.Text;
                            else
                                newName += _editingObject.DCU.Name + StringConstants.SLASHWITHSPACES + _eName.Text;
                            _editingObject.Name = newName;
                        }
                    }
                    else if (_editingObject.CCU != null)
                    {
                        if (!Plugin.MainServerProvider.GetEnableParentInFullName())
                        {
                            string newName = _editingObject.CCU.Name + StringConstants.SLASHWITHSPACES;
                            newName += _eName.Text;
                            _editingObject.Name = newName;
                        }
                    }
                }
                return true;
            }
            catch
            {
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool CheckValues()
        {
            if (Insert)
            {
                if (_actDcu == null && _actCcu == null)
                {
                    var stringToLocalise = string.Empty;
                    if (!_ccuInput && _actDcu == null)
                        stringToLocalise = "ErrorSpecifyDcu";
                    else if (_ccuInput && _actCcu == null)
                        stringToLocalise = "ErrorSpecifyCcu";

                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDCU,
                            GetString(stringToLocalise), ControlNotificationSettings.Default);
                    _tbmDCU.Focus();
                    return false;
                }
                if (InputIndexUsed())
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _nudInputIndex,
                        GetString("ErrorInputIndexUsed"), ControlNotificationSettings.Default);
                    _nudInputIndex.Focus();
                    return false;
                }
            }

            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                        GetString("ErrorInsertInputName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }

            if (!_rbBSI.Checked && !_rbDI.Checked)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _rbBSI,
                        GetString("ErrorSelectDIBSIState"), ControlNotificationSettings.Default);
                _rbBSI.Focus();
                return false;
            }

            var blockingTypeItem = _baseBlockingTypeItem;
            if (blockingTypeItem != null && blockingTypeItem.BlockingType == BlockingType.BlockedByObject && _actOnOffObject == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                    GetString("ErrorInsertOnOffObjectForBlockingInput"), ControlNotificationSettings.Default);
                _tbmControlObject.Focus();
                return false;
            }

            return true;
        }

        private void CancelClick(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void OkClick(object sender, EventArgs e)
        {
            Ok_Click();
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            var retValue = Plugin.MainServerProvider.Inputs.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message == Input.COLUMNNAME)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedInputName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
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

            bool retValue = 
                onlyInDatabase
                    ? Plugin.MainServerProvider.Inputs.UpdateOnlyInDatabase(_editingObject, out error) 
                    : Plugin.MainServerProvider.Inputs.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message == Input.COLUMNNAME)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedInputName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            if (sender == _cbBlockingType)
            {
                var actualBlockingTypeItem = _cbBlockingType.SelectedItem as BlockingTypeItem;

                if (!IsSetValues || _baseBlockingTypeItem != actualBlockingTypeItem)
                {
                    _baseBlockingTypeItem = actualBlockingTypeItem;
                    if (_baseBlockingTypeItem != null && _baseBlockingTypeItem.BlockingType == BlockingType.BlockedByObject)
                        _gbControlObject.Enabled = true;
                    else
                        _gbControlObject.Enabled = false;
                }
                else
                {
                    return;
                }
            }

            if (!Insert)
                _bApply.Enabled = true;

            base.EditTextChanger(sender, e);
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.Inputs != null)
                Plugin.MainServerProvider.Inputs.EditEnd(_editingObject);
        }

        private void ModifyAlarm()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                var listModObj = new List<IModifyObject>();

                var listPGFromDatabase = CgpClient.Singleton.MainServerProvider.PresentationGroups.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listPGFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, CgpClient.Singleton.LocalizationHelper.GetString("PresentationGroupsFormPresentationGroupsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    var presetationGroup = CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectById(outModObj.GetId);
                    SetAlarmPresentationGroup(presetationGroup);
                    Plugin.AddToRecentList(_actAlarmPG);
                }
            }
            catch
            {
                Dialog.Error(error);
            }
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            var presetationGroup = new PresentationGroup();
            PresentationGroupsForm.Singleton.OpenInsertFromEdit(ref presetationGroup, DoAfterCreatedPG);
        }

        private void DoAfterCreatedPG(object newPg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCreatedPG), newPg);
            }
            else
            {
                SetAlarmPresentationGroup(newPg as PresentationGroup);
            }
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            SetAlarmPresentationGroup(null);
        }

        private void AddAlarmPresentationGroup(object newPresetationGroup)
        {
            try
            {
                if (newPresetationGroup.GetType() == typeof(PresentationGroup))
                {
                    SetAlarmPresentationGroup((PresentationGroup)newPresetationGroup);
                    Plugin.AddToRecentList(newPresetationGroup);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmAlarmPresentationGroup.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void SetAlarmPresentationGroup(PresentationGroup presetationGroup)
        {
            EditTextChanger(null, null);
            _actAlarmPG = presetationGroup;
            if (presetationGroup == null)
            {
                _tbmAlarmPresentationGroup.Text = string.Empty;
                _chbAlarmPGPresentationStateInpendentOfAlarm.Enabled = false;
            }
            else
            {
                _tbmAlarmPresentationGroup.Text = _actAlarmPG.ToString();
                _tbmAlarmPresentationGroup.TextImage = Plugin.GetImageForAOrmObject(_actAlarmPG);
                _chbAlarmPGPresentationStateInpendentOfAlarm.Enabled = true;
            }
        }

        private void _tbmAlarmPresentationGroup_DoubleClick(object sender, EventArgs e)
        {
            if (_actAlarmPG != null)
            {
                PresentationGroupsForm.Singleton.OpenEditForm(_actAlarmPG);
            }
        }

        private void _tbmAlarmPresentationGroup_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddAlarmPresentationGroup(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmAlarmPresentationGroup_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void ModifyTamper()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                var listModObj = new List<IModifyObject>();

                var listPGFromDatabase = CgpClient.Singleton.MainServerProvider.PresentationGroups.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listPGFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, CgpClient.Singleton.LocalizationHelper.GetString("PresentationGroupsFormPresentationGroupsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    var presetationGroup = CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectById(outModObj.GetId);
                    SetTamperAlarmPG(presetationGroup);
                    Plugin.AddToRecentList(presetationGroup);
                }
            }
            catch
            {
                Dialog.Error(error);
            }
        }

        private void DoAfterCreatedTamperPG(object newPg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCreatedTamperPG), newPg);
            }
            else
            {
                SetTamperAlarmPG(newPg as PresentationGroup);
            }
        }

        private void _bDelete1_Click(object sender, EventArgs e)
        {
            SetTamperAlarmPG(null);
        }

        private void AddTamperAlarmPG(object newPresetationGroup)
        {
            try
            {
                if (newPresetationGroup.GetType() == typeof(PresentationGroup))
                {
                    SetTamperAlarmPG((PresentationGroup)newPresetationGroup);
                    Plugin.AddToRecentList(newPresetationGroup);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmTamperPresentationGroup.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void SetTamperAlarmPG(PresentationGroup presetationGroup)
        {
            EditTextChanger(null, null);
            _actTamperAlarmPG = presetationGroup;
            if (presetationGroup == null)
            {
                _tbmTamperPresentationGroup.Text = string.Empty;
            }
            else
            {
                _tbmTamperPresentationGroup.Text = _actTamperAlarmPG.ToString();
                _tbmTamperPresentationGroup.TextImage = Plugin.GetImageForAOrmObject(_actTamperAlarmPG);
            }
        }

        private void _tbmTamperPresentationGroup_DoubleClick(object sender, EventArgs e)
        {
            if (_actTamperAlarmPG != null)
            {
                PresentationGroupsForm.Singleton.OpenEditForm(_actTamperAlarmPG);
            }
        }

        private void _tbmTamperPresentationGroup_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddTamperAlarmPG(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmTamperPresentationGroup_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _rbDI_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            _gbTamperPresentationGroup.Enabled = false;
            _gbTamperDelay.Enabled = false;
            _gbEnableAlarms1.Enabled = false;
        }

        private void _rbBSI_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            if (_isPGEnabled)
            {
                _gbTamperPresentationGroup.Enabled = true;
                _gbTamperDelay.Enabled = true;
                _gbEnableAlarms1.Enabled = true;
            }
            ShowTamperTab(_rbBSI.Checked);
        }

        private void ShowTamperTab(bool visible)
        {
            this.BeginInvokeInUI(() =>
            {
                if (visible)
                {
                    if (!_tcInput.Contains(_tpTamper))
                        _tcInput.TabPages.Insert(2, _tpTamper);

                    LocalizationHelper.TranslateControl(_tpTamper);
                }
                else
                {
                    if (_tcInput.Contains(_tpTamper))
                        _tcInput.TabPages.Remove(_tpTamper);
                }
            
            });
        
        }

        private void SetTimeFromEditObject()
        {
            _editDelayToOn.SetMiliSeconds(_editingObject.DelayToOn);
            _editDelayToOff.SetMiliSeconds(_editingObject.DelayToOff);
            _editTamperDelayToOn.SetMiliSeconds(_editingObject.TamperDelayToOn);
        }


        private void TimeToEditObject()
        {
            _editingObject.DelayToOn = _editDelayToOn.GetMiliSeconds();
            _editingObject.DelayToOff = _editDelayToOff.GetMiliSeconds();
            _editingObject.TamperDelayToOn = _editTamperDelayToOn.GetMiliSeconds();
        }

        private void ObtainDoorEnvironmentForInput()
        {
            _doorEnvironment = Plugin.MainServerProvider.DoorEnvironments.GetInputDoorEnvironment(_editingObject.IdInput);
            if (_doorEnvironment != null)
            {
                _itbDoorEnvironment.Text = _doorEnvironment.ToString();
                _itbDoorEnvironment.Image = Plugin.GetImageForAOrmObject(_doorEnvironment);
            }
        }

        private void ShowAaInputs()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(ShowAaInputs));
                return;
            }

            if (_editingObject.AAInputs == null
                || _editingObject.AAInputs.Count == 0)
            {
                _tcInput.TabPages.Remove(_tpAlarmAreas);
                return;
            }

            _dgvAaInputs.DataGrid.DataSource = new BindingSource
            {
                DataSource = _editingObject.AAInputs.OrderBy(aaInput => aaInput.AlarmArea.Id),
            };

            HideColumnDgw(_dgvAaInputs.DataGrid, AAInput.COLUMNIDAAINPUT);
            HideColumnDgw(_dgvAaInputs.DataGrid, AAInput.COLUMN_ID);
            HideColumnDgw(_dgvAaInputs.DataGrid, AAInput.COLUMNINPUT);
            HideColumnDgw(_dgvAaInputs.DataGrid, AAInput.COLUMNGUIDINPUT);
            HideColumnDgw(_dgvAaInputs.DataGrid, AAInput.COLUMNNOCRITICALINPUT);
            HideColumnDgw(_dgvAaInputs.DataGrid, AAInput.COLUMN_BLOCK_TEMPORARILY_UNTIL);
            HideColumnDgw(_dgvAaInputs.DataGrid, AAInput.COLUMN_SENSOR_PURPOSE);
            HideColumnDgw(_dgvAaInputs.DataGrid, AAInput.ColumnVersion);

            if (_dgvAaInputs.DataGrid.Columns.Contains(AAInput.COLUMN_SECTION_ID))
            {
                _dgvAaInputs.DataGrid.Columns[AAInput.COLUMN_SECTION_ID].AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.DisplayedCells;

                _dgvAaInputs.DataGrid.Columns[AAInput.COLUMN_SECTION_ID].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;

                _dgvAaInputs.DataGrid.Columns[AAInput.COLUMN_SECTION_ID].SortMode =
                    DataGridViewColumnSortMode.NotSortable;
            }

            if (_dgvAaInputs.DataGrid.Columns.Contains(AAInput.COLUMNALARMAREA)
                && _dgvAaInputs.DataGrid.Columns.Contains(AAInput.COLUMNINPUTNAME))
            {
                int width = (_dgvAaInputs.DataGrid.Width -
                             _dgvAaInputs.DataGrid.Columns[AAInput.COLUMN_SECTION_ID].GetPreferredWidth(
                                 DataGridViewAutoSizeColumnMode.DisplayedCells, true) - 40)/2;

                _dgvAaInputs.DataGrid.Columns[AAInput.COLUMNALARMAREA].Width = width;
                _dgvAaInputs.DataGrid.Columns[AAInput.COLUMNINPUTNAME].Width = width;
            }

            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvAaInputs.DataGrid);
        }

        private void OpenEditFormForAlarmArea()
        {
            var bsAaInputs = _dgvAaInputs.DataGrid.DataSource as BindingSource;

            if (bsAaInputs == null)
                return;

            var aaInput = bsAaInputs.Current as AAInput;

            if (aaInput == null)
                return;

            NCASAlarmAreasForm.Singleton.OpenEditForm(aaInput.AlarmArea);
        }

        private void _itbDoorEnvironment_DoubleClick(object sender, EventArgs e)
        {
            if (_doorEnvironment != null)
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;

                switch (_doorEnvironment.GetObjectType())
                {
                    case ObjectType.DoorEnvironment:
                        var doorEnvironemnt = (DoorEnvironment)_doorEnvironment;

                        if (doorEnvironemnt.DCU != null)
                        {
                            NCASDCUsForm.Singleton.OpenEditForm(doorEnvironemnt.DCU);
                        }
                        else
                        {
                            NCASDoorEnvironmentsForm.Singleton.OpenEditForm(doorEnvironemnt);
                        }

                        break;
                    case ObjectType.MultiDoorElement:
                        NCASMultiDoorElementsForm.Singleton.OpenEditForm(_doorEnvironment as MultiDoorElement);
                        break;
                } 
            }
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, NCASClient.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return Plugin.MainServerProvider.Inputs.
                GetReferencedObjects(_editingObject.IdInput, CgpClient.Singleton.GetListLoadedPlugins());
        }


        #region UserFolders
        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }
        #endregion

        private void _tbmAlarmPresentationGroup_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyAlarm();
            }
            else if (item.Name == "_tsiRemove")
            {
                SetAlarmPresentationGroup(null);
            }
            else if (item.Name == "_tsiCreate")
            {
                var presetationGroup = new PresentationGroup();
                PresentationGroupsForm.Singleton.OpenInsertFromEdit(ref presetationGroup, DoAfterCreatedPG);
            }
        }

        private void _tbmTamperPresentationGroup_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                ModifyTamper();
            }
            else if (item.Name == "_tsiRemove2")
            {
                SetTamperAlarmPG(null);
            }
            else if (item.Name == "_tsiCreate2")
            {
                var presetationGroup = new PresentationGroup();
                PresentationGroupsForm.Singleton.OpenInsertFromEdit(ref presetationGroup, DoAfterCreatedTamperPG);
            }
        }

        private void _itbDCUName_DoubleClick(object sender, EventArgs e)
        {
            if (_editingObject.DCU != null)
            {
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;
                NCASDCUsForm.Singleton.OpenEditForm(_editingObject.DCU);
            }
            else if (_editingObject.CCU != null)
            {
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;
                NCASCCUsForm.Singleton.OpenEditForm(_editingObject.CCU);
            }
        }

        #region TabPage AlarmSettings
        bool _firstTimeTpAlarmSettings = true;
        private void _tpAlarmSettings_Enter(object sender, EventArgs e)
        {
            if (_firstTimeTpAlarmSettings)
            {
                _firstTimeTpAlarmSettings = false;
                LoadTpAlarmSettings();
            }
        }
        private void LoadTpAlarmSettings()
        {
            LockChanges();

            _actAlarmPG = _editingObject.AlarmPresentationGroup;
            if (_actAlarmPG != null)
            {
                _tbmAlarmPresentationGroup.Text = _actAlarmPG.GroupName;
                _tbmAlarmPresentationGroup.TextImage = Plugin.GetImageForAOrmObject(_actAlarmPG);
                _chbAlarmPGPresentationStateInpendentOfAlarm.Enabled = true;
            }
            else
            {
                _chbAlarmPGPresentationStateInpendentOfAlarm.Enabled = false;
            }

            _chbAlarmPGPresentationStateInpendentOfAlarm.Checked = _editingObject.AlarmPGPresentationStateInpendentOfAlarm;

            _chbDIAlarmOn.Checked = _editingObject.AlarmOn;

            UnlockChanges();
        }

        private void GetValuesTpAlarmSettings()
        {
            if (!_firstTimeTpAlarmSettings)
            {
                _editingObject.AlarmOn = _chbDIAlarmOn.Checked;
                _editingObject.AlarmPresentationGroup = _actAlarmPG;
                _editingObject.AlarmPGPresentationStateInpendentOfAlarm = _chbAlarmPGPresentationStateInpendentOfAlarm.Checked;
            }
        }
        #endregion

        #region TabPage Tamper
        bool _firstTimeTpTamper = true;
        private void _tpTamper_Enter(object sender, EventArgs e)
        {
            if (_firstTimeTpTamper)
            {
                _firstTimeTpTamper = false;
                LoadTpTamper();
            }
        }

        private void LoadTpTamper()
        {
            LockChanges();
            _actTamperAlarmPG = _editingObject.TamperAlarmPresentationGroup;
            if (_actTamperAlarmPG != null)
            {
                _tbmTamperPresentationGroup.Text = _actTamperAlarmPG.GroupName;
                _tbmTamperPresentationGroup.TextImage = Plugin.GetImageForAOrmObject(_actTamperAlarmPG);
            }
            _chbAlarmTamper.Checked = _editingObject.AlarmTamper;
            UnlockChanges();
        }

        private void GetValuesTpTamper()
        {
            if (!_firstTimeTpTamper)
            {
                _editingObject.TamperAlarmPresentationGroup = _actTamperAlarmPG;
                _editingObject.AlarmTamper = _chbAlarmTamper.Checked;
            }
        }
        #endregion

        #region TabPage Description
        bool _firstTimeTpDescription = true;
        private void _tpDescription_Enter(object sender, EventArgs e)
        {
            if (_firstTimeTpDescription)
            {
                _firstTimeTpDescription = false;
                LoadTpDescription();
            }
        }

        private void LoadTpDescription()
        {
            LockChanges();
            _eDescription.Text = _editingObject.Description;
            UnlockChanges();
        }

        private void GetValuesTpDescription()
        {
            if (!_firstTimeTpDescription)
            {
                _editingObject.Description = _eDescription.Text;
            }
        }
        #endregion

        private void _tbmControlObject_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify1)
            {
                ModifyOnOffObject();
            }
            else if (item == _tsiRemove1)
            {
                SetOnOffObject(null);
            }
        }

        private void ModifyOnOffObject()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                ConnectionLost();

                var listObjects = new List<IModifyObject>();

                Exception error;
                var listTimeZonesFromDatabase = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;

                if (listTimeZonesFromDatabase != null && listTimeZonesFromDatabase.Count > 0)
                    listObjects.AddRange(listTimeZonesFromDatabase);

                var listDailyPlansFromDatabase = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateDailyPlans(listDailyPlansFromDatabase);
                if (listDailyPlansFromDatabase != null && listDailyPlansFromDatabase.Count > 0)
                {
                    listObjects.AddRange(listDailyPlansFromDatabase);
                }

                IList<FilterSettings> filterSettings = new List<FilterSettings>();
                var filterSetting = new FilterSettings(Input.COLUMNIDIMPUT, _editingObject.IdInput, ComparerModes.NOTEQUALL);
                filterSettings.Add(filterSetting);

                IList<IModifyObject> listInputsFromDatabase = Plugin.MainServerProvider.Inputs.ModifyObjectsSelectByCriteria(filterSettings, out error);

                if (error != null)
                    throw error;

                if (listInputsFromDatabase != null && listInputsFromDatabase.Count > 0)
                    listObjects.AddRange(listInputsFromDatabase);

                IList<IModifyObject> listOutputsFromDatabase = Plugin.MainServerProvider.Outputs.ListModifyObjects(out error);

                if (error != null) throw error;

                if (listOutputsFromDatabase != null && listOutputsFromDatabase.Count > 0)
                    listObjects.AddRange(listOutputsFromDatabase);

                var formAdd = new ListboxFormAdd(listObjects, GetString("NCASInputEditFormListObjectForBlockedInputText"));
                object outObject;
                formAdd.ShowDialog(out outObject);
                var modifyObject = outObject as IModifyObject;
                if (modifyObject != null)
                {
                    switch (modifyObject.GetOrmObjectType)
                    {
                        case ObjectType.TimeZone:
                            outObject =
                                CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(
                                    modifyObject.GetId);
                            break;
                        case ObjectType.DailyPlan:
                            outObject =
                                CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(
                                    modifyObject.GetId);
                            break;
                        case ObjectType.Input:
                            outObject =
                                Plugin.MainServerProvider.Inputs.GetObjectById(modifyObject.GetId);
                            break;
                        case ObjectType.Output:
                            outObject =
                                Plugin.MainServerProvider.Outputs.GetObjectById(modifyObject.GetId);
                            break;
                    }
                }
                else
                {
                    return;
                }

                var onOffObject = outObject as AOnOffObject;
                if (onOffObject != null)
                {
                    SetOnOffObject(onOffObject);
                    Plugin.AddToRecentList(onOffObject);
                }
            }
            catch
            {
            }
        }

        private void _tbmControlObject_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actOnOffObject != null)
            {
                if (_actOnOffObject is TimeZone)
                {
                    TimeZonesForm.Singleton.OpenEditForm(_actOnOffObject as TimeZone);
                }
                else if (_actOnOffObject is DailyPlan)
                {
                    DailyPlansForm.Singleton.OpenEditForm(_actOnOffObject as DailyPlan);
                }
                else if (_actOnOffObject is Input)
                {
                    NCASInputsForm.Singleton.OpenEditForm(_actOnOffObject as Input);
                }
                else if (_actOnOffObject is Output)
                {
                    NCASOutputsForm.Singleton.OpenEditForm(_actOnOffObject as Output);
                }
            }
        }

        private void _tbmControlObject_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmControlObject_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOnOffObject(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddOnOffObject(object newOnOffObject)
        {
            try
            {
                if (newOnOffObject is TimeZone || newOnOffObject is DailyPlan ||
                    newOnOffObject is Input || newOnOffObject is Output)
                {
                    if (!(newOnOffObject is Input && _editingObject.Compare(newOnOffObject)))
                    {
                        if (_actCcu != null)
                        {
                            var isCorrectInputOutput = true;
                            var input = newOnOffObject as Input;
                            if (input != null)
                            {
                                if (input.CCU != null)
                                {
                                    isCorrectInputOutput = input.CCU.IdCCU == _actCcu.IdCCU;
                                }
                                else
                                    if (input.DCU != null &&
                                        input.DCU.CCU != null)
                                    {
                                        isCorrectInputOutput = input.DCU.CCU.IdCCU == _actCcu.IdCCU;
                                    }
                            }
                            else
                            {
                                var output = newOnOffObject as Output;

                                if (output != null)
                                {
                                    if (output.CCU != null)
                                    {
                                        isCorrectInputOutput = output.CCU.IdCCU == _actCcu.IdCCU;
                                    }
                                    else
                                        if (output.DCU != null &&
                                            output.DCU.CCU != null)
                                        {
                                            isCorrectInputOutput = output.DCU.CCU.IdCCU ==
                                                                   _actCcu.IdCCU;
                                        }
                                }
                            }

                            if (!isCorrectInputOutput)
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                                    GetString("ErrorInputInputOutputFromCurrentCCU"), ControlNotificationSettings.Default);
                                return;
                            }
                        }

                        SetOnOffObject((AOnOffObject)newOnOffObject);
                        Plugin.AddToRecentList(newOnOffObject);
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                            GetString("ErrorOnOffObjectForBlockingCanNotBeActualEditingInput"), ControlNotificationSettings.Default);
                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmDCU_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var parent = e.Data.GetFormats();
                
                if (parent == null) 
                    return;
                
                AddParent(e.Data.GetData(parent[0]));
            }
            catch
            {
            }
        }

        private void _tbmDCU_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmDCU_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify3")
            {
                ModifyParent();
            }
            else if (item.Name == "_tsiRemove3")
            {
                RemoveParent();
            }
        }

        private void AddParent(object newParent)
        {
            try
            {
                CcuInput = newParent.GetType() == typeof (CCU);

                if (newParent.GetType() != typeof(DCU) && newParent.GetType() != typeof(CCU))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDCU.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
                else
                {
                    if (_ccuInput)
                    {
                        var ccu = newParent as CCU;
                        if (ccu != null)
                        {
                            if (CcuHasInputs(ccu))
                            {
                                SetActCcu(ccu);
                                Plugin.AddToRecentList(newParent);
                            }
                            else
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDCU.ImageTextBox,
                                    GetString("ErrorCcuHasNoInputs"), ControlNotificationSettings.Default);
                            }
                        }
                    }
                    else
                    {
                        var dcu = newParent as DCU;
                        if (dcu != null)
                        {
                            if (DcuHasInputs(dcu))
                            {
                                SetActDcu(dcu);
                                Plugin.AddToRecentList(newParent);
                            }
                            else
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDCU.ImageTextBox,
                                    GetString("ErrorDcuHasNoInputs"), ControlNotificationSettings.Default);
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        private void ModifyParent()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                var ccus = Plugin.MainServerProvider.CCUs.ListModifyObjects(out error);
                var dcus = Plugin.MainServerProvider.DCUs.ListModifyObjects(out error);

                if (error != null) throw error;

                var listModObj = new List<IModifyObject>();
                listModObj.AddRange(ccus);
                listModObj.AddRange(dcus);

                var formAdd = new ListboxFormAdd(listModObj,
                    string.Format("{0}, {1}", GetString("NCASCCUsFormNCASCCUsForm"),
                        GetString("NCASDCUsFormNCASDCUsForm"))); 

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);

                if (outModObj != null)
                {
                    CcuInput = outModObj.GetOrmObjectType == ObjectType.CCU;

                    if (CcuInput)
                    {
                        var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(outModObj.GetId);
                        if (!CcuHasInputs(ccu))
                        {
                            Dialog.Error(GetString("ErrorCcuHasNoInputs"));
                            return;
                        }
                        SetActCcu(ccu);
                        Plugin.AddToRecentList(_actCcu);
                    }
                    else
                    {
                        var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(outModObj.GetId);
                        if (!DcuHasInputs(dcu))
                        {
                            Dialog.Error(GetString("ErrorDcuHasNoInputs"));
                            return;
                        }
                        SetActDcu(dcu);
                        Plugin.AddToRecentList(_actDcu);
                    }
                }
            }
            catch
            {
                Dialog.Error(error);
            }
        }

        private static bool DcuHasInputs(DCU dcu)
        {
            if (dcu.InputsCount != null && dcu.InputsCount > 0)
                return true;
            if (dcu.Inputs.Count > 0)
                return true;
            return false;
        }

        private static bool CcuHasInputs(CCU ccu)
        {
            if (ccu == null)
                return false;
            if (ccu.InputsCount != null && ccu.InputsCount > 0)
                return true;
            if (ccu.Inputs != null && ccu.Inputs.Count > 0)
                return true;
            return false;
        }

        private void SetActDcu(DCU dcu)
        {
            _actDcu = dcu;
            ShowActDcu();
        }

        private void SetActCcu(CCU ccu)
        {
            _actCcu = ccu;
            ShowActCcu();
        }

        private void RemoveParent()
        {
            if (_ccuInput)
            {
                _actCcu = null;
                ShowActCcu();
            }
            else
            {
                _actDcu = null;
                ShowActDcu();
            }
        }

        private void ShowActDcu()
        {
            if (_actDcu != null)
            {
                _tbmDCU.Text = _actDcu.ToString();
                _tbmDCU.TextImage = Plugin.GetImageForAOrmObject(_actDcu);
            }
            else
            {
                _tbmDCU.Text = string.Empty;
            }
            SetInputNumberInterval();
        }

        private void ShowActCcu()
        {
            if (_actCcu != null)
            {
                _tbmDCU.Text = _actCcu.ToString();
                _tbmDCU.TextImage = Plugin.GetImageForAOrmObject(_actCcu);
            }
            else
            {
                _tbmDCU.Text = string.Empty;
            }
            SetInputNumberInterval();
        }

        private void SetInputNumberInterval()
        {
            if (_actCcu != null)
            {
                if (_actCcu.InputsCount != null)
                    _nudInputIndex.Maximum = (decimal)_actCcu.InputsCount;
                else
                    _nudInputIndex.Maximum = _actCcu.Inputs.Count;
            }
            else if (_actDcu != null)
            {
                if (_actDcu.InputsCount != null)
                    _nudInputIndex.Maximum = (decimal)_actDcu.InputsCount;
                else
                    _nudInputIndex.Maximum = _actDcu.Inputs.Count;
            }
            else
            {
                _nudInputIndex.Maximum = 1;
            }
        }

        private bool InputIndexUsed()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return false;
            if (CcuInput)
                return Plugin.MainServerProvider.Inputs.ExistCcuInputWithIndex((byte)(_nudInputIndex.Value - 1), _actCcu);
            return Plugin.MainServerProvider.Inputs.ExistInputWithIndex((byte)(_nudInputIndex.Value - 1), _actDcu);
        }

        private void _tbmDCU_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_ccuInput)
            {
                if (_actCcu != null)
                {
                    if (CgpClient.Singleton.IsConnectionLost(false))
                        return;
                    NCASCCUsForm.Singleton.OpenEditForm(_actCcu);
                }
            }
            else

                if (_actDcu != null)
                {
                    if (CgpClient.Singleton.IsConnectionLost(false))
                        return;
                    NCASDCUsForm.Singleton.OpenEditForm(_actDcu);
                }
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                _bApply.Enabled = false;
            }
        }

        private void _panelBack_Paint(object sender, PaintEventArgs e)
        {

        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.InputsLocalAlarmInstructionsView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.InputsLocalAlarmInstructionsAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion
    }

    public class BlockingTypeItem
    {
        private readonly BlockingType _blockingType;
        private readonly LocalizationHelper _localizationHelper;

        public BlockingType BlockingType { get { return _blockingType; } }

        public BlockingTypeItem(BlockingType blockingType, LocalizationHelper localizationHelper)
        {
            _blockingType = blockingType;
            _localizationHelper = localizationHelper;
        }

        public override string ToString()
        {
            if (_localizationHelper != null)
                return _localizationHelper.GetString("BlockingType_" + _blockingType);
            return _blockingType.ToString();
        }

        public static IList<BlockingTypeItem> GetList(LocalizationHelper localizationHelper)
        {
            return EnumHelper.ListAllValuesWithProcessing<BlockingType, BlockingTypeItem>(
                (value) => new BlockingTypeItem(value, localizationHelper));
        }
    }
}
