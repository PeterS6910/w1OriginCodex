namespace Contal.Cgp.NCAS.Client
{
    partial class CcuConfigurePassword
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
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lCcuPassword = new System.Windows.Forms.Label();
            this._eCcuPassword = new System.Windows.Forms.TextBox();
            this._gbPassword = new System.Windows.Forms.GroupBox();
            this._eVerifyCcuPassword = new System.Windows.Forms.TextBox();
            this._lCcuRetypePassword = new System.Windows.Forms.Label();
            this._gbOldPassword = new System.Windows.Forms.GroupBox();
            this._eOldCcuPassword = new System.Windows.Forms.TextBox();
            this._gbPassword.SuspendLayout();
            this._gbOldPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bOk
            // 
            this._bOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._bOk.Location = new System.Drawing.Point(5, 182);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._bCancel.Location = new System.Drawing.Point(141, 182);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lCcuPassword
            // 
            this._lCcuPassword.AutoSize = true;
            this._lCcuPassword.Location = new System.Drawing.Point(6, 16);
            this._lCcuPassword.Name = "_lCcuPassword";
            this._lCcuPassword.Size = new System.Drawing.Size(53, 13);
            this._lCcuPassword.TabIndex = 0;
            this._lCcuPassword.Text = "Password";
            // 
            // _eCcuPassword
            // 
            this._eCcuPassword.Location = new System.Drawing.Point(6, 32);
            this._eCcuPassword.Name = "_eCcuPassword";
            this._eCcuPassword.PasswordChar = '●';
            this._eCcuPassword.Size = new System.Drawing.Size(192, 20);
            this._eCcuPassword.TabIndex = 1;
            // 
            // _gbPassword
            // 
            this._gbPassword.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._gbPassword.Controls.Add(this._eVerifyCcuPassword);
            this._gbPassword.Controls.Add(this._lCcuRetypePassword);
            this._gbPassword.Controls.Add(this._eCcuPassword);
            this._gbPassword.Controls.Add(this._lCcuPassword);
            this._gbPassword.Location = new System.Drawing.Point(5, 59);
            this._gbPassword.Name = "_gbPassword";
            this._gbPassword.Size = new System.Drawing.Size(211, 100);
            this._gbPassword.TabIndex = 1;
            this._gbPassword.TabStop = false;
            this._gbPassword.Text = "Password";
            // 
            // _eVerifyCcuPassword
            // 
            this._eVerifyCcuPassword.Location = new System.Drawing.Point(6, 71);
            this._eVerifyCcuPassword.Name = "_eVerifyCcuPassword";
            this._eVerifyCcuPassword.PasswordChar = '●';
            this._eVerifyCcuPassword.Size = new System.Drawing.Size(192, 20);
            this._eVerifyCcuPassword.TabIndex = 3;
            // 
            // _lCcuRetypePassword
            // 
            this._lCcuRetypePassword.AutoSize = true;
            this._lCcuRetypePassword.Location = new System.Drawing.Point(6, 55);
            this._lCcuRetypePassword.Name = "_lCcuRetypePassword";
            this._lCcuRetypePassword.Size = new System.Drawing.Size(81, 13);
            this._lCcuRetypePassword.TabIndex = 2;
            this._lCcuRetypePassword.Text = "Verify password";
            // 
            // _gbOldPassword
            // 
            this._gbOldPassword.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._gbOldPassword.Controls.Add(this._eOldCcuPassword);
            this._gbOldPassword.Location = new System.Drawing.Point(5, 5);
            this._gbOldPassword.Name = "_gbOldPassword";
            this._gbOldPassword.Size = new System.Drawing.Size(211, 54);
            this._gbOldPassword.TabIndex = 0;
            this._gbOldPassword.TabStop = false;
            this._gbOldPassword.Text = "Old password";
            // 
            // _eOldCcuPassword
            // 
            this._eOldCcuPassword.Location = new System.Drawing.Point(6, 19);
            this._eOldCcuPassword.Name = "_eOldCcuPassword";
            this._eOldCcuPassword.PasswordChar = '●';
            this._eOldCcuPassword.Size = new System.Drawing.Size(189, 20);
            this._eOldCcuPassword.TabIndex = 0;
            // 
            // CcuConfigurePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(224, 214);
            this.Controls.Add(this._gbOldPassword);
            this.Controls.Add(this._gbPassword);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CcuConfigurePassword";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ChangeConfigurePassword";
            this.Shown += new System.EventHandler(this.CcuConfigurePassword_Shown);
            this._gbPassword.ResumeLayout(false);
            this._gbPassword.PerformLayout();
            this._gbOldPassword.ResumeLayout(false);
            this._gbOldPassword.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lCcuPassword;
        private System.Windows.Forms.TextBox _eCcuPassword;
        private System.Windows.Forms.GroupBox _gbPassword;
        private System.Windows.Forms.TextBox _eVerifyCcuPassword;
        private System.Windows.Forms.Label _lCcuRetypePassword;
        private System.Windows.Forms.GroupBox _gbOldPassword;
        private System.Windows.Forms.TextBox _eOldCcuPassword;
    }
}
