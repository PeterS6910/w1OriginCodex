namespace Contal.Cgp.DBSCreator
{
    partial class fServerLogin
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._eSqlServer = new System.Windows.Forms.TextBox();
            this._eName = new System.Windows.Forms.TextBox();
            this._ePassword = new System.Windows.Forms.TextBox();
            this._bBack = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Prihlásenie do SQL Servera:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Prihlasovacie meno:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Heslo:";
            // 
            // _eSqlServer
            // 
            this._eSqlServer.Enabled = false;
            this._eSqlServer.Location = new System.Drawing.Point(158, 65);
            this._eSqlServer.Name = "_eSqlServer";
            this._eSqlServer.Size = new System.Drawing.Size(183, 20);
            this._eSqlServer.TabIndex = 3;
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(158, 91);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(183, 20);
            this._eName.TabIndex = 4;
            // 
            // _ePassword
            // 
            this._ePassword.Location = new System.Drawing.Point(158, 117);
            this._ePassword.Name = "_ePassword";
            this._ePassword.PasswordChar = '*';
            this._ePassword.Size = new System.Drawing.Size(183, 20);
            this._ePassword.TabIndex = 5;
            // 
            // _bBack
            // 
            this._bBack.Location = new System.Drawing.Point(140, 222);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(75, 23);
            this._bBack.TabIndex = 6;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _bNext
            // 
            this._bNext.Location = new System.Drawing.Point(221, 222);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(75, 23);
            this._bNext.TabIndex = 7;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(323, 222);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 8;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Nastavenie používateľa databázy";
            // 
            // fServerLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 257);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._ePassword);
            this.Controls.Add(this._eName);
            this.Controls.Add(this._eSqlServer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "fServerLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nastavenie SQL Servera";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _eSqlServer;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.TextBox _ePassword;
        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label label4;
    }
}