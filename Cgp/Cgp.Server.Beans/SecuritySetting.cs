using System;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    public enum SecuritySettingSeverity : byte
    {
        High = 0
    }


    [Serializable()]
    public class SecuritySetting : AOrmObject
    {
        public const string COLUMNIDENUMSECURITYSETTING= "IdEnumSecuritySetting";

        public virtual byte IdEnumSecuritySetting { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is SecuritySetting)
            {
                return (obj as SecuritySetting).IdEnumSecuritySetting == IdEnumSecuritySetting;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdEnumSecuritySetting.ToString();
        }

        public override object GetId()
        {
            return IdEnumSecuritySetting;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.SecuritySetting;
        }
    }
}
