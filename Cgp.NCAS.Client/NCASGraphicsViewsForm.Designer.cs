namespace Contal.Cgp.NCAS.Client
{
    partial class NCASGraphicsViewsForm
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
            this._dgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._tbName = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
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
            this._dgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._dgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._dgvData.DataGrid.Name = "_dgvData";
            this._dgvData.DataGrid.ReadOnly = true;
            this._dgvData.DataGrid.RowHeadersVisible = false;
            this._dgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this._dgvData.DataGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._dgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvData.DataGrid.Size = new System.Drawing.Size(951, 400);
            this._dgvData.DataGrid.TabIndex = 0;
            this._dgvData.LocalizationHelper = null;
            this._dgvData.Location = new System.Drawing.Point(0, 0);
            this._dgvData.Name = "_dgvData";
            this._dgvData.Size = new System.Drawing.Size(951, 400);
            this._dgvData.TabIndex = 2;
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bFilterClear.Location = new System.Drawing.Point(258, 406);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(77, 26);
            this._bFilterClear.TabIndex = 14;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRunFilter.Location = new System.Drawing.Point(175, 406);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(77, 26);
            this._bRunFilter.TabIndex = 13;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bFilter_Click);
            // 
            // _tbName
            // 
            this._tbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._tbName.Location = new System.Drawing.Point(40, 409);
            this._tbName.Name = "_tbName";
            this._tbName.Size = new System.Drawing.Size(129, 20);
            this._tbName.TabIndex = 12;
            // 
            // _lName
            // 
            this._lName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(1, 411);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 11;
            this._lName.Text = "Name";
            // 
            // NCASGraphicsViewsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(953, 435);
            this.Controls.Add(this._bFilterClear);
            this.Controls.Add(this._bRunFilter);
            this.Controls.Add(this._tbName);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._dgvData);
            this.Name = "NCASGraphicsViewsForm";
            this.Text = "NCASGraphicsViewsForm";
            ((System.ComponentModel.ISupportInitialize)(this._dgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Contal.Cgp.Components.CgpDataGridView _dgvData;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.TextBox _tbName;
        private System.Windows.Forms.Label _lName;
    }
}
