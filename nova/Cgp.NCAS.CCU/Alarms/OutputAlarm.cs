using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(722)]
    public class OutputAlarm : Alarm, ICreateEventAlarmOccured, ICreateEventAlarmChanged
    {
        public OutputAlarm()
        {

        }

        public OutputAlarm(
            Guid idOutput)
            : base(
                new AlarmKey(
                    AlarmType.Output_Alarm,
                    new IdAndObjectType(
                        idOutput,
                        ObjectType.Output)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idOutput)
        {
            return new AlarmKey(
                AlarmType.Output_Alarm,
                new IdAndObjectType(
                    idOutput,
                    ObjectType.Output));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedOutputAlarm(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedOutputAlarm(
                false,
                this);
        }

        public static void Update(
            Guid idOutput,
            State outputState,
            bool alarmOn)
        {
            if (outputState == State.On)
            {
                if (!alarmOn)
                {
                    AlarmsManager.Singleton.StopAlarm(
                        CreateAlarmKey(idOutput));

                    return;
                }

                AlarmsManager.Singleton.AddAlarm(
                    new OutputAlarm(
                        idOutput));

                return;
            }

            AlarmsManager.Singleton.StopAlarm(
                CreateAlarmKey(idOutput));
        }
    }
}
