using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    public interface IAlarmsStorage
    {
        IEnumerable<Alarm> GetAllAlarms();
        void SaveAlarm(Alarm alarm);
        void UpdateAlarm(Alarm alarm);
        void DeleteAlarm(Alarm alarm);
    }
}
