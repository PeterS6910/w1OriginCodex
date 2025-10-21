using System;

using System.Collections.Generic;
using System.Linq;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(761)]
    public class CrInvalidPinRetriesLimitReached :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        IGetCreateAlarmFactoryWhenAlarmWasEnabled,
        ICreateCatAlarms
    {
        private class CreateAlarmFactoryWhenAlarmWasEnabled : ICreateAlarmFactoryWhenAlarmWasEnabled
        {
            private readonly Guid _idCard;
            private readonly Guid _idCardReader;

            public CreateAlarmFactoryWhenAlarmWasEnabled(
                Guid idCard,
                Guid idCardReader)
            {
                _idCard = idCard;
                _idCardReader = idCardReader;
            }

            Alarm ICreateAlarmFactoryWhenAlarmWasEnabled.CreateAlarm(bool processEvent)
            {
                return new CrInvalidPinRetriesLimitReached(
                    _idCard,
                    _idCardReader);
            }

            void ICreateAlarmFactoryWhenAlarmWasEnabled.CreateEvent()
            {

            }
        }

        public CrInvalidPinRetriesLimitReached()
        {

        }

        public CrInvalidPinRetriesLimitReached(
            Guid idCard,
            Guid idCardReader)
            : base(
                CreateAlarmKey(
                    idCard,
                    idCardReader),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(
            Guid idCard,
            Guid idCardRader)
        {
            return new AlarmKey(
                AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached,
                new IdAndObjectType(
                    idCard,
                    ObjectType.Card),
                Enumerable.Repeat(
                    new IdAndObjectType(
                        idCardRader,
                        ObjectType.CardReader),
                    1));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCrInvalidPinRetriesLimitReached(
                false,
                this);
        }

        ICreateAlarmFactoryWhenAlarmWasEnabled IGetCreateAlarmFactoryWhenAlarmWasEnabled.GetCreateAlarmFactory()
        {
            var alarmObject = AlarmKey.AlarmObject;

            if (alarmObject == null)
                return null;

            var cardReaderIdAndObjectType = AlarmKey.ExtendedObjects != null
                ? AlarmKey.ExtendedObjects.FirstOrDefault(
                    idAndObjectType => idAndObjectType.ObjectType == ObjectType.CardReader)
                : null;

            var idCardReader = cardReaderIdAndObjectType != null
                ? (Guid) cardReaderIdAndObjectType.Id
                : Guid.Empty;

            return new CreateAlarmFactoryWhenAlarmWasEnabled(
                (Guid) AlarmKey.AlarmObject.Id,
                idCardReader);
        }

        public ICollection<CatAlarm> CreateCatAlarms(bool alarmAcknowledged)
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

            var cardReaderIdAndObjectType = AlarmKey.ExtendedObjects != null
                ? AlarmKey.ExtendedObjects.FirstOrDefault(
                    idAndObjectType => idAndObjectType.ObjectType == ObjectType.CardReader)
                : null;

            if (cardReaderIdAndObjectType == null)
                return null;

            var cardReader = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.CardReader,
                (Guid) cardReaderIdAndObjectType.Id) as DB.CardReader;

            if (cardReader == null)
                return null;

            return new[]
            {
                CatAlarmsManager.CreateCrAlarm(
                    AlarmEventCode.AccessBlocked,
                    this,
                    cardReader,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
