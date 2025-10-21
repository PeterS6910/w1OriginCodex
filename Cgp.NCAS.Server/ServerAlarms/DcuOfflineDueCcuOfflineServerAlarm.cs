using System;
using System.Collections.Generic;
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
    [LwSerialize(816)]
    public class DcuOfflineDueCcuOfflineServerAlarm : ServerAlarm
    {
        private DcuOfflineDueCcuOfflineServerAlarm()
        {

        }

        private DcuOfflineDueCcuOfflineServerAlarm(DCU dcu)
            : base(
                new ServerAlarmCore(
                    new Alarm(
                        CreateAlarmKey(dcu.IdDCU),
                        AlarmState.Alarm),
                    new LinkedList<IdAndObjectType>
                        (
                        Enumerable.Repeat(
                            new IdAndObjectType(
                                dcu.CCU.IdCCU,
                                ObjectType.CCU),
                            1)),
                    string.Format(
                        "{0} : {1}",
                        AlarmType.DCU_Offline_Due_CCU_Offline,
                        dcu.ToString()),
                    dcu.CCU.Name,
                    string.Format(
                        "DCU is offline due CCU offline: {0}",
                        dcu.ToString())))
        {

        }

        private static AlarmKey CreateAlarmKey(Guid idDcu)
        {
            return new AlarmKey(
                AlarmType.DCU_Offline_Due_CCU_Offline,
                new IdAndObjectType(
                    idDcu,
                    ObjectType.DCU));
        }

        public static void AddAlarm(DCU dcu)
        {
            AlarmsManager.Singleton.AddAlarm(
                new DcuOfflineDueCcuOfflineServerAlarm(
                    dcu));
        }

        public static void StopAlarm(Guid idDcu)
        {
            AlarmsManager.Singleton.TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.StopAlarm(
                        CreateAlarmKey(idDcu)));
        }

        public override PresentationGroup GetPresentationGroup()
        {
            var dcu = DCUs.Singleton.GetById(ServerAlarmCore.Alarm.AlarmKey.AlarmObject.Id);

            return dcu != null && dcu.OfflinePresentationGroup != null
                ? dcu.OfflinePresentationGroup
                : DevicesAlarmSettings.Singleton.AlarmDcuOfflinePresentationGroup();
        }
    }
}
