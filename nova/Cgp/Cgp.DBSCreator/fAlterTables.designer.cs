namespace Contal.Cgp.DBSCreator
{
    partial class fAlterTables
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
            this.SuspendLayout();
            // 
            // _rbInfo
            // 
            this._rbInfo.Location = new System.Drawing.Point(12, 12);
            this._rbInfo.Name = "_rbInfo";
            this._rbInfo.Size = new System.Drawing.Size(297, 216);
            this._rbInfo.TabIndex = 0;
            this._rbInfo.Text = "";
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(234, 234);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bRunAlter
            // 
            this._bRunAlter.Location = new System.Drawing.Point(153, 234);
            this._bRunAlter.Name = "_bRunAlter";
            this._bRunAlter.Size = new System.Drawing.Size(75, 23);
            this._bRunAlter.TabIndex = 1;
            this._bRunAlter.Text = "Oprav";
            this._bRunAlter.UseVisualStyleBackColor = true;
            this._bRunAlter.Click += new System.EventHandler(this._bRunAlter_Click);
            // 
            // _bListErrors
            // 
            this._bListErrors.Location = new System.Drawing.Point(72, 234);
            this._bListErrors.Name = "_bListErrors";
            this._bListErrors.Size = new System.Drawing.Size(75, 23);
            this._bListErrors.TabIndex = 3;
            this._bListErrors.Text = "Zobraz";
            this._bListErrors.UseVisualStyleBackColor = true;
            this._bListErrors.Click += new System.EventHandler(this._bListErrors_Click);
            // 
            // fAlterTables
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 271);
            this.ControlBox = false;
            this.Controls.Add(this._bListErrors);
            this.Controls.Add(this._bRunAlter);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._rbInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "fAlterTables";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Úprava tabuliek";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox _rbInfo;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bRunAlter;
        private System.Windows.Forms.Button _bListErrors;
    }
}