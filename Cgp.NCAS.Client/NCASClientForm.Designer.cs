namespace Contal.Cgp.NCAS.Client
{
    partial class NCASClientForm
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
            this._lvNcasPlugins = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // _lvNcasPlugins
            // 
            this._lvNcasPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lvNcasPlugins.Location = new System.Drawing.Point(0, 0);
            this._lvNcasPlugins.Name = "_lvNcasPlugins";
            this._lvNcasPlugins.Size = new System.Drawing.Size(443, 349);
            this._lvNcasPlugins.TabIndex = 0;
            this._lvNcasPlugins.UseCompatibleStateImageBehavior = false;
            // 
            // NCASClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(443, 349);
            this.Controls.Add(this._lvNcasPlugins);
            this.Name = "NCASClientForm";
            this.Text = "NCASClientForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView _lvNcasPlugins;



    }
}
