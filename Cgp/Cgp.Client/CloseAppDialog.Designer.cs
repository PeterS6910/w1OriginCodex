namespace Contal.Cgp.Client
{
    partial class CloseAppDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloseAppDialog));
            this._lQuestionCloseApp = new System.Windows.Forms.Label();
            this._bLogout = new System.Windows.Forms.Button();
            this._bClose = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._pWhiteUnderText = new System.Windows.Forms.Panel();
            this._pbAsterisk = new System.Windows.Forms.PictureBox();
            this._pWhiteUnderText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbAsterisk)).BeginInit();
            this.SuspendLayout();
            // 
            // _lQuestionCloseApp
            // 
            this._lQuestionCloseApp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lQuestionCloseApp.AutoSize = true;
            this._lQuestionCloseApp.BackColor = System.Drawing.Color.White;
            this._lQuestionCloseApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lQuestionCloseApp.Location = new System.Drawing.Point(55, 31);
            this._lQuestionCloseApp.Name = "_lQuestionCloseApp";
            this._lQuestionCloseApp.Size = new System.Drawing.Size(148, 15);
            this._lQuestionCloseApp.TabIndex = 0;
            this._lQuestionCloseApp.Text = "Would you like to logout ...";
            this._lQuestionCloseApp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _bLogout
            // 
            this._bLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bLogout.Location = new System.Drawing.Point(8, 88);
            this._bLogout.Name = "_bLogout";
            this._bLogout.Size = new System.Drawing.Size(75, 23);
            this._bLogout.TabIndex = 2;
            this._bLogout.Text = "Logout";
            this._bLogout.UseVisualStyleBackColor = true;
            this._bLogout.Click += new System.EventHandler(this._bLogout_Click);
            // 
            // _bClose
            // 
            this._bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bClose.Location = new System.Drawing.Point(89, 88);
            this._bClose.Name = "_bClose";
            this._bClose.Size = new System.Drawing.Size(75, 23);
            this._bClose.TabIndex = 3;
            this._bClose.Text = "Close";
            this._bClose.UseVisualStyleBackColor = true;
            this._bClose.Click += new System.EventHandler(this._bClose_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(170, 88);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 1;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _pWhiteUnderText
            // 
            this._pWhiteUnderText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pWhiteUnderText.BackColor = System.Drawing.Color.White;
            this._pWhiteUnderText.Controls.Add(this._lQuestionCloseApp);
            this._pWhiteUnderText.Controls.Add(this._pbAsterisk);
            this._pWhiteUnderText.Location = new System.Drawing.Point(0, 0);
            this._pWhiteUnderText.Name = "_pWhiteUnderText";
            this._pWhiteUnderText.Size = new System.Drawing.Size(254, 78);
            this._pWhiteUnderText.TabIndex = 4;
            // 
            // _pbAsterisk
            // 
            this._pbAsterisk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_pbAsterisk.BackgroundImage")));
            this._pbAsterisk.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this._pbAsterisk.Location = new System.Drawing.Point(17, 23);
            this._pbAsterisk.Name = "_pbAsterisk";
            this._pbAsterisk.Size = new System.Drawing.Size(32, 32);
            this._pbAsterisk.TabIndex = 0;
            this._pbAsterisk.TabStop = false;
            // 
            // CloseAppDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(253, 119);
            this.Controls.Add(this._pWhiteUnderText);
            this.Controls.Add(this._bClose);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bLogout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CloseAppDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CloseDialog";
            this._pWhiteUnderText.ResumeLayout(false);
            this._pWhiteUnderText.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbAsterisk)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _lQuestionCloseApp;
        private System.Windows.Forms.Button _bLogout;
        private System.Windows.Forms.Button _bClose;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Panel _pWhiteUnderText;
        private System.Windows.Forms.PictureBox _pbAsterisk;
    }
}