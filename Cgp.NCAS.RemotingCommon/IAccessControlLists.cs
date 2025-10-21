using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IAccessControlLists : IBaseOrmTable<AccessControlList>
    {
        ICollection<AccessControlListShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);

        IList<IModifyObject> ListModifyObjects(out Exception error);
        IList<IModifyObject> ListModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
    }
}
