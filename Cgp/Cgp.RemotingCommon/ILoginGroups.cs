using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.RemotingCommon
{
    public interface ILoginGroups : IBaseOrmTable<LoginGroup>
    {
        ICollection<LoginGroupShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);

        IList<IModifyObject> ListModifyObjectsFromLoginReferencesSites(
            Guid? idLogin,
            out Exception error);

        bool IsLoginGroupSuperAdmin(string loginGroupName);
        bool GetAccessControl(string loginGroupName, Access access);
        IdAndObjectType GetLoginGroupByName(string loginGroupName);
        ICollection<ObjectInSiteInfo> GetLoginsOfLoginGroup(Guid loginGroupId);
    }
}
