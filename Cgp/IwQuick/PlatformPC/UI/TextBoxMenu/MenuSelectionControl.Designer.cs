namespace Contal.IwQuick.UI
{
    partial class MenuSelectionControl
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
            this._bLeft = new System.Windows.Forms.Button();
            this._bRightBottom = new System.Windows.Forms.Button();
            this._bLeftBottom = new System.Windows.Forms.Button();
            this._bLeftTop = new System.Windows.Forms.Button();
            this._bRight = new System.Windows.Forms.Button();
            this._bRightTop = new System.Windows.Forms.Button();
            this._textBoxPosition = new System.Windows.Forms.TextBox();
            this._bTop = new System.Windows.Forms.Button();
            this._bBottom = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _bLeft
            // 
            this._bLeft.Location = new System.Drawing.Point(0, 19);
            this._bLeft.Name = "_bLeft";
            this._bLeft.Size = new System.Drawing.Size(20, 20);
            this._bLeft.TabIndex = 1;
            this._bLeft.UseVisualStyleBackColor = true;
            this._bLeft.Click += new System.EventHandler(this._bLeft_Click);
            // 
            // _bRightBottom
            // 
            this._bRightBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRightBottom.Location = new System.Drawing.Point(101, 38);
            this._bRightBottom.Name = "_bRightBottom";
            this._bRightBottom.Size = new System.Drawing.Size(20, 20);
            this._bRightBottom.TabIndex = 3;
            this._bRightBottom.UseVisualStyleBackColor = true;
            this._bRightBottom.Click += new System.EventHandler(this._bRightBottom_Click);
            // 
            // _bLeftBottom
            // 
            this._bLeftBottom.Location = new System.Drawing.Point(17, 38);
            this._bLeftBottom.Name = "_bLeftBottom";
            this._bLeftBottom.Size = new System.Drawing.Size(20, 20);
            this._bLeftBottom.TabIndex = 2;
            this._bLeftBottom.UseVisualStyleBackColor = true;
            this._bLeftBottom.Click += new System.EventHandler(this._bLeftBottom_Click);
            // 
            // _bLeftTop
            // 
            this._bLeftTop.Location = new System.Drawing.Point(17, 0);
            this._bLeftTop.Name = "_bLeftTop";
            this._bLeftTop.Size = new System.Drawing.Size(20, 20);
            this._bLeftTop.TabIndex = 4;
            this._bLeftTop.UseVisualStyleBackColor = true;
            this._bLeftTop.Click += new System.EventHandler(this._bLeftTop_Click);
            // 
            // _bRight
            // 
            this._bRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bRight.Location = new System.Drawing.Point(118, 19);
            this._bRight.Name = "_bRight";
            this._bRight.Size = new System.Drawing.Size(20, 20);
            this._bRight.TabIndex = 0;
            this._bRight.UseVisualStyleBackColor = true;
            this._bRight.Click += new System.EventHandler(this._bRight_Click);
            // 
            // _bRightTop
            // 
            this._bRightTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bRightTop.Location = new System.Drawing.Point(101, 0);
            this._bRightTop.Name = "_bRightTop";
            this._bRightTop.Size = new System.Drawing.Size(20, 20);
            this._bRightTop.TabIndex = 5;
            this._bRightTop.UseVisualStyleBackColor = true;
            this._bRightTop.Click += new System.EventHandler(this._bRightTop_Click);
            // 
            // _textBoxPosition
            // 
            this._textBoxPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxPosition.Location = new System.Drawing.Point(19, 19);
            this._textBoxPosition.Name = "_textBoxPosition";
            this._textBoxPosition.Size = new System.Drawing.Size(100, 20);
            this._textBoxPosition.TabIndex = 6;
            // 
            // _bTop
            // 
            this._bTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._bTop.Location = new System.Drawing.Point(36, 0);
            this._bTop.Name = "_bTop";
            this._bTop.Size = new System.Drawing.Size(66, 20);
            this._bTop.TabIndex = 7;
            this._bTop.UseVisualStyleBackColor = true;
            this._bTop.Click += new System.EventHandler(this._bTop_Click);
            // 
            // _bBottom
            // 
            this._bBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._bBottom.Location = new System.Drawing.Point(36, 38);
            this._bBottom.Name = "_bBottom";
            this._bBottom.Size = new System.Drawing.Size(66, 20);
            this._bBottom.TabIndex = 8;
            this._bBottom.UseVisualStyleBackColor = true;
            this._bBottom.Click += new System.EventHandler(this._bBottom_Click);
            // 
            // MenuSelectionControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this._bRightBottom);
            this.Controls.Add(this._bBottom);
            this.Controls.Add(this._bLeftBottom);
            this.Controls.Add(this._textBoxPosition);
            this.Controls.Add(this._bTop);
            this.Controls.Add(this._bRightTop);
            this.Controls.Add(this._bRight);
            this.Controls.Add(this._bLeftTop);
            this.Controls.Add(this._bLeft);
            this.Name = "MenuSelectionControl";
            this.Size = new System.Drawing.Size(138, 58);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bLeft;
        private System.Windows.Forms.Button _bRightBottom;
        private System.Windows.Forms.Button _bLeftBottom;
        private System.Windows.Forms.Button _bLeftTop;
        private System.Windows.Forms.Button _bRight;
        private System.Windows.Forms.Button _bRightTop;
        private System.Windows.Forms.TextBox _textBoxPosition;
        private System.Windows.Forms.Button _bTop;
        private System.Windows.Forms.Button _bBottom;
    }
}
