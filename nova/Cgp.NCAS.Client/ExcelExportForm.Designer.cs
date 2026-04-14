namespace Contal.Cgp.NCAS.Client
{
    partial class ExcelExportForm
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
            this._gbColumns = new System.Windows.Forms.GroupBox();
            this._cbColumnEmail = new System.Windows.Forms.CheckBox();
            this._cbColumnPhone = new System.Windows.Forms.CheckBox();
            this._cbColumnDepartment = new System.Windows.Forms.CheckBox();
            this._cbColumnBD = new System.Windows.Forms.CheckBox();
            this._cbColumnID = new System.Windows.Forms.CheckBox();
            this._gbExportSettings = new System.Windows.Forms.GroupBox();
            this._cbDateFormat = new System.Windows.Forms.ComboBox();
            this._lDateFormat = new System.Windows.Forms.Label();
            this._bExport = new System.Windows.Forms.Button();
            this._bBrowse = new System.Windows.Forms.Button();
            this._eFilePath = new System.Windows.Forms.TextBox();
            this._lFilePath = new System.Windows.Forms.Label();
            this._saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._bClose = new System.Windows.Forms.Button();
            this._pbExport = new System.Windows.Forms.ProgressBar();
            this._gbColumns.SuspendLayout();
            this._gbExportSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // _gbColumns
            // 
            this._gbColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._gbColumns.Controls.Add(this._cbColumnEmail);
            this._gbColumns.Controls.Add(this._cbColumnPhone);
            this._gbColumns.Controls.Add(this._cbColumnDepartment);
            this._gbColumns.Controls.Add(this._cbColumnBD);
            this._gbColumns.Controls.Add(this._cbColumnID);
            this._gbColumns.Location = new System.Drawing.Point(12, 12);
            this._gbColumns.Name = "_gbColumns";
            this._gbColumns.Size = new System.Drawing.Size(223, 127);
            this._gbColumns.TabIndex = 0;
            this._gbColumns.TabStop = false;
            this._gbColumns.Text = "Columns";
            // 
            // _cbColumnEmail
            // 
            this._cbColumnEmail.AutoSize = true;
            this._cbColumnEmail.Checked = true;
            this._cbColumnEmail.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnEmail.Location = new System.Drawing.Point(6, 111);
            this._cbColumnEmail.Name = "_cbColumnEmail";
            this._cbColumnEmail.Size = new System.Drawing.Size(51, 17);
            this._cbColumnEmail.TabIndex = 4;
            this._cbColumnEmail.Text = "Email";
            this._cbColumnEmail.UseVisualStyleBackColor = true;
            // 
            // _cbColumnPhone
            // 
            this._cbColumnPhone.AutoSize = true;
            this._cbColumnPhone.Checked = true;
            this._cbColumnPhone.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnPhone.Location = new System.Drawing.Point(6, 88);
            this._cbColumnPhone.Name = "_cbColumnPhone";
            this._cbColumnPhone.Size = new System.Drawing.Size(57, 17);
            this._cbColumnPhone.TabIndex = 3;
            this._cbColumnPhone.Text = "Phone";
            this._cbColumnPhone.UseVisualStyleBackColor = true;
            // 
            // _cbColumnDepartment
            // 
            this._cbColumnDepartment.AutoSize = true;
            this._cbColumnDepartment.Checked = true;
            this._cbColumnDepartment.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnDepartment.Location = new System.Drawing.Point(6, 65);
            this._cbColumnDepartment.Name = "_cbColumnDepartment";
            this._cbColumnDepartment.Size = new System.Drawing.Size(81, 17);
            this._cbColumnDepartment.TabIndex = 2;
            this._cbColumnDepartment.Text = "Department";
            this._cbColumnDepartment.UseVisualStyleBackColor = true;
            // 
            // _cbColumnBD
            // 
            this._cbColumnBD.AutoSize = true;
            this._cbColumnBD.Checked = true;
            this._cbColumnBD.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnBD.Location = new System.Drawing.Point(6, 42);
            this._cbColumnBD.Name = "_cbColumnBD";
            this._cbColumnBD.Size = new System.Drawing.Size(66, 17);
            this._cbColumnBD.TabIndex = 1;
            this._cbColumnBD.Text = "BirthDay";
            this._cbColumnBD.UseVisualStyleBackColor = true;
            // 
            // _cbColumnID
            // 
            this._cbColumnID.AutoSize = true;
            this._cbColumnID.Checked = true;
            this._cbColumnID.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnID.Location = new System.Drawing.Point(6, 19);
            this._cbColumnID.Name = "_cbColumnID";
            this._cbColumnID.Size = new System.Drawing.Size(81, 17);
            this._cbColumnID.TabIndex = 0;
            this._cbColumnID.Text = "Personal ID";
            this._cbColumnID.UseVisualStyleBackColor = true;
            // 
            // _gbExportSettings
            // 
            this._gbExportSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbExportSettings.Controls.Add(this._cbDateFormat);
            this._gbExportSettings.Controls.Add(this._lDateFormat);
            this._gbExportSettings.Controls.Add(this._bExport);
            this._gbExportSettings.Controls.Add(this._bBrowse);
            this._gbExportSettings.Controls.Add(this._eFilePath);
            this._gbExportSettings.Controls.Add(this._lFilePath);
            this._gbExportSettings.Location = new System.Drawing.Point(241, 17);
            this._gbExportSettings.Name = "_gbExportSettings";
            this._gbExportSettings.Size = new System.Drawing.Size(411, 122);
            this._gbExportSettings.TabIndex = 1;
            this._gbExportSettings.TabStop = false;
            this._gbExportSettings.Text = "Export settings";
            // 
            // _cbDateFormat
            // 
            this._cbDateFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cbDateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbDateFormat.FormattingEnabled = true;
            this._cbDateFormat.Location = new System.Drawing.Point(95, 86);
            this._cbDateFormat.Name = "_cbDateFormat";
            this._cbDateFormat.Size = new System.Drawing.Size(229, 21);
            this._cbDateFormat.TabIndex = 9;
            this._cbDateFormat.Visible = false;
            // 
            // _lDateFormat
            // 
            this._lDateFormat.AutoSize = true;
            this._lDateFormat.Location = new System.Drawing.Point(6, 89);
            this._lDateFormat.Name = "_lDateFormat";
            this._lDateFormat.Size = new System.Drawing.Size(62, 13);
            this._lDateFormat.TabIndex = 0;
            this._lDateFormat.Text = "Date format";
            this._lDateFormat.Visible = false;
            // 
            // _bExport
            // 
            this._bExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bExport.Location = new System.Drawing.Point(330, 83);
            this._bExport.Name = "_bExport";
            this._bExport.Size = new System.Drawing.Size(75, 23);
            this._bExport.TabIndex = 10;
            this._bExport.Text = "Export";
            this._bExport.UseVisualStyleBackColor = true;
            this._bExport.Click += new System.EventHandler(this._bExport_Click);
            // 
            // _bBrowse
            // 
            this._bBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bBrowse.Location = new System.Drawing.Point(330, 40);
            this._bBrowse.Name = "_bBrowse";
            this._bBrowse.Size = new System.Drawing.Size(75, 23);
            this._bBrowse.TabIndex = 7;
            this._bBrowse.Text = "Browse";
            this._bBrowse.UseVisualStyleBackColor = true;
            this._bBrowse.Click += new System.EventHandler(this._bBrowse_Click);
            // 
            // _eFilePath
            // 
            this._eFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eFilePath.Location = new System.Drawing.Point(95, 42);
            this._eFilePath.Name = "_eFilePath";
            this._eFilePath.Size = new System.Drawing.Size(229, 20);
            this._eFilePath.TabIndex = 6;
            // 
            // _lFilePath
            // 
            this._lFilePath.AutoSize = true;
            this._lFilePath.Location = new System.Drawing.Point(6, 45);
            this._lFilePath.Name = "_lFilePath";
            this._lFilePath.Size = new System.Drawing.Size(47, 13);
            this._lFilePath.TabIndex = 0;
            this._lFilePath.Text = "File path";
            // 
            // _saveFileDialog
            // 
            this._saveFileDialog.DefaultExt = "csv";
            this._saveFileDialog.Filter = "(*.csv, *.tsv)|*.csv;*.tsv|(*.txt)|*.txt|(*.*)|*.*";
            // 
            // _bClose
            // 
            this._bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bClose.Location = new System.Drawing.Point(577, 158);
            this._bClose.Name = "_bClose";
            this._bClose.Size = new System.Drawing.Size(75, 23);
            this._bClose.TabIndex = 11;
            this._bClose.Text = "Close";
            this._bClose.UseVisualStyleBackColor = true;
            this._bClose.Click += new System.EventHandler(this._bClose_Click);
            // 
            // _pbExport
            // 
            this._pbExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pbExport.Location = new System.Drawing.Point(12, 158);
            this._pbExport.Name = "_pbExport";
            this._pbExport.Size = new System.Drawing.Size(553, 23);
            this._pbExport.TabIndex = 0;
            // 
            // ExcelExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(664, 202);
            this.Controls.Add(this._pbExport);
            this.Controls.Add(this._bClose);
            this.Controls.Add(this._gbExportSettings);
            this.Controls.Add(this._gbColumns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ExcelExportForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Data Form";
            this._gbColumns.ResumeLayout(false);
            this._gbColumns.PerformLayout();
            this._gbExportSettings.ResumeLayout(false);
            this._gbExportSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _gbColumns;
        private System.Windows.Forms.CheckBox _cbColumnID;
        private System.Windows.Forms.CheckBox _cbColumnEmail;
        private System.Windows.Forms.CheckBox _cbColumnPhone;
        private System.Windows.Forms.CheckBox _cbColumnDepartment;
        private System.Windows.Forms.CheckBox _cbColumnBD;
        private System.Windows.Forms.GroupBox _gbExportSettings;
        private System.Windows.Forms.TextBox _eFilePath;
        private System.Windows.Forms.Label _lFilePath;
        private System.Windows.Forms.Button _bBrowse;
        private System.Windows.Forms.Button _bExport;
        private System.Windows.Forms.SaveFileDialog _saveFileDialog;
        private System.Windows.Forms.Button _bClose;
        private System.Windows.Forms.ProgressBar _pbExport;
        private System.Windows.Forms.ComboBox _cbDateFormat;
        private System.Windows.Forms.Label _lDateFormat;

        public override NCASClient Plugin => NCASClient.Singleton;
    }
}
