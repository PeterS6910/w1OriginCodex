using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Contal.Cgp.BaseLib;

using Contal.IwQuick;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Client
{
    public partial class CisNGsForm :
#if DESIGNER
        Form
#else
        ACgpTableForm<CisNG, CisNGShort>
#endif
    {
        public CisNGsForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private static volatile CisNGsForm _singleton = null;
        private static object _syncRoot = new object();

        public static CisNGsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new CisNGsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvData.ImageList = ObjectImageList.Singleton.GetClientObjectsImages();
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (CisNGShort cis in bindingSource)
            {
                cis.Symbol = _cdgvData.GetDefaultImage(cis);
            }
        }

        protected override CisNG GetObjectForEdit(CisNGShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.CisNGs.GetObjectForEditById(listObj.IdCisNG, out editAllowed);
        }

        protected override CisNG GetFromShort(CisNGShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.CisNGs.GetObjectById(listObj.IdCisNG);
        }

        protected override CisNG GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.CisNGs.GetObjectById(idObj);
        }

        #region CRUD CisNG
        private void InsertClick(object sender, EventArgs e)
        {
            Insert();
        }

        private void EditClick(object sender, EventArgs e)
        {
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

        private void DeleteClick(object sender, EventArgs e)
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

        protected override void DeleteObj(CisNG obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.CisNGs.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.CisNGs.DeleteById(idObj, out error))
                throw error;
        }

        protected override ACgpEditForm<CisNG> CreateEditForm(CisNG obj, ShowOptionsEditForm showOption)
        {
            return new CisNGEditForm(obj, showOption);
        }

        #endregion

        #region Filters
        private void RunFilterClick(object sender, EventArgs e)
        {
            if (!ValidIpAddress()) return;
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();
            if (!string.IsNullOrEmpty(_eNameFilter.Text))
            {
                FilterSettings filterSettingName = new FilterSettings(CisNG.COLUMNCISNGNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingName);
            }
            if (!string.IsNullOrEmpty(_eIpAddressFilter.Text))
            {
                FilterSettings filterSettingIpAddress = new FilterSettings(CisNG.COLUMNIPADDRESS, _eIpAddressFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingIpAddress);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
            _eIpAddressFilter.Text = string.Empty;
        }

        private void FilterClearClick(object sender, EventArgs e)
        {
            FilterClear_Click();
        }
        #endregion

        #region RefreshDataGrid
        protected override ICollection<CisNGShort> GetData()
        {
            Exception error;
            ICollection<CisNGShort> list = CgpClient.Singleton.MainServerProvider.CisNGs.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(CisNG obj1, CisNG obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, CisNGShort.COLUMN_SYMBOL, CisNGShort.COLUMNCISNGNAME, CisNGShort.COLUMNIPADDRESS, CisNGShort.COLUMNDESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }
        #endregion

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
                    return CgpClient.Singleton.MainServerProvider.CisNGs.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(CisNG CisNg)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CisNGs.HasAccessViewForObject(CisNg);

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CisNGs.HasAccessInsert();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessDelete()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CisNGs.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidIpAddress()
        {
            if (_eIpAddressFilter.Text == string.Empty)
                return true;

            if (!Contal.IwQuick.Net.IPHelper.IsValid(_eIpAddressFilter.Text))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eIpAddressFilter,
                        GetString("ErrorInsertValidIPAddress"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _eIpAddressFilter.Focus();
                return false;
            }
            return true;
        }

        private void _eIpAddressFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (_eIpAddressFilter.Text == string.Empty || Contal.IwQuick.Net.IPHelper.IsValid(_eIpAddressFilter.Text))
                    RunFilter();
            }
        }

    }
}
