using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.StructuredSiteEvaluator;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using NHibernate.Criterion;
using NHibernate;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class ACLGroups :
        ANcasBaseOrmTable<ACLGroups, ACLGroup>, 
        IACLGroups
    {
        private readonly ProcessingQueue<DVoid2Void> _onAclGroupOrPersonChanged;

        private ACLGroups() : base(null)
        {
            _onAclGroupOrPersonChanged = new ProcessingQueue<DVoid2Void>();
            _onAclGroupOrPersonChanged.ItemProcessing += lambda => lambda();
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.ACL_GROUPS),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AclGroupsInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.ACL_GROUPS),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AclGroupsInsertDeletePerform),
                login);
        }

        protected override void LoadObjectsInRelationship(ACLGroup obj)
        {
            if (obj.Departments != null)
            {
                var departmentsSnapShot = obj.Departments.ToArray();

                obj.Departments.Clear();
                foreach (UserFoldersStructure department in departmentsSnapShot)
                {
                    obj.Departments.Add(
                        UserFoldersStructures.Singleton.GetById(department.IdUserFoldersStructure));
                }
            }

            if (obj.AccessControlLists != null)
            {
                var accessControlListsSnapShot = obj.AccessControlLists.ToArray();

                obj.AccessControlLists.Clear();
                foreach (AccessControlList accessControlList in accessControlListsSnapShot)
                {
                    obj.AccessControlLists.Add(
                        AccessControlLists.Singleton.GetById(accessControlList.IdAccessControlList));
                }
            }
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(
            ACLGroup aclGroup)
        {
            var acls = aclGroup.AccessControlLists;

            if (acls != null)
            {
                return acls.Cast<AOrmObject>();
            }

            return null;
        }

        public ICollection<ACLGroupShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<ACLGroup> aclGroups = SelectByCriteria(filterSettings, out error);
            ICollection<ACLGroupShort> aclGroupsShort = new List<ACLGroupShort>();
            if (aclGroups != null && aclGroups.Count > 0)
            {
                foreach (ACLGroup aclGroup in aclGroups)
                {
                    aclGroupsShort.Add(new ACLGroupShort(aclGroup));
                }
            }

            return aclGroupsShort;
        }

        private IEnumerable<ACLGroup> GetAclGroupsForPersonsWhithoutDepartment(
            GlobalEvaluator.GlobalSiteInfo globalSiteInfoForPerson)
        {
            return
                StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsFromSite(
                    globalSiteInfoForPerson,
                    SelectLinq<ACLGroup>(
                        aclGroup =>
                            aclGroup.IsImplicit == true));
        }

        private IEnumerable<ACLGroup> GetAclGroupsForAllPersons(
            GlobalEvaluator.GlobalSiteInfo globalSiteInfoForPerson)
        {
            return
                StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsFromSameSiteAndParentSites(
                    globalSiteInfoForPerson,
                    SelectLinq<ACLGroup>(
                        aclGroup =>
                            aclGroup.UseForAllPerson == true));
        }

        private IEnumerable<ACLGroup> GetAclGroupsForDepartment(
            GlobalEvaluator.GlobalSiteInfo globalSiteInfoForPerson,
            UserFoldersStructure department)
        {
            return
                StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsFromSite(
                    globalSiteInfoForPerson,
                    SelectLinq<ACLGroup>(
                        aclGroup =>
                            aclGroup.Departments.Contains(department) &&
                            aclGroup.UseDepartmentAclGroupRelation == true));
        }

        private IEnumerable<ACLGroup> GetAclGroupsForPerson(
            GlobalEvaluator.GlobalSiteInfo globalSiteInfoForPerson,
            UserFoldersStructure department)
        {
            var aclGroups = GetAclGroupsForAllPersons(globalSiteInfoForPerson);

            var aclGroupForDepartment = department != null
                ? GetAclGroupsForDepartment(globalSiteInfoForPerson, department)
                : GetAclGroupsForPersonsWhithoutDepartment(globalSiteInfoForPerson);


            return aclGroups.Concat(aclGroupForDepartment);
        }

        public ICollection<ACLGroup> GetAclGroupsForAcl(AccessControlList accessControlList)
        {
            return SelectLinq<ACLGroup>(aclGroup => aclGroup.AccessControlLists.Contains(accessControlList));
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name, bool single)
        {
            IEnumerable<ACLGroup> linqResult;

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
                        SelectLinq<ACLGroup>(
                            aclGroup => aclGroup.Name.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult =
                        SelectLinq<ACLGroup>(
                            aclGroup =>
                                aclGroup.Name.IndexOf(name) >= 0 ||
                                aclGroup.Description.IndexOf(name) >= 0);
                }
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<ACLGroup> linqResult = null;

            if (!string.IsNullOrEmpty(name))
                linqResult =
                    SelectLinq<ACLGroup>(
                        aclGroup =>
                            aclGroup.Name.IndexOf(name) >= 0 ||
                            aclGroup.Description.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<ACLGroup> linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<ACLGroup>(aclGroup => aclGroup.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(IEnumerable<ACLGroup> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(acl => acl.Name);
                foreach (ACLGroup aclGroup in linqResult)
                {
                    resultList.Add(aclGroup);
                }
            }

            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public override void AfterInsert(ACLGroup newAclGroup)
        {
            if (newAclGroup == null)
                return;

            var newAclGropuFromDatabase = GetById(newAclGroup.IdACLGroup);

            _onAclGroupOrPersonChanged.Enqueue(
                () => ApplyChangesForAclGroup(
                    new AclGroupModifications(
                        newAclGropuFromDatabase,
                        (ACLGroup)null,
                        StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                            newAclGroup.IdACLGroup,
                            ObjectType.ACLGroup) ?? StructuredSubSites.Singleton.GlobalEvaluator.Root)));
        }

        public override void BeforeUpdate(ACLGroup oldAclGroup)
        {
            LoadObjectsInRelationship(oldAclGroup);
        }

        public override void AfterUpdate(ACLGroup newAclGroup, ACLGroup oldAclGroup)
        {
            if (newAclGroup == null ||
                oldAclGroup == null)
                return;

            var newAclGropuFromDatabase = GetById(newAclGroup.IdACLGroup);

            _onAclGroupOrPersonChanged.Enqueue(
                () => ApplyChangesForAclGroup(
                    new AclGroupModifications(
                        newAclGropuFromDatabase,
                        oldAclGroup,
                        StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                            newAclGroup.IdACLGroup,
                            ObjectType.ACLGroup))));

;
        }

        public override void BeforeDelete(ACLGroup aclGroupToDelete)
        {
            LoadObjectsInRelationship(aclGroupToDelete);
        }

        public override void AfterDelete(ACLGroup deletedAclGroup)
        {
            if (deletedAclGroup == null)
                return;

            _onAclGroupOrPersonChanged.Enqueue(
                () => ApplyChangesForAclGroup(
                    new AclGroupModifications(
                        null,
                        deletedAclGroup,
                        StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                            deletedAclGroup.IdACLGroup,
                            ObjectType.ACLGroup))));
        }

        private void ApplyChangesForAclGroup(AclGroupModifications aclGroupModifications)
        {
            if (aclGroupModifications == null)
                return;

            ICollection<AccessControlList> addedAcls;
            ICollection<AccessControlList> removedAcls;

            aclGroupModifications.CheckAclsChanges(out addedAcls, out removedAcls);

            var departmentsWasChanged = aclGroupModifications.CheckDepartmentChanges();
            var globalSiteInfoWasChanged = aclGroupModifications.GlobaSiteInfoWasChanged();

            if (addedAcls == null
                && removedAcls == null
                && !departmentsWasChanged
                && !globalSiteInfoWasChanged
                && aclGroupModifications.OldImplicit == aclGroupModifications.NewImplicit
                && aclGroupModifications.OldUseForAllPersons == aclGroupModifications.NewUseForAllPersons
                && aclGroupModifications.OldUseDepAclGroupRelation == aclGroupModifications.NewUseDepAclGroupRelation
                && aclGroupModifications.OldApplyEndValidity == aclGroupModifications.NewApplyEndValidity
                && aclGroupModifications.OldYearsOfValidity == aclGroupModifications.NewYearsOfValidity
                && aclGroupModifications.OldMonthsOfValidity == aclGroupModifications.NewMonthsOfValidity
                && aclGroupModifications.OldDaysOfValidity == aclGroupModifications.NewDaysOfValidity)
            {
                return;
            }

            ICollection<Person> oldPersons;
            if (aclGroupModifications.OldUseForAllPersons)
            {
                oldPersons =
                    new LinkedList<Person>(
                        StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsFromSameSiteAndChildSites(
                            aclGroupModifications.OldGlobalSiteInfo,
                            Persons.Singleton.List()));
            }
            else
            {
                oldPersons = FilterPersonsForAclGroupDepartments(
                    aclGroupModifications.OldDepartments,
                    aclGroupModifications.OldUseDepAclGroupRelation,
                    aclGroupModifications.OldImplicit,
                    StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsFromSite(
                        aclGroupModifications.OldGlobalSiteInfo,
                        Persons.Singleton.List()));
            }

            ICollection<Person> newPersons = null;
            if (departmentsWasChanged ||
                globalSiteInfoWasChanged ||
                aclGroupModifications.OldImplicit != aclGroupModifications.NewImplicit ||
                aclGroupModifications.OldUseForAllPersons != aclGroupModifications.NewUseForAllPersons ||
                aclGroupModifications.OldUseDepAclGroupRelation != aclGroupModifications.NewUseDepAclGroupRelation)
            {
                if (aclGroupModifications.NewUseForAllPersons)
                {
                    newPersons =
                        new LinkedList<Person>(
                            StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsFromSameSiteAndChildSites(
                                aclGroupModifications.NewGlobalSiteInfo,
                                Persons.Singleton.List()));
                }
                else
                {
                    newPersons = FilterPersonsForAclGroupDepartments(
                        aclGroupModifications.NewDepartments,
                        aclGroupModifications.NewUseDepAclGroupRelation,
                        aclGroupModifications.NewImplicit,
                        StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsFromSite(
                            aclGroupModifications.NewGlobalSiteInfo,
                            Persons.Singleton.List()));
                }
            }

            var dateTimeNow = DateTime.Now;

            if (newPersons == null)
            {
                foreach (var oldPerson in oldPersons)
                {
                    var aclGroupsForPerson = new LinkedList<ACLGroup>(
                        GetAclGroupsForPerson(
                            StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                                oldPerson.IdPerson,
                                ObjectType.Person),
                            UserFoldersStructures.Singleton.GetPersonDepartment(
                                oldPerson.IdPerson.ToString())));

                    var aclPersonsByAclId = GetAclPersonsByAclId(oldPerson);

                    AddAndModifyAclPersons(
                        oldPerson,
                        false,
                        aclGroupsForPerson,
                        aclPersonsByAclId,
                        aclGroupModifications.NewAcls,
                        dateTimeNow);

                    if (removedAcls != null)
                    {
                        RemoveAclPersons(
                            oldPerson,
                            false,
                            aclGroupsForPerson,
                            aclPersonsByAclId,
                            removedAcls,
                            dateTimeNow);
                    }
                }

                return;
            }

            var newPersonsIds = new HashSet<Guid>(
                newPersons.Select(
                    person =>
                        person.IdPerson));

            foreach (var oldPerson in oldPersons)
            {
                if (newPersonsIds.Contains(oldPerson.IdPerson))
                    continue;

                RemoveAclPersons(
                    oldPerson,
                    false,
                    new LinkedList<ACLGroup>(
                        GetAclGroupsForPerson(
                            StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                                oldPerson.IdPerson,
                                ObjectType.Person),
                            UserFoldersStructures.Singleton.GetPersonDepartment(
                                oldPerson.IdPerson.ToString()))),
                    GetAclPersonsByAclId(oldPerson),
                    aclGroupModifications.OldAcls,
                    dateTimeNow);
            }

            var oldPersonsIds = new HashSet<Guid>(
                oldPersons.Select(
                    person =>
                        person.IdPerson));

            foreach (var newPerson in newPersons)
            {
                var aclGroupsForPerson = new LinkedList<ACLGroup>(
                    GetAclGroupsForPerson(
                        StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                            newPerson.IdPerson,
                            ObjectType.Person),
                        UserFoldersStructures.Singleton.GetPersonDepartment(
                            newPerson.IdPerson.ToString())));

                var aclPersonsByAclId = GetAclPersonsByAclId(newPerson);

                if (aclGroupModifications.RemoveAllAcls
                    && !oldPersonsIds.Contains(newPerson.IdPerson))
                {
                    RemoveAllAclPersons(
                        aclGroupsForPerson,
                        aclPersonsByAclId.Values);
                }

                AddAndModifyAclPersons(
                    newPerson,
                    false,
                    aclGroupsForPerson,
                    aclPersonsByAclId,
                    aclGroupModifications.NewAcls,
                    dateTimeNow);

                if (removedAcls != null &&
                    oldPersonsIds.Contains(newPerson.IdPerson))
                {
                    RemoveAclPersons(
                        newPerson,
                        false,
                        aclGroupsForPerson,
                        aclPersonsByAclId,
                        removedAcls,
                        dateTimeNow);
                }
            }
        }

        private static Dictionary<Guid, ACLPerson> GetAclPersonsByAclId(Person person)
        {
            var aclIdAclPerson = new Dictionary<Guid, ACLPerson>();

            var aclPersons = ACLPersons.Singleton.GetAclPersonsForPerson(person);

            if (aclPersons == null)
                return aclIdAclPerson;

            foreach (var aclPerson in aclPersons)
            {
                AccessControlList accessControlList = aclPerson.AccessControlList;

                if (accessControlList == null)
                    continue;

                aclIdAclPerson.Add(
                    accessControlList.IdAccessControlList,
                    aclPerson);
            }

            return aclIdAclPerson;
        }

        private static ICollection<Person> FilterPersonsForAclGroupDepartments(
            IEnumerable<UserFoldersStructure> departments,
            bool useDepartmentAclGroupRelation,
            bool isImplicitForPersonWithoutDepartment,
            IEnumerable<Person> persons)
        {
            var personsForAclGroupDepartments = new LinkedList<Person>();

            if (persons == null ||
                (!useDepartmentAclGroupRelation &&
                 !isImplicitForPersonWithoutDepartment))
            {
                return personsForAclGroupDepartments;
            }

            var departmentsIds = departments != null
                ? new HashSet<Guid>(
                    departments.Select(
                        department =>
                            department.IdUserFoldersStructure))
                : new HashSet<Guid>();

            foreach (var person in persons)
            {
                var personDepartment = UserFoldersStructures.Singleton.GetPersonDepartment(person.IdPerson.ToString());
                if (personDepartment == null)
                {
                    if (isImplicitForPersonWithoutDepartment)
                        personsForAclGroupDepartments.AddLast(person);

                    continue;
                }

                if (!useDepartmentAclGroupRelation)
                    continue;

                if (departmentsIds.Contains(personDepartment.IdUserFoldersStructure))
                    personsForAclGroupDepartments.AddLast(person);
            }

            return personsForAclGroupDepartments;
        }

        private void AddAndModifyAclPersons(
            Person person,
            bool personChanged,
            ICollection<ACLGroup> aclGroupsForPerson,
            IDictionary<Guid, ACLPerson> alcIdAclPerson,
            IEnumerable<AccessControlList> addedAcls,
            DateTime dateTimeNow)
        {
            if (addedAcls == null)
                return;

            foreach (var acl in addedAcls)
            {
                bool applyEndValidity;
                byte yearsOfValidity;
                byte monthsOfValidity;
                byte daysOfValidity;

                GetValidityOfAcl(
                    acl,
                    aclGroupsForPerson,
                    out applyEndValidity,
                    out yearsOfValidity,
                    out monthsOfValidity,
                    out daysOfValidity);

                ACLPerson aclPerson;
                if (alcIdAclPerson.TryGetValue(acl.IdAccessControlList, out aclPerson))
                {
                    var dateFrom = personChanged
                        ? dateTimeNow
                        : aclPerson.DateFrom ?? dateTimeNow;

                    var dateTo = GetDateToForAclPerson(
                        dateFrom,
                        applyEndValidity,
                        yearsOfValidity,
                        monthsOfValidity,
                        daysOfValidity,
                        person.EmploymentEndDate);

                    if (dateTo.HasValue != aclPerson.DateTo.HasValue
                        || (dateTo.HasValue && dateTo.Value != aclPerson.DateTo.Value))
                    {
                        aclPerson = ACLPersons.Singleton.GetObjectForEdit(aclPerson.IdACLPerson);
                        if (aclPerson != null)
                        {
                            aclPerson.DateFrom = dateFrom;
                            aclPerson.DateTo = dateTo;

                            ACLPersons.Singleton.Update(aclPerson);
                            ACLPersons.Singleton.EditEnd(aclPerson);
                        }
                    }

                    continue;
                }

                aclPerson = new ACLPerson
                {
                    Person = person,
                    AccessControlList = acl,
                    DateFrom = dateTimeNow,
                    DateTo = GetDateToForAclPerson(
                        dateTimeNow,
                        applyEndValidity,
                        yearsOfValidity,
                        monthsOfValidity,
                        daysOfValidity,
                        person.EmploymentEndDate)
                };

                ACLPersons.Singleton.Insert(ref aclPerson);
            }
        }

        private DateTime? GetDateToForAclPerson(
            DateTime dateFrom,
            bool applyEndValidity,
            byte yearsOfValidity,
            byte monthsOfValidity,
            byte daysOfValidity,
            DateTime? employmentEndDate)
        {
            if (applyEndValidity)
                return employmentEndDate;
            
            var dateToForAclPerson = new DateTime(
                dateFrom.Year,
                dateFrom.Month,
                dateFrom.Day,
                23,
                59,
                59,
                0);

            dateToForAclPerson = dateToForAclPerson
                .AddYears(yearsOfValidity)
                .AddMonths(monthsOfValidity)
                .AddDays(daysOfValidity);

            if (employmentEndDate != null &&
                employmentEndDate.Value < dateToForAclPerson)
            {
                return employmentEndDate;
            }

            return dateToForAclPerson;
        }

        private void RemoveAclPersons(
            Person person,
            bool personChanged,
            ICollection<ACLGroup> aclGroupsForPerson,
            IDictionary<Guid, ACLPerson> alcIdAclPerson,
            IEnumerable<AccessControlList> removedAcls,
            DateTime dateTimeNow)
        {
            if (removedAcls == null)
                return;

            var aclIdsFromAllAclGroupsForPerson = new HashSet<Guid>(
                aclGroupsForPerson
                    .Where(aclGroupForPerson => aclGroupForPerson.AccessControlLists != null)
                    .SelectMany(aclGroupForPerson => aclGroupForPerson.AccessControlLists)
                    .Select(acl => acl.IdAccessControlList));

            foreach (var acl in removedAcls)
            {
                if (aclIdsFromAllAclGroupsForPerson.Contains(acl.IdAccessControlList))
                {
                    ACLPerson aclPerson;
                    if (!alcIdAclPerson.TryGetValue(acl.IdAccessControlList, out aclPerson))
                        continue;

                    bool applyEndValidity;
                    byte yearsOfValidity;
                    byte monthsOfValidity;
                    byte daysOfValidity;

                    GetValidityOfAcl(
                        acl,
                        aclGroupsForPerson,
                        out applyEndValidity,
                        out yearsOfValidity,
                        out monthsOfValidity,
                        out daysOfValidity);

                    var dateFrom = personChanged
                        ? dateTimeNow
                        : aclPerson.DateFrom ?? dateTimeNow;

                    var dateTo = GetDateToForAclPerson(
                        dateFrom,
                        applyEndValidity,
                        yearsOfValidity,
                        monthsOfValidity,
                        daysOfValidity,
                        person.EmploymentEndDate);

                    if (dateTo.HasValue != aclPerson.DateTo.HasValue
                        || (dateTo.HasValue && dateTo.Value != aclPerson.DateTo.Value))
                    {
                        aclPerson = ACLPersons.Singleton.GetObjectForEdit(aclPerson.IdACLPerson);
                        if (aclPerson != null)
                        {
                            aclPerson.DateFrom = dateFrom;
                            aclPerson.DateTo = dateTo;

                            ACLPersons.Singleton.Update(aclPerson);
                            ACLPersons.Singleton.EditEnd(aclPerson);
                        }
                    }

                    continue;
                }

                var aclId = acl.IdAccessControlList;

                ACLPersons.Singleton.DeleteByCriteria(
                    aclPerson =>
                        aclPerson.Person != null &&
                        aclPerson.Person.IdPerson == person.IdPerson &&
                        aclPerson.AccessControlList != null &&
                        aclPerson.AccessControlList.IdAccessControlList == aclId,
                    null);
            }
        }

        private void RemoveAllAclPersons(
            IEnumerable<ACLGroup> aclGroupsForPerson,
            IEnumerable<ACLPerson> aclPersons)
        {
            var aclIdsFromAllAclGroupsForPerson = new HashSet<Guid>(
                aclGroupsForPerson
                    .Where(aclGruop => aclGruop.AccessControlLists != null)
                    .SelectMany(aclGruop => aclGruop.AccessControlLists)
                    .Select(acl => acl.IdAccessControlList));

            foreach (var aclPerson in aclPersons)
            {
                if (aclPerson.AccessControlList == null
                    || aclIdsFromAllAclGroupsForPerson.Contains(
                        aclPerson.AccessControlList.IdAccessControlList))
                {
                    continue;
                }

                ACLPersons.Singleton.DeleteById(aclPerson.IdACLPerson);
            }
        }

        private static void GetValidityOfAcl(
            AccessControlList acl,
            IEnumerable<ACLGroup> aclGroupsForPerson,
            out bool appplyEndlessValidity,
            out byte yearsOfValidity,
            out byte monthsOfValidity,
            out byte daysOfValidity)
        {
            appplyEndlessValidity = true;
            yearsOfValidity = 0;
            monthsOfValidity = 0;
            daysOfValidity = 0;

            foreach (var aclGroup in aclGroupsForPerson)
            {
                if (aclGroup.AccessControlLists == null ||
                    aclGroup.AccessControlLists.All(
                        aclGroupAcl =>
                            aclGroupAcl.IdAccessControlList != acl.IdAccessControlList))
                {
                    continue;
                }

                ModifyValidity(
                    aclGroup,
                    ref appplyEndlessValidity,
                    ref yearsOfValidity,
                    ref monthsOfValidity,
                    ref daysOfValidity);
            }
        }

        private static void ModifyValidity(
            ACLGroup aclGroup,
            ref bool applyEndlessValidity,
            ref byte yearsOfValidity,
            ref byte monthsOfValidity,
            ref byte daysOfValidity)
        {
            if (aclGroup.ApplyEndlessValidity)
                return;

            if (applyEndlessValidity)
            {
                applyEndlessValidity = false;
                yearsOfValidity = aclGroup.YearsOfValidity;
                monthsOfValidity = aclGroup.MonthsOfValidity;
                daysOfValidity = aclGroup.DaysOfValidity;

                return;
            }

            if (yearsOfValidity < aclGroup.YearsOfValidity)
            {
                return;
            }

            if (yearsOfValidity > aclGroup.YearsOfValidity)
            {
                yearsOfValidity = aclGroup.YearsOfValidity;
                monthsOfValidity = aclGroup.MonthsOfValidity;
                daysOfValidity = aclGroup.DaysOfValidity;

                return;
            }

            if (monthsOfValidity < aclGroup.MonthsOfValidity)
                return;

            if (monthsOfValidity > aclGroup.MonthsOfValidity)
            {
                monthsOfValidity = aclGroup.MonthsOfValidity;
                daysOfValidity = aclGroup.DaysOfValidity;
                return;
            }

            if (daysOfValidity < aclGroup.DaysOfValidity)
                return;

            daysOfValidity = aclGroup.DaysOfValidity;
        }

        public void AddEventOnObjectMoved()
        {
            SafeThread.StartThread(AddEventOnObjectMovedInternal);
        }

        private void AddEventOnObjectMovedInternal()
        {
            StructuredSubSites.Singleton.GlobalEvaluator.AddEventOnObjectMoved(OnObjectMoved);
            StructuredSubSites.Singleton.GlobalEvaluator.AddEventOnObjectAdded(OnObjectAdded);
        }

        private void DoObjectMoved(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            GlobalEvaluator.GlobalSiteInfo oldGlobalSiteInfo)
        {
            if (globalObjectInfo == null
                || oldGlobalSiteInfo == null)
            {
                return;
            }

            switch (globalObjectInfo.ObjectType)
            {
                case ObjectType.ACLGroup:

                    var aclGroup = GetById(globalObjectInfo.Id);
                    if (aclGroup == null)
                        return;

                    ApplyChangesForAclGroup(
                        new AclGroupModifications(
                            aclGroup,
                            globalObjectInfo.SiteInfo,
                            oldGlobalSiteInfo));

                    break;

                case ObjectType.Person:

                    PersonChanged(
                        (Guid)globalObjectInfo.Id,
                        null,
                        globalObjectInfo.SiteInfo,
                        oldGlobalSiteInfo);

                    break;
            }
        }

        private void OnObjectMoved(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo,
            GlobalEvaluator.GlobalSiteInfo oldGlobalSiteInfo)
        {
            _onAclGroupOrPersonChanged.Enqueue(
                () => DoObjectMoved(globalObjectInfo, oldGlobalSiteInfo));
        }

        private void DoObjectAdded(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
        {
            if (globalObjectInfo == null)
                return;

            switch (globalObjectInfo.ObjectType)
            {
                case ObjectType.ACLGroup:

                    var aclGroup = GetById(globalObjectInfo.Id);
                    if (aclGroup == null)
                        return;

                    ApplyChangesForAclGroup(
                        new AclGroupModifications(
                            aclGroup,
                            globalObjectInfo.SiteInfo,
                            globalObjectInfo.SiteInfo));

                    break;

                case ObjectType.Person:

                    PersonChanged(
                        (Guid) globalObjectInfo.Id,
                        null,
                        globalObjectInfo.SiteInfo,
                        globalObjectInfo.SiteInfo);

                    break;
            }
        }

        private void OnObjectAdded(
            GlobalEvaluator.GlobalObjectInfo globalObjectInfo)
        {
            _onAclGroupOrPersonChanged.Enqueue(
                () => DoObjectAdded(globalObjectInfo));
        }

        protected override bool AddCriteriaSpecial(ref ICriteria criteria, FilterSettings filterSetting)
        {
            if (filterSetting.Column == ACLGroup.COLUMN_DEPARTMENTS)
            {
                criteria.CreateAlias(ACLGroup.COLUMN_DEPARTMENTS, "AclGroupDepartment")
                    .Add(
                        Restrictions.Eq(
                            string.Format("AclGroupDepartment.{0}",
                                UserFoldersStructure.COLUMN_ID_USER_FOLDERS_STRUCTURE),
                            ((UserFoldersStructure)filterSetting.Value).IdUserFoldersStructure));

                return true;
            }

            return false;
        }

        public void PersonDepartmentChanged(
            Guid personId, 
            PersonDepartmentChange personDepartmentChange)
        {
            _onAclGroupOrPersonChanged.Enqueue(() =>
            {
                var personGlobalSiteInfo = StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                    personId,
                    ObjectType.Person);

                PersonChanged(
                    personId,
                    personDepartmentChange,
                    personGlobalSiteInfo,
                    personGlobalSiteInfo);
            });
        }

        public void PersonChanged(
            Guid personId,
            PersonDepartmentChange personDepartmentChange,
            GlobalEvaluator.GlobalSiteInfo newGlobalSiteInfo,
            GlobalEvaluator.GlobalSiteInfo oldGlobalSiteInfo)
        {
            var person = Persons.Singleton.GetById(personId);

            if (person == null)
                return;

            var oldPersonDepartment = personDepartmentChange != null
                ? personDepartmentChange.OldPersonDepartment
                : UserFoldersStructures.Singleton.GetPersonDepartment(personId.ToString());

            var oldAclGroups = new HashSet<ACLGroup>(
                GetAclGroupsForPerson(
                    oldGlobalSiteInfo,
                    oldPersonDepartment));

            var newPersonDepartment = personDepartmentChange != null
                ? personDepartmentChange.NewPersonDepartment
                : oldPersonDepartment;

            var newAclGroups = new HashSet<ACLGroup>(
                GetAclGroupsForPerson(
                    newGlobalSiteInfo,
                    newPersonDepartment));

            var aclPersonsByAclId = GetAclPersonsByAclId(person);
            
            var removeAllAcls = false;
            var addedAclGroups = new LinkedList<ACLGroup>();

            foreach (var newAclGroup in newAclGroups)
            {
                if (oldAclGroups.Contains(newAclGroup))
                    continue;

                if (newAclGroup.RemoveAllAcls)
                    removeAllAcls = true;

                addedAclGroups.AddLast(newAclGroup);
            }

            var dateTimeNow = DateTime.Now;

            if (removeAllAcls)
                RemoveAllAclPersons(
                    newAclGroups,
                    aclPersonsByAclId.Values);
            else
                foreach (var oldAclGroup in oldAclGroups)
                {
                    if (newAclGroups.Contains(oldAclGroup))
                        continue;

                    if (personDepartmentChange == null
                        || (personDepartmentChange.NewPersonDepartment == null
                            && oldAclGroup.RemoveWhenDepartmentRemove)
                        || (personDepartmentChange.NewPersonDepartment != null
                            && oldAclGroup.RemoveWhenDepartmentChange))
                    {
                        RemoveAclPersons(
                            person,
                            true,
                            newAclGroups,
                            aclPersonsByAclId,
                            oldAclGroup.AccessControlLists,
                            dateTimeNow);
                    }
                }

            AddAndModifyAclPersons(
                person,
                true,
                newAclGroups,
                aclPersonsByAclId,
                new HashSet<AccessControlList>(
                    addedAclGroups
                        .Where(aclGroup => aclGroup.AccessControlLists != null)
                        .SelectMany(aclGroup => aclGroup.AccessControlLists),
                    new AclByIdEqualityComparer()),
                dateTimeNow);
        }

        public void PersonEmploymentEndDateChanged(Guid personId)
        {
            var person = Persons.Singleton.GetById(personId);

            if (person == null)
                return;

            var aclGroups = new HashSet<ACLGroup>(
                GetAclGroupsForPerson(
                    StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                        personId,
                        ObjectType.Person),
                    UserFoldersStructures.Singleton.GetPersonDepartment(
                        personId.ToString())));

            var aclPersonsByAclId = GetAclPersonsByAclId(person);

            AddAndModifyAclPersons(
                person,
                false,
                aclGroups,
                aclPersonsByAclId,
                new HashSet<AccessControlList>(
                    aclGroups
                        .Where(aclGroup => aclGroup.AccessControlLists != null)
                        .SelectMany(aclGroup => aclGroup.AccessControlLists),
                    new AclByIdEqualityComparer()),
                DateTime.Now);
        }

        private class AclByIdEqualityComparer : IEqualityComparer<AccessControlList>
        {
            public bool Equals(
                AccessControlList firstAcl,
                AccessControlList secondAcl)
            {
                return firstAcl.IdAccessControlList.Equals(secondAcl.IdAccessControlList);
            }

            public int GetHashCode(AccessControlList acl)
            {
                return acl.IdAccessControlList.GetHashCode();
            }
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.ACLGroup; }
        }
    }
}
