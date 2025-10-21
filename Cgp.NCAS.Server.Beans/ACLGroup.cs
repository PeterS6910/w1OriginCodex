using System;
using System.Collections.Generic;
using System.Drawing;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public class ACLGroup : AOrmObject
    {
        public const string COLUMN_ID_ACL_GROUP = "IdACLGroup";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_IS_IMPLICIT = "IsImplicit";
        public const string COLUMN_USE_FOR_ALL_PERSON = "UseForAllPerson";
        public const string COLUMN_USE_DEPARTMENT_ACL_GROUP_RELATION = "UseDepartmentAclGroupRelation";
        public const string COLUMN_REMOVE_ALL_ACLS = "RemoveAllAcls";
        public const string COLUMN_REMOVE_WHEN_DEPARTMENT_CHANGE = "RemoveWhenDepartmentChange";
        public const string COLUMN_REMOVE_WHEN_DEPARTMENT_REMOVE = "RemoveWhenDepartmentRemove";
        public const string COLUMN_APPLY_ENDLESS_VALIDITY = "ApplyEndlessValidity";
        public const string COLUMN_YEARS_OF_VALIDITY = "YearsOfValidity";
        public const string COLUMN_MONTHS_OF_VALIDITY = "MonthsOfValidity";
        public const string COLUMN_DAYS_OF_VALIDITY = "DaysOfValidity";
        public const string COLUMN_ACCESS_CONTROL_LISTS = "AccessControlLists";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_DEPARTMENTS = "Departments";

        public virtual Guid IdACLGroup { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsImplicit { get; set; }
        public virtual bool UseForAllPerson { get; set; }
        public virtual bool RemoveAllAcls { get; set; }
        public virtual bool UseDepartmentAclGroupRelation { get; set; }
        public virtual bool RemoveWhenDepartmentChange { get; set; }
        public virtual bool RemoveWhenDepartmentRemove { get; set; }
        public virtual bool ApplyEndlessValidity { get; set; }
        public virtual byte YearsOfValidity { get; set; }
        public virtual byte MonthsOfValidity { get; set; }
        public virtual byte DaysOfValidity { get; set; }
        public virtual ICollection<UserFoldersStructure> Departments { get; set; }
        public virtual ICollection<AccessControlList> AccessControlLists { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public ACLGroup()
        {
            ObjectType = (byte) Cgp.Globals.ObjectType.ACLGroup;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            return this.Equals(obj);
        }

        public override string GetIdString()
        {
            return IdACLGroup.ToString();
        }

        public override object GetId()
        {
            return IdACLGroup;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.ACLGroup;
        }

        public override bool Equals(object obj)
        {
            ACLGroup other = obj as ACLGroup;
            if (other == null)
                return false;

            return IdACLGroup == other.IdACLGroup;
        }
    }

    [Serializable()]
    public class ACLGroupShort : IShortObject
    {
        public const string COLUMN_SYBMOL = "Symbol";
        public const string COLUMN_ID_ACL_GROUP = "IdACLGroup";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_IS_IMPLICIT = "IsImplicit";
        public const string COLUMN_DESCRIPTION = "Description";

        public Image Symbol { get; set; }
        public Guid IdACLGroup { get; set; }
        public string Name { get; set; }
        public bool IsImplicit { get; set; }
        public string Description { get; set; }

        public ACLGroupShort(ACLGroup aclGroup)
        {
            IdACLGroup = aclGroup.IdACLGroup;
            Name = aclGroup.Name;
            IsImplicit = aclGroup.IsImplicit;
            Description = aclGroup.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.ACLGroup; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdACLGroup; } }

        #endregion
    }
}
