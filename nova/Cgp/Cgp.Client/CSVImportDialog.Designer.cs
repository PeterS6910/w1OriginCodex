namespace Contal.Cgp.Client
{
    partial class CSVImportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSVImportDialog));
            this._dgCSVImportColumns = new System.Windows.Forms.DataGridView();
            this._cbSeparator = new System.Windows.Forms.ComboBox();
            this._lSeparator = new System.Windows.Forms.Label();
            this._cbDateFormat = new System.Windows.Forms.ComboBox();
            this._lDateFormat = new System.Windows.Forms.Label();
            this.openFileDialogCSVImport = new System.Windows.Forms.OpenFileDialog();
            this._eFilePath = new System.Windows.Forms.TextBox();
            this._lFilePath = new System.Windows.Forms.Label();
            this._bOpenFile = new System.Windows.Forms.Button();
            this._dgCSVImportValues = new System.Windows.Forms.DataGridView();
            this._lTemplate = new System.Windows.Forms.Label();
            this._cbTemplate = new System.Windows.Forms.ComboBox();
            this._lImportType = new System.Windows.Forms.Label();
            this._cbImportType = new System.Windows.Forms.ComboBox();
            this._bImport = new System.Windows.Forms.Button();
            this._lDepartmentFolder = new System.Windows.Forms.Label();
            this._eDepartmentFolder = new System.Windows.Forms.TextBox();
            this._pbLoadFileImport = new System.Windows.Forms.ProgressBar();
            this._cbParseBirthdateFromPersonId = new System.Windows.Forms.CheckBox();
            this._tcImportReport = new System.Windows.Forms.TabControl();
            this._tpImport = new System.Windows.Forms.TabPage();
            this._bClear = new System.Windows.Forms.Button();
            this._bSelectDeparment = new System.Windows.Forms.Button();
            this._lSubSite = new System.Windows.Forms.Label();
            this._tbmSubSite = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._gbPersonalIDValidation = new System.Windows.Forms.GroupBox();
            this._lValidationType = new System.Windows.Forms.Label();
            this._cbValidationType = new System.Windows.Forms.ComboBox();
            this._gbCompositPersonalId = new System.Windows.Forms.GroupBox();
            this._lSecondColumn = new System.Windows.Forms.Label();
            this._lFirstColumn = new System.Windows.Forms.Label();
            this._cbPersonalIdSecondColumn = new System.Windows.Forms.ComboBox();
            this._cbPersonalIdFirstColumn = new System.Windows.Forms.ComboBox();
            this._cbAllowCompositPersonalId = new System.Windows.Forms.CheckBox();
            this._gbImportSettings = new System.Windows.Forms.GroupBox();
            this._gbFilePreview = new System.Windows.Forms.GroupBox();
            this._dgPrewiev = new System.Windows.Forms.DataGridView();
            this._bUpdateTemplate = new System.Windows.Forms.Button();
            this._bCreateTemplate = new System.Windows.Forms.Button();
            this._tpReport = new System.Windows.Forms.TabPage();
            this._bEdit = new System.Windows.Forms.Button();
            this._dgReport = new System.Windows.Forms.DataGridView();
            this.IdPerson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Person = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImportResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImportRow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FirstName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.LastName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.MiddleName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Address = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PhoneNumber = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Email = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PersonalID = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EmployeeNumber = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Company = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Role = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Department = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.CostCenter = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.RelativeSuperior = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.RelativeSuperiorsPhoneNumber = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EmploymentBeginningDate = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EmploymentEndDate = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dgCSVImportColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgCSVImportValues)).BeginInit();
            this._tcImportReport.SuspendLayout();
            this._tpImport.SuspendLayout();
            this._gbPersonalIDValidation.SuspendLayout();
            this._gbCompositPersonalId.SuspendLayout();
            this._gbImportSettings.SuspendLayout();
            this._gbFilePreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgPrewiev)).BeginInit();
            this._tpReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgReport)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgCSVImportColumns
            // 
            this._dgCSVImportColumns.AllowUserToAddRows = false;
            this._dgCSVImportColumns.AllowUserToDeleteRows = false;
            this._dgCSVImportColumns.AllowUserToResizeRows = false;
            this._dgCSVImportColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgCSVImportColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgCSVImportColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ImportRow,
            this.FirstName,
            this.LastName,
            this.MiddleName,
            this.Address,
            this.Title,
            this.PhoneNumber,
            this.Email,
            this.PersonalID,
            this.EmployeeNumber,
            this.Company,
            this.Role,
            this.Department,
            this.CostCenter,
            this.RelativeSuperior,
            this.RelativeSuperiorsPhoneNumber,
            this.EmploymentBeginningDate,
            this.EmploymentEndDate});
            this._dgCSVImportColumns.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgCSVImportColumns.Location = new System.Drawing.Point(6, 19);
            this._dgCSVImportColumns.Name = "_dgCSVImportColumns";
            this._dgCSVImportColumns.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._dgCSVImportColumns.RowTemplate.Height = 24;
            this._dgCSVImportColumns.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this._dgCSVImportColumns.Size = new System.Drawing.Size(871, 60);
            this._dgCSVImportColumns.TabIndex = 17;
            this._dgCSVImportColumns.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this._dgCSVImportColumns_EditingControlShowing);
            this._dgCSVImportColumns.CurrentCellDirtyStateChanged += new System.EventHandler(this._dgCSVImportColumns_CurrentCellDirtyStateChanged);
            this._dgCSVImportColumns.Click += new System.EventHandler(this._dgCSVImportColumns_Click);
            // 
            // _cbSeparator
            // 
            this._cbSeparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSeparator.FormattingEnabled = true;
            this._cbSeparator.Location = new System.Drawing.Point(123, 10);
            this._cbSeparator.Name = "_cbSeparator";
            this._cbSeparator.Size = new System.Drawing.Size(121, 21);
            this._cbSeparator.TabIndex = 1;
            this._cbSeparator.SelectedValueChanged += new System.EventHandler(this._cbSeparator_SelectedValueChanged);
            // 
            // _lSeparator
            // 
            this._lSeparator.AutoSize = true;
            this._lSeparator.Location = new System.Drawing.Point(12, 13);
            this._lSeparator.Name = "_lSeparator";
            this._lSeparator.Size = new System.Drawing.Size(53, 13);
            this._lSeparator.TabIndex = 2;
            this._lSeparator.Text = "Separator";
            // 
            // _cbDateFormat
            // 
            this._cbDateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbDateFormat.FormattingEnabled = true;
            this._cbDateFormat.ItemHeight = 13;
            this._cbDateFormat.Location = new System.Drawing.Point(123, 90);
            this._cbDateFormat.Name = "_cbDateFormat";
            this._cbDateFormat.Size = new System.Drawing.Size(156, 21);
            this._cbDateFormat.TabIndex = 7;
            // 
            // _lDateFormat
            // 
            this._lDateFormat.AutoSize = true;
            this._lDateFormat.Location = new System.Drawing.Point(12, 93);
            this._lDateFormat.Name = "_lDateFormat";
            this._lDateFormat.Size = new System.Drawing.Size(62, 13);
            this._lDateFormat.TabIndex = 4;
            this._lDateFormat.Text = "Date format";
            // 
            // openFileDialogCSVImport
            // 
            this.openFileDialogCSVImport.Filter = "(*.csv, *.tsv, *.txt)|*.csv;*.tsv;*.txt|(*.*)|*.*";
            // 
            // _eFilePath
            // 
            this._eFilePath.Location = new System.Drawing.Point(123, 37);
            this._eFilePath.Name = "_eFilePath";
            this._eFilePath.ReadOnly = true;
            this._eFilePath.Size = new System.Drawing.Size(385, 20);
            this._eFilePath.TabIndex = 2;
            // 
            // _lFilePath
            // 
            this._lFilePath.AutoSize = true;
            this._lFilePath.Location = new System.Drawing.Point(12, 40);
            this._lFilePath.Name = "_lFilePath";
            this._lFilePath.Size = new System.Drawing.Size(47, 13);
            this._lFilePath.TabIndex = 6;
            this._lFilePath.Text = "File path";
            // 
            // _bOpenFile
            // 
            this._bOpenFile.Location = new System.Drawing.Point(514, 35);
            this._bOpenFile.Name = "_bOpenFile";
            this._bOpenFile.Size = new System.Drawing.Size(100, 23);
            this._bOpenFile.TabIndex = 3;
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
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12,
            this.Column13,
            this.Column14,
            this.Column15,
            this.Column16,
            this.Column17,
            this.Column18});
            this._dgCSVImportValues.Location = new System.Drawing.Point(6, 85);
            this._dgCSVImportValues.Name = "_dgCSVImportValues";
            this._dgCSVImportValues.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._dgCSVImportValues.Size = new System.Drawing.Size(871, 133);
            this._dgCSVImportValues.TabIndex = 18;
            this._dgCSVImportValues.Scroll += new System.Windows.Forms.ScrollEventHandler(this._dgCSVImportValues_Scroll);
            this._dgCSVImportValues.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this._dgCSVImportValues_ColumnWidthChanged);
            this._dgCSVImportValues.Click += new System.EventHandler(this._dgCSVImportValues_Click);
            // 
            // _lTemplate
            // 
            this._lTemplate.AutoSize = true;
            this._lTemplate.Location = new System.Drawing.Point(12, 66);
            this._lTemplate.Name = "_lTemplate";
            this._lTemplate.Size = new System.Drawing.Size(97, 13);
            this._lTemplate.TabIndex = 11;
            this._lTemplate.Text = "Template for import";
            // 
            // _cbTemplate
            // 
            this._cbTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTemplate.FormattingEnabled = true;
            this._cbTemplate.Location = new System.Drawing.Point(123, 63);
            this._cbTemplate.Name = "_cbTemplate";
            this._cbTemplate.Size = new System.Drawing.Size(385, 21);
            this._cbTemplate.TabIndex = 4;
            this._cbTemplate.SelectedValueChanged += new System.EventHandler(this._cbTemplate_SelectedValueChanged);
            // 
            // _lImportType
            // 
            this._lImportType.AutoSize = true;
            this._lImportType.Location = new System.Drawing.Point(304, 93);
            this._lImportType.Name = "_lImportType";
            this._lImportType.Size = new System.Drawing.Size(59, 13);
            this._lImportType.TabIndex = 13;
            this._lImportType.Text = "Import type";
            // 
            // _cbImportType
            // 
            this._cbImportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbImportType.FormattingEnabled = true;
            this._cbImportType.Location = new System.Drawing.Point(369, 90);
            this._cbImportType.Name = "_cbImportType";
            this._cbImportType.Size = new System.Drawing.Size(296, 21);
            this._cbImportType.TabIndex = 8;
            // 
            // _bImport
            // 
            this._bImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bImport.Enabled = false;
            this._bImport.Location = new System.Drawing.Point(814, 171);
            this._bImport.Name = "_bImport";
            this._bImport.Size = new System.Drawing.Size(75, 23);
            this._bImport.TabIndex = 15;
            this._bImport.Text = "Import";
            this._bImport.UseVisualStyleBackColor = true;
            this._bImport.Click += new System.EventHandler(this._bImport_Click);
            // 
            // _lDepartmentFolder
            // 
            this._lDepartmentFolder.AutoSize = true;
            this._lDepartmentFolder.Location = new System.Drawing.Point(12, 176);
            this._lDepartmentFolder.Name = "_lDepartmentFolder";
            this._lDepartmentFolder.Size = new System.Drawing.Size(91, 13);
            this._lDepartmentFolder.TabIndex = 16;
            this._lDepartmentFolder.Text = "Department folder";
            // 
            // _eDepartmentFolder
            // 
            this._eDepartmentFolder.Location = new System.Drawing.Point(123, 171);
            this._eDepartmentFolder.Name = "_eDepartmentFolder";
            this._eDepartmentFolder.Size = new System.Drawing.Size(297, 20);
            this._eDepartmentFolder.TabIndex = 14;
            // 
            // _pbLoadFileImport
            // 
            this._pbLoadFileImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pbLoadFileImport.Location = new System.Drawing.Point(525, 171);
            this._pbLoadFileImport.Name = "_pbLoadFileImport";
            this._pbLoadFileImport.Size = new System.Drawing.Size(283, 23);
            this._pbLoadFileImport.Step = 1;
            this._pbLoadFileImport.TabIndex = 17;
            // 
            // _cbParseBirthdateFromPersonId
            // 
            this._cbParseBirthdateFromPersonId.AutoSize = true;
            this._cbParseBirthdateFromPersonId.Location = new System.Drawing.Point(685, 93);
            this._cbParseBirthdateFromPersonId.Name = "_cbParseBirthdateFromPersonId";
            this._cbParseBirthdateFromPersonId.Size = new System.Drawing.Size(169, 17);
            this._cbParseBirthdateFromPersonId.TabIndex = 9;
            this._cbParseBirthdateFromPersonId.Text = "Parse birthdate from person ID";
            this._cbParseBirthdateFromPersonId.UseVisualStyleBackColor = true;
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
            this._tcImportReport.Size = new System.Drawing.Size(903, 625);
            this._tcImportReport.TabIndex = 0;
            // 
            // _tpImport
            // 
            this._tpImport.BackColor = System.Drawing.SystemColors.Control;
            this._tpImport.Controls.Add(this._bClear);
            this._tpImport.Controls.Add(this._bSelectDeparment);
            this._tpImport.Controls.Add(this._lSubSite);
            this._tpImport.Controls.Add(this._tbmSubSite);
            this._tpImport.Controls.Add(this._gbPersonalIDValidation);
            this._tpImport.Controls.Add(this._gbCompositPersonalId);
            this._tpImport.Controls.Add(this._gbImportSettings);
            this._tpImport.Controls.Add(this._gbFilePreview);
            this._tpImport.Controls.Add(this._bUpdateTemplate);
            this._tpImport.Controls.Add(this._bCreateTemplate);
            this._tpImport.Controls.Add(this._cbSeparator);
            this._tpImport.Controls.Add(this._cbParseBirthdateFromPersonId);
            this._tpImport.Controls.Add(this._pbLoadFileImport);
            this._tpImport.Controls.Add(this._lSeparator);
            this._tpImport.Controls.Add(this._lDepartmentFolder);
            this._tpImport.Controls.Add(this._cbDateFormat);
            this._tpImport.Controls.Add(this._eDepartmentFolder);
            this._tpImport.Controls.Add(this._lDateFormat);
            this._tpImport.Controls.Add(this._bImport);
            this._tpImport.Controls.Add(this._eFilePath);
            this._tpImport.Controls.Add(this._lImportType);
            this._tpImport.Controls.Add(this._lFilePath);
            this._tpImport.Controls.Add(this._cbImportType);
            this._tpImport.Controls.Add(this._bOpenFile);
            this._tpImport.Controls.Add(this._lTemplate);
            this._tpImport.Controls.Add(this._cbTemplate);
            this._tpImport.Location = new System.Drawing.Point(4, 22);
            this._tpImport.Name = "_tpImport";
            this._tpImport.Padding = new System.Windows.Forms.Padding(3);
            this._tpImport.Size = new System.Drawing.Size(895, 599);
            this._tpImport.TabIndex = 0;
            this._tpImport.Text = "Import";
            // 
            // _bClear
            // 
            this._bClear.Location = new System.Drawing.Point(458, 169);
            this._bClear.Name = "_bClear";
            this._bClear.Size = new System.Drawing.Size(60, 23);
            this._bClear.TabIndex = 30;
            this._bClear.Text = "Clear";
            this._bClear.UseVisualStyleBackColor = true;
            this._bClear.Click += new System.EventHandler(this._bClear_Click);
            // 
            // _bSelectDeparment
            // 
            this._bSelectDeparment.Location = new System.Drawing.Point(426, 169);
            this._bSelectDeparment.Name = "_bSelectDeparment";
            this._bSelectDeparment.Size = new System.Drawing.Size(26, 23);
            this._bSelectDeparment.TabIndex = 29;
            this._bSelectDeparment.Text = "...";
            this._bSelectDeparment.UseVisualStyleBackColor = true;
            this._bSelectDeparment.Click += new System.EventHandler(this._bSelectDeparment_Click);
            // 
            // _lSubSite
            // 
            this._lSubSite.AutoSize = true;
            this._lSubSite.Location = new System.Drawing.Point(12, 200);
            this._lSubSite.Name = "_lSubSite";
            this._lSubSite.Size = new System.Drawing.Size(44, 13);
            this._lSubSite.TabIndex = 28;
            this._lSubSite.Text = "SubSite";
            // 
            // _tbmSubSite
            // 
            this._tbmSubSite.AllowDrop = true;
            this._tbmSubSite.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmSubSite.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmSubSite.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmSubSite.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmSubSite.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmSubSite.Button.Image")));
            this._tbmSubSite.Button.Location = new System.Drawing.Point(370, 0);
            this._tbmSubSite.Button.Name = "_bMenu";
            this._tbmSubSite.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmSubSite.Button.TabIndex = 3;
            this._tbmSubSite.Button.UseVisualStyleBackColor = false;
            this._tbmSubSite.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmSubSite.ButtonDefaultBehaviour = true;
            this._tbmSubSite.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmSubSite.ButtonImage = null;
            // 
            // 
            // 
            this._tbmSubSite.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiRemove});
            this._tbmSubSite.ButtonPopupMenu.Name = global::Contal.Cgp.Client.Localization_Swedish.AlarmStates_NotSelected;
            this._tbmSubSite.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmSubSite.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmSubSite.ButtonShowImage = true;
            this._tbmSubSite.ButtonSizeHeight = 20;
            this._tbmSubSite.ButtonSizeWidth = 20;
            this._tbmSubSite.ButtonText = global::Contal.Cgp.Client.Localization_Swedish.GeneralOptionsForm_chbEnableggingSDPSTZChanges;
            this._tbmSubSite.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmSubSite.HoverTime = 500;
            // 
            // 
            // 
            this._tbmSubSite.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmSubSite.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._tbmSubSite.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmSubSite.ImageTextBox.ContextMenuStrip = this._tbmSubSite.ButtonPopupMenu;
            this._tbmSubSite.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmSubSite.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmSubSite.ImageTextBox.Image")));
            this._tbmSubSite.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmSubSite.ImageTextBox.Name = "_textBox";
            this._tbmSubSite.ImageTextBox.NoTextNoImage = true;
            this._tbmSubSite.ImageTextBox.ReadOnly = true;
            this._tbmSubSite.ImageTextBox.Size = new System.Drawing.Size(370, 20);
            this._tbmSubSite.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmSubSite.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmSubSite.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmSubSite.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmSubSite.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmSubSite.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmSubSite.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmSubSite.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmSubSite.ImageTextBox.TextBox.Size = new System.Drawing.Size(368, 13);
            this._tbmSubSite.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmSubSite.ImageTextBox.UseImage = true;
            this._tbmSubSite.Location = new System.Drawing.Point(123, 197);
            this._tbmSubSite.MaximumSize = new System.Drawing.Size(1200, 20);
            this._tbmSubSite.MinimumSize = new System.Drawing.Size(21, 20);
            this._tbmSubSite.Name = "_tbmSubSite";
            this._tbmSubSite.Size = new System.Drawing.Size(390, 20);
            this._tbmSubSite.TabIndex = 27;
            this._tbmSubSite.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmSubSite.TextImage")));
            this._tbmSubSite.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmSubSite_ButtonPopupMenuItemClick);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(117, 22);
            this._tsiModify.Text = "Modify";
            // 
            // _tsiRemove
            // 
            this._tsiRemove.Name = "_tsiRemove";
            this._tsiRemove.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove.Text = "Remove";
            // 
            // _gbPersonalIDValidation
            // 
            this._gbPersonalIDValidation.Controls.Add(this._lValidationType);
            this._gbPersonalIDValidation.Controls.Add(this._cbValidationType);
            this._gbPersonalIDValidation.Location = new System.Drawing.Point(620, 117);
            this._gbPersonalIDValidation.Name = "_gbPersonalIDValidation";
            this._gbPersonalIDValidation.Size = new System.Drawing.Size(269, 50);
            this._gbPersonalIDValidation.TabIndex = 26;
            this._gbPersonalIDValidation.TabStop = false;
            this._gbPersonalIDValidation.Text = "Personal ID validation";
            // 
            // _lValidationType
            // 
            this._lValidationType.AutoSize = true;
            this._lValidationType.Location = new System.Drawing.Point(6, 22);
            this._lValidationType.Name = "_lValidationType";
            this._lValidationType.Size = new System.Drawing.Size(76, 13);
            this._lValidationType.TabIndex = 27;
            this._lValidationType.Text = "Validation type";
            // 
            // _cbValidationType
            // 
            this._cbValidationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbValidationType.FormattingEnabled = true;
            this._cbValidationType.Location = new System.Drawing.Point(88, 19);
            this._cbValidationType.Name = "_cbValidationType";
            this._cbValidationType.Size = new System.Drawing.Size(175, 21);
            this._cbValidationType.TabIndex = 13;
            this._cbValidationType.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectedIndexChanged);
            // 
            // _gbCompositPersonalId
            // 
            this._gbCompositPersonalId.Controls.Add(this._lSecondColumn);
            this._gbCompositPersonalId.Controls.Add(this._lFirstColumn);
            this._gbCompositPersonalId.Controls.Add(this._cbPersonalIdSecondColumn);
            this._gbCompositPersonalId.Controls.Add(this._cbPersonalIdFirstColumn);
            this._gbCompositPersonalId.Controls.Add(this._cbAllowCompositPersonalId);
            this._gbCompositPersonalId.Location = new System.Drawing.Point(9, 117);
            this._gbCompositPersonalId.Name = "_gbCompositPersonalId";
            this._gbCompositPersonalId.Size = new System.Drawing.Size(605, 50);
            this._gbCompositPersonalId.TabIndex = 23;
            this._gbCompositPersonalId.TabStop = false;
            this._gbCompositPersonalId.Text = "Composit personal ID";
            // 
            // _lSecondColumn
            // 
            this._lSecondColumn.AutoSize = true;
            this._lSecondColumn.Location = new System.Drawing.Point(385, 22);
            this._lSecondColumn.Name = "_lSecondColumn";
            this._lSecondColumn.Size = new System.Drawing.Size(81, 13);
            this._lSecondColumn.TabIndex = 4;
            this._lSecondColumn.Text = "Second column";
            // 
            // _lFirstColumn
            // 
            this._lFirstColumn.AutoSize = true;
            this._lFirstColumn.Location = new System.Drawing.Point(180, 22);
            this._lFirstColumn.Name = "_lFirstColumn";
            this._lFirstColumn.Size = new System.Drawing.Size(63, 13);
            this._lFirstColumn.TabIndex = 3;
            this._lFirstColumn.Text = "First column";
            // 
            // _cbPersonalIdSecondColumn
            // 
            this._cbPersonalIdSecondColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPersonalIdSecondColumn.Enabled = false;
            this._cbPersonalIdSecondColumn.FormattingEnabled = true;
            this._cbPersonalIdSecondColumn.Location = new System.Drawing.Point(469, 19);
            this._cbPersonalIdSecondColumn.Name = "_cbPersonalIdSecondColumn";
            this._cbPersonalIdSecondColumn.Size = new System.Drawing.Size(130, 21);
            this._cbPersonalIdSecondColumn.TabIndex = 12;
            this._cbPersonalIdSecondColumn.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectedIndexChanged);
            // 
            // _cbPersonalIdFirstColumn
            // 
            this._cbPersonalIdFirstColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPersonalIdFirstColumn.Enabled = false;
            this._cbPersonalIdFirstColumn.FormattingEnabled = true;
            this._cbPersonalIdFirstColumn.Location = new System.Drawing.Point(249, 19);
            this._cbPersonalIdFirstColumn.Name = "_cbPersonalIdFirstColumn";
            this._cbPersonalIdFirstColumn.Size = new System.Drawing.Size(130, 21);
            this._cbPersonalIdFirstColumn.TabIndex = 11;
            this._cbPersonalIdFirstColumn.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SelectedIndexChanged);
            // 
            // _cbAllowCompositPersonalId
            // 
            this._cbAllowCompositPersonalId.AutoSize = true;
            this._cbAllowCompositPersonalId.Location = new System.Drawing.Point(6, 21);
            this._cbAllowCompositPersonalId.Name = "_cbAllowCompositPersonalId";
            this._cbAllowCompositPersonalId.Size = new System.Drawing.Size(153, 17);
            this._cbAllowCompositPersonalId.TabIndex = 10;
            this._cbAllowCompositPersonalId.Text = "Allow composit personal ID";
            this._cbAllowCompositPersonalId.UseVisualStyleBackColor = true;
            this._cbAllowCompositPersonalId.CheckedChanged += new System.EventHandler(this._cbAllowComposidPersonalId_CheckedChanged);
            // 
            // _gbImportSettings
            // 
            this._gbImportSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbImportSettings.Controls.Add(this._dgCSVImportValues);
            this._gbImportSettings.Controls.Add(this._dgCSVImportColumns);
            this._gbImportSettings.Location = new System.Drawing.Point(9, 369);
            this._gbImportSettings.Name = "_gbImportSettings";
            this._gbImportSettings.Size = new System.Drawing.Size(883, 224);
            this._gbImportSettings.TabIndex = 22;
            this._gbImportSettings.TabStop = false;
            this._gbImportSettings.Text = "Import settings";
            // 
            // _gbFilePreview
            // 
            this._gbFilePreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbFilePreview.Controls.Add(this._dgPrewiev);
            this._gbFilePreview.Location = new System.Drawing.Point(9, 223);
            this._gbFilePreview.Name = "_gbFilePreview";
            this._gbFilePreview.Size = new System.Drawing.Size(883, 140);
            this._gbFilePreview.TabIndex = 21;
            this._gbFilePreview.TabStop = false;
            this._gbFilePreview.Text = "File prewiev";
            // 
            // _dgPrewiev
            // 
            this._dgPrewiev.AllowUserToAddRows = false;
            this._dgPrewiev.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgPrewiev.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this._dgPrewiev.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgPrewiev.Location = new System.Drawing.Point(6, 19);
            this._dgPrewiev.Name = "_dgPrewiev";
            this._dgPrewiev.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._dgPrewiev.Size = new System.Drawing.Size(871, 115);
            this._dgPrewiev.TabIndex = 16;
            // 
            // _bUpdateTemplate
            // 
            this._bUpdateTemplate.Location = new System.Drawing.Point(670, 61);
            this._bUpdateTemplate.Name = "_bUpdateTemplate";
            this._bUpdateTemplate.Size = new System.Drawing.Size(150, 23);
            this._bUpdateTemplate.TabIndex = 6;
            this._bUpdateTemplate.Text = "Update actual template";
            this._bUpdateTemplate.UseVisualStyleBackColor = true;
            this._bUpdateTemplate.Click += new System.EventHandler(this._bUpdateTemplate_Click);
            // 
            // _bCreateTemplate
            // 
            this._bCreateTemplate.Location = new System.Drawing.Point(514, 61);
            this._bCreateTemplate.Name = "_bCreateTemplate";
            this._bCreateTemplate.Size = new System.Drawing.Size(150, 23);
            this._bCreateTemplate.TabIndex = 5;
            this._bCreateTemplate.Text = "Create new template";
            this._bCreateTemplate.UseVisualStyleBackColor = true;
            this._bCreateTemplate.Click += new System.EventHandler(this._bCreateTemplate_Click);
            // 
            // _tpReport
            // 
            this._tpReport.BackColor = System.Drawing.SystemColors.Control;
            this._tpReport.Controls.Add(this._bEdit);
            this._tpReport.Controls.Add(this._dgReport);
            this._tpReport.Location = new System.Drawing.Point(4, 22);
            this._tpReport.Name = "_tpReport";
            this._tpReport.Padding = new System.Windows.Forms.Padding(3);
            this._tpReport.Size = new System.Drawing.Size(895, 599);
            this._tpReport.TabIndex = 1;
            this._tpReport.Text = "Report form import";
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(814, 570);
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
            this._dgReport.AllowUserToResizeRows = false;
            this._dgReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdPerson,
            this.Person,
            this.ImportResult});
            this._dgReport.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgReport.Location = new System.Drawing.Point(6, 6);
            this._dgReport.MultiSelect = false;
            this._dgReport.Name = "_dgReport";
            this._dgReport.RowHeadersVisible = false;
            this._dgReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgReport.Size = new System.Drawing.Size(883, 558);
            this._dgReport.TabIndex = 12;
            this._dgReport.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this._dgReport_CellMouseDoubleClick);
            // 
            // IdPerson
            // 
            this.IdPerson.DataPropertyName = "IdPerson";
            this.IdPerson.HeaderText = "Id person";
            this.IdPerson.Name = "IdPerson";
            this.IdPerson.Visible = false;
            // 
            // Person
            // 
            this.Person.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Person.DataPropertyName = "FullName";
            this.Person.HeaderText = "Person";
            this.Person.MinimumWidth = 120;
            this.Person.Name = "Person";
            this.Person.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Person.Width = 120;
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
            // ImportRow
            // 
            this.ImportRow.HeaderText = "Import row";
            this.ImportRow.MinimumWidth = 120;
            this.ImportRow.Name = "ImportRow";
            this.ImportRow.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ImportRow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ImportRow.Width = 120;
            // 
            // FirstName
            // 
            this.FirstName.HeaderText = "First name";
            this.FirstName.MinimumWidth = 120;
            this.FirstName.Name = "FirstName";
            this.FirstName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FirstName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.FirstName.Width = 120;
            // 
            // LastName
            // 
            this.LastName.HeaderText = "Last name";
            this.LastName.MinimumWidth = 120;
            this.LastName.Name = "LastName";
            this.LastName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.LastName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.LastName.Width = 120;
            // 
            // MiddleName
            // 
            this.MiddleName.HeaderText = "Middle name";
            this.MiddleName.MinimumWidth = 120;
            this.MiddleName.Name = "MiddleName";
            this.MiddleName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MiddleName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.MiddleName.Width = 120;
            // 
            // Address
            // 
            this.Address.HeaderText = "Address";
            this.Address.MinimumWidth = 120;
            this.Address.Name = "Address";
            this.Address.Width = 120;
            // 
            // Title
            // 
            this.Title.HeaderText = "Title";
            this.Title.MinimumWidth = 120;
            this.Title.Name = "Title";
            this.Title.Width = 120;
            // 
            // PhoneNumber
            // 
            this.PhoneNumber.HeaderText = "Phone number";
            this.PhoneNumber.MinimumWidth = 120;
            this.PhoneNumber.Name = "PhoneNumber";
            this.PhoneNumber.Width = 120;
            // 
            // Email
            // 
            this.Email.HeaderText = "Email";
            this.Email.MinimumWidth = 120;
            this.Email.Name = "Email";
            this.Email.Width = 120;
            // 
            // PersonalID
            // 
            this.PersonalID.HeaderText = "Presonal ID";
            this.PersonalID.MinimumWidth = 120;
            this.PersonalID.Name = "PersonalID";
            this.PersonalID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PersonalID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.PersonalID.Width = 120;
            // 
            // EmployeeNumber
            // 
            this.EmployeeNumber.HeaderText = "Employee number";
            this.EmployeeNumber.MinimumWidth = 120;
            this.EmployeeNumber.Name = "EmployeeNumber";
            this.EmployeeNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EmployeeNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.EmployeeNumber.Width = 120;
            // 
            // Company
            // 
            this.Company.HeaderText = "Company";
            this.Company.MinimumWidth = 120;
            this.Company.Name = "Company";
            this.Company.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Company.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Company.Width = 120;
            // 
            // Role
            // 
            this.Role.HeaderText = "Role";
            this.Role.MinimumWidth = 120;
            this.Role.Name = "Role";
            this.Role.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Role.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Role.Width = 120;
            // 
            // Department
            // 
            this.Department.HeaderText = "Department";
            this.Department.MinimumWidth = 120;
            this.Department.Name = "Department";
            this.Department.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Department.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Department.Width = 120;
            // 
            // CostCenter
            // 
            this.CostCenter.HeaderText = "Cost center";
            this.CostCenter.MinimumWidth = 120;
            this.CostCenter.Name = "CostCenter";
            this.CostCenter.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CostCenter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CostCenter.Width = 120;
            // 
            // RelativeSuperior
            // 
            this.RelativeSuperior.HeaderText = "Relative/superior";
            this.RelativeSuperior.MinimumWidth = 120;
            this.RelativeSuperior.Name = "RelativeSuperior";
            this.RelativeSuperior.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RelativeSuperior.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.RelativeSuperior.Width = 120;
            // 
            // RelativeSuperiorsPhoneNumber
            // 
            this.RelativeSuperiorsPhoneNumber.HeaderText = "Relative/superior\'s phone number";
            this.RelativeSuperiorsPhoneNumber.MinimumWidth = 120;
            this.RelativeSuperiorsPhoneNumber.Name = "RelativeSuperiorsPhoneNumber";
            this.RelativeSuperiorsPhoneNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RelativeSuperiorsPhoneNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.RelativeSuperiorsPhoneNumber.Width = 120;
            // 
            // EmploymentBeginningDate
            // 
            this.EmploymentBeginningDate.HeaderText = "Employment beginning date";
            this.EmploymentBeginningDate.MinimumWidth = 120;
            this.EmploymentBeginningDate.Name = "EmploymentBeginningDate";
            this.EmploymentBeginningDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EmploymentBeginningDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.EmploymentBeginningDate.Width = 120;
            // 
            // EmploymentEndDate
            // 
            this.EmploymentEndDate.HeaderText = "Employment end date";
            this.EmploymentEndDate.MinimumWidth = 120;
            this.EmploymentEndDate.Name = "EmploymentEndDate";
            this.EmploymentEndDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EmploymentEndDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.EmploymentEndDate.Width = 120;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.DataPropertyName = "Column1";
            this.dataGridViewCheckBoxColumn1.HeaderText = "Import row";
            this.dataGridViewCheckBoxColumn1.MinimumWidth = 120;
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.Width = 120;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Column2";
            this.Column2.HeaderText = "First name";
            this.Column2.MinimumWidth = 120;
            this.Column2.Name = "Column2";
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column2.Width = 120;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "Column3";
            this.Column3.HeaderText = "Last name";
            this.Column3.MinimumWidth = 120;
            this.Column3.Name = "Column3";
            this.Column3.Width = 120;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "Column4";
            this.Column4.HeaderText = "Middle name";
            this.Column4.MinimumWidth = 120;
            this.Column4.Name = "Column4";
            this.Column4.Width = 120;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "Column5";
            this.Column5.HeaderText = "Address";
            this.Column5.MinimumWidth = 120;
            this.Column5.Name = "Column5";
            this.Column5.Width = 120;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "Column6";
            this.Column6.HeaderText = "Title";
            this.Column6.MinimumWidth = 120;
            this.Column6.Name = "Column6";
            this.Column6.Width = 120;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "Column7";
            this.Column7.HeaderText = "Phone number";
            this.Column7.MinimumWidth = 120;
            this.Column7.Name = "Column7";
            this.Column7.Width = 120;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "Column8";
            this.Column8.HeaderText = "Email";
            this.Column8.MinimumWidth = 120;
            this.Column8.Name = "Column8";
            this.Column8.Width = 120;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "Column9";
            this.Column9.HeaderText = "Presonal ID";
            this.Column9.MinimumWidth = 120;
            this.Column9.Name = "Column9";
            this.Column9.Width = 120;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "Column10";
            this.Column10.HeaderText = "Employee number";
            this.Column10.MinimumWidth = 120;
            this.Column10.Name = "Column10";
            this.Column10.Width = 120;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "Column11";
            this.Column11.HeaderText = "Company";
            this.Column11.MinimumWidth = 120;
            this.Column11.Name = "Column11";
            this.Column11.Width = 120;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "Column12";
            this.Column12.HeaderText = "Role";
            this.Column12.MinimumWidth = 120;
            this.Column12.Name = "Column12";
            this.Column12.Width = 120;
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "Column13";
            this.Column13.HeaderText = "Department";
            this.Column13.MinimumWidth = 120;
            this.Column13.Name = "Column13";
            this.Column13.Width = 120;
            // 
            // Column14
            // 
            this.Column14.DataPropertyName = "Column14";
            this.Column14.HeaderText = "Cost center";
            this.Column14.MinimumWidth = 120;
            this.Column14.Name = "Column14";
            this.Column14.Width = 120;
            // 
            // Column15
            // 
            this.Column15.DataPropertyName = "Column15";
            this.Column15.HeaderText = "Relative/superior";
            this.Column15.MinimumWidth = 120;
            this.Column15.Name = "Column15";
            this.Column15.Width = 120;
            // 
            // Column16
            // 
            this.Column16.DataPropertyName = "Column16";
            this.Column16.HeaderText = "Relative/superior\'s phone number";
            this.Column16.MinimumWidth = 120;
            this.Column16.Name = "Column16";
            this.Column16.Width = 120;
            // 
            // Column17
            // 
            this.Column17.DataPropertyName = "Column17";
            this.Column17.HeaderText = "Employment beginning date";
            this.Column17.MinimumWidth = 120;
            this.Column17.Name = "Column17";
            this.Column17.Width = 120;
            // 
            // Column18
            // 
            this.Column18.DataPropertyName = "Column18";
            this.Column18.HeaderText = "Employment end date";
            this.Column18.MinimumWidth = 120;
            this.Column18.Name = "Column18";
            this.Column18.Width = 120;
            // 
            // CSVImportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 649);
            this.Controls.Add(this._tcImportReport);
            this.Name = "CSVImportDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CSVImportDialog";
            this.Shown += new System.EventHandler(this.CSVImportDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this._dgCSVImportColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgCSVImportValues)).EndInit();
            this._tcImportReport.ResumeLayout(false);
            this._tpImport.ResumeLayout(false);
            this._tpImport.PerformLayout();
            this._gbPersonalIDValidation.ResumeLayout(false);
            this._gbPersonalIDValidation.PerformLayout();
            this._gbCompositPersonalId.ResumeLayout(false);
            this._gbCompositPersonalId.PerformLayout();
            this._gbImportSettings.ResumeLayout(false);
            this._gbFilePreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgPrewiev)).EndInit();
            this._tpReport.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgReport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox _cbSeparator;
        private System.Windows.Forms.Label _lSeparator;
        private System.Windows.Forms.ComboBox _cbDateFormat;
        private System.Windows.Forms.Label _lDateFormat;
        private System.Windows.Forms.OpenFileDialog openFileDialogCSVImport;
        private System.Windows.Forms.TextBox _eFilePath;
        private System.Windows.Forms.Label _lFilePath;
        private System.Windows.Forms.Button _bOpenFile;
        private System.Windows.Forms.DataGridView _dgCSVImportValues;
        private System.Windows.Forms.DataGridView _dgCSVImportColumns;
        private System.Windows.Forms.Label _lTemplate;
        private System.Windows.Forms.ComboBox _cbTemplate;
        private System.Windows.Forms.Label _lImportType;
        private System.Windows.Forms.ComboBox _cbImportType;
        private System.Windows.Forms.Button _bImport;
        private System.Windows.Forms.Label _lDepartmentFolder;
        private System.Windows.Forms.TextBox _eDepartmentFolder;
        private System.Windows.Forms.ProgressBar _pbLoadFileImport;
        private System.Windows.Forms.CheckBox _cbParseBirthdateFromPersonId;
        private System.Windows.Forms.TabControl _tcImportReport;
        private System.Windows.Forms.TabPage _tpImport;
        private System.Windows.Forms.TabPage _tpReport;
        private System.Windows.Forms.DataGridView _dgReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdPerson;
        private System.Windows.Forms.DataGridViewTextBoxColumn Person;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImportResult;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bCreateTemplate;
        private System.Windows.Forms.Button _bUpdateTemplate;
        private System.Windows.Forms.GroupBox _gbFilePreview;
        private System.Windows.Forms.DataGridView _dgPrewiev;
        private System.Windows.Forms.GroupBox _gbImportSettings;
        private System.Windows.Forms.GroupBox _gbCompositPersonalId;
        private System.Windows.Forms.Label _lSecondColumn;
        private System.Windows.Forms.Label _lFirstColumn;
        private System.Windows.Forms.ComboBox _cbPersonalIdSecondColumn;
        private System.Windows.Forms.ComboBox _cbPersonalIdFirstColumn;
        private System.Windows.Forms.CheckBox _cbAllowCompositPersonalId;
        private System.Windows.Forms.GroupBox _gbPersonalIDValidation;
        private System.Windows.Forms.Label _lValidationType;
        private System.Windows.Forms.ComboBox _cbValidationType;
        private System.Windows.Forms.Label _lSubSite;
        private Contal.IwQuick.UI.TextBoxMenu _tbmSubSite;
        private System.Windows.Forms.Button _bSelectDeparment;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
        private System.Windows.Forms.Button _bClear;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ImportRow;
        private System.Windows.Forms.DataGridViewComboBoxColumn FirstName;
        private System.Windows.Forms.DataGridViewComboBoxColumn LastName;
        private System.Windows.Forms.DataGridViewComboBoxColumn MiddleName;
        private System.Windows.Forms.DataGridViewComboBoxColumn Address;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title;
        private System.Windows.Forms.DataGridViewComboBoxColumn PhoneNumber;
        private System.Windows.Forms.DataGridViewComboBoxColumn Email;
        private System.Windows.Forms.DataGridViewComboBoxColumn PersonalID;
        private System.Windows.Forms.DataGridViewComboBoxColumn EmployeeNumber;
        private System.Windows.Forms.DataGridViewComboBoxColumn Company;
        private System.Windows.Forms.DataGridViewComboBoxColumn Role;
        private System.Windows.Forms.DataGridViewComboBoxColumn Department;
        private System.Windows.Forms.DataGridViewComboBoxColumn CostCenter;
        private System.Windows.Forms.DataGridViewComboBoxColumn RelativeSuperior;
        private System.Windows.Forms.DataGridViewComboBoxColumn RelativeSuperiorsPhoneNumber;
        private System.Windows.Forms.DataGridViewComboBoxColumn EmploymentBeginningDate;
        private System.Windows.Forms.DataGridViewComboBoxColumn EmploymentEndDate;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column18;
    }
}