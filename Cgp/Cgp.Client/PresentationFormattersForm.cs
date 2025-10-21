using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class PresentationFormattersForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<PresentationFormatter, PresentationFormatter>
#endif
    {
        public PresentationFormattersForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private static volatile PresentationFormattersForm _singleton = null;
        private static object _syncRoot = new object();

        public static PresentationFormattersForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new PresentationFormattersForm();
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
            _cdgvData.CgpDataGridEvents = this;
        }

        protected override PresentationFormatter GetObjectForEdit(PresentationFormatter listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.PresentationFormatters.GetObjectForEditById(listObj.IdFormatter, out editAllowed);
        }

        protected override PresentationFormatter GetFromShort(PresentationFormatter listObj)
        {
            return listObj;
        }

        protected override PresentationFormatter GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.PresentationFormatters.GetObjectById(idObj);
        }


        #region CRUP Presentation Formatter

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

        protected override void DeleteObj(PresentationFormatter obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.PresentationFormatters.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.PresentationFormatters.DeleteById(idObj, out error))
                throw error;
        }

        protected override ACgpEditForm<PresentationFormatter> CreateEditForm(PresentationFormatter obj, ShowOptionsEditForm showOption)
        {
            return new PresentationFormatterEditForm(obj, showOption);
        }
        #endregion

        #region Filters
        private void RunFilterClick(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void FilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eFormatterMessageFilter.Text = string.Empty;
            _eFormatterNameFilter.Text = string.Empty;
            _filterSettings.Clear();
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();

            if (string.IsNullOrEmpty(_eFormatterMessageFilter.Text) &&
                string.IsNullOrEmpty(_eFormatterNameFilter.Text))
            {
                return;
            }

            if (!string.IsNullOrEmpty(_eFormatterNameFilter.Text))
            {
                FilterSettings filterSetting = new FilterSettings(PresentationFormatter.COLUMNFORMATTERNAME, _eFormatterNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSetting);
            }

            if (!string.IsNullOrEmpty(_eFormatterMessageFilter.Text))
            {
                FilterSettings filterSetting = new FilterSettings(PresentationFormatter.COLUMNMESSAGEFORMAT, _eFormatterMessageFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSetting);
            }
        }
        #endregion


        #region RefreshDataGrid
        protected override ICollection<PresentationFormatter> GetData()
        {
            Exception error;
            ICollection<PresentationFormatter> list = CgpClient.Singleton.MainServerProvider.PresentationFormatters.SelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(PresentationFormatter obj1, PresentationFormatter obj2)
        {
            return obj1.Compare(obj2);
        }
        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, PresentationFormatter.COLUMNFORMATTERNAME, PresentationFormatter.COLUMNMESSAGEFORMAT, PresentationFormatter.COLUMNDESCRIPTION);
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
                    return CgpClient.Singleton.MainServerProvider.PresentationFormatters.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(PresentationFormatter presentationFormatter)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return
                        CgpClient.Singleton.MainServerProvider.PresentationFormatters.HasAccessViewForObject(
                            presentationFormatter);

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
                    return CgpClient.Singleton.MainServerProvider.PresentationFormatters.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.PresentationFormatters.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
