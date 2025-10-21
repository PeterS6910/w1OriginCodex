using System;

using Contal.IwQuick.Data;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(305)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class ACLSetting : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdACLSetting { get; set; }
        [LwSerialize()]
        public virtual Guid GuidAccessControlList { get; set; }
        [LwSerialize()]
        public virtual Guid GuidCardReaderObject { get; set; }
        [LwSerialize()]
        public virtual byte CardReaderObjectType { get; set; }
        [LwSerialize()]
        public virtual Guid GuidTimeZone { get; set; }
        [LwSerialize()]
        public virtual bool? Disabled { get; set; }

        public Guid GetGuid()
        {
            return IdACLSetting;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.ACLSetting;
        }
    }
}
