using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public class FolderStructureObjectsInfo
    {
        private HashSet<IdAndObjectType> _objects = new HashSet<IdAndObjectType>();

        public IdAndObjectType FolderStructureId { get; private set; }
        public StructuredSiteNode FolderStructureNode { get; private set; }

        public FolderStructureObjectsInfo(IdAndObjectType folderStructureId, StructuredSiteNode folderStructureNode)
        {
            FolderStructureId = folderStructureId;
            FolderStructureNode = folderStructureNode;
        }

        public void AddObject(IdAndObjectType objectId)
        {
            _objects.Add(objectId);
        }

        public bool Contains(IdAndObjectType objectId)
        {
            return _objects.Contains(objectId);
        }

        public void RemoveObject(IdAndObjectType objectId,
            Action<IdAndObjectType, StructuredSiteNode> beforeRemove)
        {
            if (_objects.Contains(objectId))
            {
                if (beforeRemove != null)
                    beforeRemove(FolderStructureId, FolderStructureNode);

                _objects.Remove(objectId);
            }
        }

        public void RemoveObject(IdAndObjectType objectId,
            Action<StructuredSiteNode> beforeRemove)
        {
            if (_objects.Contains(objectId))
            {
                if (beforeRemove != null)
                    beforeRemove(FolderStructureNode);

                _objects.Remove(objectId);
            }
        }
    }
}
