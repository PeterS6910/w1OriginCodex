namespace Clsp485Sniffer
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label51 = new System.Windows.Forms.Label();
            this._portList = new System.Windows.Forms.ComboBox();
            this._startButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._resendFilter = new System.Windows.Forms.CheckBox();
            this._protocolList = new System.Windows.Forms.ComboBox();
            this._filterByProtocol = new System.Windows.Forms.CheckBox();
            this._autoscroll = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._keyText = new System.Windows.Forms.TextBox();
            this._ivText = new System.Windows.Forms.TextBox();
            this._isEncryptedChckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._masterColorLabel = new System.Windows.Forms.Label();
            this._nodeColorLabel = new System.Windows.Forms.Label();
            this._dataTextbox = new Contal.IwQuick.UI.ConsoleTextBox(this.components);
            this._infoTextbox = new Contal.IwQuick.UI.ConsoleTextBox(this.components);
            this.groupBox1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(10, 10);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(60, 13);
            this.label51.TabIndex = 26;
            this.label51.Text = "Port Name:";
            // 
            // _portList
            // 
            this._portList.FormattingEnabled = true;
            this._portList.Location = new System.Drawing.Point(76, 7);
            this._portList.Name = "_portList";
            this._portList.Size = new System.Drawing.Size(297, 21);
            this._portList.TabIndex = 25;
            this._portList.SelectedIndexChanged += new System.EventHandler(this._portList_SelectedIndexChanged);
            // 
            // _startButton
            // 
            this._startButton.Enabled = false;
            this._startButton.Location = new System.Drawing.Point(379, 5);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(75, 23);
            this._startButton.TabIndex = 24;
            this._startButton.Text = "Start";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this._startButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this._resendFilter);
            this.groupBox1.Controls.Add(this._protocolList);
            this.groupBox1.Controls.Add(this._filterByProtocol);
            this.groupBox1.Location = new System.Drawing.Point(12, 514);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(988, 53);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter by:";
            // 
            // _resendFilter
            // 
            this._resendFilter.AutoSize = true;
            this._resendFilter.Location = new System.Drawing.Point(249, 19);
            this._resendFilter.Name = "_resendFilter";
            this._resendFilter.Size = new System.Drawing.Size(63, 17);
            this._resendFilter.TabIndex = 2;
            this._resendFilter.Text = "Resend";
            this._resendFilter.UseVisualStyleBackColor = true;
            // 
            // _protocolList
            // 
            this._protocolList.FormattingEnabled = true;
            this._protocolList.Items.AddRange(new object[] {
            "All",
            "Link Layer",
            "Access",
            "Uploader",
            "Card Reader",
            "Testing"});
            this._protocolList.Location = new System.Drawing.Point(86, 17);
            this._protocolList.Name = "_protocolList";
            this._protocolList.Size = new System.Drawing.Size(121, 21);
            this._protocolList.TabIndex = 1;
            this._protocolList.SelectedIndexChanged += new System.EventHandler(this._protocolList_SelectedIndexChanged);
            // 
            // _filterByProtocol
            // 
            this._filterByProtocol.AutoSize = true;
            this._filterByProtocol.Checked = true;
            this._filterByProtocol.CheckState = System.Windows.Forms.CheckState.Checked;
            this._filterByProtocol.Location = new System.Drawing.Point(15, 19);
            this._filterByProtocol.Name = "_filterByProtocol";
            this._filterByProtocol.Size = new System.Drawing.Size(65, 17);
            this._filterByProtocol.TabIndex = 0;
            this._filterByProtocol.Text = "Protocol";
            this._filterByProtocol.UseVisualStyleBackColor = true;
            // 
            // _autoscroll
            // 
            this._autoscroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._autoscroll.AutoSize = true;
            this._autoscroll.Location = new System.Drawing.Point(27, 491);
            this._autoscroll.Name = "_autoscroll";
            this._autoscroll.Size = new System.Drawing.Size(74, 17);
            this._autoscroll.TabIndex = 29;
            this._autoscroll.Text = "auto scroll";
            this._autoscroll.UseVisualStyleBackColor = true;
            this._autoscroll.CheckedChanged += new System.EventHandler(this._autoscroll_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 34);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._dataTextbox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._infoTextbox);
            this.splitContainer1.Size = new System.Drawing.Size(941, 447);
            this.splitContainer1.SplitterDistance = 470;
            this.splitContainer1.TabIndex = 32;
            // 
            // _keyText
            // 
            this._keyText.Location = new System.Drawing.Point(636, 7);
            this._keyText.Name = "_keyText";
            this._keyText.Size = new System.Drawing.Size(141, 20);
            this._keyText.TabIndex = 33;
            this._keyText.Text = "0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15";
            // 
            // _ivText
            // 
            this._ivText.Location = new System.Drawing.Point(810, 7);
            this._ivText.Name = "_ivText";
            this._ivText.Size = new System.Drawing.Size(141, 20);
            this._ivText.TabIndex = 34;
            this._ivText.Text = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
            // 
            // _isEncryptedChckbox
            // 
            this._isEncryptedChckbox.AutoSize = true;
            this._isEncryptedChckbox.Location = new System.Drawing.Point(513, 9);
            this._isEncryptedChckbox.Name = "_isEncryptedChckbox";
            this._isEncryptedChckbox.Size = new System.Drawing.Size(74, 17);
            this._isEncryptedChckbox.TabIndex = 3;
            this._isEncryptedChckbox.Text = "Encrypted";
            this._isEncryptedChckbox.UseVisualStyleBackColor = true;
            this._isEncryptedChckbox.CheckedChanged += new System.EventHandler(this._isEncryptedChckbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(602, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 35;
            this.label1.Text = "Key:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(784, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "IV:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 492);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "Master Color:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(360, 492);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "Node Color:";
            // 
            // _masterColorLabel
            // 
            this._masterColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._masterColorLabel.AutoSize = true;
            this._masterColorLabel.BackColor = System.Drawing.SystemColors.Window;
            this._masterColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._masterColorLabel.Location = new System.Drawing.Point(264, 492);
            this._masterColorLabel.MinimumSize = new System.Drawing.Size(50, 2);
            this._masterColorLabel.Name = "_masterColorLabel";
            this._masterColorLabel.Size = new System.Drawing.Size(50, 15);
            this._masterColorLabel.TabIndex = 39;
            // 
            // _nodeColorLabel
            // 
            this._nodeColorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._nodeColorLabel.AutoSize = true;
            this._nodeColorLabel.BackColor = System.Drawing.SystemColors.Window;
            this._nodeColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._nodeColorLabel.Location = new System.Drawing.Point(429, 492);
            this._nodeColorLabel.MinimumSize = new System.Drawing.Size(50, 2);
            this._nodeColorLabel.Name = "_nodeColorLabel";
            this._nodeColorLabel.Size = new System.Drawing.Size(50, 15);
            this._nodeColorLabel.TabIndex = 40;
            // 
            // _dataTextbox
            // 
            this._dataTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dataTextbox.AutoScroll = true;
            this._dataTextbox.HexMode = false;
            this._dataTextbox.Location = new System.Drawing.Point(3, 3);
            this._dataTextbox.MaxLines = 1024;
            this._dataTextbox.Name = "_dataTextbox";
            this._dataTextbox.RenderLocalInput = false;
            this._dataTextbox.Size = new System.Drawing.Size(464, 441);
            this._dataTextbox.TabIndex = 27;
            this._dataTextbox.Text = "";
            // 
            // _infoTextbox
            // 
            this._infoTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._infoTextbox.AutoScroll = true;
            this._infoTextbox.HexMode = false;
            this._infoTextbox.Location = new System.Drawing.Point(3, 3);
            this._infoTextbox.MaxLines = 1024;
            this._infoTextbox.Name = "_infoTextbox";
            this._infoTextbox.RenderLocalInput = false;
            this._infoTextbox.Size = new System.Drawing.Size(461, 441);
            this._infoTextbox.TabIndex = 30;
            this._infoTextbox.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 579);
            this.Controls.Add(this._nodeColorLabel);
            this.Controls.Add(this._masterColorLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._isEncryptedChckbox);
            this.Controls.Add(this._ivText);
            this.Controls.Add(this._keyText);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this._autoscroll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label51);
            this.Controls.Add(this._portList);
            this.Controls.Add(this._startButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "CLSP485 Communication Sniffer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.ComboBox _portList;
        private System.Windows.Forms.Button _startButton;
        private Contal.IwQuick.UI.ConsoleTextBox _dataTextbox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox _protocolList;
        private System.Windows.Forms.CheckBox _filterByProtocol;
        private System.Windows.Forms.CheckBox _autoscroll;
        private Contal.IwQuick.UI.ConsoleTextBox _infoTextbox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox _resendFilter;
        private System.Windows.Forms.TextBox _keyText;
        private System.Windows.Forms.TextBox _ivText;
        private System.Windows.Forms.CheckBox _isEncryptedChckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label _masterColorLabel;
        private System.Windows.Forms.Label _nodeColorLabel;
    }
}

