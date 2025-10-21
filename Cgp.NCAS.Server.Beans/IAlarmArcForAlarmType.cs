using System;

namespace Contal.Cgp.NCAS.Server.Beans
{
    public interface IAlarmArcForAlarmType
    {
        Guid IdAlarmArc { get; }
        byte AlarmType { get; }
    }
}
