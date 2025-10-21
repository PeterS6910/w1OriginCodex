using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

using JetBrains.Annotations;
using Contal.IwQuick.CrossPlatform;

namespace Contal.Cgp.NCAS.Server.CcuDataReplicator
{
    public sealed partial class DataReplicationManager :
        ASingleton<DataReplicationManager>,
        IImportEventHandler
    {
        private interface IRequest
        {
            IEnumerable<Guid> Execute(
                IDictionary<IdAndObjectType, HashSet<Guid>> idCcusCacheForObjects,
                Dictionary<IdAndObjectType, ICollection<AOrmObject>> referencedObjectsCache);
        }

        private class AddOrUpdateObjectsRequest :
            IRequest
        {
            [NotNull]
            private ICollection<AOrmObjectWithVersion> _editedObjects;

            private IDictionary<IdAndObjectType, HashSet<Guid>> _cacheOfParentCcuIdsForObjects;
            private IDictionary<IdAndObjectType, ICollection<AOrmObject>> _cacheOfReferencedObjects;

            public AddOrUpdateObjectsRequest(
                [NotNull] ICollection<AOrmObjectWithVersion> ormObjects)
            {
                _editedObjects = ormObjects;
            }

            private void IncrementObjectReferenceCountForCCUs(
                AOrmObjectWithVersion ormObject,
                Guid parentCcuId,
                bool isEditedObject)
            {
                CcuDataReplicator ccuDataReplicator;

                if (!Singleton._ccuDataReplicators.TryGetValue(
                        parentCcuId,
                        out ccuDataReplicator))
                {
                    return;
                }

                IEnumerable<AOrmObject> referencedObjects;

                if (!ccuDataReplicator.UpdateObjectInfo(
                        ormObject,
                        _cacheOfParentCcuIdsForObjects,
                        _cacheOfReferencedObjects,
                        out referencedObjects)
                    && !isEditedObject)
                {
                    return;
                }

                var idAndObjectType =
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType());

                foreach (var referencedObject in referencedObjects)
                {
                    IncrementObjectReferenceCountForCCUs(
                        (AOrmObjectWithVersion)referencedObject,
                        parentCcuId,
                        false);
                }
            }

            public IEnumerable<Guid> Execute(
                IDictionary<IdAndObjectType, HashSet<Guid>> idCcusCacheForObjects,
                Dictionary<IdAndObjectType, ICollection<AOrmObject>> referencedObjectsCache)
            {
                try
                {
                    _cacheOfParentCcuIdsForObjects = idCcusCacheForObjects;
                    _cacheOfReferencedObjects = referencedObjectsCache;

                    var targetCcuIds = new HashSet<Guid>();

                    foreach (var editedObject in _editedObjects)
                    {
                        var idAndObjectType = new IdAndObjectType(
                            editedObject.GetId(),
                            editedObject.GetObjectType());

                        var oldParentCcuIds =
                            new LinkedList<Guid>(
                                Singleton.GetIdParentCcusFromObjectInfos(idAndObjectType));

                        var currentParentCcuIds =
                            idAndObjectType.ObjectType != ObjectType.CardSystem
                            && Singleton.IsObjectTypeWithParentCcuIdsFromReferences(idAndObjectType.ObjectType)
                                ? oldParentCcuIds
                                : Singleton.GetExplicitIdParentCcus(
                                    idAndObjectType,
                                    _cacheOfParentCcuIdsForObjects);

                        foreach (var idCcu in oldParentCcuIds)
                        {
                            Singleton.DecrementObjectReferenceCountForCCU(
                                idCcu,
                                idAndObjectType,
                                true);

                            targetCcuIds.Add(idCcu);
                        }

                        foreach (var parentCcuId in currentParentCcuIds)
                        {
                            IncrementObjectReferenceCountForCCUs(
                                editedObject,
                                parentCcuId,
                                true);

                            targetCcuIds.Add(parentCcuId);
                        }
                    }

                    return targetCcuIds.Count != 0
                        ? targetCcuIds
                        : null;
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                }

                return null;
            }
        }

