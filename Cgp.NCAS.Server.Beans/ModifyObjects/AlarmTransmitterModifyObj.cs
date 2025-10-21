using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans.ModifyObjects
{
    [Serializable]
    public class AlarmTransmitterModifyObj : AModifyObject
    {
        public AlarmTransmitterModifyObj(AlarmTransmitter alarmTransmitter)
        {
            Id = alarmTransmitter.IdAlarmTransmitter;
            FullName = alarmTransmitter.ToString();
            Description = alarmTransmitter.Description;
        }

        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.AlarmTransmitter; }
        }
    }
}
