namespace Contal.Cgp.DBSCreator
{
    partial class fServerDatabase
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
            this._eSqlServer = new System.Windows.Forms.TextBox();
            this._cbDatabaseList = new System.Windows.Forms.ComboBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bNext = new System.Windows.Forms.Button();
            this._bBack = new System.Windows.Forms.Button();
            this._lProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQL Server:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Názov databázy:";
            // 
            // _eSqlServer
            // 
            this._eSqlServer.Enabled = false;
            this._eSqlServer.Location = new System.Drawing.Point(140, 58);
            this._eSqlServer.Name = "_eSqlServer";
            this._eSqlServer.Size = new System.Drawing.Size(191, 20);
            this._eSqlServer.TabIndex = 1;
            // 
            // _cbDatabaseList
            // 
            this._cbDatabaseList.FormattingEnabled = true;
            this._cbDatabaseList.Location = new System.Drawing.Point(140, 89);
            this._cbDatabaseList.Name = "_cbDatabaseList";
            this._cbDatabaseList.Size = new System.Drawing.Size(191, 21);
            this._cbDatabaseList.TabIndex = 3;
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(323, 222);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bNext
            // 
            this._bNext.Location = new System.Drawing.Point(221, 222);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(75, 23);
            this._bNext.TabIndex = 6;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bBack
            // 
            this._bBack.Location = new System.Drawing.Point(140, 222);
            this._bBack.Name = "_bBack";
            this._bBack.Size = new System.Drawing.Size(75, 23);
            this._bBack.TabIndex = 5;
            this._bBack.Text = "< Back";
            this._bBack.UseVisualStyleBackColor = true;
            this._bBack.Click += new System.EventHandler(this._bBack_Click);
            // 
            // _lProgress
            // 
            this._lProgress.AutoSize = true;
            this._lProgress.Location = new System.Drawing.Point(153, 113);
            this._lProgress.Name = "_lProgress";
            this._lProgress.Size = new System.Drawing.Size(125, 13);
            this._lProgress.TabIndex = 4;
            this._lProgress.Text = "Vyhľadávam databázy ...";
            // 
            // fServerDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 257);
            this.ControlBox = false;
            this.Controls.Add(this._lProgress);
            this.Controls.Add(this._bBack);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._cbDatabaseList);
            this.Controls.Add(this._eSqlServer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "fServerDatabase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nastavenie SQL Servera";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _eSqlServer;
        private System.Windows.Forms.ComboBox _cbDatabaseList;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bBack;
        private System.Windows.Forms.Label _lProgress;
    }
}