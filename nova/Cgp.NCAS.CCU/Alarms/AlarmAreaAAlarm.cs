using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(723)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAreaAAlarm : Alarm, ICreateEventAlarmOccured, ICreateEventAlarmChanged
    {
        public int CountOfSensorsInAlarm { get; private set; }

        public AlarmAreaAAlarm()
        {

        }

        public AlarmAreaAAlarm(
            Guid idAlarmArea,
            int countOfSensorsInAlarm)
            : base(
                new AlarmKey(
                    AlarmType.AlarmArea_AAlarm,
                    new IdAndObjectType(
                        idAlarmArea,
                        ObjectType.AlarmArea)),
                AlarmState.Alarm)
        {
            CountOfSensorsInAlarm = countOfSensorsInAlarm;
        }

        public static AlarmKey CreateAlarmKey(Guid idAlarmArea)
        {
            return new AlarmKey(
                AlarmType.AlarmArea_AAlarm,
                new IdAndObjectType(
                    idAlarmArea,
                    ObjectType.AlarmArea));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedAlarmAreaAAlarm(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedAlarmAreaAAlarm(
                false,
                this);
        }
    }
}
