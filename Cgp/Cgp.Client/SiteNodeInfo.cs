using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public class SiteNodeInfo
    {
        private class SiteObjectNodes
        {
            private readonly Dictionary<IdAndObjectType, ICollection<StructuredSiteNode>> _siteObjectNodes =
                new Dictionary<IdAndObjectType, ICollection<StructuredSiteNode>>();

            private readonly Dictionary<IdAndObjectType, ICollection<StructuredSiteNode>>
                _subTreeReferencingObjectNodes
                    =
                    new Dictionary<IdAndObjectType, ICollection<StructuredSiteNode>>();

            public void AddObjectNode(IdAndObjectType objectId, StructuredSiteNode structuredSiteNode,
                bool isReference)
            {
                AddObjectNode(
                    objectId,
                    structuredSiteNode,
                    isReference ? _subTreeReferencingObjectNodes : _siteObjectNodes);
            }

            private void AddObjectNode(IdAndObjectType objectId, StructuredSiteNode structuredSiteNode,
                Dictionary<IdAndObjectType, ICollection<StructuredSiteNode>> objectNodes)
            {
                ICollection<StructuredSiteNode> nodes;
                if (!objectNodes.TryGetValue(objectId, out nodes))
                {
                    nodes = new LinkedList<StructuredSiteNode>();
                    objectNodes.Add(objectId, nodes);
                }

                nodes.Add(structuredSiteNode);
            }

            public ICollection<StructuredSiteNode> GetObjectNodes(IdAndObjectType objectId, bool isReference)
            {
                return GetObjectNodes(
                    objectId,
                    isReference ? _subTreeReferencingObjectNodes : _siteObjectNodes);
            }

            private ICollection<StructuredSiteNode> GetObjectNodes(IdAndObjectType objectId,
                Dictionary<IdAndObjectType, ICollection<StructuredSiteNode>> objectNodes)
            {
                ICollection<StructuredSiteNode> nodes;
                if (objectNodes.TryGetValue(objectId, out nodes))
                {
                    return nodes;
                }

                return null;
            }

            public void RemoveObjectNode(IdAndObjectType objectId, StructuredSiteNode structuredSiteNode,
                bool isReference)
            {
                RemoveObjectNode(
                    objectId,
                    structuredSiteNode,
                    isReference ? _subTreeReferencingObjectNodes : _siteObjectNodes);
            }

            private void RemoveObjectNode(IdAndObjectType objectId, StructuredSiteNode structuredSiteNode,
                Dictionary<IdAndObjectType, ICollection<StructuredSiteNode>> objectNodes)
            {
                ICollection<StructuredSiteNode> nodes;
                if (objectNodes.TryGetValue(objectId, out nodes))
                {
                    if (!nodes.Remove(structuredSiteNode))
                        nodes.Remove(structuredSiteNode.OtherTreeViewNode);

                    if (nodes.Count == 0)
                        objectNodes.Remove(objectId);
                }
            }
        }

        private SiteObjectNodes _objectNodes = new SiteObjectNodes();

        private readonly Dictionary<IdAndObjectType, FolderStructureObjectsInfo> _folderStructuresObjectsInfo =
            new Dictionary<IdAndObjectType, FolderStructureObjectsInfo>();

        public int SiteId { get; private set; }
        public bool ObjectsWasLoaded { get; set; }
        public StructuredSiteNode SiteNode { get; private set; }

        public SiteNodeInfo(int siteId, StructuredSiteNode siteNode)
        {
            SiteId = siteId;
            SiteNode = siteNode;
        }

        public void AddObjectNode(IdAndObjectType objectId, StructuredSiteNode structuredSiteNode, bool isReference)
        {
            _objectNodes.AddObjectNode(objectId, structuredSiteNode, isReference);
        }

        public ICollection<StructuredSiteNode> GetObjectNodes(IdAndObjectType objectId, bool isReference)
        {
            return _objectNodes.GetObjectNodes(objectId, isReference);
        }

        public void RemoveObjectNode(IdAndObjectType objectId, StructuredSiteNode structuredSiteNode, bool isReference)
        {
            _objectNodes.RemoveObjectNode(objectId, structuredSiteNode, isReference);
        }

        public void AddFolderStructureObjectsInfo(FolderStructureObjectsInfo folderStructureObjectsInfo)
        {
            _folderStructuresObjectsInfo.Add(folderStructureObjectsInfo.FolderStructureId, folderStructureObjectsInfo);
        }

        public void RemoveFolderStructureObjectsInfo(IdAndObjectType folderStructureId)
        {
            _folderStructuresObjectsInfo.Remove(folderStructureId);
        }

        public bool FolderStructureContainsObject(IdAndObjectType objectId)
        {
            foreach (var folderStructureObjects in _folderStructuresObjectsInfo.Values)
            {
                if (folderStructureObjects.Contains(objectId))
                    return true;
            }

            return false;
        }

        public void RemoveFolderStructureObject(IdAndObjectType objectId,
            Action<IdAndObjectType, StructuredSiteNode> beforeRemove)
        {
            foreach (var folderStructureObjects in _folderStructuresObjectsInfo.Values)
            {
                folderStructureObjects.RemoveObject(objectId, beforeRemove);
            }
        }
    }
}
