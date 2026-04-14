namespace Contal.IwQuick.Localization.LocalizationAssistant
{
    partial class Options
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.m_tabDefaultPaths = new System.Windows.Forms.TabPage();
            this._btMinus = new System.Windows.Forms.Button();
            this._lbGenerateCSharpPaths = new System.Windows.Forms.ListBox();
            this._btPlus = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this._btOkPaths = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.m_tabDefaultPaths.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.tabControl1.Controls.Add(this.m_tabDefaultPaths);
            this.tabControl1.Location = new System.Drawing.Point(0, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(503, 137);
            this.tabControl1.TabIndex = 0;
            // 
            // m_tabDefaultPaths
            // 
            this.m_tabDefaultPaths.Controls.Add(this._btMinus);
            this.m_tabDefaultPaths.Controls.Add(this._lbGenerateCSharpPaths);
            this.m_tabDefaultPaths.Controls.Add(this._btPlus);
            this.m_tabDefaultPaths.Controls.Add(this.label1);
            this.m_tabDefaultPaths.Location = new System.Drawing.Point(4, 22);
            this.m_tabDefaultPaths.Name = "m_tabDefaultPaths";
            this.m_tabDefaultPaths.Padding = new System.Windows.Forms.Padding(3);
            this.m_tabDefaultPaths.Size = new System.Drawing.Size(495, 111);
            this.m_tabDefaultPaths.TabIndex = 1;
            this.m_tabDefaultPaths.Text = "Paths";
            this.m_tabDefaultPaths.UseVisualStyleBackColor = true;
            // 
            // _btMinus
            // 
            this._btMinus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btMinus.Location = new System.Drawing.Point(413, 3);
            this._btMinus.Name = "_btMinus";
            this._btMinus.Size = new System.Drawing.Size(32, 21);
            this._btMinus.TabIndex = 9;
            this._btMinus.Text = "-";
            this._btMinus.UseVisualStyleBackColor = true;
            this._btMinus.Click += new System.EventHandler(this.m_btMinus_Click);
            // 
            // _lbGenerateCSharpPaths
            // 
            this._lbGenerateCSharpPaths.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbGenerateCSharpPaths.FormattingEnabled = true;
            this._lbGenerateCSharpPaths.Location = new System.Drawing.Point(8, 25);
            this._lbGenerateCSharpPaths.Name = "_lbGenerateCSharpPaths";
            this._lbGenerateCSharpPaths.Size = new System.Drawing.Size(483, 82);
            this._lbGenerateCSharpPaths.TabIndex = 6;
            // 
            // _btPlus
            // 
            this._btPlus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btPlus.Location = new System.Drawing.Point(451, 3);
            this._btPlus.Name = "_btPlus";
            this._btPlus.Size = new System.Drawing.Size(32, 21);
            this._btPlus.TabIndex = 5;
            this._btPlus.Text = "+";
            this._btPlus.UseVisualStyleBackColor = true;
            this._btPlus.Click += new System.EventHandler(this.BrowseCSharp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "C# generate paths";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btOkPaths});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(503, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // _btOkPaths
            // 
            this._btOkPaths.Name = "_btOkPaths";
            this._btOkPaths.Size = new System.Drawing.Size(32, 20);
            this._btOkPaths.Text = "Ok";
            this._btOkPaths.Click += new System.EventHandler(this._btOkPaths_Click);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 138);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.Shown += new System.EventHandler(this.frmOptions_Shown);
            this.tabControl1.ResumeLayout(false);
            this.m_tabDefaultPaths.ResumeLayout(false);
            this.m_tabDefaultPaths.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage m_tabDefaultPaths;
        private System.Windows.Forms.Button _btPlus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox _lbGenerateCSharpPaths;
        private System.Windows.Forms.Button _btMinus;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem _btOkPaths;
    }
}