using Contal.Cgp.Globals.PlatformPC;

namespace Contal.Cgp.Client
{
    partial class CardTemplatesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardTemplatesForm));
            this._pFilter = new System.Windows.Forms.Panel();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._bClone = new System.Windows.Forms.Button();
            this._eCloneName = new System.Windows.Forms.TextBox();
            this._lCloneName = new System.Windows.Forms.Label();
            this._lNameFilter = new System.Windows.Forms.Label();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._eFilterName = new System.Windows.Forms.TextBox();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._bClone);
            this._pFilter.Controls.Add(this._eCloneName);
            this._pFilter.Controls.Add(this._lCloneName);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._eFilterName);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._pFilter.Location = new System.Drawing.Point(0, 331);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(609, 86);
            this._pFilter.TabIndex = 0;
            // 
            // _lRecordCount
            // 
            this._lRecordCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lRecordCount.Location = new System.Drawing.Point(450, 22);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(0, 25);
            this._lRecordCount.TabIndex = 6;
            // 
            // _bClone
            // 
            this._bClone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._bClone.Location = new System.Drawing.Point(429, 34);
            this._bClone.Name = "_bClone";
            this._bClone.Size = new System.Drawing.Size(75, 32);
            this._bClone.TabIndex = 5;
            this._bClone.Text = "Clone";
            this._bClone.UseVisualStyleBackColor = true;
            this._bClone.Click += new System.EventHandler(this._bClone_Click);
            // 
            // _eCloneName
            // 
            this._eCloneName.Location = new System.Drawing.Point(304, 37);
            this._eCloneName.Name = "_eCloneName";
            this._eCloneName.Size = new System.Drawing.Size(119, 31);
            this._eCloneName.TabIndex = 4;
            // 
            // _lCloneName
            // 
            this._lCloneName.AutoSize = true;
            this._lCloneName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lCloneName.Location = new System.Drawing.Point(301, 4);
            this._lCloneName.Name = "_lCloneName";
            this._lCloneName.Size = new System.Drawing.Size(209, 25);
            this._lCloneName.TabIndex = 3;
            this._lCloneName.Text = "Name for template clone";
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lNameFilter.Location = new System.Drawing.Point(12, 4);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Size = new System.Drawing.Size(59, 25);
            this._lNameFilter.TabIndex = 2;
            this._lNameFilter.Text = "Name";
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._bFilterClear.Location = new System.Drawing.Point(220, 35);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(75, 32);
            this._bFilterClear.TabIndex = 2;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._bRunFilter.Location = new System.Drawing.Point(139, 35);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(75, 32);
            this._bRunFilter.TabIndex = 1;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this.RunFilterClick);
            // 
            // _eFilterName
            // 
            this._eFilterName.Location = new System.Drawing.Point(12, 37);
            this._eFilterName.Name = "_eFilterName";
            this._eFilterName.Size = new System.Drawing.Size(121, 31);
            this._eFilterName.TabIndex = 0;
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
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(609, 331);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(609, 331);
            this._cdgvData.TabIndex = 2;
            // 
            // CardTemplatesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(609, 417);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CardTemplatesForm";
            this.Text = "CardTemplatesForm";
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.TextBox _eFilterName;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.Button _bClone;
        private System.Windows.Forms.TextBox _eCloneName;
        private System.Windows.Forms.Label _lCloneName;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
