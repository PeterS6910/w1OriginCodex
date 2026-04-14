using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Globals;
using Contal.IwQuick.Localization;
using Contal.Cgp.Server.Beans;
using System.Reflection;
using Contal.IwQuick.UI;
using Contal.IwQuick;
using Contal.Cgp.BaseLib;

namespace Contal.Cgp.Client
{
    public partial class ReferencedByForm : UserControl
    {
        private const int BUTTONS_MARGIN_ADJUST = 6;

        protected IList<AOrmObject> _listReferencedObj = null;
        protected BindingSource _bindingSourceOrmObjects = new BindingSource();
        private LocalizationHelper _localizationHelper;
        private ImageList _imageList;

        public delegate IList<AOrmObject> GetListReferencedObjectsHandler();
        public event GetListReferencedObjectsHandler _getListReferencedObjects;

        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }


        public DataGridView DataGrid
        {
            get { return _dgRootReferencedBy; }
            set { _dgRootReferencedBy = value; }
        }

        public ReferencedByForm(GetListReferencedObjectsHandler listReferencedObjectsMethod)
            : this(listReferencedObjectsMethod, CgpClient.Singleton.LocalizationHelper, ObjectImageList.Singleton.GetAllObjectImages())
        {
        }

        private GetListReferencedObjectsHandler _listReferencedObjectsMethod;

