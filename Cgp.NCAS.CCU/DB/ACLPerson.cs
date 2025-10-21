using System;
using Contal.IwQuick.Data;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(304)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class ACLPerson : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdACLPerson { get; set; }
        [LwSerialize()]
        public virtual Guid GuidAccessControlList { get; set; }
        [LwSerialize()]
        public virtual Guid GuidPerson { get; set; }
        [LwSerialize()]
        public virtual DateTime? DateFrom { get; set; }
        [LwSerialize()]
        public virtual DateTime? DateTo { get; set; }

        public Guid GetGuid()
        {
            return IdACLPerson;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.ACLPerson;
        }
    }
}
