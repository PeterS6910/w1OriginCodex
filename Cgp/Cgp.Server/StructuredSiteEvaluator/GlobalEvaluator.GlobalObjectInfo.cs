using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    partial class GlobalEvaluator
    {
        public class GlobalObjectInfo :
            AObjectInfo<
                GlobalObjectInfo,
                GlobalObjectInfo,
                Subset<GlobalObjectInfo>,
                Subset<GlobalObjectInfo>,
                GlobalSiteInfo,
                Subset<int>>
        {
            private string _name;
            private GlobalObjectInfo _parent;

            public GlobalObjectInfo(
                    object objectId,
                    ObjectType objectType,
                    GlobalSiteInfo siteInfo,
                    string name)
                : base(
                    objectId,
                    objectType,
                    siteInfo,
                    new Subset<GlobalObjectInfo>(),
                    new Subset<GlobalObjectInfo>(),
                    new Subset<GlobalObjectInfo>(),
                    new Subset<GlobalObjectInfo>(),
                    new Subset<int>())
            {
                _name = name;
            }

            public Subset<GlobalObjectInfo> DirectReferencesSet
            {
                get { return _directReferences; }
            }

            public Subset<GlobalObjectInfo> BackReferencesSet
            {
                get { return _backReferences; }
            }

            public Subset<GlobalObjectInfo> BrokenDirectReferencesSet
            {
                get { return _brokenDirectReferences; }
            }

            public Subset<GlobalObjectInfo> BrokenBackReferencesSet
            {
                get { return _brokenBackReferences; }
            }

            public override string Name
            {
                get { return _name; }
            }

            public override IdAndObjectType Parent
            {
                get { return _parent; }
            }

            private readonly ICollection<GlobalObjectInfo> _children =
                new HashSet<GlobalObjectInfo>();

            public override IEnumerable<GlobalObjectInfo> Children
            {
                get { return _children; }
            }

            public void AddChild(GlobalObjectInfo childObjectInfo)
            {
                _children.Add(childObjectInfo);
            }

            public void RemoveChild(GlobalObjectInfo childObjectInfo)
            {
                _children.Remove(childObjectInfo);
            }

            public Subset<int> SubTreeReferenceSitesSet
            {
                get { return _subTreeReferenceSites; }
            }

            public void SetName(string name)
            {
                _name = name;
            }

            public void SetParent(GlobalObjectInfo parentObjectInfo)
            {
                if (_parent != null)
                    _parent.RemoveChild(this);

                _parent = parentObjectInfo;

                if (parentObjectInfo != null)
                    parentObjectInfo.AddChild(this);
            }

            public void AddDirectReference(GlobalObjectInfo targetObjectInfo)
            {
                _directReferences.Add(targetObjectInfo);
            }

            public void AddBackReference(GlobalObjectInfo sourceObjectInfo)
            {
                _backReferences.Add(sourceObjectInfo);
            }

            public void RemoveBackReference(GlobalObjectInfo objectInfo)
            {
                if (_backReferences.Remove(objectInfo))
                    _brokenBackReferences.Remove(objectInfo);
            }

            public bool RemoveDirectReference(GlobalObjectInfo objectInfo)
            {
                return 
                    _directReferences.Remove(objectInfo) && 
                    _brokenDirectReferences.Remove(objectInfo);
            }
        }
    }
}
