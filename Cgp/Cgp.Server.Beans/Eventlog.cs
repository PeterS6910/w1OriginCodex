using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class Eventlog : AOrmObject
    {
        public const string TYPECONNECTDATABASE = "Connect database";
        public const string TYPEDISCONNECTDATABASE = "Disconnect database";
        public const string TYPECLIENTCONNECT = "Connect client";
        public const string TYPECLIENTDISCONNECT = "Disconnect client";
        public const string TYPECLIENTLOGIN = "Client login";
        public const string TYPECLIENTLOGOUT = "Client logout";
        public const string TYPECLIENTLOGINWRONGPASSWORD = "Client login wrong password";
        public const string TYPEPRESENTATIONPROCESSOR = "Presentation processor error";
        public const string TYPEINITIALIZEPLUGIN = "Initialize plugin";
        public const string TYPELOADPLUGINSUCCESFUL = "Load plugin succesful";
        public const string TYPEALARMOCCURED = "Alarm occured";
        public const string TYPEACTALARMACKNOWLEDGED = "Active alarm acknowledged";
        public const string TYPEINACTALARMACKNOWLEDGED = "Inactive alarm acknowledged and removed";
        public const string TYPEBINDINGEVENTFAILED = "Binding event failed";
        public const string TYPERUNMETHODFAILED = "Run method failed";
        public const string TYPEACCESSGRANTED = "Access granted";
        public const string TYPEACCESSDENIED = "Access denied";
        public const string TYPEACCESSDENIEDINVALIDPIN = "Access denied invalid PIN";
        public const string TYPEACCESSDENIEDINVALIDGIN = "Access denied invalid GIN";
        public const string TYPEUNKNOWNCARD = "Unknown card";
        public const string TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE = "Access denied card blocked or inactive";
        public const string TYPEACCESSDENIEDSETALARMAREAINVALIDGIN = "Access denied set alarm area invalid GIN";
        public const string TYPEACCESSDENIEDUNSETALARMAREAINVALIDGIN = "Access denied unset alarm area invalid GIN";
        public const string TYPEACCESSDENIEDSETALARMAREAINVALIDPIN = "Access denied set alarm area invalid PIN";
        public const string TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN = "Access denied unset alarm area invalid PIN";
        public const string TYPEACCESSDENIEDSETALARMAREANORIGHTS = "Access denied set alarm area no rights";
        public const string TYPEACCESSDENIEDUNSETALARMAREANORIGHTS = "Access denied unset alarm area no rights";
        public const string TYPESETALARMAREAFROMCARDREADER = "Set alarm area from card reader";
        public const string TYPEUNSETALARMAREAFROMCARDREADER = "Unset alarm area from card reader";

        public Eventlog(string type, DateTime date, string cgpSource, string eventSource, string localization, string description)
        {
            Contal.IwQuick.Validator.CheckNullString(type);
            Contal.IwQuick.Validator.CheckNullString(cgpSource);
            Contal.IwQuick.Validator.CheckNullString(description);

            Type = type;
            Date = date;
            CGPSource = cgpSource;
            EventSource = eventSource;
            Localization = localization;
            Description = description;
        }

        public Eventlog()
        {
        }

        public virtual Guid IdEventlog { get; set; }
        public virtual string Type { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string CGPSource { get; set; }
        public virtual string EventSource { get; set; }
        public virtual string Localization { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<EventlogParameter> EventlogParameters { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Eventlog)
            {
                return (obj as Eventlog).IdEventlog == IdEventlog;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return Date.ToString("dd.MM.yyyy HH:mm:ss") + ": " + Type;
        }

        public override string GetPkHashCode()
        {
            return Contal.IwQuick.Crypto.QuickHashes.GetSHA256String(this.GetType().Name + IdEventlog.ToString());
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (this.ToString().ToLower().Contains(expression)) return true;
            if (this.Description.ToLower().Contains(expression)) return true;
            return false;
        }

        public override string GetIdString()
        {
            return IdEventlog.ToString();
        }
    }
}