        private class DeleteObjectsRequest :
            IRequest
        {
            [NotNull]
            private IdAndObjectType _objectToDelete;

            public DeleteObjectsRequest(
                [NotNull] IdAndObjectType objectToDelete)
            {
                _objectToDelete = objectToDelete;
            }

            public IEnumerable<Guid> Execute(
                IDictionary<IdAndObjectType, HashSet<Guid>> idCcusCacheForObjects,
                Dictionary<IdAndObjectType, ICollection<AOrmObject>> referencedObjectsCache)
            {
                var idTargetCcus = new HashSet<Guid>();

                try
                {
                    var idCcus =
                        new HashSet<Guid>(
                            Singleton.GetIdParentCcusFromObjectInfos(_objectToDelete));

                    foreach (var idCcu in idCcus)
                    {
                        Singleton.DecrementObjectReferenceCountForCCU(
                            idCcu,
                            _objectToDelete,
                            true);

                        idTargetCcus.Add(idCcu);
                    }

                    return idTargetCcus;
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                }

                return null;
            }
        }

        private class CreateCcuDataBatchExecutor : ABatchExecutor<IRequest, DataReplicationManager>
        {
            Dictionary<IdAndObjectType, HashSet<Guid>> _idCcusCacheForObjects;
            Dictionary<IdAndObjectType, ICollection<AOrmObject>> _referencedObjectsCache;
            HashSet<Guid> _idTargetCcus;

            public CreateCcuDataBatchExecutor(DataReplicationManager dataReplicationManager)
                : base(dataReplicationManager)
            {
            }

            protected override bool BeforeBatch(DataReplicationManager dataReplicationManager)
            {
                if (!dataReplicationManager._suspended)
                {
                    _idCcusCacheForObjects = new Dictionary<IdAndObjectType, HashSet<Guid>>();
                    _referencedObjectsCache = new Dictionary<IdAndObjectType, ICollection<AOrmObject>>();
                    _idTargetCcus = new HashSet<Guid>();

                    return true;
                }

                return false;
            }

            protected override void ExecuteInternal(
                IRequest request,
                DataReplicationManager dataReplicationManager)
            {
                IEnumerable<Guid> currentTargetCcus;

                lock (dataReplicationManager._ccuDataReplicators)
                {
                    currentTargetCcus = request.Execute(
                        _idCcusCacheForObjects,
                        _referencedObjectsCache);
                }

                if (currentTargetCcus == null)
                    return;

                foreach (var currentTargetCcu in currentTargetCcus)
                {
                    _idTargetCcus.Add(currentTargetCcu);
                }
            }

            protected override bool OnError(
                IRequest request,
                Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return true;
            }

            protected override void AfterBatch(DataReplicationManager dataReplicationManager)
            {
                dataReplicationManager.CreateAndEnqueueCcuDataBatches(_idTargetCcus);

                _idCcusCacheForObjects = null;
                _referencedObjectsCache = null;
                _idTargetCcus = null;
            }
        }

        private BatchWorker<IRequest> _sendModifiedObjectToCcusBatchWorker;

