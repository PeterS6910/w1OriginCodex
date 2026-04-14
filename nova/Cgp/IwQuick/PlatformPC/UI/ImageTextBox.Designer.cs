namespace Contal.IwQuick.UI
{
    partial class ImageTextBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._tbTextBox = new System.Windows.Forms.TextBox();
            this._pbPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._pbPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _tbTextBox
            // 
            this._tbTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tbTextBox.BackColor = System.Drawing.Color.White;
            this._tbTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbTextBox.Location = new System.Drawing.Point(19, 2);
            this._tbTextBox.Name = "_tbTextBox";
            this._tbTextBox.Size = new System.Drawing.Size(124, 13);
            this._tbTextBox.TabIndex = 2;
            this._tbTextBox.DoubleClick += new System.EventHandler(this._tbTextBox_DoubleClick);
            this._tbTextBox.TextChanged += new System.EventHandler(this._tbTextBox_TextChanged);
            // 
            // _pbPictureBox
            // 
            this._pbPictureBox.BackColor = System.Drawing.Color.White;
            this._pbPictureBox.Location = new System.Drawing.Point(1, 1);
            this._pbPictureBox.Name = "_pbPictureBox";
            this._pbPictureBox.Size = new System.Drawing.Size(16, 16);
            this._pbPictureBox.TabIndex = 3;
            this._pbPictureBox.TabStop = false;
            this._pbPictureBox.DoubleClick += new System.EventHandler(this._pbPictureBox_DoubleClick);
            // 
            // ImageTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this._pbPictureBox);
            this.Controls.Add(this._tbTextBox);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "ImageTextBox";
            this.Size = new System.Drawing.Size(144, 20);
            this.BackColorChanged += new System.EventHandler(this.ImageTextBox_BackColorChanged);
            ((System.ComponentModel.ISupportInitialize)(this._pbPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _pbPictureBox;
        private System.Windows.Forms.TextBox _tbTextBox;

    }
}
