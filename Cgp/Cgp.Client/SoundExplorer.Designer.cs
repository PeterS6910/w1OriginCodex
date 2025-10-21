namespace Contal.Cgp.Client
{
    partial class SoundExplorer
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
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lvFiles = new System.Windows.Forms.ListView();
            this.cFiles = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // _bOk
            // 
            this._bOk.Location = new System.Drawing.Point(12, 12);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 0;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(188, 12);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 1;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lvFiles
            // 
            this._lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cFiles});
            this._lvFiles.Location = new System.Drawing.Point(12, 45);
            this._lvFiles.MultiSelect = false;
            this._lvFiles.Name = "_lvFiles";
            this._lvFiles.Size = new System.Drawing.Size(251, 273);
            this._lvFiles.TabIndex = 2;
            this._lvFiles.UseCompatibleStateImageBehavior = false;
            this._lvFiles.DoubleClick += new System.EventHandler(this._lvFiles_DoubleClick);
            // 
            // cFiles
            // 
            this.cFiles.Width = 240;
            // 
            // SoundExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 330);
            this.Controls.Add(this._lvFiles);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoundExplorer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SoundExplorer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.ListView _lvFiles;
        private System.Windows.Forms.ColumnHeader cFiles;
    }
}