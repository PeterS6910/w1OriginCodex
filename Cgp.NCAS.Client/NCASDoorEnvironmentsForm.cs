using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Localization;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Definitions;
using Contal.IwQuick.Sys;


namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASDoorEnvironmentsForm :
#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, DoorEnvironment, DoorEnvironmentShort>
#endif
    {
        Action<Guid, byte> _eventStateChanged;
        private bool _showOnlyConfigured = true;
        private ItemDoorEnvironment _ccuFilterParameter;

        public NCASDoorEnvironmentsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.Doorenvironment48;
            InitializeComponent();
            _chbShowOnlyConfigured.Checked = true;
            FormOnEnter += FormEnterHF;
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
            _cdgvData.EnabledInsertButton = false;
            _cdgvData.EnabledDeleteButton = false;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (DoorEnvironmentShort de in bindingSource)
            {
                de.Symbol = _cdgvData.GetDefaultImage(de);
                de.StringState = TranslateDoorEnvironmentState(de.State);
            }
        }

        protected override DoorEnvironment GetObjectForEdit(DoorEnvironmentShort listObj, out bool editAllowed)
        {
            DoorEnvironment doorEnvironment = Plugin.MainServerProvider.DoorEnvironments.GetObjectById(listObj.IdDoorEnvironment);

            editAllowed = HasAccessView();
            return doorEnvironment;
        }

        protected override DoorEnvironment GetFromShort(DoorEnvironmentShort listObj)
        {
            return Plugin.MainServerProvider.DoorEnvironments.GetObjectById(listObj.IdDoorEnvironment);
        }

        protected override void AfterTranslateForm()
        {
            TranslateDgValues();
        }

        private void FormEnterHF(Form form)
        {
            FillCb();
        }

        const string columnFullName = "FullName";
        const string columnObjectType = "ObjectType";
        const string columnGuid = "Guid";

        private static volatile NCASDoorEnvironmentsForm _singleton;
        private static object _syncRoot = new object();

        public static NCASDoorEnvironmentsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASDoorEnvironmentsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }
                return _singleton;
            }
        }

        protected override ICollection<DoorEnvironmentShort> GetData()
        {
            Exception error;

            var list = Plugin.MainServerProvider.
                DoorEnvironments.GetShortSelectByCriteria(FilterSettings, out error);

            if (error != null)
                throw (error);

            if (list != null)
            {
                if (_ccuFilterParameter != null)
                {
                    if (_ccuFilterParameter.CCU != null)
                    {
                        list = new LinkedList<DoorEnvironmentShort>(
                            list.Where(
                                doorEnvironment =>
                                    doorEnvironment.IdCcu.Equals(_ccuFilterParameter.CCU.IdCCU)));
                    }
                    else
                    {
                        list = new LinkedList<DoorEnvironmentShort>(
                            list.Where(
                                doorEnvironment =>
                                    doorEnvironment.HasDcu));
                    }
                }

                if (_showOnlyConfigured)
                {
                    list = new LinkedList<DoorEnvironmentShort>(
                        list.Where(
                            doorEnvironment =>
                                doorEnvironment.Configured));
                }
            }
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
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            try
            {
                _cdgvData.ModifyGridView(bindingSource, DoorEnvironmentShort.COLUMN_SYMBOL, DoorEnvironmentShort.COLUMNFULLNAME, DoorEnvironmentShort.COLUMNSTRINGSTATE,
                    DoorEnvironmentShort.COLUMNNAME, DoorEnvironmentShort.COLUMNDESCRIPTION);
            }
            catch
            { }
        }

        private void TranslateDoorEnvironmentState(DataGridViewRow row, DoorEnvironmentState doorEnvironmentState)
        {
            row.Cells[DoorEnvironmentShort.COLUMNSTRINGSTATE].Value = TranslateDoorEnvironmentState(doorEnvironmentState);
        }

        public string TranslateDoorEnvironmentState(DoorEnvironmentState doorEnvironmentState)
        {
            switch (doorEnvironmentState)
            {
                case DoorEnvironmentState.Locked:
                    return GetString("DoorEnvironmentStateLocked");
                case DoorEnvironmentState.Locking:
                    return GetString("DoorEnvironmentStateLocking");
                case DoorEnvironmentState.Opened:
                    return GetString("DoorEnvironmentStateOpened");
                case DoorEnvironmentState.Unlocked:
                    return GetString("DoorEnvironmentStateUnlocked");
                case DoorEnvironmentState.Unlocking:
                    return GetString("DoorEnvironmentStateUnlocking");
                case DoorEnvironmentState.AjarPrewarning:
                    return GetString("DoorEnvironmentStateDoorAjarPrewarning");
                case DoorEnvironmentState.Ajar:
                    return GetString("DoorEnvironmentStateDoorAjar");
                case DoorEnvironmentState.Intrusion:
                    return GetString("DoorEnvironmentStateIntrusion");
                case DoorEnvironmentState.Sabotage:
                    return GetString("DoorEnvironmentStateSatotage");
                case DoorEnvironmentState.Unknown:
                    return GetString("DoorEnvironmentStateUnknown");
                default:
                    return string.Empty;
            }
        }

        private void TranslateDgValues()
        {
            try
            {
                foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                {
                    var deShort = (DoorEnvironmentShort)((BindingSource)_cdgvData.DataGrid.DataSource).List[row.Index];
                    TranslateDoorEnvironmentState(row, deShort.State);
                }
            }
            catch { }
        }

        private void DoorEnvironmentStateChanged(Guid doorEnvironmentGuid, byte state)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<Guid, byte>(DoorEnvironmentStateChanged), doorEnvironmentGuid, state);
                }
                else
                {
                    foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
                    {
                        var deShort = (DoorEnvironmentShort)((BindingSource)_cdgvData.DataGrid.DataSource).List[row.Index];

                        if (deShort != null && deShort.IdDoorEnvironment == doorEnvironmentGuid)
                        {
                            row.Cells[DoorEnvironmentShort.COLUMNSTATE].Value = (DoorEnvironmentState)state;
                            TranslateDoorEnvironmentState(row, (DoorEnvironmentState)state);
                            return;
                        }
                    }
                }
            }
            catch { }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.DataGrid.DataSource = null;
        }

        protected override void DeleteObj(DoorEnvironment obj)
        {
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();

            if (_eNameFilter.Text != "")
            {
                var filterSettingName = new FilterSettings(DoorEnvironment.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSettingName);
            }

            _ccuFilterParameter = _cbControlUnit.SelectedItem as ItemDoorEnvironment;

            //if (_cbControlUnit.SelectedItem != null)
            //{
            //    var filterSettingCcu = new FilterSettings(DoorEnvironment.COLUMNCCU,
            //        ((ItemDoorEnvironment)_cbControlUnit.SelectedItem).CCU, ComparerModes.EQUALL);

            //    FilterSettings.Add(filterSettingCcu);
            //}
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = "";
        }

        protected override bool Compare(DoorEnvironment obj1, DoorEnvironment obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(DoorEnvironment obj, Guid idObj)
        {
            return obj.IdDoorEnvironment == idObj;
        }

        protected override ACgpPluginEditForm<NCASClient, DoorEnvironment> CreateEditForm(DoorEnvironment obj, ShowOptionsEditForm showOption)
        {
            return new NCASDoorEnvironmentEditForm(obj, showOption, this);
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.DoorEnvironments.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(DoorEnvironment doorEnvironment)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.DoorEnvironments.HasAccessViewForObject(doorEnvironment);
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
                    return Plugin.MainServerProvider.DoorEnvironments.HasAccessInsert();
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
                    return Plugin.MainServerProvider.DoorEnvironments.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_cdgvData.DataGrid.SelectedRows.Count == 1)
            {
                Edit_Click();
            }
            else
            {
                DataGridViewSelectedRowCollection selected = _cdgvData.DataGrid.SelectedRows;
                for (int i = 0; i < selected.Count; i++)
                {
                    EditFromPosition(selected[i].Index);
                }
            }
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
            //_bRunFilter_Click(sender, e);
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        private void FillCb()
        {
            try
            {
                object selected = null;
                if (_cbControlUnit.Items.Count != 0 &&
                    _cbControlUnit.SelectedItem is ItemDoorEnvironment)
                {
                    selected = _cbControlUnit.SelectedItem;
                }


                _cbControlUnit.Items.Clear();
                _cbControlUnit.Items.Add(String.Empty);

                Exception error;
                var ccus = Plugin.MainServerProvider.CCUs.List(out error).ToList();
                var itemDoorEnvironments = ItemDoorEnvironment.GetList(ccus, NCASClient.LocalizationHelper);

                foreach (ItemDoorEnvironment ide in itemDoorEnvironments)
                {
                    _cbControlUnit.Items.Add(ide);
                }

                if (selected != null &&
                    _cbControlUnit.Items.Contains(selected))
                {
                    _cbControlUnit.SelectedItem = selected;
                }
            }
            catch
            { }
        }

        private void _cbControlUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterValueChanged(sender, e);
            RunFilter();
        }

        protected override void FilterKeyDown(object sender, KeyEventArgs e)
        {
            base.FilterKeyDown(sender, e);
        }

        protected override void RegisterEvents()
        {
            if (_eventStateChanged == null)
            {
                _eventStateChanged = DoorEnvironmentStateChanged;
                DoorEnvironmentStateChangedHandler.Singleton.RegisterStateChanged(_eventStateChanged);
            }
        }

        protected override void UnregisterEvents()
        {
            if (_eventStateChanged != null)
            {
                DoorEnvironmentStateChangedHandler.Singleton.UnregisterStateChanged(_eventStateChanged);
                _eventStateChanged = null;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.MdiFormClosing)
            {
                _showOnlyConfigured = true;
                _chbShowOnlyConfigured.Checked = true;
            }

            base.OnFormClosing(e);
        }

        private void _chbShowOnlyConfigured_CheckedChanged(object sender, EventArgs e)
        {
            if (_showOnlyConfigured == _chbShowOnlyConfigured.Checked)
                return;

            _showOnlyConfigured = _chbShowOnlyConfigured.Checked;
            ShowData();
        }
    }

    public class ItemDoorEnvironment
    {
        private CCU _ccu;
        private bool _dcuOnly;
        private LocalizationHelper _localizationHelper;

        public CCU CCU { get { return _ccu; } }

        public ItemDoorEnvironment(CCU ccu, LocalizationHelper localizationHelper)
        {
            _ccu = ccu;
            if (ccu == null)
            {
                _dcuOnly = true;
            }
            else
            {
                _dcuOnly = false;
            }
            _localizationHelper = localizationHelper;
        }

        public override string ToString()
        {
            if (_dcuOnly)
            {
                return _localizationHelper.GetString("InfoAllDcus");
            }
            if (_ccu != null)
            {
                return _ccu.ToString();
            }
            return string.Empty;
        }

        public static IList<ItemDoorEnvironment> GetList(IList<CCU> ccus, LocalizationHelper localizationHelper)
        {
            IList<ItemDoorEnvironment> list = new List<ItemDoorEnvironment>();
            //ICollection<CCU> ccus = (this.Plugin as NCASClient).MainServerProvider.CCUs.List(out error);
            foreach (CCU ccu in ccus)
            {
                list.Add(new ItemDoorEnvironment(ccu, localizationHelper));
            }
            list.Add(new ItemDoorEnvironment(null, localizationHelper));

            return list;
        }

        public override bool Equals(object obj)
        {
            if (obj is ItemDoorEnvironment)
            {
                if ((obj as ItemDoorEnvironment).CCU.IdCCU == CCU.IdCCU)
                    return true;
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (CCU != null && CCU.IdCCU != null)
                return CCU.IdCCU.GetHashCode();
            return base.GetHashCode();
        }
    }
}
