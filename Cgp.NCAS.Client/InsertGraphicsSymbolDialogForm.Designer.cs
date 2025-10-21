namespace Contal.Cgp.NCAS.Client
{
    partial class InsertGraphicsSymbolDialogForm
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
            this._lSelectSymbolType = new System.Windows.Forms.Label();
            this._cbSymbolType = new System.Windows.Forms.ComboBox();
            this._lFilterKey = new System.Windows.Forms.Label();
            this._lSelectFile = new System.Windows.Forms.Label();
            this._tbFileName = new System.Windows.Forms.TextBox();
            this._bSelectFile = new System.Windows.Forms.Button();
            this._bInsert = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._lTemplateName = new System.Windows.Forms.Label();
            this._tbTemplateName = new System.Windows.Forms.TextBox();
            this._gbCreateTemplate = new System.Windows.Forms.GroupBox();
            this._bClear = new System.Windows.Forms.Button();
            this._tbTemplateDescription = new System.Windows.Forms.TextBox();
            this._lTemplateDescription = new System.Windows.Forms.Label();
            this._bInsertTemplate = new System.Windows.Forms.Button();
            this._lSelectTemplate = new System.Windows.Forms.Label();
            this._cbSelectTemplate = new System.Windows.Forms.ComboBox();
            this._bSelectExistingImage = new System.Windows.Forms.Button();
            this._cbFilterKeys = new System.Windows.Forms.ComboBox();
            this._chbUseFilterKey = new System.Windows.Forms.CheckBox();
            this._lSymbolState = new System.Windows.Forms.Label();
            this._cbSymbolState = new System.Windows.Forms.ComboBox();
            this._gbCreateTemplate.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lSelectSymbolType
            // 
            this._lSelectSymbolType.AutoSize = true;
            this._lSelectSymbolType.Location = new System.Drawing.Point(9, 14);
            this._lSelectSymbolType.Name = "_lSelectSymbolType";
            this._lSelectSymbolType.Size = new System.Drawing.Size(95, 13);
            this._lSelectSymbolType.TabIndex = 0;
            this._lSelectSymbolType.Text = "Select symbol type";
            // 
            // _cbSymbolType
            // 
            this._cbSymbolType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSymbolType.FormattingEnabled = true;
            this._cbSymbolType.Location = new System.Drawing.Point(12, 30);
            this._cbSymbolType.Name = "_cbSymbolType";
            this._cbSymbolType.Size = new System.Drawing.Size(227, 21);
            this._cbSymbolType.TabIndex = 1;
            // 
            // _lFilterKey
            // 
            this._lFilterKey.AutoSize = true;
            this._lFilterKey.Location = new System.Drawing.Point(12, 229);
            this._lFilterKey.Name = "_lFilterKey";
            this._lFilterKey.Size = new System.Drawing.Size(49, 13);
            this._lFilterKey.TabIndex = 2;
            this._lFilterKey.Text = "Filter key";
            // 
            // _lSelectFile
            // 
            this._lSelectFile.AutoSize = true;
            this._lSelectFile.Location = new System.Drawing.Point(12, 277);
            this._lSelectFile.Name = "_lSelectFile";
            this._lSelectFile.Size = new System.Drawing.Size(53, 13);
            this._lSelectFile.TabIndex = 4;
            this._lSelectFile.Text = "Select file";
            // 
            // _tbFileName
            // 
            this._tbFileName.Location = new System.Drawing.Point(15, 293);
            this._tbFileName.Name = "_tbFileName";
            this._tbFileName.Size = new System.Drawing.Size(417, 20);
            this._tbFileName.TabIndex = 5;
            // 
            // _bSelectFile
            // 
            this._bSelectFile.Location = new System.Drawing.Point(438, 292);
            this._bSelectFile.Name = "_bSelectFile";
            this._bSelectFile.Size = new System.Drawing.Size(41, 21);
            this._bSelectFile.TabIndex = 6;
            this._bSelectFile.Text = "...";
            this._bSelectFile.UseVisualStyleBackColor = true;
            this._bSelectFile.Click += new System.EventHandler(this._bSelectFile_Click);
            // 
            // _bInsert
            // 
            this._bInsert.Location = new System.Drawing.Point(159, 360);
            this._bInsert.Name = "_bInsert";
            this._bInsert.Size = new System.Drawing.Size(80, 28);
            this._bInsert.TabIndex = 7;
            this._bInsert.Text = "Insert";
            this._bInsert.UseVisualStyleBackColor = true;
            this._bInsert.Click += new System.EventHandler(this._bInsert_Click);
            // 
            // _bCancel
            // 
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(245, 360);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(80, 28);
            this._bCancel.TabIndex = 8;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "\"Image Files|*.bmp; *.jpg; *.png; *.svg;\"";
            // 
            // _lTemplateName
            // 
            this._lTemplateName.AutoSize = true;
            this._lTemplateName.Location = new System.Drawing.Point(6, 22);
            this._lTemplateName.Name = "_lTemplateName";
            this._lTemplateName.Size = new System.Drawing.Size(80, 13);
            this._lTemplateName.TabIndex = 9;
            this._lTemplateName.Text = "Template name";
            // 
            // _tbTemplateName
            // 
            this._tbTemplateName.Location = new System.Drawing.Point(89, 19);
            this._tbTemplateName.Name = "_tbTemplateName";
            this._tbTemplateName.Size = new System.Drawing.Size(372, 20);
            this._tbTemplateName.TabIndex = 10;
            // 
            // _gbCreateTemplate
            // 
            this._gbCreateTemplate.Controls.Add(this._bClear);
            this._gbCreateTemplate.Controls.Add(this._tbTemplateDescription);
            this._gbCreateTemplate.Controls.Add(this._lTemplateDescription);
            this._gbCreateTemplate.Controls.Add(this._bInsertTemplate);
            this._gbCreateTemplate.Controls.Add(this._tbTemplateName);
            this._gbCreateTemplate.Controls.Add(this._lTemplateName);
            this._gbCreateTemplate.Location = new System.Drawing.Point(12, 119);
            this._gbCreateTemplate.Name = "_gbCreateTemplate";
            this._gbCreateTemplate.Size = new System.Drawing.Size(467, 107);
            this._gbCreateTemplate.TabIndex = 11;
            this._gbCreateTemplate.TabStop = false;
            this._gbCreateTemplate.Text = "Create new template";
            // 
            // _bClear
            // 
            this._bClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bClear.Location = new System.Drawing.Point(395, 81);
            this._bClear.Name = "_bClear";
            this._bClear.Size = new System.Drawing.Size(66, 20);
            this._bClear.TabIndex = 14;
            this._bClear.Text = "Clear";
            this._bClear.UseVisualStyleBackColor = true;
            this._bClear.Click += new System.EventHandler(this._bClear_Click);
            // 
            // _tbTemplateDescription
            // 
            this._tbTemplateDescription.Location = new System.Drawing.Point(89, 47);
            this._tbTemplateDescription.Name = "_tbTemplateDescription";
            this._tbTemplateDescription.Size = new System.Drawing.Size(372, 20);
            this._tbTemplateDescription.TabIndex = 14;
            // 
            // _lTemplateDescription
            // 
            this._lTemplateDescription.AutoSize = true;
            this._lTemplateDescription.Location = new System.Drawing.Point(6, 50);
            this._lTemplateDescription.Name = "_lTemplateDescription";
            this._lTemplateDescription.Size = new System.Drawing.Size(60, 13);
            this._lTemplateDescription.TabIndex = 13;
            this._lTemplateDescription.Text = "Description";
            // 
            // _bInsertTemplate
            // 
            this._bInsertTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bInsertTemplate.Location = new System.Drawing.Point(323, 81);
            this._bInsertTemplate.Name = "_bInsertTemplate";
            this._bInsertTemplate.Size = new System.Drawing.Size(66, 20);
            this._bInsertTemplate.TabIndex = 12;
            this._bInsertTemplate.Text = "Insert";
            this._bInsertTemplate.UseVisualStyleBackColor = true;
            this._bInsertTemplate.Click += new System.EventHandler(this._bInsertTemplate_Click);
            // 
            // _lSelectTemplate
            // 
            this._lSelectTemplate.AutoSize = true;
            this._lSelectTemplate.Location = new System.Drawing.Point(9, 64);
            this._lSelectTemplate.Name = "_lSelectTemplate";
            this._lSelectTemplate.Size = new System.Drawing.Size(80, 13);
            this._lSelectTemplate.TabIndex = 12;
            this._lSelectTemplate.Text = "Select template";
            // 
            // _cbSelectTemplate
            // 
            this._cbSelectTemplate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSelectTemplate.FormattingEnabled = true;
            this._cbSelectTemplate.Location = new System.Drawing.Point(12, 80);
            this._cbSelectTemplate.Name = "_cbSelectTemplate";
            this._cbSelectTemplate.Size = new System.Drawing.Size(467, 21);
            this._cbSelectTemplate.TabIndex = 13;
            // 
            // _bSelectExistingImage
            // 
            this._bSelectExistingImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._bSelectExistingImage.Location = new System.Drawing.Point(15, 319);
            this._bSelectExistingImage.Name = "_bSelectExistingImage";
            this._bSelectExistingImage.Size = new System.Drawing.Size(202, 22);
            this._bSelectExistingImage.TabIndex = 15;
            this._bSelectExistingImage.Text = "Select image from existing image";
            this._bSelectExistingImage.UseVisualStyleBackColor = true;
            this._bSelectExistingImage.Click += new System.EventHandler(this._bSelectExistingImage_Click);
            // 
            // _cbFilterKeys
            // 
            this._cbFilterKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbFilterKeys.FormattingEnabled = true;
            this._cbFilterKeys.Location = new System.Drawing.Point(15, 245);
            this._cbFilterKeys.Name = "_cbFilterKeys";
            this._cbFilterKeys.Size = new System.Drawing.Size(248, 21);
            this._cbFilterKeys.TabIndex = 16;
            // 
            // _chbUseFilterKey
            // 
            this._chbUseFilterKey.AutoSize = true;
            this._chbUseFilterKey.Checked = true;
            this._chbUseFilterKey.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chbUseFilterKey.Location = new System.Drawing.Point(280, 245);
            this._chbUseFilterKey.Name = "_chbUseFilterKey";
            this._chbUseFilterKey.Size = new System.Drawing.Size(94, 17);
            this._chbUseFilterKey.TabIndex = 17;
            this._chbUseFilterKey.Text = "Insert filter key";
            this._chbUseFilterKey.UseVisualStyleBackColor = true;
            this._chbUseFilterKey.CheckedChanged += new System.EventHandler(this._chbUseFilterKey_CheckedChanged);
            // 
            // _lSymbolState
            // 
            this._lSymbolState.AutoSize = true;
            this._lSymbolState.Location = new System.Drawing.Point(254, 14);
            this._lSymbolState.Name = "_lSymbolState";
            this._lSymbolState.Size = new System.Drawing.Size(98, 13);
            this._lSymbolState.TabIndex = 18;
            this._lSymbolState.Text = "Select symbol state";
            // 
            // _cbSymbolState
            // 
            this._cbSymbolState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbSymbolState.FormattingEnabled = true;
            this._cbSymbolState.Location = new System.Drawing.Point(257, 30);
            this._cbSymbolState.Name = "_cbSymbolState";
            this._cbSymbolState.Size = new System.Drawing.Size(222, 21);
            this._cbSymbolState.TabIndex = 19;
            // 
            // InsertGraphicsSymbolDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(487, 400);
            this.Controls.Add(this._cbSymbolState);
            this.Controls.Add(this._lSymbolState);
            this.Controls.Add(this._chbUseFilterKey);
            this.Controls.Add(this._cbFilterKeys);
            this.Controls.Add(this._bSelectExistingImage);
            this.Controls.Add(this._cbSelectTemplate);
            this.Controls.Add(this._lSelectTemplate);
            this.Controls.Add(this._gbCreateTemplate);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bInsert);
            this.Controls.Add(this._bSelectFile);
            this.Controls.Add(this._tbFileName);
            this.Controls.Add(this._lSelectFile);
            this.Controls.Add(this._lFilterKey);
            this.Controls.Add(this._cbSymbolType);
            this.Controls.Add(this._lSelectSymbolType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "InsertGraphicsSymbolDialogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Insert graphics symbol";
            this.Load += new System.EventHandler(this.InsertGraphicsSymbolDialogForm_Load);
            this._gbCreateTemplate.ResumeLayout(false);
            this._gbCreateTemplate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lSelectSymbolType;
        private System.Windows.Forms.ComboBox _cbSymbolType;
        private System.Windows.Forms.Label _lFilterKey;
        private System.Windows.Forms.Label _lSelectFile;
        private System.Windows.Forms.TextBox _tbFileName;
        private System.Windows.Forms.Button _bSelectFile;
        private System.Windows.Forms.Button _bInsert;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label _lTemplateName;
        private System.Windows.Forms.TextBox _tbTemplateName;
        private System.Windows.Forms.GroupBox _gbCreateTemplate;
        private System.Windows.Forms.Button _bClear;
        private System.Windows.Forms.TextBox _tbTemplateDescription;
        private System.Windows.Forms.Label _lTemplateDescription;
        private System.Windows.Forms.Button _bInsertTemplate;
        private System.Windows.Forms.Label _lSelectTemplate;
        private System.Windows.Forms.ComboBox _cbSelectTemplate;
        private System.Windows.Forms.Button _bSelectExistingImage;
        private System.Windows.Forms.ComboBox _cbFilterKeys;
        private System.Windows.Forms.CheckBox _chbUseFilterKey;
        private System.Windows.Forms.Label _lSymbolState;
        private System.Windows.Forms.ComboBox _cbSymbolState;
    }
}
