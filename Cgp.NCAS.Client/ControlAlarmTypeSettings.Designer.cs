namespace Contal.Cgp.NCAS.Client
{
    partial class ControlAlarmTypeSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlAlarmTypeSettings));
            this._pAlarmEnabled = new System.Windows.Forms.Panel();
            this._chbAlarmEnabled = new System.Windows.Forms.CheckBox();
            this._pBlockAlarm = new System.Windows.Forms.Panel();
            this._chbBlockAlarm = new System.Windows.Forms.CheckBox();
            this._pObjectForBlockingAlarm = new System.Windows.Forms.Panel();
            this._tbmObjectForBlockingAlarm = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove1 = new System.Windows.Forms.ToolStripMenuItem();
            this._lObjectForBlockingAlarm = new System.Windows.Forms.Label();
            this._pEventlogDuringBlockedAlarm = new System.Windows.Forms.Panel();
            this._chbEventlogDuringBlockedAlarm = new System.Windows.Forms.CheckBox();
            this._pPresentationGroup = new System.Windows.Forms.Panel();
            this._tbmPresentationGroup = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate2 = new System.Windows.Forms.ToolStripMenuItem();
            this._lPresentationGroup = new System.Windows.Forms.Label();
            this._pAlarmEnabled.SuspendLayout();
            this._pBlockAlarm.SuspendLayout();
            this._pObjectForBlockingAlarm.SuspendLayout();
            this._pEventlogDuringBlockedAlarm.SuspendLayout();
            this._pPresentationGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pAlarmEnabled
            // 
            this._pAlarmEnabled.BackColor = System.Drawing.SystemColors.Control;
            this._pAlarmEnabled.Controls.Add(this._chbAlarmEnabled);
            this._pAlarmEnabled.Location = new System.Drawing.Point(0, 0);
            this._pAlarmEnabled.Margin = new System.Windows.Forms.Padding(0);
            this._pAlarmEnabled.Name = "_pAlarmEnabled";
            this._pAlarmEnabled.Size = new System.Drawing.Size(400, 27);
            this._pAlarmEnabled.TabIndex = 2;
            // 
            // _chbAlarmEnabled
            // 
            this._chbAlarmEnabled.AutoSize = true;
            this._chbAlarmEnabled.Location = new System.Drawing.Point(3, 3);
            this._chbAlarmEnabled.Margin = new System.Windows.Forms.Padding(0);
            this._chbAlarmEnabled.MaximumSize = new System.Drawing.Size(140, 21);
            this._chbAlarmEnabled.MinimumSize = new System.Drawing.Size(0, 21);
            this._chbAlarmEnabled.Name = "_chbAlarmEnabled";
            this._chbAlarmEnabled.Size = new System.Drawing.Size(93, 21);
            this._chbAlarmEnabled.TabIndex = 3;
            this._chbAlarmEnabled.Text = "Alarm enabled";
            this._chbAlarmEnabled.UseVisualStyleBackColor = true;
            this._chbAlarmEnabled.CheckStateChanged += new System.EventHandler(this.DoEditTextChanger);
            // 
            // _pBlockAlarm
            // 
            this._pBlockAlarm.BackColor = System.Drawing.SystemColors.ControlLight;
            this._pBlockAlarm.Controls.Add(this._chbBlockAlarm);
            this._pBlockAlarm.Location = new System.Drawing.Point(400, 0);
            this._pBlockAlarm.Margin = new System.Windows.Forms.Padding(0);
            this._pBlockAlarm.Name = "_pBlockAlarm";
            this._pBlockAlarm.Size = new System.Drawing.Size(146, 27);
            this._pBlockAlarm.TabIndex = 3;
            // 
            // _chbBlockAlarm
            // 
            this._chbBlockAlarm.AutoSize = true;
            this._chbBlockAlarm.Location = new System.Drawing.Point(3, 3);
            this._chbBlockAlarm.Margin = new System.Windows.Forms.Padding(0);
            this._chbBlockAlarm.MaximumSize = new System.Drawing.Size(140, 21);
            this._chbBlockAlarm.MinimumSize = new System.Drawing.Size(0, 21);
            this._chbBlockAlarm.Name = "_chbBlockAlarm";
            this._chbBlockAlarm.Size = new System.Drawing.Size(81, 21);
            this._chbBlockAlarm.TabIndex = 4;
            this._chbBlockAlarm.Text = "Block alarm";
            this._chbBlockAlarm.UseVisualStyleBackColor = true;
            this._chbBlockAlarm.CheckStateChanged += new System.EventHandler(this.DoEditTextChanger);
            // 
            // _pObjectForBlockingAlarm
            // 
            this._pObjectForBlockingAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pObjectForBlockingAlarm.BackColor = System.Drawing.SystemColors.ControlLight;
            this._pObjectForBlockingAlarm.Controls.Add(this._tbmObjectForBlockingAlarm);
            this._pObjectForBlockingAlarm.Controls.Add(this._lObjectForBlockingAlarm);
            this._pObjectForBlockingAlarm.Location = new System.Drawing.Point(400, 27);
            this._pObjectForBlockingAlarm.Margin = new System.Windows.Forms.Padding(0);
            this._pObjectForBlockingAlarm.Name = "_pObjectForBlockingAlarm";
            this._pObjectForBlockingAlarm.Size = new System.Drawing.Size(400, 42);
            this._pObjectForBlockingAlarm.TabIndex = 4;
            // 
            // _tbmObjectForBlockingAlarm
            // 
            this._tbmObjectForBlockingAlarm.AllowDrop = true;
            this._tbmObjectForBlockingAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjectForBlockingAlarm.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmObjectForBlockingAlarm.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjectForBlockingAlarm.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmObjectForBlockingAlarm.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmObjectForBlockingAlarm.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmObjectForBlockingAlarm.Button.Image")));
            this._tbmObjectForBlockingAlarm.Button.Location = new System.Drawing.Point(374, 0);
            this._tbmObjectForBlockingAlarm.Button.Name = "_bMenu";
            this._tbmObjectForBlockingAlarm.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmObjectForBlockingAlarm.Button.TabIndex = 3;
            this._tbmObjectForBlockingAlarm.Button.UseVisualStyleBackColor = false;
            this._tbmObjectForBlockingAlarm.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmObjectForBlockingAlarm.ButtonDefaultBehaviour = true;
            this._tbmObjectForBlockingAlarm.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmObjectForBlockingAlarm.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmObjectForBlockingAlarm.ButtonImage")));
            // 
            // 
            // 
            this._tbmObjectForBlockingAlarm.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify1,
            this._tsiRemove1});
            this._tbmObjectForBlockingAlarm.ButtonPopupMenu.Name = "";
            this._tbmObjectForBlockingAlarm.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmObjectForBlockingAlarm.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmObjectForBlockingAlarm.ButtonShowImage = true;
            this._tbmObjectForBlockingAlarm.ButtonSizeHeight = 20;
            this._tbmObjectForBlockingAlarm.ButtonSizeWidth = 20;
            this._tbmObjectForBlockingAlarm.ButtonText = "";
            this._tbmObjectForBlockingAlarm.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjectForBlockingAlarm.HoverTime = 500;
            // 
            // 
            // 
            this._tbmObjectForBlockingAlarm.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjectForBlockingAlarm.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._tbmObjectForBlockingAlarm.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmObjectForBlockingAlarm.ImageTextBox.ContextMenuStrip = this._tbmObjectForBlockingAlarm.ButtonPopupMenu;
            this._tbmObjectForBlockingAlarm.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjectForBlockingAlarm.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmObjectForBlockingAlarm.ImageTextBox.Image")));
            this._tbmObjectForBlockingAlarm.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmObjectForBlockingAlarm.ImageTextBox.Name = "_itbTextBox";
            this._tbmObjectForBlockingAlarm.ImageTextBox.NoTextNoImage = true;
            this._tbmObjectForBlockingAlarm.ImageTextBox.ReadOnly = false;
            this._tbmObjectForBlockingAlarm.ImageTextBox.Size = new System.Drawing.Size(374, 20);
            this._tbmObjectForBlockingAlarm.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmObjectForBlockingAlarm.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjectForBlockingAlarm.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmObjectForBlockingAlarm.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmObjectForBlockingAlarm.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjectForBlockingAlarm.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmObjectForBlockingAlarm.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmObjectForBlockingAlarm.ImageTextBox.TextBox.Size = new System.Drawing.Size(372, 13);
            this._tbmObjectForBlockingAlarm.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmObjectForBlockingAlarm.ImageTextBox.UseImage = true;
            this._tbmObjectForBlockingAlarm.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmObjectForBlockingAlarm_ImageTextBox_DoubleClick);
            this._tbmObjectForBlockingAlarm.Location = new System.Drawing.Point(3, 19);
            this._tbmObjectForBlockingAlarm.Margin = new System.Windows.Forms.Padding(0);
            this._tbmObjectForBlockingAlarm.MaximumSize = new System.Drawing.Size(1200, 20);
            this._tbmObjectForBlockingAlarm.MinimumSize = new System.Drawing.Size(21, 20);
            this._tbmObjectForBlockingAlarm.Name = "_tbmObjectForBlockingAlarm";
            this._tbmObjectForBlockingAlarm.Size = new System.Drawing.Size(394, 20);
            this._tbmObjectForBlockingAlarm.TabIndex = 7;
            this._tbmObjectForBlockingAlarm.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmObjectForBlockingAlarm.TextImage")));
            this._tbmObjectForBlockingAlarm.DragOver += new System.Windows.Forms.DragEventHandler(this.DoDragOver);
            this._tbmObjectForBlockingAlarm.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmObjectForBlockingAlarm_DragDrop);
            this._tbmObjectForBlockingAlarm.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmObjectForBlockingAlarm_ButtonPopupMenuItemClick);
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
            // _lObjectForBlockingAlarm
            // 
            this._lObjectForBlockingAlarm.Location = new System.Drawing.Point(3, 3);
            this._lObjectForBlockingAlarm.Margin = new System.Windows.Forms.Padding(0);
            this._lObjectForBlockingAlarm.Name = "_lObjectForBlockingAlarm";
            this._lObjectForBlockingAlarm.Size = new System.Drawing.Size(394, 13);
            this._lObjectForBlockingAlarm.TabIndex = 6;
            this._lObjectForBlockingAlarm.Text = "On/Off object for blocking alarm";
            this._lObjectForBlockingAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _pEventlogDuringBlockedAlarm
            // 
            this._pEventlogDuringBlockedAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pEventlogDuringBlockedAlarm.BackColor = System.Drawing.SystemColors.ControlLight;
            this._pEventlogDuringBlockedAlarm.Controls.Add(this._chbEventlogDuringBlockedAlarm);
            this._pEventlogDuringBlockedAlarm.Location = new System.Drawing.Point(546, 0);
            this._pEventlogDuringBlockedAlarm.Margin = new System.Windows.Forms.Padding(0);
            this._pEventlogDuringBlockedAlarm.Name = "_pEventlogDuringBlockedAlarm";
            this._pEventlogDuringBlockedAlarm.Size = new System.Drawing.Size(254, 27);
            this._pEventlogDuringBlockedAlarm.TabIndex = 5;
            this._pEventlogDuringBlockedAlarm.SizeChanged += new System.EventHandler(this._pEventlogDuringBlockedAlarm_SizeChanged);
            // 
            // _chbEventlogDuringBlockedAlarm
            // 
            this._chbEventlogDuringBlockedAlarm.AutoSize = true;
            this._chbEventlogDuringBlockedAlarm.Location = new System.Drawing.Point(0, 3);
            this._chbEventlogDuringBlockedAlarm.Margin = new System.Windows.Forms.Padding(0);
            this._chbEventlogDuringBlockedAlarm.MaximumSize = new System.Drawing.Size(248, 21);
            this._chbEventlogDuringBlockedAlarm.MinimumSize = new System.Drawing.Size(0, 21);
            this._chbEventlogDuringBlockedAlarm.Name = "_chbEventlogDuringBlockedAlarm";
            this._chbEventlogDuringBlockedAlarm.Size = new System.Drawing.Size(165, 21);
            this._chbEventlogDuringBlockedAlarm.TabIndex = 8;
            this._chbEventlogDuringBlockedAlarm.Text = "Record in eventlog if blocked";
            this._chbEventlogDuringBlockedAlarm.UseVisualStyleBackColor = true;
            this._chbEventlogDuringBlockedAlarm.CheckStateChanged += new System.EventHandler(this.DoEditTextChanger);
            // 
            // _pPresentationGroup
            // 
            this._pPresentationGroup.BackColor = System.Drawing.SystemColors.Control;
            this._pPresentationGroup.Controls.Add(this._tbmPresentationGroup);
            this._pPresentationGroup.Controls.Add(this._lPresentationGroup);
            this._pPresentationGroup.Location = new System.Drawing.Point(0, 27);
            this._pPresentationGroup.Margin = new System.Windows.Forms.Padding(0);
            this._pPresentationGroup.Name = "_pPresentationGroup";
            this._pPresentationGroup.Size = new System.Drawing.Size(400, 42);
            this._pPresentationGroup.TabIndex = 6;
            // 
            // _tbmPresentationGroup
            // 
            this._tbmPresentationGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPresentationGroup.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmPresentationGroup.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPresentationGroup.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmPresentationGroup.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmPresentationGroup.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroup.Button.Image")));
            this._tbmPresentationGroup.Button.Location = new System.Drawing.Point(374, 0);
            this._tbmPresentationGroup.Button.Name = "_bMenu";
            this._tbmPresentationGroup.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmPresentationGroup.Button.TabIndex = 3;
            this._tbmPresentationGroup.Button.UseVisualStyleBackColor = false;
            this._tbmPresentationGroup.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmPresentationGroup.ButtonDefaultBehaviour = true;
            this._tbmPresentationGroup.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmPresentationGroup.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroup.ButtonImage")));
            // 
            // 
            // 
            this._tbmPresentationGroup.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2,
            this._tsiCreate2});
            this._tbmPresentationGroup.ButtonPopupMenu.Name = "";
            this._tbmPresentationGroup.ButtonPopupMenu.Size = new System.Drawing.Size(118, 70);
            this._tbmPresentationGroup.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmPresentationGroup.ButtonShowImage = true;
            this._tbmPresentationGroup.ButtonSizeHeight = 20;
            this._tbmPresentationGroup.ButtonSizeWidth = 20;
            this._tbmPresentationGroup.ButtonText = "";
            this._tbmPresentationGroup.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPresentationGroup.HoverTime = 500;
            // 
            // 
            // 
            this._tbmPresentationGroup.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPresentationGroup.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._tbmPresentationGroup.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmPresentationGroup.ImageTextBox.ContextMenuStrip = this._tbmPresentationGroup.ButtonPopupMenu;
            this._tbmPresentationGroup.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPresentationGroup.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroup.ImageTextBox.Image")));
            this._tbmPresentationGroup.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmPresentationGroup.ImageTextBox.Name = "_itbTextBox";
            this._tbmPresentationGroup.ImageTextBox.NoTextNoImage = true;
            this._tbmPresentationGroup.ImageTextBox.ReadOnly = false;
            this._tbmPresentationGroup.ImageTextBox.Size = new System.Drawing.Size(374, 20);
            this._tbmPresentationGroup.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmPresentationGroup.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPresentationGroup.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmPresentationGroup.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmPresentationGroup.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPresentationGroup.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmPresentationGroup.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmPresentationGroup.ImageTextBox.TextBox.Size = new System.Drawing.Size(372, 13);
            this._tbmPresentationGroup.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmPresentationGroup.ImageTextBox.UseImage = true;
            this._tbmPresentationGroup.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmPresentationGroup_ImageTextBox_DoubleClick);
            this._tbmPresentationGroup.Location = new System.Drawing.Point(3, 19);
            this._tbmPresentationGroup.Margin = new System.Windows.Forms.Padding(0);
            this._tbmPresentationGroup.MaximumSize = new System.Drawing.Size(1200, 20);
            this._tbmPresentationGroup.MinimumSize = new System.Drawing.Size(21, 20);
            this._tbmPresentationGroup.Name = "_tbmPresentationGroup";
            this._tbmPresentationGroup.Size = new System.Drawing.Size(394, 20);
            this._tbmPresentationGroup.TabIndex = 9;
            this._tbmPresentationGroup.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroup.TextImage")));
            this._tbmPresentationGroup.DragOver += new System.Windows.Forms.DragEventHandler(this.DoDragOver);
            this._tbmPresentationGroup.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmPresentationGroup_DragDrop);
            this._tbmPresentationGroup.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmPresentationGroup_ButtonPopupMenuItemClick);
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
            // _tsiCreate2
            // 
            this._tsiCreate2.Name = "_tsiCreate2";
            this._tsiCreate2.Size = new System.Drawing.Size(117, 22);
            this._tsiCreate2.Text = "Create";
            // 
            // _lPresentationGroup
            // 
            this._lPresentationGroup.Location = new System.Drawing.Point(3, 3);
            this._lPresentationGroup.Margin = new System.Windows.Forms.Padding(0);
            this._lPresentationGroup.Name = "_lPresentationGroup";
            this._lPresentationGroup.Size = new System.Drawing.Size(394, 13);
            this._lPresentationGroup.TabIndex = 8;
            this._lPresentationGroup.Text = "Presentation group";
            this._lPresentationGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ControlAlarmTypeSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this._pObjectForBlockingAlarm);
            this.Controls.Add(this._pBlockAlarm);
            this.Controls.Add(this._pEventlogDuringBlockedAlarm);
            this.Controls.Add(this._pAlarmEnabled);
            this.Controls.Add(this._pPresentationGroup);
            this.Name = "ControlAlarmTypeSettings";
            this.Size = new System.Drawing.Size(800, 69);
            this._pAlarmEnabled.ResumeLayout(false);
            this._pAlarmEnabled.PerformLayout();
            this._pBlockAlarm.ResumeLayout(false);
            this._pBlockAlarm.PerformLayout();
            this._pObjectForBlockingAlarm.ResumeLayout(false);
            this._pEventlogDuringBlockedAlarm.ResumeLayout(false);
            this._pEventlogDuringBlockedAlarm.PerformLayout();
            this._pPresentationGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pAlarmEnabled;
        private System.Windows.Forms.CheckBox _chbAlarmEnabled;
        private System.Windows.Forms.Panel _pBlockAlarm;
        private System.Windows.Forms.CheckBox _chbBlockAlarm;
        private System.Windows.Forms.Panel _pObjectForBlockingAlarm;
        private System.Windows.Forms.Label _lObjectForBlockingAlarm;
        private Contal.IwQuick.UI.TextBoxMenu _tbmObjectForBlockingAlarm;
        private System.Windows.Forms.Panel _pEventlogDuringBlockedAlarm;
        private System.Windows.Forms.CheckBox _chbEventlogDuringBlockedAlarm;
        private System.Windows.Forms.Panel _pPresentationGroup;
        private Contal.IwQuick.UI.TextBoxMenu _tbmPresentationGroup;
        private System.Windows.Forms.Label _lPresentationGroup;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove1;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate2;
    }
}
