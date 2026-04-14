using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    partial class LocalEvaluator
    {
        public class ClientConnector : MarshalByRefObject, IStructuredSiteBuilder
        {
            private readonly LocalEvaluator _localEvaluator;
            private readonly IStructuredSiteBuilderClient _structuredSiteBuilderClient;

            private readonly ProcessingQueue<DVoid2Void> _notificationToClientQueue =
                new ProcessingQueue<DVoid2Void>();

            int IStructuredSiteBuilder.GetSiteOfObject(IdAndObjectType objectId)
            {
                var objectInfo = _localEvaluator.GetObjectInfo(objectId);

                return
                    objectInfo != null
                        ? objectInfo.SiteInfo.Id
                        : -1;
            }

            void IStructuredSiteBuilder.Save()
            {
                _localEvaluator.Save();
            }

            void IStructuredSiteBuilder.Cancel()
            {
                _localEvaluator.Cancel();
            }

            IEnumerable<ObjectInSiteInfo> IStructuredSiteBuilder.GetSubTreeReferences(int siteId)
            {
                var siteInfo = _localEvaluator.GetSiteInfo(siteId);

                if (siteInfo == null)
                    return null;

                return
                    new LinkedList<ObjectInSiteInfo>(
                        siteInfo.SubTreeReferences
                            .Select(idAndObjectType => 
                                CreateObjectInSiteInfo(
                                    _localEvaluator.GetObjectInfo(idAndObjectType)))
                            .Where(objectInSiteInfo => objectInSiteInfo != null));
            }

            IEnumerable<ReferenceInSiteInfo> IStructuredSiteBuilder.GetAllSubTreeReferences(int siteId)
            {
                var siteInfo = _localEvaluator.GetSiteInfo(siteId);

                if (siteInfo == null)
                    return null;

                ICollection<ReferenceInSiteInfo> result =
                    new LinkedList<ReferenceInSiteInfo>();

                var subTreeReferences = new HashSet<IdAndObjectType>();

                foreach (var subTreeReference in siteInfo.SubTreeReferences)
                    if (subTreeReferences.Add(subTreeReference))
                        result.Add(
                            _localEvaluator.CreateReferenceInSiteInfo(
                                subTreeReference,
                                true));

                siteInfo = siteInfo.ParentSite;

                while (siteInfo != null)
                {
                    foreach (var subTreeReference in siteInfo.SubTreeReferences)
                        if (subTreeReferences.Add(subTreeReference))
                            result.Add(
                                _localEvaluator.CreateReferenceInSiteInfo(
                                    subTreeReference,
                                    false));

                    siteInfo = siteInfo.ParentSite;
                }

                return result;
            }

            IEnumerable<ObjectInSiteInfo> IStructuredSiteBuilder.GetObjectsInSite(int siteId)
            {
                LocalSiteInfo siteInfo = _localEvaluator.GetSiteInfo(siteId);

                return
                    new LinkedList<ObjectInSiteInfo>(
                        siteInfo.Objects
                            .Where(_localEvaluator.ObjectPlacementVisible)
                            .Select(idAndObjectType => CreateObjectInSiteInfo(_localEvaluator.GetObjectInfo(idAndObjectType)))
                            .Where(objectInSiteInfo => objectInSiteInfo != null));
            }

            int IStructuredSiteBuilder.GetSubTreeReferenceCount(IdAndObjectType idAndObjectType)
            {
                var objectInfo = _localEvaluator.GetObjectInfo(idAndObjectType);

                return objectInfo != null
                    ? objectInfo.SubTreeReferenceCount
                    : 0;
            }

            void IStructuredSiteBuilder.RemoveSubTreeReference(IdAndObjectType objectId, int siteId)
            {
                var siteInfo = _localEvaluator.GetSiteInfo(siteId);

                if (siteInfo == null)
                    return;

                var objectInfo = _localEvaluator.GetObjectInfo(objectId);

                if (objectInfo == null)
                    return;

                _localEvaluator.PerformModificationAndNotifyClient(
                    _localEvaluator.RemoveSubTreeReferenceInternal,
                    objectInfo,
                    siteInfo);
            }

            IEnumerable<SiteIdAndName> IStructuredSiteBuilder.GetTopMostSites()
            {
                return
                    new LinkedList<SiteIdAndName>(
                        _localEvaluator._topMostSites.Select(
                            siteInfo =>
                                new SiteIdAndName(
                                    siteInfo.Id,
                                    siteInfo.Name)));
            }

            public void NotifySubTreeReferenceRemoved(
                IdAndObjectType idAndObjectType,
                int siteId,
                bool ancestralReferencesExist)
            {
                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.SubTreeReferenceRemoved(
                        idAndObjectType,
                        siteId,
                        ancestralReferencesExist));
            }

            public void NotifySubTreeReferenceAdded(
                ReferenceInSiteInfo referenceInSiteInfo,
                int siteId)
            {
                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.SubTreeReferenceAdded(
                        referenceInSiteInfo,
                        siteId));
            }

            void IStructuredSiteBuilder.DeleteSite(int siteId)
            {
                var siteInfo = _localEvaluator.GetSiteInfo(siteId);

                if (siteInfo == null)
                    return;

                _localEvaluator.PerformModificationAndNotifyClient(
                    _localEvaluator.DeleteSiteInternal,
                    siteInfo,
                    siteInfo.ParentSite);
            }

            int IStructuredSiteBuilder.CreateSubSite(int parentSiteId, string name)
            {
                var siteInfo =
                    _localEvaluator.PerformModificationAndNotifyClient<LocalSiteInfo, int, string>(
                        _localEvaluator.CreateSiteInfoInternal,
                        parentSiteId,
                        name);

                return
                    siteInfo != null
                        ? siteInfo.Id
                        : -1;
            }

            void IStructuredSiteBuilder.RenameSite(int siteId, string newName)
            {
                var site = _localEvaluator.GetSiteInfo(siteId);

                if (site == null)
                    return;

                _localEvaluator.RenameSiteInternal(newName, site);
            }

            IEnumerable<BrokenReferencesInfo> IStructuredSiteBuilder.GetObjectsWithBrokenDirectReferences()
            {
                var objectsWithBrokenDirectReferences =
                    new LinkedList<BrokenReferencesInfo>(
                        _localEvaluator._objectsWithBrokenDirectReferences.Elements
                            .Select(idAndObjectType => CreateBrokenReferencesInfo(idAndObjectType))
                            .Where(brokenReferencesInfo => brokenReferencesInfo != null));

                return
                    objectsWithBrokenDirectReferences.Count > 0
                        ? objectsWithBrokenDirectReferences
                        : null;
            }

            void IStructuredSiteBuilder.MoveObject(
                IdAndObjectType idAndObjectType,
                int newSiteId)
            {
                var objectInfo = _localEvaluator.GetObjectInfo(idAndObjectType);

                if (objectInfo == null || objectInfo.Parent != null)
                    return;

                var newSiteInfo = _localEvaluator.GetSiteInfo(newSiteId);

                if (newSiteInfo == null)
                    return;

                _localEvaluator.PerformModificationAndNotifyClient(
                    _localEvaluator.MoveObject,
                    objectInfo,
                    newSiteInfo);
            }

            void IStructuredSiteBuilder.AddSubTreeReference(IdAndObjectType objectId, int siteId)
            {
                var siteInfo = _localEvaluator.GetSiteInfo(siteId);

                if (siteInfo == null)
                    return;

                var objectInfo = _localEvaluator.GetObjectInfo(objectId);

                if (objectInfo == null)
                    return;

                _localEvaluator.PerformModificationAndNotifyClient(
                    _localEvaluator.AddSubTreeReferenceInternal,
                    objectInfo,
                    siteInfo);
            }

            IEnumerable<SiteIdAndName> IStructuredSiteBuilder.GetSubSites(int siteId)
            {
                LocalSiteInfo siteInfo = _localEvaluator.GetSiteInfo(siteId);

                if (siteInfo == null)
                    return null;

                return
                    new LinkedList<SiteIdAndName>(
                        siteInfo.ChildrenSites.Select(
                            child => new SiteIdAndName(
                                child.Id,
                                child.Name)));
            }

            public void NotifyObjectAdded(
                ObjectInSiteInfo objectInSiteInfo,
                int newSiteId)
            {
                if (objectInSiteInfo == null)
                    return;

                if (!_localEvaluator.ObjectPlacementVisible(objectInSiteInfo))
                    return;

                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.ObjectAdded(
                        objectInSiteInfo,
                        newSiteId));
            }

            public void NotifyObjectRemoved(
                IdAndObjectType idAndObjectType,
                int oldSiteId)
            {
                if (idAndObjectType.ObjectType == ObjectType.Login ||
                    idAndObjectType.ObjectType == ObjectType.LoginGroup)
                {
                    return;
                }

                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.ObjectRemoved(
                        idAndObjectType,
                        oldSiteId));
            }

            public void NotifyObjectRenamed(
                IdAndObjectType idAndObjectType,
                string newName)
            {
                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.ObjectRenamed(
                        idAndObjectType,
                        newName));
            }

            private BrokenReferencesInfo CreateBrokenReferencesInfo(IdAndObjectType idAndObjectType)
            {
                var objectInfo = _localEvaluator.GetObjectInfo(idAndObjectType);

                if (objectInfo == null)
                    return null;

                var parentIdAndObjectType = objectInfo.Parent;

                return new BrokenReferencesInfo(
                    objectInfo,
                    objectInfo.Name,
                    parentIdAndObjectType != null
                        ? new IdAndObjectType(
                            parentIdAndObjectType.Id,
                            parentIdAndObjectType.ObjectType)
                        : null,
                    GetBrokenDirectReferences(objectInfo));
            }

            public void NotifySiteDeleted(int siteId)
            {
                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.SiteDeleted(siteId));
            }

            public void NotifySiteRenamed(SiteIdAndName siteIdAndName)
            {
                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.SiteRenamed(siteIdAndName));
            }

            public void NotifySiteCreated(
                SiteIdAndName siteIdAndName,
                int parentSiteId)
            {
                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.SiteCreated(
                        siteIdAndName,
                        parentSiteId));
            }

            public void NotifyTopMostSitesChanged()
            {
                _notificationToClientQueue.Enqueue(
                    _structuredSiteBuilderClient.TopMostSitesChanged);
            }

            public void NotifyParentChanged(
                IdAndObjectType idAndObjectType,
                IdAndObjectType newParentIdAndObjectType,
                IdAndObjectType oldParentIdAndObjectType)
            {
                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.ObjectParentChanged(
                        idAndObjectType,
                        newParentIdAndObjectType,
                        oldParentIdAndObjectType));
            }

            public void NotifyStructureChanged()
            {
                _notificationToClientQueue.Enqueue(
                    _structuredSiteBuilderClient.StructureChanged);
            }

            public ClientConnector(
                LocalEvaluator localEvaluator,
                IStructuredSiteBuilderClient structuredSiteBuilderClient)
            {
                _localEvaluator = localEvaluator;
                _structuredSiteBuilderClient = structuredSiteBuilderClient;

                _notificationToClientQueue.ItemProcessing += SendNotificationToClient;

            }

            public void NotifyEndUpdate()
            {
                _notificationToClientQueue.Enqueue(
                    NotifyEndUpdateInternal);
            }

            private void NotifyEndUpdateInternal()
            {
                _structuredSiteBuilderClient.EndUpdate();
            }

            public void NotifyBeginUpdate()
            {
                _notificationToClientQueue.Enqueue(
                    NotifyBeginUpdateInternal);
            }

            private void NotifyBeginUpdateInternal()
            {
                _structuredSiteBuilderClient.BeginUpdate();
            }

            private void SendNotificationToClient(DVoid2Void action)
            {
                try
                {
                    action();
                }
                catch (Exception)
                {
                    _notificationToClientQueue.Clear();
                    _localEvaluator.Cancel();
                }
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }

            public void NotifyBrokenReferences(
                IEnumerable<LocalObjectInfo> objectsWithBrokenReferences)
            {
                var brokenReferencesInfos = 
                    new LinkedList<BrokenReferencesInfo>(
                        objectsWithBrokenReferences
                            .Select(localObjectInfo => CreateBrokenReferencesInfo(localObjectInfo))
                            .Where(brokenReferencesInfo => brokenReferencesInfo != null));

                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.BrokenDirectReferencesForObjectsChanged(
                        brokenReferencesInfos));
            }

            public void NotifyEmptyBrokenReferences(LocalObjectInfo localObjectInfo)
            {
                var brokenReferencesInfos = 
                    new LinkedList<BrokenReferencesInfo>(
                        Enumerable.Repeat(
                            new BrokenReferencesInfo(
                                localObjectInfo,
                                null,
                                null,
                                null),
                            1));

                _notificationToClientQueue.Enqueue(
                    () => _structuredSiteBuilderClient.BrokenDirectReferencesForObjectsChanged(
                        brokenReferencesInfos));
            }

            #region FolderStructure

            void IStructuredSiteBuilder.CreateFolder(int siteId, IdAndObjectType parentFolderObject, string folderName)
            {
                var siteInfo = _localEvaluator.GetSiteInfo(siteId);

                if (siteInfo == null)
                    return;

                _localEvaluator.PerformModificationAndNotifyClient(
                    _localEvaluator.CreateFolderInternal,
                    siteInfo,
                    parentFolderObject,
                    folderName);
            }

            void IStructuredSiteBuilder.RemoveFolder(IdAndObjectType objectId)
            {
                _localEvaluator.PerformModificationAndNotifyClient(
                    _localEvaluator.RemoveFolderInternal,
                    objectId);
            }

            void IStructuredSiteBuilder.RenameFolder(IdAndObjectType objectId, string newFolderName)
            {
                _localEvaluator.PerformModificationAndNotifyClient(
                    _localEvaluator.RenameFolderInternal,
                    objectId,
                    newFolderName);
            }

            void IStructuredSiteBuilder.AddObjectToFolder(IdAndObjectType objectId, IdAndObjectType folderId)
            {
                   _localEvaluator.AddObjectToFolder(
                    objectId,
                    folderId);
            }

            void IStructuredSiteBuilder.RemoveObjectFromFolder(IdAndObjectType objectId, IdAndObjectType folderId)
            {
                _localEvaluator.RemoveObjectFromFolder(
                 objectId,
                 folderId);
            }

            #endregion
        }
    }
}