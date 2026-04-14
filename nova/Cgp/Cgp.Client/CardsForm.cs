using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class CardsForm :
#if DESIGNER
        Form
#else
        ACgpTableForm<Card, CardShort>
#endif
    {
        public CardsForm()
        {
            InitializeComponent();

            FormOnEnter += FormEnter;
            InitCGPDataGridView();
        }

        protected override void RegisterEvents()
        {
            CUDObjectHandler.Singleton.Register(
                OnCardChanged,
                ObjectType.Card);
        }

        protected override void UnregisterEvents()
        {
            CUDObjectHandler.Singleton.Unregister(
                OnCardChanged,
                ObjectType.Card);
        }

        private void OnCardChanged(ObjectType objectType, object id, bool isInsert)
        {
            if (isInsert)
                return;

            var cardFromDatabase = CgpClient.Singleton.MainServerProvider.Cards.GetObjectById(id);

            if (cardFromDatabase == null)
                return;

            var bindingSource = _cdgvData.DataGrid.DataSource as BindingSource;

            if (bindingSource == null)
                return;

            for (int cardIdx = 0;
                cardIdx < bindingSource.Count; ++cardIdx)
            {
                var cardShort = (CardShort)bindingSource[cardIdx];

                if (!cardShort.IdCard.Equals(id))
                    continue;

                cardShort.CardState = cardFromDatabase.State;

                cardShort.StringCardState =
                    GetString("CardStates_" + ((CardState)cardShort.CardState));

                cardShort.Symbol = _cdgvData.GetSubTypeImage(cardShort, cardShort.CardState);

                _cdgvData.DataGrid.Rows[cardIdx].Cells[CardShort.COLUMN_STATE].Value =
                    cardShort.StringCardState;

                _cdgvData.DataGrid.Rows[cardIdx].Cells[CardShort.COLUMN_SYMBOL].Value =
                    cardShort.Symbol;

                return;
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
            string generalTrue = GetString("General_true");
            foreach (CardShort cardShort in bindingSource.List)
            {
                cardShort.Symbol = _cdgvData.GetSubTypeImage(cardShort, cardShort.CardState);
                cardShort.StringCardState = GetString("CardStates_" + ((CardState)cardShort.CardState));
                cardShort.TimetecSync = (cardShort.TimetecSync.CompareTo("True") == 0 || cardShort.TimetecSync.CompareTo(generalTrue) == 0) ? generalTrue : GetString("General_false");
            }
        }

        protected override Card GetObjectForEdit(CardShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.Cards.GetObjectForEditById(listObj.IdCard, out editAllowed);
        }

        protected override Card GetFromShort(CardShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.Cards.GetObjectById(listObj.IdCard);
        }

        protected override Card GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.Cards.GetObjectById(idObj);
        }

        private void FormEnter(Form form)
        {
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            IList<CardStates> cardStates = CardStates.GetCardStatesList(LocalizationHelper);

            CardStates selectedCardState = null;
            if (_cbCardStateFilter.Items.Count != 0)
                selectedCardState = _cbCardStateFilter.SelectedItem as CardStates;

            _cbCardStateFilter.Items.Clear();
            _cbCardStateFilter.Items.Add("");
            if (selectedCardState == null)
                if (_cbCardStateFilter.SelectedItem != null && _cbCardStateFilter.SelectedItem.ToString() != "")
                    _cbCardStateFilter.SelectedItem = "";

            foreach (CardStates cardState in cardStates)
            {
                _cbCardStateFilter.Items.Add(cardState);
                if (selectedCardState != null && cardState.Value == selectedCardState.Value)
                    _cbCardStateFilter.SelectedItem = cardState;
            }

            CardSystem selectedCardSystem = null;

            if (_cbCardSystemFilter.Items.Count != 0)
                selectedCardSystem = _cbCardSystemFilter.SelectedItem as CardSystem;

            _cbCardSystemFilter.Items.Clear();
            _cbCardSystemFilter.Items.Add("");
            if (selectedCardSystem == null)
                if (_cbCardSystemFilter.SelectedItem != null && _cbCardSystemFilter.SelectedItem.ToString() != "")
                    _cbCardSystemFilter.SelectedItem = "";

            Exception error;
            ICollection<CardSystem> cardSystemList =
                CgpClient.Singleton.MainServerProvider.CardSystems.List(out error);

            foreach (CardSystem cardSystem in cardSystemList)
            {
                _cbCardSystemFilter.Items.Add(cardSystem);
                if (cardSystem.Compare(selectedCardSystem))
                {
                    _cbCardSystemFilter.SelectedItem = cardSystem;
                }
            }

            bool hasAccessCardPrintPerform =
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    BaseAccess.GetAccess(LoginAccess.IdManagementCardPrintPerform));

#if !DEBUG
            if (hasAccessCardPrintPerform)
            {
                string localisedName = string.Empty;
                object value = null;
                if (CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(Cgp.Globals.RequiredLicenceProperties.IdManagement.ToString(), out localisedName, out value))
                {
                    if (value is bool && (bool)value == true)
                        _bPrint.Enabled = true;
                }
            }
#else
            _bPrint.Enabled = hasAccessCardPrintPerform;
#endif

            FillTypeOfAssigment();
        }

        private void FillTypeOfAssigment()
        {
            TypeOfAssigmentItem selected = null;
            if (_cbPersonFilter.SelectedItem is TypeOfAssigmentItem)
            {
                selected = (TypeOfAssigmentItem)_cbPersonFilter.SelectedItem;
            }

            _cbPersonFilter.Items.Clear();
            _cbPersonFilter.Items.Add(string.Empty);

            TypeOfAssigmentItem assigned = new TypeOfAssigmentItem(GetString("General_Assigned"), true);
            _cbPersonFilter.Items.Add(assigned);
            if (selected != null && selected.Assigned)
            {
                _cbPersonFilter.SelectedItem = assigned;
            }

            TypeOfAssigmentItem notAssigned = new TypeOfAssigmentItem(GetString("General_NotAssigned"), false);
            _cbPersonFilter.Items.Add(notAssigned);
            if (selected != null && !selected.Assigned)
            {
                _cbPersonFilter.SelectedItem = notAssigned;
            }
        }

        private static volatile CardsForm _singleton = null;
        private static object _syncRoot = new object();

        public static CardsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new CardsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }
                return _singleton;
            }
        }

        protected override ICollection<CardShort> GetData()
        {
            Exception error;
            ICollection<CardShort> list = CgpClient.Singleton.MainServerProvider.Cards.ShortSelectByCriteria(_filterSettings, out error);
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

        protected override bool Compare(Card obj1, Card obj2)
        {
            return obj1.Compare(obj2);
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            if (CgpClient.Singleton.MainServerProvider.CheckTimetecLicense())
            {
                // Slovak market has different columns
                _cdgvData.ModifyGridView(bindingSource, CardShort.COLUMN_SYMBOL, CardShort.COLUMN_NUMBER,
                CardShort.COLUMN_FULL_CARD_NUMBER, CardShort.COLUMN_CARD_SYSTEM, CardShort.COLUMN_PERSONAL_ID, CardShort.COLUMN_PERSON, CardShort.COLUMN_STATE,
                CardShort.COLUMN_TIMETEC_SYNC, CardShort.COLUMN_TIMETEC_VALIDFROM, CardShort.COLUMN_TIMETEC_VALIDTO, CardShort.COLUMN_DESCRIPTION);
            }
            else
            {
                _cdgvData.ModifyGridView(bindingSource, CardShort.COLUMN_SYMBOL, CardShort.COLUMN_NUMBER,
                CardShort.COLUMN_FULL_CARD_NUMBER, CardShort.COLUMN_CARD_SYSTEM, CardShort.COLUMN_PERSONAL_ID, CardShort.COLUMN_PERSON, CardShort.COLUMN_STATE, CardShort.COLUMN_DESCRIPTION);
            }
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        #endregion

        protected override ACgpEditForm<Card> CreateEditForm(Card obj, ShowOptionsEditForm showOption)
        {
            return new CardsEditForm(obj, showOption);
        }

        protected override void DeleteObj(Card obj)
        {
            Exception error;

            if (obj.Person != null)
            {
                if (Dialog.WarningQuestion(GetString("WarningCardIsAssignedToPerson")))
                {
                    bool allowedEdit;
                    var card = CgpClient.Singleton.MainServerProvider.Cards.GetObjectForEditById(obj.GetId(),
                        out allowedEdit);

                    if (card != null && allowedEdit)
                    {
                        card.Person = null;
                        if (!CgpClient.Singleton.MainServerProvider.Cards.Update(card, out error))
                            throw error;
                    }
                }
                else
                {
                    return;
                }
            }

            if (!CgpClient.Singleton.MainServerProvider.Cards.Delete(obj, out error))
                throw error;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.Cards.DeleteById(idObj, out error))
                throw error;
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();

            if (_eCardNumberFilter.Text != "")
            {
                FilterSettings filterSetting = new FilterSettings(Card.COLUMNFULLCARDNUMBER, _eCardNumberFilter.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSetting);
            }

            if (_cbCardSystemFilter.SelectedItem is CardSystem)
            {
                FilterSettings filterSetting;

                if ((_cbCardSystemFilter.SelectedItem as CardSystem).CardType == (byte)(CardType.DirectSerial))
                    filterSetting = new FilterSettings(Card.COLUMNCARDSYSTEM, null, ComparerModes.EQUALL);
                else
                    filterSetting = new FilterSettings(Card.COLUMNCARDSYSTEM, _cbCardSystemFilter.SelectedItem, ComparerModes.EQUALL);
                
                _filterSettings.Add(filterSetting);
            }

            if (_cbPersonFilter.SelectedItem is TypeOfAssigmentItem)
            {
                FilterSettings filterSetting;
                if ((_cbPersonFilter.SelectedItem as TypeOfAssigmentItem).Assigned)
                    filterSetting = new FilterSettings(Card.COLUMNPERSON, null, ComparerModes.NOTEQUALL);
                else
                    filterSetting = new FilterSettings(Card.COLUMNPERSON, null, ComparerModes.EQUALL);

                _filterSettings.Add(filterSetting);
            }
            else
            {
                var persons = GetPersons(_cbPersonFilter.Text);

                if (persons != null)
                {
                    FilterSettings filterSetting = new FilterSettings(Card.COLUMNPERSON, persons, ComparerModes.IN);
                    _filterSettings.Add(filterSetting);
                }
            }

            if (_cbCardStateFilter.SelectedItem is CardStates)
            {
                FilterSettings filterSetting = null;
                filterSetting = new FilterSettings(Card.COLUMNSTATE, (byte)(_cbCardStateFilter.SelectedItem as CardStates).Value, ComparerModes.EQUALL);
                _filterSettings.Add(filterSetting);
            }
        }

        private ICollection<Person> GetPersons(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Exception error;
            List<FilterSettings> personFilterSettings = new List<FilterSettings>()
            {
                new FilterSettings(CentralNameRegister.COLUMN_NAME, name, ComparerModes.LIKEBOTH),
                new FilterSettings(CentralNameRegister.COLUMN_OBJECTTYPE, (byte) ObjectType.Person, ComparerModes.EQUALL)
            };

            var list = CgpClient.Singleton.MainServerProvider.CentralNameRegisters.SelectByCriteria(personFilterSettings, out error);

            if (error != null)
                throw (error);

            ICollection<Person> persons = new LinkedList<Person>();

            foreach (var personInfo in list)
            {
                persons.Add(CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(personInfo.Id));
            }

            return persons;
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eCardNumberFilter.Text = "";
            _cbCardSystemFilter.SelectedItem = null;
            _cbPersonFilter.SelectedItem = null;
            _cbCardStateFilter.SelectedItem = null;
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, e);
            if (sender == _cbCardStateFilter || sender == _cbCardSystemFilter)
                _bRunFilter_Click(sender, e);
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
                    return CgpClient.Singleton.MainServerProvider.Cards.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(Card card)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Cards.HasAccessViewForObject(card);

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
                    return CgpClient.Singleton.MainServerProvider.Cards.HasAccessInsert();

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
                    return CgpClient.Singleton.MainServerProvider.Cards.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void _bCSVImport_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;
            {
#if !DEBUG
                string localisedName = string.Empty;
                object value = null;
                if (!CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(Cgp.Globals.RequiredLicenceProperties.OfflineImport.ToString(), out localisedName, out value))
                {
                    //Dialog.Error(GetString("ErrorCanNotObtainLicencePropertyInfo") + " (" + Cgp.Globals.RequiredLicenceProperties.OfflineImport.ToString() + ")");
                    Dialog.Error(GetString("ErrorFeatureNotSupportedByLicense"));
                    return;
                }
                if (value is bool && (bool)value == true)
                {
#endif
                CSVCardImportDialog csvImportDialog = new CSVCardImportDialog();
                csvImportDialog.ShowDialog();
#if !DEBUG
                }
                else
                    Dialog.Error(GetString("LicenceCanNotImportData"));
#endif
            }
        }

        protected override Card GetObjectIfExists(object obj)
        {
            try
            {
                if (obj is string)
                    return CgpClient.Singleton.MainServerProvider.Cards.GetCardByFullNumber((string)obj);
                else if (obj is Card)
                {
                    Card card = obj as Card;
                    if (card != null)
                        return CgpClient.Singleton.MainServerProvider.Cards.GetCardByFullNumber(card.FullCardNumber);
                }
            }
            catch { }

            return null;
        }

        protected override bool QuestionOpenEditFormIfObjectAlreadyExists()
        {
            return Contal.IwQuick.UI.Dialog.Question(GetString("QuestionCardOpenEditFormIfObjectAlreadyExists"));
        }

        private void _bPrint_Click(object sender, EventArgs e)
        {
            if (_cdgvData.DataGrid.SelectedRows == null || _cdgvData.DataGrid.SelectedRows.Count == 0)
                return;

            List<Card> selectedCards = new List<Card>();
            foreach (DataGridViewRow row in _cdgvData.DataGrid.SelectedRows)
            {
                Card card = null;
                if (GetObjectFromRow(row.Index, ref card) && card != null)
                    selectedCards.Add(card);
            }

            try
            {
                CardPrintForm printForm = new CardPrintForm(selectedCards.ToArray(), null);
                printForm.Show();
            }
            catch (Exception ex)
            {
                Dialog.Error(GetString("ExceptionOccured") + ": " + ex.Message);
            }
        }

        private void _cbPersonFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            base.FilterValueChanged(sender, null);
            base.RunFilter();
        }

        private void bExportExcel_Click(object sender, EventArgs e)
        {
            var exportForm=new ExportForm(_filterSettings);
            exportForm.TopMost = true;
            exportForm.Show();
        }
    }

    public class TypeOfAssigmentItem
    {
        private readonly string _name;
        private readonly bool _assigned;

        public bool Assigned { get { return _assigned; } }

        public TypeOfAssigmentItem(string name, bool assigned)
        {
            _name = name;
            _assigned = assigned;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
