namespace Contal.Cgp.NCAS.Client
{
    partial class PersonAclAssignment
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
            this._bContinue = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._tbdpDateTo = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._tbdpDateFrom = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._lDateTo = new System.Windows.Forms.Label();
            this._lDateFrom = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _bContinue
            // 
            this._bContinue.Location = new System.Drawing.Point(269, 79);
            this._bContinue.Name = "_bContinue";
            this._bContinue.Size = new System.Drawing.Size(75, 23);
            this._bContinue.TabIndex = 4;
            this._bContinue.Text = "Continue";
            this._bContinue.UseVisualStyleBackColor = true;
            this._bContinue.Click += new System.EventHandler(this._bContinue_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(350, 79);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _tbdpDateTo
            // 
            this._tbdpDateTo.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateTo.ButtonClearDateImage = null;
            this._tbdpDateTo.ButtonClearDateText = "";
            this._tbdpDateTo.ButtonClearDateWidth = 23;
            this._tbdpDateTo.ButtonDateImage = null;
            this._tbdpDateTo.ButtonDateText = "";
            this._tbdpDateTo.ButtonDateWidth = 23;
            // 
            // 
            // 
            this._tbdpDateTo.CustomFormat = "d. M. yyyy";
            this._tbdpDateTo.DateFormName = "Calendar";
            this._tbdpDateTo.LocalizationHelper = null;
            this._tbdpDateTo.Location = new System.Drawing.Point(220, 30);
            this._tbdpDateTo.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateTo.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateTo.Name = "_tbdpDateTo";
            this._tbdpDateTo.ReadOnly = false;
            this._tbdpDateTo.Size = new System.Drawing.Size(202, 22);
            this._tbdpDateTo.TabIndex = 3;
            this._tbdpDateTo.ValidateAfter = 2;
            this._tbdpDateTo.ValidationEnabled = false;
            this._tbdpDateTo.ValidationError = "";
            this._tbdpDateTo.Value = null;
            // 
            // _tbdpDateFrom
            // 
            this._tbdpDateFrom.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateFrom.ButtonClearDateImage = null;
            this._tbdpDateFrom.ButtonClearDateText = "";
            this._tbdpDateFrom.ButtonClearDateWidth = 23;
            this._tbdpDateFrom.ButtonDateImage = null;
            this._tbdpDateFrom.ButtonDateText = "";
            this._tbdpDateFrom.ButtonDateWidth = 23;
            // 
            // 
            // 
            this._tbdpDateFrom.CustomFormat = "d. M. yyyy";
            this._tbdpDateFrom.DateFormName = "Calendar";
            this._tbdpDateFrom.LocalizationHelper = null;
            this._tbdpDateFrom.Location = new System.Drawing.Point(12, 30);
            this._tbdpDateFrom.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateFrom.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateFrom.Name = "_tbdpDateFrom";
            this._tbdpDateFrom.ReadOnly = false;
            this._tbdpDateFrom.Size = new System.Drawing.Size(202, 22);
            this._tbdpDateFrom.TabIndex = 1;
            this._tbdpDateFrom.ValidateAfter = 2;
            this._tbdpDateFrom.ValidationEnabled = false;
            this._tbdpDateFrom.ValidationError = "";
            this._tbdpDateFrom.Value = null;
            // 
            // _lDateTo
            // 
            this._lDateTo.AutoSize = true;
            this._lDateTo.Location = new System.Drawing.Point(217, 14);
            this._lDateTo.Name = "_lDateTo";
            this._lDateTo.Size = new System.Drawing.Size(42, 13);
            this._lDateTo.TabIndex = 2;
            this._lDateTo.Text = "Date to";
            // 
            // _lDateFrom
            // 
            this._lDateFrom.AutoSize = true;
            this._lDateFrom.Location = new System.Drawing.Point(9, 14);
            this._lDateFrom.Name = "_lDateFrom";
            this._lDateFrom.Size = new System.Drawing.Size(53, 13);
            this._lDateFrom.TabIndex = 0;
            this._lDateFrom.Text = "Date from";
            // 
            // PersonAclAssignment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 114);
            this.Controls.Add(this._tbdpDateTo);
            this.Controls.Add(this._tbdpDateFrom);
            this.Controls.Add(this._lDateTo);
            this.Controls.Add(this._lDateFrom);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bContinue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PersonAclAssignment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PersonAclAssignment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bContinue;
        private System.Windows.Forms.Button _bCancel;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateTo;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateFrom;
        private System.Windows.Forms.Label _lDateTo;
        private System.Windows.Forms.Label _lDateFrom;
    }
}