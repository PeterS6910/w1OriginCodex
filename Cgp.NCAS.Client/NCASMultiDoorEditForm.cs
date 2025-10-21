using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASMultiDoorEditForm :
#if DESIGNER
    Form
#else
    ACgpPluginEditForm<NCASClient, MultiDoor>
#endif
    {
        private enum SettingObject : byte
        {
            CardReader,
            BlockAlarmArea,
        }

        private CardReader _cardReader;
        private AlarmArea _blockAlarmArea;
        private readonly ICollection<MultiDoorTypeItem> _multiDoorTypeItems;
        private BindingSource _bsMultiDoorElements;
        private Guid? _parentCcuId;
        private Guid? _cardReaderParentCcuId;
        private Guid? _blockAlarmAreaParentCcuId;

        private Guid? GetParentCcuId(SettingObject settingObject)
        {
            if (_parentCcuId != null)
                return _parentCcuId;

            if (settingObject != SettingObject.CardReader &&
                _cardReaderParentCcuId != null)
            {
                return _cardReaderParentCcuId;
            }

            if (settingObject != SettingObject.BlockAlarmArea &&
                _blockAlarmAreaParentCcuId != null)
            {
                return _blockAlarmAreaParentCcuId;
            }

            return null;
        }

        public NCASMultiDoorEditForm(
                MultiDoor multiDoor,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                multiDoor,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            WheelTabContorol = _tcMultiDoor;
            _cbType.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            _eDescription.MouseWheel += ControlMouseWheel;
            SetReferenceEditColors();
            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            _bApply.Enabled = false;

            _multiDoorTypeItems = MultiDoorTypeItem.GetMultiDoorTypeItems();

            _dgMultiDoorElements.LocalizationHelper = NCASClient.LocalizationHelper;
            _dgMultiDoorElements.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _dgMultiDoorElements.DataGrid.MouseDoubleClick += _dgMultiDoorElements_MouseDoubleClick;
            _dgMultiDoorElements.BeforeGridModified += _dgMultiDoorElements_BeforeGridModified;

            SafeThread.StartThread(HideDisableTabPages);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsSettingsAdmin)));

                HideDisableTabPageApas(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsApasView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsApasAdmin)));

                HideDisableTabPageDoors(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsDoorsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsDoorsAdmin)));

                HideDisableTabPageObjectPlacement(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.MultiDoorsDescriptionAdmin)));
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
                    _tcMultiDoor.TabPages.Remove(_tpSettings);
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
                    _tcMultiDoor.TabPages.Remove(_tpApas);
                    return;
                }

                _tpApas.Enabled = admin;
            }
        }

        private void HideDisableTabPageDoors(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDoors),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcMultiDoor.TabPages.Remove(_tpDoors);
                    return;
                }

                _bCreate.Enabled = admin;
                _bDelete.Enabled = admin;
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
                    _tcMultiDoor.TabPages.Remove(_tpObjectPlacement);
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
                    _tcMultiDoor.TabPages.Remove(_tpReferencedBy);
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
                    _tcMultiDoor.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        private void TranslateMultiDoorElementsState()
        {
            TranslateMultiDoorElementsState(null, DoorEnvironmentState.Unknown);
        }

        private void TranslateMultiDoorElementsState(Guid? multiDoorElementId, DoorEnvironmentState state)
        {
            var bindingSource = _dgMultiDoorElements.DataGrid.DataSource as BindingSource;

            if (bindingSource == null)
                return;

            foreach (MultiDoorElementShort multiDoorElementShort in bindingSource.List)
            {
                if (multiDoorElementId != null)
                {
                    if (multiDoorElementShort.IdMultiDoorElement != multiDoorElementId)
                        continue;

                    multiDoorElementShort.State = state;
                    multiDoorElementShort.StringState = NCASDCUsForm.GetTranslatedDoorEnvironmentState(
                        multiDoorElementShort.State,
                        LocalizationHelper);

                    return;
                }

                multiDoorElementShort.StringState = NCASDCUsForm.GetTranslatedDoorEnvironmentState(
                    multiDoorElementShort.State,
                    LocalizationHelper);
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
                Text = GetString("NCASMultiDoorEditFormInsertText");

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);

            var selectedMultiDoorTypeItem = _cbType.SelectedItem as MultiDoorTypeItem;

            _cbType.Items.Clear();

            foreach (var multiDoorTypeItem in _multiDoorTypeItems)
            {
                multiDoorTypeItem.Translate(LocalizationHelper);
            }

            _cbType.Items.AddRange(_multiDoorTypeItems.Cast<object>().ToArray());

            if (selectedMultiDoorTypeItem != null)
            {
                _cbType.SelectedItem = selectedMultiDoorTypeItem;
            }

            TranslateMultiDoorElementsState();
        }

        protected override void BeforeInsert()
        {
            NCASMultiDoorsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASMultiDoorsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASMultiDoorsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASMultiDoorsForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj =
                Plugin.MainServerProvider.MultiDoors.GetObjectForEdit(
                    _editingObject.IdMultiDoor,
                    out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj =
                        Plugin.MainServerProvider.MultiDoors.GetObjectById(
                            _editingObject.IdMultiDoor);

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
            Plugin.MainServerProvider.MultiDoors.RenewObjectForEdit(
                _editingObject.IdMultiDoor,
                out error);

            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            if (_tcMultiDoor.TabPages.Contains(_tpDoors))
                _tcMultiDoor.TabPages.Remove(_tpDoors);
        }

        protected override void SetValuesEdit()
        {
            SetReferencedBy();

            _eName.Text = _editingObject.Name;
            SetCardReader(_editingObject.CardReader);
            SetBlockAlarmArea(_editingObject.BlockAlarmArea);

            SetMultiDoorTypeItem(_editingObject.Type);
            _cbType.Enabled = false;

            _eTimeUnlock.Value = _editingObject.DoorTimeUnlock;
            _eTimeOpen.Value = _editingObject.DoorTimeOpen;
            _eTimePreAlarm.Value = _editingObject.DoorTimePreAlarm;
            _eDelayBeforeUnlock.Value = _editingObject.DoorDelayBeforeUnlock;
            _eDelayBeforeClose.Value = _editingObject.DoorDelayBeforeClose;
            _eDelayBeforeLock.Value = _editingObject.DoorDelayBeforeLock;

            _chbElectricStrikeImpulse.Checked = _editingObject.ElectricStrikeImpulse;
            _eElectricStrikeImpulseDelay.Value = _editingObject.ElectricStrikeImpulseDelay;

            _chbExtraElectricStrikeImpulse.Checked = _editingObject.ExtraElectricStrikeImpulse;
            _eExtraElectricStrikeImpulseDelay.Value = _editingObject.ExtraElectricStrikeImpulseDelay;

            _chbOpenDoorInverted.Checked = _editingObject.SensorsOpenDoorsInverted;
            _chbOpenDoorBalanced.Checked = _editingObject.SensorsOpenDoorsBalanced;

            _eDescription.Text = _editingObject.Description;

            ShowMultiDoorElements(_editingObject.Doors);

            _parentCcuId = Plugin.MainServerProvider.CardReaders.GetParentCCU(_editingObject.CardReader.IdCardReader);

            _bApply.Enabled = false;
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
            _blockAlarmAreaParentCcuId = ccu != null ? (Guid?)ccu.IdCCU : null;
        }

        private void ShowMultiDoorElements(IEnumerable<MultiDoorElement> multiDoorElements)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IEnumerable<MultiDoorElement>>(ShowMultiDoorElements), multiDoorElements);
                return;
            }

            if (multiDoorElements == null)
            {
                _dgMultiDoorElements.DataGrid.DataSource = null;
                return;
            }

            multiDoorElements = _editingObject.Type == MultiDoorType.Elevator
                ? multiDoorElements.OrderBy(multiDoorElement => multiDoorElement.DoorIndex)
                : multiDoorElements.OrderBy(multiDoorElement => multiDoorElement.ToString());

            _bsMultiDoorElements = new BindingSource
            {
                DataSource =
                    new SortableBindingList<MultiDoorElementShort>(
                        multiDoorElements.Select(
                            multiDoorElement =>
                                new MultiDoorElementShort(multiDoorElement, false)))
            };

            _dgMultiDoorElements.ModifyGridView(
                _bsMultiDoorElements,
                MultiDoorElementShort.ColumnSymbol,
                MultiDoorElementShort.ColumnName,
                MultiDoorElementShort.ColumnDoorIndex,
                MultiDoorElementShort.ColumnStringState,
                MultiDoorElementShort.ColumnDescription);

            var columnSymbol = _dgMultiDoorElements.DataGrid.Columns[MultiDoorElementShort.ColumnSymbol];
            if (columnSymbol != null)
                columnSymbol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void _dgMultiDoorElements_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (MultiDoorElementShort multiDoorElementShort in bindingSource.List)
            {
                multiDoorElementShort.Symbol = _dgMultiDoorElements.GetDefaultImage(multiDoorElementShort);

                multiDoorElementShort.State =
                    Plugin.MainServerProvider.MultiDoorElements.GetMultiDoorElementState(
                        multiDoorElementShort.IdMultiDoorElement);

                multiDoorElementShort.StringState = NCASDCUsForm.GetTranslatedDoorEnvironmentState(
                    multiDoorElementShort.State,
                    LocalizationHelper);
            }
        }

        private void _dgMultiDoorElements_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _bEdit_Click(sender, e);
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
                Plugin.MainServerProvider.MultiDoors
                    .GetReferencedObjects(
                        _editingObject.IdMultiDoor,
                        CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void SetMultiDoorTypeItem(MultiDoorType type)
        {
            foreach (var multiDoorTypeItem in _multiDoorTypeItems)
            {
                if (multiDoorTypeItem.Type == type)
                {
                    _cbType.SelectedItem = multiDoorTypeItem;
                    break;
                }
            }
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorInsertMultiDoorName"), CgpClient.Singleton.ClientControlNotificationSettings);
                
                _eName.Focus();
                return false;
            }

            if (_cardReader == null)
            {
                _tcMultiDoor.SelectedTab = _tpSettings;

                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCardReader.ImageTextBox,
                    GetString("ErrorEntryCardReader"), CgpClient.Singleton.ClientControlNotificationSettings);
                
                _eCardReader.Focus();
                return false;
            }

            if (_cbType.SelectedItem as MultiDoorTypeItem == null)
            {
                _tcMultiDoor.SelectedTab = _tpSettings;

                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cbType,
                    GetString("ErrorEntryMultiDoorType"), CgpClient.Singleton.ClientControlNotificationSettings);
                
                _cbType.Focus();
                return false;
            }

            return true;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;
                _editingObject.CardReader = _cardReader;
                _editingObject.BlockAlarmArea = _blockAlarmArea;

                var multiDoorTypeItem = _cbType.SelectedItem as MultiDoorTypeItem;
                if (multiDoorTypeItem != null)
                {
                    _editingObject.Type = multiDoorTypeItem.Type;
                }

                _editingObject.DoorTimeUnlock = (byte)_eTimeUnlock.Value;
                _editingObject.DoorTimeOpen = (int)_eTimeOpen.Value;
                _editingObject.DoorTimePreAlarm = (byte)_eTimePreAlarm.Value;
                _editingObject.DoorDelayBeforeUnlock = (int)_eDelayBeforeUnlock.Value;
                _editingObject.DoorDelayBeforeClose = (int)_eDelayBeforeClose.Value;
                _editingObject.DoorDelayBeforeLock = (int)_eDelayBeforeLock.Value;

                _editingObject.ElectricStrikeImpulse = _chbElectricStrikeImpulse.Checked;
                _editingObject.ElectricStrikeImpulseDelay = (int)_eElectricStrikeImpulseDelay.Value;

                _editingObject.ExtraElectricStrikeImpulse = _chbExtraElectricStrikeImpulse.Checked;
                _editingObject.ExtraElectricStrikeImpulseDelay = (int)_eExtraElectricStrikeImpulseDelay.Value;

                _editingObject.SensorsOpenDoorsInverted = _chbOpenDoorInverted.Checked;
                _editingObject.SensorsOpenDoorsBalanced = _chbOpenDoorBalanced.Checked;

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
            var retValue = Plugin.MainServerProvider.MultiDoors.Insert(ref _editingObject, out error);
            
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains(MultiDoor.ColumnName))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eName,
                            GetString("ErrorUsedMultiDoorName"),
                            CgpClient.Singleton.ClientControlNotificationSettings);

                        _eName.Focus();
                        return false;
                    }

                    if (error.Message.Contains(MultiDoor.ColumnCardReader))
                    {
                        _tcMultiDoor.SelectedTab = _tpSettings;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eCardReader.ImageTextBox,
                            GetString("ErrorCardReaderAlreadyUsed"),
                            ControlNotificationSettings.Default);

                        _eCardReader.Focus();
                        return false;
                    }
                }

                throw error;
            }

            if (retValue &&
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsInsertDeletePerform)))
            {
                if (_editingObject.Type == MultiDoorType.MultiDoor)
                {
                    CreateDoorsForMultiDoorForm.ShowDialog(
                        _editingObject.IdMultiDoor,
                        Plugin.MainServerProvider.MultiDoors);
                }
                else
                {
                    CreateDoorsForElevatorForm.ShowDialog(
                        _editingObject.IdMultiDoor,
                        Plugin.MainServerProvider.MultiDoors);
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

            bool retValue =
                onlyInDatabase
                    ? Plugin.MainServerProvider.MultiDoors.UpdateOnlyInDatabase(_editingObject, out error)
                    : Plugin.MainServerProvider.MultiDoors.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains(MultiDoor.ColumnName))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eName,
                            GetString("ErrorUsedMultiDoorName"),
                            CgpClient.Singleton.ClientControlNotificationSettings);

                        _eName.Focus();
                        return false;
                    }

                    if (error.Message.Contains(MultiDoor.ColumnCardReader))
                    {
                        _tcMultiDoor.SelectedTab = _tpSettings;

                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eCardReader.ImageTextBox,
                            GetString("ErrorCardReaderAlreadyUsed"),
                            ControlNotificationSettings.Default);

                        _eCardReader.Focus();
                        return false;
                    }
                }

                throw error;
            }

            return retValue;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.MultiDoors != null)
                Plugin.MainServerProvider.MultiDoors.EditEnd(_editingObject);
        }

        private void StateChanged(Guid multiDoorElementId, byte state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<Guid, byte>(StateChanged),
                    multiDoorElementId,
                    state);
            }
            else
            {
                TranslateMultiDoorElementsState(
                    multiDoorElementId,
                    (DoorEnvironmentState)state);

                _dgMultiDoorElements.Refresh();
            }
        }

        protected override void RegisterEvents()
        {
            DoorEnvironmentStateChangedHandler.Singleton.RegisterStateChanged(StateChanged);
        }

        protected override void UnregisterEvents()
        {
            DoorEnvironmentStateChangedHandler.Singleton.UnregisterStateChanged(StateChanged);
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
            if (!ControlExistingDirectlyCardReaderAclAssigment())
                return;

            if (Apply_Click())
            {
                _bApply.Enabled = false;
            }
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (!ControlExistingDirectlyCardReaderAclAssigment())
                return;

            Ok_Click();
        }

        private bool ControlExistingDirectlyCardReaderAclAssigment()
        {
            if (_cardReader == null)
                return true;

            if (Plugin.MainServerProvider.ACLSettings.ExistDirectlyCardReaderAclAssigment(
                    _cardReader.IdCardReader))
            {
                return Dialog.Question(GetString("QuestionContinueSavingExistDirectlyCrAclAssigment"));
            }

            return true;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void _eCardReader_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify)
            {
                ModifyCardReader();
            }
            else if (item == _tsiRemove)
            {
                SetCardReader(null);
            }
        }

        private void SetCardReader(CardReader cardReader)
        {
            EditTextChanger(null, null);
            _cardReader = cardReader;

            if (cardReader != null)
            {
                _eCardReader.Text = cardReader.ToString();
                _eCardReader.TextImage = Plugin.GetImageForAOrmObject(cardReader);

                _cardReaderParentCcuId = Plugin.MainServerProvider.CardReaders.GetParentCCU(cardReader.IdCardReader);
            }
            else
            {
                _eCardReader.Text = string.Empty;
                _cardReaderParentCcuId = null;
            }
        }

        private void ModifyCardReader()
        {
            Exception error;

            var multiDoorId = !Insert ? (Guid?) _editingObject.IdMultiDoor : null;

            var cardReaders = Plugin.MainServerProvider.MultiDoors.ListNotUsedCardReadersModifyObjects(
                multiDoorId,
                GetParentCcuId(SettingObject.CardReader),
                out error);

            var formAdd = new ListboxFormAdd(cardReaders, GetString("NCASCardReadersFormNCASCardReadersForm"));
            IModifyObject outObject;
            formAdd.ShowDialog(out outObject);

            if (outObject == null)
                return;

            SetCardReader(Plugin.MainServerProvider.CardReaders.GetObjectById(outObject.GetId));
        }

        private void _cbType_SelectedValueChanged(object sender, EventArgs e)
        {
            var multiDoorTypeItem = _cbType.SelectedItem as MultiDoorTypeItem;

            if (multiDoorTypeItem != null &&
                multiDoorTypeItem.Type == MultiDoorType.MultiDoor)
            {
                _lAlarmArea.Visible = true;
                _eBlockAlarmArea.Visible = true;
            }
            else
            {
                _lAlarmArea.Visible = false;
                _eBlockAlarmArea.Visible = false;
                SetBlockAlarmArea(null);
            }

            EditTextChanger(sender, e);
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            var multiDoorElement = new MultiDoorElement {MultiDoor = _editingObject};

            NCASMultiDoorElementsForm.Singleton.OpenInsertFromEdit(ref multiDoorElement, RefreshMultiDoorElements);
        }

        private void RefreshMultiDoorElements(object obj)
        {
            var multiDoorElement = obj as MultiDoorElement;

            if (multiDoorElement == null)
                return;

            if (_editingObject.Doors == null)
                _editingObject.Doors = new LinkedList<MultiDoorElement>();

            _editingObject.Doors.Add(multiDoorElement);

            ShowMultiDoorElements(_editingObject.Doors);
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_bsMultiDoorElements == null)
                return;

            if (_editingObject.Doors == null ||
                _editingObject.Doors.Count == 0)
            {
                return;
            }

            var multiDoorElementShort = _bsMultiDoorElements.List[_bsMultiDoorElements.Position] as MultiDoorElementShort;
            if (multiDoorElementShort == null)
                return;

            var multiDoorElementForEdit =
                _editingObject.Doors.FirstOrDefault(
                    multiDoorElement => multiDoorElement.IdMultiDoorElement == multiDoorElementShort.IdMultiDoorElement);

            if (multiDoorElementForEdit == null)
                return;

            NCASMultiDoorElementsForm.Singleton.OpenEditForm(multiDoorElementForEdit);
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_bsMultiDoorElements == null)
                return;

            if (_editingObject.Doors == null ||
                _editingObject.Doors.Count == 0)
            {
                return;
            }

            var multiDoorElementShort = _bsMultiDoorElements.List[_bsMultiDoorElements.Position] as MultiDoorElementShort;
            if (multiDoorElementShort == null)
                return;

            var multiDoorElementForDelete =
                _editingObject.Doors.FirstOrDefault(
                    multiDoorElement => multiDoorElement.IdMultiDoorElement == multiDoorElementShort.IdMultiDoorElement);

            if (multiDoorElementForDelete == null)
                return;

            _editingObject.Doors.Remove(multiDoorElementForDelete);

            EditTextChanger(null, null);
            ShowMultiDoorElements(_editingObject.Doors);
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _tpObjectPlacement_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _eCardReader_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_cardReader != null)
            {
                NCASCardReadersForm.Singleton.OpenEditForm(_cardReader);
            }
        }

        private void _eCardReader_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _eCardReader_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddCardReader(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddCardReader(object newCardReader)
        {
            try
            {
                var cardReader = newCardReader as CardReader;

                if (cardReader == null)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eCardReader.ImageTextBox,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                var parentCcuId = GetParentCcuId(SettingObject.CardReader);

                if (parentCcuId != null &&
                    parentCcuId != Plugin.MainServerProvider.CardReaders.GetParentCCU(cardReader.IdCardReader))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eCardReader.ImageTextBox,
                        GetString("ErrorSetCardReaderFromAnotherCCU"),
                        ControlNotificationSettings.Default);

                    return;
                }

                if (Plugin.MainServerProvider.MultiDoors.IsCardReaderUsedInDoorElement(
                    cardReader.IdCardReader,
                    Insert ? null : (Guid?) _editingObject.IdMultiDoor))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eCardReader.ImageTextBox,
                        GetString("ErrorCardReaderAlreadyUsed"),
                        ControlNotificationSettings.Default);

                    return;
                }

                SetCardReader(cardReader);
                Plugin.AddToRecentList(cardReader);
            }
            catch
            {
            }
        }

        private void _eTimeUnlock_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
        }

        private void _eTimeOpen_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
        }

        private void _eTimePreAlarm_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
        }

        private void _eDelayBeforeUnlock_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
        }

        private void _eDelayBeforeLock_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
        }

        private void _eDelayBeforeClose_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
        }

        private void _chbElectricStrikeImpulse_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
        }

        private void _eElectricStrikeImpulseDelay_ValueChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
        }

        private void _chbExtraElectricStrikeImpulse_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
        }

        private void _eExtraElectricStrikeImpulseDelay_ValueChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
        }

        private void _chbOpenDoorInverted_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
        }

        private void _chbOpenDoorBalanced_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
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
                        GetString("ErrorSetAlarmAreaFromAnotherCCUMD"),
                        ControlNotificationSettings.Default);

                    return;
                }
            }

            SetBlockAlarmArea(blockAlarmArea);
            Plugin.AddToRecentList(blockAlarmArea);
        }

        private void _eBlockAlarmArea_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify2)
            {
                ModifyBlockAlarmArea();
                return;
            }

            if (item == _tsiRemove2)
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
    }
}
