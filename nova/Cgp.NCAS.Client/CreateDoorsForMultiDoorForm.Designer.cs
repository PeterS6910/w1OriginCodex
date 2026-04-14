namespace Contal.Cgp.NCAS.Client
{
    partial class CreateDoorsForMultiDoorForm
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
            this._bCreate = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._eDoorsCount = new System.Windows.Forms.NumericUpDown();
            this._lDoorsCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._eDoorsCount)).BeginInit();
            this.SuspendLayout();
            // 
            // _bCreate
            // 
            this._bCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreate.Location = new System.Drawing.Point(116, 38);
            this._bCreate.Name = "_bCreate";
            this._bCreate.Size = new System.Drawing.Size(75, 23);
            this._bCreate.TabIndex = 6;
            this._bCreate.Text = "Create";
            this._bCreate.UseVisualStyleBackColor = true;
            this._bCreate.Click += new System.EventHandler(this._bCreate_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(197, 38);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _eDoorsCount
            // 
            this._eDoorsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDoorsCount.Location = new System.Drawing.Point(116, 12);
            this._eDoorsCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._eDoorsCount.Name = "_eDoorsCount";
            this._eDoorsCount.Size = new System.Drawing.Size(156, 20);
            this._eDoorsCount.TabIndex = 8;
            // 
            // _lDoorsCount
            // 
            this._lDoorsCount.AutoSize = true;
            this._lDoorsCount.Location = new System.Drawing.Point(12, 14);
            this._lDoorsCount.Name = "_lDoorsCount";
            this._lDoorsCount.Size = new System.Drawing.Size(65, 13);
            this._lDoorsCount.TabIndex = 9;
            this._lDoorsCount.Text = "Doors count";
            // 
            // CreateDoorsForMultiDoorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(284, 73);
            this.Controls.Add(this._lDoorsCount);
            this.Controls.Add(this._eDoorsCount);
            this.Controls.Add(this._bCreate);
            this.Controls.Add(this._bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(300, 112);
            this.Name = "CreateDoorsForMultiDoorForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CreateDoorsForMultiDoorForm";
            ((System.ComponentModel.ISupportInitialize)(this._eDoorsCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bCreate;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.NumericUpDown _eDoorsCount;
        private System.Windows.Forms.Label _lDoorsCount;
    }
}
