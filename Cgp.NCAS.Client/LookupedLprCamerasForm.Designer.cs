namespace Contal.Cgp.NCAS.Client
{
    partial class LookupedLprCamerasForm
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

        private void InitializeComponent()
        {
            this._pBack = new System.Windows.Forms.Panel();
            this._cbSelectUnselectAll = new System.Windows.Forms.CheckBox();
            this._bAdd = new System.Windows.Forms.Button();
            this._dgLookupedCameras = new System.Windows.Forms.DataGridView();
            this._bCancel = new System.Windows.Forms.Button();
            this._pBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgLookupedCameras)).BeginInit();
            this.SuspendLayout();
            // 
            // _pBack
            // 
            this._pBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pBack.Controls.Add(this._cbSelectUnselectAll);
            this._pBack.Controls.Add(this._bAdd);
            this._pBack.Controls.Add(this._dgLookupedCameras);
            this._pBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pBack.Location = new System.Drawing.Point(0, 0);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(900, 420);
            this._pBack.TabIndex = 0;
            // 
            // _cbSelectUnselectAll
            // 
            this._cbSelectUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._cbSelectUnselectAll.AutoSize = true;
            this._cbSelectUnselectAll.Checked = true;
            this._cbSelectUnselectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbSelectUnselectAll.Location = new System.Drawing.Point(12, 388);
            this._cbSelectUnselectAll.Name = "_cbSelectUnselectAll";
            this._cbSelectUnselectAll.Size = new System.Drawing.Size(152, 21);
            this._cbSelectUnselectAll.TabIndex = 1;
            this._cbSelectUnselectAll.Text = "Select / unselect all";
            this._cbSelectUnselectAll.UseVisualStyleBackColor = true;
            this._cbSelectUnselectAll.CheckedChanged += new System.EventHandler(this._cbSelectUnselectAll_CheckedChanged);
            // 
            // _bAdd
            // 
            this._bAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd.Location = new System.Drawing.Point(698, 384);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(90, 30);
            this._bAdd.TabIndex = 2;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _dgLookupedCameras
            // 
            this._dgLookupedCameras.AllowUserToAddRows = false;
            this._dgLookupedCameras.AllowUserToDeleteRows = false;
            this._dgLookupedCameras.AllowUserToResizeRows = false;
            this._dgLookupedCameras.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgLookupedCameras.Dock = System.Windows.Forms.DockStyle.Top;
            this._dgLookupedCameras.Location = new System.Drawing.Point(0, 0);
            this._dgLookupedCameras.MultiSelect = false;
            this._dgLookupedCameras.Name = "_dgLookupedCameras";
            this._dgLookupedCameras.RowHeadersVisible = false;
            this._dgLookupedCameras.RowTemplate.Height = 24;
            this._dgLookupedCameras.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgLookupedCameras.Size = new System.Drawing.Size(898, 372);
            this._dgLookupedCameras.TabIndex = 0;
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(794, 384);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 30);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // LookupedLprCamerasForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(900, 420);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._pBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LookupedLprCamerasForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LookupedLprCamerasForm";
            this.Shown += new System.EventHandler(this.LookupedLprCamerasForm_Shown);
            this._pBack.ResumeLayout(false);
            this._pBack.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgLookupedCameras)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.CheckBox _cbSelectUnselectAll;
        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.DataGridView _dgLookupedCameras;
        private System.Windows.Forms.Button _bCancel;
    }
}
