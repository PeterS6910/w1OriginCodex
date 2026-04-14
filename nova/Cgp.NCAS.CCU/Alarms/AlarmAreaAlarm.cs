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
    [LwSerialize(724)]
    public class AlarmAreaAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public AlarmAreaAlarm()
        {

        }

        public AlarmAreaAlarm(
            Guid idAlarmArea)
            : base(
                new AlarmKey(
                    AlarmType.AlarmArea_Alarm,
                    new IdAndObjectType(
                        idAlarmArea,
                        ObjectType.AlarmArea)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idAlarmArea)
        {
            return new AlarmKey(
                AlarmType.AlarmArea_Alarm,
                new IdAndObjectType(
                    idAlarmArea,
                    ObjectType.AlarmArea));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedAlarmAreaAlarm(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedAlarmAreaAlarm(
                false,
                this);
        }

        public static void Update(
            Guid idAlarmArea,
            State alarmState)
        {
            if (alarmState == State.Alarm)
            {
                AlarmsManager.Singleton.AddAlarm(
                    new AlarmAreaAlarm(idAlarmArea));

                return;
            }

            AlarmsManager.Singleton.StopAlarm(
                CreateAlarmKey(idAlarmArea));
        }

        ICollection<CatAlarm> ICreateCatAlarms.CreateCatAlarms(bool alarmAcknowledged)
        {
            if (!alarmAcknowledged
                || AlarmKey == null
                || AlarmKey.AlarmObject == null)
            {
                return null;
            }

            var alarmArea = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.AlarmArea,
                (Guid) AlarmKey.AlarmObject.Id) as DB.AlarmArea;

            if (alarmArea == null)
                return null;

            return CreateCatAlarmsForActivatedSensorsDuringAlarmState(
                alarmArea,
                AlarmAreas.Singleton.GetActivatedSensorsDuringAlarmStateAndClear(alarmArea.IdAlarmArea));
        }

        public static ICollection<CatAlarm> CreateCatAlarmsForActivatedSensorsDuringAlarmState(
            DB.AlarmArea alarmArea,
            IEnumerable<Guid> activatedSensorsDuringAlarmState)
        {
            if (alarmArea == null
                || activatedSensorsDuringAlarmState == null)
            {
                return null;
            }

            var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                AlarmType.AlarmArea_Alarm,
                new IdAndObjectType(
                    alarmArea.IdAlarmArea,
                    ObjectType.AlarmArea));

            if (alarmArcs == null)
                return null;

            var catAlarms = new LinkedList<CatAlarm>();

            foreach (var activatedSensorDuringAlarmState in activatedSensorsDuringAlarmState)
            {
                var input = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.Input,
                    activatedSensorDuringAlarmState) as Input;

                if (input == null)
                    continue;

                var alarms = AlarmsManager.Singleton.GetAlarms(
                    AlarmType.Sensor_Alarm,
                    new IdAndObjectType(
                        input.IdInput,
                        ObjectType.Input));

                if (alarms == null)
                    continue;

                var sensorAlarm = (SensorAlarm) alarms.First();

                catAlarms.AddLast(
                    CatAlarmsManager.CreateSensorAlarm(
                        sensorAlarm,
                        false,
                        input,
                        alarmArea,
                        AlarmAreas.Singleton.GetSensorPurpose(
                            alarmArea.IdAlarmArea,
                            input.IdInput),
                        false,
                        alarmArcs));
            }

            return catAlarms;
        }
    }
}
