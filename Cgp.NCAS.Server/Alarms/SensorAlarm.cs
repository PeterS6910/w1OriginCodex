using System;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    [LwSerialize(759)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class SensorAlarm : Alarm, ICreateServerAlarm
    {
        private SensorAlarm()
        {
            
        }

        public ServerAlarm CreateServerAlarm(Guid idCcu)
        {
            var ccu = CCUs.Singleton.GetById(idCcu);

            if (ccu == null)
                return null;

            var input = Inputs.Singleton.GetById(AlarmKey.AlarmObject.Id);

            if (input == null)
                return null;

            var alarmAreaIdAndObjectType = AlarmKey.ExtendedObjects != null
                ? AlarmKey.ExtendedObjects.FirstOrDefault(
                    idAndObjectType =>
                        idAndObjectType.ObjectType == ObjectType.AlarmArea)
                : null;

            if (alarmAreaIdAndObjectType == null)
                return null;

            return new SensorServerAlarm(
                ccu,
                input,
                (Guid) alarmAreaIdAndObjectType.Id,
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
