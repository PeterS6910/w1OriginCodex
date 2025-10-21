namespace Contal.Cgp.NCAS.Client
{
    partial class NCASActiveCardReaderForPersonForm
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
            this._dgValues = new System.Windows.Forms.DataGridView();
            this._pBack = new System.Windows.Forms.Panel();
            this._bRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).BeginInit();
            this._pBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgValues
            // 
            this._dgValues.AllowUserToAddRows = false;
            this._dgValues.AllowUserToDeleteRows = false;
            this._dgValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgValues.Location = new System.Drawing.Point(12, 12);
            this._dgValues.MultiSelect = false;
            this._dgValues.Name = "_dgValues";
            this._dgValues.ReadOnly = true;
            this._dgValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgValues.Size = new System.Drawing.Size(401, 275);
            this._dgValues.TabIndex = 90;
            this._dgValues.TabStop = false;
            this._dgValues.DoubleClick += new System.EventHandler(this._dgValues_DoubleClick);
            // 
            // _pBack
            // 
            this._pBack.AutoSize = true;
            this._pBack.Controls.Add(this._bRefresh);
            this._pBack.Controls.Add(this._dgValues);
            this._pBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pBack.Location = new System.Drawing.Point(0, 0);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(416, 328);
            this._pBack.TabIndex = 1;
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(333, 293);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(80, 22);
            this._bRefresh.TabIndex = 1;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // NCASActiveCardReaderForPersonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(416, 328);
            this.Controls.Add(this._pBack);
            this.Name = "NCASActiveCardReaderForPersonForm";
            this.Text = "NCASActiveCardReaderForPersonForm";
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).EndInit();
            this._pBack.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgValues;
        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.Button _bRefresh;
    }
}
