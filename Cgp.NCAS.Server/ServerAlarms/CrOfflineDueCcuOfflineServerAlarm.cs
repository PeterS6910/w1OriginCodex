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
    [LwSerialize(822)]
    public class CrOfflineDueCcuOfflineServerAlarm : ServerAlarm
    {
        private CrOfflineDueCcuOfflineServerAlarm()
        {
            
        }

        private CrOfflineDueCcuOfflineServerAlarm(
            CCU ccu,
            CardReader cardReader)
            : base(
                new ServerAlarmCore(
                    new Alarm(
                        CreateAlarmKey(cardReader.IdCardReader),
                        AlarmState.Alarm),
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(
                            ccu.IdCCU,
                            cardReader)),
                    string.Format(
                        "{0} : {1}",
                        AlarmType.CardReader_Offline_Due_CCU_Offline,
                        cardReader.ToString()),
                    ccu.Name,
                    string.Format(
                        "Card reader is offline due CCU offline: {0}",
                        cardReader.ToString())))
        {

        }

        private static IEnumerable<IdAndObjectType> GetParentObjects(
            Guid idCcu,
            CardReader cardReader)
        {
            var parentObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            if (cardReader.DCU != null)
            {
                parentObjects = parentObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            cardReader.DCU.IdDCU,
                            ObjectType.DCU),
                        1));
            }

            return parentObjects;
        }

        public static void AddAlarm(
            CCU ccu,
            CardReader cardReader)
        {
            AlarmsManager.Singleton.AddAlarm(
                new CrOfflineDueCcuOfflineServerAlarm(
                    ccu,
                    cardReader));
        }

        public static void StopAlarm(Guid idCardReader)
        {
            AlarmsManager.Singleton.TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.StopAlarm(
                        CreateAlarmKey(idCardReader)));
        }

        private static AlarmKey CreateAlarmKey(Guid idCardReader)
        {
            return new AlarmKey(
                AlarmType.CardReader_Offline_Due_CCU_Offline,
                new IdAndObjectType(
                    idCardReader,
                    ObjectType.CardReader));
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.AlarmCrOfflinePresentationGroup();
        }
    }
}
