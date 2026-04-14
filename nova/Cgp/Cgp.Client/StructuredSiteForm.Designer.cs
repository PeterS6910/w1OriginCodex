namespace Contal.Cgp.Client
{
    partial class StructuredSiteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StructuredSiteForm));
            this._scSructuredSite = new System.Windows.Forms.SplitContainer();
            this._tvStructuredSiteLeft = new Contal.IwQuick.PlatformPC.UI.TreeViewWithFilter();
            this._tvStructuredSiteRight = new Contal.IwQuick.PlatformPC.UI.TreeViewWithFilter();
            this._bCreateSubSite = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._bRenameSubSite = new System.Windows.Forms.Button();
            this._bApply = new System.Windows.Forms.Button();
            this._tvBrokenReferences = new System.Windows.Forms.TreeView();
            this._lBrokenReferences = new System.Windows.Forms.Label();
            this._scStructuredSiteBrokenReferences = new System.Windows.Forms.SplitContainer();
            this._bCreateFolder = new System.Windows.Forms.Button();
            this._bRenameFolder = new System.Windows.Forms.Button();
            this._lNotification = new System.Windows.Forms.Label();
            this._scSructuredSite.Panel1.SuspendLayout();
            this._scSructuredSite.Panel2.SuspendLayout();
            this._scSructuredSite.SuspendLayout();
            this._scStructuredSiteBrokenReferences.Panel1.SuspendLayout();
            this._scStructuredSiteBrokenReferences.Panel2.SuspendLayout();
            this._scStructuredSiteBrokenReferences.SuspendLayout();
            this.SuspendLayout();
            // 
            // _scSructuredSite
            // 
            this._scSructuredSite.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._scSructuredSite.Location = new System.Drawing.Point(3, 0);
            this._scSructuredSite.Name = "_scSructuredSite";
            // 
            // _scSructuredSite.Panel1
            // 
            this._scSructuredSite.Panel1.Controls.Add(this._lNotification);
            this._scSructuredSite.Panel1.Controls.Add(this._tvStructuredSiteLeft);
            this._scSructuredSite.Panel1MinSize = 0;
            // 
            // _scSructuredSite.Panel2
            // 
            this._scSructuredSite.Panel2.Controls.Add(this._tvStructuredSiteRight);
            this._scSructuredSite.Panel2MinSize = 0;
            this._scSructuredSite.Size = new System.Drawing.Size(453, 354);
            this._scSructuredSite.SplitterDistance = 221;
            this._scSructuredSite.TabIndex = 0;
            // 
            // _tvStructuredSiteLeft
            // 
            this._tvStructuredSiteLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tvStructuredSiteLeft.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this._tvStructuredSiteLeft.FullRowSelect = true;
            this._tvStructuredSiteLeft.ItemHeight = 20;
            this._tvStructuredSiteLeft.Location = new System.Drawing.Point(0, 0);
            this._tvStructuredSiteLeft.Name = "_tvStructuredSiteLeft";
            this._tvStructuredSiteLeft.Size = new System.Drawing.Size(221, 354);
            this._tvStructuredSiteLeft.TabIndex = 0;
            // 
            // _tvStructuredSiteRight
            // 
            this._tvStructuredSiteRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tvStructuredSiteRight.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this._tvStructuredSiteRight.FullRowSelect = true;
            this._tvStructuredSiteRight.ItemHeight = 20;
            this._tvStructuredSiteRight.Location = new System.Drawing.Point(0, 0);
            this._tvStructuredSiteRight.Name = "_tvStructuredSiteRight";
            this._tvStructuredSiteRight.Size = new System.Drawing.Size(228, 354);
            this._tvStructuredSiteRight.TabIndex = 0;
            // 
            // _bCreateSubSite
            // 
            this._bCreateSubSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bCreateSubSite.Location = new System.Drawing.Point(12, 372);
            this._bCreateSubSite.Name = "_bCreateSubSite";
            this._bCreateSubSite.Size = new System.Drawing.Size(100, 23);
            this._bCreateSubSite.TabIndex = 1;
            this._bCreateSubSite.Text = "Create sub site";
            this._bCreateSubSite.UseVisualStyleBackColor = true;
            this._bCreateSubSite.Click += new System.EventHandler(this._bCreateSubSite_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(486, 372);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDelete.Location = new System.Drawing.Point(224, 372);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(100, 23);
            this._bDelete.TabIndex = 3;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(567, 372);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 4;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bRenameSubSite
            // 
            this._bRenameSubSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRenameSubSite.Location = new System.Drawing.Point(118, 372);
            this._bRenameSubSite.Name = "_bRenameSubSite";
            this._bRenameSubSite.Size = new System.Drawing.Size(100, 23);
            this._bRenameSubSite.TabIndex = 5;
            this._bRenameSubSite.Text = "Rename sub site";
            this._bRenameSubSite.UseVisualStyleBackColor = true;
            this._bRenameSubSite.Click += new System.EventHandler(this._bRenameSubSite_Click);
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(405, 372);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 6;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _tvBrokenReferences
            // 
            this._tvBrokenReferences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tvBrokenReferences.FullRowSelect = true;
            this._tvBrokenReferences.Location = new System.Drawing.Point(3, 16);
            this._tvBrokenReferences.Name = "_tvBrokenReferences";
            this._tvBrokenReferences.Size = new System.Drawing.Size(164, 338);
            this._tvBrokenReferences.TabIndex = 7;
            this._tvBrokenReferences.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._tvBrokenReferences_NodeMouseDoubleClick);
            this._tvBrokenReferences.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._tvBrokenReferences_BeforeExpand);
            this._tvBrokenReferences.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this._tvBrokenReferences_BeforeCollapse);
            this._tvBrokenReferences.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this._tvBrokenReferences_ItemDrag);
            // 
            // _lBrokenReferences
            // 
            this._lBrokenReferences.AutoSize = true;
            this._lBrokenReferences.Location = new System.Drawing.Point(3, 0);
            this._lBrokenReferences.Name = "_lBrokenReferences";
            this._lBrokenReferences.Size = new System.Drawing.Size(154, 13);
            this._lBrokenReferences.TabIndex = 8;
            this._lBrokenReferences.Text = "Objects with broken references";
            // 
            // _scStructuredSiteBrokenReferences
            // 
            this._scStructuredSiteBrokenReferences.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._scStructuredSiteBrokenReferences.Location = new System.Drawing.Point(12, 12);
            this._scStructuredSiteBrokenReferences.Name = "_scStructuredSiteBrokenReferences";
            // 
            // _scStructuredSiteBrokenReferences.Panel1
            // 
            this._scStructuredSiteBrokenReferences.Panel1.Controls.Add(this._scSructuredSite);
            this._scStructuredSiteBrokenReferences.Panel1MinSize = 0;
            // 
            // _scStructuredSiteBrokenReferences.Panel2
            // 
            this._scStructuredSiteBrokenReferences.Panel2.Controls.Add(this._tvBrokenReferences);
            this._scStructuredSiteBrokenReferences.Panel2.Controls.Add(this._lBrokenReferences);
            this._scStructuredSiteBrokenReferences.Panel2MinSize = 0;
            this._scStructuredSiteBrokenReferences.Size = new System.Drawing.Size(630, 354);
            this._scStructuredSiteBrokenReferences.SplitterDistance = 459;
            this._scStructuredSiteBrokenReferences.TabIndex = 9;
            // 
            // _bCreateFolder
            // 
            this._bCreateFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bCreateFolder.Location = new System.Drawing.Point(12, 401);
            this._bCreateFolder.Name = "_bCreateFolder";
            this._bCreateFolder.Size = new System.Drawing.Size(100, 23);
            this._bCreateFolder.TabIndex = 10;
            this._bCreateFolder.Text = "Create folder";
            this._bCreateFolder.UseVisualStyleBackColor = true;
            this._bCreateFolder.Click += new System.EventHandler(this._bCreateFolder_Click);
            // 
            // _bRenameFolder
            // 
            this._bRenameFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRenameFolder.Location = new System.Drawing.Point(118, 401);
            this._bRenameFolder.Name = "_bRenameFolder";
            this._bRenameFolder.Size = new System.Drawing.Size(100, 23);
            this._bRenameFolder.TabIndex = 11;
            this._bRenameFolder.Text = "Rename folder";
            this._bRenameFolder.UseVisualStyleBackColor = true;
            this._bRenameFolder.Click += new System.EventHandler(this._bRenameFolder_Click);
            // 
            // _lNotification
            // 
            this._lNotification.AutoSize = true;
            this._lNotification.Location = new System.Drawing.Point(-3, 0);
            this._lNotification.Name = "_lNotification";
            this._lNotification.Size = new System.Drawing.Size(60, 13);
            this._lNotification.TabIndex = 1;
            this._lNotification.Text = "Notification";
            this._lNotification.Visible = false;
            // 
            // StructuredSiteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 436);
            this.Controls.Add(this._bRenameFolder);
            this.Controls.Add(this._bCreateFolder);
            this.Controls.Add(this._scStructuredSiteBrokenReferences);
            this.Controls.Add(this._bApply);
            this.Controls.Add(this._bRenameSubSite);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bDelete);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCreateSubSite);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StructuredSiteForm";
            this.Text = "StructuredSiteForm";
            this._scSructuredSite.Panel1.ResumeLayout(false);
            this._scSructuredSite.Panel1.PerformLayout();
            this._scSructuredSite.Panel2.ResumeLayout(false);
            this._scSructuredSite.ResumeLayout(false);
            this._scStructuredSiteBrokenReferences.Panel1.ResumeLayout(false);
            this._scStructuredSiteBrokenReferences.Panel2.ResumeLayout(false);
            this._scStructuredSiteBrokenReferences.Panel2.PerformLayout();
            this._scStructuredSiteBrokenReferences.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _scSructuredSite;
        private Contal.IwQuick.PlatformPC.UI.TreeViewWithFilter _tvStructuredSiteLeft;
        private Contal.IwQuick.PlatformPC.UI.TreeViewWithFilter _tvStructuredSiteRight;
        private System.Windows.Forms.Button _bCreateSubSite;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bRenameSubSite;
        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.TreeView _tvBrokenReferences;
        private System.Windows.Forms.Label _lBrokenReferences;
        private System.Windows.Forms.SplitContainer _scStructuredSiteBrokenReferences;
        private System.Windows.Forms.Button _bCreateFolder;
        private System.Windows.Forms.Button _bRenameFolder;
        private System.Windows.Forms.Label _lNotification;
    }
}