namespace Contal.Cgp.Client
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
            this._mcExpirationDate = new System.Windows.Forms.MonthCalendar();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _mcExpirationDate
            // 
            this._mcExpirationDate.Location = new System.Drawing.Point(20, 20);
            this._mcExpirationDate.Margin = new System.Windows.Forms.Padding(11);
            this._mcExpirationDate.MaxSelectionCount = 1;
            this._mcExpirationDate.Name = "_mcExpirationDate";
            this._mcExpirationDate.ShowWeekNumbers = true;
            this._mcExpirationDate.TabIndex = 0;
            this._mcExpirationDate.MouseDown += new System.Windows.Forms.MouseEventHandler(this._mcExpirationDate_MouseDown);
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(138, 242);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(74, 29);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Location = new System.Drawing.Point(20, 242);
            this._bOk.Margin = new System.Windows.Forms.Padding(4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(78, 29);
            this._bOk.TabIndex = 1;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // CalendarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(243, 281);
            this.ControlBox = false;
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._mcExpirationDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CalendarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CalendarForm";
            this.Load += new System.EventHandler(this.CalendarForm_Load);
            this.Shown += new System.EventHandler(this.CalendarForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalendarForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MonthCalendar _mcExpirationDate;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
    }
}