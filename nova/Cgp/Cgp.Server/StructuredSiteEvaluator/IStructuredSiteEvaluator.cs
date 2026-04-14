using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public interface IStructuredSiteEvaluator<TObjectInfo, TSiteInfo>
    {
        TObjectInfo GetObjectInfo(IdAndObjectType idAndObjectType);
        TSiteInfo GetSiteInfo(int siteId);

        void EvaluateDirectReferences(TObjectInfo objectInfo);

        void EvaluateBackReferences(
            TObjectInfo targetObjectInfo,
            IEnumerable<int> initialInfeasibleSites);
 
        TSiteInfo Root { get; }

        void AddBrokenReference(
            TObjectInfo sourceObjectInfo, 
            TObjectInfo targetObjectInfo);
    }
}