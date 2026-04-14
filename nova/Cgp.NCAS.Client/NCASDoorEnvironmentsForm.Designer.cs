using System.Security.AccessControl;
using System.Windows.Forms;

namespace Contal.Cgp.NCAS.Client
{
    partial class NCASDoorEnvironmentsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASDoorEnvironmentsForm));
            this._pFilter = new System.Windows.Forms.Panel();
            this._lRecordCount = new System.Windows.Forms.Label();
            this._lControlUnit = new System.Windows.Forms.Label();
            this._cbControlUnit = new System.Windows.Forms.ComboBox();
            this._lNameFilter = new System.Windows.Forms.Label();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._chbShowOnlyConfigured = new System.Windows.Forms.CheckBox();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._lRecordCount);
            this._pFilter.Controls.Add(this._lControlUnit);
            this._pFilter.Controls.Add(this._cbControlUnit);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._chbShowOnlyConfigured);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 379);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(813, 70);
            this._pFilter.TabIndex = 24;
            // 
            // _lRecordCount
            // 
            this._lRecordCount.AutoSize = true;
            this._lRecordCount.Location = new System.Drawing.Point(321, 6);
            this._lRecordCount.Name = "_lRecordCount";
            this._lRecordCount.Size = new System.Drawing.Size(109, 20);
            this._lRecordCount.TabIndex = 49;
            this._lRecordCount.Text = "Record count:";
            // 
            // _lControlUnit
            // 
            this._lControlUnit.AutoSize = true;
            this._lControlUnit.Location = new System.Drawing.Point(139, 6);
            this._lControlUnit.Name = "_lControlUnit";
            this._lControlUnit.Size = new System.Drawing.Size(90, 20);
            this._lControlUnit.TabIndex = 5;
            this._lControlUnit.Text = "Control unit";
            // 
            // _cbControlUnit
            // 
            this._cbControlUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbControlUnit.FormattingEnabled = true;
            this._cbControlUnit.Location = new System.Drawing.Point(137, 30);
            this._cbControlUnit.Name = "_cbControlUnit";
            this._cbControlUnit.Size = new System.Drawing.Size(161, 28);
            this._cbControlUnit.TabIndex = 1;
            this._cbControlUnit.SelectedIndexChanged += new System.EventHandler(this._cbControlUnit_SelectedIndexChanged);
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Location = new System.Drawing.Point(12, 6);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Size = new System.Drawing.Size(51, 20);
            this._lNameFilter.TabIndex = 0;
            this._lNameFilter.Text = "Name";
            // 
            // _eNameFilter
            // 
            this._eNameFilter.Location = new System.Drawing.Point(12, 30);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Size = new System.Drawing.Size(119, 26);
            this._eNameFilter.TabIndex = 0;
            this._eNameFilter.TextChanged += new System.EventHandler(this.FilterValueChanged);
            this._eNameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Location = new System.Drawing.Point(385, 28);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(75, 32);
            this._bFilterClear.TabIndex = 3;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Location = new System.Drawing.Point(304, 28);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(75, 32);
            this._bRunFilter.TabIndex = 2;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _chbShowOnlyConfigured
            // 
            this._chbShowOnlyConfigured.Location = new System.Drawing.Point(476, 34);
            this._chbShowOnlyConfigured.Name = "_chbShowOnlyConfigured";
            this._chbShowOnlyConfigured.Size = new System.Drawing.Size(250, 25);
            this._chbShowOnlyConfigured.TabIndex = 1;
            this._chbShowOnlyConfigured.Text = "Show only configured";
            this._chbShowOnlyConfigured.UseVisualStyleBackColor = true;
            this._chbShowOnlyConfigured.CheckedChanged += new System.EventHandler(this._chbShowOnlyConfigured_CheckedChanged);
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
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(813, 379);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.DefaultSortColumnName = null;
            this._cdgvData.DefaultSortDirection = System.ComponentModel.ListSortDirection.Ascending;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(813, 379);
            this._cdgvData.TabIndex = 25;
            // 
            // NCASDoorEnvironmentsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(813, 449);
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this._pFilter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NCASDoorEnvironmentsForm";
            this.Text = "NCASDoorEnvironmentsForm";
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.CheckBox _chbShowOnlyConfigured;
        private System.Windows.Forms.ComboBox _cbControlUnit;
        private System.Windows.Forms.Label _lControlUnit;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
        private Label _lRecordCount;
    }
}
