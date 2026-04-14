using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(841)]
    public class CrInvalidPinRetriesLimitReachedServerAlarm : ServerAlarm
    {
        private CrInvalidPinRetriesLimitReachedServerAlarm()
        {

        }

        private CrInvalidPinRetriesLimitReachedServerAlarm(
            CCU ccu,
            Card card,
            CardReader cardReader,
            Alarm alarm,
            string description)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(
                            ccu.IdCCU,
                            cardReader)),
                    card.Person != null
                        ? new LinkedList<IdAndObjectType>(
                            Enumerable.Repeat(
                                new IdAndObjectType(
                                    card.Person.IdPerson,
                                    ObjectType.Person),
                                1))
                        : null,
                    GetName(card),
                    ccu.Name,
                    description))
        {

        }

        public static CrInvalidPinRetriesLimitReachedServerAlarm CreateCrInvalidPinRetriesLimitReachedServerAlarm(
            CCU ccu,
            Card card,
            CardReader cardReader,
            Alarm alarm)
        {
            var description = new StringBuilder();

            description.AppendFormat(
                "Invalid PIN retries limit reached for card: {0}",
                card.GetFullCardNumber());

            if (card.CardSystem != null)
            {
                description.AppendFormat(
                    ", card system: {0}",
                    card.CardSystem.Name);
            }

            if (card.Person != null)
            {
                description.AppendFormat(
                    " and person: {0}",
                    card.Person.ToString());
            }

            return new CrInvalidPinRetriesLimitReachedServerAlarm(
                ccu,
                card,
                cardReader,
                alarm,
                description.ToString());
        }

        public static string GetName(Card card)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached,
               card.ToString());
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
            Card card,
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

            if (card.Person != null)
            {
                relatedObjects = relatedObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            card.Person.IdPerson,
                            ObjectType.Person),
                        1));
            }

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        card.IdCard,
                        ObjectType.Card),
                    1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.PgAlarmInvalidPinRetriesLimitReached();
        }
    }
}
