using System;

//using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class CustomAccessControl : AOrmObject
    {
        public const string COLUMNIDCUSTOMACCESSCONTROL = "IdCustomAccessControl";
        public const string COLUMNNAME = "Name";
        public const string COLUMNACCESS = "Access";
        public const string COLUMNOBJECTTYPE = "ObjectType";

        public virtual Guid IdCustomAccessControl { get; set; }
        public virtual string Name { get; set; }
        public virtual int Access { get; set; }
        public virtual string AccessControlGroup { get; set; }
        public virtual byte ObjectType { get; set; }

        public CustomAccessControl()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.CustomAccessControl;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CustomAccessControl)
            {
                return (obj as CustomAccessControl).IdCustomAccessControl == IdCustomAccessControl;
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
            return IdCustomAccessControl.ToString();
        }

        public override object GetId()
        {
            return IdCustomAccessControl;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.CustomAccessControl;
        }
    }
}
