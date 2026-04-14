using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASSecurityTimeZonesForm :
#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, SecurityTimeZone, SecurityTimeZoneShort>
#endif
    {

        public NCASSecurityTimeZonesForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.SecurityTimeZone48;
            InitializeComponent();
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (SecurityTimeZoneShort stzShort in bindingSource.List)
            {
                stzShort.Symbol = _cdgvData.GetDefaultImage(stzShort);
            }
        }

        protected override SecurityTimeZone GetObjectForEdit(SecurityTimeZoneShort listObj, out bool editAllowed)
        {
            return Plugin.MainServerProvider.SecurityTimeZones.GetObjectForEditById(listObj.IdSecurityTimeZone, out editAllowed);
        }

        protected override SecurityTimeZone GetFromShort(SecurityTimeZoneShort listObj)
        {
            return Plugin.MainServerProvider.SecurityTimeZones.GetObjectById(listObj.IdSecurityTimeZone);
        }

        private static volatile NCASSecurityTimeZonesForm _singleton;
        private static object _syncRoot = new object();

        public static NCASSecurityTimeZonesForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASSecurityTimeZonesForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        protected override ICollection<SecurityTimeZoneShort> GetData()
        {
            Exception error;
            ICollection<SecurityTimeZoneShort> list = Plugin.MainServerProvider.SecurityTimeZones.ShortSelectByCriteria(FilterSettings, out error);

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

        protected override bool Compare(SecurityTimeZone obj1, SecurityTimeZone obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(SecurityTimeZone obj, Guid idObj)
        {
            return obj.IdSecurityTimeZone == idObj;
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, SecurityTimeZoneShort.COLUMN_SYMBOL, SecurityTimeZoneShort.COLUMN_NAME,
                SecurityTimeZoneShort.COLUMN_DESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion


        protected override ACgpPluginEditForm<NCASClient, SecurityTimeZone> CreateEditForm(SecurityTimeZone obj, ShowOptionsEditForm showOption)
        {
            return new NCASSecurityTimeZoneEditForm(obj, showOption, this);
        }

        protected override void DeleteObj(SecurityTimeZone obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.SecurityTimeZones.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != "")
            {
                FilterSettings filterSetting = new FilterSettings(SecurityTimeZone.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }
            if (_eDescriptionFilter.Text != "")
            {
                FilterSettings filterSetting = new FilterSettings(SecurityTimeZone.COLUMNDESCRIPTION, _eDescriptionFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = "";
            _eDescriptionFilter.Text = "";
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.SecurityTimeZones.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(SecurityTimeZone securityTimeZone)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return
                        Plugin.MainServerProvider.SecurityTimeZones.HasAccessViewForObject(securityTimeZone);
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
                    return Plugin.MainServerProvider.SecurityTimeZones.HasAccessInsert();
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
                    return Plugin.MainServerProvider.SecurityTimeZones.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            Edit_Click();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
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

        protected override void FilterKeyDown(object sender, KeyEventArgs e)
        {
            base.FilterKeyDown(sender, e);
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
            //_bRunFilter_Click(sender, e);
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }
    }
}
