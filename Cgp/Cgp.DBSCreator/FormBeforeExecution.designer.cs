namespace Contal.Cgp.DBSCreator
{
    partial class FormBeforeExecution
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
            this._eSqlServer = new System.Windows.Forms.TextBox();
            this._lDatabase = new System.Windows.Forms.Label();
            this._eDatabase = new System.Windows.Forms.TextBox();
            this._lLogin = new System.Windows.Forms.Label();
            this._tbLogin = new System.Windows.Forms.TextBox();
            this._bBack = new System.Windows.Forms.Button();
            this._bPerform = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lProgress = new System.Windows.Forms.Label();
            this._rbProgress = new System.Windows.Forms.RichTextBox();
            this._pbWait = new System.Windows.Forms.ProgressBar();
            this._gbMainDatabase = new System.Windows.Forms.GroupBox();
            this._gbExternalDatabase = new System.Windows.Forms.GroupBox();
            this._lDatabase1 = new System.Windows.Forms.Label();
            this._eExternDatabase = new System.Windows.Forms.TextBox();
            this._gbMainDatabase.SuspendLayout();
            this._gbExternalDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lSqlServer
            // 
            this._lSqlServer.AutoSize = true;
            this._lSqlServer.Enabled = false;
            this._lSqlServer.Location = new System.Drawing.Point(8, 34);
            this._lSqlServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lSqlServer.Name = "_lSqlServer";
            this._lSqlServer.Size = new System.Drawing.Size(86, 17);
            this._lSqlServer.TabIndex = 1;
            this._lSqlServer.Text = "SQL Server:";
            // 
            // _eSqlServer
            // 
            this._eSqlServer.Enabled = false;
            this._eSqlServer.Location = new System.Drawing.Point(172, 30);
            this._eSqlServer.Margin = new System.Windows.Forms.Padding(4);
            this._eSqlServer.Name = "_eSqlServer";
            this._eSqlServer.Size = new System.Drawing.Size(308, 22);
            this._eSqlServer.TabIndex = 2;
            // 
            // _lDatabase
            // 
            this._lDatabase.AutoSize = true;
            this._lDatabase.Enabled = false;
            this._lDatabase.Location = new System.Drawing.Point(8, 66);
            this._lDatabase.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDatabase.Name = "_lDatabase";
            this._lDatabase.Size = new System.Drawing.Size(73, 17);
            this._lDatabase.TabIndex = 3;
            this._lDatabase.Text = "Database:";
            // 
            // _eDatabase
            // 
            this._eDatabase.Enabled = false;
            this._eDatabase.Location = new System.Drawing.Point(172, 62);
            this._eDatabase.Margin = new System.Windows.Forms.Padding(4);
            this._eDatabase.Name = "_eDatabase";
            this._eDatabase.Size = new System.Drawing.Size(308, 22);
            this._eDatabase.TabIndex = 3;
            // 
            // _lLogin
            // 
            this._lLogin.AutoSize = true;
            this._lLogin.Enabled = false;
            this._lLogin.Location = new System.Drawing.Point(14, 192);
            this._lLogin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lLogin.Name = "_lLogin";
            this._lLogin.Size = new System.Drawing.Size(47, 17);
            this._lLogin.TabIndex = 6;
            this._lLogin.Text = "Login:";
            // 
            // _tbLogin
            // 
            this._tbLogin.Enabled = false;
            this._tbLogin.Location = new System.Drawing.Point(189, 189);
            this._tbLogin.Margin = new System.Windows.Forms.Padding(4);
            this._tbLogin.Name = "_tbLogin";
            this._tbLogin.Size = new System.Drawing.Size(315, 22);
            this._tbLogin.TabIndex = 4;
            // 
            // _bBack
            // 
            this._bBack.Location = new System.Drawing.Point(182, 391);
            this._bBack.Margin = new System.Windows.Forms.Padding(4);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(94, 29);
            this._bBack.TabIndex = 1;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _bPerform
            // 
            this._bPerform.Location = new System.Drawing.Point(284, 391);
            this._bPerform.Margin = new System.Windows.Forms.Padding(4);
            this._bPerform.Name = "_bPerform";
            this._bPerform.Size = new System.Drawing.Size(94, 29);
            this._bPerform.TabIndex = 0;
            this._bPerform.Text = "Perform";
            this._bPerform.UseVisualStyleBackColor = true;
            this._bPerform.Click += new System.EventHandler(this._bPerform_Click);
            // 
            // _bCancel
            // 
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(411, 391);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 29);
            this._bCancel.TabIndex = 6;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lProgress
            // 
            this._lProgress.AutoSize = true;
            this._lProgress.Enabled = false;
            this._lProgress.Location = new System.Drawing.Point(12, 225);
            this._lProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lProgress.Name = "_lProgress";
            this._lProgress.Size = new System.Drawing.Size(69, 17);
            this._lProgress.TabIndex = 13;
            this._lProgress.Text = "Progress:";
            // 
            // _rbProgress
            // 
            this._rbProgress.Location = new System.Drawing.Point(189, 221);
            this._rbProgress.Margin = new System.Windows.Forms.Padding(4);
            this._rbProgress.Name = "_rbProgress";
            this._rbProgress.ReadOnly = true;
            this._rbProgress.Size = new System.Drawing.Size(315, 156);
            this._rbProgress.TabIndex = 5;
            this._rbProgress.Text = "";
            // 
            // _pbWait
            // 
            this._pbWait.Location = new System.Drawing.Point(189, 365);
            this._pbWait.Margin = new System.Windows.Forms.Padding(4);
            this._pbWait.Name = "_pbWait";
            this._pbWait.Size = new System.Drawing.Size(316, 14);
            this._pbWait.TabIndex = 28;
            this._pbWait.Visible = false;
            // 
            // _gbMainDatabase
            // 
            this._gbMainDatabase.Controls.Add(this._eSqlServer);
            this._gbMainDatabase.Controls.Add(this._lSqlServer);
            this._gbMainDatabase.Controls.Add(this._lDatabase);
            this._gbMainDatabase.Controls.Add(this._eDatabase);
            this._gbMainDatabase.Location = new System.Drawing.Point(16, 15);
            this._gbMainDatabase.Margin = new System.Windows.Forms.Padding(4);
            this._gbMainDatabase.Name = "_gbMainDatabase";
            this._gbMainDatabase.Padding = new System.Windows.Forms.Padding(4);
            this._gbMainDatabase.Size = new System.Drawing.Size(489, 98);
            this._gbMainDatabase.TabIndex = 29;
            this._gbMainDatabase.TabStop = false;
            this._gbMainDatabase.Text = "Main database";
            // 
            // _gbExternalDatabase
            // 
            this._gbExternalDatabase.Controls.Add(this._lDatabase1);
            this._gbExternalDatabase.Controls.Add(this._eExternDatabase);
            this._gbExternalDatabase.Location = new System.Drawing.Point(16, 120);
            this._gbExternalDatabase.Margin = new System.Windows.Forms.Padding(4);
            this._gbExternalDatabase.Name = "_gbExternalDatabase";
            this._gbExternalDatabase.Padding = new System.Windows.Forms.Padding(4);
            this._gbExternalDatabase.Size = new System.Drawing.Size(489, 61);
            this._gbExternalDatabase.TabIndex = 30;
            this._gbExternalDatabase.TabStop = false;
            this._gbExternalDatabase.Text = "External database";
            // 
            // _lDatabase1
            // 
            this._lDatabase1.AutoSize = true;
            this._lDatabase1.Enabled = false;
            this._lDatabase1.Location = new System.Drawing.Point(8, 28);
            this._lDatabase1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDatabase1.Name = "_lDatabase1";
            this._lDatabase1.Size = new System.Drawing.Size(73, 17);
            this._lDatabase1.TabIndex = 3;
            this._lDatabase1.Text = "Database:";
            // 
            // _eExternDatabase
            // 
            this._eExternDatabase.Enabled = false;
            this._eExternDatabase.Location = new System.Drawing.Point(172, 24);
            this._eExternDatabase.Margin = new System.Windows.Forms.Padding(4);
            this._eExternDatabase.Name = "_eExternDatabase";
            this._eExternDatabase.Size = new System.Drawing.Size(308, 22);
            this._eExternDatabase.TabIndex = 3;
            // 
            // FormBeforeExecution
            // 
            this.AcceptButton = this._bPerform;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(520, 435);
            this.ControlBox = false;
            this.Controls.Add(this._gbExternalDatabase);
            this.Controls.Add(this._gbMainDatabase);
            this.Controls.Add(this._pbWait);
            this.Controls.Add(this._rbProgress);
            this.Controls.Add(this._lProgress);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._bPerform);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._tbLogin);
            this.Controls.Add(this._lLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormBeforeExecution";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create database";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormBeforeExecution_FormClosed);
            this._gbMainDatabase.ResumeLayout(false);
            this._gbMainDatabase.PerformLayout();
            this._gbExternalDatabase.ResumeLayout(false);
            this._gbExternalDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lSqlServer;
        private System.Windows.Forms.TextBox _eSqlServer;
        private System.Windows.Forms.Label _lDatabase;
        private System.Windows.Forms.TextBox _eDatabase;
        private System.Windows.Forms.Label _lLogin;
        private System.Windows.Forms.TextBox _tbLogin;
        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Button _bPerform;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lProgress;
        private System.Windows.Forms.RichTextBox _rbProgress;
        private System.Windows.Forms.ProgressBar _pbWait;
        private System.Windows.Forms.GroupBox _gbMainDatabase;
        private System.Windows.Forms.GroupBox _gbExternalDatabase;
        private System.Windows.Forms.Label _lDatabase1;
        private System.Windows.Forms.TextBox _eExternDatabase;
    }
}