namespace Contal.Cgp.Client
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralOptionsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this._tcGeneralOptions = new System.Windows.Forms.TabControl();
            this._tpLicence = new System.Windows.Forms.TabPage();
            this._bInsertLicenceFile = new System.Windows.Forms.Button();
            this._pBack = new System.Windows.Forms.Panel();
            this._epgLicenceInfo = new Contal.IwQuick.UI.ExtendedPropertyGrid();
            this._buttonLicenceRequest = new System.Windows.Forms.Button();
            this._lFunctionalPropertiesText = new System.Windows.Forms.Label();
            this._lExpirationDateText = new System.Windows.Forms.Label();
            this._lLicenceFileText = new System.Windows.Forms.Label();
            this._tpServerControl = new System.Windows.Forms.TabPage();
            this._gbServerInformations = new System.Windows.Forms.GroupBox();
            this._bRefresh = new System.Windows.Forms.Button();
            this._dgServerInformations = new System.Windows.Forms.DataGridView();
            this._gbResetUserSession = new System.Windows.Forms.GroupBox();
            this._bResetUserSession = new System.Windows.Forms.Button();
            this._lLogin = new System.Windows.Forms.Label();
            this._tbmLogin = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify3 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove3 = new System.Windows.Forms.ToolStripMenuItem();
            this._bDBTest = new System.Windows.Forms.Button();
            this._lStopServer = new System.Windows.Forms.Label();
            this._bStop = new System.Windows.Forms.Button();
            this._tpRemoteServicesSettings = new System.Windows.Forms.TabPage();
            this._gbNtpSettings = new System.Windows.Forms.GroupBox();
            this._nudTimeDiffTolerance = new System.Windows.Forms.NumericUpDown();
            this._lTimeDiffTolerance = new System.Windows.Forms.Label();
            this._bResolve = new System.Windows.Forms.Button();
            this._lIpAddress = new System.Windows.Forms.Label();
            this._eDnsHostName = new System.Windows.Forms.TextBox();
            this._eNtpIpAddress = new System.Windows.Forms.TextBox();
            this._lDnsHostname1 = new System.Windows.Forms.Label();
            this._bAdd = new System.Windows.Forms.Button();
            this._lNtpIpAddresses = new System.Windows.Forms.ListBox();
            this._bDelete2 = new System.Windows.Forms.Button();
            this._gbSmtpSettings = new System.Windows.Forms.GroupBox();
            this._epgSMTPServerSettings = new Contal.IwQuick.UI.ExtendedPropertyGrid();
            this._bSaveSmtp = new System.Windows.Forms.Button();
            this._tpDHCPServer = new System.Windows.Forms.TabPage();
            this._bStopDHCP = new System.Windows.Forms.Button();
            this._bStartDHCP = new System.Windows.Forms.Button();
            this._chbAutoStartDHCP = new System.Windows.Forms.CheckBox();
            this._gbIPGroup = new System.Windows.Forms.GroupBox();
            this._bDeleteIpGroup = new System.Windows.Forms.Button();
            this._gbNetwork = new System.Windows.Forms.GroupBox();
            this._tbSubnet = new System.Windows.Forms.TextBox();
            this._lSubnet = new System.Windows.Forms.Label();
            this._tbNetworkMask = new System.Windows.Forms.TextBox();
            this._lSubnetMask = new System.Windows.Forms.Label();
            this._bDHCPSave = new System.Windows.Forms.Button();
            this._tbMaxLeaseTime = new System.Windows.Forms.TextBox();
            this._tbLeaseTime = new System.Windows.Forms.TextBox();
            this._tbGateway = new System.Windows.Forms.TextBox();
            this._lMaxLeaseTime = new System.Windows.Forms.Label();
            this._gbRange = new System.Windows.Forms.GroupBox();
            this._buttonSetMacGroup = new System.Windows.Forms.Button();
            this._bMaxRange = new System.Windows.Forms.Button();
            this._chbEnable = new System.Windows.Forms.CheckBox();
            this._lIPRangeFrom = new System.Windows.Forms.Label();
            this._lIPRangeTo = new System.Windows.Forms.Label();
            this._cbMACMask = new System.Windows.Forms.ComboBox();
            this._tbIPRangeFrom = new System.Windows.Forms.TextBox();
            this._lMACMask = new System.Windows.Forms.Label();
            this._tbIPRangeTo = new System.Windows.Forms.TextBox();
            this._lLeaseTime = new System.Windows.Forms.Label();
            this._gbDNS = new System.Windows.Forms.GroupBox();
            this._lDNSSuffix = new System.Windows.Forms.Label();
            this._lAlternateDNSServer = new System.Windows.Forms.Label();
            this._tbDnsSuffix = new System.Windows.Forms.TextBox();
            this._lPrefferedDNSServer = new System.Windows.Forms.Label();
            this._tbAlternateDNS = new System.Windows.Forms.TextBox();
            this._tbDNS = new System.Windows.Forms.TextBox();
            this._lGateway = new System.Windows.Forms.Label();
            this._cbIPGroup = new System.Windows.Forms.ComboBox();
            this._lIPGroup = new System.Windows.Forms.Label();
            this._tpSerialPort = new System.Windows.Forms.TabPage();
            this._bSaveSerialPort = new System.Windows.Forms.Button();
            this._ePortPin = new System.Windows.Forms.TextBox();
            this._lPin = new System.Windows.Forms.Label();
            this._chbPortCarrierDetect = new System.Windows.Forms.CheckBox();
            this._chbPortParityCheck = new System.Windows.Forms.CheckBox();
            this._PortNumber = new System.Windows.Forms.Label();
            this._cbPortFlowControl = new System.Windows.Forms.ComboBox();
            this._lFlowControl = new System.Windows.Forms.Label();
            this._cbPort = new System.Windows.Forms.ComboBox();
            this._lBaud = new System.Windows.Forms.Label();
            this._cbPortStopBits = new System.Windows.Forms.ComboBox();
            this._cbPortBaudRate = new System.Windows.Forms.ComboBox();
            this._cbPortDataBits = new System.Windows.Forms.ComboBox();
            this._lStopBits = new System.Windows.Forms.Label();
            this._lBits = new System.Windows.Forms.Label();
            this._lParity = new System.Windows.Forms.Label();
            this._cbPortParity = new System.Windows.Forms.ComboBox();
            this._tpDatabaseOptions = new System.Windows.Forms.TabPage();
            this._gbEventLogExpiration = new System.Windows.Forms.GroupBox();
            this._bSaveEventlogExpiration = new System.Windows.Forms.Button();
            this._lCalculatedValue = new System.Windows.Forms.Label();
            this._lExponent = new System.Windows.Forms.Label();
            this._gbTimeZone1 = new System.Windows.Forms.GroupBox();
            this._tbmEventlogTimeZone = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate1 = new System.Windows.Forms.ToolStripMenuItem();
            this._lInfoEventlogExpirationTimeZone = new System.Windows.Forms.Label();
            this._bForceEvenlogExpirationClean = new System.Windows.Forms.Button();
            this._eResultMaxEventlogRecords = new System.Windows.Forms.TextBox();
            this._lMaxEventlogsRecords = new System.Windows.Forms.Label();
            this._tbMaxEventlogSlider = new System.Windows.Forms.TrackBar();
            this._eMaxEventlogRecords = new System.Windows.Forms.NumericUpDown();
            this._lDaysExpiration = new System.Windows.Forms.Label();
            this._eCountDaysExpiration = new System.Windows.Forms.NumericUpDown();
            this._gbDatabaseBackup = new System.Windows.Forms.GroupBox();
            this._bClearAndSaveDbsBackup = new System.Windows.Forms.Button();
            this._gbBackupPath = new System.Windows.Forms.GroupBox();
            this._bDbsBackupPath = new System.Windows.Forms.Button();
            this._eDbsBackupPath = new System.Windows.Forms.TextBox();
            this._pgForceDbsBackup = new System.Windows.Forms.ProgressBar();
            this._bSaveDatabaseBackup = new System.Windows.Forms.Button();
            this._gbTimeZone = new System.Windows.Forms.GroupBox();
            this._tbmTimeZone = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate = new System.Windows.Forms.ToolStripMenuItem();
            this._lInfoDatabaseBackup = new System.Windows.Forms.Label();
            this._bForceDBSBackup = new System.Windows.Forms.Button();
            this._tpSecuritySettings = new System.Windows.Forms.TabPage();
            this._epgSecuritySettings = new Contal.IwQuick.UI.ExtendedPropertyGrid();
            this._buttonSaveSecuritySettings = new System.Windows.Forms.Button();
            this._tpAlarmSettings = new System.Windows.Forms.TabPage();
            this._cdgvAlarmSettings = new Contal.Cgp.Components.CgpDataGridView();
            this._bSave5 = new System.Windows.Forms.Button();
            this._tpUiSettings = new System.Windows.Forms.TabPage();
            this._gbAutoclose = new System.Windows.Forms.GroupBox();
            this._buttonSave = new System.Windows.Forms.Button();
            this._lAutocloseRange = new System.Windows.Forms.Label();
            this._labelTimeout = new System.Windows.Forms.Label();
            this._cbAutocCoseTurnedOn = new System.Windows.Forms.CheckBox();
            this._nUpDoAutoCloseTimeout = new System.Windows.Forms.NumericUpDown();
            this._gbColorSettings = new System.Windows.Forms.GroupBox();
            this._gbAlarmNotAcknowledged = new System.Windows.Forms.GroupBox();
            this._bAlarmNotAcknowledgedColorBackground = new System.Windows.Forms.Button();
            this._bAlarmNotAcknowledgedColorText = new System.Windows.Forms.Button();
            this._eAlarmNotAcknowledgedPreview = new System.Windows.Forms.TextBox();
            this._lColourBackground = new System.Windows.Forms.Label();
            this._lColourText = new System.Windows.Forms.Label();
            this._gbAlarmColour = new System.Windows.Forms.GroupBox();
            this._bAlarmColorBackground = new System.Windows.Forms.Button();
            this._bAlarmColorText = new System.Windows.Forms.Button();
            this._eAlarmPreview = new System.Windows.Forms.TextBox();
            this._lColourBackground1 = new System.Windows.Forms.Label();
            this._lColourText1 = new System.Windows.Forms.Label();
            this._bSave = new System.Windows.Forms.Button();
            this._gbNormalNotAcknowledgedColour = new System.Windows.Forms.GroupBox();
            this._bNormalNotAcknowledgedColorBackground = new System.Windows.Forms.Button();
            this._bNormalNotAcknowledgedColorText = new System.Windows.Forms.Button();
            this._eNormalNotAcknowledgedPreview = new System.Windows.Forms.TextBox();
            this._lColourBackground2 = new System.Windows.Forms.Label();
            this._lColourText2 = new System.Windows.Forms.Label();
            this._gbReferenceObjectsColour = new System.Windows.Forms.GroupBox();
            this._bReferenceObjectColorBackground = new System.Windows.Forms.Button();
            this._bReferenceObjectColorText = new System.Windows.Forms.Button();
            this._eReferenceObjectsPreview = new System.Windows.Forms.TextBox();
            this._lColourBackground6 = new System.Windows.Forms.Label();
            this._lColourText6 = new System.Windows.Forms.Label();
            this._gbNormalColour = new System.Windows.Forms.GroupBox();
            this._bNormalColorBackground = new System.Windows.Forms.Button();
            this._bNormalColorText = new System.Windows.Forms.Button();
            this._eNormalPreview = new System.Windows.Forms.TextBox();
            this._lColourBackground3 = new System.Windows.Forms.Label();
            this._lColourText3 = new System.Windows.Forms.Label();
            this._gbDropDownColour = new System.Windows.Forms.GroupBox();
            this._bDragDropColorBackground = new System.Windows.Forms.Button();
            this._bDragDropColorText = new System.Windows.Forms.Button();
            this._eDropDownPreview = new System.Windows.Forms.TextBox();
            this._lColourBackground5 = new System.Windows.Forms.Label();
            this._lColourText5 = new System.Windows.Forms.Label();
            this._gbNoAlarmsInQueueColour = new System.Windows.Forms.GroupBox();
            this._bNoAlarmsInQueueColorBackground = new System.Windows.Forms.Button();
            this._bNoAlarmsInQueueColorText = new System.Windows.Forms.Button();
            this._eNoAlarmQueuePreview = new System.Windows.Forms.TextBox();
            this._lColourBackground4 = new System.Windows.Forms.Label();
            this._lColourText4 = new System.Windows.Forms.Label();
            this._tpEventlogs = new System.Windows.Forms.TabPage();
            this._gbEventlogReports = new System.Windows.Forms.GroupBox();
            this._lblReportsEmails = new System.Windows.Forms.Label();
            this._lblReportsTimezone = new System.Windows.Forms.Label();
            this._eEventlogsReportsEmails = new System.Windows.Forms.TextBox();
            this._tbmEventlogsReportsTimezone = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._cbEventSourcesReverseOrder = new System.Windows.Forms.CheckBox();
            this._bEventlogsSave = new System.Windows.Forms.Button();
            this._cbEventlogCardReaderOnlineStateChanged = new System.Windows.Forms.CheckBox();
            this._cbEventlogAlarmAreaActivationStateChanged = new System.Windows.Forms.CheckBox();
            this._cbEventlogAlarmAreaAlarmStateChanged = new System.Windows.Forms.CheckBox();
            this._cbEventlogOutputStateChanged = new System.Windows.Forms.CheckBox();
            this._cbEventlogInputStateChanged = new System.Windows.Forms.CheckBox();
            this._tpLanguage = new System.Windows.Forms.TabPage();
            this._bSave2 = new System.Windows.Forms.Button();
            this._tpAdvancedAccessSettings = new System.Windows.Forms.TabPage();
            this._chbAlarmAreaRestrictivePolicyForTimeBuying = new System.Windows.Forms.CheckBox();
            this._chbEnableLoggingSDPSTZChanges = new System.Windows.Forms.CheckBox();
            this._gbSyncingOfTimeFromServerSettings = new System.Windows.Forms.GroupBox();
            this._ePeriodicTimeSyncTolerance = new System.Windows.Forms.NumericUpDown();
            this._lPeriodicTimeSyncTolerance = new System.Windows.Forms.Label();
            this._ePeriodOfTimeSyncWithoutStratum = new System.Windows.Forms.NumericUpDown();
            this._lPeriodOfTimeSyncWithoutStratum = new System.Windows.Forms.Label();
            this._cbSyncingTimeFromServer = new System.Windows.Forms.CheckBox();
            this._bSave3 = new System.Windows.Forms.Button();
            this._tpAdvancedSettings = new System.Windows.Forms.TabPage();
            this._lDelayForSendingChangesToCcu = new System.Windows.Forms.Label();
            this._eDelayForSendingChangesToCcu = new System.Windows.Forms.NumericUpDown();
            this._lAlarmListSuspendedRefreshTimeout = new System.Windows.Forms.Label();
            this._eAlarmListSuspendedRefreshTimeout = new System.Windows.Forms.NumericUpDown();
            this._cbCorrectDeserializationFailures = new System.Windows.Forms.CheckBox();
            this._lClientSessionTimeout = new System.Windows.Forms.Label();
            this._eClientSessionTimeout = new System.Windows.Forms.NumericUpDown();
            this._bSave4 = new System.Windows.Forms.Button();
            this._lDelayForSaveEvents = new System.Windows.Forms.Label();
            this._eDelayForSaveEvents = new System.Windows.Forms.NumericUpDown();
            this._lMaxEventsCountForInsert = new System.Windows.Forms.Label();
            this._eMaxEventsCountForInsert = new System.Windows.Forms.NumericUpDown();
            this._tpCustomerSupplierInfo = new System.Windows.Forms.TabPage();
            this._bAdd8 = new System.Windows.Forms.Button();
            this._bRemove = new System.Windows.Forms.Button();
            this._lSupplierLogo = new System.Windows.Forms.Label();
            this._pbSupplierLogo = new System.Windows.Forms.PictureBox();
            this._gbSupplierInfo = new System.Windows.Forms.GroupBox();
            this._epgSupplierInfo = new Contal.IwQuick.UI.ExtendedPropertyGrid();
            this._gbCustomerInfo = new System.Windows.Forms.GroupBox();
            this._epgCustomerInfo = new Contal.IwQuick.UI.ExtendedPropertyGrid();
            this._bSave6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._fbDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._colourDialog = new System.Windows.Forms.ColorDialog();
            this._lIPAddress1 = new System.Windows.Forms.Label();
            this._bDelete1 = new System.Windows.Forms.Button();
            this._eSNTPDNSHostName = new System.Windows.Forms.TextBox();
            this._lSNTPDNSHostNames = new System.Windows.Forms.ListBox();
            this._eSNTPIpAddress = new System.Windows.Forms.TextBox();
            this._lDNSHostname = new System.Windows.Forms.Label();
            this._lSNTPIPAddresses = new System.Windows.Forms.ListBox();
            this._bDelete = new System.Windows.Forms.Button();
            this._ofdBrowseImage = new System.Windows.Forms.OpenFileDialog();
            this._tcGeneralOptions.SuspendLayout();
            this._tpLicence.SuspendLayout();
            this._pBack.SuspendLayout();
            this._tpServerControl.SuspendLayout();
            this._gbServerInformations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgServerInformations)).BeginInit();
            this._gbResetUserSession.SuspendLayout();
            this._tpRemoteServicesSettings.SuspendLayout();
            this._gbNtpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudTimeDiffTolerance)).BeginInit();
            this._gbSmtpSettings.SuspendLayout();
            this._tpDHCPServer.SuspendLayout();
            this._gbIPGroup.SuspendLayout();
            this._gbNetwork.SuspendLayout();
            this._gbRange.SuspendLayout();
            this._gbDNS.SuspendLayout();
            this._tpSerialPort.SuspendLayout();
            this._tpDatabaseOptions.SuspendLayout();
            this._gbEventLogExpiration.SuspendLayout();
            this._gbTimeZone1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tbMaxEventlogSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eMaxEventlogRecords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eCountDaysExpiration)).BeginInit();
            this._gbDatabaseBackup.SuspendLayout();
            this._gbBackupPath.SuspendLayout();
            this._gbTimeZone.SuspendLayout();
            this._tpSecuritySettings.SuspendLayout();
            this._tpAlarmSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvAlarmSettings.DataGrid)).BeginInit();
            this._tpUiSettings.SuspendLayout();
            this._gbAutoclose.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nUpDoAutoCloseTimeout)).BeginInit();
            this._gbColorSettings.SuspendLayout();
            this._gbAlarmNotAcknowledged.SuspendLayout();
            this._gbAlarmColour.SuspendLayout();
            this._gbNormalNotAcknowledgedColour.SuspendLayout();
            this._gbReferenceObjectsColour.SuspendLayout();
            this._gbNormalColour.SuspendLayout();
            this._gbDropDownColour.SuspendLayout();
            this._gbNoAlarmsInQueueColour.SuspendLayout();
            this._tpEventlogs.SuspendLayout();
            this._gbEventlogReports.SuspendLayout();
            this._tpLanguage.SuspendLayout();
            this._tpAdvancedAccessSettings.SuspendLayout();
            this._gbSyncingOfTimeFromServerSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._ePeriodicTimeSyncTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._ePeriodOfTimeSyncWithoutStratum)).BeginInit();
            this._tpAdvancedSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayForSendingChangesToCcu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eAlarmListSuspendedRefreshTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eClientSessionTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayForSaveEvents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eMaxEventsCountForInsert)).BeginInit();
            this._tpCustomerSupplierInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbSupplierLogo)).BeginInit();
            this._gbSupplierInfo.SuspendLayout();
            this._gbCustomerInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // _tcGeneralOptions
            // 
            this._tcGeneralOptions.Controls.Add(this._tpLicence);
            this._tcGeneralOptions.Controls.Add(this._tpServerControl);
            this._tcGeneralOptions.Controls.Add(this._tpRemoteServicesSettings);
            this._tcGeneralOptions.Controls.Add(this._tpDHCPServer);
            this._tcGeneralOptions.Controls.Add(this._tpSerialPort);
            this._tcGeneralOptions.Controls.Add(this._tpDatabaseOptions);
            this._tcGeneralOptions.Controls.Add(this._tpSecuritySettings);
            this._tcGeneralOptions.Controls.Add(this._tpAlarmSettings);
            this._tcGeneralOptions.Controls.Add(this._tpUiSettings);
            this._tcGeneralOptions.Controls.Add(this._tpEventlogs);
            this._tcGeneralOptions.Controls.Add(this._tpLanguage);
            this._tcGeneralOptions.Controls.Add(this._tpAdvancedAccessSettings);
            this._tcGeneralOptions.Controls.Add(this._tpAdvancedSettings);
            this._tcGeneralOptions.Controls.Add(this._tpCustomerSupplierInfo);
            this._tcGeneralOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tcGeneralOptions.Location = new System.Drawing.Point(0, 0);
            this._tcGeneralOptions.Name = "_tcGeneralOptions";
            this._tcGeneralOptions.SelectedIndex = 0;
            this._tcGeneralOptions.Size = new System.Drawing.Size(969, 633);
            this._tcGeneralOptions.TabIndex = 0;
            this._tcGeneralOptions.TabStop = false;
            // 
            // _tpLicence
            // 
            this._tpLicence.BackColor = System.Drawing.SystemColors.Control;
            this._tpLicence.Controls.Add(this._bInsertLicenceFile);
            this._tpLicence.Controls.Add(this._pBack);
            this._tpLicence.Controls.Add(this._buttonLicenceRequest);
            this._tpLicence.Controls.Add(this._lFunctionalPropertiesText);
            this._tpLicence.Controls.Add(this._lExpirationDateText);
            this._tpLicence.Controls.Add(this._lLicenceFileText);
            this._tpLicence.Location = new System.Drawing.Point(4, 22);
            this._tpLicence.Name = "_tpLicence";
            this._tpLicence.Size = new System.Drawing.Size(961, 607);
            this._tpLicence.TabIndex = 7;
            this._tpLicence.Text = "Licence info";
            this._tpLicence.Enter += new System.EventHandler(this._tpLicence_Enter);
            // 
            // _bInsertLicenceFile
            // 
            this._bInsertLicenceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bInsertLicenceFile.AutoSize = true;
            this._bInsertLicenceFile.Location = new System.Drawing.Point(11, 497);
            this._bInsertLicenceFile.Name = "_bInsertLicenceFile";
            this._bInsertLicenceFile.Size = new System.Drawing.Size(160, 27);
            this._bInsertLicenceFile.TabIndex = 6;
            this._bInsertLicenceFile.Text = "InsertLicenceFile";
            this._bInsertLicenceFile.UseVisualStyleBackColor = true;
            this._bInsertLicenceFile.Click += new System.EventHandler(this._bInsertLicenceFile_Click);
            // 
            // _pBack
            // 
            this._pBack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pBack.Controls.Add(this._epgLicenceInfo);
            this._pBack.Location = new System.Drawing.Point(11, 11);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(942, 447);
            this._pBack.TabIndex = 5;
            // 
            // _epgLicenceInfo
            // 
            this._epgLicenceInfo.BackColor = System.Drawing.SystemColors.Control;
            this._epgLicenceInfo.ButtonTextAddProperty = "Add property";
            this._epgLicenceInfo.ButtonTextRemoveProperty = "Remove property";
            // 
            // 
            // 
            this._epgLicenceInfo.DocCommentDescription.AutoEllipsis = true;
            this._epgLicenceInfo.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgLicenceInfo.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this._epgLicenceInfo.DocCommentDescription.Name = "";
            this._epgLicenceInfo.DocCommentDescription.Size = new System.Drawing.Size(936, 37);
            this._epgLicenceInfo.DocCommentDescription.TabIndex = 1;
            this._epgLicenceInfo.DocCommentImage = null;
            // 
            // 
            // 
            this._epgLicenceInfo.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgLicenceInfo.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this._epgLicenceInfo.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this._epgLicenceInfo.DocCommentTitle.Name = "";
            this._epgLicenceInfo.DocCommentTitle.Size = new System.Drawing.Size(936, 15);
            this._epgLicenceInfo.DocCommentTitle.TabIndex = 0;
            this._epgLicenceInfo.DocCommentTitle.UseMnemonic = false;
            this._epgLicenceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this._epgLicenceInfo.LineColor = System.Drawing.SystemColors.ControlDark;
            this._epgLicenceInfo.Location = new System.Drawing.Point(0, 0);
            this._epgLicenceInfo.Name = "_epgLicenceInfo";
            this._epgLicenceInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this._epgLicenceInfo.Size = new System.Drawing.Size(942, 447);
            this._epgLicenceInfo.TabIndex = 4;
            this._epgLicenceInfo.ToolbarVisible = false;
            // 
            // 
            // 
            this._epgLicenceInfo.ToolStrip.AccessibleName = "ToolBar";
            this._epgLicenceInfo.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this._epgLicenceInfo.ToolStrip.AllowMerge = false;
            this._epgLicenceInfo.ToolStrip.AutoSize = false;
            this._epgLicenceInfo.ToolStrip.BackColor = System.Drawing.SystemColors.Control;
            this._epgLicenceInfo.ToolStrip.CanOverflow = false;
            this._epgLicenceInfo.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._epgLicenceInfo.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._epgLicenceInfo.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this._epgLicenceInfo.ToolStrip.Name = "ExtendedPropertyToolStrip";
            this._epgLicenceInfo.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this._epgLicenceInfo.ToolStrip.Size = new System.Drawing.Size(942, 25);
            this._epgLicenceInfo.ToolStrip.TabIndex = 1;
            this._epgLicenceInfo.ToolStrip.TabStop = true;
            this._epgLicenceInfo.ToolStrip.Text = "PropertyGridToolBar";
            this._epgLicenceInfo.ToolStrip.Visible = false;
            // 
            // _buttonLicenceRequest
            // 
            this._buttonLicenceRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._buttonLicenceRequest.AutoSize = true;
            this._buttonLicenceRequest.Location = new System.Drawing.Point(11, 464);
            this._buttonLicenceRequest.Name = "_buttonLicenceRequest";
            this._buttonLicenceRequest.Size = new System.Drawing.Size(160, 27);
            this._buttonLicenceRequest.TabIndex = 0;
            this._buttonLicenceRequest.Text = "Create licence request";
            this._buttonLicenceRequest.UseVisualStyleBackColor = true;
            this._buttonLicenceRequest.Visible = false;
            this._buttonLicenceRequest.Click += new System.EventHandler(this._buttonLicenceRequest_Click);
            // 
            // _lFunctionalPropertiesText
            // 
            this._lFunctionalPropertiesText.AutoSize = true;
            this._lFunctionalPropertiesText.Location = new System.Drawing.Point(232, 56);
            this._lFunctionalPropertiesText.Name = "_lFunctionalPropertiesText";
            this._lFunctionalPropertiesText.Size = new System.Drawing.Size(0, 13);
            this._lFunctionalPropertiesText.TabIndex = 1;
            // 
            // _lExpirationDateText
            // 
            this._lExpirationDateText.AutoSize = true;
            this._lExpirationDateText.Location = new System.Drawing.Point(232, 33);
            this._lExpirationDateText.Name = "_lExpirationDateText";
            this._lExpirationDateText.Size = new System.Drawing.Size(0, 13);
            this._lExpirationDateText.TabIndex = 1;
            // 
            // _lLicenceFileText
            // 
            this._lLicenceFileText.AutoSize = true;
            this._lLicenceFileText.Location = new System.Drawing.Point(232, 11);
            this._lLicenceFileText.Name = "_lLicenceFileText";
            this._lLicenceFileText.Size = new System.Drawing.Size(0, 13);
            this._lLicenceFileText.TabIndex = 1;
            // 
            // _tpServerControl
            // 
            this._tpServerControl.BackColor = System.Drawing.Color.Transparent;
            this._tpServerControl.Controls.Add(this._gbServerInformations);
            this._tpServerControl.Controls.Add(this._gbResetUserSession);
            this._tpServerControl.Controls.Add(this._bDBTest);
            this._tpServerControl.Controls.Add(this._lStopServer);
            this._tpServerControl.Controls.Add(this._bStop);
            this._tpServerControl.Location = new System.Drawing.Point(4, 22);
            this._tpServerControl.Name = "_tpServerControl";
            this._tpServerControl.Padding = new System.Windows.Forms.Padding(3);
            this._tpServerControl.Size = new System.Drawing.Size(961, 607);
            this._tpServerControl.TabIndex = 4;
            this._tpServerControl.Text = "Server control";
            this._tpServerControl.UseVisualStyleBackColor = true;
            this._tpServerControl.Enter += new System.EventHandler(this._tpServerControl_Enter);
            // 
            // _gbServerInformations
            // 
            this._gbServerInformations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbServerInformations.Controls.Add(this._bRefresh);
            this._gbServerInformations.Controls.Add(this._dgServerInformations);
            this._gbServerInformations.Location = new System.Drawing.Point(8, 149);
            this._gbServerInformations.Name = "_gbServerInformations";
            this._gbServerInformations.Size = new System.Drawing.Size(945, 274);
            this._gbServerInformations.TabIndex = 6;
            this._gbServerInformations.TabStop = false;
            this._gbServerInformations.Text = "Server informations";
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(864, 245);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 6;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _dgServerInformations
            // 
            this._dgServerInformations.AllowUserToAddRows = false;
            this._dgServerInformations.AllowUserToDeleteRows = false;
            this._dgServerInformations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgServerInformations.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this._dgServerInformations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgServerInformations.ColumnHeadersVisible = false;
            this._dgServerInformations.Location = new System.Drawing.Point(6, 19);
            this._dgServerInformations.MultiSelect = false;
            this._dgServerInformations.Name = "_dgServerInformations";
            this._dgServerInformations.ReadOnly = true;
            this._dgServerInformations.RowHeadersVisible = false;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgServerInformations.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this._dgServerInformations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgServerInformations.Size = new System.Drawing.Size(933, 220);
            this._dgServerInformations.TabIndex = 5;
            // 
            // _gbResetUserSession
            // 
            this._gbResetUserSession.Controls.Add(this._bResetUserSession);
            this._gbResetUserSession.Controls.Add(this._lLogin);
            this._gbResetUserSession.Controls.Add(this._tbmLogin);
            this._gbResetUserSession.Location = new System.Drawing.Point(6, 64);
            this._gbResetUserSession.Name = "_gbResetUserSession";
            this._gbResetUserSession.Size = new System.Drawing.Size(311, 79);
            this._gbResetUserSession.TabIndex = 4;
            this._gbResetUserSession.TabStop = false;
            this._gbResetUserSession.Text = "Reset user session";
            // 
            // _bResetUserSession
            // 
            this._bResetUserSession.Location = new System.Drawing.Point(185, 50);
            this._bResetUserSession.Name = "_bResetUserSession";
            this._bResetUserSession.Size = new System.Drawing.Size(120, 23);
            this._bResetUserSession.TabIndex = 5;
            this._bResetUserSession.Text = "Reset session";
            this._bResetUserSession.UseVisualStyleBackColor = true;
            this._bResetUserSession.Click += new System.EventHandler(this._bResetUserSession_Click);
            // 
            // _lLogin
            // 
            this._lLogin.AutoSize = true;
            this._lLogin.Location = new System.Drawing.Point(6, 23);
            this._lLogin.Name = "_lLogin";
            this._lLogin.Size = new System.Drawing.Size(33, 13);
            this._lLogin.TabIndex = 4;
            this._lLogin.Text = "Login";
            // 
            // _tbmLogin
            // 
            this._tbmLogin.AllowDrop = true;
            this._tbmLogin.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmLogin.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmLogin.Button.BackColor = System.Drawing.Color.White;
            this._tbmLogin.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmLogin.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmLogin.Button.Image")));
            this._tbmLogin.Button.Location = new System.Drawing.Point(209, 0);
            this._tbmLogin.Button.Name = "_bMenu";
            this._tbmLogin.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmLogin.Button.TabIndex = 3;
            this._tbmLogin.Button.UseVisualStyleBackColor = false;
            this._tbmLogin.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmLogin.ButtonDefaultBehaviour = true;
            this._tbmLogin.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmLogin.ButtonImage = null;
            // 
            // 
            // 
            this._tbmLogin.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify3,
            this._tsiRemove3});
            this._tbmLogin.ButtonPopupMenu.Name = "";
            this._tbmLogin.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmLogin.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmLogin.ButtonShowImage = true;
            this._tbmLogin.ButtonSizeHeight = 20;
            this._tbmLogin.ButtonSizeWidth = 20;
            this._tbmLogin.ButtonText = "";
            this._tbmLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmLogin.HoverTime = 500;
            // 
            // 
            // 
            this._tbmLogin.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmLogin.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmLogin.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmLogin.ImageTextBox.ContextMenuStrip = this._tbmLogin.ButtonPopupMenu;
            this._tbmLogin.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmLogin.ImageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._tbmLogin.ImageTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this._tbmLogin.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmLogin.ImageTextBox.Image")));
            this._tbmLogin.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmLogin.ImageTextBox.Name = "_textBox";
            this._tbmLogin.ImageTextBox.NoTextNoImage = true;
            this._tbmLogin.ImageTextBox.ReadOnly = true;
            this._tbmLogin.ImageTextBox.Size = new System.Drawing.Size(209, 20);
            this._tbmLogin.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmLogin.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmLogin.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmLogin.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmLogin.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmLogin.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmLogin.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmLogin.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmLogin.ImageTextBox.TextBox.Size = new System.Drawing.Size(207, 13);
            this._tbmLogin.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmLogin.ImageTextBox.UseImage = true;
            this._tbmLogin.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmLogin_ImageTextBox_DoubleClick);
            this._tbmLogin.Location = new System.Drawing.Point(76, 19);
            this._tbmLogin.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmLogin.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmLogin.Name = "_tbmLogin";
            this._tbmLogin.Size = new System.Drawing.Size(229, 22);
            this._tbmLogin.TabIndex = 3;
            this._tbmLogin.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmLogin.TextImage")));
            this._tbmLogin.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmLogin_ButtonPopupMenuItemClick);
            this._tbmLogin.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmLogin_DragDrop);
            this._tbmLogin.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmLogin_DragOver);
            // 
            // _tsiModify3
            // 
            this._tsiModify3.Name = "_tsiModify3";
            this._tsiModify3.Size = new System.Drawing.Size(117, 22);
            this._tsiModify3.Text = "Modify";
            // 
            // _tsiRemove3
            // 
            this._tsiRemove3.Name = "_tsiRemove3";
            this._tsiRemove3.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove3.Text = "Remove";
            // 
            // _bDBTest
            // 
            this._bDBTest.Location = new System.Drawing.Point(116, 35);
            this._bDBTest.Name = "_bDBTest";
            this._bDBTest.Size = new System.Drawing.Size(75, 23);
            this._bDBTest.TabIndex = 2;
            this._bDBTest.Text = "Run Test";
            this._bDBTest.UseVisualStyleBackColor = true;
            this._bDBTest.Visible = false;
            this._bDBTest.Click += new System.EventHandler(this._bDBTest_Click);
            // 
            // _lStopServer
            // 
            this._lStopServer.AutoSize = true;
            this._lStopServer.Location = new System.Drawing.Point(8, 11);
            this._lStopServer.Name = "_lStopServer";
            this._lStopServer.Size = new System.Drawing.Size(61, 13);
            this._lStopServer.TabIndex = 1;
            this._lStopServer.Text = "Stop server";
            // 
            // _bStop
            // 
            this._bStop.Location = new System.Drawing.Point(116, 6);
            this._bStop.Name = "_bStop";
            this._bStop.Size = new System.Drawing.Size(75, 23);
            this._bStop.TabIndex = 0;
            this._bStop.Text = "Stop";
            this._bStop.UseVisualStyleBackColor = true;
            this._bStop.Click += new System.EventHandler(this._bStop_Click);
            // 
            // _tpRemoteServicesSettings
            // 
            this._tpRemoteServicesSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpRemoteServicesSettings.Controls.Add(this._gbNtpSettings);
            this._tpRemoteServicesSettings.Controls.Add(this._gbSmtpSettings);
            this._tpRemoteServicesSettings.Controls.Add(this._bSaveSmtp);
            this._tpRemoteServicesSettings.Location = new System.Drawing.Point(4, 22);
            this._tpRemoteServicesSettings.Name = "_tpRemoteServicesSettings";
            this._tpRemoteServicesSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpRemoteServicesSettings.Size = new System.Drawing.Size(961, 607);
            this._tpRemoteServicesSettings.TabIndex = 2;
            this._tpRemoteServicesSettings.Text = "Remote services settings";
            // 
            // _gbNtpSettings
            // 
            this._gbNtpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbNtpSettings.Controls.Add(this._nudTimeDiffTolerance);
            this._gbNtpSettings.Controls.Add(this._lTimeDiffTolerance);
            this._gbNtpSettings.Controls.Add(this._bResolve);
            this._gbNtpSettings.Controls.Add(this._lIpAddress);
            this._gbNtpSettings.Controls.Add(this._eDnsHostName);
            this._gbNtpSettings.Controls.Add(this._eNtpIpAddress);
            this._gbNtpSettings.Controls.Add(this._lDnsHostname1);
            this._gbNtpSettings.Controls.Add(this._bAdd);
            this._gbNtpSettings.Controls.Add(this._lNtpIpAddresses);
            this._gbNtpSettings.Controls.Add(this._bDelete2);
            this._gbNtpSettings.Location = new System.Drawing.Point(3, 199);
            this._gbNtpSettings.Name = "_gbNtpSettings";
            this._gbNtpSettings.Size = new System.Drawing.Size(955, 195);
            this._gbNtpSettings.TabIndex = 13;
            this._gbNtpSettings.TabStop = false;
            this._gbNtpSettings.Text = "NTP Settings";
            // 
            // _nudTimeDiffTolerance
            // 
            this._nudTimeDiffTolerance.Location = new System.Drawing.Point(6, 169);
            this._nudTimeDiffTolerance.Maximum = new decimal(new int[] {
            3600000,
            0,
            0,
            0});
            this._nudTimeDiffTolerance.Minimum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this._nudTimeDiffTolerance.Name = "_nudTimeDiffTolerance";
            this._nudTimeDiffTolerance.Size = new System.Drawing.Size(124, 20);
            this._nudTimeDiffTolerance.TabIndex = 19;
            this._nudTimeDiffTolerance.Value = new decimal(new int[] {
            120000,
            0,
            0,
            0});
            this._nudTimeDiffTolerance.ValueChanged += new System.EventHandler(this.EditTextChangerRemoteServicesSettings);
            // 
            // _lTimeDiffTolerance
            // 
            this._lTimeDiffTolerance.AutoSize = true;
            this._lTimeDiffTolerance.Location = new System.Drawing.Point(6, 153);
            this._lTimeDiffTolerance.Name = "_lTimeDiffTolerance";
            this._lTimeDiffTolerance.Size = new System.Drawing.Size(242, 13);
            this._lTimeDiffTolerance.TabIndex = 18;
            this._lTimeDiffTolerance.Text = "Allowed server - CCU time difference (miliseconds)";
            // 
            // _bResolve
            // 
            this._bResolve.Location = new System.Drawing.Point(402, 27);
            this._bResolve.Name = "_bResolve";
            this._bResolve.Size = new System.Drawing.Size(75, 23);
            this._bResolve.TabIndex = 16;
            this._bResolve.Text = "Resolve";
            this._bResolve.UseVisualStyleBackColor = true;
            this._bResolve.Click += new System.EventHandler(this.ResolveDnsClick);
            // 
            // _lIpAddress
            // 
            this._lIpAddress.AutoSize = true;
            this._lIpAddress.Location = new System.Drawing.Point(6, 13);
            this._lIpAddress.Name = "_lIpAddress";
            this._lIpAddress.Size = new System.Drawing.Size(57, 13);
            this._lIpAddress.TabIndex = 15;
            this._lIpAddress.Text = "IP address";
            // 
            // _eDnsHostName
            // 
            this._eDnsHostName.Location = new System.Drawing.Point(246, 29);
            this._eDnsHostName.Name = "_eDnsHostName";
            this._eDnsHostName.Size = new System.Drawing.Size(150, 20);
            this._eDnsHostName.TabIndex = 8;
            // 
            // _eNtpIpAddress
            // 
            this._eNtpIpAddress.Location = new System.Drawing.Point(6, 30);
            this._eNtpIpAddress.Name = "_eNtpIpAddress";
            this._eNtpIpAddress.Size = new System.Drawing.Size(150, 20);
            this._eNtpIpAddress.TabIndex = 5;
            this._eNtpIpAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._eNtpIpAddress_KeyPress);
            // 
            // _lDnsHostname1
            // 
            this._lDnsHostname1.AutoSize = true;
            this._lDnsHostname1.Location = new System.Drawing.Point(243, 13);
            this._lDnsHostname1.Name = "_lDnsHostname1";
            this._lDnsHostname1.Size = new System.Drawing.Size(79, 13);
            this._lDnsHostname1.TabIndex = 1;
            this._lDnsHostname1.Text = "DNS hostname";
            // 
            // _bAdd
            // 
            this._bAdd.Location = new System.Drawing.Point(162, 26);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(75, 23);
            this._bAdd.TabIndex = 6;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this.AddIpAddress);
            // 
            // _lNtpIpAddresses
            // 
            this._lNtpIpAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._lNtpIpAddresses.FormattingEnabled = true;
            this._lNtpIpAddresses.Location = new System.Drawing.Point(6, 55);
            this._lNtpIpAddresses.Name = "_lNtpIpAddresses";
            this._lNtpIpAddresses.Size = new System.Drawing.Size(471, 95);
            this._lNtpIpAddresses.TabIndex = 9;
            // 
            // _bDelete2
            // 
            this._bDelete2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDelete2.Location = new System.Drawing.Point(402, 156);
            this._bDelete2.Name = "_bDelete2";
            this._bDelete2.Size = new System.Drawing.Size(75, 23);
            this._bDelete2.TabIndex = 0;
            this._bDelete2.Text = "Delete";
            this._bDelete2.UseVisualStyleBackColor = true;
            this._bDelete2.Click += new System.EventHandler(this._bDelete2_Click);
            // 
            // _gbSmtpSettings
            // 
            this._gbSmtpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbSmtpSettings.Controls.Add(this._epgSMTPServerSettings);
            this._gbSmtpSettings.Location = new System.Drawing.Point(3, 6);
            this._gbSmtpSettings.Name = "_gbSmtpSettings";
            this._gbSmtpSettings.Size = new System.Drawing.Size(955, 187);
            this._gbSmtpSettings.TabIndex = 2;
            this._gbSmtpSettings.TabStop = false;
            this._gbSmtpSettings.Text = "SMTP settings";
            // 
            // _epgSMTPServerSettings
            // 
            this._epgSMTPServerSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._epgSMTPServerSettings.ButtonTextAddProperty = "Add property";
            this._epgSMTPServerSettings.ButtonTextRemoveProperty = "Remove property";
            // 
            // 
            // 
            this._epgSMTPServerSettings.DocCommentDescription.AutoEllipsis = true;
            this._epgSMTPServerSettings.DocCommentDescription.AutoSize = true;
            this._epgSMTPServerSettings.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgSMTPServerSettings.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this._epgSMTPServerSettings.DocCommentDescription.Name = "";
            this._epgSMTPServerSettings.DocCommentDescription.Size = new System.Drawing.Size(0, 15);
            this._epgSMTPServerSettings.DocCommentDescription.TabIndex = 1;
            this._epgSMTPServerSettings.DocCommentImage = null;
            // 
            // 
            // 
            this._epgSMTPServerSettings.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgSMTPServerSettings.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this._epgSMTPServerSettings.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this._epgSMTPServerSettings.DocCommentTitle.Name = "";
            this._epgSMTPServerSettings.DocCommentTitle.Size = new System.Drawing.Size(0, 0);
            this._epgSMTPServerSettings.DocCommentTitle.TabIndex = 0;
            this._epgSMTPServerSettings.DocCommentTitle.UseMnemonic = false;
            this._epgSMTPServerSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._epgSMTPServerSettings.HelpVisible = false;
            this._epgSMTPServerSettings.LineColor = System.Drawing.SystemColors.ControlDark;
            this._epgSMTPServerSettings.Location = new System.Drawing.Point(6, 19);
            this._epgSMTPServerSettings.Name = "_epgSMTPServerSettings";
            this._epgSMTPServerSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this._epgSMTPServerSettings.Size = new System.Drawing.Size(943, 162);
            this._epgSMTPServerSettings.TabIndex = 0;
            this._epgSMTPServerSettings.TabStop = false;
            this._epgSMTPServerSettings.ToolbarVisible = false;
            // 
            // 
            // 
            this._epgSMTPServerSettings.ToolStrip.AccessibleName = "ToolBar";
            this._epgSMTPServerSettings.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this._epgSMTPServerSettings.ToolStrip.AllowMerge = false;
            this._epgSMTPServerSettings.ToolStrip.AutoSize = false;
            this._epgSMTPServerSettings.ToolStrip.CanOverflow = false;
            this._epgSMTPServerSettings.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._epgSMTPServerSettings.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._epgSMTPServerSettings.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this._epgSMTPServerSettings.ToolStrip.Name = "ExtendedPropertyToolStrip";
            this._epgSMTPServerSettings.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this._epgSMTPServerSettings.ToolStrip.Size = new System.Drawing.Size(130, 25);
            this._epgSMTPServerSettings.ToolStrip.TabIndex = 1;
            this._epgSMTPServerSettings.ToolStrip.TabStop = true;
            this._epgSMTPServerSettings.ToolStrip.Text = "PropertyGridToolBar";
            this._epgSMTPServerSettings.ToolStrip.Visible = false;
            // 
            // _bSaveSmtp
            // 
            this._bSaveSmtp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSaveSmtp.Location = new System.Drawing.Point(6, 495);
            this._bSaveSmtp.Name = "_bSaveSmtp";
            this._bSaveSmtp.Size = new System.Drawing.Size(75, 23);
            this._bSaveSmtp.TabIndex = 1;
            this._bSaveSmtp.Text = "Save";
            this._bSaveSmtp.UseVisualStyleBackColor = true;
            this._bSaveSmtp.Click += new System.EventHandler(this.SaveSmtpClick);
            // 
            // _tpDHCPServer
            // 
            this._tpDHCPServer.BackColor = System.Drawing.SystemColors.Control;
            this._tpDHCPServer.Controls.Add(this._bStopDHCP);
            this._tpDHCPServer.Controls.Add(this._bStartDHCP);
            this._tpDHCPServer.Controls.Add(this._chbAutoStartDHCP);
            this._tpDHCPServer.Controls.Add(this._gbIPGroup);
            this._tpDHCPServer.Controls.Add(this._cbIPGroup);
            this._tpDHCPServer.Controls.Add(this._lIPGroup);
            this._tpDHCPServer.Location = new System.Drawing.Point(4, 22);
            this._tpDHCPServer.Name = "_tpDHCPServer";
            this._tpDHCPServer.Size = new System.Drawing.Size(961, 607);
            this._tpDHCPServer.TabIndex = 8;
            this._tpDHCPServer.Text = "DHCP server";
            this._tpDHCPServer.Enter += new System.EventHandler(this._tpDHCPServer_Enter);
            // 
            // _bStopDHCP
            // 
            this._bStopDHCP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this._bStopDHCP.Location = new System.Drawing.Point(6, 27);
            this._bStopDHCP.Name = "_bStopDHCP";
            this._bStopDHCP.Size = new System.Drawing.Size(75, 23);
            this._bStopDHCP.TabIndex = 1;
            this._bStopDHCP.Text = "Stop";
            this._bStopDHCP.UseVisualStyleBackColor = false;
            this._bStopDHCP.Visible = false;
            this._bStopDHCP.Click += new System.EventHandler(this._bStopDHCP_Click);
            // 
            // _bStartDHCP
            // 
            this._bStartDHCP.Location = new System.Drawing.Point(6, 27);
            this._bStartDHCP.Name = "_bStartDHCP";
            this._bStartDHCP.Size = new System.Drawing.Size(75, 23);
            this._bStartDHCP.TabIndex = 8;
            this._bStartDHCP.Text = "Start";
            this._bStartDHCP.UseVisualStyleBackColor = true;
            this._bStartDHCP.Click += new System.EventHandler(this._bStartDHCP_Click);
            // 
            // _chbAutoStartDHCP
            // 
            this._chbAutoStartDHCP.AutoSize = true;
            this._chbAutoStartDHCP.Location = new System.Drawing.Point(6, 4);
            this._chbAutoStartDHCP.Name = "_chbAutoStartDHCP";
            this._chbAutoStartDHCP.Size = new System.Drawing.Size(151, 17);
            this._chbAutoStartDHCP.TabIndex = 0;
            this._chbAutoStartDHCP.Text = "Auto start DHCP on server";
            this._chbAutoStartDHCP.UseVisualStyleBackColor = true;
            this._chbAutoStartDHCP.CheckedChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            this._chbAutoStartDHCP.Click += new System.EventHandler(this._chbAutoStartDHCP_Click);
            // 
            // _gbIPGroup
            // 
            this._gbIPGroup.Controls.Add(this._bDeleteIpGroup);
            this._gbIPGroup.Controls.Add(this._gbNetwork);
            this._gbIPGroup.Controls.Add(this._bDHCPSave);
            this._gbIPGroup.Controls.Add(this._tbMaxLeaseTime);
            this._gbIPGroup.Controls.Add(this._tbLeaseTime);
            this._gbIPGroup.Controls.Add(this._tbGateway);
            this._gbIPGroup.Controls.Add(this._lMaxLeaseTime);
            this._gbIPGroup.Controls.Add(this._gbRange);
            this._gbIPGroup.Controls.Add(this._lLeaseTime);
            this._gbIPGroup.Controls.Add(this._gbDNS);
            this._gbIPGroup.Controls.Add(this._lGateway);
            this._gbIPGroup.Enabled = false;
            this._gbIPGroup.Location = new System.Drawing.Point(6, 78);
            this._gbIPGroup.Name = "_gbIPGroup";
            this._gbIPGroup.Size = new System.Drawing.Size(501, 260);
            this._gbIPGroup.TabIndex = 6;
            this._gbIPGroup.TabStop = false;
            this._gbIPGroup.Text = "IP Group";
            // 
            // _bDeleteIpGroup
            // 
            this._bDeleteIpGroup.Location = new System.Drawing.Point(7, 231);
            this._bDeleteIpGroup.Name = "_bDeleteIpGroup";
            this._bDeleteIpGroup.Size = new System.Drawing.Size(75, 23);
            this._bDeleteIpGroup.TabIndex = 17;
            this._bDeleteIpGroup.Text = "Remove";
            this._bDeleteIpGroup.UseVisualStyleBackColor = true;
            this._bDeleteIpGroup.Click += new System.EventHandler(this._bDeleteIpGroup_Click);
            // 
            // _gbNetwork
            // 
            this._gbNetwork.Controls.Add(this._tbSubnet);
            this._gbNetwork.Controls.Add(this._lSubnet);
            this._gbNetwork.Controls.Add(this._tbNetworkMask);
            this._gbNetwork.Controls.Add(this._lSubnetMask);
            this._gbNetwork.Location = new System.Drawing.Point(6, 19);
            this._gbNetwork.Name = "_gbNetwork";
            this._gbNetwork.Size = new System.Drawing.Size(239, 71);
            this._gbNetwork.TabIndex = 2;
            this._gbNetwork.TabStop = false;
            this._gbNetwork.Text = "Network";
            // 
            // _tbSubnet
            // 
            this._tbSubnet.Location = new System.Drawing.Point(81, 13);
            this._tbSubnet.Name = "_tbSubnet";
            this._tbSubnet.Size = new System.Drawing.Size(152, 20);
            this._tbSubnet.TabIndex = 3;
            this._tbSubnet.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            // 
            // _lSubnet
            // 
            this._lSubnet.AutoSize = true;
            this._lSubnet.Location = new System.Drawing.Point(6, 16);
            this._lSubnet.Name = "_lSubnet";
            this._lSubnet.Size = new System.Drawing.Size(45, 13);
            this._lSubnet.TabIndex = 0;
            this._lSubnet.Text = "Address";
            // 
            // _tbNetworkMask
            // 
            this._tbNetworkMask.Location = new System.Drawing.Point(81, 39);
            this._tbNetworkMask.Name = "_tbNetworkMask";
            this._tbNetworkMask.Size = new System.Drawing.Size(152, 20);
            this._tbNetworkMask.TabIndex = 4;
            this._tbNetworkMask.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            // 
            // _lSubnetMask
            // 
            this._lSubnetMask.AutoSize = true;
            this._lSubnetMask.Location = new System.Drawing.Point(6, 42);
            this._lSubnetMask.Name = "_lSubnetMask";
            this._lSubnetMask.Size = new System.Drawing.Size(33, 13);
            this._lSubnetMask.TabIndex = 0;
            this._lSubnetMask.Text = "Mask";
            // 
            // _bDHCPSave
            // 
            this._bDHCPSave.Location = new System.Drawing.Point(87, 231);
            this._bDHCPSave.Name = "_bDHCPSave";
            this._bDHCPSave.Size = new System.Drawing.Size(75, 23);
            this._bDHCPSave.TabIndex = 18;
            this._bDHCPSave.Text = "Save";
            this._bDHCPSave.UseVisualStyleBackColor = true;
            this._bDHCPSave.Click += new System.EventHandler(this._bDHCPSave_Click);
            // 
            // _tbMaxLeaseTime
            // 
            this._tbMaxLeaseTime.Location = new System.Drawing.Point(425, 188);
            this._tbMaxLeaseTime.Name = "_tbMaxLeaseTime";
            this._tbMaxLeaseTime.Size = new System.Drawing.Size(63, 20);
            this._tbMaxLeaseTime.TabIndex = 16;
            this._tbMaxLeaseTime.Text = "7200";
            this._tbMaxLeaseTime.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            // 
            // _tbLeaseTime
            // 
            this._tbLeaseTime.Location = new System.Drawing.Point(425, 162);
            this._tbLeaseTime.Name = "_tbLeaseTime";
            this._tbLeaseTime.Size = new System.Drawing.Size(63, 20);
            this._tbLeaseTime.TabIndex = 15;
            this._tbLeaseTime.Text = "3600";
            this._tbLeaseTime.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            // 
            // _tbGateway
            // 
            this._tbGateway.Location = new System.Drawing.Point(337, 123);
            this._tbGateway.Name = "_tbGateway";
            this._tbGateway.Size = new System.Drawing.Size(152, 20);
            this._tbGateway.TabIndex = 14;
            this._tbGateway.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            // 
            // _lMaxLeaseTime
            // 
            this._lMaxLeaseTime.AutoSize = true;
            this._lMaxLeaseTime.Location = new System.Drawing.Point(257, 191);
            this._lMaxLeaseTime.Name = "_lMaxLeaseTime";
            this._lMaxLeaseTime.Size = new System.Drawing.Size(77, 13);
            this._lMaxLeaseTime.TabIndex = 2;
            this._lMaxLeaseTime.Text = "Max lease time";
            // 
            // _gbRange
            // 
            this._gbRange.Controls.Add(this._buttonSetMacGroup);
            this._gbRange.Controls.Add(this._bMaxRange);
            this._gbRange.Controls.Add(this._chbEnable);
            this._gbRange.Controls.Add(this._lIPRangeFrom);
            this._gbRange.Controls.Add(this._lIPRangeTo);
            this._gbRange.Controls.Add(this._cbMACMask);
            this._gbRange.Controls.Add(this._tbIPRangeFrom);
            this._gbRange.Controls.Add(this._lMACMask);
            this._gbRange.Controls.Add(this._tbIPRangeTo);
            this._gbRange.Location = new System.Drawing.Point(6, 96);
            this._gbRange.Name = "_gbRange";
            this._gbRange.Size = new System.Drawing.Size(239, 120);
            this._gbRange.TabIndex = 3;
            this._gbRange.TabStop = false;
            this._gbRange.Text = "Range for specific MAC mask";
            // 
            // _buttonSetMacGroup
            // 
            this._buttonSetMacGroup.AutoSize = true;
            this._buttonSetMacGroup.Location = new System.Drawing.Point(89, 92);
            this._buttonSetMacGroup.Name = "_buttonSetMacGroup";
            this._buttonSetMacGroup.Size = new System.Drawing.Size(67, 27);
            this._buttonSetMacGroup.TabIndex = 9;
            this._buttonSetMacGroup.Text = "Set";
            this._buttonSetMacGroup.UseVisualStyleBackColor = true;
            this._buttonSetMacGroup.Click += new System.EventHandler(this._buttonSetMacGroup_Click);
            // 
            // _bMaxRange
            // 
            this._bMaxRange.Location = new System.Drawing.Point(162, 92);
            this._bMaxRange.Name = "_bMaxRange";
            this._bMaxRange.Size = new System.Drawing.Size(70, 23);
            this._bMaxRange.TabIndex = 10;
            this._bMaxRange.Text = "<min,max>";
            this._bMaxRange.UseVisualStyleBackColor = true;
            this._bMaxRange.Click += new System.EventHandler(this._bMaxRAnge_Click);
            // 
            // _chbEnable
            // 
            this._chbEnable.AutoSize = true;
            this._chbEnable.Location = new System.Drawing.Point(11, 96);
            this._chbEnable.Name = "_chbEnable";
            this._chbEnable.Size = new System.Drawing.Size(65, 17);
            this._chbEnable.TabIndex = 8;
            this._chbEnable.Text = "Enabled";
            this._chbEnable.UseVisualStyleBackColor = true;
            this._chbEnable.CheckedChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            this._chbEnable.Click += new System.EventHandler(this._chbEnable_Click);
            // 
            // _lIPRangeFrom
            // 
            this._lIPRangeFrom.AutoSize = true;
            this._lIPRangeFrom.Location = new System.Drawing.Point(8, 43);
            this._lIPRangeFrom.Name = "_lIPRangeFrom";
            this._lIPRangeFrom.Size = new System.Drawing.Size(30, 13);
            this._lIPRangeFrom.TabIndex = 0;
            this._lIPRangeFrom.Text = "From";
            // 
            // _lIPRangeTo
            // 
            this._lIPRangeTo.AutoSize = true;
            this._lIPRangeTo.Location = new System.Drawing.Point(8, 69);
            this._lIPRangeTo.Name = "_lIPRangeTo";
            this._lIPRangeTo.Size = new System.Drawing.Size(16, 13);
            this._lIPRangeTo.TabIndex = 0;
            this._lIPRangeTo.Text = "to";
            // 
            // _cbMACMask
            // 
            this._cbMACMask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbMACMask.FormattingEnabled = true;
            this._cbMACMask.Location = new System.Drawing.Point(89, 13);
            this._cbMACMask.Name = "_cbMACMask";
            this._cbMACMask.Size = new System.Drawing.Size(144, 21);
            this._cbMACMask.TabIndex = 5;
            this._cbMACMask.SelectedIndexChanged += new System.EventHandler(this._cbMACMask_SelectedIndexChanged);
            // 
            // _tbIPRangeFrom
            // 
            this._tbIPRangeFrom.Location = new System.Drawing.Point(89, 40);
            this._tbIPRangeFrom.Name = "_tbIPRangeFrom";
            this._tbIPRangeFrom.Size = new System.Drawing.Size(144, 20);
            this._tbIPRangeFrom.TabIndex = 6;
            this._tbIPRangeFrom.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            this._tbIPRangeFrom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._tbIPRangeFrom_KeyPress);
            this._tbIPRangeFrom.Leave += new System.EventHandler(this._tbIPRangeFrom_Leave);
            // 
            // _lMACMask
            // 
            this._lMACMask.AutoSize = true;
            this._lMACMask.Location = new System.Drawing.Point(6, 16);
            this._lMACMask.Name = "_lMACMask";
            this._lMACMask.Size = new System.Drawing.Size(59, 13);
            this._lMACMask.TabIndex = 2;
            this._lMACMask.Text = "MAC Mask";
            // 
            // _tbIPRangeTo
            // 
            this._tbIPRangeTo.Location = new System.Drawing.Point(89, 66);
            this._tbIPRangeTo.Name = "_tbIPRangeTo";
            this._tbIPRangeTo.Size = new System.Drawing.Size(144, 20);
            this._tbIPRangeTo.TabIndex = 7;
            this._tbIPRangeTo.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            this._tbIPRangeTo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._tbIPRangeFrom_KeyPress);
            this._tbIPRangeTo.Leave += new System.EventHandler(this._tbIPRangeTo_Leave);
            // 
            // _lLeaseTime
            // 
            this._lLeaseTime.AutoSize = true;
            this._lLeaseTime.Location = new System.Drawing.Point(257, 165);
            this._lLeaseTime.Name = "_lLeaseTime";
            this._lLeaseTime.Size = new System.Drawing.Size(58, 13);
            this._lLeaseTime.TabIndex = 2;
            this._lLeaseTime.Text = "Lease time";
            // 
            // _gbDNS
            // 
            this._gbDNS.Controls.Add(this._lDNSSuffix);
            this._gbDNS.Controls.Add(this._lAlternateDNSServer);
            this._gbDNS.Controls.Add(this._tbDnsSuffix);
            this._gbDNS.Controls.Add(this._lPrefferedDNSServer);
            this._gbDNS.Controls.Add(this._tbAlternateDNS);
            this._gbDNS.Controls.Add(this._tbDNS);
            this._gbDNS.Location = new System.Drawing.Point(251, 19);
            this._gbDNS.Name = "_gbDNS";
            this._gbDNS.Size = new System.Drawing.Size(240, 93);
            this._gbDNS.TabIndex = 4;
            this._gbDNS.TabStop = false;
            this._gbDNS.Text = "DNS server";
            // 
            // _lDNSSuffix
            // 
            this._lDNSSuffix.AutoSize = true;
            this._lDNSSuffix.Location = new System.Drawing.Point(7, 69);
            this._lDNSSuffix.Name = "_lDNSSuffix";
            this._lDNSSuffix.Size = new System.Drawing.Size(33, 13);
            this._lDNSSuffix.TabIndex = 2;
            this._lDNSSuffix.Text = "Suffix";
            // 
            // _lAlternateDNSServer
            // 
            this._lAlternateDNSServer.AutoSize = true;
            this._lAlternateDNSServer.Location = new System.Drawing.Point(6, 42);
            this._lAlternateDNSServer.Name = "_lAlternateDNSServer";
            this._lAlternateDNSServer.Size = new System.Drawing.Size(49, 13);
            this._lAlternateDNSServer.TabIndex = 2;
            this._lAlternateDNSServer.Text = "Alternate";
            // 
            // _tbDnsSuffix
            // 
            this._tbDnsSuffix.Location = new System.Drawing.Point(85, 66);
            this._tbDnsSuffix.Name = "_tbDnsSuffix";
            this._tbDnsSuffix.Size = new System.Drawing.Size(152, 20);
            this._tbDnsSuffix.TabIndex = 13;
            this._tbDnsSuffix.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            // 
            // _lPrefferedDNSServer
            // 
            this._lPrefferedDNSServer.AutoSize = true;
            this._lPrefferedDNSServer.Location = new System.Drawing.Point(6, 16);
            this._lPrefferedDNSServer.Name = "_lPrefferedDNSServer";
            this._lPrefferedDNSServer.Size = new System.Drawing.Size(50, 13);
            this._lPrefferedDNSServer.TabIndex = 2;
            this._lPrefferedDNSServer.Text = "Preffered";
            // 
            // _tbAlternateDNS
            // 
            this._tbAlternateDNS.Location = new System.Drawing.Point(84, 39);
            this._tbAlternateDNS.Name = "_tbAlternateDNS";
            this._tbAlternateDNS.Size = new System.Drawing.Size(152, 20);
            this._tbAlternateDNS.TabIndex = 12;
            this._tbAlternateDNS.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            // 
            // _tbDNS
            // 
            this._tbDNS.Location = new System.Drawing.Point(84, 13);
            this._tbDNS.Name = "_tbDNS";
            this._tbDNS.Size = new System.Drawing.Size(152, 20);
            this._tbDNS.TabIndex = 11;
            this._tbDNS.TextChanged += new System.EventHandler(this.EditTextChangerDHCPServerSettings);
            // 
            // _lGateway
            // 
            this._lGateway.AutoSize = true;
            this._lGateway.Location = new System.Drawing.Point(258, 126);
            this._lGateway.Name = "_lGateway";
            this._lGateway.Size = new System.Drawing.Size(49, 13);
            this._lGateway.TabIndex = 2;
            this._lGateway.Text = "Gateway";
            // 
            // _cbIPGroup
            // 
            this._cbIPGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbIPGroup.FormattingEnabled = true;
            this._cbIPGroup.Location = new System.Drawing.Point(88, 54);
            this._cbIPGroup.Name = "_cbIPGroup";
            this._cbIPGroup.Size = new System.Drawing.Size(159, 21);
            this._cbIPGroup.TabIndex = 2;
            this._cbIPGroup.SelectedValueChanged += new System.EventHandler(this._cbIPGroup_SelectedValueChanged);
            this._cbIPGroup.Click += new System.EventHandler(this._cbIPGroup_Click);
            this._cbIPGroup.MouseDown += new System.Windows.Forms.MouseEventHandler(this._cbIPGroup_MouseDown);
            // 
            // _lIPGroup
            // 
            this._lIPGroup.AutoSize = true;
            this._lIPGroup.Location = new System.Drawing.Point(5, 57);
            this._lIPGroup.Name = "_lIPGroup";
            this._lIPGroup.Size = new System.Drawing.Size(52, 13);
            this._lIPGroup.TabIndex = 2;
            this._lIPGroup.Text = "IP groups";
            // 
            // _tpSerialPort
            // 
            this._tpSerialPort.BackColor = System.Drawing.SystemColors.Control;
            this._tpSerialPort.Controls.Add(this._bSaveSerialPort);
            this._tpSerialPort.Controls.Add(this._ePortPin);
            this._tpSerialPort.Controls.Add(this._lPin);
            this._tpSerialPort.Controls.Add(this._chbPortCarrierDetect);
            this._tpSerialPort.Controls.Add(this._chbPortParityCheck);
            this._tpSerialPort.Controls.Add(this._PortNumber);
            this._tpSerialPort.Controls.Add(this._cbPortFlowControl);
            this._tpSerialPort.Controls.Add(this._lFlowControl);
            this._tpSerialPort.Controls.Add(this._cbPort);
            this._tpSerialPort.Controls.Add(this._lBaud);
            this._tpSerialPort.Controls.Add(this._cbPortStopBits);
            this._tpSerialPort.Controls.Add(this._cbPortBaudRate);
            this._tpSerialPort.Controls.Add(this._cbPortDataBits);
            this._tpSerialPort.Controls.Add(this._lStopBits);
            this._tpSerialPort.Controls.Add(this._lBits);
            this._tpSerialPort.Controls.Add(this._lParity);
            this._tpSerialPort.Controls.Add(this._cbPortParity);
            this._tpSerialPort.Location = new System.Drawing.Point(4, 22);
            this._tpSerialPort.Name = "_tpSerialPort";
            this._tpSerialPort.Padding = new System.Windows.Forms.Padding(3);
            this._tpSerialPort.Size = new System.Drawing.Size(961, 607);
            this._tpSerialPort.TabIndex = 3;
            this._tpSerialPort.Text = "Serail Port settings";
            // 
            // _bSaveSerialPort
            // 
            this._bSaveSerialPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSaveSerialPort.Location = new System.Drawing.Point(8, 409);
            this._bSaveSerialPort.Name = "_bSaveSerialPort";
            this._bSaveSerialPort.Size = new System.Drawing.Size(75, 23);
            this._bSaveSerialPort.TabIndex = 9;
            this._bSaveSerialPort.Text = "Save";
            this._bSaveSerialPort.UseVisualStyleBackColor = true;
            this._bSaveSerialPort.Click += new System.EventHandler(this.SaveSerialPortClick);
            // 
            // _ePortPin
            // 
            this._ePortPin.Location = new System.Drawing.Point(3, 305);
            this._ePortPin.Name = "_ePortPin";
            this._ePortPin.Size = new System.Drawing.Size(190, 20);
            this._ePortPin.TabIndex = 8;
            this._ePortPin.TextChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _lPin
            // 
            this._lPin.AutoSize = true;
            this._lPin.Location = new System.Drawing.Point(3, 289);
            this._lPin.Name = "_lPin";
            this._lPin.Size = new System.Drawing.Size(25, 13);
            this._lPin.TabIndex = 13;
            this._lPin.Text = "PIN";
            // 
            // _chbPortCarrierDetect
            // 
            this._chbPortCarrierDetect.AutoSize = true;
            this._chbPortCarrierDetect.Location = new System.Drawing.Point(6, 269);
            this._chbPortCarrierDetect.Name = "_chbPortCarrierDetect";
            this._chbPortCarrierDetect.Size = new System.Drawing.Size(91, 17);
            this._chbPortCarrierDetect.TabIndex = 7;
            this._chbPortCarrierDetect.Text = "Carrier Detect";
            this._chbPortCarrierDetect.UseVisualStyleBackColor = true;
            this._chbPortCarrierDetect.CheckedChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _chbPortParityCheck
            // 
            this._chbPortParityCheck.AutoSize = true;
            this._chbPortParityCheck.Location = new System.Drawing.Point(6, 246);
            this._chbPortParityCheck.Name = "_chbPortParityCheck";
            this._chbPortParityCheck.Size = new System.Drawing.Size(86, 17);
            this._chbPortParityCheck.TabIndex = 6;
            this._chbPortParityCheck.Text = "Parity Check";
            this._chbPortParityCheck.UseVisualStyleBackColor = true;
            this._chbPortParityCheck.CheckedChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _PortNumber
            // 
            this._PortNumber.AutoSize = true;
            this._PortNumber.Location = new System.Drawing.Point(3, 3);
            this._PortNumber.Name = "_PortNumber";
            this._PortNumber.Size = new System.Drawing.Size(26, 13);
            this._PortNumber.TabIndex = 0;
            this._PortNumber.Text = "Port";
            // 
            // _cbPortFlowControl
            // 
            this._cbPortFlowControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPortFlowControl.FormattingEnabled = true;
            this._cbPortFlowControl.Items.AddRange(new object[] {
            "Xon/Xoff",
            "Hadware",
            "None"});
            this._cbPortFlowControl.Location = new System.Drawing.Point(6, 219);
            this._cbPortFlowControl.Name = "_cbPortFlowControl";
            this._cbPortFlowControl.Size = new System.Drawing.Size(187, 21);
            this._cbPortFlowControl.TabIndex = 5;
            this._cbPortFlowControl.SelectedValueChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _lFlowControl
            // 
            this._lFlowControl.AutoSize = true;
            this._lFlowControl.Location = new System.Drawing.Point(3, 203);
            this._lFlowControl.Name = "_lFlowControl";
            this._lFlowControl.Size = new System.Drawing.Size(65, 13);
            this._lFlowControl.TabIndex = 18;
            this._lFlowControl.Text = "Flow Control";
            // 
            // _cbPort
            // 
            this._cbPort.FormattingEnabled = true;
            this._cbPort.Items.AddRange(new object[] {
            "COM1",
            "COM2"});
            this._cbPort.Location = new System.Drawing.Point(6, 19);
            this._cbPort.Name = "_cbPort";
            this._cbPort.Size = new System.Drawing.Size(187, 21);
            this._cbPort.TabIndex = 0;
            this._cbPort.SelectedValueChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _lBaud
            // 
            this._lBaud.AutoSize = true;
            this._lBaud.Location = new System.Drawing.Point(3, 43);
            this._lBaud.Name = "_lBaud";
            this._lBaud.Size = new System.Drawing.Size(58, 13);
            this._lBaud.TabIndex = 2;
            this._lBaud.Text = "Baud Rate";
            // 
            // _cbPortStopBits
            // 
            this._cbPortStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPortStopBits.FormattingEnabled = true;
            this._cbPortStopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this._cbPortStopBits.Location = new System.Drawing.Point(6, 179);
            this._cbPortStopBits.Name = "_cbPortStopBits";
            this._cbPortStopBits.Size = new System.Drawing.Size(187, 21);
            this._cbPortStopBits.TabIndex = 4;
            this._cbPortStopBits.SelectedValueChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _cbPortBaudRate
            // 
            this._cbPortBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPortBaudRate.FormattingEnabled = true;
            this._cbPortBaudRate.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200"});
            this._cbPortBaudRate.Location = new System.Drawing.Point(6, 59);
            this._cbPortBaudRate.Name = "_cbPortBaudRate";
            this._cbPortBaudRate.Size = new System.Drawing.Size(187, 21);
            this._cbPortBaudRate.TabIndex = 1;
            this._cbPortBaudRate.SelectedValueChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _cbPortDataBits
            // 
            this._cbPortDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPortDataBits.FormattingEnabled = true;
            this._cbPortDataBits.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8"});
            this._cbPortDataBits.Location = new System.Drawing.Point(6, 99);
            this._cbPortDataBits.Name = "_cbPortDataBits";
            this._cbPortDataBits.Size = new System.Drawing.Size(187, 21);
            this._cbPortDataBits.TabIndex = 2;
            this._cbPortDataBits.SelectedValueChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _lStopBits
            // 
            this._lStopBits.AutoSize = true;
            this._lStopBits.Location = new System.Drawing.Point(3, 163);
            this._lStopBits.Name = "_lStopBits";
            this._lStopBits.Size = new System.Drawing.Size(49, 13);
            this._lStopBits.TabIndex = 8;
            this._lStopBits.Text = "Stop Bits";
            // 
            // _lBits
            // 
            this._lBits.AutoSize = true;
            this._lBits.Location = new System.Drawing.Point(3, 83);
            this._lBits.Name = "_lBits";
            this._lBits.Size = new System.Drawing.Size(50, 13);
            this._lBits.TabIndex = 4;
            this._lBits.Text = "Data Bits";
            // 
            // _lParity
            // 
            this._lParity.AutoSize = true;
            this._lParity.Location = new System.Drawing.Point(3, 123);
            this._lParity.Name = "_lParity";
            this._lParity.Size = new System.Drawing.Size(33, 13);
            this._lParity.TabIndex = 6;
            this._lParity.Text = "Parity";
            // 
            // _cbPortParity
            // 
            this._cbPortParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPortParity.FormattingEnabled = true;
            this._cbPortParity.Items.AddRange(new object[] {
            "N",
            "O",
            "E",
            "M",
            "S"});
            this._cbPortParity.Location = new System.Drawing.Point(6, 139);
            this._cbPortParity.Name = "_cbPortParity";
            this._cbPortParity.Size = new System.Drawing.Size(187, 21);
            this._cbPortParity.TabIndex = 3;
            this._cbPortParity.SelectedValueChanged += new System.EventHandler(this.EditTextChangerSerialPortSettings);
            // 
            // _tpDatabaseOptions
            // 
            this._tpDatabaseOptions.BackColor = System.Drawing.SystemColors.Control;
            this._tpDatabaseOptions.Controls.Add(this._gbEventLogExpiration);
            this._tpDatabaseOptions.Controls.Add(this._gbDatabaseBackup);
            this._tpDatabaseOptions.Location = new System.Drawing.Point(4, 22);
            this._tpDatabaseOptions.Name = "_tpDatabaseOptions";
            this._tpDatabaseOptions.Padding = new System.Windows.Forms.Padding(3);
            this._tpDatabaseOptions.Size = new System.Drawing.Size(961, 607);
            this._tpDatabaseOptions.TabIndex = 9;
            this._tpDatabaseOptions.Text = "Database options";
            this._tpDatabaseOptions.Enter += new System.EventHandler(this._tpDatabaseOptions_Enter);
            // 
            // _gbEventLogExpiration
            // 
            this._gbEventLogExpiration.Controls.Add(this._bSaveEventlogExpiration);
            this._gbEventLogExpiration.Controls.Add(this._lCalculatedValue);
            this._gbEventLogExpiration.Controls.Add(this._lExponent);
            this._gbEventLogExpiration.Controls.Add(this._gbTimeZone1);
            this._gbEventLogExpiration.Controls.Add(this._bForceEvenlogExpirationClean);
            this._gbEventLogExpiration.Controls.Add(this._eResultMaxEventlogRecords);
            this._gbEventLogExpiration.Controls.Add(this._lMaxEventlogsRecords);
            this._gbEventLogExpiration.Controls.Add(this._tbMaxEventlogSlider);
            this._gbEventLogExpiration.Controls.Add(this._eMaxEventlogRecords);
            this._gbEventLogExpiration.Controls.Add(this._lDaysExpiration);
            this._gbEventLogExpiration.Controls.Add(this._eCountDaysExpiration);
            this._gbEventLogExpiration.Location = new System.Drawing.Point(6, 242);
            this._gbEventLogExpiration.Name = "_gbEventLogExpiration";
            this._gbEventLogExpiration.Size = new System.Drawing.Size(772, 255);
            this._gbEventLogExpiration.TabIndex = 6;
            this._gbEventLogExpiration.TabStop = false;
            this._gbEventLogExpiration.Text = "Eventlog expiration";
            // 
            // _bSaveEventlogExpiration
            // 
            this._bSaveEventlogExpiration.Location = new System.Drawing.Point(6, 224);
            this._bSaveEventlogExpiration.Name = "_bSaveEventlogExpiration";
            this._bSaveEventlogExpiration.Size = new System.Drawing.Size(75, 23);
            this._bSaveEventlogExpiration.TabIndex = 10;
            this._bSaveEventlogExpiration.Text = "Save";
            this._bSaveEventlogExpiration.UseVisualStyleBackColor = true;
            this._bSaveEventlogExpiration.Click += new System.EventHandler(this._bSaveEventlogExpiration_Click);
            // 
            // _lCalculatedValue
            // 
            this._lCalculatedValue.AutoSize = true;
            this._lCalculatedValue.Location = new System.Drawing.Point(268, 55);
            this._lCalculatedValue.Name = "_lCalculatedValue";
            this._lCalculatedValue.Size = new System.Drawing.Size(154, 13);
            this._lCalculatedValue.TabIndex = 9;
            this._lCalculatedValue.Text = "Actual limit for eventlog records";
            // 
            // _lExponent
            // 
            this._lExponent.AutoSize = true;
            this._lExponent.Location = new System.Drawing.Point(144, 55);
            this._lExponent.Name = "_lExponent";
            this._lExponent.Size = new System.Drawing.Size(52, 13);
            this._lExponent.TabIndex = 8;
            this._lExponent.Text = "Exponent";
            // 
            // _gbTimeZone1
            // 
            this._gbTimeZone1.Controls.Add(this._tbmEventlogTimeZone);
            this._gbTimeZone1.Controls.Add(this._lInfoEventlogExpirationTimeZone);
            this._gbTimeZone1.Location = new System.Drawing.Point(6, 125);
            this._gbTimeZone1.Name = "_gbTimeZone1";
            this._gbTimeZone1.Size = new System.Drawing.Size(463, 64);
            this._gbTimeZone1.TabIndex = 7;
            this._gbTimeZone1.TabStop = false;
            this._gbTimeZone1.Text = "Time zone";
            // 
            // _tbmEventlogTimeZone
            // 
            this._tbmEventlogTimeZone.AllowDrop = true;
            this._tbmEventlogTimeZone.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmEventlogTimeZone.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEventlogTimeZone.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmEventlogTimeZone.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmEventlogTimeZone.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEventlogTimeZone.Button.Image")));
            this._tbmEventlogTimeZone.Button.Location = new System.Drawing.Point(423, 0);
            this._tbmEventlogTimeZone.Button.Name = "_bMenu";
            this._tbmEventlogTimeZone.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmEventlogTimeZone.Button.TabIndex = 3;
            this._tbmEventlogTimeZone.Button.UseVisualStyleBackColor = false;
            this._tbmEventlogTimeZone.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmEventlogTimeZone.ButtonDefaultBehaviour = true;
            this._tbmEventlogTimeZone.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmEventlogTimeZone.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmEventlogTimeZone.ButtonImage")));
            // 
            // 
            // 
            this._tbmEventlogTimeZone.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify1,
            this._tsiRemove1,
            this._tsiCreate1});
            this._tbmEventlogTimeZone.ButtonPopupMenu.Name = "";
            this._tbmEventlogTimeZone.ButtonPopupMenu.Size = new System.Drawing.Size(118, 70);
            this._tbmEventlogTimeZone.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmEventlogTimeZone.ButtonShowImage = true;
            this._tbmEventlogTimeZone.ButtonSizeHeight = 20;
            this._tbmEventlogTimeZone.ButtonSizeWidth = 20;
            this._tbmEventlogTimeZone.ButtonText = "";
            this._tbmEventlogTimeZone.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEventlogTimeZone.HoverTime = 500;
            // 
            // 
            // 
            this._tbmEventlogTimeZone.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEventlogTimeZone.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEventlogTimeZone.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmEventlogTimeZone.ImageTextBox.ContextMenuStrip = this._tbmEventlogTimeZone.ButtonPopupMenu;
            this._tbmEventlogTimeZone.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEventlogTimeZone.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEventlogTimeZone.ImageTextBox.Image")));
            this._tbmEventlogTimeZone.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmEventlogTimeZone.ImageTextBox.Name = "_textBox";
            this._tbmEventlogTimeZone.ImageTextBox.NoTextNoImage = true;
            this._tbmEventlogTimeZone.ImageTextBox.ReadOnly = true;
            this._tbmEventlogTimeZone.ImageTextBox.Size = new System.Drawing.Size(423, 20);
            this._tbmEventlogTimeZone.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.Size = new System.Drawing.Size(421, 13);
            this._tbmEventlogTimeZone.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmEventlogTimeZone.ImageTextBox.UseImage = true;
            this._tbmEventlogTimeZone.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmEventlogTimeZone_TextBox_DoubleClick);
            this._tbmEventlogTimeZone.Location = new System.Drawing.Point(6, 32);
            this._tbmEventlogTimeZone.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmEventlogTimeZone.MinimumSize = new System.Drawing.Size(30, 20);
            this._tbmEventlogTimeZone.Name = "_tbmEventlogTimeZone";
            this._tbmEventlogTimeZone.Size = new System.Drawing.Size(443, 20);
            this._tbmEventlogTimeZone.TabIndex = 1;
            this._tbmEventlogTimeZone.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmEventlogTimeZone.TextImage")));
            this._tbmEventlogTimeZone.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmEventlogTimeZone_ButtonPopupMenuItemClick);
            this._tbmEventlogTimeZone.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmEventlogTimeZone_DragDrop);
            this._tbmEventlogTimeZone.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmEventlogTimeZone_DragOver);
            // 
            // _tsiModify1
            // 
            this._tsiModify1.Name = "_tsiModify1";
            this._tsiModify1.Size = new System.Drawing.Size(117, 22);
            this._tsiModify1.Text = "Modify";
            // 
            // _tsiRemove1
            // 
            this._tsiRemove1.Name = "_tsiRemove1";
            this._tsiRemove1.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove1.Text = "Remove";
            // 
            // _tsiCreate1
            // 
            this._tsiCreate1.Name = "_tsiCreate1";
            this._tsiCreate1.Size = new System.Drawing.Size(117, 22);
            this._tsiCreate1.Text = "Create";
            // 
            // _lInfoEventlogExpirationTimeZone
            // 
            this._lInfoEventlogExpirationTimeZone.AutoSize = true;
            this._lInfoEventlogExpirationTimeZone.Location = new System.Drawing.Point(6, 16);
            this._lInfoEventlogExpirationTimeZone.Name = "_lInfoEventlogExpirationTimeZone";
            this._lInfoEventlogExpirationTimeZone.Size = new System.Drawing.Size(25, 13);
            this._lInfoEventlogExpirationTimeZone.TabIndex = 0;
            this._lInfoEventlogExpirationTimeZone.Text = "Info";
            // 
            // _bForceEvenlogExpirationClean
            // 
            this._bForceEvenlogExpirationClean.Location = new System.Drawing.Point(6, 195);
            this._bForceEvenlogExpirationClean.Name = "_bForceEvenlogExpirationClean";
            this._bForceEvenlogExpirationClean.Size = new System.Drawing.Size(156, 23);
            this._bForceEvenlogExpirationClean.TabIndex = 6;
            this._bForceEvenlogExpirationClean.Text = "Force Eventlog clean";
            this._bForceEvenlogExpirationClean.UseVisualStyleBackColor = true;
            this._bForceEvenlogExpirationClean.Click += new System.EventHandler(this._bForceEvenlogExpirationClean_Click);
            // 
            // _eResultMaxEventlogRecords
            // 
            this._eResultMaxEventlogRecords.Location = new System.Drawing.Point(268, 74);
            this._eResultMaxEventlogRecords.Name = "_eResultMaxEventlogRecords";
            this._eResultMaxEventlogRecords.ReadOnly = true;
            this._eResultMaxEventlogRecords.Size = new System.Drawing.Size(100, 20);
            this._eResultMaxEventlogRecords.TabIndex = 5;
            // 
            // _lMaxEventlogsRecords
            // 
            this._lMaxEventlogsRecords.AutoSize = true;
            this._lMaxEventlogsRecords.Location = new System.Drawing.Point(3, 55);
            this._lMaxEventlogsRecords.Name = "_lMaxEventlogsRecords";
            this._lMaxEventlogsRecords.Size = new System.Drawing.Size(127, 13);
            this._lMaxEventlogsRecords.TabIndex = 4;
            this._lMaxEventlogsRecords.Text = "Maximal eventlog records";
            // 
            // _tbMaxEventlogSlider
            // 
            this._tbMaxEventlogSlider.BackColor = System.Drawing.SystemColors.Window;
            this._tbMaxEventlogSlider.Location = new System.Drawing.Point(147, 74);
            this._tbMaxEventlogSlider.Maximum = 7;
            this._tbMaxEventlogSlider.Minimum = 1;
            this._tbMaxEventlogSlider.Name = "_tbMaxEventlogSlider";
            this._tbMaxEventlogSlider.Size = new System.Drawing.Size(104, 45);
            this._tbMaxEventlogSlider.TabIndex = 3;
            this._tbMaxEventlogSlider.Value = 1;
            this._tbMaxEventlogSlider.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // _eMaxEventlogRecords
            // 
            this._eMaxEventlogRecords.Location = new System.Drawing.Point(6, 74);
            this._eMaxEventlogRecords.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this._eMaxEventlogRecords.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eMaxEventlogRecords.Name = "_eMaxEventlogRecords";
            this._eMaxEventlogRecords.Size = new System.Drawing.Size(120, 20);
            this._eMaxEventlogRecords.TabIndex = 2;
            this._eMaxEventlogRecords.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this._eMaxEventlogRecords.ValueChanged += new System.EventHandler(this.MaxEventlogRecordsValueChanged);
            // 
            // _lDaysExpiration
            // 
            this._lDaysExpiration.AutoSize = true;
            this._lDaysExpiration.Location = new System.Drawing.Point(3, 16);
            this._lDaysExpiration.Name = "_lDaysExpiration";
            this._lDaysExpiration.Size = new System.Drawing.Size(77, 13);
            this._lDaysExpiration.TabIndex = 1;
            this._lDaysExpiration.Text = "Dates to expiry";
            // 
            // _eCountDaysExpiration
            // 
            this._eCountDaysExpiration.Location = new System.Drawing.Point(6, 32);
            this._eCountDaysExpiration.Maximum = new decimal(new int[] {
            1825,
            0,
            0,
            0});
            this._eCountDaysExpiration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eCountDaysExpiration.Name = "_eCountDaysExpiration";
            this._eCountDaysExpiration.Size = new System.Drawing.Size(120, 20);
            this._eCountDaysExpiration.TabIndex = 0;
            this._eCountDaysExpiration.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eCountDaysExpiration.ValueChanged += new System.EventHandler(this.EditTextChangerEvenlogExpiationSettings);
            // 
            // _gbDatabaseBackup
            // 
            this._gbDatabaseBackup.Controls.Add(this._bClearAndSaveDbsBackup);
            this._gbDatabaseBackup.Controls.Add(this._gbBackupPath);
            this._gbDatabaseBackup.Controls.Add(this._pgForceDbsBackup);
            this._gbDatabaseBackup.Controls.Add(this._bSaveDatabaseBackup);
            this._gbDatabaseBackup.Controls.Add(this._gbTimeZone);
            this._gbDatabaseBackup.Controls.Add(this._bForceDBSBackup);
            this._gbDatabaseBackup.Location = new System.Drawing.Point(6, 6);
            this._gbDatabaseBackup.Name = "_gbDatabaseBackup";
            this._gbDatabaseBackup.Size = new System.Drawing.Size(774, 230);
            this._gbDatabaseBackup.TabIndex = 5;
            this._gbDatabaseBackup.TabStop = false;
            this._gbDatabaseBackup.Text = "Database backup";
            // 
            // _bClearAndSaveDbsBackup
            // 
            this._bClearAndSaveDbsBackup.Location = new System.Drawing.Point(89, 201);
            this._bClearAndSaveDbsBackup.Name = "_bClearAndSaveDbsBackup";
            this._bClearAndSaveDbsBackup.Size = new System.Drawing.Size(121, 23);
            this._bClearAndSaveDbsBackup.TabIndex = 5;
            this._bClearAndSaveDbsBackup.Text = "Clear and save";
            this._bClearAndSaveDbsBackup.UseVisualStyleBackColor = true;
            this._bClearAndSaveDbsBackup.Click += new System.EventHandler(this._bClearAndSaveDbsBackup_Click);
            // 
            // _gbBackupPath
            // 
            this._gbBackupPath.Controls.Add(this._bDbsBackupPath);
            this._gbBackupPath.Controls.Add(this._eDbsBackupPath);
            this._gbBackupPath.Location = new System.Drawing.Point(6, 19);
            this._gbBackupPath.Name = "_gbBackupPath";
            this._gbBackupPath.Size = new System.Drawing.Size(463, 52);
            this._gbBackupPath.TabIndex = 0;
            this._gbBackupPath.TabStop = false;
            this._gbBackupPath.Text = "Backup path";
            // 
            // _bDbsBackupPath
            // 
            this._bDbsBackupPath.Location = new System.Drawing.Point(428, 17);
            this._bDbsBackupPath.Name = "_bDbsBackupPath";
            this._bDbsBackupPath.Size = new System.Drawing.Size(29, 23);
            this._bDbsBackupPath.TabIndex = 1;
            this._bDbsBackupPath.UseVisualStyleBackColor = true;
            this._bDbsBackupPath.Click += new System.EventHandler(this._bDbsBackupPath_Click);
            // 
            // _eDbsBackupPath
            // 
            this._eDbsBackupPath.Location = new System.Drawing.Point(6, 19);
            this._eDbsBackupPath.Name = "_eDbsBackupPath";
            this._eDbsBackupPath.Size = new System.Drawing.Size(416, 20);
            this._eDbsBackupPath.TabIndex = 0;
            this._eDbsBackupPath.TextChanged += new System.EventHandler(this.EditTextChangerDatabaseBackupSettings);
            // 
            // _pgForceDbsBackup
            // 
            this._pgForceDbsBackup.Location = new System.Drawing.Point(6, 182);
            this._pgForceDbsBackup.Maximum = 60;
            this._pgForceDbsBackup.Name = "_pgForceDbsBackup";
            this._pgForceDbsBackup.Size = new System.Drawing.Size(156, 10);
            this._pgForceDbsBackup.TabIndex = 3;
            this._pgForceDbsBackup.Visible = false;
            // 
            // _bSaveDatabaseBackup
            // 
            this._bSaveDatabaseBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSaveDatabaseBackup.Location = new System.Drawing.Point(6, 201);
            this._bSaveDatabaseBackup.Name = "_bSaveDatabaseBackup";
            this._bSaveDatabaseBackup.Size = new System.Drawing.Size(75, 23);
            this._bSaveDatabaseBackup.TabIndex = 4;
            this._bSaveDatabaseBackup.Text = "Save";
            this._bSaveDatabaseBackup.UseVisualStyleBackColor = true;
            this._bSaveDatabaseBackup.Click += new System.EventHandler(this._bSaveDatabaseBackup_Click);
            // 
            // _gbTimeZone
            // 
            this._gbTimeZone.Controls.Add(this._tbmTimeZone);
            this._gbTimeZone.Controls.Add(this._lInfoDatabaseBackup);
            this._gbTimeZone.Location = new System.Drawing.Point(6, 77);
            this._gbTimeZone.Name = "_gbTimeZone";
            this._gbTimeZone.Size = new System.Drawing.Size(463, 70);
            this._gbTimeZone.TabIndex = 1;
            this._gbTimeZone.TabStop = false;
            this._gbTimeZone.Text = "Time zone";
            // 
            // _tbmTimeZone
            // 
            this._tbmTimeZone.AllowDrop = true;
            this._tbmTimeZone.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmTimeZone.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZone.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmTimeZone.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmTimeZone.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZone.Button.Image")));
            this._tbmTimeZone.Button.Location = new System.Drawing.Point(425, 0);
            this._tbmTimeZone.Button.Name = "_bMenu";
            this._tbmTimeZone.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmTimeZone.Button.TabIndex = 3;
            this._tbmTimeZone.Button.UseVisualStyleBackColor = false;
            this._tbmTimeZone.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmTimeZone.ButtonDefaultBehaviour = true;
            this._tbmTimeZone.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmTimeZone.ButtonImage = null;
            // 
            // 
            // 
            this._tbmTimeZone.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiRemove,
            this._tsiCreate});
            this._tbmTimeZone.ButtonPopupMenu.Name = "";
            this._tbmTimeZone.ButtonPopupMenu.Size = new System.Drawing.Size(118, 70);
            this._tbmTimeZone.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmTimeZone.ButtonShowImage = true;
            this._tbmTimeZone.ButtonSizeHeight = 20;
            this._tbmTimeZone.ButtonSizeWidth = 20;
            this._tbmTimeZone.ButtonText = "";
            this._tbmTimeZone.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZone.HoverTime = 500;
            // 
            // 
            // 
            this._tbmTimeZone.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZone.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmTimeZone.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmTimeZone.ImageTextBox.ContextMenuStrip = this._tbmTimeZone.ButtonPopupMenu;
            this._tbmTimeZone.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZone.ImageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._tbmTimeZone.ImageTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this._tbmTimeZone.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZone.ImageTextBox.Image")));
            this._tbmTimeZone.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmTimeZone.ImageTextBox.Name = "_textBox";
            this._tbmTimeZone.ImageTextBox.NoTextNoImage = true;
            this._tbmTimeZone.ImageTextBox.ReadOnly = true;
            this._tbmTimeZone.ImageTextBox.Size = new System.Drawing.Size(425, 20);
            this._tbmTimeZone.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmTimeZone.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZone.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmTimeZone.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmTimeZone.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZone.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmTimeZone.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmTimeZone.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmTimeZone.ImageTextBox.TextBox.Size = new System.Drawing.Size(423, 13);
            this._tbmTimeZone.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmTimeZone.ImageTextBox.UseImage = true;
            this._tbmTimeZone.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmTimeZone_TextBox_DoubleClick);
            this._tbmTimeZone.Location = new System.Drawing.Point(6, 32);
            this._tbmTimeZone.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmTimeZone.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmTimeZone.Name = "_tbmTimeZone";
            this._tbmTimeZone.Size = new System.Drawing.Size(445, 22);
            this._tbmTimeZone.TabIndex = 1;
            this._tbmTimeZone.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZone.TextImage")));
            this._tbmTimeZone.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmTimeZone_PopupMenuItemClick);
            this._tbmTimeZone.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmTimeZone_DragDrop);
            this._tbmTimeZone.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmTimeZone_DragOver);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(117, 22);
            this._tsiModify.Text = "Modify";
            // 
            // _tsiRemove
            // 
            this._tsiRemove.Name = "_tsiRemove";
            this._tsiRemove.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove.Text = "Remove";
            // 
            // _tsiCreate
            // 
            this._tsiCreate.Name = "_tsiCreate";
            this._tsiCreate.Size = new System.Drawing.Size(117, 22);
            this._tsiCreate.Text = "Create";
            // 
            // _lInfoDatabaseBackup
            // 
            this._lInfoDatabaseBackup.AutoSize = true;
            this._lInfoDatabaseBackup.Location = new System.Drawing.Point(6, 16);
            this._lInfoDatabaseBackup.Name = "_lInfoDatabaseBackup";
            this._lInfoDatabaseBackup.Size = new System.Drawing.Size(25, 13);
            this._lInfoDatabaseBackup.TabIndex = 0;
            this._lInfoDatabaseBackup.Text = "Info";
            // 
            // _bForceDBSBackup
            // 
            this._bForceDBSBackup.Location = new System.Drawing.Point(6, 153);
            this._bForceDBSBackup.Name = "_bForceDBSBackup";
            this._bForceDBSBackup.Size = new System.Drawing.Size(156, 23);
            this._bForceDBSBackup.TabIndex = 2;
            this._bForceDBSBackup.Text = "Force DBS backup";
            this._bForceDBSBackup.UseVisualStyleBackColor = true;
            this._bForceDBSBackup.Click += new System.EventHandler(this._bForceDBSBackup_Click);
            // 
            // _tpSecuritySettings
            // 
            this._tpSecuritySettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpSecuritySettings.Controls.Add(this._epgSecuritySettings);
            this._tpSecuritySettings.Controls.Add(this._buttonSaveSecuritySettings);
            this._tpSecuritySettings.Location = new System.Drawing.Point(4, 22);
            this._tpSecuritySettings.Name = "_tpSecuritySettings";
            this._tpSecuritySettings.Size = new System.Drawing.Size(961, 607);
            this._tpSecuritySettings.TabIndex = 9;
            this._tpSecuritySettings.Text = "Security settings";
            // 
            // _epgSecuritySettings
            // 
            this._epgSecuritySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._epgSecuritySettings.ButtonTextAddProperty = "Add property";
            this._epgSecuritySettings.ButtonTextRemoveProperty = "Remove property";
            // 
            // 
            // 
            this._epgSecuritySettings.DocCommentDescription.AutoEllipsis = true;
            this._epgSecuritySettings.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgSecuritySettings.DocCommentDescription.Location = new System.Drawing.Point(3, 19);
            this._epgSecuritySettings.DocCommentDescription.Name = "";
            this._epgSecuritySettings.DocCommentDescription.Size = new System.Drawing.Size(949, 36);
            this._epgSecuritySettings.DocCommentDescription.TabIndex = 1;
            this._epgSecuritySettings.DocCommentImage = null;
            // 
            // 
            // 
            this._epgSecuritySettings.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgSecuritySettings.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this._epgSecuritySettings.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this._epgSecuritySettings.DocCommentTitle.Name = "";
            this._epgSecuritySettings.DocCommentTitle.Size = new System.Drawing.Size(949, 16);
            this._epgSecuritySettings.DocCommentTitle.TabIndex = 0;
            this._epgSecuritySettings.DocCommentTitle.UseMnemonic = false;
            this._epgSecuritySettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._epgSecuritySettings.LineColor = System.Drawing.SystemColors.ControlDark;
            this._epgSecuritySettings.Location = new System.Drawing.Point(3, 3);
            this._epgSecuritySettings.Name = "_epgSecuritySettings";
            this._epgSecuritySettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this._epgSecuritySettings.Size = new System.Drawing.Size(955, 486);
            this._epgSecuritySettings.TabIndex = 2;
            this._epgSecuritySettings.TabStop = false;
            this._epgSecuritySettings.ToolbarVisible = false;
            // 
            // 
            // 
            this._epgSecuritySettings.ToolStrip.AccessibleName = "ToolBar";
            this._epgSecuritySettings.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this._epgSecuritySettings.ToolStrip.AllowMerge = false;
            this._epgSecuritySettings.ToolStrip.AutoSize = false;
            this._epgSecuritySettings.ToolStrip.CanOverflow = false;
            this._epgSecuritySettings.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._epgSecuritySettings.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._epgSecuritySettings.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this._epgSecuritySettings.ToolStrip.Name = "ExtendedPropertyToolStrip";
            this._epgSecuritySettings.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this._epgSecuritySettings.ToolStrip.Size = new System.Drawing.Size(130, 25);
            this._epgSecuritySettings.ToolStrip.TabIndex = 1;
            this._epgSecuritySettings.ToolStrip.TabStop = true;
            this._epgSecuritySettings.ToolStrip.Text = "PropertyGridToolBar";
            this._epgSecuritySettings.ToolStrip.Visible = false;
            this._epgSecuritySettings.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this._epgSecuritySettings_PropertyValueChanged);
            // 
            // _buttonSaveSecuritySettings
            // 
            this._buttonSaveSecuritySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._buttonSaveSecuritySettings.Location = new System.Drawing.Point(8, 495);
            this._buttonSaveSecuritySettings.Name = "_buttonSaveSecuritySettings";
            this._buttonSaveSecuritySettings.Size = new System.Drawing.Size(75, 23);
            this._buttonSaveSecuritySettings.TabIndex = 1;
            this._buttonSaveSecuritySettings.Text = "Save";
            this._buttonSaveSecuritySettings.UseVisualStyleBackColor = true;
            this._buttonSaveSecuritySettings.Click += new System.EventHandler(this._buttonSaveSecuritySettings_Click);
            // 
            // _tpAlarmSettings
            // 
            this._tpAlarmSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpAlarmSettings.Controls.Add(this._cdgvAlarmSettings);
            this._tpAlarmSettings.Controls.Add(this._bSave5);
            this._tpAlarmSettings.Location = new System.Drawing.Point(4, 22);
            this._tpAlarmSettings.Name = "_tpAlarmSettings";
            this._tpAlarmSettings.Size = new System.Drawing.Size(961, 607);
            this._tpAlarmSettings.TabIndex = 15;
            this._tpAlarmSettings.Text = "Alarm settings";
            this._tpAlarmSettings.Enter += new System.EventHandler(this._tpAlarmSettings_Enter);
            // 
            // _cdgvAlarmSettings
            // 
            this._cdgvAlarmSettings.AllwaysRefreshOrder = false;
            this._cdgvAlarmSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvAlarmSettings.CgpDataGridEvents = null;
            this._cdgvAlarmSettings.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvAlarmSettings.DataGrid.AllowUserToAddRows = false;
            this._cdgvAlarmSettings.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvAlarmSettings.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvAlarmSettings.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvAlarmSettings.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvAlarmSettings.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvAlarmSettings.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvAlarmSettings.DataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._cdgvAlarmSettings.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvAlarmSettings.DataGrid.Name = "_dgvData";
            this._cdgvAlarmSettings.DataGrid.RowHeadersVisible = false;
            this._cdgvAlarmSettings.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            this._cdgvAlarmSettings.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this._cdgvAlarmSettings.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvAlarmSettings.DataGrid.Size = new System.Drawing.Size(950, 486);
            this._cdgvAlarmSettings.DataGrid.TabIndex = 0;
            this._cdgvAlarmSettings.LocalizationHelper = null;
            this._cdgvAlarmSettings.Location = new System.Drawing.Point(3, 3);
            this._cdgvAlarmSettings.Name = "_cdgvAlarmSettings";
            this._cdgvAlarmSettings.Size = new System.Drawing.Size(950, 486);
            this._cdgvAlarmSettings.TabIndex = 5;
            // 
            // _bSave5
            // 
            this._bSave5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSave5.Location = new System.Drawing.Point(8, 495);
            this._bSave5.Name = "_bSave5";
            this._bSave5.Size = new System.Drawing.Size(75, 23);
            this._bSave5.TabIndex = 4;
            this._bSave5.Text = "Save";
            this._bSave5.UseVisualStyleBackColor = true;
            this._bSave5.Click += new System.EventHandler(this._bSave5_Click);
            // 
            // _tpUiSettings
            // 
            this._tpUiSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpUiSettings.Controls.Add(this._gbAutoclose);
            this._tpUiSettings.Controls.Add(this._gbColorSettings);
            this._tpUiSettings.Location = new System.Drawing.Point(4, 22);
            this._tpUiSettings.Name = "_tpUiSettings";
            this._tpUiSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpUiSettings.Size = new System.Drawing.Size(961, 607);
            this._tpUiSettings.TabIndex = 10;
            this._tpUiSettings.Text = "UI settings";
            // 
            // _gbAutoclose
            // 
            this._gbAutoclose.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbAutoclose.Controls.Add(this._buttonSave);
            this._gbAutoclose.Controls.Add(this._lAutocloseRange);
            this._gbAutoclose.Controls.Add(this._labelTimeout);
            this._gbAutoclose.Controls.Add(this._cbAutocCoseTurnedOn);
            this._gbAutoclose.Controls.Add(this._nUpDoAutoCloseTimeout);
            this._gbAutoclose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbAutoclose.Location = new System.Drawing.Point(3, 296);
            this._gbAutoclose.Name = "_gbAutoclose";
            this._gbAutoclose.Size = new System.Drawing.Size(955, 136);
            this._gbAutoclose.TabIndex = 10;
            this._gbAutoclose.TabStop = false;
            this._gbAutoclose.Text = "Autoclose tab settings";
            // 
            // _buttonSave
            // 
            this._buttonSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._buttonSave.Location = new System.Drawing.Point(6, 99);
            this._buttonSave.Name = "_buttonSave";
            this._buttonSave.Size = new System.Drawing.Size(75, 23);
            this._buttonSave.TabIndex = 12;
            this._buttonSave.Text = "Save";
            this._buttonSave.UseVisualStyleBackColor = true;
            this._buttonSave.Click += new System.EventHandler(this._buttonSave_Click);
            // 
            // _lAutocloseRange
            // 
            this._lAutocloseRange.AutoSize = true;
            this._lAutocloseRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lAutocloseRange.Location = new System.Drawing.Point(213, 29);
            this._lAutocloseRange.Name = "_lAutocloseRange";
            this._lAutocloseRange.Size = new System.Drawing.Size(71, 13);
            this._lAutocloseRange.TabIndex = 11;
            this._lAutocloseRange.Text = "min (1 - 1440)";
            // 
            // _labelTimeout
            // 
            this._labelTimeout.AutoSize = true;
            this._labelTimeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._labelTimeout.Location = new System.Drawing.Point(9, 28);
            this._labelTimeout.Name = "_labelTimeout";
            this._labelTimeout.Size = new System.Drawing.Size(123, 13);
            this._labelTimeout.TabIndex = 10;
            this._labelTimeout.Text = "Close after inactivity time";
            // 
            // _cbAutocCoseTurnedOn
            // 
            this._cbAutocCoseTurnedOn.AutoSize = true;
            this._cbAutocCoseTurnedOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._cbAutocCoseTurnedOn.Location = new System.Drawing.Point(12, 65);
            this._cbAutocCoseTurnedOn.Name = "_cbAutocCoseTurnedOn";
            this._cbAutocCoseTurnedOn.Size = new System.Drawing.Size(65, 17);
            this._cbAutocCoseTurnedOn.TabIndex = 9;
            this._cbAutocCoseTurnedOn.Text = "Enabled";
            this._cbAutocCoseTurnedOn.UseVisualStyleBackColor = true;
            this._cbAutocCoseTurnedOn.CheckStateChanged += new System.EventHandler(this._cbAutocCoseTurnedOn_CheckStateChanged);
            // 
            // _nUpDoAutoCloseTimeout
            // 
            this._nUpDoAutoCloseTimeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._nUpDoAutoCloseTimeout.Location = new System.Drawing.Point(164, 26);
            this._nUpDoAutoCloseTimeout.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this._nUpDoAutoCloseTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nUpDoAutoCloseTimeout.Name = "_nUpDoAutoCloseTimeout";
            this._nUpDoAutoCloseTimeout.Size = new System.Drawing.Size(45, 20);
            this._nUpDoAutoCloseTimeout.TabIndex = 8;
            this._nUpDoAutoCloseTimeout.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this._nUpDoAutoCloseTimeout.ValueChanged += new System.EventHandler(this.EditTextChangerAutoCloseSettings);
            // 
            // _gbColorSettings
            // 
            this._gbColorSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbColorSettings.Controls.Add(this._gbAlarmNotAcknowledged);
            this._gbColorSettings.Controls.Add(this._gbAlarmColour);
            this._gbColorSettings.Controls.Add(this._bSave);
            this._gbColorSettings.Controls.Add(this._gbNormalNotAcknowledgedColour);
            this._gbColorSettings.Controls.Add(this._gbReferenceObjectsColour);
            this._gbColorSettings.Controls.Add(this._gbNormalColour);
            this._gbColorSettings.Controls.Add(this._gbDropDownColour);
            this._gbColorSettings.Controls.Add(this._gbNoAlarmsInQueueColour);
            this._gbColorSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbColorSettings.Location = new System.Drawing.Point(3, 6);
            this._gbColorSettings.Name = "_gbColorSettings";
            this._gbColorSettings.Size = new System.Drawing.Size(955, 284);
            this._gbColorSettings.TabIndex = 9;
            this._gbColorSettings.TabStop = false;
            this._gbColorSettings.Text = "Color setting";
            // 
            // _gbAlarmNotAcknowledged
            // 
            this._gbAlarmNotAcknowledged.Controls.Add(this._bAlarmNotAcknowledgedColorBackground);
            this._gbAlarmNotAcknowledged.Controls.Add(this._bAlarmNotAcknowledgedColorText);
            this._gbAlarmNotAcknowledged.Controls.Add(this._eAlarmNotAcknowledgedPreview);
            this._gbAlarmNotAcknowledged.Controls.Add(this._lColourBackground);
            this._gbAlarmNotAcknowledged.Controls.Add(this._lColourText);
            this._gbAlarmNotAcknowledged.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbAlarmNotAcknowledged.Location = new System.Drawing.Point(6, 21);
            this._gbAlarmNotAcknowledged.Name = "_gbAlarmNotAcknowledged";
            this._gbAlarmNotAcknowledged.Size = new System.Drawing.Size(370, 50);
            this._gbAlarmNotAcknowledged.TabIndex = 0;
            this._gbAlarmNotAcknowledged.TabStop = false;
            this._gbAlarmNotAcknowledged.Text = "Alarm not acknowledged";
            // 
            // _bAlarmNotAcknowledgedColorBackground
            // 
            this._bAlarmNotAcknowledgedColorBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bAlarmNotAcknowledgedColorBackground.Location = new System.Drawing.Point(124, 19);
            this._bAlarmNotAcknowledgedColorBackground.Name = "_bAlarmNotAcknowledgedColorBackground";
            this._bAlarmNotAcknowledgedColorBackground.Size = new System.Drawing.Size(20, 20);
            this._bAlarmNotAcknowledgedColorBackground.TabIndex = 2;
            this._bAlarmNotAcknowledgedColorBackground.UseVisualStyleBackColor = true;
            this._bAlarmNotAcknowledgedColorBackground.Click += new System.EventHandler(this.SetAlarmNotAcknowledgedColorBackground);
            // 
            // _bAlarmNotAcknowledgedColorText
            // 
            this._bAlarmNotAcknowledgedColorText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bAlarmNotAcknowledgedColorText.Location = new System.Drawing.Point(6, 19);
            this._bAlarmNotAcknowledgedColorText.Name = "_bAlarmNotAcknowledgedColorText";
            this._bAlarmNotAcknowledgedColorText.Size = new System.Drawing.Size(20, 20);
            this._bAlarmNotAcknowledgedColorText.TabIndex = 0;
            this._bAlarmNotAcknowledgedColorText.UseVisualStyleBackColor = true;
            this._bAlarmNotAcknowledgedColorText.Click += new System.EventHandler(this.SetAlarmNotAcknowledgedColorText);
            // 
            // _eAlarmNotAcknowledgedPreview
            // 
            this._eAlarmNotAcknowledgedPreview.Location = new System.Drawing.Point(260, 19);
            this._eAlarmNotAcknowledgedPreview.Name = "_eAlarmNotAcknowledgedPreview";
            this._eAlarmNotAcknowledgedPreview.Size = new System.Drawing.Size(100, 20);
            this._eAlarmNotAcknowledgedPreview.TabIndex = 4;
            this._eAlarmNotAcknowledgedPreview.TextChanged += new System.EventHandler(this.EditTextChangerColorSettings);
            // 
            // _lColourBackground
            // 
            this._lColourBackground.AutoSize = true;
            this._lColourBackground.Location = new System.Drawing.Point(150, 23);
            this._lColourBackground.Name = "_lColourBackground";
            this._lColourBackground.Size = new System.Drawing.Size(97, 13);
            this._lColourBackground.TabIndex = 3;
            this._lColourBackground.Text = "Colour background";
            this._lColourBackground.Click += new System.EventHandler(this.SetAlarmNotAcknowledgedColorBackground);
            // 
            // _lColourText
            // 
            this._lColourText.AutoSize = true;
            this._lColourText.Location = new System.Drawing.Point(32, 23);
            this._lColourText.Name = "_lColourText";
            this._lColourText.Size = new System.Drawing.Size(57, 13);
            this._lColourText.TabIndex = 1;
            this._lColourText.Text = "Colour text";
            this._lColourText.Click += new System.EventHandler(this.SetAlarmNotAcknowledgedColorText);
            // 
            // _gbAlarmColour
            // 
            this._gbAlarmColour.Controls.Add(this._bAlarmColorBackground);
            this._gbAlarmColour.Controls.Add(this._bAlarmColorText);
            this._gbAlarmColour.Controls.Add(this._eAlarmPreview);
            this._gbAlarmColour.Controls.Add(this._lColourBackground1);
            this._gbAlarmColour.Controls.Add(this._lColourText1);
            this._gbAlarmColour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbAlarmColour.Location = new System.Drawing.Point(382, 21);
            this._gbAlarmColour.Name = "_gbAlarmColour";
            this._gbAlarmColour.Size = new System.Drawing.Size(370, 50);
            this._gbAlarmColour.TabIndex = 1;
            this._gbAlarmColour.TabStop = false;
            this._gbAlarmColour.Text = "Alarm";
            // 
            // _bAlarmColorBackground
            // 
            this._bAlarmColorBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bAlarmColorBackground.Location = new System.Drawing.Point(114, 19);
            this._bAlarmColorBackground.Name = "_bAlarmColorBackground";
            this._bAlarmColorBackground.Size = new System.Drawing.Size(20, 20);
            this._bAlarmColorBackground.TabIndex = 2;
            this._bAlarmColorBackground.UseVisualStyleBackColor = true;
            this._bAlarmColorBackground.Click += new System.EventHandler(this.SetAlarmColorBackground);
            // 
            // _bAlarmColorText
            // 
            this._bAlarmColorText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bAlarmColorText.Location = new System.Drawing.Point(6, 19);
            this._bAlarmColorText.Name = "_bAlarmColorText";
            this._bAlarmColorText.Size = new System.Drawing.Size(20, 20);
            this._bAlarmColorText.TabIndex = 0;
            this._bAlarmColorText.UseVisualStyleBackColor = true;
            this._bAlarmColorText.Click += new System.EventHandler(this.SetAlarmColorText);
            // 
            // _eAlarmPreview
            // 
            this._eAlarmPreview.Location = new System.Drawing.Point(260, 19);
            this._eAlarmPreview.Name = "_eAlarmPreview";
            this._eAlarmPreview.Size = new System.Drawing.Size(100, 20);
            this._eAlarmPreview.TabIndex = 4;
            this._eAlarmPreview.TextChanged += new System.EventHandler(this.EditTextChangerColorSettings);
            // 
            // _lColourBackground1
            // 
            this._lColourBackground1.AutoSize = true;
            this._lColourBackground1.Location = new System.Drawing.Point(150, 23);
            this._lColourBackground1.Name = "_lColourBackground1";
            this._lColourBackground1.Size = new System.Drawing.Size(97, 13);
            this._lColourBackground1.TabIndex = 3;
            this._lColourBackground1.Text = "Colour background";
            this._lColourBackground1.Click += new System.EventHandler(this.SetAlarmColorBackground);
            // 
            // _lColourText1
            // 
            this._lColourText1.AutoSize = true;
            this._lColourText1.Location = new System.Drawing.Point(32, 23);
            this._lColourText1.Name = "_lColourText1";
            this._lColourText1.Size = new System.Drawing.Size(57, 13);
            this._lColourText1.TabIndex = 1;
            this._lColourText1.Text = "Colour text";
            this._lColourText1.Click += new System.EventHandler(this.SetAlarmColorText);
            // 
            // _bSave
            // 
            this._bSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._bSave.Location = new System.Drawing.Point(6, 245);
            this._bSave.Name = "_bSave";
            this._bSave.Size = new System.Drawing.Size(75, 23);
            this._bSave.TabIndex = 7;
            this._bSave.Text = "Save";
            this._bSave.UseVisualStyleBackColor = true;
            this._bSave.Click += new System.EventHandler(this._bSave_Click);
            // 
            // _gbNormalNotAcknowledgedColour
            // 
            this._gbNormalNotAcknowledgedColour.Controls.Add(this._bNormalNotAcknowledgedColorBackground);
            this._gbNormalNotAcknowledgedColour.Controls.Add(this._bNormalNotAcknowledgedColorText);
            this._gbNormalNotAcknowledgedColour.Controls.Add(this._eNormalNotAcknowledgedPreview);
            this._gbNormalNotAcknowledgedColour.Controls.Add(this._lColourBackground2);
            this._gbNormalNotAcknowledgedColour.Controls.Add(this._lColourText2);
            this._gbNormalNotAcknowledgedColour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbNormalNotAcknowledgedColour.Location = new System.Drawing.Point(6, 77);
            this._gbNormalNotAcknowledgedColour.Name = "_gbNormalNotAcknowledgedColour";
            this._gbNormalNotAcknowledgedColour.Size = new System.Drawing.Size(370, 50);
            this._gbNormalNotAcknowledgedColour.TabIndex = 2;
            this._gbNormalNotAcknowledgedColour.TabStop = false;
            this._gbNormalNotAcknowledgedColour.Text = "Normal not acknowledged";
            // 
            // _bNormalNotAcknowledgedColorBackground
            // 
            this._bNormalNotAcknowledgedColorBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bNormalNotAcknowledgedColorBackground.Location = new System.Drawing.Point(124, 19);
            this._bNormalNotAcknowledgedColorBackground.Name = "_bNormalNotAcknowledgedColorBackground";
            this._bNormalNotAcknowledgedColorBackground.Size = new System.Drawing.Size(20, 20);
            this._bNormalNotAcknowledgedColorBackground.TabIndex = 2;
            this._bNormalNotAcknowledgedColorBackground.UseVisualStyleBackColor = true;
            this._bNormalNotAcknowledgedColorBackground.Click += new System.EventHandler(this.SetNormalNotAcknowledgedColorBackground);
            // 
            // _bNormalNotAcknowledgedColorText
            // 
            this._bNormalNotAcknowledgedColorText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bNormalNotAcknowledgedColorText.Location = new System.Drawing.Point(6, 19);
            this._bNormalNotAcknowledgedColorText.Name = "_bNormalNotAcknowledgedColorText";
            this._bNormalNotAcknowledgedColorText.Size = new System.Drawing.Size(20, 20);
            this._bNormalNotAcknowledgedColorText.TabIndex = 0;
            this._bNormalNotAcknowledgedColorText.UseVisualStyleBackColor = true;
            this._bNormalNotAcknowledgedColorText.Click += new System.EventHandler(this.SetNormalNotAcknowledgedColorText);
            // 
            // _eNormalNotAcknowledgedPreview
            // 
            this._eNormalNotAcknowledgedPreview.Location = new System.Drawing.Point(260, 19);
            this._eNormalNotAcknowledgedPreview.Name = "_eNormalNotAcknowledgedPreview";
            this._eNormalNotAcknowledgedPreview.Size = new System.Drawing.Size(100, 20);
            this._eNormalNotAcknowledgedPreview.TabIndex = 4;
            this._eNormalNotAcknowledgedPreview.TextChanged += new System.EventHandler(this.EditTextChangerColorSettings);
            // 
            // _lColourBackground2
            // 
            this._lColourBackground2.AutoSize = true;
            this._lColourBackground2.Location = new System.Drawing.Point(150, 23);
            this._lColourBackground2.Name = "_lColourBackground2";
            this._lColourBackground2.Size = new System.Drawing.Size(97, 13);
            this._lColourBackground2.TabIndex = 3;
            this._lColourBackground2.Text = "Colour background";
            this._lColourBackground2.Click += new System.EventHandler(this.SetNormalNotAcknowledgedColorBackground);
            // 
            // _lColourText2
            // 
            this._lColourText2.AutoSize = true;
            this._lColourText2.Location = new System.Drawing.Point(32, 23);
            this._lColourText2.Name = "_lColourText2";
            this._lColourText2.Size = new System.Drawing.Size(57, 13);
            this._lColourText2.TabIndex = 1;
            this._lColourText2.Text = "Colour text";
            this._lColourText2.Click += new System.EventHandler(this.SetNormalNotAcknowledgedColorText);
            // 
            // _gbReferenceObjectsColour
            // 
            this._gbReferenceObjectsColour.Controls.Add(this._bReferenceObjectColorBackground);
            this._gbReferenceObjectsColour.Controls.Add(this._bReferenceObjectColorText);
            this._gbReferenceObjectsColour.Controls.Add(this._eReferenceObjectsPreview);
            this._gbReferenceObjectsColour.Controls.Add(this._lColourBackground6);
            this._gbReferenceObjectsColour.Controls.Add(this._lColourText6);
            this._gbReferenceObjectsColour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbReferenceObjectsColour.Location = new System.Drawing.Point(382, 189);
            this._gbReferenceObjectsColour.Name = "_gbReferenceObjectsColour";
            this._gbReferenceObjectsColour.Size = new System.Drawing.Size(370, 50);
            this._gbReferenceObjectsColour.TabIndex = 6;
            this._gbReferenceObjectsColour.TabStop = false;
            this._gbReferenceObjectsColour.Text = "Reference objects";
            // 
            // _bReferenceObjectColorBackground
            // 
            this._bReferenceObjectColorBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bReferenceObjectColorBackground.Location = new System.Drawing.Point(124, 20);
            this._bReferenceObjectColorBackground.Name = "_bReferenceObjectColorBackground";
            this._bReferenceObjectColorBackground.Size = new System.Drawing.Size(20, 20);
            this._bReferenceObjectColorBackground.TabIndex = 2;
            this._bReferenceObjectColorBackground.UseVisualStyleBackColor = true;
            this._bReferenceObjectColorBackground.Click += new System.EventHandler(this.SetReferenceObjectColorBackground);
            // 
            // _bReferenceObjectColorText
            // 
            this._bReferenceObjectColorText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bReferenceObjectColorText.Location = new System.Drawing.Point(6, 20);
            this._bReferenceObjectColorText.Name = "_bReferenceObjectColorText";
            this._bReferenceObjectColorText.Size = new System.Drawing.Size(20, 20);
            this._bReferenceObjectColorText.TabIndex = 0;
            this._bReferenceObjectColorText.UseVisualStyleBackColor = true;
            this._bReferenceObjectColorText.Click += new System.EventHandler(this.SetReferenceObjectColorText);
            // 
            // _eReferenceObjectsPreview
            // 
            this._eReferenceObjectsPreview.Location = new System.Drawing.Point(260, 19);
            this._eReferenceObjectsPreview.Name = "_eReferenceObjectsPreview";
            this._eReferenceObjectsPreview.Size = new System.Drawing.Size(100, 20);
            this._eReferenceObjectsPreview.TabIndex = 4;
            this._eReferenceObjectsPreview.TextChanged += new System.EventHandler(this.EditTextChangerColorSettings);
            // 
            // _lColourBackground6
            // 
            this._lColourBackground6.AutoSize = true;
            this._lColourBackground6.Location = new System.Drawing.Point(150, 23);
            this._lColourBackground6.Name = "_lColourBackground6";
            this._lColourBackground6.Size = new System.Drawing.Size(97, 13);
            this._lColourBackground6.TabIndex = 3;
            this._lColourBackground6.Text = "Colour background";
            this._lColourBackground6.Click += new System.EventHandler(this.SetReferenceObjectColorBackground);
            // 
            // _lColourText6
            // 
            this._lColourText6.AutoSize = true;
            this._lColourText6.Location = new System.Drawing.Point(32, 23);
            this._lColourText6.Name = "_lColourText6";
            this._lColourText6.Size = new System.Drawing.Size(57, 13);
            this._lColourText6.TabIndex = 1;
            this._lColourText6.Text = "Colour text";
            this._lColourText6.Click += new System.EventHandler(this.SetReferenceObjectColorText);
            // 
            // _gbNormalColour
            // 
            this._gbNormalColour.Controls.Add(this._bNormalColorBackground);
            this._gbNormalColour.Controls.Add(this._bNormalColorText);
            this._gbNormalColour.Controls.Add(this._eNormalPreview);
            this._gbNormalColour.Controls.Add(this._lColourBackground3);
            this._gbNormalColour.Controls.Add(this._lColourText3);
            this._gbNormalColour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbNormalColour.Location = new System.Drawing.Point(382, 77);
            this._gbNormalColour.Name = "_gbNormalColour";
            this._gbNormalColour.Size = new System.Drawing.Size(370, 50);
            this._gbNormalColour.TabIndex = 3;
            this._gbNormalColour.TabStop = false;
            this._gbNormalColour.Text = "Normal";
            // 
            // _bNormalColorBackground
            // 
            this._bNormalColorBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bNormalColorBackground.Location = new System.Drawing.Point(114, 19);
            this._bNormalColorBackground.Name = "_bNormalColorBackground";
            this._bNormalColorBackground.Size = new System.Drawing.Size(20, 20);
            this._bNormalColorBackground.TabIndex = 2;
            this._bNormalColorBackground.UseVisualStyleBackColor = true;
            this._bNormalColorBackground.Click += new System.EventHandler(this.SetNormalColorBackground);
            // 
            // _bNormalColorText
            // 
            this._bNormalColorText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bNormalColorText.Location = new System.Drawing.Point(6, 19);
            this._bNormalColorText.Name = "_bNormalColorText";
            this._bNormalColorText.Size = new System.Drawing.Size(20, 20);
            this._bNormalColorText.TabIndex = 0;
            this._bNormalColorText.UseVisualStyleBackColor = true;
            this._bNormalColorText.Click += new System.EventHandler(this.SetNormalColorText);
            // 
            // _eNormalPreview
            // 
            this._eNormalPreview.Location = new System.Drawing.Point(260, 19);
            this._eNormalPreview.Name = "_eNormalPreview";
            this._eNormalPreview.Size = new System.Drawing.Size(100, 20);
            this._eNormalPreview.TabIndex = 4;
            this._eNormalPreview.TextChanged += new System.EventHandler(this.EditTextChangerColorSettings);
            // 
            // _lColourBackground3
            // 
            this._lColourBackground3.AutoSize = true;
            this._lColourBackground3.Location = new System.Drawing.Point(150, 23);
            this._lColourBackground3.Name = "_lColourBackground3";
            this._lColourBackground3.Size = new System.Drawing.Size(97, 13);
            this._lColourBackground3.TabIndex = 3;
            this._lColourBackground3.Text = "Colour background";
            this._lColourBackground3.Click += new System.EventHandler(this.SetNormalColorBackground);
            // 
            // _lColourText3
            // 
            this._lColourText3.AutoSize = true;
            this._lColourText3.Location = new System.Drawing.Point(32, 23);
            this._lColourText3.Name = "_lColourText3";
            this._lColourText3.Size = new System.Drawing.Size(57, 13);
            this._lColourText3.TabIndex = 1;
            this._lColourText3.Text = "Colour text";
            this._lColourText3.Click += new System.EventHandler(this.SetNormalColorText);
            // 
            // _gbDropDownColour
            // 
            this._gbDropDownColour.Controls.Add(this._bDragDropColorBackground);
            this._gbDropDownColour.Controls.Add(this._bDragDropColorText);
            this._gbDropDownColour.Controls.Add(this._eDropDownPreview);
            this._gbDropDownColour.Controls.Add(this._lColourBackground5);
            this._gbDropDownColour.Controls.Add(this._lColourText5);
            this._gbDropDownColour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbDropDownColour.Location = new System.Drawing.Point(6, 189);
            this._gbDropDownColour.Name = "_gbDropDownColour";
            this._gbDropDownColour.Size = new System.Drawing.Size(370, 50);
            this._gbDropDownColour.TabIndex = 5;
            this._gbDropDownColour.TabStop = false;
            this._gbDropDownColour.Text = "Drop down";
            // 
            // _bDragDropColorBackground
            // 
            this._bDragDropColorBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bDragDropColorBackground.Location = new System.Drawing.Point(124, 19);
            this._bDragDropColorBackground.Name = "_bDragDropColorBackground";
            this._bDragDropColorBackground.Size = new System.Drawing.Size(20, 20);
            this._bDragDropColorBackground.TabIndex = 2;
            this._bDragDropColorBackground.UseVisualStyleBackColor = true;
            this._bDragDropColorBackground.Click += new System.EventHandler(this.DragDropBackColour);
            // 
            // _bDragDropColorText
            // 
            this._bDragDropColorText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bDragDropColorText.Location = new System.Drawing.Point(6, 19);
            this._bDragDropColorText.Name = "_bDragDropColorText";
            this._bDragDropColorText.Size = new System.Drawing.Size(20, 20);
            this._bDragDropColorText.TabIndex = 0;
            this._bDragDropColorText.UseVisualStyleBackColor = true;
            this._bDragDropColorText.Click += new System.EventHandler(this.SetDragDropColorText);
            // 
            // _eDropDownPreview
            // 
            this._eDropDownPreview.Location = new System.Drawing.Point(260, 19);
            this._eDropDownPreview.Name = "_eDropDownPreview";
            this._eDropDownPreview.Size = new System.Drawing.Size(100, 20);
            this._eDropDownPreview.TabIndex = 4;
            this._eDropDownPreview.TextChanged += new System.EventHandler(this.EditTextChangerColorSettings);
            // 
            // _lColourBackground5
            // 
            this._lColourBackground5.AutoSize = true;
            this._lColourBackground5.Location = new System.Drawing.Point(150, 23);
            this._lColourBackground5.Name = "_lColourBackground5";
            this._lColourBackground5.Size = new System.Drawing.Size(97, 13);
            this._lColourBackground5.TabIndex = 3;
            this._lColourBackground5.Text = "Colour background";
            this._lColourBackground5.Click += new System.EventHandler(this.DragDropBackColour);
            // 
            // _lColourText5
            // 
            this._lColourText5.AutoSize = true;
            this._lColourText5.Location = new System.Drawing.Point(32, 23);
            this._lColourText5.Name = "_lColourText5";
            this._lColourText5.Size = new System.Drawing.Size(57, 13);
            this._lColourText5.TabIndex = 1;
            this._lColourText5.Text = "Colour text";
            this._lColourText5.Click += new System.EventHandler(this.SetDragDropColorText);
            // 
            // _gbNoAlarmsInQueueColour
            // 
            this._gbNoAlarmsInQueueColour.Controls.Add(this._bNoAlarmsInQueueColorBackground);
            this._gbNoAlarmsInQueueColour.Controls.Add(this._bNoAlarmsInQueueColorText);
            this._gbNoAlarmsInQueueColour.Controls.Add(this._eNoAlarmQueuePreview);
            this._gbNoAlarmsInQueueColour.Controls.Add(this._lColourBackground4);
            this._gbNoAlarmsInQueueColour.Controls.Add(this._lColourText4);
            this._gbNoAlarmsInQueueColour.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._gbNoAlarmsInQueueColour.Location = new System.Drawing.Point(6, 133);
            this._gbNoAlarmsInQueueColour.Name = "_gbNoAlarmsInQueueColour";
            this._gbNoAlarmsInQueueColour.Size = new System.Drawing.Size(370, 50);
            this._gbNoAlarmsInQueueColour.TabIndex = 4;
            this._gbNoAlarmsInQueueColour.TabStop = false;
            this._gbNoAlarmsInQueueColour.Text = "No alarms in queue";
            // 
            // _bNoAlarmsInQueueColorBackground
            // 
            this._bNoAlarmsInQueueColorBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bNoAlarmsInQueueColorBackground.Location = new System.Drawing.Point(124, 19);
            this._bNoAlarmsInQueueColorBackground.Name = "_bNoAlarmsInQueueColorBackground";
            this._bNoAlarmsInQueueColorBackground.Size = new System.Drawing.Size(20, 20);
            this._bNoAlarmsInQueueColorBackground.TabIndex = 2;
            this._bNoAlarmsInQueueColorBackground.UseVisualStyleBackColor = true;
            this._bNoAlarmsInQueueColorBackground.Click += new System.EventHandler(this.SetNoAlarmsInQueueColorBackground);
            // 
            // _bNoAlarmsInQueueColorText
            // 
            this._bNoAlarmsInQueueColorText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bNoAlarmsInQueueColorText.Location = new System.Drawing.Point(6, 19);
            this._bNoAlarmsInQueueColorText.Name = "_bNoAlarmsInQueueColorText";
            this._bNoAlarmsInQueueColorText.Size = new System.Drawing.Size(20, 20);
            this._bNoAlarmsInQueueColorText.TabIndex = 0;
            this._bNoAlarmsInQueueColorText.UseVisualStyleBackColor = true;
            this._bNoAlarmsInQueueColorText.Click += new System.EventHandler(this.SetNoAlarmsInQueueColorText);
            // 
            // _eNoAlarmQueuePreview
            // 
            this._eNoAlarmQueuePreview.Location = new System.Drawing.Point(260, 19);
            this._eNoAlarmQueuePreview.Name = "_eNoAlarmQueuePreview";
            this._eNoAlarmQueuePreview.Size = new System.Drawing.Size(100, 20);
            this._eNoAlarmQueuePreview.TabIndex = 4;
            this._eNoAlarmQueuePreview.TextChanged += new System.EventHandler(this.EditTextChangerColorSettings);
            // 
            // _lColourBackground4
            // 
            this._lColourBackground4.AutoSize = true;
            this._lColourBackground4.Location = new System.Drawing.Point(150, 23);
            this._lColourBackground4.Name = "_lColourBackground4";
            this._lColourBackground4.Size = new System.Drawing.Size(97, 13);
            this._lColourBackground4.TabIndex = 3;
            this._lColourBackground4.Text = "Colour background";
            this._lColourBackground4.Click += new System.EventHandler(this.SetNoAlarmsInQueueColorBackground);
            // 
            // _lColourText4
            // 
            this._lColourText4.AutoSize = true;
            this._lColourText4.Location = new System.Drawing.Point(32, 23);
            this._lColourText4.Name = "_lColourText4";
            this._lColourText4.Size = new System.Drawing.Size(57, 13);
            this._lColourText4.TabIndex = 1;
            this._lColourText4.Text = "Colour text";
            this._lColourText4.Click += new System.EventHandler(this.SetNoAlarmsInQueueColorText);
            // 
            // _tpEventlogs
            // 
            this._tpEventlogs.BackColor = System.Drawing.SystemColors.Control;
            this._tpEventlogs.Controls.Add(this._gbEventlogReports);
            this._tpEventlogs.Controls.Add(this._cbEventSourcesReverseOrder);
            this._tpEventlogs.Controls.Add(this._bEventlogsSave);
            this._tpEventlogs.Controls.Add(this._cbEventlogCardReaderOnlineStateChanged);
            this._tpEventlogs.Controls.Add(this._cbEventlogAlarmAreaActivationStateChanged);
            this._tpEventlogs.Controls.Add(this._cbEventlogAlarmAreaAlarmStateChanged);
            this._tpEventlogs.Controls.Add(this._cbEventlogOutputStateChanged);
            this._tpEventlogs.Controls.Add(this._cbEventlogInputStateChanged);
            this._tpEventlogs.Location = new System.Drawing.Point(4, 22);
            this._tpEventlogs.Name = "_tpEventlogs";
            this._tpEventlogs.Size = new System.Drawing.Size(961, 607);
            this._tpEventlogs.TabIndex = 11;
            this._tpEventlogs.Text = "Eventlogs";
            // 
            // _gbEventlogReports
            // 
            this._gbEventlogReports.Controls.Add(this._lblReportsEmails);
            this._gbEventlogReports.Controls.Add(this._lblReportsTimezone);
            this._gbEventlogReports.Controls.Add(this._eEventlogsReportsEmails);
            this._gbEventlogReports.Controls.Add(this._tbmEventlogsReportsTimezone);
            this._gbEventlogReports.Location = new System.Drawing.Point(8, 173);
            this._gbEventlogReports.Name = "_gbEventlogReports";
            this._gbEventlogReports.Size = new System.Drawing.Size(267, 163);
            this._gbEventlogReports.TabIndex = 7;
            this._gbEventlogReports.TabStop = false;
            this._gbEventlogReports.Text = "Eventlog Reports";
            // 
            // _lblReportsEmails
            // 
            this._lblReportsEmails.AutoSize = true;
            this._lblReportsEmails.Location = new System.Drawing.Point(14, 95);
            this._lblReportsEmails.Name = "_lblReportsEmails";
            this._lblReportsEmails.Size = new System.Drawing.Size(79, 13);
            this._lblReportsEmails.TabIndex = 7;
            this._lblReportsEmails.Text = "Reports emails:";
            // 
            // _lblReportsTimezone
            // 
            this._lblReportsTimezone.AutoSize = true;
            this._lblReportsTimezone.Location = new System.Drawing.Point(14, 28);
            this._lblReportsTimezone.Name = "_lblReportsTimezone";
            this._lblReportsTimezone.Size = new System.Drawing.Size(92, 13);
            this._lblReportsTimezone.TabIndex = 6;
            this._lblReportsTimezone.Text = "Reports timezone:";
            // 
            // _eEventlogsReportsEmails
            // 
            this._eEventlogsReportsEmails.Location = new System.Drawing.Point(17, 120);
            this._eEventlogsReportsEmails.Name = "_eEventlogsReportsEmails";
            this._eEventlogsReportsEmails.Size = new System.Drawing.Size(229, 20);
            this._eEventlogsReportsEmails.TabIndex = 5;
            // 
            // _tbmEventlogsReportsTimezone
            // 
            this._tbmEventlogsReportsTimezone.AllowDrop = true;
            this._tbmEventlogsReportsTimezone.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmEventlogsReportsTimezone.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEventlogsReportsTimezone.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmEventlogsReportsTimezone.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmEventlogsReportsTimezone.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEventlogsReportsTimezone.Button.Image")));
            this._tbmEventlogsReportsTimezone.Button.Location = new System.Drawing.Point(209, 0);
            this._tbmEventlogsReportsTimezone.Button.Name = "_bMenu";
            this._tbmEventlogsReportsTimezone.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmEventlogsReportsTimezone.Button.TabIndex = 3;
            this._tbmEventlogsReportsTimezone.Button.UseVisualStyleBackColor = false;
            this._tbmEventlogsReportsTimezone.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmEventlogsReportsTimezone.ButtonDefaultBehaviour = true;
            this._tbmEventlogsReportsTimezone.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmEventlogsReportsTimezone.ButtonImage = null;
            // 
            // 
            // 
            this._tbmEventlogsReportsTimezone.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2});
            this._tbmEventlogsReportsTimezone.ButtonPopupMenu.Name = "";
            this._tbmEventlogsReportsTimezone.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmEventlogsReportsTimezone.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmEventlogsReportsTimezone.ButtonShowImage = true;
            this._tbmEventlogsReportsTimezone.ButtonSizeHeight = 20;
            this._tbmEventlogsReportsTimezone.ButtonSizeWidth = 20;
            this._tbmEventlogsReportsTimezone.ButtonText = "";
            this._tbmEventlogsReportsTimezone.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEventlogsReportsTimezone.HoverTime = 500;
            // 
            // 
            // 
            this._tbmEventlogsReportsTimezone.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEventlogsReportsTimezone.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEventlogsReportsTimezone.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmEventlogsReportsTimezone.ImageTextBox.ContextMenuStrip = this._tbmEventlogsReportsTimezone.ButtonPopupMenu;
            this._tbmEventlogsReportsTimezone.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEventlogsReportsTimezone.ImageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._tbmEventlogsReportsTimezone.ImageTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this._tbmEventlogsReportsTimezone.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEventlogsReportsTimezone.ImageTextBox.Image")));
            this._tbmEventlogsReportsTimezone.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmEventlogsReportsTimezone.ImageTextBox.Name = "_textBox";
            this._tbmEventlogsReportsTimezone.ImageTextBox.NoTextNoImage = true;
            this._tbmEventlogsReportsTimezone.ImageTextBox.ReadOnly = true;
            this._tbmEventlogsReportsTimezone.ImageTextBox.Size = new System.Drawing.Size(209, 20);
            this._tbmEventlogsReportsTimezone.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.Size = new System.Drawing.Size(207, 13);
            this._tbmEventlogsReportsTimezone.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmEventlogsReportsTimezone.ImageTextBox.UseImage = true;
            this._tbmEventlogsReportsTimezone.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmEventlogsReportsTimezone_TextBox_DoubleClick);
            this._tbmEventlogsReportsTimezone.Location = new System.Drawing.Point(17, 55);
            this._tbmEventlogsReportsTimezone.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmEventlogsReportsTimezone.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmEventlogsReportsTimezone.Name = "_tbmEventlogsReportsTimezone";
            this._tbmEventlogsReportsTimezone.Size = new System.Drawing.Size(229, 22);
            this._tbmEventlogsReportsTimezone.TabIndex = 4;
            this._tbmEventlogsReportsTimezone.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmEventlogsReportsTimezone.TextImage")));
            this._tbmEventlogsReportsTimezone.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmEventlogsReportsTimezone_PopupMenuItemClick);
            this._tbmEventlogsReportsTimezone.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmEventlogsReportsTimezone_DragDrop);
            this._tbmEventlogsReportsTimezone.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmEventlogsReportsTimezone_DragOver);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(117, 22);
            this._tsiModify2.Text = "Modify";
            // 
            // _tsiRemove2
            // 
            this._tsiRemove2.Name = "_tsiRemove2";
            this._tsiRemove2.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove2.Text = "Remove";
            // 
            // _cbEventSourcesReverseOrder
            // 
            this._cbEventSourcesReverseOrder.AutoSize = true;
            this._cbEventSourcesReverseOrder.Location = new System.Drawing.Point(8, 130);
            this._cbEventSourcesReverseOrder.Name = "_cbEventSourcesReverseOrder";
            this._cbEventSourcesReverseOrder.Size = new System.Drawing.Size(251, 17);
            this._cbEventSourcesReverseOrder.TabIndex = 6;
            this._cbEventSourcesReverseOrder.Text = "Event sources of the table view in reverse order";
            this._cbEventSourcesReverseOrder.UseVisualStyleBackColor = true;
            this._cbEventSourcesReverseOrder.CheckedChanged += new System.EventHandler(this.EditTextChangerEventlogSettings);
            // 
            // _bEventlogsSave
            // 
            this._bEventlogsSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bEventlogsSave.Location = new System.Drawing.Point(8, 495);
            this._bEventlogsSave.Name = "_bEventlogsSave";
            this._bEventlogsSave.Size = new System.Drawing.Size(75, 23);
            this._bEventlogsSave.TabIndex = 5;
            this._bEventlogsSave.Text = "Save";
            this._bEventlogsSave.UseVisualStyleBackColor = true;
            this._bEventlogsSave.Click += new System.EventHandler(this._bEventlogsSave_Click);
            // 
            // _cbEventlogCardReaderOnlineStateChanged
            // 
            this._cbEventlogCardReaderOnlineStateChanged.AutoSize = true;
            this._cbEventlogCardReaderOnlineStateChanged.Location = new System.Drawing.Point(8, 107);
            this._cbEventlogCardReaderOnlineStateChanged.Name = "_cbEventlogCardReaderOnlineStateChanged";
            this._cbEventlogCardReaderOnlineStateChanged.Size = new System.Drawing.Size(254, 17);
            this._cbEventlogCardReaderOnlineStateChanged.TabIndex = 4;
            this._cbEventlogCardReaderOnlineStateChanged.Text = "Save eventlog card reader online state changed";
            this._cbEventlogCardReaderOnlineStateChanged.UseVisualStyleBackColor = true;
            this._cbEventlogCardReaderOnlineStateChanged.CheckedChanged += new System.EventHandler(this.EditTextChangerEventlogSettings);
            // 
            // _cbEventlogAlarmAreaActivationStateChanged
            // 
            this._cbEventlogAlarmAreaActivationStateChanged.AutoSize = true;
            this._cbEventlogAlarmAreaActivationStateChanged.Location = new System.Drawing.Point(8, 84);
            this._cbEventlogAlarmAreaActivationStateChanged.Name = "_cbEventlogAlarmAreaActivationStateChanged";
            this._cbEventlogAlarmAreaActivationStateChanged.Size = new System.Drawing.Size(267, 17);
            this._cbEventlogAlarmAreaActivationStateChanged.TabIndex = 3;
            this._cbEventlogAlarmAreaActivationStateChanged.Text = "Save eventlog alarm area activation state changed";
            this._cbEventlogAlarmAreaActivationStateChanged.UseVisualStyleBackColor = true;
            this._cbEventlogAlarmAreaActivationStateChanged.CheckedChanged += new System.EventHandler(this.EditTextChangerEventlogSettings);
            // 
            // _cbEventlogAlarmAreaAlarmStateChanged
            // 
            this._cbEventlogAlarmAreaAlarmStateChanged.AutoSize = true;
            this._cbEventlogAlarmAreaAlarmStateChanged.Location = new System.Drawing.Point(8, 61);
            this._cbEventlogAlarmAreaAlarmStateChanged.Name = "_cbEventlogAlarmAreaAlarmStateChanged";
            this._cbEventlogAlarmAreaAlarmStateChanged.Size = new System.Drawing.Size(246, 17);
            this._cbEventlogAlarmAreaAlarmStateChanged.TabIndex = 2;
            this._cbEventlogAlarmAreaAlarmStateChanged.Text = "Save eventlog alarm area alarm state changed";
            this._cbEventlogAlarmAreaAlarmStateChanged.UseVisualStyleBackColor = true;
            this._cbEventlogAlarmAreaAlarmStateChanged.CheckedChanged += new System.EventHandler(this.EditTextChangerEventlogSettings);
            // 
            // _cbEventlogOutputStateChanged
            // 
            this._cbEventlogOutputStateChanged.AutoSize = true;
            this._cbEventlogOutputStateChanged.Location = new System.Drawing.Point(8, 38);
            this._cbEventlogOutputStateChanged.Name = "_cbEventlogOutputStateChanged";
            this._cbEventlogOutputStateChanged.Size = new System.Drawing.Size(199, 17);
            this._cbEventlogOutputStateChanged.TabIndex = 1;
            this._cbEventlogOutputStateChanged.Text = "Save eventlog output state changed";
            this._cbEventlogOutputStateChanged.UseVisualStyleBackColor = true;
            this._cbEventlogOutputStateChanged.CheckedChanged += new System.EventHandler(this.EditTextChangerEventlogSettings);
            // 
            // _cbEventlogInputStateChanged
            // 
            this._cbEventlogInputStateChanged.AutoSize = true;
            this._cbEventlogInputStateChanged.Location = new System.Drawing.Point(8, 15);
            this._cbEventlogInputStateChanged.Name = "_cbEventlogInputStateChanged";
            this._cbEventlogInputStateChanged.Size = new System.Drawing.Size(192, 17);
            this._cbEventlogInputStateChanged.TabIndex = 0;
            this._cbEventlogInputStateChanged.Text = "Save eventlog input state changed";
            this._cbEventlogInputStateChanged.UseVisualStyleBackColor = true;
            this._cbEventlogInputStateChanged.CheckedChanged += new System.EventHandler(this.EditTextChangerEventlogSettings);
            // 
            // _tpLanguage
            // 
            this._tpLanguage.BackColor = System.Drawing.SystemColors.Control;
            this._tpLanguage.Controls.Add(this._bSave2);
            this._tpLanguage.Location = new System.Drawing.Point(4, 22);
            this._tpLanguage.Name = "_tpLanguage";
            this._tpLanguage.Size = new System.Drawing.Size(961, 607);
            this._tpLanguage.TabIndex = 12;
            this._tpLanguage.Text = "Language settings";
            this._tpLanguage.Enter += new System.EventHandler(this._tpLanguage_Enter);
            // 
            // _bSave2
            // 
            this._bSave2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSave2.Location = new System.Drawing.Point(8, 495);
            this._bSave2.Name = "_bSave2";
            this._bSave2.Size = new System.Drawing.Size(75, 23);
            this._bSave2.TabIndex = 0;
            this._bSave2.Text = "Save";
            this._bSave2.UseVisualStyleBackColor = true;
            this._bSave2.Click += new System.EventHandler(this._bSave2_Click);
            // 
            // _tpAdvancedAccessSettings
            // 
            this._tpAdvancedAccessSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpAdvancedAccessSettings.Controls.Add(this._chbAlarmAreaRestrictivePolicyForTimeBuying);
            this._tpAdvancedAccessSettings.Controls.Add(this._chbEnableLoggingSDPSTZChanges);
            this._tpAdvancedAccessSettings.Controls.Add(this._gbSyncingOfTimeFromServerSettings);
            this._tpAdvancedAccessSettings.Controls.Add(this._bSave3);
            this._tpAdvancedAccessSettings.Location = new System.Drawing.Point(4, 22);
            this._tpAdvancedAccessSettings.Name = "_tpAdvancedAccessSettings";
            this._tpAdvancedAccessSettings.Size = new System.Drawing.Size(961, 607);
            this._tpAdvancedAccessSettings.TabIndex = 13;
            this._tpAdvancedAccessSettings.Text = "Advanced access settings";
            // 
            // _chbAlarmAreaRestrictivePolicyForTimeBuying
            // 
            this._chbAlarmAreaRestrictivePolicyForTimeBuying.AutoSize = true;
            this._chbAlarmAreaRestrictivePolicyForTimeBuying.Location = new System.Drawing.Point(8, 142);
            this._chbAlarmAreaRestrictivePolicyForTimeBuying.Name = "_chbAlarmAreaRestrictivePolicyForTimeBuying";
            this._chbAlarmAreaRestrictivePolicyForTimeBuying.Size = new System.Drawing.Size(225, 17);
            this._chbAlarmAreaRestrictivePolicyForTimeBuying.TabIndex = 13;
            this._chbAlarmAreaRestrictivePolicyForTimeBuying.Text = "Alarm area restrictive policy for time buying";
            this._chbAlarmAreaRestrictivePolicyForTimeBuying.UseVisualStyleBackColor = true;
            this._chbAlarmAreaRestrictivePolicyForTimeBuying.CheckedChanged += new System.EventHandler(this.EditTextChangerAdvancedAccessSettings);
            // 
            // _chbEnableLoggingSDPSTZChanges
            // 
            this._chbEnableLoggingSDPSTZChanges.AutoSize = true;
            this._chbEnableLoggingSDPSTZChanges.Location = new System.Drawing.Point(8, 14);
            this._chbEnableLoggingSDPSTZChanges.Name = "_chbEnableLoggingSDPSTZChanges";
            this._chbEnableLoggingSDPSTZChanges.Size = new System.Drawing.Size(270, 17);
            this._chbEnableLoggingSDPSTZChanges.TabIndex = 12;
            this._chbEnableLoggingSDPSTZChanges.Text = "Enable logging of SDP/STZ changes into database";
            this._chbEnableLoggingSDPSTZChanges.UseVisualStyleBackColor = true;
            this._chbEnableLoggingSDPSTZChanges.CheckedChanged += new System.EventHandler(this.EditTextChangerAdvancedAccessSettings);
            // 
            // _gbSyncingOfTimeFromServerSettings
            // 
            this._gbSyncingOfTimeFromServerSettings.Controls.Add(this._ePeriodicTimeSyncTolerance);
            this._gbSyncingOfTimeFromServerSettings.Controls.Add(this._lPeriodicTimeSyncTolerance);
            this._gbSyncingOfTimeFromServerSettings.Controls.Add(this._ePeriodOfTimeSyncWithoutStratum);
            this._gbSyncingOfTimeFromServerSettings.Controls.Add(this._lPeriodOfTimeSyncWithoutStratum);
            this._gbSyncingOfTimeFromServerSettings.Controls.Add(this._cbSyncingTimeFromServer);
            this._gbSyncingOfTimeFromServerSettings.Location = new System.Drawing.Point(8, 37);
            this._gbSyncingOfTimeFromServerSettings.Name = "_gbSyncingOfTimeFromServerSettings";
            this._gbSyncingOfTimeFromServerSettings.Size = new System.Drawing.Size(382, 99);
            this._gbSyncingOfTimeFromServerSettings.TabIndex = 11;
            this._gbSyncingOfTimeFromServerSettings.TabStop = false;
            this._gbSyncingOfTimeFromServerSettings.Text = "Syncing of time from Nova Server settings";
            // 
            // _ePeriodicTimeSyncTolerance
            // 
            this._ePeriodicTimeSyncTolerance.Location = new System.Drawing.Point(223, 68);
            this._ePeriodicTimeSyncTolerance.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this._ePeriodicTimeSyncTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._ePeriodicTimeSyncTolerance.Name = "_ePeriodicTimeSyncTolerance";
            this._ePeriodicTimeSyncTolerance.Size = new System.Drawing.Size(120, 20);
            this._ePeriodicTimeSyncTolerance.TabIndex = 13;
            this._ePeriodicTimeSyncTolerance.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this._ePeriodicTimeSyncTolerance.ValueChanged += new System.EventHandler(this.EditTextChangerAdvancedAccessSettings);
            // 
            // _lPeriodicTimeSyncTolerance
            // 
            this._lPeriodicTimeSyncTolerance.AutoSize = true;
            this._lPeriodicTimeSyncTolerance.Location = new System.Drawing.Point(6, 70);
            this._lPeriodicTimeSyncTolerance.Name = "_lPeriodicTimeSyncTolerance";
            this._lPeriodicTimeSyncTolerance.Size = new System.Drawing.Size(165, 13);
            this._lPeriodicTimeSyncTolerance.TabIndex = 12;
            this._lPeriodicTimeSyncTolerance.Text = "Periodic time sync tolerance (sec)";
            // 
            // _ePeriodOfTimeSyncWithoutStratum
            // 
            this._ePeriodOfTimeSyncWithoutStratum.Location = new System.Drawing.Point(223, 42);
            this._ePeriodOfTimeSyncWithoutStratum.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this._ePeriodOfTimeSyncWithoutStratum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._ePeriodOfTimeSyncWithoutStratum.Name = "_ePeriodOfTimeSyncWithoutStratum";
            this._ePeriodOfTimeSyncWithoutStratum.Size = new System.Drawing.Size(120, 20);
            this._ePeriodOfTimeSyncWithoutStratum.TabIndex = 11;
            this._ePeriodOfTimeSyncWithoutStratum.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this._ePeriodOfTimeSyncWithoutStratum.ValueChanged += new System.EventHandler(this.EditTextChangerAdvancedAccessSettings);
            // 
            // _lPeriodOfTimeSyncWithoutStratum
            // 
            this._lPeriodOfTimeSyncWithoutStratum.AutoSize = true;
            this._lPeriodOfTimeSyncWithoutStratum.Location = new System.Drawing.Point(6, 44);
            this._lPeriodOfTimeSyncWithoutStratum.Name = "_lPeriodOfTimeSyncWithoutStratum";
            this._lPeriodOfTimeSyncWithoutStratum.Size = new System.Drawing.Size(195, 13);
            this._lPeriodOfTimeSyncWithoutStratum.TabIndex = 11;
            this._lPeriodOfTimeSyncWithoutStratum.Text = "Period of time sync without stratum (min)";
            // 
            // _cbSyncingTimeFromServer
            // 
            this._cbSyncingTimeFromServer.AutoSize = true;
            this._cbSyncingTimeFromServer.Location = new System.Drawing.Point(6, 19);
            this._cbSyncingTimeFromServer.Name = "_cbSyncingTimeFromServer";
            this._cbSyncingTimeFromServer.Size = new System.Drawing.Size(337, 17);
            this._cbSyncingTimeFromServer.TabIndex = 10;
            this._cbSyncingTimeFromServer.Text = "Periodic syncing of time from Nova Server without stratum analysis";
            this._cbSyncingTimeFromServer.UseVisualStyleBackColor = true;
            this._cbSyncingTimeFromServer.CheckedChanged += new System.EventHandler(this.EditTextChangerAdvancedAccessSettings);
            // 
            // _bSave3
            // 
            this._bSave3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSave3.Location = new System.Drawing.Point(8, 495);
            this._bSave3.Name = "_bSave3";
            this._bSave3.Size = new System.Drawing.Size(75, 23);
            this._bSave3.TabIndex = 6;
            this._bSave3.Text = "Save";
            this._bSave3.UseVisualStyleBackColor = true;
            this._bSave3.Click += new System.EventHandler(this._bSave3_Click);
            // 
            // _tpAdvancedSettings
            // 
            this._tpAdvancedSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpAdvancedSettings.Controls.Add(this._lDelayForSendingChangesToCcu);
            this._tpAdvancedSettings.Controls.Add(this._eDelayForSendingChangesToCcu);
            this._tpAdvancedSettings.Controls.Add(this._lAlarmListSuspendedRefreshTimeout);
            this._tpAdvancedSettings.Controls.Add(this._eAlarmListSuspendedRefreshTimeout);
            this._tpAdvancedSettings.Controls.Add(this._cbCorrectDeserializationFailures);
            this._tpAdvancedSettings.Controls.Add(this._lClientSessionTimeout);
            this._tpAdvancedSettings.Controls.Add(this._eClientSessionTimeout);
            this._tpAdvancedSettings.Controls.Add(this._bSave4);
            this._tpAdvancedSettings.Controls.Add(this._lDelayForSaveEvents);
            this._tpAdvancedSettings.Controls.Add(this._eDelayForSaveEvents);
            this._tpAdvancedSettings.Controls.Add(this._lMaxEventsCountForInsert);
            this._tpAdvancedSettings.Controls.Add(this._eMaxEventsCountForInsert);
            this._tpAdvancedSettings.Location = new System.Drawing.Point(4, 22);
            this._tpAdvancedSettings.Name = "_tpAdvancedSettings";
            this._tpAdvancedSettings.Size = new System.Drawing.Size(961, 607);
            this._tpAdvancedSettings.TabIndex = 14;
            this._tpAdvancedSettings.Text = "Advanced settings";
            // 
            // _lDelayForSendingChangesToCcu
            // 
            this._lDelayForSendingChangesToCcu.AutoSize = true;
            this._lDelayForSendingChangesToCcu.Location = new System.Drawing.Point(8, 118);
            this._lDelayForSendingChangesToCcu.Name = "_lDelayForSendingChangesToCcu";
            this._lDelayForSendingChangesToCcu.Size = new System.Drawing.Size(204, 13);
            this._lDelayForSendingChangesToCcu.TabIndex = 14;
            this._lDelayForSendingChangesToCcu.Text = "The dealy for sending changes to CCU (s)";
            // 
            // _eDelayForSendingChangesToCcu
            // 
            this._eDelayForSendingChangesToCcu.Location = new System.Drawing.Point(293, 116);
            this._eDelayForSendingChangesToCcu.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this._eDelayForSendingChangesToCcu.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eDelayForSendingChangesToCcu.Name = "_eDelayForSendingChangesToCcu";
            this._eDelayForSendingChangesToCcu.Size = new System.Drawing.Size(120, 20);
            this._eDelayForSendingChangesToCcu.TabIndex = 13;
            this._eDelayForSendingChangesToCcu.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _lAlarmListSuspendedRefreshTimeout
            // 
            this._lAlarmListSuspendedRefreshTimeout.AutoSize = true;
            this._lAlarmListSuspendedRefreshTimeout.Location = new System.Drawing.Point(8, 92);
            this._lAlarmListSuspendedRefreshTimeout.Name = "_lAlarmListSuspendedRefreshTimeout";
            this._lAlarmListSuspendedRefreshTimeout.Size = new System.Drawing.Size(178, 13);
            this._lAlarmListSuspendedRefreshTimeout.TabIndex = 12;
            this._lAlarmListSuspendedRefreshTimeout.Text = "AlarmListSuspendedRefreshTimeout";
            // 
            // _eAlarmListSuspendedRefreshTimeout
            // 
            this._eAlarmListSuspendedRefreshTimeout.Location = new System.Drawing.Point(293, 90);
            this._eAlarmListSuspendedRefreshTimeout.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this._eAlarmListSuspendedRefreshTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eAlarmListSuspendedRefreshTimeout.Name = "_eAlarmListSuspendedRefreshTimeout";
            this._eAlarmListSuspendedRefreshTimeout.Size = new System.Drawing.Size(120, 20);
            this._eAlarmListSuspendedRefreshTimeout.TabIndex = 11;
            this._eAlarmListSuspendedRefreshTimeout.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // _cbCorrectDeserializationFailures
            // 
            this._cbCorrectDeserializationFailures.AutoSize = true;
            this._cbCorrectDeserializationFailures.Location = new System.Drawing.Point(8, 142);
            this._cbCorrectDeserializationFailures.Name = "_cbCorrectDeserializationFailures";
            this._cbCorrectDeserializationFailures.Size = new System.Drawing.Size(254, 17);
            this._cbCorrectDeserializationFailures.TabIndex = 10;
            this._cbCorrectDeserializationFailures.Text = "Automatically correct flash deserialization failures";
            this._cbCorrectDeserializationFailures.UseVisualStyleBackColor = true;
            // 
            // _lClientSessionTimeout
            // 
            this._lClientSessionTimeout.AutoSize = true;
            this._lClientSessionTimeout.Location = new System.Drawing.Point(8, 66);
            this._lClientSessionTimeout.Name = "_lClientSessionTimeout";
            this._lClientSessionTimeout.Size = new System.Drawing.Size(108, 13);
            this._lClientSessionTimeout.TabIndex = 9;
            this._lClientSessionTimeout.Text = "Client session timeout";
            // 
            // _eClientSessionTimeout
            // 
            this._eClientSessionTimeout.Location = new System.Drawing.Point(293, 64);
            this._eClientSessionTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eClientSessionTimeout.Name = "_eClientSessionTimeout";
            this._eClientSessionTimeout.Size = new System.Drawing.Size(120, 20);
            this._eClientSessionTimeout.TabIndex = 8;
            this._eClientSessionTimeout.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this._eClientSessionTimeout.ValueChanged += new System.EventHandler(this.EditTextChangerAdvancedSettings);
            // 
            // _bSave4
            // 
            this._bSave4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSave4.Location = new System.Drawing.Point(8, 495);
            this._bSave4.Name = "_bSave4";
            this._bSave4.Size = new System.Drawing.Size(75, 23);
            this._bSave4.TabIndex = 7;
            this._bSave4.Text = "Save";
            this._bSave4.UseVisualStyleBackColor = true;
            this._bSave4.Click += new System.EventHandler(this._bSave4_Click);
            // 
            // _lDelayForSaveEvents
            // 
            this._lDelayForSaveEvents.AutoSize = true;
            this._lDelayForSaveEvents.Location = new System.Drawing.Point(8, 40);
            this._lDelayForSaveEvents.Name = "_lDelayForSaveEvents";
            this._lDelayForSaveEvents.Size = new System.Drawing.Size(152, 13);
            this._lDelayForSaveEvents.TabIndex = 5;
            this._lDelayForSaveEvents.Text = "The delay for save events (ms)";
            // 
            // _eDelayForSaveEvents
            // 
            this._eDelayForSaveEvents.Location = new System.Drawing.Point(293, 38);
            this._eDelayForSaveEvents.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._eDelayForSaveEvents.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._eDelayForSaveEvents.Name = "_eDelayForSaveEvents";
            this._eDelayForSaveEvents.Size = new System.Drawing.Size(120, 20);
            this._eDelayForSaveEvents.TabIndex = 4;
            this._eDelayForSaveEvents.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._eDelayForSaveEvents.ValueChanged += new System.EventHandler(this.EditTextChangerAdvancedSettings);
            // 
            // _lMaxEventsCountForInsert
            // 
            this._lMaxEventsCountForInsert.AutoSize = true;
            this._lMaxEventsCountForInsert.Location = new System.Drawing.Point(8, 14);
            this._lMaxEventsCountForInsert.Name = "_lMaxEventsCountForInsert";
            this._lMaxEventsCountForInsert.Size = new System.Drawing.Size(228, 13);
            this._lMaxEventsCountForInsert.TabIndex = 3;
            this._lMaxEventsCountForInsert.Text = "The maximum number of events saved at once";
            // 
            // _eMaxEventsCountForInsert
            // 
            this._eMaxEventsCountForInsert.Location = new System.Drawing.Point(293, 12);
            this._eMaxEventsCountForInsert.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._eMaxEventsCountForInsert.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eMaxEventsCountForInsert.Name = "_eMaxEventsCountForInsert";
            this._eMaxEventsCountForInsert.Size = new System.Drawing.Size(120, 20);
            this._eMaxEventsCountForInsert.TabIndex = 2;
            this._eMaxEventsCountForInsert.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eMaxEventsCountForInsert.ValueChanged += new System.EventHandler(this.EditTextChangerAdvancedSettings);
            // 
            // _tpCustomerSupplierInfo
            // 
            this._tpCustomerSupplierInfo.Controls.Add(this._bAdd8);
            this._tpCustomerSupplierInfo.Controls.Add(this._bRemove);
            this._tpCustomerSupplierInfo.Controls.Add(this._lSupplierLogo);
            this._tpCustomerSupplierInfo.Controls.Add(this._pbSupplierLogo);
            this._tpCustomerSupplierInfo.Controls.Add(this._gbSupplierInfo);
            this._tpCustomerSupplierInfo.Controls.Add(this._gbCustomerInfo);
            this._tpCustomerSupplierInfo.Controls.Add(this._bSave6);
            this._tpCustomerSupplierInfo.Location = new System.Drawing.Point(4, 22);
            this._tpCustomerSupplierInfo.Name = "_tpCustomerSupplierInfo";
            this._tpCustomerSupplierInfo.Size = new System.Drawing.Size(961, 607);
            this._tpCustomerSupplierInfo.TabIndex = 16;
            this._tpCustomerSupplierInfo.Text = "Customer & Supplier info";
            this._tpCustomerSupplierInfo.UseVisualStyleBackColor = true;
            // 
            // _bAdd8
            // 
            this._bAdd8.Location = new System.Drawing.Point(99, 551);
            this._bAdd8.Name = "_bAdd8";
            this._bAdd8.Size = new System.Drawing.Size(69, 23);
            this._bAdd8.TabIndex = 16;
            this._bAdd8.Text = "Add";
            this._bAdd8.UseVisualStyleBackColor = true;
            this._bAdd8.Click += new System.EventHandler(this._bAddPicture_Click);
            // 
            // _bRemove
            // 
            this._bRemove.Location = new System.Drawing.Point(174, 551);
            this._bRemove.Name = "_bRemove";
            this._bRemove.Size = new System.Drawing.Size(69, 23);
            this._bRemove.TabIndex = 15;
            this._bRemove.Text = "Remove";
            this._bRemove.UseVisualStyleBackColor = true;
            this._bRemove.Click += new System.EventHandler(this._bRemove_Click);
            // 
            // _lSupplierLogo
            // 
            this._lSupplierLogo.AutoSize = true;
            this._lSupplierLogo.Location = new System.Drawing.Point(3, 410);
            this._lSupplierLogo.Name = "_lSupplierLogo";
            this._lSupplierLogo.Size = new System.Drawing.Size(69, 13);
            this._lSupplierLogo.TabIndex = 14;
            this._lSupplierLogo.Text = "SupplierLogo";
            // 
            // _pbSupplierLogo
            // 
            this._pbSupplierLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pbSupplierLogo.Location = new System.Drawing.Point(3, 426);
            this._pbSupplierLogo.Name = "_pbSupplierLogo";
            this._pbSupplierLogo.Size = new System.Drawing.Size(240, 120);
            this._pbSupplierLogo.TabIndex = 13;
            this._pbSupplierLogo.TabStop = false;
            // 
            // _gbSupplierInfo
            // 
            this._gbSupplierInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbSupplierInfo.Controls.Add(this._epgSupplierInfo);
            this._gbSupplierInfo.Location = new System.Drawing.Point(3, 205);
            this._gbSupplierInfo.Name = "_gbSupplierInfo";
            this._gbSupplierInfo.Size = new System.Drawing.Size(955, 200);
            this._gbSupplierInfo.TabIndex = 12;
            this._gbSupplierInfo.TabStop = false;
            this._gbSupplierInfo.Text = "Supplier info";
            // 
            // _epgSupplierInfo
            // 
            this._epgSupplierInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._epgSupplierInfo.ButtonTextAddProperty = "Add property";
            this._epgSupplierInfo.ButtonTextRemoveProperty = "Remove property";
            // 
            // 
            // 
            this._epgSupplierInfo.DocCommentDescription.AutoEllipsis = true;
            this._epgSupplierInfo.DocCommentDescription.AutoSize = true;
            this._epgSupplierInfo.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgSupplierInfo.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this._epgSupplierInfo.DocCommentDescription.Name = "";
            this._epgSupplierInfo.DocCommentDescription.Size = new System.Drawing.Size(0, 15);
            this._epgSupplierInfo.DocCommentDescription.TabIndex = 1;
            this._epgSupplierInfo.DocCommentImage = null;
            // 
            // 
            // 
            this._epgSupplierInfo.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgSupplierInfo.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this._epgSupplierInfo.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this._epgSupplierInfo.DocCommentTitle.Name = "";
            this._epgSupplierInfo.DocCommentTitle.Size = new System.Drawing.Size(0, 0);
            this._epgSupplierInfo.DocCommentTitle.TabIndex = 0;
            this._epgSupplierInfo.DocCommentTitle.UseMnemonic = false;
            this._epgSupplierInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._epgSupplierInfo.HelpVisible = false;
            this._epgSupplierInfo.LineColor = System.Drawing.SystemColors.ControlDark;
            this._epgSupplierInfo.Location = new System.Drawing.Point(0, 19);
            this._epgSupplierInfo.Name = "_epgSupplierInfo";
            this._epgSupplierInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this._epgSupplierInfo.Size = new System.Drawing.Size(949, 174);
            this._epgSupplierInfo.TabIndex = 10;
            this._epgSupplierInfo.TabStop = false;
            this._epgSupplierInfo.ToolbarVisible = false;
            // 
            // 
            // 
            this._epgSupplierInfo.ToolStrip.AccessibleName = "ToolBar";
            this._epgSupplierInfo.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this._epgSupplierInfo.ToolStrip.AllowMerge = false;
            this._epgSupplierInfo.ToolStrip.AutoSize = false;
            this._epgSupplierInfo.ToolStrip.CanOverflow = false;
            this._epgSupplierInfo.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._epgSupplierInfo.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._epgSupplierInfo.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this._epgSupplierInfo.ToolStrip.Name = "ExtendedPropertyToolStrip";
            this._epgSupplierInfo.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this._epgSupplierInfo.ToolStrip.Size = new System.Drawing.Size(130, 25);
            this._epgSupplierInfo.ToolStrip.TabIndex = 1;
            this._epgSupplierInfo.ToolStrip.TabStop = true;
            this._epgSupplierInfo.ToolStrip.Text = "PropertyGridToolBar";
            this._epgSupplierInfo.ToolStrip.Visible = false;
            this._epgSupplierInfo.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this._epgSupplierInfo_PropertyValueChanged);
            // 
            // _gbCustomerInfo
            // 
            this._gbCustomerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbCustomerInfo.Controls.Add(this._epgCustomerInfo);
            this._gbCustomerInfo.Location = new System.Drawing.Point(0, 3);
            this._gbCustomerInfo.Name = "_gbCustomerInfo";
            this._gbCustomerInfo.Size = new System.Drawing.Size(958, 200);
            this._gbCustomerInfo.TabIndex = 11;
            this._gbCustomerInfo.TabStop = false;
            this._gbCustomerInfo.Text = "Customer info";
            // 
            // _epgCustomerInfo
            // 
            this._epgCustomerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._epgCustomerInfo.ButtonTextAddProperty = "Add property";
            this._epgCustomerInfo.ButtonTextRemoveProperty = "Remove property";
            // 
            // 
            // 
            this._epgCustomerInfo.DocCommentDescription.AutoEllipsis = true;
            this._epgCustomerInfo.DocCommentDescription.AutoSize = true;
            this._epgCustomerInfo.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgCustomerInfo.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this._epgCustomerInfo.DocCommentDescription.Name = "";
            this._epgCustomerInfo.DocCommentDescription.Size = new System.Drawing.Size(0, 15);
            this._epgCustomerInfo.DocCommentDescription.TabIndex = 1;
            this._epgCustomerInfo.DocCommentImage = null;
            // 
            // 
            // 
            this._epgCustomerInfo.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this._epgCustomerInfo.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this._epgCustomerInfo.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this._epgCustomerInfo.DocCommentTitle.Name = "";
            this._epgCustomerInfo.DocCommentTitle.Size = new System.Drawing.Size(0, 0);
            this._epgCustomerInfo.DocCommentTitle.TabIndex = 0;
            this._epgCustomerInfo.DocCommentTitle.UseMnemonic = false;
            this._epgCustomerInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._epgCustomerInfo.HelpVisible = false;
            this._epgCustomerInfo.LineColor = System.Drawing.SystemColors.ControlDark;
            this._epgCustomerInfo.Location = new System.Drawing.Point(3, 18);
            this._epgCustomerInfo.Name = "_epgCustomerInfo";
            this._epgCustomerInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this._epgCustomerInfo.Size = new System.Drawing.Size(949, 180);
            this._epgCustomerInfo.TabIndex = 9;
            this._epgCustomerInfo.TabStop = false;
            this._epgCustomerInfo.ToolbarVisible = false;
            // 
            // 
            // 
            this._epgCustomerInfo.ToolStrip.AccessibleName = "ToolBar";
            this._epgCustomerInfo.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this._epgCustomerInfo.ToolStrip.AllowMerge = false;
            this._epgCustomerInfo.ToolStrip.AutoSize = false;
            this._epgCustomerInfo.ToolStrip.CanOverflow = false;
            this._epgCustomerInfo.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._epgCustomerInfo.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._epgCustomerInfo.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this._epgCustomerInfo.ToolStrip.Name = "ExtendedPropertyToolStrip";
            this._epgCustomerInfo.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this._epgCustomerInfo.ToolStrip.Size = new System.Drawing.Size(130, 25);
            this._epgCustomerInfo.ToolStrip.TabIndex = 1;
            this._epgCustomerInfo.ToolStrip.TabStop = true;
            this._epgCustomerInfo.ToolStrip.Text = "PropertyGridToolBar";
            this._epgCustomerInfo.ToolStrip.Visible = false;
            this._epgCustomerInfo.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this._epgCustomerInfo_PropertyValueChanged);
            // 
            // _bSave6
            // 
            this._bSave6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSave6.Location = new System.Drawing.Point(3, 581);
            this._bSave6.Name = "_bSave6";
            this._bSave6.Size = new System.Drawing.Size(75, 23);
            this._bSave6.TabIndex = 8;
            this._bSave6.Text = "Save";
            this._bSave6.UseVisualStyleBackColor = true;
            this._bSave6.Click += new System.EventHandler(this._bSave6_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(14, 369);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 0;
            this.button4.Text = "Save";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(14, 417);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "Save";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(8, 16);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(112, 17);
            this.checkBox2.TabIndex = 11;
            this.checkBox2.Text = "High security level";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 42);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(249, 56);
            this.listBox1.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(5, 102);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(249, 27);
            this.button1.TabIndex = 10;
            this.button1.Text = "Lookup";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(6, 155);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(109, 20);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 258);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(249, 20);
            this.textBox1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 241);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "AES Key";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(6, 19);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(249, 20);
            this.textBox2.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(14, 369);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "IP Address";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 221);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(95, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "AES Remoting";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(6, 195);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(249, 20);
            this.textBox3.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Firendly name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Port";
            // 
            // _lIPAddress1
            // 
            this._lIPAddress1.AutoSize = true;
            this._lIPAddress1.Location = new System.Drawing.Point(6, 13);
            this._lIPAddress1.Name = "_lIPAddress1";
            this._lIPAddress1.Size = new System.Drawing.Size(57, 13);
            this._lIPAddress1.TabIndex = 15;
            this._lIPAddress1.Text = "IP address";
            // 
            // _bDelete1
            // 
            this._bDelete1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDelete1.Location = new System.Drawing.Point(534, 247);
            this._bDelete1.Name = "_bDelete1";
            this._bDelete1.Size = new System.Drawing.Size(75, 23);
            this._bDelete1.TabIndex = 2;
            this._bDelete1.Text = "Delete";
            this._bDelete1.UseVisualStyleBackColor = true;
            // 
            // _eSNTPDNSHostName
            // 
            this._eSNTPDNSHostName.Location = new System.Drawing.Point(9, 68);
            this._eSNTPDNSHostName.Name = "_eSNTPDNSHostName";
            this._eSNTPDNSHostName.Size = new System.Drawing.Size(216, 20);
            this._eSNTPDNSHostName.TabIndex = 8;
            // 
            // _lSNTPDNSHostNames
            // 
            this._lSNTPDNSHostNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._lSNTPDNSHostNames.FormattingEnabled = true;
            this._lSNTPDNSHostNames.Location = new System.Drawing.Point(312, 55);
            this._lSNTPDNSHostNames.Name = "_lSNTPDNSHostNames";
            this._lSNTPDNSHostNames.Size = new System.Drawing.Size(297, 186);
            this._lSNTPDNSHostNames.TabIndex = 10;
            // 
            // _eSNTPIpAddress
            // 
            this._eSNTPIpAddress.Location = new System.Drawing.Point(9, 29);
            this._eSNTPIpAddress.Name = "_eSNTPIpAddress";
            this._eSNTPIpAddress.Size = new System.Drawing.Size(216, 20);
            this._eSNTPIpAddress.TabIndex = 5;
            // 
            // _lDNSHostname
            // 
            this._lDNSHostname.AutoSize = true;
            this._lDNSHostname.Location = new System.Drawing.Point(6, 52);
            this._lDNSHostname.Name = "_lDNSHostname";
            this._lDNSHostname.Size = new System.Drawing.Size(79, 13);
            this._lDNSHostname.TabIndex = 1;
            this._lDNSHostname.Text = "DNS hostname";
            // 
            // _lSNTPIPAddresses
            // 
            this._lSNTPIPAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._lSNTPIPAddresses.FormattingEnabled = true;
            this._lSNTPIPAddresses.Location = new System.Drawing.Point(9, 94);
            this._lSNTPIPAddresses.Name = "_lSNTPIPAddresses";
            this._lSNTPIPAddresses.Size = new System.Drawing.Size(297, 147);
            this._lSNTPIPAddresses.TabIndex = 9;
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDelete.Location = new System.Drawing.Point(231, 247);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(75, 23);
            this._bDelete.TabIndex = 0;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            // 
            // _ofdBrowseImage
            // 
            this._ofdBrowseImage.Filter = "Images|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            // 
            // GeneralOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(969, 633);
            this.Controls.Add(this._tcGeneralOptions);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GeneralOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "General options";
            this.Load += new System.EventHandler(this.GeneralOptionsForm_Load);
            this._tcGeneralOptions.ResumeLayout(false);
            this._tpLicence.ResumeLayout(false);
            this._tpLicence.PerformLayout();
            this._pBack.ResumeLayout(false);
            this._tpServerControl.ResumeLayout(false);
            this._tpServerControl.PerformLayout();
            this._gbServerInformations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgServerInformations)).EndInit();
            this._gbResetUserSession.ResumeLayout(false);
            this._gbResetUserSession.PerformLayout();
            this._tpRemoteServicesSettings.ResumeLayout(false);
            this._gbNtpSettings.ResumeLayout(false);
            this._gbNtpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudTimeDiffTolerance)).EndInit();
            this._gbSmtpSettings.ResumeLayout(false);
            this._tpDHCPServer.ResumeLayout(false);
            this._tpDHCPServer.PerformLayout();
            this._gbIPGroup.ResumeLayout(false);
            this._gbIPGroup.PerformLayout();
            this._gbNetwork.ResumeLayout(false);
            this._gbNetwork.PerformLayout();
            this._gbRange.ResumeLayout(false);
            this._gbRange.PerformLayout();
            this._gbDNS.ResumeLayout(false);
            this._gbDNS.PerformLayout();
            this._tpSerialPort.ResumeLayout(false);
            this._tpSerialPort.PerformLayout();
            this._tpDatabaseOptions.ResumeLayout(false);
            this._gbEventLogExpiration.ResumeLayout(false);
            this._gbEventLogExpiration.PerformLayout();
            this._gbTimeZone1.ResumeLayout(false);
            this._gbTimeZone1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tbMaxEventlogSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eMaxEventlogRecords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eCountDaysExpiration)).EndInit();
            this._gbDatabaseBackup.ResumeLayout(false);
            this._gbBackupPath.ResumeLayout(false);
            this._gbBackupPath.PerformLayout();
            this._gbTimeZone.ResumeLayout(false);
            this._gbTimeZone.PerformLayout();
            this._tpSecuritySettings.ResumeLayout(false);
            this._tpAlarmSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvAlarmSettings.DataGrid)).EndInit();
            this._tpUiSettings.ResumeLayout(false);
            this._gbAutoclose.ResumeLayout(false);
            this._gbAutoclose.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nUpDoAutoCloseTimeout)).EndInit();
            this._gbColorSettings.ResumeLayout(false);
            this._gbAlarmNotAcknowledged.ResumeLayout(false);
            this._gbAlarmNotAcknowledged.PerformLayout();
            this._gbAlarmColour.ResumeLayout(false);
            this._gbAlarmColour.PerformLayout();
            this._gbNormalNotAcknowledgedColour.ResumeLayout(false);
            this._gbNormalNotAcknowledgedColour.PerformLayout();
            this._gbReferenceObjectsColour.ResumeLayout(false);
            this._gbReferenceObjectsColour.PerformLayout();
            this._gbNormalColour.ResumeLayout(false);
            this._gbNormalColour.PerformLayout();
            this._gbDropDownColour.ResumeLayout(false);
            this._gbDropDownColour.PerformLayout();
            this._gbNoAlarmsInQueueColour.ResumeLayout(false);
            this._gbNoAlarmsInQueueColour.PerformLayout();
            this._tpEventlogs.ResumeLayout(false);
            this._tpEventlogs.PerformLayout();
            this._gbEventlogReports.ResumeLayout(false);
            this._gbEventlogReports.PerformLayout();
            this._tpLanguage.ResumeLayout(false);
            this._tpAdvancedAccessSettings.ResumeLayout(false);
            this._tpAdvancedAccessSettings.PerformLayout();
            this._gbSyncingOfTimeFromServerSettings.ResumeLayout(false);
            this._gbSyncingOfTimeFromServerSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._ePeriodicTimeSyncTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._ePeriodOfTimeSyncWithoutStratum)).EndInit();
            this._tpAdvancedSettings.ResumeLayout(false);
            this._tpAdvancedSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayForSendingChangesToCcu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eAlarmListSuspendedRefreshTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eClientSessionTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eDelayForSaveEvents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eMaxEventsCountForInsert)).EndInit();
            this._tpCustomerSupplierInfo.ResumeLayout(false);
            this._tpCustomerSupplierInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbSupplierLogo)).EndInit();
            this._gbSupplierInfo.ResumeLayout(false);
            this._gbCustomerInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }        

        #endregion

        private System.Windows.Forms.TabControl _tcGeneralOptions;
        private System.Windows.Forms.TabPage _tpRemoteServicesSettings;
        private System.Windows.Forms.TabPage _tpSerialPort;
        private System.Windows.Forms.TextBox _ePortPin;
        private System.Windows.Forms.Label _lPin;
        private System.Windows.Forms.CheckBox _chbPortCarrierDetect;
        private System.Windows.Forms.CheckBox _chbPortParityCheck;
        private System.Windows.Forms.Label _PortNumber;
        private System.Windows.Forms.ComboBox _cbPortFlowControl;
        private System.Windows.Forms.Label _lFlowControl;
        private System.Windows.Forms.ComboBox _cbPort;
        private System.Windows.Forms.Label _lBaud;
        private System.Windows.Forms.ComboBox _cbPortStopBits;
        private System.Windows.Forms.ComboBox _cbPortBaudRate;
        private System.Windows.Forms.ComboBox _cbPortDataBits;
        private System.Windows.Forms.Label _lStopBits;
        private System.Windows.Forms.Label _lBits;
        private System.Windows.Forms.Label _lParity;
        private System.Windows.Forms.ComboBox _cbPortParity;
        private System.Windows.Forms.Button _bSaveSmtp;
        private System.Windows.Forms.Button _bSaveSerialPort;
        private System.Windows.Forms.TabPage _tpServerControl;
        private System.Windows.Forms.Label _lStopServer;
        private System.Windows.Forms.Button _bStop;
        private System.Windows.Forms.TabPage _tpLicence;
        private System.Windows.Forms.Label _lExpirationDateText;
        private System.Windows.Forms.Label _lLicenceFileText;
        private System.Windows.Forms.Label _lFunctionalPropertiesText;
        private System.Windows.Forms.TabPage _tpDHCPServer;
        private System.Windows.Forms.Label _lIPRangeTo;
        private System.Windows.Forms.Label _lIPRangeFrom;
        private System.Windows.Forms.TextBox _tbIPRangeFrom;
        private System.Windows.Forms.TextBox _tbIPRangeTo;
        private System.Windows.Forms.TextBox _tbSubnet;
        private System.Windows.Forms.Label _lSubnet;
        private System.Windows.Forms.GroupBox _gbRange;
        private System.Windows.Forms.GroupBox _gbNetwork;
        private System.Windows.Forms.TextBox _tbNetworkMask;
        private System.Windows.Forms.Label _lSubnetMask;
        private System.Windows.Forms.TextBox _tbDNS;
        private System.Windows.Forms.GroupBox _gbDNS;
        private System.Windows.Forms.Label _lAlternateDNSServer;
        private System.Windows.Forms.Label _lPrefferedDNSServer;
        private System.Windows.Forms.TextBox _tbAlternateDNS;
        private System.Windows.Forms.Label _lGateway;
        private System.Windows.Forms.TextBox _tbGateway;
        private System.Windows.Forms.ComboBox _cbMACMask;
        private System.Windows.Forms.ComboBox _cbIPGroup;
        private System.Windows.Forms.Label _lIPGroup;
        private System.Windows.Forms.Label _lMACMask;
        private System.Windows.Forms.GroupBox _gbIPGroup;
        private System.Windows.Forms.Button _bDHCPSave;
        private System.Windows.Forms.CheckBox _chbEnable;
        private System.Windows.Forms.TextBox _tbMaxLeaseTime;
        private System.Windows.Forms.TextBox _tbLeaseTime;
        private System.Windows.Forms.Label _lMaxLeaseTime;
        private System.Windows.Forms.Label _lLeaseTime;
        private System.Windows.Forms.Button _bMaxRange;
        private System.Windows.Forms.Label _lDNSSuffix;
        private System.Windows.Forms.TextBox _tbDnsSuffix;
        private System.Windows.Forms.Button _bDeleteIpGroup;
        private System.Windows.Forms.CheckBox _chbAutoStartDHCP;
        private System.Windows.Forms.Button _bStartDHCP;
        private System.Windows.Forms.Button _bStopDHCP;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button _buttonLicenceRequest;
        private System.Windows.Forms.TabPage _tpDatabaseOptions;
        private System.Windows.Forms.GroupBox _gbBackupPath;
        private System.Windows.Forms.GroupBox _gbTimeZone;
        private System.Windows.Forms.Button _bSaveDatabaseBackup;
        private System.Windows.Forms.Button _bForceDBSBackup;
        private System.Windows.Forms.FolderBrowserDialog _fbDialog;
        private System.Windows.Forms.Label _lInfoDatabaseBackup;
        private System.Windows.Forms.Button _buttonSetMacGroup;
        private System.Windows.Forms.TabPage _tpUiSettings;
        private System.Windows.Forms.GroupBox _gbNormalColour;
        private System.Windows.Forms.GroupBox _gbNormalNotAcknowledgedColour;
        private System.Windows.Forms.GroupBox _gbAlarmColour;
        private System.Windows.Forms.GroupBox _gbAlarmNotAcknowledged;
        private System.Windows.Forms.Label _lColourText;
        private System.Windows.Forms.GroupBox _gbReferenceObjectsColour;
        private System.Windows.Forms.GroupBox _gbDropDownColour;
        private System.Windows.Forms.GroupBox _gbNoAlarmsInQueueColour;
        private System.Windows.Forms.TextBox _eAlarmNotAcknowledgedPreview;
        private System.Windows.Forms.Label _lColourBackground;
        private System.Windows.Forms.TextBox _eReferenceObjectsPreview;
        private System.Windows.Forms.Label _lColourBackground6;
        private System.Windows.Forms.Label _lColourText6;
        private System.Windows.Forms.Label _lColourBackground5;
        private System.Windows.Forms.Label _lColourText5;
        private System.Windows.Forms.TextBox _eNoAlarmQueuePreview;
        private System.Windows.Forms.Label _lColourBackground4;
        private System.Windows.Forms.Label _lColourText4;
        private System.Windows.Forms.TextBox _eNormalPreview;
        private System.Windows.Forms.Label _lColourBackground3;
        private System.Windows.Forms.Label _lColourText3;
        private System.Windows.Forms.TextBox _eNormalNotAcknowledgedPreview;
        private System.Windows.Forms.Label _lColourBackground2;
        private System.Windows.Forms.Label _lColourText2;
        private System.Windows.Forms.TextBox _eAlarmPreview;
        private System.Windows.Forms.Label _lColourBackground1;
        private System.Windows.Forms.Label _lColourText1;
        private System.Windows.Forms.TextBox _eDropDownPreview;
        private System.Windows.Forms.Button _bDragDropColorBackground;
        private System.Windows.Forms.Button _bDragDropColorText;
        private System.Windows.Forms.ColorDialog _colourDialog;
        private System.Windows.Forms.Button _bSave;
        private System.Windows.Forms.Button _bReferenceObjectColorText;
        private System.Windows.Forms.Button _bReferenceObjectColorBackground;
        private System.Windows.Forms.Button _bNoAlarmsInQueueColorBackground;
        private System.Windows.Forms.Button _bNoAlarmsInQueueColorText;
        private System.Windows.Forms.Button _bNormalColorBackground;
        private System.Windows.Forms.Button _bNormalColorText;
        private System.Windows.Forms.Button _bNormalNotAcknowledgedColorBackground;
        private System.Windows.Forms.Button _bNormalNotAcknowledgedColorText;
        private System.Windows.Forms.Button _bAlarmColorBackground;
        private System.Windows.Forms.Button _bAlarmColorText;
        private System.Windows.Forms.Button _bAlarmNotAcknowledgedColorBackground;
        private System.Windows.Forms.Button _bAlarmNotAcknowledgedColorText;
        private System.Windows.Forms.ProgressBar _pgForceDbsBackup;
        private System.Windows.Forms.NumericUpDown _nUpDoAutoCloseTimeout;
        private System.Windows.Forms.GroupBox _gbColorSettings;
        private System.Windows.Forms.GroupBox _gbAutoclose;
        private System.Windows.Forms.Label _labelTimeout;
        private System.Windows.Forms.CheckBox _cbAutocCoseTurnedOn;
        private System.Windows.Forms.Label _lAutocloseRange;
        private System.Windows.Forms.Button _buttonSave;
        private Contal.IwQuick.UI.TextBoxMenu _tbmTimeZone;
        private Contal.IwQuick.UI.ExtendedPropertyGrid _epgSMTPServerSettings;
        private Contal.IwQuick.UI.ExtendedPropertyGrid _epgLicenceInfo;
        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.GroupBox _gbSmtpSettings;
        private System.Windows.Forms.GroupBox _gbNtpSettings;
        private System.Windows.Forms.Button _bResolve;
        private System.Windows.Forms.Label _lIpAddress;
        private System.Windows.Forms.TextBox _eDnsHostName;
        private System.Windows.Forms.TextBox _eNtpIpAddress;
        private System.Windows.Forms.Label _lDnsHostname1;
        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.ListBox _lNtpIpAddresses;
        private System.Windows.Forms.Button _bDelete2;
        private System.Windows.Forms.Label _lIPAddress1;
        private System.Windows.Forms.Button _bDelete1;
        private System.Windows.Forms.TextBox _eSNTPDNSHostName;
        private System.Windows.Forms.ListBox _lSNTPDNSHostNames;
        private System.Windows.Forms.TextBox _eSNTPIpAddress;
        private System.Windows.Forms.Label _lDNSHostname;
        private System.Windows.Forms.ListBox _lSNTPIPAddresses;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.GroupBox _gbEventLogExpiration;
        private System.Windows.Forms.GroupBox _gbDatabaseBackup;
        private System.Windows.Forms.NumericUpDown _eMaxEventlogRecords;
        private System.Windows.Forms.Label _lDaysExpiration;
        private System.Windows.Forms.NumericUpDown _eCountDaysExpiration;
        private System.Windows.Forms.TrackBar _tbMaxEventlogSlider;
        private System.Windows.Forms.Label _lMaxEventlogsRecords;
        private System.Windows.Forms.TextBox _eResultMaxEventlogRecords;
        private System.Windows.Forms.GroupBox _gbTimeZone1;
        private System.Windows.Forms.Button _bForceEvenlogExpirationClean;
        private System.Windows.Forms.Label _lInfoEventlogExpirationTimeZone;
        private System.Windows.Forms.Button _bSaveEventlogExpiration;
        private System.Windows.Forms.Label _lCalculatedValue;
        private System.Windows.Forms.Label _lExponent;
        private Contal.IwQuick.UI.TextBoxMenu _tbmEventlogTimeZone;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove1;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate1;
        private System.Windows.Forms.Button _bDBTest;
        private System.Windows.Forms.TabPage _tpEventlogs;
        private System.Windows.Forms.CheckBox _cbEventlogAlarmAreaAlarmStateChanged;
        private System.Windows.Forms.CheckBox _cbEventlogOutputStateChanged;
        private System.Windows.Forms.CheckBox _cbEventlogInputStateChanged;
        private System.Windows.Forms.CheckBox _cbEventlogCardReaderOnlineStateChanged;
        private System.Windows.Forms.CheckBox _cbEventlogAlarmAreaActivationStateChanged;
        private System.Windows.Forms.Button _bEventlogsSave;
        private System.Windows.Forms.Button _bDbsBackupPath;
        private System.Windows.Forms.TextBox _eDbsBackupPath;
        private System.Windows.Forms.Button _bClearAndSaveDbsBackup;
        private System.Windows.Forms.TabPage _tpLanguage;
        private System.Windows.Forms.Button _bSave2;
        private System.Windows.Forms.Label _lTimeDiffTolerance;
        private System.Windows.Forms.NumericUpDown _nudTimeDiffTolerance;
        private System.Windows.Forms.TabPage _tpAdvancedAccessSettings;
        private System.Windows.Forms.Button _bSave3;
        private System.Windows.Forms.TabPage _tpAdvancedSettings;
        private System.Windows.Forms.Label _lDelayForSaveEvents;
        private System.Windows.Forms.NumericUpDown _eDelayForSaveEvents;
        private System.Windows.Forms.Label _lMaxEventsCountForInsert;
        private System.Windows.Forms.NumericUpDown _eMaxEventsCountForInsert;
        private System.Windows.Forms.Button _bSave4;
        private System.Windows.Forms.TabPage _tpAlarmSettings;
        private System.Windows.Forms.Button _bSave5;
        private System.Windows.Forms.TabPage _tpSecuritySettings;
        private Contal.IwQuick.UI.ExtendedPropertyGrid _epgSecuritySettings;
        private System.Windows.Forms.Button _buttonSaveSecuritySettings;
        private System.Windows.Forms.GroupBox _gbSyncingOfTimeFromServerSettings;
        private System.Windows.Forms.CheckBox _cbSyncingTimeFromServer;
        private System.Windows.Forms.NumericUpDown _ePeriodOfTimeSyncWithoutStratum;
        private System.Windows.Forms.Label _lPeriodOfTimeSyncWithoutStratum;
        private System.Windows.Forms.NumericUpDown _ePeriodicTimeSyncTolerance;
        private System.Windows.Forms.Label _lPeriodicTimeSyncTolerance;
        private System.Windows.Forms.GroupBox _gbResetUserSession;
        private System.Windows.Forms.Button _bResetUserSession;
        private System.Windows.Forms.Label _lLogin;
        private Contal.IwQuick.UI.TextBoxMenu _tbmLogin;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
        private System.Windows.Forms.DataGridView _dgServerInformations;
        private System.Windows.Forms.GroupBox _gbServerInformations;
        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.Label _lClientSessionTimeout;
        private System.Windows.Forms.NumericUpDown _eClientSessionTimeout;
        private System.Windows.Forms.CheckBox _cbEventSourcesReverseOrder;
        private Contal.Cgp.Components.CgpDataGridView _cdgvAlarmSettings;
        private System.Windows.Forms.CheckBox _chbEnableLoggingSDPSTZChanges;
        private System.Windows.Forms.CheckBox _cbCorrectDeserializationFailures;
        private System.Windows.Forms.TabPage _tpCustomerSupplierInfo;
        private System.Windows.Forms.Button _bSave6;
        private Contal.IwQuick.UI.ExtendedPropertyGrid _epgCustomerInfo;
        private System.Windows.Forms.GroupBox _gbCustomerInfo;
        private Contal.IwQuick.UI.ExtendedPropertyGrid _epgSupplierInfo;
        private System.Windows.Forms.GroupBox _gbSupplierInfo;
        private System.Windows.Forms.Label _lSupplierLogo;
        private System.Windows.Forms.PictureBox _pbSupplierLogo;
        private System.Windows.Forms.Button _bRemove;
        private System.Windows.Forms.Button _bAdd8;
        private System.Windows.Forms.OpenFileDialog _ofdBrowseImage;
        private System.Windows.Forms.Label _lAlarmListSuspendedRefreshTimeout;
        private System.Windows.Forms.NumericUpDown _eAlarmListSuspendedRefreshTimeout;
        private System.Windows.Forms.Button _bInsertLicenceFile;
        private System.Windows.Forms.Label _lDelayForSendingChangesToCcu;
        private System.Windows.Forms.NumericUpDown _eDelayForSendingChangesToCcu;
        private System.Windows.Forms.CheckBox _chbAlarmAreaRestrictivePolicyForTimeBuying;
        private System.Windows.Forms.GroupBox _gbEventlogReports;
        private IwQuick.UI.TextBoxMenu _tbmEventlogsReportsTimezone;
        private System.Windows.Forms.Label _lblReportsEmails;
        private System.Windows.Forms.Label _lblReportsTimezone;
        private System.Windows.Forms.TextBox _eEventlogsReportsEmails;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify3;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove3;
    }
}