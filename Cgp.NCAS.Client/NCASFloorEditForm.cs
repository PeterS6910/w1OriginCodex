using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASFloorEditForm :
#if DESIGNER
    Form
#else
    ACgpPluginEditForm<NCASClient, Floor>
#endif
    {
        private enum SettingObject : byte
        {
            MultiDoorElements,
            BlockAlarmArea,
        }

        private BindingSource _bsMultiDoorElements;
        private AlarmArea _blockAlarmArea;
        private Guid? _multiDoorElementsParentCcuId;
        private Guid? _blockAlarmAreaParentCcuId;
        private bool _generateName;

        private Guid? GetParentCcuId(SettingObject settingObject)
        {
            if (settingObject != SettingObject.MultiDoorElements &&
                _multiDoorElementsParentCcuId != null)
            {
                return _multiDoorElementsParentCcuId;
            }

            if (settingObject != SettingObject.BlockAlarmArea &&
                _blockAlarmAreaParentCcuId != null)
            {
                return _blockAlarmAreaParentCcuId;
            }

            return null;
        }

        public NCASFloorEditForm(
                Floor floor,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                floor,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            WheelTabContorol = _tcFloor;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            _eDescription.MouseWheel += ControlMouseWheel;
            SetReferenceEditColors();
            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            _bApply.Enabled = false;

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
                        NCASAccess.GetAccess(AccessNCAS.FloorsSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.FloorsSettingsAdmin)));

                HideDisableTabPageDoors(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.FloorsDoorsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.FloorsDoorsAdmin)));

                HideDisableTabPageObjectPlacement(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.FloorsReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.FloorsDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.FloorsDescriptionAdmin)));
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
                    _tcFloor.TabPages.Remove(_tpSettings);
                    return;
                }

                _tpSettings.Enabled = admin;
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
                    _tcFloor.TabPages.Remove(_tpDoors);
                    return;
                }

                _bAdd.Enabled = admin;
                _bRemove.Enabled = admin;
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
                    _tcFloor.TabPages.Remove(_tpObjectPlacement);
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
                    _tcFloor.TabPages.Remove(_tpReferencedBy);
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
                    _tcFloor.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        protected override void RegisterEvents()
        {

        }

        protected override void UnregisterEvents()
        {

        }

        protected override void BeforeInsert()
        {
            NCASFloorsForm.Singleton.BeforeInsert(this);
        }

        protected override void AfterInsert()
        {
            NCASFloorsForm.Singleton.AfterInsert();
        }

        protected override void BeforeEdit()
        {
            NCASFloorsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterEdit()
        {
            NCASFloorsForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj =
                Plugin.MainServerProvider.Floors.GetObjectForEdit(
                    _editingObject.IdFloor,
                    out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj =
                        Plugin.MainServerProvider.Floors.GetObjectById(
                            _editingObject.IdFloor);

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
            Plugin.MainServerProvider.Floors.RenewObjectForEdit(
                _editingObject.IdFloor,
                out error);

            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            _generateName = true;
            GenerateName();
        }

        private void GenerateName()
        {
            _eName.Text = string.Format(
                "{0} {1}",
                GetString("StringFloor"),
                (int) _eFloorNumber.Value);
        }

        protected override void SetValuesEdit()
        {
            SetReferencedBy();

            _eName.Text = _editingObject.Name;
            _eFloorNumber.Value = _editingObject.FloorNumber;           
            _eDescription.Text = _editingObject.Description;

            SetBlockAlarmArea(_editingObject.BlockAlarmArea);
            EnableDisableFloorNumber();
            ShowMultiDoorElements(_editingObject.Doors);

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
                Plugin.MainServerProvider.Floors
                    .GetReferencedObjects(
                        _editingObject.IdFloor,
                        CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void EnableDisableFloorNumber()
        {
            if (_editingObject.Doors != null &&
                _editingObject.Doors.Count > 0)
            {
                _eFloorNumber.Enabled = false;
            }
            else
            {
                _eFloorNumber.Enabled = true;
            }
        }

        private void ShowMultiDoorElements(ICollection<MultiDoorElement> multiDoorElements)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ICollection<MultiDoorElement>>(ShowMultiDoorElements), multiDoorElements);
                return;
            }

            if (multiDoorElements == null)
            {
                _dgMultiDoorElements.DataGrid.DataSource = null;
                _multiDoorElementsParentCcuId = null;
                _eBlockAlarmArea.Enabled = true;
                return;
            }

            _bsMultiDoorElements = new BindingSource
            {
                DataSource =
                    new SortableBindingList<MultiDoorElementShort>(
                        multiDoorElements.Select(multiDoorElement => new MultiDoorElementShort(multiDoorElement)))
            };

            _dgMultiDoorElements.ModifyGridView(
                _bsMultiDoorElements,
                MultiDoorElementShort.ColumnSymbol,
                MultiDoorElementShort.ColumnName,
                MultiDoorElementShort.ColumnDescription);

            var columnSymbol = _dgMultiDoorElements.DataGrid.Columns[MultiDoorElementShort.ColumnSymbol];
            if (columnSymbol != null)
                columnSymbol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            _multiDoorElementsParentCcuId = null;
            foreach (var multiDoorElement in multiDoorElements)
            {
                if (_multiDoorElementsParentCcuId == null)
                {
                    _multiDoorElementsParentCcuId =
                        Plugin.MainServerProvider.MultiDoors.GetParentCcuId(multiDoorElement.MultiDoor.IdMultiDoor);

                    continue;
                }

                var multiDoorElementParentCcu =
                    Plugin.MainServerProvider.MultiDoors.GetParentCcuId(multiDoorElement.MultiDoor.IdMultiDoor);

                if (multiDoorElementParentCcu != _multiDoorElementsParentCcuId)
                {
                    _multiDoorElementsParentCcuId = null;
                    _eBlockAlarmArea.Enabled = false;
                    SetBlockAlarmArea(null);
                    return;
                }
            }

            _eBlockAlarmArea.Enabled = true;
        }

        private void _dgMultiDoorElements_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (MultiDoorElementShort multiDoorElementShort in bindingSource.List)
            {
                multiDoorElementShort.Symbol = _dgMultiDoorElements.GetDefaultImage(multiDoorElementShort);
            }
        }

        private void _dgMultiDoorElements_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _bEdit_Click(sender, e);
        }

        protected override bool CheckValues()
        {
            return true;
        }

        protected override bool GetValues()
        {
            _editingObject.Name = _eName.Text;
            _editingObject.FloorNumber = (int)_eFloorNumber.Value;
            _editingObject.BlockAlarmArea = _blockAlarmArea;
            _editingObject.Description = _eDescription.Text;

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            var retValue = Plugin.MainServerProvider.Floors.Insert(ref _editingObject, out error);
            
            if (!retValue && error != null)
            {
                if (error is IwQuick.SqlUniqueException)
                {
                    if (error.Message.Contains(Floor.ColumnName))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eName,
                            GetString("ErrorUsedFloorName"),
                            ControlNotificationSettings.Default);

                        _eName.Focus();
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
                    ? Plugin.MainServerProvider.Floors.UpdateOnlyInDatabase(_editingObject, out error)
                    : Plugin.MainServerProvider.Floors.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is IwQuick.SqlUniqueException)
                {
                    if (error.Message.Contains(Floor.ColumnName))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _eName,
                            GetString("ErrorUsedFloorName"),
                            ControlNotificationSettings.Default);

                        _eName.Focus();
                        return false;
                    }
                }

                throw error;
            }

            return retValue;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.Floors != null)
                Plugin.MainServerProvider.Floors.EditEnd(_editingObject);
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

        private void _bAdd_Click(object sender, EventArgs e)
        {
            Exception error;
            var multiDoorElements = Plugin.MainServerProvider.MultiDoorElements.ListModifyElevatorDoors(
                (int) _eFloorNumber.Value,
                GetAlreadyUsedElevatorDoors(),
                GetParentCcuId(SettingObject.MultiDoorElements),
                out error);

            var formAdd = new ListboxFormAdd(multiDoorElements, GetString("NCASMultiDoorElementsFormNCASMultiDoorElementsForm"));
            ListOfObjects outObjects;
            formAdd.ShowDialogMultiSelect(out outObjects);

            if (outObjects == null)
                return;

            foreach (var outObject in outObjects)
            {
                var modifyObject = outObject as IModifyObject;
                if (modifyObject == null)
                    continue;

                var multiDoorElement = Plugin.MainServerProvider.MultiDoorElements.GetObjectById(modifyObject.GetId);
                if (multiDoorElement == null)
                    continue;

                if (_editingObject.Doors == null)
                    _editingObject.Doors = new LinkedList<MultiDoorElement>();

                _editingObject.Doors.Add(multiDoorElement);
            }

            EditTextChanger(null, null);
            EnableDisableFloorNumber();
            ShowMultiDoorElements(_editingObject.Doors);
        }

        private IEnumerable<Guid> GetAlreadyUsedElevatorDoors()
        {
            if (_editingObject.Doors == null)
                return null;

            return
                new HashSet<Guid>(_editingObject.Doors.Select(
                    multiDoorElement =>
                        multiDoorElement.IdMultiDoorElement));
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

        private void _bRemove_Click(object sender, EventArgs e)
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
            EnableDisableFloorNumber();
            ShowMultiDoorElements(_editingObject.Doors);
        }

        private void _eName_TextChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
        }

        private void _eName_KeyPress(object sender, KeyPressEventArgs e)
        {
            _generateName = false;
        }

        private void _eFloorNumber_ValueChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);

            if (_generateName)
                GenerateName();
        }

        private void _eDescription_TextChanged(object sender, EventArgs e)
        {
            EditTextChangerOnlyInDatabase(null, null);
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

        protected override void AfterTranslateForm()
        {
            if (Insert)
                Text = GetString("NCASFloorEditFormInsertText");

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        private void _dgMultiDoorElements_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _dgMultiDoorElements_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddElevatorDoor(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddElevatorDoor(object newElevatorDoor)
        {
            try
            {
                var elevatorDoor = newElevatorDoor as MultiDoorElement;

                if (elevatorDoor == null)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _dgMultiDoorElements,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                if (elevatorDoor.MultiDoor.Type != MultiDoorType.Elevator)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _dgMultiDoorElements,
                        GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);

                    return;
                }

                if (elevatorDoor.DoorIndex != _editingObject.FloorNumber)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _dgMultiDoorElements,
                        GetString("ErrorMultiDoorElementMustHaveSameFloorNumberAsFloor"),
                        ControlNotificationSettings.Default);

                    return;
                }

                var alreadUsedElevatorDoors = GetAlreadyUsedElevatorDoors();
                if (alreadUsedElevatorDoors != null &&
                    alreadUsedElevatorDoors.Contains(elevatorDoor.IdMultiDoorElement))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _dgMultiDoorElements,
                        GetString("ErrorMultiDoorElementAlreadyAdded"),
                        ControlNotificationSettings.Default);

                    return;
                }

                _editingObject.Doors.Add(elevatorDoor);

                EditTextChanger(null, null);
                EnableDisableFloorNumber();
                ShowMultiDoorElements(_editingObject.Doors);
                Plugin.AddToRecentList(elevatorDoor);
            }
            catch
            {
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
                        GetString("ErrorSetAlarmAreaFromAnotherCCUFloor"),
                        ControlNotificationSettings.Default);

                    return;
                }
            }

            SetBlockAlarmArea(blockAlarmArea);
            Plugin.AddToRecentList(blockAlarmArea);
        }

        private void _eBlockAlarmArea_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify)
            {
                ModifyBlockAlarmArea();
                return;
            }

            if (item == _tsiRemove)
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
