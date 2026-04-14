namespace Contal.Cgp.Client
{
    partial class PresentationFormatterEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PresentationFormatterEditForm));
            this._eFormatMsg = new System.Windows.Forms.TextBox();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lFormatterName = new System.Windows.Forms.Label();
            this._lForamtterMsg = new System.Windows.Forms.Label();
            this._eFormatterName = new System.Windows.Forms.TextBox();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._panelBack = new System.Windows.Forms.Panel();
            this._tcFormaterSettings = new System.Windows.Forms.TabControl();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._panelBack.SuspendLayout();
            this._tcFormaterSettings.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // _eFormatMsg
            // 
            this._eFormatMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eFormatMsg.Location = new System.Drawing.Point(155, 48);
            this._eFormatMsg.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eFormatMsg.Name = "_eFormatMsg";
            this._eFormatMsg.Size = new System.Drawing.Size(300, 22);
            this._eFormatMsg.TabIndex = 1;
            this._eFormatMsg.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(362, 368);
            this._bOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(94, 32);
            this._bOk.TabIndex = 6;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(465, 368);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 32);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lFormatterName
            // 
            this._lFormatterName.AutoSize = true;
            this._lFormatterName.Location = new System.Drawing.Point(15, 19);
            this._lFormatterName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFormatterName.Name = "_lFormatterName";
            this._lFormatterName.Size = new System.Drawing.Size(44, 16);
            this._lFormatterName.TabIndex = 0;
            this._lFormatterName.Text = "Name";
            // 
            // _lForamtterMsg
            // 
            this._lForamtterMsg.AutoSize = true;
            this._lForamtterMsg.Location = new System.Drawing.Point(15, 51);
            this._lForamtterMsg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lForamtterMsg.Name = "_lForamtterMsg";
            this._lForamtterMsg.Size = new System.Drawing.Size(64, 16);
            this._lForamtterMsg.TabIndex = 2;
            this._lForamtterMsg.Text = "Message";
            // 
            // _eFormatterName
            // 
            this._eFormatterName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eFormatterName.Location = new System.Drawing.Point(155, 15);
            this._eFormatterName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eFormatterName.Name = "_eFormatterName";
            this._eFormatterName.Size = new System.Drawing.Size(300, 22);
            this._eFormatterName.TabIndex = 0;
            this._eFormatterName.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(4, 4);
            this._eDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(528, 243);
            this._eDescription.TabIndex = 2;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._tcFormaterSettings);
            this._panelBack.Controls.Add(this._eFormatterName);
            this._panelBack.Controls.Add(this._eFormatMsg);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._lForamtterMsg);
            this._panelBack.Controls.Add(this._lFormatterName);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(574, 411);
            this._panelBack.TabIndex = 0;
            // 
            // _tcFormaterSettings
            // 
            this._tcFormaterSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tcFormaterSettings.Controls.Add(this._tpUserFolders);
            this._tcFormaterSettings.Controls.Add(this._tpReferencedBy);
            this._tcFormaterSettings.Controls.Add(this._tpDescription);
            this._tcFormaterSettings.Location = new System.Drawing.Point(15, 80);
            this._tcFormaterSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tcFormaterSettings.Name = "_tcFormaterSettings";
            this._tcFormaterSettings.SelectedIndex = 0;
            this._tcFormaterSettings.Size = new System.Drawing.Size(544, 280);
            this._tcFormaterSettings.TabIndex = 2;
            this._tcFormaterSettings.TabStop = false;
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 25);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(536, 251);
            this._tpUserFolders.TabIndex = 2;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(440, 215);
            this._bRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(94, 32);
            this._bRefresh.TabIndex = 2;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _lbUserFolders
            // 
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
            this._lbUserFolders.Size = new System.Drawing.Size(533, 199);
            this._lbUserFolders.TabIndex = 2;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 25);
            this._tpReferencedBy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Size = new System.Drawing.Size(536, 251);
            this._tpReferencedBy.TabIndex = 1;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 25);
            this._tpDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDescription.Size = new System.Drawing.Size(536, 251);
            this._tpDescription.TabIndex = 0;
            this._tpDescription.Text = "Description";
            // 
            // PresentationFormatterEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(574, 411);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(527, 271);
            this.Name = "PresentationFormatterEditForm";
            this.Text = "Presentation formatter";
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this._tcFormaterSettings.ResumeLayout(false);
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox _eFormatMsg;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lFormatterName;
        private System.Windows.Forms.Label _lForamtterMsg;
        private System.Windows.Forms.TextBox _eFormatterName;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.TabControl _tcFormaterSettings;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
    }
}