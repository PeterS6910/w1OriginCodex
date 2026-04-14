using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Cgp.Components;
using Contal.IwQuick.Localization;
using System.Reflection;
using Contal.Cgp.Globals;
using System.Collections;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.Cgp.Components
{
    public partial class CgpDataGridView : UserControl
    {
        private const string COLUMN_SYMBOL_NAME = "Symbol";
        private const string COLUMN_DELETE_ACTION = "Delete";
        private const string COLUMN_EDIT_ACTION = "Edit";

        private readonly SyncDictionary<string, Image> _imageList = new SyncDictionary<string, Image>();
        private Dictionary<string, int> _columnsVisibilityOrder;
        private LocalizationHelper _localizationHelper;
        private bool _columnsPropertiesSet;
        private bool _allwaysRefreshOrder;
        private bool _columnsResized;
        private string _imageColumnName = COLUMN_SYMBOL_NAME;
        private readonly HashSet<int> _selectedRowsIndex = new HashSet<int>();
        private List<int> _lastSelectedIndexes; 
        public bool AllowedActionButtonClick { get; private set; }
        private ICgpDataGridView _actions;
        private Panel _controlsPanel;
        private bool _draging;
        private int _lastLeft, _lastTop;
        private bool _enabledDeleteButton = true;
        private bool _enabledInsertButton = true;
        private bool _smallSizeInsertButton;
        private bool _autoOpenEditFormByDoubleClick = true;
        private readonly SyncDictionary<string, Func<int, bool>> _controlColumns =
            new SyncDictionary<string, Func<int, bool>>();

        public bool CopyOnRightClick { get; set; }
        public HashSet<string> DisabledDoubleClickEventForColumns { get; private set; }
        public delegate void GridModifiedEventHalndler(BindingSource bindingSource);
        public event GridModifiedEventHalndler BeforeGridModified;
        public event GridModifiedEventHalndler AfterGridModified;
        public delegate void DataSorted(IList dataSourceList);
        public event DataSorted AfterSort;

        public int CurrentRowIndex
        {
            get
            {
                return _dgvData != null && _dgvData.CurrentRow != null ? _dgvData.CurrentRow.Index : -1;
            }
        }

        public LocalizationHelper LocalizationHelper
        {
            get { return _localizationHelper; }
            set { _localizationHelper = value; }
        }

        public ImageList ImageList
        {
            set
            {
                if (value != null)
                {
                    var oldImages = _imageList.ValuesSnapshot;

                    _imageList.Clear();

                    foreach (var image in oldImages)
                    {
                        image.Dispose();
                    }

                    foreach (var imageKey in value.Images.Keys)
                    {
                        _imageList.Add(imageKey, value.Images[imageKey]);
                    }
                }
            }
        }

        public ICgpDataGridView CgpDataGridEvents
        {
            set { _actions = value; }
            get { return _actions; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool EnabledDeleteButton 
        {
            set
            {
                _enabledDeleteButton = value;
                //ReInitControlsColumn();
            }
            get { return _enabledDeleteButton; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool EnabledInsertButton
        {
            set
            {
                _enabledInsertButton = value;

                if (_controlsPanel != null)
                {
                    _controlsPanel.Visible = _enabledInsertButton;
                }
                else if (_enabledInsertButton)
                {
                    InitControlsPanel();
                }
            }
            get { return _enabledInsertButton; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool SmallSizeInsertButton
        {
            get { return _smallSizeInsertButton; }
            set
            {
                _smallSizeInsertButton = value;

                if (_controlsPanel != null)
                {
                    Parent.Controls.Remove(_controlsPanel);
                    _controlsPanel = null;
                }

                InitControlsPanel();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool AutoOpenEditFormByDoubleClick
        {
            set { _autoOpenEditFormByDoubleClick = value; }
            get { return _autoOpenEditFormByDoubleClick; }
        }

        public bool AllwaysRefreshOrder
        {
            get { return _allwaysRefreshOrder; }
            set { _allwaysRefreshOrder = value; }
        }

        public DataGridViewSelectedRowCollection SelectedRows
        {
            get
            {
                return (_dgvData == null ? null : _dgvData.SelectedRows);
            }
        }

        public string DefaultSortColumnName { get; set; }

        public ListSortDirection DefaultSortDirection { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DataGridView DataGrid
        {
            get { return _dgvData; }
            set { _dgvData = value; }
        }

        public CgpDataGridView()
        {
            DisabledDoubleClickEventForColumns = new HashSet<string>();
            InitDataGridView();
        }

        private void InitDataGridView()
        {
            InitializeComponent();
            SetDoubleBuffered();
            CopyOnRightClick = true;

            _dgvData.RowsDefaultCellStyle.BackColor = Color.White;
            _dgvData.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
            _dgvData.CellBorderStyle = DataGridViewCellBorderStyle.None;
            _dgvData.SelectionChanged += _dgvData_SelectionChanged;
            _dgvData.MouseUp += _dgvData_MouseUp;
            _dgvData.MouseDown += _dgvData_MouseDown;
            _dgvData.SizeChanged += _dgvData_SizeChanged;
            _dgvData.DataError += _dgvData_DataError;
            _dgvData.MouseDoubleClick += _dgvData_MouseDoubleClick;
            _dgvData.MouseMove += _dgvData_MouseMove;
            _dgvData.DataSourceChanged += _dgvData_DataSourceChanged;

            _dgvData.EnableHeadersVisualStyles = false;
            _dgvData.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            _dgvData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _dgvData.AutoResizeColumnHeadersHeight();
        }

        private void ReInitControlsColumn()
        {
            if (_dgvData.Columns.Contains(COLUMN_DELETE_ACTION))
                _dgvData.Columns.Remove(COLUMN_DELETE_ACTION);

            if (_dgvData.Columns.Contains(COLUMN_EDIT_ACTION))
                _dgvData.Columns.Remove(COLUMN_EDIT_ACTION);

            InitControlsColumn();
        }

        void _dgvData_DataSourceChanged(object sender, EventArgs e)
        {
            if (_actions == null)
                return;

            InitControlsColumn();
            InitControlsPanel();

            if (_columnsPropertiesSet)
                SetColumnOrder();
        }

        public void AddControlColumn(string columnName)
        {
            AddControlColumn(
                columnName,
                rowIndex =>
                    true);
        }

        public void AddControlColumn(
            string columnName,
            [NotNull] Func<int, bool> isConrolColumn)
        {
            _controlColumns.Add(
                columnName,
                isConrolColumn);
        }

        void _dgvData_MouseMove(object sender, MouseEventArgs e)
        {
            var hitTest = _dgvData.HitTest(e.X, e.Y);

            if (hitTest.RowIndex > -1
                && _dgvData.Rows[hitTest.RowIndex].Selected)
            {
                string columnName = _dgvData.Columns[hitTest.ColumnIndex].Name;

                var neededCursor = columnName == COLUMN_DELETE_ACTION
                                  || columnName == COLUMN_EDIT_ACTION
                                  || IsControlColumn(
                                      columnName,
                                      hitTest.RowIndex)
                    ? Cursors.Hand
                    : Cursors.Default;

                if (_dgvData.Cursor != null
                    && _dgvData.Cursor != neededCursor)
                {
                    _dgvData.Cursor = neededCursor;
                }
            }
            else if (_dgvData.Cursor != null
                     && _dgvData.Cursor != Cursors.Default)
            {
                _dgvData.Cursor = Cursors.Default;
            }

            if (_controlsPanel == null)
                return;

            _controlsPanel.Enabled = true;

            foreach (Control control in _controlsPanel.Controls)
                control.Enabled = true;

            int panelOffset = _smallSizeInsertButton
                ? 7
                : 15;

            if (e.Y < Height/2)
            {
                if (_controlsPanel.Top > Height/2)
                    _controlsPanel.Top = Top + panelOffset;
            }
            else if (_controlsPanel.Top <= Height / 2)
            {
                _controlsPanel.Top = Top + Height - _controlsPanel.Height - panelOffset;
            }
        }

        private bool IsControlColumn(
            string columnName,
            int rowIndex)
        {
            Func<int, bool> isControlColumn;

            if (!_controlColumns.TryGetValue(
                columnName,
                out isControlColumn))
            {
                return false;
            }

            return isControlColumn(rowIndex);
        }

        void _dgvData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!_autoOpenEditFormByDoubleClick)
                return;

            var hitTest = _dgvData.HitTest(e.X, e.Y);

            if (_actions == null
                || hitTest.ColumnIndex < 0
                || hitTest.RowIndex <0)
            {
                return;
            }

            string columnName = _dgvData.Columns[hitTest.ColumnIndex].Name;

            if (columnName != COLUMN_DELETE_ACTION
                && columnName != COLUMN_EDIT_ACTION
                && !DisabledDoubleClickEventForColumns.Contains(columnName))
            {
                _actions.EditClick();
            }
        }

        void _dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {            
        }

        void _dgvData_SizeChanged(object sender, EventArgs e)
        {
            SetPositionForControlsPanel();
        }

        void _dgvData_MouseDown(object sender, MouseEventArgs e)
        {
            _lastSelectedIndexes = _dgvData.SelectedRows.OfType<DataGridViewRow>().Select(row => row.Index).ToList();

            var hitTest = _dgvData.HitTest(e.X, e.Y);

            if (hitTest.ColumnIndex >= 0
                && hitTest.RowIndex >= 0)
            {
                AllowedActionButtonClick = _dgvData.Rows[hitTest.RowIndex].Selected;
            }
        }

        void _dgvData_MouseUp(object sender, MouseEventArgs e)
        {
            if (AllowedActionButtonClick
                && _actions != null)
            {
                var hitTest = _dgvData.HitTest(e.X, e.Y);

                if (!_selectedRowsIndex.Contains(hitTest.RowIndex))
                    return;

                switch (_dgvData.Columns[hitTest.ColumnIndex].Name)
                {
                    case COLUMN_DELETE_ACTION:

                        if (_lastSelectedIndexes.Count > 1)
                        {
                            _actions.DeleteClick(_lastSelectedIndexes);
                        }
                        else
                            _actions.DeleteClick();

                        break;

                    case COLUMN_EDIT_ACTION:

                        if (_lastSelectedIndexes.Count > 1)
                        {
                            _actions.EditClick(_lastSelectedIndexes);
                        }
                        else
                            _actions.EditClick();
                        
                        break;

                    default:
                        return;
                }
            }
        }

        void _dgvData_SelectionChanged(object sender, EventArgs e)
        {
            foreach (int index in _selectedRowsIndex)
            {
                if (_dgvData.Rows.Count > index)
                {
                    if (_dgvData.Columns.Contains(COLUMN_DELETE_ACTION))
                        _dgvData.Rows[index].Cells[COLUMN_DELETE_ACTION] = new DataGridViewTextBoxCell();

                    if (_dgvData.Columns.Contains(COLUMN_EDIT_ACTION))
                        _dgvData.Rows[index].Cells[COLUMN_EDIT_ACTION] = new DataGridViewTextBoxCell();
                }
            }

            _selectedRowsIndex.Clear();

            SetActionsButtons(COLUMN_DELETE_ACTION);
            SetActionsButtons(COLUMN_EDIT_ACTION);
        }

        private Image GetCellImage(string columnName)
        {
            Image img = null;

            switch (columnName)
            {
                case COLUMN_DELETE_ACTION:
                    img = Resource.Delete;
                    break;

                case COLUMN_EDIT_ACTION:
                    img = Resource.Edit;
                    break;
            }

            return img;
        }

        private string GetTitleText(string columnName)
        {
            string title = string.Empty;

            switch (columnName)
            {
                case COLUMN_DELETE_ACTION:
                    title = _localizationHelper.GetString("General_bDelete");
                    break;

                case COLUMN_EDIT_ACTION:
                    title = _localizationHelper.GetString("General_bEdit");
                    break;
            }

            return title;
        }

        private void SetActionsButtons(string columnName)
        {
            if (!_dgvData.Columns.Contains(columnName))
                return;

            var img = GetCellImage(columnName);

            if (img == null)
                return;

            foreach (DataGridViewRow selectedRow in _dgvData.SelectedRows)
            {
                var imgCell = new DataGridViewImageCell
                {
                    Value = img,
                    ToolTipText = GetTitleText(columnName)
                };

                _dgvData.Rows[selectedRow.Index].Cells[columnName] = imgCell;

                if (!_selectedRowsIndex.Contains(selectedRow.Index))
                    _selectedRowsIndex.Add(selectedRow.Index);
            }
        }

        protected void SetDoubleBuffered()
        {
            try
            {
                Type dgvType = _dgvData.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(_dgvData, true, null);
            }
            catch
            { }
        }

        /// <summary>
        /// Arrange columns visibility and order by column name index
        /// </summary>
        /// <param name="columnsNames"></param>
        private void SetColumnsVisibilityAndOrder(params string[] columnsNames)
        {
            if (ColumnPropertiesSet())
                return;

            _columnsVisibilityOrder = new Dictionary<string, int>();

            // Columns Delete and Edit are special - always visible and first
            if (_enabledDeleteButton)
                _columnsVisibilityOrder.Add(COLUMN_DELETE_ACTION, 0);
            if (_enabledInsertButton)
                _columnsVisibilityOrder.Add(COLUMN_EDIT_ACTION, _enabledDeleteButton ? 1 : 0);

            for (int i = 0; i < columnsNames.Length; i++)
            {
                _columnsVisibilityOrder.Add(columnsNames[i], _enabledDeleteButton ? (i+2) : (i+1));
            }

            int displayIndex = 0;
            foreach(DataGridViewColumn column in _dgvData.Columns)
            {
                column.Visible = false;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.DisplayIndex = displayIndex++;
            }

            // Set Columns order
            foreach (var column in _columnsVisibilityOrder)
            {
                try
                {
                    _dgvData.Columns[column.Key].DisplayIndex = column.Value;
                    _dgvData.Columns[column.Key].Visible = true;
                }
                catch(Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Column " + column.Key + " not found!");
                }
            }

            // Last column is always set to Fill and Symbol to ColumnHeader
            if (_dgvData.Columns.Contains(COLUMN_SYMBOL_NAME))
                _dgvData.Columns[COLUMN_SYMBOL_NAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            _dgvData.Columns[_columnsVisibilityOrder.Keys.Last()].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _columnsPropertiesSet = true;
        }

        /// <summary>
        /// Checks if properties are set. Used to remove unnecessary dataGridView redraws.
        /// </summary>
        /// <returns></returns>
        private bool ColumnPropertiesSet()
        {
            if (_columnsPropertiesSet)
            {
                if (_allwaysRefreshOrder)
                    SetColumnOrder();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Its used to set correct order when binding source automaticaly change dataGridView columns order when added as data source. 
        /// </summary>
        private void SetColumnOrder()
        {
            foreach (var column in _columnsVisibilityOrder)
            {
                _dgvData.Columns[column.Key].DisplayIndex = column.Value;
                _dgvData.Columns[column.Key].Visible = true;
            }
        }

        /// <summary>
        /// Removes dataGridView datasource. 
        /// IMPORTANT TO USE THIS METHOD WHEN REMOVING DATASOURCE IN Cgp.CLient AND Cgp.NCAS.Client FORMS !!!
        /// </summary>
        public void RemoveDataSource()
        {
            _columnsResized = false;
            _columnsPropertiesSet = false;
            _dgvData.DataSource = null;
        }

        /// <summary>
        /// Add values from bindingSource to datagrid. Only columns that are in columnNames parameter are shown.
        /// Columns are ordered by their position in columnsNames.
        /// </summary>
        /// <param name="bindingSource"></param>
        /// <param name="showObjectSymbols">If true object symbol are shown</param>
        /// <param name="columnsWidths"></param>
        /// <param name="columnsNames">Columns to be shown</param>
        public void ModifyGridView(BindingSource bindingSource, params string[] columnsNames)
        {
            ModifyGridView(string.Empty, bindingSource, columnsNames);
        }

        /// <summary>
        /// Add values from bindingSource to datagrid. Only columns that are in columnNames parameter are shown.
        /// Columns are ordered by their position in columnsNames.
        /// </summary>
        /// <param name="bindingSource"></param>
        /// <param name="showObjectSymbols">If true object symbol are shown</param>
        /// <param name="columnsWidths"></param>
        /// <param name="columnsNames">Columns to be shown</param>
        public void ModifyGridView(string imageColumnName, BindingSource bindingSource, params string[] columnsNames)
        {
            try
            {
                _dgvData.SuspendLayout();

                _imageColumnName = (imageColumnName == string.Empty ? COLUMN_SYMBOL_NAME : imageColumnName);
                //remember last position in DataGridView _dgvData
                int lastPosition = _dgvData.FirstDisplayedScrollingRowIndex;

                if (BeforeGridModified != null)
                    BeforeGridModified(bindingSource);

                var sortedColumn = _dgvData.SortedColumn;
                var sortOrder = _dgvData.SortOrder;

                _dgvData.DataSource = bindingSource;

                SetColumnsVisibilityAndOrder(columnsNames);

                if (_localizationHelper != null)
                    _localizationHelper.TranslateDataGridViewColumnsHeaders(_dgvData);

                if (AfterGridModified != null)
                    AfterGridModified(bindingSource);

                bool defaultSortApplied = false;
                if (sortedColumn == null && _dgvData.SortedColumn == null && DefaultSortColumnName != null)
                {
                    _dgvData.Sort(_dgvData.Columns[DefaultSortColumnName], DefaultSortDirection);
                    defaultSortApplied = true;
                }

                if (bindingSource.SupportsSorting)
                {
                    if (sortedColumn == null)
                    {
                        if (!defaultSortApplied)
                            DoImplicitSort();
                    }
                    else
                        _dgvData.Sort(_dgvData.Columns[sortedColumn.Name],
                            sortOrder == SortOrder.Descending
                                ? ListSortDirection.Descending
                                : ListSortDirection.Ascending);
                }

                try
                {
                    if (lastPosition >= 0)
                        //set same position as previous
                        _dgvData.FirstDisplayedScrollingRowIndex = lastPosition;
                }
                catch
                {
                }

                if (!_columnsResized)
                {
                    _columnsResized = true;
                    _dgvData.AutoResizeColumns();
                }
                _dgvData.ResumeLayout();
            }
            catch
            {
            }
        }

        private void InitControlsColumn()
        {
            int columnIndex = 0;

            if (_enabledDeleteButton)
            {
                InsertControlColumn(COLUMN_DELETE_ACTION, columnIndex);
                _dgvData.Columns[COLUMN_DELETE_ACTION].DefaultCellStyle.NullValue = null;
                columnIndex++;
            }

            InsertControlColumn(COLUMN_EDIT_ACTION, columnIndex);
            _dgvData.Columns[COLUMN_EDIT_ACTION].DefaultCellStyle.NullValue = null;

            _dgvData_SelectionChanged(null, null);
        }

        private void InsertControlColumn(string columnName, int displayIndex)
        {
            if (_dgvData.Columns.Contains(columnName))
            {
                _dgvData.Columns.Remove(columnName);
                _selectedRowsIndex.Clear();
            }

            _dgvData.Columns.Add(columnName, string.Empty);
            _dgvData.Columns[columnName].DisplayIndex = displayIndex;
            _dgvData.Columns[columnName].Resizable = DataGridViewTriState.False;
            _dgvData.Columns[columnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            _dgvData.Columns[columnName].ReadOnly = true;
            _dgvData.Columns[columnName].MinimumWidth = 19;
            _dgvData.Columns[columnName].Width = 19;
            _dgvData.Columns[columnName].Visible = true;
        }

        private void InitControlsPanel()
        {
            if (!_enabledInsertButton || _controlsPanel != null)
                return;

            int size = _smallSizeInsertButton
                ? 25
                : 50;

            _controlsPanel = new Panel();
            _controlsPanel.Size = new Size(size, size);

            SetPositionForControlsPanel();

            _controlsPanel.BackColor = Color.WhiteSmoke;
            Parent.Controls.Add(_controlsPanel);
            _controlsPanel.BringToFront();
            _controlsPanel.MouseDown += _controlsPsnel_MouseDown;
            _controlsPanel.MouseUp += _controlsPsnel_MouseUp;
            _controlsPanel.MouseMove += _controlsPsnel_MouseMove;

            var insertButton = new Button();
            insertButton.FlatStyle = FlatStyle.Flat;
            insertButton.FlatAppearance.BorderSize = 0;
            insertButton.Size = new Size(size, size);
            insertButton.Location = new Point(0, 0);
            insertButton.BackgroundImage = _smallSizeInsertButton 
                ? Resource.smallInsert 
                : Resource.Insert;
            insertButton.Click += insertButton_Click;
            _controlsPanel.Controls.Add(insertButton);
        }

        void insertButton_Click(object sender, EventArgs e)
        {
            if (_actions != null)
                _actions.InsertClick();
        }

        private void SetPositionForControlsPanel()
        {
            if (_controlsPanel == null)
                return;

            int left = Left + Width - _controlsPanel.Width - 22;
            int top = Top + Height - _controlsPanel.Height - (_smallSizeInsertButton ? 7 : 15);
            _controlsPanel.Location = new Point(left, top);
        }

        void _controlsPsnel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_draging)
                return;

            var deltaX = Cursor.Position.X - _lastLeft;
            var deltaY = Cursor.Position.Y - _lastTop;
            var newLeft = _controlsPanel.Left + deltaX;
            var newTop = _controlsPanel.Top + deltaY;

            if (deltaX != 0 && newLeft >= 0 && newLeft + _controlsPanel.Width <= Left + Width)
                _controlsPanel.Left = newLeft;

            if (deltaY != 0 && newTop >= 0 && _controlsPanel.Top + deltaY + _controlsPanel.Height <= Top + Height)
                _controlsPanel.Top = newTop;

            _lastLeft = Cursor.Position.X;
            _lastTop = Cursor.Position.Y;
        }

        void _controlsPsnel_MouseUp(object sender, MouseEventArgs e)
        {
            _draging = false; 
        }

        void _controlsPsnel_MouseDown(object sender, MouseEventArgs e)
        {
            _draging = true;
            _lastLeft = Cursor.Position.X;
            _lastTop = Cursor.Position.Y;  
        }

        private void DoImplicitSort()
        {
            DataGridViewColumn sortColumn = null;
            foreach (DataGridViewColumn column in _dgvData.Columns)
            {
                if (!column.Visible || column.SortMode == DataGridViewColumnSortMode.NotSortable || column.SortMode == DataGridViewColumnSortMode.Programmatic || column.IsDataBound == false)
                    continue;

                if (sortColumn == null || column.DisplayIndex < sortColumn.DisplayIndex)
                    sortColumn = column;
            }

            if (sortColumn != null)
                _dgvData.Sort(sortColumn, ListSortDirection.Ascending);

        }

        private void _dgvData_Sorted(object sender, EventArgs e)
        {
            if (AfterSort != null)
                try
                {
                    AfterSort((_dgvData.DataSource as BindingSource).List);
                }
                catch { }
        }

        /// <summary>
        /// Update dataGridView value at specific row, even if binding is used.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="visible"></param>
        public void UpdateVisibility(int rowIndex, bool visible)
        {
            if (_dgvData.Rows[rowIndex].Visible != visible)
            {

                BindingManagerBase currencyManager;
                try
                {
                    currencyManager = BindingContext[_dgvData.DataSource];
                }
                catch
                {
                    currencyManager = null;
                }

                if (currencyManager != null)
                {
                    currencyManager.SuspendBinding();
                    _dgvData.Rows[rowIndex].Visible = visible;
                    currencyManager.ResumeBinding();
                }
                else
                    _dgvData.Rows[rowIndex].Visible = visible;
            }
        }

        /// <summary>
        /// Update dataGridView value at specific cell.   
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void UpdateValue(int rowIndex, string columnName, object value)
        {
            try
            {
                bool wasVisible = _dgvData.Rows[rowIndex].Visible;
                
                _dgvData.Rows[rowIndex].Cells[columnName].Value = value;
             
                // Restore old visible state because of DataGridView visible state change on value update
                UpdateVisibility(rowIndex, wasVisible);
            }
            catch { }
        }

        /// <summary>
        /// Update dataGridView value at specific cell and change cell text color.   
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void UpdateValueTextColor(int rowIndex, string columnName, object value, Color color)
        {
            try
            {
                bool wasVisible = _dgvData.Rows[rowIndex].Visible;

                _dgvData.Rows[rowIndex].Cells[columnName].Value = value;
                _dgvData.Rows[rowIndex].Cells[columnName].Style.ForeColor = color;

                // Restore old visible state because of DataGridView visible state change on value update
                UpdateVisibility(rowIndex, wasVisible);
            }
            catch { }
        }

        public void UpdateImage(IShortObject shortObject, int rowIndex, object value)
        {
            try
            {
                bool wasVisible = _dgvData.Rows[rowIndex].Visible;

                _dgvData.Rows[rowIndex].Cells[_imageColumnName].Value =
                    GetSubTypeImage(shortObject, value);

                // Restore old visible state because of DataGridView visible state change on value update
                UpdateVisibility(rowIndex, wasVisible);
            }
            catch { }
        }

        /// <summary>
        /// Update object symbol at specific row
        /// </summary>
        /// <param name="shortObject"></param>
        /// <param name="value"></param>
        public Image GetSubTypeImage(IShortObject shortObject, object value)
        {
            Image image;

            return 
                _imageList.TryGetValue(
                        GetSubTypeImageKey(shortObject, value), 
                        out image) 
                    ? image
                    : null;
        }

        /// <summary>
        /// Update object symbol at specific row using defult image by object type
        /// </summary>
        /// <param name="shortObject"></param>
        public Image GetDefaultImage(IShortObject shortObject)
        {
            Image image;
            if (_imageList.TryGetValue(GetDefaultImageKey(shortObject), out image))
                return image;

            return null;
        }

        /// <summary>
        /// Update object symbol at specific row using defult image by object type
        /// </summary>
        /// <param name="shortObject"></param>
        /// <param name="rowIndex"></param>
        public Image GetDefaultImage(string imageKey, int rowIndex)
        {
            Image image;
            if (_imageList.TryGetValue(imageKey, out image))
                return image;

            return null;
        }

        private string GetDefaultImageKey(IShortObject shortObject)
        {
            try
            {
                return shortObject.ObjectType.ToString();
            }
            catch { }

            return string.Empty;
        }


        private static string GetSubTypeImageKey(IShortObject shortObject, object value)
        {
            try
            {
                return shortObject.GetSubTypeImageString(value);
            }
            catch { }

            return string.Empty;
        }

        private void _dgvData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && CopyOnRightClick)
            {
                var hitTest = _dgvData.HitTest(e.X, e.Y);

                foreach (DataGridViewRow row in _dgvData.SelectedRows)
                    row.Selected = false;

                if (hitTest.RowIndex > -1)
                {
                    _dgvData.Rows[hitTest.RowIndex].Selected = true;
                    object textToClipboard = _dgvData[hitTest.ColumnIndex, hitTest.RowIndex].Value;

                    if (!string.IsNullOrEmpty(textToClipboard as string) ||
                        textToClipboard is DateTime)
                    {
                        Clipboard.SetText(textToClipboard.ToString());
                    }
                }
            }
        }
    }

    public interface ICgpDataGridView
    {
        void EditClick();
        void EditClick(ICollection<int> indexes);
        void DeleteClick();
        void DeleteClick(ICollection<int> indexes);
        void InsertClick();
    }
}
