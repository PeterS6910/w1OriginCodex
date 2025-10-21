using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public abstract class ASiteInfo<TSiteInfo, TIdAndObjectTypeSet> :
        ISiteInfo<TSiteInfo>,
        IEquatable<TSiteInfo>
        where TSiteInfo : ASiteInfo<TSiteInfo, TIdAndObjectTypeSet>
        where TIdAndObjectTypeSet : ISubset<IdAndObjectType>
    {
        protected readonly TIdAndObjectTypeSet _subTreeReferences;
        protected readonly TIdAndObjectTypeSet _objects;

        protected ASiteInfo(
            TIdAndObjectTypeSet subTreeReferences,
            TIdAndObjectTypeSet objects)
            : this(null, subTreeReferences, objects)
        {
        }

        protected ASiteInfo(
            TSiteInfo parentSite,
            TIdAndObjectTypeSet subTreeReferences,
            TIdAndObjectTypeSet objects)
        {
            ParentSite = parentSite;

            _subTreeReferences = subTreeReferences;
            _objects = objects;
        }

        public abstract int Id { get; }

        public TSiteInfo ParentSite
        {
            get { return _parentSite; }

            set
            {
                if (_parentSite != null)
                    _parentSite.RemoveChild(This);

                _parentSite = value;

                if (_parentSite == null)
                    return;

                _parentSite.AddChild(This);
            }
        }

        public abstract string Name { get; set; }

        protected abstract TSiteInfo This { get; }

        public void AddChild(TSiteInfo childSiteInfo)
        {
            _childrenSites.Add(childSiteInfo.Id, childSiteInfo);
        }

        public void RemoveChild(TSiteInfo childSiteInfo)
        {
            _childrenSites.Remove(childSiteInfo.Id);
        }

        public IEnumerable<IdAndObjectType> SubTreeReferences
        {
            get
            {
                return _subTreeReferences.Elements;
            }
        }

        public void AddObject(IdAndObjectType idAndObjectType)
        {
            _objects.Add(idAndObjectType);
        }

        public void RemoveObject(IdAndObjectType idAndObjectType)
        {
            _objects.Remove(idAndObjectType);
        }

        public IEnumerable<IdAndObjectType> Objects
        {
            get { return _objects.Elements; }
        }

        private readonly IDictionary<int, TSiteInfo> _childrenSites =
            new Dictionary<int, TSiteInfo>();

        private TSiteInfo _parentSite;

        public IEnumerable<TSiteInfo> ChildrenSites
        {
            get { return _childrenSites.Values; }
        }

        public bool HasChild(int siteId)
        {
            return _childrenSites.ContainsKey(siteId);
        }

        public void AddSubTreeReference(IdAndObjectType idAndObjectType)
        {
            _subTreeReferences.Add(idAndObjectType);
        }

        public void RemoveSubTreeReference(IdAndObjectType idAndObjectType)
        {
            _subTreeReferences.Remove(idAndObjectType);
        }

        public void RenumberChildId(TSiteInfo child, int oldId)
        {
            _childrenSites.Remove(oldId);
            _childrenSites.Add(child.Id, child);
        }

        public bool ContainsSubTreeReference(IdAndObjectType idAndObjectType)
        {
            return _subTreeReferences.Contains(idAndObjectType);
        }

        public bool Equals(TSiteInfo other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}