using System;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(207)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DayType : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdDayType { get; set; }

        public Guid GetGuid()
        {
            return IdDayType;
        }

        public Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.DayType;
        }
    }
}
