using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;

using Cgp.Components;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Components;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;
using Contal.Cgp.Globals;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;

namespace Contal.Cgp.Client.PluginSupport
{
    public abstract class ACgpPluginTableForm<TCgpVisualPlugin, TObject, TShortObject> :
        ACgpPluginFullscreenForm<TCgpVisualPlugin>,
        ICgpTableForm, ICgpDataGridView
        where TCgpVisualPlugin : ICgpVisualPlugin
        where TObject : AOrmObject
    {
        private bool _changeFilerValues;
        private bool _changeRuntimeFilerValues;
        private bool _runShowData;
        private ACgpPluginEditForm<TCgpVisualPlugin, TObject> _formInsert;
        private readonly Dictionary<IdAndObjectType, ACgpPluginEditForm<TCgpVisualPlugin, TObject>> _openedEdit = new Dictionary<IdAndObjectType, ACgpPluginEditForm<TCgpVisualPlugin, TObject>>();
        private readonly ICgpClientMainForm _cgpClientMainForm;
        private const int _minScreenHeight = 900;

        protected ACgpPluginTableForm(
            ICgpClientMainForm cgpClientMainForm,
            TCgpVisualPlugin plugin,
            LocalizationHelper localizationHelper)
            : base(
                cgpClientMainForm,
                plugin,
                localizationHelper)
        {
            FilterSettings = new List<FilterSettings>();
            _cgpClientMainForm = cgpClientMainForm;
            FormOnEnter += Form_Enter;
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

        protected virtual bool SortEnabled()
        {
            return true;
        }

        protected void IntitDoubleBufferGrid(DataGridView dataGridView)
        {
            try
            {
                var dgvType = dataGridView.GetType();
                var pi = dgvType.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dataGridView, true, null);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        protected void ShowData()
        {
            ShowData(false);
        }

        private readonly object _lockShowData = new object();
        private void ShowData(bool runRegisterEvents)
        {
            lock (_lockShowData)
            {
                if (_cgpClientMainForm.MainIsConnectionLost(true)) return;
                if (_runShowData) return;
                _runShowData = true;
                _cgpClientMainForm.StartProgress(this);
                
                SafeThread<bool>.StartThread(
                    LoadTable,
                    runRegisterEvents);
            }
        }

        private void LoadTable(bool runRegisterEvents)
        {
            try
            {
                string checkLicenseErrorMessage;
                if (!CheckLicense(out checkLicenseErrorMessage))
                {
                    BeginInvoke(new DVoid2Void(() =>
                    {
                        Dialog.Error(checkLicenseErrorMessage);
                        Close();
                    }));
                    return;
                }

                UpdateGridView(GetData());

                if (runRegisterEvents)
                    RegisterEvents();
            }
            catch (Exception error)
            {
                if (error is AccessDeniedException)
                {
                    Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorLoadTableAccessDenied"));
                }
                else
                {
                    HandledExceptionAdapter.Examine(error);
                    Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorLoadTable"));
                }
            }
            finally
            {
                _runShowData = false;
                _cgpClientMainForm.StopProgress(this);
            }
        }

        protected virtual bool CheckLicense(out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        protected abstract ICollection<TShortObject> GetData();

        private void UpdateGridView(ICollection<TShortObject> data)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ICollection<TShortObject>>(UpdateGridView), data);
            }
            else
            {
                var lastPosition = 0;

                if (BindingSource != null && BindingSource.Count != 0)
                    lastPosition = BindingSource.Position;

                BindingSource = 
                    new BindingSource
                    {
                        DataSource = 
                            SortEnabled()
                                ? new SortableBindingList<TShortObject>(data)
                                : data,
                        AllowNew = false
                    };

                if (lastPosition != 0)
                    BindingSource.Position = lastPosition;

                ModifyGridView(BindingSource);
                _changeRuntimeFilerValues = false;
            }
        }        

        protected abstract void ModifyGridView(BindingSource bindingSource);

        protected void RemoveDataSource()
        {
            RemoveGridView();
            BindingSource = null;
        }

        protected abstract void RemoveGridView();

        #region Connection

        #endregion

        public virtual bool HasAccessView(TObject ormObject)
        {
            return false;
        }

        public virtual bool HasAccessInsert()
        {
            return false;
        }

        protected void Insert()
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;

            if (!HasAccessInsert())
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertAccessDenied"));
                return;
            }

