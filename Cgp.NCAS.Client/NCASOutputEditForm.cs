using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

using SqlUniqueException = Contal.IwQuick.SqlUniqueException;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASOutputEditForm :
#if DESIGNER
    Form
#else
    ACgpPluginEditFormWithAlarmInstructions<Output>
#endif
    {
        private PresentationGroup _actAlarmPG;
        private AOnOffObject _actOnOffObject;
        private DCU _actDcu;
        private CCU _actCcu;

        DelayTextEdit _editSettingsDelayToOn = new DelayTextEdit();
        DelayTextEdit _editSettingsDelayToOff = new DelayTextEdit();
        DelayTextEdit _editSettingsPulseLength = new DelayTextEdit();
        DelayTextEdit _editSettingsPulseDelay = new DelayTextEdit();
        private Action<Guid, byte, Guid> _eventStateChanged;
        private Action<Guid, byte, Guid> _eventRealStateChanged;

        private bool _ccuOutput;
        public bool CcuOutput
        {
            get { return _ccuOutput; }
            set
            {
                _ccuOutput = value;
                SetVisualComponents(_ccuOutput);
            }
        }

        public NCASOutputEditForm(
                Output output,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                output,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            _ccuOutput = output.CCU != null;
            _editSettingsDelayToOn.AssimilateTextBox(_eSettingsDelayToOn);
            _editSettingsDelayToOff.AssimilateTextBox(_eSettingsDelayToOff);
            _editSettingsPulseLength.AssimilateTextBox(_eSettingsPulseLength);
            _editSettingsPulseDelay.AssimilateTextBox(_eSettingsPulseDelay);
            WheelTabContorol = _tcOutput;
            ControlAccessRights();            
            MinimumSize = new Size(Width, Height);
            _cbOutPutType.MouseWheel += ControlMouseWheel;
            _cbOutputControlType.MouseWheel += ControlMouseWheel;
            _eDescription.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
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
                        NCASAccess.GetAccess(AccessNCAS.OutputsSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.OutputsSettingsAdmin)));

                HideDisableTabPageAlarmSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.OutputsAlarmSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.OutputsAlarmSettingsAdmin)));

                HideDisableTabPageOutputControl(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.OutputsOutputControlView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.OutputsOutputControlAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.OutputsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.OutputsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.OutputsDescriptionAdmin)));
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
                    _tcOutput.TabPages.Remove(_tpSettings);
                    return;
                }

                _tpSettings.Enabled = admin;
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
                    _tcOutput.TabPages.Remove(_tpAlarmSettings);
                    return;
                }

                _tpAlarmSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageOutputControl(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageOutputControl),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcOutput.TabPages.Remove(_tpOutputControl);
                    return;
                }

                _tpOutputControl.Enabled = admin;
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
                    _tcOutput.TabPages.Remove(_tpUserFolders);
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
                    _tcOutput.TabPages.Remove(_tpReferencedBy);
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
                    _tcOutput.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        private void SetVisualComponents(bool ccuOutput)
        {
            if (ccuOutput)
            {
                _lParent.Text = GetString("NCASInputEditForm_lCCUName");
                _itbDoorEnvironment.Enabled = false;
                _lDoorEnvironment.Enabled = false;

                if (Insert)
                    Text = GetString("CreateCCUOutput");
            }
            else
            {
                _lParent.Text = GetString("NCASInputEditForm_lDCUName");
                _itbDoorEnvironment.Enabled = true;
                _lDoorEnvironment.Enabled = true;

                if (Insert)
                    Text = GetString("CreateDCUOutput");
            }
        }

        public void SetFixedCCUParent(CCU ccu)
        {
            if (ccu == null)
                return;

            _actCcu = ccu;
            SetOutputNumberInterval();
            _editingObject.CCU = ccu;
            _itbDcuName.Text = ccu.Name;
            _itbDcuName.Image = Plugin.GetImageForAOrmObject(_editingObject.CCU);
            _tbmDCU.Visible = false;
            _itbDcuName.Visible = true;
        }

        protected override void RegisterEvents()
        {
            _eventStateChanged = ChangeState;
            StateChangedOutputHandler.Singleton.RegisterStateChanged(_eventStateChanged);

            _eventRealStateChanged = ChangeRealState;
            RealStateChangedOutputHandler.Singleton.RegisterStateChanged(_eventRealStateChanged);
        }

        protected override void UnregisterEvents()
        {
            StateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventStateChanged);
            RealStateChangedOutputHandler.Singleton.UnregisterStateChanged(_eventRealStateChanged);
        }

        private void ControlAccessRights()
        {
            try
            {
                if (!PresentationGroupsForm.Singleton.HasAccessView())
                {
                    _gbAlarmPresentationGroup.Enabled = false;
                }
            }
            catch
            { }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                if (!_ccuOutput)
                {
                    Text = GetString("CreateDCUOutput");
                }
                else
                    Text = GetString("CreateCCUOutput");
            }
            else
            {
                if (_ccuOutput)
                    Text = GetString("OutputCCUEditFormCCUText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            NCASOutputsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASOutputsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASOutputsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASOutputsForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error = null;
            Output obj = null;

            obj = Plugin.MainServerProvider.Outputs.GetObjectForEdit(_editingObject.IdOutput, out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.Outputs.GetObjectById(_editingObject.IdOutput);
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
                CheckOutputForDoorEnvironment();
            }

            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.Outputs.RenewObjectForEdit(_editingObject.IdOutput, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            LoadOutputType();
            LoadOutputControlType();
            _nudOutputIndex.Left = _eOutputIndex.Left;
            _nudOutputIndex.Visible = true;
            _tbmDCU.Visible = true;
            _eOutputIndex.Visible = false;
            _itbDcuName.Visible = false;
            _bApply.Enabled = false;
        }

        protected override void SetValuesEdit()
        {
            _eName.Text = _editingObject.Name;
            _eDescription.Text = _editingObject.Description;
            if (_editingObject.DCU != null)
            {
                _itbDcuName.Text = _editingObject.DCU.Name;
                _itbDcuName.Image = Plugin.GetImageForAOrmObject(_editingObject.DCU);
            }
            else if (_editingObject.CCU != null)
            {
                _itbDcuName.Text = _editingObject.CCU.Name;
                _itbDcuName.Image = Plugin.GetImageForAOrmObject(_editingObject.CCU);
            }
            _actAlarmPG = _editingObject.AlarmPresentationGroup;
            if (_actAlarmPG != null)
            {
                _tbmAlarmPresentationGroup.Text = _actAlarmPG.GroupName;
                _tbmAlarmPresentationGroup.TextImage = Plugin.GetImageForAOrmObject(_actAlarmPG);
            }

            _tbmDCU.Visible = false;
            _itbDcuName.Visible = true;
            _nudOutputIndex.Visible = false;
            _eOutputIndex.Text = (_editingObject.OutputNumber + 1).ToString();
            _actOnOffObject = _editingObject.OnOffObject;
            SetVisualComponents(_ccuOutput);
            RefreshOnOffObject();

            if (_editingObject.AlarmControlByObjOn)
            {
                _chbAlarmCBOOn.Checked = true;
            }
            else
            {
                _chbAlarmCBOOn.Checked = false;
            }

            LoadOutputType();
            LoadOutputControlType();
            SetTimeFromEditObject();
            _chbForcedToOff.Checked = _editingObject.SettingsForcedToOff;
            _cbInverted.Checked = _editingObject.Inverted;

            ChangeState(_editingObject.IdOutput, (byte)Plugin.MainServerProvider.Outputs.GetActualStates(_editingObject), Guid.Empty);
            ChangeRealState(_editingObject.IdOutput, (byte)Plugin.MainServerProvider.Outputs.GetRealStates(_editingObject), Guid.Empty);
            CheckOutputForDoorEnvironment();
            SetReferencedBy();
            _cbSendRealStateChanges.Checked = _editingObject.RealStateChanges;
            _bApply.Enabled = false;
        }

        private void CheckOutputForDoorEnvironment()
        {
            try
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_editingObject))
                {
                    DisabledFormInDoorEnvironment();
                    ObtainDoorEnvironmentForOutput();
                }
            }
            catch { }
        }

        private void ChangeState(Guid outputGuid, byte state, Guid parent)
        {
            if (outputGuid != _editingObject.IdOutput)
                return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte, Guid>(ChangeState), outputGuid, state, parent);
            }
            else
            {
                if (state == (byte)OutputState.On)
                {
                    _eStatusOnOff.Text = GetString("On");
                }
                else if (state == (byte)OutputState.Off)
                {
                    _eStatusOnOff.Text = GetString("Off");
                }
                else if (state == (byte)OutputState.UsedByAnotherAplication)
                {
                    _eStatusOnOff.Text = GetString("UsedByAnotherAplication");
                }
                else if (state == (byte)OutputState.OutOfRange)
                {
                    _eStatusOnOff.Text = GetString("OutOfRange");
                }
                else
                {
                    _eStatusOnOff.Text = GetString("Unknown");
                }
            }
        }

        private void ChangeRealState(Guid outputGuid, byte state, Guid parent)
        {
            if (outputGuid != _editingObject.IdOutput)
                return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte, Guid>(ChangeRealState), outputGuid, state, parent);
            }
            else
            {
                if (!_editingObject.RealStateChanges)
                {
                    _eRealStatusOnOff.Text = GetString("ReportingSupressed");
                }
                else
                {
                    if (state == (byte)OutputState.On)
                    {
                        _eRealStatusOnOff.Text = GetString("On");
                    }
                    else if (state == (byte)OutputState.Off)
                    {
                        _eRealStatusOnOff.Text = GetString("Off");
                    }
                    else
                    {
                        _eRealStatusOnOff.Text = GetString("ReportingSupressed");
                    }
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
            _itbDcuName.Enabled = true;
            _eDescription.Enabled = true;
            _itbDoorEnvironment.Enabled = true;
            _itbDoorEnvironment.TextBox.Enabled = true;
            _itbDcuName.Enabled = true;
            _itbDcuName.TextBox.Enabled = true;
            //_dgReferencedBy.Enabled = true;
            //_eRoFilterText.Enabled = true;
            //_bRoClear.Enabled = true;
            //_bRoRefresh.Enabled = true;
            //_bRoSearch.Enabled = true;
            _eOutputIndex.Enabled = true;
            EnabledControls(_tpReferencedBy);
        }

        protected override bool GetValues()
        {
            try
            {
                var oldOutputType = _editingObject.OutputType;
                var oldFrocedToOff = _editingObject.SettingsForcedToOff;
                _editingObject.Name = _eName.Text;
                _editingObject.Description = _eDescription.Text;
                _editingObject.OutputType = (byte)(_cbOutPutType.SelectedItem as OutputTypes).Value;
                _editingObject.ControlType = (byte)(_cbOutputControlType.SelectedItem as OutputControlTypes).Value;
                _editingObject.AlarmPresentationGroup = _actAlarmPG;
                
                if (_chbForcedToOff.Checked)
                    _editingObject.SettingsForcedToOff = true;
                else
                {
                    _editingObject.SettingsForcedToOff = false;

                    if (!Insert &&
                        (oldOutputType != _editingObject.OutputType || oldFrocedToOff != _editingObject.SettingsForcedToOff) &&
                        (_editingObject.OutputType == (byte)OutputCharacteristic.pulsed ||
                        _editingObject.OutputType == (byte)OutputCharacteristic.frequency) &&
                        Plugin.MainServerProvider.AlarmAreas.IsOutputUsedAsSiren(_editingObject.IdOutput))
                    {
                        if (Dialog.Question(GetString("QuestionEnableForcedOffOnSirenOutput")))
                        {
                            _editingObject.SettingsForcedToOff = true;
                            _chbForcedToOff.Checked = true;
                        }
                    }
                }

                TimeToEditObject();
                _editingObject.Inverted = _cbInverted.Checked;
                _editingObject.OnOffObject = _actOnOffObject;
                _editingObject.AlarmControlByObjOn = _chbAlarmCBOOn.Checked;
                _editingObject.RealStateChanges = _cbSendRealStateChanges.Checked;
                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

                if (Insert)
                {
                    _editingObject.DCU = _actDcu;
                    _editingObject.CCU = _actCcu;
                    _editingObject.OutputNumber = (byte)(_nudOutputIndex.Value - 1);

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
                        var newName = string.Empty;
                        if (!Plugin.MainServerProvider.GetEnableParentInFullName())
                        {
                            newName = _editingObject.CCU.Name + StringConstants.SLASHWITHSPACES;
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
                    if (!_ccuOutput && _actDcu == null)
                        stringToLocalise = "ErrorSpecifyDcu";
                    else if (_ccuOutput && _actCcu == null)
                        stringToLocalise = "ErrorSpecifyCcu";

                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDCU,
                        GetString(stringToLocalise), ControlNotificationSettings.Default);
                    _tbmDCU.Focus();
                    return false;
                }
                if (OutputIndexUsed())
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _nudOutputIndex,
                        GetString("ErrorOutputIndexUsed"), ControlNotificationSettings.Default);
                    _nudOutputIndex.Focus();
                    return false;
                }
            }

            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                        GetString("ErrorInsertOutputName"), CgpClient.Singleton.ClientControlNotificationSettings);
                _eName.Focus();
                return false;
            }
            if (!(_cbOutPutType.SelectedItem is OutputTypes))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbOutPutType,
                        GetString("ErrorSelectOutputType"), ControlNotificationSettings.Default);
                if (_tcOutput.SelectedTab != _tpSettings)
                {
                    _tcOutput.SelectedTab = _tpSettings;
                }
                _cbOutPutType.Focus();
                return false;
            }
            if (!(_cbOutputControlType.SelectedItem is OutputControlTypes))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbOutputControlType,
                        GetString("ErrorSelectOutputControlType"), ControlNotificationSettings.Default);
                if (_tcOutput.SelectedTab != _tpOutputControl)
                {
                    _tcOutput.SelectedTab = _tpOutputControl;
                }
                _cbOutputControlType.Focus();
                return false;
            }
            if ((_cbOutputControlType.SelectedItem as OutputControlTypes).Value == OutputControl.controledByObject)
            {
                if (_actOnOffObject == null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                        GetString("ErrorSelectControledObject"), ControlNotificationSettings.Default);
                    if (_tcOutput.SelectedTab != _tpOutputControl)
                    {
                        _tcOutput.SelectedTab = _tpOutputControl;
                    }
                    _tbmControlObject.ImageTextBox.Focus();
                    return false;
                }
                if (CheckCrossRelation())
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                        GetString("ErrorControledObjectCrossRelation"), ControlNotificationSettings.Default);
                    if (_tcOutput.SelectedTab != _tpOutputControl)
                    {
                        _tcOutput.SelectedTab = _tpOutputControl;
                    }
                    _tbmControlObject.ImageTextBox.Focus();
                    return false;
                }
            }
            if ((_cbOutPutType.SelectedItem as OutputTypes).Value == OutputCharacteristic.pulsed)
            {
                if (_editSettingsPulseLength.GetMiliSeconds() <= 0)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSettingsPulseLength,
                        GetString("ErrorNotNullPulseLength"), ControlNotificationSettings.Default);
                    if (_tcOutput.SelectedTab != _tpSettings)
                    {
                        _tcOutput.SelectedTab = _tpSettings;
                    }
                    _eSettingsPulseLength.Focus();
                    return false;
                }
            }
            if ((_cbOutPutType.SelectedItem as OutputTypes).Value == OutputCharacteristic.frequency)
            {
                if (_editSettingsPulseLength.GetMiliSeconds() <= 0)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSettingsPulseLength,
                        GetString("ErrorZeroPulseLength"), ControlNotificationSettings.Default);
                    if (_tcOutput.SelectedTab != _tpSettings)
                    {
                        _tcOutput.SelectedTab = _tpSettings;
                    }
                    _eSettingsPulseLength.Focus();
                    return false;
                }
                if (_editSettingsPulseDelay.GetMiliSeconds() <= 0)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSettingsPulseDelay,
                        GetString("ErrorZeroPulseDelay"), ControlNotificationSettings.Default);
                    if (_tcOutput.SelectedTab != _tpSettings)
                    {
                        _tcOutput.SelectedTab = _tpSettings;
                    }
                    _eSettingsPulseDelay.Focus();
                    return false;
                }
            }
            if (!IsValidImpulseDelayValues())
            {
                return false;
            }
            return true;
        }

        private bool CheckCrossRelation()
        {
            Output testOutput;
            testOutput = Plugin.MainServerProvider.Outputs.GetObjectById(_actOnOffObject.GetId());
            while (testOutput != null)
            {
                if (testOutput.IdOutput == _editingObject.IdOutput)
                {
                    return true;
                }

                if (testOutput.OnOffObjectId == null)
                {
                    return false;
                }
                testOutput = Plugin.MainServerProvider.Outputs.GetObjectById(testOutput.OnOffObjectId);
            }
            return false;
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
            var retValue = Plugin.MainServerProvider.Outputs.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message == Output.COLUMNNAME)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedOutputName"), CgpClient.Singleton.ClientControlNotificationSettings);
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
            bool retValue;

            if (onlyInDatabase)
                retValue = Plugin.MainServerProvider.Outputs.UpdateOnlyInDatabase(_editingObject, out error);
            else
                retValue = Plugin.MainServerProvider.Outputs.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message == Input.COLUMNNAME)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorUsedOutputName"), CgpClient.Singleton.ClientControlNotificationSettings);
                    _eName.Focus();
                }
                else
                    throw error;
            }

            return retValue;
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

        protected override void EditEnd()
        {
            if (_editingObject == null) return;
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.Outputs != null)
                Plugin.MainServerProvider.Outputs.EditEnd(_editingObject);
        }
        private void ModifyAlarm()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                ConnectionLost();

                var listModObj = new List<IModifyObject>();

                Exception error;
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
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
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
            }
            else
            {
                _tbmAlarmPresentationGroup.Text = _actAlarmPG.ToString();
                _tbmAlarmPresentationGroup.TextImage = Plugin.GetImageForAOrmObject(_actAlarmPG);
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

        private void SetTimeFromEditObject()
        {
            _editSettingsDelayToOn.SetMiliSeconds(_editingObject.SettingsDelayToOn);
            _editSettingsDelayToOff.SetMiliSeconds(_editingObject.SettingsDelayToOff);
            _editSettingsPulseLength.SetMiliSeconds(_editingObject.SettingsPulseLength);
            _editSettingsPulseDelay.SetMiliSeconds(_editingObject.SettingsPulseDelay);
        }


        private void TimeToEditObject()
        {
            _editingObject.SettingsDelayToOn = _editSettingsDelayToOn.GetMiliSeconds();
            _editingObject.SettingsDelayToOff = _editSettingsDelayToOff.GetMiliSeconds();
            _editingObject.SettingsPulseLength = _editSettingsPulseLength.GetMiliSeconds();
            _editingObject.SettingsPulseDelay = _editSettingsPulseDelay.GetMiliSeconds();
        }

        private void _cbOutPutType_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            if ((_cbOutPutType.SelectedItem as OutputTypes).Value == OutputCharacteristic.level)
            {
                _cbSendRealStateChanges.Enabled = true;
                _lSettingsPulseLength.Visible = false;
                _eSettingsPulseLength.Visible = false;
                _lSettingsPulseDelay.Visible = false;
                _eSettingsPulseDelay.Visible = false;
                _chbForcedToOff.Visible = false;
            }
            else if ((_cbOutPutType.SelectedItem as OutputTypes).Value == OutputCharacteristic.pulsed)
            {
                _cbSendRealStateChanges.Enabled = true;
                _lSettingsPulseLength.Visible = true;
                _eSettingsPulseLength.Visible = true;
                _lSettingsPulseDelay.Visible = false;
                _eSettingsPulseDelay.Visible = false;
                _chbForcedToOff.Visible = true;

                if (_editSettingsPulseLength.GetMiliSeconds() < 50)
                {
                    _editSettingsPulseLength.SetMiliSeconds(50);
                }
                if (_editSettingsPulseLength.GetMiliSeconds() < 200)
                {
                    _cbSendRealStateChanges.Checked = false;
                }

                if (_editSettingsPulseLength.GetMiliSeconds() < 100)
                {
                    _chbForcedToOff.Enabled = false;
                    _chbForcedToOff.Checked = false;
                }
                else
                {
                    _chbForcedToOff.Enabled = true;
                }
            }
            else if ((_cbOutPutType.SelectedItem as OutputTypes).Value == OutputCharacteristic.frequency)
            {
                _cbSendRealStateChanges.Enabled = false;
                _cbSendRealStateChanges.Checked = false;
                _lSettingsPulseLength.Visible = true;
                _eSettingsPulseLength.Visible = true;
                _lSettingsPulseDelay.Visible = true;
                _eSettingsPulseDelay.Visible = true;
                _chbForcedToOff.Visible = true;

                if (_editSettingsPulseLength.GetMiliSeconds() < 200)
                {
                    _editSettingsPulseLength.SetMiliSeconds(200);
                }
                if (_editSettingsPulseDelay.GetMiliSeconds() < 200)
                {
                    _editSettingsPulseDelay.SetMiliSeconds(200);
                }

                if (_editSettingsPulseLength.GetMiliSeconds() < 500)
                {
                    _chbForcedToOff.Enabled = false;
                    _chbForcedToOff.Checked = false;
                }
                else
                {
                    _chbForcedToOff.Enabled = true;
                }
            }

        }

        private void _tbmControlObject_DoubleClick(object sender, EventArgs e)
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

        private void _tbmControlObject_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void ModifyObject()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                ConnectionLost();

                var listModObj = new List<IModifyObject>();
                var listTimeZones = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listTimeZones);

                var listDailyPlansFromDatabase = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateDailyPlans(listDailyPlansFromDatabase);
                listModObj.AddRange(listDailyPlansFromDatabase);

                var listInputsFromDatabase = Plugin.MainServerProvider.Inputs.ListModifyObjectsForOtputControlByObject(_editingObject.IdOutput, out error);
                if (error != null) throw error;
                listModObj.AddRange(listInputsFromDatabase);

                IList<FilterSettings> filterSettings = new List<FilterSettings>();
                var filterSetting = new FilterSettings(Output.COLUMNIDOUTPUT, _editingObject.IdOutput, ComparerModes.NOTEQUALL);
                filterSettings.Add(filterSetting);

                var listOutputsFromDatabase = Plugin.MainServerProvider.Outputs.ListModifyObjectsForOtputControlByObject(_editingObject.IdOutput, filterSettings, out error);
                if (error != null) throw error;
                listModObj.AddRange(listOutputsFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, GetString("NCASOutputEditFormListObjectForAutomaticActivationText"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    _actOnOffObject = null;

                    switch (outModObj.GetOrmObjectType)
                    {
                        case ObjectType.TimeZone:
                            _actOnOffObject = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outModObj.GetId);
                            break;
                        case ObjectType.DailyPlan:
                            _actOnOffObject = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(outModObj.GetId);
                            break;
                        case ObjectType.Input:
                            _actOnOffObject = Plugin.MainServerProvider.Inputs.GetObjectById(outModObj.GetId);
                            break;
                        case ObjectType.Output:
                            _actOnOffObject = Plugin.MainServerProvider.Outputs.GetObjectById(outModObj.GetId);
                            break;
                    }

                    if (_actOnOffObject != null)
                    {
                        Plugin.AddToRecentList(_actOnOffObject);
                    }
                    RefreshOnOffObject();
                }
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private void AddOnOffObject(object newOnOffObject)
        {
            try
            {
                if (newOnOffObject is TimeZone || newOnOffObject is DailyPlan ||
                    newOnOffObject is Input || newOnOffObject is Output)
                {
                    if (!(newOnOffObject is Output && _editingObject.Compare(newOnOffObject)))
                    {
                        if (newOnOffObject is Output)
                        {
                            var output = newOnOffObject as Output;
                            if (output != null)
                            {
                                ConnectionLost();
                                if (!Plugin.MainServerProvider.Outputs.OutputFromTheSameCCUOrInterCCUComunicationEnabledOutputControlObject(_editingObject.IdOutput, output.IdOutput))
                                {
                                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                                        GetString("InterCCUCommunicationNotEnabled"), ControlNotificationSettings.Default);

                                    return;
                                }
                            }
                        }

                        if (newOnOffObject is Input)
                        {
                            var input = newOnOffObject as Input;
                            if (input != null)
                            {
                                ConnectionLost();
                                if (!Plugin.MainServerProvider.Inputs.InputFromTheSameCCUOrInterCCUComunicationEnabledOutputControlObject(_editingObject.IdOutput, input.IdInput))
                                {
                                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                                        GetString("InterCCUCommunicationNotEnabled"), ControlNotificationSettings.Default);

                                    return;
                                }
                            }
                        }

                        SetOnOffObject((AOnOffObject)newOnOffObject);
                        Plugin.AddToRecentList(newOnOffObject);
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmControlObject.ImageTextBox,
                            GetString("ErrorOnOffObjectForAutomaticActivationCanNotBeActualEditingOutput"), ControlNotificationSettings.Default);
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

        private void SetOnOffObject(AOnOffObject onOffobject)
        {
            _actOnOffObject = onOffobject;
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
                if (_actOnOffObject is Input)
                {
                    var input = _actOnOffObject as Input;

                    if (input != null)
                    {
                        if (input.CCU != null)
                        {
                            idCCUObjAutomaticAct = input.CCU.IdCCU;
                        }
                        else if (input.DCU != null && input.DCU.CCU != null)
                        {
                            idCCUObjAutomaticAct = input.DCU.CCU.IdCCU;
                        }
                    }
                }
                else if (_actOnOffObject is Output)
                {
                    var output = _actOnOffObject as Output;

                    if (output.CCU != null)
                    {
                        idCCUObjAutomaticAct = output.CCU.IdCCU;
                    }
                    else if (output.DCU != null && output.DCU.CCU != null)
                    {
                        idCCUObjAutomaticAct = output.DCU.CCU.IdCCU;
                    }
                }

                var idOutputCCU = Guid.Empty;
                if (_editingObject != null)
                {
                    if (_editingObject.CCU != null)
                    {
                        idOutputCCU = _editingObject.CCU.IdCCU;
                    }
                    else if (_editingObject.DCU != null && _editingObject.DCU.CCU != null)
                    {
                        idOutputCCU = _editingObject.DCU.CCU.IdCCU;
                    }
                }

                if (idCCUObjAutomaticAct != Guid.Empty && idOutputCCU != Guid.Empty && idCCUObjAutomaticAct != idOutputCCU)
                {
                    _lICCU.Visible = true;
                }
                else
                {
                    _lICCU.Visible = false;
                }
            }
        }

        private void LoadOutputType()
        {
            _cbOutPutType.Items.Clear();
            var outputTypes = OutputTypes.GetOutputTypesList(LocalizationHelper);

            foreach (var outputType in outputTypes)
            {
                _cbOutPutType.Items.Add(outputType);
            }

            byte outputTypeValue = 0;
            if (!Insert)
            {
                outputTypeValue = _editingObject.OutputType;
            }

            var outputTypeAct = OutputTypes.GetOutputType(LocalizationHelper, outputTypes, outputTypeValue);
            _cbOutPutType.SelectedItem = outputTypeAct;
        }

        private void LoadOutputControlType()
        {
            _cbOutputControlType.Items.Clear();

            if (IsDoorsControl())
            {
                var outputControlTypeActOO = OutputControlTypes.GetOutputControlType(LocalizationHelper, (byte)OutputControl.controledByDoorEnvironment);
                _cbOutputControlType.Items.Add(outputControlTypeActOO);
                _cbOutputControlType.SelectedItem = outputControlTypeActOO;
                //_cbOutputControlType.Enabled = false;
                return;
            }

            if (IsWatchdogOutput())
            {
                var outputControlTypeActWD = OutputControlTypes.GetOutputControlType(LocalizationHelper, (byte)OutputControl.watchdog);
                _cbOutputControlType.Items.Add(outputControlTypeActWD);
                _cbOutputControlType.SelectedItem = outputControlTypeActWD;
                //_cbOutputControlType.Enabled = false;
                return;
            }


            _cbOutputControlType.Enabled = true;
            var outputControlTypes = OutputControlTypes.GetOutputControlTypesList(LocalizationHelper);

            foreach (var outputControlType in outputControlTypes)
            {
                if (outputControlType.Value != OutputControl.controledByDoorEnvironment &&
                    outputControlType.Value != OutputControl.watchdog)
                    _cbOutputControlType.Items.Add(outputControlType);
            }

            byte outputControlTypeValue = 0;
            if (!Insert)
            {
                outputControlTypeValue = _editingObject.ControlType;
            }
            var outputControlTypeAct = OutputControlTypes.GetOutputControlType(LocalizationHelper, outputControlTypes, outputControlTypeValue);
            _cbOutputControlType.SelectedItem = outputControlTypeAct;
        }

        private void _cbOutputControlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            if (_cbOutputControlType.SelectedItem is OutputControlTypes)
            {
                if ((_cbOutputControlType.SelectedItem as OutputControlTypes).Value == OutputControl.controledByObject ||
                    (_cbOutputControlType.SelectedItem as OutputControlTypes).Value == OutputControl.watchdog)
                {
                    _gbControlObject.Enabled = true;
                }
                else
                {
                    _gbControlObject.Enabled = false;
                }
            }
        }

        private bool IsDoorsControl()
        {
            if (_editingObject.ControlType == (byte)OutputControl.controledByDoorEnvironment
                && Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_editingObject))
                return true;

            return false;
        }

        private bool IsWatchdogOutput()
        {
            if (_editingObject.ControlType == (byte)OutputControl.watchdog)
            {
                return true;
            }

            return false;
        }

        private AOrmObject _doorEnvironment;
        private void ObtainDoorEnvironmentForOutput()
        {
            _doorEnvironment = Plugin.MainServerProvider.DoorEnvironments.GetOutputDoorEnvironment(_editingObject.IdOutput);
            if (_doorEnvironment != null)
            {
                _itbDoorEnvironment.Text = _doorEnvironment.ToString();
                _itbDoorEnvironment.Image = Plugin.GetImageForAOrmObject(_doorEnvironment);
            }
        }

        private void _itbDoorEnvironment_DoubleClick(object sender, EventArgs e)
        {
            if (_doorEnvironment != null)
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;

                switch (_doorEnvironment.GetObjectType())
                {
                    case ObjectType.DoorEnvironment:
                        var doorEnvironemnt = (DoorEnvironment) _doorEnvironment;
                        
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
            return Plugin.MainServerProvider.Outputs.
                GetReferencedObjects(_editingObject.IdOutput, CgpClient.Singleton.GetListLoadedPlugins());
        }


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

        private void _tbmControlObject_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify1)
            {
                ModifyObject();
            }
            else if (item == _tsiRemove1)
            {
                SetOnOffObject(null);
            }
        }

        private void _itbDcuName_DoubleClick(object sender, EventArgs e)
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

        private void _eSettingsPulseLength_TextChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            if (!(_cbOutPutType.SelectedItem is OutputTypes)) return;

            switch ((_cbOutPutType.SelectedItem as OutputTypes).Value)
            {
                case OutputCharacteristic.pulsed:
                    {
                        //if (_editSettingsPulseLength.GetMiliSeconds() < 50)
                        //{
                        //    _editSettingsPulseLength.SetMiliSeconds(50);
                        //}

                        if (_editSettingsPulseLength.GetMiliSeconds() < 100)
                        {
                            _chbForcedToOff.Enabled = false;
                            _chbForcedToOff.Checked = false;
                        }
                        else
                        {
                            _chbForcedToOff.Enabled = true;
                        }

                        if (_editSettingsPulseLength.GetMiliSeconds() < 200)
                        {
                            _cbSendRealStateChanges.Checked = false;
                        }
                        break;
                    }
                case OutputCharacteristic.frequency:
                    {
                        //if (_editSettingsPulseLength.GetMiliSeconds() < 200)
                        //{
                        //    _editSettingsPulseLength.SetMiliSeconds(200);
                        //}
                        //if (_editSettingsPulseDelay.GetMiliSeconds() < 200)
                        //{
                        //    _editSettingsPulseDelay.SetMiliSeconds(200);
                        //}

                        if (_editSettingsPulseLength.GetMiliSeconds() < 500)
                        {
                            _chbForcedToOff.Enabled = false;
                            _chbForcedToOff.Checked = false;
                        }
                        else
                        {
                            _chbForcedToOff.Enabled = true;
                        }
                        break;
                    }
            }
        }

        private bool IsValidImpulseDelayValues()
        {
            if (!(_cbOutPutType.SelectedItem is OutputTypes)) return true;

            switch ((_cbOutPutType.SelectedItem as OutputTypes).Value)
            {
                case OutputCharacteristic.pulsed:
                    {
                        if (_editSettingsPulseLength.GetMiliSeconds() < 50)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSettingsPulseLength,
                                GetString("ErrorInvalidPulsedPulseLength"), ControlNotificationSettings.Default);
                            if (_tcOutput.SelectedTab != _tpSettings)
                            {
                                _tcOutput.SelectedTab = _tpSettings;
                            }
                            _eSettingsPulseLength.Focus();
                            return false;
                        }
                        if (_editSettingsPulseLength.GetMiliSeconds() < 100 && _chbForcedToOff.Checked)
                        {
                            _chbForcedToOff.Checked = false;
                        }
                        if (_editSettingsPulseLength.GetMiliSeconds() < 200 && _cbSendRealStateChanges.Checked)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbSendRealStateChanges,
                               GetString("ErrorInvalidPulsedPulseLengthRealState"), ControlNotificationSettings.Default);
                            if (_tcOutput.SelectedTab != _tpSettings)
                            {
                                _tcOutput.SelectedTab = _tpSettings;
                            }
                            _cbSendRealStateChanges.Focus();
                            return false;
                        }
                        break;
                    }
                case OutputCharacteristic.frequency:
                    {
                        if (_editSettingsPulseLength.GetMiliSeconds() < 200)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSettingsPulseLength,
                                GetString("ErrorInvalidFrequencyPulseLength"), ControlNotificationSettings.Default);
                            if (_tcOutput.SelectedTab != _tpSettings)
                            {
                                _tcOutput.SelectedTab = _tpSettings;
                            }
                            _eSettingsPulseLength.Focus();
                            return false;
                        }
                        if (_editSettingsPulseDelay.GetMiliSeconds() < 200)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSettingsPulseDelay,
                                GetString("ErrorInvalidFrequencyPulseLength"), ControlNotificationSettings.Default);
                            if (_tcOutput.SelectedTab != _tpSettings)
                            {
                                _tcOutput.SelectedTab = _tpSettings;
                            }
                            _eSettingsPulseDelay.Focus();
                            return false;
                        }

                        if (_editSettingsPulseLength.GetMiliSeconds() < 500 && _chbForcedToOff.Checked)
                        {
                            _chbForcedToOff.Checked = false;
                        }

                        _cbSendRealStateChanges.Checked = false;
                        break;
                    }
            }
            return true;
        }

        private void _tbmDCU_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var parent = e.Data.GetFormats();
                if (parent == null) return;
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
            if (item.Name == "_tsiModify5")
            {
                ModifyParent();
            }
            else if (item.Name == "_tsiRemove5")
            {
                RemoveParent();
            }
        }

        private void _tbmDCU_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (CcuOutput)
            {
                if (_actCcu != null)
                {
                    if (CgpClient.Singleton.IsConnectionLost(false))
                        return;
                    NCASCCUsForm.Singleton.OpenEditForm(_actCcu);
                }
            }
            else
            {
                if (_actDcu != null)
                {
                    if (CgpClient.Singleton.IsConnectionLost(false))
                        return;
                    NCASDCUsForm.Singleton.OpenEditForm(_actDcu);
                }
            }
        }

        private void AddParent(object newParent)
        {
            try
            {
                CcuOutput = newParent.GetType() == typeof (CCU);

                if (newParent.GetType() != typeof(DCU) && newParent.GetType() != typeof(CCU))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDCU.ImageTextBox,
                       GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
                else
                {
                    if (_ccuOutput)
                    {
                        var ccu = newParent as CCU;
                        if (ccu != null)
                        {
                            if (CcuHasOutputs(ccu))
                            {
                                SetActCcu(ccu);
                                Plugin.AddToRecentList(newParent);
                            }
                        }
                        else
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDCU.ImageTextBox,
                                GetString("ErrorCcuHasNoOutputs"), ControlNotificationSettings.Default);
                        }
                    }
                    else
                    {
                        var dcu = newParent as DCU;
                        if (dcu != null)
                        {
                            if (DcuHasOutputs(dcu))
                            {
                                SetActDcu(dcu);
                                Plugin.AddToRecentList(newParent);
                            }
                        }
                        else
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmDCU.ImageTextBox,
                                GetString("ErrorDcuHasNoOutputs"), ControlNotificationSettings.Default);
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
                
                if (error != null) 
                    throw error;

                var listModObj = new List<IModifyObject>();
                listModObj.AddRange(ccus);
                listModObj.AddRange(dcus);

                var formAdd = new ListboxFormAdd(listModObj, string.Format("{0}, {1}", 
                    GetString("NCASCCUsFormNCASCCUsForm"), GetString("NCASDCUsFormNCASDCUsForm")));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);

                if (outModObj != null)
                {
                    CcuOutput = outModObj.GetOrmObjectType == ObjectType.CCU;

                    if (CcuOutput)
                    {
                        var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(outModObj.GetId);
                        if (!CcuHasOutputs(ccu))
                        {
                            Dialog.Error(GetString("ErrorCcuHasNoOutputs"));
                            return;
                        }
                        SetActCcu(ccu);
                        Plugin.AddToRecentList(_actCcu);
                    }
                    else
                    {

                        var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(outModObj.GetId);
                        if (!DcuHasOutputs(dcu))
                        {
                            Dialog.Error(GetString("ErrorDcuHasNoOutputs"));
                            return;
                        }
                        SetActDcu(dcu);
                        Plugin.AddToRecentList(_actDcu);
                    }
                }
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private bool CcuHasOutputs(CCU ccu)
        {
            if (ccu == null)
                return false;
            if (ccu.OutputsCount != null && ccu.OutputsCount > 0)
                return true;
            if (ccu.Outputs == null || ccu.Outputs.Count == 0)
                return false;
            return true;
        }

        private bool DcuHasOutputs(DCU dcu)
        {
            if (dcu.OutputsCount != null && dcu.OutputsCount > 0)
                return true;
            if (dcu.Outputs.Count > 0)
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
            if (CcuOutput)
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
            SetOutputNumberInterval();
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
            SetOutputNumberInterval();
        }

        private void SetOutputNumberInterval()
        {
            if (_actCcu != null)
            {
                if (_actCcu.OutputsCount != null)
                    _nudOutputIndex.Maximum = (decimal)_actCcu.OutputsCount;
                else
                    _nudOutputIndex.Maximum = _actCcu.Outputs.Count;
            }
            else if (_actDcu != null)
            {
                if (_actDcu.OutputsCount != null)
                {
                    _nudOutputIndex.Maximum = (decimal)_actDcu.OutputsCount;
                }
                else
                {
                    _nudOutputIndex.Maximum = _actDcu.Outputs.Count;
                }
            }
            else
            {
                _nudOutputIndex.Maximum = 1;
            }
        }

        private bool OutputIndexUsed()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return false;
            if (CcuOutput)
                return Plugin.MainServerProvider.Outputs.ExistCcuOutputWithIndex((byte)(_nudOutputIndex.Value - 1), _actCcu);
            return Plugin.MainServerProvider.Outputs.ExistOutputWithIndex((byte)(_nudOutputIndex.Value - 1), _actDcu);
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                _bApply.Enabled = false;
            }
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.OutputsLocalAlarmInstructionsView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.OutputsLocalAlarmInstructionsAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion
    }
}
