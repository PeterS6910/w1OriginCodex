using Contal.Cgp.Globals.PlatformPC;

namespace Contal.Cgp.Client
{
    partial class CardSystemsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardSystemsForm));
            this._pFilter = new System.Windows.Forms.Panel();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._cbCardTypeFilter = new System.Windows.Forms.ComboBox();
            this._eCompanyCodeFilter = new System.Windows.Forms.TextBox();
            this._lCompanyCodeFilter = new System.Windows.Forms.Label();
            this._lCardTypeFilter = new System.Windows.Forms.Label();
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
            this._pFilter.Controls.Add(this._cbCardTypeFilter);
            this._pFilter.Controls.Add(this._eCompanyCodeFilter);
            this._pFilter.Controls.Add(this._lCompanyCodeFilter);
            this._pFilter.Controls.Add(this._lCardTypeFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 417);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Font = CgpUIDesign.Default;
            this._pFilter.Size = new System.Drawing.Size(861, 72);
            this._pFilter.TabIndex = 0;
            // 
            // _lRecordCount
            // 
            this._lRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(680, 40);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Font = CgpUIDesign.Default;
            this._lRecordCount.Size = new System.Drawing.Size(0, 20);
            this._lRecordCount.TabIndex = 5;
            // 
            // _cbCardTypeFilter
            // 
            this._cbCardTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCardTypeFilter.FormattingEnabled = true;
            this._cbCardTypeFilter.Location = new System.Drawing.Point(259, 31);
            this._cbCardTypeFilter.Name = "_cbCardTypeFilter";
            this._cbCardTypeFilter.Font = CgpUIDesign.Default;
            this._cbCardTypeFilter.Size = new System.Drawing.Size(121, 28);
            this._cbCardTypeFilter.TabIndex = 2;
            this._cbCardTypeFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._cbCardTypeFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _eCompanyCodeFilter
            // 
            this._eCompanyCodeFilter.Location = new System.Drawing.Point(142, 31);
            this._eCompanyCodeFilter.Name = "_eCompanyCodeFilter";
            this._eCompanyCodeFilter.Font = CgpUIDesign.Default;
            this._eCompanyCodeFilter.Size = new System.Drawing.Size(111, 26);
            this._eCompanyCodeFilter.TabIndex = 1;
            this._eCompanyCodeFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eCompanyCodeFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _lCompanyCodeFilter
            // 
            this._lCompanyCodeFilter.AutoSize = true;
            this._lCompanyCodeFilter.Location = new System.Drawing.Point(139, 6);
            this._lCompanyCodeFilter.Name = "_lCompanyCodeFilter";
            this._lCompanyCodeFilter.Font = CgpUIDesign.Default;
            this._lCompanyCodeFilter.Size = new System.Drawing.Size(115, 20);
            this._lCompanyCodeFilter.TabIndex = 18;
            this._lCompanyCodeFilter.Text = "Company code";
            // 
            // _lCardTypeFilter
            // 
            this._lCardTypeFilter.AutoSize = true;
            this._lCardTypeFilter.Location = new System.Drawing.Point(256, 6);
            this._lCardTypeFilter.Name = "_lCardTypeFilter";
            this._lCardTypeFilter.Font = CgpUIDesign.Default;
            this._lCardTypeFilter.Size = new System.Drawing.Size(77, 20);
            this._lCardTypeFilter.TabIndex = 17;
            this._lCardTypeFilter.Text = "Card type";
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Location = new System.Drawing.Point(12, 6);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Font = CgpUIDesign.Default;
            this._lNameFilter.Size = new System.Drawing.Size(51, 20);
            this._lNameFilter.TabIndex = 14;
            this._lNameFilter.Text = "Name";
            // 
            // _eNameFilter
            // 
            this._eNameFilter.Location = new System.Drawing.Point(12, 31);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Font = CgpUIDesign.Default;
            this._eNameFilter.Size = new System.Drawing.Size(124, 26);
            this._eNameFilter.TabIndex = 0;
            this._eNameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eNameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Location = new System.Drawing.Point(467, 29);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Font = CgpUIDesign.Default;
            this._bFilterClear.Size = new System.Drawing.Size(75, 32);
            this._bFilterClear.TabIndex = 4;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Location = new System.Drawing.Point(386, 29);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Font = CgpUIDesign.Default;
            this._bRunFilter.Size = new System.Drawing.Size(75, 32);
            this._bRunFilter.TabIndex = 3;
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
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(861, 417);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(861, 417);
            this._cdgvData.TabIndex = 2;
            // 
            // CardSystemsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(861, 489);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CardSystemsForm";
            this.Text = "CardSystemForm";
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.TextBox _eCompanyCodeFilter;
        private System.Windows.Forms.Label _lCompanyCodeFilter;
        private System.Windows.Forms.Label _lCardTypeFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.ComboBox _cbCardTypeFilter;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
