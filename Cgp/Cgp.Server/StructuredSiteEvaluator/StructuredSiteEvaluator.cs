using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public abstract class StructuredSiteEvaluator<
            TObjectInfo, 
            TObjectInfoReferences,
            TObjectInfoReferencesReadOnlySet, 
            TObjectInfoReferencesSet, 
            TSiteInfo, 
            TIdAndObjectTypeSet, 
            TSiteIdSet> :
        IStructuredSiteEvaluator<TObjectInfo, TSiteInfo>
        where TObjectInfo : AObjectInfo<
            TObjectInfo,
            TObjectInfoReferences, 
            TObjectInfoReferencesReadOnlySet, 
            TObjectInfoReferencesSet, 
            TSiteInfo, 
            TSiteIdSet>
        where TObjectInfoReferencesReadOnlySet : IReadOnlySubSet<TObjectInfoReferences>
        where TObjectInfoReferencesSet : ISubset<TObjectInfoReferences>
        where TSiteInfo : class, ISiteInfo<TSiteInfo>
        where TIdAndObjectTypeSet : ISubset<IdAndObjectType>
        where TSiteIdSet : ISubset<int>
    {
        public class MoveOperation
        {
            private readonly IStructuredSiteEvaluator<TObjectInfo, TSiteInfo> _evaluator;
            private readonly TObjectInfo _objectInfo;

            private readonly TSiteInfo _oldSiteInfo;
            private readonly TSiteInfo _newSiteInfo;

            private TSiteInfo _oldPathSubTreeReference;

            private ICollection<int> _oldPathContent;

            public MoveOperation(
                IStructuredSiteEvaluator<TObjectInfo, TSiteInfo> evaluator, 
                TObjectInfo objectInfo, 
                TSiteInfo newSiteInfo)
            {
                _evaluator = evaluator;

                _objectInfo = objectInfo;
                _oldSiteInfo = _objectInfo.SiteInfo;

                _newSiteInfo = newSiteInfo;
            }

            public bool Execute()
            {
                _objectInfo.SiteInfo = _newSiteInfo;

                if (_oldSiteInfo == null || _newSiteInfo == null)
                    return false;

                if (_oldSiteInfo.Id == _newSiteInfo.Id)
                    return false;

                TraverseOldPath();

                TraverseNewPath();

                TrimOldPath();

                _evaluator.EvaluateBackReferences(
                    _objectInfo, 
                    _oldPathContent);

                _evaluator.EvaluateDirectReferences(_objectInfo);

                return true;
            }

            private void TrimOldPath()
            {
                if (_oldPathSubTreeReference == null)
                    return;

                TSiteInfo currentSite = _oldSiteInfo;

                do
                {
                    int currentSiteId = currentSite.Id;

                    if (!_oldPathContent.Contains(currentSiteId))
                        return;

                    _oldPathContent.Remove(currentSiteId);

                    if (currentSite == _oldPathSubTreeReference)
                        return;

                    currentSite = currentSite.ParentSite;
                }
                while (true);
            }

            private void TraverseOldPath()
            {
                TSiteInfo currentSite = _oldSiteInfo;

                _oldPathContent = new HashSet<int>();

                do
                {
                    _oldPathContent.Add(currentSite.Id);

                    if (currentSite.ContainsSubTreeReference(_objectInfo))
                        _oldPathSubTreeReference = currentSite;

                    if (currentSite == _newSiteInfo)
                        break;

                    if (currentSite == _evaluator.Root)
                        break;

                    currentSite = currentSite.ParentSite;
                }
                while (true);
            }            

            private void TraverseNewPath()
            {
                TSiteInfo currentSite = _newSiteInfo;

                do
                {
                    if (_oldPathContent.Contains(currentSite.Id))
                        break;

                    currentSite = currentSite.ParentSite;
                }
                while (true);

                _oldPathContent.Remove(currentSite.Id);

                while (currentSite != _evaluator.Root)
                {
                    currentSite = currentSite.ParentSite;

                    _oldPathContent.Remove(currentSite.Id);

                    if (currentSite == _newSiteInfo)
                        break;
                }
            }
        }

        private readonly IDictionary<IdAndObjectType, TObjectInfo> _objectInfos =
            new Dictionary<IdAndObjectType, TObjectInfo>();

        private readonly IDictionary<int, TSiteInfo> _siteInfos =
            new Dictionary<int, TSiteInfo>();

        private readonly HashSet<int> _feasibleSites =
                new HashSet<int>();

        private readonly HashSet<int> _subTreeFeasibleSites =
            new HashSet<int>();

        private readonly HashSet<int> _subTreeInfeasibleSites =
            new HashSet<int>();

        private readonly HashSet<int> _infeasibleSites =
            new HashSet<int>();

        private readonly ICollection<int> _fathomedSites =
            new List<int>();

        protected readonly TIdAndObjectTypeSet _objectsWithBrokenDirectReferences;

        protected StructuredSiteEvaluator(TIdAndObjectTypeSet objectsWithBrokenDirectReferences)
        {
            _objectsWithBrokenDirectReferences = objectsWithBrokenDirectReferences;
        }

        public void EvaluateBackReferences(
            TObjectInfo targetObjectInfo,
            IEnumerable<int> initialInfeasibleSites)
        {
            _subTreeFeasibleSites.Clear();
            _subTreeInfeasibleSites.Clear();

            _feasibleSites.Clear();
            _infeasibleSites.Clear();

            if (initialInfeasibleSites != null)
                foreach (var initialInfeasibleSite in initialInfeasibleSites)
                    _infeasibleSites.Add(initialInfeasibleSite);

            foreach (var backReference in GetBackReferences(targetObjectInfo))
                EvaluateBackReference(backReference, targetObjectInfo);
        }

        protected void EvaluateBackReferences(TObjectInfo targetObjectInfo)
        {
            EvaluateBackReferences(
                targetObjectInfo, 
                null);
        }

        public abstract IEnumerable<TObjectInfo> GetChildren(TObjectInfo objectInfo);

        public abstract IEnumerable<TObjectInfo> GetDirectReferences(TObjectInfo objectInfo);
        public abstract IEnumerable<TObjectInfo> GetBackReferences(TObjectInfo objectInfo);

        public void AddBrokenReference(
            TObjectInfo sourceObjectInfo,
            TObjectInfo targetObjectInfo)
        {
            _objectsWithBrokenDirectReferences.Add(
                sourceObjectInfo);

            AddBrokenReferenceInternal(
                sourceObjectInfo,
                targetObjectInfo);
        }

        protected abstract void AddBrokenReferenceInternal(
            TObjectInfo sourceObjectInfo,
            TObjectInfo targetObjectInfo);

        protected void RemoveBrokenReference(
            TObjectInfo sourceObjectInfo,
            TObjectInfo targetObjectInfo)
        {
            RemoveBrokenReferenceInternal(
                sourceObjectInfo,
                targetObjectInfo);

            if (!sourceObjectInfo.HasBrokenDirectReferences)
                _objectsWithBrokenDirectReferences.Remove(sourceObjectInfo);
        }

        protected abstract void RemoveBrokenReferenceInternal(
            TObjectInfo sourceObjectInfo,
            TObjectInfo targetObjectInfo);

        public void EvaluateDirectReferences(TObjectInfo sourceObjectInfo)
        {
            _feasibleSites.Clear();
            _infeasibleSites.Clear();

            _subTreeFeasibleSites.Clear();
            _subTreeInfeasibleSites.Clear();

            foreach (var directReference in GetDirectReferences(sourceObjectInfo))
                EvaluateDirectReference(
                    sourceObjectInfo,
                    directReference);
        }

        public abstract TSiteInfo Root { get; }

        public IEnumerable<TObjectInfo> ObjectInfos
        {
            get { return _objectInfos.Values; }
        }

        public IEnumerable<TSiteInfo> SiteInfos
        {
            get { return _siteInfos.Values; }
        }

        private bool ExistsAsSubTreeReference(
            IdAndObjectType targetIdAndObjectType,
            TSiteInfo sourceObjectSite)
        {
            TSiteInfo currentSite = sourceObjectSite;

            _fathomedSites.Clear();

            do
            {
                var currentSiteId = currentSite.Id;

                if (_subTreeInfeasibleSites.Contains(currentSiteId))
                    return false;

                if (_subTreeFeasibleSites.Contains(currentSiteId))
                    return true;

                _fathomedSites.Add(currentSiteId);

                if (currentSite.ContainsSubTreeReference(targetIdAndObjectType))
                    return true;

                currentSite = currentSite.ParentSite;
            }
            while (currentSite != null);

            return false;
        }

        private void EvaluateDirectReference(
            TObjectInfo sourceObjectInfo,
            TObjectInfo targetObjectInfo)
        {
            TSiteInfo sourceObjectSite = sourceObjectInfo.SiteInfo;

            if (ExistsAsSubTreeReference(targetObjectInfo, sourceObjectSite))
            {
                RemoveBrokenReference(
                    sourceObjectInfo,
                    targetObjectInfo);

                return;
            }

            if (EvaluateDirectReference(sourceObjectSite.Id, targetObjectInfo))
            {
                InsertFathomedSitesIntoSet(_feasibleSites);

                RemoveBrokenReference(
                    sourceObjectInfo,
                    targetObjectInfo);
            }
            else
            {
                InsertFathomedSitesIntoSet(_infeasibleSites);

                AddBrokenReference(
                    sourceObjectInfo,
                    targetObjectInfo);
            }
        }

        private bool EvaluateDirectReference(
            int sourceObjectSiteId,
            TObjectInfo targetObjectInfo)
        {
            _fathomedSites.Clear();

            TSiteInfo currentSite = targetObjectInfo.SiteInfo;

            do
            {
                var currentSiteId = currentSite.Id;

                if (_feasibleSites.Contains(currentSiteId))
                    return true;

                if (_infeasibleSites.Contains(currentSiteId))
                    break;

                _fathomedSites.Add(currentSiteId);

                if (currentSiteId == sourceObjectSiteId)
                    return true;

                currentSite = currentSite.ParentSite;
            }
            while (currentSite != null);

            return false;
        }

        private void EvaluateBackReference(
            TObjectInfo sourceObjectInfo,
            TObjectInfo targetObjectInfo)
        {
            TSiteInfo sourceObjectSite = sourceObjectInfo.SiteInfo;

            if (ExistsAsSubTreeReference(targetObjectInfo, sourceObjectSite))
            {
                InsertFathomedSitesIntoSet(_subTreeFeasibleSites);

                RemoveBrokenReference(
                    sourceObjectInfo,
                    targetObjectInfo);

                return;
            }

            InsertFathomedSitesIntoSet(_subTreeInfeasibleSites);

            var sourceObjectSiteId = sourceObjectSite.Id;

            if (_feasibleSites.Contains(sourceObjectSiteId))
            {
                RemoveBrokenReference(
                    sourceObjectInfo,
                    targetObjectInfo);

                return;
            }

            if (EvaluateBackReference(sourceObjectSiteId, targetObjectInfo))
            {
                InsertFathomedSitesIntoSet(_feasibleSites);

                RemoveBrokenReference(
                    sourceObjectInfo,
                    targetObjectInfo);
            }
            else
            {
                _infeasibleSites.Add(sourceObjectSiteId);

                AddBrokenReference(
                    sourceObjectInfo, 
                    targetObjectInfo);
            }
        }

        private void InsertFathomedSitesIntoSet(HashSet<int> set)
        {
            foreach (var fathomedSiteId in _fathomedSites)
                set.Add(fathomedSiteId);
        }

        private bool EvaluateBackReference(int sourceObjectSiteId, TObjectInfo targetObjectInfo)
        {
            _fathomedSites.Clear();

            TSiteInfo currentSite = targetObjectInfo.SiteInfo;

            if (_infeasibleSites.Contains(sourceObjectSiteId))
                return false;

            do
            {
                int currentSiteId = currentSite.Id;

                _fathomedSites.Add(currentSiteId);

                if (currentSite.Id == sourceObjectSiteId)
                    return true;

                currentSite = currentSite.ParentSite;
            }
            while (currentSite != null);

            return false;
        }

        public void RemoveSubTreeReference(
            TObjectInfo objectInfo, 
            TSiteInfo siteInfo)
        {
            if (!objectInfo.RemoveSubTreeReference(siteInfo))
                return;

            EvaluateBackReferences(objectInfo);
            EvaluateDirectReferences(objectInfo);

            OnSubTreeReferenceRemoved(
                objectInfo,
                siteInfo);
        }

        protected bool AncestralReferenceExists(
            TObjectInfo objectInfo,
            IEnumerable<TSiteInfo> siteInfos)
        {
            _subTreeFeasibleSites.Clear();
            _subTreeInfeasibleSites.Clear();

            return 
                siteInfos.Any(
                    siteInfo => ExistsAsSubTreeReference(
                        objectInfo, 
                        siteInfo));
        }

        protected bool AncestralReferenceExists(
            TObjectInfo objectInfo,
            TSiteInfo siteInfo)
        {
            _subTreeFeasibleSites.Clear();
            _subTreeInfeasibleSites.Clear();

            return ExistsAsSubTreeReference(objectInfo, siteInfo);
        }

        protected virtual void OnSubTreeReferenceRemoved(
            TObjectInfo objectInfo, 
            TSiteInfo siteInfo)
        {
        }

        protected virtual void OnSubTreeReferenceAdded(
            TObjectInfo objectInfo,
            TSiteInfo siteInfo)
        {
        }

        public void AddSubTreeReference(
            TObjectInfo objectInfo,
            TSiteInfo siteInfo)
        {
            if (!objectInfo.AddSubTreeReference(siteInfo))
                return;

            EvaluateBackReferences(objectInfo);
            EvaluateDirectReferences(objectInfo);

            OnSubTreeReferenceAdded(
                objectInfo,
                siteInfo);
        }

        public TObjectInfo GetObjectInfo(IdAndObjectType idAndObjectType)
        {
            TObjectInfo result;

            _objectInfos.TryGetValue(idAndObjectType, out result);

            return result;
        }

        public TSiteInfo GetSiteInfo(int siteId)
        {
            TSiteInfo result;

            _siteInfos.TryGetValue(siteId, out result);

            return result;
        }
        
        protected bool SiteExists(int id)
        {
            return _siteInfos.ContainsKey(id);
        }

        public void AddObjectInfo(TObjectInfo objectInfo)
        {
            _objectInfos.Add(
                objectInfo,
                objectInfo);
        }

        public bool RemoveObjectInfo(TObjectInfo objectInfo)
        {
            if (!_objectInfos.Remove(objectInfo))
                return false;

            _objectsWithBrokenDirectReferences.Remove(objectInfo);
            objectInfo.SiteInfo.RemoveObject(objectInfo);

            return true;
        }

        public void AddSiteInfo(TSiteInfo siteInfo, TSiteInfo parentSiteInfo)
        {
            siteInfo.ParentSite = parentSiteInfo;

            _siteInfos.Add(
                siteInfo.Id,
                siteInfo);
        }

        protected void RemoveSiteInfo(TSiteInfo siteInfo)
        {
            siteInfo.ParentSite = null;
            _siteInfos.Remove(siteInfo.Id);
        }

        protected void Clear()
        {
            _objectInfos.Clear();
            _siteInfos.Clear();

            _fathomedSites.Clear();

            _feasibleSites.Clear();
            _infeasibleSites.Clear();

            _subTreeFeasibleSites.Clear();
            _subTreeInfeasibleSites.Clear();
        }

        protected void RenumberSiteId(TSiteInfo siteInfo, int oldId)
        {
            _siteInfos.Remove(oldId);
            _siteInfos.Add(siteInfo.Id, siteInfo);
        }
    }
}
