using Contal.Cgp.Components;

namespace Cgp.Components
{
    partial class MultiRenameDialog
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
            this._lQuestion = new System.Windows.Forms.Label();
            this._bRename = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._bRenameAll = new System.Windows.Forms.Button();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this._wpfObjectListView = new WpfObjectListView();
            this.SuspendLayout();
            // 
            // _lQuestion
            // 
            this._lQuestion.AutoSize = true;
            this._lQuestion.Location = new System.Drawing.Point(12, 11);
            this._lQuestion.Name = "_lQuestion";
            this._lQuestion.Size = new System.Drawing.Size(176, 13);
            this._lQuestion.TabIndex = 12;
            this._lQuestion.Text = "Doyou want to rename these items?";
            // 
            // _bRename
            // 
            this._bRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRename.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bRename.Location = new System.Drawing.Point(322, 224);
            this._bRename.Name = "_bRename";
            this._bRename.Size = new System.Drawing.Size(108, 25);
            this._bRename.TabIndex = 11;
            this._bRename.Text = "RenameSelected";
            this._bRename.UseVisualStyleBackColor = true;
            this._bRename.Click += new System.EventHandler(this._bRename_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(436, 224);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(81, 25);
            this._bCancel.TabIndex = 10;
            this._bCancel.Text = "CANCEL";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bRenameAll
            // 
            this._bRenameAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRenameAll.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bRenameAll.Location = new System.Drawing.Point(235, 224);
            this._bRenameAll.Name = "_bRenameAll";
            this._bRenameAll.Size = new System.Drawing.Size(81, 25);
            this._bRenameAll.TabIndex = 9;
            this._bRenameAll.Text = "RenameAll";
            this._bRenameAll.UseVisualStyleBackColor = true;
            this._bRenameAll.Click += new System.EventHandler(this._bRenameAll_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost1.Location = new System.Drawing.Point(12, 37);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(505, 181);
            this.elementHost1.TabIndex = 13;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this._wpfObjectListView;
            // 
            // MultiRenameDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 256);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this._lQuestion);
            this.Controls.Add(this._bRename);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bRenameAll);
            this.MinimizeBox = false;
            this.Name = "MultiRenameDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MultiRenameDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private WpfObjectListView _wpfObjectListView;
        private System.Windows.Forms.Label _lQuestion;
        private System.Windows.Forms.Button _bRename;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bRenameAll;
    }
}