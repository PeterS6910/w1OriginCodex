using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.UI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Contal.Cgp.Client
{
    public partial class CarsForm :
#if DESIGNER
        Form
#else
        ACgpTableForm<Car, CarShort>
#endif
    {
        private static volatile CarsForm _singleton;
        private static object _syncRoot = new object();

        public static CarsForm Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new CarsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }
                return _singleton;
            }
        }

        public CarsForm()
        {
            InitializeComponent();
            FormOnEnter += FormEnter;
            InitCGPDataGridView();
        }

        private void FormEnter(Form form)
        {
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            var selected = _cbSecurityLevelFilter.SelectedItem as CarSecurityLevel?;
            _cbSecurityLevelFilter.Items.Clear();
            foreach (CarSecurityLevel level in Enum.GetValues(typeof(CarSecurityLevel)))
            {
                _cbSecurityLevelFilter.Items.Add(level);
                if (selected != null && level.Equals(selected.Value))
                    _cbSecurityLevelFilter.SelectedItem = level;
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

            foreach (CarShort carShort in bindingSource)
            {
                carShort.Symbol = _cdgvData.GetDefaultImage(carShort);
            }
        }

        protected override ICollection<CarShort> GetData()
        {
            Exception error;
            var list = CgpClient.Singleton.MainServerProvider.Cars.ShortSelectByCriteria(_filterSettings, out error);
            if (error != null)
                throw error;
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

        protected override Car GetObjectForEdit(CarShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.Cars.GetObjectForEditById(listObj.IdCar, out editAllowed);
        }

        protected override Car GetFromShort(CarShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.Cars.GetObjectById(listObj.IdCar);
        }

        protected override Car GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.Cars.GetObjectById(idObj);
        }

        protected override ACgpEditForm<Car> CreateEditForm(Car obj, ShowOptionsEditForm showOption)
        {
            return new CarEditForm(obj, showOption);
        }

        protected override bool Compare(Car obj1, Car obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(
                bindingSource,
                CarShort.COLUMN_SYMBOL,
                CarShort.COLUMNLP,
                CarShort.COLUMNBRAND,
                CarShort.COLUMNVALIDITYDATEFROM,
                CarShort.COLUMNVALIDITYDATETO,
                CarShort.COLUMNSECURITYLEVEL,                
                CarShort.COLUMNDESCRIPTION);
            // nastavenie šírok
            SetWidthColumn(_cdgvData.DataGrid, CarShort.COLUMNLP, 100);
            SetWidthColumn(_cdgvData.DataGrid, CarShort.COLUMNBRAND, 120);
            SetWidthColumn(_cdgvData.DataGrid, CarShort.COLUMNVALIDITYDATEFROM, 160);
            SetWidthColumn(_cdgvData.DataGrid, CarShort.COLUMNVALIDITYDATETO, 160);
            // Description nech je širší a vyplní zvyšok okna
            _cdgvData.DataGrid.Columns[CarShort.COLUMNDESCRIPTION].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override void DeleteObj(Car obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.Cars.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.Cars.DeleteById(idObj, out error))
                throw error;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();

            if (!string.IsNullOrEmpty(_eLpFilter.Text))
            {
                _filterSettings.Add(new FilterSettings(Car.COLUMNLP, _eLpFilter.Text, ComparerModes.LIKEBOTH));
            }

            if (!string.IsNullOrEmpty(_eBrandFilter.Text))
            {
                _filterSettings.Add(new FilterSettings(Car.COLUMNBRAND, _eBrandFilter.Text, ComparerModes.LIKEBOTH));
            }

            if (_cbSecurityLevelFilter.SelectedItem is CarSecurityLevel level)
            {
                _filterSettings.Add(new FilterSettings(Car.COLUMNSECURITYLEVEL, level, ComparerModes.EQUALL));
            }
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {

            _cbSecurityLevelFilter.SelectedItem = null;
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
            if (sender == _cbSecurityLevelFilter)
                _bRunFilter_Click(sender, e);
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Cars.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(Car car)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Cars.HasAccessViewForObject(car);

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
                    return CgpClient.Singleton.MainServerProvider.Cars.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.Cars.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
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
    }
}
