namespace Contal.Cgp.Client
{
    partial class LookupedCCUsForm
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
            this._bAdd = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._cbSelectUnselectAll = new System.Windows.Forms.CheckBox();
            this._pBack = new System.Windows.Forms.Panel();
            this._dgLookupedCcus = new System.Windows.Forms.DataGridView();
            this._pBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgLookupedCcus)).BeginInit();
            this.SuspendLayout();
            // 
            // _bAdd
            // 
            this._bAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd.Location = new System.Drawing.Point(457, 359);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(75, 23);
            this._bAdd.TabIndex = 0;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(539, 360);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 1;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _cbSelectUnselectAll
            // 
            this._cbSelectUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._cbSelectUnselectAll.AutoSize = true;
            this._cbSelectUnselectAll.Checked = true;
            this._cbSelectUnselectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbSelectUnselectAll.Location = new System.Drawing.Point(25, 363);
            this._cbSelectUnselectAll.Name = "_cbSelectUnselectAll";
            this._cbSelectUnselectAll.Size = new System.Drawing.Size(120, 17);
            this._cbSelectUnselectAll.TabIndex = 0;
            this._cbSelectUnselectAll.Text = "Select / unselect all";
            this._cbSelectUnselectAll.UseVisualStyleBackColor = true;
            this._cbSelectUnselectAll.CheckedChanged += new System.EventHandler(this._cbSelectUnselectAll_CheckedChanged);
            // 
            // _pBack
            // 
            this._pBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pBack.Controls.Add(this._cbSelectUnselectAll);
            this._pBack.Controls.Add(this._bAdd);
            this._pBack.Controls.Add(this._dgLookupedCcus);
            this._pBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pBack.Location = new System.Drawing.Point(0, 0);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(621, 390);
            this._pBack.TabIndex = 3;
            // 
            // _dgLookupedCcus
            // 
            this._dgLookupedCcus.AllowUserToAddRows = false;
            this._dgLookupedCcus.AllowUserToDeleteRows = false;
            this._dgLookupedCcus.AllowUserToResizeColumns = true;
            this._dgLookupedCcus.AllowUserToResizeRows = false;
            this._dgLookupedCcus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgLookupedCcus.Dock = System.Windows.Forms.DockStyle.Top;
            this._dgLookupedCcus.Location = new System.Drawing.Point(0, 0);
            this._dgLookupedCcus.MultiSelect = false;
            this._dgLookupedCcus.Name = "_dgLookupedCcus";
            this._dgLookupedCcus.RowHeadersVisible = false;
            this._dgLookupedCcus.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgLookupedCcus.Size = new System.Drawing.Size(619, 353);
            this._dgLookupedCcus.TabIndex = 0;
            // 
            // LookupedCCUsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(621, 390);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._pBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LookupedCCUsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LookupedCCUsForm";
            this.Shown += new System.EventHandler(this.LookupedCCUsForm_Shown);
            this._pBack.ResumeLayout(false);
            this._pBack.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgLookupedCcus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.CheckBox _cbSelectUnselectAll;
        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.DataGridView _dgLookupedCcus;
    }
}
