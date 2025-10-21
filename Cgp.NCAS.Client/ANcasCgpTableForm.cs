using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.RemotingCommonNCAS;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Client;

namespace Contal.Cgp.NCAS.Client
{
    public abstract class ANcasCgpTableForm<T> : ANcasCgpFullscreenForm
    {
        private BindingSource _bindingSource = null;
        protected IList<FilterSettings> _filterSettings = new List<FilterSettings>();
        private bool _changeFilerValues = false;
        private bool _runShowData = false;
        private ACgpEditForm<T> _formInsert = null;
        private Dictionary<T, ACgpEditForm<T>> _openedEdit = new Dictionary<T, ACgpEditForm<T>>();

        public ANcasCgpTableForm()
        {
            FormOnEnter += new Contal.IwQuick.DFromTToVoid<Form>(Form_Enter);
        }

        private void Form_Enter(Form form)
        {
            if (!CgpClient.Singleton.IsConnectionLost(false))
            {
                ShowData();
            }
            else
            {
                RemoveDataSource();
            }
        }

        protected void ShowData()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            if (_runShowData) return;
            _runShowData = true;
            CgpClientMainForm.Singleton.StartProgress();
            Contal.IwQuick.Threads.SafeThread.StartThread(new Contal.IwQuick.DFromVoidToVoid(LoadTable));
        }

        private void LoadTable()
        {
            UpdateGridView(GetData());
            _runShowData = false;
            CgpClientMainForm.Singleton.StopProgress();
            CgpClientMainForm.Singleton.ModifyOpenWindowImage(this);
        }

        protected abstract ICollection<T> GetData();

        private void UpdateGridView(ICollection<T> data)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DFromTToVoid<ICollection<T>>(UpdateGridView), data);
            }
            else
            {
                int lastPosition = 0;
                if (_bindingSource != null && _bindingSource.Count != 0)
                    lastPosition = _bindingSource.Position;
                _bindingSource = new BindingSource();
                _bindingSource.DataSource = data;
                if (lastPosition != 0) _bindingSource.Position = lastPosition;
                _bindingSource.AllowNew = false;

                ModifyGridView(_bindingSource);
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
            //return null != CgpClient.Singleton.MainServerProvider;
            return false;

        }

        #endregion

        protected void Insert()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
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
                    _formInsert.BringToFront();
                }

            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertFailed"));
            }
        }

        public bool OpenInsertDialg(ref T outObj)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return false;

            try
            {
                ACgpEditForm<T> formInsert = CreateEditForm(outObj, ShowOptionsEditForm.InsertDialog);
                return formInsert.ShowInsertDialog(ref outObj);
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorInsertFailed"));
                return false;
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
            ShowData();
        }

        protected void Edit_Click()
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            Edit();
        }

        private void Edit()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            T obj = (T)_bindingSource.List[_bindingSource.Position];
            OpenEditForm(obj);
        }

        public void OpenEditForm(T editObj)
        {
            try
            {
                if (!OpenExistEditForm(editObj))
                {
                    ACgpEditForm<T> formEdit = CreateEditForm(editObj, ShowOptionsEditForm.Edit);
                    formEdit.Show();
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorEditFailed"));
            }
        }

        private bool OpenExistEditForm(T obj)
        {
            foreach (KeyValuePair<T, ACgpEditForm<T>> keyvaluepair in _openedEdit)
            {
                if (Compare(keyvaluepair.Key, obj))
                {
                    keyvaluepair.Value.BringToFront();
                    return true;
                }
            }

            return false;
        }

        protected abstract bool Compare(T obj1, T obj2);

        private void RemoveFromOpenedEdit(T obj)
        {
            foreach (KeyValuePair<T, ACgpEditForm<T>> keyvaluepair in _openedEdit)
            {
                if (Compare(keyvaluepair.Key, obj))
                {
                    _openedEdit.Remove(keyvaluepair.Key);
                    return;
                }
            }
        }

        public void BeforeEdit(ACgpEditForm<T> formEdit, T obj)
        {
            CgpClientMainForm.Singleton.AddToRecentList(obj);
            _openedEdit.Add(obj, formEdit);
        }

        public void AfterEdit(T obj)
        {
            RemoveFromOpenedEdit(obj);
        }

        protected void Delete_Click()
        {
            if (_bindingSource == null || _bindingSource.Count == 0) return;
            if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionDeleteConfirm")))
            {
                Delete();
            }
        }

        private void Delete()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;
            try
            {
                T deletedObj = (T)_bindingSource.List[_bindingSource.Position];
                if (!IsOpenedEdit(deletedObj))
                {
                    DeleteObj(deletedObj);

                    CgpClientMainForm.Singleton.DeleteFromRecentList(deletedObj);
                    ShowData();
                }
                else
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteEditing"));
                }
            }
            catch (Exception error)
            {
                if (error is Contal.IwQuick.SqlDeleteReferenceConstraintException)
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteRowInRelationship"));
                else
                    Contal.IwQuick.UI.Dialog.Error(GetString("ErrorDeleteFailed"));
            }
        }

        protected abstract void DeleteObj(T obj);

        private bool IsOpenedEdit(T obj)
        {
            foreach (KeyValuePair<T, ACgpEditForm<T>> keyvaluepair in _openedEdit)
            {
                if (Compare(keyvaluepair.Key, obj))
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

            obj = (T)_bindingSource.List[index];
            return true;
        }

        protected bool GetObjectFromActualRow(ref T obj)
        {
            if (_bindingSource == null || _bindingSource.Count == 0)
            {
                return false;
            }
            
            obj = (T)_bindingSource.List[_bindingSource.Position];
            return true;
        }

        protected void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView == null) return;
            if (gridView.Columns.Count < 1) return;
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (gridView.Columns[i].Name == columnName)
                    gridView.Columns[i].Visible = false;
            }
        }

        protected void GetWidthColumn(DataGridView gridView, string columnName, ref int width)
        {
            if (gridView.Columns.Count < 1) return;
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (gridView.Columns[i].Name == columnName)
                    width = gridView.Columns[i].Width;
            }
        }

        protected void SetWidthColumn(DataGridView gridView, string columnName, int width)
        {
            if (gridView.Columns.Count < 1) return;
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (gridView.Columns[i].Name == columnName)
                    gridView.Columns[i].Width = width;
            }
        }
    }
}
