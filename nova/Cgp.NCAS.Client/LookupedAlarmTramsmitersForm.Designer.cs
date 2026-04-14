namespace Contal.Cgp.NCAS.Client
{
    partial class LookupedAlarmTramsmitersForm
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
            this._pBack = new System.Windows.Forms.Panel();
            this._cbSelectUnselectAll = new System.Windows.Forms.CheckBox();
            this._bAdd = new System.Windows.Forms.Button();
            this._dgLookupedTransmitters = new System.Windows.Forms.DataGridView();
            this._bCancel = new System.Windows.Forms.Button();
            this._pBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgLookupedTransmitters)).BeginInit();
            this.SuspendLayout();
            // 
            // _pBack
            // 
            this._pBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pBack.Controls.Add(this._cbSelectUnselectAll);
            this._pBack.Controls.Add(this._bAdd);
            this._pBack.Controls.Add(this._dgLookupedTransmitters);
            this._pBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pBack.Location = new System.Drawing.Point(0, 0);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(621, 390);
            this._pBack.TabIndex = 5;
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
            // _dgLookupedTransmitters
            // 
            this._dgLookupedTransmitters.AllowUserToAddRows = false;
            this._dgLookupedTransmitters.AllowUserToDeleteRows = false;
            this._dgLookupedTransmitters.AllowUserToResizeRows = false;
            this._dgLookupedTransmitters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgLookupedTransmitters.Dock = System.Windows.Forms.DockStyle.Top;
            this._dgLookupedTransmitters.Location = new System.Drawing.Point(0, 0);
            this._dgLookupedTransmitters.MultiSelect = false;
            this._dgLookupedTransmitters.Name = "_dgLookupedTransmitters";
            this._dgLookupedTransmitters.RowHeadersVisible = false;
            this._dgLookupedTransmitters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgLookupedTransmitters.Size = new System.Drawing.Size(619, 353);
            this._dgLookupedTransmitters.TabIndex = 0;
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(539, 360);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 4;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // LookupedAlarmTramsmitersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(621, 390);
            this.ControlBox = false;
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._pBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "LookupedAlarmTramsmitersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LookupedAlarmTramsmitersForm";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LookupedTramsmitersForm_KeyUp);
            this._pBack.ResumeLayout(false);
            this._pBack.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgLookupedTransmitters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.CheckBox _cbSelectUnselectAll;
        private System.Windows.Forms.Button _bAdd;
        private System.Windows.Forms.DataGridView _dgLookupedTransmitters;
        private System.Windows.Forms.Button _bCancel;
    }
}
