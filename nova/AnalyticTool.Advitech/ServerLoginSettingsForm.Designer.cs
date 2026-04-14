namespace AnalyticTool.Advitech
{
    partial class ServerLoginSettingsForm
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
            this._lServerPort = new System.Windows.Forms.Label();
            this._lPassword = new System.Windows.Forms.Label();
            this._lServerIp = new System.Windows.Forms.Label();
            this._lUserName = new System.Windows.Forms.Label();
            this._tbServerIp = new System.Windows.Forms.TextBox();
            this._tbUserName = new System.Windows.Forms.TextBox();
            this._tbPassword = new System.Windows.Forms.TextBox();
            this._bNext = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._ePort = new System.Windows.Forms.NumericUpDown();
            this._lInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._ePort)).BeginInit();
            this.SuspendLayout();
            // 
            // _lServerPort
            // 
            this._lServerPort.AutoSize = true;
            this._lServerPort.Location = new System.Drawing.Point(206, 9);
            this._lServerPort.Name = "_lServerPort";
            this._lServerPort.Size = new System.Drawing.Size(59, 13);
            this._lServerPort.TabIndex = 11;
            this._lServerPort.Text = "Server port";
            // 
            // _lPassword
            // 
            this._lPassword.AutoSize = true;
            this._lPassword.Location = new System.Drawing.Point(12, 97);
            this._lPassword.Name = "_lPassword";
            this._lPassword.Size = new System.Drawing.Size(53, 13);
            this._lPassword.TabIndex = 16;
            this._lPassword.Text = "Password";
            // 
            // _lServerIp
            // 
            this._lServerIp.AutoSize = true;
            this._lServerIp.Location = new System.Drawing.Point(12, 9);
            this._lServerIp.Name = "_lServerIp";
            this._lServerIp.Size = new System.Drawing.Size(51, 13);
            this._lServerIp.TabIndex = 9;
            this._lServerIp.Text = "Server IP";
            // 
            // _lUserName
            // 
            this._lUserName.AutoSize = true;
            this._lUserName.Location = new System.Drawing.Point(12, 57);
            this._lUserName.Name = "_lUserName";
            this._lUserName.Size = new System.Drawing.Size(58, 13);
            this._lUserName.TabIndex = 15;
            this._lUserName.Text = "User name";
            // 
            // _tbServerIp
            // 
            this._tbServerIp.Location = new System.Drawing.Point(15, 26);
            this._tbServerIp.Name = "_tbServerIp";
            this._tbServerIp.Size = new System.Drawing.Size(176, 20);
            this._tbServerIp.TabIndex = 1;
            this._tbServerIp.Text = "127.0.0.1";
            this._tbServerIp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._tbServerIp_KeyPress);
            // 
            // _tbUserName
            // 
            this._tbUserName.Location = new System.Drawing.Point(15, 74);
            this._tbUserName.Name = "_tbUserName";
            this._tbUserName.Size = new System.Drawing.Size(176, 20);
            this._tbUserName.TabIndex = 3;
            // 
            // _tbPassword
            // 
            this._tbPassword.Location = new System.Drawing.Point(15, 114);
            this._tbPassword.Name = "_tbPassword";
            this._tbPassword.Size = new System.Drawing.Size(176, 20);
            this._tbPassword.TabIndex = 4;
            this._tbPassword.UseSystemPasswordChar = true;
            // 
            // _bNext
            // 
            this._bNext.Location = new System.Drawing.Point(209, 164);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(95, 26);
            this._bNext.TabIndex = 5;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(314, 164);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(95, 26);
            this._bCancel.TabIndex = 6;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _ePort
            // 
            this._ePort.Location = new System.Drawing.Point(209, 26);
            this._ePort.Margin = new System.Windows.Forms.Padding(2);
            this._ePort.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this._ePort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._ePort.Name = "_ePort";
            this._ePort.Size = new System.Drawing.Size(109, 20);
            this._ePort.TabIndex = 2;
            this._ePort.Value = new decimal(new int[] {
            54001,
            0,
            0,
            0});
            // 
            // _lInfo
            // 
            this._lInfo.AutoSize = true;
            this._lInfo.ForeColor = System.Drawing.Color.Red;
            this._lInfo.Location = new System.Drawing.Point(12, 137);
            this._lInfo.Name = "_lInfo";
            this._lInfo.Size = new System.Drawing.Size(196, 13);
            this._lInfo.TabIndex = 17;
            this._lInfo.Text = "Contal Nova Server 2.1 must be running";
            // 
            // ServerLoginSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 202);
            this.Controls.Add(this._lInfo);
            this.Controls.Add(this._ePort);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._lServerPort);
            this.Controls.Add(this._lPassword);
            this.Controls.Add(this._lServerIp);
            this.Controls.Add(this._lUserName);
            this.Controls.Add(this._tbServerIp);
            this.Controls.Add(this._tbUserName);
            this.Controls.Add(this._tbPassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ServerLoginSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server login settings";
            ((System.ComponentModel.ISupportInitialize)(this._ePort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lServerPort;
        private System.Windows.Forms.Label _lPassword;
        private System.Windows.Forms.Label _lServerIp;
        private System.Windows.Forms.Label _lUserName;
        private System.Windows.Forms.TextBox _tbServerIp;
        private System.Windows.Forms.TextBox _tbUserName;
        private System.Windows.Forms.TextBox _tbPassword;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.NumericUpDown _ePort;
        private System.Windows.Forms.Label _lInfo;
    }
}