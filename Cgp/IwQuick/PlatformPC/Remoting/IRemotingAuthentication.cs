using System.Collections.Generic;

namespace Contal.IwQuick.Remoting
{
    public interface IRemotingAuthentication<T_USER_ID>
    {
        void Authenticate(RemotingSession session, ARemotingAuthenticationParameters parameters);
        void CheckValidParameters(ARemotingAuthenticationParameters parameters);
        bool IsValidParamaters(ARemotingAuthenticationParameters parameters);
        void Logout(RemotingSession session);
        void LogoutSessionTimeOut(object sessionIdentification);
        object GetSessionId(ARemotingAuthenticationParameters parameters);
        IEnumerable<object> GetSessionsForUser(T_USER_ID userIdentification);
    }
}
