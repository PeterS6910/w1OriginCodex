namespace NovaEventLogs
{
    partial class EventLogs
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
            this._dgValues = new System.Windows.Forms.DataGridView();
            this._pFilter = new System.Windows.Forms.Panel();
            this._cbType = new System.Windows.Forms.ComboBox();
            this._tbdpDateToFilter = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._tbdpDateFromFilter = new Contal.IwQuick.UI.TextBoxDatePicker();
            this._bFilterDateOneDay = new System.Windows.Forms.Button();
            this._lDateToFilter = new System.Windows.Forms.Label();
            this._lDescription = new System.Windows.Forms.Label();
            this._eDescriptionFilter = new System.Windows.Forms.TextBox();
            this._lEventSourceFilter = new System.Windows.Forms.Label();
            this._lDateFromFilter = new System.Windows.Forms.Label();
            this._lCGPSourceFilter = new System.Windows.Forms.Label();
            this._lTypeFilter = new System.Windows.Forms.Label();
            this._eEventSourceFilter = new System.Windows.Forms.TextBox();
            this._eCGPSourceFilter = new System.Windows.Forms.TextBox();
            this._pControl = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._lTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).BeginInit();
            this._pFilter.SuspendLayout();
            this._pControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgValues
            // 
            this._dgValues.AllowUserToAddRows = false;
            this._dgValues.AllowUserToDeleteRows = false;
            this._dgValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgValues.Location = new System.Drawing.Point(0, 0);
            this._dgValues.MultiSelect = false;
            this._dgValues.Name = "_dgValues";
            this._dgValues.ReadOnly = true;
            this._dgValues.RowTemplate.Height = 24;
            this._dgValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgValues.Size = new System.Drawing.Size(890, 411);
            this._dgValues.TabIndex = 12;
            this._dgValues.TabStop = false;
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._cbType);
            this._pFilter.Controls.Add(this._tbdpDateToFilter);
            this._pFilter.Controls.Add(this._tbdpDateFromFilter);
            this._pFilter.Controls.Add(this._bFilterDateOneDay);
            this._pFilter.Controls.Add(this._lDateToFilter);
            this._pFilter.Controls.Add(this._lDescription);
            this._pFilter.Controls.Add(this._eDescriptionFilter);
            this._pFilter.Controls.Add(this._lEventSourceFilter);
            this._pFilter.Controls.Add(this._lDateFromFilter);
            this._pFilter.Controls.Add(this._lCGPSourceFilter);
            this._pFilter.Controls.Add(this._lTypeFilter);
            this._pFilter.Controls.Add(this._eEventSourceFilter);
            this._pFilter.Controls.Add(this._eCGPSourceFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 411);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(890, 88);
            this._pFilter.TabIndex = 11;
            // 
            // _cbType
            // 
            this._cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbType.FormattingEnabled = true;
            this._cbType.Location = new System.Drawing.Point(10, 18);
            this._cbType.Name = "_cbType";
            this._cbType.Size = new System.Drawing.Size(224, 21);
            this._cbType.TabIndex = 16;
            // 
            // _tbdpDateToFilter
            // 
            this._tbdpDateToFilter.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateToFilter.ButtonClearDateImage = null;
            this._tbdpDateToFilter.ButtonClearDateText = "";
            this._tbdpDateToFilter.ButtonClearDateWidth = 23;
            this._tbdpDateToFilter.ButtonDateImage = null;
            this._tbdpDateToFilter.ButtonDateText = "";
            this._tbdpDateToFilter.ButtonDateWidth = 23;
            // 
            // 
            // 
            this._tbdpDateToFilter.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._tbdpDateToFilter.DateFormName = "Calendar";
            this._tbdpDateToFilter.LocalizationHelper = null;
            this._tbdpDateToFilter.Location = new System.Drawing.Point(457, 17);
            this._tbdpDateToFilter.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateToFilter.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateToFilter.Name = "_tbdpDateToFilter";
            this._tbdpDateToFilter.ReadOnly = false;
            this._tbdpDateToFilter.Size = new System.Drawing.Size(190, 22);
            this._tbdpDateToFilter.TabIndex = 2;
            this._tbdpDateToFilter.ValidateAfter = 2;
            this._tbdpDateToFilter.ValidationEnabled = false;
            this._tbdpDateToFilter.ValidationError = "";
            this._tbdpDateToFilter.Value = null;
            // 
            // _tbdpDateFromFilter
            // 
            this._tbdpDateFromFilter.BackColor = System.Drawing.Color.Transparent;
            this._tbdpDateFromFilter.ButtonClearDateImage = null;
            this._tbdpDateFromFilter.ButtonClearDateText = "";
            this._tbdpDateFromFilter.ButtonClearDateWidth = 23;
            this._tbdpDateFromFilter.ButtonDateImage = null;
            this._tbdpDateFromFilter.ButtonDateText = "";
            this._tbdpDateFromFilter.ButtonDateWidth = 23;
            // 
            // 
            // 
            this._tbdpDateFromFilter.CustomFormat = "d. M. yyyy HH:mm:ss";
            this._tbdpDateFromFilter.DateFormName = "Calendar";
            this._tbdpDateFromFilter.LocalizationHelper = null;
            this._tbdpDateFromFilter.Location = new System.Drawing.Point(261, 17);
            this._tbdpDateFromFilter.MaximumSize = new System.Drawing.Size(1000, 60);
            this._tbdpDateFromFilter.MinimumSize = new System.Drawing.Size(100, 22);
            this._tbdpDateFromFilter.Name = "_tbdpDateFromFilter";
            this._tbdpDateFromFilter.ReadOnly = false;
            this._tbdpDateFromFilter.Size = new System.Drawing.Size(190, 22);
            this._tbdpDateFromFilter.TabIndex = 1;
            this._tbdpDateFromFilter.ValidateAfter = 2;
            this._tbdpDateFromFilter.ValidationEnabled = false;
            this._tbdpDateFromFilter.ValidationError = "";
            this._tbdpDateFromFilter.Value = null;
            // 
            // _bFilterDateOneDay
            // 
            this._bFilterDateOneDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._bFilterDateOneDay.Location = new System.Drawing.Point(672, 16);
            this._bFilterDateOneDay.Name = "_bFilterDateOneDay";
            this._bFilterDateOneDay.Size = new System.Drawing.Size(60, 23);
            this._bFilterDateOneDay.TabIndex = 3;
            this._bFilterDateOneDay.Text = "One day";
            this._bFilterDateOneDay.UseVisualStyleBackColor = true;
            this._bFilterDateOneDay.Click += new System.EventHandler(this._bFilterDateOneDay_Click);
            // 
            // _lDateToFilter
            // 
            this._lDateToFilter.AutoSize = true;
            this._lDateToFilter.Location = new System.Drawing.Point(470, 5);
            this._lDateToFilter.Name = "_lDateToFilter";
            this._lDateToFilter.Size = new System.Drawing.Size(42, 13);
            this._lDateToFilter.TabIndex = 15;
            this._lDateToFilter.Text = "Date to";
            // 
            // _lDescription
            // 
            this._lDescription.AutoSize = true;
            this._lDescription.Location = new System.Drawing.Point(672, 42);
            this._lDescription.Name = "_lDescription";
            this._lDescription.Size = new System.Drawing.Size(60, 13);
            this._lDescription.TabIndex = 12;
            this._lDescription.Text = "Description";
            // 
            // _eDescriptionFilter
            // 
            this._eDescriptionFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._eDescriptionFilter.Location = new System.Drawing.Point(672, 58);
            this._eDescriptionFilter.Name = "_eDescriptionFilter";
            this._eDescriptionFilter.Size = new System.Drawing.Size(180, 20);
            this._eDescriptionFilter.TabIndex = 6;
            // 
            // _lEventSourceFilter
            // 
            this._lEventSourceFilter.AutoSize = true;
            this._lEventSourceFilter.Location = new System.Drawing.Point(258, 42);
            this._lEventSourceFilter.Name = "_lEventSourceFilter";
            this._lEventSourceFilter.Size = new System.Drawing.Size(70, 13);
            this._lEventSourceFilter.TabIndex = 6;
            this._lEventSourceFilter.Text = "Event source";
            // 
            // _lDateFromFilter
            // 
            this._lDateFromFilter.AutoSize = true;
            this._lDateFromFilter.Location = new System.Drawing.Point(258, 5);
            this._lDateFromFilter.Name = "_lDateFromFilter";
            this._lDateFromFilter.Size = new System.Drawing.Size(53, 13);
            this._lDateFromFilter.TabIndex = 4;
            this._lDateFromFilter.Text = "Date from";
            // 
            // _lCGPSourceFilter
            // 
            this._lCGPSourceFilter.AutoSize = true;
            this._lCGPSourceFilter.Location = new System.Drawing.Point(7, 42);
            this._lCGPSourceFilter.Name = "_lCGPSourceFilter";
            this._lCGPSourceFilter.Size = new System.Drawing.Size(64, 13);
            this._lCGPSourceFilter.TabIndex = 2;
            this._lCGPSourceFilter.Text = "CGP source";
            // 
            // _lTypeFilter
            // 
            this._lTypeFilter.AutoSize = true;
            this._lTypeFilter.Location = new System.Drawing.Point(8, 5);
            this._lTypeFilter.Name = "_lTypeFilter";
            this._lTypeFilter.Size = new System.Drawing.Size(31, 13);
            this._lTypeFilter.TabIndex = 0;
            this._lTypeFilter.Text = "Type";
            // 
            // _eEventSourceFilter
            // 
            this._eEventSourceFilter.Location = new System.Drawing.Point(261, 58);
            this._eEventSourceFilter.Name = "_eEventSourceFilter";
            this._eEventSourceFilter.Size = new System.Drawing.Size(386, 20);
            this._eEventSourceFilter.TabIndex = 5;
            // 
            // _eCGPSourceFilter
            // 
            this._eCGPSourceFilter.Location = new System.Drawing.Point(10, 58);
            this._eCGPSourceFilter.Name = "_eCGPSourceFilter";
            this._eCGPSourceFilter.Size = new System.Drawing.Size(224, 20);
            this._eCGPSourceFilter.TabIndex = 4;
            // 
            // _pControl
            // 
            this._pControl.Controls.Add(this._lTime);
            this._pControl.Controls.Add(this.button1);
            this._pControl.Controls.Add(this._bFilterClear);
            this._pControl.Controls.Add(this._bRunFilter);
            this._pControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pControl.Location = new System.Drawing.Point(0, 499);
            this._pControl.Name = "_pControl";
            this._pControl.Size = new System.Drawing.Size(890, 37);
            this._pControl.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Connection String";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bFilterClear.Location = new System.Drawing.Point(782, 6);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(96, 23);
            this._bFilterClear.TabIndex = 8;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._bRunFilter.Location = new System.Drawing.Point(679, 6);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(96, 23);
            this._bRunFilter.TabIndex = 7;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _lTime
            // 
            this._lTime.AutoSize = true;
            this._lTime.Location = new System.Drawing.Point(352, 11);
            this._lTime.Name = "_lTime";
            this._lTime.Size = new System.Drawing.Size(13, 13);
            this._lTime.TabIndex = 12;
            this._lTime.Text = "0";
            // 
            // EventLogs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 536);
            this.Controls.Add(this._dgValues);
            this.Controls.Add(this._pFilter);
            this.Controls.Add(this._pControl);
            this.Name = "EventLogs";
            this.Text = "EventLogs";
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).EndInit();
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            this._pControl.ResumeLayout(false);
            this._pControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _dgValues;
        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.ComboBox _cbType;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateToFilter;
        private Contal.IwQuick.UI.TextBoxDatePicker _tbdpDateFromFilter;
        private System.Windows.Forms.Button _bFilterDateOneDay;
        private System.Windows.Forms.Label _lDateToFilter;
        private System.Windows.Forms.Label _lDescription;
        private System.Windows.Forms.TextBox _eDescriptionFilter;
        private System.Windows.Forms.Label _lEventSourceFilter;
        private System.Windows.Forms.Label _lDateFromFilter;
        private System.Windows.Forms.Label _lCGPSourceFilter;
        private System.Windows.Forms.Label _lTypeFilter;
        private System.Windows.Forms.TextBox _eEventSourceFilter;
        private System.Windows.Forms.TextBox _eCGPSourceFilter;
        private System.Windows.Forms.Panel _pControl;
        private System.Windows.Forms.Button _bFilterClear;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label _lTime;
    }
}