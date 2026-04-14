using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IPresentationGroups : IBaseOrmTable<PresentationGroup>
    {
        bool ExistPresentationGroupName(string groupName);

        ICollection<CisNG> GetCisNGs(PresentationGroup pg);
        ICollection<CisNGGroup> GetCisNGGroups(PresentationGroup pg);

        bool AddCisNG(PresentationGroup pg, CisNG cisNG);
        bool RemoveCisNG(PresentationGroup pg, CisNG cisNG);
        bool AddCisNGGroup(PresentationGroup pg, CisNGGroup cisNGGroup);
        bool RemoveCisNGGroup(PresentationGroup pg, CisNGGroup cisNG);
        ICollection<CisNG> ReturnAllCisNG(PresentationGroup pg);
        void RefreshRelationWithEvent(PresentationGroup pg, string eventName);

        void SendCisInfoMessage(PresentationGroup noDbsPg, string msg);

        ICollection<PresentationGroupShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
    }
}
