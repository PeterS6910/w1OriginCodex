namespace Contal.Cgp.Client
{
    partial class ListboxFormShow
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
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._eFilter = new System.Windows.Forms.TextBox();
            this._lbData = new System.Windows.Forms.ListBox();
            this._lLBDescription = new System.Windows.Forms.Label();
            this._bDelete = new System.Windows.Forms.Button();
            this._bAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Location = new System.Drawing.Point(816, 33);
            this._bFilterClear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(100, 28);
            this._bFilterClear.TabIndex = 40;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Location = new System.Drawing.Point(708, 33);
            this._bRunFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(100, 28);
            this._bRunFilter.TabIndex = 39;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _eFilter
            // 
            this._eFilter.Location = new System.Drawing.Point(391, 36);
            this._eFilter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._eFilter.Name = "_eFilter";
            this._eFilter.Size = new System.Drawing.Size(308, 22);
            this._eFilter.TabIndex = 38;
            this._eFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this._eFilter_KeyDown);
            // 
            // _lbData
            // 
            this._lbData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbData.FormattingEnabled = true;
            this._lbData.ItemHeight = 16;
            this._lbData.Location = new System.Drawing.Point(391, 69);
            this._lbData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._lbData.Name = "_lbData";
            this._lbData.Size = new System.Drawing.Size(523, 308);
            this._lbData.TabIndex = 37;
            // 
            // _lLBDescription
            // 
            this._lLBDescription.AutoSize = true;
            this._lLBDescription.Location = new System.Drawing.Point(388, 9);
            this._lLBDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lLBDescription.Name = "_lLBDescription";
            this._lLBDescription.Size = new System.Drawing.Size(79, 17);
            this._lLBDescription.TabIndex = 41;
            this._lLBDescription.Text = "Description";
            // 
            // _bDelete
            // 
            this._bDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bDelete.Location = new System.Drawing.Point(814, 390);
            this._bDelete.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(100, 28);
            this._bDelete.TabIndex = 45;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bAdd
            // 
            this._bAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bAdd.Location = new System.Drawing.Point(706, 390);
            this._bAdd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bAdd.Name = "_bAdd";
            this._bAdd.Size = new System.Drawing.Size(100, 28);
            this._bAdd.TabIndex = 44;
            this._bAdd.Text = "Add";
            this._bAdd.UseVisualStyleBackColor = true;
            this._bAdd.Click += new System.EventHandler(this._bAdd_Click);
            // 
            // ListboxFormShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 431);
            this.ControlBox = false;
            this.Controls.Add(this._bDelete);
            this.Controls.Add(this._bAdd);
            this.Controls.Add(this._lLBDescription);
            this.Controls.Add(this._bFilterClear);
            this.Controls.Add(this._bRunFilter);
            this.Controls.Add(this._eFilter);
            this.Controls.Add(this._lbData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ListboxFormShow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ListboxFormShow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.TextBox _eFilter;
        private System.Windows.Forms.ListBox _lbData;
        private System.Windows.Forms.Label _lLBDescription;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bAdd;
    }
}