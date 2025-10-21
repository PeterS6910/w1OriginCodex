using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class StructuredSubSite : AOrmObject
    {
        public const string COLUMN_PARENT_SITE = "ParentSite";

        public virtual int IdStructuredSubSite { get; set; }
        public virtual StructuredSubSite ParentSite { get; set; }
        public virtual string Name { get; set; }
        public virtual ICollection<StructuredSubSiteObject> StructuredSubSiteObjects { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is StructuredSubSite)
            {
                return (obj as StructuredSubSite).IdStructuredSubSite == IdStructuredSubSite;
            }
            else
            {
                return false;
            }
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (this.Name.ToString().ToLower().Contains(expression)) return true;
            return false;
        }

        public override string GetIdString()
        {
            return IdStructuredSubSite.ToString();
        }

        public override object GetId()
        {
            return IdStructuredSubSite;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.StructuredSubSite;
        }
    }
}
