namespace Contal.IwQuick.UI
{
    partial class DateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateForm));
            this._mcDate = new System.Windows.Forms.MonthCalendar();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._gbSelectTime = new System.Windows.Forms.GroupBox();
            this._dtpCustom = new System.Windows.Forms.DateTimePicker();
            this._rbCustom = new System.Windows.Forms.RadioButton();
            this._dtpDelta = new System.Windows.Forms.DateTimePicker();
            this._rbEndOfDay = new System.Windows.Forms.RadioButton();
            this._rbDelta = new System.Windows.Forms.RadioButton();
            this._rbStartOfDay = new System.Windows.Forms.RadioButton();
            this._gbSelectTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mcDate
            // 
            this._mcDate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._mcDate.Location = new System.Drawing.Point(3, 16);
            this._mcDate.Margin = new System.Windows.Forms.Padding(4);
            this._mcDate.Name = "_mcDate";
            this._mcDate.TabIndex = 0;
            this._mcDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this._mcExpirationDate_MouseDown);
            // 
            // _bOk
            // 
            this._bOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._bOk.Location = new System.Drawing.Point(27, 188);
            this._bOk.Margin = new System.Windows.Forms.Padding(2);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(71, 25);
            this._bOk.TabIndex = 1;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._bCancel.Location = new System.Drawing.Point(174, 188);
            this._bCancel.Margin = new System.Windows.Forms.Padding(2);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(71, 25);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _gbSelectTime
            // 
            this._gbSelectTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._gbSelectTime.Controls.Add(this._dtpCustom);
            this._gbSelectTime.Controls.Add(this._rbCustom);
            this._gbSelectTime.Controls.Add(this._dtpDelta);
            this._gbSelectTime.Controls.Add(this._rbEndOfDay);
            this._gbSelectTime.Controls.Add(this._rbDelta);
            this._gbSelectTime.Controls.Add(this._rbStartOfDay);
            this._gbSelectTime.Location = new System.Drawing.Point(18, 16);
            this._gbSelectTime.Margin = new System.Windows.Forms.Padding(2);
            this._gbSelectTime.Name = "_gbSelectTime";
            this._gbSelectTime.Padding = new System.Windows.Forms.Padding(2);
            this._gbSelectTime.Size = new System.Drawing.Size(227, 162);
            this._gbSelectTime.TabIndex = 3;
            this._gbSelectTime.TabStop = false;
            this._gbSelectTime.Text = "Select time of day";
            // 
            // _dtpCustom
            // 
            this._dtpCustom.CustomFormat = "HH:mm:ss";
            this._dtpCustom.Enabled = false;
            this._dtpCustom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._dtpCustom.Location = new System.Drawing.Point(103, 62);
            this._dtpCustom.Margin = new System.Windows.Forms.Padding(2);
            this._dtpCustom.Name = "_dtpCustom";
            this._dtpCustom.ShowUpDown = true;
            this._dtpCustom.Size = new System.Drawing.Size(83, 20);
            this._dtpCustom.TabIndex = 5;
            this._dtpCustom.ValueChanged += new System.EventHandler(this._dtpCustom_ValueChanged);
            // 
            // _rbCustom
            // 
            this._rbCustom.AutoSize = true;
            this._rbCustom.Location = new System.Drawing.Point(4, 62);
            this._rbCustom.Margin = new System.Windows.Forms.Padding(2);
            this._rbCustom.Name = "_rbCustom";
            this._rbCustom.Size = new System.Drawing.Size(60, 17);
            this._rbCustom.TabIndex = 4;
            this._rbCustom.Text = "Custom";
            this._rbCustom.UseVisualStyleBackColor = true;
            this._rbCustom.CheckedChanged += new System.EventHandler(this._rbCustom_CheckedChanged);
            // 
            // _dtpDelta
            // 
            this._dtpDelta.CustomFormat = "HH:mm:ss";
            this._dtpDelta.Enabled = false;
            this._dtpDelta.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._dtpDelta.Location = new System.Drawing.Point(103, 38);
            this._dtpDelta.Margin = new System.Windows.Forms.Padding(2);
            this._dtpDelta.Name = "_dtpDelta";
            this._dtpDelta.ShowUpDown = true;
            this._dtpDelta.Size = new System.Drawing.Size(83, 20);
            this._dtpDelta.TabIndex = 3;
            this._dtpDelta.ValueChanged += new System.EventHandler(this._dtpDelta_ValueChanged);
            // 
            // _rbEndOfDay
            // 
            this._rbEndOfDay.AutoSize = true;
            this._rbEndOfDay.Location = new System.Drawing.Point(4, 86);
            this._rbEndOfDay.Margin = new System.Windows.Forms.Padding(2);
            this._rbEndOfDay.Name = "_rbEndOfDay";
            this._rbEndOfDay.Size = new System.Drawing.Size(67, 17);
            this._rbEndOfDay.TabIndex = 2;
            this._rbEndOfDay.Text = "23:59:59";
            this._rbEndOfDay.UseVisualStyleBackColor = true;
            this._rbEndOfDay.CheckedChanged += new System.EventHandler(this._rbEndOfDay_CheckedChanged);
            // 
            // _rbDelta
            // 
            this._rbDelta.AutoSize = true;
            this._rbDelta.Location = new System.Drawing.Point(4, 38);
            this._rbDelta.Margin = new System.Windows.Forms.Padding(2);
            this._rbDelta.Name = "_rbDelta";
            this._rbDelta.Size = new System.Drawing.Size(81, 17);
            this._rbDelta.TabIndex = 1;
            this._rbDelta.Text = "Now - Delta";
            this._rbDelta.UseVisualStyleBackColor = true;
            this._rbDelta.CheckedChanged += new System.EventHandler(this._rbDelta_CheckedChanged);
            // 
            // _rbStartOfDay
            // 
            this._rbStartOfDay.AutoSize = true;
            this._rbStartOfDay.Checked = true;
            this._rbStartOfDay.Location = new System.Drawing.Point(4, 15);
            this._rbStartOfDay.Margin = new System.Windows.Forms.Padding(2);
            this._rbStartOfDay.Name = "_rbStartOfDay";
            this._rbStartOfDay.Size = new System.Drawing.Size(67, 17);
            this._rbStartOfDay.TabIndex = 0;
            this._rbStartOfDay.TabStop = true;
            this._rbStartOfDay.Text = "00:00:00";
            this._rbStartOfDay.UseVisualStyleBackColor = true;
            this._rbStartOfDay.CheckedChanged += new System.EventHandler(this._rbStartOfDay_CheckedChanged);
            // 
            // DateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 281);
            this.Controls.Add(this._gbSelectTime);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._mcDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Calendar";
            this.Load += new System.EventHandler(this.DateForm_Load);
            this.Shown += new System.EventHandler(this.DateForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DateForm_KeyDown);
            this._gbSelectTime.ResumeLayout(false);
            this._gbSelectTime.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MonthCalendar _mcDate;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.GroupBox _gbSelectTime;
        private System.Windows.Forms.RadioButton _rbEndOfDay;
        private System.Windows.Forms.RadioButton _rbDelta;
        private System.Windows.Forms.RadioButton _rbStartOfDay;
        private System.Windows.Forms.DateTimePicker _dtpDelta;
        private System.Windows.Forms.DateTimePicker _dtpCustom;
        private System.Windows.Forms.RadioButton _rbCustom;
    }
}