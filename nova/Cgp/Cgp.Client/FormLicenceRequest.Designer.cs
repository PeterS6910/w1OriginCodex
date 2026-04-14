namespace Contal.Cgp.Client
{
    partial class FormLicenceRequest
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
            this._buttonSave = new System.Windows.Forms.Button();
            this._groupBoxCustomerInfo = new System.Windows.Forms.GroupBox();
            this._textBoxLastName = new System.Windows.Forms.TextBox();
            this._textBoxEmail = new System.Windows.Forms.TextBox();
            this._labelContactEmail = new System.Windows.Forms.Label();
            this._labelContactLastName = new System.Windows.Forms.Label();
            this._textBoxPhone = new System.Windows.Forms.TextBox();
            this._labelContactPhone = new System.Windows.Forms.Label();
            this._textBoxFirstName = new System.Windows.Forms.TextBox();
            this._labelContactFirstName = new System.Windows.Forms.Label();
            this._textBoxAddress = new System.Windows.Forms.TextBox();
            this._labelCustAddr = new System.Windows.Forms.Label();
            this._textBoxCustomerName = new System.Windows.Forms.TextBox();
            this._labelCustName = new System.Windows.Forms.Label();
            this._buttonCancel = new System.Windows.Forms.Button();
            this._groupBoxCustomerInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // _buttonSave
            // 
            this._buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonSave.Location = new System.Drawing.Point(215, 204);
            this._buttonSave.Name = "_buttonSave";
            this._buttonSave.Size = new System.Drawing.Size(75, 23);
            this._buttonSave.TabIndex = 100;
            this._buttonSave.Text = "Save";
            this._buttonSave.UseVisualStyleBackColor = true;
            this._buttonSave.Click += new System.EventHandler(this._buttonSave_Click);
            // 
            // _groupBoxCustomerInfo
            // 
            this._groupBoxCustomerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._groupBoxCustomerInfo.Controls.Add(this._textBoxLastName);
            this._groupBoxCustomerInfo.Controls.Add(this._textBoxEmail);
            this._groupBoxCustomerInfo.Controls.Add(this._labelContactEmail);
            this._groupBoxCustomerInfo.Controls.Add(this._labelContactLastName);
            this._groupBoxCustomerInfo.Controls.Add(this._textBoxPhone);
            this._groupBoxCustomerInfo.Controls.Add(this._labelContactPhone);
            this._groupBoxCustomerInfo.Controls.Add(this._textBoxFirstName);
            this._groupBoxCustomerInfo.Controls.Add(this._labelContactFirstName);
            this._groupBoxCustomerInfo.Controls.Add(this._textBoxAddress);
            this._groupBoxCustomerInfo.Controls.Add(this._labelCustAddr);
            this._groupBoxCustomerInfo.Controls.Add(this._textBoxCustomerName);
            this._groupBoxCustomerInfo.Controls.Add(this._labelCustName);
            this._groupBoxCustomerInfo.Location = new System.Drawing.Point(12, 12);
            this._groupBoxCustomerInfo.Name = "_groupBoxCustomerInfo";
            this._groupBoxCustomerInfo.Size = new System.Drawing.Size(359, 186);
            this._groupBoxCustomerInfo.TabIndex = 2;
            this._groupBoxCustomerInfo.TabStop = false;
            this._groupBoxCustomerInfo.Text = "Customer information";
            // 
            // _textBoxLastName
            // 
            this._textBoxLastName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxLastName.BackColor = System.Drawing.Color.White;
            this._textBoxLastName.Location = new System.Drawing.Point(160, 97);
            this._textBoxLastName.MaxLength = 20;
            this._textBoxLastName.Name = "_textBoxLastName";
            this._textBoxLastName.Size = new System.Drawing.Size(189, 20);
            this._textBoxLastName.TabIndex = 4;
            this._textBoxLastName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textBoxCustomerName_KeyPress);
            // 
            // _textBoxEmail
            // 
            this._textBoxEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxEmail.BackColor = System.Drawing.Color.White;
            this._textBoxEmail.Location = new System.Drawing.Point(160, 149);
            this._textBoxEmail.MaxLength = 30;
            this._textBoxEmail.Name = "_textBoxEmail";
            this._textBoxEmail.Size = new System.Drawing.Size(189, 20);
            this._textBoxEmail.TabIndex = 6;
            this._textBoxEmail.Leave += new System.EventHandler(this._textBoxEmail_Leave);
            this._textBoxEmail.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textBoxEmail_KeyPress);
            // 
            // _labelContactEmail
            // 
            this._labelContactEmail.AutoSize = true;
            this._labelContactEmail.Location = new System.Drawing.Point(12, 152);
            this._labelContactEmail.Name = "_labelContactEmail";
            this._labelContactEmail.Size = new System.Drawing.Size(71, 13);
            this._labelContactEmail.TabIndex = 6;
            this._labelContactEmail.Text = "Contact email";
            // 
            // _labelContactLastName
            // 
            this._labelContactLastName.AutoSize = true;
            this._labelContactLastName.Location = new System.Drawing.Point(12, 100);
            this._labelContactLastName.Name = "_labelContactLastName";
            this._labelContactLastName.Size = new System.Drawing.Size(92, 13);
            this._labelContactLastName.TabIndex = 10;
            this._labelContactLastName.Text = "Contact last name";
            // 
            // _textBoxPhone
            // 
            this._textBoxPhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxPhone.BackColor = System.Drawing.Color.White;
            this._textBoxPhone.Location = new System.Drawing.Point(160, 123);
            this._textBoxPhone.MaxLength = 20;
            this._textBoxPhone.Name = "_textBoxPhone";
            this._textBoxPhone.Size = new System.Drawing.Size(189, 20);
            this._textBoxPhone.TabIndex = 5;
            this._textBoxPhone.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textBoxCustomerName_KeyPress);
            // 
            // _labelContactPhone
            // 
            this._labelContactPhone.AutoSize = true;
            this._labelContactPhone.Location = new System.Drawing.Point(12, 126);
            this._labelContactPhone.Name = "_labelContactPhone";
            this._labelContactPhone.Size = new System.Drawing.Size(77, 13);
            this._labelContactPhone.TabIndex = 8;
            this._labelContactPhone.Text = "Contact phone";
            // 
            // _textBoxFirstName
            // 
            this._textBoxFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxFirstName.BackColor = System.Drawing.Color.White;
            this._textBoxFirstName.Location = new System.Drawing.Point(160, 71);
            this._textBoxFirstName.MaxLength = 20;
            this._textBoxFirstName.Name = "_textBoxFirstName";
            this._textBoxFirstName.Size = new System.Drawing.Size(189, 20);
            this._textBoxFirstName.TabIndex = 3;
            this._textBoxFirstName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textBoxCustomerName_KeyPress);
            // 
            // _labelContactFirstName
            // 
            this._labelContactFirstName.AutoSize = true;
            this._labelContactFirstName.Location = new System.Drawing.Point(12, 74);
            this._labelContactFirstName.Name = "_labelContactFirstName";
            this._labelContactFirstName.Size = new System.Drawing.Size(92, 13);
            this._labelContactFirstName.TabIndex = 4;
            this._labelContactFirstName.Text = "Contact first name";
            // 
            // _textBoxAddress
            // 
            this._textBoxAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxAddress.BackColor = System.Drawing.Color.White;
            this._textBoxAddress.Location = new System.Drawing.Point(160, 45);
            this._textBoxAddress.MaxLength = 50;
            this._textBoxAddress.Name = "_textBoxAddress";
            this._textBoxAddress.Size = new System.Drawing.Size(189, 20);
            this._textBoxAddress.TabIndex = 2;
            this._textBoxAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textBoxCustomerName_KeyPress);
            // 
            // _labelCustAddr
            // 
            this._labelCustAddr.AutoSize = true;
            this._labelCustAddr.Location = new System.Drawing.Point(12, 48);
            this._labelCustAddr.Name = "_labelCustAddr";
            this._labelCustAddr.Size = new System.Drawing.Size(45, 13);
            this._labelCustAddr.TabIndex = 2;
            this._labelCustAddr.Text = "Address";
            // 
            // _textBoxCustomerName
            // 
            this._textBoxCustomerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxCustomerName.BackColor = System.Drawing.Color.White;
            this._textBoxCustomerName.Location = new System.Drawing.Point(160, 19);
            this._textBoxCustomerName.MaxLength = 20;
            this._textBoxCustomerName.Name = "_textBoxCustomerName";
            this._textBoxCustomerName.Size = new System.Drawing.Size(189, 20);
            this._textBoxCustomerName.TabIndex = 1;
            this._textBoxCustomerName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textBoxCustomerName_KeyPress);
            // 
            // _labelCustName
            // 
            this._labelCustName.AutoSize = true;
            this._labelCustName.Location = new System.Drawing.Point(12, 22);
            this._labelCustName.Name = "_labelCustName";
            this._labelCustName.Size = new System.Drawing.Size(80, 13);
            this._labelCustName.TabIndex = 0;
            this._labelCustName.Text = "Customer name";
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonCancel.Location = new System.Drawing.Point(296, 204);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 101;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this._buttonCancel_Click);
            // 
            // FormLicenceRequest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(383, 239);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._groupBoxCustomerInfo);
            this.Controls.Add(this._buttonSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormLicenceRequest";
            this.Text = "LicenceRequest";
            this._groupBoxCustomerInfo.ResumeLayout(false);
            this._groupBoxCustomerInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _buttonSave;
        private System.Windows.Forms.GroupBox _groupBoxCustomerInfo;
        private System.Windows.Forms.TextBox _textBoxLastName;
        private System.Windows.Forms.TextBox _textBoxEmail;
        private System.Windows.Forms.Label _labelContactEmail;
        private System.Windows.Forms.Label _labelContactLastName;
        private System.Windows.Forms.TextBox _textBoxPhone;
        private System.Windows.Forms.Label _labelContactPhone;
        private System.Windows.Forms.TextBox _textBoxFirstName;
        private System.Windows.Forms.Label _labelContactFirstName;
        private System.Windows.Forms.TextBox _textBoxAddress;
        private System.Windows.Forms.Label _labelCustAddr;
        private System.Windows.Forms.TextBox _textBoxCustomerName;
        private System.Windows.Forms.Label _labelCustName;
        private System.Windows.Forms.Button _buttonCancel;
    }
}