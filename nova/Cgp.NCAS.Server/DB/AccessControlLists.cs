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
using NHibernate;
using NHibernate.Criterion;
using System.Data;
using System.Globalization;
using System.Threading;
using Contal.IwQuick.Localization;
using System.Windows.Forms;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AccessControlLists :
        ANcasBaseOrmTable<AccessControlLists, AccessControlList>, 
        IAccessControlLists
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<AccessControlList>
        {
            protected override IEnumerable<AOrmObjectWithVersion> GetSubObjects(AccessControlList obj)
            {
                var ormObjects = obj.ACLSettings != null
                    ? obj.ACLSettings
                    : Enumerable.Empty<AOrmObjectWithVersion>();

                ormObjects.Concat(obj.ACLSettingAAs != null
                    ? obj.ACLSettingAAs
                    : Enumerable.Empty<AOrmObjectWithVersion>());

                return ormObjects;
            }
        }

        private AccessControlLists()
            : base(
                  null,
                  new CudPreparation())
        {
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(
            AccessControlList accessControlList)
        {
            var aclSettings = accessControlList.ACLSettings;

            if (aclSettings != null)
                foreach (var aclSetting in aclSettings)
                {
                    ACLSettings.ReadCardReaderObject(aclSetting);
                    yield return aclSetting.CardReaderObject;

                    yield return aclSetting.TimeZone;
                }

            var aclSettingAAs = accessControlList.ACLSettingAAs;

            if (aclSettingAAs != null)
                foreach (var aclSettingAA in aclSettingAAs)
                    yield return aclSettingAA.AlarmArea;
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(AccessControlList.COLUMNNAME, true));
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.ACCESS_CONTROL_LISTS),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AclsInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.ACCESS_CONTROL_LISTS),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AclsInsertDeletePerform),
                login);
        }

        public override void CUDSpecial(AccessControlList accessControlList, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        accessControlList.GetId(),
                        accessControlList.GetObjectType()));
            }
            else if (accessControlList != null)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(accessControlList);
            }
        }

        private ICollection<AOrmObjectWithVersion> GetObjectsForDataReplication(AccessControlList accessControlList)
        {
            var objectsForDataReplication = new List<AOrmObjectWithVersion>();

            if (accessControlList.ACLSettings != null && accessControlList.ACLSettings.Count > 0)
            {
                foreach (ACLSetting aclSetting in accessControlList.ACLSettings)
                {
                    objectsForDataReplication.Add(aclSetting);
                }
            }

            if (accessControlList.ACLSettingAAs != null && accessControlList.ACLSettingAAs.Count > 0)
            {
                foreach (ACLSettingAA aclSettingAA in accessControlList.ACLSettingAAs)
                {
                    objectsForDataReplication.Add(aclSettingAA);
                }
            }

            return objectsForDataReplication;
        }

        protected override void LoadObjectsInRelationship(AccessControlList obj)
        {
            if (obj.ACLSettings != null)
            {
                IList<ACLSetting> list = new List<ACLSetting>();

                foreach (ACLSetting aclSetting in obj.ACLSettings)
                {
                    list.Add(ACLSettings.Singleton.GetById(aclSetting.IdACLSetting));
                }

                obj.ACLSettings.Clear();
                foreach (ACLSetting aclSetting in list)
                    obj.ACLSettings.Add(aclSetting);
            }

            if (obj.ACLSettingAAs != null)
            {
                IList<ACLSettingAA> list = new List<ACLSettingAA>();

                foreach (ACLSettingAA aclSettingAA in obj.ACLSettingAAs)
                {
                    list.Add(ACLSettingAAs.Singleton.GetById(aclSettingAA.IdACLSettingAA));
                }

                obj.ACLSettingAAs.Clear();
                foreach (ACLSettingAA aclSettingAA in list)
                    obj.ACLSettingAAs.Add(aclSettingAA);
            }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            //Alarm Area
            //Person
            try
            {
                AccessControlList acl = GetById(idObj);
                if (acl == null)
                    return null;

                IEnumerable<AOrmObject> result = null;

                ICollection<Person> listPersons = ACLPersons.Singleton.GetPersonsForACL(acl);
                if (listPersons != null)
                {
                    result = listPersons.Cast<AOrmObject>();
                }

                var aclGroups = ACLGroups.Singleton.GetAclGroupsForAcl(acl);
                if (aclGroups != null)
                {
                    if (result != null)
                    {
                        result = result.Concat(aclGroups.Cast<AOrmObject>());
                    }
                    else
                    {
                        result = aclGroups.Cast<AOrmObject>();
                    }
                }

                return 
                    result != null 
                        ? result.OrderBy(orm => orm.ToString()).ToList() 
                        : null;
            }
            catch
            {
                return null;
            }
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<AccessControlList> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                if (single)
                {
                    linqResult =
                        SelectLinq<AccessControlList>(
                            accessControlList => accessControlList.Name.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult =
                        SelectLinq<AccessControlList>(
                            accessControlList =>
                                accessControlList.Name.IndexOf(name) >= 0 ||
                                accessControlList.Description.IndexOf(name) >= 0);
                }
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(acl => acl.Name).ToList();
                foreach (AccessControlList accessControlList in linqResult)
                {
                    resultList.Add(GetObjectById(accessControlList.IdAccessControlList));
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<AccessControlList> linqResult = null;

            if (!string.IsNullOrEmpty(name))
                linqResult =
                    SelectLinq<AccessControlList>(
                        acl => 
                            acl.Name.IndexOf(name) >= 0 || 
                            acl.Description.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<AccessControlList> linqResult = 
                string.IsNullOrEmpty(name) 
                    ? List() 
                    : SelectLinq<AccessControlList>(acl => acl.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<AccessControlList> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(acl => acl.Name).ToList();
                foreach (AccessControlList acl in linqResult)
                {
                    resultList.Add(acl);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<AccessControlListShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<AccessControlListShort> resultList = new List<AccessControlListShort>();
            ICollection<AccessControlList> listAcl = SelectByCriteria(filterSettings, out error);
            if (listAcl != null)
            {
                foreach (AccessControlList acl in listAcl)
                {
                    resultList.Add(new AccessControlListShort(acl));
                }
            }
            return resultList;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<AccessControlList> listAcl = List(out error);
            IList<IModifyObject> listAclModifyObj = null;
            if (listAcl != null)
            {
                listAclModifyObj = new List<IModifyObject>();
                foreach (AccessControlList acl in listAcl)
                {
                    listAclModifyObj.Add(new AccessControlListModifyObj(acl));
                }
                listAclModifyObj = listAclModifyObj.OrderBy(acl => acl.ToString()).ToList();
            }
            return listAclModifyObj;
        }

        public IList<IModifyObject> ListModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<AccessControlList> listAcl = SelectByCriteria(filterSettings, out error);
            IList<IModifyObject> listAclModifyObj = null;
            if (listAcl != null)
            {
                listAclModifyObj = new List<IModifyObject>();
                foreach (AccessControlList acl in listAcl)
                {
                    listAclModifyObj.Add(new AccessControlListModifyObj(acl));
                }
                listAclModifyObj = listAclModifyObj.OrderBy(acl => acl.ToString()).ToList();
            }
            return listAclModifyObj;
        }

        public void GetParentCCU(
            ICollection<Guid> ccus,
            Guid idAccessControlList)
        {
            GetParentCCU(
                ccus,
                GetById(idAccessControlList));
        }

        public void GetParentCCU(
            ICollection<Guid> ccus,
            AccessControlList acl)
        {
            if (ccus != null && acl != null)
            {
                if (acl.ACLSettingAAs != null)
                {
                    foreach (ACLSettingAA item in acl.ACLSettingAAs)
                    {
                        if (item != null)
                        {
                            ACLSettingAAs.Singleton.GetParentCCU(ccus, item.IdACLSettingAA);
                        }
                    }
                }
                if (acl.ACLSettings != null)
                {
                    foreach (ACLSetting item in acl.ACLSettings)
                    {
                        if (item != null)
                        {
                            ACLSettings.Singleton.GetParentCCU(ccus, item.IdACLSetting);
                        }
                    }
                }
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idAccessControlList)
        {
            var objects = new List<AOrmObject>();

            AccessControlList acl = GetById(idAccessControlList);
            if (acl != null)
            {
                if (acl.ACLSettingAAs != null)
                {
                    foreach (ACLSettingAA item in acl.ACLSettingAAs)
                    {
                        if (item != null)
                        {
                            objects.Add(item);
                        }
                    }
                }

                if (acl.ACLSettings != null)
                {
                    foreach (ACLSetting item in acl.ACLSettings)
                    {
                        if (item != null)
                        {
                            objects.Add(item);
                        }
                    }
                }

                ICollection<ACLPerson> aclPersons = ACLPersons.Singleton.GetACLPersonForACL(acl);
                if (aclPersons != null)
                {
                    foreach (ACLPerson item in aclPersons)
                    {
                        objects.Add(item);
                    }
                }
            }

            return objects;
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.AccessControlList; }
        }



        public IEnumerable<ICardReaderObject> GetCardReaderObjects(
            AccessControlList accessControlList,
            DateTime dateTime)
        {
            if (accessControlList == null
                || accessControlList.ACLSettings == null)
            {
                yield break;
            }

            foreach (var aclSetting in accessControlList.ACLSettings)
            {
                if (aclSetting.Disabled == true)
                    continue;

                if (dateTime != DateTime.MinValue && aclSetting.TimeZone != null &&
                    !aclSetting.TimeZone.IsOn(dateTime))
                {
                    continue;
                }

                yield return GetCardReaderObject(
                    aclSetting.CardReaderObjectType,
                    aclSetting.GuidCardReaderObject);
            }
        }

        public static ICardReaderObject GetCardReaderObject(
            byte objectType,
            Guid idObject)
        {
            switch (objectType)
            {

                case (byte) ObjectType.CardReader:
                {
                    return CardReaders.Singleton.GetById(idObject);
                }

                case (byte) ObjectType.DoorEnvironment:
                {
                    return DoorEnvironments.Singleton.GetById(idObject);
                }

                case (byte) ObjectType.AlarmArea:
                {
                    return AlarmAreas.Singleton.GetById(idObject);
                }

                case (byte) ObjectType.DCU:
                {
                    return DCUs.Singleton.GetById(idObject);
                }

                case (byte) ObjectType.MultiDoor:
                {
                    return MultiDoors.Singleton.GetById(idObject);
                }

                case (byte) ObjectType.MultiDoorElement:
                {
                    return MultiDoorElements.Singleton.GetById(idObject);
                }

                case (byte) ObjectType.Floor:
                {
                    return Floors.Singleton.GetById(idObject);
                }

                default:
                    return null;
            }
        }
    }
}



