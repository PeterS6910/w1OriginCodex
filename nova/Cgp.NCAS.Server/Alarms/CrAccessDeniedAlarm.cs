using System;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(742)]
    public class CrAccessDeniedAlarm : Alarm, ICreateServerAlarm
    {
        private CrAccessDeniedAlarm()
        {
            
        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            var cardReader = CardReaders.Singleton.GetById(AlarmKey.AlarmObject.Id);

            if (cardReader == null)
                return null;

            var cardIdAndObjectType =
                AlarmKey.ExtendedObjects != null
                    ? AlarmKey.ExtendedObjects.FirstOrDefault(
                        idAndObjectType =>
                            idAndObjectType.ObjectType == ObjectType.Card)
                    : null;

            if (cardIdAndObjectType != null)
            {
                var card = Cards.Singleton.GetById(cardIdAndObjectType.Id);

                if (card == null)
                    return null;

                return CrAccessDeniedServerAlarm.CreateCrAccessDeniedServerAlarm(
                    ccu,
                    cardReader,
                    card,
                    new Alarm(
                        Id,
                        AlarmKey,
                        CreatedDateTime,
                        AlarmState,
                        IsAcknowledged,
                        IsBlockedGeneral,
                        IsBlockedIndividual));
            }

            var personIdAndObjectType =
                AlarmKey.ExtendedObjects != null
                    ? AlarmKey.ExtendedObjects.FirstOrDefault(
                        idAndObjectType =>
                            idAndObjectType.ObjectType == ObjectType.Person)
                    : null;

            var person = personIdAndObjectType != null
                ? Persons.Singleton.GetById(personIdAndObjectType.Id)
                : null;

            if (person == null)
                return null;

            return CrAccessDeniedServerAlarm.CreateCrAccessDeniedServerAlarm(
                ccu,
                cardReader,
                person,
                new Alarm(
                    Id,
                    AlarmKey,
                    CreatedDateTime,
                    AlarmState,
                    IsAcknowledged,
                    IsBlockedGeneral,
                    IsBlockedIndividual));
        }
    }
}
