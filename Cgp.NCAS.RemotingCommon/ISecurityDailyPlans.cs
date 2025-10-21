using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface ISecurityDailyPlans : IBaseOrmTable<SecurityDailyPlan>
    {
        ICollection<SecurityDailyPlanShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);

        IList<IModifyObject> ListModifyObjects(out Exception error);
    }
}
