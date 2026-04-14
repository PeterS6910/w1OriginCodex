using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;

namespace Contal.Cgp.NCAS.Client
{
    public enum ShowOptionsEditForm : byte
    {
        Insert = 0,
        Edit = 1,
        InsertDialog = 2
    }

    public abstract class ACgpEditForm<T> : MdiChildForm
    {
        protected T _editingObject;
        private ShowOptionsEditForm _showOption;
        private bool _wasChangedValues;
        private bool _isSetValues = false;

        protected bool Insert
        {
            get { return _showOption == ShowOptionsEditForm.Insert || _showOption == ShowOptionsEditForm.InsertDialog; }
        }

        protected bool IsSetValues
        {
            get { return _isSetValues; }
        }

        public ACgpEditForm(T editingObject, ShowOptionsEditForm showOption)
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            _editingObject = editingObject;
            _showOption = showOption;

            if (_showOption != ShowOptionsEditForm.InsertDialog)
                MdiParent = CgpClientMainForm.Singleton;

            FormOnEnter += new Contal.IwQuick.DFromTToVoid<Form>(EditForm_Enter);
            FormOnLeave += new Contal.IwQuick.DFromVoidToVoid(EditForm_Leave);

            AutoScaleMode = AutoScaleMode.Dpi;
        }

        protected abstract void BeforeInsert();

        protected abstract void BeforeEdit();

        private void EditForm_Enter(Form form)
        {
            if (!_isSetValues)
            {
                if (_showOption == ShowOptionsEditForm.Insert)
                {
                    BeforeInsert();
                    CgpClientMainForm.Singleton.AddToOpenWindows(this);
                }
                else if (_showOption == ShowOptionsEditForm.Edit)
                {
                    BeforeEdit();    
                    CgpClientMainForm.Singleton.AddToOpenWindows(this, _editingObject.ToString());
                }

                SetValues();
                _wasChangedValues = false;
                _isSetValues = true;
            }
            
            if (_showOption != ShowOptionsEditForm.InsertDialog)
                CgpClientMainForm.Singleton.SetActOpenWindow(this);
        }

        protected abstract void SetValues();

        private void EditForm_Leave()
        {
            if (_showOption != ShowOptionsEditForm.InsertDialog)
                CgpClientMainForm.Singleton.ModifyOpenWindowImage(this);
        }

        protected void Cancel_Click()
        {
            Close();
        }

        private bool _isOkClose = false;
        protected void Ok_Click()
        {
            if (!_wasChangedValues && !(Insert))
            {
                if (ControlValues())
                {
                    _isOkClose = true;
                    Close();
                }
            }
            else if (ControlValues())
            {
                if (SaveToDatabase())
                {
                    _isOkClose = true;
                    Close();
                }
            }
        }

        protected abstract bool ControlValues();

        protected abstract bool SaveToDatabase();

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            
            if (_showOption == ShowOptionsEditForm.Insert)
                AfterInsert();
            else if (_showOption == ShowOptionsEditForm.Edit)
                AfterEdit();

            if (_isOkClose)
            {
                CgpClientMainForm.Singleton.AddToRecentList(_editingObject);
            }

            if (_showOption != ShowOptionsEditForm.InsertDialog)
                CgpClientMainForm.Singleton.RemoveFromOpenWindows(this);

            if (!Insert)
                EditEnd();
        }

        protected abstract void EditEnd();

        protected abstract void AfterInsert();

        protected abstract void AfterEdit();

        protected virtual void EditTextChanger(object sender, EventArgs e)
        {
            _wasChangedValues = true;
        }

        protected void ConnectionLost()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                Close();
        }

        public bool ShowInsertDialog(ref T outObj)
        {
            if (_showOption != ShowOptionsEditForm.InsertDialog)
                return false ;

            ShowDialog();

            if (_isOkClose)
            {
                outObj = _editingObject;
                return true;
            }

            return false;
        }
    }
}
