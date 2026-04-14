namespace Contal.Cgp.NCAS.Client
{
    partial class SelectDCUHWTypeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectDCUHWTypeDialog));
            this._lQuestion = new System.Windows.Forms.Label();
            this._cbHWType = new System.Windows.Forms.ComboBox();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lQuestion
            // 
            this._lQuestion.AutoSize = true;
            this._lQuestion.Location = new System.Drawing.Point(12, 25);
            this._lQuestion.Name = "_lQuestion";
            this._lQuestion.Size = new System.Drawing.Size(308, 13);
            this._lQuestion.TabIndex = 0;
            this._lQuestion.Text = "DCU hardware type is not defined. Please select hardware type.";
            // 
            // _cbHWType
            // 
            this._cbHWType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbHWType.FormattingEnabled = true;
            this._cbHWType.Location = new System.Drawing.Point(15, 50);
            this._cbHWType.Name = "_cbHWType";
            this._cbHWType.Size = new System.Drawing.Size(120, 21);
            this._cbHWType.TabIndex = 1;
            // 
            // _bOk
            // 
            this._bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._bOk.Location = new System.Drawing.Point(107, 101);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(81, 25);
            this._bOk.TabIndex = 2;
            this._bOk.Text = "Ok";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(194, 101);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(81, 25);
            this._bCancel.TabIndex = 3;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            // 
            // SelectDCUHWTypeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 138);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.Controls.Add(this._cbHWType);
            this.Controls.Add(this._lQuestion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SelectDCUHWTypeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DCU hardware type";
            this.Load += new System.EventHandler(this.SelectDCUHWTypeDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lQuestion;
        private System.Windows.Forms.ComboBox _cbHWType;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
    }
}