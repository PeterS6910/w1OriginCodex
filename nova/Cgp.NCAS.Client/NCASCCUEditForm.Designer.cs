namespace Contal.Cgp.NCAS.Client
{
    partial class NCASCCUEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASCCUEditForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._tcCCU = new System.Windows.Forms.TabControl();
            this._tpInformation = new System.Windows.Forms.TabPage();
            this._bPrecreateCardReader = new System.Windows.Forms.Button();
            this._nudOutputCount = new System.Windows.Forms.NumericUpDown();
            this._lOutputCount = new System.Windows.Forms.Label();
            this._nudInputCount = new System.Windows.Forms.NumericUpDown();
            this._lInputCount = new System.Windows.Forms.Label();
            this._eMaximumExpectedDCUCount = new System.Windows.Forms.NumericUpDown();
            this._lMaximumExpectedDCUCount = new System.Windows.Forms.Label();
            this._bPrecreateDCU = new System.Windows.Forms.Button();
            this._bDcuMarkAsDead = new System.Windows.Forms.Button();
            this._bCrMarkAsDead = new System.Windows.Forms.Button();
            this._dgDCUs = new System.Windows.Forms.DataGridView();
            this._dgCardReaders = new System.Windows.Forms.DataGridView();
            this._bTest = new System.Windows.Forms.Button();
            this._tpTimeSettings = new System.Windows.Forms.TabPage();
            this._gbTimeZone = new System.Windows.Forms.GroupBox();
            this._cbTimeZone = new System.Windows.Forms.ComboBox();
            this._lTimeZone = new System.Windows.Forms.Label();
            this._lTimeShift = new System.Windows.Forms.Label();
            this._lHours = new System.Windows.Forms.Label();
            this._lMinutes = new System.Windows.Forms.Label();
            this._eHours = new System.Windows.Forms.NumericUpDown();
            this._eMinutes = new System.Windows.Forms.NumericUpDown();
            this._lDeltaValue = new System.Windows.Forms.Label();
            this._lDelta = new System.Windows.Forms.Label();
            this._cbSyncingTimeFromServer = new System.Windows.Forms.CheckBox();
            this._gbSetTimeManually = new System.Windows.Forms.GroupBox();
            this._bSet2 = new System.Windows.Forms.Button();
            this._tbdpSetTimeManually = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._bTimeRefresh = new System.Windows.Forms.Button();
            this._eTime = new System.Windows.Forms.Label();
            this._lTime = new System.Windows.Forms.Label();
            this._gbNtpSettings = new System.Windows.Forms.GroupBox();
            this._chbInheritedGeneralNtpSettings = new System.Windows.Forms.CheckBox();
            this._bResolve = new System.Windows.Forms.Button();
            this._lIPAddress1 = new System.Windows.Forms.Label();
            this._eNtpDnsHostName = new System.Windows.Forms.TextBox();
            this._eSNTPIpAddress = new System.Windows.Forms.TextBox();
            this._lDNSHostname = new System.Windows.Forms.Label();
            this._bAdd = new System.Windows.Forms.Button();
            this._lSNTPIPAddresses = new System.Windows.Forms.ListBox();
            this._bDelete = new System.Windows.Forms.Button();
            this._tpControl = new System.Windows.Forms.TabPage();
            this._gbLogging = new System.Windows.Forms.GroupBox();
            this._prbDebugFilesTransfer = new Contal.IwQuick.PlatformPC.UI.ProgressBarWithText();
            this._bMakeLogDump = new System.Windows.Forms.Button();
            this._bSet1 = new System.Windows.Forms.Button();
            this._lVerbosityLevel = new System.Windows.Forms.Label();
            this._nudVerbosityLevel = new System.Windows.Forms.NumericUpDown();
            this._cbVerbosityLevel = new System.Windows.Forms.ComboBox();
            this._gbCEUpgrade = new System.Windows.Forms.GroupBox();
            this._ceUpgrade = new System.Windows.Forms.Integration.ElementHost();
            this._ceUpgradeFiles = new Contal.Cgp.Components.WpfUpgradeFilesViewer();
            this._prbTransferCe = new Contal.IwQuick.PlatformPC.UI.ProgressBarWithText();
            this._lImageVersion = new System.Windows.Forms.Label();
            this._eImageVersion = new System.Windows.Forms.TextBox();
            this._bRefresh5 = new System.Windows.Forms.Button();
            this._eCEUpgradeProgress = new System.Windows.Forms.TextBox();
            this._lCEUpgradeProgress = new System.Windows.Forms.Label();
            this._bStopTransferCEFile = new System.Windows.Forms.Button();
            this._lTransferPercents = new System.Windows.Forms.Label();
            this._lAvailableCEFiles = new System.Windows.Forms.Label();
            this._gbDeviceUpgrade = new System.Windows.Forms.GroupBox();
            this._ccuUpgrade = new System.Windows.Forms.Integration.ElementHost();
            this._ccuUpgradeFiles = new Contal.Cgp.Components.WpfUpgradeFilesViewer();
            this._prbUnpack = new Contal.IwQuick.PlatformPC.UI.ProgressBarWithText();
            this._prbTransfer = new Contal.IwQuick.PlatformPC.UI.ProgressBarWithText();
            this._eUpgradeFinalisation = new System.Windows.Forms.TextBox();
            this._lUpgradeFinalisation = new System.Windows.Forms.Label();
            this._lFirmwareVersion = new System.Windows.Forms.Label();
            this._bRefresh = new System.Windows.Forms.Button();
            this._eFirmware = new System.Windows.Forms.TextBox();
            this._lAvailableVersions = new System.Windows.Forms.Label();
            this._lTransferProgress = new System.Windows.Forms.Label();
            this._lUpgradeProgress = new System.Windows.Forms.Label();
            this._bStopTransfer = new System.Windows.Forms.Button();
            this._gbConfiguration = new System.Windows.Forms.GroupBox();
            this._bDeleteEvents = new System.Windows.Forms.Button();
            this._bCancelConfigurationPassword = new System.Windows.Forms.Button();
            this._bChangeConfigurePassword = new System.Windows.Forms.Button();
            this._bForceReconfiguration = new System.Windows.Forms.Button();
            this._lResultOfAction = new System.Windows.Forms.Label();
            this._eResultOfAction = new System.Windows.Forms.TextBox();
            this._bConfigureForThisServer = new System.Windows.Forms.Button();
            this._bUnconfigure = new System.Windows.Forms.Button();
            this._gbDevice = new System.Windows.Forms.GroupBox();
            this._bMemoryCollect = new System.Windows.Forms.Button();
            this._bSoftReset = new System.Windows.Forms.Button();
            this._bHardReset = new System.Windows.Forms.Button();
            this._tpInputOutput = new System.Windows.Forms.TabPage();
            this._scInputsOutputs = new System.Windows.Forms.SplitContainer();
            this._gbInputsCCU = new System.Windows.Forms.GroupBox();
            this._bPrecreateInput = new System.Windows.Forms.Button();
            this._dgvInputCCU = new System.Windows.Forms.DataGridView();
            this._gbOutputsCCU = new System.Windows.Forms.GroupBox();
            this._bPrecreateOutput = new System.Windows.Forms.Button();
            this._dgvOutputCCU = new System.Windows.Forms.DataGridView();
            this._tpLevelBSI = new System.Windows.Forms.TabPage();
            this._gbScheme = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this._bRecalculate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._lRefU = new System.Windows.Forms.Label();
            this._tbUref = new System.Windows.Forms.TextBox();
            this._lTamperStateNormallyClosed = new System.Windows.Forms.Label();
            this._lk2 = new System.Windows.Forms.Label();
            this._lR2 = new System.Windows.Forms.Label();
            this._lk = new System.Windows.Forms.Label();
            this._lR1 = new System.Windows.Forms.Label();
            this._tbR2 = new System.Windows.Forms.TextBox();
            this._tbR1 = new System.Windows.Forms.TextBox();
            this._lAlarmStateNormallyClosed = new System.Windows.Forms.Label();
            this._gbBsiLevelInfo = new System.Windows.Forms.GroupBox();
            this._lValueBreak = new System.Windows.Forms.Label();
            this._lValueAlarm = new System.Windows.Forms.Label();
            this._lInfoShort = new System.Windows.Forms.Label();
            this._bSet = new System.Windows.Forms.Button();
            this._lInfoNormal = new System.Windows.Forms.Label();
            this._lValueNormal = new System.Windows.Forms.Label();
            this._lValueShort = new System.Windows.Forms.Label();
            this._cbTemplateBsiLevel = new System.Windows.Forms.ComboBox();
            this._lInfoBreak = new System.Windows.Forms.Label();
            this._lInfoAlarm = new System.Windows.Forms.Label();
            this._gbBsiLevelSet = new System.Windows.Forms.GroupBox();
            this._nBreak = new System.Windows.Forms.NumericUpDown();
            this._lBreak = new System.Windows.Forms.Label();
            this._nZeno = new System.Windows.Forms.NumericUpDown();
            this._lAlarm = new System.Windows.Forms.Label();
            this._lShort = new System.Windows.Forms.Label();
            this._lNormal = new System.Windows.Forms.Label();
            this._nShortNormal = new System.Windows.Forms.NumericUpDown();
            this._nNormalAlarm = new System.Windows.Forms.NumericUpDown();
            this._nAlarmBreak = new System.Windows.Forms.NumericUpDown();
            this._tpDoorEnvironments = new System.Windows.Forms.TabPage();
            this._dgvDoorEnvironments = new System.Windows.Forms.DataGridView();
            this._tpDCUsUpgrade = new System.Windows.Forms.TabPage();
            this._dcuUpgrade = new System.Windows.Forms.Integration.ElementHost();
            this._dcuUpgradeFiles = new Contal.Cgp.Components.WpfUpgradeFilesViewer();
            this._chbSelectAll = new System.Windows.Forms.CheckBox();
            this._lDCUsToUpgrade = new System.Windows.Forms.Label();
            this._lAvailableDCUUpgrades = new System.Windows.Forms.Label();
            this._bRefreshDCUs = new System.Windows.Forms.Button();
            this._dgvDCUUpgrading = new System.Windows.Forms.DataGridView();
            this._tpCRUpgrade = new System.Windows.Forms.TabPage();
            this._crUpgrade = new System.Windows.Forms.Integration.ElementHost();
            this._crUpgradeFiles = new Contal.Cgp.Components.WpfUpgradeFilesViewer();
            this._bUpgradeCRRefresh = new System.Windows.Forms.Button();
            this._chbCRSelectAll = new System.Windows.Forms.CheckBox();
            this._dgvCRUpgrading = new System.Windows.Forms.DataGridView();
            this._lAvailableCRUpradeVersions = new System.Windows.Forms.Label();
            this._tpPortSettings = new System.Windows.Forms.TabPage();
            this._gbComPortSettings = new System.Windows.Forms.GroupBox();
            this._chbEnabledComPort = new System.Windows.Forms.CheckBox();
            this._cbPortComBaudRate = new System.Windows.Forms.ComboBox();
            this._lComPortBaudRate = new System.Windows.Forms.Label();
            this._tpIPSettings = new System.Windows.Forms.TabPage();
            this._gbStaticIPSettings = new System.Windows.Forms.GroupBox();
            this._bTestIP = new System.Windows.Forms.Button();
            this._lResultOfApplyIPSettings = new System.Windows.Forms.Label();
            this._eResultOfApplyIPSettings = new System.Windows.Forms.TextBox();
            this._bApply1 = new System.Windows.Forms.Button();
            this._eIPSettingsGateway = new System.Windows.Forms.TextBox();
            this._lGateway = new System.Windows.Forms.Label();
            this._eIPSettingsMask = new System.Windows.Forms.TextBox();
            this._lMask = new System.Windows.Forms.Label();
            this._eIPSettingsIPAddress = new System.Windows.Forms.TextBox();
            this._lIPAddress2 = new System.Windows.Forms.Label();
            this._rbStatic = new System.Windows.Forms.RadioButton();
            this._rbDHCP = new System.Windows.Forms.RadioButton();
            this._tpAlarmSettings = new System.Windows.Forms.TabPage();
            this._lAlarmTransmitter = new System.Windows.Forms.Label();
            this._tbmAlarmTransmitter = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify24 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove24 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate24 = new System.Windows.Forms.ToolStripMenuItem();
            this._accordionAlarmSettings = new Contal.IwQuick.PlatformPC.UI.Accordion.Accordion();
            this._cbOpenAllAlarmSettings = new System.Windows.Forms.CheckBox();
            this._gbSettingsForEventlogOnCR = new System.Windows.Forms.GroupBox();
            this._tbLastEventTimeForMarkAlarmArea = new System.Windows.Forms.TextBox();
            this._lCRLastEventTimeForMarkAlarmArea = new System.Windows.Forms.Label();
            this._tbCrEvetlogLimitedSizeValue = new System.Windows.Forms.TextBox();
            this._lCrEventlogLimitedSize = new System.Windows.Forms.Label();
            this._gbServerWatchdog = new System.Windows.Forms.GroupBox();
            this._cmsoCcuOffline = new Contal.Cgp.NCAS.Client.ControlModifySpecialOutput();
            this._tpUpsMonitor = new System.Windows.Forms.TabPage();
            this._gpMaintenance = new System.Windows.Forms.GroupBox();
            this._lUpsOnlineState = new System.Windows.Forms.Label();
            this._lOnlineState = new System.Windows.Forms.Label();
            this.m_labSuccessRate = new System.Windows.Forms.Label();
            this._lSuccessRate = new System.Windows.Forms.Label();
            this.m_labResets = new System.Windows.Forms.Label();
            this._lResets = new System.Windows.Forms.Label();
            this.m_labMode = new System.Windows.Forms.Label();
            this._lMode = new System.Windows.Forms.Label();
            this._gpAlarms = new System.Windows.Forms.GroupBox();
            this._lBatteryFuse = new System.Windows.Forms.Label();
            this._lTamper = new System.Windows.Forms.Label();
            this._lOvertemperature = new System.Windows.Forms.Label();
            this._lBatteryEmpty = new System.Windows.Forms.Label();
            this._lBatteryFault = new System.Windows.Forms.Label();
            this._lPrimaryPowerMissing = new System.Windows.Forms.Label();
            this._lOutputOutOfTollerance = new System.Windows.Forms.Label();
            this._lOutputFuse = new System.Windows.Forms.Label();
            this._gpValues = new System.Windows.Forms.GroupBox();
            this.m_pgEstimatedBatteryCapacity = new System.Windows.Forms.ProgressBar();
            this.m_labTemperature = new System.Windows.Forms.Label();
            this.m_labEstimatedBatteryCapacity = new System.Windows.Forms.Label();
            this.m_labCurrentLoad = new System.Windows.Forms.Label();
            this.m_labCurrentBattery = new System.Windows.Forms.Label();
            this.m_labVoltageBattery = new System.Windows.Forms.Label();
            this.m_labVoltageOutput = new System.Windows.Forms.Label();
            this.m_labVoltageInput = new System.Windows.Forms.Label();
            this._lTemperature = new System.Windows.Forms.Label();
            this._lEstimatedBatterycapacity = new System.Windows.Forms.Label();
            this._lLoadCurrent = new System.Windows.Forms.Label();
            this._lBatteryCurrent = new System.Windows.Forms.Label();
            this._lBatteryVoltage = new System.Windows.Forms.Label();
            this._lOutputVoltage = new System.Windows.Forms.Label();
            this._lInputVoltage = new System.Windows.Forms.Label();
            this._tpStatistics = new System.Windows.Forms.TabPage();
            this._tcStatistics = new System.Windows.Forms.TabControl();
            this._tpCommunicationAndMemory = new System.Windows.Forms.TabPage();
            this._gbCommunicationStatistic = new System.Windows.Forms.GroupBox();
            this._tlpCOmmunicationStatistics = new System.Windows.Forms.TableLayoutPanel();
            this._lCCUNotAcnknowledgedAutonomousEvents = new System.Windows.Forms.Label();
            this._eCCUNotAcnknowledgedAutonomousEvents = new System.Windows.Forms.TextBox();
            this._eCcuSended = new System.Windows.Forms.TextBox();
            this._lCCUNotAcnknowledgedEvents = new System.Windows.Forms.Label();
            this._eCCUUnprocessedEvents = new System.Windows.Forms.TextBox();
            this._bReset1 = new System.Windows.Forms.Button();
            this._lServerSended = new System.Windows.Forms.Label();
            this._eServerSended = new System.Windows.Forms.TextBox();
            this._bCcuResetSended = new System.Windows.Forms.Button();
            this._bResetServerSended = new System.Windows.Forms.Button();
            this._lCcuSended = new System.Windows.Forms.Label();
            this._bServerResetReceived = new System.Windows.Forms.Button();
            this._lCcuMsgRetry = new System.Windows.Forms.Label();
            this._eCommandTimeouts = new System.Windows.Forms.TextBox();
            this._lCommandTimeoutCount = new System.Windows.Forms.Label();
            this._bCcuMsgRetry = new System.Windows.Forms.Button();
            this._eServerReceived = new System.Windows.Forms.TextBox();
            this._lServerReceived = new System.Windows.Forms.Label();
            this._eCcuMsgRetry = new System.Windows.Forms.TextBox();
            this._lCcuReceived = new System.Windows.Forms.Label();
            this._eCcuReceived = new System.Windows.Forms.TextBox();
            this._bCcuResetReceived = new System.Windows.Forms.Button();
            this._lCcuReceivedError = new System.Windows.Forms.Label();
            this._bCcuResetReceivedError = new System.Windows.Forms.Button();
            this._lServerDeserializeError = new System.Windows.Forms.Label();
            this._eCcuReceivedError = new System.Windows.Forms.TextBox();
            this._lCcuDeserializeError = new System.Windows.Forms.Label();
            this._bServerResetDeserializeError = new System.Windows.Forms.Button();
            this._bCcuResetDeserializeError = new System.Windows.Forms.Button();
            this._lServerMsgRetry = new System.Windows.Forms.Label();
            this._lServerReceivedError = new System.Windows.Forms.Label();
            this._eCcuDeserializeError = new System.Windows.Forms.TextBox();
            this._lServersNotStoredEvents = new System.Windows.Forms.Label();
            this._lServersNotProcessedEvents = new System.Windows.Forms.Label();
            this._eServerDeserializeError = new System.Windows.Forms.TextBox();
            this._eServerMsgRetry = new System.Windows.Forms.TextBox();
            this._eServerReceivedError = new System.Windows.Forms.TextBox();
            this._eServersNotStoredEvents = new System.Windows.Forms.TextBox();
            this._eServersNotProcessedEvents = new System.Windows.Forms.TextBox();
            this._bResetServerMsgRetry = new System.Windows.Forms.Button();
            this._bServerResetReceivedError = new System.Windows.Forms.Button();
            this._lCCUUnprocessedEvents = new System.Windows.Forms.Label();
            this._eCCUNotAcknowledgedEvents = new System.Windows.Forms.TextBox();
            this._bResetAll = new System.Windows.Forms.Button();
            this._bRefreshCommunicationStatistic = new System.Windows.Forms.Button();
            this._gbOtherStatistics = new System.Windows.Forms.GroupBox();
            this._tlpPerformanceStatistics = new System.Windows.Forms.TableLayoutPanel();
            this._eFreeTotalSDSpace = new System.Windows.Forms.TextBox();
            this._lFreeTotalSDSpace = new System.Windows.Forms.Label();
            this._lFreeMemory = new System.Windows.Forms.Label();
            this._lThreadCount = new System.Windows.Forms.Label();
            this._lTotalMemory = new System.Windows.Forms.Label();
            this._eTotalMemory = new System.Windows.Forms.TextBox();
            this._eThreads = new System.Windows.Forms.TextBox();
            this._eFreeMemory = new System.Windows.Forms.TextBox();
            this._lFreeTotalFlashSpace = new System.Windows.Forms.Label();
            this._eFreeTotalFlashSpace = new System.Windows.Forms.TextBox();
            this._eMemoryLoad = new System.Windows.Forms.TextBox();
            this._lMemoryLoad = new System.Windows.Forms.Label();
            this._bRefreshAll = new System.Windows.Forms.Button();
            this._tpThreadMap = new System.Windows.Forms.TabPage();
            this._tbThreadId = new System.Windows.Forms.TextBox();
            this._lSearchByThreadId = new System.Windows.Forms.Label();
            this._bRefresh3 = new System.Windows.Forms.Button();
            this._cdgvThreadMap = new Contal.Cgp.Components.CgpDataGridView();
            this._tpOtherStatistics = new System.Windows.Forms.TabPage();
            this._gbCcuStartsCount = new System.Windows.Forms.GroupBox();
            this._lCEUptime = new System.Windows.Forms.Label();
            this._eCEUptime = new System.Windows.Forms.TextBox();
            this._lCCUUptime = new System.Windows.Forms.Label();
            this._eCCUUptime = new System.Windows.Forms.TextBox();
            this._bCcuStartsCountReset = new System.Windows.Forms.Button();
            this._lCcuStartsCount = new System.Windows.Forms.Label();
            this._eCcuStartsCount = new System.Windows.Forms.TextBox();
            this._bCcuStartsCountRefresh = new System.Windows.Forms.Button();
            this._gbCoprocessorVersion = new System.Windows.Forms.GroupBox();
            this._bRefresh9 = new System.Windows.Forms.Button();
            this._eCoprocessorUpgradeResult = new System.Windows.Forms.TextBox();
            this._eCoprocessorActualBuildNumber = new System.Windows.Forms.TextBox();
            this._lUpgradeResult = new System.Windows.Forms.Label();
            this._lActualBuildNumber = new System.Windows.Forms.Label();
            this._tpMemory = new System.Windows.Forms.TabPage();
            this._cdgvMemory = new Contal.Cgp.Components.CgpDataGridView();
            this._bRefresh4 = new System.Windows.Forms.Button();
            this._tpProcesses = new System.Windows.Forms.TabPage();
            this._lCommandLine = new System.Windows.Forms.Label();
            this._bStartExplicityCKM = new System.Windows.Forms.Button();
            this._bStartImplicityCKM = new System.Windows.Forms.Button();
            this._bStopCKM = new System.Windows.Forms.Button();
            this._lbCmdResults = new System.Windows.Forms.ListBox();
            this._tbCommandLine = new System.Windows.Forms.TextBox();
            this._bRunCmd = new System.Windows.Forms.Button();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh1 = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._tpTesting = new System.Windows.Forms.TabPage();
            this._gbSimulationAARights = new System.Windows.Forms.GroupBox();
            this._lResultsAADB = new System.Windows.Forms.Label();
            this._rtbResultsDB = new System.Windows.Forms.RichTextBox();
            this._bTestAlarmAreaSimulation = new System.Windows.Forms.Button();
            this._lResultsCCU = new System.Windows.Forms.Label();
            this._tbmAlarmArea = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify4 = new System.Windows.Forms.ToolStripMenuItem();
            this._lPerson = new System.Windows.Forms.Label();
            this._lAlarmArea = new System.Windows.Forms.Label();
            this._tbmPerson = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify3 = new System.Windows.Forms.ToolStripMenuItem();
            this._rtbResultsCCU = new System.Windows.Forms.RichTextBox();
            this._gbTestRoutineDCU = new System.Windows.Forms.GroupBox();
            this._bRefreshTestDcu = new System.Windows.Forms.Button();
            this._bTestDcu = new System.Windows.Forms.Button();
            this._cbSelectAllToggleADC = new System.Windows.Forms.CheckBox();
            this._cbSelectAllToggleCard = new System.Windows.Forms.CheckBox();
            this._dgvTestDcu = new System.Windows.Forms.DataGridView();
            this._gbTimeZonesDailyPlansState = new System.Windows.Forms.GroupBox();
            this._bGetState = new System.Windows.Forms.Button();
            this._lStateOnServer = new System.Windows.Forms.Label();
            this._eStateOnServer = new System.Windows.Forms.TextBox();
            this._lStateOnCCU = new System.Windows.Forms.Label();
            this._eStateOnCCU = new System.Windows.Forms.TextBox();
            this._tbmTimeZonesDailyPlans = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._lTimeZoneDailyPlan = new System.Windows.Forms.Label();
            this._gbSimulationCardSwiped = new System.Windows.Forms.GroupBox();
            this._lPersonsCards = new System.Windows.Forms.Label();
            this._ePersonsCards = new System.Windows.Forms.ComboBox();
            this._lResultDB = new System.Windows.Forms.Label();
            this._eResultCardSwipedDB = new System.Windows.Forms.TextBox();
            this._ePin = new System.Windows.Forms.TextBox();
            this._lPin = new System.Windows.Forms.Label();
            this._tbmFullCardNumber = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._lResult = new System.Windows.Forms.Label();
            this._lCardReader = new System.Windows.Forms.Label();
            this._eResultCardSwiped = new System.Windows.Forms.TextBox();
            this._bTest1 = new System.Windows.Forms.Button();
            this._bRefresh2 = new System.Windows.Forms.Button();
            this._cbCardReaderForSimulation = new System.Windows.Forms.ComboBox();
            this._lCardNumber = new System.Windows.Forms.Label();
            this._lMACCCUInformation = new System.Windows.Forms.Label();
            this._lMACCCU = new System.Windows.Forms.Label();
            this._eIPAddress = new System.Windows.Forms.TextBox();
            this._lIPAddress = new System.Windows.Forms.Label();
            this._cbMainboarType = new System.Windows.Forms.ComboBox();
            this._lMainBoardTypeInformation = new System.Windows.Forms.Label();
            this._lMainBoardType = new System.Windows.Forms.Label();
            this._lConfigured = new System.Windows.Forms.Label();
            this._eConfigured = new System.Windows.Forms.TextBox();
            this._panelBack = new System.Windows.Forms.Panel();
            this._eIndex = new Contal.IwQuick.UI.NumericUpDownWithCustomTextFormat();
            this._lIndex = new System.Windows.Forms.Label();
            this._bApply = new System.Windows.Forms.Button();
            this._eState = new System.Windows.Forms.Label();
            this._lState = new System.Windows.Forms.Label();
            this._ofdBrowse = new System.Windows.Forms.OpenFileDialog();
            this._panelCmd = new System.Windows.Forms.Panel();
            this._tcCCU.SuspendLayout();
            this._tpInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudOutputCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudInputCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eMaximumExpectedDCUCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgDCUs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgCardReaders)).BeginInit();
            this._tpTimeSettings.SuspendLayout();
            this._gbTimeZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eMinutes)).BeginInit();
            this._gbSetTimeManually.SuspendLayout();
            this._gbNtpSettings.SuspendLayout();
            this._tpControl.SuspendLayout();
            this._gbLogging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudVerbosityLevel)).BeginInit();
            this._gbCEUpgrade.SuspendLayout();
            this._gbDeviceUpgrade.SuspendLayout();
            this._gbConfiguration.SuspendLayout();
            this._gbDevice.SuspendLayout();
            this._tpInputOutput.SuspendLayout();
            this._scInputsOutputs.Panel1.SuspendLayout();
            this._scInputsOutputs.Panel2.SuspendLayout();
            this._scInputsOutputs.SuspendLayout();
            this._gbInputsCCU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvInputCCU)).BeginInit();
            this._gbOutputsCCU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvOutputCCU)).BeginInit();
            this._tpLevelBSI.SuspendLayout();
            this._gbScheme.SuspendLayout();
            this.panel1.SuspendLayout();
            this._gbBsiLevelInfo.SuspendLayout();
            this._gbBsiLevelSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nBreak)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nZeno)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nShortNormal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nNormalAlarm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nAlarmBreak)).BeginInit();
            this._tpDoorEnvironments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvDoorEnvironments)).BeginInit();
            this._tpDCUsUpgrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvDCUUpgrading)).BeginInit();
            this._tpCRUpgrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvCRUpgrading)).BeginInit();
            this._tpPortSettings.SuspendLayout();
            this._gbComPortSettings.SuspendLayout();
            this._tpIPSettings.SuspendLayout();
            this._gbStaticIPSettings.SuspendLayout();
            this._tpAlarmSettings.SuspendLayout();
            this._gbSettingsForEventlogOnCR.SuspendLayout();
            this._gbServerWatchdog.SuspendLayout();
            this._tpUpsMonitor.SuspendLayout();
            this._gpMaintenance.SuspendLayout();
            this._gpAlarms.SuspendLayout();
            this._gpValues.SuspendLayout();
            this._tpStatistics.SuspendLayout();
            this._tcStatistics.SuspendLayout();
            this._tpCommunicationAndMemory.SuspendLayout();
            this._gbCommunicationStatistic.SuspendLayout();
            this._tlpCOmmunicationStatistics.SuspendLayout();
            this._gbOtherStatistics.SuspendLayout();
            this._tlpPerformanceStatistics.SuspendLayout();
            this._tpThreadMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvThreadMap.DataGrid)).BeginInit();
            this._tpOtherStatistics.SuspendLayout();
            this._gbCcuStartsCount.SuspendLayout();
            this._gbCoprocessorVersion.SuspendLayout();
            this._tpMemory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvMemory.DataGrid)).BeginInit();
            this._tpProcesses.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._tpTesting.SuspendLayout();
            this._gbSimulationAARights.SuspendLayout();
            this._gbTestRoutineDCU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvTestDcu)).BeginInit();
            this._gbTimeZonesDailyPlansState.SuspendLayout();
            this._gbSimulationCardSwiped.SuspendLayout();
            this._panelBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eIndex)).BeginInit();
            this._panelCmd.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(985, 780);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 29);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(884, 780);
            this._bOk.Margin = new System.Windows.Forms.Padding(4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(94, 29);
            this._bOk.TabIndex = 6;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this.OkClick);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(182, 40);
            this._eName.Margin = new System.Windows.Forms.Padding(4);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(779, 20);
            this._eName.TabIndex = 1;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(16, 44);
            this._lName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _tcCCU
            // 
            this._tcCCU.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcCCU.Controls.Add(this._tpInformation);
            this._tcCCU.Controls.Add(this._tpTimeSettings);
            this._tcCCU.Controls.Add(this._tpControl);
            this._tcCCU.Controls.Add(this._tpInputOutput);
            this._tcCCU.Controls.Add(this._tpLevelBSI);
            this._tcCCU.Controls.Add(this._tpDoorEnvironments);
            this._tcCCU.Controls.Add(this._tpDCUsUpgrade);
            this._tcCCU.Controls.Add(this._tpCRUpgrade);
            this._tcCCU.Controls.Add(this._tpPortSettings);
            this._tcCCU.Controls.Add(this._tpIPSettings);
            this._tcCCU.Controls.Add(this._tpAlarmSettings);
            this._tcCCU.Controls.Add(this._tpUpsMonitor);
            this._tcCCU.Controls.Add(this._tpStatistics);
            this._tcCCU.Controls.Add(this._tpUserFolders);
            this._tcCCU.Controls.Add(this._tpReferencedBy);
            this._tcCCU.Controls.Add(this._tpDescription);
            this._tcCCU.Controls.Add(this._tpTesting);
            this._tcCCU.Location = new System.Drawing.Point(15, 206);
            this._tcCCU.Margin = new System.Windows.Forms.Padding(4);
            this._tcCCU.Multiline = true;
            this._tcCCU.Name = "_tcCCU";
            this._tcCCU.SelectedIndex = 0;
            this._tcCCU.Size = new System.Drawing.Size(1068, 574);
            this._tcCCU.TabIndex = 4;
            this._tcCCU.TabStop = false;
            // 
            // _tpInformation
            // 
            this._tpInformation.BackColor = System.Drawing.SystemColors.Control;
            this._tpInformation.Controls.Add(this._bPrecreateCardReader);
            this._tpInformation.Controls.Add(this._nudOutputCount);
            this._tpInformation.Controls.Add(this._lOutputCount);
            this._tpInformation.Controls.Add(this._nudInputCount);
            this._tpInformation.Controls.Add(this._lInputCount);
            this._tpInformation.Controls.Add(this._eMaximumExpectedDCUCount);
            this._tpInformation.Controls.Add(this._lMaximumExpectedDCUCount);
            this._tpInformation.Controls.Add(this._bPrecreateDCU);
            this._tpInformation.Controls.Add(this._bDcuMarkAsDead);
            this._tpInformation.Controls.Add(this._bCrMarkAsDead);
            this._tpInformation.Controls.Add(this._dgDCUs);
            this._tpInformation.Controls.Add(this._dgCardReaders);
            this._tpInformation.Controls.Add(this._bTest);
            this._tpInformation.Location = new System.Drawing.Point(4, 40);
            this._tpInformation.Margin = new System.Windows.Forms.Padding(4);
            this._tpInformation.Name = "_tpInformation";
            this._tpInformation.Padding = new System.Windows.Forms.Padding(4);
            this._tpInformation.Size = new System.Drawing.Size(1060, 530);
            this._tpInformation.TabIndex = 0;
            this._tpInformation.Text = "Information";
            this._tpInformation.Enter += new System.EventHandler(this._tpInformation_Enter);
            // 
            // _bPrecreateCardReader
            // 
            this._bPrecreateCardReader.Location = new System.Drawing.Point(9, 318);
            this._bPrecreateCardReader.Margin = new System.Windows.Forms.Padding(4);
            this._bPrecreateCardReader.Name = "_bPrecreateCardReader";
            this._bPrecreateCardReader.Size = new System.Drawing.Size(226, 29);
            this._bPrecreateCardReader.TabIndex = 12;
            this._bPrecreateCardReader.Text = "Precreate CR";
            this._bPrecreateCardReader.UseVisualStyleBackColor = true;
            this._bPrecreateCardReader.Click += new System.EventHandler(this._bPrecreateCardReader_Click);
            // 
            // _nudOutputCount
            // 
            this._nudOutputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._nudOutputCount.Location = new System.Drawing.Point(242, 431);
            this._nudOutputCount.Margin = new System.Windows.Forms.Padding(4);
            this._nudOutputCount.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this._nudOutputCount.Name = "_nudOutputCount";
            this._nudOutputCount.Size = new System.Drawing.Size(150, 20);
            this._nudOutputCount.TabIndex = 5;
            this._nudOutputCount.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // _lOutputCount
            // 
            this._lOutputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lOutputCount.AutoSize = true;
            this._lOutputCount.Location = new System.Drawing.Point(9, 433);
            this._lOutputCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lOutputCount.Name = "_lOutputCount";
            this._lOutputCount.Size = new System.Drawing.Size(69, 13);
            this._lOutputCount.TabIndex = 4;
            this._lOutputCount.Text = "Output count";
            // 
            // _nudInputCount
            // 
            this._nudInputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._nudInputCount.Location = new System.Drawing.Point(242, 399);
            this._nudInputCount.Margin = new System.Windows.Forms.Padding(4);
            this._nudInputCount.Minimum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this._nudInputCount.Name = "_nudInputCount";
            this._nudInputCount.Size = new System.Drawing.Size(150, 20);
            this._nudInputCount.TabIndex = 5;
            this._nudInputCount.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            // 
            // _lInputCount
            // 
            this._lInputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lInputCount.AutoSize = true;
            this._lInputCount.Location = new System.Drawing.Point(9, 401);
            this._lInputCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lInputCount.Name = "_lInputCount";
            this._lInputCount.Size = new System.Drawing.Size(61, 13);
            this._lInputCount.TabIndex = 4;
            this._lInputCount.Text = "Input count";
            // 
            // _eMaximumExpectedDCUCount
            // 
            this._eMaximumExpectedDCUCount.Location = new System.Drawing.Point(242, 8);
            this._eMaximumExpectedDCUCount.Margin = new System.Windows.Forms.Padding(4);
            this._eMaximumExpectedDCUCount.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this._eMaximumExpectedDCUCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eMaximumExpectedDCUCount.Name = "_eMaximumExpectedDCUCount";
            this._eMaximumExpectedDCUCount.Size = new System.Drawing.Size(150, 20);
            this._eMaximumExpectedDCUCount.TabIndex = 5;
            this._eMaximumExpectedDCUCount.Value = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this._eMaximumExpectedDCUCount.ValueChanged += new System.EventHandler(this._eMaximumExpectedDCUCount_ValueChanged);
            // 
            // _lMaximumExpectedDCUCount
            // 
            this._lMaximumExpectedDCUCount.AutoSize = true;
            this._lMaximumExpectedDCUCount.Location = new System.Drawing.Point(8, 10);
            this._lMaximumExpectedDCUCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMaximumExpectedDCUCount.Name = "_lMaximumExpectedDCUCount";
            this._lMaximumExpectedDCUCount.Size = new System.Drawing.Size(154, 13);
            this._lMaximumExpectedDCUCount.TabIndex = 4;
            this._lMaximumExpectedDCUCount.Text = "Maximum expected DCU count";
            // 
            // _bPrecreateDCU
            // 
            this._bPrecreateDCU.Location = new System.Drawing.Point(9, 88);
            this._bPrecreateDCU.Margin = new System.Windows.Forms.Padding(4);
            this._bPrecreateDCU.Name = "_bPrecreateDCU";
            this._bPrecreateDCU.Size = new System.Drawing.Size(226, 29);
            this._bPrecreateDCU.TabIndex = 8;
            this._bPrecreateDCU.Text = "Precreate DCU";
            this._bPrecreateDCU.UseVisualStyleBackColor = true;
            this._bPrecreateDCU.Click += new System.EventHandler(this._bPrecreateDCU_Click);
            // 
            // _bDcuMarkAsDead
            // 
            this._bDcuMarkAsDead.Enabled = false;
            this._bDcuMarkAsDead.Location = new System.Drawing.Point(9, 52);
            this._bDcuMarkAsDead.Margin = new System.Windows.Forms.Padding(4);
            this._bDcuMarkAsDead.Name = "_bDcuMarkAsDead";
            this._bDcuMarkAsDead.Size = new System.Drawing.Size(226, 29);
            this._bDcuMarkAsDead.TabIndex = 7;
            this._bDcuMarkAsDead.Text = "Mark as dead";
            this._bDcuMarkAsDead.UseVisualStyleBackColor = true;
            this._bDcuMarkAsDead.Click += new System.EventHandler(this._bDcuMarkAsDead_Click);
            // 
            // _bCrMarkAsDead
            // 
            this._bCrMarkAsDead.Enabled = false;
            this._bCrMarkAsDead.Location = new System.Drawing.Point(9, 282);
            this._bCrMarkAsDead.Margin = new System.Windows.Forms.Padding(4);
            this._bCrMarkAsDead.Name = "_bCrMarkAsDead";
            this._bCrMarkAsDead.Size = new System.Drawing.Size(226, 29);
            this._bCrMarkAsDead.TabIndex = 11;
            this._bCrMarkAsDead.Text = "Mark as Dead";
            this._bCrMarkAsDead.UseVisualStyleBackColor = true;
            this._bCrMarkAsDead.Click += new System.EventHandler(this._bCrMarkAsDead_Click);
            // 
            // _dgDCUs
            // 
            this._dgDCUs.AllowUserToAddRows = false;
            this._dgDCUs.AllowUserToDeleteRows = false;
            this._dgDCUs.AllowUserToResizeRows = false;
            this._dgDCUs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgDCUs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgDCUs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgDCUs.Location = new System.Drawing.Point(242, 52);
            this._dgDCUs.Margin = new System.Windows.Forms.Padding(4);
            this._dgDCUs.MultiSelect = false;
            this._dgDCUs.Name = "_dgDCUs";
            this._dgDCUs.ReadOnly = true;
            this._dgDCUs.RowHeadersVisible = false;
            this._dgDCUs.RowTemplate.Height = 24;
            this._dgDCUs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgDCUs.Size = new System.Drawing.Size(776, 339);
            this._dgDCUs.TabIndex = 9;
            this._dgDCUs.TabStop = false;
            this._dgDCUs.DoubleClick += new System.EventHandler(this._dgDCUs_DoubleClick);
            this._dgDCUs.SelectionChanged += new System.EventHandler(this._dgDCUs_SelectionChanged);
            // 
            // _dgCardReaders
            // 
            this._dgCardReaders.AllowUserToAddRows = false;
            this._dgCardReaders.AllowUserToDeleteRows = false;
            this._dgCardReaders.AllowUserToResizeRows = false;
            this._dgCardReaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgCardReaders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgCardReaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgCardReaders.Location = new System.Drawing.Point(242, 52);
            this._dgCardReaders.Margin = new System.Windows.Forms.Padding(4);
            this._dgCardReaders.MultiSelect = false;
            this._dgCardReaders.Name = "_dgCardReaders";
            this._dgCardReaders.ReadOnly = true;
            this._dgCardReaders.RowHeadersVisible = false;
            this._dgCardReaders.RowTemplate.Height = 24;
            this._dgCardReaders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgCardReaders.Size = new System.Drawing.Size(776, 339);
            this._dgCardReaders.TabIndex = 13;
            this._dgCardReaders.TabStop = false;
            this._dgCardReaders.DoubleClick += new System.EventHandler(this._dgCardReaders_DoubleClick);
            this._dgCardReaders.SelectionChanged += new System.EventHandler(this._dgCardReaders_SelectionChanged);
            // 
            // _bTest
            // 
            this._bTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bTest.Location = new System.Drawing.Point(11, 355);
            this._bTest.Margin = new System.Windows.Forms.Padding(2);
            this._bTest.Name = "_bTest";
            this._bTest.Size = new System.Drawing.Size(78, 26);
            this._bTest.TabIndex = 14;
            this._bTest.Text = "Test";
            this._bTest.UseVisualStyleBackColor = true;
            this._bTest.Visible = false;
            this._bTest.Click += new System.EventHandler(this._bTest_Click);
            // 
            // _tpTimeSettings
            // 
            this._tpTimeSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpTimeSettings.Controls.Add(this._gbTimeZone);
            this._tpTimeSettings.Controls.Add(this._cbSyncingTimeFromServer);
            this._tpTimeSettings.Controls.Add(this._gbSetTimeManually);
            this._tpTimeSettings.Controls.Add(this._bTimeRefresh);
            this._tpTimeSettings.Controls.Add(this._eTime);
            this._tpTimeSettings.Controls.Add(this._lTime);
            this._tpTimeSettings.Controls.Add(this._gbNtpSettings);
            this._tpTimeSettings.Location = new System.Drawing.Point(4, 40);
            this._tpTimeSettings.Margin = new System.Windows.Forms.Padding(4);
            this._tpTimeSettings.Name = "_tpTimeSettings";
            this._tpTimeSettings.Padding = new System.Windows.Forms.Padding(4);
            this._tpTimeSettings.Size = new System.Drawing.Size(1060, 530);
            this._tpTimeSettings.TabIndex = 2;
            this._tpTimeSettings.Text = "Time Settings";
            // 
            // _gbTimeZone
            // 
            this._gbTimeZone.Controls.Add(this._cbTimeZone);
            this._gbTimeZone.Controls.Add(this._lTimeZone);
            this._gbTimeZone.Controls.Add(this._lTimeShift);
            this._gbTimeZone.Controls.Add(this._lHours);
            this._gbTimeZone.Controls.Add(this._lMinutes);
            this._gbTimeZone.Controls.Add(this._eHours);
            this._gbTimeZone.Controls.Add(this._eMinutes);
            this._gbTimeZone.Controls.Add(this._lDeltaValue);
            this._gbTimeZone.Controls.Add(this._lDelta);
            this._gbTimeZone.Location = new System.Drawing.Point(556, 7);
            this._gbTimeZone.Name = "_gbTimeZone";
            this._gbTimeZone.Size = new System.Drawing.Size(488, 150);
            this._gbTimeZone.TabIndex = 16;
            this._gbTimeZone.TabStop = false;
            this._gbTimeZone.Text = "Time zone";
            this._gbTimeZone.Visible = false;
            // 
            // _cbTimeZone
            // 
            this._cbTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTimeZone.FormattingEnabled = true;
            this._cbTimeZone.Location = new System.Drawing.Point(167, 20);
            this._cbTimeZone.Margin = new System.Windows.Forms.Padding(4);
            this._cbTimeZone.Name = "_cbTimeZone";
            this._cbTimeZone.Size = new System.Drawing.Size(295, 21);
            this._cbTimeZone.TabIndex = 1;
            this._cbTimeZone.TabStop = false;
            this._cbTimeZone.SelectedIndexChanged += new System.EventHandler(this._cbTimeZone_SelectedIndexChanged);
            // 
            // _lTimeZone
            // 
            this._lTimeZone.AutoSize = true;
            this._lTimeZone.Location = new System.Drawing.Point(10, 23);
            this._lTimeZone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTimeZone.Name = "_lTimeZone";
            this._lTimeZone.Size = new System.Drawing.Size(58, 13);
            this._lTimeZone.TabIndex = 0;
            this._lTimeZone.Text = "Time Zone";
            // 
            // _lTimeShift
            // 
            this._lTimeShift.AutoSize = true;
            this._lTimeShift.Location = new System.Drawing.Point(10, 72);
            this._lTimeShift.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTimeShift.Name = "_lTimeShift";
            this._lTimeShift.Size = new System.Drawing.Size(51, 13);
            this._lTimeShift.TabIndex = 2;
            this._lTimeShift.Text = "TimeShift";
            // 
            // _lHours
            // 
            this._lHours.AutoSize = true;
            this._lHours.Location = new System.Drawing.Point(163, 50);
            this._lHours.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lHours.Name = "_lHours";
            this._lHours.Size = new System.Drawing.Size(35, 13);
            this._lHours.TabIndex = 3;
            this._lHours.Text = "Hours";
            // 
            // _lMinutes
            // 
            this._lMinutes.AutoSize = true;
            this._lMinutes.Location = new System.Drawing.Point(316, 50);
            this._lMinutes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMinutes.Name = "_lMinutes";
            this._lMinutes.Size = new System.Drawing.Size(44, 13);
            this._lMinutes.TabIndex = 5;
            this._lMinutes.Text = "Minutes";
            // 
            // _eHours
            // 
            this._eHours.Location = new System.Drawing.Point(167, 70);
            this._eHours.Margin = new System.Windows.Forms.Padding(4);
            this._eHours.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this._eHours.Minimum = new decimal(new int[] {
            24,
            0,
            0,
            -2147483648});
            this._eHours.Name = "_eHours";
            this._eHours.Size = new System.Drawing.Size(144, 20);
            this._eHours.TabIndex = 4;
            this._eHours.TabStop = false;
            this._eHours.ValueChanged += new System.EventHandler(this._eHours_ValueChanged);
            // 
            // _eMinutes
            // 
            this._eMinutes.Location = new System.Drawing.Point(320, 70);
            this._eMinutes.Margin = new System.Windows.Forms.Padding(4);
            this._eMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this._eMinutes.Minimum = new decimal(new int[] {
            59,
            0,
            0,
            -2147483648});
            this._eMinutes.Name = "_eMinutes";
            this._eMinutes.Size = new System.Drawing.Size(144, 20);
            this._eMinutes.TabIndex = 6;
            this._eMinutes.TabStop = false;
            this._eMinutes.ValueChanged += new System.EventHandler(this._eMinutes_ValueChanged);
            // 
            // _lDeltaValue
            // 
            this._lDeltaValue.AutoSize = true;
            this._lDeltaValue.Location = new System.Drawing.Point(164, 106);
            this._lDeltaValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDeltaValue.Name = "_lDeltaValue";
            this._lDeltaValue.Size = new System.Drawing.Size(0, 13);
            this._lDeltaValue.TabIndex = 8;
            // 
            // _lDelta
            // 
            this._lDelta.AutoSize = true;
            this._lDelta.Location = new System.Drawing.Point(10, 106);
            this._lDelta.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDelta.Name = "_lDelta";
            this._lDelta.Size = new System.Drawing.Size(32, 13);
            this._lDelta.TabIndex = 7;
            this._lDelta.Text = "Delta";
            // 
            // _cbSyncingTimeFromServer
            // 
            this._cbSyncingTimeFromServer.AutoSize = true;
            this._cbSyncingTimeFromServer.Checked = true;
            this._cbSyncingTimeFromServer.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this._cbSyncingTimeFromServer.Location = new System.Drawing.Point(8, 115);
            this._cbSyncingTimeFromServer.Margin = new System.Windows.Forms.Padding(4);
            this._cbSyncingTimeFromServer.Name = "_cbSyncingTimeFromServer";
            this._cbSyncingTimeFromServer.Size = new System.Drawing.Size(337, 17);
            this._cbSyncingTimeFromServer.TabIndex = 15;
            this._cbSyncingTimeFromServer.Text = "Periodic syncing of time from Nova Server without stratum analysis";
            this._cbSyncingTimeFromServer.ThreeState = true;
            this._cbSyncingTimeFromServer.UseVisualStyleBackColor = true;
            this._cbSyncingTimeFromServer.CheckStateChanged += new System.EventHandler(this._cbSyncingTimeFromServer_CheckStateChanged);
            // 
            // _gbSetTimeManually
            // 
            this._gbSetTimeManually.Controls.Add(this._bSet2);
            this._gbSetTimeManually.Controls.Add(this._tbdpSetTimeManually);
            this._gbSetTimeManually.Location = new System.Drawing.Point(8, 41);
            this._gbSetTimeManually.Margin = new System.Windows.Forms.Padding(4);
            this._gbSetTimeManually.Name = "_gbSetTimeManually";
            this._gbSetTimeManually.Padding = new System.Windows.Forms.Padding(4);
            this._gbSetTimeManually.Size = new System.Drawing.Size(456, 66);
            this._gbSetTimeManually.TabIndex = 14;
            this._gbSetTimeManually.TabStop = false;
            this._gbSetTimeManually.Text = "Set time manually";
            // 
            // _bSet2
            // 
            this._bSet2.Enabled = false;
            this._bSet2.Location = new System.Drawing.Point(344, 24);
            this._bSet2.Margin = new System.Windows.Forms.Padding(4);
            this._bSet2.Name = "_bSet2";
            this._bSet2.Size = new System.Drawing.Size(94, 29);
            this._bSet2.TabIndex = 14;
            this._bSet2.Text = "Set";
            this._bSet2.UseVisualStyleBackColor = true;
            this._bSet2.Click += new System.EventHandler(this._bSet2_Click);
            this._bSet2.EnabledChanged += new System.EventHandler(this._bSet2_EnabledChanged);
            // 
            // _tbdpSetTimeManually
            // 
            this._tbdpSetTimeManually.addActualTime = false;
            this._tbdpSetTimeManually.BackColor = System.Drawing.Color.Transparent;
            this._tbdpSetTimeManually.ButtonClearDateImage = null;
            this._tbdpSetTimeManually.ButtonClearDateText = "";
            this._tbdpSetTimeManually.ButtonClearDateWidth = 23;
            this._tbdpSetTimeManually.ButtonDateImage = null;
            this._tbdpSetTimeManually.ButtonDateText = "";
            this._tbdpSetTimeManually.ButtonDateWidth = 23;
            this._tbdpSetTimeManually.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._tbdpSetTimeManually.DateFormName = "Calendar";
            this._tbdpSetTimeManually.LocalizationHelper = null;
            this._tbdpSetTimeManually.Location = new System.Drawing.Point(8, 24);
            this._tbdpSetTimeManually.Margin = new System.Windows.Forms.Padding(4);
            this._tbdpSetTimeManually.MaximumSize = new System.Drawing.Size(1250, 75);
            this._tbdpSetTimeManually.MinimumSize = new System.Drawing.Size(70, 28);
            this._tbdpSetTimeManually.Name = "_tbdpSetTimeManually";
            this._tbdpSetTimeManually.ReadOnly = false;
            this._tbdpSetTimeManually.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.Unknown;
            this._tbdpSetTimeManually.Size = new System.Drawing.Size(278, 28);
            this._tbdpSetTimeManually.TabIndex = 13;
            this._tbdpSetTimeManually.ValidateAfter = 2;
            this._tbdpSetTimeManually.ValidationEnabled = true;
            this._tbdpSetTimeManually.ValidationError = "";
            this._tbdpSetTimeManually.Value = null;
            this._tbdpSetTimeManually.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this._tbdpSetTimeManually_TextDateChanged);
            // 
            // _bTimeRefresh
            // 
            this._bTimeRefresh.Location = new System.Drawing.Point(370, 8);
            this._bTimeRefresh.Margin = new System.Windows.Forms.Padding(4);
            this._bTimeRefresh.Name = "_bTimeRefresh";
            this._bTimeRefresh.Size = new System.Drawing.Size(94, 29);
            this._bTimeRefresh.TabIndex = 11;
            this._bTimeRefresh.Text = "Refresh";
            this._bTimeRefresh.UseVisualStyleBackColor = true;
            this._bTimeRefresh.Click += new System.EventHandler(this._bTimeRefresh_Click);
            // 
            // _eTime
            // 
            this._eTime.AutoSize = true;
            this._eTime.Location = new System.Drawing.Point(134, 19);
            this._eTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._eTime.Name = "_eTime";
            this._eTime.Size = new System.Drawing.Size(0, 13);
            this._eTime.TabIndex = 10;
            // 
            // _lTime
            // 
            this._lTime.AutoSize = true;
            this._lTime.Location = new System.Drawing.Point(5, 19);
            this._lTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTime.Name = "_lTime";
            this._lTime.Size = new System.Drawing.Size(30, 13);
            this._lTime.TabIndex = 9;
            this._lTime.Text = "Time";
            // 
            // _gbNtpSettings
            // 
            this._gbNtpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._gbNtpSettings.Controls.Add(this._chbInheritedGeneralNtpSettings);
            this._gbNtpSettings.Controls.Add(this._bResolve);
            this._gbNtpSettings.Controls.Add(this._lIPAddress1);
            this._gbNtpSettings.Controls.Add(this._eNtpDnsHostName);
            this._gbNtpSettings.Controls.Add(this._eSNTPIpAddress);
            this._gbNtpSettings.Controls.Add(this._lDNSHostname);
            this._gbNtpSettings.Controls.Add(this._bAdd);
            this._gbNtpSettings.Controls.Add(this._lSNTPIPAddresses);
            this._gbNtpSettings.Controls.Add(this._bDelete);
            this._gbNtpSettings.Location = new System.Drawing.Point(8, 144);
            this._gbNtpSettings.Margin = new System.Windows.Forms.Padding(4);
            this._gbNtpSettings.Name = "_gbNtpSettings";
            this._gbNtpSettings.Padding = new System.Windows.Forms.Padding(4);
            this._gbNtpSettings.Size = new System.Drawing.Size(456, 378);
            this._gbNtpSettings.TabIndex = 12;
            this._gbNtpSettings.TabStop = false;
            this._gbNtpSettings.Text = "NTP Settings";
            // 
            // _chbInheritedGeneralNtpSettings
            // 
            this._chbInheritedGeneralNtpSettings.AutoSize = true;
            this._chbInheritedGeneralNtpSettings.Location = new System.Drawing.Point(11, 118);
            this._chbInheritedGeneralNtpSettings.Margin = new System.Windows.Forms.Padding(4);
            this._chbInheritedGeneralNtpSettings.Name = "_chbInheritedGeneralNtpSettings";
            this._chbInheritedGeneralNtpSettings.Size = new System.Drawing.Size(157, 17);
            this._chbInheritedGeneralNtpSettings.TabIndex = 6;
            this._chbInheritedGeneralNtpSettings.Text = "Inherit general NTP settings";
            this._chbInheritedGeneralNtpSettings.UseVisualStyleBackColor = true;
            this._chbInheritedGeneralNtpSettings.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _bResolve
            // 
            this._bResolve.Location = new System.Drawing.Point(355, 81);
            this._bResolve.Margin = new System.Windows.Forms.Padding(4);
            this._bResolve.Name = "_bResolve";
            this._bResolve.Size = new System.Drawing.Size(94, 29);
            this._bResolve.TabIndex = 5;
            this._bResolve.Text = "Resolve";
            this._bResolve.UseVisualStyleBackColor = true;
            this._bResolve.Click += new System.EventHandler(this._bResolve_Click);
            // 
            // _lIPAddress1
            // 
            this._lIPAddress1.AutoSize = true;
            this._lIPAddress1.Location = new System.Drawing.Point(8, 16);
            this._lIPAddress1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lIPAddress1.Name = "_lIPAddress1";
            this._lIPAddress1.Size = new System.Drawing.Size(57, 13);
            this._lIPAddress1.TabIndex = 0;
            this._lIPAddress1.Text = "IP address";
            // 
            // _eNtpDnsHostName
            // 
            this._eNtpDnsHostName.Location = new System.Drawing.Point(11, 85);
            this._eNtpDnsHostName.Margin = new System.Windows.Forms.Padding(4);
            this._eNtpDnsHostName.Name = "_eNtpDnsHostName";
            this._eNtpDnsHostName.Size = new System.Drawing.Size(335, 20);
            this._eNtpDnsHostName.TabIndex = 4;
            // 
            // _eSNTPIpAddress
            // 
            this._eSNTPIpAddress.Location = new System.Drawing.Point(11, 36);
            this._eSNTPIpAddress.Margin = new System.Windows.Forms.Padding(4);
            this._eSNTPIpAddress.Name = "_eSNTPIpAddress";
            this._eSNTPIpAddress.Size = new System.Drawing.Size(335, 20);
            this._eSNTPIpAddress.TabIndex = 1;
            this._eSNTPIpAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._eSNTPIpAddress_KeyPress);
            // 
            // _lDNSHostname
            // 
            this._lDNSHostname.AutoSize = true;
            this._lDNSHostname.Location = new System.Drawing.Point(8, 65);
            this._lDNSHostname.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDNSHostname.Name = "_lDNSHostname";
            this._lDNSHostname.Size = new System.Drawing.Size(79, 13);
            this._lDNSHostname.TabIndex = 3;
            this._lDNSHostname.Text = "DNS hostname";
            // 
            // _bAdd
            // 
            this._bAdd.Location = new System.Drawing.Point(355, 34);
            this._bAdd.Margin = new System.Windows.Forms.Padding(4);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(94, 29);
            this._bAdd.TabIndex = 2;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _lSNTPIPAddresses
            // 
            this._lSNTPIPAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._lSNTPIPAddresses.FormattingEnabled = true;
            this._lSNTPIPAddresses.Location = new System.Drawing.Point(11, 146);
            this._lSNTPIPAddresses.Margin = new System.Windows.Forms.Padding(4);
            this._lSNTPIPAddresses.Name = "_lSNTPIPAddresses";
            this._lSNTPIPAddresses.Size = new System.Drawing.Size(437, 121);
            this._lSNTPIPAddresses.TabIndex = 7;
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDelete.Location = new System.Drawing.Point(356, 342);
            this._bDelete.Margin = new System.Windows.Forms.Padding(4);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(94, 29);
            this._bDelete.TabIndex = 8;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _tpControl
            // 
            this._tpControl.BackColor = System.Drawing.SystemColors.Control;
            this._tpControl.Controls.Add(this._gbLogging);
            this._tpControl.Controls.Add(this._gbCEUpgrade);
            this._tpControl.Controls.Add(this._gbDeviceUpgrade);
            this._tpControl.Controls.Add(this._gbConfiguration);
            this._tpControl.Controls.Add(this._gbDevice);
            this._tpControl.Location = new System.Drawing.Point(4, 40);
            this._tpControl.Margin = new System.Windows.Forms.Padding(4);
            this._tpControl.Name = "_tpControl";
            this._tpControl.Padding = new System.Windows.Forms.Padding(4);
            this._tpControl.Size = new System.Drawing.Size(1060, 530);
            this._tpControl.TabIndex = 3;
            this._tpControl.Text = "Control";
            this._tpControl.Enter += new System.EventHandler(this._tpControl_Enter);
            // 
            // _gbLogging
            // 
            this._gbLogging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._gbLogging.Controls.Add(this._prbDebugFilesTransfer);
            this._gbLogging.Controls.Add(this._bMakeLogDump);
            this._gbLogging.Controls.Add(this._bSet1);
            this._gbLogging.Controls.Add(this._lVerbosityLevel);
            this._gbLogging.Controls.Add(this._nudVerbosityLevel);
            this._gbLogging.Controls.Add(this._cbVerbosityLevel);
            this._gbLogging.Location = new System.Drawing.Point(10, 404);
            this._gbLogging.Margin = new System.Windows.Forms.Padding(4);
            this._gbLogging.Name = "_gbLogging";
            this._gbLogging.Padding = new System.Windows.Forms.Padding(4);
            this._gbLogging.Size = new System.Drawing.Size(324, 115);
            this._gbLogging.TabIndex = 6;
            this._gbLogging.TabStop = false;
            this._gbLogging.Text = "Diagnostic logging";
            // 
            // _prbDebugFilesTransfer
            // 
            this._prbDebugFilesTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._prbDebugFilesTransfer.Location = new System.Drawing.Point(32, 79);
            this._prbDebugFilesTransfer.Name = "_prbDebugFilesTransfer";
            this._prbDebugFilesTransfer.Size = new System.Drawing.Size(100, 23);
            this._prbDebugFilesTransfer.TabIndex = 21;
            this._prbDebugFilesTransfer.Visible = false;
            // 
            // _bMakeLogDump
            // 
            this._bMakeLogDump.Location = new System.Drawing.Point(8, 77);
            this._bMakeLogDump.Margin = new System.Windows.Forms.Padding(4);
            this._bMakeLogDump.Name = "_bMakeLogDump";
            this._bMakeLogDump.Size = new System.Drawing.Size(150, 29);
            this._bMakeLogDump.TabIndex = 4;
            this._bMakeLogDump.Text = "MakeLogDump";
            this._bMakeLogDump.UseVisualStyleBackColor = true;
            this._bMakeLogDump.Click += new System.EventHandler(this._bMakeLogDump_Click);
            // 
            // _bSet1
            // 
            this._bSet1.Location = new System.Drawing.Point(242, 39);
            this._bSet1.Margin = new System.Windows.Forms.Padding(4);
            this._bSet1.Name = "_bSet1";
            this._bSet1.Size = new System.Drawing.Size(71, 29);
            this._bSet1.TabIndex = 3;
            this._bSet1.Text = "Set";
            this._bSet1.UseVisualStyleBackColor = true;
            this._bSet1.Click += new System.EventHandler(this._bSet1_Click);
            // 
            // _lVerbosityLevel
            // 
            this._lVerbosityLevel.AutoSize = true;
            this._lVerbosityLevel.Location = new System.Drawing.Point(8, 20);
            this._lVerbosityLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lVerbosityLevel.Name = "_lVerbosityLevel";
            this._lVerbosityLevel.Size = new System.Drawing.Size(33, 13);
            this._lVerbosityLevel.TabIndex = 2;
            this._lVerbosityLevel.Text = "Level";
            // 
            // _nudVerbosityLevel
            // 
            this._nudVerbosityLevel.Location = new System.Drawing.Point(166, 41);
            this._nudVerbosityLevel.Margin = new System.Windows.Forms.Padding(4);
            this._nudVerbosityLevel.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this._nudVerbosityLevel.Name = "_nudVerbosityLevel";
            this._nudVerbosityLevel.Size = new System.Drawing.Size(69, 20);
            this._nudVerbosityLevel.TabIndex = 1;
            this._nudVerbosityLevel.ValueChanged += new System.EventHandler(this._nudVerbosityLevel_ValueChanged);
            // 
            // _cbVerbosityLevel
            // 
            this._cbVerbosityLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbVerbosityLevel.FormattingEnabled = true;
            this._cbVerbosityLevel.Location = new System.Drawing.Point(8, 40);
            this._cbVerbosityLevel.Margin = new System.Windows.Forms.Padding(4);
            this._cbVerbosityLevel.Name = "_cbVerbosityLevel";
            this._cbVerbosityLevel.Size = new System.Drawing.Size(150, 21);
            this._cbVerbosityLevel.TabIndex = 0;
            this._cbVerbosityLevel.SelectedValueChanged += new System.EventHandler(this._cbVerbosityLevel_SelectedValueChanged);
            // 
            // _gbCEUpgrade
            // 
            this._gbCEUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._gbCEUpgrade.Controls.Add(this._ceUpgrade);
            this._gbCEUpgrade.Controls.Add(this._prbTransferCe);
            this._gbCEUpgrade.Controls.Add(this._lImageVersion);
            this._gbCEUpgrade.Controls.Add(this._eImageVersion);
            this._gbCEUpgrade.Controls.Add(this._bRefresh5);
            this._gbCEUpgrade.Controls.Add(this._eCEUpgradeProgress);
            this._gbCEUpgrade.Controls.Add(this._lCEUpgradeProgress);
            this._gbCEUpgrade.Controls.Add(this._bStopTransferCEFile);
            this._gbCEUpgrade.Controls.Add(this._lTransferPercents);
            this._gbCEUpgrade.Controls.Add(this._lAvailableCEFiles);
            this._gbCEUpgrade.Location = new System.Drawing.Point(702, 8);
            this._gbCEUpgrade.Margin = new System.Windows.Forms.Padding(4);
            this._gbCEUpgrade.Name = "_gbCEUpgrade";
            this._gbCEUpgrade.Padding = new System.Windows.Forms.Padding(4);
            this._gbCEUpgrade.Size = new System.Drawing.Size(348, 511);
            this._gbCEUpgrade.TabIndex = 5;
            this._gbCEUpgrade.TabStop = false;
            this._gbCEUpgrade.Text = "Windows CE image upgrade";
            // 
            // _ceUpgrade
            // 
            this._ceUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._ceUpgrade.Location = new System.Drawing.Point(11, 106);
            this._ceUpgrade.Name = "_ceUpgrade";
            this._ceUpgrade.Size = new System.Drawing.Size(329, 278);
            this._ceUpgrade.TabIndex = 23;
            this._ceUpgrade.Text = "_ceUpgrade";
            this._ceUpgrade.Child = this._ceUpgradeFiles;
            // 
            // _prbTransferCe
            // 
            this._prbTransferCe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._prbTransferCe.Location = new System.Drawing.Point(151, 397);
            this._prbTransferCe.Name = "_prbTransferCe";
            this._prbTransferCe.Size = new System.Drawing.Size(100, 23);
            this._prbTransferCe.TabIndex = 22;
            // 
            // _lImageVersion
            // 
            this._lImageVersion.AutoSize = true;
            this._lImageVersion.Location = new System.Drawing.Point(8, 28);
            this._lImageVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lImageVersion.Name = "_lImageVersion";
            this._lImageVersion.Size = new System.Drawing.Size(73, 13);
            this._lImageVersion.TabIndex = 0;
            this._lImageVersion.Text = "Image version";
            // 
            // _eImageVersion
            // 
            this._eImageVersion.Location = new System.Drawing.Point(151, 24);
            this._eImageVersion.Margin = new System.Windows.Forms.Padding(4);
            this._eImageVersion.Name = "_eImageVersion";
            this._eImageVersion.ReadOnly = true;
            this._eImageVersion.Size = new System.Drawing.Size(188, 20);
            this._eImageVersion.TabIndex = 1;
            // 
            // _bRefresh5
            // 
            this._bRefresh5.Location = new System.Drawing.Point(246, 59);
            this._bRefresh5.Margin = new System.Windows.Forms.Padding(4);
            this._bRefresh5.Name = "_bRefresh5";
            this._bRefresh5.Size = new System.Drawing.Size(94, 29);
            this._bRefresh5.TabIndex = 2;
            this._bRefresh5.Text = "Refresh";
            this._bRefresh5.UseVisualStyleBackColor = true;
            this._bRefresh5.Click += new System.EventHandler(this.RefreshWinCEImageVersionClick);
            // 
            // _eCEUpgradeProgress
            // 
            this._eCEUpgradeProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._eCEUpgradeProgress.Location = new System.Drawing.Point(152, 438);
            this._eCEUpgradeProgress.Margin = new System.Windows.Forms.Padding(4);
            this._eCEUpgradeProgress.Name = "_eCEUpgradeProgress";
            this._eCEUpgradeProgress.ReadOnly = true;
            this._eCEUpgradeProgress.Size = new System.Drawing.Size(183, 20);
            this._eCEUpgradeProgress.TabIndex = 11;
            // 
            // _lCEUpgradeProgress
            // 
            this._lCEUpgradeProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lCEUpgradeProgress.AutoSize = true;
            this._lCEUpgradeProgress.Location = new System.Drawing.Point(9, 447);
            this._lCEUpgradeProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCEUpgradeProgress.Name = "_lCEUpgradeProgress";
            this._lCEUpgradeProgress.Size = new System.Drawing.Size(91, 13);
            this._lCEUpgradeProgress.TabIndex = 10;
            this._lCEUpgradeProgress.Text = "Upgrade progress";
            // 
            // _bStopTransferCEFile
            // 
            this._bStopTransferCEFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bStopTransferCEFile.Location = new System.Drawing.Point(267, 397);
            this._bStopTransferCEFile.Margin = new System.Windows.Forms.Padding(4);
            this._bStopTransferCEFile.Name = "_bStopTransferCEFile";
            this._bStopTransferCEFile.Size = new System.Drawing.Size(73, 23);
            this._bStopTransferCEFile.TabIndex = 6;
            this._bStopTransferCEFile.Text = "Stop";
            this._bStopTransferCEFile.UseVisualStyleBackColor = true;
            this._bStopTransferCEFile.Click += new System.EventHandler(this._bStopTransferCEFile_Click);
            // 
            // _lTransferPercents
            // 
            this._lTransferPercents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lTransferPercents.AutoSize = true;
            this._lTransferPercents.Location = new System.Drawing.Point(9, 402);
            this._lTransferPercents.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTransferPercents.Name = "_lTransferPercents";
            this._lTransferPercents.Size = new System.Drawing.Size(89, 13);
            this._lTransferPercents.TabIndex = 7;
            this._lTransferPercents.Text = "Transfer progress";
            // 
            // _lAvailableCEFiles
            // 
            this._lAvailableCEFiles.AutoSize = true;
            this._lAvailableCEFiles.Location = new System.Drawing.Point(8, 86);
            this._lAvailableCEFiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAvailableCEFiles.Name = "_lAvailableCEFiles";
            this._lAvailableCEFiles.Size = new System.Drawing.Size(71, 13);
            this._lAvailableCEFiles.TabIndex = 3;
            this._lAvailableCEFiles.Text = "Available files";
            // 
            // _gbDeviceUpgrade
            // 
            this._gbDeviceUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._gbDeviceUpgrade.Controls.Add(this._ccuUpgrade);
            this._gbDeviceUpgrade.Controls.Add(this._prbUnpack);
            this._gbDeviceUpgrade.Controls.Add(this._prbTransfer);
            this._gbDeviceUpgrade.Controls.Add(this._eUpgradeFinalisation);
            this._gbDeviceUpgrade.Controls.Add(this._lUpgradeFinalisation);
            this._gbDeviceUpgrade.Controls.Add(this._lFirmwareVersion);
            this._gbDeviceUpgrade.Controls.Add(this._bRefresh);
            this._gbDeviceUpgrade.Controls.Add(this._eFirmware);
            this._gbDeviceUpgrade.Controls.Add(this._lAvailableVersions);
            this._gbDeviceUpgrade.Controls.Add(this._lTransferProgress);
            this._gbDeviceUpgrade.Controls.Add(this._lUpgradeProgress);
            this._gbDeviceUpgrade.Controls.Add(this._bStopTransfer);
            this._gbDeviceUpgrade.Location = new System.Drawing.Point(340, 8);
            this._gbDeviceUpgrade.Margin = new System.Windows.Forms.Padding(4);
            this._gbDeviceUpgrade.Name = "_gbDeviceUpgrade";
            this._gbDeviceUpgrade.Padding = new System.Windows.Forms.Padding(4);
            this._gbDeviceUpgrade.Size = new System.Drawing.Size(355, 511);
            this._gbDeviceUpgrade.TabIndex = 4;
            this._gbDeviceUpgrade.TabStop = false;
            this._gbDeviceUpgrade.Text = "CCU firmware upgrade";
            // 
            // _ccuUpgrade
            // 
            this._ccuUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._ccuUpgrade.Location = new System.Drawing.Point(11, 106);
            this._ccuUpgrade.Name = "_ccuUpgrade";
            this._ccuUpgrade.Size = new System.Drawing.Size(335, 278);
            this._ccuUpgrade.TabIndex = 22;
            this._ccuUpgrade.Text = "_ccuUpgrade";
            this._ccuUpgrade.Child = this._ccuUpgradeFiles;
            // 
            // _prbUnpack
            // 
            this._prbUnpack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._prbUnpack.Location = new System.Drawing.Point(159, 438);
            this._prbUnpack.Name = "_prbUnpack";
            this._prbUnpack.Size = new System.Drawing.Size(100, 23);
            this._prbUnpack.TabIndex = 21;
            // 
            // _prbTransfer
            // 
            this._prbTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._prbTransfer.Location = new System.Drawing.Point(159, 402);
            this._prbTransfer.Name = "_prbTransfer";
            this._prbTransfer.Size = new System.Drawing.Size(100, 23);
            this._prbTransfer.TabIndex = 20;
            // 
            // _eUpgradeFinalisation
            // 
            this._eUpgradeFinalisation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._eUpgradeFinalisation.Location = new System.Drawing.Point(159, 478);
            this._eUpgradeFinalisation.Margin = new System.Windows.Forms.Padding(4);
            this._eUpgradeFinalisation.Name = "_eUpgradeFinalisation";
            this._eUpgradeFinalisation.ReadOnly = true;
            this._eUpgradeFinalisation.Size = new System.Drawing.Size(188, 20);
            this._eUpgradeFinalisation.TabIndex = 12;
            this._eUpgradeFinalisation.TabStop = false;
            // 
            // _lUpgradeFinalisation
            // 
            this._lUpgradeFinalisation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lUpgradeFinalisation.AutoSize = true;
            this._lUpgradeFinalisation.Location = new System.Drawing.Point(21, 482);
            this._lUpgradeFinalisation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lUpgradeFinalisation.Name = "_lUpgradeFinalisation";
            this._lUpgradeFinalisation.Size = new System.Drawing.Size(59, 13);
            this._lUpgradeFinalisation.TabIndex = 11;
            this._lUpgradeFinalisation.Text = "Finalisation";
            // 
            // _lFirmwareVersion
            // 
            this._lFirmwareVersion.AutoSize = true;
            this._lFirmwareVersion.Location = new System.Drawing.Point(20, 30);
            this._lFirmwareVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFirmwareVersion.Name = "_lFirmwareVersion";
            this._lFirmwareVersion.Size = new System.Drawing.Size(86, 13);
            this._lFirmwareVersion.TabIndex = 10;
            this._lFirmwareVersion.Text = "Firmware version";
            // 
            // _bRefresh
            // 
            this._bRefresh.Location = new System.Drawing.Point(250, 59);
            this._bRefresh.Margin = new System.Windows.Forms.Padding(4);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(98, 29);
            this._bRefresh.TabIndex = 1;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _eFirmware
            // 
            this._eFirmware.Enabled = false;
            this._eFirmware.Location = new System.Drawing.Point(152, 26);
            this._eFirmware.Margin = new System.Windows.Forms.Padding(4);
            this._eFirmware.Name = "_eFirmware";
            this._eFirmware.ReadOnly = true;
            this._eFirmware.Size = new System.Drawing.Size(194, 20);
            this._eFirmware.TabIndex = 0;
            // 
            // _lAvailableVersions
            // 
            this._lAvailableVersions.AutoSize = true;
            this._lAvailableVersions.Location = new System.Drawing.Point(8, 86);
            this._lAvailableVersions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAvailableVersions.Name = "_lAvailableVersions";
            this._lAvailableVersions.Size = new System.Drawing.Size(92, 13);
            this._lAvailableVersions.TabIndex = 0;
            this._lAvailableVersions.Text = "Available versions";
            // 
            // _lTransferProgress
            // 
            this._lTransferProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lTransferProgress.AutoSize = true;
            this._lTransferProgress.Location = new System.Drawing.Point(20, 402);
            this._lTransferProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTransferProgress.Name = "_lTransferProgress";
            this._lTransferProgress.Size = new System.Drawing.Size(89, 13);
            this._lTransferProgress.TabIndex = 3;
            this._lTransferProgress.Text = "Transfer progress";
            // 
            // _lUpgradeProgress
            // 
            this._lUpgradeProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lUpgradeProgress.AutoSize = true;
            this._lUpgradeProgress.Location = new System.Drawing.Point(21, 443);
            this._lUpgradeProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lUpgradeProgress.Name = "_lUpgradeProgress";
            this._lUpgradeProgress.Size = new System.Drawing.Size(88, 13);
            this._lUpgradeProgress.TabIndex = 7;
            this._lUpgradeProgress.Text = "Unpack progress";
            // 
            // _bStopTransfer
            // 
            this._bStopTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bStopTransfer.Location = new System.Drawing.Point(273, 402);
            this._bStopTransfer.Margin = new System.Windows.Forms.Padding(4);
            this._bStopTransfer.Name = "_bStopTransfer";
            this._bStopTransfer.Size = new System.Drawing.Size(73, 23);
            this._bStopTransfer.TabIndex = 5;
            this._bStopTransfer.Text = "Stop";
            this._bStopTransfer.UseVisualStyleBackColor = true;
            this._bStopTransfer.Click += new System.EventHandler(this._bStopTransfer_Click);
            // 
            // _gbConfiguration
            // 
            this._gbConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._gbConfiguration.Controls.Add(this._bDeleteEvents);
            this._gbConfiguration.Controls.Add(this._bCancelConfigurationPassword);
            this._gbConfiguration.Controls.Add(this._bChangeConfigurePassword);
            this._gbConfiguration.Controls.Add(this._bForceReconfiguration);
            this._gbConfiguration.Controls.Add(this._lResultOfAction);
            this._gbConfiguration.Controls.Add(this._eResultOfAction);
            this._gbConfiguration.Controls.Add(this._bConfigureForThisServer);
            this._gbConfiguration.Controls.Add(this._bUnconfigure);
            this._gbConfiguration.Location = new System.Drawing.Point(8, 8);
            this._gbConfiguration.Margin = new System.Windows.Forms.Padding(4);
            this._gbConfiguration.Name = "_gbConfiguration";
            this._gbConfiguration.Padding = new System.Windows.Forms.Padding(4);
            this._gbConfiguration.Size = new System.Drawing.Size(326, 286);
            this._gbConfiguration.TabIndex = 0;
            this._gbConfiguration.TabStop = false;
            this._gbConfiguration.Text = "Configuration";
            // 
            // _bDeleteEvents
            // 
            this._bDeleteEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDeleteEvents.Location = new System.Drawing.Point(8, 249);
            this._bDeleteEvents.Margin = new System.Windows.Forms.Padding(4);
            this._bDeleteEvents.Name = "_bDeleteEvents";
            this._bDeleteEvents.Size = new System.Drawing.Size(206, 29);
            this._bDeleteEvents.TabIndex = 7;
            this._bDeleteEvents.Text = "Delete events";
            this._bDeleteEvents.UseVisualStyleBackColor = true;
            this._bDeleteEvents.Visible = false;
            this._bDeleteEvents.Click += new System.EventHandler(this._bDeleteEvents_Click);
            // 
            // _bCancelConfigurationPassword
            // 
            this._bCancelConfigurationPassword.Location = new System.Drawing.Point(8, 130);
            this._bCancelConfigurationPassword.Margin = new System.Windows.Forms.Padding(4);
            this._bCancelConfigurationPassword.Name = "_bCancelConfigurationPassword";
            this._bCancelConfigurationPassword.Size = new System.Drawing.Size(206, 29);
            this._bCancelConfigurationPassword.TabIndex = 6;
            this._bCancelConfigurationPassword.Text = "Cancel password";
            this._bCancelConfigurationPassword.UseVisualStyleBackColor = true;
            this._bCancelConfigurationPassword.Click += new System.EventHandler(this._bCancelConfigurationPassword_Click);
            // 
            // _bChangeConfigurePassword
            // 
            this._bChangeConfigurePassword.Location = new System.Drawing.Point(8, 94);
            this._bChangeConfigurePassword.Margin = new System.Windows.Forms.Padding(4);
            this._bChangeConfigurePassword.Name = "_bChangeConfigurePassword";
            this._bChangeConfigurePassword.Size = new System.Drawing.Size(206, 29);
            this._bChangeConfigurePassword.TabIndex = 5;
            this._bChangeConfigurePassword.Text = "Change password";
            this._bChangeConfigurePassword.UseVisualStyleBackColor = true;
            this._bChangeConfigurePassword.Click += new System.EventHandler(this._bChangeConfigurePassword_Click);
            // 
            // _bForceReconfiguration
            // 
            this._bForceReconfiguration.Location = new System.Drawing.Point(8, 58);
            this._bForceReconfiguration.Margin = new System.Windows.Forms.Padding(4);
            this._bForceReconfiguration.Name = "_bForceReconfiguration";
            this._bForceReconfiguration.Size = new System.Drawing.Size(206, 29);
            this._bForceReconfiguration.TabIndex = 2;
            this._bForceReconfiguration.Text = "Force reconfiguration";
            this._bForceReconfiguration.UseVisualStyleBackColor = true;
            this._bForceReconfiguration.Click += new System.EventHandler(this._bForceReconfiguration_Click);
            // 
            // _lResultOfAction
            // 
            this._lResultOfAction.AutoSize = true;
            this._lResultOfAction.Location = new System.Drawing.Point(9, 182);
            this._lResultOfAction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lResultOfAction.Name = "_lResultOfAction";
            this._lResultOfAction.Size = new System.Drawing.Size(81, 13);
            this._lResultOfAction.TabIndex = 3;
            this._lResultOfAction.Text = "Result of action";
            // 
            // _eResultOfAction
            // 
            this._eResultOfAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._eResultOfAction.Location = new System.Drawing.Point(125, 182);
            this._eResultOfAction.Margin = new System.Windows.Forms.Padding(4);
            this._eResultOfAction.Multiline = true;
            this._eResultOfAction.Name = "_eResultOfAction";
            this._eResultOfAction.ReadOnly = true;
            this._eResultOfAction.Size = new System.Drawing.Size(189, 59);
            this._eResultOfAction.TabIndex = 4;
            // 
            // _bConfigureForThisServer
            // 
            this._bConfigureForThisServer.Location = new System.Drawing.Point(8, 24);
            this._bConfigureForThisServer.Margin = new System.Windows.Forms.Padding(4);
            this._bConfigureForThisServer.Name = "_bConfigureForThisServer";
            this._bConfigureForThisServer.Size = new System.Drawing.Size(206, 29);
            this._bConfigureForThisServer.TabIndex = 1;
            this._bConfigureForThisServer.Text = "Configure for this server";
            this._bConfigureForThisServer.UseVisualStyleBackColor = true;
            this._bConfigureForThisServer.Click += new System.EventHandler(this._bConfigureForThisServer_Click);
            // 
            // _bUnconfigure
            // 
            this._bUnconfigure.Location = new System.Drawing.Point(8, 24);
            this._bUnconfigure.Margin = new System.Windows.Forms.Padding(4);
            this._bUnconfigure.Name = "_bUnconfigure";
            this._bUnconfigure.Size = new System.Drawing.Size(206, 29);
            this._bUnconfigure.TabIndex = 0;
            this._bUnconfigure.Text = "Unconfigure";
            this._bUnconfigure.UseVisualStyleBackColor = true;
            this._bUnconfigure.Click += new System.EventHandler(this._bUnconfigure_Click);
            // 
            // _gbDevice
            // 
            this._gbDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._gbDevice.Controls.Add(this._bMemoryCollect);
            this._gbDevice.Controls.Add(this._bSoftReset);
            this._gbDevice.Controls.Add(this._bHardReset);
            this._gbDevice.Location = new System.Drawing.Point(9, 302);
            this._gbDevice.Margin = new System.Windows.Forms.Padding(4);
            this._gbDevice.Name = "_gbDevice";
            this._gbDevice.Padding = new System.Windows.Forms.Padding(4);
            this._gbDevice.Size = new System.Drawing.Size(325, 94);
            this._gbDevice.TabIndex = 1;
            this._gbDevice.TabStop = false;
            this._gbDevice.Text = "Device";
            // 
            // _bMemoryCollect
            // 
            this._bMemoryCollect.Location = new System.Drawing.Point(59, 56);
            this._bMemoryCollect.Name = "_bMemoryCollect";
            this._bMemoryCollect.Size = new System.Drawing.Size(125, 29);
            this._bMemoryCollect.TabIndex = 2;
            this._bMemoryCollect.Text = "Memory collect";
            this._bMemoryCollect.UseVisualStyleBackColor = true;
            this._bMemoryCollect.Click += new System.EventHandler(this._bMemoryCollect_Click);
            // 
            // _bSoftReset
            // 
            this._bSoftReset.Location = new System.Drawing.Point(59, 20);
            this._bSoftReset.Margin = new System.Windows.Forms.Padding(4);
            this._bSoftReset.Name = "_bSoftReset";
            this._bSoftReset.Size = new System.Drawing.Size(125, 29);
            this._bSoftReset.TabIndex = 1;
            this._bSoftReset.Text = "Soft reset";
            this._bSoftReset.UseVisualStyleBackColor = true;
            this._bSoftReset.Click += new System.EventHandler(this._bSoftReset_Click);
            // 
            // _bHardReset
            // 
            this._bHardReset.Location = new System.Drawing.Point(192, 20);
            this._bHardReset.Margin = new System.Windows.Forms.Padding(4);
            this._bHardReset.Name = "_bHardReset";
            this._bHardReset.Size = new System.Drawing.Size(125, 29);
            this._bHardReset.TabIndex = 0;
            this._bHardReset.Text = "Hard reset";
            this._bHardReset.UseVisualStyleBackColor = true;
            this._bHardReset.Click += new System.EventHandler(this._bHardReset_Click);
            // 
            // _tpInputOutput
            // 
            this._tpInputOutput.BackColor = System.Drawing.Color.Transparent;
            this._tpInputOutput.Controls.Add(this._scInputsOutputs);
            this._tpInputOutput.Location = new System.Drawing.Point(4, 40);
            this._tpInputOutput.Margin = new System.Windows.Forms.Padding(4);
            this._tpInputOutput.Name = "_tpInputOutput";
            this._tpInputOutput.Padding = new System.Windows.Forms.Padding(4);
            this._tpInputOutput.Size = new System.Drawing.Size(1060, 530);
            this._tpInputOutput.TabIndex = 4;
            this._tpInputOutput.Text = "Inputs / Outputs";
            this._tpInputOutput.UseVisualStyleBackColor = true;
            this._tpInputOutput.Enter += new System.EventHandler(this._tpInputOutput_Enter);
            // 
            // _scInputsOutputs
            // 
            this._scInputsOutputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._scInputsOutputs.Location = new System.Drawing.Point(4, 4);
            this._scInputsOutputs.Margin = new System.Windows.Forms.Padding(4);
            this._scInputsOutputs.Name = "_scInputsOutputs";
            this._scInputsOutputs.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _scInputsOutputs.Panel1
            // 
            this._scInputsOutputs.Panel1.Controls.Add(this._gbInputsCCU);
            this._scInputsOutputs.Panel1MinSize = 150;
            // 
            // _scInputsOutputs.Panel2
            // 
            this._scInputsOutputs.Panel2.Controls.Add(this._gbOutputsCCU);
            this._scInputsOutputs.Panel2MinSize = 150;
            this._scInputsOutputs.Size = new System.Drawing.Size(1052, 522);
            this._scInputsOutputs.SplitterDistance = 253;
            this._scInputsOutputs.SplitterWidth = 5;
            this._scInputsOutputs.TabIndex = 4;
            // 
            // _gbInputsCCU
            // 
            this._gbInputsCCU.BackColor = System.Drawing.SystemColors.Control;
            this._gbInputsCCU.Controls.Add(this._bPrecreateInput);
            this._gbInputsCCU.Controls.Add(this._dgvInputCCU);
            this._gbInputsCCU.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gbInputsCCU.Location = new System.Drawing.Point(0, 0);
            this._gbInputsCCU.Margin = new System.Windows.Forms.Padding(4);
            this._gbInputsCCU.Name = "_gbInputsCCU";
            this._gbInputsCCU.Padding = new System.Windows.Forms.Padding(4);
            this._gbInputsCCU.Size = new System.Drawing.Size(1052, 253);
            this._gbInputsCCU.TabIndex = 2;
            this._gbInputsCCU.TabStop = false;
            this._gbInputsCCU.Text = "Inputs CCU";
            // 
            // _bPrecreateInput
            // 
            this._bPrecreateInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bPrecreateInput.AutoSize = true;
            this._bPrecreateInput.Location = new System.Drawing.Point(904, 213);
            this._bPrecreateInput.Margin = new System.Windows.Forms.Padding(4);
            this._bPrecreateInput.Name = "_bPrecreateInput";
            this._bPrecreateInput.Size = new System.Drawing.Size(144, 34);
            this._bPrecreateInput.TabIndex = 1;
            this._bPrecreateInput.Text = "Precreate input";
            this._bPrecreateInput.UseVisualStyleBackColor = true;
            this._bPrecreateInput.Click += new System.EventHandler(this._bPrecreateInput_Click);
            // 
            // _dgvInputCCU
            // 
            this._dgvInputCCU.AllowUserToAddRows = false;
            this._dgvInputCCU.AllowUserToDeleteRows = false;
            this._dgvInputCCU.AllowUserToResizeRows = false;
            this._dgvInputCCU.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvInputCCU.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvInputCCU.Location = new System.Drawing.Point(4, 24);
            this._dgvInputCCU.Margin = new System.Windows.Forms.Padding(4);
            this._dgvInputCCU.MultiSelect = false;
            this._dgvInputCCU.Name = "_dgvInputCCU";
            this._dgvInputCCU.ReadOnly = true;
            this._dgvInputCCU.RowHeadersVisible = false;
            this._dgvInputCCU.RowTemplate.Height = 24;
            this._dgvInputCCU.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvInputCCU.Size = new System.Drawing.Size(1044, 185);
            this._dgvInputCCU.TabIndex = 0;
            this._dgvInputCCU.TabStop = false;
            this._dgvInputCCU.DoubleClick += new System.EventHandler(this._dgvInputCCU_DoubleClick);
            // 
            // _gbOutputsCCU
            // 
            this._gbOutputsCCU.Controls.Add(this._bPrecreateOutput);
            this._gbOutputsCCU.Controls.Add(this._dgvOutputCCU);
            this._gbOutputsCCU.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gbOutputsCCU.Location = new System.Drawing.Point(0, 0);
            this._gbOutputsCCU.Margin = new System.Windows.Forms.Padding(4);
            this._gbOutputsCCU.Name = "_gbOutputsCCU";
            this._gbOutputsCCU.Padding = new System.Windows.Forms.Padding(4);
            this._gbOutputsCCU.Size = new System.Drawing.Size(1052, 264);
            this._gbOutputsCCU.TabIndex = 3;
            this._gbOutputsCCU.TabStop = false;
            this._gbOutputsCCU.Text = "Outputs CCU";
            // 
            // _bPrecreateOutput
            // 
            this._bPrecreateOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bPrecreateOutput.AutoSize = true;
            this._bPrecreateOutput.Location = new System.Drawing.Point(893, 223);
            this._bPrecreateOutput.Margin = new System.Windows.Forms.Padding(4);
            this._bPrecreateOutput.Name = "_bPrecreateOutput";
            this._bPrecreateOutput.Size = new System.Drawing.Size(155, 34);
            this._bPrecreateOutput.TabIndex = 1;
            this._bPrecreateOutput.Text = "Precreate output";
            this._bPrecreateOutput.UseVisualStyleBackColor = true;
            this._bPrecreateOutput.Click += new System.EventHandler(this._bPrecreateOutput_Click);
            // 
            // _dgvOutputCCU
            // 
            this._dgvOutputCCU.AllowUserToAddRows = false;
            this._dgvOutputCCU.AllowUserToDeleteRows = false;
            this._dgvOutputCCU.AllowUserToResizeRows = false;
            this._dgvOutputCCU.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvOutputCCU.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvOutputCCU.Location = new System.Drawing.Point(4, 24);
            this._dgvOutputCCU.Margin = new System.Windows.Forms.Padding(4);
            this._dgvOutputCCU.MultiSelect = false;
            this._dgvOutputCCU.Name = "_dgvOutputCCU";
            this._dgvOutputCCU.ReadOnly = true;
            this._dgvOutputCCU.RowHeadersVisible = false;
            this._dgvOutputCCU.RowTemplate.Height = 24;
            this._dgvOutputCCU.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvOutputCCU.Size = new System.Drawing.Size(1044, 195);
            this._dgvOutputCCU.TabIndex = 0;
            this._dgvOutputCCU.TabStop = false;
            this._dgvOutputCCU.DoubleClick += new System.EventHandler(this._dgvOutputCCU_DoubleClick);
            // 
            // _tpLevelBSI
            // 
            this._tpLevelBSI.BackColor = System.Drawing.SystemColors.Control;
            this._tpLevelBSI.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this._tpLevelBSI.Controls.Add(this._gbScheme);
            this._tpLevelBSI.Controls.Add(this._gbBsiLevelInfo);
            this._tpLevelBSI.Controls.Add(this._gbBsiLevelSet);
            this._tpLevelBSI.Location = new System.Drawing.Point(4, 40);
            this._tpLevelBSI.Margin = new System.Windows.Forms.Padding(4);
            this._tpLevelBSI.Name = "_tpLevelBSI";
            this._tpLevelBSI.Padding = new System.Windows.Forms.Padding(4);
            this._tpLevelBSI.Size = new System.Drawing.Size(1060, 530);
            this._tpLevelBSI.TabIndex = 5;
            this._tpLevelBSI.Text = "Level BSI";
            // 
            // _gbScheme
            // 
            this._gbScheme.Controls.Add(this.panel1);
            this._gbScheme.Location = new System.Drawing.Point(470, 8);
            this._gbScheme.Name = "_gbScheme";
            this._gbScheme.Size = new System.Drawing.Size(373, 350);
            this._gbScheme.TabIndex = 3;
            this._gbScheme.TabStop = false;
            this._gbScheme.Text = "groupBox1";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this._bRecalculate);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this._lRefU);
            this.panel1.Controls.Add(this._tbUref);
            this.panel1.Controls.Add(this._lTamperStateNormallyClosed);
            this.panel1.Controls.Add(this._lk2);
            this.panel1.Controls.Add(this._lR2);
            this.panel1.Controls.Add(this._lk);
            this.panel1.Controls.Add(this._lR1);
            this.panel1.Controls.Add(this._tbR2);
            this.panel1.Controls.Add(this._tbR1);
            this.panel1.Controls.Add(this._lAlarmStateNormallyClosed);
            this.panel1.Location = new System.Drawing.Point(5, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 327);
            this.panel1.TabIndex = 2;
            // 
            // _bRecalculate
            // 
            this._bRecalculate.Location = new System.Drawing.Point(200, 292);
            this._bRecalculate.Name = "_bRecalculate";
            this._bRecalculate.Size = new System.Drawing.Size(75, 23);
            this._bRecalculate.TabIndex = 11;
            this._bRecalculate.Text = "Recalculate";
            this._bRecalculate.UseVisualStyleBackColor = true;
            this._bRecalculate.Click += new System.EventHandler(this._bRecalculate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(140, 298);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "V";
            // 
            // _lRefU
            // 
            this._lRefU.AutoSize = true;
            this._lRefU.Location = new System.Drawing.Point(24, 298);
            this._lRefU.Name = "_lRefU";
            this._lRefU.Size = new System.Drawing.Size(33, 13);
            this._lRefU.TabIndex = 9;
            this._lRefU.Text = "ref. U";
            // 
            // _tbUref
            // 
            this._tbUref.Location = new System.Drawing.Point(60, 295);
            this._tbUref.Name = "_tbUref";
            this._tbUref.ReadOnly = true;
            this._tbUref.Size = new System.Drawing.Size(74, 20);
            this._tbUref.TabIndex = 8;
            this._tbUref.Text = "20";
            // 
            // _lTamperStateNormallyClosed
            // 
            this._lTamperStateNormallyClosed.AutoSize = true;
            this._lTamperStateNormallyClosed.Location = new System.Drawing.Point(197, 216);
            this._lTamperStateNormallyClosed.Name = "_lTamperStateNormallyClosed";
            this._lTamperStateNormallyClosed.Size = new System.Drawing.Size(146, 13);
            this._lTamperStateNormallyClosed.TabIndex = 7;
            this._lTamperStateNormallyClosed.Text = "Tamper state Normally closed";
            // 
            // _lk2
            // 
            this._lk2.AutoSize = true;
            this._lk2.Location = new System.Drawing.Point(285, 164);
            this._lk2.Name = "_lk2";
            this._lk2.Size = new System.Drawing.Size(21, 13);
            this._lk2.TabIndex = 6;
            this._lk2.Text = "kΩ";
            // 
            // _lR2
            // 
            this._lR2.AutoSize = true;
            this._lR2.Location = new System.Drawing.Point(178, 164);
            this._lR2.Name = "_lR2";
            this._lR2.Size = new System.Drawing.Size(21, 13);
            this._lR2.TabIndex = 5;
            this._lR2.Text = "R2";
            // 
            // _lk
            // 
            this._lk.AutoSize = true;
            this._lk.Location = new System.Drawing.Point(140, 127);
            this._lk.Name = "_lk";
            this._lk.Size = new System.Drawing.Size(21, 13);
            this._lk.TabIndex = 4;
            this._lk.Text = "kΩ";
            // 
            // _lR1
            // 
            this._lR1.AutoSize = true;
            this._lR1.Location = new System.Drawing.Point(33, 127);
            this._lR1.Name = "_lR1";
            this._lR1.Size = new System.Drawing.Size(21, 13);
            this._lR1.TabIndex = 3;
            this._lR1.Text = "R1";
            // 
            // _tbR2
            // 
            this._tbR2.Location = new System.Drawing.Point(205, 161);
            this._tbR2.Name = "_tbR2";
            this._tbR2.Size = new System.Drawing.Size(74, 20);
            this._tbR2.TabIndex = 2;
            this._tbR2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxResistors_KeyPress);
            // 
            // _tbR1
            // 
            this._tbR1.Location = new System.Drawing.Point(60, 124);
            this._tbR1.Name = "_tbR1";
            this._tbR1.Size = new System.Drawing.Size(74, 20);
            this._tbR1.TabIndex = 1;
            this._tbR1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxResistors_KeyPress);
            // 
            // _lAlarmStateNormallyClosed
            // 
            this._lAlarmStateNormallyClosed.AutoSize = true;
            this._lAlarmStateNormallyClosed.Location = new System.Drawing.Point(117, 2);
            this._lAlarmStateNormallyClosed.Name = "_lAlarmStateNormallyClosed";
            this._lAlarmStateNormallyClosed.Size = new System.Drawing.Size(136, 26);
            this._lAlarmStateNormallyClosed.TabIndex = 0;
            this._lAlarmStateNormallyClosed.Text = "\r\nAlarm state Normally closed";
            // 
            // _gbBsiLevelInfo
            // 
            this._gbBsiLevelInfo.Controls.Add(this._lValueBreak);
            this._gbBsiLevelInfo.Controls.Add(this._lValueAlarm);
            this._gbBsiLevelInfo.Controls.Add(this._lInfoShort);
            this._gbBsiLevelInfo.Controls.Add(this._bSet);
            this._gbBsiLevelInfo.Controls.Add(this._lInfoNormal);
            this._gbBsiLevelInfo.Controls.Add(this._lValueNormal);
            this._gbBsiLevelInfo.Controls.Add(this._lValueShort);
            this._gbBsiLevelInfo.Controls.Add(this._cbTemplateBsiLevel);
            this._gbBsiLevelInfo.Controls.Add(this._lInfoBreak);
            this._gbBsiLevelInfo.Controls.Add(this._lInfoAlarm);
            this._gbBsiLevelInfo.Location = new System.Drawing.Point(141, 8);
            this._gbBsiLevelInfo.Margin = new System.Windows.Forms.Padding(4);
            this._gbBsiLevelInfo.Name = "_gbBsiLevelInfo";
            this._gbBsiLevelInfo.Padding = new System.Windows.Forms.Padding(4);
            this._gbBsiLevelInfo.Size = new System.Drawing.Size(311, 269);
            this._gbBsiLevelInfo.TabIndex = 0;
            this._gbBsiLevelInfo.TabStop = false;
            this._gbBsiLevelInfo.Text = "BSI level info";
            // 
            // _lValueBreak
            // 
            this._lValueBreak.AutoSize = true;
            this._lValueBreak.Location = new System.Drawing.Point(80, 138);
            this._lValueBreak.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lValueBreak.Name = "_lValueBreak";
            this._lValueBreak.Size = new System.Drawing.Size(65, 13);
            this._lValueBreak.TabIndex = 9;
            this._lValueBreak.Text = "Value Break";
            // 
            // _lValueAlarm
            // 
            this._lValueAlarm.AutoSize = true;
            this._lValueAlarm.Location = new System.Drawing.Point(80, 112);
            this._lValueAlarm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lValueAlarm.Name = "_lValueAlarm";
            this._lValueAlarm.Size = new System.Drawing.Size(63, 13);
            this._lValueAlarm.TabIndex = 7;
            this._lValueAlarm.Text = "Value Alarm";
            // 
            // _lInfoShort
            // 
            this._lInfoShort.AutoSize = true;
            this._lInfoShort.Location = new System.Drawing.Point(8, 62);
            this._lInfoShort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lInfoShort.Name = "_lInfoShort";
            this._lInfoShort.Size = new System.Drawing.Size(32, 13);
            this._lInfoShort.TabIndex = 2;
            this._lInfoShort.Text = "Short";
            // 
            // _bSet
            // 
            this._bSet.Location = new System.Drawing.Point(204, 21);
            this._bSet.Margin = new System.Windows.Forms.Padding(4);
            this._bSet.Name = "_bSet";
            this._bSet.Size = new System.Drawing.Size(94, 29);
            this._bSet.TabIndex = 1;
            this._bSet.Text = "8";
            this._bSet.UseVisualStyleBackColor = true;
            this._bSet.Click += new System.EventHandler(this._bSet_Click);
            // 
            // _lInfoNormal
            // 
            this._lInfoNormal.AutoSize = true;
            this._lInfoNormal.Location = new System.Drawing.Point(8, 88);
            this._lInfoNormal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lInfoNormal.Name = "_lInfoNormal";
            this._lInfoNormal.Size = new System.Drawing.Size(40, 13);
            this._lInfoNormal.TabIndex = 4;
            this._lInfoNormal.Text = "Normal";
            // 
            // _lValueNormal
            // 
            this._lValueNormal.AutoSize = true;
            this._lValueNormal.Location = new System.Drawing.Point(80, 88);
            this._lValueNormal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lValueNormal.Name = "_lValueNormal";
            this._lValueNormal.Size = new System.Drawing.Size(70, 13);
            this._lValueNormal.TabIndex = 5;
            this._lValueNormal.Text = "Value Normal";
            // 
            // _lValueShort
            // 
            this._lValueShort.AutoSize = true;
            this._lValueShort.Location = new System.Drawing.Point(80, 61);
            this._lValueShort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lValueShort.Name = "_lValueShort";
            this._lValueShort.Size = new System.Drawing.Size(59, 13);
            this._lValueShort.TabIndex = 3;
            this._lValueShort.Text = "ValueShort";
            // 
            // _cbTemplateBsiLevel
            // 
            this._cbTemplateBsiLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbTemplateBsiLevel.FormattingEnabled = true;
            this._cbTemplateBsiLevel.Location = new System.Drawing.Point(8, 24);
            this._cbTemplateBsiLevel.Margin = new System.Windows.Forms.Padding(4);
            this._cbTemplateBsiLevel.Name = "_cbTemplateBsiLevel";
            this._cbTemplateBsiLevel.Size = new System.Drawing.Size(188, 21);
            this._cbTemplateBsiLevel.TabIndex = 0;
            this._cbTemplateBsiLevel.SelectedIndexChanged += new System.EventHandler(this._cbTemplateBsiLevel_SelectedIndexChanged);
            // 
            // _lInfoBreak
            // 
            this._lInfoBreak.AutoSize = true;
            this._lInfoBreak.Location = new System.Drawing.Point(8, 138);
            this._lInfoBreak.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lInfoBreak.Name = "_lInfoBreak";
            this._lInfoBreak.Size = new System.Drawing.Size(35, 13);
            this._lInfoBreak.TabIndex = 8;
            this._lInfoBreak.Text = "Break";
            // 
            // _lInfoAlarm
            // 
            this._lInfoAlarm.AutoSize = true;
            this._lInfoAlarm.Location = new System.Drawing.Point(8, 112);
            this._lInfoAlarm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lInfoAlarm.Name = "_lInfoAlarm";
            this._lInfoAlarm.Size = new System.Drawing.Size(33, 13);
            this._lInfoAlarm.TabIndex = 6;
            this._lInfoAlarm.Text = "Alarm";
            // 
            // _gbBsiLevelSet
            // 
            this._gbBsiLevelSet.Controls.Add(this._nBreak);
            this._gbBsiLevelSet.Controls.Add(this._lBreak);
            this._gbBsiLevelSet.Controls.Add(this._nZeno);
            this._gbBsiLevelSet.Controls.Add(this._lAlarm);
            this._gbBsiLevelSet.Controls.Add(this._lShort);
            this._gbBsiLevelSet.Controls.Add(this._lNormal);
            this._gbBsiLevelSet.Controls.Add(this._nShortNormal);
            this._gbBsiLevelSet.Controls.Add(this._nNormalAlarm);
            this._gbBsiLevelSet.Controls.Add(this._nAlarmBreak);
            this._gbBsiLevelSet.Location = new System.Drawing.Point(8, 8);
            this._gbBsiLevelSet.Margin = new System.Windows.Forms.Padding(4);
            this._gbBsiLevelSet.Name = "_gbBsiLevelSet";
            this._gbBsiLevelSet.Padding = new System.Windows.Forms.Padding(4);
            this._gbBsiLevelSet.Size = new System.Drawing.Size(126, 269);
            this._gbBsiLevelSet.TabIndex = 1;
            this._gbBsiLevelSet.TabStop = false;
            this._gbBsiLevelSet.Text = "Settings";
            // 
            // _nBreak
            // 
            this._nBreak.Location = new System.Drawing.Point(8, 221);
            this._nBreak.Margin = new System.Windows.Forms.Padding(4);
            this._nBreak.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._nBreak.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._nBreak.Name = "_nBreak";
            this._nBreak.Size = new System.Drawing.Size(92, 20);
            this._nBreak.TabIndex = 8;
            this._nBreak.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // _lBreak
            // 
            this._lBreak.AutoSize = true;
            this._lBreak.Location = new System.Drawing.Point(8, 201);
            this._lBreak.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lBreak.Name = "_lBreak";
            this._lBreak.Size = new System.Drawing.Size(35, 13);
            this._lBreak.TabIndex = 7;
            this._lBreak.Text = "Break";
            // 
            // _nZeno
            // 
            this._nZeno.Location = new System.Drawing.Point(8, 26);
            this._nZeno.Margin = new System.Windows.Forms.Padding(4);
            this._nZeno.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this._nZeno.Name = "_nZeno";
            this._nZeno.Size = new System.Drawing.Size(92, 20);
            this._nZeno.TabIndex = 0;
            // 
            // _lAlarm
            // 
            this._lAlarm.AutoSize = true;
            this._lAlarm.Location = new System.Drawing.Point(4, 152);
            this._lAlarm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAlarm.Name = "_lAlarm";
            this._lAlarm.Size = new System.Drawing.Size(33, 13);
            this._lAlarm.TabIndex = 5;
            this._lAlarm.Text = "Alarm";
            // 
            // _lShort
            // 
            this._lShort.AutoSize = true;
            this._lShort.Location = new System.Drawing.Point(4, 55);
            this._lShort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lShort.Name = "_lShort";
            this._lShort.Size = new System.Drawing.Size(35, 13);
            this._lShort.TabIndex = 1;
            this._lShort.Text = "Short ";
            // 
            // _lNormal
            // 
            this._lNormal.AutoSize = true;
            this._lNormal.Location = new System.Drawing.Point(4, 104);
            this._lNormal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lNormal.Name = "_lNormal";
            this._lNormal.Size = new System.Drawing.Size(43, 13);
            this._lNormal.TabIndex = 3;
            this._lNormal.Text = "Normal ";
            // 
            // _nShortNormal
            // 
            this._nShortNormal.DecimalPlaces = 1;
            this._nShortNormal.Location = new System.Drawing.Point(8, 75);
            this._nShortNormal.Margin = new System.Windows.Forms.Padding(4);
            this._nShortNormal.Name = "_nShortNormal";
            this._nShortNormal.Size = new System.Drawing.Size(92, 20);
            this._nShortNormal.TabIndex = 2;
            this._nShortNormal.ValueChanged += new System.EventHandler(this._nShortNormal_ValueChanged);
            // 
            // _nNormalAlarm
            // 
            this._nNormalAlarm.DecimalPlaces = 1;
            this._nNormalAlarm.Location = new System.Drawing.Point(8, 124);
            this._nNormalAlarm.Margin = new System.Windows.Forms.Padding(4);
            this._nNormalAlarm.Name = "_nNormalAlarm";
            this._nNormalAlarm.Size = new System.Drawing.Size(92, 20);
            this._nNormalAlarm.TabIndex = 4;
            this._nNormalAlarm.ValueChanged += new System.EventHandler(this._nNormalAlarm_ValueChanged);
            // 
            // _nAlarmBreak
            // 
            this._nAlarmBreak.DecimalPlaces = 1;
            this._nAlarmBreak.Location = new System.Drawing.Point(8, 172);
            this._nAlarmBreak.Margin = new System.Windows.Forms.Padding(4);
            this._nAlarmBreak.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._nAlarmBreak.Name = "_nAlarmBreak";
            this._nAlarmBreak.Size = new System.Drawing.Size(92, 20);
            this._nAlarmBreak.TabIndex = 6;
            this._nAlarmBreak.ValueChanged += new System.EventHandler(this._nAlarmBreak_ValueChanged);
            // 
            // _tpDoorEnvironments
            // 
            this._tpDoorEnvironments.BackColor = System.Drawing.SystemColors.Control;
            this._tpDoorEnvironments.Controls.Add(this._dgvDoorEnvironments);
            this._tpDoorEnvironments.Location = new System.Drawing.Point(4, 40);
            this._tpDoorEnvironments.Margin = new System.Windows.Forms.Padding(4);
            this._tpDoorEnvironments.Name = "_tpDoorEnvironments";
            this._tpDoorEnvironments.Padding = new System.Windows.Forms.Padding(4);
            this._tpDoorEnvironments.Size = new System.Drawing.Size(1060, 530);
            this._tpDoorEnvironments.TabIndex = 7;
            this._tpDoorEnvironments.Text = "Door Environments";
            this._tpDoorEnvironments.Enter += new System.EventHandler(this.TpDoorEnvironmentEnter);
            // 
            // _dgvDoorEnvironments
            // 
            this._dgvDoorEnvironments.AllowUserToAddRows = false;
            this._dgvDoorEnvironments.AllowUserToDeleteRows = false;
            this._dgvDoorEnvironments.AllowUserToResizeRows = false;
            this._dgvDoorEnvironments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvDoorEnvironments.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvDoorEnvironments.Location = new System.Drawing.Point(4, 4);
            this._dgvDoorEnvironments.Margin = new System.Windows.Forms.Padding(4);
            this._dgvDoorEnvironments.MultiSelect = false;
            this._dgvDoorEnvironments.Name = "_dgvDoorEnvironments";
            this._dgvDoorEnvironments.ReadOnly = true;
            this._dgvDoorEnvironments.RowHeadersVisible = false;
            this._dgvDoorEnvironments.RowTemplate.Height = 24;
            this._dgvDoorEnvironments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvDoorEnvironments.Size = new System.Drawing.Size(1052, 522);
            this._dgvDoorEnvironments.TabIndex = 1;
            this._dgvDoorEnvironments.TabStop = false;
            this._dgvDoorEnvironments.DoubleClick += new System.EventHandler(this.DgvDoorEnvironmentsDoubleClick);
            // 
            // _tpDCUsUpgrade
            // 
            this._tpDCUsUpgrade.BackColor = System.Drawing.SystemColors.Control;
            this._tpDCUsUpgrade.Controls.Add(this._dcuUpgrade);
            this._tpDCUsUpgrade.Controls.Add(this._chbSelectAll);
            this._tpDCUsUpgrade.Controls.Add(this._lDCUsToUpgrade);
            this._tpDCUsUpgrade.Controls.Add(this._lAvailableDCUUpgrades);
            this._tpDCUsUpgrade.Controls.Add(this._bRefreshDCUs);
            this._tpDCUsUpgrade.Controls.Add(this._dgvDCUUpgrading);
            this._tpDCUsUpgrade.Location = new System.Drawing.Point(4, 40);
            this._tpDCUsUpgrade.Margin = new System.Windows.Forms.Padding(4);
            this._tpDCUsUpgrade.Name = "_tpDCUsUpgrade";
            this._tpDCUsUpgrade.Size = new System.Drawing.Size(1060, 530);
            this._tpDCUsUpgrade.TabIndex = 13;
            this._tpDCUsUpgrade.Text = "DCU upgrade";
            // 
            // _dcuUpgrade
            // 
            this._dcuUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._dcuUpgrade.Location = new System.Drawing.Point(19, 58);
            this._dcuUpgrade.Name = "_dcuUpgrade";
            this._dcuUpgrade.Size = new System.Drawing.Size(303, 431);
            this._dcuUpgrade.TabIndex = 26;
            this._dcuUpgrade.Text = "_dcuUpgrade";
            this._dcuUpgrade.Child = this._dcuUpgradeFiles;
            // 
            // _chbSelectAll
            // 
            this._chbSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._chbSelectAll.AutoSize = true;
            this._chbSelectAll.Location = new System.Drawing.Point(430, 506);
            this._chbSelectAll.Margin = new System.Windows.Forms.Padding(4);
            this._chbSelectAll.Name = "_chbSelectAll";
            this._chbSelectAll.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._chbSelectAll.Size = new System.Drawing.Size(159, 17);
            this._chbSelectAll.TabIndex = 7;
            this._chbSelectAll.Text = "Select/unselect all available";
            this._chbSelectAll.UseVisualStyleBackColor = true;
            this._chbSelectAll.CheckedChanged += new System.EventHandler(this._chbSelectAll_CheckedChanged);
            // 
            // _lDCUsToUpgrade
            // 
            this._lDCUsToUpgrade.AutoSize = true;
            this._lDCUsToUpgrade.Location = new System.Drawing.Point(329, 32);
            this._lDCUsToUpgrade.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDCUsToUpgrade.Name = "_lDCUsToUpgrade";
            this._lDCUsToUpgrade.Size = new System.Drawing.Size(87, 13);
            this._lDCUsToUpgrade.TabIndex = 6;
            this._lDCUsToUpgrade.Text = "DCU for upgrade";
            // 
            // _lAvailableDCUUpgrades
            // 
            this._lAvailableDCUUpgrades.AutoSize = true;
            this._lAvailableDCUUpgrades.Location = new System.Drawing.Point(15, 32);
            this._lAvailableDCUUpgrades.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAvailableDCUUpgrades.Name = "_lAvailableDCUUpgrades";
            this._lAvailableDCUUpgrades.Size = new System.Drawing.Size(92, 13);
            this._lAvailableDCUUpgrades.TabIndex = 3;
            this._lAvailableDCUUpgrades.Text = "Available versions";
            // 
            // _bRefreshDCUs
            // 
            this._bRefreshDCUs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRefreshDCUs.Location = new System.Drawing.Point(329, 497);
            this._bRefreshDCUs.Margin = new System.Windows.Forms.Padding(4);
            this._bRefreshDCUs.Name = "_bRefreshDCUs";
            this._bRefreshDCUs.Size = new System.Drawing.Size(94, 29);
            this._bRefreshDCUs.TabIndex = 1;
            this._bRefreshDCUs.Text = "Refresh";
            this._bRefreshDCUs.UseVisualStyleBackColor = true;
            this._bRefreshDCUs.Click += new System.EventHandler(this._bRefreshDCUs_Click);
            // 
            // _dgvDCUUpgrading
            // 
            this._dgvDCUUpgrading.AllowUserToAddRows = false;
            this._dgvDCUUpgrading.AllowUserToDeleteRows = false;
            this._dgvDCUUpgrading.AllowUserToResizeRows = false;
            this._dgvDCUUpgrading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvDCUUpgrading.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvDCUUpgrading.Location = new System.Drawing.Point(329, 58);
            this._dgvDCUUpgrading.Margin = new System.Windows.Forms.Padding(4);
            this._dgvDCUUpgrading.MultiSelect = false;
            this._dgvDCUUpgrading.Name = "_dgvDCUUpgrading";
            this._dgvDCUUpgrading.RowHeadersVisible = false;
            this._dgvDCUUpgrading.RowTemplate.Height = 24;
            this._dgvDCUUpgrading.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvDCUUpgrading.ShowEditingIcon = false;
            this._dgvDCUUpgrading.Size = new System.Drawing.Size(725, 431);
            this._dgvDCUUpgrading.TabIndex = 0;
            this._dgvDCUUpgrading.TabStop = false;
            this._dgvDCUUpgrading.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgvDCUUpgrading_CellDoubleClick);
            this._dgvDCUUpgrading.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvDCUUpgradingCellClick);
            // 
            // _tpCRUpgrade
            // 
            this._tpCRUpgrade.BackColor = System.Drawing.SystemColors.Control;
            this._tpCRUpgrade.Controls.Add(this._crUpgrade);
            this._tpCRUpgrade.Controls.Add(this._bUpgradeCRRefresh);
            this._tpCRUpgrade.Controls.Add(this._chbCRSelectAll);
            this._tpCRUpgrade.Controls.Add(this._dgvCRUpgrading);
            this._tpCRUpgrade.Controls.Add(this._lAvailableCRUpradeVersions);
            this._tpCRUpgrade.Location = new System.Drawing.Point(4, 40);
            this._tpCRUpgrade.Margin = new System.Windows.Forms.Padding(4);
            this._tpCRUpgrade.Name = "_tpCRUpgrade";
            this._tpCRUpgrade.Size = new System.Drawing.Size(1060, 530);
            this._tpCRUpgrade.TabIndex = 15;
            this._tpCRUpgrade.Text = "CR upgrade";
            // 
            // _crUpgrade
            // 
            this._crUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this._crUpgrade.Location = new System.Drawing.Point(14, 28);
            this._crUpgrade.Name = "_crUpgrade";
            this._crUpgrade.Size = new System.Drawing.Size(314, 442);
            this._crUpgrade.TabIndex = 26;
            this._crUpgrade.Text = "elementHost1";
            this._crUpgrade.Child = this._crUpgradeFiles;
            // 
            // _bUpgradeCRRefresh
            // 
            this._bUpgradeCRRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bUpgradeCRRefresh.Location = new System.Drawing.Point(960, 477);
            this._bUpgradeCRRefresh.Margin = new System.Windows.Forms.Padding(4);
            this._bUpgradeCRRefresh.Name = "_bUpgradeCRRefresh";
            this._bUpgradeCRRefresh.Size = new System.Drawing.Size(94, 29);
            this._bUpgradeCRRefresh.TabIndex = 2;
            this._bUpgradeCRRefresh.Text = "Refresh";
            this._bUpgradeCRRefresh.UseVisualStyleBackColor = true;
            this._bUpgradeCRRefresh.Click += new System.EventHandler(this._bUpgradeCRRefresh_Click);
            // 
            // _chbCRSelectAll
            // 
            this._chbCRSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._chbCRSelectAll.AutoSize = true;
            this._chbCRSelectAll.Location = new System.Drawing.Point(335, 489);
            this._chbCRSelectAll.Margin = new System.Windows.Forms.Padding(4);
            this._chbCRSelectAll.Name = "_chbCRSelectAll";
            this._chbCRSelectAll.Size = new System.Drawing.Size(159, 17);
            this._chbCRSelectAll.TabIndex = 1;
            this._chbCRSelectAll.Text = "Select/unselect all available";
            this._chbCRSelectAll.UseVisualStyleBackColor = true;
            this._chbCRSelectAll.CheckedChanged += new System.EventHandler(this._chbCRSelectAll_CheckedChanged);
            // 
            // _dgvCRUpgrading
            // 
            this._dgvCRUpgrading.AllowUserToAddRows = false;
            this._dgvCRUpgrading.AllowUserToDeleteRows = false;
            this._dgvCRUpgrading.AllowUserToResizeRows = false;
            this._dgvCRUpgrading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvCRUpgrading.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvCRUpgrading.Location = new System.Drawing.Point(335, 28);
            this._dgvCRUpgrading.Margin = new System.Windows.Forms.Padding(4);
            this._dgvCRUpgrading.MultiSelect = false;
            this._dgvCRUpgrading.Name = "_dgvCRUpgrading";
            this._dgvCRUpgrading.RowHeadersVisible = false;
            this._dgvCRUpgrading.RowTemplate.Height = 24;
            this._dgvCRUpgrading.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvCRUpgrading.ShowEditingIcon = false;
            this._dgvCRUpgrading.Size = new System.Drawing.Size(719, 442);
            this._dgvCRUpgrading.TabIndex = 0;
            this._dgvCRUpgrading.TabStop = false;
            this._dgvCRUpgrading.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgvCRUpgrading_CellDoubleClick);
            this._dgvCRUpgrading.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgvCRUpgrading_CellClick);
            // 
            // _lAvailableCRUpradeVersions
            // 
            this._lAvailableCRUpradeVersions.AutoSize = true;
            this._lAvailableCRUpradeVersions.Location = new System.Drawing.Point(11, 8);
            this._lAvailableCRUpradeVersions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAvailableCRUpradeVersions.Name = "_lAvailableCRUpradeVersions";
            this._lAvailableCRUpradeVersions.Size = new System.Drawing.Size(92, 13);
            this._lAvailableCRUpradeVersions.TabIndex = 5;
            this._lAvailableCRUpradeVersions.Text = "Available versions";
            // 
            // _tpPortSettings
            // 
            this._tpPortSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpPortSettings.Controls.Add(this._gbComPortSettings);
            this._tpPortSettings.Location = new System.Drawing.Point(4, 40);
            this._tpPortSettings.Margin = new System.Windows.Forms.Padding(4);
            this._tpPortSettings.Name = "_tpPortSettings";
            this._tpPortSettings.Padding = new System.Windows.Forms.Padding(4);
            this._tpPortSettings.Size = new System.Drawing.Size(1060, 530);
            this._tpPortSettings.TabIndex = 8;
            this._tpPortSettings.Text = "Port settings";
            // 
            // _gbComPortSettings
            // 
            this._gbComPortSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbComPortSettings.Controls.Add(this._chbEnabledComPort);
            this._gbComPortSettings.Controls.Add(this._cbPortComBaudRate);
            this._gbComPortSettings.Controls.Add(this._lComPortBaudRate);
            this._gbComPortSettings.Location = new System.Drawing.Point(11, 8);
            this._gbComPortSettings.Margin = new System.Windows.Forms.Padding(4);
            this._gbComPortSettings.Name = "_gbComPortSettings";
            this._gbComPortSettings.Padding = new System.Windows.Forms.Padding(4);
            this._gbComPortSettings.Size = new System.Drawing.Size(1039, 110);
            this._gbComPortSettings.TabIndex = 0;
            this._gbComPortSettings.TabStop = false;
            this._gbComPortSettings.Text = "COM port settings";
            // 
            // _chbEnabledComPort
            // 
            this._chbEnabledComPort.AutoSize = true;
            this._chbEnabledComPort.Location = new System.Drawing.Point(8, 24);
            this._chbEnabledComPort.Margin = new System.Windows.Forms.Padding(4);
            this._chbEnabledComPort.Name = "_chbEnabledComPort";
            this._chbEnabledComPort.Size = new System.Drawing.Size(113, 17);
            this._chbEnabledComPort.TabIndex = 0;
            this._chbEnabledComPort.TabStop = false;
            this._chbEnabledComPort.Text = "Enabled COM port";
            this._chbEnabledComPort.UseVisualStyleBackColor = true;
            this._chbEnabledComPort.CheckedChanged += new System.EventHandler(this._chbEnabledComPort_CheckedChanged);
            // 
            // _cbPortComBaudRate
            // 
            this._cbPortComBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPortComBaudRate.FormattingEnabled = true;
            this._cbPortComBaudRate.Items.AddRange(new object[] {
            "4800",
            "9600",
            "19200"});
            this._cbPortComBaudRate.Location = new System.Drawing.Point(8, 69);
            this._cbPortComBaudRate.Margin = new System.Windows.Forms.Padding(4);
            this._cbPortComBaudRate.Name = "_cbPortComBaudRate";
            this._cbPortComBaudRate.Size = new System.Drawing.Size(233, 21);
            this._cbPortComBaudRate.TabIndex = 2;
            this._cbPortComBaudRate.SelectedIndexChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lComPortBaudRate
            // 
            this._lComPortBaudRate.AutoSize = true;
            this._lComPortBaudRate.Location = new System.Drawing.Point(8, 49);
            this._lComPortBaudRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lComPortBaudRate.Name = "_lComPortBaudRate";
            this._lComPortBaudRate.Size = new System.Drawing.Size(106, 13);
            this._lComPortBaudRate.TabIndex = 1;
            this._lComPortBaudRate.Text = "COM port Baud Rate";
            // 
            // _tpIPSettings
            // 
            this._tpIPSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpIPSettings.Controls.Add(this._gbStaticIPSettings);
            this._tpIPSettings.Controls.Add(this._rbStatic);
            this._tpIPSettings.Controls.Add(this._rbDHCP);
            this._tpIPSettings.Location = new System.Drawing.Point(4, 40);
            this._tpIPSettings.Margin = new System.Windows.Forms.Padding(4);
            this._tpIPSettings.Name = "_tpIPSettings";
            this._tpIPSettings.Padding = new System.Windows.Forms.Padding(4);
            this._tpIPSettings.Size = new System.Drawing.Size(1060, 530);
            this._tpIPSettings.TabIndex = 9;
            this._tpIPSettings.Text = "CCU IP settings";
            // 
            // _gbStaticIPSettings
            // 
            this._gbStaticIPSettings.Controls.Add(this._bTestIP);
            this._gbStaticIPSettings.Controls.Add(this._lResultOfApplyIPSettings);
            this._gbStaticIPSettings.Controls.Add(this._eResultOfApplyIPSettings);
            this._gbStaticIPSettings.Controls.Add(this._bApply1);
            this._gbStaticIPSettings.Controls.Add(this._eIPSettingsGateway);
            this._gbStaticIPSettings.Controls.Add(this._lGateway);
            this._gbStaticIPSettings.Controls.Add(this._eIPSettingsMask);
            this._gbStaticIPSettings.Controls.Add(this._lMask);
            this._gbStaticIPSettings.Controls.Add(this._eIPSettingsIPAddress);
            this._gbStaticIPSettings.Controls.Add(this._lIPAddress2);
            this._gbStaticIPSettings.Location = new System.Drawing.Point(8, 65);
            this._gbStaticIPSettings.Margin = new System.Windows.Forms.Padding(4);
            this._gbStaticIPSettings.Name = "_gbStaticIPSettings";
            this._gbStaticIPSettings.Padding = new System.Windows.Forms.Padding(4);
            this._gbStaticIPSettings.Size = new System.Drawing.Size(791, 341);
            this._gbStaticIPSettings.TabIndex = 2;
            this._gbStaticIPSettings.TabStop = false;
            this._gbStaticIPSettings.Text = "Static IP settings";
            // 
            // _bTestIP
            // 
            this._bTestIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bTestIP.Enabled = false;
            this._bTestIP.Location = new System.Drawing.Point(588, 176);
            this._bTestIP.Margin = new System.Windows.Forms.Padding(4);
            this._bTestIP.Name = "_bTestIP";
            this._bTestIP.Size = new System.Drawing.Size(94, 29);
            this._bTestIP.TabIndex = 9;
            this._bTestIP.Text = "Test";
            this._bTestIP.UseVisualStyleBackColor = true;
            this._bTestIP.Click += new System.EventHandler(this._bTestIP_Click);
            // 
            // _lResultOfApplyIPSettings
            // 
            this._lResultOfApplyIPSettings.AutoSize = true;
            this._lResultOfApplyIPSettings.Location = new System.Drawing.Point(8, 148);
            this._lResultOfApplyIPSettings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lResultOfApplyIPSettings.Name = "_lResultOfApplyIPSettings";
            this._lResultOfApplyIPSettings.Size = new System.Drawing.Size(129, 13);
            this._lResultOfApplyIPSettings.TabIndex = 6;
            this._lResultOfApplyIPSettings.Text = "Result of apply IP settings";
            // 
            // _eResultOfApplyIPSettings
            // 
            this._eResultOfApplyIPSettings.Location = new System.Drawing.Point(208, 144);
            this._eResultOfApplyIPSettings.Margin = new System.Windows.Forms.Padding(4);
            this._eResultOfApplyIPSettings.Name = "_eResultOfApplyIPSettings";
            this._eResultOfApplyIPSettings.ReadOnly = true;
            this._eResultOfApplyIPSettings.Size = new System.Drawing.Size(575, 20);
            this._eResultOfApplyIPSettings.TabIndex = 7;
            // 
            // _bApply1
            // 
            this._bApply1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply1.Location = new System.Drawing.Point(690, 176);
            this._bApply1.Margin = new System.Windows.Forms.Padding(4);
            this._bApply1.Name = "_bApply1";
            this._bApply1.Size = new System.Drawing.Size(94, 29);
            this._bApply1.TabIndex = 8;
            this._bApply1.Text = "Apply";
            this._bApply1.UseVisualStyleBackColor = true;
            this._bApply1.Click += new System.EventHandler(this._bApply1_Click);
            // 
            // _eIPSettingsGateway
            // 
            this._eIPSettingsGateway.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eIPSettingsGateway.Location = new System.Drawing.Point(165, 94);
            this._eIPSettingsGateway.Margin = new System.Windows.Forms.Padding(4);
            this._eIPSettingsGateway.Name = "_eIPSettingsGateway";
            this._eIPSettingsGateway.Size = new System.Drawing.Size(618, 20);
            this._eIPSettingsGateway.TabIndex = 5;
            this._eIPSettingsGateway.TextChanged += new System.EventHandler(this._eIPSettingsGateway_TextChanged);
            this._eIPSettingsGateway.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._eSNTPIpAddress_KeyPress);
            // 
            // _lGateway
            // 
            this._lGateway.AutoSize = true;
            this._lGateway.Location = new System.Drawing.Point(8, 98);
            this._lGateway.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lGateway.Name = "_lGateway";
            this._lGateway.Size = new System.Drawing.Size(49, 13);
            this._lGateway.TabIndex = 4;
            this._lGateway.Text = "Gateway";
            // 
            // _eIPSettingsMask
            // 
            this._eIPSettingsMask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eIPSettingsMask.Location = new System.Drawing.Point(165, 61);
            this._eIPSettingsMask.Margin = new System.Windows.Forms.Padding(4);
            this._eIPSettingsMask.Name = "_eIPSettingsMask";
            this._eIPSettingsMask.Size = new System.Drawing.Size(618, 20);
            this._eIPSettingsMask.TabIndex = 3;
            this._eIPSettingsMask.TextChanged += new System.EventHandler(this._eIPSettingsMask_TextChanged);
            this._eIPSettingsMask.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._eSNTPIpAddress_KeyPress);
            // 
            // _lMask
            // 
            this._lMask.AutoSize = true;
            this._lMask.Location = new System.Drawing.Point(8, 65);
            this._lMask.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMask.Name = "_lMask";
            this._lMask.Size = new System.Drawing.Size(69, 13);
            this._lMask.TabIndex = 2;
            this._lMask.Text = "Subnet mask";
            // 
            // _eIPSettingsIPAddress
            // 
            this._eIPSettingsIPAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eIPSettingsIPAddress.Location = new System.Drawing.Point(165, 29);
            this._eIPSettingsIPAddress.Margin = new System.Windows.Forms.Padding(4);
            this._eIPSettingsIPAddress.Name = "_eIPSettingsIPAddress";
            this._eIPSettingsIPAddress.Size = new System.Drawing.Size(618, 20);
            this._eIPSettingsIPAddress.TabIndex = 1;
            this._eIPSettingsIPAddress.TextChanged += new System.EventHandler(this._eIPSettingsIPAddress_TextChanged);
            this._eIPSettingsIPAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._eSNTPIpAddress_KeyPress);
            // 
            // _lIPAddress2
            // 
            this._lIPAddress2.AutoSize = true;
            this._lIPAddress2.Location = new System.Drawing.Point(8, 32);
            this._lIPAddress2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lIPAddress2.Name = "_lIPAddress2";
            this._lIPAddress2.Size = new System.Drawing.Size(58, 13);
            this._lIPAddress2.TabIndex = 0;
            this._lIPAddress2.Text = "IP Address";
            // 
            // _rbStatic
            // 
            this._rbStatic.AutoSize = true;
            this._rbStatic.Location = new System.Drawing.Point(8, 36);
            this._rbStatic.Margin = new System.Windows.Forms.Padding(4);
            this._rbStatic.Name = "_rbStatic";
            this._rbStatic.Size = new System.Drawing.Size(108, 17);
            this._rbStatic.TabIndex = 1;
            this._rbStatic.Text = "Static assignment";
            this._rbStatic.UseVisualStyleBackColor = true;
            this._rbStatic.CheckedChanged += new System.EventHandler(this._rbStatic_CheckedChanged);
            // 
            // _rbDHCP
            // 
            this._rbDHCP.AutoSize = true;
            this._rbDHCP.Checked = true;
            this._rbDHCP.Location = new System.Drawing.Point(8, 8);
            this._rbDHCP.Margin = new System.Windows.Forms.Padding(4);
            this._rbDHCP.Name = "_rbDHCP";
            this._rbDHCP.Size = new System.Drawing.Size(111, 17);
            this._rbDHCP.TabIndex = 0;
            this._rbDHCP.TabStop = true;
            this._rbDHCP.Text = "DHCP assignment";
            this._rbDHCP.UseVisualStyleBackColor = true;
            this._rbDHCP.CheckedChanged += new System.EventHandler(this._rbDHCP_CheckedChanged);
            // 
            // _tpAlarmSettings
            // 
            this._tpAlarmSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpAlarmSettings.Controls.Add(this._lAlarmTransmitter);
            this._tpAlarmSettings.Controls.Add(this._tbmAlarmTransmitter);
            this._tpAlarmSettings.Controls.Add(this._accordionAlarmSettings);
            this._tpAlarmSettings.Controls.Add(this._cbOpenAllAlarmSettings);
            this._tpAlarmSettings.Controls.Add(this._gbSettingsForEventlogOnCR);
            this._tpAlarmSettings.Controls.Add(this._gbServerWatchdog);
            this._tpAlarmSettings.Location = new System.Drawing.Point(4, 40);
            this._tpAlarmSettings.Margin = new System.Windows.Forms.Padding(4);
            this._tpAlarmSettings.Name = "_tpAlarmSettings";
            this._tpAlarmSettings.Padding = new System.Windows.Forms.Padding(4);
            this._tpAlarmSettings.Size = new System.Drawing.Size(1060, 530);
            this._tpAlarmSettings.TabIndex = 12;
            this._tpAlarmSettings.Text = "Alarm settings";
            // 
            // _lAlarmTransmitter
            // 
            this._lAlarmTransmitter.AutoSize = true;
            this._lAlarmTransmitter.Location = new System.Drawing.Point(8, 185);
            this._lAlarmTransmitter.Name = "_lAlarmTransmitter";
            this._lAlarmTransmitter.Size = new System.Drawing.Size(84, 13);
            this._lAlarmTransmitter.TabIndex = 18;
            this._lAlarmTransmitter.Text = "Alarm transmitter";
            // 
            // _tbmAlarmTransmitter
            // 
            this._tbmAlarmTransmitter.AllowDrop = true;
            this._tbmAlarmTransmitter.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmAlarmTransmitter.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmTransmitter.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmAlarmTransmitter.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmAlarmTransmitter.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmTransmitter.Button.Image")));
            this._tbmAlarmTransmitter.Button.Location = new System.Drawing.Point(439, 0);
            this._tbmAlarmTransmitter.Button.Name = "_bMenu";
            this._tbmAlarmTransmitter.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmAlarmTransmitter.Button.TabIndex = 3;
            this._tbmAlarmTransmitter.Button.UseVisualStyleBackColor = false;
            this._tbmAlarmTransmitter.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmAlarmTransmitter.ButtonDefaultBehaviour = true;
            this._tbmAlarmTransmitter.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmAlarmTransmitter.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmTransmitter.ButtonImage")));
            // 
            // 
            // 
            this._tbmAlarmTransmitter.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify24,
            this._tsiRemove24,
            this._tsiCreate24});
            this._tbmAlarmTransmitter.ButtonPopupMenu.Name = "";
            this._tbmAlarmTransmitter.ButtonPopupMenu.Size = new System.Drawing.Size(118, 70);
            this._tbmAlarmTransmitter.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmAlarmTransmitter.ButtonShowImage = true;
            this._tbmAlarmTransmitter.ButtonSizeHeight = 20;
            this._tbmAlarmTransmitter.ButtonSizeWidth = 20;
            this._tbmAlarmTransmitter.ButtonText = "";
            this._tbmAlarmTransmitter.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmTransmitter.HoverTime = 500;
            // 
            // 
            // 
            this._tbmAlarmTransmitter.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmTransmitter.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._tbmAlarmTransmitter.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmAlarmTransmitter.ImageTextBox.ContextMenuStrip = this._tbmAlarmTransmitter.ButtonPopupMenu;
            this._tbmAlarmTransmitter.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmTransmitter.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmTransmitter.ImageTextBox.Image")));
            this._tbmAlarmTransmitter.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmAlarmTransmitter.ImageTextBox.Name = "_itbTextBox";
            this._tbmAlarmTransmitter.ImageTextBox.NoTextNoImage = true;
            this._tbmAlarmTransmitter.ImageTextBox.ReadOnly = false;
            this._tbmAlarmTransmitter.ImageTextBox.Size = new System.Drawing.Size(439, 20);
            this._tbmAlarmTransmitter.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmAlarmTransmitter.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmTransmitter.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmAlarmTransmitter.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmAlarmTransmitter.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmTransmitter.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmAlarmTransmitter.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmAlarmTransmitter.ImageTextBox.TextBox.Size = new System.Drawing.Size(437, 13);
            this._tbmAlarmTransmitter.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmAlarmTransmitter.ImageTextBox.UseImage = true;
            this._tbmAlarmTransmitter.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmAlarmTransmitter_ImageTextBox_DoubleClick);
            this._tbmAlarmTransmitter.Location = new System.Drawing.Point(118, 183);
            this._tbmAlarmTransmitter.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmAlarmTransmitter.MinimumSize = new System.Drawing.Size(30, 20);
            this._tbmAlarmTransmitter.Name = "_tbmAlarmTransmitter";
            this._tbmAlarmTransmitter.Size = new System.Drawing.Size(459, 20);
            this._tbmAlarmTransmitter.TabIndex = 17;
            this._tbmAlarmTransmitter.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmTransmitter.TextImage")));
            this._tbmAlarmTransmitter.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmAlarmTransmitter_DragOver);
            this._tbmAlarmTransmitter.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmAlarmTransmitter_DragDrop);
            this._tbmAlarmTransmitter.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmAlarmTransmitter_ButtonPopupMenuItemClick);
            // 
            // _tsiModify24
            // 
            this._tsiModify24.Name = "_tsiModify24";
            this._tsiModify24.Size = new System.Drawing.Size(117, 22);
            this._tsiModify24.Text = "Modify";
            // 
            // _tsiRemove24
            // 
            this._tsiRemove24.Name = "_tsiRemove24";
            this._tsiRemove24.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove24.Text = "Remove";
            // 
            // _tsiCreate24
            // 
            this._tsiCreate24.Name = "_tsiCreate24";
            this._tsiCreate24.Size = new System.Drawing.Size(117, 22);
            this._tsiCreate24.Text = "Create";
            // 
            // _accordionAlarmSettings
            // 
            this._accordionAlarmSettings.AddResizeBars = true;
            this._accordionAlarmSettings.AllowMouseResize = true;
            this._accordionAlarmSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._accordionAlarmSettings.AnimateCloseEffect = Contal.IwQuick.PlatformPC.UI.Accordion.AnimateWindowFlags.Hide;
            this._accordionAlarmSettings.AnimateCloseMillis = 150;
            this._accordionAlarmSettings.AnimateOpenEffect = Contal.IwQuick.PlatformPC.UI.Accordion.AnimateWindowFlags.Show;
            this._accordionAlarmSettings.AnimateOpenMillis = 150;
            this._accordionAlarmSettings.AutoFixDockStyle = false;
            this._accordionAlarmSettings.AutoScroll = true;
            this._accordionAlarmSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._accordionAlarmSettings.CheckBoxFactory = null;
            this._accordionAlarmSettings.CheckBoxMargin = new System.Windows.Forms.Padding(0);
            this._accordionAlarmSettings.ContentBackColor = null;
            this._accordionAlarmSettings.ContentMargin = null;
            this._accordionAlarmSettings.ContentPadding = new System.Windows.Forms.Padding(5);
            this._accordionAlarmSettings.ControlBackColor = null;
            this._accordionAlarmSettings.ControlMinimumHeightIsItsPreferredHeight = false;
            this._accordionAlarmSettings.ControlMinimumWidthIsItsPreferredWidth = false;
            this._accordionAlarmSettings.DownArrow = null;
            this._accordionAlarmSettings.FillHeight = true;
            this._accordionAlarmSettings.FillLastOpened = false;
            this._accordionAlarmSettings.FillModeGrowOnly = false;
            this._accordionAlarmSettings.FillResetOnCollapse = false;
            this._accordionAlarmSettings.FillWidth = true;
            this._accordionAlarmSettings.GrabCursor = System.Windows.Forms.Cursors.SizeNS;
            this._accordionAlarmSettings.GrabRequiresPositiveFillWeight = true;
            this._accordionAlarmSettings.GrabWidth = 6;
            this._accordionAlarmSettings.GrowAndShrink = true;
            this._accordionAlarmSettings.Insets = new System.Windows.Forms.Padding(0);
            this._accordionAlarmSettings.Location = new System.Drawing.Point(3, 231);
            this._accordionAlarmSettings.Name = "_accordionAlarmSettings";
            this._accordionAlarmSettings.OpenOnAdd = false;
            this._accordionAlarmSettings.OpenOneOnly = false;
            this._accordionAlarmSettings.ResizeBarFactory = null;
            this._accordionAlarmSettings.ResizeBarsAlign = 0.5;
            this._accordionAlarmSettings.ResizeBarsArrowKeyDelta = 10;
            this._accordionAlarmSettings.ResizeBarsFadeInMillis = 800;
            this._accordionAlarmSettings.ResizeBarsFadeOutMillis = 800;
            this._accordionAlarmSettings.ResizeBarsFadeProximity = 24;
            this._accordionAlarmSettings.ResizeBarsFill = 1;
            this._accordionAlarmSettings.ResizeBarsKeepFocusAfterMouseDrag = false;
            this._accordionAlarmSettings.ResizeBarsKeepFocusIfControlOutOfView = true;
            this._accordionAlarmSettings.ResizeBarsKeepFocusOnClick = true;
            this._accordionAlarmSettings.ResizeBarsMargin = null;
            this._accordionAlarmSettings.ResizeBarsMinimumLength = 50;
            this._accordionAlarmSettings.ResizeBarsStayInViewOnArrowKey = true;
            this._accordionAlarmSettings.ResizeBarsStayInViewOnMouseDrag = true;
            this._accordionAlarmSettings.ResizeBarsStayVisibleIfFocused = true;
            this._accordionAlarmSettings.ResizeBarsTabStop = true;
            this._accordionAlarmSettings.ShowPartiallyVisibleResizeBars = false;
            this._accordionAlarmSettings.ShowToolMenu = true;
            this._accordionAlarmSettings.ShowToolMenuOnHoverWhenClosed = false;
            this._accordionAlarmSettings.ShowToolMenuOnRightClick = true;
            this._accordionAlarmSettings.ShowToolMenuRequiresPositiveFillWeight = false;
            this._accordionAlarmSettings.Size = new System.Drawing.Size(1054, 296);
            this._accordionAlarmSettings.TabIndex = 8;
            this._accordionAlarmSettings.UpArrow = null;
            // 
            // _cbOpenAllAlarmSettings
            // 
            this._cbOpenAllAlarmSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cbOpenAllAlarmSettings.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._cbOpenAllAlarmSettings.Location = new System.Drawing.Point(854, 208);
            this._cbOpenAllAlarmSettings.Name = "_cbOpenAllAlarmSettings";
            this._cbOpenAllAlarmSettings.Size = new System.Drawing.Size(200, 17);
            this._cbOpenAllAlarmSettings.TabIndex = 7;
            this._cbOpenAllAlarmSettings.Text = "Open all";
            this._cbOpenAllAlarmSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._cbOpenAllAlarmSettings.UseVisualStyleBackColor = true;
            this._cbOpenAllAlarmSettings.CheckedChanged += new System.EventHandler(this._cbOpenAllAlarmSettings_CheckedChanged);
            // 
            // _gbSettingsForEventlogOnCR
            // 
            this._gbSettingsForEventlogOnCR.Controls.Add(this._tbLastEventTimeForMarkAlarmArea);
            this._gbSettingsForEventlogOnCR.Controls.Add(this._lCRLastEventTimeForMarkAlarmArea);
            this._gbSettingsForEventlogOnCR.Controls.Add(this._tbCrEvetlogLimitedSizeValue);
            this._gbSettingsForEventlogOnCR.Controls.Add(this._lCrEventlogLimitedSize);
            this._gbSettingsForEventlogOnCR.Location = new System.Drawing.Point(8, 85);
            this._gbSettingsForEventlogOnCR.Name = "_gbSettingsForEventlogOnCR";
            this._gbSettingsForEventlogOnCR.Size = new System.Drawing.Size(569, 92);
            this._gbSettingsForEventlogOnCR.TabIndex = 4;
            this._gbSettingsForEventlogOnCR.TabStop = false;
            this._gbSettingsForEventlogOnCR.Text = "Settings for eventlog on card reader";
            // 
            // _tbLastEventTimeForMarkAlarmArea
            // 
            this._tbLastEventTimeForMarkAlarmArea.Location = new System.Drawing.Point(260, 58);
            this._tbLastEventTimeForMarkAlarmArea.Name = "_tbLastEventTimeForMarkAlarmArea";
            this._tbLastEventTimeForMarkAlarmArea.Size = new System.Drawing.Size(59, 20);
            this._tbLastEventTimeForMarkAlarmArea.TabIndex = 8;
            this._tbLastEventTimeForMarkAlarmArea.Text = "60";
            this._tbLastEventTimeForMarkAlarmArea.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbLastEventTimeForMarkAlarmArea.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._tbCrEvetlogLimitedSizeValue_KeyPress);
            // 
            // _lCRLastEventTimeForMarkAlarmArea
            // 
            this._lCRLastEventTimeForMarkAlarmArea.AutoSize = true;
            this._lCRLastEventTimeForMarkAlarmArea.Location = new System.Drawing.Point(8, 61);
            this._lCRLastEventTimeForMarkAlarmArea.Name = "_lCRLastEventTimeForMarkAlarmArea";
            this._lCRLastEventTimeForMarkAlarmArea.Size = new System.Drawing.Size(172, 13);
            this._lCRLastEventTimeForMarkAlarmArea.TabIndex = 7;
            this._lCRLastEventTimeForMarkAlarmArea.Text = "Last event time for mark alarm area";
            // 
            // _tbCrEvetlogLimitedSizeValue
            // 
            this._tbCrEvetlogLimitedSizeValue.Location = new System.Drawing.Point(260, 24);
            this._tbCrEvetlogLimitedSizeValue.Name = "_tbCrEvetlogLimitedSizeValue";
            this._tbCrEvetlogLimitedSizeValue.Size = new System.Drawing.Size(59, 20);
            this._tbCrEvetlogLimitedSizeValue.TabIndex = 5;
            this._tbCrEvetlogLimitedSizeValue.Text = "100";
            this._tbCrEvetlogLimitedSizeValue.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbCrEvetlogLimitedSizeValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._tbCrEvetlogLimitedSizeValue_KeyPress);
            // 
            // _lCrEventlogLimitedSize
            // 
            this._lCrEventlogLimitedSize.AutoSize = true;
            this._lCrEventlogLimitedSize.Location = new System.Drawing.Point(8, 27);
            this._lCrEventlogLimitedSize.Name = "_lCrEventlogLimitedSize";
            this._lCrEventlogLimitedSize.Size = new System.Drawing.Size(192, 13);
            this._lCrEventlogLimitedSize.TabIndex = 4;
            this._lCrEventlogLimitedSize.Text = "Card reader eventlog limited queue size";
            // 
            // _gbServerWatchdog
            // 
            this._gbServerWatchdog.Controls.Add(this._cmsoCcuOffline);
            this._gbServerWatchdog.Location = new System.Drawing.Point(8, 8);
            this._gbServerWatchdog.Margin = new System.Windows.Forms.Padding(4);
            this._gbServerWatchdog.Name = "_gbServerWatchdog";
            this._gbServerWatchdog.Padding = new System.Windows.Forms.Padding(4);
            this._gbServerWatchdog.Size = new System.Drawing.Size(569, 70);
            this._gbServerWatchdog.TabIndex = 0;
            this._gbServerWatchdog.TabStop = false;
            this._gbServerWatchdog.Text = "Server connection";
            // 
            // _cmsoCcuOffline
            // 
            this._cmsoCcuOffline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cmsoCcuOffline.Location = new System.Drawing.Point(10, 21);
            this._cmsoCcuOffline.Name = "_cmsoCcuOffline";
            this._cmsoCcuOffline.Plugin = null;
            this._cmsoCcuOffline.Size = new System.Drawing.Size(550, 42);
            this._cmsoCcuOffline.SpecialOutput = null;
            this._cmsoCcuOffline.TabIndex = 0;
            // 
            // _tpUpsMonitor
            // 
            this._tpUpsMonitor.BackColor = System.Drawing.SystemColors.Control;
            this._tpUpsMonitor.Controls.Add(this._gpMaintenance);
            this._tpUpsMonitor.Controls.Add(this._gpAlarms);
            this._tpUpsMonitor.Controls.Add(this._gpValues);
            this._tpUpsMonitor.Location = new System.Drawing.Point(4, 40);
            this._tpUpsMonitor.Margin = new System.Windows.Forms.Padding(4);
            this._tpUpsMonitor.Name = "_tpUpsMonitor";
            this._tpUpsMonitor.Padding = new System.Windows.Forms.Padding(4);
            this._tpUpsMonitor.Size = new System.Drawing.Size(1060, 530);
            this._tpUpsMonitor.TabIndex = 16;
            this._tpUpsMonitor.Text = "UPS monitor";
            this._tpUpsMonitor.Leave += new System.EventHandler(this._tpUpsMonitor_Leave);
            this._tpUpsMonitor.Enter += new System.EventHandler(this._tpUpsMonitor_Enter);
            // 
            // _gpMaintenance
            // 
            this._gpMaintenance.Controls.Add(this._lUpsOnlineState);
            this._gpMaintenance.Controls.Add(this._lOnlineState);
            this._gpMaintenance.Controls.Add(this.m_labSuccessRate);
            this._gpMaintenance.Controls.Add(this._lSuccessRate);
            this._gpMaintenance.Controls.Add(this.m_labResets);
            this._gpMaintenance.Controls.Add(this._lResets);
            this._gpMaintenance.Controls.Add(this.m_labMode);
            this._gpMaintenance.Controls.Add(this._lMode);
            this._gpMaintenance.Location = new System.Drawing.Point(8, 328);
            this._gpMaintenance.Margin = new System.Windows.Forms.Padding(4);
            this._gpMaintenance.Name = "_gpMaintenance";
            this._gpMaintenance.Padding = new System.Windows.Forms.Padding(4);
            this._gpMaintenance.Size = new System.Drawing.Size(659, 110);
            this._gpMaintenance.TabIndex = 31;
            this._gpMaintenance.TabStop = false;
            this._gpMaintenance.Text = "Maintenance indicators";
            // 
            // _lUpsOnlineState
            // 
            this._lUpsOnlineState.BackColor = System.Drawing.Color.Gray;
            this._lUpsOnlineState.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lUpsOnlineState.ForeColor = System.Drawing.Color.White;
            this._lUpsOnlineState.Location = new System.Drawing.Point(505, 66);
            this._lUpsOnlineState.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lUpsOnlineState.Name = "_lUpsOnlineState";
            this._lUpsOnlineState.Size = new System.Drawing.Size(145, 29);
            this._lUpsOnlineState.TabIndex = 40;
            this._lUpsOnlineState.Text = "Unknown";
            this._lUpsOnlineState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lOnlineState
            // 
            this._lOnlineState.AutoSize = true;
            this._lOnlineState.Location = new System.Drawing.Point(336, 68);
            this._lOnlineState.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lOnlineState.Name = "_lOnlineState";
            this._lOnlineState.Size = new System.Drawing.Size(63, 13);
            this._lOnlineState.TabIndex = 39;
            this._lOnlineState.Text = "Online state";
            // 
            // m_labSuccessRate
            // 
            this.m_labSuccessRate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labSuccessRate.Location = new System.Drawing.Point(505, 31);
            this.m_labSuccessRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labSuccessRate.Name = "m_labSuccessRate";
            this.m_labSuccessRate.Size = new System.Drawing.Size(145, 29);
            this.m_labSuccessRate.TabIndex = 38;
            this.m_labSuccessRate.Tag = "value";
            this.m_labSuccessRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lSuccessRate
            // 
            this._lSuccessRate.AutoSize = true;
            this._lSuccessRate.Location = new System.Drawing.Point(336, 32);
            this._lSuccessRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lSuccessRate.Name = "_lSuccessRate";
            this._lSuccessRate.Size = new System.Drawing.Size(69, 13);
            this._lSuccessRate.TabIndex = 37;
            this._lSuccessRate.Text = "Success rate";
            // 
            // m_labResets
            // 
            this.m_labResets.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labResets.Location = new System.Drawing.Point(162, 65);
            this.m_labResets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labResets.Name = "m_labResets";
            this.m_labResets.Size = new System.Drawing.Size(145, 29);
            this.m_labResets.TabIndex = 36;
            this.m_labResets.Tag = "value";
            this.m_labResets.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lResets
            // 
            this._lResets.AutoSize = true;
            this._lResets.Location = new System.Drawing.Point(6, 66);
            this._lResets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lResets.Name = "_lResets";
            this._lResets.Size = new System.Drawing.Size(40, 13);
            this._lResets.TabIndex = 35;
            this._lResets.Text = "Resets";
            // 
            // m_labMode
            // 
            this.m_labMode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labMode.Location = new System.Drawing.Point(162, 31);
            this.m_labMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labMode.Name = "m_labMode";
            this.m_labMode.Size = new System.Drawing.Size(145, 29);
            this.m_labMode.TabIndex = 34;
            this.m_labMode.Tag = "value";
            this.m_labMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lMode
            // 
            this._lMode.AutoSize = true;
            this._lMode.Location = new System.Drawing.Point(8, 32);
            this._lMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMode.Name = "_lMode";
            this._lMode.Size = new System.Drawing.Size(34, 13);
            this._lMode.TabIndex = 34;
            this._lMode.Text = "Mode";
            // 
            // _gpAlarms
            // 
            this._gpAlarms.Controls.Add(this._lBatteryFuse);
            this._gpAlarms.Controls.Add(this._lTamper);
            this._gpAlarms.Controls.Add(this._lOvertemperature);
            this._gpAlarms.Controls.Add(this._lBatteryEmpty);
            this._gpAlarms.Controls.Add(this._lBatteryFault);
            this._gpAlarms.Controls.Add(this._lPrimaryPowerMissing);
            this._gpAlarms.Controls.Add(this._lOutputOutOfTollerance);
            this._gpAlarms.Controls.Add(this._lOutputFuse);
            this._gpAlarms.Location = new System.Drawing.Point(340, 8);
            this._gpAlarms.Margin = new System.Windows.Forms.Padding(4);
            this._gpAlarms.Name = "_gpAlarms";
            this._gpAlarms.Padding = new System.Windows.Forms.Padding(4);
            this._gpAlarms.Size = new System.Drawing.Size(326, 312);
            this._gpAlarms.TabIndex = 30;
            this._gpAlarms.TabStop = false;
            this._gpAlarms.Text = "Alarm indicators";
            // 
            // _lBatteryFuse
            // 
            this._lBatteryFuse.BackColor = System.Drawing.Color.Silver;
            this._lBatteryFuse.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lBatteryFuse.ForeColor = System.Drawing.Color.Gray;
            this._lBatteryFuse.Location = new System.Drawing.Point(8, 199);
            this._lBatteryFuse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lBatteryFuse.Name = "_lBatteryFuse";
            this._lBatteryFuse.Size = new System.Drawing.Size(310, 29);
            this._lBatteryFuse.TabIndex = 34;
            this._lBatteryFuse.Tag = "value";
            this._lBatteryFuse.Text = "Battery fuse";
            this._lBatteryFuse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lTamper
            // 
            this._lTamper.BackColor = System.Drawing.Color.Silver;
            this._lTamper.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lTamper.ForeColor = System.Drawing.Color.Gray;
            this._lTamper.Location = new System.Drawing.Point(8, 266);
            this._lTamper.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTamper.Name = "_lTamper";
            this._lTamper.Size = new System.Drawing.Size(310, 29);
            this._lTamper.TabIndex = 33;
            this._lTamper.Tag = "value";
            this._lTamper.Text = "Tamper";
            this._lTamper.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lOvertemperature
            // 
            this._lOvertemperature.BackColor = System.Drawing.Color.Silver;
            this._lOvertemperature.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lOvertemperature.ForeColor = System.Drawing.Color.Gray;
            this._lOvertemperature.Location = new System.Drawing.Point(8, 232);
            this._lOvertemperature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lOvertemperature.Name = "_lOvertemperature";
            this._lOvertemperature.Size = new System.Drawing.Size(310, 29);
            this._lOvertemperature.TabIndex = 32;
            this._lOvertemperature.Tag = "value";
            this._lOvertemperature.Text = "Overtemperature";
            this._lOvertemperature.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lBatteryEmpty
            // 
            this._lBatteryEmpty.BackColor = System.Drawing.Color.Silver;
            this._lBatteryEmpty.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lBatteryEmpty.ForeColor = System.Drawing.Color.Gray;
            this._lBatteryEmpty.Location = new System.Drawing.Point(8, 165);
            this._lBatteryEmpty.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lBatteryEmpty.Name = "_lBatteryEmpty";
            this._lBatteryEmpty.Size = new System.Drawing.Size(310, 29);
            this._lBatteryEmpty.TabIndex = 31;
            this._lBatteryEmpty.Tag = "value";
            this._lBatteryEmpty.Text = "Battery empty";
            this._lBatteryEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lBatteryFault
            // 
            this._lBatteryFault.BackColor = System.Drawing.Color.Silver;
            this._lBatteryFault.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lBatteryFault.ForeColor = System.Drawing.Color.Gray;
            this._lBatteryFault.Location = new System.Drawing.Point(8, 131);
            this._lBatteryFault.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lBatteryFault.Name = "_lBatteryFault";
            this._lBatteryFault.Size = new System.Drawing.Size(310, 29);
            this._lBatteryFault.TabIndex = 30;
            this._lBatteryFault.Tag = "value";
            this._lBatteryFault.Text = "Battery fault";
            this._lBatteryFault.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lPrimaryPowerMissing
            // 
            this._lPrimaryPowerMissing.BackColor = System.Drawing.Color.Silver;
            this._lPrimaryPowerMissing.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lPrimaryPowerMissing.ForeColor = System.Drawing.Color.Gray;
            this._lPrimaryPowerMissing.Location = new System.Drawing.Point(8, 98);
            this._lPrimaryPowerMissing.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPrimaryPowerMissing.Name = "_lPrimaryPowerMissing";
            this._lPrimaryPowerMissing.Size = new System.Drawing.Size(310, 29);
            this._lPrimaryPowerMissing.TabIndex = 29;
            this._lPrimaryPowerMissing.Tag = "value";
            this._lPrimaryPowerMissing.Text = "Primary power missing";
            this._lPrimaryPowerMissing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lOutputOutOfTollerance
            // 
            this._lOutputOutOfTollerance.BackColor = System.Drawing.Color.Silver;
            this._lOutputOutOfTollerance.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lOutputOutOfTollerance.ForeColor = System.Drawing.Color.Gray;
            this._lOutputOutOfTollerance.Location = new System.Drawing.Point(8, 64);
            this._lOutputOutOfTollerance.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lOutputOutOfTollerance.Name = "_lOutputOutOfTollerance";
            this._lOutputOutOfTollerance.Size = new System.Drawing.Size(310, 29);
            this._lOutputOutOfTollerance.TabIndex = 28;
            this._lOutputOutOfTollerance.Tag = "value";
            this._lOutputOutOfTollerance.Text = "Output out of tollerance";
            this._lOutputOutOfTollerance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lOutputFuse
            // 
            this._lOutputFuse.BackColor = System.Drawing.Color.Silver;
            this._lOutputFuse.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lOutputFuse.ForeColor = System.Drawing.Color.Gray;
            this._lOutputFuse.Location = new System.Drawing.Point(8, 30);
            this._lOutputFuse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lOutputFuse.Name = "_lOutputFuse";
            this._lOutputFuse.Size = new System.Drawing.Size(310, 29);
            this._lOutputFuse.TabIndex = 27;
            this._lOutputFuse.Tag = "value";
            this._lOutputFuse.Text = "Output fuse";
            this._lOutputFuse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _gpValues
            // 
            this._gpValues.Controls.Add(this.m_pgEstimatedBatteryCapacity);
            this._gpValues.Controls.Add(this.m_labTemperature);
            this._gpValues.Controls.Add(this.m_labEstimatedBatteryCapacity);
            this._gpValues.Controls.Add(this.m_labCurrentLoad);
            this._gpValues.Controls.Add(this.m_labCurrentBattery);
            this._gpValues.Controls.Add(this.m_labVoltageBattery);
            this._gpValues.Controls.Add(this.m_labVoltageOutput);
            this._gpValues.Controls.Add(this.m_labVoltageInput);
            this._gpValues.Controls.Add(this._lTemperature);
            this._gpValues.Controls.Add(this._lEstimatedBatterycapacity);
            this._gpValues.Controls.Add(this._lLoadCurrent);
            this._gpValues.Controls.Add(this._lBatteryCurrent);
            this._gpValues.Controls.Add(this._lBatteryVoltage);
            this._gpValues.Controls.Add(this._lOutputVoltage);
            this._gpValues.Controls.Add(this._lInputVoltage);
            this._gpValues.Location = new System.Drawing.Point(8, 8);
            this._gpValues.Margin = new System.Windows.Forms.Padding(4);
            this._gpValues.Name = "_gpValues";
            this._gpValues.Padding = new System.Windows.Forms.Padding(4);
            this._gpValues.Size = new System.Drawing.Size(325, 312);
            this._gpValues.TabIndex = 29;
            this._gpValues.TabStop = false;
            this._gpValues.Text = "Value indicators";
            // 
            // m_pgEstimatedBatteryCapacity
            // 
            this.m_pgEstimatedBatteryCapacity.Location = new System.Drawing.Point(162, 199);
            this.m_pgEstimatedBatteryCapacity.Margin = new System.Windows.Forms.Padding(4);
            this.m_pgEstimatedBatteryCapacity.Name = "m_pgEstimatedBatteryCapacity";
            this.m_pgEstimatedBatteryCapacity.Size = new System.Drawing.Size(145, 29);
            this.m_pgEstimatedBatteryCapacity.TabIndex = 33;
            this.m_pgEstimatedBatteryCapacity.Tag = "value";
            // 
            // m_labTemperature
            // 
            this.m_labTemperature.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labTemperature.Location = new System.Drawing.Point(162, 266);
            this.m_labTemperature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labTemperature.Name = "m_labTemperature";
            this.m_labTemperature.Size = new System.Drawing.Size(145, 29);
            this.m_labTemperature.TabIndex = 32;
            this.m_labTemperature.Tag = "value";
            this.m_labTemperature.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labEstimatedBatteryCapacity
            // 
            this.m_labEstimatedBatteryCapacity.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labEstimatedBatteryCapacity.Location = new System.Drawing.Point(162, 232);
            this.m_labEstimatedBatteryCapacity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labEstimatedBatteryCapacity.Name = "m_labEstimatedBatteryCapacity";
            this.m_labEstimatedBatteryCapacity.Size = new System.Drawing.Size(145, 29);
            this.m_labEstimatedBatteryCapacity.TabIndex = 31;
            this.m_labEstimatedBatteryCapacity.Tag = "value";
            this.m_labEstimatedBatteryCapacity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labCurrentLoad
            // 
            this.m_labCurrentLoad.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labCurrentLoad.Location = new System.Drawing.Point(162, 165);
            this.m_labCurrentLoad.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labCurrentLoad.Name = "m_labCurrentLoad";
            this.m_labCurrentLoad.Size = new System.Drawing.Size(145, 29);
            this.m_labCurrentLoad.TabIndex = 30;
            this.m_labCurrentLoad.Tag = "value";
            this.m_labCurrentLoad.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labCurrentBattery
            // 
            this.m_labCurrentBattery.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labCurrentBattery.Location = new System.Drawing.Point(162, 131);
            this.m_labCurrentBattery.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labCurrentBattery.Name = "m_labCurrentBattery";
            this.m_labCurrentBattery.Size = new System.Drawing.Size(145, 29);
            this.m_labCurrentBattery.TabIndex = 29;
            this.m_labCurrentBattery.Tag = "value";
            this.m_labCurrentBattery.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labVoltageBattery
            // 
            this.m_labVoltageBattery.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labVoltageBattery.Location = new System.Drawing.Point(162, 98);
            this.m_labVoltageBattery.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labVoltageBattery.Name = "m_labVoltageBattery";
            this.m_labVoltageBattery.Size = new System.Drawing.Size(145, 29);
            this.m_labVoltageBattery.TabIndex = 28;
            this.m_labVoltageBattery.Tag = "value";
            this.m_labVoltageBattery.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labVoltageOutput
            // 
            this.m_labVoltageOutput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labVoltageOutput.Location = new System.Drawing.Point(162, 64);
            this.m_labVoltageOutput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labVoltageOutput.Name = "m_labVoltageOutput";
            this.m_labVoltageOutput.Size = new System.Drawing.Size(145, 29);
            this.m_labVoltageOutput.TabIndex = 27;
            this.m_labVoltageOutput.Tag = "value";
            this.m_labVoltageOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_labVoltageInput
            // 
            this.m_labVoltageInput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_labVoltageInput.Location = new System.Drawing.Point(162, 30);
            this.m_labVoltageInput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_labVoltageInput.Name = "m_labVoltageInput";
            this.m_labVoltageInput.Size = new System.Drawing.Size(145, 29);
            this.m_labVoltageInput.TabIndex = 26;
            this.m_labVoltageInput.Tag = "value";
            this.m_labVoltageInput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lTemperature
            // 
            this._lTemperature.AutoSize = true;
            this._lTemperature.Location = new System.Drawing.Point(6, 268);
            this._lTemperature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTemperature.Name = "_lTemperature";
            this._lTemperature.Size = new System.Drawing.Size(67, 13);
            this._lTemperature.TabIndex = 25;
            this._lTemperature.Text = "Temperature";
            // 
            // _lEstimatedBatterycapacity
            // 
            this._lEstimatedBatterycapacity.Location = new System.Drawing.Point(8, 199);
            this._lEstimatedBatterycapacity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lEstimatedBatterycapacity.Name = "_lEstimatedBatterycapacity";
            this._lEstimatedBatterycapacity.Size = new System.Drawing.Size(145, 62);
            this._lEstimatedBatterycapacity.TabIndex = 24;
            this._lEstimatedBatterycapacity.Text = "Estimated battery capacity";
            this._lEstimatedBatterycapacity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._lEstimatedBatterycapacity.UseCompatibleTextRendering = true;
            // 
            // _lLoadCurrent
            // 
            this._lLoadCurrent.AutoSize = true;
            this._lLoadCurrent.Location = new System.Drawing.Point(6, 166);
            this._lLoadCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lLoadCurrent.Name = "_lLoadCurrent";
            this._lLoadCurrent.Size = new System.Drawing.Size(67, 13);
            this._lLoadCurrent.TabIndex = 23;
            this._lLoadCurrent.Text = "Load current";
            // 
            // _lBatteryCurrent
            // 
            this._lBatteryCurrent.AutoSize = true;
            this._lBatteryCurrent.Location = new System.Drawing.Point(6, 132);
            this._lBatteryCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lBatteryCurrent.Name = "_lBatteryCurrent";
            this._lBatteryCurrent.Size = new System.Drawing.Size(76, 13);
            this._lBatteryCurrent.TabIndex = 22;
            this._lBatteryCurrent.Text = "Battery current";
            // 
            // _lBatteryVoltage
            // 
            this._lBatteryVoltage.AutoSize = true;
            this._lBatteryVoltage.Location = new System.Drawing.Point(6, 99);
            this._lBatteryVoltage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lBatteryVoltage.Name = "_lBatteryVoltage";
            this._lBatteryVoltage.Size = new System.Drawing.Size(78, 13);
            this._lBatteryVoltage.TabIndex = 21;
            this._lBatteryVoltage.Text = "Battery voltage";
            // 
            // _lOutputVoltage
            // 
            this._lOutputVoltage.AutoSize = true;
            this._lOutputVoltage.Location = new System.Drawing.Point(6, 65);
            this._lOutputVoltage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lOutputVoltage.Name = "_lOutputVoltage";
            this._lOutputVoltage.Size = new System.Drawing.Size(77, 13);
            this._lOutputVoltage.TabIndex = 20;
            this._lOutputVoltage.Text = "Output voltage";
            // 
            // _lInputVoltage
            // 
            this._lInputVoltage.AutoSize = true;
            this._lInputVoltage.Location = new System.Drawing.Point(6, 31);
            this._lInputVoltage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lInputVoltage.Name = "_lInputVoltage";
            this._lInputVoltage.Size = new System.Drawing.Size(69, 13);
            this._lInputVoltage.TabIndex = 19;
            this._lInputVoltage.Text = "Input voltage";
            // 
            // _tpStatistics
            // 
            this._tpStatistics.BackColor = System.Drawing.SystemColors.Control;
            this._tpStatistics.Controls.Add(this._tcStatistics);
            this._tpStatistics.Location = new System.Drawing.Point(4, 40);
            this._tpStatistics.Margin = new System.Windows.Forms.Padding(4);
            this._tpStatistics.Name = "_tpStatistics";
            this._tpStatistics.Padding = new System.Windows.Forms.Padding(4);
            this._tpStatistics.Size = new System.Drawing.Size(1060, 530);
            this._tpStatistics.TabIndex = 14;
            this._tpStatistics.Text = "Statistics";
            // 
            // _tcStatistics
            // 
            this._tcStatistics.Controls.Add(this._tpCommunicationAndMemory);
            this._tcStatistics.Controls.Add(this._tpThreadMap);
            this._tcStatistics.Controls.Add(this._tpOtherStatistics);
            this._tcStatistics.Controls.Add(this._tpMemory);
            this._tcStatistics.Controls.Add(this._tpProcesses);
            this._tcStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tcStatistics.Location = new System.Drawing.Point(4, 4);
            this._tcStatistics.Margin = new System.Windows.Forms.Padding(4);
            this._tcStatistics.Name = "_tcStatistics";
            this._tcStatistics.SelectedIndex = 0;
            this._tcStatistics.Size = new System.Drawing.Size(1052, 522);
            this._tcStatistics.TabIndex = 5;
            // 
            // _tpCommunicationAndMemory
            // 
            this._tpCommunicationAndMemory.BackColor = System.Drawing.Color.Transparent;
            this._tpCommunicationAndMemory.Controls.Add(this._gbCommunicationStatistic);
            this._tpCommunicationAndMemory.Controls.Add(this._gbOtherStatistics);
            this._tpCommunicationAndMemory.Location = new System.Drawing.Point(4, 22);
            this._tpCommunicationAndMemory.Margin = new System.Windows.Forms.Padding(4);
            this._tpCommunicationAndMemory.Name = "_tpCommunicationAndMemory";
            this._tpCommunicationAndMemory.Padding = new System.Windows.Forms.Padding(4);
            this._tpCommunicationAndMemory.Size = new System.Drawing.Size(1044, 496);
            this._tpCommunicationAndMemory.TabIndex = 0;
            this._tpCommunicationAndMemory.Text = "Communication and performance";
            this._tpCommunicationAndMemory.UseVisualStyleBackColor = true;
            // 
            // _gbCommunicationStatistic
            // 
            this._gbCommunicationStatistic.Controls.Add(this._tlpCOmmunicationStatistics);
            this._gbCommunicationStatistic.Controls.Add(this._bResetAll);
            this._gbCommunicationStatistic.Controls.Add(this._bRefreshCommunicationStatistic);
            this._gbCommunicationStatistic.Location = new System.Drawing.Point(8, 150);
            this._gbCommunicationStatistic.Margin = new System.Windows.Forms.Padding(4);
            this._gbCommunicationStatistic.Name = "_gbCommunicationStatistic";
            this._gbCommunicationStatistic.Padding = new System.Windows.Forms.Padding(4);
            this._gbCommunicationStatistic.Size = new System.Drawing.Size(1025, 362);
            this._gbCommunicationStatistic.TabIndex = 0;
            this._gbCommunicationStatistic.TabStop = false;
            this._gbCommunicationStatistic.Text = "Communication statistic";
            // 
            // _tlpCOmmunicationStatistics
            // 
            this._tlpCOmmunicationStatistics.AutoScroll = true;
            this._tlpCOmmunicationStatistics.ColumnCount = 6;
            this._tlpCOmmunicationStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpCOmmunicationStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpCOmmunicationStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpCOmmunicationStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpCOmmunicationStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpCOmmunicationStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCCUNotAcnknowledgedAutonomousEvents, 3, 7);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCCUNotAcnknowledgedAutonomousEvents, 4, 7);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCcuSended, 4, 0);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCCUNotAcnknowledgedEvents, 3, 6);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCCUUnprocessedEvents, 4, 8);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bReset1, 5, 5);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lServerSended, 0, 0);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eServerSended, 1, 0);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bCcuResetSended, 5, 0);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bResetServerSended, 2, 0);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCcuSended, 3, 0);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bServerResetReceived, 2, 1);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCcuMsgRetry, 3, 4);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCommandTimeouts, 4, 5);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCommandTimeoutCount, 3, 5);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bCcuMsgRetry, 5, 4);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eServerReceived, 1, 1);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lServerReceived, 0, 1);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCcuMsgRetry, 4, 4);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCcuReceived, 3, 1);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCcuReceived, 4, 1);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bCcuResetReceived, 5, 1);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCcuReceivedError, 3, 3);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bCcuResetReceivedError, 5, 3);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lServerDeserializeError, 0, 2);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCcuReceivedError, 4, 3);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCcuDeserializeError, 3, 2);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bServerResetDeserializeError, 2, 2);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bCcuResetDeserializeError, 5, 2);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lServerMsgRetry, 0, 4);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lServerReceivedError, 0, 3);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCcuDeserializeError, 4, 2);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lServersNotStoredEvents, 0, 5);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lServersNotProcessedEvents, 0, 6);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eServerDeserializeError, 1, 2);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eServerMsgRetry, 1, 4);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eServerReceivedError, 1, 3);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eServersNotStoredEvents, 1, 5);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eServersNotProcessedEvents, 1, 6);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bResetServerMsgRetry, 2, 4);
            this._tlpCOmmunicationStatistics.Controls.Add(this._bServerResetReceivedError, 2, 3);
            this._tlpCOmmunicationStatistics.Controls.Add(this._lCCUUnprocessedEvents, 3, 8);
            this._tlpCOmmunicationStatistics.Controls.Add(this._eCCUNotAcknowledgedEvents, 4, 6);
            this._tlpCOmmunicationStatistics.Location = new System.Drawing.Point(8, 24);
            this._tlpCOmmunicationStatistics.Margin = new System.Windows.Forms.Padding(4);
            this._tlpCOmmunicationStatistics.Name = "_tlpCOmmunicationStatistics";
            this._tlpCOmmunicationStatistics.RowCount = 9;
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpCOmmunicationStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this._tlpCOmmunicationStatistics.Size = new System.Drawing.Size(885, 320);
            this._tlpCOmmunicationStatistics.TabIndex = 42;
            // 
            // _lCCUNotAcnknowledgedAutonomousEvents
            // 
            this._lCCUNotAcnknowledgedAutonomousEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCCUNotAcnknowledgedAutonomousEvents.AutoSize = true;
            this._lCCUNotAcnknowledgedAutonomousEvents.Location = new System.Drawing.Point(394, 254);
            this._lCCUNotAcnknowledgedAutonomousEvents.Margin = new System.Windows.Forms.Padding(4);
            this._lCCUNotAcnknowledgedAutonomousEvents.MaximumSize = new System.Drawing.Size(194, 32);
            this._lCCUNotAcnknowledgedAutonomousEvents.Name = "_lCCUNotAcnknowledgedAutonomousEvents";
            this._lCCUNotAcnknowledgedAutonomousEvents.Size = new System.Drawing.Size(184, 26);
            this._lCCUNotAcnknowledgedAutonomousEvents.TabIndex = 43;
            this._lCCUNotAcnknowledgedAutonomousEvents.Text = "CCU not acknowledged autonomous events";
            // 
            // _eCCUNotAcnknowledgedAutonomousEvents
            // 
            this._eCCUNotAcnknowledgedAutonomousEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCCUNotAcnknowledgedAutonomousEvents.Location = new System.Drawing.Point(586, 257);
            this._eCCUNotAcnknowledgedAutonomousEvents.Margin = new System.Windows.Forms.Padding(4);
            this._eCCUNotAcnknowledgedAutonomousEvents.Name = "_eCCUNotAcnknowledgedAutonomousEvents";
            this._eCCUNotAcnknowledgedAutonomousEvents.ReadOnly = true;
            this._eCCUNotAcnknowledgedAutonomousEvents.Size = new System.Drawing.Size(124, 20);
            this._eCCUNotAcnknowledgedAutonomousEvents.TabIndex = 42;
            // 
            // _eCcuSended
            // 
            this._eCcuSended.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCcuSended.Location = new System.Drawing.Point(586, 8);
            this._eCcuSended.Margin = new System.Windows.Forms.Padding(4);
            this._eCcuSended.Name = "_eCcuSended";
            this._eCcuSended.ReadOnly = true;
            this._eCcuSended.Size = new System.Drawing.Size(124, 20);
            this._eCcuSended.TabIndex = 16;
            this._eCcuSended.Visible = false;
            // 
            // _lCCUNotAcnknowledgedEvents
            // 
            this._lCCUNotAcnknowledgedEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCCUNotAcnknowledgedEvents.AutoSize = true;
            this._lCCUNotAcnknowledgedEvents.Location = new System.Drawing.Point(394, 229);
            this._lCCUNotAcnknowledgedEvents.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCCUNotAcnknowledgedEvents.Name = "_lCCUNotAcnknowledgedEvents";
            this._lCCUNotAcnknowledgedEvents.Size = new System.Drawing.Size(155, 13);
            this._lCCUNotAcnknowledgedEvents.TabIndex = 35;
            this._lCCUNotAcnknowledgedEvents.Text = "CCU not acknowledged events";
            // 
            // _eCCUUnprocessedEvents
            // 
            this._eCCUUnprocessedEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCCUUnprocessedEvents.Location = new System.Drawing.Point(586, 292);
            this._eCCUUnprocessedEvents.Margin = new System.Windows.Forms.Padding(4);
            this._eCCUUnprocessedEvents.Name = "_eCCUUnprocessedEvents";
            this._eCCUUnprocessedEvents.ReadOnly = true;
            this._eCCUUnprocessedEvents.Size = new System.Drawing.Size(124, 20);
            this._eCCUUnprocessedEvents.TabIndex = 40;
            this._eCCUUnprocessedEvents.Visible = false;
            // 
            // _bReset1
            // 
            this._bReset1.Location = new System.Drawing.Point(718, 189);
            this._bReset1.Margin = new System.Windows.Forms.Padding(4);
            this._bReset1.Name = "_bReset1";
            this._bReset1.Size = new System.Drawing.Size(94, 29);
            this._bReset1.TabIndex = 41;
            this._bReset1.Text = "Reset";
            this._bReset1.UseVisualStyleBackColor = true;
            this._bReset1.Visible = false;
            this._bReset1.Click += new System.EventHandler(this._Reset1_Click);
            // 
            // _lServerSended
            // 
            this._lServerSended.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lServerSended.AutoSize = true;
            this._lServerSended.Location = new System.Drawing.Point(4, 12);
            this._lServerSended.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServerSended.Name = "_lServerSended";
            this._lServerSended.Size = new System.Drawing.Size(61, 13);
            this._lServerSended.TabIndex = 0;
            this._lServerSended.Text = "Server sent";
            this._lServerSended.Visible = false;
            // 
            // _eServerSended
            // 
            this._eServerSended.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eServerSended.Location = new System.Drawing.Point(160, 8);
            this._eServerSended.Margin = new System.Windows.Forms.Padding(4);
            this._eServerSended.Name = "_eServerSended";
            this._eServerSended.ReadOnly = true;
            this._eServerSended.Size = new System.Drawing.Size(124, 20);
            this._eServerSended.TabIndex = 1;
            this._eServerSended.Visible = false;
            // 
            // _bCcuResetSended
            // 
            this._bCcuResetSended.Location = new System.Drawing.Point(718, 4);
            this._bCcuResetSended.Margin = new System.Windows.Forms.Padding(4);
            this._bCcuResetSended.Name = "_bCcuResetSended";
            this._bCcuResetSended.Size = new System.Drawing.Size(94, 29);
            this._bCcuResetSended.TabIndex = 17;
            this._bCcuResetSended.Text = "Reset";
            this._bCcuResetSended.UseVisualStyleBackColor = true;
            this._bCcuResetSended.Visible = false;
            this._bCcuResetSended.Click += new System.EventHandler(this._bCcuResetSended_Click);
            // 
            // _bResetServerSended
            // 
            this._bResetServerSended.Location = new System.Drawing.Point(292, 4);
            this._bResetServerSended.Margin = new System.Windows.Forms.Padding(4);
            this._bResetServerSended.Name = "_bResetServerSended";
            this._bResetServerSended.Size = new System.Drawing.Size(94, 29);
            this._bResetServerSended.TabIndex = 2;
            this._bResetServerSended.Text = "Reset";
            this._bResetServerSended.UseVisualStyleBackColor = true;
            this._bResetServerSended.Visible = false;
            this._bResetServerSended.Click += new System.EventHandler(this._bResetServerSended_Click);
            // 
            // _lCcuSended
            // 
            this._lCcuSended.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCcuSended.AutoSize = true;
            this._lCcuSended.Location = new System.Drawing.Point(394, 12);
            this._lCcuSended.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCcuSended.Name = "_lCcuSended";
            this._lCcuSended.Size = new System.Drawing.Size(52, 13);
            this._lCcuSended.TabIndex = 15;
            this._lCcuSended.Text = "CCU sent";
            this._lCcuSended.Visible = false;
            // 
            // _bServerResetReceived
            // 
            this._bServerResetReceived.Location = new System.Drawing.Point(292, 41);
            this._bServerResetReceived.Margin = new System.Windows.Forms.Padding(4);
            this._bServerResetReceived.Name = "_bServerResetReceived";
            this._bServerResetReceived.Size = new System.Drawing.Size(94, 29);
            this._bServerResetReceived.TabIndex = 5;
            this._bServerResetReceived.Text = "Reset";
            this._bServerResetReceived.UseVisualStyleBackColor = true;
            this._bServerResetReceived.Visible = false;
            this._bServerResetReceived.Click += new System.EventHandler(this._bServerResetReceived_Click);
            // 
            // _lCcuMsgRetry
            // 
            this._lCcuMsgRetry.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCcuMsgRetry.AutoSize = true;
            this._lCcuMsgRetry.Location = new System.Drawing.Point(394, 160);
            this._lCcuMsgRetry.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCcuMsgRetry.Name = "_lCcuMsgRetry";
            this._lCcuMsgRetry.Size = new System.Drawing.Size(97, 13);
            this._lCcuMsgRetry.TabIndex = 27;
            this._lCcuMsgRetry.Text = "CCU message retry";
            this._lCcuMsgRetry.Visible = false;
            // 
            // _eCommandTimeouts
            // 
            this._eCommandTimeouts.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCommandTimeouts.Location = new System.Drawing.Point(586, 193);
            this._eCommandTimeouts.Margin = new System.Windows.Forms.Padding(4);
            this._eCommandTimeouts.Name = "_eCommandTimeouts";
            this._eCommandTimeouts.ReadOnly = true;
            this._eCommandTimeouts.Size = new System.Drawing.Size(124, 20);
            this._eCommandTimeouts.TabIndex = 0;
            this._eCommandTimeouts.Visible = false;
            // 
            // _lCommandTimeoutCount
            // 
            this._lCommandTimeoutCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCommandTimeoutCount.AutoSize = true;
            this._lCommandTimeoutCount.Location = new System.Drawing.Point(394, 197);
            this._lCommandTimeoutCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCommandTimeoutCount.Name = "_lCommandTimeoutCount";
            this._lCommandTimeoutCount.Size = new System.Drawing.Size(120, 13);
            this._lCommandTimeoutCount.TabIndex = 1;
            this._lCommandTimeoutCount.Text = "CCU command timeouts";
            this._lCommandTimeoutCount.Visible = false;
            // 
            // _bCcuMsgRetry
            // 
            this._bCcuMsgRetry.Location = new System.Drawing.Point(718, 152);
            this._bCcuMsgRetry.Margin = new System.Windows.Forms.Padding(4);
            this._bCcuMsgRetry.Name = "_bCcuMsgRetry";
            this._bCcuMsgRetry.Size = new System.Drawing.Size(94, 29);
            this._bCcuMsgRetry.TabIndex = 29;
            this._bCcuMsgRetry.Text = "Reset";
            this._bCcuMsgRetry.UseVisualStyleBackColor = true;
            this._bCcuMsgRetry.Visible = false;
            this._bCcuMsgRetry.Click += new System.EventHandler(this._bCcuMsgRetry_Click);
            // 
            // _eServerReceived
            // 
            this._eServerReceived.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eServerReceived.Location = new System.Drawing.Point(160, 45);
            this._eServerReceived.Margin = new System.Windows.Forms.Padding(4);
            this._eServerReceived.Name = "_eServerReceived";
            this._eServerReceived.ReadOnly = true;
            this._eServerReceived.Size = new System.Drawing.Size(124, 20);
            this._eServerReceived.TabIndex = 4;
            this._eServerReceived.Visible = false;
            // 
            // _lServerReceived
            // 
            this._lServerReceived.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lServerReceived.AutoSize = true;
            this._lServerReceived.Location = new System.Drawing.Point(4, 49);
            this._lServerReceived.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServerReceived.Name = "_lServerReceived";
            this._lServerReceived.Size = new System.Drawing.Size(82, 13);
            this._lServerReceived.TabIndex = 3;
            this._lServerReceived.Text = "Server received";
            this._lServerReceived.Visible = false;
            // 
            // _eCcuMsgRetry
            // 
            this._eCcuMsgRetry.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCcuMsgRetry.Location = new System.Drawing.Point(586, 156);
            this._eCcuMsgRetry.Margin = new System.Windows.Forms.Padding(4);
            this._eCcuMsgRetry.Name = "_eCcuMsgRetry";
            this._eCcuMsgRetry.ReadOnly = true;
            this._eCcuMsgRetry.Size = new System.Drawing.Size(124, 20);
            this._eCcuMsgRetry.TabIndex = 28;
            this._eCcuMsgRetry.Visible = false;
            // 
            // _lCcuReceived
            // 
            this._lCcuReceived.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCcuReceived.AutoSize = true;
            this._lCcuReceived.Location = new System.Drawing.Point(394, 49);
            this._lCcuReceived.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCcuReceived.Name = "_lCcuReceived";
            this._lCcuReceived.Size = new System.Drawing.Size(73, 13);
            this._lCcuReceived.TabIndex = 18;
            this._lCcuReceived.Text = "CCU received";
            this._lCcuReceived.Visible = false;
            // 
            // _eCcuReceived
            // 
            this._eCcuReceived.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCcuReceived.Location = new System.Drawing.Point(586, 45);
            this._eCcuReceived.Margin = new System.Windows.Forms.Padding(4);
            this._eCcuReceived.Name = "_eCcuReceived";
            this._eCcuReceived.ReadOnly = true;
            this._eCcuReceived.Size = new System.Drawing.Size(124, 20);
            this._eCcuReceived.TabIndex = 19;
            this._eCcuReceived.Visible = false;
            // 
            // _bCcuResetReceived
            // 
            this._bCcuResetReceived.Location = new System.Drawing.Point(718, 41);
            this._bCcuResetReceived.Margin = new System.Windows.Forms.Padding(4);
            this._bCcuResetReceived.Name = "_bCcuResetReceived";
            this._bCcuResetReceived.Size = new System.Drawing.Size(94, 29);
            this._bCcuResetReceived.TabIndex = 20;
            this._bCcuResetReceived.Text = "Reset";
            this._bCcuResetReceived.UseVisualStyleBackColor = true;
            this._bCcuResetReceived.Visible = false;
            this._bCcuResetReceived.Click += new System.EventHandler(this._bCcuResetReceived_Click);
            // 
            // _lCcuReceivedError
            // 
            this._lCcuReceivedError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCcuReceivedError.AutoSize = true;
            this._lCcuReceivedError.Location = new System.Drawing.Point(394, 123);
            this._lCcuReceivedError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCcuReceivedError.Name = "_lCcuReceivedError";
            this._lCcuReceivedError.Size = new System.Drawing.Size(97, 13);
            this._lCcuReceivedError.TabIndex = 24;
            this._lCcuReceivedError.Text = "CCU received error";
            this._lCcuReceivedError.Visible = false;
            // 
            // _bCcuResetReceivedError
            // 
            this._bCcuResetReceivedError.Location = new System.Drawing.Point(718, 115);
            this._bCcuResetReceivedError.Margin = new System.Windows.Forms.Padding(4);
            this._bCcuResetReceivedError.Name = "_bCcuResetReceivedError";
            this._bCcuResetReceivedError.Size = new System.Drawing.Size(94, 29);
            this._bCcuResetReceivedError.TabIndex = 26;
            this._bCcuResetReceivedError.Text = "Reset";
            this._bCcuResetReceivedError.UseVisualStyleBackColor = true;
            this._bCcuResetReceivedError.Visible = false;
            this._bCcuResetReceivedError.Click += new System.EventHandler(this._bCcuResetReceivedError_Click);
            // 
            // _lServerDeserializeError
            // 
            this._lServerDeserializeError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lServerDeserializeError.AutoSize = true;
            this._lServerDeserializeError.Location = new System.Drawing.Point(4, 86);
            this._lServerDeserializeError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServerDeserializeError.Name = "_lServerDeserializeError";
            this._lServerDeserializeError.Size = new System.Drawing.Size(111, 13);
            this._lServerDeserializeError.TabIndex = 6;
            this._lServerDeserializeError.Text = "Sever deserialize error";
            this._lServerDeserializeError.Visible = false;
            // 
            // _eCcuReceivedError
            // 
            this._eCcuReceivedError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCcuReceivedError.Location = new System.Drawing.Point(586, 119);
            this._eCcuReceivedError.Margin = new System.Windows.Forms.Padding(4);
            this._eCcuReceivedError.Name = "_eCcuReceivedError";
            this._eCcuReceivedError.ReadOnly = true;
            this._eCcuReceivedError.Size = new System.Drawing.Size(124, 20);
            this._eCcuReceivedError.TabIndex = 25;
            this._eCcuReceivedError.Visible = false;
            // 
            // _lCcuDeserializeError
            // 
            this._lCcuDeserializeError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCcuDeserializeError.AutoSize = true;
            this._lCcuDeserializeError.Location = new System.Drawing.Point(394, 86);
            this._lCcuDeserializeError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCcuDeserializeError.Name = "_lCcuDeserializeError";
            this._lCcuDeserializeError.Size = new System.Drawing.Size(105, 13);
            this._lCcuDeserializeError.TabIndex = 21;
            this._lCcuDeserializeError.Text = "CCU deserialize error";
            this._lCcuDeserializeError.Visible = false;
            // 
            // _bServerResetDeserializeError
            // 
            this._bServerResetDeserializeError.Location = new System.Drawing.Point(292, 78);
            this._bServerResetDeserializeError.Margin = new System.Windows.Forms.Padding(4);
            this._bServerResetDeserializeError.Name = "_bServerResetDeserializeError";
            this._bServerResetDeserializeError.Size = new System.Drawing.Size(94, 29);
            this._bServerResetDeserializeError.TabIndex = 8;
            this._bServerResetDeserializeError.Text = "Reset";
            this._bServerResetDeserializeError.UseVisualStyleBackColor = true;
            this._bServerResetDeserializeError.Visible = false;
            this._bServerResetDeserializeError.Click += new System.EventHandler(this._bServerResetDeserializeError_Click);
            // 
            // _bCcuResetDeserializeError
            // 
            this._bCcuResetDeserializeError.Location = new System.Drawing.Point(718, 78);
            this._bCcuResetDeserializeError.Margin = new System.Windows.Forms.Padding(4);
            this._bCcuResetDeserializeError.Name = "_bCcuResetDeserializeError";
            this._bCcuResetDeserializeError.Size = new System.Drawing.Size(94, 29);
            this._bCcuResetDeserializeError.TabIndex = 23;
            this._bCcuResetDeserializeError.Text = "Reset";
            this._bCcuResetDeserializeError.UseVisualStyleBackColor = true;
            this._bCcuResetDeserializeError.Visible = false;
            this._bCcuResetDeserializeError.Click += new System.EventHandler(this._bCcuResetDeserializeError_Click);
            // 
            // _lServerMsgRetry
            // 
            this._lServerMsgRetry.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lServerMsgRetry.AutoSize = true;
            this._lServerMsgRetry.Location = new System.Drawing.Point(4, 160);
            this._lServerMsgRetry.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServerMsgRetry.Name = "_lServerMsgRetry";
            this._lServerMsgRetry.Size = new System.Drawing.Size(106, 13);
            this._lServerMsgRetry.TabIndex = 12;
            this._lServerMsgRetry.Text = "Server message retry";
            this._lServerMsgRetry.Visible = false;
            // 
            // _lServerReceivedError
            // 
            this._lServerReceivedError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lServerReceivedError.AutoSize = true;
            this._lServerReceivedError.Location = new System.Drawing.Point(4, 123);
            this._lServerReceivedError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServerReceivedError.Name = "_lServerReceivedError";
            this._lServerReceivedError.Size = new System.Drawing.Size(106, 13);
            this._lServerReceivedError.TabIndex = 9;
            this._lServerReceivedError.Text = "Server received error";
            this._lServerReceivedError.Visible = false;
            // 
            // _eCcuDeserializeError
            // 
            this._eCcuDeserializeError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCcuDeserializeError.Location = new System.Drawing.Point(586, 82);
            this._eCcuDeserializeError.Margin = new System.Windows.Forms.Padding(4);
            this._eCcuDeserializeError.Name = "_eCcuDeserializeError";
            this._eCcuDeserializeError.ReadOnly = true;
            this._eCcuDeserializeError.Size = new System.Drawing.Size(124, 20);
            this._eCcuDeserializeError.TabIndex = 22;
            this._eCcuDeserializeError.Visible = false;
            // 
            // _lServersNotStoredEvents
            // 
            this._lServersNotStoredEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lServersNotStoredEvents.AutoSize = true;
            this._lServersNotStoredEvents.Location = new System.Drawing.Point(4, 197);
            this._lServersNotStoredEvents.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServersNotStoredEvents.Name = "_lServersNotStoredEvents";
            this._lServersNotStoredEvents.Size = new System.Drawing.Size(128, 13);
            this._lServersNotStoredEvents.TabIndex = 32;
            this._lServersNotStoredEvents.Text = "Servers not stored events";
            // 
            // _lServersNotProcessedEvents
            // 
            this._lServersNotProcessedEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lServersNotProcessedEvents.AutoSize = true;
            this._lServersNotProcessedEvents.Location = new System.Drawing.Point(4, 229);
            this._lServersNotProcessedEvents.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServersNotProcessedEvents.Name = "_lServersNotProcessedEvents";
            this._lServersNotProcessedEvents.Size = new System.Drawing.Size(148, 13);
            this._lServersNotProcessedEvents.TabIndex = 37;
            this._lServersNotProcessedEvents.Text = "Servers not processed events";
            // 
            // _eServerDeserializeError
            // 
            this._eServerDeserializeError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eServerDeserializeError.Location = new System.Drawing.Point(160, 82);
            this._eServerDeserializeError.Margin = new System.Windows.Forms.Padding(4);
            this._eServerDeserializeError.Name = "_eServerDeserializeError";
            this._eServerDeserializeError.ReadOnly = true;
            this._eServerDeserializeError.Size = new System.Drawing.Size(124, 20);
            this._eServerDeserializeError.TabIndex = 7;
            this._eServerDeserializeError.Visible = false;
            // 
            // _eServerMsgRetry
            // 
            this._eServerMsgRetry.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eServerMsgRetry.Location = new System.Drawing.Point(160, 156);
            this._eServerMsgRetry.Margin = new System.Windows.Forms.Padding(4);
            this._eServerMsgRetry.Name = "_eServerMsgRetry";
            this._eServerMsgRetry.ReadOnly = true;
            this._eServerMsgRetry.Size = new System.Drawing.Size(124, 20);
            this._eServerMsgRetry.TabIndex = 13;
            this._eServerMsgRetry.Visible = false;
            // 
            // _eServerReceivedError
            // 
            this._eServerReceivedError.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eServerReceivedError.Location = new System.Drawing.Point(160, 119);
            this._eServerReceivedError.Margin = new System.Windows.Forms.Padding(4);
            this._eServerReceivedError.Name = "_eServerReceivedError";
            this._eServerReceivedError.ReadOnly = true;
            this._eServerReceivedError.Size = new System.Drawing.Size(124, 20);
            this._eServerReceivedError.TabIndex = 10;
            this._eServerReceivedError.Visible = false;
            // 
            // _eServersNotStoredEvents
            // 
            this._eServersNotStoredEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eServersNotStoredEvents.Location = new System.Drawing.Point(160, 193);
            this._eServersNotStoredEvents.Margin = new System.Windows.Forms.Padding(4);
            this._eServersNotStoredEvents.Name = "_eServersNotStoredEvents";
            this._eServersNotStoredEvents.ReadOnly = true;
            this._eServersNotStoredEvents.Size = new System.Drawing.Size(124, 20);
            this._eServersNotStoredEvents.TabIndex = 33;
            // 
            // _eServersNotProcessedEvents
            // 
            this._eServersNotProcessedEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eServersNotProcessedEvents.Location = new System.Drawing.Point(160, 226);
            this._eServersNotProcessedEvents.Margin = new System.Windows.Forms.Padding(4);
            this._eServersNotProcessedEvents.Name = "_eServersNotProcessedEvents";
            this._eServersNotProcessedEvents.ReadOnly = true;
            this._eServersNotProcessedEvents.Size = new System.Drawing.Size(124, 20);
            this._eServersNotProcessedEvents.TabIndex = 38;
            // 
            // _bResetServerMsgRetry
            // 
            this._bResetServerMsgRetry.Location = new System.Drawing.Point(292, 152);
            this._bResetServerMsgRetry.Margin = new System.Windows.Forms.Padding(4);
            this._bResetServerMsgRetry.Name = "_bResetServerMsgRetry";
            this._bResetServerMsgRetry.Size = new System.Drawing.Size(94, 29);
            this._bResetServerMsgRetry.TabIndex = 14;
            this._bResetServerMsgRetry.Text = "Reset";
            this._bResetServerMsgRetry.UseVisualStyleBackColor = true;
            this._bResetServerMsgRetry.Visible = false;
            this._bResetServerMsgRetry.Click += new System.EventHandler(this._bResetServerMsgRetry_Click);
            // 
            // _bServerResetReceivedError
            // 
            this._bServerResetReceivedError.Location = new System.Drawing.Point(292, 115);
            this._bServerResetReceivedError.Margin = new System.Windows.Forms.Padding(4);
            this._bServerResetReceivedError.Name = "_bServerResetReceivedError";
            this._bServerResetReceivedError.Size = new System.Drawing.Size(94, 29);
            this._bServerResetReceivedError.TabIndex = 11;
            this._bServerResetReceivedError.Text = "Reset";
            this._bServerResetReceivedError.UseVisualStyleBackColor = true;
            this._bServerResetReceivedError.Visible = false;
            this._bServerResetReceivedError.Click += new System.EventHandler(this._bServerResetReceivedError_Click);
            // 
            // _lCCUUnprocessedEvents
            // 
            this._lCCUUnprocessedEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCCUUnprocessedEvents.AutoSize = true;
            this._lCCUUnprocessedEvents.Location = new System.Drawing.Point(394, 295);
            this._lCCUUnprocessedEvents.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCCUUnprocessedEvents.Name = "_lCCUUnprocessedEvents";
            this._lCCUUnprocessedEvents.Size = new System.Drawing.Size(128, 13);
            this._lCCUUnprocessedEvents.TabIndex = 39;
            this._lCCUUnprocessedEvents.Text = "CCU unprocessed events";
            this._lCCUUnprocessedEvents.Visible = false;
            // 
            // _eCCUNotAcknowledgedEvents
            // 
            this._eCCUNotAcknowledgedEvents.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eCCUNotAcknowledgedEvents.Location = new System.Drawing.Point(586, 226);
            this._eCCUNotAcknowledgedEvents.Margin = new System.Windows.Forms.Padding(4);
            this._eCCUNotAcknowledgedEvents.Name = "_eCCUNotAcknowledgedEvents";
            this._eCCUNotAcknowledgedEvents.ReadOnly = true;
            this._eCCUNotAcknowledgedEvents.Size = new System.Drawing.Size(124, 20);
            this._eCCUNotAcknowledgedEvents.TabIndex = 36;
            // 
            // _bResetAll
            // 
            this._bResetAll.Location = new System.Drawing.Point(901, 24);
            this._bResetAll.Margin = new System.Windows.Forms.Padding(4);
            this._bResetAll.Name = "_bResetAll";
            this._bResetAll.Size = new System.Drawing.Size(116, 29);
            this._bResetAll.TabIndex = 30;
            this._bResetAll.Text = "Reset All";
            this._bResetAll.UseVisualStyleBackColor = true;
            this._bResetAll.Click += new System.EventHandler(this._bResetAll_Click);
            // 
            // _bRefreshCommunicationStatistic
            // 
            this._bRefreshCommunicationStatistic.Location = new System.Drawing.Point(924, 315);
            this._bRefreshCommunicationStatistic.Margin = new System.Windows.Forms.Padding(4);
            this._bRefreshCommunicationStatistic.Name = "_bRefreshCommunicationStatistic";
            this._bRefreshCommunicationStatistic.Size = new System.Drawing.Size(94, 29);
            this._bRefreshCommunicationStatistic.TabIndex = 31;
            this._bRefreshCommunicationStatistic.Text = "Refresh";
            this._bRefreshCommunicationStatistic.UseVisualStyleBackColor = true;
            this._bRefreshCommunicationStatistic.Click += new System.EventHandler(this.button5_Click);
            // 
            // _gbOtherStatistics
            // 
            this._gbOtherStatistics.Controls.Add(this._tlpPerformanceStatistics);
            this._gbOtherStatistics.Controls.Add(this._bRefreshAll);
            this._gbOtherStatistics.Location = new System.Drawing.Point(8, 8);
            this._gbOtherStatistics.Margin = new System.Windows.Forms.Padding(4);
            this._gbOtherStatistics.Name = "_gbOtherStatistics";
            this._gbOtherStatistics.Padding = new System.Windows.Forms.Padding(4);
            this._gbOtherStatistics.Size = new System.Drawing.Size(1025, 135);
            this._gbOtherStatistics.TabIndex = 4;
            this._gbOtherStatistics.TabStop = false;
            this._gbOtherStatistics.Text = "Performance CCU statistics";
            // 
            // _tlpPerformanceStatistics
            // 
            this._tlpPerformanceStatistics.ColumnCount = 4;
            this._tlpPerformanceStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpPerformanceStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpPerformanceStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpPerformanceStatistics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlpPerformanceStatistics.Controls.Add(this._eFreeTotalSDSpace, 0, 2);
            this._tlpPerformanceStatistics.Controls.Add(this._lFreeTotalSDSpace, 0, 2);
            this._tlpPerformanceStatistics.Controls.Add(this._lFreeMemory, 2, 0);
            this._tlpPerformanceStatistics.Controls.Add(this._lThreadCount, 0, 0);
            this._tlpPerformanceStatistics.Controls.Add(this._lTotalMemory, 2, 1);
            this._tlpPerformanceStatistics.Controls.Add(this._eTotalMemory, 3, 1);
            this._tlpPerformanceStatistics.Controls.Add(this._eThreads, 1, 0);
            this._tlpPerformanceStatistics.Controls.Add(this._eFreeMemory, 3, 0);
            this._tlpPerformanceStatistics.Controls.Add(this._lFreeTotalFlashSpace, 0, 1);
            this._tlpPerformanceStatistics.Controls.Add(this._eFreeTotalFlashSpace, 1, 1);
            this._tlpPerformanceStatistics.Controls.Add(this._eMemoryLoad, 3, 2);
            this._tlpPerformanceStatistics.Controls.Add(this._lMemoryLoad, 2, 2);
            this._tlpPerformanceStatistics.Location = new System.Drawing.Point(8, 24);
            this._tlpPerformanceStatistics.Margin = new System.Windows.Forms.Padding(4);
            this._tlpPerformanceStatistics.Name = "_tlpPerformanceStatistics";
            this._tlpPerformanceStatistics.RowCount = 3;
            this._tlpPerformanceStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpPerformanceStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpPerformanceStatistics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlpPerformanceStatistics.Size = new System.Drawing.Size(886, 84);
            this._tlpPerformanceStatistics.TabIndex = 6;
            // 
            // _eFreeTotalSDSpace
            // 
            this._eFreeTotalSDSpace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eFreeTotalSDSpace.Location = new System.Drawing.Point(128, 60);
            this._eFreeTotalSDSpace.Margin = new System.Windows.Forms.Padding(4);
            this._eFreeTotalSDSpace.Name = "_eFreeTotalSDSpace";
            this._eFreeTotalSDSpace.ReadOnly = true;
            this._eFreeTotalSDSpace.Size = new System.Drawing.Size(134, 20);
            this._eFreeTotalSDSpace.TabIndex = 7;
            // 
            // _lFreeTotalSDSpace
            // 
            this._lFreeTotalSDSpace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lFreeTotalSDSpace.AutoSize = true;
            this._lFreeTotalSDSpace.Location = new System.Drawing.Point(4, 63);
            this._lFreeTotalSDSpace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFreeTotalSDSpace.Name = "_lFreeTotalSDSpace";
            this._lFreeTotalSDSpace.Size = new System.Drawing.Size(106, 13);
            this._lFreeTotalSDSpace.TabIndex = 6;
            this._lFreeTotalSDSpace.Text = "SD space (free/total)";
            // 
            // _lFreeMemory
            // 
            this._lFreeMemory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lFreeMemory.AutoSize = true;
            this._lFreeMemory.Location = new System.Drawing.Point(270, 7);
            this._lFreeMemory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFreeMemory.Name = "_lFreeMemory";
            this._lFreeMemory.Size = new System.Drawing.Size(67, 13);
            this._lFreeMemory.TabIndex = 1;
            this._lFreeMemory.Text = "Free memory";
            // 
            // _lThreadCount
            // 
            this._lThreadCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lThreadCount.AutoSize = true;
            this._lThreadCount.Location = new System.Drawing.Point(4, 7);
            this._lThreadCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lThreadCount.Name = "_lThreadCount";
            this._lThreadCount.Size = new System.Drawing.Size(46, 13);
            this._lThreadCount.TabIndex = 1;
            this._lThreadCount.Text = "Threads";
            // 
            // _lTotalMemory
            // 
            this._lTotalMemory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lTotalMemory.AutoSize = true;
            this._lTotalMemory.Location = new System.Drawing.Point(270, 35);
            this._lTotalMemory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTotalMemory.Name = "_lTotalMemory";
            this._lTotalMemory.Size = new System.Drawing.Size(70, 13);
            this._lTotalMemory.TabIndex = 5;
            this._lTotalMemory.Text = "Total memory";
            // 
            // _eTotalMemory
            // 
            this._eTotalMemory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eTotalMemory.Location = new System.Drawing.Point(348, 32);
            this._eTotalMemory.Margin = new System.Windows.Forms.Padding(4);
            this._eTotalMemory.Name = "_eTotalMemory";
            this._eTotalMemory.ReadOnly = true;
            this._eTotalMemory.Size = new System.Drawing.Size(134, 20);
            this._eTotalMemory.TabIndex = 4;
            // 
            // _eThreads
            // 
            this._eThreads.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eThreads.Location = new System.Drawing.Point(128, 4);
            this._eThreads.Margin = new System.Windows.Forms.Padding(4);
            this._eThreads.Name = "_eThreads";
            this._eThreads.ReadOnly = true;
            this._eThreads.Size = new System.Drawing.Size(134, 20);
            this._eThreads.TabIndex = 0;
            // 
            // _eFreeMemory
            // 
            this._eFreeMemory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eFreeMemory.Location = new System.Drawing.Point(348, 4);
            this._eFreeMemory.Margin = new System.Windows.Forms.Padding(4);
            this._eFreeMemory.Name = "_eFreeMemory";
            this._eFreeMemory.ReadOnly = true;
            this._eFreeMemory.Size = new System.Drawing.Size(134, 20);
            this._eFreeMemory.TabIndex = 0;
            // 
            // _lFreeTotalFlashSpace
            // 
            this._lFreeTotalFlashSpace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lFreeTotalFlashSpace.AutoSize = true;
            this._lFreeTotalFlashSpace.Location = new System.Drawing.Point(4, 35);
            this._lFreeTotalFlashSpace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFreeTotalFlashSpace.Name = "_lFreeTotalFlashSpace";
            this._lFreeTotalFlashSpace.Size = new System.Drawing.Size(116, 13);
            this._lFreeTotalFlashSpace.TabIndex = 1;
            this._lFreeTotalFlashSpace.Text = "Flash space (free/total)";
            // 
            // _eFreeTotalFlashSpace
            // 
            this._eFreeTotalFlashSpace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eFreeTotalFlashSpace.Location = new System.Drawing.Point(128, 32);
            this._eFreeTotalFlashSpace.Margin = new System.Windows.Forms.Padding(4);
            this._eFreeTotalFlashSpace.Name = "_eFreeTotalFlashSpace";
            this._eFreeTotalFlashSpace.ReadOnly = true;
            this._eFreeTotalFlashSpace.Size = new System.Drawing.Size(134, 20);
            this._eFreeTotalFlashSpace.TabIndex = 0;
            // 
            // _eMemoryLoad
            // 
            this._eMemoryLoad.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._eMemoryLoad.Location = new System.Drawing.Point(348, 60);
            this._eMemoryLoad.Margin = new System.Windows.Forms.Padding(4);
            this._eMemoryLoad.Name = "_eMemoryLoad";
            this._eMemoryLoad.ReadOnly = true;
            this._eMemoryLoad.Size = new System.Drawing.Size(134, 20);
            this._eMemoryLoad.TabIndex = 4;
            // 
            // _lMemoryLoad
            // 
            this._lMemoryLoad.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lMemoryLoad.AutoSize = true;
            this._lMemoryLoad.Location = new System.Drawing.Point(270, 63);
            this._lMemoryLoad.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMemoryLoad.Name = "_lMemoryLoad";
            this._lMemoryLoad.Size = new System.Drawing.Size(67, 13);
            this._lMemoryLoad.TabIndex = 5;
            this._lMemoryLoad.Text = "Memory load";
            // 
            // _bRefreshAll
            // 
            this._bRefreshAll.Location = new System.Drawing.Point(924, 99);
            this._bRefreshAll.Margin = new System.Windows.Forms.Padding(4);
            this._bRefreshAll.Name = "_bRefreshAll";
            this._bRefreshAll.Size = new System.Drawing.Size(94, 29);
            this._bRefreshAll.TabIndex = 3;
            this._bRefreshAll.Text = "Refresh";
            this._bRefreshAll.UseVisualStyleBackColor = true;
            this._bRefreshAll.Click += new System.EventHandler(this._bRefreshAll_Click);
            // 
            // _tpThreadMap
            // 
            this._tpThreadMap.BackColor = System.Drawing.Color.Transparent;
            this._tpThreadMap.Controls.Add(this._tbThreadId);
            this._tpThreadMap.Controls.Add(this._lSearchByThreadId);
            this._tpThreadMap.Controls.Add(this._bRefresh3);
            this._tpThreadMap.Controls.Add(this._cdgvThreadMap);
            this._tpThreadMap.Location = new System.Drawing.Point(4, 22);
            this._tpThreadMap.Margin = new System.Windows.Forms.Padding(4);
            this._tpThreadMap.Name = "_tpThreadMap";
            this._tpThreadMap.Size = new System.Drawing.Size(1044, 496);
            this._tpThreadMap.TabIndex = 2;
            this._tpThreadMap.Text = "Thread map";
            this._tpThreadMap.UseVisualStyleBackColor = true;
            // 
            // _tbThreadId
            // 
            this._tbThreadId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._tbThreadId.Location = new System.Drawing.Point(113, 450);
            this._tbThreadId.Name = "_tbThreadId";
            this._tbThreadId.Size = new System.Drawing.Size(100, 20);
            this._tbThreadId.TabIndex = 3;
            this._tbThreadId.TextChanged += new System.EventHandler(this._tbThreadId_TextChanged);
            // 
            // _lSearchByThreadId
            // 
            this._lSearchByThreadId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lSearchByThreadId.AutoSize = true;
            this._lSearchByThreadId.Location = new System.Drawing.Point(3, 453);
            this._lSearchByThreadId.Name = "_lSearchByThreadId";
            this._lSearchByThreadId.Size = new System.Drawing.Size(104, 13);
            this._lSearchByThreadId.TabIndex = 2;
            this._lSearchByThreadId.Text = "Search by Thread Id";
            // 
            // _bRefresh3
            // 
            this._bRefresh3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh3.Location = new System.Drawing.Point(946, 446);
            this._bRefresh3.Margin = new System.Windows.Forms.Padding(4);
            this._bRefresh3.Name = "_bRefresh3";
            this._bRefresh3.Size = new System.Drawing.Size(94, 26);
            this._bRefresh3.TabIndex = 1;
            this._bRefresh3.Text = "Refresh";
            this._bRefresh3.UseVisualStyleBackColor = true;
            this._bRefresh3.Click += new System.EventHandler(this._bRefresh3_Click);
            // 
            // _cdgvThreadMap
            // 
            this._cdgvThreadMap.AllwaysRefreshOrder = false;
            this._cdgvThreadMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvThreadMap.CgpDataGridEvents = null;
            this._cdgvThreadMap.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvThreadMap.DataGrid.AllowUserToAddRows = false;
            this._cdgvThreadMap.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvThreadMap.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvThreadMap.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this._cdgvThreadMap.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvThreadMap.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvThreadMap.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvThreadMap.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvThreadMap.DataGrid.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvThreadMap.DataGrid.Name = "_dgvData";
            this._cdgvThreadMap.DataGrid.ReadOnly = true;
            this._cdgvThreadMap.DataGrid.RowHeadersVisible = false;
            this._cdgvThreadMap.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            this._cdgvThreadMap.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this._cdgvThreadMap.DataGrid.RowTemplate.Height = 24;
            this._cdgvThreadMap.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvThreadMap.DataGrid.Size = new System.Drawing.Size(1036, 435);
            this._cdgvThreadMap.DataGrid.TabIndex = 0;
            this._cdgvThreadMap.LocalizationHelper = null;
            this._cdgvThreadMap.Location = new System.Drawing.Point(4, 4);
            this._cdgvThreadMap.Margin = new System.Windows.Forms.Padding(5);
            this._cdgvThreadMap.Name = "_cdgvThreadMap";
            this._cdgvThreadMap.Size = new System.Drawing.Size(1036, 435);
            this._cdgvThreadMap.TabIndex = 0;
            // 
            // _tpOtherStatistics
            // 
            this._tpOtherStatistics.BackColor = System.Drawing.Color.Transparent;
            this._tpOtherStatistics.Controls.Add(this._gbCcuStartsCount);
            this._tpOtherStatistics.Controls.Add(this._gbCoprocessorVersion);
            this._tpOtherStatistics.Location = new System.Drawing.Point(4, 22);
            this._tpOtherStatistics.Margin = new System.Windows.Forms.Padding(4);
            this._tpOtherStatistics.Name = "_tpOtherStatistics";
            this._tpOtherStatistics.Padding = new System.Windows.Forms.Padding(4);
            this._tpOtherStatistics.Size = new System.Drawing.Size(1044, 496);
            this._tpOtherStatistics.TabIndex = 1;
            this._tpOtherStatistics.Text = "Other";
            this._tpOtherStatistics.UseVisualStyleBackColor = true;
            // 
            // _gbCcuStartsCount
            // 
            this._gbCcuStartsCount.Controls.Add(this._lCEUptime);
            this._gbCcuStartsCount.Controls.Add(this._eCEUptime);
            this._gbCcuStartsCount.Controls.Add(this._lCCUUptime);
            this._gbCcuStartsCount.Controls.Add(this._eCCUUptime);
            this._gbCcuStartsCount.Controls.Add(this._bCcuStartsCountReset);
            this._gbCcuStartsCount.Controls.Add(this._lCcuStartsCount);
            this._gbCcuStartsCount.Controls.Add(this._eCcuStartsCount);
            this._gbCcuStartsCount.Controls.Add(this._bCcuStartsCountRefresh);
            this._gbCcuStartsCount.Location = new System.Drawing.Point(8, 8);
            this._gbCcuStartsCount.Margin = new System.Windows.Forms.Padding(4);
            this._gbCcuStartsCount.Name = "_gbCcuStartsCount";
            this._gbCcuStartsCount.Padding = new System.Windows.Forms.Padding(4);
            this._gbCcuStartsCount.Size = new System.Drawing.Size(325, 159);
            this._gbCcuStartsCount.TabIndex = 1;
            this._gbCcuStartsCount.TabStop = false;
            this._gbCcuStartsCount.Text = "CCU starts count";
            // 
            // _lCEUptime
            // 
            this._lCEUptime.AutoSize = true;
            this._lCEUptime.Location = new System.Drawing.Point(8, 90);
            this._lCEUptime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCEUptime.Name = "_lCEUptime";
            this._lCEUptime.Size = new System.Drawing.Size(86, 13);
            this._lCEUptime.TabIndex = 6;
            this._lCEUptime.Text = "CE image uptime";
            // 
            // _eCEUptime
            // 
            this._eCEUptime.Location = new System.Drawing.Point(146, 86);
            this._eCEUptime.Margin = new System.Windows.Forms.Padding(4);
            this._eCEUptime.Name = "_eCEUptime";
            this._eCEUptime.ReadOnly = true;
            this._eCEUptime.Size = new System.Drawing.Size(165, 20);
            this._eCEUptime.TabIndex = 7;
            // 
            // _lCCUUptime
            // 
            this._lCCUUptime.AutoSize = true;
            this._lCCUUptime.Location = new System.Drawing.Point(8, 58);
            this._lCCUUptime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCCUUptime.Name = "_lCCUUptime";
            this._lCCUUptime.Size = new System.Drawing.Size(63, 13);
            this._lCCUUptime.TabIndex = 4;
            this._lCCUUptime.Text = "CCU uptime";
            // 
            // _eCCUUptime
            // 
            this._eCCUUptime.Location = new System.Drawing.Point(146, 54);
            this._eCCUUptime.Margin = new System.Windows.Forms.Padding(4);
            this._eCCUUptime.Name = "_eCCUUptime";
            this._eCCUUptime.ReadOnly = true;
            this._eCCUUptime.Size = new System.Drawing.Size(165, 20);
            this._eCCUUptime.TabIndex = 5;
            // 
            // _bCcuStartsCountReset
            // 
            this._bCcuStartsCountReset.Location = new System.Drawing.Point(11, 119);
            this._bCcuStartsCountReset.Margin = new System.Windows.Forms.Padding(4);
            this._bCcuStartsCountReset.Name = "_bCcuStartsCountReset";
            this._bCcuStartsCountReset.Size = new System.Drawing.Size(94, 29);
            this._bCcuStartsCountReset.TabIndex = 2;
            this._bCcuStartsCountReset.Text = "Reset";
            this._bCcuStartsCountReset.UseVisualStyleBackColor = true;
            this._bCcuStartsCountReset.Click += new System.EventHandler(this._bCcuStartsCountReset_Click);
            // 
            // _lCcuStartsCount
            // 
            this._lCcuStartsCount.AutoSize = true;
            this._lCcuStartsCount.Location = new System.Drawing.Point(8, 25);
            this._lCcuStartsCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCcuStartsCount.Name = "_lCcuStartsCount";
            this._lCcuStartsCount.Size = new System.Drawing.Size(35, 13);
            this._lCcuStartsCount.TabIndex = 0;
            this._lCcuStartsCount.Text = "Count";
            // 
            // _eCcuStartsCount
            // 
            this._eCcuStartsCount.Location = new System.Drawing.Point(146, 21);
            this._eCcuStartsCount.Margin = new System.Windows.Forms.Padding(4);
            this._eCcuStartsCount.Name = "_eCcuStartsCount";
            this._eCcuStartsCount.ReadOnly = true;
            this._eCcuStartsCount.Size = new System.Drawing.Size(165, 20);
            this._eCcuStartsCount.TabIndex = 1;
            // 
            // _bCcuStartsCountRefresh
            // 
            this._bCcuStartsCountRefresh.Location = new System.Drawing.Point(219, 119);
            this._bCcuStartsCountRefresh.Margin = new System.Windows.Forms.Padding(4);
            this._bCcuStartsCountRefresh.Name = "_bCcuStartsCountRefresh";
            this._bCcuStartsCountRefresh.Size = new System.Drawing.Size(94, 29);
            this._bCcuStartsCountRefresh.TabIndex = 3;
            this._bCcuStartsCountRefresh.Text = "Refresh";
            this._bCcuStartsCountRefresh.UseVisualStyleBackColor = true;
            this._bCcuStartsCountRefresh.Click += new System.EventHandler(this.RefreshCcuStartsCountClick);
            // 
            // _gbCoprocessorVersion
            // 
            this._gbCoprocessorVersion.Controls.Add(this._bRefresh9);
            this._gbCoprocessorVersion.Controls.Add(this._eCoprocessorUpgradeResult);
            this._gbCoprocessorVersion.Controls.Add(this._eCoprocessorActualBuildNumber);
            this._gbCoprocessorVersion.Controls.Add(this._lUpgradeResult);
            this._gbCoprocessorVersion.Controls.Add(this._lActualBuildNumber);
            this._gbCoprocessorVersion.Location = new System.Drawing.Point(340, 8);
            this._gbCoprocessorVersion.Margin = new System.Windows.Forms.Padding(4);
            this._gbCoprocessorVersion.Name = "_gbCoprocessorVersion";
            this._gbCoprocessorVersion.Padding = new System.Windows.Forms.Padding(4);
            this._gbCoprocessorVersion.Size = new System.Drawing.Size(692, 159);
            this._gbCoprocessorVersion.TabIndex = 2;
            this._gbCoprocessorVersion.TabStop = false;
            this._gbCoprocessorVersion.Text = "Coprocessor version";
            // 
            // _bRefresh9
            // 
            this._bRefresh9.Location = new System.Drawing.Point(591, 119);
            this._bRefresh9.Margin = new System.Windows.Forms.Padding(4);
            this._bRefresh9.Name = "_bRefresh9";
            this._bRefresh9.Size = new System.Drawing.Size(94, 29);
            this._bRefresh9.TabIndex = 4;
            this._bRefresh9.Text = "Refresh";
            this._bRefresh9.UseVisualStyleBackColor = true;
            this._bRefresh9.Click += new System.EventHandler(this._bRefresh9_Click);
            // 
            // _eCoprocessorUpgradeResult
            // 
            this._eCoprocessorUpgradeResult.Location = new System.Drawing.Point(200, 56);
            this._eCoprocessorUpgradeResult.Margin = new System.Windows.Forms.Padding(4);
            this._eCoprocessorUpgradeResult.Name = "_eCoprocessorUpgradeResult";
            this._eCoprocessorUpgradeResult.ReadOnly = true;
            this._eCoprocessorUpgradeResult.Size = new System.Drawing.Size(484, 20);
            this._eCoprocessorUpgradeResult.TabIndex = 3;
            // 
            // _eCoprocessorActualBuildNumber
            // 
            this._eCoprocessorActualBuildNumber.Location = new System.Drawing.Point(200, 24);
            this._eCoprocessorActualBuildNumber.Margin = new System.Windows.Forms.Padding(4);
            this._eCoprocessorActualBuildNumber.Name = "_eCoprocessorActualBuildNumber";
            this._eCoprocessorActualBuildNumber.ReadOnly = true;
            this._eCoprocessorActualBuildNumber.Size = new System.Drawing.Size(484, 20);
            this._eCoprocessorActualBuildNumber.TabIndex = 1;
            // 
            // _lUpgradeResult
            // 
            this._lUpgradeResult.AutoSize = true;
            this._lUpgradeResult.Location = new System.Drawing.Point(8, 60);
            this._lUpgradeResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lUpgradeResult.Name = "_lUpgradeResult";
            this._lUpgradeResult.Size = new System.Drawing.Size(76, 13);
            this._lUpgradeResult.TabIndex = 2;
            this._lUpgradeResult.Text = "Upgrade result";
            // 
            // _lActualBuildNumber
            // 
            this._lActualBuildNumber.AutoSize = true;
            this._lActualBuildNumber.Location = new System.Drawing.Point(8, 28);
            this._lActualBuildNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lActualBuildNumber.Name = "_lActualBuildNumber";
            this._lActualBuildNumber.Size = new System.Drawing.Size(100, 13);
            this._lActualBuildNumber.TabIndex = 0;
            this._lActualBuildNumber.Text = "Actual build number";
            // 
            // _tpMemory
            // 
            this._tpMemory.BackColor = System.Drawing.Color.Transparent;
            this._tpMemory.Controls.Add(this._cdgvMemory);
            this._tpMemory.Controls.Add(this._bRefresh4);
            this._tpMemory.Location = new System.Drawing.Point(4, 22);
            this._tpMemory.Name = "_tpMemory";
            this._tpMemory.Size = new System.Drawing.Size(1044, 496);
            this._tpMemory.TabIndex = 3;
            this._tpMemory.Text = "Memory";
            this._tpMemory.UseVisualStyleBackColor = true;
            // 
            // _cdgvMemory
            // 
            this._cdgvMemory.AllwaysRefreshOrder = false;
            this._cdgvMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvMemory.AutoSize = true;
            this._cdgvMemory.CgpDataGridEvents = null;
            this._cdgvMemory.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvMemory.DataGrid.AllowUserToAddRows = false;
            this._cdgvMemory.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvMemory.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvMemory.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this._cdgvMemory.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvMemory.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvMemory.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvMemory.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvMemory.DataGrid.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvMemory.DataGrid.Name = "_dgvData";
            this._cdgvMemory.DataGrid.ReadOnly = true;
            this._cdgvMemory.DataGrid.RowHeadersVisible = false;
            this._cdgvMemory.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
            this._cdgvMemory.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this._cdgvMemory.DataGrid.RowTemplate.Height = 24;
            this._cdgvMemory.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvMemory.DataGrid.Size = new System.Drawing.Size(1036, 453);
            this._cdgvMemory.DataGrid.TabIndex = 0;
            this._cdgvMemory.LocalizationHelper = null;
            this._cdgvMemory.Location = new System.Drawing.Point(4, 4);
            this._cdgvMemory.Margin = new System.Windows.Forms.Padding(5);
            this._cdgvMemory.Name = "_cdgvMemory";
            this._cdgvMemory.Size = new System.Drawing.Size(1036, 453);
            this._cdgvMemory.TabIndex = 1;
            // 
            // _bRefresh4
            // 
            this._bRefresh4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh4.Location = new System.Drawing.Point(946, 466);
            this._bRefresh4.Margin = new System.Windows.Forms.Padding(4);
            this._bRefresh4.Name = "_bRefresh4";
            this._bRefresh4.Size = new System.Drawing.Size(94, 26);
            this._bRefresh4.TabIndex = 2;
            this._bRefresh4.Text = "Refresh";
            this._bRefresh4.UseVisualStyleBackColor = true;
            this._bRefresh4.Click += new System.EventHandler(this._bRefresh4_Click);
            // 
            // _tpProcesses
            // 
            this._tpProcesses.Controls.Add(this._panelCmd);
            this._tpProcesses.Controls.Add(this._bStartExplicityCKM);
            this._tpProcesses.Controls.Add(this._bStartImplicityCKM);
            this._tpProcesses.Controls.Add(this._bStopCKM);
            this._tpProcesses.Controls.Add(this._lbCmdResults);
            this._tpProcesses.Location = new System.Drawing.Point(4, 22);
            this._tpProcesses.Name = "_tpProcesses";
            this._tpProcesses.Size = new System.Drawing.Size(1044, 496);
            this._tpProcesses.TabIndex = 4;
            this._tpProcesses.Text = "Processes";
            this._tpProcesses.UseVisualStyleBackColor = true;
            // 
            // _lCommandLine
            // 
            this._lCommandLine.AutoSize = true;
            this._lCommandLine.Location = new System.Drawing.Point(8, 6);
            this._lCommandLine.Name = "_lCommandLine";
            this._lCommandLine.Size = new System.Drawing.Size(74, 13);
            this._lCommandLine.TabIndex = 6;
            this._lCommandLine.Text = "CommandLine";
            this._lCommandLine.Visible = false;
            // 
            // _bStartExplicityCKM
            // 
            this._bStartExplicityCKM.Location = new System.Drawing.Point(263, 67);
            this._bStartExplicityCKM.Name = "_bStartExplicityCKM";
            this._bStartExplicityCKM.Size = new System.Drawing.Size(115, 40);
            this._bStartExplicityCKM.TabIndex = 5;
            this._bStartExplicityCKM.Text = "StartExplicityCKM";
            this._bStartExplicityCKM.UseVisualStyleBackColor = true;
            this._bStartExplicityCKM.Click += new System.EventHandler(this._bStartExplicityCKM_Click);
            // 
            // _bStartImplicityCKM
            // 
            this._bStartImplicityCKM.Location = new System.Drawing.Point(142, 67);
            this._bStartImplicityCKM.Name = "_bStartImplicityCKM";
            this._bStartImplicityCKM.Size = new System.Drawing.Size(115, 40);
            this._bStartImplicityCKM.TabIndex = 4;
            this._bStartImplicityCKM.Text = "StartImplicityCKM";
            this._bStartImplicityCKM.UseVisualStyleBackColor = true;
            this._bStartImplicityCKM.Click += new System.EventHandler(this._bStartImplicityCKM_Click);
            // 
            // _bStopCKM
            // 
            this._bStopCKM.Location = new System.Drawing.Point(21, 67);
            this._bStopCKM.Name = "_bStopCKM";
            this._bStopCKM.Size = new System.Drawing.Size(115, 40);
            this._bStopCKM.TabIndex = 3;
            this._bStopCKM.Text = "StopCKM";
            this._bStopCKM.UseVisualStyleBackColor = true;
            this._bStopCKM.Click += new System.EventHandler(this._bStopCKM_Click);
            // 
            // _lbCmdResults
            // 
            this._lbCmdResults.FormattingEnabled = true;
            this._lbCmdResults.Location = new System.Drawing.Point(21, 120);
            this._lbCmdResults.Name = "_lbCmdResults";
            this._lbCmdResults.Size = new System.Drawing.Size(502, 121);
            this._lbCmdResults.TabIndex = 2;
            // 
            // _tbCommandLine
            // 
            this._tbCommandLine.Location = new System.Drawing.Point(11, 24);
            this._tbCommandLine.Name = "_tbCommandLine";
            this._tbCommandLine.Size = new System.Drawing.Size(381, 20);
            this._tbCommandLine.TabIndex = 1;
            this._tbCommandLine.Visible = false;
            // 
            // _bRunCmd
            // 
            this._bRunCmd.Location = new System.Drawing.Point(398, 22);
            this._bRunCmd.Name = "_bRunCmd";
            this._bRunCmd.Size = new System.Drawing.Size(116, 23);
            this._bRunCmd.TabIndex = 0;
            this._bRunCmd.Text = "RunCmd";
            this._bRunCmd.UseVisualStyleBackColor = true;
            this._bRunCmd.Visible = false;
            this._bRunCmd.Click += new System.EventHandler(this._bRunCmd_Click);
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh1);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 40);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(4);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(1060, 530);
            this._tpUserFolders.TabIndex = 11;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh1
            // 
            this._bRefresh1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh1.Location = new System.Drawing.Point(960, 394);
            this._bRefresh1.Margin = new System.Windows.Forms.Padding(4);
            this._bRefresh1.Name = "_bRefresh1";
            this._bRefresh1.Size = new System.Drawing.Size(94, 29);
            this._bRefresh1.TabIndex = 2;
            this._bRefresh1.Text = "Refresh";
            this._bRefresh1.UseVisualStyleBackColor = true;
            this._bRefresh1.Click += new System.EventHandler(this._bRefresh1_Click);
            // 
            // _lbUserFolders
            // 
            this._lbUserFolders.AllowDrop = false;
            this._lbUserFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbUserFolders.BackColor = System.Drawing.SystemColors.Info;
            this._lbUserFolders.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._lbUserFolders.FormattingEnabled = true;
            this._lbUserFolders.ImageList = null;
            this._lbUserFolders.Location = new System.Drawing.Point(4, 2);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(1050, 303);
            this._lbUserFolders.TabIndex = 30;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 40);
            this._tpReferencedBy.Margin = new System.Windows.Forms.Padding(4);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(4);
            this._tpReferencedBy.Size = new System.Drawing.Size(1060, 530);
            this._tpReferencedBy.TabIndex = 10;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 40);
            this._tpDescription.Margin = new System.Windows.Forms.Padding(4);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(4);
            this._tpDescription.Size = new System.Drawing.Size(1060, 530);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(4, 4);
            this._eDescription.Margin = new System.Windows.Forms.Padding(4);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(1052, 522);
            this._eDescription.TabIndex = 2;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _tpTesting
            // 
            this._tpTesting.BackColor = System.Drawing.SystemColors.Control;
            this._tpTesting.Controls.Add(this._gbSimulationAARights);
            this._tpTesting.Controls.Add(this._gbTestRoutineDCU);
            this._tpTesting.Controls.Add(this._gbTimeZonesDailyPlansState);
            this._tpTesting.Controls.Add(this._gbSimulationCardSwiped);
            this._tpTesting.Location = new System.Drawing.Point(4, 40);
            this._tpTesting.Margin = new System.Windows.Forms.Padding(4);
            this._tpTesting.Name = "_tpTesting";
            this._tpTesting.Size = new System.Drawing.Size(1060, 530);
            this._tpTesting.TabIndex = 17;
            this._tpTesting.Text = "Testing";
            // 
            // _gbSimulationAARights
            // 
            this._gbSimulationAARights.Controls.Add(this._lResultsAADB);
            this._gbSimulationAARights.Controls.Add(this._rtbResultsDB);
            this._gbSimulationAARights.Controls.Add(this._bTestAlarmAreaSimulation);
            this._gbSimulationAARights.Controls.Add(this._lResultsCCU);
            this._gbSimulationAARights.Controls.Add(this._tbmAlarmArea);
            this._gbSimulationAARights.Controls.Add(this._lPerson);
            this._gbSimulationAARights.Controls.Add(this._lAlarmArea);
            this._gbSimulationAARights.Controls.Add(this._tbmPerson);
            this._gbSimulationAARights.Controls.Add(this._rtbResultsCCU);
            this._gbSimulationAARights.Location = new System.Drawing.Point(4, 221);
            this._gbSimulationAARights.Margin = new System.Windows.Forms.Padding(4);
            this._gbSimulationAARights.Name = "_gbSimulationAARights";
            this._gbSimulationAARights.Padding = new System.Windows.Forms.Padding(4);
            this._gbSimulationAARights.Size = new System.Drawing.Size(1042, 138);
            this._gbSimulationAARights.TabIndex = 6;
            this._gbSimulationAARights.TabStop = false;
            this._gbSimulationAARights.Text = "Simulation alarm area rights";
            // 
            // _lResultsAADB
            // 
            this._lResultsAADB.AutoSize = true;
            this._lResultsAADB.Location = new System.Drawing.Point(634, 12);
            this._lResultsAADB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lResultsAADB.Name = "_lResultsAADB";
            this._lResultsAADB.Size = new System.Drawing.Size(60, 13);
            this._lResultsAADB.TabIndex = 26;
            this._lResultsAADB.Text = "Results DB";
            // 
            // _rtbResultsDB
            // 
            this._rtbResultsDB.Location = new System.Drawing.Point(638, 32);
            this._rtbResultsDB.Margin = new System.Windows.Forms.Padding(4);
            this._rtbResultsDB.Name = "_rtbResultsDB";
            this._rtbResultsDB.Size = new System.Drawing.Size(294, 94);
            this._rtbResultsDB.TabIndex = 25;
            this._rtbResultsDB.Text = "";
            // 
            // _bTestAlarmAreaSimulation
            // 
            this._bTestAlarmAreaSimulation.Location = new System.Drawing.Point(940, 32);
            this._bTestAlarmAreaSimulation.Margin = new System.Windows.Forms.Padding(4);
            this._bTestAlarmAreaSimulation.Name = "_bTestAlarmAreaSimulation";
            this._bTestAlarmAreaSimulation.Size = new System.Drawing.Size(94, 29);
            this._bTestAlarmAreaSimulation.TabIndex = 24;
            this._bTestAlarmAreaSimulation.Text = "Test";
            this._bTestAlarmAreaSimulation.UseVisualStyleBackColor = true;
            this._bTestAlarmAreaSimulation.Click += new System.EventHandler(this._bTestAlarmAreaSimulation_Click);
            // 
            // _lResultsCCU
            // 
            this._lResultsCCU.AutoSize = true;
            this._lResultsCCU.Location = new System.Drawing.Point(305, 12);
            this._lResultsCCU.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lResultsCCU.Name = "_lResultsCCU";
            this._lResultsCCU.Size = new System.Drawing.Size(42, 13);
            this._lResultsCCU.TabIndex = 23;
            this._lResultsCCU.Text = "Results";
            // 
            // _tbmAlarmArea
            // 
            this._tbmAlarmArea.AllowDrop = true;
            this._tbmAlarmArea.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmAlarmArea.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmArea.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmAlarmArea.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmAlarmArea.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmArea.Button.Image")));
            this._tbmAlarmArea.Button.Location = new System.Drawing.Point(208, 0);
            this._tbmAlarmArea.Button.Name = "_bMenu";
            this._tbmAlarmArea.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmAlarmArea.Button.TabIndex = 3;
            this._tbmAlarmArea.Button.UseVisualStyleBackColor = false;
            this._tbmAlarmArea.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmAlarmArea.ButtonDefaultBehaviour = true;
            this._tbmAlarmArea.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmAlarmArea.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmArea.ButtonImage")));
            // 
            // 
            // 
            this._tbmAlarmArea.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify4});
            this._tbmAlarmArea.ButtonPopupMenu.Name = "";
            this._tbmAlarmArea.ButtonPopupMenu.Size = new System.Drawing.Size(113, 26);
            this._tbmAlarmArea.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmAlarmArea.ButtonShowImage = true;
            this._tbmAlarmArea.ButtonSizeHeight = 20;
            this._tbmAlarmArea.ButtonSizeWidth = 20;
            this._tbmAlarmArea.ButtonText = "";
            this._tbmAlarmArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmArea.HoverTime = 500;
            // 
            // 
            // 
            this._tbmAlarmArea.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmArea.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmAlarmArea.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmAlarmArea.ImageTextBox.ContextMenuStrip = this._tbmAlarmArea.ButtonPopupMenu;
            this._tbmAlarmArea.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmArea.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmArea.ImageTextBox.Image")));
            this._tbmAlarmArea.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmAlarmArea.ImageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmAlarmArea.ImageTextBox.Name = "_textBox";
            this._tbmAlarmArea.ImageTextBox.NoTextNoImage = true;
            this._tbmAlarmArea.ImageTextBox.ReadOnly = false;
            this._tbmAlarmArea.ImageTextBox.Size = new System.Drawing.Size(208, 20);
            this._tbmAlarmArea.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmAlarmArea.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmAlarmArea.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmAlarmArea.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmAlarmArea.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmAlarmArea.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmAlarmArea.ImageTextBox.TextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmAlarmArea.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmAlarmArea.ImageTextBox.TextBox.Size = new System.Drawing.Size(206, 13);
            this._tbmAlarmArea.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmAlarmArea.ImageTextBox.UseImage = true;
            this._tbmAlarmArea.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmAlarmArea_ImageTextBox_DoubleClick);
            this._tbmAlarmArea.Location = new System.Drawing.Point(11, 84);
            this._tbmAlarmArea.Margin = new System.Windows.Forms.Padding(4);
            this._tbmAlarmArea.MaximumSize = new System.Drawing.Size(1500, 69);
            this._tbmAlarmArea.MinimumSize = new System.Drawing.Size(38, 28);
            this._tbmAlarmArea.Name = "_tbmAlarmArea";
            this._tbmAlarmArea.Size = new System.Drawing.Size(228, 22);
            this._tbmAlarmArea.TabIndex = 22;
            this._tbmAlarmArea.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmAlarmArea.TextImage")));
            this._tbmAlarmArea.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmAlarmArea_DragOver);
            this._tbmAlarmArea.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmAlarmArea_DragDrop);
            this._tbmAlarmArea.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmAlarmArea_ButtonPopupMenuItemClick);
            // 
            // _tsiModify4
            // 
            this._tsiModify4.Name = "_tsiModify4";
            this._tsiModify4.Size = new System.Drawing.Size(112, 22);
            this._tsiModify4.Text = "Modify";
            // 
            // _lPerson
            // 
            this._lPerson.AutoSize = true;
            this._lPerson.Location = new System.Drawing.Point(8, 15);
            this._lPerson.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPerson.Name = "_lPerson";
            this._lPerson.Size = new System.Drawing.Size(40, 13);
            this._lPerson.TabIndex = 21;
            this._lPerson.Text = "Person";
            // 
            // _lAlarmArea
            // 
            this._lAlarmArea.AutoSize = true;
            this._lAlarmArea.Location = new System.Drawing.Point(8, 66);
            this._lAlarmArea.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAlarmArea.Name = "_lAlarmArea";
            this._lAlarmArea.Size = new System.Drawing.Size(57, 13);
            this._lAlarmArea.TabIndex = 20;
            this._lAlarmArea.Text = "Alarm area";
            // 
            // _tbmPerson
            // 
            this._tbmPerson.AllowDrop = true;
            this._tbmPerson.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmPerson.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmPerson.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmPerson.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.Button.Image")));
            this._tbmPerson.Button.Location = new System.Drawing.Point(208, 0);
            this._tbmPerson.Button.Name = "_bMenu";
            this._tbmPerson.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmPerson.Button.TabIndex = 3;
            this._tbmPerson.Button.UseVisualStyleBackColor = false;
            this._tbmPerson.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmPerson.ButtonDefaultBehaviour = true;
            this._tbmPerson.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmPerson.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.ButtonImage")));
            // 
            // 
            // 
            this._tbmPerson.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify3});
            this._tbmPerson.ButtonPopupMenu.Name = "";
            this._tbmPerson.ButtonPopupMenu.Size = new System.Drawing.Size(113, 26);
            this._tbmPerson.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmPerson.ButtonShowImage = true;
            this._tbmPerson.ButtonSizeHeight = 20;
            this._tbmPerson.ButtonSizeWidth = 20;
            this._tbmPerson.ButtonText = "";
            this._tbmPerson.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPerson.HoverTime = 500;
            // 
            // 
            // 
            this._tbmPerson.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmPerson.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmPerson.ImageTextBox.ContextMenuStrip = this._tbmPerson.ButtonPopupMenu;
            this._tbmPerson.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPerson.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.ImageTextBox.Image")));
            this._tbmPerson.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmPerson.ImageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmPerson.ImageTextBox.Name = "_textBox";
            this._tbmPerson.ImageTextBox.NoTextNoImage = true;
            this._tbmPerson.ImageTextBox.ReadOnly = false;
            this._tbmPerson.ImageTextBox.Size = new System.Drawing.Size(208, 20);
            this._tbmPerson.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmPerson.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmPerson.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmPerson.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPerson.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmPerson.ImageTextBox.TextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmPerson.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmPerson.ImageTextBox.TextBox.Size = new System.Drawing.Size(206, 13);
            this._tbmPerson.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmPerson.ImageTextBox.UseImage = true;
            this._tbmPerson.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmPerson_ImageTextBox_DoubleClick);
            this._tbmPerson.Location = new System.Drawing.Point(11, 35);
            this._tbmPerson.Margin = new System.Windows.Forms.Padding(4);
            this._tbmPerson.MaximumSize = new System.Drawing.Size(1500, 69);
            this._tbmPerson.MinimumSize = new System.Drawing.Size(38, 28);
            this._tbmPerson.Name = "_tbmPerson";
            this._tbmPerson.Size = new System.Drawing.Size(228, 22);
            this._tbmPerson.TabIndex = 19;
            this._tbmPerson.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.TextImage")));
            this._tbmPerson.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmPerson_DragOver);
            this._tbmPerson.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmPerson_DragDrop);
            this._tbmPerson.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmPerson_ButtonPopupMenuItemClick);
            // 
            // _tsiModify3
            // 
            this._tsiModify3.Name = "_tsiModify3";
            this._tsiModify3.Size = new System.Drawing.Size(112, 22);
            this._tsiModify3.Text = "Modify";
            // 
            // _rtbResultsCCU
            // 
            this._rtbResultsCCU.Location = new System.Drawing.Point(309, 32);
            this._rtbResultsCCU.Margin = new System.Windows.Forms.Padding(4);
            this._rtbResultsCCU.Name = "_rtbResultsCCU";
            this._rtbResultsCCU.Size = new System.Drawing.Size(320, 94);
            this._rtbResultsCCU.TabIndex = 17;
            this._rtbResultsCCU.Text = "";
            this._rtbResultsCCU.TextChanged += new System.EventHandler(this._rtbResults_TextChanged);
            // 
            // _gbTestRoutineDCU
            // 
            this._gbTestRoutineDCU.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbTestRoutineDCU.Controls.Add(this._bRefreshTestDcu);
            this._gbTestRoutineDCU.Controls.Add(this._bTestDcu);
            this._gbTestRoutineDCU.Controls.Add(this._cbSelectAllToggleADC);
            this._gbTestRoutineDCU.Controls.Add(this._cbSelectAllToggleCard);
            this._gbTestRoutineDCU.Controls.Add(this._dgvTestDcu);
            this._gbTestRoutineDCU.Location = new System.Drawing.Point(4, 366);
            this._gbTestRoutineDCU.Margin = new System.Windows.Forms.Padding(4);
            this._gbTestRoutineDCU.Name = "_gbTestRoutineDCU";
            this._gbTestRoutineDCU.Padding = new System.Windows.Forms.Padding(4);
            this._gbTestRoutineDCU.Size = new System.Drawing.Size(1054, 156);
            this._gbTestRoutineDCU.TabIndex = 5;
            this._gbTestRoutineDCU.TabStop = false;
            this._gbTestRoutineDCU.Text = "Test routine DCU";
            // 
            // _bRefreshTestDcu
            // 
            this._bRefreshTestDcu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefreshTestDcu.Location = new System.Drawing.Point(845, 116);
            this._bRefreshTestDcu.Margin = new System.Windows.Forms.Padding(4);
            this._bRefreshTestDcu.Name = "_bRefreshTestDcu";
            this._bRefreshTestDcu.Size = new System.Drawing.Size(94, 29);
            this._bRefreshTestDcu.TabIndex = 4;
            this._bRefreshTestDcu.Text = "Refresh";
            this._bRefreshTestDcu.UseVisualStyleBackColor = true;
            this._bRefreshTestDcu.Click += new System.EventHandler(this._bRefreshTestDcu_Click);
            // 
            // _bTestDcu
            // 
            this._bTestDcu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bTestDcu.Location = new System.Drawing.Point(946, 116);
            this._bTestDcu.Margin = new System.Windows.Forms.Padding(4);
            this._bTestDcu.Name = "_bTestDcu";
            this._bTestDcu.Size = new System.Drawing.Size(94, 29);
            this._bTestDcu.TabIndex = 3;
            this._bTestDcu.Text = "Run test";
            this._bTestDcu.UseVisualStyleBackColor = true;
            this._bTestDcu.Click += new System.EventHandler(this._bTestDcu_Click);
            // 
            // _cbSelectAllToggleADC
            // 
            this._cbSelectAllToggleADC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._cbSelectAllToggleADC.AutoSize = true;
            this._cbSelectAllToggleADC.Location = new System.Drawing.Point(8, 128);
            this._cbSelectAllToggleADC.Margin = new System.Windows.Forms.Padding(4);
            this._cbSelectAllToggleADC.Name = "_cbSelectAllToggleADC";
            this._cbSelectAllToggleADC.Size = new System.Drawing.Size(175, 17);
            this._cbSelectAllToggleADC.TabIndex = 2;
            this._cbSelectAllToggleADC.Text = "Select/unselect all Toggle ADC";
            this._cbSelectAllToggleADC.UseVisualStyleBackColor = true;
            this._cbSelectAllToggleADC.CheckedChanged += new System.EventHandler(this._cbSelectAllToggleADC_CheckedChanged);
            // 
            // _cbSelectAllToggleCard
            // 
            this._cbSelectAllToggleCard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._cbSelectAllToggleCard.AutoSize = true;
            this._cbSelectAllToggleCard.Location = new System.Drawing.Point(8, 99);
            this._cbSelectAllToggleCard.Margin = new System.Windows.Forms.Padding(4);
            this._cbSelectAllToggleCard.Name = "_cbSelectAllToggleCard";
            this._cbSelectAllToggleCard.Size = new System.Drawing.Size(175, 17);
            this._cbSelectAllToggleCard.TabIndex = 1;
            this._cbSelectAllToggleCard.Text = "Select/unselect all Toggle Card";
            this._cbSelectAllToggleCard.UseVisualStyleBackColor = true;
            this._cbSelectAllToggleCard.CheckedChanged += new System.EventHandler(this._cbSelectAllToggleCard_CheckedChanged);
            // 
            // _dgvTestDcu
            // 
            this._dgvTestDcu.AllowUserToAddRows = false;
            this._dgvTestDcu.AllowUserToDeleteRows = false;
            this._dgvTestDcu.AllowUserToResizeRows = false;
            this._dgvTestDcu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvTestDcu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvTestDcu.Location = new System.Drawing.Point(8, 24);
            this._dgvTestDcu.Margin = new System.Windows.Forms.Padding(4);
            this._dgvTestDcu.MultiSelect = false;
            this._dgvTestDcu.Name = "_dgvTestDcu";
            this._dgvTestDcu.RowHeadersVisible = false;
            this._dgvTestDcu.RowTemplate.Height = 24;
            this._dgvTestDcu.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvTestDcu.Size = new System.Drawing.Size(1032, 64);
            this._dgvTestDcu.TabIndex = 0;
            // 
            // _gbTimeZonesDailyPlansState
            // 
            this._gbTimeZonesDailyPlansState.Controls.Add(this._bGetState);
            this._gbTimeZonesDailyPlansState.Controls.Add(this._lStateOnServer);
            this._gbTimeZonesDailyPlansState.Controls.Add(this._eStateOnServer);
            this._gbTimeZonesDailyPlansState.Controls.Add(this._lStateOnCCU);
            this._gbTimeZonesDailyPlansState.Controls.Add(this._eStateOnCCU);
            this._gbTimeZonesDailyPlansState.Controls.Add(this._tbmTimeZonesDailyPlans);
            this._gbTimeZonesDailyPlansState.Controls.Add(this._lTimeZoneDailyPlan);
            this._gbTimeZonesDailyPlansState.Location = new System.Drawing.Point(4, 4);
            this._gbTimeZonesDailyPlansState.Margin = new System.Windows.Forms.Padding(4);
            this._gbTimeZonesDailyPlansState.Name = "_gbTimeZonesDailyPlansState";
            this._gbTimeZonesDailyPlansState.Padding = new System.Windows.Forms.Padding(4);
            this._gbTimeZonesDailyPlansState.Size = new System.Drawing.Size(1042, 78);
            this._gbTimeZonesDailyPlansState.TabIndex = 4;
            this._gbTimeZonesDailyPlansState.TabStop = false;
            this._gbTimeZonesDailyPlansState.Text = "Time zones and daily plans states";
            // 
            // _bGetState
            // 
            this._bGetState.Location = new System.Drawing.Point(940, 36);
            this._bGetState.Margin = new System.Windows.Forms.Padding(4);
            this._bGetState.Name = "_bGetState";
            this._bGetState.Size = new System.Drawing.Size(94, 29);
            this._bGetState.TabIndex = 13;
            this._bGetState.Text = "Get state";
            this._bGetState.UseVisualStyleBackColor = true;
            this._bGetState.Click += new System.EventHandler(this._bGetState_Click);
            // 
            // _lStateOnServer
            // 
            this._lStateOnServer.AutoSize = true;
            this._lStateOnServer.Location = new System.Drawing.Point(680, 20);
            this._lStateOnServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lStateOnServer.Name = "_lStateOnServer";
            this._lStateOnServer.Size = new System.Drawing.Size(79, 13);
            this._lStateOnServer.TabIndex = 11;
            this._lStateOnServer.Text = "State on server";
            // 
            // _eStateOnServer
            // 
            this._eStateOnServer.Location = new System.Drawing.Point(682, 39);
            this._eStateOnServer.Margin = new System.Windows.Forms.Padding(4);
            this._eStateOnServer.Name = "_eStateOnServer";
            this._eStateOnServer.Size = new System.Drawing.Size(215, 20);
            this._eStateOnServer.TabIndex = 12;
            // 
            // _lStateOnCCU
            // 
            this._lStateOnCCU.AutoSize = true;
            this._lStateOnCCU.Location = new System.Drawing.Point(389, 20);
            this._lStateOnCCU.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lStateOnCCU.Name = "_lStateOnCCU";
            this._lStateOnCCU.Size = new System.Drawing.Size(72, 13);
            this._lStateOnCCU.TabIndex = 9;
            this._lStateOnCCU.Text = "State on CCU";
            // 
            // _eStateOnCCU
            // 
            this._eStateOnCCU.Location = new System.Drawing.Point(391, 39);
            this._eStateOnCCU.Margin = new System.Windows.Forms.Padding(4);
            this._eStateOnCCU.Name = "_eStateOnCCU";
            this._eStateOnCCU.Size = new System.Drawing.Size(215, 20);
            this._eStateOnCCU.TabIndex = 10;
            // 
            // _tbmTimeZonesDailyPlans
            // 
            this._tbmTimeZonesDailyPlans.AllowDrop = true;
            this._tbmTimeZonesDailyPlans.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmTimeZonesDailyPlans.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZonesDailyPlans.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmTimeZonesDailyPlans.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmTimeZonesDailyPlans.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZonesDailyPlans.Button.Image")));
            this._tbmTimeZonesDailyPlans.Button.Location = new System.Drawing.Point(208, 0);
            this._tbmTimeZonesDailyPlans.Button.Name = "_bMenu";
            this._tbmTimeZonesDailyPlans.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmTimeZonesDailyPlans.Button.TabIndex = 3;
            this._tbmTimeZonesDailyPlans.Button.UseVisualStyleBackColor = false;
            this._tbmTimeZonesDailyPlans.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmTimeZonesDailyPlans.ButtonDefaultBehaviour = true;
            this._tbmTimeZonesDailyPlans.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmTimeZonesDailyPlans.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZonesDailyPlans.ButtonImage")));
            // 
            // 
            // 
            this._tbmTimeZonesDailyPlans.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2});
            this._tbmTimeZonesDailyPlans.ButtonPopupMenu.Name = "";
            this._tbmTimeZonesDailyPlans.ButtonPopupMenu.Size = new System.Drawing.Size(113, 26);
            this._tbmTimeZonesDailyPlans.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmTimeZonesDailyPlans.ButtonShowImage = true;
            this._tbmTimeZonesDailyPlans.ButtonSizeHeight = 20;
            this._tbmTimeZonesDailyPlans.ButtonSizeWidth = 20;
            this._tbmTimeZonesDailyPlans.ButtonText = "";
            this._tbmTimeZonesDailyPlans.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZonesDailyPlans.HoverTime = 500;
            // 
            // 
            // 
            this._tbmTimeZonesDailyPlans.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZonesDailyPlans.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmTimeZonesDailyPlans.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmTimeZonesDailyPlans.ImageTextBox.ContextMenuStrip = this._tbmTimeZonesDailyPlans.ButtonPopupMenu;
            this._tbmTimeZonesDailyPlans.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZonesDailyPlans.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZonesDailyPlans.ImageTextBox.Image")));
            this._tbmTimeZonesDailyPlans.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmTimeZonesDailyPlans.ImageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmTimeZonesDailyPlans.ImageTextBox.Name = "_textBox";
            this._tbmTimeZonesDailyPlans.ImageTextBox.NoTextNoImage = true;
            this._tbmTimeZonesDailyPlans.ImageTextBox.ReadOnly = true;
            this._tbmTimeZonesDailyPlans.ImageTextBox.Size = new System.Drawing.Size(208, 20);
            this._tbmTimeZonesDailyPlans.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.Size = new System.Drawing.Size(206, 13);
            this._tbmTimeZonesDailyPlans.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmTimeZonesDailyPlans.ImageTextBox.UseImage = true;
            this._tbmTimeZonesDailyPlans.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmTimeZonesDailyPlans_ImageTextBox_DoubleClick);
            this._tbmTimeZonesDailyPlans.Location = new System.Drawing.Point(11, 39);
            this._tbmTimeZonesDailyPlans.Margin = new System.Windows.Forms.Padding(4);
            this._tbmTimeZonesDailyPlans.MaximumSize = new System.Drawing.Size(1500, 69);
            this._tbmTimeZonesDailyPlans.MinimumSize = new System.Drawing.Size(38, 28);
            this._tbmTimeZonesDailyPlans.Name = "_tbmTimeZonesDailyPlans";
            this._tbmTimeZonesDailyPlans.Size = new System.Drawing.Size(228, 22);
            this._tbmTimeZonesDailyPlans.TabIndex = 3;
            this._tbmTimeZonesDailyPlans.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmTimeZonesDailyPlans.TextImage")));
            this._tbmTimeZonesDailyPlans.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmTimeZonesDailyPlans_DragOver);
            this._tbmTimeZonesDailyPlans.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmTimeZonesDailyPlans_DragDrop);
            this._tbmTimeZonesDailyPlans.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmTimeZonesDailyPlans_ButtonPopupMenuItemClick);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(112, 22);
            this._tsiModify2.Text = "Modify";
            // 
            // _lTimeZoneDailyPlan
            // 
            this._lTimeZoneDailyPlan.AutoSize = true;
            this._lTimeZoneDailyPlan.Location = new System.Drawing.Point(8, 20);
            this._lTimeZoneDailyPlan.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTimeZoneDailyPlan.Name = "_lTimeZoneDailyPlan";
            this._lTimeZoneDailyPlan.Size = new System.Drawing.Size(234, 13);
            this._lTimeZoneDailyPlan.TabIndex = 2;
            this._lTimeZoneDailyPlan.Text = "Time zone (security t.z.), daily plan (security d.p.)";
            // 
            // _gbSimulationCardSwiped
            // 
            this._gbSimulationCardSwiped.Controls.Add(this._lPersonsCards);
            this._gbSimulationCardSwiped.Controls.Add(this._ePersonsCards);
            this._gbSimulationCardSwiped.Controls.Add(this._lResultDB);
            this._gbSimulationCardSwiped.Controls.Add(this._eResultCardSwipedDB);
            this._gbSimulationCardSwiped.Controls.Add(this._ePin);
            this._gbSimulationCardSwiped.Controls.Add(this._lPin);
            this._gbSimulationCardSwiped.Controls.Add(this._tbmFullCardNumber);
            this._gbSimulationCardSwiped.Controls.Add(this._lResult);
            this._gbSimulationCardSwiped.Controls.Add(this._lCardReader);
            this._gbSimulationCardSwiped.Controls.Add(this._eResultCardSwiped);
            this._gbSimulationCardSwiped.Controls.Add(this._bTest1);
            this._gbSimulationCardSwiped.Controls.Add(this._bRefresh2);
            this._gbSimulationCardSwiped.Controls.Add(this._cbCardReaderForSimulation);
            this._gbSimulationCardSwiped.Controls.Add(this._lCardNumber);
            this._gbSimulationCardSwiped.Location = new System.Drawing.Point(4, 89);
            this._gbSimulationCardSwiped.Margin = new System.Windows.Forms.Padding(4);
            this._gbSimulationCardSwiped.Name = "_gbSimulationCardSwiped";
            this._gbSimulationCardSwiped.Padding = new System.Windows.Forms.Padding(4);
            this._gbSimulationCardSwiped.Size = new System.Drawing.Size(1042, 125);
            this._gbSimulationCardSwiped.TabIndex = 3;
            this._gbSimulationCardSwiped.TabStop = false;
            this._gbSimulationCardSwiped.Text = "Simulation card swiped";
            // 
            // _lPersonsCards
            // 
            this._lPersonsCards.AutoSize = true;
            this._lPersonsCards.Location = new System.Drawing.Point(8, 70);
            this._lPersonsCards.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPersonsCards.Name = "_lPersonsCards";
            this._lPersonsCards.Size = new System.Drawing.Size(114, 13);
            this._lPersonsCards.TabIndex = 12;
            this._lPersonsCards.Text = "Added cards to person";
            // 
            // _ePersonsCards
            // 
            this._ePersonsCards.FormattingEnabled = true;
            this._ePersonsCards.Location = new System.Drawing.Point(11, 90);
            this._ePersonsCards.Margin = new System.Windows.Forms.Padding(4);
            this._ePersonsCards.Name = "_ePersonsCards";
            this._ePersonsCards.Size = new System.Drawing.Size(284, 21);
            this._ePersonsCards.TabIndex = 13;
            // 
            // _lResultDB
            // 
            this._lResultDB.AutoSize = true;
            this._lResultDB.Location = new System.Drawing.Point(708, 70);
            this._lResultDB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lResultDB.Name = "_lResultDB";
            this._lResultDB.Size = new System.Drawing.Size(55, 13);
            this._lResultDB.TabIndex = 11;
            this._lResultDB.Text = "Result DB";
            // 
            // _eResultCardSwipedDB
            // 
            this._eResultCardSwipedDB.Location = new System.Drawing.Point(709, 90);
            this._eResultCardSwipedDB.Margin = new System.Windows.Forms.Padding(4);
            this._eResultCardSwipedDB.Name = "_eResultCardSwipedDB";
            this._eResultCardSwipedDB.Size = new System.Drawing.Size(223, 20);
            this._eResultCardSwipedDB.TabIndex = 10;
            // 
            // _ePin
            // 
            this._ePin.Location = new System.Drawing.Point(309, 90);
            this._ePin.Margin = new System.Windows.Forms.Padding(4);
            this._ePin.Name = "_ePin";
            this._ePin.Size = new System.Drawing.Size(284, 20);
            this._ePin.TabIndex = 3;
            // 
            // _lPin
            // 
            this._lPin.AutoSize = true;
            this._lPin.Location = new System.Drawing.Point(309, 70);
            this._lPin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPin.Name = "_lPin";
            this._lPin.Size = new System.Drawing.Size(25, 13);
            this._lPin.TabIndex = 2;
            this._lPin.Text = "PIN";
            // 
            // _tbmFullCardNumber
            // 
            this._tbmFullCardNumber.AllowDrop = true;
            this._tbmFullCardNumber.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmFullCardNumber.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmFullCardNumber.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmFullCardNumber.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmFullCardNumber.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmFullCardNumber.Button.Image")));
            this._tbmFullCardNumber.Button.Location = new System.Drawing.Point(208, 0);
            this._tbmFullCardNumber.Button.Name = "_bMenu";
            this._tbmFullCardNumber.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmFullCardNumber.Button.TabIndex = 3;
            this._tbmFullCardNumber.Button.UseVisualStyleBackColor = false;
            this._tbmFullCardNumber.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmFullCardNumber.ButtonDefaultBehaviour = true;
            this._tbmFullCardNumber.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmFullCardNumber.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmFullCardNumber.ButtonImage")));
            // 
            // 
            // 
            this._tbmFullCardNumber.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify1});
            this._tbmFullCardNumber.ButtonPopupMenu.Name = "";
            this._tbmFullCardNumber.ButtonPopupMenu.Size = new System.Drawing.Size(113, 26);
            this._tbmFullCardNumber.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmFullCardNumber.ButtonShowImage = true;
            this._tbmFullCardNumber.ButtonSizeHeight = 20;
            this._tbmFullCardNumber.ButtonSizeWidth = 20;
            this._tbmFullCardNumber.ButtonText = "";
            this._tbmFullCardNumber.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmFullCardNumber.HoverTime = 500;
            // 
            // 
            // 
            this._tbmFullCardNumber.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmFullCardNumber.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmFullCardNumber.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmFullCardNumber.ImageTextBox.ContextMenuStrip = this._tbmFullCardNumber.ButtonPopupMenu;
            this._tbmFullCardNumber.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmFullCardNumber.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmFullCardNumber.ImageTextBox.Image")));
            this._tbmFullCardNumber.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmFullCardNumber.ImageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmFullCardNumber.ImageTextBox.Name = "_textBox";
            this._tbmFullCardNumber.ImageTextBox.NoTextNoImage = true;
            this._tbmFullCardNumber.ImageTextBox.ReadOnly = false;
            this._tbmFullCardNumber.ImageTextBox.Size = new System.Drawing.Size(208, 20);
            this._tbmFullCardNumber.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmFullCardNumber.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmFullCardNumber.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmFullCardNumber.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmFullCardNumber.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmFullCardNumber.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmFullCardNumber.ImageTextBox.TextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmFullCardNumber.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmFullCardNumber.ImageTextBox.TextBox.Size = new System.Drawing.Size(206, 13);
            this._tbmFullCardNumber.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmFullCardNumber.ImageTextBox.UseImage = true;
            this._tbmFullCardNumber.Location = new System.Drawing.Point(11, 38);
            this._tbmFullCardNumber.Margin = new System.Windows.Forms.Padding(4);
            this._tbmFullCardNumber.MaximumSize = new System.Drawing.Size(1500, 69);
            this._tbmFullCardNumber.MinimumSize = new System.Drawing.Size(38, 28);
            this._tbmFullCardNumber.Name = "_tbmFullCardNumber";
            this._tbmFullCardNumber.Size = new System.Drawing.Size(228, 22);
            this._tbmFullCardNumber.TabIndex = 1;
            this._tbmFullCardNumber.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmFullCardNumber.TextImage")));
            this._tbmFullCardNumber.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmFullCardNumber_DragOver);
            this._tbmFullCardNumber.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmFullCardNumber_DragDrop);
            this._tbmFullCardNumber.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmFullCardNumber_ButtonPopupMenuItemClick);
            // 
            // _tsiModify1
            // 
            this._tsiModify1.Name = "_tsiModify1";
            this._tsiModify1.Size = new System.Drawing.Size(112, 22);
            this._tsiModify1.Text = "Modify";
            // 
            // _lResult
            // 
            this._lResult.AutoSize = true;
            this._lResult.Location = new System.Drawing.Point(706, 20);
            this._lResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lResult.Name = "_lResult";
            this._lResult.Size = new System.Drawing.Size(37, 13);
            this._lResult.TabIndex = 7;
            this._lResult.Text = "Result";
            // 
            // _lCardReader
            // 
            this._lCardReader.AutoSize = true;
            this._lCardReader.Location = new System.Drawing.Point(305, 19);
            this._lCardReader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCardReader.Name = "_lCardReader";
            this._lCardReader.Size = new System.Drawing.Size(59, 13);
            this._lCardReader.TabIndex = 4;
            this._lCardReader.Text = "Cardreader";
            // 
            // _eResultCardSwiped
            // 
            this._eResultCardSwiped.Location = new System.Drawing.Point(709, 39);
            this._eResultCardSwiped.Margin = new System.Windows.Forms.Padding(4);
            this._eResultCardSwiped.Name = "_eResultCardSwiped";
            this._eResultCardSwiped.Size = new System.Drawing.Size(324, 20);
            this._eResultCardSwiped.TabIndex = 8;
            // 
            // _bTest1
            // 
            this._bTest1.Location = new System.Drawing.Point(940, 88);
            this._bTest1.Margin = new System.Windows.Forms.Padding(4);
            this._bTest1.Name = "_bTest1";
            this._bTest1.Size = new System.Drawing.Size(94, 29);
            this._bTest1.TabIndex = 9;
            this._bTest1.Text = "Test";
            this._bTest1.UseVisualStyleBackColor = true;
            this._bTest1.Click += new System.EventHandler(this.button2_Click);
            // 
            // _bRefresh2
            // 
            this._bRefresh2.Location = new System.Drawing.Point(605, 36);
            this._bRefresh2.Margin = new System.Windows.Forms.Padding(4);
            this._bRefresh2.Name = "_bRefresh2";
            this._bRefresh2.Size = new System.Drawing.Size(94, 29);
            this._bRefresh2.TabIndex = 6;
            this._bRefresh2.Text = "Refresh";
            this._bRefresh2.UseVisualStyleBackColor = true;
            this._bRefresh2.Click += new System.EventHandler(this.button1_Click);
            // 
            // _cbCardReaderForSimulation
            // 
            this._cbCardReaderForSimulation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCardReaderForSimulation.FormattingEnabled = true;
            this._cbCardReaderForSimulation.Location = new System.Drawing.Point(309, 39);
            this._cbCardReaderForSimulation.Margin = new System.Windows.Forms.Padding(4);
            this._cbCardReaderForSimulation.Name = "_cbCardReaderForSimulation";
            this._cbCardReaderForSimulation.Size = new System.Drawing.Size(288, 21);
            this._cbCardReaderForSimulation.TabIndex = 5;
            // 
            // _lCardNumber
            // 
            this._lCardNumber.AutoSize = true;
            this._lCardNumber.Location = new System.Drawing.Point(8, 18);
            this._lCardNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCardNumber.Name = "_lCardNumber";
            this._lCardNumber.Size = new System.Drawing.Size(67, 13);
            this._lCardNumber.TabIndex = 0;
            this._lCardNumber.Text = "Card number";
            // 
            // _lMACCCUInformation
            // 
            this._lMACCCUInformation.AutoSize = true;
            this._lMACCCUInformation.Location = new System.Drawing.Point(179, 151);
            this._lMACCCUInformation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMACCCUInformation.Name = "_lMACCCUInformation";
            this._lMACCCUInformation.Size = new System.Drawing.Size(42, 13);
            this._lMACCCUInformation.TabIndex = 3;
            this._lMACCCUInformation.Text = "no_text";
            // 
            // _lMACCCU
            // 
            this._lMACCCU.AutoSize = true;
            this._lMACCCU.Location = new System.Drawing.Point(16, 151);
            this._lMACCCU.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMACCCU.Name = "_lMACCCU";
            this._lMACCCU.Size = new System.Drawing.Size(55, 13);
            this._lMACCCU.TabIndex = 2;
            this._lMACCCU.Text = "MAC CCU";
            // 
            // _eIPAddress
            // 
            this._eIPAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eIPAddress.Location = new System.Drawing.Point(182, 124);
            this._eIPAddress.Margin = new System.Windows.Forms.Padding(4);
            this._eIPAddress.Name = "_eIPAddress";
            this._eIPAddress.Size = new System.Drawing.Size(779, 20);
            this._eIPAddress.TabIndex = 1;
            this._eIPAddress.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._eIPAddress.KeyUp += new System.Windows.Forms.KeyEventHandler(this._eIPAddress_KeyUp);
            // 
            // _lIPAddress
            // 
            this._lIPAddress.AutoSize = true;
            this._lIPAddress.Location = new System.Drawing.Point(16, 127);
            this._lIPAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lIPAddress.Name = "_lIPAddress";
            this._lIPAddress.Size = new System.Drawing.Size(58, 13);
            this._lIPAddress.TabIndex = 0;
            this._lIPAddress.Text = "IP Address";
            // 
            // _cbMainboarType
            // 
            this._cbMainboarType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbMainboarType.FormattingEnabled = true;
            this._cbMainboarType.Location = new System.Drawing.Point(181, 174);
            this._cbMainboarType.Margin = new System.Windows.Forms.Padding(4);
            this._cbMainboarType.Name = "_cbMainboarType";
            this._cbMainboarType.Size = new System.Drawing.Size(150, 21);
            this._cbMainboarType.TabIndex = 18;
            this._cbMainboarType.Visible = false;
            this._cbMainboarType.SelectedIndexChanged += new System.EventHandler(this._cbMainboarType_SelectedIndexChanged);
            // 
            // _lMainBoardTypeInformation
            // 
            this._lMainBoardTypeInformation.AutoSize = true;
            this._lMainBoardTypeInformation.Location = new System.Drawing.Point(179, 177);
            this._lMainBoardTypeInformation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMainBoardTypeInformation.Name = "_lMainBoardTypeInformation";
            this._lMainBoardTypeInformation.Size = new System.Drawing.Size(42, 13);
            this._lMainBoardTypeInformation.TabIndex = 7;
            this._lMainBoardTypeInformation.Text = "no_text";
            // 
            // _lMainBoardType
            // 
            this._lMainBoardType.AutoSize = true;
            this._lMainBoardType.Location = new System.Drawing.Point(16, 177);
            this._lMainBoardType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMainBoardType.Name = "_lMainBoardType";
            this._lMainBoardType.Size = new System.Drawing.Size(81, 13);
            this._lMainBoardType.TabIndex = 6;
            this._lMainBoardType.Text = "MainBoard type";
            // 
            // _lConfigured
            // 
            this._lConfigured.AutoSize = true;
            this._lConfigured.Location = new System.Drawing.Point(16, 100);
            this._lConfigured.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lConfigured.Name = "_lConfigured";
            this._lConfigured.Size = new System.Drawing.Size(58, 13);
            this._lConfigured.TabIndex = 4;
            this._lConfigured.Text = "Configured";
            // 
            // _eConfigured
            // 
            this._eConfigured.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eConfigured.Location = new System.Drawing.Point(182, 96);
            this._eConfigured.Margin = new System.Windows.Forms.Padding(4);
            this._eConfigured.Name = "_eConfigured";
            this._eConfigured.ReadOnly = true;
            this._eConfigured.Size = new System.Drawing.Size(779, 20);
            this._eConfigured.TabIndex = 5;
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._eIndex);
            this._panelBack.Controls.Add(this._lIndex);
            this._panelBack.Controls.Add(this._cbMainboarType);
            this._panelBack.Controls.Add(this._bApply);
            this._panelBack.Controls.Add(this._eState);
            this._panelBack.Controls.Add(this._lState);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._tcCCU);
            this._panelBack.Controls.Add(this._lMainBoardTypeInformation);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._lMainBoardType);
            this._panelBack.Controls.Add(this._eConfigured);
            this._panelBack.Controls.Add(this._lMACCCUInformation);
            this._panelBack.Controls.Add(this._lConfigured);
            this._panelBack.Controls.Add(this._eIPAddress);
            this._panelBack.Controls.Add(this._lIPAddress);
            this._panelBack.Controls.Add(this._lMACCCU);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Margin = new System.Windows.Forms.Padding(4);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(1094, 811);
            this._panelBack.TabIndex = 0;
            // 
            // _eIndex
            // 
            this._eIndex.Location = new System.Drawing.Point(182, 15);
            this._eIndex.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this._eIndex.MinimalValueLength = 3;
            this._eIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eIndex.Name = "_eIndex";
            this._eIndex.Prefix = null;
            this._eIndex.Size = new System.Drawing.Size(73, 20);
            this._eIndex.Sufix = null;
            this._eIndex.TabIndex = 21;
            this._eIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eIndex.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lIndex
            // 
            this._lIndex.AutoSize = true;
            this._lIndex.Location = new System.Drawing.Point(16, 19);
            this._lIndex.Name = "_lIndex";
            this._lIndex.Size = new System.Drawing.Size(15, 13);
            this._lIndex.TabIndex = 20;
            this._lIndex.Text = "id";
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(782, 780);
            this._bApply.Margin = new System.Windows.Forms.Padding(4);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(94, 29);
            this._bApply.TabIndex = 5;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _eState
            // 
            this._eState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eState.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eState.Location = new System.Drawing.Point(182, 69);
            this._eState.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._eState.Name = "_eState";
            this._eState.Size = new System.Drawing.Size(779, 24);
            this._eState.TabIndex = 3;
            // 
            // _lState
            // 
            this._lState.AutoSize = true;
            this._lState.Location = new System.Drawing.Point(16, 71);
            this._lState.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lState.Name = "_lState";
            this._lState.Size = new System.Drawing.Size(32, 13);
            this._lState.TabIndex = 2;
            this._lState.Text = "State";
            // 
            // _panelCmd
            // 
            this._panelCmd.Controls.Add(this._lCommandLine);
            this._panelCmd.Controls.Add(this._bRunCmd);
            this._panelCmd.Controls.Add(this._tbCommandLine);
            this._panelCmd.Location = new System.Drawing.Point(12, 4);
            this._panelCmd.Name = "_panelCmd";
            this._panelCmd.Size = new System.Drawing.Size(527, 57);
            this._panelCmd.TabIndex = 7;
            this._panelCmd.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._panelCmd_MouseDoubleClick);
            // 
            // NCASCCUEditForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1094, 811);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(858, 668);
            this.Name = "NCASCCUEditForm";
            this.Text = "NCASCCUEditForm";
            this.Load += new System.EventHandler(this.NCASCCUEditForm_Load);
            this.Shown += new System.EventHandler(this.NCASCCUEditForm_Shown);
            this.Enter += new System.EventHandler(this.NCASCCUEditForm_Enter);
            this._tcCCU.ResumeLayout(false);
            this._tpInformation.ResumeLayout(false);
            this._tpInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudOutputCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudInputCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eMaximumExpectedDCUCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgDCUs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgCardReaders)).EndInit();
            this._tpTimeSettings.ResumeLayout(false);
            this._tpTimeSettings.PerformLayout();
            this._gbTimeZone.ResumeLayout(false);
            this._gbTimeZone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eMinutes)).EndInit();
            this._gbSetTimeManually.ResumeLayout(false);
            this._gbNtpSettings.ResumeLayout(false);
            this._gbNtpSettings.PerformLayout();
            this._tpControl.ResumeLayout(false);
            this._gbLogging.ResumeLayout(false);
            this._gbLogging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudVerbosityLevel)).EndInit();
            this._gbCEUpgrade.ResumeLayout(false);
            this._gbCEUpgrade.PerformLayout();
            this._gbDeviceUpgrade.ResumeLayout(false);
            this._gbDeviceUpgrade.PerformLayout();
            this._gbConfiguration.ResumeLayout(false);
            this._gbConfiguration.PerformLayout();
            this._gbDevice.ResumeLayout(false);
            this._tpInputOutput.ResumeLayout(false);
            this._scInputsOutputs.Panel1.ResumeLayout(false);
            this._scInputsOutputs.Panel2.ResumeLayout(false);
            this._scInputsOutputs.ResumeLayout(false);
            this._gbInputsCCU.ResumeLayout(false);
            this._gbInputsCCU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvInputCCU)).EndInit();
            this._gbOutputsCCU.ResumeLayout(false);
            this._gbOutputsCCU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvOutputCCU)).EndInit();
            this._tpLevelBSI.ResumeLayout(false);
            this._gbScheme.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this._gbBsiLevelInfo.ResumeLayout(false);
            this._gbBsiLevelInfo.PerformLayout();
            this._gbBsiLevelSet.ResumeLayout(false);
            this._gbBsiLevelSet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nBreak)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nZeno)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nShortNormal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nNormalAlarm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nAlarmBreak)).EndInit();
            this._tpDoorEnvironments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvDoorEnvironments)).EndInit();
            this._tpDCUsUpgrade.ResumeLayout(false);
            this._tpDCUsUpgrade.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvDCUUpgrading)).EndInit();
            this._tpCRUpgrade.ResumeLayout(false);
            this._tpCRUpgrade.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvCRUpgrading)).EndInit();
            this._tpPortSettings.ResumeLayout(false);
            this._gbComPortSettings.ResumeLayout(false);
            this._gbComPortSettings.PerformLayout();
            this._tpIPSettings.ResumeLayout(false);
            this._tpIPSettings.PerformLayout();
            this._gbStaticIPSettings.ResumeLayout(false);
            this._gbStaticIPSettings.PerformLayout();
            this._tpAlarmSettings.ResumeLayout(false);
            this._tpAlarmSettings.PerformLayout();
            this._gbSettingsForEventlogOnCR.ResumeLayout(false);
            this._gbSettingsForEventlogOnCR.PerformLayout();
            this._gbServerWatchdog.ResumeLayout(false);
            this._tpUpsMonitor.ResumeLayout(false);
            this._gpMaintenance.ResumeLayout(false);
            this._gpMaintenance.PerformLayout();
            this._gpAlarms.ResumeLayout(false);
            this._gpValues.ResumeLayout(false);
            this._gpValues.PerformLayout();
            this._tpStatistics.ResumeLayout(false);
            this._tcStatistics.ResumeLayout(false);
            this._tpCommunicationAndMemory.ResumeLayout(false);
            this._gbCommunicationStatistic.ResumeLayout(false);
            this._tlpCOmmunicationStatistics.ResumeLayout(false);
            this._tlpCOmmunicationStatistics.PerformLayout();
            this._gbOtherStatistics.ResumeLayout(false);
            this._tlpPerformanceStatistics.ResumeLayout(false);
            this._tlpPerformanceStatistics.PerformLayout();
            this._tpThreadMap.ResumeLayout(false);
            this._tpThreadMap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvThreadMap.DataGrid)).EndInit();
            this._tpOtherStatistics.ResumeLayout(false);
            this._gbCcuStartsCount.ResumeLayout(false);
            this._gbCcuStartsCount.PerformLayout();
            this._gbCoprocessorVersion.ResumeLayout(false);
            this._gbCoprocessorVersion.PerformLayout();
            this._tpMemory.ResumeLayout(false);
            this._tpMemory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvMemory.DataGrid)).EndInit();
            this._tpProcesses.ResumeLayout(false);
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._tpTesting.ResumeLayout(false);
            this._gbSimulationAARights.ResumeLayout(false);
            this._gbSimulationAARights.PerformLayout();
            this._gbTestRoutineDCU.ResumeLayout(false);
            this._gbTestRoutineDCU.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvTestDcu)).EndInit();
            this._gbTimeZonesDailyPlansState.ResumeLayout(false);
            this._gbTimeZonesDailyPlansState.PerformLayout();
            this._gbSimulationCardSwiped.ResumeLayout(false);
            this._gbSimulationCardSwiped.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eIndex)).EndInit();
            this._panelCmd.ResumeLayout(false);
            this._panelCmd.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TabControl _tcCCU;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.TabPage _tpInformation;
        private System.Windows.Forms.Label _lMACCCU;
        private System.Windows.Forms.TextBox _eIPAddress;
        private System.Windows.Forms.Label _lIPAddress;
        private System.Windows.Forms.TabPage _tpTimeSettings;
        private System.Windows.Forms.Label _lTimeZone;
        private System.Windows.Forms.NumericUpDown _eMinutes;
        private System.Windows.Forms.NumericUpDown _eHours;
        private System.Windows.Forms.Label _lMinutes;
        private System.Windows.Forms.Label _lHours;
        private System.Windows.Forms.Label _lTimeShift;
        private System.Windows.Forms.TabPage _tpControl;
        private System.Windows.Forms.TextBox _eFirmware;
        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.Button _bHardReset;
        private System.Windows.Forms.GroupBox _gbDevice;
        private System.Windows.Forms.ComboBox _cbTimeZone;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.Button _bTest;
        private System.Windows.Forms.TabPage _tpInputOutput;
        private System.Windows.Forms.GroupBox _gbOutputsCCU;
        private System.Windows.Forms.DataGridView _dgvOutputCCU;
        private System.Windows.Forms.GroupBox _gbInputsCCU;
        private System.Windows.Forms.DataGridView _dgvInputCCU;
        private System.Windows.Forms.TabPage _tpLevelBSI;
        private System.Windows.Forms.Label _lAlarm;
        private System.Windows.Forms.Label _lNormal;
        private System.Windows.Forms.Label _lShort;
        private System.Windows.Forms.NumericUpDown _nAlarmBreak;
        private System.Windows.Forms.NumericUpDown _nNormalAlarm;
        private System.Windows.Forms.NumericUpDown _nShortNormal;
        private System.Windows.Forms.GroupBox _gbBsiLevelInfo;
        private System.Windows.Forms.GroupBox _gbBsiLevelSet;
        private System.Windows.Forms.ComboBox _cbTemplateBsiLevel;
        private System.Windows.Forms.Label _lInfoShort;
        private System.Windows.Forms.Label _lInfoBreak;
        private System.Windows.Forms.Label _lInfoAlarm;
        private System.Windows.Forms.Label _lInfoNormal;
        private System.Windows.Forms.Label _lValueBreak;
        private System.Windows.Forms.Label _lValueAlarm;
        private System.Windows.Forms.Label _lValueNormal;
        private System.Windows.Forms.Label _lValueShort;
        private System.Windows.Forms.Button _bSet;
        private System.Windows.Forms.Label _lBreak;
        private System.Windows.Forms.NumericUpDown _nZeno;
        private System.Windows.Forms.NumericUpDown _nBreak;
        private System.Windows.Forms.Label _lState;
        private System.Windows.Forms.Label _eState;
        private System.Windows.Forms.Button _bUnconfigure;
        private System.Windows.Forms.Label _lConfigured;
        private System.Windows.Forms.TextBox _eConfigured;
        private System.Windows.Forms.GroupBox _gbConfiguration;
        private System.Windows.Forms.Label _lResultOfAction;
        private System.Windows.Forms.TextBox _eResultOfAction;
        private System.Windows.Forms.Button _bConfigureForThisServer;
        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.TabPage _tpDoorEnvironments;
        private System.Windows.Forms.DataGridView _dgvDoorEnvironments;
        private System.Windows.Forms.DataGridView _dgCardReaders;
        private System.Windows.Forms.Label _lMACCCUInformation;
        private System.Windows.Forms.Label _lMainBoardTypeInformation;
        private System.Windows.Forms.Label _lMainBoardType;
        private System.Windows.Forms.TabPage _tpPortSettings;
        private System.Windows.Forms.Label _lComPortBaudRate;
        private System.Windows.Forms.ComboBox _cbPortComBaudRate;
        private System.Windows.Forms.Button _bForceReconfiguration;
        private System.Windows.Forms.Label _lDeltaValue;
        private System.Windows.Forms.Label _lDelta;
        private System.Windows.Forms.ListBox _lSNTPIPAddresses;
        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Label _lDNSHostname;
        private System.Windows.Forms.Label _lIPAddress1;
        private System.Windows.Forms.TextBox _eSNTPIpAddress;
        private System.Windows.Forms.TextBox _eNtpDnsHostName;
        private System.Windows.Forms.TabPage _tpIPSettings;
        private System.Windows.Forms.GroupBox _gbStaticIPSettings;
        private System.Windows.Forms.TextBox _eIPSettingsGateway;
        private System.Windows.Forms.Label _lGateway;
        private System.Windows.Forms.TextBox _eIPSettingsMask;
        private System.Windows.Forms.Label _lMask;
        private System.Windows.Forms.TextBox _eIPSettingsIPAddress;
        private System.Windows.Forms.Label _lIPAddress2;
        private System.Windows.Forms.RadioButton _rbStatic;
        private System.Windows.Forms.RadioButton _rbDHCP;
        private System.Windows.Forms.Button _bApply1;
        private System.Windows.Forms.Label _lResultOfApplyIPSettings;
        private System.Windows.Forms.TextBox _eResultOfApplyIPSettings;
        private System.Windows.Forms.GroupBox _gbComPortSettings;
        private System.Windows.Forms.CheckBox _chbEnabledComPort;
        private System.Windows.Forms.GroupBox _gbNtpSettings;
        private System.Windows.Forms.SplitContainer _scInputsOutputs;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh1;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.DataGridView _dgDCUs;
        private System.Windows.Forms.Button _bDcuMarkAsDead;
        private System.Windows.Forms.Button _bCrMarkAsDead;
        private System.Windows.Forms.Button _bTimeRefresh;
        private System.Windows.Forms.Label _eTime;
        private System.Windows.Forms.Label _lTime;
        private System.Windows.Forms.TabPage _tpAlarmSettings;
        private System.Windows.Forms.GroupBox _gbServerWatchdog;
        private System.Windows.Forms.Button _bPrecreateDCU;
        private System.Windows.Forms.Label _lUpgradeProgress;
        private System.Windows.Forms.Label _lTransferProgress;
        private System.Windows.Forms.Button _bStopTransfer;
        private System.Windows.Forms.GroupBox _gbDeviceUpgrade;
        private System.Windows.Forms.Label _lAvailableVersions;
        private System.Windows.Forms.NumericUpDown _eMaximumExpectedDCUCount;
        private System.Windows.Forms.Label _lMaximumExpectedDCUCount;
        private System.Windows.Forms.GroupBox _gbCEUpgrade;
        private System.Windows.Forms.Button _bStopTransferCEFile;
        private System.Windows.Forms.Label _lTransferPercents;
        private System.Windows.Forms.Label _lAvailableCEFiles;
        private System.Windows.Forms.Button _bResolve;
        private System.Windows.Forms.CheckBox _chbInheritedGeneralNtpSettings;
        private System.Windows.Forms.Label _lCEUpgradeProgress;
        private System.Windows.Forms.TextBox _eCEUpgradeProgress;
        private System.Windows.Forms.Label _lFirmwareVersion;
        private System.Windows.Forms.GroupBox _gbCommunicationStatistic;
        private System.Windows.Forms.Button _bRefreshCommunicationStatistic;
        private System.Windows.Forms.Button _bServerResetReceivedError;
        private System.Windows.Forms.Button _bServerResetDeserializeError;
        private System.Windows.Forms.Button _bServerResetReceived;
        private System.Windows.Forms.Button _bResetServerSended;
        private System.Windows.Forms.Label _lServerReceivedError;
        private System.Windows.Forms.Label _lServerDeserializeError;
        private System.Windows.Forms.Label _lServerReceived;
        private System.Windows.Forms.TextBox _eServerReceivedError;
        private System.Windows.Forms.TextBox _eServerDeserializeError;
        private System.Windows.Forms.TextBox _eServerReceived;
        private System.Windows.Forms.TextBox _eServerSended;
        private System.Windows.Forms.TextBox _eCcuSended;
        private System.Windows.Forms.Button _bCcuResetReceivedError;
        private System.Windows.Forms.Button _bCcuResetDeserializeError;
        private System.Windows.Forms.Button _bCcuResetReceived;
        private System.Windows.Forms.Button _bCcuResetSended;
        private System.Windows.Forms.TextBox _eCcuReceivedError;
        private System.Windows.Forms.TextBox _eCcuDeserializeError;
        private System.Windows.Forms.TextBox _eCcuReceived;
        private System.Windows.Forms.Label _lCcuSended;
        private System.Windows.Forms.Label _lCcuReceivedError;
        private System.Windows.Forms.Label _lCcuDeserializeError;
        private System.Windows.Forms.Label _lCcuReceived;
        private System.Windows.Forms.Label _lServerSended;
        private System.Windows.Forms.Button _bResetAll;
        private System.Windows.Forms.Label _lCcuMsgRetry;
        private System.Windows.Forms.Button _bCcuMsgRetry;
        private System.Windows.Forms.TextBox _eCcuMsgRetry;
        private System.Windows.Forms.TextBox _eServerMsgRetry;
        private System.Windows.Forms.Label _lServerMsgRetry;
        private System.Windows.Forms.Button _bResetServerMsgRetry;
        private System.Windows.Forms.GroupBox _gbCcuStartsCount;
        private System.Windows.Forms.Button _bCcuStartsCountReset;
        private System.Windows.Forms.Label _lCcuStartsCount;
        private System.Windows.Forms.TextBox _eCcuStartsCount;
        private System.Windows.Forms.Button _bCcuStartsCountRefresh;
        private System.Windows.Forms.Label _lImageVersion;
        private System.Windows.Forms.TextBox _eImageVersion;
        private System.Windows.Forms.Button _bRefresh5;
        private System.Windows.Forms.Label _lUpgradeFinalisation;
        private System.Windows.Forms.TextBox _eUpgradeFinalisation;
        private System.Windows.Forms.Button _bCancelConfigurationPassword;
        private System.Windows.Forms.Button _bChangeConfigurePassword;
        private System.Windows.Forms.TabPage _tpStatistics;
        private System.Windows.Forms.GroupBox _gbCoprocessorVersion;
        private System.Windows.Forms.Button _bRefresh9;
        private System.Windows.Forms.TextBox _eCoprocessorUpgradeResult;
        private System.Windows.Forms.TextBox _eCoprocessorActualBuildNumber;
        private System.Windows.Forms.Label _lUpgradeResult;
        private System.Windows.Forms.Label _lActualBuildNumber;
        private System.Windows.Forms.GroupBox _gbSimulationCardSwiped;
        private System.Windows.Forms.Label _lResult;
        private System.Windows.Forms.Label _lCardReader;
        private System.Windows.Forms.TextBox _eResultCardSwiped;
        private System.Windows.Forms.Button _bTest1;
        private System.Windows.Forms.Button _bRefresh2;
        private System.Windows.Forms.ComboBox _cbCardReaderForSimulation;
        private System.Windows.Forms.Label _lCardNumber;
        private Contal.IwQuick.UI.TextBoxMenu _tbmFullCardNumber;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.TextBox _ePin;
        private System.Windows.Forms.Label _lPin;
        private System.Windows.Forms.Label _lResultDB;
        private System.Windows.Forms.TextBox _eResultCardSwipedDB;
        private System.Windows.Forms.ComboBox _cbMainboarType;
        private System.Windows.Forms.TabPage _tpCRUpgrade;
        private System.Windows.Forms.CheckBox _chbCRSelectAll;
        private System.Windows.Forms.DataGridView _dgvCRUpgrading;
        private System.Windows.Forms.Label _lAvailableCRUpradeVersions;
        private System.Windows.Forms.Button _bUpgradeCRRefresh;
        private System.Windows.Forms.Button _bPrecreateCardReader;
        private System.Windows.Forms.Button _bSoftReset;
        private System.Windows.Forms.Button _bPrecreateInput;
        private System.Windows.Forms.Button _bPrecreateOutput;
        private System.Windows.Forms.NumericUpDown _nudOutputCount;
        private System.Windows.Forms.Label _lOutputCount;
        private System.Windows.Forms.NumericUpDown _nudInputCount;
        private System.Windows.Forms.Label _lInputCount;
        private System.Windows.Forms.Label _lCCUNotAcnknowledgedEvents;
        private System.Windows.Forms.TextBox _eCCUNotAcknowledgedEvents;
        private System.Windows.Forms.TextBox _eServersNotStoredEvents;
        private System.Windows.Forms.Label _lServersNotStoredEvents;
        private System.Windows.Forms.GroupBox _gbTimeZonesDailyPlansState;
        private System.Windows.Forms.Button _bGetState;
        private System.Windows.Forms.Label _lStateOnServer;
        private System.Windows.Forms.TextBox _eStateOnServer;
        private System.Windows.Forms.Label _lStateOnCCU;
        private System.Windows.Forms.TextBox _eStateOnCCU;
        private Contal.IwQuick.UI.TextBoxMenu _tbmTimeZonesDailyPlans;
        private System.Windows.Forms.Label _lTimeZoneDailyPlan;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify4;
        private System.Windows.Forms.TabPage _tpUpsMonitor;
        private System.Windows.Forms.GroupBox _gpMaintenance;
        private System.Windows.Forms.Label _lUpsOnlineState;
        private System.Windows.Forms.Label _lOnlineState;
        private System.Windows.Forms.Label m_labSuccessRate;
        private System.Windows.Forms.Label _lSuccessRate;
        private System.Windows.Forms.Label m_labResets;
        private System.Windows.Forms.Label _lResets;
        private System.Windows.Forms.Label m_labMode;
        private System.Windows.Forms.Label _lMode;
        private System.Windows.Forms.GroupBox _gpAlarms;
        private System.Windows.Forms.Label _lBatteryFuse;
        private System.Windows.Forms.Label _lTamper;
        private System.Windows.Forms.Label _lOvertemperature;
        private System.Windows.Forms.Label _lBatteryEmpty;
        private System.Windows.Forms.Label _lBatteryFault;
        private System.Windows.Forms.Label _lPrimaryPowerMissing;
        private System.Windows.Forms.Label _lOutputOutOfTollerance;
        private System.Windows.Forms.Label _lOutputFuse;
        private System.Windows.Forms.GroupBox _gpValues;
        private System.Windows.Forms.ProgressBar m_pgEstimatedBatteryCapacity;
        private System.Windows.Forms.Label m_labTemperature;
        private System.Windows.Forms.Label m_labEstimatedBatteryCapacity;
        private System.Windows.Forms.Label m_labCurrentLoad;
        private System.Windows.Forms.Label m_labCurrentBattery;
        private System.Windows.Forms.Label m_labVoltageBattery;
        private System.Windows.Forms.Label m_labVoltageOutput;
        private System.Windows.Forms.Label m_labVoltageInput;
        private System.Windows.Forms.Label _lTemperature;
        private System.Windows.Forms.Label _lEstimatedBatterycapacity;
        private System.Windows.Forms.Label _lLoadCurrent;
        private System.Windows.Forms.Label _lBatteryCurrent;
        private System.Windows.Forms.Label _lBatteryVoltage;
        private System.Windows.Forms.Label _lOutputVoltage;
        private System.Windows.Forms.Label _lInputVoltage;
        private System.Windows.Forms.TabPage _tpTesting;
        private System.Windows.Forms.GroupBox _gbOtherStatistics;
        private System.Windows.Forms.Label _lFreeMemory;
        private System.Windows.Forms.Label _lThreadCount;
        private System.Windows.Forms.Label _lFreeTotalFlashSpace;
        private System.Windows.Forms.Label _lCommandTimeoutCount;
        private System.Windows.Forms.TextBox _eFreeMemory;
        private System.Windows.Forms.TextBox _eThreads;
        private System.Windows.Forms.TextBox _eFreeTotalFlashSpace;
        private System.Windows.Forms.TextBox _eCommandTimeouts;
        private System.Windows.Forms.Button _bRefreshAll;
        private System.Windows.Forms.Label _lMemoryLoad;
        private System.Windows.Forms.Label _lTotalMemory;
        private System.Windows.Forms.TextBox _eMemoryLoad;
        private System.Windows.Forms.TextBox _eTotalMemory;
        private System.Windows.Forms.GroupBox _gbTestRoutineDCU;
        private System.Windows.Forms.Button _bTestDcu;
        private System.Windows.Forms.CheckBox _cbSelectAllToggleADC;
        private System.Windows.Forms.CheckBox _cbSelectAllToggleCard;
        private System.Windows.Forms.DataGridView _dgvTestDcu;
        private System.Windows.Forms.Button _bRefreshTestDcu;
        private System.Windows.Forms.GroupBox _gbSimulationAARights;
        private System.Windows.Forms.Label _lPerson;
        private System.Windows.Forms.Label _lAlarmArea;
        private Contal.IwQuick.UI.TextBoxMenu _tbmPerson;
        private System.Windows.Forms.RichTextBox _rtbResultsCCU;
        private Contal.IwQuick.UI.TextBoxMenu _tbmAlarmArea;
        private System.Windows.Forms.Label _lResultsCCU;
        private System.Windows.Forms.Button _bTestAlarmAreaSimulation;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify3;
        private System.Windows.Forms.RichTextBox _rtbResultsDB;
        private System.Windows.Forms.Label _lResultsAADB;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.OpenFileDialog _ofdBrowse;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpSetTimeManually;
        private System.Windows.Forms.GroupBox _gbSetTimeManually;
        private System.Windows.Forms.Button _bSet2;
        private System.Windows.Forms.Button _bDeleteEvents;
        private System.Windows.Forms.TabControl _tcStatistics;
        private System.Windows.Forms.TabPage _tpCommunicationAndMemory;
        private System.Windows.Forms.TabPage _tpOtherStatistics;
        private System.Windows.Forms.TextBox _eServersNotProcessedEvents;
        private System.Windows.Forms.Label _lServersNotProcessedEvents;
        private System.Windows.Forms.Label _lPersonsCards;
        private System.Windows.Forms.ComboBox _ePersonsCards;
        private System.Windows.Forms.CheckBox _cbSyncingTimeFromServer;
        private System.Windows.Forms.Label _lCCUUnprocessedEvents;
        private System.Windows.Forms.TextBox _eCCUUnprocessedEvents;
        private System.Windows.Forms.Label _lCCUUptime;
        private System.Windows.Forms.TextBox _eCCUUptime;
        private System.Windows.Forms.Label _lCEUptime;
        private System.Windows.Forms.TextBox _eCEUptime;
        private System.Windows.Forms.TabPage _tpThreadMap;
        private System.Windows.Forms.Button _bRefresh3;
        private Contal.Cgp.Components.CgpDataGridView _cdgvThreadMap;
        private System.Windows.Forms.GroupBox _gbLogging;
        private System.Windows.Forms.NumericUpDown _nudVerbosityLevel;
        private System.Windows.Forms.ComboBox _cbVerbosityLevel;
        private System.Windows.Forms.Label _lVerbosityLevel;
        private System.Windows.Forms.Button _bSet1;
        private System.Windows.Forms.Button _bReset1;
        private System.Windows.Forms.TableLayoutPanel _tlpCOmmunicationStatistics;
        private System.Windows.Forms.TableLayoutPanel _tlpPerformanceStatistics;
        private System.Windows.Forms.TextBox _eCCUNotAcnknowledgedAutonomousEvents;
        private System.Windows.Forms.Label _lCCUNotAcnknowledgedAutonomousEvents;
        private System.Windows.Forms.GroupBox _gbTimeZone;
        private System.Windows.Forms.Button _bTestIP;
        private System.Windows.Forms.Label _lFreeTotalSDSpace;
        private System.Windows.Forms.TextBox _eFreeTotalSDSpace;
        private System.Windows.Forms.Button _bMemoryCollect;
        private System.Windows.Forms.GroupBox _gbSettingsForEventlogOnCR;
        private System.Windows.Forms.TextBox _tbCrEvetlogLimitedSizeValue;
        private System.Windows.Forms.Label _lCrEventlogLimitedSize;
        private System.Windows.Forms.TextBox _tbLastEventTimeForMarkAlarmArea;
        private System.Windows.Forms.Label _lCRLastEventTimeForMarkAlarmArea;
        private System.Windows.Forms.TextBox _tbThreadId;
        private System.Windows.Forms.Label _lSearchByThreadId;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify24;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove24;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate24;
        private ControlModifySpecialOutput _cmsoCcuOffline;
        private System.Windows.Forms.CheckBox _cbOpenAllAlarmSettings;
        private Contal.IwQuick.PlatformPC.UI.Accordion.Accordion _accordionAlarmSettings;
        private System.Windows.Forms.Label _lAlarmTransmitter;
        private Contal.IwQuick.UI.TextBoxMenu _tbmAlarmTransmitter;
        private Contal.IwQuick.PlatformPC.UI.ProgressBarWithText _prbTransfer;
        private Contal.IwQuick.PlatformPC.UI.ProgressBarWithText _prbUnpack;
        private Contal.IwQuick.PlatformPC.UI.ProgressBarWithText _prbTransferCe;
        private System.Windows.Forms.Integration.ElementHost _ccuUpgrade;
        private Contal.Cgp.Components.WpfUpgradeFilesViewer _ccuUpgradeFiles;
        private System.Windows.Forms.Integration.ElementHost _ceUpgrade;
        private Contal.Cgp.Components.WpfUpgradeFilesViewer _ceUpgradeFiles;
        private System.Windows.Forms.Integration.ElementHost _crUpgrade;
        private Contal.Cgp.Components.WpfUpgradeFilesViewer _crUpgradeFiles;
        private System.Windows.Forms.TabPage _tpDCUsUpgrade;
        private System.Windows.Forms.Integration.ElementHost _dcuUpgrade;
        private Contal.Cgp.Components.WpfUpgradeFilesViewer _dcuUpgradeFiles;
        private System.Windows.Forms.CheckBox _chbSelectAll;
        private System.Windows.Forms.Label _lDCUsToUpgrade;
        private System.Windows.Forms.Label _lAvailableDCUUpgrades;
        private System.Windows.Forms.Button _bRefreshDCUs;
        private System.Windows.Forms.DataGridView _dgvDCUUpgrading;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label _lAlarmStateNormallyClosed;
        private System.Windows.Forms.TextBox _tbR2;
        private System.Windows.Forms.TextBox _tbR1;
        private System.Windows.Forms.Label _lk2;
        private System.Windows.Forms.Label _lR2;
        private System.Windows.Forms.Label _lk;
        private System.Windows.Forms.Label _lR1;
        private System.Windows.Forms.Label _lTamperStateNormallyClosed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _lRefU;
        private System.Windows.Forms.TextBox _tbUref;
        private System.Windows.Forms.Button _bRecalculate;
        private System.Windows.Forms.GroupBox _gbScheme;
        private Contal.IwQuick.UI.NumericUpDownWithCustomTextFormat _eIndex;
        private System.Windows.Forms.Label _lIndex;
        private System.Windows.Forms.Button _bMakeLogDump;
        private Contal.IwQuick.PlatformPC.UI.ProgressBarWithText _prbDebugFilesTransfer;
        private System.Windows.Forms.TabPage _tpMemory;
        private Contal.Cgp.Components.CgpDataGridView _cdgvMemory;
        private System.Windows.Forms.Button _bRefresh4;
        private System.Windows.Forms.TabPage _tpProcesses;
        private System.Windows.Forms.TextBox _tbCommandLine;
        private System.Windows.Forms.Button _bRunCmd;
        private System.Windows.Forms.Label _lCommandLine;
        private System.Windows.Forms.Button _bStartExplicityCKM;
        private System.Windows.Forms.Button _bStartImplicityCKM;
        private System.Windows.Forms.Button _bStopCKM;
        private System.Windows.Forms.ListBox _lbCmdResults;
        private System.Windows.Forms.Panel _panelCmd;
    }
}