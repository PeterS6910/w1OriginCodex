namespace Contal.IwQuick.UI
{
    partial class ComponentList<T>
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._tlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // _tlPanel
            // 
            this._tlPanel.ColumnCount = 1;
            this._tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tlPanel.Location = new System.Drawing.Point(0, 0);
            this._tlPanel.Name = "_tlPanel";
            this._tlPanel.RowCount = 1;
            this._tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._tlPanel.Size = new System.Drawing.Size(432, 228);
            this._tlPanel.TabIndex = 0;
            // 
            // ComponentList
            // 
            this.Controls.Add(this._tlPanel);
            this.Name = "ComponentList";
            this.Size = new System.Drawing.Size(432, 228);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tlPanel;
    }
}
