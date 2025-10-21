namespace Contal.Cgp.Client
{
    partial class ReferencedByForm
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
            this._bRefresh = new System.Windows.Forms.Button();
            this._tbFilter = new System.Windows.Forms.TextBox();
            this._dgRootReferencedBy = new System.Windows.Forms.DataGridView();
            this._bFilter = new System.Windows.Forms.Button();
            this._icbObjectType = new Contal.IwQuick.UI.ImageComboBox();
            this._lText = new System.Windows.Forms.Label();
            this._lObjectType = new System.Windows.Forms.Label();
            this._bClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._dgRootReferencedBy)).BeginInit();
            this.SuspendLayout();
            // 
            // _bRefresh
            // 
            this._bRefresh.Location = new System.Drawing.Point(3, 3);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 0;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _tbFilter
            // 
            this._tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._tbFilter.Location = new System.Drawing.Point(3, 346);
            this._tbFilter.Name = "_tbFilter";
            this._tbFilter.Size = new System.Drawing.Size(160, 20);
            this._tbFilter.TabIndex = 1;
            this._tbFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this._tbFilter_KeyDown);
            // 
            // _dgRootReferencedBy
            // 
            this._dgRootReferencedBy.AllowUserToAddRows = false;
            this._dgRootReferencedBy.AllowUserToDeleteRows = false;
            this._dgRootReferencedBy.AllowUserToResizeColumns = true;
            this._dgRootReferencedBy.AllowUserToResizeRows = false;
            this._dgRootReferencedBy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dgRootReferencedBy.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgRootReferencedBy.Location = new System.Drawing.Point(3, 32);
            this._dgRootReferencedBy.MultiSelect = false;
            this._dgRootReferencedBy.Name = "_dgRootReferencedBy";
            this._dgRootReferencedBy.ReadOnly = true;
            this._dgRootReferencedBy.RowHeadersVisible = false;
            this._dgRootReferencedBy.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgRootReferencedBy.Size = new System.Drawing.Size(594, 293);
            this._dgRootReferencedBy.TabIndex = 2;
            this._dgRootReferencedBy.TabStop = false;
            this._dgRootReferencedBy.DoubleClick += new System.EventHandler(this._dgRootReferencedBy_DoubleClick);
            // 
            // _bFilter
            // 
            this._bFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bFilter.Location = new System.Drawing.Point(335, 344);
            this._bFilter.Name = "_bFilter";
            this._bFilter.Size = new System.Drawing.Size(75, 23);
            this._bFilter.TabIndex = 3;
            this._bFilter.Text = "Filter";
            this._bFilter.UseVisualStyleBackColor = true;
            this._bFilter.Click += new System.EventHandler(this._bFilter_Click);
            // 
            // _icbObjectType
            // 
            this._icbObjectType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._icbObjectType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._icbObjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._icbObjectType.FormattingEnabled = true;
            this._icbObjectType.ImageList = null;
            this._icbObjectType.Items.AddRange(new object[] {
            ""});
            this._icbObjectType.Localizationhelper = null;
            this._icbObjectType.Location = new System.Drawing.Point(169, 346);
            this._icbObjectType.Name = "_icbObjectType";
            this._icbObjectType.SelectedItemObject = null;
            this._icbObjectType.SelectedItemObjectType = null;
            this._icbObjectType.Size = new System.Drawing.Size(160, 21);
            this._icbObjectType.TabIndex = 4;
            this._icbObjectType.TranslationPrefix = "ObjectType_";
            this._icbObjectType.SelectedValueChanged += new System.EventHandler(this._icbObjectType_SelectedValueChanged);
            // 
            // _lText
            // 
            this._lText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lText.AutoSize = true;
            this._lText.Location = new System.Drawing.Point(3, 329);
            this._lText.Name = "_lText";
            this._lText.Size = new System.Drawing.Size(28, 13);
            this._lText.TabIndex = 5;
            this._lText.Text = "Text";
            // 
            // _lObjectType
            // 
            this._lObjectType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lObjectType.AutoSize = true;
            this._lObjectType.Location = new System.Drawing.Point(166, 329);
            this._lObjectType.Name = "_lObjectType";
            this._lObjectType.Size = new System.Drawing.Size(61, 13);
            this._lObjectType.TabIndex = 6;
            this._lObjectType.Text = "Object type";
            // 
            // _bClear
            // 
            this._bClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bClear.Location = new System.Drawing.Point(416, 344);
            this._bClear.Name = "_bClear";
            this._bClear.Size = new System.Drawing.Size(75, 23);
            this._bClear.TabIndex = 7;
            this._bClear.Text = "Clear";
            this._bClear.UseVisualStyleBackColor = true;
            this._bClear.Click += new System.EventHandler(this._bClear_Click);
            // 
            // ReferencedByForm
            // 
            this.Controls.Add(this._bClear);
            this.Controls.Add(this._lObjectType);
            this.Controls.Add(this._lText);
            this.Controls.Add(this._icbObjectType);
            this.Controls.Add(this._bFilter);
            this.Controls.Add(this._dgRootReferencedBy);
            this.Controls.Add(this._tbFilter);
            this.Controls.Add(this._bRefresh);
            this.Name = "ReferencedByForm";
            this.Size = new System.Drawing.Size(600, 381);
            this.Load += new System.EventHandler(this.ReferencedByForm_Load);
            this.Disposed += new System.EventHandler(ReferencedByForm_Disposed);
            ((System.ComponentModel.ISupportInitialize)(this._dgRootReferencedBy)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bRefresh;
        private System.Windows.Forms.TextBox _tbFilter;
        private System.Windows.Forms.DataGridView _dgRootReferencedBy;
        private System.Windows.Forms.Button _bFilter;
        private Contal.IwQuick.UI.ImageComboBox _icbObjectType;
        private System.Windows.Forms.Label _lText;
        private System.Windows.Forms.Label _lObjectType;
        private System.Windows.Forms.Button _bClear;
    }
}