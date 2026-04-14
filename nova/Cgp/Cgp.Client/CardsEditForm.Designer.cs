namespace Contal.Cgp.Client
{
    partial class CardsEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardsEditForm));
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._tcCard = new System.Windows.Forms.TabControl();
            this._tpProperties = new System.Windows.Forms.TabPage();
            this._bCreateCard = new System.Windows.Forms.Button();
            this._eAlternateCardNumber = new System.Windows.Forms.TextBox();
            this._lAlternateCardNumber = new System.Windows.Forms.Label();
            this._tbmRelatedCard = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify2 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove2 = new System.Windows.Forms.ToolStripMenuItem();
            this._lRelatedCardTypeValue = new System.Windows.Forms.Label();
            this._lRelatedCardType = new System.Windows.Forms.Label();
            this._lRelatedCard = new System.Windows.Forms.Label();
            this._icbCardState = new Contal.IwQuick.UI.ImageComboBox();
            this._tbmCardSystem = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiCreate = new System.Windows.Forms.ToolStripMenuItem();
            this._tbmPerson = new Contal.IwQuick.UI.TextBoxMenu();
            this._tsiModify1 = new System.Windows.Forms.ToolStripMenuItem();
            this._tsiRemove1 = new System.Windows.Forms.ToolStripMenuItem();
            this._lActualCardType = new System.Windows.Forms.Label();
            this._lCardType = new System.Windows.Forms.Label();
            this._lPerson = new System.Windows.Forms.Label();
            this._lActualFullCardNumber = new System.Windows.Forms.Label();
            this._lFullCardNumber = new System.Windows.Forms.Label();
            this._eDateStateLastChange = new System.Windows.Forms.TextBox();
            this._lDateStateLastChange = new System.Windows.Forms.Label();
            this._lCardState = new System.Windows.Forms.Label();
            this._ePin = new System.Windows.Forms.TextBox();
            this._lPin = new System.Windows.Forms.Label();
            this._eCardNumber = new System.Windows.Forms.TextBox();
            this._lCardNumber = new System.Windows.Forms.Label();
            this._lCardSystem = new System.Windows.Forms.Label();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpTimetecSettings = new System.Windows.Forms.TabPage();
            this._lValidityDateTo = new System.Windows.Forms.Label();
            this._lValidatyDateFrom = new System.Windows.Forms.Label();
            this._dpValidityDateTo = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._dpValidityDateFrom = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._panelBack = new System.Windows.Forms.Panel();
            this._tcCard.SuspendLayout();
            this._tpProperties.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpTimetecSettings.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(480, 570);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(120, 33);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(351, 570);
            this._bOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(120, 33);
            this._bOk.TabIndex = 1;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _tcCard
            // 
            this._tcCard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tcCard.Controls.Add(this._tpProperties);
            this._tcCard.Controls.Add(this._tpUserFolders);
            this._tcCard.Controls.Add(this._tpTimetecSettings);
            this._tcCard.Controls.Add(this._tpReferencedBy);
            this._tcCard.Controls.Add(this._tpDescription);
            this._tcCard.Location = new System.Drawing.Point(18, 18);
            this._tcCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tcCard.Multiline = true;
            this._tcCard.Name = "_tcCard";
            this._tcCard.SelectedIndex = 0;
            this._tcCard.Size = new System.Drawing.Size(582, 543);
            this._tcCard.TabIndex = 0;
            this._tcCard.TabStop = false;
            // 
            // _tpProperties
            // 
            this._tpProperties.BackColor = System.Drawing.Color.Transparent;
            this._tpProperties.Controls.Add(this._bCreateCard);
            this._tpProperties.Controls.Add(this._eAlternateCardNumber);
            this._tpProperties.Controls.Add(this._lAlternateCardNumber);
            this._tpProperties.Controls.Add(this._tbmRelatedCard);
            this._tpProperties.Controls.Add(this._lRelatedCardTypeValue);
            this._tpProperties.Controls.Add(this._lRelatedCardType);
            this._tpProperties.Controls.Add(this._lRelatedCard);
            this._tpProperties.Controls.Add(this._icbCardState);
            this._tpProperties.Controls.Add(this._tbmCardSystem);
            this._tpProperties.Controls.Add(this._tbmPerson);
            this._tpProperties.Controls.Add(this._lActualCardType);
            this._tpProperties.Controls.Add(this._lCardType);
            this._tpProperties.Controls.Add(this._lPerson);
            this._tpProperties.Controls.Add(this._lActualFullCardNumber);
            this._tpProperties.Controls.Add(this._lFullCardNumber);
            this._tpProperties.Controls.Add(this._eDateStateLastChange);
            this._tpProperties.Controls.Add(this._lDateStateLastChange);
            this._tpProperties.Controls.Add(this._lCardState);
            this._tpProperties.Controls.Add(this._ePin);
            this._tpProperties.Controls.Add(this._lPin);
            this._tpProperties.Controls.Add(this._eCardNumber);
            this._tpProperties.Controls.Add(this._lCardNumber);
            this._tpProperties.Controls.Add(this._lCardSystem);
            this._tpProperties.Location = new System.Drawing.Point(4, 29);
            this._tpProperties.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpProperties.Name = "_tpProperties";
            this._tpProperties.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpProperties.Size = new System.Drawing.Size(574, 510);
            this._tpProperties.TabIndex = 0;
            this._tpProperties.Text = "Properties";
            this._tpProperties.UseVisualStyleBackColor = true;
            // 
            // _bCreateCard
            // 
            this._bCreateCard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreateCard.Location = new System.Drawing.Point(424, 316);
            this._bCreateCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCreateCard.Name = "_bCreateCard";
            this._bCreateCard.Size = new System.Drawing.Size(120, 33);
            this._bCreateCard.TabIndex = 21;
            this._bCreateCard.Text = "Create or Edit";
            this._bCreateCard.UseVisualStyleBackColor = true;
            this._bCreateCard.Click += new System.EventHandler(this._bCreateCard_Click);
            // 
            // _eAlternateCardNumber
            // 
            this._eAlternateCardNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eAlternateCardNumber.Location = new System.Drawing.Point(222, 249);
            this._eAlternateCardNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eAlternateCardNumber.Name = "_eAlternateCardNumber";
            this._eAlternateCardNumber.Size = new System.Drawing.Size(192, 26);
            this._eAlternateCardNumber.TabIndex = 20;
            this._eAlternateCardNumber.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _lAlternateCardNumber
            // 
            this._lAlternateCardNumber.AutoSize = true;
            this._lAlternateCardNumber.Location = new System.Drawing.Point(10, 254);
            this._lAlternateCardNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lAlternateCardNumber.Name = "_lAlternateCardNumber";
            this._lAlternateCardNumber.Size = new System.Drawing.Size(167, 20);
            this._lAlternateCardNumber.TabIndex = 19;
            this._lAlternateCardNumber.Text = "Alternate card number";
            // 
            // _tbmRelatedCard
            // 
            this._tbmRelatedCard.AllowDrop = true;
            this._tbmRelatedCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmRelatedCard.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this._tbmRelatedCard.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmRelatedCard.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmRelatedCard.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmRelatedCard.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmRelatedCard.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmRelatedCard.Button.Image")));
            this._tbmRelatedCard.Button.Location = new System.Drawing.Point(109, 0);
            this._tbmRelatedCard.Button.Name = "_bMenu";
            this._tbmRelatedCard.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmRelatedCard.Button.TabIndex = 3;
            this._tbmRelatedCard.Button.UseVisualStyleBackColor = false;
            this._tbmRelatedCard.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmRelatedCard.ButtonDefaultBehaviour = true;
            this._tbmRelatedCard.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmRelatedCard.ButtonImage = ((System.Drawing.Image)(resources.GetObject("_tbmRelatedCard.ButtonImage")));
            // 
            // 
            // 
            this._tbmRelatedCard.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify2,
            this._tsiRemove2});
            this._tbmRelatedCard.ButtonPopupMenu.Name = "";
            this._tbmRelatedCard.ButtonPopupMenu.Size = new System.Drawing.Size(244, 68);
            this._tbmRelatedCard.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmRelatedCard.ButtonShowImage = true;
            this._tbmRelatedCard.ButtonSizeHeight = 20;
            this._tbmRelatedCard.ButtonSizeWidth = 20;
            this._tbmRelatedCard.ButtonText = "";
            this._tbmRelatedCard.HoverTime = 500;
            // 
            // 
            // 
            this._tbmRelatedCard.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmRelatedCard.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmRelatedCard.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmRelatedCard.ImageTextBox.ContextMenuStrip = this._tbmRelatedCard.ButtonPopupMenu;
            this._tbmRelatedCard.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmRelatedCard.ImageTextBox.Image")));
            this._tbmRelatedCard.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmRelatedCard.ImageTextBox.Name = "_itbTextBox";
            this._tbmRelatedCard.ImageTextBox.NoTextNoImage = true;
            this._tbmRelatedCard.ImageTextBox.ReadOnly = true;
            this._tbmRelatedCard.ImageTextBox.Size = new System.Drawing.Size(109, 20);
            this._tbmRelatedCard.ImageTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._tbmRelatedCard.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmRelatedCard.ImageTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._tbmRelatedCard.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmRelatedCard.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmRelatedCard.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmRelatedCard.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmRelatedCard.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmRelatedCard.ImageTextBox.TextBox.Size = new System.Drawing.Size(107, 19);
            this._tbmRelatedCard.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmRelatedCard.ImageTextBox.UseImage = true;
            this._tbmRelatedCard.ImageTextBox.TextChanged += new System.EventHandler(this.RelatedCardTextChanged);
            this._tbmRelatedCard.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmRelatedCard_DoubleClick);
            this._tbmRelatedCard.Location = new System.Drawing.Point(222, 210);
            this._tbmRelatedCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbmRelatedCard.MaximumSize = new System.Drawing.Size(1800, 82);
            this._tbmRelatedCard.MinimumSize = new System.Drawing.Size(45, 30);
            this._tbmRelatedCard.Name = "_tbmRelatedCard";
            this._tbmRelatedCard.Size = new System.Drawing.Size(129, 20);
            this._tbmRelatedCard.TabIndex = 18;
            this._tbmRelatedCard.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmRelatedCard.TextImage")));
            this._tbmRelatedCard.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmRelatedCard_ButtonPopupMenuItemClick);
            this._tbmRelatedCard.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmRelatedCard_DragDrop);
            this._tbmRelatedCard.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmRelatedCard_DragOver);
            // 
            // _tsiModify2
            // 
            this._tsiModify2.Name = "_tsiModify2";
            this._tsiModify2.Size = new System.Drawing.Size(243, 32);
            this._tsiModify2.Text = "toolStripMenuItem1";
            // 
            // _tsiRemove2
            // 
            this._tsiRemove2.Name = "_tsiRemove2";
            this._tsiRemove2.Size = new System.Drawing.Size(243, 32);
            this._tsiRemove2.Text = "toolStripMenuItem2";
            // 
            // _lRelatedCardTypeValue
            // 
            this._lRelatedCardTypeValue.AutoSize = true;
            this._lRelatedCardTypeValue.Location = new System.Drawing.Point(218, 177);
            this._lRelatedCardTypeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lRelatedCardTypeValue.Name = "_lRelatedCardTypeValue";
            this._lRelatedCardTypeValue.Size = new System.Drawing.Size(0, 20);
            this._lRelatedCardTypeValue.TabIndex = 17;
            // 
            // _lRelatedCardType
            // 
            this._lRelatedCardType.AutoSize = true;
            this._lRelatedCardType.Location = new System.Drawing.Point(10, 177);
            this._lRelatedCardType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lRelatedCardType.Name = "_lRelatedCardType";
            this._lRelatedCardType.Size = new System.Drawing.Size(134, 20);
            this._lRelatedCardType.TabIndex = 16;
            this._lRelatedCardType.Text = "Related card type";
            // 
            // _lRelatedCard
            // 
            this._lRelatedCard.AutoSize = true;
            this._lRelatedCard.Location = new System.Drawing.Point(10, 216);
            this._lRelatedCard.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lRelatedCard.Name = "_lRelatedCard";
            this._lRelatedCard.Size = new System.Drawing.Size(100, 20);
            this._lRelatedCard.TabIndex = 16;
            this._lRelatedCard.Text = "Related card";
            // 
            // _icbCardState
            // 
            this._icbCardState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._icbCardState.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._icbCardState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._icbCardState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._icbCardState.FormattingEnabled = true;
            this._icbCardState.ImageList = null;
            this._icbCardState.ItemHeight = 18;
            this._icbCardState.Localizationhelper = null;
            this._icbCardState.Location = new System.Drawing.Point(222, 324);
            this._icbCardState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._icbCardState.Name = "_icbCardState";
            this._icbCardState.SelectedItemObject = null;
            this._icbCardState.SelectedItemObjectType = null;
            this._icbCardState.Size = new System.Drawing.Size(192, 24);
            this._icbCardState.TabIndex = 11;
            this._icbCardState.TranslationPrefix = "ObjectType_";
            this._icbCardState.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _tbmCardSystem
            // 
            this._tbmCardSystem.AllowDrop = true;
            this._tbmCardSystem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCardSystem.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this._tbmCardSystem.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCardSystem.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmCardSystem.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmCardSystem.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmCardSystem.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmCardSystem.Button.Image")));
            this._tbmCardSystem.Button.Location = new System.Drawing.Point(109, 0);
            this._tbmCardSystem.Button.Name = "_bMenu";
            this._tbmCardSystem.Button.Size = new System.Drawing.Size(20, 20);
            this._tbmCardSystem.Button.TabIndex = 3;
            this._tbmCardSystem.Button.UseVisualStyleBackColor = false;
            this._tbmCardSystem.ButtonBaseColor = System.Drawing.SystemColors.Control;
            this._tbmCardSystem.ButtonDefaultBehaviour = true;
            this._tbmCardSystem.ButtonHoverColor = System.Drawing.SystemColors.GradientActiveCaption;
            this._tbmCardSystem.ButtonImage = null;
            // 
            // 
            // 
            this._tbmCardSystem.ButtonPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tsiModify,
            this._tsiRemove,
            this._tsiCreate});
            this._tbmCardSystem.ButtonPopupMenu.Name = "Create";
            this._tbmCardSystem.ButtonPopupMenu.Size = new System.Drawing.Size(149, 100);
            this._tbmCardSystem.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmCardSystem.ButtonShowImage = true;
            this._tbmCardSystem.ButtonSizeHeight = 20;
            this._tbmCardSystem.ButtonSizeWidth = 20;
            this._tbmCardSystem.ButtonText = "";
            this._tbmCardSystem.HoverTime = 500;
            // 
            // 
            // 
            this._tbmCardSystem.ImageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCardSystem.ImageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmCardSystem.ImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._tbmCardSystem.ImageTextBox.ContextMenuStrip = this._tbmCardSystem.ButtonPopupMenu;
            this._tbmCardSystem.ImageTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_tbmCardSystem.ImageTextBox.Image")));
            this._tbmCardSystem.ImageTextBox.Location = new System.Drawing.Point(0, 0);
            this._tbmCardSystem.ImageTextBox.Name = "_textBox";
            this._tbmCardSystem.ImageTextBox.NoTextNoImage = true;
            this._tbmCardSystem.ImageTextBox.ReadOnly = true;
            this._tbmCardSystem.ImageTextBox.Size = new System.Drawing.Size(109, 20);
            this._tbmCardSystem.ImageTextBox.TabIndex = 0;
            // 
            // 
            // 
            this._tbmCardSystem.ImageTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmCardSystem.ImageTextBox.TextBox.BackColor = System.Drawing.SystemColors.Info;
            this._tbmCardSystem.ImageTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbmCardSystem.ImageTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._tbmCardSystem.ImageTextBox.TextBox.Location = new System.Drawing.Point(1, 2);
            this._tbmCardSystem.ImageTextBox.TextBox.Name = "_tbTextBox";
            this._tbmCardSystem.ImageTextBox.TextBox.ReadOnly = true;
            this._tbmCardSystem.ImageTextBox.TextBox.Size = new System.Drawing.Size(107, 19);
            this._tbmCardSystem.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmCardSystem.ImageTextBox.UseImage = true;
            this._tbmCardSystem.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmCardSystem.ImageTextBox.DoubleClick += new System.EventHandler(this._eCardSystem_DoubleClick);
            this._tbmCardSystem.Location = new System.Drawing.Point(222, 12);
            this._tbmCardSystem.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbmCardSystem.MaximumSize = new System.Drawing.Size(1800, 30);
            this._tbmCardSystem.MinimumSize = new System.Drawing.Size(32, 30);
            this._tbmCardSystem.Name = "_tbmCardSystem";
            this._tbmCardSystem.Size = new System.Drawing.Size(129, 20);
            this._tbmCardSystem.TabIndex = 1;
            this._tbmCardSystem.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmCardSystem.TextImage")));
            this._tbmCardSystem.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmCardSystem_ButtonPopupMenuItemClick);
            this._tbmCardSystem.DragDrop += new System.Windows.Forms.DragEventHandler(this._eCardSystem_DragDrop);
            this._tbmCardSystem.DragOver += new System.Windows.Forms.DragEventHandler(this._eCardSystem_DragOver);
            // 
            // _tsiModify
            // 
            this._tsiModify.Name = "_tsiModify";
            this._tsiModify.Size = new System.Drawing.Size(148, 32);
            this._tsiModify.Text = "Modify";
            // 
            // _tsiRemove
            // 
            this._tsiRemove.Name = "_tsiRemove";
            this._tsiRemove.Size = new System.Drawing.Size(148, 32);
            this._tsiRemove.Text = "Remove";
            // 
            // _tsiCreate
            // 
            this._tsiCreate.Name = "_tsiCreate";
            this._tsiCreate.Size = new System.Drawing.Size(148, 32);
            this._tsiCreate.Text = "Create";
            // 
            // _tbmPerson
            // 
            this._tbmPerson.AllowDrop = true;
            this._tbmPerson.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this._tbmPerson.Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._tbmPerson.Button.BackColor = System.Drawing.SystemColors.Control;
            this._tbmPerson.Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this._tbmPerson.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._tbmPerson.Button.Image = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.Button.Image")));
            this._tbmPerson.Button.Location = new System.Drawing.Point(109, 0);
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
            this._tsiModify1,
            this._tsiRemove1});
            this._tbmPerson.ButtonPopupMenu.Name = "Create";
            this._tbmPerson.ButtonPopupMenu.Size = new System.Drawing.Size(149, 68);
            this._tbmPerson.ButtonPosition = Contal.IwQuick.UI.MenuPosition.Right;
            this._tbmPerson.ButtonShowImage = true;
            this._tbmPerson.ButtonSizeHeight = 20;
            this._tbmPerson.ButtonSizeWidth = 20;
            this._tbmPerson.ButtonText = "";
            this._tbmPerson.ForeColor = System.Drawing.SystemColors.ControlText;
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
            this._tbmPerson.ImageTextBox.Size = new System.Drawing.Size(237, 20);
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
            this._tbmPerson.ImageTextBox.TextBox.Size = new System.Drawing.Size(107, 19);
            this._tbmPerson.ImageTextBox.TextBox.TabIndex = 2;
            this._tbmPerson.ImageTextBox.UseImage = true;
            this._tbmPerson.ImageTextBox.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._tbmPerson.ImageTextBox.DoubleClick += new System.EventHandler(this._tbmPerson_DoubleClick);
            this._tbmPerson.Location = new System.Drawing.Point(222, 411);
            this._tbmPerson.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tbmPerson.MaximumSize = new System.Drawing.Size(1800, 30);
            this._tbmPerson.MinimumSize = new System.Drawing.Size(32, 30);
            this._tbmPerson.Name = "_tbmPerson";
            this._tbmPerson.Size = new System.Drawing.Size(258, 20);
            this._tbmPerson.TabIndex = 15;
            this._tbmPerson.TextImage = ((System.Drawing.Image)(resources.GetObject("_tbmPerson.TextImage")));
            this._tbmPerson.ButtonPopupMenuItemClick += new Contal.IwQuick.UI.TextBoxMenu.DPopupMenuHandler(this._tbmPerson_ButtonPopupMenuItemClick);
            this._tbmPerson.DragDrop += new System.Windows.Forms.DragEventHandler(this._tbmPerson_DragDrop);
            this._tbmPerson.DragOver += new System.Windows.Forms.DragEventHandler(this._tbmPerson_DragOver);
            // 
            // _tsiModify1
            // 
            this._tsiModify1.Name = "_tsiModify1";
            this._tsiModify1.Size = new System.Drawing.Size(148, 32);
            this._tsiModify1.Text = "Modify";
            // 
            // _tsiRemove1
            // 
            this._tsiRemove1.Name = "_tsiRemove1";
            this._tsiRemove1.Size = new System.Drawing.Size(148, 32);
            this._tsiRemove1.Text = "Remove";
            // 
            // _lActualCardType
            // 
            this._lActualCardType.AutoSize = true;
            this._lActualCardType.Location = new System.Drawing.Point(218, 70);
            this._lActualCardType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lActualCardType.Name = "_lActualCardType";
            this._lActualCardType.Size = new System.Drawing.Size(123, 20);
            this._lActualCardType.TabIndex = 3;
            this._lActualCardType.Text = "Actual card type";
            // 
            // _lCardType
            // 
            this._lCardType.AutoSize = true;
            this._lCardType.Location = new System.Drawing.Point(10, 70);
            this._lCardType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCardType.Name = "_lCardType";
            this._lCardType.Size = new System.Drawing.Size(77, 20);
            this._lCardType.TabIndex = 2;
            this._lCardType.Text = "Card type";
            // 
            // _lPerson
            // 
            this._lPerson.AutoSize = true;
            this._lPerson.Location = new System.Drawing.Point(10, 410);
            this._lPerson.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPerson.Name = "_lPerson";
            this._lPerson.Size = new System.Drawing.Size(59, 20);
            this._lPerson.TabIndex = 14;
            this._lPerson.Text = "Person";
            // 
            // _lActualFullCardNumber
            // 
            this._lActualFullCardNumber.AutoSize = true;
            this._lActualFullCardNumber.Location = new System.Drawing.Point(218, 140);
            this._lActualFullCardNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lActualFullCardNumber.Name = "_lActualFullCardNumber";
            this._lActualFullCardNumber.Size = new System.Drawing.Size(171, 20);
            this._lActualFullCardNumber.TabIndex = 7;
            this._lActualFullCardNumber.Text = "Actual full card number";
            // 
            // _lFullCardNumber
            // 
            this._lFullCardNumber.AutoSize = true;
            this._lFullCardNumber.Location = new System.Drawing.Point(10, 140);
            this._lFullCardNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lFullCardNumber.Name = "_lFullCardNumber";
            this._lFullCardNumber.Size = new System.Drawing.Size(127, 20);
            this._lFullCardNumber.TabIndex = 6;
            this._lFullCardNumber.Text = "Full card number";
            // 
            // _eDateStateLastChange
            // 
            this._eDateStateLastChange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eDateStateLastChange.Enabled = false;
            this._eDateStateLastChange.Location = new System.Drawing.Point(222, 366);
            this._eDateStateLastChange.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDateStateLastChange.Name = "_eDateStateLastChange";
            this._eDateStateLastChange.Size = new System.Drawing.Size(192, 26);
            this._eDateStateLastChange.TabIndex = 13;
            this._eDateStateLastChange.TabStop = false;
            // 
            // _lDateStateLastChange
            // 
            this._lDateStateLastChange.AutoSize = true;
            this._lDateStateLastChange.Location = new System.Drawing.Point(10, 370);
            this._lDateStateLastChange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDateStateLastChange.Name = "_lDateStateLastChange";
            this._lDateStateLastChange.Size = new System.Drawing.Size(155, 20);
            this._lDateStateLastChange.TabIndex = 12;
            this._lDateStateLastChange.Text = "Last state change at";
            // 
            // _lCardState
            // 
            this._lCardState.AutoSize = true;
            this._lCardState.Location = new System.Drawing.Point(10, 332);
            this._lCardState.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCardState.Name = "_lCardState";
            this._lCardState.Size = new System.Drawing.Size(83, 20);
            this._lCardState.TabIndex = 10;
            this._lCardState.Text = "Card state";
            // 
            // _ePin
            // 
            this._ePin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._ePin.Location = new System.Drawing.Point(222, 288);
            this._ePin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._ePin.Name = "_ePin";
            this._ePin.Size = new System.Drawing.Size(192, 26);
            this._ePin.TabIndex = 9;
            this._ePin.TextChanged += new System.EventHandler(this.EditTextChanger);
            this._ePin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._ePin_KeyPress);
            // 
            // _lPin
            // 
            this._lPin.AutoSize = true;
            this._lPin.Location = new System.Drawing.Point(10, 292);
            this._lPin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lPin.Name = "_lPin";
            this._lPin.Size = new System.Drawing.Size(31, 20);
            this._lPin.TabIndex = 8;
            this._lPin.Text = "Pin";
            // 
            // _eCardNumber
            // 
            this._eCardNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eCardNumber.Location = new System.Drawing.Point(222, 100);
            this._eCardNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eCardNumber.Name = "_eCardNumber";
            this._eCardNumber.Size = new System.Drawing.Size(192, 26);
            this._eCardNumber.TabIndex = 5;
            this._eCardNumber.TextChanged += new System.EventHandler(this.EditTextChanger);
            // 
            // _lCardNumber
            // 
            this._lCardNumber.AutoSize = true;
            this._lCardNumber.Location = new System.Drawing.Point(10, 104);
            this._lCardNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCardNumber.Name = "_lCardNumber";
            this._lCardNumber.Size = new System.Drawing.Size(101, 20);
            this._lCardNumber.TabIndex = 4;
            this._lCardNumber.Text = "Card number";
            // 
            // _lCardSystem
            // 
            this._lCardSystem.AutoSize = true;
            this._lCardSystem.Location = new System.Drawing.Point(10, 12);
            this._lCardSystem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lCardSystem.Name = "_lCardSystem";
            this._lCardSystem.Size = new System.Drawing.Size(97, 20);
            this._lCardSystem.TabIndex = 0;
            this._lCardSystem.Text = "Card system";
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.Color.Transparent;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 54);
            this._tpUserFolders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(574, 485);
            this._tpUserFolders.TabIndex = 3;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.UseVisualStyleBackColor = true;
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(453, 412);
            this._bRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(112, 34);
            this._bRefresh.TabIndex = 1;
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
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(568, 394);
            this._lbUserFolders.TabIndex = 0;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpTimetecSettings
            // 
            this._tpTimetecSettings.Controls.Add(this._lValidityDateTo);
            this._tpTimetecSettings.Controls.Add(this._lValidatyDateFrom);
            this._tpTimetecSettings.Controls.Add(this._dpValidityDateTo);
            this._tpTimetecSettings.Controls.Add(this._dpValidityDateFrom);
            this._tpTimetecSettings.Location = new System.Drawing.Point(4, 54);
            this._tpTimetecSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpTimetecSettings.Name = "_tpTimetecSettings";
            this._tpTimetecSettings.Size = new System.Drawing.Size(574, 485);
            this._tpTimetecSettings.TabIndex = 4;
            this._tpTimetecSettings.Text = "TimetecSettings";
            this._tpTimetecSettings.UseVisualStyleBackColor = true;
            // 
            // _lValidityDateTo
            // 
            this._lValidityDateTo.AutoSize = true;
            this._lValidityDateTo.Location = new System.Drawing.Point(4, 87);
            this._lValidityDateTo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lValidityDateTo.Name = "_lValidityDateTo";
            this._lValidityDateTo.Size = new System.Drawing.Size(112, 20);
            this._lValidityDateTo.TabIndex = 27;
            this._lValidityDateTo.Text = "ValidityDateTo";
            // 
            // _lValidatyDateFrom
            // 
            this._lValidatyDateFrom.AutoSize = true;
            this._lValidatyDateFrom.Location = new System.Drawing.Point(4, 26);
            this._lValidatyDateFrom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lValidatyDateFrom.Name = "_lValidatyDateFrom";
            this._lValidatyDateFrom.Size = new System.Drawing.Size(131, 20);
            this._lValidatyDateFrom.TabIndex = 26;
            this._lValidatyDateFrom.Text = "ValidityDateFrom";
            // 
            // _dpValidityDateTo
            // 
            this._dpValidityDateTo.addActualTime = false;
            this._dpValidityDateTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dpValidityDateTo.BackColor = System.Drawing.Color.Transparent;
            this._dpValidityDateTo.ButtonClearDateImage = null;
            this._dpValidityDateTo.ButtonClearDateText = "";
            this._dpValidityDateTo.ButtonClearDateWidth = 23;
            this._dpValidityDateTo.ButtonDateImage = null;
            this._dpValidityDateTo.ButtonDateText = "";
            this._dpValidityDateTo.ButtonDateWidth = 23;
            this._dpValidityDateTo.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._dpValidityDateTo.DateFormName = "Calendar";
            this._dpValidityDateTo.LocalizationHelper = null;
            this._dpValidityDateTo.Location = new System.Drawing.Point(231, 82);
            this._dpValidityDateTo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._dpValidityDateTo.MaximumSize = new System.Drawing.Size(1500, 90);
            this._dpValidityDateTo.MinimumSize = new System.Drawing.Size(180, 33);
            this._dpValidityDateTo.Name = "_dpValidityDateTo";
            this._dpValidityDateTo.ReadOnly = false;
            this._dpValidityDateTo.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.StartOfDay;
            this._dpValidityDateTo.Size = new System.Drawing.Size(321, 33);
            this._dpValidityDateTo.TabIndex = 25;
            this._dpValidityDateTo.ValidateAfter = 2D;
            this._dpValidityDateTo.ValidationEnabled = false;
            this._dpValidityDateTo.ValidationError = "";
            this._dpValidityDateTo.Value = null;
            this._dpValidityDateTo.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this.EditTextChanger);
            // 
            // _dpValidityDateFrom
            // 
            this._dpValidityDateFrom.addActualTime = false;
            this._dpValidityDateFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dpValidityDateFrom.BackColor = System.Drawing.Color.Transparent;
            this._dpValidityDateFrom.ButtonClearDateImage = null;
            this._dpValidityDateFrom.ButtonClearDateText = "";
            this._dpValidityDateFrom.ButtonClearDateWidth = 23;
            this._dpValidityDateFrom.ButtonDateImage = null;
            this._dpValidityDateFrom.ButtonDateText = "";
            this._dpValidityDateFrom.ButtonDateWidth = 23;
            this._dpValidityDateFrom.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._dpValidityDateFrom.DateFormName = "Calendar";
            this._dpValidityDateFrom.LocalizationHelper = null;
            this._dpValidityDateFrom.Location = new System.Drawing.Point(231, 20);
            this._dpValidityDateFrom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._dpValidityDateFrom.MaximumSize = new System.Drawing.Size(1500, 90);
            this._dpValidityDateFrom.MinimumSize = new System.Drawing.Size(180, 33);
            this._dpValidityDateFrom.Name = "_dpValidityDateFrom";
            this._dpValidityDateFrom.ReadOnly = false;
            this._dpValidityDateFrom.SelectTime = Contal.IwQuick.UI.SelectedTimeOfDay.StartOfDay;
            this._dpValidityDateFrom.Size = new System.Drawing.Size(321, 33);
            this._dpValidityDateFrom.TabIndex = 24;
            this._dpValidityDateFrom.ValidateAfter = 2D;
            this._dpValidityDateFrom.ValidationEnabled = false;
            this._dpValidityDateFrom.ValidationError = "";
            this._dpValidityDateFrom.Value = null;
            this._dpValidityDateFrom.TextDateChanged += new Contal.IwQuick.UI.TextBoxDatePicker.DTextChanged(this.EditTextChanger);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.Color.Transparent;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 54);
            this._tpReferencedBy.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpReferencedBy.Size = new System.Drawing.Size(574, 485);
            this._tpReferencedBy.TabIndex = 2;
            this._tpReferencedBy.Text = "Referenced by";
            this._tpReferencedBy.UseVisualStyleBackColor = true;
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.Color.Transparent;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 54);
            this._tpDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tpDescription.Size = new System.Drawing.Size(574, 485);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            this._tpDescription.UseVisualStyleBackColor = true;
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(4, 4);
            this._eDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(566, 477);
            this._eDescription.TabIndex = 0;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._tcCard);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(618, 620);
            this._panelBack.TabIndex = 0;
            // 
            // CardsEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(618, 620);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(606, 504);
            this.Name = "CardsEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CardsEditForm";
            this._tcCard.ResumeLayout(false);
            this._tpProperties.ResumeLayout(false);
            this._tpProperties.PerformLayout();
            this._tpUserFolders.ResumeLayout(false);
            this._tpTimetecSettings.ResumeLayout(false);
            this._tpTimetecSettings.PerformLayout();
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TabControl _tcCard;
        private System.Windows.Forms.TabPage _tpProperties;
        private System.Windows.Forms.Label _lActualCardType;
        private System.Windows.Forms.Label _lCardType;
        private System.Windows.Forms.Label _lPerson;
        private System.Windows.Forms.Label _lActualFullCardNumber;
        private System.Windows.Forms.Label _lFullCardNumber;
        private System.Windows.Forms.TextBox _eDateStateLastChange;
        private System.Windows.Forms.Label _lDateStateLastChange;
        private System.Windows.Forms.Label _lCardState;
        private System.Windows.Forms.TextBox _ePin;
        private System.Windows.Forms.Label _lPin;
        private System.Windows.Forms.TextBox _eCardNumber;
        private System.Windows.Forms.Label _lCardNumber;
        private System.Windows.Forms.Label _lCardSystem;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify;
        private System.Windows.Forms.ToolStripMenuItem _tsiCreate;
        private Contal.IwQuick.UI.TextBoxMenu _tbmCardSystem;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove1;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify1;
        private Contal.IwQuick.UI.ImageComboBox _icbCardState;
        private Contal.IwQuick.UI.TextBoxMenu _tbmPerson;
        private System.Windows.Forms.Label _lRelatedCard;
        private System.Windows.Forms.Label _lRelatedCardType;
        private System.Windows.Forms.Label _lRelatedCardTypeValue;
        private Contal.IwQuick.UI.TextBoxMenu _tbmRelatedCard;
        private System.Windows.Forms.ToolStripMenuItem _tsiModify2;
        private System.Windows.Forms.ToolStripMenuItem _tsiRemove2;
        private System.Windows.Forms.TextBox _eAlternateCardNumber;
        private System.Windows.Forms.Label _lAlternateCardNumber;
        private System.Windows.Forms.Button _bCreateCard;
        private System.Windows.Forms.TabPage _tpTimetecSettings;
        private Contal.IwQuick.UI.TextBoxDatePicker _dpValidityDateFrom;
        private System.Windows.Forms.Label _lValidityDateTo;
        private System.Windows.Forms.Label _lValidatyDateFrom;
        private Contal.IwQuick.UI.TextBoxDatePicker _dpValidityDateTo;
    }
}
