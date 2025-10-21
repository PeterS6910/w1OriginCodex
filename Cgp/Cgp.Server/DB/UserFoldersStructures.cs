using Contal.Cgp.Globals;
using Contal.Cgp.ORM;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.StructuredSiteEvaluator;
using NHibernate.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contal.Cgp.Server.DB
{
    public sealed class UserFoldersStructures :
        ABaseOrmTable<UserFoldersStructures, UserFoldersStructure>, 
        IUserFoldersSutructures
    {
        private UserFoldersStructures() : base(null)
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.FOLDERS_SRUCTURE), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.FoldersStructureAdmin), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.FoldersStructureAdmin), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.FoldersStructureAdmin), login);
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var userFolderStructure = ormObject as UserFoldersStructure;

            return userFolderStructure != null
                ? userFolderStructure.ParentFolder
                : null;
        }

        protected override void LoadObjectsInRelationship(UserFoldersStructure obj)
        {
            if (obj.UserFoldersStructureObjects != null)
            {
                IList<UserFoldersStructureObject> list = new List<UserFoldersStructureObject>();

                foreach (UserFoldersStructureObject userFoldersStructureObject in obj.UserFoldersStructureObjects)
                {
                    UserFoldersStructureObject actUserFoldersStructureObject = UserFoldersStructureObjects.Singleton.GetById(userFoldersStructureObject.IdUserFoldersStructureObject);
                    actUserFoldersStructureObject.Folder = obj;

                    list.Add(actUserFoldersStructureObject);
                }

                obj.UserFoldersStructureObjects.Clear();
                foreach (UserFoldersStructureObject userFoldersStructureObject in list)
                    obj.UserFoldersStructureObjects.Add(userFoldersStructureObject);
            }

            if (obj.ParentFolder != null)
            {
                obj.ParentFolder = GetById(obj.ParentFolder.IdUserFoldersStructure);
            }
        }

        public bool HasSubFolders(UserFoldersStructure userFoldersStructure)
        {
            if (userFoldersStructure == null)
                return false;

            userFoldersStructure = GetById(userFoldersStructure.IdUserFoldersStructure);

            if (userFoldersStructure != null)
            {
                if (userFoldersStructure.UserFoldersStructureObjects != null && userFoldersStructure.UserFoldersStructureObjects.Count > 0)
                    return true;

                ICollection<UserFoldersStructure> list = SelectLinq<UserFoldersStructure>(ufs => ufs.ParentFolder == userFoldersStructure);
                return list != null && list.Count > 0;
            }

            return false;
        }

        public ICollection<UserFoldersStructure> GetSubFolders(object userFoldersStructureId)
        {
            var userFoldersStructure = GetById(userFoldersStructureId);

            if (userFoldersStructure == null)
                return null;

            return SelectLinq<UserFoldersStructure>(ufs => ufs.ParentFolder == userFoldersStructure);
        }

        public IList<UserFoldersStructure> GetUserFoldersForObject(string objectId, ObjectType objectType)
        {
            return UserFoldersStructureObjects.Singleton.GetUserFoldersForObject(objectId, objectType);
        }

        public string GetFullPath(UserFoldersStructure userFoldersStructure)
        {
            userFoldersStructure = GetById(userFoldersStructure.IdUserFoldersStructure);
            if (userFoldersStructure != null)
            {
                string fullPath = string.Empty;
                UserFoldersStructure parentFolder = userFoldersStructure.ParentFolder;
                while (parentFolder != null)
                {
                    fullPath = parentFolder.FolderName + "\\" + fullPath;
                    parentFolder = parentFolder.ParentFolder;
                }

                fullPath += userFoldersStructure.FolderName;

                return fullPath;
            }

            return string.Empty;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<UserFoldersStructure> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name)) return null;
                linqResult = SelectLinq<UserFoldersStructure>(ufs => ufs.FolderName.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(ufs => ufs.FolderName).ToList();
                foreach (UserFoldersStructure tz in linqResult)
                {
                    resultList.Add(tz);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<UserFoldersStructure> linqResult = null;

            if (name != null && name != string.Empty)
            {
                linqResult = SelectLinq<UserFoldersStructure>(ufs => ufs.FolderName.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<UserFoldersStructure> linqResult = 
                string.IsNullOrEmpty(name) ? 
                    List() : 
                    SelectLinq<UserFoldersStructure>(ufs => ufs.FolderName.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public IList<UserFoldersStructure> FolderStructureSearch(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            IList<UserFoldersStructure> resultList = new List<UserFoldersStructure>();

            ICollection<UserFoldersStructure> linqResult = 
                SelectLinq<UserFoldersStructure>(ufs => ufs.FolderName.IndexOf(text) >= 0);

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(ufs => ufs.FolderName).ToList();
                foreach (UserFoldersStructure ufs in linqResult)
                {
                    UserFoldersStructure outUfs = GetById(ufs.IdUserFoldersStructure);
                    resultList.Add(outUfs);
                }
            }
            return resultList;
        }

        public UserFoldersStructure FolderStructureSearchExactName(string name, UserFoldersStructure parent)
        {
            UserFoldersStructure outUfs = null;

            if (string.IsNullOrEmpty(name)) return outUfs;

            IList<UserFoldersStructure> resultList = new List<UserFoldersStructure>();

            UserFoldersStructure linqResult =
                SelectLinq<UserFoldersStructure>(ufs => ufs.FolderName == name && ufs.ParentFolder == parent).FirstOrDefault();

            if (linqResult != null)
            {
                 outUfs = GetById(linqResult.IdUserFoldersStructure);

            }
            return outUfs;
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<UserFoldersStructure> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(ufs => ufs.FolderName).ToList();
                foreach (UserFoldersStructure tz in linqResult)
                {
                    resultList.Add(tz);
                }
            }
            return resultList;
        }

        public ICollection<AOrmObject> ListDepartments(string rootName, string separator, object personId,
            out Exception error)
        {
            var login = AccessChecker.GetActualLogin();

            var departments = List(login, out error);
            if (departments == null)
                return null;

            var loginTopMostSitesId =
                new HashSet<int>(
                    StructuredSubSites.Singleton.GetTopMostSites(login).Select(siteInfo => siteInfo.Id));

            ObjectPlacement objectPlacement;
            if (personId != null)
            {
                objectPlacement = StructuredSubSites.Singleton.GetObjectPlacements(
                    ObjectType.Person,
                    personId.ToString(),
                    separator,
                    rootName,
                    loginTopMostSitesId,
                    true).FirstOrDefault();
            }
            else
            {
                objectPlacement = null;
            }

            var result = new List<AOrmObject>();

            foreach (var department in departments)
            {
                var departmentPlacment = StructuredSubSites.Singleton.GetObjectPlacements(
                    department.GetObjectType(),
                    department.GetIdString(),
                    separator,
                    rootName,
                    loginTopMostSitesId,
                    true).FirstOrDefault();

                if (departmentPlacment != null &&
                    (objectPlacement == null ||
                     objectPlacement.SiteId == departmentPlacment.SiteId))
                {
                    department.SetFullFolderName(
                        string.Format(
                            "{0}{1}{2}",
                            departmentPlacment.Name,
                            separator,
                            department.FolderName));

                    result.Add(department);
                }
            }

            result.Sort(delegate(AOrmObject ufs1, AOrmObject ufs2)
            {
                return ufs1.ToString().CompareTo(ufs2.ToString());
            });

            return result;
        }

        public ICollection<AOrmObject> ListDepartmentsBySubSite(
            int subSiteId,
            out Exception error)
        {
            var login = AccessChecker.GetActualLogin();

            var departments = List(login, out error);
            if (departments == null)
                return null;

            var result = new List<AOrmObject>();

            foreach (var department in departments)
            {
                var globalSiteInfo = StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                    department.IdUserFoldersStructure,
                    ObjectType.UserFoldersStructure);

                if (globalSiteInfo.Id == subSiteId)
                {
                    result.Add(department);

                    department.SetFullFolderName(
                        GetFullPath(department));
                }
            }

            result.Sort(delegate(AOrmObject ufs1, AOrmObject ufs2)
            {
                return ufs1.ToString().CompareTo(ufs2.ToString());
            });

            return result;
        }

        public UserFoldersStructure GetPersonDepartment(string strPersonId)
        {
            var personUfsos =
                SelectLinq<UserFoldersStructureObject>(
                    ufso => ufso.ObjectType == ObjectType.Person && ufso.ObjectId == strPersonId);

            if (personUfsos != null && personUfsos.Count > 0)
            {
                var folder = personUfsos.First().Folder;

                if (folder != null)
                {
                    return GetById(folder.IdUserFoldersStructure);
                }
            }

            // fallback to legacy department stored directly on person
            try
            {
                var session = NhHelper.Singleton.GetSession();
                using (session.BeginTransaction())
                {
                    var query = session.CreateSQLQuery(
                        string.Format("select {0} from Person where {1} = :id", Person.COLUMNDEPARTMENT, Person.COLUMNIDPERSON));
                    query.SetGuid("id", new Guid(strPersonId));
                    var department = query.UniqueResult<string>();

                    if (!string.IsNullOrEmpty(department))
                    {
                        return new UserFoldersStructure
                        {
                            FolderName = department
                        };
                    }
                }
            }
            catch
            {
                // ignore errors and return null
            }

            return null;
        }

        public string GetFullDepartmentName(string strDepartmentId, string folderName, string separator, string rootName)
        {
            var departmentPlacment = StructuredSubSites.Singleton.GetObjectPlacements(
                    ObjectType.UserFoldersStructure,
                    strDepartmentId,
                    separator,
                    rootName,
                    true).FirstOrDefault();

            if (departmentPlacment != null)
            {
                return string.Format(
                    "{0}{1}{2}",
                    departmentPlacment.Name,
                    separator,
                    folderName);
            }

            return folderName;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.UserFoldersStructure; }
        }

        public ICollection<ObjectInSiteInfo> LoadFolderStructureObjects(object folderStructureId)
        {
            var objectsInfo = new LinkedList<ObjectInSiteInfo>();
            var userFolderStructure =
                GetObjectById(folderStructureId);

            if (userFolderStructure != null &&
                userFolderStructure.UserFoldersStructureObjects != null)
            {
                var login = AccessChecker.GetActualLogin();
                var topMostSites = StructuredSubSites.Singleton.GetTopMostSites(login);
                var tabelOrmByObjectType = new Dictionary<ObjectType, ITableORM>();
                var hasAccessViewByObjectType = new Dictionary<ObjectType, bool>();

                foreach (var userfolderStructureObject in userFolderStructure.UserFoldersStructureObjects)
                {
                    var objectType = userfolderStructureObject.ObjectType;

                    ITableORM tableOrm;
                    if (!tabelOrmByObjectType.TryGetValue(
                        objectType,
                        out tableOrm))
                    {
                        tableOrm = CgpServerRemotingProvider.Singleton.GetTableOrmForObjectType(
                            objectType);

                        tabelOrmByObjectType.Add(
                            objectType,
                            tableOrm);
                    }

                    if (tableOrm == null)
                        continue;

                    bool hasAccessView;
                    if (!hasAccessViewByObjectType.TryGetValue(
                        objectType,
                        out hasAccessView))
                    {
                        hasAccessView = tableOrm.HasAccessView(login);

                        hasAccessViewByObjectType.Add(
                            objectType,
                            hasAccessView);
                    }

                    if (!hasAccessView)
                        continue;

                    var objectId = (Guid) tableOrm.ParseId(
                        userfolderStructureObject.ObjectId);

                    if (!StructuredSubSites.Singleton.IsObjectVisibleForLogin(
                        new IdAndObjectType(
                            objectId,
                            objectType),
                        topMostSites))
                    {
                        continue;
                    }

                    objectsInfo.AddLast(new ObjectInSiteInfo(
                        objectId,
                        objectType,
                        CentralNameRegisters.Singleton.GetNameFromId(
                            objectId),
                        null));
                }
            }

            return objectsInfo;
        }
    }
}
