using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Globals;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class ACLSettings :
        ANcasBaseOrmTable<ACLSettings, ACLSetting>, 
        IACLSettings
    {
        private ACLSettings()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<ACLSetting>())
        {
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

        protected override void LoadObjectsInRelationship(ACLSetting obj)
        {
            if (obj.AccessControlList != null)
            {
                obj.AccessControlList = AccessControlLists.Singleton.GetById(obj.AccessControlList.IdAccessControlList);
            }

            obj.CardReaderObject = ReadCardReaderObject(obj);

            if (obj.TimeZone != null)
            {
                obj.TimeZone = TimeZones.Singleton.GetById(obj.TimeZone.IdTimeZone);
            }
        }

        public static AOrmObject ReadCardReaderObject(ACLSetting aclSetting)
        {
            if (aclSetting.GuidCardReaderObject != Guid.Empty)
            {
                if (aclSetting.CardReaderObjectType == (byte)ObjectType.CardReader)
                    return CardReaders.Singleton.GetById(aclSetting.GuidCardReaderObject);

                if (aclSetting.CardReaderObjectType == (byte)ObjectType.AlarmArea)
                    return AlarmAreas.Singleton.GetById(aclSetting.GuidCardReaderObject);

                if (aclSetting.CardReaderObjectType == (byte)ObjectType.DoorEnvironment)
                    return DoorEnvironments.Singleton.GetById(aclSetting.GuidCardReaderObject);
                
                if (aclSetting.CardReaderObjectType == (byte)ObjectType.DCU)
                    return DCUs.Singleton.GetById(aclSetting.GuidCardReaderObject);

                if (aclSetting.CardReaderObjectType == (byte)ObjectType.MultiDoor)
                    return MultiDoors.Singleton.GetById(aclSetting.GuidCardReaderObject);

                if (aclSetting.CardReaderObjectType == (byte)ObjectType.MultiDoorElement)
                    return MultiDoorElements.Singleton.GetById(aclSetting.GuidCardReaderObject);

                if (aclSetting.CardReaderObjectType == (byte)ObjectType.Floor)
                    return Floors.Singleton.GetById(aclSetting.GuidCardReaderObject);
            }

            return null;
        }

        public IList<AccessControlList> UsedLikeCardReaderObject(Guid idObj, ObjectType typeObj)
        {
            try
            {
                var aclSettingList = SelectLinq<ACLSetting>(aclSetting => aclSetting.GuidCardReaderObject == idObj);
                if (aclSettingList != null)
                {
                    IList<AccessControlList> result = new List<AccessControlList>();
                    foreach (var aclSetting in aclSettingList)
                    {
                        result.Add(aclSetting.AccessControlList);
                    }
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }


        public ICollection<AccessControlList> GetAclForTimeZone(TimeZone timeZone)
        {
            try
            {
                if (timeZone == null) return null;

                var aclSettingsList = SelectLinq<ACLSetting>(aclSetting => aclSetting.TimeZone == timeZone);
                if (aclSettingsList == null) return null;

                ICollection<AccessControlList> resultACL = new List<AccessControlList>();
                foreach (var aclSetting in aclSettingsList)
                {
                    resultACL.Add(aclSetting.AccessControlList);
                }
                return resultACL;
            }
            catch
            {
                return null;
            }
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idACLSettings)
        {
            try
            {
                var aclSettings = GetObjectById(idACLSettings);

                if (ccus != null && aclSettings != null)
                {
                    if (aclSettings.CardReaderObject != null)
                    {
                        switch (aclSettings.CardReaderObject.GetObjectType())
                        {
                            case ObjectType.CardReader:
                                CardReaders.Singleton.GetParentCCU(ccus, (Guid)aclSettings.CardReaderObject.GetId());
                                break;
                            case ObjectType.AlarmArea:
                                AlarmAreas.Singleton.GetParentCCU(ccus, (Guid)aclSettings.CardReaderObject.GetId());
                                break;
                            case ObjectType.DoorEnvironment:
                                DoorEnvironments.Singleton.GetParentCCU(ccus, (Guid)aclSettings.CardReaderObject.GetId());
                                break;
                            case ObjectType.DCU:
                                DCUs.Singleton.GetParentCCU(ccus, (Guid)aclSettings.CardReaderObject.GetId());
                                break;
                            case ObjectType.MultiDoor:
                                MultiDoors.Singleton.GetParentCCU(ccus, (Guid)aclSettings.CardReaderObject.GetId());
                                break;
                            case ObjectType.MultiDoorElement:
                                MultiDoorElements.Singleton.GetParentCCU(ccus, (Guid)aclSettings.CardReaderObject.GetId());
                                break;
                            case ObjectType.Floor:
                                Floors.Singleton.GetParentCCU(ccus, (Guid)aclSettings.CardReaderObject.GetId());
                                break;
                        }
                    }
                }
            }
            catch { }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idACLSettings)
        {
            var objects = new List<AOrmObject>();

            var aclSettings = GetById(idACLSettings);
            if (aclSettings != null)
            {
                if (aclSettings.TimeZone != null)
                {
                    objects.Add(aclSettings.TimeZone);
                }
            }

            return objects;
        }

        public bool ExistDirectlyCardReaderAclAssigment(Guid cardReaderId)
        {
            var aclSettings = SelectLinq<ACLSetting>(
                aclSetting =>
                    aclSetting.CardReaderObjectType == (byte) ObjectType.CardReader &&
                    aclSetting.GuidCardReaderObject == cardReaderId);

            return aclSettings != null &&
                   aclSettings.Count > 0;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.ACLSetting; }
        }
    }
}



