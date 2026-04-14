using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.RemotingCommon;
using NHibernate;
using NHibernate.Criterion;

namespace Contal.Cgp.Server.DB
{
    public sealed class LoginGroups : 
        ABaseOrmTable<LoginGroups, LoginGroup>, 
        ILoginGroups
    {
        private LoginGroups() : base(null)
        {
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(LoginGroup obj)
        {
            var logins = obj.Logins;

            if (logins != null)
                foreach (var login in logins)
                    yield return login;
        }

        protected override IModifyObject CreateModifyObject(LoginGroup ormbObject)
        {
            return new LoginGroupModifyObj(ormbObject);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.LOGINS_LOGIN_GROUPS),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.LoginsLoginGroupsAdmin), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.LoginsLoginGroupsAdmin), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.LoginsLoginGroupsAdmin), login);
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(LoginGroup.COLUMN_LOGIN_GROUP_NAME, true));
        }

        public override void AfterUpdate(LoginGroup newLoginGroup, LoginGroup oldLoginGroup)
        {
            if (newLoginGroup.Logins == null)
            {
                return;
            }

            foreach (var login in newLoginGroup.Logins)
            {
                var loginForEdit = Logins.Singleton.GetObjectForEdit(login.IdLogin);
                if (loginForEdit != null)
                {
                    if (loginForEdit.AccessControls != null &&
                        loginForEdit.AccessControls.Count > 0)
                    {
                        loginForEdit.AccessControls.Clear();
                        Logins.Singleton.Update(loginForEdit);
                    }

                    Logins.Singleton.EditEnd(loginForEdit);
                }
            }
        }


        protected override void LoadObjectsInRelationship(LoginGroup obj)
        {
            if (obj.Logins != null)
            {
                IList<Login> list = new List<Login>();

                foreach (Login login in obj.Logins)
                {
                    list.Add(Logins.Singleton.GetById(login.IdLogin));
                }

                obj.Logins.Clear();
                foreach (Login login in list)
                    obj.Logins.Add(login);
            }

            if (obj.AccessControls != null)
            {
                IList<AccessControl> list = new List<AccessControl>();

                foreach (AccessControl accessControl in obj.AccessControls)
                {
                    list.Add(AccessControls.Singleton.GetById(accessControl.IdAccessControl));
                }

                obj.AccessControls.Clear();
                foreach (AccessControl accessControl in list)
                    obj.AccessControls.Add(accessControl);
            }
        }

        protected override bool AddCriteriaSpecial(ref ICriteria c, FilterSettings filterSetting)
        {
            if (filterSetting.Column == "ExpirationSet")
            {
                if ((bool)filterSetting.Value)
                {
                    c = c.Add(Restrictions.IsNotNull("ExpirationDate"));
                }
                else
                {
                    c = c.Add(Restrictions.IsNull("ExpirationDate"));
                }

                return true;
            }

            return false;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<LoginGroup> linqResult;

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
                    linqResult = SelectLinq<LoginGroup>(l => l.LoginGroupName.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult = SelectLinq<LoginGroup>(l => l.LoginGroupName.IndexOf(name) >= 0 || l.Description.IndexOf(name) >= 0);
                }
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(login => login.LoginGroupName).ToList();
                foreach (var dp in linqResult)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<LoginGroup> linqResult = null;

            if (!string.IsNullOrEmpty(name))
                linqResult =
                    SelectLinq<LoginGroup>(
                        lg =>
                            lg.LoginGroupName.IndexOf(name) >= 0 ||
                            lg.Description.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name) 
                    ? List() 
                    : SelectLinq<LoginGroup>(
                        lg => lg.LoginGroupName.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<LoginGroup> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(lg => lg.LoginGroupName).ToList();
                foreach (LoginGroup login in linqResult)
                {
                    resultList.Add(login);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<LoginGroupShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            ICollection<LoginGroup> listLoginGroup = SelectByCriteria(filterSettings, out error);
            
            var result = new List<LoginGroupShort>();
            if (listLoginGroup != null)
            {
                foreach (LoginGroup loginGroup in listLoginGroup)
                {
                    result.Add(new LoginGroupShort(loginGroup));
                }
            }

            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            ICollection<LoginGroup> listLoginGroups = List(out error);

            if (listLoginGroups != null)
            {
                var listLoginGroupsModifyObj = new List<IModifyObject>();
                foreach (LoginGroup loginGroup in listLoginGroups)
                {
                    listLoginGroupsModifyObj.Add(new LoginGroupModifyObj(loginGroup));
                }

                return listLoginGroupsModifyObj.OrderBy(loginGroup => loginGroup.ToString()).ToList();
            }

            return null;
        }

        public IList<IModifyObject> ListModifyObjectsFromLoginReferencesSites(
            Guid? idLogin,
            out Exception error)
        {
            var listLoginGroups = List(out error) as IEnumerable<LoginGroup>;

            if (listLoginGroups == null)
                return null;
            
            if (idLogin != null)
            {
                listLoginGroups =
                    StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsWhithSubTreeReferencesInSites(
                        StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfosOfObjectReferences(
                            idLogin.Value,
                            ObjectType.Login),
                        listLoginGroups,
                        (idLoginGroup) =>
                            IsDefaultAdminLoginGroup((Guid) idLoginGroup));
            }

            return listLoginGroups
                .Select(
                    loginGroup =>
                        new LoginGroupModifyObj(loginGroup))
                .Cast<IModifyObject>()
                .OrderBy(
                    modifyObject =>
                        modifyObject.ToString())
                .ToList();
        }

        public bool IsDefaultAdminLoginGroup(Guid idLoginGroup)
        {
            var loginGroup = GetById(idLoginGroup);

            return loginGroup != null
                   && loginGroup.LoginGroupName == CgpServerGlobals.DEFAULT_ADMIN_LOGIN_GROUP;
        }

        public bool IsLoginGroupSuperAdmin(string loginGroupName)
        {
            return GetAccessControl(loginGroupName, BaseAccess.GetAccess(LoginAccess.SuperAdmin));
        }
        
        public bool GetAccessControl(string loginGroupName, Access access)
        {
            if (string.IsNullOrEmpty(loginGroupName))
                return false;

            return GetAccessControl(SelectLinq<LoginGroup>(actLoginGroup => actLoginGroup.LoginGroupName == loginGroupName).FirstOrDefault(), access);
        }

        public bool GetAccessControl(LoginGroup loginGroup, Access access)
        {
            if (loginGroup == null)
                return false;

            return Login.HasAccess(loginGroup.AccessControls, access);
        }

        public LoginGroup GetOrCreateDefaultAdminLoginGroup()
        {
            LoginGroup adminLoginGroup =
                SelectLinq<LoginGroup>(
                    loginGroup => loginGroup.LoginGroupName == CgpServerGlobals.DEFAULT_ADMIN_LOGIN_GROUP)
                    .FirstOrDefault();

            if (adminLoginGroup == null)
            {
                adminLoginGroup = new LoginGroup(CgpServerGlobals.DEFAULT_ADMIN_LOGIN_GROUP, false, null, true);
                Insert(ref adminLoginGroup);
                
                var structuredSubSiteObject = new StructuredSubSiteObject
                {
                    StructuredSubSite = null,
                    ObjectId = adminLoginGroup.IdLoginGroup.ToString(),
                    ObjectType = ObjectType.LoginGroup,
                    IsReference = true
                };

                StructuredSubSiteObjects.Singleton.Insert(ref structuredSubSiteObject);
            }

            return adminLoginGroup;
        }

        public IdAndObjectType GetLoginGroupByName(string loginGroupName)
        {
            var loginGroups = SelectLinq<LoginGroup>(
                loginGroupFromDatabase =>
                    loginGroupFromDatabase.LoginGroupName == loginGroupName);

            if (loginGroups == null
                || loginGroups.Count == 0)
            {
                return null;
            }

            var loginGroup = loginGroups.First();
            return new IdAndObjectType(loginGroup.IdLoginGroup, ObjectType.LoginGroup);
        }

        public ICollection<ObjectInSiteInfo> GetLoginsOfLoginGroup(Guid loginGroupId)
        {
            var loginGroup = GetById(loginGroupId);
            if (loginGroup == null
                || loginGroup.Logins == null)
                return new LinkedList<ObjectInSiteInfo>();

            return new LinkedList<ObjectInSiteInfo>(
                loginGroup.Logins.Select(
                    login =>
                        new ObjectInSiteInfo(
                            login.IdLogin,
                            ObjectType.Login,
                            login.Username,
                            null)));
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.LoginGroup; }
        }
    }
}
