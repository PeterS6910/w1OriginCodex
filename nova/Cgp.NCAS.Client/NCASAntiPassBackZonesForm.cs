//#define DESIGNER
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAntiPassBackZonesForm :
#if DESIGNER
        Form
#else
 ACgpPluginTableForm<NCASClient, AntiPassBackZone, AntiPassBackZoneShort>
#endif
    {
        public NCASAntiPassBackZonesForm()
            : base(
                CgpClientMainForm.Singleton, 
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            FormImage = ResourceGlobal.AntiPassBackZone48;

            _cdgvData.LocalizationHelper = LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.BeforeGridModified += _cdgv_BeforeGridModified;
            _cdgvData.CgpDataGridEvents = this;
        }

        private void _cdgv_BeforeGridModified(BindingSource bindingsource)
        {
            foreach (AntiPassBackZoneShort antiPassBackZoneShort in bindingsource.List)
                antiPassBackZoneShort.Symbol =
                    _cdgvData.GetDefaultImage(antiPassBackZoneShort);
        }

        protected override ICollection<AntiPassBackZoneShort> GetData()
        {
            Exception error;

            var list = Plugin.MainServerProvider.AntiPassBackZones.ShortSelectByCriteria(
                out error,
                LogicalOperators.AND);

            if (error != null)
                throw (error);

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

        private static volatile NCASAntiPassBackZonesForm _singleton;
        private static readonly object _syncRoot = new object();

        public static NCASAntiPassBackZonesForm Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                        if (_singleton == null)
                            _singleton =
                                new NCASAntiPassBackZonesForm
                                {
                                    MdiParent = CgpClientMainForm.Singleton
                                };

                return _singleton;
            }
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.AntiPassBackZones.HasAccessView();
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
                    return
                        Plugin.MainServerProvider.AntiPassBackZones
                            .HasAccessInsert();
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
                    return
                        Plugin.MainServerProvider.AntiPassBackZones
                            .HasAccessDelete();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public override bool HasAccessView(AntiPassBackZone ormObject)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return
                        Plugin.MainServerProvider.AntiPassBackZones
                            .HasAccessViewForObject(ormObject);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            var dgcell = _cdgvData.DataGrid.CurrentCell;
            _cdgvData.DataGrid.CurrentCell = null;

            _cdgvData.ModifyGridView(
                bindingSource,
                AntiPassBackZoneShort.COLUMN_SYMBOL,
                AntiPassBackZoneShort.COLUMN_NAME,
                AntiPassBackZoneShort.COLUMN_DESCRIPTION);

            try
            {
                if (dgcell != null && dgcell.Visible)
                    _cdgvData.DataGrid.CurrentCell = dgcell;
            }
            catch
            {
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ACgpPluginEditForm<NCASClient, AntiPassBackZone> CreateEditForm(
            AntiPassBackZone obj,
            ShowOptionsEditForm showOption)
        {
            return new NCASAntiPassBackZoneEditForm(obj, showOption, this);
        }

        protected override AntiPassBackZone GetObjectForEdit(
            AntiPassBackZoneShort shortObject,
            out bool editEnabled)
        {
            return
                Plugin.MainServerProvider.AntiPassBackZones
                    .GetObjectForEditById(
                        shortObject.Id, 
                        out editEnabled);
        }

        protected override AntiPassBackZone GetFromShort(
            AntiPassBackZoneShort shortObject)
        {
            return 
                Plugin.MainServerProvider.AntiPassBackZones
                    .GetObjectById(shortObject.Id);
        }

        protected override bool Compare(
            AntiPassBackZone obj1,
            AntiPassBackZone obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(
            AntiPassBackZone obj,
            Guid idObj)
        {
            return obj.IdAntiPassBackZone.Equals(idObj);
        }

        protected override void DeleteObj(AntiPassBackZone obj)
        {
            Exception error;

            if (!Plugin.MainServerProvider.AntiPassBackZones.Delete(obj, out error))
                throw error;
        }

        protected override void SetFilterSettings()
        {
            FilterSettings.Clear();
            if (_tbName.Text != string.Empty)
            {
                var filterSetting = 
                    new FilterSettings(
                        AntiPassBackZone.COLUMNNAME, 
                        _tbName.Text, 
                        ComparerModes.LIKEBOTH);

                FilterSettings.Add(filterSetting);
            }
        }

        protected override void ClearFilterEdits()
        {
            _tbName.Text = string.Empty;
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (_cdgvData.DataGrid.SelectedRows.Count == 1)
            {
                Edit_Click();
            }
            else
            {
                var selected = _cdgvData.DataGrid.SelectedRows;

                for (var i = 0; i < selected.Count; i++)
                {
                    EditFromPosition(selected[i].Index);
                }
            }
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
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
    }
}
