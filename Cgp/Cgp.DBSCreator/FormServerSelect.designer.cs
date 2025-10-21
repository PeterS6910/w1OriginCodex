namespace Contal.Cgp.DBSCreator
{
    partial class FormServerSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormServerSelect));
            this._lServerInfo = new System.Windows.Forms.Label();
            this._cbSqlServers = new System.Windows.Forms.ComboBox();
            this._bNext = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._bObtainDatabases = new System.Windows.Forms.Button();
            this._lMainInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _lServerInfo
            // 
            this._lServerInfo.AutoSize = true;
            this._lServerInfo.Location = new System.Drawing.Point(12, 34);
            this._lServerInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lServerInfo.Name = "_lServerInfo";
            this._lServerInfo.Size = new System.Drawing.Size(124, 13);
            this._lServerInfo.TabIndex = 1;
            this._lServerInfo.Text = "SQL Server for database";
            // 
            // _cbSqlServers
            // 
            this._cbSqlServers.FormattingEnabled = true;
            this._cbSqlServers.Location = new System.Drawing.Point(12, 54);
            this._cbSqlServers.Margin = new System.Windows.Forms.Padding(4);
            this._cbSqlServers.Name = "_cbSqlServers";
            this._cbSqlServers.Size = new System.Drawing.Size(326, 21);
            this._cbSqlServers.TabIndex = 1;
            this._cbSqlServers.SelectionChangeCommitted += new System.EventHandler(this._cbSqlServers_SelectionChangeCommitted);
            this._cbSqlServers.DropDown += new System.EventHandler(this._cbSqlServers_DropDown);
            // 
            // _bNext
            // 
            this._bNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bNext.Location = new System.Drawing.Point(274, 226);
            this._bNext.Margin = new System.Windows.Forms.Padding(4);
            this._bNext.Name = "_bNext";
            this._bNext.Size = new System.Drawing.Size(94, 29);
            this._bNext.TabIndex = 0;
            this._bNext.Text = "Next >";
            this._bNext.UseVisualStyleBackColor = true;
            this._bNext.Click += new System.EventHandler(this._bNext_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(406, 226);
            this._bCancel.Margin = new System.Windows.Forms.Padding(4);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(94, 29);
            this._bCancel.TabIndex = 2;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bObtainDatabases
            // 
            this._bObtainDatabases.Image = ((System.Drawing.Image)(resources.GetObject("_bObtainDatabases.Image")));
            this._bObtainDatabases.Location = new System.Drawing.Point(348, 42);
            this._bObtainDatabases.Margin = new System.Windows.Forms.Padding(4);
            this._bObtainDatabases.Name = "_bObtainDatabases";
            this._bObtainDatabases.Size = new System.Drawing.Size(46, 46);
            this._bObtainDatabases.TabIndex = 3;
            this._bObtainDatabases.UseVisualStyleBackColor = true;
            this._bObtainDatabases.Click += new System.EventHandler(this._bObtainDatabases_Click);
            // 
            // _lMainInfo
            // 
            this._lMainInfo.AutoSize = true;
            this._lMainInfo.Location = new System.Drawing.Point(12, 11);
            this._lMainInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lMainInfo.Name = "_lMainInfo";
            this._lMainInfo.Size = new System.Drawing.Size(95, 13);
            this._lMainInfo.TabIndex = 0;
            this._lMainInfo.Text = "Select SQL Server";
            // 
            // FormServerSelect
            // 
            this.AcceptButton = this._bNext;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(515, 270);
            this.ControlBox = false;
            this.Controls.Add(this._lMainInfo);
            this.Controls.Add(this._bObtainDatabases);
            this.Controls.Add(this._bCancel);
            this.Controls.Add(this._bNext);
            this.Controls.Add(this._cbSqlServers);
            this.Controls.Add(this._lServerInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormServerSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Server settings";
            this.Shown += new System.EventHandler(this.FormServerSelect_Shown);
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
    }
}