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
    [LwSerialize(824)]
    public class CrAccessDeniedServerAlarm : ServerAlarm
    {
        private CrAccessDeniedServerAlarm()
        {
        }

        private CrAccessDeniedServerAlarm(
            CCU ccu,
            CardReader cardReader,
            Card card,
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
                    GetName(cardReader),
                    ccu.Name,
                    description))
        {
        }

        private CrAccessDeniedServerAlarm(
            CCU ccu,
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
                    GetName(cardReader),
                    ccu.Name,
                    description))
        {
        }

        public static CrAccessDeniedServerAlarm CreateCrAccessDeniedServerAlarm(
            CCU ccu,
            CardReader cardReader,
            Card card,
            Alarm alarm)
        {
            var description = string.Format(
                "Access denied on card reader: {0} for card: {1}",
                cardReader.ToString(),
                card.GetFullCardNumber());

            if (card.CardSystem != null)
            {
                description += string.Format(
                    ", card system: {0}",
                    card.CardSystem.Name);
            }

            if (card.Person != null)
            {
                description += string.Format(
                    " and person: {0}",
                    card.Person.ToString());
            }

            return new CrAccessDeniedServerAlarm(
                ccu,
                cardReader,
                card,
                alarm,
                description);
        }

        public static CrAccessDeniedServerAlarm CreateCrAccessDeniedServerAlarm(
            CCU ccu,
            CardReader cardReader,
            Person person,
            Alarm alarm)
        {
            var description = string.Format(
                "Access denied on card reader: {0} for person: {1}",
                cardReader.ToString(),
                person.ToString());

            return new CrAccessDeniedServerAlarm(
                ccu,
                cardReader,
                alarm,
                description);
        }

        public static string GetName(CardReader cardReader)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.CardReader_AccessDenied,
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

            var doorEnvironment = DoorEnvironments.Singleton.GetDoorEnvironmentForCardReader(cardReader.IdCardReader);

            if (doorEnvironment != null)
            {
                parentObjects = parentObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            doorEnvironment.IdDoorEnvironment,
                            ObjectType.DoorEnvironment),
                        1));
            }

            return parentObjects;
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            CardReader cardReader,
            Card card)
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

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            CardReader cardReader,
            Person person)
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

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        person.IdPerson,
                        ObjectType.Person),
                    1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.AlarmCrAccessDeniedPresentationGroup();
        }
    }
}
