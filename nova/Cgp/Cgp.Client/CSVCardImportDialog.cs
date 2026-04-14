using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Reflection;

using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Threads;
using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class CSVCardImportDialog :
#if DESIGNER
        Form
#else
 CgpTranslateForm
#endif
    {
        public const string MIXED_COLUMN_SEPARATOR = "-";
        public const string SEPARATED_PERSONAL_ID_TEMPLATE = "SeparatedPersonalId";
        public const string COMBINED_PERSONAL_ID_TEMPLATE = "CombinedPersonalId";

        public const string PERSONAL_ID_SEPARATOR_NORTH_STATES_NAME = "NordicCountriesConvention";
        public const string PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_NAME = "CentralEuropeStatesConvention";
        public const string PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_NAME = "WithoutSeparator";
        public const string PERSONAL_ID_SEPARATOR_NON_STANDART = "NonStandard";

        public const string PERSONAL_ID_SEPARATOR_NORTH_STATES_SYMBOL = "-";
        public const string PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_SYMBOL = @"/";
        public const string PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_SYMBOL = "";

        private string _actMixedColumSeparator = PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_SYMBOL;

        private List<List<string>> _csvTable = new List<List<string>>();
        private Action<Guid, int> _importedCardCountChanged = null;
        private Guid _formIdentification = Guid.NewGuid();
        private bool _isRunningImport = false;

        public CSVCardImportDialog()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();

            _dgCSVImportColumns.Rows.Add();
            _dgCSVImportValues.Height = _dgCSVImportValues.Height + _dgCSVImportValues.Top - _dgCSVImportColumns.Bottom;
            _dgCSVImportValues.Top = _dgCSVImportColumns.Bottom;

            List<Separators> separators = new List<Separators>();
            separators.Add(new Separators("TAB", '\t'));
            separators.Add(new Separators(",", ','));
            separators.Add(new Separators(";", ';'));
            _cbSeparator.Items.AddRange(separators.ToArray());
            _cbSeparator.SelectedItem = separators[0];
            IntitDoubleBufferGrid(_dgCSVImportValues);
            CardImportTemplate template1 = new CardImportTemplate(GetString(SEPARATED_PERSONAL_ID_TEMPLATE), new ColumnFromCSVFile(0), new ColumnFromCSVFile(1),
                null, new ColumnFromCSVFile(2), new ColumnFromCSVFile(3), CSVImportType.OverwriteNonEmptyData);
            CardImportTemplate template2 = new CardImportTemplate(GetString(COMBINED_PERSONAL_ID_TEMPLATE), null, null,
                new ColumnFromCSVFile(0), new ColumnFromCSVFile(1), new ColumnFromCSVFile(2), CSVImportType.OverwriteNonEmptyData);

            _cbTemplate.Items.Add(template1);
            _cbTemplate.Items.Add(template2);
            _cbTemplate.Enabled = false;

            SetValuesComboBoxPersonalIdSeparator();

            FillComboBoxCardSystem();
            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgCSVImportColumns);
            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgReport);

            _eReplacmentPin.Enabled = false;

            _importedCardCountChanged = new Action<Guid, int>(ImportedCardCountChanged);
            ImportedCardCountChangedHandler.Singleton.RegisterImportedCardCountChanged(_importedCardCountChanged);
        }

        private void FillComboBoxCardSystem()
        {
            if (InvokeRequired)
                Invoke(new DVoid2Void(FillComboBoxCardSystem));
            else
            {
                if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                    return;
                Exception ex = null;
                ICollection<CardSystem> cardSystems = CgpClient.Singleton.MainServerProvider.CardSystems.List(out ex);
                _cbCardSystem.Items.Clear();
                if (ex == null && cardSystems != null && cardSystems.Count > 0)
                {
                    _cbCardSystem.Items.AddRange(cardSystems.ToArray());
                    _cbCardSystem.SelectedItem = _cbCardSystem.Items[0];
                }
            }
        }

        private void SetValuesComboBoxPersonalIdSeparator()
        {
            _cbPersonalIdSeparator.Items.Clear();
            _cbPersonalIdSeparator.Items.Add(new ItemSeparatorType(string.Empty, string.Empty, PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_SYMBOL));
            _cbPersonalIdSeparator.Items.Add(new ItemSeparatorType(PERSONAL_ID_SEPARATOR_NORTH_STATES_NAME, GetString(PERSONAL_ID_SEPARATOR_NORTH_STATES_NAME), PERSONAL_ID_SEPARATOR_NORTH_STATES_SYMBOL));
            _cbPersonalIdSeparator.Items.Add(new ItemSeparatorType(PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_NAME, GetString(PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_NAME), PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_SYMBOL));
            _cbPersonalIdSeparator.Items.Add(new ItemSeparatorType(PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_NAME, GetString(PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_NAME), PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_SYMBOL));
            _cbPersonalIdSeparator.Items.Add(new ItemSeparatorType(PERSONAL_ID_SEPARATOR_NON_STANDART, GetString(PERSONAL_ID_SEPARATOR_NON_STANDART), PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_SYMBOL));
        }

        private void _bOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogCSVImport.ShowDialog() == DialogResult.OK)
            {
                _eFilePath.Text = openFileDialogCSVImport.FileName;
                SetDefaultHeaderCellsWidth();
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

            FillColumnComboBoxes();

            _cbTemplate.Enabled = true;
            _cbTemplate.SelectedItem = null;
            _cbPersonalIdSeparator.Enabled = true;
            _dgCSVImportValues.DataSource = new List<TableCardsRow>();
            _bImport.Enabled = true;
            _eReplacmentPin.Enabled = true;
        }

        private void FillColumnComboBoxes()
        {
            if (InvokeRequired)
                Invoke(new DVoid2Void(FillColumnComboBoxes));
            else
            {
                DataGridViewRow row = _dgCSVImportColumns.Rows[0];
                foreach (DataGridViewCell dataGridViewCell in row.Cells)
                {
                    if (dataGridViewCell is DataGridViewComboBoxCell)
                    {
                        DataGridViewComboBoxCell cell = dataGridViewCell as DataGridViewComboBoxCell;
                        if (cell != null)
                        {
                            cell.Items.Clear();

                            ColumnFromCSVFile defaultColumnFromCSVFile = new ColumnFromCSVFile(-1);
                            cell.Items.Add(defaultColumnFromCSVFile);
                            cell.ValueMember = "Name";
                            cell.Value = defaultColumnFromCSVFile;

                            for (int actColumn = 0; actColumn < _csvTable.Count; actColumn++)
                            {
                                ColumnFromCSVFile ColumnFromCSVFile = new ColumnFromCSVFile(actColumn);
                                cell.Items.Add(ColumnFromCSVFile);
                            }
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
                _dgCSVImportColumns.HorizontalScrollingOffset = e.NewValue;
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
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_enableColumnChanged)
                {
                    _dgCSVImportColumns_ComboBoxValueChanged();
                }
            }
            catch { }
        }

        private void _dgCSVImportColumns_ComboBoxValueChanged()
        {
            try
            {
                Dictionary<int, CSVColumn> dicActSelectedIndexes = new Dictionary<int, CSVColumn>();
                //if (columnIndex == -1)
                //{

                bool removeAllRows = true;

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
                                    if (csvColumn.FirstColumn != -1)
                                        removeAllRows = false;

                                    dicActSelectedIndexes.Add(cell.ColumnIndex - 1, csvColumn);
                                }
                                break;
                            }
                        }
                    }
                }

                if (removeAllRows)
                {
                    _dgCSVImportValues.DataSource = new List<TableCardsRow>();
                }
                else
                {
                    if (_csvTable.Count > 0 && dicActSelectedIndexes.Count > 0)
                    {
                        _dgCSVImportValues.SuspendLayout();

                        int rowCount = _csvTable[0].Count;

                        _pbLoadFileImport.Maximum = rowCount;
                        _pbLoadFileImport.Value = 0;

                        SafeThread<Dictionary<int, CSVColumn>, int>.StartThread(Do_dgCSVImportColumns_CellValueChanged, dicActSelectedIndexes, rowCount);
                    }
                }
            }
            catch { }
        }

        private void Do_dgCSVImportColumns_CellValueChanged(Dictionary<int, CSVColumn> dicActSelectedIndexes, int rowCount)
        {
            List<TableCardsRow> tableCardsRows = new List<TableCardsRow>();
            List<TableCardsRow> oldTableCardsRows = _dgCSVImportValues.DataSource as List<TableCardsRow>;

            try
            {
                string[] tableCardsRow = new string[5];
                for (int row = 0; row < rowCount; row++)
                {
                    for (int i = 0; i < tableCardsRow.Length; i++)
                    {
                        tableCardsRow[i] = string.Empty;
                    }

                    foreach (KeyValuePair<int, CSVColumn> kvp in dicActSelectedIndexes)
                    {
                        if (kvp.Value != null)
                        {
                            if (kvp.Value.FirstColumn >= 0)
                            {
                                if (kvp.Value is MixedColumnFromCSVFile && (kvp.Value as MixedColumnFromCSVFile).SecondColumn >= 0)
                                {
                                    if (kvp.Key < tableCardsRow.Length)
                                    {
                                        tableCardsRow[kvp.Key] = _csvTable[kvp.Value.FirstColumn][row] + (kvp.Value as MixedColumnFromCSVFile).Separator +
                                            _csvTable[(kvp.Value as MixedColumnFromCSVFile).SecondColumn][row];
                                    }
                                }
                                else
                                {
                                    if (kvp.Key < tableCardsRow.Length)
                                    {
                                        tableCardsRow[kvp.Key] = _csvTable[kvp.Value.FirstColumn][row];
                                    }
                                }
                            }
                            else
                            {
                                if (kvp.Key < tableCardsRow.Length)
                                {
                                    tableCardsRow[kvp.Key] = string.Empty;
                                }
                            }
                        }
                    }

                    bool importRow = true;
                    if (oldTableCardsRows != null && oldTableCardsRows.Count > row)
                    {
                        importRow = oldTableCardsRows[row].Column6;
                    }

                    List<string> rowList = tableCardsRow.ToList();
                    FillSpecialColumns(rowList);
                    tableCardsRow = rowList.ToArray();

                    tableCardsRows.Add(new TableCardsRow(tableCardsRow, importRow));
                    _pbLoadFileImport_NexStep(row + 1);
                }
            }
            catch { }

            End_dgCSVImportColumns_CellValueChanged(tableCardsRows);
        }

        private void FillSpecialColumns(List<string> tableCardsRow)
        {
            if (InvokeRequired)
                Invoke(new Action<List<string>>(FillSpecialColumns), tableCardsRow);
            else
            {
                if (tableCardsRow[2] == string.Empty)
                {
                    tableCardsRow[2] = tableCardsRow[0] + _actMixedColumSeparator + tableCardsRow[1];
                }
                else if (tableCardsRow[0] == string.Empty && tableCardsRow[1] == string.Empty)
                {
                    if (_actMixedColumSeparator == string.Empty)
                    {
                        if (tableCardsRow[2].Length == 10)
                        {
                            tableCardsRow[0] = tableCardsRow[2].Substring(0,6);
                            tableCardsRow[1] = tableCardsRow[2].Substring(6,4);
                        }
                    }
                    else
                    {
                        if (_actMixedColumSeparator.Length != 1) return;
                        string[] parsedPersonId = tableCardsRow[2].Split(_actMixedColumSeparator[0]);
                        if (parsedPersonId.Length == 2)
                        {
                            tableCardsRow[0] = parsedPersonId[0];
                            tableCardsRow[1] = parsedPersonId[1];
                        }
                    }
                }
            }
        }

        private void End_dgCSVImportColumns_CellValueChanged(List<TableCardsRow> tableCardsRows)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<List<TableCardsRow>>(End_dgCSVImportColumns_CellValueChanged), tableCardsRows);
            }
            else
            {
                _pbLoadFileImport.Value = 0;
                _dgCSVImportValues.DataSource = tableCardsRows;
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
            CardImportTemplate template = _cbTemplate.SelectedItem as CardImportTemplate;
            if (template != null)
            {
                FillColumnComboBoxes();
                _enableColumnChanged = false;

                if (template.Column1 != null)
                {
                    DataGridViewComboBoxCell cell = _dgCSVImportColumns.Rows[0].Cells[1] as DataGridViewComboBoxCell;
                    if (cell != null && cell.Items != null)
                    {
                        foreach (object obj in cell.Items)
                        {
                            if (obj != null && obj.ToString() == template.Column1.ToString())
                            {
                                cell.Value = obj;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                    cell = new DataGridViewComboBoxCell();
                    cell.Items.Add(GetString("ParsedColumn"));
                    cell.Value = cell.Items[0];
                    _dgCSVImportColumns.Rows[0].Cells[1] = cell;
                }

                if (template.Column2 != null)
                {
                    DataGridViewComboBoxCell cell = _dgCSVImportColumns.Rows[0].Cells[2] as DataGridViewComboBoxCell;
                    if (cell != null && cell.Items != null)
                    {
                        foreach (object obj in cell.Items)
                        {
                            if (obj != null && obj.ToString() == template.Column2.ToString())
                            {
                                cell.Value = obj;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                    cell = new DataGridViewComboBoxCell();
                    cell.Items.Add(GetString("ParsedColumn"));
                    cell.Value = cell.Items[0];
                    _dgCSVImportColumns.Rows[0].Cells[2] = cell;
                }

                if (template.Column3 != null)
                {
                    DataGridViewComboBoxCell cell = _dgCSVImportColumns.Rows[0].Cells[3] as DataGridViewComboBoxCell;
                    if (cell != null && cell.Items != null)
                    {
                        foreach (object obj in cell.Items)
                        {
                            if (obj != null && obj.ToString() == template.Column3.ToString())
                            {
                                cell.Value = obj;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                    cell = new DataGridViewComboBoxCell();
                    cell.Items.Add(GetString("MixedColumn"));
                    cell.Value = cell.Items[0];
                    _dgCSVImportColumns.Rows[0].Cells[3] = cell;
                }

                if (template.Column4 != null)
                {
                    DataGridViewComboBoxCell cell = _dgCSVImportColumns.Rows[0].Cells[4] as DataGridViewComboBoxCell;
                    if (cell != null && cell.Items != null)
                    {
                        foreach (object obj in cell.Items)
                        {
                            if (obj != null && obj.ToString() == template.Column4.ToString())
                            {
                                cell.Value = obj;
                                break;
                            }
                        }
                    }
                }

                if (template.Column5 != null)
                {
                    DataGridViewComboBoxCell cell = _dgCSVImportColumns.Rows[0].Cells[5] as DataGridViewComboBoxCell;
                    if (cell != null && cell.Items != null)
                    {
                        foreach (object obj in cell.Items)
                        {
                            if (obj != null && obj.ToString() == template.Column5.ToString())
                            {
                                cell.Value = obj;
                                break;
                            }
                        }
                    }
                }

                _enableColumnChanged = true;
                _dgCSVImportColumns_ComboBoxValueChanged();
            }
        }

        private void _bImport_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            DisableControlBeforeImport();

            List<TableCardsRow> rows = _dgCSVImportValues.DataSource as List<TableCardsRow>;

            if (rows == null || rows.Count == 0)
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("ErrorNoDataForImport"));
                AllowControlsAfterImpot();
                return;
            }

            if (_eReplacmentPin.Text != string.Empty)
            {
                if (InvalidPinFormat(_eReplacmentPin.Text))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eReplacmentPin,
                        string.Format(
                            CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongPinLength"),
                            Card.MinimalPinLength,
                            Card.MaximalPinLength),
                        CgpClient.Singleton.ClientControlNotificationSettings);
                    return;
                }

                if (NotSecurePin(_eReplacmentPin.Text))
                {
                    Contal.IwQuick.UI.ControlNotification.Singleton.Error(Contal.IwQuick.UI.NotificationPriority.JustOne, _eReplacmentPin,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorNotSecurityPin"), CgpClient.Singleton.ClientControlNotificationSettings);
                    return;
                }
            }
            
            _pbLoadFileImport.Value = 0;
            _dgReport.DataSource = new List<CSVImportPerson>();

            CardSystem cardSystem = _chbCardSystem.Checked ? _cbCardSystem.SelectedItem as CardSystem : null;
            DoControlForPersonalId();
            SafeThread<List<TableCardsRow>, CardSystem>.StartThread(DoImport, rows, cardSystem);
        }

        private void DisableControlBeforeImport()
        {
            _cbSeparator.Enabled = false;
            _eFilePath.Enabled = false;
            _bOpenFile.Enabled = false;
            _bImport.Enabled = false;
            _cbTemplate.Enabled = false;
            _dgCSVImportColumns.Enabled = false;
            _dgCSVImportValues.Enabled = false;
            _cbPersonalIdSeparator.Enabled = false;
            _eReplacmentPin.Enabled = false;
        }

        private void AllowControlsAfterImpot()
        {
            _cbSeparator.Enabled = true;
            _eFilePath.Enabled = true;
            _bOpenFile.Enabled = true;
            _bImport.Enabled = true;
            _cbTemplate.Enabled = true;
            _dgCSVImportColumns.Enabled = true;
            _dgCSVImportValues.Enabled = true;
            _cbPersonalIdSeparator.Enabled = true;
            _eReplacmentPin.Enabled = true;
        }

        private bool _doControlForPersonalId = true;
        private void DoControlForPersonalId()
        {
            if (InvokeRequired)
            {
                Invoke(new Contal.IwQuick.DVoid2Void(DoControlForPersonalId));
            }
            else
            {
                if (_cbPersonalIdSeparator.SelectedItem is ItemSeparatorType)
                {
                    ItemSeparatorType st = (ItemSeparatorType)_cbPersonalIdSeparator.SelectedItem;
                    if (st.Name == PERSONAL_ID_SEPARATOR_NON_STANDART)
                    {
                        _doControlForPersonalId = false;
                        return;
                    }
                }
                _doControlForPersonalId = true;
            }
        }

        public void DoImport(List<TableCardsRow> rows, CardSystem cardSystem)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            List<ImportCardData> importCardsData = new List<ImportCardData>();
            List<CSVImportCard> cardsReport = new List<CSVImportCard>();

            List<int> replacmentPinIndexes = new List<int>();
            int allCardsToImport = 0;

            if (rows != null && rows.Count > 0)
            {
                foreach (TableCardsRow row in rows)
                {
                    if (row.Column6)
                    {
                        allCardsToImport++;
                        try
                        {
                            Card card = new Card();
                            card.Number = row.Column4;
                            if (NotValidPersonalIdFormat(row.Column3))
                            {
                                cardsReport.Add(new CSVImportCard(Guid.Empty, card.Number, CSVImportResult.InvalidPersonalIdFormat));
                                continue;
                            }

                            bool replacedIndex = false;
                            string pin = row.Column5;
                            if (InvalidPinFormat(pin))
                            {
                                if (_eReplacmentPin.Text != string.Empty)
                                {
                                    pin = _eReplacmentPin.Text;
                                    replacedIndex = true;
                                }
                                else
                                {
                                    cardsReport.Add(new CSVImportCard(Guid.Empty, card.Number, CSVImportResult.PinInvalidFormat));
                                    continue;
                                }
                            }
                            if (NotSecurePin(pin))
                            {
                                if (_eReplacmentPin.Text != string.Empty)
                                {
                                    pin = _eReplacmentPin.Text;
                                    replacedIndex = true;
                                }
                                else
                                {
                                    cardsReport.Add(new CSVImportCard(Guid.Empty, card.Number, CSVImportResult.PinNotSecure));
                                    continue;
                                }
                            }

                            card.Pin = IwQuick.Crypto.QuickHashes.GetCRC32String(pin);
                            card.PinLength = (byte)pin.Length;

                            if (replacedIndex)
                                replacmentPinIndexes.Add(importCardsData.Count);

                            ImportCardData importCardData = new ImportCardData(row.Column3, row.Column4, pin, (byte)pin.Length);
                            importCardsData.Add(importCardData);
                        }
                        catch { }
                    }
                }
            }

            List<CSVImportCard> importCardsReport = null;
            bool licenceRestriction = false;
            int importedCardsCount = 0;
            Guid cardSystemGuid = Guid.Empty;
            if (cardSystem != null)
                cardSystemGuid = cardSystem.IdCardSystem;

            _isRunningImport = true;
            try
            {
                _pbLoadFileImport.Maximum = importCardsData.Count;
                if (!CgpClient.Singleton.MainServerProvider.Cards.ImportCards(_formIdentification, importCardsData, CSVImportType.OverwriteOnConflict, cardSystemGuid, out importCardsReport, out licenceRestriction, out importedCardsCount))
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
                    return;
                }
            }
            catch { }
            _isRunningImport = false;

            List<CSVCardImportRow> report = new List<CSVCardImportRow>();

            if (cardsReport != null && cardsReport.Count > 0)
            {
                foreach (CSVImportCard csvImportCard in cardsReport)
                {
                    if (csvImportCard != null)
                        report.Add(new CSVCardImportRow(csvImportCard, false));
                }
            }

            int reportedRowIndex = 0;
            if (importCardsReport != null && importCardsData.Count > 0)
            {
                foreach (CSVImportCard csvImportCard in importCardsReport)
                {
                    if (csvImportCard != null)
                    {
                        report.Add(new CSVCardImportRow(csvImportCard, replacmentPinIndexes.Contains(reportedRowIndex)));
                    }

                    reportedRowIndex++;
                }
            }

            EndImport(report, importedCardsCount, allCardsToImport);
        }

        private bool NotValidPersonalIdFormat(string personalId)
        {
            if (!_doControlForPersonalId) return false;

            if (_actMixedColumSeparator == string.Empty)
            {
                if (personalId.Length == 10)
                    return false;
            }
            else
            {
                if (_actMixedColumSeparator.Length != 1) return true;
                string[] separateId = personalId.Split(_actMixedColumSeparator[0]);
                if (separateId.Length == 2)
                {
                    if (separateId[0].Length == 6 && separateId[1].Length == 4)
                        return false;
                }
            }
            return true;
        }

        private bool NotSecurePin(string pin)
        {
            if (!GeneralOptionsForm.Singleton.RequiredSecurePin) return false;

            if (pin.Length > 2)
            {
                if (pin[0] == pin[1])
                {
                    for (int i = 2; i < pin.Length; i++)
                    {
                        if (pin[i - 1] != pin[i])
                            return false;
                    }
                }
                else if (pin[0] == pin[1] + 1)
                {
                    for (int i = 2; i < pin.Length; i++)
                    {
                        if (pin[i - 1] != pin[i] + 1)
                            return false;
                    }
                }
                else if (pin[0] == pin[1] - 1)
                {
                    for (int i = 2; i < pin.Length; i++)
                    {
                        if (pin[i - 1] != pin[i] - 1)
                            return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private bool InvalidPinFormat(string pin)
        {
            if (pin.Length < Card.MinimalPinLength || pin.Length > Card.MaximalPinLength)
            {
                return true;
            }
            return false;
        }

        public void EndImport(List<CSVCardImportRow> report, int importedCardsCount, int allCardsToImport)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<List<CSVCardImportRow>, int, int>(EndImport), report, importedCardsCount, allCardsToImport);
            }
            else
            {
                _pbLoadFileImport.Value = 0;

                if (allCardsToImport > importedCardsCount)
                {
                    Contal.IwQuick.UI.Dialog.Info(GetString("InfoImportFinished") + Environment.NewLine + Environment.NewLine +
                        GetString("InfoImportedCardsCount") + " " + importedCardsCount + Environment.NewLine +
                        GetString("InfoNotImportedCardsCount") + " " + (allCardsToImport - importedCardsCount));
                }
                else
                {
                    Contal.IwQuick.UI.Dialog.Info(GetString("InfoImportFinished") + Environment.NewLine + Environment.NewLine +
                        GetString("InfoImportedCardsCount") + " " + importedCardsCount);
                }

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
                List<TableCardsRow> rows = _dgCSVImportValues.DataSource as List<TableCardsRow>;
                if (rows != null && rows.Count > 0)
                {
                    foreach (TableCardsRow row in rows)
                    {
                        row.Column6 = (bool)cell.EditedFormattedValue;
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

            List<CSVCardImportRow> rows = _dgReport.DataSource as List<CSVCardImportRow>;
            if (rows != null && rowIndex >= 0 && rows.Count > rowIndex)
            {
                Card card = CgpClient.Singleton.MainServerProvider.Cards.GetObjectById(rows[rowIndex].IdCard);

                if (card != null)
                {
                    CardsForm.Singleton.OpenEditDialog(card);
                }
            }
        }

        private void _chbCardSystem_CheckedChanged(object sender, EventArgs e)
        {
            _cbCardSystem.Enabled = _chbCardSystem.Checked;
        }

        private void _cbPersonalIdSeparator_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_cbPersonalIdSeparator.SelectedItem is ItemSeparatorType)
            {
                ItemSeparatorType st = (ItemSeparatorType)_cbPersonalIdSeparator.SelectedItem;
                _actMixedColumSeparator = st.Separator;
                _dgCSVImportColumns_ComboBoxValueChanged();
            }
        }

        private void ImportedCardCountChanged(Guid formIdentification, int importedCardCount)
        {
            if (_isRunningImport && _formIdentification == formIdentification)
                _pbLoadFileImport_NexStep(importedCardCount);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (_importedCardCountChanged != null)
                ImportedCardCountChangedHandler.Singleton.UnregisterImportedCardCountChanged(_importedCardCountChanged);
        }
    }

    public class CardImportTemplate
    {
        private string _name;
        private ColumnFromCSVFile _column1;
        private ColumnFromCSVFile _column2;
        private ColumnFromCSVFile _column3;
        private ColumnFromCSVFile _column4;
        private ColumnFromCSVFile _column5;
        CSVImportType _importype = CSVImportType.OverwriteOnConflict;

        public ColumnFromCSVFile Column1 { get { return _column1; } }
        public ColumnFromCSVFile Column2 { get { return _column2; } }
        public ColumnFromCSVFile Column3 { get { return _column3; } }
        public ColumnFromCSVFile Column4 { get { return _column4; } }
        public ColumnFromCSVFile Column5 { get { return _column5; } }

        public CardImportTemplate(string name, ColumnFromCSVFile column1, ColumnFromCSVFile column2, ColumnFromCSVFile column3,
            ColumnFromCSVFile column4, ColumnFromCSVFile column5, CSVImportType importType)
        {
            _name = name;
            _column1 = column1;
            _column2 = column2;
            _column3 = column3;
            _column4 = column4;
            _column5 = column5;
            _importype = importType;
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class TableCardsRow
    {
        private string _column1;
        private string _column2;
        private string _column3;
        private string _column4;
        private string _column5;
        private bool _column6;

        public string Column1 { get { return _column1; } }
        public string Column2 { get { return _column2; } }
        public string Column3 { get { return _column3; } }
        public string Column4 { get { return _column4; } }
        public string Column5 { get { return _column5; } }
        public bool Column6 { get { return _column6; } set { _column6 = value; } }

        public TableCardsRow(string[] parameters, bool importRow)
        {
            if (parameters.Length == 5)
            {
                _column1 = parameters[0];
                _column2 = parameters[1];
                _column3 = parameters[2];
                _column4 = parameters[3];
                _column5 = parameters[4];
                _column6 = importRow;
            }
        }
    }

    public class CSVCardImportRow
    {
        private Guid _idCard;
        private string _number;
        private string _importResult;

        public Guid IdCard { get { return _idCard; } }
        public string Number { get { return _number; } }
        public string ImportResult { get { return _importResult; } }

        public CSVCardImportRow(CSVImportCard importCard, bool pinReplaced)
        {
            if (importCard != null)
            {
                _idCard = importCard.IdCard;
                _number = importCard.Number;
                _importResult = CgpClient.Singleton.LocalizationHelper.GetString("CSVImportResult_" + importCard.ImportResult.ToString());

                if (pinReplaced && (importCard.ImportResult == CSVImportResult.Added || importCard.ImportResult == CSVImportResult.Overwritten))
                    _importResult += ". " + CgpClient.Singleton.LocalizationHelper.GetString("InfoInvalidPinAndReplaced");
            }
        }
    }

    public enum SeparatorType : byte
    {
        MixedColumns = 0,
        NorthStates = 1,
        CentalEuropeStates = 2,
        WithoOutSeparator = 3,
        NonStandard = 4
    }

    public class ItemSeparatorType
    {
        private string _name;
        private string _translate;
        private string _symbolSeparator;
        private SeparatorType _separatorType;

        public string Separator
        {
            get { return _symbolSeparator; }
        }

        public string Name
        {
            get { return _name; }
        }

        public SeparatorType SeparatorType { get { return _separatorType; } }

        public ItemSeparatorType(string type, string translate, string separator)
        {
            _name = type;
            _translate = translate;
            _symbolSeparator = separator;

            if (_name == string.Empty)
                _separatorType = SeparatorType.MixedColumns;
            else
                switch (_name)
                {
                    case CSVCardImportDialog.PERSONAL_ID_SEPARATOR_NORTH_STATES_NAME:
                        _separatorType = SeparatorType.NorthStates;
                        break;
                    case CSVCardImportDialog.PERSONAL_ID_SEPARATOR_CENTRAL_EUROPE_STATES_NAME:
                        _separatorType = SeparatorType.CentalEuropeStates;
                        break;
                    case CSVCardImportDialog.PERSONAL_ID_SEPARATOR_WITHOUT_SEPARATOR_NAME:
                        _separatorType = SeparatorType.WithoOutSeparator;
                        break;
                    case CSVCardImportDialog.PERSONAL_ID_SEPARATOR_NON_STANDART:
                        _separatorType = SeparatorType.NonStandard;
                        break;
                }
        }

        public override string ToString()
        {
            return _translate;
        }
    }
}
