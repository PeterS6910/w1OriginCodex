using System;
using System.Collections.Generic;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(727)]
    public class IccuSendingOfObjectStateFailedAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public IccuSendingOfObjectStateFailedAlarm()
        {

        }

        public IccuSendingOfObjectStateFailedAlarm(
            Guid idObject,
            ObjectType objectType)
            : base(
                new AlarmKey(
                    AlarmType.ICCU_SendingOfObjectStateFailed,
                    new IdAndObjectType(
                        idObject,
                        objectType)),
               AlarmState.Normal)
        {

        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedIccuSendingOfObjectStateFailed(
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

