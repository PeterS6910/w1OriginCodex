namespace Contal.Cgp.Client
{
    partial class PluginDashboardForm
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
            this._pluginsLV = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // _pluginsLV
            // 
            this._pluginsLV.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pluginsLV.Location = new System.Drawing.Point(0, 0);
            this._pluginsLV.Name = "_pluginsLV";
            this._pluginsLV.Size = new System.Drawing.Size(983, 516);
            this._pluginsLV.TabIndex = 0;
            this._pluginsLV.UseCompatibleStateImageBehavior = false;
            this._pluginsLV.DoubleClick += new System.EventHandler(this._pluginsLV_DoubleClick);
            // 
            // PluginDashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(983, 516);
            this.ControlBox = false;
            this.Controls.Add(this._pluginsLV);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "PluginDashboardForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Plugin dashboard";
            this.Shown += new System.EventHandler(this.PluginDashboardForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView _pluginsLV;
    }
}