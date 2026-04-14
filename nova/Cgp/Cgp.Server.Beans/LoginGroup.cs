using System;
using System.Collections.Generic;

//using Contal.Cgp.RemotingCommon;

using Contal.IwQuick;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class LoginGroup : AOrmObject
    {
        public const string COLUMN_LOGIN_GROUP_NAME = "LoginGroupName";
        public const string COLUMN_EXPIRATIONDATE = "ExpirationDate";
        public const string COLUMN_IS_DISABLED= "IsDisabled";
        public const string COLUMN_LOGINS = "Logins";
        public const string COLUMN_ACCESS_CONTROLS = "AccessControls";
        public const string COLUMN_DESCRIPTION = "Description";

        private string _loginGroupName;
        private bool _isDisabled;
        private DateTime? _expirationDate;
        private ICollection<AccessControl> _accessControls;

        public LoginGroup()
        {
        }

        public LoginGroup(string loginGroupName, bool isDisabled, DateTime? expirationDate, bool superAdmin)
        {
            Validator.CheckNullString(loginGroupName);

            _loginGroupName = loginGroupName;
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

        public virtual Guid IdLoginGroup { get; set; }

        public virtual string LoginGroupName
        {
            get { return _loginGroupName; }
            set { _loginGroupName = value; }
        }

        public virtual bool IsDisabled
        {
            get { return _isDisabled; } 
            set { _isDisabled = value; }
        }

        public virtual DateTime? ExpirationDate
        {
            get { return _expirationDate; }
            set { _expirationDate = value; }
        }

        public virtual ICollection<Login> Logins { get; set; }

        public virtual ICollection<AccessControl> AccessControls
        {
            get { return _accessControls; }
            set { _accessControls = value; }
        }

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
            var loginGroup = obj as LoginGroup;

            return 
                loginGroup != null && 
                loginGroup.LoginGroupName == LoginGroupName;
        }

        public override string ToString()
        {
            return LoginGroupName;
        }

        public virtual void SetAccessControls(IList<Access> listAccess)
        {
            if (AccessControls != null)
                AccessControls.Clear();
            else
                AccessControls = new List<AccessControl>();

            if (listAccess != null)
                foreach (var access in listAccess)
                {
                    var accessControl = 
                        new AccessControl
                        {
                            Source = access.Source,
                            Access = access.AccessValue
                        };

                    AccessControls.Add(accessControl);
                }
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            return 
                LoginGroupName.ToLower().Contains(expression) || 
                Description != null && Description.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdLoginGroup.ToString();
        }

        public override object GetId()
        {
            return IdLoginGroup;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new LoginGroupModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.LoginGroup;
        }
    }

    [Serializable]
    public class LoginGroupShort : IShortObject
    {
        public const string COLUMN_LOGIN_GROUP_NAME = "LoginGroupName";
        public const string COLUMN_IS_DISABLED = "IsDisabled";
        public const string COLUMN_EXPIRATION_DATE = "ExpirationDate";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdLoginGroup { get; set; }
        public string LoginGroupName { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public LoginGroupShort(LoginGroup loginGroup)
        {
            IdLoginGroup = loginGroup.IdLoginGroup;
            LoginGroupName = loginGroup.LoginGroupName;
            IsDisabled = loginGroup.IsDisabled;
            ExpirationDate = loginGroup.ExpirationDate;
            Description = loginGroup.Description;
        }

        public override string ToString()
        {
            return LoginGroupName;
        }

        #region IShortObject Members

        public string Name { get { return LoginGroupName; } }

        public object Id { get { return LoginGroupName; } }

        public ObjectType ObjectType
        {
            get
            {
                return ObjectType.LoginGroup;
            }
        }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class LoginGroupModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.LoginGroup; }
        }

        public LoginGroupModifyObj(LoginGroup loginGroup)
        {
            Id = loginGroup.IdLoginGroup;
            FullName = loginGroup.ToString();
            Description = loginGroup.Description;
        }
    }
}
