namespace Contal.Cgp.NCAS.Client
{
    partial class NCASMultiDoorEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASMultiDoorEditForm));
            this._lName = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._tcMultiDoor = new System.Windows.Forms.TabControl();
            this._tpSettings = new System.Windows.Forms.TabPage();
            this._lAlarmArea = new System.Windows.Forms.Label();
            this._eBlockAlarmArea = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._lType = new System.Windows.Forms.Label();
            this._cbType = new System.Windows.Forms.ComboBox();
            this._lCardReader = new System.Windows.Forms.Label();
            this._eCardReader = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._tpApas = new System.Windows.Forms.TabPage();
            this._gbSensorOpenDoor = new System.Windows.Forms.GroupBox();
            this._chbOpenDoorBalanced = new System.Windows.Forms.CheckBox();
            this._chbOpenDoorInverted = new System.Windows.Forms.CheckBox();
            this._gbExtraElectricStrike = new System.Windows.Forms.GroupBox();
            this._eExtraElectricStrikeImpulseDelay = new System.Windows.Forms.NumericUpDown();
            this._chbExtraElectricStrikeImpulse = new System.Windows.Forms.CheckBox();
            this._gbElectricStike = new System.Windows.Forms.GroupBox();
            this._eElectricStrikeImpulseDelay = new System.Windows.Forms.NumericUpDown();
            this._chbElectricStrikeImpulse = new System.Windows.Forms.CheckBox();
            this._gbDoorsDelays = new System.Windows.Forms.GroupBox();
            this._lDelayBeforeLock = new System.Windows.Forms.Label();
            this._eDelayBeforeLock = new System.Windows.Forms.NumericUpDown();
            this._lDelayBeforeClose = new System.Windows.Forms.Label();
            this._lDelayBeforeUnlock = new System.Windows.Forms.Label();
            this._eDelayBeforeClose = new System.Windows.Forms.NumericUpDown();
            this._eDelayBeforeUnlock = new System.Windows.Forms.NumericUpDown();
            this._gbDoorsTimes = new System.Windows.Forms.GroupBox();
            this._lTimePreAlarm = new System.Windows.Forms.Label();
            this._eTimePreAlarm = new System.Windows.Forms.NumericUpDown();
            this._lTimeOpen = new System.Windows.Forms.Label();
            this._eTimeOpen = new System.Windows.Forms.NumericUpDown();
            this._eTimeUnlock = new System.Windows.Forms.NumericUpDown();
            this._lTimeUnlock = new System.Windows.Forms.Label();
            this._tpDoors = new System.Windows.Forms.TabPage();
            this._dgMultiDoorElements = new Contal.Cgp.Components.CgpDataGridView();
            this._bCreate = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._tpObjectPlacement = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._bApply = new System.Windows.Forms.Button();
            this._tcMultiDoor.SuspendLayout();
            this._tpSettings.SuspendLayout();
            this._tpApas.SuspendLayout();
            this._gbSensorOpenDoor.SuspendLayout();
            this._gbExtraElectricStrike.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eExtraElectricStrikeImpulseDelay)).BeginInit();
            this._gbElectricStike.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eElectricStrikeImpulseDelay)).BeginInit();
            this._gbDoorsDelays.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayBeforeLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayBeforeClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayBeforeUnlock)).BeginInit();
            this._gbDoorsTimes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eTimePreAlarm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eTimeOpen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eTimeUnlock)).BeginInit();
            this._tpDoors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgMultiDoorElements.DataGrid)).BeginInit();
            this._tpObjectPlacement.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this.SuspendLayout();
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
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(102, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(540, 20);
            this._eName.TabIndex = 1;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _tcMultiDoor
            // 
            this._tcMultiDoor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcMultiDoor.Controls.Add(this._tpSettings);
            this._tcMultiDoor.Controls.Add(this._tpApas);
            this._tcMultiDoor.Controls.Add(this._tpDoors);
            this._tcMultiDoor.Controls.Add(this._tpObjectPlacement);
            this._tcMultiDoor.Controls.Add(this._tpReferencedBy);
            this._tcMultiDoor.Controls.Add(this._tpDescription);
            this._tcMultiDoor.Location = new System.Drawing.Point(12, 38);
            this._tcMultiDoor.Name = "_tcMultiDoor";
            this._tcMultiDoor.SelectedIndex = 0;
            this._tcMultiDoor.Size = new System.Drawing.Size(630, 302);
            this._tcMultiDoor.TabIndex = 2;
            // 
            // _tpSettings
            // 
            this._tpSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpSettings.Controls.Add(this._lAlarmArea);
            this._tpSettings.Controls.Add(this._eBlockAlarmArea);
            this._tpSettings.Controls.Add(this._lType);
            this._tpSettings.Controls.Add(this._cbType);
            this._tpSettings.Controls.Add(this._lCardReader);
            this._tpSettings.Controls.Add(this._eCardReader);
            this._tpSettings.Location = new System.Drawing.Point(4, 22);
            this._tpSettings.Name = "_tpSettings";
            this._tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpSettings.Size = new System.Drawing.Size(622, 276);
            this._tpSettings.TabIndex = 0;
            this._tpSettings.Text = "Settings";
            // 
            // _lAlarmArea
            // 
            this._lAlarmArea.AutoSize = true;
            this._lAlarmArea.Location = new System.Drawing.Point(6, 62);
            this._lAlarmArea.Name = "_lAlarmArea";
            this._lAlarmArea.Size = new System.Drawing.Size(115, 13);
            this._lAlarmArea.TabIndex = 11;
            this._lAlarmArea.Text = "Alarm area for blocking";
            // 
            // _eBlockAlarmArea
            // 
            this._eBlockAlarmArea.AllowDrop = true;
            this._eBlockAlarmArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eBlockAlarmArea.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._eBlockAlarmArea.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._eBlockAlarmArea.Button.BackColor = System.Drawing.SystemColors.Control;
            this._eBlockAlarmArea.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._eBlockAlarmArea.Button.Image = ((System.Drawing.Image)(resources.GetObject("_eBlockAlarmArea.Button.Image")));
            this._eBlockAlarmArea.Button.Location = new System.Drawing.Point(380, 0);
            this._eBlockAlarmArea.Button.Name = "_bMenu";
            this._eBlockAlarmArea.Button.Size = new System.Drawing.Size(20, 20);
            this._eBlockAlarmArea.Button.TabIndex = 3;
            this._eBlockAlarmArea.Button.UseVisualStyleBackColor = false;
            this._eBlockAlarmArea.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._eBlockAlarmArea.ButtonDefaultBehaviour = true;
            this._eBlockAlarmArea.ButtonHoverColor = System.Drawing.Color.Empty;
            this._eBlockAlarmArea.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_eBlockAlarmArea.ButtonImage")));
            // 
            // 
            // 
            this._eBlockAlarmArea.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2});
            this._eBlockAlarmArea.ButtonPopupMenu.Name = "";
            this._eBlockAlarmArea.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._eBlockAlarmArea.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._eBlockAlarmArea.ButtonShowImage = true;
            this._eBlockAlarmArea.ButtonSizeHeight = 20;
            this._eBlockAlarmArea.ButtonSizeWidth = 20;
            this._eBlockAlarmArea.ButtonText = "";
            this._eBlockAlarmArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eBlockAlarmArea.HoverTime = 500;
            // 
            // 
            // 
            this._eBlockAlarmArea.ImageTextBox.AllowDrop = true;
            this._eBlockAlarmArea.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eBlockAlarmArea.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._eBlockAlarmArea.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eBlockAlarmArea.ImageTextBox.ContextMenuStrip = this._eBlockAlarmArea.ButtonPopupMenu;
            this._eBlockAlarmArea.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eBlockAlarmArea.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_eBlockAlarmArea.ImageTextBox.Image")));
            this._eBlockAlarmArea.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._eBlockAlarmArea.ImageTextBox.Name = "_itbTextBox";
            this._eBlockAlarmArea.ImageTextBox.NoTextNoImage = true;
            this._eBlockAlarmArea.ImageTextBox.ReadOnly = false;
            this._eBlockAlarmArea.ImageTextBox.Size = new System.Drawing.Size(380, 20);
            this._eBlockAlarmArea.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._eBlockAlarmArea.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eBlockAlarmArea.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._eBlockAlarmArea.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._eBlockAlarmArea.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eBlockAlarmArea.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._eBlockAlarmArea.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._eBlockAlarmArea.ImageTextBox.TextBox.Size = new System.Drawing.Size(378, 13);
            this._eBlockAlarmArea.ImageTextBox.TextBox.TabIndex = 2;
            this._eBlockAlarmArea.ImageTextBox.UseImage = true;
            this._eBlockAlarmArea.ImageTextBox.DoubleClick += new System.EventHandler(this._eBlockAlarmArea_ImageTextBox_DoubleClick);
            this._eBlockAlarmArea.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._eBlockAlarmArea_ImageTextBox_DragDrop);
            this._eBlockAlarmArea.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._eBlockAlarmArea_ImageTextBox_DragOver);
            this._eBlockAlarmArea.Location = new System.Drawing.Point(157, 59);
            this._eBlockAlarmArea.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eBlockAlarmArea.MinimumSize = new System.Drawing.Size(30, 20);
            this._eBlockAlarmArea.Name = "_eBlockAlarmArea";
            this._eBlockAlarmArea.Size = new System.Drawing.Size(400, 20);
            this._eBlockAlarmArea.TabIndex = 10;
            this._eBlockAlarmArea.TextImage = ((System.Drawing.Image)(resources.GetObject("_eBlockAlarmArea.TextImage")));
            this._eBlockAlarmArea.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eBlockAlarmArea_ButtonPopupMenuItemClick);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(117, 22);
            this._tsiModify2.Text = "Modify";
            // 
            // _tsiRemove2
            // 
            this._tsiRemove2.Name = "_tsiRemove2";
            this._tsiRemove2.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove2.Text = "Remove";
            // 
            // _lType
            // 
            this._lType.AutoSize = true;
            this._lType.Location = new System.Drawing.Point(6, 35);
            this._lType.Name = "_lType";
            this._lType.Size = new System.Drawing.Size(31, 13);
            this._lType.TabIndex = 9;
            this._lType.Text = "Type";
            // 
            // _cbType
            // 
            this._cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbType.FormattingEnabled = true;
            this._cbType.Location = new System.Drawing.Point(157, 32);
            this._cbType.Name = "_cbType";
            this._cbType.Size = new System.Drawing.Size(400, 21);
            this._cbType.TabIndex = 8;
            this._cbType.SelectedValueChanged += new System.EventHandler(this._cbType_SelectedValueChanged);
            // 
            // _lCardReader
            // 
            this._lCardReader.AutoSize = true;
            this._lCardReader.Location = new System.Drawing.Point(6, 9);
            this._lCardReader.Name = "_lCardReader";
            this._lCardReader.Size = new System.Drawing.Size(62, 13);
            this._lCardReader.TabIndex = 6;
            this._lCardReader.Text = "Card reader";
            // 
            // _eCardReader
            // 
            this._eCardReader.AllowDrop = true;
            this._eCardReader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eCardReader.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._eCardReader.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._eCardReader.Button.BackColor = System.Drawing.SystemColors.Control;
            this._eCardReader.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._eCardReader.Button.Image = ((System.Drawing.Image)(resources.GetObject("_eCardReader.Button.Image")));
            this._eCardReader.Button.Location = new System.Drawing.Point(380, 0);
            this._eCardReader.Button.Name = "_bMenu";
            this._eCardReader.Button.Size = new System.Drawing.Size(20, 20);
            this._eCardReader.Button.TabIndex = 3;
            this._eCardReader.Button.UseVisualStyleBackColor = false;
            this._eCardReader.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._eCardReader.ButtonDefaultBehaviour = true;
            this._eCardReader.ButtonHoverColor = System.Drawing.Color.Empty;
            this._eCardReader.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_eCardReader.ButtonImage")));
            // 
            // 
            // 
            this._eCardReader.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiRemove});
            this._eCardReader.ButtonPopupMenu.Name = "";
            this._eCardReader.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._eCardReader.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._eCardReader.ButtonShowImage = true;
            this._eCardReader.ButtonSizeHeight = 20;
            this._eCardReader.ButtonSizeWidth = 20;
            this._eCardReader.ButtonText = "";
            this._eCardReader.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eCardReader.HoverTime = 500;
            // 
            // 
            // 
            this._eCardReader.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eCardReader.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._eCardReader.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eCardReader.ImageTextBox.ContextMenuStrip = this._eCardReader.ButtonPopupMenu;
            this._eCardReader.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eCardReader.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_eCardReader.ImageTextBox.Image")));
            this._eCardReader.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._eCardReader.ImageTextBox.Name = "_itbTextBox";
            this._eCardReader.ImageTextBox.NoTextNoImage = true;
            this._eCardReader.ImageTextBox.ReadOnly = false;
            this._eCardReader.ImageTextBox.Size = new System.Drawing.Size(380, 20);
            this._eCardReader.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._eCardReader.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eCardReader.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._eCardReader.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._eCardReader.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eCardReader.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._eCardReader.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._eCardReader.ImageTextBox.TextBox.Size = new System.Drawing.Size(378, 13);
            this._eCardReader.ImageTextBox.TextBox.TabIndex = 2;
            this._eCardReader.ImageTextBox.UseImage = true;
            this._eCardReader.ImageTextBox.DoubleClick += new System.EventHandler(this._eCardReader_ImageTextBox_DoubleClick);
            this._eCardReader.Location = new System.Drawing.Point(157, 6);
            this._eCardReader.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eCardReader.MinimumSize = new System.Drawing.Size(30, 20);
            this._eCardReader.Name = "_eCardReader";
            this._eCardReader.Size = new System.Drawing.Size(400, 20);
            this._eCardReader.TabIndex = 7;
            this._eCardReader.TextImage = ((System.Drawing.Image)(resources.GetObject("_eCardReader.TextImage")));
            this._eCardReader.DragOver += new System.Windows.Forms.DragEventHandler(this._eCardReader_DragOver);
            this._eCardReader.DragDrop += new System.Windows.Forms.DragEventHandler(this._eCardReader_DragDrop);
            this._eCardReader.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eCardReader_ButtonPopupMenuItemClick);
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
            // _tpApas
            // 
            this._tpApas.BackColor = System.Drawing.SystemColors.Control;
            this._tpApas.Controls.Add(this._gbSensorOpenDoor);
            this._tpApas.Controls.Add(this._gbExtraElectricStrike);
            this._tpApas.Controls.Add(this._gbElectricStike);
            this._tpApas.Controls.Add(this._gbDoorsDelays);
            this._tpApas.Controls.Add(this._gbDoorsTimes);
            this._tpApas.Location = new System.Drawing.Point(4, 22);
            this._tpApas.Name = "_tpApas";
            this._tpApas.Size = new System.Drawing.Size(622, 276);
            this._tpApas.TabIndex = 5;
            this._tpApas.Text = "Apas";
            // 
            // _gbSensorOpenDoor
            // 
            this._gbSensorOpenDoor.Controls.Add(this._chbOpenDoorBalanced);
            this._gbSensorOpenDoor.Controls.Add(this._chbOpenDoorInverted);
            this._gbSensorOpenDoor.Location = new System.Drawing.Point(3, 212);
            this._gbSensorOpenDoor.Name = "_gbSensorOpenDoor";
            this._gbSensorOpenDoor.Size = new System.Drawing.Size(261, 45);
            this._gbSensorOpenDoor.TabIndex = 14;
            this._gbSensorOpenDoor.TabStop = false;
            this._gbSensorOpenDoor.Text = "Sensor open door";
            // 
            // _chbOpenDoorBalanced
            // 
            this._chbOpenDoorBalanced.Location = new System.Drawing.Point(159, 22);
            this._chbOpenDoorBalanced.Name = "_chbOpenDoorBalanced";
            this._chbOpenDoorBalanced.Size = new System.Drawing.Size(96, 17);
            this._chbOpenDoorBalanced.TabIndex = 4;
            this._chbOpenDoorBalanced.Text = "Balanced";
            this._chbOpenDoorBalanced.UseVisualStyleBackColor = true;
            this._chbOpenDoorBalanced.CheckedChanged += new System.EventHandler(this._chbOpenDoorBalanced_CheckedChanged);
            // 
            // _chbOpenDoorInverted
            // 
            this._chbOpenDoorInverted.Location = new System.Drawing.Point(6, 22);
            this._chbOpenDoorInverted.Name = "_chbOpenDoorInverted";
            this._chbOpenDoorInverted.Size = new System.Drawing.Size(85, 17);
            this._chbOpenDoorInverted.TabIndex = 3;
            this._chbOpenDoorInverted.Text = "Inverted";
            this._chbOpenDoorInverted.UseVisualStyleBackColor = true;
            this._chbOpenDoorInverted.CheckedChanged += new System.EventHandler(this._chbOpenDoorInverted_CheckedChanged);
            // 
            // _gbExtraElectricStrike
            // 
            this._gbExtraElectricStrike.Controls.Add(this._eExtraElectricStrikeImpulseDelay);
            this._gbExtraElectricStrike.Controls.Add(this._chbExtraElectricStrikeImpulse);
            this._gbExtraElectricStrike.Location = new System.Drawing.Point(3, 161);
            this._gbExtraElectricStrike.Name = "_gbExtraElectricStrike";
            this._gbExtraElectricStrike.Size = new System.Drawing.Size(261, 45);
            this._gbExtraElectricStrike.TabIndex = 13;
            this._gbExtraElectricStrike.TabStop = false;
            this._gbExtraElectricStrike.Text = "Extra electric stike";
            // 
            // _eExtraElectricStrikeImpulseDelay
            // 
            this._eExtraElectricStrikeImpulseDelay.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this._eExtraElectricStrikeImpulseDelay.Location = new System.Drawing.Point(159, 19);
            this._eExtraElectricStrikeImpulseDelay.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._eExtraElectricStrikeImpulseDelay.Name = "_eExtraElectricStrikeImpulseDelay";
            this._eExtraElectricStrikeImpulseDelay.Size = new System.Drawing.Size(74, 20);
            this._eExtraElectricStrikeImpulseDelay.TabIndex = 7;
            this._eExtraElectricStrikeImpulseDelay.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._eExtraElectricStrikeImpulseDelay.ValueChanged += new System.EventHandler(this._eExtraElectricStrikeImpulseDelay_ValueChanged);
            // 
            // _chbExtraElectricStrikeImpulse
            // 
            this._chbExtraElectricStrikeImpulse.Location = new System.Drawing.Point(6, 20);
            this._chbExtraElectricStrikeImpulse.Name = "_chbExtraElectricStrikeImpulse";
            this._chbExtraElectricStrikeImpulse.Size = new System.Drawing.Size(140, 17);
            this._chbExtraElectricStrikeImpulse.TabIndex = 6;
            this._chbExtraElectricStrikeImpulse.Text = "Impulse";
            this._chbExtraElectricStrikeImpulse.UseVisualStyleBackColor = true;
            this._chbExtraElectricStrikeImpulse.CheckedChanged += new System.EventHandler(this._chbExtraElectricStrikeImpulse_CheckedChanged);
            // 
            // _gbElectricStike
            // 
            this._gbElectricStike.Controls.Add(this._eElectricStrikeImpulseDelay);
            this._gbElectricStike.Controls.Add(this._chbElectricStrikeImpulse);
            this._gbElectricStike.Location = new System.Drawing.Point(3, 110);
            this._gbElectricStike.Name = "_gbElectricStike";
            this._gbElectricStike.Size = new System.Drawing.Size(261, 45);
            this._gbElectricStike.TabIndex = 12;
            this._gbElectricStike.TabStop = false;
            this._gbElectricStike.Text = "Electric strike";
            // 
            // _eElectricStrikeImpulseDelay
            // 
            this._eElectricStrikeImpulseDelay.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this._eElectricStrikeImpulseDelay.Location = new System.Drawing.Point(159, 19);
            this._eElectricStrikeImpulseDelay.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._eElectricStrikeImpulseDelay.Name = "_eElectricStrikeImpulseDelay";
            this._eElectricStrikeImpulseDelay.Size = new System.Drawing.Size(74, 20);
            this._eElectricStrikeImpulseDelay.TabIndex = 4;
            this._eElectricStrikeImpulseDelay.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._eElectricStrikeImpulseDelay.ValueChanged += new System.EventHandler(this._eElectricStrikeImpulseDelay_ValueChanged);
            // 
            // _chbElectricStrikeImpulse
            // 
            this._chbElectricStrikeImpulse.Location = new System.Drawing.Point(6, 20);
            this._chbElectricStrikeImpulse.Name = "_chbElectricStrikeImpulse";
            this._chbElectricStrikeImpulse.Size = new System.Drawing.Size(140, 17);
            this._chbElectricStrikeImpulse.TabIndex = 3;
            this._chbElectricStrikeImpulse.Text = "Impulse";
            this._chbElectricStrikeImpulse.UseVisualStyleBackColor = true;
            this._chbElectricStrikeImpulse.CheckedChanged += new System.EventHandler(this._chbElectricStrikeImpulse_CheckedChanged);
            // 
            // _gbDoorsDelays
            // 
            this._gbDoorsDelays.Controls.Add(this._lDelayBeforeLock);
            this._gbDoorsDelays.Controls.Add(this._eDelayBeforeLock);
            this._gbDoorsDelays.Controls.Add(this._lDelayBeforeClose);
            this._gbDoorsDelays.Controls.Add(this._lDelayBeforeUnlock);
            this._gbDoorsDelays.Controls.Add(this._eDelayBeforeClose);
            this._gbDoorsDelays.Controls.Add(this._eDelayBeforeUnlock);
            this._gbDoorsDelays.Location = new System.Drawing.Point(313, 3);
            this._gbDoorsDelays.Name = "_gbDoorsDelays";
            this._gbDoorsDelays.Size = new System.Drawing.Size(304, 101);
            this._gbDoorsDelays.TabIndex = 11;
            this._gbDoorsDelays.TabStop = false;
            this._gbDoorsDelays.Text = "Door delays";
            // 
            // _lDelayBeforeLock
            // 
            this._lDelayBeforeLock.AutoSize = true;
            this._lDelayBeforeLock.Location = new System.Drawing.Point(6, 47);
            this._lDelayBeforeLock.Name = "_lDelayBeforeLock";
            this._lDelayBeforeLock.Size = new System.Drawing.Size(83, 13);
            this._lDelayBeforeLock.TabIndex = 2;
            this._lDelayBeforeLock.Text = "Before lock (ms)";
            // 
            // _eDelayBeforeLock
            // 
            this._eDelayBeforeLock.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this._eDelayBeforeLock.Location = new System.Drawing.Point(141, 45);
            this._eDelayBeforeLock.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._eDelayBeforeLock.Name = "_eDelayBeforeLock";
            this._eDelayBeforeLock.Size = new System.Drawing.Size(120, 20);
            this._eDelayBeforeLock.TabIndex = 1;
            this._eDelayBeforeLock.ValueChanged += new System.EventHandler(this._eDelayBeforeLock_ValueChanged);
            // 
            // _lDelayBeforeClose
            // 
            this._lDelayBeforeClose.AutoSize = true;
            this._lDelayBeforeClose.Location = new System.Drawing.Point(6, 73);
            this._lDelayBeforeClose.Name = "_lDelayBeforeClose";
            this._lDelayBeforeClose.Size = new System.Drawing.Size(88, 13);
            this._lDelayBeforeClose.TabIndex = 4;
            this._lDelayBeforeClose.Text = "Before close (ms)";
            // 
            // _lDelayBeforeUnlock
            // 
            this._lDelayBeforeUnlock.AutoSize = true;
            this._lDelayBeforeUnlock.Location = new System.Drawing.Point(6, 21);
            this._lDelayBeforeUnlock.Name = "_lDelayBeforeUnlock";
            this._lDelayBeforeUnlock.Size = new System.Drawing.Size(95, 13);
            this._lDelayBeforeUnlock.TabIndex = 0;
            this._lDelayBeforeUnlock.Text = "Before unlock (ms)";
            // 
            // _eDelayBeforeClose
            // 
            this._eDelayBeforeClose.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this._eDelayBeforeClose.Location = new System.Drawing.Point(141, 71);
            this._eDelayBeforeClose.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._eDelayBeforeClose.Name = "_eDelayBeforeClose";
            this._eDelayBeforeClose.Size = new System.Drawing.Size(120, 20);
            this._eDelayBeforeClose.TabIndex = 2;
            this._eDelayBeforeClose.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._eDelayBeforeClose.ValueChanged += new System.EventHandler(this._eDelayBeforeClose_ValueChanged);
            // 
            // _eDelayBeforeUnlock
            // 
            this._eDelayBeforeUnlock.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this._eDelayBeforeUnlock.Location = new System.Drawing.Point(141, 19);
            this._eDelayBeforeUnlock.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._eDelayBeforeUnlock.Name = "_eDelayBeforeUnlock";
            this._eDelayBeforeUnlock.Size = new System.Drawing.Size(120, 20);
            this._eDelayBeforeUnlock.TabIndex = 0;
            this._eDelayBeforeUnlock.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this._eDelayBeforeUnlock.ValueChanged += new System.EventHandler(this._eDelayBeforeUnlock_ValueChanged);
            // 
            // _gbDoorsTimes
            // 
            this._gbDoorsTimes.Controls.Add(this._lTimePreAlarm);
            this._gbDoorsTimes.Controls.Add(this._eTimePreAlarm);
            this._gbDoorsTimes.Controls.Add(this._lTimeOpen);
            this._gbDoorsTimes.Controls.Add(this._eTimeOpen);
            this._gbDoorsTimes.Controls.Add(this._eTimeUnlock);
            this._gbDoorsTimes.Controls.Add(this._lTimeUnlock);
            this._gbDoorsTimes.Location = new System.Drawing.Point(3, 3);
            this._gbDoorsTimes.Name = "_gbDoorsTimes";
            this._gbDoorsTimes.Size = new System.Drawing.Size(304, 101);
            this._gbDoorsTimes.TabIndex = 10;
            this._gbDoorsTimes.TabStop = false;
            this._gbDoorsTimes.Text = "Door timing";
            // 
            // _lTimePreAlarm
            // 
            this._lTimePreAlarm.AutoSize = true;
            this._lTimePreAlarm.Location = new System.Drawing.Point(6, 73);
            this._lTimePreAlarm.Name = "_lTimePreAlarm";
            this._lTimePreAlarm.Size = new System.Drawing.Size(66, 13);
            this._lTimePreAlarm.TabIndex = 4;
            this._lTimePreAlarm.Text = "Pre-Alarm (s)";
            // 
            // _eTimePreAlarm
            // 
            this._eTimePreAlarm.Location = new System.Drawing.Point(141, 71);
            this._eTimePreAlarm.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
            this._eTimePreAlarm.Name = "_eTimePreAlarm";
            this._eTimePreAlarm.Size = new System.Drawing.Size(120, 20);
            this._eTimePreAlarm.TabIndex = 2;
            this._eTimePreAlarm.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this._eTimePreAlarm.ValueChanged += new System.EventHandler(this._eTimePreAlarm_ValueChanged);
            // 
            // _lTimeOpen
            // 
            this._lTimeOpen.AutoSize = true;
            this._lTimeOpen.Location = new System.Drawing.Point(6, 47);
            this._lTimeOpen.Name = "_lTimeOpen";
            this._lTimeOpen.Size = new System.Drawing.Size(47, 13);
            this._lTimeOpen.TabIndex = 2;
            this._lTimeOpen.Text = "Open (s)";
            // 
            // _eTimeOpen
            // 
            this._eTimeOpen.Location = new System.Drawing.Point(141, 45);
            this._eTimeOpen.Maximum = new decimal(new int[] {
            1800,
            0,
            0,
            0});
            this._eTimeOpen.Name = "_eTimeOpen";
            this._eTimeOpen.Size = new System.Drawing.Size(120, 20);
            this._eTimeOpen.TabIndex = 1;
            this._eTimeOpen.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this._eTimeOpen.ValueChanged += new System.EventHandler(this._eTimeOpen_ValueChanged);
            // 
            // _eTimeUnlock
            // 
            this._eTimeUnlock.Location = new System.Drawing.Point(141, 19);
            this._eTimeUnlock.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
            this._eTimeUnlock.Name = "_eTimeUnlock";
            this._eTimeUnlock.Size = new System.Drawing.Size(120, 20);
            this._eTimeUnlock.TabIndex = 0;
            this._eTimeUnlock.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this._eTimeUnlock.ValueChanged += new System.EventHandler(this._eTimeUnlock_ValueChanged);
            // 
            // _lTimeUnlock
            // 
            this._lTimeUnlock.AutoSize = true;
            this._lTimeUnlock.Location = new System.Drawing.Point(6, 21);
            this._lTimeUnlock.Name = "_lTimeUnlock";
            this._lTimeUnlock.Size = new System.Drawing.Size(55, 13);
            this._lTimeUnlock.TabIndex = 0;
            this._lTimeUnlock.Text = "Unlock (s)";
            // 
            // _tpDoors
            // 
            this._tpDoors.BackColor = System.Drawing.SystemColors.Control;
            this._tpDoors.Controls.Add(this._dgMultiDoorElements);
            this._tpDoors.Controls.Add(this._bCreate);
            this._tpDoors.Controls.Add(this._bEdit);
            this._tpDoors.Controls.Add(this._bDelete);
            this._tpDoors.Location = new System.Drawing.Point(4, 22);
            this._tpDoors.Name = "_tpDoors";
            this._tpDoors.Padding = new System.Windows.Forms.Padding(3);
            this._tpDoors.Size = new System.Drawing.Size(622, 276);
            this._tpDoors.TabIndex = 1;
            this._tpDoors.Text = "Doors";
            // 
            // _dgMultiDoorElements
            // 
            this._dgMultiDoorElements.AllwaysRefreshOrder = false;
            this._dgMultiDoorElements.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgMultiDoorElements.CopyOnRightClick = true;
            // 
            // 
            // 
            this._dgMultiDoorElements.DataGrid.AllowUserToAddRows = false;
            this._dgMultiDoorElements.DataGrid.AllowUserToDeleteRows = false;
            this._dgMultiDoorElements.DataGrid.AllowUserToResizeRows = false;
            this._dgMultiDoorElements.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._dgMultiDoorElements.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgMultiDoorElements.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._dgMultiDoorElements.DataGrid.Name = "_dgvData";
            this._dgMultiDoorElements.DataGrid.RowHeadersVisible = false;
            this._dgMultiDoorElements.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._dgMultiDoorElements.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgMultiDoorElements.DataGrid.Size = new System.Drawing.Size(610, 232);
            this._dgMultiDoorElements.DataGrid.TabIndex = 0;
            this._dgMultiDoorElements.LocalizationHelper = null;
            this._dgMultiDoorElements.Location = new System.Drawing.Point(6, 3);
            this._dgMultiDoorElements.Name = "_dgMultiDoorElements";
            this._dgMultiDoorElements.Size = new System.Drawing.Size(610, 232);
            this._dgMultiDoorElements.TabIndex = 3;
            // 
            // _bCreate
            // 
            this._bCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreate.Location = new System.Drawing.Point(379, 241);
            this._bCreate.Name = "_bCreate";
            this._bCreate.Size = new System.Drawing.Size(75, 23);
            this._bCreate.TabIndex = 2;
            this._bCreate.Text = "Create";
            this._bCreate.UseVisualStyleBackColor = true;
            this._bCreate.Click += new System.EventHandler(this._bCreate_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(460, 241);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 1;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(541, 241);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(75, 23);
            this._bDelete.TabIndex = 0;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _tpObjectPlacement
            // 
            this._tpObjectPlacement.BackColor = System.Drawing.SystemColors.Control;
            this._tpObjectPlacement.Controls.Add(this._bRefresh);
            this._tpObjectPlacement.Controls.Add(this._lbUserFolders);
            this._tpObjectPlacement.Location = new System.Drawing.Point(4, 22);
            this._tpObjectPlacement.Name = "_tpObjectPlacement";
            this._tpObjectPlacement.Size = new System.Drawing.Size(622, 276);
            this._tpObjectPlacement.TabIndex = 3;
            this._tpObjectPlacement.Text = "Object placement";
            this._tpObjectPlacement.Enter += new System.EventHandler(this._tpObjectPlacement_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(544, 250);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 29;
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
            this._lbUserFolders.Size = new System.Drawing.Size(617, 225);
            this._lbUserFolders.TabIndex = 30;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Size = new System.Drawing.Size(622, 276);
            this._tpReferencedBy.TabIndex = 4;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Size = new System.Drawing.Size(622, 276);
            this._tpDescription.TabIndex = 2;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(0, 0);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(622, 276);
            this._eDescription.TabIndex = 7;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(567, 346);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(486, 346);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(405, 346);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 5;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // NCASMultiDoorEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(654, 381);
            this.Controls.Add(this._bApply);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._tcMultiDoor);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._lName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(670, 420);
            this.Name = "NCASMultiDoorEditForm";
            this.Text = "NCASMultiDoorEditForm";
            this._tcMultiDoor.ResumeLayout(false);
            this._tpSettings.ResumeLayout(false);
            this._tpSettings.PerformLayout();
            this._tpApas.ResumeLayout(false);
            this._gbSensorOpenDoor.ResumeLayout(false);
            this._gbExtraElectricStrike.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._eExtraElectricStrikeImpulseDelay)).EndInit();
            this._gbElectricStike.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._eElectricStrikeImpulseDelay)).EndInit();
            this._gbDoorsDelays.ResumeLayout(false);
            this._gbDoorsDelays.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayBeforeLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayBeforeClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayBeforeUnlock)).EndInit();
            this._gbDoorsTimes.ResumeLayout(false);
            this._gbDoorsTimes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eTimePreAlarm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eTimeOpen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eTimeUnlock)).EndInit();
            this._tpDoors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgMultiDoorElements.DataGrid)).EndInit();
            this._tpObjectPlacement.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.TabControl _tcMultiDoor;
        private System.Windows.Forms.TabPage _tpSettings;
        private System.Windows.Forms.TabPage _tpDoors;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.Label _lCardReader;
        private Contal.IwQuick.UI.TextBoxMenu _eCardReader;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
        private System.Windows.Forms.Label _lType;
        private System.Windows.Forms.ComboBox _cbType;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Button _bCreate;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.TabPage _tpObjectPlacement;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private Contal.Cgp.Components.CgpDataGridView _dgMultiDoorElements;
        private System.Windows.Forms.GroupBox _gbDoorsDelays;
        private System.Windows.Forms.Label _lDelayBeforeLock;
        private System.Windows.Forms.NumericUpDown _eDelayBeforeLock;
        private System.Windows.Forms.Label _lDelayBeforeClose;
        private System.Windows.Forms.Label _lDelayBeforeUnlock;
        private System.Windows.Forms.NumericUpDown _eDelayBeforeClose;
        private System.Windows.Forms.NumericUpDown _eDelayBeforeUnlock;
        private System.Windows.Forms.GroupBox _gbDoorsTimes;
        private System.Windows.Forms.Label _lTimePreAlarm;
        private System.Windows.Forms.NumericUpDown _eTimePreAlarm;
        private System.Windows.Forms.Label _lTimeOpen;
        private System.Windows.Forms.NumericUpDown _eTimeOpen;
        private System.Windows.Forms.NumericUpDown _eTimeUnlock;
        private System.Windows.Forms.Label _lTimeUnlock;
        private System.Windows.Forms.TabPage _tpApas;
        private System.Windows.Forms.GroupBox _gbElectricStike;
        private System.Windows.Forms.NumericUpDown _eElectricStrikeImpulseDelay;
        private System.Windows.Forms.CheckBox _chbElectricStrikeImpulse;
        private System.Windows.Forms.GroupBox _gbSensorOpenDoor;
        private System.Windows.Forms.GroupBox _gbExtraElectricStrike;
        private System.Windows.Forms.NumericUpDown _eExtraElectricStrikeImpulseDelay;
        private System.Windows.Forms.CheckBox _chbExtraElectricStrikeImpulse;
        private System.Windows.Forms.CheckBox _chbOpenDoorBalanced;
        private System.Windows.Forms.CheckBox _chbOpenDoorInverted;
        private System.Windows.Forms.Label _lAlarmArea;
        private Contal.IwQuick.UI.TextBoxMenu _eBlockAlarmArea;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
    }
}
