using System;
using System.IO;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(330)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AlarmTransmitter : IDbObject
    {
        [LwSerialize]
        public Guid IdAlarmTransmitter { get; set; }
        [LwSerialize]
        public string IpAddress { get; set; }

        public Guid GetGuid()
        {
            return IdAlarmTransmitter;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.AlarmTransmitter;
        }
    }
}
