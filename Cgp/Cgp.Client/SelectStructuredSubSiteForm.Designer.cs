using Contal.IwQuick.PlatformPC.UI;

namespace Contal.Cgp.Client
{
    partial class SelectStructuredSubSiteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectStructuredSubSiteForm));
            this._lSubSites = new System.Windows.Forms.Label();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._ehWpfCheckBoxTreeview = new System.Windows.Forms.Integration.ElementHost();
            this._wpfCheckBoxesTreeview = new Contal.IwQuick.PlatformPC.UI.WpfCheckBoxesTreeview();
            this.SuspendLayout();
            // 
            // _lSubSites
            // 
            this._lSubSites.AutoSize = true;
            this._lSubSites.Location = new System.Drawing.Point(12, 9);
            this._lSubSites.Name = "_lSubSites";
            this._lSubSites.Size = new System.Drawing.Size(50, 13);
            this._lSubSites.TabIndex = 1;
            this._lSubSites.Text = "Sub-sites";
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bOk.Location = new System.Drawing.Point(348, 265);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 6;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(429, 265);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _ehWpfCheckBoxTreeview
            // 
            this._ehWpfCheckBoxTreeview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ehWpfCheckBoxTreeview.Location = new System.Drawing.Point(12, 25);
            this._ehWpfCheckBoxTreeview.Name = "_ehWpfCheckBoxTreeview";
            this._ehWpfCheckBoxTreeview.Size = new System.Drawing.Size(492, 234);
            this._ehWpfCheckBoxTreeview.TabIndex = 8;
            this._ehWpfCheckBoxTreeview.Text = "_ehWpfCheckBoxTreeview";
            this._ehWpfCheckBoxTreeview.Child = this._wpfCheckBoxesTreeview;
            // 
            // SelectStructuredSubSiteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 300);
            this.Controls.Add(this._ehWpfCheckBoxTreeview);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._lSubSites);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectStructuredSubSiteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Selection of structured sub-sites";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lSubSites;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Integration.ElementHost _ehWpfCheckBoxTreeview;
        private WpfCheckBoxesTreeview _wpfCheckBoxesTreeview;
    }
}