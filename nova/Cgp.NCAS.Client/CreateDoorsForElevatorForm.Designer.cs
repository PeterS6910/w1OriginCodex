namespace Contal.Cgp.NCAS.Client
{
    partial class CreateDoorsForElevatorForm
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
            this._lFloorsCount = new System.Windows.Forms.Label();
            this._eFloorsCount = new System.Windows.Forms.NumericUpDown();
            this._eLowestFloorNumber = new System.Windows.Forms.NumericUpDown();
            this._lLowestFloorNumber = new System.Windows.Forms.Label();
            this._cbAssignDoorsToFloors = new System.Windows.Forms.CheckBox();
            this._gbAssingDoorsToFloors = new System.Windows.Forms.GroupBox();
            this._cbCreatFloorsIfNotExist = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this._eFloorsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._eLowestFloorNumber)).BeginInit();
            this._gbAssingDoorsToFloors.SuspendLayout();
            this.SuspendLayout();
            // 
            // _bCreate
            // 
            this._bCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCreate.Location = new System.Drawing.Point(178, 138);
            this._bCreate.Name = "_bCreate";
            this._bCreate.Size = new System.Drawing.Size(75, 23);
            this._bCreate.TabIndex = 8;
            this._bCreate.Text = "Create";
            this._bCreate.UseVisualStyleBackColor = true;
            this._bCreate.Click += new System.EventHandler(this._bCreate_Click);
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(259, 138);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 9;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _lFloorsCount
            // 
            this._lFloorsCount.AutoSize = true;
            this._lFloorsCount.Location = new System.Drawing.Point(12, 40);
            this._lFloorsCount.Name = "_lFloorsCount";
            this._lFloorsCount.Size = new System.Drawing.Size(65, 13);
            this._lFloorsCount.TabIndex = 11;
            this._lFloorsCount.Text = "Floors count";
            // 
            // _eFloorsCount
            // 
            this._eFloorsCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eFloorsCount.Location = new System.Drawing.Point(178, 38);
            this._eFloorsCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._eFloorsCount.Name = "_eFloorsCount";
            this._eFloorsCount.Size = new System.Drawing.Size(156, 20);
            this._eFloorsCount.TabIndex = 10;
            // 
            // _eLowestFloorNumber
            // 
            this._eLowestFloorNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eLowestFloorNumber.Location = new System.Drawing.Point(178, 12);
            this._eLowestFloorNumber.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this._eLowestFloorNumber.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this._eLowestFloorNumber.Name = "_eLowestFloorNumber";
            this._eLowestFloorNumber.Size = new System.Drawing.Size(156, 20);
            this._eLowestFloorNumber.TabIndex = 12;
            // 
            // _lLowestFloorNumber
            // 
            this._lLowestFloorNumber.AutoSize = true;
            this._lLowestFloorNumber.Location = new System.Drawing.Point(12, 14);
            this._lLowestFloorNumber.Name = "_lLowestFloorNumber";
            this._lLowestFloorNumber.Size = new System.Drawing.Size(102, 13);
            this._lLowestFloorNumber.TabIndex = 13;
            this._lLowestFloorNumber.Text = "Lowest floor number";
            // 
            // _cbAssignDoorsToFloors
            // 
            this._cbAssignDoorsToFloors.AutoSize = true;
            this._cbAssignDoorsToFloors.Location = new System.Drawing.Point(6, 19);
            this._cbAssignDoorsToFloors.Name = "_cbAssignDoorsToFloors";
            this._cbAssignDoorsToFloors.Size = new System.Drawing.Size(126, 17);
            this._cbAssignDoorsToFloors.TabIndex = 14;
            this._cbAssignDoorsToFloors.Text = "Assign doors to floors";
            this._cbAssignDoorsToFloors.UseVisualStyleBackColor = true;
            this._cbAssignDoorsToFloors.CheckedChanged += new System.EventHandler(this._cbAssignDoorsToFloors_CheckedChanged);
            // 
            // _gbAssingDoorsToFloors
            // 
            this._gbAssingDoorsToFloors.Controls.Add(this._cbCreatFloorsIfNotExist);
            this._gbAssingDoorsToFloors.Controls.Add(this._cbAssignDoorsToFloors);
            this._gbAssingDoorsToFloors.Location = new System.Drawing.Point(15, 64);
            this._gbAssingDoorsToFloors.Name = "_gbAssingDoorsToFloors";
            this._gbAssingDoorsToFloors.Size = new System.Drawing.Size(319, 68);
            this._gbAssingDoorsToFloors.TabIndex = 15;
            this._gbAssingDoorsToFloors.TabStop = false;
            this._gbAssingDoorsToFloors.Text = "Assign doors to floors";
            // 
            // _cbCreatFloorsIfNotExist
            // 
            this._cbCreatFloorsIfNotExist.AutoSize = true;
            this._cbCreatFloorsIfNotExist.Enabled = false;
            this._cbCreatFloorsIfNotExist.Location = new System.Drawing.Point(6, 42);
            this._cbCreatFloorsIfNotExist.Name = "_cbCreatFloorsIfNotExist";
            this._cbCreatFloorsIfNotExist.Size = new System.Drawing.Size(157, 17);
            this._cbCreatFloorsIfNotExist.TabIndex = 15;
            this._cbCreatFloorsIfNotExist.Text = "Create floor if it doesn\'t exist";
            this._cbCreatFloorsIfNotExist.UseVisualStyleBackColor = true;
            // 
            // CreateDoorsForElevatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(346, 173);
            this.Controls.Add(this._gbAssingDoorsToFloors);
            this.Controls.Add(this._lLowestFloorNumber);
            this.Controls.Add(this._eLowestFloorNumber);
            this.Controls.Add(this._lFloorsCount);
            this.Controls.Add(this._eFloorsCount);
            this.Controls.Add(this._bCreate);
            this.Controls.Add(this._bCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(362, 212);
            this.Name = "CreateDoorsForElevatorForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CreateDoorsForElevatorForm";
            ((System.ComponentModel.ISupportInitialize)(this._eFloorsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._eLowestFloorNumber)).EndInit();
            this._gbAssingDoorsToFloors.ResumeLayout(false);
            this._gbAssingDoorsToFloors.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _bCreate;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Label _lFloorsCount;
        private System.Windows.Forms.NumericUpDown _eFloorsCount;
        private System.Windows.Forms.NumericUpDown _eLowestFloorNumber;
        private System.Windows.Forms.Label _lLowestFloorNumber;
        private System.Windows.Forms.CheckBox _cbAssignDoorsToFloors;
        private System.Windows.Forms.GroupBox _gbAssingDoorsToFloors;
        private System.Windows.Forms.CheckBox _cbCreatFloorsIfNotExist;
    }
}
