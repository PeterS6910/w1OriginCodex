namespace Contal.Cgp.NCAS.Client
{
    partial class NCASCardReadersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASCardReadersForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._pFilter = new System.Windows.Forms.Panel();
            this.bExportExcel = new System.Windows.Forms.Button();
            this._tbmMemberOfAclFilter = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove1 = new System.Windows.Forms.ToolStripMenuItem();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._gbRuntimeFilter = new System.Windows.Forms.GroupBox();
            this._bUnblock = new System.Windows.Forms.Button();
            this._chbBlocked = new System.Windows.Forms.CheckBox();
            this._cbOnlineStateFilter = new System.Windows.Forms.ComboBox();
            this._lOnlineStateFilter = new System.Windows.Forms.Label();
            this._lCurrentSecurityLevelFilter = new System.Windows.Forms.Label();
            this._clbCurrentSecurityLevel = new System.Windows.Forms.CheckedListBox();
            this._cbCurrentForcedSecurityLevel = new System.Windows.Forms.ComboBox();
            this._lCurrentForcedSecurityLevel = new System.Windows.Forms.Label();
            this._rbFilterOr = new System.Windows.Forms.RadioButton();
            this._rbFilterAnd = new System.Windows.Forms.RadioButton();
            this._lMemberOfAclFilter = new System.Windows.Forms.Label();
            this._clbSecurityLevelFilter = new System.Windows.Forms.CheckedListBox();
            this._lSecurityLevelFilter = new System.Windows.Forms.Label();
            this._lDcu = new System.Windows.Forms.Label();
            this._cbDcu = new System.Windows.Forms.ComboBox();
            this._lEmergencyCodeFilter = new System.Windows.Forms.Label();
            this._lForcedSecurityLevel = new System.Windows.Forms.Label();
            this._lCcu = new System.Windows.Forms.Label();
            this._cbEmergencyCode = new System.Windows.Forms.ComboBox();
            this._cbForcedSecurityLevel = new System.Windows.Forms.ComboBox();
            this._cbCcu = new System.Windows.Forms.ComboBox();
            this._lNameFilter = new System.Windows.Forms.Label();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter.SuspendLayout();
            this._gbRuntimeFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this.bExportExcel);
            this._pFilter.Controls.Add(this._tbmMemberOfAclFilter);
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._gbRuntimeFilter);
            this._pFilter.Controls.Add(this._rbFilterOr);
            this._pFilter.Controls.Add(this._rbFilterAnd);
            this._pFilter.Controls.Add(this._lMemberOfAclFilter);
            this._pFilter.Controls.Add(this._clbSecurityLevelFilter);
            this._pFilter.Controls.Add(this._lSecurityLevelFilter);
            this._pFilter.Controls.Add(this._lDcu);
            this._pFilter.Controls.Add(this._cbDcu);
            this._pFilter.Controls.Add(this._lEmergencyCodeFilter);
            this._pFilter.Controls.Add(this._lForcedSecurityLevel);
            this._pFilter.Controls.Add(this._lCcu);
            this._pFilter.Controls.Add(this._cbEmergencyCode);
            this._pFilter.Controls.Add(this._cbForcedSecurityLevel);
            this._pFilter.Controls.Add(this._cbCcu);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 282);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(1200, 210);
            this._pFilter.TabIndex = 1;
            // 
            // bExportExcel
            // 
            this.bExportExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bExportExcel.Location = new System.Drawing.Point(1073, 170);
            this.bExportExcel.Name = "bExportExcel";
            this.bExportExcel.Size = new System.Drawing.Size(124, 32);
            this.bExportExcel.TabIndex = 51;
            this.bExportExcel.Text = "Export to Excel";
            this.bExportExcel.UseVisualStyleBackColor = false;
            this.bExportExcel.Click += new System.EventHandler(this.bExportToExcel_Click);
            // 
            // _tbmMemberOfAclFilter
            // 
            this._tbmMemberOfAclFilter.AllowDrop = true;
            this._tbmMemberOfAclFilter.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmMemberOfAclFilter.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmMemberOfAclFilter.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmMemberOfAclFilter.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmMemberOfAclFilter.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmMemberOfAclFilter.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmMemberOfAclFilter.Button.Image")));
            this._tbmMemberOfAclFilter.Button.Location = new System.Drawing.Point(210, 0);
            this._tbmMemberOfAclFilter.Button.Name = "_bMenu";
            this._tbmMemberOfAclFilter.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmMemberOfAclFilter.Button.TabIndex = 3;
            this._tbmMemberOfAclFilter.Button.UseVisualStyleBackColor = false;
            this._tbmMemberOfAclFilter.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmMemberOfAclFilter.ButtonDefaultBehaviour = true;
            this._tbmMemberOfAclFilter.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmMemberOfAclFilter.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmMemberOfAclFilter.ButtonImage")));
            // 
            // 
            // 
            this._tbmMemberOfAclFilter.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify1,
            this._tsiRemove1});
            this._tbmMemberOfAclFilter.ButtonPopupMenu.Name = "";
            this._tbmMemberOfAclFilter.ButtonPopupMenu.Size = new System.Drawing.Size(149, 68);
            this._tbmMemberOfAclFilter.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmMemberOfAclFilter.ButtonShowImage = true;
            this._tbmMemberOfAclFilter.ButtonSizeHeight = 20;
            this._tbmMemberOfAclFilter.ButtonSizeWidth = 20;
            this._tbmMemberOfAclFilter.ButtonText = "";
            this._tbmMemberOfAclFilter.HoverTime = 500;
            // 
            // 
            // 
            this._tbmMemberOfAclFilter.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmMemberOfAclFilter.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmMemberOfAclFilter.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmMemberOfAclFilter.ImageTextBox.ContextMenuStrip = this._tbmMemberOfAclFilter.ButtonPopupMenu;
            this._tbmMemberOfAclFilter.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmMemberOfAclFilter.ImageTextBox.Image")));
            this._tbmMemberOfAclFilter.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmMemberOfAclFilter.ImageTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmMemberOfAclFilter.ImageTextBox.Name = "_textBox";
            this._tbmMemberOfAclFilter.ImageTextBox.NoTextNoImage = true;
            this._tbmMemberOfAclFilter.ImageTextBox.ReadOnly = true;
            this._tbmMemberOfAclFilter.ImageTextBox.Size = new System.Drawing.Size(210, 20);
            this._tbmMemberOfAclFilter.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.Margin = new System.Windows.Forms.Padding(4);
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.Size = new System.Drawing.Size(208, 19);
            this._tbmMemberOfAclFilter.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmMemberOfAclFilter.ImageTextBox.UseImage = true;
            this._tbmMemberOfAclFilter.Location = new System.Drawing.Point(8, 81);
            this._tbmMemberOfAclFilter.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmMemberOfAclFilter.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmMemberOfAclFilter.Name = "_tbmMemberOfAclFilter";
            this._tbmMemberOfAclFilter.Size = new System.Drawing.Size(230, 22);
            this._tbmMemberOfAclFilter.TabIndex = 50;
            this._tbmMemberOfAclFilter.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmMemberOfAclFilter.TextImage")));
            this._tbmMemberOfAclFilter.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmMemberOfAclFilter_ButtonPopupMenuItemClick);
            this._tbmMemberOfAclFilter.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmMemberOfAclFilter_DragDrop);
            this._tbmMemberOfAclFilter.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmMemberOfAclFilter_DragOver);
            // 
            // _tsiModify1
            // 
            this._tsiModify1.Name = "_tsiModify1";
            this._tsiModify1.Size = new System.Drawing.Size(148, 32);
            this._tsiModify1.Text = "Modify";
            // 
            // _tsiRemove1
            // 
            this._tsiRemove1.Name = "_tsiRemove1";
            this._tsiRemove1.Size = new System.Drawing.Size(148, 32);
            this._tsiRemove1.Text = "Remove";
            // 
            // _lRecordCount
            // 
            this._lRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(988, 135);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(109, 20);
            this._lRecordCount.TabIndex = 49;
            this._lRecordCount.Text = "Record count:";
            // 
            // _gbRuntimeFilter
            // 
            this._gbRuntimeFilter.Controls.Add(this._bUnblock);
            this._gbRuntimeFilter.Controls.Add(this._chbBlocked);
            this._gbRuntimeFilter.Controls.Add(this._cbOnlineStateFilter);
            this._gbRuntimeFilter.Controls.Add(this._lOnlineStateFilter);
            this._gbRuntimeFilter.Controls.Add(this._lCurrentSecurityLevelFilter);
            this._gbRuntimeFilter.Controls.Add(this._clbCurrentSecurityLevel);
            this._gbRuntimeFilter.Controls.Add(this._cbCurrentForcedSecurityLevel);
            this._gbRuntimeFilter.Controls.Add(this._lCurrentForcedSecurityLevel);
            this._gbRuntimeFilter.Location = new System.Drawing.Point(601, 10);
            this._gbRuntimeFilter.Name = "_gbRuntimeFilter";
            this._gbRuntimeFilter.Size = new System.Drawing.Size(364, 150);
            this._gbRuntimeFilter.TabIndex = 47;
            this._gbRuntimeFilter.TabStop = false;
            this._gbRuntimeFilter.Tag = "RealtimeFiltering";
            this._gbRuntimeFilter.Text = "Runtime filters";
            // 
            // _bUnblock
            // 
            this._bUnblock.Enabled = false;
            this._bUnblock.Location = new System.Drawing.Point(279, 98);
            this._bUnblock.Name = "_bUnblock";
            this._bUnblock.Size = new System.Drawing.Size(75, 23);
            this._bUnblock.TabIndex = 30;
            this._bUnblock.Text = "Unblock";
            this._bUnblock.UseVisualStyleBackColor = true;
            this._bUnblock.Click += new System.EventHandler(this._bUnblock_Click);
            // 
            // _chbBlocked
            // 
            this._chbBlocked.AutoSize = true;
            this._chbBlocked.Location = new System.Drawing.Point(9, 120);
            this._chbBlocked.Name = "_chbBlocked";
            this._chbBlocked.Size = new System.Drawing.Size(323, 24);
            this._chbBlocked.TabIndex = 29;
            this._chbBlocked.Text = "Blocked (invalid GIN retries limit reached)";
            this._chbBlocked.UseVisualStyleBackColor = true;
            this._chbBlocked.CheckedChanged += new System.EventHandler(this._chbBlocked_CheckedChanged);
            // 
            // _cbOnlineStateFilter
            // 
            this._cbOnlineStateFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbOnlineStateFilter.FormattingEnabled = true;
            this._cbOnlineStateFilter.Items.AddRange(new object[] {
            "",
            "Unknown",
            "Offline",
            "Online",
            "Upgrading",
            "WaitingForUpgrade",
            "AutoUpgrading",
            "Reseting"});
            this._cbOnlineStateFilter.Location = new System.Drawing.Point(199, 22);
            this._cbOnlineStateFilter.Name = "_cbOnlineStateFilter";
            this._cbOnlineStateFilter.Size = new System.Drawing.Size(155, 28);
            this._cbOnlineStateFilter.TabIndex = 7;
            this._cbOnlineStateFilter.Tag = new string[] {
        "",
        "Unknown",
        "Offline",
        "Online",
        "Upgrading",
        "WaitingForUpgrade",
        "AutoUpgrading",
        "Reseting"};
            this._cbOnlineStateFilter.SelectedIndexChanged += new System.EventHandler(this._rbFilterOr_CheckedChanged);
            this._cbOnlineStateFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress);
            // 
            // _lOnlineStateFilter
            // 
            this._lOnlineStateFilter.AutoSize = true;
            this._lOnlineStateFilter.Location = new System.Drawing.Point(196, 2);
            this._lOnlineStateFilter.Name = "_lOnlineStateFilter";
            this._lOnlineStateFilter.Size = new System.Drawing.Size(94, 20);
            this._lOnlineStateFilter.TabIndex = 6;
            this._lOnlineStateFilter.Text = "Online state";
            // 
            // _lCurrentSecurityLevelFilter
            // 
            this._lCurrentSecurityLevelFilter.AutoSize = true;
            this._lCurrentSecurityLevelFilter.Location = new System.Drawing.Point(6, 30);
            this._lCurrentSecurityLevelFilter.Name = "_lCurrentSecurityLevelFilter";
            this._lCurrentSecurityLevelFilter.Size = new System.Drawing.Size(155, 20);
            this._lCurrentSecurityLevelFilter.TabIndex = 27;
            this._lCurrentSecurityLevelFilter.Text = "Current security level";
            // 
            // _clbCurrentSecurityLevel
            // 
            this._clbCurrentSecurityLevel.FormattingEnabled = true;
            this._clbCurrentSecurityLevel.IntegralHeight = false;
            this._clbCurrentSecurityLevel.Items.AddRange(new object[] {
            "Unlocked",
            "Only CODE",
            "CODE/Card",
            "CODE/Card + PIN",
            "Card",
            "Card + PIN",
            "Locked",
            "Not used"});
            this._clbCurrentSecurityLevel.Location = new System.Drawing.Point(5, 60);
            this._clbCurrentSecurityLevel.Name = "_clbCurrentSecurityLevel";
            this._clbCurrentSecurityLevel.Size = new System.Drawing.Size(181, 54);
            this._clbCurrentSecurityLevel.TabIndex = 28;
            this._clbCurrentSecurityLevel.Tag = new string[] {
        "SecurityLevelStates_Unlocked",
        "SecurityLevelStates_CODE",
        "SecurityLevelStates_CODEORCARD",
        "SecurityLevelStates_CODEORCARDPIN",
        "SecurityLevelStates_CARD",
        "SecurityLevelStates_CARDPIN",
        "SecurityLevelStates_Locked",
        "StringNotUsed"};
            this._clbCurrentSecurityLevel.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.TextBoxListItemChecked_ItemCheck);
            // 
            // _cbCurrentForcedSecurityLevel
            // 
            this._cbCurrentForcedSecurityLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCurrentForcedSecurityLevel.FormattingEnabled = true;
            this._cbCurrentForcedSecurityLevel.Items.AddRange(new object[] {
            "",
            "True",
            "False"});
            this._cbCurrentForcedSecurityLevel.Location = new System.Drawing.Point(199, 71);
            this._cbCurrentForcedSecurityLevel.Name = "_cbCurrentForcedSecurityLevel";
            this._cbCurrentForcedSecurityLevel.Size = new System.Drawing.Size(155, 28);
            this._cbCurrentForcedSecurityLevel.TabIndex = 3;
            this._cbCurrentForcedSecurityLevel.Tag = new string[] {
        "",
        "General_True",
        "General_False"};
            this._cbCurrentForcedSecurityLevel.SelectedIndexChanged += new System.EventHandler(this._rbFilterOr_CheckedChanged);
            this._cbCurrentForcedSecurityLevel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress);
            // 
            // _lCurrentForcedSecurityLevel
            // 
            this._lCurrentForcedSecurityLevel.AutoSize = true;
            this._lCurrentForcedSecurityLevel.Location = new System.Drawing.Point(196, 50);
            this._lCurrentForcedSecurityLevel.Name = "_lCurrentForcedSecurityLevel";
            this._lCurrentForcedSecurityLevel.Size = new System.Drawing.Size(152, 20);
            this._lCurrentForcedSecurityLevel.TabIndex = 2;
            this._lCurrentForcedSecurityLevel.Text = "Forced security level";
            // 
            // _rbFilterOr
            // 
            this._rbFilterOr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbFilterOr.AutoSize = true;
            this._rbFilterOr.Location = new System.Drawing.Point(3, 170);
            this._rbFilterOr.Name = "_rbFilterOr";
            this._rbFilterOr.Size = new System.Drawing.Size(411, 24);
            this._rbFilterOr.TabIndex = 32;
            this._rbFilterOr.Text = "Show card readers that match at least one parameter";
            this._rbFilterOr.UseVisualStyleBackColor = true;
            this._rbFilterOr.CheckedChanged += new System.EventHandler(this._rbFilterOr_CheckedChanged);
            // 
            // _rbFilterAnd
            // 
            this._rbFilterAnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbFilterAnd.AutoSize = true;
            this._rbFilterAnd.Checked = true;
            this._rbFilterAnd.Location = new System.Drawing.Point(444, 170);
            this._rbFilterAnd.Name = "_rbFilterAnd";
            this._rbFilterAnd.Size = new System.Drawing.Size(351, 24);
            this._rbFilterAnd.TabIndex = 31;
            this._rbFilterAnd.TabStop = true;
            this._rbFilterAnd.Text = "Show card readers that match all parameters";
            this._rbFilterAnd.UseVisualStyleBackColor = true;
            this._rbFilterAnd.CheckedChanged += new System.EventHandler(this._rbFilterOr_CheckedChanged);
            // 
            // _lMemberOfAclFilter
            // 
            this._lMemberOfAclFilter.AutoSize = true;
            this._lMemberOfAclFilter.Location = new System.Drawing.Point(5, 55);
            this._lMemberOfAclFilter.Name = "_lMemberOfAclFilter";
            this._lMemberOfAclFilter.Size = new System.Drawing.Size(214, 20);
            this._lMemberOfAclFilter.TabIndex = 29;
            this._lMemberOfAclFilter.Text = "Member of access control list";
            // 
            // _clbSecurityLevelFilter
            // 
            this._clbSecurityLevelFilter.FormattingEnabled = true;
            this._clbSecurityLevelFilter.IntegralHeight = false;
            this._clbSecurityLevelFilter.Items.AddRange(new object[] {
            "Unlocked",
            "Only CODE",
            "CODE/Card",
            "CODE/Card + PIN",
            "Card",
            "Card + PIN",
            "Locked",
            "Sec. time zone / daily plan"});
            this._clbSecurityLevelFilter.Location = new System.Drawing.Point(401, 30);
            this._clbSecurityLevelFilter.Name = "_clbSecurityLevelFilter";
            this._clbSecurityLevelFilter.Size = new System.Drawing.Size(181, 79);
            this._clbSecurityLevelFilter.TabIndex = 28;
            this._clbSecurityLevelFilter.Tag = new string[] {
        "SecurityLevelStates_Unlocked",
        "SecurityLevelStates_CODE",
        "SecurityLevelStates_CODEORCARD",
        "SecurityLevelStates_CODEORCARDPIN",
        "SecurityLevelStates_CARD",
        "SecurityLevelStates_CARDPIN",
        "SecurityLevelStates_Locked",
        "SecurityLevelStates_SecurityTimeZoneSecurityDailyPlan"};
            this._clbSecurityLevelFilter.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.TextBoxListItemChecked_ItemCheck);
            // 
            // _lSecurityLevelFilter
            // 
            this._lSecurityLevelFilter.AutoSize = true;
            this._lSecurityLevelFilter.Location = new System.Drawing.Point(398, 4);
            this._lSecurityLevelFilter.Name = "_lSecurityLevelFilter";
            this._lSecurityLevelFilter.Size = new System.Drawing.Size(101, 20);
            this._lSecurityLevelFilter.TabIndex = 27;
            this._lSecurityLevelFilter.Text = "Security level";
            // 
            // _lDcu
            // 
            this._lDcu.AutoSize = true;
            this._lDcu.Location = new System.Drawing.Point(246, 55);
            this._lDcu.Name = "_lDcu";
            this._lDcu.Size = new System.Drawing.Size(44, 20);
            this._lDcu.TabIndex = 4;
            this._lDcu.Text = "DCU";
            // 
            // _cbDcu
            // 
            this._cbDcu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbDcu.FormattingEnabled = true;
            this._cbDcu.Location = new System.Drawing.Point(244, 77);
            this._cbDcu.Name = "_cbDcu";
            this._cbDcu.Size = new System.Drawing.Size(151, 28);
            this._cbDcu.TabIndex = 5;
            this._cbDcu.SelectedIndexChanged += new System.EventHandler(this._cbDcu_SelectedIndexChanged);
            // 
            // _lEmergencyCodeFilter
            // 
            this._lEmergencyCodeFilter.AutoSize = true;
            this._lEmergencyCodeFilter.Location = new System.Drawing.Point(240, 111);
            this._lEmergencyCodeFilter.Name = "_lEmergencyCodeFilter";
            this._lEmergencyCodeFilter.Size = new System.Drawing.Size(128, 20);
            this._lEmergencyCodeFilter.TabIndex = 2;
            this._lEmergencyCodeFilter.Text = "Emergency code";
            // 
            // _lForcedSecurityLevel
            // 
            this._lForcedSecurityLevel.AutoSize = true;
            this._lForcedSecurityLevel.Location = new System.Drawing.Point(5, 111);
            this._lForcedSecurityLevel.Name = "_lForcedSecurityLevel";
            this._lForcedSecurityLevel.Size = new System.Drawing.Size(152, 20);
            this._lForcedSecurityLevel.TabIndex = 2;
            this._lForcedSecurityLevel.Text = "Forced security level";
            // 
            // _lCcu
            // 
            this._lCcu.AutoSize = true;
            this._lCcu.Location = new System.Drawing.Point(241, 4);
            this._lCcu.Name = "_lCcu";
            this._lCcu.Size = new System.Drawing.Size(43, 20);
            this._lCcu.TabIndex = 2;
            this._lCcu.Text = "CCU";
            // 
            // _cbEmergencyCode
            // 
            this._cbEmergencyCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbEmergencyCode.FormattingEnabled = true;
            this._cbEmergencyCode.Items.AddRange(new object[] {
            "",
            "True",
            "False"});
            this._cbEmergencyCode.Location = new System.Drawing.Point(244, 137);
            this._cbEmergencyCode.Name = "_cbEmergencyCode";
            this._cbEmergencyCode.Size = new System.Drawing.Size(112, 28);
            this._cbEmergencyCode.TabIndex = 3;
            this._cbEmergencyCode.Tag = new string[] {
        "",
        "General_True",
        "General_False"};
            this._cbEmergencyCode.SelectedIndexChanged += new System.EventHandler(this._rbFilterOr_CheckedChanged);
            this._cbEmergencyCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress);
            // 
            // _cbForcedSecurityLevel
            // 
            this._cbForcedSecurityLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbForcedSecurityLevel.FormattingEnabled = true;
            this._cbForcedSecurityLevel.Items.AddRange(new object[] {
            "",
            "True",
            "False"});
            this._cbForcedSecurityLevel.Location = new System.Drawing.Point(8, 137);
            this._cbForcedSecurityLevel.Name = "_cbForcedSecurityLevel";
            this._cbForcedSecurityLevel.Size = new System.Drawing.Size(112, 28);
            this._cbForcedSecurityLevel.TabIndex = 3;
            this._cbForcedSecurityLevel.Tag = new string[] {
        "",
        "General_True",
        "General_False"};
            this._cbForcedSecurityLevel.SelectedIndexChanged += new System.EventHandler(this._rbFilterOr_CheckedChanged);
            this._cbForcedSecurityLevel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress);
            // 
            // _cbCcu
            // 
            this._cbCcu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCcu.FormattingEnabled = true;
            this._cbCcu.Location = new System.Drawing.Point(244, 26);
            this._cbCcu.Name = "_cbCcu";
            this._cbCcu.Size = new System.Drawing.Size(151, 28);
            this._cbCcu.TabIndex = 3;
            this._cbCcu.SelectedIndexChanged += new System.EventHandler(this._cbCcu_SelectedIndexChanged);
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Location = new System.Drawing.Point(5, 4);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Size = new System.Drawing.Size(51, 20);
            this._lNameFilter.TabIndex = 0;
            this._lNameFilter.Text = "Name";
            // 
            // _eNameFilter
            // 
            this._eNameFilter.Location = new System.Drawing.Point(8, 26);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Size = new System.Drawing.Size(230, 26);
            this._eNameFilter.TabIndex = 1;
            this._eNameFilter.TextChanged += new System.EventHandler(this._eNameFilter_TextChanged);
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bFilterClear.Location = new System.Drawing.Point(980, 170);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(75, 32);
            this._bFilterClear.TabIndex = 9;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRunFilter.Location = new System.Drawing.Point(888, 170);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(75, 32);
            this._bRunFilter.TabIndex = 8;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.CgpDataGridEvents = null;
            this._cdgvData.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvData.DataGrid.AllowUserToAddRows = false;
            this._cdgvData.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvData.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvData.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._cdgvData.DataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._cdgvData.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvData.DataGrid.ColumnHeadersHeight = 34;
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidth = 62;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(1200, 282);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(1200, 282);
            this._cdgvData.TabIndex = 2;
            // 
            // NCASCardReadersForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1200, 492);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NCASCardReadersForm";
            this.Text = "CardReadersForm";
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this._gbRuntimeFilter.ResumeLayout(false);
            this._gbRuntimeFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lCcu;
        private System.Windows.Forms.ComboBox _cbCcu;
        private System.Windows.Forms.Label _lDcu;
        private System.Windows.Forms.ComboBox _cbDcu;
        private System.Windows.Forms.ComboBox _cbOnlineStateFilter;
        private System.Windows.Forms.Label _lOnlineStateFilter;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.CheckedListBox _clbSecurityLevelFilter;
        private System.Windows.Forms.Label _lSecurityLevelFilter;
        private System.Windows.Forms.Label _lMemberOfAclFilter;
        private System.Windows.Forms.RadioButton _rbFilterOr;
        private System.Windows.Forms.RadioButton _rbFilterAnd;
        private System.Windows.Forms.GroupBox _gbRuntimeFilter;
        private System.Windows.Forms.Label _lForcedSecurityLevel;
        private System.Windows.Forms.ComboBox _cbForcedSecurityLevel;
        private System.Windows.Forms.Label _lEmergencyCodeFilter;
        private System.Windows.Forms.ComboBox _cbEmergencyCode;
        private System.Windows.Forms.Label _lCurrentSecurityLevelFilter;
        private System.Windows.Forms.CheckedListBox _clbCurrentSecurityLevel;
        private System.Windows.Forms.ComboBox _cbCurrentForcedSecurityLevel;
        private System.Windows.Forms.Label _lCurrentForcedSecurityLevel;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove1;
        private System.Windows.Forms.Label _lRecordCount;
        private Contal.IwQuick.UI.TextBoxMenu _tbmMemberOfAclFilter;
        private System.Windows.Forms.CheckBox _chbBlocked;
        private System.Windows.Forms.Button _bUnblock;
        private System.Windows.Forms.Button bExportExcel;
    }
}
