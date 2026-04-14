namespace Contal.Cgp.Client
{
    partial class CSVCardImportDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._dgCSVImportColumns = new System.Windows.Forms.DataGridView();
            this.ImportRow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PersonalNumber = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ControlCode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PersonalID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.CardNumber = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PIN = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._cbSeparator = new System.Windows.Forms.ComboBox();
            this._lSeparator = new System.Windows.Forms.Label();
            this.openFileDialogCSVImport = new System.Windows.Forms.OpenFileDialog();
            this._eFilePath = new System.Windows.Forms.TextBox();
            this._lFilePath = new System.Windows.Forms.Label();
            this._bOpenFile = new System.Windows.Forms.Button();
            this._dgCSVImportValues = new System.Windows.Forms.DataGridView();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._lTemplate = new System.Windows.Forms.Label();
            this._cbTemplate = new System.Windows.Forms.ComboBox();
            this._bImport = new System.Windows.Forms.Button();
            this._pbLoadFileImport = new System.Windows.Forms.ProgressBar();
            this._tcImportReport = new System.Windows.Forms.TabControl();
            this._tpImport = new System.Windows.Forms.TabPage();
            this._eReplacmentPin = new System.Windows.Forms.TextBox();
            this._lReplacementPin = new System.Windows.Forms.Label();
            this._cbPersonalIdSeparator = new System.Windows.Forms.ComboBox();
            this._lPersonalIdSeparator = new System.Windows.Forms.Label();
            this._chbCardSystem = new System.Windows.Forms.CheckBox();
            this._cbCardSystem = new System.Windows.Forms.ComboBox();
            this._lCardSystem = new System.Windows.Forms.Label();
            this._tpReport = new System.Windows.Forms.TabPage();
            this._bEdit = new System.Windows.Forms.Button();
            this._dgReport = new System.Windows.Forms.DataGridView();
            this.IdCard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImportResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dgCSVImportColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgCSVImportValues)).BeginInit();
            this._tcImportReport.SuspendLayout();
            this._tpImport.SuspendLayout();
            this._tpReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgReport)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgCSVImportColumns
            // 
            this._dgCSVImportColumns.AllowUserToAddRows = false;
            this._dgCSVImportColumns.AllowUserToDeleteRows = false;
            this._dgCSVImportColumns.AllowUserToResizeColumns = true;
            this._dgCSVImportColumns.AllowUserToResizeRows = false;
            this._dgCSVImportColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgCSVImportColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgCSVImportColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImportRow,
            this.PersonalNumber,
            this.ControlCode,
            this.PersonalID,
            this.CardNumber,
            this.PIN});
            this._dgCSVImportColumns.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgCSVImportColumns.Location = new System.Drawing.Point(6, 106);
            this._dgCSVImportColumns.Name = "_dgCSVImportColumns";
            this._dgCSVImportColumns.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._dgCSVImportColumns.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this._dgCSVImportColumns.Size = new System.Drawing.Size(883, 60);
            this._dgCSVImportColumns.TabIndex = 13;
            this._dgCSVImportColumns.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this._dgCSVImportColumns_EditingControlShowing);
            this._dgCSVImportColumns.CurrentCellDirtyStateChanged += new System.EventHandler(this._dgCSVImportColumns_CurrentCellDirtyStateChanged);
            this._dgCSVImportColumns.Click += new System.EventHandler(this._dgCSVImportColumns_Click);
            // 
            // ImportRow
            // 
            this.ImportRow.HeaderText = "Import row";
            this.ImportRow.MinimumWidth = 140;
            this.ImportRow.Name = "ImportRow";
            this.ImportRow.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ImportRow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ImportRow.Width = 140;
            // 
            // PersonalNumber
            // 
            this.PersonalNumber.HeaderText = "Personal number";
            this.PersonalNumber.MinimumWidth = 140;
            this.PersonalNumber.Name = "PersonalNumber";
            this.PersonalNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PersonalNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.PersonalNumber.Width = 140;
            // 
            // ControlCode
            // 
            this.ControlCode.HeaderText = "Control code";
            this.ControlCode.MinimumWidth = 140;
            this.ControlCode.Name = "ControlCode";
            this.ControlCode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ControlCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ControlCode.Width = 140;
            // 
            // PersonalID
            // 
            this.PersonalID.HeaderText = "Presonal ID";
            this.PersonalID.MinimumWidth = 140;
            this.PersonalID.Name = "PersonalID";
            this.PersonalID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PersonalID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.PersonalID.Width = 140;
            // 
            // CardNumber
            // 
            this.CardNumber.HeaderText = "Card number";
            this.CardNumber.MinimumWidth = 140;
            this.CardNumber.Name = "CardNumber";
            this.CardNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CardNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CardNumber.Width = 140;
            // 
            // PIN
            // 
            this.PIN.HeaderText = "PIN";
            this.PIN.MinimumWidth = 140;
            this.PIN.Name = "PIN";
            this.PIN.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PIN.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.PIN.Width = 140;
            // 
            // _cbSeparator
            // 
            this._cbSeparator.FormattingEnabled = true;
            this._cbSeparator.Location = new System.Drawing.Point(98, 40);
            this._cbSeparator.Name = "_cbSeparator";
            this._cbSeparator.Size = new System.Drawing.Size(109, 21);
            this._cbSeparator.TabIndex = 4;
            this._cbSeparator.SelectedValueChanged += new System.EventHandler(this._cbSeparator_SelectedValueChanged);
            // 
            // _lSeparator
            // 
            this._lSeparator.AutoSize = true;
            this._lSeparator.Location = new System.Drawing.Point(6, 43);
            this._lSeparator.Name = "_lSeparator";
            this._lSeparator.Size = new System.Drawing.Size(53, 13);
            this._lSeparator.TabIndex = 3;
            this._lSeparator.Text = "Separator";
            // 
            // openFileDialogCSVImport
            // 
            this.openFileDialogCSVImport.Filter = "(*.csv, *.tsv, *.txt)|*.csv;*.tsv;*.txt|(*.*)|*.*";
            // 
            // _eFilePath
            // 
            this._eFilePath.Location = new System.Drawing.Point(98, 10);
            this._eFilePath.Name = "_eFilePath";
            this._eFilePath.ReadOnly = true;
            this._eFilePath.Size = new System.Drawing.Size(435, 20);
            this._eFilePath.TabIndex = 1;
            // 
            // _lFilePath
            // 
            this._lFilePath.AutoSize = true;
            this._lFilePath.Location = new System.Drawing.Point(6, 13);
            this._lFilePath.Name = "_lFilePath";
            this._lFilePath.Size = new System.Drawing.Size(47, 13);
            this._lFilePath.TabIndex = 0;
            this._lFilePath.Text = "File path";
            // 
            // _bOpenFile
            // 
            this._bOpenFile.Location = new System.Drawing.Point(539, 8);
            this._bOpenFile.Name = "_bOpenFile";
            this._bOpenFile.Size = new System.Drawing.Size(100, 23);
            this._bOpenFile.TabIndex = 2;
            this._bOpenFile.Text = "Open file";
            this._bOpenFile.UseVisualStyleBackColor = true;
            this._bOpenFile.Click += new System.EventHandler(this._bOpenFile_Click);
            // 
            // _dgCSVImportValues
            // 
            this._dgCSVImportValues.AllowUserToAddRows = false;
            this._dgCSVImportValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgCSVImportValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._dgCSVImportValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgCSVImportValues.ColumnHeadersVisible = false;
            this._dgCSVImportValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewCheckBoxColumn1,
            this.Column1,
            this.Column2,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this._dgCSVImportValues.Location = new System.Drawing.Point(6, 172);
            this._dgCSVImportValues.Name = "_dgCSVImportValues";
            this._dgCSVImportValues.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._dgCSVImportValues.Size = new System.Drawing.Size(883, 322);
            this._dgCSVImportValues.TabIndex = 14;
            this._dgCSVImportValues.Scroll += new System.Windows.Forms.ScrollEventHandler(this._dgCSVImportValues_Scroll);
            this._dgCSVImportValues.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this._dgCSVImportValues_ColumnWidthChanged);
            this._dgCSVImportValues.Click += new System.EventHandler(this._dgCSVImportValues_Click);
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.DataPropertyName = "Column6";
            this.dataGridViewCheckBoxColumn1.HeaderText = "Import row";
            this.dataGridViewCheckBoxColumn1.MinimumWidth = 140;
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.Width = 140;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Column1";
            this.Column1.HeaderText = "Personal number";
            this.Column1.MinimumWidth = 140;
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.Width = 140;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Column2";
            this.Column2.HeaderText = "Control code";
            this.Column2.MinimumWidth = 140;
            this.Column2.Name = "Column2";
            this.Column2.Width = 140;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Column3";
            this.dataGridViewTextBoxColumn2.HeaderText = "Personal ID";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 140;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 140;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Column4";
            this.dataGridViewTextBoxColumn3.HeaderText = "Card number";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 140;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 140;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Column5";
            this.dataGridViewTextBoxColumn4.HeaderText = "PIN";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 140;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 140;
            // 
            // _lTemplate
            // 
            this._lTemplate.AutoSize = true;
            this._lTemplate.Location = new System.Drawing.Point(213, 43);
            this._lTemplate.Name = "_lTemplate";
            this._lTemplate.Size = new System.Drawing.Size(97, 13);
            this._lTemplate.TabIndex = 5;
            this._lTemplate.Text = "Template for import";
            // 
            // _cbTemplate
            // 
            this._cbTemplate.FormattingEnabled = true;
            this._cbTemplate.Location = new System.Drawing.Point(316, 40);
            this._cbTemplate.Name = "_cbTemplate";
            this._cbTemplate.Size = new System.Drawing.Size(134, 21);
            this._cbTemplate.TabIndex = 6;
            this._cbTemplate.SelectedValueChanged += new System.EventHandler(this._cbTemplate_SelectedValueChanged);
            // 
            // _bImport
            // 
            this._bImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bImport.Enabled = false;
            this._bImport.Location = new System.Drawing.Point(814, 71);
            this._bImport.Name = "_bImport";
            this._bImport.Size = new System.Drawing.Size(75, 23);
            this._bImport.TabIndex = 12;
            this._bImport.Text = "Import";
            this._bImport.UseVisualStyleBackColor = true;
            this._bImport.Click += new System.EventHandler(this._bImport_Click);
            // 
            // _pbLoadFileImport
            // 
            this._pbLoadFileImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pbLoadFileImport.Location = new System.Drawing.Point(6, 500);
            this._pbLoadFileImport.Name = "_pbLoadFileImport";
            this._pbLoadFileImport.Size = new System.Drawing.Size(881, 10);
            this._pbLoadFileImport.Step = 1;
            this._pbLoadFileImport.TabIndex = 15;
            // 
            // _tcImportReport
            // 
            this._tcImportReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcImportReport.Controls.Add(this._tpImport);
            this._tcImportReport.Controls.Add(this._tpReport);
            this._tcImportReport.Location = new System.Drawing.Point(12, 12);
            this._tcImportReport.Name = "_tcImportReport";
            this._tcImportReport.SelectedIndex = 0;
            this._tcImportReport.Size = new System.Drawing.Size(903, 542);
            this._tcImportReport.TabIndex = 0;
            // 
            // _tpImport
            // 
            this._tpImport.BackColor = System.Drawing.SystemColors.Control;
            this._tpImport.Controls.Add(this._eReplacmentPin);
            this._tpImport.Controls.Add(this._lReplacementPin);
            this._tpImport.Controls.Add(this._cbPersonalIdSeparator);
            this._tpImport.Controls.Add(this._lPersonalIdSeparator);
            this._tpImport.Controls.Add(this._chbCardSystem);
            this._tpImport.Controls.Add(this._cbCardSystem);
            this._tpImport.Controls.Add(this._lCardSystem);
            this._tpImport.Controls.Add(this._cbSeparator);
            this._tpImport.Controls.Add(this._dgCSVImportColumns);
            this._tpImport.Controls.Add(this._pbLoadFileImport);
            this._tpImport.Controls.Add(this._lSeparator);
            this._tpImport.Controls.Add(this._bImport);
            this._tpImport.Controls.Add(this._eFilePath);
            this._tpImport.Controls.Add(this._lFilePath);
            this._tpImport.Controls.Add(this._bOpenFile);
            this._tpImport.Controls.Add(this._lTemplate);
            this._tpImport.Controls.Add(this._dgCSVImportValues);
            this._tpImport.Controls.Add(this._cbTemplate);
            this._tpImport.Location = new System.Drawing.Point(4, 22);
            this._tpImport.Name = "_tpImport";
            this._tpImport.Padding = new System.Windows.Forms.Padding(3);
            this._tpImport.Size = new System.Drawing.Size(895, 516);
            this._tpImport.TabIndex = 0;
            this._tpImport.Text = "Import";
            // 
            // _eReplacmentPin
            // 
            this._eReplacmentPin.Location = new System.Drawing.Point(632, 71);
            this._eReplacmentPin.Name = "_eReplacmentPin";
            this._eReplacmentPin.Size = new System.Drawing.Size(169, 20);
            this._eReplacmentPin.TabIndex = 17;
            // 
            // _lReplacementPin
            // 
            this._lReplacementPin.AutoSize = true;
            this._lReplacementPin.Location = new System.Drawing.Point(545, 74);
            this._lReplacementPin.Name = "_lReplacementPin";
            this._lReplacementPin.Size = new System.Drawing.Size(87, 13);
            this._lReplacementPin.TabIndex = 16;
            this._lReplacementPin.Text = "Replacement pin";
            // 
            // _cbPersonalIdSeparator
            // 
            this._cbPersonalIdSeparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPersonalIdSeparator.Enabled = false;
            this._cbPersonalIdSeparator.FormattingEnabled = true;
            this._cbPersonalIdSeparator.Location = new System.Drawing.Point(581, 40);
            this._cbPersonalIdSeparator.Name = "_cbPersonalIdSeparator";
            this._cbPersonalIdSeparator.Size = new System.Drawing.Size(220, 21);
            this._cbPersonalIdSeparator.TabIndex = 8;
            this._cbPersonalIdSeparator.SelectedValueChanged += new System.EventHandler(this._cbPersonalIdSeparator_SelectedValueChanged);
            // 
            // _lPersonalIdSeparator
            // 
            this._lPersonalIdSeparator.AutoSize = true;
            this._lPersonalIdSeparator.Location = new System.Drawing.Point(456, 43);
            this._lPersonalIdSeparator.Name = "_lPersonalIdSeparator";
            this._lPersonalIdSeparator.Size = new System.Drawing.Size(109, 13);
            this._lPersonalIdSeparator.TabIndex = 7;
            this._lPersonalIdSeparator.Text = "Personal ID separator";
            // 
            // _chbCardSystem
            // 
            this._chbCardSystem.AutoSize = true;
            this._chbCardSystem.Checked = true;
            this._chbCardSystem.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chbCardSystem.Location = new System.Drawing.Point(9, 74);
            this._chbCardSystem.Name = "_chbCardSystem";
            this._chbCardSystem.Size = new System.Drawing.Size(116, 17);
            this._chbCardSystem.TabIndex = 9;
            this._chbCardSystem.Text = "Assign card system";
            this._chbCardSystem.UseVisualStyleBackColor = true;
            this._chbCardSystem.CheckedChanged += new System.EventHandler(this._chbCardSystem_CheckedChanged);
            // 
            // _cbCardSystem
            // 
            this._cbCardSystem.FormattingEnabled = true;
            this._cbCardSystem.Location = new System.Drawing.Point(316, 71);
            this._cbCardSystem.Name = "_cbCardSystem";
            this._cbCardSystem.Size = new System.Drawing.Size(217, 21);
            this._cbCardSystem.TabIndex = 11;
            // 
            // _lCardSystem
            // 
            this._lCardSystem.AutoSize = true;
            this._lCardSystem.Location = new System.Drawing.Point(219, 74);
            this._lCardSystem.Name = "_lCardSystem";
            this._lCardSystem.Size = new System.Drawing.Size(64, 13);
            this._lCardSystem.TabIndex = 10;
            this._lCardSystem.Text = "Card system";
            // 
            // _tpReport
            // 
            this._tpReport.BackColor = System.Drawing.SystemColors.Control;
            this._tpReport.Controls.Add(this._bEdit);
            this._tpReport.Controls.Add(this._dgReport);
            this._tpReport.Location = new System.Drawing.Point(4, 22);
            this._tpReport.Name = "_tpReport";
            this._tpReport.Padding = new System.Windows.Forms.Padding(3);
            this._tpReport.Size = new System.Drawing.Size(895, 516);
            this._tpReport.TabIndex = 1;
            this._tpReport.Text = "Report form import";
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(814, 487);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 13;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _dgReport
            // 
            this._dgReport.AllowUserToAddRows = false;
            this._dgReport.AllowUserToDeleteRows = false;
            this._dgReport.AllowUserToResizeColumns = true;
            this._dgReport.AllowUserToResizeRows = false;
            this._dgReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdCard,
            this.Number,
            this.ImportResult});
            this._dgReport.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgReport.Location = new System.Drawing.Point(6, 6);
            this._dgReport.MultiSelect = false;
            this._dgReport.Name = "_dgReport";
            this._dgReport.RowHeadersVisible = false;
            this._dgReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgReport.Size = new System.Drawing.Size(883, 475);
            this._dgReport.TabIndex = 12;
            this._dgReport.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this._dgReport_CellMouseDoubleClick);
            // 
            // IdCard
            // 
            this.IdCard.DataPropertyName = "IdCard";
            this.IdCard.HeaderText = "Id card";
            this.IdCard.Name = "IdCard";
            this.IdCard.Visible = false;
            // 
            // Number
            // 
            this.Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Number.DataPropertyName = "Number";
            this.Number.HeaderText = "Number";
            this.Number.MinimumWidth = 120;
            this.Number.Name = "Number";
            this.Number.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Number.Width = 120;
            // 
            // ImportResult
            // 
            this.ImportResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ImportResult.DataPropertyName = "ImportResult";
            this.ImportResult.HeaderText = "Import result";
            this.ImportResult.MinimumWidth = 120;
            this.ImportResult.Name = "ImportResult";
            this.ImportResult.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // CSVCardImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 566);
            this.Controls.Add(this._tcImportReport);
            this.Name = "CSVCardImportDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CSVImportDialog";
            ((System.ComponentModel.ISupportInitialize)(this._dgCSVImportColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgCSVImportValues)).EndInit();
            this._tcImportReport.ResumeLayout(false);
            this._tpImport.ResumeLayout(false);
            this._tpImport.PerformLayout();
            this._tpReport.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgReport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox _cbSeparator;
        private System.Windows.Forms.Label _lSeparator;
        private System.Windows.Forms.OpenFileDialog openFileDialogCSVImport;
        private System.Windows.Forms.TextBox _eFilePath;
        private System.Windows.Forms.Label _lFilePath;
        private System.Windows.Forms.Button _bOpenFile;
        private System.Windows.Forms.DataGridView _dgCSVImportValues;
        private System.Windows.Forms.DataGridView _dgCSVImportColumns;
        private System.Windows.Forms.Label _lTemplate;
        private System.Windows.Forms.ComboBox _cbTemplate;
        private System.Windows.Forms.Button _bImport;
        private System.Windows.Forms.ProgressBar _pbLoadFileImport;
        private System.Windows.Forms.TabControl _tcImportReport;
        private System.Windows.Forms.TabPage _tpImport;
        private System.Windows.Forms.TabPage _tpReport;
        private System.Windows.Forms.DataGridView _dgReport;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ImportRow;
        private System.Windows.Forms.DataGridViewComboBoxColumn PersonalNumber;
        private System.Windows.Forms.DataGridViewComboBoxColumn ControlCode;
        private System.Windows.Forms.DataGridViewComboBoxColumn PersonalID;
        private System.Windows.Forms.DataGridViewComboBoxColumn CardNumber;
        private System.Windows.Forms.DataGridViewComboBoxColumn PIN;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.ComboBox _cbCardSystem;
        private System.Windows.Forms.Label _lCardSystem;
        private System.Windows.Forms.CheckBox _chbCardSystem;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdCard;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImportResult;
        private System.Windows.Forms.ComboBox _cbPersonalIdSeparator;
        private System.Windows.Forms.Label _lPersonalIdSeparator;
        private System.Windows.Forms.Label _lReplacementPin;
        private System.Windows.Forms.TextBox _eReplacmentPin;
    }
}