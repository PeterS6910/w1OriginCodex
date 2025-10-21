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
    [LwSerialize(747)]
    public class CrAccessPermittedAlarm : Alarm, ICreateServerAlarm
    {
        private CrAccessPermittedAlarm()
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

            if (AlarmKey.ExtendedObjects == null)
            {
                return CrAccessPermittedServerAlarm.CreateCrAccessPermittedServerAlarm(
                    ccu,
                    cardReader,
                    new Alarm(
                        Id,
                        AlarmKey,
                        CreatedDateTime,
                        AlarmState,
                        IsAcknowledged,
                        IsBlockedGeneral,
                        IsBlockedIndividual));
            }

            var cardIdAndObjectType = AlarmKey.ExtendedObjects.FirstOrDefault(
                        idAndObjectType =>
                            idAndObjectType.ObjectType == ObjectType.Card);

            if (cardIdAndObjectType != null)
            {
                var card = Cards.Singleton.GetById(cardIdAndObjectType.Id);

                if (card == null)
                    return null;

                return CrAccessPermittedServerAlarm.CreateCrAccessPermittedServerAlarm(
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

            var personIdAndObjectType = AlarmKey.ExtendedObjects.FirstOrDefault(
                        idAndObjectType =>
                            idAndObjectType.ObjectType == ObjectType.Person);

            var person = personIdAndObjectType != null
                ? Persons.Singleton.GetById(personIdAndObjectType.Id)
                : null;

            if (person == null)
                return null;

            return CrAccessPermittedServerAlarm.CreateCrAccessPermittedServerAlarm(
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
