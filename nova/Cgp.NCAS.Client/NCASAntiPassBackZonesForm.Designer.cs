namespace Contal.Cgp.NCAS.Client
{
    partial class NCASAntiPassBackZonesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASAntiPassBackZonesForm));
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this._tbName = new System.Windows.Forms.TextBox();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._lName = new System.Windows.Forms.Label();
            this._lRecordCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            this._cdgvData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
            this._cdgvData.DataGrid.Margin = new System.Windows.Forms.Padding(4);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidth = 62;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._cdgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._cdgvData.DataGrid.RowTemplate.Height = 24;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(897, 227);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Margin = new System.Windows.Forms.Padding(0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(901, 231);
            this._cdgvData.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this._lRecordCount);
            this.panel2.Controls.Add(this._tbName);
            this.panel2.Controls.Add(this._bFilterClear);
            this.panel2.Controls.Add(this._bRunFilter);
            this.panel2.Controls.Add(this._lName);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 231);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(901, 70);
            this.panel2.TabIndex = 0;
            // 
            // _tbName
            // 
            this._tbName.Location = new System.Drawing.Point(8, 30);
            this._tbName.Margin = new System.Windows.Forms.Padding(2);
            this._tbName.Name = "_tbName";
            this._tbName.Size = new System.Drawing.Size(156, 26);
            this._tbName.TabIndex = 1;
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._bFilterClear.Location = new System.Drawing.Point(248, 28);
            this._bFilterClear.Margin = new System.Windows.Forms.Padding(0);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(74, 32);
            this._bFilterClear.TabIndex = 0;
            this._bFilterClear.Text = "&Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._bRunFilter.Location = new System.Drawing.Point(168, 28);
            this._bRunFilter.Margin = new System.Windows.Forms.Padding(0);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(74, 32);
            this._bRunFilter.TabIndex = 0;
            this._bRunFilter.Text = "&Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(9, 6);
            this._lName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(51, 20);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _lRecordCount
            // 
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(357, 28);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(109, 20);
            this._lRecordCount.TabIndex = 57;
            this._lRecordCount.Text = "Record count:";
            // 
            // NCASAntiPassBackZonesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(901, 301);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "NCASAntiPassBackZonesForm";
            this.Text = "NCASAntiPassBackZonesForm";
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _tbName;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
