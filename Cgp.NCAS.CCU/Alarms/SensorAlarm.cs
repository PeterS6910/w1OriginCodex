using System;
using System.Collections.Generic;
using System.Linq;

using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(759)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class SensorAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public SensorAlarm()
        {
            
        }

        public SensorAlarm(
            Guid idInput,
            Guid idAlarmArea)
            : base(
                new AlarmKey(
                    AlarmType.Sensor_Alarm,
                    new IdAndObjectType(
                        idInput,
                        ObjectType.Input),
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            idAlarmArea,
                            ObjectType.AlarmArea),
                        1)),
                AlarmState.Alarm)
        {
            
        }

        public static AlarmKey CreateAlarmKey(
            Guid idInput,
            Guid idAlarmArea)
        {
            return new AlarmKey(
                AlarmType.Sensor_Alarm,
                new IdAndObjectType(
                    idInput,
                    ObjectType.Input),
                Enumerable.Repeat(
                    new IdAndObjectType(
                        idAlarmArea,
                        ObjectType.AlarmArea),
                    1));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedSensorAlarm(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedSensorAlarm(
                false,
                this);
        }

        ICollection<CatAlarm> ICreateCatAlarms.CreateCatAlarms(bool alarmAcknowledged)
        {
            if (AlarmKey == null
                || AlarmKey.AlarmObject == null
                || AlarmKey.ExtendedObjects == null)
            {
                return null;
            }

            var input = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.Input,
                (Guid) AlarmKey.AlarmObject.Id) as Input;

            if (input == null)
                return null;

            var alarmAreaIdAndObjectType = AlarmKey.ExtendedObjects.FirstOrDefault();

            if (alarmAreaIdAndObjectType == null)
                return null;

            var alarmArea = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.AlarmArea,
                (Guid) alarmAreaIdAndObjectType.Id) as DB.AlarmArea;

            if (alarmArea == null)
                return null;

            var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                AlarmType.AlarmArea_Alarm,
                new IdAndObjectType(
                    alarmArea.IdAlarmArea,
                    ObjectType.AlarmArea));

            if (alarmArcs == null)
                return null;

            if (alarmAcknowledged)
            {
                if (!AlarmAreas.Singleton.RemoveActivatedSensorDuringAlarmState(
                        alarmArea.IdAlarmArea,
                        input.IdInput))
                {
                    return null;
                }

                return new[]
                {
                    CatAlarmsManager.CreateSensorAlarm(
                        this,
                        false,
                        input,
                        alarmArea,
                        AlarmAreas.Singleton.GetSensorPurpose(
                            alarmArea.IdAlarmArea,
                            input.IdInput),
                        false,
                        alarmArcs)
                };
            }

            if (AlarmState != AlarmState.Alarm
                || !AlarmAreas.Singleton.AddActivatedSensorDuringAlarmState(
                alarmArea.IdAlarmArea,
                input.IdInput))
            {
                return null;
            }

            return new[]
            {
                CatAlarmsManager.CreateSensorAlarm(
                    this,
                    true,
                    input,
                    alarmArea,
                    AlarmAreas.Singleton.GetSensorPurpose(
                        alarmArea.IdAlarmArea,
                        input.IdInput),
                    false,
                    alarmArcs)
            };
        }
    }
}
