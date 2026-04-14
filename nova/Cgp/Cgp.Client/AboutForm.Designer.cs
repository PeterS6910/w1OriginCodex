namespace Contal.Cgp.Client
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this._pbLoading = new System.Windows.Forms.PictureBox();
            this._bOk = new System.Windows.Forms.Button();
            this._lvVersions = new System.Windows.Forms.ListView();
            this._pbLogo = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this._pbLoading);
            this.panel1.Controls.Add(this._bOk);
            this.panel1.Controls.Add(this._lvVersions);
            this.panel1.Controls.Add(this._pbLogo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(460, 147);
            this.panel1.TabIndex = 7;
            // 
            // _pbLoading
            // 
            this._pbLoading.Image = global::Contal.Cgp.Client.Properties.Resources.splash_loading;
            this._pbLoading.Location = new System.Drawing.Point(319, 111);
            this._pbLoading.Name = "_pbLoading";
            this._pbLoading.Size = new System.Drawing.Size(100, 13);
            this._pbLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pbLoading.TabIndex = 7;
            this._pbLoading.TabStop = false;
            this._pbLoading.Visible = false;
            // 
            // _bOk
            // 
            this._bOk.Location = new System.Drawing.Point(339, 111);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(100, 24);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _lvVersions
            // 
            this._lvVersions.AllowColumnReorder = true;
            this._lvVersions.BackColor = System.Drawing.Color.White;
            this._lvVersions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lvVersions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lvVersions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._lvVersions.Location = new System.Drawing.Point(19, 85);
            this._lvVersions.Name = "_lvVersions";
            this._lvVersions.Size = new System.Drawing.Size(420, 49);
            this._lvVersions.TabIndex = 5;
            this._lvVersions.UseCompatibleStateImageBehavior = false;
            this._lvVersions.View = System.Windows.Forms.View.Details;
            // 
            // _pbLogo
            // 
            this._pbLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_pbLogo.BackgroundImage")));
            this._pbLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._pbLogo.Location = new System.Drawing.Point(19, 11);
            this._pbLogo.Name = "_pbLogo";
            this._pbLogo.Size = new System.Drawing.Size(420, 68);
            this._pbLogo.TabIndex = 6;
            this._pbLogo.TabStop = false;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(460, 147);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contal Nova™";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.AboutForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AboutForm_FormClosing);
            this.Shown += new System.EventHandler(this.AboutForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.ListView _lvVersions;
        private System.Windows.Forms.PictureBox _pbLogo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox _pbLoading;
    }
}