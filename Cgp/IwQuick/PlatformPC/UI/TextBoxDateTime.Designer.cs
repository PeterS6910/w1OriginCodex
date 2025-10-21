namespace Contal.IwQuick.UI
{
    partial class TextBoxDateTime
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
            this._dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this._textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _dateTimePicker
            // 
            this._dateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._dateTimePicker.Location = new System.Drawing.Point(0, 1);
            this._dateTimePicker.Name = "_dateTimePicker";
            this._dateTimePicker.ShowUpDown = true;
            this._dateTimePicker.Size = new System.Drawing.Size(168, 20);
            this._dateTimePicker.TabIndex = 0;
            this._dateTimePicker.Value = new System.DateTime(2012, 6, 27, 9, 18, 0, 0);
            this._dateTimePicker.Visible = false;
            this._dateTimePicker.ValueChanged += new System.EventHandler(this._dateTimePicker_ValueChanged);
            // 
            // _textBox
            // 
            this._textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._textBox.BackColor = System.Drawing.SystemColors.Window;
            this._textBox.Location = new System.Drawing.Point(0, 1);
            this._textBox.Name = "_textBox";
            this._textBox.ReadOnly = true;
            this._textBox.Size = new System.Drawing.Size(168, 20);
            this._textBox.TabIndex = 2;
            this._textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._textBox_KeyPress);
            // 
            // TextBoxDateTime
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this._dateTimePicker);
            this.Controls.Add(this._textBox);
            this.Name = "TextBoxDateTime";
            this.Size = new System.Drawing.Size(170, 22);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker _dateTimePicker;
        private System.Windows.Forms.TextBox _textBox;
    }
}
