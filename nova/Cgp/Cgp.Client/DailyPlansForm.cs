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

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Components;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;
using System.Threading;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;


namespace Contal.Cgp.Client
{
    public partial class DailyPlansForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<DailyPlan, DailyPlanShort>
#endif
    {
        public DailyPlansForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private static volatile DailyPlansForm _singleton = null;
        private static object _syncRoot = new object();

        public static DailyPlansForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new DailyPlansForm();
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

            foreach (DailyPlanShort dp in bindingSource)
            {
                dp.Symbol = _cdgvData.GetDefaultImage(dp);
            }
        }

        protected override DailyPlan GetObjectForEdit(DailyPlanShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectForEditById(listObj.IdDailyPlan, out editAllowed);
        }

        protected override DailyPlan GetFromShort(DailyPlanShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(listObj.IdDailyPlan);
        }

        protected override DailyPlan GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(idObj);
        }

        #region CRUD DailyPlan
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
                DailyPlan onDeleteObj = new DailyPlan();
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

        protected override bool DeleteObjectNotAllowed(DailyPlan obj, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (obj == null) return false;

            Exception error;
            DailyPlan deleteObj = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(obj.IdDailyPlan);

            if (deleteObj == null) return false;
            if (deleteObj.DailyPlanName == DailyPlan.IMPLICIT_DP_ALWAYS_ON ||
                    deleteObj.DailyPlanName == DailyPlan.IMPLICIT_DP_ALWAYS_OFF ||
                    deleteObj.DailyPlanName == DailyPlan.IMPLICIT_DP_DAYLIGHT_ON ||
                    deleteObj.DailyPlanName == DailyPlan.IMPLICIT_DP_NIGHT_ON)
            {
                errorMsg = GetString("ErrorDeleteImplicitDailyPlan");
                return true;
            }
            return false;
        }

        protected override void DeleteObj(DailyPlan obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.DailyPlans.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.DailyPlans.DeleteById(idObj, out error))
                throw error;
        }

        protected override ACgpEditForm<DailyPlan> CreateEditForm(DailyPlan obj, ShowOptionsEditForm showOption)
        {
            return new DailyPlanEditForm(obj, showOption);
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
            if (!string.IsNullOrEmpty(_eNameFilter.Text))
            {
                FilterSettings filterSettingName = new FilterSettings(DailyPlan.COLUMNDAILYPLANNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingName);
            }
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }
        #endregion

        #region RefreshDataGrid
        protected override ICollection<DailyPlanShort> GetData()
        {
            Exception error;
            ICollection<DailyPlanShort> list = CgpClient.Singleton.MainServerProvider.DailyPlans.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(DailyPlan obj1, DailyPlan obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, DailyPlanShort.COLUMN_SYMBOL, DailyPlanShort.COLUMNDAILYPLANNAME, DailyPlanShort.COLUMNDESCRIPTION);

            foreach (DataGridViewRow row in _cdgvData.DataGrid.Rows)
            {
                string actValue = (string)row.Cells[DailyPlan.COLUMNDAILYPLANNAME].Value;
                switch (actValue)
                {
                    case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                        row.Cells[DailyPlan.COLUMNDAILYPLANNAME].Value = GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                        break;
                    case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                        row.Cells[DailyPlan.COLUMNDAILYPLANNAME].Value = GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                        break;
                    case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                        row.Cells[DailyPlan.COLUMNDAILYPLANNAME].Value = GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                        break;
                    case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                        row.Cells[DailyPlan.COLUMNDAILYPLANNAME].Value = GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                        break;
                }
            }
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
                    return CgpClient.Singleton.MainServerProvider.DailyPlans.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(DailyPlan dailyPlan)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.DailyPlans.HasAccessViewForObject(dailyPlan);

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
                    return CgpClient.Singleton.MainServerProvider.DailyPlans.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.DailyPlans.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool _isGetFromGraphics;
        private void SetValuesGraphics()
        {
            if (!_isGetFromGraphics)
            {
                Graphics grfx = CreateGraphics();
                _sizeDpiXY = new double[2];
                _sizeDpiXY[0] = (float)grfx.DpiX / 96.0;
                _sizeDpiXY[1] = (float)grfx.DpiY / 96.0;
                grfx.Dispose();
                _isGetFromGraphics = true;
            }
        }

        private double[] _sizeDpiXY;
        public double[] SizeDpiXY
        {
            get
            {
                SetValuesGraphics();
                return _sizeDpiXY;
            }
        }
    }
}
