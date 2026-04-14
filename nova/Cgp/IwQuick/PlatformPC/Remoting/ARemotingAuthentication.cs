using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    public abstract class ARemotingAuthentication<T_AUTH_PARAMS, T_USER_ID> : IRemotingAuthentication<T_USER_ID> 
        where T_AUTH_PARAMS: ARemotingAuthenticationParameters
    {
        /// <summary>
        /// authentication verification process body to be implemented in overriden class;
        /// overriding method should return the process result either by bool return or by 
        /// returning true if successful and throwing an exception if verification failed
        /// </summary>
        /// <param name="sessionIdentification"></param>
        /// <param name="parameters"></param>
        protected abstract bool Verification(object sessionIdentification, T_AUTH_PARAMS parameters);

        public abstract void LogoutSessionTimeOut(object sessionIdentification);

        /// <summary>
        /// Abstract method for obtaining session id according to authentication parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract object GetSessionId(T_AUTH_PARAMS parameters);

        /// <summary>
        /// performs the authentication; if ends without exception, authentication succeeded
        /// </summary>
        /// <param name="session"></param>
        /// <param name="parameters"></param>
        /// <exception cref="InvalidSecureEntityException">authentication process did not succeeded</exception>
        /// <exception cref="InvalidCastException">if supplied authentication parameters are not valid for this kind of authentication</exception>
        /// <exception cref="ArgumentNullException">if the parameters argument is null or related to wrong authentication</exception>
        public void Authenticate([NotNull] RemotingSession session, ARemotingAuthenticationParameters parameters)
        {
            Validator.CheckForNull(session,"session");
            CheckValidParameters(parameters);

            try
            {
                bool result = Verification(session.Identification, (T_AUTH_PARAMS)parameters);

                if (result)
                    session.Authenticated = true;
                else
                    throw new InvalidSecureEntityException(_implicitAuthError);
            }
            catch (InvalidSecureEntityException) {
                session.Authenticated = false;
                throw;
            }
            catch (Exception error)
            {
                session.Authenticated = false;
                throw new InvalidSecureEntityException(_implicitAuthError, error);
            }

            
        }

        /// <summary>
        /// reverts the session back to the un-authenticated state
        /// </summary>
        /// <param name="session"></param>
        public void Logout(RemotingSession session)
        {
            session.Authenticated = false;
            session.Clear();
            LogoutSessionTimeOut(session.Identification);
        }

        /// <summary>
        /// Obtaining session id according to authentication parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object GetSessionId (ARemotingAuthenticationParameters parameters)
        {
            try
            {
                return GetSessionId((T_AUTH_PARAMS)parameters);
            }
            catch { }

            return null;
        }

        private const string _implicitAuthError = "Wrong authentication parameters";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="InvalidCastException">if supplied authentication parameters are not valid for this kind of authentication</exception>
        /// <exception cref="ArgumentNullException">if the parameters argument is null or related to wrong authentication</exception>
        public void CheckValidParameters([NotNull] ARemotingAuthenticationParameters parameters)
        {
            Validator.CheckForNull(parameters,"parameters");

            parameters.Validate();
        }

        /// <summary>
        /// returns true, if the parameters are correct and valid according to current authentication method
        /// </summary>
        /// <param name="parameters"></param>
        public bool IsValidParamaters(ARemotingAuthenticationParameters parameters)
        {
            try
            {
                CheckValidParameters(parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public abstract IEnumerable<object> GetSessionsForUser(T_USER_ID userIdentification);
    }
}
