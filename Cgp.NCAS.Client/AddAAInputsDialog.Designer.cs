namespace Contal.Cgp.NCAS.Client
{
    partial class AddAAInputsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddAAInputsDialog));
            this._lBlockTemporarilyUntil = new System.Windows.Forms.Label();
            this._cbBlockTemporarilyUntil = new System.Windows.Forms.ComboBox();
            this._chbLowSecurityInput = new System.Windows.Forms.CheckBox();
            this._tbmInput = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._bInsert = new System.Windows.Forms.Button();
            this._bClose = new System.Windows.Forms.Button();
            this._cbSymbolPurpose = new System.Windows.Forms.ComboBox();
            this._lSymbolPurpose = new System.Windows.Forms.Label();
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._pbSensorTemporarilyBlockingOnlyInSabotageInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // _lBlockTemporarilyUntil
            // 
            this._lBlockTemporarilyUntil.Location = new System.Drawing.Point(9, 48);
            this._lBlockTemporarilyUntil.Name = "_lBlockTemporarilyUntil";
            this._lBlockTemporarilyUntil.Size = new System.Drawing.Size(125, 17);
            this._lBlockTemporarilyUntil.TabIndex = 15;
            this._lBlockTemporarilyUntil.Text = "Block temporarily until";
            this._lBlockTemporarilyUntil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cbBlockTemporarilyUntil
            // 
            this._cbBlockTemporarilyUntil.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cbBlockTemporarilyUntil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbBlockTemporarilyUntil.FormattingEnabled = true;
            this._cbBlockTemporarilyUntil.Location = new System.Drawing.Point(171, 47);
            this._cbBlockTemporarilyUntil.Name = "_cbBlockTemporarilyUntil";
            this._cbBlockTemporarilyUntil.Size = new System.Drawing.Size(130, 21);
            this._cbBlockTemporarilyUntil.TabIndex = 19;
            // 
            // _chbLowSecurityInput
            // 
            this._chbLowSecurityInput.Location = new System.Drawing.Point(12, 111);
            this._chbLowSecurityInput.Name = "_chbLowSecurityInput";
            this._chbLowSecurityInput.Size = new System.Drawing.Size(130, 17);
            this._chbLowSecurityInput.TabIndex = 18;
            this._chbLowSecurityInput.Text = "Low security input";
            this._chbLowSecurityInput.UseVisualStyleBackColor = true;
            this._chbLowSecurityInput.CheckedChanged += new System.EventHandler(this._chbLowSecurityInput_CheckedChanged);
            // 
            // _tbmInput
            // 
            this._tbmInput.AllowDrop = true;
            this._tbmInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmInput.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmInput.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmInput.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmInput.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmInput.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmInput.Button.Image")));
            this._tbmInput.Button.Location = new System.Drawing.Point(269, 0);
            this._tbmInput.Button.Name = "_bMenu";
            this._tbmInput.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmInput.Button.TabIndex = 3;
            this._tbmInput.Button.UseVisualStyleBackColor = false;
            this._tbmInput.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmInput.ButtonDefaultBehaviour = true;
            this._tbmInput.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmInput.ButtonImage = null;
            // 
            // 
            // 
            this._tbmInput.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify});
            this._tbmInput.ButtonPopupMenu.Name = "";
            this._tbmInput.ButtonPopupMenu.Size = new System.Drawing.Size(113, 26);
            this._tbmInput.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmInput.ButtonShowImage = true;
            this._tbmInput.ButtonSizeHeight = 20;
            this._tbmInput.ButtonSizeWidth = 20;
            this._tbmInput.ButtonText = "";
            this._tbmInput.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmInput.HoverTime = 500;
            // 
            // 
            // 
            this._tbmInput.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmInput.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmInput.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmInput.ImageTextBox.ContextMenuStrip = this._tbmInput.ButtonPopupMenu;
            this._tbmInput.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmInput.ImageTextBox.Image = null;
            this._tbmInput.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmInput.ImageTextBox.Name = "_textBox";
            this._tbmInput.ImageTextBox.NoTextNoImage = true;
            this._tbmInput.ImageTextBox.ReadOnly = true;
            this._tbmInput.ImageTextBox.Size = new System.Drawing.Size(269, 20);
            this._tbmInput.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmInput.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmInput.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmInput.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmInput.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmInput.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmInput.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmInput.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmInput.ImageTextBox.TextBox.Size = new System.Drawing.Size(267, 13);
            this._tbmInput.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmInput.ImageTextBox.UseImage = true;
            this._tbmInput.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmInput_DoubleClick);
            this._tbmInput.Location = new System.Drawing.Point(12, 12);
            this._tbmInput.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmInput.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmInput.Name = "_tbmInput";
            this._tbmInput.Size = new System.Drawing.Size(258, 22);
            this._tbmInput.TabIndex = 16;
            this._tbmInput.TextImage = null;
            this._tbmInput.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmInput_DragOver);
            this._tbmInput.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmInput_DragDrop);
            this._tbmInput.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmInput_ButtonPopupMenuItemClick);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(112, 22);
            this._tsiModify.Text = "Modify";
            // 
            // _bInsert
            // 
            this._bInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bInsert.Location = new System.Drawing.Point(147, 133);
            this._bInsert.Margin = new System.Windows.Forms.Padding(2);
            this._bInsert.Name = "_bInsert";
            this._bInsert.Size = new System.Drawing.Size(75, 23);
            this._bInsert.TabIndex = 17;
            this._bInsert.Text = "Insert";
            this._bInsert.UseVisualStyleBackColor = true;
            this._bInsert.Click += new System.EventHandler(this._bInsert_Click);
            // 
            // _bClose
            // 
            this._bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bClose.Location = new System.Drawing.Point(226, 133);
            this._bClose.Margin = new System.Windows.Forms.Padding(2);
            this._bClose.Name = "_bClose";
            this._bClose.Size = new System.Drawing.Size(75, 23);
            this._bClose.TabIndex = 21;
            this._bClose.Text = "Close";
            this._bClose.UseVisualStyleBackColor = true;
            this._bClose.Click += new System.EventHandler(this._bClose_Click);
            // 
            // _cbSymbolPurpose
            // 
            this._cbSymbolPurpose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cbSymbolPurpose.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSymbolPurpose.FormattingEnabled = true;
            this._cbSymbolPurpose.Location = new System.Drawing.Point(171, 79);
            this._cbSymbolPurpose.Name = "_cbSymbolPurpose";
            this._cbSymbolPurpose.Size = new System.Drawing.Size(130, 21);
            this._cbSymbolPurpose.TabIndex = 23;
            // 
            // _lSymbolPurpose
            // 
            this._lSymbolPurpose.Location = new System.Drawing.Point(9, 80);
            this._lSymbolPurpose.Name = "_lSymbolPurpose";
            this._lSymbolPurpose.Size = new System.Drawing.Size(125, 17);
            this._lSymbolPurpose.TabIndex = 22;
            this._lSymbolPurpose.Text = "SymbolPurpose";
            this._lSymbolPurpose.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _pbSensorTemporarilyBlockingOnlyInSabotageInfo
            // 
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo.Image = global::Contal.Cgp.NCAS.Client.ResourceGlobal.information;
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo.Location = new System.Drawing.Point(140, 45);
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo.Name = "_pbSensorTemporarilyBlockingOnlyInSabotageInfo";
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo.Size = new System.Drawing.Size(25, 25);
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo.TabIndex = 24;
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo.TabStop = false;
            this._pbSensorTemporarilyBlockingOnlyInSabotageInfo.Visible = false;
            // 
            // AddAAInputsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(314, 165);
            this.Controls.Add(this._pbSensorTemporarilyBlockingOnlyInSabotageInfo);
            this.Controls.Add(this._cbSymbolPurpose);
            this.Controls.Add(this._lSymbolPurpose);
            this.Controls.Add(this._bClose);
            this.Controls.Add(this._cbBlockTemporarilyUntil);
            this.Controls.Add(this._chbLowSecurityInput);
            this.Controls.Add(this._tbmInput);
            this.Controls.Add(this._bInsert);
            this.Controls.Add(this._lBlockTemporarilyUntil);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddAAInputsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AddAAInputsDialog";
            ((System.ComponentModel.ISupportInitialize)(this._pbSensorTemporarilyBlockingOnlyInSabotageInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lBlockTemporarilyUntil;
        private System.Windows.Forms.ComboBox _cbBlockTemporarilyUntil;
        private System.Windows.Forms.CheckBox _chbLowSecurityInput;
        private Contal.IwQuick.UI.TextBoxMenu _tbmInput;
        private System.Windows.Forms.Button _bInsert;
        private System.Windows.Forms.Button _bClose;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ComboBox _cbSymbolPurpose;
        private System.Windows.Forms.Label _lSymbolPurpose;
        private System.Windows.Forms.PictureBox _pbSensorTemporarilyBlockingOnlyInSabotageInfo;
    }
}
