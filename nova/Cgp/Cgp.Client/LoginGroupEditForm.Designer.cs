namespace Contal.Cgp.Client
{
    partial class LoginGroupEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginGroupEditForm));
            this._tpProperties = new System.Windows.Forms.TabPage();
            this._bApply2 = new System.Windows.Forms.Button();
            this._tbdpExpirationDate = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._lMustChangePassword = new System.Windows.Forms.Label();
            this._lLoginGroupName = new System.Windows.Forms.Label();
            this._eLoginGroupName = new System.Windows.Forms.TextBox();
            this._lDisabled = new System.Windows.Forms.Label();
            this._lExpirationDate = new System.Windows.Forms.Label();
            this._cbDisabled = new System.Windows.Forms.CheckBox();
            this._bRefresh = new System.Windows.Forms.Button();
            this._panelBack = new System.Windows.Forms.Panel();
            this._bApply = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._tpAccessControl = new System.Windows.Forms.TabPage();
            this._panelClbAccess = new System.Windows.Forms.Panel();
            this._tpMembers = new System.Windows.Forms.TabPage();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._lUsername = new System.Windows.Forms.Label();
            this._eUsername = new System.Windows.Forms.TextBox();
            this._bEdit = new System.Windows.Forms.Button();
            this._bAdd = new System.Windows.Forms.Button();
            this._bCreate = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._cdgvMembers = new Contal.Cgp.Components.CgpDataGridView();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._tpProperties.SuspendLayout();
            this._panelBack.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this._tpAccessControl.SuspendLayout();
            this._tpMembers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvMembers.DataGrid)).BeginInit();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tpProperties
            // 
            this._tpProperties.BackColor = System.Drawing.SystemColors.Control;
            this._tpProperties.Controls.Add(this._bApply2);
            this._tpProperties.Controls.Add(this._tbdpExpirationDate);
            this._tpProperties.Controls.Add(this._lMustChangePassword);
            this._tpProperties.Controls.Add(this._lLoginGroupName);
            this._tpProperties.Controls.Add(this._eLoginGroupName);
            this._tpProperties.Controls.Add(this._lDisabled);
            this._tpProperties.Controls.Add(this._lExpirationDate);
            this._tpProperties.Controls.Add(this._cbDisabled);
            this._tpProperties.Location = new System.Drawing.Point(4, 25);
            this._tpProperties.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpProperties.Name = "_tpProperties";
            this._tpProperties.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpProperties.Size = new System.Drawing.Size(965, 367);
            this._tpProperties.TabIndex = 0;
            this._tpProperties.Text = "Properties";
            // 
            // _bApply2
            // 
            this._bApply2.Location = new System.Drawing.Point(264, 68);
            this._bApply2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bApply2.Name = "_bApply2";
            this._bApply2.Size = new System.Drawing.Size(100, 32);
            this._bApply2.TabIndex = 12;
            this._bApply2.Text = "Apply";
            this._bApply2.UseVisualStyleBackColor = true;
            this._bApply2.Click += new System.EventHandler(this._bChangedPasswordAtNextLogin_Click);
            // 
            // _tbdpExpirationDate
            // 
            this._tbdpExpirationDate.addActualTime = false;
            this._tbdpExpirationDate.BackColor = System.Drawing.Color.Transparent;
            this._tbdpExpirationDate.ButtonClearDateImage = null;
            this._tbdpExpirationDate.ButtonClearDateText = "";
            this._tbdpExpirationDate.ButtonClearDateWidth = 23;
            this._tbdpExpirationDate.ButtonDateImage = null;
            this._tbdpExpirationDate.ButtonDateText = "";
            this._tbdpExpirationDate.ButtonDateWidth = 23;
            this._tbdpExpirationDate.CustomFormat = "dd.MM.yyyy";
            this._tbdpExpirationDate.DateFormName = "Calendar";
            this._tbdpExpirationDate.LocalizationHelper = null;
            this._tbdpExpirationDate.Location = new System.Drawing.Point(264, 103);
            this._tbdpExpirationDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbdpExpirationDate.MaximumSize = new System.Drawing.Size(1333, 74);
            this._tbdpExpirationDate.MinimumSize = new System.Drawing.Size(133, 27);
            this._tbdpExpirationDate.Name = "_tbdpExpirationDate";
            this._tbdpExpirationDate.ReadOnly = false;
            this._tbdpExpirationDate.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.Unknown;
            this._tbdpExpirationDate.Size = new System.Drawing.Size(400, 27);
            this._tbdpExpirationDate.TabIndex = 11;
            this._tbdpExpirationDate.ValidateAfter = 2D;
            this._tbdpExpirationDate.ValidationEnabled = false;
            this._tbdpExpirationDate.ValidationError = "";
            this._tbdpExpirationDate.Value = null;
            this._tbdpExpirationDate.ButtonClearDateClick += new Contal.IwQuick.UI.TextBoxDatePicker.DButtonClicked(this._tbdpExpirationDate_ButtonClearDateClick);
            this._tbdpExpirationDate.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this.EditTextChanger);
            // 
            // _lMustChangePassword
            // 
            this._lMustChangePassword.AutoSize = true;
            this._lMustChangePassword.Location = new System.Drawing.Point(8, 74);
            this._lMustChangePassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMustChangePassword.Name = "_lMustChangePassword";
            this._lMustChangePassword.Size = new System.Drawing.Size(189, 16);
            this._lMustChangePassword.TabIndex = 8;
            this._lMustChangePassword.Text = "Change password at next login";
            // 
            // _lLoginGroupName
            // 
            this._lLoginGroupName.AutoSize = true;
            this._lLoginGroupName.Location = new System.Drawing.Point(8, 15);
            this._lLoginGroupName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lLoginGroupName.Name = "_lLoginGroupName";
            this._lLoginGroupName.Size = new System.Drawing.Size(115, 16);
            this._lLoginGroupName.TabIndex = 0;
            this._lLoginGroupName.Text = "Login group name";
            // 
            // _eLoginGroupName
            // 
            this._eLoginGroupName.Location = new System.Drawing.Point(264, 11);
            this._eLoginGroupName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eLoginGroupName.Name = "_eLoginGroupName";
            this._eLoginGroupName.Size = new System.Drawing.Size(399, 22);
            this._eLoginGroupName.TabIndex = 1;
            this._eLoginGroupName.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lDisabled
            // 
            this._lDisabled.AutoSize = true;
            this._lDisabled.Location = new System.Drawing.Point(8, 43);
            this._lDisabled.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDisabled.Name = "_lDisabled";
            this._lDisabled.Size = new System.Drawing.Size(62, 16);
            this._lDisabled.TabIndex = 6;
            this._lDisabled.Text = "Disabled";
            // 
            // _lExpirationDate
            // 
            this._lExpirationDate.AutoSize = true;
            this._lExpirationDate.Location = new System.Drawing.Point(8, 108);
            this._lExpirationDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lExpirationDate.Name = "_lExpirationDate";
            this._lExpirationDate.Size = new System.Drawing.Size(96, 16);
            this._lExpirationDate.TabIndex = 10;
            this._lExpirationDate.Text = "Expiration date";
            // 
            // _cbDisabled
            // 
            this._cbDisabled.AutoSize = true;
            this._cbDisabled.Location = new System.Drawing.Point(264, 43);
            this._cbDisabled.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cbDisabled.Name = "_cbDisabled";
            this._cbDisabled.Size = new System.Drawing.Size(18, 17);
            this._cbDisabled.TabIndex = 7;
            this._cbDisabled.UseVisualStyleBackColor = true;
            this._cbDisabled.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(859, 332);
            this._bRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(100, 32);
            this._bRefresh.TabIndex = 0;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._bApply);
            this._panelBack.Controls.Add(this.tabControl1);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(1004, 476);
            this._panelBack.TabIndex = 1;
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(673, 433);
            this._bApply.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(100, 32);
            this._bApply.TabIndex = 4;
            this._bApply.Text = "OK";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this._tpProperties);
            this.tabControl1.Controls.Add(this._tpAccessControl);
            this.tabControl1.Controls.Add(this._tpMembers);
            this.tabControl1.Controls.Add(this._tpUserFolders);
            this.tabControl1.Controls.Add(this._tpReferencedBy);
            this.tabControl1.Controls.Add(this._tpDescription);
            this.tabControl1.Location = new System.Drawing.Point(16, 30);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(973, 396);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.TabStop = false;
            // 
            // _tpAccessControl
            // 
            this._tpAccessControl.BackColor = System.Drawing.SystemColors.Control;
            this._tpAccessControl.Controls.Add(this._panelClbAccess);
            this._tpAccessControl.Location = new System.Drawing.Point(4, 25);
            this._tpAccessControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpAccessControl.Name = "_tpAccessControl";
            this._tpAccessControl.Size = new System.Drawing.Size(965, 367);
            this._tpAccessControl.TabIndex = 2;
            this._tpAccessControl.Text = "Access control";
            // 
            // _panelClbAccess
            // 
            this._panelClbAccess.AutoScroll = true;
            this._panelClbAccess.BackColor = System.Drawing.SystemColors.Window;
            this._panelClbAccess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._panelClbAccess.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelClbAccess.Location = new System.Drawing.Point(0, 0);
            this._panelClbAccess.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelClbAccess.Name = "_panelClbAccess";
            this._panelClbAccess.Size = new System.Drawing.Size(965, 367);
            this._panelClbAccess.TabIndex = 1;
            // 
            // _tpMembers
            // 
            this._tpMembers.BackColor = System.Drawing.SystemColors.Control;
            this._tpMembers.Controls.Add(this._bFilterClear);
            this._tpMembers.Controls.Add(this._bRunFilter);
            this._tpMembers.Controls.Add(this._lUsername);
            this._tpMembers.Controls.Add(this._eUsername);
            this._tpMembers.Controls.Add(this._bEdit);
            this._tpMembers.Controls.Add(this._bAdd);
            this._tpMembers.Controls.Add(this._bCreate);
            this._tpMembers.Controls.Add(this._bDelete);
            this._tpMembers.Controls.Add(this._cdgvMembers);
            this._tpMembers.Location = new System.Drawing.Point(4, 25);
            this._tpMembers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpMembers.Name = "_tpMembers";
            this._tpMembers.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpMembers.Size = new System.Drawing.Size(965, 367);
            this._tpMembers.TabIndex = 5;
            this._tpMembers.Text = "Members";
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bFilterClear.Location = new System.Drawing.Point(320, 327);
            this._bFilterClear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(100, 32);
            this._bFilterClear.TabIndex = 11;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRunFilter.Location = new System.Drawing.Point(212, 329);
            this._bRunFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(100, 32);
            this._bRunFilter.TabIndex = 10;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _lUsername
            // 
            this._lUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lUsername.AutoSize = true;
            this._lUsername.Location = new System.Drawing.Point(8, 311);
            this._lUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lUsername.Name = "_lUsername";
            this._lUsername.Size = new System.Drawing.Size(73, 16);
            this._lUsername.TabIndex = 9;
            this._lUsername.Text = "User name";
            // 
            // _eUsername
            // 
            this._eUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._eUsername.Location = new System.Drawing.Point(8, 331);
            this._eUsername.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eUsername.Name = "_eUsername";
            this._eUsername.Size = new System.Drawing.Size(195, 22);
            this._eUsername.TabIndex = 8;
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(855, 329);
            this._bEdit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(100, 32);
            this._bEdit.TabIndex = 7;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _bAdd
            // 
            this._bAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd.Location = new System.Drawing.Point(531, 329);
            this._bAdd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(100, 32);
            this._bAdd.TabIndex = 6;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _bCreate
            // 
            this._bCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreate.Location = new System.Drawing.Point(639, 329);
            this._bCreate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCreate.Name = "_bCreate";
            this._bCreate.Size = new System.Drawing.Size(100, 32);
            this._bCreate.TabIndex = 5;
            this._bCreate.Text = "Create";
            this._bCreate.UseVisualStyleBackColor = true;
            this._bCreate.Click += new System.EventHandler(this._bCreate_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(747, 329);
            this._bDelete.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(100, 32);
            this._bDelete.TabIndex = 4;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _cdgvMembers
            // 
            this._cdgvMembers.AllowDrop = true;
            this._cdgvMembers.AllwaysRefreshOrder = false;
            this._cdgvMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cdgvMembers.CgpDataGridEvents = null;
            this._cdgvMembers.CopyOnRightClick = true;
            // 
            // 
            // 
            this._cdgvMembers.DataGrid.AllowUserToAddRows = false;
            this._cdgvMembers.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvMembers.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvMembers.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._cdgvMembers.DataGrid.ColumnHeadersHeight = 29;
            this._cdgvMembers.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvMembers.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvMembers.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvMembers.DataGrid.Name = "_dgvData";
            this._cdgvMembers.DataGrid.ReadOnly = true;
            this._cdgvMembers.DataGrid.RowHeadersVisible = false;
            this._cdgvMembers.DataGrid.RowHeadersWidth = 51;
            this._cdgvMembers.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvMembers.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvMembers.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvMembers.DataGrid.Size = new System.Drawing.Size(963, 308);
            this._cdgvMembers.DataGrid.TabIndex = 0;
            this._cdgvMembers.DefaultSortColumnName = null;
            this._cdgvMembers.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvMembers.LocalizationHelper = null;
            this._cdgvMembers.Location = new System.Drawing.Point(0, 0);
            this._cdgvMembers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cdgvMembers.Name = "_cdgvMembers";
            this._cdgvMembers.Size = new System.Drawing.Size(963, 308);
            this._cdgvMembers.TabIndex = 3;
            this._cdgvMembers.DragDrop += new System.Windows.Forms.DragEventHandler(this._cdgvMembers_DragDrop);
            this._cdgvMembers.DragOver += new System.Windows.Forms.DragEventHandler(this._cdgvMembers_DragOver);
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 25);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(965, 367);
            this._tpUserFolders.TabIndex = 4;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
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
            this._lbUserFolders.Location = new System.Drawing.Point(0, 0);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(959, 316);
            this._lbUserFolders.TabIndex = 16;
            this._lbUserFolders.TabStop = false;
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 25);
            this._tpReferencedBy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Size = new System.Drawing.Size(965, 367);
            this._tpReferencedBy.TabIndex = 3;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 25);
            this._tpDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDescription.Size = new System.Drawing.Size(965, 367);
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
            this._eDescription.Size = new System.Drawing.Size(957, 359);
            this._eDescription.TabIndex = 0;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(781, 433);
            this._bOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(100, 32);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(888, 433);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(100, 32);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // LoginGroupEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 476);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(1019, 513);
            this.Name = "LoginGroupEditForm";
            this.Text = "Edit login group";
            this._tpProperties.ResumeLayout(false);
            this._tpProperties.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this._tpAccessControl.ResumeLayout(false);
            this._tpMembers.ResumeLayout(false);
            this._tpMembers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvMembers.DataGrid)).EndInit();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage _tpProperties;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpExpirationDate;
        private System.Windows.Forms.Label _lMustChangePassword;
        private System.Windows.Forms.Label _lLoginGroupName;
        private System.Windows.Forms.TextBox _eLoginGroupName;
        private System.Windows.Forms.Label _lDisabled;
        private System.Windows.Forms.Label _lExpirationDate;
        private System.Windows.Forms.CheckBox _cbDisabled;
        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _tpAccessControl;
        private System.Windows.Forms.Panel _panelClbAccess;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.TabPage _tpMembers;
        private Contal.Cgp.Components.CgpDataGridView _cdgvMembers;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.Button _bCreate;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lUsername;
        private System.Windows.Forms.TextBox _eUsername;
        private System.Windows.Forms.Button _bApply2;
        private System.Windows.Forms.Button _bApply;
    }
}