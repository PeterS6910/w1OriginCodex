using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class PresentationGroupsForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<PresentationGroup, PresentationGroupShort>
#endif
    {
        public PresentationGroupsForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private static volatile PresentationGroupsForm _singleton = null;
        private static object _syncRoot = new object();

        public static PresentationGroupsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new PresentationGroupsForm();
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

            foreach (PresentationGroupShort pg in bindingSource)
            {
                pg.Symbol = _cdgvData.GetDefaultImage(pg);   
            }
        }

        protected override PresentationGroup GetObjectForEdit(PresentationGroupShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectForEditById(listObj.IdGroup, out editAllowed);
        }

        protected override PresentationGroup GetFromShort(PresentationGroupShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectById(listObj.IdGroup);
        }

        protected override PresentationGroup GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.PresentationGroups.GetObjectById(idObj);
        }

        #region CRUP Presentation Groups

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

        protected override void DeleteObj(PresentationGroup obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.PresentationGroups.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.PresentationGroups.DeleteById(idObj, out error))
                throw error;
        }

        protected override ACgpEditForm<PresentationGroup> CreateEditForm(PresentationGroup obj, ShowOptionsEditForm showOption)
        {
            return new PresentationGroupEditForm(obj, showOption);
        }

        #endregion

        #region Filters
        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();

            if (string.IsNullOrEmpty(_eGroupNameFilter.Text) &&
                 string.IsNullOrEmpty(_eEmailFilter.Text))
            {
                return;
            }

            if (!string.IsNullOrEmpty(_eGroupNameFilter.Text))
            {
                FilterSettings filterSetting = new FilterSettings(PresentationGroup.COLUMNGROUPNAME, _eGroupNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSetting);
            }
            if (!string.IsNullOrEmpty(_eEmailFilter.Text))
            {
                FilterSettings filterSetting = new FilterSettings(PresentationGroup.COLUMNEMAIL, _eEmailFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSetting);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eGroupNameFilter.Text = string.Empty;
            _eEmailFilter.Text = string.Empty;
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }
        #endregion

        #region RefreshDataGrid
        protected override ICollection<PresentationGroupShort> GetData()
        {
            Exception error;
            ICollection<PresentationGroupShort> list = CgpClient.Singleton.MainServerProvider.PresentationGroups.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(PresentationGroup obj1, PresentationGroup obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, PresentationGroupShort.COLUMN_SYMBOL, PresentationGroupShort.COLUMNGROUPNAME, PresentationGroupShort.COLUMNEMAIL,
                PresentationGroupShort.COLUMNCISCLASS, PresentationGroupShort.COLUMNDESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        //protected override void ShowData()
        //{
        //    if (CgpClient.Singleton.IsConnectionLost(true)) return;
        //    if (_runShowData) return;
        //    _runShowData = true;
        //    CgpClientMainForm.Singleton.StartProgress();
        //    Contal.IwQuick.Threads.SafeThread.StartThread(new Contal.IwQuick.DVoid2Void(LoadTable));
        //}
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
                    return CgpClient.Singleton.MainServerProvider.PresentationGroups.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(PresentationGroup presentationGroup)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return
                        CgpClient.Singleton.MainServerProvider.PresentationGroups.HasAccessViewForObject(
                            presentationGroup);

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
                    return CgpClient.Singleton.MainServerProvider.PresentationGroups.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.PresentationGroups.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
