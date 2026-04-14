using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using JetBrains.Annotations;
using System.Linq;

namespace Contal.Cgp.NCAS.Server.CcuDataReplicator
{
    public partial class DataReplicationManager
    {
        private partial class CcuDataReplicator :
            IDisposable
        {
            private class CcuObjectInfosForObjectType :
                IDisposable
            {
                private readonly ObjectType _objectType;

                private readonly IDictionary<Guid, ObjectInfo> _objectInfos =
                    new Dictionary<Guid, ObjectInfo>();

                private int _maximumVersionOnCcu;
                private ICollection<Guid> _idsOnCcu;

                public ICollection<Guid> ObjectIds
                {
                    get { return new LinkedList<Guid>(_objectInfos.Keys); }
                }

                public CcuObjectInfosForObjectType(ObjectType objectType)
                {
                    _objectType = objectType;
                }

                public void Initialize(CcuDataReplicator ccuDataReplicator)
                {
                    if (ObjectType == ObjectType.AccessControlList)
                        return;

                    LoadCcuMaxObjectVersionAndIds(ccuDataReplicator._idCcu);
                }

                private void LoadCcuMaxObjectVersionAndIds(Guid idCcu)
                {
                    var maximumVersionAndIds = CCUConfigurationHandler.Singleton.ReadMaximumVersionAndIds(
                        idCcu,
                        _objectType);

                    Console.WriteLine(
                        "Version for object of type: {0} is: {1}, ids count is: {2}",
                        _objectType,
                        maximumVersionAndIds.MaximumVersion,
                        maximumVersionAndIds.Ids != null
                            ? maximumVersionAndIds.Ids.Count
                            : 0);

                    _maximumVersionOnCcu = maximumVersionAndIds.MaximumVersion;
                    _idsOnCcu = new HashSet<Guid>(maximumVersionAndIds.Ids);
                }

                public void ReloadCcuMaxObjectVersionAndIds(Guid idCcu)
                {
                    LoadCcuMaxObjectVersionAndIds(idCcu);
                }

                public ObjectType ObjectType
                {
                    get { return _objectType; }
                }

                public ObjectInfo GetOrAddObjectInfo(AOrmObjectWithVersion ormObject)
                {
                    ObjectInfo result;

                    var id = (Guid) ormObject.GetId();

                    if (!_objectInfos.TryGetValue(
                        id,
                        out result))
                    {
                        result = new ObjectInfo(ormObject);

                        _objectInfos.Add(
                            id,
                            result);
                    }

                    return result;
                }

                public ObjectInfo GetObjectInfo(Guid id)
                {
                    ObjectInfo result;

                    return
                        _objectInfos.TryGetValue(
                            id,
                            out result)
                            ? result
                            : null;
                }

                public bool ContainsObjectInfo(Guid id)
                {
                    return _objectInfos.ContainsKey(id);
                }

                public void RemoveObjectInfo(Guid id)
                {
                    _objectInfos.Remove(id);
                }

                public void PopulateCcuDataBatch(
                    CcuDataReplicator ccuDataReplicator,
                    CcuDataBatch ccuDataBatch)
                {
                    if (ObjectType == ObjectType.AccessControlList)
                        return;

                    var modifiedObejctsId = new LinkedList<Guid>();
                    int maximumVersion = -1;

                    foreach (var objectInfo in _objectInfos.Values)
                    {
                        var objectInfoVersion = objectInfo.Version;

                        if (maximumVersion < objectInfoVersion)
                            maximumVersion = objectInfoVersion;

                        if (objectInfoVersion > _maximumVersionOnCcu
                            || !_idsOnCcu.Contains(objectInfo.Id))
                        {
                            modifiedObejctsId.AddLast(objectInfo.Id);
                        }
                    }

                    _maximumVersionOnCcu = maximumVersion;

                    if (modifiedObejctsId.Count > 0)
                    {
                        var modifiedObjectsCollection = new ModifiedObjectsCollection(
                            maximumVersion,
                            _objectType);

                        modifiedObjectsCollection.AddObjects(modifiedObejctsId);

                        ccuDataBatch.AddModifiedObjectsCollection(
                            modifiedObjectsCollection);
                    }

                    foreach (var id in _idsOnCcu)
                    {
                        if (!_objectInfos.ContainsKey(id))
                        {
                            ccuDataBatch.SetObjectToDelete(
                                ObjectType,
                                id);
                        }
                    }

                    _idsOnCcu = new HashSet<Guid>(_objectInfos.Keys);
                }

                public void Dispose()
                {
                    foreach (var objectInfo in _objectInfos.Values)
                        objectInfo.Dispose();

                    _objectInfos.Clear();
                }
            }

            private class ObjectInfo :
                IEquatable<ObjectInfo>,
                IDisposable
            {
                private class VersionChangedListener
                    : OrmObjectVersionChangedListener
                {
                    private readonly ObjectInfo _objectInfo;
                    
                    public VersionChangedListener(ObjectInfo objectInfo)
                    {
                        _objectInfo = objectInfo;
                    }

                    public override void OnVersionChanged(int newVersion)
                    {
                        _objectInfo.Version = newVersion;
                    }
                }

                private readonly IdAndObjectType _idAndObjectType;
                private int _referenceCount;
                private readonly VersionChangedListener _versionChangedListener;

                private readonly ICollection<IdAndObjectType> _referencedObjects =
                    new LinkedList<IdAndObjectType>();

                public Guid Id
                {
                    get { return (Guid)_idAndObjectType.Id; }
                }

                public int Version
                {
                    get;
                    private set;
                }

                public bool IsReferenced
                {
                    get { return _referenceCount > 0; }
                }

                public IEnumerable<IdAndObjectType> ReferencedObjects { get { return _referencedObjects; } }

                public ObjectInfo(AOrmObjectWithVersion ormObject)
                {
                    _idAndObjectType = new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType());

                    _versionChangedListener = new VersionChangedListener(this);

                    OrmObjectsVersionService.Singleton.AddListener(
                        _idAndObjectType,
                        _versionChangedListener);

                    _referenceCount = 0;
                    Version = ormObject.Version;
                }

                public void SetReferencedObjects(IEnumerable<IdAndObjectType> idAndObjectTypes)
                {
                    _referencedObjects.Clear();

                    if (idAndObjectTypes == null)
                        return;

                    foreach (var idAndObjectType in idAndObjectTypes)
                    {
                        _referencedObjects.Add(idAndObjectType);
                    }
                }

                public override int GetHashCode()
                {
                    return _idAndObjectType.GetHashCode();
                }

                public bool Equals(ObjectInfo otherCcuObjectReference)
                {
                    return
                        otherCcuObjectReference != null
                        && _idAndObjectType.Equals(otherCcuObjectReference._idAndObjectType);
                }

                public override bool Equals(object obj)
                {
                    return Equals(obj as ObjectInfo);
                }

                public void DecrementReferenceCount()
                {
                    --_referenceCount;
                }

                public void IncrementReferenceCount()
                {
                    _referenceCount++;
                }

                public void Dispose()
                {
                    OrmObjectsVersionService.Singleton.RemoveListener(
                        _idAndObjectType,
                        _versionChangedListener);
                }
            }

