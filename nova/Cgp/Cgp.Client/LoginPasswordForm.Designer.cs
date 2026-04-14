namespace Contal.Cgp.Client
{
    partial class LoginPasswordForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginPasswordForm));
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._lPassword = new System.Windows.Forms.Label();
            this._lUserName = new System.Windows.Forms.Label();
            this._ePassword = new System.Windows.Forms.TextBox();
            this._eUserName = new System.Windows.Forms.TextBox();
            this._authGroup = new System.Windows.Forms.GroupBox();
            this._lCRloginEnabled = new System.Windows.Forms.Label();
            this._gbCard = new System.Windows.Forms.GroupBox();
            this._ePIN = new System.Windows.Forms.TextBox();
            this._lPIN = new System.Windows.Forms.Label();
            this._pInfo = new System.Windows.Forms.Panel();
            this._lLogin = new System.Windows.Forms.Label();
            this._lInfo = new System.Windows.Forms.Label();
            this._pbLogo = new System.Windows.Forms.PictureBox();
            this._authGroup.SuspendLayout();
            this._gbCard.SuspendLayout();
            this._pInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(185, 242);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(104, 242);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _lPassword
            // 
            this._lPassword.AutoSize = true;
            this._lPassword.Location = new System.Drawing.Point(6, 51);
            this._lPassword.Name = "_lPassword";
            this._lPassword.Size = new System.Drawing.Size(53, 13);
            this._lPassword.TabIndex = 2;
            this._lPassword.Text = "Password";
            // 
            // _lUserName
            // 
            this._lUserName.AutoSize = true;
            this._lUserName.Location = new System.Drawing.Point(6, 25);
            this._lUserName.Name = "_lUserName";
            this._lUserName.Size = new System.Drawing.Size(57, 13);
            this._lUserName.TabIndex = 0;
            this._lUserName.Text = "UserName";
            // 
            // _ePassword
            // 
            this._ePassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ePassword.Location = new System.Drawing.Point(108, 49);
            this._ePassword.Name = "_ePassword";
            this._ePassword.Size = new System.Drawing.Size(141, 20);
            this._ePassword.TabIndex = 3;
            this._ePassword.UseSystemPasswordChar = true;
            // 
            // _eUserName
            // 
            this._eUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eUserName.Location = new System.Drawing.Point(108, 23);
            this._eUserName.Name = "_eUserName";
            this._eUserName.Size = new System.Drawing.Size(141, 20);
            this._eUserName.TabIndex = 1;
            // 
            // _authGroup
            // 
            this._authGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._authGroup.Controls.Add(this._lPassword);
            this._authGroup.Controls.Add(this._lUserName);
            this._authGroup.Controls.Add(this._ePassword);
            this._authGroup.Controls.Add(this._eUserName);
            this._authGroup.Location = new System.Drawing.Point(7, 79);
            this._authGroup.Margin = new System.Windows.Forms.Padding(2);
            this._authGroup.Name = "_authGroup";
            this._authGroup.Padding = new System.Windows.Forms.Padding(2);
            this._authGroup.Size = new System.Drawing.Size(257, 83);
            this._authGroup.TabIndex = 0;
            this._authGroup.TabStop = false;
            // 
            // _lCRloginEnabled
            // 
            this._lCRloginEnabled.AutoSize = true;
            this._lCRloginEnabled.Location = new System.Drawing.Point(6, 25);
            this._lCRloginEnabled.Name = "_lCRloginEnabled";
            this._lCRloginEnabled.Size = new System.Drawing.Size(55, 13);
            this._lCRloginEnabled.TabIndex = 0;
            this._lCRloginEnabled.Text = "CR Online";
            // 
            // _gbCard
            // 
            this._gbCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbCard.Controls.Add(this._ePIN);
            this._gbCard.Controls.Add(this._lPIN);
            this._gbCard.Controls.Add(this._lCRloginEnabled);
            this._gbCard.Location = new System.Drawing.Point(7, 167);
            this._gbCard.Name = "_gbCard";
            this._gbCard.Size = new System.Drawing.Size(257, 69);
            this._gbCard.TabIndex = 1;
            this._gbCard.TabStop = false;
            this._gbCard.Text = "Card";
            // 
            // _ePIN
            // 
            this._ePIN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ePIN.Location = new System.Drawing.Point(108, 41);
            this._ePIN.Name = "_ePIN";
            this._ePIN.Size = new System.Drawing.Size(141, 20);
            this._ePIN.TabIndex = 2;
            this._ePIN.UseSystemPasswordChar = true;
            this._ePIN.TextChanged += new System.EventHandler(this._ePIN_TextChanged);
            this._ePIN.KeyDown += new System.Windows.Forms.KeyEventHandler(this._ePIN_KeyDown);
            this._ePIN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._ePIN_KeyPress);
            // 
            // _lPIN
            // 
            this._lPIN.AutoSize = true;
            this._lPIN.Location = new System.Drawing.Point(6, 44);
            this._lPIN.Name = "_lPIN";
            this._lPIN.Size = new System.Drawing.Size(25, 13);
            this._lPIN.TabIndex = 1;
            this._lPIN.Text = "PIN";
            // 
            // _pInfo
            // 
            this._pInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pInfo.BackColor = System.Drawing.Color.Transparent;
            this._pInfo.Controls.Add(this._lLogin);
            this._pInfo.Controls.Add(this._lInfo);
            this._pInfo.Location = new System.Drawing.Point(8, 50);
            this._pInfo.Name = "_pInfo";
            this._pInfo.Size = new System.Drawing.Size(256, 33);
            this._pInfo.TabIndex = 5;
            // 
            // _lLogin
            // 
            this._lLogin.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lLogin.Location = new System.Drawing.Point(6, 2);
            this._lLogin.Name = "_lLogin";
            this._lLogin.Size = new System.Drawing.Size(248, 26);
            this._lLogin.TabIndex = 0;
            this._lLogin.Text = "Login";
            this._lLogin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lInfo
            // 
            this._lInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lInfo.Location = new System.Drawing.Point(1, 1);
            this._lInfo.Name = "_lInfo";
            this._lInfo.Size = new System.Drawing.Size(253, 31);
            this._lInfo.TabIndex = 1;
            this._lInfo.Text = "Info\r\nInfo";
            this._lInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _pbLogo
            // 
            this._pbLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_pbLogo.BackgroundImage")));
            this._pbLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._pbLogo.Location = new System.Drawing.Point(4, 10);
            this._pbLogo.Name = "_pbLogo";
            this._pbLogo.Size = new System.Drawing.Size(260, 38);
            this._pbLogo.TabIndex = 8;
            this._pbLogo.TabStop = false;
            // 
            // LoginPasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(272, 283);
            this.ControlBox = false;
            this.Controls.Add(this._pInfo);
            this.Controls.Add(this._authGroup);
            this.Controls.Add(this._pbLogo);
            this.Controls.Add(this._gbCard);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "LoginPasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " ";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.LoginPasswordForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LoginPasswordForm_KeyDown);
            this._authGroup.ResumeLayout(false);
            this._authGroup.PerformLayout();
            this._gbCard.ResumeLayout(false);
            this._gbCard.PerformLayout();
            this._pInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pbLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Label _lPassword;
        private System.Windows.Forms.Label _lUserName;
        private System.Windows.Forms.TextBox _ePassword;
        private System.Windows.Forms.TextBox _eUserName;
        private System.Windows.Forms.GroupBox _authGroup;
        private System.Windows.Forms.Label _lCRloginEnabled;
        private System.Windows.Forms.GroupBox _gbCard;
        private System.Windows.Forms.TextBox _ePIN;
        private System.Windows.Forms.Label _lPIN;
        private System.Windows.Forms.Panel _pInfo;
        private System.Windows.Forms.Label _lInfo;
        private System.Windows.Forms.PictureBox _pbLogo;
        private System.Windows.Forms.Label _lLogin;
    }
}