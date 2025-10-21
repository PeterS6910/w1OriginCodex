namespace Contal.Cgp.Client
{
    partial class ClientUpgradeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientUpgradeForm));
            this._pbUpgradeProgress = new System.Windows.Forms.ProgressBar();
            this._lUpgradeStage = new System.Windows.Forms.Label();
            this._bStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _pbUpgradeProgress
            // 
            this._pbUpgradeProgress.Location = new System.Drawing.Point(12, 12);
            this._pbUpgradeProgress.Name = "_pbUpgradeProgress";
            this._pbUpgradeProgress.Size = new System.Drawing.Size(486, 14);
            this._pbUpgradeProgress.TabIndex = 1;
            // 
            // _lUpgradeStage
            // 
            this._lUpgradeStage.AutoSize = true;
            this._lUpgradeStage.Location = new System.Drawing.Point(12, 29);
            this._lUpgradeStage.Name = "_lUpgradeStage";
            this._lUpgradeStage.Size = new System.Drawing.Size(65, 13);
            this._lUpgradeStage.TabIndex = 2;
            this._lUpgradeStage.Text = "...transfering";
            // 
            // _bStop
            // 
            this._bStop.Location = new System.Drawing.Point(423, 32);
            this._bStop.Name = "_bStop";
            this._bStop.Size = new System.Drawing.Size(75, 23);
            this._bStop.TabIndex = 0;
            this._bStop.Text = "Stop";
            this._bStop.UseVisualStyleBackColor = true;
            this._bStop.Click += new System.EventHandler(this._bStop_Click);
            // 
            // ClientUpgradeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(510, 62);
            this.Controls.Add(this._bStop);
            this.Controls.Add(this._lUpgradeStage);
            this.Controls.Add(this._pbUpgradeProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClientUpgradeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ClientUpgradeForm";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.ClientUpgradeForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientUpgradeForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar _pbUpgradeProgress;
        private System.Windows.Forms.Label _lUpgradeStage;
        private System.Windows.Forms.Button _bStop;
    }
}