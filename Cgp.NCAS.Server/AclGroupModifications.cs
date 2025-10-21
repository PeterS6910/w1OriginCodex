using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

using Contal.Cgp.Server.StructuredSiteEvaluator;

namespace Contal.Cgp.NCAS.Server
{
    public class AclGroupModifications
    {
        public bool RemoveAllAcls { get; private set; }

        public GlobalEvaluator.GlobalSiteInfo OldGlobalSiteInfo { get; private set; }
        public GlobalEvaluator.GlobalSiteInfo NewGlobalSiteInfo { get; private set; }
        public ICollection<AccessControlList> OldAcls { get; private set; }
        public ICollection<AccessControlList> NewAcls { get; private set; }
        public ICollection<UserFoldersStructure> OldDepartments { get; private set; }
        public ICollection<UserFoldersStructure> NewDepartments { get; private set; }
        public bool OldImplicit { get; private set; }
        public bool NewImplicit { get; private set; }
        public bool OldUseForAllPersons { get; private set; }
        public bool NewUseForAllPersons { get; private set; }
        public bool OldUseDepAclGroupRelation { get; private set; }
        public bool NewUseDepAclGroupRelation { get; private set; }
        public bool OldApplyEndValidity { get; private set; }
        public bool NewApplyEndValidity { get; private set; }
        public byte OldYearsOfValidity { get; private set; }
        public byte NewYearsOfValidity { get; private set; }
        public byte OldMonthsOfValidity { get; private set; }
        public byte NewMonthsOfValidity { get; private set; }
        public byte OldDaysOfValidity { get; private set; }
        public byte NewDaysOfValidity { get; private set; }

        public AclGroupModifications(
            ACLGroup newAclGroup,
            GlobalEvaluator.GlobalSiteInfo newGlobalSiteInfo,
            ACLGroup oldAclGroup,
            GlobalEvaluator.GlobalSiteInfo oldGlobalSiteInfo)
        {
            if (newAclGroup != null)
            {
                NewGlobalSiteInfo = newGlobalSiteInfo;
                NewAcls = newAclGroup.AccessControlLists;
                NewDepartments = newAclGroup.Departments;
                NewImplicit = newAclGroup.IsImplicit;
                RemoveAllAcls = newAclGroup.RemoveAllAcls;
                NewUseForAllPersons = newAclGroup.UseForAllPerson;
                NewUseDepAclGroupRelation = newAclGroup.UseDepartmentAclGroupRelation;
                NewApplyEndValidity = newAclGroup.ApplyEndlessValidity;
                NewYearsOfValidity = newAclGroup.YearsOfValidity;
                NewMonthsOfValidity = newAclGroup.MonthsOfValidity;
                NewDaysOfValidity = newAclGroup.DaysOfValidity;
            }

            if (oldAclGroup != null)
            {
                OldGlobalSiteInfo = oldGlobalSiteInfo;
                OldAcls = oldAclGroup.AccessControlLists;
                OldDepartments = oldAclGroup.Departments;
                OldImplicit = oldAclGroup.IsImplicit;
                OldUseForAllPersons = oldAclGroup.UseForAllPerson;
                OldUseDepAclGroupRelation = oldAclGroup.UseDepartmentAclGroupRelation;
                OldApplyEndValidity = oldAclGroup.ApplyEndlessValidity;
                OldYearsOfValidity = oldAclGroup.YearsOfValidity;
                OldMonthsOfValidity = oldAclGroup.MonthsOfValidity;
                OldDaysOfValidity = oldAclGroup.DaysOfValidity;
            }
        }

        public AclGroupModifications(
            ACLGroup newAclGroup,
            ACLGroup oldAclGroup,
            GlobalEvaluator.GlobalSiteInfo globalSiteInfo)
            : this(
                newAclGroup,
                globalSiteInfo,
                oldAclGroup,
                globalSiteInfo)
        {

        }

        public AclGroupModifications(
            ACLGroup aclGroup,
            GlobalEvaluator.GlobalSiteInfo newGlobalSiteInfo,
            GlobalEvaluator.GlobalSiteInfo oldGlobalSiteInfo
            )
            : this(
                aclGroup,
                newGlobalSiteInfo,
                aclGroup,
                oldGlobalSiteInfo)
        {

        }

        public void CheckAclsChanges(
            out ICollection<AccessControlList> addedAcls,
            out ICollection<AccessControlList> removedAcls)
        {
            addedAcls = null;
            removedAcls = null;

            if (OldAcls == null &&
                NewAcls == null)
            {
                return;
            }

            if (OldAcls == null)
            {
                addedAcls = NewAcls;
                return;
            }

            if (NewAcls == null)
            {
                removedAcls = OldAcls;
                return;
            }

            var hashSetOldAcls = new HashSet<Guid>(
                OldAcls.Select(
                    acl =>
                        acl.IdAccessControlList));

            foreach (var acl in NewAcls.Where(
                acl =>
                    !hashSetOldAcls.Contains(acl.IdAccessControlList)))
            {
                if (addedAcls == null)
                    addedAcls = new LinkedList<AccessControlList>();

                addedAcls.Add(acl);
            }

            var hashSetNewAcls = new HashSet<Guid>(
                NewAcls.Select(
                    acl =>
                        acl.IdAccessControlList));

            foreach (var acl in OldAcls.Where(
                acl =>
                    !hashSetNewAcls.Contains(acl.IdAccessControlList)))
            {
                if (removedAcls == null)
                    removedAcls = new LinkedList<AccessControlList>();

                removedAcls.Add(acl);
            }
        }

        public bool CheckDepartmentChanges()
        {

            if (OldDepartments == null &&
                NewDepartments == null)
            {
                return false;
            }

            if (OldDepartments == null)
            {
                return true;
            }

            if (NewDepartments == null)
            {
                return true;
            }

            var hashSetOldDepartments = new HashSet<Guid>(
                OldDepartments.Select(
                    department =>
                        department.IdUserFoldersStructure));

            if (NewDepartments.Any(
                department =>
                    !hashSetOldDepartments.Contains(department.IdUserFoldersStructure)))
            {
                return true;
            }

            var hashSetNewDepartments = new HashSet<Guid>(
                NewDepartments.Select(
                    department =>
                        department.IdUserFoldersStructure));

            if (OldDepartments.Any(
                department =>
                    !hashSetNewDepartments.Contains(department.IdUserFoldersStructure)))
            {
                return true;
            }

            return false;
        }

        public bool GlobaSiteInfoWasChanged()
        {
            if (OldGlobalSiteInfo == null)
                return NewGlobalSiteInfo == null;

            if (NewGlobalSiteInfo == null)
                return OldGlobalSiteInfo == null;

            return !OldGlobalSiteInfo.Equals(NewGlobalSiteInfo);
        }
    }
}
