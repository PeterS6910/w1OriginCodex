using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Remoting;
using System;

namespace Contal.Cgp.Server.DB
{
    public static class AccessChecker
    {
        public static Login GetActualLogin()
        {
            var loginId = GetActualLoginId();

            return loginId == Guid.Empty
                ? null
                : Logins.Singleton.GetById(loginId);
        }

        public static Guid GetActualLoginId()
        {
            string sessionId = ClientIdentificationServerSink.CallingSessionId;

            if (sessionId == null)
                return Guid.Empty;

            var rs = RemotingSessionHandler.Singleton[sessionId];

            var parameters =
                rs[RemotingSession.SESSIONPARAMETERS] as LoginAuthenticationParameters;

            return parameters != null
                ? parameters.LoginId
                : Guid.Empty;
        }

        public static bool HasAccessControl(Access access, Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login) ||
                   Logins.Singleton.GetAccessControl(login, access);
        }

        public static bool HasAccessControl(List<Access> accesses, Login login)
        {
            if (Logins.Singleton.IsLoginSuperAdmin(login))
                return true;

            if (accesses == null || accesses.Count == 0)
                return false;

            foreach (var access in accesses)
            {
                if (Logins.Singleton.GetAccessControl(login, access))
                    return true;
            }

            return false;
        }

        public static bool IsSuperAdminLogged()
        {
            var loginId = GetActualLoginId();

            return loginId != Guid.Empty &&
                   Logins.Singleton.IsLoginSuperAdmin(loginId);
        }
    }
}