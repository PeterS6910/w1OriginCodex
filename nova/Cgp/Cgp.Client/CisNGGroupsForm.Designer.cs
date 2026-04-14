namespace Contal.Cgp.Client
{
    partial class CisNGGroupsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CisNGGroupsForm));
            this._pFilter = new System.Windows.Forms.Panel();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._lDescriptionFilter = new System.Windows.Forms.Label();
            this._lGroupNameFilter = new System.Windows.Forms.Label();
            this._eDescriptionFilter = new System.Windows.Forms.TextBox();
            this._eGroupNameFilter = new System.Windows.Forms.TextBox();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._lDescriptionFilter);
            this._pFilter.Controls.Add(this._lGroupNameFilter);
            this._pFilter.Controls.Add(this._eDescriptionFilter);
            this._pFilter.Controls.Add(this._eGroupNameFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 624);
            this._pFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(1112, 75);
            this._pFilter.TabIndex = 4;
            // 
            // _lRecordCount
            // 
            this._lRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(930, 30);
            this._lRecordCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(0, 20);
            this._lRecordCount.TabIndex = 10;
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Location = new System.Drawing.Point(504, 33);
            this._bFilterClear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(112, 32);
            this._bFilterClear.TabIndex = 9;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this.FilterClearClick);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Location = new System.Drawing.Point(382, 33);
            this._bRunFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(112, 32);
            this._bRunFilter.TabIndex = 8;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this.RunFilterClick);
            // 
            // _lDescriptionFilter
            // 
            this._lDescriptionFilter.AutoSize = true;
            this._lDescriptionFilter.Location = new System.Drawing.Point(198, 8);
            this._lDescriptionFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDescriptionFilter.Name = "_lDescriptionFilter";
            this._lDescriptionFilter.Size = new System.Drawing.Size(89, 20);
            this._lDescriptionFilter.TabIndex = 2;
            this._lDescriptionFilter.Text = "Description";
            // 
            // _lGroupNameFilter
            // 
            this._lGroupNameFilter.AutoSize = true;
            this._lGroupNameFilter.Location = new System.Drawing.Point(12, 8);
            this._lGroupNameFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lGroupNameFilter.Name = "_lGroupNameFilter";
            this._lGroupNameFilter.Size = new System.Drawing.Size(100, 20);
            this._lGroupNameFilter.TabIndex = 0;
            this._lGroupNameFilter.Text = "Group Name";
            // 
            // _eDescriptionFilter
            // 
            this._eDescriptionFilter.Location = new System.Drawing.Point(202, 37);
            this._eDescriptionFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDescriptionFilter.Name = "_eDescriptionFilter";
            this._eDescriptionFilter.Size = new System.Drawing.Size(169, 26);
            this._eDescriptionFilter.TabIndex = 2;
            this._eDescriptionFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eDescriptionFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _eGroupNameFilter
            // 
            this._eGroupNameFilter.Location = new System.Drawing.Point(12, 37);
            this._eGroupNameFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eGroupNameFilter.Name = "_eGroupNameFilter";
            this._eGroupNameFilter.Size = new System.Drawing.Size(180, 26);
            this._eGroupNameFilter.TabIndex = 1;
            this._eGroupNameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eGroupNameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
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
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(1112, 624);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(1112, 624);
            this._cdgvData.TabIndex = 5;
            // 
            // CisNGGroupsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1112, 699);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CisNGGroupsForm";
            this.Text = "CisNGGroupsForm";
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lDescriptionFilter;
        private System.Windows.Forms.Label _lGroupNameFilter;
        private System.Windows.Forms.TextBox _eDescriptionFilter;
        private System.Windows.Forms.TextBox _eGroupNameFilter;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
