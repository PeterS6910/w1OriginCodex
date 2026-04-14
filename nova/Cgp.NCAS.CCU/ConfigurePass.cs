using System;
using Contal.IwQuick;
using Contal.IwQuick.Sys.Microsoft;
using Microsoft.Win32;

namespace Contal.Cgp.NCAS.CCU
{
    public class ConfigurePass
    {
        public bool CompareConfigurePassword(string password)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool ConfigurePass.CompareConfigurePassword(string password): [{0}]", Log.GetStringFromParameters(password)));

            string psw = GetRegistryConfigurePassword();
            if (string.IsNullOrEmpty(psw))
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool ConfigurePass.CompareConfigurePassword return true[1]");
                return true;
            }
            if (psw == password)
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool ConfigurePass.CompareConfigurePassword return true[2]");
                return true;
            }
            else
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool ConfigurePass.CompareConfigurePassword return false[1]");
                return false;
            }
        }

        public void SaveConfigurePassword(string newPassword)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void ConfigurePass.SaveConfigurePassword(string newPassword): [{0}]", Log.GetStringFromParameters(newPassword)));
            RegistryHelper.TrySetValue(CcuCore.REGISTRY_CCU_PATH, CCU_CONFIGURE_PASSWORD, newPassword, RegistryValueKind.String);
        }

        public bool IsUsedCcuConfigurePassword()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool ConfigurePass.IsUsedCcuConfigurePassword()");
            string psw = GetRegistryConfigurePassword();
            bool result = false;
            if (psw != string.Empty)
                result = true;            
            else
                result = false;
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool ConfigurePass.IsUsedCcuConfigurePassword return {0}", Log.GetStringFromParameters(result)));

            return result;
        }

        private const string CCU_CONFIGURE_PASSWORD = @"ConfigurePassword";

        public string GetRegistryConfigurePassword()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string ConfigurePass.GetRegistryConfigurePassword()");
            string passwordHash = String.Empty;
            RegistryHelper.TryGetValue(CcuCore.REGISTRY_CCU_PATH, CCU_CONFIGURE_PASSWORD, ref passwordHash);

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("string ConfigurePass.GetRegistryConfigurePassword return {0}", Log.GetStringFromParameters(passwordHash)));
            return passwordHash;
        }
    }
}
