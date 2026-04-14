namespace Contal.Cgp.Client
{
    partial class UserFoldersStructuresCreateFolder
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
            this._userTreeView = new System.Windows.Forms.TreeView();
            this._lPosition = new System.Windows.Forms.Label();
            this._lName = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _userTreeView
            // 
            this._userTreeView.AllowDrop = true;
            this._userTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._userTreeView.Location = new System.Drawing.Point(12, 28);
            this._userTreeView.Name = "_userTreeView";
            this._userTreeView.Size = new System.Drawing.Size(476, 354);
            this._userTreeView.TabIndex = 1;
            this._userTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._userTreeView_BeforeExpand);
            this._userTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._userTreeView_AfterSelect);
            this._userTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this._userTreeView_BeforeSelect);
            // 
            // _lPosition
            // 
            this._lPosition.AutoSize = true;
            this._lPosition.Location = new System.Drawing.Point(12, 9);
            this._lPosition.Name = "_lPosition";
            this._lPosition.Size = new System.Drawing.Size(44, 13);
            this._lPosition.TabIndex = 0;
            this._lPosition.Text = "Position";
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 389);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 2;
            this._lName.Text = "Name";
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(12, 405);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(177, 20);
            this._eName.TabIndex = 3;
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(413, 402);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(332, 402);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // UserFoldersStructuresCreateFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 456);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._lPosition);
            this.Controls.Add(this._userTreeView);
            this.Name = "UserFoldersStructuresCreateFolder";
            this.Text = "Create folder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _userTreeView;
        private System.Windows.Forms.Label _lPosition;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
    }
}