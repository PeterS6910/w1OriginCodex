namespace Contal.Cgp.Client
{
    partial class EventlogsExportForm
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
            this._cbColumnDescription = new System.Windows.Forms.CheckBox();
            this._cbColumnEventSources = new System.Windows.Forms.CheckBox();
            this._cbColumnCGPSource = new System.Windows.Forms.CheckBox();
            this._cbColumnDate = new System.Windows.Forms.CheckBox();
            this._cbColumnType = new System.Windows.Forms.CheckBox();
            this._gbExportSettings = new System.Windows.Forms.GroupBox();
            this._cbDateFormat = new System.Windows.Forms.ComboBox();
            this._lDateFormat = new System.Windows.Forms.Label();
            this._cbAddColumnHeaders = new System.Windows.Forms.CheckBox();
            this._cbSeparator = new System.Windows.Forms.ComboBox();
            this._lSeparator = new System.Windows.Forms.Label();
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
            this._gbColumns.Controls.Add(this._cbColumnDescription);
            this._gbColumns.Controls.Add(this._cbColumnEventSources);
            this._gbColumns.Controls.Add(this._cbColumnCGPSource);
            this._gbColumns.Controls.Add(this._cbColumnDate);
            this._gbColumns.Controls.Add(this._cbColumnType);
            this._gbColumns.Location = new System.Drawing.Point(12, 12);
            this._gbColumns.Name = "_gbColumns";
            this._gbColumns.Size = new System.Drawing.Size(223, 172);
            this._gbColumns.TabIndex = 0;
            this._gbColumns.TabStop = false;
            this._gbColumns.Text = "Columns";
            // 
            // _cbColumnDescription
            // 
            this._cbColumnDescription.AutoSize = true;
            this._cbColumnDescription.Checked = true;
            this._cbColumnDescription.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnDescription.Location = new System.Drawing.Point(6, 111);
            this._cbColumnDescription.Name = "_cbColumnDescription";
            this._cbColumnDescription.Size = new System.Drawing.Size(79, 17);
            this._cbColumnDescription.TabIndex = 4;
            this._cbColumnDescription.Text = "Description";
            this._cbColumnDescription.UseVisualStyleBackColor = true;
            // 
            // _cbColumnEventSources
            // 
            this._cbColumnEventSources.AutoSize = true;
            this._cbColumnEventSources.Checked = true;
            this._cbColumnEventSources.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnEventSources.Location = new System.Drawing.Point(6, 88);
            this._cbColumnEventSources.Name = "_cbColumnEventSources";
            this._cbColumnEventSources.Size = new System.Drawing.Size(94, 17);
            this._cbColumnEventSources.TabIndex = 3;
            this._cbColumnEventSources.Text = "Event sources";
            this._cbColumnEventSources.UseVisualStyleBackColor = true;
            // 
            // _cbColumnCGPSource
            // 
            this._cbColumnCGPSource.AutoSize = true;
            this._cbColumnCGPSource.Checked = true;
            this._cbColumnCGPSource.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnCGPSource.Location = new System.Drawing.Point(6, 65);
            this._cbColumnCGPSource.Name = "_cbColumnCGPSource";
            this._cbColumnCGPSource.Size = new System.Drawing.Size(83, 17);
            this._cbColumnCGPSource.TabIndex = 2;
            this._cbColumnCGPSource.Text = "CGP source";
            this._cbColumnCGPSource.UseVisualStyleBackColor = true;
            // 
            // _cbColumnDate
            // 
            this._cbColumnDate.AutoSize = true;
            this._cbColumnDate.Checked = true;
            this._cbColumnDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnDate.Location = new System.Drawing.Point(6, 42);
            this._cbColumnDate.Name = "_cbColumnDate";
            this._cbColumnDate.Size = new System.Drawing.Size(49, 17);
            this._cbColumnDate.TabIndex = 1;
            this._cbColumnDate.Text = "Date";
            this._cbColumnDate.UseVisualStyleBackColor = true;
            // 
            // _cbColumnType
            // 
            this._cbColumnType.AutoSize = true;
            this._cbColumnType.Checked = true;
            this._cbColumnType.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbColumnType.Location = new System.Drawing.Point(6, 19);
            this._cbColumnType.Name = "_cbColumnType";
            this._cbColumnType.Size = new System.Drawing.Size(50, 17);
            this._cbColumnType.TabIndex = 0;
            this._cbColumnType.Text = "Type";
            this._cbColumnType.UseVisualStyleBackColor = true;
            // 
            // _gbExportSettings
            // 
            this._gbExportSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbExportSettings.Controls.Add(this._cbDateFormat);
            this._gbExportSettings.Controls.Add(this._lDateFormat);
            this._gbExportSettings.Controls.Add(this._cbAddColumnHeaders);
            this._gbExportSettings.Controls.Add(this._cbSeparator);
            this._gbExportSettings.Controls.Add(this._lSeparator);
            this._gbExportSettings.Controls.Add(this._bExport);
            this._gbExportSettings.Controls.Add(this._bBrowse);
            this._gbExportSettings.Controls.Add(this._eFilePath);
            this._gbExportSettings.Controls.Add(this._lFilePath);
            this._gbExportSettings.Location = new System.Drawing.Point(241, 17);
            this._gbExportSettings.Name = "_gbExportSettings";
            this._gbExportSettings.Size = new System.Drawing.Size(411, 167);
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
            this._cbDateFormat.Location = new System.Drawing.Point(95, 95);
            this._cbDateFormat.Name = "_cbDateFormat";
            this._cbDateFormat.Size = new System.Drawing.Size(229, 21);
            this._cbDateFormat.TabIndex = 9;
            // 
            // _lDateFormat
            // 
            this._lDateFormat.AutoSize = true;
            this._lDateFormat.Location = new System.Drawing.Point(6, 98);
            this._lDateFormat.Name = "_lDateFormat";
            this._lDateFormat.Size = new System.Drawing.Size(62, 13);
            this._lDateFormat.TabIndex = 0;
            this._lDateFormat.Text = "Date format";
            // 
            // _cbAddColumnHeaders
            // 
            this._cbAddColumnHeaders.AutoSize = true;
            this._cbAddColumnHeaders.Checked = true;
            this._cbAddColumnHeaders.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbAddColumnHeaders.Location = new System.Drawing.Point(6, 19);
            this._cbAddColumnHeaders.Name = "_cbAddColumnHeaders";
            this._cbAddColumnHeaders.Size = new System.Drawing.Size(123, 17);
            this._cbAddColumnHeaders.TabIndex = 5;
            this._cbAddColumnHeaders.Text = "Add column headers";
            this._cbAddColumnHeaders.UseVisualStyleBackColor = true;
            // 
            // _cbSeparator
            // 
            this._cbSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cbSeparator.FormattingEnabled = true;
            this._cbSeparator.Location = new System.Drawing.Point(95, 68);
            this._cbSeparator.Name = "_cbSeparator";
            this._cbSeparator.Size = new System.Drawing.Size(229, 21);
            this._cbSeparator.TabIndex = 8;
            // 
            // _lSeparator
            // 
            this._lSeparator.AutoSize = true;
            this._lSeparator.Location = new System.Drawing.Point(6, 71);
            this._lSeparator.Name = "_lSeparator";
            this._lSeparator.Size = new System.Drawing.Size(53, 13);
            this._lSeparator.TabIndex = 0;
            this._lSeparator.Text = "Separator";
            // 
            // _bExport
            // 
            this._bExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bExport.Location = new System.Drawing.Point(330, 138);
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
            this._bClose.Location = new System.Drawing.Point(577, 190);
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
            this._pbExport.Location = new System.Drawing.Point(18, 190);
            this._pbExport.Name = "_pbExport";
            this._pbExport.Size = new System.Drawing.Size(553, 23);
            this._pbExport.TabIndex = 0;
            // 
            // EventlogsExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(664, 225);
            this.Controls.Add(this._pbExport);
            this.Controls.Add(this._bClose);
            this.Controls.Add(this._gbExportSettings);
            this.Controls.Add(this._gbColumns);
            this.Name = "EventlogsExportForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EvnetlogsExportForm";
            this._gbColumns.ResumeLayout(false);
            this._gbColumns.PerformLayout();
            this._gbExportSettings.ResumeLayout(false);
            this._gbExportSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _gbColumns;
        private System.Windows.Forms.CheckBox _cbColumnType;
        private System.Windows.Forms.CheckBox _cbColumnDescription;
        private System.Windows.Forms.CheckBox _cbColumnEventSources;
        private System.Windows.Forms.CheckBox _cbColumnCGPSource;
        private System.Windows.Forms.CheckBox _cbColumnDate;
        private System.Windows.Forms.GroupBox _gbExportSettings;
        private System.Windows.Forms.TextBox _eFilePath;
        private System.Windows.Forms.Label _lFilePath;
        private System.Windows.Forms.Button _bBrowse;
        private System.Windows.Forms.Button _bExport;
        private System.Windows.Forms.Label _lSeparator;
        private System.Windows.Forms.ComboBox _cbSeparator;
        private System.Windows.Forms.SaveFileDialog _saveFileDialog;
        private System.Windows.Forms.Button _bClose;
        private System.Windows.Forms.CheckBox _cbAddColumnHeaders;
        private System.Windows.Forms.ProgressBar _pbExport;
        private System.Windows.Forms.ComboBox _cbDateFormat;
        private System.Windows.Forms.Label _lDateFormat;
    }
}