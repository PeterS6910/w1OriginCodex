using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IACLGroups : IBaseOrmTable<ACLGroup>
    {
        ICollection<ACLGroupShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
    }
}
