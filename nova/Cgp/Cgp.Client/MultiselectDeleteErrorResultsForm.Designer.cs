namespace Contal.Cgp.Client
{
    partial class MultiselectDeleteErrorResultsForm
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
            this._dgErrorResults = new System.Windows.Forms.DataGridView();
            this.AOrmObject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeleteError = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._bCancel = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._dgErrorResults)).BeginInit();
            this.SuspendLayout();
            // 
            // _dgErrorResults
            // 
            this._dgErrorResults.AllowUserToAddRows = false;
            this._dgErrorResults.AllowUserToDeleteRows = false;
            this._dgErrorResults.AllowUserToResizeColumns = true;
            this._dgErrorResults.AllowUserToResizeRows = false;
            this._dgErrorResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgErrorResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgErrorResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AOrmObject,
            this.ObjectName,
            this.DeleteError});
            this._dgErrorResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgErrorResults.Location = new System.Drawing.Point(12, 12);
            this._dgErrorResults.MultiSelect = false;
            this._dgErrorResults.Name = "_dgErrorResults";
            this._dgErrorResults.RowHeadersVisible = false;
            this._dgErrorResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgErrorResults.Size = new System.Drawing.Size(702, 383);
            this._dgErrorResults.TabIndex = 13;
            this._dgErrorResults.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this._dgErrorResults_CellMouseDoubleClick);
            // 
            // AOrmObject
            // 
            this.AOrmObject.DataPropertyName = "AOrmObject";
            this.AOrmObject.HeaderText = "ORM object";
            this.AOrmObject.Name = "AOrmObject";
            this.AOrmObject.Visible = false;
            // 
            // ObjectName
            // 
            this.ObjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ObjectName.DataPropertyName = "FullName";
            this.ObjectName.HeaderText = "Object name";
            this.ObjectName.MinimumWidth = 120;
            this.ObjectName.Name = "ObjectName";
            this.ObjectName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ObjectName.Width = 120;
            // 
            // DeleteError
            // 
            this.DeleteError.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DeleteError.DataPropertyName = "Error";
            this.DeleteError.HeaderText = "Delet error";
            this.DeleteError.MinimumWidth = 120;
            this.DeleteError.Name = "DeleteError";
            this.DeleteError.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(639, 401);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 14;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bClose_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bEdit.Location = new System.Drawing.Point(558, 401);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 15;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // MultiselectDeleteErrorResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 436);
            this.Controls.Add(this._bEdit);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._dgErrorResults);
            this.Name = "MultiselectDeleteErrorResultsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MultiselectDeleteErrorResultsForm";
            ((System.ComponentModel.ISupportInitialize)(this._dgErrorResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgErrorResults;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.DataGridViewTextBoxColumn AOrmObject;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeleteError;
    }
}