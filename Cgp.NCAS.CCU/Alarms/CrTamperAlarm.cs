using System;
using System.Collections.Generic;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(735)]
    public class CrTamperAlarm :
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
                if (processEvent)
                    ((ICreateAlarmFactoryWhenAlarmWasEnabled) this).CreateEvent();

                return new CrTamperAlarm(
                    _idCardReader);
            }

            void ICreateAlarmFactoryWhenAlarmWasEnabled.CreateEvent()
            {
                Events.ProcessEvent(
                    new EventSendTamper(
                        _idCardReader,
                        ObjectType.CardReader,
                        true));
            }
        }

        public CrTamperAlarm()
        {

        }

        public CrTamperAlarm(Guid idCardReader)
            : base(
                new AlarmKey(
                    AlarmType.CardReader_TamperSabotage,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idCardReader)
        {
            return new AlarmKey(
                AlarmType.CardReader_TamperSabotage,
                new IdAndObjectType(
                    idCardReader,
                    ObjectType.CardReader));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCrTamper(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCrTamper(
                false,
                this);
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
                    AlarmEventCode.DeviceSabotage,
                    this,
                    cardReader,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
