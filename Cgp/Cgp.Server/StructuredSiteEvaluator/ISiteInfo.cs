using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public interface ISiteInfo<TSiteInfo>
        where TSiteInfo : ISiteInfo<TSiteInfo>
    {
        int Id { get; }
        TSiteInfo ParentSite { get; set; }

        string Name { get; set; }

        IEnumerable<TSiteInfo> ChildrenSites { get; }

        void AddChild(TSiteInfo childSiteInfo);
        void RemoveChild(TSiteInfo childSiteInfo);

        void AddSubTreeReference(IdAndObjectType idAndObjectType);
        void RemoveSubTreeReference(IdAndObjectType idAndObjectType);

        bool ContainsSubTreeReference(IdAndObjectType objectInfo);

        IEnumerable<IdAndObjectType> SubTreeReferences { get; }

        void AddObject(IdAndObjectType idAndObjectType);
        void RemoveObject(IdAndObjectType idAndObjectType);

        IEnumerable<IdAndObjectType> Objects { get; }
    }
}