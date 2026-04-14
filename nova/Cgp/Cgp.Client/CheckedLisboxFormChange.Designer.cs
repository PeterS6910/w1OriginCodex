namespace Contal.Cgp.Client
{
    partial class CheckedLisboxFormChange
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
            this._chblbObjects = new System.Windows.Forms.CheckedListBox();
            this._eFilter = new System.Windows.Forms.TextBox();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._lShown = new System.Windows.Forms.Label();
            this._lShownNumber = new System.Windows.Forms.Label();
            this._bClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _chblbObjects
            // 
            this._chblbObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._chblbObjects.FormattingEnabled = true;
            this._chblbObjects.Location = new System.Drawing.Point(12, 38);
            this._chblbObjects.Name = "_chblbObjects";
            this._chblbObjects.Size = new System.Drawing.Size(514, 244);
            this._chblbObjects.Sorted = true;
            this._chblbObjects.TabIndex = 0;
            this._chblbObjects.MouseClick += new System.Windows.Forms.MouseEventHandler(this._chblbObjects_MouseClick);
            this._chblbObjects.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._chblbObjects_ItemCheck);
            // 
            // _eFilter
            // 
            this._eFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eFilter.Location = new System.Drawing.Point(12, 12);
            this._eFilter.Name = "_eFilter";
            this._eFilter.Size = new System.Drawing.Size(514, 20);
            this._eFilter.TabIndex = 1;
            this._eFilter.KeyUp += new System.Windows.Forms.KeyEventHandler(this._eFilter_KeyUp);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(370, 326);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(451, 326);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lShown
            // 
            this._lShown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lShown.AutoSize = true;
            this._lShown.Location = new System.Drawing.Point(9, 315);
            this._lShown.Name = "_lShown";
            this._lShown.Size = new System.Drawing.Size(43, 13);
            this._lShown.TabIndex = 4;
            this._lShown.Text = "Shown:";
            // 
            // _lShownNumber
            // 
            this._lShownNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lShownNumber.AutoSize = true;
            this._lShownNumber.Location = new System.Drawing.Point(9, 333);
            this._lShownNumber.Name = "_lShownNumber";
            this._lShownNumber.Size = new System.Drawing.Size(24, 13);
            this._lShownNumber.TabIndex = 5;
            this._lShownNumber.Text = "n/n";
            // 
            // _bClear
            // 
            this._bClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bClear.Location = new System.Drawing.Point(451, 288);
            this._bClear.Name = "_bClear";
            this._bClear.Size = new System.Drawing.Size(75, 23);
            this._bClear.TabIndex = 6;
            this._bClear.Text = "Clear";
            this._bClear.UseVisualStyleBackColor = true;
            this._bClear.Click += new System.EventHandler(this._bClear_Click);
            // 
            // CheckedLisboxFormChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 361);
            this.ControlBox = false;
            this.Controls.Add(this._bClear);
            this.Controls.Add(this._lShownNumber);
            this.Controls.Add(this._lShown);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._eFilter);
            this.Controls.Add(this._chblbObjects);
            this.KeyPreview = true;
            this.Name = "CheckedLisboxFormChange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CheckedLisboxFormChange";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CheckedLisboxFormAdd_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox _chblbObjects;
        private System.Windows.Forms.TextBox _eFilter;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lShown;
        private System.Windows.Forms.Label _lShownNumber;
        private System.Windows.Forms.Button _bClear;
    }
}