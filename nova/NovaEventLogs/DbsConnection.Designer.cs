namespace NovaEventLogs
{
    partial class DbsConnection
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
            this._gbConnectionSettings = new System.Windows.Forms.GroupBox();
            this._ePassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._eUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._eDataSoucerDbLog = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._eDataSoucerDb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._eServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._Ok = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._gbConnectionSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // _gbConnectionSettings
            // 
            this._gbConnectionSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbConnectionSettings.Controls.Add(this._ePassword);
            this._gbConnectionSettings.Controls.Add(this.label5);
            this._gbConnectionSettings.Controls.Add(this._eUser);
            this._gbConnectionSettings.Controls.Add(this.label4);
            this._gbConnectionSettings.Controls.Add(this._eDataSoucerDbLog);
            this._gbConnectionSettings.Controls.Add(this.label3);
            this._gbConnectionSettings.Controls.Add(this._eDataSoucerDb);
            this._gbConnectionSettings.Controls.Add(this.label2);
            this._gbConnectionSettings.Controls.Add(this._eServer);
            this._gbConnectionSettings.Controls.Add(this.label1);
            this._gbConnectionSettings.Location = new System.Drawing.Point(12, 12);
            this._gbConnectionSettings.Name = "_gbConnectionSettings";
            this._gbConnectionSettings.Size = new System.Drawing.Size(357, 225);
            this._gbConnectionSettings.TabIndex = 0;
            this._gbConnectionSettings.TabStop = false;
            this._gbConnectionSettings.Text = "Connection settings";
            // 
            // _ePassword
            // 
            this._ePassword.Location = new System.Drawing.Point(9, 188);
            this._ePassword.Name = "_ePassword";
            this._ePassword.Size = new System.Drawing.Size(243, 20);
            this._ePassword.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "password";
            // 
            // _eUser
            // 
            this._eUser.Location = new System.Drawing.Point(6, 149);
            this._eUser.Name = "_eUser";
            this._eUser.Size = new System.Drawing.Size(246, 20);
            this._eUser.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "user name";
            // 
            // _eDataSoucerDbLog
            // 
            this._eDataSoucerDbLog.Location = new System.Drawing.Point(6, 110);
            this._eDataSoucerDbLog.Name = "_eDataSoucerDbLog";
            this._eDataSoucerDbLog.Size = new System.Drawing.Size(246, 20);
            this._eDataSoucerDbLog.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Database eventlogs";
            // 
            // _eDataSoucerDb
            // 
            this._eDataSoucerDb.Location = new System.Drawing.Point(6, 71);
            this._eDataSoucerDb.Name = "_eDataSoucerDb";
            this._eDataSoucerDb.Size = new System.Drawing.Size(246, 20);
            this._eDataSoucerDb.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Database";
            // 
            // _eServer
            // 
            this._eServer.Location = new System.Drawing.Point(6, 32);
            this._eServer.Name = "_eServer";
            this._eServer.Size = new System.Drawing.Size(246, 20);
            this._eServer.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // _Ok
            // 
            this._Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._Ok.Location = new System.Drawing.Point(213, 243);
            this._Ok.Name = "_Ok";
            this._Ok.Size = new System.Drawing.Size(75, 23);
            this._Ok.TabIndex = 1;
            this._Ok.Text = "Ok";
            this._Ok.UseVisualStyleBackColor = true;
            this._Ok.Click += new System.EventHandler(this._Ok_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(294, 243);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // DbsConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 278);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._Ok);
            this.Controls.Add(this._gbConnectionSettings);
            this.Name = "DbsConnection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DbsConnection";
            this._gbConnectionSettings.ResumeLayout(false);
            this._gbConnectionSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _gbConnectionSettings;
        private System.Windows.Forms.TextBox _eUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _eDataSoucerDbLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _eDataSoucerDb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _eServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _Ok;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.TextBox _ePassword;
        private System.Windows.Forms.Label label5;
    }
}