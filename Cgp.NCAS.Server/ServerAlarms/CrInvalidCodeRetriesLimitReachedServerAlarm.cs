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
    [LwSerialize(840)]
    public class CrInvalidCodeRetriesLimitReachedServerAlarm : ServerAlarm
    {
        private CrInvalidCodeRetriesLimitReachedServerAlarm()
        {
            
        }

        public CrInvalidCodeRetriesLimitReachedServerAlarm(
            CCU ccu,
            CardReader cardReader,
            Alarm alarm)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(
                            ccu.IdCCU,
                            cardReader)),
                    GetName(cardReader),
                    ccu.Name,
                    string.Format(
                        "Invalid code retries limit reached on card reader: {0}",
                        cardReader.ToString())))
        {

        }

        public static string GetName(CardReader cardReader)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached,
                cardReader.ToString());
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

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            CardReader cardReader)
        {
            var relatedObjects = GetParentObjects(
                idCcu,
                cardReader);

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        cardReader.IdCardReader,
                        ObjectType.CardReader),
                    1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.PgAlarmInvalidGinRetriesLimitReached();
        }
    }
}
