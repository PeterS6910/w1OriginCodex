using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.Cgp.Globals;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASMultiDoorElementEditForm :
#if DESIGNER
    Form
#else
    ACgpPluginEditForm<NCASClient, MultiDoorElement>
#endif
    {
        private enum SettingObject : byte
        {
            MultiDoor,
            BlockAlarmArea,
            BlockOnOffObject,
            ElectricStrike,
            ExtraElectricStrike,
            SensorOpenDoor
        }

        private MultiDoor _multiDoor;
        private Floor _floor;
        private AlarmArea _blockAlarmArea;
        private AOrmObject _blockOnOffObject;
        private Input _sensorOpenDoor;
        private Output _electricStrike;
        private Output _extraElectricStrike;
        private Guid? _multiDoorParentCcuId;
        private Guid? _blockAlarmAreaParentCcuId;
        private Guid? _blockOnOffObjectParentCcuId;
        private Guid? _electricStrikeParentCcuId;
        private Guid? _extraElectricStrikeParentCcuId;
        private Guid? _sensorOpenDoorParentCcuId;
        private bool _generateName;

        private Guid? GetParentCcuId(SettingObject settingObject)
        {
            if (settingObject != SettingObject.MultiDoor &&
                _multiDoorParentCcuId != null)
            {
                return _multiDoorParentCcuId;
            }

            if (settingObject != SettingObject.BlockAlarmArea &&
                _blockAlarmAreaParentCcuId != null)
            {
                return _blockAlarmAreaParentCcuId;
            }

            if (settingObject != SettingObject.BlockOnOffObject &&
                _blockOnOffObjectParentCcuId != null)
            {
                return _blockOnOffObjectParentCcuId;
            }

            if (settingObject != SettingObject.ElectricStrike &&
                _electricStrikeParentCcuId != null)
            {
                return _electricStrikeParentCcuId;
            }

            if (settingObject != SettingObject.ExtraElectricStrike &&
                _extraElectricStrikeParentCcuId != null)
            {
                return _extraElectricStrikeParentCcuId;
            }

            if (settingObject != SettingObject.SensorOpenDoor &&
                _sensorOpenDoorParentCcuId != null)
            {
                return _sensorOpenDoorParentCcuId;
            }

            return null;
        }

        public NCASMultiDoorElementEditForm(
                MultiDoorElement multiDoorElement,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                multiDoorElement,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            WheelTabContorol = _tcMultiDoorElement;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            _eDescription.MouseWheel += ControlMouseWheel;
            SetReferenceEditColors();
            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            _bApply.Enabled = false;

            SafeThread.StartThread(HideDisableTabPages);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsSettingsAdmin)));

                HideDisableTabPageApas(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsApasView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsApasAdmin)));

                HideDisableTabPageObjectPlacement(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsDescriptionAdmin)));
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
                    _tcMultiDoorElement.TabPages.Remove(_tpSettings);
                    return;
                }

                _tpSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageApas(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageApas),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcMultiDoorElement.TabPages.Remove(_tpApas);
                    return;
                }

                _tpApas.Enabled = admin;
            }
        }

        private void HideDisableTabPageObjectPlacement(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageObjectPlacement), view);
            }
            else
            {
                if (!view)
                {
                    _tcMultiDoorElement.TabPages.Remove(_tpObjectPlacement);
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
                    _tcMultiDoorElement.TabPages.Remove(_tpReferencedBy);
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
                    _tcMultiDoorElement.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        protected override void BeforeInsert()
        {
            NCASMultiDoorElementsForm.Singleton.BeforeInsert(this);
        }

        protected override void AfterInsert()
        {
            NCASMultiDoorElementsForm.Singleton.AfterInsert();
        }

        protected override void BeforeEdit()
        {
            NCASMultiDoorElementsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterEdit()
        {
            NCASMultiDoorElementsForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj =
                Plugin.MainServerProvider.MultiDoorElements.GetObjectForEdit(
                    _editingObject.IdMultiDoorElement,
                    out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj =
                        Plugin.MainServerProvider.MultiDoorElements.GetObjectById(
                            _editingObject.IdMultiDoorElement);

                    DisableForm();
                }
                else
                {
                    throw error;
                }
            }
            else
            {
                allowEdit = true;
            }

            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.MultiDoorElements.RenewObjectForEdit(
                _editingObject.IdMultiDoorElement,
                out error);

            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            SetMultiDoor(_editingObject.MultiDoor);

            if (_editingObject.MultiDoor != null)
            {
                _eMultiDoorEnvironment.Enabled = false;
            }

            _generateName = true;
            GenerateName();
        }

        private void GenerateName()
        {
            _eName.Text = string.Format(
                "{0} {1}",
                _multiDoor == null || _multiDoor.Type == MultiDoorType.MultiDoor
                    ? GetString("StringDoor")
                    : GetString("StringFloor"),
                (int) _eDoorIndex.Value);
        }

        private void SetMultiDoor(MultiDoor multiDoor)
        {
            EditTextChanger(null, null);
            _multiDoor = multiDoor;

            if (_multiDoor == null ||
                _multiDoor.Type != MultiDoorType.Elevator)
            {
                SetFloor(null);
            }

            if (multiDoor == null)
            {
                _eMultiDoorEnvironment.Text = string.Empty;
                _multiDoorParentCcuId = null;
                _eBlockAlarmArea.Enabled = _floor == null || _floor.BlockAlarmArea == null;
            }
            else
            {
                if (multiDoor.BlockAlarmArea != null)
                {
                    SetBlockAlarmArea(
                        Plugin.MainServerProvider.AlarmAreas.GetObjectById(multiDoor.BlockAlarmArea.IdAlarmArea));

                    _eBlockAlarmArea.Enabled = false;
                }
                else
                {
                    _eBlockAlarmArea.Enabled = _floor == null || _floor.BlockAlarmArea == null;
                }

                _eMultiDoorEnvironment.Text = multiDoor.ToString();
                _eMultiDoorEnvironment.TextImage = Plugin.GetImageForObjectType(ObjectType.MultiDoor.ToString());

                _multiDoorParentCcuId =
                    Plugin.MainServerProvider.MultiDoors.GetParentCcuId(
                        multiDoor.IdMultiDoor);
            }

            if (multiDoor != null &&
                multiDoor.Type == MultiDoorType.Elevator)
            {
                _eFloor.Visible = true;
                _lFloor.Visible = true;
                _eDoorIndex.Minimum = -1000;
            }
            else
            {
                _eFloor.Visible = false;
                _lFloor.Visible = false;
                _eDoorIndex.Minimum = 1;
            }

            if (_generateName)
                GenerateName();
        }

        protected override void SetValuesEdit()
        {
            SetReferencedBy();

            _eName.Text = _editingObject.Name;
            SetMultiDoor(_editingObject.MultiDoor);
            _eMultiDoorEnvironment.Enabled = false;
            SetFloor(_editingObject.Floor);
            _eDoorIndex.Value = _editingObject.DoorIndex;
            SetBlockAlarmArea(_editingObject.BlockAlarmArea);

            if (_editingObject.BlockOnOffObjectType != null &&
                _editingObject.BlockOnOffObjectId != null)
            {
                SetBlockOnOffObject(
                    CgpClient.Singleton.MainServerProvider.GetTableObject(
                        _editingObject.BlockOnOffObjectType.Value,
                        _editingObject.BlockOnOffObjectId.ToString()));
            }
            else
            {
                SetBlockOnOffObject(null);
            }

            SetElectricStrike(_editingObject.ElectricStrike);
            SetExtraElectricStrike(_editingObject.ExtraElectricStrike);
            SetSensorOpenDoor(_editingObject.SensorOpenDoor);

            _eDescription.Text = _editingObject.Description;

            _eActualState.Text = NCASDCUsForm.GetTranslatedDoorEnvironmentState(
                Plugin.MainServerProvider.MultiDoorElements.GetMultiDoorElementState(
                    _editingObject.IdMultiDoorElement),
                LocalizationHelper);

            _bApply.Enabled = false;
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(
                new ReferencedByForm(
                    GetListReferencedObjects,
                    NCASClient.LocalizationHelper,
                    ObjectImageList.Singleton.GetAllObjectImages()));
        }

        private IList<AOrmObject> GetListReferencedObjects()
        {
            return
                Plugin.MainServerProvider.MultiDoorElements
                    .GetReferencedObjects(
                        _editingObject.IdMultiDoorElement,
                        CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void SetFloor(Floor floor)
        {
            EditTextChanger(null, null);
            _floor = floor;

            if (floor == null)
            {
                _eFloor.Text = string.Empty;
                _eDoorIndex.Enabled = true;
                _eBlockAlarmArea.Enabled = _multiDoor == null || _multiDoor.BlockAlarmArea == null;
                return;
            }

            if (floor.BlockAlarmArea != null)
            {
                SetBlockAlarmArea(
                    Plugin.MainServerProvider.AlarmAreas.GetObjectById(floor.BlockAlarmArea.IdAlarmArea));

                _eBlockAlarmArea.Enabled = false;
            }
            else
            {
                _eBlockAlarmArea.Enabled = _multiDoor == null || _multiDoor.BlockAlarmArea == null;
            }

            _eFloor.Text = floor.ToString();
            _eFloor.TextImage = Plugin.GetImageForAOrmObject(floor);
            _eDoorIndex.Enabled = false;
            _eDoorIndex.Value = floor.FloorNumber;
        }

        private void SetBlockAlarmArea(AlarmArea blockAlarmArea)
        {
            EditTextChanger(null, null);
            _blockAlarmArea = blockAlarmArea;

            if (blockAlarmArea == null)
            {
                _eBlockAlarmArea.Text = string.Empty;
                _blockAlarmAreaParentCcuId = null;

                return;
            }

            _eBlockAlarmArea.Text = blockAlarmArea.ToString();
            _eBlockAlarmArea.TextImage = Plugin.GetImageForAOrmObject(blockAlarmArea);

            var ccu = Plugin.MainServerProvider.AlarmAreas.GetImplicitCCUForAlarmArea(blockAlarmArea.IdAlarmArea);
            _blockAlarmAreaParentCcuId = ccu != null ? (Guid?) ccu.IdCCU : null;
        }

        private void SetBlockOnOffObject(AOrmObject ormObject)
        {
            EditTextChanger(null, null);
            _blockOnOffObject = ormObject;

            if (ormObject == null)
            {
                _eBlockOnOffObject.Text = string.Empty;
                _blockOnOffObjectParentCcuId = null;

                return;
            }

            _eBlockOnOffObject.Text = ormObject.ToString();
            _eBlockOnOffObject.TextImage = Plugin.GetImageForAOrmObject(ormObject);

            switch (ormObject.GetObjectType())
            {
                case ObjectType.Input:
                    _blockOnOffObjectParentCcuId =
                        Plugin.MainServerProvider.Inputs.GetParentCCU((Guid) ormObject.GetId());
                    break;
                case ObjectType.Output:
                    _blockOnOffObjectParentCcuId =
                        Plugin.MainServerProvider.Outputs.GetParentCCU((Guid) ormObject.GetId());
                    break;
                default:
                    _blockOnOffObjectParentCcuId = null;
                    break;
            }
        }

        private void SetElectricStrike(Output electricStrike)
        {
            EditTextChanger(null, null);
            _electricStrike = electricStrike;

            if (electricStrike == null)
            {
                _eElectricStrike.Text = string.Empty;
                _electricStrikeParentCcuId = null;

                return;
            }

            _eElectricStrike.Text = electricStrike.ToString();
            _eElectricStrike.TextImage = Plugin.GetImageForAOrmObject(electricStrike);
            _electricStrikeParentCcuId = Plugin.MainServerProvider.Outputs.GetParentCCU(electricStrike.IdOutput);
        }

        private void SetExtraElectricStrike(Output extraElectricStrike)
        {
            EditTextChanger(null, null);
            _extraElectricStrike = extraElectricStrike;

            if (extraElectricStrike == null)
            {
                _eExtraElectricStrike.Text = string.Empty;
                _extraElectricStrikeParentCcuId = null;

                return;
            }

            _eExtraElectricStrike.Text = extraElectricStrike.ToString();
            _eExtraElectricStrike.TextImage = Plugin.GetImageForAOrmObject(extraElectricStrike);
            _extraElectricStrikeParentCcuId = Plugin.MainServerProvider.Outputs.GetParentCCU(extraElectricStrike.IdOutput);
        }

        private void SetSensorOpenDoor(Input sensorOpenDoor)
        {
            EditTextChanger(null, null);
            _sensorOpenDoor = sensorOpenDoor;

            if (sensorOpenDoor == null)
            {
                _eSensorOpenDoor.Text = string.Empty;
                _sensorOpenDoorParentCcuId = null;

                return;
            }

            _eSensorOpenDoor.Text = sensorOpenDoor.ToString();
            _eSensorOpenDoor.TextImage = Plugin.GetImageForAOrmObject(sensorOpenDoor);
            _sensorOpenDoorParentCcuId = Plugin.MainServerProvider.Outputs.GetParentCCU(sensorOpenDoor.IdInput);
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorInsertMultiDoorElementName"), CgpClient.Singleton.ClientControlNotificationSettings);

                _eName.Focus();
                return false;
            }

            if (_multiDoor == null)
            {
                _tcMultiDoorElement.SelectedTab = _tpSettings;

                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eMultiDoorEnvironment.ImageTextBox,
                    GetString("ErrorEntryMultiDoor"), CgpClient.Singleton.ClientControlNotificationSettings);

                _eMultiDoorEnvironment.Focus();
                return false;
            }

            if (_electricStrike == null)
            {
                _tcMultiDoorElement.SelectedTab = _tpApas;

                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eElectricStrike.ImageTextBox,
                    GetString("ErrorEntryElectricStrike"), CgpClient.Singleton.ClientControlNotificationSettings);

                _eElectricStrike.Focus();
                return false;
            }

            return true;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;
                _editingObject.MultiDoor = _multiDoor;
                _editingObject.Floor = _multiDoor.Type == MultiDoorType.Elevator ? _floor : null;
                _editingObject.DoorIndex = (int)_eDoorIndex.Value;
                _editingObject.BlockAlarmArea = _blockAlarmArea;

                if (_blockOnOffObject != null)
                {
                    _editingObject.BlockOnOffObjectType = _blockOnOffObject.GetObjectType();
                    _editingObject.BlockOnOffObjectId = (Guid) _blockOnOffObject.GetId();
                }
                else
                {
                    _editingObject.BlockOnOffObjectType = null;
                    _editingObject.BlockOnOffObjectId = null;
                }

                _editingObject.ElectricStrike = _electricStrike;
                _editingObject.ExtraElectricStrike = _extraElectricStrike;
                _editingObject.SensorOpenDoor = _sensorOpenDoor;

                _editingObject.Description = _eDescription.Text;
                return true;
            }
            catch
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            var retValue = Plugin.MainServerProvider.MultiDoorElements.Insert(ref _editingObject, out error);
            
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains(MultiDoorElement.ColumnElectricStrike))
                    {
                        _tcMultiDoorElement.SelectedTab = _tpApas;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eElectricStrike.ImageTextBox,
                            GetString("ErrorOutputAlreadyUsed"),
                            ControlNotificationSettings.Default);

                        _eElectricStrike.Focus();
                        return false;
                    }

                    if (error.Message.Contains(MultiDoorElement.ColumnExtraElectricStrike))
                    {
                        _tcMultiDoorElement.SelectedTab = _tpApas;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eExtraElectricStrike.ImageTextBox,
                            GetString("ErrorOutputAlreadyUsed"),
                            ControlNotificationSettings.Default);

                        _eExtraElectricStrike.Focus();
                        return false;
                    }

                    if (error.Message.Contains(MultiDoorElement.ColumnSensorOpenDoor))
                    {
                        _tcMultiDoorElement.SelectedTab = _tpApas;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eSensorOpenDoor.ImageTextBox,
                            GetString("ErrorInputAlreadyUsed"),
                            ControlNotificationSettings.Default);

                        _eSensorOpenDoor.Focus();
                        return false;
                    }

                    if (error.Message.Contains(MultiDoorElement.ColumnDoorIndex))
                    {
                        _tcMultiDoorElement.SelectedTab = _tpSettings;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eDoorIndex,
                            _editingObject.MultiDoor.Type == MultiDoorType.MultiDoor
                                ? GetString("ErrorMultiDoorElementWithDoorIndexAlreadyExists")
                                : GetString("ErrorMultiDoorElementWithFloorNumberAlreadyExists"),
                            ControlNotificationSettings.Default);

                        _eDoorIndex.Focus();
                        return false;
                    }
                }

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
                    ? Plugin.MainServerProvider.MultiDoorElements.UpdateOnlyInDatabase(_editingObject, out error)
                    : Plugin.MainServerProvider.MultiDoorElements.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains(MultiDoorElement.ColumnElectricStrike))
                    {
                        _tcMultiDoorElement.SelectedTab = _tpApas;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eElectricStrike.ImageTextBox,
                            GetString("ErrorOutputAlreadyUsed"),
                            ControlNotificationSettings.Default);

                        _eElectricStrike.Focus();
                        return false;
                    }

                    if (error.Message.Contains(MultiDoorElement.ColumnExtraElectricStrike))
                    {
                        _tcMultiDoorElement.SelectedTab = _tpApas;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eExtraElectricStrike.ImageTextBox,
                            GetString("ErrorOutputAlreadyUsed"),
                            ControlNotificationSettings.Default);

                        _eExtraElectricStrike.Focus();
                        return false;
                    }

                    if (error.Message.Contains(MultiDoorElement.ColumnSensorOpenDoor))
                    {
                        _tcMultiDoorElement.SelectedTab = _tpApas;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eSensorOpenDoor.ImageTextBox,
                            GetString("ErrorInputAlreadyUsed"),
                            ControlNotificationSettings.Default);

                        _eSensorOpenDoor.Focus();
                        return false;
                    }

                    if (error.Message.Contains(MultiDoorElement.ColumnDoorIndex))
                    {
                        _tcMultiDoorElement.SelectedTab = _tpSettings;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eDoorIndex,
                            _editingObject.MultiDoor.Type == MultiDoorType.MultiDoor
                                ? GetString("ErrorMultiDoorElementWithDoorIndexAlreadyExists")
                                : GetString("ErrorMultiDoorElementWithFloorNumberAlreadyExists"),
                            ControlNotificationSettings.Default);

                        _eDoorIndex.Focus();
                        return false;
                    }
                }

                throw error;
            }

            return retValue;
        }

        protected override void RegisterEvents()
        {
            DoorEnvironmentStateChangedHandler.Singleton.RegisterStateChanged(StateChanged);
        }

        protected override void UnregisterEvents()
        {
            DoorEnvironmentStateChangedHandler.Singleton.UnregisterStateChanged(StateChanged);
        }

        private void StateChanged(Guid multiDoorElementId, byte state)
        {
            if (Insert ||
                multiDoorElementId != _editingObject.IdMultiDoorElement)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(StateChanged), multiDoorElementId, state);
            }
            else
            {
                _eActualState.Text = NCASDCUsForm.GetTranslatedDoorEnvironmentState(
                    (DoorEnvironmentState) state,
                    LocalizationHelper);
            }
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.MultiDoorElements != null)
                Plugin.MainServerProvider.MultiDoorElements.EditEnd(_editingObject);
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

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

        private void _eName_TextChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
        }

        private void _eName_KeyPress(object sender, KeyPressEventArgs e)
        {
            _generateName = false;
        }

        private void _eDoorIndex_ValueChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);

            if (_generateName)
                GenerateName();
        }

        private void _eMultiDoorEnvironment_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify1)
            {
                ModifyMultiDoor();
                return;
            }

            if (item == _tsiRemove1)
                SetMultiDoor(null);
        }

        private void ModifyMultiDoor()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            Exception error;
            var modifyObjects =
                Plugin.MainServerProvider.MultiDoors.ListModifyObjects(
                    GetParentCcuId(SettingObject.MultiDoor),
                    out error);

            if (modifyObjects != null)
            {
                var formAdd = new ListboxFormAdd(modifyObjects,
                    GetString("NCASMultiDoorsFormNCASMultiDoorsForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    SetMultiDoor(
                        Plugin.MainServerProvider.MultiDoors.GetObjectById(outModObj.GetId));
                }
            }
        }

        private void _eMultiDoorEnvironment_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_multiDoor != null)
            {
                NCASMultiDoorsForm.Singleton.OpenEditForm(_multiDoor);
            }
        }

        private void _eMultiDoorEnvironment_ImageTextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eMultiDoorEnvironment_ImageTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddMultiDoor(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddMultiDoor(object newMultiDoor)
        {
            var multiDoor = newMultiDoor as MultiDoor;

            if (multiDoor == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eMultiDoorEnvironment.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);

                return;
            }

            var parentCcuId = GetParentCcuId(SettingObject.MultiDoor);

            if (parentCcuId != null &&
                Plugin.MainServerProvider.MultiDoors.GetParentCcuId(
                    multiDoor.IdMultiDoor) != parentCcuId)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eMultiDoorEnvironment.ImageTextBox,
                    GetString("ErrorSetMultiDoorFromAnotherCCU"),
                    ControlNotificationSettings.Default);

                return;
            }

            SetMultiDoor(multiDoor);
            Plugin.AddToRecentList(multiDoor);
        }

        private void _eFloor_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify2)
            {
                ModifyFloor();
                return;
            }

            if (item == _tsiRemove2)
                SetFloor(null);
        }

        private void ModifyFloor()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            Exception error;
            var modifyObjects =
                Plugin.MainServerProvider.Floors.ListModifyObjects(out error);

            if (modifyObjects != null)
            {
                var formAdd = new ListboxFormAdd(modifyObjects,
                    GetString("NCASFloorsFormNCASFloorsForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    SetFloor(
                        Plugin.MainServerProvider.Floors.GetObjectById(outModObj.GetId));
                }
            }
        }

        private void _eFloor_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_floor != null)
            {
                NCASFloorsForm.Singleton.OpenEditForm(_floor);
            }
        }

        private void _eFloor_ImageTextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eFloor_ImageTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddFloor(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddFloor(object newFloor)
        {
            var floor = newFloor as Floor;

            if (floor == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eFloor.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);

                return;
            }

            SetFloor(floor);
        }

        private void _eBlockAlarmArea_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify3)
            {
                ModifyBlockAlarmArea();
                return;
            }

            if (item == _tsiRemove3)
            {
                SetBlockAlarmArea(null);
            }
        }

        private void ModifyBlockAlarmArea()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            Exception error;
            var modifyObjects =
                Plugin.MainServerProvider.AlarmAreas.ListModifyObjects(
                    GetParentCcuId(SettingObject.BlockAlarmArea),
                    out error);

            if (modifyObjects != null)
            {
                var formAdd = new ListboxFormAdd(modifyObjects,
                    GetString("NCASAlarmAreasFormNCASAlarmAreasForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    SetBlockAlarmArea(
                        Plugin.MainServerProvider.AlarmAreas.GetObjectById(outModObj.GetId));
                }
            }
        }

        private void _eBlockAlarmArea_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_blockAlarmArea != null)
            {
                NCASAlarmAreasForm.Singleton.OpenEditForm(_blockAlarmArea);
            }
        }

        private void _eBlockAlarmArea_ImageTextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eBlockAlarmArea_ImageTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddBlockAlarmArea(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddBlockAlarmArea(object newBlockAlarmArea)
        {
            var blockAlarmArea = newBlockAlarmArea as AlarmArea;

            if (blockAlarmArea == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eBlockAlarmArea.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);

                return;
            }

            var parentCcuId = GetParentCcuId(SettingObject.BlockAlarmArea);

            if (parentCcuId != null)
            {
                var ccu = Plugin.MainServerProvider.AlarmAreas.GetImplicitCCUForAlarmArea(blockAlarmArea.IdAlarmArea);
                if (ccu == null ||
                    ccu.IdCCU != parentCcuId)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eBlockAlarmArea.ImageTextBox,
                        GetString("ErrorSetAlarmAreaFromAnotherCCUMDE"),
                        ControlNotificationSettings.Default);

                    return;
                }
            }

            SetBlockAlarmArea(blockAlarmArea);
            Plugin.AddToRecentList(blockAlarmArea);
        }

        private void _eBlockOnOffObject_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify4)
            {
                ModifyBlockOnOffObject();
                return;
            }

            if (item == _tsiRemove4)
            {
                SetBlockOnOffObject(null);
            }
        }

        private void ModifyBlockOnOffObject()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            
            Exception error;
            var modifyObjects = Enumerable.Empty<IModifyObject>();

            var timeZones = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);

            if (timeZones != null)
            {
                modifyObjects = modifyObjects.Concat(timeZones);
            }

            var dailyPlans = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);

            if (dailyPlans != null)
            {
                modifyObjects = modifyObjects.Concat(dailyPlans);
            }

            var inputs =
                Plugin.MainServerProvider.Inputs.ListModifyObjectsFromCCU(
                    GetParentCcuId(SettingObject.BlockOnOffObject) ?? Guid.Empty,
                    out error);

            if (inputs != null)
            {
                modifyObjects = modifyObjects.Concat(inputs);
            }

            var outputs =
                Plugin.MainServerProvider.Outputs.ListModifyObjectsFromCCU(
                    GetParentCcuId(SettingObject.BlockOnOffObject) ?? Guid.Empty, out error);

            if (outputs != null)
            {
                modifyObjects = modifyObjects.Concat(outputs);
            }

            var formAdd = new ListboxFormAdd(
                new LinkedList<IModifyObject>(modifyObjects),
                GetString("NCASMultiDoorEditForm_onOffObjects"));

            IModifyObject outModObj;
            formAdd.ShowDialog(out outModObj);
            if (outModObj != null)
            {
                SetBlockOnOffObject(
                    CgpClient.Singleton.MainServerProvider.GetTableObject(
                        outModObj.GetOrmObjectType,
                        outModObj.GetId.ToString()));
            }
        }

        private void _eBlockOnOffObject_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_blockOnOffObject != null)
            {
                DbsSupport.OpenEditForm(_blockOnOffObject);
            }
        }

        private void _eBlockOnOffObject_ImageTextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eBlockOnOffObject_ImageTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddBlockOnOffObject(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddBlockOnOffObject(object newOnOffObject)
        {
            var blockOnOffObject = newOnOffObject as AOrmObject;

            if (blockOnOffObject == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eBlockOnOffObject.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);

                return;
            }

            var objectType = blockOnOffObject.GetObjectType();

            if (objectType != ObjectType.TimeZone &&
                objectType != ObjectType.DailyPlan &&
                objectType != ObjectType.Input &&
                objectType != ObjectType.Output)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eBlockOnOffObject.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);

                return;
            }

            var parentCcuId = GetParentCcuId(SettingObject.BlockAlarmArea);

            if (parentCcuId != null)
            {
                bool isFromSameCcu;

                switch (objectType)
                {
                    case ObjectType.Input:
                        isFromSameCcu = parentCcuId ==
                                        Plugin.MainServerProvider.Inputs.GetParentCCU((Guid) blockOnOffObject.GetId());
                        break;
                    case ObjectType.Output:
                        isFromSameCcu = parentCcuId ==
                                        Plugin.MainServerProvider.Outputs.GetParentCCU((Guid) blockOnOffObject.GetId());
                        break;
                    default:
                        isFromSameCcu = true;
                        break;
                }

                if (!isFromSameCcu)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eBlockOnOffObject.ImageTextBox,
                        GetString("ErrorSetOnOffObjectFromAnotherCCU"),
                        ControlNotificationSettings.Default);

                    return;
                }
            }

            SetBlockOnOffObject(blockOnOffObject);
            Plugin.AddToRecentList(blockOnOffObject);
        }

        private void _eElectricStrike_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify5)
            {
                ModifyElectricStrike();
                return;
            }

            if (item == _tsiRemove5)
                SetElectricStrike(null);
        }

        private void ModifyElectricStrike()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            Exception error;
            var modifyObjects = Plugin.MainServerProvider.MultiDoorElements.ListNotUsedOutputsModifyObjects(
                Insert ? null : (Guid?) _editingObject.IdMultiDoorElement,
                GetParentCcuId(SettingObject.ElectricStrike),
                GetUsedGuids(SettingObject.ElectricStrike),
                out error);

            if (modifyObjects != null)
            {
                var formAdd = new ListboxFormAdd(
                    modifyObjects,
                    GetString("NCASOutputsFormNCASOutputsForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    SetElectricStrike(Plugin.MainServerProvider.Outputs.GetObjectById(outModObj.GetId));
                }
            }
        }

        private void _eElectricStrike_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_electricStrike != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_electricStrike);
            }
        }

        private void _eElectricStrike_ImageTextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eElectricStrike_ImageTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddElectricStrike(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddElectricStrike(object newElectricStrike)
        {
            var electricStrike = newElectricStrike as Output;

            if (electricStrike == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eElectricStrike.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);

                return;
            }

            var parentCcuId = GetParentCcuId(SettingObject.ElectricStrike);

            if (parentCcuId != null &&
                Plugin.MainServerProvider.Outputs.GetParentCCU(
                    electricStrike.IdOutput) != parentCcuId)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eElectricStrike.ImageTextBox,
                    GetString("ErrorSetElectricStrikeFromAnotherCCU"),
                    ControlNotificationSettings.Default);

                return;
            }

            if (Plugin.MainServerProvider.MultiDoorElements.IsOutputUsedInDoorElement(
                electricStrike.IdOutput,
                Insert ? null : (Guid?) _editingObject.IdMultiDoorElement,
                GetUsedGuids(SettingObject.ElectricStrike)))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eElectricStrike.ImageTextBox,
                    GetString("ErrorOutputAlreadyUsed"),
                    ControlNotificationSettings.Default);

                return;
            }

            SetElectricStrike(electricStrike);
            Plugin.AddToRecentList(electricStrike);
        }

        private IEnumerable<Guid> GetUsedGuids(SettingObject settingObject)
        {
            var usedGuids = new LinkedList<Guid>();

            switch (settingObject)
            {
               case SettingObject.ElectricStrike:
                {
                    var onOffObjectGuid = GetBlockOnOffObjectGuid(ObjectType.Output);
                    if (onOffObjectGuid != null)
                        usedGuids.AddLast(onOffObjectGuid.Value);

                    if (_extraElectricStrike != null)
                        usedGuids.AddLast(_extraElectricStrike.IdOutput);

                    break;
                }

                case SettingObject.ExtraElectricStrike:
                {
                    var onOffObjectGuid = GetBlockOnOffObjectGuid(ObjectType.Output);
                    if (onOffObjectGuid != null)
                        usedGuids.AddLast(onOffObjectGuid.Value);

                    if (_electricStrike != null)
                        usedGuids.AddLast(_electricStrike.IdOutput);

                    break;
                }

                case SettingObject.SensorOpenDoor:
                {
                    var onOffObjectGuid = GetBlockOnOffObjectGuid(ObjectType.Input);
                    if (onOffObjectGuid != null)
                        usedGuids.AddLast(onOffObjectGuid.Value);

                    break;
                }
            }

            return usedGuids;
        }

        private Guid? GetBlockOnOffObjectGuid(ObjectType objectType)
        {
            if (_blockOnOffObject == null)
                return null;

            if (_blockOnOffObject.GetObjectType() != objectType)
                return null;

            return (Guid) _blockOnOffObject.GetId();
        }

        private void _eExtraElectricStrike_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify6)
            {
                ModifyExtraElectricStrike();
                return;
            }

            if (item == _tsiRemove6)
                SetExtraElectricStrike(null);
        }

        private void ModifyExtraElectricStrike()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            Exception error;
            var modifyObjects = Plugin.MainServerProvider.MultiDoorElements.ListNotUsedOutputsModifyObjects(
                Insert ? null : (Guid?)_editingObject.IdMultiDoorElement,
                GetParentCcuId(SettingObject.ExtraElectricStrike),
                GetUsedGuids(SettingObject.ExtraElectricStrike),
                out error);

            if (modifyObjects != null)
            {
                var formAdd = new ListboxFormAdd(
                    modifyObjects,
                    GetString("NCASOutputsFormNCASOutputsForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    SetExtraElectricStrike(Plugin.MainServerProvider.Outputs.GetObjectById(outModObj.GetId));
                }
            }
        }

        private void _eExtraElectricStrike_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_extraElectricStrike != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_extraElectricStrike);
            }
        }

        private void _eExtraElectricStrike_ImageTextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eExtraElectricStrike_ImageTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddExtraElectricStrike(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddExtraElectricStrike(object newExtraElectricStrike)
        {
            var extraElectricStrike = newExtraElectricStrike as Output;

            if (extraElectricStrike == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eExtraElectricStrike.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);

                return;
            }

            var parentCcuId = GetParentCcuId(SettingObject.ExtraElectricStrike);

            if (parentCcuId != null &&
                Plugin.MainServerProvider.Outputs.GetParentCCU(
                    extraElectricStrike.IdOutput) != parentCcuId)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eExtraElectricStrike.ImageTextBox,
                    GetString("ErrorSetExtraElectricStrikeFromAnotherCCU"),
                    ControlNotificationSettings.Default);

                return;
            }

            if (Plugin.MainServerProvider.MultiDoorElements.IsOutputUsedInDoorElement(
                extraElectricStrike.IdOutput,
                Insert ? null : (Guid?)_editingObject.IdMultiDoorElement,
                GetUsedGuids(SettingObject.ExtraElectricStrike)))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eExtraElectricStrike.ImageTextBox,
                    GetString("ErrorOutputAlreadyUsed"),
                    ControlNotificationSettings.Default);

                return;
            }

            SetExtraElectricStrike(extraElectricStrike);
            Plugin.AddToRecentList(extraElectricStrike);
        }

        private void _eSensorOpenDoor_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify7)
            {
                ModifySensorOpenDoor();
                return;
            }

            if (item == _tsiRemove7)
                SetSensorOpenDoor(null);
        }

        private void ModifySensorOpenDoor()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            Exception error;
            var modifyObjects = Plugin.MainServerProvider.MultiDoorElements.ListNotUsedInputsModifyObjects(
                Insert ? null : (Guid?)_editingObject.IdMultiDoorElement,
                GetParentCcuId(SettingObject.SensorOpenDoor),
                GetUsedGuids(SettingObject.SensorOpenDoor),
                out error);

            if (modifyObjects != null)
            {
                var formAdd = new ListboxFormAdd(
                    modifyObjects,
                    GetString("NCASInputsFormNCASInputsForm"));

                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    SetSensorOpenDoor(Plugin.MainServerProvider.Inputs.GetObjectById(outModObj.GetId));
                }
            }
        }

        private void _eSensorOpenDoor_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_sensorOpenDoor != null)
            {
                NCASInputsForm.Singleton.OpenEditForm(_sensorOpenDoor);
            }
        }

        private void _eSensorOpenDoor_ImageTextBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eSensorOpenDoor_ImageTextBox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddSensorOpenDoor(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddSensorOpenDoor(object newSensorOpenDoor)
        {
            var sensorOpenDoor = newSensorOpenDoor as Input;

            if (sensorOpenDoor == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eSensorOpenDoor.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);

                return;
            }

            var parentCcuId = GetParentCcuId(SettingObject.SensorOpenDoor);

            if (parentCcuId != null &&
                Plugin.MainServerProvider.Inputs.GetParentCCU(
                    sensorOpenDoor.IdInput) != parentCcuId)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eSensorOpenDoor.ImageTextBox,
                    GetString("ErrorSetSensorOpendDoorFromAnotherCCU"),
                    ControlNotificationSettings.Default);

                return;
            }

            if (Plugin.MainServerProvider.MultiDoorElements.IsInputUsedInDoorElement(
                sensorOpenDoor.IdInput,
                Insert ? null : (Guid?)_editingObject.IdMultiDoorElement,
                GetUsedGuids(SettingObject.SensorOpenDoor)))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eSensorOpenDoor.ImageTextBox,
                    GetString("ErrorInputAlreadyUsed"),
                    ControlNotificationSettings.Default);

                return;
            }

            SetSensorOpenDoor(sensorOpenDoor);
            Plugin.AddToRecentList(sensorOpenDoor);
        }

        private void _tpObjectPlacement_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _eDescription_TextChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
                Text = GetString("NCASMultiDoorElementEditFormInsertText");

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);

            if (!Insert)
            {
                _eActualState.Text = NCASDCUsForm.GetTranslatedDoorEnvironmentState(
                    Plugin.MainServerProvider.MultiDoorElements.GetMultiDoorElementState(
                        _editingObject.IdMultiDoorElement),
                    LocalizationHelper);
            }
        }
    }
}
