using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class EventlogParameter
    {
        public const string TYPECLIENTIP = "Client IP";
        public const string TYPECLIENTFRIENDLYNAME = "Client friendly name";
        public const string TYPEUSERNAME = "User name";
        public const string TYPEPLUGINNAME = "Plugin name";
        public const string TYPECCU = "CCU ip adress";
        public const string TYPECARDNUMBER = "Card number";
        public const string TYPEPERSONNAME = "Person name";
        public const string TYPEPEALARMAREANAME = "Alarm area name";

        public EventlogParameter()
        {
        }

        public EventlogParameter(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public virtual Guid IdEventlogParameter { get; set; }
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }

        public virtual Eventlog Eventlog { get; set; }

        public virtual bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is EventlogParameter)
            {
                return (obj as EventlogParameter).IdEventlogParameter == IdEventlogParameter;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return Type + ": " + Value;
        }        
    }
}
