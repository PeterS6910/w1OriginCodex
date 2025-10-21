using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Contal.Cgp.Client
{
    public partial class EditPersonsForm : CgpTranslateForm
    {
        private bool _ok = false;
        private bool _insert = false;
        Cgp.RemotingCommon.IPerson _person;

        public EditPersonsForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
        }

        #region Properties
        public string Name
        {
            get { return _eName.Text; }
            set { _eName.Text = value; }
        }
        public string Surname
        {
            get { return _eSurname.Text; }
            set { _eSurname.Text = value; }
        }
        public string Number
        {
            get { return _eNumber.Text; }
            set { _eNumber.Text = value; }
        }
        public string Date
        {
            get { return _eDate.Text; }
            set { _eDate.Text = value; }
        }
        public bool Ok
        {
            get { return _ok; }
            set { _ok = value; }
        }
        public Cgp.RemotingCommon.IPerson Person
        {
            set 
            {
                if (value != null)
                {
                    _person = value;
                    _eName.Text = _person.Name;
                    _eSurname.Text = _person.Surname;
                    _eNumber.Text = _person.Identification;
                    if (_person.Birthday != null)
                    {
                        _eDate.Text = _person.Birthday.Value.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        _eDate.Text = string.Empty;
                    }
                }
            }
        }
        public bool Insert
        {
            set { _insert = value; }
        }
        #endregion

        private bool IsVariableOk()
        {
            if (string.IsNullOrEmpty(_eName.Text))
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("EditPersonsForm.InsertName", "Insert name"));
                _eName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(_eSurname.Text))
            {
                Contal.IwQuick.UI.Dialog.Error(GetString("EditPersonsForm.InsertSurname", "Insert surname"));
                _eSurname.Focus();
                return false;
            }
            if (!string.IsNullOrEmpty(_eDate.Text))
            {
                DateTime dt;
                if (!DateTime.TryParse(_eDate.Text, out dt))
                {
                    Contal.IwQuick.UI.Dialog.Error(GetString("EditPersonsForm.InsertDate", "Insert date"));
                    _eDate.Focus();
                    return false;
                }
                else
                {
                    if (dt < DateTime.Parse("1.1.1753"))
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("EditPersonsForm.InsertDateRange", "Date is out of range"));
                        _eDate.Focus();
                        return false;
                    }
                }
            }
            return true;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _ok = false;
            Close();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (!IsVariableOk())
                return;
            SavePerson();
            _ok = true;
            Close();
        }

        private void SavePerson()
        {
            _person.Name = _eName.Text;
            _person.Surname = _eSurname.Text;
            _person.Identification = _eNumber.Text;
            if (_eDate.Text == string.Empty)
            {
                _person.Birthday = null;
            }
            else
            {
                DateTime dt;
                if (DateTime.TryParse(_eDate.Text, out dt))
                {
                    _person.Birthday = dt;
                }
                else
                {
                    Contal.IwQuick.UI.Dialog.Error("Data fail.");
                    return;
                }
            }

            if (_insert)
            {
                if (!CgpClient.Singleton.OrmRemotingProvider.Persons.Insert(_person))
                {
                    Contal.IwQuick.UI.Dialog.Error("Insert fail.");
                }
            }
            else
            {
                if (!CgpClient.Singleton.OrmRemotingProvider.Persons.Update(_person))
                {
                    Contal.IwQuick.UI.Dialog.Error("Update fail.");
                }
            }
            
            
            
            //if (!CgpClient.Singleton.OrmRemotingProvider.Persons.Insert(newPerson))
        }
    }
}


/*
 * private void BeforeEdit()
        {
            if (_edit)
            {
                _editPerson = (Cgp.RemotingCommon.IPerson)_bs.List[_bs.Position];
                EditPersonsForm editPersonForm = new EditPersonsForm();
                editPersonForm.Name = _editPerson.Name;
                editPersonForm.Surname = _editPerson.Surname;
                if (_editPerson.Birthday != null)
                {
                    editPersonForm.Date = _editPerson.Birthday.Value.ToString("dd.MM.yyyy");
                }
                else
                {
                    editPersonForm.Date = string.Empty;
                }
                editPersonForm.Number = _editPerson.Identification;
                editPersonForm.ShowDialog();

                if (editPersonForm.Ok)
                {
                    if (CgpClient.Singleton.OrmRemotingProvider == null || CgpClient.Singleton.OrmRemotingProvider.Persons == null)
                    {
                        //throw new Exception
                        Contal.IwQuick.UI.Dialog.Error(GetString("PersonsForm.ErrorConnection", "Connection to server not available"));
                        return;
                    }

                    _editPerson.Name = editPersonForm.Name;
                    _editPerson.Surname = editPersonForm.Surname;
                    _editPerson.Identification = editPersonForm.Number;
                    if (string.IsNullOrEmpty(editPersonForm.Date))
                    {
                        _editPerson.Birthday = null;
                    }
                    else
                    {
                        DateTime dt;
                        if (DateTime.TryParse(editPersonForm.Date, out dt))
                        {
                            _editPerson.Birthday = dt;
                        }
                        else
                        {
                            Contal.IwQuick.UI.Dialog.Error(GetString("PersonsForm.ErrorConnection", "Edit fail."));
                            return;
                        }                        
                    }
                    if (!CgpClient.Singleton.OrmRemotingProvider.Persons.Update(_editPerson))
                    {
                        Contal.IwQuick.UI.Dialog.Error(GetString("PersonsForm.ErrorConnection", "Edit fail."));
                    }
                    else
                    {
                        ShowData(null, null, null);
                    }
                }
            }
            else
            {
                EditPersonsForm editPersonForm = new EditPersonsForm();
                editPersonForm.Name = string.Empty;
                editPersonForm.Surname = string.Empty;
                editPersonForm.Date = string.Empty;
                editPersonForm.Number = string.Empty;
                editPersonForm.ShowDialog();

                if (editPersonForm.Ok)
                {
                    if (CgpClient.Singleton.OrmRemotingProvider == null || CgpClient.Singleton.OrmRemotingProvider.Persons == null)
                    {
                        //throw new Exception
                        Contal.IwQuick.UI.Dialog.Error(GetString("PersonsForm.ErrorConnection", "Connection to server not available"));
                        return;
                    }

                    Cgp.RemotingCommon.IPerson newPerson = CgpClient.Singleton.OrmRemotingProvider.Persons.CreatePerson();
                    if (editPersonForm.Date != string.Empty)
                    {
                        DateTime dt;
                        if (DateTime.TryParse(editPersonForm.Date, out dt))
                        {
                            newPerson.Birthday = dt;
                        }
                        else
                        {
                            Contal.IwQuick.UI.Dialog.Error("Insert fail.");
                            return;
                        }
                    }
                    newPerson.Identification = editPersonForm.Number;
                    newPerson.Name = editPersonForm.Name;
                    newPerson.Surname = editPersonForm.Surname;
                    if (!CgpClient.Singleton.OrmRemotingProvider.Persons.Insert(newPerson))
                    {
                        Contal.IwQuick.UI.Dialog.Error("Insert fail.");
                    }
                    else
                    {
                        ShowData(null, null, null);
                    }
                }
            }
        }
*/