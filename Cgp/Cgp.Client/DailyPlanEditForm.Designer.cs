namespace Contal.Cgp.Client
{
    partial class DailyPlanEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DailyPlanEditForm));
            this._eName = new System.Windows.Forms.TextBox();
            this._tcDailyPlan = new System.Windows.Forms.TabControl();
            this._tpSchedule = new System.Windows.Forms.TabPage();
            this._dmDailyPlan = new Contal.IwQuick.UI.DateMatrix();
            this._gbInfo = new System.Windows.Forms.GroupBox();
            this._lMouseColorRight = new System.Windows.Forms.Label();
            this._lMouseColorLeft = new System.Windows.Forms.Label();
            this._lColorOff = new System.Windows.Forms.Label();
            this._lColorOn = new System.Windows.Forms.Label();
            this._lMouse = new System.Windows.Forms.Label();
            this._lStatusOff = new System.Windows.Forms.Label();
            this._lStatusOn = new System.Windows.Forms.Label();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lName = new System.Windows.Forms.Label();
            this._panelBack = new System.Windows.Forms.Panel();
            this._tcDailyPlan.SuspendLayout();
            this._tpSchedule.SuspendLayout();
            this._gbInfo.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(294, 15);
            this._eName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(498, 22);
            this._eName.TabIndex = 1;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _tcDailyPlan
            // 
            this._tcDailyPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tcDailyPlan.Controls.Add(this._tpSchedule);
            this._tcDailyPlan.Controls.Add(this._tpUserFolders);
            this._tcDailyPlan.Controls.Add(this._tpReferencedBy);
            this._tcDailyPlan.Controls.Add(this._tpDescription);
            this._tcDailyPlan.Location = new System.Drawing.Point(15, 48);
            this._tcDailyPlan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tcDailyPlan.Name = "_tcDailyPlan";
            this._tcDailyPlan.SelectedIndex = 0;
            this._tcDailyPlan.Size = new System.Drawing.Size(981, 596);
            this._tcDailyPlan.TabIndex = 2;
            this._tcDailyPlan.TabStop = false;
            // 
            // _tpSchedule
            // 
            this._tpSchedule.BackColor = System.Drawing.SystemColors.Control;
            this._tpSchedule.Controls.Add(this._dmDailyPlan);
            this._tpSchedule.Controls.Add(this._gbInfo);
            this._tpSchedule.Location = new System.Drawing.Point(4, 25);
            this._tpSchedule.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpSchedule.Name = "_tpSchedule";
            this._tpSchedule.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpSchedule.Size = new System.Drawing.Size(973, 567);
            this._tpSchedule.TabIndex = 0;
            this._tpSchedule.Text = "Schedule";
            // 
            // _dmDailyPlan
            // 
            this._dmDailyPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dmDailyPlan.ButtonAlign = Contal.IwQuick.UI.DateMatrix.ButtonsAlign.Center;
            this._dmDailyPlan.ButtonsFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._dmDailyPlan.ButtonsHeight = 29;
            this._dmDailyPlan.ButtonsWidth = 29;
            this._dmDailyPlan.Coloras = new System.Drawing.Color[] {
        System.Drawing.Color.Red,
        System.Drawing.Color.LightGreen};
            this._dmDailyPlan.ColorLeftClickIndex = 1;
            this._dmDailyPlan.ColorRightClickIndex = 0;
            this._dmDailyPlan.DefaultColorIndex = 0;
            this._dmDailyPlan.Editable = true;
            this._dmDailyPlan.LegendStepX = 10;
            this._dmDailyPlan.LegendStepY = 6;
            this._dmDailyPlan.Location = new System.Drawing.Point(8, 64);
            this._dmDailyPlan.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this._dmDailyPlan.Name = "_dmDailyPlan";
            this._dmDailyPlan.ResizeButtons = true;
            this._dmDailyPlan.SelectionType = Contal.IwQuick.UI.SelectionType.Multiselection;
            this._dmDailyPlan.Size = new System.Drawing.Size(956, 494);
            this._dmDailyPlan.TabIndex = 3;
            this._dmDailyPlan.ViewButtons = true;
            // 
            // _gbInfo
            // 
            this._gbInfo.Controls.Add(this._lMouseColorRight);
            this._gbInfo.Controls.Add(this._lMouseColorLeft);
            this._gbInfo.Controls.Add(this._lColorOff);
            this._gbInfo.Controls.Add(this._lColorOn);
            this._gbInfo.Controls.Add(this._lMouse);
            this._gbInfo.Controls.Add(this._lStatusOff);
            this._gbInfo.Controls.Add(this._lStatusOn);
            this._gbInfo.Location = new System.Drawing.Point(31, 8);
            this._gbInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._gbInfo.Name = "_gbInfo";
            this._gbInfo.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._gbInfo.Size = new System.Drawing.Size(478, 54);
            this._gbInfo.TabIndex = 0;
            this._gbInfo.TabStop = false;
            this._gbInfo.Text = "Daily plan status";
            // 
            // _lMouseColorRight
            // 
            this._lMouseColorRight.BackColor = System.Drawing.Color.Red;
            this._lMouseColorRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lMouseColorRight.Location = new System.Drawing.Point(318, 22);
            this._lMouseColorRight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMouseColorRight.Name = "_lMouseColorRight";
            this._lMouseColorRight.Size = new System.Drawing.Size(18, 18);
            this._lMouseColorRight.TabIndex = 5;
            this._lMouseColorRight.Text = "R";
            this._lMouseColorRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lMouseColorLeft
            // 
            this._lMouseColorLeft.BackColor = System.Drawing.Color.LightGreen;
            this._lMouseColorLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lMouseColorLeft.Location = new System.Drawing.Point(299, 22);
            this._lMouseColorLeft.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMouseColorLeft.Name = "_lMouseColorLeft";
            this._lMouseColorLeft.Size = new System.Drawing.Size(18, 18);
            this._lMouseColorLeft.TabIndex = 4;
            this._lMouseColorLeft.Text = "L";
            this._lMouseColorLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lColorOff
            // 
            this._lColorOff.BackColor = System.Drawing.Color.Red;
            this._lColorOff.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorOff.Location = new System.Drawing.Point(150, 22);
            this._lColorOff.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lColorOff.Name = "_lColorOff";
            this._lColorOff.Size = new System.Drawing.Size(18, 18);
            this._lColorOff.TabIndex = 2;
            // 
            // _lColorOn
            // 
            this._lColorOn.BackColor = System.Drawing.Color.LightGreen;
            this._lColorOn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorOn.Location = new System.Drawing.Point(8, 22);
            this._lColorOn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lColorOn.Name = "_lColorOn";
            this._lColorOn.Size = new System.Drawing.Size(18, 18);
            this._lColorOn.TabIndex = 0;
            // 
            // _lMouse
            // 
            this._lMouse.AutoSize = true;
            this._lMouse.Location = new System.Drawing.Point(339, 22);
            this._lMouse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMouse.Name = "_lMouse";
            this._lMouse.Size = new System.Drawing.Size(48, 16);
            this._lMouse.TabIndex = 6;
            this._lMouse.Text = "Mouse";
            // 
            // _lStatusOff
            // 
            this._lStatusOff.AutoSize = true;
            this._lStatusOff.Location = new System.Drawing.Point(176, 24);
            this._lStatusOff.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lStatusOff.Name = "_lStatusOff";
            this._lStatusOff.Size = new System.Drawing.Size(73, 16);
            this._lStatusOff.TabIndex = 3;
            this._lStatusOff.Text = "Status OFF";
            // 
            // _lStatusOn
            // 
            this._lStatusOn.AutoSize = true;
            this._lStatusOn.Location = new System.Drawing.Point(34, 24);
            this._lStatusOn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lStatusOn.Name = "_lStatusOn";
            this._lStatusOn.Size = new System.Drawing.Size(67, 16);
            this._lStatusOn.TabIndex = 1;
            this._lStatusOn.Text = "Status ON";
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 25);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(973, 567);
            this._tpUserFolders.TabIndex = 3;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(874, 531);
            this._bRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(94, 32);
            this._bRefresh.TabIndex = 1;
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
            this._lbUserFolders.Location = new System.Drawing.Point(2, 2);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(965, 498);
            this._lbUserFolders.TabIndex = 14;
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
            this._tpReferencedBy.Size = new System.Drawing.Size(973, 567);
            this._tpReferencedBy.TabIndex = 2;
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
            this._tpDescription.Size = new System.Drawing.Size(973, 567);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(4, 4);
            this._eDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(965, 559);
            this._eDescription.TabIndex = 1;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(789, 651);
            this._bOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(100, 32);
            this._bOk.TabIndex = 3;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(895, 651);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(100, 32);
            this._bCancel.TabIndex = 4;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(15, 18);
            this._lName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(44, 16);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._tcDailyPlan);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(1010, 706);
            this._panelBack.TabIndex = 0;
            // 
            // DailyPlanEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1010, 706);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DailyPlanEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DailyPlanEditForm";
            this._tcDailyPlan.ResumeLayout(false);
            this._tpSchedule.ResumeLayout(false);
            this._gbInfo.ResumeLayout(false);
            this._gbInfo.PerformLayout();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.TabControl _tcDailyPlan;
        private System.Windows.Forms.TabPage _tpSchedule;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.GroupBox _gbInfo;
        private System.Windows.Forms.Label _lStatusOff;
        private System.Windows.Forms.Label _lStatusOn;
        private System.Windows.Forms.Label _lMouse;
        private System.Windows.Forms.Label _lColorOff;
        private System.Windows.Forms.Label _lColorOn;
        private System.Windows.Forms.Label _lMouseColorRight;
        private System.Windows.Forms.Label _lMouseColorLeft;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private Contal.IwQuick.UI.DateMatrix _dmDailyPlan;
        private System.Windows.Forms.TabPage _tpReferencedBy;

    }
}