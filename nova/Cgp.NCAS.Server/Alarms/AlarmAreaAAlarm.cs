using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(723)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAreaAAlarm : Alarm, ICreateServerAlarm
    {
        public int CountOfSensorsInAlarm { get; private set; } 

        private AlarmAreaAAlarm()
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

            return new AlarmAreaAServerAlarm(
                ccu,
                alarmArea,
                CountOfSensorsInAlarm,
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
