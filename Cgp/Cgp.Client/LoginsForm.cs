using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class LoginsForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<Login, LoginShort>
#endif
    {
        public LoginsForm()
        {
            InitializeComponent();
            FillFilteringComboBoxes();
            LocalizationHelper.LanguageChanged += new DVoid2Void(LocalizationHelper_LanguageChanged);
            InitCGPDataGridView();
        }

        private static volatile LoginsForm _singleton = null;
        private static object _syncRoot = new object();

        public static LoginsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new LoginsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvData.ImageList = ObjectImageList.Singleton.ClientObjectImages;
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (LoginShort l in bindingSource)
            {
                l.Symbol = _cdgvData.GetDefaultImage(l);
            }
        }

        protected override Login GetObjectForEdit(LoginShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.Logins.GetObjectForEditById(listObj.Id, out editAllowed);
        }

        protected override Login GetFromShort(LoginShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(listObj.Id);
        }

        protected override Login GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(idObj);
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

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
        }

        void LocalizationHelper_LanguageChanged()
        {
            FillFilteringComboBoxes();
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

        protected override void ClearFilterEdits()
        {
            _eUserNameFilter.Text = string.Empty;
            _eLoginGroupFilter.Text = string.Empty;
            _eDisabledFilter.Text = string.Empty;
            _eExpirationFilter.Text = string.Empty;
        }

        #region ShowData

        protected override ICollection<LoginShort> GetData()
        {
            Exception error;
            ICollection<LoginShort> list = CgpClient.Singleton.MainServerProvider.Logins.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(
                bindingSource,
                LoginShort.COLUMN_SYMBOL,
                LoginShort.COLUMN_USERNAME,
                LoginShort.COLUMN_LOGIN_GROUP,
                LoginShort.COLUMN_IS_DISABLED,
                LoginShort.COLUMN_EXPIRATION_DATE,
                LoginShort.COLUMN_MUST_CHANGE_PASSWORD,
                LoginShort.COLUMN_CLIENT_LANGUAGE,
                LoginShort.COLUMN_DESCRIPTION);
        }

        private void EnsureLastColumn(string columnName)
        {
            for (int i = 0; i < _cdgvData.DataGrid.Columns.Count; i++)
            {
                //if another visible column has lower display index
                if (_cdgvData.DataGrid.Columns[i].Visible && _cdgvData.DataGrid.Columns[columnName].DisplayIndex < _cdgvData.DataGrid.Columns[i].DisplayIndex)
                {
                    //switch display indexes
                    _cdgvData.DataGrid.Columns[i].DisplayIndex = _cdgvData.DataGrid.Columns[columnName].DisplayIndex;
                }
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        protected override void SetFilterSettings()
        {
            if (!string.IsNullOrEmpty(_eUserNameFilter.Text))
            {
                var filterSettingUserName = new FilterSettings("Username", _eUserNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingUserName);
            }
            if (!string.IsNullOrEmpty(_eLoginGroupFilter.Text))
            {
                var filterSettingUserName = new FilterSettings(LoginShort.COLUMN_LOGIN_GROUP, _eLoginGroupFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingUserName);
            }
            if (_eDisabledFilter.Text != string.Empty)
            {
                var filterSettingIsDisabled = new FilterSettings("IsDisabled",
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

        protected override ACgpEditForm<Login> CreateEditForm(Login obj, ShowOptionsEditForm showOption)
        {
            return new LoginEditForm(obj, showOption);
        }

        protected override void DeleteObj(Login obj)
        {
            obj = CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(obj.IdLogin);

            CheckRightsForDelete(obj);

            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.Logins.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Login obj = CgpClient.Singleton.MainServerProvider.Logins.GetObjectById(idObj);

            CheckRightsForDelete(obj);

            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.Logins.Delete(obj, out error))
                throw error;
        }

        private void CheckRightsForDelete(Login login)
        {
            if (login.Username == CgpServerGlobals.DEFAULT_ADMIN_LOGIN)
                throw new OperationDeniedException();

            if (CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(login.Username) &&
                !CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs))
            {
                throw new AccessDeniedException();
            }

            if (login.Username == CgpClient.Singleton.LoggedAs)
                throw new InvalidDeleteOperationException();
        }

        protected override bool Compare(Login obj1, Login obj2)
        {
            return obj1.Compare(obj2);
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
                    return CgpClient.Singleton.MainServerProvider.Logins.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(Login login)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Logins.HasAccessViewForObject(login);

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
                    return CgpClient.Singleton.MainServerProvider.Logins.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.Logins.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
