namespace AnalyticTool.Advitech
{
    partial class DatabaseServerSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseServerSettingsForm));
            this._lDbPassword = new System.Windows.Forms.Label();
            this._lLoginName = new System.Windows.Forms.Label();
            this._tbLoginName = new System.Windows.Forms.TextBox();
            this._tbDbPassword = new System.Windows.Forms.TextBox();
            this._cbSqlServers = new System.Windows.Forms.ComboBox();
            this._lSqlServer = new System.Windows.Forms.Label();
            this._bBack = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._bObtainDatabases = new System.Windows.Forms.Button();
            this._gbLoginForDatabase = new System.Windows.Forms.GroupBox();
            this._rbUseCGPLogin = new System.Windows.Forms.RadioButton();
            this._rbUseCustomLogin = new System.Windows.Forms.RadioButton();
            this._gbLoginForDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lDbPassword
            // 
            this._lDbPassword.AutoSize = true;
            this._lDbPassword.Location = new System.Drawing.Point(7, 101);
            this._lDbPassword.Name = "_lDbPassword";
            this._lDbPassword.Size = new System.Drawing.Size(53, 13);
            this._lDbPassword.TabIndex = 15;
            this._lDbPassword.Text = "Password";
            // 
            // _lLoginName
            // 
            this._lLoginName.AutoSize = true;
            this._lLoginName.Location = new System.Drawing.Point(7, 59);
            this._lLoginName.Name = "_lLoginName";
            this._lLoginName.Size = new System.Drawing.Size(126, 13);
            this._lLoginName.TabIndex = 14;
            this._lLoginName.Text = "System admin login name";
            // 
            // _tbLoginName
            // 
            this._tbLoginName.Location = new System.Drawing.Point(10, 77);
            this._tbLoginName.Name = "_tbLoginName";
            this._tbLoginName.Size = new System.Drawing.Size(176, 20);
            this._tbLoginName.TabIndex = 2;
            // 
            // _tbDbPassword
            // 
            this._tbDbPassword.Location = new System.Drawing.Point(10, 119);
            this._tbDbPassword.Name = "_tbDbPassword";
            this._tbDbPassword.Size = new System.Drawing.Size(176, 20);
            this._tbDbPassword.TabIndex = 3;
            this._tbDbPassword.UseSystemPasswordChar = true;
            // 
            // _cbSqlServers
            // 
            this._cbSqlServers.FormattingEnabled = true;
            this._cbSqlServers.Location = new System.Drawing.Point(10, 34);
            this._cbSqlServers.Name = "_cbSqlServers";
            this._cbSqlServers.Size = new System.Drawing.Size(273, 21);
            this._cbSqlServers.TabIndex = 1;
            // 
            // _lSqlServer
            // 
            this._lSqlServer.AutoSize = true;
            this._lSqlServer.Location = new System.Drawing.Point(7, 15);
            this._lSqlServer.Name = "_lSqlServer";
            this._lSqlServer.Size = new System.Drawing.Size(124, 13);
            this._lSqlServer.TabIndex = 10;
            this._lSqlServer.Text = "SQL Server for database";
            // 
            // _bBack
            // 
            this._bBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bBack.Location = new System.Drawing.Point(263, 281);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(95, 26);
            this._bBack.TabIndex = 7;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _bNext
            // 
            this._bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bNext.Location = new System.Drawing.Point(364, 281);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(95, 26);
            this._bNext.TabIndex = 6;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bObtainDatabases
            // 
            this._bObtainDatabases.Image = ((System.Drawing.Image)(resources.GetObject("_bObtainDatabases.Image")));
            this._bObtainDatabases.Location = new System.Drawing.Point(414, 34);
            this._bObtainDatabases.Margin = new System.Windows.Forms.Padding(4);
            this._bObtainDatabases.Name = "_bObtainDatabases";
            this._bObtainDatabases.Size = new System.Drawing.Size(46, 46);
            this._bObtainDatabases.TabIndex = 21;
            this._bObtainDatabases.UseVisualStyleBackColor = true;
            this._bObtainDatabases.Click += new System.EventHandler(this._bObtainDatabases_Click);
            // 
            // _gbLoginForDatabase
            // 
            this._gbLoginForDatabase.Controls.Add(this._rbUseCGPLogin);
            this._gbLoginForDatabase.Controls.Add(this._rbUseCustomLogin);
            this._gbLoginForDatabase.Location = new System.Drawing.Point(10, 162);
            this._gbLoginForDatabase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._gbLoginForDatabase.Name = "_gbLoginForDatabase";
            this._gbLoginForDatabase.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._gbLoginForDatabase.Size = new System.Drawing.Size(450, 98);
            this._gbLoginForDatabase.TabIndex = 22;
            this._gbLoginForDatabase.TabStop = false;
            this._gbLoginForDatabase.Text = "Login for database";
            // 
            // _rbUseCGPLogin
            // 
            this._rbUseCGPLogin.AutoSize = true;
            this._rbUseCGPLogin.Checked = true;
            this._rbUseCGPLogin.Location = new System.Drawing.Point(9, 30);
            this._rbUseCGPLogin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._rbUseCGPLogin.Name = "_rbUseCGPLogin";
            this._rbUseCGPLogin.Size = new System.Drawing.Size(234, 17);
            this._rbUseCGPLogin.TabIndex = 4;
            this._rbUseCGPLogin.TabStop = true;
            this._rbUseCGPLogin.Text = "Standard database login for CONTAL NOVA";
            this._rbUseCGPLogin.UseVisualStyleBackColor = true;
            // 
            // _rbUseCustomLogin
            // 
            this._rbUseCustomLogin.AutoSize = true;
            this._rbUseCustomLogin.Location = new System.Drawing.Point(9, 65);
            this._rbUseCustomLogin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._rbUseCustomLogin.Name = "_rbUseCustomLogin";
            this._rbUseCustomLogin.Size = new System.Drawing.Size(132, 17);
            this._rbUseCustomLogin.TabIndex = 5;
            this._rbUseCustomLogin.Text = "Custom database login";
            this._rbUseCustomLogin.UseVisualStyleBackColor = true;
            // 
            // DatabaseServerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 319);
            this.Controls.Add(this._gbLoginForDatabase);
            this.Controls.Add(this._bObtainDatabases);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._lDbPassword);
            this.Controls.Add(this._lLoginName);
            this.Controls.Add(this._tbLoginName);
            this.Controls.Add(this._tbDbPassword);
            this.Controls.Add(this._cbSqlServers);
            this.Controls.Add(this._lSqlServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DatabaseServerSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database server settings";
            this._gbLoginForDatabase.ResumeLayout(false);
            this._gbLoginForDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lDbPassword;
        private System.Windows.Forms.Label _lLoginName;
        private System.Windows.Forms.TextBox _tbLoginName;
        private System.Windows.Forms.TextBox _tbDbPassword;
        private System.Windows.Forms.ComboBox _cbSqlServers;
        private System.Windows.Forms.Label _lSqlServer;
        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bObtainDatabases;
        private System.Windows.Forms.GroupBox _gbLoginForDatabase;
        private System.Windows.Forms.RadioButton _rbUseCGPLogin;
        private System.Windows.Forms.RadioButton _rbUseCustomLogin;
    }
}