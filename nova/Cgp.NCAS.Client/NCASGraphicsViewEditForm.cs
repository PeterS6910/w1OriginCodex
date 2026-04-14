using System;
using System.Windows.Forms;
using Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASGraphicsViewEditForm :
#if DESIGNER    
        Form
#else
 ACgpPluginEditForm<NCASClient, GraphicsView>
#endif
    {
        private readonly GraphicsSceneEventsHelper _graphicsSceneEventsHelper = new GraphicsSceneEventsHelper();

        //private byte[] array = new byte[100000000];

        public NCASGraphicsViewEditForm(
            GraphicsView graphicsView,
            ShowOptionsEditForm options,
            PluginMainForm<NCASClient> myTableForm)
            : base(
                graphicsView,
                options == ShowOptionsEditForm.View
                    ? ShowOptionsEditForm.Edit
                    : options,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            if (GraphicsScene.LocalizationHelper == null)
                GraphicsScene.LocalizationHelper = NCASClient.LocalizationHelper;

            if (GraphicsScene.MainServerProvider == null)
                GraphicsScene.MainServerProvider = Plugin.MainServerProvider;

            if (GraphicsViewControl.MainServerProvider == null)
                GraphicsViewControl.MainServerProvider = Plugin.MainServerProvider;

            InitializeComponent();
            graphicsViewControl.SaveView += graphicsView_SaveView;
            graphicsViewControl.EditMode = options != ShowOptionsEditForm.View;
            graphicsViewControl.GraphicsSceneEventsHelper = _graphicsSceneEventsHelper;
            _graphicsSceneEventsHelper.EditGraphicsSymbolClick += graphicsControl_EditObjectClick;
        }

        void graphicsView_SaveView()
        {
            _editingObject = graphicsViewControl.GraphicsView;
            EditTextChanger(this, null);
            Apply_Click();
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
            _graphicsSceneEventsHelper.Initialize();
        }

        protected override void BeforeInsert()
        {
            NCASGraphicsViewsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASGraphicsViewsForm.Singleton.BeforeEdit(this, _editingObject, true);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj = Plugin.MainServerProvider.GraphicsViews.GetObjectForEdit(_editingObject.IdGraphicsView, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.GraphicsViews.GetObjectById(_editingObject.IdGraphicsView);
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
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.Scenes.RenewObjectForEdit(_editingObject.IdGraphicsView, out error);

            if (error != null)
                throw error;
        }

        protected override void SetValuesInsert()
        {
        }

        protected override void SetValuesEdit()
        {
            //var random = new Random();

            //MessageBox.Show(array[random.Next(0, 100000000 - 1)].ToString());
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
            bool retValue = Plugin.MainServerProvider.GraphicsViews.Insert(ref _editingObject, out error);
            
            if (!retValue && error != null)
            {
                    throw error;
            }

            AfterInsert();

            return retValue;
        }

        protected override bool SaveToDatabaseEdit()
        {
            Exception error;
            bool retValue;
            retValue = Plugin.MainServerProvider.GraphicsViews.UpdateOnlyInDatabase(_editingObject, out error);            
            return retValue;
        }

        protected override void UnregisterEvents()
        {
            _graphicsSceneEventsHelper.RemoveAllGraphicsScenes();
            _graphicsSceneEventsHelper.Deinitialize();
            graphicsViewControl.DisposeLocalizationHelper();
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.AccessControlLists != null)
                Plugin.MainServerProvider.GraphicsViews.EditEnd(_editingObject);
        }

        protected override void AfterInsert()
        {
            NCASGraphicsViewsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASGraphicsViewsForm.Singleton.AfterEdit(_editingObject);
        }

        public string GetLocalizationString(string key)
        {
            return GetString(key);
        }

        private void NCASGraphicsViewEditForm_Load(object sender, EventArgs e)
        {
            graphicsViewControl.Focus();
            WindowState = FormWindowState.Maximized;
            graphicsViewControl.GraphicsView = _editingObject;
        }
    }
}
