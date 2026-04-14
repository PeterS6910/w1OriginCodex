namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    partial class LocalizationAssistentMainForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalizationAssistentMainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this._rbTransMode = new System.Windows.Forms.RadioButton();
            this._rbDevMod = new System.Windows.Forms.RadioButton();
            this._tcMain = new System.Windows.Forms.TabControl();
            this._tpTransMode = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._lbMasterLocalization = new System.Windows.Forms.ListBox();
            this._labMasterLang = new System.Windows.Forms.Label();
            this._cbNonMasterLangs = new System.Windows.Forms.ComboBox();
            this._lbOtherLocalization = new System.Windows.Forms.ListBox();
            this._labTranslationReport = new System.Windows.Forms.Label();
            this._edTranslationItem = new System.Windows.Forms.TextBox();
            this._labDescriptionItem = new System.Windows.Forms.Label();
            this._labMasterItem = new System.Windows.Forms.Label();
            this._tpDevelopMode = new System.Windows.Forms.TabPage();
            this._btInsertNewRow = new System.Windows.Forms.Button();
            this._btDeleteTransItem = new System.Windows.Forms.Button();
            this._dgwLocalization = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this._cbLocalizations = new System.Windows.Forms.ComboBox();
            this._btDisDeveloperMode = new System.Windows.Forms.Button();
            this._sdSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this._ofdOpen = new System.Windows.Forms.OpenFileDialog();
            this._btReloadPackage = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._btNewLoc = new System.Windows.Forms.ToolStripMenuItem();
            this._btSaveLocData = new System.Windows.Forms.ToolStripMenuItem();
            this._btSaveLocalizationDataAs = new System.Windows.Forms.ToolStripMenuItem();
            this._btLoadLocData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._btSynchAllSym = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._btSavePackage = new System.Windows.Forms.ToolStripMenuItem();
            this._btSavePackageAs = new System.Windows.Forms.ToolStripMenuItem();
            this._btLOadPackage = new System.Windows.Forms.ToolStripMenuItem();
            this._btClosePackage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._btExit = new System.Windows.Forms.ToolStripMenuItem();
            this._btGenerate = new System.Windows.Forms.ToolStripMenuItem();
            this._btCEnumerationFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this._btGenAll = new System.Windows.Forms.ToolStripMenuItem();
            this._btDefGenPaths = new System.Windows.Forms.ToolStripMenuItem();
            this._tcMain.SuspendLayout();
            this._tpTransMode.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this._tpDevelopMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgwLocalization)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.statusStrip1.Location = new System.Drawing.Point(0, 567);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(1038, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            // 
            // _rbTransMode
            // 
            this._rbTransMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbTransMode.AutoSize = true;
            this._rbTransMode.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this._rbTransMode.Checked = true;
            this._rbTransMode.Location = new System.Drawing.Point(10, 571);
            this._rbTransMode.Name = "_rbTransMode";
            this._rbTransMode.Size = new System.Drawing.Size(101, 17);
            this._rbTransMode.TabIndex = 2;
            this._rbTransMode.TabStop = true;
            this._rbTransMode.Text = "Translator mode";
            this._rbTransMode.UseVisualStyleBackColor = false;
            this._rbTransMode.CheckedChanged += new System.EventHandler(this._rbTransMode_CheckedChanged);
            // 
            // _rbDevMod
            // 
            this._rbDevMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbDevMod.AutoSize = true;
            this._rbDevMod.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this._rbDevMod.Location = new System.Drawing.Point(117, 571);
            this._rbDevMod.Name = "_rbDevMod";
            this._rbDevMod.Size = new System.Drawing.Size(103, 17);
            this._rbDevMod.TabIndex = 3;
            this._rbDevMod.Text = "Developer mode";
            this._rbDevMod.UseVisualStyleBackColor = false;
            this._rbDevMod.CheckedChanged += new System.EventHandler(this._rbDevMod_CheckedChanged);
            // 
            // _tcMain
            // 
            this._tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this._tcMain.Controls.Add(this._tpTransMode);
            this._tcMain.Controls.Add(this._tpDevelopMode);
            this._tcMain.ItemSize = new System.Drawing.Size(0, 1);
            this._tcMain.Location = new System.Drawing.Point(0, 24);
            this._tcMain.Name = "_tcMain";
            this._tcMain.SelectedIndex = 0;
            this._tcMain.Size = new System.Drawing.Size(1041, 545);
            this._tcMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this._tcMain.TabIndex = 4;
            this._tcMain.SelectedIndexChanged += new System.EventHandler(this._tcMain_SelectedIndexChanged);
            // 
            // _tpTransMode
            // 
            this._tpTransMode.Controls.Add(this.splitContainer1);
            this._tpTransMode.Controls.Add(this._labTranslationReport);
            this._tpTransMode.Controls.Add(this._edTranslationItem);
            this._tpTransMode.Controls.Add(this._labDescriptionItem);
            this._tpTransMode.Controls.Add(this._labMasterItem);
            this._tpTransMode.Location = new System.Drawing.Point(4, 24);
            this._tpTransMode.Name = "_tpTransMode";
            this._tpTransMode.Padding = new System.Windows.Forms.Padding(3);
            this._tpTransMode.Size = new System.Drawing.Size(1033, 517);
            this._tpTransMode.TabIndex = 0;
            this._tpTransMode.Text = "Translate mode";
            this._tpTransMode.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._lbMasterLocalization);
            this.splitContainer1.Panel1.Controls.Add(this._labMasterLang);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._cbNonMasterLangs);
            this.splitContainer1.Panel2.Controls.Add(this._lbOtherLocalization);
            this.splitContainer1.Size = new System.Drawing.Size(1027, 355);
            this.splitContainer1.SplitterDistance = 720;
            this.splitContainer1.TabIndex = 15;
            // 
            // _lbMasterLocalization
            // 
            this._lbMasterLocalization.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbMasterLocalization.FormattingEnabled = true;
            this._lbMasterLocalization.Location = new System.Drawing.Point(0, 29);
            this._lbMasterLocalization.Name = "_lbMasterLocalization";
            this._lbMasterLocalization.Size = new System.Drawing.Size(720, 316);
            this._lbMasterLocalization.Sorted = true;
            this._lbMasterLocalization.TabIndex = 9;
            this._lbMasterLocalization.SelectedIndexChanged += new System.EventHandler(this._lbMasterLocalization_SelectedIndexChanged);
            // 
            // _labMasterLang
            // 
            this._labMasterLang.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._labMasterLang.Dock = System.Windows.Forms.DockStyle.Top;
            this._labMasterLang.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labMasterLang.Location = new System.Drawing.Point(0, 0);
            this._labMasterLang.Name = "_labMasterLang";
            this._labMasterLang.Size = new System.Drawing.Size(720, 26);
            this._labMasterLang.TabIndex = 7;
            this._labMasterLang.Text = "Master language name";
            this._labMasterLang.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _cbNonMasterLangs
            // 
            this._cbNonMasterLangs.Dock = System.Windows.Forms.DockStyle.Top;
            this._cbNonMasterLangs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbNonMasterLangs.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cbNonMasterLangs.FormattingEnabled = true;
            this._cbNonMasterLangs.Location = new System.Drawing.Point(0, 0);
            this._cbNonMasterLangs.Name = "_cbNonMasterLangs";
            this._cbNonMasterLangs.Size = new System.Drawing.Size(303, 26);
            this._cbNonMasterLangs.TabIndex = 8;
            this._cbNonMasterLangs.SelectedIndexChanged += new System.EventHandler(this._cbNonMasterLangs_SelectedIndexChanged);
            // 
            // _lbOtherLocalization
            // 
            this._lbOtherLocalization.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbOtherLocalization.FormattingEnabled = true;
            this._lbOtherLocalization.Location = new System.Drawing.Point(0, 29);
            this._lbOtherLocalization.Name = "_lbOtherLocalization";
            this._lbOtherLocalization.Size = new System.Drawing.Size(303, 316);
            this._lbOtherLocalization.TabIndex = 10;
            this._lbOtherLocalization.SelectedIndexChanged += new System.EventHandler(this._lbOtherLocalization_SelectedIndexChanged);
            // 
            // _labTranslationReport
            // 
            this._labTranslationReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._labTranslationReport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._labTranslationReport.Font = new System.Drawing.Font("Arial", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labTranslationReport.Location = new System.Drawing.Point(3, 455);
            this._labTranslationReport.Name = "_labTranslationReport";
            this._labTranslationReport.Size = new System.Drawing.Size(1027, 59);
            this._labTranslationReport.TabIndex = 14;
            // 
            // _edTranslationItem
            // 
            this._edTranslationItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._edTranslationItem.Font = new System.Drawing.Font("Arial", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._edTranslationItem.Location = new System.Drawing.Point(3, 426);
            this._edTranslationItem.Name = "_edTranslationItem";
            this._edTranslationItem.Size = new System.Drawing.Size(1027, 26);
            this._edTranslationItem.TabIndex = 13;
            this._edTranslationItem.TextChanged += new System.EventHandler(this._edTranslationItem_TextChanged);
            this._edTranslationItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this._edTranslationItem_KeyDown);
            this._edTranslationItem.KeyUp += new System.Windows.Forms.KeyEventHandler(this._edTranslationItem_KeyUp);
            // 
            // _labDescriptionItem
            // 
            this._labDescriptionItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._labDescriptionItem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._labDescriptionItem.Location = new System.Drawing.Point(3, 358);
            this._labDescriptionItem.Name = "_labDescriptionItem";
            this._labDescriptionItem.Size = new System.Drawing.Size(1027, 34);
            this._labDescriptionItem.TabIndex = 12;
            // 
            // _labMasterItem
            // 
            this._labMasterItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._labMasterItem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._labMasterItem.Location = new System.Drawing.Point(3, 392);
            this._labMasterItem.Name = "_labMasterItem";
            this._labMasterItem.Size = new System.Drawing.Size(1027, 31);
            this._labMasterItem.TabIndex = 11;
            // 
            // _tpDevelopMode
            // 
            this._tpDevelopMode.Controls.Add(this._btInsertNewRow);
            this._tpDevelopMode.Controls.Add(this._btDeleteTransItem);
            this._tpDevelopMode.Controls.Add(this._dgwLocalization);
            this._tpDevelopMode.Controls.Add(this.label1);
            this._tpDevelopMode.Controls.Add(this._cbLocalizations);
            this._tpDevelopMode.Location = new System.Drawing.Point(4, 5);
            this._tpDevelopMode.Name = "_tpDevelopMode";
            this._tpDevelopMode.Padding = new System.Windows.Forms.Padding(3);
            this._tpDevelopMode.Size = new System.Drawing.Size(1033, 536);
            this._tpDevelopMode.TabIndex = 1;
            this._tpDevelopMode.Text = "Developer mode";
            this._tpDevelopMode.UseVisualStyleBackColor = true;
            // 
            // _btInsertNewRow
            // 
            this._btInsertNewRow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btInsertNewRow.Location = new System.Drawing.Point(278, 7);
            this._btInsertNewRow.Name = "_btInsertNewRow";
            this._btInsertNewRow.Size = new System.Drawing.Size(135, 21);
            this._btInsertNewRow.TabIndex = 9;
            this._btInsertNewRow.Text = "Insert new row (Ctrl+I)";
            this._btInsertNewRow.UseVisualStyleBackColor = true;
            this._btInsertNewRow.Click += new System.EventHandler(this._btInsertNewRow_Click);
            // 
            // _btDeleteTransItem
            // 
            this._btDeleteTransItem.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btDeleteTransItem.Location = new System.Drawing.Point(418, 6);
            this._btDeleteTransItem.Name = "_btDeleteTransItem";
            this._btDeleteTransItem.Size = new System.Drawing.Size(97, 21);
            this._btDeleteTransItem.TabIndex = 5;
            this._btDeleteTransItem.Text = "Delete (Ctrl+D)";
            this._btDeleteTransItem.UseVisualStyleBackColor = true;
            this._btDeleteTransItem.Click += new System.EventHandler(this._btDeleteTransItem_Click);
            // 
            // _dgwLocalization
            // 
            this._dgwLocalization.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgwLocalization.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgwLocalization.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgwLocalization.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._dgwLocalization.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgwLocalization.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dgwLocalization.DefaultCellStyle = dataGridViewCellStyle2;
            this._dgwLocalization.Location = new System.Drawing.Point(3, 33);
            this._dgwLocalization.MultiSelect = false;
            this._dgwLocalization.Name = "_dgwLocalization";
            this._dgwLocalization.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgwLocalization.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this._dgwLocalization.Size = new System.Drawing.Size(1027, 500);
            this._dgwLocalization.TabIndex = 4;
            this._dgwLocalization.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this._dgwLocalization_CellBeginEdit);
            this._dgwLocalization.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this._dgwLocalization_CellValidating);
            this._dgwLocalization.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgwLocalization_CellEndEdit);
            this._dgwLocalization.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgwLocalization_CellEnter);
            this._dgwLocalization.Click += new System.EventHandler(this._dgwLocalization_Click);
            // 
            // Column1
            // 
            this.Column1.FillWeight = 50F;
            this.Column1.HeaderText = "Name";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Value";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.FillWeight = 50F;
            this.Column3.HeaderText = "Comment";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Localizations";
            // 
            // _cbLocalizations
            // 
            this._cbLocalizations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbLocalizations.FormattingEnabled = true;
            this._cbLocalizations.Location = new System.Drawing.Point(78, 7);
            this._cbLocalizations.Margin = new System.Windows.Forms.Padding(4);
            this._cbLocalizations.Name = "_cbLocalizations";
            this._cbLocalizations.Size = new System.Drawing.Size(193, 21);
            this._cbLocalizations.TabIndex = 3;
            this._cbLocalizations.SelectedIndexChanged += new System.EventHandler(this._cbLocalizations_SelectedIndexChanged);
            this._cbLocalizations.Click += new System.EventHandler(this._cbLocalizations_Click);
            // 
            // _btDisDeveloperMode
            // 
            this._btDisDeveloperMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btDisDeveloperMode.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btDisDeveloperMode.Location = new System.Drawing.Point(226, 569);
            this._btDisDeveloperMode.Name = "_btDisDeveloperMode";
            this._btDisDeveloperMode.Size = new System.Drawing.Size(129, 21);
            this._btDisDeveloperMode.TabIndex = 0;
            this._btDisDeveloperMode.Text = "Disable developer mode";
            this._btDisDeveloperMode.UseVisualStyleBackColor = true;
            this._btDisDeveloperMode.Visible = false;
            this._btDisDeveloperMode.Click += new System.EventHandler(this._btDisDeveloperMode_Click);
            // 
            // _sdSaveDialog
            // 
            this._sdSaveDialog.RestoreDirectory = true;
            // 
            // _ofdOpen
            // 
            this._ofdOpen.RestoreDirectory = true;
            this._ofdOpen.ShowReadOnly = true;
            // 
            // _btReloadPackage
            // 
            this._btReloadPackage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btReloadPackage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._btReloadPackage.Location = new System.Drawing.Point(365, 569);
            this._btReloadPackage.Name = "_btReloadPackage";
            this._btReloadPackage.Size = new System.Drawing.Size(117, 22);
            this._btReloadPackage.TabIndex = 7;
            this._btReloadPackage.Text = "Reload package";
            this._btReloadPackage.UseVisualStyleBackColor = true;
            this._btReloadPackage.Visible = false;
            this._btReloadPackage.Click += new System.EventHandler(this._btReloadPackage_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this._btGenerate});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1038, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btNewLoc,
            this._btSaveLocData,
            this._btSaveLocalizationDataAs,
            this._btLoadLocData,
            this.toolStripSeparator1,
            this._btSynchAllSym,
            this.toolStripSeparator2,
            this._btSavePackage,
            this._btSavePackageAs,
            this._btLOadPackage,
            this._btClosePackage,
            this.toolStripSeparator3,
            this._btExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // _btNewLoc
            // 
            this._btNewLoc.Enabled = false;
            this._btNewLoc.Name = "_btNewLoc";
            this._btNewLoc.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this._btNewLoc.Size = new System.Drawing.Size(258, 22);
            this._btNewLoc.Text = "New localization";
            this._btNewLoc.Click += new System.EventHandler(this._btNewLoc_Click);
            // 
            // _btSaveLocData
            // 
            this._btSaveLocData.Enabled = false;
            this._btSaveLocData.Name = "_btSaveLocData";
            this._btSaveLocData.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.S)));
            this._btSaveLocData.Size = new System.Drawing.Size(258, 22);
            this._btSaveLocData.Text = "Save localization data";
            this._btSaveLocData.Click += new System.EventHandler(this._btSaveLocData_Click);
            // 
            // _btSaveLocalizationDataAs
            // 
            this._btSaveLocalizationDataAs.Enabled = false;
            this._btSaveLocalizationDataAs.Name = "_btSaveLocalizationDataAs";
            this._btSaveLocalizationDataAs.Size = new System.Drawing.Size(258, 22);
            this._btSaveLocalizationDataAs.Text = "Save localization data as...";
            this._btSaveLocalizationDataAs.Click += new System.EventHandler(this._btSaveLocalizationDataAs_Click);
            // 
            // _btLoadLocData
            // 
            this._btLoadLocData.Enabled = false;
            this._btLoadLocData.Name = "_btLoadLocData";
            this._btLoadLocData.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.L)));
            this._btLoadLocData.Size = new System.Drawing.Size(258, 22);
            this._btLoadLocData.Text = "Load localization data";
            this._btLoadLocData.Click += new System.EventHandler(this._btLoadLocData_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(255, 6);
            // 
            // _btSynchAllSym
            // 
            this._btSynchAllSym.Enabled = false;
            this._btSynchAllSym.Name = "_btSynchAllSym";
            this._btSynchAllSym.Size = new System.Drawing.Size(258, 22);
            this._btSynchAllSym.Text = "Synchronize all symbols from master";
            this._btSynchAllSym.Click += new System.EventHandler(this._btSynchAllSym_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(255, 6);
            // 
            // _btSavePackage
            // 
            this._btSavePackage.Name = "_btSavePackage";
            this._btSavePackage.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._btSavePackage.Size = new System.Drawing.Size(258, 22);
            this._btSavePackage.Text = "Save package";
            this._btSavePackage.Click += new System.EventHandler(this._btSavePackage_Click);
            // 
            // _btSavePackageAs
            // 
            this._btSavePackageAs.Name = "_btSavePackageAs";
            this._btSavePackageAs.Size = new System.Drawing.Size(258, 22);
            this._btSavePackageAs.Text = "Save package as...";
            this._btSavePackageAs.Click += new System.EventHandler(this._btSavePackageAs_Click);
            // 
            // _btLOadPackage
            // 
            this._btLOadPackage.Name = "_btLOadPackage";
            this._btLOadPackage.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this._btLOadPackage.Size = new System.Drawing.Size(258, 22);
            this._btLOadPackage.Text = "Load package";
            this._btLOadPackage.Click += new System.EventHandler(this._btLOadPackage_Click);
            // 
            // _btClosePackage
            // 
            this._btClosePackage.Enabled = false;
            this._btClosePackage.Name = "_btClosePackage";
            this._btClosePackage.Size = new System.Drawing.Size(258, 22);
            this._btClosePackage.Text = "Close package";
            this._btClosePackage.Click += new System.EventHandler(this._btClosePackage_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(255, 6);
            // 
            // _btExit
            // 
            this._btExit.Name = "_btExit";
            this._btExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this._btExit.Size = new System.Drawing.Size(258, 22);
            this._btExit.Text = "Exit";
            this._btExit.Click += new System.EventHandler(this._btExit_Click);
            // 
            // _btGenerate
            // 
            this._btGenerate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btCEnumerationFile,
            this.toolStripSeparator4,
            this._btGenAll,
            this._btDefGenPaths});
            this._btGenerate.Enabled = false;
            this._btGenerate.Name = "_btGenerate";
            this._btGenerate.Size = new System.Drawing.Size(64, 20);
            this._btGenerate.Text = "Generate";
            // 
            // _btCEnumerationFile
            // 
            this._btCEnumerationFile.Name = "_btCEnumerationFile";
            this._btCEnumerationFile.Size = new System.Drawing.Size(205, 22);
            this._btCEnumerationFile.Text = "C# enumeration file";
            this._btCEnumerationFile.Click += new System.EventHandler(this._btCEnumerationFile_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(202, 6);
            // 
            // _btGenAll
            // 
            this._btGenAll.Enabled = false;
            this._btGenAll.Name = "_btGenAll";
            this._btGenAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this._btGenAll.Size = new System.Drawing.Size(205, 22);
            this._btGenAll.Text = "Gerate all";
            this._btGenAll.Click += new System.EventHandler(this._btGenAll_Click);
            // 
            // _btDefGenPaths
            // 
            this._btDefGenPaths.Name = "_btDefGenPaths";
            this._btDefGenPaths.Size = new System.Drawing.Size(205, 22);
            this._btDefGenPaths.Text = "Default generation paths";
            this._btDefGenPaths.Click += new System.EventHandler(this._btDefGenPaths_Click);
            // 
            // LocalizationAssistentMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 589);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this._btReloadPackage);
            this.Controls.Add(this._btDisDeveloperMode);
            this.Controls.Add(this._tcMain);
            this.Controls.Add(this._rbDevMod);
            this.Controls.Add(this._rbTransMode);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "LocalizationAssistentMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contal Localization Assistant";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
            this._tcMain.ResumeLayout(false);
            this._tpTransMode.ResumeLayout(false);
            this._tpTransMode.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this._tpDevelopMode.ResumeLayout(false);
            this._tpDevelopMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgwLocalization)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.RadioButton _rbTransMode;
        private System.Windows.Forms.RadioButton _rbDevMod;
        private System.Windows.Forms.TabControl _tcMain;
        private System.Windows.Forms.TabPage _tpTransMode;
        private System.Windows.Forms.TabPage _tpDevelopMode;
        private System.Windows.Forms.Button _btDisDeveloperMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _cbLocalizations;
        private System.Windows.Forms.DataGridView _dgwLocalization;
        private System.Windows.Forms.SaveFileDialog _sdSaveDialog;
        private System.Windows.Forms.Button _btDeleteTransItem;
        private System.Windows.Forms.OpenFileDialog _ofdOpen;
        private System.Windows.Forms.Label _labMasterLang;
        private System.Windows.Forms.Label _labTranslationReport;
        private System.Windows.Forms.TextBox _edTranslationItem;
        private System.Windows.Forms.Label _labDescriptionItem;
        private System.Windows.Forms.Label _labMasterItem;
        private System.Windows.Forms.ListBox _lbOtherLocalization;
        private System.Windows.Forms.ListBox _lbMasterLocalization;
        private System.Windows.Forms.ComboBox _cbNonMasterLangs;
        private System.Windows.Forms.Button _btReloadPackage;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.Button _btInsertNewRow;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _btNewLoc;
        private System.Windows.Forms.ToolStripMenuItem _btSaveLocData;
        private System.Windows.Forms.ToolStripMenuItem _btSaveLocalizationDataAs;
        private System.Windows.Forms.ToolStripMenuItem _btLoadLocData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _btSynchAllSym;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem _btSavePackage;
        private System.Windows.Forms.ToolStripMenuItem _btSavePackageAs;
        private System.Windows.Forms.ToolStripMenuItem _btLOadPackage;
        private System.Windows.Forms.ToolStripMenuItem _btClosePackage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem _btExit;
        private System.Windows.Forms.ToolStripMenuItem _btGenerate;
        private System.Windows.Forms.ToolStripMenuItem _btCEnumerationFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem _btGenAll;
        private System.Windows.Forms.ToolStripMenuItem _btDefGenPaths;
    }
}

