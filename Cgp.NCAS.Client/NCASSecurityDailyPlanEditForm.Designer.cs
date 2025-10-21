namespace Contal.Cgp.NCAS.Client
{
    partial class NCASSecurityDailyPlanEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NCASSecurityDailyPlanEditForm));
            this._lName = new System.Windows.Forms.Label();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this._tcSdp = new System.Windows.Forms.TabControl();
            this._tpSchedule = new System.Windows.Forms.TabPage();
            this._dmSecDailyPlan = new Contal.IwQuick.UI.DateMatrix();
            this._gbInfo = new System.Windows.Forms.GroupBox();
            this._lColorGinOrCardPin = new System.Windows.Forms.Label();
            this._lColorGinOrCard = new System.Windows.Forms.Label();
            this._lColorGin = new System.Windows.Forms.Label();
            this._lColorToggleCardPin = new System.Windows.Forms.Label();
            this._lColorCardPin = new System.Windows.Forms.Label();
            this._lColorToggleCard = new System.Windows.Forms.Label();
            this._lColorCard = new System.Windows.Forms.Label();
            this._lGinOrCardPin = new System.Windows.Forms.Label();
            this._lGinOrCard = new System.Windows.Forms.Label();
            this._lGin = new System.Windows.Forms.Label();
            this._lToggleCardPin = new System.Windows.Forms.Label();
            this._lCardPin = new System.Windows.Forms.Label();
            this._lToggleCard = new System.Windows.Forms.Label();
            this._lCard = new System.Windows.Forms.Label();
            this._lMouseColorRight = new System.Windows.Forms.Label();
            this._lMouseColorLeft = new System.Windows.Forms.Label();
            this._lColorLocked = new System.Windows.Forms.Label();
            this._lColorUnlocked = new System.Windows.Forms.Label();
            this._lMouse = new System.Windows.Forms.Label();
            this._lLocked = new System.Windows.Forms.Label();
            this._lUnlocked = new System.Windows.Forms.Label();
            this._tpUserFolders = new System.Windows.Forms.TabPage();
            this._bRefresh = new System.Windows.Forms.Button();
            this._lbUserFolders = new Contal.IwQuick.UI.ImageListBox();
            this._tpReferencedBy = new System.Windows.Forms.TabPage();
            this._tpDescription = new System.Windows.Forms.TabPage();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._eName = new System.Windows.Forms.TextBox();
            this._panelBack = new System.Windows.Forms.Panel();
            this._tcSdp.SuspendLayout();
            this._tpSchedule.SuspendLayout();
            this._gbInfo.SuspendLayout();
            this._tpUserFolders.SuspendLayout();
            this._tpDescription.SuspendLayout();
            this._panelBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 14);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 9;
            this._lName.Text = "Name";
            // 
            // _bCancel
            // 
            this._bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bCancel.Location = new System.Drawing.Point(707, 531);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(80, 22);
            this._bCancel.TabIndex = 7;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this.CancelClick);
            // 
            // _bOk
            // 
            this._bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bOk.Location = new System.Drawing.Point(621, 531);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(80, 22);
            this._bOk.TabIndex = 6;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this.OkClick);
            // 
            // _tcSdp
            // 
            this._tcSdp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._tcSdp.Controls.Add(this._tpSchedule);
            this._tcSdp.Controls.Add(this._tpUserFolders);
            this._tcSdp.Controls.Add(this._tpReferencedBy);
            this._tcSdp.Controls.Add(this._tpDescription);
            this._tcSdp.Location = new System.Drawing.Point(12, 38);
            this._tcSdp.Name = "_tcSdp";
            this._tcSdp.SelectedIndex = 0;
            this._tcSdp.Size = new System.Drawing.Size(785, 487);
            this._tcSdp.TabIndex = 1;
            this._tcSdp.TabStop = false;
            // 
            // _tpSchedule
            // 
            this._tpSchedule.BackColor = System.Drawing.SystemColors.Control;
            this._tpSchedule.Controls.Add(this._dmSecDailyPlan);
            this._tpSchedule.Controls.Add(this._gbInfo);
            this._tpSchedule.Location = new System.Drawing.Point(4, 22);
            this._tpSchedule.Name = "_tpSchedule";
            this._tpSchedule.Padding = new System.Windows.Forms.Padding(3);
            this._tpSchedule.Size = new System.Drawing.Size(777, 461);
            this._tpSchedule.TabIndex = 0;
            this._tpSchedule.Text = "Schedule";
            // 
            // _dmSecDailyPlan
            // 
            this._dmSecDailyPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dmSecDailyPlan.ButtonAlign = Contal.IwQuick.UI.DateMatrix.ButtonsAlign.Center;
            this._dmSecDailyPlan.ButtonsFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._dmSecDailyPlan.ButtonsHeight = 27;
            this._dmSecDailyPlan.ButtonsWidth = 27;
            this._dmSecDailyPlan.Coloras = new System.Drawing.Color[] {
        System.Drawing.Color.LightGreen,
        System.Drawing.Color.Red,
        System.Drawing.Color.Yellow,
        System.Drawing.Color.Magenta,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Aqua,
        System.Drawing.Color.Lavender,
        System.Drawing.Color.DarkGoldenrod,
        System.Drawing.Color.Indigo};
            this._dmSecDailyPlan.ColorLeftClickIndex = 0;
            this._dmSecDailyPlan.ColorRightClickIndex = 1;
            this._dmSecDailyPlan.DefaultColorIndex = 1;
            this._dmSecDailyPlan.Editable = true;
            this._dmSecDailyPlan.LegendStepX = 10;
            this._dmSecDailyPlan.LegendStepY = 6;
            this._dmSecDailyPlan.Location = new System.Drawing.Point(9, 74);
            this._dmSecDailyPlan.Name = "_dmSecDailyPlan";
            this._dmSecDailyPlan.ResizeButtons = true;
            this._dmSecDailyPlan.SelectionType = Contal.IwQuick.UI.SelectionType.Multiselection;
            this._dmSecDailyPlan.Size = new System.Drawing.Size(762, 381);
            this._dmSecDailyPlan.TabIndex = 3;
            this._dmSecDailyPlan.ViewButtons = true;
            // 
            // _gbInfo
            // 
            this._gbInfo.Controls.Add(this._lColorGinOrCardPin);
            this._gbInfo.Controls.Add(this._lColorGinOrCard);
            this._gbInfo.Controls.Add(this._lColorGin);
            this._gbInfo.Controls.Add(this._lColorToggleCardPin);
            this._gbInfo.Controls.Add(this._lColorCardPin);
            this._gbInfo.Controls.Add(this._lColorToggleCard);
            this._gbInfo.Controls.Add(this._lColorCard);
            this._gbInfo.Controls.Add(this._lGinOrCardPin);
            this._gbInfo.Controls.Add(this._lGinOrCard);
            this._gbInfo.Controls.Add(this._lGin);
            this._gbInfo.Controls.Add(this._lToggleCardPin);
            this._gbInfo.Controls.Add(this._lCardPin);
            this._gbInfo.Controls.Add(this._lToggleCard);
            this._gbInfo.Controls.Add(this._lCard);
            this._gbInfo.Controls.Add(this._lMouseColorRight);
            this._gbInfo.Controls.Add(this._lMouseColorLeft);
            this._gbInfo.Controls.Add(this._lColorLocked);
            this._gbInfo.Controls.Add(this._lColorUnlocked);
            this._gbInfo.Controls.Add(this._lMouse);
            this._gbInfo.Controls.Add(this._lLocked);
            this._gbInfo.Controls.Add(this._lUnlocked);
            this._gbInfo.Location = new System.Drawing.Point(26, 6);
            this._gbInfo.Name = "_gbInfo";
            this._gbInfo.Size = new System.Drawing.Size(645, 65);
            this._gbInfo.TabIndex = 2;
            this._gbInfo.TabStop = false;
            this._gbInfo.Text = "Daily plan status";
            // 
            // _lColorGinOrCardPin
            // 
            this._lColorGinOrCardPin.BackColor = System.Drawing.Color.Lavender;
            this._lColorGinOrCardPin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorGinOrCardPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorGinOrCardPin.Location = new System.Drawing.Point(293, 18);
            this._lColorGinOrCardPin.Name = "_lColorGinOrCardPin";
            this._lColorGinOrCardPin.Size = new System.Drawing.Size(15, 15);
            this._lColorGinOrCardPin.TabIndex = 17;
            this._lColorGinOrCardPin.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorGinOrCardPin);
            this._lColorGinOrCardPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorGinOrCardPin);
            // 
            // _lColorGinOrCard
            // 
            this._lColorGinOrCard.BackColor = System.Drawing.Color.Aqua;
            this._lColorGinOrCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorGinOrCard.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorGinOrCard.Location = new System.Drawing.Point(198, 38);
            this._lColorGinOrCard.Name = "_lColorGinOrCard";
            this._lColorGinOrCard.Size = new System.Drawing.Size(15, 15);
            this._lColorGinOrCard.TabIndex = 17;
            this._lColorGinOrCard.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorGinOrCard);
            this._lColorGinOrCard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorGinOrCard);
            // 
            // _lColorGin
            // 
            this._lColorGin.BackColor = System.Drawing.Color.Blue;
            this._lColorGin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorGin.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorGin.Location = new System.Drawing.Point(198, 18);
            this._lColorGin.Name = "_lColorGin";
            this._lColorGin.Size = new System.Drawing.Size(15, 15);
            this._lColorGin.TabIndex = 17;
            this._lColorGin.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorGin);
            this._lColorGin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorGin);
            // 
            // _lColorToggleCardPin
            // 
            this._lColorToggleCardPin.BackColor = System.Drawing.Color.Indigo;
            this._lColorToggleCardPin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorToggleCardPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorToggleCardPin.Location = new System.Drawing.Point(400, 38);
            this._lColorToggleCardPin.Name = "_lColorToggleCardPin";
            this._lColorToggleCardPin.Size = new System.Drawing.Size(15, 15);
            this._lColorToggleCardPin.TabIndex = 16;
            this._lColorToggleCardPin.Visible = false;
            this._lColorToggleCardPin.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorToggleCardPin);
            this._lColorToggleCardPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorToggleCardPin);
            // 
            // _lColorCardPin
            // 
            this._lColorCardPin.BackColor = System.Drawing.Color.Magenta;
            this._lColorCardPin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorCardPin.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorCardPin.Location = new System.Drawing.Point(102, 38);
            this._lColorCardPin.Name = "_lColorCardPin";
            this._lColorCardPin.Size = new System.Drawing.Size(15, 15);
            this._lColorCardPin.TabIndex = 14;
            this._lColorCardPin.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorCardPin);
            this._lColorCardPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorCardPin);
            // 
            // _lColorToggleCard
            // 
            this._lColorToggleCard.BackColor = System.Drawing.Color.DarkGoldenrod;
            this._lColorToggleCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorToggleCard.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorToggleCard.Location = new System.Drawing.Point(400, 18);
            this._lColorToggleCard.Name = "_lColorToggleCard";
            this._lColorToggleCard.Size = new System.Drawing.Size(15, 15);
            this._lColorToggleCard.TabIndex = 15;
            this._lColorToggleCard.Visible = false;
            this._lColorToggleCard.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorToggleCard);
            this._lColorToggleCard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorToggleCard);
            // 
            // _lColorCard
            // 
            this._lColorCard.BackColor = System.Drawing.Color.Yellow;
            this._lColorCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorCard.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorCard.Location = new System.Drawing.Point(102, 18);
            this._lColorCard.Name = "_lColorCard";
            this._lColorCard.Size = new System.Drawing.Size(15, 15);
            this._lColorCard.TabIndex = 13;
            this._lColorCard.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorCard);
            this._lColorCard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorCard);
            // 
            // _lGinOrCardPin
            // 
            this._lGinOrCardPin.AutoSize = true;
            this._lGinOrCardPin.Location = new System.Drawing.Point(314, 19);
            this._lGinOrCardPin.Name = "_lGinOrCardPin";
            this._lGinOrCardPin.Size = new System.Drawing.Size(80, 13);
            this._lGinOrCardPin.TabIndex = 12;
            this._lGinOrCardPin.Text = "Gin/Card + PIN";
            this._lGinOrCardPin.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorGinOrCardPin);
            this._lGinOrCardPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorGinOrCardPin);
            // 
            // _lGinOrCard
            // 
            this._lGinOrCard.AutoSize = true;
            this._lGinOrCard.Location = new System.Drawing.Point(219, 39);
            this._lGinOrCard.Name = "_lGinOrCard";
            this._lGinOrCard.Size = new System.Drawing.Size(50, 13);
            this._lGinOrCard.TabIndex = 12;
            this._lGinOrCard.Text = "Gin/Card";
            this._lGinOrCard.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorGinOrCard);
            this._lGinOrCard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorGinOrCard);
            // 
            // _lGin
            // 
            this._lGin.AutoSize = true;
            this._lGin.Location = new System.Drawing.Point(219, 19);
            this._lGin.Name = "_lGin";
            this._lGin.Size = new System.Drawing.Size(23, 13);
            this._lGin.TabIndex = 12;
            this._lGin.Text = "Gin";
            this._lGin.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorGin);
            this._lGin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorGin);
            // 
            // _lToggleCardPin
            // 
            this._lToggleCardPin.AutoSize = true;
            this._lToggleCardPin.Location = new System.Drawing.Point(421, 40);
            this._lToggleCardPin.Name = "_lToggleCardPin";
            this._lToggleCardPin.Size = new System.Drawing.Size(95, 13);
            this._lToggleCardPin.TabIndex = 11;
            this._lToggleCardPin.Text = "Toggle Card + PIN";
            this._lToggleCardPin.Visible = false;
            this._lToggleCardPin.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorToggleCardPin);
            this._lToggleCardPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorToggleCardPin);
            // 
            // _lCardPin
            // 
            this._lCardPin.AutoSize = true;
            this._lCardPin.Location = new System.Drawing.Point(123, 40);
            this._lCardPin.Name = "_lCardPin";
            this._lCardPin.Size = new System.Drawing.Size(59, 13);
            this._lCardPin.TabIndex = 8;
            this._lCardPin.Text = "Card + PIN";
            this._lCardPin.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorCardPin);
            this._lCardPin.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorCardPin);
            // 
            // _lToggleCard
            // 
            this._lToggleCard.AutoSize = true;
            this._lToggleCard.Location = new System.Drawing.Point(421, 19);
            this._lToggleCard.Name = "_lToggleCard";
            this._lToggleCard.Size = new System.Drawing.Size(64, 13);
            this._lToggleCard.TabIndex = 10;
            this._lToggleCard.Text = "Toggle card";
            this._lToggleCard.Visible = false;
            this._lToggleCard.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorToggleCard);
            this._lToggleCard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorToggleCard);
            // 
            // _lCard
            // 
            this._lCard.AutoSize = true;
            this._lCard.Location = new System.Drawing.Point(123, 19);
            this._lCard.Name = "_lCard";
            this._lCard.Size = new System.Drawing.Size(29, 13);
            this._lCard.TabIndex = 7;
            this._lCard.Text = "Card";
            this._lCard.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorCard);
            this._lCard.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorCard);
            // 
            // _lMouseColorRight
            // 
            this._lMouseColorRight.BackColor = System.Drawing.Color.Red;
            this._lMouseColorRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lMouseColorRight.Location = new System.Drawing.Point(578, 41);
            this._lMouseColorRight.Name = "_lMouseColorRight";
            this._lMouseColorRight.Size = new System.Drawing.Size(15, 15);
            this._lMouseColorRight.TabIndex = 6;
            this._lMouseColorRight.Text = "R";
            this._lMouseColorRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lMouseColorLeft
            // 
            this._lMouseColorLeft.BackColor = System.Drawing.Color.LightGreen;
            this._lMouseColorLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lMouseColorLeft.Location = new System.Drawing.Point(563, 41);
            this._lMouseColorLeft.Name = "_lMouseColorLeft";
            this._lMouseColorLeft.Size = new System.Drawing.Size(15, 15);
            this._lMouseColorLeft.TabIndex = 5;
            this._lMouseColorLeft.Text = "L";
            this._lMouseColorLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _lColorLocked
            // 
            this._lColorLocked.BackColor = System.Drawing.Color.Red;
            this._lColorLocked.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorLocked.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorLocked.Location = new System.Drawing.Point(6, 38);
            this._lColorLocked.Name = "_lColorLocked";
            this._lColorLocked.Size = new System.Drawing.Size(15, 15);
            this._lColorLocked.TabIndex = 4;
            this._lColorLocked.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorLocked);
            this._lColorLocked.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorLocked);
            // 
            // _lColorUnlocked
            // 
            this._lColorUnlocked.BackColor = System.Drawing.Color.LightGreen;
            this._lColorUnlocked.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lColorUnlocked.Cursor = System.Windows.Forms.Cursors.Hand;
            this._lColorUnlocked.Location = new System.Drawing.Point(6, 18);
            this._lColorUnlocked.Name = "_lColorUnlocked";
            this._lColorUnlocked.Size = new System.Drawing.Size(15, 15);
            this._lColorUnlocked.TabIndex = 3;
            this._lColorUnlocked.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorUnlocked);
            this._lColorUnlocked.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorUnlocked);
            // 
            // _lMouse
            // 
            this._lMouse.AutoSize = true;
            this._lMouse.Location = new System.Drawing.Point(560, 20);
            this._lMouse.Name = "_lMouse";
            this._lMouse.Size = new System.Drawing.Size(39, 13);
            this._lMouse.TabIndex = 2;
            this._lMouse.Text = "Mouse";
            // 
            // _lLocked
            // 
            this._lLocked.AutoSize = true;
            this._lLocked.Location = new System.Drawing.Point(27, 40);
            this._lLocked.Name = "_lLocked";
            this._lLocked.Size = new System.Drawing.Size(43, 13);
            this._lLocked.TabIndex = 1;
            this._lLocked.Text = "Locked";
            this._lLocked.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorLocked);
            this._lLocked.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorLocked);
            // 
            // _lUnlocked
            // 
            this._lUnlocked.AutoSize = true;
            this._lUnlocked.Location = new System.Drawing.Point(27, 19);
            this._lUnlocked.Name = "_lUnlocked";
            this._lUnlocked.Size = new System.Drawing.Size(53, 13);
            this._lUnlocked.TabIndex = 0;
            this._lUnlocked.Text = "Unlocked";
            this._lUnlocked.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ColorUnlocked);
            this._lUnlocked.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ColorUnlocked);
            // 
            // _tpUserFolders
            // 
            this._tpUserFolders.BackColor = System.Drawing.SystemColors.Control;
            this._tpUserFolders.Controls.Add(this._bRefresh);
            this._tpUserFolders.Controls.Add(this._lbUserFolders);
            this._tpUserFolders.Location = new System.Drawing.Point(4, 22);
            this._tpUserFolders.Name = "_tpUserFolders";
            this._tpUserFolders.Size = new System.Drawing.Size(777, 461);
            this._tpUserFolders.TabIndex = 3;
            this._tpUserFolders.Text = "User folders";
            this._tpUserFolders.Enter += new System.EventHandler(this._tpUserFolders_Enter);
            // 
            // _bRefresh
            // 
            this._bRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRefresh.Location = new System.Drawing.Point(696, 425);
            this._bRefresh.Name = "_bRefresh";
            this._bRefresh.Size = new System.Drawing.Size(75, 23);
            this._bRefresh.TabIndex = 2;
            this._bRefresh.Text = "Refresh";
            this._bRefresh.UseVisualStyleBackColor = true;
            this._bRefresh.Click += new System.EventHandler(this._bRefresh_Click);
            // 
            // _lbUserFolders
            // 
            this._lbUserFolders.AllowDrop = false;
            this._lbUserFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbUserFolders.BackColor = System.Drawing.SystemColors.Info;
            this._lbUserFolders.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this._lbUserFolders.FormattingEnabled = true;
            this._lbUserFolders.ImageList = null;
            this._lbUserFolders.Location = new System.Drawing.Point(0, 0);
            this._lbUserFolders.Margin = new System.Windows.Forms.Padding(2);
            this._lbUserFolders.Name = "_lbUserFolders";
            this._lbUserFolders.SelectedItemObject = null;
            this._lbUserFolders.Size = new System.Drawing.Size(771, 420);
            this._lbUserFolders.TabIndex = 42;
            this._lbUserFolders.TabStop = false;
            this._lbUserFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._lbUserFolders_MouseDoubleClick);
            // 
            // _tpReferencedBy
            // 
            this._tpReferencedBy.BackColor = System.Drawing.SystemColors.Control;
            this._tpReferencedBy.Location = new System.Drawing.Point(4, 22);
            this._tpReferencedBy.Name = "_tpReferencedBy";
            this._tpReferencedBy.Padding = new System.Windows.Forms.Padding(3);
            this._tpReferencedBy.Size = new System.Drawing.Size(777, 461);
            this._tpReferencedBy.TabIndex = 2;
            this._tpReferencedBy.Text = "Referenced by";
            // 
            // _tpDescription
            // 
            this._tpDescription.BackColor = System.Drawing.SystemColors.Control;
            this._tpDescription.Controls.Add(this._eDescription);
            this._tpDescription.Location = new System.Drawing.Point(4, 22);
            this._tpDescription.Name = "_tpDescription";
            this._tpDescription.Padding = new System.Windows.Forms.Padding(3);
            this._tpDescription.Size = new System.Drawing.Size(777, 461);
            this._tpDescription.TabIndex = 1;
            this._tpDescription.Text = "Description";
            // 
            // _eDescription
            // 
            this._eDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this._eDescription.Location = new System.Drawing.Point(3, 3);
            this._eDescription.Multiline = true;
            this._eDescription.Name = "_eDescription";
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(771, 455);
            this._eDescription.TabIndex = 2;
            this._eDescription.TabStop = false;
            this._eDescription.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _eName
            // 
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(235, 12);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(389, 20);
            this._eName.TabIndex = 0;
            this._eName.TextChanged += new System.EventHandler(this.EditTextChangerOnlyInDatabase);
            // 
            // _panelBack
            // 
            this._panelBack.Controls.Add(this._lName);
            this._panelBack.Controls.Add(this._bCancel);
            this._panelBack.Controls.Add(this._eName);
            this._panelBack.Controls.Add(this._bOk);
            this._panelBack.Controls.Add(this._tcSdp);
            this._panelBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelBack.Location = new System.Drawing.Point(0, 0);
            this._panelBack.Name = "_panelBack";
            this._panelBack.Size = new System.Drawing.Size(808, 565);
            this._panelBack.TabIndex = 10;
            // 
            // NCASSecurityDailyPlanEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(808, 565);
            this.Controls.Add(this._panelBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NCASSecurityDailyPlanEditForm";
            this.Text = "NCASSecurityDailyPlanEditForm";
            this._tcSdp.ResumeLayout(false);
            this._tpSchedule.ResumeLayout(false);
            this._gbInfo.ResumeLayout(false);
            this._gbInfo.PerformLayout();
            this._tpUserFolders.ResumeLayout(false);
            this._tpDescription.ResumeLayout(false);
            this._tpDescription.PerformLayout();
            this._panelBack.ResumeLayout(false);
            this._panelBack.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.TabControl _tcSdp;
        private System.Windows.Forms.TabPage _tpSchedule;
        private System.Windows.Forms.GroupBox _gbInfo;
        private System.Windows.Forms.Label _lMouseColorRight;
        private System.Windows.Forms.Label _lMouseColorLeft;
        private System.Windows.Forms.Label _lColorLocked;
        private System.Windows.Forms.Label _lColorUnlocked;
        private System.Windows.Forms.Label _lMouse;
        private System.Windows.Forms.Label _lLocked;
        private System.Windows.Forms.Label _lUnlocked;
        private System.Windows.Forms.TabPage _tpDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lColorGin;
        private System.Windows.Forms.Label _lColorToggleCardPin;
        private System.Windows.Forms.Label _lColorToggleCard;
        private System.Windows.Forms.Label _lColorCardPin;
        private System.Windows.Forms.Label _lColorCard;
        private System.Windows.Forms.Label _lGin;
        private System.Windows.Forms.Label _lToggleCardPin;
        private System.Windows.Forms.Label _lToggleCard;
        private System.Windows.Forms.Label _lCardPin;
        private System.Windows.Forms.Label _lCard;
        private System.Windows.Forms.Panel _panelBack;
        private System.Windows.Forms.TabPage _tpReferencedBy;
        private System.Windows.Forms.TabPage _tpUserFolders;
        private System.Windows.Forms.Button _bRefresh;
        private Contal.IwQuick.UI.ImageListBox _lbUserFolders;
        private System.Windows.Forms.Label _lColorGinOrCardPin;
        private System.Windows.Forms.Label _lColorGinOrCard;
        private System.Windows.Forms.Label _lGinOrCardPin;
        private System.Windows.Forms.Label _lGinOrCard;
        private Contal.IwQuick.UI.DateMatrix _dmSecDailyPlan;
    }
}
