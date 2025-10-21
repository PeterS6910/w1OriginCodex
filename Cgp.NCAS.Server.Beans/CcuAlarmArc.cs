using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class CcuAlarmArc :
        AOrmObject,
        IAlarmArcForAlarmType
    {
        public const string COLUMN_ID_CCU_ALARM_ARC = "IdCcuAlarmArc";
        public const string COLUMN_CCU = "Ccu";
        public const string COLUMN_ALARM_TYPE = "AlarmType";
        public const string COLUMN_ALARM_ARC = "AlarmArc";
        public const string COLUMN_ID_ALARM_ARC = "IdAlarmArc";

        public virtual Guid IdCcuAlarmArc { get; set; }
        public virtual CCU Ccu { get; set; }
        public virtual byte AlarmType { get; set; }
        public virtual AlarmArc AlarmArc { get; set; }

        public virtual Guid IdAlarmArc
        {
            get
            {
                return AlarmArc != null
                    ? AlarmArc.IdAlarmArc
                    : Guid.Empty;
            }
        }

        public CcuAlarmArc()
        {

        }

        public CcuAlarmArc(
            CCU ccu,
            AlarmArc alarmArc,
            AlarmType alarmType)
        {
            Ccu = ccu;
            AlarmArc = alarmArc;
            AlarmType = (byte)alarmType;
        }

        public override bool Compare(object obj)
        {
            var ccuAlarmArc = obj as CcuAlarmArc;

            if (ccuAlarmArc == null)
                return false;

            return ccuAlarmArc.IdCcuAlarmArc.Equals(
                IdCcuAlarmArc);
        }

        public override string GetIdString()
        {
            return IdCcuAlarmArc.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.CcuAlarmArc;
        }

        public override object GetId()
        {
            return IdCcuAlarmArc;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
