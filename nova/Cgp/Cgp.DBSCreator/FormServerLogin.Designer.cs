namespace Contal.Cgp.DBSCreator
{
    partial class FormServerLogin
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
            this._lLoginName = new System.Windows.Forms.Label();
            this._lPassword = new System.Windows.Forms.Label();
            this._eSqlServer = new System.Windows.Forms.TextBox();
            this._eName = new System.Windows.Forms.TextBox();
            this._ePassword = new System.Windows.Forms.TextBox();
            this._bBack = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lMainInfo = new System.Windows.Forms.Label();
            this._rbUseCGPLogin = new System.Windows.Forms.RadioButton();
            this._rbUseCustomLogin = new System.Windows.Forms.RadioButton();
            this._rbUseSaLogin = new System.Windows.Forms.RadioButton();
            this._gbLoginForDatabase = new System.Windows.Forms.GroupBox();
            this._gbLoginForDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lSqlServer
            // 
            this._lSqlServer.AutoSize = true;
            this._lSqlServer.Enabled = false;
            this._lSqlServer.Location = new System.Drawing.Point(17, 51);
            this._lSqlServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lSqlServer.Name = "_lSqlServer";
            this._lSqlServer.Size = new System.Drawing.Size(65, 13);
            this._lSqlServer.TabIndex = 0;
            this._lSqlServer.Text = "SQL Server:";
            // 
            // _lLoginName
            // 
            this._lLoginName.AutoSize = true;
            this._lLoginName.Enabled = false;
            this._lLoginName.Location = new System.Drawing.Point(17, 86);
            this._lLoginName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lLoginName.Name = "_lLoginName";
            this._lLoginName.Size = new System.Drawing.Size(65, 13);
            this._lLoginName.TabIndex = 0;
            this._lLoginName.Text = "Login name:";
            // 
            // _lPassword
            // 
            this._lPassword.AutoSize = true;
            this._lPassword.Enabled = false;
            this._lPassword.Location = new System.Drawing.Point(17, 126);
            this._lPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPassword.Name = "_lPassword";
            this._lPassword.Size = new System.Drawing.Size(56, 13);
            this._lPassword.TabIndex = 0;
            this._lPassword.Text = "Password:";
            // 
            // _eSqlServer
            // 
            this._eSqlServer.Enabled = false;
            this._eSqlServer.Location = new System.Drawing.Point(223, 40);
            this._eSqlServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._eSqlServer.Name = "_eSqlServer";
            this._eSqlServer.Size = new System.Drawing.Size(256, 20);
            this._eSqlServer.TabIndex = 1;
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(223, 81);
            this._eName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(256, 20);
            this._eName.TabIndex = 2;
            // 
            // _ePassword
            // 
            this._ePassword.Location = new System.Drawing.Point(223, 122);
            this._ePassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._ePassword.Name = "_ePassword";
            this._ePassword.PasswordChar = '●';
            this._ePassword.Size = new System.Drawing.Size(256, 20);
            this._ePassword.TabIndex = 3;
            // 
            // _bBack
            // 
            this._bBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bBack.Location = new System.Drawing.Point(197, 314);
            this._bBack.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(106, 36);
            this._bBack.TabIndex = 7;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _bNext
            // 
            this._bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bNext.Location = new System.Drawing.Point(310, 314);
            this._bNext.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(106, 36);
            this._bNext.TabIndex = 8;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(454, 314);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(106, 36);
            this._bCancel.TabIndex = 9;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lMainInfo
            // 
            this._lMainInfo.AutoSize = true;
            this._lMainInfo.Enabled = false;
            this._lMainInfo.Location = new System.Drawing.Point(17, 14);
            this._lMainInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMainInfo.Name = "_lMainInfo";
            this._lMainInfo.Size = new System.Drawing.Size(132, 13);
            this._lMainInfo.TabIndex = 0;
            this._lMainInfo.Text = "Settings for database login";
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
            // _rbUseSaLogin
            // 
            this._rbUseSaLogin.AutoSize = true;
            this._rbUseSaLogin.Location = new System.Drawing.Point(9, 101);
            this._rbUseSaLogin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._rbUseSaLogin.Name = "_rbUseSaLogin";
            this._rbUseSaLogin.Size = new System.Drawing.Size(158, 17);
            this._rbUseSaLogin.TabIndex = 6;
            this._rbUseSaLogin.Text = "Entered login for SQL server";
            this._rbUseSaLogin.UseVisualStyleBackColor = true;
            // 
            // _gbLoginForDatabase
            // 
            this._gbLoginForDatabase.Controls.Add(this._rbUseCGPLogin);
            this._gbLoginForDatabase.Controls.Add(this._rbUseSaLogin);
            this._gbLoginForDatabase.Controls.Add(this._rbUseCustomLogin);
            this._gbLoginForDatabase.Location = new System.Drawing.Point(17, 162);
            this._gbLoginForDatabase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._gbLoginForDatabase.Name = "_gbLoginForDatabase";
            this._gbLoginForDatabase.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._gbLoginForDatabase.Size = new System.Drawing.Size(542, 142);
            this._gbLoginForDatabase.TabIndex = 0;
            this._gbLoginForDatabase.TabStop = false;
            this._gbLoginForDatabase.Text = "Login for database";
            // 
            // FormServerLogin
            // 
            this.AcceptButton = this._bNext;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(576, 369);
            this.ControlBox = false;
            this.Controls.Add(this._gbLoginForDatabase);
            this.Controls.Add(this._lMainInfo);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._ePassword);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._eSqlServer);
            this.Controls.Add(this._lPassword);
            this.Controls.Add(this._lLoginName);
            this.Controls.Add(this._lSqlServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormServerLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Server settings";
            this.Load += new System.EventHandler(this.FormServerLogin_Load);
            this._gbLoginForDatabase.ResumeLayout(false);
            this._gbLoginForDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lSqlServer;
        private System.Windows.Forms.Label _lLoginName;
        private System.Windows.Forms.Label _lPassword;
        private System.Windows.Forms.TextBox _eSqlServer;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.TextBox _ePassword;
        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lMainInfo;
        private System.Windows.Forms.RadioButton _rbUseCGPLogin;
        private System.Windows.Forms.RadioButton _rbUseCustomLogin;
        private System.Windows.Forms.RadioButton _rbUseSaLogin;
        private System.Windows.Forms.GroupBox _gbLoginForDatabase;
    }
}