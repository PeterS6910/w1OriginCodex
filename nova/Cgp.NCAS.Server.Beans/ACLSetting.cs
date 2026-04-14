using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(305)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class ACLSetting : AOrmObjectWithVersion
    {
        public const string COLUMN_ID_ACL_SETTING = "IdACLSetting";
        public const string COLUMN_ACCESS_CONTROL_LIST = "AccessControlList";
        public const string COLUMN_GUID_ACCESS_CONTROL_LIST = "GuidAccessControlList";
        public const string COLUMN_CARD_READER_OBJECT = "CardReaderObject";
        public const string COLUMN_GUID_CARD_READER_OBJECT = "GuidCardReaderObject";
        public const string COLUMN_CARD_READER_OBJECT_TYPE = "CardReaderObjectType";
        public const string COLUMN_TIMEZONE = "TimeZone";
        public const string COLUMN_GUID_TIME_ZONE = "GuidTimeZone";
        public const string COLUMN_DISABLED = "Disabled";
        public const string COLUMN_STRING_DISABLED = "StringDisabled";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdACLSetting { get; set; }
        public virtual AccessControlList AccessControlList { get; set; }

        [LwSerialize]
        public virtual Guid GuidAccessControlList { get; set; }

        public virtual AOrmObject CardReaderObject { get; set; }

        [LwSerialize]
        public virtual Guid GuidCardReaderObject { get; set; }
        [LwSerialize]
        public virtual byte CardReaderObjectType { get; set; }
        public virtual Cgp.Server.Beans.TimeZone TimeZone { get; set; }

        public ACLSetting()
        {
            GuidTimeZone = Guid.Empty;
            CardReaderObject = null;
            GuidAccessControlList = Guid.Empty;
        }

        [LwSerialize]
        public virtual Guid GuidTimeZone { get; set; }

        [LwSerializeAttribute]
        public virtual bool? Disabled { get; set; }

        public virtual string StringDisabled { get; set; }
        public virtual System.Drawing.Image Symbol { get; set; }
        public virtual string Description { get; set; }


        public override bool Compare(object obj)
        {
            var aclSetting = obj as ACLSetting;

            return
                aclSetting != null &&
                aclSetting.IdACLSetting == IdACLSetting;
        }

        public virtual void PrepareToSend()
        {
            GuidTimeZone =
                TimeZone != null
                    ? TimeZone.IdTimeZone
                    : Guid.Empty;

            GuidAccessControlList =
                AccessControlList != null
                    ? AccessControlList.IdAccessControlList
                    : Guid.Empty;
        }

        public override string GetIdString()
        {
            return IdACLSetting.ToString();
        }

        public override object GetId()
        {
            return IdACLSetting;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Cgp.Globals.ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.ACLSetting;
        }
    }
}
