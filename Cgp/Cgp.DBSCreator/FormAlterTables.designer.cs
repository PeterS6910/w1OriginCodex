namespace Contal.Cgp.DBSCreator
{
    internal partial class FormAlterTables
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
            this._rbInfo = new System.Windows.Forms.RichTextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bRunAlter = new System.Windows.Forms.Button();
            this._bListErrors = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._cbErrors = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _rbInfo
            // 
            this._rbInfo.Location = new System.Drawing.Point(15, 94);
            this._rbInfo.Margin = new System.Windows.Forms.Padding(4);
            this._rbInfo.Name = "_rbInfo";
            this._rbInfo.ReadOnly = true;
            this._rbInfo.Size = new System.Drawing.Size(370, 209);
            this._rbInfo.TabIndex = 2;
            this._rbInfo.Text = "";
            // 
            // _bCancel
            // 
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(292, 311);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 29);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bRunAlter
            // 
            this._bRunAlter.Location = new System.Drawing.Point(191, 311);
            this._bRunAlter.Margin = new System.Windows.Forms.Padding(4);
            this._bRunAlter.Name = "_bRunAlter";
            this._bRunAlter.Size = new System.Drawing.Size(94, 29);
            this._bRunAlter.TabIndex = 1;
            this._bRunAlter.Text = "Fix";
            this._bRunAlter.UseVisualStyleBackColor = true;
            this._bRunAlter.Click += new System.EventHandler(this._bRunAlter_Click);
            // 
            // _bListErrors
            // 
            this._bListErrors.Location = new System.Drawing.Point(90, 311);
            this._bListErrors.Margin = new System.Windows.Forms.Padding(4);
            this._bListErrors.Name = "_bListErrors";
            this._bListErrors.Size = new System.Drawing.Size(94, 29);
            this._bListErrors.TabIndex = 0;
            this._bListErrors.Text = "Show";
            this._bListErrors.UseVisualStyleBackColor = true;
            this._bListErrors.Click += new System.EventHandler(this._bListErrors_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Error type";
            // 
            // _cbErrors
            // 
            this._cbErrors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbErrors.FormattingEnabled = true;
            this._cbErrors.Items.AddRange(new object[] {
            "Tables not in SQL database",
            "Tables with miss columns",
            "Tables with wrong columns",
            "Tables with useless columns"});
            this._cbErrors.Location = new System.Drawing.Point(15, 31);
            this._cbErrors.Margin = new System.Windows.Forms.Padding(4);
            this._cbErrors.Name = "_cbErrors";
            this._cbErrors.Size = new System.Drawing.Size(370, 24);
            this._cbErrors.TabIndex = 5;
            this._cbErrors.SelectedIndexChanged += new System.EventHandler(this._cbErrors_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Information";
            // 
            // FormAlterTables
            // 
            this.AcceptButton = this._bRunAlter;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(401, 362);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this._cbErrors);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._bListErrors);
            this.Controls.Add(this._bRunAlter);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._rbInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormAlterTables";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox _rbInfo;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bRunAlter;
        private System.Windows.Forms.Button _bListErrors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _cbErrors;
        private System.Windows.Forms.Label label2;
    }
}