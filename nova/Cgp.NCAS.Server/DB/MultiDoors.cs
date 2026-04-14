using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class MultiDoors :
        ANcasBaseOrmTable<MultiDoors, MultiDoor>, 
        IMultiDoors
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<MultiDoor>
        {
            protected override IEnumerable<AOrmObjectWithVersion> GetSubObjects(MultiDoor obj)
            {
                if (obj.Doors != null)
                    return obj.Doors;

                return Enumerable.Empty<AOrmObjectWithVersion>();
            }
        }

        private MultiDoors()
            : base(
                  null,
                  new CudPreparation())
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.MultiDoors), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.MultiDoorsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.MultiDoors), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.MultiDoorsInsertDeletePerform), login);
        }

        protected override void LoadObjectsInRelationship(MultiDoor obj)
        {
            if (obj.CardReader != null)
            {
                obj.CardReader = CardReaders.Singleton.GetById(obj.CardReader.IdCardReader);
            }

            if (obj.BlockAlarmArea != null)
            {
                obj.BlockAlarmArea = AlarmAreas.Singleton.GetById(obj.BlockAlarmArea.IdAlarmArea);
            }

            if (obj.Doors != null)
            {
                var doorsId =
                    new LinkedList<Guid>(obj.Doors.Select(multiDoorElement => multiDoorElement.IdMultiDoorElement));

                obj.Doors.Clear();

                foreach (var doorId in doorsId)
                {
                    obj.Doors.Add(MultiDoorElements.Singleton.GetById(doorId));
                }
            }
        }

        public override void AfterInsert(MultiDoor newMultiDoor)
        {
            if (newMultiDoor == null)
                return;

            if (newMultiDoor.CardReader != null)
                RemoveAclSettingsForCardReader(newMultiDoor.CardReader.IdCardReader);

            if (newMultiDoor.BlockAlarmArea == null ||
                newMultiDoor.Doors == null)
            {
                return;
            }

            foreach (
                var multiDoorElelementObjectForEdit in
                    newMultiDoor.Doors.Select(
                        multiDoorElelement =>
                            MultiDoorElements.Singleton.GetObjectForEdit(multiDoorElelement.IdMultiDoorElement))
                        .Where(multiDoorElelementObjectForEdit => multiDoorElelementObjectForEdit != null))
            {
                if (multiDoorElelementObjectForEdit.BlockAlarmArea == null ||
                    multiDoorElelementObjectForEdit.BlockAlarmArea.IdAlarmArea !=
                    newMultiDoor.BlockAlarmArea.IdAlarmArea)
                {
                    multiDoorElelementObjectForEdit.BlockAlarmArea = newMultiDoor.BlockAlarmArea;
                    MultiDoorElements.Singleton.Update(multiDoorElelementObjectForEdit);
                }

                MultiDoorElements.Singleton.EditEnd(multiDoorElelementObjectForEdit);
            }
        }

        public override void AfterUpdate(MultiDoor newMultiDoor, MultiDoor oldMultiDoor)
        {
            if (newMultiDoor == null)
                return;

            if (newMultiDoor.CardReader != null)
                RemoveAclSettingsForCardReader(newMultiDoor.CardReader.IdCardReader);

            if (newMultiDoor.Doors == null)
                return;

            foreach (var multiDoorElement in newMultiDoor.Doors)
            {
                MultiDoorElements.Singleton.AfterInsertUpdate(multiDoorElement, newMultiDoor);
            }

            if (oldMultiDoor == null ||
                (oldMultiDoor.BlockAlarmArea == null &&
                 newMultiDoor.BlockAlarmArea == null))
            {
                return;
            }

            foreach (
                var multiDoorElelementObjectForEdit in
                    newMultiDoor.Doors.Select(
                        multiDoorElelement =>
                            MultiDoorElements.Singleton.GetObjectForEdit(multiDoorElelement.IdMultiDoorElement))
                        .Where(multiDoorElelementObjectForEdit => multiDoorElelementObjectForEdit != null))
            {
                if ((newMultiDoor.BlockAlarmArea == null &&
                     multiDoorElelementObjectForEdit.BlockAlarmArea != null) ||
                    (newMultiDoor.BlockAlarmArea != null &&
                     (multiDoorElelementObjectForEdit.BlockAlarmArea == null ||
                      multiDoorElelementObjectForEdit.BlockAlarmArea.IdAlarmArea !=
                      newMultiDoor.BlockAlarmArea.IdAlarmArea)))
                {
                    multiDoorElelementObjectForEdit.BlockAlarmArea = newMultiDoor.BlockAlarmArea;
                    MultiDoorElements.Singleton.Update(multiDoorElelementObjectForEdit);
                }

                MultiDoorElements.Singleton.EditEnd(multiDoorElelementObjectForEdit);
            }
        }

        private void RemoveAclSettingsForCardReader(Guid cardReaderId)
        {
            ACLSettings.Singleton.DeleteByCriteria(
                aclSetting =>
                    aclSetting.CardReaderObjectType == (byte) ObjectType.CardReader &&
                    aclSetting.GuidCardReaderObject == cardReaderId,
                null);
        }

        public override bool CheckData(MultiDoor multiDoor, out Exception error)
        {
            error = null;

            if (multiDoor.CardReader != null)
            {
                var multiDoorId = multiDoor.IdMultiDoor != Guid.Empty ? (Guid?) multiDoor.IdMultiDoor : null;
                if (IsCardReaderUsedInDoorElement(multiDoor.CardReader.IdCardReader, multiDoorId))
                {
                    error = new IwQuick.SqlUniqueException(MultiDoor.ColumnCardReader);
                    return false;
                }
            }

            return true;
        }

        public override void CUDSpecial(
            MultiDoor multiDoor,
            ObjectDatabaseAction objectDatabaseAction)
        {
            if (multiDoor == null)
                return;

            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        multiDoor.GetId(),
                        multiDoor.GetObjectType()));

                return;
            }

            DataReplicationManager.Singleton.SendModifiedObjectToCcus(multiDoor);
        }

        public ICollection<MultiDoorShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings,
            out Exception error)
        {
            var list = SelectByCriteria(filterSettings, out error);

            if (list != null)
            {
                return
                    new LinkedList<MultiDoorShort>(
                        list.Select(multiDoorEnvironment => new MultiDoorShort(multiDoorEnvironment)));
            }

            return null;
        }

        public ICollection<IModifyObject> ListModifyObjects(Guid? ccuId, out Exception error)
        {
            var multiDoorEnvironments = List(out error) as IEnumerable<MultiDoor>;

            if (multiDoorEnvironments == null)
                return null;

            if (ccuId != null)
            {
                multiDoorEnvironments =
                    multiDoorEnvironments.Where(
                        multiDoorEnvironment =>
                            GetParentCcuId(multiDoorEnvironment) == ccuId);
            }

            return
                new LinkedList<IModifyObject>(
                    multiDoorEnvironments.Select(
                        multiDoorEnvironment => new MultiDoorModifyObj(multiDoorEnvironment))
                        .OrderBy(multiDoorEnvironmentModObj => multiDoorEnvironmentModObj.ToString())
                        .Cast<IModifyObject>());
        }

        public ICollection<IModifyObject> ListNotUsedCardReadersModifyObjects(
            Guid? multiDoorId,
            Guid? ccuId,
            out Exception error)
        {
            var cardReaders = CardReaders.Singleton.List(out error) as IEnumerable<CardReader>;

            if (cardReaders == null)
                return null;

            if (ccuId != null)
            {
                cardReaders =
                    cardReaders.Where(cardReader => CardReaders.Singleton.GetParentCCU(cardReader.IdCardReader) == ccuId);
            }

            var usedCardReadersInDoorElements = GetUsedCardReadersInDoorElements(multiDoorId);

            return
                new LinkedList<IModifyObject>(
                    cardReaders.Where(cardReader => !usedCardReadersInDoorElements.Contains(cardReader.IdCardReader))
                        .Select(cardReader => new CardReaderModifyObj(cardReader))
                        .OrderBy(cardReaderShort => cardReaderShort.ToString())
                        .Cast<IModifyObject>());
        }

        private HashSet<Guid> GetUsedCardReadersInDoorElements(Guid? multiDoorId)
        {
            var doorEnvironments =
                SelectLinq<DoorEnvironment>(
                    doorEnvironemnt =>
                        doorEnvironemnt.CardReaderInternal != null ||
                        doorEnvironemnt.CardReaderExternal != null);

            var multiDoors = multiDoorId != null
                ? SelectLinq<MultiDoor>(multiDoor => multiDoor.IdMultiDoor != multiDoorId)
                : List();

            var usedCardReaders = Enumerable.Empty<Guid>();

            if (doorEnvironments != null)
                usedCardReaders =
                    usedCardReaders
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.CardReaderInternal != null)
                            .Select(doorEnvironment => doorEnvironment.CardReaderInternal.IdCardReader))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.CardReaderExternal != null)
                            .Select(doorEnvironment => doorEnvironment.CardReaderExternal.IdCardReader));

            if (multiDoors != null)
                usedCardReaders =
                    usedCardReaders
                        .Concat(multiDoors
                            .Select(
                                multiDoor => multiDoor.CardReader.IdCardReader));

            return new HashSet<Guid>(usedCardReaders);
        }

        public bool IsCardReaderUsedInDoorElement(Guid cardReaderId, Guid? multiDoorId)
        {
            return GetUsedCardReadersInDoorElements(multiDoorId).Contains(cardReaderId);
        }

        public bool IsCardReaderUsedInMultiDoor(Guid cardReaderId)
        {
            var multiDoors = SelectLinq<MultiDoor>(
                multiDoor => multiDoor.CardReader != null &&
                             multiDoor.CardReader.IdCardReader == cardReaderId);

            return multiDoors != null && multiDoors.Count > 0;
        }

        public Guid? GetParentCcuId(Guid multiDoorId)
        {
            var multiDoor = GetById(multiDoorId);
            if (multiDoor == null)
                return null;

            return GetParentCcuId(multiDoor);
        }

        public Guid GetParentCcuId(MultiDoor multiDoor)
        {
            return CardReaders.Singleton.GetParentCCU(multiDoor.CardReader.IdCardReader);
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid multiDoorId)
        {
            GetParentCCU(ccus, GetById(multiDoorId));
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name, bool single)
        {
            if (single && string.IsNullOrEmpty(name))
            {
                var multiDoors = List();

                return multiDoors != null
                    ? new LinkedList<AOrmObject>(
                        multiDoors.OrderBy(multiDoor => multiDoor.Name).Cast<AOrmObject>())
                    : null;
            }

            if (string.IsNullOrEmpty(name))
                return null;

            var linqResult =
                single
                    ? SelectLinq<MultiDoor>(
                        multiDoor => multiDoor.Name.IndexOf(name) >= 0)
                    : SelectLinq<MultiDoor>(
                        multiDoor =>
                            multiDoor.Name.IndexOf(name) >= 0 ||
                            multiDoor.Description.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(multiDoor => multiDoor.Name).Cast<AOrmObject>())
                : null;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<MultiDoor> linqResult;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<MultiDoor>(
                        multiDoor =>
                            multiDoor.Name.IndexOf(name) >= 0 ||
                            multiDoor.Description.IndexOf(name) >= 0);
            }
            else
            {
                linqResult = null;
            }

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(multiDoor => multiDoor.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<MultiDoor>(
                        multiDoor => multiDoor.Name.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(multiDoor => multiDoor.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public void GetParentCCU(ICollection<Guid> ccus, MultiDoor multiDoor)
        {
            if (ccus != null &&
                multiDoor != null)
            {
                var ccuId = GetParentCcuId(multiDoor);
                if (ccuId != Guid.Empty && !ccus.Contains(ccuId))
                {
                    ccus.Add(ccuId);
                }
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid multiDoorId)
        {
            var objects = new List<AOrmObject>();

            var multiDoor = GetById(multiDoorId);
            if (multiDoor != null)
            {
                if (multiDoor.Doors != null)
                {
                    foreach (var multiDoorElement in multiDoor.Doors)
                    {
                        objects.Add(multiDoorElement);
                    }
                }
            }

            return objects;
        }

        public IEnumerable<MultiDoor> GetMultiDoorsForCardReaders(
            ICollection<Guid> guidCardReaders)
        {
            var multiDoors = List();

            if (multiDoors == null)
                return Enumerable.Empty<MultiDoor>();

            return
                multiDoors.Where(
                    multiDoor =>
                        multiDoor.CardReader != null &&
                        guidCardReaders.Contains(multiDoor.CardReader.IdCardReader));
        }

        public ICollection<MultiDoor> GetMultiDoorsForCardReader(Guid cardReaderId)
        {
            return SelectLinq<MultiDoor>(
                multiDoor =>
                    multiDoor.CardReader != null &&
                    multiDoor.CardReader.IdCardReader == cardReaderId);
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(MultiDoor multiDoor)
        {
            if (multiDoor.CardReader != null)
                yield return multiDoor.CardReader;

            if (multiDoor.BlockAlarmArea != null)
                yield return multiDoor.BlockAlarmArea;

            if (multiDoor.Doors != null)
            {
                foreach (var multiDoorElement in multiDoor.Doors)
                {
                    yield return multiDoorElement;
                }
            }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var multiDoor = GetById(idObj);
                if (multiDoor == null)
                    return null;

                var result = new LinkedList<AOrmObject>();
                var usedInAcl = ACLSettings.Singleton.UsedLikeCardReaderObject(multiDoor.IdMultiDoor, ObjectType.MultiDoor);
                if (usedInAcl != null)
                {
                    foreach (var acl in usedInAcl)
                    {
                        var outAcl = AccessControlLists.Singleton.GetById(acl.IdAccessControlList);
                        result.AddLast(outAcl);
                    }
                }

                var usedInAz = AccessZones.Singleton.UsedLikeCardReaderObject(multiDoor.IdMultiDoor, ObjectType.MultiDoor);
                if (usedInAz != null)
                {
                    foreach (var person in usedInAz)
                    {
                        var outPerson = Persons.Singleton.GetById(person.IdPerson);
                        result.AddLast(outPerson);
                    }
                }

                var ccu = CardReaders.Singleton.GetParentCcu(multiDoor.CardReader);
                if (ccu != null)
                {
                    result.AddLast(CCUs.Singleton.GetById(ccu.IdCCU));
                }

                return
                    result.Count > 0
                        ? result.OrderBy(orm => orm.ToString()).ToList()
                        : null;
            }
            catch
            {
                return null;
            }
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var multiDoor = ormObject as MultiDoor;

            if (multiDoor == null)
                return null;

            return CardReaders.Singleton.GetParentCcu(multiDoor.CardReader);
        }

        public override bool DeleteIfReferenced(object id, IList<AOrmObject> referencedObjects)
        {
            if (referencedObjects == null || referencedObjects.Count == 0)
                return true;

            return referencedObjects.All(ormObject => ormObject.GetObjectType() == ObjectType.CCU);
        }

        public override bool IsReferencedSubObjects(MultiDoor multiDoorProxy)
        {
            if (multiDoorProxy == null)
                return false;

            var multiDoor = GetById(multiDoorProxy.IdMultiDoor);

            if (multiDoor == null ||
                multiDoor.Doors == null)
            {
                return false;
            }

            return
                multiDoor.Doors.Any(
                    multiDoorElement =>
                        !MultiDoorElements.Singleton.DeleteIfReferenced(
                            multiDoorElement.IdMultiDoorElement,
                            MultiDoorElements.Singleton.GetReferencedObjectsAllPlugins(
                                (multiDoorElement.IdMultiDoorElement))));
        }

        public IEnumerable<MultiDoor> GetMultiDoorsForAlarmArea(Guid alarmAreaId)
        {
            return SelectLinq<MultiDoor>(
                multiDoor =>
                    multiDoor.BlockAlarmArea != null &&
                    multiDoor.BlockAlarmArea.IdAlarmArea == alarmAreaId);
        }

        public void CreateDoorsForMultiDoor(
            Guid multiDoorId,
            string doorLocalization,
            int doorsCount)
        {
            var multiDoor = GetById(multiDoorId);
            if (multiDoor == null)
                return;

            for (var i = 1; i <= doorsCount; i++)
            {
                var multiDoorElement = new MultiDoorElement
                {
                    MultiDoor = multiDoor,

                    Name = string.Format(
                        "{0} {1}",
                        doorLocalization,
                        i),

                    DoorIndex = i
                };

                MultiDoorElements.Singleton.Insert(ref multiDoorElement);
            }
        }

        public void CreateDoorsForElevator(
            Guid multiDoorId,
            string floorLocalization,
            int lowestDoorIndex,
            int doorsCount,
            bool assignToFloors,
            bool createFloorIfNotExist)
        {
            var multiDoor = GetById(multiDoorId);
            if (multiDoor == null)
                return;

            var doorIndex = lowestDoorIndex;
            for (var i = 1; i <= doorsCount; i++)
            {
                var multiDoorElement = new MultiDoorElement
                {
                    MultiDoor = multiDoor,

                    Name = string.Format(
                        "{0} {1}",
                        floorLocalization,
                        doorIndex),

                    DoorIndex = doorIndex,

                    Floor = assignToFloors
                        ? Floors.Singleton.GetFloor(
                            GetParentCcuId(multiDoor),
                            doorIndex,
                            createFloorIfNotExist,
                            floorLocalization,
                            multiDoor.Name)
                        : null
                };

                MultiDoorElements.Singleton.Insert(ref multiDoorElement);
                doorIndex++;
            }
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.MultiDoor; }
        }
    }
}
