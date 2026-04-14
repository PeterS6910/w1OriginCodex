using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.PlatformPC.UI;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class StructuredSiteForm :
#if DESIGNER
        Form
#else
        ACgpTableForm<StructuredSubSite, StructuredSubSite>,
        IEditFormBase
#endif
    {
        private enum DragObjectSource : byte
        {
            StrucutredSite = 0,
            FolderStructure = 1,
            BrokenReferences = 2
        }

        private class DragObject
        {
            public ObjectInSiteInfo ObjectInfo { get; private set; }
            public bool IsReference { get; private set; }
            public DragObjectSource Source { get; private set; }
            public object SourceData { get; private set; }
            public int SiteId { get; private set; }

            public DragObject(ObjectInSiteInfo objectInfo, bool isReference, DragObjectSource source, object sourceData,
                int siteId)
            {
                ObjectInfo = objectInfo;
                IsReference = isReference;
                Source = source;
                SourceData = sourceData;
                SiteId = siteId;
            }
        }

        private static volatile StructuredSiteForm _singleton;
        private static readonly object _syncRoot = new object();
        private IStructuredSiteBuilder _structuredSiteBuilder;
        private readonly StructuredSiteTreeViews _structuredSiteTreeViews;
        private bool _hasAccessUpdate;
        private readonly HashSet<int> _subSitesForLogin = new HashSet<int>();
        private bool _reloadStructuredTreeViews = true;
        private bool _showNotificationStructuredWasChanged;
        private readonly StructuredSiteBuilderClient _structuredSiteBuilderClient;
        private bool? _showedNoObjectsWithBrokenReferences;
        private ObjectPlacement _selectedObjectPlacement;
        private IdAndObjectType _selectedObject;

        private bool _hasAccessViewFolderStructure;
        private bool _hasAccessUpdateFolderStructure;
        private bool _hasAccessInsertFolderStructure;
        private bool _hasAccessDeleteFolderStructure;

        public static StructuredSiteForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new StructuredSiteForm
                            {
                                MdiParent = CgpClientMainForm.Singleton
                            };
                        }
                    }

                return _singleton;
            }
        }

        public StructuredSiteForm()
        {
            InitializeComponent();

            var objectsImageList = new ImageList();
            ObjectImageList.Singleton.FillWithClientAndPluginsObjectImages(objectsImageList);
            objectsImageList.Images.Add(TreeViewWithFilter.FILTER_NODE_IMAGE_KEY, ResourceGlobal.Search16);
            objectsImageList.Images.Add(StructuredSiteTreeViews.SubSitesFolderNodeImageKey, ResourceGlobal.Folder_collaps);
            objectsImageList.Images.Add(StructuredSiteTreeViews.SubSiteNodeImageKey, ResourceGlobal.Folder_expand);
            objectsImageList.Images.Add(StructuredSiteTreeViews.HardwareFolderNodeImageKey, ResourceGlobal.Hardware);
            objectsImageList.Images.Add(StructuredSiteTreeViews.SubTreeReferencingNodeImageKey, ResourceGlobal.SubTreeReferencing);
            objectsImageList.Images.Add(StructuredSiteTreeViews.FolderStructureNodeImageKey, ResourceGlobal.Folder_expand);

            foreach (ObjectType objectType in Enum.GetValues(typeof(ObjectType)))
            {
                if (objectsImageList.Images.ContainsKey(objectType.ToString()))
                {
                    var objectTypeIcon = objectsImageList.Images[objectType.ToString()];

                    if (objectTypeIcon == null)
                        continue;

                    var objectTypeReferenceFromParent = new Bitmap(
                        objectTypeIcon.Width + ResourceGlobal.ScrollUp.Width/2,
                        objectTypeIcon.Height + ResourceGlobal.ScrollUp.Height/2);

                    using (Graphics g = Graphics.FromImage(objectTypeReferenceFromParent))
                    {
                        g.DrawImage(objectTypeIcon, ResourceGlobal.ScrollUp.Width/2, ResourceGlobal.ScrollUp.Height/2);
                        g.DrawImage(ResourceGlobal.ScrollUp, 0, 0);
                    }

                    objectsImageList.Images.Add(
                        string.Format(
                            "{0}_{1}",
                            objectType,
                            StructuredSiteTreeViews.ObjectReferenceFromParentImageKey),
                        objectTypeReferenceFromParent);
                }
            }

            _bDelete.Enabled = false;
            _bCreateSubSite.Enabled = false;
            _bRenameSubSite.Enabled = false;

            _bCreateFolder.Enabled = false;
            _bRenameFolder.Enabled = false;

            _tvStructuredSiteLeft.ImageList = objectsImageList;
            _tvStructuredSiteRight.ImageList = objectsImageList;
            _tvBrokenReferences.ImageList = objectsImageList;

            _tvStructuredSiteLeft.AfterSelect += NodeMouseClick;
            //_tvStructuredSiteLeft.NodeMouseClick += NodeMouseClick;
            _tvStructuredSiteLeft.NodeMouseDoubleClick += NodeMouseDoubleClick;
            _tvStructuredSiteLeft.NodeAfterEdit += NodeAfterEdit;
            _tvStructuredSiteLeft.AllowDrop = true;
            _tvStructuredSiteLeft.ItemDrag += StructuredTreeViewItemDrag;
            _tvStructuredSiteLeft.DragOver += StructuredTreeViewDragOver;
            _tvStructuredSiteLeft.DragDrop += StructuredTreeViewDragDrop;

            _tvStructuredSiteRight.AfterSelect += NodeMouseClick;
            //_tvStructuredSiteRight.NodeMouseClick += NodeMouseClick;
            _tvStructuredSiteRight.NodeMouseDoubleClick += NodeMouseDoubleClick;
            _tvStructuredSiteRight.NodeAfterEdit += NodeAfterEdit;
            _tvStructuredSiteRight.AllowDrop = true;
            _tvStructuredSiteRight.ItemDrag += StructuredTreeViewItemDrag;
            _tvStructuredSiteRight.DragOver += StructuredTreeViewDragOver;
            _tvStructuredSiteRight.DragDrop += StructuredTreeViewDragDrop;

            _structuredSiteBuilderClient = new StructuredSiteBuilderClient(this);

            _structuredSiteTreeViews = new StructuredSiteTreeViews(_tvStructuredSiteLeft, _tvStructuredSiteRight);
            _structuredSiteTreeViews.HasAccessViewFolderStructure += HasAccessViewFolderStructure;
            _structuredSiteTreeViews.LoadFolderStructureObjects += LoadFolderStructureObjects;
            _structuredSiteTreeViews.LoadSubSites += LoadSubSites;
            _structuredSiteTreeViews.LoadObjectsInSite += LoadObjectsInSite;
            _structuredSiteTreeViews.LoadSubTreeReferences += LoadSubTreeReferences;
            _structuredSiteTreeViews.AddObjectToFolderStructure += AddObjectToFolderStructure;
            _structuredSiteTreeViews.RemoveObjectFromFolderStructure += RemoveObjectFromFolderStructure;

            _scSructuredSite.SplitterDistance = (_scSructuredSite.Width - _scSructuredSite.SplitterWidth) / 2;
        }

        public void OpenFormAndSelectObject(ObjectPlacement objectPlacement, IdAndObjectType objectId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<ObjectPlacement, IdAndObjectType>(OpenFormAndSelectObject),
                    objectPlacement,
                    objectId);
            }
            else
            {
                _selectedObjectPlacement = objectPlacement;
                _selectedObject = objectId;
                Show();
            }
        }

        private void NodeMouseClick(object sender, TreeViewEventArgs e)
        {
            NodeMouseClick(e.Node);
        }

        private void NodeMouseClick(TreeNode node)
        {
            if (node == null)
                return;

            SetFocusedTreeView(node);

            var selectedNodes = _structuredSiteTreeViews.ActualTreeView.SelectedNodes;

            var bDeleteEnabled = selectedNodes.Count > 0;
            var bCreateSubSiteEnabled = selectedNodes.Count == 1;
            var bRenameSubSiteEnabled = selectedNodes.Count == 1;
            var bCreateFolderEnabled = selectedNodes.Count == 1;
            var bRenameFolderEnabled = selectedNodes.Count == 1;

            foreach (var selectedNode in selectedNodes)
            {
                var structuredSiteNode = selectedNode as StructuredSiteNode;
                if (structuredSiteNode == null)
                {
                    bDeleteEnabled = false;
                    bCreateSubSiteEnabled = false;
                    bRenameSubSiteEnabled = false;
                    bCreateFolderEnabled = false;
                    bRenameFolderEnabled = false;

                    break;
                }

                switch (structuredSiteNode.StructuredSiteNodeType)
                {
                    case StructuredSiteNodeType.SubSitesFolderNode:
                        bDeleteEnabled = false;
                        bRenameSubSiteEnabled = false;
                        bCreateFolderEnabled = false;
                        bRenameFolderEnabled = false;

                        if (!_hasAccessUpdate)
                            bCreateSubSiteEnabled = false;

                        break;

                    case StructuredSiteNodeType.SubSiteNode:
                        bCreateSubSiteEnabled = false;
                        bCreateFolderEnabled = false;
                        bRenameFolderEnabled = false;

                        if (!_hasAccessUpdate)
                        {
                            bDeleteEnabled = false;
                            bRenameSubSiteEnabled = false;
                        }

                        break;

                    case StructuredSiteNodeType.OrmObjectNode:
                        bCreateSubSiteEnabled = false;
                        bRenameSubSiteEnabled = false;
                        bCreateFolderEnabled = false;
                        bRenameFolderEnabled = false;

                        StructuredSiteNode parentNode;
                        var referenceInfo = structuredSiteNode.Tag as ReferenceInSiteInfo;

                        if (referenceInfo != null)
                        {
                            if (_structuredSiteTreeViews.IsObjectTypeInTheHardwareFolder(referenceInfo.ObjectType))
                                parentNode = structuredSiteNode.Parent.Parent.Parent as StructuredSiteNode;
                            else
                                parentNode = structuredSiteNode.Parent.Parent as StructuredSiteNode;

                            if (parentNode == null &&
                                referenceInfo.ObjectType != ObjectType.Login &&
                                referenceInfo.ObjectType != ObjectType.LoginGroup)
                            {
                                bDeleteEnabled = false;
                                break;
                            }

                            if (referenceInfo.Parent != null &&
                                _structuredSiteTreeViews.ObjectExistInTheSubSite(referenceInfo.Parent, parentNode))
                            {
                                bDeleteEnabled = false;
                                break;
                            }

                            if (!(structuredSiteNode.Tag as ReferenceInSiteInfo).DefinedInThisSite)
                            {
                                bDeleteEnabled = false;
                                break;
                            }

                            if (referenceInfo.ObjectType == ObjectType.Login ||
                                referenceInfo.ObjectType == ObjectType.LoginGroup)
                            {
                                try
                                {
                                    if (_structuredSiteBuilder.GetSubTreeReferenceCount(referenceInfo) == 1)
                                    {
                                        bDeleteEnabled = false;
                                        break;
                                    }
                                }
                                catch
                                {
                                    CgpClient.Singleton.IsConnectionLost(false);
                                    return;
                                }
                            }

                            if (!_hasAccessUpdate)
                                bDeleteEnabled = false;

                            break;
                        }

                        parentNode = structuredSiteNode.Parent as StructuredSiteNode;
                        if (parentNode == null ||
                            parentNode.StructuredSiteNodeType != StructuredSiteNodeType.FolderStructureNode)
                        {
                            bDeleteEnabled = false;
                            break;
                        }

                        if (!_hasAccessUpdateFolderStructure)
                            bDeleteEnabled = false;

                        break;

                    case StructuredSiteNodeType.FolderStructuresFolderNode:
                        bDeleteEnabled = false;
                        bCreateSubSiteEnabled = false;
                        bRenameSubSiteEnabled = false;
                        bRenameFolderEnabled = false;

                        if (!_hasAccessInsertFolderStructure)
                            bCreateFolderEnabled = false;
                        
                        break;

                    case StructuredSiteNodeType.FolderStructureNode:
                        bCreateSubSiteEnabled = false;
                        bRenameSubSiteEnabled = false;

                        if (!_hasAccessDeleteFolderStructure)
                            bDeleteEnabled = false;

                        if (!_hasAccessInsertFolderStructure)
                            bCreateFolderEnabled = false;

                        if (!_hasAccessUpdateFolderStructure)
                            bRenameFolderEnabled = false;

                        break;

                    default:
                        bDeleteEnabled = false;
                        bCreateSubSiteEnabled = false;
                        bRenameSubSiteEnabled = false;
                        bCreateFolderEnabled = false;
                        bRenameFolderEnabled = false;
                        break;
                }
            }


            _bDelete.Enabled = bDeleteEnabled;

            _bCreateSubSite.Enabled = bCreateSubSiteEnabled;
            _bRenameSubSite.Enabled = bRenameSubSiteEnabled;

            _bCreateFolder.Enabled = bCreateFolderEnabled;
            _bRenameFolder.Enabled = bRenameFolderEnabled;
        }

        private void NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
                return;

            var objectInfo = e.Node.Tag as ObjectInSiteInfo;
            if (objectInfo == null || objectInfo.ObjectType == ObjectType.UserFoldersStructure)
                return;

            var tableObject = DbsSupport.GetTableObject(objectInfo.ObjectType, objectInfo.Id.ToString());
            if (tableObject == null)
                return;

            DbsSupport.OpenEditForm(tableObject);
        }

        private void NodeAfterEdit(TreeNode node)
        {
            var structuredSiteNode = node as StructuredSiteNode;
            if (structuredSiteNode == null)
                return;

            if (structuredSiteNode.StructuredSiteNodeType == StructuredSiteNodeType.SubSiteNode)
            {
                var siteNodeInfo = structuredSiteNode.Tag as SiteNodeInfo;
                if (siteNodeInfo == null)
                    return;

                BeginUpdate();

                SafeThread<int, string>.StartThread(RenameSite, siteNodeInfo.SiteId, structuredSiteNode.Text);

                EndUpdate();
            }
            else
            {
                var folderId = structuredSiteNode.Tag as IdAndObjectType;
                if (folderId == null)
                    return;

                BeginUpdate();

                SafeThread<int, string>.StartThread(RenameFolder, folderId, structuredSiteNode.Text);

                EndUpdate();
            }
        }

        private void RenameSite(int siteId, string newName)
        {
            try
            {
                _structuredSiteBuilder.RenameSite(siteId, newName);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        private void StructuredTreeViewItemDrag(object sender, ItemDragEventArgs e)
        {
            var dragNode = e.Item as StructuredSiteNode;
            if (dragNode == null)
                return;

            var structuredSiteNodes = GetTreeViewForNode(dragNode).IsSelected(dragNode)
                ? GetTreeViewForNode(dragNode).SelectedNodes.OfType<StructuredSiteNode>()
                : new LinkedList<StructuredSiteNode>(Enumerable.Repeat(
                    dragNode,
                    1));

            var dragObjects = new LinkedList<DragObject>();
            DragDropEffects dragDropEffects = DragDropEffects.Move | DragDropEffects.Copy;

            foreach (var structuredSiteNode in structuredSiteNodes)
            {
                if (structuredSiteNode.StructuredSiteNodeType != StructuredSiteNodeType.OrmObjectNode)
                    continue;

                var objectInfo = (ObjectInSiteInfo)structuredSiteNode.Tag;
                switch (objectInfo.ObjectType)
                {
                    case ObjectType.LoginGroup:
                        if (!HasAccessDragLoginGroup(objectInfo.Name))
                            continue;

                        break;
                    case ObjectType.Login:
                        if (!HasAccessDragLogin(objectInfo.Name))
                            continue;

                        break;
                }

                bool isReference;
                var siteNode = _structuredSiteTreeViews.GetSubSiteNode(structuredSiteNode, out isReference);

                if (isReference ||
                    objectInfo.ObjectType == ObjectType.Login ||
                    objectInfo.ObjectType == ObjectType.LoginGroup)
                {
                    var referenceInfo = structuredSiteNode.Tag as ReferenceInSiteInfo;
                    if (referenceInfo == null || !referenceInfo.DefinedInThisSite)
                    {
                        continue;
                    }

                    if (isReference && !HasAccessForReferencedObject(referenceInfo))
                    {
                        continue;
                    }
                }

                object sourceData;
                var source = GetObjectSource(structuredSiteNode, out sourceData);

                if (objectInfo.ObjectType == ObjectType.Login
                    || objectInfo.ObjectType == ObjectType.LoginGroup)
                {
                    if (objectInfo.Parent != null &&
                        source != DragObjectSource.FolderStructure &&
                        _structuredSiteTreeViews.ObjectExistInTheSubSite(
                            objectInfo.Parent,
                            siteNode))
                    {
                        continue;
                    }
                }
                else
                {
                    if (e.Button == MouseButtons.Right
                        || (objectInfo.Parent != null &&
                            source != DragObjectSource.FolderStructure &&
                            _structuredSiteTreeViews.ObjectExistInTheSubSite(
                                objectInfo.Parent,
                                isReference
                                    ? _structuredSiteTreeViews.GetSubTreeReferencingFolder(siteNode)
                                    : siteNode)))
                    {
                        dragDropEffects = DragDropEffects.Copy;
                    }
                }

                dragObjects.AddLast(
                    new DragObject(
                        objectInfo,
                        isReference,
                        source,
                        sourceData,
                        siteNode != null ? ((SiteNodeInfo) siteNode.Tag).SiteId : -1));
            }

            if (dragObjects.Count == 0)
                return;

            DoDragDrop(dragObjects, dragDropEffects);
        }

        private bool HasAccessDragLoginGroup(string loginGroupName)
        {
            if (loginGroupName == CgpServerGlobals.DEFAULT_ADMIN_LOGIN_GROUP)
                return false;

            if (CgpClient.Singleton.MainServerProvider.LoginGroups.IsLoginGroupSuperAdmin(loginGroupName))
                return CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs);

            return true;
        }

        private bool HasAccessDragLogin(string loginName)
        {
            if (loginName == CgpServerGlobals.DEFAULT_ADMIN_LOGIN)
                return false;

            if (CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(loginName))
                return CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs);

            return true;
        }

        private DragObjectSource GetObjectSource(StructuredSiteNode objectNode, out object sourceData)
        {
            var folderStructureNode = _structuredSiteTreeViews.GetFolderStructureNode(objectNode);
            if (folderStructureNode != null)
            {
                sourceData = folderStructureNode.Tag;
                return DragObjectSource.FolderStructure;
            }

            sourceData = null;
            return DragObjectSource.StrucutredSite;
        }

        private bool HasAccessForReferencedObject(ReferenceInSiteInfo referenceInfo)
        {
            int objectSiteId;
            try
            {
                objectSiteId = _structuredSiteBuilder.GetSiteOfObject(referenceInfo);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
                return false;
            }

            return _structuredSiteTreeViews.HasAccessForSite(objectSiteId);
        }

        private void StructuredTreeViewDragOver(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                Point pt = ((TreeView) sender).PointToClient(new Point(e.X, e.Y));
                var dragDropNode = ((TreeView) sender).GetNodeAt(pt) as StructuredSiteNode;
                if (dragDropNode == null ||
                    dragDropNode.StructuredSiteNodeType == StructuredSiteNodeType.SubSitesFolderNode ||
                    dragDropNode.StructuredSiteNodeType == StructuredSiteNodeType.FolderStructuresFolderNode)
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }

                var dragObjects = e.Data.GetData(output[0]) as ICollection<DragObject>;

                if (dragObjects == null)
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }

                foreach (var dragObject in dragObjects)
                {
                    var folderStructureNode = _structuredSiteTreeViews.GetFolderStructureNode(dragDropNode);
                    if (folderStructureNode != null)
                    {
                        if (!_hasAccessUpdateFolderStructure)
                        {
                            e.Effect = DragDropEffects.None;
                            return;
                        }

                        var effect = StructuredTreeViewDragOverFolderStructureNode(
                            dragObject,
                            folderStructureNode,
                            e.AllowedEffect);

                        e.Effect = effect;

                        if (effect == DragDropEffects.None)
                            return;

                        continue;
                    }

                    if (!_hasAccessUpdate)
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }

                    bool dragDropNodeIsReference;
                    var dragDropSiteNode = _structuredSiteTreeViews.GetSubSiteNode(
                        dragDropNode,
                        out dragDropNodeIsReference);

                    var dragDropSiteId = dragDropSiteNode != null
                        ? ((SiteNodeInfo) dragDropSiteNode.Tag).SiteId
                        : -1;

                    if (dragDropSiteId == dragObject.SiteId &&
                        dragDropNodeIsReference == dragObject.IsReference)
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }

                    if (dragDropSiteId == -1 &&
                        dragObject.Source == DragObjectSource.FolderStructure)
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }

                    if (dragDropNodeIsReference)
                    {
                        if (dragObject.ObjectInfo.ObjectType == ObjectType.Login ||
                            dragObject.ObjectInfo.ObjectType == ObjectType.LoginGroup ||
                            dragObject.ObjectInfo.ObjectType == ObjectType.ACLGroup)
                        {
                            e.Effect = DragDropEffects.None;
                            return;
                        }
                    }
                    else if (dragObject.IsReference
                             || dragObject.Source == DragObjectSource.BrokenReferences
                             || (dragObject.ObjectInfo.ObjectType != ObjectType.Login
                                 && dragObject.ObjectInfo.ObjectType != ObjectType.LoginGroup
                                 && e.AllowedEffect == DragDropEffects.Copy))
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }

                    if (e.AllowedEffect == DragDropEffects.Copy ||
                        (dragDropNodeIsReference
                         && !dragObject.IsReference))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                }
            }
            catch
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private DragDropEffects StructuredTreeViewDragOverFolderStructureNode(DragObject dragObject,
            StructuredSiteNode folderStructureNode, DragDropEffects allowedEffects)
        {
            var effect = allowedEffects == DragDropEffects.Copy ||
                         dragObject.Source != DragObjectSource.FolderStructure
                ? DragDropEffects.Copy
                : DragDropEffects.Move;

            if (dragObject.ObjectInfo.ObjectType == ObjectType.Person)
            {
                var folderStructureSiteNode = _structuredSiteTreeViews.GetSubSiteNode(folderStructureNode);
                var folderStructureSiteNodeId = folderStructureSiteNode == null
                    ? -1
                    : ((SiteNodeInfo)folderStructureSiteNode.Tag).SiteId;

                if (dragObject.SiteId != folderStructureSiteNodeId)
                {
                   return DragDropEffects.None;
                }

                return effect;
            }

            return effect;
        }

        private void StructuredTreeViewDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Effect == DragDropEffects.None)
                    return;

                string[] output = e.Data.GetFormats();
                if (output == null)
                    return;

                var dragObjects = e.Data.GetData(output[0]) as ICollection<DragObject>;
                if (dragObjects == null)
                    return;

                var dragDropNode =
                    ((TreeView) sender).GetNodeAt(
                        ((TreeView) sender).PointToClient(new Point(e.X, e.Y))) as StructuredSiteNode;

                if (dragDropNode == null)
                    return;

                SetFocusedTreeView(dragDropNode);

                var folderStructureNode = _structuredSiteTreeViews.GetFolderStructureNode(dragDropNode);
                if (folderStructureNode != null)
                {
                    var folderStructureId = (IdAndObjectType) folderStructureNode.Tag;

                    BeginUpdate();

                    foreach (var dragObject in dragObjects)
                    {
                        var folderStructuresContainsObject = _structuredSiteTreeViews.GetFolderStructuresForObject(
                            dragObject.ObjectInfo);

                        if (dragObject.ObjectInfo.ObjectType == ObjectType.Person)
                        {
                            foreach (var folderStructuresContainObject in folderStructuresContainsObject)
                            {
                                if (folderStructureId.Equals(folderStructuresContainObject))
                                    continue;

                                _structuredSiteTreeViews.RemoveFolderStructureObject(
                                    folderStructuresContainObject,
                                    dragObject.ObjectInfo);
                            }
                        }
                        else if (e.Effect == DragDropEffects.Move)
                        {
                            _structuredSiteTreeViews.RemoveFolderStructureObject(
                                (IdAndObjectType) dragObject.SourceData,
                                dragObject.ObjectInfo);
                        }

                        if (!folderStructuresContainsObject.Contains(folderStructureId))
                            _structuredSiteTreeViews.AddFolderStructureObject(
                                folderStructureId,
                                dragObject.ObjectInfo);
                    }

                    EndUpdate();

                    WasChanged();
                    return;
                }

                bool dragDropNodeIsReference;
                var dragDropSiteNode =
                    _structuredSiteTreeViews.GetSubSiteNode(
                        dragDropNode,
                        out dragDropNodeIsReference);

                _structuredSiteTreeViews.ExpandSiteNode(dragDropSiteNode);

                BeginUpdate();
                bool wasChanged = false;

                try
                {
                    foreach (var dragObject in dragObjects)
                    {
                        if (dragDropNodeIsReference ||
                            dragObject.ObjectInfo.ObjectType == ObjectType.Login ||
                            dragObject.ObjectInfo.ObjectType == ObjectType.LoginGroup)
                        {
                            StructuredSiteNode parentNode;
                            if (dragObject.ObjectInfo.ObjectType == ObjectType.Login ||
                                dragObject.ObjectInfo.ObjectType == ObjectType.LoginGroup)
                            {
                                parentNode = dragDropSiteNode;
                            }
                            else
                            {
                                parentNode = _structuredSiteTreeViews.GetSubTreeReferencingFolder(dragDropSiteNode);
                            }

                            if (_structuredSiteTreeViews.ObjectExistInTheSubSite(dragObject.ObjectInfo, parentNode))
                            {
                                switch (dragObject.ObjectInfo.ObjectType)
                                {
                                    case ObjectType.Login:
                                        Dialog.Error(
                                            CgpClient.Singleton.LocalizationHelper.GetString(
                                                "ErrorLoginAlreadAddedToSubSite"));
                                        break;
                                    case ObjectType.LoginGroup:
                                        Dialog.Error(
                                            CgpClient.Singleton.LocalizationHelper.GetString(
                                                "ErrorLoginGroupAlreadAddedToSubSite"));
                                        break;
                                    default:
                                        Dialog.Error(
                                            CgpClient.Singleton.LocalizationHelper.GetString(
                                                "ErrorReferenceAlreadExistInSubSite"));
                                        break;
                                }

                                return;
                            }

                            int? siteIdForRemove;
                            if (e.Effect == DragDropEffects.Move)
                            {
                                siteIdForRemove = dragObject.SiteId;
                            }
                            else
                            {
                                siteIdForRemove = null;
                            }

                            SafeThread<IdAndObjectType, int?, int?>.StartThread(
                                AddRemoveSubTreeReference,
                                dragObject.ObjectInfo,
                                siteIdForRemove,
                                dragDropSiteNode != null
                                    ? (int?) ((SiteNodeInfo) dragDropSiteNode.Tag).SiteId
                                    : -1);

                            wasChanged = true;

                            continue;
                        }

                        if (dragObject.Source == DragObjectSource.FolderStructure)
                        {
                            int objectSiteId;
                            try
                            {
                                objectSiteId = _structuredSiteBuilder.GetSiteOfObject(dragObject.ObjectInfo);
                            }
                            catch
                            {
                                CgpClient.Singleton.IsConnectionLost(false);
                                return;
                            }

                            if (objectSiteId != -1)
                            {
                                Dialog.Error(
                                    CgpClient.Singleton.LocalizationHelper.GetString(
                                        "ErrorObjectAlreadExistInSubSite"));

                                return;
                            }
                        }

                        if (dragObject.ObjectInfo.ObjectType == ObjectType.Person)
                        {
                            _structuredSiteTreeViews.RemoveFolderStructureObject(dragObject.SiteId,
                                dragObject.ObjectInfo);
                        }

                        SafeThread<IdAndObjectType, int>.StartThread(
                            MoveObject,
                            dragObject.ObjectInfo,
                            dragDropSiteNode != null ? ((SiteNodeInfo) dragDropSiteNode.Tag).SiteId : -1);

                        wasChanged = true;
                    }
                }
                finally
                {
                    EndUpdate();

                    if (wasChanged)
                        WasChanged();
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddRemoveSubTreeReference(IdAndObjectType objectId, int? siteIdForRemove, int? siteIdForAdd)
        {
            try
            {
                if (siteIdForAdd != null)
                {
                    _structuredSiteBuilder.AddSubTreeReference(
                        objectId,
                        siteIdForAdd.Value);
                }

                if (siteIdForRemove != null)
                {
                    _structuredSiteBuilder.RemoveSubTreeReference(
                        objectId,
                        siteIdForRemove.Value);
                }
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        private void MoveObject(IdAndObjectType objectId, int newSiteId)
        {
            try
            {
                _structuredSiteBuilder.MoveObject(
                    objectId,
                    newSiteId);
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        private void AddObjectToFolder(IdAndObjectType objectId, IdAndObjectType folderId)
        {
            try
            {
                _structuredSiteBuilder.AddObjectToFolder(
                    new IdAndObjectType(
                        objectId.Id.ToString(),
                        objectId.ObjectType),
                    folderId);
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        private void RemoveObjectFromFolder(IdAndObjectType objectId, IdAndObjectType folderId)
        {
            try
            {
                _structuredSiteBuilder.RemoveObjectFromFolder(
                    new IdAndObjectType(
                        objectId.Id.ToString(),
                        objectId.ObjectType),
                    folderId);
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        private void SetFocusedTreeView(TreeNode node)
        {
            _structuredSiteTreeViews.SetActualTreeView(GetTreeViewForNode(node));
        }

        private TreeViewWithFilter GetTreeViewForNode(TreeNode node)
        {
            if (node == null || node.TreeView == _tvStructuredSiteLeft)
            {
                return _tvStructuredSiteLeft;
            }

            return _tvStructuredSiteRight;
        }

        private void _bCreateSubSite_Click(object sender, EventArgs e)
        {
            var siteNode = _structuredSiteTreeViews.GetSubSiteNode(
                _structuredSiteTreeViews.ActualTreeView.SelectedNode as StructuredSiteNode);

            int siteId;
            if (siteNode != null)
            {
                siteId = ((SiteNodeInfo)siteNode.Tag).SiteId;
            }
            else
            {
                siteId = -1;
            }

            SafeThread<int, string>.StartThread(
                CreateSubSite,
                siteId,
                GetString("StructuredSiteForm_SubsiteName"));

            WasChanged();
        }

        private void CreateSubSite(int parentSiteId, string name)
        {
            try
            {
                _structuredSiteBuilder.CreateSubSite(parentSiteId, name);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        protected override StructuredSubSite GetObjectForEdit(StructuredSubSite obj, out bool editAllowed)
        {
            throw new NotImplementedException();
        }

        protected override StructuredSubSite GetFromShort(StructuredSubSite obj)
        {
            throw new NotImplementedException();
        }

        protected override StructuredSubSite GetById(object idObj)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteObj(StructuredSubSite obj)
        {
            throw new NotImplementedException();
        }

        protected override void DeleteObjById(object idObj)
        {
            throw new NotImplementedException();
        }

        protected override bool Compare(StructuredSubSite obj1, StructuredSubSite obj2)
        {
            throw new NotImplementedException();
        }

        protected override ACgpEditForm<StructuredSubSite> CreateEditForm(StructuredSubSite obj, ShowOptionsEditForm showOption)
        {
            throw new NotImplementedException();
        }

        protected override ICollection<StructuredSubSite> GetData()
        {
            if (_structuredSiteBuilder == null)
            {
                _structuredSiteBuilder =
                    CgpClient.Singleton.MainServerProvider.StructuredSubSites.GetBuilder(
                        _structuredSiteBuilderClient);
            }

            _hasAccessUpdate = CgpClient.Singleton.MainServerProvider.StructuredSubSites.HasAccessUpdate();

            #if !DEBUG

            if (_hasAccessUpdate)
            {
                string localisedName;
                object licenseValue;

                if (!CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(
                    Cgp.Globals.RequiredLicenceProperties.MaxSubsiteCount.ToString(), out localisedName,
                    out licenseValue) ||
                    (int) licenseValue <= 0)
                {
                    _hasAccessUpdate = false;
                }
            }

            #endif

            _hasAccessViewFolderStructure =
                CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessView();

            _hasAccessInsertFolderStructure =
                CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessInsert();

            _hasAccessUpdateFolderStructure =
                CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessUpdate();

            _hasAccessDeleteFolderStructure =
                CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessDelete();

            if (_reloadStructuredTreeViews)
            {
                _reloadStructuredTreeViews = false;
                _subSitesForLogin.Clear();
                ClearForm();

                var specialReadingOfChilds = new SpecialReadingOfChildObjects();
                specialReadingOfChilds.Add(new LgAdminSpecialReadingOfChildObjectObjects());

                _structuredSiteTreeViews.Init(
                    new LinkedList<SiteIdAndName>(_structuredSiteBuilder.GetTopMostSites()),
                    _subSitesForLogin,
                    specialReadingOfChilds);

                ShowBrokenReferences(_structuredSiteBuilder.GetObjectsWithBrokenDirectReferences());

                if (_showNotificationStructuredWasChanged)
                {
                    _showNotificationStructuredWasChanged = false;

                    ControlNotification.Singleton.Warning(
                        NotificationPriority.JustOne,
                        _lNotification,
                        CgpClient.Singleton.LocalizationHelper.GetString("InfoStructuredSiteStructureWasChanged"),
                        ControlNotificationSettings.Default);
                }
            }

            CheckAccess();

            if (_selectedObjectPlacement != null && _selectedObject != null)
            {
                if (_selectedObjectPlacement.Parent != null)
                    _structuredSiteTreeViews.SelectObject(
                        _selectedObjectPlacement.SiteId,
                        _selectedObjectPlacement.Parent,
                        _selectedObject);
                else
                    _structuredSiteTreeViews.SelectObject(
                        _selectedObjectPlacement.SiteId,
                        _selectedObject,
                        _selectedObjectPlacement.IsReference);
            }

            _selectedObjectPlacement = null;
            _selectedObject = null;

            return new LinkedList<StructuredSubSite>();
        }

        private void ClearForm()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(ClearForm));
            }
            else
            {
                _structuredSiteTreeViews.Clear();
                _tvBrokenReferences.Nodes.Clear();
                _bApply.Enabled = false;
            }
        }

        public void SiteCreated(SiteIdAndName newSiteIdAndName, int parentSiteId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<SiteIdAndName, int>(SiteCreated), newSiteIdAndName, parentSiteId);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.SubSiteCreated(newSiteIdAndName, parentSiteId);
            }
        }

        public void SiteRenamed(SiteIdAndName siteIdAndName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<SiteIdAndName>(SiteRenamed), siteIdAndName);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.SiteRenamed(siteIdAndName);
            }
        }

        public void SiteDeleted(int siteId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(SiteDeleted), siteId);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.SiteDeleted(siteId);
            }
        }

        public void ObjectAdded(ObjectInSiteInfo objectInfo, int siteId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ObjectInSiteInfo, int>(ObjectAdded), objectInfo, siteId);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.ObjectAdded(objectInfo, siteId);
            }
        }

        public void ObjectRenamed(IdAndObjectType objectId, string newName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IdAndObjectType, string>(ObjectRenamed), objectId, newName);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.ObjectRenamed(objectId, newName);
            }
        }

        public void ObjectRemoved(IdAndObjectType objectId, int siteId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IdAndObjectType, int>(ObjectRemoved), objectId, siteId);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.ObjectRemoved(objectId, siteId);
            }
        }

        public void SubTreeReferenceAdded(ReferenceInSiteInfo referecneInfo, int siteId)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ReferenceInSiteInfo, int>(SubTreeReferenceAdded), referecneInfo, siteId);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.SubTreeReferenceAdded(referecneInfo, siteId);
            }
        }

        public void SubTreeReferenceRemoved(IdAndObjectType referenceId, int siteId, bool isReferenceDefinedInParent)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<IdAndObjectType, int, bool>(SubTreeReferenceRemoved),
                    referenceId,
                    siteId,
                    isReferenceDefinedInParent);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.SubTreeReferenceRemoved(referenceId, siteId, isReferenceDefinedInParent);
            }
        }

        public void ObjectParentChanged(IdAndObjectType idAndObjectType, IdAndObjectType newParent,
            IdAndObjectType oldParent)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<IdAndObjectType, IdAndObjectType, IdAndObjectType>(ObjectParentChanged),
                    idAndObjectType,
                    newParent,
                    oldParent);
            }
            else
            {
                if (_structuredSiteTreeViews == null)
                    return;

                _structuredSiteTreeViews.ObjectParentChanged(idAndObjectType, newParent, oldParent);
            }
        }

        public void StructureChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(StructureChanged));
            }
            else
            {
                _reloadStructuredTreeViews = true;
                _showNotificationStructuredWasChanged = true;

                if (ReferenceEquals(CgpClientMainForm.Singleton.FocusedWindow, this))
                {
                    ShowData();
                }
            }
        }

        public void TopMostSitesChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(TopMostSitesChanged));
            }
            else
            {
                _reloadStructuredTreeViews = true;

                ShowData();
            }
        }

        private void ShowBrokenReferences(IEnumerable<BrokenReferencesInfo> objectsWithBrokenReferences)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<IEnumerable<BrokenReferencesInfo>>(
                        ShowBrokenReferences),
                    objectsWithBrokenReferences);
            }
            else
            {
                if (objectsWithBrokenReferences == null)
                {
                    ShowNoObjectsWithBrokenReferences();
                    return;
                }

                _tvBrokenReferences.BeginUpdate();

                foreach (var kvpObjectWithBrokenReferences in objectsWithBrokenReferences)
                {
                    AddObjectWithBrokenReferences(kvpObjectWithBrokenReferences, false);
                }

                _tvBrokenReferences.Sort();
                _tvBrokenReferences.EndUpdate();
            }
        }

        private void ShowNoObjectsWithBrokenReferences()
        {
            if (_showedNoObjectsWithBrokenReferences == null || !_showedNoObjectsWithBrokenReferences.Value)
            {
                _lBrokenReferences.Text = GetString("InfoNoBrokenReferences");
                _lBrokenReferences.ForeColor = Color.Green;

                _scStructuredSiteBrokenReferences.SplitterDistance = _scStructuredSiteBrokenReferences.Width;
                
                _showedNoObjectsWithBrokenReferences = true;
            }
        }

        private void ClearShowNoObjectsWithBrokenReferences()
        {
            if (_showedNoObjectsWithBrokenReferences == null || _showedNoObjectsWithBrokenReferences.Value)
            {
                _lBrokenReferences.Text = GetString("StructuredSiteForm_lBrokenReferences");
                _lBrokenReferences.ForeColor = Color.Red;

                _scStructuredSiteBrokenReferences.SplitterDistance = _scStructuredSiteBrokenReferences.Width -
                                                                     (_scStructuredSiteBrokenReferences.Width/4);
                
                _showedNoObjectsWithBrokenReferences = false;
            }
        }

        private void AddObjectWithBrokenReferences(BrokenReferencesInfo brokenReferencesInfo, bool removeIfExist)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<BrokenReferencesInfo, bool>(
                        AddObjectWithBrokenReferences),
                    brokenReferencesInfo,
                    removeIfExist);
            }
            else
            {


                if (removeIfExist)
                {
                    var nodeToRemove = _tvBrokenReferences.Nodes
                        .Cast<TreeNode>()
                        .FirstOrDefault(
                            node =>
                                brokenReferencesInfo.Equals(node.Tag));

                    if (nodeToRemove != null)
                    {
                        _tvBrokenReferences.Nodes.Remove(nodeToRemove);
                    }
                }

                if (brokenReferencesInfo.BrokenReferences == null)
                {
                    if (_tvBrokenReferences.Nodes.Count == 0)
                        ShowNoObjectsWithBrokenReferences();

                    return;
                }

                ClearShowNoObjectsWithBrokenReferences();

                var objectNode = new TreeNode(brokenReferencesInfo.Name)
                {
                    Tag = brokenReferencesInfo
                };

                var imageKey = brokenReferencesInfo.ObjectType.ToString();
                objectNode.ImageKey = imageKey;
                objectNode.SelectedImageKey = imageKey;
                objectNode.StateImageKey = imageKey;

                foreach (var brokenReference in brokenReferencesInfo.BrokenReferences)
                {
                    var brokenReferenceNode = new TreeNode(brokenReference.Name)
                    {
                        Tag = brokenReference
                    };

                    var imageKeyBrokenReference = brokenReference.ObjectType.ToString();
                    brokenReferenceNode.ImageKey = imageKeyBrokenReference;
                    brokenReferenceNode.SelectedImageKey = imageKeyBrokenReference;
                    brokenReferenceNode.StateImageKey = imageKeyBrokenReference;

                    objectNode.Nodes.Add(brokenReferenceNode);
                }

                _tvBrokenReferences.Nodes.Add(objectNode);
            }
        }

        public void BrokenDirectReferencesForObjectChanged(IEnumerable<BrokenReferencesInfo> objectsWithBrokenReferences)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<IEnumerable<BrokenReferencesInfo>>(
                        BrokenDirectReferencesForObjectChanged),
                    objectsWithBrokenReferences);
            }
            else
            {
                _tvBrokenReferences.BeginUpdate();

                foreach (var kvpObjectsWithBrokenReference in objectsWithBrokenReferences)
                {
                    AddObjectWithBrokenReferences(kvpObjectsWithBrokenReference, true);
                }

                _tvBrokenReferences.Sort();
                _tvBrokenReferences.EndUpdate();
            }
        }

        private bool HasAccessViewFolderStructure()
        {
            return _hasAccessViewFolderStructure;
        }

        private ICollection<ObjectInSiteInfo> LoadFolderStructureObjects(object folderStructureId)
        {
            try
            {
                return
                    CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.LoadFolderStructureObjects(
                        folderStructureId);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
                return null;
            }
        }

        private IEnumerable<SiteIdAndName> LoadSubSites(int siteId)
        {
            try
            {
                return _structuredSiteBuilder.GetSubSites(siteId);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
                return null;
            }
            
        }

        private IEnumerable<ObjectInSiteInfo> LoadObjectsInSite(int siteId)
        {
            try
            {
                return _structuredSiteBuilder.GetObjectsInSite(siteId);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
                return null;
            }
        }

        private IEnumerable<ReferenceInSiteInfo> LoadSubTreeReferences(int siteId)
        {
            try
            {
                if (_subSitesForLogin.Contains(siteId))
                {
                    return _structuredSiteBuilder.GetAllSubTreeReferences(siteId);
                }

                var subTreeReferencesFromServer = _structuredSiteBuilder.GetSubTreeReferences(siteId);
                if (subTreeReferencesFromServer == null)
                    return null;

                var subTreeReferences = new LinkedList<ReferenceInSiteInfo>();
                foreach (var subTreeReferenceFromServer in subTreeReferencesFromServer)
                {
                    subTreeReferences.AddLast(new ReferenceInSiteInfo(
                        subTreeReferenceFromServer.Id,
                        subTreeReferenceFromServer.ObjectType,
                        subTreeReferenceFromServer.Name,
                        subTreeReferenceFromServer.Parent,
                        true));
                }

                return subTreeReferences;
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
                return null;
            }
        }

        private void AddObjectToFolderStructure(IdAndObjectType objectId, IdAndObjectType folderStructureId)
        {
            SafeThread<IdAndObjectType, int>.StartThread(
                AddObjectToFolder,
                objectId,
                folderStructureId);
        }

        private void RemoveObjectFromFolderStructure(IdAndObjectType objectId, IdAndObjectType folderStructureId)
        {
            SafeThread<IdAndObjectType, int>.StartThread(
                RemoveObjectFromFolder,
                objectId,
                folderStructureId);
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
        }

        protected override void RemoveGridView()
        {
        }

        private void CheckAccess()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(CheckAccess));
            else
            {
                if (!_hasAccessUpdate)
                {
                    _bCreateSubSite.Enabled = false;
                    _bRenameSubSite.Enabled = false;
                    _bDelete.Enabled = false;
                }

                if (!_hasAccessInsertFolderStructure)
                    _bCreateFolder.Enabled = false;

                if (!_hasAccessUpdateFolderStructure)
                    _bRenameFolder.Enabled = false;

                if (!_hasAccessDeleteFolderStructure)
                    _bDelete.Enabled = false;
            }
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.StructuredSubSites.HasAccessView() ||
                           CgpClient.Singleton.MainServerProvider.UserFoldersSutructures.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.StructuredSubSites.HasAccessInsert();

                return false;
            }
            catch
            {
                return false;
            }
        }

        protected override void SetFilterSettings()
        {
            throw new NotImplementedException();
        }

        protected override void ClearFilterEdits()
        {
            throw new NotImplementedException();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            var selectedNodes = _structuredSiteTreeViews.ActualTreeView.SelectedNodes;

            foreach (var selectedNode in selectedNodes)
            {
                var structuredSiteNode = selectedNode as StructuredSiteNode;
                
                if (structuredSiteNode == null)
                    continue;

                WasChanged();

                switch (structuredSiteNode.StructuredSiteNodeType)
                {
                    case StructuredSiteNodeType.SubSiteNode:
                        SafeThread<int>.StartThread(
                            DeleteSite,
                            ((SiteNodeInfo) structuredSiteNode.Tag).SiteId);

                        break;
                    case StructuredSiteNodeType.FolderStructureNode:
                        SafeThread<IdAndObjectType>.StartThread(
                            RemoveFolder,
                            (IdAndObjectType) structuredSiteNode.Tag);

                        break;
                    case StructuredSiteNodeType.OrmObjectNode:
                        var folderStructureNode = _structuredSiteTreeViews.GetFolderStructureNode(structuredSiteNode);

                        if (folderStructureNode != null)
                        {
                            var objectId = (IdAndObjectType) structuredSiteNode.Tag;
                            var folderStructureId = (IdAndObjectType) folderStructureNode.Tag;

                            BeginUpdate();

                            _structuredSiteTreeViews.RemoveFolderStructureObject(
                                folderStructureId,
                                objectId);

                            EndUpdate();

                            break;
                        }

                        var subSiteNode = _structuredSiteTreeViews.GetSubSiteNode(structuredSiteNode);

                        SafeThread<IdAndObjectType, int?, int?>.StartThread(
                            AddRemoveSubTreeReference,
                            (IdAndObjectType) structuredSiteNode.Tag,
                            subSiteNode != null ? (int?) ((SiteNodeInfo) subSiteNode.Tag).SiteId : -1,
                            (int?) null);

                        break;
                }
            }

            _bDelete.Enabled = false;
        }

        private void DeleteSite(int siteId)
        {
            try
            {
                _structuredSiteBuilder.DeleteSite(siteId);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }

            if (_structuredSiteTreeViews != null)
            {
                _structuredSiteTreeViews.Clear();
                _reloadStructuredTreeViews = true;
            }

            if (_structuredSiteBuilder != null)
            {
                try
                {
                    _structuredSiteBuilder.Cancel();
                }
                catch(Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                _structuredSiteBuilder = null;
            }

            _subSitesForLogin.Clear();
            _tvBrokenReferences.Nodes.Clear();

            _bApply.Enabled = false;
            base.OnFormClosing(e);
        }

        private void _bRenameSubSite_Click(object sender, EventArgs e)
        {
            _structuredSiteTreeViews.ActualTreeView.BeginEditNode(_structuredSiteTreeViews.ActualTreeView.SelectedNodes.First());
            WasChanged();
        }

        public void BeginUpdate()
        {
            _structuredSiteTreeViews.BeginUpdate();
        }

        public void EndUpdate()
        {
            _structuredSiteTreeViews.EndUpdate();
        }

        private void WasChanged()
        {
            _bApply.Enabled = true;
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            try
            {
                _structuredSiteBuilder.Save();
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
                return;
            }

            _bApply.Enabled = false;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (_bApply.Enabled)
            {
                try
                {
                    _structuredSiteBuilder.Save();
                }
                catch
                {
                    CgpClient.Singleton.IsConnectionLost(false);
                    return;
                }
            }

            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private bool Cancel_Click()
        {
            if (_bApply.Enabled)
            {
                DialogResult result =
                    MessageBox.Show(
                        string.Format(
                            @"{0}\n{1}",
                            GetString("ValueChanged"),
                            GetString("SaveAfterCancel")),
                        GetString("Question"),
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Cancel:
                    {
                        return false;
                    }
                    case DialogResult.No:
                    {
                        Close();
                        return true;
                    }
                    case DialogResult.Yes:
                    {
                        _bOk_Click(null, null);
                        return true;
                    }
                }
            }

            Close();
            return true;
        }

        public override void CallEscape()
        {
            if (MdiParent != null)
            {
                Cancel_Click();
            }
        }

        public bool SaveAfterCancel()
        {
            return Cancel_Click();
        }

        private void _tvBrokenReferences_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var objectId = (IdAndObjectType) e.Node.Tag;
            int siteId;
            try
            {
                siteId = _structuredSiteBuilder.GetSiteOfObject(objectId);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
                return;
            }

            _structuredSiteTreeViews.SelectObject(siteId, objectId, false);
        }

        private void _tvBrokenReferences_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (!_hasAccessUpdate)
                return;

            var node = e.Item as TreeNode;
            if (node == null)
                return;

            var objectInfo = node.Tag as ObjectInSiteInfo;

            int siteId;
            try
            {
                siteId = _structuredSiteBuilder.GetSiteOfObject(objectInfo);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
                return;
            }

            if (!_structuredSiteTreeViews.HasAccessForSite(siteId))
                return;

            var dragObjects = new LinkedList<DragObject>(
                Enumerable.Repeat(
                    new DragObject(
                        objectInfo,
                        false,
                        DragObjectSource.BrokenReferences,
                        null,
                        -1),
                    1));

            DoDragDrop(dragObjects, DragDropEffects.Copy);
        }

        private void _tvBrokenReferences_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (_tvBrokenReferences.PointToClient(Cursor.Position).X >= e.Node.Bounds.Left)
                e.Cancel = true;
        }

        private void _tvBrokenReferences_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (_tvBrokenReferences.PointToClient(Cursor.Position).X >= e.Node.Bounds.Left)
                e.Cancel = true;
        }

        private void _bCreateFolder_Click(object sender, EventArgs e)
        {
            string folderName;

            var form = new CreateFolderForm();
            DialogResult result = form.ShowDialog(out folderName);

            if (result == DialogResult.OK)
            {
                WasChanged();

                var actualNode = _structuredSiteTreeViews.ActualTreeView.SelectedNode as StructuredSiteNode;
                if (actualNode == null)
                    return;

                IdAndObjectType parentFoldeObject = null;
                if (actualNode.StructuredSiteNodeType == StructuredSiteNodeType.FolderStructureNode)
                {
                    parentFoldeObject = (IdAndObjectType) actualNode.Tag;
                }

                var siteNode = _structuredSiteTreeViews.GetSubSiteNode(
                    _structuredSiteTreeViews.ActualTreeView.SelectedNode as StructuredSiteNode);

                int siteId;
                if (siteNode != null)
                {
                    siteId = ((SiteNodeInfo)siteNode.Tag).SiteId;
                }
                else
                {
                    siteId = -1;
                }

                SafeThread<int, IdAndObjectType, string>.StartThread(
                    CreateFolder,
                    siteId,
                    parentFoldeObject,
                    folderName);
            }
        }

        private void CreateFolder(int siteId, IdAndObjectType parentFoldeObject, string folderName)
        {
            try
            {
                _structuredSiteBuilder.CreateFolder(siteId, parentFoldeObject, folderName);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        private void _bRenameFolder_Click(object sender, EventArgs e)
        {
            _structuredSiteTreeViews.ActualTreeView.BeginEditNode(_structuredSiteTreeViews.ActualTreeView.SelectedNode);
            WasChanged();
        }

        private void RenameFolder(IdAndObjectType folderId, string newName)
        {
            try
            {
                _structuredSiteBuilder.RenameFolder(folderId, newName);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }

        private void RemoveFolder(IdAndObjectType folderId)
        {
            try
            {
                _structuredSiteBuilder.RemoveFolder(folderId);
            }
            catch
            {
                CgpClient.Singleton.IsConnectionLost(false);
            }
        }
    }

    public class StructuredSiteBuilderClient : MarshalByRefObject, IStructuredSiteBuilderClient
    {
        private readonly StructuredSiteForm _structuredSiteForm;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public StructuredSiteBuilderClient(StructuredSiteForm structuredSiteForm)
        {
            _structuredSiteForm = structuredSiteForm;
        }

        #region IStructuredSiteBuilderClient Members

        public void StructureChanged()
        {
            _structuredSiteForm.StructureChanged();
        }

        public void TopMostSitesChanged()
        {
            _structuredSiteForm.TopMostSitesChanged();
        }

        public void SiteCreated(SiteIdAndName siteIdAndName, int parentSiteId)
        {
            _structuredSiteForm.SiteCreated(siteIdAndName, parentSiteId);
        }

        public void SiteRenamed(SiteIdAndName siteIdAndName)
        {
            _structuredSiteForm.SiteRenamed(siteIdAndName);
        }

        public void SiteDeleted(int siteId)
        {
            _structuredSiteForm.SiteDeleted(siteId);
        }

        public void ObjectAdded(ObjectInSiteInfo objectInSiteInfo, int siteId)
        {
            _structuredSiteForm.ObjectAdded(objectInSiteInfo, siteId);
        }

        public void ObjectRenamed(IdAndObjectType idAndObjectType, string newName)
        {
            _structuredSiteForm.ObjectRenamed(idAndObjectType, newName);
        }

        public void ObjectRemoved(IdAndObjectType idAndObjectType, int siteId)
        {
            _structuredSiteForm.ObjectRemoved(idAndObjectType, siteId);
        }

        public void SubTreeReferenceAdded(ReferenceInSiteInfo referenceInSiteInfo, int siteId)
        {
            _structuredSiteForm.SubTreeReferenceAdded(referenceInSiteInfo, siteId);
        }

        public void SubTreeReferenceRemoved(IdAndObjectType idAndObjectType, int siteId, bool isReferenceDefinedInParent)
        {
            _structuredSiteForm.SubTreeReferenceRemoved(idAndObjectType, siteId, isReferenceDefinedInParent);
        }

        public void ObjectParentChanged(IdAndObjectType idAndObjectType, IdAndObjectType newParent,
            IdAndObjectType oldParent)
        {
            _structuredSiteForm.ObjectParentChanged(idAndObjectType, newParent, oldParent);
        }

        public void BrokenDirectReferencesForObjectsChanged(IEnumerable<BrokenReferencesInfo> objectsWithBrokenReferences)
        {
            _structuredSiteForm.BrokenDirectReferencesForObjectChanged(objectsWithBrokenReferences);
        }

        public void BeginUpdate()
        {
            _structuredSiteForm.BeginUpdate();
        }

        public void EndUpdate()
        {
            _structuredSiteForm.EndUpdate();
        }

        #endregion
    }
}
