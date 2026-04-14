using System;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(743)]
    public class CrUnknownCardAlarm : Alarm, ICreateServerAlarm
    {
        private CrUnknownCardAlarm()
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

            var cardNumberParameter = AlarmKey.Parameters.FirstOrDefault(
                alarmParameter =>
                    alarmParameter.TypeParameter == ParameterType.CardNumber);

            var fullCardNumber = cardNumberParameter != null
                ? cardNumberParameter.Value
                : null;

            if (fullCardNumber == null)
                return null;

            return CrUnknownCardServerAlarm.CreateCrUnknownCardServerAlarm(
                ccu,
                cardReader,
                fullCardNumber,
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
