using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(727)]
    public class IccuSendingOfObjectStateFailedAlarm : Alarm, ICreateServerAlarm
    {
        private IccuSendingOfObjectStateFailedAlarm()
        {
            
        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            if (AlarmKey.AlarmObject == null)
                return null;

            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            var iTableOrm = CgpServerRemotingProvider.Singleton.GetTableOrmForObjectType(AlarmKey.AlarmObject.ObjectType);

            if (iTableOrm == null)
                return null;

            var ormObject = iTableOrm.GetObjectById(AlarmKey.AlarmObject.Id);

            if (ormObject == null)
                return null;

            var parentObjects = GetParentObjects(
                idCcu,
                ormObject);

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
                    new LinkedList<IdAndObjectType>(parentObjects),
                    GetName(ormObject),
                    ccu.Name,
                    "ICCU: Sending of object state failed"));
        }

        public static string GetName(AOrmObject ormObject)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.ICCU_SendingOfObjectStateFailed,
                ormObject.ToString());
        }

        private static IEnumerable<IdAndObjectType> GetParentObjects(
            Guid idCcu,
            AOrmObject ormObject)
        {
            var parentObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            var iGetDcu = ormObject as IGetDcu;

            if (iGetDcu != null)
            {
                var dcu = iGetDcu.GetDcu();

                if (dcu != null)
                {
                    parentObjects = parentObjects.Concat(
                        Enumerable.Repeat(
                            new IdAndObjectType(
                                dcu.IdDCU,
                                ObjectType.DCU),
                            1));
                }
            }

            return parentObjects;
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            AOrmObject ormObject)
        {
            var relatedObjects = GetParentObjects(
                idCcu,
                ormObject);

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()),
                    1));

            return relatedObjects;
        }
    }
}
