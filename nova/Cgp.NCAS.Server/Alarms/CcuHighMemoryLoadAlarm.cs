using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(731)]
    public class CcuHighMemoryLoadAlarm : Alarm, ICreateServerAlarm
    {
        private CcuHighMemoryLoadAlarm()
        {

        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            return new ServerAlarm(
                new ServerAlarmCore(
                    idCcu,
                    new Alarm(
                        Id,
                        AlarmKey,
                        CreatedDateTime,
                        AlarmState,
                        IsAcknowledged,
                        IsBlockedGeneral,
                        IsBlockedIndividual),
                    GetName(ccu),
                    ccu.Name,
                    "CCU high memory load"));
        }

        public static string GetName(CCU ccu)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.CCU_HighMemoryLoad,
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
    }
}
