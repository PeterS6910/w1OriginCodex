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
    [LwSerialize(833)]
    public class AlarmAreaServerAlarm : ServerAlarm
    {
        private AlarmAreaServerAlarm()
        {
            
        }

        public AlarmAreaServerAlarm(
            CCU ccu,
            AlarmArea alarmArea,
            Alarm alarm)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(ccu.IdCCU)),
                    GetName(alarmArea),
                    ccu.Name,
                    string.Format(
                        "Alarm area '{0}' is in alarm",
                        alarmArea.ToString())))
        {

        }

        public static string GetName(AlarmArea alarmArea)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.AlarmArea_Alarm,
                alarmArea.ToString());
        }

        private static IEnumerable<IdAndObjectType> GetParentObjects(
            Guid idCcu)
        {
            var parentObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            return parentObjects;
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            AlarmArea alarmArea)
        {
            var relatedObjects = GetParentObjects(idCcu);

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        alarmArea.IdAlarmArea,
                        ObjectType.AlarmArea),
                    1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            var alarmArea = AlarmAreas.Singleton.GetById(ServerAlarmCore.Alarm.AlarmKey.AlarmObject.Id);

            return alarmArea != null && alarmArea.PresentationGroup != null
                ? alarmArea.PresentationGroup
                : DevicesAlarmSettings.Singleton.AlarmAreaAlarmPresentationGroup();
        }
    }
}
