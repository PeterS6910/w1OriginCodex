namespace Contal.Cgp.Client
{
    partial class LoadingForm
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
            this._pictureBoxLoading = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // _pictureBoxLoading
            // 
            this._pictureBoxLoading.Image = global::Contal.Cgp.Client.Properties.Resources.form_loading;
            this._pictureBoxLoading.Location = new System.Drawing.Point(0, 0);
            this._pictureBoxLoading.Name = "_pictureBoxLoading";
            this._pictureBoxLoading.Size = new System.Drawing.Size(24, 24);
            this._pictureBoxLoading.TabIndex = 0;
            this._pictureBoxLoading.TabStop = false;
            this._pictureBoxLoading.Click += new System.EventHandler(this._pictureBoxLoading_Click);
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(24, 24);
            this.Controls.Add(this._pictureBoxLoading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(24, 24);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(24, 24);
            this.Name = "LoadingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "LoadingForm";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxLoading)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _pictureBoxLoading;
    }
}