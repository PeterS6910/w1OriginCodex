using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.Server.DB;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class ACLSettingAAs :
        ANcasBaseOrmTable<ACLSettingAAs, ACLSettingAA>, 
        IACLSettingAAs
    {
        private ACLSettingAAs()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<ACLSettingAA>())
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

        protected override void LoadObjectsInRelationship(ACLSettingAA obj)
        {
            if (obj.AccessControlList != null)
            {
                obj.AccessControlList = AccessControlLists.Singleton.GetById(obj.AccessControlList.IdAccessControlList);
            }

            if (obj.AlarmArea != null)
            {
                obj.AlarmArea = AlarmAreas.Singleton.GetById(obj.AlarmArea.IdAlarmArea);
            }
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idACLSettingAA)
        {
            var aclSettingAA = GetById(idACLSettingAA);
            if (ccus != null && aclSettingAA != null)
            {
                if (aclSettingAA.AlarmArea != null)
                {
                    AlarmAreas.Singleton.GetParentCCU(ccus, aclSettingAA.AlarmArea.IdAlarmArea);
                }
                //else if (aclSettingAA.AccessControlList != null)
                //{
                //    AccessControlLists.Singleton.GetParentCCU(ccus, aclSettingAA.AccessControlList.IdAccessControlList);
                //}
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idACLSettingAA)
        {
            var objects = new List<AOrmObject>();

            var aclSettingAA = GetById(idACLSettingAA);
            if (aclSettingAA != null)
            {               
                if (aclSettingAA.AlarmArea != null)
                {
                    var ccu = AlarmAreas.Singleton.GetImplicitCCUForAlarmArea(aclSettingAA.AlarmArea.IdAlarmArea);
                    if (ccu != null && guidCCU == ccu.IdCCU)
                    {
                        objects.Add(aclSettingAA.AlarmArea);
                    }
                }
            }

            return objects;
        }

        public IList<AccessControlList> GetReferencedAclFormAa(AlarmArea alarmArea)
        {
            IList<AccessControlList> refAcl = new List<AccessControlList>();
            var selected = SelectLinq<ACLSettingAA>(aclAa => aclAa.AlarmArea == alarmArea 
                && (aclAa.AlarmAreaSet || aclAa.AlarmAreaUnconditionalSet || aclAa.AlarmAreaUnset || aclAa.SensorHandling));

            if (selected != null)
            {
                foreach (var aclAa in selected)
                {
                    if (aclAa.AccessControlList != null)
                        refAcl.Add(aclAa.AccessControlList);
                }
            }

            return refAcl;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.ACLSettingAA; }
        }
    }
}
