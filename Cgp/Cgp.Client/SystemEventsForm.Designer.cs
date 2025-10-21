namespace Contal.Cgp.Client
{
    partial class SystemEventsForm
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
            this._pControl = new System.Windows.Forms.Panel();
            this._bDelete = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._bInsert = new System.Windows.Forms.Button();
            this._dgValues = new System.Windows.Forms.DataGridView();
            this._pControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).BeginInit();
            this.SuspendLayout();
            // 
            // _pControl
            // 
            this._pControl.Controls.Add(this._bDelete);
            this._pControl.Controls.Add(this._bEdit);
            this._pControl.Controls.Add(this._bInsert);
            this._pControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pControl.Location = new System.Drawing.Point(0, 333);
            this._pControl.Name = "_pControl";
            this._pControl.Size = new System.Drawing.Size(929, 37);
            this._pControl.TabIndex = 8;
            // 
            // _bDelete
            // 
            this._bDelete.Location = new System.Drawing.Point(174, 6);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(75, 23);
            this._bDelete.TabIndex = 2;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Location = new System.Drawing.Point(93, 6);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 1;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _bInsert
            // 
            this._bInsert.Location = new System.Drawing.Point(12, 6);
            this._bInsert.Name = "_bInsert";
            this._bInsert.Size = new System.Drawing.Size(75, 23);
            this._bInsert.TabIndex = 0;
            this._bInsert.Text = "Insert";
            this._bInsert.UseVisualStyleBackColor = true;
            this._bInsert.Click += new System.EventHandler(this._bInsert_Click);
            // 
            // _dgValues
            // 
            this._dgValues.AllowUserToAddRows = false;
            this._dgValues.AllowUserToDeleteRows = false;
            this._dgValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgValues.Location = new System.Drawing.Point(0, 0);
            this._dgValues.Name = "_dgValues";
            this._dgValues.ReadOnly = true;
            this._dgValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgValues.Size = new System.Drawing.Size(929, 333);
            this._dgValues.TabIndex = 9;
            this._dgValues.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this._dgValues_CellMouseDoubleClick);
            // 
            // SystemEventsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 370);
            this.Controls.Add(this._dgValues);
            this.Controls.Add(this._pControl);
            this.Name = "SystemEventsForm";
            this.Text = "SystemEventsForm";
            this.Enter += new System.EventHandler(this.SystemEventsForm_Enter);
            this.Leave += new System.EventHandler(this.SystemEventsForm_Leave);
            this._pControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pControl;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bInsert;
        private System.Windows.Forms.DataGridView _dgValues;


    }
}