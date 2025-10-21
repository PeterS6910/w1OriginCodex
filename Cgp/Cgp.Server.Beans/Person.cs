using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using System.Drawing;
using System.Collections;

using Contal.IwQuick;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    [LwSerialize(217)]
    [LwSerializeMode(LwSerializationMode.Selective)]
    [LwSerializeNoParent]
    public class Person : AOrmObjectWithVersion, IOrmObjectWithAlarmInstructions, IComparer
    {
        public const string COLUMNIDPERSON = "IdPerson";
        public const string COLUMNFIRSTNAME = "FirstName";
        public const string COLUMNMIDDLENAME = "MiddleName";
        public const string COLUMNSURNAME = "Surname";
        public const string COLUMNTITLE = "Title";
        public const string COLUMNBIRTHDAY = "Birthday";
        public const string COLUMN_ADDRESS = "Address";
        public const string COLUMNCOMPANY = "Company";
        public const string COLUMNEMAIL = "Email";
        public const string COLUMNPHONENUMBER = "PhoneNumber";
        public const string COLUMNIDENTIFICATION = "Identification";
        public const string COLUMNCARDS = "Cards";
        public const string COLUMNLOGINS = "Logins";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNCKUNIQUE = "CkUnique";
        public const string COLUMNDEPARTMENT = "Department";
        public const string COLUMNEMPLOYEENUMBER = "EmployeeNumber";
        public const string COLUMNRELATIVESUPERIOR = "RelativeSuperior";
        public const string COLUMNRELATIVESUPERIORSPHONENUMBER = "RelativeSuperiorsPhoneNumber";
        public const string COLUMNCOSTCENTER = "CostCenter";
        public const string COLUMNEMPLOYMENTBEGINNINGDATE = "EmploymentBeginningDate";
        public const string COLUMNEMPLOYMENTENDDATE = "EmploymentEndDate";
        public const string COLUMNROLE = "Role";
        public const string COLUMNPHOTOFILENAME = "PhotoFileName";
        public const string COLUMNOTHERINFORMATIONFIELDS = "OtherInformationFields";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMN_PERSONAL_CODE_HASH = "PersonalCodeHash";
        public const string COLUMN_PERSONAL_CODE_LENGTH = "PersonalCodeLength";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdPerson { get; set; }

        public virtual string FirstName
        {
            get { return _firstName; } 
            set { _firstName = value; }
        }

        public virtual string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        public virtual string WholeName
        {
            get { return _wholeName; }
            set { _wholeName = value; }
        }

        public virtual string Surname
        {
            get { return _surname; } 
            set { _surname = value; }
        }

        public virtual string Tiltle
        {
            get { return _title; }
            set { _title = value; }
        }

        public virtual DateTime? Birthday { get; set; }

        public virtual string Company
        {
            get { return _company; } 
            set { _company = value; }
        }

        public virtual string Address 
        {
            get { return _address; }
            set { _address = value; }
        }

        public virtual string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public virtual string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public virtual string Identification
        {
            get { return _identification; }
            set { _identification = value; }
        }

        public virtual string Description { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Login> Logins { get; set; }

        public virtual bool SynchronizedWithTimetec { get; set; }

        public virtual byte ObjectType
        {
            get
            {
                return (byte)GetObjectType();
            }
            set
            {
            }
        }

        public virtual Guid CkUnique
        {
            get { return _ckUnique; } 
            set { _ckUnique = value; }
        }

        private UserFoldersStructure _department;
        private string _identification;
        private string _firstName;
        private string _middleName;
        private string _wholeName;
        private string _title;
        private string _phoneNumber;
        private string _email;
        private string _surname;
        private string _address;
        private string _employeeNumber;
        private string _role;
        private string _company;
        private string _relativeSuperior;
        private string _relativeSuperiorsPhoneNumber;
        private string _costCenter;
        private DateTime? _employmentBeginningDate;
        private DateTime? _employmentEndDate;
        private Guid _ckUnique;

        public virtual UserFoldersStructure Department {
            get { return _department; }
            set { _department = value; }
        }

        public virtual string EmployeeNumber
        {
            get { return _employeeNumber; }
            set { _employeeNumber = value; }
        }

        public virtual string RelativeSuperior
        {
            get { return _relativeSuperior; }
            set { _relativeSuperior = value; }
        }

        public virtual string RelativeSuperiorsPhoneNumber
        {
            get { return _relativeSuperiorsPhoneNumber; }
            set { _relativeSuperiorsPhoneNumber = value; }
        }

        public virtual string CostCenter
        {
            get { return _costCenter; }
            set { _costCenter = value; }
        }

        public virtual DateTime? EmploymentBeginningDate
        {
            get { return _employmentBeginningDate; }
            set { _employmentBeginningDate = value; }
        }

        public virtual DateTime? EmploymentEndDate
        {
            get { return _employmentEndDate; }
            set { _employmentEndDate = value; }
        }

        public virtual string Role
        {
            get { return _role; } 
            set { _role = value; }
        }

        public virtual string PhotoFileName { get; set; }
        public virtual string LocalAlarmInstruction { get; set; }

        [LwSerialize]
        public virtual string PersonalCodeHash { get; set; }

        public virtual byte PersonalCodeLength { get; set; }

        public Person()
        {
            _ckUnique = Guid.NewGuid();
        }

        public Person(ImportPersonData importPersonData)
            : this()
        {
            _identification = importPersonData.Identification;
            _firstName = importPersonData.FirstName;
            _middleName = importPersonData.MiddleName;
            _surname = importPersonData.Surname;
            _wholeName = importPersonData.FirstName + " " + importPersonData.Surname;
            _address = importPersonData.Address;
            _title = importPersonData.Title;
            _phoneNumber = importPersonData.PhoneNumber;
            _email = importPersonData.Email;
            _employeeNumber = importPersonData.EmployeeNumber;
            _company = importPersonData.Company;
            _role = importPersonData.Role;
            _relativeSuperior = importPersonData.RelativeSuperior;
            _relativeSuperiorsPhoneNumber = importPersonData.RelativeSuperiorsPhoneNumber;
            _costCenter = importPersonData.CostCenter;
            _employmentBeginningDate = importPersonData.EmploymentBeginningDate;
            _employmentEndDate = importPersonData.EmploymentEndDate;
        }

        public override string ToString()
        {
            return FirstName + " " + (Surname ?? "");
        }

        public override bool Compare(object obj)
        {
            var person = obj as Person;

            return 
                person != null && 
                person.IdPerson == IdPerson;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (Birthday.ToString().ToLower().Contains(expression)) return true;
            if (FirstName.ToLower().Contains(expression)) return true;
            if (Surname.ToLower().Contains(expression)) return true;
            if (Identification != null)
            {
                if (Identification.ToLower().Contains(expression)) return true;
            }
            if (Description != null)
            {
                if (Description.ToLower().Contains(expression)) return true;
            }
            return false;
        }

        public override string GetIdString()
        {
            return IdPerson.ToString();
        }

        public override object GetId()
        {
            return IdPerson;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new PersonModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.Person;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        public virtual void PrepareToSend()
        {
            PersonalCodeHash = PersonalCodeHash ?? string.Empty;
        }

        #region IComparer Members

        public virtual int Compare(object x, object y)
        {
            var pX = x as Person;
            var pY = y as Person;

            if (pX == null && pY == null)
                return 0;

            if (pX != null && pY == null)
                return 1;

            if (pX == null && pY != null)
                return -1;

            if (pX.WholeName == null && pY.WholeName == null)
                return 0;

            if (pX.WholeName != null && pY.WholeName == null)
                return 1;

            if (pX.WholeName == null && pY.WholeName != null)
                return -1;

            return (pX.WholeName.CompareTo(pY.WholeName));
        }

        #endregion
    }

    [Serializable]
    public class PersonShort : IShortObject
    {
        public const string COLUMNIDPERSON = "IdPerson";
        public const string COLUMNFIRSTNAME = "FirstName";
        public const string COLUMNSURNAME = "Surname";
        public const string COLUMNBIRTHDAY = "Birthday";
        public const string COLUMNIDENTIFICATION = "Identification";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNDEPARTMENT = "Department";
        public const string COLUMN_SYMBOL = "Symbol";

        public const string COLUMN_TIMETEC_SYNC = "TimetecSync";

        public Guid IdPerson { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public DateTime? Birthday { get; set; }
        public string Identification { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public Image Symbol { get; set; }
        public string TimetecSync { get; set; }

        public PersonShort(Person person)
        {
            IdPerson = person.IdPerson;
            FirstName = person.FirstName;
            MiddleName = person.MiddleName;
            Surname = person.Surname;
            Title = person.Tiltle;
            Birthday = person.Birthday;
            Identification = person.Identification;
            Description = person.Description;
            Department = person.Department?.FolderName;
            TimetecSync = person.SynchronizedWithTimetec.ToString();
        }

        public override string ToString()
        {
            return FirstName + " " + Surname;
        }

        #region IShortObject Members

        public object Id { get { return IdPerson; } }

        public string Name { get { return FirstName + " " + Surname; } }

        public ObjectType ObjectType { get { return ObjectType.Person; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class PersonModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.Person; } }

        public PersonModifyObj(Person person)
        {
            Id = person.IdPerson;
            FullName = person.ToString();
            Description = person.Description;
        }
    }

    [Serializable]
    public enum CSVImportType : byte
    {

        OverwriteOnConflict = 0,
        OverwriteNonEmptyData = 1,
        IgnoreOnConflict = 2,
        AddAll = 3
    }

    [Serializable]
    public enum CSVImportResult : byte
    {
        Added = 0,
        Overwritten = 1,
        IgnoreConflict = 2,
        Failed = 3,
        InvalidCardNumberLength = 4,
        InvalidPersonalIdFormat = 5,
        NonExistingPersonalId = 6,
        PinNotSecure = 7,
        PinInvalidFormat = 8,
        PersonIdIsNotEntry = 9,
        MorePersonsWithSameID = 10
    }

    [Serializable]
    public class CSVImportPerson
    {
        Guid _idPerson;
        string _fullName;
        CSVImportResult _importResult;

        public Guid IdPerson { get { return _idPerson; } }
        public string FullName { get { return _fullName; } }
        public CSVImportResult ImportResult { get { return _importResult; } }

        public CSVImportPerson(Guid idPerson, string fullName, CSVImportResult importResult)
        {
            _idPerson = idPerson;
            _fullName = fullName;
            _importResult = importResult;
        }
    }

    [Serializable]
    public class ImportPersonData
    {
        private string _identification;
        private string _firstName;
        private string _middleName;
        private string _surname;
        private string _address;
        private string _title;
        private string _phoneNumber;
        private string _email;
        private string _employeeNumber;
        private string _company;
        private string _role;
        private string _relativeSuperior;
        private string _relativeSuperiorsPhoneNumber;
        private string _costCenter;
        private DateTime? _employmentBeginningDate;
        private DateTime? _employmentEndDate;
        private string _department;

        public string Identification { get { return _identification; } }
        public string FirstName { get { return _firstName; } }
        public string MiddleName { get { return _middleName; } }
        public string Surname { get { return _surname; } }
        public string Address { get { return _address; } }
        public string Title { get { return _title; } }
        public string PhoneNumber { get { return _phoneNumber; } }
        public string Email { get { return _email; } }
        public string EmployeeNumber { get { return _employeeNumber; } }
        public string Company { get { return _company; } }
        public string Role { get { return _role; } }
        public string CostCenter { get { return _costCenter; } }
        public string RelativeSuperior { get { return _relativeSuperior; } }
        public string RelativeSuperiorsPhoneNumber { get { return _relativeSuperiorsPhoneNumber; } }
        public DateTime? EmploymentBeginningDate { get { return _employmentBeginningDate; } }
        public DateTime? EmploymentEndDate { get { return _employmentEndDate; } }
        public string Department { get { return _department; } }

        public ImportPersonData(string identification, 
            string firstName, 
            string middleName, 
            string surname,
            string address,
            string title, 
            string phoneNumber, 
            string email, 
            string employeeNumber, 
            string company, 
            string role,
            string department, 
            string costCenter,
            string relativeSuperior, 
            string relativeSuperiorsPhoneNumber, 
            DateTime? employmentBeginningDate, 
            DateTime? employmentEndDate)
        {
            _identification = identification;
            _firstName = firstName;
            _middleName = middleName;
            _surname = surname;
            _address = address;
            _title = title;
            _phoneNumber = phoneNumber;
            _email = email;
            _employeeNumber = employeeNumber;
            _company = company;
            _role = role;
            _costCenter = costCenter;
            _relativeSuperior = relativeSuperior;
            _relativeSuperiorsPhoneNumber = relativeSuperiorsPhoneNumber;
            _employmentBeginningDate = employmentBeginningDate;
            _employmentEndDate = employmentEndDate;
            _department = department;
        }
    }

    public class ImportedPersonCountChangedHandler : ARemotingCallbackHandler
    {
        private static volatile ImportedPersonCountChangedHandler _singleton;
        private static object _syncRoot = new object();

        private Action<Guid, int> _importedPersonCountChanged;

        public static ImportedPersonCountChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new ImportedPersonCountChangedHandler();
                    }

                return _singleton;
            }
        }

        public ImportedPersonCountChangedHandler()
            : base("ImportedPersonCountChangedHandler")
        {
        }

        public void RegisterImportedPersonCountChanged(Action<Guid, int> importedPersonCountChanged)
        {
            _importedPersonCountChanged += importedPersonCountChanged;
        }

        public void UnregisterImportedPersonCountChanged(Action<Guid, int> importedPersonCountChanged)
        {
            _importedPersonCountChanged -= importedPersonCountChanged;
        }

        public void RunEvent(Guid formIdentification, int percent)
        {
            if (_importedPersonCountChanged != null)
                _importedPersonCountChanged(formIdentification, percent);
        }
    }

    [Serializable]
    public class BinaryPhoto
    {
        private byte[] _binaryData;
        private string _extension;

        public byte[] BinaryData { get { return _binaryData; } }
        public string Extension { get { return _extension; } }

        public BinaryPhoto(byte[] data, string extension)
        {
            _binaryData = data;
            _extension = extension;
        }
    }
}
