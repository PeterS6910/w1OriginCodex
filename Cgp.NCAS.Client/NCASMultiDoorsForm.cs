using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class NCASMultiDoorsForm :
#if DESIGNER
    Form
#else
    ACgpPluginTableForm<NCASClient, MultiDoor, MultiDoorShort>
#endif
    {
        private static volatile NCASMultiDoorsForm _singleton;
        private static readonly object SyncRoot = new object();
        private readonly ICollection<MultiDoorTypeItem> _multiDoorTypeItems;

        public static NCASMultiDoorsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (SyncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASMultiDoorsForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        public NCASMultiDoorsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            FormImage = ResourceGlobal.MultiDoor48;
            InitCGPDataGridView();

            _multiDoorTypeItems = MultiDoorTypeItem.GetMultiDoorTypeItems();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        private void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (MultiDoorShort multiDoorShort in bindingSource.List)
            {
                multiDoorShort.Symbol = _cdgvData.GetDefaultImage(multiDoorShort);
                multiDoorShort.StringType =
                    GetString(string.Format(
                        "MultiDoorType_{0}",
                        multiDoorShort.Type));
            }
        }

        protected override void AfterTranslateForm()
        {
            var selectedMultiDoorTypeItem = _cbTypeFilter.SelectedItem as MultiDoorTypeItem;

            _cbTypeFilter.Items.Clear();

            foreach (var multiDoorTypeItem in _multiDoorTypeItems)
            {
                multiDoorTypeItem.Translate(LocalizationHelper);
            }

            _cbTypeFilter.Items.Add(string.Empty);

            _cbTypeFilter.Items.AddRange(_multiDoorTypeItems.Cast<object>().ToArray());

            if (selectedMultiDoorTypeItem != null)
            {
                _cbTypeFilter.SelectedItem = selectedMultiDoorTypeItem;
            }
            else
            {
                _cbTypeFilter.SelectedItem = string.Empty;
            }

            var bindingSource = _cdgvData.DataGrid.DataSource as BindingSource;

            if (bindingSource == null)
                return;

            foreach (MultiDoorShort multiDoorShort in bindingSource.List)
            {
                multiDoorShort.StringType =
                    GetString(string.Format(
                        "MultiDoorType_{0}",
                        multiDoorShort.Type));
            }
        }

        protected override ICollection<MultiDoorShort> GetData()
        {
            Exception error;
            var list =
                Plugin.MainServerProvider.MultiDoors.ShortSelectByCriteria(FilterSettings, out error);

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
                    return Plugin.MainServerProvider.MultiDoors.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(MultiDoor multiDoor)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.MultiDoors.HasAccessViewForObject(multiDoor);
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
                    return Plugin.MainServerProvider.MultiDoors.HasAccessInsert();
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
                    return Plugin.MainServerProvider.MultiDoors.HasAccessDelete();
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
                MultiDoorShort.ColumnSymbol,
                MultiDoorShort.ColumnName,
                MultiDoorShort.ColumnStringType,
                MultiDoorShort.ColumnDescription);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ACgpPluginEditForm<NCASClient, MultiDoor> CreateEditForm(MultiDoor obj, ShowOptionsEditForm showOption)
        {
            return new NCASMultiDoorEditForm(obj, showOption, this);
        }

        protected override MultiDoor GetObjectForEdit(MultiDoorShort listObj, out bool editEnabled)
        {
            return Plugin.MainServerProvider.MultiDoors.GetObjectForEditById(listObj.Id, out editEnabled);
        }

        protected override MultiDoor GetFromShort(MultiDoorShort listObj)
        {
            return Plugin.MainServerProvider.MultiDoors.GetObjectById(listObj.Id);
        }

        protected override bool Compare(MultiDoor obj1, MultiDoor obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(MultiDoor obj, Guid idMultiDoor)
        {
            return obj.IdMultiDoor == idMultiDoor;
        }

        protected override void DeleteObj(MultiDoor obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.MultiDoors.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != string.Empty)
            {
                var filterSetting = new FilterSettings(
                    MultiDoorShort.ColumnName,
                    _eNameFilter.Text,
                    ComparerModes.LIKEBOTH);

                FilterSettings.Add(filterSetting);
            }

            var multiDoorTypeItem = _cbTypeFilter.SelectedItem as MultiDoorTypeItem;
            if (multiDoorTypeItem != null)
            {
                var filterSetting = new FilterSettings(
                    MultiDoorShort.ColumnType,
                    multiDoorTypeItem.Type,
                    ComparerModes.EQUALL);

                FilterSettings.Add(filterSetting);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
            _cbTypeFilter.SelectedItem = string.Empty;
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

        private void _eNameFilter_KeyDown(object sender, KeyEventArgs e)
        {
            FilterKeyDown(sender, e);
        }

        private void _eNameFilter_TextChanged(object sender, EventArgs e)
        {
            FilterValueChanged(sender, e);
        }

        private void _cbTypeFilter_SelectedValueChanged(object sender, EventArgs e)
        {
            FilterValueChanged(sender, e);
        }
    }
}
