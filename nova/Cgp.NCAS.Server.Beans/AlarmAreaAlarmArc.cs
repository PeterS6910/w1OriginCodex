using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class AlarmAreaAlarmArc : AOrmObject
    {
        public const string COLUMN_ID_ALARM_AREA_ALARM_ARC = "IdAlarmAreaAlarmArc";
        public const string COLUMN_ALARM_AREA = "AlarmArea";
        public const string COLUMN_ALARM_TYPE = "AlarmType";
        public const string COLUMN_ALARM_ARC = "AlarmArc";
        public const string COLUMN_ID_ALARM_ARC = "IdAlarmArc";

        public virtual Guid IdAlarmAreaAlarmArc { get; set; }
        public virtual AlarmArea AlarmArea { get; set; }
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

        public AlarmAreaAlarmArc()
        {

        }

        public AlarmAreaAlarmArc(
            AlarmArea alarmArea,
            AlarmArc alarmArc,
            AlarmType alarmType)
        {
            AlarmArea = alarmArea;
            AlarmArc = alarmArc;
            AlarmType = (byte)alarmType;
        }

        public override bool Compare(object obj)
        {
            var alarmAreaAlarmArc = obj as AlarmAreaAlarmArc;

            if (alarmAreaAlarmArc == null)
                return false;

            return alarmAreaAlarmArc.IdAlarmAreaAlarmArc.Equals(
                IdAlarmAreaAlarmArc);
        }

        public override string GetIdString()
        {
            return IdAlarmAreaAlarmArc.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.AlarmAreaAlarmArc;
        }

        public override object GetId()
        {
            return IdAlarmAreaAlarmArc;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
