namespace Contal.Cgp.NCAS.Client
{
    partial class NCASTimetecSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASTimetecSettingsForm));
            this._gbUsedCardReaders = new System.Windows.Forms.GroupBox();
            this.elementHost = new System.Windows.Forms.Integration.ElementHost();
            this._wpfUsedCardReadersList = new Contal.Cgp.Components.WpfObjectListView();
            this._chbEnableCommunication = new System.Windows.Forms.CheckBox();
            this._gbTimetecConnectivitySettings = new System.Windows.Forms.GroupBox();
            this._chbDoNotImportDepartments = new System.Windows.Forms.CheckBox();
            this._bResetEventlogLastId = new System.Windows.Forms.Button();
            this._dtpDefaultStartDateTime = new System.Windows.Forms.DateTimePicker();
            this._lDefaultStartDate = new System.Windows.Forms.Label();
            this._mtbPassword = new System.Windows.Forms.MaskedTextBox();
            this._tbUserName = new System.Windows.Forms.TextBox();
            this._tbTimetecConnectivityState = new System.Windows.Forms.TextBox();
            this._lPassword = new System.Windows.Forms.Label();
            this._lOnlineState = new System.Windows.Forms.Label();
            this._lUserName = new System.Windows.Forms.Label();
            this._nPort = new System.Windows.Forms.NumericUpDown();
            this._tbIpAddress = new System.Windows.Forms.TextBox();
            this._lIpAddress = new System.Windows.Forms.Label();
            this._lPort = new System.Windows.Forms.Label();
            this._bSave = new System.Windows.Forms.Button();
            this._bClose = new System.Windows.Forms.Button();
            this._tcTimetecSettings = new System.Windows.Forms.TabControl();
            this._tpTimetecConnectivitySettings = new System.Windows.Forms.TabPage();
            this._tpEventRecovery = new System.Windows.Forms.TabPage();
            this._chbSelectAll = new System.Windows.Forms.CheckBox();
            this._bDelete = new System.Windows.Forms.Button();
            this._bResend = new System.Windows.Forms.Button();
            this._bRefresh = new System.Windows.Forms.Button();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this._wpfErrorEvents = new Contal.Cgp.Components.WpfObjectListView();
            this._gbUsedCardReaders.SuspendLayout();
            this._gbTimetecConnectivitySettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nPort)).BeginInit();
            this._tcTimetecSettings.SuspendLayout();
            this._tpTimetecConnectivitySettings.SuspendLayout();
            this._tpEventRecovery.SuspendLayout();
            this.SuspendLayout();
            // 
            // _gbUsedCardReaders
            // 
            this._gbUsedCardReaders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._gbUsedCardReaders.Controls.Add(this.elementHost);
            this._gbUsedCardReaders.Location = new System.Drawing.Point(6, 259);
            this._gbUsedCardReaders.Name = "_gbUsedCardReaders";
            this._gbUsedCardReaders.Size = new System.Drawing.Size(482, 259);
            this._gbUsedCardReaders.TabIndex = 14;
            this._gbUsedCardReaders.TabStop = false;
            this._gbUsedCardReaders.Text = "UsedCardReaders";
            // 
            // elementHost
            // 
            this.elementHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost.Location = new System.Drawing.Point(6, 19);
            this.elementHost.Name = "elementHost";
            this.elementHost.Size = new System.Drawing.Size(470, 234);
            this.elementHost.TabIndex = 0;
            this.elementHost.Text = "elementHost";
            this.elementHost.Child = this._wpfUsedCardReadersList;
            // 
            // _chbEnableCommunication
            // 
            this._chbEnableCommunication.AutoSize = true;
            this._chbEnableCommunication.Location = new System.Drawing.Point(6, 3);
            this._chbEnableCommunication.Name = "_chbEnableCommunication";
            this._chbEnableCommunication.Size = new System.Drawing.Size(245, 24);
            this._chbEnableCommunication.TabIndex = 13;
            this._chbEnableCommunication.Text = "EnableTimetecComminication";
            this._chbEnableCommunication.UseVisualStyleBackColor = true;
            this._chbEnableCommunication.CheckedChanged += new System.EventHandler(this.EditChanged);
            // 
            // _gbTimetecConnectivitySettings
            // 
            this._gbTimetecConnectivitySettings.Controls.Add(this._chbDoNotImportDepartments);
            this._gbTimetecConnectivitySettings.Controls.Add(this._bResetEventlogLastId);
            this._gbTimetecConnectivitySettings.Controls.Add(this._dtpDefaultStartDateTime);
            this._gbTimetecConnectivitySettings.Controls.Add(this._lDefaultStartDate);
            this._gbTimetecConnectivitySettings.Controls.Add(this._mtbPassword);
            this._gbTimetecConnectivitySettings.Controls.Add(this._tbUserName);
            this._gbTimetecConnectivitySettings.Controls.Add(this._tbTimetecConnectivityState);
            this._gbTimetecConnectivitySettings.Controls.Add(this._lPassword);
            this._gbTimetecConnectivitySettings.Controls.Add(this._lOnlineState);
            this._gbTimetecConnectivitySettings.Controls.Add(this._lUserName);
            this._gbTimetecConnectivitySettings.Controls.Add(this._nPort);
            this._gbTimetecConnectivitySettings.Controls.Add(this._tbIpAddress);
            this._gbTimetecConnectivitySettings.Controls.Add(this._lIpAddress);
            this._gbTimetecConnectivitySettings.Controls.Add(this._lPort);
            this._gbTimetecConnectivitySettings.Location = new System.Drawing.Point(6, 26);
            this._gbTimetecConnectivitySettings.Name = "_gbTimetecConnectivitySettings";
            this._gbTimetecConnectivitySettings.Size = new System.Drawing.Size(482, 227);
            this._gbTimetecConnectivitySettings.TabIndex = 12;
            this._gbTimetecConnectivitySettings.TabStop = false;
            this._gbTimetecConnectivitySettings.Text = "ConnectivitySettings";
            // 
            // _chbDoNotImportDepartments
            // 
            this._chbDoNotImportDepartments.AutoSize = true;
            this._chbDoNotImportDepartments.Location = new System.Drawing.Point(15, 190);
            this._chbDoNotImportDepartments.Name = "_chbDoNotImportDepartments";
            this._chbDoNotImportDepartments.Size = new System.Drawing.Size(232, 24);
            this._chbDoNotImportDepartments.TabIndex = 18;
            this._chbDoNotImportDepartments.Text = "Do Not Import Departments";
            this._chbDoNotImportDepartments.UseVisualStyleBackColor = true;
            this._chbDoNotImportDepartments.CheckedChanged += new System.EventHandler(this._chbDoNotImportDepartments_CheckedChanged);
            // 
            // _bResetEventlogLastId
            // 
            this._bResetEventlogLastId.Location = new System.Drawing.Point(325, 158);
            this._bResetEventlogLastId.Name = "_bResetEventlogLastId";
            this._bResetEventlogLastId.Size = new System.Drawing.Size(90, 38);
            this._bResetEventlogLastId.TabIndex = 17;
            this._bResetEventlogLastId.Text = "Reset";
            this._bResetEventlogLastId.UseVisualStyleBackColor = true;
            this._bResetEventlogLastId.Click += new System.EventHandler(this._bResetEventlogLastId_Click);
            // 
            // _dtpDefaultStartDateTime
            // 
            this._dtpDefaultStartDateTime.CustomFormat = "dd.MM.yyyy   HH:mm:ss";
            this._dtpDefaultStartDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._dtpDefaultStartDateTime.Location = new System.Drawing.Point(119, 160);
            this._dtpDefaultStartDateTime.Name = "_dtpDefaultStartDateTime";
            this._dtpDefaultStartDateTime.Size = new System.Drawing.Size(200, 26);
            this._dtpDefaultStartDateTime.TabIndex = 8;
            this._dtpDefaultStartDateTime.ValueChanged += new System.EventHandler(this.EditChanged);
            // 
            // _lDefaultStartDate
            // 
            this._lDefaultStartDate.AutoSize = true;
            this._lDefaultStartDate.Location = new System.Drawing.Point(13, 162);
            this._lDefaultStartDate.Name = "_lDefaultStartDate";
            this._lDefaultStartDate.Size = new System.Drawing.Size(131, 20);
            this._lDefaultStartDate.TabIndex = 7;
            this._lDefaultStartDate.Text = "DefaultStartDate";
            // 
            // _mtbPassword
            // 
            this._mtbPassword.Location = new System.Drawing.Point(119, 130);
            this._mtbPassword.Name = "_mtbPassword";
            this._mtbPassword.PasswordChar = '*';
            this._mtbPassword.Size = new System.Drawing.Size(151, 26);
            this._mtbPassword.TabIndex = 4;
            this._mtbPassword.TextChanged += new System.EventHandler(this.EditChanged);
            this._mtbPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this._mtbPassword_KeyDown);
            // 
            // _tbUserName
            // 
            this._tbUserName.Location = new System.Drawing.Point(119, 102);
            this._tbUserName.Name = "_tbUserName";
            this._tbUserName.Size = new System.Drawing.Size(151, 26);
            this._tbUserName.TabIndex = 3;
            this._tbUserName.TextChanged += new System.EventHandler(this.EditChanged);
            // 
            // _tbTimetecConnectivityState
            // 
            this._tbTimetecConnectivityState.Location = new System.Drawing.Point(119, 73);
            this._tbTimetecConnectivityState.Name = "_tbTimetecConnectivityState";
            this._tbTimetecConnectivityState.ReadOnly = true;
            this._tbTimetecConnectivityState.Size = new System.Drawing.Size(151, 26);
            this._tbTimetecConnectivityState.TabIndex = 6;
            this._tbTimetecConnectivityState.Text = "Unknown";
            // 
            // _lPassword
            // 
            this._lPassword.AutoSize = true;
            this._lPassword.Location = new System.Drawing.Point(13, 133);
            this._lPassword.Name = "_lPassword";
            this._lPassword.Size = new System.Drawing.Size(78, 20);
            this._lPassword.TabIndex = 1;
            this._lPassword.Text = "Password";
            // 
            // _lOnlineState
            // 
            this._lOnlineState.AutoSize = true;
            this._lOnlineState.Location = new System.Drawing.Point(12, 76);
            this._lOnlineState.Name = "_lOnlineState";
            this._lOnlineState.Size = new System.Drawing.Size(48, 20);
            this._lOnlineState.TabIndex = 5;
            this._lOnlineState.Text = "State";
            // 
            // _lUserName
            // 
            this._lUserName.AutoSize = true;
            this._lUserName.Location = new System.Drawing.Point(13, 105);
            this._lUserName.Name = "_lUserName";
            this._lUserName.Size = new System.Drawing.Size(85, 20);
            this._lUserName.TabIndex = 0;
            this._lUserName.Text = "UserName";
            // 
            // _nPort
            // 
            this._nPort.Location = new System.Drawing.Point(119, 45);
            this._nPort.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this._nPort.Name = "_nPort";
            this._nPort.Size = new System.Drawing.Size(84, 26);
            this._nPort.TabIndex = 4;
            this._nPort.Value = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this._nPort.ValueChanged += new System.EventHandler(this.EditChanged);
            // 
            // _tbIpAddress
            // 
            this._tbIpAddress.Location = new System.Drawing.Point(119, 19);
            this._tbIpAddress.Name = "_tbIpAddress";
            this._tbIpAddress.Size = new System.Drawing.Size(151, 26);
            this._tbIpAddress.TabIndex = 2;
            this._tbIpAddress.TextChanged += new System.EventHandler(this.EditChanged);
            // 
            // _lIpAddress
            // 
            this._lIpAddress.AutoSize = true;
            this._lIpAddress.Location = new System.Drawing.Point(12, 22);
            this._lIpAddress.Name = "_lIpAddress";
            this._lIpAddress.Size = new System.Drawing.Size(82, 20);
            this._lIpAddress.TabIndex = 0;
            this._lIpAddress.Text = "IpAddress";
            // 
            // _lPort
            // 
            this._lPort.AutoSize = true;
            this._lPort.Location = new System.Drawing.Point(12, 48);
            this._lPort.Name = "_lPort";
            this._lPort.Size = new System.Drawing.Size(38, 20);
            this._lPort.TabIndex = 1;
            this._lPort.Text = "Port";
            // 
            // _bSave
            // 
            this._bSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bSave.Location = new System.Drawing.Point(597, 566);
            this._bSave.Name = "_bSave";
            this._bSave.Size = new System.Drawing.Size(90, 38);
            this._bSave.TabIndex = 15;
            this._bSave.Text = "Save";
            this._bSave.UseVisualStyleBackColor = true;
            this._bSave.Click += new System.EventHandler(this._bSave_Click);
            // 
            // _bClose
            // 
            this._bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bClose.Location = new System.Drawing.Point(678, 566);
            this._bClose.Name = "_bClose";
            this._bClose.Size = new System.Drawing.Size(90, 38);
            this._bClose.TabIndex = 16;
            this._bClose.Text = "Close";
            this._bClose.UseVisualStyleBackColor = true;
            this._bClose.Click += new System.EventHandler(this._bClose_Click);
            // 
            // _tcTimetecSettings
            // 
            this._tcTimetecSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tcTimetecSettings.Controls.Add(this._tpTimetecConnectivitySettings);
            this._tcTimetecSettings.Controls.Add(this._tpEventRecovery);
            this._tcTimetecSettings.Location = new System.Drawing.Point(3, 3);
            this._tcTimetecSettings.Name = "_tcTimetecSettings";
            this._tcTimetecSettings.SelectedIndex = 0;
            this._tcTimetecSettings.Size = new System.Drawing.Size(750, 557);
            this._tcTimetecSettings.TabIndex = 17;
            // 
            // _tpTimetecConnectivitySettings
            // 
            this._tpTimetecConnectivitySettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpTimetecConnectivitySettings.Controls.Add(this._chbEnableCommunication);
            this._tpTimetecConnectivitySettings.Controls.Add(this._gbTimetecConnectivitySettings);
            this._tpTimetecConnectivitySettings.Controls.Add(this._gbUsedCardReaders);
            this._tpTimetecConnectivitySettings.Location = new System.Drawing.Point(4, 29);
            this._tpTimetecConnectivitySettings.Name = "_tpTimetecConnectivitySettings";
            this._tpTimetecConnectivitySettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpTimetecConnectivitySettings.Size = new System.Drawing.Size(742, 524);
            this._tpTimetecConnectivitySettings.TabIndex = 0;
            this._tpTimetecConnectivitySettings.Text = "ConnectivitySettings";
            // 
            // _tpEventRecovery
            // 
            this._tpEventRecovery.BackColor = System.Drawing.SystemColors.Control;
            this._tpEventRecovery.Controls.Add(this._chbSelectAll);
            this._tpEventRecovery.Controls.Add(this._bDelete);
            this._tpEventRecovery.Controls.Add(this._bResend);
            this._tpEventRecovery.Controls.Add(this._bRefresh);
            this._tpEventRecovery.Controls.Add(this.elementHost1);
            this._tpEventRecovery.Location = new System.Drawing.Point(4, 29);
            this._tpEventRecovery.Name = "_tpEventRecovery";
            this._tpEventRecovery.Padding = new System.Windows.Forms.Padding(3);
            this._tpEventRecovery.Size = new System.Drawing.Size(742, 524);
            this._tpEventRecovery.TabIndex = 1;
            this._tpEventRecovery.Text = "EventRecovery";
            // 
            // _chbSelectAll
            // 
            this._chbSelectAll.AutoSize = true;
            this._chbSelectAll.Location = new System.Drawing.Point(6, 25);
            this._chbSelectAll.Name = "_chbSelectAll";
            this._chbSelectAll.Size = new System.Drawing.Size(97, 24);
            this._chbSelectAll.TabIndex = 20;
            this._chbSelectAll.Text = "SelectAll";
            this._chbSelectAll.UseVisualStyleBackColor = true;
            this._chbSelectAll.CheckedChanged += new System.EventHandler(this._chbSelectAll_CheckedChanged);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDelete.Location = new System.Drawing.Point(87, 473);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(90, 38);
            this._bDelete.TabIndex = 19;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bResend
            // 
            this._bResend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bResend.Location = new System.Drawing.Point(6, 473);
            this._bResend.Name = "_bResend";
            this._bResend.Size = new System.Drawing.Size(90, 38);
            this._bResend.TabIndex = 18;
            this._bResend.Text = "Resend";
            this._bResend.UseVisualStyleBackColor = true;
            this._bResend.Click += new System.EventHandler(this._bResend_Click);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(661, 473);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(90, 38);
            this._bRefresh.TabIndex = 17;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost1.Location = new System.Drawing.Point(6, 48);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(730, 419);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this._wpfErrorEvents;
            // 
            // NCASTimetecSettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(759, 601);
            this.Controls.Add(this._tcTimetecSettings);
            this.Controls.Add(this._bClose);
            this.Controls.Add(this._bSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NCASTimetecSettingsForm";
            this.Text = "NCASTimetecSettingsForm";
            this._gbUsedCardReaders.ResumeLayout(false);
            this._gbTimetecConnectivitySettings.ResumeLayout(false);
            this._gbTimetecConnectivitySettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nPort)).EndInit();
            this._tcTimetecSettings.ResumeLayout(false);
            this._tpTimetecConnectivitySettings.ResumeLayout(false);
            this._tpTimetecConnectivitySettings.PerformLayout();
            this._tpEventRecovery.ResumeLayout(false);
            this._tpEventRecovery.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _gbUsedCardReaders;
        private System.Windows.Forms.Integration.ElementHost elementHost;
        private System.Windows.Forms.CheckBox _chbEnableCommunication;
        private System.Windows.Forms.GroupBox _gbTimetecConnectivitySettings;
        private System.Windows.Forms.MaskedTextBox _mtbPassword;
        private System.Windows.Forms.TextBox _tbUserName;
        private System.Windows.Forms.TextBox _tbTimetecConnectivityState;
        private System.Windows.Forms.Label _lPassword;
        private System.Windows.Forms.Label _lOnlineState;
        private System.Windows.Forms.Label _lUserName;
        private System.Windows.Forms.NumericUpDown _nPort;
        private System.Windows.Forms.TextBox _tbIpAddress;
        private System.Windows.Forms.Label _lIpAddress;
        private System.Windows.Forms.Label _lPort;
        private System.Windows.Forms.Button _bSave;
        private Cgp.Components.WpfObjectListView _wpfUsedCardReadersList;
        private System.Windows.Forms.Button _bClose;
        private System.Windows.Forms.DateTimePicker _dtpDefaultStartDateTime;
        private System.Windows.Forms.Label _lDefaultStartDate;
        private System.Windows.Forms.Button _bResetEventlogLastId;
        private System.Windows.Forms.TabControl _tcTimetecSettings;
        private System.Windows.Forms.TabPage _tpTimetecConnectivitySettings;
        private System.Windows.Forms.TabPage _tpEventRecovery;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private Contal.Cgp.Components.WpfObjectListView _wpfErrorEvents;
        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.CheckBox _chbSelectAll;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bResend;
        private System.Windows.Forms.CheckBox _chbDoNotImportDepartments;
    }
}
