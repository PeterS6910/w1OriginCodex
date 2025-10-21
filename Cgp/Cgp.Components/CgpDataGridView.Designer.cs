namespace Contal.Cgp.Components
{
    partial class CgpDataGridView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._dgvData = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgvData
            // 
            this._dgvData.AllowUserToAddRows = false;
            this._dgvData.AllowUserToDeleteRows = false;
            this._dgvData.AllowUserToResizeRows = false;
            this._dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvData.Location = new System.Drawing.Point(0, 0);
            this._dgvData.Name = "_dgvData";
            this._dgvData.RowHeadersVisible = false;
            this._dgvData.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvData.Size = new System.Drawing.Size(323, 192);
            this._dgvData.TabIndex = 0;
            this._dgvData.Sorted += new System.EventHandler(this._dgvData_Sorted);
            this._dgvData.MouseClick += new System.Windows.Forms.MouseEventHandler(this._dgvData_MouseClick);
            // 
            // CgpDataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this._dgvData);
            this.DoubleBuffered = true;
            this.Name = "CgpDataGridView";
            this.Size = new System.Drawing.Size(323, 192);
            ((System.ComponentModel.ISupportInitialize)(this._dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgvData;
    }
}
