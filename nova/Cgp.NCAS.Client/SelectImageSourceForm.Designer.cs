namespace Contal.Cgp.NCAS.Client
{
    partial class SelectImageSourceForm
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
            this._ehImageSourceViewer = new System.Windows.Forms.Integration.ElementHost();
            this.imageSourceViewer1 = new Contal.Cgp.NCAS.Client.ImageSourceViewer();
            this._bSelect = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _ehImageSourceViewer
            // 
            this._ehImageSourceViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ehImageSourceViewer.Location = new System.Drawing.Point(0, 0);
            this._ehImageSourceViewer.Name = "_ehImageSourceViewer";
            this._ehImageSourceViewer.Size = new System.Drawing.Size(535, 407);
            this._ehImageSourceViewer.TabIndex = 0;
            this._ehImageSourceViewer.Text = "_ehImageSourceViewer";
            this._ehImageSourceViewer.Child = this.imageSourceViewer1;
            // 
            // _bSelect
            // 
            this._bSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSelect.Location = new System.Drawing.Point(4, 413);
            this._bSelect.Name = "_bSelect";
            this._bSelect.Size = new System.Drawing.Size(85, 26);
            this._bSelect.TabIndex = 1;
            this._bSelect.Text = "Select";
            this._bSelect.UseVisualStyleBackColor = true;
            this._bSelect.Click += new System.EventHandler(this._bSelect_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bCancel.Location = new System.Drawing.Point(186, 413);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(85, 26);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDelete.Location = new System.Drawing.Point(95, 413);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(85, 26);
            this._bDelete.TabIndex = 3;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // SelectImageSourceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(535, 448);
            this.Controls.Add(this._bDelete);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bSelect);
            this.Controls.Add(this._ehImageSourceViewer);
            this.Name = "SelectImageSourceForm";
            this.Text = "Select Image Source";
            this.Load += new System.EventHandler(this.SelectImageSourceForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost _ehImageSourceViewer;
        private ImageSourceViewer imageSourceViewer1;
        private System.Windows.Forms.Button _bSelect;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bDelete;
    }
}
