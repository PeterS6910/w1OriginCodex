namespace AnalyticTool.Advitech
{
    partial class SelectDatabaseAndTableForm
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
            this._cbDatabaseList = new System.Windows.Forms.ComboBox();
            this._lDatabaseName = new System.Windows.Forms.Label();
            this._bBack = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._cbTables = new System.Windows.Forms.ComboBox();
            this._lTableName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _cbDatabaseList
            // 
            this._cbDatabaseList.FormattingEnabled = true;
            this._cbDatabaseList.Location = new System.Drawing.Point(161, 13);
            this._cbDatabaseList.Margin = new System.Windows.Forms.Padding(4);
            this._cbDatabaseList.Name = "_cbDatabaseList";
            this._cbDatabaseList.Size = new System.Drawing.Size(238, 21);
            this._cbDatabaseList.TabIndex = 5;
            this._cbDatabaseList.SelectedIndexChanged += new System.EventHandler(this._cbDatabaseList_SelectedIndexChanged);
            // 
            // _lDatabaseName
            // 
            this._lDatabaseName.AutoSize = true;
            this._lDatabaseName.Enabled = false;
            this._lDatabaseName.Location = new System.Drawing.Point(13, 16);
            this._lDatabaseName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lDatabaseName.Name = "_lDatabaseName";
            this._lDatabaseName.Size = new System.Drawing.Size(82, 13);
            this._lDatabaseName.TabIndex = 4;
            this._lDatabaseName.Text = "Database name";
            // 
            // _bBack
            // 
            this._bBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bBack.Location = new System.Drawing.Point(203, 118);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(95, 26);
            this._bBack.TabIndex = 9;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _bNext
            // 
            this._bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bNext.Location = new System.Drawing.Point(304, 118);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(95, 26);
            this._bNext.TabIndex = 8;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _cbTables
            // 
            this._cbTables.FormattingEnabled = true;
            this._cbTables.Location = new System.Drawing.Point(161, 42);
            this._cbTables.Margin = new System.Windows.Forms.Padding(4);
            this._cbTables.Name = "_cbTables";
            this._cbTables.Size = new System.Drawing.Size(238, 21);
            this._cbTables.TabIndex = 11;
            // 
            // _lTableName
            // 
            this._lTableName.AutoSize = true;
            this._lTableName.Enabled = false;
            this._lTableName.Location = new System.Drawing.Point(13, 45);
            this._lTableName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lTableName.Name = "_lTableName";
            this._lTableName.Size = new System.Drawing.Size(63, 13);
            this._lTableName.TabIndex = 10;
            this._lTableName.Text = "Table name";
            // 
            // SelectDatabaseAndTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 156);
            this.Controls.Add(this._cbTables);
            this.Controls.Add(this._lTableName);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._cbDatabaseList);
            this.Controls.Add(this._lDatabaseName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SelectDatabaseAndTableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select database and table";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _cbDatabaseList;
        private System.Windows.Forms.Label _lDatabaseName;
        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.ComboBox _cbTables;
        private System.Windows.Forms.Label _lTableName;
    }
}