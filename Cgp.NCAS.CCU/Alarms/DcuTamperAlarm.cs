using System;
using System.Collections.Generic;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(734)]
    public class DcuTamperAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        IGetCreateAlarmFactoryWhenAlarmWasEnabled,
        ICreateCatAlarms
    {
        private class CreateAlarmFactoryWhenAlarmWasEnabled : ICreateAlarmFactoryWhenAlarmWasEnabled
        {
            private readonly Guid _idDcu;

            public CreateAlarmFactoryWhenAlarmWasEnabled(Guid idDcu)
            {
                _idDcu = idDcu;
            }

            Alarm ICreateAlarmFactoryWhenAlarmWasEnabled.CreateAlarm(bool processEvent)
            {
                if (processEvent)
                    ((ICreateAlarmFactoryWhenAlarmWasEnabled) this).CreateEvent();

                return new DcuTamperAlarm(
                    _idDcu);
            }

            void ICreateAlarmFactoryWhenAlarmWasEnabled.CreateEvent()
            {
                Events.ProcessEvent(
                    new EventSendTamper(
                        _idDcu,
                        ObjectType.DCU,
                        true));
            }
        }

        public DcuTamperAlarm()
        {

        }

        public DcuTamperAlarm(Guid idDcu)
            : base(
                new AlarmKey(
                    AlarmType.DCU_TamperSabotage,
                    new IdAndObjectType(
                        idDcu,
                        ObjectType.DCU)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idDcu)
        {
            return new AlarmKey(
                AlarmType.DCU_TamperSabotage,
                new IdAndObjectType(
                    idDcu,
                    ObjectType.DCU));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedDcuTamper(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedDcuTamper(
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

            var dcu = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.DCU,
                (Guid) AlarmKey.AlarmObject.Id) as DCU;

            if (dcu == null)
                return null;

            return new[]
            {
                CatAlarmsManager.CreateDcuAlarm(
                    AlarmEventCode.DeviceSabotage,
                    this,
                    dcu,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
