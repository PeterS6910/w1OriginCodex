using Contal.Cgp.NCAS.WpfGraphicsControl;

namespace Contal.Cgp.NCAS.Client
{
    partial class NCASSceneEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASSceneEditForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.elementHost = new System.Windows.Forms.Integration.ElementHost();
            this._bImportGS = new System.Windows.Forms.Button();
            this.graphicsControl = new GraphicsScene(false);
            this._panelAlarms = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._bHideOrShow = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // elementHost
            // 
            this.elementHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost.Location = new System.Drawing.Point(0, 0);
            this.elementHost.Name = "elementHost";
            this.elementHost.Size = new System.Drawing.Size(1140, 707);
            this.elementHost.TabIndex = 4;
            this.elementHost.Text = "elementHost";
            this.elementHost.Child = this.graphicsControl;
            // 
            // _bImportGS
            // 
            this._bImportGS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bImportGS.Location = new System.Drawing.Point(998, 12);
            this._bImportGS.Name = "_bImportGS";
            this._bImportGS.Size = new System.Drawing.Size(146, 28);
            this._bImportGS.TabIndex = 5;
            this._bImportGS.Text = "Import Graphic Symbol";
            this._bImportGS.UseVisualStyleBackColor = true;
            this._bImportGS.Visible = false;
            this._bImportGS.Click += new System.EventHandler(this._bImportGS_Click);
            // 
            // _panelAlarms
            // 
            this._panelAlarms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._panelAlarms.Location = new System.Drawing.Point(0, 0);
            this._panelAlarms.Name = "_panelAlarms";
            this._panelAlarms.Size = new System.Drawing.Size(1140, 405);
            this._panelAlarms.TabIndex = 6;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.splitContainer1.Location = new System.Drawing.Point(2, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._bHideOrShow);
            this.splitContainer1.Panel1.Controls.Add(this.elementHost);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._panelAlarms);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.Size = new System.Drawing.Size(1141, 710);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 7;
            // 
            // _bHideOrShow
            // 
            this._bHideOrShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._bHideOrShow.FlatAppearance.BorderSize = 0;
            this._bHideOrShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._bHideOrShow.Image = global::Contal.Cgp.NCAS.Client.ResourceGlobal.hide12;
            this._bHideOrShow.Location = new System.Drawing.Point(-2, 699);
            this._bHideOrShow.Name = "_bHideOrShow";
            this._bHideOrShow.Size = new System.Drawing.Size(1141, 8);
            this._bHideOrShow.TabIndex = 81;
            this._bHideOrShow.UseVisualStyleBackColor = true;
            this._bHideOrShow.Click += new System.EventHandler(this._bHideOrShow_Click);
            // 
            // NCASSceneEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1146, 715);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this._bImportGS);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1084, 668);
            this.Name = "NCASSceneEditForm";
            this.Text = "Scene:";
            this.Load += new System.EventHandler(this.NCASSceneEditForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NCASSceneEditForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Integration.ElementHost elementHost;
        private Contal.Cgp.NCAS.WpfGraphicsControl.GraphicsScene graphicsControl;
        private System.Windows.Forms.Button _bImportGS;
        private System.Windows.Forms.Panel _panelAlarms;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button _bHideOrShow;
    }
}
