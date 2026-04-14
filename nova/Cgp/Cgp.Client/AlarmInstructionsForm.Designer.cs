namespace Contal.Cgp.Client
{
    partial class AlarmInstructionsForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._lLocalInstruction = new System.Windows.Forms.Label();
            this._eLocalInstruction = new System.Windows.Forms.TextBox();
            this._lGlobalInstructions = new System.Windows.Forms.Label();
            this._dgGlobalInstructions = new System.Windows.Forms.DataGridView();
            this._bCreate = new System.Windows.Forms.Button();
            this._bInsert = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._pLocalAlarmInstructions = new System.Windows.Forms.Panel();
            this._pGlobalAlarmInstruction = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._dgGlobalInstructions)).BeginInit();
            this._pLocalAlarmInstructions.SuspendLayout();
            this._pGlobalAlarmInstruction.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lLocalInstruction
            // 
            this._lLocalInstruction.AutoSize = true;
            this._lLocalInstruction.Location = new System.Drawing.Point(3, 6);
            this._lLocalInstruction.Name = "_lLocalInstruction";
            this._lLocalInstruction.Size = new System.Drawing.Size(84, 13);
            this._lLocalInstruction.TabIndex = 0;
            this._lLocalInstruction.Text = "Local instruction";
            // 
            // _eLocalInstruction
            // 
            this._eLocalInstruction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eLocalInstruction.Location = new System.Drawing.Point(3, 22);
            this._eLocalInstruction.Multiline = true;
            this._eLocalInstruction.Name = "_eLocalInstruction";
            this._eLocalInstruction.Size = new System.Drawing.Size(327, 72);
            this._eLocalInstruction.TabIndex = 1;
            this._eLocalInstruction.TextChanged += new System.EventHandler(this._eLocalInstruction_TextChanged);
            // 
            // _lGlobalInstructions
            // 
            this._lGlobalInstructions.AutoSize = true;
            this._lGlobalInstructions.Location = new System.Drawing.Point(3, 6);
            this._lGlobalInstructions.Name = "_lGlobalInstructions";
            this._lGlobalInstructions.Size = new System.Drawing.Size(93, 13);
            this._lGlobalInstructions.TabIndex = 2;
            this._lGlobalInstructions.Text = "Global instructions";
            // 
            // _dgGlobalInstructions
            // 
            this._dgGlobalInstructions.AllowUserToAddRows = false;
            this._dgGlobalInstructions.AllowUserToDeleteRows = false;
            this._dgGlobalInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgGlobalInstructions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgGlobalInstructions.Location = new System.Drawing.Point(3, 22);
            this._dgGlobalInstructions.Name = "_dgGlobalInstructions";
            this._dgGlobalInstructions.ReadOnly = true;
            this._dgGlobalInstructions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgGlobalInstructions.Size = new System.Drawing.Size(327, 138);
            this._dgGlobalInstructions.TabIndex = 1;
            this._dgGlobalInstructions.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._dgGlobalInstructions_MouseDoubleClick);
            this._dgGlobalInstructions.DragOver += new System.Windows.Forms.DragEventHandler(this._dgGlobalInstructions_DragOver);
            this._dgGlobalInstructions.DragDrop += new System.Windows.Forms.DragEventHandler(this._dgGlobalInstructions_DragDrop);
            // 
            // _bCreate
            // 
            this._bCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreate.Location = new System.Drawing.Point(12, 166);
            this._bCreate.Name = "_bCreate";
            this._bCreate.Size = new System.Drawing.Size(75, 23);
            this._bCreate.TabIndex = 2;
            this._bCreate.Text = "Create";
            this._bCreate.UseVisualStyleBackColor = true;
            this._bCreate.Click += new System.EventHandler(this._bCreate_Click);
            // 
            // _bInsert
            // 
            this._bInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bInsert.Location = new System.Drawing.Point(93, 166);
            this._bInsert.Name = "_bInsert";
            this._bInsert.Size = new System.Drawing.Size(75, 23);
            this._bInsert.TabIndex = 3;
            this._bInsert.Text = "Insert";
            this._bInsert.UseVisualStyleBackColor = true;
            this._bInsert.Click += new System.EventHandler(this._bInsert_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(255, 166);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(75, 23);
            this._bDelete.TabIndex = 5;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(174, 166);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 4;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _pLocalAlarmInstructions
            // 
            this._pLocalAlarmInstructions.Controls.Add(this._lLocalInstruction);
            this._pLocalAlarmInstructions.Controls.Add(this._eLocalInstruction);
            this._pLocalAlarmInstructions.Dock = System.Windows.Forms.DockStyle.Top;
            this._pLocalAlarmInstructions.Location = new System.Drawing.Point(0, 0);
            this._pLocalAlarmInstructions.Name = "_pLocalAlarmInstructions";
            this._pLocalAlarmInstructions.Size = new System.Drawing.Size(333, 97);
            this._pLocalAlarmInstructions.TabIndex = 1;
            // 
            // _pGlobalAlarmInstruction
            // 
            this._pGlobalAlarmInstruction.Controls.Add(this._lGlobalInstructions);
            this._pGlobalAlarmInstruction.Controls.Add(this._dgGlobalInstructions);
            this._pGlobalAlarmInstruction.Controls.Add(this._bEdit);
            this._pGlobalAlarmInstruction.Controls.Add(this._bDelete);
            this._pGlobalAlarmInstruction.Controls.Add(this._bCreate);
            this._pGlobalAlarmInstruction.Controls.Add(this._bInsert);
            this._pGlobalAlarmInstruction.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pGlobalAlarmInstruction.Location = new System.Drawing.Point(0, 97);
            this._pGlobalAlarmInstruction.Name = "_pGlobalAlarmInstruction";
            this._pGlobalAlarmInstruction.Size = new System.Drawing.Size(333, 192);
            this._pGlobalAlarmInstruction.TabIndex = 2;
            // 
            // AlarmInstructionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this._pGlobalAlarmInstruction);
            this.Controls.Add(this._pLocalAlarmInstructions);
            this.Name = "AlarmInstructionsForm";
            this.Size = new System.Drawing.Size(333, 289);
            ((System.ComponentModel.ISupportInitialize)(this._dgGlobalInstructions)).EndInit();
            this._pLocalAlarmInstructions.ResumeLayout(false);
            this._pLocalAlarmInstructions.PerformLayout();
            this._pGlobalAlarmInstruction.ResumeLayout(false);
            this._pGlobalAlarmInstruction.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _lLocalInstruction;
        private System.Windows.Forms.TextBox _eLocalInstruction;
        private System.Windows.Forms.Label _lGlobalInstructions;
        private System.Windows.Forms.DataGridView _dgGlobalInstructions;
        private System.Windows.Forms.Button _bCreate;
        private System.Windows.Forms.Button _bInsert;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Panel _pLocalAlarmInstructions;
        private System.Windows.Forms.Panel _pGlobalAlarmInstruction;
    }
}
