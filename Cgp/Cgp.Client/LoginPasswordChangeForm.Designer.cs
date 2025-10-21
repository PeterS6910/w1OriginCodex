namespace Contal.Cgp.Client
{
    partial class LoginPasswordChangeForm
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
            this._lLogin = new System.Windows.Forms.Label();
            this._lOldPassword = new System.Windows.Forms.Label();
            this._lNewPassword = new System.Windows.Forms.Label();
            this._lConfirmPassword = new System.Windows.Forms.Label();
            this._eLogin = new System.Windows.Forms.TextBox();
            this._eOldPassword = new System.Windows.Forms.TextBox();
            this._eNewPassword = new System.Windows.Forms.TextBox();
            this._eConfirmPassword = new System.Windows.Forms.TextBox();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lLogin
            // 
            this._lLogin.AutoSize = true;
            this._lLogin.Location = new System.Drawing.Point(6, 26);
            this._lLogin.Name = "_lLogin";
            this._lLogin.Size = new System.Drawing.Size(60, 13);
            this._lLogin.TabIndex = 0;
            this._lLogin.Text = "User Name";
            // 
            // _lOldPassword
            // 
            this._lOldPassword.AutoSize = true;
            this._lOldPassword.Location = new System.Drawing.Point(6, 48);
            this._lOldPassword.Name = "_lOldPassword";
            this._lOldPassword.Size = new System.Drawing.Size(72, 13);
            this._lOldPassword.TabIndex = 1;
            this._lOldPassword.Text = "Old Password";
            // 
            // _lNewPassword
            // 
            this._lNewPassword.AutoSize = true;
            this._lNewPassword.Location = new System.Drawing.Point(6, 83);
            this._lNewPassword.Name = "_lNewPassword";
            this._lNewPassword.Size = new System.Drawing.Size(53, 13);
            this._lNewPassword.TabIndex = 2;
            this._lNewPassword.Text = "Password";
            // 
            // _lConfirmPassword
            // 
            this._lConfirmPassword.AutoSize = true;
            this._lConfirmPassword.Location = new System.Drawing.Point(6, 109);
            this._lConfirmPassword.Name = "_lConfirmPassword";
            this._lConfirmPassword.Size = new System.Drawing.Size(91, 13);
            this._lConfirmPassword.TabIndex = 3;
            this._lConfirmPassword.Text = "Confirm Password";
            // 
            // _eLogin
            // 
            this._eLogin.Enabled = false;
            this._eLogin.Location = new System.Drawing.Point(114, 19);
            this._eLogin.Name = "_eLogin";
            this._eLogin.Size = new System.Drawing.Size(158, 20);
            this._eLogin.TabIndex = 0;
            // 
            // _eOldPassword
            // 
            this._eOldPassword.Location = new System.Drawing.Point(114, 45);
            this._eOldPassword.Name = "_eOldPassword";
            this._eOldPassword.Size = new System.Drawing.Size(158, 20);
            this._eOldPassword.TabIndex = 1;
            this._eOldPassword.UseSystemPasswordChar = true;
            // 
            // _eNewPassword
            // 
            this._eNewPassword.Location = new System.Drawing.Point(114, 80);
            this._eNewPassword.Name = "_eNewPassword";
            this._eNewPassword.Size = new System.Drawing.Size(158, 20);
            this._eNewPassword.TabIndex = 2;
            this._eNewPassword.UseSystemPasswordChar = true;
            // 
            // _eConfirmPassword
            // 
            this._eConfirmPassword.Location = new System.Drawing.Point(114, 106);
            this._eConfirmPassword.Name = "_eConfirmPassword";
            this._eConfirmPassword.Size = new System.Drawing.Size(158, 20);
            this._eConfirmPassword.TabIndex = 3;
            this._eConfirmPassword.UseSystemPasswordChar = true;
            // 
            // _bOk
            // 
            this._bOk.Location = new System.Drawing.Point(150, 166);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(231, 166);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._eLogin);
            this.groupBox1.Controls.Add(this._lLogin);
            this.groupBox1.Controls.Add(this._eNewPassword);
            this.groupBox1.Controls.Add(this._lConfirmPassword);
            this.groupBox1.Controls.Add(this._lNewPassword);
            this.groupBox1.Controls.Add(this._eConfirmPassword);
            this.groupBox1.Controls.Add(this._lOldPassword);
            this.groupBox1.Controls.Add(this._eOldPassword);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 139);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // LoginPasswordChangeForm
            // 
            this.AcceptButton = this._bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(318, 227);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LoginPasswordChangeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LoginPasswordChangeForm";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _lLogin;
        private System.Windows.Forms.Label _lOldPassword;
        private System.Windows.Forms.Label _lNewPassword;
        private System.Windows.Forms.Label _lConfirmPassword;
        private System.Windows.Forms.TextBox _eLogin;
        private System.Windows.Forms.TextBox _eOldPassword;
        private System.Windows.Forms.TextBox _eNewPassword;
        private System.Windows.Forms.TextBox _eConfirmPassword;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}