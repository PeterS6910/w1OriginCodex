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
    [LwSerialize(745)]
    public class CrInvalidPinAlarm : Alarm, ICreateServerAlarm
    {
        private CrInvalidPinAlarm()
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

            var card = cardIdAndObjectType != null
                ? Cards.Singleton.GetById(cardIdAndObjectType.Id)
                : null;

            if (card == null)
                return null;

            return CrInvalidPinServerAlarm.CreateCrInvalidPinServerAlarm(
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
    }
}
