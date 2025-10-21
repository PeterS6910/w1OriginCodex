namespace Contal.Cgp.NCAS.Client
{
    partial class NCASAccessZonePersonEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASAccessZonePersonEditForm));
            this._pBack = new System.Windows.Forms.Panel();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._tbmCardReader = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tbmTimeZone = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate2 = new System.Windows.Forms.ToolStripMenuItem();
            this._bUpdate = new System.Windows.Forms.Button();
            this._lDescription = new System.Windows.Forms.Label();
            this._bCancel = new System.Windows.Forms.Button();
            this._bCreate0 = new System.Windows.Forms.Button();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._lCardReader = new System.Windows.Forms.Label();
            this._lTimeZone = new System.Windows.Forms.Label();
            this._pBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pBack
            // 
            this._pBack.AutoSize = true;
            this._pBack.Controls.Add(this._cdgvData);
            this._pBack.Controls.Add(this._tbmCardReader);
            this._pBack.Controls.Add(this._tbmTimeZone);
            this._pBack.Controls.Add(this._bUpdate);
            this._pBack.Controls.Add(this._lDescription);
            this._pBack.Controls.Add(this._bCancel);
            this._pBack.Controls.Add(this._bCreate0);
            this._pBack.Controls.Add(this._eDescription);
            this._pBack.Controls.Add(this._lCardReader);
            this._pBack.Controls.Add(this._lTimeZone);
            this._pBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pBack.Location = new System.Drawing.Point(0, 0);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(423, 295);
            this._pBack.TabIndex = 0;
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvData.CgpDataGridEvents = null;
            this._cdgvData.CopyOnRightClick = true;
            // 
            // 
            // 
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
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(412, 177);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(6, 114);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(412, 177);
            this._cdgvData.TabIndex = 11;
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
            this._tbmCardReader.Button.Location = new System.Drawing.Point(136, 0);
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
            this._tsiModify});
            this._tbmCardReader.ButtonPopupMenu.Name = "";
            this._tbmCardReader.ButtonPopupMenu.Size = new System.Drawing.Size(113, 26);
            this._tbmCardReader.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmCardReader.ButtonShowImage = true;
            this._tbmCardReader.ButtonSizeHeight = 20;
            this._tbmCardReader.ButtonSizeWidth = 20;
            this._tbmCardReader.ButtonText = "";
            this._tbmCardReader.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCardReader.HoverTime = 600;
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
            this._tbmCardReader.ImageTextBox.Size = new System.Drawing.Size(136, 20);
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
            this._tbmCardReader.ImageTextBox.TextBox.Size = new System.Drawing.Size(134, 13);
            this._tbmCardReader.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmCardReader.ImageTextBox.UseImage = true;
            this._tbmCardReader.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmCardReader_DoubleClick);
            this._tbmCardReader.Location = new System.Drawing.Point(6, 21);
            this._tbmCardReader.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmCardReader.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmCardReader.Name = "_tbmCardReader";
            this._tbmCardReader.Size = new System.Drawing.Size(156, 22);
            this._tbmCardReader.TabIndex = 2;
            this._tbmCardReader.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmCardReader.TextImage")));
            this._tbmCardReader.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmCardReader_DragOver);
            this._tbmCardReader.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmCardReader_DragDrop);
            this._tbmCardReader.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmCardReader_ButtonPopupMenuItemClick);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(112, 22);
            this._tsiModify.Text = "Modify";
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
            this._tbmTimeZone.Button.Location = new System.Drawing.Point(217, 0);
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
            this._tsiModify2,
            this._tsiRemove,
            this._tsiCreate2});
            this._tbmTimeZone.ButtonPopupMenu.Name = "";
            this._tbmTimeZone.ButtonPopupMenu.Size = new System.Drawing.Size(118, 70);
            this._tbmTimeZone.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmTimeZone.ButtonShowImage = true;
            this._tbmTimeZone.ButtonSizeHeight = 20;
            this._tbmTimeZone.ButtonSizeWidth = 20;
            this._tbmTimeZone.ButtonText = "";
            this._tbmTimeZone.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZone.HoverTime = 600;
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
            this._tbmTimeZone.ImageTextBox.Size = new System.Drawing.Size(217, 20);
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
            this._tbmTimeZone.ImageTextBox.TextBox.Size = new System.Drawing.Size(215, 13);
            this._tbmTimeZone.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmTimeZone.ImageTextBox.UseImage = true;
            this._tbmTimeZone.ImageTextBox.DoubleClick += new System.EventHandler(this.TimeZoneDoubleClick);
            this._tbmTimeZone.Location = new System.Drawing.Point(168, 21);
            this._tbmTimeZone.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmTimeZone.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmTimeZone.Name = "_tbmTimeZone";
            this._tbmTimeZone.Size = new System.Drawing.Size(237, 22);
            this._tbmTimeZone.TabIndex = 3;
            this._tbmTimeZone.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZone.TextImage")));
            this._tbmTimeZone.DragOver += new System.Windows.Forms.DragEventHandler(this.TimeZoneDragOver);
            this._tbmTimeZone.DragDrop += new System.Windows.Forms.DragEventHandler(this.TimeZoneDragDrop);
            this._tbmTimeZone.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmTimeZone_ButtonPopupMenuItemClick);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(117, 22);
            this._tsiModify2.Text = "Modify";
            // 
            // _tsiRemove
            // 
            this._tsiRemove.Name = "_tsiRemove";
            this._tsiRemove.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove.Text = "Remove";
            // 
            // _tsiCreate2
            // 
            this._tsiCreate2.Name = "_tsiCreate2";
            this._tsiCreate2.Size = new System.Drawing.Size(117, 22);
            this._tsiCreate2.Text = "Create";
            // 
            // _bUpdate
            // 
            this._bUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bUpdate.Location = new System.Drawing.Point(262, 85);
            this._bUpdate.Name = "_bUpdate";
            this._bUpdate.Size = new System.Drawing.Size(75, 23);
            this._bUpdate.TabIndex = 6;
            this._bUpdate.Text = "Update";
            this._bUpdate.UseVisualStyleBackColor = true;
            this._bUpdate.Click += new System.EventHandler(this._bUpdate_Click);
            // 
            // _lDescription
            // 
            this._lDescription.AutoSize = true;
            this._lDescription.Location = new System.Drawing.Point(3, 46);
            this._lDescription.Name = "_lDescription";
            this._lDescription.Size = new System.Drawing.Size(60, 13);
            this._lDescription.TabIndex = 4;
            this._lDescription.Text = "Description";
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(343, 85);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bCreate0
            // 
            this._bCreate0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreate0.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCreate0.Location = new System.Drawing.Point(262, 85);
            this._bCreate0.Name = "_bCreate0";
            this._bCreate0.Size = new System.Drawing.Size(75, 23);
            this._bCreate0.TabIndex = 4;
            this._bCreate0.Text = "Create";
            this._bCreate0.UseVisualStyleBackColor = true;
            this._bCreate0.Click += new System.EventHandler(this._bCreate0_Click);
            // 
            // _eDescription
            // 
            this._eDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDescription.Location = new System.Drawing.Point(6, 62);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(249, 46);
            this._eDescription.TabIndex = 5;
            // 
            // _lCardReader
            // 
            this._lCardReader.AutoSize = true;
            this._lCardReader.Location = new System.Drawing.Point(6, 5);
            this._lCardReader.Name = "_lCardReader";
            this._lCardReader.Size = new System.Drawing.Size(62, 13);
            this._lCardReader.TabIndex = 0;
            this._lCardReader.Text = "Card reader";
            // 
            // _lTimeZone
            // 
            this._lTimeZone.AutoSize = true;
            this._lTimeZone.Location = new System.Drawing.Point(165, 5);
            this._lTimeZone.Name = "_lTimeZone";
            this._lTimeZone.Size = new System.Drawing.Size(56, 13);
            this._lTimeZone.TabIndex = 1;
            this._lTimeZone.Text = "Time zone";
            // 
            // NCASAccessZonePersonEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(423, 295);
            this.Controls.Add(this._pBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "NCASAccessZonePersonEditForm";
            this.Text = "NCASAccessZonePersonEditForm";
            this._pBack.ResumeLayout(false);
            this._pBack.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.Button _bCreate0;
        private System.Windows.Forms.Label _lCardReader;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Label _lDescription;
        private System.Windows.Forms.Label _lTimeZone;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bUpdate;
        private Contal.IwQuick.UI.TextBoxMenu _tbmCardReader;
        private Contal.IwQuick.UI.TextBoxMenu _tbmTimeZone;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate2;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
    }
}
