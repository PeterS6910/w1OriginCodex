using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class DoorEnvironmentAlarmArc : AOrmObject
    {
        public const string COLUMN_ID_DOOR_ENVIRONMENT_ALARM_ARC = "IdDoorEnvironmentAlarmArc";
        public const string COLUMN_DOOR_ENVIRONMENT = "DoorEnvironment";
        public const string COLUMN_ALARM_TYPE = "AlarmType";
        public const string COLUMN_ALARM_ARC = "AlarmArc";

        public virtual Guid IdDoorEnvironmentAlarmArc { get; set; }
        public virtual DoorEnvironment DoorEnvironment { get; set; }
        public virtual byte AlarmType { get; set; }
        public virtual AlarmArc AlarmArc { get; set; }

        public DoorEnvironmentAlarmArc()
        {

        }

        public DoorEnvironmentAlarmArc(
            DoorEnvironment doorEnvironment,
            AlarmArc alarmArc,
            AlarmType alarmType)
        {
            DoorEnvironment = doorEnvironment;
            AlarmArc = alarmArc;
            AlarmType = (byte)alarmType;
        }

        public override bool Compare(object obj)
        {
            var cardReaderAlarmArc = obj as DoorEnvironmentAlarmArc;

            if (cardReaderAlarmArc == null)
                return false;

            return cardReaderAlarmArc.IdDoorEnvironmentAlarmArc.Equals(
                IdDoorEnvironmentAlarmArc);
        }

        public override string GetIdString()
        {
            return IdDoorEnvironmentAlarmArc.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.DoorEnvironmentAlarmArc;
        }

        public override object GetId()
        {
            return IdDoorEnvironmentAlarmArc;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
