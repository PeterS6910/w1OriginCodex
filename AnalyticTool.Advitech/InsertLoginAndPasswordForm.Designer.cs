namespace AnalyticTool.Advitech
{
    partial class InsertLoginAndPasswordForm
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
            this._ePassword = new System.Windows.Forms.TextBox();
            this._lPassword = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(262, 100);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(95, 26);
            this._bOk.TabIndex = 3;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(363, 100);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(95, 26);
            this._bCancel.TabIndex = 4;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _ePassword
            // 
            this._ePassword.Location = new System.Drawing.Point(165, 45);
            this._ePassword.Margin = new System.Windows.Forms.Padding(4);
            this._ePassword.Name = "_ePassword";
            this._ePassword.PasswordChar = '●';
            this._ePassword.Size = new System.Drawing.Size(292, 20);
            this._ePassword.TabIndex = 2;
            // 
            // _lPassword
            // 
            this._lPassword.AutoSize = true;
            this._lPassword.Location = new System.Drawing.Point(10, 48);
            this._lPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPassword.Name = "_lPassword";
            this._lPassword.Size = new System.Drawing.Size(53, 13);
            this._lPassword.TabIndex = 25;
            this._lPassword.Text = "Password";
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(165, 13);
            this._eName.Margin = new System.Windows.Forms.Padding(4);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(292, 20);
            this._eName.TabIndex = 1;
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(10, 16);
            this._lName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 23;
            this._lName.Text = "Name";
            // 
            // InsertLoginAndPasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 138);
            this.Controls.Add(this._ePassword);
            this.Controls.Add(this._lPassword);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InsertLoginAndPasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InsertLoginAndPasswordForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.TextBox _ePassword;
        private System.Windows.Forms.Label _lPassword;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
    }
}