            private readonly Guid _idCcu;

            private static readonly ICollection<ObjectType> RelevantObjectTypes = new[]
            {
                ObjectType.Calendar,
                ObjectType.CalendarDateSetting,
                ObjectType.Card,
                ObjectType.CardSystem,
                ObjectType.DailyPlan,
                ObjectType.DayType,
                ObjectType.Person,
                ObjectType.TimeZone,
                ObjectType.TimeZoneDateSetting,
                ObjectType.AACardReader,
                ObjectType.AAInput,
                ObjectType.AccessControlList,
                ObjectType.AccessZone,
                ObjectType.ACLPerson,
                ObjectType.ACLSetting,
                ObjectType.ACLSettingAA,
                ObjectType.AlarmArea,
                ObjectType.AlarmTransmitter,
                ObjectType.AlarmArc,
                ObjectType.AntiPassBackZone,
                ObjectType.CardReader,
                ObjectType.CCU,
                ObjectType.DCU,
                ObjectType.DevicesAlarmSetting,
                ObjectType.DoorEnvironment,
                ObjectType.Input,
                ObjectType.MultiDoor,
                ObjectType.MultiDoorElement,
                ObjectType.Output,
                ObjectType.SecurityDailyPlan,
                ObjectType.SecurityDayInterval,
                ObjectType.SecurityTimeZone,
                ObjectType.SecurityTimeZoneDateSetting
            };

