using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASSceneEditForm :
#if DESIGNER    
        Form
#else
 ACgpPluginEditForm<NCASClient, Scene>
#endif
    {
        private ShowOptionsEditForm _options;
        private DVoid2Void _eventAlarmChanged;
        private readonly GraphicsSceneEventsHelper _graphicsSceneEventsHelper = new GraphicsSceneEventsHelper();
        private readonly List<ServerAlarmCore> _serverAlarms = new List<ServerAlarmCore>();
        private AlarmsDialog _alarmDialog;
        private bool _isSceneSaved = true;

        //private readonly byte[] pole = new byte[200000000];

        public NCASSceneEditForm(
                Scene scene,
                ShowOptionsEditForm options,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                scene,
                options == ShowOptionsEditForm.View
                    ? ShowOptionsEditForm.Edit
                    : options, 
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            DisableFormForViewMode = false;
            InitializeComponent();
            _options = options;
            Closing += NCASSceneEditForm_Closing;

            if (options == ShowOptionsEditForm.Insert)
                _isSceneSaved = false;

            if (options == ShowOptionsEditForm.Edit
                || options == ShowOptionsEditForm.Insert)
                graphicsControl.EditMode = true;
            else
                graphicsControl.EditMode = false;

            GraphicsScene.LocalizationHelper = NCASClient.LocalizationHelper;
            _graphicsSceneEventsHelper.AddGraphicsScene(graphicsControl);
            _graphicsSceneEventsHelper.Initialize();

            //var random = new Random();

            //for (int i = 0; i < 200000000; i++)
            //    pole[i] = (byte)random.Next(0, 250);
        }

        void NCASSceneEditForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (graphicsControl.NeedSaveScene
                && Dialog.Question(LocalizationHelper.GetString("QuestionSaveScene")))
            {
                SaveScene();
            }
        }

        protected override void OnShown(EventArgs e)
        {
#if !DEBUG
            string localisedName;
            object licenseValue;

            if (!CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(
                Cgp.NCAS.Globals.RequiredLicenceProperties.Graphics.ToString(), out localisedName, out licenseValue) ||
                !(bool)licenseValue)
            {
                Dialog.Error(GetString("ErrorGraphicNotSupportedByLicense"));
                Close();
            }
#endif
            base.OnShown(e);
        }

        protected override void RegisterEvents()
        {
            //alarms
            _eventAlarmChanged = ChangeAlarms;
            ChangeAlarmsHandler.Singleton.RegisterChangeAlarms(_eventAlarmChanged);
        }

        protected override void UnregisterEvents()
        {
            //alarms
            ChangeAlarmsHandler.Singleton.UnregisterChangeAlarm(_eventAlarmChanged);

            _graphicsSceneEventsHelper.RemoveAllGraphicsScenes();
            _graphicsSceneEventsHelper.Deinitialize();
            graphicsControl.UnRegisterEvents();
            _panelAlarms.Controls.Remove(_alarmDialog);
        }

        protected override void BeforeInsert()
        {
            NCASScenesForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASScenesForm.Singleton.BeforeEdit(this, _editingObject, graphicsControl.EditMode);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj = Plugin.MainServerProvider.Scenes.GetObjectForEdit(_editingObject.IdScene, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.Scenes.GetObjectById(_editingObject.IdScene);
                }
                else
                {
                    throw error;
                }
                //DisableForm();
                graphicsControl.AllowedEdit = false;
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
            Plugin.MainServerProvider.Scenes.RenewObjectForEdit(_editingObject.IdScene, out error);

            if (error != null)
                throw error;
        }

        protected override void SetValuesInsert()
        {
        }

        protected override void SetValuesEdit()
        {
            //var random = new Random();

            //MessageBox.Show(pole[random.Next(0, 200000000)].ToString());
        }

        void NCASSceneEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);
        }

        protected override bool CheckValues()
        {
            return true;
        }

        protected override bool GetValues()
        {
            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            bool retValue = Plugin.MainServerProvider.Scenes.Insert(ref _editingObject, out error);
            graphicsControl.Scene.IdScene = _editingObject.IdScene;

            if (!retValue && error != null)
            {
                throw error;
            }

            _isSceneSaved = true;
            AfterInsert();

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue;
            retValue = Plugin.MainServerProvider.Scenes.UpdateOnlyInDatabase(_editingObject, out error);            
            return retValue;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.AccessControlLists != null)
                Plugin.MainServerProvider.Scenes.EditEnd(_editingObject);
        }

        protected override void AfterInsert()
        {
            NCASScenesForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASScenesForm.Singleton.AfterEdit(_editingObject);
        }

        private void SaveScene()
        {
            if (string.IsNullOrEmpty(_editingObject.Name))
            {
                Dialog.Warning(GetString("WarningInsertSceneName"));
                return;
            }

            graphicsControl.SaveScene();
            _editingObject = graphicsControl.Scene;

            if (_editingObject.RowDataBackground == null)
            {
                Dialog.Warning(GetString("WarningBackgroundNotSelected"));
                return;
            }

            EditTextChanger(this, null);
            Apply_Click();
            Dialog.Info(GetString("InfoSceneSaved"));
        }

        private void NCASSceneEditForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            Text = string.Format("Scene: {0}", _editingObject.Name);
            graphicsControl.GetLocalizationString += GetLocalizationString;
            GraphicsScene.MainServerProvider = Plugin.MainServerProvider;
            graphicsControl.LoadedModel += graphicsControl_LoadedModel;
            graphicsControl.ShowScenes += graphicsControl_ShowScenes;
            graphicsControl.SaveSceneClick += graphicsControl_SaveSceneClick;
            graphicsControl.ChangeEditMode += graphicsControl_ChangeEditMode;
            graphicsControl.ButtonClick += graphicsControl_ButtonClick;
            graphicsControl.EditObjectClick += graphicsControl_EditObjectClick;
            graphicsControl.SceneChanged += graphicsControl_SceneChanged;
            

            _alarmDialog = new AlarmsDialog(_serverAlarms);
            _alarmDialog.NestedInAnotherWindow = false;
            _alarmDialog.TopLevel = false;
            _panelAlarms.Controls.Add(_alarmDialog);
            _alarmDialog.Show();
            _alarmDialog.FormOnEnter(_alarmDialog);

            ChangeEditMode(graphicsControl.EditMode);
            _bHideOrShow.Image = ResourceGlobal.show12;
            ShowOption = _options;
            graphicsControl.Scene = _editingObject;
        }

        void graphicsControl_SceneChanged(Scene scene)
        {
            Text = scene.Name;
        }

        void graphicsControl_EditObjectClick(ObjectType objectType, Guid id)
        {
            switch (objectType)
            {
                case ObjectType.AlarmArea:
                    var alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById(id);

                    if (alarmArea != null)
                        NCASAlarmAreasForm.Singleton.OpenEditForm(alarmArea);
                    break;

                case ObjectType.CardReader:
                    var cardReader = Plugin.MainServerProvider.CardReaders.GetObjectById(id);

                    if (cardReader != null)
                        NCASCardReadersForm.Singleton.OpenEditForm(cardReader);
                    break;

                case ObjectType.Input:
                    var input = Plugin.MainServerProvider.Inputs.GetObjectById(id);

                    if (input != null)
                        NCASInputsForm.Singleton.OpenEditForm(input);
                    break;

                case ObjectType.Output:
                    var output = Plugin.MainServerProvider.Outputs.GetObjectById(id);

                    if (output != null)
                        NCASOutputsForm.Singleton.OpenEditForm(output);
                    break;

                case ObjectType.DoorEnvironment:
                    var doorEnvironment = Plugin.MainServerProvider.DoorEnvironments.GetObjectById(id);

                    if (doorEnvironment != null)
                        NCASDoorEnvironmentsForm.Singleton.OpenEditForm(doorEnvironment);
                    break;

                case ObjectType.CCU:
                    var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(id);

                    if (ccu != null)
                        NCASCCUsForm.Singleton.OpenEditForm(ccu);
                    break;

                case ObjectType.DCU:
                    var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(id);

                    if (dcu != null)
                        NCASDCUsForm.Singleton.OpenEditForm(dcu);
                    break;
            }
        }

        void graphicsControl_ButtonClick(string buttonName)
        {
            switch (buttonName)
            {
                case "_bInsertSymbol":
                    _bImportGS_Click(null, null);
                    break;
            }
        }

        void graphicsControl_ChangeEditMode(bool editMode)
        {
            ChangeEditMode(editMode);
        }

        private void ChangeEditMode(bool editMode)
        {
            if (!editMode)
            {
                splitContainer1.Panel1Collapsed = false;
                DisabledControls(this);
                _bHideOrShow.Enabled = true;
                _bHideOrShow.Visible = true;
                elementHost.Height = splitContainer1.Panel1.Size.Height - _bHideOrShow.Height - 3;
                _isAlarmDialogOpen = false;
                _bHideOrShow.Image = ResourceGlobal.show12;
                ShowOption = ShowOptionsEditForm.View;
            }
            else
            {
                splitContainer1.Panel1Collapsed = false;
                splitContainer1.Panel2Collapsed = true;
                EnabledControls(this);
                _bHideOrShow.Enabled = false;
                _bHideOrShow.Visible = false;
                elementHost.Height = splitContainer1.Panel1.Size.Height - 3;
                ShowOption = ShowOptionsEditForm.Edit;
            }

            elementHost.Enabled = true;

            if (_isSceneSaved)
            {
                AfterEdit();
                BeforeEdit();
            }
        }

        private void EnabledControls(Control control)
        {
            if (control == _alarmDialog
                || control == elementHost)
            {
                return;
            }

            if (control.Controls.Count == 0)
                EnabledControl(control);
            else
                foreach (Control actControl in control.Controls)
                    EnabledControls(actControl);
        }

        private void EnabledControl(Control control)
        {
            if (control == _alarmDialog
                || control == elementHost)
            {
                return;
            }

            control.Enabled = true;
        }

        private void DisabledControls(Control control)
        {
            if (control == _alarmDialog
                || control == elementHost)
            {
                return;
            }

            if (control.Controls.Count == 0)
                DisabledControl(control);
            else
                foreach (Control actControl in control.Controls)
                    DisabledControls(actControl);
        }

        private void DisabledControl(Control control)
        {
            if (control == _alarmDialog
                || control == elementHost)
            {
                return;
            }

            if (!(control is Label))
                control.Enabled = false;
        }

        void graphicsControl_SaveSceneClick()
        {
            SaveScene();
        }

        void graphicsControl_ShowScenes()
        {
            NCASScenesForm.Singleton.Show();
        }

        void graphicsControl_LoadedModel()
        {
            ShowAlarms();
            elementHost.Enabled = true;
        }
        
        private void ChangeAlarms()
        {
            if (_options == ShowOptionsEditForm.Edit)
                return;

            Invoke((MethodInvoker)ShowAlarms);
        }

        private void ShowAlarms()
        {
            if (_options == ShowOptionsEditForm.Edit)
                return;

            _serverAlarms.Clear();
            var serverAlarms = CgpClient.Singleton.MainServerProvider.GetAlarms();
            var guids = graphicsControl.GetObjectsGuid();

            if (guids != null && guids.Count > 0 && serverAlarms != null && serverAlarms.Count > 0)
                foreach (var serverAlarm in serverAlarms)
                {
                    var alarmObject = serverAlarm.Alarm.AlarmKey.AlarmObject;

                    if (alarmObject == null)
                        continue;

                    if (guids.Contains((Guid) alarmObject.Id))
                        _serverAlarms.Add(serverAlarm);
                }

            _alarmDialog.ShowAlarms(_serverAlarms, false);
        }

        private void _bImportGS_Click(object sender, EventArgs e)
        {
            var graphicSymbolsForm = new GraphicSymbolsForm(Plugin.MainServerProvider);
            graphicSymbolsForm.Show();
        }

        public string GetLocalizationString(string key)
        {
            return GetString(key);
        }

        private bool _isAlarmDialogOpen;

        private void _bHideOrShow_Click(object sender, EventArgs e)
        {
            if (_isAlarmDialogOpen)
            {
                splitContainer1.Panel2Collapsed = true;
                _isAlarmDialogOpen = false;
                _bHideOrShow.Image = ResourceGlobal.show12;
            }
            else
            {
                splitContainer1.Panel2Collapsed = false;
                _isAlarmDialogOpen = true;
                splitContainer1.Panel2MinSize = _alarmDialog.MinimumSize.Height;
                splitContainer1.SplitterDistance = Height - splitContainer1.Panel2MinSize;
                _panelAlarms.Size = new Size(splitContainer1.Panel2.Width, splitContainer1.Panel2.Height);
                _bHideOrShow.Image = ResourceGlobal.hide12;
            }
        }
    }
}
