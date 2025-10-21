using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;

using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(801)]
    public class CcuOfflineServerAlarm : ServerAlarm
    {
        private CcuOfflineServerAlarm()
        {
            
        }

        private CcuOfflineServerAlarm(CCU ccu)
            : base(
                new ServerAlarmCore(
                    new Alarm(
                        CreateAlarmKey(ccu.IdCCU),
                        AlarmState.Alarm),
                    string.Format(
                        "{0} : {1}",
                        AlarmType.CCU_Offline,
                        ccu.ToString()),
                    ccu.Name,
                    string.Format(
                        "CCU offline: {0}",
                        ccu.Name)))
        {

        }

        public static void AddAlarm(CCU ccu)
        {
            AlarmsManager.Singleton.AddAlarm(
                new CcuOfflineServerAlarm(ccu));
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

        private static AlarmKey CreateAlarmKey(Guid idCcu)
        {
            return new AlarmKey(
                AlarmType.CCU_Offline,
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU));
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.AlarmCCUOfflinePresentationGroup();
        }
    }
}
