using Contal.Cgp.Globals.PlatformPC;

namespace Contal.Cgp.Client
{
    partial class CardsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._pFilter = new System.Windows.Forms.Panel();
            this._cbPersonFilter = new System.Windows.Forms.ComboBox();
            this._lPersonFilter = new System.Windows.Forms.Label();
            this._cbCardStateFilter = new System.Windows.Forms.ComboBox();
            this._lCardState = new System.Windows.Forms.Label();
            this._cbCardSystemFilter = new System.Windows.Forms.ComboBox();
            this._lCardSystemFilter = new System.Windows.Forms.Label();
            this._lCardNumberFilter = new System.Windows.Forms.Label();
            this._eCardNumberFilter = new System.Windows.Forms.TextBox();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._pControl = new System.Windows.Forms.Panel();
            this.bExportExcel = new System.Windows.Forms.Button();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._bPrint = new System.Windows.Forms.Button();
            this._bCSVImport = new System.Windows.Forms.Button();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter.SuspendLayout();
            this._pControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._cbPersonFilter);
            this._pFilter.Controls.Add(this._lPersonFilter);
            this._pFilter.Controls.Add(this._cbCardStateFilter);
            this._pFilter.Controls.Add(this._lCardState);
            this._pFilter.Controls.Add(this._cbCardSystemFilter);
            this._pFilter.Controls.Add(this._lCardSystemFilter);
            this._pFilter.Controls.Add(this._lCardNumberFilter);
            this._pFilter.Controls.Add(this._eCardNumberFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            resources.ApplyResources(this._pFilter, "_pFilter");
            this._pFilter.Name = "_pFilter";
            // 
            // _cbPersonFilter
            // 
            resources.ApplyResources(this._cbPersonFilter, "_cbPersonFilter");
            this._cbPersonFilter.FormattingEnabled = true;
            this._cbPersonFilter.Name = "_cbPersonFilter";
            this._cbPersonFilter.SelectedIndexChanged += new System.EventHandler(this._cbPersonFilter_SelectedIndexChanged);
            this._cbPersonFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._cbPersonFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _lPersonFilter
            // 
            resources.ApplyResources(this._lPersonFilter, "_lPersonFilter");
            this._lPersonFilter.Name = "_lPersonFilter";
            // 
            // _cbCardStateFilter
            // 
            this._cbCardStateFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this._cbCardStateFilter, "_cbCardStateFilter");
            this._cbCardStateFilter.FormattingEnabled = true;
            this._cbCardStateFilter.Name = "_cbCardStateFilter";
            this._cbCardStateFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._cbCardStateFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _lCardState
            // 
            resources.ApplyResources(this._lCardState, "_lCardState");
            this._lCardState.Name = "_lCardState";
            // 
            // _cbCardSystemFilter
            // 
            this._cbCardSystemFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this._cbCardSystemFilter, "_cbCardSystemFilter");
            this._cbCardSystemFilter.FormattingEnabled = true;
            this._cbCardSystemFilter.Name = "_cbCardSystemFilter";
            this._cbCardSystemFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._cbCardSystemFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _lCardSystemFilter
            // 
            resources.ApplyResources(this._lCardSystemFilter, "_lCardSystemFilter");
            this._lCardSystemFilter.Name = "_lCardSystemFilter";
            // 
            // _lCardNumberFilter
            // 
            resources.ApplyResources(this._lCardNumberFilter, "_lCardNumberFilter");
            this._lCardNumberFilter.Name = "_lCardNumberFilter";
            // 
            // _eCardNumberFilter
            // 
            resources.ApplyResources(this._eCardNumberFilter, "_eCardNumberFilter");
            this._eCardNumberFilter.Name = "_eCardNumberFilter";
            this._eCardNumberFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eCardNumberFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _bFilterClear
            // 
            resources.ApplyResources(this._bFilterClear, "_bFilterClear");
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            resources.ApplyResources(this._bRunFilter, "_bRunFilter");
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _pControl
            // 
            this._pControl.Controls.Add(this.bExportExcel);
            this._pControl.Controls.Add(this._lRecordCount);
            this._pControl.Controls.Add(this._bPrint);
            this._pControl.Controls.Add(this._bCSVImport);
            resources.ApplyResources(this._pControl, "_pControl");
            this._pControl.Name = "_pControl";
            // 
            // bExportExcel
            // 
            resources.ApplyResources(this.bExportExcel, "bExportExcel");
            this.bExportExcel.Name = "bExportExcel";
            this.bExportExcel.UseVisualStyleBackColor = true;
            this.bExportExcel.Click += new System.EventHandler(this.bExportExcel_Click);
            // 
            // _lRecordCount
            // 
            resources.ApplyResources(this._lRecordCount, "_lRecordCount");
            this._lRecordCount.Name = "_lRecordCount";
            // 
            // _bPrint
            // 
            resources.ApplyResources(this._bPrint, "_bPrint");
            this._bPrint.Name = "_bPrint";
            this._bPrint.UseVisualStyleBackColor = true;
            this._bPrint.Click += new System.EventHandler(this._bPrint_Click);
            // 
            // _bCSVImport
            // 
            resources.ApplyResources(this._bCSVImport, "_bCSVImport");
            this._bCSVImport.Name = "_bCSVImport";
            this._bCSVImport.UseVisualStyleBackColor = true;
            this._bCSVImport.Click += new System.EventHandler(this._bCSVImport_Click);
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
            this._cdgvData.DataGrid.ColumnHeadersHeight = ((int)(resources.GetObject("resource.ColumnHeadersHeight")));
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("resource.Dock")));
            this._cdgvData.DataGrid.Location = ((System.Drawing.Point)(resources.GetObject("resource.Location")));
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidth = ((int)(resources.GetObject("resource.RowHeadersWidth")));
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = ((System.Drawing.Size)(resources.GetObject("resource.Size")));
            this._cdgvData.DataGrid.TabIndex = ((int)(resources.GetObject("resource.TabIndex")));
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            resources.ApplyResources(this._cdgvData, "_cdgvData");
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Name = "_cdgvData";
            // 
            // CardsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Controls.Add(this._pControl);
            this.Name = "CardsForm";
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this._pControl.ResumeLayout(false);
            this._pControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Panel _pControl;
        private System.Windows.Forms.ComboBox _cbCardSystemFilter;
        private System.Windows.Forms.Label _lCardSystemFilter;
        private System.Windows.Forms.Label _lCardNumberFilter;
        private System.Windows.Forms.TextBox _eCardNumberFilter;
        private System.Windows.Forms.ComboBox _cbCardStateFilter;
        private System.Windows.Forms.Label _lCardState;
        private System.Windows.Forms.ComboBox _cbPersonFilter;
        private System.Windows.Forms.Label _lPersonFilter;
        private System.Windows.Forms.Button _bCSVImport;
        private System.Windows.Forms.Button _bPrint;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Label _lRecordCount;
        private System.Windows.Forms.Button bExportExcel;
    }
}
