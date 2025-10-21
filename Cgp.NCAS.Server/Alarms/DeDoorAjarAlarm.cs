using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(740)]
    public class DeDoorAjarAlarm : Alarm, ICreateServerAlarm
    {
        private DeDoorAjarAlarm()
        {
            
        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            var doorEnvironment = DoorEnvironments.Singleton.GetById(AlarmKey.AlarmObject.Id);

            if (doorEnvironment == null)
                return null;

            return new DeDoorAjarServerAlarm(
                ccu,
                doorEnvironment,
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
