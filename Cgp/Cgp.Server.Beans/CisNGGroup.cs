using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class CisNGGroup : AOrmObject, IOrmObjectWithAlarmInstructions
    {
        public const string COLUMNIDCISNGGROUP = "IdCisNGGroup";
        public const string COLUMNGROUPNAME = "GroupName";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNCISNG = "CisNG";
        public const string COLUMNPRESENTATIONGROUP = "PresentationGroup";
        public const string COLUMNOBJECTTYPE = "ObjectType";

        public virtual Guid IdCisNGGroup { get; set; }
        public virtual string GroupName { get; set; }
        public virtual string LocalAlarmInstruction { get; set; }
        public virtual string Description { get; set; }
        public virtual ICollection<CisNG> CisNG { get; set; }
        public virtual ICollection<PresentationGroup> PresentationGroup { get; set; }
        public virtual byte ObjectType { get; set; }

        public CisNGGroup()
        {
            ObjectType = (byte)Globals.ObjectType.CisNGGroup;
        }

        public override string ToString()
        {
            return GroupName;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CisNGGroup)
            {
                return (obj as CisNGGroup).IdCisNGGroup == IdCisNGGroup;
            }
            return false;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (GroupName.ToLower().Contains(expression)) return true;
            if (Description != null)
            {
                if (Description.ToLower().Contains(expression)) return true;
            }
            return false;
        }

        public override string GetIdString()
        {
            return IdCisNGGroup.ToString();
        }

        public override object GetId()
        {
            return IdCisNGGroup;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new CisNGGroupModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.CisNGGroup;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }
    }

    [Serializable]
    public class CisNGGroupShort : IShortObject
    {
        public const string COLUMNIDCISNGGROUP = "IdCisNGGroup";
        public const string COLUMNGROUPNAME = "GroupName";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdCisNGGroup { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public CisNGGroupShort(CisNGGroup cisNGGroup)
        {
            IdCisNGGroup = cisNGGroup.IdCisNGGroup;
            GroupName = cisNGGroup.GroupName;
            Description = cisNGGroup.Description;
        }

        public override string ToString()
        {
            return GroupName;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.CisNGGroup; } }

        public string Name { get { return GroupName; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdCisNGGroup; } }

        #endregion
    }

    [Serializable]
    public class CisNGGroupModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.CisNGGroup; } }

        public CisNGGroupModifyObj(CisNGGroup cisNGGroup)
        {
            Id = cisNGGroup.IdCisNGGroup;
            FullName = cisNGGroup.ToString();
            Description = cisNGGroup.Description;
        }
    }

}
