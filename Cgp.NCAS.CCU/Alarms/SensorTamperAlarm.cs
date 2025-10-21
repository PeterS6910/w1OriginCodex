using System;
using System.Linq;
using System.Collections.Generic;

using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(760)]
    internal class SensorTamperAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        private static class CreatedSensorTamperAlarms 
        {
            private readonly static SyncDictionary<Guid, HashSet<Guid>> _createdAlarms =
                new SyncDictionary<Guid, HashSet<Guid>>();

            public static bool Add(
                Guid idAlarmArea,
                Guid idInput)
            {
                var result = false;

                _createdAlarms.GetOrAddValue(
                    idAlarmArea,
                    key => new HashSet<Guid>(),
                    (key, value, newleAdded) =>
                    {
                        result = value.Add(idInput);
                    });

                return result;
            }

            public static bool Remove(
                Guid idAlarmArea,
                Guid idInput)
            {
                var result = false;

                _createdAlarms.TryGetValue(
                    idAlarmArea,
                    (key, found, value) =>
                    {
                        if (!found)
                            return;

                        result = value.Remove(idInput);
                    });

                return result;
            }
        }

        public SensorTamperAlarm()
        {

        }

        public SensorTamperAlarm(
            Guid idInput,
            Guid idAlarmArea)
            : base(
                new AlarmKey(
                    AlarmType.Sensor_Tamper_Alarm,
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
                AlarmType.Sensor_Tamper_Alarm,
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
            return new EventAlarmOccuredOrChangedSensorTamperAlarm(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedSensorTamperAlarm(
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
                if (!CreatedSensorTamperAlarms.Remove(
                    alarmArea.IdAlarmArea,
                    input.IdInput))
                {
                    return null;
                }

                return new[]
                {
                    CatAlarmsManager.CreateSensorTamperAlarm(
                        this,
                        false,
                        input,
                        alarmArea,
                        alarmArcs)
                };
            }

            if (AlarmState != AlarmState.Alarm)
                return null;

            if (!CreatedSensorTamperAlarms.Add(
                alarmArea.IdAlarmArea,
                input.IdInput))
            {
                return null;
            }

            return new[]
            {
                CatAlarmsManager.CreateSensorTamperAlarm(
                    this,
                    true,
                    input,
                    alarmArea,
                    alarmArcs)
            };
        }
    }
}
