using System;
using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(303)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AccessZone : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdAccessZone { get; set; }
        [LwSerialize()]
        public virtual Guid GuidPerson{ get; set; }
        [LwSerialize()]
        public virtual Guid GuidCardReaderObject { get; set; }
        [LwSerialize()]
        public virtual byte CardReaderObjectType { get; set; }
        [LwSerialize()]
        public virtual Guid GuidTimeZone{ get; set; }

        public Guid GetGuid()
        {
            return IdAccessZone;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.AccessZone;
        }
    }
}
