namespace Contal.Cgp.NCAS.Client
{
    partial class NCASMultiDoorElementEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASMultiDoorElementEditForm));
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._bApply = new System.Windows.Forms.Button();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._tcMultiDoorElement = new System.Windows.Forms.TabControl();
            this._tpSettings = new System.Windows.Forms.TabPage();
            this._lFloor = new System.Windows.Forms.Label();
            this._eFloor = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._gbBlockObjects = new System.Windows.Forms.GroupBox();
            this._lOnOffObject = new System.Windows.Forms.Label();
            this._eBlockOnOffObject = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify4 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove4 = new System.Windows.Forms.ToolStripMenuItem();
            this._lAlarmArea = new System.Windows.Forms.Label();
            this._eBlockAlarmArea = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify3 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove3 = new System.Windows.Forms.ToolStripMenuItem();
            this._lDoorIndex = new System.Windows.Forms.Label();
            this._eDoorIndex = new System.Windows.Forms.NumericUpDown();
            this._lMultiDoor = new System.Windows.Forms.Label();
            this._eMultiDoorEnvironment = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tpApas = new System.Windows.Forms.TabPage();
            this._gbActuators = new System.Windows.Forms.GroupBox();
            this._eExtraElectricStrike = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify6 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove6 = new System.Windows.Forms.ToolStripMenuItem();
            this._eElectricStrike = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify5 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove5 = new System.Windows.Forms.ToolStripMenuItem();
            this._lExtraElectricStrike = new System.Windows.Forms.Label();
            this._lElectricStrike = new System.Windows.Forms.Label();
            this._gbSensors = new System.Windows.Forms.GroupBox();
            this._eSensorOpenDoor = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify7 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove7 = new System.Windows.Forms.ToolStripMenuItem();
            this._lSensorOpenDoor = new System.Windows.Forms.Label();
            this._tpObjectPlacement = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._lActualState = new System.Windows.Forms.Label();
            this._eActualState = new System.Windows.Forms.TextBox();
            this._tcMultiDoorElement.SuspendLayout();
            this._tpSettings.SuspendLayout();
            this._gbBlockObjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eDoorIndex)).BeginInit();
            this._tpApas.SuspendLayout();
            this._gbActuators.SuspendLayout();
            this._gbSensors.SuspendLayout();
            this._tpObjectPlacement.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(397, 296);
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
            this._bOk.Location = new System.Drawing.Point(316, 296);
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
            this._bApply.Location = new System.Drawing.Point(235, 296);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 3;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(98, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(374, 20);
            this._eName.TabIndex = 0;
            this._eName.TextChanged += new System.EventHandler(this._eName_TextChanged);
            this._eName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._eName_KeyPress);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 15);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 4;
            this._lName.Text = "Name";
            // 
            // _tcMultiDoorElement
            // 
            this._tcMultiDoorElement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcMultiDoorElement.Controls.Add(this._tpSettings);
            this._tcMultiDoorElement.Controls.Add(this._tpApas);
            this._tcMultiDoorElement.Controls.Add(this._tpObjectPlacement);
            this._tcMultiDoorElement.Controls.Add(this._tpReferencedBy);
            this._tcMultiDoorElement.Controls.Add(this._tpDescription);
            this._tcMultiDoorElement.Location = new System.Drawing.Point(12, 64);
            this._tcMultiDoorElement.Name = "_tcMultiDoorElement";
            this._tcMultiDoorElement.SelectedIndex = 0;
            this._tcMultiDoorElement.Size = new System.Drawing.Size(460, 226);
            this._tcMultiDoorElement.TabIndex = 2;
            // 
            // _tpSettings
            // 
            this._tpSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpSettings.Controls.Add(this._lFloor);
            this._tpSettings.Controls.Add(this._eFloor);
            this._tpSettings.Controls.Add(this._gbBlockObjects);
            this._tpSettings.Controls.Add(this._lDoorIndex);
            this._tpSettings.Controls.Add(this._eDoorIndex);
            this._tpSettings.Controls.Add(this._lMultiDoor);
            this._tpSettings.Controls.Add(this._eMultiDoorEnvironment);
            this._tpSettings.Location = new System.Drawing.Point(4, 22);
            this._tpSettings.Name = "_tpSettings";
            this._tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpSettings.Size = new System.Drawing.Size(452, 200);
            this._tpSettings.TabIndex = 0;
            this._tpSettings.Text = "Settings";
            // 
            // _lFloor
            // 
            this._lFloor.AutoSize = true;
            this._lFloor.Location = new System.Drawing.Point(6, 35);
            this._lFloor.Name = "_lFloor";
            this._lFloor.Size = new System.Drawing.Size(30, 13);
            this._lFloor.TabIndex = 6;
            this._lFloor.Text = "Floor";
            // 
            // _eFloor
            // 
            this._eFloor.AllowDrop = true;
            this._eFloor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eFloor.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._eFloor.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._eFloor.Button.BackColor = System.Drawing.SystemColors.Control;
            this._eFloor.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._eFloor.Button.Image = ((System.Drawing.Image)(resources.GetObject("_eFloor.Button.Image")));
            this._eFloor.Button.Location = new System.Drawing.Point(268, 0);
            this._eFloor.Button.Name = "_bMenu";
            this._eFloor.Button.Size = new System.Drawing.Size(20, 20);
            this._eFloor.Button.TabIndex = 3;
            this._eFloor.Button.UseVisualStyleBackColor = false;
            this._eFloor.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._eFloor.ButtonDefaultBehaviour = true;
            this._eFloor.ButtonHoverColor = System.Drawing.Color.Empty;
            this._eFloor.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_eFloor.ButtonImage")));
            // 
            // 
            // 
            this._eFloor.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2});
            this._eFloor.ButtonPopupMenu.Name = "";
            this._eFloor.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._eFloor.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._eFloor.ButtonShowImage = true;
            this._eFloor.ButtonSizeHeight = 20;
            this._eFloor.ButtonSizeWidth = 20;
            this._eFloor.ButtonText = "";
            this._eFloor.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eFloor.HoverTime = 500;
            // 
            // 
            // 
            this._eFloor.ImageTextBox.AllowDrop = true;
            this._eFloor.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eFloor.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._eFloor.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eFloor.ImageTextBox.ContextMenuStrip = this._eFloor.ButtonPopupMenu;
            this._eFloor.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eFloor.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_eFloor.ImageTextBox.Image")));
            this._eFloor.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._eFloor.ImageTextBox.Name = "_itbTextBox";
            this._eFloor.ImageTextBox.NoTextNoImage = true;
            this._eFloor.ImageTextBox.ReadOnly = false;
            this._eFloor.ImageTextBox.Size = new System.Drawing.Size(268, 20);
            this._eFloor.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._eFloor.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eFloor.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._eFloor.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._eFloor.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eFloor.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._eFloor.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._eFloor.ImageTextBox.TextBox.Size = new System.Drawing.Size(266, 13);
            this._eFloor.ImageTextBox.TextBox.TabIndex = 2;
            this._eFloor.ImageTextBox.UseImage = true;
            this._eFloor.ImageTextBox.DoubleClick += new System.EventHandler(this._eFloor_ImageTextBox_DoubleClick);
            this._eFloor.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._eFloor_ImageTextBox_DragDrop);
            this._eFloor.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._eFloor_ImageTextBox_DragOver);
            this._eFloor.Location = new System.Drawing.Point(158, 32);
            this._eFloor.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eFloor.MinimumSize = new System.Drawing.Size(30, 20);
            this._eFloor.Name = "_eFloor";
            this._eFloor.Size = new System.Drawing.Size(288, 20);
            this._eFloor.TabIndex = 1;
            this._eFloor.TextImage = ((System.Drawing.Image)(resources.GetObject("_eFloor.TextImage")));
            this._eFloor.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eFloor_ButtonPopupMenuItemClick);
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
            // _gbBlockObjects
            // 
            this._gbBlockObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbBlockObjects.Controls.Add(this._lOnOffObject);
            this._gbBlockObjects.Controls.Add(this._eBlockOnOffObject);
            this._gbBlockObjects.Controls.Add(this._lAlarmArea);
            this._gbBlockObjects.Controls.Add(this._eBlockAlarmArea);
            this._gbBlockObjects.Location = new System.Drawing.Point(9, 84);
            this._gbBlockObjects.Name = "_gbBlockObjects";
            this._gbBlockObjects.Size = new System.Drawing.Size(437, 77);
            this._gbBlockObjects.TabIndex = 3;
            this._gbBlockObjects.TabStop = false;
            this._gbBlockObjects.Text = "Block objects";
            // 
            // _lOnOffObject
            // 
            this._lOnOffObject.AutoSize = true;
            this._lOnOffObject.Location = new System.Drawing.Point(6, 49);
            this._lOnOffObject.Name = "_lOnOffObject";
            this._lOnOffObject.Size = new System.Drawing.Size(72, 13);
            this._lOnOffObject.TabIndex = 4;
            this._lOnOffObject.Text = "On/Off object";
            // 
            // _eBlockOnOffObject
            // 
            this._eBlockOnOffObject.AllowDrop = true;
            this._eBlockOnOffObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eBlockOnOffObject.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._eBlockOnOffObject.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._eBlockOnOffObject.Button.BackColor = System.Drawing.SystemColors.Control;
            this._eBlockOnOffObject.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._eBlockOnOffObject.Button.Image = ((System.Drawing.Image)(resources.GetObject("_eBlockOnOffObject.Button.Image")));
            this._eBlockOnOffObject.Button.Location = new System.Drawing.Point(268, 0);
            this._eBlockOnOffObject.Button.Name = "_bMenu";
            this._eBlockOnOffObject.Button.Size = new System.Drawing.Size(20, 20);
            this._eBlockOnOffObject.Button.TabIndex = 3;
            this._eBlockOnOffObject.Button.UseVisualStyleBackColor = false;
            this._eBlockOnOffObject.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._eBlockOnOffObject.ButtonDefaultBehaviour = true;
            this._eBlockOnOffObject.ButtonHoverColor = System.Drawing.Color.Empty;
            this._eBlockOnOffObject.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_eBlockOnOffObject.ButtonImage")));
            // 
            // 
            // 
            this._eBlockOnOffObject.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify4,
            this._tsiRemove4});
            this._eBlockOnOffObject.ButtonPopupMenu.Name = "";
            this._eBlockOnOffObject.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._eBlockOnOffObject.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._eBlockOnOffObject.ButtonShowImage = true;
            this._eBlockOnOffObject.ButtonSizeHeight = 20;
            this._eBlockOnOffObject.ButtonSizeWidth = 20;
            this._eBlockOnOffObject.ButtonText = "";
            this._eBlockOnOffObject.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eBlockOnOffObject.HoverTime = 500;
            // 
            // 
            // 
            this._eBlockOnOffObject.ImageTextBox.AllowDrop = true;
            this._eBlockOnOffObject.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eBlockOnOffObject.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._eBlockOnOffObject.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eBlockOnOffObject.ImageTextBox.ContextMenuStrip = this._eBlockOnOffObject.ButtonPopupMenu;
            this._eBlockOnOffObject.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eBlockOnOffObject.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_eBlockOnOffObject.ImageTextBox.Image")));
            this._eBlockOnOffObject.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._eBlockOnOffObject.ImageTextBox.Name = "_itbTextBox";
            this._eBlockOnOffObject.ImageTextBox.NoTextNoImage = true;
            this._eBlockOnOffObject.ImageTextBox.ReadOnly = false;
            this._eBlockOnOffObject.ImageTextBox.Size = new System.Drawing.Size(268, 20);
            this._eBlockOnOffObject.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._eBlockOnOffObject.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eBlockOnOffObject.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._eBlockOnOffObject.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._eBlockOnOffObject.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eBlockOnOffObject.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._eBlockOnOffObject.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._eBlockOnOffObject.ImageTextBox.TextBox.Size = new System.Drawing.Size(266, 13);
            this._eBlockOnOffObject.ImageTextBox.TextBox.TabIndex = 2;
            this._eBlockOnOffObject.ImageTextBox.UseImage = true;
            this._eBlockOnOffObject.ImageTextBox.DoubleClick += new System.EventHandler(this._eBlockOnOffObject_ImageTextBox_DoubleClick);
            this._eBlockOnOffObject.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._eBlockOnOffObject_ImageTextBox_DragDrop);
            this._eBlockOnOffObject.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._eBlockOnOffObject_ImageTextBox_DragOver);
            this._eBlockOnOffObject.Location = new System.Drawing.Point(149, 45);
            this._eBlockOnOffObject.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eBlockOnOffObject.MinimumSize = new System.Drawing.Size(30, 20);
            this._eBlockOnOffObject.Name = "_eBlockOnOffObject";
            this._eBlockOnOffObject.Size = new System.Drawing.Size(288, 20);
            this._eBlockOnOffObject.TabIndex = 1;
            this._eBlockOnOffObject.TextImage = ((System.Drawing.Image)(resources.GetObject("_eBlockOnOffObject.TextImage")));
            this._eBlockOnOffObject.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eBlockOnOffObject_ButtonPopupMenuItemClick);
            // 
            // _tsiModify4
            // 
            this._tsiModify4.Name = "_tsiModify4";
            this._tsiModify4.Size = new System.Drawing.Size(117, 22);
            this._tsiModify4.Text = "Modify";
            // 
            // _tsiRemove4
            // 
            this._tsiRemove4.Name = "_tsiRemove4";
            this._tsiRemove4.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove4.Text = "Remove";
            // 
            // _lAlarmArea
            // 
            this._lAlarmArea.AutoSize = true;
            this._lAlarmArea.Location = new System.Drawing.Point(6, 23);
            this._lAlarmArea.Name = "_lAlarmArea";
            this._lAlarmArea.Size = new System.Drawing.Size(57, 13);
            this._lAlarmArea.TabIndex = 2;
            this._lAlarmArea.Text = "Alarm area";
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
            this._eBlockAlarmArea.Button.Location = new System.Drawing.Point(268, 0);
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
            this._tsiModify3,
            this._tsiRemove3});
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
            this._eBlockAlarmArea.ImageTextBox.Size = new System.Drawing.Size(268, 20);
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
            this._eBlockAlarmArea.ImageTextBox.TextBox.Size = new System.Drawing.Size(266, 13);
            this._eBlockAlarmArea.ImageTextBox.TextBox.TabIndex = 2;
            this._eBlockAlarmArea.ImageTextBox.UseImage = true;
            this._eBlockAlarmArea.ImageTextBox.DoubleClick += new System.EventHandler(this._eBlockAlarmArea_ImageTextBox_DoubleClick);
            this._eBlockAlarmArea.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._eBlockAlarmArea_ImageTextBox_DragDrop);
            this._eBlockAlarmArea.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._eBlockAlarmArea_ImageTextBox_DragOver);
            this._eBlockAlarmArea.Location = new System.Drawing.Point(149, 19);
            this._eBlockAlarmArea.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eBlockAlarmArea.MinimumSize = new System.Drawing.Size(30, 20);
            this._eBlockAlarmArea.Name = "_eBlockAlarmArea";
            this._eBlockAlarmArea.Size = new System.Drawing.Size(288, 20);
            this._eBlockAlarmArea.TabIndex = 0;
            this._eBlockAlarmArea.TextImage = ((System.Drawing.Image)(resources.GetObject("_eBlockAlarmArea.TextImage")));
            this._eBlockAlarmArea.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eBlockAlarmArea_ButtonPopupMenuItemClick);
            // 
            // _tsiModify3
            // 
            this._tsiModify3.Name = "_tsiModify3";
            this._tsiModify3.Size = new System.Drawing.Size(117, 22);
            this._tsiModify3.Text = "Modify";
            // 
            // _tsiRemove3
            // 
            this._tsiRemove3.Name = "_tsiRemove3";
            this._tsiRemove3.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove3.Text = "Remove";
            // 
            // _lDoorIndex
            // 
            this._lDoorIndex.AutoSize = true;
            this._lDoorIndex.Location = new System.Drawing.Point(6, 62);
            this._lDoorIndex.Name = "_lDoorIndex";
            this._lDoorIndex.Size = new System.Drawing.Size(58, 13);
            this._lDoorIndex.TabIndex = 3;
            this._lDoorIndex.Text = "Door index";
            // 
            // _eDoorIndex
            // 
            this._eDoorIndex.Location = new System.Drawing.Point(158, 58);
            this._eDoorIndex.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._eDoorIndex.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this._eDoorIndex.Name = "_eDoorIndex";
            this._eDoorIndex.Size = new System.Drawing.Size(100, 20);
            this._eDoorIndex.TabIndex = 2;
            this._eDoorIndex.ValueChanged += new System.EventHandler(this._eDoorIndex_ValueChanged);
            // 
            // _lMultiDoor
            // 
            this._lMultiDoor.AutoSize = true;
            this._lMultiDoor.Location = new System.Drawing.Point(6, 10);
            this._lMultiDoor.Name = "_lMultiDoor";
            this._lMultiDoor.Size = new System.Drawing.Size(53, 13);
            this._lMultiDoor.TabIndex = 1;
            this._lMultiDoor.Text = "Multi door";
            // 
            // _eMultiDoorEnvironment
            // 
            this._eMultiDoorEnvironment.AllowDrop = true;
            this._eMultiDoorEnvironment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eMultiDoorEnvironment.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._eMultiDoorEnvironment.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._eMultiDoorEnvironment.Button.BackColor = System.Drawing.SystemColors.Control;
            this._eMultiDoorEnvironment.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._eMultiDoorEnvironment.Button.Image = ((System.Drawing.Image)(resources.GetObject("_eMultiDoorEnvironment.Button.Image")));
            this._eMultiDoorEnvironment.Button.Location = new System.Drawing.Point(268, 0);
            this._eMultiDoorEnvironment.Button.Name = "_bMenu";
            this._eMultiDoorEnvironment.Button.Size = new System.Drawing.Size(20, 20);
            this._eMultiDoorEnvironment.Button.TabIndex = 3;
            this._eMultiDoorEnvironment.Button.UseVisualStyleBackColor = false;
            this._eMultiDoorEnvironment.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._eMultiDoorEnvironment.ButtonDefaultBehaviour = true;
            this._eMultiDoorEnvironment.ButtonHoverColor = System.Drawing.Color.Empty;
            this._eMultiDoorEnvironment.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_eMultiDoorEnvironment.ButtonImage")));
            // 
            // 
            // 
            this._eMultiDoorEnvironment.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify1,
            this._tsiRemove1});
            this._eMultiDoorEnvironment.ButtonPopupMenu.Name = "";
            this._eMultiDoorEnvironment.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._eMultiDoorEnvironment.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._eMultiDoorEnvironment.ButtonShowImage = true;
            this._eMultiDoorEnvironment.ButtonSizeHeight = 20;
            this._eMultiDoorEnvironment.ButtonSizeWidth = 20;
            this._eMultiDoorEnvironment.ButtonText = "";
            this._eMultiDoorEnvironment.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eMultiDoorEnvironment.HoverTime = 500;
            // 
            // 
            // 
            this._eMultiDoorEnvironment.ImageTextBox.AllowDrop = true;
            this._eMultiDoorEnvironment.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eMultiDoorEnvironment.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._eMultiDoorEnvironment.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eMultiDoorEnvironment.ImageTextBox.ContextMenuStrip = this._eMultiDoorEnvironment.ButtonPopupMenu;
            this._eMultiDoorEnvironment.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eMultiDoorEnvironment.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_eMultiDoorEnvironment.ImageTextBox.Image")));
            this._eMultiDoorEnvironment.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._eMultiDoorEnvironment.ImageTextBox.Name = "_itbTextBox";
            this._eMultiDoorEnvironment.ImageTextBox.NoTextNoImage = true;
            this._eMultiDoorEnvironment.ImageTextBox.ReadOnly = false;
            this._eMultiDoorEnvironment.ImageTextBox.Size = new System.Drawing.Size(268, 20);
            this._eMultiDoorEnvironment.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._eMultiDoorEnvironment.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eMultiDoorEnvironment.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._eMultiDoorEnvironment.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._eMultiDoorEnvironment.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eMultiDoorEnvironment.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._eMultiDoorEnvironment.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._eMultiDoorEnvironment.ImageTextBox.TextBox.Size = new System.Drawing.Size(266, 13);
            this._eMultiDoorEnvironment.ImageTextBox.TextBox.TabIndex = 2;
            this._eMultiDoorEnvironment.ImageTextBox.UseImage = true;
            this._eMultiDoorEnvironment.ImageTextBox.DoubleClick += new System.EventHandler(this._eMultiDoorEnvironment_ImageTextBox_DoubleClick);
            this._eMultiDoorEnvironment.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._eMultiDoorEnvironment_ImageTextBox_DragDrop);
            this._eMultiDoorEnvironment.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._eMultiDoorEnvironment_ImageTextBox_DragOver);
            this._eMultiDoorEnvironment.Location = new System.Drawing.Point(158, 6);
            this._eMultiDoorEnvironment.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eMultiDoorEnvironment.MinimumSize = new System.Drawing.Size(30, 20);
            this._eMultiDoorEnvironment.Name = "_eMultiDoorEnvironment";
            this._eMultiDoorEnvironment.Size = new System.Drawing.Size(288, 20);
            this._eMultiDoorEnvironment.TabIndex = 0;
            this._eMultiDoorEnvironment.TextImage = ((System.Drawing.Image)(resources.GetObject("_eMultiDoorEnvironment.TextImage")));
            this._eMultiDoorEnvironment.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eMultiDoorEnvironment_ButtonPopupMenuItemClick);
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
            // _tpApas
            // 
            this._tpApas.BackColor = System.Drawing.SystemColors.Control;
            this._tpApas.Controls.Add(this._gbActuators);
            this._tpApas.Controls.Add(this._gbSensors);
            this._tpApas.Location = new System.Drawing.Point(4, 22);
            this._tpApas.Name = "_tpApas";
            this._tpApas.Padding = new System.Windows.Forms.Padding(3);
            this._tpApas.Size = new System.Drawing.Size(452, 200);
            this._tpApas.TabIndex = 1;
            this._tpApas.Text = "Apas";
            // 
            // _gbActuators
            // 
            this._gbActuators.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbActuators.Controls.Add(this._eExtraElectricStrike);
            this._gbActuators.Controls.Add(this._eElectricStrike);
            this._gbActuators.Controls.Add(this._lExtraElectricStrike);
            this._gbActuators.Controls.Add(this._lElectricStrike);
            this._gbActuators.Location = new System.Drawing.Point(6, 6);
            this._gbActuators.Name = "_gbActuators";
            this._gbActuators.Size = new System.Drawing.Size(440, 77);
            this._gbActuators.TabIndex = 2;
            this._gbActuators.TabStop = false;
            this._gbActuators.Text = "Actuators";
            // 
            // _eExtraElectricStrike
            // 
            this._eExtraElectricStrike.AllowDrop = true;
            this._eExtraElectricStrike.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eExtraElectricStrike.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._eExtraElectricStrike.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._eExtraElectricStrike.Button.BackColor = System.Drawing.SystemColors.Control;
            this._eExtraElectricStrike.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._eExtraElectricStrike.Button.Image = ((System.Drawing.Image)(resources.GetObject("_eExtraElectricStrike.Button.Image")));
            this._eExtraElectricStrike.Button.Location = new System.Drawing.Point(251, 0);
            this._eExtraElectricStrike.Button.Name = "_bMenu";
            this._eExtraElectricStrike.Button.Size = new System.Drawing.Size(20, 20);
            this._eExtraElectricStrike.Button.TabIndex = 3;
            this._eExtraElectricStrike.Button.UseVisualStyleBackColor = false;
            this._eExtraElectricStrike.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._eExtraElectricStrike.ButtonDefaultBehaviour = true;
            this._eExtraElectricStrike.ButtonHoverColor = System.Drawing.Color.Empty;
            this._eExtraElectricStrike.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_eExtraElectricStrike.ButtonImage")));
            // 
            // 
            // 
            this._eExtraElectricStrike.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify6,
            this._tsiRemove6});
            this._eExtraElectricStrike.ButtonPopupMenu.Name = "";
            this._eExtraElectricStrike.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._eExtraElectricStrike.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._eExtraElectricStrike.ButtonShowImage = true;
            this._eExtraElectricStrike.ButtonSizeHeight = 20;
            this._eExtraElectricStrike.ButtonSizeWidth = 20;
            this._eExtraElectricStrike.ButtonText = "";
            this._eExtraElectricStrike.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eExtraElectricStrike.HoverTime = 500;
            // 
            // 
            // 
            this._eExtraElectricStrike.ImageTextBox.AllowDrop = true;
            this._eExtraElectricStrike.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eExtraElectricStrike.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._eExtraElectricStrike.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eExtraElectricStrike.ImageTextBox.ContextMenuStrip = this._eExtraElectricStrike.ButtonPopupMenu;
            this._eExtraElectricStrike.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eExtraElectricStrike.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_eExtraElectricStrike.ImageTextBox.Image")));
            this._eExtraElectricStrike.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._eExtraElectricStrike.ImageTextBox.Name = "_itbTextBox";
            this._eExtraElectricStrike.ImageTextBox.NoTextNoImage = true;
            this._eExtraElectricStrike.ImageTextBox.ReadOnly = false;
            this._eExtraElectricStrike.ImageTextBox.Size = new System.Drawing.Size(251, 20);
            this._eExtraElectricStrike.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._eExtraElectricStrike.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eExtraElectricStrike.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._eExtraElectricStrike.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._eExtraElectricStrike.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eExtraElectricStrike.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._eExtraElectricStrike.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._eExtraElectricStrike.ImageTextBox.TextBox.Size = new System.Drawing.Size(249, 13);
            this._eExtraElectricStrike.ImageTextBox.TextBox.TabIndex = 2;
            this._eExtraElectricStrike.ImageTextBox.UseImage = true;
            this._eExtraElectricStrike.ImageTextBox.DoubleClick += new System.EventHandler(this._eExtraElectricStrike_ImageTextBox_DoubleClick);
            this._eExtraElectricStrike.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._eExtraElectricStrike_ImageTextBox_DragDrop);
            this._eExtraElectricStrike.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._eExtraElectricStrike_ImageTextBox_DragOver);
            this._eExtraElectricStrike.Location = new System.Drawing.Point(163, 45);
            this._eExtraElectricStrike.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eExtraElectricStrike.MinimumSize = new System.Drawing.Size(30, 20);
            this._eExtraElectricStrike.Name = "_eExtraElectricStrike";
            this._eExtraElectricStrike.Size = new System.Drawing.Size(271, 20);
            this._eExtraElectricStrike.TabIndex = 3;
            this._eExtraElectricStrike.TextImage = ((System.Drawing.Image)(resources.GetObject("_eExtraElectricStrike.TextImage")));
            this._eExtraElectricStrike.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eExtraElectricStrike_ButtonPopupMenuItemClick);
            // 
            // _tsiModify6
            // 
            this._tsiModify6.Name = "_tsiModify6";
            this._tsiModify6.Size = new System.Drawing.Size(117, 22);
            this._tsiModify6.Text = "Modify";
            // 
            // _tsiRemove6
            // 
            this._tsiRemove6.Name = "_tsiRemove6";
            this._tsiRemove6.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove6.Text = "Remove";
            // 
            // _eElectricStrike
            // 
            this._eElectricStrike.AllowDrop = true;
            this._eElectricStrike.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eElectricStrike.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._eElectricStrike.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._eElectricStrike.Button.BackColor = System.Drawing.SystemColors.Control;
            this._eElectricStrike.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._eElectricStrike.Button.Image = ((System.Drawing.Image)(resources.GetObject("_eElectricStrike.Button.Image")));
            this._eElectricStrike.Button.Location = new System.Drawing.Point(251, 0);
            this._eElectricStrike.Button.Name = "_bMenu";
            this._eElectricStrike.Button.Size = new System.Drawing.Size(20, 20);
            this._eElectricStrike.Button.TabIndex = 3;
            this._eElectricStrike.Button.UseVisualStyleBackColor = false;
            this._eElectricStrike.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._eElectricStrike.ButtonDefaultBehaviour = true;
            this._eElectricStrike.ButtonHoverColor = System.Drawing.Color.Empty;
            this._eElectricStrike.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_eElectricStrike.ButtonImage")));
            // 
            // 
            // 
            this._eElectricStrike.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify5,
            this._tsiRemove5});
            this._eElectricStrike.ButtonPopupMenu.Name = "";
            this._eElectricStrike.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._eElectricStrike.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._eElectricStrike.ButtonShowImage = true;
            this._eElectricStrike.ButtonSizeHeight = 20;
            this._eElectricStrike.ButtonSizeWidth = 20;
            this._eElectricStrike.ButtonText = "";
            this._eElectricStrike.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eElectricStrike.HoverTime = 500;
            // 
            // 
            // 
            this._eElectricStrike.ImageTextBox.AllowDrop = true;
            this._eElectricStrike.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eElectricStrike.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._eElectricStrike.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eElectricStrike.ImageTextBox.ContextMenuStrip = this._eElectricStrike.ButtonPopupMenu;
            this._eElectricStrike.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eElectricStrike.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_eElectricStrike.ImageTextBox.Image")));
            this._eElectricStrike.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._eElectricStrike.ImageTextBox.Name = "_itbTextBox";
            this._eElectricStrike.ImageTextBox.NoTextNoImage = true;
            this._eElectricStrike.ImageTextBox.ReadOnly = false;
            this._eElectricStrike.ImageTextBox.Size = new System.Drawing.Size(251, 20);
            this._eElectricStrike.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._eElectricStrike.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eElectricStrike.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._eElectricStrike.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._eElectricStrike.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eElectricStrike.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._eElectricStrike.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._eElectricStrike.ImageTextBox.TextBox.Size = new System.Drawing.Size(249, 13);
            this._eElectricStrike.ImageTextBox.TextBox.TabIndex = 2;
            this._eElectricStrike.ImageTextBox.UseImage = true;
            this._eElectricStrike.ImageTextBox.DoubleClick += new System.EventHandler(this._eElectricStrike_ImageTextBox_DoubleClick);
            this._eElectricStrike.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._eElectricStrike_ImageTextBox_DragDrop);
            this._eElectricStrike.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._eElectricStrike_ImageTextBox_DragOver);
            this._eElectricStrike.Location = new System.Drawing.Point(163, 19);
            this._eElectricStrike.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eElectricStrike.MinimumSize = new System.Drawing.Size(30, 20);
            this._eElectricStrike.Name = "_eElectricStrike";
            this._eElectricStrike.Size = new System.Drawing.Size(271, 20);
            this._eElectricStrike.TabIndex = 0;
            this._eElectricStrike.TextImage = ((System.Drawing.Image)(resources.GetObject("_eElectricStrike.TextImage")));
            this._eElectricStrike.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eElectricStrike_ButtonPopupMenuItemClick);
            // 
            // _tsiModify5
            // 
            this._tsiModify5.Name = "_tsiModify5";
            this._tsiModify5.Size = new System.Drawing.Size(117, 22);
            this._tsiModify5.Text = "Modify";
            // 
            // _tsiRemove5
            // 
            this._tsiRemove5.Name = "_tsiRemove5";
            this._tsiRemove5.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove5.Text = "Remove";
            // 
            // _lExtraElectricStrike
            // 
            this._lExtraElectricStrike.AutoSize = true;
            this._lExtraElectricStrike.Location = new System.Drawing.Point(6, 48);
            this._lExtraElectricStrike.Name = "_lExtraElectricStrike";
            this._lExtraElectricStrike.Size = new System.Drawing.Size(96, 13);
            this._lExtraElectricStrike.TabIndex = 10;
            this._lExtraElectricStrike.Text = "Extra electric strike";
            // 
            // _lElectricStrike
            // 
            this._lElectricStrike.AutoSize = true;
            this._lElectricStrike.Location = new System.Drawing.Point(6, 22);
            this._lElectricStrike.Name = "_lElectricStrike";
            this._lElectricStrike.Size = new System.Drawing.Size(70, 13);
            this._lElectricStrike.TabIndex = 2;
            this._lElectricStrike.Text = "Electric strike";
            // 
            // _gbSensors
            // 
            this._gbSensors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbSensors.Controls.Add(this._eSensorOpenDoor);
            this._gbSensors.Controls.Add(this._lSensorOpenDoor);
            this._gbSensors.Location = new System.Drawing.Point(6, 89);
            this._gbSensors.Name = "_gbSensors";
            this._gbSensors.Size = new System.Drawing.Size(440, 52);
            this._gbSensors.TabIndex = 3;
            this._gbSensors.TabStop = false;
            this._gbSensors.Text = "Sensors";
            // 
            // _eSensorOpenDoor
            // 
            this._eSensorOpenDoor.AllowDrop = true;
            this._eSensorOpenDoor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eSensorOpenDoor.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._eSensorOpenDoor.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._eSensorOpenDoor.Button.BackColor = System.Drawing.SystemColors.Control;
            this._eSensorOpenDoor.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._eSensorOpenDoor.Button.Image = ((System.Drawing.Image)(resources.GetObject("_eSensorOpenDoor.Button.Image")));
            this._eSensorOpenDoor.Button.Location = new System.Drawing.Point(251, 0);
            this._eSensorOpenDoor.Button.Name = "_bMenu";
            this._eSensorOpenDoor.Button.Size = new System.Drawing.Size(20, 20);
            this._eSensorOpenDoor.Button.TabIndex = 3;
            this._eSensorOpenDoor.Button.UseVisualStyleBackColor = false;
            this._eSensorOpenDoor.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._eSensorOpenDoor.ButtonDefaultBehaviour = true;
            this._eSensorOpenDoor.ButtonHoverColor = System.Drawing.Color.Empty;
            this._eSensorOpenDoor.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_eSensorOpenDoor.ButtonImage")));
            // 
            // 
            // 
            this._eSensorOpenDoor.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify7,
            this._tsiRemove7});
            this._eSensorOpenDoor.ButtonPopupMenu.Name = "";
            this._eSensorOpenDoor.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._eSensorOpenDoor.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._eSensorOpenDoor.ButtonShowImage = true;
            this._eSensorOpenDoor.ButtonSizeHeight = 20;
            this._eSensorOpenDoor.ButtonSizeWidth = 20;
            this._eSensorOpenDoor.ButtonText = "";
            this._eSensorOpenDoor.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eSensorOpenDoor.HoverTime = 500;
            // 
            // 
            // 
            this._eSensorOpenDoor.ImageTextBox.AllowDrop = true;
            this._eSensorOpenDoor.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eSensorOpenDoor.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._eSensorOpenDoor.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eSensorOpenDoor.ImageTextBox.ContextMenuStrip = this._eSensorOpenDoor.ButtonPopupMenu;
            this._eSensorOpenDoor.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eSensorOpenDoor.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_eSensorOpenDoor.ImageTextBox.Image")));
            this._eSensorOpenDoor.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._eSensorOpenDoor.ImageTextBox.Name = "_itbTextBox";
            this._eSensorOpenDoor.ImageTextBox.NoTextNoImage = true;
            this._eSensorOpenDoor.ImageTextBox.ReadOnly = false;
            this._eSensorOpenDoor.ImageTextBox.Size = new System.Drawing.Size(251, 20);
            this._eSensorOpenDoor.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._eSensorOpenDoor.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eSensorOpenDoor.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._eSensorOpenDoor.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._eSensorOpenDoor.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._eSensorOpenDoor.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._eSensorOpenDoor.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._eSensorOpenDoor.ImageTextBox.TextBox.Size = new System.Drawing.Size(249, 13);
            this._eSensorOpenDoor.ImageTextBox.TextBox.TabIndex = 2;
            this._eSensorOpenDoor.ImageTextBox.UseImage = true;
            this._eSensorOpenDoor.ImageTextBox.DoubleClick += new System.EventHandler(this._eSensorOpenDoor_ImageTextBox_DoubleClick);
            this._eSensorOpenDoor.ImageTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._eSensorOpenDoor_ImageTextBox_DragDrop);
            this._eSensorOpenDoor.ImageTextBox.DragOver += new System.Windows.Forms.DragEventHandler(this._eSensorOpenDoor_ImageTextBox_DragOver);
            this._eSensorOpenDoor.Location = new System.Drawing.Point(163, 19);
            this._eSensorOpenDoor.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eSensorOpenDoor.MinimumSize = new System.Drawing.Size(30, 20);
            this._eSensorOpenDoor.Name = "_eSensorOpenDoor";
            this._eSensorOpenDoor.Size = new System.Drawing.Size(271, 20);
            this._eSensorOpenDoor.TabIndex = 0;
            this._eSensorOpenDoor.TextImage = ((System.Drawing.Image)(resources.GetObject("_eSensorOpenDoor.TextImage")));
            this._eSensorOpenDoor.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eSensorOpenDoor_ButtonPopupMenuItemClick);
            // 
            // _tsiModify7
            // 
            this._tsiModify7.Name = "_tsiModify7";
            this._tsiModify7.Size = new System.Drawing.Size(117, 22);
            this._tsiModify7.Text = "Modify";
            // 
            // _tsiRemove7
            // 
            this._tsiRemove7.Name = "_tsiRemove7";
            this._tsiRemove7.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove7.Text = "Remove";
            // 
            // _lSensorOpenDoor
            // 
            this._lSensorOpenDoor.AutoSize = true;
            this._lSensorOpenDoor.Location = new System.Drawing.Point(6, 22);
            this._lSensorOpenDoor.Name = "_lSensorOpenDoor";
            this._lSensorOpenDoor.Size = new System.Drawing.Size(91, 13);
            this._lSensorOpenDoor.TabIndex = 4;
            this._lSensorOpenDoor.Text = "Sensor open door";
            // 
            // _tpObjectPlacement
            // 
            this._tpObjectPlacement.BackColor = System.Drawing.SystemColors.Control;
            this._tpObjectPlacement.Controls.Add(this._bRefresh);
            this._tpObjectPlacement.Controls.Add(this._lbUserFolders);
            this._tpObjectPlacement.Location = new System.Drawing.Point(4, 22);
            this._tpObjectPlacement.Name = "_tpObjectPlacement";
            this._tpObjectPlacement.Size = new System.Drawing.Size(452, 200);
            this._tpObjectPlacement.TabIndex = 3;
            this._tpObjectPlacement.Text = "Object placement";
            this._tpObjectPlacement.Enter += new System.EventHandler(this._tpObjectPlacement_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(374, 174);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 27;
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
            this._lbUserFolders.Location = new System.Drawing.Point(3, 2);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(447, 160);
            this._lbUserFolders.TabIndex = 28;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Size = new System.Drawing.Size(452, 200);
            this._tpReferencedBy.TabIndex = 4;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Size = new System.Drawing.Size(452, 200);
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
            this._eDescription.Size = new System.Drawing.Size(452, 200);
            this._eDescription.TabIndex = 7;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this._eDescription_TextChanged);
            // 
            // _lActualState
            // 
            this._lActualState.AutoSize = true;
            this._lActualState.Location = new System.Drawing.Point(12, 41);
            this._lActualState.Name = "_lActualState";
            this._lActualState.Size = new System.Drawing.Size(63, 13);
            this._lActualState.TabIndex = 7;
            this._lActualState.Text = "Actual state";
            // 
            // _eActualState
            // 
            this._eActualState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eActualState.BackColor = System.Drawing.SystemColors.Window;
            this._eActualState.Location = new System.Drawing.Point(98, 38);
            this._eActualState.Name = "_eActualState";
            this._eActualState.ReadOnly = true;
            this._eActualState.Size = new System.Drawing.Size(374, 20);
            this._eActualState.TabIndex = 1;
            this._eActualState.TabStop = false;
            // 
            // NCASMultiDoorElementEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(484, 331);
            this.Controls.Add(this._lActualState);
            this.Controls.Add(this._eActualState);
            this.Controls.Add(this._tcMultiDoorElement);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._bApply);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 370);
            this.Name = "NCASMultiDoorElementEditForm";
            this.Text = "NCASMultiDoorElementEditForm";
            this._tcMultiDoorElement.ResumeLayout(false);
            this._tpSettings.ResumeLayout(false);
            this._tpSettings.PerformLayout();
            this._gbBlockObjects.ResumeLayout(false);
            this._gbBlockObjects.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eDoorIndex)).EndInit();
            this._tpApas.ResumeLayout(false);
            this._gbActuators.ResumeLayout(false);
            this._gbActuators.PerformLayout();
            this._gbSensors.ResumeLayout(false);
            this._gbSensors.PerformLayout();
            this._tpObjectPlacement.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TabControl _tcMultiDoorElement;
        private System.Windows.Forms.TabPage _tpSettings;
        private System.Windows.Forms.TabPage _tpApas;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TabPage _tpObjectPlacement;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.NumericUpDown _eDoorIndex;
        private System.Windows.Forms.Label _lMultiDoor;
        private Contal.IwQuick.UI.TextBoxMenu _eMultiDoorEnvironment;
        private System.Windows.Forms.GroupBox _gbBlockObjects;
        private System.Windows.Forms.Label _lDoorIndex;
        private System.Windows.Forms.Label _lOnOffObject;
        private Contal.IwQuick.UI.TextBoxMenu _eBlockOnOffObject;
        private System.Windows.Forms.Label _lAlarmArea;
        private Contal.IwQuick.UI.TextBoxMenu _eBlockAlarmArea;
        private System.Windows.Forms.GroupBox _gbActuators;
        private System.Windows.Forms.Label _lExtraElectricStrike;
        private System.Windows.Forms.Label _lElectricStrike;
        private System.Windows.Forms.GroupBox _gbSensors;
        private System.Windows.Forms.Label _lSensorOpenDoor;
        private System.Windows.Forms.Label _lActualState;
        private System.Windows.Forms.TextBox _eActualState;
        private System.Windows.Forms.Label _lFloor;
        private Contal.IwQuick.UI.TextBoxMenu _eFloor;
        private Contal.IwQuick.UI.TextBoxMenu _eElectricStrike;
        private Contal.IwQuick.UI.TextBoxMenu _eExtraElectricStrike;
        private Contal.IwQuick.UI.TextBoxMenu _eSensorOpenDoor;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove1;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify4;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove4;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify3;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove3;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify6;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove6;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify5;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove5;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify7;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove7;
    }
}
