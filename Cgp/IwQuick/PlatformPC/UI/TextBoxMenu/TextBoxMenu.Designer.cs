using Contal.IwQuick.PlatformPC.Properties;
namespace Contal.IwQuick.UI
{
    partial class TextBoxMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextBoxMenu));
            this._itbTextBox = new Contal.IwQuick.UI.ImageTextBox();
            this._bMenu = new Contal.IwQuick.UI.ButtonMenu();
            this.SuspendLayout();
            // 
            // _itbTextBox
            // 
            this._itbTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbTextBox.BackColor = System.Drawing.Color.White;
            this._itbTextBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._itbTextBox.Image = ((System.Drawing.Image)(resources.GetObject("_itbTextBox.Image")));
            this._itbTextBox.Location = new System.Drawing.Point(0, 0);
            this._itbTextBox.Name = "_itbTextBox";
            this._itbTextBox.ReadOnly = false;
            this._itbTextBox.Size = new System.Drawing.Size(100, 20);
            this._itbTextBox.TabIndex = 4;
            // 
            // 
            // 
            this._itbTextBox.TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._itbTextBox.TextBox.BackColor = System.Drawing.Color.White;
            this._itbTextBox.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._itbTextBox.TextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this._itbTextBox.TextBox.Location = new System.Drawing.Point(19, 2);
            this._itbTextBox.TextBox.Name = "_tbTextBox";
            this._itbTextBox.TextBox.Size = new System.Drawing.Size(80, 13);
            this._itbTextBox.TextBox.TabIndex = 2;
            this._itbTextBox.UseImage = true;
            // 
            // _bMenu
            // 
            this._bMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bMenu.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._bMenu.Image = Resources.PopupMenu;
            this._bMenu.Location = new System.Drawing.Point(100, 0);
            this._bMenu.Name = "_bMenu";
            this._bMenu.Size = new System.Drawing.Size(20, 20);
            this._bMenu.TabIndex = 3;
            this._bMenu.UseVisualStyleBackColor = true;
            // 
            // TextBoxMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this._itbTextBox);
            this.Controls.Add(this._bMenu);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.MaximumSize = new System.Drawing.Size(1200, 55);
            this.MinimumSize = new System.Drawing.Size(30, 20);
            this.Name = "TextBoxMenu";
            this.Size = new System.Drawing.Size(120, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private ButtonMenu _bMenu;
        private ImageTextBox _itbTextBox;
        
    }
}
