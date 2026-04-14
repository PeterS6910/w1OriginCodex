using System;
using System.Collections.Generic;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IDayTypes : IBaseOrmTable<DayType>
    {
        ICollection<DayTypeShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
    }
}