            private readonly IDictionary<ObjectType, CcuObjectInfosForObjectType>
                _ccuObjectInfosForObjectTypes =
                    new Dictionary<ObjectType, CcuObjectInfosForObjectType>();

            public CcuDataReplicator(Guid idCcu)
            {
                _idCcu = idCcu;

                foreach (var relevantObjectType in RelevantObjectTypes)
                {
                    var ccuObjectInfosForObjectType = new CcuObjectInfosForObjectType(relevantObjectType);

                    _ccuObjectInfosForObjectTypes.Add(
                        relevantObjectType,
                        ccuObjectInfosForObjectType);

                    ccuObjectInfosForObjectType.Initialize(this);
                }
            }

            public void ReadObjectsForCcu(CCU ccu)
            {
                new Initializer(this).ReadObjectsForCcu(ccu);
            }

            private static bool HasAlarmAreaCardReaderFromThisList(
                AlarmArea alarmArea,
                HashSet<Guid> hashSetAllCCUCardReaders)
            {
                if (alarmArea != null
                    && alarmArea.AACardReaders != null
                    && alarmArea.AACardReaders.Count > 0)
                {
                    foreach (var aaCardReader in alarmArea.AACardReaders)
                    {
                        if (aaCardReader.CardReader != null
                            && hashSetAllCCUCardReaders.Contains(
                                aaCardReader.CardReader.IdCardReader))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            private static bool HasDoorEnvironmentCardReaderFromThisList(
                DoorEnvironment doorEnvironment,
                HashSet<Guid> hashSetAllCCUCardReaders)
            {
                if (doorEnvironment != null)
                {
                    if (doorEnvironment.CardReaderExternal != null
                        && hashSetAllCCUCardReaders.Contains(
                            doorEnvironment.CardReaderExternal.IdCardReader))
                    {
                        return true;
                    }

                    if (doorEnvironment.CardReaderInternal != null
                        && hashSetAllCCUCardReaders.Contains(
                            doorEnvironment.CardReaderInternal.IdCardReader))
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool UpdateObjectInfo(
                AOrmObjectWithVersion ormObject,
                [NotNull] IDictionary<IdAndObjectType, HashSet<Guid>> cacheOfParentCcuIdsForObjects,
                [NotNull] IDictionary<IdAndObjectType, ICollection<AOrmObject>> referencedObjectsCache)
            {
                IEnumerable<AOrmObject> referencedObjects;

                return UpdateObjectInfo(
                    ormObject,
                    cacheOfParentCcuIdsForObjects,
                    referencedObjectsCache,
                    out referencedObjects);
            }

            public bool UpdateObjectInfo(
                AOrmObjectWithVersion ormObject,
                [NotNull] IDictionary<IdAndObjectType, HashSet<Guid>> cacheOfParentCcuIdsForObjects,
                [NotNull] IDictionary<IdAndObjectType, ICollection<AOrmObject>> referencedObjectsCache,
                out IEnumerable<AOrmObject> referencedObjects)
            {
                CcuObjectInfosForObjectType ccuObjectInfosForObjectType;

                var objectType = ormObject.GetObjectType();

                if (!_ccuObjectInfosForObjectTypes.TryGetValue(
                    objectType,
                    out ccuObjectInfosForObjectType))
                {
                    referencedObjects = Enumerable.Empty<AOrmObject>();
                    return false;
                }

                var objectInfo = ccuObjectInfosForObjectType.GetOrAddObjectInfo(ormObject);

                bool retValue = !objectInfo.IsReferenced;

                objectInfo.IncrementReferenceCount();

                var referencedOrmObjects = new LinkedList<AOrmObject>();
                var referencedIdAndObjectTypes = new LinkedList<IdAndObjectType>();

                var allReferencedObjects = GetReferencedObjectsForCcuReplication(
                        new IdAndObjectType(
                            ormObject.GetId(),
                            objectType),
                        referencedObjectsCache);

                if (allReferencedObjects != null)
                {
                    foreach (var referencedObject in allReferencedObjects)
                    {
                        if (referencedObject == null)
                            continue;

                        var referencedIdAndObjectType = new IdAndObjectType(
                            referencedObject.GetId(),
                            referencedObject.GetObjectType());

                        if (!Singleton.IsObjectTypeWithParentCcuIdsFromReferences(referencedIdAndObjectType.ObjectType)
                            && !Singleton.GetExplicitIdParentCcus(
                                    referencedIdAndObjectType,
                                    cacheOfParentCcuIdsForObjects).Contains(_idCcu))
                        {
                            continue;
                        }

                        referencedOrmObjects.AddLast(referencedObject);
                        referencedIdAndObjectTypes.AddLast(referencedIdAndObjectType);
                    }
                }

                objectInfo.SetReferencedObjects(referencedIdAndObjectTypes);
                referencedObjects = referencedOrmObjects;

                return retValue;
            }

            public ICollection<AOrmObject> GetReferencedObjectsForCcuReplication(
                IdAndObjectType idAndObjectType,
                [NotNull] IDictionary<IdAndObjectType, ICollection<AOrmObject>> referencedObjectsCache)
            {
                ICollection<AOrmObject> referencedObjects;

                if (referencedObjectsCache.TryGetValue(
                    idAndObjectType,
                    out referencedObjects))
                {
                    return referencedObjects;
                }

                referencedObjects = GetReferencedObjectsForCcuReplication(idAndObjectType);

                referencedObjectsCache.Add(
                    idAndObjectType,
                    referencedObjects);

                return referencedObjects;
            }

            //TODO Make this polymorfic using CrpServerRemotingProvider.GetTableOrmForObjectType
            private ICollection<AOrmObject> GetReferencedObjectsForCcuReplication(IdAndObjectType idAndObjectType)
            {
                var id = (Guid)idAndObjectType.Id;

                switch (idAndObjectType.ObjectType)
                {
                    case ObjectType.CCU:

                        return CCUs.Singleton.GetReferencedObjectsForCcuReplication(id);

                    case ObjectType.Input:

                        return Inputs.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.Output:

                        return Outputs.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.ACLSetting:

                        return ACLSettings.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.ACLPerson:

                        return ACLPersons.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.ACLSettingAA:

                        return ACLSettingAAs.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.AccessControlList:

                        return AccessControlLists.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.AlarmArea:

                        return AlarmAreas.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.AccessZone:

                        return AccessZones.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.DCU:

                        return DCUs.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.DoorEnvironment:

                        return
                            DoorEnvironments.Singleton.GetReferencedObjectsForCcuReplication(id);

                    case ObjectType.CardReader:

                        return CardReaders.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.TimeZone:

                        return TimeZones.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.TimeZoneDateSetting:

                        return
                            TimeZoneDateSettings.Singleton.GetReferencedObjectsForCcuReplication(
                                _idCcu,
                                id);

                    case ObjectType.SecurityTimeZone:

                        return SecurityTimeZones.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.SecurityTimeZoneDateSetting:

                        return
                            SecurityTimeZoneDateSettings.Singleton
                                .GetReferencedObjectsForCcuReplication(
                                    _idCcu,
                                    id);

                    case ObjectType.Calendar:

                        return Calendars.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.CalendarDateSetting:

                        return
                            CalendarDateSettings.Singleton.GetReferencedObjectsForCcuReplication(
                                _idCcu,
                                id);

                    case ObjectType.Card:

                        return Cards.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.Person:

                        return Persons.Singleton.GetReferencedObjectsForCcuReplication(
                            _idCcu,
                            id);

                    case ObjectType.DevicesAlarmSetting:

                        return
                            DevicesAlarmSettings.Singleton.GetReferencedObjectsForCcuReplication(id);

                    case ObjectType.AntiPassBackZone:

                        return
                            AntiPassBackZones.Singleton.GetReferencedObjectsForCcuReplication(id);

                    case ObjectType.MultiDoor:

                        return MultiDoors.Singleton.GetReferencedObjectsForCcuReplication(id);

                    case ObjectType.MultiDoorElement:

                        return
                            MultiDoorElements.Singleton.GetReferencedObjectsForCcuReplication(id);
                }

                return null;
            }

            public bool ContainsObjectInfo(IdAndObjectType idAndObjectType)
            {
                CcuObjectInfosForObjectType ccuObjectInfosForObjectType;

                return
                    _ccuObjectInfosForObjectTypes.TryGetValue(
                        idAndObjectType.ObjectType,
                        out ccuObjectInfosForObjectType)
                    && ccuObjectInfosForObjectType.ContainsObjectInfo((Guid)idAndObjectType.Id);
            }

            public void CreateAndEnqueueCcuDataBatch()
            {
                CreateAndEnqueueCcuDataBatch(false);
            }

            public bool CreateAndEnqueueCcuDataBatch(bool invokedFromInitCCU)
            {
                Console.WriteLine("SendModifyObjectsToCCU START");

                var ccuDataBatch = new CcuDataBatch(invokedFromInitCCU);

                foreach (var ccuObjectReferencesForObjectType in _ccuObjectInfosForObjectTypes.Values)
                    ccuObjectReferencesForObjectType.PopulateCcuDataBatch(this, ccuDataBatch);

                return CCUConfigurationHandler.Singleton.EnqueueCcuDataBatch(_idCcu, ccuDataBatch);
            }

            public void DecrementObjectReferenceCount(
                IdAndObjectType idAndObjectType,
                bool isEditedObject)
            {
                CcuObjectInfosForObjectType ccuObjectInfosForObjectType;

                if (!_ccuObjectInfosForObjectTypes.TryGetValue(
                    idAndObjectType.ObjectType,
                    out ccuObjectInfosForObjectType))
                {
                    return;
                }

                var objectInfo =
                    ccuObjectInfosForObjectType
                        .GetObjectInfo((Guid)idAndObjectType.Id);

                if (objectInfo == null)
                    return;

                objectInfo.DecrementReferenceCount();

                if (objectInfo.IsReferenced
                    && !isEditedObject)
                {
                    return;
                }

                foreach (var referencedObject in objectInfo.ReferencedObjects)
                    DecrementObjectReferenceCount(
                        referencedObject,
                        false);

                if (objectInfo.IsReferenced)
                    return;

                ccuObjectInfosForObjectType.RemoveObjectInfo((Guid)idAndObjectType.Id);
                objectInfo.Dispose();
            }

            public void ReloadCcuMaxObjectVersionAndIdsForObjectType(ObjectType objectType)
            {
                CcuObjectInfosForObjectType ccuObjectInfosForObjectType;

                if (_ccuObjectInfosForObjectTypes.TryGetValue(
                    objectType,
                    out ccuObjectInfosForObjectType))
                {
                    ccuObjectInfosForObjectType.ReloadCcuMaxObjectVersionAndIds(_idCcu);
                }
            }

            public void Dispose()
            {
                foreach (var ccuObjectInfosForObjectType in _ccuObjectInfosForObjectTypes.Values)
                    ccuObjectInfosForObjectType.Dispose();

                _ccuObjectInfosForObjectTypes.Clear();
            }
        }
    }
}