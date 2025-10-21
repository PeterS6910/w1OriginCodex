using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class DcuAlarmArc :
        AOrmObject,
        IAlarmArcForAlarmType
    {
        public const string COLUMN_ID_DCU_ALARM_ARC = "IdDcuAlarmArc";
        public const string COLUMN_DCU = "Dcu";
        public const string COLUMN_ALARM_TYPE = "AlarmType";
        public const string COLUMN_ALARM_ARC = "AlarmArc";
        public const string COLUMN_ID_ALARM_ARC = "IdAlarmArc";

        public virtual Guid IdDcuAlarmArc { get; set; }
        public virtual DCU Dcu { get; set; }
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

        public DcuAlarmArc()
        {

        }

        public DcuAlarmArc(
            DCU dcu,
            AlarmArc alarmArc,
            AlarmType alarmType)
        {
            Dcu = dcu;
            AlarmArc = alarmArc;
            AlarmType = (byte)alarmType;
        }

        public override bool Compare(object obj)
        {
            var dcuAlarmArc = obj as DcuAlarmArc;

            if (dcuAlarmArc == null)
                return false;

            return dcuAlarmArc.IdDcuAlarmArc.Equals(
                IdDcuAlarmArc);
        }

        public override string GetIdString()
        {
            return IdDcuAlarmArc.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.DcuAlarmArc;
        }

        public override object GetId()
        {
            return IdDcuAlarmArc;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
