using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAlarmTransmittersForm :
#if DESIGNER
    Form
#else
    ACgpPluginTableForm<NCASClient, AlarmTransmitter, AlarmTransmitterShort>
#endif
    {
        private static volatile NCASAlarmTransmittersForm _singleton;
        private static readonly object SyncRoot = new object();

        public static NCASAlarmTransmittersForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (SyncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASAlarmTransmittersForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        public NCASAlarmTransmittersForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            FormImage = ResourceGlobal.Cat48;
            InitCgpDataGridView();
        }

        private void InitCgpDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin) NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        private void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (AlarmTransmitterShort alarmTransmitterShort in bindingSource.List)
            {
                alarmTransmitterShort.Symbol = _cdgvData.GetDefaultImage(alarmTransmitterShort);
            }
        }

        protected override ICollection<AlarmTransmitterShort> GetData()
        {
            Exception error;
            var list =
                Plugin.MainServerProvider.AlarmTransmitters.ShortSelectByCriteria(
                    FilterSettings,
                    out error);

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
                    return Plugin.MainServerProvider.AlarmTransmitters.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(AlarmTransmitter alarmTransmitter)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.AlarmTransmitters.HasAccessViewForObject(alarmTransmitter);
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
                    return Plugin.MainServerProvider.AlarmTransmitters.HasAccessInsert();
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
                    return Plugin.MainServerProvider.AlarmTransmitters.HasAccessDelete();
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
                AlarmTransmitterShort.COLUMN_SYMBOL,
                AlarmTransmitterShort.COLUMN_NAME,
                AlarmTransmitterShort.COLUMN_IP_ADDRESS,
                AlarmTransmitterShort.COLUMN_ONLINE_STATE,
                AlarmTransmitterShort.COLUMN_DESCRIPTION);

            TranslateColumnIpAddress();
        }

        private void TranslateColumnIpAddress()
        {
            if (_cdgvData.DataGrid.Columns.Contains(AlarmTransmitterShort.COLUMN_IP_ADDRESS))
            {
                _cdgvData.DataGrid.Columns[AlarmTransmitterShort.COLUMN_IP_ADDRESS].HeaderText =
                    GetString("ColumnIPAddress");
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ACgpPluginEditForm<NCASClient, AlarmTransmitter> CreateEditForm(
            AlarmTransmitter obj,
            ShowOptionsEditForm showOption)
        {
            return new NCASAlarmTransmitterEditForm(
                obj,
                showOption,
                this);
        }

        protected override AlarmTransmitter GetObjectForEdit(
            AlarmTransmitterShort listObj,
            out bool editEnabled)
        {
            return Plugin.MainServerProvider.AlarmTransmitters.GetObjectForEditById(
                listObj.Id,
                out editEnabled);
        }

        protected override AlarmTransmitter GetFromShort(AlarmTransmitterShort listObj)
        {
            return Plugin.MainServerProvider.AlarmTransmitters.GetObjectById(listObj.Id);
        }

        protected override bool Compare(
            AlarmTransmitter obj1,
            AlarmTransmitter obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(
            AlarmTransmitter obj,
            Guid idAlarmTransmitter)
        {
            return obj.IdAlarmTransmitter == idAlarmTransmitter;
        }

        protected override void DeleteObj(AlarmTransmitter obj)
        {
            Exception error;

            if (!Plugin.MainServerProvider.AlarmTransmitters.Delete(
                obj,
                out error))
            {
                throw error;
            }
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != string.Empty)
            {
                var filterSetting = new FilterSettings(
                    AlarmTransmitterShort.COLUMN_NAME,
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
            AlarmTransmittersLookupFinishedHandler.Singleton.RegisterLookupFinished(AlarmTransmittersLookupFinished);
            AlarmTransmitterOnlineStateChangedHandler.Singleton.RegisterOnlineStateChanged(OnlineStateChanged);
        }

        protected override void UnregisterEvents()
        {
            AlarmTransmittersLookupFinishedHandler.Singleton.UnregisterLookupFinished(AlarmTransmittersLookupFinished);
            AlarmTransmitterOnlineStateChangedHandler.Singleton.UnregisterOnlineStateChanged(OnlineStateChanged);
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

        private void _bTransmittersLookup_Click(object sender, EventArgs e)
        {
            SafeThread.StartThread(LookupAlarmTransmitters);
        }

        private void LookupAlarmTransmitters()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            Plugin.MainServerProvider.AlarmTransmitters.AlarmTransmittersLookup(
                CgpClient.Singleton.ClientID);
        }

        private void AlarmTransmittersLookupFinished(
            ICollection<LookupedAlarmTransmitter> lookupedAlarmTransmitters,
            ICollection<Guid> lookupingClients)
        {
            if (lookupedAlarmTransmitters == null
                || lookupingClients == null
                || !lookupingClients.Contains(CgpClient.Singleton.ClientID))
            {
                return;
            }

            SafeThread<ICollection<LookupedAlarmTransmitter>>.StartThread(
                AlarmTransmittersLookupFinished,
                lookupedAlarmTransmitters);
        }

        private void AlarmTransmittersLookupFinished(
            ICollection<LookupedAlarmTransmitter> lookupedAlarmTransmitters)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<ICollection<LookupedAlarmTransmitter>>(AlarmTransmittersLookupFinished),
                    lookupedAlarmTransmitters);

                return;
            }

            var lookupedTramsmitersForm = new LookupedAlarmTramsmitersForm(Plugin);
            lookupedTramsmitersForm.ShowDialog(lookupedAlarmTransmitters);

            ShowData();
        }

        private void OnlineStateChanged(
            string ipAddress,
            OnlineState onlineState)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<string, OnlineState>(OnlineStateChanged),
                    ipAddress,
                    onlineState);

                return;
            }

            if (_cdgvData == null)
                return;

            var bindingSource = _cdgvData.DataGrid.DataSource as BindingSource;

            if (bindingSource == null)
                return;

            foreach (AlarmTransmitterShort alarmTransmitterShort in bindingSource)
            {
                if (!alarmTransmitterShort.IpAddress.Equals(ipAddress))
                    continue;

                alarmTransmitterShort.OnlineState = onlineState;
                break;
            }

            _cdgvData.Refresh();
        }
    }
}
