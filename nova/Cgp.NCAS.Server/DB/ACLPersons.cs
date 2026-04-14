using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class ACLPersons :
        ANcasBaseOrmTable<ACLPersons, ACLPerson>,
        IACLPersons
    {
        private ACLPersons()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<ACLPerson>())
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

        public override void CUDSpecial(ACLPerson aclPerson, ObjectDatabaseAction objectDatabaseAction)
        {
            if (aclPerson.AccessControlList != null)
            {
                var accessControlList = AccessControlLists.Singleton.GetById(aclPerson.AccessControlList.IdAccessControlList);

                if (accessControlList != null)
                {
                    DataReplicationManager.Singleton.SendModifiedObjectToCcus(accessControlList);
                }
            }
        }

        protected override void LoadObjectsInRelationship(ACLPerson obj)
        {
            if (obj.AccessControlList != null)
            {
                obj.AccessControlList = AccessControlLists.Singleton.GetById(obj.AccessControlList.IdAccessControlList);
            }

            if (obj.Person != null)
            {
                obj.Person = Persons.Singleton.GetById(obj.Person.IdPerson);
            }
        }

        public bool HasAccess(
            Guid idPerson,
            ObjectType objectType,
            Guid idObject)
        {
            var actDateTime = DateTime.Now;

            return AclsHasAccess(
                idPerson,
                objectType,
                idObject,
                actDateTime)
                   || AccessZonesHasAccess(
                       idPerson,
                       objectType,
                       idObject,
                       actDateTime);
        }

        private bool AclsHasAccess(
            Guid idPerson,
            ObjectType objectType,
            Guid idObject,
            DateTime dateTime)
        {
            var aclPersons = GetAssignedAclPersons(idPerson);

            if (aclPersons == null)
                return false;

            foreach (var aclPerson in aclPersons)
            {
                var dateFrom = aclPerson.DateFrom;
                var dateTo = aclPerson.DateTo;

                if (dateFrom != null && dateFrom.Value.Date > dateTime.Date)
                    continue;

                if (dateTo != null && dateTo.Value.Date < dateTime.Date)
                    continue;


                if (HasAccess(
                    AccessControlLists.Singleton.GetCardReaderObjects(
                        aclPerson.AccessControlList,
                        dateTime),
                    objectType,
                    idObject))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasAccess(
            IEnumerable<ICardReaderObject> cardReaderObjects,
            ObjectType objectType,
            Guid idObject)
        {
            return cardReaderObjects.Any(
                cardReaderObject =>
                    (cardReaderObject.GetObjectType() == objectType
                     && idObject.Equals(cardReaderObject.GetId()))
                    || HasAccess(cardReaderObject.GetChildObjects(), objectType, idObject));
        }

        private bool AccessZonesHasAccess(
            Guid idPerson,
            ObjectType objectType,
            Guid idObject,
            DateTime dateTime)
        {
            var accessZones = AccessZones.Singleton.GetAssignedAccessZones(idPerson);

            if (accessZones == null)
                return false;

            foreach (var accessZone in accessZones)
            {
                if (accessZone.TimeZone != null &&
                    !accessZone.TimeZone.IsOn(dateTime))
                {
                    continue;
                }

                if (HasAccess(
                    AccessZones.Singleton.GetCardReaderObjects(accessZone),
                    objectType,
                    idObject))
                {
                    return true;
                }
            }

            return false;
        }

        public ICollection<CardReader> LoadActiveCardReaders(Person person, out Exception error)
        {
            error = null;

            if (HasAccessView())
                return new LinkedList<CardReader>(
                    LoadActiveCardReaders(person).Values);

            error = new AccessDeniedException();
            return null;
        }

        public IDictionary<Guid, CardReader> LoadActiveCardReaders(Person person)
        {
            IDictionary<Guid, CardReader> cardReaders = 
                new Dictionary<Guid, CardReader>();

            var actDateTime = DateTime.Now;

            LoadActiveCardReadersFromACLs(person, actDateTime, cardReaders);

            LoadActiveCardReadersFromAccessZones(person, cardReaders, actDateTime);

            return cardReaders;
        }

        private static void LoadActiveCardReadersFromAccessZones(
            Person person,
            IDictionary<Guid, CardReader> cardReaders,
            DateTime actDateTime)
        {
            IList<FilterSettings> filterSettings =
                new List<FilterSettings>
                {
                    new FilterSettings(
                        AccessZone.COLUMNPERSON,
                        person,
                        ComparerModes.EQUALL)
                };

            var accessZones = 
                AccessZones.Singleton.SelectByCriteria(filterSettings);

            if (accessZones == null)
                return;

            foreach (var accessZone in accessZones)
            {
                if (accessZone.TimeZone != null &&
                    !accessZone.TimeZone.IsOn(actDateTime))
                {
                    continue;
                }

                AddCardReaders(
                    cardReaders,
                    accessZone.CardReaderObjectType,
                    accessZone.GuidCardReaderObject);
            }
        }

        private void LoadActiveCardReadersFromACLs(
            Person person,
            DateTime actDateTime,
            IDictionary<Guid, CardReader> cardReaders)
        {
            IList<FilterSettings> filterSettings =
                new List<FilterSettings>
                {
                    new FilterSettings(
                        ACLPerson.COLUMNPERSON,
                        person,
                        ComparerModes.EQUALL)
                };

            var aclPersons = SelectByCriteria(filterSettings);

            if (aclPersons == null)
                return;

            foreach (var aclPerson in aclPersons)
            {
                var dateFrom = aclPerson.DateFrom;
                var dateTo = aclPerson.DateTo;

                if (dateFrom != null && dateFrom.Value.Date > actDateTime.Date)
                    continue;

                if (dateTo != null && dateTo.Value.Date < actDateTime.Date)
                    continue;

                var accessControlList = aclPerson.AccessControlList;

                if (accessControlList == null)
                    continue;

                var aclSettings = accessControlList.ACLSettings;

                if (aclSettings == null)
                    continue;

                foreach (var aclSetting in aclSettings)
                {
                    if (aclSetting.Disabled == true)
                        continue;

                    if (aclSetting.TimeZone != null &&
                        !aclSetting.TimeZone.IsOn(actDateTime))
                    {
                        continue;
                    }

                    AddCardReaders(
                        cardReaders,
                        aclSetting.CardReaderObjectType,
                        aclSetting.GuidCardReaderObject);
                }
            }
        }

        private static void AddCardReaders(
            IDictionary<Guid, CardReader> cardReaders,
            byte cardReaderObjType,
            Guid cardReaderObjGuid)
        {
            switch (cardReaderObjType)
            {
                case (byte) ObjectType.CardReader:
                {
                    AddCardReader(cardReaders, cardReaderObjGuid);

                    return;
                }

                case (byte) ObjectType.DoorEnvironment:
                {
                    var doorEnvironment =
                        DoorEnvironments.Singleton.GetById(cardReaderObjGuid);

                    if (doorEnvironment == null)
                        return;

                    var cardReaderExternal = doorEnvironment.CardReaderExternal;

                    if (cardReaderExternal != null)
                        AddCardReader(
                            cardReaders,
                            cardReaderExternal.IdCardReader);

                    var cardReaderInternal = doorEnvironment.CardReaderInternal;

                    if (cardReaderInternal != null)
                        AddCardReader(
                            cardReaders,
                            cardReaderInternal.IdCardReader);

                    return;
                }

                case (byte) ObjectType.AlarmArea:
                {
                    var alarmArea = AlarmAreas.Singleton.GetById(cardReaderObjGuid);

                    if (alarmArea == null ||
                        alarmArea.AACardReaders == null)
                    {
                        return;
                    }

                    foreach (var aaCardReader in alarmArea.AACardReaders)
                        AddCardReader(
                            cardReaders,
                            aaCardReader.CardReader.IdCardReader);

                    return;
                }

                case (byte) ObjectType.DCU:
                {
                    var dcu = DCUs.Singleton.GetById(cardReaderObjGuid);

                    if (dcu == null ||
                        dcu.CardReaders == null)
                    {
                        return;
                    }

                    foreach (var cardReader in dcu.CardReaders)
                        AddCardReader(
                            cardReaders,
                            cardReader.IdCardReader);

                    return;
                }

                case (byte) ObjectType.MultiDoor:
                {
                    var multiDoor = MultiDoors.Singleton.GetById(cardReaderObjGuid);

                    if (multiDoor == null
                        || multiDoor.CardReader == null)
                    {
                        return;
                    }

                    AddCardReader(
                        cardReaders,
                        multiDoor.CardReader.IdCardReader);

                    return;
                }

                case (byte) ObjectType.MultiDoorElement:
                {
                    var multiDoorElement = MultiDoorElements.Singleton.GetById(cardReaderObjGuid);

                    if (multiDoorElement == null
                        || multiDoorElement.MultiDoor == null
                        || multiDoorElement.MultiDoor.CardReader == null)
                    {
                        return;
                    }

                    AddCardReader(
                        cardReaders,
                        multiDoorElement.MultiDoor.CardReader.IdCardReader);

                    return;
                }

                case (byte) ObjectType.Floor:
                {
                    var floor = Floors.Singleton.GetById(cardReaderObjGuid);

                    if (floor == null
                        || floor.Doors == null)
                    {
                        return;
                    }

                    foreach (var door in floor.Doors)
                    {
                        if (door.MultiDoor == null
                            || door.MultiDoor.CardReader == null)
                        {
                            continue;
                        }

                        AddCardReader(
                            cardReaders,
                            door.MultiDoor.CardReader.IdCardReader);
                    }

                    return;
                }
            }
        }

        private static void AddCardReader(
            IDictionary<Guid, CardReader> cardReaders,
            Guid idCardReader)
        {
            if (cardReaders.ContainsKey(idCardReader))
                return;

            var cardReader = 
                CardReaders.Singleton.GetById(idCardReader);

            if (cardReader == null)
                return;

            cardReaders.Add(
                idCardReader, 
                cardReader);
        }

        #region GetActualAccessAOrmObjects(Person person, out Exception error)

        public Dictionary<Guid, AOrmObject> GetActualAccessAOrmObjects(
            Person person, 
            out Dictionary<Guid, object> objectStates, 
            out Exception error)
        {
            error = null;
            objectStates = null;

            if (HasAccessView())
                return GetActualAccessAOrmObjects(
                    person, 
                    out objectStates);

            error = new AccessDeniedException();

            return null;
        }

        public Dictionary<Guid, AOrmObject> GetActualAccessAOrmObjects(
            Person person, 
            out Dictionary<Guid, object> objectStates)
        {
            var aOrmObjectsAccessList = new Dictionary<Guid, AOrmObject>();

            objectStates = new Dictionary<Guid, object>();

            IList<FilterSettings> filterSettings = 
                new List<FilterSettings>
                {
                    new FilterSettings(
                        ACLPerson.COLUMNPERSON,
                        person,
                        ComparerModes.EQUALL)
                };

            AddACLPersonsAccessAOrmObjects(
                filterSettings, 
                aOrmObjectsAccessList, 
                ref objectStates);

            AddAccessZoneAccessAOrmObjects(
                filterSettings, 
                aOrmObjectsAccessList, 
                ref objectStates);

            return aOrmObjectsAccessList;
        }

        private static void AddACLPersonsAccessAOrmObjects(
            IList<FilterSettings> filterSettings,
            Dictionary<Guid, AOrmObject> aOrmObjectsAccessDictionary,
            ref Dictionary<Guid, object> objectStates)
        {
            var aclPersons = 
                Singleton.SelectByCriteria(filterSettings);

            if (aclPersons == null)
                return;

            var actualDateTime = DateTime.Now;

            foreach (var aclPerson in aclPersons)
            {
                var dateFrom = aclPerson.DateFrom;
                var dateTo = aclPerson.DateTo;

                if (dateFrom != null && 
                    dateFrom.Value.Date > actualDateTime.Date)
                {
                    continue;
                }

                if (dateTo != null && 
                    dateTo.Value.Date < actualDateTime.Date)
                {
                    continue;
                }

                var accessControlList = 
                    aclPerson.AccessControlList;

                if (accessControlList == null)
                    continue;

                var aclSettings = 
                    accessControlList.ACLSettings;

                if (aclSettings == null)
                    continue;

                foreach (var aclSetting in aclSettings)
                {
                    if (aclSetting.Disabled != false)
                        continue;

                    var timeZone = aclSetting.TimeZone;

                    if (timeZone != null && !timeZone.IsOn(actualDateTime))
                        continue;

                    AddAccessAOrmObjectByType(
                        aOrmObjectsAccessDictionary,
                        ref objectStates,
                        aclSetting.CardReaderObjectType,
                        aclSetting.GuidCardReaderObject);
                }
            }
        }

        private static void AddAccessZoneAccessAOrmObjects(
            IList<FilterSettings> filterSettings,
            Dictionary<Guid, AOrmObject> aOrmObjectsAccessDictionary,
            ref Dictionary<Guid, object> objectStates)
        {
            var accessZones = AccessZones.Singleton.SelectByCriteria(filterSettings);
            if (accessZones == null)
                return;

            foreach (var accessZone in accessZones)
            {
                var timeZone = accessZone.TimeZone;

                if (timeZone != null && !timeZone.IsOn(DateTime.Now))
                    continue;

                AddAccessAOrmObjectByType(
                    aOrmObjectsAccessDictionary,
                    ref objectStates,
                    accessZone.CardReaderObjectType,
                    accessZone.GuidCardReaderObject);
            }
        }

        private static void AddAccessAOrmObjectByType(
            Dictionary<Guid, AOrmObject> aOrmObjectsAccessDictionary,
            ref Dictionary<Guid, object> objectStates,
            byte objectType,
            Guid guidAccessObject)
        {
            switch (objectType)
            {
                case (byte) ObjectType.AlarmArea:

                    var alarmArea = AlarmAreas.Singleton.GetObjectById(guidAccessObject);

                    if (alarmArea != null)
                    {
                        AddAccessAOrmObjectToDictionary(aOrmObjectsAccessDictionary, alarmArea);
                        AddAlarmAreaObjectState(objectStates, alarmArea);
                    }

                    return;

                case (byte) ObjectType.DCU:

                    var dcu = DCUs.Singleton.GetObjectById(guidAccessObject);

                    if (dcu != null)
                    {
                        AddAccessAOrmObjectToDictionary(aOrmObjectsAccessDictionary, dcu);
                        AddDCUState(objectStates, dcu);
                    }

                    return;

                case (byte) ObjectType.DoorEnvironment:

                    var doorEnvironment = DoorEnvironments.Singleton.GetObjectById(guidAccessObject);

                    if (doorEnvironment != null)
                    {
                        AddAccessAOrmObjectToDictionary(aOrmObjectsAccessDictionary, doorEnvironment);
                        AddDoorEnvironmentState(objectStates, doorEnvironment);
                    }

                    return;

                case (byte) ObjectType.CardReader:

                    var cardReader = CardReaders.Singleton.GetById(guidAccessObject);

                    if (cardReader != null)
                    {
                        AddAccessAOrmObjectToDictionary(aOrmObjectsAccessDictionary, cardReader);
                        AddCardReaderState(objectStates, cardReader);
                    }

                    return;

                case (byte) ObjectType.MultiDoor:

                    var multiDoor = MultiDoors.Singleton.GetObjectById(guidAccessObject);

                    if (multiDoor != null)
                    {
                        AddAccessAOrmObjectToDictionary(aOrmObjectsAccessDictionary, multiDoor);
                        AddMultiDoorState(objectStates, multiDoor);
                    }

                    return;

                case (byte) ObjectType.MultiDoorElement:

                    var multiDoorElement = MultiDoorElements.Singleton.GetObjectById(guidAccessObject);

                    if (multiDoorElement != null)
                    {
                        AddAccessAOrmObjectToDictionary(aOrmObjectsAccessDictionary, multiDoorElement);
                        AddMultiDoorElementState(objectStates, multiDoorElement);
                    }

                    return;

                case (byte)ObjectType.Floor:

                    var floor = Floors.Singleton.GetObjectById(guidAccessObject);

                    if (floor != null)
                    {
                        AddAccessAOrmObjectToDictionary(aOrmObjectsAccessDictionary, floor);
                        AddFloorState(objectStates, floor);
                    }

                    return;
            }
        }

        private static void AddAlarmAreaObjectState(
            Dictionary<Guid, object> objectStates, 
            AlarmArea alarmArea)
        {
            try
            {
                foreach (var aaCardReader in alarmArea.AACardReaders)
                {
                    var aCardReader = CardReaders.Singleton.GetById(aaCardReader.CardReader.IdCardReader);
                    aaCardReader.CardReader = aCardReader;
                    AddCardReaderState(objectStates, aCardReader);
                }
            }
            catch { }
        }

        private static void AddDCUState(Dictionary<Guid, object> objectStates, DCU dcu)
        {
            try
            {
                foreach (var dcuCardReader in dcu.CardReaders)
                    AddCardReaderState(
                        objectStates, 
                        dcuCardReader);
            }
            catch { }
        }

        private static void AddDoorEnvironmentState(
            Dictionary<Guid, object> objectStates, 
            DoorEnvironment doorEnvironment)
        {
            try
            {
                var cardReaderExternal = doorEnvironment.CardReaderExternal;

                if (cardReaderExternal != null) 
                    AddCardReaderState(
                        objectStates, 
                        cardReaderExternal);

                var cardReaderInternal = doorEnvironment.CardReaderInternal;

                if (cardReaderInternal != null) 
                    AddCardReaderState(
                        objectStates, 
                        cardReaderInternal);
            }
            catch { }
        }

        private static void AddMultiDoorState(
            Dictionary<Guid, object> objectStates,
            MultiDoor multiDoor)
        {
            try
            {
                var cardReader = multiDoor.CardReader;

                if (cardReader == null)
                    return;

                AddCardReaderState(
                    objectStates,
                    cardReader);
            }
            catch
            {
            }
        }

        private static void AddMultiDoorElementState(
            Dictionary<Guid, object> objectStates,
            MultiDoorElement multiDoorElement)
        {
            try
            {
                if (multiDoorElement.MultiDoor == null)
                    return;

                var cardReader = multiDoorElement.MultiDoor.CardReader;

                if (cardReader == null)
                    return;

                AddCardReaderState(
                    objectStates,
                    cardReader);
            }
            catch
            {
            }
        }

        private static void AddFloorState(
            Dictionary<Guid, object> objectStates,
            Floor floor)
        {
            try
            {
                if (floor.Doors == null)
                    return;

                foreach (var door in floor.Doors)
                {
                    if (door.MultiDoor == null
                        || door.MultiDoor.CardReader == null)
                    {
                        continue;
                    }

                    AddCardReaderState(
                        objectStates,
                        door.MultiDoor.CardReader);
                }
            }
            catch
            {
            }
        }

        private static void AddCardReaderState(
            Dictionary<Guid, object> objectStates, 
            CardReader cardReader)
        {
            try
            {
                if (objectStates.ContainsKey(cardReader.IdCardReader))
                    return;

                OnlineState onlineStates = 
                    CardReaders.Singleton.GetOnlineStates(cardReader.IdCardReader);

                objectStates.Add(
                    cardReader.IdCardReader,
                    onlineStates);
            }
            catch { }
        }

        private static void AddAccessAOrmObjectToDictionary(Dictionary<Guid, AOrmObject> aOrmObjectsAccessDictionary, AOrmObject aOrmObject)
        {
            try
            {
                var id = (Guid)aOrmObject.GetId();

                if (!aOrmObjectsAccessDictionary.Keys.Contains(id))
                    aOrmObjectsAccessDictionary.Add(
                        id, 
                        aOrmObject);
            }
            catch { }
        }

        #endregion

        public ICollection<ACLPerson> GetACLPersonForACL(AccessControlList acl)
        {
            try
            {
                return 
                    acl != null
                        ? SelectLinq<ACLPerson>(aclPerson => aclPerson.AccessControlList == acl)
                        : null;
            }
            catch
            {
                return null;
            }
        }

        public ICollection<Person> GetPersonsForACL(AccessControlList acl)
        {
            try
            {
                var aclPersonList = 
                    SelectLinq<ACLPerson>(
                        aclPerson => aclPerson.AccessControlList == acl);

                if (aclPersonList != null && aclPersonList.Count > 0)
                {
                    IList<Person> result = new List<Person>();

                    foreach (var aclPerson in aclPersonList)
                        result.Add(aclPerson.Person);

                    return result;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public ICollection<ACLPerson> GetAclPersonsForPerson(Person person)
        {
            return
                person != null 
                    ? SelectLinq<ACLPerson>(aclPerson => aclPerson.Person == person)
                    : null;
        }

        public ICollection<AccessControlList> GetAclForPerson(Person person)
        {
            try
            {
                if (person == null) 
                    return null;

                var aclPersonList = 
                    SelectLinq<ACLPerson>(aclPerson => aclPerson.Person == person);

                if (aclPersonList == null) 
                    return null;

                ICollection<AccessControlList> resultACL = 
                    new List<AccessControlList>();

                foreach (var aclPerson in aclPersonList)
                    resultACL.Add(aclPerson.AccessControlList);
                
                return resultACL;
            }
            catch
            {
                return null;
            }
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idACLPerson)
        {
            var aclPersons = GetById(idACLPerson);

            if (ccus == null || aclPersons == null)
                return;

            if (aclPersons.AccessControlList != null)
                AccessControlLists.Singleton.GetParentCCU(
                    ccus,
                    aclPersons.AccessControlList.IdAccessControlList);
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idACLPerson)
        {
            var objects = new List<AOrmObject>();

            var aclPersons = GetById(idACLPerson);

            if (aclPersons == null)
                return objects;

            if (aclPersons.Person != null)
                objects.Add(aclPersons.Person);

            return objects;
        }

        private static Guid GetAlarmAreaCardReadersParentCCU(AlarmArea alarmArea)
        {
            if (alarmArea.AACardReaders == null)
                return Guid.Empty;

            foreach (var aaCr in alarmArea.AACardReaders)
            {
                var cardReader = aaCr.CardReader;

                var dcu = cardReader.DCU;

                return 
                    dcu != null 
                        ? dcu.CCU.IdCCU
                        : cardReader.CCU.IdCCU;
            }

            return Guid.Empty;
        }

        private static List<Guid> GetACLCCUGuids(AccessControlList acl)
        {
            var guidsCCU = new List<Guid>();

            var aclSettings = acl.ACLSettings;

            if (aclSettings == null)
                return guidsCCU;

            foreach (var acls in aclSettings)
            {
                try
                {
                    switch (acls.CardReaderObjectType)
                    {
                        case (byte)ObjectType.DCU:
                        {
                            var dcu = DCUs.Singleton.GetById(acls.GuidCardReaderObject);

                            guidsCCU.Add(dcu.CCU.IdCCU);
                            break;
                        }

                        case (byte)ObjectType.AlarmArea:
                        {
                            var alarmArea = AlarmAreas.Singleton.GetById(acls.GuidCardReaderObject);

                            guidsCCU.Add(GetAlarmAreaCardReadersParentCCU(alarmArea));
                            break;
                        }

                        case (byte)ObjectType.DoorEnvironment:
                        {
                            var doorEnvironment =
                                DoorEnvironments.Singleton.GetById(acls.GuidCardReaderObject);

                            guidsCCU.Add(
                                doorEnvironment.DCU != null
                                    ? doorEnvironment.DCU.CCU.IdCCU
                                    : doorEnvironment.CCU.IdCCU);

                            break;
                        }

                        case (byte)ObjectType.CardReader:
                        {
                            var cardReader = CardReaders.Singleton.GetById(acls.GuidCardReaderObject);

                            guidsCCU.Add(
                                cardReader.DCU != null
                                    ? cardReader.DCU.CCU.IdCCU
                                    : cardReader.CCU.IdCCU);

                            break;
                        }
                    }
                }
                catch { }
            }

            return guidsCCU;
        }

        public string GetAlarmAreaActivationRights(
            Guid guidCCU, 
            Person person, 
            AlarmArea alarmArea)
        {
            if (person == null || alarmArea == null)
            {
                return string.Empty;
            }

            var aclPersonList = 
                SelectLinq<ACLPerson>(
                    az => 
                        az.Person == person
                        && (az.DateFrom <= DateTime.Now || az.DateFrom == null)
                        && (az.DateTo >= DateTime.Now || az.DateTo == null));

            var results = new bool[5];

            foreach (var aclPerson in aclPersonList)
            {
                var accessControlList = aclPerson.AccessControlList;

                if (accessControlList == null)
                    continue;

                if (!GetACLCCUGuids(accessControlList).Contains(guidCCU))
                    continue;

                foreach (var aclSettingAA in accessControlList.ACLSettingAAs)
                {
                    var alarmArea1 = aclSettingAA.AlarmArea;

                    if (aclSettingAA.AlarmAreaSet && alarmArea1.IdAlarmArea == alarmArea.IdAlarmArea) 
                        results[0] = true;

                    if (aclSettingAA.AlarmAreaUnset && alarmArea1.IdAlarmArea == alarmArea.IdAlarmArea) 
                        results[1] = true;

                    if (aclSettingAA.AlarmAreaUnconditionalSet && alarmArea1.IdAlarmArea == alarmArea.IdAlarmArea) 
                        results[2] = true;

                    if (aclSettingAA.SensorHandling && alarmArea1.IdAlarmArea == alarmArea.IdAlarmArea) 
                        results[3] = true;

                    if (aclSettingAA.AlarmAreaTimeBuying && alarmArea1.IdAlarmArea == alarmArea.IdAlarmArea)
                        results[4] = true;
                }
            }

            return GetResultInfo(results);
        }

        private static string GetResultInfo(bool[] results)
        {
            var resultsInfo = string.Empty;
            resultsInfo += results[0] ? "Set: true\n" : "Set: false\n";

            resultsInfo += results[1] ? "Unset: true\n" : "Unset: false\n";

            resultsInfo += results[2] ? "Unconditional set: true\n" : "Unconditional set: false\n";

            resultsInfo += results[3] ? "Sensor handling: true\n" : "Sensor handling: false\n";

            resultsInfo += results[4] ? "Time buying: true\n" : "Time buying: false\n";

            return resultsInfo;
        }

        public ICollection<ACLPerson> GetAclPersonsByPerson(Guid idPerson, out Exception error)
        {
            error = null;
            ICollection<ACLPerson> resultAclPerson = new List<ACLPerson>();

            try
            {
                var person = Persons.Singleton.GetById(idPerson);
                if (person != null)
                {
                    var aclPersonList = GetAssignedAclPersons(person.IdPerson);

                    if (aclPersonList != null)
                        return
                            aclPersonList
                                .OrderBy(acl => acl.ToString())
                                .ToList();
                }
            }
            catch (Exception exError)
            {
                error = exError;
            }
            return resultAclPerson;
        }

        public IList<String> PersonAclAssignment(
            IList<Object> persons, 
            IList<Guid> idAcls, 
            DateTime? dateFrom, 
            DateTime? dateTo)
        {
            IList<String> errorsStr = new List<string>();
            var acls = new List<AOrmObjectWithVersion>();
            var listAclPersons = new List<AOrmObject>();

            foreach (var idAcl in idAcls)
            {
                var acl = AccessControlLists.Singleton.GetById(idAcl);
                if (acl == null) continue;

                acls.Add(acl);

                foreach (var objIdPerson in persons)
                {
                    var person = Persons.Singleton.GetById(objIdPerson);
                    if (person == null) continue;
                    try
                    {
                        ACLPerson tmpAclPerson = AssignAclPerson(
                            person,
                            acl,
                            dateFrom,
                            dateTo);
                    }
                    catch
                    {
                        errorsStr.Add(GetErrorString(person, acl));
                    }
                }
            }

            DataReplicationManager.Singleton.SendModifiedObjectsToCcus(acls);

            return 
                errorsStr.Count != 0 
                    ? errorsStr
                    : null;
        }

        private static string GetErrorString(Person person, AccessControlList acl)
        {
            var resultStr = string.Empty;
            try
            {
                if (person != null)
                    resultStr += person + " ";

                if (acl != null)
                    resultStr += acl.ToString();
            }
            catch { }
            return resultStr;
        }

        private ACLPerson AssignAclPerson(Person person, AccessControlList acl, DateTime? dateFrom, DateTime? dateTo)
        {
            if (person == null || acl == null) return null;
            if (person.EmploymentEndDate != null &&
                (dateTo == null || dateTo > person.EmploymentEndDate))
            {
                dateTo = person.EmploymentEndDate;
            }

            if (dateFrom != null && dateTo != null && (dateFrom.Value > dateTo.Value)) return null;

            var aclPerson = GetAssignedAclPerson(person, acl);
            if (aclPerson == null)
            {
                aclPerson = new ACLPerson
                {
                    AccessControlList = acl,
                    Person = person,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                };
                if (InsertOnlyInDatabase(ref aclPerson))
                    return aclPerson;
            }
            else
            {
                if (aclPerson.DateFrom != dateFrom || aclPerson.DateTo != dateTo)
                {
                    aclPerson = GetObjectForEdit(aclPerson.IdACLPerson);
                    aclPerson.DateFrom = dateFrom;
                    aclPerson.DateTo = dateTo;

                    if (UpdateOnlyInDatabase(aclPerson))
                        return aclPerson;
                }
            }
            return null;
        }

        private ACLPerson GetAssignedAclPerson(Person person, AccessControlList acl)
        {
            var result = SelectLinq<ACLPerson>(aclperson => aclperson.Person == person && aclperson.AccessControlList == acl);
            if (result != null && result.Count > 0)
            {
                return result.First();
            }
            return null;
        }

        public ICollection<ACLPerson> GetAssignedAclPersons(Guid idPerson)
        {
            var aclPersonsDB = GetAssignedAclPersonsCore(idPerson);

            if (aclPersonsDB == null)
                return null;

            foreach (var aclPerson in aclPersonsDB)
                LoadObjectsInRelationship(aclPerson);

            return aclPersonsDB;
        }

        private ICollection<ACLPerson> GetAssignedAclPersonsCore(Guid idPerson)
        {
            return SelectLinq<ACLPerson>(
                    aclperson =>
                        aclperson.Person != null &&
                        aclperson.Person.IdPerson == idPerson);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.ACLPerson; }
        }

        public override void AfterInsert(ACLPerson newObject)
        {
            if (newObject.Person != null)
                Persons.DoCUDObjectEvent(ObjectType.Person, newObject.Person.IdPerson, false);
        }

        public override void AfterUpdate(ACLPerson newObject, ACLPerson oldObjectBeforUpdate)
        {
            if (oldObjectBeforUpdate.Person != null)
            {
                Persons.DoCUDObjectEvent(ObjectType.Person, oldObjectBeforUpdate.Person.IdPerson, false);

                if (newObject.Person == null
                    || newObject.Person.IdPerson == oldObjectBeforUpdate.Person.IdPerson)
                {
                    return;
                }

                Persons.DoCUDObjectEvent(ObjectType.Person, newObject.Person.IdPerson, false);

                return;
            }

            if (newObject.Person != null)
                Persons.DoCUDObjectEvent(ObjectType.Person, newObject.Person.IdPerson, false);
        }

        public override void AfterDelete(ACLPerson deletedObject)
        {
            if (deletedObject.Person != null)
                Persons.DoCUDObjectEvent(ObjectType.Person, deletedObject.Person.IdPerson, false);
        }

        public void PersonEmploymentEndDateChanged(Guid personId)
        {
            var person = Persons.Singleton.GetById(personId);

            if (person == null
                || !person.EmploymentEndDate.HasValue)
            {
                return;
            }

            var aclPersons = GetAssignedAclPersonsCore(personId);

            if (aclPersons == null)
                return;

            var employmentEndDate = person.EmploymentEndDate.Value;

            foreach (var aclPerson in aclPersons)
            {
                var aclPersonForEdit = GetObjectForEdit(aclPerson.IdACLPerson);

                if (!aclPersonForEdit.DateTo.HasValue
                    || aclPersonForEdit.DateTo.Value > employmentEndDate)
                {
                    aclPersonForEdit.DateTo = employmentEndDate;
                    Update(aclPersonForEdit);
                }

                EditEnd(aclPersonForEdit);
            }
        }
    }
}
