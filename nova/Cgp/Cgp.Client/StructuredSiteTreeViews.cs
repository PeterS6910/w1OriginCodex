using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.RemotingCommon;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

using Contal.IwQuick;
using Contal.Cgp.Globals;
using Contal.IwQuick.PlatformPC.UI;

namespace Contal.Cgp.Client
{
    public class StructuredSiteTreeViews
    {
        private class AddedObjectNodes
        {
            public ICollection<TreeNode> ObjectNodes { get; private set; }
            public ICollection<TreeNode> OtherTreeViewObjectNodes { get; private set; }

            public AddedObjectNodes()
            {
                ObjectNodes = new LinkedList<TreeNode>();
                OtherTreeViewObjectNodes = new LinkedList<TreeNode>();
            }
        }

        public const string SubSiteNodeImageKey = "SubSiteNodeImageKey";
        public const string SubSitesFolderNodeImageKey = "SubSitesFolderNodeImageKey";
        public const string SubTreeReferencingNodeImageKey = "SubTreeReferencingNodeImageKey";
        public const string HardwareFolderNodeImageKey = "HardwareFolderNodeImageKey";
        public const string FolderStructureNodeImageKey = "FolderStructureNodeImageKey";
        public const string ObjectReferenceFromParentImageKey = "ObjectReferenceFromParentImageKey";

        public TreeViewWithFilter ActualTreeView { get; private set; }
        private TreeViewWithFilter OtherTreeView { get; set; }
        private readonly Dictionary<int, SiteNodeInfo> _siteNodesInfo = new Dictionary<int, SiteNodeInfo>();
        private SpecialReadingOfChildObjects _specialReadingOfChildObjects;

        private readonly Dictionary<IdAndObjectType, FolderStructureObjectsInfo> _folderStructuresObjectsInfo =
            new Dictionary<IdAndObjectType, FolderStructureObjectsInfo>();

        public event Func<bool> HasAccessViewFolderStructure;
        
        public event Func<object,ICollection<ObjectInSiteInfo>> LoadFolderStructureObjects;

        public event Func<int, IEnumerable<SiteIdAndName>> LoadSubSites;

        public event Func<int, IEnumerable<ObjectInSiteInfo>> LoadObjectsInSite;

        public event Func<int, IEnumerable<ReferenceInSiteInfo>> LoadSubTreeReferences;

        public event Action<IdAndObjectType, IdAndObjectType> AddObjectToFolderStructure;

        public event Action<IdAndObjectType, IdAndObjectType> RemoveObjectFromFolderStructure;

        private readonly ProcessingQueue<bool> _endUpdateProcessingQueue = new ProcessingQueue<bool>();

        public StructuredSiteTreeViews(TreeViewWithFilter actualTreeView, TreeViewWithFilter otherTreeView)
        {
            ActualTreeView = actualTreeView;
            OtherTreeView = otherTreeView;
            
            ActualTreeView.BeforeExpand += TreeViewsBeforeExpand;
            OtherTreeView.BeforeExpand += TreeViewsBeforeExpand;

            ActualTreeView.DeleteParentNode += TreeViewsDeleteParentNode;
            OtherTreeView.DeleteParentNode += TreeViewsDeleteParentNode;

            _endUpdateProcessingQueue.ItemProcessing += EndUpdateCore;
        }

        public void Init(
            ICollection<SiteIdAndName> topMostSites,
            HashSet<int> topMostSiteIds,
            SpecialReadingOfChildObjects specialReadingOfChildObjects)
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.Invoke(
                    new Action<ICollection<SiteIdAndName>, HashSet<int>, SpecialReadingOfChildObjects>(Init),
                    topMostSites,
                    topMostSiteIds,
                    specialReadingOfChildObjects);
            }
            else
            {
                BeginUpdate();

                try
                {
                    if (specialReadingOfChildObjects != null)
                    {
                        _specialReadingOfChildObjects = specialReadingOfChildObjects;
                        _specialReadingOfChildObjects.RepaintChildObjects += RepaintChildObjects;
                    }

                    if (topMostSites.First().Id == -1)
                    {
                        CreateSubTreeReferencingFolder(null);
                        CreateSubSiteFolder(null);
                        CreateFolderStructuresFolder(null);

                        var siteNodeInfo = new SiteNodeInfo(-1, null);
                        _siteNodesInfo.Add(-1, siteNodeInfo);

                        LoadAllSubSites(-1, null);

                        LoadSiteNode(null, siteNodeInfo);
                    }
                    else
                    {
                        foreach (var topMostSite in topMostSites)
                        {
                            LoadAllSubSites(
                                topMostSite.Id,
                                AddSubSite(
                                    null,
                                    new SiteIdAndName(topMostSite.Id, topMostSite.Name)));

                            topMostSiteIds.Add(topMostSite.Id);
                        }
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                EndUpdate();
            }
        }

        private void RepaintChildObjects(
            IEnumerable<StructuredSiteNode> objectNodes,
            ICollection<ObjectInSiteInfo> addedChildObjects,
            ICollection<ObjectInSiteInfo> removedChildObjects)
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.Invoke(
                    new
                        Action<
                            IEnumerable<StructuredSiteNode>,
                            ICollection<ObjectInSiteInfo>,
                            ICollection<ObjectInSiteInfo>>
                        (RepaintChildObjects),
                    objectNodes,
                    addedChildObjects,
                    removedChildObjects);

                return;
            }

            BeginUpdate();

            if (objectNodes == null)
                return;

            var addedObjectNodes = new Dictionary<StructuredSiteNode, AddedObjectNodes>();

            foreach (var objectNode in objectNodes)
            {
                foreach (var removedChildObject in removedChildObjects)
                {
                    RemoveObjectNode(
                        objectNode,
                        removedChildObject);
                }

                var siteNode = GetSubSiteNode(objectNode);

                var siteNodeInfo = siteNode != null
                    ? (SiteNodeInfo)siteNode.Tag
                    : _siteNodesInfo[-1];

                foreach (var addedChildObject in addedChildObjects)
                {
                    AddObjectNode(
                        siteNodeInfo,
                        objectNode,
                        addedChildObject,
                        addedObjectNodes);
                }
            }

            foreach (var kvpAddedObjectNodes in addedObjectNodes)
            {
                AddTreeNodes(
                    kvpAddedObjectNodes.Key,
                    kvpAddedObjectNodes.Value.ObjectNodes,
                    kvpAddedObjectNodes.Value.OtherTreeViewObjectNodes);
            }

