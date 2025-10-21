using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Contal.IwQuick.Data;
using Contal.IwQuick.UI;
using Contal.IwQuick.Parsing;
using System.IO;

namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    public partial class LocalizationAssistentMainForm : Form
    {
        private readonly LocalizationHelper _localizationHelper;
        private Localization _devActualLocalization;
        private Localization _transActualLocalization;
        private bool _translationOk;
        private bool _isPackageLoaded;
        private bool _wasChangeMade;
        private string _packageFileNamePath;
        private string _locFilePath;
        private bool _isNoTranslate;
     
        private string _passwd;
        private string Passwd
        {
            set
            {
                _passwd = value;
                if (_passwd.Equals(string.Empty))
                {
                    _btNewLoc.Enabled = false;
                    _btSaveLocData.Enabled = false;
                    _btLoadLocData.Enabled = false;
                    _btSynchAllSym.Enabled = false;
                    _btGenerate.Enabled = false;
                    _btSaveLocalizationDataAs.Enabled = false;
                }
                else
                {
                    _btSaveLocData.Enabled = true;
                    _btLoadLocData.Enabled = true;
                    _btNewLoc.Enabled = true;
                    _btSynchAllSym.Enabled = true;
                    _btGenerate.Enabled = true;
                    _btSaveLocalizationDataAs.Enabled = true;
                }
            }
        }

        public LocalizationAssistentMainForm()
        {
            InitializeLocalizationAssistant();
            _isNoTranslate = false;
            _localizationHelper = new LocalizationHelper();
        }

        public LocalizationAssistentMainForm(string actualLanguage, string resourceFolderPath, LocalizationHelper localizationHelper)
        {
            InitializeLocalizationAssistant();

            string resourceFile = resourceFolderPath + @"\Localization." + actualLanguage + ".resx";

            if (File.Exists(resourceFile))
            {
                _btDisDeveloperMode.Visible = true;
                _btReloadPackage.Visible = true;
                _locFilePath = resourceFolderPath;
                _localizationHelper = localizationHelper;
                _localizationHelper.WordsAdded += OnWordsAdded;
                _isNoTranslate = true;
                _tcMain.SelectedIndex = 1;
                _rbDevMod.Checked = true;
                Passwd = "developerMode";
                Localization localization = _localizationHelper.LoadResxLocalization(resourceFolderPath + @"\Localization." + actualLanguage + ".resx", actualLanguage, false);
                RefreshLocalizationsComboBox();
                SelectLocalization(localization.Language);
            }
            else
            {
                Dialog.Error("Resource file " + resourceFile + " does not exists!");
                System.Diagnostics.Debug.WriteLine("Resource file " + resourceFile + " does not exists!");
            }
        }

        private readonly SyncDictionary<string,bool> _wordsAdded = new SyncDictionary<string, bool>(50); 

        private void OnWordsAdded(string word)
        {
            Thread2UI.Invoke(this, () =>
            {
                try
                {
                    bool dummy;
                    bool newlyAdded = _wordsAdded.GetOrAddValue(word, out dummy, true);

                    if (newlyAdded)
                    {
                        int addRow = _dgwLocalization.Rows.Add();
                        var cell = _dgwLocalization.Rows[addRow].Cells[0];
                        cell.Value = word;
                        cell.Style.BackColor = Color.Red;
                    }
                }
                catch
                {

                }
            });
        }

        private void InitializeLocalizationAssistant()
        {
            InitializeComponent();
            _passwd = string.Empty;
            _devActualLocalization = null;
            _transActualLocalization = null;
            ReloadLocalizationData();
            _btDeleteTransItem.Enabled = false;
            _btInsertNewRow.Enabled = false;
            _translationOk = false;
            _isPackageLoaded = false;
            _wasChangeMade = false;
            _packageFileNamePath = string.Empty;
            _locFilePath = string.Empty;
        }

        private void _rbTransMode_CheckedChanged(object sender, EventArgs e)
        {
            _tcMain.SelectedIndex = 0;
        }

        private void _rbDevMod_CheckedChanged(object sender, EventArgs e)
        {
            if (_rbDevMod.Checked)
            {
                if (_isNoTranslate)
                {
                    _isNoTranslate = false;
                    _passwd = "password";
                }
                if (_passwd.Equals(string.Empty))
                {
                    if (Dialog.Question("Do you want start developer mode?"))
                    {
                        PasswordDialog passDialog = new PasswordDialog();
                        passDialog.ShowDialog();
                        if (passDialog.DialogResult == DialogResult.OK)
                        {
                            Passwd = passDialog.Password;
                            _btDisDeveloperMode.Visible = true;
                            _btReloadPackage.Visible = true;
                            _tcMain.SelectedIndex = 1;
                        }
                        else if (passDialog.DialogResult == DialogResult.Abort)
                        {
                            Dialog.Error("Wrong password");
                            _rbTransMode.Checked = true;
                        }
                        else
                        {
                            _rbTransMode.Checked = true;
                        }
                    }
                    else
                    {
                        _rbTransMode.Checked = true;
                    }
                }
                else
                {
                    _tcMain.SelectedIndex = 1;
                }
            }
        }

        private void _btDisDeveloperMode_Click(object sender, EventArgs e)
        {
            Passwd = string.Empty;
            _rbTransMode.Checked = true;
            _btDisDeveloperMode.Visible = false;
            _btReloadPackage.Visible = false;
            _btClosePackage.Enabled = false;
        }

        private void _btExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _dgwLocalization_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_dgwLocalization.Rows[_dgwLocalization.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells[0].Value == null)
            {
                _dgwLocalization.Rows[_dgwLocalization.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells[0].Value = "Name" + GetNextIndex();
                _dgwLocalization.Rows[_dgwLocalization.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells[1].Value = "<value>";
            }
            if (e.RowIndex != _dgwLocalization.Rows.GetLastRow(DataGridViewElementStates.Visible))
            {
                _dgwLocalization.Rows[_dgwLocalization.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells[0].Value = null;
                _dgwLocalization.Rows[_dgwLocalization.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells[1].Value = null;
            }
        }

        private int GetNextIndex()
        {
            int strIndex = 1;
            foreach (DataGridViewRow row in _dgwLocalization.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString().Length >= 4 && row.Cells[0].Value.ToString().Substring(0, 4).Equals("Name"))
                {
                    int outNum;
                    if (int.TryParse(row.Cells[0].Value.ToString().Substring(4, 1), out outNum))
                    {
                        if (outNum >= strIndex)
                        {
                            strIndex = outNum + 1;
                        }
                    }
                }
            }
            return strIndex;
        }

        private void _cbLocalizations_Click(object sender, EventArgs e)
        {
            if (_cbLocalizations.Items.Count == 0)
            {
                if (Dialog.Question("Do you want to create new localization?"))
                {
                    CreateLocalization();
                }
            }
        }

        private void _dgwLocalization_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (IsInDatatagridView(e.FormattedValue.ToString(), e.RowIndex))
                {
                    _dgwLocalization.Rows[e.RowIndex].ErrorText = "Name already exists!";
                    e.Cancel = true;
                }
                if (e.FormattedValue.ToString().Equals(string.Empty))
                {
                    _dgwLocalization.Rows[e.RowIndex].ErrorText = "Name cannot be empty string";
                    e.Cancel = true;
                }
            }
            if (e.ColumnIndex == 1)
            {
                if (e.FormattedValue.ToString().Equals(string.Empty))
                {
                    _dgwLocalization.Rows[e.RowIndex].ErrorText = "Value cannot be empty string";
                    e.Cancel = true;
                }
            }
            if (e.Cancel == false)
            {
                menuStrip1.Enabled = true;
                statusStrip1.Enabled = true;
                _cbLocalizations.Enabled = true;
                _btInsertNewRow.Enabled = true;
                _btDeleteTransItem.Enabled = true;
                _rbTransMode.Enabled = true;
                _rbDevMod.Enabled = true;
                _btDisDeveloperMode.Enabled = true;
                _btReloadPackage.Enabled = true;
            }
        }

        private bool IsInDatatagridView(string value, int rowIndex)
        {
            int i = 0;
            foreach (DataGridViewRow row in _dgwLocalization.Rows)
            {
                var cell = row.Cells[0];

                if (i != rowIndex 
                    && cell.Value != null 
                    && cell.Value.Equals(value))
                {
                    return true;
                }
                i++;
            }
            return false;
        }

        private void _dgwLocalization_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _dgwLocalization.Rows[e.RowIndex].ErrorText = string.Empty;
            if (_dgwLocalization.Rows[e.RowIndex].Cells[1].Value == null || _dgwLocalization.Rows[e.RowIndex].Cells[1].Value.ToString().Equals(string.Empty) && _dgwLocalization.Rows[e.RowIndex].Cells[1].Value == null || _dgwLocalization.Rows[e.RowIndex].Cells[1].Value.ToString().Equals(string.Empty))
            {
                _dgwLocalization.Rows[e.RowIndex].Cells[1].Value = "<value>";
            }
            if (!_dgwLocalization.Rows[e.RowIndex].IsNewRow)
            {
                try
                {
                    _devActualLocalization.InsertTranslationItem(_dgwLocalization.Rows[e.RowIndex].Cells[0].Value.ToString(), _dgwLocalization.Rows[e.RowIndex].Cells[1].Value.ToString(), (null != _dgwLocalization.Rows[e.RowIndex].Cells[2].Value ? _dgwLocalization.Rows[e.RowIndex].Cells[2].Value.ToString() : string.Empty));
                    _dgwLocalization.Rows[e.RowIndex].Cells[0].ReadOnly = true;
                    _dgwLocalization.Rows[e.RowIndex].Cells[0].Style.BackColor = Color.LightGray;
                }
                catch
                {
                    _devActualLocalization.UpdateTranslationItem(_dgwLocalization.Rows[e.RowIndex].Cells[0].Value.ToString(), _dgwLocalization.Rows[e.RowIndex].Cells[1].Value.ToString(), (null != _dgwLocalization.Rows[e.RowIndex].Cells[2].Value ? _dgwLocalization.Rows[e.RowIndex].Cells[2].Value.ToString() : string.Empty));
                }
                _wasChangeMade = true;
            }
        }

        private void _btNewLoc_Click(object sender, EventArgs e)
        {
            CreateLocalization();
        }

        private void CreateLocalization()
        {
            LocalizationDialog locDialog = new LocalizationDialog();
            if (locDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Localization firstLocalization = _localizationHelper.GetFirstLocalization();
                    Localization localization = _localizationHelper.CreateLocalization(locDialog.Language, locDialog.IsMasterResource);
                    if (null != firstLocalization)
                    {
                        if (Dialog.Question("Do you want to copy translation symbols from existing \"" + firstLocalization.Language + "\" ?"))
                        {
                            localization.CopySymbolsFrom(firstLocalization, true);
                        }
                    }
                    RefreshLocalizationsComboBox();
                    SelectLocalization(locDialog.Language);
                    _wasChangeMade = true;
                }
                catch (Exception aError)
                {
                    Dialog.Error(aError.Message);
                }
            }
        }

        private void _cbLocalizations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (0 <= _cbLocalizations.SelectedIndex)
            {
                _devActualLocalization = (Localization)_cbLocalizations.SelectedItem;
            }
            else
            {
                _devActualLocalization = null;
            }
            ReloadLocalizationData();
            if (_cbLocalizations.Text.Equals(string.Empty))
            {
                _dgwLocalization.ReadOnly = true;
                _btDeleteTransItem.Enabled = false;
                _btInsertNewRow.Enabled = false;
            }
            else
            {
                _dgwLocalization.ReadOnly = false;
                _btDeleteTransItem.Enabled = true;
                _btInsertNewRow.Enabled = true;
            }
        }

        private void ReloadLocalizationData()
        {
            _dgwLocalization.Rows.Clear();
            if (null == _devActualLocalization)
            {
                _dgwLocalization.Rows[0].Cells[0].Value = "Name1";
                _dgwLocalization.Rows[0].Cells[1].Value = "<value>";
            }
            else
            {
                foreach (KeyValuePair<string, TranslationItem> pair in _devActualLocalization.Symbols)
                {
                    int rowAdd = _dgwLocalization.Rows.Add();
                    _dgwLocalization.Rows[rowAdd].Cells[0].Value = pair.Key;
                    _dgwLocalization.Rows[rowAdd].Cells[0].ReadOnly = true;
                    _dgwLocalization.Rows[rowAdd].Cells[0].Style.BackColor = Color.LightGray;
                    if (pair.Value.Value.Equals(string.Empty))
                    {
                        _dgwLocalization.Rows[rowAdd].Cells[1].Value = "<value>";
                    }
                    else
                    {
                        _dgwLocalization.Rows[rowAdd].Cells[1].Value = pair.Value.Value;
                    }
                    _dgwLocalization.Rows[rowAdd].Cells[2].Value = pair.Value.Comment;
                }
                if (_dgwLocalization.Rows.Count > 1)
                {
                    foreach (DataGridViewRow row in _dgwLocalization.Rows)
                    {
                        if (row.IsNewRow)
                        {
                            row.Cells[0].Value = null;
                            row.Cells[1].Value = null;
                        }
                    }
                }
            }
        }

        private void RefreshLocalizationsComboBox()
        {
            _cbLocalizations.Items.Clear();
            foreach (Localization aLocalization in _localizationHelper)
            {
                _cbLocalizations.Items.Add(aLocalization);
            }
            int selectedItem = _cbNonMasterLangs.SelectedIndex;
            _cbNonMasterLangs.Items.Clear();
            foreach (Localization aLocalization in _localizationHelper)
            {
                if (aLocalization.IsMasterResource)
                {
                    _labMasterLang.Text = aLocalization.ToString();
                    continue;
                }
                _cbNonMasterLangs.Items.Add(aLocalization);
            }
            if (_cbNonMasterLangs.Items.Count > 0)
            {
                _cbNonMasterLangs.SelectedIndex = selectedItem;
            }
        }

        private void SelectLocalization(string language)
        {
            if (Validator.IsNullString(language))
            {
                return;
            }
            for (int i = 0; i < _cbLocalizations.Items.Count; i++)
            {
                Localization item = (Localization)_cbLocalizations.Items[i];
                if (null == item)
                {
                    continue;
                }
                if (item.Language == language)
                {
                    _cbLocalizations.SelectedIndex = i;
                    break;
                }
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N && !_passwd.Equals(string.Empty))
            {
                CreateLocalization();
                return;
            }
            if (e.Control && e.KeyCode == Keys.D)
            {
                if (_dgwLocalization.SelectedCells.Count > 0)
                {
                    foreach (DataGridViewCell cell in _dgwLocalization.SelectedCells)
                    {
                        if (!_dgwLocalization.Rows[cell.RowIndex].IsNewRow)
                        {
                            _devActualLocalization.DeleteTranslationItem(_dgwLocalization.Rows[cell.RowIndex].Cells[0].Value.ToString());
                            _dgwLocalization.Rows.Remove(_dgwLocalization.Rows[cell.RowIndex]);
                            _wasChangeMade = true;
                        }
                    }
                    if (_dgwLocalization.SelectedRows.Count == 1)
                    {
                        _dgwLocalization.Rows[0].Cells[0].Value = "Name1";
                        _dgwLocalization.Rows[0].Cells[1].Value = "<value>";
                    }
                    return;
                }
                
                Dialog.Info("Please select tranlation item");
                return;
            }
            if (e.Control && !e.Alt && e.KeyCode == Keys.L)
            {
                _btLOadPackage.PerformClick();
                return;
            }
            if (e.Control && !e.Alt && e.KeyCode == Keys.S)
            {
                _btSavePackage.PerformClick();
                return;
            }
            if (e.Control && e.Alt && e.KeyCode == Keys.S && !_passwd.Equals(string.Empty))
            {
                _btSaveLocData.PerformClick();
                return;
            }
            if (e.Control && e.Alt && e.KeyCode == Keys.L && !_passwd.Equals(string.Empty))
            {
                _btLoadLocData.PerformClick();
                return;
            }
            if (e.Control && e.KeyCode == Keys.I)
            {
                _dgwLocalization.CurrentCell = _dgwLocalization[0, _dgwLocalization.Rows.Count - 1];
                _dgwLocalization.BeginEdit(false);
            }
        }

        private void _btSaveLocData_Click(object sender, EventArgs e)
        {
            if (_devActualLocalization != null)
            {
                if (_devActualLocalization.Data.Count > 0)
                {
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    DialogResult dialogResult = DialogResult.None;
                    if (_locFilePath.Equals(string.Empty))
                    {
                        dialogResult = dialog.ShowDialog();
                        _locFilePath = dialog.SelectedPath;
                    }
                    else if (dialogResult == DialogResult.None)
                    {
                        dialog.SelectedPath = _locFilePath;
                    }

                    if (dialogResult != DialogResult.Cancel)
                    {
                        SaveLocalization(dialog.SelectedPath);
                    }
                }
                else
                {
                    Dialog.Warning("Current localization is empty");
                }
            }
            else
            {
                Dialog.Warning("No localization is selected");
            }
        }

        private void _dgwLocalization_Click(object sender, EventArgs e)
        {
            if (_dgwLocalization.ReadOnly)
            {
                Dialog.Info("Please choose localization");
            }
        }

        private void _dgwLocalization_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.Cancel = false;
            }
            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
            {
                menuStrip1.Enabled = false;
                statusStrip1.Enabled = false;
                _cbLocalizations.Enabled = false;
                _btInsertNewRow.Enabled = false;
                _btDeleteTransItem.Enabled = false;
                _rbTransMode.Enabled = false;
                _rbDevMod.Enabled = false;
                _btDisDeveloperMode.Enabled = false;
                _btReloadPackage.Enabled = false;
            }
        }

        private void _btDeleteTransItem_Click(object sender, EventArgs e)
        {
            if (_dgwLocalization.SelectedCells.Count > 0)
            {
                foreach (DataGridViewCell cell in _dgwLocalization.SelectedCells)
                {
                    if (!_dgwLocalization.Rows[cell.RowIndex].IsNewRow)
                    {
                        _devActualLocalization.DeleteTranslationItem(_dgwLocalization.Rows[cell.RowIndex].Cells[0].Value.ToString());
                        _dgwLocalization.Rows.Remove(_dgwLocalization.Rows[cell.RowIndex]);
                        _wasChangeMade = true;
                    }
                }
                if (_dgwLocalization.SelectedRows.Count == 1)
                {
                    _dgwLocalization.Rows[0].Cells[0].Value = "Name1";
                    _dgwLocalization.Rows[0].Cells[1].Value = "<value>";
                }
            }
            else
            {
                Dialog.Info("Please select tranlation item");
            }
        }

        private void _btSynchAllSym_Click(object sender, EventArgs e)
        {
            if (!_passwd.Equals(string.Empty))
            {

                if (null == _localizationHelper.MasterLocalization)
                {
                    Dialog.Error("There is no master localization present");
                    return;
                }
                try
                {
                    _localizationHelper.SynchronizeSymbolsFromMaster();
                    ReloadLocalizationData();
                    Dialog.Info("Symbol synchronization proceed successfuly");
                    _wasChangeMade = true;
                }
                catch (Exception aError)
                {
                    Dialog.Error(aError.Message);
                }
            }
        }

        private void _btCEnumerationFile_Click(object sender, EventArgs e)
        {
            if (null == _devActualLocalization)
            {
                Dialog.Warning("No localization is selected");
                _cbLocalizations.Focus();
                return;
            }
            _sdSaveDialog.DefaultExt = "cs";
            _sdSaveDialog.FileName = string.Empty;
            _sdSaveDialog.Filter = "C# class/source (*.cs)|*.cs";
            if (_sdSaveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _devActualLocalization.GenCSharpEnum(_sdSaveDialog.FileName);
                }
                catch (Exception aError)
                {
                    Dialog.Error(aError.Message);
                }
            }
        }

        private void _btLoadLocData_Click(object sender, EventArgs e)
        {
            if (_passwd.Equals(string.Empty))
            {
                return;
            }
            _ofdOpen.Filter = "Resource file (*.resx)|*.resx";
            if (DialogResult.OK == _ofdOpen.ShowDialog())
            {
                try
                {
                    _locFilePath = Path.GetDirectoryName(_ofdOpen.FileName);
                    string[] fileNameItems = _ofdOpen.SafeFileName.Split('.');
                    if (fileNameItems[0].Contains("Localization"))
                    {
                        Localization localization = _localizationHelper.LoadResxLocalization(_ofdOpen.FileName, fileNameItems[1], false);
                        RefreshLocalizationsComboBox();
                        SelectLocalization(localization.Language);
                    }
                    else
                    {
                        Dialog.Error("Wrong localization file");
                    }
                }
                catch (Exception aError)
                {
                    Dialog.Error(aError.Message);
                }
            }
        }

        private void _btLOadPackage_Click(object sender, EventArgs e)
        {
            _ofdOpen.DefaultExt = "lix";
            _ofdOpen.Filter = "XML localization info (*.lix)|*.lix";
            if (DialogResult.OK == _ofdOpen.ShowDialog())
            {
                try
                {
                    _localizationHelper.Clear();
                    _localizationHelper.LoadResxPackage(_ofdOpen.FileName);
                    RefreshLocalizationsComboBox();
                    if (null != _localizationHelper.MasterLocalization)
                    {
                        SelectLocalization(_localizationHelper.MasterLocalization.Language);
                        ReloadMasterLocalizationData(_localizationHelper.MasterLocalization, false);
                    }
                    if (!_passwd.Equals(string.Empty))
                    {
                        _btClosePackage.Enabled = true;
                    }
                    _isPackageLoaded = true;
                    _wasChangeMade = true;
                    _packageFileNamePath = _ofdOpen.FileName;
                }
                catch (Exception aError)
                {
                    Dialog.Error(aError.Message);
                }
            }
        }

        private void _btSavePackage_Click(object sender, EventArgs e)
        {
            if (0 == _localizationHelper.Count)
            {
                Dialog.Error("Current package does not contain any localizations");
                return;
            }
            if (null == _localizationHelper.MasterLocalization)
            {
                List<string> languages = new List<string>();
                foreach (Localization localization in _localizationHelper)
                {
                    languages.Add(localization.Language);
                }
                ChooseLanguage lanDialog = new ChooseLanguage(languages);
                DialogResult result = lanDialog.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                
                if (result == DialogResult.Abort)
                {
                    if (!Dialog.WarningQuestion("The current package does not contain any master localizations.\nDo you want to continue ?"))
                    {
                        return;
                    }
                }
                else if (result == DialogResult.OK)
                {
                    foreach (Localization localization in _localizationHelper)
                    {
                        if (localization.Language.Contains(lanDialog.Language))
                        {
                            localization.IsMasterResource = true;
                            _localizationHelper.MasterLocalization = localization;
                            break;
                        }
                    }
                }
            }
            _sdSaveDialog.DefaultExt = "lix";
            _sdSaveDialog.FileName = "localizations.lix";
            _sdSaveDialog.Filter = "XML localization info (*.lix)|*.lix";
            DialogResult dialogResult = DialogResult.None;
            if (_packageFileNamePath.Equals(string.Empty))
            {
                dialogResult = _sdSaveDialog.ShowDialog();
                _packageFileNamePath = _sdSaveDialog.FileName;
            }
            else if (dialogResult == DialogResult.None)
            {
                _sdSaveDialog.FileName = _packageFileNamePath;
            }
            if (dialogResult != DialogResult.Cancel)
            {
                try
                {
                    _localizationHelper.SaveResxPackage(_sdSaveDialog.FileName);
                    RefreshLocalizationsComboBox();
                    if (null != _localizationHelper.MasterLocalization)
                    {
                        SelectLocalization(_localizationHelper.MasterLocalization.Language);
                        ReloadMasterLocalizationData(_localizationHelper.MasterLocalization, false);
                    }
                    _wasChangeMade = true;
                    if (!_passwd.Equals(string.Empty))
                    {
                        _btClosePackage.Enabled = true;
                    }
                }
                catch (Exception aError)
                {
                    Dialog.Error(aError.Message);
                }
            }
        }

