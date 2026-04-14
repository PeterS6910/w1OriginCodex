namespace Contal.Cgp.NCAS.Client
{
    partial class EventlogsDoorEnvironmentEditForm
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
            this._bRefresh = new System.Windows.Forms.Button();
            this._dgEventlogs = new System.Windows.Forms.DataGridView();
            this._dgcDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._dgcType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._dgcDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._dgEventlogs)).BeginInit();
            this.SuspendLayout();
            // 
            // _bRefresh
            // 
            this._bRefresh.Location = new System.Drawing.Point(3, 3);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(90, 32);
            this._bRefresh.TabIndex = 0;
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _dgEventlogs
            // 
            this._dgEventlogs.AllowUserToAddRows = false;
            this._dgEventlogs.AllowUserToDeleteRows = false;
            this._dgEventlogs.AllowUserToResizeRows = false;
            this._dgEventlogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dgEventlogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgEventlogs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._dgcDateTime,
            this._dgcType,
            this._dgcDescription});
            this._dgEventlogs.Location = new System.Drawing.Point(3, 41);
            this._dgEventlogs.MultiSelect = false;
            this._dgEventlogs.Name = "_dgEventlogs";
            this._dgEventlogs.ReadOnly = true;
            this._dgEventlogs.RowHeadersVisible = false;
            this._dgEventlogs.RowHeadersWidth = 51;
            this._dgEventlogs.RowTemplate.Height = 24;
            this._dgEventlogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgEventlogs.Size = new System.Drawing.Size(843, 385);
            this._dgEventlogs.TabIndex = 1;
            this._dgEventlogs.TabStop = false;
            // 
            // _dgcDateTime
            // 
            this._dgcDateTime.DataPropertyName = "EventlogDateTime";
            this._dgcDateTime.MinimumWidth = 6;
            this._dgcDateTime.Name = "_dgcDateTime";
            this._dgcDateTime.ReadOnly = true;
            this._dgcDateTime.Width = 125;
            // 
            // _dgcType
            // 
            this._dgcType.DataPropertyName = "Type";
            this._dgcType.MinimumWidth = 6;
            this._dgcType.Name = "_dgcType";
            this._dgcType.ReadOnly = true;
            this._dgcType.Width = 125;
            // 
            // _dgcDescription
            // 
            this._dgcDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._dgcDescription.DataPropertyName = "EventSources";
            this._dgcDescription.MinimumWidth = 6;
            this._dgcDescription.Name = "EventSources";
            this._dgcDescription.ReadOnly = true;
            // 
            // EventlogsDoorEnvironmentEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dgEventlogs);
            this.Controls.Add(this._bRefresh);
            this.Name = "EventlogsDoorEnvironmentEditForm";
            this.Size = new System.Drawing.Size(849, 432);
            ((System.ComponentModel.ISupportInitialize)(this._dgEventlogs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.DataGridView _dgEventlogs;
        private System.Windows.Forms.DataGridViewTextBoxColumn _dgcDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn _dgcType;
        private System.Windows.Forms.DataGridViewTextBoxColumn _dgcDescription;
    }
}
