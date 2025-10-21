namespace Contal.Cgp.NCAS.Client
{
    partial class NCASOutputEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASOutputEditForm));
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._tcOutput = new System.Windows.Forms.TabControl();
            this._tpSettings = new System.Windows.Forms.TabPage();
            this._cbInverted = new System.Windows.Forms.CheckBox();
            this._cbSendRealStateChanges = new System.Windows.Forms.CheckBox();
            this._gbTimingSettings = new System.Windows.Forms.GroupBox();
            this._eSettingsPulseDelay = new System.Windows.Forms.TextBox();
            this._lSettingsPulseDelay = new System.Windows.Forms.Label();
            this._eSettingsPulseLength = new System.Windows.Forms.TextBox();
            this._lSettingsPulseLength = new System.Windows.Forms.Label();
            this._chbForcedToOff = new System.Windows.Forms.CheckBox();
            this._eSettingsDelayToOff = new System.Windows.Forms.TextBox();
            this._lSettingsDelayToOff = new System.Windows.Forms.Label();
            this._eSettingsDelayToOn = new System.Windows.Forms.TextBox();
            this._lSettingsDelayToOn = new System.Windows.Forms.Label();
            this._lOutputType = new System.Windows.Forms.Label();
            this._cbOutPutType = new System.Windows.Forms.ComboBox();
            this._tpAlarmSettings = new System.Windows.Forms.TabPage();
            this._gbAlarmPresentationGroup = new System.Windows.Forms.GroupBox();
            this._tbmAlarmPresentationGroup = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate = new System.Windows.Forms.ToolStripMenuItem();
            this._lName1 = new System.Windows.Forms.Label();
            this._gbEnableAlarms = new System.Windows.Forms.GroupBox();
            this._chbAlarmCBOOn = new System.Windows.Forms.CheckBox();
            this._tpOutputControl = new System.Windows.Forms.TabPage();
            this._lICCU = new System.Windows.Forms.Label();
            this._gbControlObject = new System.Windows.Forms.GroupBox();
            this._tbmControlObject = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove1 = new System.Windows.Forms.ToolStripMenuItem();
            this._lName2 = new System.Windows.Forms.Label();
            this._cbOutputControlType = new System.Windows.Forms.ComboBox();
            this._lControlType = new System.Windows.Forms.Label();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._lDoorEnvironment = new System.Windows.Forms.Label();
            this._lRealStatusOnOff = new System.Windows.Forms.Label();
            this._eRealStatusOnOff = new System.Windows.Forms.TextBox();
            this._lStatusOnOff = new System.Windows.Forms.Label();
            this._eStatusOnOff = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._itbDoorEnvironment = new Contal.IwQuick.UI.ImageTextBox();
            this._itbDcuName = new Contal.IwQuick.UI.ImageTextBox();
            this._nudOutputIndex = new System.Windows.Forms.NumericUpDown();
            this._eOutputIndex = new System.Windows.Forms.TextBox();
            this._lOutputIndex = new System.Windows.Forms.Label();
            this._lParent = new System.Windows.Forms.Label();
            this._tbmDCU = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify5 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove5 = new System.Windows.Forms.ToolStripMenuItem();
            this._panelBack = new System.Windows.Forms.Panel();
            this._bApply = new System.Windows.Forms.Button();
            this._tcOutput.SuspendLayout();
            this._tpSettings.SuspendLayout();
            this._gbTimingSettings.SuspendLayout();
            this._tpAlarmSettings.SuspendLayout();
            this._gbAlarmPresentationGroup.SuspendLayout();
            this._gbEnableAlarms.SuspendLayout();
            this._tpOutputControl.SuspendLayout();
            this._gbControlObject.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudOutputIndex)).BeginInit();
            this._panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(479, 436);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(398, 436);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this.OkClick);
            // 
            // _tcOutput
            // 
            this._tcOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcOutput.Controls.Add(this._tpSettings);
            this._tcOutput.Controls.Add(this._tpAlarmSettings);
            this._tcOutput.Controls.Add(this._tpOutputControl);
            this._tcOutput.Controls.Add(this._tpUserFolders);
            this._tcOutput.Controls.Add(this._tpReferencedBy);
            this._tcOutput.Controls.Add(this._tpDescription);
            this._tcOutput.Location = new System.Drawing.Point(12, 201);
            this._tcOutput.Name = "_tcOutput";
            this._tcOutput.SelectedIndex = 0;
            this._tcOutput.Size = new System.Drawing.Size(547, 229);
            this._tcOutput.TabIndex = 1;
            this._tcOutput.TabStop = false;
            // 
            // _tpSettings
            // 
            this._tpSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpSettings.Controls.Add(this._cbInverted);
            this._tpSettings.Controls.Add(this._cbSendRealStateChanges);
            this._tpSettings.Controls.Add(this._gbTimingSettings);
            this._tpSettings.Controls.Add(this._lOutputType);
            this._tpSettings.Controls.Add(this._cbOutPutType);
            this._tpSettings.Location = new System.Drawing.Point(4, 22);
            this._tpSettings.Name = "_tpSettings";
            this._tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpSettings.Size = new System.Drawing.Size(539, 203);
            this._tpSettings.TabIndex = 0;
            this._tpSettings.Text = "Settings";
            // 
            // _cbInverted
            // 
            this._cbInverted.AutoSize = true;
            this._cbInverted.Location = new System.Drawing.Point(3, 142);
            this._cbInverted.Name = "_cbInverted";
            this._cbInverted.Size = new System.Drawing.Size(65, 17);
            this._cbInverted.TabIndex = 4;
            this._cbInverted.Text = "Inverted";
            this._cbInverted.UseVisualStyleBackColor = true;
            this._cbInverted.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _cbSendRealStateChanges
            // 
            this._cbSendRealStateChanges.AutoSize = true;
            this._cbSendRealStateChanges.Location = new System.Drawing.Point(3, 165);
            this._cbSendRealStateChanges.Name = "_cbSendRealStateChanges";
            this._cbSendRealStateChanges.Size = new System.Drawing.Size(174, 17);
            this._cbSendRealStateChanges.TabIndex = 3;
            this._cbSendRealStateChanges.Text = "Send output real state changes";
            this._cbSendRealStateChanges.UseVisualStyleBackColor = true;
            this._cbSendRealStateChanges.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _gbTimingSettings
            // 
            this._gbTimingSettings.Controls.Add(this._eSettingsPulseDelay);
            this._gbTimingSettings.Controls.Add(this._lSettingsPulseDelay);
            this._gbTimingSettings.Controls.Add(this._eSettingsPulseLength);
            this._gbTimingSettings.Controls.Add(this._lSettingsPulseLength);
            this._gbTimingSettings.Controls.Add(this._chbForcedToOff);
            this._gbTimingSettings.Controls.Add(this._eSettingsDelayToOff);
            this._gbTimingSettings.Controls.Add(this._lSettingsDelayToOff);
            this._gbTimingSettings.Controls.Add(this._eSettingsDelayToOn);
            this._gbTimingSettings.Controls.Add(this._lSettingsDelayToOn);
            this._gbTimingSettings.Location = new System.Drawing.Point(3, 33);
            this._gbTimingSettings.Name = "_gbTimingSettings";
            this._gbTimingSettings.Size = new System.Drawing.Size(420, 103);
            this._gbTimingSettings.TabIndex = 2;
            this._gbTimingSettings.TabStop = false;
            this._gbTimingSettings.Text = "Timing settings";
            // 
            // _eSettingsPulseDelay
            // 
            this._eSettingsPulseDelay.Location = new System.Drawing.Point(174, 72);
            this._eSettingsPulseDelay.Name = "_eSettingsPulseDelay";
            this._eSettingsPulseDelay.Size = new System.Drawing.Size(114, 20);
            this._eSettingsPulseDelay.TabIndex = 7;
            this._eSettingsPulseDelay.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lSettingsPulseDelay
            // 
            this._lSettingsPulseDelay.AutoSize = true;
            this._lSettingsPulseDelay.Location = new System.Drawing.Point(171, 55);
            this._lSettingsPulseDelay.Name = "_lSettingsPulseDelay";
            this._lSettingsPulseDelay.Size = new System.Drawing.Size(61, 13);
            this._lSettingsPulseDelay.TabIndex = 6;
            this._lSettingsPulseDelay.Text = "Pulse delay";
            // 
            // _eSettingsPulseLength
            // 
            this._eSettingsPulseLength.Location = new System.Drawing.Point(9, 72);
            this._eSettingsPulseLength.Name = "_eSettingsPulseLength";
            this._eSettingsPulseLength.Size = new System.Drawing.Size(114, 20);
            this._eSettingsPulseLength.TabIndex = 5;
            this._eSettingsPulseLength.TextChanged += new System.EventHandler(this._eSettingsPulseLength_TextChanged);
            // 
            // _lSettingsPulseLength
            // 
            this._lSettingsPulseLength.AutoSize = true;
            this._lSettingsPulseLength.Location = new System.Drawing.Point(6, 56);
            this._lSettingsPulseLength.Name = "_lSettingsPulseLength";
            this._lSettingsPulseLength.Size = new System.Drawing.Size(65, 13);
            this._lSettingsPulseLength.TabIndex = 4;
            this._lSettingsPulseLength.Text = "Pulse length";
            // 
            // _chbForcedToOff
            // 
            this._chbForcedToOff.AutoSize = true;
            this._chbForcedToOff.Location = new System.Drawing.Point(305, 74);
            this._chbForcedToOff.Name = "_chbForcedToOff";
            this._chbForcedToOff.Size = new System.Drawing.Size(86, 17);
            this._chbForcedToOff.TabIndex = 8;
            this._chbForcedToOff.Text = "Forced to off";
            this._chbForcedToOff.UseVisualStyleBackColor = true;
            this._chbForcedToOff.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _eSettingsDelayToOff
            // 
            this._eSettingsDelayToOff.Location = new System.Drawing.Point(174, 32);
            this._eSettingsDelayToOff.Name = "_eSettingsDelayToOff";
            this._eSettingsDelayToOff.Size = new System.Drawing.Size(114, 20);
            this._eSettingsDelayToOff.TabIndex = 3;
            this._eSettingsDelayToOff.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lSettingsDelayToOff
            // 
            this._lSettingsDelayToOff.AutoSize = true;
            this._lSettingsDelayToOff.Location = new System.Drawing.Point(171, 15);
            this._lSettingsDelayToOff.Name = "_lSettingsDelayToOff";
            this._lSettingsDelayToOff.Size = new System.Drawing.Size(61, 13);
            this._lSettingsDelayToOff.TabIndex = 2;
            this._lSettingsDelayToOff.Text = "Delay to off";
            // 
            // _eSettingsDelayToOn
            // 
            this._eSettingsDelayToOn.Location = new System.Drawing.Point(9, 32);
            this._eSettingsDelayToOn.Name = "_eSettingsDelayToOn";
            this._eSettingsDelayToOn.Size = new System.Drawing.Size(114, 20);
            this._eSettingsDelayToOn.TabIndex = 1;
            this._eSettingsDelayToOn.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lSettingsDelayToOn
            // 
            this._lSettingsDelayToOn.AutoSize = true;
            this._lSettingsDelayToOn.Location = new System.Drawing.Point(6, 16);
            this._lSettingsDelayToOn.Name = "_lSettingsDelayToOn";
            this._lSettingsDelayToOn.Size = new System.Drawing.Size(61, 13);
            this._lSettingsDelayToOn.TabIndex = 0;
            this._lSettingsDelayToOn.Text = "Delay to on";
            // 
            // _lOutputType
            // 
            this._lOutputType.AutoSize = true;
            this._lOutputType.Location = new System.Drawing.Point(2, 9);
            this._lOutputType.Name = "_lOutputType";
            this._lOutputType.Size = new System.Drawing.Size(62, 13);
            this._lOutputType.TabIndex = 0;
            this._lOutputType.Text = "Output type";
            // 
            // _cbOutPutType
            // 
            this._cbOutPutType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbOutPutType.FormattingEnabled = true;
            this._cbOutPutType.Location = new System.Drawing.Point(149, 6);
            this._cbOutPutType.Name = "_cbOutPutType";
            this._cbOutPutType.Size = new System.Drawing.Size(264, 21);
            this._cbOutPutType.TabIndex = 1;
            this._cbOutPutType.SelectedIndexChanged += new System.EventHandler(this._cbOutPutType_SelectedIndexChanged);
            // 
            // _tpAlarmSettings
            // 
            this._tpAlarmSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpAlarmSettings.Controls.Add(this._gbAlarmPresentationGroup);
            this._tpAlarmSettings.Controls.Add(this._gbEnableAlarms);
            this._tpAlarmSettings.Location = new System.Drawing.Point(4, 22);
            this._tpAlarmSettings.Name = "_tpAlarmSettings";
            this._tpAlarmSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpAlarmSettings.Size = new System.Drawing.Size(539, 203);
            this._tpAlarmSettings.TabIndex = 1;
            this._tpAlarmSettings.Text = "Alarm Settings";
            // 
            // _gbAlarmPresentationGroup
            // 
            this._gbAlarmPresentationGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbAlarmPresentationGroup.Controls.Add(this._tbmAlarmPresentationGroup);
            this._gbAlarmPresentationGroup.Controls.Add(this._lName1);
            this._gbAlarmPresentationGroup.Location = new System.Drawing.Point(6, 6);
            this._gbAlarmPresentationGroup.Name = "_gbAlarmPresentationGroup";
            this._gbAlarmPresentationGroup.Size = new System.Drawing.Size(367, 53);
            this._gbAlarmPresentationGroup.TabIndex = 0;
            this._gbAlarmPresentationGroup.TabStop = false;
            this._gbAlarmPresentationGroup.Text = "Alarm PG";
            // 
            // _tbmAlarmPresentationGroup
            // 
            this._tbmAlarmPresentationGroup.AllowDrop = true;
            this._tbmAlarmPresentationGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmPresentationGroup.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmAlarmPresentationGroup.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmPresentationGroup.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmAlarmPresentationGroup.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmAlarmPresentationGroup.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmPresentationGroup.Button.Image")));
            this._tbmAlarmPresentationGroup.Button.Location = new System.Drawing.Point(265, 0);
            this._tbmAlarmPresentationGroup.Button.Name = "_bMenu";
            this._tbmAlarmPresentationGroup.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmAlarmPresentationGroup.Button.TabIndex = 3;
            this._tbmAlarmPresentationGroup.Button.UseVisualStyleBackColor = false;
            this._tbmAlarmPresentationGroup.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmAlarmPresentationGroup.ButtonDefaultBehaviour = true;
            this._tbmAlarmPresentationGroup.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmAlarmPresentationGroup.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmPresentationGroup.ButtonImage")));
            // 
            // 
            // 
            this._tbmAlarmPresentationGroup.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiRemove,
            this._tsiCreate});
            this._tbmAlarmPresentationGroup.ButtonPopupMenu.Name = "";
            this._tbmAlarmPresentationGroup.ButtonPopupMenu.Size = new System.Drawing.Size(118, 70);
            this._tbmAlarmPresentationGroup.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmAlarmPresentationGroup.ButtonShowImage = true;
            this._tbmAlarmPresentationGroup.ButtonSizeHeight = 20;
            this._tbmAlarmPresentationGroup.ButtonSizeWidth = 20;
            this._tbmAlarmPresentationGroup.ButtonText = "";
            this._tbmAlarmPresentationGroup.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmPresentationGroup.HoverTime = 500;
            // 
            // 
            // 
            this._tbmAlarmPresentationGroup.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmPresentationGroup.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmAlarmPresentationGroup.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmAlarmPresentationGroup.ImageTextBox.ContextMenuStrip = this._tbmAlarmPresentationGroup.ButtonPopupMenu;
            this._tbmAlarmPresentationGroup.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmPresentationGroup.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmPresentationGroup.ImageTextBox.Image")));
            this._tbmAlarmPresentationGroup.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmAlarmPresentationGroup.ImageTextBox.Name = "_textBox";
            this._tbmAlarmPresentationGroup.ImageTextBox.NoTextNoImage = true;
            this._tbmAlarmPresentationGroup.ImageTextBox.ReadOnly = true;
            this._tbmAlarmPresentationGroup.ImageTextBox.Size = new System.Drawing.Size(265, 20);
            this._tbmAlarmPresentationGroup.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.Size = new System.Drawing.Size(263, 13);
            this._tbmAlarmPresentationGroup.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmAlarmPresentationGroup.ImageTextBox.UseImage = true;
            this._tbmAlarmPresentationGroup.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            this._tbmAlarmPresentationGroup.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmAlarmPresentationGroup_DoubleClick);
            this._tbmAlarmPresentationGroup.Location = new System.Drawing.Point(70, 20);
            this._tbmAlarmPresentationGroup.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmAlarmPresentationGroup.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmAlarmPresentationGroup.Name = "_tbmAlarmPresentationGroup";
            this._tbmAlarmPresentationGroup.Size = new System.Drawing.Size(285, 22);
            this._tbmAlarmPresentationGroup.TabIndex = 1;
            this._tbmAlarmPresentationGroup.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmPresentationGroup.TextImage")));
            this._tbmAlarmPresentationGroup.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmAlarmPresentationGroup_DragOver);
            this._tbmAlarmPresentationGroup.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmAlarmPresentationGroup_DragDrop);
            this._tbmAlarmPresentationGroup.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmAlarmPresentationGroup_ButtonPopupMenuItemClick);
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
            // _lName1
            // 
            this._lName1.AutoSize = true;
            this._lName1.Location = new System.Drawing.Point(6, 22);
            this._lName1.Name = "_lName1";
            this._lName1.Size = new System.Drawing.Size(35, 13);
            this._lName1.TabIndex = 0;
            this._lName1.Text = "Name";
            // 
            // _gbEnableAlarms
            // 
            this._gbEnableAlarms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbEnableAlarms.Controls.Add(this._chbAlarmCBOOn);
            this._gbEnableAlarms.Location = new System.Drawing.Point(6, 65);
            this._gbEnableAlarms.Name = "_gbEnableAlarms";
            this._gbEnableAlarms.Size = new System.Drawing.Size(367, 44);
            this._gbEnableAlarms.TabIndex = 1;
            this._gbEnableAlarms.TabStop = false;
            this._gbEnableAlarms.Text = "Enable alarms";
            // 
            // _chbAlarmCBOOn
            // 
            this._chbAlarmCBOOn.AutoSize = true;
            this._chbAlarmCBOOn.Location = new System.Drawing.Point(6, 19);
            this._chbAlarmCBOOn.Name = "_chbAlarmCBOOn";
            this._chbAlarmCBOOn.Size = new System.Drawing.Size(40, 17);
            this._chbAlarmCBOOn.TabIndex = 0;
            this._chbAlarmCBOOn.Text = "On";
            this._chbAlarmCBOOn.UseVisualStyleBackColor = true;
            this._chbAlarmCBOOn.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tpOutputControl
            // 
            this._tpOutputControl.BackColor = System.Drawing.SystemColors.Control;
            this._tpOutputControl.Controls.Add(this._lICCU);
            this._tpOutputControl.Controls.Add(this._gbControlObject);
            this._tpOutputControl.Controls.Add(this._cbOutputControlType);
            this._tpOutputControl.Controls.Add(this._lControlType);
            this._tpOutputControl.Location = new System.Drawing.Point(4, 22);
            this._tpOutputControl.Name = "_tpOutputControl";
            this._tpOutputControl.Padding = new System.Windows.Forms.Padding(3);
            this._tpOutputControl.Size = new System.Drawing.Size(539, 203);
            this._tpOutputControl.TabIndex = 4;
            this._tpOutputControl.Text = "Output control";
            // 
            // _lICCU
            // 
            this._lICCU.AutoSize = true;
            this._lICCU.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lICCU.Location = new System.Drawing.Point(376, 59);
            this._lICCU.Name = "_lICCU";
            this._lICCU.Size = new System.Drawing.Size(35, 13);
            this._lICCU.TabIndex = 4;
            this._lICCU.Text = "iCCU";
            this._lICCU.Visible = false;
            // 
            // _gbControlObject
            // 
            this._gbControlObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbControlObject.Controls.Add(this._tbmControlObject);
            this._gbControlObject.Controls.Add(this._lName2);
            this._gbControlObject.Location = new System.Drawing.Point(3, 33);
            this._gbControlObject.Name = "_gbControlObject";
            this._gbControlObject.Size = new System.Drawing.Size(367, 53);
            this._gbControlObject.TabIndex = 2;
            this._gbControlObject.TabStop = false;
            this._gbControlObject.Text = "Controled object";
            // 
            // _tbmControlObject
            // 
            this._tbmControlObject.AllowDrop = true;
            this._tbmControlObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmControlObject.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmControlObject.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmControlObject.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmControlObject.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmControlObject.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmControlObject.Button.Image")));
            this._tbmControlObject.Button.Location = new System.Drawing.Point(265, 0);
            this._tbmControlObject.Button.Name = "_bMenu";
            this._tbmControlObject.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmControlObject.Button.TabIndex = 3;
            this._tbmControlObject.Button.UseVisualStyleBackColor = false;
            this._tbmControlObject.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmControlObject.ButtonDefaultBehaviour = true;
            this._tbmControlObject.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmControlObject.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmControlObject.ButtonImage")));
            // 
            // 
            // 
            this._tbmControlObject.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify1,
            this._tsiRemove1});
            this._tbmControlObject.ButtonPopupMenu.Name = "";
            this._tbmControlObject.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmControlObject.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmControlObject.ButtonShowImage = true;
            this._tbmControlObject.ButtonSizeHeight = 20;
            this._tbmControlObject.ButtonSizeWidth = 20;
            this._tbmControlObject.ButtonText = "";
            this._tbmControlObject.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmControlObject.HoverTime = 500;
            // 
            // 
            // 
            this._tbmControlObject.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmControlObject.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmControlObject.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmControlObject.ImageTextBox.ContextMenuStrip = this._tbmControlObject.ButtonPopupMenu;
            this._tbmControlObject.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmControlObject.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmControlObject.ImageTextBox.Image")));
            this._tbmControlObject.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmControlObject.ImageTextBox.Name = "_textBox";
            this._tbmControlObject.ImageTextBox.NoTextNoImage = true;
            this._tbmControlObject.ImageTextBox.ReadOnly = true;
            this._tbmControlObject.ImageTextBox.Size = new System.Drawing.Size(265, 20);
            this._tbmControlObject.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmControlObject.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmControlObject.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmControlObject.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmControlObject.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmControlObject.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmControlObject.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmControlObject.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmControlObject.ImageTextBox.TextBox.Size = new System.Drawing.Size(263, 13);
            this._tbmControlObject.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmControlObject.ImageTextBox.UseImage = true;
            this._tbmControlObject.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmControlObject.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmControlObject_DoubleClick);
            this._tbmControlObject.Location = new System.Drawing.Point(73, 20);
            this._tbmControlObject.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmControlObject.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmControlObject.Name = "_tbmControlObject";
            this._tbmControlObject.Size = new System.Drawing.Size(285, 22);
            this._tbmControlObject.TabIndex = 1;
            this._tbmControlObject.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmControlObject.TextImage")));
            this._tbmControlObject.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmControlObject_DragOver);
            this._tbmControlObject.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmControlObject_DragDrop);
            this._tbmControlObject.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmControlObject_ButtonPopupMenuItemClick);
            // 
            // _tsiModify1
            // 
            this._tsiModify1.Name = "_tsiModify1";
            this._tsiModify1.Size = new System.Drawing.Size(117, 22);
            this._tsiModify1.Text = "Modify";
            // 
            // _tsiRemove1
            // 
            this._tsiRemove1.Name = "_tsiRemove1";
            this._tsiRemove1.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove1.Text = "Remove";
            // 
            // _lName2
            // 
            this._lName2.AutoSize = true;
            this._lName2.Location = new System.Drawing.Point(6, 26);
            this._lName2.Name = "_lName2";
            this._lName2.Size = new System.Drawing.Size(35, 13);
            this._lName2.TabIndex = 0;
            this._lName2.Text = "Name";
            // 
            // _cbOutputControlType
            // 
            this._cbOutputControlType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cbOutputControlType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbOutputControlType.FormattingEnabled = true;
            this._cbOutputControlType.Location = new System.Drawing.Point(123, 6);
            this._cbOutputControlType.Name = "_cbOutputControlType";
            this._cbOutputControlType.Size = new System.Drawing.Size(237, 21);
            this._cbOutputControlType.TabIndex = 1;
            this._cbOutputControlType.TabStop = false;
            this._cbOutputControlType.SelectedIndexChanged += new System.EventHandler(this._cbOutputControlType_SelectedIndexChanged);
            // 
            // _lControlType
            // 
            this._lControlType.AutoSize = true;
            this._lControlType.Location = new System.Drawing.Point(6, 9);
            this._lControlType.Name = "_lControlType";
            this._lControlType.Size = new System.Drawing.Size(63, 13);
            this._lControlType.TabIndex = 0;
            this._lControlType.Text = "Control type";
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 22);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(539, 203);
            this._tpUserFolders.TabIndex = 6;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(461, 177);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 4;
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
            this._lbUserFolders.Location = new System.Drawing.Point(0, 0);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(539, 160);
            this._lbUserFolders.TabIndex = 40;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(3);
            this._tpReferencedBy.Size = new System.Drawing.Size(539, 203);
            this._tpReferencedBy.TabIndex = 5;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(3);
            this._tpDescription.Size = new System.Drawing.Size(539, 203);
            this._tpDescription.TabIndex = 3;
            this._tpDescription.Text = "Descripton";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(3, 3);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(533, 197);
            this._eDescription.TabIndex = 4;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _lDoorEnvironment
            // 
            this._lDoorEnvironment.AutoSize = true;
            this._lDoorEnvironment.Location = new System.Drawing.Point(16, 96);
            this._lDoorEnvironment.Name = "_lDoorEnvironment";
            this._lDoorEnvironment.Size = new System.Drawing.Size(91, 13);
            this._lDoorEnvironment.TabIndex = 8;
            this._lDoorEnvironment.Text = "Door environment";
            // 
            // _lRealStatusOnOff
            // 
            this._lRealStatusOnOff.AutoSize = true;
            this._lRealStatusOnOff.Location = new System.Drawing.Point(16, 148);
            this._lRealStatusOnOff.Name = "_lRealStatusOnOff";
            this._lRealStatusOnOff.Size = new System.Drawing.Size(60, 13);
            this._lRealStatusOnOff.TabIndex = 12;
            this._lRealStatusOnOff.Text = "Real status";
            // 
            // _eRealStatusOnOff
            // 
            this._eRealStatusOnOff.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eRealStatusOnOff.BackColor = System.Drawing.SystemColors.Window;
            this._eRealStatusOnOff.Location = new System.Drawing.Point(127, 145);
            this._eRealStatusOnOff.Name = "_eRealStatusOnOff";
            this._eRealStatusOnOff.ReadOnly = true;
            this._eRealStatusOnOff.Size = new System.Drawing.Size(302, 20);
            this._eRealStatusOnOff.TabIndex = 13;
            // 
            // _lStatusOnOff
            // 
            this._lStatusOnOff.AutoSize = true;
            this._lStatusOnOff.Location = new System.Drawing.Point(16, 122);
            this._lStatusOnOff.Name = "_lStatusOnOff";
            this._lStatusOnOff.Size = new System.Drawing.Size(37, 13);
            this._lStatusOnOff.TabIndex = 10;
            this._lStatusOnOff.Text = "Status";
            // 
            // _eStatusOnOff
            // 
            this._eStatusOnOff.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eStatusOnOff.BackColor = System.Drawing.SystemColors.Window;
            this._eStatusOnOff.Location = new System.Drawing.Point(127, 119);
            this._eStatusOnOff.Name = "_eStatusOnOff";
            this._eStatusOnOff.ReadOnly = true;
            this._eStatusOnOff.Size = new System.Drawing.Size(302, 20);
            this._eStatusOnOff.TabIndex = 11;
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(16, 15);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(127, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(302, 20);
            this._eName.TabIndex = 1;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _itbDoorEnvironment
            // 
            this._itbDoorEnvironment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbDoorEnvironment.BackColor = System.Drawing.SystemColors.Info;
            this._itbDoorEnvironment.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._itbDoorEnvironment.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbDoorEnvironment.Image = null;
            this._itbDoorEnvironment.Location = new System.Drawing.Point(127, 91);
            this._itbDoorEnvironment.Name = "_itbDoorEnvironment";
            this._itbDoorEnvironment.NoTextNoImage = true;
            this._itbDoorEnvironment.ReadOnly = true;
            this._itbDoorEnvironment.Size = new System.Drawing.Size(302, 20);
            this._itbDoorEnvironment.TabIndex = 8;
            this._itbDoorEnvironment.Tag = "Reference";
            // 
            // 
            // 
            this._itbDoorEnvironment.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbDoorEnvironment.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._itbDoorEnvironment.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._itbDoorEnvironment.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbDoorEnvironment.TextBox.Location = new System.Drawing.Point(1, 2);
            this._itbDoorEnvironment.TextBox.Name = "_tbTextBox";
            this._itbDoorEnvironment.TextBox.ReadOnly = true;
            this._itbDoorEnvironment.TextBox.Size = new System.Drawing.Size(300, 13);
            this._itbDoorEnvironment.TextBox.TabIndex = 2;
            this._itbDoorEnvironment.UseImage = true;
            this._itbDoorEnvironment.DoubleClick += new System.EventHandler(this._itbDoorEnvironment_DoubleClick);
            // 
            // _itbDcuName
            // 
            this._itbDcuName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbDcuName.BackColor = System.Drawing.SystemColors.Info;
            this._itbDcuName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._itbDcuName.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbDcuName.Image = null;
            this._itbDcuName.Location = new System.Drawing.Point(127, 65);
            this._itbDcuName.Name = "_itbDcuName";
            this._itbDcuName.NoTextNoImage = true;
            this._itbDcuName.ReadOnly = true;
            this._itbDcuName.Size = new System.Drawing.Size(302, 20);
            this._itbDcuName.TabIndex = 6;
            this._itbDcuName.Tag = "Reference";
            // 
            // 
            // 
            this._itbDcuName.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbDcuName.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._itbDcuName.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._itbDcuName.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbDcuName.TextBox.Location = new System.Drawing.Point(1, 2);
            this._itbDcuName.TextBox.Name = "_tbTextBox";
            this._itbDcuName.TextBox.ReadOnly = true;
            this._itbDcuName.TextBox.Size = new System.Drawing.Size(300, 13);
            this._itbDcuName.TextBox.TabIndex = 2;
            this._itbDcuName.UseImage = true;
            this._itbDcuName.DoubleClick += new System.EventHandler(this._itbDcuName_DoubleClick);
            // 
            // _nudOutputIndex
            // 
            this._nudOutputIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._nudOutputIndex.Location = new System.Drawing.Point(127, 39);
            this._nudOutputIndex.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudOutputIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudOutputIndex.Name = "_nudOutputIndex";
            this._nudOutputIndex.Size = new System.Drawing.Size(302, 20);
            this._nudOutputIndex.TabIndex = 4;
            this._nudOutputIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudOutputIndex.Visible = false;
            // 
            // _eOutputIndex
            // 
            this._eOutputIndex.Location = new System.Drawing.Point(127, 39);
            this._eOutputIndex.Name = "_eOutputIndex";
            this._eOutputIndex.ReadOnly = true;
            this._eOutputIndex.Size = new System.Drawing.Size(100, 20);
            this._eOutputIndex.TabIndex = 3;
            // 
            // _lOutputIndex
            // 
            this._lOutputIndex.AutoSize = true;
            this._lOutputIndex.Location = new System.Drawing.Point(16, 42);
            this._lOutputIndex.Name = "_lOutputIndex";
            this._lOutputIndex.Size = new System.Drawing.Size(33, 13);
            this._lOutputIndex.TabIndex = 2;
            this._lOutputIndex.Text = "Index";
            // 
            // _lParent
            // 
            this._lParent.AutoSize = true;
            this._lParent.Location = new System.Drawing.Point(16, 68);
            this._lParent.Name = "_lParent";
            this._lParent.Size = new System.Drawing.Size(88, 13);
            this._lParent.TabIndex = 5;
            this._lParent.Text = "ParentCcuOrDcu";
            // 
            // _tbmDCU
            // 
            this._tbmDCU.AllowDrop = true;
            this._tbmDCU.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmDCU.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmDCU.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmDCU.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmDCU.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmDCU.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmDCU.Button.Image")));
            this._tbmDCU.Button.Location = new System.Drawing.Point(282, 0);
            this._tbmDCU.Button.Name = "_bMenu";
            this._tbmDCU.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmDCU.Button.TabIndex = 3;
            this._tbmDCU.Button.UseVisualStyleBackColor = false;
            this._tbmDCU.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmDCU.ButtonDefaultBehaviour = true;
            this._tbmDCU.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmDCU.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmDCU.ButtonImage")));
            // 
            // 
            // 
            this._tbmDCU.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify5,
            this._tsiRemove5});
            this._tbmDCU.ButtonPopupMenu.Name = "";
            this._tbmDCU.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmDCU.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmDCU.ButtonShowImage = true;
            this._tbmDCU.ButtonSizeHeight = 20;
            this._tbmDCU.ButtonSizeWidth = 20;
            this._tbmDCU.ButtonText = "";
            this._tbmDCU.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmDCU.HoverTime = 500;
            // 
            // 
            // 
            this._tbmDCU.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmDCU.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._tbmDCU.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmDCU.ImageTextBox.ContextMenuStrip = this._tbmDCU.ButtonPopupMenu;
            this._tbmDCU.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmDCU.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmDCU.ImageTextBox.Image")));
            this._tbmDCU.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmDCU.ImageTextBox.Name = "_textBox";
            this._tbmDCU.ImageTextBox.NoTextNoImage = true;
            this._tbmDCU.ImageTextBox.ReadOnly = true;
            this._tbmDCU.ImageTextBox.Size = new System.Drawing.Size(282, 20);
            this._tbmDCU.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmDCU.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmDCU.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmDCU.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmDCU.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmDCU.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmDCU.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmDCU.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmDCU.ImageTextBox.TextBox.Size = new System.Drawing.Size(280, 13);
            this._tbmDCU.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmDCU.ImageTextBox.UseImage = true;
            this._tbmDCU.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmDCU_TextBox_DoubleClick);
            this._tbmDCU.Location = new System.Drawing.Point(127, 65);
            this._tbmDCU.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmDCU.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmDCU.Name = "_tbmDCU";
            this._tbmDCU.Size = new System.Drawing.Size(302, 22);
            this._tbmDCU.TabIndex = 7;
            this._tbmDCU.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmDCU.TextImage")));
            this._tbmDCU.Visible = false;
            this._tbmDCU.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmDCU_DragOver);
            this._tbmDCU.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmDCU_DragDrop);
            this._tbmDCU.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmDCU_ButtonPopupMenuItemClick);
            // 
            // _tsiModify5
            // 
            this._tsiModify5.Name = "_tsiModify5";
            this._tsiModify5.Size = new System.Drawing.Size(180, 22);
            this._tsiModify5.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove5
            // 
            this._tsiRemove5.Name = "_tsiRemove5";
            this._tsiRemove5.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove5.Text = "toolStripMenuItem1";
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._itbDoorEnvironment);
            this._panelBack.Controls.Add(this._bApply);
            this._panelBack.Controls.Add(this._itbDcuName);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._nudOutputIndex);
            this._panelBack.Controls.Add(this._eOutputIndex);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._lOutputIndex);
            this._panelBack.Controls.Add(this._tcOutput);
            this._panelBack.Controls.Add(this._lDoorEnvironment);
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._tbmDCU);
            this._panelBack.Controls.Add(this._lParent);
            this._panelBack.Controls.Add(this._eStatusOnOff);
            this._panelBack.Controls.Add(this._lRealStatusOnOff);
            this._panelBack.Controls.Add(this._lStatusOnOff);
            this._panelBack.Controls.Add(this._eRealStatusOnOff);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(564, 474);
            this._panelBack.TabIndex = 0;
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(317, 436);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 4;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // NCASOutputEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(564, 474);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(554, 414);
            this.Name = "NCASOutputEditForm";
            this.Text = "NCASOutputEditForm";
            this._tcOutput.ResumeLayout(false);
            this._tpSettings.ResumeLayout(false);
            this._tpSettings.PerformLayout();
            this._gbTimingSettings.ResumeLayout(false);
            this._gbTimingSettings.PerformLayout();
            this._tpAlarmSettings.ResumeLayout(false);
            this._gbAlarmPresentationGroup.ResumeLayout(false);
            this._gbAlarmPresentationGroup.PerformLayout();
            this._gbEnableAlarms.ResumeLayout(false);
            this._gbEnableAlarms.PerformLayout();
            this._tpOutputControl.ResumeLayout(false);
            this._tpOutputControl.PerformLayout();
            this._gbControlObject.ResumeLayout(false);
            this._gbControlObject.PerformLayout();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudOutputIndex)).EndInit();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TabPage _tpSettings;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.TabPage _tpAlarmSettings;
        private System.Windows.Forms.GroupBox _gbAlarmPresentationGroup;
        private System.Windows.Forms.Label _lName1;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.GroupBox _gbTimingSettings;
        private System.Windows.Forms.TextBox _eSettingsDelayToOff;
        private System.Windows.Forms.Label _lSettingsDelayToOff;
        private System.Windows.Forms.TextBox _eSettingsDelayToOn;
        private System.Windows.Forms.Label _lSettingsDelayToOn;
        private System.Windows.Forms.Label _lOutputType;
        private System.Windows.Forms.ComboBox _cbOutPutType;
        private System.Windows.Forms.TabPage _tpOutputControl;
        private System.Windows.Forms.GroupBox _gbControlObject;
        private System.Windows.Forms.Label _lName2;
        private System.Windows.Forms.ComboBox _cbOutputControlType;
        private System.Windows.Forms.Label _lControlType;
        private System.Windows.Forms.Label _lParent;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.CheckBox _chbForcedToOff;
        private System.Windows.Forms.GroupBox _gbEnableAlarms;
        private System.Windows.Forms.CheckBox _chbAlarmCBOOn;
        private System.Windows.Forms.Label _lStatusOnOff;
        private System.Windows.Forms.TextBox _eStatusOnOff;
        private System.Windows.Forms.Label _lRealStatusOnOff;
        private System.Windows.Forms.TextBox _eRealStatusOnOff;
        private System.Windows.Forms.CheckBox _cbSendRealStateChanges;
        private System.Windows.Forms.TextBox _eSettingsPulseDelay;
        private System.Windows.Forms.Label _lSettingsPulseDelay;
        private System.Windows.Forms.TextBox _eSettingsPulseLength;
        private System.Windows.Forms.Label _lSettingsPulseLength;
        private System.Windows.Forms.Label _lDoorEnvironment;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private Contal.IwQuick.UI.TextBoxMenu _tbmAlarmPresentationGroup;
        private Contal.IwQuick.UI.TextBoxMenu _tbmControlObject;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove1;
        private System.Windows.Forms.TabControl _tcOutput;
        private System.Windows.Forms.Label _lOutputIndex;
        private System.Windows.Forms.TextBox _eOutputIndex;
        private System.Windows.Forms.NumericUpDown _nudOutputIndex;
        private Contal.IwQuick.UI.TextBoxMenu _tbmDCU;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify5;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove5;
        private System.Windows.Forms.Button _bApply;
        private Contal.IwQuick.UI.ImageTextBox _itbDcuName;
        private Contal.IwQuick.UI.ImageTextBox _itbDoorEnvironment;
        private System.Windows.Forms.CheckBox _cbInverted;
        private System.Windows.Forms.Label _lICCU;
    }
}
