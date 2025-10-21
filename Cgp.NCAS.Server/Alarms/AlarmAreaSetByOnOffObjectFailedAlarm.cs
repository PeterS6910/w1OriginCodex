using System;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(758)]
    public class AlarmAreaSetByOnOffObjectFailedAlarm : Alarm, ICreateServerAlarm
    {
        private AlarmAreaSetByOnOffObjectFailedAlarm()
        {

        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            var alarmArea = AlarmAreas.Singleton.GetById(AlarmKey.AlarmObject.Id);

            if (alarmArea == null)
                return null;

            if (AlarmKey.ExtendedObjects == null)
                return null;

            var onOffObject = AlarmKey.ExtendedObjects.First();

            return AlarmAreaSetByOnOffObjectFailedServerAlarm.CreateAlarmAreaSetByOnOffObjectFailedServerAlarm(
                ccu,
                alarmArea,
                onOffObject,
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
