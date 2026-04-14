using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Client
{
    public class SpecialReadingOfChildObjects
    {
        private readonly SyncDictionary<IdAndObjectType, ISpecialReadingOfChildObjectsForObject> _specialReadingForObjects =
            new SyncDictionary<IdAndObjectType, ISpecialReadingOfChildObjectsForObject>();

        private readonly SyncDictionary<IdAndObjectType, ICollection<StructuredSiteNode>> _objectNodes =
            new SyncDictionary<IdAndObjectType, ICollection<StructuredSiteNode>>();

        public event
            Action<
                IEnumerable<StructuredSiteNode>,
                ICollection<ObjectInSiteInfo>,
                ICollection<ObjectInSiteInfo>>
            RepaintChildObjects;

        public void Add(ISpecialReadingOfChildObjectsForObject specialReadingForObject)
        {
            if (specialReadingForObject == null)
                return;

            var idAndObjectType = specialReadingForObject.ParentObject;
            if (idAndObjectType == null)
                return;

            _specialReadingForObjects.Add(idAndObjectType, specialReadingForObject);
            specialReadingForObject.ChildObjectsChanged += ChildObjectsChanged;
        }

        public bool IsAddedSpecialReadingOfChildObjects(IdAndObjectType idAndObjectType)
        {
            return _specialReadingForObjects.ContainsKey(idAndObjectType);
        }

        public void AddStructuredSiteNodeForObject(
            IdAndObjectType idAndObjectType,
            StructuredSiteNode objectNode)
        {
            ICollection<StructuredSiteNode> nodes;
            if (!_objectNodes.TryGetValue(idAndObjectType, out nodes))
            {
                nodes = new LinkedList<StructuredSiteNode>();
                _objectNodes.Add(idAndObjectType, nodes);
            }

            nodes.Add(objectNode);
        }

        public IEnumerable<ObjectInSiteInfo> GetChildObjects(IdAndObjectType idAndObjectType)
        {
            if (idAndObjectType == null)
                return Enumerable.Empty<ObjectInSiteInfo>();

            ISpecialReadingOfChildObjectsForObject specialReadingForObject;
            if (!_specialReadingForObjects.TryGetValue(idAndObjectType, out specialReadingForObject))
            {
                return Enumerable.Empty<ObjectInSiteInfo>();
            }

            return specialReadingForObject.ChildObjects;
        }

        private void ChildObjectsChanged(
            IdAndObjectType idAndObjectType,
            ICollection<ObjectInSiteInfo> addedChildObjects,
            ICollection<ObjectInSiteInfo> removedChildObjects)
        {
            if (RepaintChildObjects == null)
                return;

            ICollection<StructuredSiteNode> nodes;
            if (!_objectNodes.TryGetValue(idAndObjectType, out nodes))
                return;

            RepaintChildObjects(
                nodes,
                addedChildObjects,
                removedChildObjects);
        }

        public void Clear()
        {
            foreach (var specialReadingForObject in _specialReadingForObjects.GetValuesSnapshot(false))
            {
                specialReadingForObject.Close();
            }

            RepaintChildObjects = null;

            _specialReadingForObjects.Clear();
            _objectNodes.Clear();
        }
    }
}
