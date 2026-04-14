using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Cgp.Components;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Components;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.UI;
using Contal.Cgp.Globals;
using Contal.IwQuick.Threads;
using Contal.IwQuick;

using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;
using SqlUniqueException = Contal.IwQuick.SqlUniqueException;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASACLPersonEditForm :
#if DESIGNER    
        Form
#else
 PluginMainForm<NCASClient>, ICgpDataGridView
#endif
    {
        private ListOfObjects _actAccessControlLists;
        private readonly Person _actPerson;
        private BindingSource _bindingSource;
        private ACLPerson _editingACLPerson;
        DVoid2Void _dAfterTranslateForm;
        private readonly bool _allowEdit;
        private readonly Control _parentControl;

        public override NCASClient Plugin
        {
            get { return NCASClient.Singleton; }
        }

        public NCASACLPersonEditForm(Person person, Control control, bool allowEdit)
            : base(NCASClient.LocalizationHelper, CgpClientMainForm.Singleton)
        {
            InitializeComponent();
            _tbdpDateFrom.LocalizationHelper = LocalizationHelper;
            _tbdpDateTo.LocalizationHelper = LocalizationHelper;
            LocalizationHelper.TranslateForm(this);
            _pBack.Parent = control;
            _parentControl = control;
            _actPerson = person;
            control.Enter += RunOnEnter;
            control.Disposed += RunOnDisposed;
            ResetValues();
            MinimumSize = new Size(405, 334);
            _allowEdit = allowEdit;
            InitCGPDataGridView();
            _cdgvData.DataGrid.MultiSelect = true;
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = NCASClient.LocalizationHelper;
            _cdgvData.ImageList = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();
            _cdgvData.CgpDataGridEvents = this;
            _cdgvData.DataGrid.DragDrop += _dgValues_DragDrop;
            _cdgvData.DataGrid.DragOver += _dgValues_DragOver;
            _cdgvData.EnabledInsertButton = false;
        }

        protected override void AfterTranslateForm()
        {
            LocalizationHelper.TranslateControl(_pBack);
        }

        void RunOnEnter(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_dAfterTranslateForm == null)
            {
                _dAfterTranslateForm = AfterTranslateForm;
                LocalizationHelper.LanguageChanged += _dAfterTranslateForm;
            }

            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (_allowEdit)
                EnabledForm();
            else
                DisabledForm();

            SafeThread.StartThread(ShowACLPersons);
        }

        private void DisabledForm()
        {
            _tbmAccessControlList.Enabled = false;
            _tbdpDateFrom.Enabled = false;
            _tbdpDateTo.Enabled = false;

            _bCreate1.Enabled = false;
            //_bEdit.Enabled = false;
            _cdgvData.EnabledDeleteButton = false;
        }

        private void EnabledForm()
        {
            _tbmAccessControlList.Enabled = true;
            _tbdpDateFrom.Enabled = true;
            _tbdpDateTo.Enabled = true;

            _bCreate1.Enabled = true;
            //_bEdit.Enabled = true;
            _cdgvData.EnabledDeleteButton = true;
        }

        void RunOnDisposed(object sender, EventArgs e)
        {
            LocalizationHelper.LanguageChanged -= _dAfterTranslateForm;
        }

        private void ShowACLPersons()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (_parentControl == null)
                return;

            Exception error;
            var aclPersons = Plugin.MainServerProvider.ACLPersons.GetAclPersonsByPerson(_actPerson.IdPerson, out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorLoadTableAccessDenied"));
                }
                else
                {
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorLoadTable"));
                }

                _parentControl.Invoke(new DVoid2Void(
                    delegate
                    {
                        _bindingSource = null;
                        _cdgvData.DataGrid.DataSource = null;
                    }));

                return;
            }

            _parentControl.Invoke(new DVoid2Void(
                delegate
                {
                    _bindingSource = 
                        new BindingSource
                        {
                            DataSource = aclPersons
                        };

                    _cdgvData.ModifyGridView(_bindingSource, ACLPerson.COLUMNACCESSCONTROLLIST, ACLPerson.COLUMNDATEFROM, ACLPerson.COLUMNDATETO);
                    _cdgvData.DataGrid.Columns["DateFrom"].DefaultCellStyle.Format = "MM-dd-yyyy HH:mm:ss";
                    _cdgvData.DataGrid.Columns["DateFrom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    _cdgvData.DataGrid.Columns["DateTo"].DefaultCellStyle.Format = "MM-dd-yyyy HH:mm:ss";
                }));
        }

        protected void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView == null) return;

            if (gridView.Columns.Contains(columnName))
                gridView.Columns[columnName].Visible = false;
        }

        private void RefreshAccessControlList()
        {
            if (_actAccessControlLists != null)
            {
                _tbmAccessControlList.Text = _actAccessControlLists.ToString();
                _tbmAccessControlList.TextImage = Plugin.GetImageForObjectType(ObjectType.AccessControlList);
            }
            else
            {
                _tbmAccessControlList.Text = string.Empty;
            }
        }

        private void ModifyAccesControlList()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                var listAccessControlList = new List<AOrmObject>();

                Exception error;
                var listAccessControlListFromDatabase = Plugin.MainServerProvider.AccessControlLists.List(out error);
                foreach (var accessControlList in listAccessControlListFromDatabase)
                {
                    listAccessControlList.Add(accessControlList);
                }

                var formAdd = new ListboxFormAdd(listAccessControlList, GetString("NCASAccessControlListsFormNCASAccessControlListsForm"));
                ListOfObjects outAccessControlLists;
                if (_bCreate1.Visible)
                {
                    formAdd.ShowDialogMultiSelect(out outAccessControlLists);
                    if (outAccessControlLists != null)
                    {
                        _actAccessControlLists = outAccessControlLists;
                        RefreshAccessControlList();
                    }
                }
                else
                {
                    object outAccessControlList;
                    formAdd.ShowDialog(out outAccessControlList);
                    if (outAccessControlList is AccessControlList)
                    {
                        outAccessControlLists = new ListOfObjects();
                        outAccessControlLists.Objects.Add(outAccessControlList);
                        _actAccessControlLists = outAccessControlLists;
                        RefreshAccessControlList();
                    }
                }
            }
            catch
            {
            }
        }

        private void _tbmAccessControlList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmAccessControlList_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddAccessControlList(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddAccessControlList(object newAccessControlList)
        {
            try
            {
                if (newAccessControlList.GetType() == typeof(AccessControlList))
                {
                    var accessControlList = newAccessControlList as AccessControlList;
                    _actAccessControlLists = new ListOfObjects();
                    _actAccessControlLists.Objects.Add(accessControlList);
                    RefreshAccessControlList();
                    Plugin.AddToRecentList(newAccessControlList);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmAccessControlList.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmAccessControlList_DoubleClick(object sender, EventArgs e)
        {
            if (_actAccessControlLists != null && _actAccessControlLists.Count == 1)
            {
                NCASAccessControlListsForm.Singleton.OpenEditForm(_actAccessControlLists[0] as AccessControlList);
            }
        }

        private bool ControlValues()
        {
            if (_actAccessControlLists == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmAccessControlList.ImageTextBox,
                GetString("ErrorEntryAccessControlList"), ControlNotificationSettings.Default);
                return false;
            }

            if ((_tbdpDateTo.Value != null && _tbdpDateFrom.Value != null) && _tbdpDateFrom.Value > _tbdpDateTo.Value)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                   _tbdpDateTo.TextBox, GetString("ErrorACLDateRange"), ControlNotificationSettings.Default);
                return false;
            }

            return true;
        }

        private void GetValues(ACLPerson aclPerson, AccessControlList accessControlList)
        {
            aclPerson.AccessControlList = accessControlList;
            aclPerson.Person = _actPerson;

            if (_tbdpDateFrom.Text != string.Empty)
            {
                DateTime dateTime;
                if (DateTime.TryParse(_tbdpDateFrom.Text, out dateTime))
                {
                    aclPerson.DateFrom = dateTime;
                }
            }
            else
                aclPerson.DateFrom = null;

            if (_tbdpDateTo.Text != string.Empty)
            {
                DateTime dateTime;
                if (DateTime.TryParse(_tbdpDateTo.Text, out dateTime))
                {
                    aclPerson.DateTo = dateTime;
                }
            }
            else
                aclPerson.DateTo = null;
        }

        private bool GetValues(ACLPerson aclPerson)
        {
            if (!ControlValues())
            {
                return false;
            }

            if (_actAccessControlLists.Objects.Count == 0)
            {
                return false;
            }

            try
            {
                GetValues(aclPerson, _actAccessControlLists.Objects[0] as AccessControlList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetValues(out IList<ACLPerson> aclPersons)
        {
            aclPersons = null;

            if (!ControlValues())
            {
                return false;
            }

            try
            {
                aclPersons = new List<ACLPerson>();

                foreach (var obj in _actAccessControlLists.Objects)
                {
                    var aclPerson = new ACLPerson();
                    GetValues(aclPerson, obj as AccessControlList);
                    aclPersons.Add(aclPerson);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void _bCreate1_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            IList<ACLPerson> aclPersons;

            if (!ControlValues())
            {
                return;
            }

            if (!GetValues(out aclPersons))
                return;

            if (!Dialog.Question(GetString("QuestionInsertACLPerson")))
                return;

            DateTime? dateTimeTo = null;
            if (_tbdpDateTo.Text != string.Empty)
            {
                DateTime dateTime;
                if (DateTime.TryParse(_tbdpDateTo.Text, out dateTime))
                    dateTimeTo = dateTime;
            }

            if ((dateTimeTo != null && _actPerson != null && _actPerson.EmploymentEndDate != null &&
                dateTimeTo.Value.CompareTo(_actPerson.EmploymentEndDate) > 0) ||
                (dateTimeTo == null && _actPerson != null && _actPerson.EmploymentEndDate != null))
            {
                if (!Dialog.WarningQuestion(GetString("QuestionUpdateACLDateTo")))
                    return;
            }

            CreateAclPerson(aclPersons);
        }

        private void CreateAclPerson(IList<ACLPerson> aclPersons)
        {
            SafeThread<IList<ACLPerson>>.StartThread(DoCreateAclPerson, aclPersons);
        }

        private void DoCreateAclPerson(IList<ACLPerson> aclPersons)
        {
            if (aclPersons != null && aclPersons.Count > 0)
            {
                foreach (var actAclPerson in aclPersons)
                {
                    Exception error;
                    var aclPerson = actAclPerson;
                    Plugin.MainServerProvider.ACLPersons.Insert(ref aclPerson, out error);
                    if (error != null)
                    {
                        if (error is AccessDeniedException)
                        {
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertAccessDenied"));
                        }
                        else if (error is SqlUniqueException)
                        {
                            Dialog.Error(GetString("ErrorUsedAccessControlListPerson"));
                        }
                        else
                        {
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertFailed"));
                        }

                        ShowACLPersons();
                        return;
                    }
                }

                ResetValues();
                ShowACLPersons();
            }
        }

        private void ResetValues()
        {
            _actAccessControlLists = null;

            if (_parentControl == null)
                return;

            _parentControl.Invoke(new DVoid2Void(
                delegate
                {
                    RefreshAccessControlList();
                    _tbdpDateFrom.Value = DateTime.Now.Date;
                    _tbdpDateTo.Value = null;
                }));
        }

        private void DeleteAlcPerson(ICollection<ACLPerson> aclPerson)
        {
            SafeThread<ACLPerson>.StartThread(DoDeleteAlcPerson, aclPerson);
        }

        private void DoDeleteAlcPerson(ICollection<ACLPerson> aclPersons)
        {
            if (aclPersons != null)
            {
                foreach (var aclPerson in aclPersons)
                {
                    if (!aclPerson.Compare(_editingACLPerson))
                    {
                        Exception error;
                        Plugin.MainServerProvider.ACLPersons.Delete(aclPerson, out error);

                        if (error != null)
                        {
                            if (error is SqlDeleteReferenceConstraintException)
                                Dialog.Error(
                                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteRowInRelationship"));
                            else
                                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteFailed"));
                        }
                        else
                        {
                            ShowACLPersons();
                        }
                    }
                    else
                    {
                        Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorDeleteEditing"));
                    }
                }
            }
        }

        private void SetValues()
        {
            if (_editingACLPerson != null)
            {
                _actAccessControlLists = new ListOfObjects();
                _actAccessControlLists.Objects.Add(_editingACLPerson.AccessControlList);

                if (_parentControl == null)
                    return;

                _parentControl.Invoke(new DVoid2Void(
                    delegate
                    {
                        RefreshAccessControlList();

                        if (_editingACLPerson.DateFrom != null)
                            _tbdpDateFrom.Value = _editingACLPerson.DateFrom.Value;
                        else
                            _tbdpDateFrom.Value = null;

                        if (_editingACLPerson.DateTo != null)
                            _tbdpDateTo.Value = _editingACLPerson.DateTo.Value;
                        else
                            _tbdpDateTo.Value = null;
                    }));
            }
        }

        private void _bCancelEdit_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            _editingACLPerson = null;

            if (_parentControl != null)
            {
                _parentControl.Invoke(new DVoid2Void(
                    delegate
                    {
                        _bUpdate.Visible = false;
                        _bCreate1.Visible = true;
                        _bCancelEdit.Visible = false;
                    }));
            }

            ResetValues();
        }

        private void _bUpdate_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (!GetValues(_editingACLPerson))
                return;

            if (!Dialog.Question(GetString("QuestionUpdateACLPerson")))
                return;

            if ((_editingACLPerson.DateTo != null && _editingACLPerson.Person != null && _editingACLPerson.Person.EmploymentEndDate != null &&
                _editingACLPerson.DateTo.Value.CompareTo(_editingACLPerson.Person.EmploymentEndDate) > 0) ||
                (_editingACLPerson.DateTo == null && _editingACLPerson.Person != null && _editingACLPerson.Person.EmploymentEndDate != null))
            {
                if (!Dialog.WarningQuestion(GetString("QuestionUpdateACLDateTo")))
                    return;
            }

            UpdateAclPerson();
        }

        private void UpdateAclPerson()
        {
            SafeThread.StartThread(DoUpdateAclPerson);
        }

        private void DoUpdateAclPerson()
        {
            Exception error;
            Plugin.MainServerProvider.ACLPersons.Update(_editingACLPerson, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditAccessDenied"));
                }
                else if (error is SqlUniqueException)
                {
                    Dialog.Error(GetString("ErrorUsedAccessControlListPerson"));
                }
                else if (error is IncoherentDataException)
                {
                    if (Dialog.WarningQuestion(CgpClient.Singleton.LocalizationHelper.GetString("QuestionLoadActualData")))
                    {
                        _editingACLPerson = Plugin.MainServerProvider.ACLPersons.GetObjectForEdit(_editingACLPerson.IdACLPerson, out error);
                        SetValues();
                    }
                    else
                    {
                        _editingACLPerson = Plugin.MainServerProvider.ACLPersons.GetObjectForEdit(_editingACLPerson.IdACLPerson, out error);
                    }
                }
                else
                {
                    Dialog.Error(
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditFailed") +
                        ": " + error.Message);
                }

                return;
            }

            Plugin.MainServerProvider.ACLPersons.EditEnd(_editingACLPerson);
            _bCancelEdit_Click(null, null);
            ResetValues();
            ShowACLPersons();
        }

        private void DoAfterACCreated(object newAccessControlList)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterACCreated), newAccessControlList);
            }
            else
            {
                if (newAccessControlList is AccessControlList)
                {
                    _actAccessControlLists = new ListOfObjects();
                    _actAccessControlLists.Objects.Add(newAccessControlList as AccessControlList);
                    RefreshAccessControlList();
                }
            }
        }

        private static void _dgValues_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _dgValues_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                SafeThread<object>.StartThread(AddACLPerson, e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddACLPerson(object newAccessControlList)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            try
            {
                if (newAccessControlList.GetType() == typeof(AccessControlList))
                {
                    var accessControlList = newAccessControlList as AccessControlList;
                    var aclPerson = 
                        new ACLPerson
                        {
                            AccessControlList = accessControlList,
                            DateFrom = DateTime.Now.Date,
                            DateTo = null,
                            Person = _actPerson
                        };

                    Exception error;
                    Plugin.MainServerProvider.ACLPersons.Insert(ref aclPerson, out error);
                    if (error != null)
                    {
                        if (error is AccessDeniedException)
                        {
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertAccessDenied"));
                        }
                        else if (error is SqlUniqueException)
                        {
                            Dialog.Error(GetString("ErrorUsedAccessControlListPerson"));
                        }
                        else
                        {
                            Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertFailed"));
                        }

                        return;
                    }

                    ShowACLPersons();
                    Plugin.AddToRecentList(newAccessControlList);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _cdgvData.DataGrid,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void _tbmAccessControlList_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyAccesControlList();
            }
            else if (item.Name == "_tsiCreate")
            {
                var accessControlList = new AccessControlList();
                NCASAccessControlListsForm.Singleton.OpenInsertFromEdit(ref accessControlList, DoAfterACCreated);
            }
        }

        public static void SaveAfterInsertWithData(NCASClient ncasClient, Person person, Person clonedPerson)
        {
            if (ncasClient == null || person == null || CgpClient.Singleton.IsConnectionLost(false)) return;

            IList<FilterSettings> filterSettings = new List<FilterSettings>();
            var filterSetting = new FilterSettings(ACLPerson.COLUMNPERSON, clonedPerson, ComparerModes.EQUALL);
            filterSettings.Add(filterSetting);

            Exception error;
            var aclPersons = ncasClient.MainServerProvider.ACLPersons.SelectByCriteria(filterSettings, out error);

            if (aclPersons != null && aclPersons.Count > 0)
            {
                foreach (var aclPerson in aclPersons)
                {
                    if (aclPerson != null)
                    {
                        var newACLPerson = 
                            new ACLPerson
                            {
                                Person = person,
                                AccessControlList = aclPerson.AccessControlList,
                                DateFrom = aclPerson.DateFrom,
                                DateTo = aclPerson.DateTo
                            };

                        ncasClient.MainServerProvider.ACLPersons.Insert(ref newACLPerson, out error);
                    }
                }
            }
        }

        #region ICgpDataGridView Members

        public void EditClick()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) 
                return;

            if (_bindingSource != null && _bindingSource.Count > 0)
            {
                _editingACLPerson = _bindingSource[_bindingSource.Position] as ACLPerson;

                if (_editingACLPerson != null)
                {
                    Exception error;
                    _editingACLPerson = Plugin.MainServerProvider.ACLPersons.GetObjectForEdit(_editingACLPerson.IdACLPerson, out error);

                    if (_editingACLPerson != null)
                    {
                        SetValues();
                        _bCreate1.Visible = false;
                        _bUpdate.Visible = true;
                        _bCancelEdit.Visible = true;
                    }
                }
            }
        }

        public virtual void EditClick(ICollection<int> indexes)
        {
        }

        public void DeleteClick()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) 
                return;

            if (_bindingSource != null && _bindingSource.Count > 0)
            {
                var aclPersons = new LinkedList<ACLPerson>();

                foreach (var selectedRow in _cdgvData.DataGrid.SelectedRows)
                {
                    aclPersons.AddLast(_bindingSource[((DataGridViewRow)selectedRow).Index] as ACLPerson);
                }

                DeleteAlcPerson(aclPersons);
            }
        }

        public virtual void DeleteClick(ICollection<int> indexes)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (_bindingSource == null
                || _bindingSource.Count == 0)
                return;

            var items =
                indexes.Select(index => (IShortObject) (new AccessControlListShort(((ACLPerson) _bindingSource.List[index]).AccessControlList)))
                    .ToList();

            var dialog = new DeleteDataGridItemsDialog(
                Plugin.GetPluginObjectsImages(),
                items,
                CgpClient.Singleton.LocalizationHelper)
            {
                SelectItem = items.FirstOrDefault(item => item.Id.Equals((((ACLPerson)_bindingSource.Current).AccessControlList.IdAccessControlList)))
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var aclPersons = new LinkedList<ACLPerson>();

                if (dialog.DeleteAll)
                {
                    foreach (int index in indexes)
                    {
                        if (_bindingSource.Count > index)
                            aclPersons.AddLast(_bindingSource[index] as ACLPerson);
                    }
                }
                else
                {
                    foreach (var item in dialog.SelectedItems)
                    {
                        var aclPerson =
                            _bindingSource.List.Cast<ACLPerson>()
                                .FirstOrDefault(person => person.AccessControlList.IdAccessControlList.Equals(item.Id));

                        aclPersons.AddLast(aclPerson);
                    }
                }

                DeleteAlcPerson(aclPersons);
            }
        }

        public void InsertClick()
        {
        }

        #endregion
    }
}
