namespace Contal.Cgp.Client
{
    partial class EditPersonsForm
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
            this._lDate = new System.Windows.Forms.Label();
            this._lNumber = new System.Windows.Forms.Label();
            this._lSurname = new System.Windows.Forms.Label();
            this._lName = new System.Windows.Forms.Label();
            this._eNumber = new System.Windows.Forms.TextBox();
            this._eSurname = new System.Windows.Forms.TextBox();
            this._eName = new System.Windows.Forms.TextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._eDate = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _lDate
            // 
            this._lDate.AutoSize = true;
            this._lDate.Location = new System.Drawing.Point(12, 86);
            this._lDate.Name = "_lDate";
            this._lDate.Size = new System.Drawing.Size(30, 13);
            this._lDate.TabIndex = 4;
            this._lDate.Text = "Date";
            // 
            // _lNumber
            // 
            this._lNumber.AutoSize = true;
            this._lNumber.Location = new System.Drawing.Point(12, 112);
            this._lNumber.Name = "_lNumber";
            this._lNumber.Size = new System.Drawing.Size(44, 13);
            this._lNumber.TabIndex = 6;
            this._lNumber.Text = "Number";
            // 
            // _lSurname
            // 
            this._lSurname.AutoSize = true;
            this._lSurname.Location = new System.Drawing.Point(12, 60);
            this._lSurname.Name = "_lSurname";
            this._lSurname.Size = new System.Drawing.Size(49, 13);
            this._lSurname.TabIndex = 2;
            this._lSurname.Text = "Surname";
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 34);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            // 
            // _eNumber
            // 
            this._eNumber.Location = new System.Drawing.Point(74, 109);
            this._eNumber.Name = "_eNumber";
            this._eNumber.Size = new System.Drawing.Size(164, 20);
            this._eNumber.TabIndex = 7;
            // 
            // _eSurname
            // 
            this._eSurname.Location = new System.Drawing.Point(74, 57);
            this._eSurname.Name = "_eSurname";
            this._eSurname.Size = new System.Drawing.Size(164, 20);
            this._eSurname.TabIndex = 3;
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(74, 31);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(164, 20);
            this._eName.TabIndex = 1;
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(163, 171);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 9;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Location = new System.Drawing.Point(15, 171);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 8;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _eDate
            // 
            this._eDate.Location = new System.Drawing.Point(74, 83);
            this._eDate.Name = "_eDate";
            this._eDate.Size = new System.Drawing.Size(164, 20);
            this._eDate.TabIndex = 5;
            // 
            // EditPersonsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 217);
            this.Controls.Add(this._eDate);
            this.Controls.Add(this._lDate);
            this.Controls.Add(this._lNumber);
            this.Controls.Add(this._lSurname);
            this.Controls.Add(this._lName);
            this.Controls.Add(this._eNumber);
            this.Controls.Add(this._eSurname);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EditPersonsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EditPersonsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lDate;
        private System.Windows.Forms.Label _lNumber;
        private System.Windows.Forms.Label _lSurname;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eNumber;
        private System.Windows.Forms.TextBox _eSurname;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TextBox _eDate;
    }
}