using System;
using System.Collections.Generic;
using System.Drawing;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class AccessControlList : AOrmObjectWithVersion
    {
        public const string COLUMNIDACCESSCONTROLLIST = "IdAccessControlList";
        public const string COLUMNNAME = "Name";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNACLSETTINGS = "ACLSettings";
        public const string COLUMNGUIDACLSETTINGS = "GuidACLSettings";
        public const string COLUMNACLSETTINGAAS = "ACLSettingAAs";
        public const string COLUMNGUIDACLSETTINGAAS = "GuidACLSettingAAs";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdAccessControlList { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual ICollection<ACLSetting> ACLSettings { get; set; }
        private List<Guid> _guidACLSettings = new List<Guid>();
        [LwSerialize]
        public virtual List<Guid> GuidACLSettings { get { return _guidACLSettings; } set { _guidACLSettings = value; } }

        public virtual ICollection<ACLSettingAA> ACLSettingAAs { get; set; }
        private List<Guid> _guidACLSettingAAs = new List<Guid>();
        [LwSerialize]
        public virtual List<Guid> GuidACLSettingAAs { get { return _guidACLSettingAAs; } set { _guidACLSettingAAs = value; } }
        public virtual byte ObjectType { get; set; }

        public AccessControlList()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.AccessControlList;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            var accessControlList = obj as AccessControlList;

            return
                accessControlList != null &&
                accessControlList.IdAccessControlList == IdAccessControlList;
        }

        public virtual void PrepareToSend()
        {
            GuidACLSettings.Clear();
            if (ACLSettings != null)
            {
                foreach (ACLSetting aclSetting in ACLSettings)
                {
                    GuidACLSettings.Add(aclSetting.IdACLSetting);
                }
            }

            GuidACLSettingAAs.Clear();
            if (ACLSettingAAs != null)
            {
                foreach (ACLSettingAA aclSettingAA in ACLSettingAAs)
                {
                    GuidACLSettingAAs.Add(aclSettingAA.IdACLSettingAA);
                }
            }
        }

        public override string GetIdString()
        {
            return IdAccessControlList.ToString();
        }

        public override object GetId()
        {
            return IdAccessControlList;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new AccessControlListModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.AccessControlList;
        }
    }

    [Serializable]
    public class AccessControlListShort : IShortObject
    {
        public const string COLUMN_ID_ACCESS_CONTROL_LIST = "IdAccessControlList";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYBMOL = "Symbol";
            
        public Guid IdAccessControlList { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public AccessControlListShort(AccessControlList acl)
        {
            IdAccessControlList = acl.IdAccessControlList;
            Name = acl.Name;
            Description = acl.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.AccessControlList; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion

        #region IShortObject Members

        public object Id { get { return IdAccessControlList; } }

        #endregion
    }

    [Serializable]
    public class AccessControlListModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.AccessControlList; } }

        public AccessControlListModifyObj(AccessControlList acl)
        {
            Id = acl.IdAccessControlList;
            FullName = acl.Name;
            Description = acl.Description;
        }
    }
}
