namespace Contal.Cgp.DBSCreator
{
    partial class FormDbsConversion
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
            this._pb = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this._lConversionTime = new System.Windows.Forms.Label();
            this._bContinue = new System.Windows.Forms.Button();
            this._eInfo = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pb
            // 
            this._pb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pb.Location = new System.Drawing.Point(0, 629);
            this._pb.Margin = new System.Windows.Forms.Padding(4);
            this._pb.Name = "_pb";
            this._pb.Size = new System.Drawing.Size(565, 29);
            this._pb.TabIndex = 0;
            this._pb.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._lConversionTime);
            this.panel1.Controls.Add(this._bContinue);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(565, 59);
            this.panel1.TabIndex = 1;
            // 
            // _lConversionTime
            // 
            this._lConversionTime.AutoSize = true;
            this._lConversionTime.Location = new System.Drawing.Point(15, 21);
            this._lConversionTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lConversionTime.Name = "_lConversionTime";
            this._lConversionTime.Size = new System.Drawing.Size(64, 17);
            this._lConversionTime.TabIndex = 2;
            this._lConversionTime.Text = "00:00:00";
            // 
            // _bContinue
            // 
            this._bContinue.Enabled = false;
            this._bContinue.Location = new System.Drawing.Point(456, 15);
            this._bContinue.Margin = new System.Windows.Forms.Padding(4);
            this._bContinue.Name = "_bContinue";
            this._bContinue.Size = new System.Drawing.Size(94, 29);
            this._bContinue.TabIndex = 1;
            this._bContinue.Text = "Continue";
            this._bContinue.UseVisualStyleBackColor = true;
            this._bContinue.Click += new System.EventHandler(this._bContinue_Click);
            // 
            // _eInfo
            // 
            this._eInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eInfo.Location = new System.Drawing.Point(0, 59);
            this._eInfo.Margin = new System.Windows.Forms.Padding(4);
            this._eInfo.Multiline = true;
            this._eInfo.Name = "_eInfo";
            this._eInfo.ReadOnly = true;
            this._eInfo.Size = new System.Drawing.Size(565, 570);
            this._eInfo.TabIndex = 2;
            // 
            // FormDbsConversion
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(565, 658);
            this.Controls.Add(this._eInfo);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._pb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormDbsConversion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DbsConversion";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormDbsConversion_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar _pb;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _bContinue;
        private System.Windows.Forms.TextBox _eInfo;
        private System.Windows.Forms.Label _lConversionTime;
    }
}