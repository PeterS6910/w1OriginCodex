using System;
using System.Collections.Generic;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(731)]
    public class CcuHighMemoryLoadAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public CcuHighMemoryLoadAlarm()
        {

        }

        public CcuHighMemoryLoadAlarm(Guid idCcu)
            : base(
                CreateAlarmKey(idCcu),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idCcu)
        {
            return new AlarmKey(
                AlarmType.CCU_HighMemoryLoad,
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCcuHighMemoryLoad(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCcuHighMemoryLoad(
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
