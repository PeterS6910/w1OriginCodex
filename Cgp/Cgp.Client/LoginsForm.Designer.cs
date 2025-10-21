using Contal.Cgp.Globals.PlatformPC;

namespace Contal.Cgp.Client
{
    partial class LoginsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginsForm));
            this.panel3 = new System.Windows.Forms.Panel();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter = new System.Windows.Forms.Panel();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._lLoginGroupFilter = new System.Windows.Forms.Label();
            this._eLoginGroupFilter = new System.Windows.Forms.TextBox();
            this._eDisabledFilter = new System.Windows.Forms.ComboBox();
            this._eExpirationFilter = new System.Windows.Forms.ComboBox();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._lExpirationSetFilter = new System.Windows.Forms.Label();
            this._lDisabledFilter = new System.Windows.Forms.Label();
            this._lUserNameFilter = new System.Windows.Forms.Label();
            this._eUserNameFilter = new System.Windows.Forms.TextBox();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this._pFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._cdgvData);
            this.panel3.Controls.Add(this._pFilter);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(824, 385);
            this.panel3.TabIndex = 3;
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
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(824, 313);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(824, 313);
            this._cdgvData.TabIndex = 2;
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._lLoginGroupFilter);
            this._pFilter.Controls.Add(this._eLoginGroupFilter);
            this._pFilter.Controls.Add(this._eDisabledFilter);
            this._pFilter.Controls.Add(this._eExpirationFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._lExpirationSetFilter);
            this._pFilter.Controls.Add(this._lDisabledFilter);
            this._pFilter.Controls.Add(this._lUserNameFilter);
            this._pFilter.Controls.Add(this._eUserNameFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._pFilter.Location = new System.Drawing.Point(0, 313);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(824, 72);
            this._pFilter.TabIndex = 1;
            // 
            // _lRecordCount
            // 
            this._lRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lRecordCount.Location = new System.Drawing.Point(650, 8);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(0, 25);
            this._lRecordCount.TabIndex = 9;
            // 
            // _lLoginGroupFilter
            // 
            this._lLoginGroupFilter.AutoSize = true;
            this._lLoginGroupFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lLoginGroupFilter.Location = new System.Drawing.Point(135, 5);
            this._lLoginGroupFilter.Name = "_lLoginGroupFilter";
            this._lLoginGroupFilter.Size = new System.Drawing.Size(110, 25);
            this._lLoginGroupFilter.TabIndex = 8;
            this._lLoginGroupFilter.Text = "Login group";
            // 
            // _eLoginGroupFilter
            // 
            this._eLoginGroupFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._eLoginGroupFilter.Location = new System.Drawing.Point(135, 37);
            this._eLoginGroupFilter.Name = "_eLoginGroupFilter";
            this._eLoginGroupFilter.Size = new System.Drawing.Size(121, 31);
            this._eLoginGroupFilter.TabIndex = 7;
            this._eLoginGroupFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eLoginGroupFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _eDisabledFilter
            // 
            this._eDisabledFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._eDisabledFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._eDisabledFilter.FormattingEnabled = true;
            this._eDisabledFilter.Items.AddRange(new object[] {
            "",
            "true",
            "false"});
            this._eDisabledFilter.Location = new System.Drawing.Point(262, 37);
            this._eDisabledFilter.Name = "_eDisabledFilter";
            this._eDisabledFilter.Size = new System.Drawing.Size(70, 33);
            this._eDisabledFilter.TabIndex = 1;
            this._eDisabledFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _eExpirationFilter
            // 
            this._eExpirationFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._eExpirationFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._eExpirationFilter.FormattingEnabled = true;
            this._eExpirationFilter.Items.AddRange(new object[] {
            "",
            "true",
            "false"});
            this._eExpirationFilter.Location = new System.Drawing.Point(338, 37);
            this._eExpirationFilter.Name = "_eExpirationFilter";
            this._eExpirationFilter.Size = new System.Drawing.Size(70, 33);
            this._eExpirationFilter.TabIndex = 2;
            this._eExpirationFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._bFilterClear.Location = new System.Drawing.Point(573, 33);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(75, 32);
            this._bFilterClear.TabIndex = 4;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._bRunFilter.Location = new System.Drawing.Point(492, 33);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(75, 32);
            this._bRunFilter.TabIndex = 3;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _lExpirationSetFilter
            // 
            this._lExpirationSetFilter.AutoSize = true;
            this._lExpirationSetFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lExpirationSetFilter.Location = new System.Drawing.Point(338, 5);
            this._lExpirationSetFilter.Name = "_lExpirationSetFilter";
            this._lExpirationSetFilter.Size = new System.Drawing.Size(118, 25);
            this._lExpirationSetFilter.TabIndex = 6;
            this._lExpirationSetFilter.Text = "Expiration set";
            // 
            // _lDisabledFilter
            // 
            this._lDisabledFilter.AutoSize = true;
            this._lDisabledFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lDisabledFilter.Location = new System.Drawing.Point(259, 5);
            this._lDisabledFilter.Name = "_lDisabledFilter";
            this._lDisabledFilter.Size = new System.Drawing.Size(81, 25);
            this._lDisabledFilter.TabIndex = 5;
            this._lDisabledFilter.Text = "Disabled";
            // 
            // _lUserNameFilter
            // 
            this._lUserNameFilter.AutoSize = true;
            this._lUserNameFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lUserNameFilter.Location = new System.Drawing.Point(8, 5);
            this._lUserNameFilter.Name = "_lUserNameFilter";
            this._lUserNameFilter.Size = new System.Drawing.Size(96, 25);
            this._lUserNameFilter.TabIndex = 4;
            this._lUserNameFilter.Text = "User name";
            // 
            // _eUserNameFilter
            // 
            this._eUserNameFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._eUserNameFilter.Location = new System.Drawing.Point(8, 37);
            this._eUserNameFilter.Name = "_eUserNameFilter";
            this._eUserNameFilter.Size = new System.Drawing.Size(121, 31);
            this._eUserNameFilter.TabIndex = 0;
            this._eUserNameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eUserNameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // LoginsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(824, 385);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginsForm";
            this.Text = "User logins";
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lExpirationSetFilter;
        private System.Windows.Forms.Label _lDisabledFilter;
        private System.Windows.Forms.Label _lUserNameFilter;
        private System.Windows.Forms.TextBox _eUserNameFilter;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox _eExpirationFilter;
        private System.Windows.Forms.ComboBox _eDisabledFilter;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Label _lLoginGroupFilter;
        private System.Windows.Forms.TextBox _eLoginGroupFilter;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
