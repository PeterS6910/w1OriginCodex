using System;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(331)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    class AlarmArc : IDbObject
    {
        [LwSerialize]
        public Guid IdAlarmArc { get; set; }
        [LwSerialize]
        public string Name { get; set; }

        public Guid GetGuid()
        {
            return IdAlarmArc;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.AlarmArc;
        }
    }

    [LwSerialize(332)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AlarmTypeAndIdAlarmArc
    {
        [LwSerialize]
        public AlarmType AlarmType { get; private set; }

        [LwSerialize]
        public Guid IdAlarmArc { get; private set; }

        public AlarmTypeAndIdAlarmArc()
        {

        }

        public AlarmTypeAndIdAlarmArc(
            AlarmType alarmType,
            Guid idAlarmArc)
        {
            AlarmType = alarmType;
            IdAlarmArc = idAlarmArc;
        }
    }
}
