using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans.ModifyObjects
{
    [Serializable]
    public class AlarmArcModifyObj : AModifyObject
    {
        public AlarmArcModifyObj(AlarmArc alarmArc)
        {
            Id = alarmArc.IdAlarmArc;
            FullName = alarmArc.ToString();
            Description = alarmArc.Description;
        }

        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.AlarmArc; }
        }
    }
}
