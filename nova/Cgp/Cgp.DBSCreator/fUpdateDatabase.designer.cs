namespace Contal.Cgp.DBSCreator
{
    partial class fUpdateDatabase
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
            this._bPerform = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._eUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._eDatabase = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._eSqlServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._rbProgress = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._bFixDbs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _bBack
            // 
            this._bBack.Location = new System.Drawing.Point(147, 217);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(75, 23);
            this._bBack.TabIndex = 23;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _bPerform
            // 
            this._bPerform.Location = new System.Drawing.Point(228, 217);
            this._bPerform.Name = "_bPerform";
            this._bPerform.Size = new System.Drawing.Size(75, 23);
            this._bPerform.TabIndex = 24;
            this._bPerform.Text = "Vykonaj";
            this._bPerform.UseVisualStyleBackColor = true;
            this._bPerform.Click += new System.EventHandler(this._bPerform_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(330, 217);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 25;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _eUser
            // 
            this._eUser.Enabled = false;
            this._eUser.Location = new System.Drawing.Point(143, 88);
            this._eUser.Name = "_eUser";
            this._eUser.Size = new System.Drawing.Size(184, 20);
            this._eUser.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Užívateľ:";
            // 
            // _eDatabase
            // 
            this._eDatabase.Enabled = false;
            this._eDatabase.Location = new System.Drawing.Point(143, 62);
            this._eDatabase.Name = "_eDatabase";
            this._eDatabase.Size = new System.Drawing.Size(184, 20);
            this._eDatabase.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Databáza:";
            // 
            // _eSqlServer
            // 
            this._eSqlServer.Enabled = false;
            this._eSqlServer.Location = new System.Drawing.Point(143, 36);
            this._eSqlServer.Name = "_eSqlServer";
            this._eSqlServer.Size = new System.Drawing.Size(184, 20);
            this._eSqlServer.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "SQL Server:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Príprava na vytvorenie";
            // 
            // _rbProgress
            // 
            this._rbProgress.Location = new System.Drawing.Point(143, 114);
            this._rbProgress.Name = "_rbProgress";
            this._rbProgress.Size = new System.Drawing.Size(262, 97);
            this._rbProgress.TabIndex = 27;
            this._rbProgress.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Progres:";
            // 
            // _bFixDbs
            // 
            this._bFixDbs.Location = new System.Drawing.Point(66, 217);
            this._bFixDbs.Name = "_bFixDbs";
            this._bFixDbs.Size = new System.Drawing.Size(75, 23);
            this._bFixDbs.TabIndex = 28;
            this._bFixDbs.Text = "Oprav DBS";
            this._bFixDbs.UseVisualStyleBackColor = true;
            this._bFixDbs.Click += new System.EventHandler(this._bFixDbs_Click);
            // 
            // fUpdateDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 250);
            this.ControlBox = false;
            this.Controls.Add(this._bFixDbs);
            this.Controls.Add(this._rbProgress);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._bPerform);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._eUser);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._eDatabase);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._eSqlServer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "fUpdateDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update databázy";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Button _bPerform;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.TextBox _eUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _eDatabase;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _eSqlServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox _rbProgress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button _bFixDbs;
    }
}