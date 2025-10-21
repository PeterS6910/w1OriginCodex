namespace Contal.Cgp.Client
{
    partial class RenameFolderForm
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
            this._eNewName = new System.Windows.Forms.TextBox();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lNewFolderName = new System.Windows.Forms.Label();
            this._eOldName = new System.Windows.Forms.TextBox();
            this._lOldFolderName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _eNewName
            // 
            this._eNewName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eNewName.Location = new System.Drawing.Point(147, 38);
            this._eNewName.Name = "_eNewName";
            this._eNewName.Size = new System.Drawing.Size(192, 20);
            this._eNewName.TabIndex = 3;
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(183, 64);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(264, 64);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lNewFolderName
            // 
            this._lNewFolderName.AutoSize = true;
            this._lNewFolderName.Location = new System.Drawing.Point(12, 41);
            this._lNewFolderName.Name = "_lNewFolderName";
            this._lNewFolderName.Size = new System.Drawing.Size(87, 13);
            this._lNewFolderName.TabIndex = 2;
            this._lNewFolderName.Text = "New folder name";
            // 
            // _eOldName
            // 
            this._eOldName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eOldName.Location = new System.Drawing.Point(147, 12);
            this._eOldName.Name = "_eOldName";
            this._eOldName.ReadOnly = true;
            this._eOldName.Size = new System.Drawing.Size(192, 20);
            this._eOldName.TabIndex = 1;
            // 
            // _lOldFolderName
            // 
            this._lOldFolderName.AutoSize = true;
            this._lOldFolderName.Location = new System.Drawing.Point(12, 15);
            this._lOldFolderName.Name = "_lOldFolderName";
            this._lOldFolderName.Size = new System.Drawing.Size(81, 13);
            this._lOldFolderName.TabIndex = 0;
            this._lOldFolderName.Text = "Old folder name";
            // 
            // RenameFolderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(351, 97);
            this.ControlBox = false;
            this.Controls.Add(this._lOldFolderName);
            this.Controls.Add(this._eOldName);
            this.Controls.Add(this._lNewFolderName);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._eNewName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "RenameFolderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RenameUserTreeViewNodeForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RenameFolderForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _eNewName;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lNewFolderName;
        private System.Windows.Forms.TextBox _eOldName;
        private System.Windows.Forms.Label _lOldFolderName;
    }
}