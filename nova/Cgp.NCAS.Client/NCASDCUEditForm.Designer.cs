namespace Contal.Cgp.NCAS.Client
{
    partial class NCASDCUEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASDCUEditForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._eName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._lDoorEnvironmentState = new System.Windows.Forms.Label();
            this._eDoorEnvironmentState = new System.Windows.Forms.TextBox();
            this._nudLogicalAddress = new System.Windows.Forms.NumericUpDown();
            this._eOnlineState = new System.Windows.Forms.TextBox();
            this._lActualState = new System.Windows.Forms.Label();
            this._ePhysicalAddress = new System.Windows.Forms.TextBox();
            this._lPhysicalAddress = new System.Windows.Forms.Label();
            this._eLogicalAddress = new System.Windows.Forms.TextBox();
            this._lLogicalAddress = new System.Windows.Forms.Label();
            this._lSuperiorCCU = new System.Windows.Forms.Label();
            this._tsiModify3 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove3 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove1 = new System.Windows.Forms.ToolStripMenuItem();
            this._panelBack = new System.Windows.Forms.Panel();
            this._itbDoorEnvironment = new Contal.IwQuick.UI.ImageTextBox();
            this._lDoorEnvironment = new System.Windows.Forms.Label();
            this._eDcuInputsSabotageState = new System.Windows.Forms.TextBox();
            this._lDcuInputsSabotageState = new System.Windows.Forms.Label();
            this._itbSuperiorCCU = new Contal.IwQuick.UI.ImageTextBox();
            this._bApply = new System.Windows.Forms.Button();
            this._tcDCU = new System.Windows.Forms.TabControl();
            this._tpSettings = new System.Windows.Forms.TabPage();
            this._ilbCardReaders = new Contal.IwQuick.UI.ImageListBox();
            this._eOutputCount = new System.Windows.Forms.NumericUpDown();
            this._lOutputCount = new System.Windows.Forms.Label();
            this._lInputCount = new System.Windows.Forms.Label();
            this._eInputCount = new System.Windows.Forms.NumericUpDown();
            this._bCRMarkAsDead = new System.Windows.Forms.Button();
            this._bPrecreateCR = new System.Windows.Forms.Button();
            this._lCardReaders = new System.Windows.Forms.Label();
            this._tpInpupOutputSettings = new System.Windows.Forms.TabPage();
            this._scInputsOutputs = new System.Windows.Forms.SplitContainer();
            this._gbInputDCU = new System.Windows.Forms.GroupBox();
            this._cdgvInput = new Contal.Cgp.Components.CgpDataGridView();
            this._gbOutputDCU = new System.Windows.Forms.GroupBox();
            this._cdgvOutput = new Contal.Cgp.Components.CgpDataGridView();
            this._tpControl = new System.Windows.Forms.TabPage();
            this._gbDcuMemoryLoad = new System.Windows.Forms.GroupBox();
            this._bDcuMemoryLoadRefresh = new System.Windows.Forms.Button();
            this._eDcuMemoryLoad = new System.Windows.Forms.TextBox();
            this._lDcuMemoryLoad = new System.Windows.Forms.Label();
            this._gbDcuUpgrade = new System.Windows.Forms.GroupBox();
            this._lFirmwareVersion = new System.Windows.Forms.Label();
            this._eFirmware = new System.Windows.Forms.TextBox();
            this._bRefresh = new System.Windows.Forms.Button();
            this._gbDevice = new System.Windows.Forms.GroupBox();
            this._bReset = new System.Windows.Forms.Button();
            this._tpCRUpgrades = new System.Windows.Forms.TabPage();
            this._bCRUpgrade = new System.Windows.Forms.Button();
            this._bUpgradeCRRefresh = new System.Windows.Forms.Button();
            this._chbSelectAll = new System.Windows.Forms.CheckBox();
            this._dgvCRUpgrading = new System.Windows.Forms.DataGridView();
            this._lAvailableCRUpradeVersions = new System.Windows.Forms.Label();
            this._lbAvailableCRUpgradeVersions = new System.Windows.Forms.ListBox();
            this._tpAlarmSettings = new System.Windows.Forms.TabPage();
            this._accordionAlarmSettings = new Contal.IwQuick.PlatformPC.UI.Accordion.Accordion();
            this._cbOpenAllAlarmSettings = new System.Windows.Forms.CheckBox();
            this._tpSpecialOutputs = new System.Windows.Forms.TabPage();
            this._gbOutputSabotageDCUInputs = new System.Windows.Forms.GroupBox();
            this._tbmOutputSabotageDCUInputs = new Contal.IwQuick.UI.TextBoxMenu();
            this._lOutput6 = new System.Windows.Forms.Label();
            this._gbOutputOfflineDCU = new System.Windows.Forms.GroupBox();
            this._tbmOutputOfflineDcu = new Contal.IwQuick.UI.TextBoxMenu();
            this._lOutput5 = new System.Windows.Forms.Label();
            this._gbOutputSabotageDCU = new System.Windows.Forms.GroupBox();
            this._tbmOutputSabotageDcu = new Contal.IwQuick.UI.TextBoxMenu();
            this._lOutput4 = new System.Windows.Forms.Label();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh1 = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this._nudLogicalAddress)).BeginInit();
            this._panelBack.SuspendLayout();
            this._tcDCU.SuspendLayout();
            this._tpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eOutputCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eInputCount)).BeginInit();
            this._tpInpupOutputSettings.SuspendLayout();
            this._scInputsOutputs.Panel1.SuspendLayout();
            this._scInputsOutputs.Panel2.SuspendLayout();
            this._scInputsOutputs.SuspendLayout();
            this._gbInputDCU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvInput.DataGrid)).BeginInit();
            this._gbOutputDCU.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvOutput.DataGrid)).BeginInit();
            this._tpControl.SuspendLayout();
            this._gbDcuMemoryLoad.SuspendLayout();
            this._gbDcuUpgrade.SuspendLayout();
            this._gbDevice.SuspendLayout();
            this._tpCRUpgrades.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvCRUpgrading)).BeginInit();
            this._tpAlarmSettings.SuspendLayout();
            this._tpSpecialOutputs.SuspendLayout();
            this._gbOutputSabotageDCUInputs.SuspendLayout();
            this._gbOutputOfflineDCU.SuspendLayout();
            this._gbOutputSabotageDCU.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(797, 631);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(716, 631);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 6;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(169, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(458, 20);
            this._eName.TabIndex = 1;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            this._eName.KeyDown += new System.Windows.Forms.KeyEventHandler(this._eName_KeyDown);
            this._eName.KeyUp += new System.Windows.Forms.KeyEventHandler(this._eName_KeyUp);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 15);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _lDoorEnvironmentState
            // 
            this._lDoorEnvironmentState.AutoSize = true;
            this._lDoorEnvironmentState.Location = new System.Drawing.Point(12, 41);
            this._lDoorEnvironmentState.Name = "_lDoorEnvironmentState";
            this._lDoorEnvironmentState.Size = new System.Drawing.Size(117, 13);
            this._lDoorEnvironmentState.TabIndex = 2;
            this._lDoorEnvironmentState.Text = "Door environment state";
            // 
            // _eDoorEnvironmentState
            // 
            this._eDoorEnvironmentState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDoorEnvironmentState.BackColor = System.Drawing.SystemColors.Window;
            this._eDoorEnvironmentState.Location = new System.Drawing.Point(169, 38);
            this._eDoorEnvironmentState.Name = "_eDoorEnvironmentState";
            this._eDoorEnvironmentState.ReadOnly = true;
            this._eDoorEnvironmentState.Size = new System.Drawing.Size(458, 20);
            this._eDoorEnvironmentState.TabIndex = 2;
            // 
            // _nudLogicalAddress
            // 
            this._nudLogicalAddress.Location = new System.Drawing.Point(412, 116);
            this._nudLogicalAddress.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this._nudLogicalAddress.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudLogicalAddress.Name = "_nudLogicalAddress";
            this._nudLogicalAddress.Size = new System.Drawing.Size(120, 20);
            this._nudLogicalAddress.TabIndex = 6;
            this._nudLogicalAddress.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudLogicalAddress.Visible = false;
            this._nudLogicalAddress.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _eOnlineState
            // 
            this._eOnlineState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eOnlineState.Location = new System.Drawing.Point(169, 90);
            this._eOnlineState.Name = "_eOnlineState";
            this._eOnlineState.ReadOnly = true;
            this._eOnlineState.Size = new System.Drawing.Size(458, 20);
            this._eOnlineState.TabIndex = 4;
            // 
            // _lActualState
            // 
            this._lActualState.AutoSize = true;
            this._lActualState.Location = new System.Drawing.Point(12, 93);
            this._lActualState.Name = "_lActualState";
            this._lActualState.Size = new System.Drawing.Size(63, 13);
            this._lActualState.TabIndex = 2;
            this._lActualState.Text = "Actual state";
            // 
            // _ePhysicalAddress
            // 
            this._ePhysicalAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ePhysicalAddress.Location = new System.Drawing.Point(169, 142);
            this._ePhysicalAddress.Name = "_ePhysicalAddress";
            this._ePhysicalAddress.ReadOnly = true;
            this._ePhysicalAddress.Size = new System.Drawing.Size(458, 20);
            this._ePhysicalAddress.TabIndex = 7;
            // 
            // _lPhysicalAddress
            // 
            this._lPhysicalAddress.AutoSize = true;
            this._lPhysicalAddress.Location = new System.Drawing.Point(12, 145);
            this._lPhysicalAddress.Name = "_lPhysicalAddress";
            this._lPhysicalAddress.Size = new System.Drawing.Size(86, 13);
            this._lPhysicalAddress.TabIndex = 7;
            this._lPhysicalAddress.Text = "Physical address";
            // 
            // _eLogicalAddress
            // 
            this._eLogicalAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eLogicalAddress.Location = new System.Drawing.Point(169, 116);
            this._eLogicalAddress.Name = "_eLogicalAddress";
            this._eLogicalAddress.ReadOnly = true;
            this._eLogicalAddress.Size = new System.Drawing.Size(458, 20);
            this._eLogicalAddress.TabIndex = 5;
            // 
            // _lLogicalAddress
            // 
            this._lLogicalAddress.AutoSize = true;
            this._lLogicalAddress.Location = new System.Drawing.Point(12, 119);
            this._lLogicalAddress.Name = "_lLogicalAddress";
            this._lLogicalAddress.Size = new System.Drawing.Size(81, 13);
            this._lLogicalAddress.TabIndex = 4;
            this._lLogicalAddress.Text = "Logical address";
            // 
            // _lSuperiorCCU
            // 
            this._lSuperiorCCU.AutoSize = true;
            this._lSuperiorCCU.Location = new System.Drawing.Point(12, 67);
            this._lSuperiorCCU.Name = "_lSuperiorCCU";
            this._lSuperiorCCU.Size = new System.Drawing.Size(71, 13);
            this._lSuperiorCCU.TabIndex = 0;
            this._lSuperiorCCU.Text = "Superior CCU";
            // 
            // _tsiModify3
            // 
            this._tsiModify3.Name = "_tsiModify3";
            this._tsiModify3.Size = new System.Drawing.Size(180, 22);
            this._tsiModify3.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove3
            // 
            this._tsiRemove3.Name = "_tsiRemove3";
            this._tsiRemove3.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove3.Text = "toolStripMenuItem1";
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(180, 22);
            this._tsiModify2.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove2
            // 
            this._tsiRemove2.Name = "_tsiRemove2";
            this._tsiRemove2.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove2.Text = "toolStripMenuItem2";
            // 
            // _tsiModify1
            // 
            this._tsiModify1.Name = "_tsiModify1";
            this._tsiModify1.Size = new System.Drawing.Size(180, 22);
            this._tsiModify1.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove1
            // 
            this._tsiRemove1.Name = "_tsiRemove1";
            this._tsiRemove1.Size = new System.Drawing.Size(180, 22);
            this._tsiRemove1.Text = "toolStripMenuItem2";
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._itbDoorEnvironment);
            this._panelBack.Controls.Add(this._lDoorEnvironment);
            this._panelBack.Controls.Add(this._eDcuInputsSabotageState);
            this._panelBack.Controls.Add(this._lDcuInputsSabotageState);
            this._panelBack.Controls.Add(this._itbSuperiorCCU);
            this._panelBack.Controls.Add(this._bApply);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._lDoorEnvironmentState);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._eDoorEnvironmentState);
            this._panelBack.Controls.Add(this._nudLogicalAddress);
            this._panelBack.Controls.Add(this._tcDCU);
            this._panelBack.Controls.Add(this._eOnlineState);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Controls.Add(this._lActualState);
            this._panelBack.Controls.Add(this._lSuperiorCCU);
            this._panelBack.Controls.Add(this._ePhysicalAddress);
            this._panelBack.Controls.Add(this._lPhysicalAddress);
            this._panelBack.Controls.Add(this._lLogicalAddress);
            this._panelBack.Controls.Add(this._eLogicalAddress);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.MinimumSize = new System.Drawing.Size(661, 463);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(884, 661);
            this._panelBack.TabIndex = 0;
            // 
            // _itbDoorEnvironment
            // 
            this._itbDoorEnvironment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbDoorEnvironment.BackColor = System.Drawing.SystemColors.Info;
            this._itbDoorEnvironment.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._itbDoorEnvironment.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbDoorEnvironment.Image = ((System.Drawing.Image)(resources.GetObject("_itbDoorEnvironment.Image")));
            this._itbDoorEnvironment.Location = new System.Drawing.Point(169, 194);
            this._itbDoorEnvironment.Name = "_itbDoorEnvironment";
            this._itbDoorEnvironment.NoTextNoImage = true;
            this._itbDoorEnvironment.ReadOnly = true;
            this._itbDoorEnvironment.Size = new System.Drawing.Size(458, 20);
            this._itbDoorEnvironment.TabIndex = 12;
            this._itbDoorEnvironment.Tag = "Reference";
            // 
            // 
            // 
            this._itbDoorEnvironment.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbDoorEnvironment.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._itbDoorEnvironment.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._itbDoorEnvironment.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbDoorEnvironment.TextBox.Location = new System.Drawing.Point(1, 2);
            this._itbDoorEnvironment.TextBox.Name = "_tbTextBox";
            this._itbDoorEnvironment.TextBox.ReadOnly = true;
            this._itbDoorEnvironment.TextBox.Size = new System.Drawing.Size(456, 13);
            this._itbDoorEnvironment.TextBox.TabIndex = 2;
            this._itbDoorEnvironment.UseImage = true;
            this._itbDoorEnvironment.DoubleClick += new System.EventHandler(this._itbDoorEnvironment_DoubleClick);
            // 
            // _lDoorEnvironment
            // 
            this._lDoorEnvironment.AutoSize = true;
            this._lDoorEnvironment.Location = new System.Drawing.Point(12, 197);
            this._lDoorEnvironment.Name = "_lDoorEnvironment";
            this._lDoorEnvironment.Size = new System.Drawing.Size(91, 13);
            this._lDoorEnvironment.TabIndex = 11;
            this._lDoorEnvironment.Text = "Door environment";
            // 
            // _eDcuInputsSabotageState
            // 
            this._eDcuInputsSabotageState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDcuInputsSabotageState.Location = new System.Drawing.Point(169, 168);
            this._eDcuInputsSabotageState.Name = "_eDcuInputsSabotageState";
            this._eDcuInputsSabotageState.ReadOnly = true;
            this._eDcuInputsSabotageState.Size = new System.Drawing.Size(458, 20);
            this._eDcuInputsSabotageState.TabIndex = 9;
            // 
            // _lDcuInputsSabotageState
            // 
            this._lDcuInputsSabotageState.AutoSize = true;
            this._lDcuInputsSabotageState.Location = new System.Drawing.Point(12, 171);
            this._lDcuInputsSabotageState.Name = "_lDcuInputsSabotageState";
            this._lDcuInputsSabotageState.Size = new System.Drawing.Size(122, 13);
            this._lDcuInputsSabotageState.TabIndex = 8;
            this._lDcuInputsSabotageState.Text = "Sabotage of DCU inputs";
            // 
            // _itbSuperiorCCU
            // 
            this._itbSuperiorCCU.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbSuperiorCCU.BackColor = System.Drawing.SystemColors.Info;
            this._itbSuperiorCCU.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._itbSuperiorCCU.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbSuperiorCCU.Image = ((System.Drawing.Image)(resources.GetObject("_itbSuperiorCCU.Image")));
            this._itbSuperiorCCU.Location = new System.Drawing.Point(169, 64);
            this._itbSuperiorCCU.Name = "_itbSuperiorCCU";
            this._itbSuperiorCCU.NoTextNoImage = true;
            this._itbSuperiorCCU.ReadOnly = true;
            this._itbSuperiorCCU.Size = new System.Drawing.Size(458, 20);
            this._itbSuperiorCCU.TabIndex = 3;
            this._itbSuperiorCCU.Tag = "Reference";
            // 
            // 
            // 
            this._itbSuperiorCCU.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbSuperiorCCU.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._itbSuperiorCCU.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._itbSuperiorCCU.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._itbSuperiorCCU.TextBox.Location = new System.Drawing.Point(1, 2);
            this._itbSuperiorCCU.TextBox.Name = "_tbTextBox";
            this._itbSuperiorCCU.TextBox.ReadOnly = true;
            this._itbSuperiorCCU.TextBox.Size = new System.Drawing.Size(456, 13);
            this._itbSuperiorCCU.TextBox.TabIndex = 2;
            this._itbSuperiorCCU.UseImage = true;
            this._itbSuperiorCCU.DoubleClick += new System.EventHandler(this._itbSuperiorCCU_DoubleClick);
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(635, 631);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 5;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _tcDCU
            // 
            this._tcDCU.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcDCU.Controls.Add(this._tpSettings);
            this._tcDCU.Controls.Add(this._tpInpupOutputSettings);
            this._tcDCU.Controls.Add(this._tpControl);
            this._tcDCU.Controls.Add(this._tpCRUpgrades);
            this._tcDCU.Controls.Add(this._tpAlarmSettings);
            this._tcDCU.Controls.Add(this._tpSpecialOutputs);
            this._tcDCU.Controls.Add(this._tpUserFolders);
            this._tcDCU.Controls.Add(this._tpReferencedBy);
            this._tcDCU.Controls.Add(this._tpDescription);
            this._tcDCU.Location = new System.Drawing.Point(15, 220);
            this._tcDCU.Multiline = true;
            this._tcDCU.Name = "_tcDCU";
            this._tcDCU.SelectedIndex = 0;
            this._tcDCU.Size = new System.Drawing.Size(859, 405);
            this._tcDCU.TabIndex = 4;
            this._tcDCU.TabStop = false;
            // 
            // _tpSettings
            // 
            this._tpSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpSettings.Controls.Add(this._ilbCardReaders);
            this._tpSettings.Controls.Add(this._eOutputCount);
            this._tpSettings.Controls.Add(this._lOutputCount);
            this._tpSettings.Controls.Add(this._lInputCount);
            this._tpSettings.Controls.Add(this._eInputCount);
            this._tpSettings.Controls.Add(this._bCRMarkAsDead);
            this._tpSettings.Controls.Add(this._bPrecreateCR);
            this._tpSettings.Controls.Add(this._lCardReaders);
            this._tpSettings.Location = new System.Drawing.Point(4, 22);
            this._tpSettings.Name = "_tpSettings";
            this._tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpSettings.Size = new System.Drawing.Size(851, 379);
            this._tpSettings.TabIndex = 0;
            this._tpSettings.Text = "Settings";
            this._tpSettings.Enter += new System.EventHandler(this._tpSettings_Enter);
            // 
            // _ilbCardReaders
            // 
            this._ilbCardReaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ilbCardReaders.BackColor = System.Drawing.SystemColors.Info;
            this._ilbCardReaders.Cursor = System.Windows.Forms.Cursors.Hand;
            this._ilbCardReaders.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._ilbCardReaders.FormattingEnabled = true;
            this._ilbCardReaders.ImageList = null;
            this._ilbCardReaders.ItemHeight = 16;
            this._ilbCardReaders.Location = new System.Drawing.Point(6, 22);
            this._ilbCardReaders.Name = "_ilbCardReaders";
            this._ilbCardReaders.SelectedItemObject = null;
            this._ilbCardReaders.Size = new System.Drawing.Size(839, 148);
            this._ilbCardReaders.TabIndex = 17;
            this._ilbCardReaders.Tag = "Reference";
            this._ilbCardReaders.SelectedIndexChanged += new System.EventHandler(this._ilbCardReaders_SelectedIndexChanged);
            this._ilbCardReaders.DoubleClick += new System.EventHandler(this._ilbCardReaders_DoubleClick);
            // 
            // _eOutputCount
            // 
            this._eOutputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._eOutputCount.Location = new System.Drawing.Point(152, 243);
            this._eOutputCount.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._eOutputCount.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this._eOutputCount.Name = "_eOutputCount";
            this._eOutputCount.Size = new System.Drawing.Size(243, 20);
            this._eOutputCount.TabIndex = 16;
            this._eOutputCount.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this._eOutputCount.Visible = false;
            // 
            // _lOutputCount
            // 
            this._lOutputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lOutputCount.AutoSize = true;
            this._lOutputCount.Location = new System.Drawing.Point(8, 245);
            this._lOutputCount.Name = "_lOutputCount";
            this._lOutputCount.Size = new System.Drawing.Size(69, 13);
            this._lOutputCount.TabIndex = 15;
            this._lOutputCount.Text = "Output count";
            this._lOutputCount.Visible = false;
            // 
            // _lInputCount
            // 
            this._lInputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lInputCount.AutoSize = true;
            this._lInputCount.Location = new System.Drawing.Point(8, 219);
            this._lInputCount.Name = "_lInputCount";
            this._lInputCount.Size = new System.Drawing.Size(61, 13);
            this._lInputCount.TabIndex = 14;
            this._lInputCount.Text = "Input count";
            this._lInputCount.Visible = false;
            // 
            // _eInputCount
            // 
            this._eInputCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._eInputCount.Location = new System.Drawing.Point(152, 217);
            this._eInputCount.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._eInputCount.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this._eInputCount.Name = "_eInputCount";
            this._eInputCount.Size = new System.Drawing.Size(243, 20);
            this._eInputCount.TabIndex = 13;
            this._eInputCount.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this._eInputCount.Visible = false;
            // 
            // _bCRMarkAsDead
            // 
            this._bCRMarkAsDead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bCRMarkAsDead.AutoSize = true;
            this._bCRMarkAsDead.Enabled = false;
            this._bCRMarkAsDead.Location = new System.Drawing.Point(6, 183);
            this._bCRMarkAsDead.Name = "_bCRMarkAsDead";
            this._bCRMarkAsDead.Size = new System.Drawing.Size(110, 23);
            this._bCRMarkAsDead.TabIndex = 11;
            this._bCRMarkAsDead.Text = "Mark as dead";
            this._bCRMarkAsDead.UseVisualStyleBackColor = true;
            this._bCRMarkAsDead.Click += new System.EventHandler(this._bCRMarkAsDead_Click);
            // 
            // _bPrecreateCR
            // 
            this._bPrecreateCR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bPrecreateCR.Location = new System.Drawing.Point(150, 183);
            this._bPrecreateCR.Name = "_bPrecreateCR";
            this._bPrecreateCR.Size = new System.Drawing.Size(110, 23);
            this._bPrecreateCR.TabIndex = 12;
            this._bPrecreateCR.Text = "Precreate CR";
            this._bPrecreateCR.UseVisualStyleBackColor = true;
            this._bPrecreateCR.Click += new System.EventHandler(this._bPrecreateCR_Click);
            // 
            // _lCardReaders
            // 
            this._lCardReaders.AutoSize = true;
            this._lCardReaders.Location = new System.Drawing.Point(8, 6);
            this._lCardReaders.Name = "_lCardReaders";
            this._lCardReaders.Size = new System.Drawing.Size(72, 13);
            this._lCardReaders.TabIndex = 9;
            this._lCardReaders.Text = "Card Readers";
            // 
            // _tpInpupOutputSettings
            // 
            this._tpInpupOutputSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpInpupOutputSettings.Controls.Add(this._scInputsOutputs);
            this._tpInpupOutputSettings.Location = new System.Drawing.Point(4, 22);
            this._tpInpupOutputSettings.Name = "_tpInpupOutputSettings";
            this._tpInpupOutputSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpInpupOutputSettings.Size = new System.Drawing.Size(851, 379);
            this._tpInpupOutputSettings.TabIndex = 6;
            this._tpInpupOutputSettings.Text = "Input / Output settings";
            this._tpInpupOutputSettings.Enter += new System.EventHandler(this._tpInpupOutputSettings_Enter);
            // 
            // _scInputsOutputs
            // 
            this._scInputsOutputs.BackColor = System.Drawing.SystemColors.Control;
            this._scInputsOutputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._scInputsOutputs.Location = new System.Drawing.Point(3, 3);
            this._scInputsOutputs.Name = "_scInputsOutputs";
            this._scInputsOutputs.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _scInputsOutputs.Panel1
            // 
            this._scInputsOutputs.Panel1.Controls.Add(this._gbInputDCU);
            this._scInputsOutputs.Panel1MinSize = 150;
            // 
            // _scInputsOutputs.Panel2
            // 
            this._scInputsOutputs.Panel2.Controls.Add(this._gbOutputDCU);
            this._scInputsOutputs.Panel2MinSize = 150;
            this._scInputsOutputs.Size = new System.Drawing.Size(845, 373);
            this._scInputsOutputs.SplitterDistance = 188;
            this._scInputsOutputs.TabIndex = 2;
            // 
            // _gbInputDCU
            // 
            this._gbInputDCU.BackColor = System.Drawing.SystemColors.Control;
            this._gbInputDCU.Controls.Add(this._cdgvInput);
            this._gbInputDCU.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gbInputDCU.Location = new System.Drawing.Point(0, 0);
            this._gbInputDCU.Name = "_gbInputDCU";
            this._gbInputDCU.Size = new System.Drawing.Size(845, 188);
            this._gbInputDCU.TabIndex = 0;
            this._gbInputDCU.TabStop = false;
            this._gbInputDCU.Text = "Input DCU";
            // 
            // _cdgvInput
            // 
            this._cdgvInput.AllwaysRefreshOrder = false;
            this._cdgvInput.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvInput.DataGrid.AllowUserToAddRows = false;
            this._cdgvInput.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvInput.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._cdgvInput.DataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._cdgvInput.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._cdgvInput.DataGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvInput.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvInput.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvInput.DataGrid.MultiSelect = false;
            this._cdgvInput.DataGrid.Name = "_dgvData";
            this._cdgvInput.DataGrid.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._cdgvInput.DataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this._cdgvInput.DataGrid.RowHeadersVisible = false;
            this._cdgvInput.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._cdgvInput.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvInput.DataGrid.Size = new System.Drawing.Size(839, 169);
            this._cdgvInput.DataGrid.TabIndex = 0;
            this._cdgvInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvInput.LocalizationHelper = null;
            this._cdgvInput.Location = new System.Drawing.Point(3, 16);
            this._cdgvInput.Name = "_cdgvInput";
            this._cdgvInput.Size = new System.Drawing.Size(839, 169);
            this._cdgvInput.TabIndex = 0;
            // 
            // _gbOutputDCU
            // 
            this._gbOutputDCU.Controls.Add(this._cdgvOutput);
            this._gbOutputDCU.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gbOutputDCU.Location = new System.Drawing.Point(0, 0);
            this._gbOutputDCU.Name = "_gbOutputDCU";
            this._gbOutputDCU.Size = new System.Drawing.Size(845, 181);
            this._gbOutputDCU.TabIndex = 1;
            this._gbOutputDCU.TabStop = false;
            this._gbOutputDCU.Text = "Output DCU";
            // 
            // _cdgvOutput
            // 
            this._cdgvOutput.AllwaysRefreshOrder = false;
            this._cdgvOutput.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvOutput.DataGrid.AllowUserToAddRows = false;
            this._cdgvOutput.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvOutput.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._cdgvOutput.DataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this._cdgvOutput.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._cdgvOutput.DataGrid.DefaultCellStyle = dataGridViewCellStyle5;
            this._cdgvOutput.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvOutput.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvOutput.DataGrid.MultiSelect = false;
            this._cdgvOutput.DataGrid.Name = "_dgvData";
            this._cdgvOutput.DataGrid.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._cdgvOutput.DataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this._cdgvOutput.DataGrid.RowHeadersVisible = false;
            this._cdgvOutput.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._cdgvOutput.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvOutput.DataGrid.Size = new System.Drawing.Size(839, 162);
            this._cdgvOutput.DataGrid.TabIndex = 0;
            this._cdgvOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvOutput.LocalizationHelper = null;
            this._cdgvOutput.Location = new System.Drawing.Point(3, 16);
            this._cdgvOutput.Name = "_cdgvOutput";
            this._cdgvOutput.Size = new System.Drawing.Size(839, 162);
            this._cdgvOutput.TabIndex = 0;
            // 
            // _tpControl
            // 
            this._tpControl.BackColor = System.Drawing.SystemColors.Control;
            this._tpControl.Controls.Add(this._gbDcuMemoryLoad);
            this._tpControl.Controls.Add(this._gbDcuUpgrade);
            this._tpControl.Controls.Add(this._gbDevice);
            this._tpControl.Location = new System.Drawing.Point(4, 22);
            this._tpControl.Name = "_tpControl";
            this._tpControl.Padding = new System.Windows.Forms.Padding(3);
            this._tpControl.Size = new System.Drawing.Size(851, 379);
            this._tpControl.TabIndex = 4;
            this._tpControl.Text = "Control";
            this._tpControl.Enter += new System.EventHandler(this._tpControl_Enter);
            // 
            // _gbDcuMemoryLoad
            // 
            this._gbDcuMemoryLoad.Controls.Add(this._bDcuMemoryLoadRefresh);
            this._gbDcuMemoryLoad.Controls.Add(this._eDcuMemoryLoad);
            this._gbDcuMemoryLoad.Controls.Add(this._lDcuMemoryLoad);
            this._gbDcuMemoryLoad.Location = new System.Drawing.Point(200, 63);
            this._gbDcuMemoryLoad.Name = "_gbDcuMemoryLoad";
            this._gbDcuMemoryLoad.Size = new System.Drawing.Size(296, 54);
            this._gbDcuMemoryLoad.TabIndex = 3;
            this._gbDcuMemoryLoad.TabStop = false;
            this._gbDcuMemoryLoad.Text = "Dcu memory load condition";
            // 
            // _bDcuMemoryLoadRefresh
            // 
            this._bDcuMemoryLoadRefresh.Location = new System.Drawing.Point(215, 16);
            this._bDcuMemoryLoadRefresh.Name = "_bDcuMemoryLoadRefresh";
            this._bDcuMemoryLoadRefresh.Size = new System.Drawing.Size(75, 23);
            this._bDcuMemoryLoadRefresh.TabIndex = 2;
            this._bDcuMemoryLoadRefresh.Text = "Refresh";
            this._bDcuMemoryLoadRefresh.UseVisualStyleBackColor = true;
            this._bDcuMemoryLoadRefresh.Click += new System.EventHandler(this._bDcuMemoryLoadRefresh_Click);
            // 
            // _eDcuMemoryLoad
            // 
            this._eDcuMemoryLoad.Location = new System.Drawing.Point(105, 19);
            this._eDcuMemoryLoad.Name = "_eDcuMemoryLoad";
            this._eDcuMemoryLoad.ReadOnly = true;
            this._eDcuMemoryLoad.Size = new System.Drawing.Size(88, 20);
            this._eDcuMemoryLoad.TabIndex = 1;
            // 
            // _lDcuMemoryLoad
            // 
            this._lDcuMemoryLoad.AutoSize = true;
            this._lDcuMemoryLoad.Location = new System.Drawing.Point(6, 22);
            this._lDcuMemoryLoad.Name = "_lDcuMemoryLoad";
            this._lDcuMemoryLoad.Size = new System.Drawing.Size(71, 13);
            this._lDcuMemoryLoad.TabIndex = 0;
            this._lDcuMemoryLoad.Text = "Used memory";
            // 
            // _gbDcuUpgrade
            // 
            this._gbDcuUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbDcuUpgrade.Controls.Add(this._lFirmwareVersion);
            this._gbDcuUpgrade.Controls.Add(this._eFirmware);
            this._gbDcuUpgrade.Controls.Add(this._bRefresh);
            this._gbDcuUpgrade.Location = new System.Drawing.Point(6, 6);
            this._gbDcuUpgrade.Name = "_gbDcuUpgrade";
            this._gbDcuUpgrade.Size = new System.Drawing.Size(705, 51);
            this._gbDcuUpgrade.TabIndex = 0;
            this._gbDcuUpgrade.TabStop = false;
            this._gbDcuUpgrade.Text = "DCU firmware";
            // 
            // _lFirmwareVersion
            // 
            this._lFirmwareVersion.AutoSize = true;
            this._lFirmwareVersion.Location = new System.Drawing.Point(6, 25);
            this._lFirmwareVersion.Name = "_lFirmwareVersion";
            this._lFirmwareVersion.Size = new System.Drawing.Size(86, 13);
            this._lFirmwareVersion.TabIndex = 11;
            this._lFirmwareVersion.Text = "Firmware version";
            // 
            // _eFirmware
            // 
            this._eFirmware.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eFirmware.Location = new System.Drawing.Point(109, 22);
            this._eFirmware.Name = "_eFirmware";
            this._eFirmware.ReadOnly = true;
            this._eFirmware.Size = new System.Drawing.Size(480, 20);
            this._eFirmware.TabIndex = 1;
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(595, 20);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 0;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _gbDevice
            // 
            this._gbDevice.Controls.Add(this._bReset);
            this._gbDevice.Location = new System.Drawing.Point(6, 63);
            this._gbDevice.Name = "_gbDevice";
            this._gbDevice.Size = new System.Drawing.Size(173, 56);
            this._gbDevice.TabIndex = 2;
            this._gbDevice.TabStop = false;
            this._gbDevice.Text = "Device";
            // 
            // _bReset
            // 
            this._bReset.Location = new System.Drawing.Point(92, 19);
            this._bReset.Name = "_bReset";
            this._bReset.Size = new System.Drawing.Size(75, 23);
            this._bReset.TabIndex = 0;
            this._bReset.Text = "Reset";
            this._bReset.UseVisualStyleBackColor = true;
            this._bReset.Click += new System.EventHandler(this._bReset_Click);
            // 
            // _tpCRUpgrades
            // 
            this._tpCRUpgrades.BackColor = System.Drawing.SystemColors.Control;
            this._tpCRUpgrades.Controls.Add(this._bCRUpgrade);
            this._tpCRUpgrades.Controls.Add(this._bUpgradeCRRefresh);
            this._tpCRUpgrades.Controls.Add(this._chbSelectAll);
            this._tpCRUpgrades.Controls.Add(this._dgvCRUpgrading);
            this._tpCRUpgrades.Controls.Add(this._lAvailableCRUpradeVersions);
            this._tpCRUpgrades.Controls.Add(this._lbAvailableCRUpgradeVersions);
            this._tpCRUpgrades.Location = new System.Drawing.Point(4, 22);
            this._tpCRUpgrades.Name = "_tpCRUpgrades";
            this._tpCRUpgrades.Size = new System.Drawing.Size(851, 379);
            this._tpCRUpgrades.TabIndex = 11;
            this._tpCRUpgrades.Text = "Card readers upgrade";
            this._tpCRUpgrades.Enter += new System.EventHandler(this._tpCRUpgrades_Enter);
            // 
            // _bCRUpgrade
            // 
            this._bCRUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCRUpgrade.Location = new System.Drawing.Point(773, 348);
            this._bCRUpgrade.Name = "_bCRUpgrade";
            this._bCRUpgrade.Size = new System.Drawing.Size(75, 23);
            this._bCRUpgrade.TabIndex = 2;
            this._bCRUpgrade.Text = "Upgrade";
            this._bCRUpgrade.UseVisualStyleBackColor = true;
            this._bCRUpgrade.Click += new System.EventHandler(this._bCRUpgrade_Click);
            // 
            // _bUpgradeCRRefresh
            // 
            this._bUpgradeCRRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bUpgradeCRRefresh.Location = new System.Drawing.Point(692, 436);
            this._bUpgradeCRRefresh.Name = "_bUpgradeCRRefresh";
            this._bUpgradeCRRefresh.Size = new System.Drawing.Size(75, 23);
            this._bUpgradeCRRefresh.TabIndex = 4;
            this._bUpgradeCRRefresh.Text = "Refresh";
            this._bUpgradeCRRefresh.UseVisualStyleBackColor = true;
            this._bUpgradeCRRefresh.Click += new System.EventHandler(this._bUpgradeCRRefresh_Click);
            // 
            // _chbSelectAll
            // 
            this._chbSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._chbSelectAll.AutoSize = true;
            this._chbSelectAll.Location = new System.Drawing.Point(6, 381);
            this._chbSelectAll.Name = "_chbSelectAll";
            this._chbSelectAll.Size = new System.Drawing.Size(159, 17);
            this._chbSelectAll.TabIndex = 1;
            this._chbSelectAll.Text = "Select/unselect all available";
            this._chbSelectAll.UseVisualStyleBackColor = true;
            this._chbSelectAll.CheckedChanged += new System.EventHandler(this._chbSelectAll_CheckedChanged);
            // 
            // _dgvCRUpgrading
            // 
            this._dgvCRUpgrading.AllowUserToAddRows = false;
            this._dgvCRUpgrading.AllowUserToDeleteRows = false;
            this._dgvCRUpgrading.AllowUserToResizeRows = false;
            this._dgvCRUpgrading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgvCRUpgrading.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this._dgvCRUpgrading.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._dgvCRUpgrading.DefaultCellStyle = dataGridViewCellStyle8;
            this._dgvCRUpgrading.Location = new System.Drawing.Point(3, 117);
            this._dgvCRUpgrading.MultiSelect = false;
            this._dgvCRUpgrading.Name = "_dgvCRUpgrading";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgvCRUpgrading.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this._dgvCRUpgrading.RowHeadersVisible = false;
            this._dgvCRUpgrading.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvCRUpgrading.ShowEditingIcon = false;
            this._dgvCRUpgrading.Size = new System.Drawing.Size(845, 225);
            this._dgvCRUpgrading.TabIndex = 0;
            this._dgvCRUpgrading.TabStop = false;
            this._dgvCRUpgrading.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgvCRUpgrading_CellDoubleClick);
            this._dgvCRUpgrading.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgvCRUpgrading_CellClick);
            this._dgvCRUpgrading.VisibleChanged += new System.EventHandler(this._dgvCRUpgrading_VisibleChanged);
            // 
            // _lAvailableCRUpradeVersions
            // 
            this._lAvailableCRUpradeVersions.AutoSize = true;
            this._lAvailableCRUpradeVersions.Location = new System.Drawing.Point(3, 0);
            this._lAvailableCRUpradeVersions.Name = "_lAvailableCRUpradeVersions";
            this._lAvailableCRUpradeVersions.Size = new System.Drawing.Size(92, 13);
            this._lAvailableCRUpradeVersions.TabIndex = 1;
            this._lAvailableCRUpradeVersions.Text = "Available versions";
            // 
            // _lbAvailableCRUpgradeVersions
            // 
            this._lbAvailableCRUpgradeVersions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbAvailableCRUpgradeVersions.FormattingEnabled = true;
            this._lbAvailableCRUpgradeVersions.Location = new System.Drawing.Point(3, 16);
            this._lbAvailableCRUpgradeVersions.Name = "_lbAvailableCRUpgradeVersions";
            this._lbAvailableCRUpgradeVersions.Size = new System.Drawing.Size(845, 95);
            this._lbAvailableCRUpgradeVersions.TabIndex = 3;
            // 
            // _tpAlarmSettings
            // 
            this._tpAlarmSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpAlarmSettings.Controls.Add(this._accordionAlarmSettings);
            this._tpAlarmSettings.Controls.Add(this._cbOpenAllAlarmSettings);
            this._tpAlarmSettings.Location = new System.Drawing.Point(4, 22);
            this._tpAlarmSettings.Name = "_tpAlarmSettings";
            this._tpAlarmSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tpAlarmSettings.Size = new System.Drawing.Size(851, 379);
            this._tpAlarmSettings.TabIndex = 9;
            this._tpAlarmSettings.Text = "Alarm settings";
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
            this._accordionAlarmSettings.Location = new System.Drawing.Point(3, 29);
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
            this._accordionAlarmSettings.Size = new System.Drawing.Size(845, 347);
            this._accordionAlarmSettings.TabIndex = 6;
            this._accordionAlarmSettings.UpArrow = null;
            // 
            // _cbOpenAllAlarmSettings
            // 
            this._cbOpenAllAlarmSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cbOpenAllAlarmSettings.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._cbOpenAllAlarmSettings.Location = new System.Drawing.Point(645, 6);
            this._cbOpenAllAlarmSettings.Name = "_cbOpenAllAlarmSettings";
            this._cbOpenAllAlarmSettings.Size = new System.Drawing.Size(200, 17);
            this._cbOpenAllAlarmSettings.TabIndex = 5;
            this._cbOpenAllAlarmSettings.Text = "Open all";
            this._cbOpenAllAlarmSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._cbOpenAllAlarmSettings.UseVisualStyleBackColor = true;
            this._cbOpenAllAlarmSettings.CheckedChanged += new System.EventHandler(this._cbOpenAllAlarmSettings_CheckedChanged);
            // 
            // _tpSpecialOutputs
            // 
            this._tpSpecialOutputs.BackColor = System.Drawing.SystemColors.Control;
            this._tpSpecialOutputs.Controls.Add(this._gbOutputSabotageDCUInputs);
            this._tpSpecialOutputs.Controls.Add(this._gbOutputOfflineDCU);
            this._tpSpecialOutputs.Controls.Add(this._gbOutputSabotageDCU);
            this._tpSpecialOutputs.Location = new System.Drawing.Point(4, 22);
            this._tpSpecialOutputs.Name = "_tpSpecialOutputs";
            this._tpSpecialOutputs.Size = new System.Drawing.Size(851, 379);
            this._tpSpecialOutputs.TabIndex = 10;
            this._tpSpecialOutputs.Text = "Special outputs";
            // 
            // _gbOutputSabotageDCUInputs
            // 
            this._gbOutputSabotageDCUInputs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputSabotageDCUInputs.Controls.Add(this._tbmOutputSabotageDCUInputs);
            this._gbOutputSabotageDCUInputs.Controls.Add(this._lOutput6);
            this._gbOutputSabotageDCUInputs.Location = new System.Drawing.Point(6, 118);
            this._gbOutputSabotageDCUInputs.MaximumSize = new System.Drawing.Size(1200, 50);
            this._gbOutputSabotageDCUInputs.Name = "_gbOutputSabotageDCUInputs";
            this._gbOutputSabotageDCUInputs.Size = new System.Drawing.Size(839, 50);
            this._gbOutputSabotageDCUInputs.TabIndex = 5;
            this._gbOutputSabotageDCUInputs.TabStop = false;
            this._gbOutputSabotageDCUInputs.Text = "Output for sabotage of DCU inputs";
            // 
            // _tbmOutputSabotageDCUInputs
            // 
            this._tbmOutputSabotageDCUInputs.AllowDrop = true;
            this._tbmOutputSabotageDCUInputs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotageDCUInputs.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputSabotageDCUInputs.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotageDCUInputs.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSabotageDCUInputs.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputSabotageDCUInputs.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotageDCUInputs.Button.Image")));
            this._tbmOutputSabotageDCUInputs.Button.Location = new System.Drawing.Point(710, 0);
            this._tbmOutputSabotageDCUInputs.Button.Name = "_bMenu";
            this._tbmOutputSabotageDCUInputs.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputSabotageDCUInputs.Button.TabIndex = 3;
            this._tbmOutputSabotageDCUInputs.Button.UseVisualStyleBackColor = false;
            this._tbmOutputSabotageDCUInputs.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSabotageDCUInputs.ButtonDefaultBehaviour = true;
            this._tbmOutputSabotageDCUInputs.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmOutputSabotageDCUInputs.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotageDCUInputs.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputSabotageDCUInputs.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify3,
            this._tsiRemove3});
            this._tbmOutputSabotageDCUInputs.ButtonPopupMenu.Name = "";
            this._tbmOutputSabotageDCUInputs.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputSabotageDCUInputs.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputSabotageDCUInputs.ButtonShowImage = true;
            this._tbmOutputSabotageDCUInputs.ButtonSizeHeight = 20;
            this._tbmOutputSabotageDCUInputs.ButtonSizeWidth = 20;
            this._tbmOutputSabotageDCUInputs.ButtonText = "";
            this._tbmOutputSabotageDCUInputs.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotageDCUInputs.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputSabotageDCUInputs.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotageDCUInputs.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.ContextMenuStrip = this._tbmOutputSabotageDCUInputs.ButtonPopupMenu;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotageDCUInputs.ImageTextBox.Image")));
            this._tbmOutputSabotageDCUInputs.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputSabotageDCUInputs.ImageTextBox.Name = "_itbTextBox";
            this._tbmOutputSabotageDCUInputs.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.ReadOnly = false;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.Size = new System.Drawing.Size(710, 20);
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextBox.Size = new System.Drawing.Size(708, 13);
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.UseImage = true;
            this._tbmOutputSabotageDCUInputs.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputSabotageDCUInputs.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputSabotageDCUInputs_ImageTextBox_DoubleClick);
            this._tbmOutputSabotageDCUInputs.Location = new System.Drawing.Point(103, 19);
            this._tbmOutputSabotageDCUInputs.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputSabotageDCUInputs.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputSabotageDCUInputs.Name = "_tbmOutputSabotageDCUInputs";
            this._tbmOutputSabotageDCUInputs.Size = new System.Drawing.Size(730, 22);
            this._tbmOutputSabotageDCUInputs.TabIndex = 2;
            this._tbmOutputSabotageDCUInputs.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotageDCUInputs.TextImage")));
            this._tbmOutputSabotageDCUInputs.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmOutputSabotageDCUInputs_DragOver);
            this._tbmOutputSabotageDCUInputs.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputSabotageDCUInputs_DragDrop);
            this._tbmOutputSabotageDCUInputs.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmOutputSabotageDCUInputs_ButtonPopupMenuItemClick);
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
            // _gbOutputOfflineDCU
            // 
            this._gbOutputOfflineDCU.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputOfflineDCU.Controls.Add(this._tbmOutputOfflineDcu);
            this._gbOutputOfflineDCU.Controls.Add(this._lOutput5);
            this._gbOutputOfflineDCU.Location = new System.Drawing.Point(6, 6);
            this._gbOutputOfflineDCU.MaximumSize = new System.Drawing.Size(1200, 50);
            this._gbOutputOfflineDCU.Name = "_gbOutputOfflineDCU";
            this._gbOutputOfflineDCU.Size = new System.Drawing.Size(839, 50);
            this._gbOutputOfflineDCU.TabIndex = 4;
            this._gbOutputOfflineDCU.TabStop = false;
            this._gbOutputOfflineDCU.Text = "Output offline DCU";
            // 
            // _tbmOutputOfflineDcu
            // 
            this._tbmOutputOfflineDcu.AllowDrop = true;
            this._tbmOutputOfflineDcu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputOfflineDcu.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputOfflineDcu.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputOfflineDcu.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputOfflineDcu.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputOfflineDcu.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputOfflineDcu.Button.Image")));
            this._tbmOutputOfflineDcu.Button.Location = new System.Drawing.Point(710, 0);
            this._tbmOutputOfflineDcu.Button.Name = "_bMenu";
            this._tbmOutputOfflineDcu.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputOfflineDcu.Button.TabIndex = 3;
            this._tbmOutputOfflineDcu.Button.UseVisualStyleBackColor = false;
            this._tbmOutputOfflineDcu.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputOfflineDcu.ButtonDefaultBehaviour = true;
            this._tbmOutputOfflineDcu.ButtonHoverColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this._tbmOutputOfflineDcu.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputOfflineDcu.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputOfflineDcu.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2});
            this._tbmOutputOfflineDcu.ButtonPopupMenu.Name = "";
            this._tbmOutputOfflineDcu.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputOfflineDcu.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputOfflineDcu.ButtonShowImage = true;
            this._tbmOutputOfflineDcu.ButtonSizeHeight = 20;
            this._tbmOutputOfflineDcu.ButtonSizeWidth = 20;
            this._tbmOutputOfflineDcu.ButtonText = "";
            this._tbmOutputOfflineDcu.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputOfflineDcu.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputOfflineDcu.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputOfflineDcu.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputOfflineDcu.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputOfflineDcu.ImageTextBox.ContextMenuStrip = this._tbmOutputOfflineDcu.ButtonPopupMenu;
            this._tbmOutputOfflineDcu.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputOfflineDcu.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputOfflineDcu.ImageTextBox.Image")));
            this._tbmOutputOfflineDcu.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputOfflineDcu.ImageTextBox.Name = "_itbTextBox";
            this._tbmOutputOfflineDcu.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputOfflineDcu.ImageTextBox.ReadOnly = true;
            this._tbmOutputOfflineDcu.ImageTextBox.Size = new System.Drawing.Size(710, 20);
            this._tbmOutputOfflineDcu.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.Size = new System.Drawing.Size(708, 13);
            this._tbmOutputOfflineDcu.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputOfflineDcu.ImageTextBox.UseImage = true;
            this._tbmOutputOfflineDcu.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputOfflineDcu.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputOfflineDcu_ImageTextBox_DoubleClick);
            this._tbmOutputOfflineDcu.Location = new System.Drawing.Point(103, 19);
            this._tbmOutputOfflineDcu.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputOfflineDcu.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputOfflineDcu.Name = "_tbmOutputOfflineDcu";
            this._tbmOutputOfflineDcu.Size = new System.Drawing.Size(730, 22);
            this._tbmOutputOfflineDcu.TabIndex = 1;
            this._tbmOutputOfflineDcu.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputOfflineDcu.TextImage")));
            this._tbmOutputOfflineDcu.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmOutputOfflineDcu_DragOver);
            this._tbmOutputOfflineDcu.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputOfflineDcu_DragDrop);
            this._tbmOutputOfflineDcu.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmOutputOfflineDcu_ButtonPopupMenuItemClick);
            // 
            // _lOutput5
            // 
            this._lOutput5.AutoSize = true;
            this._lOutput5.Location = new System.Drawing.Point(6, 23);
            this._lOutput5.Name = "_lOutput5";
            this._lOutput5.Size = new System.Drawing.Size(39, 13);
            this._lOutput5.TabIndex = 0;
            this._lOutput5.Text = "Output";
            // 
            // _gbOutputSabotageDCU
            // 
            this._gbOutputSabotageDCU.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbOutputSabotageDCU.Controls.Add(this._tbmOutputSabotageDcu);
            this._gbOutputSabotageDCU.Controls.Add(this._lOutput4);
            this._gbOutputSabotageDCU.Location = new System.Drawing.Point(6, 62);
            this._gbOutputSabotageDCU.MaximumSize = new System.Drawing.Size(1200, 50);
            this._gbOutputSabotageDCU.Name = "_gbOutputSabotageDCU";
            this._gbOutputSabotageDCU.Size = new System.Drawing.Size(839, 50);
            this._gbOutputSabotageDCU.TabIndex = 3;
            this._gbOutputSabotageDCU.TabStop = false;
            this._gbOutputSabotageDCU.Text = "Output sabotage DCU";
            // 
            // _tbmOutputSabotageDcu
            // 
            this._tbmOutputSabotageDcu.AllowDrop = true;
            this._tbmOutputSabotageDcu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotageDcu.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmOutputSabotageDcu.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotageDcu.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSabotageDcu.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmOutputSabotageDcu.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotageDcu.Button.Image")));
            this._tbmOutputSabotageDcu.Button.Location = new System.Drawing.Point(710, 0);
            this._tbmOutputSabotageDcu.Button.Name = "_bMenu";
            this._tbmOutputSabotageDcu.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmOutputSabotageDcu.Button.TabIndex = 3;
            this._tbmOutputSabotageDcu.Button.UseVisualStyleBackColor = false;
            this._tbmOutputSabotageDcu.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmOutputSabotageDcu.ButtonDefaultBehaviour = true;
            this._tbmOutputSabotageDcu.ButtonHoverColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this._tbmOutputSabotageDcu.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotageDcu.ButtonImage")));
            // 
            // 
            // 
            this._tbmOutputSabotageDcu.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify1,
            this._tsiRemove1});
            this._tbmOutputSabotageDcu.ButtonPopupMenu.Name = "";
            this._tbmOutputSabotageDcu.ButtonPopupMenu.Size = new System.Drawing.Size(181, 48);
            this._tbmOutputSabotageDcu.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmOutputSabotageDcu.ButtonShowImage = true;
            this._tbmOutputSabotageDcu.ButtonSizeHeight = 20;
            this._tbmOutputSabotageDcu.ButtonSizeWidth = 20;
            this._tbmOutputSabotageDcu.ButtonText = "";
            this._tbmOutputSabotageDcu.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotageDcu.HoverTime = 500;
            // 
            // 
            // 
            this._tbmOutputSabotageDcu.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotageDcu.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmOutputSabotageDcu.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmOutputSabotageDcu.ImageTextBox.ContextMenuStrip = this._tbmOutputSabotageDcu.ButtonPopupMenu;
            this._tbmOutputSabotageDcu.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotageDcu.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotageDcu.ImageTextBox.Image")));
            this._tbmOutputSabotageDcu.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmOutputSabotageDcu.ImageTextBox.Name = "_itbTextBox";
            this._tbmOutputSabotageDcu.ImageTextBox.NoTextNoImage = true;
            this._tbmOutputSabotageDcu.ImageTextBox.ReadOnly = true;
            this._tbmOutputSabotageDcu.ImageTextBox.Size = new System.Drawing.Size(710, 20);
            this._tbmOutputSabotageDcu.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.Size = new System.Drawing.Size(708, 13);
            this._tbmOutputSabotageDcu.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmOutputSabotageDcu.ImageTextBox.UseImage = true;
            this._tbmOutputSabotageDcu.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmOutputSabotageDcu.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmOutputSabotageDcu_ImageTextBox_DoubleClick);
            this._tbmOutputSabotageDcu.Location = new System.Drawing.Point(103, 19);
            this._tbmOutputSabotageDcu.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmOutputSabotageDcu.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmOutputSabotageDcu.Name = "_tbmOutputSabotageDcu";
            this._tbmOutputSabotageDcu.Size = new System.Drawing.Size(730, 22);
            this._tbmOutputSabotageDcu.TabIndex = 1;
            this._tbmOutputSabotageDcu.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmOutputSabotageDcu.TextImage")));
            this._tbmOutputSabotageDcu.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmOutputSabotageDcu_DragOver);
            this._tbmOutputSabotageDcu.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmOutputSabotageDcu_DragDrop);
            this._tbmOutputSabotageDcu.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmOutputSabotageDcu_ButtonPopupMenuItemClick);
            // 
            // _lOutput4
            // 
            this._lOutput4.AutoSize = true;
            this._lOutput4.Location = new System.Drawing.Point(6, 23);
            this._lOutput4.Name = "_lOutput4";
            this._lOutput4.Size = new System.Drawing.Size(39, 13);
            this._lOutput4.TabIndex = 0;
            this._lOutput4.Text = "Output";
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh1);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 22);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(851, 379);
            this._tpUserFolders.TabIndex = 8;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh1
            // 
            this._bRefresh1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh1.Location = new System.Drawing.Point(773, 348);
            this._bRefresh1.Name = "_bRefresh1";
            this._bRefresh1.Size = new System.Drawing.Size(75, 23);
            this._bRefresh1.TabIndex = 3;
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
            this._lbUserFolders.Location = new System.Drawing.Point(3, 3);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(845, 329);
            this._lbUserFolders.TabIndex = 32;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(3);
            this._tpReferencedBy.Size = new System.Drawing.Size(851, 379);
            this._tpReferencedBy.TabIndex = 7;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(2);
            this._tpDescription.Size = new System.Drawing.Size(851, 379);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(2, 2);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(847, 375);
            this._eDescription.TabIndex = 3;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // NCASDCUEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(884, 661);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(900, 700);
            this.Name = "NCASDCUEditForm";
            this.Text = "NCASDCUEditForm";
            ((System.ComponentModel.ISupportInitialize)(this._nudLogicalAddress)).EndInit();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this._tcDCU.ResumeLayout(false);
            this._tpSettings.ResumeLayout(false);
            this._tpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._eOutputCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eInputCount)).EndInit();
            this._tpInpupOutputSettings.ResumeLayout(false);
            this._scInputsOutputs.Panel1.ResumeLayout(false);
            this._scInputsOutputs.Panel2.ResumeLayout(false);
            this._scInputsOutputs.ResumeLayout(false);
            this._gbInputDCU.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvInput.DataGrid)).EndInit();
            this._gbOutputDCU.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvOutput.DataGrid)).EndInit();
            this._tpControl.ResumeLayout(false);
            this._gbDcuMemoryLoad.ResumeLayout(false);
            this._gbDcuMemoryLoad.PerformLayout();
            this._gbDcuUpgrade.ResumeLayout(false);
            this._gbDcuUpgrade.PerformLayout();
            this._gbDevice.ResumeLayout(false);
            this._tpCRUpgrades.ResumeLayout(false);
            this._tpCRUpgrades.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvCRUpgrading)).EndInit();
            this._tpAlarmSettings.ResumeLayout(false);
            this._tpSpecialOutputs.ResumeLayout(false);
            this._gbOutputSabotageDCUInputs.ResumeLayout(false);
            this._gbOutputSabotageDCUInputs.PerformLayout();
            this._gbOutputOfflineDCU.ResumeLayout(false);
            this._gbOutputOfflineDCU.PerformLayout();
            this._gbOutputSabotageDCU.ResumeLayout(false);
            this._gbOutputSabotageDCU.PerformLayout();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.Label _lSuperiorCCU;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.Label _lDoorEnvironmentState;
        private System.Windows.Forms.TextBox _eDoorEnvironmentState;
        private System.Windows.Forms.TextBox _ePhysicalAddress;
        private System.Windows.Forms.Label _lPhysicalAddress;
        private System.Windows.Forms.TextBox _eLogicalAddress;
        private System.Windows.Forms.Label _lLogicalAddress;
        private System.Windows.Forms.TextBox _eOnlineState;
        private System.Windows.Forms.Label _lActualState;
        private System.Windows.Forms.NumericUpDown _nudLogicalAddress;
        private Contal.IwQuick.UI.ImageTextBox _itbSuperiorCCU;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove1;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify3;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove3;
        private System.Windows.Forms.TextBox _eDcuInputsSabotageState;
        private System.Windows.Forms.Label _lDcuInputsSabotageState;
        private System.Windows.Forms.TabControl _tcDCU;
        private System.Windows.Forms.TabPage _tpSettings;
        private Contal.IwQuick.UI.ImageListBox _ilbCardReaders;
        private System.Windows.Forms.NumericUpDown _eOutputCount;
        private System.Windows.Forms.Label _lOutputCount;
        private System.Windows.Forms.Label _lInputCount;
        private System.Windows.Forms.NumericUpDown _eInputCount;
        private System.Windows.Forms.Button _bCRMarkAsDead;
        private System.Windows.Forms.Button _bPrecreateCR;
        private System.Windows.Forms.Label _lCardReaders;
        private System.Windows.Forms.TabPage _tpInpupOutputSettings;
        private System.Windows.Forms.SplitContainer _scInputsOutputs;
        private System.Windows.Forms.GroupBox _gbInputDCU;
        private Contal.Cgp.Components.CgpDataGridView _cdgvInput;
        private System.Windows.Forms.GroupBox _gbOutputDCU;
        private Contal.Cgp.Components.CgpDataGridView _cdgvOutput;
        private System.Windows.Forms.TabPage _tpControl;
        private System.Windows.Forms.GroupBox _gbDcuMemoryLoad;
        private System.Windows.Forms.Button _bDcuMemoryLoadRefresh;
        private System.Windows.Forms.TextBox _eDcuMemoryLoad;
        private System.Windows.Forms.Label _lDcuMemoryLoad;
        private System.Windows.Forms.GroupBox _gbDcuUpgrade;
        private System.Windows.Forms.Label _lFirmwareVersion;
        private System.Windows.Forms.TextBox _eFirmware;
        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.GroupBox _gbDevice;
        private System.Windows.Forms.Button _bReset;
        private System.Windows.Forms.TabPage _tpCRUpgrades;
        private System.Windows.Forms.Button _bCRUpgrade;
        private System.Windows.Forms.Button _bUpgradeCRRefresh;
        private System.Windows.Forms.CheckBox _chbSelectAll;
        private System.Windows.Forms.DataGridView _dgvCRUpgrading;
        private System.Windows.Forms.Label _lAvailableCRUpradeVersions;
        private System.Windows.Forms.ListBox _lbAvailableCRUpgradeVersions;
        private System.Windows.Forms.TabPage _tpAlarmSettings;
        private System.Windows.Forms.TabPage _tpSpecialOutputs;
        private System.Windows.Forms.GroupBox _gbOutputSabotageDCUInputs;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputSabotageDCUInputs;
        private System.Windows.Forms.Label _lOutput6;
        private System.Windows.Forms.GroupBox _gbOutputOfflineDCU;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputOfflineDcu;
        private System.Windows.Forms.Label _lOutput5;
        private System.Windows.Forms.GroupBox _gbOutputSabotageDCU;
        private Contal.IwQuick.UI.TextBoxMenu _tbmOutputSabotageDcu;
        private System.Windows.Forms.Label _lOutput4;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh1;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Label _lDoorEnvironment;
        private Contal.IwQuick.UI.ImageTextBox _itbDoorEnvironment;
        private System.Windows.Forms.CheckBox _cbOpenAllAlarmSettings;
        private Contal.IwQuick.PlatformPC.UI.Accordion.Accordion _accordionAlarmSettings;

    }
}
