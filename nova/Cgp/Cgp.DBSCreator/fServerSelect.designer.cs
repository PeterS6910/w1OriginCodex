namespace Contal.Cgp.DBSCreator
{
    partial class fServerSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fServerSelect));
            this._lServerInfo = new System.Windows.Forms.Label();
            this._cbSqlServers = new System.Windows.Forms.ComboBox();
            this._bNext = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._bObtainDatabases = new System.Windows.Forms.Button();
            this._lMainInfo = new System.Windows.Forms.Label();
            this._lProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _lServerInfo
            // 
            this._lServerInfo.AutoSize = true;
            this._lServerInfo.Location = new System.Drawing.Point(12, 65);
            this._lServerInfo.Name = "_lServerInfo";
            this._lServerInfo.Size = new System.Drawing.Size(193, 13);
            this._lServerInfo.TabIndex = 1;
            this._lServerInfo.Text = "SQL Server pre Konfiguračnú databázu";
            // 
            // _cbSqlServers
            // 
            this._cbSqlServers.FormattingEnabled = true;
            this._cbSqlServers.Location = new System.Drawing.Point(12, 81);
            this._cbSqlServers.Name = "_cbSqlServers";
            this._cbSqlServers.Size = new System.Drawing.Size(262, 21);
            this._cbSqlServers.TabIndex = 2;
            this._cbSqlServers.DropDown += new System.EventHandler(this._cbSqlServers_DropDown);
            // 
            // _bNext
            // 
            this._bNext.Location = new System.Drawing.Point(221, 222);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(75, 23);
            this._bNext.TabIndex = 5;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(323, 222);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 6;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bObtainDatabases
            // 
            this._bObtainDatabases.Image = ((System.Drawing.Image)(resources.GetObject("_bObtainDatabases.Image")));
            this._bObtainDatabases.Location = new System.Drawing.Point(280, 72);
            this._bObtainDatabases.Name = "_bObtainDatabases";
            this._bObtainDatabases.Size = new System.Drawing.Size(37, 37);
            this._bObtainDatabases.TabIndex = 3;
            this._bObtainDatabases.UseVisualStyleBackColor = true;
            this._bObtainDatabases.Click += new System.EventHandler(this._bObtainDatabases_Click);
            // 
            // _lMainInfo
            // 
            this._lMainInfo.AutoSize = true;
            this._lMainInfo.Location = new System.Drawing.Point(12, 18);
            this._lMainInfo.Name = "_lMainInfo";
            this._lMainInfo.Size = new System.Drawing.Size(199, 13);
            this._lMainInfo.TabIndex = 0;
            this._lMainInfo.Text = "Prosím zadajte SQL Server pre databázy";
            // 
            // _lProgress
            // 
            this._lProgress.AutoSize = true;
            this._lProgress.Location = new System.Drawing.Point(12, 118);
            this._lProgress.Name = "_lProgress";
            this._lProgress.Size = new System.Drawing.Size(137, 13);
            this._lProgress.TabIndex = 4;
            this._lProgress.Text = "Vyhľadávam SQL Servre ...";
            // 
            // fServerSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 259);
            this.ControlBox = false;
            this.Controls.Add(this._lProgress);
            this.Controls.Add(this._lMainInfo);
            this.Controls.Add(this._bObtainDatabases);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._cbSqlServers);
            this.Controls.Add(this._lServerInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "fServerSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nastavenie SQL Servera";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lServerInfo;
        private System.Windows.Forms.ComboBox _cbSqlServers;
        private System.Windows.Forms.Button _bNext;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bObtainDatabases;
        private System.Windows.Forms.Label _lMainInfo;
        private System.Windows.Forms.Label _lProgress;
    }
}