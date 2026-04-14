using System;
using System.Collections.Generic;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(739)]
    public class DeIntrusionAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        IGetCreateAlarmFactoryWhenAlarmWasEnabled,
        ICreateCatAlarms
    {
        private class CreateAlarmFactoryWhenAlarmWasEnabled : ICreateAlarmFactoryWhenAlarmWasEnabled
        {
            private readonly Guid _idDoorEnvironment;

            public CreateAlarmFactoryWhenAlarmWasEnabled(Guid idDoorEnvironment)
            {
                _idDoorEnvironment = idDoorEnvironment;
            }

            Alarm ICreateAlarmFactoryWhenAlarmWasEnabled.CreateAlarm(bool processEvent)
            {
                return new DeIntrusionAlarm(
                    _idDoorEnvironment);
            }

            void ICreateAlarmFactoryWhenAlarmWasEnabled.CreateEvent()
            {

            }
        }

        public DeIntrusionAlarm()
        {

        }

        public DeIntrusionAlarm(Guid idDoorEnvironment)
            : base(
                new AlarmKey(
                    AlarmType.DoorEnvironment_Intrusion,
                    new IdAndObjectType(
                        idDoorEnvironment,
                        ObjectType.DoorEnvironment)),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idDoorEnvironment)
        {
            return new AlarmKey(
                AlarmType.DoorEnvironment_Intrusion,
                new IdAndObjectType(
                    idDoorEnvironment,
                    ObjectType.DoorEnvironment));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedDeIntrusion(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedDeIntrusion(
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

            var doorEnvironment = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.DoorEnvironment,
                (Guid) AlarmKey.AlarmObject.Id) as DoorEnvironment;

            if (doorEnvironment == null)
                return null;

            return new[]
            {
                CatAlarmsManager.CreateDeAlarm(
                    AlarmEventCode.DoorIntrusion,
                    this,
                    doorEnvironment,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
