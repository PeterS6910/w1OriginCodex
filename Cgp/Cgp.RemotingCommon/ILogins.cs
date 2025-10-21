using System;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface ILogins : IBaseOrmTable<Login>
    {
        bool ChengeLoginPassword(string loginUserName, string oldPassword, string newPassword, out Exception error);
        bool CheckLoginPassword(string loginUserName, string password, out Exception error);
        bool CheckLoginPassword(string fullCardNumber, out string loginName, out Exception error);

        ICollection<LoginShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);

        IList<IModifyObject> ListModifyObjectsFromLoginGroupReferencesSites(
            Guid idLoginGroup,
            ICollection<Login> addedLogins,
            out Exception error);

        bool IsLoginInLoginGroupReferencesSites(Guid idLoginGroup, Guid idLogin);

        bool IsLoginSuperAdmin(string username);
        bool IsPassOutOfDate();
        bool MustChangePassword();
        Login GetActualLogin();
        bool AssignedToLoginGroup(Guid loginId, Guid loginGroupId);
    }
}
