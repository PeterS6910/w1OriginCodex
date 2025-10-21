using System;
using System.Collections.Generic;

//using Contal.Cgp.RemotingCommon;
using System.Linq;

using Contal.IwQuick;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class Login : AOrmObject, IOrmObjectWithAlarmInstructions
    {
        private string _username;
        private string _passwordHash;
        private LoginGroup _loginGroup;
        private bool _isDisabled;
        private DateTime? _expirationDate;
        private ICollection<AccessControl> _accessControls;
        public const string COLUMN_USERNAME = "Username";
        public const string COLUMN_PASSWORD_HASH = "PasswordHash";
        public const string COLUMN_PUBLIC_KEY = "PublicKey";
        public const string COLUMN_IS_DISABLED = "IsDisabled";
        public const string COLUMN_EXPIRATION_DATE = "ExpirationDate";
        public const string COLUMN_MUST_CHANGE_PASSWORD = "MustChangePassword";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_LOGIN_GROUP = "LoginGroup";
        public const string COLUMN_PERSON = "Person";
        public const string COLUMN_ACCESS_CONTROLS = "AccessControls";
        public const string COLUMN_QUICK_MENU = "QuickMenu";
        public const string COLUMN_USER_OPENED_WINDOWS = "UserOpenedWindows";
        public const string COLUMN_LAST_PASSWORD_CHANGE_DATE = "LastPasswordChangeDate";
        public const string COLUMN_CLIENT_LANGUAGE = "ClientLanguage";
        public const string COLUMN_LOCAL_ALARM_INSTRUCTION = "LocalAlarmInstruction";

        public Login()
        {
        }

        public Login(string username, string passwordHash, LoginGroup loginGroup, bool isDisabled, DateTime? expirationDate, bool superAdmin)
        {
            Validator.CheckNullString(username);
            Validator.CheckNullString(passwordHash);

            _username = username;
            _passwordHash = passwordHash;
            _loginGroup = loginGroup;
            _isDisabled = isDisabled;
            _expirationDate = expirationDate;

            if (superAdmin)
            {
                var accessControl = new AccessControl();
                accessControl.Access = (int)LoginAccess.SuperAdmin;

                if (_accessControls == null)
                    _accessControls = new List<AccessControl>();

                _accessControls.Add(accessControl);
            }
        }

        public virtual Guid IdLogin { get; set; }

        public virtual string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
            }
        }

        public virtual string PasswordHash
        {
            get
            {
                return _passwordHash;
            }

            set
            {
                _passwordHash = value;
            }
        }

        public virtual string PublicKey { get; set; }

        public virtual bool IsDisabled
        {
            get
            {
                return _isDisabled;
            }

            set
            {
                _isDisabled = value;
            }
        }

        public virtual DateTime? ExpirationDate
        {
            get
            {
                return _expirationDate;
            }

            set
            {
                _expirationDate = value;
            }
        }

        //public virtual IDictionary<string, int> QuickMenu { get; set; }
        public virtual bool MustChangePassword { get; set; }
        public virtual DateTime? LastPasswordChangeDate { get; set; }
        public virtual string ClientLanguage { get; set; }
        public virtual string LocalAlarmInstruction { get; set; }

        public virtual LoginGroup LoginGroup
        {
            get
            {
                return _loginGroup;
            }

            set
            {
                _loginGroup = value;
            }
        }

        public virtual Person Person { get; set; }

        public virtual ICollection<AccessControl> AccessControls
        {
            get
            {
                return _accessControls;
            }

            set
            {
                _accessControls = value;
            }
        }

        public virtual ICollection<UserOpenedWindow> UserOpenedWindows { get; set; }

        public virtual string Description { get; set; }

        public virtual byte ObjectType
        {
            get
            {
                return (byte)GetObjectType();
            }

            set
            {
                
            }
        }

        public override bool Compare(object obj)
        {
            var login = obj as Login;
            return login != null && login.Username == Username;
        }

        public override string ToString()
        {
            return Username;
        }

        public virtual void SetAccessControls(IList<Access> listAccess)
        {
            if (AccessControls != null)
                AccessControls.Clear();
            else
                AccessControls = new List<AccessControl>();

            if (listAccess != null)
                foreach (Access access in listAccess)
                    AccessControls.Add(
                        new AccessControl
                        {
                            Source = access.Source,
                            Access = access.AccessValue
                        });
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            return 
                Username.ToLower().Contains(expression) || 
                    Description != null && 
                    Description.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdLogin.ToString();
        }

        public override object GetId()
        {
            return IdLogin;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new LoginModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.Login;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        public static bool HasAccess(
            ICollection<AccessControl> accessControls, 
            Access access)
        {
            if (access == null || accessControls == null || accessControls.Count == 0)
                return false;

            return 
                accessControls
                    .Any(
                        accessControl => 
                            accessControl.Source == access.Source && 
                            (accessControl.Access == access.AccessValue || 
                                (access.GeneralAccessValue != null && 
                                    accessControl.Access == access.GeneralAccessValue.Value)));
        }
    }

    [Serializable]
    public class LoginShort : IShortObject
    {
        public const string COLUMN_SYMBOL = "Symbol";
        public const string COLUMN_USERNAME = "Username";
        public const string COLUMN_LOGIN_GROUP = "LoginGroup";
        public const string COLUMN_IS_DISABLED = "IsDisabled";
        public const string COLUMN_EXPIRATION_DATE = "ExpirationDate";
        public const string COLUMN_MUST_CHANGE_PASSWORD = "MustChangePassword";
        public const string COLUMN_CLIENT_LANGUAGE = "ClientLanguage";
        public const string COLUMN_DESCRIPTION = "Description";

        public Image Symbol { get; set; }
        public Guid IdLogin { get; set; }
        public string Username { get; set; }
        public string LoginGroup { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool MustChangePassword { get; set; }
        public string ClientLanguage { get; set; }
        public string Description { get; set; }

        public LoginShort(Login login)
        {
            IdLogin = login.IdLogin;
            Username = login.Username;
            
            IsDisabled = login.LoginGroup != null
                ? login.LoginGroup.IsDisabled
                  || login.IsDisabled
                : login.IsDisabled;

            ExpirationDate = login.ExpirationDate;
            MustChangePassword = login.MustChangePassword;
            ClientLanguage = login.ClientLanguage;
            LoginGroup = login.LoginGroup != null ? login.LoginGroup.ToString() : string.Empty;
            Description = login.Description;
        }
        
        public override string ToString()
        {
            return Username;
        }

        #region IShortObject Members

        public object Id { get { return IdLogin; } }

        public string Name { get { return Username; } }

        public ObjectType ObjectType { get { return ObjectType.Login; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class LoginModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.Login; } }

        public LoginModifyObj(Login login)
        {
            Id = login.IdLogin;
            FullName = login.Username;
            Description = login.Description;
        }
    }
                    }
