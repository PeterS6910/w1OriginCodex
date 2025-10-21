using Contal.Cgp.Components;

namespace Cgp.Components
{
    partial class DeleteDataGridItemsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteDataGridItemsDialog));
            this._bCancel = new System.Windows.Forms.Button();
            this._bDeleteAll = new System.Windows.Forms.Button();
            this._bDelete = new System.Windows.Forms.Button();
            this._lQuestion = new System.Windows.Forms.Label();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this._wpfObjectListView = new WpfObjectListView();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(361, 171);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(81, 25);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "CANCEL";
            this._bCancel.UseVisualStyleBackColor = true;
            // 
            // _bDeleteAll
            // 
            this._bDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDeleteAll.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bDeleteAll.Location = new System.Drawing.Point(160, 171);
            this._bDeleteAll.Name = "_bDeleteAll";
            this._bDeleteAll.Size = new System.Drawing.Size(81, 25);
            this._bDeleteAll.TabIndex = 4;
            this._bDeleteAll.Text = "DeleteAll";
            this._bDeleteAll.UseVisualStyleBackColor = true;
            this._bDeleteAll.Click += new System.EventHandler(this._bDeleteAll_Click);
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bDelete.Location = new System.Drawing.Point(247, 171);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(108, 25);
            this._bDelete.TabIndex = 6;
            this._bDelete.Text = "DeleteSelected";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _lQuestion
            // 
            this._lQuestion.AutoSize = true;
            this._lQuestion.Location = new System.Drawing.Point(12, 9);
            this._lQuestion.Name = "_lQuestion";
            this._lQuestion.Size = new System.Drawing.Size(170, 13);
            this._lQuestion.TabIndex = 7;
            this._lQuestion.Text = "Doyou want to delete these items?";
            // 
            // elementHost1
            // 
            this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost1.Location = new System.Drawing.Point(12, 35);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(424, 130);
            this.elementHost1.TabIndex = 8;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this._wpfObjectListView;
            // 
            // DeleteDataGridItemsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 204);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this._lQuestion);
            this.Controls.Add(this._bDelete);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bDeleteAll);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(464, 243);
            this.Name = "DeleteDataGridItemsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DeleteDataGridItemsDialog";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DeleteDataGridItemsDialog_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bDeleteAll;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Label _lQuestion;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private WpfObjectListView _wpfObjectListView;
    }
}