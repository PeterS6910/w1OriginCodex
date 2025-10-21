namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    partial class ChooseLanguage
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
            this._dgwLanguage = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this._btSave = new System.Windows.Forms.ToolStripMenuItem();
            this._btCancel = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this._dgwLanguage)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgwLanguage
            // 
            this._dgwLanguage.AllowUserToAddRows = false;
            this._dgwLanguage.AllowUserToDeleteRows = false;
            this._dgwLanguage.AllowUserToResizeColumns = true;
            this._dgwLanguage.AllowUserToResizeRows = false;
            this._dgwLanguage.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgwLanguage.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this._dgwLanguage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgwLanguage.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this._dgwLanguage.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this._dgwLanguage.Location = new System.Drawing.Point(0, 24);
            this._dgwLanguage.MultiSelect = false;
            this._dgwLanguage.Name = "_dgwLanguage";
            this._dgwLanguage.RowHeadersVisible = false;
            this._dgwLanguage.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgwLanguage.Size = new System.Drawing.Size(361, 277);
            this._dgwLanguage.TabIndex = 15;
            this._dgwLanguage.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dgwLanguage_CellClick);
            this._dgwLanguage.SelectionChanged += new System.EventHandler(this._dgwLanguage_SelectionChanged);
            // 
            // Column1
            // 
            this.Column1.FillWeight = 159.3909F;
            this.Column1.HeaderText = "Language";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.FillWeight = 40.60914F;
            this.Column2.HeaderText = System.String.Empty;
            this.Column2.Name = "Column2";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btSave,
            this._btCancel});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(361, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // _btSave
            // 
            this._btSave.Name = "_btSave";
            this._btSave.Size = new System.Drawing.Size(43, 20);
            this._btSave.Text = "Save";
            this._btSave.Click += new System.EventHandler(this._btSave_Click);
            // 
            // _btCancel
            // 
            this._btCancel.Name = "_btCancel";
            this._btCancel.Size = new System.Drawing.Size(51, 20);
            this._btCancel.Text = "Cancel";
            this._btCancel.Click += new System.EventHandler(this._btCancel_Click);
            // 
            // ChooseLanguage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 301);
            this.ControlBox = false;
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this._dgwLanguage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ChooseLanguage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose master language";
            this.Load += new System.EventHandler(this.frm_ChooseCustomer_Load);
            ((System.ComponentModel.ISupportInitialize)(this._dgwLanguage)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgwLanguage;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem _btSave;
        private System.Windows.Forms.ToolStripMenuItem _btCancel;
    }
}