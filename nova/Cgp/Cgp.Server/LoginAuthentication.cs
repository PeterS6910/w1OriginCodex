using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server
{
    using T_USER_ID = Guid;

    public class LoginAuthentication 
        : Contal.IwQuick.Remoting.ARemotingAuthentication<
            Contal.Cgp.BaseLib.LoginAuthenticationParameters,
            T_USER_ID>
    {
        private Dictionary<object, string> _loggedUsers = new Dictionary<object, string>();
        private IDictionary<T_USER_ID, ICollection<object>> _loggedUsersId = new Dictionary<T_USER_ID, ICollection<object>>();

        /// <summary>
        /// Verify if current user is logged in. 
        /// </summary>
        /// <param name="sessionIdentification"></param>
        /// <param name="parameters"></param>
        /// <returns>return true if success or throw exception</returns>
        protected override bool Verification(object sessionIdentification, Contal.Cgp.BaseLib.LoginAuthenticationParameters parameters)
        {
            Guid loginId;
            bool isDisabled;
            bool isExpired;
            bool isSuperAdmin;
            bool result = Logins.Singleton.ExistLogin(parameters.LoginUsername, parameters.LoginPasswordHash, out loginId, out isDisabled, out isExpired, out isSuperAdmin);

            if (!result)
                throw new InvalidSecureEntityException(LoginAuthenticationParameters.WRONG_USERNAME_OR_PASSWORD, parameters.LoginUsername);

            if (isDisabled)
                throw new InvalidSecureEntityException(LoginAuthenticationParameters.LOGIN_DISABLED, parameters.LoginUsername);

            if (isExpired)
                throw new InvalidSecureEntityException(LoginAuthenticationParameters.LOGIN_EXPIRED, parameters.LoginUsername);

            if (sessionIdentification != null)
            {
                if (!isSuperAdmin)
                {
                    lock (_loggedUsers)
                    {
                        if (_loggedUsers.ContainsValue(parameters.LoginUsername))
                        {
                            throw new InvalidSecureEntityException(LoginAuthenticationParameters.USER_ALREADY_LOGGED, parameters.LoginUsername);
                        }
                    }
                }

                Login login = Logins.Singleton.GetLoginByUserName(parameters.LoginUsername);

                lock (_loggedUsers)
                {
                    _loggedUsers[sessionIdentification] = parameters.LoginUsername;

                    ICollection<object> sessionsForUser;

                    if (!_loggedUsersId.TryGetValue(login.IdLogin, out sessionsForUser))
                    {
                        sessionsForUser = new HashSet<object>();
                        _loggedUsersId[login.IdLogin] = sessionsForUser;
                    }

                    if (!sessionsForUser.Contains(sessionIdentification))
                    {
                        sessionsForUser.Add(sessionIdentification);
                    }
                }
            }

            parameters.LoginId = loginId;
            return true;
        }

        /// <summary>
        /// Remove user from user list because his session timeouted
        /// </summary>
        /// <param name="sessionIdentification"></param>
        public override void LogoutSessionTimeOut(object sessionIdentification)
        {
            lock (_loggedUsers)
            {
                if (_loggedUsers.ContainsKey(sessionIdentification))
                {
                    RemoveUserBySessionIdInternal(sessionIdentification);
                }
            }
        }

        /// <summary>
        /// Obtaining session id according to authentication parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override object GetSessionId(LoginAuthenticationParameters parameters)
        {
            if (parameters == null)
                return null;

            lock (_loggedUsers)
            {
                object sessionIdentification = null;
                foreach (KeyValuePair<object, string> kvp in _loggedUsers)
                {
                    if (parameters.LoginUsername == kvp.Value)
                    {
                        sessionIdentification = kvp.Key;
                        break;
                    }
                }

                return sessionIdentification;
            }
        }

        /// <summary>
        /// Remove user with provided session from logged in list. This method should be called from lock(_loggedUsers){...}
        /// </summary>
        /// <param name="sessionIdentification"></param>
        private void RemoveUserBySessionIdInternal(object sessionIdentification)
        {
            _loggedUsers.Remove(sessionIdentification);

            Guid id = Guid.Empty;

            // Find user guid for current session
            foreach (var userRecord in _loggedUsersId)
            {
                object tempId = userRecord
                    .Value
                    .FirstOrDefault(sessionId => sessionId.Equals(sessionIdentification));

                if (tempId != null)
                {
                    id = userRecord.Key;
                    break;
                }
            }

            // Check if user id was found
            if (id != Guid.Empty)
            {
                // Get session list for current users
                ICollection<object> sessions;
                if (_loggedUsersId.TryGetValue(id, out sessions))
                {
                    sessions.Remove(sessionIdentification);

                    // If there is no more sessions for this user ID remove record with this ID
                    if (sessions.Count == 0)
                        _loggedUsersId.Remove(id);
                }
            }
        }

        public override IEnumerable<object> GetSessionsForUser(T_USER_ID userIdentification)
        {
            lock (_loggedUsers)
            {
                ICollection<object> sessions;
                if (!_loggedUsersId.TryGetValue(userIdentification, out sessions))
                    return new object[0];

                return new LinkedList<object>(sessions);
            }
        }
    }
}
