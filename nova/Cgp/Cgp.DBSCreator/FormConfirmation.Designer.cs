namespace Contal.Cgp.DBSCreator
{
    partial class FormConfirmation
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
            this._bBack = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _bBack
            // 
            this._bBack.Location = new System.Drawing.Point(187, 267);
            this._bBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(100, 28);
            this._bBack.TabIndex = 1;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _bNext
            // 
            this._bNext.Location = new System.Drawing.Point(295, 267);
            this._bNext.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(100, 28);
            this._bNext.TabIndex = 0;
            this._bNext.Text = "Finish";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(431, 267);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(100, 28);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(13, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "The configuration of SQL Servera is done.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(13, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(208, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Press Finish to create database";
            // 
            // FormConfirmation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 334);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormConfirmation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Server settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}