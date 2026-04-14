using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public partial class GlobalEvaluator : StructuredSiteEvaluator<
        GlobalEvaluator.GlobalObjectInfo,
        GlobalEvaluator.GlobalObjectInfo,
        Subset<GlobalEvaluator.GlobalObjectInfo>,
        Subset<GlobalEvaluator.GlobalObjectInfo>,
        GlobalEvaluator.GlobalSiteInfo,
        Subset<IdAndObjectType>,
        Subset<int>>
    {
        private static readonly HashSet<ObjectType> RelevantObjectTypes =
            new HashSet<ObjectType>
            {
                ObjectType.DailyPlan,
                ObjectType.TimeZone,
                ObjectType.Calendar,
                ObjectType.SecurityDailyPlan,
                ObjectType.DayType,
                ObjectType.Output,
                ObjectType.Input,
                ObjectType.AccessControlList,
                ObjectType.Card,
                ObjectType.CardSystem,
                ObjectType.AlarmArea,
                ObjectType.AntiPassBackZone,
                ObjectType.CCU,
                ObjectType.DCU,
                ObjectType.DoorEnvironment,
                ObjectType.CardReader,
                ObjectType.Person,
                ObjectType.SecurityTimeZone,
                ObjectType.ACLGroup,
                ObjectType.Login,
                ObjectType.PresentationGroup,
                ObjectType.CisNGGroup,
                ObjectType.PresentationFormatter,
                ObjectType.CisNG,
                ObjectType.GlobalAlarmInstruction,
                ObjectType.CardTemplate,
                ObjectType.LoginGroup,
                ObjectType.Scene,
                ObjectType.UserFoldersStructure,
                ObjectType.MultiDoor,
                ObjectType.MultiDoorElement,
                ObjectType.Floor,
                ObjectType.AlarmTransmitter,
                ObjectType.AlarmArc
            };

        public static bool IsObjectTypeRelevantForStructuredSites(ObjectType objectType)
        {
            return RelevantObjectTypes.Contains(objectType);
        }

        public GlobalEvaluator()
            : base(new Subset<IdAndObjectType>())
        {
            _root = new GlobalSiteInfo(-1);

            AddSiteInfo(_root, null);

            var cudRegistered = new AutoResetEvent(false);

#if THREADED_START
            SafeThread<AutoResetEvent>.StartThread(
                new InitOperation(this).Execute,
                cudRegistered);

            cudRegistered.WaitOne();
#else
            new InitOperation(this).Execute(cudRegistered);
#endif

            LocalEvaluatorManager = new LocalEvaluatorManagerType(this);
        }

        private void CUDObjectEvent(ObjectType objectType, object objectId, bool isInserted)
        {
            _accessLock.EnterWriteLock();

            try
            {
                if (isInserted)
                {
                    OnObjectAdded(
                        objectId,
                        objectType);

                    return;
                }

                OnUpdateOrDeleteObject(objectType, objectId);
            }
            finally
            {
                _accessLock.ExitWriteLock();
            }
        }

        private void OnUpdateOrDeleteObject(ObjectType objectType, object objectId)
        {
            if (!RelevantObjectTypes.Contains(objectType))
                return;

            ITableORM tableOrm =
                CgpServerRemotingProvider.Singleton
                    .GetTableOrmForObjectType(objectType);

            if (tableOrm == null)
                return;

            if (BeginUpdate != null)
                BeginUpdate();

            try
            {
                var ormObject = tableOrm.GetObjectById(objectId);

                var idAndObjectType =
                    new IdAndObjectType(
                        objectId,
                        objectType);

                GlobalObjectInfo objectInfo = GetObjectInfo(idAndObjectType);

                if (ormObject == null)
                {
                    if (objectInfo != null)
                        OnObjectDeleteInternal(objectInfo);

                    return;
                }

                if (objectInfo == null)
                {
                    CreateObjectInfo(ormObject);
                    return;
                }

                var newParentOrmObject =
                    tableOrm.GetObjectParent(ormObject);

                var currentParentObjectIdAndObjectType = objectInfo.Parent;

                var newParentObjectIdAndObjectType =
                    newParentOrmObject != null
                        ? new IdAndObjectType(
                            newParentOrmObject.GetId(),
                            newParentOrmObject.GetObjectType())
                        : null;

                if (currentParentObjectIdAndObjectType != null && !currentParentObjectIdAndObjectType.Equals(newParentObjectIdAndObjectType) ||
                    currentParentObjectIdAndObjectType == null && newParentObjectIdAndObjectType != null)
                {
                    var parentObjectInfo =
                        newParentObjectIdAndObjectType != null
                            ? (GetObjectInfo(newParentObjectIdAndObjectType)
                               ?? CreateObjectInfo(newParentOrmObject))
                            : null;

                    ChangeParent(
                        objectInfo,
                        parentObjectInfo);
                }

                var newName = ormObject.ToString();

                if (objectInfo.Name != newName)
                    RenameObject(objectInfo, newName);

                ICollection<AOrmObject> newObjects =
                    new HashSet<AOrmObject>();

                UpdateDirectReferences(
                    tableOrm,
                    objectId,
                    objectInfo,
                    newObjects);

                UpdateBackReferences(
                    tableOrm,
                    objectId,
                    objectInfo,
                    newObjects);

                EvaluateBackReferences(objectInfo);
                EvaluateDirectReferences(objectInfo);

                if (ObjectUpdated != null)
                    ObjectUpdated(objectInfo);

                foreach (var newObject in newObjects)
                    CreateObjectInfo(newObject);
            }
            finally
            {
                if (EndUpdate != null)
                    EndUpdate();
            }
        }

        protected GlobalSiteInfo _root;

        public override GlobalSiteInfo Root
        {
            get { return _root; }
        }

        public int MaxSiteId { get; private set; }

        public Subset<IdAndObjectType> ObjectsWithBrokenDirectReferencesSet
        {
            get { return _objectsWithBrokenDirectReferences; }
        }

        public override IEnumerable<GlobalObjectInfo> GetBackReferences(GlobalObjectInfo objectInfo)
        {
            return objectInfo.BackReferences;
        }

        public override IEnumerable<GlobalObjectInfo> GetChildren(GlobalObjectInfo objectInfo)
        {
            return objectInfo.Children;
        }

        public override IEnumerable<GlobalObjectInfo> GetDirectReferences(GlobalObjectInfo objectInfo)
        {
            return objectInfo.DirectReferences;
        }

        private static void AddReferences(
            GlobalObjectInfo sourceObjectInfo,
            GlobalObjectInfo targetObjectInfo)
        {
            targetObjectInfo.AddBackReference(sourceObjectInfo);
            sourceObjectInfo.AddDirectReference(targetObjectInfo);
        }

        private static bool RemoveReferences(
            GlobalObjectInfo sourceObjectInfo,
            GlobalObjectInfo targetObjectInfo)
        {
            targetObjectInfo.RemoveBackReference(sourceObjectInfo);
            return sourceObjectInfo.RemoveDirectReference(targetObjectInfo);
        }

        protected override void AddBrokenReferenceInternal(
            GlobalObjectInfo sourceObjectInfo,
            GlobalObjectInfo targetObjectInfo)
        {
            targetObjectInfo.AddBrokenBackReference(sourceObjectInfo);

            if (sourceObjectInfo.AddBrokenDirectReference(targetObjectInfo))
                if (DirectBrokenReferencesChanged != null)
                    DirectBrokenReferencesChanged(sourceObjectInfo);
        }

        public ICollection<GlobalSiteInfo> GetTopMostSites(Login login)
        {
            var result = new HashSet<GlobalSiteInfo>();

            AddTopMostSites(
                result, 
                Root, 
                login);

            return result;
        }

        private static void AddTopMostSites(
            HashSet<GlobalSiteInfo> result,
            GlobalSiteInfo siteInfo, 
            Login login)
        {
            foreach (var subTreeReference in siteInfo.SubTreeReferences)
                if (subTreeReference.ObjectType == ObjectType.Login &&
                    subTreeReference.Id.Equals(login.IdLogin))
                {
                    result.Add(siteInfo);
                    return;
                }

            foreach (var childSiteInfo in siteInfo.ChildrenSites)
                AddTopMostSites(
                    result, 
                    childSiteInfo,
                    login);
        }

        protected override void RemoveBrokenReferenceInternal(
            GlobalObjectInfo sourceObjectInfo,
            GlobalObjectInfo targetObjectInfo)
        {
            targetObjectInfo.RemoveBrokenBackReference(sourceObjectInfo);

            if (sourceObjectInfo.RemoveBrokenDirectReference(targetObjectInfo))
                if (DirectBrokenReferencesChanged != null)
                    DirectBrokenReferencesChanged(sourceObjectInfo);
        }

        #region Reference updates

        protected void UpdateBackReferences(
            ITableORM tableOrm,
            object idObj,
            GlobalObjectInfo targetObjectInfo,
            ICollection<AOrmObject> newObjects)
        {
            var backReferences = tableOrm.GetReferencedObjectsAllPlugins(idObj);

            ICollection<GlobalObjectInfo> oldBackReferences =
                new HashSet<GlobalObjectInfo>(GetBackReferences(targetObjectInfo));

            if (backReferences != null)
                foreach (var backReference in backReferences)
                {
                    var objectType = backReference.GetObjectType();

                    if (!RelevantObjectTypes.Contains(objectType))
                        continue;

                    var sourceIdAndObjectType =
                        new IdAndObjectType(
                            backReference.GetId(),
                            objectType);

                    GlobalObjectInfo sourceObjectInfo = GetObjectInfo(sourceIdAndObjectType);

                    if (sourceObjectInfo == null)
                    {
                        newObjects.Add(backReference);
                        continue;
                    }

                    oldBackReferences.Remove(sourceObjectInfo);

                    AddReferences(sourceObjectInfo, targetObjectInfo);
                }

            foreach (var sourceObjectInfo in oldBackReferences)
                if (RemoveReferences(
                    sourceObjectInfo,
                    targetObjectInfo))
                {
                    if (DirectBrokenReferencesChanged != null)
                        DirectBrokenReferencesChanged(sourceObjectInfo);
                }
        }

        protected void UpdateDirectReferences(
            ITableORM tableOrm,
            object idObj,
            GlobalObjectInfo sourceObjectInfo,
            ICollection<AOrmObject> newObjects)
        {
            var directReferences =
                tableOrm.GetDirectReferences(idObj);

            ICollection<GlobalObjectInfo> oldDirectReferences =
                new HashSet<GlobalObjectInfo>(GetDirectReferences(sourceObjectInfo));

            if (directReferences != null)
                foreach (var directReference in directReferences)
                {
                    var objectType = directReference.GetObjectType();

                    if (!RelevantObjectTypes.Contains(objectType))
                        continue;

                    var targetIdAndObjectType =
                        new IdAndObjectType(
                            directReference.GetId(),
                            objectType);

                    GlobalObjectInfo targetObjectInfo = GetObjectInfo(targetIdAndObjectType);

                    if (targetObjectInfo == null)
                    {
                        newObjects.Add(directReference);
                        continue;
                    }

                    oldDirectReferences.Remove(targetObjectInfo);

                    AddReferences(sourceObjectInfo, targetObjectInfo);
                }

            bool removedBrokenDirectReferences = false;

            foreach (var targetObjectInfo in oldDirectReferences)
                if (RemoveReferences(
                    sourceObjectInfo,
                    targetObjectInfo))
                {
                    removedBrokenDirectReferences = true;
                }

            if (removedBrokenDirectReferences && DirectBrokenReferencesChanged != null)
                DirectBrokenReferencesChanged(sourceObjectInfo);
        }

        #endregion

        private class InitOperation
        {
            private readonly GlobalEvaluator _globalEvaluator;

            private readonly IDictionary<GlobalObjectInfo, IdAndObjectType> _parentsForObjects =
                new Dictionary<GlobalObjectInfo, IdAndObjectType>();

            private readonly IDictionary<IdAndObjectType, ICollection<GlobalSiteInfo>> _objectSubtreeReferences =
                new Dictionary<IdAndObjectType, ICollection<GlobalSiteInfo>>();

            public InitOperation(GlobalEvaluator globalEvaluator)
            {
                _globalEvaluator = globalEvaluator;
            }

            public void Execute(AutoResetEvent cudRegistered)
            {
                _globalEvaluator._accessLock.EnterWriteLock();
                try
                {
                    CUDObjectHandlerForServer.Singleton.Register(
                        CUDObjectEvent,
                        RelevantObjectTypes
                            .Concat(Enumerable.Repeat(ObjectType.StructuredSubSiteObject, 1))
                            .ToArray());

                    cudRegistered.Set();

                    CreateSiteInfos();

                    CreateObjectsSitePlacements();

                    foreach (var objectType in RelevantObjectTypes)
                        InitObjectsOfType(objectType);

                    foreach (var objectAndParent in _parentsForObjects)
                        objectAndParent.Key.SetParent(
                            _globalEvaluator.GetObjectInfo(objectAndParent.Value));

                    foreach (var objectInfo in _globalEvaluator.ObjectInfos)
                        _globalEvaluator.EvaluateDirectReferences(objectInfo);
                }
                finally
                {
                    _globalEvaluator._accessLock.ExitWriteLock();
                }
            }

            private void CUDObjectEvent(
                ObjectType objectType,
                object objectId,
                bool isInserted)
            {
                SafeThread<ObjectType, object, bool>.StartThread(
                    _globalEvaluator.CUDObjectEvent,
                    objectType,
                    objectId,
                    isInserted);
            }

            private void CreateSiteInfos()
            {
                var subSites = StructuredSubSites.Singleton.List();

                IDictionary<int, int> parentSiteIds = new Dictionary<int, int>();

                int maxSiteId = 0;

                foreach (var structuredSubSite in subSites)
                {
                    _globalEvaluator.AddSiteInfo(
                        new GlobalSiteInfo(structuredSubSite.IdStructuredSubSite)
                        {
                            Name = structuredSubSite.Name
                        },
                        null);

                    maxSiteId =
                        Math.Max(
                            maxSiteId,
                            structuredSubSite.IdStructuredSubSite);

                    var parentSite = structuredSubSite.ParentSite;

                    parentSiteIds.Add(
                        structuredSubSite.IdStructuredSubSite,
                        parentSite != null
                            ? parentSite.IdStructuredSubSite
                            : -1);
                }

                _globalEvaluator.MaxSiteId = maxSiteId;

                // set the references to the parents
                foreach (var siteInfo in _globalEvaluator.SiteInfos)
                {
                    int siteId = siteInfo.Id;

                    int parentSiteId;

                    if (!parentSiteIds.TryGetValue(siteId, out parentSiteId))
                        continue;

                    GlobalSiteInfo parentSiteInfo = _globalEvaluator.GetSiteInfo(parentSiteId);

                    if (parentSiteInfo == null)
                        continue;

                    siteInfo.ParentSite = parentSiteInfo;
                }
            }

            private void CreateObjectsSitePlacements()
            {
                var structuredSubSiteObjects =
                    StructuredSubSiteObjects.Singleton.List();

                foreach (var structuredSubSiteObject in structuredSubSiteObjects)
                {
                    var subSite = structuredSubSiteObject.StructuredSubSite;

                    var subSiteId =
                        subSite != null
                            ? subSite.IdStructuredSubSite
                            : -1;

                    GlobalSiteInfo siteInfo = _globalEvaluator.GetSiteInfo(subSiteId);

                    var objectType = structuredSubSiteObject.ObjectType;

                    var tableOrm =
                        CgpServerRemotingProvider.Singleton
                            .GetTableOrmForObjectType(objectType);

                    if (tableOrm == null)
                        continue;

                    var objectId = tableOrm.ParseId(structuredSubSiteObject.ObjectId);

                    if (objectId == null)
                        continue;

                    if (structuredSubSiteObject.IsReference)
                        AddSubTreeReference(
                            new IdAndObjectType(
                                objectId,
                                objectType),
                            siteInfo);
                    else
                    {
                        AOrmObject ormObject = tableOrm.GetObjectById(objectId);

                        if (ormObject == null)
                            continue;

                        CreateObjectInfo(
                            objectId,
                            objectType,
                            siteInfo,
                            ormObject,
                            tableOrm.GetObjectParent(ormObject));
                    }
                }
            }

            private GlobalObjectInfo CreateObjectInfo(
                object objectId,
                ObjectType objectType,
                GlobalSiteInfo siteInfo,
                AOrmObject ormObject,
                AOrmObject parentOrmObject)
            {
                var objectInfo =
                    new GlobalObjectInfo(
                        objectId,
                        objectType,
                        siteInfo,
                        ormObject.ToString());

                _globalEvaluator.AddObjectInfo(objectInfo);

                if (parentOrmObject != null)
                    _parentsForObjects.Add(
                        objectInfo,
                        new IdAndObjectType(
                            parentOrmObject.GetId(),
                            parentOrmObject.GetObjectType()));

                ICollection<GlobalSiteInfo> objectSubtreeReferences;

                if (_objectSubtreeReferences.TryGetValue(
                    objectInfo,
                    out objectSubtreeReferences))
                {
                    foreach (var globalSiteInfo in objectSubtreeReferences)
                        objectInfo.AddSubTreeReference(globalSiteInfo);

                    _objectSubtreeReferences.Remove(objectInfo);
                }

                return objectInfo;
            }

            private void AddSubTreeReference(
                IdAndObjectType idAndObjectType,
                GlobalSiteInfo siteInfo)
            {
                var objectInfo = _globalEvaluator.GetObjectInfo(idAndObjectType);

                if (objectInfo != null)
                {
                    objectInfo.AddSubTreeReference(siteInfo);
                    return;
                }

                ICollection<GlobalSiteInfo> objectSubTreeReferences;

                if (!_objectSubtreeReferences.TryGetValue(
                    idAndObjectType,
                    out objectSubTreeReferences))
                {
                    objectSubTreeReferences = new LinkedList<GlobalSiteInfo>();

                    _objectSubtreeReferences.Add(
                        idAndObjectType,
                        objectSubTreeReferences);
                }

                objectSubTreeReferences.Add(siteInfo);
            }

            private void InitObjectsOfType(
                ObjectType objectType)
            {
                var tableOrm =
                    CgpServerRemotingProvider.Singleton.GetTableOrmForObjectType(objectType);

                if (tableOrm == null)
                    return;

                var targetObjects = tableOrm.List();

                foreach (var targetObject in targetObjects)
                    InitObject(
                        targetObject,
                        tableOrm);
            }

            private void InitObject(
                AOrmObject ormObject,
                ITableORM tableOrm)
            {
                var objectId = ormObject.GetId();

                var idAndObjectType =
                    new IdAndObjectType(
                        objectId,
                        ormObject.GetObjectType());

                GlobalObjectInfo objectInfo =
                    _globalEvaluator.GetObjectInfo(idAndObjectType)
                        ?? CreateObjectInfo(
                            objectId,
                            idAndObjectType.ObjectType,
                            _globalEvaluator.Root,
                            ormObject,
                            tableOrm.GetObjectParent(ormObject));

                var sourceObjects =
                    tableOrm.GetReferencedObjectsAllPlugins(objectId);

                if (sourceObjects != null)
                    foreach (var sourceObject in sourceObjects)
                    {
                        var objectType = sourceObject.GetObjectType();

                        if (!RelevantObjectTypes.Contains(objectType))
                            continue;

                        var sourceIdAndObjectType =
                            new IdAndObjectType(
                                sourceObject.GetId(),
                                objectType);

                        GlobalObjectInfo sourceObjectInfo =
                            _globalEvaluator.GetObjectInfo(sourceIdAndObjectType);

                        if (sourceObjectInfo == null)
                            continue;

                        AddReferences(sourceObjectInfo, objectInfo);
                    }

                var targetObjects =
                    tableOrm.GetDirectReferences(objectId);

                if (targetObjects != null)
                    foreach (var targetObject in targetObjects)
                    {
                        var objectType = targetObject.GetObjectType();

                        if (!RelevantObjectTypes.Contains(objectType))
                            continue;

                        var targetIdAndObjectType =
                            new IdAndObjectType(
                                targetObject.GetId(),
                                objectType);

                        GlobalObjectInfo targetObjectInfo =
                            _globalEvaluator.GetObjectInfo(targetIdAndObjectType);

                        if (targetObjectInfo == null)
                            continue;

                        AddReferences(objectInfo, targetObjectInfo);
                    }
            }
        }

        #region CUD

        private void OnObjectDeleteInternal(
            GlobalObjectInfo objectInfo)
        {
            var childObjects = 
                new LinkedList<GlobalObjectInfo>(
                    objectInfo.Children);

            foreach (var childObjectInfo in childObjects)
                OnObjectDeleteInternal(childObjectInfo);

            var subTreeReferenceSites =
                new LinkedList<int>(
                    objectInfo.SubTreeReferenceSites);

            foreach (var subTreeReferenceSiteId in subTreeReferenceSites)
            {
                var siteInfo = GetSiteInfo(subTreeReferenceSiteId);

                if (siteInfo == null)
                    continue;

                RemoveSubTreeReference(
                    objectInfo,
                    siteInfo);
            }

            RemoveObjectInfo(objectInfo);

            foreach (var directReference in GetDirectReferences(objectInfo))
                directReference.RemoveBackReference(objectInfo);

            foreach (var backReference in GetBackReferences(objectInfo))
                if (backReference.RemoveDirectReference(objectInfo))
                    if (DirectBrokenReferencesChanged != null)
                        DirectBrokenReferencesChanged(backReference);

            if (ObjectDeleted != null)
                ObjectDeleted(objectInfo);
        }

        public void OnObjectAdded(
            object objectId,
            ObjectType objectType)
        {
            if (!RelevantObjectTypes.Contains(objectType))
            {
                if (objectType == ObjectType.StructuredSubSiteObject)
                    OnObjectPlacementCreated(objectId);

                return;
            }

            if (BeginUpdate != null)
                BeginUpdate();

            try
            {
                var tableOrm =
                    CgpServerRemotingProvider.Singleton
                        .GetTableOrmForObjectType(objectType);

                if (tableOrm == null)
                    return;

                var ormObject = tableOrm.GetObjectById(objectId);

                if (ormObject == null)
                    return;

                CreateObjectInfo(
                    objectId,
                    objectType,
                    ormObject,
                    () => GetObjectPlacement(objectType, objectId),
                    tableOrm);
            }
            finally
            {
                if (EndUpdate != null)
                    EndUpdate();
            }
        }

        private GlobalObjectInfo CreateObjectInfo(AOrmObject ormObject)
        {
            var objectType = ormObject.GetObjectType();

            ITableORM tableOrm =
                CgpServerRemotingProvider.Singleton
                    .GetTableOrmForObjectType(objectType);

            if (tableOrm == null)
                return null;

            var objectId = ormObject.GetId();

            return
                CreateObjectInfo(
                    objectId,
                    objectType,
                    ormObject,
                    () => GetObjectPlacement(objectType, objectId),
                    tableOrm);
        }

        private GlobalObjectInfo CreateObjectInfo(
            object objectId,
            ObjectType objectType,
            AOrmObject ormObject,
            Func<StructuredSubSiteObject> getObjectPlacement,
            ITableORM tableOrm)
        {
            GlobalSiteInfo siteInfo;

            AOrmObject parentOrmObject = tableOrm.GetObjectParent(ormObject);

            GlobalObjectInfo parent;

            var structuredSubSiteObject = getObjectPlacement();

            if (parentOrmObject != null)
            {
                parent =
                    GetObjectInfo(
                        new IdAndObjectType(
                            parentOrmObject.GetId(),
                            parentOrmObject.GetObjectType()))
                    ?? CreateObjectInfo(parentOrmObject);

                siteInfo = parent.SiteInfo;

                var siteId = siteInfo.Id;

                if (structuredSubSiteObject == null &&
                    siteId != -1)
                {
                    StructuredSubSites.Singleton.AddObjectToSite(
                        ormObject.GetId(),
                        ormObject.GetObjectType(),
                        siteId);
                }
            }
            else
            {
                parent = null;

                var structuredSubSite =
                    structuredSubSiteObject != null
                        ? structuredSubSiteObject.StructuredSubSite
                        : null;

                int idStructuredSubSite =
                    structuredSubSite != null
                        ? structuredSubSite.IdStructuredSubSite
                        : -1;

                siteInfo =
                    GetSiteInfo(idStructuredSubSite) ??
                    CreateSiteInfo(structuredSubSite);
            }

            var newObjectInfo =
                new GlobalObjectInfo(
                    objectId,
                    objectType,
                    siteInfo,
                    ormObject.ToString());

            newObjectInfo.SetParent(parent);

            AddObjectInfo(newObjectInfo);

            ICollection<AOrmObject> newObjects =
                new HashSet<AOrmObject>();

            UpdateDirectReferences(
                tableOrm,
                objectId,
                newObjectInfo,
                newObjects);

            UpdateBackReferences(
                tableOrm,
                objectId,
                newObjectInfo,
                newObjects);

            EvaluateBackReferences(newObjectInfo);
            EvaluateDirectReferences(newObjectInfo);

            if (ObjectAdded != null)
                ObjectAdded(newObjectInfo);

            if (parent != null)
                foreach (var subTreeReferenceSiteId in parent.SubTreeReferenceSites)
                {
                    var subTreeReferenceSiteInfo = GetSiteInfo(subTreeReferenceSiteId);

                    if (subTreeReferenceSiteInfo == null)
                        continue;

                    AddSubTreeReference(
                        newObjectInfo,
                        subTreeReferenceSiteInfo);
                }

            foreach (var newObject in newObjects)
                CreateObjectInfo(newObject);

            return newObjectInfo;
        }

        private static StructuredSubSiteObject GetObjectPlacement(
            ObjectType objectType,
            object objectId)
        {
            return
                StructuredSubSiteObjects.Singleton.FindStructuredSubSiteObject(
                    objectType,
                    objectId.ToString());
        }

        private readonly ReaderWriterLockSlim _accessLock =
            new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private void OnObjectPlacementCreated(object idStructuredSubSiteObject)
        {
            var structuredSubSiteObject =
                StructuredSubSiteObjects.Singleton
                    .GetById(idStructuredSubSiteObject);

            if (structuredSubSiteObject == null)
                return;

            var objectType = structuredSubSiteObject.ObjectType;

            bool isLoginOrGroupReference;

            if (structuredSubSiteObject.IsReference)
            {
                if (objectType != ObjectType.Login &&
                    objectType != ObjectType.LoginGroup)
                {
                    return;
                }

                isLoginOrGroupReference = true;
            }
            else
            {
                if (objectType == ObjectType.Login ||
                    objectType == ObjectType.LoginGroup)
                {
                    return;
                }

                isLoginOrGroupReference = false;
            }

            var tableOrm =
                CgpServerRemotingProvider.Singleton
                    .GetTableOrmForObjectType(objectType);

            if (tableOrm == null)
                return;

            var objectId = tableOrm.ParseId(structuredSubSiteObject.ObjectId);

            if (objectId == null)
                return;

            var idAndObjectType =
                new IdAndObjectType(
                    objectId,
                    objectType);

            var objectInfo = GetObjectInfo(idAndObjectType);

            if (objectInfo == null)
            {
                var ormObject = tableOrm.GetObjectById(objectId);

                if (ormObject == null)
                    return;

                if (!isLoginOrGroupReference)
                {
                    CreateObjectInfo(
                        objectId,
                        objectType,
                        ormObject,
                        () => structuredSubSiteObject,
                        tableOrm);

                    return;
                }

                objectInfo =
                    CreateObjectInfo(
                        objectId,
                        objectType,
                        ormObject,
                        () => null,
                        tableOrm);
            }

            var structuredSubSite = structuredSubSiteObject.StructuredSubSite;

            int idStructuredSubSite =
                structuredSubSite != null
                    ? structuredSubSite.IdStructuredSubSite
                    : -1;

            var siteInfo =
                GetSiteInfo(idStructuredSubSite) ??
                CreateSiteInfo(structuredSubSite);

            if (isLoginOrGroupReference)
            {
                AddSubTreeReferenceAndChildren(
                    objectInfo,
                    siteInfo);

                return;
            }

            if (siteInfo.Id != objectInfo.SiteInfo.Id)
                MoveObjectAndChildren(
                    objectInfo,
                    siteInfo);
        }

        private void MoveObjectAndChildren(GlobalObjectInfo objectInfo, GlobalSiteInfo siteInfo)
        {
            var oldSiteInfo = objectInfo.SiteInfo;

            new MoveOperation(
                this,
                objectInfo,
                siteInfo).Execute();

            if (ObjectMoved != null)
                ObjectMoved(
                    objectInfo,
                    oldSiteInfo);

            foreach (var child in objectInfo.Children)
                MoveObjectAndChildren(
                    child,
                    siteInfo);
        }

        private GlobalSiteInfo CreateSiteInfo(
            StructuredSubSite structuredSubSite)
        {
            if (structuredSubSite == null)
                return _root;

            var idStructuredSubSite = structuredSubSite.IdStructuredSubSite;
            var siteInfo = GetSiteInfo(idStructuredSubSite);

            MaxSiteId =
                Math.Max(
                    MaxSiteId,
                    idStructuredSubSite);

            if (siteInfo != null)
                return siteInfo;

            var parentStructuredSubSite = structuredSubSite.ParentSite;

            siteInfo =
                new GlobalSiteInfo(idStructuredSubSite)
                {
                    Name = structuredSubSite.Name,
                };

            AddSiteInfo(
                siteInfo,
                parentStructuredSubSite != null
                    ? (GetSiteInfo(parentStructuredSubSite.IdStructuredSubSite) ??
                       CreateSiteInfo(parentStructuredSubSite))
                    : _root);

            if (SiteCreated != null)
                SiteCreated(siteInfo);

            return siteInfo;
        }

        private void ChangeParent(
            GlobalObjectInfo objectInfo,
            GlobalObjectInfo parentObjectInfo)
        {
            var oldObjectParent = objectInfo.Parent;

            objectInfo.SetParent(parentObjectInfo);

            MoveObjectToSiteOfParent(
                objectInfo,
                parentObjectInfo);

            if (ObjectParentChanged != null)
                ObjectParentChanged(
                    objectInfo,
                    oldObjectParent);
        }

        private void RenameObject(
            GlobalObjectInfo objectInfo,
            string newName)
        {
            objectInfo.SetName(newName);

            if (ObjectRenamed != null)
                ObjectRenamed(objectInfo);
        }

        private void MoveObjectToSiteOfParent(
            GlobalObjectInfo objectInfo,
            GlobalObjectInfo parentObjectInfo)
        {
            if (parentObjectInfo == null)
                return;

            var parentSiteInfo = parentObjectInfo.SiteInfo;

            if (objectInfo.SiteInfo.Id != parentSiteInfo.Id)
            {
                MoveObjectAndChildren(
                    objectInfo,
                    parentSiteInfo);
            }

            foreach (var subTreeReferenceSiteId in parentObjectInfo.SubTreeReferenceSites)
            {
                var siteInfo = GetSiteInfo(subTreeReferenceSiteId);

                if (siteInfo == null)
                    continue;

                AddSubTreeReferenceAndChildren(
                    objectInfo,
                    siteInfo);
            }
        }

        protected override void OnSubTreeReferenceAdded(
            GlobalObjectInfo objectInfo,
            GlobalSiteInfo siteInfo)
        {
            var structuredSubSiteObject =
                new StructuredSubSiteObject
                {
                    ObjectId = objectInfo.Id.ToString(),
                    ObjectType = objectInfo.ObjectType,
                    IsReference = true,
                    StructuredSubSite = StructuredSubSites.Singleton.GetById(siteInfo.Id)
                };

            StructuredSubSiteObjects.Singleton.Insert(ref structuredSubSiteObject);

            if (SubTreeReferenceAdded != null)
                SubTreeReferenceAdded(
                    objectInfo,
                    siteInfo);
        }

        protected override void OnSubTreeReferenceRemoved(
            GlobalObjectInfo objectInfo,
            GlobalSiteInfo siteInfo)
        {
            StructuredSubSiteObjects.Singleton.DeleteStructuredSubSiteObjectReference(
                objectInfo.ObjectType,
                objectInfo.Id.ToString(),
                siteInfo.Id);

            if (SubTreeReferenceRemoved != null)
                SubTreeReferenceRemoved(
                    objectInfo,
                    siteInfo);
        }

        private void AddSubTreeReferenceAndChildren(
            GlobalObjectInfo objectInfo,
            GlobalSiteInfo siteInfo)
        {
            AddSubTreeReference(
                objectInfo,
                siteInfo);

            foreach (var child in objectInfo.Children)
                AddSubTreeReferenceAndChildren(
                    child,
                    siteInfo);
        }

        #endregion

        public int? SaveCreateSite(string name, int parentSiteId)
        {
            var parentSiteInfo = GetSiteInfo(parentSiteId);

            if (parentSiteInfo == null)
                return null;

            var structuredSubSite =
                new StructuredSubSite
                {
                    Name = name,
                    ParentSite = StructuredSubSites.Singleton.GetById(parentSiteId)
                };

            if (!StructuredSubSites.Singleton.Insert(
                ref structuredSubSite))
            {
                return null;
            }

            return CreateSiteInfo(structuredSubSite).Id;
        }

        public void SaveUpdateSite(
            GlobalSiteInfo siteInfo,
            string name)
        {
            var structuredSubSite =
                StructuredSubSites.Singleton
                    .GetObjectForEdit(siteInfo.Id);

            try
            {

                structuredSubSite.Name = name;

                StructuredSubSites.Singleton.Update(structuredSubSite);
            }
            finally
            {
                StructuredSubSites.Singleton.EditEnd(structuredSubSite);
            }

            siteInfo.Name = name;

            if (SiteUpdated != null)
                SiteUpdated(siteInfo);
        }

        public void SaveMoveObject(
            IdAndObjectType idAndObjectType,
            GlobalSiteInfo newSiteInfo)
        {
            var objectInfo = GetObjectInfo(idAndObjectType);

            if (objectInfo == null)
                return;

            var oldSiteInfo = objectInfo.SiteInfo;

            var movePerformed =
                new MoveOperation(
                    this,
                    objectInfo,
                    newSiteInfo).Execute();

            if (!movePerformed)
                return;

            var newSiteId = newSiteInfo.Id;

            var objectId = objectInfo.Id;
            var objectType = objectInfo.ObjectType;

            if (oldSiteInfo.Id == -1)
                StructuredSubSites.Singleton.AddObjectToSite(
                    objectId,
                    objectType,
                    newSiteId);
            else
                if (newSiteId == -1)
                    StructuredSubSites.MoveObjectToRoot(
                        objectId,
                        objectType);
                else
                    StructuredSubSites.Singleton.MoveObjectToSite(
                        objectId,
                        objectType,
                        newSiteId);

            if (ObjectMoved != null)
                ObjectMoved(
                    objectInfo,
                    oldSiteInfo);
        }

        public GlobalObjectInfo SaveCreateFolder(UserFoldersStructure folderStructure, object folderStructureId)
        {
            UserFoldersStructures.Singleton.InsertWithObjectId(ref folderStructure, folderStructureId);
            return CreateObjectInfo(folderStructure);
        }

        public void SaveUpdateFolder(UserFoldersStructure folderStructure)
        {
            UserFoldersStructures.Singleton.Update(folderStructure);
        }

        public bool SaveDeleteFolder(IdAndObjectType folderStructureId)
        {
            return UserFoldersStructures.Singleton.DeleteById(folderStructureId.Id);
        }

        private event Action<GlobalObjectInfo> ObjectAdded;
        private event Action<GlobalObjectInfo> ObjectUpdated;
        private event Action<IdAndObjectType> ObjectDeleted;
        private event Action<GlobalObjectInfo, GlobalSiteInfo> ObjectMoved;

        private event Action<GlobalObjectInfo, IdAndObjectType> ObjectParentChanged;
        private event Action<GlobalObjectInfo> ObjectRenamed;

        private event Action<GlobalObjectInfo, GlobalSiteInfo> SubTreeReferenceAdded;
        private event Action<GlobalObjectInfo, GlobalSiteInfo> SubTreeReferenceRemoved;

        private event Action<GlobalObjectInfo> DirectBrokenReferencesChanged;

        private event Action<GlobalSiteInfo> SiteCreated;
        private event Action<GlobalSiteInfo> SiteUpdated;

        private event DVoid2Void BeginUpdate;
        private event DVoid2Void EndUpdate;

        private event DVoid2Void UserFoldersStructureChanged;

        public LocalEvaluatorManagerType LocalEvaluatorManager { get; private set; }

        private event Action<GlobalSiteInfo> SiteRemoved;

        public bool SaveRemoveSite(GlobalSiteInfo globalSiteInfo)
        {
            if (!StructuredSubSites.Singleton.DeleteById(globalSiteInfo.Id))
                return false;

            var subTreeReferences =
                new LinkedList<IdAndObjectType>(globalSiteInfo.SubTreeReferences);

            foreach (var idAndObjectType in subTreeReferences)
            {
                var objectInfo = GetObjectInfo(idAndObjectType);

                if (objectInfo == null)
                    continue;

                RemoveSubTreeReference(
                    objectInfo,
                    globalSiteInfo);
            }

            RemoveSiteInfo(globalSiteInfo);

            if (SiteRemoved != null)
                SiteRemoved(globalSiteInfo);

            return true;
        }

        public IEnumerable<ObjectType> GetVisibleObjectTypes(Login login)
        {
            foreach (var relevantObjectType in RelevantObjectTypes)
            {
                var tableOrm =
                    CgpServerRemotingProvider.Singleton
                        .GetTableOrmForObjectType(relevantObjectType);

                if (tableOrm == null)
                    continue;

                if (tableOrm.HasAccessView(login))
                    yield return relevantObjectType;
            }
        }

        private static bool IsObjectVisibleFromSite(
            ICollection<GlobalSiteInfo> topMostSites,
            GlobalSiteInfo currentSiteInfo)
        {

            do
            {
                if (topMostSites.Contains(currentSiteInfo))
                    return true;

                currentSiteInfo = currentSiteInfo.ParentSite;
            }
            while (currentSiteInfo != null);

            return false;
        }

        public bool AreObjectsVisibleForLogin(
            IEnumerable<IdAndObjectType> objectGroup,
            ICollection<GlobalSiteInfo> topMostSites,
            bool referencesAllowed)
        {
            if (objectGroup == null)
                return false;

            var objectInfos =
                new LinkedList<GlobalObjectInfo>(
                    objectGroup
                        .Select(
                            idAndObjectType =>
                                GetObjectInfo(idAndObjectType))
                        .Where(
                            globalObjectInfo =>
                                globalObjectInfo != null));

            if (referencesAllowed && 
                objectInfos.Any(
                    objectInfo =>
                        AncestralReferenceExists(
                            objectInfo,
                            topMostSites)))
            {
                return true;
            }

            foreach (var objectInfo in objectInfos)
            {
                var objectType = objectInfo.ObjectType;

                if (objectType != ObjectType.Login &&
                    objectType != ObjectType.LoginGroup)
                {
                    if (IsObjectVisibleFromSite(
                        topMostSites,
                        objectInfo.SiteInfo))
                    {
                        return true;
                    }

                    continue;
                }

                foreach (var subTreeReferenceSiteId in objectInfo.SubTreeReferenceSites)
                {
                    var siteInfo = GetSiteInfo(subTreeReferenceSiteId);

                    if (siteInfo != null &&
                        IsObjectVisibleFromSite(topMostSites, siteInfo))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public IEnumerable<T> GetObjectsFromSite<T>(
            GlobalSiteInfo siteInfo,
            IEnumerable<T> objects)
            where T : AOrmObject
        {
            if (siteInfo == null ||
                objects == null)
            {
                yield break;
            }

            foreach (var ormObject in objects)
            {
                var objectInfo = GetObjectInfo(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()));

                if (objectInfo == null)
                    continue;

                if (siteInfo.Id == objectInfo.SiteInfo.Id)
                    yield return ormObject;
            }
        }

        public IEnumerable<T> GetObjectsWhithSubTreeReferencesInSites<T>(
            IEnumerable<GlobalSiteInfo> siteInfos,
            IEnumerable<T> objects,
            Func<object, bool> forcedAddingObject)
            where T : AOrmObject
        {
            if (siteInfos == null ||
                objects == null)
            {
                yield break;
            }

            var allReferencesForSites = new HashSet<IdAndObjectType>(
                siteInfos.SelectMany(
                    globalSiteInfo =>
                        globalSiteInfo.SubTreeReferences));

            foreach (var ormObject in objects)
            {
                if (forcedAddingObject != null
                    && forcedAddingObject(ormObject.GetId()))
                {
                    yield return ormObject;
                    continue;
                }

                var objectInfo = GetObjectInfo(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()));

                if (objectInfo == null)
                    continue;

                if (allReferencesForSites.Contains(objectInfo))
                    yield return ormObject;
            }
        }

        public GlobalSiteInfo GetGlobalSiteInfoOfObject(object objectId, ObjectType objectType)
        {
            if (objectId == null)
                return null;

            GlobalObjectInfo objectInfo = GetObjectInfo(
                new IdAndObjectType(
                    objectId,
                    objectType));

            return objectInfo != null
                ? objectInfo.SiteInfo
                : null;
        }

        public IEnumerable<T> GetObjectsFromSameSiteAndParentSites<T>(
            GlobalSiteInfo baseObjectSiteInfo,
            IEnumerable<T> objects)
            where T : AOrmObject
        {
            if (baseObjectSiteInfo == null ||
                objects == null)
            {
                yield break;
            }

            foreach (var ormObject in objects)
            {
                var objectInfo = GetObjectInfo(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()));

                if (objectInfo == null)
                    continue;

                while (baseObjectSiteInfo != null)
                {
                    if (baseObjectSiteInfo.Equals(objectInfo.SiteInfo))
                    {
                        yield return ormObject;
                        break;
                    }

                    baseObjectSiteInfo = baseObjectSiteInfo.ParentSite;
                }
            }
        }

        public IEnumerable<T> GetObjectsFromSameSiteAndChildSites<T>(
            GlobalSiteInfo baseObjectSiteInfo,
            IEnumerable<T> objects)
            where T : AOrmObject
        {
            if (baseObjectSiteInfo == null ||
                objects == null)
            {
                yield break;
            }

            foreach (var ormObject in objects)
            {
                var objectInfo = GetObjectInfo(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()));

                if (objectInfo == null)
                    continue;

                var objectSiteInfo = objectInfo.SiteInfo;
                while (objectSiteInfo != null)
                {
                    if (objectSiteInfo.Equals(baseObjectSiteInfo))
                    {
                        yield return ormObject;
                        break;
                    }

                    objectSiteInfo = objectSiteInfo.ParentSite;
                }
            }
        }

        public IEnumerable<GlobalSiteInfo> GetGlobalSiteInfosOfObjectReferences(object objectId, ObjectType objectType)
        {
            if (objectId == null)
                yield break;

            GlobalObjectInfo objectInfo = GetObjectInfo(
                new IdAndObjectType(
                    objectId,
                    objectType));

            if (objectInfo == null
                || objectInfo.SubTreeReferenceSites == null)
            {
                yield break;
            }

            foreach (var siteId in objectInfo.SubTreeReferenceSites)
            {
                yield return GetSiteInfo(siteId);
            }
        }

        public void AddEventOnObjectMoved(Action<GlobalObjectInfo, GlobalSiteInfo> onObjectMoved)
        {
            _accessLock.EnterWriteLock();

            try
            {
                ObjectMoved += onObjectMoved;
            }
            finally 
            {
                _accessLock.ExitWriteLock();
            }
        }

        public void RemoveEventOnObjectMoved(Action<GlobalObjectInfo, GlobalSiteInfo> onObjectMoved)
        {
            _accessLock.EnterWriteLock();

            try
            {
                ObjectMoved -= onObjectMoved;
            }
            finally
            {
                _accessLock.ExitWriteLock();
            }
        }

        public void AddEventOnObjectAdded(Action<GlobalObjectInfo> onObjectAdded)
        {
            _accessLock.EnterWriteLock();

            try
            {
                ObjectAdded += onObjectAdded;
            }
            finally
            {
                _accessLock.ExitWriteLock();
            }
        }

        public void RemoveEventOnObjectAdded(Action<GlobalObjectInfo> onObjectAdded)
        {
            _accessLock.EnterWriteLock();

            try
            {
                ObjectAdded -= onObjectAdded;
            }
            finally
            {
                _accessLock.ExitWriteLock();
            }
        }

        #region FolderStructure

        public void UserFoldersStructureWasChanged()
        {
            _accessLock.EnterWriteLock();

            try
            {
                if (UserFoldersStructureChanged != null)
                {
                    UserFoldersStructureChanged();
                }
            }
            finally
            {
                _accessLock.ExitWriteLock();
            }
        }

        #endregion
    }
}