using System;
using System.Collections.Generic;
using System.Reflection;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class SecuritySettings :
        ABaseOrmTable<SecuritySettings, SecuritySetting>
    {
        private SecuritySettings() : base(null)
        {
        }

        public override object ParseId(string strObjectId)
        {
            byte result;

            return
                byte.TryParse(strObjectId, out result)
                    ? (object)result
                    : null;
        }

        public override bool HasAccessView(Login login)
        {
            return
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsSecuritySettingsAdmin),
                    login) ||
                AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsSecuritySettingsView),
                    login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.GeneralOptionsSecuritySettingsAdmin),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.GeneralOptionsSecuritySettingsAdmin),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.GeneralOptionsSecuritySettingsAdmin),
                login);
        }

        public IList<SecuritySettingSeverity> GetSecuritySettings(out Exception error)
        {
            ICollection<SecuritySetting> securitySettings = List(out error);
            if (error != null)
                return null;

            IList<SecuritySettingSeverity> enumSecuritySettings = new List<SecuritySettingSeverity>();

            if (securitySettings != null)
            {
                foreach (SecuritySetting securitySetting in securitySettings)
                {
                    SecuritySettingSeverity? enumSecuritySetting = null;
                    foreach (SecuritySettingSeverity enumValue in Enum.GetValues(typeof(SecuritySettingSeverity)))
                    {
                        if (securitySetting.IdEnumSecuritySetting == (byte)enumValue)
                        {
                            enumSecuritySetting = enumValue;
                            break;
                        }
                    }

                    if (enumSecuritySetting != null)
                    {
                        enumSecuritySettings.Add(enumSecuritySetting.Value);
                    }
                }
            }

            return enumSecuritySettings;
        }

        public bool IsSetSecuritySetting(SecuritySettingSeverity enumSecuritySetting)
        {
            SecuritySetting securitySetting = GetById((byte)enumSecuritySetting);
            return securitySetting != null;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.SecuritySetting; }
        }
    }
}
