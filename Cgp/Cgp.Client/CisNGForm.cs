using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.BaseLib;
using System.Reflection;

using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;
using System.Threading;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Client
{
    public partial class CisNGForm : ACgpFullscreenForm
    {
        private BindingSource _bindingSource = null;
        private string _filerEnterValue;
        private IList<FilterSettings> _filterSettings = new List<FilterSettings>();
        private bool _runShowData = false;
        private bool _refreshData = false;
        private bool _freeInsert = true;
        private List<string> _listActivatedObjects = new List<string>();

        public CisNGForm()
        {
            InitializeComponent();
            RegisterToMain();
        }

        private static CisNGForm _singleton;
        public static CisNGForm Singleton
        {
            get
            {
                if (null == _singleton)
                {
                    _singleton = new CisNGForm();
                    _singleton.MdiParent = CgpClientMainForm.Singleton;
                }
                return _singleton;
            }
        }

        #region CRUD CisNG
        private void InsertClick(object sender, EventArgs e)
        {
            if (!_freeInsert)
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertNotFree"));
                return;
            }
            _freeInsert = false;
            InsertCisNG();
            _filterSettings.Clear();
            ShowData();
        }

        private void InsertCisNG()
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;
                CisNG newCisNG = new CisNG();
                CisNGEditForm editCisNGForm = new CisNGEditForm(newCisNG, true);
                editCisNGForm.Show();
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInserCisNG"));
            }
        }

        private void EditClick(object sender, EventArgs e)
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            UpdateCisNG();
            _dgValues.Refresh();
        }

        private void UpdateCisNG()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            CisNG newCisNG = (CisNG)_bindingSource.List[_bindingSource.Position];
            if (IsInActivatedList(newCisNG))
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorAlreadyEdited"));
                return;
            }
            AddToEditList(newCisNG);

            try
            {
                newCisNG.ToString();
            }
            catch
            {
                RefreshData();
                newCisNG = (CisNG)_bindingSource.List[_bindingSource.Position];
            }


            var ws = this.WindowState;

            CisNGEditForm editCisNGForm = new CisNGEditForm(newCisNG, false);
            CgpClientMainForm.Singleton.AddToRecentList(newCisNG, editCisNGForm);
            editCisNGForm.Show();
            editCisNGForm.WindowState = ws;
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            try
            {
                if (_bindingSource == null || _bindingSource.Count == 0) return;
                if (CgpClient.Singleton.IsConnectionLost(true)) return;
                CisNG cis = (CisNG)_bindingSource.List[_bindingSource.Position];
                if (IsInActivatedList(cis))
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteEditing"));
                    return;
                }
                if (Contal.IwQuick.UI.Dialog.Question(GetString("DeleteCisNGConfirm")))
                {
                    DeleteCisNG();
                    _filterSettings.Clear();
                    ShowData();
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteFailed"));
            }
        }

        private void DeleteCisNG()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            CisNG cis = (CisNG)_bindingSource.List[_bindingSource.Position];

            Exception ex;
            if (CgpClient.Singleton.MainServerProvider.CisNGs.Delete(cis, out ex))
            {
                CgpClientMainForm.Singleton.DeleteFromRecentList(cis);
            }
            else
            {
                if (ex is Contal.IwQuick.SqlDeleteReferenceConstraintException)
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteRowInRelationship"));
                }
                else
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteFailed"));
                }
            }
        }
        #endregion

        #region Filters
        private void RunFilterClick(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void RunFilter()
        {
            if (!string.IsNullOrEmpty(_eNameFilter.Text))
            {
                FilterSettings filterSettingName = new FilterSettings(CisNG.CISNGNAME, _eNameFilter.Text, ComparerModes.LIKE);
                _filterSettings.Add(filterSettingName);
            }
            if (!string.IsNullOrEmpty(_eIpAddressFilter.Text))
            {
                FilterSettings filterSettingIpAddress = new FilterSettings(CisNG.IPADDRESS, _eIpAddressFilter.Text, ComparerModes.EQUALL);
                _filterSettings.Add(filterSettingIpAddress);
            }
            ShowData();
        }

        private void FilterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                //if ( ((TextBox)sender).Name == "_eDateFilter" &&  !CheckDate()) return;
                if (_filerEnterValue != ((TextBox)sender).Text)
                {
                    _filerEnterValue = ((TextBox)sender).Text;
                    RunFilter();
                }
            }
        }

        private void BeforeFilterEnter(object sender, EventArgs e)
        {
            _filerEnterValue = ((TextBox)sender).Text;
        }

        private void FilterClearClick(object sender, EventArgs e)
        {
            _eNameFilter.Text = string.Empty;
            _eIpAddressFilter.Text = string.Empty;
            _filterSettings.Clear();
            ShowData();
        }
        #endregion

        #region Connection

        private void _dgValues_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            CgpClient.Singleton.ReportOrmProviderProblem(e.Exception);
            e.Cancel = true;
            _refreshData = true;
            _dgValues.Visible = false;
        }

        protected override bool VerifySources()
        {
            return null != CgpClient.Singleton.MainServerProvider;
        }

        void ConnectionLost(Type type)
        {
            CgpClientMainForm.Singleton.StopProgress();
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DFromVoidToVoid(RemoveDataSource));
            }
            else
            {
                RemoveDataSource();
            }
        }

        protected override void RemoveDataSource()
        {
            _dgValues.Visible = false;
            _dgValues.DataSource = null;
            _bindingSource = null;
            _dgValues.Visible = true;
            //Contal.IwQuick.UI.Dialog.Error(GetString("ErrorConnection"));
        }

        void ConnectionObtain(ICgpServerRemotingProvider parameter)
        {
            _filterSettings.Clear();
            ShowData();
        }
        #endregion

        #region RefreshDataGrid
        protected override void ShowData()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (_runShowData) return; //if already run ShowData then return, don't do it twice
            _runShowData = true;
            CgpClientMainForm.Singleton.StartProgress();
            Contal.IwQuick.Threads.SafeThread.StartThread(LoadTable);
        }

        private void LoadTable()
        {
            UpdateGridView(CgpClient.Singleton.MainServerProvider.CisNGs.SelectByCriteria(_filterSettings));
        }

        private void UpdateGridView(ICollection<Cgp.Server.Beans.CisNG> listFromDB)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DFromTToVoid<ICollection<Cgp.Server.Beans.CisNG>>(UpdateGridView), listFromDB);
            }
            else
            {
                int lastPosition = 0;
                if (_bindingSource != null && _bindingSource.Count != 0)
                    lastPosition = _bindingSource.Position;
                _bindingSource = new BindingSource();
                _bindingSource.DataSource = listFromDB;
                if (lastPosition != 0) _bindingSource.Position = lastPosition;
                _dgValues.DataSource = _bindingSource;
                HideColumnDgw(_dgValues, Server.Beans.CisNG.IDCISNG);
                HideColumnDgw(_dgValues, Server.Beans.CisNG.PASSWORD);
                HideColumnDgw(_dgValues, Server.Beans.CisNG.USERNAME);
                HideColumnDgw(_dgValues, Server.Beans.CisNG.PORT);
                HideColumnDgw(_dgValues, Server.Beans.CisNG.CISNGGROUP);
                HideColumnDgw(_dgValues, Server.Beans.CisNG.PRESENTATIONGROUP);
                CgpClient.Singleton.LocalizationHelper.TranslateDataGridView(_dgValues);
                _dgValues.Visible = true;
                _runShowData = false;
                CgpClientMainForm.Singleton.StopProgress();
            }
        }
        #endregion

        private void _dgValues_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            UpdateCisNG();
            _dgValues.Refresh();
        }

        private void RefreshData()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            int position = _bindingSource.Position;
            try
            {
                UpdateGridView(CgpClient.Singleton.MainServerProvider.CisNGs.SelectByCriteria(_filterSettings));
            }
            catch
            {
            }
            _bindingSource.Position = position;
        }

        public void RefreshData(bool insert, CisNG cisNg)
        {
            if (insert)
            {
                _freeInsert = true;
                _filterSettings.Clear();
                ShowData();
            }
            else
            {
                RemoveFromEditList(cisNg);
            }
            _dgValues.Refresh();
        }

        public void AddToEditList(CisNG cisNg)
        {
            if (cisNg == null) return;
            _listActivatedObjects.Add(cisNg.IdCisNG.ToString());
        }

        private void RemoveFromEditList(CisNG cisNg)
        {
            if (cisNg == null) return;
            _listActivatedObjects.Remove(cisNg.IdCisNG.ToString());
        }

        private bool IsInActivatedList(CisNG cisNg)
        {
            if (cisNg == null) return false;
            return _listActivatedObjects.Contains(cisNg.IdCisNG.ToString());
        }

        private void _dgValues_Paint(object sender, PaintEventArgs e)
        {
            if (_refreshData)
            {
                RefreshData();
                _refreshData = false;
                _dgValues.Visible = true;
            }
        }
    }


}