        private static void GetExplicitIdParentCcus(
            IdAndObjectType idAndObjectType,
            HashSet<Guid> idCcus)
        {
            var id = (Guid)idAndObjectType.Id;

            switch (idAndObjectType.ObjectType)
            {
                case ObjectType.Output:

                    Outputs.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.Input:

                    Inputs.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.ACLSetting:

                    ACLSettings.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.ACLPerson:

                    ACLPersons.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.ACLSettingAA:

                    ACLSettingAAs.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.AccessControlList:

                    AccessControlLists.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.AlarmArea:

                    AlarmAreas.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.AccessZone:

                    AccessZones.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.CCU:

                    CCUs.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.AAInput:

                    AAInputs.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.DCU:

                    DCUs.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.DoorEnvironment:

                    DoorEnvironments.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.CardReader:

                    CardReaders.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.AACardReader:

                    AACardReaders.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.DevicesAlarmSetting:

                    var allCCUs = CCUs.Singleton.List();

                    foreach (var ccu in allCCUs)
                        if (ccu != null
                            && idCcus != null
                            && !idCcus.Contains(ccu.IdCCU))
                        {
                            idCcus.Add(ccu.IdCCU);
                        }

                    return;

                case ObjectType.AntiPassBackZone:

                    AntiPassBackZones.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.MultiDoor:

                    MultiDoors.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.MultiDoorElement:

                    MultiDoorElements.Singleton.GetParentCCU(
                        idCcus,
                        id);

                    return;

                case ObjectType.CardSystem:

                    var cardSystem = CardSystems.Singleton.GetById(id);

                    if (cardSystem == null)
                        return;

                    if (cardSystem.ExplicitSmartCardDataPopulation)
                    {
                        foreach (var idCcu in CCUs.Singleton.List()
                            .Select(ccu => ccu.IdCCU))
                        {
                            idCcus.Add(idCcu);
                        }

                        return;
                    }

                    if (cardSystem.Cards == null)
                        return;

                    foreach (var card in cardSystem.Cards)
                    {
                        var parentCcuIds = Singleton.GetIdParentCcusFromObjectInfos(
                            new IdAndObjectType(
                                card.IdCard,
                                card.GetObjectType()));

                        foreach (var parentCcuId in parentCcuIds)
                        {
                            idCcus.Add(parentCcuId);
                        }
                    }

                    return;
            }
        }

        private bool IsObjectTypeWithParentCcuIdsFromReferences(ObjectType objectType)
        {
            return _objectTypesWithParentCcuIdsFromReferences.Contains(objectType);
        }

        [NotNull]
        private IEnumerable<Guid> GetExplicitIdParentCcus(
            IdAndObjectType idAndObjectType,
            IDictionary<IdAndObjectType, HashSet<Guid>> cacheOfParentCcuIdsForObjects)
        {
            HashSet<Guid> parentCcuIds;

            if (cacheOfParentCcuIdsForObjects.TryGetValue(
                    idAndObjectType,
                    out parentCcuIds))
            {
                return parentCcuIds;
            }

            parentCcuIds = new HashSet<Guid>();

            GetExplicitIdParentCcus(
                idAndObjectType,
                parentCcuIds);

            cacheOfParentCcuIdsForObjects.Add(
                idAndObjectType,
                parentCcuIds);

            return parentCcuIds;
        }

        private readonly Dictionary<Guid, CcuDataReplicator>
            _ccuDataReplicators =
                new Dictionary<Guid, CcuDataReplicator>();

        private readonly ICollection<ObjectType> _objectTypesWithParentCcuIdsFromReferences;

        private bool _suspended;

        private DataReplicationManager()
            : base(null)
        {
            _objectTypesWithParentCcuIdsFromReferences =
                new HashSet<ObjectType>
                {
                    ObjectType.Calendar,
                    ObjectType.CalendarDateSetting,
                    ObjectType.Card,
                    ObjectType.CardSystem,
                    ObjectType.DailyPlan,
                    ObjectType.DayInterval,
                    ObjectType.DayType,
                    ObjectType.SecurityDailyPlan,
                    ObjectType.SecurityDayInterval,
                    ObjectType.TimeZone,
                    ObjectType.TimeZoneDateSetting,
                    ObjectType.Person,
                    ObjectType.SecurityTimeZone,
                    ObjectType.SecurityTimeZoneDateSetting,
                    ObjectType.AlarmTransmitter,
                    ObjectType.AlarmArc,
                };


            _sendModifiedObjectToCcusBatchWorker = new BatchWorker<IRequest>(
                new CreateCcuDataBatchExecutor(this),
                GeneralOptions.Singleton.DelayForSendingChangesToCcu * 1000);

            Persons.Singleton.AddImportEventHandler(this);
            Cards.Singleton.AddImportEventHandler(this);
        }

        public void GeneralOptionsChanged()
        {
            _sendModifiedObjectToCcusBatchWorker.Timeout =
                GeneralOptions.Singleton.DelayForSendingChangesToCcu * 1000;
        }

