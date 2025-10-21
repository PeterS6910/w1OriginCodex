namespace Contal.Cgp.NCAS.Client
{
    partial class NCASAlarmAreaEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASAlarmAreaEditForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._eShortName = new System.Windows.Forms.TextBox();
            this._lShortName = new System.Windows.Forms.Label();
            this._tcAlarmArea = new System.Windows.Forms.TabControl();
            this._tpBasicSettings = new System.Windows.Forms.TabPage();
            this._panelBasicSettings = new System.Windows.Forms.Panel();
            this._gbControl = new System.Windows.Forms.GroupBox();
            this._pbTimeBuyingMatrixStateInfo = new System.Windows.Forms.PictureBox();
            this._lResultOfAction = new System.Windows.Forms.Label();
            this._eResultOfAction = new System.Windows.Forms.TextBox();
            this._bAlarmAreaBuyTime = new System.Windows.Forms.Button();
            this._bAlarmAreaSetUnset = new System.Windows.Forms.Button();
            this._chbUnconditional = new System.Windows.Forms.CheckBox();
            this._eBuyTime = new System.Windows.Forms.DateTimePicker();
            this._cbNoPrewarning = new System.Windows.Forms.CheckBox();
            this._gbEISSettings = new System.Windows.Forms.GroupBox();
            this._chbEISInputActivationStateInverted = new System.Windows.Forms.CheckBox();
            this._lEISSetUnsetPulseLength = new System.Windows.Forms.Label();
            this._nudEISSetUnsetPulseLength = new System.Windows.Forms.NumericUpDown();
            this._lEISFilterTime = new System.Windows.Forms.Label();
            this._nudEISFilterTime = new System.Windows.Forms.NumericUpDown();
            this._lEISSetUnsetOutput = new System.Windows.Forms.Label();
            this._lEISInpurtActivationState = new System.Windows.Forms.Label();
            this._tbmEISInputActivatianState = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify11 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove11 = new System.Windows.Forms.ToolStripMenuItem();
            this._tbmEISSetUnsetOutput = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify12 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove12 = new System.Windows.Forms.ToolStripMenuItem();
            this._tblAreaInputsAndCardReaders = new System.Windows.Forms.TableLayoutPanel();
            this._gbAreaInputs = new System.Windows.Forms.GroupBox();
            this._cdgvDataInputs = new Contal.Cgp.Components.CgpDataGridView();
            this._gbCardReaders = new System.Windows.Forms.GroupBox();
            this._cdgvDataCRs = new Contal.Cgp.Components.CgpDataGridView();
            this._chbUseEIS = new System.Windows.Forms.CheckBox();
            this._tpBasicTiming = new System.Windows.Forms.TabPage();
            this._gbTimeBuying = new System.Windows.Forms.GroupBox();
            this._pbWarningActiveTimeBuying = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._cbTimeBuyingOnlyInPrewarning = new System.Windows.Forms.CheckBox();
            this._cbActivateTimeBuyingOn = new System.Windows.Forms.CheckBox();
            this._cbMaxTotalTimeBuyingOn = new System.Windows.Forms.CheckBox();
            this._lTimeBuyingOnlyInPrewarning = new System.Windows.Forms.Label();
            this._eMaxTotalTimeBuying = new System.Windows.Forms.DateTimePicker();
            this._lActivateTimeBuying = new System.Windows.Forms.Label();
            this._cbMaxTimeBuyingDurationOn = new System.Windows.Forms.CheckBox();
            this._lMaxTotalTimeBuying = new System.Windows.Forms.Label();
            this._eMaxTimeBuyingDuration = new System.Windows.Forms.DateTimePicker();
            this._lMaxTimeBuyingDuration = new System.Windows.Forms.Label();
            this._gbPrewarning = new System.Windows.Forms.GroupBox();
            this._lMmSsLegend1 = new System.Windows.Forms.Label();
            this._cbPrewarningOn = new System.Windows.Forms.CheckBox();
            this._ePrewarningDuration = new System.Windows.Forms.DateTimePicker();
            this._lPrewarningDuration = new System.Windows.Forms.Label();
            this._gbPrealarm = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._pbWarningNoExitSensor = new System.Windows.Forms.PictureBox();
            this._pbWarningNoEntrySensor = new System.Windows.Forms.PictureBox();
            this._lTemporaryUnsetExitDuration = new System.Windows.Forms.Label();
            this._cbPrealamOn = new System.Windows.Forms.CheckBox();
            this._eTemporaryUnsetDuration = new System.Windows.Forms.DateTimePicker();
            this._ePrealarmWarningDuration = new System.Windows.Forms.DateTimePicker();
            this._lTemporaryUnsetEntryDuration = new System.Windows.Forms.Label();
            this._cbProvideOnlyUnset = new System.Windows.Forms.CheckBox();
            this._gbObjAutomaticAct = new System.Windows.Forms.GroupBox();
            this._cbAutomaticActivationMode = new System.Windows.Forms.ComboBox();
            this._lICCU = new System.Windows.Forms.Label();
            this._cbOfAAInverted = new System.Windows.Forms.CheckBox();
            this._tbmObjAutomaticAct = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify3 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove3 = new System.Windows.Forms.ToolStripMenuItem();
            this._cbAutomaticDeactivate = new System.Windows.Forms.CheckBox();
            this._gbObjForcedTimeBuying = new System.Windows.Forms.GroupBox();
            this._pbWarningUnsetAction = new System.Windows.Forms.PictureBox();
            this._pbWarningTimeBuyingInfo = new System.Windows.Forms.PictureBox();
            this._chbTimeBuyingAlwaysOn = new System.Windows.Forms.CheckBox();
            this._lICCU1 = new System.Windows.Forms.Label();
            this._cbOfFTBInverted = new System.Windows.Forms.CheckBox();
            this._tbmObjForcedTimeBuying = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify16 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove16 = new System.Windows.Forms.ToolStripMenuItem();
            this._tpAlarmSettings = new System.Windows.Forms.TabPage();
            this._chbAllowAAtoCRsReporting = new System.Windows.Forms.CheckBox();
            this._accordionAlarmAreaAlarms = new Contal.IwQuick.PlatformPC.UI.Accordion.Accordion();
            this._gbCardReaderEventlogEvents = new System.Windows.Forms.GroupBox();
            this._chbEnableEventLogs = new System.Windows.Forms.CheckBox();
            this._lAlarmAreaEventSettings = new System.Windows.Forms.Label();
            this._chbPermanentlyBlockedEvent = new System.Windows.Forms.CheckBox();
            this._chbSetEvent = new System.Windows.Forms.CheckBox();
            this._chbTemporarilyBlockedEvent = new System.Windows.Forms.CheckBox();
            this._chbUnsetEvent = new System.Windows.Forms.CheckBox();
            this._chbUnblockedEvent = new System.Windows.Forms.CheckBox();
            this._chbAlarmEvent = new System.Windows.Forms.CheckBox();
            this._chbAcknowledgeEventSensor = new System.Windows.Forms.CheckBox();
            this._chbNormalEvent = new System.Windows.Forms.CheckBox();
            this._chbNormalEventSensor = new System.Windows.Forms.CheckBox();
            this._chbAcknowledgeEvent = new System.Windows.Forms.CheckBox();
            this._chbAlarmEventSensor = new System.Windows.Forms.CheckBox();
            this._chbUnconditionalSet = new System.Windows.Forms.CheckBox();
            this._lSensorEvents = new System.Windows.Forms.Label();
            this._gbABAlarmHandling = new System.Windows.Forms.GroupBox();
            this._lPercentageSensorsToAAlarmActual = new System.Windows.Forms.Label();
            this._tbPercentageSensorsToAAlarm = new System.Windows.Forms.TrackBar();
            this._lPercentageSensorsToAAlarm = new System.Windows.Forms.Label();
            this._chbABAlarmHandling = new System.Windows.Forms.CheckBox();
            this._tpSpecialOutputs = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this._gbOutputSetByObjectForAaFailed = new System.Windows.Forms.GroupBox();
            this._eOutputSetAaNotCalmOnPeriod = new System.Windows.Forms.NumericUpDown();
            this._lOutputSetAaNotCalmOnPeriod = new System.Windows.Forms.Label();
            this._lOutput11 = new System.Windows.Forms.Label();
            this._tbmOutputSetByObjectForAaFailed = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify17 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove17 = new System.Windows.Forms.ToolStripMenuItem();
            this._gbOutputMotion = new System.Windows.Forms.GroupBox();
            this._tbmOutputMotion = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify15 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove15 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput10 = new System.Windows.Forms.Label();
            this._gbOutpuSabotage = new System.Windows.Forms.GroupBox();
            this._tbmOutputSabotage = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify13 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove13 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput8 = new System.Windows.Forms.Label();
            this._gbOutputActivation = new System.Windows.Forms.GroupBox();
            this._tbmOutputActivation = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify4 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove4 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput1 = new System.Windows.Forms.Label();
            this._gbOutputNotAcknowledged = new System.Windows.Forms.GroupBox();
            this._tbmOutputNotAcknowledged = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify14 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove14 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput9 = new System.Windows.Forms.Label();
            this._gbOutputAlarmState = new System.Windows.Forms.GroupBox();
            this._tbmOutputAlarmState = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify5 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove5 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput2 = new System.Windows.Forms.Label();
            this._gbOutputPrewarning = new System.Windows.Forms.GroupBox();
            this._tbmOutputPrewarning = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify6 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove6 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput3 = new System.Windows.Forms.Label();
            this._gbOutputTmpUnsetEntry = new System.Windows.Forms.GroupBox();
            this._tbmOutputTmpUnsetEntry = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify7 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove7 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput4 = new System.Windows.Forms.Label();
            this._gbSirenOutput = new System.Windows.Forms.GroupBox();
            this._lMaximumOnPperiod = new System.Windows.Forms.Label();
            this._dtpSirenMaximumOnPeriod = new System.Windows.Forms.DateTimePicker();
            this._tbmOutputSiren = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify10 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove10 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput7 = new System.Windows.Forms.Label();
            this._gbOutputTmpUnsetExit = new System.Windows.Forms.GroupBox();
            this._tbmOutputTmpUnsetExit = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify8 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove8 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput5 = new System.Windows.Forms.Label();
            this._gbOutputAAlarm = new System.Windows.Forms.GroupBox();
            this._tbmOutputAAlarm = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify9 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove9 = new System.Windows.Forms.ToolStripMenuItem();
            this._lOutput6 = new System.Windows.Forms.Label();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate1 = new System.Windows.Forms.ToolStripMenuItem();
            this._eAlarmState = new System.Windows.Forms.TextBox();
            this._lAlarmState = new System.Windows.Forms.Label();
            this._eActivationState = new System.Windows.Forms.TextBox();
            this._lActivationState = new System.Windows.Forms.Label();
            this._panelBack = new System.Windows.Forms.Panel();
            this._cbPurpose = new System.Windows.Forms.ComboBox();
            this._lPurpose = new System.Windows.Forms.Label();
            this._eId = new Contal.IwQuick.UI.NumericUpDownWithCustomTextFormat();
            this._lId = new System.Windows.Forms.Label();
            this._eSabotage = new System.Windows.Forms.TextBox();
            this._lSabotage = new System.Windows.Forms.Label();
            this._eRequestActivationState = new System.Windows.Forms.TextBox();
            this._lRequestActivationState = new System.Windows.Forms.Label();
            this._itbImplicitManager = new Contal.IwQuick.UI.ImageTextBox();
            this._bApply = new System.Windows.Forms.Button();
            this._lImplicitManager = new System.Windows.Forms.Label();
            this._tcAlarmArea.SuspendLayout();
            this._tpBasicSettings.SuspendLayout();
            this._panelBasicSettings.SuspendLayout();
            this._gbControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbTimeBuyingMatrixStateInfo)).BeginInit();
            this._gbEISSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudEISSetUnsetPulseLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudEISFilterTime)).BeginInit();
            this._tblAreaInputsAndCardReaders.SuspendLayout();
            this._gbAreaInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvDataInputs.DataGrid)).BeginInit();
            this._gbCardReaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvDataCRs.DataGrid)).BeginInit();
            this._tpBasicTiming.SuspendLayout();
            this._gbTimeBuying.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningActiveTimeBuying)).BeginInit();
            this._gbPrewarning.SuspendLayout();
            this._gbPrealarm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningNoExitSensor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningNoEntrySensor)).BeginInit();
            this._gbObjAutomaticAct.SuspendLayout();
            this._gbObjForcedTimeBuying.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningUnsetAction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningTimeBuyingInfo)).BeginInit();
            this._tpAlarmSettings.SuspendLayout();
            this._gbCardReaderEventlogEvents.SuspendLayout();
            this._gbABAlarmHandling.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tbPercentageSensorsToAAlarm)).BeginInit();
            this._tpSpecialOutputs.SuspendLayout();
            this.panel1.SuspendLayout();
            this._gbOutputSetByObjectForAaFailed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eOutputSetAaNotCalmOnPeriod)).BeginInit();
            this._gbOutputMotion.SuspendLayout();
            this._gbOutpuSabotage.SuspendLayout();
            this._gbOutputActivation.SuspendLayout();
            this._gbOutputNotAcknowledged.SuspendLayout();
            this._gbOutputAlarmState.SuspendLayout();
            this._gbOutputPrewarning.SuspendLayout();
            this._gbOutputTmpUnsetEntry.SuspendLayout();
            this._gbSirenOutput.SuspendLayout();
            this._gbOutputTmpUnsetExit.SuspendLayout();
            this._gbOutputAAlarm.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._panelBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eId)).BeginInit();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(1046, 837);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 13;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(965, 837);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 12;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(157, 42);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(836, 20);
            this._eName.TabIndex = 1;
            this._eName.TextChanged += new System.EventHandler(this._eName_TextChanged);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(15, 45);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _eShortName
            // 
            this._eShortName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eShortName.Location = new System.Drawing.Point(157, 68);
            this._eShortName.Name = "_eShortName";
            this._eShortName.Size = new System.Drawing.Size(836, 20);
            this._eShortName.TabIndex = 3;
            this._eShortName.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lShortName
            // 
            this._lShortName.AutoSize = true;
            this._lShortName.Location = new System.Drawing.Point(15, 71);
            this._lShortName.Name = "_lShortName";
            this._lShortName.Size = new System.Drawing.Size(61, 13);
            this._lShortName.TabIndex = 2;
            this._lShortName.Text = "Short name";
            // 
            // _tcAlarmArea
            // 
            this._tcAlarmArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tcAlarmArea.Controls.Add(this._tpBasicSettings);
            this._tcAlarmArea.Controls.Add(this._tpBasicTiming);
            this._tcAlarmArea.Controls.Add(this._tpAlarmSettings);
            this._tcAlarmArea.Controls.Add(this._tpSpecialOutputs);
            this._tcAlarmArea.Controls.Add(this._tpUserFolders);
            this._tcAlarmArea.Controls.Add(this._tpReferencedBy);
            this._tcAlarmArea.Controls.Add(this._tpDescription);
            this._tcAlarmArea.Location = new System.Drawing.Point(12, 225);
            this._tcAlarmArea.Multiline = true;
            this._tcAlarmArea.Name = "_tcAlarmArea";
            this._tcAlarmArea.SelectedIndex = 0;
            this._tcAlarmArea.Size = new System.Drawing.Size(1105, 606);
            this._tcAlarmArea.TabIndex = 10;
            this._tcAlarmArea.TabStop = false;
            // 
            // _tpBasicSettings
            // 
            this._tpBasicSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpBasicSettings.Controls.Add(this._panelBasicSettings);
            this._tpBasicSettings.Location = new System.Drawing.Point(4, 22);
            this._tpBasicSettings.Name = "_tpBasicSettings";
            this._tpBasicSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpBasicSettings.Size = new System.Drawing.Size(1097, 580);
            this._tpBasicSettings.TabIndex = 2;
            this._tpBasicSettings.Text = "Basic settings";
            this._tpBasicSettings.Enter += new System.EventHandler(this._tpBaseSettings_Enter);
            // 
            // _panelBasicSettings
            // 
            this._panelBasicSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panelBasicSettings.AutoScroll = true;
            this._panelBasicSettings.AutoScrollMinSize = new System.Drawing.Size(0, 581);
            this._panelBasicSettings.BackColor = System.Drawing.SystemColors.Control;
            this._panelBasicSettings.Controls.Add(this._gbControl);
            this._panelBasicSettings.Controls.Add(this._gbEISSettings);
            this._panelBasicSettings.Controls.Add(this._tblAreaInputsAndCardReaders);
            this._panelBasicSettings.Controls.Add(this._chbUseEIS);
            this._panelBasicSettings.Location = new System.Drawing.Point(0, 0);
            this._panelBasicSettings.Name = "_panelBasicSettings";
            this._panelBasicSettings.Size = new System.Drawing.Size(1097, 581);
            this._panelBasicSettings.TabIndex = 6;
            // 
            // _gbControl
            // 
            this._gbControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbControl.BackColor = System.Drawing.SystemColors.Control;
            this._gbControl.Controls.Add(this._pbTimeBuyingMatrixStateInfo);
            this._gbControl.Controls.Add(this._lResultOfAction);
            this._gbControl.Controls.Add(this._eResultOfAction);
            this._gbControl.Controls.Add(this._bAlarmAreaBuyTime);
            this._gbControl.Controls.Add(this._bAlarmAreaSetUnset);
            this._gbControl.Controls.Add(this._chbUnconditional);
            this._gbControl.Controls.Add(this._eBuyTime);
            this._gbControl.Controls.Add(this._cbNoPrewarning);
            this._gbControl.Location = new System.Drawing.Point(2, 0);
            this._gbControl.Name = "_gbControl";
            this._gbControl.Size = new System.Drawing.Size(1072, 97);
            this._gbControl.TabIndex = 5;
            this._gbControl.TabStop = false;
            this._gbControl.Text = "Control";
            // 
            // _pbTimeBuyingMatrixStateInfo
            // 
            this._pbTimeBuyingMatrixStateInfo.Image = global::Contal.Cgp.NCAS.Client.ResourceGlobal.information;
            this._pbTimeBuyingMatrixStateInfo.Location = new System.Drawing.Point(10, 43);
            this._pbTimeBuyingMatrixStateInfo.Name = "_pbTimeBuyingMatrixStateInfo";
            this._pbTimeBuyingMatrixStateInfo.Size = new System.Drawing.Size(25, 25);
            this._pbTimeBuyingMatrixStateInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pbTimeBuyingMatrixStateInfo.TabIndex = 12;
            this._pbTimeBuyingMatrixStateInfo.TabStop = false;
            this._pbTimeBuyingMatrixStateInfo.Visible = false;
            // 
            // _lResultOfAction
            // 
            this._lResultOfAction.AutoSize = true;
            this._lResultOfAction.Location = new System.Drawing.Point(261, 16);
            this._lResultOfAction.Name = "_lResultOfAction";
            this._lResultOfAction.Size = new System.Drawing.Size(84, 13);
            this._lResultOfAction.TabIndex = 3;
            this._lResultOfAction.Text = "Result of action:";
            // 
            // _eResultOfAction
            // 
            this._eResultOfAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eResultOfAction.Location = new System.Drawing.Point(264, 32);
            this._eResultOfAction.Multiline = true;
            this._eResultOfAction.Name = "_eResultOfAction";
            this._eResultOfAction.ReadOnly = true;
            this._eResultOfAction.Size = new System.Drawing.Size(802, 56);
            this._eResultOfAction.TabIndex = 4;
            // 
            // _bAlarmAreaBuyTime
            // 
            this._bAlarmAreaBuyTime.Location = new System.Drawing.Point(133, 19);
            this._bAlarmAreaBuyTime.Name = "_bAlarmAreaBuyTime";
            this._bAlarmAreaBuyTime.Size = new System.Drawing.Size(104, 23);
            this._bAlarmAreaBuyTime.TabIndex = 1;
            this._bAlarmAreaBuyTime.Text = "BuyTime";
            this._bAlarmAreaBuyTime.UseVisualStyleBackColor = true;
            this._bAlarmAreaBuyTime.Click += new System.EventHandler(this._bAlarmAreaBuyTime_Click);
            // 
            // _bAlarmAreaSetUnset
            // 
            this._bAlarmAreaSetUnset.Location = new System.Drawing.Point(10, 19);
            this._bAlarmAreaSetUnset.Name = "_bAlarmAreaSetUnset";
            this._bAlarmAreaSetUnset.Size = new System.Drawing.Size(104, 23);
            this._bAlarmAreaSetUnset.TabIndex = 1;
            this._bAlarmAreaSetUnset.Text = "Set / Unset";
            this._bAlarmAreaSetUnset.UseVisualStyleBackColor = true;
            this._bAlarmAreaSetUnset.Click += new System.EventHandler(this._bAlarmAreaSetUnset_Click);
            // 
            // _chbUnconditional
            // 
            this._chbUnconditional.AutoSize = true;
            this._chbUnconditional.Location = new System.Drawing.Point(10, 71);
            this._chbUnconditional.Name = "_chbUnconditional";
            this._chbUnconditional.Size = new System.Drawing.Size(91, 17);
            this._chbUnconditional.TabIndex = 0;
            this._chbUnconditional.Text = "Unconditional";
            this._chbUnconditional.UseVisualStyleBackColor = true;
            // 
            // _eBuyTime
            // 
            this._eBuyTime.CustomFormat = "HH:mm";
            this._eBuyTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._eBuyTime.Location = new System.Drawing.Point(153, 48);
            this._eBuyTime.Name = "_eBuyTime";
            this._eBuyTime.ShowUpDown = true;
            this._eBuyTime.Size = new System.Drawing.Size(68, 20);
            this._eBuyTime.TabIndex = 5;
            this._eBuyTime.Value = new System.DateTime(2014, 10, 8, 0, 0, 0, 0);
            // 
            // _cbNoPrewarning
            // 
            this._cbNoPrewarning.AutoSize = true;
            this._cbNoPrewarning.Location = new System.Drawing.Point(133, 71);
            this._cbNoPrewarning.Name = "_cbNoPrewarning";
            this._cbNoPrewarning.Size = new System.Drawing.Size(95, 17);
            this._cbNoPrewarning.TabIndex = 0;
            this._cbNoPrewarning.Text = "No prewarning";
            this._cbNoPrewarning.UseVisualStyleBackColor = true;
            // 
            // _gbEISSettings
            // 
            this._gbEISSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbEISSettings.Controls.Add(this._chbEISInputActivationStateInverted);
            this._gbEISSettings.Controls.Add(this._lEISSetUnsetPulseLength);
            this._gbEISSettings.Controls.Add(this._nudEISSetUnsetPulseLength);
            this._gbEISSettings.Controls.Add(this._lEISFilterTime);
            this._gbEISSettings.Controls.Add(this._nudEISFilterTime);
            this._gbEISSettings.Controls.Add(this._lEISSetUnsetOutput);
            this._gbEISSettings.Controls.Add(this._lEISInpurtActivationState);
            this._gbEISSettings.Controls.Add(this._tbmEISInputActivatianState);
            this._gbEISSettings.Controls.Add(this._tbmEISSetUnsetOutput);
            this._gbEISSettings.Location = new System.Drawing.Point(3, 122);
            this._gbEISSettings.Name = "_gbEISSettings";
            this._gbEISSettings.Size = new System.Drawing.Size(1074, 103);
            this._gbEISSettings.TabIndex = 65;
            this._gbEISSettings.TabStop = false;
            this._gbEISSettings.Text = "Settings for externa intrusion system";
            // 
            // _chbEISInputActivationStateInverted
            // 
            this._chbEISInputActivationStateInverted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._chbEISInputActivationStateInverted.AutoSize = true;
            this._chbEISInputActivationStateInverted.Location = new System.Drawing.Point(873, 19);
            this._chbEISInputActivationStateInverted.Name = "_chbEISInputActivationStateInverted";
            this._chbEISInputActivationStateInverted.Size = new System.Drawing.Size(65, 17);
            this._chbEISInputActivationStateInverted.TabIndex = 8;
            this._chbEISInputActivationStateInverted.Text = "Inverted";
            this._chbEISInputActivationStateInverted.UseVisualStyleBackColor = true;
            this._chbEISInputActivationStateInverted.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lEISSetUnsetPulseLength
            // 
            this._lEISSetUnsetPulseLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lEISSetUnsetPulseLength.AutoSize = true;
            this._lEISSetUnsetPulseLength.Location = new System.Drawing.Point(862, 42);
            this._lEISSetUnsetPulseLength.MaximumSize = new System.Drawing.Size(150, 26);
            this._lEISSetUnsetPulseLength.MinimumSize = new System.Drawing.Size(150, 26);
            this._lEISSetUnsetPulseLength.Name = "_lEISSetUnsetPulseLength";
            this._lEISSetUnsetPulseLength.Size = new System.Drawing.Size(150, 26);
            this._lEISSetUnsetPulseLength.TabIndex = 7;
            this._lEISSetUnsetPulseLength.Text = "Pulse length for external set/unset handshaking (sec.)";
            this._lEISSetUnsetPulseLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _nudEISSetUnsetPulseLength
            // 
            this._nudEISSetUnsetPulseLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._nudEISSetUnsetPulseLength.Location = new System.Drawing.Point(1018, 45);
            this._nudEISSetUnsetPulseLength.Name = "_nudEISSetUnsetPulseLength";
            this._nudEISSetUnsetPulseLength.Size = new System.Drawing.Size(53, 20);
            this._nudEISSetUnsetPulseLength.TabIndex = 6;
            this._nudEISSetUnsetPulseLength.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lEISFilterTime
            // 
            this._lEISFilterTime.AutoSize = true;
            this._lEISFilterTime.Location = new System.Drawing.Point(6, 73);
            this._lEISFilterTime.Name = "_lEISFilterTime";
            this._lEISFilterTime.Size = new System.Drawing.Size(205, 13);
            this._lEISFilterTime.TabIndex = 5;
            this._lEISFilterTime.Text = "Filter time for EIS acknowledgement (sec.)";
            // 
            // _nudEISFilterTime
            // 
            this._nudEISFilterTime.Location = new System.Drawing.Point(250, 71);
            this._nudEISFilterTime.Name = "_nudEISFilterTime";
            this._nudEISFilterTime.Size = new System.Drawing.Size(75, 20);
            this._nudEISFilterTime.TabIndex = 4;
            this._nudEISFilterTime.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lEISSetUnsetOutput
            // 
            this._lEISSetUnsetOutput.AutoSize = true;
            this._lEISSetUnsetOutput.Location = new System.Drawing.Point(6, 48);
            this._lEISSetUnsetOutput.Name = "_lEISSetUnsetOutput";
            this._lEISSetUnsetOutput.Size = new System.Drawing.Size(122, 13);
            this._lEISSetUnsetOutput.TabIndex = 3;
            this._lEISSetUnsetOutput.Text = "Output for set/unset EIS";
            // 
            // _lEISInpurtActivationState
            // 
            this._lEISInpurtActivationState.AutoSize = true;
            this._lEISInpurtActivationState.Location = new System.Drawing.Point(6, 22);
            this._lEISInpurtActivationState.Name = "_lEISInpurtActivationState";
            this._lEISInpurtActivationState.Size = new System.Drawing.Size(162, 13);
            this._lEISInpurtActivationState.TabIndex = 2;
            this._lEISInpurtActivationState.Text = "Sensor for activation state of EIS";
            // 
            // _tbmEISInputActivatianState
            // 
            this._tbmEISInputActivatianState.AllowDrop = true;
            this._tbmEISInputActivatianState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEISInputActivatianState.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmEISInputActivatianState.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEISInputActivatianState.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmEISInputActivatianState.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmEISInputActivatianState.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEISInputActivatianState.Button.Image")));
            this._tbmEISInputActivatianState.Button.Location = new System.Drawing.Point(250, 0);
            this._tbmEISInputActivatianState.Button.Name = "_bMenu";
            this._tbmEISInputActivatianState.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmEISInputActivatianState.Button.TabIndex = 3;
            this._tbmEISInputActivatianState.Button.UseVisualStyleBackColor = false;
            this._tbmEISInputActivatianState.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmEISInputActivatianState.ButtonDefaultBehaviour = true;
            this._tbmEISInputActivatianState.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmEISInputActivatianState.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmEISInputActivatianState.ButtonImage")));
            // 
            // 
            // 
            this._tbmEISInputActivatianState.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify11,
            this._tsiRemove11});
            this._tbmEISInputActivatianState.ButtonPopupMenu.Name = "";
            this._tbmEISInputActivatianState.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmEISInputActivatianState.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmEISInputActivatianState.ButtonShowImage = true;
            this._tbmEISInputActivatianState.ButtonSizeHeight = 20;
            this._tbmEISInputActivatianState.ButtonSizeWidth = 20;
            this._tbmEISInputActivatianState.ButtonText = "";
            this._tbmEISInputActivatianState.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEISInputActivatianState.HoverTime = 500;
            // 
            // 
            // 
            this._tbmEISInputActivatianState.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEISInputActivatianState.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEISInputActivatianState.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmEISInputActivatianState.ImageTextBox.ContextMenuStrip = this._tbmEISInputActivatianState.ButtonPopupMenu;
            this._tbmEISInputActivatianState.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEISInputActivatianState.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEISInputActivatianState.ImageTextBox.Image")));
            this._tbmEISInputActivatianState.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmEISInputActivatianState.ImageTextBox.Name = "_itbTextBox";
            this._tbmEISInputActivatianState.ImageTextBox.NoTextNoImage = true;
            this._tbmEISInputActivatianState.ImageTextBox.ReadOnly = false;
            this._tbmEISInputActivatianState.ImageTextBox.Size = new System.Drawing.Size(250, 20);
            this._tbmEISInputActivatianState.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmEISInputActivatianState.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEISInputActivatianState.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEISInputActivatianState.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmEISInputActivatianState.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEISInputActivatianState.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmEISInputActivatianState.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmEISInputActivatianState.ImageTextBox.TextBox.Size = new System.Drawing.Size(248, 13);
            this._tbmEISInputActivatianState.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmEISInputActivatianState.ImageTextBox.UseImage = true;
            this._tbmEISInputActivatianState.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmEISInputActivatianState_ImageTextBox_DoubleClick);
            this._tbmEISInputActivatianState.Location = new System.Drawing.Point(250, 15);
            this._tbmEISInputActivatianState.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmEISInputActivatianState.MinimumSize = new System.Drawing.Size(30, 20);
            this._tbmEISInputActivatianState.Name = "_tbmEISInputActivatianState";
            this._tbmEISInputActivatianState.Size = new System.Drawing.Size(270, 20);
            this._tbmEISInputActivatianState.TabIndex = 0;
            this._tbmEISInputActivatianState.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmEISInputActivatianState.TextImage")));
            this._tbmEISInputActivatianState.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmEISInputActivatianState_ButtonPopupMenuItemClick);
            this._tbmEISInputActivatianState.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmEISInputActivatianState_DragDrop);
            this._tbmEISInputActivatianState.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmEISInputActivatianState_DragOver);
            // 
            // _tsiModify11
            // 
            this._tsiModify11.Name = "_tsiModify11";
            this._tsiModify11.Size = new System.Drawing.Size(117, 22);
            this._tsiModify11.Text = "Modify";
            // 
            // _tsiRemove11
            // 
            this._tsiRemove11.Name = "_tsiRemove11";
            this._tsiRemove11.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove11.Text = "Remove";
            // 
            // _tbmEISSetUnsetOutput
            // 
            this._tbmEISSetUnsetOutput.AllowDrop = true;
            this._tbmEISSetUnsetOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEISSetUnsetOutput.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmEISSetUnsetOutput.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEISSetUnsetOutput.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmEISSetUnsetOutput.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmEISSetUnsetOutput.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEISSetUnsetOutput.Button.Image")));
            this._tbmEISSetUnsetOutput.Button.Location = new System.Drawing.Point(250, 0);
            this._tbmEISSetUnsetOutput.Button.Name = "_bMenu";
            this._tbmEISSetUnsetOutput.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmEISSetUnsetOutput.Button.TabIndex = 3;
            this._tbmEISSetUnsetOutput.Button.UseVisualStyleBackColor = false;
            this._tbmEISSetUnsetOutput.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmEISSetUnsetOutput.ButtonDefaultBehaviour = true;
            this._tbmEISSetUnsetOutput.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmEISSetUnsetOutput.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmEISSetUnsetOutput.ButtonImage")));
            // 
            // 
            // 
            this._tbmEISSetUnsetOutput.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify12,
            this._tsiRemove12});
            this._tbmEISSetUnsetOutput.ButtonPopupMenu.Name = "";
            this._tbmEISSetUnsetOutput.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmEISSetUnsetOutput.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmEISSetUnsetOutput.ButtonShowImage = true;
            this._tbmEISSetUnsetOutput.ButtonSizeHeight = 20;
            this._tbmEISSetUnsetOutput.ButtonSizeWidth = 20;
            this._tbmEISSetUnsetOutput.ButtonText = "";
            this._tbmEISSetUnsetOutput.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEISSetUnsetOutput.HoverTime = 500;
            // 
            // 
            // 
            this._tbmEISSetUnsetOutput.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEISSetUnsetOutput.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEISSetUnsetOutput.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmEISSetUnsetOutput.ImageTextBox.ContextMenuStrip = this._tbmEISSetUnsetOutput.ButtonPopupMenu;
            this._tbmEISSetUnsetOutput.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEISSetUnsetOutput.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmEISSetUnsetOutput.ImageTextBox.Image")));
            this._tbmEISSetUnsetOutput.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmEISSetUnsetOutput.ImageTextBox.Name = "_itbTextBox";
            this._tbmEISSetUnsetOutput.ImageTextBox.NoTextNoImage = true;
            this._tbmEISSetUnsetOutput.ImageTextBox.ReadOnly = false;
            this._tbmEISSetUnsetOutput.ImageTextBox.Size = new System.Drawing.Size(250, 20);
            this._tbmEISSetUnsetOutput.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmEISSetUnsetOutput.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmEISSetUnsetOutput.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmEISSetUnsetOutput.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmEISSetUnsetOutput.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmEISSetUnsetOutput.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmEISSetUnsetOutput.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmEISSetUnsetOutput.ImageTextBox.TextBox.Size = new System.Drawing.Size(248, 13);
            this._tbmEISSetUnsetOutput.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmEISSetUnsetOutput.ImageTextBox.UseImage = true;
            this._tbmEISSetUnsetOutput.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmEISSetUnsetOutput_ImageTextBox_DoubleClick);
            this._tbmEISSetUnsetOutput.Location = new System.Drawing.Point(250, 45);
            this._tbmEISSetUnsetOutput.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmEISSetUnsetOutput.MinimumSize = new System.Drawing.Size(30, 20);
            this._tbmEISSetUnsetOutput.Name = "_tbmEISSetUnsetOutput";
            this._tbmEISSetUnsetOutput.Size = new System.Drawing.Size(270, 20);
            this._tbmEISSetUnsetOutput.TabIndex = 1;
            this._tbmEISSetUnsetOutput.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmEISSetUnsetOutput.TextImage")));
            this._tbmEISSetUnsetOutput.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmEISSetUnsetOutput_ButtonPopupMenuItemClick);
            this._tbmEISSetUnsetOutput.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmEISSetUnsetOutput_DragDrop);
            this._tbmEISSetUnsetOutput.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmEISSetUnsetOutput_DragOver);
            // 
            // _tsiModify12
            // 
            this._tsiModify12.Name = "_tsiModify12";
            this._tsiModify12.Size = new System.Drawing.Size(117, 22);
            this._tsiModify12.Text = "Modify";
            // 
            // _tsiRemove12
            // 
            this._tsiRemove12.Name = "_tsiRemove12";
            this._tsiRemove12.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove12.Text = "Remove";
            // 
            // _tblAreaInputsAndCardReaders
            // 
            this._tblAreaInputsAndCardReaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tblAreaInputsAndCardReaders.AutoScroll = true;
            this._tblAreaInputsAndCardReaders.BackColor = System.Drawing.SystemColors.Control;
            this._tblAreaInputsAndCardReaders.ColumnCount = 1;
            this._tblAreaInputsAndCardReaders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tblAreaInputsAndCardReaders.Controls.Add(this._gbAreaInputs, 0, 0);
            this._tblAreaInputsAndCardReaders.Controls.Add(this._gbCardReaders, 1, 0);
            this._tblAreaInputsAndCardReaders.Location = new System.Drawing.Point(3, 231);
            this._tblAreaInputsAndCardReaders.MinimumSize = new System.Drawing.Size(0, 300);
            this._tblAreaInputsAndCardReaders.Name = "_tblAreaInputsAndCardReaders";
            this._tblAreaInputsAndCardReaders.RowCount = 2;
            this._tblAreaInputsAndCardReaders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tblAreaInputsAndCardReaders.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tblAreaInputsAndCardReaders.Size = new System.Drawing.Size(1074, 343);
            this._tblAreaInputsAndCardReaders.TabIndex = 65;
            // 
            // _gbAreaInputs
            // 
            this._gbAreaInputs.Controls.Add(this._cdgvDataInputs);
            this._gbAreaInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gbAreaInputs.Location = new System.Drawing.Point(3, 3);
            this._gbAreaInputs.Name = "_gbAreaInputs";
            this._gbAreaInputs.Size = new System.Drawing.Size(1068, 165);
            this._gbAreaInputs.TabIndex = 0;
            this._gbAreaInputs.TabStop = false;
            this._gbAreaInputs.Text = "Area inputs";
            // 
            // _cdgvDataInputs
            // 
            this._cdgvDataInputs.AllwaysRefreshOrder = false;
            this._cdgvDataInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvDataInputs.CgpDataGridEvents = null;
            this._cdgvDataInputs.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvDataInputs.DataGrid.AllowDrop = true;
            this._cdgvDataInputs.DataGrid.AllowUserToAddRows = false;
            this._cdgvDataInputs.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvDataInputs.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvDataInputs.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._cdgvDataInputs.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvDataInputs.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvDataInputs.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvDataInputs.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvDataInputs.DataGrid.MultiSelect = false;
            this._cdgvDataInputs.DataGrid.Name = "_dgvData";
            this._cdgvDataInputs.DataGrid.ReadOnly = true;
            this._cdgvDataInputs.DataGrid.RowHeadersVisible = false;
            this._cdgvDataInputs.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvDataInputs.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvDataInputs.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvDataInputs.DataGrid.Size = new System.Drawing.Size(1057, 140);
            this._cdgvDataInputs.DataGrid.TabIndex = 0;
            this._cdgvDataInputs.LocalizationHelper = null;
            this._cdgvDataInputs.Location = new System.Drawing.Point(6, 19);
            this._cdgvDataInputs.Name = "_cdgvDataInputs";
            this._cdgvDataInputs.Size = new System.Drawing.Size(1057, 140);
            this._cdgvDataInputs.TabIndex = 8;
            // 
            // _gbCardReaders
            // 
            this._gbCardReaders.Controls.Add(this._cdgvDataCRs);
            this._gbCardReaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gbCardReaders.Location = new System.Drawing.Point(3, 174);
            this._gbCardReaders.Name = "_gbCardReaders";
            this._gbCardReaders.Size = new System.Drawing.Size(1068, 166);
            this._gbCardReaders.TabIndex = 0;
            this._gbCardReaders.TabStop = false;
            this._gbCardReaders.Text = "Card readers";
            // 
            // _cdgvDataCRs
            // 
            this._cdgvDataCRs.AllwaysRefreshOrder = false;
            this._cdgvDataCRs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvDataCRs.CgpDataGridEvents = null;
            this._cdgvDataCRs.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvDataCRs.DataGrid.AllowDrop = true;
            this._cdgvDataCRs.DataGrid.AllowUserToAddRows = false;
            this._cdgvDataCRs.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvDataCRs.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvDataCRs.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this._cdgvDataCRs.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvDataCRs.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvDataCRs.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvDataCRs.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvDataCRs.DataGrid.MultiSelect = false;
            this._cdgvDataCRs.DataGrid.Name = "_dgvData";
            this._cdgvDataCRs.DataGrid.ReadOnly = true;
            this._cdgvDataCRs.DataGrid.RowHeadersVisible = false;
            this._cdgvDataCRs.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this._cdgvDataCRs.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this._cdgvDataCRs.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvDataCRs.DataGrid.Size = new System.Drawing.Size(1057, 141);
            this._cdgvDataCRs.DataGrid.TabIndex = 0;
            this._cdgvDataCRs.LocalizationHelper = null;
            this._cdgvDataCRs.Location = new System.Drawing.Point(6, 19);
            this._cdgvDataCRs.Name = "_cdgvDataCRs";
            this._cdgvDataCRs.Size = new System.Drawing.Size(1057, 141);
            this._cdgvDataCRs.TabIndex = 10;
            // 
            // _chbUseEIS
            // 
            this._chbUseEIS.AutoSize = true;
            this._chbUseEIS.Location = new System.Drawing.Point(12, 99);
            this._chbUseEIS.Name = "_chbUseEIS";
            this._chbUseEIS.Size = new System.Drawing.Size(162, 17);
            this._chbUseEIS.TabIndex = 63;
            this._chbUseEIS.Text = "Use external intrusion system";
            this._chbUseEIS.UseVisualStyleBackColor = true;
            this._chbUseEIS.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tpBasicTiming
            // 
            this._tpBasicTiming.BackColor = System.Drawing.SystemColors.Control;
            this._tpBasicTiming.Controls.Add(this._gbTimeBuying);
            this._tpBasicTiming.Controls.Add(this._gbPrewarning);
            this._tpBasicTiming.Controls.Add(this._gbPrealarm);
            this._tpBasicTiming.Controls.Add(this._cbProvideOnlyUnset);
            this._tpBasicTiming.Controls.Add(this._gbObjAutomaticAct);
            this._tpBasicTiming.Controls.Add(this._gbObjForcedTimeBuying);
            this._tpBasicTiming.Location = new System.Drawing.Point(4, 22);
            this._tpBasicTiming.Name = "_tpBasicTiming";
            this._tpBasicTiming.Padding = new System.Windows.Forms.Padding(3);
            this._tpBasicTiming.Size = new System.Drawing.Size(1097, 580);
            this._tpBasicTiming.TabIndex = 3;
            this._tpBasicTiming.Text = "Basic timing";
            // 
            // _gbTimeBuying
            // 
            this._gbTimeBuying.Controls.Add(this._pbWarningActiveTimeBuying);
            this._gbTimeBuying.Controls.Add(this.label2);
            this._gbTimeBuying.Controls.Add(this.label1);
            this._gbTimeBuying.Controls.Add(this._cbTimeBuyingOnlyInPrewarning);
            this._gbTimeBuying.Controls.Add(this._cbActivateTimeBuyingOn);
            this._gbTimeBuying.Controls.Add(this._cbMaxTotalTimeBuyingOn);
            this._gbTimeBuying.Controls.Add(this._lTimeBuyingOnlyInPrewarning);
            this._gbTimeBuying.Controls.Add(this._eMaxTotalTimeBuying);
            this._gbTimeBuying.Controls.Add(this._lActivateTimeBuying);
            this._gbTimeBuying.Controls.Add(this._cbMaxTimeBuyingDurationOn);
            this._gbTimeBuying.Controls.Add(this._lMaxTotalTimeBuying);
            this._gbTimeBuying.Controls.Add(this._eMaxTimeBuyingDuration);
            this._gbTimeBuying.Controls.Add(this._lMaxTimeBuyingDuration);
            this._gbTimeBuying.Location = new System.Drawing.Point(6, 279);
            this._gbTimeBuying.Name = "_gbTimeBuying";
            this._gbTimeBuying.Size = new System.Drawing.Size(449, 114);
            this._gbTimeBuying.TabIndex = 5;
            this._gbTimeBuying.TabStop = false;
            this._gbTimeBuying.Text = "Time buying";
            // 
            // _pbWarningActiveTimeBuying
            // 
            this._pbWarningActiveTimeBuying.Image = ((System.Drawing.Image)(resources.GetObject("_pbWarningActiveTimeBuying.Image")));
            this._pbWarningActiveTimeBuying.Location = new System.Drawing.Point(49, 16);
            this._pbWarningActiveTimeBuying.Name = "_pbWarningActiveTimeBuying";
            this._pbWarningActiveTimeBuying.Size = new System.Drawing.Size(22, 22);
            this._pbWarningActiveTimeBuying.TabIndex = 9;
            this._pbWarningActiveTimeBuying.TabStop = false;
            this._pbWarningActiveTimeBuying.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(388, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "HH:MM";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(388, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "HH:MM";
            // 
            // _cbTimeBuyingOnlyInPrewarning
            // 
            this._cbTimeBuyingOnlyInPrewarning.AutoSize = true;
            this._cbTimeBuyingOnlyInPrewarning.Location = new System.Drawing.Point(6, 42);
            this._cbTimeBuyingOnlyInPrewarning.Name = "_cbTimeBuyingOnlyInPrewarning";
            this._cbTimeBuyingOnlyInPrewarning.Size = new System.Drawing.Size(40, 17);
            this._cbTimeBuyingOnlyInPrewarning.TabIndex = 0;
            this._cbTimeBuyingOnlyInPrewarning.Text = "On";
            this._cbTimeBuyingOnlyInPrewarning.UseVisualStyleBackColor = true;
            this._cbTimeBuyingOnlyInPrewarning.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _cbActivateTimeBuyingOn
            // 
            this._cbActivateTimeBuyingOn.AutoSize = true;
            this._cbActivateTimeBuyingOn.Location = new System.Drawing.Point(6, 19);
            this._cbActivateTimeBuyingOn.Name = "_cbActivateTimeBuyingOn";
            this._cbActivateTimeBuyingOn.Size = new System.Drawing.Size(40, 17);
            this._cbActivateTimeBuyingOn.TabIndex = 0;
            this._cbActivateTimeBuyingOn.Text = "On";
            this._cbActivateTimeBuyingOn.UseVisualStyleBackColor = true;
            this._cbActivateTimeBuyingOn.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _cbMaxTotalTimeBuyingOn
            // 
            this._cbMaxTotalTimeBuyingOn.AutoSize = true;
            this._cbMaxTotalTimeBuyingOn.Location = new System.Drawing.Point(6, 66);
            this._cbMaxTotalTimeBuyingOn.Name = "_cbMaxTotalTimeBuyingOn";
            this._cbMaxTotalTimeBuyingOn.Size = new System.Drawing.Size(40, 17);
            this._cbMaxTotalTimeBuyingOn.TabIndex = 0;
            this._cbMaxTotalTimeBuyingOn.Text = "On";
            this._cbMaxTotalTimeBuyingOn.UseVisualStyleBackColor = true;
            this._cbMaxTotalTimeBuyingOn.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lTimeBuyingOnlyInPrewarning
            // 
            this._lTimeBuyingOnlyInPrewarning.Location = new System.Drawing.Point(95, 44);
            this._lTimeBuyingOnlyInPrewarning.Name = "_lTimeBuyingOnlyInPrewarning";
            this._lTimeBuyingOnlyInPrewarning.Size = new System.Drawing.Size(206, 15);
            this._lTimeBuyingOnlyInPrewarning.TabIndex = 1;
            this._lTimeBuyingOnlyInPrewarning.Text = "Time buying only in prewarning";
            // 
            // _eMaxTotalTimeBuying
            // 
            this._eMaxTotalTimeBuying.CustomFormat = "HH:mm";
            this._eMaxTotalTimeBuying.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._eMaxTotalTimeBuying.Location = new System.Drawing.Point(306, 64);
            this._eMaxTotalTimeBuying.Name = "_eMaxTotalTimeBuying";
            this._eMaxTotalTimeBuying.ShowUpDown = true;
            this._eMaxTotalTimeBuying.Size = new System.Drawing.Size(76, 20);
            this._eMaxTotalTimeBuying.TabIndex = 2;
            this._eMaxTotalTimeBuying.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lActivateTimeBuying
            // 
            this._lActivateTimeBuying.Location = new System.Drawing.Point(95, 20);
            this._lActivateTimeBuying.Name = "_lActivateTimeBuying";
            this._lActivateTimeBuying.Size = new System.Drawing.Size(206, 18);
            this._lActivateTimeBuying.TabIndex = 1;
            this._lActivateTimeBuying.Text = "Activate time buying";
            // 
            // _cbMaxTimeBuyingDurationOn
            // 
            this._cbMaxTimeBuyingDurationOn.AutoSize = true;
            this._cbMaxTimeBuyingDurationOn.Location = new System.Drawing.Point(6, 90);
            this._cbMaxTimeBuyingDurationOn.Name = "_cbMaxTimeBuyingDurationOn";
            this._cbMaxTimeBuyingDurationOn.Size = new System.Drawing.Size(40, 17);
            this._cbMaxTimeBuyingDurationOn.TabIndex = 0;
            this._cbMaxTimeBuyingDurationOn.Text = "On";
            this._cbMaxTimeBuyingDurationOn.UseVisualStyleBackColor = true;
            this._cbMaxTimeBuyingDurationOn.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lMaxTotalTimeBuying
            // 
            this._lMaxTotalTimeBuying.Location = new System.Drawing.Point(95, 68);
            this._lMaxTotalTimeBuying.Name = "_lMaxTotalTimeBuying";
            this._lMaxTotalTimeBuying.Size = new System.Drawing.Size(206, 13);
            this._lMaxTotalTimeBuying.TabIndex = 1;
            this._lMaxTotalTimeBuying.Text = "Maximum total time buying";
            // 
            // _eMaxTimeBuyingDuration
            // 
            this._eMaxTimeBuyingDuration.CustomFormat = "HH:mm";
            this._eMaxTimeBuyingDuration.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._eMaxTimeBuyingDuration.Location = new System.Drawing.Point(306, 88);
            this._eMaxTimeBuyingDuration.Name = "_eMaxTimeBuyingDuration";
            this._eMaxTimeBuyingDuration.ShowUpDown = true;
            this._eMaxTimeBuyingDuration.Size = new System.Drawing.Size(76, 20);
            this._eMaxTimeBuyingDuration.TabIndex = 2;
            this._eMaxTimeBuyingDuration.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lMaxTimeBuyingDuration
            // 
            this._lMaxTimeBuyingDuration.Location = new System.Drawing.Point(95, 92);
            this._lMaxTimeBuyingDuration.Name = "_lMaxTimeBuyingDuration";
            this._lMaxTimeBuyingDuration.Size = new System.Drawing.Size(205, 13);
            this._lMaxTimeBuyingDuration.TabIndex = 1;
            this._lMaxTimeBuyingDuration.Text = "Maximum time buying duration";
            // 
            // _gbPrewarning
            // 
            this._gbPrewarning.Controls.Add(this._lMmSsLegend1);
            this._gbPrewarning.Controls.Add(this._cbPrewarningOn);
            this._gbPrewarning.Controls.Add(this._ePrewarningDuration);
            this._gbPrewarning.Controls.Add(this._lPrewarningDuration);
            this._gbPrewarning.Location = new System.Drawing.Point(6, 129);
            this._gbPrewarning.Name = "_gbPrewarning";
            this._gbPrewarning.Size = new System.Drawing.Size(449, 42);
            this._gbPrewarning.TabIndex = 1;
            this._gbPrewarning.TabStop = false;
            this._gbPrewarning.Text = "Prewarning";
            // 
            // _lMmSsLegend1
            // 
            this._lMmSsLegend1.AutoSize = true;
            this._lMmSsLegend1.Location = new System.Drawing.Point(388, 21);
            this._lMmSsLegend1.Name = "_lMmSsLegend1";
            this._lMmSsLegend1.Size = new System.Drawing.Size(42, 13);
            this._lMmSsLegend1.TabIndex = 10;
            this._lMmSsLegend1.Text = "MM:SS";
            // 
            // _cbPrewarningOn
            // 
            this._cbPrewarningOn.AutoSize = true;
            this._cbPrewarningOn.Location = new System.Drawing.Point(6, 19);
            this._cbPrewarningOn.Name = "_cbPrewarningOn";
            this._cbPrewarningOn.Size = new System.Drawing.Size(40, 17);
            this._cbPrewarningOn.TabIndex = 0;
            this._cbPrewarningOn.Text = "On";
            this._cbPrewarningOn.UseVisualStyleBackColor = true;
            this._cbPrewarningOn.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _ePrewarningDuration
            // 
            this._ePrewarningDuration.CustomFormat = "mm:ss";
            this._ePrewarningDuration.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._ePrewarningDuration.Location = new System.Drawing.Point(306, 17);
            this._ePrewarningDuration.Name = "_ePrewarningDuration";
            this._ePrewarningDuration.ShowUpDown = true;
            this._ePrewarningDuration.Size = new System.Drawing.Size(76, 20);
            this._ePrewarningDuration.TabIndex = 2;
            this._ePrewarningDuration.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lPrewarningDuration
            // 
            this._lPrewarningDuration.Location = new System.Drawing.Point(95, 21);
            this._lPrewarningDuration.Name = "_lPrewarningDuration";
            this._lPrewarningDuration.Size = new System.Drawing.Size(205, 13);
            this._lPrewarningDuration.TabIndex = 1;
            this._lPrewarningDuration.Text = "Prewarning duration";
            // 
            // _gbPrealarm
            // 
            this._gbPrealarm.Controls.Add(this.label4);
            this._gbPrealarm.Controls.Add(this.label3);
            this._gbPrealarm.Controls.Add(this._pbWarningNoExitSensor);
            this._gbPrealarm.Controls.Add(this._pbWarningNoEntrySensor);
            this._gbPrealarm.Controls.Add(this._lTemporaryUnsetExitDuration);
            this._gbPrealarm.Controls.Add(this._cbPrealamOn);
            this._gbPrealarm.Controls.Add(this._eTemporaryUnsetDuration);
            this._gbPrealarm.Controls.Add(this._ePrealarmWarningDuration);
            this._gbPrealarm.Controls.Add(this._lTemporaryUnsetEntryDuration);
            this._gbPrealarm.Location = new System.Drawing.Point(6, 179);
            this._gbPrealarm.Name = "_gbPrealarm";
            this._gbPrealarm.Size = new System.Drawing.Size(449, 94);
            this._gbPrealarm.TabIndex = 4;
            this._gbPrealarm.TabStop = false;
            this._gbPrealarm.Text = "Prealarm";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(388, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "MM:SS";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(388, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "MM:SS";
            // 
            // _pbWarningNoExitSensor
            // 
            this._pbWarningNoExitSensor.Image = ((System.Drawing.Image)(resources.GetObject("_pbWarningNoExitSensor.Image")));
            this._pbWarningNoExitSensor.Location = new System.Drawing.Point(49, 16);
            this._pbWarningNoExitSensor.Name = "_pbWarningNoExitSensor";
            this._pbWarningNoExitSensor.Size = new System.Drawing.Size(22, 22);
            this._pbWarningNoExitSensor.TabIndex = 7;
            this._pbWarningNoExitSensor.TabStop = false;
            this._pbWarningNoExitSensor.Visible = false;
            // 
            // _pbWarningNoEntrySensor
            // 
            this._pbWarningNoEntrySensor.Image = ((System.Drawing.Image)(resources.GetObject("_pbWarningNoEntrySensor.Image")));
            this._pbWarningNoEntrySensor.Location = new System.Drawing.Point(49, 57);
            this._pbWarningNoEntrySensor.Name = "_pbWarningNoEntrySensor";
            this._pbWarningNoEntrySensor.Size = new System.Drawing.Size(22, 22);
            this._pbWarningNoEntrySensor.TabIndex = 8;
            this._pbWarningNoEntrySensor.TabStop = false;
            this._pbWarningNoEntrySensor.Visible = false;
            // 
            // _lTemporaryUnsetExitDuration
            // 
            this._lTemporaryUnsetExitDuration.Location = new System.Drawing.Point(95, 21);
            this._lTemporaryUnsetExitDuration.Name = "_lTemporaryUnsetExitDuration";
            this._lTemporaryUnsetExitDuration.Size = new System.Drawing.Size(206, 16);
            this._lTemporaryUnsetExitDuration.TabIndex = 2;
            this._lTemporaryUnsetExitDuration.Text = "Temporary unset exit duration";
            // 
            // _cbPrealamOn
            // 
            this._cbPrealamOn.AutoSize = true;
            this._cbPrealamOn.Location = new System.Drawing.Point(6, 60);
            this._cbPrealamOn.Name = "_cbPrealamOn";
            this._cbPrealamOn.Size = new System.Drawing.Size(40, 17);
            this._cbPrealamOn.TabIndex = 0;
            this._cbPrealamOn.Text = "On";
            this._cbPrealamOn.UseVisualStyleBackColor = true;
            this._cbPrealamOn.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _eTemporaryUnsetDuration
            // 
            this._eTemporaryUnsetDuration.CustomFormat = "mm:ss";
            this._eTemporaryUnsetDuration.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._eTemporaryUnsetDuration.Location = new System.Drawing.Point(306, 17);
            this._eTemporaryUnsetDuration.Name = "_eTemporaryUnsetDuration";
            this._eTemporaryUnsetDuration.ShowUpDown = true;
            this._eTemporaryUnsetDuration.Size = new System.Drawing.Size(76, 20);
            this._eTemporaryUnsetDuration.TabIndex = 3;
            this._eTemporaryUnsetDuration.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _ePrealarmWarningDuration
            // 
            this._ePrealarmWarningDuration.CustomFormat = "mm:ss";
            this._ePrealarmWarningDuration.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._ePrealarmWarningDuration.Location = new System.Drawing.Point(306, 58);
            this._ePrealarmWarningDuration.Name = "_ePrealarmWarningDuration";
            this._ePrealarmWarningDuration.ShowUpDown = true;
            this._ePrealarmWarningDuration.Size = new System.Drawing.Size(76, 20);
            this._ePrealarmWarningDuration.TabIndex = 2;
            this._ePrealarmWarningDuration.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lTemporaryUnsetEntryDuration
            // 
            this._lTemporaryUnsetEntryDuration.Location = new System.Drawing.Point(95, 59);
            this._lTemporaryUnsetEntryDuration.Name = "_lTemporaryUnsetEntryDuration";
            this._lTemporaryUnsetEntryDuration.Size = new System.Drawing.Size(205, 18);
            this._lTemporaryUnsetEntryDuration.TabIndex = 1;
            this._lTemporaryUnsetEntryDuration.Text = "Temporary unset entry duration";
            // 
            // _cbProvideOnlyUnset
            // 
            this._cbProvideOnlyUnset.AutoSize = true;
            this._cbProvideOnlyUnset.Location = new System.Drawing.Point(11, 557);
            this._cbProvideOnlyUnset.Name = "_cbProvideOnlyUnset";
            this._cbProvideOnlyUnset.Size = new System.Drawing.Size(268, 17);
            this._cbProvideOnlyUnset.TabIndex = 1;
            this._cbProvideOnlyUnset.Text = "Provide only unset action if time buying not required";
            this._cbProvideOnlyUnset.UseVisualStyleBackColor = true;
            this._cbProvideOnlyUnset.Visible = false;
            this._cbProvideOnlyUnset.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _gbObjAutomaticAct
            // 
            this._gbObjAutomaticAct.Controls.Add(this._cbAutomaticActivationMode);
            this._gbObjAutomaticAct.Controls.Add(this._lICCU);
            this._gbObjAutomaticAct.Controls.Add(this._cbOfAAInverted);
            this._gbObjAutomaticAct.Controls.Add(this._tbmObjAutomaticAct);
            this._gbObjAutomaticAct.Controls.Add(this._cbAutomaticDeactivate);
            this._gbObjAutomaticAct.Location = new System.Drawing.Point(6, 6);
            this._gbObjAutomaticAct.Name = "_gbObjAutomaticAct";
            this._gbObjAutomaticAct.Size = new System.Drawing.Size(449, 117);
            this._gbObjAutomaticAct.TabIndex = 0;
            this._gbObjAutomaticAct.TabStop = false;
            this._gbObjAutomaticAct.Text = "Object for automatic activation";
            // 
            // _cbAutomaticActivationMode
            // 
            this._cbAutomaticActivationMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbAutomaticActivationMode.FormattingEnabled = true;
            this._cbAutomaticActivationMode.Location = new System.Drawing.Point(5, 91);
            this._cbAutomaticActivationMode.Name = "_cbAutomaticActivationMode";
            this._cbAutomaticActivationMode.Size = new System.Drawing.Size(295, 21);
            this._cbAutomaticActivationMode.TabIndex = 5;
            this._cbAutomaticActivationMode.SelectedIndexChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lICCU
            // 
            this._lICCU.AutoSize = true;
            this._lICCU.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lICCU.Location = new System.Drawing.Point(307, 24);
            this._lICCU.Name = "_lICCU";
            this._lICCU.Size = new System.Drawing.Size(35, 13);
            this._lICCU.TabIndex = 3;
            this._lICCU.Text = "iCCU";
            this._lICCU.Visible = false;
            // 
            // _cbOfAAInverted
            // 
            this._cbOfAAInverted.AutoSize = true;
            this._cbOfAAInverted.Location = new System.Drawing.Point(6, 70);
            this._cbOfAAInverted.Name = "_cbOfAAInverted";
            this._cbOfAAInverted.Size = new System.Drawing.Size(65, 17);
            this._cbOfAAInverted.TabIndex = 2;
            this._cbOfAAInverted.Text = "Inverted";
            this._cbOfAAInverted.UseVisualStyleBackColor = true;
            this._cbOfAAInverted.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tbmObjAutomaticAct
            // 
            this._tbmObjAutomaticAct.AllowDrop = true;
            this._tbmObjAutomaticAct.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmObjAutomaticAct.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjAutomaticAct.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmObjAutomaticAct.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmObjAutomaticAct.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmObjAutomaticAct.Button.Image")));
            this._tbmObjAutomaticAct.Button.Location = new System.Drawing.Point(253, 0);
            this._tbmObjAutomaticAct.Button.Name = "_bMenu";
            this._tbmObjAutomaticAct.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmObjAutomaticAct.Button.TabIndex = 3;
            this._tbmObjAutomaticAct.Button.UseVisualStyleBackColor = false;
            this._tbmObjAutomaticAct.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmObjAutomaticAct.ButtonDefaultBehaviour = true;
            this._tbmObjAutomaticAct.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmObjAutomaticAct.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmObjAutomaticAct.ButtonImage")));
            // 
            // 
            // 
            this._tbmObjAutomaticAct.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify3,
            this._tsiRemove3});
            this._tbmObjAutomaticAct.ButtonPopupMenu.Name = "";
            this._tbmObjAutomaticAct.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmObjAutomaticAct.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmObjAutomaticAct.ButtonShowImage = true;
            this._tbmObjAutomaticAct.ButtonSizeHeight = 20;
            this._tbmObjAutomaticAct.ButtonSizeWidth = 20;
            this._tbmObjAutomaticAct.ButtonText = "";
            this._tbmObjAutomaticAct.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjAutomaticAct.HoverTime = 500;
            // 
            // 
            // 
            this._tbmObjAutomaticAct.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjAutomaticAct.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmObjAutomaticAct.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmObjAutomaticAct.ImageTextBox.ContextMenuStrip = this._tbmObjAutomaticAct.ButtonPopupMenu;
            this._tbmObjAutomaticAct.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjAutomaticAct.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmObjAutomaticAct.ImageTextBox.Image")));
            this._tbmObjAutomaticAct.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmObjAutomaticAct.ImageTextBox.Name = "_textBox";
            this._tbmObjAutomaticAct.ImageTextBox.NoTextNoImage = true;
            this._tbmObjAutomaticAct.ImageTextBox.ReadOnly = true;
            this._tbmObjAutomaticAct.ImageTextBox.Size = new System.Drawing.Size(253, 20);
            this._tbmObjAutomaticAct.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.Size = new System.Drawing.Size(251, 13);
            this._tbmObjAutomaticAct.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmObjAutomaticAct.ImageTextBox.UseImage = true;
            this._tbmObjAutomaticAct.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmObjAutomaticAct.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmObjAutomaticAct_DoubleClick);
            this._tbmObjAutomaticAct.Location = new System.Drawing.Point(6, 19);
            this._tbmObjAutomaticAct.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmObjAutomaticAct.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmObjAutomaticAct.Name = "_tbmObjAutomaticAct";
            this._tbmObjAutomaticAct.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._tbmObjAutomaticAct.Size = new System.Drawing.Size(273, 22);
            this._tbmObjAutomaticAct.TabIndex = 0;
            this._tbmObjAutomaticAct.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmObjAutomaticAct.TextImage")));
            this._tbmObjAutomaticAct.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmObjAutomaticAct_ButtonPopupMenuItemClick);
            this._tbmObjAutomaticAct.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmObjAutomaticAct_DragDrop);
            this._tbmObjAutomaticAct.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmOnOffObject_DragOver);
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
            // _cbAutomaticDeactivate
            // 
            this._cbAutomaticDeactivate.AutoSize = true;
            this._cbAutomaticDeactivate.Location = new System.Drawing.Point(6, 47);
            this._cbAutomaticDeactivate.Name = "_cbAutomaticDeactivate";
            this._cbAutomaticDeactivate.Size = new System.Drawing.Size(273, 17);
            this._cbAutomaticDeactivate.TabIndex = 1;
            this._cbAutomaticDeactivate.Text = "Automatically deactivate when object turns ON state";
            this._cbAutomaticDeactivate.UseVisualStyleBackColor = true;
            this._cbAutomaticDeactivate.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _gbObjForcedTimeBuying
            // 
            this._gbObjForcedTimeBuying.Controls.Add(this._pbWarningUnsetAction);
            this._gbObjForcedTimeBuying.Controls.Add(this._pbWarningTimeBuyingInfo);
            this._gbObjForcedTimeBuying.Controls.Add(this._chbTimeBuyingAlwaysOn);
            this._gbObjForcedTimeBuying.Controls.Add(this._lICCU1);
            this._gbObjForcedTimeBuying.Controls.Add(this._cbOfFTBInverted);
            this._gbObjForcedTimeBuying.Controls.Add(this._tbmObjForcedTimeBuying);
            this._gbObjForcedTimeBuying.Location = new System.Drawing.Point(6, 398);
            this._gbObjForcedTimeBuying.Name = "_gbObjForcedTimeBuying";
            this._gbObjForcedTimeBuying.Size = new System.Drawing.Size(449, 121);
            this._gbObjForcedTimeBuying.TabIndex = 6;
            this._gbObjForcedTimeBuying.TabStop = false;
            this._gbObjForcedTimeBuying.Text = "Object for forced time buying";
            // 
            // _pbWarningUnsetAction
            // 
            this._pbWarningUnsetAction.Image = global::Contal.Cgp.NCAS.Client.ResourceGlobal.information;
            this._pbWarningUnsetAction.Location = new System.Drawing.Point(2, 91);
            this._pbWarningUnsetAction.Name = "_pbWarningUnsetAction";
            this._pbWarningUnsetAction.Size = new System.Drawing.Size(25, 25);
            this._pbWarningUnsetAction.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pbWarningUnsetAction.TabIndex = 11;
            this._pbWarningUnsetAction.TabStop = false;
            // 
            // _pbWarningTimeBuyingInfo
            // 
            this._pbWarningTimeBuyingInfo.Image = global::Contal.Cgp.NCAS.Client.ResourceGlobal.information;
            this._pbWarningTimeBuyingInfo.Location = new System.Drawing.Point(391, 43);
            this._pbWarningTimeBuyingInfo.Name = "_pbWarningTimeBuyingInfo";
            this._pbWarningTimeBuyingInfo.Size = new System.Drawing.Size(25, 25);
            this._pbWarningTimeBuyingInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pbWarningTimeBuyingInfo.TabIndex = 10;
            this._pbWarningTimeBuyingInfo.TabStop = false;
            this._pbWarningTimeBuyingInfo.Visible = false;
            // 
            // _chbTimeBuyingAlwaysOn
            // 
            this._chbTimeBuyingAlwaysOn.AutoSize = true;
            this._chbTimeBuyingAlwaysOn.Location = new System.Drawing.Point(5, 19);
            this._chbTimeBuyingAlwaysOn.Name = "_chbTimeBuyingAlwaysOn";
            this._chbTimeBuyingAlwaysOn.Size = new System.Drawing.Size(73, 17);
            this._chbTimeBuyingAlwaysOn.TabIndex = 7;
            this._chbTimeBuyingAlwaysOn.Text = "AlwaysOn";
            this._chbTimeBuyingAlwaysOn.UseVisualStyleBackColor = true;
            this._chbTimeBuyingAlwaysOn.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lICCU1
            // 
            this._lICCU1.AutoSize = true;
            this._lICCU1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lICCU1.Location = new System.Drawing.Point(307, 49);
            this._lICCU1.Name = "_lICCU1";
            this._lICCU1.Size = new System.Drawing.Size(35, 13);
            this._lICCU1.TabIndex = 3;
            this._lICCU1.Text = "iCCU";
            this._lICCU1.Visible = false;
            // 
            // _cbOfFTBInverted
            // 
            this._cbOfFTBInverted.AutoSize = true;
            this._cbOfFTBInverted.Location = new System.Drawing.Point(6, 72);
            this._cbOfFTBInverted.Name = "_cbOfFTBInverted";
            this._cbOfFTBInverted.Size = new System.Drawing.Size(65, 17);
            this._cbOfFTBInverted.TabIndex = 2;
            this._cbOfFTBInverted.Text = "Inverted";
            this._cbOfFTBInverted.UseVisualStyleBackColor = true;
            this._cbOfFTBInverted.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tbmObjForcedTimeBuying
            // 
            this._tbmObjForcedTimeBuying.AllowDrop = true;
            this._tbmObjForcedTimeBuying.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmObjForcedTimeBuying.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjForcedTimeBuying.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmObjForcedTimeBuying.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmObjForcedTimeBuying.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmObjForcedTimeBuying.Button.Image")));
            this._tbmObjForcedTimeBuying.Button.Location = new System.Drawing.Point(253, 0);
            this._tbmObjForcedTimeBuying.Button.Name = "_bMenu";
            this._tbmObjForcedTimeBuying.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmObjForcedTimeBuying.Button.TabIndex = 3;
            this._tbmObjForcedTimeBuying.Button.UseVisualStyleBackColor = false;
            this._tbmObjForcedTimeBuying.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmObjForcedTimeBuying.ButtonDefaultBehaviour = true;
            this._tbmObjForcedTimeBuying.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmObjForcedTimeBuying.ButtonImage = null;
            // 
            // 
            // 
            this._tbmObjForcedTimeBuying.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify16,
            this._tsiRemove16});
            this._tbmObjForcedTimeBuying.ButtonPopupMenu.Name = "";
            this._tbmObjForcedTimeBuying.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmObjForcedTimeBuying.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmObjForcedTimeBuying.ButtonShowImage = true;
            this._tbmObjForcedTimeBuying.ButtonSizeHeight = 20;
            this._tbmObjForcedTimeBuying.ButtonSizeWidth = 20;
            this._tbmObjForcedTimeBuying.ButtonText = "";
            this._tbmObjForcedTimeBuying.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjForcedTimeBuying.HoverTime = 500;
            // 
            // 
            // 
            this._tbmObjForcedTimeBuying.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjForcedTimeBuying.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmObjForcedTimeBuying.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmObjForcedTimeBuying.ImageTextBox.ContextMenuStrip = this._tbmObjForcedTimeBuying.ButtonPopupMenu;
            this._tbmObjForcedTimeBuying.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjForcedTimeBuying.ImageTextBox.Image = null;
            this._tbmObjForcedTimeBuying.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmObjForcedTimeBuying.ImageTextBox.Name = "_textBox";
            this._tbmObjForcedTimeBuying.ImageTextBox.NoTextNoImage = true;
            this._tbmObjForcedTimeBuying.ImageTextBox.ReadOnly = true;
            this._tbmObjForcedTimeBuying.ImageTextBox.Size = new System.Drawing.Size(253, 20);
            this._tbmObjForcedTimeBuying.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.Size = new System.Drawing.Size(251, 13);
            this._tbmObjForcedTimeBuying.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmObjForcedTimeBuying.ImageTextBox.UseImage = true;
            this._tbmObjForcedTimeBuying.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmObjForcedTimeBuying.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmObjForcedTimeBuying_DoubleClick);
            this._tbmObjForcedTimeBuying.Location = new System.Drawing.Point(6, 44);
            this._tbmObjForcedTimeBuying.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmObjForcedTimeBuying.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmObjForcedTimeBuying.Name = "_tbmObjForcedTimeBuying";
            this._tbmObjForcedTimeBuying.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._tbmObjForcedTimeBuying.Size = new System.Drawing.Size(273, 22);
            this._tbmObjForcedTimeBuying.TabIndex = 0;
            this._tbmObjForcedTimeBuying.TextImage = null;
            this._tbmObjForcedTimeBuying.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmObjForcedTimeBuying_ButtonPopupMenuItemClick);
            this._tbmObjForcedTimeBuying.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmObjForcedTimeBuying_DragDrop);
            this._tbmObjForcedTimeBuying.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmOnOffObject_DragOver);
            // 
            // _tsiModify16
            // 
            this._tsiModify16.Name = "_tsiModify16";
            this._tsiModify16.Size = new System.Drawing.Size(117, 22);
            this._tsiModify16.Text = "Modify";
            // 
            // _tsiRemove16
            // 
            this._tsiRemove16.Name = "_tsiRemove16";
            this._tsiRemove16.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove16.Text = "Remove";
            // 
            // _tpAlarmSettings
            // 
            this._tpAlarmSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpAlarmSettings.Controls.Add(this._chbAllowAAtoCRsReporting);
            this._tpAlarmSettings.Controls.Add(this._accordionAlarmAreaAlarms);
            this._tpAlarmSettings.Controls.Add(this._gbCardReaderEventlogEvents);
            this._tpAlarmSettings.Controls.Add(this._gbABAlarmHandling);
            this._tpAlarmSettings.Location = new System.Drawing.Point(4, 22);
            this._tpAlarmSettings.Name = "_tpAlarmSettings";
            this._tpAlarmSettings.Size = new System.Drawing.Size(1097, 580);
            this._tpAlarmSettings.TabIndex = 4;
            this._tpAlarmSettings.Text = "Alarm settings";
            // 
            // _chbAllowAAtoCRsReporting
            // 
            this._chbAllowAAtoCRsReporting.AutoSize = true;
            this._chbAllowAAtoCRsReporting.Checked = true;
            this._chbAllowAAtoCRsReporting.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this._chbAllowAAtoCRsReporting.Location = new System.Drawing.Point(6, 3);
            this._chbAllowAAtoCRsReporting.Name = "_chbAllowAAtoCRsReporting";
            this._chbAllowAAtoCRsReporting.Size = new System.Drawing.Size(265, 17);
            this._chbAllowAAtoCRsReporting.TabIndex = 4;
            this._chbAllowAAtoCRsReporting.Text = "Allow alarm area state reporting on it\'s card readers";
            this._chbAllowAAtoCRsReporting.ThreeState = true;
            this._chbAllowAAtoCRsReporting.UseVisualStyleBackColor = true;
            this._chbAllowAAtoCRsReporting.CheckStateChanged += new System.EventHandler(this._chbInheritGeneralAlarmSetting_CheckedChanged);
            // 
            // _accordionAlarmAreaAlarms
            // 
            this._accordionAlarmAreaAlarms.AddResizeBars = true;
            this._accordionAlarmAreaAlarms.AllowMouseResize = true;
            this._accordionAlarmAreaAlarms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._accordionAlarmAreaAlarms.AnimateCloseEffect = Contal.IwQuick.PlatformPC.UI.Accordion.AnimateWindowFlags.Hide;
            this._accordionAlarmAreaAlarms.AnimateCloseMillis = 300;
            this._accordionAlarmAreaAlarms.AnimateOpenEffect = Contal.IwQuick.PlatformPC.UI.Accordion.AnimateWindowFlags.Show;
            this._accordionAlarmAreaAlarms.AnimateOpenMillis = 300;
            this._accordionAlarmAreaAlarms.AutoFixDockStyle = true;
            this._accordionAlarmAreaAlarms.AutoScroll = true;
            this._accordionAlarmAreaAlarms.CheckBoxFactory = null;
            this._accordionAlarmAreaAlarms.CheckBoxMargin = new System.Windows.Forms.Padding(0);
            this._accordionAlarmAreaAlarms.ContentBackColor = null;
            this._accordionAlarmAreaAlarms.ContentMargin = null;
            this._accordionAlarmAreaAlarms.ContentPadding = new System.Windows.Forms.Padding(5);
            this._accordionAlarmAreaAlarms.ControlBackColor = null;
            this._accordionAlarmAreaAlarms.ControlMinimumHeightIsItsPreferredHeight = true;
            this._accordionAlarmAreaAlarms.ControlMinimumWidthIsItsPreferredWidth = true;
            this._accordionAlarmAreaAlarms.DownArrow = null;
            this._accordionAlarmAreaAlarms.FillHeight = true;
            this._accordionAlarmAreaAlarms.FillLastOpened = false;
            this._accordionAlarmAreaAlarms.FillModeGrowOnly = false;
            this._accordionAlarmAreaAlarms.FillResetOnCollapse = false;
            this._accordionAlarmAreaAlarms.FillWidth = true;
            this._accordionAlarmAreaAlarms.GrabCursor = System.Windows.Forms.Cursors.SizeNS;
            this._accordionAlarmAreaAlarms.GrabRequiresPositiveFillWeight = true;
            this._accordionAlarmAreaAlarms.GrabWidth = 6;
            this._accordionAlarmAreaAlarms.GrowAndShrink = true;
            this._accordionAlarmAreaAlarms.Insets = new System.Windows.Forms.Padding(0);
            this._accordionAlarmAreaAlarms.Location = new System.Drawing.Point(6, 356);
            this._accordionAlarmAreaAlarms.Name = "_accordionAlarmAreaAlarms";
            this._accordionAlarmAreaAlarms.OpenOnAdd = false;
            this._accordionAlarmAreaAlarms.OpenOneOnly = false;
            this._accordionAlarmAreaAlarms.ResizeBarFactory = null;
            this._accordionAlarmAreaAlarms.ResizeBarsAlign = 0.5D;
            this._accordionAlarmAreaAlarms.ResizeBarsArrowKeyDelta = 10;
            this._accordionAlarmAreaAlarms.ResizeBarsFadeInMillis = 800;
            this._accordionAlarmAreaAlarms.ResizeBarsFadeOutMillis = 800;
            this._accordionAlarmAreaAlarms.ResizeBarsFadeProximity = 24;
            this._accordionAlarmAreaAlarms.ResizeBarsFill = 1D;
            this._accordionAlarmAreaAlarms.ResizeBarsKeepFocusAfterMouseDrag = false;
            this._accordionAlarmAreaAlarms.ResizeBarsKeepFocusIfControlOutOfView = true;
            this._accordionAlarmAreaAlarms.ResizeBarsKeepFocusOnClick = true;
            this._accordionAlarmAreaAlarms.ResizeBarsMargin = null;
            this._accordionAlarmAreaAlarms.ResizeBarsMinimumLength = 50;
            this._accordionAlarmAreaAlarms.ResizeBarsStayInViewOnArrowKey = true;
            this._accordionAlarmAreaAlarms.ResizeBarsStayInViewOnMouseDrag = true;
            this._accordionAlarmAreaAlarms.ResizeBarsStayVisibleIfFocused = true;
            this._accordionAlarmAreaAlarms.ResizeBarsTabStop = true;
            this._accordionAlarmAreaAlarms.ShowPartiallyVisibleResizeBars = false;
            this._accordionAlarmAreaAlarms.ShowToolMenu = true;
            this._accordionAlarmAreaAlarms.ShowToolMenuOnHoverWhenClosed = false;
            this._accordionAlarmAreaAlarms.ShowToolMenuOnRightClick = true;
            this._accordionAlarmAreaAlarms.ShowToolMenuRequiresPositiveFillWeight = false;
            this._accordionAlarmAreaAlarms.Size = new System.Drawing.Size(752, 221);
            this._accordionAlarmAreaAlarms.TabIndex = 23;
            this._accordionAlarmAreaAlarms.UpArrow = null;
            // 
            // _gbCardReaderEventlogEvents
            // 
            this._gbCardReaderEventlogEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbEnableEventLogs);
            this._gbCardReaderEventlogEvents.Controls.Add(this._lAlarmAreaEventSettings);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbPermanentlyBlockedEvent);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbSetEvent);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbTemporarilyBlockedEvent);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbUnsetEvent);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbUnblockedEvent);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbAlarmEvent);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbAcknowledgeEventSensor);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbNormalEvent);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbNormalEventSensor);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbAcknowledgeEvent);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbAlarmEventSensor);
            this._gbCardReaderEventlogEvents.Controls.Add(this._chbUnconditionalSet);
            this._gbCardReaderEventlogEvents.Controls.Add(this._lSensorEvents);
            this._gbCardReaderEventlogEvents.Location = new System.Drawing.Point(6, 149);
            this._gbCardReaderEventlogEvents.Name = "_gbCardReaderEventlogEvents";
            this._gbCardReaderEventlogEvents.Size = new System.Drawing.Size(610, 201);
            this._gbCardReaderEventlogEvents.TabIndex = 21;
            this._gbCardReaderEventlogEvents.TabStop = false;
            this._gbCardReaderEventlogEvents.Text = "CardReaderEventlogEvents";
            // 
            // _chbEnableEventLogs
            // 
            this._chbEnableEventLogs.AutoSize = true;
            this._chbEnableEventLogs.Location = new System.Drawing.Point(9, 19);
            this._chbEnableEventLogs.Name = "_chbEnableEventLogs";
            this._chbEnableEventLogs.Size = new System.Drawing.Size(158, 17);
            this._chbEnableEventLogs.TabIndex = 5;
            this._chbEnableEventLogs.Text = "Enable Event in card reader";
            this._chbEnableEventLogs.UseVisualStyleBackColor = true;
            this._chbEnableEventLogs.CheckedChanged += new System.EventHandler(this._chbEnableEventLogs_CheckedChanged);
            // 
            // _lAlarmAreaEventSettings
            // 
            this._lAlarmAreaEventSettings.AutoSize = true;
            this._lAlarmAreaEventSettings.Location = new System.Drawing.Point(6, 46);
            this._lAlarmAreaEventSettings.Name = "_lAlarmAreaEventSettings";
            this._lAlarmAreaEventSettings.Size = new System.Drawing.Size(58, 13);
            this._lAlarmAreaEventSettings.TabIndex = 6;
            this._lAlarmAreaEventSettings.Text = "Alarm Area";
            // 
            // _chbPermanentlyBlockedEvent
            // 
            this._chbPermanentlyBlockedEvent.AutoSize = true;
            this._chbPermanentlyBlockedEvent.Location = new System.Drawing.Point(183, 177);
            this._chbPermanentlyBlockedEvent.Name = "_chbPermanentlyBlockedEvent";
            this._chbPermanentlyBlockedEvent.Size = new System.Drawing.Size(125, 17);
            this._chbPermanentlyBlockedEvent.TabIndex = 19;
            this._chbPermanentlyBlockedEvent.Text = "Permanently blocked";
            this._chbPermanentlyBlockedEvent.UseVisualStyleBackColor = true;
            this._chbPermanentlyBlockedEvent.CheckedChanged += new System.EventHandler(this._chbPermanentlyBlockedEvent_CheckedChanged);
            // 
            // _chbSetEvent
            // 
            this._chbSetEvent.AutoSize = true;
            this._chbSetEvent.Location = new System.Drawing.Point(9, 62);
            this._chbSetEvent.Name = "_chbSetEvent";
            this._chbSetEvent.Size = new System.Drawing.Size(42, 17);
            this._chbSetEvent.TabIndex = 7;
            this._chbSetEvent.Text = "Set";
            this._chbSetEvent.UseVisualStyleBackColor = true;
            this._chbSetEvent.CheckedChanged += new System.EventHandler(this._chbSetEvent_CheckedChanged);
            // 
            // _chbTemporarilyBlockedEvent
            // 
            this._chbTemporarilyBlockedEvent.AutoSize = true;
            this._chbTemporarilyBlockedEvent.Location = new System.Drawing.Point(183, 154);
            this._chbTemporarilyBlockedEvent.Name = "_chbTemporarilyBlockedEvent";
            this._chbTemporarilyBlockedEvent.Size = new System.Drawing.Size(121, 17);
            this._chbTemporarilyBlockedEvent.TabIndex = 18;
            this._chbTemporarilyBlockedEvent.Text = "Temporarily blocked";
            this._chbTemporarilyBlockedEvent.UseVisualStyleBackColor = true;
            this._chbTemporarilyBlockedEvent.CheckedChanged += new System.EventHandler(this._chbTemporarilyBlockedEvent_CheckedChanged);
            // 
            // _chbUnsetEvent
            // 
            this._chbUnsetEvent.AutoSize = true;
            this._chbUnsetEvent.Location = new System.Drawing.Point(9, 85);
            this._chbUnsetEvent.Name = "_chbUnsetEvent";
            this._chbUnsetEvent.Size = new System.Drawing.Size(54, 17);
            this._chbUnsetEvent.TabIndex = 8;
            this._chbUnsetEvent.Text = "Unset";
            this._chbUnsetEvent.UseVisualStyleBackColor = true;
            this._chbUnsetEvent.CheckedChanged += new System.EventHandler(this._chbUnsetEvent_CheckedChanged);
            // 
            // _chbUnblockedEvent
            // 
            this._chbUnblockedEvent.AutoSize = true;
            this._chbUnblockedEvent.Location = new System.Drawing.Point(183, 131);
            this._chbUnblockedEvent.Name = "_chbUnblockedEvent";
            this._chbUnblockedEvent.Size = new System.Drawing.Size(78, 17);
            this._chbUnblockedEvent.TabIndex = 17;
            this._chbUnblockedEvent.Text = "Unblocked";
            this._chbUnblockedEvent.UseVisualStyleBackColor = true;
            this._chbUnblockedEvent.CheckedChanged += new System.EventHandler(this._chbUnblockedEvent_CheckedChanged);
            // 
            // _chbAlarmEvent
            // 
            this._chbAlarmEvent.AutoSize = true;
            this._chbAlarmEvent.Location = new System.Drawing.Point(9, 108);
            this._chbAlarmEvent.Name = "_chbAlarmEvent";
            this._chbAlarmEvent.Size = new System.Drawing.Size(52, 17);
            this._chbAlarmEvent.TabIndex = 9;
            this._chbAlarmEvent.Text = "Alarm";
            this._chbAlarmEvent.UseVisualStyleBackColor = true;
            this._chbAlarmEvent.CheckedChanged += new System.EventHandler(this._chbAlarmEvent_CheckedChanged);
            // 
            // _chbAcknowledgeEventSensor
            // 
            this._chbAcknowledgeEventSensor.AutoSize = true;
            this._chbAcknowledgeEventSensor.Location = new System.Drawing.Point(183, 108);
            this._chbAcknowledgeEventSensor.Name = "_chbAcknowledgeEventSensor";
            this._chbAcknowledgeEventSensor.Size = new System.Drawing.Size(91, 17);
            this._chbAcknowledgeEventSensor.TabIndex = 16;
            this._chbAcknowledgeEventSensor.Text = "Acknowledge";
            this._chbAcknowledgeEventSensor.UseVisualStyleBackColor = true;
            this._chbAcknowledgeEventSensor.CheckedChanged += new System.EventHandler(this._chbAcknowledgeEventSensor_CheckedChanged);
            // 
            // _chbNormalEvent
            // 
            this._chbNormalEvent.AutoSize = true;
            this._chbNormalEvent.Location = new System.Drawing.Point(9, 131);
            this._chbNormalEvent.Name = "_chbNormalEvent";
            this._chbNormalEvent.Size = new System.Drawing.Size(59, 17);
            this._chbNormalEvent.TabIndex = 10;
            this._chbNormalEvent.Text = "Normal";
            this._chbNormalEvent.UseVisualStyleBackColor = true;
            this._chbNormalEvent.CheckedChanged += new System.EventHandler(this._chbNormalEvent_CheckedChanged);
            // 
            // _chbNormalEventSensor
            // 
            this._chbNormalEventSensor.AutoSize = true;
            this._chbNormalEventSensor.Location = new System.Drawing.Point(183, 85);
            this._chbNormalEventSensor.Name = "_chbNormalEventSensor";
            this._chbNormalEventSensor.Size = new System.Drawing.Size(59, 17);
            this._chbNormalEventSensor.TabIndex = 15;
            this._chbNormalEventSensor.Text = "Normal";
            this._chbNormalEventSensor.UseVisualStyleBackColor = true;
            this._chbNormalEventSensor.CheckedChanged += new System.EventHandler(this._chbNormalEventSensor_CheckedChanged);
            // 
            // _chbAcknowledgeEvent
            // 
            this._chbAcknowledgeEvent.AutoSize = true;
            this._chbAcknowledgeEvent.Location = new System.Drawing.Point(9, 154);
            this._chbAcknowledgeEvent.Name = "_chbAcknowledgeEvent";
            this._chbAcknowledgeEvent.Size = new System.Drawing.Size(97, 17);
            this._chbAcknowledgeEvent.TabIndex = 11;
            this._chbAcknowledgeEvent.Text = "Acknowledged";
            this._chbAcknowledgeEvent.UseVisualStyleBackColor = true;
            this._chbAcknowledgeEvent.CheckedChanged += new System.EventHandler(this._chbAcknowledgeEvent_CheckedChanged);
            // 
            // _chbAlarmEventSensor
            // 
            this._chbAlarmEventSensor.AutoSize = true;
            this._chbAlarmEventSensor.Location = new System.Drawing.Point(183, 62);
            this._chbAlarmEventSensor.Name = "_chbAlarmEventSensor";
            this._chbAlarmEventSensor.Size = new System.Drawing.Size(52, 17);
            this._chbAlarmEventSensor.TabIndex = 14;
            this._chbAlarmEventSensor.Text = "Alarm";
            this._chbAlarmEventSensor.UseVisualStyleBackColor = true;
            this._chbAlarmEventSensor.CheckedChanged += new System.EventHandler(this._chbAlarmEventSensor_CheckedChanged);
            // 
            // _chbUnconditionalSet
            // 
            this._chbUnconditionalSet.AutoSize = true;
            this._chbUnconditionalSet.Location = new System.Drawing.Point(9, 177);
            this._chbUnconditionalSet.Name = "_chbUnconditionalSet";
            this._chbUnconditionalSet.Size = new System.Drawing.Size(108, 17);
            this._chbUnconditionalSet.TabIndex = 12;
            this._chbUnconditionalSet.Text = "Unconditional set";
            this._chbUnconditionalSet.UseVisualStyleBackColor = true;
            this._chbUnconditionalSet.Visible = false;
            this._chbUnconditionalSet.CheckedChanged += new System.EventHandler(this._chbUnconditionalSet_CheckedChanged);
            // 
            // _lSensorEvents
            // 
            this._lSensorEvents.AutoSize = true;
            this._lSensorEvents.Location = new System.Drawing.Point(180, 46);
            this._lSensorEvents.Name = "_lSensorEvents";
            this._lSensorEvents.Size = new System.Drawing.Size(45, 13);
            this._lSensorEvents.TabIndex = 13;
            this._lSensorEvents.Text = "Sensors";
            // 
            // _gbABAlarmHandling
            // 
            this._gbABAlarmHandling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbABAlarmHandling.Controls.Add(this._lPercentageSensorsToAAlarmActual);
            this._gbABAlarmHandling.Controls.Add(this._tbPercentageSensorsToAAlarm);
            this._gbABAlarmHandling.Controls.Add(this._lPercentageSensorsToAAlarm);
            this._gbABAlarmHandling.Controls.Add(this._chbABAlarmHandling);
            this._gbABAlarmHandling.Location = new System.Drawing.Point(6, 26);
            this._gbABAlarmHandling.Name = "_gbABAlarmHandling";
            this._gbABAlarmHandling.Size = new System.Drawing.Size(610, 117);
            this._gbABAlarmHandling.TabIndex = 3;
            this._gbABAlarmHandling.TabStop = false;
            this._gbABAlarmHandling.Text = "A-B alarm handling";
            // 
            // _lPercentageSensorsToAAlarmActual
            // 
            this._lPercentageSensorsToAAlarmActual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lPercentageSensorsToAAlarmActual.AutoSize = true;
            this._lPercentageSensorsToAAlarmActual.Location = new System.Drawing.Point(579, 75);
            this._lPercentageSensorsToAAlarmActual.Name = "_lPercentageSensorsToAAlarmActual";
            this._lPercentageSensorsToAAlarmActual.Size = new System.Drawing.Size(13, 13);
            this._lPercentageSensorsToAAlarmActual.TabIndex = 3;
            this._lPercentageSensorsToAAlarmActual.Text = "0";
            // 
            // _tbPercentageSensorsToAAlarm
            // 
            this._tbPercentageSensorsToAAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbPercentageSensorsToAAlarm.Location = new System.Drawing.Point(6, 65);
            this._tbPercentageSensorsToAAlarm.Maximum = 100;
            this._tbPercentageSensorsToAAlarm.Name = "_tbPercentageSensorsToAAlarm";
            this._tbPercentageSensorsToAAlarm.Size = new System.Drawing.Size(567, 45);
            this._tbPercentageSensorsToAAlarm.TabIndex = 2;
            this._tbPercentageSensorsToAAlarm.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lPercentageSensorsToAAlarm
            // 
            this._lPercentageSensorsToAAlarm.AutoSize = true;
            this._lPercentageSensorsToAAlarm.Location = new System.Drawing.Point(3, 49);
            this._lPercentageSensorsToAAlarm.Name = "_lPercentageSensorsToAAlarm";
            this._lPercentageSensorsToAAlarm.Size = new System.Drawing.Size(213, 13);
            this._lPercentageSensorsToAAlarm.TabIndex = 1;
            this._lPercentageSensorsToAAlarm.Text = "Percentage of sensors to invoke an A-alarm";
            // 
            // _chbABAlarmHandling
            // 
            this._chbABAlarmHandling.AutoSize = true;
            this._chbABAlarmHandling.Location = new System.Drawing.Point(9, 19);
            this._chbABAlarmHandling.Name = "_chbABAlarmHandling";
            this._chbABAlarmHandling.Size = new System.Drawing.Size(150, 17);
            this._chbABAlarmHandling.TabIndex = 0;
            this._chbABAlarmHandling.Text = "Enable A-B alarm handling";
            this._chbABAlarmHandling.UseVisualStyleBackColor = true;
            this._chbABAlarmHandling.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tpSpecialOutputs
            // 
            this._tpSpecialOutputs.AutoScroll = true;
            this._tpSpecialOutputs.AutoScrollMinSize = new System.Drawing.Size(0, 682);
            this._tpSpecialOutputs.BackColor = System.Drawing.SystemColors.Control;
            this._tpSpecialOutputs.Controls.Add(this.panel1);
            this._tpSpecialOutputs.Location = new System.Drawing.Point(4, 22);
            this._tpSpecialOutputs.Name = "_tpSpecialOutputs";
            this._tpSpecialOutputs.Padding = new System.Windows.Forms.Padding(3);
            this._tpSpecialOutputs.Size = new System.Drawing.Size(1097, 580);
            this._tpSpecialOutputs.TabIndex = 5;
            this._tpSpecialOutputs.Text = "Special outputs";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this._gbOutputSetByObjectForAaFailed);
            this.panel1.Controls.Add(this._gbOutputMotion);
            this.panel1.Controls.Add(this._gbOutpuSabotage);
            this.panel1.Controls.Add(this._gbOutputActivation);
            this.panel1.Controls.Add(this._gbOutputNotAcknowledged);
            this.panel1.Controls.Add(this._gbOutputAlarmState);
            this.panel1.Controls.Add(this._gbOutputPrewarning);
            this.panel1.Controls.Add(this._gbOutputTmpUnsetEntry);
            this.panel1.Controls.Add(this._gbSirenOutput);
            this.panel1.Controls.Add(this._gbOutputTmpUnsetExit);
            this.panel1.Controls.Add(this._gbOutputAAlarm);
            this.panel1.Location = new System.Drawing.Point(0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(689, 679);
            this.panel1.TabIndex = 5;
            // 
            // _gbOutputSetByObjectForAaFailed
            // 
            this._gbOutputSetByObjectForAaFailed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputSetByObjectForAaFailed.Controls.Add(this._eOutputSetAaNotCalmOnPeriod);
            this._gbOutputSetByObjectForAaFailed.Controls.Add(this._lOutputSetAaNotCalmOnPeriod);
            this._gbOutputSetByObjectForAaFailed.Controls.Add(this._lOutput11);
            this._gbOutputSetByObjectForAaFailed.Controls.Add(this._tbmOutputSetByObjectForAaFailed);
            this._gbOutputSetByObjectForAaFailed.Location = new System.Drawing.Point(2, 599);
            this._gbOutputSetByObjectForAaFailed.Name = "_gbOutputSetByObjectForAaFailed";
            this._gbOutputSetByObjectForAaFailed.Size = new System.Drawing.Size(685, 77);
            this._gbOutputSetByObjectForAaFailed.TabIndex = 6;
            this._gbOutputSetByObjectForAaFailed.TabStop = false;
            this._gbOutputSetByObjectForAaFailed.Text = "Output set by object for automatic activation failed";
            // 
            // _eOutputSetAaNotCalmOnPeriod
            // 
            this._eOutputSetAaNotCalmOnPeriod.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this._eOutputSetAaNotCalmOnPeriod.Location = new System.Drawing.Point(134, 47);
            this._eOutputSetAaNotCalmOnPeriod.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._eOutputSetAaNotCalmOnPeriod.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._eOutputSetAaNotCalmOnPeriod.Name = "_eOutputSetAaNotCalmOnPeriod";
            this._eOutputSetAaNotCalmOnPeriod.Size = new System.Drawing.Size(86, 20);
            this._eOutputSetAaNotCalmOnPeriod.TabIndex = 5;
            this._eOutputSetAaNotCalmOnPeriod.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._eOutputSetAaNotCalmOnPeriod.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lOutputSetAaNotCalmOnPeriod
            // 
            this._lOutputSetAaNotCalmOnPeriod.AutoSize = true;
            this._lOutputSetAaNotCalmOnPeriod.Location = new System.Drawing.Point(6, 50);
            this._lOutputSetAaNotCalmOnPeriod.Name = "_lOutputSetAaNotCalmOnPeriod";
            this._lOutputSetAaNotCalmOnPeriod.Size = new System.Drawing.Size(112, 13);
            this._lOutputSetAaNotCalmOnPeriod.TabIndex = 4;
            this._lOutputSetAaNotCalmOnPeriod.Text = "Output ON period (ms)";
            // 
            // _lOutput11
            // 
            this._lOutput11.AutoSize = true;
            this._lOutput11.Location = new System.Drawing.Point(6, 23);
            this._lOutput11.Name = "_lOutput11";
            this._lOutput11.Size = new System.Drawing.Size(39, 13);
            this._lOutput11.TabIndex = 3;
            this._lOutput11.Text = "Output";
            // 
            // _tbmOutputSetByObjectForAaFailed
            // 
            this._tbmOutputSetByObjectForAaFailed.AllowDrop = true;
            this._tbmOutputSetByObjectForAaFailed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSetByObjectForAaFailed.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputSetByObjectForAaFailed.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSetByObjectForAaFailed.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSetByObjectForAaFailed.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputSetByObjectForAaFailed.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSetByObjectForAaFailed.Button.Image")));
            this._tbmOutputSetByObjectForAaFailed.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputSetByObjectForAaFailed.Button.Name = "_bMenu";
            this._tbmOutputSetByObjectForAaFailed.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputSetByObjectForAaFailed.Button.TabIndex = 3;
            this._tbmOutputSetByObjectForAaFailed.Button.UseVisualStyleBackColor = false;
            this._tbmOutputSetByObjectForAaFailed.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSetByObjectForAaFailed.ButtonDefaultBehaviour = true;
            this._tbmOutputSetByObjectForAaFailed.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmOutputSetByObjectForAaFailed.ButtonImage = null;
            // 
            // 
            // 
            this._tbmOutputSetByObjectForAaFailed.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify17,
            this._tsiRemove17});
            this._tbmOutputSetByObjectForAaFailed.ButtonPopupMenu.Name = "";
            this._tbmOutputSetByObjectForAaFailed.ButtonPopupMenu.Size = new System.Drawing.Size(118, 48);
            this._tbmOutputSetByObjectForAaFailed.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputSetByObjectForAaFailed.ButtonShowImage = true;
            this._tbmOutputSetByObjectForAaFailed.ButtonSizeHeight = 20;
            this._tbmOutputSetByObjectForAaFailed.ButtonSizeWidth = 20;
            this._tbmOutputSetByObjectForAaFailed.ButtonText = "";
            this._tbmOutputSetByObjectForAaFailed.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSetByObjectForAaFailed.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.BackColor = System.Drawing.Color.White;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.ContextMenuStrip = this._tbmOutputSetByObjectForAaFailed.ButtonPopupMenu;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.Image = null;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.Name = "_itbTextBox";
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.ReadOnly = false;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.UseImage = true;
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputSetByObjectForAaFailed.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputSetByObjectForAaFailed_ImageTextBox_DoubleClick);
            this._tbmOutputSetByObjectForAaFailed.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputSetByObjectForAaFailed.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputSetByObjectForAaFailed.MinimumSize = new System.Drawing.Size(30, 20);
            this._tbmOutputSetByObjectForAaFailed.Name = "_tbmOutputSetByObjectForAaFailed";
            this._tbmOutputSetByObjectForAaFailed.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputSetByObjectForAaFailed.TabIndex = 0;
            this._tbmOutputSetByObjectForAaFailed.TextImage = null;
            this._tbmOutputSetByObjectForAaFailed.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmOutputSetByObjectForAaFailed_ButtonPopupMenuItemClick);
            this._tbmOutputSetByObjectForAaFailed.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputSetByObjectForAaFailed_DragDrop);
            this._tbmOutputSetByObjectForAaFailed.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify17
            // 
            this._tsiModify17.Name = "_tsiModify17";
            this._tsiModify17.Size = new System.Drawing.Size(117, 22);
            this._tsiModify17.Text = "Modify";
            // 
            // _tsiRemove17
            // 
            this._tsiRemove17.Name = "_tsiRemove17";
            this._tsiRemove17.Size = new System.Drawing.Size(117, 22);
            this._tsiRemove17.Text = "Remove";
            // 
            // _gbOutputMotion
            // 
            this._gbOutputMotion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputMotion.Controls.Add(this._tbmOutputMotion);
            this._gbOutputMotion.Controls.Add(this._lOutput10);
            this._gbOutputMotion.Location = new System.Drawing.Point(3, 542);
            this._gbOutputMotion.Name = "_gbOutputMotion";
            this._gbOutputMotion.Size = new System.Drawing.Size(685, 51);
            this._gbOutputMotion.TabIndex = 10;
            this._gbOutputMotion.TabStop = false;
            this._gbOutputMotion.Text = "Output motion";
            // 
            // _tbmOutputMotion
            // 
            this._tbmOutputMotion.AllowDrop = true;
            this._tbmOutputMotion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputMotion.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputMotion.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputMotion.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputMotion.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputMotion.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputMotion.Button.Image")));
            this._tbmOutputMotion.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputMotion.Button.Name = "_bMenu";
            this._tbmOutputMotion.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputMotion.Button.TabIndex = 3;
            this._tbmOutputMotion.Button.UseVisualStyleBackColor = false;
            this._tbmOutputMotion.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputMotion.ButtonDefaultBehaviour = true;
            this._tbmOutputMotion.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmOutputMotion.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputMotion.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputMotion.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify15,
            this._tsiRemove15});
            this._tbmOutputMotion.ButtonPopupMenu.Name = "";
            this._tbmOutputMotion.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputMotion.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputMotion.ButtonShowImage = true;
            this._tbmOutputMotion.ButtonSizeHeight = 20;
            this._tbmOutputMotion.ButtonSizeWidth = 20;
            this._tbmOutputMotion.ButtonText = "";
            this._tbmOutputMotion.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputMotion.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputMotion.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputMotion.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputMotion.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputMotion.ImageTextBox.ContextMenuStrip = this._tbmOutputMotion.ButtonPopupMenu;
            this._tbmOutputMotion.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputMotion.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputMotion.ImageTextBox.Image")));
            this._tbmOutputMotion.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputMotion.ImageTextBox.Name = "_itbTextBox";
            this._tbmOutputMotion.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputMotion.ImageTextBox.ReadOnly = false;
            this._tbmOutputMotion.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputMotion.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmOutputMotion.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputMotion.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputMotion.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputMotion.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputMotion.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputMotion.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputMotion.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputMotion.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputMotion.ImageTextBox.UseImage = true;
            this._tbmOutputMotion.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputMotion.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputMotion_ImageTextBox_DoubleClick);
            this._tbmOutputMotion.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputMotion.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputMotion.MinimumSize = new System.Drawing.Size(30, 20);
            this._tbmOutputMotion.Name = "_tbmOutputMotion";
            this._tbmOutputMotion.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputMotion.TabIndex = 3;
            this._tbmOutputMotion.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputMotion.TextImage")));
            this._tbmOutputMotion.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputMotion.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputMotion_DragDrop);
            this._tbmOutputMotion.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify15
            // 
            this._tsiModify15.Name = "_tsiModify15";
            this._tsiModify15.Size = new System.Drawing.Size(180, 22);
            this._tsiModify15.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove15
            // 
            this._tsiRemove15.Name = "_tsiRemove15";
            this._tsiRemove15.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove15.Text = "toolStripMenuItem1";
            // 
            // _lOutput10
            // 
            this._lOutput10.AutoSize = true;
            this._lOutput10.Location = new System.Drawing.Point(6, 23);
            this._lOutput10.Name = "_lOutput10";
            this._lOutput10.Size = new System.Drawing.Size(39, 13);
            this._lOutput10.TabIndex = 2;
            this._lOutput10.Text = "Output";
            // 
            // _gbOutpuSabotage
            // 
            this._gbOutpuSabotage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutpuSabotage.Controls.Add(this._tbmOutputSabotage);
            this._gbOutpuSabotage.Controls.Add(this._lOutput8);
            this._gbOutpuSabotage.Location = new System.Drawing.Point(3, 3);
            this._gbOutpuSabotage.Name = "_gbOutpuSabotage";
            this._gbOutpuSabotage.Size = new System.Drawing.Size(685, 51);
            this._gbOutpuSabotage.TabIndex = 1;
            this._gbOutpuSabotage.TabStop = false;
            this._gbOutpuSabotage.Text = "Output sabotage";
            // 
            // _tbmOutputSabotage
            // 
            this._tbmOutputSabotage.AllowDrop = true;
            this._tbmOutputSabotage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotage.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputSabotage.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotage.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSabotage.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputSabotage.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotage.Button.Image")));
            this._tbmOutputSabotage.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputSabotage.Button.Name = "_bMenu";
            this._tbmOutputSabotage.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputSabotage.Button.TabIndex = 3;
            this._tbmOutputSabotage.Button.UseVisualStyleBackColor = false;
            this._tbmOutputSabotage.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSabotage.ButtonDefaultBehaviour = true;
            this._tbmOutputSabotage.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmOutputSabotage.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotage.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputSabotage.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify13,
            this._tsiRemove13});
            this._tbmOutputSabotage.ButtonPopupMenu.Name = "";
            this._tbmOutputSabotage.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputSabotage.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputSabotage.ButtonShowImage = true;
            this._tbmOutputSabotage.ButtonSizeHeight = 20;
            this._tbmOutputSabotage.ButtonSizeWidth = 20;
            this._tbmOutputSabotage.ButtonText = "";
            this._tbmOutputSabotage.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotage.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputSabotage.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotage.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputSabotage.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputSabotage.ImageTextBox.ContextMenuStrip = this._tbmOutputSabotage.ButtonPopupMenu;
            this._tbmOutputSabotage.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotage.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotage.ImageTextBox.Image")));
            this._tbmOutputSabotage.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputSabotage.ImageTextBox.Name = "_itbTextBox";
            this._tbmOutputSabotage.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputSabotage.ImageTextBox.ReadOnly = false;
            this._tbmOutputSabotage.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputSabotage.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmOutputSabotage.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotage.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputSabotage.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputSabotage.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotage.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputSabotage.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputSabotage.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputSabotage.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputSabotage.ImageTextBox.UseImage = true;
            this._tbmOutputSabotage.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputSabotage.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputSabotageState_ImageTextBox_DoubleClick);
            this._tbmOutputSabotage.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputSabotage.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputSabotage.MinimumSize = new System.Drawing.Size(30, 20);
            this._tbmOutputSabotage.Name = "_tbmOutputSabotage";
            this._tbmOutputSabotage.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputSabotage.TabIndex = 2;
            this._tbmOutputSabotage.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotage.TextImage")));
            this._tbmOutputSabotage.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputSabotage.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputSabotageState_DragDrop);
            this._tbmOutputSabotage.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify13
            // 
            this._tsiModify13.Name = "_tsiModify13";
            this._tsiModify13.Size = new System.Drawing.Size(180, 22);
            this._tsiModify13.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove13
            // 
            this._tsiRemove13.Name = "_tsiRemove13";
            this._tsiRemove13.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove13.Text = "toolStripMenuItem2";
            // 
            // _lOutput8
            // 
            this._lOutput8.AutoSize = true;
            this._lOutput8.Location = new System.Drawing.Point(6, 23);
            this._lOutput8.Name = "_lOutput8";
            this._lOutput8.Size = new System.Drawing.Size(39, 13);
            this._lOutput8.TabIndex = 1;
            this._lOutput8.Text = "Output";
            // 
            // _gbOutputActivation
            // 
            this._gbOutputActivation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputActivation.Controls.Add(this._tbmOutputActivation);
            this._gbOutputActivation.Controls.Add(this._lOutput1);
            this._gbOutputActivation.Location = new System.Drawing.Point(3, 174);
            this._gbOutputActivation.Name = "_gbOutputActivation";
            this._gbOutputActivation.Size = new System.Drawing.Size(685, 51);
            this._gbOutputActivation.TabIndex = 4;
            this._gbOutputActivation.TabStop = false;
            this._gbOutputActivation.Text = "Output activation";
            // 
            // _tbmOutputActivation
            // 
            this._tbmOutputActivation.AllowDrop = true;
            this._tbmOutputActivation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputActivation.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputActivation.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputActivation.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputActivation.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputActivation.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputActivation.Button.Image")));
            this._tbmOutputActivation.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputActivation.Button.Name = "_bMenu";
            this._tbmOutputActivation.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputActivation.Button.TabIndex = 3;
            this._tbmOutputActivation.Button.UseVisualStyleBackColor = false;
            this._tbmOutputActivation.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputActivation.ButtonDefaultBehaviour = true;
            this._tbmOutputActivation.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmOutputActivation.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputActivation.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputActivation.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify4,
            this._tsiRemove4});
            this._tbmOutputActivation.ButtonPopupMenu.Name = "";
            this._tbmOutputActivation.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputActivation.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputActivation.ButtonShowImage = true;
            this._tbmOutputActivation.ButtonSizeHeight = 20;
            this._tbmOutputActivation.ButtonSizeWidth = 20;
            this._tbmOutputActivation.ButtonText = "";
            this._tbmOutputActivation.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputActivation.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputActivation.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputActivation.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputActivation.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputActivation.ImageTextBox.ContextMenuStrip = this._tbmOutputActivation.ButtonPopupMenu;
            this._tbmOutputActivation.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputActivation.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputActivation.ImageTextBox.Image")));
            this._tbmOutputActivation.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputActivation.ImageTextBox.Name = "_textBox";
            this._tbmOutputActivation.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputActivation.ImageTextBox.ReadOnly = true;
            this._tbmOutputActivation.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputActivation.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmOutputActivation.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputActivation.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputActivation.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputActivation.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputActivation.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputActivation.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputActivation.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputActivation.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputActivation.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputActivation.ImageTextBox.UseImage = true;
            this._tbmOutputActivation.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputActivation.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputActivation_TextBox_DoubleClick);
            this._tbmOutputActivation.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputActivation.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputActivation.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputActivation.Name = "_tbmOutputActivation";
            this._tbmOutputActivation.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputActivation.TabIndex = 2;
            this._tbmOutputActivation.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputActivation.TextImage")));
            this._tbmOutputActivation.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputActivation.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputActivation_DragDrop);
            this._tbmOutputActivation.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify4
            // 
            this._tsiModify4.Name = "_tsiModify4";
            this._tsiModify4.Size = new System.Drawing.Size(180, 22);
            this._tsiModify4.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove4
            // 
            this._tsiRemove4.Name = "_tsiRemove4";
            this._tsiRemove4.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove4.Text = "toolStripMenuItem2";
            // 
            // _lOutput1
            // 
            this._lOutput1.AutoSize = true;
            this._lOutput1.Location = new System.Drawing.Point(6, 23);
            this._lOutput1.Name = "_lOutput1";
            this._lOutput1.Size = new System.Drawing.Size(39, 13);
            this._lOutput1.TabIndex = 1;
            this._lOutput1.Text = "Output";
            // 
            // _gbOutputNotAcknowledged
            // 
            this._gbOutputNotAcknowledged.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputNotAcknowledged.Controls.Add(this._tbmOutputNotAcknowledged);
            this._gbOutputNotAcknowledged.Controls.Add(this._lOutput9);
            this._gbOutputNotAcknowledged.Location = new System.Drawing.Point(3, 485);
            this._gbOutputNotAcknowledged.Name = "_gbOutputNotAcknowledged";
            this._gbOutputNotAcknowledged.Size = new System.Drawing.Size(685, 51);
            this._gbOutputNotAcknowledged.TabIndex = 9;
            this._gbOutputNotAcknowledged.TabStop = false;
            this._gbOutputNotAcknowledged.Text = "Ouptut not acknowledged";
            // 
            // _tbmOutputNotAcknowledged
            // 
            this._tbmOutputNotAcknowledged.AllowDrop = true;
            this._tbmOutputNotAcknowledged.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputNotAcknowledged.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputNotAcknowledged.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputNotAcknowledged.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputNotAcknowledged.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputNotAcknowledged.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputNotAcknowledged.Button.Image")));
            this._tbmOutputNotAcknowledged.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputNotAcknowledged.Button.Name = "_bMenu";
            this._tbmOutputNotAcknowledged.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputNotAcknowledged.Button.TabIndex = 3;
            this._tbmOutputNotAcknowledged.Button.UseVisualStyleBackColor = false;
            this._tbmOutputNotAcknowledged.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputNotAcknowledged.ButtonDefaultBehaviour = true;
            this._tbmOutputNotAcknowledged.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmOutputNotAcknowledged.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputNotAcknowledged.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputNotAcknowledged.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify14,
            this._tsiRemove14});
            this._tbmOutputNotAcknowledged.ButtonPopupMenu.Name = "";
            this._tbmOutputNotAcknowledged.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputNotAcknowledged.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputNotAcknowledged.ButtonShowImage = true;
            this._tbmOutputNotAcknowledged.ButtonSizeHeight = 20;
            this._tbmOutputNotAcknowledged.ButtonSizeWidth = 20;
            this._tbmOutputNotAcknowledged.ButtonText = "";
            this._tbmOutputNotAcknowledged.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputNotAcknowledged.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputNotAcknowledged.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputNotAcknowledged.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputNotAcknowledged.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputNotAcknowledged.ImageTextBox.ContextMenuStrip = this._tbmOutputNotAcknowledged.ButtonPopupMenu;
            this._tbmOutputNotAcknowledged.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputNotAcknowledged.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputNotAcknowledged.ImageTextBox.Image")));
            this._tbmOutputNotAcknowledged.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputNotAcknowledged.ImageTextBox.Name = "_itbTextBox";
            this._tbmOutputNotAcknowledged.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputNotAcknowledged.ImageTextBox.ReadOnly = false;
            this._tbmOutputNotAcknowledged.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputNotAcknowledged.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmOutputNotAcknowledged.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputNotAcknowledged.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputNotAcknowledged.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputNotAcknowledged.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputNotAcknowledged.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputNotAcknowledged.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputNotAcknowledged.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputNotAcknowledged.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputNotAcknowledged.ImageTextBox.UseImage = true;
            this._tbmOutputNotAcknowledged.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputNotAcknowledged.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputNotAcknowledged_ImageTextBox_DoubleClick);
            this._tbmOutputNotAcknowledged.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputNotAcknowledged.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputNotAcknowledged.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputNotAcknowledged.Name = "_tbmOutputNotAcknowledged";
            this._tbmOutputNotAcknowledged.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputNotAcknowledged.TabIndex = 2;
            this._tbmOutputNotAcknowledged.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputNotAcknowledged.TextImage")));
            this._tbmOutputNotAcknowledged.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputNotAcknowledged.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputNotAcknowledged_DragDrop);
            this._tbmOutputNotAcknowledged.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify14
            // 
            this._tsiModify14.Name = "_tsiModify14";
            this._tsiModify14.Size = new System.Drawing.Size(180, 22);
            this._tsiModify14.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove14
            // 
            this._tsiRemove14.Name = "_tsiRemove14";
            this._tsiRemove14.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove14.Text = "toolStripMenuItem1";
            // 
            // _lOutput9
            // 
            this._lOutput9.AutoSize = true;
            this._lOutput9.Location = new System.Drawing.Point(6, 23);
            this._lOutput9.Name = "_lOutput9";
            this._lOutput9.Size = new System.Drawing.Size(39, 13);
            this._lOutput9.TabIndex = 1;
            this._lOutput9.Text = "Output";
            // 
            // _gbOutputAlarmState
            // 
            this._gbOutputAlarmState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputAlarmState.Controls.Add(this._tbmOutputAlarmState);
            this._gbOutputAlarmState.Controls.Add(this._lOutput2);
            this._gbOutputAlarmState.Location = new System.Drawing.Point(3, 288);
            this._gbOutputAlarmState.Name = "_gbOutputAlarmState";
            this._gbOutputAlarmState.Size = new System.Drawing.Size(685, 51);
            this._gbOutputAlarmState.TabIndex = 6;
            this._gbOutputAlarmState.TabStop = false;
            this._gbOutputAlarmState.Text = "Output alarm state";
            // 
            // _tbmOutputAlarmState
            // 
            this._tbmOutputAlarmState.AllowDrop = true;
            this._tbmOutputAlarmState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputAlarmState.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputAlarmState.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputAlarmState.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputAlarmState.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputAlarmState.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputAlarmState.Button.Image")));
            this._tbmOutputAlarmState.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputAlarmState.Button.Name = "_bMenu";
            this._tbmOutputAlarmState.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputAlarmState.Button.TabIndex = 3;
            this._tbmOutputAlarmState.Button.UseVisualStyleBackColor = false;
            this._tbmOutputAlarmState.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputAlarmState.ButtonDefaultBehaviour = true;
            this._tbmOutputAlarmState.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmOutputAlarmState.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputAlarmState.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputAlarmState.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify5,
            this._tsiRemove5});
            this._tbmOutputAlarmState.ButtonPopupMenu.Name = "";
            this._tbmOutputAlarmState.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputAlarmState.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputAlarmState.ButtonShowImage = true;
            this._tbmOutputAlarmState.ButtonSizeHeight = 20;
            this._tbmOutputAlarmState.ButtonSizeWidth = 20;
            this._tbmOutputAlarmState.ButtonText = "";
            this._tbmOutputAlarmState.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputAlarmState.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputAlarmState.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputAlarmState.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputAlarmState.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputAlarmState.ImageTextBox.ContextMenuStrip = this._tbmOutputAlarmState.ButtonPopupMenu;
            this._tbmOutputAlarmState.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputAlarmState.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputAlarmState.ImageTextBox.Image")));
            this._tbmOutputAlarmState.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputAlarmState.ImageTextBox.Name = "_textBox";
            this._tbmOutputAlarmState.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputAlarmState.ImageTextBox.ReadOnly = true;
            this._tbmOutputAlarmState.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputAlarmState.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmOutputAlarmState.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputAlarmState.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputAlarmState.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputAlarmState.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputAlarmState.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputAlarmState.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputAlarmState.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputAlarmState.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputAlarmState.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputAlarmState.ImageTextBox.UseImage = true;
            this._tbmOutputAlarmState.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputAlarmState.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputAlarmState_TextBox_DoubleClick);
            this._tbmOutputAlarmState.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputAlarmState.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputAlarmState.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputAlarmState.Name = "_tbmOutputAlarmState";
            this._tbmOutputAlarmState.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputAlarmState.TabIndex = 2;
            this._tbmOutputAlarmState.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputAlarmState.TextImage")));
            this._tbmOutputAlarmState.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputAlarmState.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputAlarmState_DragDrop);
            this._tbmOutputAlarmState.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify5
            // 
            this._tsiModify5.Name = "_tsiModify5";
            this._tsiModify5.Size = new System.Drawing.Size(180, 22);
            this._tsiModify5.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove5
            // 
            this._tsiRemove5.Name = "_tsiRemove5";
            this._tsiRemove5.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove5.Text = "toolStripMenuItem2";
            // 
            // _lOutput2
            // 
            this._lOutput2.AutoSize = true;
            this._lOutput2.Location = new System.Drawing.Point(6, 23);
            this._lOutput2.Name = "_lOutput2";
            this._lOutput2.Size = new System.Drawing.Size(39, 13);
            this._lOutput2.TabIndex = 1;
            this._lOutput2.Text = "Output";
            // 
            // _gbOutputPrewarning
            // 
            this._gbOutputPrewarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputPrewarning.Controls.Add(this._tbmOutputPrewarning);
            this._gbOutputPrewarning.Controls.Add(this._lOutput3);
            this._gbOutputPrewarning.Location = new System.Drawing.Point(3, 60);
            this._gbOutputPrewarning.Name = "_gbOutputPrewarning";
            this._gbOutputPrewarning.Size = new System.Drawing.Size(685, 51);
            this._gbOutputPrewarning.TabIndex = 2;
            this._gbOutputPrewarning.TabStop = false;
            this._gbOutputPrewarning.Text = "Output prewarning";
            // 
            // _tbmOutputPrewarning
            // 
            this._tbmOutputPrewarning.AllowDrop = true;
            this._tbmOutputPrewarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputPrewarning.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputPrewarning.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputPrewarning.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputPrewarning.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputPrewarning.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputPrewarning.Button.Image")));
            this._tbmOutputPrewarning.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputPrewarning.Button.Name = "_bMenu";
            this._tbmOutputPrewarning.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputPrewarning.Button.TabIndex = 3;
            this._tbmOutputPrewarning.Button.UseVisualStyleBackColor = false;
            this._tbmOutputPrewarning.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputPrewarning.ButtonDefaultBehaviour = true;
            this._tbmOutputPrewarning.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmOutputPrewarning.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputPrewarning.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputPrewarning.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify6,
            this._tsiRemove6});
            this._tbmOutputPrewarning.ButtonPopupMenu.Name = "";
            this._tbmOutputPrewarning.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputPrewarning.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputPrewarning.ButtonShowImage = true;
            this._tbmOutputPrewarning.ButtonSizeHeight = 20;
            this._tbmOutputPrewarning.ButtonSizeWidth = 20;
            this._tbmOutputPrewarning.ButtonText = "";
            this._tbmOutputPrewarning.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputPrewarning.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputPrewarning.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputPrewarning.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputPrewarning.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputPrewarning.ImageTextBox.ContextMenuStrip = this._tbmOutputPrewarning.ButtonPopupMenu;
            this._tbmOutputPrewarning.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputPrewarning.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputPrewarning.ImageTextBox.Image")));
            this._tbmOutputPrewarning.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputPrewarning.ImageTextBox.Name = "_textBox";
            this._tbmOutputPrewarning.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputPrewarning.ImageTextBox.ReadOnly = true;
            this._tbmOutputPrewarning.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputPrewarning.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmOutputPrewarning.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputPrewarning.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputPrewarning.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputPrewarning.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputPrewarning.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputPrewarning.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputPrewarning.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputPrewarning.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputPrewarning.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputPrewarning.ImageTextBox.UseImage = true;
            this._tbmOutputPrewarning.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputPrewarning.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputPrewarning_TextBox_DoubleClick);
            this._tbmOutputPrewarning.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputPrewarning.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputPrewarning.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputPrewarning.Name = "_tbmOutputPrewarning";
            this._tbmOutputPrewarning.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputPrewarning.TabIndex = 2;
            this._tbmOutputPrewarning.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputPrewarning.TextImage")));
            this._tbmOutputPrewarning.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputPrewarning.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputPrewarning_DragDrop);
            this._tbmOutputPrewarning.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify6
            // 
            this._tsiModify6.Name = "_tsiModify6";
            this._tsiModify6.Size = new System.Drawing.Size(180, 22);
            this._tsiModify6.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove6
            // 
            this._tsiRemove6.Name = "_tsiRemove6";
            this._tsiRemove6.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove6.Text = "toolStripMenuItem2";
            // 
            // _lOutput3
            // 
            this._lOutput3.AutoSize = true;
            this._lOutput3.Location = new System.Drawing.Point(6, 23);
            this._lOutput3.Name = "_lOutput3";
            this._lOutput3.Size = new System.Drawing.Size(39, 13);
            this._lOutput3.TabIndex = 1;
            this._lOutput3.Text = "Output";
            // 
            // _gbOutputTmpUnsetEntry
            // 
            this._gbOutputTmpUnsetEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputTmpUnsetEntry.Controls.Add(this._tbmOutputTmpUnsetEntry);
            this._gbOutputTmpUnsetEntry.Controls.Add(this._lOutput4);
            this._gbOutputTmpUnsetEntry.Location = new System.Drawing.Point(3, 231);
            this._gbOutputTmpUnsetEntry.Name = "_gbOutputTmpUnsetEntry";
            this._gbOutputTmpUnsetEntry.Size = new System.Drawing.Size(685, 51);
            this._gbOutputTmpUnsetEntry.TabIndex = 5;
            this._gbOutputTmpUnsetEntry.TabStop = false;
            this._gbOutputTmpUnsetEntry.Text = "Output temporary unset entry";
            // 
            // _tbmOutputTmpUnsetEntry
            // 
            this._tbmOutputTmpUnsetEntry.AllowDrop = true;
            this._tbmOutputTmpUnsetEntry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputTmpUnsetEntry.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputTmpUnsetEntry.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputTmpUnsetEntry.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputTmpUnsetEntry.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputTmpUnsetEntry.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputTmpUnsetEntry.Button.Image")));
            this._tbmOutputTmpUnsetEntry.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputTmpUnsetEntry.Button.Name = "_bMenu";
            this._tbmOutputTmpUnsetEntry.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputTmpUnsetEntry.Button.TabIndex = 3;
            this._tbmOutputTmpUnsetEntry.Button.UseVisualStyleBackColor = false;
            this._tbmOutputTmpUnsetEntry.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputTmpUnsetEntry.ButtonDefaultBehaviour = true;
            this._tbmOutputTmpUnsetEntry.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmOutputTmpUnsetEntry.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputTmpUnsetEntry.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputTmpUnsetEntry.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify7,
            this._tsiRemove7});
            this._tbmOutputTmpUnsetEntry.ButtonPopupMenu.Name = "";
            this._tbmOutputTmpUnsetEntry.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputTmpUnsetEntry.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputTmpUnsetEntry.ButtonShowImage = true;
            this._tbmOutputTmpUnsetEntry.ButtonSizeHeight = 20;
            this._tbmOutputTmpUnsetEntry.ButtonSizeWidth = 20;
            this._tbmOutputTmpUnsetEntry.ButtonText = "";
            this._tbmOutputTmpUnsetEntry.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputTmpUnsetEntry.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputTmpUnsetEntry.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputTmpUnsetEntry.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.ContextMenuStrip = this._tbmOutputTmpUnsetEntry.ButtonPopupMenu;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputTmpUnsetEntry.ImageTextBox.Image")));
            this._tbmOutputTmpUnsetEntry.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputTmpUnsetEntry.ImageTextBox.Name = "_textBox";
            this._tbmOutputTmpUnsetEntry.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.ReadOnly = true;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.UseImage = true;
            this._tbmOutputTmpUnsetEntry.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputTmpUnsetEntry.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputTmpUnsetEntry_TextBox_DoubleClick);
            this._tbmOutputTmpUnsetEntry.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputTmpUnsetEntry.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputTmpUnsetEntry.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputTmpUnsetEntry.Name = "_tbmOutputTmpUnsetEntry";
            this._tbmOutputTmpUnsetEntry.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputTmpUnsetEntry.TabIndex = 2;
            this._tbmOutputTmpUnsetEntry.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputTmpUnsetEntry.TextImage")));
            this._tbmOutputTmpUnsetEntry.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputTmpUnsetEntry.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputTmpUnsetEntry_DragDrop);
            this._tbmOutputTmpUnsetEntry.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify7
            // 
            this._tsiModify7.Name = "_tsiModify7";
            this._tsiModify7.Size = new System.Drawing.Size(180, 22);
            this._tsiModify7.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove7
            // 
            this._tsiRemove7.Name = "_tsiRemove7";
            this._tsiRemove7.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove7.Text = "toolStripMenuItem2";
            // 
            // _lOutput4
            // 
            this._lOutput4.AutoSize = true;
            this._lOutput4.Location = new System.Drawing.Point(6, 23);
            this._lOutput4.Name = "_lOutput4";
            this._lOutput4.Size = new System.Drawing.Size(39, 13);
            this._lOutput4.TabIndex = 1;
            this._lOutput4.Text = "Output";
            // 
            // _gbSirenOutput
            // 
            this._gbSirenOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbSirenOutput.Controls.Add(this._lMaximumOnPperiod);
            this._gbSirenOutput.Controls.Add(this._dtpSirenMaximumOnPeriod);
            this._gbSirenOutput.Controls.Add(this._tbmOutputSiren);
            this._gbSirenOutput.Controls.Add(this._lOutput7);
            this._gbSirenOutput.Location = new System.Drawing.Point(3, 345);
            this._gbSirenOutput.Name = "_gbSirenOutput";
            this._gbSirenOutput.Size = new System.Drawing.Size(685, 77);
            this._gbSirenOutput.TabIndex = 7;
            this._gbSirenOutput.TabStop = false;
            this._gbSirenOutput.Text = "Output alarm state to siren";
            // 
            // _lMaximumOnPperiod
            // 
            this._lMaximumOnPperiod.AutoSize = true;
            this._lMaximumOnPperiod.Location = new System.Drawing.Point(6, 50);
            this._lMaximumOnPperiod.Name = "_lMaximumOnPperiod";
            this._lMaximumOnPperiod.Size = new System.Drawing.Size(140, 13);
            this._lMaximumOnPperiod.TabIndex = 3;
            this._lMaximumOnPperiod.Text = "Maximum ON period (mm:ss)";
            // 
            // _dtpSirenMaximumOnPeriod
            // 
            this._dtpSirenMaximumOnPeriod.CustomFormat = "mm:ss";
            this._dtpSirenMaximumOnPeriod.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._dtpSirenMaximumOnPeriod.Location = new System.Drawing.Point(134, 47);
            this._dtpSirenMaximumOnPeriod.Name = "_dtpSirenMaximumOnPeriod";
            this._dtpSirenMaximumOnPeriod.ShowUpDown = true;
            this._dtpSirenMaximumOnPeriod.Size = new System.Drawing.Size(86, 20);
            this._dtpSirenMaximumOnPeriod.TabIndex = 4;
            this._dtpSirenMaximumOnPeriod.Value = new System.DateTime(2013, 3, 12, 0, 15, 0, 0);
            this._dtpSirenMaximumOnPeriod.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tbmOutputSiren
            // 
            this._tbmOutputSiren.AllowDrop = true;
            this._tbmOutputSiren.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSiren.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputSiren.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSiren.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSiren.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputSiren.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSiren.Button.Image")));
            this._tbmOutputSiren.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputSiren.Button.Name = "_bMenu";
            this._tbmOutputSiren.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputSiren.Button.TabIndex = 3;
            this._tbmOutputSiren.Button.UseVisualStyleBackColor = false;
            this._tbmOutputSiren.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSiren.ButtonDefaultBehaviour = true;
            this._tbmOutputSiren.ButtonHoverColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this._tbmOutputSiren.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSiren.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputSiren.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify10,
            this._tsiRemove10});
            this._tbmOutputSiren.ButtonPopupMenu.Name = "";
            this._tbmOutputSiren.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputSiren.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputSiren.ButtonShowImage = true;
            this._tbmOutputSiren.ButtonSizeHeight = 20;
            this._tbmOutputSiren.ButtonSizeWidth = 20;
            this._tbmOutputSiren.ButtonText = "";
            this._tbmOutputSiren.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSiren.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputSiren.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSiren.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputSiren.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputSiren.ImageTextBox.ContextMenuStrip = this._tbmOutputSiren.ButtonPopupMenu;
            this._tbmOutputSiren.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSiren.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSiren.ImageTextBox.Image")));
            this._tbmOutputSiren.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputSiren.ImageTextBox.Name = "_itbTextBox";
            this._tbmOutputSiren.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputSiren.ImageTextBox.ReadOnly = true;
            this._tbmOutputSiren.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputSiren.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmOutputSiren.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSiren.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmOutputSiren.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputSiren.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSiren.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputSiren.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputSiren.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputSiren.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputSiren.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputSiren.ImageTextBox.UseImage = true;
            this._tbmOutputSiren.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputSiren.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputSiren_ImageTextBox_DoubleClick);
            this._tbmOutputSiren.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputSiren.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputSiren.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputSiren.Name = "_tbmOutputSiren";
            this._tbmOutputSiren.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputSiren.TabIndex = 2;
            this._tbmOutputSiren.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSiren.TextImage")));
            this._tbmOutputSiren.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputSiren.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputSiren_DragDrop);
            this._tbmOutputSiren.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify10
            // 
            this._tsiModify10.Name = "_tsiModify10";
            this._tsiModify10.Size = new System.Drawing.Size(180, 22);
            this._tsiModify10.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove10
            // 
            this._tsiRemove10.Name = "_tsiRemove10";
            this._tsiRemove10.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove10.Text = "toolStripMenuItem1";
            // 
            // _lOutput7
            // 
            this._lOutput7.AutoSize = true;
            this._lOutput7.Location = new System.Drawing.Point(6, 23);
            this._lOutput7.Name = "_lOutput7";
            this._lOutput7.Size = new System.Drawing.Size(39, 13);
            this._lOutput7.TabIndex = 1;
            this._lOutput7.Text = "Output";
            // 
            // _gbOutputTmpUnsetExit
            // 
            this._gbOutputTmpUnsetExit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputTmpUnsetExit.Controls.Add(this._tbmOutputTmpUnsetExit);
            this._gbOutputTmpUnsetExit.Controls.Add(this._lOutput5);
            this._gbOutputTmpUnsetExit.Location = new System.Drawing.Point(3, 117);
            this._gbOutputTmpUnsetExit.Name = "_gbOutputTmpUnsetExit";
            this._gbOutputTmpUnsetExit.Size = new System.Drawing.Size(685, 51);
            this._gbOutputTmpUnsetExit.TabIndex = 3;
            this._gbOutputTmpUnsetExit.TabStop = false;
            this._gbOutputTmpUnsetExit.Text = "Output temporary unset exit";
            // 
            // _tbmOutputTmpUnsetExit
            // 
            this._tbmOutputTmpUnsetExit.AllowDrop = true;
            this._tbmOutputTmpUnsetExit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputTmpUnsetExit.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputTmpUnsetExit.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputTmpUnsetExit.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputTmpUnsetExit.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputTmpUnsetExit.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputTmpUnsetExit.Button.Image")));
            this._tbmOutputTmpUnsetExit.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputTmpUnsetExit.Button.Name = "_bMenu";
            this._tbmOutputTmpUnsetExit.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputTmpUnsetExit.Button.TabIndex = 3;
            this._tbmOutputTmpUnsetExit.Button.UseVisualStyleBackColor = false;
            this._tbmOutputTmpUnsetExit.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputTmpUnsetExit.ButtonDefaultBehaviour = true;
            this._tbmOutputTmpUnsetExit.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmOutputTmpUnsetExit.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputTmpUnsetExit.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputTmpUnsetExit.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify8,
            this._tsiRemove8});
            this._tbmOutputTmpUnsetExit.ButtonPopupMenu.Name = "";
            this._tbmOutputTmpUnsetExit.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputTmpUnsetExit.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputTmpUnsetExit.ButtonShowImage = true;
            this._tbmOutputTmpUnsetExit.ButtonSizeHeight = 20;
            this._tbmOutputTmpUnsetExit.ButtonSizeWidth = 20;
            this._tbmOutputTmpUnsetExit.ButtonText = "";
            this._tbmOutputTmpUnsetExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputTmpUnsetExit.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputTmpUnsetExit.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputTmpUnsetExit.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputTmpUnsetExit.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputTmpUnsetExit.ImageTextBox.ContextMenuStrip = this._tbmOutputTmpUnsetExit.ButtonPopupMenu;
            this._tbmOutputTmpUnsetExit.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputTmpUnsetExit.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputTmpUnsetExit.ImageTextBox.Image")));
            this._tbmOutputTmpUnsetExit.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputTmpUnsetExit.ImageTextBox.Name = "_textBox";
            this._tbmOutputTmpUnsetExit.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputTmpUnsetExit.ImageTextBox.ReadOnly = true;
            this._tbmOutputTmpUnsetExit.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputTmpUnsetExit.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputTmpUnsetExit.ImageTextBox.UseImage = true;
            this._tbmOutputTmpUnsetExit.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputTmpUnsetExit.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputTmpUnsetExit_TextBox_DoubleClick);
            this._tbmOutputTmpUnsetExit.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputTmpUnsetExit.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputTmpUnsetExit.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputTmpUnsetExit.Name = "_tbmOutputTmpUnsetExit";
            this._tbmOutputTmpUnsetExit.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputTmpUnsetExit.TabIndex = 2;
            this._tbmOutputTmpUnsetExit.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputTmpUnsetExit.TextImage")));
            this._tbmOutputTmpUnsetExit.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputTmpUnsetExit.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputTmpUnsetExit_DragDrop);
            this._tbmOutputTmpUnsetExit.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify8
            // 
            this._tsiModify8.Name = "_tsiModify8";
            this._tsiModify8.Size = new System.Drawing.Size(180, 22);
            this._tsiModify8.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove8
            // 
            this._tsiRemove8.Name = "_tsiRemove8";
            this._tsiRemove8.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove8.Text = "toolStripMenuItem2";
            // 
            // _lOutput5
            // 
            this._lOutput5.AutoSize = true;
            this._lOutput5.Location = new System.Drawing.Point(6, 23);
            this._lOutput5.Name = "_lOutput5";
            this._lOutput5.Size = new System.Drawing.Size(39, 13);
            this._lOutput5.TabIndex = 1;
            this._lOutput5.Text = "Output";
            // 
            // _gbOutputAAlarm
            // 
            this._gbOutputAAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputAAlarm.Controls.Add(this._tbmOutputAAlarm);
            this._gbOutputAAlarm.Controls.Add(this._lOutput6);
            this._gbOutputAAlarm.Location = new System.Drawing.Point(3, 428);
            this._gbOutputAAlarm.Name = "_gbOutputAAlarm";
            this._gbOutputAAlarm.Size = new System.Drawing.Size(685, 51);
            this._gbOutputAAlarm.TabIndex = 8;
            this._gbOutputAAlarm.TabStop = false;
            this._gbOutputAAlarm.Text = "Output A-alarm";
            // 
            // _tbmOutputAAlarm
            // 
            this._tbmOutputAAlarm.AllowDrop = true;
            this._tbmOutputAAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputAAlarm.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputAAlarm.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputAAlarm.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputAAlarm.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputAAlarm.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputAAlarm.Button.Image")));
            this._tbmOutputAAlarm.Button.Location = new System.Drawing.Point(525, 0);
            this._tbmOutputAAlarm.Button.Name = "_bMenu";
            this._tbmOutputAAlarm.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputAAlarm.Button.TabIndex = 3;
            this._tbmOutputAAlarm.Button.UseVisualStyleBackColor = false;
            this._tbmOutputAAlarm.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputAAlarm.ButtonDefaultBehaviour = true;
            this._tbmOutputAAlarm.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmOutputAAlarm.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputAAlarm.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputAAlarm.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify9,
            this._tsiRemove9});
            this._tbmOutputAAlarm.ButtonPopupMenu.Name = "";
            this._tbmOutputAAlarm.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputAAlarm.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputAAlarm.ButtonShowImage = true;
            this._tbmOutputAAlarm.ButtonSizeHeight = 20;
            this._tbmOutputAAlarm.ButtonSizeWidth = 20;
            this._tbmOutputAAlarm.ButtonText = "";
            this._tbmOutputAAlarm.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputAAlarm.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputAAlarm.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputAAlarm.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputAAlarm.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputAAlarm.ImageTextBox.ContextMenuStrip = this._tbmOutputAAlarm.ButtonPopupMenu;
            this._tbmOutputAAlarm.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputAAlarm.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputAAlarm.ImageTextBox.Image")));
            this._tbmOutputAAlarm.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputAAlarm.ImageTextBox.Name = "_textBox";
            this._tbmOutputAAlarm.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputAAlarm.ImageTextBox.ReadOnly = true;
            this._tbmOutputAAlarm.ImageTextBox.Size = new System.Drawing.Size(525, 20);
            this._tbmOutputAAlarm.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmOutputAAlarm.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputAAlarm.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputAAlarm.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputAAlarm.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputAAlarm.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputAAlarm.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputAAlarm.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputAAlarm.ImageTextBox.TextBox.Size = new System.Drawing.Size(523, 13);
            this._tbmOutputAAlarm.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputAAlarm.ImageTextBox.UseImage = true;
            this._tbmOutputAAlarm.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputAAlarm.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputAAlarm_TextBox_DoubleClick);
            this._tbmOutputAAlarm.Location = new System.Drawing.Point(134, 19);
            this._tbmOutputAAlarm.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputAAlarm.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputAAlarm.Name = "_tbmOutputAAlarm";
            this._tbmOutputAAlarm.Size = new System.Drawing.Size(545, 22);
            this._tbmOutputAAlarm.TabIndex = 2;
            this._tbmOutputAAlarm.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputAAlarm.TextImage")));
            this._tbmOutputAAlarm.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this.ItemClicked);
            this._tbmOutputAAlarm.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputAAlarm_DragDrop);
            this._tbmOutputAAlarm.DragOver += new System.Windows.Forms.DragEventHandler(this.AlarmOutputsDragOver);
            // 
            // _tsiModify9
            // 
            this._tsiModify9.Name = "_tsiModify9";
            this._tsiModify9.Size = new System.Drawing.Size(180, 22);
            this._tsiModify9.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove9
            // 
            this._tsiRemove9.Name = "_tsiRemove9";
            this._tsiRemove9.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove9.Text = "toolStripMenuItem2";
            // 
            // _lOutput6
            // 
            this._lOutput6.AutoSize = true;
            this._lOutput6.Location = new System.Drawing.Point(6, 23);
            this._lOutput6.Name = "_lOutput6";
            this._lOutput6.Size = new System.Drawing.Size(39, 13);
            this._lOutput6.TabIndex = 1;
            this._lOutput6.Text = "Output";
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 22);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(1097, 580);
            this._tpUserFolders.TabIndex = 8;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(1014, 544);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 6;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _lbUserFolders
            // 
            this._lbUserFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lbUserFolders.BackColor = System.Drawing.SystemColors.Info;
            this._lbUserFolders.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._lbUserFolders.FormattingEnabled = true;
            this._lbUserFolders.ImageList = null;
            this._lbUserFolders.Location = new System.Drawing.Point(2, 2);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(1091, 537);
            this._lbUserFolders.TabIndex = 26;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(3);
            this._tpReferencedBy.Size = new System.Drawing.Size(1097, 580);
            this._tpReferencedBy.TabIndex = 7;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(3);
            this._tpDescription.Size = new System.Drawing.Size(1097, 580);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(3, 3);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(1091, 574);
            this._eDescription.TabIndex = 6;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(112, 22);
            this._tsiModify.Text = "Modify";
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(112, 22);
            this._tsiModify2.Text = "Modify";
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
            // _eAlarmState
            // 
            this._eAlarmState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eAlarmState.BackColor = System.Drawing.SystemColors.Window;
            this._eAlarmState.Location = new System.Drawing.Point(157, 94);
            this._eAlarmState.Name = "_eAlarmState";
            this._eAlarmState.ReadOnly = true;
            this._eAlarmState.Size = new System.Drawing.Size(836, 20);
            this._eAlarmState.TabIndex = 5;
            // 
            // _lAlarmState
            // 
            this._lAlarmState.AutoSize = true;
            this._lAlarmState.Location = new System.Drawing.Point(15, 97);
            this._lAlarmState.Name = "_lAlarmState";
            this._lAlarmState.Size = new System.Drawing.Size(59, 13);
            this._lAlarmState.TabIndex = 4;
            this._lAlarmState.Text = "Alarm state";
            // 
            // _eActivationState
            // 
            this._eActivationState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eActivationState.BackColor = System.Drawing.SystemColors.Window;
            this._eActivationState.Location = new System.Drawing.Point(157, 120);
            this._eActivationState.Name = "_eActivationState";
            this._eActivationState.ReadOnly = true;
            this._eActivationState.Size = new System.Drawing.Size(836, 20);
            this._eActivationState.TabIndex = 7;
            // 
            // _lActivationState
            // 
            this._lActivationState.AutoSize = true;
            this._lActivationState.Location = new System.Drawing.Point(15, 123);
            this._lActivationState.Name = "_lActivationState";
            this._lActivationState.Size = new System.Drawing.Size(80, 13);
            this._lActivationState.TabIndex = 6;
            this._lActivationState.Text = "Activation state";
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._cbPurpose);
            this._panelBack.Controls.Add(this._lPurpose);
            this._panelBack.Controls.Add(this._eId);
            this._panelBack.Controls.Add(this._lId);
            this._panelBack.Controls.Add(this._eSabotage);
            this._panelBack.Controls.Add(this._lSabotage);
            this._panelBack.Controls.Add(this._eRequestActivationState);
            this._panelBack.Controls.Add(this._lRequestActivationState);
            this._panelBack.Controls.Add(this._itbImplicitManager);
            this._panelBack.Controls.Add(this._bApply);
            this._panelBack.Controls.Add(this._lImplicitManager);
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._eActivationState);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._lActivationState);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._eAlarmState);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Controls.Add(this._lAlarmState);
            this._panelBack.Controls.Add(this._lShortName);
            this._panelBack.Controls.Add(this._tcAlarmArea);
            this._panelBack.Controls.Add(this._eShortName);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(1134, 867);
            this._panelBack.TabIndex = 0;
            // 
            // _cbPurpose
            // 
            this._cbPurpose.DisplayMember = "Name";
            this._cbPurpose.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbPurpose.FormattingEnabled = true;
            this._cbPurpose.Location = new System.Drawing.Point(352, 15);
            this._cbPurpose.Name = "_cbPurpose";
            this._cbPurpose.Size = new System.Drawing.Size(250, 21);
            this._cbPurpose.TabIndex = 21;
            this._cbPurpose.ValueMember = "Purpose";
            this._cbPurpose.SelectedIndexChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lPurpose
            // 
            this._lPurpose.AutoSize = true;
            this._lPurpose.Location = new System.Drawing.Point(266, 18);
            this._lPurpose.Name = "_lPurpose";
            this._lPurpose.Size = new System.Drawing.Size(46, 13);
            this._lPurpose.TabIndex = 20;
            this._lPurpose.Text = "Purpose";
            // 
            // _eId
            // 
            this._eId.Location = new System.Drawing.Point(157, 16);
            this._eId.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this._eId.MinimalValueLength = 2;
            this._eId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eId.Name = "_eId";
            this._eId.Prefix = null;
            this._eId.Size = new System.Drawing.Size(73, 20);
            this._eId.Sufix = null;
            this._eId.TabIndex = 19;
            this._eId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._eId.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lId
            // 
            this._lId.AutoSize = true;
            this._lId.Location = new System.Drawing.Point(15, 18);
            this._lId.Name = "_lId";
            this._lId.Size = new System.Drawing.Size(15, 13);
            this._lId.TabIndex = 18;
            this._lId.Text = "id";
            // 
            // _eSabotage
            // 
            this._eSabotage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eSabotage.BackColor = System.Drawing.SystemColors.Window;
            this._eSabotage.Location = new System.Drawing.Point(157, 172);
            this._eSabotage.Name = "_eSabotage";
            this._eSabotage.ReadOnly = true;
            this._eSabotage.Size = new System.Drawing.Size(836, 20);
            this._eSabotage.TabIndex = 17;
            // 
            // _lSabotage
            // 
            this._lSabotage.AutoSize = true;
            this._lSabotage.Location = new System.Drawing.Point(15, 175);
            this._lSabotage.Name = "_lSabotage";
            this._lSabotage.Size = new System.Drawing.Size(53, 13);
            this._lSabotage.TabIndex = 16;
            this._lSabotage.Text = "Sabotage";
            // 
            // _eRequestActivationState
            // 
            this._eRequestActivationState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eRequestActivationState.BackColor = System.Drawing.SystemColors.Window;
            this._eRequestActivationState.Location = new System.Drawing.Point(157, 146);
            this._eRequestActivationState.Name = "_eRequestActivationState";
            this._eRequestActivationState.ReadOnly = true;
            this._eRequestActivationState.Size = new System.Drawing.Size(836, 20);
            this._eRequestActivationState.TabIndex = 15;
            // 
            // _lRequestActivationState
            // 
            this._lRequestActivationState.AutoSize = true;
            this._lRequestActivationState.Location = new System.Drawing.Point(15, 149);
            this._lRequestActivationState.Name = "_lRequestActivationState";
            this._lRequestActivationState.Size = new System.Drawing.Size(122, 13);
            this._lRequestActivationState.TabIndex = 14;
            this._lRequestActivationState.Text = "Request activation state";
            // 
            // _itbImplicitManager
            // 
            this._itbImplicitManager.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._itbImplicitManager.BackColor = System.Drawing.SystemColors.Info;
            this._itbImplicitManager.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._itbImplicitManager.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbImplicitManager.Image = null;
            this._itbImplicitManager.Location = new System.Drawing.Point(157, 198);
            this._itbImplicitManager.Name = "_itbImplicitManager";
            this._itbImplicitManager.NoTextNoImage = true;
            this._itbImplicitManager.ReadOnly = true;
            this._itbImplicitManager.Size = new System.Drawing.Size(836, 20);
            this._itbImplicitManager.TabIndex = 8;
            this._itbImplicitManager.Tag = "Reference";
            // 
            // 
            // 
            this._itbImplicitManager.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._itbImplicitManager.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._itbImplicitManager.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._itbImplicitManager.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbImplicitManager.TextBox.Location = new System.Drawing.Point(1, 2);
            this._itbImplicitManager.TextBox.Name = "_tbTextBox";
            this._itbImplicitManager.TextBox.ReadOnly = true;
            this._itbImplicitManager.TextBox.Size = new System.Drawing.Size(834, 13);
            this._itbImplicitManager.TextBox.TabIndex = 2;
            this._itbImplicitManager.UseImage = true;
            this._itbImplicitManager.DoubleClick += new System.EventHandler(this._itbImplicitManager_DoubleClick);
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(884, 837);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 11;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _lImplicitManager
            // 
            this._lImplicitManager.AutoSize = true;
            this._lImplicitManager.Location = new System.Drawing.Point(15, 201);
            this._lImplicitManager.Name = "_lImplicitManager";
            this._lImplicitManager.Size = new System.Drawing.Size(83, 13);
            this._lImplicitManager.TabIndex = 8;
            this._lImplicitManager.Text = "Implicit manager";
            // 
            // NCASAlarmAreaEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1134, 867);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1150, 830);
            this.Name = "NCASAlarmAreaEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NCASAlarmAreaEditForm";
            this._tcAlarmArea.ResumeLayout(false);
            this._tpBasicSettings.ResumeLayout(false);
            this._panelBasicSettings.ResumeLayout(false);
            this._panelBasicSettings.PerformLayout();
            this._gbControl.ResumeLayout(false);
            this._gbControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbTimeBuyingMatrixStateInfo)).EndInit();
            this._gbEISSettings.ResumeLayout(false);
            this._gbEISSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudEISSetUnsetPulseLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudEISFilterTime)).EndInit();
            this._tblAreaInputsAndCardReaders.ResumeLayout(false);
            this._gbAreaInputs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvDataInputs.DataGrid)).EndInit();
            this._gbCardReaders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvDataCRs.DataGrid)).EndInit();
            this._tpBasicTiming.ResumeLayout(false);
            this._tpBasicTiming.PerformLayout();
            this._gbTimeBuying.ResumeLayout(false);
            this._gbTimeBuying.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningActiveTimeBuying)).EndInit();
            this._gbPrewarning.ResumeLayout(false);
            this._gbPrewarning.PerformLayout();
            this._gbPrealarm.ResumeLayout(false);
            this._gbPrealarm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningNoExitSensor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningNoEntrySensor)).EndInit();
            this._gbObjAutomaticAct.ResumeLayout(false);
            this._gbObjAutomaticAct.PerformLayout();
            this._gbObjForcedTimeBuying.ResumeLayout(false);
            this._gbObjForcedTimeBuying.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningUnsetAction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarningTimeBuyingInfo)).EndInit();
            this._tpAlarmSettings.ResumeLayout(false);
            this._tpAlarmSettings.PerformLayout();
            this._gbCardReaderEventlogEvents.ResumeLayout(false);
            this._gbCardReaderEventlogEvents.PerformLayout();
            this._gbABAlarmHandling.ResumeLayout(false);
            this._gbABAlarmHandling.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tbPercentageSensorsToAAlarm)).EndInit();
            this._tpSpecialOutputs.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this._gbOutputSetByObjectForAaFailed.ResumeLayout(false);
            this._gbOutputSetByObjectForAaFailed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eOutputSetAaNotCalmOnPeriod)).EndInit();
            this._gbOutputMotion.ResumeLayout(false);
            this._gbOutputMotion.PerformLayout();
            this._gbOutpuSabotage.ResumeLayout(false);
            this._gbOutpuSabotage.PerformLayout();
            this._gbOutputActivation.ResumeLayout(false);
            this._gbOutputActivation.PerformLayout();
            this._gbOutputNotAcknowledged.ResumeLayout(false);
            this._gbOutputNotAcknowledged.PerformLayout();
            this._gbOutputAlarmState.ResumeLayout(false);
            this._gbOutputAlarmState.PerformLayout();
            this._gbOutputPrewarning.ResumeLayout(false);
            this._gbOutputPrewarning.PerformLayout();
            this._gbOutputTmpUnsetEntry.ResumeLayout(false);
            this._gbOutputTmpUnsetEntry.PerformLayout();
            this._gbSirenOutput.ResumeLayout(false);
            this._gbSirenOutput.PerformLayout();
            this._gbOutputTmpUnsetExit.ResumeLayout(false);
            this._gbOutputTmpUnsetExit.PerformLayout();
            this._gbOutputAAlarm.ResumeLayout(false);
            this._gbOutputAAlarm.PerformLayout();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eId)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eShortName;
        private System.Windows.Forms.Label _lShortName;
        private System.Windows.Forms.TabControl _tcAlarmArea;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.TabPage _tpBasicSettings;
        private System.Windows.Forms.TabPage _tpBasicTiming;
        private System.Windows.Forms.TextBox _eAlarmState;
        private System.Windows.Forms.Label _lAlarmState;
        private System.Windows.Forms.GroupBox _gbObjAutomaticAct;
        private System.Windows.Forms.CheckBox _cbAutomaticDeactivate;
        private System.Windows.Forms.GroupBox _gbPrealarm;
        private System.Windows.Forms.CheckBox _cbPrealamOn;
        private System.Windows.Forms.DateTimePicker _ePrealarmWarningDuration;
        private System.Windows.Forms.Label _lTemporaryUnsetEntryDuration;
        private System.Windows.Forms.TabPage _tpAlarmSettings;
        private System.Windows.Forms.GroupBox _gbAreaInputs;
        private System.Windows.Forms.GroupBox _gbCardReaders;
        private System.Windows.Forms.TextBox _eActivationState;
        private System.Windows.Forms.Label _lActivationState;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.GroupBox _gbPrewarning;
        private System.Windows.Forms.CheckBox _cbPrewarningOn;
        private System.Windows.Forms.DateTimePicker _ePrewarningDuration;
        private System.Windows.Forms.Label _lPrewarningDuration;
        private System.Windows.Forms.GroupBox _gbTimeBuying;
        private System.Windows.Forms.CheckBox _cbMaxTimeBuyingDurationOn;
        private System.Windows.Forms.Label _lMaxTimeBuyingDuration;
        private System.Windows.Forms.Label _lImplicitManager;
        private System.Windows.Forms.Label _lTemporaryUnsetExitDuration;
        private System.Windows.Forms.DateTimePicker _eTemporaryUnsetDuration;
        private System.Windows.Forms.Label _lResultOfAction;
        private System.Windows.Forms.TextBox _eResultOfAction;
        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.TabPage _tpSpecialOutputs;
        private System.Windows.Forms.GroupBox _gbOutputActivation;
        private System.Windows.Forms.Label _lOutput1;
        private System.Windows.Forms.GroupBox _gbOutputAlarmState;
        private System.Windows.Forms.Label _lOutput2;
        private System.Windows.Forms.GroupBox _gbOutputTmpUnsetExit;
        private System.Windows.Forms.Label _lOutput5;
        private System.Windows.Forms.GroupBox _gbOutputTmpUnsetEntry;
        private System.Windows.Forms.Label _lOutput4;
        private System.Windows.Forms.GroupBox _gbOutputPrewarning;
        private System.Windows.Forms.Label _lOutput3;
        private System.Windows.Forms.GroupBox _gbABAlarmHandling;
        private System.Windows.Forms.CheckBox _chbABAlarmHandling;
        private System.Windows.Forms.GroupBox _gbOutputAAlarm;
        private System.Windows.Forms.Label _lOutput6;
        private System.Windows.Forms.Label _lPercentageSensorsToAAlarmActual;
        private System.Windows.Forms.TrackBar _tbPercentageSensorsToAAlarm;
        private System.Windows.Forms.Label _lPercentageSensorsToAAlarm;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private Contal.IwQuick.UI.TextBoxMenu _tbmObjAutomaticAct;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove1;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate1;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify3;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove3;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputActivation;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputAAlarm;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputTmpUnsetExit;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputTmpUnsetEntry;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputPrewarning;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputAlarmState;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify9;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove9;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify8;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove8;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify7;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove7;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify6;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove6;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify5;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove5;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify4;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove4;
        private System.Windows.Forms.CheckBox _chbAllowAAtoCRsReporting;
        private System.Windows.Forms.GroupBox _gbControl;
        private System.Windows.Forms.Button _bAlarmAreaSetUnset;
        private System.Windows.Forms.CheckBox _chbUnconditional;
        private System.Windows.Forms.CheckBox _cbOfAAInverted;
        private Contal.IwQuick.UI.ImageTextBox _itbImplicitManager;
        private System.Windows.Forms.TextBox _eRequestActivationState;
        private System.Windows.Forms.Label _lRequestActivationState;
        private System.Windows.Forms.Label _lICCU;
        private Contal.Cgp.Components.CgpDataGridView _cdgvDataInputs;
        private Contal.Cgp.Components.CgpDataGridView _cdgvDataCRs;
        private System.Windows.Forms.GroupBox _gbSirenOutput;
        private System.Windows.Forms.Label _lMaximumOnPperiod;
        private System.Windows.Forms.DateTimePicker _dtpSirenMaximumOnPeriod;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputSiren;
        private System.Windows.Forms.Label _lOutput7;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify10;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove10;
        private System.Windows.Forms.CheckBox _chbUseEIS;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify11;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove11;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify12;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove12;
        private System.Windows.Forms.TextBox _eSabotage;
        private System.Windows.Forms.Label _lSabotage;
        private System.Windows.Forms.GroupBox _gbOutpuSabotage;
        private System.Windows.Forms.Label _lOutput8;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputSabotage;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify13;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove13;
        private System.Windows.Forms.GroupBox _gbOutputNotAcknowledged;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputNotAcknowledged;
        private System.Windows.Forms.Label _lOutput9;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify14;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove14;
        private System.Windows.Forms.GroupBox _gbOutputMotion;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputMotion;
        private System.Windows.Forms.Label _lOutput10;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify15;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove15;
        private System.Windows.Forms.CheckBox _chbUnconditionalSet;
        private System.Windows.Forms.CheckBox _chbAcknowledgeEvent;
        private System.Windows.Forms.CheckBox _chbNormalEvent;
        private System.Windows.Forms.CheckBox _chbAlarmEvent;
        private System.Windows.Forms.CheckBox _chbUnsetEvent;
        private System.Windows.Forms.CheckBox _chbSetEvent;
        private System.Windows.Forms.Label _lAlarmAreaEventSettings;
        private System.Windows.Forms.CheckBox _chbEnableEventLogs;
        private System.Windows.Forms.CheckBox _chbPermanentlyBlockedEvent;
        private System.Windows.Forms.CheckBox _chbTemporarilyBlockedEvent;
        private System.Windows.Forms.CheckBox _chbUnblockedEvent;
        private System.Windows.Forms.CheckBox _chbAcknowledgeEventSensor;
        private System.Windows.Forms.CheckBox _chbNormalEventSensor;
        private System.Windows.Forms.CheckBox _chbAlarmEventSensor;
        private System.Windows.Forms.Label _lSensorEvents;
        private System.Windows.Forms.CheckBox _cbActivateTimeBuyingOn;
        private System.Windows.Forms.CheckBox _cbMaxTotalTimeBuyingOn;
        private System.Windows.Forms.DateTimePicker _eMaxTotalTimeBuying;
        private System.Windows.Forms.Label _lMaxTotalTimeBuying;
        private System.Windows.Forms.DateTimePicker _eMaxTimeBuyingDuration;
        private System.Windows.Forms.Label _lActivateTimeBuying;
        private System.Windows.Forms.CheckBox _cbNoPrewarning;
        private System.Windows.Forms.DateTimePicker _eBuyTime;
        private System.Windows.Forms.CheckBox _cbTimeBuyingOnlyInPrewarning;
        private System.Windows.Forms.Label _lTimeBuyingOnlyInPrewarning;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _bAlarmAreaBuyTime;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify16;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove16;
        private System.Windows.Forms.GroupBox _gbObjForcedTimeBuying;
        private System.Windows.Forms.Label _lICCU1;
        private System.Windows.Forms.CheckBox _cbOfFTBInverted;
        private Contal.IwQuick.UI.TextBoxMenu _tbmObjForcedTimeBuying;
        private System.Windows.Forms.CheckBox _cbProvideOnlyUnset;
        private System.Windows.Forms.GroupBox _gbCardReaderEventlogEvents;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel _tblAreaInputsAndCardReaders;
        private System.Windows.Forms.GroupBox _gbEISSettings;
        private System.Windows.Forms.CheckBox _chbEISInputActivationStateInverted;
        private System.Windows.Forms.Label _lEISSetUnsetPulseLength;
        private System.Windows.Forms.NumericUpDown _nudEISSetUnsetPulseLength;
        private System.Windows.Forms.Label _lEISFilterTime;
        private System.Windows.Forms.NumericUpDown _nudEISFilterTime;
        private System.Windows.Forms.Label _lEISSetUnsetOutput;
        private System.Windows.Forms.Label _lEISInpurtActivationState;
        private Contal.IwQuick.UI.TextBoxMenu _tbmEISInputActivatianState;
        private Contal.IwQuick.UI.TextBoxMenu _tbmEISSetUnsetOutput;
        private System.Windows.Forms.Panel _panelBasicSettings;
        private System.Windows.Forms.Label _lId;
        private Contal.IwQuick.UI.NumericUpDownWithCustomTextFormat _eId;
        private Contal.IwQuick.PlatformPC.UI.Accordion.Accordion _accordionAlarmAreaAlarms;
        private System.Windows.Forms.ComboBox _cbAutomaticActivationMode;
        private System.Windows.Forms.PictureBox _pbWarningNoExitSensor;
        private System.Windows.Forms.PictureBox _pbWarningNoEntrySensor;
        private System.Windows.Forms.ComboBox _cbPurpose;
        private System.Windows.Forms.Label _lPurpose;
        private System.Windows.Forms.PictureBox _pbWarningActiveTimeBuying;
        private System.Windows.Forms.GroupBox _gbOutputSetByObjectForAaFailed;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputSetByObjectForAaFailed;
        private System.Windows.Forms.Label _lOutput11;
        private System.Windows.Forms.NumericUpDown _eOutputSetAaNotCalmOnPeriod;
        private System.Windows.Forms.Label _lOutputSetAaNotCalmOnPeriod;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify17;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove17;
        private System.Windows.Forms.CheckBox _chbTimeBuyingAlwaysOn;
        private System.Windows.Forms.PictureBox _pbWarningTimeBuyingInfo;
        private System.Windows.Forms.PictureBox _pbWarningUnsetAction;
        private System.Windows.Forms.PictureBox _pbTimeBuyingMatrixStateInfo;
        private System.Windows.Forms.Label _lMmSsLegend1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}
