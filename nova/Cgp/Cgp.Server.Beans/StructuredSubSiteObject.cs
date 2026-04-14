using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class StructuredSubSiteObject : AOrmObject
    {
        public const string COLUMN_OBJECT_ID = "ObjectId";
        public const string COLUMN_STRUCTURED_SUB_SITE = "StructuredSubSite";
        public const string COLUMN_IS_REFERNCE = "IsReference";

        public virtual int IdStructuredSubSiteObject { get; set; }
        public virtual StructuredSubSite StructuredSubSite { get; set; }
        public virtual ObjectType ObjectType { get; set; }
        public virtual string ObjectId { get; set; }
        public virtual bool IsReference { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            var structuredSubSiteObject = obj as StructuredSubSiteObject;

            return 
                structuredSubSiteObject != null &&
                structuredSubSiteObject.IdStructuredSubSiteObject == IdStructuredSubSiteObject;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            return false;
        }

        public override string GetIdString()
        {
            return IdStructuredSubSiteObject.ToString();
        }

        public override object GetId()
        {
            return IdStructuredSubSiteObject;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.StructuredSubSiteObject;
        }
    }

    [Serializable]
    public class StructuredSiteObjectWithChildObjects
    {
        public AOrmObject OrmObject { get; private set; }
        public AOrmObject ParentObject { get; private set; }
        public Dictionary<ObjectType, ICollection<StructuredSiteObjectWithChildObjects>> ChildObjects { get; private set; }

        public StructuredSubSiteObject StructuredSubSiteObject { get; private set; }

        public StructuredSiteObjectWithChildObjects(AOrmObject ormObject, AOrmObject parentObject,
            Dictionary<ObjectType, ICollection<StructuredSiteObjectWithChildObjects>> childObjects,
            StructuredSubSiteObject structuredSubSiteObject)
        {
            OrmObject = ormObject;
            ParentObject = parentObject;
            ChildObjects = childObjects;
            StructuredSubSiteObject = structuredSubSiteObject;
        }
    }
}
