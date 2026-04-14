namespace Contal.Cgp.Client
{
    partial class CalendarEditForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalendarEditForm));
            this._tcDateSettingsStatus = new System.Windows.Forms.TabControl();
            this._tpCalendarSettings = new System.Windows.Forms.TabPage();
            this._gbCallendarSettings = new System.Windows.Forms.GroupBox();
            this._cbYear = new System.Windows.Forms.ComboBox();
            this._cbDay = new System.Windows.Forms.ComboBox();
            this._bCalendar1 = new System.Windows.Forms.Button();
            this._bCreateDayType = new System.Windows.Forms.Button();
            this._cbDayType = new System.Windows.Forms.ComboBox();
            this._lDayType = new System.Windows.Forms.Label();
            this._lYear = new System.Windows.Forms.Label();
            this._lDay = new System.Windows.Forms.Label();
            this._cbWeek = new System.Windows.Forms.ComboBox();
            this._lMonth = new System.Windows.Forms.Label();
            this._lWeek = new System.Windows.Forms.Label();
            this._lDescription = new System.Windows.Forms.Label();
            this._eDescriptionDateSettings = new System.Windows.Forms.TextBox();
            this._cbMonth = new System.Windows.Forms.ComboBox();
            this._pnlValues = new System.Windows.Forms.Panel();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._bCreateCalendarSetting = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._bCancelEdit = new System.Windows.Forms.Button();
            this._bUpdateDateSetting = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._tpDayTypeStatus = new System.Windows.Forms.TabPage();
            this._pnlDayTypeView = new System.Windows.Forms.Panel();
            this._cdgvDayTypeView = new Contal.Cgp.Components.CgpDataGridView();
            this._bCalendar = new System.Windows.Forms.Button();
            this._lDate = new System.Windows.Forms.Label();
            this._eDate = new System.Windows.Forms.TextBox();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._panelBack = new System.Windows.Forms.Panel();
            this._tcDateSettingsStatus.SuspendLayout();
            this._tpCalendarSettings.SuspendLayout();
            this._gbCallendarSettings.SuspendLayout();
            this._pnlValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this._tpDayTypeStatus.SuspendLayout();
            this._pnlDayTypeView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvDayTypeView.DataGrid)).BeginInit();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tcDateSettingsStatus
            // 
            this._tcDateSettingsStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tcDateSettingsStatus.Controls.Add(this._tpCalendarSettings);
            this._tcDateSettingsStatus.Controls.Add(this._tpDayTypeStatus);
            this._tcDateSettingsStatus.Controls.Add(this._tpUserFolders);
            this._tcDateSettingsStatus.Controls.Add(this._tpReferencedBy);
            this._tcDateSettingsStatus.Controls.Add(this._tpDescription);
            this._tcDateSettingsStatus.Location = new System.Drawing.Point(15, 48);
            this._tcDateSettingsStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tcDateSettingsStatus.Name = "_tcDateSettingsStatus";
            this._tcDateSettingsStatus.SelectedIndex = 0;
            this._tcDateSettingsStatus.Size = new System.Drawing.Size(954, 611);
            this._tcDateSettingsStatus.TabIndex = 1;
            this._tcDateSettingsStatus.TabStop = false;
            // 
            // _tpCalendarSettings
            // 
            this._tpCalendarSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tpCalendarSettings.Controls.Add(this._gbCallendarSettings);
            this._tpCalendarSettings.Controls.Add(this._pnlValues);
            this._tpCalendarSettings.Controls.Add(this._bCreateCalendarSetting);
            this._tpCalendarSettings.Controls.Add(this._bDelete);
            this._tpCalendarSettings.Controls.Add(this._bCancelEdit);
            this._tpCalendarSettings.Controls.Add(this._bUpdateDateSetting);
            this._tpCalendarSettings.Controls.Add(this._bEdit);
            this._tpCalendarSettings.Location = new System.Drawing.Point(4, 25);
            this._tpCalendarSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpCalendarSettings.Name = "_tpCalendarSettings";
            this._tpCalendarSettings.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpCalendarSettings.Size = new System.Drawing.Size(946, 582);
            this._tpCalendarSettings.TabIndex = 0;
            this._tpCalendarSettings.Text = "Calendar settings";
            // 
            // _gbCallendarSettings
            // 
            this._gbCallendarSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._gbCallendarSettings.Controls.Add(this._cbYear);
            this._gbCallendarSettings.Controls.Add(this._cbDay);
            this._gbCallendarSettings.Controls.Add(this._bCalendar1);
            this._gbCallendarSettings.Controls.Add(this._bCreateDayType);
            this._gbCallendarSettings.Controls.Add(this._cbDayType);
            this._gbCallendarSettings.Controls.Add(this._lDayType);
            this._gbCallendarSettings.Controls.Add(this._lYear);
            this._gbCallendarSettings.Controls.Add(this._lDay);
            this._gbCallendarSettings.Controls.Add(this._cbWeek);
            this._gbCallendarSettings.Controls.Add(this._lMonth);
            this._gbCallendarSettings.Controls.Add(this._lWeek);
            this._gbCallendarSettings.Controls.Add(this._lDescription);
            this._gbCallendarSettings.Controls.Add(this._eDescriptionDateSettings);
            this._gbCallendarSettings.Controls.Add(this._cbMonth);
            this._gbCallendarSettings.Location = new System.Drawing.Point(14, 8);
            this._gbCallendarSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._gbCallendarSettings.Name = "_gbCallendarSettings";
            this._gbCallendarSettings.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._gbCallendarSettings.Size = new System.Drawing.Size(736, 194);
            this._gbCallendarSettings.TabIndex = 0;
            this._gbCallendarSettings.TabStop = false;
            // 
            // _cbYear
            // 
            this._cbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbYear.FormattingEnabled = true;
            this._cbYear.Location = new System.Drawing.Point(8, 32);
            this._cbYear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cbYear.Name = "_cbYear";
            this._cbYear.Size = new System.Drawing.Size(112, 24);
            this._cbYear.TabIndex = 1;
            this._cbYear.SelectedIndexChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _cbDay
            // 
            this._cbDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbDay.FormattingEnabled = true;
            this._cbDay.Location = new System.Drawing.Point(411, 34);
            this._cbDay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cbDay.Name = "_cbDay";
            this._cbDay.Size = new System.Drawing.Size(112, 24);
            this._cbDay.TabIndex = 4;
            this._cbDay.SelectedIndexChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _bCalendar1
            // 
            this._bCalendar1.Location = new System.Drawing.Point(8, 68);
            this._bCalendar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCalendar1.Name = "_bCalendar1";
            this._bCalendar1.Size = new System.Drawing.Size(112, 32);
            this._bCalendar1.TabIndex = 6;
            this._bCalendar1.Text = "Calendar";
            this._bCalendar1.UseVisualStyleBackColor = true;
            this._bCalendar1.Click += new System.EventHandler(this._bCalendar1_Click);
            // 
            // _bCreateDayType
            // 
            this._bCreateDayType.Location = new System.Drawing.Point(531, 69);
            this._bCreateDayType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCreateDayType.Name = "_bCreateDayType";
            this._bCreateDayType.Size = new System.Drawing.Size(195, 32);
            this._bCreateDayType.TabIndex = 7;
            this._bCreateDayType.Text = "Create";
            this._bCreateDayType.UseVisualStyleBackColor = true;
            this._bCreateDayType.Click += new System.EventHandler(this._bCreate_Click);
            // 
            // _cbDayType
            // 
            this._cbDayType.AllowDrop = true;
            this._cbDayType.BackColor = System.Drawing.SystemColors.Window;
            this._cbDayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbDayType.FormattingEnabled = true;
            this._cbDayType.Location = new System.Drawing.Point(531, 34);
            this._cbDayType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cbDayType.Name = "_cbDayType";
            this._cbDayType.Size = new System.Drawing.Size(194, 24);
            this._cbDayType.TabIndex = 5;
            this._cbDayType.SelectedIndexChanged += new System.EventHandler(this._cbDayType_SelectedIndexChanged);
            this._cbDayType.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.comboBox1_MouseDoubleClick);
            // 
            // _lDayType
            // 
            this._lDayType.AutoSize = true;
            this._lDayType.Location = new System.Drawing.Point(535, 15);
            this._lDayType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDayType.Name = "_lDayType";
            this._lDayType.Size = new System.Drawing.Size(61, 16);
            this._lDayType.TabIndex = 8;
            this._lDayType.Text = "Day type";
            // 
            // _lYear
            // 
            this._lYear.AutoSize = true;
            this._lYear.Location = new System.Drawing.Point(4, 12);
            this._lYear.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lYear.Name = "_lYear";
            this._lYear.Size = new System.Drawing.Size(36, 16);
            this._lYear.TabIndex = 0;
            this._lYear.Text = "Year";
            // 
            // _lDay
            // 
            this._lDay.AutoSize = true;
            this._lDay.Location = new System.Drawing.Point(415, 14);
            this._lDay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDay.Name = "_lDay";
            this._lDay.Size = new System.Drawing.Size(32, 16);
            this._lDay.TabIndex = 6;
            this._lDay.Text = "Day";
            // 
            // _cbWeek
            // 
            this._cbWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbWeek.FormattingEnabled = true;
            this._cbWeek.Location = new System.Drawing.Point(248, 34);
            this._cbWeek.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cbWeek.Name = "_cbWeek";
            this._cbWeek.Size = new System.Drawing.Size(155, 24);
            this._cbWeek.TabIndex = 3;
            this._cbWeek.SelectedIndexChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lMonth
            // 
            this._lMonth.AutoSize = true;
            this._lMonth.Location = new System.Drawing.Point(131, 14);
            this._lMonth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMonth.Name = "_lMonth";
            this._lMonth.Size = new System.Drawing.Size(43, 16);
            this._lMonth.TabIndex = 2;
            this._lMonth.Text = "Month";
            // 
            // _lWeek
            // 
            this._lWeek.AutoSize = true;
            this._lWeek.Location = new System.Drawing.Point(251, 14);
            this._lWeek.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lWeek.Name = "_lWeek";
            this._lWeek.Size = new System.Drawing.Size(43, 16);
            this._lWeek.TabIndex = 4;
            this._lWeek.Text = "Week";
            // 
            // _lDescription
            // 
            this._lDescription.AutoSize = true;
            this._lDescription.Location = new System.Drawing.Point(15, 102);
            this._lDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDescription.Name = "_lDescription";
            this._lDescription.Size = new System.Drawing.Size(75, 16);
            this._lDescription.TabIndex = 12;
            this._lDescription.Text = "Description";
            // 
            // _eDescriptionDateSettings
            // 
            this._eDescriptionDateSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eDescriptionDateSettings.Location = new System.Drawing.Point(11, 122);
            this._eDescriptionDateSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDescriptionDateSettings.Multiline = true;
            this._eDescriptionDateSettings.Name = "_eDescriptionDateSettings";
            this._eDescriptionDateSettings.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescriptionDateSettings.Size = new System.Drawing.Size(714, 59);
            this._eDescriptionDateSettings.TabIndex = 8;
            this._eDescriptionDateSettings.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _cbMonth
            // 
            this._cbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbMonth.FormattingEnabled = true;
            this._cbMonth.Location = new System.Drawing.Point(128, 34);
            this._cbMonth.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cbMonth.Name = "_cbMonth";
            this._cbMonth.Size = new System.Drawing.Size(112, 24);
            this._cbMonth.TabIndex = 2;
            this._cbMonth.SelectedIndexChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _pnlValues
            // 
            this._pnlValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pnlValues.Controls.Add(this._cdgvData);
            this._pnlValues.Location = new System.Drawing.Point(8, 212);
            this._pnlValues.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._pnlValues.Name = "_pnlValues";
            this._pnlValues.Size = new System.Drawing.Size(929, 321);
            this._pnlValues.TabIndex = 92;
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
            this._cdgvData.DataGrid.ColumnHeadersHeight = 29;
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidth = 51;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(929, 321);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(929, 321);
            this._cdgvData.TabIndex = 0;
            // 
            // _bCreateCalendarSetting
            // 
            this._bCreateCalendarSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreateCalendarSetting.Location = new System.Drawing.Point(758, 40);
            this._bCreateCalendarSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCreateCalendarSetting.Name = "_bCreateCalendarSetting";
            this._bCreateCalendarSetting.Size = new System.Drawing.Size(179, 32);
            this._bCreateCalendarSetting.TabIndex = 1;
            this._bCreateCalendarSetting.Text = "Create date setting";
            this._bCreateCalendarSetting.UseVisualStyleBackColor = true;
            this._bCreateCalendarSetting.Click += new System.EventHandler(this._bCreateDateSetting_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(836, 544);
            this._bDelete.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(100, 32);
            this._bDelete.TabIndex = 12;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bCancelEdit
            // 
            this._bCancelEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancelEdit.Location = new System.Drawing.Point(758, 76);
            this._bCancelEdit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancelEdit.Name = "_bCancelEdit";
            this._bCancelEdit.Size = new System.Drawing.Size(179, 32);
            this._bCancelEdit.TabIndex = 2;
            this._bCancelEdit.Text = "Cancel edit";
            this._bCancelEdit.UseVisualStyleBackColor = true;
            this._bCancelEdit.Visible = false;
            this._bCancelEdit.Click += new System.EventHandler(this._bCancelEdit_Click);
            // 
            // _bUpdateDateSetting
            // 
            this._bUpdateDateSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bUpdateDateSetting.Location = new System.Drawing.Point(758, 41);
            this._bUpdateDateSetting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bUpdateDateSetting.Name = "_bUpdateDateSetting";
            this._bUpdateDateSetting.Size = new System.Drawing.Size(179, 28);
            this._bUpdateDateSetting.TabIndex = 83;
            this._bUpdateDateSetting.Text = "Update date setting";
            this._bUpdateDateSetting.UseVisualStyleBackColor = true;
            this._bUpdateDateSetting.Visible = false;
            this._bUpdateDateSetting.Click += new System.EventHandler(this._bUpdateDateSetting_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(729, 544);
            this._bEdit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(100, 32);
            this._bEdit.TabIndex = 11;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _tpDayTypeStatus
            // 
            this._tpDayTypeStatus.BackColor = System.Drawing.SystemColors.Control;
            this._tpDayTypeStatus.Controls.Add(this._pnlDayTypeView);
            this._tpDayTypeStatus.Controls.Add(this._bCalendar);
            this._tpDayTypeStatus.Controls.Add(this._lDate);
            this._tpDayTypeStatus.Controls.Add(this._eDate);
            this._tpDayTypeStatus.Location = new System.Drawing.Point(4, 25);
            this._tpDayTypeStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDayTypeStatus.Name = "_tpDayTypeStatus";
            this._tpDayTypeStatus.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDayTypeStatus.Size = new System.Drawing.Size(946, 582);
            this._tpDayTypeStatus.TabIndex = 2;
            this._tpDayTypeStatus.Text = "Day type status";
            // 
            // _pnlDayTypeView
            // 
            this._pnlDayTypeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pnlDayTypeView.Controls.Add(this._cdgvDayTypeView);
            this._pnlDayTypeView.Location = new System.Drawing.Point(5, 50);
            this._pnlDayTypeView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._pnlDayTypeView.Name = "_pnlDayTypeView";
            this._pnlDayTypeView.Size = new System.Drawing.Size(931, 519);
            this._pnlDayTypeView.TabIndex = 4;
            // 
            // _cdgvDayTypeView
            // 
            this._cdgvDayTypeView.AllwaysRefreshOrder = false;
            this._cdgvDayTypeView.CgpDataGridEvents = null;
            this._cdgvDayTypeView.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvDayTypeView.DataGrid.AllowUserToAddRows = false;
            this._cdgvDayTypeView.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvDayTypeView.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvDayTypeView.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this._cdgvDayTypeView.DataGrid.ColumnHeadersHeight = 29;
            this._cdgvDayTypeView.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvDayTypeView.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvDayTypeView.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvDayTypeView.DataGrid.Name = "_dgvData";
            this._cdgvDayTypeView.DataGrid.ReadOnly = true;
            this._cdgvDayTypeView.DataGrid.RowHeadersVisible = false;
            this._cdgvDayTypeView.DataGrid.RowHeadersWidth = 51;
            this._cdgvDayTypeView.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this._cdgvDayTypeView.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this._cdgvDayTypeView.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvDayTypeView.DataGrid.Size = new System.Drawing.Size(931, 519);
            this._cdgvDayTypeView.DataGrid.TabIndex = 0;
            this._cdgvDayTypeView.DefaultSortColumnName = null;
            this._cdgvDayTypeView.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvDayTypeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvDayTypeView.LocalizationHelper = null;
            this._cdgvDayTypeView.Location = new System.Drawing.Point(0, 0);
            this._cdgvDayTypeView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cdgvDayTypeView.Name = "_cdgvDayTypeView";
            this._cdgvDayTypeView.Size = new System.Drawing.Size(931, 519);
            this._cdgvDayTypeView.TabIndex = 0;
            // 
            // _bCalendar
            // 
            this._bCalendar.Location = new System.Drawing.Point(8, 14);
            this._bCalendar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCalendar.Name = "_bCalendar";
            this._bCalendar.Size = new System.Drawing.Size(100, 32);
            this._bCalendar.TabIndex = 1;
            this._bCalendar.Text = "Calendar";
            this._bCalendar.UseVisualStyleBackColor = true;
            this._bCalendar.Click += new System.EventHandler(this._bCalendar_Click);
            // 
            // _lDate
            // 
            this._lDate.AutoSize = true;
            this._lDate.Location = new System.Drawing.Point(115, 20);
            this._lDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDate.Name = "_lDate";
            this._lDate.Size = new System.Drawing.Size(36, 16);
            this._lDate.TabIndex = 0;
            this._lDate.Text = "Date";
            // 
            // _eDate
            // 
            this._eDate.Location = new System.Drawing.Point(160, 14);
            this._eDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDate.Name = "_eDate";
            this._eDate.ReadOnly = true;
            this._eDate.Size = new System.Drawing.Size(93, 22);
            this._eDate.TabIndex = 2;
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 25);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(946, 582);
            this._tpUserFolders.TabIndex = 4;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(846, 546);
            this._bRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(94, 29);
            this._bRefresh.TabIndex = 1;
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
            this._lbUserFolders.Size = new System.Drawing.Size(938, 524);
            this._lbUserFolders.TabIndex = 0;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 25);
            this._tpReferencedBy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Size = new System.Drawing.Size(946, 582);
            this._tpReferencedBy.TabIndex = 3;
            this._tpReferencedBy.Text = "Referenced By";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 25);
            this._tpDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDescription.Size = new System.Drawing.Size(946, 582);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(4, 4);
            this._eDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(938, 574);
            this._eDescription.TabIndex = 1;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(15, 19);
            this._lName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(44, 16);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(199, 15);
            this._eName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(250, 22);
            this._eName.TabIndex = 0;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(869, 672);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(100, 32);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(761, 672);
            this._bOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(100, 32);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._tcDateSettingsStatus);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(984, 715);
            this._panelBack.TabIndex = 0;
            // 
            // CalendarEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(984, 715);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(989, 723);
            this.Name = "CalendarEditForm";
            this.Text = "CalendarEditForm";
            this._tcDateSettingsStatus.ResumeLayout(false);
            this._tpCalendarSettings.ResumeLayout(false);
            this._gbCallendarSettings.ResumeLayout(false);
            this._gbCallendarSettings.PerformLayout();
            this._pnlValues.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this._tpDayTypeStatus.ResumeLayout(false);
            this._tpDayTypeStatus.PerformLayout();
            this._pnlDayTypeView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvDayTypeView.DataGrid)).EndInit();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl _tcDateSettingsStatus;
        private System.Windows.Forms.TabPage _tpCalendarSettings;
        private System.Windows.Forms.Label _lYear;
        private System.Windows.Forms.ComboBox _cbYear;
        private System.Windows.Forms.Label _lMonth;
        private System.Windows.Forms.Label _lDescription;
        private System.Windows.Forms.ComboBox _cbMonth;
        private System.Windows.Forms.TextBox _eDescriptionDateSettings;
        private System.Windows.Forms.Label _lWeek;
        private System.Windows.Forms.Button _bCreateCalendarSetting;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bCancelEdit;
        private System.Windows.Forms.ComboBox _cbWeek;
        private System.Windows.Forms.Button _bUpdateDateSetting;
        private System.Windows.Forms.Label _lDay;
        private System.Windows.Forms.Label _lDayType;
        private System.Windows.Forms.Button _bCreateDayType;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.ComboBox _cbDay;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.ComboBox _cbDayType;
        private System.Windows.Forms.TabPage _tpDayTypeStatus;
        private System.Windows.Forms.Button _bCalendar;
        private System.Windows.Forms.Label _lDate;
        private System.Windows.Forms.TextBox _eDate;
        private System.Windows.Forms.Button _bCalendar1;
        private System.Windows.Forms.Panel _pnlValues;
        private System.Windows.Forms.GroupBox _gbCallendarSettings;
        private System.Windows.Forms.Panel _pnlDayTypeView;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private Contal.Cgp.Components.CgpDataGridView _cdgvDayTypeView;
    }
}
