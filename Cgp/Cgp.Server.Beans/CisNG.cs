using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class CisNG : AOrmObject, IOrmObjectWithAlarmInstructions
    {
        public const string COLUMNIDCISNG = "IdCisNG";
        public const string COLUMNCISNGNAME = "CisNGName";
        public const string COLUMNIPADDRESS = "IpAddress";
        public const string COLUMNPORT = "Port";
        public const string COLUMNUSERNAME = "UserName";
        public const string COLUMNPASSWORD = "Password";
        public const string COLUMNCISNGGROUP = "CisNGGroup";
        public const string COLUMNPRESENTATIONGROUP = "PresentationGroup";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNOBJECTTYPE = "ObjectType";

        public virtual Guid IdCisNG { get; set; }
        public virtual string CisNGName { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual int Port { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual ICollection<CisNGGroup> CisNGGroup { get; set; }
        public virtual ICollection<PresentationGroup> PresentationGroup { get; set; }
        public virtual string LocalAlarmInstruction { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public CisNG()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.CisNG;
        }

        public override string ToString()
        {
            return CisNGName;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CisNG)
            {
                return (obj as CisNG).IdCisNG == IdCisNG;
            }
            else
            {
                return false;
            }
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (this.CisNGName.ToString().ToLower().Contains(expression)) return true;
            if (this.Description != null)
            {
                if (this.Description.ToLower().Contains(expression)) return true;
            }
            return false;
        }

        public override string GetIdString()
        {
            return IdCisNG.ToString();
        }

        public override object GetId()
        {
            return IdCisNG;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new CisNGModifyObj(this);
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.CisNG;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }
    }

    [Serializable()]
    public class CisNGShort : IShortObject
    {
        public const string COLUMNIDCISNG = "IdCisNG";
        public const string COLUMNCISNGNAME = "CisNGName";
        public const string COLUMNIPADDRESS = "IpAddress";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdCisNG { get; set; }
        public string CisNGName { get; set; }
        public string IpAddress { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public CisNGShort(CisNG cisNG)
        {
            IdCisNG = cisNG.IdCisNG;
            CisNGName = cisNG.CisNGName;
            IpAddress = cisNG.IpAddress;
            Description = cisNG.Description;
        }

        public override string ToString()
        {
            return CisNGName;
        }

        #region IShortObject Members

        public object Id { get { return IdCisNG; } }

        public string Name { get { return CisNGName; } }

        public ObjectType ObjectType { get { return ObjectType.CisNG; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable()]
    public class CisNGModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.CisNG; } }

        public CisNGModifyObj(CisNG cisNG)
        {
            Id = cisNG.IdCisNG;
            FullName = cisNG.ToString();
            Description = cisNG.Description;
        }
    }
}
