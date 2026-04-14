using System;
using Contal.IwQuick.Data;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(306)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class ACLSettingAA : IDbObject
    {
        [LwSerialize]
        public Guid IdACLSettingAA { get; set; }
        [LwSerialize]
        public Guid GuidAccessControlList { get; set; }
        [LwSerialize]
        public Guid GuidAlarmArea { get; set; }
        [LwSerialize]
        public bool AlarmAreaSet { get; set; }
        [LwSerialize]
        public bool AlarmAreaUnset { get; set; }
        [LwSerialize]
        public bool AlarmAreaUnconditionalSet { get; set; }
        [LwSerialize]
        public bool AlarmAreaAlarmAcknowledge { get; set; }
        [LwSerialize]
        public bool SensorHandling { get; set; }
        [LwSerialize]
        public bool CREventLogHandling { get; set; }
        [LwSerialize]
        public bool AlarmAreaTimeBuying { get; set; }


        public Guid GetGuid()
        {
            return IdACLSettingAA;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.ACLSettingAA;
        }
    }
}
