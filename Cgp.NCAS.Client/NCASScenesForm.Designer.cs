namespace Contal.Cgp.NCAS.Client
{
    partial class NCASScenesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASScenesForm));
            this._dgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._lName = new System.Windows.Forms.Label();
            this._tbName = new System.Windows.Forms.TextBox();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._lRecordCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgvData
            // 
            this._dgvData.AllwaysRefreshOrder = false;
            this._dgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvData.CgpDataGridEvents = null;
            this._dgvData.CopyOnRightClick = true;
            // 
            // 
            // 
            this._dgvData.DataGrid.AllowUserToAddRows = false;
            this._dgvData.DataGrid.AllowUserToDeleteRows = false;
            this._dgvData.DataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            this._dgvData.DataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._dgvData.DataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._dgvData.DataGrid.ColumnHeadersHeight = 34;
            this._dgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._dgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._dgvData.DataGrid.Name = "_dgvData";
            this._dgvData.DataGrid.ReadOnly = true;
            this._dgvData.DataGrid.RowHeadersVisible = false;
            this._dgvData.DataGrid.RowHeadersWidth = 62;
            this._dgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._dgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._dgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvData.DataGrid.Size = new System.Drawing.Size(1002, 530);
            this._dgvData.DataGrid.TabIndex = 0;
            this._dgvData.DefaultSortColumnName = null;
            this._dgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._dgvData.LocalizationHelper = null;
            this._dgvData.Location = new System.Drawing.Point(0, 0);
            this._dgvData.Name = "_dgvData";
            this._dgvData.Size = new System.Drawing.Size(1002, 530);
            this._dgvData.TabIndex = 1;
            // 
            // _lName
            // 
            this._lName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(5, 555);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(51, 20);
            this._lName.TabIndex = 4;
            this._lName.Text = "Name";
            // 
            // _tbName
            // 
            this._tbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._tbName.Location = new System.Drawing.Point(65, 550);
            this._tbName.Name = "_tbName";
            this._tbName.Size = new System.Drawing.Size(129, 26);
            this._tbName.TabIndex = 5;
            this._tbName.TextChanged += new System.EventHandler(this.FilterValueChanged);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRunFilter.Location = new System.Drawing.Point(204, 550);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(77, 32);
            this._bRunFilter.TabIndex = 6;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bFilter_Click);
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bFilterClear.Location = new System.Drawing.Point(287, 550);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(77, 32);
            this._bFilterClear.TabIndex = 7;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bClear_Click);
            // 
            // _lRecordCount
            // 
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(180, 556);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(109, 20);
            this._lRecordCount.TabIndex = 56;
            this._lRecordCount.Text = "Record count:";
            // 
            // NCASScenesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1004, 590);
            this.Controls.Add(this._lRecordCount);
            this.Controls.Add(this._bFilterClear);
            this.Controls.Add(this._bRunFilter);
            this.Controls.Add(this._tbName);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._dgvData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NCASScenesForm";
            this.Text = "NCASScenesForm";
            this.Load += new System.EventHandler(this.NCASScenesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._dgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Contal.Cgp.Components.CgpDataGridView _dgvData;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _tbName;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Label _lRecordCount;
    }
}
