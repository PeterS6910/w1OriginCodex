namespace Contal.IwQuick.UI
{
	partial class TextBoxDatePicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextBoxDatePicker));
            this._bClearDate = new System.Windows.Forms.Button();
            this._bDate = new System.Windows.Forms.Button();
            this._textBoxDateTime = new Contal.IwQuick.UI.TextBoxDateTime();
            this.SuspendLayout();
            // 
            // _bClearDate
            // 
            this._bClearDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bClearDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._bClearDate.Image = ((System.Drawing.Image)(resources.GetObject("_bClearDate.Image")));
            this._bClearDate.Location = new System.Drawing.Point(199, -1);
            this._bClearDate.Name = "_bClearDate";
            this._bClearDate.Size = new System.Drawing.Size(23, 23);
            this._bClearDate.TabIndex = 2;
            this._bClearDate.TabStop = false;
            this._bClearDate.UseVisualStyleBackColor = true;
            this._bClearDate.Click += new System.EventHandler(this._bClearDate_Click);
            // 
            // _bDate
            // 
            this._bDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._bDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._bDate.Image = ((System.Drawing.Image)(resources.GetObject("_bDate.Image")));
            this._bDate.Location = new System.Drawing.Point(175, -1);
            this._bDate.Name = "_bDate";
            this._bDate.Size = new System.Drawing.Size(23, 23);
            this._bDate.TabIndex = 1;
            this._bDate.TabStop = false;
            this._bDate.UseVisualStyleBackColor = true;
            this._bDate.Click += new System.EventHandler(this._bDate_Click);
            // 
            // _textBoxDateTime
            // 
            this._textBoxDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxDateTime.BackColor = System.Drawing.Color.Transparent;
            this._textBoxDateTime.CustomFormat = null;
            this._textBoxDateTime.Location = new System.Drawing.Point(0, 0);
            this._textBoxDateTime.Name = "_textBoxDateTime";
            this._textBoxDateTime.ReadOnly = false;
            this._textBoxDateTime.Size = new System.Drawing.Size(175, 22);
            this._textBoxDateTime.TabIndex = 0;
            this._textBoxDateTime.Value = null;
            this._textBoxDateTime.ValueChanged += new System.EventHandler(this._textBoxDateTime_ValueChanged);
            // 
            // TextBoxDatePicker
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this._textBoxDateTime);
            this.Controls.Add(this._bClearDate);
            this.Controls.Add(this._bDate);
            this.MaximumSize = new System.Drawing.Size(1000, 60);
            this.MinimumSize = new System.Drawing.Size(100, 22);
            this.Name = "TextBoxDatePicker";
            this.Size = new System.Drawing.Size(222, 22);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Button _bDate;
        private System.Windows.Forms.Button _bClearDate;
        private TextBoxDateTime _textBoxDateTime;
	}
}
