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
    [LwSerialize(726)]
    public class CrOfflineAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        IGetCreateAlarmFactoryWhenAlarmWasEnabled,
        ICreateCatAlarms
    {
        private class CreateAlarmFactoryWhenAlarmWasEnabled : ICreateAlarmFactoryWhenAlarmWasEnabled
        {
            private readonly Guid _idCardReader;

            public CreateAlarmFactoryWhenAlarmWasEnabled(Guid idCardReader)
            {
                _idCardReader = idCardReader;
            }

            Alarm ICreateAlarmFactoryWhenAlarmWasEnabled.CreateAlarm(bool processEvent)
            {
                return new CrOfflineAlarm(
                    _idCardReader);
            }

            void ICreateAlarmFactoryWhenAlarmWasEnabled.CreateEvent()
            {

            }
        }

        public CrOfflineAlarm()
        {

        }

        public CrOfflineAlarm(
            Guid idCardReader)
            : base(
                new AlarmKey(
                    AlarmType.CardReader_Offline,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idCardReader)
        {
            return new AlarmKey(
                AlarmType.CardReader_Offline,
                new IdAndObjectType(
                    idCardReader,
                    ObjectType.CardReader));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCrOfflineAlarm(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCrOfflineAlarm(
                false,
                this);
        }

        public static void Update(
            CardReader cardReader,
            bool isOnline)
        {
            if (!isOnline)
            {
                AlarmsManager.Singleton.StopAlarmsForAlarmObjects(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            cardReader.IdCardReader,
                            ObjectType.CardReader),
                        1));

                AlarmsManager.Singleton.AddAlarm(
                    new CrOfflineAlarm(
                        cardReader.IdCardReader));

                return;
            }

            AlarmsManager.Singleton.StopAlarm(
                CreateAlarmKey(
                    cardReader.IdCardReader));
        }

        ICreateAlarmFactoryWhenAlarmWasEnabled IGetCreateAlarmFactoryWhenAlarmWasEnabled.GetCreateAlarmFactory()
        {
            var alarmObject = AlarmKey.AlarmObject;

            if (alarmObject == null)
                return null;

            return new CreateAlarmFactoryWhenAlarmWasEnabled((Guid) AlarmKey.AlarmObject.Id);
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
                    AlarmEventCode.SlaveDeviceOffline,
                    this,
                    cardReader,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
