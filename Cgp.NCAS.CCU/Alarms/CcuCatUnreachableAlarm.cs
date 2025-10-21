using System;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(756)]
    public class CcuCatUnreachableAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged
    {
        public CcuCatUnreachableAlarm()
        {

        }

        public CcuCatUnreachableAlarm(
            Guid idCcu,
            string alarmTransmitterIpAddress,
            AlarmState alarmState)
            : base(
                new AlarmKey(
                    AlarmType.Ccu_CatUnreachable,
                    new IdAndObjectType(
                        idCcu,
                        ObjectType.CCU),
                    Enumerable.Repeat(
                        new AlarmParameter(
                            ParameterType.CatIpAddress,
                            alarmTransmitterIpAddress),
                        1)),
                alarmState)
        {

        }

        public static AlarmKey CreateAlarmKey(
            Guid idCcu,
            string alarmTransmitterIpAddress)
        {
            return new AlarmKey(
                AlarmType.Ccu_CatUnreachable,
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                Enumerable.Repeat(
                    new AlarmParameter(
                        ParameterType.CatIpAddress,
                        alarmTransmitterIpAddress),
                    1));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCcuCatUnreachable(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCcuCatUnreachable(
                false,
                this);
        }
    }
}
