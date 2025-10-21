using System;
using System.Collections.Generic;
using System.Linq;

using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(742)]
    public class CrAccessDeniedAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public CrAccessDeniedAlarm()
        {

        }

        public CrAccessDeniedAlarm(
            Guid idCardReader,
            [NotNull] AccessDataBase accessData)
            : base(
                new AlarmKey(
                    AlarmType.CardReader_AccessDenied,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader),
                    accessData.IdCard != Guid.Empty
                        ? Enumerable.Repeat(
                            new IdAndObjectType(
                                accessData.IdCard,
                                ObjectType.Card),
                            1)
                        : Enumerable.Repeat(
                            new IdAndObjectType(
                                accessData.IdPerson,
                                ObjectType.Person),
                            1)),
                AlarmState.Normal)
        {
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return AlarmKey.ExtendedObjects.Any(
                idAndObjectType =>
                    idAndObjectType.ObjectType == ObjectType.Card)
                ? (EventParameters.EventParameters)
                    new EventAlarmOccuredOrChangedCrAccessDeniedWithCard(
                        true,
                        this)
                : new EventAlarmOccuredOrChangedCrAccessDeniedWithPerson(
                    true,
                    this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return AlarmKey.ExtendedObjects.Any(
                idAndObjectType =>
                    idAndObjectType.ObjectType == ObjectType.Card)
                ? (EventParameters.EventParameters)
                    new EventAlarmOccuredOrChangedCrAccessDeniedWithCard(
                        false,
                        this)
                : new EventAlarmOccuredOrChangedCrAccessDeniedWithPerson(
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
                ? Database.ConfigObjectsEngine.CardsStorage.GetFullCardNumber((Guid)cardIdAndObjectType.Id)
                : null;

            return new[]
            {
                CatAlarmsManager.CreateCrAlarm(
                    AlarmEventCode.AccessNotAuthorized,
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
