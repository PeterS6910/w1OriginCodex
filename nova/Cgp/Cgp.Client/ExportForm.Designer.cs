namespace Contal.Cgp.Client
{
    partial class ExportForm
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
            this._bExport = new System.Windows.Forms.Button();
            this._bBrowse = new System.Windows.Forms.Button();
            this._eFilePath = new System.Windows.Forms.TextBox();
            this._lFilePath = new System.Windows.Forms.Label();
            this._saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._bClose = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.bBrowser = new System.Windows.Forms.Button();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbExport = new System.Windows.Forms.ProgressBar();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
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
            //// 
            //// _pbExport
            //// 
            //this._pbExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            //| System.Windows.Forms.AnchorStyles.Right)));
            //this._pbExport.Location = new System.Drawing.Point(18, 190);
            //this._pbExport.Name = "_pbExport";
            //this._pbExport.Size = new System.Drawing.Size(553, 23);
            //this._pbExport.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(373, 76);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Export";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this._bExport_Click);
            // 
            // bBrowser
            // 
            this.bBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bBrowser.Location = new System.Drawing.Point(373, 21);
            this.bBrowser.Name = "bBrowser";
            this.bBrowser.Size = new System.Drawing.Size(75, 23);
            this.bBrowser.TabIndex = 15;
            this.bBrowser.Text = "Browse";
            this.bBrowser.UseVisualStyleBackColor = true;
            this.bBrowser.Click += new System.EventHandler(this._bBrowse_Click);
            // 
            // tbPath
            // 
            this.tbPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPath.Location = new System.Drawing.Point(77, 22);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(290, 20);
            this.tbPath.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "File path";
            // 
            // pbExport
            // 
            this.pbExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbExport.Location = new System.Drawing.Point(27, 122);
            this.pbExport.Name = "pbExport";
            this.pbExport.Size = new System.Drawing.Size(340, 23);
            this.pbExport.TabIndex = 13;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(373, 124);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 17;
            this.button3.Text = "Close";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this._bClose_Click);
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(477, 169);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bBrowser);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbExport);
            this.Controls.Add(this.button3);
            this.Name = "ExportForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox _eFilePath;
        private System.Windows.Forms.Label _lFilePath;
        private System.Windows.Forms.Button _bBrowse;
        private System.Windows.Forms.Button _bExport;
        private System.Windows.Forms.SaveFileDialog _saveFileDialog;
        private System.Windows.Forms.Button _bClose;
        private System.Windows.Forms.ProgressBar _pbExport;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button bBrowser;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar pbExport;
        private System.Windows.Forms.Button button3;
    }
}
