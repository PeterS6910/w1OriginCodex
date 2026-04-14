namespace Contal.Cgp.Client
{
    partial class LoginEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginEditForm));
            this._lPerson = new System.Windows.Forms.Label();
            this._lPublicKey = new System.Windows.Forms.Label();
            this._ePublicKey = new System.Windows.Forms.TextBox();
            this._lPassword = new System.Windows.Forms.Label();
            this._ePassword = new System.Windows.Forms.TextBox();
            this._cbDisabled = new System.Windows.Forms.CheckBox();
            this._lExpirationDate = new System.Windows.Forms.Label();
            this._lUserName = new System.Windows.Forms.Label();
            this._lDisabled = new System.Windows.Forms.Label();
            this._eUserName = new System.Windows.Forms.TextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._eDescription = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._tbProperties = new System.Windows.Forms.TabPage();
            this._lLoginGroupIsDisabled = new System.Windows.Forms.Label();
            this._tbmLoginGroup = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._lLoginGroup = new System.Windows.Forms.Label();
            this._tbmPerson = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._tbdpExpirationDate = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._lMustChangePassword = new System.Windows.Forms.Label();
            this._cbMustChangePassword = new System.Windows.Forms.CheckBox();
            this._tbAccessControl = new System.Windows.Forms.TabPage();
            this._panelClbAccess = new System.Windows.Forms.Panel();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tbDescription = new System.Windows.Forms.TabPage();
            this._panelBack = new System.Windows.Forms.Panel();
            this._bClone = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this._tbProperties.SuspendLayout();
            this._tbAccessControl.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tbDescription.SuspendLayout();
            this._panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lPerson
            // 
            this._lPerson.AutoSize = true;
            this._lPerson.Location = new System.Drawing.Point(8, 251);
            this._lPerson.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPerson.Name = "_lPerson";
            this._lPerson.Size = new System.Drawing.Size(50, 16);
            this._lPerson.TabIndex = 12;
            this._lPerson.Text = "Person";
            // 
            // _lPublicKey
            // 
            this._lPublicKey.AutoSize = true;
            this._lPublicKey.Location = new System.Drawing.Point(8, 76);
            this._lPublicKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPublicKey.Name = "_lPublicKey";
            this._lPublicKey.Size = new System.Drawing.Size(69, 16);
            this._lPublicKey.TabIndex = 4;
            this._lPublicKey.Text = "Public key";
            this._lPublicKey.Visible = false;
            // 
            // _ePublicKey
            // 
            this._ePublicKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._ePublicKey.Location = new System.Drawing.Point(175, 72);
            this._ePublicKey.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._ePublicKey.Name = "_ePublicKey";
            this._ePublicKey.Size = new System.Drawing.Size(674, 22);
            this._ePublicKey.TabIndex = 5;
            this._ePublicKey.Visible = false;
            this._ePublicKey.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lPassword
            // 
            this._lPassword.AutoSize = true;
            this._lPassword.Location = new System.Drawing.Point(8, 44);
            this._lPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPassword.Name = "_lPassword";
            this._lPassword.Size = new System.Drawing.Size(67, 16);
            this._lPassword.TabIndex = 2;
            this._lPassword.Text = "Password";
            // 
            // _ePassword
            // 
            this._ePassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._ePassword.Location = new System.Drawing.Point(174, 40);
            this._ePassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._ePassword.Name = "_ePassword";
            this._ePassword.Size = new System.Drawing.Size(675, 22);
            this._ePassword.TabIndex = 3;
            this._ePassword.UseSystemPasswordChar = true;
            this._ePassword.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _cbDisabled
            // 
            this._cbDisabled.AutoSize = true;
            this._cbDisabled.Location = new System.Drawing.Point(240, 138);
            this._cbDisabled.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cbDisabled.Name = "_cbDisabled";
            this._cbDisabled.Size = new System.Drawing.Size(18, 17);
            this._cbDisabled.TabIndex = 7;
            this._cbDisabled.UseVisualStyleBackColor = true;
            this._cbDisabled.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lExpirationDate
            // 
            this._lExpirationDate.AutoSize = true;
            this._lExpirationDate.Location = new System.Drawing.Point(8, 219);
            this._lExpirationDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lExpirationDate.Name = "_lExpirationDate";
            this._lExpirationDate.Size = new System.Drawing.Size(96, 16);
            this._lExpirationDate.TabIndex = 10;
            this._lExpirationDate.Text = "Expiration date";
            // 
            // _lUserName
            // 
            this._lUserName.AutoSize = true;
            this._lUserName.Location = new System.Drawing.Point(8, 11);
            this._lUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lUserName.Name = "_lUserName";
            this._lUserName.Size = new System.Drawing.Size(70, 16);
            this._lUserName.TabIndex = 0;
            this._lUserName.Text = "Username";
            // 
            // _lDisabled
            // 
            this._lDisabled.AutoSize = true;
            this._lDisabled.Location = new System.Drawing.Point(8, 139);
            this._lDisabled.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDisabled.Name = "_lDisabled";
            this._lDisabled.Size = new System.Drawing.Size(62, 16);
            this._lDisabled.TabIndex = 6;
            this._lDisabled.Text = "Disabled";
            // 
            // _eUserName
            // 
            this._eUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eUserName.Location = new System.Drawing.Point(174, 8);
            this._eUserName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eUserName.Name = "_eUserName";
            this._eUserName.Size = new System.Drawing.Size(675, 22);
            this._eUserName.TabIndex = 1;
            this._eUserName.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(832, 440);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 32);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(732, 440);
            this._bOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(94, 32);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(4, 4);
            this._eDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(896, 365);
            this._eDescription.TabIndex = 0;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this._tbProperties);
            this.tabControl1.Controls.Add(this._tbAccessControl);
            this.tabControl1.Controls.Add(this._tpUserFolders);
            this.tabControl1.Controls.Add(this._tpReferencedBy);
            this.tabControl1.Controls.Add(this._tbDescription);
            this.tabControl1.Location = new System.Drawing.Point(15, 30);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(912, 402);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.TabStop = false;
            // 
            // _tbProperties
            // 
            this._tbProperties.BackColor = System.Drawing.SystemColors.Control;
            this._tbProperties.Controls.Add(this._lLoginGroupIsDisabled);
            this._tbProperties.Controls.Add(this._tbmLoginGroup);
            this._tbProperties.Controls.Add(this._lLoginGroup);
            this._tbProperties.Controls.Add(this._tbmPerson);
            this._tbProperties.Controls.Add(this._tbdpExpirationDate);
            this._tbProperties.Controls.Add(this._lMustChangePassword);
            this._tbProperties.Controls.Add(this._cbMustChangePassword);
            this._tbProperties.Controls.Add(this._lUserName);
            this._tbProperties.Controls.Add(this._eUserName);
            this._tbProperties.Controls.Add(this._lDisabled);
            this._tbProperties.Controls.Add(this._lExpirationDate);
            this._tbProperties.Controls.Add(this._cbDisabled);
            this._tbProperties.Controls.Add(this._ePassword);
            this._tbProperties.Controls.Add(this._lPerson);
            this._tbProperties.Controls.Add(this._lPassword);
            this._tbProperties.Controls.Add(this._ePublicKey);
            this._tbProperties.Controls.Add(this._lPublicKey);
            this._tbProperties.Location = new System.Drawing.Point(4, 25);
            this._tbProperties.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbProperties.Name = "_tbProperties";
            this._tbProperties.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbProperties.Size = new System.Drawing.Size(904, 373);
            this._tbProperties.TabIndex = 0;
            this._tbProperties.Text = "Properties";
            // 
            // _lLoginGroupIsDisabled
            // 
            this._lLoginGroupIsDisabled.AutoSize = true;
            this._lLoginGroupIsDisabled.ForeColor = System.Drawing.Color.Red;
            this._lLoginGroupIsDisabled.Location = new System.Drawing.Point(266, 139);
            this._lLoginGroupIsDisabled.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lLoginGroupIsDisabled.Name = "_lLoginGroupIsDisabled";
            this._lLoginGroupIsDisabled.Size = new System.Drawing.Size(147, 16);
            this._lLoginGroupIsDisabled.TabIndex = 16;
            this._lLoginGroupIsDisabled.Text = "Login group is disabled";
            this._lLoginGroupIsDisabled.Visible = false;
            // 
            // _tbmLoginGroup
            // 
            this._tbmLoginGroup.AllowDrop = true;
            this._tbmLoginGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmLoginGroup.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmLoginGroup.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmLoginGroup.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmLoginGroup.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmLoginGroup.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmLoginGroup.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmLoginGroup.Button.Image")));
            this._tbmLoginGroup.Button.Location = new System.Drawing.Point(148, 0);
            this._tbmLoginGroup.Button.Name = "_bMenu";
            this._tbmLoginGroup.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmLoginGroup.Button.TabIndex = 3;
            this._tbmLoginGroup.Button.UseVisualStyleBackColor = false;
            this._tbmLoginGroup.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmLoginGroup.ButtonDefaultBehaviour = true;
            this._tbmLoginGroup.ButtonHoverColor = System.Drawing.Color.Empty;
            this._tbmLoginGroup.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmLoginGroup.ButtonImage")));
            // 
            // 
            // 
            this._tbmLoginGroup.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2});
            this._tbmLoginGroup.ButtonPopupMenu.Name = "";
            this._tbmLoginGroup.ButtonPopupMenu.Size = new System.Drawing.Size(133, 52);
            this._tbmLoginGroup.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmLoginGroup.ButtonShowImage = true;
            this._tbmLoginGroup.ButtonSizeHeight = 20;
            this._tbmLoginGroup.ButtonSizeWidth = 20;
            this._tbmLoginGroup.ButtonText = "";
            this._tbmLoginGroup.HoverTime = 500;
            // 
            // 
            // 
            this._tbmLoginGroup.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmLoginGroup.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmLoginGroup.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmLoginGroup.ImageTextBox.ContextMenuStrip = this._tbmLoginGroup.ButtonPopupMenu;
            this._tbmLoginGroup.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmLoginGroup.ImageTextBox.Image")));
            this._tbmLoginGroup.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmLoginGroup.ImageTextBox.Name = "_itbTextBox";
            this._tbmLoginGroup.ImageTextBox.NoTextNoImage = true;
            this._tbmLoginGroup.ImageTextBox.ReadOnly = false;
            this._tbmLoginGroup.ImageTextBox.Size = new System.Drawing.Size(148, 20);
            this._tbmLoginGroup.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmLoginGroup.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmLoginGroup.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmLoginGroup.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmLoginGroup.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmLoginGroup.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmLoginGroup.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmLoginGroup.ImageTextBox.TextBox.Size = new System.Drawing.Size(146, 15);
            this._tbmLoginGroup.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmLoginGroup.ImageTextBox.UseImage = true;
            this._tbmLoginGroup.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmLoginGroup.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmLoginGroup_ImageTextBox_DoubleClick);
            this._tbmLoginGroup.Location = new System.Drawing.Point(174, 105);
            this._tbmLoginGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbmLoginGroup.MaximumSize = new System.Drawing.Size(1500, 69);
            this._tbmLoginGroup.MinimumSize = new System.Drawing.Size(38, 25);
            this._tbmLoginGroup.Name = "_tbmLoginGroup";
            this._tbmLoginGroup.Size = new System.Drawing.Size(168, 20);
            this._tbmLoginGroup.TabIndex = 15;
            this._tbmLoginGroup.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmLoginGroup.TextImage")));
            this._tbmLoginGroup.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmLoginGroup_ButtonPopupMenuItemClick);
            this._tbmLoginGroup.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmLoginGroup_DragDrop);
            this._tbmLoginGroup.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmLoginGroup_DragOver);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(132, 24);
            this._tsiModify2.Text = "Modify";
            // 
            // _tsiRemove2
            // 
            this._tsiRemove2.Name = "_tsiRemove2";
            this._tsiRemove2.Size = new System.Drawing.Size(132, 24);
            this._tsiRemove2.Text = "Remove";
            // 
            // _lLoginGroup
            // 
            this._lLoginGroup.AutoSize = true;
            this._lLoginGroup.Location = new System.Drawing.Point(8, 108);
            this._lLoginGroup.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lLoginGroup.Name = "_lLoginGroup";
            this._lLoginGroup.Size = new System.Drawing.Size(78, 16);
            this._lLoginGroup.TabIndex = 14;
            this._lLoginGroup.Text = "Login group";
            // 
            // _tbmPerson
            // 
            this._tbmPerson.AllowDrop = true;
            this._tbmPerson.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this._tbmPerson.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmPerson.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPerson.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmPerson.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.Button.Image")));
            this._tbmPerson.Button.Location = new System.Drawing.Point(148, 0);
            this._tbmPerson.Button.Name = "_bMenu";
            this._tbmPerson.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmPerson.Button.TabIndex = 3;
            this._tbmPerson.Button.UseVisualStyleBackColor = false;
            this._tbmPerson.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmPerson.ButtonDefaultBehaviour = true;
            this._tbmPerson.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmPerson.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.ButtonImage")));
            // 
            // 
            // 
            this._tbmPerson.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiRemove});
            this._tbmPerson.ButtonPopupMenu.Name = "";
            this._tbmPerson.ButtonPopupMenu.Size = new System.Drawing.Size(133, 52);
            this._tbmPerson.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmPerson.ButtonShowImage = true;
            this._tbmPerson.ButtonSizeHeight = 20;
            this._tbmPerson.ButtonSizeWidth = 20;
            this._tbmPerson.ButtonText = "";
            this._tbmPerson.HoverTime = 500;
            // 
            // 
            // 
            this._tbmPerson.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmPerson.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmPerson.ImageTextBox.ContextMenuStrip = this._tbmPerson.ButtonPopupMenu;
            this._tbmPerson.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.ImageTextBox.Image")));
            this._tbmPerson.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmPerson.ImageTextBox.Name = "_textBox";
            this._tbmPerson.ImageTextBox.NoTextNoImage = true;
            this._tbmPerson.ImageTextBox.ReadOnly = true;
            this._tbmPerson.ImageTextBox.Size = new System.Drawing.Size(148, 20);
            this._tbmPerson.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmPerson.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmPerson.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmPerson.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmPerson.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmPerson.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmPerson.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmPerson.ImageTextBox.TextBox.Size = new System.Drawing.Size(146, 15);
            this._tbmPerson.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmPerson.ImageTextBox.UseImage = true;
            this._tbmPerson.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmPerson.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmDailyPlan_DoubleClick);
            this._tbmPerson.Location = new System.Drawing.Point(175, 249);
            this._tbmPerson.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbmPerson.MaximumSize = new System.Drawing.Size(1500, 25);
            this._tbmPerson.MinimumSize = new System.Drawing.Size(26, 25);
            this._tbmPerson.Name = "_tbmPerson";
            this._tbmPerson.Size = new System.Drawing.Size(168, 20);
            this._tbmPerson.TabIndex = 13;
            this._tbmPerson.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.TextImage")));
            this._tbmPerson.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmDailyPlan_ButtonPopupMenuItemClick);
            this._tbmPerson.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmDailyPlan_DragDrop);
            this._tbmPerson.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmDailyPlan_DragOver);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(132, 24);
            this._tsiModify.Text = "Modify";
            // 
            // _tsiRemove
            // 
            this._tsiRemove.Name = "_tsiRemove";
            this._tsiRemove.Size = new System.Drawing.Size(132, 24);
            this._tsiRemove.Text = "Remove";
            // 
            // _tbdpExpirationDate
            // 
            this._tbdpExpirationDate.addActualTime = false;
            this._tbdpExpirationDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbdpExpirationDate.BackColor = System.Drawing.Color.Transparent;
            this._tbdpExpirationDate.ButtonClearDateImage = null;
            this._tbdpExpirationDate.ButtonClearDateText = "";
            this._tbdpExpirationDate.ButtonClearDateWidth = 23;
            this._tbdpExpirationDate.ButtonDateImage = null;
            this._tbdpExpirationDate.ButtonDateText = "";
            this._tbdpExpirationDate.ButtonDateWidth = 23;
            this._tbdpExpirationDate.CustomFormat = "dd.MM.yyyy";
            this._tbdpExpirationDate.DateFormName = "Calendar";
            this._tbdpExpirationDate.LocalizationHelper = null;
            this._tbdpExpirationDate.Location = new System.Drawing.Point(175, 212);
            this._tbdpExpirationDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbdpExpirationDate.MaximumSize = new System.Drawing.Size(1250, 75);
            this._tbdpExpirationDate.MinimumSize = new System.Drawing.Size(125, 28);
            this._tbdpExpirationDate.Name = "_tbdpExpirationDate";
            this._tbdpExpirationDate.ReadOnly = false;
            this._tbdpExpirationDate.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.Unknown;
            this._tbdpExpirationDate.Size = new System.Drawing.Size(675, 28);
            this._tbdpExpirationDate.TabIndex = 11;
            this._tbdpExpirationDate.ValidateAfter = 2D;
            this._tbdpExpirationDate.ValidationEnabled = false;
            this._tbdpExpirationDate.ValidationError = "";
            this._tbdpExpirationDate.Value = null;
            this._tbdpExpirationDate.ButtonClearDateClick += new Contal.IwQuick.UI.TextBoxDatePicker.DButtonClicked(this._tbdpExpirationDate_ButtonClearDateClick);
            this._tbdpExpirationDate.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this.EditTextChanger);
            // 
            // _lMustChangePassword
            // 
            this._lMustChangePassword.AutoSize = true;
            this._lMustChangePassword.Location = new System.Drawing.Point(8, 164);
            this._lMustChangePassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMustChangePassword.Name = "_lMustChangePassword";
            this._lMustChangePassword.Size = new System.Drawing.Size(189, 16);
            this._lMustChangePassword.TabIndex = 8;
            this._lMustChangePassword.Text = "Change password at next login";
            // 
            // _cbMustChangePassword
            // 
            this._cbMustChangePassword.AutoSize = true;
            this._cbMustChangePassword.Location = new System.Drawing.Point(240, 162);
            this._cbMustChangePassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._cbMustChangePassword.Name = "_cbMustChangePassword";
            this._cbMustChangePassword.Size = new System.Drawing.Size(18, 17);
            this._cbMustChangePassword.TabIndex = 9;
            this._cbMustChangePassword.UseVisualStyleBackColor = true;
            this._cbMustChangePassword.CheckedChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tbAccessControl
            // 
            this._tbAccessControl.BackColor = System.Drawing.SystemColors.Control;
            this._tbAccessControl.Controls.Add(this._panelClbAccess);
            this._tbAccessControl.Location = new System.Drawing.Point(4, 25);
            this._tbAccessControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbAccessControl.Name = "_tbAccessControl";
            this._tbAccessControl.Size = new System.Drawing.Size(904, 373);
            this._tbAccessControl.TabIndex = 2;
            this._tbAccessControl.Text = "Access control";
            // 
            // _panelClbAccess
            // 
            this._panelClbAccess.AutoScroll = true;
            this._panelClbAccess.BackColor = System.Drawing.SystemColors.Window;
            this._panelClbAccess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._panelClbAccess.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelClbAccess.Location = new System.Drawing.Point(0, 0);
            this._panelClbAccess.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelClbAccess.Name = "_panelClbAccess";
            this._panelClbAccess.Size = new System.Drawing.Size(904, 373);
            this._panelClbAccess.TabIndex = 1;
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 25);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(904, 373);
            this._tpUserFolders.TabIndex = 4;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(339, 256);
            this._bRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(94, 32);
            this._bRefresh.TabIndex = 0;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _lbUserFolders
            // 
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
            this._lbUserFolders.Size = new System.Drawing.Size(435, 238);
            this._lbUserFolders.TabIndex = 16;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 25);
            this._tpReferencedBy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Size = new System.Drawing.Size(904, 373);
            this._tpReferencedBy.TabIndex = 3;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tbDescription
            // 
            this._tbDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tbDescription.Controls.Add(this._eDescription);
            this._tbDescription.Location = new System.Drawing.Point(4, 25);
            this._tbDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbDescription.Name = "_tbDescription";
            this._tbDescription.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbDescription.Size = new System.Drawing.Size(904, 373);
            this._tbDescription.TabIndex = 1;
            this._tbDescription.Text = "Description";
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._bClone);
            this._panelBack.Controls.Add(this.tabControl1);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(941, 484);
            this._panelBack.TabIndex = 0;
            // 
            // _bClone
            // 
            this._bClone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bClone.Location = new System.Drawing.Point(15, 440);
            this._bClone.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bClone.Name = "_bClone";
            this._bClone.Size = new System.Drawing.Size(94, 32);
            this._bClone.TabIndex = 1;
            this._bClone.Text = "Clone";
            this._bClone.UseVisualStyleBackColor = true;
            this._bClone.Click += new System.EventHandler(this._bClone_Click);
            // 
            // LoginEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(941, 484);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(957, 521);
            this.Name = "LoginEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit login";
            this.tabControl1.ResumeLayout(false);
            this._tbProperties.ResumeLayout(false);
            this._tbProperties.PerformLayout();
            this._tbAccessControl.ResumeLayout(false);
            this._tpUserFolders.ResumeLayout(false);
            this._tbDescription.ResumeLayout(false);
            this._tbDescription.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _lPerson;
        private System.Windows.Forms.Label _lPublicKey;
        private System.Windows.Forms.TextBox _ePublicKey;
        private System.Windows.Forms.Label _lPassword;
        private System.Windows.Forms.TextBox _ePassword;
        private System.Windows.Forms.CheckBox _cbDisabled;
        private System.Windows.Forms.Label _lExpirationDate;
        private System.Windows.Forms.Label _lUserName;
        private System.Windows.Forms.Label _lDisabled;
        private System.Windows.Forms.TextBox _eUserName;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _tbProperties;
        private System.Windows.Forms.TabPage _tbDescription;
        private System.Windows.Forms.TabPage _tbAccessControl;
        private System.Windows.Forms.Panel _panelClbAccess;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.Label _lMustChangePassword;
        private System.Windows.Forms.CheckBox _cbMustChangePassword;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpExpirationDate;
        private Contal.IwQuick.UI.TextBoxMenu _tbmPerson;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
        private System.Windows.Forms.Button _bClone;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.Label _lLoginGroup;
        private Contal.IwQuick.UI.TextBoxMenu _tbmLoginGroup;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
        private System.Windows.Forms.Label _lLoginGroupIsDisabled;
    }
}