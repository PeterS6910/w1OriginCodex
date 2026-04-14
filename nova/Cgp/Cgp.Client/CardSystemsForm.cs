using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Components;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.Parsing;

namespace Contal.Cgp.Client
{
    public partial class CardSystemsForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<CardSystem, CardSystemShort>
#endif
    {
        public CardSystemsForm()
        {
            InitializeComponent();
            SetCardTypeFilter();
            InitCGPDataGridView();
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

            foreach (CardSystemShort cs in bindingSource)
            {
                cs.Symbol = _cdgvData.GetDefaultImage(cs);
            }
        }

        private void SetCardTypeFilter()
        {
            try
            {
                IList<CardTypes> cardTypes = CardTypes.GetCardTypesList();
                _cbCardTypeFilter.SuspendLayout();
                _cbCardTypeFilter.Items.Add("");
                foreach (CardTypes cardType in cardTypes)
                {
                    if (cardType.ValueByte != (byte)CardType.DirectSerial)
                        _cbCardTypeFilter.Items.Add(cardType);
                }
            }
            catch
            { }
            finally
            {
                _cbCardTypeFilter.ResumeLayout();
            }
        }

        private static volatile CardSystemsForm _singleton = null;
        private static object _syncRoot = new object();


        public static CardSystemsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new CardSystemsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        protected override CardSystem GetObjectForEdit(CardSystemShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectForEditById(listObj.IdCardSystem, out editAllowed);
        }

        protected override CardSystem GetFromShort(CardSystemShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectById(listObj.IdCardSystem);
        }

        protected override CardSystem GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.CardSystems.GetObjectById(idObj);
        }


        #region ShowData

        protected override ICollection<CardSystemShort> GetData()
        {
            Exception error;
            ICollection<CardSystemShort> list = CgpClient.Singleton.MainServerProvider.CardSystems.ShortSelectByCriteria(_filterSettings, out error);

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
            int currentRowIndex = _cdgvData.CurrentRowIndex;
            object objectForDelete = null;

            foreach (object obj in bindingSource)
            {
                if (obj != null)
                {
                    CardSystemShort cardSystemShort = obj as CardSystemShort;
                    if (cardSystemShort.CardType == (byte)CardType.DirectSerial)
                    {
                        objectForDelete = obj;
                        break;
                    }
                }
            }

            if (objectForDelete != null)
            {
                bindingSource.Remove(objectForDelete);
                if (currentRowIndex > -1)
                    bindingSource.Position = currentRowIndex;
            }
            //_dgValues.Enabled = false;

            // Filter by full company code
            if (_eCompanyCodeFilter.Text != null && _eCompanyCodeFilter.Text != string.Empty)
            {
                List<CardSystemShort> cardSystemsToDelete = new List<CardSystemShort>();
                for (int index = 0; index < bindingSource.Count; index++)
                {
                    CardSystemShort cardSystemShort = bindingSource[index] as CardSystemShort;
                    if (cardSystemShort != null)
                    {
                        if (cardSystemShort.FullCompanyCode == null || cardSystemShort.FullCompanyCode.IndexOf(_eCompanyCodeFilter.Text) < 0)
                        {
                            cardSystemsToDelete.Add(cardSystemShort);
                        }
                    }
                }

                if (cardSystemsToDelete != null && cardSystemsToDelete.Count > 0)
                {
                    foreach (CardSystemShort cardSystemShort in cardSystemsToDelete)
                    {
                        bindingSource.Remove(cardSystemShort);
                    }
                }
            }

            _cdgvData.ModifyGridView(bindingSource, CardSystemShort.COLUMN_SYMBOL, CardSystemShort.COLUMNNAME, CardSystemShort.COLUMNFULLCOMPANYCODE, CardSystemShort.COLUMNDESCRIPTION);
            _lRecordCount.BeginInvoke(new Action(() =>
    _lRecordCount.Text = string.Format("{0} : {1}",
                                       GetString("TextRecordCount"),
                                       bindingSource?.Count ?? 0)));
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        protected override bool Compare(CardSystem obj1, CardSystem obj2)
        {
            return obj1.Compare(obj2);
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        protected override ACgpEditForm<CardSystem> CreateEditForm(CardSystem obj, ShowOptionsEditForm showOption)
        {
            return new CardSystemsEditForm(obj, showOption);
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

        protected override void DeleteObj(CardSystem obj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.CardSystems.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.CardSystems.DeleteById(idObj, out error))
                throw error;
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = "";
            _eCompanyCodeFilter.Text = "";
            _cbCardTypeFilter.SelectedItem = null;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void SetFilterSettings()
        {
            if (_eNameFilter.Text != "")
            {
                FilterSettings filterSetting = new FilterSettings(CardSystem.COLUMNNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSetting);
            }

            //if (_eCompanyCodeFilter.Text != "")
            //{
            //    FilterSettings filterSetting = new FilterSettings(CardSystem.COLUMNCOMPANYCODE, _eCompanyCodeFilter.Text, ComparerModes.LIKEBOTH);
            //    _filterSettings.Add(filterSetting);
            //}

            if (_cbCardTypeFilter.SelectedItem != null && _cbCardTypeFilter.SelectedItem is CardTypes)
            {
                FilterSettings filterSetting = new FilterSettings(CardSystem.COLUMNCARDTYPE, (byte)(_cbCardTypeFilter.SelectedItem as CardTypes).Value, ComparerModes.EQUALL);
                _filterSettings.Add(filterSetting);
            }
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
            if (sender == _cbCardTypeFilter)
                _bRunFilter_Click(sender, e);
            else if (sender == _eCompanyCodeFilter)
            {
                string validtext = QuickParser.GetValidPrintableString(_eCompanyCodeFilter.Text);
                if (_eCompanyCodeFilter.Text != validtext)
                {
                    _eCompanyCodeFilter.Text = validtext;
                    _eCompanyCodeFilter.SelectionStart = _eCompanyCodeFilter.TextLength;
                    _eCompanyCodeFilter.SelectionLength = 0;
                }
            }
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
                    return CgpClient.Singleton.MainServerProvider.CardSystems.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(CardSystem cardSystem)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CardSystems.HasAccessViewForObject(cardSystem);

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
                    return CgpClient.Singleton.MainServerProvider.CardSystems.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.CardSystems.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
