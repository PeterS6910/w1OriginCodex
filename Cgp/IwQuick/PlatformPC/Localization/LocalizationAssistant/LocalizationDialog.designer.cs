namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    partial class LocalizationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalizationDialog));
            this._cbLanguage = new System.Windows.Forms.ComboBox();
            this._chbMasterResource = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._btOk = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this._btCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _cbLanguage
            // 
            this._cbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbLanguage.FormattingEnabled = true;
            this._cbLanguage.Items.AddRange(new object[] {
            "Czech",
            "English",
            "Germany",
            "Slovak",
            "Finnish",
            "Swedish",
            "Norwegian",
            "French",
            "Basque",
            "Italian",
            "Dutch"});
            this._cbLanguage.Location = new System.Drawing.Point(136, 31);
            this._cbLanguage.Name = "_cbLanguage";
            this._cbLanguage.Size = new System.Drawing.Size(203, 21);
            this._cbLanguage.TabIndex = 6;
            this._cbLanguage.TextChanged += new System.EventHandler(this._cbLanguage_TextChanged);
            // 
            // _chbMasterResource
            // 
            this._chbMasterResource.AutoSize = true;
            this._chbMasterResource.Location = new System.Drawing.Point(345, 33);
            this._chbMasterResource.Name = "_chbMasterResource";
            this._chbMasterResource.Size = new System.Drawing.Size(102, 17);
            this._chbMasterResource.TabIndex = 7;
            this._chbMasterResource.Text = "Master resource";
            this._chbMasterResource.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Language abbreviation :";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btOk,
            this.toolStripLabel1,
            this._btCancel,
            this.toolStripLabel2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(452, 25);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // _btOk
            // 
            this._btOk.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._btOk.Image = ((System.Drawing.Image)(resources.GetObject("_btOk.Image")));
            this._btOk.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._btOk.Name = "_btOk";
            this._btOk.Size = new System.Drawing.Size(23, 22);
            this._btOk.Click += new System.EventHandler(this._btOk_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel1.Text = "Ok";
            // 
            // _btCancel
            // 
            this._btCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._btCancel.Image = ((System.Drawing.Image)(resources.GetObject("_btCancel.Image")));
            this._btCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._btCancel.Name = "_btCancel";
            this._btCancel.Size = new System.Drawing.Size(23, 22);
            this._btCancel.Text = "toolStripButton2";
            this._btCancel.Click += new System.EventHandler(this._btCancel_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(39, 22);
            this.toolStripLabel2.Text = "Cancel";
            // 
            // LocalizationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 55);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this._cbLanguage);
            this.Controls.Add(this._chbMasterResource);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "LocalizationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LocalizationDialog";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _cbLanguage;
        private System.Windows.Forms.CheckBox _chbMasterResource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton _btOk;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton _btCancel;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
    }
}