            EndUpdate();
        }

        private void LoadAllSubSites(int siteId, StructuredSiteNode siteNode)
        {
            var subSites = LoadSubSites(siteId);

            foreach (var subSite in subSites)
            {
                LoadAllSubSites(subSite.Id, AddSubSite(siteNode, subSite));
            }
        }

        public void SubSiteCreated(SiteIdAndName siteIdAndName, int parentSiteId)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(parentSiteId, out siteNodeInfo))
            {
                return;
            }

            var newSubSiteNode = AddSubSite(siteNodeInfo.SiteNode, siteIdAndName);

            ActualTreeView.SelectedNode = newSubSiteNode;
            EndUpdate();
            ActualTreeView.BeginEditNode(newSubSiteNode);
        }

        public void SiteRenamed(SiteIdAndName siteIdAndName)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(siteIdAndName.Id, out siteNodeInfo))
            {
                return;
            }

            siteNodeInfo.SiteNode.OtherTreeViewNode.Text = siteIdAndName.Name;
            OtherTreeView.EndEditNode(siteNodeInfo.SiteNode.OtherTreeViewNode);
        }

        public void SiteDeleted(int siteId)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo))
            {
                return;
            }

            RemoveSubSite(siteNodeInfo.SiteNode);
        }

        private void RemoveSubSite(StructuredSiteNode subSiteNode)
        {
            RemoveNodes(subSiteNode.Parent as StructuredSiteNode, node => node == subSiteNode);
            RemoveNodes(subSiteNode.OtherTreeViewNode.Parent as StructuredSiteNode, node => node == subSiteNode.OtherTreeViewNode);
        }

        private void TreeViewsBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var structuredSiteNode = e.Node as StructuredSiteNode;

            if (structuredSiteNode == null)
                return;

            if (structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.SubSiteNode)
                return;

            var siteNodeInfo = structuredSiteNode.Tag as SiteNodeInfo;
            if (siteNodeInfo == null || siteNodeInfo.ObjectsWasLoaded)
                return;

            SafeThread<StructuredSiteNode, SiteNodeInfo>.StartThread(
                LoadSiteNode,
                structuredSiteNode,
                siteNodeInfo);

            e.Cancel = true;
        }

        private bool TreeViewsDeleteParentNode(TreeNode node)
        {
            var structuredSiteNode = node as StructuredSiteNode;

            return structuredSiteNode == null ||
                   (structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.SubSitesFolderNode &&
                    structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.SubSiteNode &&
                    structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.SubTreeReferencingFolderNode &&
                    structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.OrmObjectNode &&
                    structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.FolderStructuresFolderNode &&
                    structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.FolderStructureNode);
        }

        private void LoadSiteNode(StructuredSiteNode structuredSiteNode,
            SiteNodeInfo siteNodeInfo)
        {
            DisableTreeViews();

            var objectsFromServer = LoadObjectsInSite(siteNodeInfo.SiteId);
            var referencesFromServer = LoadSubTreeReferences(siteNodeInfo.SiteId);

            siteNodeInfo.ObjectsWasLoaded = true;

            AfterLoadSiteNode(
                objectsFromServer,
                referencesFromServer,
                structuredSiteNode);

            EnableTreeViews();
        }

        private static IEnumerable<T> SortObjects<T>(IEnumerable<T> objectsInfo)
            where T : ObjectInSiteInfo
        {
            if (objectsInfo == null)
                return null;

            var objectsWithChildObjects = new Dictionary<IdAndObjectType, ICollection<T>>();
            var objectInfosByObjectInfoId = new Dictionary<IdAndObjectType, T>();
            foreach (var objectInfo in objectsInfo)
            {
                objectInfosByObjectInfoId.Add(objectInfo, objectInfo);

                if (objectInfo.Parent == null)
                    continue;

                ICollection<T> childObjectsInfo;
                if (!objectsWithChildObjects.TryGetValue(objectInfo.Parent, out childObjectsInfo))
                {
                    childObjectsInfo = new LinkedList<T>();
                    objectsWithChildObjects.Add(objectInfo.Parent, childObjectsInfo);
                }

                childObjectsInfo.Add(objectInfo);
            }

            var result = new LinkedList<T>();

            foreach (var objectInfo in objectInfosByObjectInfoId.Values)
            {
                if (objectInfo.Parent != null &&
                    objectInfosByObjectInfoId.ContainsKey(objectInfo.Parent))
                {
                    continue;
                }

                AddSortedChildObjects(objectInfo, result, idAndObjectType =>
                {
                    ICollection<T> childObjectsInfo;
                    return !objectsWithChildObjects.TryGetValue(idAndObjectType, out childObjectsInfo)
                        ? null
                        : childObjectsInfo;
                });
            }

            return result;
        }

        private static void AddSortedChildObjects<T>(
            T objectInfo, ICollection<T> result,
            Func<IdAndObjectType,ICollection<T>> getChildObjects)
            where T : ObjectInSiteInfo
        {
            result.Add(objectInfo);

            var childObjectsInfo = getChildObjects(objectInfo);
            if (childObjectsInfo == null)
                return;

            foreach (var childObjectInfo in childObjectsInfo)
            {
                AddSortedChildObjects(childObjectInfo, result, getChildObjects);
            }
        }

        private void DisableTreeViews()
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.Invoke(new DVoid2Void(DisableTreeViews));
            }
            else
            {
                ActualTreeView.Enabled = false;
                OtherTreeView.Enabled = false;
                ActualTreeView.Parent.Cursor = Cursors.WaitCursor;
                OtherTreeView.Parent.Cursor = Cursors.WaitCursor;

                BeginUpdate();
            }
        }

        private void EnableTreeViews()
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.Invoke(new DVoid2Void(EnableTreeViews));
            }
            else
            {
                ActualTreeView.Enabled = true;
                OtherTreeView.Enabled = true;
                ActualTreeView.Parent.Cursor = Cursors.Default;
                OtherTreeView.Parent.Cursor = Cursors.Default;

                EndUpdate();
            }
        }

        private void AfterLoadSiteNode(IEnumerable<ObjectInSiteInfo> objectsInfo,
            IEnumerable<ReferenceInSiteInfo> referencesInfo, StructuredSiteNode siteNode)
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.Invoke(
                    new
                        Action
                            <ICollection<ObjectInSiteInfo>,
                                IEnumerable<ReferenceInSiteInfo>,
                                StructuredSiteNode>(
                        AfterLoadSiteNode),
                    objectsInfo,
                    referencesInfo,
                    siteNode);
            }
            else
            {
                BeginUpdate();

                AddObjects(siteNode, objectsInfo);

                AddReferences(
                    siteNode,
                    referencesInfo,
                    false);

                if (siteNode != null)
                {
                    siteNode.Expand();
                }

                EndUpdate();
            }
        }

        private void AddObjects(StructuredSiteNode siteNode, IEnumerable<ObjectInSiteInfo> objectsInfo)
        {
            if (objectsInfo == null)
            {
                return;
            }

            objectsInfo = SortObjects(objectsInfo);

            var objectNodes = new Dictionary<StructuredSiteNode, AddedObjectNodes>();
            foreach (var objectInfo in objectsInfo)
            {
                SiteNodeInfo siteNodeInfo;
                if (siteNode != null)
                    siteNodeInfo = (SiteNodeInfo)siteNode.Tag;
                else
                    siteNodeInfo = _siteNodesInfo[-1];

                AddObject(
                    siteNodeInfo,
                    siteNode,
                    objectInfo,
                    objectNodes,
                    false);
            }

            foreach (var kvpObjectNodes in objectNodes)
            {
                AddTreeNodes(
                    kvpObjectNodes.Key,
                    kvpObjectNodes.Value.ObjectNodes,
                    kvpObjectNodes.Value.OtherTreeViewObjectNodes);
            }
        }

        private void AddObject(SiteNodeInfo siteNodeInfo, StructuredSiteNode parentNode, ObjectInSiteInfo objectInfo,
            Dictionary<StructuredSiteNode, AddedObjectNodes> objectNodes, bool isReferecne)
        {
            if (objectInfo.ObjectType != ObjectType.UserFoldersStructure ||
                objectInfo.Parent == null)
            {
                siteNodeInfo.AddObjectNode(
                    objectInfo,
                    AddObjectNode(
                        siteNodeInfo,
                        parentNode,
                        objectInfo,
                        objectNodes),
                    isReferecne);
            }

            if (objectInfo.Parent != null)
            {
                var objectParentNodes = siteNodeInfo.GetObjectNodes(objectInfo.Parent, isReferecne);
                if (objectParentNodes != null)
                {
                    foreach (var objectParentNode in objectParentNodes)
                    {
                        siteNodeInfo.AddObjectNode(
                            objectInfo,
                            AddObjectNode(
                                siteNodeInfo,
                                objectParentNode,
                                objectInfo,
                                objectNodes),
                            isReferecne);
                    }
                }
            }
        }

        private StructuredSiteNode AddObjectNode(SiteNodeInfo siteNodeInfo, StructuredSiteNode parentNode,
            ObjectInSiteInfo objectInfo, Dictionary<StructuredSiteNode, AddedObjectNodes> objectNodes)
        {
            var objectNode = objectInfo.ObjectType == ObjectType.UserFoldersStructure
                ? CreateFolderStructureNode(objectInfo)
                : CreateOrmObjectNode(objectInfo);

            StructuredSiteNode objectTypeFolderNode;

            var isSiteNodeOrSubTreeReferencingNode =
                parentNode == null ||
                parentNode.StructuredSiteNodeType == StructuredSiteNodeType.SubSiteNode ||
                parentNode.StructuredSiteNodeType == StructuredSiteNodeType.SubTreeReferencingFolderNode;

            if (isSiteNodeOrSubTreeReferencingNode &&
                IsObjectTypeInTheHardwareFolder(objectInfo.ObjectType))
            {
                objectTypeFolderNode = GetOrCreateObjectTypeFolder(GetOrCreateHardwareFolder(parentNode),
                    objectInfo.ObjectType);
            }
            else if (objectInfo.ObjectType == ObjectType.UserFoldersStructure)
            {
                objectTypeFolderNode = isSiteNodeOrSubTreeReferencingNode
                    ? GetFolderStructuresFolder(parentNode)
                    : parentNode;

                CreateFolderStructureObjects(
                    CreateFolderStructureObjectInfo(
                        siteNodeInfo,
                        objectInfo,
                        objectNode));
            }
            else
            {
                objectTypeFolderNode = GetOrCreateObjectTypeFolder(parentNode, objectInfo.ObjectType);
            }

            AddedObjectNodes addedObjectNodes;
            if (!objectNodes.TryGetValue(objectTypeFolderNode, out addedObjectNodes))
            {
                addedObjectNodes = new AddedObjectNodes();
                objectNodes.Add(objectTypeFolderNode, addedObjectNodes);
            }

            addedObjectNodes.ObjectNodes.Add(objectNode);
            addedObjectNodes.OtherTreeViewObjectNodes.Add(objectNode.OtherTreeViewNode);

            if (_specialReadingOfChildObjects == null
                || !_specialReadingOfChildObjects.IsAddedSpecialReadingOfChildObjects(objectInfo))
            {
                return objectNode;
            }

            _specialReadingOfChildObjects.AddStructuredSiteNodeForObject(objectInfo, objectNode);
            var childObjectsInfo = _specialReadingOfChildObjects.GetChildObjects(objectInfo);
            if (childObjectsInfo == null)
                return objectNode;

            foreach (var childObjectInfo in childObjectsInfo)
            {
                AddObjectNode(
                    siteNodeInfo,
                    objectNode,
                    childObjectInfo,
                    objectNodes);
            }

            return objectNode;
        }

        private FolderStructureObjectsInfo CreateFolderStructureObjectInfo(SiteNodeInfo siteNodeInfo, IdAndObjectType folderStructureId,
            StructuredSiteNode folderStructureNode)
        {
            var folderStructureObjectsInfo = new FolderStructureObjectsInfo(folderStructureId, folderStructureNode);
           
            _folderStructuresObjectsInfo.Add(
                folderStructureId,
                folderStructureObjectsInfo);

            siteNodeInfo.AddFolderStructureObjectsInfo(folderStructureObjectsInfo);

            return folderStructureObjectsInfo;
        }

        private void CreateFolderStructureObjects(FolderStructureObjectsInfo folderStructureObjectsInfo)
        {
            var objectNodes = new LinkedList<TreeNode>();
            var otherTreeViewObjectNodes = new LinkedList<TreeNode>();

            var folderStructureObjects = LoadFolderStructureObjects(folderStructureObjectsInfo.FolderStructureId.Id);

            if (folderStructureObjects == null)
                return;

            foreach (var objectInfo in folderStructureObjects)
            {
                folderStructureObjectsInfo.AddObject(objectInfo);

                var objectNode = CreateOrmObjectNode(objectInfo);
                objectNodes.AddLast(objectNode);
                otherTreeViewObjectNodes.AddLast(objectNode.OtherTreeViewNode);
            }

            AddTreeNodes(folderStructureObjectsInfo.FolderStructureNode, objectNodes, otherTreeViewObjectNodes);
        }

        private void RemoveFolderStructureObject(IdAndObjectType folderStructureId,
            StructuredSiteNode folderStructureNode, IdAndObjectType objectId)
        {
            if (RemoveObjectFromFolderStructure != null)
                RemoveObjectFromFolderStructure(objectId, folderStructureId);

            RemoveNodes(folderStructureNode, node =>
            {
                if (node == null)
                    return false;

                var objectNode = node as StructuredSiteNode;
                if (objectNode == null)
                    return false;

                if (objectNode.StructuredSiteNodeType != StructuredSiteNodeType.OrmObjectNode)
                {
                    return false;
                }

                return ((IdAndObjectType) objectNode.Tag).Equals(objectId);
            });
        }

        public void AddFolderStructureObject(IdAndObjectType folderStructureId, ObjectInSiteInfo objectInfo)
        {
            FolderStructureObjectsInfo folderStructureObjectsInfo;
            if (!_folderStructuresObjectsInfo.TryGetValue(folderStructureId, out folderStructureObjectsInfo))
                return;

            if (AddObjectToFolderStructure != null)
                AddObjectToFolderStructure(objectInfo, folderStructureId);

            folderStructureObjectsInfo.AddObject(objectInfo);

            AddTreeNode(folderStructureObjectsInfo.FolderStructureNode, CreateOrmObjectNode(objectInfo));
        }

        public ICollection<IdAndObjectType> GetFolderStructuresForObject(IdAndObjectType objectId)
        {
            return new HashSet<IdAndObjectType>(
                _folderStructuresObjectsInfo.Values
                    .Where(
                        folderStructuresObjectsInfo =>
                            folderStructuresObjectsInfo.Contains(objectId))
                    .Select(
                        folderStructuresObjectsInfo =>
                            folderStructuresObjectsInfo.FolderStructureId));
        }

        public bool FolderStructureContainsObject(IdAndObjectType folderStructureId, IdAndObjectType objectId)
        {
            FolderStructureObjectsInfo folderStructureObjectsInfo;
            if (!_folderStructuresObjectsInfo.TryGetValue(folderStructureId, out folderStructureObjectsInfo))
                return false;

            return folderStructureObjectsInfo.Contains(objectId);
        }

        public bool FolderStructureContainsObject(int siteId, IdAndObjectType objectId)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo))
                return false;

            return siteNodeInfo.FolderStructureContainsObject(objectId);
        }

        public void RemoveFolderStructureObjectInfo(IdAndObjectType folderStructureId)
        {
            _folderStructuresObjectsInfo.Remove(folderStructureId);
        }

        public void RemoveFolderStructureObject(IdAndObjectType folderStructureId, IdAndObjectType objectId)
        {
            FolderStructureObjectsInfo folderStructureObjectsInfo;
            if (!_folderStructuresObjectsInfo.TryGetValue(folderStructureId, out folderStructureObjectsInfo))
                return;

            folderStructureObjectsInfo.RemoveObject(
                objectId,
                folderStructureNode =>
                    RemoveFolderStructureObject(folderStructureId, folderStructureNode, objectId));
        }

        public void RemoveFolderStructureObject(int siteId, IdAndObjectType objectId)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo))
                return;

            siteNodeInfo.RemoveFolderStructureObject(objectId,
                (folderStructureId, folderStructureNode) =>
                    RemoveFolderStructureObject(folderStructureId, folderStructureNode, objectId));
        }

        private void AddReferences(StructuredSiteNode siteNode, IEnumerable<ReferenceInSiteInfo> referencesInfo,
            bool wasLoaded)
        {
            if (referencesInfo == null)
            {
                return;
            }

            if (wasLoaded)
            {
                var collectionReferencesInfo = new LinkedList<ReferenceInSiteInfo>(referencesInfo);
                AddSubTreeReferencesToChildNodes(siteNode, collectionReferencesInfo);

                referencesInfo = SortObjects(collectionReferencesInfo);
            }
            else if (siteNode != null)
            {
                var hashSetReferencesInfo = new HashSet<ReferenceInSiteInfo>(referencesInfo);

                var parentReferencesInfo = new LinkedList<ReferenceInSiteInfo>();

                GetSubTreeReferencesFromParent(
                    GetSubTreeReferencingFolder(siteNode.Parent.Parent as StructuredSiteNode),
                    parentReferencesInfo,
                    hashSetReferencesInfo);

                GetSubTreeReferencesFromParent(
                    GetObjectTypeFolder(siteNode.Parent.Parent as StructuredSiteNode, ObjectType.Login),
                    parentReferencesInfo,
                    hashSetReferencesInfo);

                GetSubTreeReferencesFromParent(
                    GetObjectTypeFolder(siteNode.Parent.Parent as StructuredSiteNode, ObjectType.LoginGroup),
                    parentReferencesInfo,
                    hashSetReferencesInfo);

                referencesInfo = SortObjects(hashSetReferencesInfo.Concat(parentReferencesInfo));
            }
            else
            {
                referencesInfo = SortObjects(referencesInfo);
            }

            var subTreeReferencingNode = GetSubTreeReferencingFolder(siteNode);

            var referenceNodes = new Dictionary<StructuredSiteNode, AddedObjectNodes>();
            foreach (var referenceInfo in referencesInfo)
            {
                SiteNodeInfo siteNodeInfo;
                if (siteNode != null)
                    siteNodeInfo = (SiteNodeInfo) siteNode.Tag;
                else
                    siteNodeInfo = _siteNodesInfo[-1];

                AddObject(
                    siteNodeInfo,
                    referenceInfo.ObjectType == ObjectType.Login ||
                    referenceInfo.ObjectType == ObjectType.LoginGroup
                        ? siteNode
                        : subTreeReferencingNode,
                    referenceInfo,
                    referenceNodes,
                    true);
            }

            foreach (var kvpReferenceNodes in referenceNodes)
            {
                AddTreeNodes(
                    kvpReferenceNodes.Key,
                    kvpReferenceNodes.Value.ObjectNodes,
                    kvpReferenceNodes.Value.OtherTreeViewObjectNodes);
            }
        }

        private void GetSubTreeReferencesFromParent(StructuredSiteNode parentNode,
            ICollection<ReferenceInSiteInfo> parentReferencesInfo, HashSet<ReferenceInSiteInfo> hashSetReferencesInfo)
        {
            if (parentNode == null)
                return;

            if (parentNode.StructuredSiteNodeType == StructuredSiteNodeType.OrmObjectNode)
            {
                var referenceInfo = (ReferenceInSiteInfo)parentNode.Tag;
                if (hashSetReferencesInfo != null && hashSetReferencesInfo.Contains(referenceInfo))
                    return;
                
                parentReferencesInfo.Add(
                    new ReferenceInSiteInfo(
                        referenceInfo.Id,
                        referenceInfo.ObjectType,
                        referenceInfo.Name,
                        referenceInfo.Parent,
                        false));
            }
            else
            {
                foreach (var node in parentNode.Nodes)
                {
                    GetSubTreeReferencesFromParent(
                        node as StructuredSiteNode,
                        parentReferencesInfo,
                        hashSetReferencesInfo);
                }
            }
        }

        private void AddSubTreeReferencesToChildNodes(StructuredSiteNode siteNode,
            ICollection<ReferenceInSiteInfo> referencesInfo)
        {
            var subSitesFolderNode = GetSubSitesFolder(siteNode);
            if (subSitesFolderNode == null)
                return;

            foreach (var node in subSitesFolderNode.Nodes)
            {
                var childSiteNode = node as StructuredSiteNode;
                if (childSiteNode != null &&
                    ((SiteNodeInfo)childSiteNode.Tag).ObjectsWasLoaded)
                {
                    var newReferencesInfo = new LinkedList<ReferenceInSiteInfo>();
                    foreach (var referenceInfo in referencesInfo)
                    {
                        if (ObjectExistInTheSubSite(
                            referenceInfo,
                            referenceInfo.ObjectType == ObjectType.Login ||
                            referenceInfo.ObjectType == ObjectType.LoginGroup
                                ? childSiteNode
                                : GetSubTreeReferencingFolder(childSiteNode)))
                        {
                            continue;
                        }

                        newReferencesInfo.AddLast(
                            new ReferenceInSiteInfo(
                                referenceInfo.Id,
                                referenceInfo.ObjectType,
                                referenceInfo.Name,
                                referenceInfo.Parent,
                                false));
                    }

                    AddReferences(childSiteNode, newReferencesInfo, true);
                }
            }
        }

        public void ObjectAdded(ObjectInSiteInfo objectInfo, int siteId)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo) ||
                !siteNodeInfo.ObjectsWasLoaded)
            {
                return;
            }

            AddObjects(siteNodeInfo.SiteNode, Enumerable.Repeat(objectInfo, 1));
        }

        public void ObjectRenamed(IdAndObjectType objectId, string newName)
        {
            foreach (var siteObjectInfo in _siteNodesInfo.Values)
            {
                RenameObjectNodes(siteObjectInfo.GetObjectNodes(objectId, false), newName, false);
                RenameObjectNodes(siteObjectInfo.GetObjectNodes(objectId, true), newName, true);
            }
        }

        private void RenameObjectNodes(IEnumerable<StructuredSiteNode> objectNodes, string newName, bool isReference)
        {
            if (objectNodes == null)
                return;

            foreach (var objectNode in objectNodes)
            {
                if (isReference)
                {
                    var referenceInfo = (ReferenceInSiteInfo)objectNode.Tag;
                    var newReferenceInfo = new ReferenceInSiteInfo(
                        referenceInfo.Id,
                        referenceInfo.ObjectType,
                        newName,
                        referenceInfo.Parent,
                        referenceInfo.DefinedInThisSite);

                    objectNode.Tag = newReferenceInfo;
                    objectNode.OtherTreeViewNode.Tag = newReferenceInfo;
                }
                else
                {
                    var objectInfo = (ObjectInSiteInfo)objectNode.Tag;
                    var newObjectInfo = new ObjectInSiteInfo(
                        objectInfo.Id,
                        objectInfo.ObjectType,
                        newName,
                        objectInfo.Parent);

                    objectNode.Tag = newObjectInfo;
                    objectNode.OtherTreeViewNode.Tag = newObjectInfo;
                }


                objectNode.Text = newName;
                ActualTreeView.EndEditNode(objectNode);

                objectNode.OtherTreeViewNode.Text = newName;
                OtherTreeView.EndEditNode(objectNode.OtherTreeViewNode);
            }


        }

        public void ObjectRemoved(IdAndObjectType objectId, int siteId)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo) ||
                !siteNodeInfo.ObjectsWasLoaded)
            {
                return;
            }

            RemoveObject(siteNodeInfo, siteNodeInfo.SiteNode, objectId, false);
        }

        private StructuredSiteNode RemoveObject(SiteNodeInfo siteNodeInfo, StructuredSiteNode parentNode, IdAndObjectType objectId,
            bool isReference)
        {
            var removedNode = RemoveObjectNode(parentNode, objectId);

            if (removedNode != null)
            {
                siteNodeInfo.RemoveObjectNode(
                    objectId,
                    removedNode,
                    isReference);
            }

            var objectNodes = siteNodeInfo.GetObjectNodes(objectId, isReference);

            if (objectNodes != null)
            {
                objectNodes = new LinkedList<StructuredSiteNode>(siteNodeInfo.GetObjectNodes(objectId, isReference));
                foreach (var objectNode in objectNodes)
                {
                    StructuredSiteNode parentObjectNode;

                    if (objectId.ObjectType == ObjectType.UserFoldersStructure)
                    {
                        parentObjectNode = objectNode.Parent as StructuredSiteNode;
                    }
                    else
                    {
                        parentObjectNode = objectNode.Parent.Parent as StructuredSiteNode;
                    }

                    siteNodeInfo.RemoveObjectNode(
                        objectId,
                        RemoveObjectNode(
                            parentObjectNode,
                            objectId),
                        isReference);
                }
            }

            if (objectId.ObjectType == ObjectType.UserFoldersStructure)
            {
                RemoveFolderStructureObjectInfo(objectId);
            }

            return removedNode;
        }

        private StructuredSiteNode RemoveObjectNode(StructuredSiteNode parentNode, IdAndObjectType objectId)
        {
            StructuredSiteNode objectTypeFolderNode;

            var isSiteNodeOrSubTreeReferencingNode =
                parentNode == null ||
                parentNode.StructuredSiteNodeType == StructuredSiteNodeType.SubSiteNode ||
                parentNode.StructuredSiteNodeType == StructuredSiteNodeType.SubTreeReferencingFolderNode;

            if (isSiteNodeOrSubTreeReferencingNode &&
                IsObjectTypeInTheHardwareFolder(objectId.ObjectType))
            {
                objectTypeFolderNode = GetObjectTypeFolder(GetHardwareFolder(parentNode), objectId.ObjectType);
            }
            else if (objectId.ObjectType == ObjectType.UserFoldersStructure)
            {
                objectTypeFolderNode = isSiteNodeOrSubTreeReferencingNode
                    ? GetFolderStructuresFolder(parentNode)
                    : parentNode;
            }
            else
            {
                objectTypeFolderNode = GetObjectTypeFolder(parentNode, objectId.ObjectType);
            }

            StructuredSiteNode removedNode = null;
            if (objectTypeFolderNode != null)
            {
                RemoveNodes(objectTypeFolderNode, node =>
                {
                    if (node == null)
                        return false;

                    var objectNode = node as StructuredSiteNode;
                    if (objectNode == null)
                        return false;

                    if (objectNode.StructuredSiteNodeType != StructuredSiteNodeType.OrmObjectNode &&
                        objectNode.StructuredSiteNodeType != StructuredSiteNodeType.FolderStructureNode)
                    {
                        return false;
                    }

                    if (((IdAndObjectType)objectNode.Tag).Equals(objectId))
                    {
                        removedNode = objectNode;
                        return true;
                    }

                    return false;
                });
            }

            if (objectId.ObjectType == ObjectType.UserFoldersStructure)
            {
                RemoveFolderStructureObjectInfo(objectId);
            }

            return removedNode;
        }

        public void SubTreeReferenceAdded(ReferenceInSiteInfo referenceInfo, int siteId)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo) ||
                !siteNodeInfo.ObjectsWasLoaded)
            {
                return;
            }

            DoSubTreeReferenceAdded(referenceInfo, siteNodeInfo.SiteNode);
        }

        private void DoSubTreeReferenceAdded(ReferenceInSiteInfo referenceInfo, StructuredSiteNode siteNode)
        {
            if (referenceInfo.DefinedInThisSite)
                RemoveReference(siteNode, referenceInfo);

            AddReferences(
                siteNode,
                Enumerable.Repeat(referenceInfo, 1),
                true);
        }

        public void SubTreeReferenceRemoved(IdAndObjectType referenceId, int siteId, bool isReferenceDefinedInParent)
        {
            SiteNodeInfo siteNodeInfo;
            if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo) ||
                !siteNodeInfo.ObjectsWasLoaded)
            {
                return;
            }

            StructuredSiteNode removedNode = RemoveReference(siteNodeInfo.SiteNode, referenceId);

            if (isReferenceDefinedInParent)
            {
                var referenceInfo = (ReferenceInSiteInfo)removedNode.Tag;
                DoSubTreeReferenceAdded(
                    new ReferenceInSiteInfo(
                        referenceInfo.Id,
                        referenceInfo.ObjectType,
                        referenceInfo.Name,
                        referenceInfo.Parent,
                        false),
                    siteNodeInfo.SiteNode);
            }
        }

        private StructuredSiteNode RemoveReference(StructuredSiteNode siteNode, IdAndObjectType referenceId)
        {
            RemoveSubTreeReferenceFromChildNodes(siteNode, referenceId);

            SiteNodeInfo siteNodeInfo;
            if (siteNode != null)
                siteNodeInfo = (SiteNodeInfo) siteNode.Tag;
            else
                siteNodeInfo = _siteNodesInfo[-1];

            return RemoveObject(
                siteNodeInfo,
                referenceId.ObjectType == ObjectType.Login ||
                referenceId.ObjectType == ObjectType.LoginGroup
                    ? siteNode
                    : GetSubTreeReferencingFolder(siteNode),
                referenceId,
                true);
        }

        private void RemoveSubTreeReferenceFromChildNodes(StructuredSiteNode siteNode, IdAndObjectType referenceId)
        {
            var subSitesFolderNode = GetSubSitesFolder(siteNode);
            if (subSitesFolderNode == null)
                return;

            foreach (var node in subSitesFolderNode.Nodes)
            {
                var childSiteNode = node as StructuredSiteNode;
                if (childSiteNode != null &&
                    ((SiteNodeInfo)childSiteNode.Tag).ObjectsWasLoaded)
                {
                    if (ObjectExistInTheSubSite(
                        referenceId,
                        referenceId.ObjectType == ObjectType.Login ||
                        referenceId.ObjectType == ObjectType.LoginGroup
                            ? childSiteNode
                            : GetSubTreeReferencingFolder(childSiteNode)))
                    {
                        continue;
                    }

                    RemoveReference(childSiteNode, referenceId);
                }
            }
        }

        public void ObjectParentChanged(IdAndObjectType idAndObjectType, IdAndObjectType newParent,
            IdAndObjectType oldParent)
        {
            foreach (var siteNodeInfo in _siteNodesInfo.Values)
            {
                bool parentWasChanged = false;
                ObjectInSiteInfo objectInfo = null;
                ReferenceInSiteInfo referenceInfo = null;

                var objectSiteNodes = siteNodeInfo.GetObjectNodes(idAndObjectType, false);
                if (objectSiteNodes != null)
                {
                    foreach (var objectSiteNode in objectSiteNodes)
                    {
                        objectInfo = (ObjectInSiteInfo)objectSiteNode.Tag;
                        if ((objectInfo.Parent == null && newParent == null) ||
                            (objectInfo.Parent != null && objectInfo.Parent.Equals(newParent)))
                        {
                            continue;
                        }

                        objectInfo = new ObjectInSiteInfo(
                            objectInfo.Id,
                            objectInfo.ObjectType,
                            objectInfo.Name,
                            newParent);

                        objectSiteNode.Tag = objectInfo;
                        objectSiteNode.OtherTreeViewNode.Tag = objectInfo;

                        parentWasChanged = true;
                    }
                }

                var objectSubTreeReferencingNodes = siteNodeInfo.GetObjectNodes(idAndObjectType, true);
                if (objectSubTreeReferencingNodes != null)
                {
                    foreach (var objectSubTreeReferencingNode in objectSubTreeReferencingNodes)
                    {
                        referenceInfo = (ReferenceInSiteInfo)objectSubTreeReferencingNode.Tag;

                        if ((referenceInfo.Parent == null && newParent == null) ||
                            (referenceInfo.Parent != null && referenceInfo.Parent.Equals(newParent)))
                        {
                            continue;
                        }

                        referenceInfo = new ReferenceInSiteInfo(
                            referenceInfo.Id,
                            referenceInfo.ObjectType,
                            referenceInfo.Name,
                            newParent,
                            referenceInfo.DefinedInThisSite);

                        objectSubTreeReferencingNode.Tag = referenceInfo;
                        objectSubTreeReferencingNode.OtherTreeViewNode.Tag = referenceInfo;

                        parentWasChanged = true;
                    }
                }

                if (!parentWasChanged)
                    continue;

                if (oldParent != null)
                {
                    var parentObjectSiteNodes = siteNodeInfo.GetObjectNodes(oldParent, false);
                    if (parentObjectSiteNodes != null)
                    {
                        foreach (var parentObjectSiteNode in parentObjectSiteNodes)
                        {
                            siteNodeInfo.RemoveObjectNode(
                                idAndObjectType,
                                RemoveObjectNode(
                                    parentObjectSiteNode,
                                    idAndObjectType),
                                false);
                        }
                    }

                    var parentObjectSubTreeReferencingNodes = siteNodeInfo.GetObjectNodes(oldParent, true);
                    if (parentObjectSubTreeReferencingNodes != null)
                    {
                        foreach (var parentObjectSubTreeReferencingNode in parentObjectSubTreeReferencingNodes)
                        {
                            siteNodeInfo.RemoveObjectNode(
                                idAndObjectType,
                                RemoveObjectNode(
                                    parentObjectSubTreeReferencingNode,
                                    idAndObjectType),
                                true);
                        }
                    }
                }

                if (newParent != null)
                {
                    var objectNodes = new Dictionary<StructuredSiteNode, AddedObjectNodes>();
                    var parentObjectSiteNodes = siteNodeInfo.GetObjectNodes(newParent, false);
                    if (parentObjectSiteNodes != null)
                    {
                        foreach (var parentObjectSiteNode in parentObjectSiteNodes)
                        {
                            siteNodeInfo.AddObjectNode(
                                idAndObjectType,
                                AddObjectNode(
                                    siteNodeInfo,
                                    parentObjectSiteNode,
                                    objectInfo,
                                    objectNodes),
                                false);
                        }
                    }

                    var parentObjectSubTreeReferencingNodes = siteNodeInfo.GetObjectNodes(newParent, true);
                    if (parentObjectSubTreeReferencingNodes != null)
                    {
                        foreach (var parentObjectSubTreeReferencingNode in parentObjectSubTreeReferencingNodes)
                        {
                            siteNodeInfo.AddObjectNode(
                                idAndObjectType,
                                AddObjectNode(
                                    siteNodeInfo,
                                    parentObjectSubTreeReferencingNode,
                                    referenceInfo,
                                    objectNodes),
                                true);
                        }
                    }

                    foreach (var kvpObjectNodes in objectNodes)
                    {
                        AddTreeNodes(
                            kvpObjectNodes.Key,
                            kvpObjectNodes.Value.ObjectNodes,
                            kvpObjectNodes.Value.OtherTreeViewObjectNodes);
                    }
                }
            }
        }

        public StructuredSiteNode GetSubSiteNode(StructuredSiteNode node)
        {
            bool refernces;
            return GetSubSiteNode(node, out refernces);
        }

        public StructuredSiteNode GetSubSiteNode(StructuredSiteNode node, out bool references)
        {
            references = false;
            if (node == null)
                return null;

            while (node != null && node.StructuredSiteNodeType != StructuredSiteNodeType.SubSiteNode)
            {
                if (node.StructuredSiteNodeType == StructuredSiteNodeType.SubTreeReferencingFolderNode)
                    references = true;

                node = node.Parent as StructuredSiteNode;
            }

            return node;
        }

        public StructuredSiteNode GetFolderStructureNode(StructuredSiteNode node)
        {
            while (node != null && node.StructuredSiteNodeType != StructuredSiteNodeType.SubSiteNode)
            {
                if (node.StructuredSiteNodeType == StructuredSiteNodeType.FolderStructureNode)
                    return node;

                node = node.Parent as StructuredSiteNode;
            }

            return null;
        }

        public void SetActualTreeView(TreeViewWithFilter actualTreeView)
        {
            if (actualTreeView == ActualTreeView)
                return;

            actualTreeView = OtherTreeView;
            OtherTreeView = ActualTreeView;
            ActualTreeView = actualTreeView;
        }

        private void AddTreeNode(StructuredSiteNode parentNode, StructuredSiteNode newChildNode)
        {
            var otherTreeViewNode = parentNode != null ? parentNode.OtherTreeViewNode : null;

            ActualTreeView.AddNode(parentNode, newChildNode, KeySelectorForOrderBy);
            OtherTreeView.AddNode(otherTreeViewNode, newChildNode.OtherTreeViewNode, KeySelectorForOrderBy);
        }

        private void AddTreeNodes(StructuredSiteNode parentNode, ICollection<TreeNode> childNodes,
            ICollection<TreeNode> otherTreeViewChildNodes)
        {
            var otherTreeViewNode = parentNode != null ? parentNode.OtherTreeViewNode : null;

            ActualTreeView.AddNodes(parentNode, childNodes, KeySelectorForOrderBy);
            OtherTreeView.AddNodes(otherTreeViewNode, otherTreeViewChildNodes, KeySelectorForOrderBy);
        }

        private static string KeySelectorForOrderBy(TreeNode node)
        {
            var structuredSiteNode = node as StructuredSiteNode;
            if (structuredSiteNode != null)
            {
                return string.Format(
                    "{0} {1}",
                    structuredSiteNode.PriorityForOrderBy,
                    structuredSiteNode.Text);
            }

            return node.Text;
        }

        private void RemoveNodes(StructuredSiteNode parentNode, Func<TreeNode, bool> condition)
        {
            var otherTreeViewNode = parentNode != null ? parentNode.OtherTreeViewNode : null;

            ActualTreeView.DeleteNodes(parentNode, condition);
            OtherTreeView.DeleteNodes(otherTreeViewNode, condition);
        }

        private StructuredSiteNode CreateNode(string text, StructuredSiteNodeType structuredSiteNodeType, byte priorityForOrderBy)
        {
            var structuredSiteNode = new StructuredSiteNode(
                text,
                null,
                structuredSiteNodeType,
                priorityForOrderBy);

            structuredSiteNode.OtherTreeViewNode = new StructuredSiteNode(
                text,
                structuredSiteNode,
                structuredSiteNodeType,
                priorityForOrderBy);

            return structuredSiteNode;
        }

        private StructuredSiteNode CreateSubSiteNode(SiteIdAndName siteIdAndName)
        {
            var structuredSiteNode = CreateNode(siteIdAndName.Name, StructuredSiteNodeType.SubSiteNode, 2);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode,
                SubSiteNodeImageKey);

            var siteNodeInfo = new SiteNodeInfo(siteIdAndName.Id, structuredSiteNode);
            _siteNodesInfo.Add(siteIdAndName.Id, siteNodeInfo);

            structuredSiteNode.Tag = siteNodeInfo;

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode.OtherTreeViewNode,
                SubSiteNodeImageKey);

            structuredSiteNode.OtherTreeViewNode.Tag = siteNodeInfo;

            return structuredSiteNode;
        }

        private void CreateSubSiteFolder(StructuredSiteNode parentNode)
        {
            AddTreeNode(
                parentNode,
                CreateSubSiteFolderNode(
                    CgpClient.Singleton.LocalizationHelper.GetString(
                        "StructuredSiteNode_SubSites")));
        }

        private StructuredSiteNode CreateSubSiteFolderNode(string text)
        {
            var structuredSiteNode = CreateNode(text, StructuredSiteNodeType.SubSitesFolderNode, 1);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode,
                SubSitesFolderNodeImageKey);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode.OtherTreeViewNode,
                SubSitesFolderNodeImageKey);

            return structuredSiteNode;
        }

        private StructuredSiteNode CreateSubTreeReferencingFolderNode(string text)
        {
            var structuredSiteNode = CreateNode(text, StructuredSiteNodeType.SubTreeReferencingFolderNode, 3);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode,
                SubTreeReferencingNodeImageKey);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode.OtherTreeViewNode,
                SubTreeReferencingNodeImageKey);

            return structuredSiteNode;
        }

        private StructuredSiteNode CreateObjectTypeFolderNode(string text, ObjectType objectType)
        {
            var structuredSiteNode = CreateNode(text, StructuredSiteNodeType.ObjectTypeFolderNode, 0);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode,
                objectType.ToString());

            structuredSiteNode.Tag = objectType;

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode.OtherTreeViewNode,
                objectType.ToString());

            structuredSiteNode.OtherTreeViewNode.Tag = objectType;

            return structuredSiteNode;
        }

        private StructuredSiteNode CreateOrmObjectNode(ObjectInSiteInfo objectInfo)
        {
            var name = CgpClientMainForm.TranslateOrmObjectName(
                objectInfo.ObjectType,
                objectInfo.Name);

            var structuredSiteNode = CreateNode(name, StructuredSiteNodeType.OrmObjectNode, 6);

            string iconKey;

            var referenceInfo = objectInfo as ReferenceInSiteInfo;
            if (referenceInfo == null || referenceInfo.DefinedInThisSite)
            {
                iconKey = objectInfo.ObjectType.ToString();
            }
            else
            {
                iconKey = string.Format("{0}_{1}", objectInfo.ObjectType, ObjectReferenceFromParentImageKey);
            }

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode,
                iconKey);

            structuredSiteNode.Tag = objectInfo;

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode.OtherTreeViewNode,
                iconKey);

            structuredSiteNode.OtherTreeViewNode.Tag = objectInfo;

            return structuredSiteNode;
        }

        private StructuredSiteNode CreateHardewareFolderNode(string text)
        {
            var structuredSiteNode = CreateNode(text, StructuredSiteNodeType.HardwareFolderNode, 0);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode,
                HardwareFolderNodeImageKey);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode.OtherTreeViewNode,
                HardwareFolderNodeImageKey);

            return structuredSiteNode;
        }

        private void CreateFolderStructuresFolder(StructuredSiteNode parentNode)
        {
            if (!HasAccessViewFolderStructure())
                return;

            AddTreeNode(
                parentNode,
                CreateFolderStructuresFolderNode());
        }

        private StructuredSiteNode CreateFolderStructuresFolderNode()
        {
            var structuredSiteNode =
                CreateNode(
                    CgpClient.Singleton.LocalizationHelper.GetString("StructuredSiteNode_FolderStructures"),
                    StructuredSiteNodeType.FolderStructuresFolderNode, 4);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode,
                FolderStructureNodeImageKey);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode.OtherTreeViewNode,
                FolderStructureNodeImageKey);

            return structuredSiteNode;
        }

        private StructuredSiteNode CreateFolderStructureNode(ObjectInSiteInfo objectInfo)
        {
            var structuredSiteNode = CreateNode(objectInfo.Name, StructuredSiteNodeType.FolderStructureNode, 5);

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode,
                FolderStructureNodeImageKey);

            structuredSiteNode.Tag = objectInfo;

            TreeViewWithFilter.SetNodeIcon(
                structuredSiteNode.OtherTreeViewNode,
                FolderStructureNodeImageKey);

            structuredSiteNode.OtherTreeViewNode.Tag = objectInfo;

            return structuredSiteNode;
        }

        private StructuredSiteNode AddSubSite(StructuredSiteNode parentNode, SiteIdAndName siteIdAndName)
        {
            StructuredSiteNode newSubSiteNode = CreateSubSiteNode(siteIdAndName);
            var subSitesFolderNode = GetOrCreateSubSitesFolder(parentNode);
            AddTreeNode(subSitesFolderNode, newSubSiteNode);
            CreateSubTreeReferencingFolder(newSubSiteNode);
            CreateSubSiteFolder(newSubSiteNode);
            CreateFolderStructuresFolder(newSubSiteNode);

            return newSubSiteNode;
        }

        private StructuredSiteNode GetSubSitesFolder(StructuredSiteNode parentNode)
        {
            return ActualTreeView.FindNode(parentNode, node =>
            {
                var structuredSiteNode = node as StructuredSiteNode;
                if (structuredSiteNode == null)
                    return false;

                return structuredSiteNode.StructuredSiteNodeType == StructuredSiteNodeType.SubSitesFolderNode;
            }) as StructuredSiteNode;
        }

        private StructuredSiteNode GetOrCreateSubSitesFolder(StructuredSiteNode parentNode)
        {
            StructuredSiteNode subSitesFolderNode = GetSubSitesFolder(parentNode);

            if (subSitesFolderNode != null)
                return subSitesFolderNode;

            StructuredSiteNode newSubSitesFolderNode =
                CreateSubSiteFolderNode(CgpClient.Singleton.LocalizationHelper.GetString("StructuredSiteNode_SubSites"));

            AddTreeNode(parentNode, newSubSitesFolderNode);

            return newSubSitesFolderNode;
        }

        public StructuredSiteNode GetSubTreeReferencingFolder(StructuredSiteNode parentNode)
        {
            return ActualTreeView.FindNode(parentNode, node =>
            {
                var structuredSiteNode = node as StructuredSiteNode;
                if (structuredSiteNode == null)
                    return false;

                return structuredSiteNode.StructuredSiteNodeType == StructuredSiteNodeType.SubTreeReferencingFolderNode;
            }) as StructuredSiteNode;
        }

        private void CreateSubTreeReferencingFolder(StructuredSiteNode parentNode)
        {
            AddTreeNode(
                parentNode,
                CreateSubTreeReferencingFolderNode(
                    CgpClient.Singleton.LocalizationHelper.GetString(
                        "StructuredSiteNode_SubtreeReferencing")));
        }

        public bool IsObjectTypeInTheHardwareFolder(ObjectType objectType)
        {
            return (objectType == ObjectType.CCU ||
                    objectType == ObjectType.DCU ||
                    objectType == ObjectType.DoorEnvironment ||
                    objectType == ObjectType.CardReader ||
                    objectType == ObjectType.Input ||
                    objectType == ObjectType.Output ||
                    objectType == ObjectType.CisNG ||
                    objectType == ObjectType.CisNGGroup);
        }

        private StructuredSiteNode GetOrCreateObjectTypeFolder(StructuredSiteNode parentNode, ObjectType objectType)
        {
            var objectTypeFolderNode = GetObjectTypeFolder(parentNode, objectType);
            if (objectTypeFolderNode != null)
                return objectTypeFolderNode;

            var newObjectTypeFolderNode = CreateObjectTypeFolderNode(
                CgpClient.Singleton.LocalizationHelper.GetString(string.Format("ObjectType_{0}", objectType)),
                objectType);

            AddTreeNode(parentNode, newObjectTypeFolderNode);

            return newObjectTypeFolderNode;
        }

        private StructuredSiteNode GetObjectTypeFolder(StructuredSiteNode parentNode, ObjectType objectType)
        {
            return ActualTreeView.FindNode(parentNode, node =>
            {
                var structuredSiteNode = node as StructuredSiteNode;
                if (structuredSiteNode == null)
                    return false;

                return structuredSiteNode.StructuredSiteNodeType == StructuredSiteNodeType.ObjectTypeFolderNode &&
                       (ObjectType) structuredSiteNode.Tag == objectType;
            }) as StructuredSiteNode;
        }

        private StructuredSiteNode GetOrCreateHardwareFolder(StructuredSiteNode parentNode)
        {
            var hardwareFolderNode = GetHardwareFolder(parentNode);
            if (hardwareFolderNode != null)
                return hardwareFolderNode;

            var newHardwareFolderNode = CreateHardewareFolderNode(CgpClient.Singleton.LocalizationHelper.GetString("StructuredSiteNode_Hardware"));

            AddTreeNode(parentNode, newHardwareFolderNode);

            return newHardwareFolderNode;
        }

        private StructuredSiteNode GetHardwareFolder(StructuredSiteNode parentNode)
        {
            return ActualTreeView.FindNode(parentNode, node =>
            {
                var structuredSiteNode = node as StructuredSiteNode;
                if (structuredSiteNode == null)
                    return false;

                return structuredSiteNode.StructuredSiteNodeType == StructuredSiteNodeType.HardwareFolderNode;
            }) as StructuredSiteNode;
        }

        private StructuredSiteNode GetFolderStructuresFolder(StructuredSiteNode parentNode)
        {
            return ActualTreeView.FindNode(parentNode, node =>
            {
                var structuredSiteNode = node as StructuredSiteNode;
                if (structuredSiteNode == null)
                    return false;

                return structuredSiteNode.StructuredSiteNodeType == StructuredSiteNodeType.FolderStructuresFolderNode;
            }) as StructuredSiteNode;
        }

        private int _startedUpdateCount;

        private volatile object _LockBeginEndUpdate = new object();

        public void BeginUpdate()
        {
            lock (_LockBeginEndUpdate)
            {
                if (_startedUpdateCount == 0)
                {
                    DoBeginUpdate();
                }

                _startedUpdateCount++;
            }
        }

        private void DoBeginUpdate()
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.BeginInvoke(new DVoid2Void(DoBeginUpdate));
                return;
            }

            ActualTreeView.Refresh();
            ActualTreeView.DisableReadrawingTreeNodes(true);
            ActualTreeView.BeginUpdate();

            OtherTreeView.Refresh();
            OtherTreeView.DisableReadrawingTreeNodes(true);
            OtherTreeView.BeginUpdate();
        }

        public void EndUpdate()
        {
            _endUpdateProcessingQueue.Enqueue(true);
        }

        private void EndUpdateCore(bool value)
        {
            lock (_LockBeginEndUpdate)
            {
                if (_startedUpdateCount == 0)
                    return;

                if (_startedUpdateCount > 1)
                {
                    _startedUpdateCount--;
                    return;
                }
            }

            System.Threading.Thread.Sleep(500);

            lock (_LockBeginEndUpdate)
            {
                _startedUpdateCount--;

                if (_startedUpdateCount > 0)
                    return;

                DoEndUpdate();
            }
        }

        private void DoEndUpdate()
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.BeginInvoke(new DVoid2Void(DoEndUpdate));
                return;
            }

            ActualTreeView.EndUpdate();
            ActualTreeView.DisableReadrawingTreeNodes(false);

            OtherTreeView.EndUpdate();
            OtherTreeView.DisableReadrawingTreeNodes(false);

            if (ActualTreeView.SelectedNode != null)
                ActualTreeView.SelectedNode.EnsureVisible();
        }

        public bool ObjectExistInTheSubSite(IdAndObjectType objectId, StructuredSiteNode parentNode)
        {
            StructuredSiteNode objectTypeFolderNode;
            if (IsObjectTypeInTheHardwareFolder(objectId.ObjectType))
            {
                parentNode = GetHardwareFolder(parentNode);
                if (parentNode == null)
                    return false;

                objectTypeFolderNode = GetObjectTypeFolder(parentNode, objectId.ObjectType);
            }
            else
            {
                objectTypeFolderNode = GetObjectTypeFolder(parentNode, objectId.ObjectType);
            }

            if (objectTypeFolderNode == null)
                return false;

            return ActualTreeView.FindNode(objectTypeFolderNode, node =>
            {
                var structuredSiteNode = node as StructuredSiteNode;
                if (structuredSiteNode == null ||
                    structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.OrmObjectNode)
                {
                    return false;
                }

                var referenceInfo = structuredSiteNode.Tag as ReferenceInSiteInfo;
                if (referenceInfo != null && !referenceInfo.DefinedInThisSite)
                    return false;

                return ((IdAndObjectType)structuredSiteNode.Tag).Equals(objectId);
            }) != null;
        }

        public void SelectObject(int siteId, IdAndObjectType objectId, bool isReference)
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.BeginInvoke(
                    new Action<int, IdAndObjectType, bool>(SelectObject),
                    siteId,
                    objectId,
                    isReference);
            }
            else
            {
                SiteNodeInfo siteNodeInfo;
                if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo))
                {
                    return;
                }

                var siteNode = siteNodeInfo.SiteNode;

                if (siteNode != null &&
                    siteNode.TreeView != ActualTreeView)
                {
                    siteNode = siteNode.OtherTreeViewNode;
                }

                if (!siteNodeInfo.ObjectsWasLoaded)
                    LoadSiteNode(siteNode, siteNodeInfo);

                if (isReference)
                {
                    siteNode = GetSubTreeReferencingFolder(siteNode);
                    if (siteNode == null)
                        return;
                }

                StructuredSiteNode objectTypeFolderNode;
                if (IsObjectTypeInTheHardwareFolder(objectId.ObjectType))
                {
                    siteNode = GetHardwareFolder(siteNode);
                    if (siteNode == null)
                        return;

                    objectTypeFolderNode = GetObjectTypeFolder(siteNode, objectId.ObjectType);
                }
                else if (objectId.ObjectType == ObjectType.UserFoldersStructure)
                {
                    objectTypeFolderNode = GetFolderStructuresFolder(siteNode);
                }
                else
                {
                    objectTypeFolderNode = GetObjectTypeFolder(siteNode, objectId.ObjectType);
                }

                if (objectTypeFolderNode == null)
                    return;

                var objectNode = ActualTreeView.FindNode(
                    objectTypeFolderNode,
                    node =>
                        objectId.Equals(node.Tag as IdAndObjectType))
                    as StructuredSiteNode;

                if (objectNode != null)
                {
                    ActualTreeView.SelectedNode = objectNode;
                }
            }
        }

        public void SelectObject(int siteId, IdAndObjectType parentId, IdAndObjectType objectId)
        {
            if (ActualTreeView.Parent.InvokeRequired)
            {
                ActualTreeView.Parent.BeginInvoke(
                    new Action<int, IdAndObjectType, IdAndObjectType>(SelectObject),
                    siteId,
                    parentId,
                    objectId);
            }
            else
            {
                SiteNodeInfo siteNodeInfo;
                if (!_siteNodesInfo.TryGetValue(siteId, out siteNodeInfo))
                {
                    return;
                }

                if (!siteNodeInfo.ObjectsWasLoaded)
                    LoadSiteNode(siteNodeInfo.SiteNode, siteNodeInfo);

                FolderStructureObjectsInfo folderStructureObjectsInfo;
                if (!_folderStructuresObjectsInfo.TryGetValue(parentId, out folderStructureObjectsInfo))
                    return;

                StructuredSiteNode folderStructureNode;
                if (folderStructureObjectsInfo.FolderStructureNode != null &&
                    folderStructureObjectsInfo.FolderStructureNode.TreeView != ActualTreeView)
                {
                    folderStructureNode = folderStructureObjectsInfo.FolderStructureNode.OtherTreeViewNode;
                }
                else
                {
                    folderStructureNode = folderStructureObjectsInfo.FolderStructureNode;
                }

                if (folderStructureNode == null)
                    return;

                var objectNode = ActualTreeView.FindNode(
                    folderStructureNode,
                    node =>
                        objectId.Equals(node.Tag as IdAndObjectType))
                    as StructuredSiteNode;

                if (objectNode != null)
                {
                    ActualTreeView.SelectedNode = objectNode;
                }
            }
        }

        public void ExpandSiteNode(StructuredSiteNode siteNode)
        {
            if (siteNode == null || siteNode.StructuredSiteNodeType != StructuredSiteNodeType.SubSiteNode)
                return;

            var siteNodeInfo = (SiteNodeInfo)siteNode.Tag;
            if (!siteNodeInfo.ObjectsWasLoaded)
                LoadSiteNode(siteNode, siteNodeInfo);
        }

        public bool HasAccessForSite(int siteId)
        {
            return _siteNodesInfo.ContainsKey(siteId);
        }

        public void Clear()
        {
            ActualTreeView.Clear();
            OtherTreeView.Clear();

            _siteNodesInfo.Clear();
            _folderStructuresObjectsInfo.Clear();

            if (_specialReadingOfChildObjects == null)
                return;

            _specialReadingOfChildObjects.Clear();
            _specialReadingOfChildObjects = null;
        }
    }
}
