using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public partial class LocalEvaluator :
        StructuredSiteEvaluator<
            LocalEvaluator.LocalObjectInfo,
            GlobalEvaluator.GlobalObjectInfo,
            SaveableReadOnlyProxySubset<GlobalEvaluator.GlobalObjectInfo>,
            SaveableSubset<GlobalEvaluator.GlobalObjectInfo>,
            LocalEvaluator.LocalSiteInfo,
            DifferentialSubset<IdAndObjectType>,
            SaveableSubset<int>>,
        ILocalEvaluator
    {
        private readonly GlobalEvaluator _globalEvaluator;

        private int _localMaxSiteId;

        private void SaveRemoveSite(GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
        {
            _globalEvaluator.SaveRemoveSite(globalSiteInfo);
        }

        private LocalSiteInfo _siteInfoBeingSaved;
        private Login _login;

        private enum SavingState
        {
            SavingOwn,
            SavingForeign,
            NotSaving
        }

        private SavingState _savingState = SavingState.NotSaving;

        private void SaveCreateSite(LocalSiteInfo siteInfoBeingSaved)
        {
            _siteInfoBeingSaved = siteInfoBeingSaved;

            _globalEvaluator.SaveCreateSite(
                siteInfoBeingSaved.Name,
                siteInfoBeingSaved.ParentSite.Id);

            _siteInfoBeingSaved = null;
        }

        private void SaveUpdateSite(LocalSiteInfo siteInfoBeingSaved)
        {
            _siteInfoBeingSaved = siteInfoBeingSaved;

            _globalEvaluator.SaveUpdateSite(
                siteInfoBeingSaved.GlobalSiteInfo,
                siteInfoBeingSaved.Name);

            _siteInfoBeingSaved = null;
        }

        public LocalEvaluator(
            GlobalEvaluator globalEvaluator,
            Login login,
            IStructuredSiteBuilderClient structuredSiteBuilderClient)
            : base(new DifferentialSubset<IdAndObjectType>(
                globalEvaluator.ObjectsWithBrokenDirectReferencesSet))
        {
            _globalEvaluator = globalEvaluator;

            _connector =
                new ClientConnector(
                    this,
                    structuredSiteBuilderClient);

            _login = login;

            Load();
        }

        private ICollection<ObjectType> _visibleObjectTypes;

        private void Load()
        {
            _visibleObjectTypes =
                new HashSet<ObjectType>(
                    _globalEvaluator.GetVisibleObjectTypes(_login));

            CreateLocalSiteAndChilren(
                _globalEvaluator.Root,
                null);

            foreach (var globalObjectInfo in _globalEvaluator.ObjectInfos)
            {
                var localObjectInfo =
                    new LocalObjectInfo(
                        globalObjectInfo,
                        GetSiteInfo(globalObjectInfo.SiteInfo.Id));

                AddObjectInfo(localObjectInfo);
            }

            _localMaxSiteId = _globalEvaluator.MaxSiteId + 1;

            AddTopMostSites(Root);
        }

        private readonly HashSet<LocalSiteInfo> _topMostSites =
            new HashSet<LocalSiteInfo>();

        private void AddTopMostSites(LocalSiteInfo siteInfo)
        {
            foreach (var subTreeReference in siteInfo.SubTreeReferences)
                if (subTreeReference.ObjectType == ObjectType.Login &&
                    subTreeReference.Id.Equals(_login.IdLogin))
                {
                    _topMostSites.Add(siteInfo);
                    return;
                }

            foreach (var childSiteInfo in siteInfo.ChildrenSites)
                AddTopMostSites(childSiteInfo);
        }

        private void RemoveTopMostSites(LocalSiteInfo siteInfo)
        {
            foreach (var subTreeReference in siteInfo.SubTreeReferences)
                if (subTreeReference.ObjectType == ObjectType.Login &&
                    subTreeReference.Id.Equals(_login.IdLogin))
                {
                    _topMostSites.Remove(siteInfo);
                    return;
                }

            foreach (var childSiteInfo in siteInfo.ChildrenSites)
                RemoveTopMostSites(childSiteInfo);
        }

        public void NotifyIfBrokenReferencesChanged()
        {
            if (_objectsWithChangedBrokenDirectReferences.Count == 0)
                return;

            var objectsWithBrokenReferences =
                new LinkedList<LocalObjectInfo>(
                    _objectsWithChangedBrokenDirectReferences
                        .Where(idAndObjectType =>
                            _visibleObjectTypes
                                .Contains(idAndObjectType.ObjectType)));

            if (objectsWithBrokenReferences.Count != 0)
                _connector.NotifyBrokenReferences(objectsWithBrokenReferences);

            _objectsWithChangedBrokenDirectReferences.Clear();
        }

        public void PerformModificationAndNotifyClient<T>(
            Action<T> action,
            T param)
        {
            _connector.NotifyBeginUpdate();

            try
            {
                action(param);

                NotifyIfBrokenReferencesChanged();
            }
            finally
            {
                _connector.NotifyEndUpdate();
            }
        }

        public void PerformModificationAndNotifyClient<T1, T2>(
            Action<T1, T2> action,
            T1 param1,
            T2 param2)
        {
            _connector.NotifyBeginUpdate();

            try
            {
                action(param1, param2);

                NotifyIfBrokenReferencesChanged();
            }
            finally
            {
                _connector.NotifyEndUpdate();
            }
        }

        public void PerformModificationAndNotifyClient<T1, T2, T3>(
            Action<T1, T2, T3> action,
            T1 param1,
            T2 param2,
            T3 param3)
        {
            _connector.NotifyBeginUpdate();

            try
            {
                action(param1, param2, param3);

                NotifyIfBrokenReferencesChanged();
            }
            finally
            {
                _connector.NotifyEndUpdate();
            }
        }

        public TReturn PerformModificationAndNotifyClient<TReturn, T1, T2>(
            Func<T1, T2, TReturn> func,
            T1 param1,
            T2 param2)
            where TReturn : class
        {
            _connector.NotifyBeginUpdate();

            TReturn result;

            try
            {
                result = func(param1, param2);

                NotifyIfBrokenReferencesChanged();
            }
            catch
            {
                return null;
            }
            finally
            {
                _connector.NotifyEndUpdate();
            }

            return result;
        }

        void ILocalEvaluator.OnObjectAdded(GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
        {
            if (_savingState != SavingState.NotSaving)
                return;

            var parentIdAndObjectType = globalObjectInfo.Parent;

            PerformModificationAndNotifyClient(
                OnObjectAddedInternal,
                globalObjectInfo,
                GetSiteInfo(globalObjectInfo.SiteInfo.Id),
                parentIdAndObjectType != null
                    ? GetObjectInfo(parentIdAndObjectType).SiteInfo
                    : null);
        }

        private void OnObjectAddedInternal(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            LocalSiteInfo localSiteInfo,
            LocalSiteInfo localParentSiteInfo)
        {
            var localObjectInfo =
                new LocalObjectInfo(
                    globalObjectInfo,
                    localSiteInfo);

            AddObjectInfo(localObjectInfo);

            if (localParentSiteInfo != null)
                localObjectInfo.SiteInfo = localParentSiteInfo;

            EvaluateBackReferences(localObjectInfo);
            EvaluateDirectReferences(localObjectInfo);

            _connector.NotifyObjectAdded(
                CreateObjectInSiteInfo(localObjectInfo),
                localObjectInfo.SiteInfo.Id);
        }

        void ILocalEvaluator.OnObjectUpdated(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
        {
            if (_savingState == SavingState.SavingForeign)
            {
                _structureChanged = true;
                return;
            }

            if (_savingState == SavingState.SavingOwn)
                return;

            var localObjectInfo = GetObjectInfo(globalObjectInfo);

            if (localObjectInfo == null)
                return;

            PerformModificationAndNotifyClient(
                OnObjectUpdatedInternal,
                globalObjectInfo,
                localObjectInfo);
        }

        private void OnObjectUpdatedInternal(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            LocalObjectInfo localObjectInfo)
        {
            bool myLoginOrGroupUpdated =
                OnLoginOrGroupUpdated(globalObjectInfo);

            localObjectInfo.UpdateSets();

            EvaluateBackReferences(localObjectInfo);
            EvaluateDirectReferences(localObjectInfo);

            if (myLoginOrGroupUpdated)
                _connector.NotifyStructureChanged();
            else
                NotifyIfBrokenReferencesChanged();
        }

        private bool OnLoginOrGroupUpdated(GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
        {
            var username = _login.Username;

            var objectId = globalObjectInfo.Id;

            string objectIdString;

            var objectType = globalObjectInfo.ObjectType;

            if (objectType != ObjectType.LoginGroup)
            {
                if (objectType != ObjectType.Login)
                    return false;

                objectIdString = (string)objectId;

                if (username != objectIdString)
                    return false;
            }
            else
                objectIdString = (string)objectId;

            var newLogin = Logins.Singleton.GetLoginByUserName(username);

            if (newLogin == null)
                return false;

            if (objectType == ObjectType.LoginGroup)
            {
                var loginGroup = newLogin.LoginGroup;

                if (loginGroup == null ||
                    loginGroup.IdLoginGroup.ToString() != objectIdString)
                {
                    return false;
                }
            }

            _login = newLogin;

            _visibleObjectTypes =
                new HashSet<ObjectType>(
                    _globalEvaluator.GetVisibleObjectTypes(_login));

            return true;
        }

        void ILocalEvaluator.OnObjectDeleted(IdAndObjectType parameter)
        {
            var localObjectInfo = GetObjectInfo(parameter);

            if (localObjectInfo == null)
                return;

            int siteId = localObjectInfo.SiteInfo.Id;

            if (!RemoveObjectInfo(localObjectInfo))
                return;

            //TODO Clean up broken references

            PerformModificationAndNotifyClient(
                OnObjectDeletedInternal,
                localObjectInfo,
                siteId);
        }

        private void OnObjectDeletedInternal(
            LocalObjectInfo localObjectInfo,
            int siteId)
        {
            _connector.NotifyObjectRemoved(
                new IdAndObjectType(localObjectInfo),
                siteId);

            if (!_visibleObjectTypes.Contains(localObjectInfo.ObjectType))
                return;

            _connector.NotifyEmptyBrokenReferences(localObjectInfo);
        }

        private bool _structureChanged;

        void ILocalEvaluator.OnSiteCreated(
            GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
        {
            if (_savingState == SavingState.NotSaving)
                return;

            if (_savingState == SavingState.SavingOwn)
            {
                OnOwnSiteCreated(globalSiteInfo);

                return;
            }

            _structureChanged = true;

            // TODO minimize the structureChanged 
            /*
            var conflictingSiteInfo = GetSiteInfo(globalSiteInfo.Id);

            if (conflictingSiteInfo != null)
                conflictingSiteInfo.SetIdIfNew(
                    this,
                    _localMaxSiteId++);

            var globalParentSite = globalSiteInfo.ParentSite;

            var parentSiteId = globalParentSite.Id;

            var localParentSite =
                globalParentSite != null
                    ? GetSiteInfo(parentSiteId)
                    : null;

            var localSiteInfo =
                CreateLocalSiteAndChilren(
                    globalSiteInfo,
                    localParentSite);

            var siteIdAndName = 
                new SiteIdAndName(
                    localSiteInfo.Id,
                    localSiteInfo.Name);

            _structuredSiteBuilderClient.SiteCreated(
                siteIdAndName,
                localParentSite != null
                    ? parentSiteId
                    : -1);*/
        }

        void ILocalEvaluator.OnSiteUpdated(
            GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
        {
            if (_savingState == SavingState.SavingForeign)
                _structureChanged = true;
        }

        void ILocalEvaluator.OnSiteRemoved(GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
        {
            if (_savingState == SavingState.SavingForeign)
                _structureChanged = true;
        }

        void ILocalEvaluator.OnBeginSave()
        {
            if (_savingState == SavingState.NotSaving)
                _savingState = SavingState.SavingForeign;
        }

        void ILocalEvaluator.OnEndSave()
        {
            if (_savingState == SavingState.NotSaving)
                return;

            if (_savingState == SavingState.SavingOwn)
            {
                foreach (var localObjectInfo in ObjectInfos)
                    localObjectInfo.UpdateSets();

                _localMaxSiteId =
                    Math.Max(
                        _localMaxSiteId,
                        _globalEvaluator.MaxSiteId + 1);

                return;
            }

            _savingState = SavingState.NotSaving;

            if (!_structureChanged)
                return;

            try
            {
                StructureChanged();
            }
            finally
            {
                _structureChanged = false;
            }
        }

        private void StructureChanged()
        {
            _objectsWithBrokenDirectReferences.Reset();

            lock (_modifiedFolders)
                _modifiedFolders.Clear();

            Clear();
            Load();

            _connector.NotifyStructureChanged();
        }

        void ILocalEvaluator.OnObjectMoved(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            GlobalEvaluator.GlobalSiteInfo oldSiteInfo)
        {
            if (_savingState == SavingState.SavingForeign)
            {
                _structureChanged = true;
                return;
            }

            if (_savingState != SavingState.NotSaving)
                return;

            // object move due to the change of its parent

            PerformModificationAndNotifyClient(
                OnObjectMovedInternal,
                    globalObjectInfo,
                    oldSiteInfo);
        }

        private void OnObjectMovedInternal(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            GlobalEvaluator.GlobalSiteInfo oldSiteInfo)
        {
            _connector.NotifyObjectRemoved(
                new IdAndObjectType(globalObjectInfo),
                oldSiteInfo.Id);

            _connector.NotifyObjectAdded(
                CreateObjectInSiteInfo(globalObjectInfo),
                globalObjectInfo.SiteInfo.Id);
        }

        void ILocalEvaluator.OnObjectParentChanged(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            IdAndObjectType oldObjectParent)
        {
            if (_savingState == SavingState.SavingForeign)
            {
                _structureChanged = true;
                return;
            }

            if (_savingState == SavingState.SavingOwn)
                return;

            PerformModificationAndNotifyClient(
                OnObjectParentChangedInternal,
                globalObjectInfo,
                oldObjectParent);
        }

        private void OnObjectParentChangedInternal(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            IdAndObjectType oldObjectParent)
        {
            if (!_visibleObjectTypes.Contains(globalObjectInfo.ObjectType))
                return;

            var newObjectParent = globalObjectInfo.Parent;

            _connector.NotifyParentChanged(
                new IdAndObjectType(globalObjectInfo),
                newObjectParent != null
                    ? new IdAndObjectType(newObjectParent)
                    : null,
                oldObjectParent != null
                    ? new IdAndObjectType(oldObjectParent)
                    : null);
        }

        void ILocalEvaluator.OnObjectRenamed(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
        {
            if (_savingState == SavingState.SavingForeign)
            {
                _structureChanged = true;
                return;
            }

            if (_savingState == SavingState.SavingOwn)
                return;

            PerformModificationAndNotifyClient(
                OnObjectRenamedInternal,
                globalObjectInfo);
        }

        private void OnObjectRenamedInternal(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
        {
            if (!_visibleObjectTypes.Contains(globalObjectInfo.ObjectType))
                return;

            _connector.NotifyObjectRenamed(
                new IdAndObjectType(globalObjectInfo),
                globalObjectInfo.Name);
        }

        protected override void OnSubTreeReferenceAdded(
            LocalObjectInfo objectInfo,
            LocalSiteInfo siteInfo)
        {
            if (_savingState == SavingState.SavingForeign)
            {
                _structureChanged = true;
                return;
            }

            var objectType = objectInfo.ObjectType;

            if (_savingState == SavingState.NotSaving)
            {
                var parent = objectInfo.Parent;

                if (_visibleObjectTypes.Contains(objectType))
                    _connector.NotifySubTreeReferenceAdded(
                        new ReferenceInSiteInfo(
                            objectInfo.Id,
                            objectType,
                            objectInfo.Name,
                            parent != null
                                ? new IdAndObjectType(parent)
                                : null,
                            true),
                        siteInfo.Id);
            }

            if (objectType != ObjectType.Login ||
                !objectInfo.Id.Equals(_login.IdLogin))
            {
                return;
            }

            LocalSiteInfo currentSiteInfo = siteInfo;

            do
            {
                if (_topMostSites.Contains(currentSiteInfo))
                    return;

                currentSiteInfo = currentSiteInfo.ParentSite;
            }
            while (currentSiteInfo != null);

            RemoveTopMostSites(siteInfo);

            _topMostSites.Add(siteInfo);

            _connector.NotifyTopMostSitesChanged();
        }

        protected override void OnSubTreeReferenceRemoved(
            LocalObjectInfo objectInfo,
            LocalSiteInfo siteInfo)
        {
            if (_savingState == SavingState.SavingForeign)
            {
                _structureChanged = true;
                return;
            }

            if (_savingState == SavingState.NotSaving)
                _connector.NotifySubTreeReferenceRemoved(
                    new IdAndObjectType(objectInfo),
                    siteInfo.Id,
                    AncestralReferenceExists(
                        objectInfo,
                        siteInfo));

            if (objectInfo.ObjectType != ObjectType.Login ||
                !objectInfo.Id.Equals(_login.IdLogin))
            {
                return;
            }

            _topMostSites.Remove(siteInfo);

            AddTopMostSites(siteInfo);

            _connector.NotifyTopMostSitesChanged();
        }

        void ILocalEvaluator.OnSubTreeReferenceAdded(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
        {
            if (_savingState == SavingState.SavingForeign)
            {
                _structureChanged = true;
                return;
            }

            if (_savingState == SavingState.SavingOwn)
                return;

            var localObjectInfo = GetObjectInfo(globalObjectInfo);

            if (localObjectInfo == null)
                return;

            localObjectInfo.UpdateSets();

            PerformModificationAndNotifyClient(
                OnSubTreeReferenceAddedInternal,
                localObjectInfo,
                globalSiteInfo.Id);
        }

        private void OnSubTreeReferenceAddedInternal(LocalObjectInfo localObjectInfo, int siteId)
        {
            var localSiteInfo = GetSiteInfo(siteId);

            if (localSiteInfo != null)
                localSiteInfo.UpdateSets();

            EvaluateBackReferences(localObjectInfo);
            EvaluateDirectReferences(localObjectInfo);

            if (_visibleObjectTypes.Contains(localObjectInfo.ObjectType))
                _connector.NotifySubTreeReferenceAdded(
                    CreateReferenceInSiteInfo(
                        localObjectInfo,
                        true),
                    siteId);
        }

        void ILocalEvaluator.OnSubTreeReferenceRemoved(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
        {
            if (_savingState == SavingState.SavingForeign)
            {
                _structureChanged = true;
                return;
            }

            if (_savingState == SavingState.SavingOwn)
                return;

            var localObjectInfo = GetObjectInfo(globalObjectInfo);

            if (localObjectInfo == null)
                return;

            var siteId = globalSiteInfo.Id;

            PerformModificationAndNotifyClient(
                OnSubTreeReferenceRemovedInternal,
                localObjectInfo,
                siteId);
        }

        private void OnSubTreeReferenceRemovedInternal(
            LocalObjectInfo localObjectInfo,
            int siteId)
        {
            var localSiteInfo = GetSiteInfo(siteId);

            localObjectInfo.UpdateSets();

            if (localSiteInfo != null)
                localSiteInfo.UpdateSets();

            EvaluateBackReferences(localObjectInfo);
            EvaluateDirectReferences(localObjectInfo);

            _connector.NotifySubTreeReferenceRemoved(
                new IdAndObjectType(localObjectInfo),
                siteId,
                AncestralReferenceExists(
                    localObjectInfo,
                    localSiteInfo));
        }

        void ILocalEvaluator.OnDirectBrokenReferencesChanged(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
        {
            var localObjectInfo = GetObjectInfo(globalObjectInfo);

            if (localObjectInfo != null)
                _objectsWithChangedBrokenDirectReferences
                    .Add(localObjectInfo);
        }

        void ILocalEvaluator.OnBeginUpdate()
        {
            _connector.NotifyBeginUpdate();
        }

        void ILocalEvaluator.OnEndUpdate()
        {
            _connector.NotifyEndUpdate();
        }

        void ILocalEvaluator.OnUserFoldersStructureChanged()
        {
            StructureChanged();
        }

        void ILocalEvaluator.SetClient(IStructuredSiteBuilderClient structuredSiteBuilderClient)
        {
            _connector =
                new ClientConnector(
                    this,
                    structuredSiteBuilderClient);
        }

        private void OnOwnSiteCreated(GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
        {
            int newSiteId = globalSiteInfo.Id;

            if (_siteInfoBeingSaved.Id != newSiteId)
            {
                var conflictingSiteInfo = GetSiteInfo(newSiteId);

                if (conflictingSiteInfo != null)
                {
                    UpdateNextFreeSiteId();

                    conflictingSiteInfo.SetIdIfNew(
                        this,
                        _localMaxSiteId++);
                }
            }

            _siteInfoBeingSaved.OnCreated(
                this,
                globalSiteInfo);
        }

        private void CreateLocalSiteAndChilren(GlobalEvaluator.GlobalSiteInfo globalSiteInfo,
            LocalSiteInfo localParentSite)
        {
            var result =
                new LocalSiteInfo(globalSiteInfo);

            AddSiteInfo(
                result, 
                localParentSite);

            foreach (var globalChildSite in globalSiteInfo.ChildrenSites)
                CreateLocalSiteAndChilren(
                    globalChildSite,
                    result);
        }

        public override IEnumerable<LocalObjectInfo> GetBackReferences(LocalObjectInfo objectInfo)
        {
            return
                objectInfo.BackReferences
                    .Select(backReference => GetObjectInfo(backReference))
                    .Where(backReference => backReference != null);
        }

        public override LocalSiteInfo Root
        {
            get
            {
                return GetSiteInfo(_globalEvaluator.Root.Id);
            }
        }

        IStructuredSiteBuilder ILocalEvaluator.Builder
        {
            get { return _connector; }
        }

        public override IEnumerable<LocalObjectInfo> GetChildren(LocalObjectInfo objectInfo)
        {
            return
                objectInfo.Children
                    .Select(globalChildObject => GetObjectInfo(globalChildObject))
                    .Where(localChildObject => localChildObject != null);
        }

        public override IEnumerable<LocalObjectInfo> GetDirectReferences(LocalObjectInfo objectInfo)
        {
            return
                objectInfo.DirectReferences
                    .Select(directReference => GetObjectInfo(directReference))
                    .Where(directReference => directReference != null);
        }

        private readonly ICollection<LocalObjectInfo> _objectsWithChangedBrokenDirectReferences =
            new HashSet<LocalObjectInfo>();

        private ClientConnector _connector;

        protected override void AddBrokenReferenceInternal(
            LocalObjectInfo sourceObjectInfo,
            LocalObjectInfo targetObjectInfo)
        {
            targetObjectInfo.AddBrokenBackReference(sourceObjectInfo.GlobalObjectInfo);

            if (sourceObjectInfo.AddBrokenDirectReference(targetObjectInfo.GlobalObjectInfo))
                _objectsWithChangedBrokenDirectReferences.Add(sourceObjectInfo);
        }

        protected override void RemoveBrokenReferenceInternal(
            LocalObjectInfo sourceObjectInfo,
            LocalObjectInfo targetObjectInfo)
        {
            targetObjectInfo.RemoveBrokenBackReference(sourceObjectInfo.GlobalObjectInfo);

            if (sourceObjectInfo.RemoveBrokenDirectReference(targetObjectInfo.GlobalObjectInfo))
                _objectsWithChangedBrokenDirectReferences.Add(sourceObjectInfo);
        }

        private LocalSiteInfo CreateSiteInfoInternal(
            int parentSiteId,
            string name)
        {
            var parentSiteInfo = GetSiteInfo(parentSiteId);

            if (parentSiteInfo == null)
                return null;

            UpdateNextFreeSiteId();

            var siteInfo =
                new LocalSiteInfo(
                    _localMaxSiteId++,
                    name);

            AddSiteInfo(
                siteInfo,
                parentSiteInfo);

            var siteIdAndName =
                new SiteIdAndName(
                    siteInfo.Id,
                    siteInfo.Name);

            _connector.NotifySiteCreated(
                siteIdAndName,
                parentSiteId);

            return siteInfo;
        }

        private void UpdateNextFreeSiteId()
        {
            while (SiteExists(_localMaxSiteId))
                ++_localMaxSiteId;
        }

        private void RenameSiteInternal(string newName, LocalSiteInfo site)
        {
            site.Name = newName;

            _connector.NotifySiteRenamed(
                new SiteIdAndName(
                    site.Id,
                    site.Name));
        }

        private void DeleteSiteInternal(
            LocalSiteInfo siteInfo,
            LocalSiteInfo targetSiteForRemovedObjects)
        {
            var children =
                new LinkedList<LocalSiteInfo>(siteInfo.ChildrenSites);

            foreach (var childSiteInfo in children)
                DeleteSiteInternal(
                    childSiteInfo,
                    targetSiteForRemovedObjects);

            var idAndObjectTypes =
                new LinkedList<IdAndObjectType>(siteInfo.Objects);

            foreach (var idAndObjectType in idAndObjectTypes)
            {
                var objectInfo = GetObjectInfo(idAndObjectType);

                if (objectInfo == null)
                    continue;

                MoveObject(
                    objectInfo,
                    targetSiteForRemovedObjects);
            }

            var subTreeReferences =
                new LinkedList<IdAndObjectType>(siteInfo.SubTreeReferences);

            foreach (var idAndObjectType in subTreeReferences)
            {
                var objectInfo = GetObjectInfo(idAndObjectType);

                if (objectInfo == null)
                    continue;

                RemoveSubTreeReference(
                    objectInfo,
                    siteInfo);
            }

            RemoveSiteInfo(siteInfo);

            _connector.NotifySiteDeleted(siteInfo.Id);
        }

        private void MoveObject(
            LocalObjectInfo objectInfo,
            LocalSiteInfo newSiteInfo)
        {
            int oldSiteId = objectInfo.SiteInfo.Id;

            new MoveOperation(
                this,
                objectInfo,
                newSiteInfo).Execute();

            _connector.NotifyObjectRemoved(
                new IdAndObjectType(objectInfo),
                oldSiteId);

            _connector.NotifyObjectAdded(
                CreateObjectInSiteInfo(objectInfo),
                newSiteInfo.Id);

            foreach (var childObjectInfo in GetChildren(objectInfo))
                MoveObject(childObjectInfo, newSiteInfo);
        }

        private void AddSubTreeReferenceInternal(
            LocalObjectInfo objectInfo,
            LocalSiteInfo siteInfo)
        {
            AddSubTreeReference(
                objectInfo,
                siteInfo);

            if (_visibleObjectTypes.Contains(objectInfo.ObjectType))
                _connector.NotifySubTreeReferenceAdded(
                    CreateReferenceInSiteInfo(objectInfo, true),
                    siteInfo.Id);

            foreach (var localObjectInfo in GetChildren(objectInfo))
                AddSubTreeReferenceInternal(
                    localObjectInfo,
                    siteInfo);
        }

        private void RemoveSubTreeReferenceInternal(
            LocalObjectInfo objectInfo,
            LocalSiteInfo siteInfo)
        {
            RemoveSubTreeReference(
                objectInfo,
                siteInfo);

            _connector.NotifySubTreeReferenceRemoved(
                new IdAndObjectType(
                    objectInfo.Id,
                    objectInfo.ObjectType),
                siteInfo.Id,
                AncestralReferenceExists(
                    objectInfo,
                    siteInfo));

            foreach (var localObjectInfo in GetChildren(objectInfo))
                RemoveSubTreeReferenceInternal(
                    localObjectInfo,
                    siteInfo);
        }

        private bool ObjectPlacementVisible(IdAndObjectType idAndObjectType)
        {
            var objectType = idAndObjectType.ObjectType;

            return
                objectType != ObjectType.Login &&
                objectType != ObjectType.LoginGroup &&
                _visibleObjectTypes.Contains(objectType);
        }

        private static ObjectInSiteInfo CreateObjectInSiteInfo(AObjectInfo objectInfo)
        {
            if (objectInfo == null)
                return null;

            // objectInfo.Parent might possibly contain non-serializable derived types (ie GlobalObjectInfo)
            var parentIdAndObjectType = objectInfo.Parent;

            return new ObjectInSiteInfo(
                objectInfo.Id,
                objectInfo.ObjectType,
                objectInfo.Name,
                parentIdAndObjectType != null
                    ? new IdAndObjectType(
                        parentIdAndObjectType.Id,
                        parentIdAndObjectType.ObjectType)
                    : null);
        }

        private ReferenceInSiteInfo CreateReferenceInSiteInfo(
            IdAndObjectType idAndObjectType,
            bool definedInThisSite)
        {
            var objectInfo = GetObjectInfo(idAndObjectType);

            return
                objectInfo != null
                    ? CreateReferenceInSiteInfo(objectInfo, definedInThisSite)
                    : null;
        }

        private static ReferenceInSiteInfo CreateReferenceInSiteInfo(
            LocalObjectInfo objectInfo,
            bool definedInThisSite)
        {
            // objectInfo.Parent might possibly contain non-serializable derived types (ie GlobalObjectInfo)

            var parentIdAndObjectType = objectInfo.Parent;

            return new ReferenceInSiteInfo(
                objectInfo.Id,
                objectInfo.ObjectType,
                objectInfo.Name,
                parentIdAndObjectType != null
                    ? new IdAndObjectType(
                        parentIdAndObjectType.Id,
                        parentIdAndObjectType.ObjectType)
                    : null,
                definedInThisSite);
        }

        private static IEnumerable<ObjectInSiteInfo> GetBrokenDirectReferences(
            LocalObjectInfo objectInfo)
        {
            var objectInSiteInfos =
                new LinkedList<ObjectInSiteInfo>(
                    objectInfo.BrokenDirectReferences
                        .Select(brokenDirectReference => CreateObjectInSiteInfo(brokenDirectReference))
                        .Where(objectInSiteInfo => objectInSiteInfo != null));

            return objectInSiteInfos.Count > 0
                ? objectInSiteInfos
                : null;
        }

        public void Save()
        {
            _savingState = SavingState.SavingOwn;

            _globalEvaluator.LocalEvaluatorManager.OnBeginSave();

            Root.SaveSiteInfo(this);

            SaveFoldersChanges();

            Root.SaveSiteContent(_globalEvaluator);
            Root.ObjectsOnSaved();

            Root.SaveRemovedSites(this);

            _globalEvaluator.LocalEvaluatorManager.OnEndSave();

            _savingState = SavingState.NotSaving;
        }

        public void Cancel()
        {
            _globalEvaluator.LocalEvaluatorManager.RemoveLocalEvaluator(
                _globalEvaluator,
                _login.Username);
        }

#region FolderStructure

        private void SaveFoldersChanges()
        {
            var foldersToDelete = new LinkedList<KeyValuePair<LocalObjectInfo, DifferentialSubset<IdAndObjectType>>>();
            var personDepartmentChanges = new Dictionary<object, PersonDepartmentChange>();

            ICollection<KeyValuePair<LocalObjectInfo, DifferentialSubset<IdAndObjectType>>> kvpModifiedFolders;

            lock (_modifiedFolders)
                kvpModifiedFolders = SortFoldersByParent(_modifiedFolders);

            foreach (var kvpModifiedFolder in kvpModifiedFolders)
            {
                var objectInfo = GetObjectInfo(kvpModifiedFolder.Key);

                if (objectInfo == null)
                {
                    foldersToDelete.AddFirst(kvpModifiedFolder);
                    continue;
                }

                if (objectInfo.GlobalObjectInfo == null)
                {
                    var newFolderStructure = new UserFoldersStructure();

                    if (objectInfo.Parent != null)
                        newFolderStructure.ParentFolder = UserFoldersStructures.Singleton.GetById(objectInfo.Parent.Id);

                    newFolderStructure.FolderName = objectInfo.Name;
                    AddFolderStructureObjects(newFolderStructure, kvpModifiedFolder.Value, personDepartmentChanges);

                    objectInfo.OnSaved(_globalEvaluator.SaveCreateFolder(newFolderStructure, objectInfo.Id));
                    continue;
                }

                var folderStructure = UserFoldersStructures.Singleton.GetObjectForEdit(objectInfo.Id);

                folderStructure.FolderName = objectInfo.Name;
                AddFolderStructureObjects(folderStructure, kvpModifiedFolder.Value, personDepartmentChanges);

                _globalEvaluator.SaveUpdateFolder(folderStructure);
                UserFoldersStructures.Singleton.EditEnd(folderStructure);

                objectInfo.OnSaved(null);
            }

            foreach (var folderToDelete in foldersToDelete)
            {
                if (_globalEvaluator.SaveDeleteFolder(folderToDelete.Key))
                {
                    AddPersonDepartmentChanges(
                        null,
                        folderToDelete.Value.Elements,
                        null,
                        personDepartmentChanges);
                }
            }

            foreach (var kvpPersonDepartmentChange in personDepartmentChanges)
            {
                UserFoldersStructureObjects.Singleton.RunEventPersonDepartmentChanged(
                    new Guid((string) kvpPersonDepartmentChange.Key),
                    kvpPersonDepartmentChange.Value);
            }

            lock (_modifiedFolders)
                _modifiedFolders.Clear();
        }

        private ICollection<KeyValuePair<T1, T2>> SortFoldersByParent<T1, T2>(
            IEnumerable<KeyValuePair<T1, T2>> foldersInfo)
            where T1 : LocalObjectInfo
        {
            if (foldersInfo == null)
                return null;

            var foldersWithChildfolders = new Dictionary<IdAndObjectType, ICollection<KeyValuePair<T1, T2>>>();
            var foldersInfoIds = new HashSet<IdAndObjectType>();
            foreach (var folderInfo in foldersInfo)
            {
                foldersInfoIds.Add(folderInfo.Key);

                if (folderInfo.Key.Parent == null)
                    continue;

                ICollection<KeyValuePair<T1, T2>> childfolders;
                if (!foldersWithChildfolders.TryGetValue(folderInfo.Key.Parent, out childfolders))
                {
                    childfolders = new LinkedList<KeyValuePair<T1, T2>>();
                    foldersWithChildfolders.Add(folderInfo.Key.Parent, childfolders);
                }

                childfolders.Add(folderInfo);
            }

            var result = new LinkedList<KeyValuePair<T1, T2>>();

            foreach (var folderInfo in foldersInfo)
            {
                if (folderInfo.Key.Parent != null &&
                    foldersInfoIds.Contains(folderInfo.Key.Parent))
                {
                    continue;
                }

                AddSortedChildFolders<KeyValuePair<T1, T2>>(folderInfo, result, (childFolderInfo) =>
                {
                    ICollection<KeyValuePair<T1, T2>> childfolders;
                    if (!foldersWithChildfolders.TryGetValue(childFolderInfo.Key, out childfolders))
                        return null;

                    return childfolders;
                });
            }

            return result;
        }

        private void AddSortedChildFolders<T>(T folderInfo, ICollection<T> result,
            Func<T, ICollection<T>> getChildFolders)
        {
            result.Add(folderInfo);

            var childObjectsInfo = getChildFolders(folderInfo);
            if (childObjectsInfo == null)
                return;

            foreach (var childObjectInfo in childObjectsInfo)
            {
                AddSortedChildFolders(childObjectInfo, result, getChildFolders);
            }
        }

        private void AddFolderStructureObjects(UserFoldersStructure folderStructure,
            DifferentialSubset<IdAndObjectType> folderStructureObjectsId,
            Dictionary<object, PersonDepartmentChange> personDepartmentChanges)
        {
            if (folderStructure == null || folderStructureObjectsId == null)
                return;

            if (folderStructure.UserFoldersStructureObjects == null)
                folderStructure.UserFoldersStructureObjects = new LinkedList<UserFoldersStructureObject>();

            var currentFolderObjects = new Dictionary<IdAndObjectType, UserFoldersStructureObject>();
            foreach (var folderStructureObject in folderStructure.UserFoldersStructureObjects)
            {
                currentFolderObjects.Add(
                    new IdAndObjectType(
                        folderStructureObject.ObjectId,
                        folderStructureObject.ObjectType),
                    folderStructureObject);
            }

            foreach (var folderStructureObjectIdToDelete in folderStructureObjectsId.RemovedElements)
            {
                UserFoldersStructureObject folderStructureObjectToDelete;

                if (currentFolderObjects.TryGetValue(folderStructureObjectIdToDelete, out folderStructureObjectToDelete))
                    folderStructure.UserFoldersStructureObjects.Remove(folderStructureObjectToDelete);
            }

            foreach (var folderStructureObjectIdToAdd in folderStructureObjectsId.AddedElements)
            {
                var folderStructureObject = new UserFoldersStructureObject();
                folderStructureObject.ObjectId = folderStructureObjectIdToAdd.Id.ToString();
                folderStructureObject.ObjectType = folderStructureObjectIdToAdd.ObjectType;

                folderStructure.UserFoldersStructureObjects.Add(folderStructureObject);
            }

            AddPersonDepartmentChanges(
                folderStructureObjectsId.AddedElements,
                folderStructureObjectsId.RemovedElements,
                folderStructure,
                personDepartmentChanges);
        }

        private void AddPersonDepartmentChanges(IEnumerable<IdAndObjectType> addedObjects,
            IEnumerable<IdAndObjectType> removedObjects,
            UserFoldersStructure folderStructure,
            Dictionary<object, PersonDepartmentChange> personDepartmentChanges)
        {
            if (addedObjects != null)
            {
                foreach (var folderStructureObjectIdToAdd in addedObjects)
                {
                    if (folderStructureObjectIdToAdd.ObjectType == ObjectType.Person)
                    {
                        PersonDepartmentChange personDepartmentChange;
                        if (personDepartmentChanges.TryGetValue(folderStructureObjectIdToAdd.Id,
                            out personDepartmentChange))
                        {
                            personDepartmentChange.NewPersonDepartment = folderStructure;
                        }
                        else
                        {
                            personDepartmentChanges.Add(
                                folderStructureObjectIdToAdd.Id,
                                new PersonDepartmentChange(null, folderStructure));
                        }
                    }
                }
            }

            if (removedObjects != null)
            {
                foreach (var folderStructureObjectIdToDelete in removedObjects)
                {
                    if (folderStructureObjectIdToDelete.ObjectType == ObjectType.Person)
                    {
                        PersonDepartmentChange personDepartmentChange;
                        if (personDepartmentChanges.TryGetValue(folderStructureObjectIdToDelete.Id,
                            out personDepartmentChange))
                        {
                            personDepartmentChange.OldPersonDepartment = folderStructure;
                        }
                        else
                        {
                            personDepartmentChanges.Add(
                                folderStructureObjectIdToDelete.Id,
                                new PersonDepartmentChange(folderStructure, null));
                        }
                    }
                }
            }
        }

        public void CreateFolderInternal(LocalSiteInfo localSiteInfo, IdAndObjectType parentFolderId, string name)
        {
            var objectInfo = new LocalObjectInfo(
                Guid.NewGuid(),
                ObjectType.UserFoldersStructure,
                name,
                parentFolderId != null
                    ? GetObjectInfo(parentFolderId)
                    : null,
                Enumerable.Empty<GlobalEvaluator.GlobalObjectInfo>(),
                localSiteInfo);

            AddObjectInfo(objectInfo);

            AddFolderObjectsDifferentialSubset(objectInfo);

            _connector.NotifyObjectAdded(
                CreateObjectInSiteInfo(objectInfo),
                localSiteInfo.Id);
        }

        private void AddFolderObjectsDifferentialSubset(LocalObjectInfo folderInfo)
        {
            lock (_modifiedFolders)
            {
                if (!_modifiedFolders.ContainsKey(folderInfo))
                {
                    _modifiedFolders.Add(folderInfo, CreateFolderObjectsDifferentialSubset(folderInfo));
                }
            }
        }

        private DifferentialSubset<IdAndObjectType> CreateFolderObjectsDifferentialSubset(IdAndObjectType folderId)
        {
            ICollection<UserFoldersStructureObject> folderStructureObjects = null;

            var objectInfo = GetObjectInfo(folderId);
            if (objectInfo != null &&
                objectInfo.GlobalObjectInfo != null)
            {
                var folderStructure = UserFoldersStructures.Singleton.GetById(folderId.Id);
                if (folderStructure != null)
                {
                    folderStructureObjects = folderStructure.UserFoldersStructureObjects;
                }
            }

            DifferentialSubset<IdAndObjectType> folderObjectsDifferentialSubset;
            if (folderStructureObjects != null)
            {
                folderObjectsDifferentialSubset =
                    new DifferentialSubset<IdAndObjectType>(
                        new Subset<IdAndObjectType>(
                            folderStructureObjects.Select(
                                folderStructureObject =>
                                    new IdAndObjectType(folderStructureObject.ObjectId,
                                        folderStructureObject.ObjectType))));
            }
            else
            {
                folderObjectsDifferentialSubset =
                    new DifferentialSubset<IdAndObjectType>(new Subset<IdAndObjectType>());
            }

            return folderObjectsDifferentialSubset;
        }

        public void RemoveFolderInternal(IdAndObjectType folderId)
        {
            var objectInfo = GetObjectInfo(folderId);
            if (objectInfo == null)
                return;

            RemoveObjectInfo(objectInfo);
            AddOrRemoveFolderObjectsDifferentialSubset(objectInfo);
            RemoveSubFolders(folderId);

            _connector.NotifyObjectRemoved(
                folderId,
                objectInfo.SiteInfo.Id);
        }

        private void AddOrRemoveFolderObjectsDifferentialSubset(LocalObjectInfo folderInfo)
        {
            if (folderInfo.GlobalObjectInfo != null)
            {
                AddFolderObjectsDifferentialSubset(folderInfo);
            }
            else
            {
                lock (_modifiedFolders)
                    _modifiedFolders.Remove(folderInfo);
            }
        }

        private void RemoveSubFolders(IdAndObjectType folderId)
        {
            var subFolders = UserFoldersStructures.Singleton.GetSubFolders(folderId.Id);

            if (subFolders != null)
            {
                foreach (var subFolder in subFolders)
                {
                    var subFolderId = new IdAndObjectType(subFolder.IdUserFoldersStructure,
                        ObjectType.UserFoldersStructure);

                    var objectInfo = GetObjectInfo(subFolderId);
                    if (objectInfo == null)
                        continue;

                    RemoveObjectInfo(objectInfo);
                    AddOrRemoveFolderObjectsDifferentialSubset(objectInfo);
                    RemoveSubFolders(subFolderId);
                }
            }
        }

        public void RenameFolderInternal(IdAndObjectType folderId, string newFolderName)
        {
            var objectInfo = GetObjectInfo(folderId);
            if (objectInfo == null)
                return;

            objectInfo.SetLocalName(newFolderName);

            AddFolderObjectsDifferentialSubset(objectInfo);

            _connector.NotifyObjectRenamed(
                folderId,
                newFolderName);
        }

        private readonly Dictionary<LocalObjectInfo, DifferentialSubset<IdAndObjectType>> _modifiedFolders =
            new Dictionary<LocalObjectInfo, DifferentialSubset<IdAndObjectType>>();

        public void AddObjectToFolder(IdAndObjectType objectId, IdAndObjectType folderId)
        {
            var objectInfo = GetObjectInfo(folderId);
            if (objectInfo == null)
                return;

            GetOrAddFolderObjectsDifferentialSubset(objectInfo).Add(objectId);
        }

        private DifferentialSubset<IdAndObjectType> GetOrAddFolderObjectsDifferentialSubset(LocalObjectInfo folderInfo)
        {
            DifferentialSubset<IdAndObjectType> folderObjectsDifferentialSubset;
            lock (_modifiedFolders)
            {
                if (!_modifiedFolders.TryGetValue(folderInfo, out folderObjectsDifferentialSubset))
                {
                    folderObjectsDifferentialSubset = CreateFolderObjectsDifferentialSubset(folderInfo);
                    _modifiedFolders.Add(folderInfo, folderObjectsDifferentialSubset);
                }
            }

            return folderObjectsDifferentialSubset;
        }

        public void RemoveObjectFromFolder(IdAndObjectType objectId, IdAndObjectType folderId)
        {
            var objectInfo = GetObjectInfo(folderId);
            if (objectInfo == null)
                return;

            GetOrAddFolderObjectsDifferentialSubset(objectInfo).Remove(objectId);
        }

#endregion
    }
}
