using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class SystemEvent : AOrmObject
    {
        public const string DATABASE_DISCONNECTED = "Database disconnected";
        public const string CLIENT_CONNECTED = "Client connected";
        public const string CLIENT_DISCONNECTED = "Client disconnected";

        public virtual Guid IdSystemEvent { get; set; }
        public virtual string Name { get; set; }

        public virtual ICollection<PresentationGroup> PresentationGroups { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (this.Name.ToString().ToLower().Contains(expression)) return true;            
            return false;
        }

        public override string GetIdString()
        {
            return IdSystemEvent.ToString();
        }

        public override object GetId()
        {
            return IdSystemEvent;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.SystemEvent;
        }
    }
}
