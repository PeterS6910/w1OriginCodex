namespace Contal.Cgp.Client
{
    partial class ListboxFormAdd
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
            this._bOk = new System.Windows.Forms.Button();
            this._eFilter = new System.Windows.Forms.TextBox();
            this._lShown = new System.Windows.Forms.Label();
            this._lShownNumber = new System.Windows.Forms.Label();
            this._icbObjectTypeFilter = new Contal.IwQuick.UI.ImageComboBox();
            this._ilbData = new Contal.IwQuick.UI.ImageListBox();
            this.SuspendLayout();
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(430, 332);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 29);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bOk.Location = new System.Drawing.Point(329, 332);
            this._bOk.Margin = new System.Windows.Forms.Padding(4);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(94, 29);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _eFilter
            // 
            this._eFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._eFilter.Location = new System.Drawing.Point(8, 8);
            this._eFilter.Margin = new System.Windows.Forms.Padding(4);
            this._eFilter.Name = "_eFilter";
            this._eFilter.Size = new System.Drawing.Size(298, 20);
            this._eFilter.TabIndex = 0;
            this._eFilter.KeyUp += new System.Windows.Forms.KeyEventHandler(this._eFilter_KeyUp);
            // 
            // _lShown
            // 
            this._lShown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lShown.AutoSize = true;
            this._lShown.Location = new System.Drawing.Point(8, 330);
            this._lShown.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lShown.Name = "_lShown";
            this._lShown.Size = new System.Drawing.Size(43, 13);
            this._lShown.TabIndex = 53;
            this._lShown.Text = "Shown:";
            // 
            // _lShownNumber
            // 
            this._lShownNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lShownNumber.AutoSize = true;
            this._lShownNumber.Location = new System.Drawing.Point(8, 349);
            this._lShownNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lShownNumber.Name = "_lShownNumber";
            this._lShownNumber.Size = new System.Drawing.Size(24, 13);
            this._lShownNumber.TabIndex = 54;
            this._lShownNumber.Text = "n/n";
            // 
            // _icbObjectTypeFilter
            // 
            this._icbObjectTypeFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._icbObjectTypeFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._icbObjectTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._icbObjectTypeFilter.FormattingEnabled = true;
            this._icbObjectTypeFilter.ImageList = null;
            this._icbObjectTypeFilter.Localizationhelper = null;
            this._icbObjectTypeFilter.Location = new System.Drawing.Point(314, 8);
            this._icbObjectTypeFilter.Margin = new System.Windows.Forms.Padding(4);
            this._icbObjectTypeFilter.Name = "_icbObjectTypeFilter";
            this._icbObjectTypeFilter.SelectedItemObject = null;
            this._icbObjectTypeFilter.SelectedItemObjectType = null;
            this._icbObjectTypeFilter.Size = new System.Drawing.Size(209, 21);
            this._icbObjectTypeFilter.TabIndex = 4;
            this._icbObjectTypeFilter.TranslationPrefix = "ObjectType_";
            this._icbObjectTypeFilter.Visible = false;
            this._icbObjectTypeFilter.SelectedValueChanged += new System.EventHandler(this._icbObjectTypeFilter_SelectedValueChanged);
            // 
            // _ilbData
            // 
            this._ilbData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._ilbData.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._ilbData.FormattingEnabled = true;
            this._ilbData.ImageList = null;
            this._ilbData.ItemHeight = 16;
            this._ilbData.Location = new System.Drawing.Point(8, 41);
            this._ilbData.Margin = new System.Windows.Forms.Padding(4);
            this._ilbData.Name = "_ilbData";
            this._ilbData.SelectedItemObject = null;
            this._ilbData.Size = new System.Drawing.Size(515, 276);
            this._ilbData.TabIndex = 1;
            this._ilbData.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._ilbData_MouseDoubleClick);
            // 
            // ListboxFormAdd
            // 
            this.AcceptButton = this._bOk;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(531, 376);
            this.ControlBox = false;
            this.Controls.Add(this._ilbData);
            this.Controls.Add(this._icbObjectTypeFilter);
            this.Controls.Add(this._lShownNumber);
            this.Controls.Add(this._lShown);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._eFilter);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(370, 238);
            this.Name = "ListboxFormAdd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ListboxForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListboxFormAdd_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TextBox _eFilter;
        private System.Windows.Forms.Label _lShown;
        private System.Windows.Forms.Label _lShownNumber;
        private Contal.IwQuick.UI.ImageComboBox _icbObjectTypeFilter;
        private Contal.IwQuick.UI.ImageListBox _ilbData;

    }
}