            try
            {
                if (_formInsert == null)
                {
                    var obj = (TObject)Activator.CreateInstance(typeof(TObject));
                    var formInsert = CreateEditForm(obj, ShowOptionsEditForm.Insert);
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
                Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorInsertFailed"));
            }
        }

        public void InsertWithData(object obj)
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;

            if (!HasAccessInsert())
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertAccessDenied"));
                return;
            }

            try
            {
                if (_formInsert == null)
                {
                    var newObj = (TObject)Activator.CreateInstance(typeof(TObject));
                    var formInsert = CreateEditForm(newObj, ShowOptionsEditForm.InsertWithData);
                    formInsert.SetValuesInsertFromObj(obj);
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
                Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorInsertFailed"));
            }
        }

        public bool OpenInsertDialg(ref TObject outObj)
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return false;

            if (!HasAccessInsert())
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertAccessDenied"));
                return false;
            }

            try
            {
                var formInsert = CreateEditForm(outObj, ShowOptionsEditForm.InsertDialog);
                return formInsert.ShowInsertDialog(ref outObj);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorInsertFailed"));
                return false;
            }
        }

        public void OpenInsertFromEdit(ref TObject outObj, Action<object> doAfterInserted)
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;

            if (!HasAccessInsert())
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertAccessDenied"));
                return;
            }

            try
            {
                if (_formInsert == null)
                {
                    var formInsert = CreateEditForm(outObj, ShowOptionsEditForm.Insert);
                    formInsert.DoAfterInserted = doAfterInserted;
                    formInsert.Show();
                }
                else
                {
                    _formInsert.DoAfterInserted = doAfterInserted;
                    BringWindowToFront(_formInsert);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorInsertFailed"));
            }
        }

        protected abstract ACgpPluginEditForm<TCgpVisualPlugin, TObject> CreateEditForm(TObject obj, ShowOptionsEditForm showOption);

        public void BeforeInsert(ACgpPluginEditForm<TCgpVisualPlugin, TObject> formInsert)
        {
            _formInsert = formInsert;
        }

        public void AfterInsert()
        {
            _formInsert = null;
        }

        protected void GetCurrentObject(ref TObject obj)
        {
            if (BindingSource == null || BindingSource.Count == 0) return;
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;
            obj = GetFromShort((TShortObject)BindingSource.List[BindingSource.Position]);
        }

        protected void GetCurrentShortObject(ref TShortObject obj)
        {
            if (BindingSource == null || BindingSource.Count == 0) return;
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;
            obj = (TShortObject)BindingSource.List[BindingSource.Position];
        }

        protected void Edit_Click()
        {
            if (BindingSource == null || BindingSource.Count == 0) return;
            Edit();
        }

        protected void View_Click()
        {
            if (BindingSource == null || BindingSource.Count == 0) return;
            View();
        }

        protected abstract TObject GetObjectForEdit(TShortObject listObj, out bool editEnabled);
        protected abstract TObject GetFromShort(TShortObject listObj);

        private void Edit()
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;
            bool editEnabled;
            var obj = GetObjectForEdit((TShortObject)BindingSource.List[BindingSource.Position], out editEnabled);
            if (obj != null)
            {
                OpenEditForm(obj, editEnabled);
            }
            else
            {
                ShowData();
            }
        }

        private void View()
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;
            bool editEnabled;
            var obj = GetObjectForEdit((TShortObject)BindingSource.List[BindingSource.Position], out editEnabled);
            if (obj != null)
            {
                OpenEditForm(obj, false);
            }
            else
            {
                ShowData();
            }
        }

        protected void EditFromPosition(int position)
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;
            bool editEnabled;
            var obj = GetObjectForEdit((TShortObject)BindingSource.List[position], out editEnabled);
            if (obj != null)
                OpenEditForm(obj, editEnabled);
        }

        private readonly AutoResetEvent _are = new AutoResetEvent(true); 
        public Form OpenEditForm(TObject editObj)
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true))
                return null;

            if (!HasAccessView(editObj))
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditAccessDenied"));
                return null;
            }

            try
            {
                if (!_are.WaitOne(0))
                    return _formEdit;

                try
                {
                    if (!OpenExistEditForm(editObj, out _formEdit))
                    {
                        _formEdit = CreateEditForm(editObj, ShowOptionsEditForm.Edit);
                        bool allowEdit;
                        _formEdit.ReloadEditingObject(out allowEdit);
                        _formEdit.SetAllowEdit(allowEdit);
                        _formEdit.Show();
                    }
                    else
                    {
                        _are.Set();
                    }
                }
                catch
                {
                    _are.Set();
                }

                return _formEdit;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorEditFailed") + ": " + ex.Message);
            }
            return null;
        }

        public Form OpenEditForm(TObject editObj, bool editEnabled)
        {
            return OpenEditForm(editObj, editEnabled, true);
        }

        ACgpPluginEditForm<TCgpVisualPlugin, TObject> _formEdit;

        public Form OpenEditForm(TObject editObj, bool editEnabled, bool show)
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true))
                return null;

            if (!HasAccessView(editObj))
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorEditAccessDenied"));
                return null;
            }

            try
            {
                if (!_are.WaitOne(0))
                    return _formEdit;

                try
                {
                    if (!OpenExistEditForm(editObj, out _formEdit, show))
                    {
                        _formEdit =
                            CreateEditForm(
                                editObj,
                                    editEnabled
                                        ? ShowOptionsEditForm.Edit
                                        : ShowOptionsEditForm.View);

                        _formEdit.ReloadEditingObject(out editEnabled);
                        _formEdit.SetAllowEdit(editEnabled);

                        if (show)
                            _formEdit.Show();
                        else
                            BeforeEdit(_formEdit, editObj);
                    }
                    else
                    {
                        _are.Set();
                    }
                }
                catch (Exception)
                {
                    _are.Set();
                }

                if (Screen.FromControl(CgpClientMainForm.Singleton).WorkingArea.Height < _minScreenHeight)
                {
                    /* Currently disabled because of #1171
                    _formEdit.UndockWindow();
                    _formEdit.WindowState = FormWindowState.Maximized;
                    */
                }

                return _formEdit;
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorEditFailed") + ": " + ex.Message);
            }
            return null;
        }

        private bool OpenExistEditForm(TObject obj, out ACgpPluginEditForm<TCgpVisualPlugin, TObject> form)
        {
            return OpenExistEditForm(obj, out form, true);
        }

        private bool OpenExistEditForm(TObject obj, out ACgpPluginEditForm<TCgpVisualPlugin, TObject> form, bool show)
        {
            form = null;
            foreach (var keyvaluepair in _openedEdit)
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
                    form = keyvaluepair.Value;
                    return true;
                }
            }
            return false;
        }

        protected abstract bool Compare(TObject obj1, TObject obj2);

        protected abstract bool CombareByGuid(TObject obj, Guid idObj);

        private void RemoveFromOpenedEdit(TObject obj)
        {
            foreach (var keyvaluepair in _openedEdit)
            {
                //if (Compare(keyvaluepair.Key, obj))
                if (keyvaluepair.Key.Equals(new IdAndObjectType(obj.GetId(), obj.GetObjectType())))
                {
                    _openedEdit.Remove(keyvaluepair.Key);
                    return;
                }
            }
        }

        public void BeforeEdit(ACgpPluginEditForm<TCgpVisualPlugin, TObject> formEdit, TObject obj)
        {
            BeforeEdit(formEdit, obj, true);
        }

        public void BeforeEdit(ACgpPluginEditForm<TCgpVisualPlugin, TObject> formEdit, TObject obj, bool editEnable)
        {
            _cgpClientMainForm.AddToRecentList(obj, this, editEnable);

            var idAndObjectType = new IdAndObjectType(obj.GetId(), obj.GetObjectType());

            if (!_openedEdit.ContainsKey(idAndObjectType))
                _openedEdit.Add(idAndObjectType, formEdit);

            _are.Set();
        }

        public void AfterEdit(TObject obj)
        {
            RemoveFromOpenedEdit(obj);
        }

        protected void Delete_Click()
        {
            if (BindingSource == null 
                || BindingSource.Count == 0) 
                return;

            var objects = new List<IShortObject> { (IShortObject)BindingSource.List[BindingSource.Position] };

            var dialog = new DeleteDataGridItemsDialog(Plugin.GetPluginObjectsImages(),
                objects, CgpClient.Singleton.LocalizationHelper);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Delete((TShortObject)BindingSource.Current);
            }
        }

        protected void Delete_Click(ICollection<int> indexes)
        {
            if (BindingSource == null 
                || BindingSource.Count == 0) 
                return;

            var dialog = new DeleteDataGridItemsDialog(Plugin.GetPluginObjectsImages(),
                indexes.Select(index => (IShortObject)BindingSource.List[index]).ToList(),
                CgpClient.Singleton.LocalizationHelper)
            {
                SelectItem = (IShortObject)BindingSource.Current
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.DeleteAll)
                {
                    foreach (int index in indexes)
                        Delete((TShortObject)BindingSource.List[index]);
                }
                else
                {
                    foreach (var item in dialog.SelectedItems)
                        Delete((TShortObject)BindingSource.List.Cast<IShortObject>().FirstOrDefault(o => o.Id.Equals(item.Id)));
                }
            }
        }

        private void Delete(TShortObject obj)
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return;
            try
            {
                var deletedObj = GetFromShort(obj);
                //var deletedObj = GetFromShort((TShortObject)BindingSource.List[BindingSource.Position]);
                if (!IsOpenedEdit(deletedObj))
                {
                    DeleteObj(deletedObj);

                    _cgpClientMainForm.DeleteFromRecentList(deletedObj);
                    ShowData();
                }
                else
                {
                    var errorMsg = deletedObj.ToString();
                    errorMsg += " ";
                    errorMsg += _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteEditing");
                    Dialog.Error(errorMsg);
                }
            }
            catch (Exception error)
            {
                if (error is SqlDeleteReferenceConstraintException)
                    Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteRowInRelationship"));
                else
                {
                    HandledExceptionAdapter.Examine(error);
                    Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteFailed"));
                }
            }
        }

        private bool DeleteFromPosition(int position, out string errorMsg)
        {
            errorMsg = string.Empty;
            var objName = string.Empty;
            if (_cgpClientMainForm.MainIsConnectionLost(true)) return false;

            try
            {
                var deletedObj = GetFromShort((TShortObject)BindingSource.List[position]);
                if (deletedObj == null)
                {
                    errorMsg = _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteEditing");
                    return false;
                }
                objName = deletedObj + " ";
                string msg;
                if (DeleteObjectNotAllowed(deletedObj, out msg))
                {
                    errorMsg = msg;
                    return false;
                }
                if (IsOpenedEdit(deletedObj))
                {
                    errorMsg = objName + _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteEditing");

                    if (Dialog.Question(deletedObj + " " + _cgpClientMainForm.GetLocalizationHelper().GetString("QuestionCloseEditedForm")))
                    {
                        CloseEditedForm(deletedObj);
                    }
                    else
                    {
                        return false;
                    }
                }

                DeleteObj(deletedObj);
                _cgpClientMainForm.DeleteFromRecentList(deletedObj);
                return true;
            }
            catch (Exception error)
            {
                if (error is SqlDeleteReferenceConstraintException)
                    errorMsg = objName + _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteRowInRelationship");
                else if (error is AccessDeniedException)
                    errorMsg = objName + _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteAccessDenied");
                else if (error is OperationDeniedException)
                    errorMsg = objName + _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteSuperAdmin");
                else if (error is InvalidDeleteOperationException)
                    errorMsg = objName + _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteLogedLogin");
                else
                {
                    HandledExceptionAdapter.Examine(error);
                    errorMsg = objName + _cgpClientMainForm.GetLocalizationHelper().GetString("ErrorDeleteFailed");
                }
                return false;
            }
        }

        protected virtual bool DeleteObjectNotAllowed(TObject obj, out string errorMsg)
        {
            errorMsg = string.Empty;
            return false;
        }

        protected void MultiselectDeleteClic(DataGridViewSelectedRowCollection selectedRows)
        {
            if (!Dialog.Question(
                _cgpClientMainForm
                    .GetLocalizationHelper()
                    .GetString("QuestionMultiselectDeleteConfirm")))
            {
                return;
            }

            try
            {
                for (var i = 0; i < selectedRows.Count; i++)
                    if (!TryDeleteItem(selectedRows, i))
                        return;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                ShowData();
            }
        }

        private bool TryDeleteItem(
            DataGridViewSelectedRowCollection selectedRows,
            int i)
        {
            string errorMsg;

            while (!DeleteFromPosition(
                selectedRows[i].Index,
                out errorMsg))
            {
                var dialogResult =
                    MessageBox.Show(
                        errorMsg,
                        _cgpClientMainForm
                            .GetLocalizationHelper()
                            .GetString("DeleteError"),
                        MessageBoxButtons.AbortRetryIgnore,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button3);

                if (dialogResult == DialogResult.Abort)
                    return false;

                if (dialogResult == DialogResult.Ignore)
                    return true;
            }

            return true;
        }

        protected abstract void DeleteObj(TObject obj);

        private bool IsOpenedEdit(TObject obj)
        {
            return _openedEdit.Any(keyvaluepair => 
                keyvaluepair.Key.Equals(new IdAndObjectType(obj.GetId(), obj.GetObjectType())));
        }

        protected void RunFilter()
        {
            if (_changeFilerValues)
            {
                if (CheckFilterValues())
                {
                    FilterSettings.Clear();
                    SetFilterSettings();
                    _changeFilerValues = false;
                    ShowData();
                }
            }
            else if (_changeRuntimeFilerValues)
            {
                ApplyRuntimeFilter();
                _changeRuntimeFilerValues = false;
            }
        }

        protected virtual void ApplyRuntimeFilter()
        {
        }

        protected virtual bool CheckFilterValues()
        {
            return true;
        }

        protected abstract void SetFilterSettings();

        protected void FilterClear_Click()
        {
            FilterSettings.Clear();

            ClearFilterEdits();

            _changeFilerValues = true;
            ShowData();
        }

        protected abstract void ClearFilterEdits();

        protected virtual void FilterValueChanged(object sender, EventArgs e)
        {
            _changeFilerValues = true;
        }

        protected virtual void RuntimeFilterValueChanged(object sender, EventArgs e)
        {
            _changeRuntimeFilerValues = true;
        }

        protected virtual void FilterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                RunFilter();
        }

        protected bool GetObjectFromRow(int index, ref TShortObject obj)
        {
            if (BindingSource == null || BindingSource.Count < index) 
                return false;

            obj = (TShortObject)BindingSource.List[index];
            return true;
        }

        protected bool GetObjectFromActualRow(ref TShortObject obj)
        {
            if (BindingSource == null || BindingSource.Count == 0)
                return false;

            obj = (TShortObject)BindingSource.List[BindingSource.Position];
            return true;
        }

        protected void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView == null) 
                return;

            for (var i = 0; i < gridView.Columns.Count; i++)
            {
                var dataGridViewColumn = gridView.Columns[i];

                if (dataGridViewColumn.Name == columnName)
                    dataGridViewColumn.Visible = false;
            }
        }

        protected void GetWidthColumn(DataGridView gridView, string columnName, ref int width)
        {
            for (var i = 0; i < gridView.Columns.Count; i++)
            {
                var dataGridViewColumn = gridView.Columns[i];

                if (dataGridViewColumn.Name == columnName)
                    width = dataGridViewColumn.Width;
            }
        }

        protected void SetWidthColumn(DataGridView gridView, string columnName, int width)
        {
            for (var i = 0; i < gridView.Columns.Count; i++)
            {
                var dataGridViewColumn = gridView.Columns[i];

                if (dataGridViewColumn.Name == columnName)
                    dataGridViewColumn.Width = width;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                UnregisterEvents();
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);

            _cgpClientMainForm.RemoveFromOpenWindows(this);

            IsRunnedFormEnter = false;
            e.Cancel = true;

            UnregisterEvents();

            Hide();
        }

        public bool IsRunnedFormEnter { get; private set; }

        public BindingSource BindingSource { get; private set; }
        public IList<FilterSettings> FilterSettings { get; private set; }

        private void Form_Enter(Form form)
        {
            if (_cgpClientMainForm.HidingWindows)
                return;

            if (_cgpClientMainForm.MainIsConnectionLost(false))
                return;

            if (!IsRunnedFormEnter)
            {
                IsRunnedFormEnter = true;
                ShowData(true);

                return;
            }

            ShowData();
        }

        protected void ConnectionLost()
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true))
                Close();
        }

        public bool IsEditedObject(TObject obj)
        {
            if (_cgpClientMainForm.MainIsConnectionLost(true)) 
                return false;

            try
            {
                return IsOpenedEdit(obj);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        public void CloseEditedForm(TObject editObj)
        {
            try
            {
                foreach (var keyvaluepair in _openedEdit)
                {
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
                Dialog.Error(_cgpClientMainForm.GetLocalizationHelper().GetString("ErrorCloseFailed"));
            }
        }

        protected abstract void RegisterEvents();

        protected abstract void UnregisterEvents();

        public void CloseEditedFormById(Guid idObj)
        {
            try
            {
                foreach (var keyvaluepair in _openedEdit)
                {
                    if ((Guid) keyvaluepair.Key.Id == idObj)
                    {
                        CloseForm(keyvaluepair.Value);
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void CloseForm(ACgpPluginEditForm<TCgpVisualPlugin, TObject> form)
        {
            if (InvokeRequired)
                BeginInvoke(
                    new Action<ACgpPluginEditForm<TCgpVisualPlugin, TObject>>(CloseForm), 
                    form);
            else
                form.Close();
        }

        // Moves the Window to the front of the z-order.
        // If the Window is minimized, the Window is moved to the front and displayed as Normal.
        private static void BringWindowToFront(Form aForm)
        {
            if (aForm.WindowState == FormWindowState.Minimized)
                aForm.WindowState = FormWindowState.Normal;

            aForm.BringToFront();
        }

        protected class ShortObjectExtension<TShortObject, TExt>
            where TShortObject : IShortObject
        {
            public TShortObject ShortObject { get; private set; }
            public TExt Value { get; set; }

            public ShortObjectExtension(TShortObject shorObject, TExt value)
            {
                ShortObject = shorObject;
                Value = value;
            }
        }

        protected class ShortObjectExtension<TShortObject, TExt1, TExt2>
            where TShortObject : IShortObject
        {
            public TShortObject ShortObject { get; private set; }
            public TExt1 Value1 { get; set; }
            public TExt2 Value2 { get; set; }

            public ShortObjectExtension(TShortObject shorObject, TExt1 value1, TExt2 value2)
            {
                ShortObject = shorObject;
                Value1 = value1;
                Value2 = value2;
            }
        }

        protected class ShortObjectExtension<TShortObject, TExt1, TExt2, TExt3>
            where TShortObject : IShortObject
        {
            public TShortObject ShortObject { get; private set; }
            public TExt1 Value1 { get; set; }
            public TExt2 Value2 { get; set; }
            public TExt3 Value3 { get; set; }

            public ShortObjectExtension(TShortObject shorObject, TExt1 value1, TExt2 value2, TExt3 value3)
            {
                ShortObject = shorObject;
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
            }
        }

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
