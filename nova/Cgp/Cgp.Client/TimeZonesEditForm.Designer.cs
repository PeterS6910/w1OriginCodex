namespace Contal.Cgp.Client
{
    partial class TimeZonesEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeZonesEditForm));
            this._lStatus = new System.Windows.Forms.Label();
            this._eStatus = new System.Windows.Forms.Label();
            this._lName = new System.Windows.Forms.Label();
            this._lCalendar = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._cbDay = new System.Windows.Forms.ComboBox();
            this._lDay = new System.Windows.Forms.Label();
            this._cbWeek = new System.Windows.Forms.ComboBox();
            this._lWeek = new System.Windows.Forms.Label();
            this._cbMonth = new System.Windows.Forms.ComboBox();
            this._lMonth = new System.Windows.Forms.Label();
            this._cbYear = new System.Windows.Forms.ComboBox();
            this._lYear = new System.Windows.Forms.Label();
            this._lDailyPlan = new System.Windows.Forms.Label();
            this._bCreateDateSetting = new System.Windows.Forms.Button();
            this._eDescriptionDateSettings = new System.Windows.Forms.TextBox();
            this._lDescriptionDateSettings = new System.Windows.Forms.Label();
            this._tbmDailyPlan = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tcDateSettingsStatus = new System.Windows.Forms.TabControl();
            this._tbDateSettings = new System.Windows.Forms.TabPage();
            this._groupBoxTimeZoneDateSettings = new System.Windows.Forms.GroupBox();
            this._cbExplicitDailyPlan = new System.Windows.Forms.CheckBox();
            this._bCalendar1 = new System.Windows.Forms.Button();
            this._tbDailyStatus = new System.Windows.Forms.TabPage();
            this._lColorOff = new System.Windows.Forms.Label();
            this._lColorOn = new System.Windows.Forms.Label();
            this._lStatusOff = new System.Windows.Forms.Label();
            this._lStatusOn = new System.Windows.Forms.Label();
            this._dmDayStatus = new Contal.IwQuick.UI.DateMatrix();
            this._bCalendar = new System.Windows.Forms.Button();
            this._lDate = new System.Windows.Forms.Label();
            this._eDate = new System.Windows.Forms.TextBox();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._panelBack = new System.Windows.Forms.Panel();
            this._tbmCalendar = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate = new System.Windows.Forms.ToolStripMenuItem();
            this._cmsDailyPlan = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate2 = new System.Windows.Forms.ToolStripMenuItem();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._tcDateSettingsStatus.SuspendLayout();
            this._tbDateSettings.SuspendLayout();
            this._groupBoxTimeZoneDateSettings.SuspendLayout();
            this._tbDailyStatus.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._panelBack.SuspendLayout();
            this._cmsDailyPlan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _lStatus
            // 
            this._lStatus.AutoSize = true;
            this._lStatus.Location = new System.Drawing.Point(9, 38);
            this._lStatus.Name = "_lStatus";
            this._lStatus.Size = new System.Drawing.Size(37, 13);
            this._lStatus.TabIndex = 4;
            this._lStatus.Text = "Status";
            // 
            // _eStatus
            // 
            this._eStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._eStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._eStatus.Location = new System.Drawing.Point(163, 35);
            this._eStatus.Name = "_eStatus";
            this._eStatus.Size = new System.Drawing.Size(170, 20);
            this._eStatus.TabIndex = 5;
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(9, 15);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _lCalendar
            // 
            this._lCalendar.AutoSize = true;
            this._lCalendar.Location = new System.Drawing.Point(389, 15);
            this._lCalendar.Name = "_lCalendar";
            this._lCalendar.Size = new System.Drawing.Size(49, 13);
            this._lCalendar.TabIndex = 2;
            this._lCalendar.Text = "Calendar";
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(163, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(170, 20);
            this._eName.TabIndex = 1;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(3, 3);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(779, 491);
            this._eDescription.TabIndex = 3;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(724, 597);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(80, 22);
            this._bCancel.TabIndex = 8;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(639, 597);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(80, 22);
            this._bOk.TabIndex = 7;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(692, 468);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(80, 22);
            this._bDelete.TabIndex = 6;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bRemove_Click);
            // 
            // _cbDay
            // 
            this._cbDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbDay.FormattingEnabled = true;
            this._cbDay.Location = new System.Drawing.Point(345, 32);
            this._cbDay.Name = "_cbDay";
            this._cbDay.Size = new System.Drawing.Size(90, 21);
            this._cbDay.TabIndex = 7;
            this._cbDay.SelectedIndexChanged += new System.EventHandler(this._cbDay_SelectedIndexChanged);
            // 
            // _lDay
            // 
            this._lDay.AutoSize = true;
            this._lDay.Location = new System.Drawing.Point(342, 16);
            this._lDay.Name = "_lDay";
            this._lDay.Size = new System.Drawing.Size(26, 13);
            this._lDay.TabIndex = 6;
            this._lDay.Text = "Day";
            // 
            // _cbWeek
            // 
            this._cbWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbWeek.FormattingEnabled = true;
            this._cbWeek.Location = new System.Drawing.Point(214, 32);
            this._cbWeek.Name = "_cbWeek";
            this._cbWeek.Size = new System.Drawing.Size(125, 21);
            this._cbWeek.TabIndex = 5;
            // 
            // _lWeek
            // 
            this._lWeek.AutoSize = true;
            this._lWeek.Location = new System.Drawing.Point(211, 16);
            this._lWeek.Name = "_lWeek";
            this._lWeek.Size = new System.Drawing.Size(36, 13);
            this._lWeek.TabIndex = 4;
            this._lWeek.Text = "Week";
            // 
            // _cbMonth
            // 
            this._cbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbMonth.FormattingEnabled = true;
            this._cbMonth.Location = new System.Drawing.Point(118, 32);
            this._cbMonth.Name = "_cbMonth";
            this._cbMonth.Size = new System.Drawing.Size(90, 21);
            this._cbMonth.TabIndex = 3;
            // 
            // _lMonth
            // 
            this._lMonth.AutoSize = true;
            this._lMonth.Location = new System.Drawing.Point(115, 16);
            this._lMonth.Name = "_lMonth";
            this._lMonth.Size = new System.Drawing.Size(37, 13);
            this._lMonth.TabIndex = 2;
            this._lMonth.Text = "Month";
            // 
            // _cbYear
            // 
            this._cbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbYear.FormattingEnabled = true;
            this._cbYear.Location = new System.Drawing.Point(22, 32);
            this._cbYear.Name = "_cbYear";
            this._cbYear.Size = new System.Drawing.Size(90, 21);
            this._cbYear.TabIndex = 1;
            // 
            // _lYear
            // 
            this._lYear.AutoSize = true;
            this._lYear.Location = new System.Drawing.Point(19, 16);
            this._lYear.Name = "_lYear";
            this._lYear.Size = new System.Drawing.Size(29, 13);
            this._lYear.TabIndex = 0;
            this._lYear.Text = "Year";
            // 
            // _lDailyPlan
            // 
            this._lDailyPlan.AutoSize = true;
            this._lDailyPlan.Location = new System.Drawing.Point(438, 17);
            this._lDailyPlan.Name = "_lDailyPlan";
            this._lDailyPlan.Size = new System.Drawing.Size(53, 13);
            this._lDailyPlan.TabIndex = 8;
            this._lDailyPlan.Text = "Daily plan";
            // 
            // _bCreateDateSetting
            // 
            this._bCreateDateSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreateDateSetting.Location = new System.Drawing.Point(629, 170);
            this._bCreateDateSetting.Name = "_bCreateDateSetting";
            this._bCreateDateSetting.Size = new System.Drawing.Size(143, 22);
            this._bCreateDateSetting.TabIndex = 1;
            this._bCreateDateSetting.Text = "Create date setting";
            this._bCreateDateSetting.UseVisualStyleBackColor = true;
            this._bCreateDateSetting.Click += new System.EventHandler(this._bCreateDateSettings_Click);
            // 
            // _eDescriptionDateSettings
            // 
            this._eDescriptionDateSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDescriptionDateSettings.Location = new System.Drawing.Point(22, 107);
            this._eDescriptionDateSettings.Multiline = true;
            this._eDescriptionDateSettings.Name = "_eDescriptionDateSettings";
            this._eDescriptionDateSettings.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescriptionDateSettings.Size = new System.Drawing.Size(575, 67);
            this._eDescriptionDateSettings.TabIndex = 12;
            // 
            // _lDescriptionDateSettings
            // 
            this._lDescriptionDateSettings.AutoSize = true;
            this._lDescriptionDateSettings.Location = new System.Drawing.Point(19, 91);
            this._lDescriptionDateSettings.Name = "_lDescriptionDateSettings";
            this._lDescriptionDateSettings.Size = new System.Drawing.Size(60, 13);
            this._lDescriptionDateSettings.TabIndex = 11;
            this._lDescriptionDateSettings.Text = "Description";
            // 
            // _tbmDailyPlan
            // 
            this._tbmDailyPlan.AllowDrop = true;
            this._tbmDailyPlan.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmDailyPlan.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmDailyPlan.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmDailyPlan.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmDailyPlan.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmDailyPlan.Button.Image")));
            this._tbmDailyPlan.Button.Location = new System.Drawing.Point(136, 0);
            this._tbmDailyPlan.Button.Name = "_bMenu";
            this._tbmDailyPlan.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmDailyPlan.Button.TabIndex = 3;
            this._tbmDailyPlan.Button.UseVisualStyleBackColor = false;
            this._tbmDailyPlan.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmDailyPlan.ButtonDefaultBehaviour = true;
            this._tbmDailyPlan.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmDailyPlan.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmDailyPlan.ButtonImage")));
            // 
            // 
            // 
            this._tbmDailyPlan.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify1,
            this._tsiCreate1});
            this._tbmDailyPlan.ButtonPopupMenu.Name = "";
            this._tbmDailyPlan.ButtonPopupMenu.Size = new System.Drawing.Size(113, 48);
            this._tbmDailyPlan.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmDailyPlan.ButtonShowImage = true;
            this._tbmDailyPlan.ButtonSizeHeight = 20;
            this._tbmDailyPlan.ButtonSizeWidth = 20;
            this._tbmDailyPlan.ButtonText = "";
            this._tbmDailyPlan.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmDailyPlan.HoverTime = 500;
            // 
            // 
            // 
            this._tbmDailyPlan.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmDailyPlan.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmDailyPlan.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmDailyPlan.ImageTextBox.ContextMenuStrip = this._tbmDailyPlan.ButtonPopupMenu;
            this._tbmDailyPlan.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmDailyPlan.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmDailyPlan.ImageTextBox.Image")));
            this._tbmDailyPlan.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmDailyPlan.ImageTextBox.Name = "_textBox";
            this._tbmDailyPlan.ImageTextBox.NoTextNoImage = true;
            this._tbmDailyPlan.ImageTextBox.ReadOnly = true;
            this._tbmDailyPlan.ImageTextBox.Size = new System.Drawing.Size(136, 20);
            this._tbmDailyPlan.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmDailyPlan.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmDailyPlan.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmDailyPlan.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmDailyPlan.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmDailyPlan.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmDailyPlan.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmDailyPlan.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmDailyPlan.ImageTextBox.TextBox.Size = new System.Drawing.Size(134, 13);
            this._tbmDailyPlan.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmDailyPlan.ImageTextBox.UseImage = true;
            this._tbmDailyPlan.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmDailyPlan_DoubleClick);
            this._tbmDailyPlan.Location = new System.Drawing.Point(442, 32);
            this._tbmDailyPlan.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmDailyPlan.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmDailyPlan.Name = "_tbmDailyPlan";
            this._tbmDailyPlan.Size = new System.Drawing.Size(156, 22);
            this._tbmDailyPlan.TabIndex = 9;
            this._tbmDailyPlan.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmDailyPlan.TextImage")));
            this._tbmDailyPlan.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmDailyPlan_DragOver);
            this._tbmDailyPlan.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmDailyPlan_DragDrop);
            this._tbmDailyPlan.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmDailyPlan_ButtonPopupMenuItemClick);
            // 
            // _tsiModify1
            // 
            this._tsiModify1.Name = "_tsiModify1";
            this._tsiModify1.Size = new System.Drawing.Size(112, 22);
            this._tsiModify1.Text = "Modify";
            // 
            // _tsiCreate1
            // 
            this._tsiCreate1.Name = "_tsiCreate1";
            this._tsiCreate1.Size = new System.Drawing.Size(112, 22);
            this._tsiCreate1.Text = "Create";
            // 
            // _tcDateSettingsStatus
            // 
            this._tcDateSettingsStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcDateSettingsStatus.Controls.Add(this._tbDateSettings);
            this._tcDateSettingsStatus.Controls.Add(this._tbDailyStatus);
            this._tcDateSettingsStatus.Controls.Add(this._tpUserFolders);
            this._tcDateSettingsStatus.Controls.Add(this._tpReferencedBy);
            this._tcDateSettingsStatus.Controls.Add(this._tpDescription);
            this._tcDateSettingsStatus.Location = new System.Drawing.Point(11, 68);
            this._tcDateSettingsStatus.Name = "_tcDateSettingsStatus";
            this._tcDateSettingsStatus.SelectedIndex = 0;
            this._tcDateSettingsStatus.Size = new System.Drawing.Size(793, 523);
            this._tcDateSettingsStatus.TabIndex = 6;
            this._tcDateSettingsStatus.TabStop = false;
            this._tcDateSettingsStatus.Enter += new System.EventHandler(this._tcDateSettingsStatus_Enter);
            // 
            // _tbDateSettings
            // 
            this._tbDateSettings.BackColor = System.Drawing.SystemColors.Control;
            this._tbDateSettings.Controls.Add(this._cdgvData);
            this._tbDateSettings.Controls.Add(this._groupBoxTimeZoneDateSettings);
            this._tbDateSettings.Controls.Add(this._bCreateDateSetting);
            this._tbDateSettings.Controls.Add(this._bDelete);
            this._tbDateSettings.Location = new System.Drawing.Point(4, 22);
            this._tbDateSettings.Name = "_tbDateSettings";
            this._tbDateSettings.Padding = new System.Windows.Forms.Padding(3);
            this._tbDateSettings.Size = new System.Drawing.Size(785, 497);
            this._tbDateSettings.TabIndex = 0;
            this._tbDateSettings.Text = "Date settings";
            // 
            // _groupBoxTimeZoneDateSettings
            // 
            this._groupBoxTimeZoneDateSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._cbExplicitDailyPlan);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._tbmDailyPlan);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._lYear);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._bCalendar1);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._cbDay);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._cbYear);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._lMonth);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._lDailyPlan);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._lDescriptionDateSettings);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._lDay);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._cbMonth);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._cbWeek);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._eDescriptionDateSettings);
            this._groupBoxTimeZoneDateSettings.Controls.Add(this._lWeek);
            this._groupBoxTimeZoneDateSettings.Location = new System.Drawing.Point(6, 6);
            this._groupBoxTimeZoneDateSettings.Name = "_groupBoxTimeZoneDateSettings";
            this._groupBoxTimeZoneDateSettings.Size = new System.Drawing.Size(610, 186);
            this._groupBoxTimeZoneDateSettings.TabIndex = 0;
            this._groupBoxTimeZoneDateSettings.TabStop = false;
            // 
            // _cbExplicitDailyPlan
            // 
            this._cbExplicitDailyPlan.AutoSize = true;
            this._cbExplicitDailyPlan.Location = new System.Drawing.Point(441, 63);
            this._cbExplicitDailyPlan.Name = "_cbExplicitDailyPlan";
            this._cbExplicitDailyPlan.Size = new System.Drawing.Size(106, 17);
            this._cbExplicitDailyPlan.TabIndex = 10;
            this._cbExplicitDailyPlan.Text = "Explicit daily plan";
            this._cbExplicitDailyPlan.UseVisualStyleBackColor = true;
            // 
            // _bCalendar1
            // 
            this._bCalendar1.Location = new System.Drawing.Point(22, 59);
            this._bCalendar1.Name = "_bCalendar1";
            this._bCalendar1.Size = new System.Drawing.Size(90, 23);
            this._bCalendar1.TabIndex = 13;
            this._bCalendar1.Text = "Calendar";
            this._bCalendar1.UseVisualStyleBackColor = true;
            this._bCalendar1.Click += new System.EventHandler(this._bCalendarDate_Click);
            // 
            // _tbDailyStatus
            // 
            this._tbDailyStatus.BackColor = System.Drawing.SystemColors.Control;
            this._tbDailyStatus.Controls.Add(this._lColorOff);
            this._tbDailyStatus.Controls.Add(this._lColorOn);
            this._tbDailyStatus.Controls.Add(this._lStatusOff);
            this._tbDailyStatus.Controls.Add(this._lStatusOn);
            this._tbDailyStatus.Controls.Add(this._dmDayStatus);
            this._tbDailyStatus.Controls.Add(this._bCalendar);
            this._tbDailyStatus.Controls.Add(this._lDate);
            this._tbDailyStatus.Controls.Add(this._eDate);
            this._tbDailyStatus.Location = new System.Drawing.Point(4, 22);
            this._tbDailyStatus.Name = "_tbDailyStatus";
            this._tbDailyStatus.Padding = new System.Windows.Forms.Padding(3);
            this._tbDailyStatus.Size = new System.Drawing.Size(785, 497);
            this._tbDailyStatus.TabIndex = 1;
            this._tbDailyStatus.Text = "Daily status";
            // 
            // _lColorOff
            // 
            this._lColorOff.BackColor = System.Drawing.Color.Red;
            this._lColorOff.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorOff.Location = new System.Drawing.Point(345, 11);
            this._lColorOff.Name = "_lColorOff";
            this._lColorOff.Size = new System.Drawing.Size(15, 15);
            this._lColorOff.TabIndex = 6;
            // 
            // _lColorOn
            // 
            this._lColorOn.BackColor = System.Drawing.Color.LightGreen;
            this._lColorOn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorOn.Location = new System.Drawing.Point(231, 11);
            this._lColorOn.Name = "_lColorOn";
            this._lColorOn.Size = new System.Drawing.Size(15, 15);
            this._lColorOn.TabIndex = 4;
            // 
            // _lStatusOff
            // 
            this._lStatusOff.AutoSize = true;
            this._lStatusOff.Location = new System.Drawing.Point(366, 12);
            this._lStatusOff.Name = "_lStatusOff";
            this._lStatusOff.Size = new System.Drawing.Size(60, 13);
            this._lStatusOff.TabIndex = 7;
            this._lStatusOff.Text = "Status OFF";
            // 
            // _lStatusOn
            // 
            this._lStatusOn.AutoSize = true;
            this._lStatusOn.Location = new System.Drawing.Point(252, 12);
            this._lStatusOn.Name = "_lStatusOn";
            this._lStatusOn.Size = new System.Drawing.Size(56, 13);
            this._lStatusOn.TabIndex = 5;
            this._lStatusOn.Text = "Status ON";
            // 
            // _dmDayStatus
            // 
            this._dmDayStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dmDayStatus.ButtonAlign = Contal.IwQuick.UI.DateMatrix.ButtonsAlign.Center;
            this._dmDayStatus.ButtonsFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._dmDayStatus.ButtonsHeight = 27;
            this._dmDayStatus.ButtonsWidth = 27;
            this._dmDayStatus.Coloras = new System.Drawing.Color[] {
        System.Drawing.Color.Red,
        System.Drawing.Color.LightGreen};
            this._dmDayStatus.ColorLeftClickIndex = 0;
            this._dmDayStatus.ColorRightClickIndex = 1;
            this._dmDayStatus.DefaultColorIndex = 0;
            this._dmDayStatus.Editable = false;
            this._dmDayStatus.LegendStepX = 10;
            this._dmDayStatus.LegendStepY = 6;
            this._dmDayStatus.Location = new System.Drawing.Point(6, 35);
            this._dmDayStatus.Name = "_dmDayStatus";
            this._dmDayStatus.ResizeButtons = false;
            this._dmDayStatus.SelectionType = Contal.IwQuick.UI.SelectionType.Vertical;
            this._dmDayStatus.Size = new System.Drawing.Size(762, 456);
            this._dmDayStatus.TabIndex = 3;
            this._dmDayStatus.ViewButtons = false;
            // 
            // _bCalendar
            // 
            this._bCalendar.Location = new System.Drawing.Point(6, 7);
            this._bCalendar.Name = "_bCalendar";
            this._bCalendar.Size = new System.Drawing.Size(80, 22);
            this._bCalendar.TabIndex = 0;
            this._bCalendar.Text = "Calendar";
            this._bCalendar.UseVisualStyleBackColor = true;
            this._bCalendar.Click += new System.EventHandler(this._bCalendar_Click);
            // 
            // _lDate
            // 
            this._lDate.AutoSize = true;
            this._lDate.Location = new System.Drawing.Point(92, 12);
            this._lDate.Name = "_lDate";
            this._lDate.Size = new System.Drawing.Size(30, 13);
            this._lDate.TabIndex = 1;
            this._lDate.Text = "Date";
            // 
            // _eDate
            // 
            this._eDate.Location = new System.Drawing.Point(138, 9);
            this._eDate.Name = "_eDate";
            this._eDate.ReadOnly = true;
            this._eDate.Size = new System.Drawing.Size(75, 20);
            this._eDate.TabIndex = 2;
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 22);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(785, 497);
            this._tpUserFolders.TabIndex = 4;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(705, 471);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 3;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _lbUserFolders
            // 
            this._lbUserFolders.AllowDrop = false;
            this._lbUserFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbUserFolders.BackColor = System.Drawing.SystemColors.Info;
            this._lbUserFolders.FormattingEnabled = true;
            this._lbUserFolders.Location = new System.Drawing.Point(0, 0);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.Size = new System.Drawing.Size(785, 459);
            this._lbUserFolders.TabIndex = 22;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(3);
            this._tpReferencedBy.Size = new System.Drawing.Size(785, 497);
            this._tpReferencedBy.TabIndex = 3;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(3);
            this._tpDescription.Size = new System.Drawing.Size(785, 497);
            this._tpDescription.TabIndex = 2;
            this._tpDescription.Text = "Description";
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._tbmCalendar);
            this._panelBack.Controls.Add(this._lStatus);
            this._panelBack.Controls.Add(this._tcDateSettingsStatus);
            this._panelBack.Controls.Add(this._lCalendar);
            this._panelBack.Controls.Add(this._eStatus);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(816, 631);
            this._panelBack.TabIndex = 0;
            // 
            // _tbmCalendar
            // 
            this._tbmCalendar.AllowDrop = true;
            this._tbmCalendar.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmCalendar.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCalendar.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmCalendar.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmCalendar.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmCalendar.Button.Image")));
            this._tbmCalendar.Button.Location = new System.Drawing.Point(136, 0);
            this._tbmCalendar.Button.Name = "_bMenu";
            this._tbmCalendar.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmCalendar.Button.TabIndex = 3;
            this._tbmCalendar.Button.UseVisualStyleBackColor = false;
            this._tbmCalendar.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmCalendar.ButtonDefaultBehaviour = true;
            this._tbmCalendar.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmCalendar.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmCalendar.ButtonImage")));
            // 
            // 
            // 
            this._tbmCalendar.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiCreate});
            this._tbmCalendar.ButtonPopupMenu.Name = "";
            this._tbmCalendar.ButtonPopupMenu.Size = new System.Drawing.Size(113, 48);
            this._tbmCalendar.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmCalendar.ButtonShowImage = true;
            this._tbmCalendar.ButtonSizeHeight = 20;
            this._tbmCalendar.ButtonSizeWidth = 20;
            this._tbmCalendar.ButtonText = "";
            this._tbmCalendar.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCalendar.HoverTime = 500;
            // 
            // 
            // 
            this._tbmCalendar.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCalendar.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmCalendar.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmCalendar.ImageTextBox.ContextMenuStrip = this._tbmCalendar.ButtonPopupMenu;
            this._tbmCalendar.ImageTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCalendar.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmCalendar.ImageTextBox.Image")));
            this._tbmCalendar.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmCalendar.ImageTextBox.Name = "_textBox";
            this._tbmCalendar.ImageTextBox.NoTextNoImage = true;
            this._tbmCalendar.ImageTextBox.ReadOnly = true;
            this._tbmCalendar.ImageTextBox.Size = new System.Drawing.Size(136, 20);
            this._tbmCalendar.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmCalendar.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCalendar.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmCalendar.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmCalendar.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCalendar.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmCalendar.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmCalendar.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmCalendar.ImageTextBox.TextBox.Size = new System.Drawing.Size(134, 13);
            this._tbmCalendar.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmCalendar.ImageTextBox.UseImage = true;
            this._tbmCalendar.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmCalendar_DoubleClick);
            this._tbmCalendar.Location = new System.Drawing.Point(463, 13);
            this._tbmCalendar.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmCalendar.MinimumSize = new System.Drawing.Size(30, 22);
            this._tbmCalendar.Name = "_tbmCalendar";
            this._tbmCalendar.Size = new System.Drawing.Size(156, 22);
            this._tbmCalendar.TabIndex = 3;
            this._tbmCalendar.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmCalendar.TextImage")));
            this._tbmCalendar.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmCalendar_DragOver);
            this._tbmCalendar.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmCalendar_DragDrop);
            this._tbmCalendar.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmCalendar_ButtonPopupMenuItemClick);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(112, 22);
            this._tsiModify.Text = "Modify";
            // 
            // _tsiCreate
            // 
            this._tsiCreate.Name = "_tsiCreate";
            this._tsiCreate.Size = new System.Drawing.Size(112, 22);
            this._tsiCreate.Text = "Create";
            // 
            // _cmsDailyPlan
            // 
            this._cmsDailyPlan.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiCreate2});
            this._cmsDailyPlan.Name = "_cmsDailyPlan";
            this._cmsDailyPlan.Size = new System.Drawing.Size(113, 48);
            this._cmsDailyPlan.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._cmsDailyPlan_ItemClicked);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(112, 22);
            this._tsiModify2.Text = "Modify";
            // 
            // _tsiCreate2
            // 
            this._tsiCreate2.Name = "_tsiCreate2";
            this._tsiCreate2.Size = new System.Drawing.Size(112, 22);
            this._tsiCreate2.Text = "Create";
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this._cdgvData.DataGrid.AllowUserToAddRows = false;
            this._cdgvData.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvData.DataGrid.AllowUserToResizeRows = false;
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(766, 264);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DataGrid.AllowDrop = true;
            this._cdgvData.DataGrid.ReadOnly = false;
            this._cdgvData.DataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._cdgvData.ImageList = null;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(6, 198);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(766, 264);
            this._cdgvData.TabIndex = 7;
            // 
            // TimeZonesEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(816, 631);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(824, 639);
            this.Name = "TimeZonesEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TimeZonesEditForm";
            this._tcDateSettingsStatus.ResumeLayout(false);
            this._tbDateSettings.ResumeLayout(false);
            this._groupBoxTimeZoneDateSettings.ResumeLayout(false);
            this._groupBoxTimeZoneDateSettings.PerformLayout();
            this._tbDailyStatus.ResumeLayout(false);
            this._tbDailyStatus.PerformLayout();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this._cmsDailyPlan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.ComboBox _cbYear;
        private System.Windows.Forms.Label _lYear;
        private System.Windows.Forms.ComboBox _cbDay;
        private System.Windows.Forms.Label _lDay;
        private System.Windows.Forms.ComboBox _cbWeek;
        private System.Windows.Forms.Label _lWeek;
        private System.Windows.Forms.ComboBox _cbMonth;
        private System.Windows.Forms.Label _lMonth;
        private System.Windows.Forms.Label _lDailyPlan;
        private System.Windows.Forms.Button _bCreateDateSetting;
        private System.Windows.Forms.TextBox _eDescriptionDateSettings;
        private System.Windows.Forms.Label _lDescriptionDateSettings;
        private System.Windows.Forms.Label _lStatus;
        private System.Windows.Forms.Label _eStatus;
        private System.Windows.Forms.TabControl _tcDateSettingsStatus;
        private System.Windows.Forms.TabPage _tbDateSettings;
        private System.Windows.Forms.TabPage _tbDailyStatus;
        private System.Windows.Forms.Button _bCalendar;
        private System.Windows.Forms.Label _lDate;
        private System.Windows.Forms.TextBox _eDate;
        private System.Windows.Forms.Label _lCalendar;
        private System.Windows.Forms.Button _bCalendar1;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.GroupBox _groupBoxTimeZoneDateSettings;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private Contal.IwQuick.UI.TextBoxMenu _tbmCalendar;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate;
        private Contal.IwQuick.UI.TextBoxMenu _tbmDailyPlan;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate1;
        private System.Windows.Forms.CheckBox _cbExplicitDailyPlan;
        private Contal.IwQuick.UI.DateMatrix _dmDayStatus;
        private System.Windows.Forms.Label _lColorOff;
        private System.Windows.Forms.Label _lColorOn;
        private System.Windows.Forms.Label _lStatusOff;
        private System.Windows.Forms.Label _lStatusOn;
        private System.Windows.Forms.ContextMenuStrip _cmsDailyPlan;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate2;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
    }
}