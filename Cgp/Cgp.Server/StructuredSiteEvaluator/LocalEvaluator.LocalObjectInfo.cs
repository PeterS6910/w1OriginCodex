using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    partial class LocalEvaluator
    {
        public class LocalObjectInfo :
            AObjectInfo<
                LocalObjectInfo,
                GlobalEvaluator.GlobalObjectInfo,
                SaveableReadOnlyProxySubset<GlobalEvaluator.GlobalObjectInfo>,
                SaveableSubset<GlobalEvaluator.GlobalObjectInfo>,
                LocalSiteInfo,
                SaveableSubset<int>>
        {
            public GlobalEvaluator.GlobalObjectInfo GlobalObjectInfo { get; private set; }

            public LocalObjectInfo(
                object objectId,
                ObjectType objectType,
                string name,
                IdAndObjectType parent,
                IEnumerable<GlobalEvaluator.GlobalObjectInfo> children,
                LocalSiteInfo localSiteInfo)
                : base(
                    objectId,
                    objectType,
                    localSiteInfo,
                    new SaveableReadOnlyProxySubset<GlobalEvaluator.GlobalObjectInfo>(),
                    new SaveableReadOnlyProxySubset<GlobalEvaluator.GlobalObjectInfo>(),
                    new SaveableSubset<GlobalEvaluator.GlobalObjectInfo>(),
                    new SaveableSubset<GlobalEvaluator.GlobalObjectInfo>(),
                    new SaveableSubset<int>())
            {
                _localName = name;
                _parent = parent;
                _children = children;
            }

            public LocalObjectInfo(
                GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
                LocalSiteInfo localSiteInfo)
                : base(
                    globalObjectInfo.Id,
                    globalObjectInfo.ObjectType,
                    localSiteInfo,
                    new SaveableReadOnlyProxySubset<GlobalEvaluator.GlobalObjectInfo>(globalObjectInfo.DirectReferencesSet),
                    new SaveableReadOnlyProxySubset<GlobalEvaluator.GlobalObjectInfo>(globalObjectInfo.BackReferencesSet),
                    new SaveableSubset<GlobalEvaluator.GlobalObjectInfo>(globalObjectInfo.BrokenDirectReferencesSet),
                    new SaveableSubset<GlobalEvaluator.GlobalObjectInfo>(globalObjectInfo.BrokenBackReferencesSet),
                    new SaveableSubset<int>(globalObjectInfo.SubTreeReferenceSitesSet))
            {
                GlobalObjectInfo = globalObjectInfo;
            }

            private string _localName;

            public void SetLocalName(string newLocalName)
            {
                _localName = newLocalName;
            }

            public override string Name
            {
                get
                {
                    if (GlobalObjectInfo != null && _localName == null)
                        return GlobalObjectInfo.Name;
                    else
                        return _localName;
                }
            }

            private IdAndObjectType _parent;

            public override IdAndObjectType Parent
            {
                get
                {
                    if (GlobalObjectInfo != null)
                        return GlobalObjectInfo.Parent;
                    else
                        return _parent;
                }
            }

            private IEnumerable<GlobalEvaluator.GlobalObjectInfo> _children;
            public override IEnumerable<GlobalEvaluator.GlobalObjectInfo> Children
            {
                get
                {
                    if (GlobalObjectInfo != null)
                        return GlobalObjectInfo.Children;
                    else
                        return _children;
                }
            }

            public void RenumberSubTreeReferenceSiteId(
                LocalSiteInfo localSiteInfo,
                int oldId)
            {
                _subTreeReferenceSites.Remove(oldId);
                _subTreeReferenceSites.Add(localSiteInfo.Id);
            }

            public void UpdateSets()
            {
                _subTreeReferenceSites.Update();

                _brokenBackReferences.Update();

                _brokenDirectReferences.Update();
            }

            public void OnSaved(GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
            {
                if (globalObjectInfo == null)
                    return;

                if (globalObjectInfo != null)
                {
                    GlobalObjectInfo = globalObjectInfo;

                    _directReferences.OnSaved(GlobalObjectInfo.DirectReferencesSet);
                    _backReferences.OnSaved(GlobalObjectInfo.BackReferencesSet);

                    _brokenDirectReferences.OnSaved(GlobalObjectInfo.BrokenDirectReferencesSet);
                    _brokenBackReferences.OnSaved(GlobalObjectInfo.BrokenBackReferencesSet);

                    _subTreeReferenceSites.OnSaved(GlobalObjectInfo.SubTreeReferenceSitesSet);

                    _parent = null;
                    _children = null;
                }

                _localName = null;
            }
        }
    }
}
