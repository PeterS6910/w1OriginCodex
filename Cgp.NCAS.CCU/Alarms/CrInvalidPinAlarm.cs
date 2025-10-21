using System;
using System.Collections.Generic;
using System.Linq;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(745)]
    public class CrInvalidPinAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public CrInvalidPinAlarm()
        {

        }

        public CrInvalidPinAlarm(
            Guid idCardReader,
            Guid idCard)
            : base(
                new AlarmKey(
                    AlarmType.CardReader_InvalidPIN,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader),
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            idCard,
                            ObjectType.Card),
                        1)),
                AlarmState.Normal)
        {

        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCrInvalidPin(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCrInvalidPin(
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

            var extendedObjects = AlarmKey.ExtendedObjects;

            var cardIdAndObjectType = extendedObjects != null
                ? extendedObjects.FirstOrDefault(
                    idAndObjectType =>
                        idAndObjectType.ObjectType == ObjectType.Card)
                : null;

            var fullCardNumber = cardIdAndObjectType != null
                ? Database.ConfigObjectsEngine.CardsStorage.GetFullCardNumber((Guid) cardIdAndObjectType.Id)
                : null;

            return new[]
            {
                CatAlarmsManager.CreateCrAlarm(
                    AlarmEventCode.AccessDenied,
                    this,
                    cardReader,
                    fullCardNumber,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
