namespace Contal.Cgp.NCAS.Client
{
    partial class APBZConflictDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(APBZConflictDialog));
            this._lbCardReadersNormalAccess = new System.Windows.Forms.ListBox();
            this._tbMessageNormalAccess = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._rbRemoveFromOtherZones = new System.Windows.Forms.RadioButton();
            this._rbRemoveFromThisZone = new System.Windows.Forms.RadioButton();
            this._rbToggleAccessMode = new System.Windows.Forms.RadioButton();
            this._bOK = new System.Windows.Forms.Button();
            this._lAntiPassBackZonesNormalAccess = new System.Windows.Forms.Label();
            this._lbConflictingZonesNormalAccess = new System.Windows.Forms.ListBox();
            this._bCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._lbConflictingZonesAccessPermitted = new System.Windows.Forms.ListBox();
            this._lAntiPassBackZonesAccessPermitted = new System.Windows.Forms.Label();
            this._lbCardReadersAccessPermitted = new System.Windows.Forms.ListBox();
            this._tbMessageAccessPermitted = new System.Windows.Forms.TextBox();
            this._tbMessageAccessInterrupted = new System.Windows.Forms.TextBox();
            this._lbCardReadersAccessInterrupted = new System.Windows.Forms.ListBox();
            this._lAntiPassBackZonesAccessInterrupted = new System.Windows.Forms.Label();
            this._lbConflictingZonesAccessInterrupted = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lbCardReadersNormalAccess
            // 
            this._lbCardReadersNormalAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbCardReadersNormalAccess.FormattingEnabled = true;
            this._lbCardReadersNormalAccess.Location = new System.Drawing.Point(2, 58);
            this._lbCardReadersNormalAccess.Margin = new System.Windows.Forms.Padding(2);
            this._lbCardReadersNormalAccess.Name = "_lbCardReadersNormalAccess";
            this._lbCardReadersNormalAccess.Size = new System.Drawing.Size(268, 108);
            this._lbCardReadersNormalAccess.TabIndex = 0;
            // 
            // _tbMessageNormalAccess
            // 
            this._tbMessageNormalAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbMessageNormalAccess.Location = new System.Drawing.Point(2, 2);
            this._tbMessageNormalAccess.Margin = new System.Windows.Forms.Padding(2);
            this._tbMessageNormalAccess.Multiline = true;
            this._tbMessageNormalAccess.Name = "_tbMessageNormalAccess";
            this._tbMessageNormalAccess.ReadOnly = true;
            this._tbMessageNormalAccess.Size = new System.Drawing.Size(268, 52);
            this._tbMessageNormalAccess.TabIndex = 2;
            this._tbMessageNormalAccess.Text = "The following card readers have already been assigned as entry to other anti-pass" +
                "back zones with access mode \"Normal\".";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this._rbRemoveFromOtherZones);
            this.groupBox1.Controls.Add(this._rbRemoveFromThisZone);
            this.groupBox1.Controls.Add(this._rbToggleAccessMode);
            this.groupBox1.Location = new System.Drawing.Point(9, 318);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(818, 85);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resolution";
            // 
            // _rbRemoveFromOtherZones
            // 
            this._rbRemoveFromOtherZones.AutoSize = true;
            this._rbRemoveFromOtherZones.Location = new System.Drawing.Point(4, 61);
            this._rbRemoveFromOtherZones.Margin = new System.Windows.Forms.Padding(2);
            this._rbRemoveFromOtherZones.Name = "_rbRemoveFromOtherZones";
            this._rbRemoveFromOtherZones.Size = new System.Drawing.Size(277, 17);
            this._rbRemoveFromOtherZones.TabIndex = 2;
            this._rbRemoveFromOtherZones.TabStop = true;
            this._rbRemoveFromOtherZones.Text = "Remove conflicting card readers from the other zones";
            this._rbRemoveFromOtherZones.UseVisualStyleBackColor = true;
            // 
            // _rbRemoveFromThisZone
            // 
            this._rbRemoveFromThisZone.AutoSize = true;
            this._rbRemoveFromThisZone.Location = new System.Drawing.Point(4, 39);
            this._rbRemoveFromThisZone.Margin = new System.Windows.Forms.Padding(2);
            this._rbRemoveFromThisZone.Name = "_rbRemoveFromThisZone";
            this._rbRemoveFromThisZone.Size = new System.Drawing.Size(350, 17);
            this._rbRemoveFromThisZone.TabIndex = 1;
            this._rbRemoveFromThisZone.TabStop = true;
            this._rbRemoveFromThisZone.Text = "Remove conflicting card readers from the current anti-passback zone";
            this._rbRemoveFromThisZone.UseVisualStyleBackColor = true;
            // 
            // _rbToggleAccessMode
            // 
            this._rbToggleAccessMode.AutoSize = true;
            this._rbToggleAccessMode.Checked = true;
            this._rbToggleAccessMode.Location = new System.Drawing.Point(4, 17);
            this._rbToggleAccessMode.Margin = new System.Windows.Forms.Padding(2);
            this._rbToggleAccessMode.Name = "_rbToggleAccessMode";
            this._rbToggleAccessMode.Size = new System.Drawing.Size(412, 17);
            this._rbToggleAccessMode.TabIndex = 0;
            this._rbToggleAccessMode.TabStop = true;
            this._rbToggleAccessMode.Text = "Toggle access mode for conflicting card readers in the current anti-passback zone" +
                "";
            this._rbToggleAccessMode.UseVisualStyleBackColor = true;
            // 
            // _bOK
            // 
            this._bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bOK.Location = new System.Drawing.Point(682, 416);
            this._bOK.Margin = new System.Windows.Forms.Padding(2);
            this._bOK.Name = "_bOK";
            this._bOK.Size = new System.Drawing.Size(70, 24);
            this._bOK.TabIndex = 4;
            this._bOK.Text = "OK";
            this._bOK.UseVisualStyleBackColor = true;
            this._bOK.Click += new System.EventHandler(this._bOK_Click);
            // 
            // _lAntiPassBackZonesNormalAccess
            // 
            this._lAntiPassBackZonesNormalAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lAntiPassBackZonesNormalAccess.AutoSize = true;
            this._lAntiPassBackZonesNormalAccess.Location = new System.Drawing.Point(2, 178);
            this._lAntiPassBackZonesNormalAccess.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lAntiPassBackZonesNormalAccess.Name = "_lAntiPassBackZonesNormalAccess";
            this._lAntiPassBackZonesNormalAccess.Size = new System.Drawing.Size(233, 13);
            this._lAntiPassBackZonesNormalAccess.TabIndex = 5;
            this._lAntiPassBackZonesNormalAccess.Text = "Conflicting anti-passback zones (normal access)";
            // 
            // _lbConflictingZonesNormalAccess
            // 
            this._lbConflictingZonesNormalAccess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbConflictingZonesNormalAccess.FormattingEnabled = true;
            this._lbConflictingZonesNormalAccess.Location = new System.Drawing.Point(2, 193);
            this._lbConflictingZonesNormalAccess.Margin = new System.Windows.Forms.Padding(2);
            this._lbConflictingZonesNormalAccess.Name = "_lbConflictingZonesNormalAccess";
            this._lbConflictingZonesNormalAccess.Size = new System.Drawing.Size(268, 108);
            this._lbConflictingZonesNormalAccess.TabIndex = 6;
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(757, 416);
            this._bCancel.Margin = new System.Windows.Forms.Padding(2);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(70, 24);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this._lbConflictingZonesAccessPermitted, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this._lAntiPassBackZonesAccessPermitted, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._lbCardReadersAccessPermitted, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._tbMessageNormalAccess, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._lbConflictingZonesNormalAccess, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._lAntiPassBackZonesNormalAccess, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._lbCardReadersNormalAccess, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._tbMessageAccessPermitted, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._tbMessageAccessInterrupted, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this._lbCardReadersAccessInterrupted, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this._lAntiPassBackZonesAccessInterrupted, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this._lbConflictingZonesAccessInterrupted, 2, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 10);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(818, 303);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // _lbConflictingZonesAccessPermitted
            // 
            this._lbConflictingZonesAccessPermitted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbConflictingZonesAccessPermitted.FormattingEnabled = true;
            this._lbConflictingZonesAccessPermitted.Location = new System.Drawing.Point(274, 193);
            this._lbConflictingZonesAccessPermitted.Margin = new System.Windows.Forms.Padding(2);
            this._lbConflictingZonesAccessPermitted.Name = "_lbConflictingZonesAccessPermitted";
            this._lbConflictingZonesAccessPermitted.Size = new System.Drawing.Size(268, 108);
            this._lbConflictingZonesAccessPermitted.TabIndex = 9;
            // 
            // _lAntiPassBackZonesAccessPermitted
            // 
            this._lAntiPassBackZonesAccessPermitted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lAntiPassBackZonesAccessPermitted.AutoSize = true;
            this._lAntiPassBackZonesAccessPermitted.Location = new System.Drawing.Point(274, 178);
            this._lAntiPassBackZonesAccessPermitted.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lAntiPassBackZonesAccessPermitted.Name = "_lAntiPassBackZonesAccessPermitted";
            this._lAntiPassBackZonesAccessPermitted.Size = new System.Drawing.Size(245, 13);
            this._lAntiPassBackZonesAccessPermitted.TabIndex = 9;
            this._lAntiPassBackZonesAccessPermitted.Text = "Conflicting anti-passback zones (access permitted)";
            // 
            // _lbCardReadersAccessPermitted
            // 
            this._lbCardReadersAccessPermitted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbCardReadersAccessPermitted.FormattingEnabled = true;
            this._lbCardReadersAccessPermitted.Location = new System.Drawing.Point(274, 58);
            this._lbCardReadersAccessPermitted.Margin = new System.Windows.Forms.Padding(2);
            this._lbCardReadersAccessPermitted.Name = "_lbCardReadersAccessPermitted";
            this._lbCardReadersAccessPermitted.Size = new System.Drawing.Size(268, 108);
            this._lbCardReadersAccessPermitted.TabIndex = 9;
            // 
            // _tbMessageAccessPermitted
            // 
            this._tbMessageAccessPermitted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbMessageAccessPermitted.Location = new System.Drawing.Point(274, 2);
            this._tbMessageAccessPermitted.Margin = new System.Windows.Forms.Padding(2);
            this._tbMessageAccessPermitted.Multiline = true;
            this._tbMessageAccessPermitted.Name = "_tbMessageAccessPermitted";
            this._tbMessageAccessPermitted.ReadOnly = true;
            this._tbMessageAccessPermitted.Size = new System.Drawing.Size(268, 52);
            this._tbMessageAccessPermitted.TabIndex = 11;
            this._tbMessageAccessPermitted.Text = "The following card readers have already been assigned as entry to other anti-pass" +
                "back zones with access mode \"Permitted\".";
            // 
            // _tbMessageAccessInterrupted
            // 
            this._tbMessageAccessInterrupted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbMessageAccessInterrupted.Location = new System.Drawing.Point(546, 2);
            this._tbMessageAccessInterrupted.Margin = new System.Windows.Forms.Padding(2);
            this._tbMessageAccessInterrupted.Multiline = true;
            this._tbMessageAccessInterrupted.Name = "_tbMessageAccessInterrupted";
            this._tbMessageAccessInterrupted.ReadOnly = true;
            this._tbMessageAccessInterrupted.Size = new System.Drawing.Size(270, 52);
            this._tbMessageAccessInterrupted.TabIndex = 7;
            this._tbMessageAccessInterrupted.Text = "The following card readers have already been assigned as entry to other anti-pass" +
                "back zones with access mode \"Interrupted\".";
            // 
            // _lbCardReadersAccessInterrupted
            // 
            this._lbCardReadersAccessInterrupted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbCardReadersAccessInterrupted.FormattingEnabled = true;
            this._lbCardReadersAccessInterrupted.Location = new System.Drawing.Point(546, 58);
            this._lbCardReadersAccessInterrupted.Margin = new System.Windows.Forms.Padding(2);
            this._lbCardReadersAccessInterrupted.Name = "_lbCardReadersAccessInterrupted";
            this._lbCardReadersAccessInterrupted.Size = new System.Drawing.Size(270, 108);
            this._lbCardReadersAccessInterrupted.TabIndex = 8;
            // 
            // _lAntiPassBackZonesAccessInterrupted
            // 
            this._lAntiPassBackZonesAccessInterrupted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lAntiPassBackZonesAccessInterrupted.AutoSize = true;
            this._lAntiPassBackZonesAccessInterrupted.Location = new System.Drawing.Point(546, 178);
            this._lAntiPassBackZonesAccessInterrupted.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lAntiPassBackZonesAccessInterrupted.Name = "_lAntiPassBackZonesAccessInterrupted";
            this._lAntiPassBackZonesAccessInterrupted.Size = new System.Drawing.Size(252, 13);
            this._lAntiPassBackZonesAccessInterrupted.TabIndex = 9;
            this._lAntiPassBackZonesAccessInterrupted.Text = "Conflicting anti-passback zones (access interrupted)";
            // 
            // _lbConflictingZonesAccessInterrupted
            // 
            this._lbConflictingZonesAccessInterrupted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbConflictingZonesAccessInterrupted.FormattingEnabled = true;
            this._lbConflictingZonesAccessInterrupted.Location = new System.Drawing.Point(546, 193);
            this._lbConflictingZonesAccessInterrupted.Margin = new System.Windows.Forms.Padding(2);
            this._lbConflictingZonesAccessInterrupted.Name = "_lbConflictingZonesAccessInterrupted";
            this._lbConflictingZonesAccessInterrupted.Size = new System.Drawing.Size(270, 108);
            this._lbConflictingZonesAccessInterrupted.TabIndex = 10;
            // 
            // APBZConflictDialog
            // 
            this.AcceptButton = this._bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(838, 456);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOK);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(602, 495);
            this.Name = "APBZConflictDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "APBZConflictDialog";
            this.Load += new System.EventHandler(this.APBZConflictDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox _lbCardReadersNormalAccess;
        private System.Windows.Forms.TextBox _tbMessageNormalAccess;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton _rbRemoveFromOtherZones;
        private System.Windows.Forms.RadioButton _rbRemoveFromThisZone;
        private System.Windows.Forms.RadioButton _rbToggleAccessMode;
        private System.Windows.Forms.Button _bOK;
        private System.Windows.Forms.Label _lAntiPassBackZonesNormalAccess;
        private System.Windows.Forms.ListBox _lbConflictingZonesNormalAccess;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox _tbMessageAccessInterrupted;
        private System.Windows.Forms.ListBox _lbCardReadersAccessInterrupted;
        private System.Windows.Forms.ListBox _lbConflictingZonesAccessInterrupted;
        private System.Windows.Forms.Label _lAntiPassBackZonesAccessInterrupted;
        private System.Windows.Forms.ListBox _lbConflictingZonesAccessPermitted;
        private System.Windows.Forms.Label _lAntiPassBackZonesAccessPermitted;
        private System.Windows.Forms.ListBox _lbCardReadersAccessPermitted;
        private System.Windows.Forms.TextBox _tbMessageAccessPermitted;

    }
}
