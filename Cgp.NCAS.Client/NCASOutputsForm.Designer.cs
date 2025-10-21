namespace Contal.Cgp.NCAS.Client
{
    partial class NCASOutputsForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASOutputsForm));
            this._lNameFilter = new System.Windows.Forms.Label();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter = new System.Windows.Forms.Panel();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._gbRuntimeFilter = new System.Windows.Forms.GroupBox();
            this._cbCurrentlyControlledFilter = new System.Windows.Forms.ComboBox();
            this._lCurrentlyControlledFilter = new System.Windows.Forms.Label();
            this._lCurrentState = new System.Windows.Forms.Label();
            this._cbCurrentStateFilter = new System.Windows.Forms.ComboBox();
            this._clbOutputControlFilter = new System.Windows.Forms.CheckedListBox();
            this._rbFilterOr = new System.Windows.Forms.RadioButton();
            this._rbFilterAnd = new System.Windows.Forms.RadioButton();
            this._tbmControlledByFilter = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tbmPresentationGroupFilter = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify3 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove3 = new System.Windows.Forms.ToolStripMenuItem();
            this._cbEnableAlarmsFilter = new System.Windows.Forms.ComboBox();
            this._cbInvertedFilter = new System.Windows.Forms.ComboBox();
            this._lEnableAlarmsFilter = new System.Windows.Forms.Label();
            this._cbOutputTypeFilter = new System.Windows.Forms.ComboBox();
            this._lInvertedFilter = new System.Windows.Forms.Label();
            this._lOutputControlFilter = new System.Windows.Forms.Label();
            this._lOutputTypeFilter = new System.Windows.Forms.Label();
            this._lControlledByFilter = new System.Windows.Forms.Label();
            this._lPresentationGroupFilter = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this._pFilter.SuspendLayout();
            this._gbRuntimeFilter.SuspendLayout();
            this.SuspendLayout();
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
            this._eNameFilter.Location = new System.Drawing.Point(6, 25);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Size = new System.Drawing.Size(119, 26);
            this._eNameFilter.TabIndex = 0;
            this._eNameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eNameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
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
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvData.CgpDataGridEvents = null;
            this._cdgvData.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvData.DataGrid.AllowUserToAddRows = false;
            this._cdgvData.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvData.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvData.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
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
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(980, 365);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(980, 365);
            this._cdgvData.TabIndex = 25;
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._gbRuntimeFilter);
            this._pFilter.Controls.Add(this._clbOutputControlFilter);
            this._pFilter.Controls.Add(this._rbFilterOr);
            this._pFilter.Controls.Add(this._rbFilterAnd);
            this._pFilter.Controls.Add(this._tbmControlledByFilter);
            this._pFilter.Controls.Add(this._tbmPresentationGroupFilter);
            this._pFilter.Controls.Add(this._cbEnableAlarmsFilter);
            this._pFilter.Controls.Add(this._cbInvertedFilter);
            this._pFilter.Controls.Add(this._lEnableAlarmsFilter);
            this._pFilter.Controls.Add(this._cbOutputTypeFilter);
            this._pFilter.Controls.Add(this._lInvertedFilter);
            this._pFilter.Controls.Add(this._lOutputControlFilter);
            this._pFilter.Controls.Add(this._lOutputTypeFilter);
            this._pFilter.Controls.Add(this._lControlledByFilter);
            this._pFilter.Controls.Add(this._lPresentationGroupFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 337);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(980, 140);
            this._pFilter.TabIndex = 26;
            // 
            // _lRecordCount
            // 
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(830, 86);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(109, 20);
            this._lRecordCount.TabIndex = 48;
            this._lRecordCount.Text = "Record count:";
            // 
            // _gbRuntimeFilter
            // 
            this._gbRuntimeFilter.Controls.Add(this._cbCurrentlyControlledFilter);
            this._gbRuntimeFilter.Controls.Add(this._lCurrentlyControlledFilter);
            this._gbRuntimeFilter.Controls.Add(this._lCurrentState);
            this._gbRuntimeFilter.Controls.Add(this._cbCurrentStateFilter);
            this._gbRuntimeFilter.Location = new System.Drawing.Point(701, 13);
            this._gbRuntimeFilter.Name = "_gbRuntimeFilter";
            this._gbRuntimeFilter.Size = new System.Drawing.Size(248, 70);
            this._gbRuntimeFilter.TabIndex = 45;
            this._gbRuntimeFilter.TabStop = false;
            this._gbRuntimeFilter.Tag = "RealtimeFiltering";
            this._gbRuntimeFilter.Text = "Runtime filters";
            // 
            // _cbCurrentlyControlledFilter
            // 
            this._cbCurrentlyControlledFilter.FormattingEnabled = true;
            this._cbCurrentlyControlledFilter.Items.AddRange(new object[] {
            "",
            "Off",
            "On"});
            this._cbCurrentlyControlledFilter.Location = new System.Drawing.Point(14, 38);
            this._cbCurrentlyControlledFilter.Name = "_cbCurrentlyControlledFilter";
            this._cbCurrentlyControlledFilter.Size = new System.Drawing.Size(99, 28);
            this._cbCurrentlyControlledFilter.TabIndex = 36;
            this._cbCurrentlyControlledFilter.Tag = new string[] {
        "",
        "Off",
        "On"};
            this._cbCurrentlyControlledFilter.SelectedIndexChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _lCurrentlyControlledFilter
            // 
            this._lCurrentlyControlledFilter.AutoSize = true;
            this._lCurrentlyControlledFilter.Location = new System.Drawing.Point(11, 18);
            this._lCurrentlyControlledFilter.Name = "_lCurrentlyControlledFilter";
            this._lCurrentlyControlledFilter.Size = new System.Drawing.Size(145, 20);
            this._lCurrentlyControlledFilter.TabIndex = 27;
            this._lCurrentlyControlledFilter.Text = "Currently controlled";
            // 
            // _lCurrentState
            // 
            this._lCurrentState.AutoSize = true;
            this._lCurrentState.Location = new System.Drawing.Point(118, 17);
            this._lCurrentState.Name = "_lCurrentState";
            this._lCurrentState.Size = new System.Drawing.Size(102, 20);
            this._lCurrentState.TabIndex = 29;
            this._lCurrentState.Text = "Current state";
            // 
            // _cbCurrentStateFilter
            // 
            this._cbCurrentStateFilter.FormattingEnabled = true;
            this._cbCurrentStateFilter.Items.AddRange(new object[] {
            "",
            "Off",
            "On",
            "UsedByAnotherAplication",
            "OutOfRange",
            "Unknown"});
            this._cbCurrentStateFilter.Location = new System.Drawing.Point(121, 38);
            this._cbCurrentStateFilter.Name = "_cbCurrentStateFilter";
            this._cbCurrentStateFilter.Size = new System.Drawing.Size(116, 28);
            this._cbCurrentStateFilter.TabIndex = 38;
            this._cbCurrentStateFilter.Tag = new string[] {
        "",
        "Off",
        "On",
        "UsedByAnotherAplication",
        "OutOfRange",
        "Unknown"};
            this._cbCurrentStateFilter.SelectedIndexChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _clbOutputControlFilter
            // 
            this._clbOutputControlFilter.FormattingEnabled = true;
            this._clbOutputControlFilter.Items.AddRange(new object[] {
            "Unblocked",
            "Manualy nlocked",
            "Forced On",
            "Controled by object",
            "Controled by door environment",
            "Watchdog"});
            this._clbOutputControlFilter.Location = new System.Drawing.Point(216, 25);
            this._clbOutputControlFilter.Name = "_clbOutputControlFilter";
            this._clbOutputControlFilter.Size = new System.Drawing.Size(220, 50);
            this._clbOutputControlFilter.TabIndex = 44;
            this._clbOutputControlFilter.Tag = new string[] {
        "OutputControlTypes_unblocked",
        "OutputControlTypes_manualBlocked",
        "OutputControlTypes_forcedOn",
        "OutputControlTypes_controledByObject",
        "OutputControlTypes_controledByDoorEnvironment",
        "OutputControlTypes_watchdog"};
            this._clbOutputControlFilter.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._clbOutputControlFilter_ItemCheck);
            // 
            // _rbFilterOr
            // 
            this._rbFilterOr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbFilterOr.AutoSize = true;
            this._rbFilterOr.Location = new System.Drawing.Point(7, 111);
            this._rbFilterOr.Name = "_rbFilterOr";
            this._rbFilterOr.Size = new System.Drawing.Size(376, 24);
            this._rbFilterOr.TabIndex = 43;
            this._rbFilterOr.Text = "Show outputs that match at least one parameter";
            this._rbFilterOr.UseVisualStyleBackColor = true;
            this._rbFilterOr.CheckedChanged += new System.EventHandler(this._rbFilterOr_CheckedChanged);
            // 
            // _rbFilterAnd
            // 
            this._rbFilterAnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._rbFilterAnd.AutoSize = true;
            this._rbFilterAnd.Checked = true;
            this._rbFilterAnd.Location = new System.Drawing.Point(440, 111);
            this._rbFilterAnd.Name = "_rbFilterAnd";
            this._rbFilterAnd.Size = new System.Drawing.Size(316, 24);
            this._rbFilterAnd.TabIndex = 42;
            this._rbFilterAnd.TabStop = true;
            this._rbFilterAnd.Text = "Show outputs that match all parameters";
            this._rbFilterAnd.UseVisualStyleBackColor = true;
            // 
            // _tbmControlledByFilter
            // 
            this._tbmControlledByFilter.AllowDrop = true;
            this._tbmControlledByFilter.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmControlledByFilter.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmControlledByFilter.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmControlledByFilter.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmControlledByFilter.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmControlledByFilter.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmControlledByFilter.Button.Image")));
            this._tbmControlledByFilter.Button.Location = new System.Drawing.Point(217, 0);
            this._tbmControlledByFilter.Button.Name = "_bMenu";
            this._tbmControlledByFilter.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmControlledByFilter.Button.TabIndex = 3;
            this._tbmControlledByFilter.Button.UseVisualStyleBackColor = false;
            this._tbmControlledByFilter.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmControlledByFilter.ButtonDefaultBehaviour = true;
            this._tbmControlledByFilter.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmControlledByFilter.ButtonImage = null;
            // 
            // 
            // 
            this._tbmControlledByFilter.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2});
            this._tbmControlledByFilter.ButtonPopupMenu.Name = "";
            this._tbmControlledByFilter.ButtonPopupMenu.Size = new System.Drawing.Size(149, 68);
            this._tbmControlledByFilter.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmControlledByFilter.ButtonShowImage = true;
            this._tbmControlledByFilter.ButtonSizeHeight = 20;
            this._tbmControlledByFilter.ButtonSizeWidth = 20;
            this._tbmControlledByFilter.ButtonText = "";
            this._tbmControlledByFilter.HoverTime = 500;
            // 
            // 
            // 
            this._tbmControlledByFilter.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmControlledByFilter.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmControlledByFilter.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmControlledByFilter.ImageTextBox.ContextMenuStrip = this._tbmControlledByFilter.ButtonPopupMenu;
            this._tbmControlledByFilter.ImageTextBox.Image = null;
            this._tbmControlledByFilter.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmControlledByFilter.ImageTextBox.Name = "_textBox";
            this._tbmControlledByFilter.ImageTextBox.NoTextNoImage = true;
            this._tbmControlledByFilter.ImageTextBox.ReadOnly = true;
            this._tbmControlledByFilter.ImageTextBox.Size = new System.Drawing.Size(217, 20);
            this._tbmControlledByFilter.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmControlledByFilter.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmControlledByFilter.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmControlledByFilter.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmControlledByFilter.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmControlledByFilter.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmControlledByFilter.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmControlledByFilter.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmControlledByFilter.ImageTextBox.TextBox.Size = new System.Drawing.Size(215, 19);
            this._tbmControlledByFilter.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmControlledByFilter.ImageTextBox.UseImage = true;
            this._tbmControlledByFilter.Location = new System.Drawing.Point(442, 80);
            this._tbmControlledByFilter.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmControlledByFilter.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmControlledByFilter.Name = "_tbmControlledByFilter";
            this._tbmControlledByFilter.Size = new System.Drawing.Size(237, 22);
            this._tbmControlledByFilter.TabIndex = 40;
            this._tbmControlledByFilter.TextImage = null;
            this._tbmControlledByFilter.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmControlledByFilter_ButtonPopupMenuItemClick);
            this._tbmControlledByFilter.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmControlledByFilter_DragDrop);
            this._tbmControlledByFilter.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmControlledByFilter_DragOver);
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
            this._tbmPresentationGroupFilter.Location = new System.Drawing.Point(442, 25);
            this._tbmPresentationGroupFilter.MaximumSize = new System.Drawing.Size(1200, 22);
            this._tbmPresentationGroupFilter.MinimumSize = new System.Drawing.Size(21, 22);
            this._tbmPresentationGroupFilter.Name = "_tbmPresentationGroupFilter";
            this._tbmPresentationGroupFilter.Size = new System.Drawing.Size(237, 22);
            this._tbmPresentationGroupFilter.TabIndex = 41;
            this._tbmPresentationGroupFilter.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmPresentationGroupFilter.TextImage")));
            this._tbmPresentationGroupFilter.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmPresentationGroupFilter_ButtonPopupMenuItemClick);
            this._tbmPresentationGroupFilter.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmPresentationGroupFilter_DragDrop);
            this._tbmPresentationGroupFilter.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmPresentationGroupFilter_DragOver);
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
            this._cbEnableAlarmsFilter.Location = new System.Drawing.Point(7, 80);
            this._cbEnableAlarmsFilter.Name = "_cbEnableAlarmsFilter";
            this._cbEnableAlarmsFilter.Size = new System.Drawing.Size(119, 28);
            this._cbEnableAlarmsFilter.TabIndex = 35;
            this._cbEnableAlarmsFilter.Tag = new string[] {
        "",
        "checked",
        "unchecked"};
            this._cbEnableAlarmsFilter.SelectedIndexChanged += new System.EventHandler(this.FilterValueChanged);
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
            this._cbInvertedFilter.TabIndex = 37;
            this._cbInvertedFilter.Tag = new string[] {
        "",
        "true",
        "false"};
            this._cbInvertedFilter.SelectedIndexChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _lEnableAlarmsFilter
            // 
            this._lEnableAlarmsFilter.AutoSize = true;
            this._lEnableAlarmsFilter.Location = new System.Drawing.Point(4, 55);
            this._lEnableAlarmsFilter.Name = "_lEnableAlarmsFilter";
            this._lEnableAlarmsFilter.Size = new System.Drawing.Size(112, 20);
            this._lEnableAlarmsFilter.TabIndex = 28;
            this._lEnableAlarmsFilter.Text = "Enable Alarms";
            // 
            // _cbOutputTypeFilter
            // 
            this._cbOutputTypeFilter.FormattingEnabled = true;
            this._cbOutputTypeFilter.Items.AddRange(new object[] {
            "",
            "Level",
            "Pulsed",
            "Frequency"});
            this._cbOutputTypeFilter.Location = new System.Drawing.Point(131, 25);
            this._cbOutputTypeFilter.Name = "_cbOutputTypeFilter";
            this._cbOutputTypeFilter.Size = new System.Drawing.Size(79, 28);
            this._cbOutputTypeFilter.TabIndex = 34;
            this._cbOutputTypeFilter.Tag = new string[] {
        "",
        "OutputTypes_level",
        "OutputTypes_pulsed",
        "OutputTypes_frequency"};
            this._cbOutputTypeFilter.SelectedIndexChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _lInvertedFilter
            // 
            this._lInvertedFilter.AutoSize = true;
            this._lInvertedFilter.Location = new System.Drawing.Point(128, 55);
            this._lInvertedFilter.Name = "_lInvertedFilter";
            this._lInvertedFilter.Size = new System.Drawing.Size(67, 20);
            this._lInvertedFilter.TabIndex = 26;
            this._lInvertedFilter.Text = "Inverted";
            // 
            // _lOutputControlFilter
            // 
            this._lOutputControlFilter.AutoSize = true;
            this._lOutputControlFilter.Location = new System.Drawing.Point(213, 4);
            this._lOutputControlFilter.Name = "_lOutputControlFilter";
            this._lOutputControlFilter.Size = new System.Drawing.Size(110, 20);
            this._lOutputControlFilter.TabIndex = 32;
            this._lOutputControlFilter.Text = "Output control";
            // 
            // _lOutputTypeFilter
            // 
            this._lOutputTypeFilter.AutoSize = true;
            this._lOutputTypeFilter.Location = new System.Drawing.Point(128, 4);
            this._lOutputTypeFilter.Name = "_lOutputTypeFilter";
            this._lOutputTypeFilter.Size = new System.Drawing.Size(96, 20);
            this._lOutputTypeFilter.TabIndex = 33;
            this._lOutputTypeFilter.Text = "Output Type";
            // 
            // _lControlledByFilter
            // 
            this._lControlledByFilter.AutoSize = true;
            this._lControlledByFilter.Location = new System.Drawing.Point(439, 55);
            this._lControlledByFilter.Name = "_lControlledByFilter";
            this._lControlledByFilter.Size = new System.Drawing.Size(101, 20);
            this._lControlledByFilter.TabIndex = 30;
            this._lControlledByFilter.Text = "Controlled by";
            // 
            // _lPresentationGroupFilter
            // 
            this._lPresentationGroupFilter.AutoSize = true;
            this._lPresentationGroupFilter.Location = new System.Drawing.Point(439, 4);
            this._lPresentationGroupFilter.Name = "_lPresentationGroupFilter";
            this._lPresentationGroupFilter.Size = new System.Drawing.Size(144, 20);
            this._lPresentationGroupFilter.TabIndex = 31;
            this._lPresentationGroupFilter.Text = "Presentation group";
            // 
            // NCASOutputsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(980, 477);
            this.Controls.Add(this._pFilter);
            this.Controls.Add(this._cdgvData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NCASOutputsForm";
            this.Text = "NCASOutputsForm";
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this._gbRuntimeFilter.ResumeLayout(false);
            this._gbRuntimeFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify3;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove3;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.RadioButton _rbFilterOr;
        private System.Windows.Forms.RadioButton _rbFilterAnd;
        private Contal.IwQuick.UI.TextBoxMenu _tbmControlledByFilter;
        private Contal.IwQuick.UI.TextBoxMenu _tbmPresentationGroupFilter;
        private System.Windows.Forms.ComboBox _cbCurrentlyControlledFilter;
        private System.Windows.Forms.ComboBox _cbEnableAlarmsFilter;
        private System.Windows.Forms.ComboBox _cbCurrentStateFilter;
        private System.Windows.Forms.ComboBox _cbInvertedFilter;
        private System.Windows.Forms.Label _lEnableAlarmsFilter;
        private System.Windows.Forms.ComboBox _cbOutputTypeFilter;
        private System.Windows.Forms.Label _lCurrentState;
        private System.Windows.Forms.Label _lInvertedFilter;
        private System.Windows.Forms.Label _lCurrentlyControlledFilter;
        private System.Windows.Forms.Label _lOutputControlFilter;
        private System.Windows.Forms.Label _lOutputTypeFilter;
        private System.Windows.Forms.Label _lControlledByFilter;
        private System.Windows.Forms.Label _lPresentationGroupFilter;
        private System.Windows.Forms.CheckedListBox _clbOutputControlFilter;
        private System.Windows.Forms.GroupBox _gbRuntimeFilter;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
