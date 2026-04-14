using System.Collections.Generic;

using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICalendarDateSettings : IBaseOrmTable<CalendarDateSetting>
    {
        ICollection<DayType> GetDayTypeByCalendar(Calendar calendar);
        ICollection<DayType> GetDayTypeUniqueByCalendar(Calendar calendar);
    }
}

