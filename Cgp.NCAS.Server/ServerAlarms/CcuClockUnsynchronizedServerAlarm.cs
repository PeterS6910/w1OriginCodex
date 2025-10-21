using System;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;

using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(803)]
    public class CcuClockUnsynchronizedServerAlarm : ServerAlarm
    {
        private CcuClockUnsynchronizedServerAlarm()
        {
            
        }

        private CcuClockUnsynchronizedServerAlarm(
            CCU ccu,
            DateTime dateTimeCcu,
            DateTime dateTimeServer)
            : base(
                new ServerAlarmCore(
                    new Alarm(
                        CreateAlarmKey(ccu.IdCCU),
                        dateTimeServer,
                        AlarmState.Alarm),
                    string.Format(
                        "{0} : {1}",
                        AlarmType.CCU_ClockUnsynchronized,
                        ccu.ToString()),
                    ccu.Name,
                    string.Format(
                        "CCU clock unsynchronized: {0}",
                        ccu.Name)))
        {
            ServerAlarmCore.Alarm.AlarmKey.SetParameters(
                Enumerable.Repeat(
                    new AlarmParameter(
                        ParameterType.DateTimeCcu,
                        dateTimeCcu.ToString()),
                    1)
                    .Concat(Enumerable.Repeat(
                        new AlarmParameter(
                            ParameterType.DateTimeServer,
                            dateTimeServer.ToString()),
                        1)));
        }

        public static void AddAlarm(
            CCU ccu,
            DateTime dateTimeCcu,
            DateTime dateTimeServer)
        {
            AlarmsManager.Singleton.AddAlarm(
                new CcuClockUnsynchronizedServerAlarm(
                    ccu,
                    dateTimeCcu,
                    dateTimeServer));
        }

        public static void StopAlarm(Guid idCcu)
        {
            AlarmsManager.Singleton.TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.StopAlarm(CreateAlarmKey(idCcu)));
        }

        public static void RemoveAlarm(Guid idCcu)
        {
            AlarmsManager.Singleton.TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.RemoveAlarm(CreateAlarmKey(idCcu)));
        }

        private static AlarmKey CreateAlarmKey(
            Guid idCcu)
        {
            return new AlarmKey(
                AlarmType.CCU_ClockUnsynchronized,
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU));
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.AlarmCcuClockUnsynchronizedPresentationGroup();
        }
    }
}
