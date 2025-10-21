namespace Contal.Cgp.NCAS.Client
{
    partial class NCASCCUsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASCCUsForm));
            this._pFilter = new System.Windows.Forms.Panel();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._bCCUsLookUp = new System.Windows.Forms.Button();
            this._cbOnlineStateFilter = new System.Windows.Forms.ComboBox();
            this._lOnlineStateFilter = new System.Windows.Forms.Label();
            this._lNameFilter = new System.Windows.Forms.Label();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._bCCUsLookUp);
            this._pFilter.Controls.Add(this._cbOnlineStateFilter);
            this._pFilter.Controls.Add(this._lOnlineStateFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 549);
            this._pFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(1248, 82);
            this._pFilter.TabIndex = 20;
            // 
            // _lRecordCount
            // 
            this._lRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(1100, 7);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(109, 20);
            this._lRecordCount.TabIndex = 22;
            this._lRecordCount.Text = "Record count:";
            // 
            // _bCCUsLookUp
            // 
            this._bCCUsLookUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCCUsLookUp.Location = new System.Drawing.Point(1078, 29);
            this._bCCUsLookUp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._bCCUsLookUp.Name = "_bCCUsLookUp";
            this._bCCUsLookUp.Size = new System.Drawing.Size(165, 32);
            this._bCCUsLookUp.TabIndex = 6;
            this._bCCUsLookUp.Text = "Look up for CCUs";
            this._bCCUsLookUp.UseVisualStyleBackColor = true;
            this._bCCUsLookUp.Click += new System.EventHandler(this._bCCUsLookUp_Click);
            // 
            // _cbOnlineStateFilter
            // 
            this._cbOnlineStateFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbOnlineStateFilter.FormattingEnabled = true;
            this._cbOnlineStateFilter.Location = new System.Drawing.Point(206, 32);
            this._cbOnlineStateFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cbOnlineStateFilter.Name = "_cbOnlineStateFilter";
            this._cbOnlineStateFilter.Size = new System.Drawing.Size(180, 28);
            this._cbOnlineStateFilter.TabIndex = 3;
            this._cbOnlineStateFilter.SelectedIndexChanged += new System.EventHandler(this._cbOnlineStateFilter_SelectedIndexChanged);
            // 
            // _lOnlineStateFilter
            // 
            this._lOnlineStateFilter.AutoSize = true;
            this._lOnlineStateFilter.Location = new System.Drawing.Point(201, 9);
            this._lOnlineStateFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lOnlineStateFilter.Name = "_lOnlineStateFilter";
            this._lOnlineStateFilter.Size = new System.Drawing.Size(94, 20);
            this._lOnlineStateFilter.TabIndex = 2;
            this._lOnlineStateFilter.Text = "Online state";
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Location = new System.Drawing.Point(18, 9);
            this._lNameFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Size = new System.Drawing.Size(51, 20);
            this._lNameFilter.TabIndex = 0;
            this._lNameFilter.Text = "Name";
            // 
            // _eNameFilter
            // 
            this._eNameFilter.Location = new System.Drawing.Point(18, 34);
            this._eNameFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Size = new System.Drawing.Size(176, 26);
            this._eNameFilter.TabIndex = 1;
            this._eNameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Location = new System.Drawing.Point(518, 29);
            this._bFilterClear.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(112, 32);
            this._bFilterClear.TabIndex = 5;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Location = new System.Drawing.Point(396, 29);
            this._bRunFilter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(112, 32);
            this._bRunFilter.TabIndex = 4;
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
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidth = 62;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(1248, 549);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(1248, 549);
            this._cdgvData.TabIndex = 21;
            // 
            // NCASCCUsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1248, 631);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "NCASCCUsForm";
            this.Text = "NCASCCUsForm";
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Button _bCCUsLookUp;
        private System.Windows.Forms.ComboBox _cbOnlineStateFilter;
        private System.Windows.Forms.Label _lOnlineStateFilter;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
