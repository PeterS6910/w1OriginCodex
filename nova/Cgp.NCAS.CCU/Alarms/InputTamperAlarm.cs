using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(721)]
    public class InputTamperAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged
    {
        public InputTamperAlarm()
        {

        }

        public InputTamperAlarm(Guid idInput)
            : base(
                new AlarmKey(
                    AlarmType.Input_Tamper,
                    new IdAndObjectType(
                        idInput,
                        ObjectType.Input)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idInput)
        {
            return new AlarmKey(
                AlarmType.Input_Tamper,
                new IdAndObjectType(
                    idInput,
                    ObjectType.Input));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedInputTamper(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedInputTamper(
                false,
                this);
        }

        public static void Update(
            Guid idInput,
            State inputState,
            bool alarmTamperOn)
        {
            if (inputState == State.Short
                || inputState == State.Break)
            {
                if (!alarmTamperOn)
                {
                    AlarmsManager.Singleton.StopAlarm(
                        CreateAlarmKey(idInput));

                    return;
                }

                AlarmsManager.Singleton.AddAlarm(
                    new InputTamperAlarm(idInput));

                return;
            }

            AlarmsManager.Singleton.StopAlarm(
                CreateAlarmKey(idInput));
        }
    }
}
