using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASMultiDoorElementsForm :
#if DESIGNER
    Form
#else
    ACgpPluginTableForm<NCASClient, MultiDoorElement, MultiDoorElementShort>
#endif
    {
        private static volatile NCASMultiDoorElementsForm _singleton;
        private static readonly object SyncRoot = new object();

        public static NCASMultiDoorElementsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (SyncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASMultiDoorElementsForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        public NCASMultiDoorElementsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            FormImage = ResourceGlobal.MultiDoorElement48;
            InitCGPDataGridView();
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
            foreach (MultiDoorElementShort multiDoorElementShort in bindingSource.List)
            {
                multiDoorElementShort.Symbol = _cdgvData.GetDefaultImage(multiDoorElementShort);

                multiDoorElementShort.StringState = NCASDCUsForm.GetTranslatedDoorEnvironmentState(
                    multiDoorElementShort.State,
                    LocalizationHelper);
            }
        }

        private void TranslateMultiDoorElementsState()
        {
            TranslateMultiDoorElementsState(null, DoorEnvironmentState.Unknown);
        }

        private void TranslateMultiDoorElementsState(Guid? multiDoorElementId, DoorEnvironmentState state)
        {
            var bindingSource = _cdgvData.DataGrid.DataSource as BindingSource;

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

                    break;
                }

                multiDoorElementShort.StringState = NCASDCUsForm.GetTranslatedDoorEnvironmentState(
                    multiDoorElementShort.State,
                    LocalizationHelper);
            }

            _cdgvData.Refresh();
        }

        protected override void AfterTranslateForm()
        {
            TranslateMultiDoorElementsState();
        }

        protected override ICollection<MultiDoorElementShort> GetData()
        {
            Exception error;
            ICollection<MultiDoorElementShort> list =
                Plugin.MainServerProvider.MultiDoorElements.ShortSelectByCriteria(FilterSettings, out error);

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
                    return Plugin.MainServerProvider.MultiDoorElements.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(MultiDoorElement multiDoorElement)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return
                        Plugin.MainServerProvider.MultiDoorElements.HasAccessViewForObject(
                            multiDoorElement);
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
                    return Plugin.MainServerProvider.MultiDoorElements.HasAccessInsert();
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
                    return Plugin.MainServerProvider.MultiDoorElements.HasAccessDelete();
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
                MultiDoorElementShort.ColumnSymbol,
                MultiDoorElementShort.ColumnName,
                MultiDoorElementShort.ColumnDoorIndex,
                MultiDoorElementShort.ColumnStringState,
                MultiDoorElementShort.ColumnDescription);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ACgpPluginEditForm<NCASClient, MultiDoorElement> CreateEditForm(MultiDoorElement obj, ShowOptionsEditForm showOption)
        {
            return new NCASMultiDoorElementEditForm(obj, showOption, this);
        }

        protected override MultiDoorElement GetObjectForEdit(MultiDoorElementShort listObj, out bool editEnabled)
        {
            return Plugin.MainServerProvider.MultiDoorElements.GetObjectForEditById(
                listObj.Id,
                out editEnabled);
        }

        protected override MultiDoorElement GetFromShort(MultiDoorElementShort listObj)
        {
            return Plugin.MainServerProvider.MultiDoorElements.GetObjectById(listObj.Id);
        }

        protected override bool Compare(MultiDoorElement obj1, MultiDoorElement obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(MultiDoorElement obj, Guid idMultiDoorElement)
        {
            return obj.IdMultiDoorElement == idMultiDoorElement;
        }

        protected override void DeleteObj(MultiDoorElement obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.MultiDoorElements.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != string.Empty)
            {
                var filterSetting = new FilterSettings(
                    MultiDoorElement.ColumnName,
                    _eNameFilter.Text,
                    ComparerModes.LIKEBOTH);

                FilterSettings.Add(filterSetting);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
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
                    (DoorEnvironmentState) state);
            }
        }

        private bool _eventsAreReginstred; 
        protected override void RegisterEvents()
        {
            if (_eventsAreReginstred)
                return;
            
            _eventsAreReginstred = true;
            DoorEnvironmentStateChangedHandler.Singleton.RegisterStateChanged(StateChanged);
        }

        protected override void UnregisterEvents()
        {
            if (!_eventsAreReginstred)
                return;

            _eventsAreReginstred = false;
            DoorEnvironmentStateChangedHandler.Singleton.UnregisterStateChanged(StateChanged);
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
    }
}
