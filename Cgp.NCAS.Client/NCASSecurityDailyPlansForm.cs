using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.Sys;


namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASSecurityDailyPlansForm :
#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, SecurityDailyPlan, SecurityDailyPlanShort>       
#endif
    {
        public NCASSecurityDailyPlansForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.SecurityDailyPLan48;
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
            foreach (SecurityDailyPlanShort sdpShort in bindingSource.List)
            {
                sdpShort.Symbol = _cdgvData.GetDefaultImage(sdpShort);              
            }
        }

        protected override SecurityDailyPlan GetObjectForEdit(SecurityDailyPlanShort listObj, out bool editEnabled)
        {
            return Plugin.MainServerProvider.SecurityDailyPlans.GetObjectForEditById(listObj.IdSecurityDailyPlan, out editEnabled);
        }

        protected override SecurityDailyPlan GetFromShort(SecurityDailyPlanShort listObj)
        {
            return Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(listObj.IdSecurityDailyPlan);
        }

        private static volatile NCASSecurityDailyPlansForm _singleton;
        private static object _syncRoot = new object();

        public static NCASSecurityDailyPlansForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new NCASSecurityDailyPlansForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }
                return _singleton;
            }
        }

        protected override ICollection<SecurityDailyPlanShort> GetData()
        {
            Exception error;
            ICollection<SecurityDailyPlanShort> list = Plugin.MainServerProvider.SecurityDailyPlans.ShortSelectByCriteria(FilterSettings, out error);

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

        protected override bool Compare(SecurityDailyPlan obj1, SecurityDailyPlan obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(SecurityDailyPlan obj, Guid idObj)
        {
            return obj.IdSecurityDailyPlan == idObj;
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, SecurityDailyPlanShort.COLUMN_SYMBOL, SecurityDailyPlanShort.COLUMNNAME, 
                SecurityDailyPlanShort.COLUMNDESCRIPTION);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        protected override ACgpPluginEditForm<NCASClient, SecurityDailyPlan> CreateEditForm(SecurityDailyPlan obj, ShowOptionsEditForm showOption)
        {
            return new NCASSecurityDailyPlanEditForm(obj, showOption, this);
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

        protected override void DeleteObj(SecurityDailyPlan obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.SecurityDailyPlans.Delete(obj, out error))
                throw error;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_eNameFilter.Text != "")
            {
                FilterSettings filterSetting = new FilterSettings(SecurityDailyPlan.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                FilterSettings.Add(filterSetting);
            }
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = "";
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
                    return Plugin.MainServerProvider.SecurityDailyPlans.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(SecurityDailyPlan securityDailyPlan)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return
                        Plugin.MainServerProvider.SecurityDailyPlans.HasAccessViewForObject(
                            securityDailyPlan);
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
                    return Plugin.MainServerProvider.SecurityDailyPlans.HasAccessInsert();
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
                    return Plugin.MainServerProvider.SecurityDailyPlans.HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        #region ForGraphics
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
        #endregion

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }
    }
}
