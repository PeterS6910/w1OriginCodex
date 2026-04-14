using System;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(326)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class SecurityDayInterval : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdInterval { get; set; }
        [LwSerialize()]
        public virtual short MinutesFrom { get; set; }
        [LwSerialize()]
        public virtual short MinutesTo { get; set; }
        [LwSerialize()]
        public virtual byte IntervalType { get; set; }

        public Guid GetGuid()
        {
            return IdInterval;
        }

        public Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.SecurityDayInterval;
        }
    }
}
