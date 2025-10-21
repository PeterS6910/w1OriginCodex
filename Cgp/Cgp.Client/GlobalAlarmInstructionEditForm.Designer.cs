namespace Contal.Cgp.Client
{
    partial class GlobalAlarmInstructionEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalAlarmInstructionEditForm));
            this._bOk = new System.Windows.Forms.Button();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tcGlobalAlarmInstruction = new System.Windows.Forms.TabControl();
            this._tpInstructions = new System.Windows.Forms.TabPage();
            this._eInstructions = new System.Windows.Forms.TextBox();
            this._panelBack = new System.Windows.Forms.Panel();
            this._bCancel = new System.Windows.Forms.Button();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._tpUserFolders.SuspendLayout();
            this._tcGlobalAlarmInstruction.SuspendLayout();
            this._tpInstructions.SuspendLayout();
            this._panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(541, 574);
            this._bOk.Margin = new System.Windows.Forms.Padding(2);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(94, 32);
            this._bOk.TabIndex = 5;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this.OkClick);
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 25);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(713, 493);
            this._tpUserFolders.TabIndex = 3;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(614, 458);
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
            this._lbUserFolders.Size = new System.Drawing.Size(708, 433);
            this._lbUserFolders.TabIndex = 10;
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
            this._tpReferencedBy.Size = new System.Drawing.Size(713, 493);
            this._tpReferencedBy.TabIndex = 2;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tcGlobalAlarmInstruction
            // 
            this._tcGlobalAlarmInstruction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tcGlobalAlarmInstruction.Controls.Add(this._tpInstructions);
            this._tcGlobalAlarmInstruction.Controls.Add(this._tpUserFolders);
            this._tcGlobalAlarmInstruction.Controls.Add(this._tpReferencedBy);
            this._tcGlobalAlarmInstruction.Location = new System.Drawing.Point(15, 45);
            this._tcGlobalAlarmInstruction.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tcGlobalAlarmInstruction.Name = "_tcGlobalAlarmInstruction";
            this._tcGlobalAlarmInstruction.SelectedIndex = 0;
            this._tcGlobalAlarmInstruction.Size = new System.Drawing.Size(721, 522);
            this._tcGlobalAlarmInstruction.TabIndex = 4;
            this._tcGlobalAlarmInstruction.TabStop = false;
            // 
            // _tpInstructions
            // 
            this._tpInstructions.BackColor = System.Drawing.SystemColors.Control;
            this._tpInstructions.Controls.Add(this._eInstructions);
            this._tpInstructions.Location = new System.Drawing.Point(4, 25);
            this._tpInstructions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpInstructions.Name = "_tpInstructions";
            this._tpInstructions.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpInstructions.Size = new System.Drawing.Size(713, 493);
            this._tpInstructions.TabIndex = 0;
            this._tpInstructions.Text = "Instructions";
            // 
            // _eInstructions
            // 
            this._eInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eInstructions.Location = new System.Drawing.Point(4, 4);
            this._eInstructions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eInstructions.Multiline = true;
            this._eInstructions.Name = "_eInstructions";
            this._eInstructions.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eInstructions.Size = new System.Drawing.Size(705, 485);
            this._eInstructions.TabIndex = 3;
            this._eInstructions.TabStop = false;
            this._eInstructions.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._tcGlobalAlarmInstruction);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(748, 616);
            this._panelBack.TabIndex = 1;
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(640, 574);
            this._bCancel.Margin = new System.Windows.Forms.Padding(2);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 32);
            this._bCancel.TabIndex = 6;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(180, 12);
            this._eName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(463, 22);
            this._eName.TabIndex = 3;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(11, 16);
            this._lName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(44, 16);
            this._lName.TabIndex = 2;
            this._lName.Text = "Name";
            // 
            // GlobalAlarmInstructionEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(748, 616);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GlobalAlarmInstructionEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GlabalAlarmInstructionEditForm";
            this._tpUserFolders.ResumeLayout(false);
            this._tcGlobalAlarmInstruction.ResumeLayout(false);
            this._tpInstructions.ResumeLayout(false);
            this._tpInstructions.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabControl _tcGlobalAlarmInstruction;
        private System.Windows.Forms.TabPage _tpInstructions;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eInstructions;
    }
}