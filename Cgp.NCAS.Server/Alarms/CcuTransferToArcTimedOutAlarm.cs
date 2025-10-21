using System;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(757)]
    public class CcuTransferToArcTimedOutAlarm : Alarm, ICreateServerAlarm
    {
        private CcuTransferToArcTimedOutAlarm()
        {

        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            var alarmTransmitterIpAddressParameter = AlarmKey.Parameters.FirstOrDefault(
                alarmParameter =>
                    alarmParameter.TypeParameter == ParameterType.CatIpAddress);

            var alarmTransmitterIpAddress = alarmTransmitterIpAddressParameter != null
                ? alarmTransmitterIpAddressParameter.Value
                : null;

            if (alarmTransmitterIpAddress == null)
                return null;

            var arcNameParameter = AlarmKey.Parameters.FirstOrDefault(
                alarmParameter =>
                    alarmParameter.TypeParameter == ParameterType.ArcName);

            var arcName = arcNameParameter != null
                ? arcNameParameter.Value
                : null;

            if (arcName == null)
                return null;

            return CcuTransferToArcTimedOutServerAlarm.CreateCcuCatUnreachableServerAlarm(
                ccu,
                new Alarm(
                    Id,
                    AlarmKey,
                    CreatedDateTime,
                    AlarmState,
                    IsAcknowledged,
                    IsBlockedGeneral,
                    IsBlockedIndividual),
                alarmTransmitterIpAddress,
                arcName);
        }
    }
}
