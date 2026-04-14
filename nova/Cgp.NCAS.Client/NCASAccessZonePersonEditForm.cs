using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Cgp.Components;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Components;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.UI;

using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;
using SqlUniqueException = Contal.IwQuick.SqlUniqueException;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAccessZonePersonEditForm :
#if DESIGNER    
        Form
#else
 PluginMainForm<NCASClient>, ICgpDataGridView
#endif
    {
        private Person _actPerson;
        private ICollection<AccessZone> _accessZones;
        private BindingSource _bindingSource;
        DVoid2Void _dAfterTranslateForm;

        private ListOfObjects _actCardReaderObjects;
        private TimeZone _actTimeZone;
        private AccessZone _editAccessZone;
        private bool _allowEdit;

        public override NCASClient Plugin
        {
            get { return NCASClient.Singleton; }
        }

        public NCASAccessZonePersonEditForm(Person person, Control control, bool allowEdit)
            : base(NCASClient.LocalizationHelper, CgpClientMainForm.Singleton)
        {
            InitializeComponent();
#if DEBUG
            {
                _tbmCardReader.ButtonPopupMenu.Items.Add("_tsiCreateDebug");
                _tbmCardReader.ButtonPopupMenu.Items[1].Name = "_tsiCreateDebug";
            }
#endif
            LocalizationHelper.TranslateForm(this);
            _dAfterTranslateForm = AfterTranslateForm;
            _pBack.Parent = control;
            _actPerson = person;
            control.Enter += RunOnEnter;
            control.Disposed += RunOnDisposed;
            ButtonsCreateAccessZone();

            _allowEdit = allowEdit;
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.CgpDataGridEvents = this;
            _cdgvData.EnabledInsertButton = false;
            _cdgvData.DataGrid.DragDrop += _dgValues_DragDrop;
            _cdgvData.DataGrid.DragOver += _dgValues_DragOver;
            _cdgvData.DataGrid.MultiSelect = true;
        }

        protected override void AfterTranslateForm()
        {
            LocalizationHelper.TranslateControl(_pBack);
        }

        void RunOnEnter(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_dAfterTranslateForm == null)
            {
                _dAfterTranslateForm = AfterTranslateForm;
                LocalizationHelper.LanguageChanged += _dAfterTranslateForm;
            }

            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (_allowEdit)
                EnabledForm();
            else
                DisabledForm();

            ShowPersonAccessZone();
        }

        void RunOnDisposed(object sender, EventArgs e)
        {
            LocalizationHelper.LanguageChanged -= _dAfterTranslateForm;
        }

        private void DisabledForm()
        {
            _cdgvData.EnabledDeleteButton = false;
            //_bEdit.Enabled = false;

            _tbmCardReader.Enabled = false;
            _tbmTimeZone.Enabled = false;
            _eDescription.Enabled = false;
            _bUpdate.Enabled = false;
            _bCancel.Enabled = false;
            _bCreate0.Enabled = false;
        }

        private void EnabledForm()
        {
            _cdgvData.EnabledDeleteButton = true;
            //_bEdit.Enabled = true;

            _tbmCardReader.Enabled = true;
            _tbmTimeZone.Enabled = true;
            _eDescription.Enabled = true;
            _bUpdate.Enabled = true;
            _bCancel.Enabled = true;
            _bCreate0.Enabled = true;
        }

        private void ShowPersonAccessZone()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowPersonAccessZone));
            }
            else
            {
                Exception error;
                ICollection<AccessZone> accessZones = Plugin.MainServerProvider.AccessZones.GetAccessZonesByPerson(_actPerson.IdPerson, out error);

                if (error != null)
                {
                    if (error is AccessDeniedException)
                    {
                        Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorLoadTableAccessDenied"));
                    }
                    else
                    {
                        Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorLoadTable"));
                    }

                    _bindingSource = null;
                    _cdgvData.DataGrid.DataSource = null;
                    return;
                }

                if (_accessZones == null)
                {
                    _accessZones = new List<AccessZone>();
                }
                else
                {
                    _accessZones.Clear();
                }

                _accessZones = accessZones;
                _bindingSource = new BindingSource();
                _bindingSource.DataSource = _accessZones;

                _cdgvData.ModifyGridView(_bindingSource, AccessZone.COLUMNCARDREADEROBJECT, AccessZone.COLUMNTIMEZONE, AccessZone.COLUMNDESCRIPTION);
            }
        }

        protected void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView == null) return;

            if (gridView.Columns.Contains(columnName))
                gridView.Columns[columnName].Visible = false;
        }

        #region CardReader

        private void ModifyCardReader()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                List<IModifyObject> listModObj = new List<IModifyObject>();

                IList<IModifyObject> listCardReadersFromDatabase =
                    Plugin.MainServerProvider.CardReaders.ListModifyObjects(false, out error);

                if (error != null) throw error;
                listModObj.AddRange(listCardReadersFromDatabase);

                IList<IModifyObject> listDoorEnvironments = Plugin.MainServerProvider.DoorEnvironments.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listDoorEnvironments);

                ICollection<IModifyObject> listAlarmAreasFromDatabase = Plugin.MainServerProvider.AlarmAreas.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listAlarmAreasFromDatabase);

                IList<IModifyObject> listDCUsFromDatabase = Plugin.MainServerProvider.DCUs.ListModifyObjects(out error);
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

                ListboxFormAdd formAdd = new ListboxFormAdd(listModObj, GetString("NCASAccessControlListEditForm_CardReaderObjects"));

                ListOfObjects outCardReaderObjects = null;
                //enables to create more object at once by enabling multiselection (only in create mode!!!)
                if (_bCreate0.Visible)
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
                    foreach (object selectedObject in outCardReaderObjects)
                    {
                        IModifyObject modObj = selectedObject as IModifyObject;
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
            catch
            {
            }
        }

        private void DoAfterCardReaderCreated(object newCardReader)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCardReaderCreated), newCardReader);
            }
            else
            {
                if (newCardReader is CardReader)
                {
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(newCardReader as CardReader);
                    RefreshCardReaderObject();
                }
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

        private void AddCardReader(object newCardReaderObject)
        {
            try
            {
                if (newCardReaderObject is CardReader)
                {
                    CardReader cardReader = newCardReaderObject as CardReader;

                    if (Plugin.MainServerProvider.MultiDoors.IsCardReaderUsedInMultiDoor(cardReader.IdCardReader))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne,
                            _tbmCardReader.ImageTextBox,
                            GetString("ErrorCanNotBeUsedCardReaderAssignedToMultiDoor"),
                            ControlNotificationSettings.Default);

                        return;
                    }
                    
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(cardReader);
                    RefreshCardReaderObject();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else if (newCardReaderObject is DoorEnvironment)
                {
                    DoorEnvironment doorEnvironment = newCardReaderObject as DoorEnvironment;
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(doorEnvironment);
                    RefreshCardReaderObject();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else if (newCardReaderObject is AlarmArea)
                {
                    AlarmArea alarmArea = newCardReaderObject as AlarmArea;
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(alarmArea);
                    RefreshCardReaderObject();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else if (newCardReaderObject is DCU)
                {
                    DCU dcu = newCardReaderObject as DCU;
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(dcu);
                    RefreshCardReaderObject();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else if (newCardReaderObject is MultiDoor)
                {
                    var multiDoor = newCardReaderObject as MultiDoor;
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(multiDoor);
                    RefreshCardReaderObject();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else if (newCardReaderObject is MultiDoorElement)
                {
                    var multiDoorElement = newCardReaderObject as MultiDoorElement;
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(multiDoorElement);
                    RefreshCardReaderObject();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else if (newCardReaderObject is Floor)
                {
                    var floor = newCardReaderObject as Floor;
                    _actCardReaderObjects = new ListOfObjects();
                    _actCardReaderObjects.Objects.Add(floor);
                    RefreshCardReaderObject();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmCardReader_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmCardReader_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddCardReader((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmCardReader_DoubleClick(object sender, EventArgs e)
        {
            if (_actCardReaderObjects != null && _actCardReaderObjects.Count == 1)
            {
                if (_actCardReaderObjects[0] is CardReader)
                {
                    NCASCardReadersForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as CardReader);
                }
                else if (_actCardReaderObjects[0] is DoorEnvironment)
                {
                    NCASDoorEnvironmentsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as DoorEnvironment);
                }
                else if (_actCardReaderObjects[0] is AlarmArea)
                {
                    NCASAlarmAreasForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as AlarmArea);
                }
                else if (_actCardReaderObjects[0] is DCU)
                {
                    NCASDCUsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as DCU);
                }
                else if (_actCardReaderObjects[0] is MultiDoor)
                {
                    NCASMultiDoorsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as MultiDoor);
                }
                else if (_actCardReaderObjects[0] is MultiDoorElement)
                {
                    NCASMultiDoorElementsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as MultiDoorElement);
                }
                else if (_actCardReaderObjects[0] is Floor)
                {
                    NCASFloorsForm.Singleton.OpenEditForm(_actCardReaderObjects[0] as Floor);
                }
            }
        }
        #endregion

        #region TimeZone


        private void CreateTimeZoneClick(object sender, EventArgs e)
        {
            TimeZone timeZone = new TimeZone();
            TimeZonesForm.Singleton.OpenInsertFromEdit(ref timeZone, DoAfterTimeZoneCreated);
        }

        private void DoAfterTimeZoneCreated(object newTimeZone)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterTimeZoneCreated), newTimeZone);
            }
            else
            {
                if (newTimeZone is TimeZone)
                {
                    _actTimeZone = newTimeZone as TimeZone;
                    RefreshTimeZone();
                }
            }
        }
        private void ModifyTimeZone()
        {

            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                List<AOrmObject> listTimeZones = new List<AOrmObject>();

                Exception error;
                ICollection<TimeZone> listTimeZonesFromDatabase = CgpClient.Singleton.MainServerProvider.TimeZones.List(out error);
                foreach (TimeZone timeZone in listTimeZonesFromDatabase)
                {
                    listTimeZones.Add(timeZone);
                }

                ListboxFormAdd formAdd = new ListboxFormAdd(listTimeZones, CgpClient.Singleton.LocalizationHelper.GetString("TimeZonesFormTimeZonesForm"));
                object outTimeZone;
                formAdd.ShowDialog(out outTimeZone);
                if (outTimeZone != null)
                {
                    _actTimeZone = outTimeZone as TimeZone;
                    RefreshTimeZone();
                    Plugin.AddToRecentList(_actTimeZone);
                }
            }
            catch
            {
            }
        }

        private void ModifyTimeZoneClick(object sender, EventArgs e)
        {
            ModifyTimeZone();
        }


        private void RemoveTimeZoneClick(object sender, EventArgs e)
        {
            if (Dialog.Question(GetString("QuestionRemoveTimeZone")))
            {
                _actTimeZone = null;
                RefreshTimeZone();
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

        private void TimeZoneDoubleClick(object sender, EventArgs e)
        {
            if (_actTimeZone != null)
                TimeZonesForm.Singleton.OpenEditForm(_actTimeZone);
        }

        private void TimeZoneDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void TimeZoneDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddTimeZone((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddTimeZone(object newTimeZone)
        {
            try
            {
                if (newTimeZone.GetType() == typeof(TimeZone))
                {
                    if (Dialog.Question(GetString("QuestionAddTimeZone")))
                    {
                        TimeZone timeZone = newTimeZone as TimeZone;
                        _actTimeZone = timeZone;
                        RefreshTimeZone();
                        Plugin.AddToRecentList(newTimeZone);
                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmTimeZone.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        #endregion

        private void _bCreate0_Click(object sender, EventArgs e)
        {
            CreateNewAccessZone();
        }

        private void _bUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccessZone();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            ClearEdits();
        }

        private void CreateNewAccessZone()
        {
            if (_actCardReaderObjects != null)
            {
                foreach (var selectedCardReaderObject in _actCardReaderObjects)
                {
                    if (!ControlValues()) return;
                    _editAccessZone = new AccessZone
                    {
                        Person = _actPerson,
                        TimeZone = _actTimeZone,
                        Description = _eDescription.Text
                    };

                    var cardReaderObject = selectedCardReaderObject as AOrmObject;
                    _editAccessZone.CardReaderObject = cardReaderObject;

                    if (cardReaderObject != null)
                    {
                        _editAccessZone.CardReaderObjectType = (byte) cardReaderObject.GetObjectType();
                        _editAccessZone.GuidCardReaderObject = (Guid) cardReaderObject.GetId();
                    }

                    SaveAccessZoneInsert();
                }
                ShowPersonAccessZone();
                ClearEdits();
            }
            else
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader.ImageTextBox,
                GetString("ErrorEntryCardReader"), ControlNotificationSettings.Default);
                _tbmCardReader.ImageTextBox.Focus();
            }
        }

        private void UpdateAccessZone()
        {
            if (!ControlValues() || _actCardReaderObjects.Objects.Count < 1) return;
            _editAccessZone.Person = _actPerson;

            object cardReaderObject = _actCardReaderObjects.Objects[0];
            if (cardReaderObject is CardReader)
            {
                _editAccessZone.CardReaderObject = cardReaderObject as AOrmObject;
                _editAccessZone.CardReaderObjectType = (byte)ObjectType.CardReader;
                _editAccessZone.GuidCardReaderObject = (cardReaderObject as CardReader).IdCardReader;
            }
            else if (cardReaderObject is DoorEnvironment)
            {
                _editAccessZone.CardReaderObject = cardReaderObject as AOrmObject;
                _editAccessZone.CardReaderObjectType = (byte)ObjectType.DoorEnvironment;
                _editAccessZone.GuidCardReaderObject = (cardReaderObject as DoorEnvironment).IdDoorEnvironment;
            }
            else if (cardReaderObject is AlarmArea)
            {
                _editAccessZone.CardReaderObject = cardReaderObject as AOrmObject;
                _editAccessZone.CardReaderObjectType = (byte)ObjectType.AlarmArea;
                _editAccessZone.GuidCardReaderObject = (cardReaderObject as AlarmArea).IdAlarmArea;
            }
            else if (cardReaderObject is DCU)
            {
                _editAccessZone.CardReaderObject = cardReaderObject as AOrmObject;
                _editAccessZone.CardReaderObjectType = (byte)ObjectType.DCU;
                _editAccessZone.GuidCardReaderObject = (cardReaderObject as DCU).IdDCU;
            }
            else if (cardReaderObject is MultiDoor)
            {
                _editAccessZone.CardReaderObject = cardReaderObject as AOrmObject;
                _editAccessZone.CardReaderObjectType = (byte)ObjectType.MultiDoor;
                _editAccessZone.GuidCardReaderObject = (cardReaderObject as MultiDoor).IdMultiDoor;
            }
            else if (cardReaderObject is MultiDoorElement)
            {
                _editAccessZone.CardReaderObject = cardReaderObject as AOrmObject;
                _editAccessZone.CardReaderObjectType = (byte)ObjectType.MultiDoorElement;
                _editAccessZone.GuidCardReaderObject = (cardReaderObject as MultiDoorElement).IdMultiDoorElement;
            }
            else if (cardReaderObject is Floor)
            {
                _editAccessZone.CardReaderObject = cardReaderObject as AOrmObject;
                _editAccessZone.CardReaderObjectType = (byte)ObjectType.Floor;
                _editAccessZone.GuidCardReaderObject = (cardReaderObject as Floor).IdFloor;
            }

            _editAccessZone.TimeZone = _actTimeZone;
            _editAccessZone.Description = _eDescription.Text;

            if (!SaveAccessZoneEdit())
                Dialog.Error("Failed");

            ShowPersonAccessZone();
            ClearEdits();
        }

        private void EditAccessZone()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            AccessZone accessZone = _bindingSource[_bindingSource.Position] as AccessZone;
            Exception error;
            _editAccessZone = Plugin.MainServerProvider.AccessZones.GetObjectForEdit(accessZone.IdAccessZone, out error);
            if (_editAccessZone != null)
            {
                ButtonsEditAccessZone();
                if (_editAccessZone.TimeZone != null)
                {
                    _actTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(_editAccessZone.TimeZone.IdTimeZone);
                }
                else
                {
                    _actTimeZone = null;
                }
                _actCardReaderObjects = new ListOfObjects();
                if (_editAccessZone.CardReaderObjectType == (byte)ObjectType.CardReader)
                {
                    _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.CardReaders.GetObjectById(_editAccessZone.GuidCardReaderObject));
                }
                else if (_editAccessZone.CardReaderObjectType == (byte)ObjectType.AlarmArea)
                {
                    _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.AlarmAreas.GetObjectById(_editAccessZone.GuidCardReaderObject));
                }
                else if (_editAccessZone.CardReaderObjectType == (byte)ObjectType.DoorEnvironment)
                {
                    _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.DoorEnvironments.GetObjectById(_editAccessZone.GuidCardReaderObject));
                }
                else if (_editAccessZone.CardReaderObjectType == (byte)ObjectType.DCU)
                {
                    _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.DCUs.GetObjectById(_editAccessZone.GuidCardReaderObject));
                }
                else if (_editAccessZone.CardReaderObjectType == (byte)ObjectType.MultiDoor)
                {
                    _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.MultiDoors.GetObjectById(_editAccessZone.GuidCardReaderObject));
                }
                else if (_editAccessZone.CardReaderObjectType == (byte)ObjectType.MultiDoorElement)
                {
                    _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.MultiDoorElements.GetObjectById(_editAccessZone.GuidCardReaderObject));
                }
                else if (_editAccessZone.CardReaderObjectType == (byte)ObjectType.Floor)
                {
                    _actCardReaderObjects.Objects.Add(Plugin.MainServerProvider.Floors.GetObjectById(_editAccessZone.GuidCardReaderObject));
                }
                _eDescription.Text = _editAccessZone.Description;
                RefreshCardReaderObject();
                RefreshTimeZone();
                ButtonsEditAccessZone();
            }
        }

        private void ClearEdits()
        {
            _actTimeZone = null;
            _actCardReaderObjects = null;
            _tbmCardReader.Text = string.Empty;
            _tbmTimeZone.Text = string.Empty;
            _eDescription.Text = string.Empty;
            ButtonsCreateAccessZone();
        }

        private bool SaveAccessZoneInsert()
        {
            Exception error;
            bool retValue = Plugin.MainServerProvider.AccessZones.Insert(ref _editAccessZone, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    Dialog.Error(GetString("ErrorUsedAccessZonePair"));
                    _tbmCardReader.ImageTextBox.Focus();
                }
            }
            return retValue;
        }

        private bool SaveAccessZoneEdit()
        {
            Exception error;
            bool retValue = Plugin.MainServerProvider.AccessZones.Update(_editAccessZone, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    Dialog.Error(GetString("ErrorUsedAccessZonePair"));
                    _tbmCardReader.ImageTextBox.Focus();
                }
            }
            return retValue;
        }

        private bool ControlValues()
        {
            if (_actCardReaderObjects == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader.ImageTextBox,
                GetString("ErrorEntryCardReader"), ControlNotificationSettings.Default);
                _tbmCardReader.ImageTextBox.Focus();
                return false;
            }

            foreach (object obj in _actCardReaderObjects.Objects)
            {
                if (obj is DoorEnvironment)
                {
                    if (!DoorEnvironmentHasCR(obj as DoorEnvironment))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader.ImageTextBox,
                            GetString("ErrorDoorEnvironmentHasNotCardReaders"), ControlNotificationSettings.Default);
                        return false;
                    }
                }
                else if (obj is AlarmArea)
                {
                    if (!AlarmAreaHasCR(obj as AlarmArea))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader.ImageTextBox,
                            GetString("ErrorAlarmAreaHasNotCardReaders"), ControlNotificationSettings.Default);
                        return false;
                    }
                }
                else if (obj is DCU)
                {
                    if (!DCUHasCR(obj as DCU))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader.ImageTextBox,
                            GetString("ErrorDCUHasNotCardReaders"), ControlNotificationSettings.Default);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool DoorEnvironmentHasCR(DoorEnvironment de)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return false;
            DoorEnvironment doorEnvironment = Plugin.MainServerProvider.DoorEnvironments.GetObjectById(de.IdDoorEnvironment);

            if (doorEnvironment != null)
            {
                if (doorEnvironment.CardReaderInternal != null || doorEnvironment.CardReaderExternal != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool AlarmAreaHasCR(AlarmArea aa)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return false;
            AlarmArea alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById(aa.IdAlarmArea);

            if (alarmArea != null)
            {
                if (alarmArea.AACardReaders != null && alarmArea.AACardReaders.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool DCUHasCR(DCU dcu)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return false;
            DCU conrolrdDcu = Plugin.MainServerProvider.DCUs.GetObjectById(dcu.IdDCU);

            if (conrolrdDcu != null)
            {
                if (conrolrdDcu.CardReaders != null && conrolrdDcu.CardReaders.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void ButtonsCreateAccessZone()
        {
            _bCancel.Visible = false;
            _bUpdate.Visible = false;
            _bCreate0.Visible = true;
            _cdgvData.EnabledDeleteButton = true;
        }

        private void ButtonsEditAccessZone()
        {
            _bCancel.Visible = true;
            _bUpdate.Visible = true;
            _bCreate0.Visible = false;
            _cdgvData.EnabledDeleteButton = false;
        }

        private void _dgValues_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _dgValues_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddDDAccessZone((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddDDAccessZone(object newCardReaderObject)
        {
            try
            {
                if (newCardReaderObject is CardReader ||
                    newCardReaderObject is AlarmArea ||
                    newCardReaderObject is DoorEnvironment ||
                    newCardReaderObject is DCU ||
                    newCardReaderObject is MultiDoor ||
                    newCardReaderObject is MultiDoorElement ||
                    newCardReaderObject is Floor
                    )
                {
                    AOrmObject cardReaderObject = newCardReaderObject as AOrmObject;

                    if (!ControlAddedAccessZoneCRO(cardReaderObject)) return;
                    var accessZone = new AccessZone
                    {
                        Person = _actPerson,
                        TimeZone = null,
                        CardReaderObjectType = (byte) cardReaderObject.GetObjectType(),
                        GuidCardReaderObject = (Guid) cardReaderObject.GetId()
                    };

                    Exception error;
                    bool retValue = Plugin.MainServerProvider.AccessZones.Insert(ref accessZone, out error);
                    if (!retValue && error != null)
                    {
                        if (error is SqlUniqueException)
                        {
                            Dialog.Error(GetString("ErrorUsedAccessZonePair"));
                        }
                        else
                        {
                            Dialog.Error(GetString("ErrorInsertCardReaderObject"));
                        }
                        return;
                    }
                    //EditTextChanger(null, null);
                    ShowPersonAccessZone();
                    Plugin.AddToRecentList(newCardReaderObject);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvData.DataGrid,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private bool ControlAddedAccessZoneCRO(AOrmObject crObj)
        {
            switch (crObj.GetObjectType())
            {
                case ObjectType.DoorEnvironment:
                    if (!DoorEnvironmentHasCR(crObj as DoorEnvironment))
                    {
                        Dialog.Error(GetString("ErrorDoorEnvironmentHasNotCardReaders"));
                        return false;
                    }
                    break;
                case ObjectType.AlarmArea:
                    if (!AlarmAreaHasCR(crObj as AlarmArea))
                    {
                        Dialog.Error(GetString("ErrorAlarmAreaHasNotCardReaders"));
                        return false;
                    }
                    break;
                case ObjectType.DCU:
                    if (!DCUHasCR(crObj as DCU))
                    {
                        Dialog.Error(GetString("ErrorDCUHasNotCardReaders"));
                        return false;
                    }
                    break;
            }

            return true;
        }

        private void _tbmTimeZone_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                ModifyTimeZone();
            }
            else if (item.Name == "_tsiRemove")
            {
                if (Dialog.Question(GetString("QuestionRemoveTimeZone")))
                {
                    _actTimeZone = null;
                    RefreshTimeZone();
                }
            }
            else if (item.Name == "_tsiCreate2")
            {
                TimeZone timeZone = new TimeZone();
                TimeZonesForm.Singleton.OpenInsertFromEdit(ref timeZone, DoAfterTimeZoneCreated);
            }
        }

        private void _tbmCardReader_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyCardReader();
            }
            else if (item.Name == "_tsiCreateDebug")
            {
                CardReader cardReader = new CardReader();
                cardReader.EnableParentInFullName = Plugin.MainServerProvider.GetEnableParentInFullName();
                NCASCardReadersForm.Singleton.OpenInsertFromEdit(ref cardReader, DoAfterCardReaderCreated);
            }
        }

        public static void SaveAfterInsertWithData(NCASClient ncasClient, Person person, Person clonedPerson)
        {
            if (ncasClient == null || person == null || CgpClient.Singleton.IsConnectionLost(false)) return;

            IList<FilterSettings> filterSettings = new List<FilterSettings>();
            FilterSettings filterSetting = new FilterSettings(ACLPerson.COLUMNPERSON, clonedPerson, ComparerModes.EQUALL);
            filterSettings.Add(filterSetting);

            Exception error;
            ICollection<AccessZone> accessZones = ncasClient.MainServerProvider.AccessZones.SelectByCriteria(filterSettings, out error);

            if (accessZones != null && accessZones.Count > 0)
            {
                foreach (AccessZone accessZone in accessZones)
                {
                    if (accessZone != null)
                    {
                        AccessZone newAccessZone = new AccessZone();
                        newAccessZone.Person = person;
                        newAccessZone.GuidCardReaderObject = accessZone.GuidCardReaderObject;
                        newAccessZone.CardReaderObjectType = accessZone.CardReaderObjectType;
                        newAccessZone.TimeZone = accessZone.TimeZone;
                        newAccessZone.Description = accessZone.Description;

                        ncasClient.MainServerProvider.AccessZones.Insert(ref newAccessZone, out error);
                    }
                }
            }
        }

        public void EditClick()
        {
            EditAccessZone();
        }

        public virtual void EditClick(ICollection<int> indexes)
        {
        }

        public void DeleteClick()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) 
                return;

            if (_bindingSource != null && _bindingSource.Count > 0)
            {
                var accessZone = _bindingSource[_bindingSource.Position] as AccessZone;

                if (accessZone != null)
                {
                    if (Dialog.Question(CgpClient.Singleton.LocalizationHelper.GetString("QuestionDeleteConfirm")))
                    {
                        Exception error;
                        Plugin.MainServerProvider.AccessZones.Delete(accessZone, out error);

                        if (error != null)
                        {
                            if (error is SqlDeleteReferenceConstraintException)
                                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteRowInRelationship"));
                            else
                                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteFailed"));
                        }
                        else
                        {
                            ShowPersonAccessZone();
                        }
                    }
                }
            }
        }

        public virtual void DeleteClick(ICollection<int> indexes)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (_bindingSource == null
                || _bindingSource.Count == 0)
                return;

            var items =
                indexes.Select(index => (IShortObject)(new AccessZoneShort((AccessZone)_bindingSource.List[index])))
                    .ToList();

            var dialog = new DeleteDataGridItemsDialog(
                Plugin.GetPluginObjectsImages(),
                items,
                CgpClient.Singleton.LocalizationHelper)
            {
                SelectItem = items.FirstOrDefault(item => item.Id.Equals((((AccessZone)_bindingSource.Current).IdAccessZone)))
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var accessZones = new LinkedList<AccessZone>();

                if (dialog.DeleteAll)
                {
                    foreach (int index in indexes)
                    {
                        if (_bindingSource.Count > index)
                            accessZones.AddLast(_bindingSource[index] as AccessZone);
                    }
                }
                else
                {
                    foreach (var item in dialog.SelectedItems)
                    {
                        var accessZone =
                            _bindingSource.List.Cast<AccessZone>()
                                .FirstOrDefault(az => az.IdAccessZone.Equals(item.Id));

                        accessZones.AddLast(accessZone);
                    }
                }

                DeleteAccessZones(accessZones);
            }
        }

        private void DeleteAccessZones(IEnumerable<AccessZone> accessZones)
        {
            if (accessZones == null)
                return;

            Exception error = null;

            foreach (var accessZone in accessZones)
            {
                Plugin.MainServerProvider.AccessZones.Delete(accessZone, out error);
            }

            if (error != null)
            {
                if (error is SqlDeleteReferenceConstraintException)
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteRowInRelationship"));
                else
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteFailed"));
            }
            
            ShowPersonAccessZone();
        }

        public void InsertClick()
        {
        }
    }
}
