using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(825)]
    public class CrUnknownCardServerAlarm : ServerAlarm
    {
        private CrUnknownCardServerAlarm()
        {

        }

        private CrUnknownCardServerAlarm(
            CCU ccu,
            CardReader cardReader,
            string fullCardNumber,
            Alarm alarm,
            IEnumerable<IdAndObjectType> extendedObjects,
            string description)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(
                            ccu.IdCCU,
                            cardReader)),
                    extendedObjects != null
                        ? new LinkedList<IdAndObjectType>(
                            extendedObjects)
                        : null,
                    GetName(fullCardNumber),
                    ccu.Name,
                    description))
        {

        }

        public static CrUnknownCardServerAlarm CreateCrUnknownCardServerAlarm(
            CCU ccu,
            CardReader cardReader,
            string fullCardNumber,
            Alarm alarm)
        {
            var card = Cards.Singleton.GetCardByFullNumber(fullCardNumber);

            IEnumerable<IdAndObjectType> extendedObjects = null;

            if (card != null)
            {
                extendedObjects = card.Person != null
                    ? Enumerable.Repeat(
                        new IdAndObjectType(
                            card.Person.IdPerson,
                            ObjectType.Person),
                        1)
                    : Enumerable.Empty<IdAndObjectType>();

                extendedObjects = extendedObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            card.IdCard,
                            ObjectType.Card),
                        1));
            }

            var description = string.Format(
                "Unknow card on card reader: {0}. Card number: {1}",
                cardReader.ToString(),
                fullCardNumber);

            if (card != null)
            {
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
            }

            return new CrUnknownCardServerAlarm(
                ccu,
                cardReader,
                fullCardNumber,
                alarm,
                extendedObjects,
                description);
        }

        public static string GetName(string fullCardNumber)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.CardReader_UnknownCard,
                fullCardNumber);
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

            if (card != null)
            {
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
            }

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.AlarmCrUnknownCardPresentationGroup();
        }
    }
}
