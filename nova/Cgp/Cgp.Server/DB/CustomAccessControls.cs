using System.Collections.Generic;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class CustomAccesControls : 
        ABaseOrmTable<CustomAccesControls, CustomAccessControl>, 
        ICustomAccessControls
    {
        private CustomAccesControls() : base(null)
        {
        }

        public int GetCount()
        {
            ICollection<CustomAccessControl> list = List();
            return list.Count;
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

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.CustomAccessControl; }
        }
    }
}
