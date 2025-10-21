using Contal.Cgp.BaseLib;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Contal.Cgp.Client
{
    public partial class PersonsForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<Person, PersonShort>
#endif
    {
        private string _fullFilterSettingsText = string.Empty;
        private UserFoldersStructure _departmentFilter;
        private readonly IList<FilterSettings>[] _filterSettingsWithJoin;
        private LogicalOperators _filterSettingsJoinOperator = LogicalOperators.AND;

        public PersonsForm()
        {
            InitializeComponent();
            _tbdpDateFromFilter.LocalizationHelper = LocalizationHelper;
            _tbdpDateToFilter.LocalizationHelper = LocalizationHelper;
            InitCGPDataGridView();
            _filterSettingsWithJoin = new IList<FilterSettings>[] { _filterSettings, new List<FilterSettings>() };
            _cbActivePersons.Checked = true;
            _cbInactivePersons.Checked = false;
            FilterValueChanged(this, EventArgs.Empty);
            this.Load += PersonsForm_Load;
        }

        private void PersonsForm_Load(object sender, EventArgs e)
        {
            RunFilter();
            this.Load -= PersonsForm_Load;
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvData.ImageList = ObjectImageList.Singleton.ClientObjectImages;
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
            _cdgvData.DefaultSortColumnName = PersonShort.COLUMNSURNAME;
            _cdgvData.DefaultSortDirection = ListSortDirection.Ascending;
            _cdgvData.CgpDataGridEvents = this;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            foreach (PersonShort personShort in bindingSource.List)
            {
                personShort.Symbol = _cdgvData.GetDefaultImage(personShort);
                personShort.TimetecSync = personShort.TimetecSync == "True" ? GetString("General_true") : GetString("General_false");
            }
        }

        private static volatile PersonsForm _singleton = null;
        private static object _syncRoot = new object();

        public static PersonsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new PersonsForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }

                return _singleton;
            }
        }

        protected override Person GetObjectForEdit(PersonShort listObj, out bool editAllowed)
        {
            return CgpClient.Singleton.MainServerProvider.Persons.GetObjectForEditById(listObj.IdPerson, out editAllowed);
        }

        protected override Person GetFromShort(PersonShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(listObj.IdPerson);
        }

        protected override Person GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(idObj);
        }

        protected override bool Compare(Person obj1, Person obj2)
        {
            return obj1.Compare(obj2);
        }

        #region CRUD Person
        private void _bInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        protected override ACgpEditForm<Person> CreateEditForm(Person obj, ShowOptionsEditForm showOption)
        {
            return new PersonEditForm(obj, showOption);
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

        protected override bool? ShowCustomQuestionDeleteConfirm(Person deletedObj)
        {
            if (deletedObj != null &&
                CgpClient.Singleton.MainServerProvider.Persons.HasAssignedCards(deletedObj.IdPerson))
            {
                return Dialog.Question(GetString("QuestionDeletePersonWithAssignedCards"));
            }

            return base.ShowCustomQuestionDeleteConfirm(deletedObj);
        }

        protected override void DeleteObj(Person obj)
        {
            Exception error;

            if (!CgpClient.Singleton.MainServerProvider.Persons.Delete(obj, out error))
            {
                throw error;
            }
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception error;
            if (!CgpClient.Singleton.MainServerProvider.Persons.DeleteById(idObj, out error))
                throw error;
        }

        private bool CheckDateFromDateTo()
        {
            DateTime dateFrom = DateTime.MinValue;
            DateTime dateTo = DateTime.MaxValue;

            if (_tbdpDateFromFilter.Text != string.Empty)
                DateTime.TryParse(_tbdpDateFromFilter.Text, out dateFrom);

            if (_tbdpDateToFilter.Text != string.Empty)
                DateTime.TryParse(_tbdpDateToFilter.Text, out dateTo);

            if (dateTo < dateFrom)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _tbdpDateToFilter.TextBox,
                    GetString("ErrorDateTo"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                _tbdpDateToFilter.TextBox.Focus();
                return false;
            }

            return true;
        }

        private bool CheckDate(object sender, string requiredFormat)
        {
            if (sender is TextBox)
            {
                TextBox textBox = sender as TextBox;
                if (textBox.Text == string.Empty) return true;

                DateTime dt;
                if (!DateTime.TryParse(textBox.Text, out dt))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, textBox,
                        GetString("ErrorInsertDateBirthday") + " (" + requiredFormat + ")"
                        , Contal.IwQuick.UI.ControlNotificationSettings.Default);
                    textBox.Focus();
                    return false;
                }
                else
                {
                    if (dt < DateTime.Parse("1.1.1753"))
                    {
                        Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, textBox,
                        GetString("ErrorDateRange"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                        textBox.Focus();
                        return false;
                    }
                }
            }
            return true;
        }

        private void _tbdpDateFilter_Leave(object sender, EventArgs e)
        {
            CheckDate(sender, "");
        }
        #endregion

        #region Filters
        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void SelectDepartmentClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                Exception error;
                var listDepartments = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.ListDepartments(
                    CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode"),
                    @"\", null, out error);

                if (error != null) throw error;

                ListboxFormAdd formAdd = new ListboxFormAdd(
                    listDepartments,
                    CgpClient.Singleton.LocalizationHelper.GetString(
                        "UserFoldersStructuresFormUserFoldersStructuresForm"));

                object outUserFolder;
                formAdd.ShowDialog(out outUserFolder);
                if (outUserFolder != null)
                {
                    _departmentFilter = outUserFolder as UserFoldersStructure;
                    _tbmDepartmentFilter.Text = _departmentFilter?.FolderName ?? string.Empty;
                    FilterValueChanged(this, EventArgs.Empty);
                    RunFilter();
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        protected override void SetFilterSettings()
        {
            foreach (var list in _filterSettingsWithJoin)
                list.Clear();

            bool useOr = _cbActivePersons.Checked && !_cbInactivePersons.Checked;
            _filterSettingsJoinOperator = useOr ? LogicalOperators.OR : LogicalOperators.AND;

            IList<FilterSettings> filtersA = _filterSettingsWithJoin[0];
            IList<FilterSettings> filtersB = _filterSettingsWithJoin[1];

            Action<string, object, ComparerModes> addFilter = (column, value, comparer) =>
            {
                var fsA = new FilterSettings(column, value, comparer, LogicalOperators.AND);
                filtersA.Add(fsA);
                if (useOr)
                    filtersB.Add(new FilterSettings(column, value, comparer, LogicalOperators.AND));
            };

            if (!string.IsNullOrEmpty(_eNameFilter.Text))
                addFilter(Person.COLUMNFIRSTNAME, _eNameFilter.Text, ComparerModes.LIKEBOTH);

            if (!string.IsNullOrEmpty(_eSurnameFilter.Text))
                addFilter(Person.COLUMNSURNAME, _eSurnameFilter.Text, ComparerModes.LIKEBOTH);

            if (_tbdpDateFromFilter.Text != "")
                addFilter(Person.COLUMNBIRTHDAY, DateTime.Parse(_tbdpDateFromFilter.Text), ComparerModes.EQUALLMORE);

            if (_tbdpDateToFilter.Text != "")
                addFilter(Person.COLUMNBIRTHDAY, DateTime.Parse(_tbdpDateToFilter.Text), ComparerModes.EQUALLLESS);

            if (!string.IsNullOrEmpty(_eNumberFilter.Text))
                addFilter(Person.COLUMNIDENTIFICATION, _eNumberFilter.Text, ComparerModes.LIKE);

            if (!string.IsNullOrEmpty(_eOtherInformationFiledsFilter.Text))
                addFilter(Person.COLUMNOTHERINFORMATIONFIELDS, _eOtherInformationFiledsFilter.Text, ComparerModes.LIKEBOTH);

            if (useOr)
            {
                filtersA.Add(new FilterSettings(Person.COLUMNEMPLOYMENTENDDATE, DateTime.Now, ComparerModes.EQUALLMORE, LogicalOperators.AND));
                filtersB.Add(new FilterSettings(Person.COLUMNEMPLOYMENTENDDATE, null, ComparerModes.EQUALL, LogicalOperators.AND));
            }
            else if (!_cbActivePersons.Checked && _cbInactivePersons.Checked)
            {
                filtersA.Add(new FilterSettings(Person.COLUMNEMPLOYMENTENDDATE, DateTime.Now, ComparerModes.LESS, LogicalOperators.AND));
                filtersA.Add(new FilterSettings(Person.COLUMNEMPLOYMENTENDDATE, null, ComparerModes.NOTEQUALL, LogicalOperators.AND));
            }

            if (_departmentFilter != null)
                addFilter(Person.COLUMNDEPARTMENT, _departmentFilter?.FolderName, ComparerModes.EQUALL);

            _fullFilterSettingsText = _tbFullTextSearch.Text;
        }

        protected override bool CheckFilterValues()
        {
            return CheckDateFromDateTo();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _fullFilterSettingsText = string.Empty;
            foreach (var list in _filterSettingsWithJoin)
                list.Clear();
            ClearFilterEdits();
            FilterValueChanged(this, EventArgs.Empty);
            RunFilter();
        }

        protected override void ClearFilterEdits()
        {
            _eNameFilter.Text = string.Empty;
            _eNumberFilter.Text = string.Empty;
            _eSurnameFilter.Text = string.Empty;
            _tbFullTextSearch.Text = string.Empty;
            _tbdpDateFromFilter.Value = null;
            _tbdpDateToFilter.Value = null;
            _eOtherInformationFiledsFilter.Text = string.Empty;
            _cbActivePersons.Checked = true;
            _cbInactivePersons.Checked = false;
            _departmentFilter = null;
            _tbmDepartmentFilter.Text = string.Empty;
        }

        #endregion

        #region ShowData

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ICollection<PersonShort> GetData()
        {
            Exception error;
            var list = CgpClient.Singleton.MainServerProvider.Persons.ShortSelectByCriteria(
                out error,
                _filterSettingsJoinOperator,
                _filterSettingsWithJoin);

            if (error != null)
                throw (error);

            CheckAccess();

            if (list != null
                && !string.IsNullOrEmpty(_fullFilterSettingsText))
            {
                var persons = list.Where(
                    person => string.Format("{0} {1} {2} {3}",
                        RemoveDiacritism(person.Title).ToLower(),
                        RemoveDiacritism(person.FirstName).ToLower(),
                        RemoveDiacritism(person.MiddleName).ToLower(),
                        RemoveDiacritism(person.Surname).ToLower()).
                        Contains(RemoveDiacritism(_fullFilterSettingsText).ToLower())).ToList();

                if (persons.Any())
                    list = persons;
                else
                    list.Clear();
            }

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
                _bCSVImport.Visible = HasAccessCsvImport();
            }
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            if (CgpClient.Singleton.MainServerProvider.CheckTimetecLicense())
            {
                // Slovak market has different columns
                _cdgvData.ModifyGridView(bindingSource, PersonShort.COLUMN_SYMBOL, PersonShort.COLUMNIDENTIFICATION, PersonShort.COLUMNFIRSTNAME, PersonShort.COLUMNSURNAME,
                PersonShort.COLUMNDEPARTMENT, PersonShort.COLUMNBIRTHDAY, PersonShort.COLUMN_TIMETEC_SYNC, PersonShort.COLUMNDESCRIPTION);
            }
            else
            {
                _cdgvData.ModifyGridView(bindingSource, PersonShort.COLUMN_SYMBOL, PersonShort.COLUMNFIRSTNAME, PersonShort.COLUMNSURNAME,
                PersonShort.COLUMNDEPARTMENT, PersonShort.COLUMNBIRTHDAY, PersonShort.COLUMNDESCRIPTION);
            }
        }

        public static string RemoveDiacritism(string Text)
        {
            if (string.IsNullOrEmpty(Text))
                return string.Empty;

            string stringFormD = Text.Normalize(System.Text.NormalizationForm.FormD);
            var retVal = new System.Text.StringBuilder();
            for (int index = 0; index < stringFormD.Length; index++)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stringFormD[index]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    retVal.Append(stringFormD[index]);
            }
            return retVal.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        #endregion

        private void _bClearDateFrom_Click(object sender, EventArgs e)
        {
            if (_tbdpDateFromFilter.Text != string.Empty)
                _tbdpDateFromFilter.Value = null;
        }

        private void _bClearDateTo_Click(object sender, EventArgs e)
        {
            if (_tbdpDateToFilter.Text != string.Empty)
                _tbdpDateToFilter.Value = null;
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
            if (sender is DateTimePicker)
            {
                if (_tbdpDateToFilter.Value != null)
                {
                    DateTime dateTo = _tbdpDateToFilter.Value.Value.Date;
                    dateTo = dateTo.AddHours(24);
                    dateTo = dateTo.AddMilliseconds(-1);

                    _tbdpDateToFilter.Value = dateTo;
                }
            }

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
                    return CgpClient.Singleton.MainServerProvider.Persons.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(Person person)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Persons.HasAccessViewForObject(person);

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
                    return CgpClient.Singleton.MainServerProvider.Persons.HasAccessInsert();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessUpdate()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.Persons.HasAccessUpdate();

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
                    return CgpClient.Singleton.MainServerProvider.Persons.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessCsvImport()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.HasAccess(BaseAccess.GetAccess(LoginAccess.PersonsCsvImportPerform));

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void _bCSVImport_Click(object sender, EventArgs e)
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
            CSVImportDialog csvImportDialog = new CSVImportDialog();
            csvImportDialog.ShowDialog();
#if !DEBUG
            }
            else
                Dialog.Error(GetString("LicenceCanNotImportData"));
#endif
        }

        private void SetVisiblePluginButtons()
        {
            bool visibleButton = false;
            foreach (ICgpVisualPlugin item in CgpClient.Singleton.PluginManager.GetVisualPlugins())
            {
                if (item.FriendlyName == "NCAS plugin")
                {
                    visibleButton = true;
                    break;
                }
            }
            _bAclAssignment.Visible = visibleButton;
        }

        private void _bAclAssignment_Click(object sender, EventArgs e)
        {
            IPluginMainForm obj = null;
            foreach (ICgpVisualPlugin item in CgpClient.Singleton.PluginManager.GetVisualPlugins())
            {
                if (item.FriendlyName == "NCAS plugin")
                {
                    obj = item.GetEditPluginForm();
                }
            }

            if (obj != null)
            {

                obj.SpecialAction(GetSelectedPersons());
            }
        }

        private List<object> GetSelectedPersons()
        {
            List<object> idSelectedPersons = new List<object>();

            BindingSource bs = _cdgvData.DataGrid.DataSource as BindingSource;

            for (int i = 0; i < _cdgvData.SelectedRows.Count; i++)
            {
                PersonShort personShort = (PersonShort)bs.List[_cdgvData.SelectedRows[i].Index];
                idSelectedPersons.Add(personShort.IdPerson as object);
            }

            return idSelectedPersons;
        }
    }
}
