using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Client;
using Contal.IwQuick;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Client
{
    public class ReferencedByHandler
    {
        #region Referended By

        protected IList<AOrmObject> _listReferencedObj = null;
        protected BindingSource _bindingSourceOrmObjects = new BindingSource();
        protected TextBox _filterReferencedTextBox;
        protected DataGridView _dgRootReferencedBy;
        public Func<IList<AOrmObject>> DoGetReferencedObject;
        //public event Contal.IwQuick.DVoid2Void DoAfterDockUndock;

        public TextBox FilterEditRo
        {
            get { return _filterReferencedTextBox; }
            set { _filterReferencedTextBox = value; }
        }

        public DataGridView DataGridRo
        {
            get { return _dgRootReferencedBy; }
            set 
            { 
                _dgRootReferencedBy = value;
                _dgRootReferencedBy.VisibleChanged += _dgRootReferencedBy_VisibleChanged;
            }
        }

        void _dgRootReferencedBy_VisibleChanged(object sender, EventArgs e)
        {
            ShowExtendedRecords();
        }

        public ReferencedByHandler()
        {
        }

        //public ReferencedByHandler(TextBox edit, DataGridView dgv, Contal.IwQuick.DVoid2Type<IList<AOrmObject>> obtainReferencedObj, Button bRefresh, Button bClear, Button bFilter, TabPage tabRefBy)
        //{
        //    _filterReferencedTextBox = edit;
        //    _dgRootReferencedBy = dgv;
        //    DoGetReferencedObject = obtainReferencedObj;
        //    bRefresh.Click += new EventHandler(RoRefreshClick);
        //    bFilter.Click += new EventHandler(RoSearchClick);
        //    bClear.Click += new EventHandler(RoClearClick);
        //    dgv.DoubleClick += new EventHandler(ReferencedByDoubleClick);
        //    tabRefBy.Enter += new EventHandler(RefencedByEnter);
        //}

        public void RefencedByEnter(object sender, EventArgs e)
        {
            ThreadObtainReferencedObjects();
        }

        public void FilterReferencedTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                DoFilterReferencedObjects();
            }
        }

        public void RoRefreshClick(object sender, EventArgs e)
        {
            ObtainReferencedObjects();
        }

        public void RoSearchClick(object sender, EventArgs e)
        {
            DoFilterReferencedObjects();
        }

        public void RoClearClick(object sender, EventArgs e)
        {
            if (_filterReferencedTextBox == null) return; 
            _filterReferencedTextBox.Text = string.Empty;
            ObtainReferencedObjects();
        }

        public void ReferencedByDoubleClick(object sender, EventArgs e)
        {
            OpenReferencedObject();
        }

        protected void ObtainReferencedObjects()
        {
            SafeThread.StartThread(ThreadObtainReferencedObjects);
        }

        protected void ThreadObtainReferencedObjects()
        {
            if (DoGetReferencedObject == null) return;
            _listReferencedObj = DoGetReferencedObject();
            ShowReferencerObjects();
        }

        protected void DoFilterReferencedObjects()
        {
            if (_filterReferencedTextBox == null) return;
            if (_dgRootReferencedBy == null) return;
            if (_dgRootReferencedBy.InvokeRequired)
            {
                _dgRootReferencedBy.BeginInvoke(new DVoid2Void(ShowReferencerObjects));
            }
            else
            {
                if (_dgRootReferencedBy.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in _dgRootReferencedBy.Rows)
                    {
                        if (row.Cells["Object"].Value != null &&
                            row.Cells["Object"].Value.ToString().ToUpper().Contains(_filterReferencedTextBox.Text.ToUpper()))
                        {
                            row.Visible = true;
                        }
                        else
                        {
                            _dgRootReferencedBy.CurrentCell = null;
                            row.Visible = false;
                        }
                    }
                }
            }
        }

        protected void OpenReferencedObject()
        {
            if (_bindingSourceOrmObjects == null || _bindingSourceOrmObjects.Count == 0) return;
            AOrmObject ormObj = (AOrmObject)_bindingSourceOrmObjects.List[_bindingSourceOrmObjects.Position];
            DbsSupport.OpenEditForm(ormObj);
        }

        protected void ShowReferencerObjects()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_dgRootReferencedBy == null) return;

            if (_dgRootReferencedBy.InvokeRequired)
            {
                _dgRootReferencedBy.BeginInvoke(new DVoid2Void(ShowReferencerObjects));
            }
            else
            {
                _bindingSourceOrmObjects = new BindingSource();
                _bindingSourceOrmObjects.DataSource = _listReferencedObj;
                _bindingSourceOrmObjects.AllowNew = false;
                _dgRootReferencedBy.DataSource = _bindingSourceOrmObjects;

                if (!_dgRootReferencedBy.Columns.Contains("ObjectType"))
                {
                    DataGridViewImageColumn dgvic = new DataGridViewImageColumn(true);
                    dgvic.Name = "ObjectType";
                    dgvic.HeaderText = "Type";
                    dgvic.ImageLayout = DataGridViewImageCellLayout.Normal;
                    dgvic.Resizable = DataGridViewTriState.False;                 
                    dgvic.Width = 32;
                    _dgRootReferencedBy.Columns.Add(dgvic);
                }
                if (!_dgRootReferencedBy.Columns.Contains("Object"))
                {
                    _dgRootReferencedBy.Columns.Add("Object", "Object");
                }
                if (!_dgRootReferencedBy.Columns.Contains("Description"))
                {
                    _dgRootReferencedBy.Columns.Add("Description", "Description");
                }

                ShowExtendedRecords();
                
                _dgRootReferencedBy.Columns["Object"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                int widthCell = _dgRootReferencedBy.Columns["Object"].Width;
                _dgRootReferencedBy.Columns["Object"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                _dgRootReferencedBy.Columns["Object"].Width = widthCell;
                _dgRootReferencedBy.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                CgpClient.Singleton.LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgRootReferencedBy);
            }
        }

        private void ShowExtendedRecords()
        {
            if (_dgRootReferencedBy == null) return;

            if (_dgRootReferencedBy.InvokeRequired)
            {
                _dgRootReferencedBy.BeginInvoke(new DVoid2Void(ShowExtendedRecords));
            }
            else
            {
                if (_dgRootReferencedBy.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in _dgRootReferencedBy.Rows)
                    {
                        AOrmObject ormObj = (AOrmObject)_bindingSourceOrmObjects.List[row.Index];
                        if (ormObj == null) continue;
                        Type type = ormObj.GetType();

                        if (type == typeof(Calendar))
                        {
                            if (ormObj.ToString() == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                            {
                                row.Cells["Object"].Value = CgpClient.Singleton.LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                            }
                            else
                            {
                                row.Cells["Object"].Value = ormObj.ToString();
                            }
                        }
                        else if (type == typeof(DayType))
                        {
                            if (ormObj.ToString() == DayType.IMPLICIT_DAY_TYPE_HOLIDAY)
                            {
                                row.Cells["Object"].Value = CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_HOLIDAY);
                            }
                            else if (ormObj.ToString() == DayType.IMPLICIT_DAY_TYPE_VACATION)
                            {
                                row.Cells["Object"].Value = CgpClient.Singleton.LocalizationHelper.GetString(DayType.IMPLICIT_DAY_TYPE_VACATION);
                            }
                            else
                            {
                                row.Cells["Object"].Value = ormObj.ToString();
                            }
                        }
                        else
                        {
                            row.Cells["Object"].Value = ormObj.ToString();
                        }

                        row.Cells["ObjectType"].Value = DbsSupport.GetIconForObjectType(ormObj.GetObjectType());

                        PropertyInfo pi = type.GetProperty("Description");
                        if (pi != null)
                        {
                            row.Cells["Description"].Value = pi.GetValue(ormObj, null) as string;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
