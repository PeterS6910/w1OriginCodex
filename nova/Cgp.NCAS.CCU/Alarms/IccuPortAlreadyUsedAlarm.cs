using System;
using System.Collections.Generic;

using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(728)]
    public class IccuPortAlreadyUsedAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public IccuPortAlreadyUsedAlarm()
        {

        }

        public IccuPortAlreadyUsedAlarm(Guid idCcu)
            : base(
                new AlarmKey(
                    AlarmType.ICCU_PortAlreadyUsed,
                    new IdAndObjectType(
                        idCcu,
                        ObjectType.CCU)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idCcu)
        {
            return new AlarmKey(
                AlarmType.ICCU_PortAlreadyUsed,
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedIccuPortAlreadyUsed(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedIccuPortAlreadyUsed(
                false,
                this);
        }

        ICollection<CatAlarm> ICreateCatAlarms.CreateCatAlarms(bool alarmAcknowledged)
        {
            if (alarmAcknowledged
                || AlarmKey == null)
            {
                return null;
            }

            var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                AlarmKey.AlarmType,
                AlarmKey.AlarmObject);

            if (alarmArcs == null)
                return null;

            return new[]
            {
                CatAlarmsManager.CreateCcuAlarm(
                    AlarmEventCode.DeviceFailure,
                    this,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
