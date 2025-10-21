namespace Contal.Cgp.NCAS.Client
{
    partial class NCASACLPersonEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASACLPersonEditForm));
            this._pBack = new System.Windows.Forms.Panel();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._tbmAccessControlList = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate = new System.Windows.Forms.ToolStripMenuItem();
            this._tbdpDateTo = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._tbdpDateFrom = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._lDateTo = new System.Windows.Forms.Label();
            this._bCreate1 = new System.Windows.Forms.Button();
            this._lDateFrom = new System.Windows.Forms.Label();
            this._bCancelEdit = new System.Windows.Forms.Button();
            this._bUpdate = new System.Windows.Forms.Button();
            this._lAccessControlList = new System.Windows.Forms.Label();
            this._pBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pBack
            // 
            this._pBack.Controls.Add(this._cdgvData);
            this._pBack.Controls.Add(this._tbmAccessControlList);
            this._pBack.Controls.Add(this._tbdpDateTo);
            this._pBack.Controls.Add(this._tbdpDateFrom);
            this._pBack.Controls.Add(this._lDateTo);
            this._pBack.Controls.Add(this._bCreate1);
            this._pBack.Controls.Add(this._lDateFrom);
            this._pBack.Controls.Add(this._bCancelEdit);
            this._pBack.Controls.Add(this._bUpdate);
            this._pBack.Controls.Add(this._lAccessControlList);
            this._pBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pBack.Location = new System.Drawing.Point(0, 0);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(618, 325);
            this._pBack.TabIndex = 0;
            // 
            // _cdgvData
            // 
            this._cdgvData.AllowDrop = true;
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvData.CgpDataGridEvents = null;
            this._cdgvData.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvData.DataGrid.AllowDrop = true;
            this._cdgvData.DataGrid.AllowUserToAddRows = false;
            this._cdgvData.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvData.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvData.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._cdgvData.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.ColumnHeadersHeight = 34;
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.MultiSelect = false;
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(594, 232);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(12, 81);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(594, 232);
            this._cdgvData.TabIndex = 11;
            // 
            // _tbmAccessControlList
            // 
            this._tbmAccessControlList.AllowDrop = true;
            this._tbmAccessControlList.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmAccessControlList.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAccessControlList.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmAccessControlList.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmAccessControlList.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmAccessControlList.Button.Image")));
            this._tbmAccessControlList.Button.Location = new System.Drawing.Point(136, 0);
            this._tbmAccessControlList.Button.Name = "_bMenu";
            this._tbmAccessControlList.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmAccessControlList.Button.TabIndex = 3;
            this._tbmAccessControlList.Button.UseVisualStyleBackColor = false;
            this._tbmAccessControlList.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmAccessControlList.ButtonDefaultBehaviour = true;
            this._tbmAccessControlList.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmAccessControlList.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmAccessControlList.ButtonImage")));
            // 
            // 
            // 
            this._tbmAccessControlList.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiCreate});
            this._tbmAccessControlList.ButtonPopupMenu.Name = "";
            this._tbmAccessControlList.ButtonPopupMenu.Size = new System.Drawing.Size(113, 48);
            this._tbmAccessControlList.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmAccessControlList.ButtonShowImage = true;
            this._tbmAccessControlList.ButtonSizeHeight = 20;
            this._tbmAccessControlList.ButtonSizeWidth = 20;
            this._tbmAccessControlList.ButtonText = "";
            this._tbmAccessControlList.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAccessControlList.HoverTime = 600;
            // 
            // 
            // 
            this._tbmAccessControlList.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAccessControlList.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmAccessControlList.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmAccessControlList.ImageTextBox.ContextMenuStrip = this._tbmAccessControlList.ButtonPopupMenu;
            this._tbmAccessControlList.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAccessControlList.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmAccessControlList.ImageTextBox.Image")));
            this._tbmAccessControlList.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmAccessControlList.ImageTextBox.Name = "_textBox";
            this._tbmAccessControlList.ImageTextBox.NoTextNoImage = true;
            this._tbmAccessControlList.ImageTextBox.ReadOnly = true;
            this._tbmAccessControlList.ImageTextBox.Size = new System.Drawing.Size(136, 20);
            this._tbmAccessControlList.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmAccessControlList.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAccessControlList.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmAccessControlList.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmAccessControlList.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAccessControlList.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmAccessControlList.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmAccessControlList.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmAccessControlList.ImageTextBox.TextBox.Size = new System.Drawing.Size(134, 13);
            this._tbmAccessControlList.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmAccessControlList.ImageTextBox.UseImage = true;
            this._tbmAccessControlList.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmAccessControlList_DoubleClick);
            this._tbmAccessControlList.Location = new System.Drawing.Point(12, 25);
            this._tbmAccessControlList.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmAccessControlList.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmAccessControlList.Name = "_tbmAccessControlList";
            this._tbmAccessControlList.Size = new System.Drawing.Size(156, 22);
            this._tbmAccessControlList.TabIndex = 1;
            this._tbmAccessControlList.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmAccessControlList.TextImage")));
            this._tbmAccessControlList.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmAccessControlList_DragOver);
            this._tbmAccessControlList.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmAccessControlList_DragDrop);
            this._tbmAccessControlList.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmAccessControlList_ButtonPopupMenuItemClick);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(112, 22);
            this._tsiModify.Text = "Modify";
            // 
            // _tsiCreate
            // 
            this._tsiCreate.Name = "_tsiCreate";
            this._tsiCreate.Size = new System.Drawing.Size(112, 22);
            this._tsiCreate.Text = "Create";
            // 
            // _tbdpDateTo
            // 
            this._tbdpDateTo.addActualTime = false;
            this._tbdpDateTo.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateTo.ButtonClearDateImage = null;
            this._tbdpDateTo.ButtonClearDateText = "";
            this._tbdpDateTo.ButtonClearDateWidth = 23;
            this._tbdpDateTo.ButtonDateImage = null;
            this._tbdpDateTo.ButtonDateText = "";
            this._tbdpDateTo.ButtonDateWidth = 23;
            this._tbdpDateTo.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._tbdpDateTo.DateFormName = "Calendar";
            this._tbdpDateTo.LocalizationHelper = null;
            this._tbdpDateTo.Location = new System.Drawing.Point(402, 25);
            this._tbdpDateTo.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateTo.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateTo.Name = "_tbdpDateTo";
            this._tbdpDateTo.ReadOnly = false;
            this._tbdpDateTo.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.EndOfDay;
            this._tbdpDateTo.Size = new System.Drawing.Size(202, 22);
            this._tbdpDateTo.TabIndex = 5;
            this._tbdpDateTo.ValidateAfter = 2;
            this._tbdpDateTo.ValidationEnabled = false;
            this._tbdpDateTo.ValidationError = "";
            this._tbdpDateTo.Value = null;
            // 
            // _tbdpDateFrom
            // 
            this._tbdpDateFrom.addActualTime = false;
            this._tbdpDateFrom.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateFrom.ButtonClearDateImage = null;
            this._tbdpDateFrom.ButtonClearDateText = "";
            this._tbdpDateFrom.ButtonClearDateWidth = 23;
            this._tbdpDateFrom.ButtonDateImage = null;
            this._tbdpDateFrom.ButtonDateText = "";
            this._tbdpDateFrom.ButtonDateWidth = 23;
            this._tbdpDateFrom.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._tbdpDateFrom.DateFormName = "Calendar";
            this._tbdpDateFrom.LocalizationHelper = null;
            this._tbdpDateFrom.Location = new System.Drawing.Point(194, 25);
            this._tbdpDateFrom.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateFrom.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateFrom.Name = "_tbdpDateFrom";
            this._tbdpDateFrom.ReadOnly = false;
            this._tbdpDateFrom.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.StartOfDay;
            this._tbdpDateFrom.Size = new System.Drawing.Size(202, 22);
            this._tbdpDateFrom.TabIndex = 3;
            this._tbdpDateFrom.ValidateAfter = 2;
            this._tbdpDateFrom.ValidationEnabled = false;
            this._tbdpDateFrom.ValidationError = "";
            this._tbdpDateFrom.Value = null;
            // 
            // _lDateTo
            // 
            this._lDateTo.AutoSize = true;
            this._lDateTo.Location = new System.Drawing.Point(399, 9);
            this._lDateTo.Name = "_lDateTo";
            this._lDateTo.Size = new System.Drawing.Size(42, 13);
            this._lDateTo.TabIndex = 4;
            this._lDateTo.Text = "Date to";
            // 
            // _bCreate1
            // 
            this._bCreate1.Location = new System.Drawing.Point(438, 53);
            this._bCreate1.Name = "_bCreate1";
            this._bCreate1.Size = new System.Drawing.Size(166, 22);
            this._bCreate1.TabIndex = 6;
            this._bCreate1.Text = "Create";
            this._bCreate1.UseVisualStyleBackColor = true;
            this._bCreate1.Click += new System.EventHandler(this._bCreate1_Click);
            // 
            // _lDateFrom
            // 
            this._lDateFrom.AutoSize = true;
            this._lDateFrom.Location = new System.Drawing.Point(191, 9);
            this._lDateFrom.Name = "_lDateFrom";
            this._lDateFrom.Size = new System.Drawing.Size(53, 13);
            this._lDateFrom.TabIndex = 2;
            this._lDateFrom.Text = "Date from";
            // 
            // _bCancelEdit
            // 
            this._bCancelEdit.Location = new System.Drawing.Point(524, 53);
            this._bCancelEdit.Name = "_bCancelEdit";
            this._bCancelEdit.Size = new System.Drawing.Size(80, 22);
            this._bCancelEdit.TabIndex = 7;
            this._bCancelEdit.Text = "Cancel edit";
            this._bCancelEdit.UseVisualStyleBackColor = true;
            this._bCancelEdit.Visible = false;
            this._bCancelEdit.Click += new System.EventHandler(this._bCancelEdit_Click);
            // 
            // _bUpdate
            // 
            this._bUpdate.Location = new System.Drawing.Point(438, 53);
            this._bUpdate.Name = "_bUpdate";
            this._bUpdate.Size = new System.Drawing.Size(80, 22);
            this._bUpdate.TabIndex = 3;
            this._bUpdate.Text = "Update";
            this._bUpdate.UseVisualStyleBackColor = true;
            this._bUpdate.Visible = false;
            this._bUpdate.Click += new System.EventHandler(this._bUpdate_Click);
            // 
            // _lAccessControlList
            // 
            this._lAccessControlList.AutoSize = true;
            this._lAccessControlList.Location = new System.Drawing.Point(9, 9);
            this._lAccessControlList.Name = "_lAccessControlList";
            this._lAccessControlList.Size = new System.Drawing.Size(92, 13);
            this._lAccessControlList.TabIndex = 0;
            this._lAccessControlList.Text = "Access control list";
            // 
            // NCASACLPersonEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(618, 325);
            this.Controls.Add(this._pBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(405, 334);
            this.Name = "NCASACLPersonEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NCASACLPersonEditForm";
            this._pBack.ResumeLayout(false);
            this._pBack.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.Label _lAccessControlList;
        private System.Windows.Forms.Button _bCreate1;
        private System.Windows.Forms.Button _bCancelEdit;
        private System.Windows.Forms.Button _bUpdate;
        private System.Windows.Forms.Label _lDateTo;
        private System.Windows.Forms.Label _lDateFrom;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateTo;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateFrom;
        private Contal.IwQuick.UI.TextBoxMenu _tbmAccessControlList;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
    }
}
