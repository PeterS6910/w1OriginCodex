using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    partial class GlobalEvaluator
    {
        public class GlobalSiteInfo : ASiteInfo<GlobalSiteInfo, Subset<IdAndObjectType>>
        {
            private readonly int _id;

            public GlobalSiteInfo(int id)
                : base(
                    new Subset<IdAndObjectType>(),
                    new Subset<IdAndObjectType>())
            {
                _id = id;
            }

            public Subset<IdAndObjectType> SubTreeReferencesSet
            {
                get { return _subTreeReferences; }
            }

            public override int Id
            {
                get { return _id; }
            }

            public override string Name { get; set; }

            protected override GlobalSiteInfo This
            {
                get { return this; }
            }

            public Subset<IdAndObjectType> ObjectsSet
            {
                get { return _objects; }
            }
        }
    }
}
