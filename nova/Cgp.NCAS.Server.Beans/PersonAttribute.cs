using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public class PersonAttribute : AOrmObject
    {
        public const string COLUMNIDACCESSZONE = "IdPersonAttribute";
        public const string COLUMNPERSON = "Person";

        public virtual Guid IdPersonAttribute { get; set; }

        public virtual Person Person { get; set; }

        public override string ToString()
        {
            return Person.WholeName;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is PersonAttribute)
            {
                return (obj as PersonAttribute).IdPersonAttribute == IdPersonAttribute;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdPersonAttribute.ToString();
        }

        public override object GetId()
        {
            return IdPersonAttribute;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.PersonAttribute;
        }
    }
}
