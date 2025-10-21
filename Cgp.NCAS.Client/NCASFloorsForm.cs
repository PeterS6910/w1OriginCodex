using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASFloorsForm :
#if DESIGNER
    Form
#else
    ACgpPluginTableForm<NCASClient, Floor, FloorShort>
#endif
    {
        private static volatile NCASFloorsForm _singleton;
        private static readonly object SyncRoot = new object();

        public static NCASFloorsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (SyncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASFloorsForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        public NCASFloorsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            FormImage = ResourceGlobal.Floor48;
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin) NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        private void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (FloorShort floorShort in bindingSource.List)
            {
                floorShort.Symbol = _cdgvData.GetDefaultImage(floorShort);
            }
        }

        protected override ICollection<FloorShort> GetData()
        {
            Exception error;
            var list =
                Plugin.MainServerProvider.Floors.ShortSelectByCriteria(FilterSettings, out error);

            if (error != null)
                throw (error);

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
                BeginInvoke(new DVoid2Void(CheckAccess));
            else
            {
                _cdgvData.EnabledInsertButton = HasAccessInsert();
                _cdgvData.EnabledDeleteButton = HasAccessDelete();
            }
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Floors.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(Floor floor)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Floors.HasAccessViewForObject(floor);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Floors.HasAccessInsert();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private bool HasAccessDelete()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.Floors.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(
                bindingSource,
                FloorShort.ColumnSymbol,
                FloorShort.ColumnName,
                FloorShort.ColumnFloorNumber,
                FloorShort.ColumnDescription);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ACgpPluginEditForm<NCASClient, Floor> CreateEditForm(Floor obj, ShowOptionsEditForm showOption)
        {
            return new NCASFloorEditForm(obj, showOption, this);
        }

        protected override Floor GetObjectForEdit(FloorShort listObj, out bool editEnabled)
        {
            return Plugin.MainServerProvider.Floors.GetObjectForEditById(listObj.Id, out editEnabled);
        }

        protected override Floor GetFromShort(FloorShort listObj)
        {
            return Plugin.MainServerProvider.Floors.GetObjectById(listObj.Id);
        }

        protected override bool Compare(Floor obj1, Floor obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(Floor obj, Guid idFloor)
        {
            return obj.IdFloor == idFloor;
        }

        protected override void DeleteObj(Floor obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.Floors.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != string.Empty)
            {
                var filterSetting = new FilterSettings(
                    FloorShort.ColumnName,
                    _eNameFilter.Text,
                    ComparerModes.LIKEBOTH);

                FilterSettings.Add(filterSetting);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
        }

        protected override void RegisterEvents()
        {
            
        }

        protected override void UnregisterEvents()
        {
            
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        private void _eNameFilter_KeyDown(object sender, KeyEventArgs e)
        {
            FilterKeyDown(sender, e);
        }

        private void _eNameFilter_TextChanged(object sender, EventArgs e)
        {
            FilterValueChanged(sender, e);
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_cdgvData.SelectedRows.Count == 0)
                return;

            if (_cdgvData.SelectedRows.Count == 1)
            {
                Edit_Click();
            }
            else
            {
                DataGridViewSelectedRowCollection selected = _cdgvData.SelectedRows;
                for (int i = 0; i < selected.Count; i++)
                {
                    EditFromPosition(selected[i].Index);
                }
            }
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_cdgvData.SelectedRows.Count == 0)
                return;

            if (_cdgvData.SelectedRows.Count == 1)
            {
                Delete_Click();
            }
            else
            {
                MultiselectDeleteClic(_cdgvData.SelectedRows);
            }
        }
    }
}
