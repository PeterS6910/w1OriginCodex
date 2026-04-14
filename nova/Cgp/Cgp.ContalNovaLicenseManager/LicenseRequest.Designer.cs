namespace Contal.Cgp.ContalNovaLicenseManager
{
    partial class LicenseRequest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseRequest));
            this._cbEdition = new System.Windows.Forms.ComboBox();
            this._lConnectionCount = new System.Windows.Forms.Label();
            this._lEdition = new System.Windows.Forms.Label();
            this._eConnectionCount = new System.Windows.Forms.NumericUpDown();
            this._lDSMCount = new System.Windows.Forms.Label();
            this._eDSMCount = new System.Windows.Forms.NumericUpDown();
            this._chbGraphics = new System.Windows.Forms.CheckBox();
            this._chbCis = new System.Windows.Forms.CheckBox();
            this._chbIdManagement = new System.Windows.Forms.CheckBox();
            this._chbOfflineImport = new System.Windows.Forms.CheckBox();
            this._bCreateRequest = new System.Windows.Forms.Button();
            this._gbCustomerDetails = new System.Windows.Forms.GroupBox();
            this._cbCountries = new System.Windows.Forms.ComboBox();
            this._lCountry = new System.Windows.Forms.Label();
            this._eEmail = new System.Windows.Forms.TextBox();
            this._lContactEmail = new System.Windows.Forms.Label();
            this._ePhone = new System.Windows.Forms.TextBox();
            this._lContactPhone = new System.Windows.Forms.Label();
            this._eLastName = new System.Windows.Forms.TextBox();
            this._lContactLastName = new System.Windows.Forms.Label();
            this._eFirstName = new System.Windows.Forms.TextBox();
            this._lContactFirstName = new System.Windows.Forms.Label();
            this._eAddress = new System.Windows.Forms.TextBox();
            this._lAddress = new System.Windows.Forms.Label();
            this._eCustomerName = new System.Windows.Forms.TextBox();
            this._lCustomerName = new System.Windows.Forms.Label();
            this._chbMaxSubsiteCount = new System.Windows.Forms.CheckBox();
            this._eCat12ComboCount = new System.Windows.Forms.NumericUpDown();
            this._lCat12ComboCount = new System.Windows.Forms.Label();
            this._nudMajorVersion = new System.Windows.Forms.NumericUpDown();
            this._lMajorVersion = new System.Windows.Forms.Label();
            this._chbTimetecMaster = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this._eConnectionCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eDSMCount)).BeginInit();
            this._gbCustomerDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eCat12ComboCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudMajorVersion)).BeginInit();
            this.SuspendLayout();
            // 
            // _cbEdition
            // 
            this._cbEdition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbEdition.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._cbEdition.FormattingEnabled = true;
            this._cbEdition.Location = new System.Drawing.Point(172, 237);
            this._cbEdition.Name = "_cbEdition";
            this._cbEdition.Size = new System.Drawing.Size(407, 26);
            this._cbEdition.TabIndex = 1;
            this._cbEdition.SelectedIndexChanged += new System.EventHandler(this.OnEditionSelected);
            // 
            // _lConnectionCount
            // 
            this._lConnectionCount.AutoSize = true;
            this._lConnectionCount.Location = new System.Drawing.Point(25, 284);
            this._lConnectionCount.Name = "_lConnectionCount";
            this._lConnectionCount.Size = new System.Drawing.Size(119, 13);
            this._lConnectionCount.TabIndex = 1;
            this._lConnectionCount.Text = "Client connection count";
            this._lConnectionCount.Visible = false;
            // 
            // _lEdition
            // 
            this._lEdition.AutoSize = true;
            this._lEdition.ForeColor = System.Drawing.Color.Red;
            this._lEdition.Location = new System.Drawing.Point(25, 244);
            this._lEdition.Name = "_lEdition";
            this._lEdition.Size = new System.Drawing.Size(39, 13);
            this._lEdition.TabIndex = 2;
            this._lEdition.Text = "Edition";
            // 
            // _eConnectionCount
            // 
            this._eConnectionCount.Location = new System.Drawing.Point(172, 282);
            this._eConnectionCount.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this._eConnectionCount.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this._eConnectionCount.Name = "_eConnectionCount";
            this._eConnectionCount.Size = new System.Drawing.Size(102, 20);
            this._eConnectionCount.TabIndex = 2;
            this._eConnectionCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this._eConnectionCount.Visible = false;
            // 
            // _lDSMCount
            // 
            this._lDSMCount.AutoSize = true;
            this._lDSMCount.Location = new System.Drawing.Point(25, 310);
            this._lDSMCount.Name = "_lDSMCount";
            this._lDSMCount.Size = new System.Drawing.Size(121, 13);
            this._lDSMCount.TabIndex = 4;
            this._lDSMCount.Text = "Door environment count";
            this._lDSMCount.Visible = false;
            // 
            // _eDSMCount
            // 
            this._eDSMCount.Location = new System.Drawing.Point(172, 308);
            this._eDSMCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this._eDSMCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eDSMCount.Name = "_eDSMCount";
            this._eDSMCount.Size = new System.Drawing.Size(102, 20);
            this._eDSMCount.TabIndex = 3;
            this._eDSMCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eDSMCount.Visible = false;
            // 
            // _chbGraphics
            // 
            this._chbGraphics.AutoSize = true;
            this._chbGraphics.Location = new System.Drawing.Point(332, 306);
            this._chbGraphics.Name = "_chbGraphics";
            this._chbGraphics.Size = new System.Drawing.Size(68, 17);
            this._chbGraphics.TabIndex = 7;
            this._chbGraphics.Text = "Graphics";
            this._chbGraphics.UseVisualStyleBackColor = true;
            this._chbGraphics.Visible = false;
            // 
            // _chbCis
            // 
            this._chbCis.AutoSize = true;
            this._chbCis.Location = new System.Drawing.Point(332, 329);
            this._chbCis.Name = "_chbCis";
            this._chbCis.Size = new System.Drawing.Size(95, 17);
            this._chbCis.TabIndex = 4;
            this._chbCis.Text = "CIS integration";
            this._chbCis.UseVisualStyleBackColor = true;
            this._chbCis.Visible = false;
            // 
            // _chbIdManagement
            // 
            this._chbIdManagement.AutoSize = true;
            this._chbIdManagement.Location = new System.Drawing.Point(332, 398);
            this._chbIdManagement.Name = "_chbIdManagement";
            this._chbIdManagement.Size = new System.Drawing.Size(174, 17);
            this._chbIdManagement.TabIndex = 6;
            this._chbIdManagement.Text = "ID management / Batch design";
            this._chbIdManagement.UseVisualStyleBackColor = true;
            this._chbIdManagement.Visible = false;
            // 
            // _chbOfflineImport
            // 
            this._chbOfflineImport.AutoSize = true;
            this._chbOfflineImport.Location = new System.Drawing.Point(332, 352);
            this._chbOfflineImport.Name = "_chbOfflineImport";
            this._chbOfflineImport.Size = new System.Drawing.Size(222, 17);
            this._chbOfflineImport.TabIndex = 5;
            this._chbOfflineImport.Text = "Import from offline data source (CSV,TXT)";
            this._chbOfflineImport.UseVisualStyleBackColor = true;
            this._chbOfflineImport.Visible = false;
            // 
            // _bCreateRequest
            // 
            this._bCreateRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreateRequest.Location = new System.Drawing.Point(519, 424);
            this._bCreateRequest.Name = "_bCreateRequest";
            this._bCreateRequest.Size = new System.Drawing.Size(72, 23);
            this._bCreateRequest.TabIndex = 7;
            this._bCreateRequest.Text = "Save";
            this._bCreateRequest.UseVisualStyleBackColor = true;
            this._bCreateRequest.Click += new System.EventHandler(this._bCreateRequest_Click);
            // 
            // _gbCustomerDetails
            // 
            this._gbCustomerDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbCustomerDetails.Controls.Add(this._cbCountries);
            this._gbCustomerDetails.Controls.Add(this._lCountry);
            this._gbCustomerDetails.Controls.Add(this._eEmail);
            this._gbCustomerDetails.Controls.Add(this._lContactEmail);
            this._gbCustomerDetails.Controls.Add(this._ePhone);
            this._gbCustomerDetails.Controls.Add(this._lContactPhone);
            this._gbCustomerDetails.Controls.Add(this._eLastName);
            this._gbCustomerDetails.Controls.Add(this._lContactLastName);
            this._gbCustomerDetails.Controls.Add(this._eFirstName);
            this._gbCustomerDetails.Controls.Add(this._lContactFirstName);
            this._gbCustomerDetails.Controls.Add(this._eAddress);
            this._gbCustomerDetails.Controls.Add(this._lAddress);
            this._gbCustomerDetails.Controls.Add(this._eCustomerName);
            this._gbCustomerDetails.Controls.Add(this._lCustomerName);
            this._gbCustomerDetails.Location = new System.Drawing.Point(13, 13);
            this._gbCustomerDetails.Name = "_gbCustomerDetails";
            this._gbCustomerDetails.Size = new System.Drawing.Size(578, 217);
            this._gbCustomerDetails.TabIndex = 0;
            this._gbCustomerDetails.TabStop = false;
            this._gbCustomerDetails.Text = "Customer details";
            // 
            // _cbCountries
            // 
            this._cbCountries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cbCountries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCountries.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._cbCountries.FormattingEnabled = true;
            this._cbCountries.Items.AddRange(new object[] {
            "Express",
            "Standard",
            "Enterprise",
            "Premium"});
            this._cbCountries.Location = new System.Drawing.Point(159, 72);
            this._cbCountries.Name = "_cbCountries";
            this._cbCountries.Size = new System.Drawing.Size(407, 26);
            this._cbCountries.TabIndex = 13;
            this._cbCountries.SelectedValueChanged += new System.EventHandler(this._cbCountries_SelectedValueChanged);
            // 
            // _lCountry
            // 
            this._lCountry.AutoSize = true;
            this._lCountry.Location = new System.Drawing.Point(15, 79);
            this._lCountry.Name = "_lCountry";
            this._lCountry.Size = new System.Drawing.Size(43, 13);
            this._lCountry.TabIndex = 12;
            this._lCountry.Text = "Country";
            // 
            // _eEmail
            // 
            this._eEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eEmail.Location = new System.Drawing.Point(159, 186);
            this._eEmail.Name = "_eEmail";
            this._eEmail.Size = new System.Drawing.Size(407, 20);
            this._eEmail.TabIndex = 5;
            // 
            // _lContactEmail
            // 
            this._lContactEmail.AutoSize = true;
            this._lContactEmail.Location = new System.Drawing.Point(15, 189);
            this._lContactEmail.Name = "_lContactEmail";
            this._lContactEmail.Size = new System.Drawing.Size(71, 13);
            this._lContactEmail.TabIndex = 11;
            this._lContactEmail.Text = "Contact email";
            // 
            // _ePhone
            // 
            this._ePhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ePhone.Location = new System.Drawing.Point(159, 160);
            this._ePhone.Name = "_ePhone";
            this._ePhone.Size = new System.Drawing.Size(407, 20);
            this._ePhone.TabIndex = 4;
            // 
            // _lContactPhone
            // 
            this._lContactPhone.AutoSize = true;
            this._lContactPhone.Location = new System.Drawing.Point(15, 160);
            this._lContactPhone.Name = "_lContactPhone";
            this._lContactPhone.Size = new System.Drawing.Size(77, 13);
            this._lContactPhone.TabIndex = 0;
            this._lContactPhone.Text = "Contact phone";
            // 
            // _eLastName
            // 
            this._eLastName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eLastName.Location = new System.Drawing.Point(159, 134);
            this._eLastName.Name = "_eLastName";
            this._eLastName.Size = new System.Drawing.Size(407, 20);
            this._eLastName.TabIndex = 3;
            // 
            // _lContactLastName
            // 
            this._lContactLastName.AutoSize = true;
            this._lContactLastName.Location = new System.Drawing.Point(15, 134);
            this._lContactLastName.Name = "_lContactLastName";
            this._lContactLastName.Size = new System.Drawing.Size(92, 13);
            this._lContactLastName.TabIndex = 9;
            this._lContactLastName.Text = "Contact last name";
            // 
            // _eFirstName
            // 
            this._eFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eFirstName.Location = new System.Drawing.Point(159, 108);
            this._eFirstName.Name = "_eFirstName";
            this._eFirstName.Size = new System.Drawing.Size(407, 20);
            this._eFirstName.TabIndex = 2;
            // 
            // _lContactFirstName
            // 
            this._lContactFirstName.AutoSize = true;
            this._lContactFirstName.Location = new System.Drawing.Point(15, 108);
            this._lContactFirstName.Name = "_lContactFirstName";
            this._lContactFirstName.Size = new System.Drawing.Size(92, 13);
            this._lContactFirstName.TabIndex = 9;
            this._lContactFirstName.Text = "Contact first name";
            // 
            // _eAddress
            // 
            this._eAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eAddress.Location = new System.Drawing.Point(159, 46);
            this._eAddress.Name = "_eAddress";
            this._eAddress.Size = new System.Drawing.Size(407, 20);
            this._eAddress.TabIndex = 1;
            // 
            // _lAddress
            // 
            this._lAddress.AutoSize = true;
            this._lAddress.Location = new System.Drawing.Point(15, 46);
            this._lAddress.Name = "_lAddress";
            this._lAddress.Size = new System.Drawing.Size(45, 13);
            this._lAddress.TabIndex = 6;
            this._lAddress.Text = "Address";
            // 
            // _eCustomerName
            // 
            this._eCustomerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eCustomerName.Location = new System.Drawing.Point(159, 20);
            this._eCustomerName.Name = "_eCustomerName";
            this._eCustomerName.Size = new System.Drawing.Size(407, 20);
            this._eCustomerName.TabIndex = 0;
            // 
            // _lCustomerName
            // 
            this._lCustomerName.AutoSize = true;
            this._lCustomerName.Location = new System.Drawing.Point(15, 20);
            this._lCustomerName.Name = "_lCustomerName";
            this._lCustomerName.Size = new System.Drawing.Size(80, 13);
            this._lCustomerName.TabIndex = 10;
            this._lCustomerName.Text = "Customer name";
            // 
            // _chbMaxSubsiteCount
            // 
            this._chbMaxSubsiteCount.AutoSize = true;
            this._chbMaxSubsiteCount.Location = new System.Drawing.Point(332, 375);
            this._chbMaxSubsiteCount.Name = "_chbMaxSubsiteCount";
            this._chbMaxSubsiteCount.Size = new System.Drawing.Size(94, 17);
            this._chbMaxSubsiteCount.TabIndex = 10;
            this._chbMaxSubsiteCount.Text = "Structured site";
            this._chbMaxSubsiteCount.UseVisualStyleBackColor = true;
            this._chbMaxSubsiteCount.Visible = false;
            this._chbMaxSubsiteCount.CheckedChanged += new System.EventHandler(this._chbMaxSubsiteCount_CheckedChanged);
            // 
            // _eCat12ComboCount
            // 
            this._eCat12ComboCount.Location = new System.Drawing.Point(172, 334);
            this._eCat12ComboCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this._eCat12ComboCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eCat12ComboCount.Name = "_eCat12ComboCount";
            this._eCat12ComboCount.Size = new System.Drawing.Size(102, 20);
            this._eCat12ComboCount.TabIndex = 11;
            this._eCat12ComboCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eCat12ComboCount.Visible = false;
            // 
            // _lCat12ComboCount
            // 
            this._lCat12ComboCount.AutoSize = true;
            this._lCat12ComboCount.Location = new System.Drawing.Point(25, 336);
            this._lCat12ComboCount.Name = "_lCat12ComboCount";
            this._lCat12ComboCount.Size = new System.Drawing.Size(143, 13);
            this._lCat12ComboCount.TabIndex = 12;
            this._lCat12ComboCount.Text = "CCU12/CAT12CE unit count";
            this._lCat12ComboCount.Visible = false;
            // 
            // _nudMajorVersion
            // 
            this._nudMajorVersion.Location = new System.Drawing.Point(172, 413);
            this._nudMajorVersion.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this._nudMajorVersion.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudMajorVersion.Name = "_nudMajorVersion";
            this._nudMajorVersion.Size = new System.Drawing.Size(102, 20);
            this._nudMajorVersion.TabIndex = 0;
            this._nudMajorVersion.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudMajorVersion.Visible = false;
            // 
            // _lMajorVersion
            // 
            this._lMajorVersion.AutoSize = true;
            this._lMajorVersion.Location = new System.Drawing.Point(25, 415);
            this._lMajorVersion.Name = "_lMajorVersion";
            this._lMajorVersion.Size = new System.Drawing.Size(70, 13);
            this._lMajorVersion.TabIndex = 4;
            this._lMajorVersion.Text = "Major version";
            this._lMajorVersion.Visible = false;
            // 
            // _chbTimetecMaster
            // 
            this._chbTimetecMaster.AutoSize = true;
            this._chbTimetecMaster.Location = new System.Drawing.Point(332, 284);
            this._chbTimetecMaster.Name = "_chbTimetecMaster";
            this._chbTimetecMaster.Size = new System.Drawing.Size(98, 17);
            this._chbTimetecMaster.TabIndex = 13;
            this._chbTimetecMaster.Text = "Timetec master";
            this._chbTimetecMaster.UseVisualStyleBackColor = true;
            this._chbTimetecMaster.Visible = false;
            // 
            // LicenseRequest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 456);
            this.Controls.Add(this._chbTimetecMaster);
            this.Controls.Add(this._eCat12ComboCount);
            this.Controls.Add(this._lCat12ComboCount);
            this.Controls.Add(this._chbMaxSubsiteCount);
            this.Controls.Add(this._gbCustomerDetails);
            this.Controls.Add(this._bCreateRequest);
            this.Controls.Add(this._chbOfflineImport);
            this.Controls.Add(this._chbIdManagement);
            this.Controls.Add(this._chbCis);
            this.Controls.Add(this._chbGraphics);
            this.Controls.Add(this._nudMajorVersion);
            this.Controls.Add(this._lMajorVersion);
            this.Controls.Add(this._eDSMCount);
            this.Controls.Add(this._lDSMCount);
            this.Controls.Add(this._eConnectionCount);
            this.Controls.Add(this._lEdition);
            this.Controls.Add(this._lConnectionCount);
            this.Controls.Add(this._cbEdition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LicenseRequest";
            this.Text = "Contal Nova License Manager : License request";
            this.Load += new System.EventHandler(this.LicenseRequest_Load);
            ((System.ComponentModel.ISupportInitialize)(this._eConnectionCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eDSMCount)).EndInit();
            this._gbCustomerDetails.ResumeLayout(false);
            this._gbCustomerDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eCat12ComboCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudMajorVersion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _cbEdition;
        private System.Windows.Forms.Label _lConnectionCount;
        private System.Windows.Forms.Label _lEdition;
        private System.Windows.Forms.NumericUpDown _eConnectionCount;
        private System.Windows.Forms.Label _lDSMCount;
        private System.Windows.Forms.NumericUpDown _eDSMCount;
        private System.Windows.Forms.CheckBox _chbGraphics;
        private System.Windows.Forms.CheckBox _chbCis;
        private System.Windows.Forms.CheckBox _chbIdManagement;
        private System.Windows.Forms.CheckBox _chbOfflineImport;
        private System.Windows.Forms.Button _bCreateRequest;
        private System.Windows.Forms.GroupBox _gbCustomerDetails;
        private System.Windows.Forms.TextBox _eCustomerName;
        private System.Windows.Forms.Label _lCustomerName;
        private System.Windows.Forms.TextBox _eEmail;
        private System.Windows.Forms.Label _lContactEmail;
        private System.Windows.Forms.TextBox _ePhone;
        private System.Windows.Forms.Label _lContactPhone;
        private System.Windows.Forms.TextBox _eLastName;
        private System.Windows.Forms.Label _lContactLastName;
        private System.Windows.Forms.TextBox _eFirstName;
        private System.Windows.Forms.Label _lContactFirstName;
        private System.Windows.Forms.TextBox _eAddress;
        private System.Windows.Forms.Label _lAddress;
        private System.Windows.Forms.Label _lCountry;
        private System.Windows.Forms.ComboBox _cbCountries;
        private System.Windows.Forms.CheckBox _chbMaxSubsiteCount;
        private System.Windows.Forms.NumericUpDown _eCat12ComboCount;
        private System.Windows.Forms.Label _lCat12ComboCount;
        private System.Windows.Forms.NumericUpDown _nudMajorVersion;
        private System.Windows.Forms.Label _lMajorVersion;
        private System.Windows.Forms.CheckBox _chbTimetecMaster;
    }
}

