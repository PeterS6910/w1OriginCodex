using System;
using System.Collections.Generic;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(748)]
    public class CrInvalidEmergencyCodeAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public CrInvalidEmergencyCodeAlarm()
        {

        }

        public CrInvalidEmergencyCodeAlarm(
            Guid idCardReader)
            : base(
                new AlarmKey(
                    AlarmType.CardReader_InvalidEmergencyCode,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)),
                AlarmState.Normal)
        {

        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCrInvalidEmergency(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCrInvalidEmergency(
                false,
                this);
        }

        ICollection<CatAlarm> ICreateCatAlarms.CreateCatAlarms(bool alarmAcknowledged)
        {
            if (alarmAcknowledged
                || AlarmKey == null
                || AlarmKey.AlarmObject == null)
            {
                return null;
            }

            var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                AlarmKey.AlarmType,
                AlarmKey.AlarmObject);

            if (alarmArcs == null)
                return null;

            var cardReader = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.CardReader,
                (Guid) AlarmKey.AlarmObject.Id) as CardReader;

            if (cardReader == null)
                return null;

            return new[]
            {
                CatAlarmsManager.CreateCrAlarm(
                    AlarmEventCode.AccessDenied,
                    this,
                    cardReader,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
