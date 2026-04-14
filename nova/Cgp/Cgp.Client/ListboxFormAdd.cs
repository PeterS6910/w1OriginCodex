using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class ListboxFormAdd :
#if DESIGNER
        Form
#else
 CgpTranslateForm
#endif
    {
        public enum ListboxFormAddData
        {
            None = 0,
            AOrmObjects = 1,
            ListObjects = 2,
            ModifyObjects = 3,
            Strings = 4
        }

        private const int DEFAULT_CONTROLS_MARGIN = 6;

        private ListboxFormAddData _dataType = ListboxFormAddData.None;
        private ICollection<AOrmObject> _data = null;
        private ICollection<IListObjects> _dataListObjects = null;
        private ICollection<IModifyObject> _dataListModifyObjects = null;
        private object _outObject = null;

        private bool _showObjectSubTypes = false;
        
        private ImageList _objectsImageList = new ImageList();
        private List<string> _objectTypes = new List<string>();
        private bool _cbObjectTypesFilled = false;

        public ListboxFormAdd(IEnumerable<AOrmObject> data, string formName)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitComponent(formName);

            if (data != null)
                _data = new LinkedList<AOrmObject>(
                    data.OrderBy(
                        ormObject =>
                            string.Format("{0}{1}",
                                ((byte) ormObject.GetObjectType()).ToString("D3"),
                                ormObject.ToString())));

            _dataType = ListboxFormAddData.AOrmObjects;
            InitImageControls();
            ShowData("");
            ShowHideObjectTypeFilter();
        }

        public ListboxFormAdd(IEnumerable<IListObjects> data, string formName)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitComponent(formName);

            if (data != null)
                _dataListObjects = new LinkedList<IListObjects>(
                    data.OrderBy(
                        listObject =>
                            string.Format("{0}{1}",
                                ((byte) listObject.GetOrmObj().GetObjectType()).ToString("D3"),
                                listObject.ToString())));

            _dataType = ListboxFormAddData.ListObjects;
            InitImageControls();
            ShowData("");
            ShowHideObjectTypeFilter();
        }

        public ListboxFormAdd(IEnumerable<IModifyObject> data, string formName)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitComponent(formName);
            FillDataListModifyObjects(data);
            _dataType = ListboxFormAddData.ModifyObjects;
            InitImageControls();
            ShowData("");
            ShowHideObjectTypeFilter();
        }

        private void FillDataListModifyObjects(IEnumerable<IModifyObject> data)
        {
            if (data == null)
                return;

            _dataListModifyObjects = new LinkedList<IModifyObject>(
                data
                    .Select(
                        modifyObject =>
                        {
                            modifyObject.FullName =
                                CgpClientMainForm.TranslateOrmObjectName(
                                    modifyObject.GetOrmObjectType,
                                    modifyObject.FullName);

                            return modifyObject;
                        })
                    .OrderBy(
                        modifyObject =>
                            modifyObject,
                        new ComparerForIModifyObject()));
        }

        public ListboxFormAdd(IEnumerable<IModifyObject> data, string formName, bool showObjectSubTypes)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitComponent(formName);
            FillDataListModifyObjects(data);
            _dataType = ListboxFormAddData.ModifyObjects;
            _showObjectSubTypes = showObjectSubTypes;
            InitImageControls();
            ShowData("");
            ShowHideObjectTypeFilter();
        }

        private void InitComponent(string formName)
        {
            InitializeComponent();
            this.Text = formName;
        }

        private void InitImageControls()
        {
            ObjectImageList.Singleton.FillWithClientAndPluginsObjectImages(_objectsImageList);
            _objectsImageList.ColorDepth = ColorDepth.Depth32Bit;
            _objectsImageList.ImageSize = new Size(16, 16);

            _ilbData.ImageList = _objectsImageList;
            _icbObjectTypeFilter.ImageList = _objectsImageList;

            _icbObjectTypeFilter.Localizationhelper = LocalizationHelper;
        }

        private void ShowHideObjectTypeFilter()
        {
            if (_icbObjectTypeFilter.Items.Count > 2)
            {
                _icbObjectTypeFilter.Visible = true;
            }
            else
            {
                this.Controls.Remove(_icbObjectTypeFilter);
                _eFilter.Size = new Size(_ilbData.Width, _eFilter.Height);
            }
        }

        private void ShowData(string filter)
        {
            int allItems = 0;
            int shownItems = 0;
            _ilbData.Items.Clear();

            switch (_dataType)
            {
                case ListboxFormAddData.None: return;

                case ListboxFormAddData.AOrmObjects: ShowAOrmObjects(ref allItems, ref shownItems, filter);
                    break;
                case ListboxFormAddData.ListObjects: ShowListObjects(ref allItems, ref shownItems, filter);
                    break;
                case ListboxFormAddData.ModifyObjects:
                    {
                        if (!_showObjectSubTypes)
                        {
                            ShowModifyObjects(ref allItems, ref shownItems, filter);
                        }
                        else
                        {
                            try
                            {
                                ShowModifyObjectSubTypes(ref allItems, ref shownItems, filter);
                            }
                            catch
                            {
                                ShowModifyObjects(ref allItems, ref shownItems, filter);
                            }
                        }
                    }
                    break;
            }

            _lShownNumber.Text = shownItems.ToString() + "/" + allItems.ToString();
        }

        private void ShowAOrmObjects(ref int allItems, ref int shownItems, string filter)
        {
            if (_data != null)
            {
                allItems = _data.Count;
                foreach (AOrmObject obj in _data)
                {
                    if (obj.Contains(filter))
                    {
                        if (_icbObjectTypeFilter.SelectedItemObject == null
                           || (_icbObjectTypeFilter.SelectedItemObject.ToString() == string.Empty))
                        {
                            _ilbData.Items.Add(new ImageListBoxItem(obj, GetImageIndex(obj.GetObjectType().ToString())));
                            shownItems++;
                        }
                        else if (_icbObjectTypeFilter.SelectedItemObjectType == obj.GetObjectType().ToString())
                        {
                            _ilbData.Items.Add(new ImageListBoxItem(obj, GetImageIndex(obj.GetObjectType().ToString())));
                            shownItems++;
                        }
                    }
                    if (!_objectTypes.Contains(obj.GetObjectType().ToString()) && (!_cbObjectTypesFilled))
                    {
                        _objectTypes.Add(obj.GetObjectType().ToString());
                    }
                }
                FillObjectTypesComboBox();
            }
        }

        private void ShowListObjects(ref int allItems, ref int shownItems, string filter)
        {
            if (_dataListObjects != null)
            {
                allItems = _dataListObjects.Count;

                foreach (IListObjects obj in _dataListObjects)
                {
                    if (obj.Contains(filter))
                    {
                        if (_icbObjectTypeFilter.SelectedItemObject == null
                             || (_icbObjectTypeFilter.SelectedItemObject.ToString() == string.Empty))
                        {
                            _ilbData.Items.Add(new ImageListBoxItem(obj, GetImageIndex(obj.GetOrmObj().GetObjectType().ToString())));
                            shownItems++;
                        }
                        else if (_icbObjectTypeFilter.SelectedItemObjectType == obj.GetOrmObj().GetObjectType().ToString())
                        {
                            _ilbData.Items.Add(new ImageListBoxItem(obj, GetImageIndex(obj.GetOrmObj().GetObjectType().ToString())));
                            shownItems++;
                        }
                    }
                    if (!_objectTypes.Contains(obj.GetOrmObj().GetObjectType().ToString()) && (!_cbObjectTypesFilled))
                    {
                        _objectTypes.Add(obj.GetOrmObj().GetObjectType().ToString()); ;
                    }
                }
                FillObjectTypesComboBox();
            }
        }

        private void ShowModifyObjects(ref int allItems, ref int shownItems, string filter)
        {
            if (_dataListModifyObjects != null)
            {
                allItems = _dataListModifyObjects.Count;
                foreach (IModifyObject obj in _dataListModifyObjects)
                {
                    if (obj.Contains(filter))
                    {
                        if (_icbObjectTypeFilter.SelectedItemObject == null
                            || (_icbObjectTypeFilter.SelectedItemObject.ToString() == string.Empty))
                        {
                            _ilbData.Items.Add(new ImageListBoxItem(obj, GetImageIndex(obj.GetOrmObjectType.ToString())));
                            shownItems++;
                        }
                        else if (_icbObjectTypeFilter.SelectedItemObjectType == obj.GetOrmObjectType.ToString())
                        {
                            _ilbData.Items.Add(new ImageListBoxItem(obj, GetImageIndex(obj.GetOrmObjectType.ToString())));
                            shownItems++;
                        }
                    }
                    if (!_objectTypes.Contains(obj.GetOrmObjectType.ToString()) && (!_cbObjectTypesFilled))
                    {
                        _objectTypes.Add(obj.GetOrmObjectType.ToString()); ;
                    }
                }
                FillObjectTypesComboBox();
            }
        }


        private void ShowModifyObjectSubTypes(ref int allItems, ref int shownItems, string filter)
        {
            if (_dataListModifyObjects != null)
            {
                allItems = _dataListModifyObjects.Count;
                foreach (IModifyObject obj in _dataListModifyObjects)
                {
                    if (obj.Contains(filter))
                    {
                        if (_icbObjectTypeFilter.SelectedItemObject == null
                            || (_icbObjectTypeFilter.SelectedItemObject.ToString() == string.Empty))
                        {
                            _ilbData.Items.Add(new ImageListBoxItem(obj, GetImageIndex(obj.GetObjectSubType(0))));
                            shownItems++;
                        }
                        else if (_icbObjectTypeFilter.SelectedItemObjectType == obj.GetObjectSubType(0))
                        {
                            _ilbData.Items.Add(new ImageListBoxItem(obj, GetImageIndex(obj.GetObjectSubType(0))));
                            shownItems++;
                        }
                    }
                    if (!_objectTypes.Contains(obj.GetObjectSubType(0)) && (!_cbObjectTypesFilled))
                    {
                        _objectTypes.Add(obj.GetObjectSubType(0));
                    }
                }
                FillObjectTypesComboBox();
            }
        }

        private void FillObjectTypesComboBox()
        {
            if (!_cbObjectTypesFilled)
            {
                _icbObjectTypeFilter.Items.Add(new ImageComboBoxItem(string.Empty, -1));
                foreach (string item in _objectTypes)
                {
                    _icbObjectTypeFilter.Items.Add(
                        new ImageComboBoxItem(item, GetImageIndex(item.ToString())));
                }
                _cbObjectTypesFilled = true;
            }
        }

        private int GetImageIndex(string key)
        {
            return _objectsImageList.Images.IndexOfKey(key);
        }

        public void ShowDialog(out object outObject)
        {
            ShowDialog(out outObject, false);
        }

        public void ShowDialog(out IModifyObject outModifyObject)
        {
            ShowDialog(out outModifyObject, false);
        }

        public void ShowDialogMultiSelect(out Cgp.BaseLib.ListOfObjects listOfObjects)
        {
            Object outObject;
            ShowDialog(out outObject, true);

            object[] objectArray = outObject as object[];

            if (objectArray != null && objectArray.Count() > 0)
            {
                listOfObjects = new Cgp.BaseLib.ListOfObjects(objectArray.ToList());
            }
            else
            {
                listOfObjects = null;
            }
        }

        private void ShowDialog(out object outObject, bool canReturnMoreObjects)
        {
            _ilbData.SelectionMode = canReturnMoreObjects == true ? SelectionMode.MultiExtended : SelectionMode.One;
            ShowDialog();
            outObject = _outObject;

            if (canReturnMoreObjects && outObject != null)
            {
                Cgp.BaseLib.ListOfObjects listOfObjects = outObject as Cgp.BaseLib.ListOfObjects;

                if (listOfObjects != null && listOfObjects.Count == 0)
                    outObject = null;
            }
        }

        private void ShowDialog(out IModifyObject outObject, bool canReturnMoreObjects)
        {
            _ilbData.SelectionMode = canReturnMoreObjects == true ? SelectionMode.MultiExtended : SelectionMode.One;
            ShowDialog();
            outObject = (IModifyObject)_outObject;

            if (canReturnMoreObjects && outObject != null)
            {
                Cgp.BaseLib.ListOfObjects listOfObjects = outObject as Cgp.BaseLib.ListOfObjects;

                if (listOfObjects != null && listOfObjects.Count == 0)
                    outObject = null;
            }
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            ShowData(_eFilter.Text);
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _eFilter.Text = "";
            ShowData(_eFilter.Text);
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            List<object> result = null;
            if (_ilbData.SelectionMode == SelectionMode.MultiExtended)
            {
                if (_ilbData.SelectedItemsObjects == null) return;

                result = new List<object>();
                foreach (object item in _ilbData.SelectedItemsObjects)
                {
                    result.Add(item);
                }
            }
            if (_ilbData.SelectedItemObject == null) return;
            _outObject = _ilbData.SelectionMode == SelectionMode.One ? _ilbData.SelectedItemObject : result.ToArray();
            Close();
        }

        private void _lbData_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (_ilbData.SelectionMode == SelectionMode.One)
            {
                if (_ilbData.SelectedItemObject != null)
                {
                    _outObject = _ilbData.SelectedItemObject;
                    Close();
                }
            }
            if (_ilbData.SelectionMode == SelectionMode.MultiExtended)
            {
                _bOk_Click(this, null);
            }
        }

        private void ListboxFormAdd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void _eFilter_KeyUp(object sender, KeyEventArgs e)
        {
            ShowData(_eFilter.Text);
        }

        private void _icbObjectTypeFilter_SelectedValueChanged(object sender, EventArgs e)
        {
            ShowData(_eFilter.Text);
            _ilbData.Focus();
        }

        private void _ilbData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_ilbData.SelectionMode == SelectionMode.One)
            {
                if (_ilbData.SelectedItemObject != null)
                {
                    _outObject = _ilbData.SelectedItemObject;
                    Close();
                }
            }
            if (_ilbData.SelectionMode == SelectionMode.MultiExtended)
            {
                _bOk_Click(this, null);
            }
        }
    }
}
