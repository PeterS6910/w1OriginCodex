using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASGraphicsViewsForm :
#if DESIGNER
        Form
#else
 ACgpPluginTableForm<NCASClient, GraphicsView, GraphicsViewShort>
#endif
    {
        private static volatile NCASGraphicsViewsForm _singleton;
        private static object _syncRoot = new object();

        public NCASGraphicsViewsForm()
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

        public static NCASGraphicsViewsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASGraphicsViewsForm();
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

        public override bool HasAccessView(GraphicsView graphicsView)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.GraphicsViews.HasAccessViewForObject(graphicsView);
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

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        protected override ICollection<GraphicsViewShort> GetData()
        {
            Exception error;
            ICollection<GraphicsViewShort> list =
                Plugin.MainServerProvider.GraphicsViews.ShortSelectByCriteria(FilterSettings, out error);

            if (error != null)
                throw (error);

            //foreach (var graphicsViewShort in list)
            //{
            //    if (graphicsViewShort.GraphicsViewScreenRowData != null)
            //        graphicsViewShort.GraphicsViewScreen = GetImageFromByteArray(graphicsViewShort.GraphicsViewScreenRowData);
            //}

            CheckAccess();

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
            _dgvData.ModifyGridView(bindingSource, SceneShort.COLUMN_NAME, SceneShort.COLUMN_DESCRIPTION/*, SceneShort.COLUMN_SCENESCREEN*/);
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

        protected override ACgpPluginEditForm<NCASClient, GraphicsView> CreateEditForm(GraphicsView obj, ShowOptionsEditForm showOption)
        {
            return new NCASGraphicsViewEditForm(obj, showOption, this);
        }

        protected override bool Compare(GraphicsView obj1, GraphicsView obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(GraphicsView obj, Guid idObj)
        {
            return obj.IdGraphicsView == idObj;
        }

        protected override void DeleteObj(GraphicsView obj)
        {
            if (!AddToDeletedCcus(obj.IdGraphicsView))
            {
                //Dialog.Info(GetString("InfoSceneIsDeleting"));
                return;
            }
            SafeThread<GraphicsView>.StartThread(ThreadDeleteGraphicsView, obj);
        }

        private List<Guid> _actualDeletingGraphicsViews = new List<Guid>();

        private void ThreadDeleteGraphicsView(GraphicsView obj)
        {
            Guid idGraphicsView = obj.IdGraphicsView;
            try
            {
                Exception error;
                if (Plugin.MainServerProvider.GraphicsViews.Delete(obj, out error))
                {
                    ShowData();
                    //Dialog.Info("Scene: " + obj.Name + " " + GetString("SceneWasDeleted"));
                }
                else
                {
                    if (error is IwQuick.SqlDeleteReferenceConstraintException)
                        Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteRowInRelationship"));
                    else
                        Dialog.Error("Scene: " + obj.Name + " " + GetString("SceneDeleteFailed"));
                }
            }
            catch { }
            finally
            {
                RemoveFromDeletingScenes(idGraphicsView);
            }
        }

        private bool AddToDeletedCcus(Guid idGraphicsView)
        {
            try
            {
                if (_actualDeletingGraphicsViews.Contains(idGraphicsView))
                    return false;
                _actualDeletingGraphicsViews.Add(idGraphicsView);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void RemoveFromDeletingScenes(Guid idGraphicsView)
        {
            try
            {
                _actualDeletingGraphicsViews.Remove(idGraphicsView);
            }
            catch { }
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();

            if (_tbName.Text != "")
            {
                var filterSettingName = new FilterSettings(Scene.COLUMN_NAME, _tbName.Text, ComparerModes.LIKEBOTH);
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

        protected override GraphicsView GetObjectForEdit(GraphicsViewShort listObj, out bool editEnabled)
        {
            return Plugin.MainServerProvider.GraphicsViews.GetObjectForEditById(listObj.IdGraphicsView, out editEnabled);
        }

        protected override GraphicsView GetFromShort(GraphicsViewShort listObj)
        {
            if (listObj == null)
                return null;

            return Plugin.MainServerProvider.GraphicsViews.GetObjectById(listObj.IdGraphicsView);
        }

        private void ViewScene(object sender, EventArgs e)
        {
            View_Click();
        }

        private void _bFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }
    }
}
