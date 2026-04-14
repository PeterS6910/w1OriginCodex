namespace Contal.Cgp.NCAS.Client
{
    partial class NCASACLGruopEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASACLGruopEditForm));
            this._lName = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._bApply = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._tpAccessControlLists = new System.Windows.Forms.TabPage();
            this._bEdit = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._bAdd = new System.Windows.Forms.Button();
            this._ilbAccessControlLists = new Contal.IwQuick.UI.ImageListBox();
            this._tcSettings = new System.Windows.Forms.TabControl();
            this._tpImplicitAssigningConfiguration = new System.Windows.Forms.TabPage();
            this._cbRemoveAllAcls = new System.Windows.Forms.CheckBox();
            this._gbDepartmentAclGroupRelation = new System.Windows.Forms.GroupBox();
            this._cbUseDepartmentAclGroupRelation = new System.Windows.Forms.CheckBox();
            this._cbRemoveAclsDepartmentChange = new System.Windows.Forms.CheckBox();
            this._cbRemoveAclsDepartmentRemove = new System.Windows.Forms.CheckBox();
            this._cbUseForAllPersons = new System.Windows.Forms.CheckBox();
            this._gbValidityTimeOfAssigning = new System.Windows.Forms.GroupBox();
            this._lDays = new System.Windows.Forms.Label();
            this._lMonths = new System.Windows.Forms.Label();
            this._nudDays = new System.Windows.Forms.NumericUpDown();
            this._nudMonths = new System.Windows.Forms.NumericUpDown();
            this._lYears = new System.Windows.Forms.Label();
            this._nudYears = new System.Windows.Forms.NumericUpDown();
            this._chbIsImplicit = new System.Windows.Forms.CheckBox();
            this._tpDepartments = new System.Windows.Forms.TabPage();
            this._bEdit1 = new System.Windows.Forms.Button();
            this._bDelete1 = new System.Windows.Forms.Button();
            this._bAdd1 = new System.Windows.Forms.Button();
            this._ilbDepartments = new Contal.IwQuick.UI.ImageListBox();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._chbApplyEndValidity = new System.Windows.Forms.CheckBox();
            this._tpDescription.SuspendLayout();
            this._tpAccessControlLists.SuspendLayout();
            this._tcSettings.SuspendLayout();
            this._tpImplicitAssigningConfiguration.SuspendLayout();
            this._gbDepartmentAclGroupRelation.SuspendLayout();
            this._gbValidityTimeOfAssigning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudMonths)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudYears)).BeginInit();
            this._tpDepartments.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(9, 15);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 2;
            this._lName.Text = "Name";
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(65, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(457, 20);
            this._eName.TabIndex = 3;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _bApply
            // 
            this._bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bApply.Location = new System.Drawing.Point(285, 377);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(75, 23);
            this._bApply.TabIndex = 9;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(366, 377);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 7;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(447, 377);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 8;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 40);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(3);
            this._tpDescription.Size = new System.Drawing.Size(502, 285);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(3, 3);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(496, 279);
            this._eDescription.TabIndex = 2;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tpAccessControlLists
            // 
            this._tpAccessControlLists.BackColor = System.Drawing.SystemColors.Control;
            this._tpAccessControlLists.Controls.Add(this._bEdit);
            this._tpAccessControlLists.Controls.Add(this._bDelete);
            this._tpAccessControlLists.Controls.Add(this._bAdd);
            this._tpAccessControlLists.Controls.Add(this._ilbAccessControlLists);
            this._tpAccessControlLists.Location = new System.Drawing.Point(4, 40);
            this._tpAccessControlLists.Name = "_tpAccessControlLists";
            this._tpAccessControlLists.Padding = new System.Windows.Forms.Padding(3);
            this._tpAccessControlLists.Size = new System.Drawing.Size(502, 285);
            this._tpAccessControlLists.TabIndex = 0;
            this._tpAccessControlLists.Text = "Access control lists";
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(421, 250);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 3;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(340, 250);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(75, 23);
            this._bDelete.TabIndex = 2;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bAdd
            // 
            this._bAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd.Location = new System.Drawing.Point(259, 250);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(75, 23);
            this._bAdd.TabIndex = 1;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // _ilbAccessControlLists
            // 
            this._ilbAccessControlLists.AllowDrop = true;
            this._ilbAccessControlLists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ilbAccessControlLists.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._ilbAccessControlLists.FormattingEnabled = true;
            this._ilbAccessControlLists.ImageList = null;
            this._ilbAccessControlLists.Location = new System.Drawing.Point(6, 6);
            this._ilbAccessControlLists.Name = "_ilbAccessControlLists";
            this._ilbAccessControlLists.SelectedItemObject = null;
            this._ilbAccessControlLists.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._ilbAccessControlLists.Size = new System.Drawing.Size(490, 238);
            this._ilbAccessControlLists.Sorted = true;
            this._ilbAccessControlLists.TabIndex = 0;
            this._ilbAccessControlLists.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._ilbAccessControlLists_MouseDoubleClick);
            this._ilbAccessControlLists.DragOver += new System.Windows.Forms.DragEventHandler(this._ilbAccessControlLists_DragOver);
            this._ilbAccessControlLists.DragDrop += new System.Windows.Forms.DragEventHandler(this._ilbAccessControlLists_DragDrop);
            // 
            // _tcSettings
            // 
            this._tcSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcSettings.Controls.Add(this._tpImplicitAssigningConfiguration);
            this._tcSettings.Controls.Add(this._tpDepartments);
            this._tcSettings.Controls.Add(this._tpAccessControlLists);
            this._tcSettings.Controls.Add(this._tpUserFolders);
            this._tcSettings.Controls.Add(this._tpReferencedBy);
            this._tcSettings.Controls.Add(this._tpDescription);
            this._tcSettings.Location = new System.Drawing.Point(12, 38);
            this._tcSettings.Multiline = true;
            this._tcSettings.Name = "_tcSettings";
            this._tcSettings.SelectedIndex = 0;
            this._tcSettings.Size = new System.Drawing.Size(510, 333);
            this._tcSettings.TabIndex = 4;
            this._tcSettings.TabStop = false;
            // 
            // _tpImplicitAssigningConfiguration
            // 
            this._tpImplicitAssigningConfiguration.BackColor = System.Drawing.SystemColors.Control;
            this._tpImplicitAssigningConfiguration.Controls.Add(this._cbRemoveAllAcls);
            this._tpImplicitAssigningConfiguration.Controls.Add(this._gbDepartmentAclGroupRelation);
            this._tpImplicitAssigningConfiguration.Controls.Add(this._cbUseForAllPersons);
            this._tpImplicitAssigningConfiguration.Controls.Add(this._gbValidityTimeOfAssigning);
            this._tpImplicitAssigningConfiguration.Controls.Add(this._chbIsImplicit);
            this._tpImplicitAssigningConfiguration.Location = new System.Drawing.Point(4, 40);
            this._tpImplicitAssigningConfiguration.Name = "_tpImplicitAssigningConfiguration";
            this._tpImplicitAssigningConfiguration.Size = new System.Drawing.Size(502, 289);
            this._tpImplicitAssigningConfiguration.TabIndex = 2;
            this._tpImplicitAssigningConfiguration.Text = "Implicit assigning configuration";
            // 
            // _cbRemoveAllAcls
            // 
            this._cbRemoveAllAcls.AutoSize = true;
            this._cbRemoveAllAcls.Checked = true;
            this._cbRemoveAllAcls.CheckState = System.Windows.Forms.CheckState.Checked;
            this._cbRemoveAllAcls.Location = new System.Drawing.Point(3, 49);
            this._cbRemoveAllAcls.Name = "_cbRemoveAllAcls";
            this._cbRemoveAllAcls.Size = new System.Drawing.Size(451, 17);
            this._cbRemoveAllAcls.TabIndex = 15;
            this._cbRemoveAllAcls.Text = "Remove all existing ACLs when creating association between the ACL group and a pe" +
                "rson";
            this._cbRemoveAllAcls.UseVisualStyleBackColor = true;
            this._cbRemoveAllAcls.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _gbDepartmentAclGroupRelation
            // 
            this._gbDepartmentAclGroupRelation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbDepartmentAclGroupRelation.Controls.Add(this._cbUseDepartmentAclGroupRelation);
            this._gbDepartmentAclGroupRelation.Controls.Add(this._cbRemoveAclsDepartmentChange);
            this._gbDepartmentAclGroupRelation.Controls.Add(this._cbRemoveAclsDepartmentRemove);
            this._gbDepartmentAclGroupRelation.Location = new System.Drawing.Point(3, 72);
            this._gbDepartmentAclGroupRelation.Name = "_gbDepartmentAclGroupRelation";
            this._gbDepartmentAclGroupRelation.Size = new System.Drawing.Size(496, 88);
            this._gbDepartmentAclGroupRelation.TabIndex = 14;
            this._gbDepartmentAclGroupRelation.TabStop = false;
            this._gbDepartmentAclGroupRelation.Text = "Department - ACL group relation";
            // 
            // _cbUseDepartmentAclGroupRelation
            // 
            this._cbUseDepartmentAclGroupRelation.AutoSize = true;
            this._cbUseDepartmentAclGroupRelation.Location = new System.Drawing.Point(10, 19);
            this._cbUseDepartmentAclGroupRelation.Name = "_cbUseDepartmentAclGroupRelation";
            this._cbUseDepartmentAclGroupRelation.Size = new System.Drawing.Size(199, 17);
            this._cbUseDepartmentAclGroupRelation.TabIndex = 0;
            this._cbUseDepartmentAclGroupRelation.Text = "Use Department - ACL group relation";
            this._cbUseDepartmentAclGroupRelation.UseVisualStyleBackColor = true;
            this._cbUseDepartmentAclGroupRelation.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // _cbRemoveAclsDepartmentChange
            // 
            this._cbRemoveAclsDepartmentChange.AutoSize = true;
            this._cbRemoveAclsDepartmentChange.Enabled = false;
            this._cbRemoveAclsDepartmentChange.Location = new System.Drawing.Point(10, 42);
            this._cbRemoveAclsDepartmentChange.Name = "_cbRemoveAclsDepartmentChange";
            this._cbRemoveAclsDepartmentChange.Size = new System.Drawing.Size(313, 17);
            this._cbRemoveAclsDepartmentChange.TabIndex = 1;
            this._cbRemoveAclsDepartmentChange.Text = "Remove ACLs when changing department relation on person";
            this._cbRemoveAclsDepartmentChange.UseVisualStyleBackColor = true;
            this._cbRemoveAclsDepartmentChange.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _cbRemoveAclsDepartmentRemove
            // 
            this._cbRemoveAclsDepartmentRemove.AutoSize = true;
            this._cbRemoveAclsDepartmentRemove.Enabled = false;
            this._cbRemoveAclsDepartmentRemove.Location = new System.Drawing.Point(10, 65);
            this._cbRemoveAclsDepartmentRemove.Name = "_cbRemoveAclsDepartmentRemove";
            this._cbRemoveAclsDepartmentRemove.Size = new System.Drawing.Size(275, 17);
            this._cbRemoveAclsDepartmentRemove.TabIndex = 2;
            this._cbRemoveAclsDepartmentRemove.Text = "Remove ACLs when removing department on person";
            this._cbRemoveAclsDepartmentRemove.UseVisualStyleBackColor = true;
            this._cbRemoveAclsDepartmentRemove.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _cbUseForAllPersons
            // 
            this._cbUseForAllPersons.AutoSize = true;
            this._cbUseForAllPersons.Location = new System.Drawing.Point(3, 26);
            this._cbUseForAllPersons.Name = "_cbUseForAllPersons";
            this._cbUseForAllPersons.Size = new System.Drawing.Size(321, 17);
            this._cbUseForAllPersons.TabIndex = 13;
            this._cbUseForAllPersons.Text = "Use this ACL group for persons in the current site and sub-sites";
            this._cbUseForAllPersons.UseVisualStyleBackColor = true;
            this._cbUseForAllPersons.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // _gbValidityTimeOfAssigning
            // 
            this._gbValidityTimeOfAssigning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbValidityTimeOfAssigning.Controls.Add(this._chbApplyEndValidity);
            this._gbValidityTimeOfAssigning.Controls.Add(this._lDays);
            this._gbValidityTimeOfAssigning.Controls.Add(this._lMonths);
            this._gbValidityTimeOfAssigning.Controls.Add(this._nudDays);
            this._gbValidityTimeOfAssigning.Controls.Add(this._nudMonths);
            this._gbValidityTimeOfAssigning.Controls.Add(this._lYears);
            this._gbValidityTimeOfAssigning.Controls.Add(this._nudYears);
            this._gbValidityTimeOfAssigning.Location = new System.Drawing.Point(3, 166);
            this._gbValidityTimeOfAssigning.Name = "_gbValidityTimeOfAssigning";
            this._gbValidityTimeOfAssigning.Size = new System.Drawing.Size(496, 120);
            this._gbValidityTimeOfAssigning.TabIndex = 12;
            this._gbValidityTimeOfAssigning.TabStop = false;
            this._gbValidityTimeOfAssigning.Text = "Validity time of assigning";
            // 
            // _lDays
            // 
            this._lDays.AutoSize = true;
            this._lDays.Location = new System.Drawing.Point(6, 96);
            this._lDays.Name = "_lDays";
            this._lDays.Size = new System.Drawing.Size(31, 13);
            this._lDays.TabIndex = 16;
            this._lDays.Text = "Days";
            // 
            // _lMonths
            // 
            this._lMonths.AutoSize = true;
            this._lMonths.Location = new System.Drawing.Point(6, 70);
            this._lMonths.Name = "_lMonths";
            this._lMonths.Size = new System.Drawing.Size(42, 13);
            this._lMonths.TabIndex = 15;
            this._lMonths.Text = "Months";
            // 
            // _nudDays
            // 
            this._nudDays.Enabled = false;
            this._nudDays.Location = new System.Drawing.Point(106, 94);
            this._nudDays.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this._nudDays.Name = "_nudDays";
            this._nudDays.Size = new System.Drawing.Size(68, 20);
            this._nudDays.TabIndex = 14;
            this._nudDays.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _nudMonths
            // 
            this._nudMonths.Enabled = false;
            this._nudMonths.Location = new System.Drawing.Point(106, 68);
            this._nudMonths.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this._nudMonths.Name = "_nudMonths";
            this._nudMonths.Size = new System.Drawing.Size(68, 20);
            this._nudMonths.TabIndex = 13;
            this._nudMonths.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lYears
            // 
            this._lYears.AutoSize = true;
            this._lYears.Location = new System.Drawing.Point(6, 44);
            this._lYears.Name = "_lYears";
            this._lYears.Size = new System.Drawing.Size(34, 13);
            this._lYears.TabIndex = 12;
            this._lYears.Text = "Years";
            // 
            // _nudYears
            // 
            this._nudYears.Enabled = false;
            this._nudYears.Location = new System.Drawing.Point(106, 42);
            this._nudYears.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this._nudYears.Name = "_nudYears";
            this._nudYears.Size = new System.Drawing.Size(68, 20);
            this._nudYears.TabIndex = 11;
            this._nudYears.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._nudYears.ValueChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _chbIsImplicit
            // 
            this._chbIsImplicit.AutoSize = true;
            this._chbIsImplicit.Location = new System.Drawing.Point(3, 3);
            this._chbIsImplicit.Name = "_chbIsImplicit";
            this._chbIsImplicit.Size = new System.Drawing.Size(372, 17);
            this._chbIsImplicit.TabIndex = 10;
            this._chbIsImplicit.Text = "Use this ACL group for new persons without department in the current site";
            this._chbIsImplicit.UseVisualStyleBackColor = true;
            this._chbIsImplicit.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // _tpDepartments
            // 
            this._tpDepartments.BackColor = System.Drawing.SystemColors.Control;
            this._tpDepartments.Controls.Add(this._bEdit1);
            this._tpDepartments.Controls.Add(this._bDelete1);
            this._tpDepartments.Controls.Add(this._bAdd1);
            this._tpDepartments.Controls.Add(this._ilbDepartments);
            this._tpDepartments.Location = new System.Drawing.Point(4, 40);
            this._tpDepartments.Name = "_tpDepartments";
            this._tpDepartments.Size = new System.Drawing.Size(502, 285);
            this._tpDepartments.TabIndex = 3;
            this._tpDepartments.Text = "Departments";
            // 
            // _bEdit1
            // 
            this._bEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit1.Location = new System.Drawing.Point(421, 250);
            this._bEdit1.Name = "_bEdit1";
            this._bEdit1.Size = new System.Drawing.Size(75, 23);
            this._bEdit1.TabIndex = 6;
            this._bEdit1.Text = "Edit";
            this._bEdit1.UseVisualStyleBackColor = true;
            this._bEdit1.Click += new System.EventHandler(this._bEdit1_Click);
            // 
            // _bDelete1
            // 
            this._bDelete1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete1.Location = new System.Drawing.Point(340, 250);
            this._bDelete1.Name = "_bDelete1";
            this._bDelete1.Size = new System.Drawing.Size(75, 23);
            this._bDelete1.TabIndex = 5;
            this._bDelete1.Text = "Delete";
            this._bDelete1.UseVisualStyleBackColor = true;
            this._bDelete1.Click += new System.EventHandler(this._bDelete1_Click);
            // 
            // _bAdd1
            // 
            this._bAdd1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd1.Location = new System.Drawing.Point(259, 250);
            this._bAdd1.Name = "_bAdd1";
            this._bAdd1.Size = new System.Drawing.Size(75, 23);
            this._bAdd1.TabIndex = 4;
            this._bAdd1.Text = "Add";
            this._bAdd1.UseVisualStyleBackColor = true;
            this._bAdd1.Click += new System.EventHandler(this._bAdd1_Click);
            // 
            // _ilbDepartments
            // 
            this._ilbDepartments.AllowDrop = true;
            this._ilbDepartments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ilbDepartments.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._ilbDepartments.FormattingEnabled = true;
            this._ilbDepartments.ImageList = null;
            this._ilbDepartments.Location = new System.Drawing.Point(6, 6);
            this._ilbDepartments.Name = "_ilbDepartments";
            this._ilbDepartments.SelectedItemObject = null;
            this._ilbDepartments.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._ilbDepartments.Size = new System.Drawing.Size(490, 238);
            this._ilbDepartments.Sorted = true;
            this._ilbDepartments.TabIndex = 3;
            this._ilbDepartments.DragOver += new System.Windows.Forms.DragEventHandler(this._ilbDepartments_DragOver);
            this._ilbDepartments.DragDrop += new System.Windows.Forms.DragEventHandler(this._ilbDepartments_DragDrop);
            this._ilbDepartments.DoubleClick += new System.EventHandler(this._ilbDepartments_DoubleClick);
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 40);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(502, 285);
            this._tpUserFolders.TabIndex = 5;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(424, 259);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 28;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _lbUserFolders
            // 
            this._lbUserFolders.AllowDrop = false;
            this._lbUserFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbUserFolders.BackColor = System.Drawing.SystemColors.Info;
            this._lbUserFolders.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._lbUserFolders.FormattingEnabled = true;
            this._lbUserFolders.ImageList = null;
            this._lbUserFolders.Location = new System.Drawing.Point(0, 0);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(500, 251);
            this._lbUserFolders.TabIndex = 27;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 40);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Size = new System.Drawing.Size(502, 285);
            this._tpReferencedBy.TabIndex = 4;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _chbApplyEndValidity
            // 
            this._chbApplyEndValidity.AutoSize = true;
            this._chbApplyEndValidity.Location = new System.Drawing.Point(6, 19);
            this._chbApplyEndValidity.Name = "_chbApplyEndValidity";
            this._chbApplyEndValidity.Size = new System.Drawing.Size(131, 17);
            this._chbApplyEndValidity.TabIndex = 17;
            this._chbApplyEndValidity.Text = "Apply ACL end validity";
            this._chbApplyEndValidity.UseVisualStyleBackColor = true;
            this._chbApplyEndValidity.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            this._chbApplyEndValidity.Enabled = false;
            // 
            // NCASACLGruopEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(534, 412);
            this.Controls.Add(this._bApply);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._tcSettings);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._eName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(550, 451);
            this.Name = "NCASACLGruopEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NCASACLGruopEditForm";
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._tpAccessControlLists.ResumeLayout(false);
            this._tcSettings.ResumeLayout(false);
            this._tpImplicitAssigningConfiguration.ResumeLayout(false);
            this._tpImplicitAssigningConfiguration.PerformLayout();
            this._gbDepartmentAclGroupRelation.ResumeLayout(false);
            this._gbDepartmentAclGroupRelation.PerformLayout();
            this._gbValidityTimeOfAssigning.ResumeLayout(false);
            this._gbValidityTimeOfAssigning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._nudDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudMonths)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._nudYears)).EndInit();
            this._tpDepartments.ResumeLayout(false);
            this._tpUserFolders.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Button _bApply;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.TabPage _tpAccessControlLists;
        private System.Windows.Forms.TabControl _tcSettings;
        private System.Windows.Forms.CheckBox _chbIsImplicit;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bAdd;
        private Contal.IwQuick.UI.ImageListBox _ilbAccessControlLists;
        private System.Windows.Forms.TabPage _tpImplicitAssigningConfiguration;
        private System.Windows.Forms.CheckBox _cbUseDepartmentAclGroupRelation;
        private System.Windows.Forms.CheckBox _cbRemoveAclsDepartmentChange;
        private System.Windows.Forms.CheckBox _cbRemoveAclsDepartmentRemove;
        private System.Windows.Forms.NumericUpDown _nudYears;
        private System.Windows.Forms.GroupBox _gbValidityTimeOfAssigning;
        private System.Windows.Forms.Label _lDays;
        private System.Windows.Forms.Label _lMonths;
        private System.Windows.Forms.NumericUpDown _nudDays;
        private System.Windows.Forms.NumericUpDown _nudMonths;
        private System.Windows.Forms.Label _lYears;
        private System.Windows.Forms.TabPage _tpDepartments;
        private System.Windows.Forms.Button _bDelete1;
        private System.Windows.Forms.Button _bAdd1;
        private Contal.IwQuick.UI.ImageListBox _ilbDepartments;
        private System.Windows.Forms.CheckBox _cbUseForAllPersons;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.GroupBox _gbDepartmentAclGroupRelation;
        private System.Windows.Forms.Button _bEdit1;
        private System.Windows.Forms.CheckBox _cbRemoveAllAcls;
        private System.Windows.Forms.CheckBox _chbApplyEndValidity;
    }
}
