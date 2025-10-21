using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Alarms
{
    public interface IServerAlarmsOwner : IAlarmsOwner
    {
        event Action<ServerAlarm> AlarmAdded;

        event Action<ServerAlarm> AlarmStopped;

        void LoadServerAlarmsFromDatabase();

        void AddAlarm(
            ServerAlarm serverAlarm,
            ServerAlarm referencedAlarm);

        void StopAlarm(
            AlarmKey alarmKey);
        
        void StopAlarmsForAlarmType(AlarmType alarmType);

        void StopAlarmsForAlarmType(
            AlarmType alarmType,
            IEnumerable<IdAndObjectType> alarmObjects);

        /// <summary>
        /// Removes alarm
        /// </summary>
        /// <param name="alarmKey"></param>
        void RemoveAlarm(AlarmKey alarmKey);

        void WriteEventlogAlarmOccured(ServerAlarm serverAlarm);

        void WriteEventlogAlarmChanged(ServerAlarm serverAlarm);
    }
}
