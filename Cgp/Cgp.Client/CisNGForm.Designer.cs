namespace Contal.Cgp.Client
{
    partial class CisNGForm
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
            this.panel3 = new System.Windows.Forms.Panel();
            this._dgValues = new System.Windows.Forms.DataGridView();
            this._pFilter = new System.Windows.Forms.Panel();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._lIpAddressFilter = new System.Windows.Forms.Label();
            this._lNameFilter = new System.Windows.Forms.Label();
            this._eIpAddressFilter = new System.Windows.Forms.TextBox();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._pControl = new System.Windows.Forms.Panel();
            this._bDelete = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._bInsert = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).BeginInit();
            this._pFilter.SuspendLayout();
            this._pControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._dgValues);
            this.panel3.Controls.Add(this._pFilter);
            this.panel3.Controls.Add(this._pControl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(758, 385);
            this.panel3.TabIndex = 3;
            // 
            // _dgValues
            // 
            this._dgValues.AllowUserToAddRows = false;
            this._dgValues.AllowUserToDeleteRows = false;
            this._dgValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgValues.Location = new System.Drawing.Point(0, 0);
            this._dgValues.MultiSelect = false;
            this._dgValues.Name = "_dgValues";
            this._dgValues.ReadOnly = true;
            this._dgValues.RowTemplate.Height = 24;
            this._dgValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgValues.Size = new System.Drawing.Size(758, 298);
            this._dgValues.TabIndex = 0;
            this._dgValues.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._dgValues_MouseDoubleClick);
            this._dgValues.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this._dgValues_DataError);
            this._dgValues.Paint += new System.Windows.Forms.PaintEventHandler(this._dgValues_Paint);
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._lIpAddressFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._eIpAddressFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 298);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(758, 50);
            this._pFilter.TabIndex = 1;
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Location = new System.Drawing.Point(336, 19);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(75, 23);
            this._bFilterClear.TabIndex = 9;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this.FilterClearClick);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Location = new System.Drawing.Point(255, 19);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(75, 23);
            this._bRunFilter.TabIndex = 8;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this.RunFilterClick);
            // 
            // _lIpAddressFilter
            // 
            this._lIpAddressFilter.AutoSize = true;
            this._lIpAddressFilter.Location = new System.Drawing.Point(132, 5);
            this._lIpAddressFilter.Name = "_lIpAddressFilter";
            this._lIpAddressFilter.Size = new System.Drawing.Size(58, 13);
            this._lIpAddressFilter.TabIndex = 2;
            this._lIpAddressFilter.Text = "IP Address";
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Location = new System.Drawing.Point(8, 5);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Size = new System.Drawing.Size(35, 13);
            this._lNameFilter.TabIndex = 0;
            this._lNameFilter.Text = "Name";
            // 
            // _eIpAddressFilter
            // 
            this._eIpAddressFilter.Location = new System.Drawing.Point(135, 21);
            this._eIpAddressFilter.Name = "_eIpAddressFilter";
            this._eIpAddressFilter.Size = new System.Drawing.Size(114, 20);
            this._eIpAddressFilter.TabIndex = 2;
            // 
            // _eNameFilter
            // 
            this._eNameFilter.Location = new System.Drawing.Point(8, 21);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Size = new System.Drawing.Size(121, 20);
            this._eNameFilter.TabIndex = 1;
            // 
            // _pControl
            // 
            this._pControl.Controls.Add(this._bDelete);
            this._pControl.Controls.Add(this._bEdit);
            this._pControl.Controls.Add(this._bInsert);
            this._pControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pControl.Location = new System.Drawing.Point(0, 348);
            this._pControl.Name = "_pControl";
            this._pControl.Size = new System.Drawing.Size(758, 37);
            this._pControl.TabIndex = 0;
            // 
            // _bDelete
            // 
            this._bDelete.Location = new System.Drawing.Point(174, 6);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(75, 23);
            this._bDelete.TabIndex = 2;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this.DeleteClick);
            // 
            // _bEdit
            // 
            this._bEdit.Location = new System.Drawing.Point(93, 6);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 1;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this.EditClick);
            // 
            // _bInsert
            // 
            this._bInsert.Location = new System.Drawing.Point(12, 6);
            this._bInsert.Name = "_bInsert";
            this._bInsert.Size = new System.Drawing.Size(75, 23);
            this._bInsert.TabIndex = 0;
            this._bInsert.Text = "Insert";
            this._bInsert.UseVisualStyleBackColor = true;
            this._bInsert.Click += new System.EventHandler(this.InsertClick);
            // 
            // CisNGForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 385);
            this.Controls.Add(this.panel3);
            this.Name = "CisNGForm";
            this.Text = "CisNGForm";
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).EndInit();
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this._pControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView _dgValues;
        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lIpAddressFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eIpAddressFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Panel _pControl;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bInsert;
    }
}