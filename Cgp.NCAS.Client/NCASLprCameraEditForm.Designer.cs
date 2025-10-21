namespace Contal.Cgp.NCAS.Client
{
    partial class NCASLprCameraEditForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this._lName = new System.Windows.Forms.Label();
            this._eName = new System.Windows.Forms.TextBox();
            this._lId = new System.Windows.Forms.Label();
            this._eId = new System.Windows.Forms.TextBox();
            this._lIpAddress = new System.Windows.Forms.Label();
            this._eIpAddress = new System.Windows.Forms.TextBox();
            this._lPort = new System.Windows.Forms.Label();
            this._ePort = new System.Windows.Forms.TextBox();
            this._lPortSsl = new System.Windows.Forms.Label();
            this._ePortSsl = new System.Windows.Forms.TextBox();
            this._lMacAddress = new System.Windows.Forms.Label();
            this._eMacAddress = new System.Windows.Forms.TextBox();
            this._lCommunicationScope = new System.Windows.Forms.Label();
            this._cbCommunicationScope = new System.Windows.Forms.ComboBox();
            this._lCcu = new System.Windows.Forms.Label();
            this._cbCcu = new System.Windows.Forms.ComboBox();
            this._lDescription = new System.Windows.Forms.Label();
            this._eDescription = new System.Windows.Forms.TextBox();
            this._lLocalAlarmInstruction = new System.Windows.Forms.Label();
            this._eLocalAlarmInstruction = new System.Windows.Forms.TextBox();
            this._lLocked = new System.Windows.Forms.Label();
            this._chkLocked = new System.Windows.Forms.CheckBox();
            this._lLockingClientIp = new System.Windows.Forms.Label();
            this._eLockingClientIp = new System.Windows.Forms.TextBox();
            this._lIsOnline = new System.Windows.Forms.Label();
            this._chkIsOnline = new System.Windows.Forms.CheckBox();
            this._lLastHeartbeatAt = new System.Windows.Forms.Label();
            this._eLastHeartbeatAt = new System.Windows.Forms.TextBox();
            this._lLastLicensePlate = new System.Windows.Forms.Label();
            this._eLastLicensePlate = new System.Windows.Forms.TextBox();
            this._lHealthState = new System.Windows.Forms.Label();
            this._cbHealthState = new System.Windows.Forms.ComboBox();
            this._lEnableParentInFullName = new System.Windows.Forms.Label();
            this._chkEnableParentInFullName = new System.Windows.Forms.CheckBox();
            this._lCkUnique = new System.Windows.Forms.Label();
            this._eCkUnique = new System.Windows.Forms.TextBox();
            this._lObjectType = new System.Windows.Forms.Label();
            this._eObjectType = new System.Windows.Forms.TextBox();
            this._lVersion = new System.Windows.Forms.Label();
            this._eVersion = new System.Windows.Forms.TextBox();
            this._buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._bOk = new System.Windows.Forms.Button();
            this._bCancel = new System.Windows.Forms.Button();
            this._bApply = new System.Windows.Forms.Button();
            this._tableLayout.SuspendLayout();
            this._buttonPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // _tableLayout
            //
            this._tableLayout.AutoScroll = true;
            this._tableLayout.ColumnCount = 2;
            this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this._tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayout.Controls.Add(this._lName, 0, 0);
            this._tableLayout.Controls.Add(this._eName, 1, 0);
            this._tableLayout.Controls.Add(this._lId, 0, 1);
            this._tableLayout.Controls.Add(this._eId, 1, 1);
            this._tableLayout.Controls.Add(this._lIpAddress, 0, 2);
            this._tableLayout.Controls.Add(this._eIpAddress, 1, 2);
            this._tableLayout.Controls.Add(this._lPort, 0, 3);
            this._tableLayout.Controls.Add(this._ePort, 1, 3);
            this._tableLayout.Controls.Add(this._lPortSsl, 0, 4);
            this._tableLayout.Controls.Add(this._ePortSsl, 1, 4);
            this._tableLayout.Controls.Add(this._lMacAddress, 0, 5);
            this._tableLayout.Controls.Add(this._eMacAddress, 1, 5);
            this._tableLayout.Controls.Add(this._lCommunicationScope, 0, 6);
            this._tableLayout.Controls.Add(this._cbCommunicationScope, 1, 6);
            this._tableLayout.Controls.Add(this._lCcu, 0, 7);
            this._tableLayout.Controls.Add(this._cbCcu, 1, 7);
            this._tableLayout.Controls.Add(this._lDescription, 0, 8);
            this._tableLayout.Controls.Add(this._eDescription, 1, 8);
            this._tableLayout.Controls.Add(this._lLocalAlarmInstruction, 0, 9);
            this._tableLayout.Controls.Add(this._eLocalAlarmInstruction, 1, 9);
            this._tableLayout.Controls.Add(this._lLocked, 0, 10);
            this._tableLayout.Controls.Add(this._chkLocked, 1, 10);
            this._tableLayout.Controls.Add(this._lLockingClientIp, 0, 11);
            this._tableLayout.Controls.Add(this._eLockingClientIp, 1, 11);
            this._tableLayout.Controls.Add(this._lIsOnline, 0, 12);
            this._tableLayout.Controls.Add(this._chkIsOnline, 1, 12);
            this._tableLayout.Controls.Add(this._lLastHeartbeatAt, 0, 13);
            this._tableLayout.Controls.Add(this._eLastHeartbeatAt, 1, 13);
            this._tableLayout.Controls.Add(this._lLastLicensePlate, 0, 14);
            this._tableLayout.Controls.Add(this._eLastLicensePlate, 1, 14);
            this._tableLayout.Controls.Add(this._lHealthState, 0, 15);
            this._tableLayout.Controls.Add(this._cbHealthState, 1, 15);
            this._tableLayout.Controls.Add(this._lEnableParentInFullName, 0, 16);
            this._tableLayout.Controls.Add(this._chkEnableParentInFullName, 1, 16);
            this._tableLayout.Controls.Add(this._lCkUnique, 0, 17);
            this._tableLayout.Controls.Add(this._eCkUnique, 1, 17);
            this._tableLayout.Controls.Add(this._lObjectType, 0, 18);
            this._tableLayout.Controls.Add(this._eObjectType, 1, 18);
            this._tableLayout.Controls.Add(this._lVersion, 0, 19);
            this._tableLayout.Controls.Add(this._eVersion, 1, 19);
            this._tableLayout.Controls.Add(this._buttonPanel, 0, 20);
            this._tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayout.Location = new System.Drawing.Point(0, 0);
            this._tableLayout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._tableLayout.Name = "_tableLayout";
            this._tableLayout.RowCount = 21;
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayout.Size = new System.Drawing.Size(624, 720);
            this._tableLayout.TabIndex = 0;
            //
            // _lName
            //
            this._lName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(12, 9);
            this._lName.Margin = new System.Windows.Forms.Padding(12, 9, 3, 9);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(39, 15);
            this._lName.TabIndex = 0;
            this._lName.Text = "Name";
            //
            // _eName
            //
            this._eName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eName.Location = new System.Drawing.Point(163, 6);
            this._eName.Margin = new System.Windows.Forms.Padding(3, 6, 12, 6);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(449, 23);
            this._eName.TabIndex = 1;
            //
            // _lId
            //
            this._lId.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lId.AutoSize = true;
            this._lId.Location = new System.Drawing.Point(12, 44);
            this._lId.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lId.Name = "_lId";
            this._lId.Size = new System.Drawing.Size(73, 15);
            this._lId.TabIndex = 2;
            this._lId.Text = "Camera GUID";
            //
            // _eId
            //
            this._eId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eId.Location = new System.Drawing.Point(163, 35);
            this._eId.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eId.Name = "_eId";
            this._eId.ReadOnly = true;
            this._eId.Size = new System.Drawing.Size(449, 23);
            this._eId.TabIndex = 3;
            this._eId.TabStop = false;
            //
            // _lIpAddress
            //
            this._lIpAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lIpAddress.AutoSize = true;
            this._lIpAddress.Location = new System.Drawing.Point(12, 73);
            this._lIpAddress.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lIpAddress.Name = "_lIpAddress";
            this._lIpAddress.Size = new System.Drawing.Size(67, 15);
            this._lIpAddress.TabIndex = 4;
            this._lIpAddress.Text = "IP address";
            //
            // _eIpAddress
            //
            this._eIpAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eIpAddress.Location = new System.Drawing.Point(163, 35);
            this._eIpAddress.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eIpAddress.Name = "_eIpAddress";
            this._eIpAddress.Size = new System.Drawing.Size(449, 23);
            this._eIpAddress.TabIndex = 5;
            // 
            // _lPort
            // 
            this._lPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lPort.AutoSize = true;
            this._lPort.Location = new System.Drawing.Point(12, 73);
            this._lPort.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lPort.Name = "_lPort";
            this._lPort.Size = new System.Drawing.Size(31, 15);
            this._lPort.TabIndex = 4;
            this._lPort.Text = "Port";
            //
            // _ePort
            //
            this._ePort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ePort.Location = new System.Drawing.Point(163, 93);
            this._ePort.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._ePort.Name = "_ePort";
            this._ePort.Size = new System.Drawing.Size(449, 23);
            this._ePort.TabIndex = 7;
            //
            // _lPortSsl
            //
            this._lPortSsl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lPortSsl.AutoSize = true;
            this._lPortSsl.Location = new System.Drawing.Point(12, 131);
            this._lPortSsl.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lPortSsl.Name = "_lPortSsl";
            this._lPortSsl.Size = new System.Drawing.Size(51, 15);
            this._lPortSsl.TabIndex = 8;
            this._lPortSsl.Text = "SSL port";
            //
            // _ePortSsl
            //
            this._ePortSsl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ePortSsl.Location = new System.Drawing.Point(163, 122);
            this._ePortSsl.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._ePortSsl.Name = "_ePortSsl";
            this._ePortSsl.Size = new System.Drawing.Size(449, 23);
            this._ePortSsl.TabIndex = 9;
            //
            // _lMacAddress
            //
            this._lMacAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lMacAddress.AutoSize = true;
            this._lMacAddress.Location = new System.Drawing.Point(12, 160);
            this._lMacAddress.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lMacAddress.Name = "_lMacAddress";
            this._lMacAddress.Size = new System.Drawing.Size(74, 15);
            this._lMacAddress.TabIndex = 10;
            this._lMacAddress.Text = "MAC address";
            //
            // _eMacAddress
            //
            this._eMacAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eMacAddress.Location = new System.Drawing.Point(163, 151);
            this._eMacAddress.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eMacAddress.Name = "_eMacAddress";
            this._eMacAddress.Size = new System.Drawing.Size(449, 23);
            this._eMacAddress.TabIndex = 11;
            //
            // _lCommunicationScope
            //
            this._lCommunicationScope.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCommunicationScope.AutoSize = true;
            this._lCommunicationScope.Location = new System.Drawing.Point(12, 189);
            this._lCommunicationScope.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lCommunicationScope.Name = "_lCommunicationScope";
            this._lCommunicationScope.Size = new System.Drawing.Size(129, 15);
            this._lCommunicationScope.TabIndex = 12;
            this._lCommunicationScope.Text = "Communication scope";
            //
            // _cbCommunicationScope
            //
            this._cbCommunicationScope.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cbCommunicationScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCommunicationScope.FormattingEnabled = true;
            this._cbCommunicationScope.Location = new System.Drawing.Point(163, 186);
            this._cbCommunicationScope.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._cbCommunicationScope.Name = "_cbCommunicationScope";
            this._cbCommunicationScope.Size = new System.Drawing.Size(449, 23);
            this._cbCommunicationScope.TabIndex = 13;
            //
            // _lCcu
            //
            this._lCcu.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCcu.AutoSize = true;
            this._lCcu.Location = new System.Drawing.Point(12, 218);
            this._lCcu.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lCcu.Name = "_lCcu";
            this._lCcu.Size = new System.Drawing.Size(31, 15);
            this._lCcu.TabIndex = 14;
            this._lCcu.Text = "CCU";
            //
            // _cbCcu
            //
            this._cbCcu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cbCcu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbCcu.FormattingEnabled = true;
            this._cbCcu.Location = new System.Drawing.Point(163, 215);
            this._cbCcu.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._cbCcu.Name = "_cbCcu";
            this._cbCcu.Size = new System.Drawing.Size(449, 23);
            this._cbCcu.TabIndex = 15;
            //
            // _lDescription
            //
            this._lDescription.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lDescription.AutoSize = true;
            this._lDescription.Location = new System.Drawing.Point(12, 263);
            this._lDescription.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lDescription.Name = "_lDescription";
            this._lDescription.Size = new System.Drawing.Size(67, 15);
            this._lDescription.TabIndex = 16;
            this._lDescription.Text = "Description";
            //
            // _eDescription
            //
            this._eDescription.AcceptsReturn = true;
            this._eDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)
                        | System.Windows.Forms.AnchorStyles.Bottom)));
            this._eDescription.Location = new System.Drawing.Point(163, 240);
            this._eDescription.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eDescription.Multiline = true;
            this._eDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eDescription.Size = new System.Drawing.Size(449, 74);
            this._eDescription.TabIndex = 17;
            //
            // _lLocalAlarmInstruction
            //
            this._lLocalAlarmInstruction.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lLocalAlarmInstruction.AutoSize = true;
            this._lLocalAlarmInstruction.Location = new System.Drawing.Point(12, 367);
            this._lLocalAlarmInstruction.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lLocalAlarmInstruction.Name = "_lLocalAlarmInstruction";
            this._lLocalAlarmInstruction.Size = new System.Drawing.Size(126, 15);
            this._lLocalAlarmInstruction.TabIndex = 18;
            this._lLocalAlarmInstruction.Text = "Local alarm instruction";
            // 
            // _eLocalAlarmInstruction
            // 
            this._eLocalAlarmInstruction.AcceptsReturn = true;
            this._eLocalAlarmInstruction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)
                        | System.Windows.Forms.AnchorStyles.Bottom)));
            this._eLocalAlarmInstruction.Location = new System.Drawing.Point(163, 320);
            this._eLocalAlarmInstruction.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eLocalAlarmInstruction.Multiline = true;
            this._eLocalAlarmInstruction.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._eLocalAlarmInstruction.Size = new System.Drawing.Size(449, 114);
            this._eLocalAlarmInstruction.TabIndex = 19;
            //
            // _lLocked
            //
            this._lLocked.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lLocked.AutoSize = true;
            this._lLocked.Location = new System.Drawing.Point(12, 446);
            this._lLocked.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lLocked.Name = "_lLocked";
            this._lLocked.Size = new System.Drawing.Size(44, 15);
            this._lLocked.TabIndex = 20;
            this._lLocked.Text = "Locked";
            //
            // _chkLocked
            //
            this._chkLocked.AutoSize = true;
            this._chkLocked.Enabled = false;
            this._chkLocked.Location = new System.Drawing.Point(163, 440);
            this._chkLocked.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._chkLocked.Name = "_chkLocked";
            this._chkLocked.Size = new System.Drawing.Size(15, 14);
            this._chkLocked.TabIndex = 21;
            this._chkLocked.TabStop = false;
            this._chkLocked.UseVisualStyleBackColor = true;
            //
            // _lLockingClientIp
            //
            this._lLockingClientIp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lLockingClientIp.AutoSize = true;
            this._lLockingClientIp.Location = new System.Drawing.Point(12, 475);
            this._lLockingClientIp.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lLockingClientIp.Name = "_lLockingClientIp";
            this._lLockingClientIp.Size = new System.Drawing.Size(105, 15);
            this._lLockingClientIp.TabIndex = 22;
            this._lLockingClientIp.Text = "Locking client IP";
            //
            // _eLockingClientIp
            //
            this._eLockingClientIp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eLockingClientIp.Location = new System.Drawing.Point(163, 466);
            this._eLockingClientIp.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eLockingClientIp.Name = "_eLockingClientIp";
            this._eLockingClientIp.ReadOnly = true;
            this._eLockingClientIp.Size = new System.Drawing.Size(449, 23);
            this._eLockingClientIp.TabIndex = 23;
            this._eLockingClientIp.TabStop = false;
            //
            // _lIsOnline
            //
            this._lIsOnline.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lIsOnline.AutoSize = true;
            this._lIsOnline.Location = new System.Drawing.Point(12, 504);
            this._lIsOnline.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lIsOnline.Name = "_lIsOnline";
            this._lIsOnline.Size = new System.Drawing.Size(55, 15);
            this._lIsOnline.TabIndex = 24;
            this._lIsOnline.Text = "Is online";
            //
            // _chkIsOnline
            //
            this._chkIsOnline.AutoSize = true;
            this._chkIsOnline.Enabled = false;
            this._chkIsOnline.Location = new System.Drawing.Point(163, 498);
            this._chkIsOnline.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._chkIsOnline.Name = "_chkIsOnline";
            this._chkIsOnline.Size = new System.Drawing.Size(15, 14);
            this._chkIsOnline.TabIndex = 25;
            this._chkIsOnline.TabStop = false;
            this._chkIsOnline.UseVisualStyleBackColor = true;
            //
            // _lLastHeartbeatAt
            //
            this._lLastHeartbeatAt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lLastHeartbeatAt.AutoSize = true;
            this._lLastHeartbeatAt.Location = new System.Drawing.Point(12, 533);
            this._lLastHeartbeatAt.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lLastHeartbeatAt.Name = "_lLastHeartbeatAt";
            this._lLastHeartbeatAt.Size = new System.Drawing.Size(101, 15);
            this._lLastHeartbeatAt.TabIndex = 26;
            this._lLastHeartbeatAt.Text = "Last heartbeat at";
            //
            // _eLastHeartbeatAt
            //
            this._eLastHeartbeatAt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eLastHeartbeatAt.Location = new System.Drawing.Point(163, 524);
            this._eLastHeartbeatAt.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eLastHeartbeatAt.Name = "_eLastHeartbeatAt";
            this._eLastHeartbeatAt.ReadOnly = true;
            this._eLastHeartbeatAt.Size = new System.Drawing.Size(449, 23);
            this._eLastHeartbeatAt.TabIndex = 27;
            this._eLastHeartbeatAt.TabStop = false;
            //
            // _lLastLicensePlate
            //
            this._lLastLicensePlate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lLastLicensePlate.AutoSize = true;
            this._lLastLicensePlate.Location = new System.Drawing.Point(12, 562);
            this._lLastLicensePlate.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lLastLicensePlate.Name = "_lLastLicensePlate";
            this._lLastLicensePlate.Size = new System.Drawing.Size(106, 15);
            this._lLastLicensePlate.TabIndex = 28;
            this._lLastLicensePlate.Text = "Last license plate";
            //
            // _eLastLicensePlate
            //
            this._eLastLicensePlate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eLastLicensePlate.Location = new System.Drawing.Point(163, 553);
            this._eLastLicensePlate.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eLastLicensePlate.Name = "_eLastLicensePlate";
            this._eLastLicensePlate.ReadOnly = true;
            this._eLastLicensePlate.Size = new System.Drawing.Size(449, 23);
            this._eLastLicensePlate.TabIndex = 29;
            this._eLastLicensePlate.TabStop = false;
            //
            // _lHealthState
            //
            this._lHealthState.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lHealthState.AutoSize = true;
            this._lHealthState.Location = new System.Drawing.Point(12, 591);
            this._lHealthState.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lHealthState.Name = "_lHealthState";
            this._lHealthState.Size = new System.Drawing.Size(74, 15);
            this._lHealthState.TabIndex = 30;
            this._lHealthState.Text = "Health state";
            //
            // _cbHealthState
            //
            this._cbHealthState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._cbHealthState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbHealthState.Enabled = false;
            this._cbHealthState.FormattingEnabled = true;
            this._cbHealthState.Location = new System.Drawing.Point(163, 588);
            this._cbHealthState.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._cbHealthState.Name = "_cbHealthState";
            this._cbHealthState.Size = new System.Drawing.Size(449, 23);
            this._cbHealthState.TabIndex = 31;
            this._cbHealthState.TabStop = false;
            //
            // _lEnableParentInFullName
            //
            this._lEnableParentInFullName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lEnableParentInFullName.AutoSize = true;
            this._lEnableParentInFullName.Location = new System.Drawing.Point(12, 620);
            this._lEnableParentInFullName.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lEnableParentInFullName.Name = "_lEnableParentInFullName";
            this._lEnableParentInFullName.Size = new System.Drawing.Size(140, 15);
            this._lEnableParentInFullName.TabIndex = 32;
            this._lEnableParentInFullName.Text = "Enable parent in full name";
            //
            // _chkEnableParentInFullName
            //
            this._chkEnableParentInFullName.AutoSize = true;
            this._chkEnableParentInFullName.Location = new System.Drawing.Point(163, 617);
            this._chkEnableParentInFullName.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._chkEnableParentInFullName.Name = "_chkEnableParentInFullName";
            this._chkEnableParentInFullName.Size = new System.Drawing.Size(15, 14);
            this._chkEnableParentInFullName.TabIndex = 33;
            this._chkEnableParentInFullName.UseVisualStyleBackColor = true;
            //
            // _lCkUnique
            //
            this._lCkUnique.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lCkUnique.AutoSize = true;
            this._lCkUnique.Location = new System.Drawing.Point(12, 649);
            this._lCkUnique.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lCkUnique.Name = "_lCkUnique";
            this._lCkUnique.Size = new System.Drawing.Size(60, 15);
            this._lCkUnique.TabIndex = 34;
            this._lCkUnique.Text = "CK unique";
            //
            // _eCkUnique
            //
            this._eCkUnique.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eCkUnique.Location = new System.Drawing.Point(163, 640);
            this._eCkUnique.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eCkUnique.Name = "_eCkUnique";
            this._eCkUnique.ReadOnly = true;
            this._eCkUnique.Size = new System.Drawing.Size(449, 23);
            this._eCkUnique.TabIndex = 35;
            this._eCkUnique.TabStop = false;
            //
            // _lObjectType
            //
            this._lObjectType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lObjectType.AutoSize = true;
            this._lObjectType.Location = new System.Drawing.Point(12, 678);
            this._lObjectType.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lObjectType.Name = "_lObjectType";
            this._lObjectType.Size = new System.Drawing.Size(72, 15);
            this._lObjectType.TabIndex = 36;
            this._lObjectType.Text = "Object type";
            //
            // _eObjectType
            //
            this._eObjectType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eObjectType.Location = new System.Drawing.Point(163, 669);
            this._eObjectType.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eObjectType.Name = "_eObjectType";
            this._eObjectType.ReadOnly = true;
            this._eObjectType.Size = new System.Drawing.Size(449, 23);
            this._eObjectType.TabIndex = 37;
            this._eObjectType.TabStop = false;
            //
            // _lVersion
            //
            this._lVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lVersion.AutoSize = true;
            this._lVersion.Location = new System.Drawing.Point(12, 707);
            this._lVersion.Margin = new System.Windows.Forms.Padding(12, 6, 3, 9);
            this._lVersion.Name = "_lVersion";
            this._lVersion.Size = new System.Drawing.Size(47, 15);
            this._lVersion.TabIndex = 38;
            this._lVersion.Text = "Version";
            //
            // _eVersion
            //
            this._eVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eVersion.Location = new System.Drawing.Point(163, 698);
            this._eVersion.Margin = new System.Windows.Forms.Padding(3, 3, 12, 6);
            this._eVersion.Name = "_eVersion";
            this._eVersion.ReadOnly = true;
            this._eVersion.Size = new System.Drawing.Size(449, 23);
            this._eVersion.TabIndex = 39;
            this._eVersion.TabStop = false;
            //
            // _buttonPanel
            //
            this._buttonPanel.AutoSize = true;
            this._buttonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableLayout.SetColumnSpan(this._buttonPanel, 2);
            this._buttonPanel.Controls.Add(this._bOk);
            this._buttonPanel.Controls.Add(this._bCancel);
            this._buttonPanel.Controls.Add(this._bApply);
            this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this._buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this._buttonPanel.Location = new System.Drawing.Point(273, 727);
            this._buttonPanel.Margin = new System.Windows.Forms.Padding(3, 6, 12, 12);
            this._buttonPanel.Name = "_buttonPanel";
            this._buttonPanel.Size = new System.Drawing.Size(327, 78);
            this._buttonPanel.TabIndex = 40;
            // 
            // _bOk
            // 
            this._bOk.AutoSize = true;
            this._bOk.Location = new System.Drawing.Point(243, 3);
            this._bOk.Margin = new System.Windows.Forms.Padding(3, 3, 3, 9);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(81, 27);
            this._bOk.TabIndex = 0;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // _bCancel
            // 
            this._bCancel.AutoSize = true;
            this._bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._bCancel.Location = new System.Drawing.Point(156, 3);
            this._bCancel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 9);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(81, 27);
            this._bCancel.TabIndex = 1;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bApply
            // 
            this._bApply.AutoSize = true;
            this._bApply.Location = new System.Drawing.Point(69, 3);
            this._bApply.Margin = new System.Windows.Forms.Padding(3, 3, 3, 9);
            this._bApply.Name = "_bApply";
            this._bApply.Size = new System.Drawing.Size(81, 27);
            this._bApply.TabIndex = 2;
            this._bApply.Text = "Apply";
            this._bApply.UseVisualStyleBackColor = true;
            this._bApply.Click += new System.EventHandler(this._bApply_Click);
            // 
            // NCASLprCameraEditForm
            // 
            this.AcceptButton = this._bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._bCancel;
            this.ClientSize = new System.Drawing.Size(624, 720);
            this.Controls.Add(this._tableLayout);
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "NCASLprCameraEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LPR camera";
            this._tableLayout.ResumeLayout(false);
            this._tableLayout.PerformLayout();
            this._buttonPanel.ResumeLayout(false);
            this._buttonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayout;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Label _lId;
        private System.Windows.Forms.TextBox _eId;
        private System.Windows.Forms.Label _lIpAddress;
        private System.Windows.Forms.TextBox _eIpAddress;
        private System.Windows.Forms.Label _lPort;
        private System.Windows.Forms.TextBox _ePort;
        private System.Windows.Forms.Label _lPortSsl;
        private System.Windows.Forms.TextBox _ePortSsl;
        private System.Windows.Forms.Label _lMacAddress;
        private System.Windows.Forms.TextBox _eMacAddress;
        private System.Windows.Forms.Label _lCommunicationScope;
        private System.Windows.Forms.ComboBox _cbCommunicationScope;
        private System.Windows.Forms.Label _lCcu;
        private System.Windows.Forms.ComboBox _cbCcu;
        private System.Windows.Forms.Label _lDescription;
        private System.Windows.Forms.TextBox _eDescription;
        private System.Windows.Forms.Label _lLocalAlarmInstruction;
        private System.Windows.Forms.TextBox _eLocalAlarmInstruction;
        private System.Windows.Forms.Label _lLocked;
        private System.Windows.Forms.CheckBox _chkLocked;
        private System.Windows.Forms.Label _lLockingClientIp;
        private System.Windows.Forms.TextBox _eLockingClientIp;
        private System.Windows.Forms.Label _lIsOnline;
        private System.Windows.Forms.CheckBox _chkIsOnline;
        private System.Windows.Forms.Label _lLastHeartbeatAt;
        private System.Windows.Forms.TextBox _eLastHeartbeatAt;
        private System.Windows.Forms.Label _lLastLicensePlate;
        private System.Windows.Forms.TextBox _eLastLicensePlate;
        private System.Windows.Forms.Label _lHealthState;
        private System.Windows.Forms.ComboBox _cbHealthState;
        private System.Windows.Forms.Label _lEnableParentInFullName;
        private System.Windows.Forms.CheckBox _chkEnableParentInFullName;
        private System.Windows.Forms.Label _lCkUnique;
        private System.Windows.Forms.TextBox _eCkUnique;
        private System.Windows.Forms.Label _lObjectType;
        private System.Windows.Forms.TextBox _eObjectType;
        private System.Windows.Forms.Label _lVersion;
        private System.Windows.Forms.TextBox _eVersion;
        private System.Windows.Forms.FlowLayoutPanel _buttonPanel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bApply;
    }
}
