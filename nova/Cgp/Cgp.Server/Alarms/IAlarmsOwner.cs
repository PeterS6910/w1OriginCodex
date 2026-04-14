using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Alarms
{
    public interface IAlarmsOwner
    {
        /// <summary>
        /// Adds alarm
        /// </summary>
        /// <param name="serverAlarm"></param>
        void AddAlarm(
            ServerAlarm serverAlarm);

        /// <summary>
        /// Finds alarm withc specific Guid
        /// </summary>
        /// <param name="idAlarm"></param>
        /// <returns></returns>
        ServerAlarm FindAlarmByIdAlarm(Guid idAlarm);

        ServerAlarm AcknowledgeAlarm(Guid idAlarm);

        ServerAlarm AcknowledgeAlarm(Guid idAlarm, bool notifyClient);

        ServerAlarm BlockAlarmIndividual(Guid idAlarm);

        ServerAlarm BlockAlarmIndividual(
            Guid idAlarm,
            bool notifyClient);

        ServerAlarm UnblockAlarmIndividual(Guid idAlarm);

        ServerAlarm UnblockAlarmIndividual(
            Guid idAlarm,
            bool notifyClient);

        bool IsAlarmBlocked(Guid idAlarm);

        void RemoveAlarmsForAlarmObjects(
            IEnumerable<IdAndObjectType> alarmObjects);

        /// <summary>
        /// Finds alarms with specific alarm type
        /// </summary>
        /// <param name="alarmType"></param>
        /// <returns></returns>
        ICollection<ServerAlarm> FindAlarmsByAlarmType(AlarmType alarmType);

        /// <summary>
        /// Returns all alarms
        /// </summary>
        /// <returns></returns>
        ICollection<ServerAlarm> GetAlarms();
        IEnumerable<ServerAlarm> GetAlarms(AlarmType alarmType, IdAndObjectType alarmObject);
    }
}
