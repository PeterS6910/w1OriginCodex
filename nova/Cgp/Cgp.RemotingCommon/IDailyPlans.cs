using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IDailyPlans : IBaseOrmTable<DailyPlan>
    {
        DailyPlan GetDailyPlanAlwaysON();

        ICollection<DailyPlanShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
    }
}
