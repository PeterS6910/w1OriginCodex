using System;
using Contal.IwQuick;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.Data;
using Microsoft.Win32;

using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(214)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class CCUConfigurationState
    {
        //private const string CCU_CONFIG_STATE_PATH = "NandFlash\\CCU\\Temp\\";
        //private const string CCU_CONFIG_STATE_FILENAME = "CCUConfigurationState.dat";

        private static volatile CCUConfigurationState _singleton = null;
        private static object _syncRoot = new object();

        [LwSerialize]
        private bool _isConfigured = false;
        [LwSerialize]
        private string _serverHashCode = string.Empty;

        private bool _isUpgrading = false;

        public static CCUConfigurationState Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CCUConfigurationState();
                    }

                return _singleton;
            }
        }

        private CCUConfigurationState()
        {
            _isConfigured = GetRegistryIsConfigured();
            _serverHashCode = GetRegistryServerHashCode();
            _isUpgrading = false;
        }

        public bool IsUpgrading { get { return _isUpgrading; } }
        public bool IsConfigured { get { return _isConfigured; } }
        public string ServerHashCode { get { return _serverHashCode; } }

        public void SetUpgrading()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CCUConfigurationState.SetUpgrading()");
            _isUpgrading = true;
        }

        public void SetServer(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void CCUConfigurationState.SetServer(string serverHashCode): [{0}]", Log.GetStringFromParameters(serverHashCode)));
            if (serverHashCode != null)
            {
                _isConfigured = true;
                _isNewlyConfigured = true;
                _serverHashCode = serverHashCode;
            }
            
            SetRegistryCCUConfigurationState();
            SendInfoConfiguredStateChanged();
        }

        public bool IsNewlyConfigured
        {
            get { return _isNewlyConfigured; }
        }

        public void ResetNewlyConfigured()
        {
            _isNewlyConfigured = false;
        }

        public void UnsetServer()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CCUConfigurationState.UnsetServer()");
            _isConfigured = false;
            _serverHashCode = string.Empty;
            SetRegistryCCUConfigurationState();
            SendInfoConfiguredStateChanged();
        }

        private void SendInfoConfiguredStateChanged()
        {
            CcuCoreRemotingProvider.Singleton.DoCcuConfiguredStateChanged(_isConfigured, _serverHashCode);
        }

        internal void UnsetUpgrading()
        {
            _isUpgrading = false;
        }

        private string CCU_REG_SERVER_HASH_CODE = "CcuServerHashCode";
        private string CCU_REG_IS_CONFIGURED = "CcuIsConfigured";

        private bool _isNewlyConfigured;

        private void SetRegistryCCUConfigurationState()
        {
            SetRegistryServerHashCode();
            SetRegistryIsConfigured();
        }

        public string GetRegistryServerHashCode()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string CCUConfigurationState.GetRegistryServerHashCode()");
            try
            {
                RegistryKey registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    string result = Convert.ToString(registryKey.GetValue(CCU_REG_SERVER_HASH_CODE));
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("string CCUConfigurationState.GetRegistryServerHashCode return {0}[1]", Log.GetStringFromParameters(result)));
                    return result;
                }
            }
            catch
            {
                CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL,()=>string.Format("string CCUConfigurationState.GetRegistryServerHashCode return {0}[1]", Log.GetStringFromParameters(string.Empty)));
                return string.Empty;
            }
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("string CCUConfigurationState.GetRegistryServerHashCode return {0}[2]", Log.GetStringFromParameters(string.Empty)));
            return string.Empty;
        }

        private void SetRegistryServerHashCode()
        {
            try
            {
                RegistryKey registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(CCU_REG_SERVER_HASH_CODE, _serverHashCode, RegistryValueKind.String);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public bool GetRegistryIsConfigured()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CCUConfigurationState.GetRegistryIsConfigured()");
            try
            {
                RegistryKey registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    bool result = Convert.ToBoolean(registryKey.GetValue(CCU_REG_IS_CONFIGURED));
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CCUConfigurationState.GetRegistryIsConfigured return {0}[1]", Log.GetStringFromParameters(result)));
                    return result;
                }
            }
            catch
            {
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL,"bool CCUConfigurationState.GetRegistryIsConfigured return false[1]");
                return false;
            }
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CCUConfigurationState.GetRegistryIsConfigured return false[2]");
            return false;
        }

        private void SetRegistryIsConfigured()
        {
            try
            {
                RegistryKey registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(CCU_REG_IS_CONFIGURED, _isConfigured, RegistryValueKind.DWord);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }
    }
}