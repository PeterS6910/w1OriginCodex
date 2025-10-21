namespace Contal.Cgp.Client
{
    partial class AlarmDetailsForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmDetailsForm));
            this._lCreatedDateTime = new System.Windows.Forms.Label();
            this._eCreatedDateTime = new System.Windows.Forms.TextBox();
            this._lParentObject = new System.Windows.Forms.Label();
            this._lRelativeUniqueIdentificate = new System.Windows.Forms.Label();
            this._eRelativeUniqueIdentificate = new System.Windows.Forms.TextBox();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bAcknowledgeState = new System.Windows.Forms.Button();
            this._dgvParameters = new System.Windows.Forms.DataGridView();
            this._bUnblock = new System.Windows.Forms.Button();
            this._bBlock = new System.Windows.Forms.Button();
            this._lAlarmState = new System.Windows.Forms.Label();
            this._eAlarmState = new System.Windows.Forms.TextBox();
            this._ilbParentObject = new Contal.IwQuick.UI.ImageListBox();
            this._tcAlarmDetails = new System.Windows.Forms.TabControl();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._tpParameters = new System.Windows.Forms.TabPage();
            this._tpAlarmInstructions = new System.Windows.Forms.TabPage();
            this._bEdit = new System.Windows.Forms.Button();
            this._dgAlarmInstructions = new System.Windows.Forms.DataGridView();
            this._bAcknowledgeAndBlock = new System.Windows.Forms.Button();
            this._lAcknowledgement = new System.Windows.Forms.Label();
            this._pbAcknowledgement = new System.Windows.Forms.PictureBox();
            this._pbIndividualBlockingState = new System.Windows.Forms.PictureBox();
            this._pbGeneralBlockingState = new System.Windows.Forms.PictureBox();
            this._lIndividualBlockingState = new System.Windows.Forms.Label();
            this._lGeneralBlockingState = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dgvParameters)).BeginInit();
            this._tcAlarmDetails.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._tpParameters.SuspendLayout();
            this._tpAlarmInstructions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgAlarmInstructions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbAcknowledgement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbIndividualBlockingState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbGeneralBlockingState)).BeginInit();
            this.SuspendLayout();
            // 
            // _lCreatedDateTime
            // 
            this._lCreatedDateTime.AutoSize = true;
            this._lCreatedDateTime.Location = new System.Drawing.Point(12, 9);
            this._lCreatedDateTime.Name = "_lCreatedDateTime";
            this._lCreatedDateTime.Size = new System.Drawing.Size(90, 13);
            this._lCreatedDateTime.TabIndex = 64;
            this._lCreatedDateTime.Text = "Created date time";
            // 
            // _eCreatedDateTime
            // 
            this._eCreatedDateTime.BackColor = System.Drawing.SystemColors.Window;
            this._eCreatedDateTime.Location = new System.Drawing.Point(181, 2);
            this._eCreatedDateTime.Name = "_eCreatedDateTime";
            this._eCreatedDateTime.ReadOnly = true;
            this._eCreatedDateTime.Size = new System.Drawing.Size(245, 20);
            this._eCreatedDateTime.TabIndex = 0;
            // 
            // _lParentObject
            // 
            this._lParentObject.AutoSize = true;
            this._lParentObject.Location = new System.Drawing.Point(12, 35);
            this._lParentObject.Name = "_lParentObject";
            this._lParentObject.Size = new System.Drawing.Size(70, 13);
            this._lParentObject.TabIndex = 66;
            this._lParentObject.Text = "Parent object";
            // 
            // _lRelativeUniqueIdentificate
            // 
            this._lRelativeUniqueIdentificate.AutoSize = true;
            this._lRelativeUniqueIdentificate.Location = new System.Drawing.Point(12, 105);
            this._lRelativeUniqueIdentificate.Name = "_lRelativeUniqueIdentificate";
            this._lRelativeUniqueIdentificate.Size = new System.Drawing.Size(135, 13);
            this._lRelativeUniqueIdentificate.TabIndex = 68;
            this._lRelativeUniqueIdentificate.Text = "Relative unique identificate";
            // 
            // _eRelativeUniqueIdentificate
            // 
            this._eRelativeUniqueIdentificate.BackColor = System.Drawing.SystemColors.Window;
            this._eRelativeUniqueIdentificate.Location = new System.Drawing.Point(180, 102);
            this._eRelativeUniqueIdentificate.Name = "_eRelativeUniqueIdentificate";
            this._eRelativeUniqueIdentificate.ReadOnly = true;
            this._eRelativeUniqueIdentificate.Size = new System.Drawing.Size(246, 20);
            this._eRelativeUniqueIdentificate.TabIndex = 2;
            // 
            // _eDescription
            // 
            this._eDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDescription.BackColor = System.Drawing.SystemColors.Window;
            this._eDescription.Location = new System.Drawing.Point(3, 3);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ReadOnly = true;
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(400, 190);
            this._eDescription.TabIndex = 5;
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(346, 460);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(80, 22);
            this._bCancel.TabIndex = 8;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bAcknowledgeState
            // 
            this._bAcknowledgeState.Location = new System.Drawing.Point(160, 460);
            this._bAcknowledgeState.Name = "_bAcknowledgeState";
            this._bAcknowledgeState.Size = new System.Drawing.Size(94, 22);
            this._bAcknowledgeState.TabIndex = 4;
            this._bAcknowledgeState.Text = "Acknowledge state";
            this._bAcknowledgeState.UseVisualStyleBackColor = true;
            this._bAcknowledgeState.Click += new System.EventHandler(this._bConfirmState_Click);
            // 
            // _dgvParameters
            // 
            this._dgvParameters.AllowUserToAddRows = false;
            this._dgvParameters.AllowUserToDeleteRows = false;
            this._dgvParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvParameters.Location = new System.Drawing.Point(3, 3);
            this._dgvParameters.MultiSelect = false;
            this._dgvParameters.Name = "_dgvParameters";
            this._dgvParameters.ReadOnly = true;
            this._dgvParameters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgvParameters.Size = new System.Drawing.Size(400, 190);
            this._dgvParameters.TabIndex = 76;
            // 
            // _bUnblock
            // 
            this._bUnblock.Location = new System.Drawing.Point(260, 460);
            this._bUnblock.Name = "_bUnblock";
            this._bUnblock.Size = new System.Drawing.Size(80, 22);
            this._bUnblock.TabIndex = 7;
            this._bUnblock.Text = "Unblock";
            this._bUnblock.UseVisualStyleBackColor = true;
            this._bUnblock.Click += new System.EventHandler(this._bUnblock_Click);
            // 
            // _bBlock
            // 
            this._bBlock.Location = new System.Drawing.Point(260, 460);
            this._bBlock.Name = "_bBlock";
            this._bBlock.Size = new System.Drawing.Size(80, 22);
            this._bBlock.TabIndex = 6;
            this._bBlock.Text = "Block";
            this._bBlock.UseVisualStyleBackColor = true;
            this._bBlock.Click += new System.EventHandler(this._bBlock_Click);
            // 
            // _lAlarmState
            // 
            this._lAlarmState.AutoSize = true;
            this._lAlarmState.Location = new System.Drawing.Point(12, 131);
            this._lAlarmState.Name = "_lAlarmState";
            this._lAlarmState.Size = new System.Drawing.Size(59, 13);
            this._lAlarmState.TabIndex = 77;
            this._lAlarmState.Text = "Alarm state";
            // 
            // _eAlarmState
            // 
            this._eAlarmState.BackColor = System.Drawing.Color.White;
            this._eAlarmState.Location = new System.Drawing.Point(180, 128);
            this._eAlarmState.Name = "_eAlarmState";
            this._eAlarmState.ReadOnly = true;
            this._eAlarmState.Size = new System.Drawing.Size(246, 20);
            this._eAlarmState.TabIndex = 78;
            // 
            // _ilbParentObject
            // 
            this._ilbParentObject.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._ilbParentObject.FormattingEnabled = true;
            this._ilbParentObject.ImageList = null;
            this._ilbParentObject.ItemHeight = 16;
            this._ilbParentObject.Location = new System.Drawing.Point(181, 28);
            this._ilbParentObject.Name = "_ilbParentObject";
            this._ilbParentObject.SelectedItemObject = null;
            this._ilbParentObject.Size = new System.Drawing.Size(245, 68);
            this._ilbParentObject.TabIndex = 1;
            this._ilbParentObject.MouseUp += new System.Windows.Forms.MouseEventHandler(this._ilbParentObject_MouseUp);
            this._ilbParentObject.DoubleClick += new System.EventHandler(this._ilbParentObject_DoubleClick);
            // 
            // _tcAlarmDetails
            // 
            this._tcAlarmDetails.Controls.Add(this._tpDescription);
            this._tcAlarmDetails.Controls.Add(this._tpParameters);
            this._tcAlarmDetails.Controls.Add(this._tpAlarmInstructions);
            this._tcAlarmDetails.Location = new System.Drawing.Point(12, 232);
            this._tcAlarmDetails.Name = "_tcAlarmDetails";
            this._tcAlarmDetails.SelectedIndex = 0;
            this._tcAlarmDetails.Size = new System.Drawing.Size(414, 222);
            this._tcAlarmDetails.TabIndex = 81;
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(3);
            this._tpDescription.Size = new System.Drawing.Size(406, 196);
            this._tpDescription.TabIndex = 0;
            this._tpDescription.Text = "Description";
            // 
            // _tpParameters
            // 
            this._tpParameters.BackColor = System.Drawing.SystemColors.Control;
            this._tpParameters.Controls.Add(this._dgvParameters);
            this._tpParameters.Location = new System.Drawing.Point(4, 22);
            this._tpParameters.Name = "_tpParameters";
            this._tpParameters.Padding = new System.Windows.Forms.Padding(3);
            this._tpParameters.Size = new System.Drawing.Size(406, 196);
            this._tpParameters.TabIndex = 1;
            this._tpParameters.Text = "Parameters";
            // 
            // _tpAlarmInstructions
            // 
            this._tpAlarmInstructions.BackColor = System.Drawing.SystemColors.Control;
            this._tpAlarmInstructions.Controls.Add(this._bEdit);
            this._tpAlarmInstructions.Controls.Add(this._dgAlarmInstructions);
            this._tpAlarmInstructions.Location = new System.Drawing.Point(4, 22);
            this._tpAlarmInstructions.Name = "_tpAlarmInstructions";
            this._tpAlarmInstructions.Size = new System.Drawing.Size(406, 196);
            this._tpAlarmInstructions.TabIndex = 2;
            this._tpAlarmInstructions.Text = "AlarmInstructions";
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(328, 170);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 78;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _dgAlarmInstructions
            // 
            this._dgAlarmInstructions.AllowUserToAddRows = false;
            this._dgAlarmInstructions.AllowUserToDeleteRows = false;
            this._dgAlarmInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgAlarmInstructions.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this._dgAlarmInstructions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgAlarmInstructions.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._dgAlarmInstructions.DefaultCellStyle = dataGridViewCellStyle1;
            this._dgAlarmInstructions.Location = new System.Drawing.Point(3, 3);
            this._dgAlarmInstructions.MultiSelect = false;
            this._dgAlarmInstructions.Name = "_dgAlarmInstructions";
            this._dgAlarmInstructions.ReadOnly = true;
            this._dgAlarmInstructions.RowHeadersVisible = false;
            this._dgAlarmInstructions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgAlarmInstructions.Size = new System.Drawing.Size(400, 161);
            this._dgAlarmInstructions.TabIndex = 77;
            this._dgAlarmInstructions.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._dgAlarmInstructions_MouseDoubleClick);
            this._dgAlarmInstructions.SelectionChanged += new System.EventHandler(this._dgAlarmInstructions_SelectionChanged);
            // 
            // _bAcknowledgeAndBlock
            // 
            this._bAcknowledgeAndBlock.Location = new System.Drawing.Point(12, 460);
            this._bAcknowledgeAndBlock.Name = "_bAcknowledgeAndBlock";
            this._bAcknowledgeAndBlock.Size = new System.Drawing.Size(142, 22);
            this._bAcknowledgeAndBlock.TabIndex = 82;
            this._bAcknowledgeAndBlock.Text = "AcknowledgeAndBlock";
            this._bAcknowledgeAndBlock.UseVisualStyleBackColor = true;
            this._bAcknowledgeAndBlock.Click += new System.EventHandler(this._bAcknowledgeAndBlock_Click);
            // 
            // _lAcknowledgement
            // 
            this._lAcknowledgement.AutoSize = true;
            this._lAcknowledgement.Location = new System.Drawing.Point(12, 157);
            this._lAcknowledgement.Name = "_lAcknowledgement";
            this._lAcknowledgement.Size = new System.Drawing.Size(95, 13);
            this._lAcknowledgement.TabIndex = 83;
            this._lAcknowledgement.Text = "Acknowledgement";
            // 
            // _pbAcknowledgement
            // 
            this._pbAcknowledgement.Location = new System.Drawing.Point(180, 154);
            this._pbAcknowledgement.Name = "_pbAcknowledgement";
            this._pbAcknowledgement.Size = new System.Drawing.Size(20, 20);
            this._pbAcknowledgement.TabIndex = 84;
            this._pbAcknowledgement.TabStop = false;
            // 
            // _pbIndividualBlockingState
            // 
            this._pbIndividualBlockingState.Location = new System.Drawing.Point(180, 180);
            this._pbIndividualBlockingState.Name = "_pbIndividualBlockingState";
            this._pbIndividualBlockingState.Size = new System.Drawing.Size(20, 20);
            this._pbIndividualBlockingState.TabIndex = 85;
            this._pbIndividualBlockingState.TabStop = false;
            // 
            // _pbGeneralBlockingState
            // 
            this._pbGeneralBlockingState.Location = new System.Drawing.Point(180, 206);
            this._pbGeneralBlockingState.Name = "_pbGeneralBlockingState";
            this._pbGeneralBlockingState.Size = new System.Drawing.Size(20, 20);
            this._pbGeneralBlockingState.TabIndex = 86;
            this._pbGeneralBlockingState.TabStop = false;
            // 
            // _lIndividualBlockingState
            // 
            this._lIndividualBlockingState.AutoSize = true;
            this._lIndividualBlockingState.Location = new System.Drawing.Point(12, 183);
            this._lIndividualBlockingState.Name = "_lIndividualBlockingState";
            this._lIndividualBlockingState.Size = new System.Drawing.Size(121, 13);
            this._lIndividualBlockingState.TabIndex = 87;
            this._lIndividualBlockingState.Text = "Individual blocking state";
            // 
            // _lGeneralBlockingState
            // 
            this._lGeneralBlockingState.AutoSize = true;
            this._lGeneralBlockingState.Location = new System.Drawing.Point(12, 209);
            this._lGeneralBlockingState.Name = "_lGeneralBlockingState";
            this._lGeneralBlockingState.Size = new System.Drawing.Size(113, 13);
            this._lGeneralBlockingState.TabIndex = 88;
            this._lGeneralBlockingState.Text = "General blocking state";
            // 
            // AlarmDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(438, 494);
            this.Controls.Add(this._lGeneralBlockingState);
            this.Controls.Add(this._lIndividualBlockingState);
            this.Controls.Add(this._pbGeneralBlockingState);
            this.Controls.Add(this._pbIndividualBlockingState);
            this.Controls.Add(this._pbAcknowledgement);
            this.Controls.Add(this._lAcknowledgement);
            this.Controls.Add(this._bAcknowledgeAndBlock);
            this.Controls.Add(this._tcAlarmDetails);
            this.Controls.Add(this._ilbParentObject);
            this.Controls.Add(this._eAlarmState);
            this.Controls.Add(this._lAlarmState);
            this.Controls.Add(this._bBlock);
            this.Controls.Add(this._bUnblock);
            this.Controls.Add(this._bAcknowledgeState);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._lRelativeUniqueIdentificate);
            this.Controls.Add(this._eRelativeUniqueIdentificate);
            this.Controls.Add(this._lParentObject);
            this.Controls.Add(this._lCreatedDateTime);
            this.Controls.Add(this._eCreatedDateTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AlarmDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AlarmDetailsForm";
            ((System.ComponentModel.ISupportInitialize)(this._dgvParameters)).EndInit();
            this._tcAlarmDetails.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._tpParameters.ResumeLayout(false);
            this._tpAlarmInstructions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgAlarmInstructions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbAcknowledgement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbIndividualBlockingState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pbGeneralBlockingState)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lCreatedDateTime;
        private System.Windows.Forms.TextBox _eCreatedDateTime;
        private System.Windows.Forms.Label _lParentObject;
        private System.Windows.Forms.Label _lRelativeUniqueIdentificate;
        private System.Windows.Forms.TextBox _eRelativeUniqueIdentificate;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bAcknowledgeState;
        private System.Windows.Forms.DataGridView _dgvParameters;
        private System.Windows.Forms.Button _bUnblock;
        private System.Windows.Forms.Button _bBlock;
        private System.Windows.Forms.Label _lAlarmState;
        private System.Windows.Forms.TextBox _eAlarmState;
        private Contal.IwQuick.UI.ImageListBox _ilbParentObject;
        private System.Windows.Forms.TabControl _tcAlarmDetails;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TabPage _tpParameters;
        private System.Windows.Forms.TabPage _tpAlarmInstructions;
        private System.Windows.Forms.DataGridView _dgAlarmInstructions;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bAcknowledgeAndBlock;
        private System.Windows.Forms.Label _lAcknowledgement;
        private System.Windows.Forms.PictureBox _pbAcknowledgement;
        private System.Windows.Forms.PictureBox _pbIndividualBlockingState;
        private System.Windows.Forms.PictureBox _pbGeneralBlockingState;
        private System.Windows.Forms.Label _lIndividualBlockingState;
        private System.Windows.Forms.Label _lGeneralBlockingState;
    }
}