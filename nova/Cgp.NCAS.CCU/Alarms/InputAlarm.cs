using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(720)]
    public class InputAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged
    {
        public InputAlarm()
        {

        }

        public InputAlarm(Guid idInput)
            : base(
                new AlarmKey(
                    AlarmType.Input_Alarm,
                    new IdAndObjectType(
                        idInput,
                        ObjectType.Input)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idInput)
        {
            return new AlarmKey(
                AlarmType.Input_Alarm,
                new IdAndObjectType(
                    idInput,
                    ObjectType.Input));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedInputAlarm(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedInputAlarm(
                false,
                this);
        }
    }
}
