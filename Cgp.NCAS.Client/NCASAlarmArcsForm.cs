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
    public partial class NCASAlarmArcsForm :
#if DESIGNER
    Form
#else
    ACgpPluginTableForm<NCASClient, AlarmArc, AlarmArcShort>
#endif
    {
        private static volatile NCASAlarmArcsForm _singleton;
        private static readonly object SyncRoot = new object();

        public static NCASAlarmArcsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (SyncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASAlarmArcsForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        public NCASAlarmArcsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();
            FormImage = ResourceGlobal.AlarmArc48;
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
            foreach (AlarmArcShort alarmArcShort in bindingSource.List)
            {
                alarmArcShort.Symbol = _cdgvData.GetDefaultImage(alarmArcShort);
            }
        }

        protected override ICollection<AlarmArcShort> GetData()
        {
            Exception error;
            var list =
                Plugin.MainServerProvider.AlarmArcs.ShortSelectByCriteria(
                    FilterSettings,
                    out error);

            if (error != null)
                throw (error);

            CheckAccess();
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
                    return Plugin.MainServerProvider.AlarmArcs.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(AlarmArc alarmArc)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.AlarmArcs.HasAccessViewForObject(alarmArc);
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
                    return Plugin.MainServerProvider.AlarmArcs.HasAccessInsert();
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
                    return Plugin.MainServerProvider.AlarmArcs.HasAccessDelete();
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
                AlarmArcShort.COLUMN_SYMBOL,
                AlarmArcShort.COLUMN_NAME,
                AlarmArcShort.COLUMN_DESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ACgpPluginEditForm<NCASClient, AlarmArc> CreateEditForm(
            AlarmArc obj,
            ShowOptionsEditForm showOption)
        {
            return new NCASAlarmArcEditForm(
                obj,
                showOption,
                this);
        }

        protected override AlarmArc GetObjectForEdit(
            AlarmArcShort listObj,
            out bool editEnabled)
        {
            return Plugin.MainServerProvider.AlarmArcs.GetObjectForEditById(
                listObj.Id,
                out editEnabled);
        }

        protected override AlarmArc GetFromShort(AlarmArcShort listObj)
        {
            return Plugin.MainServerProvider.AlarmArcs.GetObjectById(listObj.Id);
        }

        protected override bool Compare(
            AlarmArc obj1,
            AlarmArc obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(
            AlarmArc obj,
            Guid idAlarmArc)
        {
            return obj.IdAlarmArc == idAlarmArc;
        }

        protected override void DeleteObj(AlarmArc obj)
        {
            Exception error;

            if (!Plugin.MainServerProvider.AlarmArcs.Delete(
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
                    AlarmArcShort.COLUMN_NAME,
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
