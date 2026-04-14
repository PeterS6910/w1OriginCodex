using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAlarmAreasForm :
#if DESIGNER
        Form
#else
 ACgpPluginTableForm<NCASClient, AlarmArea, AlarmAreaShort>
#endif
    {
        Action<Guid, byte> _eventActivationStateChanged;
        Action<Guid, byte, bool> _eventRequestActivationStateChanged;
        Action<Guid, byte> _eventAlarmStateChanged;

        public NCASAlarmAreasForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.AlarmAreasNew48;
            InitializeComponent();
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin) NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.AfterGridModified += _cdgvData_AfterGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (AlarmAreaShort aaShort in bindingSource.List)
            {
                aaShort.Symbol = _cdgvData.GetDefaultImage(aaShort);
                aaShort.StringActivationState = GetTranslateActivationState(aaShort.ActivationState);
                aaShort.StringRequestActivationState = GetTranslateRequestActivationState(aaShort.RequestActivationState);
                aaShort.StringAlarmState = GetTranslateAlarmState(aaShort.AlarmState);
            }
        }

        void _cdgvData_AfterGridModified(BindingSource bindingSource)
        {
            LocalizationHelper.TranslateDgvHeaderColumn(_cdgvData.DataGrid, AlarmAreaShort.COLUMN_SECTION_ID, "ColumnAlarmAreaId");
        }

        protected override AlarmArea GetObjectForEdit(AlarmAreaShort listObj, out bool editAllowed)
        {
            return Plugin.MainServerProvider.AlarmAreas.GetObjectForEditById(listObj.IdAlarmArea, out editAllowed);
        }

        protected override AlarmArea GetFromShort(AlarmAreaShort listObj)
        {
            return Plugin.MainServerProvider.AlarmAreas.GetObjectById(listObj.IdAlarmArea);
        }

        protected override void AfterTranslateForm()
        {            
        }

        private static volatile NCASAlarmAreasForm _singleton;
        private static object _syncRoot = new object();

        public static NCASAlarmAreasForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASAlarmAreasForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        protected override ICollection<AlarmAreaShort> GetData()
        {
            Exception error;
            var list = Plugin.MainServerProvider.AlarmAreas.ShortSelectByCriteria(FilterSettings, out error);

            if (!string.IsNullOrEmpty(_tbSectionIdFilter.Text))
            {
                list
                    = list.Where((alarmAreaShort) => alarmAreaShort.SectionId.Contains(_tbSectionIdFilter.Text)).ToList();
            }

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

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(
                bindingSource, 
                AlarmAreaShort.COLUMN_SYMBOL, 
                AlarmAreaShort.COLUMN_SECTION_ID, 
                AlarmAreaShort.COLUMN_NAME, 
                AlarmAreaShort.COLUMN_SHORT_NAME,
                AlarmAreaShort.COLUMN_STRING_ALARM_STATE,
                AlarmAreaShort.COLUMN_STRING_ACTIVATION_STATE,
                AlarmAreaShort.COLUMN_STRING_REQUEST_ACTIVATION_STATE,
                AlarmAreaShort.COLUMN_DESCRIPTION);

            if (_cdgvData.DataGrid.Columns.Contains(AlarmAreaShort.COLUMN_SECTION_ID))
                _cdgvData.DataGrid.Columns[AlarmAreaShort.COLUMN_SECTION_ID].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
        }


        private string GetTranslateActivationState(ActivationState activationState)
        {
            switch (activationState)
            {
                case ActivationState.Set: return GetString("AlarmAreaActivationStates_Set");
                case ActivationState.Unset: return GetString("AlarmAreaActivationStates_Unset");
                case ActivationState.Prewarning: return GetString("AlarmAreaActivationStates_Prewarning");
                case ActivationState.TemporaryUnsetExit: return GetString("AlarmAreaActivationStates_TemporaryUnsetExit");
                case ActivationState.TemporaryUnsetEntry: return GetString("AlarmAreaActivationStates_TemporaryUnsetEntry");
                case ActivationState.UnsetBoughtTime: return GetString("AlarmAreaActivationStates_UnsetBoughtTime");
                case ActivationState.Unknown: return GetString("AlarmAreaActivationStates_Unknown");
                default: return GetString("AlarmAreaActivationStates_Unknown");
            }
        }

        private string GetTranslateRequestActivationState(RequestActivationState requestActivationState)
        {
            switch (requestActivationState)
            {
                case RequestActivationState.Set: return GetString("AlarmAreaRequestActivationStates_Set");
                case RequestActivationState.UnconditionalSet: return GetString("AlarmAreaRequestActivationStates_UnconditionalSet");
                case RequestActivationState.Unset: return GetString("AlarmAreaRequestActivationStates_Unset");
                default: return string.Empty;
            }
        }

        private string GetTranslateAlarmState(AlarmAreaAlarmState alarmState)
        {
            switch (alarmState)
            {
                case AlarmAreaAlarmState.Alarm: return GetString("AlarmAreaActualStates_Alarm");
                case AlarmAreaAlarmState.Normal: return GetString("AlarmAreaActualStates_Normal");
                case AlarmAreaAlarmState.Unknown: return GetString("AlarmAreaActualStates_Unknown");
                default: return GetString("AlarmAreaActualStates_Unknown");
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        protected override ACgpPluginEditForm<NCASClient, AlarmArea> CreateEditForm(AlarmArea obj, ShowOptionsEditForm showOption)
        {
            return new NCASAlarmAreaEditForm(obj, showOption, this);
        }

        protected override bool Compare(AlarmArea obj1, AlarmArea obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(AlarmArea obj, Guid idObj)
        {
            return obj.IdAlarmArea == idObj;
        }

        protected override void DeleteObj(AlarmArea obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.AlarmAreas.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();

            if (!string.IsNullOrEmpty(_eNameFilter.Text))
            {
                var filterSetting = new FilterSettings(AlarmArea.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }

            if (!string.IsNullOrEmpty(_eShortNameFilter.Text))
            {
                var filterSetting = new FilterSettings(AlarmArea.COLUMNSHORTNAME, _eShortNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
            _eShortNameFilter.Text = string.Empty;
            _tbSectionIdFilter.Text = string.Empty;
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
        }

        protected override void FilterKeyDown(object sender, KeyEventArgs e)
        {
            base.FilterKeyDown(sender, e);
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.AlarmAreas.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(AlarmArea alarmArea)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.AlarmAreas.HasAccessViewForObject(alarmArea);
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
                    return Plugin.MainServerProvider.AlarmAreas.HasAccessInsert();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public bool HasAccessUpdate()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.AlarmAreas.HasAccessUpdate();
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
                    return Plugin.MainServerProvider.AlarmAreas.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void RegisterEvents()
        {
            _eventActivationStateChanged = ActivationStateChanged;
            ActivationStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventActivationStateChanged);
            _eventRequestActivationStateChanged = RequestActivationStateChanged;
            RequestActivationStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventRequestActivationStateChanged);
            _eventAlarmStateChanged = AlarmStateChanged;
            AlarmStateChangedAlarmAreaHandler.Singleton.RegisterStateChanged(_eventAlarmStateChanged);
        }

        protected override void UnregisterEvents()
        {
            if (_eventActivationStateChanged != null)
            {
                ActivationStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventActivationStateChanged);
                _eventActivationStateChanged = null;
            }

            if (_eventRequestActivationStateChanged != null)
            {
                RequestActivationStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventRequestActivationStateChanged);
                _eventRequestActivationStateChanged = null;
            }

            if (_eventAlarmStateChanged != null)
            {
                AlarmStateChangedAlarmAreaHandler.Singleton.UnregisterStateChanged(_eventAlarmStateChanged);
                _eventAlarmStateChanged = null;
            }
        }

        private void ActivationStateChanged(Guid guid, byte activationState)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<Guid, byte>(ActivationStateChanged), guid, activationState);
                }
                else
                {
                    int i = 0;
                    foreach (AlarmAreaShort aaShort in (_cdgvData.DataGrid.DataSource as BindingSource).List)
                    {
                        if (aaShort != null && aaShort.IdAlarmArea == guid)
                        {
                            aaShort.ActivationState = (ActivationState)activationState;
                            _cdgvData.UpdateValue(i, AlarmAreaShort.COLUMN_STRING_ACTIVATION_STATE, GetTranslateActivationState((ActivationState)activationState));
                            return;
                        }
                        i++;
                    }
                }
            }
            catch { }
        }

        private void RequestActivationStateChanged(Guid guid, byte requestActivationState, bool setUnsetNotConfirm)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<Guid, byte, bool>(RequestActivationStateChanged), guid, requestActivationState, setUnsetNotConfirm);
                }
                else
                {
                    int i = 0;
                    foreach (AlarmAreaShort aaShort in (_cdgvData.DataGrid.DataSource as BindingSource).List)
                    {
                        if (aaShort != null && aaShort.IdAlarmArea == guid)
                        {
                            aaShort.RequestActivationState = (RequestActivationState)requestActivationState;
                            _cdgvData.UpdateValue(i, AlarmAreaShort.COLUMN_STRING_REQUEST_ACTIVATION_STATE,
                                GetTranslateRequestActivationState((RequestActivationState)requestActivationState));
                            return;
                        }
                        i++;
                    }
                }
            }
            catch { }
        }

        private void AlarmStateChanged(Guid guid, byte alarmState)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<Guid, byte>(AlarmStateChanged), guid, alarmState);
                }
                else
                {
                    int i = 0;
                    foreach (AlarmAreaShort aaShort in (_cdgvData.DataGrid.DataSource as BindingSource).List)
                    {
                        if (aaShort != null && aaShort.IdAlarmArea == guid)
                        {
                            aaShort.AlarmState = (AlarmAreaAlarmState)alarmState;
                            _cdgvData.UpdateValue(i, AlarmAreaShort.COLUMN_STRING_ALARM_STATE, GetTranslateAlarmState((AlarmAreaAlarmState)alarmState));
                            return;
                        }
                        i++;
                    }
                }
            }
            catch { }
        }
    }
}
