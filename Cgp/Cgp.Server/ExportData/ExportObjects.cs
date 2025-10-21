using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.Server.ExportData
{
    public class RefACLCR : IRefExportObject
    {
        public string CardReaderName { get; private set; }
        public void FillDataRow(int ColumnIndex,ref DataRow row)
        {
            row[ColumnIndex] = CardReaderName;
        }

        public RefACLCR(string cardReaderName)
        {
            CardReaderName = cardReaderName;
        }

        public bool Compare(IRefExportObject refPerson)
        {
            var cr = refPerson as RefACLCR;
            if (cr != null)
                return CardReaderName==cr.CardReaderName;

            return false;
        }
    }

    [Serializable]
    public class RefACLPerson : RefCRPerson
    {
        public DateTime? ACLDateFrom { get; private set; }
        public DateTime? ACLDateTo { get; private set; }

        public RefACLPerson(Person Person, DateTime? DateFrom, DateTime? DateTo):base(Person)
        {
            ACLDateFrom = DateFrom;
            ACLDateTo = DateTo;
        }
        public string ACLDateFromSting()
        {
            return ACLDateFrom != null ? ACLDateFrom.Value.ToShortDateString() : "";
        }
        public string ACLDateToSting()
        {
            return ACLDateTo != null ? ACLDateTo.Value.ToShortDateString() : "";
        }

        protected override void PrepaireRow(int ColumnIndex, ref DataRow row) 
        {
            row[ColumnIndex++] = ACLDateFrom != null ? ACLDateFrom.Value.ToShortDateString() : "";
            row[ColumnIndex++] = ACLDateTo != null ? ACLDateTo.Value.ToShortDateString() : "";
            row[ColumnIndex++] = Person.ToString();
            row[ColumnIndex++] = Person.Identification ?? "";
            row[ColumnIndex++] = Person.Birthday != null ? Person.Birthday.Value.ToShortDateString() : "";
            row[ColumnIndex++] = Person.Department != null ? Person.Department.ToString() : "";
            row[ColumnIndex++] = Person.PhoneNumber ?? "";
            row[ColumnIndex++] = Person.Email ?? "";
            row[ColumnIndex++] = Person.EmploymentBeginningDate != null ? Person.EmploymentBeginningDate.Value.ToShortDateString() : "";
            row[ColumnIndex++] = Person.EmploymentEndDate != null ? Person.EmploymentEndDate.Value.ToShortDateString() : "";
        }
    }

    [Serializable]
    public class RefCRPerson : IRefExportObject
    {
        public Person Person { get; private set; }

        readonly string _rootNode;

        public RefCRPerson(Person Person)
        {
            this.Person = Person;
            var dep = UserFoldersStructures.Singleton.GetPersonDepartment(Person.GetIdString());
            if (dep != null)
            {
                _rootNode = ExportObjects.GetStringFromResources("Contal.Cgp.Server.Localization", "SelectStructuredSubSiteForm_RootNode");

                if (_rootNode != null)
                {
                    dep.SetFullFolderName(UserFoldersStructures.Singleton.GetFullDepartmentName(
                                                                   dep.GetIdString(),
                                                                   dep.FolderName,
                                                                    @"\",
                                                                    _rootNode));
                    this.Person.Department = dep;
                }
            }
        }

        public bool Compare(IRefExportObject refPerson)
        {
            var per = refPerson as RefCRPerson;
            if (per != null)
                return Person.Compare(per.Person);

            return false;
        }

        protected virtual void PrepaireRow(int ColumnIndex, ref DataRow row)
        {
            row[ColumnIndex++] = Person.ToString();
            row[ColumnIndex++] = Person.Identification ?? "";
            row[ColumnIndex++] = Person.Birthday != null ? Person.Birthday.Value.ToShortDateString() : "";
            row[ColumnIndex++] = Person.Department != null ? Person.Department.ToString() : "";
            row[ColumnIndex++] = Person.PhoneNumber ?? "";
            row[ColumnIndex++] = Person.Email ?? "";
            row[ColumnIndex++] = Person.EmploymentBeginningDate != null ? Person.EmploymentBeginningDate.Value.ToShortDateString() : "";
            row[ColumnIndex++] = Person.EmploymentEndDate != null ? Person.EmploymentEndDate.Value.ToShortDateString() : "";
        }
        public void FillDataRow(int ColumnIndex, ref DataRow row)
        {
            PrepaireRow(ColumnIndex, ref row);
        }

    }

    public class RefCardPerson : IRefExportObject
    {
        public Card Card { get; private set; }
        public string CardHolder { get; private set; }
        public string CardHolderId { get; private set; }

        public RefCardPerson(Card card, string cardHolder, string cardHolderId)
        {
            Card = card;
            CardHolder = cardHolder;
            CardHolderId = cardHolderId;
        }

        public void FillDataRow(int ColumnIndex, ref DataRow row)
        {
            row[ColumnIndex++] = Card.Number != null ? Card.Number:"";
            row[ColumnIndex++] = Card.FullCardNumber != null ? Card.FullCardNumber : "";
            row[ColumnIndex++] = CardHolder;
            row[ColumnIndex++] = CardHolderId ?? "";
            row[ColumnIndex++] = Card.ValidityDateFrom != null ? Card.ValidityDateFrom.Value.ToShortDateString() : "";
            row[ColumnIndex++] = Card.ValidityDateTo != null ? Card.ValidityDateTo.Value.ToShortDateString() : "";
            row[ColumnIndex++] = ((CardState)Card.State).ToString();
            row[ColumnIndex++] = Card.Description != null? Card.Description: "";
        }

        public bool Compare(IRefExportObject refPerson)
        {
            var card = refPerson as RefCardPerson;

            if(card!=null)
            {
                return Card.Compare(card.Card);
            }
            return false;
        }
    }

    public class RefPersonAttribute : IRefExportObject
    {
        public string PersonName { get; private set; }
        public string PersonId { get; private set; }
        public string ReaderName { get; private set; }
        public string CardNumber { get; private set; }
        public DateTime EventLogDateTimeFirst { get; private set; }
        public DateTime EventLogDateTimeLast { get; private set; }
        public string FailCounts { get; private set; }

        public RefPersonAttribute(string personName, string personid,string readerName, string cardNumber, DateTime eventLogDateTimeFirst, DateTime eventLogDateTimeLast, int failCounts)
        {
            PersonName = personName;
            PersonId= personid;
            ReaderName = readerName;
            CardNumber = cardNumber;
            FailCounts = $"{failCounts}x Access Denied";
            EventLogDateTimeFirst = eventLogDateTimeFirst;
            EventLogDateTimeLast = eventLogDateTimeLast;
        }

        public bool Compare(IRefExportObject refPerson)
        {
            var perAtt = refPerson as RefPersonAttribute;
            if(perAtt!=null)
            {
                return (PersonName.CompareTo(perAtt.PersonName) == 0) &&
                       (ReaderName.CompareTo(perAtt.ReaderName) == 0) &&
                       (CardNumber.CompareTo(perAtt.CardNumber) == 0);
            }
            return false;
        }

        public void FillDataRow(int ColumnIndex, ref DataRow row)
        {
            row[ColumnIndex++] = PersonName;
            row[ColumnIndex++] = PersonId;
            row[ColumnIndex++] = CardNumber;
            row[ColumnIndex++] = ReaderName;
            row[ColumnIndex++] = FailCounts;
            row[ColumnIndex++] = EventLogDateTimeFirst != null ? EventLogDateTimeFirst.ToString("yyyy/MM/dd/HH:mm:ss", CultureInfo.InvariantCulture) : "";
            row[ColumnIndex++] = EventLogDateTimeLast != null ? EventLogDateTimeLast.ToString("yyyy/MM/dd/HH:mm:ss", CultureInfo.InvariantCulture) : "";
        }
    }

    public class DoorAjarAlarmAttribute : IRefExportObject
    {
        public string Name { get; private set; }
        public string Parent { get; private set; }
        public string Person { get; private set; } = "-";
        public string PersonId { get; private set; }
        public DateTime dtCreated { get; private set; }
        public string  Duration { get; private set; }

        public DoorAjarAlarmAttribute(string name, string parent, string person, string personid,  DateTime dtCreated1, DateTime dtEnded)
        {
            Name = name;
            Parent = parent;
            Person = person;
            PersonId = personid;
            dtCreated = dtCreated1;
            if (dtEnded != null && dtEnded != DateTime.MinValue)
            {
                var duration = Math.Round((dtEnded - dtCreated).TotalSeconds,0);
                Duration = $"{duration}";
            }
            else
            {
                Duration = "unknown";
            }

        }

        public bool Compare(IRefExportObject refPerson)
        {
            var doorAjar = refPerson as DoorAjarAlarmAttribute;
            if (doorAjar != null)
            {
                return (Name.CompareTo(doorAjar.Name) == 0) && 
                        (dtCreated == doorAjar.dtCreated);
            }
            return false;
        }

        public void FillDataRow(int ColumnIndex, ref DataRow row)
        {
            row[ColumnIndex++] = Parent;
            row[ColumnIndex++] = Name;
            row[ColumnIndex++] = Person;
            row[ColumnIndex++] = PersonId;
            row[ColumnIndex++] = dtCreated != null ? (dtCreated.ToString("yyyy/MM/dd/HH:mm:ss", CultureInfo.InvariantCulture)):"";
            row[ColumnIndex++] = Duration;
        }
    }

    public class ExportObjects
    {
        public static string GetStringFromResources(string baseName, string columnName)
        {
            ResourceManager resources;
            string res = columnName;
            try
            {
                if (CultureInfo.CurrentCulture.Name == "en-US")
                {
                    resources = new ResourceManager(baseName + ".English",
                 typeof(Localization_English).Assembly);
                }
                else if (CultureInfo.CurrentCulture.Name == "sk-SK")
                {
                    resources = new ResourceManager(baseName + ".Slovak",
                 typeof(Localization_English).Assembly);
                }
                else
                {
                    resources = new ResourceManager(baseName + ".Swedish",
                           typeof(Localization_English).Assembly);
                }
                if (resources != null)
                {
                    var st = resources.GetString(columnName);
                    if (st != null)
                        res = st;
                }
            }
            catch(Exception)
            {
                return columnName;
            }

            return res;
        }
    }
}
