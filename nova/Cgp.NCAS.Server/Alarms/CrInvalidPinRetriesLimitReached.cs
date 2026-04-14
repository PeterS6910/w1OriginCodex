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
    [LwSerialize(761)]
    public class CrInvalidPinRetriesLimitReached : Alarm, ICreateServerAlarm
    {
        private CrInvalidPinRetriesLimitReached()
        {

        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            var card = Cards.Singleton.GetById(AlarmKey.AlarmObject.Id);

            if (card == null)
                return null;

            if (AlarmKey.ExtendedObjects == null)
                return null;

            var cardReaderIdAndObjectType =
                AlarmKey.ExtendedObjects.FirstOrDefault(
                    idAndObjectType =>
                        idAndObjectType.ObjectType == ObjectType.CardReader);

            if (cardReaderIdAndObjectType == null)
                return null;

            var cardReader = CardReaders.Singleton.GetById(cardReaderIdAndObjectType.Id);

            if (cardReader == null)
                return null;

            return CrInvalidPinRetriesLimitReachedServerAlarm.CreateCrInvalidPinRetriesLimitReachedServerAlarm(
                ccu,
                card,
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
    }
}
