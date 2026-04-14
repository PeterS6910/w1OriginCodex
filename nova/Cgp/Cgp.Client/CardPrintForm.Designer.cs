namespace Contal.Cgp.Client
{
    partial class CardPrintForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardPrintForm));
            this._cbCardTemplates = new System.Windows.Forms.ComboBox();
            this._lCardTemplates = new System.Windows.Forms.Label();
            this._pbFrontLayout = new System.Windows.Forms.PictureBox();
            this._pbBackLayout = new System.Windows.Forms.PictureBox();
            this._lFrontLayout = new System.Windows.Forms.Label();
            this._lBackLayout = new System.Windows.Forms.Label();
            this._lCards = new System.Windows.Forms.Label();
            this._bCancel = new System.Windows.Forms.Button();
            this._bPrintEncode = new System.Windows.Forms.Button();
            this._bConfigure = new System.Windows.Forms.Button();
            this._bRemoveCard = new System.Windows.Forms.Button();
            this._panelBack = new System.Windows.Forms.Panel();
            this._panelBottom = new System.Windows.Forms.Panel();
            this._tcCardPrinting = new System.Windows.Forms.TabControl();
            this._tpCardSelection = new System.Windows.Forms.TabPage();
            this._scCardSelection = new System.Windows.Forms.SplitContainer();
            this._gbMifareAuthenticationKeys = new System.Windows.Forms.GroupBox();
            this._tlp = new System.Windows.Forms.TableLayoutPanel();
            this._dgvAAuthKeys = new System.Windows.Forms.DataGridView();
            this._bRemove1 = new System.Windows.Forms.Button();
            this._bAdd2 = new System.Windows.Forms.Button();
            this._bRemove2 = new System.Windows.Forms.Button();
            this._bAdd1 = new System.Windows.Forms.Button();
            this._dgvBAuthKeys = new System.Windows.Forms.DataGridView();
            this._chbRunPreview = new System.Windows.Forms.CheckBox();
            this._dgvCards = new System.Windows.Forms.DataGridView();
            this._gbModifyRows = new System.Windows.Forms.GroupBox();
            this._bApply2 = new System.Windows.Forms.Button();
            this._chbEncode = new System.Windows.Forms.CheckBox();
            this._bApply1 = new System.Windows.Forms.Button();
            this._bApply3 = new System.Windows.Forms.Button();
            this._chbPrint = new System.Windows.Forms.CheckBox();
            this._scCardPreview = new System.Windows.Forms.SplitContainer();
            this._tpPrintProgress = new System.Windows.Forms.TabPage();
            this._tlpCardResult = new System.Windows.Forms.TableLayoutPanel();
            this._dgvSucceededCards = new System.Windows.Forms.DataGridView();
            this.ColumnImage2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnCard2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSerialNumber2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPerson2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDateTime2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._dgvFailedCards = new System.Windows.Forms.DataGridView();
            this.ColumnImage3 = new System.Windows.Forms.DataGridViewImageColumn();
            this.ColumnCard3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPerson3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._lNonPrintedCards = new System.Windows.Forms.Label();
            this._lPrintedCards = new System.Windows.Forms.Label();
            this._rtbCardPrintProgress = new System.Windows.Forms.RichTextBox();
            this._lPrintProgress = new System.Windows.Forms.Label();
            this._bCSVExport = new System.Windows.Forms.Button();
            this._lSeparator = new System.Windows.Forms.Label();
            this._cbCSVSeparator = new System.Windows.Forms.ComboBox();
            this._pbOverallProgress = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this._pbFrontLayout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbBackLayout)).BeginInit();
            this._panelBack.SuspendLayout();
            this._panelBottom.SuspendLayout();
            this._tcCardPrinting.SuspendLayout();
            this._tpCardSelection.SuspendLayout();
            this._scCardSelection.Panel1.SuspendLayout();
            this._scCardSelection.Panel2.SuspendLayout();
            this._scCardSelection.SuspendLayout();
            this._gbMifareAuthenticationKeys.SuspendLayout();
            this._tlp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvAAuthKeys)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvBAuthKeys)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvCards)).BeginInit();
            this._gbModifyRows.SuspendLayout();
            this._scCardPreview.Panel1.SuspendLayout();
            this._scCardPreview.Panel2.SuspendLayout();
            this._scCardPreview.SuspendLayout();
            this._tpPrintProgress.SuspendLayout();
            this._tlpCardResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvSucceededCards)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvFailedCards)).BeginInit();
            this.SuspendLayout();
            // 
            // _cbCardTemplates
            // 
            this._cbCardTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCardTemplates.Location = new System.Drawing.Point(100, 19);
            this._cbCardTemplates.Name = "_cbCardTemplates";
            this._cbCardTemplates.Size = new System.Drawing.Size(139, 21);
            this._cbCardTemplates.TabIndex = 0;
            // 
            // _lCardTemplates
            // 
            this._lCardTemplates.AutoSize = true;
            this._lCardTemplates.Location = new System.Drawing.Point(9, 22);
            this._lCardTemplates.Name = "_lCardTemplates";
            this._lCardTemplates.Size = new System.Drawing.Size(72, 13);
            this._lCardTemplates.TabIndex = 1;
            this._lCardTemplates.Text = "Card template";
            // 
            // _pbFrontLayout
            // 
            this._pbFrontLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pbFrontLayout.Location = new System.Drawing.Point(0, 16);
            this._pbFrontLayout.Name = "_pbFrontLayout";
            this._pbFrontLayout.Size = new System.Drawing.Size(385, 165);
            this._pbFrontLayout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pbFrontLayout.TabIndex = 2;
            this._pbFrontLayout.TabStop = false;
            // 
            // _pbBackLayout
            // 
            this._pbBackLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pbBackLayout.Location = new System.Drawing.Point(0, 16);
            this._pbBackLayout.Name = "_pbBackLayout";
            this._pbBackLayout.Size = new System.Drawing.Size(393, 165);
            this._pbBackLayout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pbBackLayout.TabIndex = 2;
            this._pbBackLayout.TabStop = false;
            // 
            // _lFrontLayout
            // 
            this._lFrontLayout.AutoSize = true;
            this._lFrontLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this._lFrontLayout.Location = new System.Drawing.Point(0, 0);
            this._lFrontLayout.Name = "_lFrontLayout";
            this._lFrontLayout.Size = new System.Drawing.Size(62, 13);
            this._lFrontLayout.TabIndex = 3;
            this._lFrontLayout.Text = "Front layout";
            // 
            // _lBackLayout
            // 
            this._lBackLayout.AutoSize = true;
            this._lBackLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this._lBackLayout.Location = new System.Drawing.Point(0, 0);
            this._lBackLayout.Name = "_lBackLayout";
            this._lBackLayout.Size = new System.Drawing.Size(63, 13);
            this._lBackLayout.TabIndex = 3;
            this._lBackLayout.Text = "Back layout";
            // 
            // _lCards
            // 
            this._lCards.AutoSize = true;
            this._lCards.Location = new System.Drawing.Point(17, 9);
            this._lCards.Name = "_lCards";
            this._lCards.Size = new System.Drawing.Size(34, 13);
            this._lCards.TabIndex = 5;
            this._lCards.Text = "Cards";
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(709, 13);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 6;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bPrintEncode
            // 
            this._bPrintEncode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bPrintEncode.Location = new System.Drawing.Point(603, 13);
            this._bPrintEncode.Name = "_bPrintEncode";
            this._bPrintEncode.Size = new System.Drawing.Size(100, 23);
            this._bPrintEncode.TabIndex = 6;
            this._bPrintEncode.Text = "Print/Encode";
            this._bPrintEncode.UseVisualStyleBackColor = true;
            this._bPrintEncode.Click += new System.EventHandler(this._bPrintEncode_Click);
            // 
            // _bConfigure
            // 
            this._bConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bConfigure.Location = new System.Drawing.Point(522, 13);
            this._bConfigure.Name = "_bConfigure";
            this._bConfigure.Size = new System.Drawing.Size(75, 23);
            this._bConfigure.TabIndex = 7;
            this._bConfigure.Text = "Configure";
            this._bConfigure.UseVisualStyleBackColor = true;
            this._bConfigure.Click += new System.EventHandler(this._bConfigure_Click);
            // 
            // _bRemoveCard
            // 
            this._bRemoveCard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRemoveCard.Location = new System.Drawing.Point(695, 182);
            this._bRemoveCard.Name = "_bRemoveCard";
            this._bRemoveCard.Size = new System.Drawing.Size(75, 23);
            this._bRemoveCard.TabIndex = 8;
            this._bRemoveCard.Text = "Remove";
            this._bRemoveCard.UseVisualStyleBackColor = true;
            this._bRemoveCard.Click += new System.EventHandler(this._bRemoveCard_Click);
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._panelBottom);
            this._panelBack.Controls.Add(this._tcCardPrinting);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(796, 684);
            this._panelBack.TabIndex = 9;
            // 
            // _panelBottom
            // 
            this._panelBottom.Controls.Add(this._bConfigure);
            this._panelBottom.Controls.Add(this._bCancel);
            this._panelBottom.Controls.Add(this._bPrintEncode);
            this._panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panelBottom.Location = new System.Drawing.Point(0, 636);
            this._panelBottom.Name = "_panelBottom";
            this._panelBottom.Size = new System.Drawing.Size(796, 48);
            this._panelBottom.TabIndex = 10;
            // 
            // _tcCardPrinting
            // 
            this._tcCardPrinting.Controls.Add(this._tpCardSelection);
            this._tcCardPrinting.Controls.Add(this._tpPrintProgress);
            this._tcCardPrinting.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tcCardPrinting.Location = new System.Drawing.Point(0, 0);
            this._tcCardPrinting.Name = "_tcCardPrinting";
            this._tcCardPrinting.SelectedIndex = 0;
            this._tcCardPrinting.Size = new System.Drawing.Size(796, 684);
            this._tcCardPrinting.TabIndex = 9;
            // 
            // _tpCardSelection
            // 
            this._tpCardSelection.BackColor = System.Drawing.SystemColors.Control;
            this._tpCardSelection.Controls.Add(this._scCardSelection);
            this._tpCardSelection.Location = new System.Drawing.Point(4, 22);
            this._tpCardSelection.Name = "_tpCardSelection";
            this._tpCardSelection.Padding = new System.Windows.Forms.Padding(3);
            this._tpCardSelection.Size = new System.Drawing.Size(788, 658);
            this._tpCardSelection.TabIndex = 0;
            this._tpCardSelection.Text = "Card selection";
            // 
            // _scCardSelection
            // 
            this._scCardSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._scCardSelection.Location = new System.Drawing.Point(3, 3);
            this._scCardSelection.Name = "_scCardSelection";
            this._scCardSelection.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _scCardSelection.Panel1
            // 
            this._scCardSelection.Panel1.Controls.Add(this._lCards);
            this._scCardSelection.Panel1.Controls.Add(this._gbMifareAuthenticationKeys);
            this._scCardSelection.Panel1.Controls.Add(this._bRemoveCard);
            this._scCardSelection.Panel1.Controls.Add(this._chbRunPreview);
            this._scCardSelection.Panel1.Controls.Add(this._dgvCards);
            this._scCardSelection.Panel1.Controls.Add(this._gbModifyRows);
            // 
            // _scCardSelection.Panel2
            // 
            this._scCardSelection.Panel2.Controls.Add(this._scCardPreview);
            this._scCardSelection.Size = new System.Drawing.Size(782, 605);
            this._scCardSelection.SplitterDistance = 417;
            this._scCardSelection.TabIndex = 17;
            this._scCardSelection.Paint += new System.Windows.Forms.PaintEventHandler(this.SplitContainerPaint);
            // 
            // _gbMifareAuthenticationKeys
            // 
            this._gbMifareAuthenticationKeys.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._gbMifareAuthenticationKeys.Controls.Add(this._tlp);
            this._gbMifareAuthenticationKeys.Location = new System.Drawing.Point(351, 211);
            this._gbMifareAuthenticationKeys.Name = "_gbMifareAuthenticationKeys";
            this._gbMifareAuthenticationKeys.Size = new System.Drawing.Size(422, 175);
            this._gbMifareAuthenticationKeys.TabIndex = 16;
            this._gbMifareAuthenticationKeys.TabStop = false;
            this._gbMifareAuthenticationKeys.Text = "A/B keys for encoding already encoded cards";
            // 
            // _tlp
            // 
            this._tlp.ColumnCount = 4;
            this._tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlp.Controls.Add(this._dgvAAuthKeys, 0, 0);
            this._tlp.Controls.Add(this._bRemove1, 1, 1);
            this._tlp.Controls.Add(this._bAdd2, 2, 1);
            this._tlp.Controls.Add(this._bRemove2, 3, 1);
            this._tlp.Controls.Add(this._bAdd1, 0, 1);
            this._tlp.Controls.Add(this._dgvBAuthKeys, 2, 0);
            this._tlp.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tlp.Location = new System.Drawing.Point(3, 16);
            this._tlp.Name = "_tlp";
            this._tlp.RowCount = 2;
            this._tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tlp.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tlp.Size = new System.Drawing.Size(416, 156);
            this._tlp.TabIndex = 0;
            // 
            // _dgvAAuthKeys
            // 
            this._dgvAAuthKeys.AllowUserToResizeColumns = false;
            this._dgvAAuthKeys.AllowUserToResizeRows = false;
            this._dgvAAuthKeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._tlp.SetColumnSpan(this._dgvAAuthKeys, 2);
            this._dgvAAuthKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvAAuthKeys.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgvAAuthKeys.Location = new System.Drawing.Point(3, 3);
            this._dgvAAuthKeys.Name = "_dgvAAuthKeys";
            this._dgvAAuthKeys.Size = new System.Drawing.Size(202, 121);
            this._dgvAAuthKeys.TabIndex = 0;
            this._dgvAAuthKeys.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this._dgvAAuthKeys_EditingControlShowing);
            // 
            // _bRemove1
            // 
            this._bRemove1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRemove1.Location = new System.Drawing.Point(82, 130);
            this._bRemove1.Name = "_bRemove1";
            this._bRemove1.Size = new System.Drawing.Size(73, 23);
            this._bRemove1.TabIndex = 3;
            this._bRemove1.Text = "Remove";
            this._bRemove1.UseVisualStyleBackColor = true;
            this._bRemove1.Click += new System.EventHandler(this._bRemoveAKey_Click);
            // 
            // _bAdd2
            // 
            this._bAdd2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bAdd2.Location = new System.Drawing.Point(211, 130);
            this._bAdd2.Name = "_bAdd2";
            this._bAdd2.Size = new System.Drawing.Size(72, 23);
            this._bAdd2.TabIndex = 3;
            this._bAdd2.Text = "Add";
            this._bAdd2.UseVisualStyleBackColor = true;
            this._bAdd2.Click += new System.EventHandler(this._bAddBKey_Click);
            // 
            // _bRemove2
            // 
            this._bRemove2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bRemove2.Location = new System.Drawing.Point(289, 130);
            this._bRemove2.Name = "_bRemove2";
            this._bRemove2.Size = new System.Drawing.Size(72, 23);
            this._bRemove2.TabIndex = 3;
            this._bRemove2.Text = "Remove";
            this._bRemove2.UseVisualStyleBackColor = true;
            this._bRemove2.Click += new System.EventHandler(this._bRemoveBKey_Click);
            // 
            // _bAdd1
            // 
            this._bAdd1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bAdd1.Location = new System.Drawing.Point(3, 130);
            this._bAdd1.Name = "_bAdd1";
            this._bAdd1.Size = new System.Drawing.Size(73, 23);
            this._bAdd1.TabIndex = 2;
            this._bAdd1.Text = "Add";
            this._bAdd1.UseVisualStyleBackColor = true;
            this._bAdd1.Click += new System.EventHandler(this._bAddAKey_Click);
            // 
            // _dgvBAuthKeys
            // 
            this._dgvBAuthKeys.AllowUserToResizeColumns = false;
            this._dgvBAuthKeys.AllowUserToResizeRows = false;
            this._dgvBAuthKeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._tlp.SetColumnSpan(this._dgvBAuthKeys, 2);
            this._dgvBAuthKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvBAuthKeys.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgvBAuthKeys.Location = new System.Drawing.Point(211, 3);
            this._dgvBAuthKeys.Name = "_dgvBAuthKeys";
            this._dgvBAuthKeys.Size = new System.Drawing.Size(202, 121);
            this._dgvBAuthKeys.TabIndex = 1;
            this._dgvBAuthKeys.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this._dgvAAuthKeys_EditingControlShowing);
            // 
            // _chbRunPreview
            // 
            this._chbRunPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._chbRunPreview.AutoSize = true;
            this._chbRunPreview.Location = new System.Drawing.Point(14, 183);
            this._chbRunPreview.Name = "_chbRunPreview";
            this._chbRunPreview.Size = new System.Drawing.Size(156, 17);
            this._chbRunPreview.TabIndex = 12;
            this._chbRunPreview.Text = "Run preview on card select";
            this._chbRunPreview.UseVisualStyleBackColor = true;
            this._chbRunPreview.CheckedChanged += new System.EventHandler(this._chbRunPreview_CheckedChanged);
            // 
            // _dgvCards
            // 
            this._dgvCards.AllowDrop = true;
            this._dgvCards.AllowUserToAddRows = false;
            this._dgvCards.AllowUserToDeleteRows = false;
            this._dgvCards.AllowUserToResizeRows = false;
            this._dgvCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvCards.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvCards.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgvCards.Location = new System.Drawing.Point(14, 27);
            this._dgvCards.Name = "_dgvCards";
            this._dgvCards.RowHeadersVisible = false;
            this._dgvCards.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvCards.Size = new System.Drawing.Size(756, 149);
            this._dgvCards.TabIndex = 11;
            this._dgvCards.DragOver += new System.Windows.Forms.DragEventHandler(this._dgvCards_DragOver);
            this._dgvCards.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgvCards_CellEndEdit);
            this._dgvCards.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this._dgvCards_DataError);
            this._dgvCards.DragDrop += new System.Windows.Forms.DragEventHandler(this._dgvCards_DragDrop);
            // 
            // _gbModifyRows
            // 
            this._gbModifyRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._gbModifyRows.Controls.Add(this._bApply2);
            this._gbModifyRows.Controls.Add(this._chbEncode);
            this._gbModifyRows.Controls.Add(this._lCardTemplates);
            this._gbModifyRows.Controls.Add(this._cbCardTemplates);
            this._gbModifyRows.Controls.Add(this._bApply1);
            this._gbModifyRows.Controls.Add(this._bApply3);
            this._gbModifyRows.Controls.Add(this._chbPrint);
            this._gbModifyRows.Location = new System.Drawing.Point(14, 211);
            this._gbModifyRows.Name = "_gbModifyRows";
            this._gbModifyRows.Size = new System.Drawing.Size(329, 107);
            this._gbModifyRows.TabIndex = 15;
            this._gbModifyRows.TabStop = false;
            this._gbModifyRows.Text = "Modify selected rows";
            // 
            // _bApply2
            // 
            this._bApply2.Location = new System.Drawing.Point(100, 47);
            this._bApply2.Name = "_bApply2";
            this._bApply2.Size = new System.Drawing.Size(75, 23);
            this._bApply2.TabIndex = 14;
            this._bApply2.Text = "Apply";
            this._bApply2.UseVisualStyleBackColor = true;
            this._bApply2.Click += new System.EventHandler(this._bApplyPrint_Click);
            // 
            // _chbEncode
            // 
            this._chbEncode.AutoSize = true;
            this._chbEncode.Checked = true;
            this._chbEncode.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chbEncode.Location = new System.Drawing.Point(12, 80);
            this._chbEncode.Name = "_chbEncode";
            this._chbEncode.Size = new System.Drawing.Size(63, 17);
            this._chbEncode.TabIndex = 10;
            this._chbEncode.Text = "Encode";
            this._chbEncode.UseVisualStyleBackColor = true;
            // 
            // _bApply1
            // 
            this._bApply1.Location = new System.Drawing.Point(245, 18);
            this._bApply1.Name = "_bApply1";
            this._bApply1.Size = new System.Drawing.Size(75, 23);
            this._bApply1.TabIndex = 14;
            this._bApply1.Text = "Apply";
            this._bApply1.UseVisualStyleBackColor = true;
            this._bApply1.Click += new System.EventHandler(this._bApplyTemplate_Click);
            // 
            // _bApply3
            // 
            this._bApply3.Location = new System.Drawing.Point(100, 76);
            this._bApply3.Name = "_bApply3";
            this._bApply3.Size = new System.Drawing.Size(75, 23);
            this._bApply3.TabIndex = 14;
            this._bApply3.Text = "Apply";
            this._bApply3.UseVisualStyleBackColor = true;
            this._bApply3.Click += new System.EventHandler(this._bApplyEncode_Click);
            // 
            // _chbPrint
            // 
            this._chbPrint.AutoSize = true;
            this._chbPrint.Checked = true;
            this._chbPrint.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chbPrint.Location = new System.Drawing.Point(12, 51);
            this._chbPrint.Name = "_chbPrint";
            this._chbPrint.Size = new System.Drawing.Size(47, 17);
            this._chbPrint.TabIndex = 10;
            this._chbPrint.Text = "Print";
            this._chbPrint.UseVisualStyleBackColor = true;
            // 
            // _scCardPreview
            // 
            this._scCardPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._scCardPreview.BackColor = System.Drawing.SystemColors.Control;
            this._scCardPreview.Location = new System.Drawing.Point(0, 0);
            this._scCardPreview.Name = "_scCardPreview";
            // 
            // _scCardPreview.Panel1
            // 
            this._scCardPreview.Panel1.Controls.Add(this._lFrontLayout);
            this._scCardPreview.Panel1.Controls.Add(this._pbFrontLayout);
            // 
            // _scCardPreview.Panel2
            // 
            this._scCardPreview.Panel2.Controls.Add(this._lBackLayout);
            this._scCardPreview.Panel2.Controls.Add(this._pbBackLayout);
            this._scCardPreview.Size = new System.Drawing.Size(782, 181);
            this._scCardPreview.SplitterDistance = 385;
            this._scCardPreview.TabIndex = 9;
            this._scCardPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.SplitContainerPaint);
            // 
            // _tpPrintProgress
            // 
            this._tpPrintProgress.BackColor = System.Drawing.SystemColors.Control;
            this._tpPrintProgress.Controls.Add(this._tlpCardResult);
            this._tpPrintProgress.Controls.Add(this._pbOverallProgress);
            this._tpPrintProgress.Location = new System.Drawing.Point(4, 22);
            this._tpPrintProgress.Name = "_tpPrintProgress";
            this._tpPrintProgress.Padding = new System.Windows.Forms.Padding(3);
            this._tpPrintProgress.Size = new System.Drawing.Size(788, 658);
            this._tpPrintProgress.TabIndex = 1;
            this._tpPrintProgress.Text = "Printing";
            // 
            // _tlpCardResult
            // 
            this._tlpCardResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tlpCardResult.ColumnCount = 4;
            this._tlpCardResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 103F));
            this._tlpCardResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this._tlpCardResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlpCardResult.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlpCardResult.Controls.Add(this._dgvSucceededCards, 0, 3);
            this._tlpCardResult.Controls.Add(this._dgvFailedCards, 3, 3);
            this._tlpCardResult.Controls.Add(this._lNonPrintedCards, 3, 2);
            this._tlpCardResult.Controls.Add(this._lPrintedCards, 0, 2);
            this._tlpCardResult.Controls.Add(this._rtbCardPrintProgress, 0, 1);
            this._tlpCardResult.Controls.Add(this._lPrintProgress, 0, 0);
            this._tlpCardResult.Controls.Add(this._bCSVExport, 0, 4);
            this._tlpCardResult.Controls.Add(this._lSeparator, 1, 4);
            this._tlpCardResult.Controls.Add(this._cbCSVSeparator, 2, 4);
            this._tlpCardResult.Location = new System.Drawing.Point(3, 3);
            this._tlpCardResult.Name = "_tlpCardResult";
            this._tlpCardResult.RowCount = 5;
            this._tlpCardResult.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tlpCardResult.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlpCardResult.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._tlpCardResult.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlpCardResult.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._tlpCardResult.Size = new System.Drawing.Size(777, 580);
            this._tlpCardResult.TabIndex = 12;
            // 
            // _dgvSucceededCards
            // 
            this._dgvSucceededCards.AllowDrop = true;
            this._dgvSucceededCards.AllowUserToAddRows = false;
            this._dgvSucceededCards.AllowUserToDeleteRows = false;
            this._dgvSucceededCards.AllowUserToResizeRows = false;
            this._dgvSucceededCards.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvSucceededCards.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnImage2,
            this.ColumnCard2,
            this.ColumnSerialNumber2,
            this.ColumnPerson2,
            this.ColumnDateTime2});
            this._tlpCardResult.SetColumnSpan(this._dgvSucceededCards, 3);
            this._dgvSucceededCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvSucceededCards.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dgvSucceededCards.Location = new System.Drawing.Point(3, 298);
            this._dgvSucceededCards.Name = "_dgvSucceededCards";
            this._dgvSucceededCards.ReadOnly = true;
            this._dgvSucceededCards.RowHeadersVisible = false;
            this._dgvSucceededCards.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvSucceededCards.Size = new System.Drawing.Size(477, 249);
            this._dgvSucceededCards.TabIndex = 11;
            this._dgvSucceededCards.DragOver += new System.Windows.Forms.DragEventHandler(this._dgvCards_DragOver);
            this._dgvSucceededCards.DragDrop += new System.Windows.Forms.DragEventHandler(this._dgvCards_DragDrop);
            // 
            // ColumnImage2
            // 
            this.ColumnImage2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ColumnImage2.HeaderText = "Image";
            this.ColumnImage2.Name = "ColumnImage2";
            this.ColumnImage2.ReadOnly = true;
            this.ColumnImage2.Width = 42;
            // 
            // ColumnCard2
            // 
            this.ColumnCard2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnCard2.HeaderText = "Card";
            this.ColumnCard2.Name = "ColumnCard2";
            this.ColumnCard2.ReadOnly = true;
            this.ColumnCard2.Width = 54;
            // 
            // ColumnSerialNumber2
            // 
            this.ColumnSerialNumber2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnSerialNumber2.HeaderText = "Serial number";
            this.ColumnSerialNumber2.Name = "ColumnSerialNumber2";
            this.ColumnSerialNumber2.ReadOnly = true;
            this.ColumnSerialNumber2.Width = 96;
            // 
            // ColumnPerson2
            // 
            this.ColumnPerson2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnPerson2.HeaderText = "Person";
            this.ColumnPerson2.Name = "ColumnPerson2";
            this.ColumnPerson2.ReadOnly = true;
            this.ColumnPerson2.Width = 65;
            // 
            // ColumnDateTime2
            // 
            this.ColumnDateTime2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnDateTime2.HeaderText = "Date and time";
            this.ColumnDateTime2.MinimumWidth = 40;
            this.ColumnDateTime2.Name = "ColumnDateTime2";
            this.ColumnDateTime2.ReadOnly = true;
            // 
            // _dgvFailedCards
            // 
            this._dgvFailedCards.AllowDrop = true;
            this._dgvFailedCards.AllowUserToAddRows = false;
            this._dgvFailedCards.AllowUserToDeleteRows = false;
            this._dgvFailedCards.AllowUserToResizeRows = false;
            this._dgvFailedCards.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvFailedCards.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnImage3,
            this.ColumnCard3,
            this.ColumnPerson3});
            this._dgvFailedCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvFailedCards.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._dgvFailedCards.Location = new System.Drawing.Point(486, 298);
            this._dgvFailedCards.Name = "_dgvFailedCards";
            this._dgvFailedCards.ReadOnly = true;
            this._dgvFailedCards.RowHeadersVisible = false;
            this._tlpCardResult.SetRowSpan(this._dgvFailedCards, 2);
            this._dgvFailedCards.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvFailedCards.Size = new System.Drawing.Size(288, 279);
            this._dgvFailedCards.TabIndex = 11;
            this._dgvFailedCards.DragOver += new System.Windows.Forms.DragEventHandler(this._dgvCards_DragOver);
            this._dgvFailedCards.DragDrop += new System.Windows.Forms.DragEventHandler(this._dgvCards_DragDrop);
            // 
            // ColumnImage3
            // 
            this.ColumnImage3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ColumnImage3.HeaderText = "Image";
            this.ColumnImage3.Name = "ColumnImage3";
            this.ColumnImage3.ReadOnly = true;
            this.ColumnImage3.Width = 42;
            // 
            // ColumnCard3
            // 
            this.ColumnCard3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnCard3.HeaderText = "Card";
            this.ColumnCard3.Name = "ColumnCard3";
            this.ColumnCard3.ReadOnly = true;
            this.ColumnCard3.Width = 54;
            // 
            // ColumnPerson3
            // 
            this.ColumnPerson3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnPerson3.HeaderText = "Person";
            this.ColumnPerson3.Name = "ColumnPerson3";
            this.ColumnPerson3.ReadOnly = true;
            // 
            // _lNonPrintedCards
            // 
            this._lNonPrintedCards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lNonPrintedCards.AutoSize = true;
            this._lNonPrintedCards.Location = new System.Drawing.Point(486, 282);
            this._lNonPrintedCards.Name = "_lNonPrintedCards";
            this._lNonPrintedCards.Size = new System.Drawing.Size(35, 13);
            this._lNonPrintedCards.TabIndex = 1;
            this._lNonPrintedCards.Text = "Failed";
            // 
            // _lPrintedCards
            // 
            this._lPrintedCards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lPrintedCards.AutoSize = true;
            this._lPrintedCards.Location = new System.Drawing.Point(3, 282);
            this._lPrintedCards.Name = "_lPrintedCards";
            this._lPrintedCards.Size = new System.Drawing.Size(62, 13);
            this._lPrintedCards.TabIndex = 1;
            this._lPrintedCards.Text = "Succeeded";
            // 
            // _rtbCardPrintProgress
            // 
            this._tlpCardResult.SetColumnSpan(this._rtbCardPrintProgress, 4);
            this._rtbCardPrintProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rtbCardPrintProgress.Location = new System.Drawing.Point(3, 23);
            this._rtbCardPrintProgress.Name = "_rtbCardPrintProgress";
            this._rtbCardPrintProgress.ReadOnly = true;
            this._rtbCardPrintProgress.Size = new System.Drawing.Size(771, 249);
            this._rtbCardPrintProgress.TabIndex = 3;
            this._rtbCardPrintProgress.Text = global::Contal.Cgp.Client.Localization_Swedish.AlarmStates_NotSelected;
            this._rtbCardPrintProgress.WordWrap = false;
            // 
            // _lPrintProgress
            // 
            this._lPrintProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lPrintProgress.AutoSize = true;
            this._tlpCardResult.SetColumnSpan(this._lPrintProgress, 4);
            this._lPrintProgress.Location = new System.Drawing.Point(3, 7);
            this._lPrintProgress.Name = "_lPrintProgress";
            this._lPrintProgress.Size = new System.Drawing.Size(112, 13);
            this._lPrintProgress.TabIndex = 4;
            this._lPrintProgress.Text = "Print/encode progress";
            // 
            // _bCSVExport
            // 
            this._bCSVExport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._bCSVExport.AutoSize = true;
            this._bCSVExport.Location = new System.Drawing.Point(9, 553);
            this._bCSVExport.Name = "_bCSVExport";
            this._bCSVExport.Size = new System.Drawing.Size(85, 23);
            this._bCSVExport.TabIndex = 12;
            this._bCSVExport.Text = "Export to CSV";
            this._bCSVExport.UseVisualStyleBackColor = true;
            this._bCSVExport.Click += new System.EventHandler(this._bCSVExport_Click);
            // 
            // _lSeparator
            // 
            this._lSeparator.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._lSeparator.AutoSize = true;
            this._lSeparator.Location = new System.Drawing.Point(120, 558);
            this._lSeparator.Name = "_lSeparator";
            this._lSeparator.Size = new System.Drawing.Size(53, 13);
            this._lSeparator.TabIndex = 13;
            this._lSeparator.Text = "Separator";
            // 
            // _cbCSVSeparator
            // 
            this._cbCSVSeparator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._cbCSVSeparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCSVSeparator.FormattingEnabled = true;
            this._cbCSVSeparator.Items.AddRange(new object[] {
            ":",
            ";",
            ",",
            "TAB"});
            this._cbCSVSeparator.Location = new System.Drawing.Point(193, 554);
            this._cbCSVSeparator.Name = "_cbCSVSeparator";
            this._cbCSVSeparator.Size = new System.Drawing.Size(121, 21);
            this._cbCSVSeparator.TabIndex = 14;
            // 
            // _pbOverallProgress
            // 
            this._pbOverallProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pbOverallProgress.Location = new System.Drawing.Point(6, 589);
            this._pbOverallProgress.Name = "_pbOverallProgress";
            this._pbOverallProgress.Size = new System.Drawing.Size(774, 19);
            this._pbOverallProgress.TabIndex = 2;
            // 
            // CardPrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 684);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 700);
            this.Name = "CardPrintForm";
            this.Text = "CardPrintForm";
            this.Load += new System.EventHandler(this.CardPrintForm_Load);
            this.Shown += new System.EventHandler(this.CardPrintForm_Shown);
            this.Enter += new System.EventHandler(this.CardPrintForm_Enter);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CardPrintForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this._pbFrontLayout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbBackLayout)).EndInit();
            this._panelBack.ResumeLayout(false);
            this._panelBottom.ResumeLayout(false);
            this._tcCardPrinting.ResumeLayout(false);
            this._tpCardSelection.ResumeLayout(false);
            this._scCardSelection.Panel1.ResumeLayout(false);
            this._scCardSelection.Panel1.PerformLayout();
            this._scCardSelection.Panel2.ResumeLayout(false);
            this._scCardSelection.ResumeLayout(false);
            this._gbMifareAuthenticationKeys.ResumeLayout(false);
            this._tlp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvAAuthKeys)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvBAuthKeys)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvCards)).EndInit();
            this._gbModifyRows.ResumeLayout(false);
            this._gbModifyRows.PerformLayout();
            this._scCardPreview.Panel1.ResumeLayout(false);
            this._scCardPreview.Panel1.PerformLayout();
            this._scCardPreview.Panel2.ResumeLayout(false);
            this._scCardPreview.Panel2.PerformLayout();
            this._scCardPreview.ResumeLayout(false);
            this._tpPrintProgress.ResumeLayout(false);
            this._tlpCardResult.ResumeLayout(false);
            this._tlpCardResult.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvSucceededCards)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvFailedCards)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox _cbCardTemplates;
        private System.Windows.Forms.Label _lCardTemplates;
        private System.Windows.Forms.PictureBox _pbFrontLayout;
        private System.Windows.Forms.PictureBox _pbBackLayout;
        private System.Windows.Forms.Label _lFrontLayout;
        private System.Windows.Forms.Label _lBackLayout;
        private System.Windows.Forms.Label _lCards;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bPrintEncode;
        private System.Windows.Forms.Button _bConfigure;
        private System.Windows.Forms.Button _bRemoveCard;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.TabControl _tcCardPrinting;
        private System.Windows.Forms.TabPage _tpCardSelection;
        private System.Windows.Forms.TabPage _tpPrintProgress;
        private System.Windows.Forms.Panel _panelBottom;
        private System.Windows.Forms.Label _lNonPrintedCards;
        private System.Windows.Forms.Label _lPrintedCards;
        private System.Windows.Forms.ProgressBar _pbOverallProgress;
        private System.Windows.Forms.SplitContainer _scCardPreview;
        private System.Windows.Forms.Label _lPrintProgress;
        private System.Windows.Forms.RichTextBox _rtbCardPrintProgress;
        private System.Windows.Forms.CheckBox _chbEncode;
        private System.Windows.Forms.CheckBox _chbPrint;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFullCardNumber;
        private System.Windows.Forms.DataGridView _dgvCards;
        private System.Windows.Forms.DataGridView _dgvFailedCards;
        private System.Windows.Forms.DataGridView _dgvSucceededCards;
        private System.Windows.Forms.TableLayoutPanel _tlpCardResult;
        private System.Windows.Forms.Button _bCSVExport;
        private System.Windows.Forms.Label _lSeparator;
        private System.Windows.Forms.ComboBox _cbCSVSeparator;
        private System.Windows.Forms.DataGridViewImageColumn ColumnImage3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCard3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPerson3;
        private System.Windows.Forms.DataGridViewImageColumn ColumnImage2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCard2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSerialNumber2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPerson2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDateTime2;
        private System.Windows.Forms.CheckBox _chbRunPreview;
        private System.Windows.Forms.GroupBox _gbModifyRows;
        private System.Windows.Forms.Button _bApply2;
        private System.Windows.Forms.Button _bApply1;
        private System.Windows.Forms.Button _bApply3;
        private System.Windows.Forms.GroupBox _gbMifareAuthenticationKeys;
        private System.Windows.Forms.TableLayoutPanel _tlp;
        private System.Windows.Forms.DataGridView _dgvAAuthKeys;
        private System.Windows.Forms.DataGridView _dgvBAuthKeys;
        private System.Windows.Forms.Button _bAdd1;
        private System.Windows.Forms.Button _bRemove1;
        private System.Windows.Forms.Button _bAdd2;
        private System.Windows.Forms.Button _bRemove2;
        private System.Windows.Forms.SplitContainer _scCardSelection;
    }
}