using Contal.Cgp.Globals.PlatformPC;

namespace Contal.Cgp.Client
{
    partial class PersonsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonsForm));
            this.panel3 = new System.Windows.Forms.Panel();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter = new System.Windows.Forms.Panel();
            this._pFilter.Font = System.Drawing.SystemFonts.MessageBoxFont;
            this._lDepartmentFilter = new System.Windows.Forms.Label();
            this._tbmDepartmentFilter = new Contal.IwQuick.UI.TextBoxMenu();
            this._lFullTextSearch = new System.Windows.Forms.Label();
            this._tbFullTextSearch = new System.Windows.Forms.TextBox();
            this._tbdpDateToFilter = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._tbdpDateFromFilter = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._lOtherInformationFields = new System.Windows.Forms.Label();
            this._eOtherInformationFiledsFilter = new System.Windows.Forms.TextBox();
            this._lDateToFilter = new System.Windows.Forms.Label();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._cbInactivePersons = new System.Windows.Forms.CheckBox();
            this._cbActivePersons = new System.Windows.Forms.CheckBox();
            this._lNumberFilter = new System.Windows.Forms.Label();
            this._lDateFromFilter = new System.Windows.Forms.Label();
            this._lSurnameFilter = new System.Windows.Forms.Label();
            this._lNameFilter = new System.Windows.Forms.Label();
            this._eNumberFilter = new System.Windows.Forms.TextBox();
            this._eSurnameFilter = new System.Windows.Forms.TextBox();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._pControl = new System.Windows.Forms.Panel();
            this._pControl.Font = System.Drawing.SystemFonts.MessageBoxFont;
            this._lRecordCount = new System.Windows.Forms.Label();
            this._bAclAssignment = new System.Windows.Forms.Button();
            this._bCSVImport = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this._pFilter.SuspendLayout();
            this._pControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._cdgvData);
            this.panel3.Controls.Add(this._pFilter);
            this.panel3.Controls.Add(this._pControl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1204, 775);
            this.panel3.TabIndex = 2;
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
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidth = 62;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(1204, 545);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(1204, 545);
            this._cdgvData.TabIndex = 3;
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._lDepartmentFilter);
            this._pFilter.Controls.Add(this._tbmDepartmentFilter);
            this._pFilter.Controls.Add(this._lFullTextSearch);
            this._pFilter.Controls.Add(this._tbFullTextSearch);
            this._pFilter.Controls.Add(this._tbdpDateToFilter);
            this._pFilter.Controls.Add(this._tbdpDateFromFilter);
            this._pFilter.Controls.Add(this._lOtherInformationFields);
            this._pFilter.Controls.Add(this._eOtherInformationFiledsFilter);
            this._pFilter.Controls.Add(this._lDateToFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._cbInactivePersons);
            this._pFilter.Controls.Add(this._cbActivePersons);
            this._pFilter.Controls.Add(this._lNumberFilter);
            this._pFilter.Controls.Add(this._lDateFromFilter);
            this._pFilter.Controls.Add(this._lSurnameFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._eNumberFilter);
            this._pFilter.Controls.Add(this._eSurnameFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 545);
            this._pFilter.Margin = new System.Windows.Forms.Padding(4);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(1204, 174);
            this._pFilter.TabIndex = 1;
            // 
            // _lDepartmentFilter
            // 
            this._lDepartmentFilter.AutoSize = true;
            this._lDepartmentFilter.Font = CgpUIDesign.Default;
            this._lDepartmentFilter.Location = new System.Drawing.Point(765, 140);
            this._lDepartmentFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDepartmentFilter.Name = "_lDepartmentFilter";
            this._lDepartmentFilter.Size = new System.Drawing.Size(94, 20);
            this._lDepartmentFilter.TabIndex = 18;
            this._lDepartmentFilter.Text = "Department";
            // 
            // _tbmDepartmentFilter
            // 
            this._tbmDepartmentFilter.BackColor = System.Drawing.SystemColors.Control;
            this._tbmDepartmentFilter.Font = CgpUIDesign.Default;
            // 
            // 
            // 
            this._tbmDepartmentFilter.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmDepartmentFilter.Button.Image")));
            this._tbmDepartmentFilter.Button.Location = new System.Drawing.Point(0, 0);
            this._tbmDepartmentFilter.Button.Name = "_bMenu";
            this._tbmDepartmentFilter.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmDepartmentFilter.Button.TabIndex = 3;
            this._tbmDepartmentFilter.Button.Click += new System.EventHandler(this.SelectDepartmentClick);
            this._tbmDepartmentFilter.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmDepartmentFilter.ButtonDefaultBehaviour = true;
            this._tbmDepartmentFilter.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmDepartmentFilter.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmDepartmentFilter.ButtonImage")));
            // 
            // 
            // 
            this._tbmDepartmentFilter.ButtonPopupMenu.Name = "";
            this._tbmDepartmentFilter.ButtonPopupMenu.Size = new System.Drawing.Size(100, 25);
            this._tbmDepartmentFilter.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmDepartmentFilter.ButtonShowImage = true;
            this._tbmDepartmentFilter.ButtonSizeHeight = 20;
            this._tbmDepartmentFilter.ButtonSizeWidth = 20;
            this._tbmDepartmentFilter.ButtonText = "";
            this._tbmDepartmentFilter.HoverTime = 500;
            // 
            // 
            // 
            this._tbmDepartmentFilter.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmDepartmentFilter.ImageTextBox.Image")));
            this._tbmDepartmentFilter.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmDepartmentFilter.ImageTextBox.Name = "_itbTextBox";
            this._tbmDepartmentFilter.ImageTextBox.NoTextNoImage = true;
            this._tbmDepartmentFilter.ImageTextBox.ReadOnly = true;
            this._tbmDepartmentFilter.ImageTextBox.Size = new System.Drawing.Size(73, 150);
            this._tbmDepartmentFilter.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmDepartmentFilter.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Control;
            this._tbmDepartmentFilter.ImageTextBox.TextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmDepartmentFilter.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmDepartmentFilter.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmDepartmentFilter.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmDepartmentFilter.ImageTextBox.UseImage = true;
            this._tbmDepartmentFilter.Location = new System.Drawing.Point(664, 140);
            this._tbmDepartmentFilter.Margin = new System.Windows.Forms.Padding(4);
            this._tbmDepartmentFilter.MaximumSize = new System.Drawing.Size(1200, 55);
            this._tbmDepartmentFilter.MinimumSize = new System.Drawing.Size(30, 20);
            this._tbmDepartmentFilter.Name = "_tbmDepartmentFilter";
            this._tbmDepartmentFilter.Size = new System.Drawing.Size(93, 26);
            this._tbmDepartmentFilter.TabIndex = 19;
            this._tbmDepartmentFilter.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmDepartmentFilter.TextImage")));
            // 
            // _lFullTextSearch
            // 
            this._lFullTextSearch.AutoSize = true;
            this._lFullTextSearch.Font = CgpUIDesign.Default;
            this._lFullTextSearch.Location = new System.Drawing.Point(12, 75);
            this._lFullTextSearch.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFullTextSearch.Name = "_lFullTextSearch";
            this._lFullTextSearch.Size = new System.Drawing.Size(115, 20);
            this._lFullTextSearch.TabIndex = 15;
            this._lFullTextSearch.Text = "FullTextSearch";
            // 
            // _tbFullTextSearch
            //
            this._tbFullTextSearch.Font = CgpUIDesign.Default;
            this._tbFullTextSearch.Location = new System.Drawing.Point(12, 104);
            this._tbFullTextSearch.Margin = new System.Windows.Forms.Padding(4);
            this._tbFullTextSearch.Name = "_tbFullTextSearch";
            this._tbFullTextSearch.Size = new System.Drawing.Size(360, 26);
            this._tbFullTextSearch.TabIndex = 14;
            this._tbFullTextSearch.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._tbFullTextSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _tbdpDateToFilter
            // 
            this._tbdpDateToFilter.addActualTime = false;
            this._tbdpDateToFilter.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateToFilter.Font = CgpUIDesign.Default;
            this._tbdpDateToFilter.ButtonClearDateImage = null;
            this._tbdpDateToFilter.ButtonClearDateText = "";
            this._tbdpDateToFilter.ButtonClearDateWidth = 23;
            this._tbdpDateToFilter.ButtonDateImage = null;
            this._tbdpDateToFilter.ButtonDateText = "";
            this._tbdpDateToFilter.ButtonDateWidth = 23;
            this._tbdpDateToFilter.CustomFormat = "d. M. yyyy";
            this._tbdpDateToFilter.DateFormName = "Calendar";
            this._tbdpDateToFilter.LocalizationHelper = null;
            this._tbdpDateToFilter.Location = new System.Drawing.Point(382, 104);
            this._tbdpDateToFilter.Margin = new System.Windows.Forms.Padding(4);
            this._tbdpDateToFilter.MaximumSize = new System.Drawing.Size(1500, 90);
            this._tbdpDateToFilter.MinimumSize = new System.Drawing.Size(150, 33);
            this._tbdpDateToFilter.Name = "_tbdpDateToFilter";
            this._tbdpDateToFilter.ReadOnly = false;
            this._tbdpDateToFilter.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.Unknown;
            this._tbdpDateToFilter.Size = new System.Drawing.Size(245, 33);
            this._tbdpDateToFilter.TabIndex = 7;
            this._tbdpDateToFilter.ValidateAfter = 2D;
            this._tbdpDateToFilter.ValidationEnabled = false;
            this._tbdpDateToFilter.ValidationError = "";
            this._tbdpDateToFilter.Value = null;
            this._tbdpDateToFilter.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this.FilterValueChanged);
            // 
            // _tbdpDateFromFilter
            // 
            this._tbdpDateFromFilter.addActualTime = false;
            this._tbdpDateFromFilter.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateFromFilter.Font = CgpUIDesign.Default;
            this._tbdpDateFromFilter.ButtonClearDateImage = null;
            this._tbdpDateFromFilter.ButtonClearDateText = "";
            this._tbdpDateFromFilter.ButtonClearDateWidth = 23;
            this._tbdpDateFromFilter.ButtonDateImage = null;
            this._tbdpDateFromFilter.ButtonDateText = "";
            this._tbdpDateFromFilter.ButtonDateWidth = 23;
            this._tbdpDateFromFilter.CustomFormat = "d. M. yyyy";
            this._tbdpDateFromFilter.DateFormName = "Calendar";
            this._tbdpDateFromFilter.LocalizationHelper = null;
            this._tbdpDateFromFilter.Location = new System.Drawing.Point(382, 32);
            this._tbdpDateFromFilter.Margin = new System.Windows.Forms.Padding(4);
            this._tbdpDateFromFilter.MaximumSize = new System.Drawing.Size(1500, 90);
            this._tbdpDateFromFilter.MinimumSize = new System.Drawing.Size(150, 33);
            this._tbdpDateFromFilter.Name = "_tbdpDateFromFilter";
            this._tbdpDateFromFilter.ReadOnly = false;
            this._tbdpDateFromFilter.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.Unknown;
            this._tbdpDateFromFilter.Size = new System.Drawing.Size(245, 33);
            this._tbdpDateFromFilter.TabIndex = 5;
            this._tbdpDateFromFilter.ValidateAfter = 2D;
            this._tbdpDateFromFilter.ValidationEnabled = false;
            this._tbdpDateFromFilter.ValidationError = "";
            this._tbdpDateFromFilter.Value = null;
            this._tbdpDateFromFilter.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this.FilterValueChanged);
            // 
            // _lOtherInformationFields
            // 
            this._lOtherInformationFields.AutoSize = true;
            this._lOtherInformationFields.Font = CgpUIDesign.Default;
            this._lOtherInformationFields.Location = new System.Drawing.Point(840, 4);
            this._lOtherInformationFields.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lOtherInformationFields.Name = "_lOtherInformationFields";
            this._lOtherInformationFields.Size = new System.Drawing.Size(173, 20);
            this._lOtherInformationFields.TabIndex = 10;
            this._lOtherInformationFields.Text = "Other information fields";
            // 
            // _eOtherInformationFiledsFilter
            // 
            this._eOtherInformationFiledsFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eOtherInformationFiledsFilter.Font = CgpUIDesign.Default;
            this._eOtherInformationFiledsFilter.Location = new System.Drawing.Point(840, 32);
            this._eOtherInformationFiledsFilter.Margin = new System.Windows.Forms.Padding(4);
            this._eOtherInformationFiledsFilter.Name = "_eOtherInformationFiledsFilter";
            this._eOtherInformationFiledsFilter.Size = new System.Drawing.Size(250, 26);
            this._eOtherInformationFiledsFilter.TabIndex = 11;
            this._eOtherInformationFiledsFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eOtherInformationFiledsFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _lDateToFilter
            // 
            this._lDateToFilter.AutoSize = true;
            this._lDateToFilter.Font = CgpUIDesign.Default;
            this._lDateToFilter.Location = new System.Drawing.Point(382, 75);
            this._lDateToFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDateToFilter.Name = "_lDateToFilter";
            this._lDateToFilter.Size = new System.Drawing.Size(62, 20);
            this._lDateToFilter.TabIndex = 6;
            this._lDateToFilter.Text = "Date to";
            // 
            // _bFilterClear
            //
            this._bFilterClear.Font = CgpUIDesign.Default;
            this._bFilterClear.Location = new System.Drawing.Point(1079, 104);
            this._bFilterClear.Margin = new System.Windows.Forms.Padding(4);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(112, 34);
            this._bFilterClear.TabIndex = 13;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            //
            this._bRunFilter.Font = CgpUIDesign.Default;
            this._bRunFilter.Location = new System.Drawing.Point(958, 104);
            this._bRunFilter.Margin = new System.Windows.Forms.Padding(4);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(112, 34);
            this._bRunFilter.TabIndex = 12;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _cbInactivePersons
            // 
            this._cbInactivePersons.AutoSize = true;
            this._cbInactivePersons.Font = CgpUIDesign.Default;
            this._cbInactivePersons.Location = new System.Drawing.Point(664, 104);
            this._cbInactivePersons.Margin = new System.Windows.Forms.Padding(4);
            this._cbInactivePersons.Name = "_cbInactivePersons";
            this._cbInactivePersons.Size = new System.Drawing.Size(151, 24);
            this._cbInactivePersons.TabIndex = 17;
            this._cbInactivePersons.Text = "Inactive persons";
            this._cbInactivePersons.UseVisualStyleBackColor = true;
            this._cbInactivePersons.CheckedChanged += new System.EventHandler(this.FilterValueChanged);
            this._cbInactivePersons.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _cbActivePersons
            // 
            this._cbActivePersons.AutoSize = true;
            this._cbActivePersons.Font = CgpUIDesign.Default;
            this._cbActivePersons.Location = new System.Drawing.Point(664, 77);
            this._cbActivePersons.Margin = new System.Windows.Forms.Padding(4);
            this._cbActivePersons.Name = "_cbActivePersons";
            this._cbActivePersons.Size = new System.Drawing.Size(139, 24);
            this._cbActivePersons.TabIndex = 16;
            this._cbActivePersons.Text = "Active persons";
            this._cbActivePersons.UseVisualStyleBackColor = true;
            this._cbActivePersons.CheckedChanged += new System.EventHandler(this.FilterValueChanged);
            this._cbActivePersons.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _lNumberFilter
            // 
            this._lNumberFilter.AutoSize = true;
            this._lNumberFilter.Font = CgpUIDesign.Default;
            this._lNumberFilter.Location = new System.Drawing.Point(664, 4);
            this._lNumberFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lNumberFilter.Name = "_lNumberFilter";
            this._lNumberFilter.Size = new System.Drawing.Size(65, 20);
            this._lNumberFilter.TabIndex = 8;
            this._lNumberFilter.Text = "Number";
            // 
            // _lDateFromFilter
            // 
            this._lDateFromFilter.AutoSize = true;
            this._lDateFromFilter.Font = CgpUIDesign.Default;
            this._lDateFromFilter.Location = new System.Drawing.Point(382, 4);
            this._lDateFromFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDateFromFilter.Name = "_lDateFromFilter";
            this._lDateFromFilter.Size = new System.Drawing.Size(80, 20);
            this._lDateFromFilter.TabIndex = 4;
            this._lDateFromFilter.Text = "Date from";
            // 
            // _lSurnameFilter
            // 
            this._lSurnameFilter.AutoSize = true;
            this._lSurnameFilter.Font = CgpUIDesign.Default;
            this._lSurnameFilter.Location = new System.Drawing.Point(202, 4);
            this._lSurnameFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lSurnameFilter.Name = "_lSurnameFilter";
            this._lSurnameFilter.Size = new System.Drawing.Size(74, 20);
            this._lSurnameFilter.TabIndex = 2;
            this._lSurnameFilter.Text = "Surname";
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Font = CgpUIDesign.Default;
            this._lNameFilter.Location = new System.Drawing.Point(12, 4);
            this._lNameFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Size = new System.Drawing.Size(51, 20);
            this._lNameFilter.TabIndex = 0;
            this._lNameFilter.Text = "Name";
            // 
            // _eNumberFilter
            //
            this._eNumberFilter.Font = CgpUIDesign.Default;
            this._eNumberFilter.Location = new System.Drawing.Point(664, 32);
            this._eNumberFilter.Margin = new System.Windows.Forms.Padding(4);
            this._eNumberFilter.Name = "_eNumberFilter";
            this._eNumberFilter.Size = new System.Drawing.Size(145, 26);
            this._eNumberFilter.TabIndex = 9;
            this._eNumberFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eNumberFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _eSurnameFilter
            //
            this._eSurnameFilter.Font = CgpUIDesign.Default;
            this._eSurnameFilter.Location = new System.Drawing.Point(202, 32);
            this._eSurnameFilter.Margin = new System.Windows.Forms.Padding(4);
            this._eSurnameFilter.Name = "_eSurnameFilter";
            this._eSurnameFilter.Size = new System.Drawing.Size(169, 26);
            this._eSurnameFilter.TabIndex = 3;
            this._eSurnameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eSurnameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _eNameFilter
            //
            this._eNameFilter.Font = CgpUIDesign.Default;
            this._eNameFilter.Location = new System.Drawing.Point(12, 32);
            this._eNameFilter.Margin = new System.Windows.Forms.Padding(4);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Size = new System.Drawing.Size(180, 26);
            this._eNameFilter.TabIndex = 1;
            this._eNameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eNameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _pControl
            // 
            this._pControl.Controls.Add(this._lRecordCount);
            this._pControl.Controls.Add(this._bAclAssignment);
            this._pControl.Controls.Add(this._bCSVImport);
            this._pControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pControl.Location = new System.Drawing.Point(0, 719);
            this._pControl.Margin = new System.Windows.Forms.Padding(4);
            this._pControl.Name = "_pControl";
            this._pControl.Size = new System.Drawing.Size(1204, 56);
            this._pControl.TabIndex = 2;
            // 
            // _lRecordCount
            // 
            this._lRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Font = CgpUIDesign.Default;
            this._lRecordCount.Location = new System.Drawing.Point(1000, 16);
            this._lRecordCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(104, 20);
            this._lRecordCount.TabIndex = 16;
            this._lRecordCount.Text = "RecordCount";
            // 
            // _bAclAssignment
            //
            this._bAclAssignment.Font = CgpUIDesign.Default;
            this._bAclAssignment.Location = new System.Drawing.Point(136, 9);
            this._bAclAssignment.Margin = new System.Windows.Forms.Padding(4);
            this._bAclAssignment.Name = "_bAclAssignment";
            this._bAclAssignment.Size = new System.Drawing.Size(152, 34);
            this._bAclAssignment.TabIndex = 4;
            this._bAclAssignment.Text = "ACL assignment ";
            this._bAclAssignment.UseVisualStyleBackColor = true;
            this._bAclAssignment.Click += new System.EventHandler(this._bAclAssignment_Click);
            // 
            // _bCSVImport
            //
            this._bCSVImport.Font = CgpUIDesign.Default;
            this._bCSVImport.Location = new System.Drawing.Point(15, 9);
            this._bCSVImport.Margin = new System.Windows.Forms.Padding(4);
            this._bCSVImport.Name = "_bCSVImport";
            this._bCSVImport.Size = new System.Drawing.Size(112, 34);
            this._bCSVImport.TabIndex = 3;
            this._bCSVImport.Text = "CSV import";
            this._bCSVImport.UseVisualStyleBackColor = true;
            this._bCSVImport.Click += new System.EventHandler(this._bCSVImport_Click);
            // 
            // PersonsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1204, 775);
            this.Font = CgpUIDesign.Default;
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PersonsForm";
            this.Text = "PersonsForm";
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this._pControl.ResumeLayout(false);
            this._pControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pControl;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lNumberFilter;
        private System.Windows.Forms.Label _lDateFromFilter;
        private System.Windows.Forms.Label _lSurnameFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eNumberFilter;
        private System.Windows.Forms.TextBox _eSurnameFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Label _lDateToFilter;
        private System.Windows.Forms.Label _lOtherInformationFields;
        private System.Windows.Forms.TextBox _eOtherInformationFiledsFilter;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateToFilter;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateFromFilter;
        private System.Windows.Forms.Button _bCSVImport;
        private System.Windows.Forms.Button _bAclAssignment;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Label _lFullTextSearch;
        private System.Windows.Forms.TextBox _tbFullTextSearch;
        private System.Windows.Forms.Label _lRecordCount;
        private System.Windows.Forms.CheckBox _cbActivePersons;
        private System.Windows.Forms.CheckBox _cbInactivePersons;
        private System.Windows.Forms.Label _lDepartmentFilter;
        private Contal.IwQuick.UI.TextBoxMenu _tbmDepartmentFilter;
    }
}
