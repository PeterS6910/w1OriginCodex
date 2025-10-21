using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class CSVImportDialog :
#if DESIGNER
        Form
#else
 CgpTranslateForm
#endif
    {
        private const int IMPORT_COLUMN_COUNT = 17;
        private const int PERSONAL_ID_COLUMN_INDEX = 8;

        private readonly List<List<string>> _csvTable = new List<List<string>>();
        private readonly Action<Guid, int> _importedPersonCountChanged;
        private readonly Guid _formIdentification = Guid.NewGuid();
        private bool _isRunningImport;
        private StructuredSubSite _selectedSubSite;
        private bool _isSelectedSubSite;

        public CSVImportDialog()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();

            _dgCSVImportColumns.Rows.Add();

            var separators = new List<Separators>();
            separators.Add(new Separators("TAB", '\t'));
            separators.Add(new Separators(";", ';'));
            _cbSeparator.Items.AddRange(separators.ToArray());
            _cbSeparator.SelectedItem = separators[1];
            IntitDoubleBufferGrid(_dgCSVImportValues);
            LoadTemplates();
            _cbTemplate.Enabled = false;
            _cbDateFormat.Enabled = false;
            _cbImportType.Enabled = false;
            _cbParseBirthdateFromPersonId.Enabled = false;
            _bCreateTemplate.Enabled = false;
            _bUpdateTemplate.Enabled = false;
            _eDepartmentFolder.Text = "Departments";
            var dateFormat = new DateFormat("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss");
            _cbDateFormat.Items.Add(dateFormat);
            dateFormat = new DateFormat("yyyy-MM-dd", "yyyy-MM-dd");
            _cbDateFormat.Items.Add(dateFormat);
            dateFormat = new DateFormat("yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm");
            _cbDateFormat.Items.Add(dateFormat);
            var csvImportTypeItems = CSVImportTypeItem.GetAllImportTypeItem();
            _cbImportType.Items.AddRange(csvImportTypeItems.ToArray());

            _cbValidationType.Items.Clear();
            _cbValidationType.Items.Add(new ItemSeparatorType(CSVCardImportDialog.PERSONAL_ID_SEPARATOR_NORTH_STATES_NAME, GetString(CSVCardImportDialog.PERSONAL_ID_SEPARATOR_NORTH_STATES_NAME), CSVCardImportDialog.PERSONAL_ID_SEPARATOR_NORTH_STATES_SYMBOL));
            _cbValidationType.Items.Add(new ItemSeparatorType(CSVCardImportDialog.PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_NAME, GetString(CSVCardImportDialog.PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_NAME), CSVCardImportDialog.PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_SYMBOL));
            _cbValidationType.Items.Add(new ItemSeparatorType(CSVCardImportDialog.PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_NAME, GetString(CSVCardImportDialog.PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_NAME), CSVCardImportDialog.PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_SYMBOL));
            _cbValidationType.Items.Add(new ItemSeparatorType(CSVCardImportDialog.PERSONAL_ID_SEPARATOR_NON_STANDART, GetString(CSVCardImportDialog.PERSONAL_ID_SEPARATOR_NON_STANDART), CSVCardImportDialog.PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_SYMBOL));

            _cbAllowCompositPersonalId.Enabled = false;
            _cbValidationType.Enabled = false;

            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgCSVImportColumns);
            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgReport);
            _importedPersonCountChanged = new Action<Guid, int>(ImportedPersonCountChanged);
            ImportedPersonCountChangedHandler.Singleton.RegisterImportedPersonCountChanged(_importedPersonCountChanged);
            
            if (SelectStructuredSubSiteForm.OnlyOneSubSiteIsRelevantForLogin(out _selectedSubSite))
            {
                if (_selectedSubSite != null)
                    SetActSubSite(_selectedSubSite);
                else
                {
                    _tbmSubSite.Text = GetString("SelectStructuredSubSiteForm_RootNode");
                    _tbmSubSite.TextImage =
                            ObjectImageList.Singleton.GetImageForObjectType(ObjectType.StructuredSubSite);
                }

                _tbmSubSite.Enabled = false;
                _isSelectedSubSite = true;
            }
        }


        private void LoadTemplates()
        {
            LoadTemplates(string.Empty);
        }

        private void LoadTemplates(string actCSVImportSchemaName)
        {
            _cbTemplate.Items.Clear();
            Exception error;
            ICollection<CSVImportSchema> templates = CgpClient.Singleton.MainServerProvider.CSVImportSchemas.List(out error);
            if (templates != null && templates.Count > 0)
            {
                templates = templates.OrderBy(csvImportSchema => csvImportSchema.CSVImportSchemaName).ToList();

                foreach(CSVImportSchema csvImportSchema in templates)
                {
                    if (csvImportSchema != null)
                    {
                        CSVImportSchema actCSVImportSchema = CgpClient.Singleton.MainServerProvider.CSVImportSchemas.GetObjectById(csvImportSchema.IdCSVImportSchema);
                        if (actCSVImportSchema != null)
                        {
                            _cbTemplate.Items.Add(actCSVImportSchema);
                            if (actCSVImportSchema.ToString() == actCSVImportSchemaName)
                                _cbTemplate.SelectedItem = actCSVImportSchema;
                        }
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (_importedPersonCountChanged != null)
                ImportedPersonCountChangedHandler.Singleton.UnregisterImportedPersonCountChanged(_importedPersonCountChanged);
        }

        private void _bOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogCSVImport.ShowDialog() == DialogResult.OK)
            {
                SetDefaultHeaderCellsWidth();
                _eFilePath.Text = openFileDialogCSVImport.FileName;
                LoadCSVFile();
            }
        }

        private void SetDefaultHeaderCellsWidth()
        {
            foreach (DataGridViewColumn column in _dgCSVImportColumns.Columns)
            {
                column.Width = column.MinimumWidth;
            }
        }

        private void LoadCSVFile()
        {
            Separators sepatator = _cbSeparator.SelectedItem as Separators;
            if (sepatator == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbSeparator,
                    GetString("ErrorEntrySeparator"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                return;
            }

            string filePath = _eFilePath.Text;
            if (filePath == null || filePath == string.Empty)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eFilePath,
                    GetString("ErrorEntryFilePath"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                return;
            }

            if (!File.Exists(filePath))
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eFilePath,
                    GetString("ErrorFileNotFound"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                return;
            }

            StreamReader csvStream = null;
            try
            {
                _csvTable.Clear();
                csvStream = new StreamReader(filePath, Encoding.Default);

                while (!csvStream.EndOfStream)
                {
                    string rowFromCSVFile = csvStream.ReadLine();
                    List<string> columns = ParseRow(rowFromCSVFile, sepatator.Sepearator);
                    int actRow = 0;
                    foreach (string column in columns)
                    {
                        if (actRow >= _csvTable.Count)
                        {
                            _csvTable.Add(new List<string>());
                        }

                        _csvTable[actRow].Add(column);
                        actRow++;
                    }
                }
            }
            catch
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne,
                    _eFilePath,
                    GetString("ErrorCannotReadFromFile"), Contal.IwQuick.UI.ControlNotificationSettings.Default);

                return;
            }
            finally
            {
                try
                {
                    if (csvStream != null)
                    {
                        csvStream.Close();
                    }
                }
                catch
                {
                }
            }

            SafeThread.StartThread(AddLinesToPreview);

            if (_dgCSVImportColumns.Rows.Count > 0)
            {
                DataGridViewRow row = _dgCSVImportColumns.Rows[0];
                foreach (DataGridViewCell dataGridViewCell in row.Cells)
                {
                    if (dataGridViewCell is DataGridViewComboBoxCell)
                    {
                        DataGridViewComboBoxCell cell = dataGridViewCell as DataGridViewComboBoxCell;
                        if (cell != null)
                        {
                            cell.ValueMember = "Name";
                        }
                    }
                    else if (dataGridViewCell is DataGridViewCheckBoxCell)
                    {
                        DataGridViewCheckBoxCell cell = dataGridViewCell as DataGridViewCheckBoxCell;
                        if (cell != null)
                            cell.Value = true;
                    }
                }
            }

            FillComboBoxItems();

            _cbTemplate.Enabled = true;
            _cbDateFormat.Enabled = true;
            _cbImportType.Enabled = true;
            _cbImportType.SelectedItem = null;
            _cbParseBirthdateFromPersonId.Enabled = true;
            _bImport.Enabled = true;
            _bCreateTemplate.Enabled = true;
            _cbAllowCompositPersonalId.Enabled = true;
            _cbValidationType.Enabled = true;

            CSVImportSchema template = _cbTemplate.SelectedItem as CSVImportSchema;

            if (template == null)
            {
                _cbDateFormat.SelectedItem = null;
                _cbParseBirthdateFromPersonId.Checked = false;
                _dgCSVImportValues.DataSource = new List<TableRow>();
            }
            else
            {
                _cbTemplate_SelectedValueChanged(null, null);
                _bUpdateTemplate.Enabled = true;
            }
        }

        private List<int> GetSelectedColumns()
        {
            List<int> selectedColumns = new List<int>();

            if (_dgCSVImportColumns.Rows.Count > 0)
            {
                foreach (DataGridViewCell dataGridViewCell in _dgCSVImportColumns.Rows[0].Cells)
                {
                    DataGridViewComboBoxCell cell = dataGridViewCell as DataGridViewComboBoxCell;
                    if (cell != null && cell.Items != null)
                    {
                        foreach (object obj in cell.Items)
                        {
                            if (obj != null && obj.ToString() == cell.EditedFormattedValue.ToString())
                            {
                                ColumnFromCSVFile columnFromCSVFile = obj as ColumnFromCSVFile;
                                if (columnFromCSVFile != null && columnFromCSVFile.FirstColumn != -1)
                                {
                                    selectedColumns.Add(columnFromCSVFile.FirstColumn);
                                }
                                break;
                            }
                        }
                    }
                }
            }

            ColumnFromCSVFile composidIDCsvColumn = _cbPersonalIdFirstColumn.SelectedItem as ColumnFromCSVFile;
            if (composidIDCsvColumn != null)
                selectedColumns.Add(composidIDCsvColumn.FirstColumn);

            composidIDCsvColumn = _cbPersonalIdSecondColumn.SelectedItem as ColumnFromCSVFile;
            if (composidIDCsvColumn != null)
                selectedColumns.Add(composidIDCsvColumn.FirstColumn);

            return selectedColumns;
        }

        private void FillComboBoxItems()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(FillComboBoxItems));
            }
            else
            {
                List<ColumnFromCSVFile> items = new List<ColumnFromCSVFile>();

                ColumnFromCSVFile defaultColumnFromCSVFile = new ColumnFromCSVFile(-1);
                items.Add(defaultColumnFromCSVFile);

                for (int actColumn = 0; actColumn < _csvTable.Count; actColumn++)
                {
                    ColumnFromCSVFile columnFromCSVFile = new ColumnFromCSVFile(actColumn);
                    items.Add(columnFromCSVFile);
                }

                List<int> selectedItems = GetSelectedColumns();

                _enableColumnChanged = false;
                DataGridViewRow row = _dgCSVImportColumns.Rows[0];
                foreach (DataGridViewCell dataGridViewCell in row.Cells)
                {

                    if (dataGridViewCell is DataGridViewComboBoxCell)
                    {
                        DataGridViewComboBoxCell cell = dataGridViewCell as DataGridViewComboBoxCell;
                        if (cell != null)
                        {
                            if (_cbAllowCompositPersonalId.Checked && cell.ColumnIndex == PERSONAL_ID_COLUMN_INDEX)
                            {
                                cell.Value = null;
                                cell.Items.Clear();
                            }
                            else
                            {
                                ColumnFromCSVFile selectedItem;
                                List<ColumnFromCSVFile> newItems = FillOneComboBoxItems(cell.EditedFormattedValue.ToString(), items, out selectedItem, selectedItems);

                                cell.Value = null;
                                cell.Items.Clear();
                                cell.Items.AddRange(newItems.ToArray());
                                cell.Value = selectedItem;
                                cell.ValueMember = "Name";
                            }
                        }
                    }
                }

                ColumnFromCSVFile composidIDSelectedItem = _cbPersonalIdFirstColumn.SelectedItem as ColumnFromCSVFile;
                string strSelectedItem = string.Empty;
                if (composidIDSelectedItem != null)
                    strSelectedItem = composidIDSelectedItem.ToString();

                List<ColumnFromCSVFile> composidIDNewItems = FillOneComboBoxItems(strSelectedItem, items, out composidIDSelectedItem, selectedItems);

                _cbPersonalIdFirstColumn.Items.Clear();
                _cbPersonalIdFirstColumn.Items.AddRange(composidIDNewItems.ToArray());
                _cbPersonalIdFirstColumn.SelectedItem = composidIDSelectedItem;

                composidIDSelectedItem = _cbPersonalIdSecondColumn.SelectedItem as ColumnFromCSVFile;
                strSelectedItem = string.Empty;
                if (composidIDSelectedItem != null)
                    strSelectedItem = composidIDSelectedItem.ToString();

                composidIDNewItems = FillOneComboBoxItems(strSelectedItem, items, out composidIDSelectedItem, selectedItems);

                _cbPersonalIdSecondColumn.Items.Clear();
                _cbPersonalIdSecondColumn.Items.AddRange(composidIDNewItems.ToArray());
                _cbPersonalIdSecondColumn.SelectedItem = composidIDSelectedItem;

                _enableColumnChanged = true;
            }
        }

        private List<ColumnFromCSVFile> FillOneComboBoxItems(string strSelectedItem, List<ColumnFromCSVFile> items, out ColumnFromCSVFile selectedItem, List<int> selectedItems)
        {
            selectedItem = null;
            List<ColumnFromCSVFile> newItems = new List<ColumnFromCSVFile>();

            foreach (ColumnFromCSVFile columnFromCSVFile in items)
            {
                if (columnFromCSVFile != null)
                {
                    if (columnFromCSVFile.FirstColumn == -1 || columnFromCSVFile.ToString() == strSelectedItem || !selectedItems.Contains(columnFromCSVFile.FirstColumn))
                    {
                        newItems.Add(columnFromCSVFile);
                        if (columnFromCSVFile.ToString() == strSelectedItem)
                            selectedItem = columnFromCSVFile;
                    }
                }
            }

            return newItems;
        }

        private void AddLinesToPreview()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(AddLinesToPreview));
            }
            else
            {
                try
                {
                    int rowCount = 3;

                    if (_csvTable.Count > 0)
                    {
                        if (rowCount > _csvTable[0].Count)
                            rowCount = _csvTable[0].Count;

                        _dgPrewiev.Rows.Clear();
                        _dgPrewiev.Columns.Clear();
                        for (int actRow = 0; actRow < rowCount; actRow++)
                        {
                            int columnCount = 0;
                            foreach (List<string> columns in _csvTable)
                            {
                                if (_dgPrewiev.Columns.Count < columnCount + 1)
                                    _dgPrewiev.Columns.Add("Column " + (columnCount + 1).ToString(), CgpClient.Singleton.LocalizationHelper.GetString("Column") + " " + (columnCount + 1).ToString());

                                if (_dgPrewiev.Rows.Count < actRow + 1)
                                    _dgPrewiev.Rows.Add();

                                _dgPrewiev[columnCount, actRow].Value = columns[actRow];

                                columnCount++;
                            }
                        }
                    }
                }
                catch { }
            }
        }



        private List<string> ParseRow(string row, char separator)
        {
            List<string> listRow = new List<string>();

            string[] columns = row.Split(separator);

            if (columns != null)
            {
                if (columns.Length > 0)
                {
                    foreach (string column in columns)
                    {
                        listRow.Add(column);
                    }
                }
            }

            return listRow;
        }

        private void _dgCSVImportValues_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                //_dgCSVImportColumns.SuspendLayout();

                if (_dgCSVImportColumns.Columns[_dgCSVImportColumns.ColumnCount - 1].Width > _dgCSVImportValues.Columns[_dgCSVImportColumns.ColumnCount - 1].Width)
                {
                    _dgCSVImportColumns.Columns[_dgCSVImportColumns.ColumnCount - 1].Width = _dgCSVImportValues.Columns[_dgCSVImportColumns.ColumnCount - 1].Width;
                }

                _dgCSVImportColumns.HorizontalScrollingOffset = e.NewValue;

                if (_dgCSVImportColumns.HorizontalScrollingOffset < e.NewValue)
                {
                    _dgCSVImportColumns.Columns[_dgCSVImportColumns.ColumnCount - 1].Width += e.NewValue - _dgCSVImportColumns.HorizontalScrollingOffset;
                    _dgCSVImportColumns.HorizontalScrollingOffset = e.NewValue;
                }

                //_dgCSVImportColumns.ResumeLayout();
            }
        }

        private void _dgCSVImportValues_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            _dgCSVImportColumns.Columns[e.Column.DisplayIndex].Width = e.Column.Width;
        }

        private void _dgCSVImportValues_Click(object sender, EventArgs e)
        {
            _dgCSVImportColumns.ClearSelection();
        }

        private void _dgCSVImportColumns_Click(object sender, EventArgs e)
        {
            _dgCSVImportValues.ClearSelection();
        }

        private void _dgCSVImportColumns_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox comboBox = e.Control as ComboBox;
            if (comboBox != null)
            {
                comboBox.SelectionChangeCommitted -= new EventHandler(ComboBox_SelectedIndexChanged);
                comboBox.SelectionChangeCommitted += new EventHandler(ComboBox_SelectedIndexChanged);

                e.CellStyle.BackColor = _dgCSVImportColumns.DefaultCellStyle.BackColor;
            }
        }

        private bool _enableColumnChanged = true;
        private ItemSeparatorType _actValidationType = null;
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == _cbValidationType)
                {
                    _actValidationType = _cbValidationType.SelectedItem as ItemSeparatorType;
                }
                
                if (_enableColumnChanged)
                {
                    _dgCSVImportColumns_ComboBoxValueChanged();
                }
            }
            catch { }
        }

        private Dictionary<int, CSVColumn> GetActSelectedColumns()
        {
            Dictionary<int, CSVColumn> dicActSelectedIndexes = new Dictionary<int, CSVColumn>();

            foreach (DataGridViewCell dataGridViewCell in _dgCSVImportColumns.Rows[0].Cells)
            {
                DataGridViewComboBoxCell cell = dataGridViewCell as DataGridViewComboBoxCell;
                if (cell != null && cell.Items != null)
                {
                    foreach (object obj in cell.Items)
                    {
                        if (obj != null && obj.ToString() == cell.EditedFormattedValue.ToString())
                        {
                            CSVColumn csvColumn = obj as CSVColumn;
                            if (csvColumn != null)
                            {
                                dicActSelectedIndexes.Add(cell.ColumnIndex, csvColumn);
                            }
                            break;
                        }
                    }
                }
            }

            return dicActSelectedIndexes;
        }

        private object _lockWriteDataToCSVImportValues = new object();
        private void _dgCSVImportColumns_ComboBoxValueChanged()
        {
            try
            {
                FillComboBoxItems();

                Dictionary<int, CSVColumn> dicActSelectedIndexes = GetActSelectedColumns();

                bool removeAllRows = true;

                ColumnFromCSVFile personalIdFirstColumn = _cbPersonalIdFirstColumn.SelectedItem as ColumnFromCSVFile;
                ColumnFromCSVFile personalIdSecondColumn = _cbPersonalIdSecondColumn.SelectedItem as ColumnFromCSVFile;

                if ((personalIdFirstColumn != null && personalIdFirstColumn.FirstColumn != -1)
                    || (personalIdSecondColumn != null && personalIdSecondColumn.FirstColumn != -1))
                    removeAllRows = false;

                if (removeAllRows && dicActSelectedIndexes != null && dicActSelectedIndexes.Count > 0)
                {
                    foreach (CSVColumn csvColumn in dicActSelectedIndexes.Values)
                    {
                        if (csvColumn != null && csvColumn.FirstColumn != -1)
                            removeAllRows = false;
                    }
                }

                if (removeAllRows)
                {
                    lock (_lockWriteDataToCSVImportValues)
                    {
                        int scrollPosition = _dgCSVImportValues.HorizontalScrollingOffset;
                        _dgCSVImportValues.DataSource = new List<TableRow>();
                        _dgCSVImportValues.HorizontalScrollingOffset = scrollPosition;
                    }
                }
                else
                {
                    _dgCSVImportValues.SuspendLayout();

                    int rowCount = _csvTable[0].Count;

                    _pbLoadFileImport.Maximum = rowCount;
                    _pbLoadFileImport.Value = 0;

                    SafeThread<CellValueChangedSettings>.StartThread(Do_dgCSVImportColumns_CellValueChanged,
                        new CellValueChangedSettings(dicActSelectedIndexes, rowCount, _cbAllowCompositPersonalId.Checked,
                            personalIdFirstColumn, personalIdSecondColumn));
                }
            }
            catch { }
        }

        private void Do_dgCSVImportColumns_CellValueChanged(CellValueChangedSettings cellValueChangedSettings)
        {
            lock (_lockWriteDataToCSVImportValues)
            {
                List<TableRow> tableRows = new List<TableRow>();
                List<TableRow> oldTableRows = _dgCSVImportValues.DataSource as List<TableRow>;

                try
                {
                    string[] tableRow = new string[IMPORT_COLUMN_COUNT];
                    for (int row = 0; row < cellValueChangedSettings.RowCount; row++)
                    {
                        for (int i = 0; i < tableRow.Length; i++)
                        {
                            tableRow[i] = string.Empty;
                        }

                        foreach (KeyValuePair<int, CSVColumn> kvp in cellValueChangedSettings.DicActSelectedIndexes)
                        {
                            if (kvp.Value != null)
                            {
                                int column = kvp.Key - 1;

                                if (kvp.Value.FirstColumn >= 0)
                                {
                                    if (kvp.Value is MixedColumnFromCSVFile && (kvp.Value as MixedColumnFromCSVFile).SecondColumn >= 0)
                                    {
                                        if (column < tableRow.Length && _csvTable.Count > kvp.Value.FirstColumn && _csvTable.Count > (kvp.Value as MixedColumnFromCSVFile).SecondColumn)
                                        {
                                            tableRow[column] = _csvTable[kvp.Value.FirstColumn][row] + (kvp.Value as MixedColumnFromCSVFile).Separator +
                                                _csvTable[(kvp.Value as MixedColumnFromCSVFile).SecondColumn][row];
                                        }
                                    }
                                    else
                                    {
                                        if (column < tableRow.Length && _csvTable.Count > kvp.Value.FirstColumn)
                                        {
                                            tableRow[column] = _csvTable[kvp.Value.FirstColumn][row];
                                        }
                                    }
                                }
                                else
                                {
                                    if (column < tableRow.Length)
                                    {
                                        tableRow[column] = string.Empty;
                                    }
                                }
                            }
                        }

                        if (cellValueChangedSettings.CheckedAllowComposidPersonlId)
                        {
                            string personalID = string.Empty;
                            if (cellValueChangedSettings.ComposidPersonalIdFirstColumn != null && cellValueChangedSettings.ComposidPersonalIdFirstColumn.FirstColumn != -1
                                && _csvTable.Count > cellValueChangedSettings.ComposidPersonalIdFirstColumn.FirstColumn)
                            {
                                personalID = _csvTable[cellValueChangedSettings.ComposidPersonalIdFirstColumn.FirstColumn][row];
                            }

                            if (_actValidationType != null)
                            {
                                personalID += _actValidationType.Separator;
                            }

                            if (cellValueChangedSettings.ComposidPersonalIdSecondColumn != null && cellValueChangedSettings.ComposidPersonalIdSecondColumn.FirstColumn != -1
                                && _csvTable.Count > cellValueChangedSettings.ComposidPersonalIdSecondColumn.FirstColumn)
                            {
                                personalID += _csvTable[cellValueChangedSettings.ComposidPersonalIdSecondColumn.FirstColumn][row];
                            }

                            tableRow[PERSONAL_ID_COLUMN_INDEX - 1] = personalID;
                        }

                        bool importRow = true;
                        if (oldTableRows != null && oldTableRows.Count > row)
                        {
                            importRow = oldTableRows[row].Column1;
                        }

                        tableRows.Add(new TableRow(tableRow, importRow));
                        _pbLoadFileImport_NexStep(row + 1);
                    }
                }
                catch { }

                End_dgCSVImportColumns_CellValueChanged(tableRows);
            }
        }

        private void End_dgCSVImportColumns_CellValueChanged(List<TableRow> tableRows)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<List<TableRow>>(End_dgCSVImportColumns_CellValueChanged), tableRows);
            }
            else
            {
                _pbLoadFileImport.Value = 0;

                int scrollPosition = _dgCSVImportValues.HorizontalScrollingOffset;
                _dgCSVImportValues.DataSource = tableRows;
                _dgCSVImportValues.HorizontalScrollingOffset = scrollPosition;

                _dgCSVImportValues.ResumeLayout();
            }
        }

        private void _pbLoadFileImport_NexStep(int value)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(_pbLoadFileImport_NexStep), value);
            }
            else
            {
                _pbLoadFileImport.Value = value;
            }
        }

        protected void IntitDoubleBufferGrid(DataGridView dataGridView)
        {
            try
            {
                Type dgvType = dataGridView.GetType();
                System.Reflection.PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                pi.SetValue(dataGridView, true, null);
            }
            catch
            { }
        }

        private void _cbTemplate_SelectedValueChanged(object sender, EventArgs e)
        {
            CSVImportSchema template = _cbTemplate.SelectedItem as CSVImportSchema;
            if (template != null)
            {
                _bUpdateTemplate.Enabled = true;
                _enableColumnChanged = false;

                if (sender != null)
                {
                    for (int i = 1; i <= IMPORT_COLUMN_COUNT; i++)
                    {
                        try
                        {
                            DataGridViewComboBoxCell cell = _dgCSVImportColumns.Rows[0].Cells[i] as DataGridViewComboBoxCell;
                            if (cell != null && cell.Items != null)
                            {
                                foreach (object obj in cell.Items)
                                {
                                    if (obj != null && obj.ToString() == string.Empty)
                                    {
                                        cell.Value = obj;
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    if (template.CSVImportColumns != null && template.CSVImportColumns.Count > 0)
                    {
                        foreach (CSVImportColumn csvImportColumn in template.CSVImportColumns)
                        {
                            if (csvImportColumn != null)
                            {
                                CSVColumn csvColumn = null;
                                if (csvImportColumn.SecondColumn == -1)
                                {
                                    csvColumn = new ColumnFromCSVFile(csvImportColumn.FirstColumn);
                                }
                                else
                                {
                                    csvColumn = new MixedColumnFromCSVFile(csvImportColumn.FirstColumn, csvImportColumn.Separator, csvImportColumn.SecondColumn);
                                }

                                if (csvColumn != null)
                                {
                                    try
                                    {
                                        DataGridViewComboBoxCell cell = _dgCSVImportColumns.Rows[0].Cells[csvImportColumn.ColumnIndex] as DataGridViewComboBoxCell;
                                        if (cell != null && cell.Items != null)
                                        {
                                            foreach (object obj in cell.Items)
                                            {
                                                if (obj != null && obj.ToString() == csvColumn.ToString())
                                                {
                                                    cell.Value = obj;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }

                    if (template.DateFormat != null && template.DateFormat != string.Empty && _cbDateFormat.Items != null &&
                        _cbDateFormat.Items.Count > 0)
                    {
                        foreach (object obj in _cbDateFormat.Items)
                        {
                            DateFormat dateFormat = obj as DateFormat;
                            if (dateFormat != null && dateFormat.DateFormatString == template.DateFormat)
                            {
                                _cbDateFormat.SelectedItem = dateFormat;
                                break;
                            }
                        }
                    }

                    if (_cbSeparator.Items != null && _cbSeparator.Items.Count > 0)
                    {
                        foreach (object obj in _cbSeparator.Items)
                        {
                            Separators separator = obj as Separators;
                            if (separator != null && separator.Sepearator == template.Separator)
                            {
                                _cbSeparator.SelectedItem = separator;
                                break;
                            }
                        }
                    }

                    _cbParseBirthdateFromPersonId.Checked = template.ParseBirthDateFromPersonId;
                    _eDepartmentFolder.Text = template.DepartmentFolder;

                    _cbAllowCompositPersonalId.Checked = template.AllowCompositPersonalId;

                    if (template.AllowCompositPersonalId)
                    {
                        int compositPersonalIdFirstColumn = template.CompositPersonalIdFirstColumn;
                        if (compositPersonalIdFirstColumn != -1)
                        {
                            foreach (object obj in _cbPersonalIdFirstColumn.Items)
                            {
                                ColumnFromCSVFile columnFromCSVFile = obj as ColumnFromCSVFile;
                                if (columnFromCSVFile != null && columnFromCSVFile.FirstColumn == compositPersonalIdFirstColumn)
                                {
                                    _cbPersonalIdFirstColumn.SelectedItem = columnFromCSVFile;
                                    break;
                                }
                            }
                        }
                        else
                            _cbPersonalIdFirstColumn.SelectedItem = null;

                        int compositPersonalIdSecondColumn = template.CompositPersonalIdSecondColumn;
                        if (compositPersonalIdSecondColumn != -1)
                        {
                            foreach (object obj in _cbPersonalIdSecondColumn.Items)
                            {
                                ColumnFromCSVFile columnFromCSVFile = obj as ColumnFromCSVFile;
                                if (columnFromCSVFile != null && columnFromCSVFile.FirstColumn == compositPersonalIdSecondColumn)
                                {
                                    _cbPersonalIdSecondColumn.SelectedItem = columnFromCSVFile;
                                    break;
                                }
                            }
                        }
                        else
                            _cbPersonalIdSecondColumn.SelectedItem = null;
                    }

                    if (template.PersonalIdValidationType == null)
                    {
                        if (_cbValidationType.Items.Count > 0)
                            _cbValidationType.SelectedItem = _cbValidationType.Items[0];
                        else
                            _cbValidationType.SelectedItem = null;
                    }
                    else
                    {
                        if (_cbValidationType.Items != null && _cbValidationType.Items.Count > 0)
                        {
                            foreach (object obj in _cbValidationType.Items)
                            {
                                ItemSeparatorType itemSeparatorType = obj as ItemSeparatorType;
                                if (itemSeparatorType != null && (byte)itemSeparatorType.SeparatorType == template.PersonalIdValidationType.Value)
                                {
                                    _cbValidationType.SelectedItem = itemSeparatorType;
                                    break;
                                }
                            }
                        }
                    }
                }

                _enableColumnChanged = true;
                _dgCSVImportColumns_ComboBoxValueChanged();
            }
            else
            {
                _bUpdateTemplate.Enabled = false;
            }
        }

        private void _bImport_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            DisableControlBeforeImport();

            var tableRows = _dgCSVImportValues.DataSource as List<TableRow>;

            DateFormat dateFormat = null;
            if (tableRows == null 
                || tableRows.Count == 0 
                || !string.IsNullOrEmpty(tableRows[0].Column17) 
                || !string.IsNullOrEmpty(tableRows[0].Column18))
            {
                dateFormat = _cbDateFormat.SelectedItem as DateFormat;
                if (dateFormat == null)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(
                        Contal.IwQuick.UI.NotificationPriority.JustOne, _cbDateFormat,
                        GetString("ErrorEntryDateFormat"), Contal.IwQuick.UI.ControlNotificationSettings.Default);

                    AllowControlsAfterImpot();
                    return;
                }
            }

            if (!_isSelectedSubSite)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(
                        Contal.IwQuick.UI.NotificationPriority.JustOne, _tbmSubSite,
                        GetString("ErrorEntrySubSite"), Contal.IwQuick.UI.ControlNotificationSettings.Default);
                AllowControlsAfterImpot();

                return;
            }

            CSVImportTypeItem csvImportTypeItem = _cbImportType.SelectedItem as CSVImportTypeItem;
            if (csvImportTypeItem == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbImportType,
                    GetString("ErrorEntryImportType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);

                AllowControlsAfterImpot();
                return;
            }

            if (_cbAllowCompositPersonalId.Checked)
            {
                ColumnFromCSVFile compositPersonalIdFirstColumn = _cbPersonalIdFirstColumn.SelectedItem as ColumnFromCSVFile;
                if (compositPersonalIdFirstColumn == null || compositPersonalIdFirstColumn.FirstColumn == -1)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbPersonalIdFirstColumn,
                        GetString("ErrorEntryFirstColumnForCompositPersonalId"), Contal.IwQuick.UI.ControlNotificationSettings.Default);

                    AllowControlsAfterImpot();
                    return;
                }

                ColumnFromCSVFile compositPersonalIdSecondColumn = _cbPersonalIdSecondColumn.SelectedItem as ColumnFromCSVFile;
                if (compositPersonalIdSecondColumn == null || compositPersonalIdSecondColumn.FirstColumn == -1)
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbPersonalIdSecondColumn,
                        GetString("ErrorEntrySecondColumnForCompositPersonalId"), Contal.IwQuick.UI.ControlNotificationSettings.Default);

                    AllowControlsAfterImpot();
                    return;
                }
            }

            ItemSeparatorType itemSeparatorType = _cbValidationType.SelectedItem as ItemSeparatorType;
            if (itemSeparatorType == null)
            {
                Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _cbValidationType,
                    GetString("ErrorEntryValidationType"), Contal.IwQuick.UI.ControlNotificationSettings.Default);

                AllowControlsAfterImpot();
                return;
            }

            List<TableRow> rows = _dgCSVImportValues.DataSource as List<TableRow>;

            if (rows == null || rows.Count == 0)
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorNoDataForImport"));
                AllowControlsAfterImpot();
                return;
            }

            _dgReport.DataSource = new List<CSVImportPerson>();
            SafeThread<DateFormat, CSVImportTypeItem, List<TableRow>>.StartThread(DoImport, dateFormat, csvImportTypeItem, rows);
        }

        private void ImportedPersonCountChanged(Guid formIdentification, int importedPersonCount)
        {
            if (_isRunningImport && _formIdentification == formIdentification)
                _pbLoadFileImport_NexStep(importedPersonCount);
        }

        private void DisableControlBeforeImport()
        {
            _cbSeparator.Enabled = false;
            _eFilePath.Enabled = false;
            _bOpenFile.Enabled = false;
            _bImport.Enabled = false;
            _cbTemplate.Enabled = false;
            _cbDateFormat.Enabled = false;
            _cbImportType.Enabled = false;
            _cbParseBirthdateFromPersonId.Enabled = false;
            _dgCSVImportColumns.Enabled = false;
            _dgCSVImportValues.Enabled = false;
            _eDepartmentFolder.Enabled = false;
            _bCreateTemplate.Enabled = false;
            _bUpdateTemplate.Enabled = false;
            _cbAllowCompositPersonalId.Enabled = false;
            _cbPersonalIdFirstColumn.Enabled = false;
            _cbPersonalIdSecondColumn.Enabled = false;
            _cbValidationType.Enabled = false;
        }

        private void AllowControlsAfterImpot()
        {
            _cbSeparator.Enabled = true;
            _eFilePath.Enabled = true;
            _bOpenFile.Enabled = true;
            _bImport.Enabled = true;
            _cbTemplate.Enabled = true;
            _cbDateFormat.Enabled = true;
            _cbImportType.Enabled = true;
            _cbParseBirthdateFromPersonId.Enabled = true;
            _dgCSVImportColumns.Enabled = true;
            _dgCSVImportValues.Enabled = true;
            _eDepartmentFolder.Enabled = true;
            _bCreateTemplate.Enabled = true;

            if (_cbTemplate.SelectedItem as CSVImportSchema != null)
                _bUpdateTemplate.Enabled = true;

            _cbAllowCompositPersonalId.Enabled = true;

            if (_cbAllowCompositPersonalId.Checked)
            {
                _cbPersonalIdFirstColumn.Enabled = true;
                _cbPersonalIdSecondColumn.Enabled = true;
            }

            _cbValidationType.Enabled = true;
        }

        public void DoImport(DateFormat dateFormat, CSVImportTypeItem csvImportTypeItem, List<TableRow> rows)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            if (rows != null && rows.Count > 0)
            {
                var importedPersonsData = new List<ImportPersonData>();
                var importedFailedReport = new List<CSVImportRow>();

                foreach (var row in rows)
                {
                    if (row.Column1)
                    {
                        try
                        {
                            string firstName = row.Column2;
                            string surname = row.Column3;
                            string middleName = row.Column4;
                            string address = row.Column5;
                            string title = row.Column6;
                            string phoneNumber = row.Column7;
                            string email = row.Column8;
                            string identification = row.Column9;
                            string employeeNumber = row.Column10;
                            string company = row.Column11;
                            string role = row.Column12;
                            string deparment = row.Column13;
                            string costCenter = row.Column14;
                            string relativeSuperior = row.Column15;
                            string relativeSuperiorsPhoneNumber = row.Column16;

                            DateTime? employmentBeginningDate = null;
                            string dateString = row.Column17;

                            if (!string.IsNullOrEmpty(dateString))
                                employmentBeginningDate = DateTime.ParseExact(dateString, dateFormat.DateFormatString, null);

                            DateTime? employmentEndDate = null;
                            dateString = row.Column18;

                            if (!string.IsNullOrEmpty(dateString))
                                employmentEndDate = DateTime.ParseExact(dateString, dateFormat.DateFormatString, null);


                            if (IsValidPersonalId(identification))
                            {
                                importedPersonsData.Add(new ImportPersonData(identification, firstName, middleName, surname, address, title, phoneNumber, email,
                                    employeeNumber, company, role, deparment, costCenter, relativeSuperior, relativeSuperiorsPhoneNumber, employmentBeginningDate, 
                                    employmentEndDate));
                            }
                            else
                            {
                                importedFailedReport.Add(new CSVImportRow(new CSVImportPerson(Guid.Empty, row.Column2 + " " + row.Column3, CSVImportResult.InvalidPersonalIdFormat)));
                            }
                        }
                        catch
                        {
                            importedFailedReport.Add(new CSVImportRow(new CSVImportPerson(Guid.Empty, row.Column2 + " " + row.Column3, CSVImportResult.Failed)));
                        }
                    }
                }

                ResetProgress(importedPersonsData.Count);
                
                try
                {
                    _isRunningImport = true;

                    if (!_isSelectedSubSite)
                    {
                        CancelImport();
                        return;
                    }

                    bool licenceRestriction;
                    List<CSVImportPerson> report;
                    int importedPersonsCount;
                    if (CgpClient.Singleton.MainServerProvider.Persons.ImportPersons(
                        _formIdentification,
                        importedPersonsData,
                        csvImportTypeItem.ImportType,
                        _eDepartmentFolder.Text,
                        _selectedSubSite,
                        _cbParseBirthdateFromPersonId.Checked,
                        out report,
                        out licenceRestriction,
                        out importedPersonsCount))
                    {
                        List<CSVImportRow> reportRows = new List<CSVImportRow>();
                        if (report != null && report.Count > 0)
                        {
                            foreach (CSVImportPerson csvImportPerson in report)
                            {
                                if (csvImportPerson != null)
                                {
                                    reportRows.Add(new CSVImportRow(csvImportPerson));
                                }
                            }
                        }

                        if (importedFailedReport != null && importedFailedReport.Count > 0)
                        {
                            foreach (CSVImportRow csvImportRow in importedFailedReport)
                            {
                                if (csvImportRow != null)
                                {
                                    reportRows.Add(csvImportRow);
                                }
                            }
                        }

                        EndImport(reportRows, importedPersonsCount);
                    }
                    else
                    {
                        NotAllowedImport(licenceRestriction);
                    }
                }
                finally
                {
                    _isRunningImport = false;
                }
            }
        }

        private void CancelImport()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(CancelImport));
                return;
            }

            AllowControlsAfterImpot();
        }

        private bool IsValidPersonalId(string personalId)
        {
            if (_actValidationType == null)
                return true;

            switch (_actValidationType.SeparatorType)
            {
                case SeparatorType.NonStandard:
                    return true;
                case SeparatorType.WithoOutSeparator:
                    if (personalId.Length == 10)
                        return true;

                    break;
                default:
                    if (_actValidationType.Separator.Length == 1)
                    {
                        string[] separateId = personalId.Split(_actValidationType.Separator[0]);
                        if (separateId.Length == 2)
                        {
                            if (separateId[0].Length == 6 && separateId[1].Length == 4)
                                return true;
                        }
                    }

                    break;
            }

            return false;
        }

        private void NotAllowedImport(bool licenceRestriction)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(NotAllowedImport), licenceRestriction);
            }
            else
            {
                if (licenceRestriction)
                {
                    Dialog.Error("LicenceCanNotImportData");
                }
                else
                {
                    Contal.IwQuick.UI.Dialog.Info(GetString("InfoAnotherClientImportsCSVFile"));
                    AllowControlsAfterImpot();
                }
            }
        }

        private void ResetProgress(int importedPersonCount)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(ResetProgress), importedPersonCount);
            }
            else
            {
                _pbLoadFileImport.Maximum = importedPersonCount;
                _pbLoadFileImport.Value = 0;
            }
        }

        public void EndImport(List<CSVImportRow> report, int importedPersonsCount)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<List<CSVImportRow>, int>(EndImport), report, importedPersonsCount);
            }
            else
            {
                _pbLoadFileImport.Value = _pbLoadFileImport.Maximum;

                if (importedPersonsCount > 0)
                    Contal.IwQuick.UI.Dialog.Info(GetString("InfoImportSucceededWithImportPersons") + " " + importedPersonsCount.ToString());
                else
                    Contal.IwQuick.UI.Dialog.Info(GetString("InfoImportSucceededWithNoImportPersons"));

                _pbLoadFileImport.Value = 0;
                AllowControlsAfterImpot();
                _dgReport.DataSource = report;
                _tcImportReport.SelectedTab = _tpReport;
            }
        }

        private void _cbSeparator_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_eFilePath.Text != string.Empty)
            {
                LoadCSVFile();
            }
        }

        private void _dgCSVImportColumns_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridViewCheckBoxCell cell = _dgCSVImportColumns.CurrentCell as DataGridViewCheckBoxCell;
            if (cell != null)
            {
                List<TableRow> rows = _dgCSVImportValues.DataSource as List<TableRow>;
                if (rows != null && rows.Count > 0)
                {
                    foreach (TableRow row in rows)
                    {
                        row.Column1 = (bool)cell.EditedFormattedValue;
                    }

                    _dgCSVImportValues.Refresh();
                }
            }

            _dgCSVImportColumns.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void _dgReport_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _bEdit_Click(sender, null);
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            int rowIndex = -1;
            DataGridViewSelectedRowCollection selectedRows = _dgReport.SelectedRows;
            if (selectedRows != null && selectedRows.Count > 0)
                rowIndex = selectedRows[0].Index;

            List<CSVImportRow> rows = _dgReport.DataSource as List<CSVImportRow>;
            if (rows != null && rowIndex >= 0 && rows.Count > rowIndex)
            {
                Person person = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(rows[rowIndex].IdPerson);

                if (person != null)
                {
                    PersonsForm.Singleton.OpenEditDialog(person);
                }
            }
        }

        private void _bCreateTemplate_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            CreateCSVImportTemplateForm form = new CreateCSVImportTemplateForm();
            form.ShowDialog();
            string templateName = form.GetCSVImportSchemaName;
            if (templateName != null && templateName != string.Empty)
            {
                CSVImportSchema importSchema = new CSVImportSchema();
                importSchema.CSVImportSchemaName = templateName;

                GetTemplateValues(importSchema);

                Exception error;
                bool retValue = CgpClient.Singleton.MainServerProvider.CSVImportSchemas.Insert(ref importSchema, out error);

                if (retValue)
                {
                    LoadTemplates(importSchema.ToString());
                }
                else
                {
                    if (error is SqlUniqueException)
                    {
                        CSVImportSchema csvImportSchema = GetImportSchema(templateName);
                        DoUpdateTemplate(csvImportSchema);
                    }
                    else
                    {
                        Dialog.Error(LocalizationHelper.GetString("ErrorInsertTemplateFailed"));
                    }
                }
            }
        }

        private CSVImportSchema GetImportSchema(string importSchemaName)
        {
            Exception error;
            ICollection<CSVImportSchema> templates = CgpClient.Singleton.MainServerProvider.CSVImportSchemas.List(out error);
            if (templates != null && templates.Count > 0)
            {
                foreach (CSVImportSchema csvImportSchema in templates)
                {
                    if (csvImportSchema != null)
                    {
                        CSVImportSchema actCSVImportSchema = CgpClient.Singleton.MainServerProvider.CSVImportSchemas.GetObjectById(csvImportSchema.IdCSVImportSchema);
                        if (actCSVImportSchema != null && actCSVImportSchema.CSVImportSchemaName == importSchemaName)
                        {
                            return actCSVImportSchema;
                        }
                    }
                }
            }
            return null;
        }

        private void GetTemplateValues(CSVImportSchema csvImportSchema)
        {
            DateFormat dateFormat = _cbDateFormat.SelectedItem as DateFormat;
            if (dateFormat != null)
            {
                csvImportSchema.DateFormat = dateFormat.DateFormatString;
            }
            else
            {
                csvImportSchema.DateFormat = string.Empty;
            }

            Separators separator = _cbSeparator.SelectedItem as Separators;
            if (separator != null)
            {
                csvImportSchema.Separator = separator.Sepearator;
            }

            csvImportSchema.ParseBirthDateFromPersonId = _cbParseBirthdateFromPersonId.Checked;
            csvImportSchema.DepartmentFolder = _eDepartmentFolder.Text;

            csvImportSchema.AllowCompositPersonalId = _cbAllowCompositPersonalId.Checked;
            
            ColumnFromCSVFile personalIdFirstColumn = _cbPersonalIdFirstColumn.SelectedItem as ColumnFromCSVFile;
            if (personalIdFirstColumn != null)
                csvImportSchema.CompositPersonalIdFirstColumn = personalIdFirstColumn.FirstColumn;
            else
                csvImportSchema.CompositPersonalIdFirstColumn = -1;

            ColumnFromCSVFile personalIdSecondColumn = _cbPersonalIdSecondColumn.SelectedItem as ColumnFromCSVFile;
            if (personalIdSecondColumn != null)
                csvImportSchema.CompositPersonalIdSecondColumn = personalIdSecondColumn.FirstColumn;
            else
                csvImportSchema.CompositPersonalIdSecondColumn = -1;

            ItemSeparatorType itemSeparatorType = _cbValidationType.SelectedItem as ItemSeparatorType;
            if (itemSeparatorType != null)
            {
                csvImportSchema.PersonalIdValidationType = (byte)itemSeparatorType.SeparatorType;
            }
            else
            {
                csvImportSchema.PersonalIdValidationType = null;
            }

            Dictionary<int, CSVColumn> actSelectedColumns = GetActSelectedColumns();
            csvImportSchema.CSVImportColumns = new List<CSVImportColumn>();
            if (actSelectedColumns != null && actSelectedColumns.Count > 0)
            {
                foreach (KeyValuePair<int, CSVColumn> kvp in actSelectedColumns)
                {
                    int column = kvp.Key;
                    CSVColumn csvColumn = kvp.Value;

                    if (csvColumn != null && csvColumn.FirstColumn != -1)
                    {
                        CSVImportColumn csvImportColumn = new CSVImportColumn();
                        csvImportColumn.ColumnIndex = column;
                        csvImportColumn.FirstColumn = csvColumn.FirstColumn;

                        if (csvColumn is MixedColumnFromCSVFile)
                        {
                            MixedColumnFromCSVFile mixedColumnFromCSVFile = csvColumn as MixedColumnFromCSVFile;
                            csvImportColumn.SecondColumn = mixedColumnFromCSVFile.SecondColumn;
                            csvImportColumn.Separator = mixedColumnFromCSVFile.Separator;
                        }
                        else
                        {
                            csvImportColumn.SecondColumn = -1;
                        }

                        csvImportSchema.CSVImportColumns.Add(csvImportColumn);
                    }
                }
            }
        }

        private void _bUpdateTemplate_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            CSVImportSchema actCSVImportSchema = _cbTemplate.SelectedItem as CSVImportSchema;
            DoUpdateTemplate(actCSVImportSchema);
        }

        private void DoUpdateTemplate(CSVImportSchema csvImportSchema)
        {
            if (csvImportSchema != null && Dialog.Question(LocalizationHelper.GetString("QuestionUpdateTemplate") + " " + csvImportSchema.ToString()))
            {
                Exception error;
                CSVImportSchema editedCSVImportSchema = CgpClient.Singleton.MainServerProvider.CSVImportSchemas.GetObjectForEdit(csvImportSchema.IdCSVImportSchema, out error);
                if (editedCSVImportSchema != null)
                {
                    GetTemplateValues(editedCSVImportSchema);
                    bool retValue = CgpClient.Singleton.MainServerProvider.CSVImportSchemas.Update(editedCSVImportSchema, out error);
                    CgpClient.Singleton.MainServerProvider.CSVImportSchemas.EditEnd(editedCSVImportSchema);

                    if (retValue)
                    {
                        LoadTemplates(csvImportSchema.ToString());
                    }
                    else
                    {
                        Dialog.Error(LocalizationHelper.GetString("ErrorUpdateTemplateFailed"));
                    }
                }
            }
        }

        private void _cbAllowComposidPersonalId_CheckedChanged(object sender, EventArgs e)
        {
            if (_cbAllowCompositPersonalId.Checked)
            {
                _cbPersonalIdFirstColumn.Enabled = true;
                _cbPersonalIdSecondColumn.Enabled = true;
            }
            else
            {
                _cbPersonalIdFirstColumn.Enabled = false;
                _cbPersonalIdFirstColumn.SelectedItem = null;

                _cbPersonalIdSecondColumn.Enabled = false;
                _cbPersonalIdSecondColumn.SelectedItem = null;
            }

            if (_enableColumnChanged)
                FillComboBoxItems();
        }

        private void CSVImportDialog_Shown(object sender, EventArgs e)
        {
            Rectangle rectangle = _dgCSVImportColumns.GetCellDisplayRectangle(0, 0, false);
            _dgCSVImportColumns.Height = rectangle.Bottom;

            _dgCSVImportValues.Height = _dgCSVImportValues.Height + _dgCSVImportValues.Top - _dgCSVImportColumns.Bottom;
            _dgCSVImportValues.Top = _dgCSVImportColumns.Bottom;
        }

        private void _tbmSubSite_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyActSubSite();
            }
            else if (item.Name == "_tsiRemove")
            {
                SetActSubSite(null);
            }
        }

        private bool ModifyActSubSite()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) 
                return false;

            ICollection<int> selectedSubSiteIds;
            var selectStructuredSubSiteForm = new SelectStructuredSubSiteForm();

            if (selectStructuredSubSiteForm.SelectStructuredSubSites(
                false,
                out selectedSubSiteIds))
            {
                if (selectedSubSiteIds != null)
                {
                    var selectedSubSiteId = selectedSubSiteIds.First();

                    if (selectedSubSiteId != -1)
                    {
                        SetActSubSite(
                            CgpClient.Singleton.MainServerProvider.StructuredSubSites.GetObjectById(
                                selectedSubSiteId));
                    }
                    else
                    {
                        _isSelectedSubSite = true;
                        _tbmSubSite.Text = GetString("SelectStructuredSubSiteForm_RootNode");
                        _tbmSubSite.TextImage =
                            ObjectImageList.Singleton.GetImageForObjectType(ObjectType.StructuredSubSite);
                    }
                }

                return true;
            }

            return false;
        }

        private void SetActSubSite(StructuredSubSite subsite)
        {
            _selectedSubSite = subsite;

            if (subsite == null)
            {
                _tbmSubSite.Text = string.Empty;
                _isSelectedSubSite = false;
            }
            else
            {
                _isSelectedSubSite = true;
                _tbmSubSite.Text = subsite.ToString();
                _tbmSubSite.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(subsite);
            }
        }

        private void _bSelectDeparment_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (!_isSelectedSubSite && !ModifyActSubSite())
            {
                return;
            }

            try
            {
                Exception error;
                var listDepartments = CgpClient.Singleton.MainServerProvider.UserFoldersSutructures
                    .ListDepartmentsBySubSite(
                        _selectedSubSite == null
                            ? -1
                            : _selectedSubSite.IdStructuredSubSite,
                        out error);

                if (error != null) throw error;

                var formAdd = new ListboxFormAdd(
                    listDepartments,
                    CgpClient.Singleton.LocalizationHelper.GetString(
                        "UserFoldersStructuresFormUserFoldersStructuresForm"));

                object outUserFolder;
                formAdd.ShowDialog(out outUserFolder);
                if (outUserFolder != null)
                {
                    _eDepartmentFolder.Text = outUserFolder.ToString();
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            _eDepartmentFolder.Text = string.Empty;
        }
    }

    public class Separators
    {
        private string _name;
        private char _separator;

        public char Sepearator { get { return _separator; } }

        public Separators(string name, char separator)
        {
            _name = name;
            _separator = separator;
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class CSVColumn
    {
        protected int _firstColumn;
        public int FirstColumn { get { return _firstColumn; } set { _firstColumn = value; } }
        public string Name { get { return ToString(); } }
    }

    public class ColumnFromCSVFile : CSVColumn
    {
        private char _separator;
        public char Separator { get { return _separator; } set { _separator = value; } }

        public ColumnFromCSVFile(int columnIndex)
        {
            _firstColumn = columnIndex;
        }

        public override string ToString()
        {
            if (_firstColumn == -1)
                return string.Empty;
            else
                return CgpClient.Singleton.LocalizationHelper.GetString("Column") + " " + (_firstColumn + 1).ToString();
        }
    }

    public class MixedColumnFromCSVFile : CSVColumn
    {
        private char _separator;
        private int _secondColumn;

        public char Separator { get { return _separator; } set { _separator = value; } }
        public int SecondColumn { get { return _secondColumn; } set { _secondColumn = value; } }

        public MixedColumnFromCSVFile(int firstcolumn, char separator, int secondColumn)
        {
            _firstColumn = firstcolumn;
            _separator = separator;
            _secondColumn = secondColumn;
        }

        public override string ToString()
        {
            if (_firstColumn == -1)
                return string.Empty;
            else if (_secondColumn == -1)
                return CgpClient.Singleton.LocalizationHelper.GetString("Column") + " " + (_firstColumn + 1).ToString();
            else
                return CgpClient.Singleton.LocalizationHelper.GetString("Column") + " " + (_firstColumn + 1).ToString() +
                    " " + _separator + " " + CgpClient.Singleton.LocalizationHelper.GetString("Column") + " " + (_secondColumn + 1).ToString();
        }
    }

    public class DateFormat
    {
        private string _name;
        private string _dateFormatString;

        public string Name { get { return _name; } }
        public string DateFormatString { get { return _dateFormatString; } }

        public DateFormat(string name, string dateFormatString)
        {
            _name = name;
            _dateFormatString = dateFormatString;
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class CSVImportTypeItem
    {
        private CSVImportType _importType;

        public CSVImportType ImportType { get { return _importType; } }

        public CSVImportTypeItem(CSVImportType importType)
        {
            _importType = importType;
        }

        public override string ToString()
        {
            return CgpClient.Singleton.LocalizationHelper.GetString("ImportType_" + _importType.ToString());
        }

        public static IList<CSVImportTypeItem> GetAllImportTypeItem()
        {
            return EnumHelper.ListAllValuesWithProcessing<CSVImportType, CSVImportTypeItem>(
                (value) => new CSVImportTypeItem(value) );
        }
    }

    public class TableRow
    {
        private bool _column1;
        private string _column2;
        private string _column3;
        private string _column4;
        private string _column5;
        private string _column6;
        private string _column7;
        private string _column8;
        private string _column9;
        private string _column10;
        private string _column11;
        private string _column12;
        private string _column13;
        private string _column14;
        private string _column15;
        private string _column16;
        private string _column17;
        private string _column18;

        public bool Column1 { get { return _column1; } set { _column1 = value; } }
        public string Column2 { get { return _column2; } }
        public string Column3 { get { return _column3; } }
        public string Column4 { get { return _column4; } }
        public string Column5 { get { return _column5; } }
        public string Column6 { get { return _column6; } }
        public string Column7 { get { return _column7; } }
        public string Column8 { get { return _column8; } }
        public string Column9 { get { return _column9; } }
        public string Column10 { get { return _column10; } }
        public string Column11 { get { return _column11; } }
        public string Column12 { get { return _column12; } }
        public string Column13 { get { return _column13; } }
        public string Column14 { get { return _column14; } }
        public string Column15 { get { return _column15; } }
        public string Column16 { get { return _column16; } }
        public string Column17 { get { return _column17; } }
        public string Column18 { get { return _column18; } }

        public TableRow(string[] parameters, bool importRow)
        {
            if (parameters.Length == 17)
            {
                _column1 = importRow;
                _column2 = parameters[0];
                _column3 = parameters[1];
                _column4 = parameters[2];
                _column5 = parameters[3];
                _column6 = parameters[4];
                _column7 = parameters[5];
                _column8 = parameters[6];
                _column9 = parameters[7];
                _column10 = parameters[8];
                _column11 = parameters[9];
                _column12 = parameters[10];
                _column13 = parameters[11];
                _column14 = parameters[12];
                _column15 = parameters[13];
                _column16 = parameters[14];
                _column17 = parameters[15];
                _column18 = parameters[16];
            }
        }
    }

    public class CSVImportRow
    {
        private Guid _idPerson;
        private string _fullName;
        private string _importResult;

        public Guid IdPerson { get { return _idPerson; } }
        public string FullName { get { return _fullName; } }
        public string ImportResult { get { return _importResult; } }

        public CSVImportRow(CSVImportPerson importPerson)
        {
            if (importPerson != null)
            {
                _idPerson = importPerson.IdPerson;
                _fullName = importPerson.FullName;
                _importResult = CgpClient.Singleton.LocalizationHelper.GetString("CSVImportResult_" + importPerson.ImportResult.ToString());
            }
        }
    }

    public class CellValueChangedSettings
    {
        private Dictionary<int, CSVColumn> _dicActSelectedIndexes;
        private int _rowCount;
        private bool _checkedAllowComposidPersonlId;
        private ColumnFromCSVFile _composidPersonalIdFirstColumn;
        private ColumnFromCSVFile _composidPersonalIdSecondColumn;

        public Dictionary<int, CSVColumn> DicActSelectedIndexes { get { return _dicActSelectedIndexes; } }
        public int RowCount { get { return _rowCount; } }
        public bool CheckedAllowComposidPersonlId { get { return _checkedAllowComposidPersonlId; } }
        public ColumnFromCSVFile ComposidPersonalIdFirstColumn { get { return _composidPersonalIdFirstColumn; } }
        public ColumnFromCSVFile ComposidPersonalIdSecondColumn { get { return _composidPersonalIdSecondColumn; } }

        public CellValueChangedSettings(Dictionary<int, CSVColumn> dicActSelectedIndexes, int rowCount, bool checkedAllowComposidPersonlId,
            ColumnFromCSVFile composidPersonalIdFirstColumn, ColumnFromCSVFile composidPersonalIdSecondColumn)
        {
            _dicActSelectedIndexes = dicActSelectedIndexes;
            _rowCount = rowCount;
            _checkedAllowComposidPersonlId = checkedAllowComposidPersonlId;
            _composidPersonalIdFirstColumn = composidPersonalIdFirstColumn;
            _composidPersonalIdSecondColumn = composidPersonalIdSecondColumn;
        }
    }
}
