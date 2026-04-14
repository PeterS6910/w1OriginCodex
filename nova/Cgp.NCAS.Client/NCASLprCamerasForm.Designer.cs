namespace Contal.Cgp.NCAS.Client
{
    partial class NCASLprCamerasForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASLprCamerasForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter = new System.Windows.Forms.Panel();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._bLookupLprCameras = new System.Windows.Forms.Button();
            this._eMacAddressFilter = new System.Windows.Forms.TextBox();
            this._lMacAddressFilter = new System.Windows.Forms.Label();
            this._eIpAddressFilter = new System.Windows.Forms.TextBox();
            this._lIpAddressFilter = new System.Windows.Forms.Label();
            this._cbOnlineStateFilter = new System.Windows.Forms.ComboBox();
            this._lOnlineStateFilter = new System.Windows.Forms.Label();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._lNameFilter = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this._pFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.BackColor = System.Drawing.Color.White;
            this._cdgvData.CgpDataGridEvents = null;
            this._cdgvData.CopyOnRightClick = true;
            // 
            // 
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            this._cdgvData.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this._cdgvData.DataGrid.ColumnHeadersHeight = ((int)(resources.GetObject("resource.ColumnHeadersHeight")));
            this._cdgvData.DataGrid.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.RowHeadersWidth = ((int)(resources.GetObject("resource.RowHeadersWidth")));
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this._cdgvData.DataGrid.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            resources.ApplyResources(this._cdgvData, "_cdgvData");
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Name = "_cdgvData";
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._bLookupLprCameras);
            this._pFilter.Controls.Add(this._eMacAddressFilter);
            this._pFilter.Controls.Add(this._lMacAddressFilter);
            this._pFilter.Controls.Add(this._eIpAddressFilter);
            this._pFilter.Controls.Add(this._lIpAddressFilter);
            this._pFilter.Controls.Add(this._cbOnlineStateFilter);
            this._pFilter.Controls.Add(this._lOnlineStateFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            resources.ApplyResources(this._pFilter, "_pFilter");
            this._pFilter.Size = new System.Drawing.Size(1522, 120);
            this._pFilter.Name = "_pFilter";
            // 
            // _lRecordCount
            // 
            resources.ApplyResources(this._lRecordCount, "_lRecordCount");
            this._lRecordCount.Name = "_lRecordCount";
            // 
            // _bFilterClear
            // 
            this._bFilterClear.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this._bFilterClear, "_bFilterClear");
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.UseVisualStyleBackColor = false;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this._bRunFilter, "_bRunFilter");
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.UseVisualStyleBackColor = false;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _bLookupLprCameras
            // 
            this._bLookupLprCameras.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this._bLookupLprCameras, "_bLookupLprCameras");
            this._bLookupLprCameras.Location = new System.Drawing.Point(917, 68);
            this._bLookupLprCameras.Name = "_bFindAll";
            this._bLookupLprCameras.UseVisualStyleBackColor = false;
            this._bLookupLprCameras.Click += new System.EventHandler(this._bLookupLprCameras_Click);
            //
            // _eMacAddressFilter
            //
            resources.ApplyResources(this._eMacAddressFilter, "_eMacAddressFilter");
            this._eMacAddressFilter.Name = "_eMacAddressFilter";
            this._eMacAddressFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eMacAddressFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            //
            // _lMacAddressFilter
            //
            resources.ApplyResources(this._lMacAddressFilter, "_lMacAddressFilter");
            this._lMacAddressFilter.Name = "_lMacAddressFilter";
            //
            // _eIpAddressFilter
            //
            resources.ApplyResources(this._eIpAddressFilter, "_eIpAddressFilter");
            this._eIpAddressFilter.Name = "_eIpAddressFilter";
            this._eIpAddressFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eIpAddressFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            //
            // _lIpAddressFilter
            //
            resources.ApplyResources(this._lIpAddressFilter, "_lIpAddressFilter");
            this._lIpAddressFilter.Name = "_lIpAddressFilter";
            // 
            // _cbOnlineStateFilter
            // 
            this._cbOnlineStateFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbOnlineStateFilter.FormattingEnabled = true;
            resources.ApplyResources(this._cbOnlineStateFilter, "_cbOnlineStateFilter");
            this._cbOnlineStateFilter.Name = "_cbOnlineStateFilter";
            this._cbOnlineStateFilter.SelectedIndexChanged += new System.EventHandler(this.FilterValueChanged);
            this._cbOnlineStateFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _lOnlineStateFilter
            // 
            resources.ApplyResources(this._lOnlineStateFilter, "_lOnlineStateFilter");
            this._lOnlineStateFilter.Name = "_lOnlineStateFilter";
            // 
            // _eNameFilter
            // 
            resources.ApplyResources(this._eNameFilter, "_eNameFilter");
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eNameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _lNameFilter
            // 
            resources.ApplyResources(this._lNameFilter, "_lNameFilter");
            this._lNameFilter.Name = "_lNameFilter";
            // 
            // NCASLprCamerasForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Name = "NCASLprCamerasForm";
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Label _lRecordCount;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Button _bLookupLprCameras;
        private System.Windows.Forms.TextBox _eMacAddressFilter;
        private System.Windows.Forms.Label _lMacAddressFilter;
        private System.Windows.Forms.TextBox _eIpAddressFilter;
        private System.Windows.Forms.Label _lIpAddressFilter;
        private System.Windows.Forms.ComboBox _cbOnlineStateFilter;
        private System.Windows.Forms.Label _lOnlineStateFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Label _lNameFilter;
    }
}
