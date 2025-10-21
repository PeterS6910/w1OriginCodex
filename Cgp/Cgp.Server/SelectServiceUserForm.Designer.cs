namespace Contal.Cgp.Server
{
    partial class SelectServiceUserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectServiceUserForm));
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._rbNovaServiceUser = new System.Windows.Forms.RadioButton();
            this._rbCustomUser = new System.Windows.Forms.RadioButton();
            this._eAccountPassword = new System.Windows.Forms.TextBox();
            this._lAccountPassword = new System.Windows.Forms.Label();
            this._eAccountDomain = new System.Windows.Forms.TextBox();
            this._lAccountDomain = new System.Windows.Forms.Label();
            this._eAccountName = new System.Windows.Forms.TextBox();
            this._lAccountName = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._cbServiceStartupType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(400, 334);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(100, 28);
            this._bCancel.TabIndex = 6;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(508, 334);
            this._bOk.Margin = new System.Windows.Forms.Padding(4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(100, 28);
            this._bOk.TabIndex = 7;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this._rbNovaServiceUser);
            this.groupBox1.Controls.Add(this._rbCustomUser);
            this.groupBox1.Controls.Add(this._eAccountPassword);
            this.groupBox1.Controls.Add(this._lAccountPassword);
            this.groupBox1.Controls.Add(this._eAccountDomain);
            this.groupBox1.Controls.Add(this._lAccountDomain);
            this.groupBox1.Controls.Add(this._eAccountName);
            this.groupBox1.Controls.Add(this._lAccountName);
            this.groupBox1.Location = new System.Drawing.Point(13, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(594, 200);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log on options";
            // 
            // _rbNovaServiceUser
            // 
            this._rbNovaServiceUser.AutoSize = true;
            this._rbNovaServiceUser.Checked = true;
            this._rbNovaServiceUser.Location = new System.Drawing.Point(7, 154);
            this._rbNovaServiceUser.Margin = new System.Windows.Forms.Padding(4);
            this._rbNovaServiceUser.Name = "_rbNovaServiceUser";
            this._rbNovaServiceUser.Size = new System.Drawing.Size(172, 21);
            this._rbNovaServiceUser.TabIndex = 18;
            this._rbNovaServiceUser.TabStop = true;
            this._rbNovaServiceUser.Text = "NOVA SERVICE USER";
            this._rbNovaServiceUser.UseVisualStyleBackColor = true;
            // 
            // _rbCustomUser
            // 
            this._rbCustomUser.AutoSize = true;
            this._rbCustomUser.Location = new System.Drawing.Point(7, 29);
            this._rbCustomUser.Margin = new System.Windows.Forms.Padding(4);
            this._rbCustomUser.Name = "_rbCustomUser";
            this._rbCustomUser.Size = new System.Drawing.Size(130, 21);
            this._rbCustomUser.TabIndex = 17;
            this._rbCustomUser.Text = "CUSTOM USER";
            this._rbCustomUser.UseVisualStyleBackColor = true;
            this._rbCustomUser.CheckedChanged += new System.EventHandler(this._rbCustomAcount_CheckedChanged);
            // 
            // _eAccountPassword
            // 
            this._eAccountPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eAccountPassword.Enabled = false;
            this._eAccountPassword.Location = new System.Drawing.Point(153, 108);
            this._eAccountPassword.Margin = new System.Windows.Forms.Padding(4);
            this._eAccountPassword.Name = "_eAccountPassword";
            this._eAccountPassword.PasswordChar = '●';
            this._eAccountPassword.Size = new System.Drawing.Size(420, 22);
            this._eAccountPassword.TabIndex = 16;
            // 
            // _lAccountPassword
            // 
            this._lAccountPassword.AutoSize = true;
            this._lAccountPassword.Location = new System.Drawing.Point(51, 111);
            this._lAccountPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAccountPassword.Name = "_lAccountPassword";
            this._lAccountPassword.Size = new System.Drawing.Size(69, 17);
            this._lAccountPassword.TabIndex = 15;
            this._lAccountPassword.Text = "Password";
            // 
            // _eAccountDomain
            // 
            this._eAccountDomain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eAccountDomain.Enabled = false;
            this._eAccountDomain.Location = new System.Drawing.Point(153, 83);
            this._eAccountDomain.Margin = new System.Windows.Forms.Padding(4);
            this._eAccountDomain.Name = "_eAccountDomain";
            this._eAccountDomain.Size = new System.Drawing.Size(420, 22);
            this._eAccountDomain.TabIndex = 14;
            // 
            // _lAccountDomain
            // 
            this._lAccountDomain.AutoSize = true;
            this._lAccountDomain.Location = new System.Drawing.Point(50, 86);
            this._lAccountDomain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAccountDomain.Name = "_lAccountDomain";
            this._lAccountDomain.Size = new System.Drawing.Size(56, 17);
            this._lAccountDomain.TabIndex = 13;
            this._lAccountDomain.Text = "Domain";
            // 
            // _eAccountName
            // 
            this._eAccountName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eAccountName.Enabled = false;
            this._eAccountName.Location = new System.Drawing.Point(153, 58);
            this._eAccountName.Margin = new System.Windows.Forms.Padding(4);
            this._eAccountName.Name = "_eAccountName";
            this._eAccountName.Size = new System.Drawing.Size(420, 22);
            this._eAccountName.TabIndex = 12;
            this._eAccountName.Leave += new System.EventHandler(this._eAccountName_Leave);
            // 
            // _lAccountName
            // 
            this._lAccountName.AutoSize = true;
            this._lAccountName.Location = new System.Drawing.Point(50, 61);
            this._lAccountName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAccountName.Name = "_lAccountName";
            this._lAccountName.Size = new System.Drawing.Size(45, 17);
            this._lAccountName.TabIndex = 11;
            this._lAccountName.Text = "Name";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._cbServiceStartupType);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 214);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(595, 93);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Other options";
            // 
            // _cbServiceStartupType
            // 
            this._cbServiceStartupType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cbServiceStartupType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbServiceStartupType.FormattingEnabled = true;
            this._cbServiceStartupType.Items.AddRange(new object[] {
            "Automatic",
            "Manual"});
            this._cbServiceStartupType.Location = new System.Drawing.Point(154, 37);
            this._cbServiceStartupType.Name = "_cbServiceStartupType";
            this._cbServiceStartupType.Size = new System.Drawing.Size(420, 24);
            this._cbServiceStartupType.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 17);
            this.label1.TabIndex = 20;
            this.label1.Text = "Startup type";
            // 
            // SelectServiceUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 377);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SelectServiceUserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contal Nova Service settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SelectServiceUserForm_Load);
            this.Shown += new System.EventHandler(this.SelectServiceUserForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton _rbNovaServiceUser;
        private System.Windows.Forms.RadioButton _rbCustomUser;
        private System.Windows.Forms.Label _lAccountPassword;
        private System.Windows.Forms.TextBox _eAccountDomain;
        private System.Windows.Forms.Label _lAccountDomain;
        private System.Windows.Forms.TextBox _eAccountName;
        private System.Windows.Forms.Label _lAccountName;
        private System.Windows.Forms.TextBox _eAccountPassword;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox _cbServiceStartupType;
        private System.Windows.Forms.Label label1;
    }
}