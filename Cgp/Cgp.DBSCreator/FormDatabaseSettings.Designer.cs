namespace Contal.Cgp.DBSCreator
{
    partial class FormDatabaseSettings
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
            this._bBack = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lMainInfo = new System.Windows.Forms.Label();
            this._lSqlServer = new System.Windows.Forms.Label();
            this._lDatabaseName = new System.Windows.Forms.Label();
            this._eSqlServer = new System.Windows.Forms.TextBox();
            this._eDatabase = new System.Windows.Forms.TextBox();
            this._eDatabasePath = new System.Windows.Forms.TextBox();
            this._lPath = new System.Windows.Forms.Label();
            this._bBrowse = new System.Windows.Forms.Button();
            this._folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this._bBrowse1 = new System.Windows.Forms.Button();
            this._lPath1 = new System.Windows.Forms.Label();
            this._eExternDatabasePath = new System.Windows.Forms.TextBox();
            this._eExternDatabase = new System.Windows.Forms.TextBox();
            this._lDatabaseName1 = new System.Windows.Forms.Label();
            this._gbExternalDatabase = new System.Windows.Forms.GroupBox();
            this._gbExternalDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bBack
            // 
            this._bBack.Location = new System.Drawing.Point(176, 338);
            this._bBack.Margin = new System.Windows.Forms.Padding(4);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(94, 29);
            this._bBack.TabIndex = 1;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _bNext
            // 
            this._bNext.Location = new System.Drawing.Point(278, 338);
            this._bNext.Margin = new System.Windows.Forms.Padding(4);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(94, 29);
            this._bNext.TabIndex = 0;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bCancel
            // 
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(405, 338);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 29);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lMainInfo
            // 
            this._lMainInfo.AutoSize = true;
            this._lMainInfo.Enabled = false;
            this._lMainInfo.Location = new System.Drawing.Point(16, 9);
            this._lMainInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMainInfo.Name = "_lMainInfo";
            this._lMainInfo.Size = new System.Drawing.Size(122, 17);
            this._lMainInfo.TabIndex = 10;
            this._lMainInfo.Text = "Database settings";
            // 
            // _lSqlServer
            // 
            this._lSqlServer.AutoSize = true;
            this._lSqlServer.Enabled = false;
            this._lSqlServer.Location = new System.Drawing.Point(16, 31);
            this._lSqlServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lSqlServer.Name = "_lSqlServer";
            this._lSqlServer.Size = new System.Drawing.Size(86, 17);
            this._lSqlServer.TabIndex = 11;
            this._lSqlServer.Text = "SQL Server:";
            // 
            // _lDatabaseName
            // 
            this._lDatabaseName.AutoSize = true;
            this._lDatabaseName.Enabled = false;
            this._lDatabaseName.Location = new System.Drawing.Point(16, 66);
            this._lDatabaseName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDatabaseName.Name = "_lDatabaseName";
            this._lDatabaseName.Size = new System.Drawing.Size(112, 17);
            this._lDatabaseName.TabIndex = 12;
            this._lDatabaseName.Text = "Database name:";
            // 
            // _eSqlServer
            // 
            this._eSqlServer.Enabled = false;
            this._eSqlServer.Location = new System.Drawing.Point(216, 28);
            this._eSqlServer.Margin = new System.Windows.Forms.Padding(4);
            this._eSqlServer.Name = "_eSqlServer";
            this._eSqlServer.Size = new System.Drawing.Size(282, 22);
            this._eSqlServer.TabIndex = 2;
            // 
            // _eDatabase
            // 
            this._eDatabase.Enabled = false;
            this._eDatabase.Location = new System.Drawing.Point(216, 58);
            this._eDatabase.Margin = new System.Windows.Forms.Padding(4);
            this._eDatabase.Name = "_eDatabase";
            this._eDatabase.Size = new System.Drawing.Size(282, 22);
            this._eDatabase.TabIndex = 3;
            // 
            // _eDatabasePath
            // 
            this._eDatabasePath.Location = new System.Drawing.Point(216, 92);
            this._eDatabasePath.Margin = new System.Windows.Forms.Padding(4);
            this._eDatabasePath.Name = "_eDatabasePath";
            this._eDatabasePath.Size = new System.Drawing.Size(282, 22);
            this._eDatabasePath.TabIndex = 5;
            // 
            // _lPath
            // 
            this._lPath.AutoSize = true;
            this._lPath.Enabled = false;
            this._lPath.Location = new System.Drawing.Point(16, 101);
            this._lPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPath.Name = "_lPath";
            this._lPath.Size = new System.Drawing.Size(156, 17);
            this._lPath.TabIndex = 16;
            this._lPath.Text = "Path to databases files:";
            // 
            // _bBrowse
            // 
            this._bBrowse.Location = new System.Drawing.Point(405, 125);
            this._bBrowse.Margin = new System.Windows.Forms.Padding(4);
            this._bBrowse.Name = "_bBrowse";
            this._bBrowse.Size = new System.Drawing.Size(94, 29);
            this._bBrowse.TabIndex = 6;
            this._bBrowse.Text = "Browse";
            this._bBrowse.UseVisualStyleBackColor = true;
            this._bBrowse.Click += new System.EventHandler(this._bBrovse_Click);
            // 
            // _bBrowse1
            // 
            this._bBrowse1.Location = new System.Drawing.Point(382, 91);
            this._bBrowse1.Margin = new System.Windows.Forms.Padding(4);
            this._bBrowse1.Name = "_bBrowse1";
            this._bBrowse1.Size = new System.Drawing.Size(94, 29);
            this._bBrowse1.TabIndex = 20;
            this._bBrowse1.Text = "Browse";
            this._bBrowse1.UseVisualStyleBackColor = true;
            this._bBrowse1.Click += new System.EventHandler(this._bBrowse1_Click);
            // 
            // _lPath1
            // 
            this._lPath1.AutoSize = true;
            this._lPath1.Enabled = false;
            this._lPath1.Location = new System.Drawing.Point(8, 68);
            this._lPath1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPath1.Name = "_lPath1";
            this._lPath1.Size = new System.Drawing.Size(156, 17);
            this._lPath1.TabIndex = 24;
            this._lPath1.Text = "Path to databases files:";
            // 
            // _eExternDatabasePath
            // 
            this._eExternDatabasePath.Location = new System.Drawing.Point(208, 59);
            this._eExternDatabasePath.Margin = new System.Windows.Forms.Padding(4);
            this._eExternDatabasePath.Name = "_eExternDatabasePath";
            this._eExternDatabasePath.Size = new System.Drawing.Size(268, 22);
            this._eExternDatabasePath.TabIndex = 19;
            // 
            // _eExternDatabase
            // 
            this._eExternDatabase.Enabled = false;
            this._eExternDatabase.Location = new System.Drawing.Point(208, 24);
            this._eExternDatabase.Margin = new System.Windows.Forms.Padding(4);
            this._eExternDatabase.Name = "_eExternDatabase";
            this._eExternDatabase.Size = new System.Drawing.Size(268, 22);
            this._eExternDatabase.TabIndex = 18;
            // 
            // _lDatabaseName1
            // 
            this._lDatabaseName1.AutoSize = true;
            this._lDatabaseName1.Enabled = false;
            this._lDatabaseName1.Location = new System.Drawing.Point(8, 32);
            this._lDatabaseName1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDatabaseName1.Name = "_lDatabaseName1";
            this._lDatabaseName1.Size = new System.Drawing.Size(112, 17);
            this._lDatabaseName1.TabIndex = 23;
            this._lDatabaseName1.Text = "Database name:";
            // 
            // _gbExternalDatabase
            // 
            this._gbExternalDatabase.Controls.Add(this._bBrowse1);
            this._gbExternalDatabase.Controls.Add(this._lDatabaseName1);
            this._gbExternalDatabase.Controls.Add(this._lPath1);
            this._gbExternalDatabase.Controls.Add(this._eExternDatabasePath);
            this._gbExternalDatabase.Controls.Add(this._eExternDatabase);
            this._gbExternalDatabase.Location = new System.Drawing.Point(15, 161);
            this._gbExternalDatabase.Margin = new System.Windows.Forms.Padding(4);
            this._gbExternalDatabase.Name = "_gbExternalDatabase";
            this._gbExternalDatabase.Padding = new System.Windows.Forms.Padding(4);
            this._gbExternalDatabase.Size = new System.Drawing.Size(484, 134);
            this._gbExternalDatabase.TabIndex = 25;
            this._gbExternalDatabase.TabStop = false;
            this._gbExternalDatabase.Text = "External database";
            // 
            // FormDatabaseSettings
            // 
            this.AcceptButton = this._bNext;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(514, 381);
            this.ControlBox = false;
            this.Controls.Add(this._gbExternalDatabase);
            this.Controls.Add(this._bBrowse);
            this.Controls.Add(this._lPath);
            this.Controls.Add(this._eDatabasePath);
            this.Controls.Add(this._eDatabase);
            this.Controls.Add(this._eSqlServer);
            this.Controls.Add(this._lDatabaseName);
            this.Controls.Add(this._lSqlServer);
            this.Controls.Add(this._lMainInfo);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormDatabaseSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Server settings";
            this._gbExternalDatabase.ResumeLayout(false);
            this._gbExternalDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lMainInfo;
        private System.Windows.Forms.Label _lSqlServer;
        private System.Windows.Forms.Label _lDatabaseName;
        private System.Windows.Forms.TextBox _eSqlServer;
        private System.Windows.Forms.TextBox _eDatabase;
        private System.Windows.Forms.TextBox _eDatabasePath;
        private System.Windows.Forms.Label _lPath;
        private System.Windows.Forms.Button _bBrowse;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowser;
        private System.Windows.Forms.Button _bBrowse1;
        private System.Windows.Forms.Label _lPath1;
        private System.Windows.Forms.TextBox _eExternDatabasePath;
        private System.Windows.Forms.TextBox _eExternDatabase;
        private System.Windows.Forms.Label _lDatabaseName1;
        private System.Windows.Forms.GroupBox _gbExternalDatabase;
    }
}