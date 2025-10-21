using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Alarms
{
    public interface IExternalAlarmsOwner : IAlarmsOwner
    {
        void StopAlarmFromOwner(Guid idAlarm);

        void AcknowledgeAlarmFromOwner(Guid idAlarm);

        void BlockAlarmGeneralFromOwner(Guid idAlarm);

        void UnblockAlarmGeneralFromOwner(Guid idAlarm);

        void BlockAlarmIndividualFromOwner(Guid idAlarm, DateTime utcDateTime);

        void UnblockAlarmIndividualFromOwner(Guid idAlarm, DateTime utcDateTime);

        /// <summary>
        /// Removes alarm
        /// </summary>
        /// <param name="idAlarm"></param>
        void RemoveAlarmFromOwner(Guid idAlarm);

        void RemoveAllAlarms();

        void AlarmOwnerConnected();

        void AlarmOwnerDisconnected();
    }
}
