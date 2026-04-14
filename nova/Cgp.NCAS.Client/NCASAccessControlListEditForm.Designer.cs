namespace Contal.Cgp.NCAS.Client
{
    partial class NCASAccessControlListEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASAccessControlListEditForm));
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._tcDateSettingsStatus = new System.Windows.Forms.TabControl();
            this._tpSettings = new System.Windows.Forms.TabPage();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._groupBoxNCASCLESettings = new System.Windows.Forms.GroupBox();
            this._tbmCardReader = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tbmTimeZone = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate = new System.Windows.Forms.ToolStripMenuItem();
            this._lCardReader = new System.Windows.Forms.Label();
            this._eDescriptionACLSettings = new System.Windows.Forms.TextBox();
            this._lDescription = new System.Windows.Forms.Label();
            this._lTimeZone = new System.Windows.Forms.Label();
            this._cbDisabled = new System.Windows.Forms.CheckBox();
            this._bCreate3 = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._bCancelEdit = new System.Windows.Forms.Button();
            this._bUpdate = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._tpAlarmAreas = new System.Windows.Forms.TabPage();
            this._panelLinesAlarmAreas = new System.Windows.Forms.Panel();
            this._panelHeaderAlarmAreas = new System.Windows.Forms.Panel();
            this._lCREventlogHandling1 = new System.Windows.Forms.Label();
            this._lTimeBuying1 = new System.Windows.Forms.Label();
            this._lSensorHandling1 = new System.Windows.Forms.Label();
            this._gbFilterSettings = new System.Windows.Forms.GroupBox();
            this._cbTimeBuying = new System.Windows.Forms.CheckBox();
            this._lTimeBuying = new System.Windows.Forms.Label();
            this._cbCREventLogHandling = new System.Windows.Forms.CheckBox();
            this._lCREventlogHandling = new System.Windows.Forms.Label();
            this._cbFilterSensorHandling = new System.Windows.Forms.CheckBox();
            this._lSensorHandling = new System.Windows.Forms.Label();
            this._lName1 = new System.Windows.Forms.Label();
            this._lAlarmAreasSet = new System.Windows.Forms.Label();
            this._lAlarmAreasUnset = new System.Windows.Forms.Label();
            this._lAlarmAreasUnconditionalSet = new System.Windows.Forms.Label();
            this._lAlarmAreasAlarmAcknowledge = new System.Windows.Forms.Label();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._eFilterNameAA = new System.Windows.Forms.TextBox();
            this._cbFilterShowOnlySetAA = new System.Windows.Forms.CheckBox();
            this._cbFilterSetAA = new System.Windows.Forms.CheckBox();
            this._cbFilterAlarmAcknowledgeAA = new System.Windows.Forms.CheckBox();
            this._cbFilterUnsetAA = new System.Windows.Forms.CheckBox();
            this._cbFilterUnconditionalSetAA = new System.Windows.Forms.CheckBox();
            this._lAlarmAreasAlarmAcknowledge1 = new System.Windows.Forms.Label();
            this._lAlarmAreasUnconditionalSet1 = new System.Windows.Forms.Label();
            this._lAlarmAreasUnset1 = new System.Windows.Forms.Label();
            this._lAlarmAreasSet1 = new System.Windows.Forms.Label();
            this._lName2 = new System.Windows.Forms.Label();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._panelBack = new System.Windows.Forms.Panel();
            this._bApply = new System.Windows.Forms.Button();
            this._bClone = new System.Windows.Forms.Button();
            this._tcDateSettingsStatus.SuspendLayout();
            this._tpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this._groupBoxNCASCLESettings.SuspendLayout();
            this._tpAlarmAreas.SuspendLayout();
            this._panelHeaderAlarmAreas.SuspendLayout();
            this._gbFilterSettings.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(1023, 528);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(942, 528);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _tcDateSettingsStatus
            // 
            this._tcDateSettingsStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcDateSettingsStatus.Controls.Add(this._tpSettings);
            this._tcDateSettingsStatus.Controls.Add(this._tpAlarmAreas);
            this._tcDateSettingsStatus.Controls.Add(this._tpUserFolders);
            this._tcDateSettingsStatus.Controls.Add(this._tpReferencedBy);
            this._tcDateSettingsStatus.Controls.Add(this._tpDescription);
            this._tcDateSettingsStatus.Location = new System.Drawing.Point(12, 51);
            this._tcDateSettingsStatus.Name = "_tcDateSettingsStatus";
            this._tcDateSettingsStatus.SelectedIndex = 0;
            this._tcDateSettingsStatus.Size = new System.Drawing.Size(1086, 471);
            this._tcDateSettingsStatus.TabIndex = 2;
            this._tcDateSettingsStatus.TabStop = false;
            // 
            // _tpSettings
            // 
            this._tpSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpSettings.Controls.Add(this._cdgvData);
            this._tpSettings.Controls.Add(this._groupBoxNCASCLESettings);
            this._tpSettings.Controls.Add(this._bCreate3);
            this._tpSettings.Controls.Add(this._bDelete);
            this._tpSettings.Controls.Add(this._bCancelEdit);
            this._tpSettings.Controls.Add(this._bUpdate);
            this._tpSettings.Controls.Add(this._bEdit);
            this._tpSettings.Location = new System.Drawing.Point(4, 22);
            this._tpSettings.Name = "_tpSettings";
            this._tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpSettings.Size = new System.Drawing.Size(1023, 445);
            this._tpSettings.TabIndex = 0;
            this._tpSettings.Text = "Settings";
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvData.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvData.DataGrid.AllowDrop = true;
            this._cdgvData.DataGrid.AllowUserToAddRows = false;
            this._cdgvData.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvData.DataGrid.AllowUserToResizeRows = false;
            this._cdgvData.DataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.ColumnHeadersHeight = 34;
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(842, 217);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(6, 193);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(842, 217);
            this._cdgvData.TabIndex = 7;
            // 
            // _groupBoxNCASCLESettings
            // 
            this._groupBoxNCASCLESettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._groupBoxNCASCLESettings.Controls.Add(this._tbmCardReader);
            this._groupBoxNCASCLESettings.Controls.Add(this._tbmTimeZone);
            this._groupBoxNCASCLESettings.Controls.Add(this._lCardReader);
            this._groupBoxNCASCLESettings.Controls.Add(this._eDescriptionACLSettings);
            this._groupBoxNCASCLESettings.Controls.Add(this._lDescription);
            this._groupBoxNCASCLESettings.Controls.Add(this._lTimeZone);
            this._groupBoxNCASCLESettings.Controls.Add(this._cbDisabled);
            this._groupBoxNCASCLESettings.Location = new System.Drawing.Point(6, 6);
            this._groupBoxNCASCLESettings.Name = "_groupBoxNCASCLESettings";
            this._groupBoxNCASCLESettings.Size = new System.Drawing.Size(551, 181);
            this._groupBoxNCASCLESettings.TabIndex = 0;
            this._groupBoxNCASCLESettings.TabStop = false;
            // 
            // _tbmCardReader
            // 
            this._tbmCardReader.AllowDrop = true;
            this._tbmCardReader.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmCardReader.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCardReader.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmCardReader.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmCardReader.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmCardReader.Button.Image")));
            this._tbmCardReader.Button.Location = new System.Drawing.Point(140, 0);
            this._tbmCardReader.Button.Name = "_bMenu";
            this._tbmCardReader.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmCardReader.Button.TabIndex = 3;
            this._tbmCardReader.Button.UseVisualStyleBackColor = false;
            this._tbmCardReader.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmCardReader.ButtonDefaultBehaviour = true;
            this._tbmCardReader.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmCardReader.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmCardReader.ButtonImage")));
            // 
            // 
            // 
            this._tbmCardReader.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2});
            this._tbmCardReader.ButtonPopupMenu.Name = "";
            this._tbmCardReader.ButtonPopupMenu.Size = new System.Drawing.Size(113, 26);
            this._tbmCardReader.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmCardReader.ButtonShowImage = true;
            this._tbmCardReader.ButtonSizeHeight = 20;
            this._tbmCardReader.ButtonSizeWidth = 20;
            this._tbmCardReader.ButtonText = "";
            this._tbmCardReader.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCardReader.HoverTime = 500;
            // 
            // 
            // 
            this._tbmCardReader.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCardReader.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmCardReader.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmCardReader.ImageTextBox.ContextMenuStrip = this._tbmCardReader.ButtonPopupMenu;
            this._tbmCardReader.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCardReader.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmCardReader.ImageTextBox.Image")));
            this._tbmCardReader.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmCardReader.ImageTextBox.Name = "_textBox";
            this._tbmCardReader.ImageTextBox.NoTextNoImage = true;
            this._tbmCardReader.ImageTextBox.ReadOnly = true;
            this._tbmCardReader.ImageTextBox.Size = new System.Drawing.Size(140, 20);
            this._tbmCardReader.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmCardReader.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCardReader.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmCardReader.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmCardReader.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCardReader.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmCardReader.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmCardReader.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmCardReader.ImageTextBox.TextBox.Size = new System.Drawing.Size(138, 13);
            this._tbmCardReader.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmCardReader.ImageTextBox.UseImage = true;
            this._tbmCardReader.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmCardReader_DoubleClick);
            this._tbmCardReader.Location = new System.Drawing.Point(10, 33);
            this._tbmCardReader.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmCardReader.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmCardReader.Name = "_tbmCardReader";
            this._tbmCardReader.Size = new System.Drawing.Size(160, 22);
            this._tbmCardReader.TabIndex = 1;
            this._tbmCardReader.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmCardReader.TextImage")));
            this._tbmCardReader.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmCardReader_DragOver);
            this._tbmCardReader.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmCardReader_DragDrop);
            this._tbmCardReader.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmCardReader_ButtonPopupMenuItemClick);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(112, 22);
            this._tsiModify2.Text = "Modify";
            // 
            // _tbmTimeZone
            // 
            this._tbmTimeZone.AllowDrop = true;
            this._tbmTimeZone.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmTimeZone.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZone.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmTimeZone.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmTimeZone.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZone.Button.Image")));
            this._tbmTimeZone.Button.Location = new System.Drawing.Point(130, 0);
            this._tbmTimeZone.Button.Name = "_bMenu";
            this._tbmTimeZone.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmTimeZone.Button.TabIndex = 3;
            this._tbmTimeZone.Button.UseVisualStyleBackColor = false;
            this._tbmTimeZone.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmTimeZone.ButtonDefaultBehaviour = true;
            this._tbmTimeZone.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmTimeZone.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZone.ButtonImage")));
            // 
            // 
            // 
            this._tbmTimeZone.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiRemove,
            this._tsiCreate});
            this._tbmTimeZone.ButtonPopupMenu.Name = "";
            this._tbmTimeZone.ButtonPopupMenu.Size = new System.Drawing.Size(118, 70);
            this._tbmTimeZone.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmTimeZone.ButtonShowImage = true;
            this._tbmTimeZone.ButtonSizeHeight = 20;
            this._tbmTimeZone.ButtonSizeWidth = 20;
            this._tbmTimeZone.ButtonText = "";
            this._tbmTimeZone.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZone.HoverTime = 500;
            // 
            // 
            // 
            this._tbmTimeZone.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZone.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmTimeZone.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmTimeZone.ImageTextBox.ContextMenuStrip = this._tbmTimeZone.ButtonPopupMenu;
            this._tbmTimeZone.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZone.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZone.ImageTextBox.Image")));
            this._tbmTimeZone.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmTimeZone.ImageTextBox.Name = "_textBox";
            this._tbmTimeZone.ImageTextBox.NoTextNoImage = true;
            this._tbmTimeZone.ImageTextBox.ReadOnly = true;
            this._tbmTimeZone.ImageTextBox.Size = new System.Drawing.Size(130, 20);
            this._tbmTimeZone.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmTimeZone.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZone.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmTimeZone.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmTimeZone.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZone.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmTimeZone.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmTimeZone.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmTimeZone.ImageTextBox.TextBox.Size = new System.Drawing.Size(128, 13);
            this._tbmTimeZone.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmTimeZone.ImageTextBox.UseImage = true;
            this._tbmTimeZone.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmTimeZone_DoubleClick);
            this._tbmTimeZone.Location = new System.Drawing.Point(202, 33);
            this._tbmTimeZone.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmTimeZone.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmTimeZone.Name = "_tbmTimeZone";
            this._tbmTimeZone.Size = new System.Drawing.Size(150, 22);
            this._tbmTimeZone.TabIndex = 3;
            this._tbmTimeZone.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZone.TextImage")));
            this._tbmTimeZone.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmTimeZone_DragOver);
            this._tbmTimeZone.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmTimeZone_DragDrop);
            this._tbmTimeZone.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmTimeZone_ButtonPopupMenuItemClick);
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
            // _tsiCreate
            // 
            this._tsiCreate.Name = "_tsiCreate";
            this._tsiCreate.Size = new System.Drawing.Size(117, 22);
            this._tsiCreate.Text = "Create";
            // 
            // _lCardReader
            // 
            this._lCardReader.AutoSize = true;
            this._lCardReader.Location = new System.Drawing.Point(6, 16);
            this._lCardReader.Name = "_lCardReader";
            this._lCardReader.Size = new System.Drawing.Size(62, 13);
            this._lCardReader.TabIndex = 0;
            this._lCardReader.Text = "Card reader";
            // 
            // _eDescriptionACLSettings
            // 
            this._eDescriptionACLSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDescriptionACLSettings.Location = new System.Drawing.Point(9, 77);
            this._eDescriptionACLSettings.Multiline = true;
            this._eDescriptionACLSettings.Name = "_eDescriptionACLSettings";
            this._eDescriptionACLSettings.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescriptionACLSettings.Size = new System.Drawing.Size(536, 98);
            this._eDescriptionACLSettings.TabIndex = 6;
            // 
            // _lDescription
            // 
            this._lDescription.AutoSize = true;
            this._lDescription.Location = new System.Drawing.Point(6, 61);
            this._lDescription.Name = "_lDescription";
            this._lDescription.Size = new System.Drawing.Size(60, 13);
            this._lDescription.TabIndex = 5;
            this._lDescription.Text = "Description";
            // 
            // _lTimeZone
            // 
            this._lTimeZone.AutoSize = true;
            this._lTimeZone.Location = new System.Drawing.Point(198, 16);
            this._lTimeZone.Name = "_lTimeZone";
            this._lTimeZone.Size = new System.Drawing.Size(56, 13);
            this._lTimeZone.TabIndex = 2;
            this._lTimeZone.Text = "Time zone";
            // 
            // _cbDisabled
            // 
            this._cbDisabled.AutoSize = true;
            this._cbDisabled.Location = new System.Drawing.Point(375, 37);
            this._cbDisabled.Name = "_cbDisabled";
            this._cbDisabled.Size = new System.Drawing.Size(67, 17);
            this._cbDisabled.TabIndex = 4;
            this._cbDisabled.Text = "Disabled";
            this._cbDisabled.UseVisualStyleBackColor = true;
            // 
            // _bCreate3
            // 
            this._bCreate3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreate3.Location = new System.Drawing.Point(564, 53);
            this._bCreate3.Name = "_bCreate3";
            this._bCreate3.Size = new System.Drawing.Size(143, 22);
            this._bCreate3.TabIndex = 1;
            this._bCreate3.Text = "Create";
            this._bCreate3.UseVisualStyleBackColor = true;
            this._bCreate3.Click += new System.EventHandler(this._bCreate3_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(768, 416);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(80, 22);
            this._bDelete.TabIndex = 6;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bCancelEdit
            // 
            this._bCancelEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancelEdit.Location = new System.Drawing.Point(564, 81);
            this._bCancelEdit.Name = "_bCancelEdit";
            this._bCancelEdit.Size = new System.Drawing.Size(143, 22);
            this._bCancelEdit.TabIndex = 3;
            this._bCancelEdit.Text = "Cancel edit";
            this._bCancelEdit.UseVisualStyleBackColor = true;
            this._bCancelEdit.Visible = false;
            this._bCancelEdit.Click += new System.EventHandler(this._bCancelEdit_Click);
            // 
            // _bUpdate
            // 
            this._bUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bUpdate.Location = new System.Drawing.Point(563, 53);
            this._bUpdate.Name = "_bUpdate";
            this._bUpdate.Size = new System.Drawing.Size(143, 22);
            this._bUpdate.TabIndex = 2;
            this._bUpdate.Text = "Update";
            this._bUpdate.UseVisualStyleBackColor = true;
            this._bUpdate.Visible = false;
            this._bUpdate.Click += new System.EventHandler(this._bUpdate_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(682, 416);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(80, 22);
            this._bEdit.TabIndex = 5;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _tpAlarmAreas
            // 
            this._tpAlarmAreas.BackColor = System.Drawing.SystemColors.Control;
            this._tpAlarmAreas.Controls.Add(this._panelLinesAlarmAreas);
            this._tpAlarmAreas.Controls.Add(this._panelHeaderAlarmAreas);
            this._tpAlarmAreas.Location = new System.Drawing.Point(4, 22);
            this._tpAlarmAreas.Name = "_tpAlarmAreas";
            this._tpAlarmAreas.Size = new System.Drawing.Size(1078, 445);
            this._tpAlarmAreas.TabIndex = 2;
            this._tpAlarmAreas.Text = "Alarm areas";
            // 
            // _panelLinesAlarmAreas
            // 
            this._panelLinesAlarmAreas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._panelLinesAlarmAreas.AutoScroll = true;
            this._panelLinesAlarmAreas.BackColor = System.Drawing.SystemColors.Window;
            this._panelLinesAlarmAreas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._panelLinesAlarmAreas.Location = new System.Drawing.Point(4, 119);
            this._panelLinesAlarmAreas.Name = "_panelLinesAlarmAreas";
            this._panelLinesAlarmAreas.Size = new System.Drawing.Size(1070, 323);
            this._panelLinesAlarmAreas.TabIndex = 16;
            this._panelLinesAlarmAreas.Resize += new System.EventHandler(this._panelLinesAlarmAreas_Resize);
            // 
            // _panelHeaderAlarmAreas
            // 
            this._panelHeaderAlarmAreas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._panelHeaderAlarmAreas.BackColor = System.Drawing.SystemColors.Window;
            this._panelHeaderAlarmAreas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._panelHeaderAlarmAreas.Controls.Add(this._lCREventlogHandling1);
            this._panelHeaderAlarmAreas.Controls.Add(this._lTimeBuying1);
            this._panelHeaderAlarmAreas.Controls.Add(this._lSensorHandling1);
            this._panelHeaderAlarmAreas.Controls.Add(this._gbFilterSettings);
            this._panelHeaderAlarmAreas.Controls.Add(this._lAlarmAreasAlarmAcknowledge1);
            this._panelHeaderAlarmAreas.Controls.Add(this._lAlarmAreasUnconditionalSet1);
            this._panelHeaderAlarmAreas.Controls.Add(this._lAlarmAreasUnset1);
            this._panelHeaderAlarmAreas.Controls.Add(this._lAlarmAreasSet1);
            this._panelHeaderAlarmAreas.Controls.Add(this._lName2);
            this._panelHeaderAlarmAreas.Location = new System.Drawing.Point(4, 3);
            this._panelHeaderAlarmAreas.Name = "_panelHeaderAlarmAreas";
            this._panelHeaderAlarmAreas.Size = new System.Drawing.Size(1070, 116);
            this._panelHeaderAlarmAreas.TabIndex = 0;
            // 
            // _lCREventlogHandling1
            // 
            this._lCREventlogHandling1.AutoSize = true;
            this._lCREventlogHandling1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lCREventlogHandling1.Location = new System.Drawing.Point(807, 96);
            this._lCREventlogHandling1.Name = "_lCREventlogHandling1";
            this._lCREventlogHandling1.Size = new System.Drawing.Size(113, 13);
            this._lCREventlogHandling1.TabIndex = 16;
            this._lCREventlogHandling1.Text = "CR Event log handling";
            // 
            // _lTimeBuying1
            // 
            this._lTimeBuying1.AutoSize = true;
            this._lTimeBuying1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lTimeBuying1.Location = new System.Drawing.Point(929, 96);
            this._lTimeBuying1.Name = "_lTimeBuying1";
            this._lTimeBuying1.Size = new System.Drawing.Size(101, 13);
            this._lTimeBuying1.TabIndex = 14;
            this._lTimeBuying1.Text = "TimeBuying";
            // 
            // _lSensorHandling1
            // 
            this._lSensorHandling1.AutoSize = true;
            this._lSensorHandling1.Location = new System.Drawing.Point(581, 96);
            this._lSensorHandling1.Name = "_lSensorHandling1";
            this._lSensorHandling1.Size = new System.Drawing.Size(83, 13);
            this._lSensorHandling1.TabIndex = 30;
            this._lSensorHandling1.Text = "Sensor handling";
            // 
            // _gbFilterSettings
            // 
            this._gbFilterSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbFilterSettings.Controls.Add(this._cbTimeBuying);
            this._gbFilterSettings.Controls.Add(this._lTimeBuying);
            this._gbFilterSettings.Controls.Add(this._cbCREventLogHandling);
            this._gbFilterSettings.Controls.Add(this._lCREventlogHandling);
            this._gbFilterSettings.Controls.Add(this._cbFilterSensorHandling);
            this._gbFilterSettings.Controls.Add(this._lSensorHandling);
            this._gbFilterSettings.Controls.Add(this._lName1);
            this._gbFilterSettings.Controls.Add(this._lAlarmAreasSet);
            this._gbFilterSettings.Controls.Add(this._lAlarmAreasUnset);
            this._gbFilterSettings.Controls.Add(this._lAlarmAreasUnconditionalSet);
            this._gbFilterSettings.Controls.Add(this._lAlarmAreasAlarmAcknowledge);
            this._gbFilterSettings.Controls.Add(this._bFilterClear);
            this._gbFilterSettings.Controls.Add(this._eFilterNameAA);
            this._gbFilterSettings.Controls.Add(this._cbFilterShowOnlySetAA);
            this._gbFilterSettings.Controls.Add(this._cbFilterSetAA);
            this._gbFilterSettings.Controls.Add(this._cbFilterAlarmAcknowledgeAA);
            this._gbFilterSettings.Controls.Add(this._cbFilterUnsetAA);
            this._gbFilterSettings.Controls.Add(this._cbFilterUnconditionalSetAA);
            this._gbFilterSettings.Location = new System.Drawing.Point(3, 3);
            this._gbFilterSettings.Name = "_gbFilterSettings";
            this._gbFilterSettings.Size = new System.Drawing.Size(1062, 88);
            this._gbFilterSettings.TabIndex = 0;
            this._gbFilterSettings.TabStop = false;
            this._gbFilterSettings.Text = "Filter settings";
            // 
            // _cbTimeBuying
            // 
            this._cbTimeBuying.AutoSize = true;
            this._cbTimeBuying.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._cbTimeBuying.Location = new System.Drawing.Point(945, 35);
            this._cbTimeBuying.Name = "_cbTimeBuying";
            this._cbTimeBuying.Size = new System.Drawing.Size(15, 14);
            this._cbTimeBuying.TabIndex = 15;
            this._cbTimeBuying.UseVisualStyleBackColor = true;
            this._cbTimeBuying.CheckedChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _lTimeBuying
            // 
            this._lTimeBuying.AutoSize = true;
            this._lTimeBuying.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lTimeBuying.Location = new System.Drawing.Point(926, 17);
            this._lTimeBuying.Name = "_lTimeBuying";
            this._lTimeBuying.Size = new System.Drawing.Size(101, 13);
            this._lTimeBuying.TabIndex = 14;
            this._lTimeBuying.Text = "TimeBuying";
            // 
            // _cbCREventLogHandling
            // 
            this._cbCREventLogHandling.AutoSize = true;
            this._cbCREventLogHandling.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._cbCREventLogHandling.Location = new System.Drawing.Point(835, 35);
            this._cbCREventLogHandling.Name = "_cbCREventLogHandling";
            this._cbCREventLogHandling.Size = new System.Drawing.Size(15, 14);
            this._cbCREventLogHandling.TabIndex = 15;
            this._cbCREventLogHandling.UseVisualStyleBackColor = true;
            this._cbCREventLogHandling.CheckedChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _lCREventlogHandling
            // 
            this._lCREventlogHandling.AutoSize = true;
            this._lCREventlogHandling.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lCREventlogHandling.Location = new System.Drawing.Point(804, 16);
            this._lCREventlogHandling.Name = "_lCREventlogHandling";
            this._lCREventlogHandling.Size = new System.Drawing.Size(113, 13);
            this._lCREventlogHandling.TabIndex = 14;
            this._lCREventlogHandling.Text = "CR Event log handling";
            // 
            // _cbFilterSensorHandling
            // 
            this._cbFilterSensorHandling.AutoSize = true;
            this._cbFilterSensorHandling.Location = new System.Drawing.Point(615, 35);
            this._cbFilterSensorHandling.Name = "_cbFilterSensorHandling";
            this._cbFilterSensorHandling.Size = new System.Drawing.Size(15, 14);
            this._cbFilterSensorHandling.TabIndex = 13;
            this._cbFilterSensorHandling.UseVisualStyleBackColor = true;
            this._cbFilterSensorHandling.CheckedChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _lSensorHandling
            // 
            this._lSensorHandling.AutoSize = true;
            this._lSensorHandling.Location = new System.Drawing.Point(578, 16);
            this._lSensorHandling.Name = "_lSensorHandling";
            this._lSensorHandling.Size = new System.Drawing.Size(83, 13);
            this._lSensorHandling.TabIndex = 12;
            this._lSensorHandling.Text = "Sensor handling";
            // 
            // _lName1
            // 
            this._lName1.AutoSize = true;
            this._lName1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lName1.Location = new System.Drawing.Point(6, 16);
            this._lName1.Name = "_lName1";
            this._lName1.Size = new System.Drawing.Size(35, 13);
            this._lName1.TabIndex = 0;
            this._lName1.Text = "Name";
            // 
            // _lAlarmAreasSet
            // 
            this._lAlarmAreasSet.AutoSize = true;
            this._lAlarmAreasSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAlarmAreasSet.Location = new System.Drawing.Point(290, 16);
            this._lAlarmAreasSet.Name = "_lAlarmAreasSet";
            this._lAlarmAreasSet.Size = new System.Drawing.Size(23, 13);
            this._lAlarmAreasSet.TabIndex = 2;
            this._lAlarmAreasSet.Text = "Set";
            // 
            // _lAlarmAreasUnset
            // 
            this._lAlarmAreasUnset.AutoSize = true;
            this._lAlarmAreasUnset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAlarmAreasUnset.Location = new System.Drawing.Point(385, 16);
            this._lAlarmAreasUnset.Name = "_lAlarmAreasUnset";
            this._lAlarmAreasUnset.Size = new System.Drawing.Size(35, 13);
            this._lAlarmAreasUnset.TabIndex = 4;
            this._lAlarmAreasUnset.Text = "Unset";
            // 
            // _lAlarmAreasUnconditionalSet
            // 
            this._lAlarmAreasUnconditionalSet.AutoSize = true;
            this._lAlarmAreasUnconditionalSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAlarmAreasUnconditionalSet.Location = new System.Drawing.Point(468, 16);
            this._lAlarmAreasUnconditionalSet.Name = "_lAlarmAreasUnconditionalSet";
            this._lAlarmAreasUnconditionalSet.Size = new System.Drawing.Size(89, 13);
            this._lAlarmAreasUnconditionalSet.TabIndex = 6;
            this._lAlarmAreasUnconditionalSet.Text = "Unconditional set";
            // 
            // _lAlarmAreasAlarmAcknowledge
            // 
            this._lAlarmAreasAlarmAcknowledge.AutoSize = true;
            this._lAlarmAreasAlarmAcknowledge.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAlarmAreasAlarmAcknowledge.Location = new System.Drawing.Point(685, 16);
            this._lAlarmAreasAlarmAcknowledge.Name = "_lAlarmAreasAlarmAcknowledge";
            this._lAlarmAreasAlarmAcknowledge.Size = new System.Drawing.Size(100, 13);
            this._lAlarmAreasAlarmAcknowledge.TabIndex = 8;
            this._lAlarmAreasAlarmAcknowledge.Text = "Alarm acknowledge";
            this._lAlarmAreasAlarmAcknowledge.Visible = false;
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bFilterClear.Location = new System.Drawing.Point(981, 58);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(75, 23);
            this._bFilterClear.TabIndex = 11;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _eFilterNameAA
            // 
            this._eFilterNameAA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eFilterNameAA.Location = new System.Drawing.Point(9, 32);
            this._eFilterNameAA.Name = "_eFilterNameAA";
            this._eFilterNameAA.Size = new System.Drawing.Size(290, 20);
            this._eFilterNameAA.TabIndex = 1;
            this._eFilterNameAA.TextChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _cbFilterShowOnlySetAA
            // 
            this._cbFilterShowOnlySetAA.AutoSize = true;
            this._cbFilterShowOnlySetAA.Location = new System.Drawing.Point(9, 62);
            this._cbFilterShowOnlySetAA.Name = "_cbFilterShowOnlySetAA";
            this._cbFilterShowOnlySetAA.Size = new System.Drawing.Size(149, 17);
            this._cbFilterShowOnlySetAA.TabIndex = 10;
            this._cbFilterShowOnlySetAA.Text = "Show only set alarm areas";
            this._cbFilterShowOnlySetAA.UseVisualStyleBackColor = true;
            this._cbFilterShowOnlySetAA.CheckedChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _cbFilterSetAA
            // 
            this._cbFilterSetAA.AutoSize = true;
            this._cbFilterSetAA.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._cbFilterSetAA.Location = new System.Drawing.Point(295, 35);
            this._cbFilterSetAA.Name = "_cbFilterSetAA";
            this._cbFilterSetAA.Size = new System.Drawing.Size(15, 14);
            this._cbFilterSetAA.TabIndex = 3;
            this._cbFilterSetAA.UseVisualStyleBackColor = true;
            this._cbFilterSetAA.CheckedChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _cbFilterAlarmAcknowledgeAA
            // 
            this._cbFilterAlarmAcknowledgeAA.AutoSize = true;
            this._cbFilterAlarmAcknowledgeAA.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._cbFilterAlarmAcknowledgeAA.Location = new System.Drawing.Point(725, 35);
            this._cbFilterAlarmAcknowledgeAA.Name = "_cbFilterAlarmAcknowledgeAA";
            this._cbFilterAlarmAcknowledgeAA.Size = new System.Drawing.Size(15, 14);
            this._cbFilterAlarmAcknowledgeAA.TabIndex = 9;
            this._cbFilterAlarmAcknowledgeAA.UseVisualStyleBackColor = true;
            this._cbFilterAlarmAcknowledgeAA.Visible = false;
            this._cbFilterAlarmAcknowledgeAA.CheckedChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _cbFilterUnsetAA
            // 
            this._cbFilterUnsetAA.AutoSize = true;
            this._cbFilterUnsetAA.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._cbFilterUnsetAA.Location = new System.Drawing.Point(395, 35);
            this._cbFilterUnsetAA.Name = "_cbFilterUnsetAA";
            this._cbFilterUnsetAA.Size = new System.Drawing.Size(15, 14);
            this._cbFilterUnsetAA.TabIndex = 5;
            this._cbFilterUnsetAA.UseVisualStyleBackColor = true;
            this._cbFilterUnsetAA.CheckedChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _cbFilterUnconditionalSetAA
            // 
            this._cbFilterUnconditionalSetAA.AutoSize = true;
            this._cbFilterUnconditionalSetAA.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._cbFilterUnconditionalSetAA.Location = new System.Drawing.Point(505, 35);
            this._cbFilterUnconditionalSetAA.Name = "_cbFilterUnconditionalSetAA";
            this._cbFilterUnconditionalSetAA.Size = new System.Drawing.Size(15, 14);
            this._cbFilterUnconditionalSetAA.TabIndex = 7;
            this._cbFilterUnconditionalSetAA.UseVisualStyleBackColor = true;
            this._cbFilterUnconditionalSetAA.CheckedChanged += new System.EventHandler(this.AlarmAreaFilterChanged);
            // 
            // _lAlarmAreasAlarmAcknowledge1
            // 
            this._lAlarmAreasAlarmAcknowledge1.AutoSize = true;
            this._lAlarmAreasAlarmAcknowledge1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAlarmAreasAlarmAcknowledge1.Location = new System.Drawing.Point(688, 94);
            this._lAlarmAreasAlarmAcknowledge1.Name = "_lAlarmAreasAlarmAcknowledge1";
            this._lAlarmAreasAlarmAcknowledge1.Size = new System.Drawing.Size(100, 13);
            this._lAlarmAreasAlarmAcknowledge1.TabIndex = 29;
            this._lAlarmAreasAlarmAcknowledge1.Text = "Alarm acknowledge";
            this._lAlarmAreasAlarmAcknowledge1.Visible = false;
            // 
            // _lAlarmAreasUnconditionalSet1
            // 
            this._lAlarmAreasUnconditionalSet1.AutoSize = true;
            this._lAlarmAreasUnconditionalSet1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAlarmAreasUnconditionalSet1.Location = new System.Drawing.Point(471, 96);
            this._lAlarmAreasUnconditionalSet1.Name = "_lAlarmAreasUnconditionalSet1";
            this._lAlarmAreasUnconditionalSet1.Size = new System.Drawing.Size(89, 13);
            this._lAlarmAreasUnconditionalSet1.TabIndex = 28;
            this._lAlarmAreasUnconditionalSet1.Text = "Unconditional set";
            // 
            // _lAlarmAreasUnset1
            // 
            this._lAlarmAreasUnset1.AutoSize = true;
            this._lAlarmAreasUnset1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAlarmAreasUnset1.Location = new System.Drawing.Point(388, 96);
            this._lAlarmAreasUnset1.Name = "_lAlarmAreasUnset1";
            this._lAlarmAreasUnset1.Size = new System.Drawing.Size(35, 13);
            this._lAlarmAreasUnset1.TabIndex = 27;
            this._lAlarmAreasUnset1.Text = "Unset";
            // 
            // _lAlarmAreasSet1
            // 
            this._lAlarmAreasSet1.AutoSize = true;
            this._lAlarmAreasSet1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAlarmAreasSet1.Location = new System.Drawing.Point(293, 96);
            this._lAlarmAreasSet1.Name = "_lAlarmAreasSet1";
            this._lAlarmAreasSet1.Size = new System.Drawing.Size(23, 13);
            this._lAlarmAreasSet1.TabIndex = 26;
            this._lAlarmAreasSet1.Text = "Set";
            // 
            // _lName2
            // 
            this._lName2.AutoSize = true;
            this._lName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lName2.Location = new System.Drawing.Point(7, 96);
            this._lName2.Name = "_lName2";
            this._lName2.Size = new System.Drawing.Size(35, 13);
            this._lName2.TabIndex = 25;
            this._lName2.Text = "Name";
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 22);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(1023, 445);
            this._tpUserFolders.TabIndex = 4;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(776, 414);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 2;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _lbUserFolders
            // 
            this._lbUserFolders.AllowDrop = false;
            this._lbUserFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbUserFolders.BackColor = System.Drawing.SystemColors.Info;
            this._lbUserFolders.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._lbUserFolders.FormattingEnabled = true;
            this._lbUserFolders.ImageList = null;
            this._lbUserFolders.Location = new System.Drawing.Point(2, 2);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(850, 407);
            this._lbUserFolders.TabIndex = 24;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(3);
            this._tpReferencedBy.Size = new System.Drawing.Size(1023, 445);
            this._tpReferencedBy.TabIndex = 3;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(3);
            this._tpDescription.Size = new System.Drawing.Size(1023, 445);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(3, 3);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(1017, 439);
            this._eDescription.TabIndex = 2;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(121, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(530, 20);
            this._eName.TabIndex = 1;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 15);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._bApply);
            this._panelBack.Controls.Add(this._bClone);
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._tcDateSettingsStatus);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(1110, 563);
            this._panelBack.TabIndex = 0;
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(861, 528);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 6;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _bClone
            // 
            this._bClone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bClone.Location = new System.Drawing.Point(12, 528);
            this._bClone.Name = "_bClone";
            this._bClone.Size = new System.Drawing.Size(75, 23);
            this._bClone.TabIndex = 3;
            this._bClone.Text = "Clone";
            this._bClone.UseVisualStyleBackColor = true;
            this._bClone.Click += new System.EventHandler(this._bClone_Click);
            // 
            // NCASAccessControlListEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1110, 563);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(756, 571);
            this.Name = "NCASAccessControlListEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NCASAccessControlListEdit";
            this._tcDateSettingsStatus.ResumeLayout(false);
            this._tpSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this._groupBoxNCASCLESettings.ResumeLayout(false);
            this._groupBoxNCASCLESettings.PerformLayout();
            this._tpAlarmAreas.ResumeLayout(false);
            this._panelHeaderAlarmAreas.ResumeLayout(false);
            this._panelHeaderAlarmAreas.PerformLayout();
            this._gbFilterSettings.ResumeLayout(false);
            this._gbFilterSettings.PerformLayout();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TabControl _tcDateSettingsStatus;
        private System.Windows.Forms.TabPage _tpSettings;
        private System.Windows.Forms.Label _lCardReader;
        private System.Windows.Forms.Label _lTimeZone;
        private System.Windows.Forms.Label _lDescription;
        private System.Windows.Forms.TextBox _eDescriptionACLSettings;
        private System.Windows.Forms.Button _bCreate3;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bCancelEdit;
        private System.Windows.Forms.Button _bUpdate;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.CheckBox _cbDisabled;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.GroupBox _groupBoxNCASCLESettings;
        private System.Windows.Forms.TabPage _tpAlarmAreas;
        private System.Windows.Forms.Panel _panelHeaderAlarmAreas;
        private System.Windows.Forms.Label _lAlarmAreasAlarmAcknowledge1;
        private System.Windows.Forms.Label _lAlarmAreasUnconditionalSet1;
        private System.Windows.Forms.Label _lAlarmAreasUnset1;
        private System.Windows.Forms.Label _lAlarmAreasSet1;
        private System.Windows.Forms.Label _lName2;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.CheckBox _cbFilterShowOnlySetAA;
        private System.Windows.Forms.CheckBox _cbFilterAlarmAcknowledgeAA;
        private System.Windows.Forms.CheckBox _cbFilterUnconditionalSetAA;
        private System.Windows.Forms.CheckBox _cbFilterUnsetAA;
        private System.Windows.Forms.CheckBox _cbFilterSetAA;
        private System.Windows.Forms.TextBox _eFilterNameAA;
        private System.Windows.Forms.Label _lAlarmAreasAlarmAcknowledge;
        private System.Windows.Forms.Label _lAlarmAreasUnconditionalSet;
        private System.Windows.Forms.Label _lAlarmAreasUnset;
        private System.Windows.Forms.Label _lAlarmAreasSet;
        private System.Windows.Forms.Label _lName1;
        private System.Windows.Forms.Panel _panelLinesAlarmAreas;
        private System.Windows.Forms.GroupBox _gbFilterSettings;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private Contal.IwQuick.UI.TextBoxMenu _tbmTimeZone;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate;
        private Contal.IwQuick.UI.TextBoxMenu _tbmCardReader;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.Button _bClone;
        private System.Windows.Forms.Label _lSensorHandling1;
        private System.Windows.Forms.CheckBox _cbFilterSensorHandling;
        private System.Windows.Forms.Label _lSensorHandling;
        private System.Windows.Forms.Button _bApply;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.CheckBox _cbCREventLogHandling;
        private System.Windows.Forms.Label _lCREventlogHandling;
        private System.Windows.Forms.Label _lCREventlogHandling1;
        private System.Windows.Forms.Label _lTimeBuying1;
        private System.Windows.Forms.CheckBox _cbTimeBuying;
        private System.Windows.Forms.Label _lTimeBuying;
    }
}
