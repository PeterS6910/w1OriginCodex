using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class Floors :
        ANcasBaseOrmTable<Floors, Floor>,
        IFloors
    {
        private Floors() : base(null)
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.Floors), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.FloorsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.Floors), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.FloorsInsertDeletePerform), login);
        }

        protected override void LoadObjectsInRelationship(Floor obj)
        {
            if (obj.BlockAlarmArea != null)
            {
                obj.BlockAlarmArea = AlarmAreas.Singleton.GetById(obj.BlockAlarmArea.IdAlarmArea);
            }

            if (obj.Doors != null)
            {
                var doorsId = new LinkedList<Guid>(obj.Doors.Select(multiDoorElement => multiDoorElement.IdMultiDoorElement));
                obj.Doors.Clear();

                foreach (var doorId in doorsId)
                {
                    obj.Doors.Add(MultiDoorElements.Singleton.GetById(doorId));
                }
            }
        }

        public ICollection<FloorShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings,
            out Exception error)
        {
            var list = SelectByCriteria(filterSettings, out error);

            if (list != null)
            {
                return
                    new LinkedList<FloorShort>(
                        list.Select(floor => new FloorShort(floor)));
            }

            return null;
        }

        public ICollection<IModifyObject> ListModifyObjects(out Exception error)
        {
            var floors = List(out error);

            if (floors == null)
                return null;

            return
                new LinkedList<IModifyObject>(
                    floors.Select(floor => new FloorModifyObj(floor))
                        .OrderBy(floorModifyObj => floorModifyObj.ToString())
                        .Cast<IModifyObject>());
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name, bool single)
        {
            if (single && string.IsNullOrEmpty(name))
            {
                var floors = List();

                return floors != null
                    ? new LinkedList<AOrmObject>(
                        floors.OrderBy(floor => floor.Name).Cast<AOrmObject>())
                    : null;
            }

            if (string.IsNullOrEmpty(name))
                return null;

            var linqResult =
                single
                    ? SelectLinq<Floor>(floor => floor.Name.IndexOf(name) >= 0)
                    : SelectLinq<Floor>(
                        floor =>
                            floor.Name.IndexOf(name) >= 0 ||
                            floor.Description.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(floor => floor.Name).Cast<AOrmObject>())
                : null;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<Floor> linqResult;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<Floor>(
                        floor =>
                            floor.Name.IndexOf(name) >= 0 ||
                            floor.Description.IndexOf(name) >= 0);
            }
            else
            {
                linqResult = null;
            }

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(floor => floor.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<Floor>(
                        floor => floor.Name.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(floor => floor.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(Floor floor)
        {
            if (floor.BlockAlarmArea != null)
                yield return floor.BlockAlarmArea;

            if (floor.Doors != null)
            {
                foreach (var multiDoorElement in floor.Doors)
                {
                    yield return multiDoorElement;
                }
            }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var floor = GetById(idObj);
                if (floor == null)
                    return null;

                var result = new LinkedList<AOrmObject>();
                var usedInAcl = ACLSettings.Singleton.UsedLikeCardReaderObject(floor.IdFloor, ObjectType.Floor);
                if (usedInAcl != null)
                {
                    foreach (var acl in usedInAcl)
                    {
                        var outAcl = AccessControlLists.Singleton.GetById(acl.IdAccessControlList);
                        result.AddLast(outAcl);
                    }
                }

                var usedInAz = AccessZones.Singleton.UsedLikeCardReaderObject(floor.IdFloor, ObjectType.Floor);
                if (usedInAz != null)
                {
                    foreach (var person in usedInAz)
                    {
                        var outPerson = Persons.Singleton.GetById(person.IdPerson);
                        result.AddLast(outPerson);
                    }
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

        public void GetParentCCU(ICollection<Guid> ccus, Guid floorId)
        {
            var floor = GetById(floorId);
            if (ccus != null &&
                floor != null &&
                floor.Doors != null)
            {
                foreach (var multiDoorElement in floor.Doors)
                {
                    MultiDoorElements.Singleton.GetParentCCU(ccus, multiDoorElement.IdMultiDoorElement);
                }
            }
        }

        public HashSet<Guid> GetParentCcusId(Guid floorId)
        {
            var floor = GetById(floorId);

            if (floor == null ||
                floor.Doors == null)
            {
                return null;
            }

            var ccusId = new HashSet<Guid>();
            foreach (var multiDoorElement in floor.Doors)
            {
                var ccuId = MultiDoorElements.Singleton.GetParentCcuId(multiDoorElement.IdMultiDoorElement);
                if (ccuId == Guid.Empty)
                    continue;

                ccusId.Add(ccuId);
            }

            return ccusId;
        }

        public override void AfterInsert(Floor newFloor)
        {
            if (newFloor == null ||
                newFloor.Doors == null ||
                newFloor.BlockAlarmArea == null)
            {
                return;
            }

            foreach (
                var multiDoorElelementObjectForEdit in
                    newFloor.Doors.Select(
                        multiDoorElelement =>
                            MultiDoorElements.Singleton.GetObjectForEdit(multiDoorElelement.IdMultiDoorElement))
                        .Where(multiDoorElelementObjectForEdit => multiDoorElelementObjectForEdit != null))
            {
                if (multiDoorElelementObjectForEdit.BlockAlarmArea == null ||
                    multiDoorElelementObjectForEdit.BlockAlarmArea.IdAlarmArea != newFloor.BlockAlarmArea.IdAlarmArea)
                {
                    multiDoorElelementObjectForEdit.BlockAlarmArea = newFloor.BlockAlarmArea;
                    MultiDoorElements.Singleton.Update(multiDoorElelementObjectForEdit);
                }

                MultiDoorElements.Singleton.EditEnd(multiDoorElelementObjectForEdit);
            }
        }

        public override void AfterUpdate(Floor newFloor, Floor oldFloor)
        {
            if (newFloor == null ||
                newFloor.Doors == null ||
                oldFloor == null ||
                (oldFloor.BlockAlarmArea == null &&
                 newFloor.BlockAlarmArea == null))
            {
                return;
            }

            foreach (
                var multiDoorElelementObjectForEdit in
                    newFloor.Doors.Select(
                        multiDoorElelement =>
                            MultiDoorElements.Singleton.GetObjectForEdit(multiDoorElelement.IdMultiDoorElement))
                        .Where(multiDoorElelementObjectForEdit => multiDoorElelementObjectForEdit != null))
            {
                if ((newFloor.BlockAlarmArea == null &&
                     multiDoorElelementObjectForEdit.BlockAlarmArea != null) ||
                    (newFloor.BlockAlarmArea != null &&
                     (multiDoorElelementObjectForEdit.BlockAlarmArea == null ||
                      multiDoorElelementObjectForEdit.BlockAlarmArea.IdAlarmArea != newFloor.BlockAlarmArea.IdAlarmArea)))
                {
                    multiDoorElelementObjectForEdit.BlockAlarmArea = newFloor.BlockAlarmArea;
                    MultiDoorElements.Singleton.Update(multiDoorElelementObjectForEdit);
                }

                MultiDoorElements.Singleton.EditEnd(multiDoorElelementObjectForEdit);
            }
        }

        public IEnumerable<Floor> GetFloorsForAlarmArea(Guid alarmAreaId)
        {
            return SelectLinq<Floor>(
                floor =>
                    floor.BlockAlarmArea != null &&
                    floor.BlockAlarmArea.IdAlarmArea == alarmAreaId);
        }

        public Floor GetFloor(
            Guid elevatorParentCcuId,
            int floorNumber,
            bool createFloorIfNotExist,
            string floorLocalization,
            string elevatorName)
        {
            var floors = SelectLinq<Floor>(
                floor =>
                    floor.FloorNumber == floorNumber);

            if (floors != null)
            {
                foreach (var floor in floors)
                {
                    if (floor.BlockAlarmArea == null)
                        return floor;

                    var blockAlarmAreaParentCcu =
                        AlarmAreas.Singleton.GetImplicitCCUForAlarmArea(floor.BlockAlarmArea.IdAlarmArea);

                    if (blockAlarmAreaParentCcu != null &&
                        elevatorParentCcuId == blockAlarmAreaParentCcu.IdCCU)
                    {
                        return floor;
                    }
                }
            }

            if (createFloorIfNotExist)
            {
                var floor = new Floor
                {
                    Name = string.Format(
                        "{0} {1}",
                        floorLocalization,
                        floorNumber),

                    FloorNumber = floorNumber
                };

                if (Insert(ref floor))
                    return floor;
                
                floor.Name = string.Format(
                    @"{0} / {1} {2}",
                    elevatorName,
                    floorLocalization,
                    floorNumber);

                if (Insert(ref floor))
                    return floor;
            }

            return null;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.Floor; }
        }
    }
}
