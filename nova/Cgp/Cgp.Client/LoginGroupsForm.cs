using System;
using System.Collections.Generic;

using System.Windows.Forms;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class LoginGroupsForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<LoginGroup, LoginGroupShort>
#endif
    {
        private static volatile LoginGroupsForm _singleton;
        private readonly static object _syncRoot = new object();

        public static LoginGroupsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new LoginGroupsForm {MdiParent = CgpClientMainForm.Singleton};
                        }
                    }

                return _singleton;
            }
        }

        public LoginGroupsForm()
        {
            InitializeComponent();
            LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
            InitCGPDataGridView();
        }

        void LocalizationHelper_LanguageChanged()
        {
            FillFilteringComboBoxes();
        }

        private void FillFilteringComboBoxes()
        {
            _eDisabledFilter.Items.Clear();
            _eDisabledFilter.Items.Add(string.Empty);
            _eDisabledFilter.Items.Add(GetString("General_true"));
            _eDisabledFilter.Items.Add(GetString("General_false"));

            _eExpirationFilter.Items.Clear();
            _eExpirationFilter.Items.Add(string.Empty);
            _eExpirationFilter.Items.Add(GetString("General_true"));
            _eExpirationFilter.Items.Add(GetString("General_false"));
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvData.ImageList = ObjectImageList.Singleton.ClientObjectImages;
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        protected override ICollection<LoginGroupShort> GetData()
        {
            Exception error;
            ICollection<LoginGroupShort> list = CgpClient.Singleton.MainServerProvider.LoginGroups.ShortSelectByCriteria(_filterSettings, out error);

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
                    return CgpClient.Singleton.MainServerProvider.LoginGroups.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(LoginGroup loginGroup)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.LoginGroups.HasAccessViewForObject(loginGroup);

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
                    return CgpClient.Singleton.MainServerProvider.LoginGroups.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.LoginGroups.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, LoginGroupShort.COLUMN_SYMBOL, LoginGroupShort.COLUMN_LOGIN_GROUP_NAME,
                LoginGroupShort.COLUMN_IS_DISABLED, LoginGroupShort.COLUMN_EXPIRATION_DATE,
                LoginGroupShort.COLUMN_DESCRIPTION);            
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ACgpEditForm<LoginGroup> CreateEditForm(LoginGroup obj, ShowOptionsEditForm showOption)
        {
            return new LoginGroupEditForm(obj, showOption);
        }

        protected override LoginGroup GetObjectForEdit(LoginGroupShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectForEditById(listObj.IdLoginGroup, out editAllowed);
        }

        protected override LoginGroup GetFromShort(LoginGroupShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(listObj.IdLoginGroup);
        }

        protected override LoginGroup GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(idObj);
        }

        protected override bool Compare(LoginGroup obj1, LoginGroup obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void DeleteObj(LoginGroup obj)
        {
            obj = CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(obj.IdLoginGroup);

            if (CgpClient.Singleton.MainServerProvider.LoginGroups.IsLoginGroupSuperAdmin(obj.LoginGroupName))
                throw new OperationDeniedException();

            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.LoginGroups.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            LoginGroup obj = CgpClient.Singleton.MainServerProvider.LoginGroups.GetObjectById(idObj);

            if (CgpClient.Singleton.MainServerProvider.LoginGroups.IsLoginGroupSuperAdmin(obj.LoginGroupName))
                throw new OperationDeniedException();

            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.LoginGroups.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            if (!string.IsNullOrEmpty(_eLoginGroupNameFilter.Text))
            {
                var filterSettingUserName = new FilterSettings(LoginGroup.COLUMN_LOGIN_GROUP_NAME, _eLoginGroupNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingUserName);
            }
            if (_eDisabledFilter.Text != string.Empty)
            {
                var filterSettingIsDisabled = new FilterSettings(LoginGroup.COLUMN_IS_DISABLED,
                    GetBoolFromComboBox(_eDisabledFilter, _eDisabledFilter.Text), ComparerModes.EQUALL);
                _filterSettings.Add(filterSettingIsDisabled);
            }
            if (_eExpirationFilter.Text != string.Empty)
            {
                var filterSettingExpirationSet = new FilterSettings("ExpirationSet",
                    GetBoolFromComboBox(_eExpirationFilter, _eExpirationFilter.Text), ComparerModes.EQUALL);
                _filterSettings.Add(filterSettingExpirationSet);
            }
        }

        private bool GetBoolFromComboBox(ComboBox comboBox, string text)
        {
            if (comboBox.Items[1].ToString().Equals(text))
            {
                return true;
            }
            return false;
        }

        protected override void ClearFilterEdits()
        {
            _eLoginGroupNameFilter.Text = "";
            _eDisabledFilter.Text = string.Empty;
            _eExpirationFilter.Text = string.Empty;
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
        }

        protected override void FilterKeyDown(object sender, KeyEventArgs e)
        {
            base.FilterKeyDown(sender, e);
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (LoginGroupShort l in bindingSource)
            {
                l.Symbol = _cdgvData.GetDefaultImage(l);
            }
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        private void _bEdit_Click(object sender, EventArgs e)
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
    }
}
