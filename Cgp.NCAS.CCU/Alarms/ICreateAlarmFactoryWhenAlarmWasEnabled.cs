using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    internal interface ICreateAlarmFactoryWhenAlarmWasEnabled
    {
        Alarm CreateAlarm(bool processEvent);
        void CreateEvent();
    }
}
