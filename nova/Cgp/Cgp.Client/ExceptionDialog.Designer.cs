namespace Contal.Cgp.Client
{
    partial class ExceptionDialog
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
            this._bDetails = new System.Windows.Forms.Button();
            this._bQuit = new System.Windows.Forms.Button();
            this._bContinue = new System.Windows.Forms.Button();
            this._eExceptionText = new System.Windows.Forms.TextBox();
            this._lException = new System.Windows.Forms.Label();
            this._errorPricure = new System.Windows.Forms.PictureBox();
            this._lExceptionType = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._errorPricure)).BeginInit();
            this.SuspendLayout();
            // 
            // _bDetails
            // 
            this._bDetails.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._bDetails.Location = new System.Drawing.Point(12, 91);
            this._bDetails.Name = "_bDetails";
            this._bDetails.Size = new System.Drawing.Size(75, 23);
            this._bDetails.TabIndex = 0;
            this._bDetails.Text = "Details";
            this._bDetails.UseVisualStyleBackColor = true;
            this._bDetails.Click += new System.EventHandler(this.DetailClick);
            // 
            // _bQuit
            // 
            this._bQuit.Location = new System.Drawing.Point(350, 91);
            this._bQuit.Name = "_bQuit";
            this._bQuit.Size = new System.Drawing.Size(75, 23);
            this._bQuit.TabIndex = 2;
            this._bQuit.Text = "Quit";
            this._bQuit.UseVisualStyleBackColor = true;
            this._bQuit.Click += new System.EventHandler(this.QuitClick);
            // 
            // _bContinue
            // 
            this._bContinue.Location = new System.Drawing.Point(269, 91);
            this._bContinue.Name = "_bContinue";
            this._bContinue.Size = new System.Drawing.Size(75, 23);
            this._bContinue.TabIndex = 1;
            this._bContinue.Text = "Continue";
            this._bContinue.UseVisualStyleBackColor = true;
            this._bContinue.Click += new System.EventHandler(this.ContinueClick);
            // 
            // _eExceptionText
            // 
            this._eExceptionText.Location = new System.Drawing.Point(12, 125);
            this._eExceptionText.Multiline = true;
            this._eExceptionText.Name = "_eExceptionText";
            this._eExceptionText.ReadOnly = true;
            this._eExceptionText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._eExceptionText.Size = new System.Drawing.Size(413, 174);
            this._eExceptionText.TabIndex = 3;
            // 
            // _lException
            // 
            this._lException.AutoSize = true;
            this._lException.Location = new System.Drawing.Point(64, 12);
            this._lException.Name = "_lException";
            this._lException.Size = new System.Drawing.Size(54, 13);
            this._lException.TabIndex = 4;
            this._lException.Text = "Exception";
            // 
            // _errorPricure
            // 
            this._errorPricure.Location = new System.Drawing.Point(12, 12);
            this._errorPricure.Name = "_errorPricure";
            this._errorPricure.Size = new System.Drawing.Size(37, 38);
            this._errorPricure.TabIndex = 6;
            this._errorPricure.TabStop = false;
            // 
            // _lExceptionType
            // 
            this._lExceptionType.AutoSize = true;
            this._lExceptionType.Location = new System.Drawing.Point(64, 65);
            this._lExceptionType.Name = "_lExceptionType";
            this._lExceptionType.Size = new System.Drawing.Size(89, 13);
            this._lExceptionType.TabIndex = 7;
            this._lExceptionType.Text = "Exception of type";
            // 
            // ExceptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 122);
            this.ControlBox = false;
            this.Controls.Add(this._lExceptionType);
            this.Controls.Add(this._errorPricure);
            this.Controls.Add(this._lException);
            this.Controls.Add(this._eExceptionText);
            this.Controls.Add(this._bContinue);
            this.Controls.Add(this._bQuit);
            this.Controls.Add(this._bDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExceptionDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ExceptionDialog";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this._errorPricure)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bDetails;
        private System.Windows.Forms.Button _bQuit;
        private System.Windows.Forms.Button _bContinue;
        private System.Windows.Forms.TextBox _eExceptionText;
        private System.Windows.Forms.Label _lException;
        private System.Windows.Forms.PictureBox _errorPricure;
        private System.Windows.Forms.Label _lExceptionType;
    }
}