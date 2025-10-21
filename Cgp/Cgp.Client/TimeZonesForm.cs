using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class TimeZonesForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<Contal.Cgp.Server.Beans.TimeZone, TimeZoneShort>
#endif
    {
        public TimeZonesForm()
        {
            InitializeComponent();
            InitCGPDataGridView();
        }

        private static volatile TimeZonesForm _singleton = null;
        private static object _syncRoot = new object();

        public static TimeZonesForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new TimeZonesForm();
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

            foreach (TimeZoneShort tz in bindingSource)
            {
                tz.Symbol = _cdgvData.GetDefaultImage(tz);
            }
        }

        protected override Contal.Cgp.Server.Beans.TimeZone GetObjectForEdit(TimeZoneShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectForEditById(listObj.IdTimeZone, out editAllowed);
        }

        protected override Contal.Cgp.Server.Beans.TimeZone GetFromShort(TimeZoneShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(listObj.IdTimeZone);
        }

        protected override Contal.Cgp.Server.Beans.TimeZone GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(idObj);
        }

        protected override ICollection<Contal.Cgp.Server.Beans.TimeZoneShort> GetData()
        {
            Exception error;
            ICollection<TimeZoneShort> list = CgpClient.Singleton.MainServerProvider.TimeZones.ShortSelectByCriteria(_filterSettings, out error);

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

        protected override bool Compare(Contal.Cgp.Server.Beans.TimeZone obj1, Contal.Cgp.Server.Beans.TimeZone obj2)
        {
            return obj1.Compare(obj2);
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, TimeZoneShort.COLUMN_SYMBOL, TimeZoneShort.COLUMNNAME, TimeZoneShort.COLUMNDESCRIPTION);
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

        protected override ACgpEditForm<Contal.Cgp.Server.Beans.TimeZone> CreateEditForm(Contal.Cgp.Server.Beans.TimeZone obj, ShowOptionsEditForm showOption)
        {
            return new TimeZonesEditForm(obj, showOption);
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

        protected override void DeleteObj(Contal.Cgp.Server.Beans.TimeZone obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.TimeZones.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.TimeZones.DeleteById(idObj, out error))
                throw error;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();

            if (_eNameFilter.Text != string.Empty)
            {
                FilterSettings filterSetting = new FilterSettings(Contal.Cgp.Server.Beans.TimeZone.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSetting);
            }

            if (_eDescriptionFilter.Text != string.Empty)
            {
                FilterSettings filterSetting = new FilterSettings(Contal.Cgp.Server.Beans.TimeZone.COLUMNDESCRIPTION, _eDescriptionFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSetting);
            }
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
            _eDescriptionFilter.Text = string.Empty;
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
                    return CgpClient.Singleton.MainServerProvider.TimeZones.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(Server.Beans.TimeZone timeZone)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.TimeZones.HasAccessViewForObject(timeZone);

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
                    return CgpClient.Singleton.MainServerProvider.TimeZones.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.TimeZones.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
