namespace Contal.Cgp.NCAS.Client
{
    partial class NCASInputsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASInputsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this._pFilter = new System.Windows.Forms.Panel();
            this._gbRuntimeFilter = new System.Windows.Forms.GroupBox();
            this._cbCurrentlyBlockedFilter = new System.Windows.Forms.ComboBox();
            this._lCurrentState = new System.Windows.Forms.Label();
            this._cbCurrentStateFilter = new System.Windows.Forms.ComboBox();
            this._lCurrentlyBlockedFilter = new System.Windows.Forms.Label();
            this._clbInputControlFilter = new System.Windows.Forms.CheckedListBox();
            this._rbFilterOr = new System.Windows.Forms.RadioButton();
            this._rbFilterAnd = new System.Windows.Forms.RadioButton();
            this._tbmBlockedByFilter = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tbmPresentationGroupFilter = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify3 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove3 = new System.Windows.Forms.ToolStripMenuItem();
            this._cbEnableAlarmsFilter = new System.Windows.Forms.ComboBox();
            this._cbInvertedFilter = new System.Windows.Forms.ComboBox();
            this._lEnableAlarmsFilter = new System.Windows.Forms.Label();
            this._cbInputTypeFilter = new System.Windows.Forms.ComboBox();
            this._lInvertedFilter = new System.Windows.Forms.Label();
            this._lInputControlFilter = new System.Windows.Forms.Label();
            this._lInputTypeFilter = new System.Windows.Forms.Label();
            this._lBlockedByFilter = new System.Windows.Forms.Label();
            this._lPresentationGroupFilter = new System.Windows.Forms.Label();
            this._lNameFilter = new System.Windows.Forms.Label();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter.SuspendLayout();
            this._gbRuntimeFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._gbRuntimeFilter);
            this._pFilter.Controls.Add(this._clbInputControlFilter);
            this._pFilter.Controls.Add(this._rbFilterOr);
            this._pFilter.Controls.Add(this._rbFilterAnd);
            this._pFilter.Controls.Add(this._tbmBlockedByFilter);
            this._pFilter.Controls.Add(this._tbmPresentationGroupFilter);
            this._pFilter.Controls.Add(this._cbEnableAlarmsFilter);
            this._pFilter.Controls.Add(this._cbInvertedFilter);
            this._pFilter.Controls.Add(this._lEnableAlarmsFilter);
            this._pFilter.Controls.Add(this._cbInputTypeFilter);
            this._pFilter.Controls.Add(this._lInvertedFilter);
            this._pFilter.Controls.Add(this._lInputControlFilter);
            this._pFilter.Controls.Add(this._lInputTypeFilter);
            this._pFilter.Controls.Add(this._lBlockedByFilter);
            this._pFilter.Controls.Add(this._lPresentationGroupFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 337);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(980, 140);
            this._pFilter.TabIndex = 22;
            // 
            // _gbRuntimeFilter
            // 
            this._gbRuntimeFilter.Controls.Add(this._cbCurrentlyBlockedFilter);
            this._gbRuntimeFilter.Controls.Add(this._lCurrentState);
            this._gbRuntimeFilter.Controls.Add(this._cbCurrentStateFilter);
            this._gbRuntimeFilter.Controls.Add(this._lCurrentlyBlockedFilter);
            this._gbRuntimeFilter.Location = new System.Drawing.Point(704, 10);
            this._gbRuntimeFilter.Name = "_gbRuntimeFilter";
            this._gbRuntimeFilter.Size = new System.Drawing.Size(248, 73);
            this._gbRuntimeFilter.TabIndex = 46;
            this._gbRuntimeFilter.TabStop = false;
            this._gbRuntimeFilter.Tag = "RealtimeFiltering";
            this._gbRuntimeFilter.Text = "Runtime filters";
            // 
            // _cbCurrentlyBlockedFilter
            // 
            this._cbCurrentlyBlockedFilter.FormattingEnabled = true;
            this._cbCurrentlyBlockedFilter.Items.AddRange(new object[] {
            "",
            "Not blocked",
            "Blocked"});
            this._cbCurrentlyBlockedFilter.Location = new System.Drawing.Point(14, 40);
            this._cbCurrentlyBlockedFilter.Name = "_cbCurrentlyBlockedFilter";
            this._cbCurrentlyBlockedFilter.Size = new System.Drawing.Size(99, 28);
            this._cbCurrentlyBlockedFilter.TabIndex = 22;
            this._cbCurrentlyBlockedFilter.Tag = new string[] {
        "",
        "BlockingType_NotBlocked",
        "BlockingType_Blocked"};
            this._cbCurrentlyBlockedFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress_KeyPress);
            // 
            // _lCurrentState
            // 
            this._lCurrentState.AutoSize = true;
            this._lCurrentState.Location = new System.Drawing.Point(118, 17);
            this._lCurrentState.Name = "_lCurrentState";
            this._lCurrentState.Size = new System.Drawing.Size(102, 20);
            this._lCurrentState.TabIndex = 20;
            this._lCurrentState.Text = "Current state";
            // 
            // _cbCurrentStateFilter
            // 
            this._cbCurrentStateFilter.FormattingEnabled = true;
            this._cbCurrentStateFilter.Items.AddRange(new object[] {
            "",
            "Normal",
            "Alarm",
            "Short",
            "Break",
            "UsedByAnotherAplication",
            "OutOfRange",
            "Unknown"});
            this._cbCurrentStateFilter.Location = new System.Drawing.Point(121, 40);
            this._cbCurrentStateFilter.Name = "_cbCurrentStateFilter";
            this._cbCurrentStateFilter.Size = new System.Drawing.Size(116, 28);
            this._cbCurrentStateFilter.TabIndex = 22;
            this._cbCurrentStateFilter.Tag = new string[] {
        "",
        "InputNormal",
        "InputAlarm",
        "InputShort",
        "InputBreak",
        "UsedByAnotherAplication",
        "OutOfRange",
        "State_Unknown"};
            this._cbCurrentStateFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress_KeyPress);
            // 
            // _lCurrentlyBlockedFilter
            // 
            this._lCurrentlyBlockedFilter.AutoSize = true;
            this._lCurrentlyBlockedFilter.Location = new System.Drawing.Point(11, 18);
            this._lCurrentlyBlockedFilter.Name = "_lCurrentlyBlockedFilter";
            this._lCurrentlyBlockedFilter.Size = new System.Drawing.Size(131, 20);
            this._lCurrentlyBlockedFilter.TabIndex = 20;
            this._lCurrentlyBlockedFilter.Text = "Currently blocked";
            // 
            // _clbInputControlFilter
            // 
            this._clbInputControlFilter.FormattingEnabled = true;
            this._clbInputControlFilter.IntegralHeight = false;
            this._clbInputControlFilter.Items.AddRange(new object[] {
            "Not blocked",
            "Forcefully blocked",
            "Forcefully set",
            "Blocked by object"});
            this._clbInputControlFilter.Location = new System.Drawing.Point(216, 28);
            this._clbInputControlFilter.Name = "_clbInputControlFilter";
            this._clbInputControlFilter.Size = new System.Drawing.Size(217, 77);
            this._clbInputControlFilter.TabIndex = 26;
            this._clbInputControlFilter.Tag = new string[] {
        "BlockingType_NotBlocked",
        "BlockingType_ForcefullyBlocked",
        "BlockingType_ForcefullySet",
        "BlockingType_BlockedByObject"};
            this._clbInputControlFilter.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._clbInputControlFilter_ItemCheck);
            // 
            // _rbFilterOr
            // 
            this._rbFilterOr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbFilterOr.AutoSize = true;
            this._rbFilterOr.Location = new System.Drawing.Point(7, 110);
            this._rbFilterOr.Name = "_rbFilterOr";
            this._rbFilterOr.Size = new System.Drawing.Size(365, 24);
            this._rbFilterOr.TabIndex = 25;
            this._rbFilterOr.Text = "Show inputs that match at least one parameter";
            this._rbFilterOr.UseVisualStyleBackColor = true;
            this._rbFilterOr.CheckedChanged += new System.EventHandler(this._rbFilterOr_CheckedChanged);
            // 
            // _rbFilterAnd
            // 
            this._rbFilterAnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbFilterAnd.AutoSize = true;
            this._rbFilterAnd.Checked = true;
            this._rbFilterAnd.Location = new System.Drawing.Point(379, 110);
            this._rbFilterAnd.Name = "_rbFilterAnd";
            this._rbFilterAnd.Size = new System.Drawing.Size(305, 24);
            this._rbFilterAnd.TabIndex = 24;
            this._rbFilterAnd.TabStop = true;
            this._rbFilterAnd.Text = "Show inputs that match all parameters";
            this._rbFilterAnd.UseVisualStyleBackColor = true;
            // 
            // _tbmBlockedByFilter
            // 
            this._tbmBlockedByFilter.AllowDrop = true;
            this._tbmBlockedByFilter.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmBlockedByFilter.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmBlockedByFilter.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmBlockedByFilter.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmBlockedByFilter.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmBlockedByFilter.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmBlockedByFilter.Button.Image")));
            this._tbmBlockedByFilter.Button.Location = new System.Drawing.Point(217, 0);
            this._tbmBlockedByFilter.Button.Name = "_bMenu";
            this._tbmBlockedByFilter.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmBlockedByFilter.Button.TabIndex = 3;
            this._tbmBlockedByFilter.Button.UseVisualStyleBackColor = false;
            this._tbmBlockedByFilter.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmBlockedByFilter.ButtonDefaultBehaviour = true;
            this._tbmBlockedByFilter.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmBlockedByFilter.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmBlockedByFilter.ButtonImage")));
            // 
            // 
            // 
            this._tbmBlockedByFilter.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2});
            this._tbmBlockedByFilter.ButtonPopupMenu.Name = "";
            this._tbmBlockedByFilter.ButtonPopupMenu.Size = new System.Drawing.Size(149, 68);
            this._tbmBlockedByFilter.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmBlockedByFilter.ButtonShowImage = true;
            this._tbmBlockedByFilter.ButtonSizeHeight = 20;
            this._tbmBlockedByFilter.ButtonSizeWidth = 20;
            this._tbmBlockedByFilter.ButtonText = "";
            this._tbmBlockedByFilter.HoverTime = 500;
            // 
            // 
            // 
            this._tbmBlockedByFilter.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmBlockedByFilter.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmBlockedByFilter.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmBlockedByFilter.ImageTextBox.ContextMenuStrip = this._tbmBlockedByFilter.ButtonPopupMenu;
            this._tbmBlockedByFilter.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmBlockedByFilter.ImageTextBox.Image")));
            this._tbmBlockedByFilter.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmBlockedByFilter.ImageTextBox.Name = "_textBox";
            this._tbmBlockedByFilter.ImageTextBox.NoTextNoImage = true;
            this._tbmBlockedByFilter.ImageTextBox.ReadOnly = true;
            this._tbmBlockedByFilter.ImageTextBox.Size = new System.Drawing.Size(217, 20);
            this._tbmBlockedByFilter.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmBlockedByFilter.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmBlockedByFilter.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmBlockedByFilter.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmBlockedByFilter.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmBlockedByFilter.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmBlockedByFilter.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmBlockedByFilter.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmBlockedByFilter.ImageTextBox.TextBox.Size = new System.Drawing.Size(215, 19);
            this._tbmBlockedByFilter.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmBlockedByFilter.ImageTextBox.UseImage = true;
            this._tbmBlockedByFilter.Location = new System.Drawing.Point(442, 79);
            this._tbmBlockedByFilter.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmBlockedByFilter.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmBlockedByFilter.Name = "_tbmBlockedByFilter";
            this._tbmBlockedByFilter.Size = new System.Drawing.Size(237, 22);
            this._tbmBlockedByFilter.TabIndex = 23;
            this._tbmBlockedByFilter.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmBlockedByFilter.TextImage")));
            this._tbmBlockedByFilter.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmBlockedBy_ButtonPopupMenuItemClick);
            this._tbmBlockedByFilter.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmBlockedBy_DragDrop);
            this._tbmBlockedByFilter.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmBlockedBy_DragOver);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(148, 32);
            this._tsiModify2.Text = "Modify";
            // 
            // _tsiRemove2
            // 
            this._tsiRemove2.Name = "_tsiRemove2";
            this._tsiRemove2.Size = new System.Drawing.Size(148, 32);
            this._tsiRemove2.Text = "Remove";
            // 
            // _tbmPresentationGroupFilter
            // 
            this._tbmPresentationGroupFilter.AllowDrop = true;
            this._tbmPresentationGroupFilter.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmPresentationGroupFilter.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPresentationGroupFilter.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmPresentationGroupFilter.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPresentationGroupFilter.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmPresentationGroupFilter.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroupFilter.Button.Image")));
            this._tbmPresentationGroupFilter.Button.Location = new System.Drawing.Point(217, 0);
            this._tbmPresentationGroupFilter.Button.Name = "_bMenu";
            this._tbmPresentationGroupFilter.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmPresentationGroupFilter.Button.TabIndex = 3;
            this._tbmPresentationGroupFilter.Button.UseVisualStyleBackColor = false;
            this._tbmPresentationGroupFilter.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmPresentationGroupFilter.ButtonDefaultBehaviour = true;
            this._tbmPresentationGroupFilter.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmPresentationGroupFilter.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroupFilter.ButtonImage")));
            // 
            // 
            // 
            this._tbmPresentationGroupFilter.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify3,
            this._tsiRemove3});
            this._tbmPresentationGroupFilter.ButtonPopupMenu.Name = "";
            this._tbmPresentationGroupFilter.ButtonPopupMenu.Size = new System.Drawing.Size(149, 68);
            this._tbmPresentationGroupFilter.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmPresentationGroupFilter.ButtonShowImage = true;
            this._tbmPresentationGroupFilter.ButtonSizeHeight = 20;
            this._tbmPresentationGroupFilter.ButtonSizeWidth = 20;
            this._tbmPresentationGroupFilter.ButtonText = "";
            this._tbmPresentationGroupFilter.HoverTime = 500;
            // 
            // 
            // 
            this._tbmPresentationGroupFilter.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPresentationGroupFilter.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmPresentationGroupFilter.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmPresentationGroupFilter.ImageTextBox.ContextMenuStrip = this._tbmPresentationGroupFilter.ButtonPopupMenu;
            this._tbmPresentationGroupFilter.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroupFilter.ImageTextBox.Image")));
            this._tbmPresentationGroupFilter.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmPresentationGroupFilter.ImageTextBox.Name = "_textBox";
            this._tbmPresentationGroupFilter.ImageTextBox.NoTextNoImage = true;
            this._tbmPresentationGroupFilter.ImageTextBox.ReadOnly = true;
            this._tbmPresentationGroupFilter.ImageTextBox.Size = new System.Drawing.Size(217, 20);
            this._tbmPresentationGroupFilter.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.Size = new System.Drawing.Size(215, 19);
            this._tbmPresentationGroupFilter.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmPresentationGroupFilter.ImageTextBox.UseImage = true;
            this._tbmPresentationGroupFilter.Location = new System.Drawing.Point(442, 29);
            this._tbmPresentationGroupFilter.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmPresentationGroupFilter.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmPresentationGroupFilter.Name = "_tbmPresentationGroupFilter";
            this._tbmPresentationGroupFilter.Size = new System.Drawing.Size(237, 22);
            this._tbmPresentationGroupFilter.TabIndex = 23;
            this._tbmPresentationGroupFilter.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroupFilter.TextImage")));
            this._tbmPresentationGroupFilter.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmPresentationGroup_ButtonPopupMenuItemClick);
            this._tbmPresentationGroupFilter.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmPresentationGroup_DragDrop);
            this._tbmPresentationGroupFilter.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmPresentationGroup_DragOver);
            // 
            // _tsiModify3
            // 
            this._tsiModify3.Name = "_tsiModify3";
            this._tsiModify3.Size = new System.Drawing.Size(148, 32);
            this._tsiModify3.Text = "Modify";
            // 
            // _tsiRemove3
            // 
            this._tsiRemove3.Name = "_tsiRemove3";
            this._tsiRemove3.Size = new System.Drawing.Size(148, 32);
            this._tsiRemove3.Text = "Remove";
            // 
            // _cbEnableAlarmsFilter
            // 
            this._cbEnableAlarmsFilter.FormattingEnabled = true;
            this._cbEnableAlarmsFilter.Items.AddRange(new object[] {
            "",
            "checked",
            "unchecked"});
            this._cbEnableAlarmsFilter.Location = new System.Drawing.Point(8, 80);
            this._cbEnableAlarmsFilter.Name = "_cbEnableAlarmsFilter";
            this._cbEnableAlarmsFilter.Size = new System.Drawing.Size(119, 28);
            this._cbEnableAlarmsFilter.TabIndex = 22;
            this._cbEnableAlarmsFilter.Tag = new string[] {
        "",
        "checked",
        "unchecked"};
            this._cbEnableAlarmsFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress_KeyPress);
            // 
            // _cbInvertedFilter
            // 
            this._cbInvertedFilter.FormattingEnabled = true;
            this._cbInvertedFilter.Items.AddRange(new object[] {
            "",
            "true",
            "false"});
            this._cbInvertedFilter.Location = new System.Drawing.Point(131, 80);
            this._cbInvertedFilter.Name = "_cbInvertedFilter";
            this._cbInvertedFilter.Size = new System.Drawing.Size(79, 28);
            this._cbInvertedFilter.TabIndex = 22;
            this._cbInvertedFilter.Tag = new string[] {
        "",
        "true",
        "false"};
            this._cbInvertedFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress_KeyPress);
            // 
            // _lEnableAlarmsFilter
            // 
            this._lEnableAlarmsFilter.AutoSize = true;
            this._lEnableAlarmsFilter.Location = new System.Drawing.Point(4, 54);
            this._lEnableAlarmsFilter.Name = "_lEnableAlarmsFilter";
            this._lEnableAlarmsFilter.Size = new System.Drawing.Size(112, 20);
            this._lEnableAlarmsFilter.TabIndex = 20;
            this._lEnableAlarmsFilter.Text = "Enable Alarms";
            // 
            // _cbInputTypeFilter
            // 
            this._cbInputTypeFilter.FormattingEnabled = true;
            this._cbInputTypeFilter.Items.AddRange(new object[] {
            "",
            "DI",
            "BSI"});
            this._cbInputTypeFilter.Location = new System.Drawing.Point(131, 26);
            this._cbInputTypeFilter.Name = "_cbInputTypeFilter";
            this._cbInputTypeFilter.Size = new System.Drawing.Size(79, 28);
            this._cbInputTypeFilter.TabIndex = 22;
            this._cbInputTypeFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DisableKeyPress_KeyPress);
            // 
            // _lInvertedFilter
            // 
            this._lInvertedFilter.AutoSize = true;
            this._lInvertedFilter.Location = new System.Drawing.Point(128, 54);
            this._lInvertedFilter.Name = "_lInvertedFilter";
            this._lInvertedFilter.Size = new System.Drawing.Size(67, 20);
            this._lInvertedFilter.TabIndex = 20;
            this._lInvertedFilter.Text = "Inverted";
            // 
            // _lInputControlFilter
            // 
            this._lInputControlFilter.AutoSize = true;
            this._lInputControlFilter.Location = new System.Drawing.Point(213, 4);
            this._lInputControlFilter.Name = "_lInputControlFilter";
            this._lInputControlFilter.Size = new System.Drawing.Size(98, 20);
            this._lInputControlFilter.TabIndex = 20;
            this._lInputControlFilter.Text = "Input control";
            // 
            // _lInputTypeFilter
            // 
            this._lInputTypeFilter.AutoSize = true;
            this._lInputTypeFilter.Location = new System.Drawing.Point(128, 4);
            this._lInputTypeFilter.Name = "_lInputTypeFilter";
            this._lInputTypeFilter.Size = new System.Drawing.Size(84, 20);
            this._lInputTypeFilter.TabIndex = 20;
            this._lInputTypeFilter.Text = "Input Type";
            // 
            // _lBlockedByFilter
            // 
            this._lBlockedByFilter.AutoSize = true;
            this._lBlockedByFilter.Location = new System.Drawing.Point(439, 54);
            this._lBlockedByFilter.Name = "_lBlockedByFilter";
            this._lBlockedByFilter.Size = new System.Drawing.Size(86, 20);
            this._lBlockedByFilter.TabIndex = 20;
            this._lBlockedByFilter.Text = "Blocked by";
            // 
            // _lPresentationGroupFilter
            // 
            this._lPresentationGroupFilter.AutoSize = true;
            this._lPresentationGroupFilter.Location = new System.Drawing.Point(439, 4);
            this._lPresentationGroupFilter.Name = "_lPresentationGroupFilter";
            this._lPresentationGroupFilter.Size = new System.Drawing.Size(144, 20);
            this._lPresentationGroupFilter.TabIndex = 20;
            this._lPresentationGroupFilter.Text = "Presentation group";
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Location = new System.Drawing.Point(3, 3);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Size = new System.Drawing.Size(51, 20);
            this._lNameFilter.TabIndex = 20;
            this._lNameFilter.Text = "Name";
            // 
            // _eNameFilter
            // 
            this._eNameFilter.Location = new System.Drawing.Point(6, 24);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Size = new System.Drawing.Size(119, 26);
            this._eNameFilter.TabIndex = 0;
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bFilterClear.Location = new System.Drawing.Point(902, 109);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(75, 32);
            this._bFilterClear.TabIndex = 2;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRunFilter.Location = new System.Drawing.Point(821, 109);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(75, 32);
            this._bRunFilter.TabIndex = 1;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _lRecordCount
            // 
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(819, 86);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(109, 20);
            this._lRecordCount.TabIndex = 47;
            this._lRecordCount.Text = "Record count:";
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
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvData.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this._cdgvData.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._cdgvData.DataGrid.ColumnHeadersHeight = 34;
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidth = 62;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(980, 337);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(980, 337);
            this._cdgvData.TabIndex = 23;
            // 
            // NCASInputsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(980, 477);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NCASInputsForm";
            this.Text = "NCASInputsForm";
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this._gbRuntimeFilter.ResumeLayout(false);
            this._gbRuntimeFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify3;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove3;
        private System.Windows.Forms.Panel _pFilter;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.ComboBox _cbInputTypeFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lInputTypeFilter;
        private Contal.IwQuick.UI.TextBoxMenu _tbmPresentationGroupFilter;
        private System.Windows.Forms.Label _lPresentationGroupFilter;
        private Contal.IwQuick.UI.TextBoxMenu _tbmBlockedByFilter;
        private System.Windows.Forms.Label _lInputControlFilter;
        private System.Windows.Forms.Label _lBlockedByFilter;
        private System.Windows.Forms.ComboBox _cbInvertedFilter;
        private System.Windows.Forms.Label _lInvertedFilter;
        private System.Windows.Forms.ComboBox _cbEnableAlarmsFilter;
        private System.Windows.Forms.Label _lEnableAlarmsFilter;
        private System.Windows.Forms.ComboBox _cbCurrentlyBlockedFilter;
        private System.Windows.Forms.Label _lCurrentlyBlockedFilter;
        private System.Windows.Forms.RadioButton _rbFilterOr;
        private System.Windows.Forms.RadioButton _rbFilterAnd;
        private System.Windows.Forms.ComboBox _cbCurrentStateFilter;
        private System.Windows.Forms.Label _lCurrentState;
        private System.Windows.Forms.CheckedListBox _clbInputControlFilter;
        private System.Windows.Forms.GroupBox _gbRuntimeFilter;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
