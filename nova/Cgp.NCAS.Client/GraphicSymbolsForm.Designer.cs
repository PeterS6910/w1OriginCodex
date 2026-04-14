namespace Contal.Cgp.NCAS.Client
{
    partial class GraphicSymbolsForm
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
            this._bInsert = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._ehGraphicSymbolsList = new System.Windows.Forms.Integration.ElementHost();
            this.graphicSymbolsViewer1 = new Contal.Cgp.NCAS.Client.GraphicSymbolsViewer();
            this.SuspendLayout();
            // 
            // _bInsert
            // 
            this._bInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bInsert.Location = new System.Drawing.Point(8, 469);
            this._bInsert.Name = "_bInsert";
            this._bInsert.Size = new System.Drawing.Size(96, 28);
            this._bInsert.TabIndex = 1;
            this._bInsert.Text = "Insert";
            this._bInsert.UseVisualStyleBackColor = true;
            this._bInsert.Click += new System.EventHandler(this._bInsert_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bEdit.Location = new System.Drawing.Point(110, 469);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(96, 28);
            this._bEdit.TabIndex = 2;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bDelete.Location = new System.Drawing.Point(212, 469);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(96, 28);
            this._bDelete.TabIndex = 3;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _ehGraphicSymbolsList
            // 
            this._ehGraphicSymbolsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ehGraphicSymbolsList.Location = new System.Drawing.Point(0, 0);
            this._ehGraphicSymbolsList.Name = "_ehGraphicSymbolsList";
            this._ehGraphicSymbolsList.Size = new System.Drawing.Size(699, 452);
            this._ehGraphicSymbolsList.TabIndex = 0;
            this._ehGraphicSymbolsList.Text = "_ehGraphicSymbolsList";
            this._ehGraphicSymbolsList.Child = this.graphicSymbolsViewer1;
            // 
            // GraphicSymbolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(699, 509);
            this.Controls.Add(this._bDelete);
            this.Controls.Add(this._bEdit);
            this.Controls.Add(this._bInsert);
            this.Controls.Add(this._ehGraphicSymbolsList);
            this.Name = "GraphicSymbolsForm";
            this.Text = "Graphic symbols";
            this.Load += new System.EventHandler(this.GraphicSymbolsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost _ehGraphicSymbolsList;
        private System.Windows.Forms.Button _bInsert;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bDelete;
        private GraphicSymbolsViewer graphicSymbolsViewer1;
    }
}
