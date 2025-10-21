namespace Contal.Cgp.Client
{
    partial class ObjectsSearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectsSearchForm));
            this._gbSearch = new System.Windows.Forms.GroupBox();
            this._lSearchType = new System.Windows.Forms.Label();
            this._cbSearchType = new System.Windows.Forms.ComboBox();
            this._cbObjectType = new System.Windows.Forms.ComboBox();
            this._bSearch = new System.Windows.Forms.Button();
            this._eNameParams = new System.Windows.Forms.TextBox();
            this._lName = new System.Windows.Forms.Label();
            this._lObjectType = new System.Windows.Forms.Label();
            this._pTop = new System.Windows.Forms.Panel();
            this._bDockUndock = new System.Windows.Forms.Button();
            this._pFill = new System.Windows.Forms.Panel();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._gbSearch.SuspendLayout();
            this._pTop.SuspendLayout();
            this._pFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _gbSearch
            // 
            this._gbSearch.Controls.Add(this._lSearchType);
            this._gbSearch.Controls.Add(this._cbSearchType);
            this._gbSearch.Controls.Add(this._cbObjectType);
            this._gbSearch.Controls.Add(this._bSearch);
            this._gbSearch.Controls.Add(this._eNameParams);
            this._gbSearch.Controls.Add(this._lName);
            this._gbSearch.Controls.Add(this._lObjectType);
            this._gbSearch.Location = new System.Drawing.Point(12, 12);
            this._gbSearch.Name = "_gbSearch";
            this._gbSearch.Size = new System.Drawing.Size(693, 62);
            this._gbSearch.TabIndex = 0;
            this._gbSearch.TabStop = false;
            this._gbSearch.Text = "Search";
            // 
            // _lSearchType
            // 
            this._lSearchType.AutoSize = true;
            this._lSearchType.Location = new System.Drawing.Point(6, 16);
            this._lSearchType.Name = "_lSearchType";
            this._lSearchType.Size = new System.Drawing.Size(64, 13);
            this._lSearchType.TabIndex = 0;
            this._lSearchType.Text = "Search type";
            // 
            // _cbSearchType
            // 
            this._cbSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSearchType.FormattingEnabled = true;
            this._cbSearchType.Location = new System.Drawing.Point(9, 31);
            this._cbSearchType.Name = "_cbSearchType";
            this._cbSearchType.Size = new System.Drawing.Size(243, 21);
            this._cbSearchType.TabIndex = 0;
            this._cbSearchType.SelectedIndexChanged += new System.EventHandler(this._cbSearchType_SelectedIndexChanged);
            // 
            // _cbObjectType
            // 
            this._cbObjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbObjectType.FormattingEnabled = true;
            this._cbObjectType.Location = new System.Drawing.Point(398, 32);
            this._cbObjectType.Name = "_cbObjectType";
            this._cbObjectType.Size = new System.Drawing.Size(179, 21);
            this._cbObjectType.TabIndex = 2;
            // 
            // _bSearch
            // 
            this._bSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bSearch.Location = new System.Drawing.Point(612, 31);
            this._bSearch.Name = "_bSearch";
            this._bSearch.Size = new System.Drawing.Size(75, 23);
            this._bSearch.TabIndex = 3;
            this._bSearch.Text = "Search";
            this._bSearch.UseVisualStyleBackColor = true;
            this._bSearch.Click += new System.EventHandler(this._bSearch_Click);
            // 
            // _eNameParams
            // 
            this._eNameParams.Location = new System.Drawing.Point(258, 32);
            this._eNameParams.Name = "_eNameParams";
            this._eNameParams.Size = new System.Drawing.Size(134, 20);
            this._eNameParams.TabIndex = 1;
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(255, 16);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 2;
            this._lName.Text = "Name";
            // 
            // _lObjectType
            // 
            this._lObjectType.AutoSize = true;
            this._lObjectType.Location = new System.Drawing.Point(395, 16);
            this._lObjectType.Name = "_lObjectType";
            this._lObjectType.Size = new System.Drawing.Size(61, 13);
            this._lObjectType.TabIndex = 4;
            this._lObjectType.Text = "Object type";
            // 
            // _pTop
            // 
            this._pTop.Controls.Add(this._bDockUndock);
            this._pTop.Controls.Add(this._gbSearch);
            this._pTop.Dock = System.Windows.Forms.DockStyle.Top;
            this._pTop.Location = new System.Drawing.Point(0, 0);
            this._pTop.Name = "_pTop";
            this._pTop.Size = new System.Drawing.Size(798, 84);
            this._pTop.TabIndex = 0;
            // 
            // _bDockUndock
            // 
            this._bDockUndock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bDockUndock.Location = new System.Drawing.Point(711, 12);
            this._bDockUndock.Name = "_bDockUndock";
            this._bDockUndock.Size = new System.Drawing.Size(75, 23);
            this._bDockUndock.TabIndex = 4;
            this._bDockUndock.Text = "Dock";
            this._bDockUndock.UseVisualStyleBackColor = true;
            this._bDockUndock.Click += new System.EventHandler(this.bDockUndockClick);
            // 
            // _pFill
            // 
            this._pFill.Controls.Add(this._cdgvData);
            this._pFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pFill.Location = new System.Drawing.Point(0, 84);
            this._pFill.Name = "_pFill";
            this._pFill.Size = new System.Drawing.Size(798, 453);
            this._pFill.TabIndex = 4;
            // 
            // _cdgvData
            // 
            this._cdgvData.AllwaysRefreshOrder = false;
            // 
            // 
            // 
            this._cdgvData.DataGrid.AllowUserToAddRows = false;
            this._cdgvData.DataGrid.AllowUserToDeleteRows = false;
            this._cdgvData.DataGrid.AllowUserToResizeRows = false;
            this._cdgvData.DataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._cdgvData.DataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(798, 453);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.ImageList = null;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(798, 453);
            this._cdgvData.TabIndex = 0;
            // 
            // ObjectsSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(798, 537);
            this.Controls.Add(this._pFill);
            this.Controls.Add(this._pTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ObjectsSearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ObjectsSearchForm";
            this.Activated += new System.EventHandler(this.ObjectsSearchForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectsSearchForm_FormClosing);
            this._gbSearch.ResumeLayout(false);
            this._gbSearch.PerformLayout();
            this._pTop.ResumeLayout(false);
            this._pFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _gbSearch;
        private System.Windows.Forms.Button _bSearch;
        private System.Windows.Forms.ComboBox _cbObjectType;
        private System.Windows.Forms.TextBox _eNameParams;
        private System.Windows.Forms.Label _lObjectType;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.Panel _pTop;
        private System.Windows.Forms.Panel _pFill;
        private System.Windows.Forms.Button _bDockUndock;
        private System.Windows.Forms.ComboBox _cbSearchType;
        private System.Windows.Forms.Label _lSearchType;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
    }
}