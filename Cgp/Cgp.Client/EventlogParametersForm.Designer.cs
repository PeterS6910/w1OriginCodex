namespace Contal.Cgp.Client
{
    partial class EventlogParametersForm
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
            this._pControl = new System.Windows.Forms.Panel();
            this._bCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this._ilbEventSource = new Contal.IwQuick.UI.ImageListBox();
            this._lEventlog = new System.Windows.Forms.Label();
            this._lELDescription = new System.Windows.Forms.Label();
            this._eELDescription = new System.Windows.Forms.TextBox();
            this._lELEventSource = new System.Windows.Forms.Label();
            this._lELCGPSource = new System.Windows.Forms.Label();
            this._eELCGPSource = new System.Windows.Forms.TextBox();
            this._lELDate = new System.Windows.Forms.Label();
            this._eELDate = new System.Windows.Forms.TextBox();
            this._lELType = new System.Windows.Forms.Label();
            this._eELType = new System.Windows.Forms.TextBox();
            this._cdgvData = new Contal.Cgp.Components.CgpDataGridView();
            this._pControl.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // _pControl
            // 
            this._pControl.Controls.Add(this._bCancel);
            this._pControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pControl.Location = new System.Drawing.Point(0, 479);
            this._pControl.Name = "_pControl";
            this._pControl.Size = new System.Drawing.Size(586, 37);
            this._pControl.TabIndex = 8;
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(499, 6);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 4;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._ilbEventSource);
            this.panel1.Controls.Add(this._lEventlog);
            this.panel1.Controls.Add(this._lELDescription);
            this.panel1.Controls.Add(this._eELDescription);
            this.panel1.Controls.Add(this._lELEventSource);
            this.panel1.Controls.Add(this._lELCGPSource);
            this.panel1.Controls.Add(this._eELCGPSource);
            this.panel1.Controls.Add(this._lELDate);
            this.panel1.Controls.Add(this._eELDate);
            this.panel1.Controls.Add(this._lELType);
            this.panel1.Controls.Add(this._eELType);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(586, 256);
            this.panel1.TabIndex = 10;
            // 
            // _ilbEventSource
            // 
            this._ilbEventSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ilbEventSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._ilbEventSource.FormattingEnabled = true;
            this._ilbEventSource.ImageList = null;
            this._ilbEventSource.ItemHeight = 16;
            this._ilbEventSource.Location = new System.Drawing.Point(309, 102);
            this._ilbEventSource.Name = "_ilbEventSource";
            this._ilbEventSource.SelectedItemObject = null;
            this._ilbEventSource.Size = new System.Drawing.Size(265, 148);
            this._ilbEventSource.TabIndex = 30;
            this._ilbEventSource.TabStop = false;
            this._ilbEventSource.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._ilbEventSource_MouseDoubleClick);
            // 
            // _lEventlog
            // 
            this._lEventlog.AutoSize = true;
            this._lEventlog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lEventlog.Location = new System.Drawing.Point(12, 9);
            this._lEventlog.Name = "_lEventlog";
            this._lEventlog.Size = new System.Drawing.Size(65, 13);
            this._lEventlog.TabIndex = 28;
            this._lEventlog.Text = "Event log:";
            // 
            // _lELDescription
            // 
            this._lELDescription.AutoSize = true;
            this._lELDescription.Location = new System.Drawing.Point(12, 86);
            this._lELDescription.Name = "_lELDescription";
            this._lELDescription.Size = new System.Drawing.Size(60, 13);
            this._lELDescription.TabIndex = 26;
            this._lELDescription.Text = "Description";
            // 
            // _eELDescription
            // 
            this._eELDescription.BackColor = System.Drawing.Color.White;
            this._eELDescription.Location = new System.Drawing.Point(12, 102);
            this._eELDescription.Multiline = true;
            this._eELDescription.Name = "_eELDescription";
            this._eELDescription.ReadOnly = true;
            this._eELDescription.Size = new System.Drawing.Size(291, 148);
            this._eELDescription.TabIndex = 3;
            // 
            // _lELEventSource
            // 
            this._lELEventSource.AutoSize = true;
            this._lELEventSource.Location = new System.Drawing.Point(306, 86);
            this._lELEventSource.Name = "_lELEventSource";
            this._lELEventSource.Size = new System.Drawing.Size(70, 13);
            this._lELEventSource.TabIndex = 22;
            this._lELEventSource.Text = "Event source";
            // 
            // _lELCGPSource
            // 
            this._lELCGPSource.AutoSize = true;
            this._lELCGPSource.Location = new System.Drawing.Point(285, 63);
            this._lELCGPSource.Name = "_lELCGPSource";
            this._lELCGPSource.Size = new System.Drawing.Size(64, 13);
            this._lELCGPSource.TabIndex = 20;
            this._lELCGPSource.Text = "CGP source";
            // 
            // _eELCGPSource
            // 
            this._eELCGPSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eELCGPSource.BackColor = System.Drawing.Color.White;
            this._eELCGPSource.Location = new System.Drawing.Point(373, 60);
            this._eELCGPSource.Name = "_eELCGPSource";
            this._eELCGPSource.ReadOnly = true;
            this._eELCGPSource.Size = new System.Drawing.Size(201, 20);
            this._eELCGPSource.TabIndex = 2;
            // 
            // _lELDate
            // 
            this._lELDate.AutoSize = true;
            this._lELDate.Location = new System.Drawing.Point(12, 60);
            this._lELDate.Name = "_lELDate";
            this._lELDate.Size = new System.Drawing.Size(30, 13);
            this._lELDate.TabIndex = 18;
            this._lELDate.Text = "Date";
            // 
            // _eELDate
            // 
            this._eELDate.BackColor = System.Drawing.Color.White;
            this._eELDate.Location = new System.Drawing.Point(100, 57);
            this._eELDate.Name = "_eELDate";
            this._eELDate.ReadOnly = true;
            this._eELDate.Size = new System.Drawing.Size(179, 20);
            this._eELDate.TabIndex = 1;
            // 
            // _lELType
            // 
            this._lELType.AutoSize = true;
            this._lELType.Location = new System.Drawing.Point(12, 34);
            this._lELType.Name = "_lELType";
            this._lELType.Size = new System.Drawing.Size(31, 13);
            this._lELType.TabIndex = 16;
            this._lELType.Text = "Type";
            // 
            // _eELType
            // 
            this._eELType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eELType.BackColor = System.Drawing.Color.White;
            this._eELType.Location = new System.Drawing.Point(100, 31);
            this._eELType.Name = "_eELType";
            this._eELType.ReadOnly = true;
            this._eELType.Size = new System.Drawing.Size(474, 20);
            this._eELType.TabIndex = 0;
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
            this._cdgvData.DataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.DataGrid.Location = new System.Drawing.Point(0, 0);
            this._cdgvData.DataGrid.Name = "_dgvData";
            this._cdgvData.DataGrid.ReadOnly = true;
            this._cdgvData.DataGrid.RowHeadersVisible = false;
            this._cdgvData.DataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this._cdgvData.DataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._cdgvData.DataGrid.Size = new System.Drawing.Size(586, 223);
            this._cdgvData.DataGrid.TabIndex = 0;
            this._cdgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cdgvData.ImageList = null;
            this._cdgvData.LocalizationHelper = null;
            this._cdgvData.Location = new System.Drawing.Point(0, 256);
            this._cdgvData.Name = "_cdgvData";
            this._cdgvData.Size = new System.Drawing.Size(586, 223);
            this._cdgvData.TabIndex = 11;
            // 
            // EventlogParametersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(586, 516);
            this.ControlBox = true;
            this.Controls.Add(this._cdgvData);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._pControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(602, 554);
            this.Name = "EventlogParametersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EventlogParametersForm";
            this.Enter += new System.EventHandler(this.EventlogParametersForm_Enter);
            this.Leave += new System.EventHandler(this.EventlogParametersForm_Leave);
            this._pControl.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._cdgvData.DataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pControl;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label _lELDescription;
        private System.Windows.Forms.TextBox _eELDescription;
        private System.Windows.Forms.Label _lELEventSource;
        private System.Windows.Forms.Label _lELCGPSource;
        private System.Windows.Forms.TextBox _eELCGPSource;
        private System.Windows.Forms.Label _lELDate;
        private System.Windows.Forms.TextBox _eELDate;
        private System.Windows.Forms.Label _lELType;
        private System.Windows.Forms.TextBox _eELType;
        private System.Windows.Forms.Label _lEventlog;
        private Contal.IwQuick.UI.ImageListBox _ilbEventSource;
        private Contal.Cgp.Components.CgpDataGridView _cdgvData;
    }
}