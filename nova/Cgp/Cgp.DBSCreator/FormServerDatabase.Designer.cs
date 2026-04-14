namespace Contal.Cgp.DBSCreator
{
    partial class FormServerDatabase
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
            this._lSqlServer = new System.Windows.Forms.Label();
            this._lDatabaseName = new System.Windows.Forms.Label();
            this._eSqlServer = new System.Windows.Forms.TextBox();
            this._cbDatabaseList = new System.Windows.Forms.ComboBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._bBack = new System.Windows.Forms.Button();
            this._lProgress = new System.Windows.Forms.Label();
            this._lProgress1 = new System.Windows.Forms.Label();
            this._cbExternDatabaseName = new System.Windows.Forms.ComboBox();
            this._lDatabaseName1 = new System.Windows.Forms.Label();
            this._gbExternalDatabase = new System.Windows.Forms.GroupBox();
            this._gbExternalDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lSqlServer
            // 
            this._lSqlServer.AutoSize = true;
            this._lSqlServer.Enabled = false;
            this._lSqlServer.Location = new System.Drawing.Point(25, 14);
            this._lSqlServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lSqlServer.Name = "_lSqlServer";
            this._lSqlServer.Size = new System.Drawing.Size(86, 17);
            this._lSqlServer.TabIndex = 0;
            this._lSqlServer.Text = "SQL Server:";
            // 
            // _lDatabaseName
            // 
            this._lDatabaseName.AutoSize = true;
            this._lDatabaseName.Enabled = false;
            this._lDatabaseName.Location = new System.Drawing.Point(25, 55);
            this._lDatabaseName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDatabaseName.Name = "_lDatabaseName";
            this._lDatabaseName.Size = new System.Drawing.Size(112, 17);
            this._lDatabaseName.TabIndex = 2;
            this._lDatabaseName.Text = "Database name:";
            // 
            // _eSqlServer
            // 
            this._eSqlServer.Enabled = false;
            this._eSqlServer.Location = new System.Drawing.Point(178, 6);
            this._eSqlServer.Margin = new System.Windows.Forms.Padding(4);
            this._eSqlServer.Name = "_eSqlServer";
            this._eSqlServer.Size = new System.Drawing.Size(238, 22);
            this._eSqlServer.TabIndex = 2;
            // 
            // _cbDatabaseList
            // 
            this._cbDatabaseList.FormattingEnabled = true;
            this._cbDatabaseList.Location = new System.Drawing.Point(178, 45);
            this._cbDatabaseList.Margin = new System.Windows.Forms.Padding(4);
            this._cbDatabaseList.Name = "_cbDatabaseList";
            this._cbDatabaseList.Size = new System.Drawing.Size(238, 24);
            this._cbDatabaseList.TabIndex = 3;
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(342, 184);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 29);
            this._bCancel.TabIndex = 4;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bNext
            // 
            this._bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bNext.Location = new System.Drawing.Point(215, 184);
            this._bNext.Margin = new System.Windows.Forms.Padding(4);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(94, 29);
            this._bNext.TabIndex = 0;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bBack
            // 
            this._bBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bBack.Location = new System.Drawing.Point(114, 184);
            this._bBack.Margin = new System.Windows.Forms.Padding(4);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(94, 29);
            this._bBack.TabIndex = 1;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _lProgress
            // 
            this._lProgress.AutoSize = true;
            this._lProgress.Enabled = false;
            this._lProgress.Location = new System.Drawing.Point(194, 75);
            this._lProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lProgress.Name = "_lProgress";
            this._lProgress.Size = new System.Drawing.Size(177, 17);
            this._lProgress.TabIndex = 4;
            this._lProgress.Text = "searching for databases ...";
            // 
            // _lProgress1
            // 
            this._lProgress1.AutoSize = true;
            this._lProgress1.Enabled = false;
            this._lProgress1.Location = new System.Drawing.Point(179, 54);
            this._lProgress1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lProgress1.Name = "_lProgress1";
            this._lProgress1.Size = new System.Drawing.Size(177, 17);
            this._lProgress1.TabIndex = 6;
            this._lProgress1.Text = "searching for databases ...";
            // 
            // _cbExternDatabaseName
            // 
            this._cbExternDatabaseName.FormattingEnabled = true;
            this._cbExternDatabaseName.Location = new System.Drawing.Point(162, 24);
            this._cbExternDatabaseName.Margin = new System.Windows.Forms.Padding(4);
            this._cbExternDatabaseName.Name = "_cbExternDatabaseName";
            this._cbExternDatabaseName.Size = new System.Drawing.Size(238, 24);
            this._cbExternDatabaseName.TabIndex = 5;
            // 
            // _lDatabaseName1
            // 
            this._lDatabaseName1.AutoSize = true;
            this._lDatabaseName1.Enabled = false;
            this._lDatabaseName1.Location = new System.Drawing.Point(10, 28);
            this._lDatabaseName1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDatabaseName1.Name = "_lDatabaseName1";
            this._lDatabaseName1.Size = new System.Drawing.Size(110, 17);
            this._lDatabaseName1.TabIndex = 7;
            this._lDatabaseName1.Text = "database name:";
            // 
            // _gbExternalDatabase
            // 
            this._gbExternalDatabase.Controls.Add(this._cbExternDatabaseName);
            this._gbExternalDatabase.Controls.Add(this._lProgress1);
            this._gbExternalDatabase.Controls.Add(this._lDatabaseName1);
            this._gbExternalDatabase.Location = new System.Drawing.Point(15, 95);
            this._gbExternalDatabase.Margin = new System.Windows.Forms.Padding(4);
            this._gbExternalDatabase.Name = "_gbExternalDatabase";
            this._gbExternalDatabase.Padding = new System.Windows.Forms.Padding(4);
            this._gbExternalDatabase.Size = new System.Drawing.Size(421, 81);
            this._gbExternalDatabase.TabIndex = 11;
            this._gbExternalDatabase.TabStop = false;
            this._gbExternalDatabase.Text = "External database";
            // 
            // FormServerDatabase
            // 
            this.AcceptButton = this._bNext;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(451, 228);
            this.ControlBox = false;
            this.Controls.Add(this._gbExternalDatabase);
            this.Controls.Add(this._lProgress);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._cbDatabaseList);
            this.Controls.Add(this._eSqlServer);
            this.Controls.Add(this._lDatabaseName);
            this.Controls.Add(this._lSqlServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormServerDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Server settings";
            this._gbExternalDatabase.ResumeLayout(false);
            this._gbExternalDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lSqlServer;
        private System.Windows.Forms.Label _lDatabaseName;
        private System.Windows.Forms.TextBox _eSqlServer;
        private System.Windows.Forms.ComboBox _cbDatabaseList;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Label _lProgress;
        private System.Windows.Forms.Label _lProgress1;
        private System.Windows.Forms.ComboBox _cbExternDatabaseName;
        private System.Windows.Forms.Label _lDatabaseName1;
        private System.Windows.Forms.GroupBox _gbExternalDatabase;
    }
}