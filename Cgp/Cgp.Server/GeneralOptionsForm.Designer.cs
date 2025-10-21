namespace Contal.Cgp.Server
{
    partial class GeneralOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralOptionsForm));
            this._eFriendlyName = new System.Windows.Forms.TextBox();
            this._lFriendlyName = new System.Windows.Forms.Label();
            this._lServerPort = new System.Windows.Forms.Label();
            this._bSave = new System.Windows.Forms.Button();
            this._bClose = new System.Windows.Forms.Button();
            this._eServerPort = new System.Windows.Forms.NumericUpDown();
            this._labelLicenceFilePath = new System.Windows.Forms.Label();
            this._lRemotingIpAddress = new System.Windows.Forms.Label();
            this._lbAvailableLicences = new System.Windows.Forms.ListBox();
            this._bRefreshLicenceList = new System.Windows.Forms.Button();
            this._chbSaveAndSkipDB = new System.Windows.Forms.CheckBox();
            this._eRemotingIpAddress = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._eServerPort)).BeginInit();
            this.SuspendLayout();
            // 
            // _eFriendlyName
            // 
            this._eFriendlyName.Location = new System.Drawing.Point(15, 130);
            this._eFriendlyName.Margin = new System.Windows.Forms.Padding(4);
            this._eFriendlyName.Name = "_eFriendlyName";
            this._eFriendlyName.Size = new System.Drawing.Size(191, 20);
            this._eFriendlyName.TabIndex = 3;
            // 
            // _lFriendlyName
            // 
            this._lFriendlyName.AutoSize = true;
            this._lFriendlyName.Enabled = false;
            this._lFriendlyName.Location = new System.Drawing.Point(13, 109);
            this._lFriendlyName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFriendlyName.Name = "_lFriendlyName";
            this._lFriendlyName.Size = new System.Drawing.Size(103, 13);
            this._lFriendlyName.TabIndex = 2;
            this._lFriendlyName.Text = "Server firendly name";
            // 
            // _lServerPort
            // 
            this._lServerPort.AutoSize = true;
            this._lServerPort.Enabled = false;
            this._lServerPort.Location = new System.Drawing.Point(13, 11);
            this._lServerPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServerPort.Name = "_lServerPort";
            this._lServerPort.Size = new System.Drawing.Size(105, 13);
            this._lServerPort.TabIndex = 0;
            this._lServerPort.Text = "Remoting server port";
            // 
            // _bSave
            // 
            this._bSave.Location = new System.Drawing.Point(16, 232);
            this._bSave.Margin = new System.Windows.Forms.Padding(4);
            this._bSave.Name = "_bSave";
            this._bSave.Size = new System.Drawing.Size(100, 28);
            this._bSave.TabIndex = 8;
            this._bSave.Text = "Save";
            this._bSave.UseVisualStyleBackColor = true;
            this._bSave.Click += new System.EventHandler(this._bSave_Click);
            // 
            // _bClose
            // 
            this._bClose.Location = new System.Drawing.Point(422, 232);
            this._bClose.Margin = new System.Windows.Forms.Padding(4);
            this._bClose.Name = "_bClose";
            this._bClose.Size = new System.Drawing.Size(100, 28);
            this._bClose.TabIndex = 9;
            this._bClose.Text = "Close";
            this._bClose.UseVisualStyleBackColor = true;
            this._bClose.Click += new System.EventHandler(this._bClose_Click);
            // 
            // _eServerPort
            // 
            this._eServerPort.Location = new System.Drawing.Point(15, 32);
            this._eServerPort.Margin = new System.Windows.Forms.Padding(4);
            this._eServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._eServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eServerPort.Name = "_eServerPort";
            this._eServerPort.Size = new System.Drawing.Size(192, 20);
            this._eServerPort.TabIndex = 1;
            this._eServerPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _labelLicenceFilePath
            // 
            this._labelLicenceFilePath.AutoSize = true;
            this._labelLicenceFilePath.Location = new System.Drawing.Point(244, 57);
            this._labelLicenceFilePath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._labelLicenceFilePath.Name = "_labelLicenceFilePath";
            this._labelLicenceFilePath.Size = new System.Drawing.Size(61, 13);
            this._labelLicenceFilePath.TabIndex = 11;
            this._labelLicenceFilePath.Text = "Licence file";
            // 
            // _lRemotingIpAddress
            // 
            this._lRemotingIpAddress.AutoSize = true;
            this._lRemotingIpAddress.Enabled = false;
            this._lRemotingIpAddress.Location = new System.Drawing.Point(12, 61);
            this._lRemotingIpAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lRemotingIpAddress.Name = "_lRemotingIpAddress";
            this._lRemotingIpAddress.Size = new System.Drawing.Size(132, 13);
            this._lRemotingIpAddress.TabIndex = 13;
            this._lRemotingIpAddress.Text = "Bind to specific IP address";
            // 
            // _lbAvailableLicences
            // 
            this._lbAvailableLicences.FormattingEnabled = true;
            this._lbAvailableLicences.Location = new System.Drawing.Point(247, 82);
            this._lbAvailableLicences.Margin = new System.Windows.Forms.Padding(4);
            this._lbAvailableLicences.Name = "_lbAvailableLicences";
            this._lbAvailableLicences.Size = new System.Drawing.Size(275, 95);
            this._lbAvailableLicences.TabIndex = 14;
            this._lbAvailableLicences.SelectedValueChanged += new System.EventHandler(this._lbAvailableLicences_SelectedValueChanged);
            // 
            // _bRefreshLicenceList
            // 
            this._bRefreshLicenceList.Location = new System.Drawing.Point(422, 50);
            this._bRefreshLicenceList.Margin = new System.Windows.Forms.Padding(4);
            this._bRefreshLicenceList.Name = "_bRefreshLicenceList";
            this._bRefreshLicenceList.Size = new System.Drawing.Size(100, 30);
            this._bRefreshLicenceList.TabIndex = 15;
            this._bRefreshLicenceList.Text = "Refresh";
            this._bRefreshLicenceList.UseVisualStyleBackColor = true;
            this._bRefreshLicenceList.Click += new System.EventHandler(this._bRefreshLicenceList_Click);
            // 
            // _chbSaveAndSkipDB
            // 
            this._chbSaveAndSkipDB.AutoSize = true;
            this._chbSaveAndSkipDB.Location = new System.Drawing.Point(123, 237);
            this._chbSaveAndSkipDB.Name = "_chbSaveAndSkipDB";
            this._chbSaveAndSkipDB.Size = new System.Drawing.Size(174, 21);
            this._chbSaveAndSkipDB.TabIndex = 16;
            this._chbSaveAndSkipDB.Text = "Skip database reconfiguration";
            this._chbSaveAndSkipDB.UseVisualStyleBackColor = true;
            this._chbSaveAndSkipDB.CheckedChanged += new System.EventHandler(this._chbSaveAndSkipDB_CheckedChanged);
            // 
            // _eRemotingIpAddress
            // 
            this._eRemotingIpAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._eRemotingIpAddress.FormattingEnabled = true;
            this._eRemotingIpAddress.Items.AddRange(new object[] {
            ""});
            this._eRemotingIpAddress.Location = new System.Drawing.Point(15, 82);
            this._eRemotingIpAddress.Name = "_eRemotingIpAddress";
            this._eRemotingIpAddress.Size = new System.Drawing.Size(190, 21);
            this._eRemotingIpAddress.TabIndex = 17;
            this._eRemotingIpAddress.SelectedIndexChanged += new System.EventHandler(this._eRemotingIpAddress_SelectedIndexChanged);
            // 
            // GeneralOptionsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(535, 343);
            this.ControlBox = false;
            this.Controls.Add(this._eRemotingIpAddress);
            this.Controls.Add(this._chbSaveAndSkipDB);
            this.Controls.Add(this._bRefreshLicenceList);
            this.Controls.Add(this._lbAvailableLicences);
            this.Controls.Add(this._lRemotingIpAddress);
            this.Controls.Add(this._labelLicenceFilePath);
            this.Controls.Add(this._eServerPort);
            this.Controls.Add(this._bClose);
            this.Controls.Add(this._bSave);
            this.Controls.Add(this._eFriendlyName);
            this.Controls.Add(this._lFriendlyName);
            this.Controls.Add(this._lServerPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GeneralOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contal Nova Server : General options";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GeneralOptionsForm_Load);
            this.Shown += new System.EventHandler(this.GeneralOptionsForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this._eServerPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _eFriendlyName;
        private System.Windows.Forms.Label _lFriendlyName;
        private System.Windows.Forms.Label _lServerPort;
        private System.Windows.Forms.Button _bSave;
        private System.Windows.Forms.Button _bClose;
        private System.Windows.Forms.NumericUpDown _eServerPort;
        private System.Windows.Forms.Label _labelLicenceFilePath;
        private System.Windows.Forms.Label _lRemotingIpAddress;
        private System.Windows.Forms.ListBox _lbAvailableLicences;
        private System.Windows.Forms.Button _bRefreshLicenceList;
        private System.Windows.Forms.CheckBox _chbSaveAndSkipDB;
        private System.Windows.Forms.ComboBox _eRemotingIpAddress;
    }
}