        void IImportEventHandler.ImportStarted()
        {
            _suspended = true;
        }

        void IImportEventHandler.ImportDone()
        {
            _suspended = false;
        }

        private IEnumerable<Guid> GetIdParentCcusFromObjectInfos(IdAndObjectType idAndObjectType)
        {
            foreach (var kvp in _ccuDataReplicators)
                if (kvp.Value.ContainsObjectInfo(idAndObjectType))
                    yield return kvp.Key;
        }

        public void RemoveCcuDataReplicator(Guid guidCCU)
        {
            lock (_ccuDataReplicators)
            {
                CcuDataReplicator ccuDataReplicator;

                if (!_ccuDataReplicators.TryGetValue(
                    guidCCU,
                    out ccuDataReplicator))
                {
                    return;
                }

                ccuDataReplicator.Dispose();
                _ccuDataReplicators.Remove(guidCCU);
            }
        }

        public void SendModifiedObjectToCcus(AOrmObjectWithVersion ormObject)
        {
            SendModifiedObjectsToCcus(new[] { ormObject });
        }

        public void SendModifiedObjectsToCcus([NotNull] ICollection<AOrmObjectWithVersion> ormObjects)
        {
            _sendModifiedObjectToCcusBatchWorker.Add(
                new AddOrUpdateObjectsRequest(
                    ormObjects));
        }

        public void DeleteObjectFroCcus(IdAndObjectType objectToDelete)
        {
            _sendModifiedObjectToCcusBatchWorker.Add(
                new DeleteObjectsRequest(objectToDelete));
        }

        private void CreateAndEnqueueCcuDataBatches(
            [NotNull] IEnumerable<Guid> idTargetCcus)
        {
            var sendingThreads =
                new LinkedList<SafeThread>();

            foreach (var idCcu in idTargetCcus)
            {
                CcuDataReplicator ccuDataReplicator;

                if (_ccuDataReplicators.TryGetValue(
                    idCcu,
                    out ccuDataReplicator))
                {
                    sendingThreads.AddLast(
                        SafeThread.StartThread(
                            ccuDataReplicator.CreateAndEnqueueCcuDataBatch));
                }
            }

            foreach (var thread in sendingThreads)
                thread.Join();
        }

        private void DecrementObjectReferenceCountForCCU(
            Guid ccuGuid,
            IdAndObjectType idAndObjectType,
            bool isEditedObject)
        {
            CcuDataReplicator ccuDataReplicator;

            if (!_ccuDataReplicators.TryGetValue(
                ccuGuid,
                out ccuDataReplicator))
            {
                return;
            }

            ccuDataReplicator.DecrementObjectReferenceCount(
                idAndObjectType,
                isEditedObject);
        }

        public bool InitialSendModifiedObjectsToCcu(Guid idCcu)
        {
            lock (_ccuDataReplicators)
            {
                var ccuDataReplicator =
                    CreateCcuDataReplicator(idCcu);

                if (ccuDataReplicator == null)
                    return false;

                return ccuDataReplicator.CreateAndEnqueueCcuDataBatch(true);
            }
        }

        private CcuDataReplicator CreateCcuDataReplicator(Guid idCcu)
        {
            CcuDataReplicator result;

            if (!_ccuDataReplicators.TryGetValue(
                idCcu,
                out result))
            {
                result = new CcuDataReplicator(idCcu);

                _ccuDataReplicators.Add(
                    idCcu,
                    result);

                var ccu = CCUs.Singleton.GetById(idCcu);

                if (ccu == null)
                    return null;

                result.ReadObjectsForCcu(ccu);
            }

            return result;
        }

        public void ReloadCcuMaxObjectVersionAndIdsForObjectType(
            Guid idCcu,
            ObjectType objectType)
        {
            CcuDataReplicator ccuDataReplicator;

            if (_ccuDataReplicators.TryGetValue(
                idCcu,
                out ccuDataReplicator))
            {
                ccuDataReplicator.ReloadCcuMaxObjectVersionAndIdsForObjectType(objectType);
            }
        }

