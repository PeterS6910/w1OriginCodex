using Cgp.Components;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Components;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;

namespace Contal.Cgp.Client
{
    public abstract class ACgpTableForm<T, K> : ACgpFullscreenForm, ICgpTableForm, ICgpDataGridView
        where T : AOrmObject
    {
        private BindingSource _bindingSource = null;
        protected IList<FilterSettings> _filterSettings = new List<FilterSettings>();
        private bool _changeFilerValues = false;
        private bool _runShowData = false;
        private ACgpEditForm<T> _formInsert = null;
        private Dictionary<IdAndObjectType, ACgpEditForm<T>> _openedEdit = new Dictionary<IdAndObjectType, ACgpEditForm<T>>();
        private const int _minScreenHeight = 900;

        public ACgpTableForm()
        {
            RegisterToMain();
            FormOnEnter += new Action<Form>(Form_Enter);
        }

        public bool IsMyEditForm(ICgpEditForm editForm)
        {
            foreach (var aCgpEditForm in _openedEdit.Values)
            {
                if (aCgpEditForm == editForm)
                {
                    return true;
                }
            }

            return false;
        }

        public override void CallEscape()
        {
            Close();
        }

        protected void IntitDoubleBufferGrid(DataGridView dataGridView)
        {
            try
            {
                Type dgvType = dataGridView.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dataGridView, true, null);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private bool _loadDataOnFormEnter = true;        

        /// <summary>
        /// Property for enable or disable loading data on form enter
        /// </summary>
        protected bool LoadDataOnFormEnter
        {
            get { return _loadDataOnFormEnter; }
            set { _loadDataOnFormEnter = value; }
        }

        public void RefreshData()
        {
            Form_Enter(this);
        }

        private bool _formIsOpened = false;

        private void Form_Enter(Form form)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                if (_loadDataOnFormEnter)
                {
                    ShowData();
                }
                else
                {
                    if (_formIsOpened)
                    {
                        WarningNotActualData();
                    }
                    else
                    {
                        RemoveDataSource();
                        _formIsOpened = true;
                    }
                }
            }
            else
            {
                RemoveDataSource();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterEvents();
            _eventsRegistered = false;

            base.OnFormClosing(e);
            _formIsOpened = false;
        }

        protected virtual void WarningNotActualData()
        {
        }

        protected void SuspendShowData()
        {
            _runShowData = true;
        }

        protected void ResumeShowData()
        {
            _runShowData = false;
        }

        protected void ShowData()
        {
            if (CgpClientMainForm.Singleton.IsClosing) return;
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (_runShowData) return;

            _runShowData = true;
            CgpClientMainForm.Singleton.StartProgress(this);
            SafeThread.StartThread(LoadTable);
        }

        private void LoadTable()
        {
            lock (_lockPrepareIdToDelete)
            {
            }

            try
            {
                UpdateGridView(GetData());

                if (!_eventsRegistered)
                {
                    RegisterEvents();
                    _eventsRegistered = true;
                }
            }
            catch (Exception error)
            {
                if (error is AccessDeniedException)
                {
                    Dialog.Error(GetString("ErrorLoadTableAccessDenied"));
                }
                else
                {
                    HandledExceptionAdapter.Examine(error);
                    Dialog.Error(GetString("ErrorLoadTable"));
                }
            }
            finally
            {
                _runShowData = false;
                CgpClientMainForm.Singleton.StopProgress(this);
            }
        }

        protected virtual void RegisterEvents()
        {
        }

        protected virtual void UnregisterEvents()
        {
            
        }

        protected virtual bool SortEnabled()
        {
            return true;
        }

        protected abstract ICollection<K> GetData();

        private void UpdateGridView(ICollection<K> data)
        {
            int lastPosition = 0;
            if (_bindingSource != null && _bindingSource.Count != 0)
                lastPosition = _bindingSource.Position;
            _bindingSource = new BindingSource();

            if (SortEnabled())
                _bindingSource.DataSource = new SortableBindingList<K>(data);
            else
                _bindingSource.DataSource = data;

            if (lastPosition != 0) _bindingSource.Position = lastPosition;
            _bindingSource.AllowNew = false;

            DoModifyGridView();
        }
        private void DoModifyGridView()
        {
            if (this is EventlogsForm)
            {
                ModifyGridView(_bindingSource);
            }
            else
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new DVoid2Void(DoModifyGridView));
                }
                else
                {
                    ModifyGridView(_bindingSource);
                }
            }
        }

        protected abstract void ModifyGridView(BindingSource bindingSource);

        protected void RemoveDataSource()
        {
            RemoveGridView();
            _bindingSource = null;
        }

        protected abstract void RemoveGridView();

        #region Connection

        protected override bool VerifySources()
        {
            return null != CgpClient.Singleton.MainServerProvider;
        }

        #endregion

        protected void Insert()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (!HasAccessInsert())
            {
                Dialog.Error(GetString("ErrorInsertAccessDenied"));
                return;
            }

            try
            {
                if (_formInsert == null)
                {
                    T obj = (T)Activator.CreateInstance(typeof(T));
                    ACgpEditForm<T> formInsert = CreateEditForm(obj, ShowOptionsEditForm.Insert);
                    formInsert.Show();
                }
                else
                {
                    BringWindowToFront(_formInsert);
                }

            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(GetString("ErrorInsertFailed"));
            }
        }

        public void InsertWithData(object obj)
        {
            InsertWithDataAndFilter(obj, null, null);
        }

        public void InsertWithData(object obj, Action<object> doAfterInsertedOrEdited)
        {
            InsertWithDataAndFilter(obj, null, doAfterInsertedOrEdited);
        }

        public void InsertWithDataAndFilter(object obj, Dictionary<string, bool> filter)
        {
            InsertWithDataAndFilter(obj, filter, null);
        }

        public void InsertWithDataAndFilter(object obj, Dictionary<string, bool> filter, Action<object> doAfterInsertedOrEdited)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, Dictionary<string, bool>, Action<object>>(InsertWithDataAndFilter), obj, filter, doAfterInsertedOrEdited);
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;

                if (!HasAccessInsert())
                {
                    Dialog.Error(GetString("ErrorInsertAccessDenied"));
                    return;
                }

                try
                {
                    T objectForEdit = GetObjectIfExists(obj);

                    if (objectForEdit != null)
                    {
                        if (QuestionOpenEditFormIfObjectAlreadyExists())
                            OpenEditForm(objectForEdit, doAfterInsertedOrEdited);
                    }
                    else
                    {
                        if (_formInsert == null)
                        {
                            T newObj = (T)Activator.CreateInstance(typeof(T));
                            ACgpEditForm<T> formInsert = CreateEditForm(newObj, ShowOptionsEditForm.InsertWithData);

                            if (filter == null || filter.Count == 0)
                                formInsert.SetValuesInsertFromObj(obj);
                            else
                                formInsert.SetValuesInsertFromObjWithFilter(obj, filter);

                            formInsert.DoAfterInserted = doAfterInsertedOrEdited;
                            formInsert.Show();
                        }
                        else
                        {
                            BringWindowToFront(_formInsert);
                        }
                    }

                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    Dialog.Error(GetString("ErrorInsertFailed"));
                }
            }
        }

        protected virtual T GetObjectIfExists(object obj)
        {
            return null;
        }

        protected virtual bool QuestionOpenEditFormIfObjectAlreadyExists()
        {
            return Dialog.Question(GetString("QuestionOpenEditFormIfObjectAlreadyExists"));
        }

        public bool OpenInsertDialg(ref T outObj)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return false;

            if (!HasAccessInsert())
            {
                Dialog.Error(GetString("ErrorInsertAccessDenied"));
                return false;
            }

            try
            {
                ACgpEditForm<T> formInsert = CreateEditForm(outObj, ShowOptionsEditForm.InsertDialog);
                return formInsert.ShowInsertDialog(ref outObj);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(GetString("ErrorInsertFailed"));
                return false;
            }
        }

        public void OpenInsertFromEdit(ref T outObj, Action<object> doAfterInserted)
        {
            OpenInsertFromEdit(ref outObj, doAfterInserted, null);
        }

        public void OpenInsertFromEdit(ref T outObj, Action<object> doAfterInserted, DVoid2Void doAfterInsertClosed)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (!HasAccessInsert())
            {
                Dialog.Error(GetString("ErrorInsertAccessDenied"));
                return;
            }

            try
            {
                if (_formInsert == null)
                {
                    ACgpEditForm<T> formInsert = CreateEditForm(outObj, ShowOptionsEditForm.Insert);
                    formInsert.DoAfterInserted = doAfterInserted;
                    formInsert.DoAfterInsertClosed = doAfterInsertClosed;
                    formInsert.Show();
                }
                else
                {
                    _formInsert.DoAfterInserted = doAfterInserted;
                    _formInsert.DoAfterInsertClosed = doAfterInsertClosed;
                    BringWindowToFront(_formInsert);
                }

            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(GetString("ErrorInsertFailed"));
            }
        }

        protected abstract ACgpEditForm<T> CreateEditForm(T obj, ShowOptionsEditForm showOption);

        public void BeforeInsert(ACgpEditForm<T> formInsert)
        {
            _formInsert = formInsert;
        }

        public void AfterInsert()
        {
            _formInsert = null;
        }

        protected void Edit_Click()
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            Edit();
        }

        protected abstract T GetObjectForEdit(K listObj, out bool editEnabled);
        protected abstract T GetFromShort(K listObj);
        protected abstract T GetById(object idObj);

        private void Edit()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            bool editEnabled;
            T obj = GetObjectForEdit((K)_bindingSource.List[_bindingSource.Position], out editEnabled);
            if (obj != null)
            {
                OpenEditForm(obj, editEnabled);
            }
            else
            {
                ShowData();
            }
        }

        protected void EditFromPosition(int position)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            bool editEnabled;
            T obj = GetObjectForEdit((K)_bindingSource.List[position], out editEnabled);
            if (obj != null)
                OpenEditForm(obj, editEnabled);
        }

        public Form OpenEditForm(T editObj)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return null;

            if (!HasAccessView(editObj))
            {
                Dialog.Error(GetString("ErrorEditAccessDenied"));
                return null;
            }

            try
            {
                ACgpEditForm<T> formEdit = null;
                if (!OpenExistEditForm(editObj, out formEdit))
                {
                    formEdit = CreateEditForm(editObj, ShowOptionsEditForm.Edit);
                    bool allowEdit;
                    formEdit.ReloadEditingObject(out allowEdit);
                    formEdit.SetAllowEdit(allowEdit);
                    formEdit.Show();
                }

                SetWindowStateByScreenResolution(formEdit);

                return formEdit;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(GetString("ErrorEditFailed") + ": " + ex.Message);
            }
            return null;
        }

        public Form OpenEditForm(T editObj, Action<object> doAfterEdited)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return null;

            if (!HasAccessView(editObj))
            {
                Dialog.Error(GetString("ErrorEditAccessDenied"));
                return null;
            }

            try
            {
                ACgpEditForm<T> formEdit = null;
                if (!OpenExistEditForm(editObj, out formEdit))
                {
                    formEdit = CreateEditForm(editObj, ShowOptionsEditForm.Edit);
                    bool allowEdit;
                    formEdit.ReloadEditingObject(out allowEdit);
                    formEdit.DoAfterEdited = doAfterEdited;
                    formEdit.SetAllowEdit(allowEdit);
                    formEdit.Show();
                }

                SetWindowStateByScreenResolution(formEdit);

                return formEdit;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(GetString("ErrorEditFailed") + ": " + ex.Message);
            }
            return null;
        }

        private void SetWindowStateByScreenResolution(ACgpEditForm<T> form)
        {
            /* Currently disabled because of #1171
            if (Screen.FromControl(CgpClientMainForm.Singleton).WorkingArea.Height < _minScreenHeight)
            {
                form.UndockWindow();
                form.WindowState = FormWindowState.Maximized;
            }*/
        }

        public virtual bool HasAccessView(T ormObject)
        {
            return false;
        }

        public virtual bool HasAccessInsert()
        {
            return false;
        }

        public void OpenEditDialog(T editObj)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            if (!HasAccessView(editObj))
            {
                Dialog.Error(GetString("ErrorEditAccessDenied"));
                return;
            }

            try
            {
                ACgpEditForm<T> formEdit = CreateEditForm(editObj, ShowOptionsEditForm.Edit);
                formEdit.MdiParent = null;

                bool allowEdit;
                formEdit.ReloadEditingObject(out allowEdit);
                formEdit.SetAllowEdit(allowEdit);
                if (formEdit.FormOnEnter != null)
                {
                    formEdit.FormOnEnter(formEdit);
                }

                formEdit.ShowDialog();
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(GetString("ErrorEditFailed") + ": " + ex.Message);
            }
        }

        public Form OpenEditForm(T editObj, bool editEnabled)
        {
            return OpenEditForm(editObj, editEnabled, true);
        }

        public Form OpenEditForm(T editObj, bool editEnabled, bool show)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return null;

            if (!HasAccessView(editObj))
            {
                Dialog.Error(GetString("ErrorEditAccessDenied"));
                return null;
            }

            try
            {
                ACgpEditForm<T> formEdit = null;
                if (!OpenExistEditForm(editObj, out formEdit, show))
                {
                    if (editEnabled)
                    {
                        formEdit = CreateEditForm(editObj, ShowOptionsEditForm.Edit);
                    }
                    else
                    {
                        formEdit = CreateEditForm(editObj, ShowOptionsEditForm.View);
                    }

                    formEdit.ReloadEditingObject(out editEnabled);
                    formEdit.SetAllowEdit(editEnabled);
                    if (show)
                        formEdit.Show();
                    else
                        BeforeEdit(formEdit, editObj);
                }

                SetWindowStateByScreenResolution(formEdit);

                return formEdit;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(GetString("ErrorEditFailed") + ": " + ex.Message);
            }
            return null;
        }

        private bool OpenExistEditForm(T obj, out ACgpEditForm<T> editForm)
        {
            return OpenExistEditForm(obj, out editForm, true);
        }

        private bool OpenExistEditForm(T obj, out ACgpEditForm<T> editForm, bool show)
        {
            editForm = null;
            foreach (KeyValuePair<IdAndObjectType, ACgpEditForm<T>> keyvaluepair in _openedEdit)
            {
                //if (Compare(keyvaluepair.Key, obj))
                if (keyvaluepair.Key.Equals(new IdAndObjectType(obj.GetId(), obj.GetObjectType())))
                {
                    if (show)
                    {
                        if (keyvaluepair.Value.Visible)
                            BringWindowToFront(keyvaluepair.Value);
                        else
                            keyvaluepair.Value.Show();
                    }
                    editForm = keyvaluepair.Value;
                    return true;
                }
            }
            return false;
        }

        protected abstract bool Compare(T obj1, T obj2);

        private void RemoveFromOpenedEdit(T obj)
        {
            foreach (KeyValuePair<IdAndObjectType, ACgpEditForm<T>> keyvaluepair in _openedEdit)
            {
                //if (Compare(keyvaluepair.Key, obj))
                if (keyvaluepair.Key.Equals(new IdAndObjectType(obj.GetId(), obj.GetObjectType())))
                {
                    _openedEdit.Remove(keyvaluepair.Key);
                    return;
                }
            }
        }

        public void BeforeEdit(ACgpEditForm<T> formEdit, T obj)
        {
            CgpClientMainForm.Singleton.AddToRecentList(obj);

            var idAndObjectType = new IdAndObjectType(obj.GetId(), obj.GetObjectType());

            if (!_openedEdit.ContainsKey(idAndObjectType))
                _openedEdit.Add(idAndObjectType, formEdit);
        }

        public void AfterEdit(T obj)
        {
            RemoveFromOpenedEdit(obj);
        }

        protected void Delete_Click(ICollection<int> indexes)
        {
            if (_bindingSource == null 
                || _bindingSource.Count == 0)
                return;

            var dialog = new DeleteDataGridItemsDialog(ObjectImageList.Singleton.ClientObjectImages, 
                indexes.Select(index => (IShortObject)_bindingSource.List[index]).ToList(),
                CgpClient.Singleton.LocalizationHelper)
            {
                SelectItem = (IShortObject)_bindingSource.Current
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.DeleteAll)
                {
                    foreach (int index in indexes)
                        Delete((K)_bindingSource.List[index]);
                }
                else
                {
                    foreach (var item in dialog.SelectedItems)
                        Delete((K) item);
                }

                ShowData();
            }
        }

        protected void Delete_Click()
        {
            if (_bindingSource == null 
                || _bindingSource.Count == 0)
                return;

            var objects = new List<IShortObject> {(IShortObject) _bindingSource.List[_bindingSource.Position]};

            var dialog = new DeleteDataGridItemsDialog(ObjectImageList.Singleton.ClientObjectImages, 
                objects, CgpClient.Singleton.LocalizationHelper);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Delete((K) _bindingSource.Current);
                ShowData();
            }
        }

        private void Delete(K obj)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) 
                return;

            try
            {
                T deletedObj = GetFromShort(obj);

                if (deletedObj == null)
                    return;
              
                if (!IsOpenedEdit(deletedObj))
                {
                    DeleteObj(deletedObj);

                    CgpClientMainForm.Singleton.DeleteFromRecentList(deletedObj);
                    //ShowData();
                }
                else
                {
                    string errorMsg = deletedObj.ToString();
                    errorMsg += " ";
                    errorMsg += GetString("ErrorDeleteEditing");
                    Dialog.Error(errorMsg);
                }
            }
            catch (Exception error)
            {
                if (error is SqlDeleteReferenceConstraintException)
                {
                    HandledExceptionAdapter.Examine(error);
                    Dialog.Error(GetString("ErrorDeleteRowInRelationship"));
                }
                else if (error is AccessDeniedException)
                    Dialog.Error(GetString("ErrorDeleteAccessDenied"));
                else if (error is OperationDeniedException)
                    Dialog.Error(GetString("ErrorDeleteSuperAdmin"));
                else if (error is InvalidDeleteOperationException)
                    Dialog.Error(GetString("ErrorDeleteLogedLogin"));
                else
                {
                    HandledExceptionAdapter.Examine(error);
                    Dialog.Error(GetString("ErrorDeleteFailed"));
                }
            }
        }

        protected virtual bool? ShowCustomQuestionDeleteConfirm(T deletedObj)
        {
            return null;
        }

        private bool DeleteObject(T deletedObj, out AOrmObject aOrmObject, out string errorMsg)
        {
            errorMsg = string.Empty;
            aOrmObject = null;
            string objName = string.Empty;
            if (CgpClient.Singleton.IsConnectionLost(true)) return false;

            try
            {
                if (deletedObj == null)
                {
                    errorMsg = GetString("DeleteError");
                    return false;
                }
                aOrmObject = deletedObj;
                objName = deletedObj + " ";
                string msg;
                if (DeleteObjectNotAllowed(deletedObj, out msg))
                {
                    errorMsg = msg;
                    return false;
                }
                DeleteObj(deletedObj);
                return true;
            }
            catch (Exception error)
            {
                if (error is SqlDeleteReferenceConstraintException)
                {
                    HandledExceptionAdapter.Examine(error);
                    errorMsg = objName + GetString("ErrorDeleteRowInRelationship");
                }
                else if (error is AccessDeniedException)
                    errorMsg = objName + GetString("ErrorDeleteAccessDenied");
                else if (error is OperationDeniedException)
                    errorMsg = objName + GetString("ErrorDeleteSuperAdmin");
                else if (error is InvalidDeleteOperationException)
                    errorMsg = objName + GetString("ErrorDeleteLogedLogin");
                else
                {
                    HandledExceptionAdapter.Examine(error);
                    errorMsg = objName + GetString("ErrorDeleteFailed");
                }
                return false;
            }
        }

        protected virtual bool DeleteObjectNotAllowed(T obj, out string errorMsg)
        {
            errorMsg = string.Empty;
            return false;
        }

        protected void MultiselectDeleteClic(DataGridViewSelectedRowCollection selectedRows)
        {
            SafeThread<DataGridViewSelectedRowCollection>.StartThread(DoMultiselectDeleteClic, selectedRows);
        }

        private object _lockMultiSelectDelete = new object();
        private bool _isRunningMultiSelectDelete = false;
        private object _lockPrepareIdToDelete = new object();

        private bool _eventsRegistered;

        private void DoMultiselectDeleteClic(DataGridViewSelectedRowCollection selectedRows)
        {
            lock (_lockMultiSelectDelete)
            {
                if (_isRunningMultiSelectDelete)
                {
                    Dialog.Error(GetString("ErrorMultiSelectDeleteIsAlreadyRunning"));
                    return;
                }

                _isRunningMultiSelectDelete = true;
            }

            IList<T> listIdToDelete = new List<T>();
            bool? resultwCustomQuestionDeleteConfirm = null;
            lock (_lockPrepareIdToDelete)
            {
                for (int i = 0; i < selectedRows.Count; i++)
                {
                    T deletedObj = GetFromShort((K)_bindingSource.List[selectedRows[i].Index]);
                    listIdToDelete.Add(deletedObj);

                    if (resultwCustomQuestionDeleteConfirm == null)
                    {
                        resultwCustomQuestionDeleteConfirm = ShowCustomQuestionDeleteConfirm(deletedObj);
                    }
                }
            }

            if (resultwCustomQuestionDeleteConfirm != null)
            {
                if (!resultwCustomQuestionDeleteConfirm.Value)
                {
                    _isRunningMultiSelectDelete = false;
                    return;
                }
            }
            else if (!Dialog.Question(GetString("QuestionMultiselectDeleteConfirm")))
            {
                _isRunningMultiSelectDelete = false;
                return;
            }

            string errorMsg;
            Dictionary<AOrmObject, string> multiSelectDeleteResults = new Dictionary<AOrmObject, string>();
            try
            {
                foreach (T delObj in listIdToDelete)
                {
                    AOrmObject aOrmObject;
                    bool deleted = DeleteObject(delObj, out aOrmObject, out errorMsg);
                    if (!deleted && aOrmObject != null)
                    {
                        multiSelectDeleteResults.Add(aOrmObject, errorMsg);
                    }
                }

                ShowData();
                if (multiSelectDeleteResults != null && multiSelectDeleteResults.Count > 0)
                {
                    Invoke(new Action<Dictionary<AOrmObject, string>>(ShowMultiSelectDeleteErrorResults), multiSelectDeleteResults);
                }
                else
                {
                    Dialog.Info(GetString("InfoAllSelectedObjetsDeleted"));
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                _isRunningMultiSelectDelete = false;
            }
        }

        private void ShowMultiSelectDeleteErrorResults(Dictionary<AOrmObject, string> multiSelectDeleteResults)
        {
            MultiselectDeleteErrorResultsForm multiselectDeleteErrorResultsForm = new MultiselectDeleteErrorResultsForm();
            multiselectDeleteErrorResultsForm.ShowDialog(multiSelectDeleteResults);
        }

        protected abstract void DeleteObj(T obj);
        protected abstract void DeleteObjById(object idObj);

        private bool IsOpenedEdit(T obj)
        {
            foreach (KeyValuePair<IdAndObjectType, ACgpEditForm<T>> keyvaluepair in _openedEdit)
            {
                //if (Compare(keyvaluepair.Key, obj))
                if (keyvaluepair.Key.Equals(new IdAndObjectType(obj.GetId(), obj.GetObjectType())))
                {
                    return true;
                }
            }

            return false;
        }

        protected void RunFilter()
        {
            if (_changeFilerValues)
            {
                if (CheckFilterValues())
                {
                    _filterSettings.Clear();
                    SetFilterSettings();
                    _changeFilerValues = false;
                    ShowData();
                }
            }
        }

        public bool IsChangedFilerValues { get { return _changeFilerValues; } }

        protected virtual bool CheckFilterValues()
        {
            return true;
        }

        protected abstract void SetFilterSettings();

        protected void FilterClear_Click()
        {
            _filterSettings.Clear();

            ClearFilterEdits();

            ShowData();
            _changeFilerValues = false;
        }

        protected abstract void ClearFilterEdits();

        protected virtual void FilterValueChanged(object sender, EventArgs e)
        {
            _changeFilerValues = true;
        }

        protected virtual void FilterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                RunFilter();
            }
        }

        protected bool GetObjectFromRow(int index, ref T obj)
        {
            if (_bindingSource == null || _bindingSource.Count < index) return false;

            obj = GetFromShort((K)_bindingSource.List[index]);
            return true;
        }

        protected bool GetObjectFromActualRow(ref T obj)
        {
            if (_bindingSource == null || _bindingSource.Count == 0)
            {
                return false;
            }

            obj = GetFromShort((K)_bindingSource.List[_bindingSource.Position]);
            return true;
        }

        protected void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView != null)
            {
                if (gridView.Columns.Contains(columnName))
                    gridView.Columns[columnName].Visible = false;
            }
        }

        protected void ShowColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView != null)
            {
                if (gridView.Columns.Contains(columnName))
                    gridView.Columns[columnName].Visible = true;
            }
        }

        protected void GetWidthColumn(DataGridView gridView, string columnName, ref int width)
        {
            if (gridView.Columns.Count > 0)
            {
                try
                {
                    DataGridViewColumn dgvc = gridView.Columns[columnName];
                    if (dgvc != null)
                        width = dgvc.Width;
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        protected void SetWidthColumn(DataGridView gridView, string columnName, int width)
        {
            if (gridView.Columns.Count > 0)
            {
                try
                {
                    DataGridViewColumn dgvc = gridView.Columns[columnName];
                    if (dgvc != null)
                        dgvc.Width = width;
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        public void CloseEditedForm(T editObj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<T>(CloseEditedForm), editObj);
            }
            else
            {
                try
                {
                    foreach (KeyValuePair<IdAndObjectType, ACgpEditForm<T>> keyvaluepair in _openedEdit)
                    {
                        //if (Compare(keyvaluepair.Key, editObj))
                        if (keyvaluepair.Key.Equals(new IdAndObjectType(editObj.GetId(), editObj.GetObjectType())))
                        {
                            keyvaluepair.Value.Close();
                            return;
                        }
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    Dialog.Error(GetString("ErrorCloseFailed"));
                }
            }
        }

        public void CloseEditedFormById(object idObj)
        {
            try
            {
                foreach (KeyValuePair<IdAndObjectType, ACgpEditForm<T>> keyvaluepair in _openedEdit)
                {

                    if (keyvaluepair.Key.ObjectType == ObjectType.Login)
                    {
                        //if ((string)keyvaluepair.Key.GetId() == (string)idObj)
                        if ((string)keyvaluepair.Key.Id == (string)idObj)
                        {
                            CloseForm(keyvaluepair.Value);
                            return;
                        }
                    }
                    else
                    {
                        //if ((Guid)keyvaluepair.Key.GetId() == (Guid)idObj)
                        if ((Guid)keyvaluepair.Key.Id == (Guid)idObj)
                        {
                            CloseForm(keyvaluepair.Value);
                            return;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void CloseForm(ACgpEditForm<T> form)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ACgpEditForm<T>>(CloseForm), form);
            }
            else
            {
                form.Close();
            }
        }

        // Moves the Window to the front of the z-order.
        // If the Window is minimized, the Window is moved to the front and displayed as Normal.
        private void BringWindowToFront(Form aForm)
        {
            if (aForm.WindowState == FormWindowState.Minimized)
            {
                aForm.WindowState = FormWindowState.Normal;
            }
            aForm.BringToFront();
        }


        #region ICgpTableForm Members

        public void TableUnregisterEvents()
        {
            
        }

        public virtual bool HasAccessView()
        {
            return false;
        }

        #endregion

        #region ICgpTableForm Members


        public virtual void EditClick()
        {
            Edit_Click();
        }

        public virtual void EditClick(ICollection<int> indexes)
        {
            foreach (int index in indexes)
            {
                EditFromPosition(index);
            }
        }

        public virtual void DeleteClick()
        {
            Delete_Click();
        }

        public virtual void DeleteClick(ICollection<int> indexes)
        {
            Delete_Click(indexes);
        }

        public virtual void InsertClick()
        {
            Insert();
        }

        #endregion
    }
}
