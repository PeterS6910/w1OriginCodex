using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NHibernate;
using NHibernate.Criterion;

using Contal.Cgp.RemotingCommon;
using Contal.IwQuick;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.DB
{
    public sealed class Logins : 
        ABaserOrmTableWithAlarmInstruction<Logins, Login>,
        ILogins
    {
        private Logins() : base(null)
        {
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var login = ormObject as Login;

            if (login == null
                || login.LoginGroup == null
                || login.LoginGroup.LoginGroupName == CgpServerGlobals.DEFAULT_ADMIN_LOGIN_GROUP)
            {
                return null;
            }

            return login.LoginGroup;
        }

        protected override IModifyObject CreateModifyObject(Login ormbObject)
        {
            return new LoginModifyObj(ormbObject);
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

            if (filterSetting.Column == LoginShort.COLUMN_LOGIN_GROUP)
            {
                var strValue = filterSetting.Value as string;

                if (string.IsNullOrEmpty(strValue))
                    return true;

                var loginGroups = SelectLinq<LoginGroup>(loginGroup => loginGroup.LoginGroupName.IndexOf(strValue) >= 0);
                c = c.Add(Restrictions.In(Login.COLUMN_LOGIN_GROUP, loginGroups as ICollection));

                return true;
            }

            return false;
        }

        public bool ExistLogin(string userName, string passwordHash, out Guid loginId, out bool isDisabled, out bool isExpired,
            out bool isSuperAdmin)
        {
            loginId = Guid.Empty;
            isDisabled = true;
            isExpired = true;
            isSuperAdmin = false;

            var login =
                SelectLinq<Login>(l => l.Username == userName && l.PasswordHash == passwordHash).FirstOrDefault();

            if (null == login)
                return false;

            loginId = login.IdLogin;
            isExpired = login.ExpirationDate <= DateTime.Now;
            isDisabled = (login.LoginGroup != null && login.LoginGroup.IsDisabled) || login.IsDisabled;
            isSuperAdmin = IsLoginSuperAdmin(login);
            return true;
        }

        protected internal void EnsureDefaultAccess()
        {
            var adminLoginGroup = LoginGroups.Singleton.GetOrCreateDefaultAdminLoginGroup();

            var admin =
                SelectLinq<Login>(login => login.Username == CgpServerGlobals.DEFAULT_ADMIN_LOGIN).FirstOrDefault();

            if (!CreateAdminLogin(ref admin, adminLoginGroup))
                return;

            var structuredSubSiteObject = new StructuredSubSiteObject
            {
                StructuredSubSite = null,
                ObjectId = admin.IdLogin.ToString(),
                ObjectType = ObjectType.Login,
                IsReference = true
            };

            StructuredSubSiteObjects.Singleton.Insert(ref structuredSubSiteObject);
        }

        private bool CreateAdminLogin(ref Login admin, LoginGroup adminLoginGroup)
        {
            if (admin == null)
            {
                admin = new Login(
                    CgpServerGlobals.DEFAULT_ADMIN_LOGIN,
                    PasswordHelper.GetPasswordhash(
                        CgpServerGlobals.DEFAULT_ADMIN_LOGIN,
                        CgpServerGlobals.DEFAULT_ADMIN_LOGIN),
                    adminLoginGroup,
                    false,
                    null,
                    true);

                Insert(ref admin);

                return true;
            }

            if (admin.LoginGroup == null)
            {
                admin = GetObjectForEdit(admin.IdLogin);
                if (admin == null)
                    return true;

                admin.LoginGroup = adminLoginGroup;
                Update(admin);
                EditEnd(admin);

                return true;
            }

            return false;
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.LOGINS_LOGIN_GROUPS), login);
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
            c.AddOrder(new Order(Login.COLUMN_USERNAME, true));
        }

        protected override void ReloadBeforeUpdateDelete(Login login)
        {
            if (login.LoginGroup != null)
            {
                login.AccessControls.Clear();
            }
        }

        public Login GetLoginByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            return SelectLinq<Login>(actLogin => actLogin.Username == userName).FirstOrDefault();
        }

        public bool ChengeLoginPassword(string loginUserName, string oldPassword, string newPassword, out Exception error)
        {
            error = null;
            var login = SelectLinq<Login>(actLogin => actLogin.Username == loginUserName).FirstOrDefault();

            if (login == null)
            {
                error = new InvalidSecureEntityException(LoginAuthenticationParameters.WRONG_USERNAME_OR_PASSWORD, loginUserName);
                return false;
            }

            login = GetObjectForEdit(login.IdLogin);

            if (login == null)
            {
                return false;
            }

            if (login.PasswordHash != oldPassword)
            {

                error = new InvalidSecureEntityException(LoginAuthenticationParameters.WRONG_USERNAME_OR_PASSWORD, loginUserName);
                return false;
            }

            login.PasswordHash = newPassword;
            login.LastPasswordChangeDate = DateTime.Now;
            login.MustChangePassword = false;

            var retValue = Update(login);
            EditEnd(login);
            return retValue;
        }

        public bool CheckLoginPassword(string loginUserName, string password, out Exception error)
        {
            error = null;
            var login = SelectLinq<Login>(actLogin => actLogin.Username == loginUserName).FirstOrDefault();

            if (login == null)
            {
                error = new InvalidSecureEntityException(LoginAuthenticationParameters.WRONG_USERNAME_OR_PASSWORD, loginUserName);
                return false;
            }

            if (login.PasswordHash != password)
            {
                error = new InvalidSecureEntityException(LoginAuthenticationParameters.WRONG_USERNAME_OR_PASSWORD, loginUserName);
                return false;
            }

            if ((login.LoginGroup != null && login.LoginGroup.IsDisabled) || login.IsDisabled)
            {
                error = new InvalidSecureEntityException(LoginAuthenticationParameters.LOGIN_DISABLED, loginUserName);
                return false;
            }

            return true;
        }

        protected override void LoadObjectsInRelationship(Login obj)
        {
            if (obj.Person != null)
                obj.Person = Persons.Singleton.GetById(obj.Person.IdPerson);

            if (obj.LoginGroup != null)
                obj.LoginGroup = LoginGroups.Singleton.GetById(obj.LoginGroup.IdLoginGroup);

            if (obj.AccessControls != null)
            {
                var list = new LinkedList<AccessControl>(
                    obj.AccessControls.Select(
                        accessControl =>
                            AccessControls.Singleton.GetById(accessControl.IdAccessControl)));

                obj.AccessControls.Clear();
                foreach (var accessControl in list)
                    obj.AccessControls.Add(accessControl);
            }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var login = GetById(idObj);
                if (login == null) return null;

                IList<AOrmObject> result = new List<AOrmObject>();
                if (login.Person != null)
                {
                    var outPerson = Persons.Singleton.GetById(login.Person.IdPerson);
                    result.Add(outPerson);
                }

                if (login.LoginGroup != null)
                {
                    var outLoginGroup = LoginGroups.Singleton.GetById(login.LoginGroup.IdLoginGroup);
                    result.Add(outLoginGroup);
                }

                if (result.Count == 0)
                {
                    return null;
                }

                return result.OrderBy(orm => orm.ToString()).ToList();
            }
            catch
            {
                return null;
            }
        }

        public LoginAuthenticationParameters GetAuthParamFromCard(string cardNumber)
        {
            var card = Cards.Singleton.GetCardByFullNumber(cardNumber);
            if (card == null || (card.State != (byte)CardState.Active && card.State != (byte)CardState.HybridActive)) return null;

            var lingResult = SelectLinq<Login>(l => l.Person.Cards.Contains(card));
            if (lingResult != null && lingResult.Count > 0)
            {
                return new LoginAuthenticationParameters(lingResult.ElementAt(0).Username, lingResult.ElementAt(0).PasswordHash, true);
            }
            return null;
        }

        public bool CheckLoginPassword(string fullCardNumber, out string loginName, out Exception error)
        {
            error = null;
            Login login = null;
            loginName = string.Empty;

            var card = Cards.Singleton.GetCardByFullNumber(fullCardNumber);
            if (card != null)
            {
                var lingResult = SelectLinq<Login>(l => l.Person.Cards.Contains(card));
                if (lingResult != null && lingResult.Count > 0)
                {
                    login = lingResult.ElementAt(0);
                    loginName = login.Username;
                }
            }

            if (login == null)
            {
                error = new InvalidSecureEntityException(LoginAuthenticationParameters.WRONG_CARD, fullCardNumber);
                return false;
            }

            if ((login.LoginGroup != null && login.LoginGroup.IsDisabled) || login.IsDisabled)
            {
                error = new InvalidSecureEntityException(LoginAuthenticationParameters.LOGIN_DISABLED, login.Username);
                return false;
            }

            return true;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<Login> linqResult;

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
                    linqResult = SelectLinq<Login>(l => l.Username.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult = SelectLinq<Login>(l => l.Username.IndexOf(name) >= 0 || l.Description.IndexOf(name) >= 0);
                }
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(login => login.Username).ToList();
                foreach (var dp in linqResult)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<Login> linqResult = null;

            if (name != null && name != string.Empty)
            {
                linqResult = SelectLinq<Login>(l => l.Username.IndexOf(name) >= 0 || l.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<Login> linqResult = 
                string.IsNullOrEmpty(name) 
                    ? List() 
                    : SelectLinq<Login>(l => l.Username.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private IList<AOrmObject> ReturnAsListOrmObject(ICollection<Login> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(login => login.Username).ToList();
                foreach (var login in linqResult)
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

        public ICollection<LoginShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listLogin = SelectByCriteria(filterSettings, out error);
            ICollection<LoginShort> result = new List<LoginShort>();
            if (listLogin != null)
            {
                foreach (var login in listLogin)
                {
                    result.Add(new LoginShort(login));
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listLogins = List(out error);
            IList<IModifyObject> listLoginsModifyObj = null;
            if (listLogins != null)
            {
                listLoginsModifyObj = listLogins
                    .Select(
                        login =>
                            new LoginModifyObj(login))
                    .Cast<IModifyObject>()
                    .OrderBy(
                        modifyObject =>
                            modifyObject.ToString())
                    .ToList();
            }
            return listLoginsModifyObj;
        }

        public IList<IModifyObject> ListModifyObjectsFromLoginGroupReferencesSites(
            Guid idLoginGroup,
            ICollection<Login> addedLogins,
            out Exception error)
        {
            var listLogins = List(out error) as IEnumerable<Login>;
            IList<IModifyObject> listLoginsModifyObj = null;
            if (listLogins != null)
            {
                if (!LoginGroups.Singleton.IsDefaultAdminLoginGroup(idLoginGroup))
                {
                    listLogins =
                        StructuredSubSites.Singleton.GlobalEvaluator.GetObjectsWhithSubTreeReferencesInSites(
                            StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfosOfObjectReferences(
                                idLoginGroup,
                                ObjectType.LoginGroup),
                            listLogins,
                            null);
                }

                var addedLoginsId = addedLogins != null
                    ? new HashSet<Guid>(
                        addedLogins.Select(
                            login =>
                                login.IdLogin))
                    : new HashSet<Guid>();

                listLoginsModifyObj = listLogins
                    .Where(
                        login =>
                            !addedLoginsId.Contains(login.IdLogin))
                    .Select(
                        login =>
                            new LoginModifyObj(login))
                    .Cast<IModifyObject>()
                    .OrderBy(
                        modifyObject =>
                            modifyObject.ToString())
                    .ToList();
            }
            return listLoginsModifyObj;
        }

        public bool IsLoginInLoginGroupReferencesSites(Guid idLoginGroup, Guid idLogin)
        {
            if (LoginGroups.Singleton.IsDefaultAdminLoginGroup(idLoginGroup))
                return true;

            var loginGroupReferecnesSiteIds = new HashSet<int>(
                StructuredSubSites.Singleton.GlobalEvaluator
                    .GetGlobalSiteInfosOfObjectReferences(
                        idLoginGroup,
                        ObjectType.LoginGroup)
                    .Select(
                        globalSiteInfo =>
                            globalSiteInfo.Id));

            var loginReferencesSiteIds =
                StructuredSubSites.Singleton.GlobalEvaluator
                    .GetGlobalSiteInfosOfObjectReferences(
                        idLogin,
                        ObjectType.Login)
                    .Select(
                        globalSiteInfo =>
                            globalSiteInfo.Id);

            return loginReferencesSiteIds.Any(
                loginGroupReferecnesSiteIds.Contains);
        }

        public bool IsPassOutOfDate()
        {
            var login = AccessChecker.GetActualLogin();

            if (login == null)
                return false;

            DateTime? lastPassChangeDate = login.LastPasswordChangeDate;
            if (lastPassChangeDate != null)
            {
                if (GeneralOptions.Singleton.ChangePassDays > 0)
                {
                    lastPassChangeDate = lastPassChangeDate.Value.AddDays(GeneralOptions.Singleton.ChangePassDays);

                    if ((lastPassChangeDate.Value.CompareTo(DateTime.Now) == -1) &&
                        (!IsLoginSuperAdmin(login.IdLogin)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool MustChangePassword()
        {
            var login = AccessChecker.GetActualLogin();

            return login != null && login.MustChangePassword;
        }

        public bool IsLoginSuperAdmin(string userName)
        {
            var login = SelectLinq<Login>(actLogin => actLogin.Username == userName).FirstOrDefault();
            return login != null && IsLoginSuperAdmin(login);
        }

        public Login GetActualLogin()
        {
            return AccessChecker.GetActualLogin();
        }

        public bool IsLoginSuperAdmin(Guid loginId)
        {
            return GetAccessControl(loginId, BaseAccess.GetAccess(LoginAccess.SuperAdmin));
        }

        public bool IsLoginSuperAdmin(Login login)
        {
            return GetAccessControl(login, BaseAccess.GetAccess(LoginAccess.SuperAdmin));
        }

        public bool GetAccessControl(Guid loginId, Access access)
        {
            return loginId != Guid.Empty &&
                   GetAccessControl(GetById(loginId), access);
        }

        public bool GetAccessControl(Login login, Access access)
        {
            if (login == null)
                return false;

            if (login.LoginGroup != null)
            {
                return LoginGroups.Singleton.GetAccessControl(login.LoginGroup, access);
            }

            return Login.HasAccess(login.AccessControls, access);
        }

        protected override bool AddObjectNotInStructuredSubSite(Login ormObject, bool getExistObjects)
        {
            if (ormObject == null)
                return false;

            if (!getExistObjects && ormObject.LoginGroup != null)
                return false;

            return true;
        }

        public bool AssignedToLoginGroup(Guid loginId, Guid loginGroupId)
        {
            var login = GetById(loginId);
            if (login == null
                || login.LoginGroup == null)
                return false;

            return login.LoginGroup.IdLoginGroup.Equals(loginGroupId);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.Login; }
        }

        public override bool DeleteIfReferenced(object id, IList<AOrmObject> referencedObjects)
        {
            return referencedObjects == null
                   || referencedObjects.All(
                       referencedObject =>
                           referencedObject.GetObjectType() == ObjectType.LoginGroup);
        }

        protected override IEnumerable<Login> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<Login>(
                login =>
                    login.LocalAlarmInstruction != null
                    && login.LocalAlarmInstruction != string.Empty);
        }
    }
}
