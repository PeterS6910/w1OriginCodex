using System.Collections.Generic;
using Contal.CatCom;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    internal interface ICreateCatAlarms
    {
        ICollection<CatAlarm> CreateCatAlarms(bool alarmAcknowledged);
    }
}
