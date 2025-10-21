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
            this._cbLocationFilter = new System.Windows.Forms.ComboBox();
            this._lLocationFilter = new System.Windows.Forms.Label();
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
            this._pFilter.Controls.Add(this._cbLocationFilter);
            this._pFilter.Controls.Add(this._lLocationFilter);
            this._pFilter.Controls.Add(this._cbOnlineStateFilter);
            this._pFilter.Controls.Add(this._lOnlineStateFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            resources.ApplyResources(this._pFilter, "_pFilter");
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
            this._bLookupLprCameras.Name = "_bFindAll";
            this._bLookupLprCameras.UseVisualStyleBackColor = false;
            this._bLookupLprCameras.Click += new System.EventHandler(this._bLookupLprCameras_Click);
            // 
            // _cbLocationFilter
            // 
            this._cbLocationFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbLocationFilter.FormattingEnabled = true;
            resources.ApplyResources(this._cbLocationFilter, "_cbLocationFilter");
            this._cbLocationFilter.Name = "_cbLocationFilter";
            // 
            // _lLocationFilter
            // 
            resources.ApplyResources(this._lLocationFilter, "_lLocationFilter");
            this._lLocationFilter.Name = "_lLocationFilter";
            // 
            // _cbOnlineStateFilter
            // 
            this._cbOnlineStateFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbOnlineStateFilter.FormattingEnabled = true;
            resources.ApplyResources(this._cbOnlineStateFilter, "_cbOnlineStateFilter");
            this._cbOnlineStateFilter.Name = "_cbOnlineStateFilter";
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
        private System.Windows.Forms.ComboBox _cbLocationFilter;
        private System.Windows.Forms.Label _lLocationFilter;
        private System.Windows.Forms.ComboBox _cbOnlineStateFilter;
        private System.Windows.Forms.Label _lOnlineStateFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Label _lNameFilter;
    }
}
