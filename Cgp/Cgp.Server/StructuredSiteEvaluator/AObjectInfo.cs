using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public abstract class AObjectInfo : IdAndObjectType
    {
        public abstract string Name { get; }
        public abstract IdAndObjectType Parent { get; }

        protected AObjectInfo(object id, ObjectType objectType)
            : base(id, objectType)
        {
        }
    }

    public abstract class AObjectInfo<
            TObjectInfo,
            TObjectInfoReferences,
            TObjectInfoReferencesReadOnlySet,
            TObjectInfoReferencesSet, 
            TSiteInfo, 
            TSiteIdSet> :
        AObjectInfo,
        IEquatable<TObjectInfo>
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
        where TSiteIdSet : ISubset<int>
    {
        protected readonly TObjectInfoReferencesReadOnlySet _directReferences;
        protected readonly TObjectInfoReferencesReadOnlySet _backReferences;

        protected readonly TObjectInfoReferencesSet _brokenBackReferences;
        protected readonly TObjectInfoReferencesSet _brokenDirectReferences;

        protected readonly TSiteIdSet _subTreeReferenceSites;

        private TSiteInfo _siteInfo;

        public TSiteInfo SiteInfo
        {
            get { return _siteInfo; }

            set
            {
                if (_siteInfo != null)
                    _siteInfo.RemoveObject(this);

                _siteInfo = value;

                if (_siteInfo != null)
                    _siteInfo.AddObject(this);
            }
        }

        protected AObjectInfo(
            object objectId, 
            ObjectType objectType,
            TSiteInfo siteInfo,
            TObjectInfoReferencesReadOnlySet directReferences,
            TObjectInfoReferencesReadOnlySet backReferences,
            TObjectInfoReferencesSet brokenDirectReferences,
            TObjectInfoReferencesSet brokenBackReferences,
            TSiteIdSet subTreeReferenceSites)
            : base(objectId, objectType)
        {
            SiteInfo = siteInfo;

            _directReferences = directReferences;
            _backReferences = backReferences;

            _brokenBackReferences = brokenBackReferences;
            _brokenDirectReferences = brokenDirectReferences;

            _subTreeReferenceSites = subTreeReferenceSites;
        }

        public abstract IEnumerable<TObjectInfoReferences> Children { get; }

        public IEnumerable<int> SubTreeReferenceSites
        {
            get { return _subTreeReferenceSites.Elements; }
        }

        public int SubTreeReferenceCount
        {
            get { return _subTreeReferenceSites.Count; }
        }

        public IEnumerable<TObjectInfoReferences> DirectReferences
        {
            get { return _directReferences.Elements; }
        }

        public IEnumerable<TObjectInfoReferences> BackReferences
        {
            get { return _backReferences.Elements; }
        }

        public IEnumerable<TObjectInfoReferences> BrokenBackReferenceses
        {
            get { return _brokenBackReferences.Elements; }
        }

        public IEnumerable<TObjectInfoReferences> BrokenDirectReferences
        {
            get { return _brokenDirectReferences.Elements; }
        }

        public bool HasBrokenDirectReferences
        {
            get { return _brokenDirectReferences.Count > 0; }
        }
        public void AddBrokenBackReference(TObjectInfoReferences sourceObjectInfo)
        {
            _brokenBackReferences.Add(sourceObjectInfo);
        }

        public bool AddBrokenDirectReference(TObjectInfoReferences targetObjectInfo)
        {
            return _brokenDirectReferences.Add(targetObjectInfo);
        }

        public void RemoveBrokenBackReference(TObjectInfoReferences sourceObjectInfo)
        {
            _brokenBackReferences.Remove(sourceObjectInfo);
        }

        public bool RemoveBrokenDirectReference(TObjectInfoReferences targetObjectInfo)
        {
            return _brokenDirectReferences.Remove(targetObjectInfo);
        }

        public bool AddSubTreeReference(TSiteInfo siteInfo)
        {
            if (!_subTreeReferenceSites.Add(siteInfo.Id))
                return false;

            siteInfo.AddSubTreeReference(this);
            return true;
        }

        public bool RemoveSubTreeReference(TSiteInfo siteInfo)
        {
            if (!_subTreeReferenceSites.Remove(siteInfo.Id))
                return false;

            siteInfo.RemoveSubTreeReference(this);

            return true;
        }

        bool IEquatable<TObjectInfo>.Equals(TObjectInfo other)
        {
            return Equals(other);
        }
    }
}