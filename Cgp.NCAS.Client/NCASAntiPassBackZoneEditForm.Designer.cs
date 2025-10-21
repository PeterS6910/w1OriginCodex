namespace Contal.Cgp.NCAS.Client
{
    partial class NCASAntiPassBackZoneEditForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASAntiPassBackZoneEditForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this._bApply = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._tcAntiPassbackZone = new System.Windows.Forms.TabControl();
            this._tpSettings = new System.Windows.Forms.TabPage();
            this._nudTimeout = new System.Windows.Forms.DateTimePicker();
            this._lProhibitAccessForCardNotPresent = new System.Windows.Forms.Label();
            this._cbProhibitAccessForCardNotPresent = new System.Windows.Forms.CheckBox();
            this._tbmExpirationTarget = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModifyExpirationTarget = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemoveExpirationTarget = new System.Windows.Forms.ToolStripMenuItem();
            this._lExpirationTarget = new System.Windows.Forms.Label();
            this._lTimeout = new System.Windows.Forms.Label();
            this._tpCardReaders = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._pEntryCardReaders = new System.Windows.Forms.Panel();
            this._cbEntryBy = new System.Windows.Forms.ComboBox();
            this._lEntryCardReaders = new System.Windows.Forms.Label();
            this._tbmEntryCardReader = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModifyEntryCardReader = new System.Windows.Forms.ToolStripMenuItem();
            this._bInsertEntryCardReader = new System.Windows.Forms.Button();
            this._bDeleteEntryCardReader = new System.Windows.Forms.Button();
            this._cdgvEntryCardReaders = new Contal.Cgp.Components.CgpDataGridView();
            this._pExitCardReaders = new System.Windows.Forms.Panel();
            this._cbExitBy = new System.Windows.Forms.ComboBox();
            this._lExitCardReaders = new System.Windows.Forms.Label();
            this._tbmExitCardReader = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModifyExitCardReader = new System.Windows.Forms.ToolStripMenuItem();
            this._bInsertExitCardReader = new System.Windows.Forms.Button();
            this._bDeleteExitCardReader = new System.Windows.Forms.Button();
            this._cdgvExitCardReaders = new Contal.Cgp.Components.CgpDataGridView();
            this._tpCardList = new System.Windows.Forms.TabPage();
            this._bAdd = new System.Windows.Forms.Button();
            this._lCardsCountInfo = new System.Windows.Forms.Label();
            this._bClear = new System.Windows.Forms.Button();
            this._bFilter = new System.Windows.Forms.Button();
            this._tbSearch = new System.Windows.Forms.TextBox();
            this._cbSelectUnselectAll = new System.Windows.Forms.CheckBox();
            this._bRefreshCards = new System.Windows.Forms.Button();
            this._bRemove = new System.Windows.Forms.Button();
            this._cdgvCards = new Contal.Cgp.Components.CgpDataGridView();
            this._bsCards = new System.Windows.Forms.BindingSource(this.components);
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._tbDescription = new System.Windows.Forms.TextBox();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._tcAntiPassbackZone.SuspendLayout();
            this._tpSettings.SuspendLayout();
            this._tpCardReaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this._pEntryCardReaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvEntryCardReaders.DataGrid)).BeginInit();
            this._pExitCardReaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvExitCardReaders.DataGrid)).BeginInit();
            this._tpCardList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvCards.DataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._bsCards)).BeginInit();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Enabled = false;
            this._bApply.Location = new System.Drawing.Point(540, 444);
            this._bApply.Margin = new System.Windows.Forms.Padding(2);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(84, 38);
            this._bApply.TabIndex = 0;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(616, 444);
            this._bOk.Margin = new System.Windows.Forms.Padding(2);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(84, 38);
            this._bOk.TabIndex = 1;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(690, 444);
            this._bCancel.Margin = new System.Windows.Forms.Padding(2);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(84, 38);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _tcAntiPassbackZone
            // 
            this._tcAntiPassbackZone.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tcAntiPassbackZone.Controls.Add(this._tpSettings);
            this._tcAntiPassbackZone.Controls.Add(this._tpCardReaders);
            this._tcAntiPassbackZone.Controls.Add(this._tpCardList);
            this._tcAntiPassbackZone.Controls.Add(this._tpUserFolders);
            this._tcAntiPassbackZone.Controls.Add(this._tpReferencedBy);
            this._tcAntiPassbackZone.Controls.Add(this._tpDescription);
            this._tcAntiPassbackZone.Location = new System.Drawing.Point(9, 50);
            this._tcAntiPassbackZone.Margin = new System.Windows.Forms.Padding(2);
            this._tcAntiPassbackZone.Name = "_tcAntiPassbackZone";
            this._tcAntiPassbackZone.SelectedIndex = 0;
            this._tcAntiPassbackZone.Size = new System.Drawing.Size(752, 389);
            this._tcAntiPassbackZone.TabIndex = 3;
            // 
            // _tpSettings
            // 
            this._tpSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpSettings.Controls.Add(this._nudTimeout);
            this._tpSettings.Controls.Add(this._lProhibitAccessForCardNotPresent);
            this._tpSettings.Controls.Add(this._cbProhibitAccessForCardNotPresent);
            this._tpSettings.Controls.Add(this._tbmExpirationTarget);
            this._tpSettings.Controls.Add(this._lExpirationTarget);
            this._tpSettings.Controls.Add(this._lTimeout);
            this._tpSettings.Location = new System.Drawing.Point(4, 29);
            this._tpSettings.Margin = new System.Windows.Forms.Padding(2);
            this._tpSettings.Name = "_tpSettings";
            this._tpSettings.Padding = new System.Windows.Forms.Padding(2);
            this._tpSettings.Size = new System.Drawing.Size(744, 356);
            this._tpSettings.TabIndex = 0;
            this._tpSettings.Text = "Settings";
            // 
            // _nudTimeout
            // 
            this._nudTimeout.CustomFormat = "HH:mm";
            this._nudTimeout.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._nudTimeout.Location = new System.Drawing.Point(253, 6);
            this._nudTimeout.Name = "_nudTimeout";
            this._nudTimeout.ShowUpDown = true;
            this._nudTimeout.Size = new System.Drawing.Size(60, 26);
            this._nudTimeout.TabIndex = 6;
            this._nudTimeout.Value = new System.DateTime(2014, 10, 8, 0, 0, 0, 0);
            this._nudTimeout.ValueChanged += new System.EventHandler(this._nudTimeout_ValueChanged);
            // 
            // _lProhibitAccessForCardNotPresent
            // 
            this._lProhibitAccessForCardNotPresent.AutoSize = true;
            this._lProhibitAccessForCardNotPresent.Location = new System.Drawing.Point(4, 52);
            this._lProhibitAccessForCardNotPresent.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lProhibitAccessForCardNotPresent.Name = "_lProhibitAccessForCardNotPresent";
            this._lProhibitAccessForCardNotPresent.Size = new System.Drawing.Size(233, 20);
            this._lProhibitAccessForCardNotPresent.TabIndex = 0;
            this._lProhibitAccessForCardNotPresent.Text = "Prohibit exit for card not present";
            // 
            // _cbProhibitAccessForCardNotPresent
            // 
            this._cbProhibitAccessForCardNotPresent.Location = new System.Drawing.Point(253, 52);
            this._cbProhibitAccessForCardNotPresent.Margin = new System.Windows.Forms.Padding(2);
            this._cbProhibitAccessForCardNotPresent.Name = "_cbProhibitAccessForCardNotPresent";
            this._cbProhibitAccessForCardNotPresent.Size = new System.Drawing.Size(38, 15);
            this._cbProhibitAccessForCardNotPresent.TabIndex = 0;
            this._cbProhibitAccessForCardNotPresent.UseVisualStyleBackColor = true;
            this._cbProhibitAccessForCardNotPresent.CheckedChanged += new System.EventHandler(this._cbProhibitAccessForCardNotPresent_CheckedChanged);
            // 
            // _tbmExpirationTarget
            // 
            this._tbmExpirationTarget.AllowDrop = true;
            this._tbmExpirationTarget.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmExpirationTarget.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmExpirationTarget.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmExpirationTarget.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmExpirationTarget.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmExpirationTarget.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmExpirationTarget.Button.Image")));
            this._tbmExpirationTarget.Button.Location = new System.Drawing.Point(280, 0);
            this._tbmExpirationTarget.Button.Name = "_bMenu";
            this._tbmExpirationTarget.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmExpirationTarget.Button.TabIndex = 3;
            this._tbmExpirationTarget.Button.UseVisualStyleBackColor = false;
            this._tbmExpirationTarget.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmExpirationTarget.ButtonDefaultBehaviour = true;
            this._tbmExpirationTarget.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmExpirationTarget.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmExpirationTarget.ButtonImage")));
            // 
            // 
            // 
            this._tbmExpirationTarget.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModifyExpirationTarget,
            this._tsiRemoveExpirationTarget});
            this._tbmExpirationTarget.ButtonPopupMenu.Name = "";
            this._tbmExpirationTarget.ButtonPopupMenu.Size = new System.Drawing.Size(149, 68);
            this._tbmExpirationTarget.ButtonPopupMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._tbmExpirationTarget_ButtonPopupMenu_ItemClicked);
            this._tbmExpirationTarget.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmExpirationTarget.ButtonShowImage = true;
            this._tbmExpirationTarget.ButtonSizeHeight = 20;
            this._tbmExpirationTarget.ButtonSizeWidth = 20;
            this._tbmExpirationTarget.ButtonText = "";
            this._tbmExpirationTarget.HoverTime = 500;
            // 
            // 
            // 
            this._tbmExpirationTarget.ImageTextBox.AllowDrop = true;
            this._tbmExpirationTarget.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmExpirationTarget.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmExpirationTarget.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmExpirationTarget.ImageTextBox.ContextMenuStrip = this._tbmExpirationTarget.ButtonPopupMenu;
            this._tbmExpirationTarget.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmExpirationTarget.ImageTextBox.Image")));
            this._tbmExpirationTarget.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmExpirationTarget.ImageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmExpirationTarget.ImageTextBox.Name = "_itbTextBox";
            this._tbmExpirationTarget.ImageTextBox.NoTextNoImage = true;
            this._tbmExpirationTarget.ImageTextBox.ReadOnly = false;
            this._tbmExpirationTarget.ImageTextBox.Size = new System.Drawing.Size(280, 20);
            this._tbmExpirationTarget.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmExpirationTarget.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmExpirationTarget.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmExpirationTarget.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmExpirationTarget.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmExpirationTarget.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmExpirationTarget.ImageTextBox.TextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmExpirationTarget.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmExpirationTarget.ImageTextBox.TextBox.Size = new System.Drawing.Size(278, 19);
            this._tbmExpirationTarget.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmExpirationTarget.ImageTextBox.UseImage = true;
            this._tbmExpirationTarget.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmExpirationTarget_ImageTextBox_DoubleClick);
            this._tbmExpirationTarget.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmExpirationTarget_ImageTextBox_DragDrop);
            this._tbmExpirationTarget.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmExpirationTarget_ImageTextBox_DragOver);
            this._tbmExpirationTarget.Location = new System.Drawing.Point(253, 28);
            this._tbmExpirationTarget.Margin = new System.Windows.Forms.Padding(2);
            this._tbmExpirationTarget.MaximumSize = new System.Drawing.Size(900, 45);
            this._tbmExpirationTarget.MinimumSize = new System.Drawing.Size(22, 16);
            this._tbmExpirationTarget.Name = "_tbmExpirationTarget";
            this._tbmExpirationTarget.Size = new System.Drawing.Size(300, 20);
            this._tbmExpirationTarget.TabIndex = 3;
            this._tbmExpirationTarget.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmExpirationTarget.TextImage")));
            // 
            // _tsiModifyExpirationTarget
            // 
            this._tsiModifyExpirationTarget.Name = "_tsiModifyExpirationTarget";
            this._tsiModifyExpirationTarget.Size = new System.Drawing.Size(148, 32);
            this._tsiModifyExpirationTarget.Text = "Modify";
            // 
            // _tsiRemoveExpirationTarget
            // 
            this._tsiRemoveExpirationTarget.Name = "_tsiRemoveExpirationTarget";
            this._tsiRemoveExpirationTarget.Size = new System.Drawing.Size(148, 32);
            this._tsiRemoveExpirationTarget.Text = "Remove";
            // 
            // _lExpirationTarget
            // 
            this._lExpirationTarget.AutoSize = true;
            this._lExpirationTarget.Location = new System.Drawing.Point(4, 31);
            this._lExpirationTarget.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lExpirationTarget.Name = "_lExpirationTarget";
            this._lExpirationTarget.Size = new System.Drawing.Size(235, 20);
            this._lExpirationTarget.TabIndex = 2;
            this._lExpirationTarget.Text = "Post-expiration target APB zone";
            // 
            // _lTimeout
            // 
            this._lTimeout.AutoSize = true;
            this._lTimeout.Location = new System.Drawing.Point(4, 10);
            this._lTimeout.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lTimeout.Name = "_lTimeout";
            this._lTimeout.Size = new System.Drawing.Size(66, 20);
            this._lTimeout.TabIndex = 1;
            this._lTimeout.Text = "Timeout";
            // 
            // _tpCardReaders
            // 
            this._tpCardReaders.BackColor = System.Drawing.SystemColors.Control;
            this._tpCardReaders.Controls.Add(this.splitContainer1);
            this._tpCardReaders.Location = new System.Drawing.Point(4, 29);
            this._tpCardReaders.Margin = new System.Windows.Forms.Padding(2);
            this._tpCardReaders.Name = "_tpCardReaders";
            this._tpCardReaders.Padding = new System.Windows.Forms.Padding(2);
            this._tpCardReaders.Size = new System.Drawing.Size(744, 356);
            this._tpCardReaders.TabIndex = 1;
            this._tpCardReaders.Text = "Card readers";
            this._tpCardReaders.Enter += new System.EventHandler(this._tpCardReaders_Enter);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(2, 2);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._pEntryCardReaders);
            this.splitContainer1.Panel1.Controls.Add(this._cdgvEntryCardReaders);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._pExitCardReaders);
            this.splitContainer1.Panel2.Controls.Add(this._cdgvExitCardReaders);
            this.splitContainer1.Size = new System.Drawing.Size(740, 352);
            this.splitContainer1.SplitterDistance = 172;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 2;
            // 
            // _pEntryCardReaders
            // 
            this._pEntryCardReaders.Controls.Add(this._cbEntryBy);
            this._pEntryCardReaders.Controls.Add(this._lEntryCardReaders);
            this._pEntryCardReaders.Controls.Add(this._tbmEntryCardReader);
            this._pEntryCardReaders.Controls.Add(this._bInsertEntryCardReader);
            this._pEntryCardReaders.Controls.Add(this._bDeleteEntryCardReader);
            this._pEntryCardReaders.Dock = System.Windows.Forms.DockStyle.Top;
            this._pEntryCardReaders.Location = new System.Drawing.Point(0, 0);
            this._pEntryCardReaders.Margin = new System.Windows.Forms.Padding(2);
            this._pEntryCardReaders.Name = "_pEntryCardReaders";
            this._pEntryCardReaders.Size = new System.Drawing.Size(740, 30);
            this._pEntryCardReaders.TabIndex = 0;
            // 
            // _cbEntryBy
            // 
            this._cbEntryBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cbEntryBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbEntryBy.Enabled = false;
            this._cbEntryBy.FormattingEnabled = true;
            this._cbEntryBy.Location = new System.Drawing.Point(321, 5);
            this._cbEntryBy.Name = "_cbEntryBy";
            this._cbEntryBy.Size = new System.Drawing.Size(272, 28);
            this._cbEntryBy.TabIndex = 6;
            // 
            // _lEntryCardReaders
            // 
            this._lEntryCardReaders.AutoSize = true;
            this._lEntryCardReaders.Location = new System.Drawing.Point(2, 8);
            this._lEntryCardReaders.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lEntryCardReaders.Name = "_lEntryCardReaders";
            this._lEntryCardReaders.Size = new System.Drawing.Size(139, 20);
            this._lEntryCardReaders.TabIndex = 4;
            this._lEntryCardReaders.Text = "Entry card readers";
            // 
            // _tbmEntryCardReader
            // 
            this._tbmEntryCardReader.AllowDrop = true;
            this._tbmEntryCardReader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEntryCardReader.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmEntryCardReader.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEntryCardReader.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmEntryCardReader.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEntryCardReader.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmEntryCardReader.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEntryCardReader.Button.Image")));
            this._tbmEntryCardReader.Button.Location = new System.Drawing.Point(180, 0);
            this._tbmEntryCardReader.Button.Name = "_bMenu";
            this._tbmEntryCardReader.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmEntryCardReader.Button.TabIndex = 3;
            this._tbmEntryCardReader.Button.UseVisualStyleBackColor = false;
            this._tbmEntryCardReader.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmEntryCardReader.ButtonDefaultBehaviour = true;
            this._tbmEntryCardReader.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmEntryCardReader.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmEntryCardReader.ButtonImage")));
            // 
            // 
            // 
            this._tbmEntryCardReader.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModifyEntryCardReader});
            this._tbmEntryCardReader.ButtonPopupMenu.Name = "";
            this._tbmEntryCardReader.ButtonPopupMenu.Size = new System.Drawing.Size(142, 36);
            this._tbmEntryCardReader.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmEntryCardReader.ButtonShowImage = true;
            this._tbmEntryCardReader.ButtonSizeHeight = 20;
            this._tbmEntryCardReader.ButtonSizeWidth = 20;
            this._tbmEntryCardReader.ButtonText = "";
            this._tbmEntryCardReader.HoverTime = 500;
            // 
            // 
            // 
            this._tbmEntryCardReader.ImageTextBox.AllowDrop = true;
            this._tbmEntryCardReader.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEntryCardReader.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEntryCardReader.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmEntryCardReader.ImageTextBox.ContextMenuStrip = this._tbmEntryCardReader.ButtonPopupMenu;
            this._tbmEntryCardReader.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEntryCardReader.ImageTextBox.Image")));
            this._tbmEntryCardReader.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmEntryCardReader.ImageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmEntryCardReader.ImageTextBox.Name = "_itbTextBox";
            this._tbmEntryCardReader.ImageTextBox.NoTextNoImage = true;
            this._tbmEntryCardReader.ImageTextBox.ReadOnly = false;
            this._tbmEntryCardReader.ImageTextBox.Size = new System.Drawing.Size(180, 20);
            this._tbmEntryCardReader.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmEntryCardReader.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEntryCardReader.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEntryCardReader.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmEntryCardReader.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmEntryCardReader.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmEntryCardReader.ImageTextBox.TextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmEntryCardReader.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmEntryCardReader.ImageTextBox.TextBox.Size = new System.Drawing.Size(178, 19);
            this._tbmEntryCardReader.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmEntryCardReader.ImageTextBox.UseImage = true;
            this._tbmEntryCardReader.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmEntryCardReader_ImageTextBox_DoubleClick);
            this._tbmEntryCardReader.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmEntryCardReader_ImageTextBox_DragDrop);
            this._tbmEntryCardReader.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmEntryCardReader_ImageTextBox_DragOver);
            this._tbmEntryCardReader.Location = new System.Drawing.Point(116, 6);
            this._tbmEntryCardReader.Margin = new System.Windows.Forms.Padding(2);
            this._tbmEntryCardReader.MaximumSize = new System.Drawing.Size(900, 45);
            this._tbmEntryCardReader.MinimumSize = new System.Drawing.Size(22, 16);
            this._tbmEntryCardReader.Name = "_tbmEntryCardReader";
            this._tbmEntryCardReader.Size = new System.Drawing.Size(200, 20);
            this._tbmEntryCardReader.TabIndex = 3;
            this._tbmEntryCardReader.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmEntryCardReader.TextImage")));
            this._tbmEntryCardReader.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmEntryCardReader_ButtonPopupMenuItemClick);
            // 
            // _tsiModifyEntryCardReader
            // 
            this._tsiModifyEntryCardReader.Name = "_tsiModifyEntryCardReader";
            this._tsiModifyEntryCardReader.Size = new System.Drawing.Size(141, 32);
            this._tsiModifyEntryCardReader.Text = "Modify";
            // 
            // _bInsertEntryCardReader
            // 
            this._bInsertEntryCardReader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._bInsertEntryCardReader.Location = new System.Drawing.Point(598, 2);
            this._bInsertEntryCardReader.Margin = new System.Windows.Forms.Padding(2);
            this._bInsertEntryCardReader.Name = "_bInsertEntryCardReader";
            this._bInsertEntryCardReader.Size = new System.Drawing.Size(68, 25);
            this._bInsertEntryCardReader.TabIndex = 1;
            this._bInsertEntryCardReader.Text = "Insert";
            this._bInsertEntryCardReader.UseVisualStyleBackColor = true;
            this._bInsertEntryCardReader.Click += new System.EventHandler(this._bInsertEntryCardReader_Click);
            // 
            // _bDeleteEntryCardReader
            // 
            this._bDeleteEntryCardReader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._bDeleteEntryCardReader.Location = new System.Drawing.Point(670, 2);
            this._bDeleteEntryCardReader.Margin = new System.Windows.Forms.Padding(2);
            this._bDeleteEntryCardReader.Name = "_bDeleteEntryCardReader";
            this._bDeleteEntryCardReader.Size = new System.Drawing.Size(68, 25);
            this._bDeleteEntryCardReader.TabIndex = 2;
            this._bDeleteEntryCardReader.Text = "Delete";
            this._bDeleteEntryCardReader.UseVisualStyleBackColor = true;
            this._bDeleteEntryCardReader.Click += new System.EventHandler(this._bDeleteEntryCardReader_Click);
            // 
            // _cdgvEntryCardReaders
            // 
            this._cdgvEntryCardReaders.AllwaysRefreshOrder = false;
            this._cdgvEntryCardReaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvEntryCardReaders.CgpDataGridEvents = null;
            this._cdgvEntryCardReaders.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvEntryCardReaders.DataGrid.AllowDrop = true;
            this._cdgvEntryCardReaders.DataGrid.AllowUserToAddRows = false;
            this._cdgvEntryCardReaders.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvEntryCardReaders.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvEntryCardReaders.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._cdgvEntryCardReaders.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvEntryCardReaders.DataGrid.ColumnHeadersHeight = 34;
            this._cdgvEntryCardReaders.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvEntryCardReaders.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvEntryCardReaders.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvEntryCardReaders.DataGrid.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvEntryCardReaders.DataGrid.Name = "_dgvData";
            this._cdgvEntryCardReaders.DataGrid.RowHeadersVisible = false;
            this._cdgvEntryCardReaders.DataGrid.RowHeadersWidth = 62;
            this._cdgvEntryCardReaders.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvEntryCardReaders.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvEntryCardReaders.DataGrid.RowTemplate.Height = 24;
            this._cdgvEntryCardReaders.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvEntryCardReaders.DataGrid.Size = new System.Drawing.Size(740, 140);
            this._cdgvEntryCardReaders.DataGrid.TabIndex = 0;
            this._cdgvEntryCardReaders.DefaultSortColumnName = null;
            this._cdgvEntryCardReaders.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvEntryCardReaders.LocalizationHelper = null;
            this._cdgvEntryCardReaders.Location = new System.Drawing.Point(0, 32);
            this._cdgvEntryCardReaders.Margin = new System.Windows.Forms.Padding(0);
            this._cdgvEntryCardReaders.Name = "_cdgvEntryCardReaders";
            this._cdgvEntryCardReaders.Size = new System.Drawing.Size(740, 140);
            this._cdgvEntryCardReaders.TabIndex = 1;
            // 
            // _pExitCardReaders
            // 
            this._pExitCardReaders.Controls.Add(this._cbExitBy);
            this._pExitCardReaders.Controls.Add(this._lExitCardReaders);
            this._pExitCardReaders.Controls.Add(this._tbmExitCardReader);
            this._pExitCardReaders.Controls.Add(this._bInsertExitCardReader);
            this._pExitCardReaders.Controls.Add(this._bDeleteExitCardReader);
            this._pExitCardReaders.Dock = System.Windows.Forms.DockStyle.Top;
            this._pExitCardReaders.Location = new System.Drawing.Point(0, 0);
            this._pExitCardReaders.Margin = new System.Windows.Forms.Padding(2);
            this._pExitCardReaders.Name = "_pExitCardReaders";
            this._pExitCardReaders.Size = new System.Drawing.Size(740, 30);
            this._pExitCardReaders.TabIndex = 2;
            // 
            // _cbExitBy
            // 
            this._cbExitBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cbExitBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbExitBy.Enabled = false;
            this._cbExitBy.FormattingEnabled = true;
            this._cbExitBy.Location = new System.Drawing.Point(321, 5);
            this._cbExitBy.Name = "_cbExitBy";
            this._cbExitBy.Size = new System.Drawing.Size(272, 28);
            this._cbExitBy.TabIndex = 7;
            // 
            // _lExitCardReaders
            // 
            this._lExitCardReaders.AutoSize = true;
            this._lExitCardReaders.Location = new System.Drawing.Point(2, 8);
            this._lExitCardReaders.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lExitCardReaders.Name = "_lExitCardReaders";
            this._lExitCardReaders.Size = new System.Drawing.Size(128, 20);
            this._lExitCardReaders.TabIndex = 4;
            this._lExitCardReaders.Text = "Exit card readers";
            // 
            // _tbmExitCardReader
            // 
            this._tbmExitCardReader.AllowDrop = true;
            this._tbmExitCardReader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmExitCardReader.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmExitCardReader.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmExitCardReader.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmExitCardReader.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmExitCardReader.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmExitCardReader.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmExitCardReader.Button.Image")));
            this._tbmExitCardReader.Button.Location = new System.Drawing.Point(180, 0);
            this._tbmExitCardReader.Button.Name = "_bMenu";
            this._tbmExitCardReader.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmExitCardReader.Button.TabIndex = 3;
            this._tbmExitCardReader.Button.UseVisualStyleBackColor = false;
            this._tbmExitCardReader.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmExitCardReader.ButtonDefaultBehaviour = true;
            this._tbmExitCardReader.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmExitCardReader.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmExitCardReader.ButtonImage")));
            // 
            // 
            // 
            this._tbmExitCardReader.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModifyExitCardReader});
            this._tbmExitCardReader.ButtonPopupMenu.Name = "";
            this._tbmExitCardReader.ButtonPopupMenu.Size = new System.Drawing.Size(142, 36);
            this._tbmExitCardReader.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmExitCardReader.ButtonShowImage = true;
            this._tbmExitCardReader.ButtonSizeHeight = 20;
            this._tbmExitCardReader.ButtonSizeWidth = 20;
            this._tbmExitCardReader.ButtonText = "";
            this._tbmExitCardReader.HoverTime = 500;
            // 
            // 
            // 
            this._tbmExitCardReader.ImageTextBox.AllowDrop = true;
            this._tbmExitCardReader.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmExitCardReader.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmExitCardReader.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmExitCardReader.ImageTextBox.ContextMenuStrip = this._tbmExitCardReader.ButtonPopupMenu;
            this._tbmExitCardReader.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmExitCardReader.ImageTextBox.Image")));
            this._tbmExitCardReader.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmExitCardReader.ImageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmExitCardReader.ImageTextBox.Name = "_itbTextBox";
            this._tbmExitCardReader.ImageTextBox.NoTextNoImage = true;
            this._tbmExitCardReader.ImageTextBox.ReadOnly = false;
            this._tbmExitCardReader.ImageTextBox.Size = new System.Drawing.Size(180, 20);
            this._tbmExitCardReader.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmExitCardReader.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmExitCardReader.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmExitCardReader.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmExitCardReader.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmExitCardReader.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmExitCardReader.ImageTextBox.TextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmExitCardReader.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmExitCardReader.ImageTextBox.TextBox.Size = new System.Drawing.Size(178, 19);
            this._tbmExitCardReader.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmExitCardReader.ImageTextBox.UseImage = true;
            this._tbmExitCardReader.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmExitCardReader_ImageTextBox_DoubleClick);
            this._tbmExitCardReader.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmExitCardReader_ImageTextBox_DragDrop);
            this._tbmExitCardReader.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmExitCardReader_ImageTextBox_DragOver);
            this._tbmExitCardReader.Location = new System.Drawing.Point(116, 6);
            this._tbmExitCardReader.Margin = new System.Windows.Forms.Padding(2);
            this._tbmExitCardReader.MaximumSize = new System.Drawing.Size(900, 45);
            this._tbmExitCardReader.MinimumSize = new System.Drawing.Size(22, 16);
            this._tbmExitCardReader.Name = "_tbmExitCardReader";
            this._tbmExitCardReader.Size = new System.Drawing.Size(200, 20);
            this._tbmExitCardReader.TabIndex = 3;
            this._tbmExitCardReader.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmExitCardReader.TextImage")));
            this._tbmExitCardReader.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmExitCardReader_ButtonPopupMenuItemClick);
            // 
            // _tsiModifyExitCardReader
            // 
            this._tsiModifyExitCardReader.Name = "_tsiModifyExitCardReader";
            this._tsiModifyExitCardReader.Size = new System.Drawing.Size(141, 32);
            this._tsiModifyExitCardReader.Text = "Modify";
            // 
            // _bInsertExitCardReader
            // 
            this._bInsertExitCardReader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._bInsertExitCardReader.Location = new System.Drawing.Point(598, 2);
            this._bInsertExitCardReader.Margin = new System.Windows.Forms.Padding(2);
            this._bInsertExitCardReader.Name = "_bInsertExitCardReader";
            this._bInsertExitCardReader.Size = new System.Drawing.Size(68, 25);
            this._bInsertExitCardReader.TabIndex = 1;
            this._bInsertExitCardReader.Text = "Insert";
            this._bInsertExitCardReader.UseVisualStyleBackColor = true;
            this._bInsertExitCardReader.Click += new System.EventHandler(this._bInsertExitCardReader_Click);
            // 
            // _bDeleteExitCardReader
            // 
            this._bDeleteExitCardReader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._bDeleteExitCardReader.Location = new System.Drawing.Point(670, 2);
            this._bDeleteExitCardReader.Margin = new System.Windows.Forms.Padding(2);
            this._bDeleteExitCardReader.Name = "_bDeleteExitCardReader";
            this._bDeleteExitCardReader.Size = new System.Drawing.Size(68, 25);
            this._bDeleteExitCardReader.TabIndex = 2;
            this._bDeleteExitCardReader.Text = "Delete";
            this._bDeleteExitCardReader.UseVisualStyleBackColor = true;
            this._bDeleteExitCardReader.Click += new System.EventHandler(this._bDeleteExitCardReader_Click);
            // 
            // _cdgvExitCardReaders
            // 
            this._cdgvExitCardReaders.AllwaysRefreshOrder = false;
            this._cdgvExitCardReaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvExitCardReaders.CgpDataGridEvents = null;
            this._cdgvExitCardReaders.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvExitCardReaders.DataGrid.AllowDrop = true;
            this._cdgvExitCardReaders.DataGrid.AllowUserToAddRows = false;
            this._cdgvExitCardReaders.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvExitCardReaders.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvExitCardReaders.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this._cdgvExitCardReaders.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvExitCardReaders.DataGrid.ColumnHeadersHeight = 34;
            this._cdgvExitCardReaders.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvExitCardReaders.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvExitCardReaders.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvExitCardReaders.DataGrid.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvExitCardReaders.DataGrid.Name = "_dgvData";
            this._cdgvExitCardReaders.DataGrid.RowHeadersVisible = false;
            this._cdgvExitCardReaders.DataGrid.RowHeadersWidth = 62;
            this._cdgvExitCardReaders.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this._cdgvExitCardReaders.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this._cdgvExitCardReaders.DataGrid.RowTemplate.Height = 24;
            this._cdgvExitCardReaders.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvExitCardReaders.DataGrid.Size = new System.Drawing.Size(739, 144);
            this._cdgvExitCardReaders.DataGrid.TabIndex = 0;
            this._cdgvExitCardReaders.DefaultSortColumnName = null;
            this._cdgvExitCardReaders.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvExitCardReaders.LocalizationHelper = null;
            this._cdgvExitCardReaders.Location = new System.Drawing.Point(0, 33);
            this._cdgvExitCardReaders.Name = "_cdgvExitCardReaders";
            this._cdgvExitCardReaders.Size = new System.Drawing.Size(739, 144);
            this._cdgvExitCardReaders.TabIndex = 1;
            // 
            // _tpCardList
            // 
            this._tpCardList.BackColor = System.Drawing.SystemColors.Control;
            this._tpCardList.Controls.Add(this._bAdd);
            this._tpCardList.Controls.Add(this._lCardsCountInfo);
            this._tpCardList.Controls.Add(this._bClear);
            this._tpCardList.Controls.Add(this._bFilter);
            this._tpCardList.Controls.Add(this._tbSearch);
            this._tpCardList.Controls.Add(this._cbSelectUnselectAll);
            this._tpCardList.Controls.Add(this._bRefreshCards);
            this._tpCardList.Controls.Add(this._bRemove);
            this._tpCardList.Controls.Add(this._cdgvCards);
            this._tpCardList.Location = new System.Drawing.Point(4, 29);
            this._tpCardList.Margin = new System.Windows.Forms.Padding(2);
            this._tpCardList.Name = "_tpCardList";
            this._tpCardList.Padding = new System.Windows.Forms.Padding(2);
            this._tpCardList.Size = new System.Drawing.Size(744, 356);
            this._tpCardList.TabIndex = 6;
            this._tpCardList.Text = "Cards";
            this._tpCardList.Enter += new System.EventHandler(this._tpCardList_Enter);
            // 
            // _bAdd
            // 
            this._bAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd.Location = new System.Drawing.Point(521, 330);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(70, 24);
            this._bAdd.TabIndex = 14;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _lCardsCountInfo
            // 
            this._lCardsCountInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lCardsCountInfo.AutoSize = true;
            this._lCardsCountInfo.Location = new System.Drawing.Point(687, 9);
            this._lCardsCountInfo.Name = "_lCardsCountInfo";
            this._lCardsCountInfo.Size = new System.Drawing.Size(31, 20);
            this._lCardsCountInfo.TabIndex = 13;
            this._lCardsCountInfo.Text = "0/0";
            // 
            // _bClear
            // 
            this._bClear.Location = new System.Drawing.Point(256, 5);
            this._bClear.Margin = new System.Windows.Forms.Padding(2);
            this._bClear.Name = "_bClear";
            this._bClear.Size = new System.Drawing.Size(52, 20);
            this._bClear.TabIndex = 12;
            this._bClear.Text = "Clear";
            this._bClear.UseVisualStyleBackColor = true;
            this._bClear.Click += new System.EventHandler(this._bClear_Click);
            // 
            // _bFilter
            // 
            this._bFilter.Location = new System.Drawing.Point(200, 5);
            this._bFilter.Margin = new System.Windows.Forms.Padding(2);
            this._bFilter.Name = "_bFilter";
            this._bFilter.Size = new System.Drawing.Size(52, 20);
            this._bFilter.TabIndex = 11;
            this._bFilter.Text = "Filter";
            this._bFilter.UseVisualStyleBackColor = true;
            this._bFilter.Click += new System.EventHandler(this._bFilter_Click);
            // 
            // _tbSearch
            // 
            this._tbSearch.Location = new System.Drawing.Point(4, 7);
            this._tbSearch.Margin = new System.Windows.Forms.Padding(2);
            this._tbSearch.Name = "_tbSearch";
            this._tbSearch.Size = new System.Drawing.Size(194, 26);
            this._tbSearch.TabIndex = 10;
            this._tbSearch.TextChanged += new System.EventHandler(this._tbSearch_TextChanged);
            // 
            // _cbSelectUnselectAll
            // 
            this._cbSelectUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._cbSelectUnselectAll.AutoSize = true;
            this._cbSelectUnselectAll.Location = new System.Drawing.Point(4, 327);
            this._cbSelectUnselectAll.Margin = new System.Windows.Forms.Padding(2);
            this._cbSelectUnselectAll.Name = "_cbSelectUnselectAll";
            this._cbSelectUnselectAll.Size = new System.Drawing.Size(171, 24);
            this._cbSelectUnselectAll.TabIndex = 9;
            this._cbSelectUnselectAll.Text = "Select / unselect all";
            this._cbSelectUnselectAll.UseVisualStyleBackColor = true;
            this._cbSelectUnselectAll.CheckedChanged += new System.EventHandler(this._cbSelectUnselectAll_CheckedChanged);
            // 
            // _bRefreshCards
            // 
            this._bRefreshCards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefreshCards.Location = new System.Drawing.Point(670, 330);
            this._bRefreshCards.Margin = new System.Windows.Forms.Padding(2);
            this._bRefreshCards.Name = "_bRefreshCards";
            this._bRefreshCards.Size = new System.Drawing.Size(70, 24);
            this._bRefreshCards.TabIndex = 8;
            this._bRefreshCards.Text = "Refresh";
            this._bRefreshCards.UseVisualStyleBackColor = true;
            this._bRefreshCards.Click += new System.EventHandler(this._bRefreshCards_Click);
            // 
            // _bRemove
            // 
            this._bRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRemove.Location = new System.Drawing.Point(596, 330);
            this._bRemove.Margin = new System.Windows.Forms.Padding(2);
            this._bRemove.Name = "_bRemove";
            this._bRemove.Size = new System.Drawing.Size(70, 24);
            this._bRemove.TabIndex = 7;
            this._bRemove.Text = "Remove";
            this._bRemove.UseVisualStyleBackColor = true;
            this._bRemove.Click += new System.EventHandler(this._bRemove_Click);
            // 
            // _cdgvCards
            // 
            this._cdgvCards.AllwaysRefreshOrder = false;
            this._cdgvCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvCards.CgpDataGridEvents = null;
            this._cdgvCards.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvCards.DataGrid.AllowDrop = true;
            this._cdgvCards.DataGrid.AllowUserToAddRows = false;
            this._cdgvCards.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvCards.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvCards.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this._cdgvCards.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvCards.DataGrid.ColumnHeadersHeight = 34;
            this._cdgvCards.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvCards.DataGrid.DataSource = this._bsCards;
            this._cdgvCards.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvCards.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvCards.DataGrid.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvCards.DataGrid.Name = "_dgvData";
            this._cdgvCards.DataGrid.RowHeadersVisible = false;
            this._cdgvCards.DataGrid.RowHeadersWidth = 62;
            this._cdgvCards.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            this._cdgvCards.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this._cdgvCards.DataGrid.RowTemplate.Height = 24;
            this._cdgvCards.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvCards.DataGrid.Size = new System.Drawing.Size(744, 294);
            this._cdgvCards.DataGrid.TabIndex = 0;
            this._cdgvCards.DefaultSortColumnName = null;
            this._cdgvCards.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvCards.LocalizationHelper = null;
            this._cdgvCards.Location = new System.Drawing.Point(0, 31);
            this._cdgvCards.Name = "_cdgvCards";
            this._cdgvCards.Size = new System.Drawing.Size(744, 294);
            this._cdgvCards.TabIndex = 0;
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 29);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Padding = new System.Windows.Forms.Padding(2);
            this._tpUserFolders.Size = new System.Drawing.Size(744, 356);
            this._tpUserFolders.TabIndex = 5;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(670, 328);
            this._bRefresh.Margin = new System.Windows.Forms.Padding(2);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(84, 38);
            this._bRefresh.TabIndex = 1;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _lbUserFolders
            // 
            this._lbUserFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lbUserFolders.BackColor = System.Drawing.SystemColors.Info;
            this._lbUserFolders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lbUserFolders.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._lbUserFolders.FormattingEnabled = true;
            this._lbUserFolders.ImageList = null;
            this._lbUserFolders.ItemHeight = 16;
            this._lbUserFolders.Location = new System.Drawing.Point(2, 2);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(888, 370);
            this._lbUserFolders.TabIndex = 0;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 29);
            this._tpReferencedBy.Margin = new System.Windows.Forms.Padding(2);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Size = new System.Drawing.Size(744, 356);
            this._tpReferencedBy.TabIndex = 4;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._tbDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 29);
            this._tpDescription.Margin = new System.Windows.Forms.Padding(2);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(2);
            this._tpDescription.Size = new System.Drawing.Size(744, 356);
            this._tpDescription.TabIndex = 3;
            this._tpDescription.Text = "Description";
            // 
            // _tbDescription
            // 
            this._tbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tbDescription.Location = new System.Drawing.Point(2, 2);
            this._tbDescription.Margin = new System.Windows.Forms.Padding(2);
            this._tbDescription.Multiline = true;
            this._tbDescription.Name = "_tbDescription";
            this._tbDescription.Size = new System.Drawing.Size(740, 352);
            this._tbDescription.TabIndex = 0;
            this._tbDescription.TextChanged += new System.EventHandler(this._tbDescription_TextChanged);
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(112, 15);
            this._eName.Margin = new System.Windows.Forms.Padding(2);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(219, 26);
            this._eName.TabIndex = 4;
            this._eName.TextChanged += new System.EventHandler(this._tbName_TextChanged);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 17);
            this._lName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(51, 20);
            this._lName.TabIndex = 5;
            this._lName.Text = "Name";
            // 
            // NCASAntiPassBackZoneEditForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(772, 478);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bApply);
            this.Controls.Add(this._tcAntiPassbackZone);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(788, 517);
            this.Name = "NCASAntiPassBackZoneEditForm";
            this.Text = "NCASAntiPassBackZoneEditForm";
            this._tcAntiPassbackZone.ResumeLayout(false);
            this._tpSettings.ResumeLayout(false);
            this._tpSettings.PerformLayout();
            this._tpCardReaders.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this._pEntryCardReaders.ResumeLayout(false);
            this._pEntryCardReaders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvEntryCardReaders.DataGrid)).EndInit();
            this._pExitCardReaders.ResumeLayout(false);
            this._pExitCardReaders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvExitCardReaders.DataGrid)).EndInit();
            this._tpCardList.ResumeLayout(false);
            this._tpCardList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvCards.DataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._bsCards)).EndInit();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.TabControl _tcAntiPassbackZone;
        private System.Windows.Forms.TabPage _tpSettings;
        private System.Windows.Forms.TabPage _tpCardReaders;
        private System.Windows.Forms.Label _lTimeout;
        private System.Windows.Forms.Label _lExpirationTarget;
        private Contal.IwQuick.UI.TextBoxMenu _tbmExpirationTarget;
        private Contal.Cgp.Components.CgpDataGridView _cdgvEntryCardReaders;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _tbDescription;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private Contal.IwQuick.UI.TextBoxMenu _tbmEntryCardReader;
        private System.Windows.Forms.Button _bDeleteEntryCardReader;
        private System.Windows.Forms.Button _bInsertEntryCardReader;
        private System.Windows.Forms.Panel _pEntryCardReaders;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Contal.Cgp.Components.CgpDataGridView _cdgvExitCardReaders;
        private System.Windows.Forms.Label _lEntryCardReaders;
        private Contal.IwQuick.UI.TextBoxMenu _tbmExitCardReader;
        private System.Windows.Forms.Panel _pExitCardReaders;
        private System.Windows.Forms.Label _lExitCardReaders;
        private System.Windows.Forms.Button _bInsertExitCardReader;
        private System.Windows.Forms.Button _bDeleteExitCardReader;
        private System.Windows.Forms.ToolStripMenuItem _tsiModifyEntryCardReader;
        private System.Windows.Forms.ToolStripMenuItem _tsiModifyExitCardReader;
        private System.Windows.Forms.ToolStripMenuItem _tsiModifyExpirationTarget;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemoveExpirationTarget;
        private System.Windows.Forms.CheckBox _cbProhibitAccessForCardNotPresent;
        private System.Windows.Forms.Label _lProhibitAccessForCardNotPresent;
        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.DateTimePicker _nudTimeout;
        private System.Windows.Forms.TabPage _tpCardList;
        private Contal.Cgp.Components.CgpDataGridView _cdgvCards;
        private System.Windows.Forms.Button _bRemove;
        private System.Windows.Forms.Button _bRefreshCards;
        private System.Windows.Forms.CheckBox _cbSelectUnselectAll;
        private System.Windows.Forms.BindingSource _bsCards;
        private System.Windows.Forms.TextBox _tbSearch;
        private System.Windows.Forms.Button _bClear;
        private System.Windows.Forms.Button _bFilter;
        private System.Windows.Forms.Label _lCardsCountInfo;
        private System.Windows.Forms.ComboBox _cbEntryBy;
        private System.Windows.Forms.ComboBox _cbExitBy;
        private System.Windows.Forms.Button _bAdd;
    }
}
