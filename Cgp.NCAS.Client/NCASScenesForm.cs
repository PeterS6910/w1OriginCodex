using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
// ReSharper disable once RedundantUsingDirective
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

using Image = System.Drawing.Image;
using Contal.IwQuick.Sys;

using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASScenesForm :
#if DESIGNER
        Form
#else
 ACgpPluginTableForm<NCASClient, Scene, SceneShort>
#endif
    {
        private static volatile NCASScenesForm _singleton;
        private static object _syncRoot = new object();

        public NCASScenesForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.Scene48;
            InitializeComponent();
            InitCGPDataGridView();
        }

        protected override bool CheckLicense(out string errorMessage)
        {
            errorMessage = null;
#if !DEBUG
            string localisedName;
            object licenseValue;
            if (!CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(
                RequiredLicenceProperties.Graphics.ToString(), out localisedName, out licenseValue) ||
                !(bool) licenseValue)
            {
                errorMessage = GetString("ErrorGraphicNotSupportedByLicense");
                return false;
            }
#endif

            return true;
        }

        public static NCASScenesForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASScenesForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        private void InitCGPDataGridView()
        {
            _dgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _dgvData.ImageList = NCASClient.Singleton.GetPluginObjectsImages();
            _dgvData.DataGrid.MouseDoubleClick += _cdgvData_MouseDoubleClick;
            _dgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _dgvData.AfterGridModified += _cdgvData_AfterGridModified;
            _dgvData.AfterSort += AfterDataChanged;
            _dgvData.DataGrid.RowTemplate.Height = 240;
            _dgvData.CgpDataGridEvents = this;
            _dgvData.AutoOpenEditFormByDoubleClick = false;
        }

        private void _cdgvData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_dgvData.DataGrid.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            ViewScene(sender, e);
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Scenes.HasAccessInsert();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Scenes.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(Scene scene)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Scenes.HasAccessViewForObject(scene);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public bool HasAccessDelete()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Scenes.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
        }

        private void AfterDataChanged(IList dataSourceList)
        {
        }

        void _cdgvData_AfterGridModified(BindingSource bindingSource)
        {
            AfterDataChanged(bindingSource.List);
        }

        private void NCASScenesForm_Load(object sender, EventArgs e)
        {
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        protected override ICollection<SceneShort> GetData()
        {
            Exception error;
            ICollection<SceneShort> list =
                Plugin.MainServerProvider.Scenes.ShortSelectByCriteria(FilterSettings, out error);

            if (error != null)
                throw (error);

            foreach (SceneShort sceneShort in list)
            {
                if (sceneShort.SceneScreenRowData != null)
                    sceneShort.SceneScreen = GetImageFromByteArray(sceneShort.SceneScreenRowData);
            }

            CheckAccess();
            _lRecordCount.BeginInvoke(new Action(
            () =>
            {
                _lRecordCount.Text = string.Format("{0} : {1}",
                                                        GetString("TextRecordCount"),
                                                        list == null
                                                            ? 0
                                                            : list.Count);
            }));
            return list;
        }

        private void CheckAccess()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(CheckAccess));
            }
            else
            {
                _dgvData.EnabledInsertButton = HasAccessInsert();
                _dgvData.EnabledDeleteButton = HasAccessDelete();
            }
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _dgvData.ModifyGridView(bindingSource, SceneShort.COLUMN_NAME, SceneShort.COLUMN_DESCRIPTION, SceneShort.COLUMN_SCENESCREEN);
        }

        private Image GetImageFromByteArray(byte[] rowData)
        {
            try
            {
                var ms = new MemoryStream(rowData);
                ms.Position = 0;
                var img = Image.FromStream(ms);
                return img;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override void RemoveGridView()
        {
        }

        protected override ACgpPluginEditForm<NCASClient, Scene> CreateEditForm(Scene obj, ShowOptionsEditForm showOption)
        {
            return new NCASSceneEditForm(obj, showOption, this);
        }

        protected override bool Compare(Scene obj1, Scene obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(Scene obj, Guid idObj)
        {
            return obj.IdScene == idObj;
        }

        protected override void DeleteObj(Scene obj)
        {
            if (!AddToDeletedCcus(obj.IdScene))
            {
                //Dialog.Info(GetString("InfoSceneIsDeleting"));
                return;
            }
            SafeThread<Scene>.StartThread(ThreadDeleteScene, obj);
        }

        private List<Guid> _actualDeletingScenes = new List<Guid>();

        private void ThreadDeleteScene(Scene obj)
        {
            Guid idScene = obj.IdScene;
            try
            {
                Exception error;
                if (Plugin.MainServerProvider.Scenes.Delete(obj, out error))
                {
                    ShowData();
                    //Dialog.Info("Scene: " + obj.Name + " " + GetString("SceneWasDeleted"));
                }
                else
                {
                    if (error is SqlDeleteReferenceConstraintException)
                        Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteRowInRelationship"));
                    else
                        Dialog.Error("Scene: " + obj.Name + " " + GetString("SceneDeleteFailed"));
                }
            }
            catch { }
            finally
            {
                RemoveFromDeletingScenes(idScene);
            }
        }

        private bool AddToDeletedCcus(Guid idScene)
        {
            try
            {
                if (_actualDeletingScenes.Contains(idScene))
                    return false;
                _actualDeletingScenes.Add(idScene);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void RemoveFromDeletingScenes(Guid idScene)
        {
            try
            {
                _actualDeletingScenes.Remove(idScene);
            }
            catch { }
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_tbName.Text != "")
            {
                FilterSettings filterSettingName = new FilterSettings(Scene.COLUMN_NAME, _tbName.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSettingName);
            }
        }

        protected override void ClearFilterEdits()
        {
            _tbName.Text = "";
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        protected override Scene GetObjectForEdit(SceneShort listObj, out bool editEnabled)
        {
            return Plugin.MainServerProvider.Scenes.GetObjectForEditById(listObj.IdScene, out editEnabled);
        }

        protected override Scene GetFromShort(SceneShort listObj)
        {
            if (listObj == null) 
                return null;

            return Plugin.MainServerProvider.Scenes.GetObjectById(listObj.IdScene);
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            Edit_Click();
        }

        private void ViewScene(object sender, EventArgs e)
        {
            View_Click();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            Delete_Click();
        }

        private void _bFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
        }
    }
}
