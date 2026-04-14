using System;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(756)]
    public class CcuCatUnreachableAlarm :  Alarm, ICreateServerAlarm
    {
        private CcuCatUnreachableAlarm()
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

            return CcuCatUnreachableServerAlarm.CreateCcuCatUnreachableServerAlarm(
                ccu,
                new Alarm(
                    Id,
                    AlarmKey,
                    CreatedDateTime,
                    AlarmState,
                    IsAcknowledged,
                    IsBlockedGeneral,
                    IsBlockedIndividual),
                alarmTransmitterIpAddress);
        }
    }
}
