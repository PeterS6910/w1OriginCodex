using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.BaseLib
{
    [Serializable]
    public class LoginAuthenticationParameters : IwQuick.Remoting.ARemotingAuthenticationParameters
    {
        private string _loginUserName;
        private string _loginPasswordHash;

        public string LoginUsername
        {
            get { return _loginUserName; }
        }

        public string LoginPasswordHash
        {
            get { return _loginPasswordHash; }
        }

        public Guid LoginId { get; set; }

        public LoginAuthenticationParameters(string userName, string password, bool hashedPwd)
        {
            _loginUserName = userName;

            if (hashedPwd)
                _loginPasswordHash = password;
            else
                _loginPasswordHash = PasswordHelper.GetPasswordhash(userName, password);
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(_loginUserName))
                throw new InvalidCastException();
        }

        public const int WRONG_USERNAME_OR_PASSWORD = 0;
        public const int LOGIN_DISABLED = 1;
        public const int WRONG_CARD = 2;
        public const int LOGIN_EXPIRED = 3;
        public const int USER_ALREADY_LOGGED = 4;
    }
}
