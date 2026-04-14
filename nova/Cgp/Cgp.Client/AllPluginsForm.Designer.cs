namespace Contal.Cgp.Client
{
    partial class AllPluginsForm
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
            this._lvCgp = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // _lvCgp
            // 
            this._lvCgp.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lvCgp.Location = new System.Drawing.Point(0, 0);
            this._lvCgp.Name = "_lvCgp";
            this._lvCgp.Size = new System.Drawing.Size(551, 429);
            this._lvCgp.TabIndex = 0;
            this._lvCgp.UseCompatibleStateImageBehavior = false;
            this._lvCgp.MultiSelect = false;
            // 
            // AllPluginsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(551, 429);
            this.Controls.Add(this._lvCgp);
            this.Name = "AllPluginsForm";
            this.Text = "Plugins";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView _lvCgp;
    }
}