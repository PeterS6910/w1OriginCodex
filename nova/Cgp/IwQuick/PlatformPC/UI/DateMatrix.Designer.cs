namespace Contal.IwQuick.UI
{
    partial class DateMatrix
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
            this._dayHourButton = new Contal.IwQuick.UI.DayHourButton();
            this._dayMatrixSchedule = new Contal.IwQuick.UI.DayMatrixSchedule();
            this.SuspendLayout();
            // 
            // _dayHourButton
            // 
            this._dayHourButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._dayHourButton.ButtonFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._dayHourButton.ButtonHeight = 27;
            this._dayHourButton.ButtonWidth = 27;
            this._dayHourButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._dayHourButton.Location = new System.Drawing.Point(9, 306);
            this._dayHourButton.MatrixColors = null;
            this._dayHourButton.Name = "_dayHourButton";
            this._dayHourButton.SelectionColor = System.Drawing.Color.Red;
            this._dayHourButton.Size = new System.Drawing.Size(648, 27);
            this._dayHourButton.TabIndex = 1;
            this._dayHourButton.Text = "dayHourBtn1";
            // 
            // _dayMatrixSchedule
            // 
            this._dayMatrixSchedule.Location = new System.Drawing.Point(3, 3);
            this._dayMatrixSchedule.Name = "_dayMatrixSchedule";
            this._dayMatrixSchedule.Size = new System.Drawing.Size(660, 300);
            this._dayMatrixSchedule.TabIndex = 0;
            this._dayMatrixSchedule.Text = "daySchedule1";
            // 
            // DateMatrix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dayHourButton);
            this.Controls.Add(this._dayMatrixSchedule);
            this.DoubleBuffered = true;
            this.Name = "DateMatrix";
            this.Size = new System.Drawing.Size(666, 340);
            this.Resize += new System.EventHandler(this.DateMatrix_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion       

        private DayMatrixSchedule _dayMatrixSchedule;
        private DayHourButton _dayHourButton;





    }
}
