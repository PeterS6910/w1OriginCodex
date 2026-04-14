//#define DESIGNER
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Components;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;
using JetBrains.Annotations;
using SqlUniqueException = Contal.IwQuick.SqlUniqueException;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASAntiPassBackZoneEditForm :
#if DESIGNER
        Form
#else
 ACgpPluginEditForm<NCASClient, AntiPassBackZone>
#endif
    {
        public NCASAntiPassBackZoneEditForm(
            AntiPassBackZone antiPassBackZone,
            ShowOptionsEditForm showOptionsEditForm,
            PluginMainForm<NCASClient> tableForm)
            : base(
                antiPassBackZone,
                showOptionsEditForm,
                CgpClientMainForm.Singleton,
                tableForm,
                NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            WheelTabContorol = _tcAntiPassbackZone;
            _tbDescription.MouseWheel += ControlMouseWheel;

            _cdgvEntryCardReaders.DataGrid.RowsAdded += _cdgvEntryCardReaders_RowsAdded;
            
            _cdgvEntryCardReaders.DataGrid.CurrentCellDirtyStateChanged +=
                _cdgvEntryCardReaders_CurrentCellDirtyStateChanged;

            _cdgvEntryCardReaders.DataGrid.CellClick += _cdgvEntryCardReaders_CellClick;

            _cdgvEntryCardReaders.DataGrid.CellDoubleClick +=
                _cdgvEntryCardReaders_CellDoubleClick;

            _cdgvEntryCardReaders.DataGrid.DragDrop +=
                _cdgvEntryCardReaders_DragDrop;

            _cdgvEntryCardReaders.DataGrid.DragOver +=
                _cdgvEntryCardReaders_DragOver;

            _cdgvExitCardReaders.DataGrid.RowsAdded += _cdgvExitCardReaders_RowsAdded;
            
            _cdgvExitCardReaders.DataGrid.CurrentCellDirtyStateChanged +=
                _cdgvExitCardReaders_CurrentCellDirtyStateChanged;

            _cdgvExitCardReaders.DataGrid.CellClick += _cdgvExitCardReaders_CellClick;

            _cdgvExitCardReaders.DataGrid.CellDoubleClick +=
                _cdgvExitCardReaders_CellDoubleClick;

            _cdgvExitCardReaders.DataGrid.DragDrop +=
                _cdgvExitCardReaders_DragDrop;

            _cdgvExitCardReaders.DataGrid.DragOver +=
                _cdgvExitCardReaders_DragOver;

            _cdgvCards.DataGrid.CurrentCellDirtyStateChanged +=
                _cdgvCards_CurrentCellDirtyStateChanged;

            _cdgvCards.DataGrid.CellValueChanged +=
                _cdgvCards_CellValueChanged;

            _cdgvCards.DataGrid.CellPainting +=
                _cdgvCards_CellPainting;

            _cdgvCards.DataGrid.CellDoubleClick +=
                _cdgvCards_CellDoubleClick;

            _cdgvCards.DataGrid.DragOver += _cdgvCards_DragOver;
            _cdgvCards.DataGrid.DragDrop += _cdgvCards_DragDrop;

            SetReferenceEditColors();
            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private static void _cdgvCards_CellPainting(
            object sender, 
            DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex != 1 && e.ColumnIndex != 2)
                return;

            Image img =
                ObjectImageList.Singleton
                    .GetImageForObjectType(
                        e.ColumnIndex == 1
                            ? ObjectType.Person
                            : ObjectType.Card);

            bool isSelected =
                (e.State & DataGridViewElementStates.Selected) ==
                    DataGridViewElementStates.Selected;

            e.PaintBackground(
                e.ClipBounds,
                isSelected);

            PointF p = e.CellBounds.Location;

            p.X += img.Width;
            p.Y += 5;

            e.Graphics.DrawImage(
                img,
                e.CellBounds.X + 2,
                e.CellBounds.Y + 5,
                16,
                16);

            e.Graphics.DrawString(
                e.Value.ToString(),
                e.CellStyle.Font,
                isSelected
                    ? new SolidBrush(e.CellStyle.SelectionForeColor)
                    : new SolidBrush(e.CellStyle.ForeColor),
                p);

            e.Handled = true;
        }

        private void _cdgvCards_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            var dataGridViewRowCollection = _cdgvCards.DataGrid.Rows;

            int rowIndex = e.RowIndex;

            if (rowIndex < 0 ||
                rowIndex >= dataGridViewRowCollection.Count)
                return;

            var dataGridViewColumnCollection = _cdgvCards.DataGrid.Columns;

            int columnIndex = e.ColumnIndex;

            if (columnIndex < 0 ||
                columnIndex > dataGridViewColumnCollection.Count)
                return;

            var dataBoundItem =
                dataGridViewRowCollection[rowIndex].DataBoundItem
                    as CardInZoneView;

            if (dataBoundItem == null)
                return;

            Card card =
                CgpClient.Singleton.MainServerProvider.Cards.GetObjectById(
                    dataBoundItem.Id);

            if (card == null)
                return;

            if (dataGridViewColumnCollection[columnIndex].DataPropertyName ==
                CardInZoneView.COLUMN_NAME)
            {
                var person = card.Person;

                if (person != null)
                {
                    PersonsForm.Singleton.OpenEditForm(person);
                    return;
                }
            }

            CardsForm.Singleton.OpenEditForm(card);
        }

        private static void OnCardReadersDoubleClick(
            CgpDataGridView cdgvCardReaders,
            int rowIndex)
        {
            var dataGridViewRowCollection = cdgvCardReaders.DataGrid.Rows;

            if (rowIndex < 0 ||
                rowIndex >= dataGridViewRowCollection.Count)
                return;

            var dataBoundItem =
                dataGridViewRowCollection[rowIndex].DataBoundItem
                    as CardReaderView;

            if (dataBoundItem == null)
                return;

            NCASCardReadersForm.Singleton
                .OpenEditForm(dataBoundItem.CardReader);
        }

        private void _cdgvEntryCardReaders_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            OnCardReadersDoubleClick(
                _cdgvEntryCardReaders,
                e.RowIndex);
        }

        private void _cdgvExitCardReaders_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            OnCardReadersDoubleClick(
                _cdgvExitCardReaders,
                e.RowIndex);
        }

        private void _cdgvCards_CellValueChanged(
            object sender,
            DataGridViewCellEventArgs e)
        {
            var dataGridView = _cdgvCards.DataGrid;

            if (dataGridView.Columns[e.ColumnIndex].DataPropertyName !=
                CardInZoneView.COLUMN_ISCHECKED)
            {
                return;
            }

            if (_cardsInZone.All(cardInZone => cardInZone.IsChecked))
            {
                _cbSelectUnselectAll.CheckState = CheckState.Checked;
                return;
            }

            if (_cardsInZone.All(cardInZone => !cardInZone.IsChecked))
            {
                _cbSelectUnselectAll.CheckState = CheckState.Unchecked;
                return;
            }

            _cbSelectUnselectAll.CheckState = CheckState.Indeterminate;
        }

        private void _cdgvCards_CurrentCellDirtyStateChanged(
            object sender,
            EventArgs e)
        {
            var dataGridView = _cdgvCards.DataGrid;

            if (dataGridView.CurrentCell.OwningColumn.DataPropertyName ==
                CardInZoneView.COLUMN_ISCHECKED ||
                dataGridView.IsCurrentCellDirty)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        protected override void RegisterEvents()
        {
        }

        protected override void BeforeInsert()
        {
            NCASAntiPassBackZonesForm.Singleton
                .BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASAntiPassBackZonesForm.Singleton.BeforeEdit(
                this,
                _editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;

            var obj =
                Plugin.MainServerProvider.AntiPassBackZones
                    .GetObjectForEdit(
                        _editingObject.IdAntiPassBackZone,
                        out error);

            if (error != null)
                if (error is AccessDeniedException)
                {
                    allowEdit = false;

                    obj =
                        Plugin.MainServerProvider.AntiPassBackZones
                            .GetObjectById(_editingObject.IdAntiPassBackZone);

                    DisableForm();
                }
                else
                    throw error;
            else
                allowEdit = true;

            _cdgvEntryCardReaders.DataGrid.DataSource = null;
            _cdgvExitCardReaders.DataGrid.DataSource = null;

            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;

            Plugin.MainServerProvider.AntiPassBackZones.RenewObjectForEdit(
                _editingObject.IdAntiPassBackZone,
                out error);

            if (error != null)
                throw error;

            _cdgvEntryCardReaders.DataGrid.DataSource = null;
            _cdgvExitCardReaders.DataGrid.DataSource = null;
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(
                new ReferencedByForm(
                    GetListReferencedObjects,
                    NCASClient.LocalizationHelper,
                    ObjectImageList.Singleton.GetAllObjectImages()));
        }

        private IList<AOrmObject> GetListReferencedObjects()
        {
            return
                Plugin.MainServerProvider.AntiPassBackZones
                    .GetReferencedObjects(
                        _editingObject.IdAntiPassBackZone,
                        CgpClient.Singleton.GetListLoadedPlugins());
        }

        protected override void SetValuesInsert()
        {
            _eName.Text = "";
            _tbDescription.Text = "";
            _nudTimeout.Value = _nudTimeout.MinDate;

            _cbProhibitAccessForCardNotPresent.Checked = false;
        }

        protected override void SetValuesEdit()
        {
            SetReferencedBy();

            _eName.Text = _editingObject.Name;

            _tbDescription.Text = _editingObject.Description;
            try
            {
                _nudTimeout.Value =
                    _nudTimeout.MinDate +
                    TimeSpan.FromMinutes(_editingObject.Timeout);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _cbProhibitAccessForCardNotPresent.Checked =
                _editingObject.ProhibitAccessForCardNotPresent;

            SetExpirationTarget(_editingObject.DestinationAPBZAfterTimeout);

            _bApply.Enabled = false;
        }

        private void SetExpirationTarget(AntiPassBackZone destinationApbzAfterTimeout)
        {
            _currentExpirationTarget = destinationApbzAfterTimeout;

            if (TimeoutValueInMinutes == 0)
            {
                _tbmExpirationTarget.Text = string.Empty;
                _tbmExpirationTarget.TextImage = null;

                _tbmExpirationTarget.Enabled = false;

                return;
            }

            _tbmExpirationTarget.Enabled = true;

            if (destinationApbzAfterTimeout == null)
            {
                _tbmExpirationTarget.Text = string.Empty;
                _tbmExpirationTarget.TextImage = null;

                return;
            }

            _tbmExpirationTarget.Text =
                destinationApbzAfterTimeout.ToString();

            _tbmExpirationTarget.TextImage =
                Plugin.GetImageForAOrmObject(destinationApbzAfterTimeout);
        }

        protected override bool CheckValues()
        {
            if (!string.IsNullOrEmpty(_eName.Text))
                return true;

            ControlNotification.Singleton.Error(
                NotificationPriority.JustOne,
                _eName,
                CgpClient.Singleton.LocalizationHelper.GetString("ErrorEntryName"),
                CgpClient.Singleton.ClientControlNotificationSettings);

            _eName.Focus();
            return false;
        }

        protected override bool GetValues()
        {
            try
            {
                _editingObject.Name = _eName.Text;

                _editingObject.Description = _tbDescription.Text;
                _editingObject.Timeout = TimeoutValueInMinutes;

                _editingObject.DestinationAPBZAfterTimeout = 
                    _editingObject.Timeout != 0
                        ? _currentExpirationTarget
                        : null;

                _editingObject.ProhibitAccessForCardNotPresent =
                    _cbProhibitAccessForCardNotPresent.Checked;
            }
            catch (Exception)
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorGetValuesFailed"));
                return false;
            }

            return true;
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;

            bool retValue =
                Plugin.MainServerProvider.AntiPassBackZones
                    .Insert(
                        ref _editingObject,
                        out error);

            return 
                retValue || error == null
                    ? retValue
                    : OnSaveToDatabaseError(error);
        }

        private bool SaveToDatabaseEditCore(bool onlyInDatabase)
        {
            Exception error;

            bool retValue =
                onlyInDatabase
                    ? Plugin.MainServerProvider.AntiPassBackZones.UpdateOnlyInDatabase(_editingObject, out error)
                    : Plugin.MainServerProvider.AntiPassBackZones.Update(_editingObject, out error);

            return 
                retValue || error == null
                    ? retValue
                    : OnSaveToDatabaseError(error);
        }

        private bool OnSaveToDatabaseError(Exception error)
        {
            if (error is SqlUniqueException)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _eName,
                    GetString("ErrorUsedAntiPassBackZoneName"),
                    CgpClient.Singleton.ClientControlNotificationSettings);

                _eName.Focus();

                return false;
            }

            if (error is CycleInExirationTargetsException)
            {
                //The current setting of after-timeout destination anti-passback zone would cause an infinite cycling of timed-out card.
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmExpirationTarget,
                    GetString("ErrorCycleInExirationTargets"),
                    CgpClient.Singleton.ClientControlNotificationSettings);

                return false;
            }

            if (error is ConflictingEntryCardReadersException)
            {
                var conflictDialogModel =
                    new APBZConflictDialogModel(
                        Plugin.MainServerProvider.AntiPassBackZones,
                        _editingObject);

                if (conflictDialogModel.ConflictExists)
                {
                    var dlg = new APBZConflictDialog(conflictDialogModel);

                    if (dlg.ShowDialog() == DialogResult.OK)
                        if (conflictDialogModel.PerformAction())
                            ShowEntryCardReaders();

                    return false;
                }
            }

            if (error is TimeoutNotSetAndNoExitCardReaderException)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _nudTimeout,
                    GetString("ErrorTimeoutNotSetAndNoExitCardReader"),
                    CgpClient.Singleton.ClientControlNotificationSettings);

                return false;
            }

            throw error;
        }

        protected override bool SaveToDatabaseEdit()
        {
            return SaveToDatabaseEditCore(false);
        }

        protected override bool SaveToDatabaseEditOnlyInDatabase()
        {
            return SaveToDatabaseEditCore(true);
        }

        protected override void UnregisterEvents()
        {
        }

        protected override void EditEnd()
        {
            Plugin.MainServerProvider.AntiPassBackZones.EditEnd(_editingObject);
        }

        protected override void AfterInsert()
        {
            NCASAntiPassBackZonesForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASAntiPassBackZonesForm.Singleton.AfterEdit(_editingObject);
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                ShowEntryCardReaders();
                ShowExitCardReaders();

                _bApply.Enabled = false;
            }
        }

        protected override void EditTextChanger(
            object sender,
            EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private ApbzCardReaderEntryExitBy GetEntryExitBy(
            CardReader cardReader,
            bool isEntryCardReader)
        {
            APBZCardReader apbzCardReader;

            return isEntryCardReader
                ? (_editingObject.EntryCardReaders.TryGetValue(
                    cardReader,
                    out apbzCardReader)
                    ? apbzCardReader.EntryExitBy
                    : ApbzCardReaderEntryExitBy.AccessPermitted)
                : (_editingObject.ExitCardReaders.TryGetValue(
                    cardReader,
                    out apbzCardReader)
                    ? apbzCardReader.EntryExitBy
                    : ApbzCardReaderEntryExitBy.AccessPermitted);
        }

        private void SetEntryExitBy(
            CardReader cardReader,
            bool isEntryCardReader,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            APBZCardReader apbzCardReader;

            if (!isEntryCardReader)
            {
                if (!_editingObject.ExitCardReaders.TryGetValue(
                    cardReader,
                    out apbzCardReader))
                {
                    return;
                }

                if (apbzCardReader.EntryExitBy != entryExitBy)
                {
                    apbzCardReader.EntryExitBy = entryExitBy;
                    EditTextChanger(null, null);
                }

                return;
            }

            if (!_editingObject.EntryCardReaders.TryGetValue(
                cardReader,
                out apbzCardReader))
            {
                return;
            }

            if (apbzCardReader.EntryExitBy == entryExitBy)
                return;

            apbzCardReader.EntryExitBy = entryExitBy;

            var conflictDialogModel =
                new APBZConflictDialogModel(
                    Plugin.MainServerProvider.AntiPassBackZones,
                    _editingObject);

            if (conflictDialogModel.ConflictExists)
            {
                BeginInvoke(
                    new Action<APBZConflictDialogModel>(ShowConflictDialog),
                    conflictDialogModel);
            }

            EditTextChanger(null, null);
        }

        private void ShowConflictDialog(APBZConflictDialogModel conflictDialogModel)
        {
            var dlg = new APBZConflictDialog(conflictDialogModel);

            if (dlg.ShowDialog() == DialogResult.OK)
                if (conflictDialogModel.PerformAction())
                    ShowEntryCardReaders();
        }

        private const string COLUMN_APBZ_CR_ENTRY_BY_COMBO_BOX = "ApbzCrEntryBy";

        private void ShowEntryCardReaders()
        {
            var dataGridView = _cdgvEntryCardReaders.DataGrid;

            if (_editingObject.EntryCardReaders == null)
            {
                _bsEntryCardReaders = null;
                dataGridView.DataSource = null;

                return;
            }

            _bsEntryCardReaders =
                new BindingSource
                {
                    DataSource =
                        _editingObject.EntryCardReaders
                            .Select(
                                kvPair =>
                                    new CardReaderView(
                                        this,
                                        kvPair.Key,
                                        true))
                            .ToArray()
                };

            dataGridView.DataSource = _bsEntryCardReaders;

            HideColumnDgw(dataGridView, CardReaderView.COLUMN_ENTRY_EXIT_BY);

            var nameDataGridViewColumn =
                dataGridView.Columns[CardReaderView.COLUMN_NAME];

            nameDataGridViewColumn.DisplayIndex = 0;
            nameDataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (!dataGridView.Columns.Contains(COLUMN_APBZ_CR_ENTRY_BY_COMBO_BOX))
            {
                dataGridView.Columns.Add(
                    new DataGridViewComboBoxColumn
                    {
                        Name = COLUMN_APBZ_CR_ENTRY_BY_COMBO_BOX,
                        AutoComplete = true,
                        FlatStyle = FlatStyle.System,
                        DataPropertyName = CardReaderView.COLUMN_ENTRY_EXIT_BY,
                        DisplayMember = ApbzCardReaderEntryExitByView.COLUMN_NAME,
                        ValueMember = ApbzCardReaderEntryExitByView.COLUMN_APBZ_CARD_READER_ENTRY_EXIT_BY,
                        ValueType = typeof(ApbzCardReaderEntryExitBy),
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                        DisplayIndex = 1,
                        MinimumWidth = 270
                    });

                _cdgvEntryCardReaders.DisabledDoubleClickEventForColumns.Add(COLUMN_APBZ_CR_ENTRY_BY_COMBO_BOX);

                foreach (DataGridViewRow row in dataGridView.Rows)
                    _cdgvEntryExitCardReaders_RowsAdded(
                        _cdgvEntryCardReaders.DataGrid,
                        row.Index,
                        COLUMN_APBZ_CR_ENTRY_BY_COMBO_BOX);
            }

            LocalizationHelper.TranslateDataGridViewColumnsHeaders(dataGridView);
        }

        void _cdgvEntryCardReaders_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            _cdgvEntryExitCardReaders_RowsAdded(
                _cdgvEntryCardReaders.DataGrid,
                e.RowIndex,
                COLUMN_APBZ_CR_ENTRY_BY_COMBO_BOX);
        }

        private void _cdgvEntryExitCardReaders_RowsAdded(
            DataGridView dataGrid,
            int rowIndex,
            string columnApbzCrEntryExitByComboBox)
        {
            if (!dataGrid.Columns.Contains(columnApbzCrEntryExitByComboBox))
                return;

            var row = dataGrid.Rows[rowIndex];

            var cardReaderView = row.DataBoundItem as CardReaderView;

            if (cardReaderView == null)
                return;

            var cellApbzCrEntryByComboBox = (DataGridViewComboBoxCell)row.Cells[columnApbzCrEntryExitByComboBox];

            if (cellApbzCrEntryByComboBox.DataSource != null)
                return;

            cellApbzCrEntryByComboBox.DataSource =
                new List<ApbzCardReaderEntryExitByView>(
                    GetEntryExitByItems(
                        Plugin.MainServerProvider.CardReaders.IsFromMinimalDe(
                            cardReaderView.CardReader.IdCardReader)));
        }

        void _cdgvEntryCardReaders_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _cdgvEntryExitCardReaders_EndEdit(_cdgvEntryCardReaders.DataGrid);
        }

        void _cdgvEntryExitCardReaders_EndEdit(DataGridView dataGrid)
        {
            if (dataGrid.IsCurrentCellDirty)
            {
                dataGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dataGrid.EndEdit();
            }
        }

        void _cdgvEntryCardReaders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0
                && _cdgvEntryCardReaders.DataGrid.Columns[e.ColumnIndex].Name == COLUMN_APBZ_CR_ENTRY_BY_COMBO_BOX)
            {
                _cdgvEntryExitCardReaders_BeginEdit(_cdgvEntryCardReaders.DataGrid);
            }
        }

        void _cdgvEntryExitCardReaders_BeginEdit(DataGridView dataGrid)
        {
            dataGrid.BeginEdit(true);
            ((ComboBox) dataGrid.EditingControl).DroppedDown = true;
        }

        public bool AddEntryCardReaders(
            ICollection<CardReader> entryCardReadersToAdd,
            ApbzCardReaderEntryExitBy entryBy)
        {
            if (entryCardReadersToAdd.Count == 0)
                return false;

            var exitCardReaders = _editingObject.ExitCardReaders;

            if (exitCardReaders != null &&
                entryCardReadersToAdd.Any(exitCardReaders.ContainsKey))
            {
                return false;
            }

            var entryCardReaders = _editingObject.EntryCardReaders;

            if (entryCardReaders != null)
            {
                if (entryCardReadersToAdd.Any(entryCardReaders.ContainsKey))
                    return false;
            }
            else
            {
                entryCardReaders = new Dictionary<CardReader, APBZCardReader>();

                _editingObject.EntryCardReaders = entryCardReaders;
            }

            foreach (var cardReader in entryCardReadersToAdd)
                entryCardReaders.Add(
                    cardReader,
                    new APBZCardReader
                    {
                        Direction = true,
                        EntryExitBy = entryBy
                    });

            return true;
        }

        private void _bInsertEntryCardReader_Click(object sender, EventArgs e)
        {
            var entryExitByView = _cbEntryBy.SelectedItem as ApbzCardReaderEntryExitByView;

            if (entryExitByView == null)
                return;

            if (!AddEntryCardReaders(_currentEntryCardReaders.Values, entryExitByView.ApbzCardReaderEntryExitBy))
                return;

            _currentEntryCardReaders.Clear();
            _tbmEntryCardReader.Text = string.Empty;

            _cbEntryBy.SelectedItem = null;
            _cbEntryBy.Enabled = false;

            AfterAddEntryCardReader();
        }

        private void AfterAddEntryCardReader()
        {
            ShowEntryCardReaders();
            EditTextChanger(null, null);

            var conflictDialogModel =
                new APBZConflictDialogModel(
                    Plugin.MainServerProvider.AntiPassBackZones,
                    _editingObject);

            if (!conflictDialogModel.ConflictExists)
                return;

            var result = new APBZConflictDialog(conflictDialogModel).ShowDialog();

            switch (result)
            {
                case DialogResult.OK:

                    if (conflictDialogModel.PerformAction())
                        ShowEntryCardReaders();

                    break;

                case DialogResult.Cancel:
                    return;
            }
        }

        private void AfterAddExitCardReader()
        {
            ShowExitCardReaders();
            EditTextChanger(null, null);
        }

        private void _bDeleteEntryCardReader_Click(object sender, EventArgs e)
        {
            if (_bsEntryCardReaders == null)
                return;

            var entryCardReaders = _editingObject.EntryCardReaders;

            if (entryCardReaders == null ||
                entryCardReaders.Count == 0)
            {
                return;
            }

            var entryCardReader =
                (CardReaderView)
                _bsEntryCardReaders.List[_bsEntryCardReaders.Position];

            entryCardReaders.Remove(entryCardReader.CardReader);

            ShowEntryCardReaders();
            EditTextChanger(null, null);
        }

        private BindingSource _bsEntryCardReaders;

        private BindingSource _bsExitCardReaders;

        private const string COLUMN_APBZ_CR_EXIT_BY_COMBO_BOX = "ApbzCrExitBy";

        private void ShowExitCardReaders()
        {
            var dataGridView = _cdgvExitCardReaders.DataGrid;

            if (_editingObject.ExitCardReaders == null)
            {
                _bsExitCardReaders = null;
                dataGridView.DataSource = null;

                return;
            }

            _bsExitCardReaders =
                new BindingSource
                {
                    DataSource =
                        _editingObject.ExitCardReaders.Keys
                            .Select(cardReader =>
                                new CardReaderView(
                                    this,
                                    cardReader,
                                    false))
                            .ToArray()
                };

            dataGridView.DataSource = _bsExitCardReaders;

            HideColumnDgw(dataGridView, CardReaderView.COLUMN_ENTRY_EXIT_BY);

            var nameDataGridViewColumn =
                dataGridView.Columns[CardReaderView.COLUMN_NAME];

            nameDataGridViewColumn.DisplayIndex = 0;
            nameDataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (!dataGridView.Columns.Contains(COLUMN_APBZ_CR_EXIT_BY_COMBO_BOX))
            {
                dataGridView.Columns.Add(
                    new DataGridViewComboBoxColumn
                    {
                        Name = COLUMN_APBZ_CR_EXIT_BY_COMBO_BOX,
                        AutoComplete = true,
                        FlatStyle = FlatStyle.System,
                        DataPropertyName = CardReaderView.COLUMN_ENTRY_EXIT_BY,
                        DisplayMember = ApbzCardReaderEntryExitByView.COLUMN_NAME,
                        ValueMember = ApbzCardReaderEntryExitByView.COLUMN_APBZ_CARD_READER_ENTRY_EXIT_BY,
                        ValueType = typeof(ApbzCardReaderEntryExitBy),
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                        DisplayIndex = 1,
                        MinimumWidth = 270
                    });

                _cdgvEntryCardReaders.DisabledDoubleClickEventForColumns.Add(COLUMN_APBZ_CR_EXIT_BY_COMBO_BOX);

                foreach (DataGridViewRow row in dataGridView.Rows)
                    _cdgvEntryExitCardReaders_RowsAdded(
                        _cdgvExitCardReaders.DataGrid,
                        row.Index,
                        COLUMN_APBZ_CR_EXIT_BY_COMBO_BOX);
            }

            LocalizationHelper.TranslateDataGridViewColumnsHeaders(dataGridView);
        }

        void _cdgvExitCardReaders_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            _cdgvEntryExitCardReaders_RowsAdded(
                _cdgvExitCardReaders.DataGrid,
                e.RowIndex,
                COLUMN_APBZ_CR_EXIT_BY_COMBO_BOX);
        }

        void _cdgvExitCardReaders_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _cdgvEntryExitCardReaders_EndEdit(_cdgvExitCardReaders.DataGrid);
        }

        void _cdgvExitCardReaders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0
                && _cdgvExitCardReaders.DataGrid.Columns[e.ColumnIndex].Name == COLUMN_APBZ_CR_EXIT_BY_COMBO_BOX)
            {
                _cdgvEntryExitCardReaders_BeginEdit(_cdgvExitCardReaders.DataGrid);
            }
        }

        private bool AddExitCardReaders(
            ICollection<CardReader> exitCardReadersToAdd,
            ApbzCardReaderEntryExitBy exitBy)
        {
            if (exitCardReadersToAdd.Count == 0)
                return false;

            var entryCardReaders = _editingObject.EntryCardReaders;

            if (entryCardReaders != null &&
                exitCardReadersToAdd.Any(entryCardReaders.ContainsKey))
            {
                return false;
            }

            var exitCardReaders = _editingObject.ExitCardReaders;

            if (exitCardReaders != null)
            {
                if (exitCardReadersToAdd.Any(exitCardReaders.ContainsKey))
                    return false;
            }
            else
            {
                exitCardReaders = 
                    new Dictionary<CardReader, APBZCardReader>();

                _editingObject.ExitCardReaders = exitCardReaders;
            }

            foreach (var cardReader in exitCardReadersToAdd)
                exitCardReaders.Add(
                    cardReader,
                    new APBZCardReader
                    {
                        Direction = false,
                        EntryExitBy = exitBy
                    });

            return true;
        }

        private void _bInsertExitCardReader_Click(object sender, EventArgs e)
        {
            var entryExitByView = _cbExitBy.SelectedItem as ApbzCardReaderEntryExitByView;

            if (entryExitByView == null)
                return;

            if (!AddExitCardReaders(_currentExitCardReaders.Values, entryExitByView.ApbzCardReaderEntryExitBy))
                return;

            _currentExitCardReaders.Clear();
            _tbmExitCardReader.Text = string.Empty;

            _cbExitBy.SelectedItem = null;
            _cbExitBy.Enabled = false;

            AfterAddExitCardReader();
        }

        private void _bDeleteExitCardReader_Click(object sender, EventArgs e)
        {
            if (_bsExitCardReaders == null)
                return;

            var exitCardReaders = _editingObject.ExitCardReaders;

            if (exitCardReaders == null ||
                exitCardReaders.Count == 0)
            {
                return;
            }

            var exitCardReader =
                (CardReaderView)
                _bsExitCardReaders.List[_bsExitCardReaders.Position];

            exitCardReaders.Remove(exitCardReader.CardReader);

            ShowExitCardReaders();
            EditTextChanger(null, null);
        }

        private readonly IDictionary<Guid, CardReader> _currentEntryCardReaders =
            new SyncDictionary<Guid, CardReader>();

        private readonly IDictionary<Guid, CardReader> _currentExitCardReaders =
            new SyncDictionary<Guid, CardReader>();

        private AntiPassBackZone _currentExpirationTarget;

        private void _tbmEntryCardReader_ButtonPopupMenuItemClick(
            ToolStripItem item,
            int index)
        {
            ConnectionLost();

            IDictionary<Guid, bool> isCrFromMinimalDe;

            var listBoxFormAdd =
                new ListboxFormAdd(
                    GetAssignableCardReaders(
                        true,
                        out isCrFromMinimalDe),
                    LocalizationHelper.GetString("NCASAntiPassBackZoneEditForm_lEntryCardReaders"));

            ListOfObjects listOfObjects;

            listBoxFormAdd.ShowDialogMultiSelect(out listOfObjects);

            if (listOfObjects == null)
                return;

            _currentEntryCardReaders.Clear();
            var containsCrFromMinimalDe = false;

            foreach (var obj in listOfObjects)
            {
                var cardReaderModifyObj = (CardReaderModifyObj)obj;

                var idCardReader = cardReaderModifyObj.GetId;

                _currentEntryCardReaders.Add(
                    idCardReader,
                    Plugin.MainServerProvider.CardReaders
                        .GetObjectById(idCardReader));

                bool crFromMinimalDe;

                if (isCrFromMinimalDe != null
                    && isCrFromMinimalDe.TryGetValue(
                        idCardReader,
                        out crFromMinimalDe)
                    && crFromMinimalDe)
                {
                    containsCrFromMinimalDe = true;
                }
            }

            FillEntryExitByItems(
                _cbEntryBy,
                containsCrFromMinimalDe);

            _cbEntryBy.Enabled = true;

            _tbmEntryCardReader.Text = listOfObjects.ToString();

            _tbmEntryCardReader.TextImage =
                Plugin.GetImageForObjectType(ObjectType.CardReader);
        }

        private IEnumerable<IModifyObject> GetAssignableCardReaders(
            bool cardReaderBeingSelectedIsEntry,
            out  IDictionary<Guid, bool> isCardReaderFromMinimalDe)
        {
            var ccu = _editingObject.GetParentCCU();
            Exception err;

            ICollection<IModifyObject> result =
                Plugin.MainServerProvider.CardReaders.GetAPBZAssignableCRModifyObjects(
                    ccu != null
                        ? ccu.IdCCU
                        : Guid.Empty,
                    out isCardReaderFromMinimalDe,
                    out err);

            if (err != null)
                throw err;

            var excludedReadersGuids =
                    GetExcludedCardReaderGuids(cardReaderBeingSelectedIsEntry);

            return
                result.Where(
                    modifyObject =>
                        !excludedReadersGuids.Contains(modifyObject.GetId));
        }

        private void FillEntryExitByItems(
            ComboBox entryExitByComboBox,
            bool crFromMinimalDe)
        {
            entryExitByComboBox.Items.Clear();

            var selectedApbzCardReaderEntryExit = crFromMinimalDe
                ? ApbzCardReaderEntryExitBy.AccessPermitted
                : ApbzCardReaderEntryExitBy.NormalAccess;

            var items = GetEntryExitByItems(crFromMinimalDe);

            foreach (var apbzCardReaderEntryExitByView in items)
            {
                entryExitByComboBox.Items.Add(apbzCardReaderEntryExitByView);

                if (apbzCardReaderEntryExitByView.ApbzCardReaderEntryExitBy == selectedApbzCardReaderEntryExit)
                    entryExitByComboBox.SelectedItem = apbzCardReaderEntryExitByView;
            }
        }

        private IEnumerable<ApbzCardReaderEntryExitByView> GetEntryExitByItems(bool crFromMinimalDe)
        {
            if (crFromMinimalDe)
            {
                yield return new ApbzCardReaderEntryExitByView(ApbzCardReaderEntryExitBy.AccessPermitted);

                yield break;
            }

            yield return new ApbzCardReaderEntryExitByView(ApbzCardReaderEntryExitBy.NormalAccess);
            yield return new ApbzCardReaderEntryExitByView(ApbzCardReaderEntryExitBy.AccessPermitted);
            yield return new ApbzCardReaderEntryExitByView(ApbzCardReaderEntryExitBy.AccessInterupted);
        }

        private void _tbmExitCardReader_ButtonPopupMenuItemClick(
            ToolStripItem item,
            int index)
        {
            ConnectionLost();

            IDictionary<Guid, bool> isCrFromMinimalDe;

            var listBoxFormAdd =
                new ListboxFormAdd(
                    GetAssignableCardReaders(
                        false,
                        out isCrFromMinimalDe),
                    LocalizationHelper.GetString("NCASAntiPassBackZoneEditForm_lExitCardReaders"));

            ListOfObjects listOfObjects;

            listBoxFormAdd.ShowDialogMultiSelect(out listOfObjects);

            if (listOfObjects == null)
                return;

            _currentExitCardReaders.Clear();
            var containsCrFromMinimalDe = false;

            foreach (var obj in listOfObjects)
            {
                var idCardReader = ((CardReaderModifyObj) obj).GetId;

                _currentExitCardReaders.Add(
                    idCardReader,
                    Plugin.MainServerProvider.CardReaders
                        .GetObjectById(idCardReader));

                bool crFromMinimalDe;

                if (isCrFromMinimalDe != null
                    && isCrFromMinimalDe.TryGetValue(
                        idCardReader,
                        out crFromMinimalDe)
                    && crFromMinimalDe)
                {
                    containsCrFromMinimalDe = true;
                }
            }

            FillEntryExitByItems(
                _cbExitBy,
                containsCrFromMinimalDe);

            _cbExitBy.Enabled = true;

            _tbmExitCardReader.Text = listOfObjects.ToString();

            _tbmExitCardReader.TextImage = 
                Plugin.GetImageForObjectType(ObjectType.CardReader);
        }

        private ICollection<Guid> GetExcludedCardReaderGuids(bool isEntry)
        {
            IEnumerable<Guid> excludedCardReaders =
                isEntry
                    ? Plugin.MainServerProvider.AntiPassBackZones
                        .GetExistingEntryCardReaderGuids(
                            _editingObject.IdAntiPassBackZone,
                            ApbzCardReaderEntryExitBy.AccessPermitted)
                    : Enumerable.Empty<Guid>();

            var entryCardReaders = _editingObject.EntryCardReaders;
            var exitCardReaders = _editingObject.ExitCardReaders;

            if (entryCardReaders != null)
                excludedCardReaders =
                    excludedCardReaders.Concat(
                        entryCardReaders.Keys
                            .Select(cardReader => cardReader.IdCardReader));

            if (exitCardReaders != null)
                excludedCardReaders =
                    excludedCardReaders.Concat(
                        exitCardReaders.Keys
                            .Select(cardReader => cardReader.IdCardReader));

            return new HashSet<Guid>(excludedCardReaders);
        }

        private void _tpCardReaders_Enter(object sender, EventArgs e)
        {
            ShowEntryCardReaders();
            ShowExitCardReaders();
        }

        private void _nudTimeout_ValueChanged(object sender, EventArgs e)
        {
            SetExpirationTarget(_currentExpirationTarget);

            EditTextChanger(sender, e);
        }

        private int TimeoutValueInMinutes { get
        {
            return (int)(_nudTimeout.Value - _nudTimeout.MinDate).TotalMinutes;
        }
        }

        private void _tbName_TextChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _tbDescription_TextChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _cbProhibitAccessForCardNotPresent_CheckedChanged(
            object sender,
            EventArgs e)
        {
            EditTextChanger(
                sender,
                e);
        }

        private void _tbmExpirationTarget_ButtonPopupMenu_ItemClicked(
            object sender,
            ToolStripItemClickedEventArgs e)
        {
            AntiPassBackZone antiPassBackZone;

            if (e.ClickedItem == _tsiModifyExpirationTarget)
            {
                antiPassBackZone = SelectExpirationTarget();

                if (antiPassBackZone == null)
                    return;
            }
            else
                if (e.ClickedItem == _tsiRemoveExpirationTarget)
                    antiPassBackZone = null;
                else
                    return;

            SetExpirationTarget(antiPassBackZone);

            EditTextChanger(_tbmExpirationTarget, e);
        }

        private AntiPassBackZone SelectExpirationTarget()
        {
            Exception err;

            var parentCCU = _editingObject.GetParentCCU();

            var modifyObjects =
                Plugin.MainServerProvider.AntiPassBackZones
                    .GetAvailableExpirationTargets(
                        _editingObject.IdAntiPassBackZone,
                        parentCCU != null
                            ? parentCCU.IdCCU
                            : Guid.Empty,
                        out err);

            if (err != null)
                throw err;

            if (modifyObjects == null)
                return null;

            var objects =
                modifyObjects
                    .Where(
                        modifyObject =>
                            !modifyObject.GetId.Equals(_editingObject.IdAntiPassBackZone))
                     .ToList();

            var listBoxFormAdd =
                new ListboxFormAdd(
                    objects,
                    "Select after timeout destination anti-passback zone");

            IModifyObject outModifyObject;
            listBoxFormAdd.ShowDialog(out outModifyObject);

            if (outModifyObject == null)
                return null;

            return
                Plugin.MainServerProvider.AntiPassBackZones
                    .GetObjectById(outModifyObject.GetId);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(
                _lbUserFolders,
                _editingObject);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(
                _lbUserFolders,
                _editingObject);
        }

        private void _lbUserFolders_MouseDoubleClick(
            object sender,
            MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(
                _lbUserFolders,
                _editingObject);
        }

        private void _cbSelectUnselectAll_CheckedChanged(
            object sender,
            EventArgs e)
        {
            if (_cbSelectUnselectAll.CheckState == CheckState.Indeterminate)
                return;

            foreach (var apbzCardInZone in _cardsInZone)
                apbzCardInZone.IsChecked = _cbSelectUnselectAll.Checked;

            ((BindingSource)
                _cdgvCards.DataGrid.DataSource).ResetBindings(false);
        }

        private void _bRefreshCards_Click(object sender, EventArgs e)
        {
            _tbSearch.Text = string.Empty;
            ShowCards();
        }

        private void _bRemove_Click(object sender, EventArgs e)
        {
            Plugin.MainServerProvider.AntiPassBackZones.RemoveCardsFromZone(
                _editingObject.IdAntiPassBackZone,
                new LinkedList<Guid>(
                    _cardsInZone
                        .Where(cardInZone => cardInZone.IsChecked)
                        .Select(cardInZone => cardInZone.Id)));

            ShowCards();
        }

        private void _tpCardList_Enter(object sender, EventArgs e)
        {
            ShowCards();
        }

        private IList<CardInZoneView> _cardsInZone;

        private void ShowCards()
        {
            var apbzCardsInZone =
                Plugin.MainServerProvider.AntiPassBackZones
                    .GetCardsInZone(_editingObject.IdAntiPassBackZone);

            _cardsInZone =
                apbzCardsInZone != null
                    ? apbzCardsInZone
                        .Select(cardInZone => new CardInZoneView(cardInZone))
                        .ToArray()
                    : new CardInZoneView[0];

            _bsCards.DataSource = _cardsInZone;

            var dataGridView = _cdgvCards.DataGrid;

            DataGridViewColumnCollection cdgvCardsColumnCollection =
                dataGridView.Columns;

            var isCheckedDataGridViewColumn =
                cdgvCardsColumnCollection[CardInZoneView.COLUMN_ISCHECKED];

            isCheckedDataGridViewColumn.DisplayIndex = 0;
            isCheckedDataGridViewColumn.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            isCheckedDataGridViewColumn.FillWeight = 10;

            var nameDataGridViewColumn =
                cdgvCardsColumnCollection[CardInZoneView.COLUMN_NAME];

            nameDataGridViewColumn.DisplayIndex = 1;
            nameDataGridViewColumn.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            nameDataGridViewColumn.FillWeight = 30;

            var cardNumberDataGridViewColumn =
                cdgvCardsColumnCollection[CardInZoneView.COLUMN_CARDNUMBER];

            cardNumberDataGridViewColumn.DisplayIndex = 2;
            cardNumberDataGridViewColumn.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            cardNumberDataGridViewColumn.FillWeight = 40;

            var entryDateTimeDataGridViewColumn =
                cdgvCardsColumnCollection[CardInZoneView.COLUMN_ENTRYDATETIME];

            entryDateTimeDataGridViewColumn.DisplayIndex = 3;
            entryDateTimeDataGridViewColumn.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            entryDateTimeDataGridViewColumn.FillWeight = 20;

            var entryCardReaderNameDataGridViewColumn =
                cdgvCardsColumnCollection[CardInZoneView.COLUMN_ENTRYCARDREADERNAME];

            entryCardReaderNameDataGridViewColumn.DisplayIndex = 4;
            entryCardReaderNameDataGridViewColumn.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            entryCardReaderNameDataGridViewColumn.FillWeight = 30;

            var accessInterruptedDataGridViewColumn =
                cdgvCardsColumnCollection[CardInZoneView.COLUMN_APBZ_CR_ENTRY_BY];

            accessInterruptedDataGridViewColumn.DisplayIndex = 5;
            accessInterruptedDataGridViewColumn.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
            accessInterruptedDataGridViewColumn.FillWeight = 20;

            LocalizationHelper.TranslateDataGridViewColumnsHeaders(dataGridView);
            _lCardsCountInfo.Text = string.Format("{0}/{1}", _bsCards.Count, _cardsInZone.Count);
        }

        private void _tbmExpirationTarget_ImageTextBox_DoubleClick(
            object sender, 
            EventArgs e)
        {
            if (_currentExpirationTarget == null)
                return;

            NCASAntiPassBackZonesForm.Singleton.OpenEditForm(
                _currentExpirationTarget);
        }

        private void _tbmExpirationTarget_ImageTextBox_DragOver(
            object sender, 
            DragEventArgs e)
        {
            e.Effect =
                e.Data.GetDataPresent(typeof(AntiPassBackZone))
                    ? DragDropEffects.All
                    : DragDropEffects.None;
        }

        private void _tbmExpirationTarget_ImageTextBox_DragDrop(
            object sender, 
            DragEventArgs e)
        {
            var antiPassBackZone =
                e.Data.GetData(typeof(AntiPassBackZone))
                    as AntiPassBackZone;

            if (antiPassBackZone == null)
                return;

            SetExpirationTarget(antiPassBackZone);
            EditTextChanger(null, null);
        }

        private void _tbmEntryCardReader_ImageTextBox_DragDrop(
            object sender,
            DragEventArgs e)
        {
            var cardReader =
                e.Data.GetData(typeof(CardReader))
                    as CardReader;

            if (cardReader == null)
                return;

            _currentEntryCardReaders.Clear();

            _currentEntryCardReaders.Add(
                cardReader.IdCardReader,
                cardReader);

            FillEntryExitByItems(
                _cbEntryBy,
                Plugin.MainServerProvider.CardReaders.IsFromMinimalDe(cardReader.IdCardReader));

            _cbEntryBy.Enabled = true;

            _tbmEntryCardReader.Text = ListOfObjects.ToString(new[]{ cardReader });

            _tbmEntryCardReader.TextImage =
                Plugin.GetImageForObjectType(ObjectType.CardReader);
        }

        private void _tbmEntryCardReader_ImageTextBox_DragOver(
            object sender, 
            DragEventArgs e)
        {
            e.Effect =
                e.Data.GetDataPresent(typeof(CardReader))
                    ? DragDropEffects.Move
                    : DragDropEffects.None;
        }

        private void _tbmExitCardReader_ImageTextBox_DragDrop(
            object sender, 
            DragEventArgs e)
        {
            var cardReader =
                e.Data.GetData(typeof(CardReader))
                    as CardReader;

            if (cardReader == null)
                return;

            _currentExitCardReaders.Clear();

            _currentExitCardReaders.Add(
                cardReader.IdCardReader,
                cardReader);

            FillEntryExitByItems(
                _cbExitBy,
                Plugin.MainServerProvider.CardReaders.IsFromMinimalDe(cardReader.IdCardReader));

            _cbExitBy.Enabled = true;

            _tbmExitCardReader.Text = ListOfObjects.ToString(new[] {cardReader});

            _tbmExitCardReader.TextImage =
                Plugin.GetImageForObjectType(ObjectType.CardReader);
        }

        private void _tbmExitCardReader_ImageTextBox_DragOver(
            object sender, 
            DragEventArgs e)
        {
            e.Effect =
                e.Data.GetDataPresent(typeof(CardReader))
                    ? DragDropEffects.Move
                    : DragDropEffects.None;
        }

        private static void _cdgvEntryCardReaders_DragOver(
            object sender,
            DragEventArgs e)
        {
            e.Effect =
                e.Data.GetDataPresent(typeof(CardReader))
                    ? DragDropEffects.Move
                    : DragDropEffects.None;
        }

        private void _cdgvEntryCardReaders_DragDrop(
            object sender,
            DragEventArgs e)
        {
            var cardReader =
                e.Data.GetData(typeof(CardReader))
                    as CardReader;

            if (cardReader == null)
                return;

            var entryBy = Plugin.MainServerProvider.CardReaders.IsFromMinimalDe(cardReader.IdCardReader)
                ? ApbzCardReaderEntryExitBy.AccessPermitted
                : ApbzCardReaderEntryExitBy.NormalAccess;

            if (AddEntryCardReaders(new[] { cardReader }, entryBy))
                AfterAddEntryCardReader();
        }

        private static void _cdgvExitCardReaders_DragOver(
            object sender,
            DragEventArgs e)
        {
            e.Effect =
                e.Data.GetDataPresent(typeof(CardReader))
                    ? DragDropEffects.Move
                    : DragDropEffects.None;
        }

        private void _cdgvExitCardReaders_DragDrop(
            object sender,
            DragEventArgs e)
        {
            var cardReader =
                e.Data.GetData(typeof(CardReader))
                    as CardReader;

            if (cardReader == null)
                return;

            var exitBy = Plugin.MainServerProvider.CardReaders.IsFromMinimalDe(cardReader.IdCardReader)
                ? ApbzCardReaderEntryExitBy.AccessPermitted
                : ApbzCardReaderEntryExitBy.NormalAccess;

            if (AddExitCardReaders(new[] { cardReader }, exitBy))
                AfterAddExitCardReader();
        }

        private void _bFilter_Click(object sender, EventArgs e)
        {
            RunCardsFilter();
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            _tbSearch.Text = string.Empty;
            _bsCards.DataSource = _cardsInZone;
        }

        private void RunCardsFilter()
        {
            _bsCards.DataSource = _cardsInZone.Where(
                c => c.Contains(_tbSearch.Text));

            _lCardsCountInfo.Text = string.Format("{0}/{1}", _bsCards.Count, _cardsInZone.Count);
        }

        private void _tbSearch_TextChanged(object sender, EventArgs e)
        {
            RunCardsFilter();
        }

        private void _tbmEntryCardReader_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            var cardReaders = 
                _currentEntryCardReaders.Values;

            if (cardReaders.Count == 1)
                NCASCardReadersForm.Singleton.OpenEditForm(cardReaders.First());
        }

        private void _tbmExitCardReader_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            var cardReaders = 
                _currentExitCardReaders.Values;

            if (cardReaders.Count == 1)
                NCASCardReadersForm.Singleton.OpenEditForm(cardReaders.First());
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            ConnectionLost();

            ICollection<Card> cardsWhichCanBeAdded;

            try
            {
                cardsWhichCanBeAdded = Plugin.MainServerProvider.AntiPassBackZones.GetCardsWhichCanBeAdded(
                    _editingObject.IdAntiPassBackZone);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return;
            }

            if (cardsWhichCanBeAdded == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _bAdd,
                    LocalizationHelper.GetString("NCASAntiPassBackZoneEditForm_NoCardsToAdd"),
                    ControlNotificationSettings.Default);

                return;
            }

            var addedPersons = new HashSet<Guid>();

            var cardsAndPersons = cardsWhichCanBeAdded
                .SelectMany(
                    card =>
                        Enumerable.Repeat(
                            card,
                            1).Cast<AOrmObject>()
                            .Concat(
                                addedPersons.Add(card.Person.IdPerson)
                                    ? Enumerable.Repeat(
                                        card.Person,
                                        1).Cast<AOrmObject>()
                                    : Enumerable.Empty<AOrmObject>()));



            var listBoxFormAdd =
                new ListboxFormAdd(
                    cardsAndPersons,
                    LocalizationHelper.GetString("NCASAntiPassBackZoneEditForm_AddCardsAndPersons"));

            object outObject;

            listBoxFormAdd.ShowDialog(out outObject);

            var ormObject = outObject as AOrmObject;

            if (ormObject == null)
                return;

            IEnumerable<Card> cardsToAdd;

            if (ormObject.GetObjectType() == ObjectType.Card)
            {
                cardsToAdd = Enumerable.Repeat(
                    ormObject as Card,
                    1);
            }
            else
            {
                var person = ormObject as Person;

                if (person == null)
                    return;

                ICollection<Card> cardsForPerson;

                try
                {
                    cardsForPerson = Plugin.MainServerProvider.AntiPassBackZones.GetCardsWhichCanBeAdded(
                        _editingObject.IdAntiPassBackZone,
                        person.IdPerson);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    return;
                }

                if (cardsForPerson == null)
                    return;

                cardsToAdd = SelectCardsFromPerson(
                    cardsForPerson,
                    person.ToString());

                if (cardsToAdd == null)
                    return;
            }

            Plugin.MainServerProvider.AntiPassBackZones.AddCardsToZone(
                _editingObject.IdAntiPassBackZone,
                new LinkedList<Guid>(
                    cardsToAdd.Select(
                        card =>
                            card.IdCard)));

            ShowCards();
        }

        private IEnumerable<Card> SelectCardsFromPerson(
            [NotNull]
            ICollection<Card> cardsForPerson,
            string personName)
        {
            if (cardsForPerson.Count == 1)
            {
                return cardsForPerson;
            }

            var listBoxFormAddCards =
                new ListboxFormAdd(
                    cardsForPerson.OfType<AOrmObject>(),
                    string.Format(
                        LocalizationHelper.GetString("NCASAntiPassBackZoneEditForm_AddCards"),
                        personName));

            object outObject;

            listBoxFormAddCards.ShowDialog(out outObject);

            var card = outObject as Card;

            if (card == null)
                return null;

            return Enumerable.Repeat(
                card,
                1);
        }

        void _cdgvCards_DragOver(object sender, DragEventArgs e)
        {
            e.Effect =
                e.Data.GetDataPresent(typeof (Card))
                || e.Data.GetDataPresent(typeof(Person))
                    ? DragDropEffects.Move
                    : DragDropEffects.None;
        }

        private void _cdgvCards_DragDrop(object sender, DragEventArgs e)
        {
            IEnumerable<Card> cardsToAdd;

            var card =
                e.Data.GetData(typeof (Card))
                    as Card;

            if (card != null)
            {
                if (!Plugin.MainServerProvider.AntiPassBackZones.CardCanBeAdded(
                    _editingObject.IdAntiPassBackZone,
                    card.IdCard))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cdgvCards.DataGrid,
                         LocalizationHelper.GetString("NCASAntiPassBackZoneEditForm_CardCanNotBeAdded"),
                        ControlNotificationSettings.Default);

                    return;
                }

                cardsToAdd = Enumerable.Repeat(
                    card,
                    1);
            }
            else
            {
                var person = e.Data.GetData(typeof (Person))
                    as Person;

                if (person == null)
                    return;

                ICollection<Card> cardsForPerson;

                try
                {
                    cardsForPerson = Plugin.MainServerProvider.AntiPassBackZones.GetCardsWhichCanBeAdded(
                        _editingObject.IdAntiPassBackZone,
                        person.IdPerson);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    return;
                }

                if (cardsForPerson == null)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cdgvCards.DataGrid,
                        LocalizationHelper.GetString("NCASAntiPassBackZoneEditForm_PersonHasNoCardThatCanBeAdded"),
                        ControlNotificationSettings.Default);

                    return;
                }

                cardsToAdd = SelectCardsFromPerson(
                    cardsForPerson,
                    person.ToString());

                if (cardsToAdd == null)
                    return;
            }

            Plugin.MainServerProvider.AntiPassBackZones.AddCardsToZone(
                _editingObject.IdAntiPassBackZone,
                new LinkedList<Guid>(
                    cardsToAdd.Select(
                        cardToAdd =>
                            cardToAdd.IdCard)));

            ShowCards();
        }
    }
}
