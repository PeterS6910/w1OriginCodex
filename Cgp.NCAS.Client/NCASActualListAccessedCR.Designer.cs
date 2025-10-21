namespace Contal.Cgp.NCAS.Client
{
    partial class NCASActualListAccessedCR
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
            this._tvActualAccessObjects = new System.Windows.Forms.TreeView();
            this._bRefresh = new System.Windows.Forms.Button();
            this._pBack = new System.Windows.Forms.Panel();
            this._lLastRefreshDateTime = new System.Windows.Forms.Label();
            this._pBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tvActualAccessObjects
            // 
            this._tvActualAccessObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tvActualAccessObjects.Location = new System.Drawing.Point(3, 3);
            this._tvActualAccessObjects.Name = "_tvActualAccessObjects";
            this._tvActualAccessObjects.Size = new System.Drawing.Size(591, 428);
            this._tvActualAccessObjects.TabIndex = 0;
            this._tvActualAccessObjects.Tag = "Reference";
            this._tvActualAccessObjects.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._tvActualCR_NodeMouseDoubleClick);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(519, 456);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 1;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _pBack
            // 
            this._pBack.AutoSize = true;
            this._pBack.Controls.Add(this._lLastRefreshDateTime);
            this._pBack.Controls.Add(this._tvActualAccessObjects);
            this._pBack.Controls.Add(this._bRefresh);
            this._pBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pBack.Location = new System.Drawing.Point(0, 0);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(597, 482);
            this._pBack.TabIndex = 2;
            // 
            // _lLastRefreshDateTime
            // 
            this._lLastRefreshDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lLastRefreshDateTime.Location = new System.Drawing.Point(3, 434);
            this._lLastRefreshDateTime.Name = "_lLastRefreshDateTime";
            this._lLastRefreshDateTime.Size = new System.Drawing.Size(591, 13);
            this._lLastRefreshDateTime.TabIndex = 2;
            this._lLastRefreshDateTime.Text = "Last refresh";
            this._lLastRefreshDateTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // NCASActualListAccessedCR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 482);
           
            this.Controls.Add(this._pBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "NCASActualListAccessedCR";
            this.Text = "NCASActualListAccessedCR";
            this._pBack.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _tvActualAccessObjects;
        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.Label _lLastRefreshDateTime;
    }
}