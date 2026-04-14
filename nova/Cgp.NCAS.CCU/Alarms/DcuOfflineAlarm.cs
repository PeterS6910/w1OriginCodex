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
    [LwSerialize(725)]
    public class DcuOfflineAlarm :
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
                return new DcuOfflineAlarm(
                    _idDcu);
            }

            void ICreateAlarmFactoryWhenAlarmWasEnabled.CreateEvent()
            {

            }
        }

        public DcuOfflineAlarm()
        {

        }

        public DcuOfflineAlarm(
            Guid idDcu)
            : base(
                new AlarmKey(
                    AlarmType.DCU_Offline,
                    new IdAndObjectType(
                        idDcu,
                        ObjectType.DCU)),
               AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(Guid idDcu)
        {
            return new AlarmKey(
                AlarmType.DCU_Offline,
                new IdAndObjectType(
                    idDcu,
                    ObjectType.DCU));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedDcuOfflineAlarm(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedDcuOfflineAlarm(
                false,
                this);
        }

        public static void Update(
            DCU dcu,
            bool isOnline)
        {
            if (!isOnline)
            {
                AlarmsManager.Singleton.StopAlarmsForAlarmObjects(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            dcu.IdDCU,
                            ObjectType.DCU),
                        1));

                if (dcu.GuidInputs != null)
                {
                    AlarmsManager.Singleton.StopAlarmsForAlarmObjects(
                        dcu.GuidInputs.Select(
                            idInput =>
                                new IdAndObjectType(
                                    idInput,
                                    ObjectType.Input)));
                }

                if (dcu.GuidOutputs != null)
                {
                    AlarmsManager.Singleton.StopAlarmsForAlarmObjects(
                        dcu.GuidOutputs.Select(
                            idOutput =>
                                new IdAndObjectType(
                                    idOutput,
                                    ObjectType.Output)));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new DcuOfflineAlarm(
                        dcu.IdDCU));

                return;
            }

            AlarmsManager.Singleton.StopAlarm(
                CreateAlarmKey(
                    dcu.IdDCU));
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
                    AlarmEventCode.SlaveDeviceOffline,
                    this,
                    dcu,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }
    }
}
