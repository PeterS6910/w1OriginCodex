using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.IwQuick;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;

namespace Contal.Cgp.Client
{
    public partial class GlobalAlarmInstructionsForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<GlobalAlarmInstruction, GlobalAlarmInstructionShort>
#endif
    {
        public GlobalAlarmInstructionsForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private static volatile GlobalAlarmInstructionsForm _singleton = null;
        private static object _syncRoot = new object();

        public static GlobalAlarmInstructionsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new GlobalAlarmInstructionsForm();
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
            _cdgvData.CgpDataGridEvents = this;
        }

        protected override GlobalAlarmInstruction GetObjectForEdit(GlobalAlarmInstructionShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectForEditById(listObj.IdGlobalAlarmInstruction, out editAllowed);
        }

        protected override GlobalAlarmInstruction GetFromShort(GlobalAlarmInstructionShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectById(listObj.IdGlobalAlarmInstruction);
        }

        protected override GlobalAlarmInstruction GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectById(idObj);
        }

        #region CRUD GlobalAlarmInstruction
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
                GlobalAlarmInstruction onDeleteObj = new GlobalAlarmInstruction();
                if (base.GetObjectFromActualRow(ref onDeleteObj))
                {
                    string msg;
                    if (DeleteObjectNotAllowed(onDeleteObj, out msg))
                    {
                        Contal.IwQuick.UI.Dialog.Error(msg);
                        return;
                    }
                    Delete_Click();
                }
            }
            else
            {
                MultiselectDeleteClic(_cdgvData.SelectedRows);
            }
        }

        protected override void DeleteObj(GlobalAlarmInstruction obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.DeleteById(idObj, out error))
                throw error;
        }

        protected override ACgpEditForm<GlobalAlarmInstruction> CreateEditForm(GlobalAlarmInstruction obj, ShowOptionsEditForm showOption)
        {
            return new GlobalAlarmInstructionEditForm(obj, showOption);
        }

        #endregion

        #region Filters
        private void RunFilterClick(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();
            if (!string.IsNullOrEmpty(_eNameFilter.Text))
            {
                FilterSettings filterSettingName = new FilterSettings(GlobalAlarmInstruction.COLUMN_NAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingName);
            }
        }

        private void FilterClearClick(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
        }

        #endregion

        #region RefreshDataGrid
        protected override ICollection<GlobalAlarmInstructionShort> GetData()
        {
            Exception error;
            ICollection<GlobalAlarmInstructionShort> list = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(GlobalAlarmInstruction obj1, GlobalAlarmInstruction obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, GlobalAlarmInstruction.COLUMN_NAME, GlobalAlarmInstruction.COLUMN_INSTRUCTIONS);
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
                    return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(GlobalAlarmInstruction globalAlarmInstruction)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return
                        CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.HasAccessViewForObject(
                            globalAlarmInstruction);

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
                    return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
