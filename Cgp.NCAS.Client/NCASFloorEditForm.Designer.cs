namespace Contal.Cgp.NCAS.Client
{
    partial class NCASFloorEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASFloorEditForm));
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._bApply = new System.Windows.Forms.Button();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._tcFloor = new System.Windows.Forms.TabControl();
            this._tpSettings = new System.Windows.Forms.TabPage();
            this._lAlarmArea = new System.Windows.Forms.Label();
            this._eBlockAlarmArea = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._lFloorNumber = new System.Windows.Forms.Label();
            this._eFloorNumber = new System.Windows.Forms.NumericUpDown();
            this._tpDoors = new System.Windows.Forms.TabPage();
            this._bEdit = new System.Windows.Forms.Button();
            this._bAdd = new System.Windows.Forms.Button();
            this._bRemove = new System.Windows.Forms.Button();
            this._dgMultiDoorElements = new Contal.Cgp.Components.CgpDataGridView();
            this._tpObjectPlacement = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._tcFloor.SuspendLayout();
            this._tpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eFloorNumber)).BeginInit();
            this._tpDoors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgMultiDoorElements.DataGrid)).BeginInit();
            this._tpObjectPlacement.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(392, 289);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 0;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(311, 289);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 1;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(230, 289);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 2;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(83, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(384, 20);
            this._eName.TabIndex = 3;
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
            // _tcFloor
            // 
            this._tcFloor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcFloor.Controls.Add(this._tpSettings);
            this._tcFloor.Controls.Add(this._tpDoors);
            this._tcFloor.Controls.Add(this._tpObjectPlacement);
            this._tcFloor.Controls.Add(this._tpReferencedBy);
            this._tcFloor.Controls.Add(this._tpDescription);
            this._tcFloor.Location = new System.Drawing.Point(12, 38);
            this._tcFloor.Name = "_tcFloor";
            this._tcFloor.SelectedIndex = 0;
            this._tcFloor.Size = new System.Drawing.Size(455, 245);
            this._tcFloor.TabIndex = 5;
            // 
            // _tpSettings
            // 
            this._tpSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpSettings.Controls.Add(this._lAlarmArea);
            this._tpSettings.Controls.Add(this._eBlockAlarmArea);
            this._tpSettings.Controls.Add(this._lFloorNumber);
            this._tpSettings.Controls.Add(this._eFloorNumber);
            this._tpSettings.Location = new System.Drawing.Point(4, 22);
            this._tpSettings.Name = "_tpSettings";
            this._tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpSettings.Size = new System.Drawing.Size(447, 219);
            this._tpSettings.TabIndex = 0;
            this._tpSettings.Text = "Settings";
            // 
            // _lAlarmArea
            // 
            this._lAlarmArea.AutoSize = true;
            this._lAlarmArea.Location = new System.Drawing.Point(6, 34);
            this._lAlarmArea.Name = "_lAlarmArea";
            this._lAlarmArea.Size = new System.Drawing.Size(57, 13);
            this._lAlarmArea.TabIndex = 7;
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
            this._tsiModify,
            this._tsiRemove});
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
            this._eBlockAlarmArea.Location = new System.Drawing.Point(128, 32);
            this._eBlockAlarmArea.MaximumSize = new System.Drawing.Size(1200, 55);
            this._eBlockAlarmArea.MinimumSize = new System.Drawing.Size(30, 20);
            this._eBlockAlarmArea.Name = "_eBlockAlarmArea";
            this._eBlockAlarmArea.Size = new System.Drawing.Size(288, 20);
            this._eBlockAlarmArea.TabIndex = 6;
            this._eBlockAlarmArea.TextImage = ((System.Drawing.Image)(resources.GetObject("_eBlockAlarmArea.TextImage")));
            this._eBlockAlarmArea.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._eBlockAlarmArea_ButtonPopupMenuItemClick);
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
            // _lFloorNumber
            // 
            this._lFloorNumber.AutoSize = true;
            this._lFloorNumber.Location = new System.Drawing.Point(6, 8);
            this._lFloorNumber.Name = "_lFloorNumber";
            this._lFloorNumber.Size = new System.Drawing.Size(68, 13);
            this._lFloorNumber.TabIndex = 5;
            this._lFloorNumber.Text = "Floor number";
            // 
            // _eFloorNumber
            // 
            this._eFloorNumber.Location = new System.Drawing.Point(128, 6);
            this._eFloorNumber.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._eFloorNumber.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this._eFloorNumber.Name = "_eFloorNumber";
            this._eFloorNumber.Size = new System.Drawing.Size(100, 20);
            this._eFloorNumber.TabIndex = 4;
            this._eFloorNumber.ValueChanged += new System.EventHandler(this._eFloorNumber_ValueChanged);
            // 
            // _tpDoors
            // 
            this._tpDoors.BackColor = System.Drawing.SystemColors.Control;
            this._tpDoors.Controls.Add(this._bEdit);
            this._tpDoors.Controls.Add(this._bAdd);
            this._tpDoors.Controls.Add(this._bRemove);
            this._tpDoors.Controls.Add(this._dgMultiDoorElements);
            this._tpDoors.Location = new System.Drawing.Point(4, 22);
            this._tpDoors.Name = "_tpDoors";
            this._tpDoors.Padding = new System.Windows.Forms.Padding(3);
            this._tpDoors.Size = new System.Drawing.Size(447, 219);
            this._tpDoors.TabIndex = 1;
            this._tpDoors.Text = "Doors";
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(285, 190);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 7;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _bAdd
            // 
            this._bAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd.Location = new System.Drawing.Point(204, 190);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(75, 23);
            this._bAdd.TabIndex = 6;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _bRemove
            // 
            this._bRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRemove.Location = new System.Drawing.Point(366, 190);
            this._bRemove.Name = "_bRemove";
            this._bRemove.Size = new System.Drawing.Size(75, 23);
            this._bRemove.TabIndex = 5;
            this._bRemove.Text = "Remove";
            this._bRemove.UseVisualStyleBackColor = true;
            this._bRemove.Click += new System.EventHandler(this._bRemove_Click);
            // 
            // _dgMultiDoorElements
            // 
            this._dgMultiDoorElements.AllowDrop = true;
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
            this._dgMultiDoorElements.DataGrid.Size = new System.Drawing.Size(435, 178);
            this._dgMultiDoorElements.DataGrid.TabIndex = 0;
            this._dgMultiDoorElements.LocalizationHelper = null;
            this._dgMultiDoorElements.Location = new System.Drawing.Point(6, 6);
            this._dgMultiDoorElements.Name = "_dgMultiDoorElements";
            this._dgMultiDoorElements.Size = new System.Drawing.Size(435, 178);
            this._dgMultiDoorElements.TabIndex = 4;
            this._dgMultiDoorElements.DragOver += new System.Windows.Forms.DragEventHandler(this._dgMultiDoorElements_DragOver);
            this._dgMultiDoorElements.DragDrop += new System.Windows.Forms.DragEventHandler(this._dgMultiDoorElements_DragDrop);
            // 
            // _tpObjectPlacement
            // 
            this._tpObjectPlacement.BackColor = System.Drawing.SystemColors.Control;
            this._tpObjectPlacement.Controls.Add(this._bRefresh);
            this._tpObjectPlacement.Controls.Add(this._lbUserFolders);
            this._tpObjectPlacement.Location = new System.Drawing.Point(4, 22);
            this._tpObjectPlacement.Name = "_tpObjectPlacement";
            this._tpObjectPlacement.Size = new System.Drawing.Size(447, 219);
            this._tpObjectPlacement.TabIndex = 2;
            this._tpObjectPlacement.Text = "Object placement";
            this._tpObjectPlacement.Enter += new System.EventHandler(this._tpObjectPlacement_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(369, 193);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 31;
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
            this._lbUserFolders.Size = new System.Drawing.Size(443, 186);
            this._lbUserFolders.TabIndex = 32;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Size = new System.Drawing.Size(447, 219);
            this._tpReferencedBy.TabIndex = 3;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Size = new System.Drawing.Size(447, 219);
            this._tpDescription.TabIndex = 4;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(0, 0);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(447, 219);
            this._eDescription.TabIndex = 8;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this._eDescription_TextChanged);
            // 
            // NCASFloorEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(479, 324);
            this.Controls.Add(this._tcFloor);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._bApply);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NCASFloorEditForm";
            this.Text = "NCASFloorEditForm";
            this._tcFloor.ResumeLayout(false);
            this._tpSettings.ResumeLayout(false);
            this._tpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eFloorNumber)).EndInit();
            this._tpDoors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgMultiDoorElements.DataGrid)).EndInit();
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
        private System.Windows.Forms.TabControl _tcFloor;
        private System.Windows.Forms.TabPage _tpSettings;
        private System.Windows.Forms.TabPage _tpDoors;
        private System.Windows.Forms.TabPage _tpObjectPlacement;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.Label _lFloorNumber;
        private System.Windows.Forms.NumericUpDown _eFloorNumber;
        private Contal.Cgp.Components.CgpDataGridView _dgMultiDoorElements;
        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.Button _bRemove;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Label _lAlarmArea;
        private Contal.IwQuick.UI.TextBoxMenu _eBlockAlarmArea;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
    }
}
