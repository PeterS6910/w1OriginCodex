namespace Contal.Cgp.NCAS.Client
{
    partial class AddAACardReaderDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddAACardReaderDialog));
            this._cbEnableEventlog = new System.Windows.Forms.CheckBox();
            this._cbUnconditionalSet = new System.Windows.Forms.CheckBox();
            this._tbmCardReader = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._cbImplicitMember = new System.Windows.Forms.CheckBox();
            this._cbAlarmAreaUnset = new System.Windows.Forms.CheckBox();
            this._cbAlarmAreaSet = new System.Windows.Forms.CheckBox();
            this._bInsert = new System.Windows.Forms.Button();
            this._bClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _cbEnableEventlog
            // 
            this._cbEnableEventlog.Location = new System.Drawing.Point(15, 83);
            this._cbEnableEventlog.Name = "_cbEnableEventlog";
            this._cbEnableEventlog.Size = new System.Drawing.Size(120, 17);
            this._cbEnableEventlog.TabIndex = 17;
            this._cbEnableEventlog.Text = "Enable eventlog";
            this._cbEnableEventlog.UseVisualStyleBackColor = true;
            // 
            // _cbUnconditionalSet
            // 
            this._cbUnconditionalSet.Location = new System.Drawing.Point(141, 37);
            this._cbUnconditionalSet.Name = "_cbUnconditionalSet";
            this._cbUnconditionalSet.Size = new System.Drawing.Size(175, 17);
            this._cbUnconditionalSet.TabIndex = 15;
            this._cbUnconditionalSet.Text = "Alarm area unconditional set";
            this._cbUnconditionalSet.UseVisualStyleBackColor = true;
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
            this._tbmCardReader.Button.Location = new System.Drawing.Point(229, 0);
            this._tbmCardReader.Button.Name = "_bMenu";
            this._tbmCardReader.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmCardReader.Button.TabIndex = 3;
            this._tbmCardReader.Button.UseVisualStyleBackColor = false;
            this._tbmCardReader.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmCardReader.ButtonDefaultBehaviour = true;
            this._tbmCardReader.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmCardReader.ButtonImage = null;
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
            this._tbmCardReader.ImageTextBox.Image = null;
            this._tbmCardReader.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmCardReader.ImageTextBox.Name = "_textBox";
            this._tbmCardReader.ImageTextBox.NoTextNoImage = true;
            this._tbmCardReader.ImageTextBox.ReadOnly = true;
            this._tbmCardReader.ImageTextBox.Size = new System.Drawing.Size(229, 20);
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
            this._tbmCardReader.ImageTextBox.TextBox.Size = new System.Drawing.Size(227, 13);
            this._tbmCardReader.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmCardReader.ImageTextBox.UseImage = true;
            this._tbmCardReader.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmCardReader_DoubleClick);
            this._tbmCardReader.Location = new System.Drawing.Point(13, 9);
            this._tbmCardReader.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmCardReader.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmCardReader.Name = "_tbmCardReader";
            this._tbmCardReader.Size = new System.Drawing.Size(249, 22);
            this._tbmCardReader.TabIndex = 12;
            this._tbmCardReader.TextImage = null;
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
            // _cbImplicitMember
            // 
            this._cbImplicitMember.Checked = true;
            this._cbImplicitMember.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbImplicitMember.Location = new System.Drawing.Point(141, 60);
            this._cbImplicitMember.Name = "_cbImplicitMember";
            this._cbImplicitMember.Size = new System.Drawing.Size(119, 17);
            this._cbImplicitMember.TabIndex = 16;
            this._cbImplicitMember.Text = "Implicit member";
            this._cbImplicitMember.UseVisualStyleBackColor = true;
            // 
            // _cbAlarmAreaUnset
            // 
            this._cbAlarmAreaUnset.Location = new System.Drawing.Point(15, 60);
            this._cbAlarmAreaUnset.Name = "_cbAlarmAreaUnset";
            this._cbAlarmAreaUnset.Size = new System.Drawing.Size(120, 17);
            this._cbAlarmAreaUnset.TabIndex = 14;
            this._cbAlarmAreaUnset.Text = "Alarm area unset";
            this._cbAlarmAreaUnset.UseVisualStyleBackColor = true;
            // 
            // _cbAlarmAreaSet
            // 
            this._cbAlarmAreaSet.Location = new System.Drawing.Point(15, 37);
            this._cbAlarmAreaSet.Name = "_cbAlarmAreaSet";
            this._cbAlarmAreaSet.Size = new System.Drawing.Size(130, 17);
            this._cbAlarmAreaSet.TabIndex = 13;
            this._cbAlarmAreaSet.Text = "Alarm area set";
            this._cbAlarmAreaSet.UseVisualStyleBackColor = true;
            // 
            // _bInsert
            // 
            this._bInsert.Location = new System.Drawing.Point(158, 127);
            this._bInsert.Margin = new System.Windows.Forms.Padding(2);
            this._bInsert.Name = "_bInsert";
            this._bInsert.Size = new System.Drawing.Size(75, 23);
            this._bInsert.TabIndex = 18;
            this._bInsert.Text = "Insert";
            this._bInsert.UseVisualStyleBackColor = true;
            this._bInsert.Click += new System.EventHandler(this._bInsert_Click);
            // 
            // _bClose
            // 
            this._bClose.Location = new System.Drawing.Point(241, 127);
            this._bClose.Margin = new System.Windows.Forms.Padding(2);
            this._bClose.Name = "_bClose";
            this._bClose.Size = new System.Drawing.Size(75, 23);
            this._bClose.TabIndex = 19;
            this._bClose.Text = "Close";
            this._bClose.UseVisualStyleBackColor = true;
            this._bClose.Click += new System.EventHandler(this._bClose_Click);
            // 
            // AddAACardReaderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(324, 161);
            this.Controls.Add(this._bClose);
            this.Controls.Add(this._bInsert);
            this.Controls.Add(this._cbEnableEventlog);
            this.Controls.Add(this._cbUnconditionalSet);
            this.Controls.Add(this._tbmCardReader);
            this.Controls.Add(this._cbImplicitMember);
            this.Controls.Add(this._cbAlarmAreaUnset);
            this.Controls.Add(this._cbAlarmAreaSet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddAACardReaderDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AddAACardReaderDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox _cbEnableEventlog;
        private System.Windows.Forms.CheckBox _cbUnconditionalSet;
        private Contal.IwQuick.UI.TextBoxMenu _tbmCardReader;
        private System.Windows.Forms.CheckBox _cbImplicitMember;
        private System.Windows.Forms.CheckBox _cbAlarmAreaUnset;
        private System.Windows.Forms.CheckBox _cbAlarmAreaSet;
        private System.Windows.Forms.Button _bInsert;
        private System.Windows.Forms.Button _bClose;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
    }
}
