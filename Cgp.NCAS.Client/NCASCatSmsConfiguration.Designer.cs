namespace Contal.Cgp.NCAS.Client
{
    partial class NCASCatSmsConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASCatSmsConfiguration));
            this._lSmsPrefix = new System.Windows.Forms.Label();
            this._lCat = new System.Windows.Forms.Label();
            this._gbPhoneNumbers = new System.Windows.Forms.GroupBox();
            this._bAdd = new System.Windows.Forms.Button();
            this._bRemove = new System.Windows.Forms.Button();
            this._lbPhoneNumbers = new System.Windows.Forms.ListBox();
            this._tbNewPhoneNumber = new System.Windows.Forms.TextBox();
            this._lNewPhoneNumber = new System.Windows.Forms.Label();
            this._tbSmsMessage = new System.Windows.Forms.TextBox();
            this._tbmCat = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._gbPhoneNumbers.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lSmsPrefix
            // 
            this._lSmsPrefix.AutoSize = true;
            this._lSmsPrefix.Location = new System.Drawing.Point(10, 53);
            this._lSmsPrefix.Name = "_lSmsPrefix";
            this._lSmsPrefix.Size = new System.Drawing.Size(53, 13);
            this._lSmsPrefix.TabIndex = 8;
            this._lSmsPrefix.Text = "SmsPrefix";
            // 
            // _lCat
            // 
            this._lCat.AutoSize = true;
            this._lCat.Location = new System.Drawing.Point(10, 9);
            this._lCat.Name = "_lCat";
            this._lCat.Size = new System.Drawing.Size(85, 13);
            this._lCat.TabIndex = 7;
            this._lCat.Text = "AlarmTransmitter";
            // 
            // _gbPhoneNumbers
            // 
            this._gbPhoneNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbPhoneNumbers.Controls.Add(this._bAdd);
            this._gbPhoneNumbers.Controls.Add(this._bRemove);
            this._gbPhoneNumbers.Controls.Add(this._lbPhoneNumbers);
            this._gbPhoneNumbers.Controls.Add(this._tbNewPhoneNumber);
            this._gbPhoneNumbers.Controls.Add(this._lNewPhoneNumber);
            this._gbPhoneNumbers.Location = new System.Drawing.Point(310, 9);
            this._gbPhoneNumbers.Name = "_gbPhoneNumbers";
            this._gbPhoneNumbers.Size = new System.Drawing.Size(300, 304);
            this._gbPhoneNumbers.TabIndex = 6;
            this._gbPhoneNumbers.TabStop = false;
            this._gbPhoneNumbers.Text = "PhoneNumbers";
            // 
            // _bAdd
            // 
            this._bAdd.Location = new System.Drawing.Point(6, 63);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(83, 27);
            this._bAdd.TabIndex = 6;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _bRemove
            // 
            this._bRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRemove.Location = new System.Drawing.Point(6, 271);
            this._bRemove.Name = "_bRemove";
            this._bRemove.Size = new System.Drawing.Size(83, 27);
            this._bRemove.TabIndex = 5;
            this._bRemove.Text = "Remove";
            this._bRemove.UseVisualStyleBackColor = true;
            this._bRemove.Click += new System.EventHandler(this._bRemove_Click);
            // 
            // _lbPhoneNumbers
            // 
            this._lbPhoneNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbPhoneNumbers.FormattingEnabled = true;
            this._lbPhoneNumbers.Location = new System.Drawing.Point(6, 96);
            this._lbPhoneNumbers.Name = "_lbPhoneNumbers";
            this._lbPhoneNumbers.Size = new System.Drawing.Size(285, 160);
            this._lbPhoneNumbers.TabIndex = 4;
            // 
            // _tbNewPhoneNumber
            // 
            this._tbNewPhoneNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbNewPhoneNumber.Location = new System.Drawing.Point(6, 38);
            this._tbNewPhoneNumber.Name = "_tbNewPhoneNumber";
            this._tbNewPhoneNumber.Size = new System.Drawing.Size(285, 20);
            this._tbNewPhoneNumber.TabIndex = 3;
            // 
            // _lNewPhoneNumber
            // 
            this._lNewPhoneNumber.AutoSize = true;
            this._lNewPhoneNumber.Location = new System.Drawing.Point(6, 22);
            this._lNewPhoneNumber.Name = "_lNewPhoneNumber";
            this._lNewPhoneNumber.Size = new System.Drawing.Size(97, 13);
            this._lNewPhoneNumber.TabIndex = 3;
            this._lNewPhoneNumber.Text = "NewPhoneNumber";
            // 
            // _tbSmsMessage
            // 
            this._tbSmsMessage.Location = new System.Drawing.Point(12, 69);
            this._tbSmsMessage.Multiline = true;
            this._tbSmsMessage.Name = "_tbSmsMessage";
            this._tbSmsMessage.Size = new System.Drawing.Size(231, 107);
            this._tbSmsMessage.TabIndex = 5;
            this._tbSmsMessage.TextChanged += new System.EventHandler(this._tbSmsMessage_TextChanged);
            // 
            // _tbmCat
            // 
            this._tbmCat.AllowDrop = true;
            this._tbmCat.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmCat.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCat.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmCat.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmCat.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmCat.Button.Image")));
            this._tbmCat.Button.Location = new System.Drawing.Point(210, 0);
            this._tbmCat.Button.Name = "_bMenu";
            this._tbmCat.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmCat.Button.TabIndex = 3;
            this._tbmCat.Button.UseVisualStyleBackColor = false;
            this._tbmCat.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmCat.ButtonDefaultBehaviour = true;
            this._tbmCat.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmCat.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmCat.ButtonImage")));
            // 
            // 
            // 
            this._tbmCat.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiRemove});
            this._tbmCat.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmCat.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmCat.ButtonShowImage = true;
            this._tbmCat.ButtonSizeHeight = 20;
            this._tbmCat.ButtonSizeWidth = 20;
            this._tbmCat.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCat.HoverTime = 500;
            // 
            // 
            // 
            this._tbmCat.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCat.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmCat.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmCat.ImageTextBox.ContextMenuStrip = this._tbmCat.ButtonPopupMenu;
            this._tbmCat.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCat.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmCat.ImageTextBox.Image")));
            this._tbmCat.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmCat.ImageTextBox.Name = "_textBox";
            this._tbmCat.ImageTextBox.NoTextNoImage = true;
            this._tbmCat.ImageTextBox.ReadOnly = true;
            this._tbmCat.ImageTextBox.Size = new System.Drawing.Size(210, 20);
            this._tbmCat.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmCat.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCat.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmCat.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmCat.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCat.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmCat.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmCat.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmCat.ImageTextBox.TextBox.Size = new System.Drawing.Size(208, 13);
            this._tbmCat.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmCat.ImageTextBox.UseImage = true;
            this._tbmCat.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmCat_ImageTextBox_DoubleClick);
            this._tbmCat.Location = new System.Drawing.Point(13, 25);
            this._tbmCat.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmCat.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmCat.Name = "_tbmCat";
            this._tbmCat.Size = new System.Drawing.Size(230, 22);
            this._tbmCat.TabIndex = 4;
            this._tbmCat.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmCat.TextImage")));
            this._tbmCat.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmCat_ButtonPopupMenuItemClick);
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
            // NCASCatSmsConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(620, 319);
            this.Controls.Add(this._lCat);
            this.Controls.Add(this._lSmsPrefix);
            this.Controls.Add(this._tbmCat);
            this.Controls.Add(this._tbSmsMessage);
            this.Controls.Add(this._gbPhoneNumbers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NCASCatSmsConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NCASCatSmsConfiguration";
            this._gbPhoneNumbers.ResumeLayout(false);
            this._gbPhoneNumbers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lSmsPrefix;
        private System.Windows.Forms.Label _lCat;
        private System.Windows.Forms.GroupBox _gbPhoneNumbers;
        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.Button _bRemove;
        private System.Windows.Forms.ListBox _lbPhoneNumbers;
        private System.Windows.Forms.TextBox _tbNewPhoneNumber;
        private System.Windows.Forms.Label _lNewPhoneNumber;
        private System.Windows.Forms.TextBox _tbSmsMessage;
        private Contal.IwQuick.UI.TextBoxMenu _tbmCat;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
    }
}