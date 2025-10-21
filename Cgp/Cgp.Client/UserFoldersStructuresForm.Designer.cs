namespace Contal.Cgp.Client
{
    partial class UserFoldersStructuresForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserFoldersStructuresForm));
            this._userTreeView = new System.Windows.Forms.TreeView();
            this._cmTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._miCreate = new System.Windows.Forms.ToolStripMenuItem();
            this._miRename = new System.Windows.Forms.ToolStripMenuItem();
            this._miDelete = new System.Windows.Forms.ToolStripMenuItem();
            this._miAssignObject = new System.Windows.Forms.ToolStripMenuItem();
            this._miUnassignObject = new System.Windows.Forms.ToolStripMenuItem();
            this._miOpenEditForm = new System.Windows.Forms.ToolStripMenuItem();
            this._miCreateObject = new System.Windows.Forms.ToolStripMenuItem();
            this._bCreate = new System.Windows.Forms.Button();
            this._bRename = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._bOpenEditForm = new System.Windows.Forms.Button();
            this._bAssignObject = new System.Windows.Forms.Button();
            this._cmAssignObject = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._bCreateObject = new System.Windows.Forms.Button();
            this._cmCreateObject = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._bDockUndock = new System.Windows.Forms.Button();
            this._bUnassignObject = new System.Windows.Forms.Button();
            this._cmTreeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // _userTreeView
            // 
            this._userTreeView.AllowDrop = true;
            this._userTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._userTreeView.Location = new System.Drawing.Point(12, 41);
            this._userTreeView.Name = "_userTreeView";
            this._userTreeView.Size = new System.Drawing.Size(568, 264);
            this._userTreeView.TabIndex = 1;
            this._userTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._userTreeView_NodeMouseDoubleClick);
            this._userTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this._userTreeView_AfterCollapse);
            this._userTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._userTreeView_BeforeExpand);
            this._userTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this._userTreeView_DragDrop);
            this._userTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._userTreeView_AfterSelect);
            this._userTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._userTreeView_NodeMouseClick);
            this._userTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this._userTreeView_BeforeSelect);
            this._userTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this._userTreeView_AfterExpand);
            this._userTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this._userTreeView_DragOver);
            // 
            // _cmTreeView
            // 
            this._cmTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miCreate,
            this._miRename,
            this._miDelete,
            this._miAssignObject,
            this._miUnassignObject,
            this._miOpenEditForm,
            this._miCreateObject});
            this._cmTreeView.Name = "contextMenuStrip1";
            this._cmTreeView.Size = new System.Drawing.Size(159, 158);
            // 
            // _miCreate
            // 
            this._miCreate.Name = "_miCreate";
            this._miCreate.Size = new System.Drawing.Size(158, 22);
            this._miCreate.Text = "Create";
            this._miCreate.Click += new System.EventHandler(this._bCreate_Click);
            // 
            // _miRename
            // 
            this._miRename.Name = "_miRename";
            this._miRename.Size = new System.Drawing.Size(158, 22);
            this._miRename.Text = "Rename";
            this._miRename.Click += new System.EventHandler(this._bRename_Click);
            // 
            // _miDelete
            // 
            this._miDelete.Name = "_miDelete";
            this._miDelete.Size = new System.Drawing.Size(158, 22);
            this._miDelete.Text = "Delete";
            this._miDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _miAssignObject
            // 
            this._miAssignObject.Name = "_miAssignObject";
            this._miAssignObject.Size = new System.Drawing.Size(158, 22);
            this._miAssignObject.Text = "Assign object";
            // 
            // _miUnassignObject
            // 
            this._miUnassignObject.Name = "_miUnassignObject";
            this._miUnassignObject.Size = new System.Drawing.Size(158, 22);
            this._miUnassignObject.Text = "Unassing object";
            this._miUnassignObject.Click += new System.EventHandler(this._bUnassignObject_Click);
            // 
            // _miOpenEditForm
            // 
            this._miOpenEditForm.Name = "_miOpenEditForm";
            this._miOpenEditForm.Size = new System.Drawing.Size(158, 22);
            this._miOpenEditForm.Text = "Open edit form";
            this._miOpenEditForm.Click += new System.EventHandler(this._miOpenEditForm_Click);
            // 
            // _miCreateObject
            // 
            this._miCreateObject.Name = "_miCreateObject";
            this._miCreateObject.Size = new System.Drawing.Size(158, 22);
            this._miCreateObject.Text = "Create object";
            // 
            // _bCreate
            // 
            this._bCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreate.Location = new System.Drawing.Point(200, 311);
            this._bCreate.Name = "_bCreate";
            this._bCreate.Size = new System.Drawing.Size(122, 22);
            this._bCreate.TabIndex = 2;
            this._bCreate.Text = "Create";
            this._bCreate.UseVisualStyleBackColor = true;
            this._bCreate.Click += new System.EventHandler(this._bCreate_Click);
            // 
            // _bRename
            // 
            this._bRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRename.Location = new System.Drawing.Point(328, 311);
            this._bRename.Name = "_bRename";
            this._bRename.Size = new System.Drawing.Size(122, 22);
            this._bRename.TabIndex = 3;
            this._bRename.Text = "Rename";
            this._bRename.UseVisualStyleBackColor = true;
            this._bRename.Click += new System.EventHandler(this._bRename_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(458, 311);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(122, 22);
            this._bDelete.TabIndex = 4;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bOpenEditForm
            // 
            this._bOpenEditForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOpenEditForm.Location = new System.Drawing.Point(458, 339);
            this._bOpenEditForm.Name = "_bOpenEditForm";
            this._bOpenEditForm.Size = new System.Drawing.Size(122, 22);
            this._bOpenEditForm.TabIndex = 8;
            this._bOpenEditForm.Text = "Open edit form";
            this._bOpenEditForm.UseVisualStyleBackColor = true;
            this._bOpenEditForm.Click += new System.EventHandler(this._bOpenEditForm_Click);
            // 
            // _bAssignObject
            // 
            this._bAssignObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAssignObject.Location = new System.Drawing.Point(200, 339);
            this._bAssignObject.Name = "_bAssignObject";
            this._bAssignObject.Size = new System.Drawing.Size(122, 22);
            this._bAssignObject.TabIndex = 6;
            this._bAssignObject.Text = "Assign object";
            this._bAssignObject.UseVisualStyleBackColor = true;
            this._bAssignObject.Click += new System.EventHandler(this._bAssignObject_Click);
            // 
            // _cmAssignObject
            // 
            this._cmAssignObject.Name = "contextMenuStrip1";
            this._cmAssignObject.Size = new System.Drawing.Size(61, 4);
            // 
            // _bCreateObject
            // 
            this._bCreateObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreateObject.Location = new System.Drawing.Point(72, 339);
            this._bCreateObject.Name = "_bCreateObject";
            this._bCreateObject.Size = new System.Drawing.Size(122, 22);
            this._bCreateObject.TabIndex = 5;
            this._bCreateObject.Text = "Create object";
            this._bCreateObject.UseVisualStyleBackColor = true;
            this._bCreateObject.Click += new System.EventHandler(this._bCreateObject_Click);
            // 
            // _cmCreateObject
            // 
            this._cmCreateObject.Name = "contextMenuStrip1";
            this._cmCreateObject.Size = new System.Drawing.Size(61, 4);
            // 
            // _bDockUndock
            // 
            this._bDockUndock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bDockUndock.Location = new System.Drawing.Point(505, 12);
            this._bDockUndock.Name = "_bDockUndock";
            this._bDockUndock.Size = new System.Drawing.Size(75, 23);
            this._bDockUndock.TabIndex = 0;
            this._bDockUndock.Text = "Undock";
            this._bDockUndock.UseVisualStyleBackColor = true;
            this._bDockUndock.Click += new System.EventHandler(this._bDockUndock_Click);
            // 
            // _bUnassignObject
            // 
            this._bUnassignObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bUnassignObject.Location = new System.Drawing.Point(328, 338);
            this._bUnassignObject.Name = "_bUnassignObject";
            this._bUnassignObject.Size = new System.Drawing.Size(124, 23);
            this._bUnassignObject.TabIndex = 7;
            this._bUnassignObject.Text = "Unassign object";
            this._bUnassignObject.UseVisualStyleBackColor = true;
            this._bUnassignObject.Click += new System.EventHandler(this._bUnassignObject_Click);
            // 
            // UserFoldersStructuresForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(592, 373);
            this.Controls.Add(this._bUnassignObject);
            this.Controls.Add(this._bDockUndock);
            this.Controls.Add(this._userTreeView);
            this.Controls.Add(this._bOpenEditForm);
            this.Controls.Add(this._bDelete);
            this.Controls.Add(this._bRename);
            this.Controls.Add(this._bCreate);
            this.Controls.Add(this._bCreateObject);
            this.Controls.Add(this._bAssignObject);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserFoldersStructuresForm";
            this.Text = "UserFoldersStructuresForm";
            this._cmTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _userTreeView;
        private System.Windows.Forms.Button _bCreate;
        private System.Windows.Forms.Button _bRename;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.ContextMenuStrip _cmTreeView;
        private System.Windows.Forms.ToolStripMenuItem _miCreate;
        private System.Windows.Forms.ToolStripMenuItem _miRename;
        private System.Windows.Forms.ToolStripMenuItem _miDelete;
        private System.Windows.Forms.ToolStripMenuItem _miAssignObject;
        private System.Windows.Forms.ToolStripMenuItem _miOpenEditForm;
        private System.Windows.Forms.Button _bOpenEditForm;
        private System.Windows.Forms.Button _bAssignObject;
        private System.Windows.Forms.ContextMenuStrip _cmAssignObject;
        private System.Windows.Forms.ToolStripMenuItem _miCreateObject;
        private System.Windows.Forms.Button _bCreateObject;
        private System.Windows.Forms.ContextMenuStrip _cmCreateObject;
        private System.Windows.Forms.Button _bDockUndock;
        private System.Windows.Forms.Button _bUnassignObject;
        private System.Windows.Forms.ToolStripMenuItem _miUnassignObject;
    }
}