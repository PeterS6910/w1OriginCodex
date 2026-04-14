using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class DevicesAlarmSettingAlarmArc :
        AOrmObject,
        IAlarmArcForAlarmType
    {
        public const string COLUMN_ID_DEVICES_ALARM_SETTING_ALARM_ARC = "IdDevicesAlarmSettingAlarmArc";
        public const string COLUMN_DEVICES_ALARM_SETTING = "DevicesAlarmSetting";
        public const string COLUMN_ALARM_TYPE = "AlarmType";
        public const string COLUMN_ALARM_ARC = "AlarmArc";
        public const string COLUMN_ID_ALARM_ARC = "IdAlarmArc";

        public virtual Guid IdDevicesAlarmSettingAlarmArc { get; set; }
        public virtual DevicesAlarmSetting DevicesAlarmSetting { get; set; }
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

        public DevicesAlarmSettingAlarmArc()
        {
            
        }

        public DevicesAlarmSettingAlarmArc(
            DevicesAlarmSetting devicesAlarmSetting,
            AlarmArc alarmArc,
            AlarmType alarmType)
        {
            DevicesAlarmSetting = devicesAlarmSetting;
            AlarmArc = alarmArc;            
            AlarmType = (byte)alarmType;
        }

        public override bool Compare(object obj)
        {
            var devicesAlarmSettingAlarmArc = obj as DevicesAlarmSettingAlarmArc;

            if (devicesAlarmSettingAlarmArc == null)
                return false;

            return devicesAlarmSettingAlarmArc.IdDevicesAlarmSettingAlarmArc.Equals(
                IdDevicesAlarmSettingAlarmArc);
        }

        public override string GetIdString()
        {
            return IdDevicesAlarmSettingAlarmArc.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.DevicesAlarmSettingAlarmArc;
        }

        public override object GetId()
        {
            return IdDevicesAlarmSettingAlarmArc;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
