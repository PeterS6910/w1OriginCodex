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
    [LwSerialize(804)]
    public class CcuTamperServerAlarm : ServerAlarm
    {
        private CcuTamperServerAlarm()
        {

        }

        public CcuTamperServerAlarm(
            CCU ccu,
            Alarm alarm)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    GetName(ccu),
                    ccu.Name,
                    string.Format(
                        "CCU tamper: {0}",
                        ccu.Name)))
        {

        }

        public static string GetName(CCU ccu)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.CCU_TamperSabotage,
                ccu.ToString());
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu)
        {
            return Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.AlarmCcuTamperSabotagePresentationGroup();
        }
    }
}
