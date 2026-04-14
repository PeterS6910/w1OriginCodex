namespace Contal.Cgp.Client
{
    partial class TimeZoneCalendarChangeForm
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
            this._eErrors = new System.Windows.Forms.TextBox();
            this._lInfo = new System.Windows.Forms.Label();
            this._bNewCalendar = new System.Windows.Forms.Button();
            this._bOldCalendar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _eErrors
            // 
            this._eErrors.BackColor = System.Drawing.SystemColors.Window;
            this._eErrors.Location = new System.Drawing.Point(12, 25);
            this._eErrors.Multiline = true;
            this._eErrors.Name = "_eErrors";
            this._eErrors.ReadOnly = true;
            this._eErrors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eErrors.Size = new System.Drawing.Size(259, 168);
            this._eErrors.TabIndex = 0;
            // 
            // _lInfo
            // 
            this._lInfo.AutoSize = true;
            this._lInfo.Location = new System.Drawing.Point(12, 9);
            this._lInfo.Name = "_lInfo";
            this._lInfo.Size = new System.Drawing.Size(172, 13);
            this._lInfo.TabIndex = 1;
            this._lInfo.Text = "Calendar doesn\'t contain day types";
            // 
            // _bNewCalendar
            // 
            this._bNewCalendar.Location = new System.Drawing.Point(116, 199);
            this._bNewCalendar.Name = "_bNewCalendar";
            this._bNewCalendar.Size = new System.Drawing.Size(75, 23);
            this._bNewCalendar.TabIndex = 1;
            this._bNewCalendar.Text = "Set New";
            this._bNewCalendar.UseVisualStyleBackColor = true;
            this._bNewCalendar.Click += new System.EventHandler(this._bNewCalendar_Click);
            // 
            // _bOldCalendar
            // 
            this._bOldCalendar.Location = new System.Drawing.Point(197, 199);
            this._bOldCalendar.Name = "_bOldCalendar";
            this._bOldCalendar.Size = new System.Drawing.Size(75, 23);
            this._bOldCalendar.TabIndex = 2;
            this._bOldCalendar.Text = "Keep Old";
            this._bOldCalendar.UseVisualStyleBackColor = true;
            this._bOldCalendar.Click += new System.EventHandler(this._bOldCalendar_Click);
            // 
            // TimeZoneCalendarChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 243);
            this.ControlBox = false;
            this.Controls.Add(this._bOldCalendar);
            this.Controls.Add(this._bNewCalendar);
            this.Controls.Add(this._lInfo);
            this.Controls.Add(this._eErrors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TimeZoneCalendarChangeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TimeZoneCalendarChangeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _eErrors;
        private System.Windows.Forms.Label _lInfo;
        private System.Windows.Forms.Button _bNewCalendar;
        private System.Windows.Forms.Button _bOldCalendar;
    }
}