using System;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using System.Collections.Generic;
using Contal.Cgp.Globals;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.RemotingCommon
{
    public interface ITimeZones : IBaseOrmTable<TimeZone>
    {
        ICollection<TimeZoneShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
        IList<DayInterval> GetActualDayInterval(Guid idTimeZone, DateTime dateTime);
    }
}
