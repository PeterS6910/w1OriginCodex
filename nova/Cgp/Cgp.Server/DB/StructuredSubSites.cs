using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.ORM;
using Contal.Cgp.Server.StructuredSiteEvaluator;

namespace Contal.Cgp.Server.DB
{
    public sealed class StructuredSubSites :
        ABaseOrmTable<StructuredSubSites, StructuredSubSite>,
        IStructuredSubSites
    {
        private readonly string _mainDatabaseName;

        private volatile GlobalEvaluator _globalEvaluator;
        private readonly object _globalEvaluatorSync = new object();

        public GlobalEvaluator GlobalEvaluator
        {
            get
            {
                if (_globalEvaluator == null)
                    lock (_globalEvaluatorSync)
                        if (_globalEvaluator == null)
                            Monitor.Wait(_globalEvaluatorSync);

                return _globalEvaluator;
            }
        }

        private StructuredSubSites() : base(null)
        {
            var connectionString =
                ConnectionString.LoadFromRegistry(
                    CgpServerGlobals.REGISTRY_CONNECTION_STRING);

            if (connectionString != null &&
                connectionString.IsValid())
            {
                _mainDatabaseName = connectionString.DatabaseName;
            }
        }

        public override object ParseId(string strObjectId)
        {
            int result;

            return int.TryParse(strObjectId, out result)
                ? (object)result
                : null;
        }

        public void CreateGlobalEvaluator()
        {
            if (_globalEvaluator != null)
                return;

            lock (_globalEvaluatorSync)
            {
                if (_globalEvaluator != null)
                    return;

                _globalEvaluator = new GlobalEvaluator();

                Monitor.PulseAll(_globalEvaluatorSync);
            }
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.STRUCTURED_SUB_SITE), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.StructuredSubSiteAdmin), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.StructuredSubSiteAdmin), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.StructuredSubSiteAdmin), login);
        }

        public bool InsertStructuredSubSiteObject(StructuredSubSiteObject structuredSubSiteObject)
        {
            return StructuredSubSiteObjects.Singleton.Insert(ref structuredSubSiteObject);
        }

        public ICollection<StructuredSubSite> GetAllSubSitesForLogin(out bool isInRootSite)
        {
            isInRootSite = false;

            var login = AccessChecker.GetActualLogin();
            if (login == null)
                return null;

            var subSitesFromDatabase = GetAllSubSitesForLogin(login, out isInRootSite);
            if (subSitesFromDatabase == null)
                return null;

            var subSites = new LinkedList<StructuredSubSite>();
            foreach (var subSiteFromDatabase in subSitesFromDatabase)
            {
                subSites.AddLast(GetById(subSiteFromDatabase.IdStructuredSubSite));
            }

            return subSites;
        }

        public ICollection<ObjectPlacement> GetObjectPlacements(ObjectType objectType, string objectIdString,
            string separator, string rootName, bool onlyInSite)
        {
            var login = AccessChecker.GetActualLogin();
            if (login == null)
                return null;

            return GetObjectPlacements(
                objectType,
                objectIdString,
                separator,
                rootName,
                new HashSet<int>(GetTopMostSites(login).Select(siteInfo => siteInfo.Id)),
                onlyInSite);
        }

        public ICollection<ObjectPlacement> GetObjectPlacements(ObjectType objectType, string objectIdString,
            string separator, string rootName, HashSet<int> loginTopMostSitesId, bool onlyInSite)
        {
            var objectPlacments = GetObjectSiteReferences(
                objectType,
                objectIdString,
                loginTopMostSitesId,
                false,
                separator,
                rootName);

            if (onlyInSite)
                return new LinkedList<ObjectPlacement>(
                    objectPlacments.OrderBy(siteReference => siteReference.Name));

            var referencesPlacments = GetObjectSiteReferences(
                objectType,
                objectIdString,
                loginTopMostSitesId,
                true,
                separator,
                rootName);

            var userFoldersPlacments = GetObjectUserFolderSiteReferences(
                objectType,
                objectIdString,
                loginTopMostSitesId,
                false,
                separator,
                rootName);

            return new LinkedList<ObjectPlacement>(
                objectPlacments.OrderBy(siteReference => siteReference.Name)
                    .Concat(referencesPlacments.OrderBy(siteReference => siteReference.Name))
                    .Concat(userFoldersPlacments.OrderBy(siteReference => siteReference.Name)));
        }

        private IEnumerable<ObjectPlacement> GetObjectSiteReferences(ObjectType objectType, string objectIdString,
            HashSet<int> loginTopMostSitesId, bool references, string separator, string rootName)
        {
            var objectSiteReferences = new LinkedList<ObjectPlacement>();

            bool referencesForDatabase;
            if (objectType == ObjectType.Login ||
                objectType == ObjectType.LoginGroup)
            {
                if (references)
                    return objectSiteReferences;
                else
                    referencesForDatabase = true;
            }
            else
            {
                referencesForDatabase = references;
            }

            var objectsFromDatabase =
                SelectLinq<StructuredSubSiteObject>(
                    structuredSubSiteObject =>
                        structuredSubSiteObject.ObjectType == objectType &&
                        structuredSubSiteObject.ObjectId == objectIdString &&
                        structuredSubSiteObject.IsReference == referencesForDatabase);

            if (objectsFromDatabase == null)
                return objectSiteReferences;

            if (!referencesForDatabase &&
                objectsFromDatabase.Count == 0)
            {
                if (objectType == ObjectType.UserFoldersStructure)
                {
                    var userFolder = UserFoldersStructures.Singleton.GetById(UserFoldersStructures.Singleton.ParseId(objectIdString));
                    if (userFolder != null && userFolder.ParentFolder != null)
                    {
                        return Enumerable.Repeat(
                            new ObjectPlacement(
                                -1,
                                string.Format(
                                    "{0}{1}{2}",
                                    rootName,
                                    separator,
                                    UserFoldersStructures.Singleton.GetFullPath(userFolder.ParentFolder)),
                                references,
                                new IdAndObjectType(
                                    userFolder.ParentFolder.GetId(),
                                    userFolder.ParentFolder.GetObjectType())),
                            1);
                    }
                }

                return Enumerable.Repeat(
                    new ObjectPlacement(
                        -1,
                        rootName,
                        references,
                        null),
                    1);
            }

            foreach (var objectFromDatabase in objectsFromDatabase)
            {
                var objectSite = objectFromDatabase.StructuredSubSite;
                var objectSitename = new StringBuilder();
                var siteId = objectSite != null ? objectSite.IdStructuredSubSite : -1;

                while (true)
                {
                    if (objectSitename.Length > 0)
                        objectSitename.Insert(0, separator);

                    if (objectSite != null)
                    {
                        objectSitename.Insert(0, objectSite.Name);
                    }
                    else
                    {
                        objectSitename.Insert(0, rootName);
                    }

                    if (loginTopMostSitesId.Contains(objectSite != null ? objectSite.IdStructuredSubSite : -1))
                    {
                        if (objectType == ObjectType.UserFoldersStructure)
                        {
                            var userFolder = UserFoldersStructures.Singleton.GetById(UserFoldersStructures.Singleton.ParseId(objectIdString));
                            if (userFolder != null && userFolder.ParentFolder != null)
                            {
                                objectSiteReferences.AddLast(
                                    new ObjectPlacement(
                                        siteId,
                                        string.Format(
                                            "{0}{1}{2}",
                                            objectSitename.ToString(),
                                            separator,
                                            UserFoldersStructures.Singleton.GetFullPath(userFolder.ParentFolder)),
                                        references,
                                        new IdAndObjectType(
                                            userFolder.ParentFolder.GetId(),
                                            userFolder.ParentFolder.GetObjectType())));

                                break;
                            }
                        }

                        objectSiteReferences.AddLast(
                            new ObjectPlacement(
                                siteId,
                                objectSitename.ToString(),
                                references,
                                null));

                        break;
                    }

                    if (objectSite == null)
                        break;

                    objectSite = objectSite.ParentSite;
                }
            }

            return objectSiteReferences;
        }

        private IEnumerable<ObjectPlacement> GetObjectUserFolderSiteReferences(ObjectType objectType, string objectIdString,
            HashSet<int> loginTopMostSitesId, bool references, string separator, string rootName)
        {
            var userFoldersPlacments = new LinkedList<ObjectPlacement>();
            var userFolders = UserFoldersStructures.Singleton.GetUserFoldersForObject(objectIdString, objectType);

            if (userFolders == null)
                return userFoldersPlacments;

            foreach (var userFolder in userFolders)
            {
                var userFolderPlacments = GetObjectSiteReferences(
                    userFolder.GetObjectType(),
                    userFolder.GetIdString(),
                    loginTopMostSitesId,
                    false,
                    separator,
                    rootName);

                foreach (var userFolderPlacment in userFolderPlacments)
                {
                    userFoldersPlacments.AddLast(new ObjectPlacement(
                        userFolderPlacment.SiteId,
                        string.Format(
                            "{0}{1}{2}",
                            userFolderPlacment.Name,
                            separator,
                            UserFoldersStructures.Singleton.GetFullPath(userFolder)),
                        userFolderPlacment.IsReference,
                        new IdAndObjectType(userFolder.GetId(), userFolder.GetObjectType())));
                }
            }

            return userFoldersPlacments;
        }

        public IStructuredSiteBuilder GetBuilder(IStructuredSiteBuilderClient structuredSiteBuilderClient)
        {
            if (_globalEvaluator == null)
                lock (_globalEvaluatorSync)
                    if (_globalEvaluator == null)
                        Monitor.Wait(_globalEvaluatorSync);

            if (_globalEvaluator == null)
                return null;

            return
                _globalEvaluator.LocalEvaluatorManager.GetLocalEvaluator(
                    structuredSiteBuilderClient,
                    AccessChecker.GetActualLogin()).Builder;
        }

        public IEnumerable<StructuredSubSite> GetAllSubSitesForLogin(
            Login login,
            out bool isInRootSite)
        {
            isInRootSite = false;

            if (login == null)
                return null;

            if (IsLoginInRootSite(login))
            {
                isInRootSite = true;
                return List();
            }

            var objectsFromDatabase =
                SelectLinq<StructuredSubSiteObject>(
                    structuredSubSiteObject =>
                        structuredSubSiteObject.ObjectType == ObjectType.Login &&
                        structuredSubSiteObject.ObjectId == login.GetIdString());

            var subSitesForLogin = new LinkedList<StructuredSubSite>();
            var subSitesForLoginIds = new HashSet<int>();
            if (objectsFromDatabase != null)
            {
                foreach (var objectFromDatabase in objectsFromDatabase)
                {
                    var structuredSubSite = objectFromDatabase.StructuredSubSite;

                    if (subSitesForLoginIds.Contains(structuredSubSite.IdStructuredSubSite))
                        continue;

                    subSitesForLogin.AddLast(structuredSubSite);
                    subSitesForLoginIds.Add(structuredSubSite.IdStructuredSubSite);

                    AddChildSubSites(structuredSubSite, subSitesForLogin, subSitesForLoginIds);
                }
            }

            return subSitesForLogin;
        }

        private bool IsLoginInRootSite(Login login)
        {
            if (login == null)
                return false;

            var objectsFromDatabase =
                SelectLinq<StructuredSubSiteObject>(
                    structuredSubSiteObject =>
                        structuredSubSiteObject.StructuredSubSite == null &&
                        structuredSubSiteObject.ObjectType == ObjectType.Login &&
                        structuredSubSiteObject.ObjectId == login.GetIdString());

            return objectsFromDatabase != null && objectsFromDatabase.Count > 0;
        }

        private void AddChildSubSites(
            StructuredSubSite subSite,
            ICollection<StructuredSubSite> childSubSites,
            HashSet<int> subSitesForLoginIds)
        {
            var childSubSitesFromDatabse =
                SelectLinq<StructuredSubSite>(childsubSite => childsubSite.ParentSite == subSite);

            if (childSubSitesFromDatabse == null)
                return;

            foreach (var childSubSiteFromDatabse in childSubSitesFromDatabse)
            {
                if (subSitesForLoginIds.Contains(childSubSiteFromDatabse.IdStructuredSubSite))
                    continue;

                subSitesForLoginIds.Add(childSubSiteFromDatabse.IdStructuredSubSite);
                childSubSites.Add(childSubSiteFromDatabse);

                AddChildSubSites(
                    childSubSiteFromDatabse,
                    childSubSites,
                    subSitesForLoginIds);
            }
        }

        public ICollection<T> GetObjectsForLogin<T>(
            Login login,
            ICollection<T> allObjectsFromDatabase)
            where T : AOrmObject
        {
            if (allObjectsFromDatabase == null ||
                allObjectsFromDatabase.Count == 0 ||
                !GlobalEvaluator.IsObjectTypeRelevantForStructuredSites(
                    allObjectsFromDatabase.First().GetObjectType()))
            {
                return allObjectsFromDatabase;
            }

            var topMostSites = GetTopMostSites(login);

            return
                new LinkedList<T>(
                    topMostSites.Contains(_globalEvaluator.Root)
                        ? allObjectsFromDatabase
                        : allObjectsFromDatabase
                            .Where(
                                objectFromDatabase =>
                                    IsObjectVisibleForLogin(
                                        new IdAndObjectType(
                                            objectFromDatabase.GetId(),
                                            objectFromDatabase.GetObjectType()),
                                        topMostSites,
                                        true)));
        }

        public ICollection<ServerAlarmCore> GetAlarmsForLogin(
            ICollection<ServerAlarmCore> allAlarms)
        {
            if (allAlarms == null || allAlarms.Count == 0)
                return allAlarms;

            var login = AccessChecker.GetActualLogin();
            if (login == null)
                return null;

            var topMostSites = GetTopMostSites(login);

            return
                new LinkedList<ServerAlarmCore>(
                    topMostSites.Contains(_globalEvaluator.Root)
                        ? allAlarms
                        : allAlarms.Where(
                            alarm => _globalEvaluator.AreObjectsVisibleForLogin(
                                alarm.RelatedObjects,
                                topMostSites,
                                false)));
        }

        public bool IsAlarmVisibleForSites(
            IEnumerable<IdAndObjectType> parentObjects,
            ICollection<int> subSitesFilter)
        {
            if (subSitesFilter == null
                || parentObjects == null)
            {
                return true;
            }

            return parentObjects.Any(
                alarmParentObject =>
                {
                    var globalSiteInfo = GlobalEvaluator.GetGlobalSiteInfoOfObject(
                        alarmParentObject.Id,
                        alarmParentObject.ObjectType);

                    return globalSiteInfo != null
                           && subSitesFilter.Contains(globalSiteInfo.Id);
                });
        }

        public string GetEventlogSubSiteConditionForLogin(
            Login login,
            string eventlogAlias,
            string eventSourceAlias,
            ICollection<int> subSitesFilter,
            bool orSubsites)
        {
            if (login == null)
                login = AccessChecker.GetActualLogin();
            if (login == null)
                return null;

            bool isInRootSite;

            var subSitesForLogin =
                GetAllSubSitesForLogin(
                    login,
                    out isInRootSite);

            if (isInRootSite
                && subSitesFilter == null)
            {
                return string.Empty;
            }

            var condition = GetEventlogConditionForLogin(
                subSitesForLogin,
                isInRootSite,
                subSitesFilter,
                eventlogAlias,
                eventSourceAlias,
                orSubsites);

            if (isInRootSite
                || Logins.Singleton.IsLoginSuperAdmin(login))
            {
                string strCnrEventSourcesGuids = null;
                var cnrEventSourcesGuids = CentralNameRegisters.Singleton.CnrEventSourcesGuids;
                if (cnrEventSourcesGuids != null)
                {
                    foreach (var cnrEventSourceGuid in cnrEventSourcesGuids)
                    {
                        if (strCnrEventSourcesGuids == null)
                        {
                            strCnrEventSourcesGuids = string.Format(
                                "'{0}'",
                                cnrEventSourceGuid);

                            continue;
                        }

                        strCnrEventSourcesGuids = string.Format(
                            "{0},'{1}'",
                            strCnrEventSourcesGuids,
                            cnrEventSourceGuid);
                    }
                }

                if (condition.Length > 0)
                    condition.Append(" OR ");

                condition.Append(
                    string.IsNullOrEmpty(strCnrEventSourcesGuids)
                        ? string.Format(
                            "(Select count({0}) from {1} as {2} WHERE {3}.{4} = {5}.{6}) = 0",
                            EventSource.COLUMN_EVENTLOG_ID,
                            EventSources.EVENTSOURCE_TABLE_NAME,
                            eventSourceAlias,
                            eventlogAlias,
                            Eventlog.COLUMN_ID_EVENTLOG,
                            eventSourceAlias,
                            EventSource.COLUMN_EVENTLOG_ID)
                        : string.Format(
                            "(Select count({0}) from {1} as {2} WHERE {3}.{4} = {5}.{6} AND {7}.{8} not in ({9})) = 0",
                            EventSource.COLUMN_EVENTLOG_ID,
                            EventSources.EVENTSOURCE_TABLE_NAME,
                            eventSourceAlias,
                            eventlogAlias,
                            Eventlog.COLUMN_ID_EVENTLOG,
                            eventSourceAlias,
                            EventSource.COLUMN_EVENTLOG_ID,
                            eventSourceAlias,
                            EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                            strCnrEventSourcesGuids));
            }

            return condition.Length > 0
                ? condition.ToString()
                : null;
        }

        private StringBuilder GetEventlogConditionForLogin(
            IEnumerable<StructuredSubSite> subSitesForLogin,
            bool isInRootSite,
            ICollection<int> subSitesFilter,
            string eventlogAlias,
            string eventSourceAlias,
            bool orSubsites)
        {
            var condition = new StringBuilder();

            if (subSitesFilter != null
                && subSitesFilter.Count == 0)
            {
                if (orSubsites)
                {
                    condition.Append(string.Format("(Select count({0}) from {1} as {2} WHERE {3}.{4} = {5}.{6}) > 0",
                        EventSource.COLUMN_EVENTLOG_ID,
                        EventSources.EVENTSOURCE_TABLE_NAME,
                        eventSourceAlias,
                        eventlogAlias,
                        Eventlog.COLUMN_ID_EVENTLOG,
                        eventSourceAlias,
                        EventSource.COLUMN_EVENTLOG_ID));
                }

                return condition;
            }

            if (isInRootSite)
            {
                if (subSitesFilter != null
                    && subSitesFilter.Contains(-1))
                {
                    condition.Append(string.Format(
                        "(Select count({0}) from {1} as {2} WHERE {3}.{4} = {5}.{6} AND convert(varchar(38), {7}.{8}) in (Select {9} from [{10}].[dbo].[{11}])) = 0",
                        EventSource.COLUMN_EVENTLOG_ID,
                        EventSources.EVENTSOURCE_TABLE_NAME,
                        eventSourceAlias,
                        eventlogAlias,
                        Eventlog.COLUMN_ID_EVENTLOG,
                        eventSourceAlias,
                        EventSource.COLUMN_EVENTLOG_ID,
                        eventSourceAlias,
                        EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                        StructuredSubSiteObject.COLUMN_OBJECT_ID,
                        _mainDatabaseName,
                        StructuredSubSiteObjects.STRUCTURED_SUB_SITE_OBJECT_TABLE_NAME));
                }
            }

            if (subSitesForLogin == null)
            {
                return condition;
            }

            var strSubSitesForLogin = new StringBuilder();

            foreach (var subSiteForLogin in subSitesForLogin)
            {
                if (subSitesFilter != null
                    && !subSitesFilter.Contains(subSiteForLogin.IdStructuredSubSite))
                {
                    continue;
                }

                strSubSitesForLogin.Append(
                    strSubSitesForLogin.Length > 0
                        ? ","
                        : "(");

                strSubSitesForLogin.Append(subSiteForLogin.IdStructuredSubSite);
            }

            if (strSubSitesForLogin.Length == 0)
                return condition;

            strSubSitesForLogin.Append(")");

            if (condition.Length > 0)
                condition.Append(" OR ");

            condition.Append(string.Format(
                "(Select count({0}) from {1} as {2} WHERE {3}.{4} = {5}.{6} AND convert(varchar(38), {7}.{8}) in (Select {9} from [{10}].[dbo].[{11}] WHERE {12} in {13})) > 0",
                EventSource.COLUMN_EVENTLOG_ID,
                EventSources.EVENTSOURCE_TABLE_NAME,
                eventSourceAlias,
                eventlogAlias,
                Eventlog.COLUMN_ID_EVENTLOG,
                eventSourceAlias,
                EventSource.COLUMN_EVENTLOG_ID,
                eventSourceAlias,
                EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                StructuredSubSiteObject.COLUMN_OBJECT_ID,
                _mainDatabaseName,
                StructuredSubSiteObjects.STRUCTURED_SUB_SITE_OBJECT_TABLE_NAME,
                StructuredSubSiteObject.COLUMN_STRUCTURED_SUB_SITE,
                strSubSitesForLogin));

            return condition;
        }

        private bool HasAccess(
            AOrmObject ormObject,
            Login login,
            bool referencesAllowed)
        {
            if (login == null ||
                ormObject == null)
            {
                return false;
            }

            var objectType = ormObject.GetObjectType();

            if (!GlobalEvaluator.IsObjectTypeRelevantForStructuredSites(
                objectType))
            {
                return true;
            }

            var topMostSites = GetTopMostSites(login);

            return
                topMostSites.Contains(_globalEvaluator.Root) ||
                IsObjectVisibleForLogin(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()),
                    topMostSites,
                    referencesAllowed);
        }

        public ICollection<GlobalEvaluator.GlobalSiteInfo> GetTopMostSites(Login login)
        {
            return _globalEvaluator.GetTopMostSites(login);
        }

        private bool IsObjectVisibleForLogin(
            IdAndObjectType idAndObjectType,
            ICollection<GlobalEvaluator.GlobalSiteInfo> topMostSites,
            bool referencesAllowed)
        {
            return _globalEvaluator.AreObjectsVisibleForLogin(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        idAndObjectType.Id,
                        idAndObjectType.ObjectType),
                    1),
                topMostSites,
                referencesAllowed);
        }

        public bool IsObjectVisibleForLogin(
            IdAndObjectType idAndObjectType,
            ICollection<GlobalEvaluator.GlobalSiteInfo> topMostSites)
        {
            return IsObjectVisibleForLogin(
                idAndObjectType,
                topMostSites,
                ReferencesAllowed(idAndObjectType.ObjectType));
        }

        public bool HasAccessView(AOrmObject ormObject, Login login)
        {
            if (ormObject == null)
                return false;

            var objectType = ormObject.GetObjectType();

            return HasAccess(
                ormObject,
                login,
                ReferencesAllowed(objectType));
        }

        private bool ReferencesAllowed(ObjectType objectType)
        {
            return objectType == ObjectType.DailyPlan
                   || objectType == ObjectType.TimeZone
                   || objectType == ObjectType.SecurityDailyPlan
                   || objectType == ObjectType.SecurityTimeZone
                   || objectType == ObjectType.Calendar
                   || objectType == ObjectType.DayType
                   || objectType == ObjectType.CardSystem
                   || objectType == ObjectType.Card
                   || objectType == ObjectType.DoorEnvironment;
        }

        public bool HasAccessUpdate(AOrmObject ormObject)
        {
            return HasAccess(ormObject, AccessChecker.GetActualLogin(), false);
        }

        public bool HasAccessDelete(AOrmObject ormObject)
        {
            return HasAccess(ormObject, AccessChecker.GetActualLogin(), false);
        }

        public void AddObjectToSite(object objectId, ObjectType objectType, int idStructuredSubSite)
        {
            if (objectId == null)
                return;

            var structuredSubSite = GetById(idStructuredSubSite);

            AddObjectToSite(objectId, objectType, structuredSubSite);
        }

        public void AddObjectToSite(AOrmObject ormObject, StructuredSubSite structuredSubSite)
        {
            AddObjectToSite(
                ormObject.GetId(),
                ormObject.GetObjectType(),
                structuredSubSite);
        }

        private static void AddObjectToSite(
            object objectId,
            ObjectType objectType,
            StructuredSubSite structuredSubSite)
        {
            if (structuredSubSite == null)
                return;

            var structuredSubSiteObject = new StructuredSubSiteObject
            {
                ObjectId = objectId.ToString(),
                ObjectType = objectType,
                StructuredSubSite = structuredSubSite,
                IsReference = false
            };

            StructuredSubSiteObjects.Singleton.Insert(ref structuredSubSiteObject);
        }

        public void MoveObjectToSite(
            object objectId,
            ObjectType objectType,
            int siteId)
        {
            var objectIdString = objectId.ToString();
            var structuredSubSite = GetById(siteId);

            if (structuredSubSite == null)
                return;

            var structuredSubSiteObject =
                StructuredSubSiteObjects.Singleton.FindStructuredSubSiteObject(
                    objectType,
                    objectIdString);

            if (structuredSubSiteObject == null)
                return;

            structuredSubSiteObject =
                StructuredSubSiteObjects.Singleton.GetObjectForEdit(
                    structuredSubSiteObject.IdStructuredSubSiteObject);
            try
            {
                structuredSubSiteObject.StructuredSubSite = structuredSubSite;

                StructuredSubSiteObjects.Singleton.Update(structuredSubSiteObject);
            }
            finally
            {
                StructuredSubSiteObjects.Singleton.EditEnd(structuredSubSiteObject);
            }
        }

        public static void MoveObjectToRoot(object objectId, ObjectType objectType)
        {
            var objectIdString = objectId.ToString();

            StructuredSubSiteObjects.Singleton.DeleteByCriteria(
                structuredSubSiteObject =>
                    structuredSubSiteObject.ObjectType == objectType &&
                    structuredSubSiteObject.ObjectId == objectIdString &&
                    structuredSubSiteObject.IsReference == false,
                null);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.StructuredSubSite; }
        }
    }
}
