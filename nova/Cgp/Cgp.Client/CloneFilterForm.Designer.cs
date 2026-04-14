namespace Contal.Cgp.Client
{
    partial class CloneFilterForm
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
            this._bCancel = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._pBack = new System.Windows.Forms.Panel();
            this._lvFilterValues = new System.Windows.Forms.ListView();
            this._columnMain = new System.Windows.Forms.ColumnHeader();
            this._pBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(136, 301);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 0;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bNext
            // 
            this._bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bNext.Location = new System.Drawing.Point(217, 301);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(75, 23);
            this._bNext.TabIndex = 0;
            this._bNext.Text = "Next";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _pBack
            // 
            this._pBack.Controls.Add(this._lvFilterValues);
            this._pBack.Controls.Add(this._bNext);
            this._pBack.Controls.Add(this._bCancel);
            this._pBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pBack.Location = new System.Drawing.Point(0, 0);
            this._pBack.Name = "_pBack";
            this._pBack.Size = new System.Drawing.Size(304, 332);
            this._pBack.TabIndex = 0;
            // 
            // _lvFilterValues
            // 
            this._lvFilterValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lvFilterValues.CheckBoxes = true;
            this._lvFilterValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._columnMain});
            this._lvFilterValues.FullRowSelect = true;
            this._lvFilterValues.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._lvFilterValues.Location = new System.Drawing.Point(13, 12);
            this._lvFilterValues.Name = "_lvFilterValues";
            this._lvFilterValues.Size = new System.Drawing.Size(279, 283);
            this._lvFilterValues.TabIndex = 0;
            this._lvFilterValues.UseCompatibleStateImageBehavior = false;
            this._lvFilterValues.View = System.Windows.Forms.View.Details;
            this._lvFilterValues.SelectedIndexChanged += new System.EventHandler(this._lvFilterValues_SelectedIndexChanged);
            // 
            // CloneFilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(304, 332);
            this.ControlBox = false;
            this.Controls.Add(this._pBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CloneFilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CloneFilterForm";
            this.Load += new System.EventHandler(this.CloneFilterForm_Load);
            this._pBack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Panel _pBack;
        private System.Windows.Forms.ListView _lvFilterValues;
        private System.Windows.Forms.ColumnHeader _columnMain;
    }
}