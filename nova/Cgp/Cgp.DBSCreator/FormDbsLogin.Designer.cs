namespace Contal.Cgp.DBSCreator
{
    internal partial class FormDbsLogin
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
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._lPassword = new System.Windows.Forms.Label();
            this._ePassword = new System.Windows.Forms.TextBox();
            this._gbDbsLogin = new System.Windows.Forms.GroupBox();
            this._rbCreateLogin = new System.Windows.Forms.RadioButton();
            this._rbExistLogin = new System.Windows.Forms.RadioButton();
            this._eConfirmPassword = new System.Windows.Forms.TextBox();
            this._lConfirmPassword = new System.Windows.Forms.Label();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._gbDbsLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(163, 52);
            this._eName.Margin = new System.Windows.Forms.Padding(4);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(415, 20);
            this._eName.TabIndex = 3;
            this._eName.Validating += new System.ComponentModel.CancelEventHandler(this._eName_Validating);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(8, 55);
            this._lName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 2;
            this._lName.Text = "Name";
            // 
            // _lPassword
            // 
            this._lPassword.AutoSize = true;
            this._lPassword.Location = new System.Drawing.Point(8, 87);
            this._lPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPassword.Name = "_lPassword";
            this._lPassword.Size = new System.Drawing.Size(53, 13);
            this._lPassword.TabIndex = 4;
            this._lPassword.Text = "Password";
            // 
            // _ePassword
            // 
            this._ePassword.Location = new System.Drawing.Point(163, 84);
            this._ePassword.Margin = new System.Windows.Forms.Padding(4);
            this._ePassword.Name = "_ePassword";
            this._ePassword.PasswordChar = '●';
            this._ePassword.Size = new System.Drawing.Size(415, 20);
            this._ePassword.TabIndex = 4;
            this._ePassword.Validating += new System.ComponentModel.CancelEventHandler(this._ePassword_Validating);
            // 
            // _gbDbsLogin
            // 
            this._gbDbsLogin.Controls.Add(this._rbCreateLogin);
            this._gbDbsLogin.Controls.Add(this._rbExistLogin);
            this._gbDbsLogin.Controls.Add(this._eConfirmPassword);
            this._gbDbsLogin.Controls.Add(this._lConfirmPassword);
            this._gbDbsLogin.Controls.Add(this._ePassword);
            this._gbDbsLogin.Controls.Add(this._lPassword);
            this._gbDbsLogin.Controls.Add(this._eName);
            this._gbDbsLogin.Controls.Add(this._lName);
            this._gbDbsLogin.Location = new System.Drawing.Point(16, 15);
            this._gbDbsLogin.Margin = new System.Windows.Forms.Padding(4);
            this._gbDbsLogin.Name = "_gbDbsLogin";
            this._gbDbsLogin.Padding = new System.Windows.Forms.Padding(4);
            this._gbDbsLogin.Size = new System.Drawing.Size(587, 150);
            this._gbDbsLogin.TabIndex = 0;
            this._gbDbsLogin.TabStop = false;
            this._gbDbsLogin.Text = "Database login";
            // 
            // _rbCreateLogin
            // 
            this._rbCreateLogin.AutoSize = true;
            this._rbCreateLogin.Location = new System.Drawing.Point(332, 23);
            this._rbCreateLogin.Margin = new System.Windows.Forms.Padding(4);
            this._rbCreateLogin.Name = "_rbCreateLogin";
            this._rbCreateLogin.Size = new System.Drawing.Size(104, 17);
            this._rbCreateLogin.TabIndex = 2;
            this._rbCreateLogin.Text = "Create new login";
            this._rbCreateLogin.UseVisualStyleBackColor = true;
            // 
            // _rbExistLogin
            // 
            this._rbExistLogin.AutoSize = true;
            this._rbExistLogin.Checked = true;
            this._rbExistLogin.Location = new System.Drawing.Point(8, 23);
            this._rbExistLogin.Margin = new System.Windows.Forms.Padding(4);
            this._rbExistLogin.Name = "_rbExistLogin";
            this._rbExistLogin.Size = new System.Drawing.Size(93, 17);
            this._rbExistLogin.TabIndex = 1;
            this._rbExistLogin.TabStop = true;
            this._rbExistLogin.Text = "Use exist login";
            this._rbExistLogin.UseVisualStyleBackColor = true;
            this._rbExistLogin.CheckedChanged += new System.EventHandler(this._rbExistLogin_CheckedChanged);
            // 
            // _eConfirmPassword
            // 
            this._eConfirmPassword.Enabled = false;
            this._eConfirmPassword.Location = new System.Drawing.Point(163, 116);
            this._eConfirmPassword.Margin = new System.Windows.Forms.Padding(4);
            this._eConfirmPassword.Name = "_eConfirmPassword";
            this._eConfirmPassword.PasswordChar = '●';
            this._eConfirmPassword.Size = new System.Drawing.Size(415, 20);
            this._eConfirmPassword.TabIndex = 5;
            this._eConfirmPassword.Validating += new System.ComponentModel.CancelEventHandler(this._eConfirmPassword_Validating);
            // 
            // _lConfirmPassword
            // 
            this._lConfirmPassword.AutoSize = true;
            this._lConfirmPassword.Location = new System.Drawing.Point(8, 119);
            this._lConfirmPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lConfirmPassword.Name = "_lConfirmPassword";
            this._lConfirmPassword.Size = new System.Drawing.Size(90, 13);
            this._lConfirmPassword.TabIndex = 6;
            this._lConfirmPassword.Text = "Confirm password";
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(503, 172);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(100, 28);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bOk.Location = new System.Drawing.Point(393, 172);
            this._bOk.Margin = new System.Windows.Forms.Padding(4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(100, 28);
            this._bOk.TabIndex = 6;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            // 
            // FormDbsLogin
            // 
            this.AcceptButton = this._bOk;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(619, 214);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._gbDbsLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormDbsLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DbsLogin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDbsLogin_FormClosing);
            this._gbDbsLogin.ResumeLayout(false);
            this._gbDbsLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.Label _lPassword;
        private System.Windows.Forms.TextBox _ePassword;
        private System.Windows.Forms.GroupBox _gbDbsLogin;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.RadioButton _rbCreateLogin;
        private System.Windows.Forms.RadioButton _rbExistLogin;
        private System.Windows.Forms.TextBox _eConfirmPassword;
        private System.Windows.Forms.Label _lConfirmPassword;
    }
}