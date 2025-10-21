using System;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(757)]
    public class CcuTransferToArcTimedOutAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged
    {
        public CcuTransferToArcTimedOutAlarm()
        {

        }

        public CcuTransferToArcTimedOutAlarm(
            Guid idCcu,
            string alarmTransmitterIpAddress,
            string arcName)
            : base(
                new AlarmKey(
                    AlarmType.Ccu_TransferToArcTimedOut,
                    new IdAndObjectType(
                        idCcu,
                        ObjectType.CCU),
                    Enumerable.Repeat(
                        new AlarmParameter(
                            ParameterType.CatIpAddress,
                            alarmTransmitterIpAddress),
                        1)
                        .Concat(
                            Enumerable.Repeat(
                                new AlarmParameter(
                                    ParameterType.ArcName,
                                    arcName),
                                1))),
                AlarmState.Normal)
        {

        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCcuTransferToArcTimedOut(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCcuTransferToArcTimedOut(
                false,
                this);
        }
    }
}
