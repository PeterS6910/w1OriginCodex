using System;

namespace Contal.Cgp.Server.Alarms
{
    public interface ICreateServerAlarm
    {
        ServerAlarm CreateServerAlarm(Guid idOwner);
    }
}
