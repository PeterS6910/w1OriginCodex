namespace Contal.IwQuick.UI
{
    partial class ConsoleTextBox
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
            this.components = new System.ComponentModel.Container();
            this.ConsoleRitchTextBox = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripConsoleRTB = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripConsoleRTB.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConsoleRitchTextBox
            // 
            this.ConsoleRitchTextBox.ContextMenuStrip = this.contextMenuStripConsoleRTB;
            this.ConsoleRitchTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ConsoleRitchTextBox.Location = new System.Drawing.Point(0, 0);
            this.ConsoleRitchTextBox.Name = "ConsoleRitchTextBox";
            this.ConsoleRitchTextBox.Size = new System.Drawing.Size(100, 96);
            this.ConsoleRitchTextBox.TabIndex = 0;
            this.ConsoleRitchTextBox.Text = System.String.Empty;
            // 
            // contextMenuStripConsoleRTB
            // 
            this.contextMenuStripConsoleRTB.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCut,
            this.toolStripMenuItemCopy,
            this.toolStripMenuItemPaste});
            this.contextMenuStripConsoleRTB.Name = "contextMenuStripConsoleRTB";
            this.contextMenuStripConsoleRTB.Size = new System.Drawing.Size(103, 70);
            // 
            // toolStripMenuItemCut
            // 
            this.toolStripMenuItemCut.Name = "toolStripMenuItemCut";
            this.toolStripMenuItemCut.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItemCut.Text = "Cut";
            // 
            // toolStripMenuItemCopy
            // 
            this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
            this.toolStripMenuItemCopy.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItemCopy.Text = "Copy";
            // 
            // toolStripMenuItemPaste
            // 
            this.toolStripMenuItemPaste.Name = "toolStripMenuItemPaste";
            this.toolStripMenuItemPaste.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItemPaste.Text = "Paste";
            this.contextMenuStripConsoleRTB.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ConsoleRitchTextBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripConsoleRTB;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCut;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPaste;

    }
}