        public static AOrmObject LoadObjectFromDatabase(object objectId, ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.AccessControlList:
                    return AccessControlLists.Singleton.GetById(objectId);
                case ObjectType.ACLPerson:
                    return ACLPersons.Singleton.GetById(objectId);
                case ObjectType.ACLSetting:
                    return ACLSettings.Singleton.GetById(objectId);
                case ObjectType.ACLSettingAA:
                    return ACLSettingAAs.Singleton.GetById(objectId);
                case ObjectType.Calendar:
                    return Calendars.Singleton.GetById(objectId);
                case ObjectType.CalendarDateSetting:
                    return CalendarDateSettings.Singleton.GetById(objectId);
                case ObjectType.Card:
                    return Cards.Singleton.GetById(objectId);
                case ObjectType.CardSystem:
                    return CardSystems.Singleton.GetById(objectId);
                case ObjectType.DailyPlan:
                    return DailyPlans.Singleton.GetById(objectId);
                case ObjectType.DayInterval:
                    return DayIntervals.Singleton.GetById(objectId);
                case ObjectType.DayType:
                    return DayTypes.Singleton.GetById(objectId);
                case ObjectType.Input:
                    return Inputs.Singleton.GetById(objectId);
                case ObjectType.Output:
                    return Outputs.Singleton.GetById(objectId);
                case ObjectType.TimeZone:
                    return TimeZones.Singleton.GetById(objectId);
                case ObjectType.TimeZoneDateSetting:
                    return TimeZoneDateSettings.Singleton.GetById(objectId);
                case ObjectType.AlarmArea:
                    return AlarmAreas.Singleton.GetById(objectId);
                case ObjectType.AACardReader:
                    return AACardReaders.Singleton.GetById(objectId);
                case ObjectType.AccessZone:
                    return AccessZones.Singleton.GetById(objectId);
                case ObjectType.CCU:
                    return CCUs.Singleton.GetById(objectId);
                case ObjectType.AAInput:
                    return AAInputs.Singleton.GetById(objectId);
                case ObjectType.DoorEnvironment:
                    return DoorEnvironments.Singleton.GetById(objectId);
                case ObjectType.CardReader:
                    return CardReaders.Singleton.GetById(objectId);
                case ObjectType.SecurityDailyPlan:
                    return SecurityDailyPlans.Singleton.GetById(objectId);
                case ObjectType.SecurityDayInterval:
                    return SecurityDayIntervals.Singleton.GetById(objectId);
                case ObjectType.DCU:
                    return DCUs.Singleton.GetById(objectId);
                case ObjectType.DevicesAlarmSetting:
                    return DevicesAlarmSettings.Singleton.GetById(objectId);
                case ObjectType.Person:
                    return Persons.Singleton.GetById(objectId);
                case ObjectType.SecurityTimeZone:
                    return SecurityTimeZones.Singleton.GetById(objectId);
                case ObjectType.SecurityTimeZoneDateSetting:
                    return SecurityTimeZoneDateSettings.Singleton.GetById(objectId);
            }

            return null;
        }

        public static AOrmObject LoadOnOffObjectFromDatabase(object objectId, byte objectType)
        {
            switch (objectType)
            {
                case (byte)ObjectType.DailyPlan:
                    return LoadObjectFromDatabase(objectId, ObjectType.DailyPlan);
                case (byte)ObjectType.TimeZone:
                    return LoadObjectFromDatabase(objectId, ObjectType.TimeZone);
                case (byte)ObjectType.Input:
                    return LoadObjectFromDatabase(objectId, ObjectType.Input);
                case (byte)ObjectType.Output:
                    return LoadObjectFromDatabase(objectId, ObjectType.Output);
            }

            return null;
        }

        public static bool OnOffObjectShouldBeStoredOnCcu(
            Guid idCcu,
            AOnOffObject onOffObject)
        {
            var input = onOffObject as Input;

            if (input != null && input.GuidCCU != idCcu)
                return false;

            var output = onOffObject as Output;

            if (output != null && output.GuidCCU != idCcu)
                return false;

            return true;
        }
    }
}
