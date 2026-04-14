namespace Contal.Cgp.Client
{
    partial class CreateFolderForm
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
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._eName = new System.Windows.Forms.TextBox();
            this._lFolderName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(262, 52);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 29);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(160, 52);
            this._bOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(94, 29);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(126, 15);
            this._eName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(229, 22);
            this._eName.TabIndex = 3;
            // 
            // _lFolderName
            // 
            this._lFolderName.AutoSize = true;
            this._lFolderName.Location = new System.Drawing.Point(15, 19);
            this._lFolderName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFolderName.Name = "_lFolderName";
            this._lFolderName.Size = new System.Drawing.Size(87, 17);
            this._lFolderName.TabIndex = 6;
            this._lFolderName.Text = "Folder name";
            // 
            // CreateFolderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(371, 96);
            this.ControlBox = false;
            this.Controls.Add(this._lFolderName);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._eName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CreateFolderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CreateFolderForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CreateFolderForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lFolderName;
    }
}