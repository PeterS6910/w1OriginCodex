namespace Contal.Cgp.NCAS.Client
{
    partial class NCASAlarmTransmitterEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASAlarmTransmitterEditForm));
            this._tcAlarmTransmitter = new System.Windows.Forms.TabControl();
            this._tpObjectPlacement = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._bApply = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lName = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._eIpAddress = new System.Windows.Forms.TextBox();
            this._lIPAddress = new System.Windows.Forms.Label();
            this._eOnlineState = new System.Windows.Forms.TextBox();
            this._lOnlineState = new System.Windows.Forms.Label();
            this._tcAlarmTransmitter.SuspendLayout();
            this._tpObjectPlacement.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tcAlarmTransmitter
            // 
            this._tcAlarmTransmitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcAlarmTransmitter.Controls.Add(this._tpObjectPlacement);
            this._tcAlarmTransmitter.Controls.Add(this._tpReferencedBy);
            this._tcAlarmTransmitter.Controls.Add(this._tpDescription);
            this._tcAlarmTransmitter.Location = new System.Drawing.Point(12, 90);
            this._tcAlarmTransmitter.Name = "_tcAlarmTransmitter";
            this._tcAlarmTransmitter.SelectedIndex = 0;
            this._tcAlarmTransmitter.Size = new System.Drawing.Size(510, 280);
            this._tcAlarmTransmitter.TabIndex = 6;
            // 
            // _tpObjectPlacement
            // 
            this._tpObjectPlacement.BackColor = System.Drawing.SystemColors.Control;
            this._tpObjectPlacement.Controls.Add(this._bRefresh);
            this._tpObjectPlacement.Controls.Add(this._lbUserFolders);
            this._tpObjectPlacement.Location = new System.Drawing.Point(4, 22);
            this._tpObjectPlacement.Name = "_tpObjectPlacement";
            this._tpObjectPlacement.Size = new System.Drawing.Size(502, 254);
            this._tpObjectPlacement.TabIndex = 2;
            this._tpObjectPlacement.Text = "Object placement";
            this._tpObjectPlacement.Enter += new System.EventHandler(this._tpObjectPlacement_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(424, 228);
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
            this._lbUserFolders.Size = new System.Drawing.Size(498, 199);
            this._lbUserFolders.TabIndex = 32;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Size = new System.Drawing.Size(502, 204);
            this._tpReferencedBy.TabIndex = 3;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Size = new System.Drawing.Size(502, 204);
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
            this._eDescription.Size = new System.Drawing.Size(502, 204);
            this._eDescription.TabIndex = 8;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(285, 376);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 9;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(366, 376);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 8;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(447, 376);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 15);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 11;
            this._lName.Text = "Name";
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(102, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(420, 20);
            this._eName.TabIndex = 10;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _eIpAddress
            // 
            this._eIpAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eIpAddress.Location = new System.Drawing.Point(102, 38);
            this._eIpAddress.Name = "_eIpAddress";
            this._eIpAddress.Size = new System.Drawing.Size(420, 20);
            this._eIpAddress.TabIndex = 12;
            this._eIpAddress.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lIPAddress
            // 
            this._lIPAddress.AutoSize = true;
            this._lIPAddress.Location = new System.Drawing.Point(12, 41);
            this._lIPAddress.Name = "_lIPAddress";
            this._lIPAddress.Size = new System.Drawing.Size(57, 13);
            this._lIPAddress.TabIndex = 13;
            this._lIPAddress.Text = "IP address";
            // 
            // _eOnlineState
            // 
            this._eOnlineState.BackColor = System.Drawing.SystemColors.Window;
            this._eOnlineState.Location = new System.Drawing.Point(102, 64);
            this._eOnlineState.Name = "_eOnlineState";
            this._eOnlineState.ReadOnly = true;
            this._eOnlineState.Size = new System.Drawing.Size(124, 20);
            this._eOnlineState.TabIndex = 14;
            // 
            // _lOnlineState
            // 
            this._lOnlineState.AutoSize = true;
            this._lOnlineState.Location = new System.Drawing.Point(12, 67);
            this._lOnlineState.Name = "_lOnlineState";
            this._lOnlineState.Size = new System.Drawing.Size(32, 13);
            this._lOnlineState.TabIndex = 15;
            this._lOnlineState.Text = "State";
            // 
            // NCASAlarmTransmitterEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(534, 411);
            this.Controls.Add(this._lOnlineState);
            this.Controls.Add(this._eOnlineState);
            this.Controls.Add(this._lIPAddress);
            this.Controls.Add(this._eIpAddress);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._bApply);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._tcAlarmTransmitter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(550, 450);
            this.Name = "NCASAlarmTransmitterEditForm";
            this.Text = "NCASAlarmTransmitterEditForm";
            this._tcAlarmTransmitter.ResumeLayout(false);
            this._tpObjectPlacement.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl _tcAlarmTransmitter;
        private System.Windows.Forms.TabPage _tpObjectPlacement;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.TextBox _eIpAddress;
        private System.Windows.Forms.Label _lIPAddress;
        private System.Windows.Forms.TextBox _eOnlineState;
        private System.Windows.Forms.Label _lOnlineState;
    }
}