        public ReferencedByForm(GetListReferencedObjectsHandler listReferencedObjectsMethod, LocalizationHelper localizationHelper,
            ImageList imageList)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            _listReferencedObjectsMethod = listReferencedObjectsMethod;
            _getListReferencedObjects += new GetListReferencedObjectsHandler(_listReferencedObjectsMethod);
            _localizationHelper = localizationHelper;
            _localizationHelper.LanguageChanged += new Contal.IwQuick.DVoid2Void(_localizationHelper_LanguageChanged);
            _localizationHelper.TranslateControl(this);
            _imageList = imageList;
            _imageList.ImageSize = new Size(16, 16);
            _icbObjectType.ImageList = _imageList;
            _icbObjectType.Localizationhelper = _localizationHelper;
        }

        void ReferencedByForm_Disposed(object sender, System.EventArgs e)
        {
            _getListReferencedObjects -= _listReferencedObjectsMethod;
            _localizationHelper.LanguageChanged -= _localizationHelper_LanguageChanged;
        }

        void _localizationHelper_LanguageChanged()
        {
            _localizationHelper.TranslateControl(this);
        }

        private void ObtainReferencedObjects()
        {
            Contal.IwQuick.Threads.SafeThread.StartThread(ThreadObtainReferencedObjects);
        }

        private void ThreadObtainReferencedObjects()
        {
            GetReferencedObjectFromServer();
            ShowReferencerObjects();
        }

        private void GetReferencedObjectFromServer()
        {
            if (_getListReferencedObjects != null)
            {
                _listReferencedObj = _getListReferencedObjects();
                FillImageComboBoxObjectType();
            }
        }

        private void FillImageComboBoxObjectType()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(FillImageComboBoxObjectType));
            }
            else
            {
                AdjustObjectTypeControls(false);
                _icbObjectType.Items.Clear();
                _icbObjectType.Items.Add(new ImageComboBoxItem(string.Empty, -1));

                if (_listReferencedObj == null)
                    return;

                foreach (string item in GetObjectTypes())
                {
                    _icbObjectType.Items.Add(
                        new ImageComboBoxItem(item,
                            GetImageIndex(item)));
                }

                if (_icbObjectType.Items.Count > 2)
                {
                    AdjustObjectTypeControls(true);
                }
            }
        }

        private void AdjustObjectTypeControls(bool visible)
        {
            if (visible)
            {
                _bFilter.Location = new Point(_icbObjectType.Location.X + _icbObjectType.Width + BUTTONS_MARGIN_ADJUST, _bFilter.Location.Y);
                _bClear.Location = new Point(_bFilter.Location.X + _bFilter.Width + BUTTONS_MARGIN_ADJUST, _bClear.Location.Y);
            }
            else
            {
                _bFilter.Location = new Point(_tbFilter.Location.X + _tbFilter.Width + BUTTONS_MARGIN_ADJUST, _bFilter.Location.Y);
                _bClear.Location = new Point(_bFilter.Location.X + _bFilter.Width + BUTTONS_MARGIN_ADJUST, _bClear.Location.Y);
            }
            _lObjectType.Visible = visible;
            _icbObjectType.Visible = visible;
        }

        private List<string> GetObjectTypes()
        {
            List<string> _objectTypes = new List<string>();

            foreach (AOrmObject obj in _listReferencedObj)
            {
                if (!_objectTypes.Contains(obj.GetObjectType().ToString()))
                {
                    _objectTypes.Add(obj.GetObjectType().ToString());
                }
            }

            return _objectTypes;
        }


        private int GetImageIndex(string key)
        {
            return _imageList.Images.IndexOfKey(key);
        }

        private void DoFilterReferencedObjects()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(ShowReferencerObjects));
            }
            else
            {
                if (_dgRootReferencedBy.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in _dgRootReferencedBy.Rows)
                    {
                        if (row.Cells["Object"].Value.ToString().ToUpper().Contains(_tbFilter.Text.ToUpper()) &&
                            IsSameTypeAsSelectedObjectType(row.Cells["ObjectType"].Value.ToString()))
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

        private bool IsSameTypeAsSelectedObjectType(string type)
        {
            if (_icbObjectType.SelectedItemObject == null || _icbObjectType.SelectedItemObject.ToString() == string.Empty
                || (_icbObjectType.SelectedItemObjectType == type))
                return true;

            return false;
        }

        private void OpenReferencedObject()
        {
            if (_bindingSourceOrmObjects == null || _bindingSourceOrmObjects.Count == 0) return;
            AOrmObject ormObj = (AOrmObject)_bindingSourceOrmObjects.List[_bindingSourceOrmObjects.Position];
            DbsSupport.OpenEditForm(ormObj);
        }

        private void ShowReferencerObjects()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(ShowReferencerObjects));
            }
            else
            {
                _bindingSourceOrmObjects = new BindingSource();
                _bindingSourceOrmObjects.DataSource = _listReferencedObj;
                _bindingSourceOrmObjects.AllowNew = false;
                _dgRootReferencedBy.DataSource = _bindingSourceOrmObjects;

                AddColumns(_dgRootReferencedBy);

                if (_dgRootReferencedBy.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in _dgRootReferencedBy.Rows)
                    {
                        AOrmObject ormObj = (AOrmObject)_bindingSourceOrmObjects.List[row.Index];
                        if (ormObj == null) continue;
                        Type type = ormObj.GetType();

                        AddColumnsValues(row, ormObj, type);
                    }
                }
                _dgRootReferencedBy.Columns["Object"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                int i = _dgRootReferencedBy.Columns["Object"].Width;
                _dgRootReferencedBy.Columns["Object"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                _dgRootReferencedBy.Columns["Object"].Width = i;
                _dgRootReferencedBy.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                _dgRootReferencedBy.Columns["ObjectType"].Visible = false;
                _localizationHelper.TranslateDataGridViewColumnsHeaders(_dgRootReferencedBy);
            }
        }

        private void AddImageColumn(DataGridView dgv)
        {
            if (!dgv.Columns.Contains("ObjectTypeIcon"))
            {
                DataGridViewImageColumn dgvic = new DataGridViewImageColumn(false);
                dgvic.Name = "ObjectTypeIcon";
                dgvic.HeaderText = "Type";
                dgvic.ImageLayout = DataGridViewImageCellLayout.Normal;
                dgvic.Resizable = DataGridViewTriState.False;
                dgvic.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dgv.Columns.Add(dgvic);
            }
        }

        private void AddColumns(DataGridView dgv)
        {
            AddImageColumn(_dgRootReferencedBy);

            if (!dgv.Columns.Contains("ObjectType"))
            {
                dgv.Columns.Add("ObjectType", "ObjectType");
            }
            if (!dgv.Columns.Contains("Object"))
            {
                dgv.Columns.Add("Object", "Object");
            }
            if (!dgv.Columns.Contains("Description"))
            {
                dgv.Columns.Add("Description", "Description");
            }
        }

        private void AddObjectValue(DataGridViewRow row, AOrmObject ormObj)
        {
            row.Cells["Object"].Value = CgpClient.Singleton.GetLocalizedObjectName(ormObj);
        }

        private void AddColumnsValues(DataGridViewRow row, AOrmObject ormObj, Type type)
        {
            AddObjectValue(row, ormObj);

            row.Cells["ObjectTypeIcon"].Value = _imageList.Images[ormObj.GetObjectType().ToString()];
            row.Cells["ObjectType"].Value = ormObj.GetObjectType();

            PropertyInfo pi = type.GetProperty("Description");
            if (pi != null)
            {
                row.Cells["Description"].Value = pi.GetValue(ormObj, null) as string;
            }
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObtainReferencedObjects();
        }

        private void _bFilter_Click(object sender, EventArgs e)
        {
            DoFilterReferencedObjects();
        }

        private void _tbFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                DoFilterReferencedObjects();
            }
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            _tbFilter.Text = string.Empty;
            _icbObjectType.SelectedIndex = 0;
            ShowReferencerObjects();
        }

        private void _dgRootReferencedBy_DoubleClick(object sender, EventArgs e)
        {
            OpenReferencedObject();
        }

        private void _icbObjectType_SelectedValueChanged(object sender, EventArgs e)
        {
            DoFilterReferencedObjects();
        }

        private void ReferencedByForm_Load(object sender, EventArgs e)
        {
            ObtainReferencedObjects();
        }
    }
}