// ReSharper disable once UnusedParameter.Local
        private void ReloadMasterLocalizationData(Localization masterLocalization, bool isScrollDown)
        {
            _lbMasterLocalization.Items.Clear();
            if (null == masterLocalization)
                return;

            foreach (KeyValuePair<string, TranslationItem> aPair in masterLocalization.Symbols)
            {
                _lbMasterLocalization.Items.Add(aPair.Value);
            }
        }

        private void _edTranslationItem_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    {
                        if (_lbMasterLocalization.SelectedIndex > 0)
                        {
                            _lbMasterLocalization.SelectedIndex--;
                        }
                        break;
                    }
                case Keys.Down:
                    {
                        if (_lbMasterLocalization.SelectedIndex < _lbMasterLocalization.Items.Count - 1)
                        {
                            _lbMasterLocalization.SelectedIndex++;
                        }
                        break;
                    }
            }
        }

        private void _edTranslationItem_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (!_translationOk)
                        return;
                    if (_lbOtherLocalization.SelectedIndex >= 0)
                    {
                        TranslationItem item = (TranslationItem)_lbOtherLocalization.SelectedItem;
                        item.Value = _edTranslationItem.Text;
                        _transActualLocalization.Data[item.Name] = item;
                        _lbOtherLocalization.Items[_lbOtherLocalization.SelectedIndex] = item;
                        _lbMasterLocalization.Focus();
                        _wasChangeMade = true;
                    }
                    break;
            }
        }

        private void _edTranslationItem_TextChanged(object sender, EventArgs e)
        {
            if (_lbMasterLocalization.Items.Count == 0 ||
                _lbOtherLocalization.Items.Count == 0)
            {
                SetTranslationReport(NotificationSeverity.Success, "");
                _translationOk = false;
                return;
            }
            if (Validator.IsNullString(_edTranslationItem.Text))
            {
                SetTranslationReport(NotificationSeverity.Warning, "No translation specified yet");
                _translationOk = false;
                return;
            }
            string masterText = _labMasterItem.Text;
            string transText = _edTranslationItem.Text;
            string report = String.Empty;
            _translationOk = true;
            foreach (string pattern in new string[] { "<br/>", "<br />", "\"", "\n", "\t" })
            {
                int masterCount = QuickParser.CountIndexOf(masterText, pattern);
                if (masterCount == 0)
                {
                    continue;
                }
                if (masterCount != QuickParser.CountIndexOf(transText, pattern))
                {
                    report += "- count of " + pattern + " sequences must be the same in master and translated text\n";
                    _translationOk = false;
                }
            }
            foreach (string pattern in new string[] { "{0}", "{1}", "{2}", "{3}", "{4}", "{5}", "{6}", "{7}", "{8}", "{9}" })
            {
                if (QuickParser.CountIndexOf(masterText, pattern) != QuickParser.CountIndexOf(transText, pattern))
                {
                    report += "- count of " + pattern + " sequences must be precisely equal in master and translated text !!!\n";
                    _translationOk = false;
                }
            }
            SetTranslationReport(_translationOk ? NotificationSeverity.Success : NotificationSeverity.Error, _translationOk ? "Confirm translation by pressing enter" : report);
        }

        private void _lbMasterLocalization_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lbMasterLocalization.SelectedIndex == _lbOtherLocalization.SelectedIndex)
            {
                return;
            }
            if (_lbMasterLocalization.Items.Count == _lbOtherLocalization.Items.Count)
            {
                _lbOtherLocalization.SelectedIndex = _lbMasterLocalization.SelectedIndex;
                _lbOtherLocalization.TopIndex = _lbMasterLocalization.TopIndex;
            }
            else
            {
                return;
            }
            TranslationItem item = (TranslationItem)_lbMasterLocalization.SelectedItem;
            _labMasterItem.Text = item.Value;
            _labDescriptionItem.Text = item.Comment;
            TranslationItem aOtherItem = (TranslationItem)_lbOtherLocalization.SelectedItem;
            _edTranslationItem.Text = " ";
            _edTranslationItem.Text = aOtherItem.Value;
            _edTranslationItem.Focus();
            _edTranslationItem.Select(0, 0);
        }

        private void _lbOtherLocalization_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lbMasterLocalization.SelectedIndex == _lbOtherLocalization.SelectedIndex)
            {
                return;
            }
            if (_lbMasterLocalization.SelectedIndex == -1 || _lbOtherLocalization.SelectedIndex == -1)
            {
                return;
            }
            if (_lbMasterLocalization.Items.Count == _lbOtherLocalization.Items.Count)
            {
                _lbMasterLocalization.SelectedIndex = _lbOtherLocalization.SelectedIndex;
                _lbMasterLocalization.TopIndex = _lbOtherLocalization.TopIndex;
            }
            else
            {
                return;
            }
            TranslationItem item = (TranslationItem)_lbMasterLocalization.SelectedItem;
            _labMasterItem.Text = item.Value;
            _labDescriptionItem.Text = item.Comment;
            TranslationItem aOtherItem = (TranslationItem)_lbOtherLocalization.SelectedItem;
            _edTranslationItem.Text = " ";
            _edTranslationItem.Text = aOtherItem.Value;
            _edTranslationItem.Select(0, 0);
        }

        private void _cbNonMasterLangs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cbNonMasterLangs.SelectedIndex >= 0)
            {
                _transActualLocalization = (Localization)_cbNonMasterLangs.SelectedItem;
            }
            else
            {
                return;
            }
            ReloadTranslationData();
        }

        private void SetTranslationReport(NotificationSeverity i_aStyle, string strReport)
        {
            _labTranslationReport.Text = strReport;
            switch (i_aStyle)
            {
                case NotificationSeverity.Success:
                    {
                        _labTranslationReport.BackColor = SystemColors.Control;
                        _labTranslationReport.ForeColor = Color.Black;
                        break;
                    }
                case NotificationSeverity.Warning:
                    {
                        _labTranslationReport.BackColor = Color.Yellow;
                        _labTranslationReport.ForeColor = Color.Black;
                        break;
                    }
                case NotificationSeverity.Error:
                    {
                        _labTranslationReport.BackColor = Color.Red;
                        _labTranslationReport.ForeColor = Color.White;
                        break;
                    }
            }
        }

        private void ReloadTranslationData()
        {
            if (null == _transActualLocalization)
            {
                return;
            }
            _lbOtherLocalization.Items.Clear();
            foreach (TranslationItem item in _lbMasterLocalization.Items)
            {
                TranslationItem otherItem;
                if (!_transActualLocalization.Symbols.TryGetValue(item.Name, out otherItem))
                {
                    otherItem = new TranslationItem {Name = item.Name, Value = string.Empty, Comment = string.Empty};
                    _transActualLocalization.Symbols.Add(otherItem.Name, otherItem);
                }
                _lbOtherLocalization.Items.Add(otherItem);
            }
        }

        private void SaveLocalization(string filePath)
        {
            Dictionary<string, TranslationItem> resourceItems = new Dictionary<string, TranslationItem>();
            foreach (KeyValuePair<string, TranslationItem> pair in _devActualLocalization.Data)
            {
                TranslationItem transItem = new TranslationItem
                {
                    Name = pair.Value.Name,
                    Value = pair.Value.Value,
                    Comment = pair.Value.Comment
                };
                resourceItems.Add(transItem.Name, transItem);
            }
            _localizationHelper.SaveResxLocalization(resourceItems, filePath, _devActualLocalization.Language);
        }

        private void _btClosePackage_Click(object sender, EventArgs e)
        {
            _localizationHelper.Clear();
            _devActualLocalization = null;
            _dgwLocalization.Rows.Clear();
            RefreshLocalizationsComboBox();
            _cbLocalizations.Items.Clear();
            _btDeleteTransItem.Enabled = false;
            _btInsertNewRow.Enabled = false;
            _btClosePackage.Enabled = false;
            _isPackageLoaded = false;
            _labMasterLang.Text = "Master language name";
            _cbNonMasterLangs.Items.Clear();
            _lbMasterLocalization.Items.Clear();
            _lbOtherLocalization.Items.Clear();
            _labDescriptionItem.Text = string.Empty;
            _labMasterItem.Text = string.Empty;
            _edTranslationItem.Clear();
            _labTranslationReport.Text = string.Empty;
            _labTranslationReport.BackColor = SystemColors.Control;
            _wasChangeMade = true;
            _packageFileNamePath = string.Empty;
        }

        private void _tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_tcMain.SelectedIndex == 1 && _isPackageLoaded)
            {
                _btClosePackage.Enabled = true;
            }
            if (_tcMain.SelectedIndex == 0)
            {
                _rbTransMode.Checked = true;
            }
            else
            {
                _rbDevMod.Checked = true;
            }
        }

        private void _btReloadPackage_Click(object sender, EventArgs e)
        {
            RefreshLocalizationsComboBox();
            if (null != _localizationHelper.MasterLocalization)
            {
                SelectLocalization(_localizationHelper.MasterLocalization.Language);
                ReloadMasterLocalizationData(_localizationHelper.MasterLocalization, false);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            

            if (_wasChangeMade)
            {
                if (Dialog.Question("Do you want to save changes?"))
                {
                    _btSavePackage.PerformClick();
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
                e.Cancel = false;
        }

        private void _btDefGenPaths_Click(object sender, EventArgs e)
        {
            Options optDialog = new Options(_localizationHelper);
            optDialog.ShowDialog();
            CheckGenerateAll();
        }

        private void CheckGenerateAll()
        {
            if (null != _localizationHelper.CSharpPaths && _localizationHelper.CSharpPaths.Count > 0)
            {
                _btGenAll.Enabled = true;
            }
            else
            {
                _btGenAll.Enabled = false;
            }
        }

        private void _btGenAll_Click(object sender, EventArgs e)
        {
            if (null == _localizationHelper.MasterLocalization)
            {
                Dialog.Warning("No master localization is present");
                return;
            }
            if (null != _localizationHelper.CSharpPaths)
            {
                foreach (string path in _localizationHelper.CSharpPaths)
                {
                    try
                    {
                        _localizationHelper.MasterLocalization.GenCSharpEnum(path + "\\localization.symbols.cs");
                    }
                    catch (Exception aError)
                    {
                        Dialog.Error(aError.Message);
                    }
                }
            }
            Dialog.Info("Files successfuly generated");
        }

        private void _btInsertNewRow_Click(object sender, EventArgs e)
        {
            _dgwLocalization.CurrentCell = _dgwLocalization[0, _dgwLocalization.Rows.Count - 1];
            _dgwLocalization.BeginEdit(false);
        }

        private void _btSaveLocalizationDataAs_Click(object sender, EventArgs e)
        {
            if (_devActualLocalization != null)
            {
                if (_devActualLocalization.Data.Count > 0)
                {
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        _locFilePath = dialog.SelectedPath;
                        SaveLocalization(dialog.SelectedPath);
                    }
                }
                else
                {
                    Dialog.Warning("Current localization is empty");
                }
            }
            else
            {
                Dialog.Warning("No localization is selected");
            }
        }

        private void _btSavePackageAs_Click(object sender, EventArgs e)
        {
            if (0 == _localizationHelper.Count)
            {
                Dialog.Error("Current package does not contain any localizations");
                return;
            }
            if (null == _localizationHelper.MasterLocalization)
            {
                List<string> languages = new List<string>();
                foreach (Localization localization in _localizationHelper)
                {
                    languages.Add(localization.Language);
                }
                ChooseLanguage lanDialog = new ChooseLanguage(languages);
                DialogResult result = lanDialog.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                if (result == DialogResult.Abort)
                {
                    if (!Dialog.WarningQuestion("The current package does not contain any master localizations.\nDo you want to continue ?"))
                    {
                        return;
                    }
                }
                else if (result == DialogResult.OK)
                {
                    foreach (Localization localization in _localizationHelper)
                    {
                        if (localization.Language.Contains(lanDialog.Language))
                        {
                            localization.IsMasterResource = true;
                            _localizationHelper.MasterLocalization = localization;
                            break;
                        }
                    }
                }
            }
            _sdSaveDialog.DefaultExt = "lix";
            _sdSaveDialog.FileName = "localizations.lix";
            _sdSaveDialog.Filter = "XML localization info (*.lix)|*.lix";
            if (DialogResult.OK == _sdSaveDialog.ShowDialog())
            {
                try
                {
                    _packageFileNamePath = _sdSaveDialog.FileName;
                    _localizationHelper.SaveResxPackage(_sdSaveDialog.FileName);
                    RefreshLocalizationsComboBox();
                    if (null != _localizationHelper.MasterLocalization)
                    {
                        SelectLocalization(_localizationHelper.MasterLocalization.Language);
                        ReloadMasterLocalizationData(_localizationHelper.MasterLocalization, false);
                    }
                    _wasChangeMade = true;
                }
                catch (Exception aError)
                {
                    Dialog.Error(aError.Message);
                }
            }
        }
    }
}