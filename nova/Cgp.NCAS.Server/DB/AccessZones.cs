using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AccessZones :
        ANcasBaseOrmTable<AccessZones, AccessZone>, 
        IAccessZones
    {
        private AccessZones()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<AccessZone>())
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesView),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin),
                login);
        }

        public override void CUDSpecial(AccessZone accessZone, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        accessZone.GetId(),
                        accessZone.GetObjectType()));
            }
            else if (accessZone != null)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(accessZone);
            }
        }

        protected override void LoadObjectsInRelationship(AccessZone obj)
        {
            if (obj.Person != null)
            {
                obj.Person = Persons.Singleton.GetById(obj.Person.IdPerson);
            }

            obj.CardReaderObject = ReadCardReaderObject(obj);

            if (obj.TimeZone != null)
            {
                obj.TimeZone = TimeZones.Singleton.GetById(obj.TimeZone.IdTimeZone);
            }
        }

        public static AOrmObject ReadCardReaderObject(AccessZone accessZone)
        {
            if (accessZone.GuidCardReaderObject == Guid.Empty)
                return null;

            switch (accessZone.CardReaderObjectType)
            {
                case (byte)ObjectType.CardReader:
                    return CardReaders.Singleton.GetById(accessZone.GuidCardReaderObject);

                case (byte)ObjectType.AlarmArea:
                    return AlarmAreas.Singleton.GetById(accessZone.GuidCardReaderObject);

                case (byte)ObjectType.DoorEnvironment:
                    return DoorEnvironments.Singleton.GetById(accessZone.GuidCardReaderObject);

                case (byte)ObjectType.DCU:
                    return DCUs.Singleton.GetById(accessZone.GuidCardReaderObject);

                case (byte)ObjectType.MultiDoor:
                    return MultiDoors.Singleton.GetById(accessZone.GuidCardReaderObject);

                case (byte)ObjectType.MultiDoorElement:
                    return MultiDoorElements.Singleton.GetById(accessZone.GuidCardReaderObject);

                case (byte)ObjectType.Floor:
                    return Floors.Singleton.GetById(accessZone.GuidCardReaderObject);
            }

            return null;
        }

        public ICollection<Person> GetPersonForTimeZone(TimeZone timeZone)
        {
            try
            {
                if (timeZone == null)
                    return null;

                ICollection<AccessZone> accessZonesList = 
                    SelectLinq<AccessZone>(accessZone => accessZone.TimeZone == timeZone);

                if (accessZonesList == null) 
                    return null;

                return
                    new LinkedList<Person>(
                        accessZonesList.Select(accessZone => accessZone.Person));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// For Referenced By mechanism, check if object are referenced in AccessZone
        /// </summary>
        /// <param name="idObj">Object Id (CardReader, Doorenvironment, AlarmArea)</param>
        /// <param name="typeObj"></param>
        /// <returns>IList of Persons, AccessZone is Person settings therefore result is list of person</returns>
        public IList<Person> UsedLikeCardReaderObject(Guid idObj, ObjectType typeObj)
        {
            try
            {
                IList<Person> result = new List<Person>();
                ICollection<AccessZone> accessZoneList = SelectLinq<AccessZone>(az => az.GuidCardReaderObject == idObj);
                if (accessZoneList != null && accessZoneList.Count > 0)
                {
                    foreach (AccessZone az in accessZoneList)
                    {
                        result.Add(az.Person);
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        public void GetParentCCU(
            ICollection<Guid> ccus,
            Guid idAccessZone)
        {
            GetParentCCU(
                ccus,
                GetObjectById(idAccessZone));
        }

        public void GetParentCCU(
            ICollection<Guid> ccus,
            AccessZone accessZone)
        {
            if (ccus != null && accessZone != null)
            {
                if (accessZone.CardReaderObject != null)
                {
                    switch (accessZone.CardReaderObject.GetObjectType())
                    {
                        case ObjectType.CardReader:
                            CardReaders.Singleton.GetParentCCU(ccus, (Guid)accessZone.CardReaderObject.GetId());
                            break;
                        case ObjectType.AlarmArea:
                            AlarmAreas.Singleton.GetParentCCU(ccus, (Guid)accessZone.CardReaderObject.GetId());
                            break;
                        case ObjectType.DoorEnvironment:
                            DoorEnvironments.Singleton.GetParentCCU(ccus, (Guid)accessZone.CardReaderObject.GetId());
                            break;
                        case ObjectType.DCU:
                            DCUs.Singleton.GetParentCCU(ccus, (Guid)accessZone.CardReaderObject.GetId());
                            break;
                        case ObjectType.MultiDoor:
                            MultiDoors.Singleton.GetParentCCU(ccus, (Guid)accessZone.CardReaderObject.GetId());
                            break;
                        case ObjectType.MultiDoorElement:
                            MultiDoorElements.Singleton.GetParentCCU(ccus, (Guid)accessZone.CardReaderObject.GetId());
                            break;
                        case ObjectType.Floor:
                            Floors.Singleton.GetParentCCU(ccus, (Guid)accessZone.CardReaderObject.GetId());
                            break;
                    }
                }
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idAccessZones)
        {
            var objects = new List<AOrmObject>();

            AccessZone accessZone = GetById(idAccessZones);
            if (accessZone != null)
            {
                if (accessZone.TimeZone != null)
                {
                    objects.Add(accessZone.TimeZone);
                }
                if (accessZone.Person != null)
                {
                    objects.Add(accessZone.Person);
                }
            }

            return objects;
        }

        public ICollection<AccessZone> GetAccessZonesByPerson(
            Guid idPerson, 
            out Exception error)
        {
            error = null;

            try
            {
                Person person = Persons.Singleton.GetById(idPerson);
                if (person != null)
                {
                    ICollection<AccessZone> accessZoneList =
                        GetAssignedAccessZones(
                            person.IdPerson);

                    if (accessZoneList != null)
                        return accessZoneList.OrderBy(az => az.ToString()).ToList();
                }
            }
            catch (Exception exError)
            {
                error = exError;
            }

            return null;
        }

        public ICollection<AccessZone> GetAssignedAccessZones(Guid idPerson)
        {
            ICollection<AccessZone> accessZonesDB =
                SelectLinq<AccessZone>(
                    accessZone => 
                        accessZone.Person != null && 
                        accessZone.Person.IdPerson == idPerson);

            if (accessZonesDB == null)
                return null;

            foreach (AccessZone accessZone in accessZonesDB)
                LoadObjectsInRelationship(accessZone);

            return accessZonesDB;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.AccessZone; }
        }

        public IEnumerable<ICardReaderObject> GetCardReaderObjects(AccessZone accessZone)
        {
            if (accessZone != null)
                yield return AccessControlLists.GetCardReaderObject(
                    accessZone.CardReaderObjectType,
                    accessZone.GuidCardReaderObject);
        }
    }
}
