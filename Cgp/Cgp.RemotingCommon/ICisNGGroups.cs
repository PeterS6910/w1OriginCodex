using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICisNGGroups : IBaseOrmTable<CisNGGroup>
    {
        ICollection<CisNG> GetCisNGs(CisNGGroup cisNGGroup);
        bool AddCisNG(CisNGGroup cisNGGroup, CisNG cisNG);
        bool RemoveCisNG(CisNGGroup cisNGGroup, CisNG cisNG);
        bool Merge(CisNGGroup obj);

        ICollection<CisNGGroupShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
    }
}