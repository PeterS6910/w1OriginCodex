namespace Contal.Cgp.Client
{
    partial class DirectCrSwipCard
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
            this._lWaitingForSwipingCardNT = new System.Windows.Forms.Label();
            this._bCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lWaitingForSwipingCardNT
            // 
            this._lWaitingForSwipingCardNT.AutoSize = true;
            this._lWaitingForSwipingCardNT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._lWaitingForSwipingCardNT.Location = new System.Drawing.Point(12, 24);
            this._lWaitingForSwipingCardNT.Name = "_lWaitingForSwipingCardNT";
            this._lWaitingForSwipingCardNT.Size = new System.Drawing.Size(200, 20);
            this._lWaitingForSwipingCardNT.TabIndex = 0;
            this._lWaitingForSwipingCardNT.Text = "Waiting for swiping card";
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(214, 62);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 1;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // DirectCrSwipCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 97);
            this.ControlBox = false;
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._lWaitingForSwipingCardNT);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DirectCrSwipCard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DirectCrSwipCard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lWaitingForSwipingCardNT;
        private System.Windows.Forms.Button _bCancel;
    }
}