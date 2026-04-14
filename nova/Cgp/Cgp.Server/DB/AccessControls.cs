using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class AccessControls : ABaseOrmTable<AccessControls, AccessControl>
    {
        private AccessControls() : base(null)
        {
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

        protected override void LoadObjectsInRelationship(AccessControl obj)
        {
            if (obj.Login != null)
                obj.Login = Logins.Singleton.GetById(obj.Login.IdLogin);

            if (obj.LoginGroup != null)
                obj.LoginGroup = LoginGroups.Singleton.GetById(obj.LoginGroup.IdLoginGroup);
        }

        public override Globals.ObjectType ObjectType
        {
            get { return Globals.ObjectType.AccessControl; }
        }
    }
}
