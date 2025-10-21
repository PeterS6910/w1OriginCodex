namespace Contal.Cgp.Client
{
    partial class UsersAccount
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
            this._pControl = new System.Windows.Forms.Panel();
            this._bDelete = new System.Windows.Forms.Button();
            this._bEdit = new System.Windows.Forms.Button();
            this._bInsert = new System.Windows.Forms.Button();
            this._pEdit = new System.Windows.Forms.Panel();
            this._eDate = new System.Windows.Forms.MaskedTextBox();
            this._lDate = new System.Windows.Forms.Label();
            this._lNumber = new System.Windows.Forms.Label();
            this._lSurname = new System.Windows.Forms.Label();
            this._lName = new System.Windows.Forms.Label();
            this._eNumber = new System.Windows.Forms.TextBox();
            this._eSurname = new System.Windows.Forms.TextBox();
            this._eName = new System.Windows.Forms.TextBox();
            this._bCancel = new System.Windows.Forms.Button();
            this._bOk = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this._pFilter = new System.Windows.Forms.Panel();
            this._bFilterClear = new System.Windows.Forms.Button();
            this._bRunFilter = new System.Windows.Forms.Button();
            this._lNumberFilter = new System.Windows.Forms.Label();
            this._lDateFilter = new System.Windows.Forms.Label();
            this._lSurnameFilter = new System.Windows.Forms.Label();
            this._lNameFilter = new System.Windows.Forms.Label();
            this._eNumberFilter = new System.Windows.Forms.TextBox();
            this._mDateFilter = new System.Windows.Forms.MaskedTextBox();
            this._eSurnameFilter = new System.Windows.Forms.TextBox();
            this._eNameFilter = new System.Windows.Forms.TextBox();
            this._dgValues = new System.Windows.Forms.DataGridView();
            this._pControl.SuspendLayout();
            this._pEdit.SuspendLayout();
            this.panel3.SuspendLayout();
            this._pFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).BeginInit();
            this.SuspendLayout();
            // 
            // _pControl
            // 
            this._pControl.Controls.Add(this._bDelete);
            this._pControl.Controls.Add(this._bEdit);
            this._pControl.Controls.Add(this._bInsert);
            this._pControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pControl.Location = new System.Drawing.Point(0, 411);
            this._pControl.Name = "_pControl";
            this._pControl.Size = new System.Drawing.Size(700, 37);
            this._pControl.TabIndex = 0;
            // 
            // _bDelete
            // 
            this._bDelete.Location = new System.Drawing.Point(174, 6);
            this._bDelete.Name = "_bDelete";
            this._bDelete.Size = new System.Drawing.Size(75, 23);
            this._bDelete.TabIndex = 2;
            this._bDelete.Text = "Delete";
            this._bDelete.UseVisualStyleBackColor = true;
            this._bDelete.Click += new System.EventHandler(this._bDelete_Click);
            // 
            // _bEdit
            // 
            this._bEdit.Location = new System.Drawing.Point(93, 6);
            this._bEdit.Name = "_bEdit";
            this._bEdit.Size = new System.Drawing.Size(75, 23);
            this._bEdit.TabIndex = 1;
            this._bEdit.Text = "Edit";
            this._bEdit.UseVisualStyleBackColor = true;
            this._bEdit.Click += new System.EventHandler(this._bEdit_Click);
            // 
            // _bInsert
            // 
            this._bInsert.Location = new System.Drawing.Point(12, 6);
            this._bInsert.Name = "_bInsert";
            this._bInsert.Size = new System.Drawing.Size(75, 23);
            this._bInsert.TabIndex = 0;
            this._bInsert.Text = "Insert";
            this._bInsert.UseVisualStyleBackColor = true;
            this._bInsert.Click += new System.EventHandler(this._bInsert_Click);
            // 
            // _pEdit
            // 
            this._pEdit.Controls.Add(this._eDate);
            this._pEdit.Controls.Add(this._lDate);
            this._pEdit.Controls.Add(this._lNumber);
            this._pEdit.Controls.Add(this._lSurname);
            this._pEdit.Controls.Add(this._lName);
            this._pEdit.Controls.Add(this._eNumber);
            this._pEdit.Controls.Add(this._eSurname);
            this._pEdit.Controls.Add(this._eName);
            this._pEdit.Controls.Add(this._bCancel);
            this._pEdit.Controls.Add(this._bOk);
            this._pEdit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pEdit.Location = new System.Drawing.Point(0, 338);
            this._pEdit.Name = "_pEdit";
            this._pEdit.Size = new System.Drawing.Size(700, 73);
            this._pEdit.TabIndex = 1;
            // 
            // _eDate
            // 
            this._eDate.Location = new System.Drawing.Point(262, 21);
            this._eDate.Mask = "00/00/0000";
            this._eDate.Name = "_eDate";
            this._eDate.Size = new System.Drawing.Size(100, 20);
            this._eDate.TabIndex = 2;
            this._eDate.ValidatingType = typeof(System.DateTime);
            this._eDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditKeyDown);
            // 
            // _lDate
            // 
            this._lDate.AutoSize = true;
            this._lDate.Location = new System.Drawing.Point(260, 5);
            this._lDate.Name = "_lDate";
            this._lDate.Size = new System.Drawing.Size(30, 13);
            this._lDate.TabIndex = 9;
            this._lDate.Text = "Date";
            // 
            // _lNumber
            // 
            this._lNumber.AutoSize = true;
            this._lNumber.Location = new System.Drawing.Point(368, 5);
            this._lNumber.Name = "_lNumber";
            this._lNumber.Size = new System.Drawing.Size(44, 13);
            this._lNumber.TabIndex = 8;
            this._lNumber.Text = "Number";
            // 
            // _lSurname
            // 
            this._lSurname.AutoSize = true;
            this._lSurname.Location = new System.Drawing.Point(130, 5);
            this._lSurname.Name = "_lSurname";
            this._lSurname.Size = new System.Drawing.Size(49, 13);
            this._lSurname.TabIndex = 6;
            this._lSurname.Text = "Surname";
            // 
            // _lName
            // 
            this._lName.AutoSize = true;
            this._lName.Location = new System.Drawing.Point(6, 5);
            this._lName.Name = "_lName";
            this._lName.Size = new System.Drawing.Size(35, 13);
            this._lName.TabIndex = 5;
            this._lName.Text = "Name";
            // 
            // _eNumber
            // 
            this._eNumber.Location = new System.Drawing.Point(368, 21);
            this._eNumber.Name = "_eNumber";
            this._eNumber.Size = new System.Drawing.Size(100, 20);
            this._eNumber.TabIndex = 3;
            this._eNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditKeyDown);
            // 
            // _eSurname
            // 
            this._eSurname.Location = new System.Drawing.Point(133, 21);
            this._eSurname.Name = "_eSurname";
            this._eSurname.Size = new System.Drawing.Size(123, 20);
            this._eSurname.TabIndex = 1;
            this._eSurname.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditKeyDown);
            // 
            // _eName
            // 
            this._eName.Location = new System.Drawing.Point(9, 21);
            this._eName.Name = "_eName";
            this._eName.Size = new System.Drawing.Size(121, 20);
            this._eName.TabIndex = 0;
            this._eName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditKeyDown);
            // 
            // _bCancel
            // 
            this._bCancel.Location = new System.Drawing.Point(493, 38);
            this._bCancel.Name = "_bCancel";
            this._bCancel.Size = new System.Drawing.Size(75, 23);
            this._bCancel.TabIndex = 5;
            this._bCancel.Text = "Cancel";
            this._bCancel.UseVisualStyleBackColor = true;
            this._bCancel.Click += new System.EventHandler(this._bCancel_Click);
            // 
            // _bOk
            // 
            this._bOk.Location = new System.Drawing.Point(493, 9);
            this._bOk.Name = "_bOk";
            this._bOk.Size = new System.Drawing.Size(75, 23);
            this._bOk.TabIndex = 4;
            this._bOk.Text = "OK";
            this._bOk.UseVisualStyleBackColor = true;
            this._bOk.Click += new System.EventHandler(this._bOk_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._dgValues);
            this.panel3.Controls.Add(this._pFilter);
            this.panel3.Controls.Add(this._pEdit);
            this.panel3.Controls.Add(this._pControl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(700, 448);
            this.panel3.TabIndex = 2;
            // 
            // _pFilter
            // 
            this._pFilter.Controls.Add(this._bFilterClear);
            this._pFilter.Controls.Add(this._bRunFilter);
            this._pFilter.Controls.Add(this._lNumberFilter);
            this._pFilter.Controls.Add(this._lDateFilter);
            this._pFilter.Controls.Add(this._lSurnameFilter);
            this._pFilter.Controls.Add(this._lNameFilter);
            this._pFilter.Controls.Add(this._eNumberFilter);
            this._pFilter.Controls.Add(this._mDateFilter);
            this._pFilter.Controls.Add(this._eSurnameFilter);
            this._pFilter.Controls.Add(this._eNameFilter);
            this._pFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pFilter.Location = new System.Drawing.Point(0, 292);
            this._pFilter.Name = "_pFilter";
            this._pFilter.Size = new System.Drawing.Size(700, 46);
            this._pFilter.TabIndex = 1;
            // 
            // _bFilterClear
            // 
            this._bFilterClear.Location = new System.Drawing.Point(573, 17);
            this._bFilterClear.Name = "_bFilterClear";
            this._bFilterClear.Size = new System.Drawing.Size(75, 23);
            this._bFilterClear.TabIndex = 9;
            this._bFilterClear.Text = "Clear";
            this._bFilterClear.UseVisualStyleBackColor = true;
            this._bFilterClear.Click += new System.EventHandler(this._bFilterClear_Click);
            // 
            // _bRunFilter
            // 
            this._bRunFilter.Location = new System.Drawing.Point(492, 17);
            this._bRunFilter.Name = "_bRunFilter";
            this._bRunFilter.Size = new System.Drawing.Size(75, 23);
            this._bRunFilter.TabIndex = 8;
            this._bRunFilter.Text = "Filter";
            this._bRunFilter.UseVisualStyleBackColor = true;
            this._bRunFilter.Click += new System.EventHandler(this._bRunFilter_Click);
            // 
            // _lNumberFilter
            // 
            this._lNumberFilter.AutoSize = true;
            this._lNumberFilter.Location = new System.Drawing.Point(367, 4);
            this._lNumberFilter.Name = "_lNumberFilter";
            this._lNumberFilter.Size = new System.Drawing.Size(44, 13);
            this._lNumberFilter.TabIndex = 7;
            this._lNumberFilter.Text = "Number";
            // 
            // _lDateFilter
            // 
            this._lDateFilter.AutoSize = true;
            this._lDateFilter.Location = new System.Drawing.Point(261, 5);
            this._lDateFilter.Name = "_lDateFilter";
            this._lDateFilter.Size = new System.Drawing.Size(30, 13);
            this._lDateFilter.TabIndex = 6;
            this._lDateFilter.Text = "Date";
            // 
            // _lSurnameFilter
            // 
            this._lSurnameFilter.AutoSize = true;
            this._lSurnameFilter.Location = new System.Drawing.Point(132, 5);
            this._lSurnameFilter.Name = "_lSurnameFilter";
            this._lSurnameFilter.Size = new System.Drawing.Size(49, 13);
            this._lSurnameFilter.TabIndex = 5;
            this._lSurnameFilter.Text = "Surname";
            // 
            // _lNameFilter
            // 
            this._lNameFilter.AutoSize = true;
            this._lNameFilter.Location = new System.Drawing.Point(8, 5);
            this._lNameFilter.Name = "_lNameFilter";
            this._lNameFilter.Size = new System.Drawing.Size(35, 13);
            this._lNameFilter.TabIndex = 4;
            this._lNameFilter.Text = "Name";
            // 
            // _eNumberFilter
            // 
            this._eNumberFilter.Location = new System.Drawing.Point(368, 20);
            this._eNumberFilter.Name = "_eNumberFilter";
            this._eNumberFilter.Size = new System.Drawing.Size(100, 20);
            this._eNumberFilter.TabIndex = 3;
            this._eNumberFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _mDateFilter
            // 
            this._mDateFilter.Location = new System.Drawing.Point(261, 21);
            this._mDateFilter.Mask = "00/00/0000";
            this._mDateFilter.Name = "_mDateFilter";
            this._mDateFilter.Size = new System.Drawing.Size(100, 20);
            this._mDateFilter.TabIndex = 2;
            this._mDateFilter.ValidatingType = typeof(System.DateTime);
            this._mDateFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _eSurnameFilter
            // 
            this._eSurnameFilter.Location = new System.Drawing.Point(135, 21);
            this._eSurnameFilter.Name = "_eSurnameFilter";
            this._eSurnameFilter.Size = new System.Drawing.Size(120, 20);
            this._eSurnameFilter.TabIndex = 1;
            this._eSurnameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
            // 
            // _eNameFilter
            // 
            this._eNameFilter.Location = new System.Drawing.Point(8, 21);
            this._eNameFilter.Name = "_eNameFilter";
            this._eNameFilter.Size = new System.Drawing.Size(121, 20);
            this._eNameFilter.TabIndex = 0;
            this._eNameFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterKeyDown);
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
            this._dgValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dgValues.Size = new System.Drawing.Size(700, 292);
            this._dgValues.TabIndex = 0;
            // 
            // UsersAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 448);
            this.Controls.Add(this.panel3);
            this.Name = "UsersAccount";
            this.Text = "UsersAccount";
            this._pControl.ResumeLayout(false);
            this._pEdit.ResumeLayout(false);
            this._pEdit.PerformLayout();
            this.panel3.ResumeLayout(false);
            this._pFilter.ResumeLayout(false);
            this._pFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgValues)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pControl;
        private System.Windows.Forms.Panel _pEdit;
        private System.Windows.Forms.Button _bDelete;
        private System.Windows.Forms.Button _bEdit;
        private System.Windows.Forms.Button _bInsert;
        private System.Windows.Forms.TextBox _eNumber;
        private System.Windows.Forms.TextBox _eSurname;
        private System.Windows.Forms.TextBox _eName;
        private System.Windows.Forms.Button _bCancel;
        private System.Windows.Forms.Button _bOk;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label _lName;
        private System.Windows.Forms.Label _lSurname;
        private System.Windows.Forms.Label _lNumber;
        private System.Windows.Forms.Label _lDate;
        private System.Windows.Forms.DataGridView _dgValues;
        private System.Windows.Forms.MaskedTextBox _eDate;
        private System.Windows.Forms.Panel _pFilter;
        private System.Windows.Forms.Button _bRunFilter;
        private System.Windows.Forms.Label _lNumberFilter;
        private System.Windows.Forms.Label _lDateFilter;
        private System.Windows.Forms.Label _lSurnameFilter;
        private System.Windows.Forms.Label _lNameFilter;
        private System.Windows.Forms.TextBox _eNumberFilter;
        private System.Windows.Forms.MaskedTextBox _mDateFilter;
        private System.Windows.Forms.TextBox _eSurnameFilter;
        private System.Windows.Forms.TextBox _eNameFilter;
        private System.Windows.Forms.Button _bFilterClear;
    }
}